﻿// ----------------------------------------------------------------------
// <copyright file="ActionDispatchExtensions.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Core
{
    /// <summary>
    /// ActionDispatch Extensions to setup the action dispatcher.
    /// </summary>
    public static class ActionDispatchExtensions
    {
        /// <summary>
        /// Setup the action dispatcher services.
        /// </summary>
        /// <typeparam name="TRootState">The root type of the action dispatcher.</typeparam>
        /// <param name="services">The services collection to setup.</param>
        /// <param name="stateInit">The initial state delegate.</param>
        /// <param name="serviceLifetime">Dispatcher service lifetime.</param>
        /// <param name="useSynchronizationContext">Tells if the current SynchronizationContext must be used.</param>
        /// <returns>The service collection given as input.</returns>
        public static IServiceCollection AddActionDispatchSupport<TRootState>(
            this IServiceCollection services,
            Func<IStateFactory, TRootState> stateInit,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
            bool useSynchronizationContext = false)
            where TRootState : IState
        {
            return AddActionDispatchSupport(
                services,
                (provider, factory) => stateInit(factory),
                serviceLifetime,
                useSynchronizationContext);
        }

        /// <summary>
        /// Setup the action dispatcher services.
        /// </summary>
        /// <typeparam name="TRootState">The root type of the action dispatcher.</typeparam>
        /// <param name="services">The services collection to setup.</param>
        /// <param name="stateInit">The initial state delegate.</param>
        /// <param name="serviceLifetime">Dispatcher service lifetime.</param>
        /// <param name="useSynchronizationContext">Tells if the current SynchronizationContext must be used.</param>
        /// <returns>The service collection given as input.</returns>
        public static IServiceCollection AddActionDispatchSupport<TRootState>(
            this IServiceCollection services,
            Func<IServiceProvider, IStateFactory, TRootState> stateInit,
            ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
            bool useSynchronizationContext = false)
            where TRootState : IState
        {
            if (services == null)
            {
                throw new ArgumentNullException($"{nameof(services)} must not be null.");
            }

            services.Add(ServiceDescriptor.Describe(
                typeof(IDispatcher<TRootState>),
                provider =>
                {
                    var factory = provider.GetService<IStateFactory>();
                    var state = stateInit(provider, factory);

                    state.Lock();

                    if (useSynchronizationContext && SynchronizationContext.Current == null)
                    {
                        throw new NotSupportedException("Could not find any synchronization context in SynchronizationContext.Current.");
                    }

                    return new Dispatcher<TRootState>(
                        state,
                        provider.GetService<ILogger<Dispatcher<TRootState>>>(),
                        useSynchronizationContext ? new SynchronizedCallingStrategy(SynchronizationContext.Current) : null);
                },
                serviceLifetime));

            return services
                .AddSingleton<IStateFactory, StateFactory>();
        }
    }
}
