// ----------------------------------------------------------------------
// <copyright file="ActionJsonTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.UTest.Action.Basic;
using SoloX.ActionDispatch.Core.UTest.State.Basic;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Action
{
    public class ActionJsonTest
    {
        [Fact]
        public void SimpleActionSerializationTest()
        {
            var someText = "Test";
            var action = new SyncAction<IStateA, IStateA>(new SyncBehavior(someText), s => s);
            var jsonAction = JsonConvert.SerializeObject(action);

            var valueMap = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonAction);

            Assert.False((bool)valueMap["isAsynchronous"]);

            var behaviorTypeName = (string)valueMap["behaviorType"];
            var behaviorType = Type.GetType(behaviorTypeName);
            Assert.Equal(typeof(SyncBehavior), behaviorType);

            Assert.Equal("s => s", valueMap["selector"]);

            var jobj = (JObject)valueMap["behavior"];
            var behavior = (SyncBehavior)jobj.ToObject(behaviorType);

            Assert.Equal(someText, behavior.SomeValue);
        }

        [Fact]
        public void SimpleActionDeserializationTest()
        {
            var someText = "Test";
            var action = new SyncAction<IStateA, IStateA>(new SyncBehavior(someText), s => s);
            var jsonAction = JsonConvert.SerializeObject(action);

            var deserializedAction = JsonConvert.DeserializeObject<SyncAction<IStateA, IStateA>>(jsonAction);

            Assert.NotNull(deserializedAction);

            var behavior = Assert.IsType<SyncBehavior>(deserializedAction.Behavior);

            Assert.Equal(someText, behavior.SomeValue);
            Assert.NotNull(deserializedAction.Selector);
            Func<IStateA, IStateA> exp = Assert.IsType<Func<IStateA, IStateA>>(deserializedAction.Selector.Compile());

            var state = new StateA();
            Assert.Same(state, exp(state));
        }
    }
}
