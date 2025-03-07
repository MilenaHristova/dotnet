﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Text.Json.Serialization;

namespace System.Text.Json.SourceGeneration
{
    [DebuggerDisplay("Name={Name}, Type={TypeMetadata}")]
    internal sealed class PropertyGenerationSpec
    {
        /// <summary>
        /// The exact name specified in the source code. This might be different
        /// from the <see cref="ClrName"/> because source code might be decorated
        /// with '@' for reserved keywords, e.g. public string @event { get; set; }
        /// </summary>
        public required string NameSpecifiedInSourceCode { get; init; }

        public required string ClrName { get; init; }

        /// <summary>
        /// Is this a property or a field?
        /// </summary>
        public bool IsProperty { get; init; }

        /// <summary>
        /// If representing a property, returns true if either the getter or setter are public.
        /// </summary>
        public bool IsPublic { get; init; }

        public bool IsVirtual { get; init; }

        /// <summary>
        /// The property name specified via JsonPropertyNameAttribute, if available.
        /// </summary>
        public string? JsonPropertyName { get; init; }

        /// <summary>
        /// The pre-determined JSON property name, accounting for <see cref="JsonNamingPolicy"/>
        /// specified ahead-of-time via <see cref="JsonSourceGenerationOptionsAttribute"/>.
        /// Only used in fast-path serialization logic.
        /// </summary>
        public required string RuntimePropertyName { get; init; }

        public required string PropertyNameVarName { get; init; }

        /// <summary>
        /// Whether the property has a set method.
        /// </summary>
        public bool IsReadOnly { get; init; }

        /// <summary>
        /// Whether the property is marked `required`.
        /// </summary>
        public bool IsRequired { get; init; }

        /// <summary>
        /// The property is marked with JsonRequiredAttribute.
        /// </summary>
        public bool HasJsonRequiredAttribute { get; init; }

        /// <summary>
        /// Whether the property has an init-only set method.
        /// </summary>
        public bool IsInitOnlySetter { get; init; }

        /// <summary>
        /// Whether the property has a public or internal (only usable when JsonIncludeAttribute is specified)
        /// getter that can be referenced in generated source code.
        /// </summary>
        public bool CanUseGetter { get; init; }

        /// <summary>
        /// Whether the property has a public or internal (only usable when JsonIncludeAttribute is specified)
        /// setter that can be referenced in generated source code.
        /// </summary>
        public bool CanUseSetter { get; init; }

        public bool GetterIsVirtual { get; init; }

        public bool SetterIsVirtual { get; init; }

        /// <summary>
        /// The <see cref="JsonIgnoreCondition"/> for the property.
        /// </summary>
        public JsonIgnoreCondition? DefaultIgnoreCondition { get; init; }

        /// <summary>
        /// The <see cref="JsonNumberHandling"/> for the property.
        /// </summary>
        public JsonNumberHandling? NumberHandling { get; init; }

        /// <summary>
        /// The serialization order of the property.
        /// </summary>
        public int Order { get; init; }

        /// <summary>
        /// Whether the property has the JsonIncludeAttribute. If so, non-public accessors can be used for (de)serialziation.
        /// </summary>
        public bool HasJsonInclude { get; init; }

        /// <summary>
        /// Whether the property has the JsonExtensionDataAttribute.
        /// </summary>
        public bool IsExtensionData { get; init; }

        /// <summary>
        /// Generation specification for the property's type.
        /// </summary>
        public required TypeGenerationSpec TypeGenerationSpec { get; init; }

        /// <summary>
        /// Compilable name of the property's declaring type.
        /// </summary>
        public required string DeclaringTypeRef { get; init; }

        public required Type DeclaringType { get; init; }

        /// <summary>
        /// Source code to instantiate design-time specified custom converter.
        /// </summary>
        public string? ConverterInstantiationLogic { get; init; }

        public bool HasFactoryConverter { get; init; }
    }
}
