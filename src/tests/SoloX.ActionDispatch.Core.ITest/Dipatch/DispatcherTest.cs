// ----------------------------------------------------------------------
// <copyright file="DispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.ITest.Dipatch.Behavior;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using Xunit;

namespace SoloX.ActionDispatch.Core.ITest.Dipatch
{
    public class DispatcherTest
    {
        [Fact]
        public void ActionObserverTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();

            var state = new StateA();

            state.Lock();

            string myText = "myText";

            var actionBehavior = new SetTextActionBehavior(myText);

            var lastText = string.Empty;
            var lastVersion = -1;
            using (var dispatcher = new Dispatcher<IStateA>(state, logger))
            using (var subscription = dispatcher.State
                .Do(s =>
                {
                    lastVersion = s.Version;
                    lastText = s.Value;
                }).Subscribe())
            {
                // The initial state value is null.
                Assert.Null(lastText);
                Assert.Equal(0, lastVersion);

                dispatcher.Dispatch(actionBehavior, s => s);

                // Once the action is done the state will be set to myText.
                Assert.Equal(myText, lastText);
                Assert.Equal(1, lastVersion);

                dispatcher.Dispatch(actionBehavior, s => s);

                // Since the action won't change the state (set to the same instance value),
                // the version should remain the same.
                Assert.Equal(myText, lastText);
                Assert.Equal(1, lastVersion);
            }
        }
    }
}
