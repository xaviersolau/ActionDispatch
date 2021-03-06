﻿// ----------------------------------------------------------------------
// <copyright file="IStateCollectionPropertyState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.State.Generator.ITest.Resources.State
{
    public interface IStateCollectionPropertyState : IState
    {
        ICollection<ISimpleState> Children { get; }
    }
}
