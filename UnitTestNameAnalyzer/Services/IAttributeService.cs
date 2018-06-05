using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal interface IAttributeService
    {
        bool HasAttribute(SyntaxList<AttributeListSyntax> attributeLists, IReadOnlyCollection<string> targetAttributeNames);
    }
}