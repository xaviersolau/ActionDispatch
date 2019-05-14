// ----------------------------------------------------------------------
// <copyright file="StateBa.cs" company="SoloX Software">
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
    public class StateBa : AStateBase<IStateBa>, IStateBa
    {
        private string value;

        /// <inheritdoc/>
        public override IStateBa Identity => this;

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
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IStateBa> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override bool LockChildrenAndCheckDirty()
        {
            var dirty = base.LockChildrenAndCheckDirty();

            return dirty;
        }

        /// <inheritdoc/>
        protected override AStateBase<IStateBa> CreateAndClone(bool deep)
        {
            var clone = new StateBa();

            this.CopyToStateBa(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToStateBa(StateBa state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.value = this.value;

            if (deep)
            {
            }
            else
            {
            }
        }
    }
}
