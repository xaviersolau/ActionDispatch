// ----------------------------------------------------------------------
// <copyright file="Dispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.Utils;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    /// <summary>
    /// Implementation of the IDispatcher.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public class Dispatcher<TRootState> : IDispatcher<TRootState>, IDisposable
        where TRootState : IState
    {
        private readonly object syncObject = new object();

        private readonly ILogger<Dispatcher<TRootState>> logger;

        private readonly BehaviorSubject<TRootState> state;

        private readonly List<ActionMiddlewareSubject<TRootState>> actionMiddlewareSubjects =
            new List<ActionMiddlewareSubject<TRootState>>();

        private readonly List<IAction<TRootState, IActionBehavior>> actionsToDispatch =
            new List<IAction<TRootState, IActionBehavior>>();

        private readonly List<Action<IAction<TRootState, IActionBehavior>>> observers =
            new List<Action<IAction<TRootState, IActionBehavior>>>();

        private readonly ICallingStrategy callingStrategy;

        private Action<IAction<TRootState, IActionBehavior>> postDispatch;

        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dispatcher{TRootState}"/> class.
        /// </summary>
        /// <param name="initialState">The initial state to use.</param>
        /// <param name="logger">The logger to use for logging.</param>
        /// <param name="callingStrategy">The calling strategy to use to dispatch actions.</param>
        public Dispatcher(TRootState initialState, ILogger<Dispatcher<TRootState>> logger, ICallingStrategy callingStrategy = null)
        {
            this.logger = logger;
            this.callingStrategy = callingStrategy ?? new DefaultCallingStrategy();
            this.state = new BehaviorSubject<TRootState>(initialState);
            this.postDispatch = this.PostDispatchUnlocked;
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
        public void Dispatch<TState>(IActionBehavior<TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            this.callingStrategy.Invoke(
                () => this.Dispatch(new SyncAction<TRootState, TState>(actionBehavior, selector)));
        }

        /// <inheritdoc />
        public void Dispatch<TState>(IActionBehaviorAsync<TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            this.callingStrategy.Invoke(
                () => this.Dispatch(new AsyncAction<TRootState, TState>(actionBehavior, selector)));
        }

        /// <inheritdoc />
        public IRelativeDispatcher<TState> CreateRelativeDispatcher<TState>(Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            return new RelativeDispatcher<TRootState, TState>(this, selector);
        }

        /// <inheritdoc />
        public void AddMidlleware(IActionMiddleware<TRootState> middleware)
        {
            if (middleware == null)
            {
                throw new ArgumentNullException($"The argument {nameof(middleware)} was null.");
            }

            this.actionMiddlewareSubjects.Add(
                new ActionMiddlewareSubject<TRootState>(
                    middleware,
                    action =>
                    {
                        this.callingStrategy.Invoke(
                            () => this.Publish(action));
                    },
                    e =>
                    {
                        this.callingStrategy.Invoke(
                            () => this.PostDispatch(CreateUnhandledExceptionAction(e)));
                    }));
        }

        /// <inheritdoc />
        public void AddObserver(Action<IAction<TRootState, IActionBehavior>> observer)
        {
            if (observer == null)
            {
                throw new ArgumentNullException($"The argument {nameof(observer)} was null.");
            }

            this.observers.Add(observer);
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

                this.actionMiddlewareSubjects.ForEach(subject => subject.Dispose());
                this.actionMiddlewareSubjects.Clear();
            }
        }

        private static IAction<TRootState, IActionBehavior> CreateUnhandledExceptionAction(Exception exception)
        {
            return new SyncAction<TRootState, TRootState>(new UnhandledExceptionBehavior<TRootState>(exception), s => s);
        }

        /// <summary>
        /// Dispatch an action on a current state.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        private void Dispatch(IAction<TRootState, IActionBehavior> action)
        {
            IAction<TRootState, IActionBehavior>[] postDispatchRequests;

            lock (this.syncObject)
            {
                this.postDispatch = this.PostDispatchLocked;
                var done = false;
                foreach (var actionMiddlewareSubject in this.actionMiddlewareSubjects)
                {
                    done = actionMiddlewareSubject.Dispatch(action);
                    if (done)
                    {
                        // The action has been dispatched by the middle ware so we can break.
                        break;
                    }
                }

                if (!done)
                {
                    this.Publish(action);
                }

                postDispatchRequests = this.actionsToDispatch.ToArray();
                this.actionsToDispatch.Clear();
                this.postDispatch = this.PostDispatchUnlocked;
            }

            foreach (var postDispatchRequest in postDispatchRequests)
            {
                this.Dispatch(postDispatchRequest);
            }
        }

        private void Publish(IAction<TRootState, IActionBehavior> action)
        {
            var actionBase = (AAction<TRootState>)action;
            try
            {
                lock (this.syncObject)
                {
                    var oldState = this.state.Value;
                    var oldVersion = oldState.Version;
                    var newState = action.Apply(this, oldState);
                    actionBase.State = ActionState.Success;
                    if (newState != null && newState.Version != oldVersion)
                    {
                        this.state.OnNext(newState);
                    }

                    this.TriggerObservers(action);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
            {
                actionBase.State = ActionState.Failed;
                this.logger.LogError(e, "ERROR in action subscription");

                // Check we are not already in an UnhandledExceptionAction to avoid a recursive exception handling.
                if (!(action.Behavior is UnhandledExceptionBehavior<TRootState>))
                {
                    // We are already in a dispatch operation so we need to post a dispatch in order to properly
                    // terminate the current one inside the syncObject lock monitor.
                    this.PostDispatch(CreateUnhandledExceptionAction(e));
                }
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        private void PostDispatch(IAction<TRootState, IActionBehavior> action)
        {
            lock (this.syncObject)
            {
                this.postDispatch(action);
            }
        }

        private void PostDispatchLocked(IAction<TRootState, IActionBehavior> action)
        {
            this.actionsToDispatch.Add(action);
        }

        private void PostDispatchUnlocked(IAction<TRootState, IActionBehavior> action)
        {
            this.Dispatch(action);
        }

        private void TriggerObservers(IAction<TRootState, IActionBehavior> action)
        {
            this.observers.ForEach(observer =>
            {
                try
                {
                    observer(action);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
                {
                    this.PostDispatch(CreateUnhandledExceptionAction(e));
                }
#pragma warning restore CA1031 // Do not catch general exception types
            });
        }
    }
}
