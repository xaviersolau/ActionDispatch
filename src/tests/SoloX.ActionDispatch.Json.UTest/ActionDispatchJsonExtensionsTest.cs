// ----------------------------------------------------------------------
// <copyright file="ActionDispatchJsonExtensionsTest.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Json.Action.Impl;
using SoloX.ActionDispatch.Json.State.Impl;
using Xunit;

namespace SoloX.ActionDispatch.Json.UTest
{
    public class ActionDispatchJsonExtensionsTest
    {
        [Fact]
        public void ServiceCollectionSetupTest()
        {
            var scMock = new Mock<IServiceCollection>();

            var descriptors = new List<ServiceDescriptor>();
            scMock.Setup(s => s.Add(It.IsAny<ServiceDescriptor>())).Callback((Action<ServiceDescriptor>)descriptors.Add);

            var sc = scMock.Object.AddActionDispatchJsonSupport();

            Assert.Same(scMock.Object, sc);

            Assert.Equal(2, descriptors.Count);
            Assert.Contains(
                descriptors,
                d => d.ServiceType == typeof(IStateSerializer)
                    && d.ImplementationType == typeof(JsonStateSerializer)
                    && d.Lifetime == ServiceLifetime.Singleton);
            Assert.Contains(
                descriptors,
                d => d.ServiceType == typeof(IActionSerializer)
                    && d.ImplementationType == typeof(JsonActionSerializer)
                    && d.Lifetime == ServiceLifetime.Singleton);
        }
    }
}
