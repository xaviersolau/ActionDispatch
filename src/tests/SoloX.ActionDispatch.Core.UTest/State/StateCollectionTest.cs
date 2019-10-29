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
using SoloX.ActionDispatch.Core.Sample.State.Basic;
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
            var state = CreateOneItemRootCollectionState(out var stateA1);

            Assert.False(state.IsLocked);
            Assert.False(stateA1.IsLocked);

            state.Lock();

            Assert.True(state.IsLocked);
            Assert.True(stateA1.IsLocked);

            var stateA2 = new StateA();

            Assert.Throws<AccessViolationException>(() => state.Items.Add(stateA2));
        }

        [Fact]
        public void CloneStateTest()
        {
            var state = CreateOneItemRootCollectionState(out var stateA);

            var childValue = stateA.Value;

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
            var childNewValue = "NewTest";

            var state = CreateTwoItemsRootCollectionState(out var oldChild1, out var child2);

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

        [Fact]
        public void StateCollectionIsReadOnlyTest()
        {
            var state = new RootCollectionState();

            Assert.False(state.Items.IsReadOnly);

            state.Lock();

            Assert.True(state.Items.IsReadOnly);
        }

        [Fact]
        public void StateCollectionClearTest()
        {
            var state = CreateOneItemRootCollectionState(out _);

            Assert.Equal(1, state.Items.Count);

            state.Items.Clear();

            Assert.Equal(0, state.Items.Count);
        }

        [Fact]
        public void StateCollectionContainsTest()
        {
            var state = new RootCollectionState();
            var stateA1 = new StateA()
            {
                Value = "Some value",
            };

            Assert.False(state.Items.Contains(stateA1));

            state.Items.Add(stateA1);

            Assert.True(state.Items.Contains(stateA1));
        }

        [Fact]
        public void StateCollectionCopyToTest()
        {
            var state = CreateOneItemRootCollectionState(out var stateA);

            var array = new IStateA[1];
            state.Items.CopyTo(array, 0);

            Assert.Same(stateA, array[0]);
        }

        [Fact]
        public void StateCollectionRemoveTest()
        {
            var state = CreateTwoItemsRootCollectionState(out var stateA1, out var stateA2);

            Assert.Equal(2, state.Items.Count);
            state.Items.Remove(stateA2);

            Assert.Equal(1, state.Items.Count);
            Assert.True(state.Items.Contains(stateA1));
            Assert.False(state.Items.Contains(stateA2));
        }

        [Fact]
        public void StateCollectionIndexOfTest()
        {
            var state = CreateTwoItemsRootCollectionState(out var stateA1, out var stateA2);

            Assert.Equal(0, state.Items.IndexOf(stateA1));
            Assert.Equal(1, state.Items.IndexOf(stateA2));
        }

        [Fact]
        public void StateCollectionInsertTest()
        {
            var state = CreateTwoItemsRootCollectionState(out var stateA1, out var stateA2);

            var stateA = new StateA()
            {
                Value = "Some value to insert...",
            };

            Assert.Equal(0, state.Items.IndexOf(stateA1));
            Assert.Equal(1, state.Items.IndexOf(stateA2));

            state.Items.Insert(1, stateA);

            Assert.Equal(0, state.Items.IndexOf(stateA1));
            Assert.Equal(1, state.Items.IndexOf(stateA));
            Assert.Equal(2, state.Items.IndexOf(stateA2));
        }

        [Fact]
        public void StateCollectionRemoveAtTest()
        {
            var state = CreateTwoItemsRootCollectionState(out var stateA1, out var stateA2);

            Assert.Equal(0, state.Items.IndexOf(stateA1));
            Assert.Equal(1, state.Items.IndexOf(stateA2));

            state.Items.RemoveAt(0);

            Assert.Equal(0, state.Items.IndexOf(stateA2));
            Assert.False(state.Items.Contains(stateA1));
        }

        private static RootCollectionState CreateOneItemRootCollectionState(out StateA stateA)
        {
            var state = new RootCollectionState();
            stateA = new StateA()
            {
                Value = "Some value 1",
            };

            state.Items.Add(stateA);

            return state;
        }

        private static RootCollectionState CreateTwoItemsRootCollectionState(out StateA stateA1, out StateA stateA2)
        {
            var state = new RootCollectionState();
            stateA1 = new StateA()
            {
                Value = "Some value 1",
            };

            stateA2 = new StateA()
            {
                Value = "Some value 2",
            };

            state.Items.Add(stateA1);
            state.Items.Add(stateA2);

            return state;
        }
    }
}
