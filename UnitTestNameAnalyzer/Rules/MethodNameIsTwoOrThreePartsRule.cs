using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal class MethodNameIsTwoOrThreePartsRule : IMethodNameRule
    {
        private const string Message = "Unit test name '{0}' is not two or three parts separated by an underscore.";

        public DiagnosticDescriptor DiagnosticDescriptor { get; } =
            new DiagnosticDescriptor(Constants.DiagnosticId, Constants.NonstandardMethodNameTitle, Message, Constants.DiagnosticCategory, DiagnosticSeverity.Warning, true);

        public void Enforce(SyntaxNodeAnalysisContext methodContext, MethodDeclarationSyntax methodDeclaration, string methodName)
        {
            if (Constants.TwoOrThreePartMethodNameFormat.IsMatch(methodName))
            {
                return;
            }

            methodContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptor, methodDeclaration.Identifier.GetLocation(), methodName));
        }
    }
}