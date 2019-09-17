// ----------------------------------------------------------------------
// <copyright file="ActionDispatchExtensions.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
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
        /// <returns>The service collection given as input.</returns>
        public static IServiceCollection AddActionDispatchSupport<TRootState>(
            this IServiceCollection services,
            Func<IStateFactory, TRootState> stateInit)
            where TRootState : IState
        {
            return services
                .AddSingleton<IStateFactory, StateFactory>()
                .AddSingleton<IDispatcher<TRootState>>(
                r =>
                {
                    var factory = r.GetService<IStateFactory>();
                    var state = stateInit(factory);

                    state.Lock();

                    return new Dispatcher<TRootState>(
                        state,
                        r.GetService<ILogger<Dispatcher<TRootState>>>());
                });
        }
    }
}
