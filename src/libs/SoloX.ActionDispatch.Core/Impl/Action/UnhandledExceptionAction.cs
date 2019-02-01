// ----------------------------------------------------------------------
// <copyright file="UnhandledExceptionAction.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SoloX.ActionDispatch.Core.Impl.Action
{
    /// <summary>
    /// Action to dispatch to report an unhandled exception.
    /// </summary>
    /// <typeparam name="TRootState">The state root type.</typeparam>
    public class UnhandledExceptionAction<TRootState> : ActionBase<TRootState, TRootState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnhandledExceptionAction{TRootState}"/> class.
        /// </summary>
        /// <param name="exception">The thrown exception.</param>
        public UnhandledExceptionAction(Exception exception)
            : base(s => s)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the unhandled exception.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc />
        public override TRootState Apply(IDispatcher<TRootState> dispatcher, TRootState state)
        {
            // nothing to do...
            return state;
        }
    }
}
