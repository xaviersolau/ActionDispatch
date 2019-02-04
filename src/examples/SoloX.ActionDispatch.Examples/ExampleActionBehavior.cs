// ----------------------------------------------------------------------
// <copyright file="ExampleActionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ActionDispatch.Core;

namespace SoloX.ActionDispatch.Examples
{
    /// <summary>
    /// Example action.
    /// </summary>
    public class ExampleActionBehavior : IActionBehavior<ExampleState, ExampleState>
    {
        /// <inheritdoc />
        public ExampleState Apply(ExampleState state)
        {
            throw new NotImplementedException();
        }
    }
}
