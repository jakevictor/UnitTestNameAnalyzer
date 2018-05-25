using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal class AttributeService : IAttributeService
    {
        private readonly IAttributeNameService attributeNameService;

        internal AttributeService(IAttributeNameService attributeNameService) =>
            this.attributeNameService = attributeNameService;

        public bool HasAttribute(SyntaxList<AttributeListSyntax> attributeLists, IReadOnlyCollection<string> targetAttributeNames)
        {
            foreach (var attributeList in attributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var attributeName = attributeNameService.GetName(attribute);

                    if (targetAttributeNames.Contains(attributeName))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}