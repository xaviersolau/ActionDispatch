// ----------------------------------------------------------------------
// <copyright file="JsonActionSerializer.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Json.State.Impl;

namespace SoloX.ActionDispatch.Json.Action.Impl
{
    /// <summary>
    /// Json action serializer implementation (using Newtonsoft).
    /// </summary>
    public class JsonActionSerializer : IActionSerializer
    {
        private IStateFactory stateFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonActionSerializer"/> class.
        /// </summary>
        /// <param name="stateFactory">The state factory used to create the state instances.</param>
        public JsonActionSerializer(IStateFactory stateFactory)
        {
            this.stateFactory = stateFactory;
        }

        /// <inheritdoc/>
        public IAction<TState> Deserialize<TState>(string action)
            where TState : IState
        {
            // In the case where the action has a State property, we need to give the state converter in
            // addition of the action converter.
            return JsonConvert.DeserializeObject<AAction<TState>>(
                action,
                new JsonActionConverter(),
                new JsonStateConverter(this.stateFactory));
        }

        /// <inheritdoc/>
        public string Serialize<TState>(IAction<TState> action)
            where TState : IState
        {
            // In the case where the action has a State property, we need to give the state converter in
            // addition of the action converter.
            return JsonConvert.SerializeObject(
                action,
                new JsonActionConverter(),
                new JsonStateConverter(this.stateFactory));
        }
    }
}
