// ----------------------------------------------------------------------
// <copyright file="SynchronizedDispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    /// <summary>
    /// Dispatcher implementation using a synchronization context to process actions.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public class SynchronizedDispatcher<TRootState> : IDispatcher<TRootState>
        where TRootState : IState
    {
        private readonly IDispatcher<TRootState> dispatcher;
        private readonly SynchronizationContext synchronizationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="SynchronizedDispatcher{TRootState}"/> class.
        /// </summary>
        /// <param name="dispatcher">The base dispatcher that will actually dispatch actions.</param>
        /// <param name="synchronizationContext">The synchronization context to use to synchronize actions.</param>
        public SynchronizedDispatcher(IDispatcher<TRootState> dispatcher, SynchronizationContext synchronizationContext)
        {
            this.dispatcher = dispatcher;
            this.synchronizationContext = synchronizationContext;
        }

        /// <inheritdoc/>
        public IObservable<TRootState> State => this.dispatcher.State;

        /// <inheritdoc/>
        public void AddObserver(Func<IObservable<IAction<TRootState, IActionBehavior>>, IObservable<IAction<TRootState, IActionBehavior>>> observer)
        {
            this.dispatcher.AddObserver(observer);
        }

        /// <inheritdoc/>
        public IRelativeDispatcher<TState> CreateRelativeDispatcher<TState>(Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            return new RelativeDispatcher<TRootState, TState>(this, selector);
        }

        /// <inheritdoc/>
        public void Dispatch<TState>(IActionBehavior<TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            this.synchronizationContext.Send(
                (s) =>
                {
                    this.dispatcher.Dispatch(actionBehavior, selector);
                },
                null);
        }

        /// <inheritdoc/>
        public void Dispatch<TState>(IActionBehaviorAsync<TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState
        {
            this.synchronizationContext.Post(
                (s) =>
                {
                    this.dispatcher.Dispatch(actionBehavior, selector);
                },
                null);
        }
    }
}
