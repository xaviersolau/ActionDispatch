// ----------------------------------------------------------------------
// <copyright file="Dispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core.Impl.Action;
using SoloX.ActionDispatch.Core.Impl.Utils;

namespace SoloX.ActionDispatch.Core.Impl
{
    /// <summary>
    /// Implementation of the IDispatcher.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public class Dispatcher<TRootState> : IDispatcher<TRootState>, IDisposable
    {
        private readonly object syncObject = new object();

        private readonly ILogger<Dispatcher<TRootState>> logger;
        private readonly BehaviorSubject<TRootState> state;
        private readonly Subject<IAction<TRootState>> action;
        private readonly Subject<IAction<TRootState>> listenerActions;

        private readonly List<IAction<TRootState>> actionsToDispatch = new List<IAction<TRootState>>();
        private readonly List<IDisposable> listenerActionSubscriptions = new List<IDisposable>();
        private readonly IDisposable actionSubscription;

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher{TRootState}"/> class.
        /// </summary>
        /// <param name="initialState">The initial state to use.</param>
        /// <param name="logger">The logger to use for logging.</param>
        public Dispatcher(TRootState initialState, ILogger<Dispatcher<TRootState>> logger)
        {
            this.logger = logger;
            this.state = new BehaviorSubject<TRootState>(initialState);
            this.action = new Subject<IAction<TRootState>>();
            this.listenerActions = new Subject<IAction<TRootState>>();
            this.actionSubscription = this.action.AsObservable().Subscribe(this.ActionSubscriber);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Dispatcher{TRootState}"/> class.
        /// </summary>
        ~Dispatcher()
        {
            this.Dispose(false);
        }

        /// <inheritdoc />
        public IObservable<TRootState> State => this.state.AsObservable();

        /// <inheritdoc />
        public void Dispatch(IAction<TRootState> action)
        {
            var actionBase = (ActionBase<TRootState>)action;

            if (actionBase.State != ActionState.None)
            {
                throw new ArgumentException("Error: Action state must be None in order to be dispatched.");
            }

            IAction<TRootState>[] postDispatchRequests;

            lock (this.syncObject)
            {
                this.action.OnNext(actionBase);
                postDispatchRequests = this.actionsToDispatch.ToArray();
                this.actionsToDispatch.Clear();
            }

            if (actionBase.State == ActionState.Success)
            {
                this.listenerActions.OnNext(actionBase);
            }

            actionBase.State = ActionState.None;

            foreach (var postDispatchRequest in postDispatchRequests)
            {
                this.Dispatch(postDispatchRequest);
            }
        }

        /// <inheritdoc />
        public void AddObserver(Func<IObservable<IAction<TRootState>>, IObservable<IAction<TRootState>>> observer)
        {
            var subscription = observer(this.listenerActions.AsObservable())
                .CatchAndContinue<IAction<TRootState>, Exception>(e =>
                {
                    // we can use Dispatch as action listener are used outside of the _syncObject lock monitor
                    this.logger.LogError(e, $"ERROR in action listener");
                    this.Dispatch(new UnhandledExceptionAction<TRootState>(e));
                })
                .Subscribe(action =>
                {
                    var actionBase = (ActionBase<TRootState>)action;
                    if (actionBase.State == ActionState.None)
                    {
                        Task.Run(() => this.Dispatch(actionBase), CancellationToken.None)
                            .ContinueWith(
                                (t) =>
                                {
                                    // dispatch an UnhandledExceptionAction (since we are not in the lock monitor we can call Dispatch directly)
                                    this.logger.LogError(t.Exception, "ERROR while dispatching actions from observer");
                                    this.Dispatch(new UnhandledExceptionAction<TRootState>(t.Exception));
                                },
                                CancellationToken.None,
                                TaskContinuationOptions.OnlyOnFaulted,
                                TaskScheduler.Current);
                    }
                });
            this.listenerActionSubscriptions.Add(subscription);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose method.
        /// </summary>
        /// <param name="isDispose">Tells if the method is called by Dispose method (finalizer otherwise).</param>
        protected virtual void Dispose(bool isDispose)
        {
            if (isDispose && !this.isDisposed)
            {
                this.isDisposed = true;
                this.state.Dispose();
                this.action.Dispose();
                this.listenerActions.Dispose();

                this.listenerActionSubscriptions.ForEach(subscription => subscription.Dispose());
                this.listenerActionSubscriptions.Clear();
            }
        }

        private void PostDispatch(IAction<TRootState> action)
        {
            this.actionsToDispatch.Add(action);
        }

        private void ActionSubscriber(IAction<TRootState> action)
        {
            var actionBase = (ActionBase<TRootState>)action;
            try
            {
                var oldState = this.state.Value;
                var newState = actionBase.Apply(this, oldState);
                actionBase.State = ActionState.Success;
                if (newState != null && !ReferenceEquals(oldState, newState))
                {
                    this.state.OnNext(newState);
                }
            }
            catch (Exception e)
            {
                actionBase.State = ActionState.Failed;
                this.logger.LogError(e, "ERROR in action subscription");

                // We are already in a dispatch operation so we need to post a dispatch in order to properly
                // terminate the current one inside the _syncObject lock monitor.
                this.PostDispatch(new UnhandledExceptionAction<TRootState>(e));
            }
        }
    }
}
