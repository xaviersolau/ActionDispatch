﻿// ----------------------------------------------------------------------
// <copyright file="AsyncBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Sample.State.Basic;

namespace SoloX.ActionDispatch.Json.UTest.Action.Basic
{
    public class AsyncBehavior : IActionBehaviorAsync<IStateA>
    {
        public AsyncBehavior(string someValue)
        {
            this.SomeValue = someValue;
        }

        public string SomeValue { get; }

        public async Task Apply(IRelativeDispatcher<IStateA> dispatcher, IStateA state)
        {
            await Task.Delay(1000).ConfigureAwait(false);
            return;
        }
    }
}
