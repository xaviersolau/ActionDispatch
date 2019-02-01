// ----------------------------------------------------------------------
// <copyright file="ExampleAction.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SoloX.ActionDispatch.Core;
using SoloX.ActionDispatch.Core.Impl;
using SoloX.ActionDispatch.Core.Impl.Action;

namespace SoloX.ActionDispatch.Examples
{
    /// <summary>
    /// Example action.
    /// </summary>
    public class ExampleAction : ActionBase<ExampleState, ExampleState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAction"/> class.
        /// </summary>
        public ExampleAction()
            : base(s => s)
        {
        }

        /// <inheritdoc />
        public override ExampleState Apply(IDispatcher<ExampleState> dispatcher, ExampleState state)
        {
            throw new NotImplementedException();
        }
    }
}
