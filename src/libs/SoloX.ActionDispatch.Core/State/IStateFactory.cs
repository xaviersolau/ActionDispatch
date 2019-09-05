// ----------------------------------------------------------------------
// <copyright file="IStateFactory.cs" company="SoloX Software">
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
    /// The state factory interface.
    /// </summary>
    public interface IStateFactory
    {
        /// <summary>
        /// Create a state instance.
        /// </summary>
        /// <typeparam name="TState">The state type that will be created.</typeparam>
        /// <returns>The created instance.</returns>
        TState Create<TState>()
            where TState : IState;

        /// <summary>
        /// Create a state instance.
        /// </summary>
        /// <param name="stateType">The state type that will be created.</param>
        /// <returns>The created state type instance.</returns>
        /// <remarks>The state type must be an interface based on IState.</remarks>
        IState Create(Type stateType);
    }
}
