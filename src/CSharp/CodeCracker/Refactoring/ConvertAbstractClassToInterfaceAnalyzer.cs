using Microsoft.CodeAnalysis.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeCracker.CSharp.Refactoring
{
    /// <summary>
    /// Baseof Study
    /// https://github.com/code-cracker/code-cracker/commit/6a35d95eee028fa945ecf43fbf82d6e2d7cc04d9
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ConvertAbstractToInterfaceAnalyzer : DiagnosticAnalyzer
    {
        internal const string Title = "Convert your abstract class to interface";
        internal const string MessageFormat = "Convert your abstract class to interface";
        internal const string Category = SupportedCategories.Refactoring;
                internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId.ConvertAbstractToInterface.ToDiagnosticId(),
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Hidden,
            isEnabledByDefault: true,
            helpLinkUri: HelpLink.ForDiagnostic(DiagnosticId.ConvertAbstractToInterface));

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
        public override void Initialize(AnalysisContext context) => context.RegisterSyntaxNodeAction(AnalyzeClass, SyntaxKind.ClassDeclaration);

        private static void AnalyzeClass(SyntaxNodeAnalysisContext context)
        {
            if (context.IsGenerated()) return;
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            //if (!classDeclaration.BaseList?.Types.Any() ?? true) return;
            //if (classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword))) return;
            var members = classDeclaration.Members;
            var diagnostic = Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }


    }
}
