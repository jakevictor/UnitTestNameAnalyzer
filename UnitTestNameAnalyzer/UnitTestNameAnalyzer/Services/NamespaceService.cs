using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Services
{
    internal class NamespaceService : INamespaceService
    {
        public bool IsInUnitTestNamespace(SyntaxNodeAnalysisContext context)
        {
            var namespaceDeclarations = context.SemanticModel.SyntaxTree.GetRoot().DescendantNodes().OfType<NamespaceDeclarationSyntax>();

            foreach (var namespaceDeclaration in namespaceDeclarations)
            {
                var namespaceName = context.SemanticModel.GetDeclaredSymbol(namespaceDeclaration).ToDisplayString();

                if (namespaceName.Split('.').Contains("Unit"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}