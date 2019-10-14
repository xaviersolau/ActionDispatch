// ----------------------------------------------------------------------
// <copyright file="IObjectPropertyState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Generator.ITest.Resources.State.Data;

namespace SoloX.ActionDispatch.State.Generator.ITest.Resources.State
{
    public interface IObjectPropertyState : IState
    {
        SomeObject Value1 { get; set; }
    }
}
