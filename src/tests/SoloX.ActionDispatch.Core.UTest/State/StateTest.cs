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
                Child = new StateA(),
            };

            Assert.False(state.IsLocked);
            Assert.False(state.Child.IsLocked);

            state.Child.Value = "some value 1";

            state.Lock();

            Assert.True(state.IsLocked);
            Assert.True(state.Child.IsLocked);

            Assert.Throws<AccessViolationException>(() => state.Child.Value = "some value 2");
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
                Child = new StateA()
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
            Assert.NotSame(state.Child, clonedState.Child);

            Assert.Equal(childValue, clonedState.Child.Value);
        }

        [Fact]
        public void PatchChildStateTest()
        {
            var value = 123;
            var childValue = "Test";
            var childNewValue = "NewTest";

            var oldChild = new StateA()
            {
                Value = childValue,
            };

            var newChild = new StateA()
            {
                Value = childNewValue,
            };

            var state = new RootState()
            {
                Child = oldChild,
                Value = value,
            };

            state.Lock();

            var patch = state.Patch(oldChild, newChild);

            patch.Lock();

            Assert.Equal(state.Version + 1, patch.Version);

            Assert.NotSame(state, patch);
            var patchedState = Assert.IsType<RootState>(patch);
            Assert.NotSame(state.Child, patchedState.Child);

            Assert.Equal(childNewValue, patchedState.Child.Value);
        }
    }
}
