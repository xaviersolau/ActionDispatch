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
using SoloX.ActionDispatch.Core.Dispatch;
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
    }
}
