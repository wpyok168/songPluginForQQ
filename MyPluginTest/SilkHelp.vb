Imports System.IO

Public Class SilkHelp
    ''' <summary>
    ''' Silk解码
    ''' </summary>
    ''' <param name="audio_path"></param>
    ''' <returns></returns>
    Public Function SilkDecoding(ByVal audio_path As String) As Byte()
        If Not File.Exists(audio_path) Then
            Return Nothing
        End If
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim ffmpge As String = rootdicpath & "\main\corn\ffmpeg.exe"
        Dim silkdecode As String = rootdicpath & "\main\corn\silkdecode.exe" '解码
        Dim silkencode As String = rootdicpath & "\main\corn\silkencode.exe" '编码
        If File.Exists(ffmpge) AndAlso File.Exists(silkdecode) AndAlso File.Exists(silkencode) Then
            Dim name As String = audio_path.Substring(audio_path.LastIndexOf("\") + 1)
            Dim audioslik As String = rootdicpath & "\main\data\voice"
            If Not Directory.Exists(audioslik) Then
                Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
            End If
            Dim tempname As String = audioslik & "\" & name.Substring(0, name.LastIndexOf("."))
            Dim arg As String = $" -i ""{name}"" ""{tempname}.mp3"""
            'ffmpeg -i "name.silk" "name1.mp3"
            Try
                Runcmd(ffmpge, arg)
                Return GetByte($"{tempname}.mp3")
            Catch e1 As Exception
                'silkdecode "name.silk" "name2.pcm"
                arg = $"""{audio_path}"" ""{tempname}.pcm"""
                Runcmd(silkdecode, arg)
                'ffmpeg -f s16le -ar 24000 -ac 1 -i "name2.pcm" "name2.mp3"
                arg = $" -f s16le -ar 24000 -ac 1 -i ""{tempname}.pcm"" ""{tempname}.mp3"""
                Runcmd(ffmpge, arg)
                Return GetByte($"{tempname}.mp3")
            End Try
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Silk编码
    ''' </summary>
    ''' <param name="audio_path"></param>
    ''' <returns></returns>
    Public Function SilkEncoding(ByVal audio_path As String) As Byte()
        If Not File.Exists(audio_path) Then
            Return Nothing
        End If
        Dim rootdicpath As String = Environment.CurrentDirectory
        Dim ffmpge As String = rootdicpath & "\main\corn\ffmpeg.exe"
        Dim silkdecode As String = rootdicpath & "\main\corn\silkdecode.exe" '解码
        Dim silkencode As String = rootdicpath & "\main\corn\silkencode.exe" '编码
        If File.Exists(ffmpge) AndAlso File.Exists(silkdecode) AndAlso File.Exists(silkencode) Then
            Dim name As String = audio_path.Substring(audio_path.LastIndexOf("\") + 1)
            Dim audioslik As String = rootdicpath & "\main\data\voice"
            If Not Directory.Exists(audioslik) Then
                Dim dic As DirectoryInfo = Directory.CreateDirectory(audioslik)
            End If
            Dim tempname As String = audioslik & "\" & name.Substring(0, name.LastIndexOf("."))
            Dim arg As String = $"-y -i ""{name}"" -f s16le -ar 24000 -ac 1 ""{tempname}.pcm"""
            'ffmpeg -y -i "1.mp3" -f s16le -ar 24000 -ac 1 "name.pcm"
            Runcmd(ffmpge, arg)
            'silkencode "name.pcm" "name.silk" -tencent
            arg = $"""{tempname}.pcm"" ""{tempname}.silk"" -tencent"
            Runcmd(silkencode, arg)
            Return GetByte($"{tempname}.silk")
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' 将文件转换为byte[]
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <returns></returns>
    Private Shared Function GetByte(ByVal filepath As String) As Byte()
        Using fs As New FileStream(filepath, FileMode.Open, FileAccess.Read)
            Dim by(fs.Length - 1) As Byte
            fs.Read(by, 0, CInt(fs.Length))
            fs.Close()
            Return by
        End Using
    End Function
    ''' <summary>
    ''' 调用外部程序
    ''' </summary>
    ''' <param name="filename"></param>
    ''' <param name="arg"></param>
    Private Sub Runcmd(ByVal filename As String, ByVal arg As String)
        Dim p As New ProcessStartInfo()
        p.FileName = filename
        p.Arguments = arg
        p.UseShellExecute = False
        p.CreateNoWindow = True
        Process.Start(p)
    End Sub
End Class

