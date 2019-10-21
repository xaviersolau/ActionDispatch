// ----------------------------------------------------------------------
// <copyright file="StateCollectionTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Collection.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.State
{
    public class StateCollectionTest
    {
        [Fact]
        public void LockStateTest()
        {
            var state = new RootCollectionState();

            var stateA1 = new StateA()
            {
                Value = "Some value...",
            };

            Assert.False(state.IsLocked);

            state.Items.Add(stateA1);

            state.Lock();

            Assert.True(state.IsLocked);

            Assert.True(stateA1.IsLocked);

            var stateA2 = new StateA();

            Assert.Throws<AccessViolationException>(() => state.Items.Add(stateA2));
        }
    }
}
