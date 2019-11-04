// ----------------------------------------------------------------------
// <copyright file="ActionMiddlewareTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using Moq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.ITest.Dispatch.Behavior;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.ITest.Dispatch
{
    public class ActionMiddlewareTest
    {
        [Fact]
        public void ActionMiddlewareThrottleTest()
        {
            var middlewareMock = new Mock<IActionMiddleware<IStateA>>();

            middlewareMock
                .Setup(mw => mw.IsApplying(It.IsAny<IActionBehavior>()))
                .Returns(true);
            middlewareMock
                .Setup(mw => mw.Setup(It.IsAny<IObservable<IAction<IStateA, IActionBehavior>>>()))
                .Returns((IObservable<IAction<IStateA, IActionBehavior>> obs) => obs.Throttle(TimeSpan.FromMilliseconds(100)));

            DispatcherTest.SetupAndTestDispatcher<IStateA>(
                new StateA(),
                dispatcher =>
                {
                    using (var waitHandler = new ManualResetEvent(false))
                    {
                        var myText1 = "Some text 1.";
                        var myText2 = "Some text 2.";
                        var actionBehavior1 = new SetTextActionBehavior(myText1);
                        var actionBehavior2 = new SetTextActionBehavior(myText2);
                        IActionBehavior observedBehavior = null;

                        dispatcher.AddMidlleware(middlewareMock.Object);
                        dispatcher.AddObserver(a =>
                        {
                            observedBehavior = a.Behavior;
                            waitHandler.Set();
                        });

                        dispatcher.Dispatch(actionBehavior1, s => s);
                        dispatcher.Dispatch(actionBehavior2, s => s);

                        waitHandler.WaitOne(2000);

                        Assert.Same(actionBehavior2, observedBehavior);
                        middlewareMock.Verify(mw => mw.IsApplying(actionBehavior1));
                        middlewareMock.Verify(mw => mw.IsApplying(actionBehavior2));
                    }
                });
        }
    }
}
