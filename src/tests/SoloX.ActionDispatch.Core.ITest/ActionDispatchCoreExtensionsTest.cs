// ----------------------------------------------------------------------
// <copyright file="ActionDispatchCoreExtensionsTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.ITest.Utils;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using Xunit;

namespace SoloX.ActionDispatch.Core.ITest
{
    public class ActionDispatchCoreExtensionsTest
    {
        [Fact]
        public void ServiceCollectionSetupWithInitialStateFactoryTest()
        {
            var expectedState = new StateA();

            var factoryMock = new Mock<IInitialStateFactory<IStateA>>();
            factoryMock.Setup(f => f.Create()).Returns(expectedState);

            IServiceCollection sc = new ServiceCollection();
            sc.AddSingleton(factoryMock.Object);
            sc.AddActionDispatchSupport<IStateA>();
            using (var provider = sc.BuildServiceProvider())
            {
                var dispatcher = provider.GetService<IDispatcher<IStateA>>();
                var state = dispatcher.State.Latest().First();

                Assert.Same(expectedState, state);
            }
        }

        [Fact]
        public void ServiceCollectionSetupWithActionObserverTest()
        {
            var expectedState = new StateA();

            var observer = MockHelpers.SetupObserver<IStateA>((a, s) => { });

            IServiceCollection sc = new ServiceCollection();
            sc.AddSingleton(observer);

            sc.AddActionDispatchSupport<IStateA>(sf => expectedState);
            using (var provider = sc.BuildServiceProvider())
            {
                var dispatcher = provider.GetService<IDispatcher<IStateA>>();

                Assert.NotNull(dispatcher.Observers);
                var observers = dispatcher.Observers.ToArray();
                Assert.Single(observers);
                Assert.True(object.ReferenceEquals(observers[0], observer));
            }
        }

        [Fact]
        public void ServiceCollectionSetupWithActionMiddlewareTest()
        {
            var expectedState = new StateA();

            var middleware1 = MockHelpers.SetupMiddleware<IStateA>(obs => obs);
            var middleware2 = MockHelpers.SetupMiddleware<IStateA>(obs => obs);

            IServiceCollection sc = new ServiceCollection();
            sc.AddSingleton(middleware1);
            sc.AddSingleton(middleware2);

            sc.AddActionDispatchSupport<IStateA>(sf => expectedState);
            using (var provider = sc.BuildServiceProvider())
            {
                var dispatcher = provider.GetService<IDispatcher<IStateA>>();

                Assert.NotNull(dispatcher.Middlewares);
                var middlewares = dispatcher.Middlewares.ToArray();
                Assert.Equal(2, middlewares.Length);
                Assert.True((object.ReferenceEquals(middlewares[0], middleware1) && object.ReferenceEquals(middlewares[1], middleware2))
                    || (object.ReferenceEquals(middlewares[0], middleware2) && object.ReferenceEquals(middlewares[1], middleware1)));
            }
        }
    }
}
