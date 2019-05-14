// ----------------------------------------------------------------------
// <copyright file="StateB.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.Sample.State.Basic;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Core.Sample.State.Basic.Impl
{
    /// <summary>
    /// State implementation pattern.
    /// </summary>
    public class StateB : AStateBase<IStateB>, IStateB
    {
        private string value;
        private AStateBase<IStateBa> child;

        /// <inheritdoc/>
        public override IStateB Identity => this;

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.value != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.value = value;
                }
            }
        }

        /// <inheritdoc/>
        public IStateBa Child
        {
            get
            {
                return this.child?.Identity;
            }

            set
            {
                if (this.child != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.child = value?.ToStateBase();
                }
            }
        }

        /// <inheritdoc/>
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IStateB> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            if (this.child != null && this.child.Patch(oldState, newState, out var childPatched))
            {
                patcher = (s) => { s.Child = childPatched.Identity; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override bool LockChildrenAndCheckDirty()
        {
            var dirty = base.LockChildrenAndCheckDirty();

            if (this.child != null)
            {
                var oldVersion = this.child.Version;
                this.child.Lock();
                dirty |= oldVersion != this.child.Version;
            }

            return dirty;
        }

        /// <inheritdoc/>
        protected override AStateBase<IStateB> CreateAndClone(bool deep)
        {
            var clone = new StateB();

            this.CopyToStateB(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToStateB(StateB state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.value = this.value;

            if (deep)
            {
                state.child = this.child?.DeepClone();
            }
            else
            {
                state.child = this.child;
            }
        }
    }
}
