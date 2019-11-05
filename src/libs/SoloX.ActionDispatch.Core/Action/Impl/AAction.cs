// ----------------------------------------------------------------------
// <copyright file="AAction.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action.Impl
{
    /// <inheritdoc/>
    internal abstract class AAction<TRootState> : IAction<TRootState>
        where TRootState : IState
    {
        /// <inheritdoc/>
        public abstract TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState rootState);
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <inheritdoc/>
    internal abstract class AAction<TRootState, TState> : AAction<TRootState>
        where TRootState : IState
        where TState : IState
#pragma warning restore SA1402 // File may only contain a single type
    {
        private Func<TRootState, TState> selectorFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="AAction{TRootState, TState}"/> class.
        /// </summary>
        /// <param name="stateSelector">The action state selector expression.</param>
        protected AAction(Expression<Func<TRootState, TState>> stateSelector)
        {
            this.Selector = stateSelector;

            this.selectorFunc = stateSelector.Compile();
        }

#pragma warning disable CA1822 // Member RootStateType does not access instance data and can be marked as static
        /// <summary>
        /// Gets the action Root state type.
        /// </summary>
        internal Type RootStateType => typeof(TRootState);
#pragma warning restore CA1822 // Member RootStateType does not access instance data and can be marked as static

        /// <summary>
        /// Gets action state selector expression.
        /// </summary>
        internal Expression<Func<TRootState, TState>> Selector { get; }

        /// <summary>
        /// Select the target state.
        /// </summary>
        /// <param name="rootState">The root state.</param>
        protected TState SelectState(TRootState rootState)
        {
            return this.selectorFunc(rootState);
        }
    }
}
