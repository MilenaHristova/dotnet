﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeStyle;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Options;

#if !CODE_STYLE
using Microsoft.CodeAnalysis.Host;
#endif

namespace Microsoft.CodeAnalysis.Formatting;

internal abstract class SyntaxFormattingOptions
{
    [DataContract]
    internal sealed record class CommonOptions
    {
        public static readonly CommonOptions Default = new();

        [DataMember] public LineFormattingOptions LineFormatting { get; init; } = LineFormattingOptions.Default;
        [DataMember] public bool SeparateImportDirectiveGroups { get; init; } = false;
        [DataMember] public AccessibilityModifiersRequired AccessibilityModifiersRequired { get; init; } = AccessibilityModifiersRequired.ForNonInterfaceMembers;
    }

    [DataMember]
    public CommonOptions Common { get; init; } = CommonOptions.Default;

    public abstract SyntaxFormattingOptions With(LineFormattingOptions lineFormatting);

    public bool UseTabs => Common.LineFormatting.UseTabs;
    public int TabSize => Common.LineFormatting.TabSize;
    public int IndentationSize => Common.LineFormatting.IndentationSize;
    public LineFormattingOptions LineFormatting => Common.LineFormatting;
    public string NewLine => Common.LineFormatting.NewLine;
    public bool SeparateImportDirectiveGroups => Common.SeparateImportDirectiveGroups;
    public AccessibilityModifiersRequired AccessibilityModifiersRequired => Common.AccessibilityModifiersRequired;

#if !CODE_STYLE
    public static SyntaxFormattingOptions GetDefault(LanguageServices languageServices)
        => languageServices.GetRequiredService<ISyntaxFormattingService>().DefaultOptions;
#endif
}

internal interface SyntaxFormattingOptionsProvider :
#if !CODE_STYLE
    OptionsProvider<SyntaxFormattingOptions>,
#endif
    LineFormattingOptionsProvider
{
}

internal static partial class SyntaxFormattingOptionsProviders
{
    public static SyntaxFormattingOptions.CommonOptions GetCommonSyntaxFormattingOptions(this IOptionsReader options, string language, SyntaxFormattingOptions.CommonOptions? fallbackOptions)
    {
        fallbackOptions ??= SyntaxFormattingOptions.CommonOptions.Default;

        return new()
        {
            LineFormatting = options.GetLineFormattingOptions(language, fallbackOptions.LineFormatting),
            SeparateImportDirectiveGroups = options.GetOption(GenerationOptions.SeparateImportDirectiveGroups, language, fallbackOptions.SeparateImportDirectiveGroups),
            AccessibilityModifiersRequired = options.GetOptionValue(CodeStyleOptions2.AccessibilityModifiersRequired, language, fallbackOptions.AccessibilityModifiersRequired),
        };
    }

#if !CODE_STYLE
    public static SyntaxFormattingOptions GetSyntaxFormattingOptions(this IOptionsReader options, SyntaxFormattingOptions? fallbackOptions, LanguageServices languageServices)
        => languageServices.GetRequiredService<ISyntaxFormattingService>().GetFormattingOptions(options, fallbackOptions);

    public static async ValueTask<SyntaxFormattingOptions> GetSyntaxFormattingOptionsAsync(this Document document, SyntaxFormattingOptions? fallbackOptions, CancellationToken cancellationToken)
    {
        var configOptions = await document.GetAnalyzerConfigOptionsAsync(cancellationToken).ConfigureAwait(false);
        return configOptions.GetSyntaxFormattingOptions(fallbackOptions, document.Project.Services);
    }

    public static async ValueTask<SyntaxFormattingOptions> GetSyntaxFormattingOptionsAsync(this Document document, SyntaxFormattingOptionsProvider fallbackOptionsProvider, CancellationToken cancellationToken)
        => await GetSyntaxFormattingOptionsAsync(document, await ((OptionsProvider<SyntaxFormattingOptions>)fallbackOptionsProvider).GetOptionsAsync(document.Project.Services, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false);
#endif
}
