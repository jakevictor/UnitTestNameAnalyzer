using System.Text.RegularExpressions;

namespace UnitTestNameAnalyzer
{
    internal class Constants
    {
        internal const string DiagnosticId = "UnitTestNameAnalyzer";

        internal const string DiagnosticCategory = "Naming";

        internal const string SystemUnderTestFieldName = "sut";

        internal const string NonstandardClassNameTitle = "Nonstandard unit test fixture name";

        internal const string NonstandardMethodNameTitle = "Nonstandard unit test name";

        internal static readonly Regex TwoOrThreePartMethodNameFormat = new Regex("^[^_]+(_[^_]+){1,2}$");

        internal static readonly string[] TestFixtureAttributeNames =
        {
            // MSTest
            "TestClass",
            // NUnit
            "TestFixture"
        };

        internal static readonly string[] TestAttributeNames =
        {
            // MSTest
            "TestMethod",
            // NUnit
            "Test"
        };
    }
}