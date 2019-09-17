// ----------------------------------------------------------------------
// <copyright file="IStateFactoryProvider.cs" company="SoloX Software">
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
    /// State factory provider that will contribute to the state factory.
    /// </summary>
    public interface IStateFactoryProvider
    {
        /// <summary>
        /// Register the state type to be used though the state factory.
        /// </summary>
        void Register();
    }
}
