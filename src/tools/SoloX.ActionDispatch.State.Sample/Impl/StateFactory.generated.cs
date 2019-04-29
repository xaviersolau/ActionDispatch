// ----------------------------------------------------------------------
// <copyright file="StateFactory.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Sample.Impl;

namespace SoloX.ActionDispatch.State.Sample.Impl
{
    /// <summary>
    /// StateFactoryPattern implementation.
    /// </summary>
    public class StateFactory : IStateFactory
    {
        static StateFactory()
        {
            Key<IMyChildState>.Create = () => new MyChildState();
            Key<IMyRootState>.Create = () => new MyRootState();
        }

        /// <inheritdoc/>
        public TState Create<TState>()
            where TState : IState<TState>
        {
            var create = Key<TState>.Create;
            if (create == null)
            {
            }

            return create();
        }

        private static class Key<T>
            where T : IState<T>
        {
            internal static Func<T> Create { get; set; }
        }
    }
}
