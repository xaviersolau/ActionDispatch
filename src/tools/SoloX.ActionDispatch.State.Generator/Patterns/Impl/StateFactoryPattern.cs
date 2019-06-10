﻿// ----------------------------------------------------------------------
// <copyright file="StateFactoryPattern.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.State.Generator.Patterns.Impl;
using SoloX.ActionDispatch.State.Generator.Patterns.Itf;

namespace SoloX.ActionDispatch.State.Generator.Patterns.Impl
{
    /// <summary>
    /// StateFactoryPattern implementation.
    /// </summary>
    public class StateFactoryPattern : IStateFactory
    {
        static StateFactoryPattern()
        {
            Key<IParentStatePattern>.Create = () => new ParentStatePattern();
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

        private static class Key<T>
            where T : IState
        {
            internal static Func<T> Create { get; set; }
        }
    }
}
