// ----------------------------------------------------------------------
// <copyright file="StateGeneratorServiceCollectionEx.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using SoloX.ActionDispatch.State.Generator.Impl;
using SoloX.GeneratorTools.Core.CSharp;

namespace SoloX.ActionDispatch.State.Generator
{
    /// <summary>
    /// StataGenerator ServiceCollection extensions.
    /// </summary>
    public static class StateGeneratorServiceCollectionEx
    {
        /// <summary>
        /// Add dependency injections for the state generator.
        /// </summary>
        /// <param name="services">The service collection where to setup dependencies.</param>
        /// <returns>The input services once setup is done.</returns>
        public static IServiceCollection AddStateGenerator(this IServiceCollection services)
        {
            return services
                .AddCSharpToolsGenerator()
                .AddTransient<IStateGenerator, StateGenerator>();
        }
    }
}
