// ----------------------------------------------------------------------
// <copyright file="IStateCollection.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace SoloX.ActionDispatch.Core.State
{
    /// <summary>
    /// IState collection interface.
    /// </summary>
    /// <typeparam name="TStateItem">Type of the collection state items.</typeparam>
    public interface IStateCollection<TStateItem> : IState, IList<TStateItem>
        where TStateItem : IState
    {
    }
}
