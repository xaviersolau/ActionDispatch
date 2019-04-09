// ----------------------------------------------------------------------
// <copyright file="IState.cs" company="SoloX Software">
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
    /// State base interface.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Gets version number of the state object.
        /// </summary>
        /// <remarks>The version number is automatically incremented every time the state object is modified.</remarks>
        int Version { get; }

        /// <summary>
        /// Gets a value indicating whether the state is locked or not.
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// Lock the state in order to prevent write access.
        /// </summary>
        void Lock();
    }

    /// <summary>
    /// State base interface.
    /// </summary>
    /// <typeparam name="TState">The actual type of the state.</typeparam>
    public interface IState<TState> : IState
        where TState : IState
    {
        /// <summary>
        /// Create a transactional state in order to update the state.
        /// </summary>
        /// <returns>The transactional state instance.</returns>
        ITransactionalState<TState> CreateTransactionalState();
    }
}
