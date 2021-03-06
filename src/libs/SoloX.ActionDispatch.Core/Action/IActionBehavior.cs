﻿// ----------------------------------------------------------------------
// <copyright file="IActionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action
{
    /// <summary>
    /// Action behavior.
    /// </summary>
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface IActionBehavior
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }

    /// <summary>
    /// Action behavior.
    /// </summary>
    /// <typeparam name="TState">The state type the action apply on.</typeparam>
    public interface IActionBehavior<TState> : IActionBehavior
        where TState : IState
    {
        /// <summary>
        /// Apply action behavior on the given state.
        /// </summary>
        /// <param name="stateContainer">The state container the action apply on.</param>
        void Apply(IStateContainer<TState> stateContainer);
    }
}
