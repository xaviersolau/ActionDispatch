// ----------------------------------------------------------------------
// <copyright file="ParentStatePattern.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using SoloX.ActionDispatch.Core.State.Impl;
using SoloX.ActionDispatch.State.Generator.Patterns.Itf;
using SoloX.GeneratorTools.Core.CSharp.Generator.Attributes;

namespace SoloX.ActionDispatch.State.Generator.Patterns.Impl
{
    /// <summary>
    /// State implementation pattern.
    /// </summary>
    public class ParentStatePattern : AStateBase<IParentStatePattern>, IParentStatePattern
    {
        private object propertyPattern;
        private AStateBase<IChildStatePattern> childPattern;

        /// <inheritdoc/>
        public override IParentStatePattern Identity => this;

        /// <inheritdoc/>
        public object PropertyPattern
        {
            get
            {
                return this.propertyPattern;
            }

            set
            {
                this.CheckUnlock();
                this.propertyPattern = value;
            }
        }

        /// <inheritdoc/>
        public IChildStatePattern ChildPattern
        {
            get
            {
                return this.childPattern.Identity;
            }

            set
            {
                this.CheckUnlock();
                this.childPattern = value.ToStateBase();
            }
        }

        /// <inheritdoc/>
        [PackStatements]
        protected override bool CheckPatch<TPatchState>(
            AStateBase<TPatchState> oldState,
            AStateBase<TPatchState> newState,
            out Action<IParentStatePattern> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            if (this.childPattern.Patch(oldState, newState, out var childPatternPatched))
            {
                patcher = (s) => { s.ChildPattern = childPatternPatched.Identity; };
                return true;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override void LockChildren()
        {
            base.LockChildren();
            this.childPattern.Lock();
        }

        /// <inheritdoc/>
        protected override AStateBase<IParentStatePattern> CreateAndClone(bool deep)
        {
            var clone = new ParentStatePattern();

            this.CopyToParentStatePattern(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current StatePattern data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToParentStatePattern(ParentStatePattern state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.propertyPattern = this.propertyPattern;

            if (deep)
            {
                state.childPattern = this.childPattern.DeepClone();
            }
            else
            {
                state.childPattern = this.childPattern;
            }
        }
    }
}
