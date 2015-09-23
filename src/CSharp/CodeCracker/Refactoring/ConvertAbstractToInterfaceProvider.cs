using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis;
using System.Composition;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeCracker.CSharp.Refactoring
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertAbstractToInterfaceProvider)), Shared]
    public class ConvertAbstractToInterfaceProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds =>
                  ImmutableArray.Create(DiagnosticId.ConvertAbstractToInterface.ToDiagnosticId());

        public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

        public async sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var diagnostic = context.Diagnostics.First();
            var document = context.Document;
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            var cancellationToken = context.CancellationToken;
            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var classDeclaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            //if (!await CanFixAsync(document, classDeclaration, cancellationToken)) return;
            //context.RegisterCodeFix(CodeAction.Create(Resources.SealMemberCodeFixProvider_Title, c => Task.FromResult(MakeSealed(context.Document, root, classDeclaration)), nameof(SealMemberCodeFixProvider)), diagnostic);
        }
    }
  
}

