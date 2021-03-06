﻿// ----------------------------------------------------------------------
// <copyright file="ActionDispatchJsonStateExtensions.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Json.State.Impl;

namespace SoloX.ActionDispatch.Json.State
{
    /// <summary>
    /// ActionDispatch Json Extensions to setup the Json serialization for action and state.
    /// </summary>
    public static class ActionDispatchJsonStateExtensions
    {
        /// <summary>
        /// Setup the Service collection in order to inject Action and State serializer Json implementation.
        /// </summary>
        /// <param name="services">The services collection instance to setup.</param>
        /// <returns>The service collection given as input.</returns>
        public static IServiceCollection AddActionDispatchStateJsonSupport(this IServiceCollection services)
        {
            return services
                .AddSingleton<IStateSerializer, JsonStateSerializer>();
        }
    }
}
