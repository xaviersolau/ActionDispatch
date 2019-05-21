// ----------------------------------------------------------------------
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
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.Dispatch.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Examples.ActionBehavior;
using SoloX.ActionDispatch.Examples.State;

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
            sc.AddSingleton<IStateFactory>(new Impl.StateFactory());
            sc.AddSingleton<IDispatcher<IExampleAppState>>(
                r =>
                {
                    var factory = r.GetService<IStateFactory>();
                    var state = factory.Create<IExampleAppState>();
                    state.ChildState = factory.Create<IExampleChildState>();

                    state.Lock();

                    return new Dispatcher<IExampleAppState>(
                        state,
                        r.GetService<ILogger<Dispatcher<IExampleAppState>>>());
                });

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
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Service.Dispose();
        }

        private void Run()
        {
            var dispatcher = this.Service.GetService<IDispatcher<IExampleAppState>>();

            dispatcher.AddObserver(obs => obs.Do(a =>
            {
                this.logger.LogWarning($"Processing action with behavior: {a.Behavior}");

                this.logger.LogWarning(JsonConvert.SerializeObject(a));
            }));

            using (var stateSubscribtion = dispatcher.State.Do(s =>
            {
                this.logger.LogWarning($"State: {s.Version}");

                this.logger.LogWarning(JsonConvert.SerializeObject(s));
            }).Subscribe())
            {
                dispatcher.Dispatch(new ExampleAsyncActionBehavior(), s => s.ChildState);

                dispatcher.Dispatch(new ExampleActionBehavior(1), s => s.ChildState);

                dispatcher.Dispatch(new ExampleActionBehavior(3), s => s.ChildState);

                Console.ReadLine();
            }
        }
    }
}
