// ----------------------------------------------------------------------
// <copyright file="AStateBase.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SoloX.ActionDispatch.Core.State.Impl
{
    /// <summary>
    /// State base implementation.
    /// </summary>
    /// <typeparam name="TState">The actual type of the state.</typeparam>
    public abstract class AStateBase<TState> : IState
        where TState : IState
    {
        private bool isDirty;

        /// <inheritdoc/>
        public int Version { get; private set; }

        /// <summary>
        /// Gets the actual typed identity state.
        /// </summary>
        public abstract TState Identity { get; }

        /// <summary>
        /// Gets a value indicating whether it is locked or not.
        /// </summary>
        public bool IsLocked
        { get; private set; }

        /// <summary>
        /// Deep clone the state object.
        /// </summary>
        /// <returns>The cloned object.</returns>
        public AStateBase<TState> DeepClone()
        {
            return this.CreateAndClone(true);
        }

        /// <inheritdoc/>
        public void Lock()
        {
            if (!this.IsLocked)
            {
                this.IsLocked = true;

                if (this.LockChildrenAndCheckDirty())
                {
                    this.Version++;
                }

                this.isDirty = false;
            }
        }

        /// <summary>
        /// Path the current state.
        /// </summary>
        /// <remarks>The current state must not be the same instance than the given oldState.</remarks>
        /// <typeparam name="TPatchState">The patch type.</typeparam>
        /// <param name="oldState">The state instance to patch.</param>
        /// <param name="newState">The new state instance.</param>
        /// <param name="patched">The patched state or null.</param>
        /// <returns>True if the current state is actually patched.</returns>
        public bool Patch<TPatchState>(AStateBase<TPatchState> oldState, AStateBase<TPatchState> newState, out AStateBase<TState> patched)
            where TPatchState : IState
        {
            if (ReferenceEquals(this, oldState))
            {
                patched = newState as AStateBase<TState>;
                return true;
            }

            if (this.CheckPatch(oldState, newState, out var patcher))
            {
                var clone = this.CreateAndClone(false);
                patcher(clone.Identity);

                patched = clone;
                return true;
            }

            patched = null;
            return false;
        }

        /// <summary>
        /// Patch the current state.
        /// </summary>
        /// <typeparam name="TPatchState">The patch type.</typeparam>
        /// <param name="oldState">The state instance to patch.</param>
        /// <param name="newState">The new state instance.</param>
        /// <returns>The patched state.</returns>
        internal AStateBase<TState> Patch<TPatchState>(AStateBase<TPatchState> oldState, AStateBase<TPatchState> newState)
            where TPatchState : IState
        {
            if (this.Patch(oldState, newState, out var patched))
            {
                return patched;
            }

            return this;
        }

        /// <summary>
        /// Process all child and check the patch.
        /// </summary>
        /// <typeparam name="TPatchState">The patch type.</typeparam>
        /// <param name="oldState">The state instance to patch.</param>
        /// <param name="newState">The new state instance.</param>
        /// <param name="patcher">The patcher delegate.</param>
        /// <returns>True if the current state is actually patched.</returns>
        protected virtual bool CheckPatch<TPatchState>(AStateBase<TPatchState> oldState, AStateBase<TPatchState> newState, out Action<TState> patcher)
            where TPatchState : IState
        {
            patcher = null;
            return false;
        }

        /// <summary>
        /// Create a cloned instance.
        /// </summary>
        /// <param name="deep">Tells if a deep clone must be done.</param>
        /// <returns>The created clone.</returns>
        protected abstract AStateBase<TState> CreateAndClone(bool deep);

        /// <summary>
        /// Lock children state and check if it is dirty.
        /// </summary>
        /// <returns>True if the state or one of the children state was actually dirty.</returns>
        protected virtual bool LockChildrenAndCheckDirty()
        {
            return this.isDirty;
        }

        /// <summary>
        /// Copy current AStateBase data to the given state target.
        /// </summary>
        /// <param name="state">Target state where to copy current object data.</param>
        /// <param name="deep">Tells if we need to make a deep copy.</param>
#pragma warning disable CA1801 // Parameter deep of method CopyToAStateBase is never used. Remove the parameter or use it in the method body.
        protected void CopyToAStateBase(AStateBase<TState> state, bool deep)
#pragma warning restore CA1801 // Parameter deep of method CopyToAStateBase is never used. Remove the parameter or use it in the method body.
        {
            if (state == null)
            {
                throw new ArgumentNullException($"The argument {nameof(state)} was null.");
            }

            state.Version = this.Version;
        }

        /// <summary>
        /// Check if the state instance is unlocked.
        /// </summary>
        protected void CheckUnlock()
        {
            if (this.IsLocked)
            {
                throw new AccessViolationException("The state is locked.");
            }
        }

        /// <summary>
        /// Make this instance as dirty.
        /// </summary>
        protected void MakeDirty()
        {
            this.isDirty = true;
        }
    }
}
