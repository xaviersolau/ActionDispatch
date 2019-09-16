// ----------------------------------------------------------------------
// <copyright file="IMyRootState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Generator.ITest.Resources.TestMultiProject.Lib1;

namespace SoloX.ActionDispatch.State.Generator.ITest.Resources.TestMultiProject.Lib3
{
    public interface IMyRootState : IState
    {
        string Value1 { get; set; }

        int Value2 { get; set; }

        IMyChildState Child1 { get; set; }

        IMyChildState Child2 { get; set; }
    }
}
