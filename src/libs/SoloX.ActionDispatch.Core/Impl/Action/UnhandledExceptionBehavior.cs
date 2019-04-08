// ----------------------------------------------------------------------
// <copyright file="UnhandledExceptionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <summary>
    /// Action behavior to report an unhandled exception.
    /// </summary>
    /// <typeparam name="TRootState">The state root type.</typeparam>
    public class UnhandledExceptionBehavior<TRootState> : IActionBehavior<TRootState, TRootState>
        where TRootState : IState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionBehavior{TRootState}"/> class.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        public UnhandledExceptionBehavior(Exception exception)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the unhandled exception.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc/>
        public TRootState Apply(TRootState state)
        {
            // Nothing to do.
            return default;
        }
    }
}
