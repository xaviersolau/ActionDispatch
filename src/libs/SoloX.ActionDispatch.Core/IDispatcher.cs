// ----------------------------------------------------------------------
// <copyright file="IDispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ActionDispatch.Core
{
    /// <summary>
    /// IDispatcher interface. Used to dispatch IAction that will change the root state object hierarchy.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IDispatcher<TRootState>
        where TRootState : IState<TRootState>
    {
        /// <summary>
        /// Gets current state.
        /// </summary>
        IObservable<TRootState> State { get; }

        /// <summary>
        /// Dispatch an action on a current state.
        /// </summary>
        /// <typeparam name="TState">The target state type.</typeparam>
        /// <param name="actionBehavior">The action behavior to apply.</param>
        /// <param name="selector">The target state selector expression.</param>
        void Dispatch<TState>(IActionBehavior<TRootState, TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState<TState>;

        /// <summary>
        /// Dispatch an action on a current state.
        /// </summary>
        /// <typeparam name="TState">The target state type.</typeparam>
        /// <param name="actionBehavior">The asynchronous action behavior to apply.</param>
        /// <param name="selector">The target state selector expression.</param>
        void Dispatch<TState>(IActionBehaviorAsync<TRootState, TState> actionBehavior, Expression<Func<TRootState, TState>> selector)
            where TState : IState<TState>;

        /// <summary>
        /// Add an action observer.
        /// </summary>
        /// <param name="observer">The observer to add.</param>
        void AddObserver(Func<IObservable<IAction<TRootState, IActionBehavior>>, IObservable<IAction<TRootState, IActionBehavior>>> observer);
    }
}
