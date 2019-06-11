// ----------------------------------------------------------------------
// <copyright file="AsyncAction.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action.Impl
{
    /// <inheritdoc/>
    internal sealed class AsyncAction<TRootState, TState> : AAction<TRootState, TState>, IAction<TRootState, IActionBehaviorAsync<TState>>
        where TRootState : IState
        where TState : IState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncAction{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="behavior">The action behavior.</param>
        /// <param name="stateSelector">The action state selector expression.</param>
        public AsyncAction(
            IActionBehaviorAsync<TState> behavior,
            Expression<Func<TRootState, TState>> stateSelector)
            : base(stateSelector)
        {
            this.Behavior = behavior;
        }

        /// <inheritdoc/>
        public IActionBehaviorAsync<TState> Behavior { get; }

        /// <inheritdoc/>
        public override TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState state)
        {
            // Get the target state object. It is locked so we won't be able to write it.
            var targetState = this.SelectState(state);

            Task.Run(
                async () =>
                {
                    var relativeDispatcher = new RelativeDispatcher<TRootState, TState>(dispatcher, this.Selector);

                    // This will run outside of the lock dispatcher monitor.
                    await this.Behavior.Apply(relativeDispatcher, targetState).ConfigureAwait(false);
                },
                CancellationToken.None)
                .ContinueWith(
                t =>
                {
                    // dispatch an UnhandledExceptionAction (since we are not in the lock monitor we can call Dispatch directly)
                    dispatcher.Dispatch(new UnhandledExceptionBehavior<TRootState>(t.Exception), s => s);
                },
                CancellationToken.None,
                TaskContinuationOptions.OnlyOnFaulted,
                TaskScheduler.Current);

            return default;
        }
    }
}
