// ----------------------------------------------------------------------
// <copyright file="IDispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Dispatch
{
    /// <summary>
    /// IDispatcher interface. Used to dispatch IAction that will change the root state object hierarchy.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IDispatcher<TRootState> : IRelativeDispatcher<TRootState>
        where TRootState : IState
    {
        /// <summary>
        /// Gets the registered action middle-ware list.
        /// </summary>
        IEnumerable<IActionMiddleware<TRootState>> Middlewares { get; }

        /// <summary>
        /// Gets the registered action observer list.
        /// </summary>
        IEnumerable<IActionObserver<TRootState>> Observers { get; }

        /// <summary>
        /// Add an action middle ware.
        /// </summary>
        /// <param name="middleware">The middle ware to add.</param>
        void AddMidlleware(IActionMiddleware<TRootState> middleware);

        /// <summary>
        /// Add an action observer.
        /// </summary>
        /// <param name="observer">The action observer to add.</param>
        void AddObserver(IActionObserver<TRootState> observer);
    }
}
