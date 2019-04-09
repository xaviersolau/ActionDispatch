// ----------------------------------------------------------------------
// <copyright file="ExampleStateFactory.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Examples.State.Impl
{
    internal class ExampleStateFactory : IStateFactory
    {
        static ExampleStateFactory()
        {
            Key<IExampleAppState>.Create = () => new ExampleAppState();
            Key<IExampleChildState>.Create = () => new ExampleChildState();
        }

        public TState Create<TState>()
            where TState : IState<TState>
        {
            if (Key<TState>.Create != null)
            {
                return Key<TState>.Create();
            }

            throw new InvalidOperationException("Unknown state type.");
        }

        private static class Key<TState>
        {
            internal static Func<TState> Create { get; set; }
        }
    }
}
