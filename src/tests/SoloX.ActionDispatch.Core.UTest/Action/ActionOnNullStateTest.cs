// ----------------------------------------------------------------------
// <copyright file="ActionOnNullStateTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Action
{
    public class ActionOnNullStateTest
    {
        [Fact]
        public void NullStateTest()
        {
            var newState = new StateA();
            var ib = new InitializeBehavior<IStateA>(newState);

            var action = new SyncAction<IStateA, IStateA>(ib, s => s);

            Assert.Throws<NullReferenceException>(() => action.Apply(null, null));
        }
    }
}
