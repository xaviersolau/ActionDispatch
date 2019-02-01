// ----------------------------------------------------------------------
// <copyright file="IDispatcher.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core
{
    /// <summary>
    /// IDispatcher interface. Used to dispatch IAction that will change the root state object hierarchy.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public interface IDispatcher<TRootState>
    {
        /// <summary>
        /// Gets current state.
        /// </summary>
        IObservable<TRootState> State { get; }

        /// <summary>
        /// Dispatch an action on a current state.
        /// </summary>
        /// <param name="action">The action to apply.</param>
        void Dispatch(IAction<TRootState> action);

        /// <summary>
        /// Add an action observer.
        /// </summary>
        /// <param name="observer">The observer to add.</param>
        void AddObserver(Func<IObservable<IAction<TRootState>>, IObservable<IAction<TRootState>>> observer);
    }
}
