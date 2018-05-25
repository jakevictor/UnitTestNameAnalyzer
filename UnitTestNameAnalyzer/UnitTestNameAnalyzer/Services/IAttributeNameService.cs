using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal interface IAttributeNameService
    {
        string GetName(AttributeSyntax attribute);
    }
}