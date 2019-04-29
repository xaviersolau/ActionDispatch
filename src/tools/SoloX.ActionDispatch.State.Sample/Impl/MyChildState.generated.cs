// ----------------------------------------------------------------------
// <copyright file="MyChildState.cs" company="SoloX Software">
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
    public class MyChildState : AStateBase<IMyChildState>, IMyChildState
    {
        private string value1;

        /// <inheritdoc/>
        public override IMyChildState Identity => this;

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
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IMyChildState> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override void LockChildren()
        {
            base.LockChildren();
        }

        /// <inheritdoc/>
        protected override AStateBase<IMyChildState> CreateAndClone(bool deep)
        {
            var clone = new MyChildState();

            this.CopyToStatePattern(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToStatePattern(MyChildState state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.value1 = this.value1;

            if (deep)
            {
            }
            else
            {
            }
        }
    }
}
