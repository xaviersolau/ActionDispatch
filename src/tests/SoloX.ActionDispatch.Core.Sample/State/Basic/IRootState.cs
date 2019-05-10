// ----------------------------------------------------------------------
// <copyright file="IRootState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Sample.State.Basic
{
    public interface IRootState : IState<IRootState>
    {
        int Value { get; set; }

        IStateA Child1 { get; set; }

        IStateA Child2 { get; set; }
    }
}
