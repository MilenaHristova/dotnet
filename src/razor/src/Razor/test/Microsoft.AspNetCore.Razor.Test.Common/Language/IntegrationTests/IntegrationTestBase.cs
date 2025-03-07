﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Razor.Language.Legacy;
using Microsoft.AspNetCore.Razor.Language.CodeGeneration;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.AspNetCore.Razor.Language.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Razor;
using Xunit;
using Xunit.Sdk;

namespace Microsoft.AspNetCore.Razor.Language.IntegrationTests;

// Sets the FileName static variable.
// Finds the test method name using reflection, and uses
// that to find the expected input/output test files in the file system.
[IntializeTestFile]

// These tests must be run serially due to the test specific FileName static var.
[Collection("IntegrationTestSerialRuns")]
public abstract class IntegrationTestBase
{
    private static readonly AsyncLocal<string> s_fileName = new();

    private static readonly CSharpCompilation s_defaultBaseCompilation;

    static IntegrationTestBase()
    {
        var referenceAssemblyRoots = new[]
        {
            typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly, // System.Runtime
        };

        var referenceAssemblies = referenceAssemblyRoots
            .SelectMany(assembly => assembly.GetReferencedAssemblies().Concat(new[] { assembly.GetName() }))
            .Distinct()
            .Select(Assembly.Load)
            .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
            .ToList();
        s_defaultBaseCompilation = CSharpCompilation.Create(
            "TestAssembly",
            Array.Empty<SyntaxTree>(),
            referenceAssemblies,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    protected IntegrationTestBase(bool? generateBaselines = null)
    {
        TestProjectRoot = TestProject.GetProjectDirectory(GetType());

        if (generateBaselines.HasValue)
        {
            GenerateBaselines = generateBaselines.Value;
        }
    }

    /// <summary>
    /// Gets the <see cref="CSharpCompilation"/> that will be used as the 'app' compilation.
    /// </summary>
    protected virtual CSharpCompilation BaseCompilation => s_defaultBaseCompilation;

    /// <summary>
    /// Gets the parse options applied when using <see cref="AddCSharpSyntaxTree(string, string)"/>.
    /// </summary>
    protected virtual CSharpParseOptions CSharpParseOptions { get; } = new CSharpParseOptions(LanguageVersion.Preview);

    /// <summary>
    /// Gets the compilation options applied when compiling assemblies.
    /// </summary>
    protected virtual CSharpCompilationOptions CSharpCompilationOptions { get; } = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

    /// <summary>
    /// Gets a list of CSharp syntax trees used that are considered part of the 'app'.
    /// </summary>
    protected virtual List<CSharpSyntaxTree> CSharpSyntaxTrees { get; } = new List<CSharpSyntaxTree>();

    /// <summary>
    /// Gets the <see cref="RazorConfiguration"/> that will be used for code generation.
    /// </summary>
    protected virtual RazorConfiguration Configuration { get; } = RazorConfiguration.Default;

    protected virtual bool DesignTime { get; } = false;

    /// <summary>
    /// Gets the
    /// </summary>
    internal VirtualRazorProjectFileSystem FileSystem { get; } = new VirtualRazorProjectFileSystem();

    /// <summary>
    /// Used to force a specific style of line-endings for testing. This matters for the baseline tests that exercise line mappings.
    /// Even though we normalize newlines for testing, the difference between platforms affects the data through the *count* of
    /// characters written.
    /// </summary>
    protected virtual string LineEnding { get; } = "\r\n";

#if GENERATE_BASELINES
    protected bool GenerateBaselines { get; } = true;
#else
    protected bool GenerateBaselines { get; } = false;
#endif

    protected string TestProjectRoot { get; }

    // Used by the test framework to set the 'base' name for test files.
    public static string FileName
    {
        get { return s_fileName.Value; }
        set { s_fileName.Value = value; }
    }

    public string FileExtension { get; set; } = ".cshtml";

    protected virtual void ConfigureProjectEngine(RazorProjectEngineBuilder builder)
    {
    }

    protected CSharpSyntaxTree AddCSharpSyntaxTree(string text, string filePath = null)
    {
        var syntaxTree = (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(text, CSharpParseOptions, path: filePath);
        CSharpSyntaxTrees.Add(syntaxTree);
        return syntaxTree;
    }

    protected RazorProjectItem AddProjectItemFromText(string text, string filePath = "_ViewImports.cshtml")
    {
        var projectItem = CreateProjectItemFromText(text, filePath);
        FileSystem.Add(projectItem);
        return projectItem;
    }

    private RazorProjectItem CreateProjectItemFromText(string text, string filePath)
    {
        // Consider the file path to be relative to the 'FileName' of the test.
        var workingDirectory = Path.GetDirectoryName(FileName);

        // Since these paths are used in baselines, we normalize them to windows style. We
        // use "" as the base path by convention to avoid baking in an actual file system
        // path.
        var basePath = "";
        var physicalPath = Path.Combine(workingDirectory, filePath).Replace('/', '\\');
        var relativePhysicalPath = physicalPath;

        // FilePaths in Razor are **always** are of the form '/a/b/c.cshtml'
        filePath = physicalPath.Replace('\\', '/');
        if (!filePath.StartsWith("/", StringComparison.OrdinalIgnoreCase))
        {
            filePath = '/' + filePath;
        }

        text = NormalizeNewLines(text);

        var projectItem = new TestRazorProjectItem(
            basePath: basePath,
            filePath: filePath,
            physicalPath: physicalPath,
            relativePhysicalPath: relativePhysicalPath)
        {
            Content = text,
        };

        return projectItem;
    }

    protected RazorProjectItem CreateProjectItemFromFile(string filePath = null, string fileKind = null)
    {
        if (FileName is null)
        {
            var message = $"{nameof(CreateProjectItemFromFile)} should only be called from an integration test, ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var suffixIndex = FileName.LastIndexOf("_", StringComparison.Ordinal);
        var normalizedFileName = suffixIndex == -1 ? FileName : FileName[..suffixIndex];
        var sourceFileName = Path.ChangeExtension(normalizedFileName, FileExtension);
        var testFile = TestFile.Create(sourceFileName, GetType().GetTypeInfo().Assembly);
        if (!testFile.Exists())
        {
            throw new XunitException($"The resource {sourceFileName} was not found.");
        }

        var fileContent = testFile.ReadAllText();
        var normalizedContent = NormalizeNewLines(fileContent);

        var workingDirectory = Path.GetDirectoryName(FileName);
        var fullPath = sourceFileName;

        // Normalize to forward-slash - these strings end up in the baselines.
        fullPath = fullPath.Replace('\\', '/');
        sourceFileName = sourceFileName.Replace('\\', '/');

        // FilePaths in Razor are **always** are of the form '/a/b/c.cshtml'
        filePath ??= sourceFileName;
        if (!filePath.StartsWith("/", StringComparison.Ordinal))
        {
            filePath = '/' + filePath;
        }

        var projectItem = new TestRazorProjectItem(
            basePath: workingDirectory,
            filePath: filePath,
            physicalPath: fullPath,
            relativePhysicalPath: sourceFileName,
            fileKind: fileKind)
        {
            Content = fileContent,
        };

        return projectItem;
    }

    protected CompiledCSharpCode CompileToCSharp(string text, string path = "test.cshtml", bool? designTime = null)
    {
        var projectItem = CreateProjectItemFromText(text, path);
        return CompileToCSharp(projectItem, designTime);
    }

    protected CompiledCSharpCode CompileToCSharp(RazorProjectItem projectItem, bool? designTime = null)
    {
        var compilation = CreateCompilation();
        var references = compilation.References.Concat(new[] { compilation.ToMetadataReference(), }).ToArray();

        var projectEngine = CreateProjectEngine(Configuration, references, ConfigureProjectEngine);
        var codeDocument = (designTime ?? DesignTime) ? projectEngine.ProcessDesignTime(projectItem) : projectEngine.Process(projectItem);

        return new CompiledCSharpCode(CSharpCompilation.Create(compilation.AssemblyName + ".Views", references: references, options: CSharpCompilationOptions), codeDocument);
    }

    protected CompiledAssembly CompileToAssembly(string text, string path = "test.cshtml", bool? designTime = null, bool throwOnFailure = true)
    {
        var compiled = CompileToCSharp(text, path, designTime);
        return CompileToAssembly(compiled, throwOnFailure);
    }

    protected CompiledAssembly CompileToAssembly(RazorProjectItem projectItem, bool? designTime = null, bool throwOnFailure = true)
    {
        var compiled = CompileToCSharp(projectItem, designTime);
        return CompileToAssembly(compiled, throwOnFailure: throwOnFailure);
    }

    protected CompiledAssembly CompileToAssembly(CompiledCSharpCode code, bool throwOnFailure = true)
    {
        var cSharpDocument = code.CodeDocument.GetCSharpDocument();
        if (cSharpDocument.Diagnostics.Any())
        {
            var diagnosticsLog = string.Join(Environment.NewLine, cSharpDocument.Diagnostics.Select(d => d.ToString()).ToArray());
            throw new InvalidOperationException($"Aborting compilation to assembly because RazorCompiler returned nonempty diagnostics: {diagnosticsLog}");
        }

        var syntaxTrees = new[]
        {
            (CSharpSyntaxTree)CSharpSyntaxTree.ParseText(cSharpDocument.GeneratedCode, CSharpParseOptions, path: code.CodeDocument.Source.FilePath),
        };

        var compilation = code.BaseCompilation.AddSyntaxTrees(syntaxTrees);

        var diagnostics = compilation
            .GetDiagnostics()
            .Where(d => d.Severity >= DiagnosticSeverity.Warning)
            .ToArray();

        if (diagnostics.Length > 0 && throwOnFailure)
        {
            throw new CompilationFailedException(compilation, diagnostics);
        }
        else if (diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error))
        {
            return new CompiledAssembly(compilation, code.CodeDocument, assembly: null);
        }

        using (var peStream = new MemoryStream())
        {
            var emit = compilation.Emit(peStream);
            diagnostics = emit
                .Diagnostics
                .Where(d => d.Severity >= DiagnosticSeverity.Warning)
                .ToArray();
            if (diagnostics.Length > 0 && throwOnFailure)
            {
                throw new CompilationFailedException(compilation, diagnostics);
            }

            return new CompiledAssembly(compilation, code.CodeDocument, Assembly.Load(peStream.ToArray()));
        }
    }

    private CSharpCompilation CreateCompilation()
    {
        var compilation = BaseCompilation.AddSyntaxTrees(CSharpSyntaxTrees);
        var diagnostics = compilation.GetDiagnostics().Where(d => d.Severity >= DiagnosticSeverity.Warning).ToArray();
        if (diagnostics.Length > 0)
        {
            throw new CompilationFailedException(compilation, diagnostics);
        }

        return compilation;
    }

    protected RazorProjectEngine CreateProjectEngine(Action<RazorProjectEngineBuilder> configure = null)
    {
        var compilation = CreateCompilation();
        var references = compilation.References.Concat(new[] { compilation.ToMetadataReference(), }).ToArray();
        return CreateProjectEngine(Configuration, references, configure);
    }

    private RazorProjectEngine CreateProjectEngine(RazorConfiguration configuration, MetadataReference[] references, Action<RazorProjectEngineBuilder> configure)
    {
        return RazorProjectEngine.Create(configuration, FileSystem, b =>
        {
            b.Phases.Insert(0, new ConfigureCodeRenderingPhase(LineEnding));

            configure?.Invoke(b);

            // Allow the test to do custom things with tag helpers, but do the default thing most of the time.
            if (!b.Features.OfType<ITagHelperFeature>().Any())
            {
                b.Features.Add(new CompilationTagHelperFeature());
                b.Features.Add(new DefaultMetadataReferenceFeature()
                {
                    References = references,
                });
            }

            b.Features.Add(new DefaultTypeNameFeature());
            b.SetCSharpLanguageVersion(CSharpParseOptions.LanguageVersion);

            // Decorate each import feature so we can normalize line endings.
            foreach (var feature in b.Features.OfType<IImportProjectFeature>().ToArray())
            {
                b.Features.Remove(feature);
                b.Features.Add(new NormalizedDefaultImportFeature(feature, LineEnding));
            }
        });
    }

    protected void AssertDocumentNodeMatchesBaseline(DocumentIntermediateNode document)
    {
        if (FileName is null)
        {
            var message = $"{nameof(AssertDocumentNodeMatchesBaseline)} should only be called from an integration test ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var baselineFileName = Path.ChangeExtension(FileName, ".ir.txt");

        if (GenerateBaselines)
        {
            var baselineFullPath = Path.Combine(TestProjectRoot, baselineFileName);
            File.WriteAllText(baselineFullPath, IntermediateNodeSerializer.Serialize(document));
            return;
        }

        var irFile = TestFile.Create(baselineFileName, GetType().GetTypeInfo().Assembly);
        if (!irFile.Exists())
        {
            throw new XunitException($"The resource {baselineFileName} was not found.");
        }

        var baseline = irFile.ReadAllText().Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        IntermediateNodeVerifier.Verify(document, baseline);
    }

    internal void AssertHtmlDocumentMatchesBaseline(RazorHtmlDocument htmlDocument)
    {
        if (FileName is null)
        {
            var message = $"{nameof(AssertHtmlDocumentMatchesBaseline)} should only be called from an integration test ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var baselineFileName = Path.ChangeExtension(FileName, ".codegen.html");

        if (GenerateBaselines)
        {
            var baselineFullPath = Path.Combine(TestProjectRoot, baselineFileName);
            File.WriteAllText(baselineFullPath, htmlDocument.GeneratedCode);
            return;
        }

        var htmlFile = TestFile.Create(baselineFileName, GetType().GetTypeInfo().Assembly);
        if (!htmlFile.Exists())
        {
            throw new XunitException($"The resource {baselineFileName} was not found.");
        }

        var baseline = htmlFile.ReadAllText();

        // Normalize newlines to match those in the baseline.
        var actual = htmlDocument.GeneratedCode.Replace("\r", "").Replace("\n", "\r\n");
        Assert.Equal(baseline, actual);
    }

    protected void AssertCSharpDocumentMatchesBaseline(RazorCSharpDocument cSharpDocument)
    {
        if (FileName is null)
        {
            var message = $"{nameof(AssertCSharpDocumentMatchesBaseline)} should only be called from an integration test ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var baselineFileName = Path.ChangeExtension(FileName, ".codegen.cs");
        var baselineDiagnosticsFileName = Path.ChangeExtension(FileName, ".diagnostics.txt");

        if (GenerateBaselines)
        {
            var baselineFullPath = Path.Combine(TestProjectRoot, baselineFileName);
            File.WriteAllText(baselineFullPath, cSharpDocument.GeneratedCode);

            var baselineDiagnosticsFullPath = Path.Combine(TestProjectRoot, baselineDiagnosticsFileName);
            var lines = cSharpDocument.Diagnostics.Select(RazorDiagnosticSerializer.Serialize).ToArray();
            if (lines.Any())
            {
                File.WriteAllLines(baselineDiagnosticsFullPath, lines);
            }
            else if (File.Exists(baselineDiagnosticsFullPath))
            {
                File.Delete(baselineDiagnosticsFullPath);
            }

            return;
        }

        var codegenFile = TestFile.Create(baselineFileName, GetType().GetTypeInfo().Assembly);
        if (!codegenFile.Exists())
        {
            throw new XunitException($"The resource {baselineFileName} was not found.");
        }

        var baseline = codegenFile.ReadAllText();

        // Normalize newlines to match those in the baseline.
        var actual = cSharpDocument.GeneratedCode.Replace("\r", "").Replace("\n", "\r\n");
        Assert.Equal(baseline, actual);

        var baselineDiagnostics = string.Empty;
        var diagnosticsFile = TestFile.Create(baselineDiagnosticsFileName, GetType().GetTypeInfo().Assembly);
        if (diagnosticsFile.Exists())
        {
            baselineDiagnostics = diagnosticsFile.ReadAllText();
        }

        var actualDiagnostics = string.Concat(cSharpDocument.Diagnostics.Select(d => NormalizeNewLines(RazorDiagnosticSerializer.Serialize(d)) + "\r\n"));
        Assert.Equal(baselineDiagnostics, actualDiagnostics);
    }

    protected void AssertSourceMappingsMatchBaseline(RazorCodeDocument codeDocument)
    {
        if (FileName is null)
        {
            var message = $"{nameof(AssertSourceMappingsMatchBaseline)} should only be called from an integration test ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var csharpDocument = codeDocument.GetCSharpDocument();
        Assert.NotNull(csharpDocument);

        var baselineFileName = Path.ChangeExtension(FileName, ".mappings.txt");
        var serializedMappings = SourceMappingsSerializer.Serialize(csharpDocument, codeDocument.Source);

        if (GenerateBaselines)
        {
            var baselineFullPath = Path.Combine(TestProjectRoot, baselineFileName);
            File.WriteAllText(baselineFullPath, serializedMappings);
            return;
        }

        var testFile = TestFile.Create(baselineFileName, GetType().GetTypeInfo().Assembly);
        if (!testFile.Exists())
        {
            throw new XunitException($"The resource {baselineFileName} was not found.");
        }

        var baseline = testFile.ReadAllText();

        // Normalize newlines to match those in the baseline.
        var actualBaseline = serializedMappings.Replace("\r", "").Replace("\n", "\r\n");

        Assert.Equal(baseline, actualBaseline);

        var syntaxTree = codeDocument.GetSyntaxTree();
        var visitor = new CodeSpanVisitor();
        visitor.Visit(syntaxTree.Root);

        var charBuffer = new char[codeDocument.Source.Length];
        codeDocument.Source.CopyTo(0, charBuffer, 0, codeDocument.Source.Length);
        var sourceContent = new string(charBuffer);

        var spans = visitor.CodeSpans;
        for (var i = 0; i < spans.Count; i++)
        {
            var span = spans[i];
            var sourceSpan = span.GetSourceSpan(codeDocument.Source);
            if (sourceSpan == SourceSpan.Undefined)
            {
                // Not in the main file, skip.
                continue;
            }

            var expectedSpan = sourceContent.Substring(sourceSpan.AbsoluteIndex, sourceSpan.Length);

            // See #2593
            if (string.IsNullOrWhiteSpace(expectedSpan))
            {
                // For now we don't verify whitespace inside of a directive. We know that directives cheat
                // with how they bound whitespace/C#/markup to make completion work.
                if (span.FirstAncestorOrSelf<RazorDirectiveSyntax>() != null)
                {
                    continue;
                }
            }

            // See #2594
            if (string.Equals("@", expectedSpan, StringComparison.Ordinal))
            {
                // For now we don't verify an escaped transition. In some cases one of the @ tokens in @@foo
                // will be mapped as C# but will not be present in the output buffer because it's not actually C#.
                continue;
            }

            var found = false;
            for (var j = 0; j < csharpDocument.SourceMappings.Count; j++)
            {
                var mapping = csharpDocument.SourceMappings[j];
                if (mapping.OriginalSpan == sourceSpan)
                {
                    var actualSpan = csharpDocument.GeneratedCode.Substring(
                        mapping.GeneratedSpan.AbsoluteIndex,
                        mapping.GeneratedSpan.Length);

                    if (!string.Equals(expectedSpan, actualSpan, StringComparison.Ordinal))
                    {
                        throw new XunitException(
                            $"Found the span {sourceSpan} in the output mappings but it contains " +
                            $"'{EscapeWhitespace(actualSpan)}' instead of '{EscapeWhitespace(expectedSpan)}'.");
                    }

                    found = true;
                    break;
                }
            }

            if (!found)
            {
                throw new XunitException(
                    $"Could not find the span {sourceSpan} - containing '{EscapeWhitespace(expectedSpan)}' " +
                    $"in the output.");
            }
        }
    }

    protected void AssertLinePragmas(RazorCodeDocument codeDocument, bool designTime)
    {
        if (FileName is null)
        {
            var message = $"{nameof(AssertSourceMappingsMatchBaseline)} should only be called from an integration test. ({nameof(FileName)} is null).";
            throw new InvalidOperationException(message);
        }

        var csharpDocument = codeDocument.GetCSharpDocument();
        Assert.NotNull(csharpDocument);
        var linePragmas = csharpDocument.LinePragmas;
        designTime = false;
        if (designTime)
        {
            var sourceMappings = csharpDocument.SourceMappings;
            foreach (var sourceMapping in sourceMappings)
            {
                var foundMatchingPragma = false;
                foreach (var linePragma in linePragmas)
                {
                    if (sourceMapping.OriginalSpan.LineIndex >= linePragma.StartLineIndex &&
                        sourceMapping.OriginalSpan.LineIndex <= linePragma.EndLineIndex)
                    {
                        // Found a match.
                        foundMatchingPragma = true;
                        break;
                    }
                }

                Assert.True(foundMatchingPragma, $"No line pragma found for code at line {sourceMapping.OriginalSpan.LineIndex + 1}.");
            }
        }
        else
        {
            var syntaxTree = codeDocument.GetSyntaxTree();
            var sourceBuffer = new char[syntaxTree.Source.Length];
            syntaxTree.Source.CopyTo(0, sourceBuffer, 0, syntaxTree.Source.Length);
            var sourceContent = new string(sourceBuffer);
            var classifiedSpans = syntaxTree.GetClassifiedSpans();
            foreach (var classifiedSpan in classifiedSpans)
            {
                var content = sourceContent.Substring(classifiedSpan.Span.AbsoluteIndex, classifiedSpan.Span.Length);
                if (!string.IsNullOrWhiteSpace(content) &&
                    classifiedSpan.BlockKind != BlockKindInternal.Directive &&
                    classifiedSpan.SpanKind == SpanKindInternal.Code)
                {
                    var foundMatchingPragma = false;
                    foreach (var linePragma in linePragmas)
                    {
                        if (classifiedSpan.Span.LineIndex >= linePragma.StartLineIndex &&
                            classifiedSpan.Span.LineIndex <= linePragma.EndLineIndex)
                        {
                            // Found a match.
                            foundMatchingPragma = true;
                            break;
                        }
                    }

                    Assert.True(foundMatchingPragma, $"No line pragma found for code '{content}' at line {classifiedSpan.Span.LineIndex + 1}.");
                }
            }
        }
    }

    private class CodeSpanVisitor : SyntaxRewriter
    {
        public List<Syntax.SyntaxNode> CodeSpans { get; } = new List<Syntax.SyntaxNode>();

        public override Syntax.SyntaxNode VisitCSharpStatementLiteral(CSharpStatementLiteralSyntax node)
        {
            var context = node.GetSpanContext();
            if (context != null && context.ChunkGenerator != SpanChunkGenerator.Null)
            {
                CodeSpans.Add(node);
            }

            return base.VisitCSharpStatementLiteral(node);
        }

        public override Syntax.SyntaxNode VisitCSharpExpressionLiteral(CSharpExpressionLiteralSyntax node)
        {
            var context = node.GetSpanContext();
            if (context != null && context.ChunkGenerator != SpanChunkGenerator.Null)
            {
                CodeSpans.Add(node);
            }

            return base.VisitCSharpExpressionLiteral(node);
        }

        public override Syntax.SyntaxNode VisitCSharpStatement(CSharpStatementSyntax node)
        {
            if (node.FirstAncestorOrSelf<MarkupTagHelperAttributeValueSyntax>() != null)
            {
                // We don't support code blocks inside tag helper attribute values.
                // If it exists, we don't want to track its code spans for source mappings.
                return node;
            }

            return base.VisitCSharpStatement(node);
        }

        public override Syntax.SyntaxNode VisitRazorDirective(RazorDirectiveSyntax node)
        {
            if (node.FirstAncestorOrSelf<MarkupTagHelperAttributeValueSyntax>() != null)
            {
                // We don't support Razor directives inside tag helper attribute values.
                // If it exists, we don't want to track its code spans for source mappings.
                return node;
            }

            return base.VisitRazorDirective(node);
        }
    }

    private static string EscapeWhitespace(string content)
    {
        return content
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    private string NormalizeNewLines(string content)
    {
        return NormalizeNewLines(content, LineEnding);
    }

    private static string NormalizeNewLines(string content, string lineEnding)
    {
        return Regex.Replace(content, "(?<!\r)\n", lineEnding, RegexOptions.None, TimeSpan.FromSeconds(10));
    }

    // This is to prevent you from accidentally checking in with GenerateBaselines = true
    [Fact]
    public void GenerateBaselinesMustBeFalse()
    {
        Assert.False(GenerateBaselines, "GenerateBaselines should be set back to false before you check in!");
    }

    private class ConfigureCodeRenderingPhase : RazorEnginePhaseBase
    {
        public ConfigureCodeRenderingPhase(string lineEnding)
        {
            LineEnding = lineEnding;
        }

        public string LineEnding { get; }

        protected override void ExecuteCore(RazorCodeDocument codeDocument)
        {
            codeDocument.Items[CodeRenderingContext.SuppressUniqueIds] = "test";
            codeDocument.Items[CodeRenderingContext.NewLineString] = LineEnding;
        }
    }

    // 'Default' imports won't have normalized line-endings, which is unfriendly for testing.
    private class NormalizedDefaultImportFeature : RazorProjectEngineFeatureBase, IImportProjectFeature
    {
        private readonly IImportProjectFeature _inner;
        private readonly string _lineEnding;

        public NormalizedDefaultImportFeature(IImportProjectFeature inner, string lineEnding)
        {
            _inner = inner;
            _lineEnding = lineEnding;
        }

        protected override void OnInitialized()
        {
            if (_inner != null)
            {
                _inner.ProjectEngine = ProjectEngine;
            }
        }

        public IReadOnlyList<RazorProjectItem> GetImports(RazorProjectItem projectItem)
        {
            if (_inner is null)
            {
                return Array.Empty<RazorProjectItem>();
            }

            var normalizedImports = new List<RazorProjectItem>();
            var imports = _inner.GetImports(projectItem);
            foreach (var import in imports)
            {
                if (import.Exists)
                {
                    var text = string.Empty;
                    using (var stream = import.Read())
                    using (var reader = new StreamReader(stream))
                    {
                        text = reader.ReadToEnd().Trim();
                    }

                    // It's important that we normalize the newlines in the default imports. The default imports will
                    // be created with Environment.NewLine, but we need to normalize to `\r\n` so that the indices
                    // are the same on xplat.
                    var normalizedText = NormalizeNewLines(text, _lineEnding);
                    var normalizedImport = new TestRazorProjectItem(import.FilePath, import.PhysicalPath, import.RelativePhysicalPath, import.BasePath)
                    {
                        Content = normalizedText
                    };

                    normalizedImports.Add(normalizedImport);
                }
            }

            return normalizedImports;
        }
    }
}
