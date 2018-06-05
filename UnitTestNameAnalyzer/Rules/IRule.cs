using Microsoft.CodeAnalysis;

namespace UnitTestNameAnalyzer.Rules
{
    internal interface IRule
    {
        DiagnosticDescriptor DiagnosticDescriptor { get; }
    }
}