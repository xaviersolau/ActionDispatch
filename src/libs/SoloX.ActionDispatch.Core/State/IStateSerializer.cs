// ----------------------------------------------------------------------
// <copyright file="IStateSerializer.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State
{
    /// <summary>
    /// IStateSerializer is an interface defining an API to serialize a State.
    /// </summary>
    public interface IStateSerializer
    {
        /// <summary>
        /// Serialize the given state.
        /// </summary>
        /// <param name="state">The state to serialize.</param>
        /// <returns>The serialized state.</returns>
        string Serialize(IState state);

        /// <summary>
        /// De-serialize the given textual state.
        /// </summary>
        /// <typeparam name="TState">The state type to read.</typeparam>
        /// <param name="state">The textual state.</param>
        /// <returns>The resulting state instance.</returns>
        TState Deserialize<TState>(string state)
            where TState : IState;
    }
}
