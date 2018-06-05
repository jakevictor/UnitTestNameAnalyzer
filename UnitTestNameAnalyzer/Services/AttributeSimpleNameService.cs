using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal class AttributeSimpleNameService : IAttributeSimpleNameService
    {
        public SimpleNameSyntax GetSimpleName(AttributeSyntax attribute)
        {
            // [Test]
            var identifierNameSyntax = attribute.Name as IdentifierNameSyntax;

            // [NUnit.Framework.Test]
            var qualifiedNameSyntax = attribute.Name as QualifiedNameSyntax;

            // using AliasedNamespace = NUnit.Framework;
            // ...
            // [AliasedNamespace::Test]
            var aliasQualifiedNameSyntax = attribute.Name as AliasQualifiedNameSyntax;

            return identifierNameSyntax ??
                qualifiedNameSyntax?.Right ??
                aliasQualifiedNameSyntax?.Name;
        }
    }
}