﻿// ----------------------------------------------------------------------
// <copyright file="InitializeActionTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Action
{
    public class InitializeActionTest
    {
        [Fact]
        public void StateReplacementTest()
        {
            IStateA current = new StateA();
            var previous = current;
            current.Lock();

            IStateA newState = new StateA();
            var ib = new InitializeBehavior<IStateA, IStateA>(newState);

            var action = new SyncAction<IStateA, IStateA>(ib, s => s);

            current = action.Apply(null, current);

            Assert.True(current.IsLocked);
            Assert.NotSame(previous, current);
            Assert.Same(newState, current);
        }

        [Fact]
        public void SubStateReplacementTest()
        {
            var child = new StateBa();
            IStateB current = new StateB()
            {
                Child = child,
            };

            var previous = current;
            current.Lock();

            IStateBa newState = new StateBa();
            var ib = new InitializeBehavior<IStateB, IStateBa>(newState);

            var action = new SyncAction<IStateB, IStateBa>(ib, s => s.Child);

            current = action.Apply(null, current);

            Assert.True(current.IsLocked);

            Assert.NotSame(previous, current);
            Assert.NotSame(child, current.Child);
            Assert.Same(newState, current.Child);
        }
    }
}
