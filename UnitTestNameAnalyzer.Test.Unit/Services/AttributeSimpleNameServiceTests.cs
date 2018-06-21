using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using UnitTestNameAnalyzer.Services;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace UnitTestNameAnalyzer.Test.Unit.Services
{
    [TestFixture]
    public class AttributeSimpleNameServiceTests
    {
        private AttributeSimpleNameService sut;

        [SetUp]
        public void SetUp() =>
            sut = new AttributeSimpleNameService();

        [Test]
        public void GetSimpleName_WhenAttributeNameIsPlainIdentifier_ReturnsIdentifier()
        {
            // Arrange
            var attributeName = IdentifierName("Test");

            var attribute = Attribute(attributeName);

            // Act
            var result = sut.GetSimpleName(attribute);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identifier, Is.Not.Null);
            Assert.That(result.Identifier.ToString(), Is.EqualTo("Test"));
        }

        [Test]
        public void GetSimpleName_WhenAttributeNameIsQualifiedIdentifier_ReturnsRightSideOfIdentifier()
        {
            // Arrange
            var attributeName = QualifiedName(
                ParseName("NUnit.Framework"),
                ParseName("Test") as SimpleNameSyntax
            );

            var attribute = Attribute(attributeName);

            // Act
            var result = sut.GetSimpleName(attribute);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identifier, Is.Not.Null);
            Assert.That(result.Identifier.ToString(), Is.EqualTo("Test"));
        }

        [Test]
        public void GetSimpleName_WhenAttributeNameIsNamespacedIdentifier_ReturnsRightSideOfIdentifier()
        {
            // Arrange
            var attributeName = AliasQualifiedName(
                IdentifierName("AliasedNamespace"),
                ParseToken("::"),
                ParseName("Test") as SimpleNameSyntax
            );

            var attribute = Attribute(attributeName);

            // Act
            var result = sut.GetSimpleName(attribute);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identifier, Is.Not.Null);
            Assert.That(result.Identifier.ToString(), Is.EqualTo("Test"));
        }
    }
}