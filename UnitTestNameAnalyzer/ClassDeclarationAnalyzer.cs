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
    internal class ClassDeclarationAnalyzer : DiagnosticAnalyzer
    {
        private readonly IClassNameAnalyzer classNameAnalyzer;

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            classNameAnalyzer.SupportedDiagnostics;

        public ClassDeclarationAnalyzer() : this(CompositionRoot.GetClassNameAnalyzer())
        {
        }

        internal ClassDeclarationAnalyzer(IClassNameAnalyzer classNameAnalyzer) =>
            this.classNameAnalyzer = classNameAnalyzer;

        public override void Initialize(AnalysisContext context) =>
            context.RegisterSyntaxNodeAction(classNameAnalyzer.AnalyzeClassName, SyntaxKind.ClassDeclaration);
    }
}