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
    /// <typeparam name="TRootState">The root application state Type.</typeparam>
    /// <typeparam name="TState">The state type to handle in the transaction.</typeparam>
    public interface ITransactionalState<TRootState, TState> : IDisposable
        where TRootState : IState
        where TState : IState
    {
        /// <summary>
        /// Gets the current state clone.
        /// </summary>
        /// <returns>A clone of the current state.</returns>
        /// <remarks>
        /// The GetState method must not be call if SetState has been called before.
        /// This method must be use if you want to modify the current state (that will be cloned).
        /// </remarks>
        TState GetState();

        /// <summary>
        /// Set the given state that will replace the current one.
        /// </summary>
        /// <param name="state">The new state that will replace the current one.</param>
        /// <remarks>
        /// The SetState method must not be call if GetState has been called before.
        /// This method must be use if you want to replace the current state with a new one created externally.
        /// </remarks>
        void SetState(TState state);

        /// <summary>
        /// Close the transaction and patch the root state.
        /// </summary>
        /// <returns>The patched root state.</returns>
        TRootState Close();
    }
}
