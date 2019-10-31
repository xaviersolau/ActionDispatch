// ----------------------------------------------------------------------
// <copyright file="IInitialStateFactory.cs" company="SoloX Software">
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
    /// Initial state factory. It is used to initialize the state of the dispatcher.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IInitialStateFactory<TRootState>
        where TRootState : IState
    {
        /// <summary>
        /// Create the initial state of the dispatcher.
        /// </summary>
        /// <returns>The created initial state.</returns>
        TRootState Create();
    }
}
