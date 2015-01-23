﻿Imports System.Runtime.CompilerServices
Imports CodeCracker.Test.TestHelper
Imports Microsoft.CodeAnalysis
Imports Xunit

Public Class MakeLocalVariablesConstWhenItIsPossibleTests
    Inherits CodeFixTest(Of MakeLocalVariableConstWhenPossibleAnalyzer, MakeLocalVariableConstWhenPossibleCodeFixProvider)

    <Fact>
    Public Async Function IgnoresConstantDeclarations() As Task
        Dim test = "const a as Integer = 10".WrapInMethod
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function

    <Fact>
    Public Async Function IgnoresDeclarationsWithNoInitializers() As Task
        Dim test = "dim a as Integer".WrapInMethod
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function
    <Fact>
    Public Async Function IgnoresDeclarationsWithNonConstants() As Task
        Dim test = "Dim a as Integer = GetValue()".WrapInMethod
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function
    <Fact>
    Public Async Function IgnoresDeclarationsWithReferenceTypes() As Task
        Dim test = "Dim a as New Foo()".WrapInMethod
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function

    ' Interpolated strings are valid as consts in VB
    '<Fact>
    'Public Async Function IgnoresStringInterpolations() As Task
    'End Function

    <Fact>
    Public Async Function IgnoresVariablesThatChangesValueOutsideDeclarations() As Task
        Dim test = "Dim a as Integer = 10 : a = 20".WrapInMethod
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function
    <Fact>
    Public Async Function CreateDiagnosticsWhenAssigningAPotentialConstant() As Task
        Dim test = "Dim a As Integer = 10".WrapInMethod()
        Dim expected = New DiagnosticResult With
        {
            .Id = PerformanceDiagnostics.MakeLocalVariableConstWhenPossibleId,
            .Message = "This variable can be made const.",
            .Severity = DiagnosticSeverity.Info,
            .Locations = {New DiagnosticResultLocation("Test0.vb", 6, 13)}
        }
        Await VerifyBasicDiagnosticsAsync(test, expected)
    End Function

    <Fact>
    Public Async Function CreateDiagnosticsWhenAssigningAPotentialConstantUsingTypeInference() As Task
        Dim test = "Dim a = 10".WrapInMethod()
        Dim expected = New DiagnosticResult With
        {
            .Id = PerformanceDiagnostics.MakeLocalVariableConstWhenPossibleId,
            .Message = "This variable can be made const.",
            .Severity = DiagnosticSeverity.Info,
            .Locations = {New DiagnosticResultLocation("Test0.vb", 6, 13)}
        }
        Await VerifyBasicDiagnosticsAsync(test, expected)
    End Function
    <Fact>
    Public Async Function CreateDiagnosticsWhenAssigningNothingToAReferenceType() As Task
        Dim test = "Dim a As Foo = Nothing".WrapInMethod()
        Dim expected = New DiagnosticResult With
        {
            .Id = PerformanceDiagnostics.MakeLocalVariableConstWhenPossibleId,
            .Message = "This variable can be made const.",
            .Severity = DiagnosticSeverity.Info,
            .Locations = {New DiagnosticResultLocation("Test0.vb", 6, 13)}
        }
        Await VerifyBasicDiagnosticsAsync(test, expected)
    End Function

    <Fact>
    Public Async Function IgnoresNullableVariables() As Task
        Dim test = "Dim a As Integer? = 1".WrapInMethod()
        Await VerifyBasicHasNoDiagnosticsAsync(test)
    End Function

    <Fact>
    Public Async Function FixMakesAVariableConstWhenDeclarationSpecifiesTypeName() As Task
        Dim test = "Dim a As Integer = 10".WrapInMethod()
        Dim expected = "Const a As Integer = 10".WrapInMethod()
        Await VerifyBasicFixAsync(test, expected)
    End Function

    <Fact>
    Public Async Function FixMakesAVariableConstWhenDeclarationInfersType() As Task
        Dim test = "Dim a = 10".WrapInMethod()
        Dim expected = "Const a = 10".WrapInMethod()
        Await VerifyBasicFixAsync(test, expected)
    End Function
    <Fact>
    Public Async Function FixMakesAVariableConstWhenDeclarationInfersString() As Task
        Dim test = "Dim a = """"".WrapInMethod()
        Dim expected = "Const a = """"".WrapInMethod()
        Await VerifyBasicFixAsync(test, expected)
    End Function

    <Fact>
    Public Async Function FixMakesAVariableConstWhenSettingNullToAReferenceType() As Task
        Dim test = "Dim a As Foo = Nothing".WrapInMethod()
        Dim expected = "Const a As Foo = Nothing".WrapInMethod()
        Await VerifyBasicFixAsync(test, expected)
    End Function
    <Fact>
    Public Async Function FixMakesAVariableConstWhenInferingType() As Task
        Dim test = "Dim a = 10".WrapInMethod()
        Dim expected = "Const a = 10".WrapInMethod()
        Await VerifyBasicFixAsync(test, expected)
    End Function

    ' Test does not apply in VB. Can't make a class into a const
    '<Fact>
    'Public Sub FixMakesAVariableConstWhenInferingReferenceType()
    '    Dim test = "Dim a As New TypeName()".WrapInMethod()
    '    Dim expected = "Const a As New TypeName()".WrapInMethod()
    '    VerifyBasicFix(test, expected)
    'End Sub


End Class

Module CodeFixTestExtensions
    <Extension>
    Public Function WrapInMethod(code As String) As String
        Return "
Imports System
Namespace ConsoleApplication1
    Class TypeName
        Public Sub Foo()
            code
            ' VB Requires value to be used or another analyzer is added which breaks the tests
            Console.WriteLine(a)
        End Sub
    End Class
End Namespace".Replace("code", code)

    End Function
End Module
