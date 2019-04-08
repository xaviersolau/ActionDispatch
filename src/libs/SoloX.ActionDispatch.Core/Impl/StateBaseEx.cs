// ----------------------------------------------------------------------
// <copyright file="StateBaseEx.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.Impl
{
    /// <summary>
    /// Extension class to handle State object.
    /// </summary>
    public static class StateBaseEx
    {
        /// <summary>
        /// Convert the given state to the AStateBase implementation abstraction.
        /// </summary>
        /// <typeparam name="TState">The type or interface of the state.</typeparam>
        /// <param name="state">The state instance to convert.</param>
        /// <returns>The AStateBase converted state.</returns>
        public static AStateBase<TState> ToStateBase<TState>(this IState<TState> state)
            where TState : IState
        {
            if (state is AStateBase<TState> stateBase)
            {
                return stateBase;
            }

            throw new InvalidCastException("Unsupported state implementation.");
        }
    }
}
