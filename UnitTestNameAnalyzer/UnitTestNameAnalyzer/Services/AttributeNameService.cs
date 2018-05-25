using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal class AttributeNameService : IAttributeNameService
    {
        private readonly IAttributeSimpleNameService attributeSimpleNameService;

        internal AttributeNameService(IAttributeSimpleNameService attributeSimpleNameService) =>
            this.attributeSimpleNameService = attributeSimpleNameService;

        public string GetName(AttributeSyntax attribute)
        {
            var simpleName = attributeSimpleNameService.GetSimpleName(attribute);

            var name = simpleName.Identifier.Text;

            const string AttributeSuffix = "Attribute";
            if (name.EndsWith(AttributeSuffix, StringComparison.Ordinal))
            {
                name = name.Substring(0, name.Length - AttributeSuffix.Length);
            }

            return name;
        }
    }
}