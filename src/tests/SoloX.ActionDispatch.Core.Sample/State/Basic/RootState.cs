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

namespace SoloX.ActionDispatch.Core.Sample.State.Basic
{
    public class RootState : AStateBase<IRootState>, IRootState
    {
        private int value;
        private AStateBase<IStateA> child1;
        private AStateBase<IStateA> child2;

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
                if (this.value != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.value = value;
                }
            }
        }

        /// <inheritdoc/>
        public IStateA Child1
        {
            get
            {
                return this.child1?.Identity;
            }

            set
            {
                if (this.child1 != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.child1 = value?.ToStateBase();
                }
            }
        }

        /// <inheritdoc/>
        public IStateA Child2
        {
            get
            {
                return this.child2?.Identity;
            }

            set
            {
                if (this.child2 != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.child2 = value?.ToStateBase();
                }
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

            if (this.child1 != null && this.child1.Patch(oldState, newState, out var child1Patched))
            {
                patcher = (s) => { s.Child1 = child1Patched.Identity; };
                return true;
            }

            if (this.child2 != null && this.child2.Patch(oldState, newState, out var child2Patched))
            {
                patcher = (s) => { s.Child2 = child2Patched.Identity; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override bool LockChildrenAndCheckDirty()
        {
            var dirty = base.LockChildrenAndCheckDirty();

            if (this.child1 != null)
            {
                var oldVersion = this.child1.Version;
                this.child1.Lock();
                dirty |= oldVersion != this.child1.Version;
            }

            if (this.child2 != null)
            {
                var oldVersion = this.child2.Version;
                this.child2.Lock();
                dirty |= oldVersion != this.child2.Version;
            }

            return dirty;
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
                state.child1 = this.child1?.DeepClone();
                state.child2 = this.child2?.DeepClone();
            }
            else
            {
                state.child1 = this.child1;
                state.child2 = this.child2;
            }
        }
    }
}
