// ----------------------------------------------------------------------
// <copyright file="ActionJsonTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.UTest.Action.Basic;
using SoloX.ActionDispatch.Json.Action.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Json.UTest.Action
{
    public class ActionJsonTest
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleActionSerializationTest(bool async)
        {
            var someText = "Test";

            var action = CreateAction(async, someText);

            string jsonAction = JsonConvert.SerializeObject(action, new JsonActionConverter());

            AssertActionJson(jsonAction, someText, async);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionSerializerSerializationTest(bool async)
        {
            var serializer = new JsonActionSerializer(Mock.Of<IStateFactory>());

            var someText = "Test";

            var action = CreateAction(async, someText);

            string jsonAction = serializer.Serialize(action);

            AssertActionJson(jsonAction, someText, async);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleActionDeserializationTest(bool async)
        {
            var someText = "Test";

            var actionBase = CreateAction(async, someText);

            string jsonAction = JsonConvert.SerializeObject(actionBase, new JsonActionConverter());

            var action = JsonConvert.DeserializeObject<AAction<IStateA, IStateA>>(jsonAction, new JsonActionConverter());

            AssertDeserializedAction(action, async, someText);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ActionSerializerDeserializationTest(bool async)
        {
            var serializer = new JsonActionSerializer(Mock.Of<IStateFactory>());

            var someText = "Test";

            var actionBase = CreateAction(async, someText);

            string jsonAction = JsonConvert.SerializeObject(actionBase, new JsonActionConverter());

            var actionItf = serializer.Deserialize<IStateA>(jsonAction);

            var action = Assert.IsAssignableFrom<AAction<IStateA, IStateA>>(actionItf);

            AssertDeserializedAction(action, async, someText);
        }

        [Theory]
        [InlineData(typeof(AAction<IStateA>), true)]
        [InlineData(typeof(AAction<IStateA, IStateA>), true)]
        [InlineData(typeof(AsyncAction<IStateA, IStateA>), true)]
        [InlineData(typeof(SyncAction<IStateA, IStateA>), true)]
        [InlineData(typeof(object), false)]
        public void ActionConverterTypeSupportTest(Type type, bool expected)
        {
            var converter = new JsonActionConverter();
            Assert.Equal(expected, converter.CanConvert(type));
        }

        private static IAction<IStateA, IActionBehavior> CreateAction(bool async, string someText)
        {
            if (async)
            {
                return new AsyncAction<IStateA, IStateA>(new AsyncBehavior(someText), s => s);
            }
            else
            {
                return new SyncAction<IStateA, IStateA>(new SyncBehavior(someText), s => s);
            }
        }

        private static void AssertActionJson(string jsonAction, string someText, bool async)
        {
            var valueMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonAction);

            Assert.Equal(async, (bool)valueMap[JsonActionConverter.IsAsynchronous]);

            var behaviorTypeName = (string)valueMap[JsonActionConverter.BehaviorType];
            var behaviorType = Type.GetType(behaviorTypeName);

            Assert.Equal("s => s", valueMap[JsonActionConverter.Selector]);

            var jobj = (JObject)valueMap[JsonActionConverter.Behavior];

            if (async)
            {
                Assert.Equal(typeof(AsyncBehavior), behaviorType);
                var behavior = (AsyncBehavior)jobj.ToObject(behaviorType);
                Assert.Equal(someText, behavior.SomeValue);
            }
            else
            {
                Assert.Equal(typeof(SyncBehavior), behaviorType);
                var behavior = (SyncBehavior)jobj.ToObject(behaviorType);
                Assert.Equal(someText, behavior.SomeValue);
            }
        }

        private static void AssertDeserializedAction(AAction<IStateA, IStateA> action, bool async, string someText)
        {
            Assert.NotNull(action);

            string behaviorSomeValue;
            if (async)
            {
                var asyncAction = Assert.IsType<AsyncAction<IStateA, IStateA>>(action);

                var behavior = Assert.IsType<AsyncBehavior>(asyncAction.Behavior);
                behaviorSomeValue = behavior.SomeValue;
            }
            else
            {
                var syncAction = Assert.IsType<SyncAction<IStateA, IStateA>>(action);

                var behavior = Assert.IsType<SyncBehavior>(syncAction.Behavior);
                behaviorSomeValue = behavior.SomeValue;
            }

            Assert.Equal(someText, behaviorSomeValue);
            var selector = action.Selector;

            Assert.NotNull(selector);
            Func<IStateA, IStateA> exp = Assert.IsType<Func<IStateA, IStateA>>(selector.Compile());

            var state = new StateA();
            Assert.Same(state, exp(state));
        }
    }
}
