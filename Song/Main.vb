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
                                    PlayTencentMusic(song_id, mid, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 2 Then
                                    Dim FileHash = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    PlayKugouMusic(FileHash, sMsg.MessageGroupQQ)
                                ElseIf MusicType = 3 Then
                                    Dim rid As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    Dim curTime As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item4)(0)
                                    Dim reqId As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item5)(0)
                                    PlayKuwoMusic(rid, reqId, curTime, song_title, song_singer, pic_url, sMsg.MessageGroupQQ)
                                Else
                                    Dim songID As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Key)(0)
                                    Dim pic_url As String = SongsDics.Where(Function(x) x.Value.Item1 = song_title And x.Value.Item2 = song_singer).Select(Function(y) y.Value.Item3)(0)
                                    PlayNetEasyMusic(songID, song_title, song_singer, pic_url, sMsg.MessageGroupQQ)
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
                    SongNameList = GetTencentMusicList(songname)
                ElseIf MusicType = 2 Then
                    SongNameList = GetKugouMusicList(songname)
                ElseIf MusicType = 3 Then
                    SongNameList = GetKuwoMusicList(songname)
                Else
                    SongNameList = GetNetEasyMusicList(songname)
                End If
                If SongNameList.Count > 0 Then API.SendGroupMsg(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, "[@" + sMsg.SenderQQ.ToString + "] 请选择要播放的歌曲项目ID:  " + vbNewLine + String.Join(vbNewLine, SongNameList), False)
            End If
        End If
        Return 0
    End Function

    Public Function GetTencentMusicList(songname As String) As List(Of String)
        Dim SongList As New List(Of String)
        If songname <> "" Then
            Dim mycookiecontainer As CookieContainer = New CookieContainer()
            Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
            Dim redirect_geturl = String.Empty
            Dim head1 = New WebHeaderCollection() From
        {
        {"Cache-Control", "max-age=0"},
        {"If-None-Match", "W/'f3-57Hp5XfYYspy0YS6zeqMLrFfTC0'"},
        {"Sec-Fetch-Mode", "navigate"},
        {"Sec-Fetch-Site", "none"},
        {"Sec-Fetch-User", "?1"},
        {"Upgrade-Insecure-Requests:1"}
        }

            Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json;charset=UTF-8"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

            Dim url = "https://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.song&searchid=66137917959202681&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=10&w=” + songname + “&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0"
            Dim res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
            If res <> "" Then
                Try
                    SongsDics.Clear()
                    Dim json As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(res)
                    Dim count As Integer = DirectCast(json("data")("song")("list"), ArrayList).Count
                    For i = 0 To count - 1
                        Dim id As String = json("data")("song")("list")(i)("id")
                        Dim mid As String = json("data")("song")("list")(i)("mid")
                        Dim pmid As String = json("data")("song")("list")(i)("album")("pmid")
                        Dim title As String = json("data")("song")("list")(i)("title")
                        Dim singer As String = json("data")("song")("list")(i)("singer")(0)("name")
                        Dim jumpUrl As String = "https://y.qq.com/n/yqq/song/" + mid + ".html"
                        Dim pic_url As String = "http://y.gtimg.cn/music/photo_new/T002R300x300M000" + pmid + ".jpg?max_age=2592000"
                        SongsDics.Add(id, New Tuple(Of String, String, String, String, String)(title, singer, pic_url, jumpUrl, mid))
                        SongList.Add((i + 1).ToString + ". " + title + " - " + singer)
                        If i > 10 Then Exit For
                    Next
                Catch ex As Exception
                    If Not ex.InnerException Is Nothing Then
                        Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                    Else
                        Debug.Print("调用失败: " + ex.Message.ToString)
                    End If
                End Try
            End If
        End If
        Return SongList
    End Function
    Public Sub PlayTencentMusic(id As String, mid As String, GroupId As Long)
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"Cache-Control", "max-age=0"},
        {"If-None-Match", "W/'f3-57Hp5XfYYspy0YS6zeqMLrFfTC0'"},
        {"Sec-Fetch-Mode", "navigate"},
        {"Sec-Fetch-Site", "none"},
        {"Sec-Fetch-User", "?1"},
        {"Upgrade-Insecure-Requests:1"}
        }
        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json;charset=UTF-8"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }
        Dim url = "https://api.qq.jsososo.com/song/url?id=" + mid
        Dim Res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If Res <> "" Then
            Try
                Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(Res)
                Dim song_url As String = jsonstring("data")
                Dim title As String = SongsDics(id).Item1
                Dim singer As String = SongsDics(id).Item2
                Dim pic_url As String = SongsDics(id).Item3
                Dim jumpUrl As String = SongsDics(id).Item4
                SongsDics.Remove(id)
                API.ShareMusic(Pinvoke.plugin_key, RobotQQ, GroupId, title, singer, jumpUrl, pic_url, song_url, 0, 1)
            Catch ex As Exception
            End Try
        End If
    End Sub
    Public Function GetKugouMusicList(songname As String) As List(Of String)
        Dim SongList As New List(Of String)
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        mycookiecontainer.Add(New Cookie("kg_mid", "37e7d0becb4297c1a1bb8d59a956fa46") With {.Domain = "mobilecdn.kugou.com"})
        mycookiecontainer.Add(New Cookie("Hm_lpvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "mobilecdn.kugou.com"})
        mycookiecontainer.Add(New Cookie("Hm_lvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "mobilecdn.kugou.com"})
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"Pragma", "no-cache"},
        {"Cache-Control", "no-cache"},
        {"Sec-Fetch-User", "?1"},
        {"Upgrade-Insecure-Requests:1"},
        {"Accept-Language: en-US, en;q=0.9, zh - CN;q=0.8, zh;q=0.7"}
        }

        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json;charset=UTF-8"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

        Dim url = "http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword=" + UCase(HttpUtility.UrlEncode(songname)) + "&page=1&pagesize=20&showtype=1"
        Dim res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Try
                SongsDics.Clear()
                Dim jsonstring As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(res)
                Dim count As Integer = DirectCast(jsonstring("data")("info"), ArrayList).Count
                For i = 0 To count - 1
                    Dim AlbumID = jsonstring("data")("info")(i)("album_id")
                    Dim FileHash = jsonstring("data")("info")(i)("hash")
                    Dim title As String = jsonstring("data")("info")(i)("songname")
                    Dim singer As String = jsonstring("data")("info")(i)("singername")
                    SongsDics.Add(AlbumID, New Tuple(Of String, String, String, String, String)(title, singer, "", "", FileHash))
                    SongList.Add((i + 1).ToString + ". " + title + " - " + singer)
                    If i > 10 Then Exit For
                Next
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try
        End If
        Return SongList
    End Function
    Public Sub PlayKugouMusic(FileHash As String, GroupId As String)
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"Pragma", "no-cache"},
        {"Cache-Control", "no-cache"},
        {"Sec-Fetch-User", "?1"},
        {"Upgrade-Insecure-Requests:1"},
        {"Accept-Language: en-US, en;q=0.9, zh - CN;q=0.8, zh;q=0.7"}
        }
        Dim mycookiecontainer1 As CookieContainer = New CookieContainer()
        mycookiecontainer1.Add(New Cookie("kg_mid", "37e7d0becb4297c1a1bb8d59a956fa46") With {.Domain = "www.kugou.com"})
        mycookiecontainer1.Add(New Cookie("Hm_lpvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "www.kugou.com"})
        mycookiecontainer1.Add(New Cookie("Hm_lvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "www.kugou.com"})
        mycookiecontainer1.Add(New Cookie("KuGooRandom", "6671602680303344") With {.Domain = "www.kugou.com"})

        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json;charset=UTF-8"},
            {"Referer", "www.kugou.com"},
            {"Host", "www.kugou.com"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

        Dim url = "http://www.kugou.com/yy/index.php?r=play/getdata&hash=" + FileHash
        Dim Res = RequestGet(url, Headerdics, head1, mycookiecontainer1, redirect_geturl)
        If Res <> "" Then
            Try
                Dim json = New JavaScriptSerializer().DeserializeObject(Res)
                Dim pic_url As String = json("data")("img")
                Dim title As String = json("data")("song_name")
                Dim singer As String = json("data")("author_name")
                Dim jumpUrl As String = json("data")("play_url")
                Dim song_url As String = json("data")("play_backup_url")
                API.ShareMusic(Pinvoke.plugin_key, RobotQQ, GroupId, title, singer, jumpUrl, pic_url, song_url, 0, 1)
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try
        End If
    End Sub
    Public Function GetKuwoMusicList(songname As String) As List(Of String)
        Dim SongList As New List(Of String)
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        mycookiecontainer.Add(New Cookie("_ga", "GA1.2.1809082709.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gid", "GA1.2.1028802585.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lvt_cdb524f42f0ce19b169a8071123a4797", "1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gat", "1") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lpvt_cdb524f42f0ce19b169a8071123a4797", "1602728441") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("kw_token", "EHOKWVXE1M") With {.Domain = "www.kuwo.cn"})
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"csrf: EHOKWVXE1M"},
        {"If-None-Match: '23975-vAMhe83OnI8ehkAXUyjU+kLga2g'"},
        {"Accept-Language: th,en;q=0.9,zh-CN;q=0.8,zh;q=0.7,zh-TW;q=0.6,en-US;q=0.5"}
        }
        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Referer", "http://www.kuwo.cn/search/list?key=" + UCase(HttpUtility.UrlEncode(songname))},
            {"Host", "www.kuwo.cn"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

        Dim url = "http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key=" + UCase(HttpUtility.UrlEncode(songname)) + "&pn=1&rn=10&httpsStatus=1&reqId=06427b90-0e8d-11eb-a5da-975a5becfc1e"
        Dim res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Try
                SongsDics.Clear()
                Dim json As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(res)
                Dim count As Integer = DirectCast(json("data")("list"), ArrayList).Count
                For i = 0 To count - 1
                    Dim reqId As String = json("reqId").ToString.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")
                    Dim curTime As String = json("curTime")
                    Dim pic_url As String = json("data")("list")(i)("pic")
                    Dim title As String = json("data")("list")(i)("name")
                    Dim singer As String = json("data")("list")(i)("artist")
                    Dim rid As String = json("data")("list")(i)("rid")
                    SongsDics.Add(rid, New Tuple(Of String, String, String, String, String)(title, singer, pic_url, curTime, reqId))
                    SongList.Add((i + 1).ToString + ". " + title + " - " + singer)
                    If i > 10 Then Exit For
                Next
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try
        End If
        Return SongList
    End Function
    Public Sub PlayKuwoMusic(rid As String, reqId As String, curTime As String, title As String, singer As String, pic_url As String, GroupId As String)
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        mycookiecontainer.Add(New Cookie("_ga", "GA1.2.1809082709.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gid", "GA1.2.1028802585.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lvt_cdb524f42f0ce19b169a8071123a4797", "1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gat", "1") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lpvt_cdb524f42f0ce19b169a8071123a4797", "1602728441") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("kw_token", "EHOKWVXE1M") With {.Domain = "www.kuwo.cn"})
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"csrf: EHOKWVXE1M"},
        {"If-None-Match: '23975-vAMhe83OnI8ehkAXUyjU+kLga2g'"},
        {"Accept-Language: th,en;q=0.9,zh-CN;q=0.8,zh;q=0.7,zh-TW;q=0.6,en-US;q=0.5"}
        }
        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Referer", "http://www.kuwo.cn/search/list?key=" + UCase(HttpUtility.UrlEncode(title))},
            {"Host", "www.kuwo.cn"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

        Dim url = "http://www.kuwo.cn/url?format=mp3&rid=" + rid + "&response=url&type=convert_url3&br=128kmp3&from=web&t=" + curTime + "&httpsStatus=1&reqId=" + reqId
        Dim Res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If Res <> "" Then
            Try
                Dim jsonstring = New JavaScriptSerializer().DeserializeObject(Res)
                Dim jumpUrl As String = jsonstring("url")
                API.ShareMusic(Pinvoke.plugin_key, RobotQQ, GroupId, title, singer, jumpUrl, pic_url, jumpUrl, 0, 1)
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try

        End If
    End Sub
    Public Function GetNetEasyMusicList(SongName As String) As List(Of String)
        Dim SongList As New List(Of String)
        Dim res As String = ""
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim head1 As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Referer", "http://music.163.com/search/"},
            {"Host", "music.163.com"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }
        Dim url = "http://music.163.com/api/search/get?csrf_token=hlpretag=&hlposttag=&s={" + SongName + "}&type=1&offset=0&total=true&limit=20"
        res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Try
                SongsDics.Clear()
                Dim json As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(res)
                Dim count As Integer = DirectCast(json("result")("songs"), ArrayList).Count
                For i = 0 To count - 1
                    Dim songID As String = json("result")("songs")(i)("id")
                    Dim pic_url As String = json("result")("songs")(i)("album")("artist")("img1v1Url")
                    Dim title As String = json("result")("songs")(i)("name")
                    Dim singer As String = json("result")("songs")(i)("artists")(0)("name")
                    SongsDics.Add(songID, New Tuple(Of String, String, String, String, String)(title, singer, pic_url, "", ""))
                    SongList.Add((i + 1).ToString + ". " + title + " - " + singer)
                    If i > 10 Then Exit For
                Next
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try

        End If
        Return SongList
    End Function
    Public Sub PlayNetEasyMusic(songID As String, title As String, singer As String, pic_url As String, GroupId As String)
        Try
            Dim data As String = "{""ids"":""[" + songID + "]"",""br"":128000,""csrf_token"":""""}"
            Dim retDictionary As List(Of String) = EncrytData(data)
            Dim postdata = String.Join("&", retDictionary)
            Dim url = "http://music.163.com/api/song/enhance/player/url?id=" + songID + "&ids=" + "%5B" + songID + "%5D" + "&br=3200000&"
            Dim redirect_geturl = String.Empty
            Dim head1 As WebHeaderCollection = New WebHeaderCollection()
            Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
            Dim mycookiecontainer As CookieContainer = New CookieContainer()
            'mycookiecontainer.Add(New Cookie("_ntes_nnid", "3ddf2945dfda076e60b4ee0162f3c0cd,1488291659575; _ntes_nuid=3ddf2945dfda076e60b4ee0162f3c0cd; vjuids=76bd0b1bc.15acc1b713d.0.73360b94f7956; vjlast=1489483035.1489483035.30; vinfo_n_f_l_n3=e441d5bafac7c9b7.1.0.1489483034993.0.1489483124982; JSESSIONID-WYYY=m%2FO4x3rjCh4e1xfBjmOCZ52hbV7rD7M9U77ZX9qn%2BMw5WeKyU0vnw1zGmpbn%5Ci5ZWmdaRmVjQromGw%2BxForePwG3mBf6jOy27vj1IMOv%5ClM3%2BXkUrPOeM7qPP9HhgO%2F%2Fd%5C1nWUq3mDUIicmtDvWMsUbEeWS%5CbNAJyUO8t%5CbHCy731FHp%3A1489661205597; _iuqxldmzr_=32; __utma=94650624.1301580310.1488291660.1489644772.1489659406.9; __utmb=94650624.4.10.1489659406; __utmc=94650624; __utmz=94650624.1489581581.6.4.utmcsr=baidu|utmccn=(organic)|utmcmd=organic") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("_ntes_nuid", "3ddf2945dfda076e60b4ee0162f3c0cd") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("vjuids", "76bd0b1bc.15acc1b713d.0.73360b94f7956") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("vjlast", "1489483035.1489483035.30") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("vinfo_n_f_l_n3", "e441d5bafac7c9b7.1.0.1489483034993.0.1489483124982") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("JSESSIONID-WYYY", "m%2FO4x3rjCh4e1xfBjmOCZ52hbV7rD7M9U77ZX9qn%2BMw5WeKyU0vnw1zGmpbn%5Ci5ZWmdaRmVjQromGw%2BxForePwG3mBf6jOy27vj1IMOv%5ClM3%2BXkUrPOeM7qPP9HhgO%2F%2Fd") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("_iuqxldmzr_", "32") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("__utma", "94650624.1301580310.1488291660.1489644772.1489659406.9") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("__utmb", "94650624.4.10.1489659406") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("__utmc", "94650624") With {.Domain = "music.163.com"})
            mycookiecontainer.Add(New Cookie("__utmz", "94650624.1489581581.6.4.utmcsr=baidu|utmccn=(organic)|utmcmd=organic") With {.Domain = "music.163.com"})

            Dim Headerdics As New Dictionary(Of String, String) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Referer", "http://music.163.com/search/"},
            {"Host", "music.163.com"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

            Dim Res = RequestPost(url, Headerdics, head1, postdata, mycookiecontainer, myWebHeaderCollection, redirect_geturl)
            'Res = RequestGet(url, "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, mycookiecontainer, redirect_geturl)
            If Res <> "" Then
                Try
                    Dim json = New JavaScriptSerializer().DeserializeObject(Res)
                    Dim jumpUrl As String = json("data")(0)("url")
                    If jumpUrl Is Nothing Or jumpUrl = "" Then
                        API.SendGroupMsg(Pinvoke.plugin_key, RobotQQ, GroupId, "不支持该歌曲播放", False)
                    Else
                        API.ShareMusic(Pinvoke.plugin_key, RobotQQ, GroupId, title, singer, jumpUrl, pic_url, jumpUrl, 0, 1)
                    End If
                Catch ex As Exception
                    If Not ex.InnerException Is Nothing Then
                        Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                    Else
                        Debug.Print("调用失败: " + ex.Message.ToString)
                    End If
                End Try
            End If
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
            Else
                Debug.Print("调用失败: " + ex.Message.ToString)
            End If
        End Try

    End Sub
    Private Function AESEncode(ByVal secretData As String, Optional ByVal secret As String = "0CoJUm6Qyw8W8jud") As String
        Dim encrypted() As Byte
        Dim IV() As Byte = Encoding.UTF8.GetBytes("0102030405060708")
        Using aesen = Aes.Create()
            aesen.Key = Encoding.UTF8.GetBytes(secret)
            aesen.IV = IV
            aesen.Mode = CipherMode.CBC
            Using encryptor = aesen.CreateEncryptor()
                Using stream = New MemoryStream()
                    Using cstream = New CryptoStream(stream, encryptor, CryptoStreamMode.Write)
                        Using sw = New StreamWriter(cstream)
                            sw.Write(secretData)
                        End Using
                        encrypted = stream.ToArray()
                    End Using
                End Using
            End Using
        End Using
        Return Convert.ToBase64String(encrypted)
    End Function
    Public Function EncrytData(encrypt As String) As List(Of String)
        Dim retValue As New List(Of String)
        Dim secKey As String = CreateRandomAlphanumericString(16)
        Dim encText As String = AESEncode(encrypt, "0CoJUm6Qyw8W8jud")
        encText = AESEncode(encText, secKey)
        Dim params As String = "params=" + HttpUtility.UrlEncode(encText)
        retValue.Add(params)
        Dim encSecKey = "encSecKey=" + RSAEncode(secKey)
        retValue.Add(encSecKey)
        Return retValue
    End Function
    Private Function RSAEncode(ByVal text As String) As String
        Dim srtext As New String(text.Reverse().ToArray())

        Dim a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", ""))
        Dim b = BCHexDec("010001")
        Dim c = BCHexDec("00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7")
        Dim key = BigInteger.ModPow(a, b, c).ToString("x")
        key = key.PadLeft(256, "0"c)
        If key.Length > 256 Then
            Return key.Substring(key.Length - 256, 256)
        Else
            Return key
        End If
    End Function
    Private Function BCHexDec(ByVal hex As String) As BigInteger
        Dim dec As New BigInteger(0)
        Dim len As Integer = hex.Length
        For i As Integer = 0 To len - 1
            dec += BigInteger.Multiply(New BigInteger(Convert.ToInt32(hex.Chars(i).ToString(), 16)), BigInteger.Pow(New BigInteger(16), len - i - 1))
        Next i
        Return dec
    End Function
    Public Function CreateRandomAlphanumericString(size As Integer) As String
        Dim allowedChars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray()
        Dim bytes = New Byte(0) {}
        Dim crypto = New RNGCryptoServiceProvider()
        crypto.GetNonZeroBytes(bytes)
        bytes = New Byte(size - 1) {}
        crypto.GetNonZeroBytes(bytes)
        Dim retVal = New StringBuilder(size)
        For Each b As Byte In bytes
            retVal.Append(allowedChars(b Mod (allowedChars.Length)))
        Next
        Return retVal.ToString()
    End Function


#End Region

End Module
