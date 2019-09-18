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
        private static readonly Dictionary<Type, Entry> Map
            = new Dictionary<Type, Entry>();

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
            if (!Map.TryGetValue(stateType, out var entry))
            {
                throw new ArgumentException($"Unknown state type: {stateType?.Name}.");
            }

            return entry.Create();
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
                    Map.Add(typeof(TStateItf), new Entry(() => new TStateImpl(), () => Key<TStateItf>.Create = null));
                }
            }
        }

        internal void Reset()
        {
            lock (Map)
            {
                foreach (var item in Map)
                {
                    item.Value.Reset();
                }

                Map.Clear();
            }
        }

        private static class Key<T>
            where T : IState
        {
            internal static Func<T> Create { get; set; }
        }

        private class Entry
        {
            public Entry(Func<IState> create, System.Action reset)
            {
                this.Create = create;
                this.Reset = reset;
            }

            public Func<IState> Create { get; }

            public System.Action Reset { get; }
        }
    }
}
