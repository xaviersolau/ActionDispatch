﻿// ----------------------------------------------------------------------
// <copyright file="IMyChildState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.State.Generator.ITest.Resources.TestMultiProject.Lib1
{
    /// <summary>
    /// IMyChildState is a child state to use in the tests.
    /// </summary>
    public interface IMyChildState : IState
    {
        string Value1 { get; set; }
    }
}
