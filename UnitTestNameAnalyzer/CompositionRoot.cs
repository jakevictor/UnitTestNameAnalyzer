using System.Collections.Generic;
using UnitTestNameAnalyzer.Analyzers;
using UnitTestNameAnalyzer.Rules;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer
{
    [ExcludeFromCodeCoverage]
    internal static class CompositionRoot
    {
        internal static IClassNameAnalyzer GetClassNameAnalyzer() =>
            new ClassNameAnalyzer(
                GetClassNameRules(),
                GetNamespaceService(),
                GetAttributeService());

        internal static IMethodNameAnalyzer GetMethodNameAnalyzer() =>
            new MethodNameAnalyzer(
                GetMethodNameRules(),
                GetNamespaceService(),
                GetAttributeService());

        private static IReadOnlyCollection<IClassNameRule> GetClassNameRules() =>
            new[]
            {
                new ClassNameStartsWithSystemUnderTestNameRule()
            };

        private static IReadOnlyCollection<IMethodNameRule> GetMethodNameRules() =>
            new IMethodNameRule[]
            {
                new MethodNameIsTwoOrThreePartsRule(),
                new FirstPartOfMethodNameIsInvokedMethodNameRule(),
                new SecondPartOfMethodNameStartsWithWhenRule()
            };

        private static INamespaceService GetNamespaceService() =>
            new NamespaceService();

        private static IAttributeService GetAttributeService() =>
            new AttributeService(
                new AttributeNameService(
                    new AttributeSimpleNameService()));
    }
}