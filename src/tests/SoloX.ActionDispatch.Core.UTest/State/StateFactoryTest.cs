// ----------------------------------------------------------------------
// <copyright file="StateFactoryTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest.State
{
    public class StateFactoryTest
    {
        [Fact]
        public void CreateRegisteredStateFromStateFactory()
        {
            var factory = new StateFactory(Array.Empty<IStateFactoryProvider>());

            StateFactory.Register<IStateA, StateA>();

            var state1 = factory.Create<IStateA>();

            Assert.NotNull(state1);
            Assert.IsType<StateA>(state1);

            var state2 = factory.Create(typeof(IStateA));

            Assert.NotNull(state2);
            Assert.IsType<StateA>(state2);

            factory.Reset();
        }

        [Fact]
        public void CreateUnregisteredStateFromStateFactory()
        {
            var factory = new StateFactory(Array.Empty<IStateFactoryProvider>());

            Assert.Throws<ArgumentException>(() => factory.Create<IStateA>());

            Assert.Throws<ArgumentException>(() => factory.Create(typeof(IStateA)));

            factory.Reset();
        }

        [Fact]
        public void CallRegisterInStateFactoryConstructor()
        {
            var providerMock = new Mock<IStateFactoryProvider>();

            var factory = new StateFactory(new IStateFactoryProvider[] { providerMock.Object });

            providerMock.Verify(provider => provider.Register(), Times.Once);
        }
    }
}
