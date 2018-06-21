using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using UnitTestNameAnalyzer.Rules;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace UnitTestNameAnalyzer.Test.Unit.Rules
{
    [TestFixture]
    public class SecondPartOfMethodNameStartsWithWhenRuleTests
    {
        private SecondPartOfMethodNameStartsWithWhenRule sut;

        [SetUp]
        public void SetUp() =>
            sut = new SecondPartOfMethodNameStartsWithWhenRule();

        [Test]
        public void Enforce_WhenMethodNameHasOnePart_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenMethodNameHasTwoParts_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo_Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenMethodNameHasThreePartsAndSecondPartDoesNotStartWithWhen_ReportsWarningDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo_Foo_Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics.Count, Is.EqualTo(1));

            var diagnostic = reportedDiagnostics.Single();
            Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Warning));
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("Second part of unit test name 'Foo_Foo_Foo' does not start with 'When'."));

            Assert.That(diagnostic.Descriptor, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Id, Is.EqualTo(Constants.DiagnosticId));
            Assert.That(diagnostic.Descriptor.Title, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Title.ToString(), Is.EqualTo(Constants.NonstandardMethodNameTitle));
            Assert.That(diagnostic.Descriptor.Category, Is.EqualTo(Constants.DiagnosticCategory));
            Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);

            var expectedStartLocation = SourceText.IndexOf(methodName, StringComparison.Ordinal);
            var expectedEndLocation = expectedStartLocation + methodName.Length;
            Assert.That(diagnostic.Location, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan.Start, Is.EqualTo(expectedStartLocation));
            Assert.That(diagnostic.Location.SourceSpan.End, Is.EqualTo(expectedEndLocation));
        }

        [Test]
        public void Enforce_WhenMethodNameHasThreePartsAndSecondPartStartsWithImproperlyCasedWhen_ReportsWarningDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo_whenFoo_Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics.Count, Is.EqualTo(1));

            var diagnostic = reportedDiagnostics.Single();
            Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Warning));
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("Second part of unit test name 'Foo_whenFoo_Foo' does not start with 'When'."));

            Assert.That(diagnostic.Descriptor, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Id, Is.EqualTo(Constants.DiagnosticId));
            Assert.That(diagnostic.Descriptor.Title, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Title.ToString(), Is.EqualTo(Constants.NonstandardMethodNameTitle));
            Assert.That(diagnostic.Descriptor.Category, Is.EqualTo(Constants.DiagnosticCategory));
            Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);

            var expectedStartLocation = SourceText.IndexOf(methodName, StringComparison.Ordinal);
            var expectedEndLocation = expectedStartLocation + methodName.Length;
            Assert.That(diagnostic.Location, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan.Start, Is.EqualTo(expectedStartLocation));
            Assert.That(diagnostic.Location.SourceSpan.End, Is.EqualTo(expectedEndLocation));
        }

        [Test]
        public void Enforce_WhenMethodNameHasThreePartsAndSecondPartStartsWithProperlyCasedWhen_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo_WhenFoo_Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenMethodNameHasFourParts_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = "void Foo_Foo_Foo_Foo() {}";

            var syntaxTree = ParseSyntaxTree(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single();

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }
    }
}