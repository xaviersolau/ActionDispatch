﻿// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Impl;

namespace SoloX.ActionDispatch.Examples
{
    /// <summary>
    /// Example program.
    /// </summary>
    public sealed class Program : IDisposable
    {
        private readonly ILogger<Program> logger;

        private Program()
        {
            IServiceCollection sc = new ServiceCollection();

            sc.AddLogging(b => b.AddConsole());

            sc.AddSingleton<IDispatcher<ExampleState>>(
                r => new Dispatcher<ExampleState>(
                    new ExampleState(),
                    r.GetService<ILogger<Dispatcher<ExampleState>>>()));

            this.Service = sc.BuildServiceProvider();

            this.logger = this.Service.GetService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main()
        {
            new Program().Run();
            Console.ReadLine();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Service.Dispose();
        }

        private void Run()
        {
            var dispatcher = this.Service.GetService<IDispatcher<ExampleState>>();

            dispatcher.AddObserver(obs => obs.Do(a => this.logger.LogWarning($"Processing action: {a}")));

            dispatcher.Dispatch(new ExampleAction());
        }
    }
}
