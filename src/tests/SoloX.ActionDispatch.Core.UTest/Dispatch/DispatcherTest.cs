// ----------------------------------------------------------------------
// <copyright file="DispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
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

            var actionBehaviorMock = new Mock<IActionBehavior<IStateA, IStateA>>();

            var stateCloneMock = new Mock<IStateA>();
            var state = CreateStateMockWithAClone(stateCloneMock.Object);

            using (var dispatcher = new Dispatcher<IStateA>(state, logger))
            {
                dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

                actionBehaviorMock.Verify(ab => ab.Apply(stateCloneMock.Object));
                stateCloneMock.Verify(s => s.Lock());
            }
        }

        [Fact]
        public void DispatchAsyncActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<IStateA>>>();

            var actionBehaviorMock = new Mock<IActionBehaviorAsync<IStateA, IStateA>>();

            var stateMock = new Mock<IStateA>();

            var isThreadPoolThread = false;
            var waitHandler = new AutoResetEvent(false);

            actionBehaviorMock
                .Setup(ab => ab.Apply(It.IsAny<IDispatcher<IStateA>>(), stateMock.Object))
                .Callback(() =>
                {
                    isThreadPoolThread = Thread.CurrentThread.IsThreadPoolThread;
                    waitHandler.Set();
                });

            using (var dispatcher = new Dispatcher<IStateA>(stateMock.Object, logger))
            {
                dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

                actionBehaviorMock.Verify(ab => ab.Apply(dispatcher, stateMock.Object));
            }

            waitHandler.WaitOne(2000);

            Assert.True(isThreadPoolThread);
            Assert.False(Thread.CurrentThread.IsThreadPoolThread);
        }

        private static TState CreateStateMockWithAClone<TState>(TState clone)
            where TState : class, IState<TState>
        {
            var stateMock = new Mock<TState>();

            var transactionalStateMock = new Mock<ITransactionalState<TState, TState>>();

            stateMock.Setup(s => s.CreateTransactionalState(It.IsAny<TState>())).Returns(transactionalStateMock.Object);

            transactionalStateMock.Setup(ts => ts.State).Returns(clone);

            transactionalStateMock.Setup(ts => ts.Close()).Returns(clone);

            transactionalStateMock.Setup(ts => ts.Dispose()).Callback(clone.Lock);

            return stateMock.Object;
        }
    }
}
