// ----------------------------------------------------------------------
// <copyright file="StateFactory.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State.Impl
{
    /// <summary>
    /// The StateFactory collecting all state factory available.
    /// </summary>
    public class StateFactory : IStateFactory
    {
        private static readonly Dictionary<Type, Func<IState>> Map = new Dictionary<Type, Func<IState>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StateFactory"/> class.
        /// </summary>
        /// <param name="providers">The providers to register.</param>
        public StateFactory(IEnumerable<IStateFactoryProvider> providers)
        {
            if (providers != null)
            {
                foreach (var provider in providers)
                {
                    provider.Register();
                }
            }
        }

        /// <inheritdoc/>
        public TState Create<TState>()
            where TState : IState
        {
            var create = Key<TState>.Create;
            if (create == null)
            {
                throw new ArgumentException($"Unknown state type: {typeof(TState).Name}.");
            }

            return create();
        }

        /// <inheritdoc/>
        public IState Create(Type stateType)
        {
            if (!Map.TryGetValue(stateType, out var create))
            {
                throw new ArgumentException($"Unknown state type: {stateType?.Name}.");
            }

            return create();
        }

        /// <summary>
        /// Register a new State implementation.
        /// </summary>
        /// <typeparam name="TStateItf">The state interface type.</typeparam>
        /// <typeparam name="TStateImpl">The state implementation type.</typeparam>
        internal static void Register<TStateItf, TStateImpl>()
            where TStateItf : IState
            where TStateImpl : TStateItf, new()
        {
            lock (Map)
            {
                if (Key<TStateItf>.Create == null)
                {
                    Key<TStateItf>.Create = () => new TStateImpl();
                    Map.Add(typeof(TStateItf), () => new TStateImpl());
                }
            }
        }

        private static class Key<T>
            where T : IState
        {
            internal static Func<T> Create { get; set; }
        }
    }
}
