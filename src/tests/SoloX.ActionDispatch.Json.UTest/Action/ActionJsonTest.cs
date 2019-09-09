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
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
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

            var action = JsonConvert.DeserializeObject<AAction<IStateA, IStateA>>(jsonAction, new JsonActionConverter());

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
