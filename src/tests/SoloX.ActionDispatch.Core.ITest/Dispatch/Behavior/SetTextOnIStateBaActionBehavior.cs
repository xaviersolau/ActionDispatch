﻿// ----------------------------------------------------------------------
// <copyright file="SetTextOnIStateBaActionBehavior.cs" company="SoloX Software">
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

namespace SoloX.ActionDispatch.Core.ITest.Dispatch.Behavior
{
    public class SetTextOnIStateBaActionBehavior : IActionBehavior<IStateBa>
    {
        public SetTextOnIStateBaActionBehavior(string text)
        {
            this.Text = text;
        }

        public string Text { get; }

        public void Apply(IStateContainer<IStateBa> stateContainer)
        {
            stateContainer.LoadState();
            var state = stateContainer.State;
            state.Value = this.Text;
        }
    }
}
