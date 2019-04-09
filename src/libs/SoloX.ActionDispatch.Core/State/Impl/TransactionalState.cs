// ----------------------------------------------------------------------
// <copyright file="TransactionalState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State.Impl
{
    internal class TransactionalState<TState> : ITransactionalState<TState>
        where TState : IState
    {
        private readonly AStateBase<TState> stateBase;
        private readonly AStateBase<TState> newState;

        public TransactionalState(AStateBase<TState> state)
        {
            this.stateBase = state;
            this.newState = state.DeepClone();
            this.State = this.newState.Identity;
        }

        public TState State { get; }

        public TRootState Patch<TRootState>(TRootState rootState)
            where TRootState : IState<TRootState>
        {
            return rootState.ToStateBase().Patch(this.stateBase, this.newState).Identity;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
        }
    }
}
