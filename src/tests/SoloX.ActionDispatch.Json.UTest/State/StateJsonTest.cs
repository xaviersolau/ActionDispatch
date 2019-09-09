// ----------------------------------------------------------------------
// <copyright file="StateJsonTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.Sample.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
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
            var state = new StateA();
            state.Value = "some value";
            state.Lock();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(new StateFactory()));

            Assert.NotNull(json);

            Assert.Contains(nameof(state.Value), json, StringComparison.InvariantCulture);
            Assert.Contains(state.Value, json, StringComparison.InvariantCulture);

            Assert.Contains(nameof(state.Version), json, StringComparison.InvariantCulture);
            Assert.Contains($"{state.Version}", json, StringComparison.InvariantCulture);

            Assert.DoesNotContain(nameof(state.IsLocked), json, StringComparison.InvariantCulture);
            Assert.DoesNotContain(nameof(state.Identity), json, StringComparison.InvariantCulture);
        }

        [Fact]
        public void SimpleStateDeserializationTest()
        {
            var state = new StateA();
            state.Value = "some value";
            state.Lock();

            var json = JsonConvert.SerializeObject(state, new JsonStateConverter(new StateFactory()));

            Assert.NotNull(json);

            var deserializedState = JsonConvert.DeserializeObject<IStateA>(json, new JsonStateConverter(new StateFactory()));

            Assert.NotNull(deserializedState);

            Assert.Equal(state.Value, deserializedState.Value);
            Assert.Equal(state.Version, deserializedState.Version);
            Assert.False(deserializedState.IsLocked);
        }
    }
}
