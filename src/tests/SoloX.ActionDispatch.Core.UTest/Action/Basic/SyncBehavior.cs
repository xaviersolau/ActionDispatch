// ----------------------------------------------------------------------
// <copyright file="SyncBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.UTest.Action.Basic
{
    public class SyncBehavior : IActionBehavior<IStateA>
    {
        public SyncBehavior(string someValue)
        {
            this.SomeValue = someValue;
        }

        public string SomeValue { get; }

        public void Apply(IStateContainer<IStateA> stateContainer)
        {
            // Nothing to do for now.
        }
    }
}
