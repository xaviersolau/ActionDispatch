// ----------------------------------------------------------------------
// <copyright file="StateTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.UTest.State.Basic;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.State
{
    public class StateTest
    {
        [Fact]
        public void LockStateTest()
        {
            var state = new StateA();

            Assert.False(state.IsLocked);

            state.Lock();

            Assert.True(state.IsLocked);
        }
    }
}
