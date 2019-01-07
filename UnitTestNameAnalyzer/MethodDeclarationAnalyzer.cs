using System.Collections.Immutable;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using UnitTestNameAnalyzer.Analyzers;

namespace UnitTestNameAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    [UsedImplicitly]
    [ExcludeFromCodeCoverage]
    internal class MethodDeclarationAnalyzer : DiagnosticAnalyzer
    {
        private readonly IMethodNameAnalyzer methodNameAnalyzer;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            methodNameAnalyzer.SupportedDiagnostics;

        public MethodDeclarationAnalyzer() : this(CompositionRoot.GetMethodNameAnalyzer())
        {
        }

        internal MethodDeclarationAnalyzer(IMethodNameAnalyzer methodNameAnalyzer) =>
            this.methodNameAnalyzer = methodNameAnalyzer;

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(methodNameAnalyzer.AnalyzeMethodName, SyntaxKind.MethodDeclaration);
    }
}