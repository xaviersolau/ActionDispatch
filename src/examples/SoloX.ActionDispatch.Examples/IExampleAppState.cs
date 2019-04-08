// ----------------------------------------------------------------------
// <copyright file="IExampleAppState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core;

namespace SoloX.ActionDispatch.Examples
{
    /// <summary>
    /// Root example state interface.
    /// </summary>
    public interface IExampleAppState : IState<IExampleAppState>
    {
        /// <summary>
        /// Gets or sets app state count.
        /// </summary>
        int AppCount
        { get; set; }

        /// <summary>
        /// Gets or sets child state.
        /// </summary>
        IExampleChildState ChildState
        { get; set; }
    }
}
