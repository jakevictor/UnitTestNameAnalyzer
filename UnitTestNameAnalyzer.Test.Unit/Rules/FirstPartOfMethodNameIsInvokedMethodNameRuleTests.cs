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
    public class FirstPartOfMethodNameIsInvokedMethodNameRuleTests
    {
        private FirstPartOfMethodNameIsInvokedMethodNameRule sut;

        [SetUp]
        public void SetUp() =>
            sut = new FirstPartOfMethodNameIsInvokedMethodNameRule();

        [Test]
        public void Enforce_WhenFirstPartOfMethodNameIsNotInvoked_ReportsWarningDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo
                {
                    public void Bar {}
                }

                public class FooTests
                {
                    private Foo sut;

                    [Test]
                    public void Baz_DoesStuff()
                    {
                        sut.Bar();
                    }
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

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
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("Unit test name 'Baz_DoesStuff' does not start with the name of a method invoked on the system under test."));

            Assert.That(diagnostic.Descriptor, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Id, Is.EqualTo(Constants.DiagnosticId));
            Assert.That(diagnostic.Descriptor.Title, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Title.ToString(), Is.EqualTo(Constants.NonstandardMethodNameTitle));
            Assert.That(diagnostic.Descriptor.Category, Is.EqualTo(Constants.DiagnosticCategory));
            Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);

            var expectedStartLocation = SourceText.IndexOf("Baz_DoesStuff", StringComparison.Ordinal);
            var expectedEndLocation = expectedStartLocation + "Baz_DoesStuff".Length;
            Assert.That(diagnostic.Location, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan.Start, Is.EqualTo(expectedStartLocation));
            Assert.That(diagnostic.Location.SourceSpan.End, Is.EqualTo(expectedEndLocation));
        }

        [Test]
        public void Enforce_WhenFirstPartOfMethodNameIsInvokedOnObjectOtherThanSystemUnderTest_ReportsWarningDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo
                {
                    public void Bar() {}
                    public void Baz() {}
                }

                public class FooTests
                {
                    private Foo sut;
                    private Foo other;

                    [Test]
                    public void Baz_DoesStuff()
                    {
                        sut.Bar();
                        other.Baz();
                    }
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(ds => ds.Identifier.Text == "Baz_DoesStuff");

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics.Count, Is.EqualTo(1));

            var diagnostic = reportedDiagnostics.Single();
            Assert.That(diagnostic.Severity, Is.EqualTo(DiagnosticSeverity.Warning));
            Assert.That(diagnostic.GetMessage(), Is.EqualTo("Unit test name 'Baz_DoesStuff' does not start with the name of a method invoked on the system under test."));

            Assert.That(diagnostic.Descriptor, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Id, Is.EqualTo(Constants.DiagnosticId));
            Assert.That(diagnostic.Descriptor.Title, Is.Not.Null);
            Assert.That(diagnostic.Descriptor.Title.ToString(), Is.EqualTo(Constants.NonstandardMethodNameTitle));
            Assert.That(diagnostic.Descriptor.Category, Is.EqualTo(Constants.DiagnosticCategory));
            Assert.That(diagnostic.Descriptor.IsEnabledByDefault, Is.True);

            var expectedStartLocation = SourceText.IndexOf("Baz_DoesStuff", StringComparison.Ordinal);
            var expectedEndLocation = expectedStartLocation + "Baz_DoesStuff".Length;
            Assert.That(diagnostic.Location, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan, Is.Not.Null);
            Assert.That(diagnostic.Location.SourceSpan.Start, Is.EqualTo(expectedStartLocation));
            Assert.That(diagnostic.Location.SourceSpan.End, Is.EqualTo(expectedEndLocation));
        }

        [Test]
        public void Enforce_WhenFirstPartOfMethodNameIsInvokedOnSystemUnderTest_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo
                {
                    public void Baz() {}
                }

                public class FooTests
                {
                    private Foo sut;

                    [Test]
                    public void Baz_DoesStuff()
                    {
                        sut.Baz();
                    }
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(ds => ds.Identifier.Text == "Baz_DoesStuff");

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }

        [Test]
        public void Enforce_WhenNothingIsInvokedOnSystemUnderTest_DoesNotReportDiagnostic()
        {
            // Arrange
            const string SourceText = @"
                public class Foo
                {
                    public void Baz() {}
                }

                public class FooTests
                {
                    private Foo sut;

                    [Test]
                    public void Baz_DoesStuff()
                    {
                    }
                }
            ";

            var syntaxTree = CSharpSyntaxTree.ParseText(SourceText);

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var reportedDiagnostics = new List<Diagnostic>();

            var methodContext = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, reportedDiagnostics.Add, d => true, default(CancellationToken));

            var methodDeclaration = syntaxTree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>().Single(ds => ds.Identifier.Text == "Baz_DoesStuff");

            var methodName = methodDeclaration.Identifier.Text;

            // Act
            sut.Enforce(methodContext, methodDeclaration, methodName);

            // Assert
            Assert.That(reportedDiagnostics, Is.Empty);
        }
    }
}
