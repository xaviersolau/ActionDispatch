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
    internal class TransactionalState<TRootState, TState> : ITransactionalState<TRootState, TState>
        where TRootState : IState<TRootState>
        where TState : IState
    {
        private readonly AStateBase<TState> stateBase;
        private AStateBase<TState> newState;
        private TRootState rootState;
        private TRootState patchedRootState;

        public TransactionalState(AStateBase<TState> state, TRootState rootState)
        {
            this.stateBase = state;
            this.rootState = rootState;
        }

        public TState GetState()
        {
            this.CheckOpen();

            this.newState = this.stateBase.DeepClone();
            return this.newState.Identity;
        }

        public void SetState(TState state)
        {
            this.CheckOpen();

            if (state.IsLocked)
            {
                throw new AccessViolationException("The new state must not be locked.");
            }

            this.newState = state.ToStateBase();
        }

        public TRootState Close()
        {
            if (this.newState == null)
            {
                return default;
            }

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

        private void CheckOpen()
        {
            if (this.newState != null)
            {
                throw new AccessViolationException("The state transaction is already opened.");
            }
        }
    }
}
