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

namespace SoloX.ActionDispatch.Core.UTest.State.Basic
{
    public class StateA : AStateBase<IStateA>, IStateA
    {
        public override IStateA Identity => this;

        protected override AStateBase<IStateA> CreateAndClone(bool deep)
        {
            var clone = new StateA();

            this.CopyToStateA(clone, deep);

            return clone;
        }

        protected void CopyToStateA(StateA state, bool deep)
        {
            this.CopyToAStateBase(state, deep);
        }
    }
}
