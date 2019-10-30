// ----------------------------------------------------------------------
// <copyright file="DefaultCallingStrategy.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.Dispatch.Impl
{
    internal class DefaultCallingStrategy : ICallingStrategy
    {
        public void Invoke(System.Action action)
        {
            action();
        }
    }
}
