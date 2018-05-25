using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal interface IMethodNameRule : IRule
    {
        void Enforce(SyntaxNodeAnalysisContext methodContext, MethodDeclarationSyntax methodDeclaration, string methodName);
    }
}