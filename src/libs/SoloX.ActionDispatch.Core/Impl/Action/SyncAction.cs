// ----------------------------------------------------------------------
// <copyright file="SyncAction.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <inheritdoc/>
    internal sealed class SyncAction<TRootState, TState> : AAction<TRootState, TState>, IAction<TRootState, IActionBehavior<TRootState, TState>>
        where TRootState : IState<TRootState>
        where TState : IState<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncAction{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="behavior">The action behavior.</param>
        /// <param name="stateSelector">The action state selector expression.</param>
        public SyncAction(
            IActionBehavior<TRootState, TState> behavior,
            Expression<Func<TRootState, TState>> stateSelector)
            : base(stateSelector)
        {
            this.Behavior = behavior;
        }

        /// <inheritdoc/>
        public IActionBehavior<TRootState, TState> Behavior { get; }

        /// <inheritdoc/>
        public override TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState rootState)
        {
            using (var stateTransaction = this.SelectStateTransaction(rootState))
            {
                this.Behavior.Apply(stateTransaction.State);

                var patched = stateTransaction.Patch(rootState);
                patched.Lock();

                return patched;
            }
        }
    }
}
