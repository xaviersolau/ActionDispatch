// ----------------------------------------------------------------------
// <copyright file="IStateContainer.cs" company="SoloX Software">
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
    /// State container interface.
    /// </summary>
    /// <typeparam name="TState">The state type to handle in the transaction.</typeparam>
    public interface IStateContainer<TState>
        where TState : IState
    {
        /// <summary>
        /// Gets a value indicating whether the container is empty or not.
        /// </summary>
        /// <remarks>If the State has been explicitly set to null the container will be seen as none empty.</remarks>
        bool IsEmpty { get; }

        /// <summary>
        /// Gets or sets the state in the container.
        /// </summary>
        /// <remarks>
        /// The State property must not be set if LoadState has been called before.
        /// This property must be set if you want to replace the current state with a new one created externally.
        /// </remarks>
        TState State { get; set; }

        /// <summary>
        /// Load the current state and clone it.
        /// </summary>
        /// <remarks>
        /// The LoadState method must not be call if State has been set before.
        /// This method must be use if you want to modify the current state (that will be cloned).
        /// </remarks>
        void LoadState();
    }
}
