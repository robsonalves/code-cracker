using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Composition;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.CodeActions;
using CodeCracker.Properties;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeCracker.CSharp.Refactoring
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertAbstractToInterfaceProvider)), Shared]
    public class ConvertAbstractToInterfaceProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
                  ImmutableArray.Create(DiagnosticId.ConvertAbstractToInterface.ToDiagnosticId());

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        private static SyntaxToken ClassAbstractSyntaxToken = SyntaxFactory.Token(SyntaxKind.ClassDeclaration);

        public async sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var cancellationToken = context.CancellationToken;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            context.RegisterCodeFix(CodeAction.Create(Resources.ConvertAbstractToInterface_Title, c => Task.FromResult(context.Document), nameof(ConvertAbstractToInterfaceProvider)), diagnostic);
            
            //if (!await CanFixAsync(document, classDeclaration, cancellationToken)) return;
        }
    }
  
}

