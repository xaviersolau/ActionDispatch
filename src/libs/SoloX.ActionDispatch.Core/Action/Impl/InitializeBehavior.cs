// ----------------------------------------------------------------------
// <copyright file="InitializeBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action.Impl
{
    /// <summary>
    /// Initialize state behavior.
    /// </summary>
    /// <typeparam name="TState">The state type the action apply on.</typeparam>
    public class InitializeBehavior<TState> : IActionBehavior<TState>
        where TState : IState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeBehavior{TState}"/> class.
        /// </summary>
        /// <param name="state">The state to use as initial value.</param>
        public InitializeBehavior(TState state)
        {
            this.State = state;
        }

        /// <summary>
        /// Gets the State to set.
        /// </summary>
        public TState State { get; }

        /// <inheritdoc/>
        public void Apply(IStateContainer<TState> stateContainer)
        {
            stateContainer.State = this.State;
        }
    }
}
