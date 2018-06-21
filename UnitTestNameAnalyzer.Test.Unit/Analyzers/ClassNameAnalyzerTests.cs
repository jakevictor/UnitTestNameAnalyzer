using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Moq;
using NUnit.Framework;
using UnitTestNameAnalyzer.Analyzers;
using UnitTestNameAnalyzer.Rules;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer.Test.Unit.Analyzers
{
    [TestFixture]
    public class ClassNameAnalyzerTests
    {
        private MockRepository moq;
        private Mock<IClassNameRule> mockClassNameRule0;
        private Mock<IClassNameRule> mockClassNameRule1;
        private Mock<INamespaceService> mockNamespaceService;
        private Mock<IAttributeService> mockAttributeService;
        private ClassNameAnalyzer sut;

        [SetUp]
        public void SetUp()
        {
            moq = new MockRepository(MockBehavior.Strict);
            mockClassNameRule0 = moq.Create<IClassNameRule>();
            mockClassNameRule1 = moq.Create<IClassNameRule>();
            mockNamespaceService = moq.Create<INamespaceService>();
            mockAttributeService = moq.Create<IAttributeService>();
            sut = new ClassNameAnalyzer(new[] { mockClassNameRule0.Object, mockClassNameRule1.Object }, mockNamespaceService.Object, mockAttributeService.Object);
        }

        [TearDown]
        public void TearDown() =>
            moq.VerifyAll();

        [Test]
        public void AnalyzeClassName_WhenClassDeclarationIsInUnitTestNamespaceAndHasTestFixtureAttribute_EnforcesEachClassNameRuleOnClassDeclaration()
        {
            // Arrange
            var classDeclaration = SyntaxFactory.ClassDeclaration(string.Empty);
            classDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(classDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(true);

            mockAttributeService.Setup(s => s.HasAttribute(classDeclaration.AttributeLists, Constants.TestFixtureAttributeNames))
                .Returns(true);

            mockClassNameRule0.Setup(r => r.Enforce(context, classDeclaration));
            mockClassNameRule1.Setup(r => r.Enforce(context, classDeclaration));

            // Act / Assert
            sut.AnalyzeClassName(context);
        }

        [Test]
        public void AnalyzeClassName_WhenClassDeclarationIsInUnitTestNamespaceButDoesNotHaveTestFixtureAttribute_DoesNotEnforceClassNameRules()
        {
            // Arrange
            var classDeclaration = SyntaxFactory.ClassDeclaration(string.Empty);
            classDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(classDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(true);

            mockAttributeService.Setup(s => s.HasAttribute(classDeclaration.AttributeLists, Constants.TestFixtureAttributeNames))
                .Returns(false);

            // Act / Assert
            sut.AnalyzeClassName(context);
        }

        [Test]
        public void AnalyzeClassName_WhenClassDeclarationIsNotInUnitTestNamespace_DoesNotEnforceClassNameRules()
        {
            // Arrange
            var classDeclaration = SyntaxFactory.ClassDeclaration(string.Empty);
            classDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(classDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(false);

            // Act / Assert
            sut.AnalyzeClassName(context);
        }

        [Test]
        public void SupportedDiagnostics_ReturnsDiagnosticDescriptorsOfClassNameRules()
        {
            // Arrange
            var diagnosticDescriptor0 = new DiagnosticDescriptor("0", null, null, string.Empty, DiagnosticSeverity.Info, false);
            var diagnosticDescriptor1 = new DiagnosticDescriptor("1", null, null, string.Empty, DiagnosticSeverity.Info, false);

            mockClassNameRule0.Setup(r => r.DiagnosticDescriptor)
                .Returns(diagnosticDescriptor0);
            mockClassNameRule1.Setup(r => r.DiagnosticDescriptor)
                .Returns(diagnosticDescriptor1);

            // Act
            var result = sut.SupportedDiagnostics;

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { diagnosticDescriptor0, diagnosticDescriptor1 }));
        }
    }
}