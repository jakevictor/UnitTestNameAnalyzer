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

namespace UnitTestNameAnalyzer.Test.Unit.Rules
{
    [TestFixture]
    public class ClassNameStartsWithSystemUnderTestNameRuleTests
    {
        private ClassNameStartsWithSystemUnderTestNameRule sut;

        [SetUp]
        public void SetUp() =>
            sut = new ClassNameStartsWithSystemUnderTestNameRule();

        [Test]
        public void Enforce_WhenClassNameDoesNotStartWithTypeOfSystemUnderTestField_ReportsWarningDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo {}

                public class BarTests
                {
                    private Foo sut;
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var classContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(cd => cd.Identifier.Text == "BarTests");

            // Act
            sut.Enforce(classContext, classDeclaration);

            // Assert
            Assert.That(reportedDiagnostics.Count, Is.EqualTo(1));

            var diagnostic = reportedDiagnostics.Single();
            Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Warning));
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("Unit test fixture name 'BarTests' does not match system under test type 'Foo'"));

            Assert.That(diagnostic.Descriptor, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Id, Is.EqualTo(Constants.DiagnosticId));
            Assert.That(diagnostic.Descriptor.Title, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Title.ToString(), Is.EqualTo(Constants.NonstandardClassNameTitle));
            Assert.That(diagnostic.Descriptor.Category, Is.EqualTo(Constants.DiagnosticCategory));
            Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);

            var expectedStartLocation = SourceText.IndexOf("BarTests", StringComparison.Ordinal);
            var expectedEndLocation = expectedStartLocation + "BarTests".Length;
            Assert.That(diagnostic.Location, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan.Start, Is.EqualTo(expectedStartLocation));
            Assert.That(diagnostic.Location.SourceSpan.End, Is.EqualTo(expectedEndLocation));
        }

        [Test]
        public void Enforce_WhenClassNameStartsWithTypeOfSystemUnderTestField_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo {}

                public class FooTests
                {
                    private Foo sut;
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var classContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(cd => cd.Identifier.Text == "FooTests");

            // Act
            sut.Enforce(classContext, classDeclaration);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenClassNameStartsWithTypeOfFullyQualifiedSystemUnderTestField_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                namespace Blah
                {
                    public class Foo {}
                }

                public class FooTests
                {
                    private Blah.Foo sut;
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var classContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(cd => cd.Identifier.Text == "FooTests");

            // Act
            sut.Enforce(classContext, classDeclaration);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenClassContainsNoSystemUnderTestField_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo {}

                public class FooTests
                {
                    private Foo wrongName;
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var classContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var classDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().Single(cd => cd.Identifier.Text == "FooTests");

            // Act
            sut.Enforce(classContext, classDeclaration);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }
    }
}