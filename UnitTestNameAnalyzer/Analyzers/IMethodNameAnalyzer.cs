using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Analyzers
{
    internal interface IMethodNameAnalyzer
    {
        ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

        void AnalyzeMethodName(SyntaxNodeAnalysisContext context);
    }
}