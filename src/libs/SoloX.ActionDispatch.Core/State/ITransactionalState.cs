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
    /// <typeparam name="TRootState">The root application state Type.</typeparam>
    public interface ITransactionalState<TState, TRootState> : IDisposable
        where TState : IState
        where TRootState : IState
    {
        /// <summary>
        /// Gets the state taken into account by the transaction.
        /// </summary>
        TState State { get; }

        /// <summary>
        /// Close the transaction and patch the root state.
        /// </summary>
        /// <returns>The patched root state.</returns>
        TRootState Close();
    }
}
