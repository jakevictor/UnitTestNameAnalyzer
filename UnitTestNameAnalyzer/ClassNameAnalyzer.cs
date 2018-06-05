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
    internal class ClassNameAnalyzer : DiagnosticAnalyzer
    {
        private readonly IReadOnlyCollection<IClassNameRule> classNameRules;

        private readonly INamespaceService namespaceService;

        private readonly IAttributeService attributeService;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            classNameRules.Select(rule => rule.DiagnosticDescriptor).ToImmutableArray();

        public ClassNameAnalyzer() : this(
            CompositionRoot.GetClassNameRules(),
            CompositionRoot.GetNamespaceService(),
            CompositionRoot.GetAttributeService())
        { }

        internal ClassNameAnalyzer(IReadOnlyCollection<IClassNameRule> classNameRules, INamespaceService namespaceService, IAttributeService attributeService)
        {
            this.classNameRules = classNameRules;
            this.namespaceService = namespaceService;
            this.attributeService = attributeService;
        }

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
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

        private bool IsUnitTestFixture(SyntaxNodeAnalysisContext classContext, ClassDeclarationSyntax classDeclaration) =>
            namespaceService.IsInUnitTestNamespace(classContext) && attributeService.HasAttribute(classDeclaration.AttributeLists, Constants.TestFixtureAttributeNames);
    }
}