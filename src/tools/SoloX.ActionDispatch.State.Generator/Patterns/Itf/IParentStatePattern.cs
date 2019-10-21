// ----------------------------------------------------------------------
// <copyright file="IParentStatePattern.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.State.Generator.Patterns.Itf
{
    /// <summary>
    /// State interface pattern declaration.
    /// </summary>
    public interface IParentStatePattern : IState
    {
        /// <summary>
        /// Gets or sets PropertyPattern.
        /// </summary>
        object PropertyPattern { get; set; }

        /// <summary>
        /// Gets or sets PropertyPattern.
        /// </summary>
        IChildStatePattern ChildPattern { get; set; }

        /// <summary>
        /// Gets collection PropertyPattern.
        /// </summary>
        ICollection<IChildStatePattern> ChildrenPattern { get; }
    }
}
