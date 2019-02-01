// ----------------------------------------------------------------------
// <copyright file="ActionBase.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <summary>
    /// Action base implementation.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    public abstract class ActionBase<TRootState> : IAction<TRootState>
    {
        /// <summary>
        /// Gets or sets action state.
        /// </summary>
        public ActionState State { get; set; }

        /// <inheritdoc/>
        public abstract TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState state);
    }

#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// Action base implementation.
    /// </summary>
    /// <typeparam name="TRootState">Type of the root state object on witch actions will apply.</typeparam>
    /// <typeparam name="TTargetState">Type of the targeted state object.</typeparam>
    public abstract class ActionBase<TRootState, TTargetState> : ActionBase<TRootState>, IAction<TRootState, TTargetState>
#pragma warning restore SA1402 // File may only contain a single type
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionBase{TRootState, TTargetState}"/> class.
        /// </summary>
        /// <param name="stateSelector">expression path to target the state.</param>
        protected ActionBase(Expression<Func<TRootState, TTargetState>> stateSelector)
        {
            this.StateSelector = stateSelector;
        }

        /// <inheritdoc/>
        public Expression<Func<TRootState, TTargetState>> StateSelector { get; }
    }
}
