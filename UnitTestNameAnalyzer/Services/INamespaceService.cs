using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Services
{
    internal interface INamespaceService
    {
        bool IsInUnitTestNamespace(SyntaxNodeAnalysisContext context);
    }
}