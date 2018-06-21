using System;
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
    public class MethodNameAnalyzerTests
    {
        private MockRepository moq;
        private Mock<IMethodNameRule> mockMethodNameRule0;
        private Mock<IMethodNameRule> mockMethodNameRule1;
        private Mock<INamespaceService> mockNamespaceService;
        private Mock<IAttributeService> mockAttributeService;
        private MethodNameAnalyzer sut;

        [SetUp]
        public void SetUp()
        {
            moq = new MockRepository(MockBehavior.Strict);
            mockMethodNameRule0 = moq.Create<IMethodNameRule>();
            mockMethodNameRule1 = moq.Create<IMethodNameRule>();
            mockNamespaceService = moq.Create<INamespaceService>();
            mockAttributeService = moq.Create<IAttributeService>();
            sut = new MethodNameAnalyzer(new[] { mockMethodNameRule0.Object, mockMethodNameRule1.Object }, mockNamespaceService.Object, mockAttributeService.Object);
        }

        [TearDown]
        public void TearDown() =>
            moq.VerifyAll();

        [Test]
        public void AnalyzeMethodName_WhenMethodDeclarationIsInUnitTestNamespaceAndHasTestAttribute_EnforcesEachMethodNameRuleOnMethodDeclaration()
        {
            // Arrange
            var methodName = Guid.NewGuid().ToString();

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(string.Empty), methodName);
            methodDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(true);

            mockAttributeService.Setup(s => s.HasAttribute(methodDeclaration.AttributeLists, Constants.TestAttributeNames))
                .Returns(true);

            mockMethodNameRule0.Setup(r => r.Enforce(context, methodDeclaration, methodName));
            mockMethodNameRule1.Setup(r => r.Enforce(context, methodDeclaration, methodName));

            // Act / Assert
            sut.AnalyzeMethodName(context);
        }

        [Test]
        public void AnalyzeMethodName_WhenMethodDeclarationIsInUnitTestNamespaceButDoesNotHaveTestAttribute_DoesNotEnforceMethodNameRules()
        {
            // Arrange
            var methodName = Guid.NewGuid().ToString();

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(string.Empty), methodName);
            methodDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(true);

            mockAttributeService.Setup(s => s.HasAttribute(methodDeclaration.AttributeLists, Constants.TestAttributeNames))
                .Returns(false);

            // Act / Assert
            sut.AnalyzeMethodName(context);
        }

        [Test]
        public void AnalyzeMethodName_WhenMethodDeclarationIsNotInUnitTestNamespace_DoesNotEnforceMethodNameRules()
        {
            // Arrange
            var methodName = Guid.NewGuid().ToString();

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(string.Empty), methodName);
            methodDeclaration.AttributeLists.Add(SyntaxFactory.AttributeList());

            var context = new SyntaxNodeAnalysisContext(methodDeclaration, null, null, null, null, default(CancellationToken));

            mockNamespaceService.Setup(s => s.IsInUnitTestNamespace(context))
                .Returns(false);

            // Act / Assert
            sut.AnalyzeMethodName(context);
        }

        [Test]
        public void SupportedDiagnostics_ReturnsDiagnosticDescriptorsOfMethodNameRules()
        {
            // Arrange
            var diagnosticDescriptor0 = new DiagnosticDescriptor("0", null, null, string.Empty, DiagnosticSeverity.Info, false);
            var diagnosticDescriptor1 = new DiagnosticDescriptor("1", null, null, string.Empty, DiagnosticSeverity.Info, false);

            mockMethodNameRule0.Setup(r => r.DiagnosticDescriptor)
                .Returns(diagnosticDescriptor0);
            mockMethodNameRule1.Setup(r => r.DiagnosticDescriptor)
                .Returns(diagnosticDescriptor1);

            // Act
            var result = sut.SupportedDiagnostics;

            // Assert
            Assert.That(result, Is.EquivalentTo(new[] { diagnosticDescriptor0, diagnosticDescriptor1 }));
        }
    }
}