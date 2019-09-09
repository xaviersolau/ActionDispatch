// ----------------------------------------------------------------------
// <copyright file="JsonActionConverter.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.Action;
using SoloX.ActionDispatch.Core.Action.Impl;
using SoloX.ActionDispatch.Core.State;
using SoloX.ExpressionTools.Parser;
using SoloX.ExpressionTools.Parser.Impl;

namespace SoloX.ActionDispatch.Json.Action.Impl
{
    /// <summary>
    /// Json converter that handles the Action Json serialization.
    /// </summary>
    public class JsonActionConverter : JsonConverter
    {
        internal const string IsAsynchronous = "isAsynchronous";
        internal const string BehaviorType = "behaviorType";
        internal const string Behavior = "behavior";
        internal const string RootStateType = "rootStateType";
        internal const string Selector = "selector";

        private static readonly Type GenericAActionType1 = typeof(AAction<>);
        private static readonly Type GenericAActionType2 = typeof(AAction<,>);
        private static readonly Type GenericAsyncActionType = typeof(AsyncAction<,>);
        private static readonly Type GenericSyncActionType = typeof(SyncAction<,>);
        private static readonly Type GenericActionBehaviorAsyncType = typeof(IActionBehaviorAsync<>);
        private static readonly Type GenericActionBehaviorType = typeof(IActionBehavior<>);

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null || !objectType.IsGenericType)
            {
                return false;
            }

            var genericType = objectType.GetGenericTypeDefinition();
            return objectType.IsGenericType
                && (ReferenceEquals(genericType, GenericAsyncActionType)
                || ReferenceEquals(genericType, GenericSyncActionType)
                || ReferenceEquals(genericType, GenericAActionType1)
                || ReferenceEquals(genericType, GenericAActionType2));
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException($"The argument {nameof(writer)} was null.");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException($"The argument {nameof(serializer)} was null.");
            }

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var valueType = value.GetType();

            // Check value type
            if (!valueType.IsGenericType)
            {
                throw new ArgumentException("unsupported value type");
            }

            var genericValueType = valueType.GetGenericTypeDefinition();

            if (!ReferenceEquals(genericValueType, GenericAsyncActionType)
                && !ReferenceEquals(genericValueType, GenericSyncActionType))
            {
                throw new ArgumentException("unsupported value type");
            }

            dynamic action = value;

            writer.WriteStartObject();

            var isAsync = ReferenceEquals(genericValueType, GenericAsyncActionType);

            writer.WritePropertyName(IsAsynchronous);
            writer.WriteValue(isAsync);

            writer.WritePropertyName(BehaviorType);
            var behaviorType = action.Behavior.GetType();
            serializer.Serialize(writer, behaviorType);

            writer.WritePropertyName(Behavior);
            serializer.Serialize(writer, action.Behavior);

            writer.WritePropertyName(RootStateType);
            serializer.Serialize(writer, action.RootStateType);

            writer.WritePropertyName(Selector);
            serializer.Serialize(writer, action.Selector.ToString());

            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var done = false;
            bool? isAsync = null;
            string selector = null;
            Type behaviorType = null;
            dynamic behavior = null;
            Type rootStateType = null;

            if (reader == null)
            {
                throw new ArgumentNullException($"The argument {nameof(reader)} was null.");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException($"The argument {nameof(serializer)} was null.");
            }

            var tknType = reader.TokenType;
            while (!done)
            {
                switch (tknType)
                {
                    case JsonToken.StartObject:
                        tknType = ReadNextToken(reader);
                        break;
                    case JsonToken.PropertyName:
                        var name = (string)reader.Value;
                        switch (name)
                        {
                            case IsAsynchronous:
                                isAsync = ReadAsBoolean(reader);
                                break;
                            case RootStateType:
                                rootStateType = Type.GetType(ReadAsString(reader));
                                break;
                            case Selector:
                                selector = ReadAsString(reader);
                                break;
                            case BehaviorType:
                                behaviorType = Type.GetType(ReadAsString(reader));
                                break;
                            case Behavior:
                                if (behaviorType == null)
                                {
                                    throw new JsonException("Expecting behaviorType property before behavior.");
                                }

                                tknType = ReadNextToken(reader);
                                behavior = serializer.Deserialize(reader, behaviorType);
                                break;
                        }

                        tknType = ReadNextToken(reader);
                        break;
                    case JsonToken.EndObject:
                        done = true;
                        break;
                }
            }

            if (behavior == null || behaviorType == null || !isAsync.HasValue || selector == null || rootStateType == null)
            {
                throw new JsonException("Missing json properties to create the action.");
            }

            return CreateAction(behaviorType, behavior, isAsync.Value, rootStateType, selector);
        }

        private static object CreateAction(Type behaviorType, dynamic behavior, bool isAsync, Type rootStateType, string selector)
        {
            Type genericActionType;
            Type genericBehaviorType;
            if (isAsync)
            {
                genericBehaviorType = GenericActionBehaviorAsyncType;
                genericActionType = GenericAsyncActionType;
            }
            else
            {
                genericBehaviorType = GenericActionBehaviorType;
                genericActionType = GenericSyncActionType;
            }

            var behaviorInterface = behaviorType.GetInterfaces()
                .First(i => i.IsGenericType && ReferenceEquals(i.GetGenericTypeDefinition(), genericBehaviorType));

            var genArgs = behaviorInterface.GetGenericArguments();
            var stateType = genArgs[0];

            var actionType = genericActionType.MakeGenericType(rootStateType, stateType);

            var expressionParser = new ExpressionParser(parameterTypeResolver: new ParameterTypeResolver(rootStateType));
            var selectorExpression = expressionParser.Parse(selector);

            return Activator.CreateInstance(actionType, behavior, selectorExpression);
        }

        private static string ReadAsString(JsonReader reader)
        {
            var val = reader.ReadAsString();
            if (val == null)
            {
                throw new JsonException($"Unable to read a string for {reader.Path}");
            }

            return val;
        }

        private static bool ReadAsBoolean(JsonReader reader)
        {
            var val = reader.ReadAsBoolean();
            if (!val.HasValue)
            {
                throw new JsonException($"Unable to read a boolean for {reader.Path}");
            }

            return val.Value;
        }

        private static JsonToken ReadNextToken(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException();
            }

            return reader.TokenType;
        }

        private class ParameterTypeResolver : IParameterTypeResolver
        {
            private Type appStateType;
            private string resolvedParameterName;

            public ParameterTypeResolver(Type appStateType)
            {
                this.appStateType = appStateType;
            }

            public Type ResolveType(string parameterName)
            {
                if (this.resolvedParameterName != null && this.resolvedParameterName != parameterName)
                {
                    throw new FormatException("Only one parameter is expected");
                }

                this.resolvedParameterName = parameterName;
                return this.appStateType;
            }
        }
    }
}
