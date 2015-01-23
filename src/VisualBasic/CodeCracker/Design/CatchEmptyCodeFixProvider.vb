﻿Imports System.Collections.Immutable
Imports System.Threading
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Formatting
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

<ExportCodeFixProviderAttribute("CodeCrackerCatchEmptyCodeFixProvider", LanguageNames.CSharp)>
Public Class CatchEmptyCodeFixProvider
    Inherits Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider

    Public Overrides Async Function ComputeFixesAsync(context As CodeFixContext) As Task
        Dim root = Await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(False)
        Dim diag = context.Diagnostics.First()
        Dim diagSpan = diag.Location.SourceSpan
        Dim declaration = root.FindToken(diagSpan.Start).Parent.AncestorsAndSelf.OfType(Of CatchBlockSyntax).First()
        context.RegisterFix(CodeAction.Create("Add an Exception class", Function(c) MakeCatchEmptyAsync(context.Document, declaration, c)), diag)

    End Function

    Public Overrides Function GetFixableDiagnosticIds() As ImmutableArray(Of String)
        Return ImmutableArray.Create(DesignDiagnostics.CatchEmptyAnalyerId)
    End Function

    Public Overrides Function GetFixAllProvider() As FixAllProvider
        Return WellKnownFixAllProviders.BatchFixer
    End Function

    Private Async Function MakeCatchEmptyAsync(document As Document, catchStatement As CatchBlockSyntax, cancellationtoken As CancellationToken) As Task(Of Document)
        Dim semanticModel = Await document.GetSemanticModelAsync(cancellationtoken)

        Dim newCatch = SyntaxFactory.CatchBlock(
            SyntaxFactory.CatchStatement(
                SyntaxFactory.IdentifierName("ex"),
                SyntaxFactory.SimpleAsClause(SyntaxFactory.IdentifierName("Exception")),
                Nothing)).
                WithStatements(catchStatement.Statements).
                WithLeadingTrivia(catchStatement.GetLeadingTrivia).
                WithTrailingTrivia(catchStatement.GetTrailingTrivia).
                WithAdditionalAnnotations(Formatter.Annotation)

        Dim root = Await document.GetSyntaxRootAsync()
        Dim newRoot = root.ReplaceNode(catchStatement, newCatch)
        Dim newDoc = document.WithSyntaxRoot(newRoot)
        Return newDoc
    End Function
End Class
