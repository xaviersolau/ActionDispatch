// ----------------------------------------------------------------------
// <copyright file="IAction.cs" company="SoloX Software">
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
    /// Base action interface with a target state type specified.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IAction<TRootState>
        where TRootState : IState<TRootState>
    {
        /// <summary>
        /// Apply the current action on the given state.
        /// </summary>
        /// <param name="dispatcher">The dispatcher to use to dispatch other actions.</param>
        /// <param name="rootState">Root state used as input.</param>
        /// <returns>The resulting root state once the action have been applied.</returns>
        TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState rootState);
    }

    /// <summary>
    /// Base action interface with a target state type specified.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    /// <typeparam name="TActionBehavior">Type of the targeted state object.</typeparam>
    public interface IAction<TRootState, out TActionBehavior> : IAction<TRootState>
        where TActionBehavior : IActionBehavior
        where TRootState : IState<TRootState>
    {
        /// <summary>
        /// Gets action behavior.
        /// </summary>
        TActionBehavior Behavior { get; }
    }
}
