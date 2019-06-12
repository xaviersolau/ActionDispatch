// ----------------------------------------------------------------------
// <copyright file="DispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Reactive.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.ITest.Dispatch.Behavior;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using Xunit;

namespace SoloX.ActionDispatch.Core.ITest.Dispatch
{
    public class DispatcherTest
    {
        [Fact]
        public void ActionObserverTest()
        {
            SetupAndTestDispatcher<IStateA>(
                new StateA(),
                dispatcher =>
                {
                    var myText1 = "Some text 1.";
                    var myText2 = "Some text 2.";
                    var actionBehavior1 = new SetTextActionBehavior(myText1);
                    var actionBehavior2 = new SetTextActionBehavior(myText2);
                    IActionBehavior observedBehavior = null;

                    dispatcher.AddObserver(o => o.Do(a => observedBehavior = a.Behavior));

                    dispatcher.Dispatch(actionBehavior1, s => s);

                    Assert.Same(actionBehavior1, observedBehavior);

                    dispatcher.Dispatch(actionBehavior2, s => s);

                    Assert.Same(actionBehavior2, observedBehavior);
                });
        }

        [Fact]
        public void StateObserverTest()
        {
            SetupAndTestDispatcher<IStateA>(
                new StateA(),
                dispatcher =>
                {
                    string myText = "myText";

                    var actionBehavior = new SetTextActionBehavior(myText);

                    var lastText = string.Empty;
                    var lastVersion = -1;
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
                });
        }

        [Fact]
        public void ThrowActionTest()
        {
            SetupAndTestDispatcher<IStateA>(
                new StateA(),
                dispatcher =>
                {
                    IActionBehavior observedBehavior = null;

                    dispatcher.AddObserver(o => o.Do(a => observedBehavior = a.Behavior));

                    var throwBehavior = new ThrowActionBehavior();
                    dispatcher.Dispatch(throwBehavior, s => s);

                    var ueb = Assert.IsAssignableFrom<IUnhandledExceptionBehavior<IStateA>>(observedBehavior);
                    Assert.Same(throwBehavior.Exception, ueb.Exception);
                });
        }

        [Fact]
        public void AsyncActionTest()
        {
            SetupAndTestDispatcher<IStateA>(
                new StateA(),
                dispatcher =>
                {
                    var someText = "Some text.";
                    var waitHandle = new ManualResetEvent(false);
                    var behaviorToDelay = new SetTextActionBehavior(someText);
                    var delayBehavior = new DelayActionBehavior(300, behaviorToDelay);

                    dispatcher.AddObserver(o => o.Do(
                        a =>
                        {
                            // Unlock the wait handle as soon as we observed the expected SetTextAction.
                            if (ReferenceEquals(a.Behavior, behaviorToDelay))
                            {
                                waitHandle.Set();
                            }
                        }));

                    IStateA lastState = null;
                    using (var subscription = dispatcher.State
                        .Do(s =>
                        {
                            lastState = s;
                        }).Subscribe())
                    {
                        dispatcher.Dispatch(delayBehavior, s => s);

                        // Wait for the end of the delayed action.
                        Assert.True(waitHandle.WaitOne(1000));

                        // Make sure the state has been set.
                        Assert.NotNull(lastState);
                        Assert.Equal(1, lastState.Version);
                        Assert.Equal(someText, lastState.Value);
                    }
                });
        }

        internal static void SetupAndTestDispatcher<TRootState>(TRootState initialState, Action<IDispatcher<TRootState>> testHandler)
            where TRootState : IState
        {
            var logger = Mock.Of<ILogger<Dispatcher<TRootState>>>();

            initialState.Lock();

            using (var dispatcher = new Dispatcher<TRootState>(initialState, logger))
            {
                testHandler(dispatcher);
            }
        }
    }
}
