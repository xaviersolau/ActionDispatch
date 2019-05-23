// ----------------------------------------------------------------------
// <copyright file="UnhandledExceptionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Core.Action.Impl
{
    /// <summary>
    /// Action behavior to report an unhandled exception.
    /// </summary>
    /// <typeparam name="TRootState">The state root type.</typeparam>
    public class UnhandledExceptionBehavior<TRootState> : IUnhandledExceptionBehavior<TRootState>
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
        public void Apply(ITransactionalState<TRootState, TRootState> transactionalState)
        {
            // Nothing to do.
        }
    }
}
