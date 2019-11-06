// ----------------------------------------------------------------------
// <copyright file="IActionObserver.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Dispatch
{
    /// <summary>
    /// Dispatcher action observer interface. Used after an action is actually published.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IActionObserver<TRootState>
        where TRootState : IState
    {
        /// <summary>
        /// Tells if the observer is observing the given action behavior.
        /// </summary>
        /// <param name="actionBehavior">The action behavior to observe.</param>
        /// <returns>True if the action behavior is selected for the observer.</returns>
        bool IsObserving(IActionBehavior actionBehavior);

        /// <summary>
        /// Observe the given published action that is producing the given state.
        /// </summary>
        /// <param name="action">The published action.</param>
        /// <param name="producedState">The state produced once the action was published.</param>
        void Observe(IAction<TRootState, IActionBehavior> action, TRootState producedState);
    }
}
