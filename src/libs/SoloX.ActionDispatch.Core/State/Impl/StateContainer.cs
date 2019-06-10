// ----------------------------------------------------------------------
// <copyright file="StateContainer.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State.Impl
{
    internal class StateContainer<TState> : IStateContainer<TState>
        where TState : IState
    {
        private readonly AStateBase<TState> stateBase;
        private AStateBase<TState> newState;

        public StateContainer(AStateBase<TState> state)
        {
            this.stateBase = state;
            this.IsEmpty = true;
        }

        public bool IsEmpty { get; private set; }

        public TState State
        {
            get
            {
                return this.newState != default ? this.newState.Identity : default;
            }

            set
            {
                this.CheckOpen();

                if (value.IsLocked)
                {
                    throw new AccessViolationException("The new state must not be locked.");
                }

                this.newState = value.ToStateBase();
                this.IsEmpty = false;
            }
        }

        public void LoadState()
        {
            this.CheckOpen();

            this.newState = this.stateBase.DeepClone();
            this.IsEmpty = false;
        }

        private void CheckOpen()
        {
            if (!this.IsEmpty)
            {
                throw new AccessViolationException("The state container is already set.");
            }
        }
    }
}
