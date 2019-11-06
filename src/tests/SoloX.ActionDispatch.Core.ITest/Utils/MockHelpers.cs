// ----------------------------------------------------------------------
// <copyright file="MockHelpers.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.ITest.Utils
{
    public static class MockHelpers
    {
        public static IActionObserver<TState> SetupObserver<TState>(Action<IAction<TState, IActionBehavior>, TState> handler)
            where TState : IState
        {
            var observerMock = new Mock<IActionObserver<TState>>();
            observerMock
                .Setup(obs => obs.IsObserving(It.IsAny<IActionBehavior>()))
                .Returns(true);
            observerMock
                .Setup(obs => obs.Observe(It.IsAny<IAction<TState, IActionBehavior>>(), It.IsAny<TState>()))
                .Callback(handler);
            return observerMock.Object;
        }

        public static IActionMiddleware<TState> SetupMiddleware<TState>(
            Func<IObservable<IAction<TState, IActionBehavior>>, IObservable<IAction<TState, IActionBehavior>>> handler)
            where TState : IState
        {
            var observerMock = new Mock<IActionMiddleware<TState>>();
            observerMock
                .Setup(obs => obs.IsApplying(It.IsAny<IActionBehavior>()))
                .Returns(true);
            observerMock
                .Setup(mw => mw.Setup(It.IsAny<IObservable<IAction<TState, IActionBehavior>>>()))
                .Returns(handler);

            return observerMock.Object;
        }
    }
}
