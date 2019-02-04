// ----------------------------------------------------------------------
// <copyright file="ActionBase.cs" company="SoloX Software">
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
    internal sealed class ActionBase<TRootState, TState> : AActionBase<TRootState, TState>, IAction<TRootState, IActionBehavior<TRootState, TState>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="behavior">The action behavior.</param>
        /// <param name="stateSelector">The action state selector expression.</param>
        public ActionBase(
            IActionBehavior<TRootState, TState> behavior,
            Expression<Func<TRootState, TState>> stateSelector)
            : base(stateSelector)
        {
            this.Behavior = behavior;
        }

        /// <inheritdoc/>
        public IActionBehavior<TRootState, TState> Behavior { get; }

        /// <inheritdoc/>
        public override TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState state)
        {
            var oldActionState = this.GetAndCloneTargetState(state);

            // TODO implement clone and patch state.
            this.Behavior.Apply(oldActionState);

            return default;
        }
    }
}
