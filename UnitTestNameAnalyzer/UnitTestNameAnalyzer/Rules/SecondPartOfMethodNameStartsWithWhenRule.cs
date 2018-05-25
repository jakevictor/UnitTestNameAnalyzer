using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal class SecondPartOfMethodNameStartsWithWhenRule : IMethodNameRule
    {
        private const string Message = "Second part of unit test name '{0}' does not start with 'When'.";

        public DiagnosticDescriptor DiagnosticDescriptor { get; } =
            new DiagnosticDescriptor(Constants.DiagnosticId, Constants.NonstandardMethodNameTitle, Message, Constants.DiagnosticCategory, DiagnosticSeverity.Warning, true);

        public void Enforce(SyntaxNodeAnalysisContext methodContext, MethodDeclarationSyntax methodDeclaration, string methodName)
        {
            var nameFormatMatch = Constants.TwoOrThreePartMethodNameFormat.Match(methodName);

            if (!nameFormatMatch.Success)
            {
                return;
            }

            var nameParts = methodName.Split('_');

            if (nameParts.Length != 3)
            {
                return;
            }

            if (nameParts[1].StartsWith("When", StringComparison.Ordinal))
            {
                return;
            }

            methodContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptor, methodDeclaration.Identifier.GetLocation(), methodName));
        }
    }
}