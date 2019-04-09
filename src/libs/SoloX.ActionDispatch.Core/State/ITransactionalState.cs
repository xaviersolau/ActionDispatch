// ----------------------------------------------------------------------
// <copyright file="ITransactionalState.cs" company="SoloX Software">
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
    /// Transactional state interface.
    /// </summary>
    /// <typeparam name="TState">The state type to handle in the transaction.</typeparam>
    public interface ITransactionalState<TState> : IDisposable
        where TState : IState
    {
        /// <summary>
        /// Gets the state taken into account by the transaction.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Patch the given owner state with the current transactional state.
        /// </summary>
        /// <typeparam name="TRootState">The root state type to patch.</typeparam>
        /// <param name="rootState">The root state instance to patch.</param>
        /// <returns>The patched state instance.</returns>
        TRootState Patch<TRootState>(TRootState rootState)
            where TRootState : IState<TRootState>;
    }
}
