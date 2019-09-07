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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.UTest.Action.Basic;
using SoloX.ActionDispatch.Json.Action;
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

            string jsonAction = GetJsonAction(async, someText);

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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SimpleActionDeserializationTest(bool async)
        {
            var someText = "Test";

            string jsonAction = GetJsonAction(async, someText);

            AAction<IStateA, IStateA> deserializedAction;
            string behaviorSomeValue;
            if (async)
            {
                var action = JsonConvert.DeserializeObject<AsyncAction<IStateA, IStateA>>(jsonAction, new JsonActionConverter());
                Assert.NotNull(action);

                var behavior = Assert.IsType<AsyncBehavior>(action.Behavior);
                behaviorSomeValue = behavior.SomeValue;
                deserializedAction = action;
            }
            else
            {
                var action = JsonConvert.DeserializeObject<SyncAction<IStateA, IStateA>>(jsonAction, new JsonActionConverter());
                Assert.NotNull(action);

                var behavior = Assert.IsType<SyncBehavior>(action.Behavior);
                behaviorSomeValue = behavior.SomeValue;
                deserializedAction = action;
            }

            Assert.Equal(someText, behaviorSomeValue);
            var selector = deserializedAction.Selector;

            Assert.NotNull(selector);
            Func<IStateA, IStateA> exp = Assert.IsType<Func<IStateA, IStateA>>(selector.Compile());

            var state = new StateA();
            Assert.Same(state, exp(state));
        }

        private static string GetJsonAction(bool async, string someText)
        {
            if (async)
            {
                var action = new AsyncAction<IStateA, IStateA>(new AsyncBehavior(someText), s => s);
                return JsonConvert.SerializeObject(action, new JsonActionConverter());
            }
            else
            {
                var action = new SyncAction<IStateA, IStateA>(new SyncBehavior(someText), s => s);
                return JsonConvert.SerializeObject(action, new JsonActionConverter());
            }
        }
    }
}
