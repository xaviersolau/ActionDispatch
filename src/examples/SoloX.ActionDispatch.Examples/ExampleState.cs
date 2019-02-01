// ----------------------------------------------------------------------
// <copyright file="ExampleState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;

namespace SoloX.ActionDispatch.Examples
{
    /// <summary>
    /// Root state object.
    /// </summary>
    public class ExampleState
    {
        /// <summary>
        /// Gets or sets state count.
        /// </summary>
        public int Count
        { get; set; }
    }
}
