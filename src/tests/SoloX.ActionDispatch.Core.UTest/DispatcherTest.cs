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
            var loggerMock = new Mock<ILogger<Dispatcher<object>>>();

            var actionBehaviorMock = new Mock<IActionBehavior<object, object>>();

            var state = new object();
            var dispatcher = new Dispatcher<object>(state, loggerMock.Object);

            dispatcher.Dispatch(actionBehaviorMock.Object, s => s);

            actionBehaviorMock.Verify(ab => ab.Apply(state));
        }
    }
}
