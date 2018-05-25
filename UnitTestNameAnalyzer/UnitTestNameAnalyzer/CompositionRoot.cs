using System.Collections.Generic;
using UnitTestNameAnalyzer.Rules;
using UnitTestNameAnalyzer.Services;

namespace UnitTestNameAnalyzer
{
    internal static class CompositionRoot
    {
        internal static IReadOnlyCollection<IClassNameRule> GetClassNameRules() =>
            new[]
            {
                new ClassNameStartsWithSystemUnderTestNameRule()
            };

        internal static  IReadOnlyCollection<IMethodNameRule> GetMethodNameRules() =>
            new IMethodNameRule[]
            {
                new MethodNameIsTwoOrThreePartsRule(),
                new FirstPartOfMethodNameIsInvokedMethodNameRule(),
                new SecondPartOfMethodNameStartsWithWhenRule()
            };

        internal static IAttributeService GetAttributeService() =>
            new AttributeService(
                new AttributeNameService(
                    new AttributeSimpleNameService()));

        internal static INamespaceService GetNamespaceService() =>
            new NamespaceService();
    }
}