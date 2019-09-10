// ----------------------------------------------------------------------
// <copyright file="StateJsonTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.Sample.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
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
        public void StateSerializerDeserializationTest()
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

        private static Mock<IStateFactory> SetupStateFactoryMock()
        {
            var stateFactoryMock = new Mock<IStateFactory>();
            stateFactoryMock.Setup(f => f.Create(typeof(IStateA))).Returns(new StateA());
            return stateFactoryMock;
        }

        private static void AssertSameState(StateA state, IStateA deserializedState)
        {
            Assert.NotNull(deserializedState);

            Assert.Equal(state.Value, deserializedState.Value);
            Assert.Equal(state.Version, deserializedState.Version);
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
    }
}
