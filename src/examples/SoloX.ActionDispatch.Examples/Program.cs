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
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Dispatch;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.Utils;
using SoloX.ActionDispatch.Examples.ActionBehavior;
using SoloX.ActionDispatch.Examples.State;
using SoloX.ActionDispatch.Json;

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
            sc.AddActionDispatchSupport(
                factory =>
                {
                    var state = factory.Create<IExampleAppState>();
                    state.ChildState = factory.Create<IExampleChildState>();
                    return state;
                });
            sc.AddActionDispatchJsonSupport();

            sc.AddSingleton<IStateFactoryProvider, Impl.StateFactoryProvider>();

            this.Service = sc.BuildServiceProvider();

            this.logger = this.Service.GetService<ILogger<Program>>();
        }

        private ServiceProvider Service { get; }

        /// <summary>
        /// Main program entry point.
        /// </summary>
        public static void Main()
        {
            using (var program = new Program())
            {
                program.Run();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Service.Dispose();
        }

        private void Run()
        {
            var dispatcher = this.Service.GetService<IDispatcher<IExampleAppState>>();

            var actionSerializer = this.Service.GetService<IActionSerializer>();
            var stateSerializer = this.Service.GetService<IStateSerializer>();

            dispatcher.AddObserver(new Observer(this.logger, actionSerializer));

            dispatcher.AddMidlleware(new Middleware());

            using (var stateSubscribtion = dispatcher.State
                .SelectWhenChanged(s => s)
                .Do(s =>
                {
                    this.logger.LogWarning(stateSerializer.Serialize(s));
                })
                .CatchAndContinue<IExampleAppState, NotImplementedException>(e =>
                {
                    this.logger.LogError(e.Message);
                })
                .Subscribe())
            {
                dispatcher.Dispatch(new ExampleAsyncActionBehavior(), s => s.ChildState);
                dispatcher.Dispatch(new ExampleActionBehavior(1), s => s.ChildState);

                dispatcher.Dispatch(new ExampleActionBehavior(3), s => s.ChildState);

                Console.ReadLine();
            }
        }

        internal class Middleware : IActionMiddleware<IExampleAppState>
        {
            public bool IsApplying(IActionBehavior actionBehavior)
            {
                return actionBehavior is ExampleActionBehavior;
            }

            public IObservable<IAction<IExampleAppState, IActionBehavior>> Setup(
                IObservable<IAction<IExampleAppState, IActionBehavior>> actionObservable)
            {
                return actionObservable.Throttle(TimeSpan.FromMilliseconds(3000));
            }
        }

        internal class Observer : IActionObserver<IExampleAppState>
        {
            private ILogger<Program> logger;
            private IActionSerializer actionSerializer;

            public Observer(ILogger<Program> logger, IActionSerializer actionSerializer)
            {
                this.logger = logger;
                this.actionSerializer = actionSerializer;
            }

            public bool IsObserving(IActionBehavior actionBehavior)
            {
                return true;
            }

            public void Observe(IAction<IExampleAppState, IActionBehavior> action, IExampleAppState producedState)
            {
                this.logger.LogWarning($"Observing action with behavior: {action.Behavior}");

                this.logger.LogWarning(actionSerializer.Serialize(action));

                // if (!(action.Behavior is IUnhandledExceptionBehavior<IExampleAppState>))
                // {
                //     throw new NotImplementedException("An error in the action observer.");
                // }
            }
        }
    }
}
