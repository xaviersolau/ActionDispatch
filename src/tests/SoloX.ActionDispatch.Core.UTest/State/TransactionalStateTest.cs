// ----------------------------------------------------------------------
// <copyright file="TransactionalStateTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.State
{
    public class TransactionalStateTest
    {
        [Fact]
        public void StateTransactionGetStateTest()
        {
            var value = "Test";
            var state = new StateA();
            state.Value = value;
            state.Lock();

            IStateA patchedState;
            using (var transactionalState = state.CreateTransactionalState<IStateA>(state))
            {
                var newState = transactionalState.GetState();
                Assert.NotSame(state, newState);
                Assert.False(newState.IsLocked);
                Assert.Equal(state.Value, newState.Value);
                patchedState = transactionalState.Close();
                Assert.Same(newState, patchedState);
            }

            Assert.True(patchedState.IsLocked);
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

            IStateA patchedState;
            using (var transactionalState = state.CreateTransactionalState<IStateA>(state))
            {
                var newState = new StateA()
                {
                    Value = value2,
                };

                transactionalState.SetState(newState);

                Assert.False(newState.IsLocked);
                patchedState = transactionalState.Close();
                Assert.Same(newState, patchedState);
            }

            Assert.True(patchedState.IsLocked);
        }
    }
}
