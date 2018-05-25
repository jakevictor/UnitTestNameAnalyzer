using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using UnitTestNameAnalyzer.Rules;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [UsedImplicitly]
    internal class MethodNameAnalyzer : DiagnosticAnalyzer
    {
        private readonly IReadOnlyCollection<IMethodNameRule> methodNameRules;

        private readonly INamespaceService namespaceService;

        private readonly IAttributeService attributeService;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            methodNameRules.Select(rule => rule.DiagnosticDescriptor).ToImmutableArray();

        public MethodNameAnalyzer() : this(
            CompositionRoot.GetMethodNameRules(),
            CompositionRoot.GetNamespaceService(),
            CompositionRoot.GetAttributeService())
        { }

        internal MethodNameAnalyzer(IReadOnlyCollection<IMethodNameRule> methodNameRules, INamespaceService namespaceService, IAttributeService attributeService)
        {
            this.methodNameRules = methodNameRules;
            this.namespaceService = namespaceService;
            this.attributeService = attributeService;
        }

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
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

        private bool IsUnitTest(SyntaxNodeAnalysisContext methodContext, MethodDeclarationSyntax methodDeclaration) =>
            namespaceService.IsInUnitTestNamespace(methodContext) && attributeService.HasAttribute(methodDeclaration.AttributeLists, Constants.TestAttributeNames);
    }
}
