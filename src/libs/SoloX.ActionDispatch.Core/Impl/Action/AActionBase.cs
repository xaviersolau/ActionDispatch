﻿// ----------------------------------------------------------------------
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
        where TRootState : IState<TRootState>
    {
        /// <summary>
        /// Gets or sets action state.
        /// </summary>
        public ActionState State { get; set; }

        /// <inheritdoc/>
        public abstract TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState rootState);
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <inheritdoc/>
    internal abstract class AActionBase<TRootState, TState> : AActionBase<TRootState>
        where TRootState : IState<TRootState>
        where TState : IState<TState>
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
        private LambdaExpression Selector { get; }

        /// <summary>
        /// Select the target state.
        /// </summary>
        /// <param name="rootState">The root state.</param>
        protected TState SelectState(TRootState rootState)
        {
            return this.selectorFunc(rootState);
        }

        /// <summary>
        /// Select the target state within a transaction.
        /// </summary>
        /// <param name="rootState">The root state.</param>
        protected ITransactionalState<TState> SelectStateTransaction(TRootState rootState)
        {
            var targetState = this.selectorFunc(rootState);
            return targetState.CreateTransactionalState();
        }
    }
}
