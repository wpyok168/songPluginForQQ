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
    Public QQ_subOrder As New Dictionary(Of Long, Long)
    Public QQOrder As New Dictionary(Of Long, Long)
    Public SourceMusciQQOrder As New Dictionary(Of Long, Long)
    Public SongsDics As New Dictionary(Of String, Tuple(Of String, String, String, String, String))
    Public SongNameList As New List(Of String)
    Public TopList As New List(Of String)
    Public TencentTopList As List(Of TencentKeyValue)

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
            If sMsg.MessageContent = "点歌菜单" Then
                Dim szContent = "命令大全:" & vbNewLine _
                    & "【点歌+歌曲名】:搜索歌曲,可用歌曲名、歌手等信息用来搜索" & vbNewLine _
                    & "【切换音乐源】: 切换歌曲的播放源 " & vbNewLine _
                    & "【排行榜】: 根据排行榜列表点歌 " & vbNewLine
                API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, szContent, False)
            ElseIf SourceMusciQQOrder.ContainsKey(sMsg.SenderQQ) AndAlso SourceMusciQQOrder.ContainsKey(sMsg.SenderQQ) Then
                SourceMusciQQOrder.Clear()
                If sMsg.MessageContent = "1" Then
                    MusicType = 1
                ElseIf sMsg.MessageContent = "2" Then
                    MusicType = 2
                ElseIf sMsg.MessageContent = "3" Then
                    MusicType = 3
                ElseIf sMsg.MessageContent = "4" Then
                    MusicType = 4
                Else
                    Return 0
                End If
                API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "]" + vbNewLine + "切换完毕.", False)
            ElseIf QQ_subOrder.ContainsKey(sMsg.SenderQQ) AndAlso QQ_subOrder.ContainsKey(sMsg.SenderQQ) Then
                QQ_subOrder.Remove(sMsg.SenderQQ)
                If New Regex("^\d{1,2}$").IsMatch(sMsg.MessageContent) = True Then
                    If Not SongNameList Is Nothing Then
                        Dim szMsg = sMsg.MessageContent
                        Dim filteredValues() As String = Array.FindAll(SongNameList.ToArray, Function(s) s.StartsWith(New Regex("^\d{1,2}$").Match(szMsg).Value + "."))
                        If filteredValues.Count > 0 Then
                            Dim id = New Regex("^\d{1,2}$").Match(sMsg.MessageContent).Value
                        End If
                    End If
                End If
            ElseIf QQ_Order.ContainsKey(sMsg.SenderQQ) AndAlso QQ_Order.ContainsKey(sMsg.SenderQQ) Then
                QQ_Order.Remove(sMsg.SenderQQ)
                If New Regex("^\d{1,2}$").IsMatch(sMsg.MessageContent) = True Then
                    If Not SongNameList Is Nothing Then
                        Dim szMsg = sMsg.MessageContent
                        Dim id = New Regex("^\d{1,2}$").Match(sMsg.MessageContent).Value
                        Dim filteredValues() As String = Array.FindAll(SongNameList.ToArray, Function(s) s.StartsWith(id + "."))
                        If filteredValues.Count > 0 Then
                            Dim song_title As String = filteredValues(0).Split("-")(0).ToString.Replace(id + ".", "").Trim
                            Dim song_singer As String = ""
                            Try
                                song_singer = filteredValues(0).Split("-")(1).Trim()
                            Catch ex As Exception

                            End Try
                            If SongsDics.Any(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer) Then
                                If MusicType = 1 Then
                                    Dim mid As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    Dim song_id As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    TencentMusic.PlayTencentMusic(song_id, mid, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 2 Then
                                    Dim FileHash = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    If FileHash = "" Then
                                        Dim song_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item4)(0)
                                        KugouMusic.PlayKugouUrl(song_url, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                    Else
                                        KugouMusic.PlayKugouMusic(FileHash, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                    End If
                                ElseIf MusicType = 3 Then
                                    Dim rid As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    Dim curTime As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item4)(0)
                                    Dim reqId As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    KuwoMusic.PlayKuwoMusic(rid, reqId, curTime, song_title, song_singer, pic_url, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 4 Then
                                    Dim songID As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    NetEasyMusic.PlayNetEasyMusic(songID, song_title, song_singer, pic_url, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                                End If
                            End If
                        End If
                    End If
                Else
                    'API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "]" + vbNewLine + "输入序号不匹配.", False)
                End If
            ElseIf QQOrder.ContainsKey(sMsg.SenderQQ) AndAlso QQOrder.ContainsKey(sMsg.SenderQQ) Then
                QQOrder.Remove(sMsg.SenderQQ)
                If New Regex("^\d{1,2}$").IsMatch(sMsg.MessageContent) = True Then
                    Dim id = New Regex("^\d{1,2}$").Match(sMsg.MessageContent).Value
                    If MusicType = 1 Then
                        If Not TopList Is Nothing Then
                            Dim szMsg = sMsg.MessageContent
                            Dim ResultValues() As String = Array.FindAll(TopList.ToArray, Function(s) s.StartsWith(New Regex("^\d{1,2}$").Match(szMsg).Value + "."))
                            If ResultValues.Count > 0 Then
                                SongNameList.Clear()
                                Dim topname As String = ResultValues(0)
                                Dim topid As String = TencentTopList.Where(Function(x) x.Name = topname).FirstOrDefault().Id
                                SongNameList = TencentMusic.GetTopList(topid.Split("@")(0), topid.Split("@")(1))
                            End If
                        End If
                    ElseIf MusicType = 2 Then
                        If Not TopList Is Nothing Then
                            Dim szMsg = sMsg.MessageContent
                            Dim ResultValues() As String = Array.FindAll(TopList.ToArray, Function(s) s.StartsWith(New Regex("^\d{1,2}$").Match(szMsg).Value + "."))
                            If ResultValues.Count > 0 Then
                                Dim topurl = ResultValues(0).Replace(id + ".", "").Trim
                                SongNameList = KugouMusic.GetKugouTopList（topurl）
                            End If
                        End If
                    ElseIf MusicType = 3 Then
                        'If Not TopList Is Nothing Then
                        '    Dim szMsg = sMsg.MessageContent
                        '    Dim ResultValues() As String = Array.FindAll(TopList.ToArray, Function(s) s.StartsWith(New Regex("^\d{1,2}$").Match(szMsg).Value + "."))
                        '    If ResultValues.Count > 0 Then
                        '        Dim bangname As String = ResultValues(0).Replace(id + ".", "").Trim
                        '        Dim bangid As String = KuwoMusic.TopDic(bangname)
                        '        SongNameList = KuwoMusic.GetKuwoTopList(bangid)
                        '    End If
                        'End If
                        If sMsg.MessageContent = "1" Then
                            SongNameList = KuwoMusic.GetKuwoTopList("93")
                        ElseIf sMsg.MessageContent = "2" Then
                            SongNameList = KuwoMusic.GetKuwoTopList("17")
                        ElseIf sMsg.MessageContent = "3" Then
                            SongNameList = KuwoMusic.GetKuwoTopList("16")
                        ElseIf sMsg.MessageContent = "4" Then
                            SongNameList = KuwoMusic.GetKuwoTopList("15")
                        ElseIf sMsg.MessageContent = "5" Then
                            SongNameList = KuwoMusic.GetKuwoTopList("14")
                        End If
                    ElseIf MusicType = 4 Then
                        If sMsg.MessageContent = "1" Then
                            SongNameList = NetEasyMusic.GetTopMusic("19723756")
                        ElseIf sMsg.MessageContent = "2" Then
                            SongNameList = NetEasyMusic.GetTopMusic("3778678")
                        ElseIf sMsg.MessageContent = "3" Then
                            SongNameList = NetEasyMusic.GetTopMusic("2250011882")
                        ElseIf sMsg.MessageContent = "4" Then
                            SongNameList = NetEasyMusic.GetTopMusic("1978921795")
                        End If
                    End If
                    QQ_Order.Add(sMsg.SenderQQ, sMsg.MessageGroupQQ)
                    If SongNameList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择要播放的歌曲项目ID:  " + vbNewLine + String.Join(vbNewLine, SongNameList), False)

                Else
                    'API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "]" + vbNewLine + "输入序号不匹配.", False)
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
            ElseIf sMsg.MessageContent = "排行榜" Then
                QQOrder.Add(sMsg.SenderQQ, sMsg.MessageGroupQQ)
                If MusicType = 1 Then
                    TencentTopList = TencentMusic.GetTopListCategory()
                    TopList = TencentTopList.Select(Function(x) x.Name).ToList()
                    If TencentTopList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择排行榜项目:  " + vbNewLine + String.Join(vbNewLine, TopList), False)
                ElseIf MusicType = 2 Then
                    Dim retList = KugouMusic.GetKugouTopNameList()
                    If retList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择排行榜项目:  " + vbNewLine + String.Join(vbNewLine, retList), False)
                ElseIf MusicType = 3 Then
                    'TopList = KuwoMusic.GetKuwoTopNameList()
                    'If TopList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择排行榜项目:  " + vbNewLine + String.Join(vbNewLine, TopList), False)
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择排行榜项目:  " + vbNewLine + "1.酷我飙升榜" + vbNewLine + "2.酷我新歌榜" + vbNewLine + "3.酷我热歌榜" + vbNewLine + "4.抖音热歌榜" + vbNewLine + "5.会员畅听榜", False)
                ElseIf MusicType = 4 Then
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择排行榜项目:  " + vbNewLine + "1.云音乐飙升榜" + vbNewLine + "2.云音乐热歌榜" + vbNewLine + "3.抖音排行榜" + vbNewLine + "4.云音乐电音榜", False)
                End If
            ElseIf sMsg.MessageContent = "切换音乐源" Then
                SourceMusciQQOrder.Add(sMsg.SenderQQ, sMsg.MessageGroupQQ)
                    API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择音乐源:  " + vbNewLine + "1.QQ音乐" + vbNewLine + "2.酷狗音乐" + vbNewLine + "3.酷我音乐" + vbNewLine + "4.网易音乐", False)
                End If
            End If
            Return 0
    End Function

#End Region

End Module
