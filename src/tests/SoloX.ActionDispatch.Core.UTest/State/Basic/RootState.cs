// ----------------------------------------------------------------------
// <copyright file="RootState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Core.UTest.State.Basic
{
    public class RootState : AStateBase<IRootState>, IRootState
    {
        private int value;
        private AStateBase<IStateA> child;

        /// <inheritdoc/>
        public override IRootState Identity => this;

        /// <inheritdoc/>
        public int Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.CheckUnlock();
                this.value = value;
            }
        }

        /// <inheritdoc/>
        public IStateA Child
        {
            get
            {
                return this.child.Identity;
            }

            set
            {
                this.CheckUnlock();
                this.child = value.ToStateBase();
            }
        }

        /// <inheritdoc/>
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IRootState> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            if (this.child.Patch(oldState, newState, out var childPatched))
            {
                patcher = (s) => { s.Child = childPatched.Identity; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override void LockChildren()
        {
            base.LockChildren();
            this.child.Lock();
        }

        /// <inheritdoc/>
        protected override AStateBase<IRootState> CreateAndClone(bool deep)
        {
            var clone = new RootState();

            this.CopyToRootState(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToRootState(RootState state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.value = this.value;

            if (deep)
            {
                state.child = this.child.DeepClone();
            }
            else
            {
                state.child = this.child;
            }
        }
    }
}
