using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnitTestNameAnalyzer.Services
{
    internal interface IAttributeSimpleNameService
    {
        SimpleNameSyntax GetSimpleName(AttributeSyntax attribute);
    }
}