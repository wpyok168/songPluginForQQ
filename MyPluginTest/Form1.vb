Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports System.Windows.Forms

Public Class Form1
    Friend Shared MyInstance As Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MyInstance = Me

    End Sub


End Class