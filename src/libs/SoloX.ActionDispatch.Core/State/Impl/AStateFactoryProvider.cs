// ----------------------------------------------------------------------
// <copyright file="AStateFactoryProvider.cs" company="SoloX Software">
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
    /// Base class of the state factory provider.
    /// </summary>
    public abstract class AStateFactoryProvider : IStateFactoryProvider
    {
        /// <summary>
        /// Register the state type to use in the state factory.
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// Register a new State implementation.
        /// </summary>
        /// <typeparam name="TStateItf">The state interface type.</typeparam>
        /// <typeparam name="TStateImpl">The state implementation type.</typeparam>
        protected static void Register<TStateItf, TStateImpl>()
            where TStateItf : IState
            where TStateImpl : TStateItf, new()
        {
            StateFactory.Register<TStateItf, TStateImpl>();
        }
    }
}
