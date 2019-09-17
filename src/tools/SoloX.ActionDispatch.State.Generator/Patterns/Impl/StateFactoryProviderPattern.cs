// ----------------------------------------------------------------------
// <copyright file="StateFactoryProviderPattern.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;
using SoloX.ActionDispatch.State.Generator.Patterns.Impl;
using SoloX.ActionDispatch.State.Generator.Patterns.Itf;

namespace SoloX.ActionDispatch.State.Generator.Patterns.Impl
{
    /// <summary>
    /// StateFactoryProviderPattern implementation.
    /// </summary>
    public class StateFactoryProviderPattern : AStateFactoryProvider, IStateFactoryProvider
    {
        /// <inheritdoc/>
        public override void Register()
        {
            Register<IParentStatePattern, ParentStatePattern>();
        }
    }
}
