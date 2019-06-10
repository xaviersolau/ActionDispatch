// ----------------------------------------------------------------------
// <copyright file="StateContainerTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.State
{
    public class StateContainerTest
    {
        [Fact]
        public void StateContainerLoadStateTest()
        {
            var value = "Test";
            var state = new StateA();
            state.Value = value;
            state.Lock();

            var stateContainer = new StateContainer<IStateA>(state);
            stateContainer.LoadState();
            var newState = stateContainer.State;
            Assert.NotSame(state, newState);
            Assert.False(newState.IsLocked);
            Assert.Equal(state.Value, newState.Value);
        }

        [Fact]
        public void StateTransactionSetStateTest()
        {
            var value = "Test";
            var state = new StateA();
            state.Value = value;
            state.Lock();

            var value2 = "Test2";
            var state2 = new StateA()
            {
                Value = value2,
            };

            var stateContainer = new StateContainer<IStateA>(state);

            var newState = new StateA()
            {
                Value = value2,
            };

            stateContainer.State = newState;

            Assert.False(newState.IsLocked);
        }

        [Fact]
        public void MultipleCallToGetSetState()
        {
            var value = "Test";
            var state = new StateA();
            state.Value = value;
            state.Lock();

            var stateContainer = new StateContainer<IStateA>(state);

            stateContainer.LoadState();

            var newState = stateContainer.State;

            Assert.Throws<AccessViolationException>(() => stateContainer.LoadState());

            Assert.Throws<AccessViolationException>(() => stateContainer.State = new StateA());

            Assert.False(newState.IsLocked);
        }
    }
}
