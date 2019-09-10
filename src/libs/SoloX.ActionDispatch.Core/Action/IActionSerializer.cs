// ----------------------------------------------------------------------
// <copyright file="IActionSerializer.cs" company="SoloX Software">
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
    /// IActionSerializer is an interface defining an API to serialize an Action.
    /// </summary>
    public interface IActionSerializer
    {
        /// <summary>
        /// Serialize the given action.
        /// </summary>
        /// <typeparam name="TState">The root state type the action apply on.</typeparam>
        /// <param name="action">The action to serialize.</param>
        /// <returns>The serialized action.</returns>
        string Serialize<TState>(IAction<TState> action)
            where TState : IState;

        /// <summary>
        /// De-serialize the given textual action.
        /// </summary>
        /// <typeparam name="TState">The root state type the action apply on.</typeparam>
        /// <param name="action">The textual action.</param>
        /// <returns>The resulting action instance.</returns>
        IAction<TState> Deserialize<TState>(string action)
            where TState : IState;
    }
}
