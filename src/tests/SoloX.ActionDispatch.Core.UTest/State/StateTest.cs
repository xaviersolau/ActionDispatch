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

            state.Value = "some value 1";

            state.Lock();

            Assert.True(state.IsLocked);

            Assert.Throws<AccessViolationException>(() => state.Value = "some value 2");
        }

        [Fact]
        public void LockStateWithChildTest()
        {
            var state = new RootState()
            {
                Child1 = new StateA(),
                Child2 = new StateA(),
            };

            Assert.False(state.IsLocked);
            Assert.False(state.Child1.IsLocked);
            Assert.False(state.Child2.IsLocked);

            state.Child1.Value = "some value 11";
            state.Child1.Value = "some value 12";

            state.Lock();

            Assert.True(state.IsLocked);
            Assert.True(state.Child1.IsLocked);
            Assert.True(state.Child2.IsLocked);

            Assert.Throws<AccessViolationException>(() => state.Child1.Value = "some value 21");
            Assert.Throws<AccessViolationException>(() => state.Child2.Value = "some value 22");
        }

        [Fact]
        public void CloneStateTest()
        {
            var value = "Test";
            var state = new StateA();
            state.Value = value;
            state.Lock();

            var clone = state.DeepClone();

            Assert.Equal(state.Version, clone.Version);

            Assert.NotSame(state, clone);
            var clonedState = Assert.IsType<StateA>(clone);
            Assert.Equal(value, clonedState.Value);
        }

        [Fact]
        public void CloneStateWithChildTest()
        {
            var value = 123;
            var childValue = "Test";
            var state = new RootState()
            {
                Child1 = new StateA()
                {
                    Value = childValue,
                },
                Value = value,
            };
            state.Lock();

            var clone = state.DeepClone();

            Assert.Equal(state.Version, clone.Version);

            Assert.NotSame(state, clone);
            var clonedState = Assert.IsType<RootState>(clone);
            Assert.NotSame(state.Child1, clonedState.Child1);

            Assert.Equal(childValue, clonedState.Child1.Value);
        }

        [Fact]
        public void PatchChildStateTest()
        {
            var value = 123;
            var childValue = "Test";
            var child2Value = "Test2";
            var childNewValue = "NewTest";

            var child2 = new StateA()
            {
                Value = child2Value,
            };

            var oldChild1 = new StateA()
            {
                Value = childValue,
            };

            var state = new RootState()
            {
                Child1 = oldChild1,
                Child2 = child2,
                Value = value,
            };

            state.Lock();

            var newChild1 = (StateA)oldChild1.DeepClone();
            newChild1.Value = childNewValue;

            var patch = state.Patch(oldChild1, newChild1);

            patch.Lock();

            Assert.Equal(state.Version + 1, patch.Version);

            Assert.NotSame(state, patch);
            var patchedState = Assert.IsType<RootState>(patch);
            Assert.NotSame(state.Child1, patchedState.Child1);
            Assert.Equal(state.Child1.Version + 1, patchedState.Child1.Version);
            Assert.Equal(childNewValue, patchedState.Child1.Value);

            Assert.Same(state.Child2, patchedState.Child2);
        }
    }
}
