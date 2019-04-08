// ----------------------------------------------------------------------
// <copyright file="DispatcherTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Microsoft.Extensions.Logging;
using Moq;
using SoloX.ActionDispatch.Core.Impl;
using SoloX.ActionDispatch.Core.Impl.Action;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest
{
    public class DispatcherTest
    {
        [Fact]
        public void DispatchActionTest()
        {
            var logger = Mock.Of<ILogger<Dispatcher<ITestState>>>();

            var actionBehaviorMock = new Mock<IActionBehavior<ITestState, ITestState>>();

            var stateMock = new Mock<ITestState>();

            var transactionalStateMock = new Mock<ITransactionalState<ITestState>>();
            stateMock.Setup(s => s.CreateTransactionalState()).Returns(transactionalStateMock.Object);

            var stateCloneMock = new Mock<ITestState>();

            transactionalStateMock.Setup(ts => ts.State).Returns(stateCloneMock.Object);

            transactionalStateMock.Setup(ts => ts.Patch(stateMock.Object)).Returns(stateCloneMock.Object);

            var dispatcher = new Dispatcher<ITestState>(stateMock.Object, logger);

            dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

            actionBehaviorMock.Verify(ab => ab.Apply(stateCloneMock.Object));
            stateCloneMock.Verify(s => s.Lock());
        }
    }
}
