using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UnitTestNameAnalyzer.Rules
{
    internal class FirstPartOfMethodNameIsInvokedMethodNameRule : IMethodNameRule
    {
        private const string Message = "Unit test name '{0}' does not start with the name of a method invoked on the system under test.";

        public DiagnosticDescriptor DiagnosticDescriptor { get; } =
            new DiagnosticDescriptor(Constants.DiagnosticId, Constants.NonstandardMethodNameTitle, Message, Constants.DiagnosticCategory, DiagnosticSeverity.Warning, true);

        public void Enforce(SyntaxNodeAnalysisContext methodContext, MethodDeclarationSyntax methodDeclaration, string methodName)
        {
            var methodsInvokedOnSystemUnderTest = GetMethodsInvokedOnSystemUnderTest(methodDeclaration);

            if (!methodsInvokedOnSystemUnderTest.Any())
            {
                // Could be because this is a constructor test or because the system under test isn't named "sut"
                // TODO: We could also parse "Foo" out of class name "FooTests" and look for invocations on an instance of Foo
                return;
            }

            if (MethodNameStartsWithMethodInvokedOnSystemUnderTest(methodName, methodsInvokedOnSystemUnderTest))
            {
                return;
            }

            methodContext.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptor, methodDeclaration.Identifier.GetLocation(), methodName));
        }

        private static IReadOnlyCollection<string> GetMethodsInvokedOnSystemUnderTest(MethodDeclarationSyntax methodDeclaration)
        {
            var invokedMethods = new List<string>();

            var invocationExpressions = methodDeclaration.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocationExpression in invocationExpressions)
            {
                var memberAccessExpression = invocationExpression.Expression as MemberAccessExpressionSyntax;

                if (memberAccessExpression?.Expression is IdentifierNameSyntax identifierName)
                {
                    var invokedMemberName = identifierName.Identifier.Text;

                    if (invokedMemberName == Constants.SystemUnderTestFieldName)
                    {
                        invokedMethods.Add(memberAccessExpression.Name.Identifier.Text);
                    }
                }
            }

            return invokedMethods;
        }

        private static bool MethodNameStartsWithMethodInvokedOnSystemUnderTest(string methodName, IReadOnlyCollection<string> methodsInvokedOnSystemUnderTest)
        {
            foreach (var methodInvokedOnSystemUnderTest in methodsInvokedOnSystemUnderTest)
            {
                if (methodName.StartsWith($"{methodInvokedOnSystemUnderTest}_", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}