// ----------------------------------------------------------------------
// <copyright file="IActionMiddleware.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Text;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Dispatch
{
    /// <summary>
    /// Dispatcher middle ware interface. Used before an action is actually published.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IActionMiddleware<TRootState>
        where TRootState : IState
    {
        /// <summary>
        /// Tells if the middle ware is applying on the given action behavior.
        /// </summary>
        /// <param name="actionBehavior">The action behavior to evaluate.</param>
        /// <returns>True if the action behavior is selected for the middle ware.</returns>
        bool IsApplying(IActionBehavior actionBehavior);

        /// <summary>
        /// Setup the middle ware on the action observable.
        /// </summary>
        /// <param name="actionObservable">The source action observable the middle ware must apply on.</param>
        /// <returns>The action observable configured with the middle ware.</returns>
        IObservable<IAction<TRootState, IActionBehavior>> Setup(
            IObservable<IAction<TRootState, IActionBehavior>> actionObservable);
    }
}
