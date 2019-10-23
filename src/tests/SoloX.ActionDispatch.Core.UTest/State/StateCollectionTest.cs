// ----------------------------------------------------------------------
// <copyright file="StateCollectionTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void CloneStateTest()
        {
            var childValue = "Test";
            var state = new RootCollectionState();
            var stateA1 = new StateA()
            {
                Value = childValue,
            };

            state.Items.Add(stateA1);

            state.Lock();

            var clone = state.DeepClone();

            Assert.Equal(state.Version, clone.Version);

            Assert.NotSame(state, clone);
            var clonedState = Assert.IsType<RootCollectionState>(clone);
            Assert.NotEmpty(clonedState.Items);
            Assert.NotSame(state.Items.First(), clonedState.Items.First());

            Assert.Equal(childValue, clonedState.Items.First().Value);
        }

        [Fact]
        public void PatchChildStateTest()
        {
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

            var state = new RootCollectionState();
            state.Items.Add(oldChild1);
            state.Items.Add(child2);

            state.Lock();

            var newChild1 = (StateA)oldChild1.DeepClone();
            newChild1.Value = childNewValue;

            var patch = state.Patch(oldChild1, newChild1);

            patch.Lock();

            Assert.Equal(state.Version + 1, patch.Version);

            Assert.NotSame(state, patch);
            var patchedState = Assert.IsType<RootCollectionState>(patch);
            Assert.NotSame(state.Items.First(), patchedState.Items.First());
            Assert.Equal(state.Items.First().Version + 1, patchedState.Items.First().Version);
            Assert.Equal(childNewValue, patchedState.Items.First().Value);

            Assert.Same(state.Items.Last(), patchedState.Items.Last());
        }
    }
}
