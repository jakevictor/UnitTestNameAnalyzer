using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using Moq;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer.Test.Unit.Services
{
    [TestFixture]
    public class AttributeServiceTests
    {
        private MockRepository moq;
        private Mock<IAttributeNameService> mockAttributeNameService;
        private AttributeService sut;

        [SetUp]
        public void SetUp()
        {
            moq = new MockRepository(MockBehavior.Strict);
            mockAttributeNameService = moq.Create<IAttributeNameService>();
            sut = new AttributeService(mockAttributeNameService.Object);
        }

        [TearDown]
        public void TearDown() =>
            moq.VerifyAll();

        [Test]
        public void HasAttribute_WhenAnAttributeListContainsAnAttributeWithNameMatchingTargetName_ReturnsTrue()
        {
            // Arrange
            var targetAttributeNames = new[] { "a", "d" };

            var attributeSyntaxB = SyntaxFactory.Attribute(SyntaxFactory.ParseName("b"));
            var attributeSyntaxC = SyntaxFactory.Attribute(SyntaxFactory.ParseName("c"));
            var attributeSyntaxD = SyntaxFactory.Attribute(SyntaxFactory.ParseName("d"));

            var attributeListB = SyntaxFactory.AttributeList(new SeparatedSyntaxList<AttributeSyntax>()
                .Add(attributeSyntaxB)
            );

            var attributeListCd = SyntaxFactory.AttributeList(new SeparatedSyntaxList<AttributeSyntax>()
                .Add(attributeSyntaxC)
                .Add(attributeSyntaxD)
            );

            mockAttributeNameService.Setup(s => s.GetName(It.Is<AttributeSyntax>(attributeSyntax => attributeSyntax.ToString() == attributeSyntaxB.ToString())))
                .Returns("b");
            mockAttributeNameService.Setup(s => s.GetName(It.Is<AttributeSyntax>(attributeSyntax => attributeSyntax.ToString() == attributeSyntaxC.ToString())))
                .Returns("c");
            mockAttributeNameService.Setup(s => s.GetName(It.Is<AttributeSyntax>(attributeSyntax => attributeSyntax.ToString() == attributeSyntaxD.ToString())))
                .Returns("d");

            var attributeLists = new SyntaxList<AttributeListSyntax>(new[] { attributeListB, attributeListCd });

            // Act
            var result = sut.HasAttribute(attributeLists, targetAttributeNames);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasAttribute_WhenNoAttributeListContainsAnAttributeWithNameMatchingTargetName_ReturnsFalse()
        {
            // Arrange
            var targetAttributeNames = new[] { "a" };

            var attributeSyntaxB = SyntaxFactory.Attribute(SyntaxFactory.ParseName("b"));

            var attributeListB = SyntaxFactory.AttributeList(new SeparatedSyntaxList<AttributeSyntax>()
                .Add(attributeSyntaxB)
            );

            mockAttributeNameService.Setup(s => s.GetName(It.Is<AttributeSyntax>(attributeSyntax => attributeSyntax.ToString() == attributeSyntaxB.ToString())))
                .Returns("b");

            var attributeLists = new SyntaxList<AttributeListSyntax>(new[] { attributeListB });

            // Act
            var result = sut.HasAttribute(attributeLists, targetAttributeNames);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}