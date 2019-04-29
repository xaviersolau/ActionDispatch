// ----------------------------------------------------------------------
// <copyright file="IChildStatePattern.cs" company="SoloX Software">
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
    /// Child State interface pattern declaration.
    /// </summary>
    public interface IChildStatePattern : IState<IChildStatePattern>
    {
    }
}
