// ----------------------------------------------------------------------
// <copyright file="IExampleChildState.cs" company="SoloX Software">
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
    /// Child example state interface.
    /// </summary>
    public interface IExampleChildState : IState<IExampleChildState>
    {
        /// <summary>
        /// Gets or sets child state count.
        /// </summary>
        int ChildCount
        { get; set; }
    }
}
