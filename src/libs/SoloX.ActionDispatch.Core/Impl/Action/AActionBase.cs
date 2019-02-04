// ----------------------------------------------------------------------
// <copyright file="AActionBase.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <inheritdoc/>
    internal abstract class AActionBase<TRootState> : IAction<TRootState>
    {
        /// <summary>
        /// Gets or sets action state.
        /// </summary>
        public ActionState State { get; set; }

        /// <inheritdoc/>
        public abstract TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState state);
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <inheritdoc/>
    internal abstract class AActionBase<TRootState, TState> : AActionBase<TRootState>
#pragma warning restore SA1402 // File may only contain a single type
    {
        private Func<TRootState, TState> selectorFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AActionBase{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="stateSelector">The action state selector expression.</param>
        protected AActionBase(Expression<Func<TRootState, TState>> stateSelector)
        {
            this.Selector = stateSelector;

            this.selectorFunc = stateSelector.Compile();
        }

        /// <summary>
        /// Gets action state selector expression.
        /// </summary>
        internal LambdaExpression Selector { get; }

        /// <summary>
        /// Gets and clones the target state.
        /// </summary>
        /// <param name="state">The root state.</param>
        /// <returns>The cloned target state.</returns>
        protected TState GetAndCloneTargetState(TRootState state)
        {
            var oldTargetState = this.selectorFunc(state);
            return oldTargetState;
        }
    }
}
