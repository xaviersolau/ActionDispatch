// ----------------------------------------------------------------------
// <copyright file="StateJsonTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.Sample.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Collection;
using SoloX.ActionDispatch.Core.Sample.State.Collection.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;
using SoloX.ActionDispatch.Json.State.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Json.UTest.State
{
    public class StateJsonTest
    {
        [Fact]
        public void SimpleStateSerializationTest()
        {
            var state = SetupStateA();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(Mock.Of<IStateFactory>()));

            AssertStateAJson(json, state);
        }

        [Fact]
        public void StateSerializerSerializeSimpleTest()
        {
            var serializer = new JsonStateSerializer(Mock.Of<IStateFactory>());

            var state = SetupStateA();

            var json = serializer.Serialize(state);

            AssertStateAJson(json, state);
        }

        [Fact]
        public void SimpleStateDeserializationTest()
        {
            var state = SetupStateA();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(Mock.Of<IStateFactory>()));

            var stateFactoryMock = SetupStateFactoryMock();

            var deserializedState = JsonConvert.DeserializeObject<IStateA>(json, new JsonStateConverter(stateFactoryMock.Object));

            stateFactoryMock.Verify(f => f.Create(typeof(IStateA)));

            AssertSameState(state, deserializedState);
        }

        [Fact]
        public void CollectionStateSerializationTest()
        {
            var state = SetupStateCollection();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(Mock.Of<IStateFactory>()));

            AssertRootCollectionStateJson(json, state);
        }

        [Fact]
        public void CollectionStateDeserializationTest()
        {
            var state = SetupStateCollection();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(Mock.Of<IStateFactory>()));

            var stateFactoryMock = SetupStateFactoryMock();

            var deserializedState = JsonConvert.DeserializeObject<IRootCollectionState>(json, new JsonStateConverter(stateFactoryMock.Object));

            stateFactoryMock.Verify(f => f.Create(typeof(IRootCollectionState)));
            stateFactoryMock.Verify(f => f.Create(typeof(IStateA)), Times.Exactly(3));

            AssertSameState(state, deserializedState);
        }

        [Fact]
        public void StateSerializerSerializationTest()
        {
            var state = SetupStateA();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(Mock.Of<IStateFactory>()));

            var stateFactoryMock = SetupStateFactoryMock();

            var serializer = new JsonStateSerializer(stateFactoryMock.Object);

            var deserializedState = serializer.Deserialize<IStateA>(json);

            stateFactoryMock.Verify(f => f.Create(typeof(IStateA)));

            AssertSameState(state, deserializedState);
        }

        [Theory]
        [InlineData(typeof(IStateA), true)]
        [InlineData(typeof(AStateBase<IStateA>), true)]
        [InlineData(typeof(object), false)]
        public void StateConverterTypeSupportTest(Type type, bool expected)
        {
            var converter = new JsonStateConverter(Mock.Of<IStateFactory>());
            Assert.Equal(expected, converter.CanConvert(type));
        }

        private static StateA SetupStateA()
        {
            var state = new StateA();
            state.Value = "some value";
            state.Lock();
            return state;
        }

        private static RootCollectionState SetupStateCollection()
        {
            var state = new RootCollectionState();
            state.Items.Add(new StateA()
            {
                Value = "ValueIdx0",
            });
            state.Items.Add(new StateA()
            {
                Value = "ValueIdx1",
            });
            state.Items.Add(new StateA()
            {
                Value = "ValueIdx2",
            });
            state.Lock();
            return state;
        }

        private static Mock<IStateFactory> SetupStateFactoryMock()
        {
            var stateFactoryMock = new Mock<IStateFactory>();
            stateFactoryMock.Setup(f => f.Create(typeof(IStateA))).Returns(() => new StateA());
            stateFactoryMock.Setup(f => f.Create(typeof(IRootCollectionState))).Returns(() => new RootCollectionState());
            return stateFactoryMock;
        }

        private static void AssertSameState(IStateA state, IStateA deserializedState)
        {
            Assert.NotNull(deserializedState);

            Assert.Equal(state.Value, deserializedState.Value);
            Assert.Equal(state.Version, deserializedState.Version);
            Assert.False(deserializedState.IsLocked);
        }

        private static void AssertSameState(RootCollectionState state, IRootCollectionState deserializedState)
        {
            Assert.NotNull(deserializedState);

            Assert.Equal(state.Items.Count, deserializedState.Items.Count);

            for (int i = 0; i < state.Items.Count; i++)
            {
                AssertSameState(state.Items.ElementAt(i), deserializedState.Items.ElementAt(i));
            }

            Assert.False(deserializedState.IsLocked);
        }

        private static void AssertStateAJson(string json, StateA state)
        {
            Assert.NotNull(json);

            Assert.Contains(nameof(state.Value), json, StringComparison.InvariantCulture);
            Assert.Contains(state.Value, json, StringComparison.InvariantCulture);

            Assert.Contains(nameof(state.Version), json, StringComparison.InvariantCulture);
            Assert.Contains($"{state.Version}", json, StringComparison.InvariantCulture);

            Assert.DoesNotContain(nameof(state.IsLocked), json, StringComparison.InvariantCulture);
            Assert.DoesNotContain(nameof(state.Identity), json, StringComparison.InvariantCulture);
        }

        private static void AssertRootCollectionStateJson(string json, RootCollectionState state)
        {
            Assert.NotNull(json);

            Assert.Contains(nameof(state.Items), json, StringComparison.InvariantCulture);

            for (int i = 0; i < state.Items.Count; i++)
            {
                Assert.Contains(state.Items.ElementAt(i).Value, json, StringComparison.InvariantCulture);
            }

            Assert.DoesNotContain(nameof(state.IsLocked), json, StringComparison.InvariantCulture);
            Assert.DoesNotContain(nameof(state.Identity), json, StringComparison.InvariantCulture);
        }
    }
}
