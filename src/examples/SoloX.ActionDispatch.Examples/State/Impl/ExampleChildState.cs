// ----------------------------------------------------------------------
// <copyright file="ExampleChildState.cs" company="SoloX Software">
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
    /// Child example state object.
    /// </summary>
    public class ExampleChildState : AStateBase<IExampleChildState>, IExampleChildState
    {
        private int childCount;

        /// <inheritdoc/>
        public override IExampleChildState Identity => this;

        /// <summary>
        /// Gets or sets child state count.
        /// </summary>
        public int ChildCount
        {
            get
            {
                return this.childCount;
            }

            set
            {
                if (this.childCount != value)
                {
                    this.CheckUnlock();
                    this.MakeDirty();
                    this.childCount = value;
                }
            }
        }

        /// <inheritdoc/>
        protected override AStateBase<IExampleChildState> CreateAndClone(bool deep)
        {
            var clone = new ExampleChildState();

            this.CopyToExampleChildState(clone, deep);

            return clone;
        }

        /// <summary>
        /// Copy current ExampleChildState data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
        protected void CopyToExampleChildState(ExampleChildState state, bool deep)
        {
            this.CopyToAStateBase(state, deep);

            state.childCount = this.childCount;
        }
    }
}
