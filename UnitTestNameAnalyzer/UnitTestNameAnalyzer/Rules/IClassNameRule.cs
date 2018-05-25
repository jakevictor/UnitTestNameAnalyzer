using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal interface IClassNameRule : IRule
    {
        void Enforce(SyntaxNodeAnalysisContext classContext, ClassDeclarationSyntax classDeclaration);
    }
}