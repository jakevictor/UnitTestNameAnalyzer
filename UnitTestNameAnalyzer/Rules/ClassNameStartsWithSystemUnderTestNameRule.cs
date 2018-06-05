using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal class ClassNameStartsWithSystemUnderTestNameRule : IClassNameRule
    {
        private const string Message = "Unit test fixture name '{0}' does not match system under test type '{1}'";

        public DiagnosticDescriptor DiagnosticDescriptor { get; } =
            new DiagnosticDescriptor(Constants.DiagnosticId, Constants.NonstandardClassNameTitle, Message, Constants.DiagnosticCategory, DiagnosticSeverity.Warning, true);

        public void Enforce(SyntaxNodeAnalysisContext classContext, ClassDeclarationSyntax classDeclaration)
        {
            var systemUnderTestFieldDeclaration = classDeclaration.DescendantNodes()
                .OfType<FieldDeclarationSyntax>()
                .SingleOrDefault(fieldDeclaration => fieldDeclaration.Declaration.Variables
                    .Any(fieldDeclarationVariable => fieldDeclarationVariable.Identifier.Text == Constants.SystemUnderTestFieldName));

            if (systemUnderTestFieldDeclaration == null)
            {
                return;
            }

            var systemUnderTestFieldTypeName = classContext.SemanticModel.GetSymbolInfo(systemUnderTestFieldDeclaration.Declaration.Type).Symbol.Name;

            var className = classDeclaration.Identifier.Text;

            if (className == $"{systemUnderTestFieldTypeName}Tests")
            {
                return;
            }

            classContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptor, classDeclaration.Identifier.GetLocation(), className, systemUnderTestFieldTypeName));
        }
    }
}