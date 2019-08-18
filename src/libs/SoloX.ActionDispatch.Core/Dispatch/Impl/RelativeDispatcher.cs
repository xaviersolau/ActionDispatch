// ----------------------------------------------------------------------
// <copyright file="RelativeDispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;
using SoloX.ExpressionTools.Transform;
using SoloX.ExpressionTools.Transform.Impl;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    /// <summary>
    /// Implement the relative dispatcher.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    /// <typeparam name="TIntermediatState">Type of the intermediate state object on witch actions will apply.</typeparam>
    public class RelativeDispatcher<TRootState, TIntermediatState> : IRelativeDispatcher<TIntermediatState>
        where TRootState : IState
        where TIntermediatState : IState
    {
        private Expression<Func<TRootState, TIntermediatState>> baseSelector;
        private Func<TRootState, TIntermediatState> selectorFunc;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeDispatcher{TRootState, TIntermediatState}"/> class.
        /// </summary>
        /// <param name="dispatcher">The base application dispatcher.</param>
        /// <param name="baseSelector">The base selector that is driving to the intermediate state.</param>
        public RelativeDispatcher(IDispatcher<TRootState> dispatcher, Expression<Func<TRootState, TIntermediatState>> baseSelector)
        {
            if (baseSelector == null)
            {
                throw new ArgumentNullException($"The argument {nameof(baseSelector)} was null.");
            }

            this.Dispatcher = dispatcher;
            this.baseSelector = baseSelector;
            this.selectorFunc = baseSelector.Compile();
        }

        /// <summary>
        /// Gets the root dispatcher.
        /// </summary>
        public IDispatcher<TRootState> Dispatcher { get; }

        /// <inheritdoc/>
        public IObservable<TIntermediatState> State => this.Dispatcher.State.Select(this.selectorFunc);

        /// <inheritdoc/>
        public void Dispatch<TState>(IActionBehavior<TState> actionBehavior, Expression<Func<TIntermediatState, TState>> selector)
            where TState : IState
        {
            var absolutSelector = this.ComputeAbsolutSelector(selector);

            this.Dispatcher.Dispatch(actionBehavior, absolutSelector);
        }

        /// <inheritdoc/>
        public void Dispatch<TState>(IActionBehaviorAsync<TState> actionBehavior, Expression<Func<TIntermediatState, TState>> selector)
            where TState : IState
        {
            var absolutSelector = this.ComputeAbsolutSelector(selector);

            this.Dispatcher.Dispatch(actionBehavior, absolutSelector);
        }

        private Expression<Func<TRootState, TState>> ComputeAbsolutSelector<TState>(Expression<Func<TIntermediatState, TState>> selector)
            where TState : IState
        {
            var resolver = new ParameterResolver(this.baseSelector);
            var inliner = new ExpressionInliner(resolver);

            return inliner.Amend<Func<TIntermediatState, TState>, Func<TRootState, TState>>(selector);
        }

        private class ParameterResolver : IParameterResolver
        {
            private LambdaExpression baseSelector;

            public ParameterResolver(LambdaExpression baseSelector)
            {
                this.baseSelector = baseSelector;
            }

            public LambdaExpression Resolve(ParameterExpression parameter)
            {
                return this.baseSelector;
            }
        }
    }
}
