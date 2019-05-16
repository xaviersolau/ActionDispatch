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
        /// <param name="inputsFile">Location where the input files must be generated.</param>
        /// <param name="outputsFile">Location where the output files must be generated.</param>
        /// <param name="generate">Tells if we must generate the output files.</param>
        void Generate(string projectFile, string inputsFile, string outputsFile, bool generate);
    }
}
