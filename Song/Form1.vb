Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged, RadioButton2.CheckedChanged, RadioButton3.CheckedChanged, RadioButton4.CheckedChanged
        If RadioButton1.Checked = True Then
            MusicType = 1
        ElseIf RadioButton2.Checked = True Then
            MusicType = 2
        ElseIf RadioButton3.Checked = True Then
            MusicType = 3
        ElseIf RadioButton4.Checked = True Then
            MusicType = 4
        End If
    End Sub
End Class