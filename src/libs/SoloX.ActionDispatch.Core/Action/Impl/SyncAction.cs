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
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Core.Action.Impl
{
    /// <inheritdoc/>
    internal sealed class SyncAction<TRootState, TState> : AAction<TRootState, TState>, IAction<TRootState, IActionBehavior<TState>>
        where TRootState : IState
        where TState : IState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyncAction{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="behavior">The action behavior.</param>
        /// <param name="stateSelector">The action state selector expression.</param>
        public SyncAction(
            IActionBehavior<TState> behavior,
            Expression<Func<TRootState, TState>> stateSelector)
            : base(stateSelector)
        {
            this.Behavior = behavior;
        }

        /// <inheritdoc/>
        public IActionBehavior<TState> Behavior { get; }

        /// <inheritdoc/>
        public override TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState rootState)
        {
            var targetStateItf = this.SelectState(rootState);

            if (targetStateItf == null)
            {
                throw new NullReferenceException("The target state is null");
            }

            // Get the target state object. It is locked so we won't be able to write it.
            var targetState = targetStateItf.ToStateBase();

            var stateContainer = new StateContainer<TState>(targetState);

            this.Behavior.Apply(stateContainer);

            if (!stateContainer.IsEmpty)
            {
                var patched = rootState.ToStateBase().Patch(targetState, stateContainer.State?.ToStateBase()).Identity;

                patched.Lock();

                return patched;
            }
            else
            {
                return default;
            }
        }
    }
}
