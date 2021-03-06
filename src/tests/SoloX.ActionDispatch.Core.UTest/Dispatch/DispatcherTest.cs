// ----------------------------------------------------------------------
// <copyright file="DispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.Dispatch
{
    public class DispatcherTest
    {
        [Fact]
        public void DispatchActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();

            var actionBehaviorMock = new Mock<IActionBehavior<IStateA>>();

            var state = new StateA();

            IStateA clone = null;

            actionBehaviorMock.Setup(ab => ab.Apply(It.IsAny<IStateContainer<IStateA>>()))
                .Callback<IStateContainer<IStateA>>(sc =>
                {
                    sc.LoadState();
                    clone = sc.State;
                });

            using (var dispatcher = new Dispatcher<IStateA>(state, logger))
            {
                dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

                actionBehaviorMock.Verify(ab => ab.Apply(It.IsAny<IStateContainer<IStateA>>()));

                Assert.True(clone.IsLocked);
            }
        }

        [Fact]
        public void DispatchAsyncActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();

            var actionBehaviorMock = new Mock<IActionBehaviorAsync<IStateA>>();

            var stateMock = new Mock<IStateA>();

            var isThreadPoolThread = false;
            using (var waitHandler = new ManualResetEvent(false))
            {
                actionBehaviorMock
                    .Setup(ab => ab.Apply(It.IsAny<IRelativeDispatcher<IStateA>>(), stateMock.Object))
                    .Callback(() =>
                    {
                        isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
                        waitHandler.Set();
                    });

                using (var dispatcher = new Dispatcher<IStateA>(stateMock.Object, logger))
                {
                    dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

                    waitHandler.WaitOne(2000);
                }
            }

            actionBehaviorMock.Verify(ab => ab.Apply(It.IsAny<IRelativeDispatcher<IStateA>>(), stateMock.Object));
            Assert.True(isThreadPoolThread);
            Assert.False(Thread.CurrentThread.IsThreadPoolThread);
        }

        [Fact]
        public void DispatcherUseCallingStrategyWithSyncActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();
            var actionBehaviorMock = new Mock<IActionBehavior<IStateA>>();

            var callingStrategyMock = new Mock<ICallingStrategy>();

            using (var dispatcher = new Dispatcher<IStateA>(
                new StateA(), logger, callingStrategyMock.Object))
            {
                dispatcher.Dispatch(actionBehaviorMock.Object, s => s);
            }

            callingStrategyMock.Verify(a => a.Invoke(It.IsAny<System.Action>()));
        }

        [Fact]
        public void DispatcherUseCallingStrategyWithAsyncActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();
            var actionBehaviorMock = new Mock<IActionBehaviorAsync<IStateA>>();

            var callingStrategyMock = new Mock<ICallingStrategy>();

            using (var dispatcher = new Dispatcher<IStateA>(
                new StateA(), logger, callingStrategyMock.Object))
            {
                dispatcher.Dispatch(actionBehaviorMock.Object, s => s);
            }

            callingStrategyMock.Verify(a => a.Invoke(It.IsAny<System.Action>()));
        }

        [Fact]
        public void SynchronizedCallingStrategyTest()
        {
            var strategy = new SynchronizedCallingStrategy(new TestSynchronizationContext());

            var done = false;
            strategy.Invoke(() => done = true);

            Assert.True(done);
        }

        private class TestSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state)
            {
                d(state);
            }
        }
    }
}
