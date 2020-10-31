Imports System.IO
Imports System.Net
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web
Imports System.Web.Script.Serialization


Module Main
    Public QQ_Order As New Dictionary(Of Long, Long)
    Public SongsDics As New Dictionary(Of String, Tuple(Of String, String, String, String, String))
    Public SongNameList As New List(Of String)

#Region "收到私聊消息"
    Public funRecvicePrivateMsg As RecvicePrivateMsg = New RecvicePrivateMsg(AddressOf RecvicetPrivateMessage)
    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Public Delegate Function RecvicePrivateMsg(ByRef sMsg As PrivateMessageEvent) As Integer
    Public Function RecvicetPrivateMessage(ByRef sMsg As PrivateMessageEvent) As Integer
        Dim MessageRandom As New Long
        Dim MessageReq As New UInteger
        If sMsg.SenderQQ <> sMsg.ThisQQ Then

        End If
        Return 0
    End Function
#End Region

#Region "收到群聊消息"
    Public funRecviceGroupMsg As RecviceGroupMsg = New RecviceGroupMsg(AddressOf RecvicetGroupMessage)
    <UnmanagedFunctionPointer(CallingConvention.StdCall)>
    Public Delegate Function RecviceGroupMsg(ByRef sMsg As GroupMessageEvent) As Integer
    Public Function RecvicetGroupMessage(ByRef sMsg As GroupMessageEvent) As Integer
        If sMsg.SenderQQ <> sMsg.ThisQQ Then
            If QQ_Order.ContainsKey(sMsg.SenderQQ) AndAlso QQ_Order.ContainsKey(sMsg.SenderQQ) Then
                QQ_Order.Remove(sMsg.SenderQQ)
                If New Regex("^\d{1,2}$").IsMatch(sMsg.MessageContent) = True Then
                    If Not SongNameList Is Nothing Then
                        Dim szMsg = sMsg.MessageContent
                        Dim filteredValues() As String = Array.FindAll(SongNameList.ToArray, Function(s) s.StartsWith(New Regex("^\d{1,2}$").Match(szMsg).Value + "."))
                        If filteredValues.Count > 0 Then
                            Dim id = New Regex("^\d{1,2}$").Match(sMsg.MessageContent).Value
                            Dim song_title As String = filteredValues(0).Split("-")(0).ToString.Replace(id + ".", "").Trim
                            Dim song_singer As String = filteredValues(0).Split("-")(1).Trim
                            If SongsDics.Any(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer) Then
                                If MusicType = 1 Then
                                    Dim mid As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    Dim song_id As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    TencentMusic.PlayTencentMusic(song_id, mid, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 2 Then
                                    Dim FileHash = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    KugouMusic.PlayKugouMusic(FileHash, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 3 Then
                                    Dim rid As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    Dim curTime As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item4)(0)
                                    Dim reqId As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    KuwoMusic.PlayKuwoMusic(rid, reqId, curTime, song_title, song_singer, pic_url, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                Else
                                    Dim songID As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    NetEasyMusic.PlayNetEasyMusic(songID, song_title, song_singer, pic_url, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                End If

                            End If
                        End If
                    End If
                Else
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "]" + vbNewLine + "输入序号不匹配.", False)
                End If
            ElseIf sMsg.MessageContent.Contains("点歌") Then
                Dim songname As String = sMsg.MessageContent.Replace("点歌", "").Trim
                If songname = "" Then Return 0
                SongNameList.Clear()
                QQ_Order.Add(sMsg.SenderQQ, sMsg.MessageGroupQQ)
                If MusicType = 1 Then
                    SongNameList = TencentMusic.GetTencentMusicList(songname)
                ElseIf MusicType = 2 Then
                    SongNameList = KugouMusic.GetKugouMusicList(songname)
                ElseIf MusicType = 3 Then
                    SongNameList = KuwoMusic.GetKuwoMusicList(songname)
                Else
                    SongNameList = NetEasyMusic.GetNetEasyMusicList(songname)
                End If
                If SongNameList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择要播放的歌曲项目ID:  " + vbNewLine + String.Join(vbNewLine, SongNameList), False)
            End If
        End If
        Return 0
    End Function






#End Region

End Module
