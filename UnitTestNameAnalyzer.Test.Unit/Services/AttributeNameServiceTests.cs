using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Moq;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer.Test.Unit.Services
{
    [TestFixture]
    public class AttributeNameServiceTests
    {
        private MockRepository moq;
        private Mock<IAttributeSimpleNameService> mockAttributeSimpleNameService;
        private AttributeNameService sut;

        [SetUp]
        public void SetUp()
        {
            moq = new MockRepository(MockBehavior.Strict);
            mockAttributeSimpleNameService = moq.Create<IAttributeSimpleNameService>();
            sut = new AttributeNameService(mockAttributeSimpleNameService.Object);
        }

        [TearDown]
        public void TearDown() =>
            moq.VerifyAll();

        [Test]
        public void GetName_ReturnsIdentifierTextForSimpleNameOfAttribute()
        {
            // Arrange
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(string.Empty));

            const string AttributeName = "Foo";

            var simpleName = SyntaxFactory.ParseName(AttributeName) as SimpleNameSyntax;

            mockAttributeSimpleNameService.Setup(s => s.GetSimpleName(attribute))
                .Returns(simpleName);

            // Act
            var result = sut.GetName(attribute);

            // Assert
            Assert.That(result, Is.EqualTo("Foo"));
        }

        [Test]
        public void GetName_WhenNameOfAttributeIncludesAttributeSuffix_OmitsAttributeSuffix()
        {
            // Arrange
            var attribute = SyntaxFactory.Attribute(SyntaxFactory.ParseName(string.Empty));

            const string AttributeName = "FooAttribute";

            var simpleName = SyntaxFactory.ParseName(AttributeName) as SimpleNameSyntax;

            mockAttributeSimpleNameService.Setup(s => s.GetSimpleName(attribute))
                .Returns(simpleName);

            // Act
            var result = sut.GetName(attribute);

            // Assert
            Assert.That(result, Is.EqualTo("Foo"));
        }
    }
}