// ----------------------------------------------------------------------
// <copyright file="IStateBa.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Sample.State.Basic
{
    public interface IStateBa : IState<IStateBa>
    {
        string Value { get; set; }
    }
}
