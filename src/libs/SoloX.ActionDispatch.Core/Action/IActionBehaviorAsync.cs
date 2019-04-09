﻿// ----------------------------------------------------------------------
// <copyright file="IActionBehaviorAsync.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action
{
    /// <summary>
    /// Asynchronous action behavior.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    /// <typeparam name="TState">The state type the action apply on.</typeparam>
    public interface IActionBehaviorAsync<TRootState, TState> : IActionBehavior
        where TRootState : IState<TRootState>
        where TState : IState
    {
        /// <summary>
        /// Apply action behavior on the given state.
        /// </summary>
        /// <param name="dispatcher">The dispatcher object.</param>
        /// <param name="state">The state the action apply on.</param>
        /// <returns>The resulting state.</returns>
        Task Apply(IDispatcher<TRootState> dispatcher, TState state);
    }
}