// ----------------------------------------------------------------------
// <copyright file="MyRootState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State.Impl;
using SoloX.ActionDispatch.State.Sample;

namespace SoloX.ActionDispatch.State.Sample.Impl
{
    /// <summary>
    /// State implementation pattern.
    /// </summary>
    public class MyRootState : AStateBase<IMyRootState>, IMyRootState
    {
        private string value1;
        private int value2;
        private AStateBase<IMyChildState> child1;
        private AStateBase<IMyChildState> child2;

        /// <inheritdoc/>
        public override IMyRootState Identity => this;

        /// <inheritdoc/>
        public string Value1
        {
            get
            {
                return this.value1;
            }

            set
            {
                this.CheckUnlock();
                this.value1 = value;
            }
        }

        /// <inheritdoc/>
        public int Value2
        {
            get
            {
                return this.value2;
            }

            set
            {
                this.CheckUnlock();
                this.value2 = value;
            }
        }

        /// <inheritdoc/>
        public IMyChildState Child1
        {
            get
            {
                return this.child1.Identity;
            }

            set
            {
                this.CheckUnlock();
                this.child1 = value.ToStateBase();
            }
        }

        /// <inheritdoc/>
        public IMyChildState Child2
        {
            get
            {
                return this.child2.Identity;
            }

            set
            {
                this.CheckUnlock();
                this.child2 = value.ToStateBase();
            }
        }

        /// <inheritdoc/>
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IMyRootState> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            if (this.child1.Patch(oldState, newState, out var child1Patched))
            {
                patcher = (s) => { s.Child1 = child1Patched.Identity; };
                return true;
            }

            if (this.child2.Patch(oldState, newState, out var child2Patched))
            {
                patcher = (s) => { s.Child2 = child2Patched.Identity; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override void LockChildren()
        {
            base.LockChildren();
            this.child1.Lock();
            this.child2.Lock();
        }

        /// <inheritdoc/>
        protected override AStateBase<IMyRootState> CreateAndClone(bool deep)
        {
            var clone = new MyRootState();

            this.CopyToStatePattern(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToStatePattern(MyRootState state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.value1 = this.value1;

            state.value2 = this.value2;

            if (deep)
            {
                state.child1 = this.child1.DeepClone();
                state.child2 = this.child2.DeepClone();
            }
            else
            {
                state.child1 = this.child1;
                state.child2 = this.child2;
            }
        }
    }
}
