﻿// ----------------------------------------------------------------------
// <copyright file="StateCollection.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State.Impl
{
    /// <summary>
    /// State collection that allows state behavior.
    /// </summary>
    /// <typeparam name="TStateItem">Type of the collection state items.</typeparam>
    public class StateCollection<TStateItem> : AStateBase<IStateCollection<TStateItem>>, IStateCollection<TStateItem>
        where TStateItem : IState
    {
        private List<TStateItem> items = new List<TStateItem>();

        /// <inheritdoc/>
        public int Count => this.items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => this.IsLocked;

        /// <inheritdoc/>
        public override IStateCollection<TStateItem> Identity => this;

        /// <inheritdoc/>
        public TStateItem this[int index]
        {
            get
            {
                return this.items[index];
            }

            set
            {
                this.CheckUnlock();
                this.MakeDirty();
                this.items[index] = value;
            }
        }

        /// <inheritdoc/>
        public void Add(TStateItem item)
        {
            this.CheckUnlock();
            this.MakeDirty();
            this.items.Add(item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.CheckUnlock();
            this.MakeDirty();
            this.items.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(TStateItem item)
        {
            return this.items.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(TStateItem[] array, int arrayIndex)
        {
            this.items.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public IEnumerator<TStateItem> GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        /// <inheritdoc/>
        public bool Remove(TStateItem item)
        {
            this.CheckUnlock();
            this.MakeDirty();
            return this.items.Remove(item);
        }

        /// <inheritdoc/>
        public int IndexOf(TStateItem item)
        {
            return this.items.IndexOf(item);
        }

        /// <inheritdoc/>
        public void Insert(int index, TStateItem item)
        {
            this.CheckUnlock();
            this.MakeDirty();
            this.items.Insert(index, item);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            this.CheckUnlock();
            this.MakeDirty();
            this.items.RemoveAt(index);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        protected override bool CheckPatch<TPatchState>(AStateBase<TPatchState> oldState, AStateBase<TPatchState> newState, out Action<IStateCollection<TStateItem>> patcher)
        {
            if (base.CheckPatch(oldState, newState, out patcher))
            {
                return true;
            }

            int i = 0;
            foreach (var item in this.items)
            {
                if (item != null && item.ToStateBase().Patch(oldState, newState, out var itemPatched))
                {
                    patcher = (s) => { s[i] = itemPatched.Identity; };
                    return true;
                }

                i++;
            }

            patcher = null;
            return false;
        }

        /// <inheritdoc/>
        protected override AStateBase<IStateCollection<TStateItem>> CreateAndClone(bool deep)
        {
            var clone = new StateCollection<TStateItem>();

            if (deep)
            {
                foreach (var item in this.items)
                {
                    clone.Add(item.ToStateBase().DeepClone().Identity);
                }
            }
            else
            {
                foreach (var item in this.items)
                {
                    clone.Add(item);
                }
            }

            return clone;
        }

        /// <inheritdoc/>
        protected override bool LockChildrenAndCheckDirty()
        {
            var dirty = base.LockChildrenAndCheckDirty();

            foreach (var item in this.items)
            {
                var oldVersion = item.Version;
                item.Lock();
                dirty |= oldVersion != item.Version;
            }

            return dirty;
        }
    }
}
