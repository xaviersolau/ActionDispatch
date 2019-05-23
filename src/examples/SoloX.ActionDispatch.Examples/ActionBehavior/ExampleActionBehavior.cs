// ----------------------------------------------------------------------
// <copyright file="ExampleActionBehavior.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Examples.State;

namespace SoloX.ActionDispatch.Examples.ActionBehavior
{
    /// <summary>
    /// Example action.
    /// </summary>
    public class ExampleActionBehavior : IActionBehavior<IExampleAppState, IExampleChildState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleActionBehavior"/> class.
        /// </summary>
        /// <param name="increment">The value to increment.</param>
        public ExampleActionBehavior(int increment)
        {
            this.Increment = increment;
        }

        /// <summary>
        /// Gets inc value.
        /// </summary>
        public int Increment { get; }

        /// <inheritdoc />
        public void Apply(ITransactionalState<IExampleAppState, IExampleChildState> transactionalState)
        {
            var state = transactionalState.GetState();

            state.ChildCount += this.Increment;
        }
    }
}
