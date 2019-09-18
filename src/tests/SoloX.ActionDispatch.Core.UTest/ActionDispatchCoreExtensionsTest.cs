// ----------------------------------------------------------------------
// <copyright file="ActionDispatchCoreExtensionsTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.Sample.State.Basic.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Core.UTest
{
    public class ActionDispatchCoreExtensionsTest
    {
        [Fact]
        public void ServiceCollectionSetupTest()
        {
            var scMock = new Mock<IServiceCollection>();

            var descriptors = new List<ServiceDescriptor>();
            scMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback((Action<ServiceDescriptor>)descriptors.Add);

            var sc = scMock.Object.AddActionDispatchSupport<IStateA>(f => new StateA());

            Assert.Same(scMock.Object, sc);

            Assert.Equal(2, descriptors.Count);
            Assert.Contains(
                descriptors,
                d => d.ServiceType == typeof(IStateFactory)
                    && d.ImplementationType == typeof(StateFactory)
                    && d.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(
                descriptors,
                d => d.ServiceType == typeof(IDispatcher<IStateA>)
                    && d.ImplementationFactory != null
                    && d.Lifetime == ServiceLifetime.Singleton);
        }
    }
}
