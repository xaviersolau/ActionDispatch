// ----------------------------------------------------------------------
// <copyright file="JsonStateConverter.cs" company="SoloX Software">
// Copyright (c) SoloX Software. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using SoloX.ActionDispatch.Core.State;
using SoloX.ActionDispatch.Core.State.Impl;

namespace SoloX.ActionDispatch.Json.State.Impl
{
    /// <summary>
    /// Json converter that handles the State Json serialization.
    /// </summary>
    public class JsonStateConverter : JsonConverter
    {
        private const string IdentityPropertyName = nameof(AStateBase<IState>.Identity);
        private const string IsLockedPropertyName = nameof(AStateBase<IState>.IsLocked);

        private static readonly Type GenericStateType = typeof(AStateBase<>);

        private IStateFactory stateFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonStateConverter"/> class.
        /// </summary>
        /// <param name="stateFactory">The state factory to use in the deserialization process.</param>
        public JsonStateConverter(IStateFactory stateFactory)
        {
            this.stateFactory = stateFactory;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null)
            {
                return false;
            }

            if (objectType.IsInterface)
            {
                return typeof(IState).IsAssignableFrom(objectType);
            }
            else
            {
                Type baseType = objectType;
                while (baseType.BaseType != typeof(object) && baseType.BaseType != null)
                {
                    baseType = baseType.BaseType;
                }

                if (!baseType.IsGenericType)
                {
                    return false;
                }

                var genericType = baseType.GetGenericTypeDefinition();
                return baseType.IsGenericType
                    && ReferenceEquals(genericType, GenericStateType);
            }
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException($"The argument {nameof(reader)} was null.");
            }

            if (serializer == null)
            {
                throw new ArgumentNullException($"The argument {nameof(serializer)} was null.");
            }

            if (objectType == null)
            {
                throw new ArgumentNullException($"The argument {nameof(objectType)} was null.");
            }

            var state = this.stateFactory.Create(objectType);

            var stateType = state.GetType();

            var done = false;
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

                        var property = stateType
                            .GetProperty(name, BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                        tknType = ReadNextToken(reader);
                        var propertyType = property.PropertyType;

                        if (typeof(IEnumerable<IState>).IsAssignableFrom(propertyType)
                            && (propertyType.GetGenericTypeDefinition() == typeof(ICollection<>)
                                || propertyType.GetGenericTypeDefinition() == typeof(IList<>)
                                || propertyType.GetGenericTypeDefinition() == typeof(IStateCollection<>)))
                        {
                            var itemType = propertyType.GetGenericArguments().First();

                            dynamic items = property.GetValue(state);

                            if (tknType != JsonToken.StartArray)
                            {
                                throw new FormatException("Expecting StartArray");
                            }

                            tknType = ReadNextToken(reader);

                            while (tknType != JsonToken.EndArray)
                            {
                                dynamic item = serializer.Deserialize(reader, itemType);
                                items.Add(item);

                                tknType = ReadNextToken(reader);
                            }
                        }
                        else
                        {
                            var value = serializer.Deserialize(reader, property.PropertyType);

                            if (property.GetSetMethod() != null)
                            {
                                property.SetValue(state, value);
                            }
                            else
                            {
                                var fieldName = $"<{property.Name}>k__BackingField";
                                var field = property.DeclaringType
                                    .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                                field.SetValue(state, value);
                            }
                        }

                        tknType = ReadNextToken(reader);
                        break;
                    case JsonToken.EndObject:
                        done = true;
                        break;
                }
            }

            return state;
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

            writer.WriteStartObject();
            var properties = value.GetType().GetProperties();

            foreach (var property in properties.Where(p => p.Name != IdentityPropertyName && p.Name != IsLockedPropertyName))
            {
                writer.WritePropertyName(property.Name);
                if (typeof(IEnumerable<IState>).IsAssignableFrom(property.PropertyType))
                {
                    writer.WriteStartArray();
                    var items = (IEnumerable<IState>)property.GetGetMethod().Invoke(value, null);
                    foreach (var item in items)
                    {
                        serializer.Serialize(writer, item);
                    }

                    writer.WriteEndArray();
                }
                else
                {
                    serializer.Serialize(writer, property.GetGetMethod().Invoke(value, null));
                }
            }

            writer.WriteEndObject();
        }

        private static JsonToken ReadNextToken(JsonReader reader)
        {
            if (!reader.Read())
            {
                throw new JsonException();
            }

            return reader.TokenType;
        }
    }
}
