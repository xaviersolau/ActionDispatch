// ----------------------------------------------------------------------
// <copyright file="SynchronizedCallingStrategy.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    internal class SynchronizedCallingStrategy : ICallingStrategy
    {
        private readonly SynchronizationContext synchronizationContext;

        public SynchronizedCallingStrategy(SynchronizationContext synchronizationContext)
        {
            this.synchronizationContext = synchronizationContext;
        }

        public void Invoke(System.Action action)
        {
            this.synchronizationContext.Post((_) => action(), null);
        }
    }
}
