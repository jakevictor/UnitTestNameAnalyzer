using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Analyzers
{
    internal interface IClassNameAnalyzer
    {
        ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        void AnalyzeClassName(SyntaxNodeAnalysisContext context);
    }
}