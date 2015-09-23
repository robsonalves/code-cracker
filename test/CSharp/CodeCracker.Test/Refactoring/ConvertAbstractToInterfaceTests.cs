using CodeCracker.CSharp.Refactoring;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace CodeCracker.Test.CSharp.Refactoring
{
    public class ConvertAbstractToInterfaceTests : CodeFixVerifier<ConvertAbstractToInterfaceAnalyzer, ConvertAbstractToInterfaceProvider>
    {
        [Fact]
        public async Task IfHaveAnAbstractClassAnalyseIssue()
        {
            const string test = @" 
                    abstract class Foo
                          { 
                                void Buzz(); 
                           } ";

            var expected = new DiagnosticResult
            {
                Id = DiagnosticId.ConvertAbstractToInterface.ToDiagnosticId(),
                Message = string.Format(ConvertAbstractToInterfaceAnalyzer.MessageFormat, "Abstract"),
                Severity = DiagnosticSeverity.Hidden,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 2, 36) }
            };

            await VerifyCSharpDiagnosticAsync(test, expected);
        }

        [Fact]
        public async Task IfHaveAnAbstractClassProviderIssue()
        {
            const string result = @" 
                    abstract class Foo
                          { 
                                void Buzz(); 
                           } ";

            //const string expectedInterface = @"
            //            interface Foo
            //               { 
            //                    void Buzz();
            //                }";
            
            var expected = new DiagnosticResult
            {
                Id = DiagnosticId.ConvertAbstractToInterface.ToDiagnosticId(),
                Message = string.Format(ConvertAbstractToInterfaceAnalyzer.MessageFormat, "Abstract"),
                Severity = DiagnosticSeverity.Hidden,
                Locations = new[] { new DiagnosticResultLocation("Test0.cs", 2, 36) }
            };

            await VerifyCSharpDiagnosticAsync(result, expected);
        }
    }
}
