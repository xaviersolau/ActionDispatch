// ----------------------------------------------------------------------
// <copyright file="JsonStateSerializer.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.State;

namespace SoloX.ActionDispatch.Json.State.Impl
{
    /// <summary>
    /// Json state serializer implementation (using Newtonsoft).
    /// </summary>
    public class JsonStateSerializer : IStateSerializer
    {
        private IStateFactory stateFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStateSerializer"/> class.
        /// </summary>
        /// <param name="stateFactory">The state factory used to create the state instances.</param>
        public JsonStateSerializer(IStateFactory stateFactory)
        {
            this.stateFactory = stateFactory;
        }

        /// <inheritdoc/>
        public TState Deserialize<TState>(string state)
            where TState : IState
        {
            return JsonConvert.DeserializeObject<TState>(state, new JsonStateConverter(this.stateFactory));
        }

        /// <inheritdoc/>
        public string Serialize(IState state)
        {
            return JsonConvert.SerializeObject(state, new JsonStateConverter(this.stateFactory));
        }
    }
}
