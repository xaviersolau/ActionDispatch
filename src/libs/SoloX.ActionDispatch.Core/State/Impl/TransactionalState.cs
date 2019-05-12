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
    internal class TransactionalState<TState, TRootState> : ITransactionalState<TState, TRootState>
        where TState : IState
        where TRootState : IState<TRootState>
    {
        private readonly AStateBase<TState> stateBase;
        private readonly AStateBase<TState> newState;
        private TRootState rootState;
        private TRootState patchedRootState;

        public TransactionalState(AStateBase<TState> state, TRootState rootState)
        {
            this.stateBase = state;
            this.newState = state.DeepClone();
            this.State = this.newState.Identity;
            this.rootState = rootState;
        }

        public TState State { get; }

        public TRootState Close()
        {
            if (this.patchedRootState != null)
            {
                throw new AccessViolationException("The root state is already patched.");
            }

            this.patchedRootState = this.rootState.ToStateBase().Patch(this.stateBase, this.newState).Identity;
            return this.patchedRootState;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.patchedRootState != null)
            {
                this.patchedRootState.Lock();
            }
        }
    }
}
