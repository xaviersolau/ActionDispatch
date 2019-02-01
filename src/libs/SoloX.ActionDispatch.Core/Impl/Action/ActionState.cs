// ----------------------------------------------------------------------
// <copyright file="ActionState.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <summary>
    /// Action state.
    /// </summary>
    public enum ActionState
    {
        /// <summary>
        /// No state.
        /// </summary>
        None,

        /// <summary>
        /// Action applied with success.
        /// </summary>
        Success,

        /// <summary>
        /// Action apply failed.
        /// </summary>
        Failed,
    }
}
