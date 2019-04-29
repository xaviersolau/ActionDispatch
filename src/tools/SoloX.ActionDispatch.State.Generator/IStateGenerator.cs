// ----------------------------------------------------------------------
// <copyright file="IStateGenerator.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.State.Generator
{
    /// <summary>
    /// The state generator interface.
    /// </summary>
    public interface IStateGenerator
    {
        /// <summary>
        /// Generate the state implementation on the given project.
        /// </summary>
        /// <param name="projectFile">Project file.</param>
        /// <param name="projectNameSpace">Root name space of the project.</param>
        void Generate(string projectFile, string projectNameSpace);
    }
}
