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
    internal class ClassNameAnalyzer : IClassNameAnalyzer
    {
        private readonly IReadOnlyCollection<IClassNameRule> classNameRules;
        private readonly INamespaceService namespaceService;
        private readonly IAttributeService attributeService;

        public ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            classNameRules.Select(rule => rule.DiagnosticDescriptor).ToImmutableArray();

        internal ClassNameAnalyzer(IReadOnlyCollection<IClassNameRule> classNameRules, INamespaceService namespaceService, IAttributeService attributeService)
        {
            this.classNameRules = classNameRules;
            this.namespaceService = namespaceService;
            this.attributeService = attributeService;
        }

        public void AnalyzeClassName(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (!IsUnitTestFixture(context, classDeclaration))
            {
                return;
            }

            foreach (var classNameRule in classNameRules)
            {
                classNameRule.Enforce(context, classDeclaration);
            }
        }

        private bool IsUnitTestFixture(SyntaxNodeAnalysisContext classContext, BaseTypeDeclarationSyntax classDeclaration) =>
            namespaceService.IsInUnitTestNamespace(classContext) && attributeService.HasAttribute(classDeclaration.AttributeLists, Constants.TestFixtureAttributeNames);
    }
}