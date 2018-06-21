using System.Threading;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer.Test.Unit.Services
{
    [TestFixture]
    public class NamespaceServiceTests
    {
        private NamespaceService sut;

        [SetUp]
        public void SetUp() =>
            sut = new NamespaceService();

        [Test]
        public void IsInUnitTestNamespace_WhenNameInNamespaceDeclarationContainsUnitSegment_ReturnsTrue()
        {
            // Arrange
            var syntaxTree = SyntaxFactory.ParseSyntaxTree("namespace Foo.Test.Unit {}");

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var context = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, null, null, default(CancellationToken));

            // Act
            var result = sut.IsInUnitTestNamespace(context);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsInUnitTestNamespace_WhenNameInNamespaceDeclarationDoesNotContainUnitSegment_ReturnsFalse()
        {
            // Arrange
            var syntaxTree = SyntaxFactory.ParseSyntaxTree("namespace Foo.Test.Unity {}");

            var compilation = CSharpCompilation.Create(null, new[] { syntaxTree });

            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            var context = new SyntaxNodeAnalysisContext(syntaxTree.GetRoot(), semanticModel, null, null, null, default(CancellationToken));

            // Act
            var result = sut.IsInUnitTestNamespace(context);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}