// ----------------------------------------------------------------------
// <copyright file="ICallingStrategy.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    /// <summary>
    /// Calling strategy interface.
    /// </summary>
    public interface ICallingStrategy
    {
        /// <summary>
        /// Invoke the given action.
        /// </summary>
        /// <param name="action">The action to call.</param>
        void Invoke(System.Action action);
    }
}
