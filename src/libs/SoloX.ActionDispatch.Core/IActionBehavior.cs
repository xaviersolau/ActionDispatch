// ----------------------------------------------------------------------
// <copyright file="IActionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core
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
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    /// <typeparam name="TState">The state type the action apply on.</typeparam>
    public interface IActionBehavior<TRootState, TState> : IActionBehavior
    {
        /// <summary>
        /// Apply action behavior on the given state.
        /// </summary>
        /// <param name="state">The state the action apply on.</param>
        /// <returns>The resulting state.</returns>
        TState Apply(TState state);
    }
}
