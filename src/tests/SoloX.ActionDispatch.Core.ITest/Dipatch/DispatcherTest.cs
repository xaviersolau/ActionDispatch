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
using SoloX.ActionDispatch.Core.Action;
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

            var myText1 = "Some text 1.";
            var myText2 = "Some text 2.";
            var actionBehavior1 = new SetTextActionBehavior(myText1);
            var actionBehavior2 = new SetTextActionBehavior(myText2);

            IActionBehavior observedBehavior = null;

            state.Lock();
            using (var dispatcher = new Dispatcher<IStateA>(state, logger))
            {
                dispatcher.AddObserver(o => o.Do(a => observedBehavior = a.Behavior));

                dispatcher.Dispatch(actionBehavior1, s => s);

                Assert.Same(actionBehavior1, observedBehavior);

                dispatcher.Dispatch(actionBehavior2, s => s);

                Assert.Same(actionBehavior2, observedBehavior);
            }
        }

        [Fact]
        public void StateObserverTest()
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
