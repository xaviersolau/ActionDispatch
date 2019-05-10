// ----------------------------------------------------------------------
// <copyright file="StateA.cs" company="SoloX Software">
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
    public class StateA : AStateBase<IStateA>, IStateA
    {
        private string value;

        /// <inheritdoc/>
        public override IStateA Identity => this;

        /// <inheritdoc/>
        public string Value
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
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IStateA> patcher)
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
        protected override AStateBase<IStateA> CreateAndClone(bool deep)
        {
            var clone = new StateA();

            this.CopyToStateA(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToStateA(StateA state, bool deep)
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
