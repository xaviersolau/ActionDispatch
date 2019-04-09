// ----------------------------------------------------------------------
// <copyright file="ExampleAppState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Examples.State.Impl
{
    /// <summary>
    /// Root example state object.
    /// </summary>
    public class ExampleAppState : AStateBase<IExampleAppState>, IExampleAppState
    {
        private int appCount;
        private AStateBase<IExampleChildState> childState;

        /// <inheritdoc/>
        public override IExampleAppState Identity => this;

        /// <inheritdoc/>
        public int AppCount
        {
            get
            {
                return this.appCount;
            }

            set
            {
                this.CheckUnlock();
                this.appCount = value;
            }
        }

        /// <inheritdoc/>
        public IExampleChildState ChildState
        {
            get
            {
                return this.childState.Identity;
            }

            set
            {
                this.CheckUnlock();
                this.childState = value.ToStateBase();
            }
        }

        /// <inheritdoc/>
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IExampleAppState> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            if (this.childState.Patch(oldState, newState, out var childStatePatched))
            {
                patcher = (s) => { s.ChildState = childStatePatched as IExampleChildState; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override void LockChildren()
        {
            base.LockChildren();
            this.childState.Lock();
        }

        /// <inheritdoc/>
        protected override AStateBase<IExampleAppState> CreateAndDeepClone()
        {
            var clone = new ExampleAppState();

            this.CopyToExampleAppState(clone, true);

            return clone;
        }

        /// <inheritdoc/>
        protected override AStateBase<IExampleAppState> CreateAndClone()
        {
            var clone = new ExampleAppState();

            this.CopyToExampleAppState(clone, false);

            return clone;
        }

        /// <inheritdoc/>
        protected override void PropagateLock()
        {
            this.childState.Lock();
        }

        /// <summary>
        /// Copy current ExampleAppState data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToExampleAppState(ExampleAppState state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.appCount = this.appCount;

            if (deep)
            {
                state.childState = this.childState.DeepClone();
            }
            else
            {
                state.childState = this.childState;
            }
        }
    }
}
