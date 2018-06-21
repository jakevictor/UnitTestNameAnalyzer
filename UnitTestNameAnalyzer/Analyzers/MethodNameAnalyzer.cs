using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UnitTestNameAnalyzer.Rules;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer.Analyzers
{
    internal class MethodNameAnalyzer : IMethodNameAnalyzer
    {
        private readonly IReadOnlyCollection<IMethodNameRule> methodNameRules;
        private readonly INamespaceService namespaceService;
        private readonly IAttributeService attributeService;

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            methodNameRules.Select(rule => rule.DiagnosticDescriptor).ToImmutableArray();

        internal MethodNameAnalyzer(IReadOnlyCollection<IMethodNameRule> methodNameRules, INamespaceService namespaceService, IAttributeService attributeService)
        {
            this.methodNameRules = methodNameRules;
            this.namespaceService = namespaceService;
            this.attributeService = attributeService;
        }

        public void AnalyzeMethodName(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;

            if (!IsUnitTest(context, methodDeclaration))
            {
                return;
            }

            var methodName = methodDeclaration.Identifier.Text;

            foreach (var methodNameRule in methodNameRules)
            {
                methodNameRule.Enforce(context, methodDeclaration, methodName);
            }
        }

        private bool IsUnitTest(SyntaxNodeAnalysisContext methodContext, BaseMethodDeclarationSyntax methodDeclaration) =>
            namespaceService.IsInUnitTestNamespace(methodContext) && attributeService.HasAttribute(methodDeclaration.AttributeLists, Constants.TestAttributeNames);
    }
}
