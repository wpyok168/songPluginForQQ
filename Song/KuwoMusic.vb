Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization
Imports HtmlAgilityPack

Public Class KuwoMusic

    Public Shared TopDic As New Dictionary(Of String, Object) From
        {
            {"酷我飙升榜", "93"},
            {"酷我新歌榜", "17"},
            {"酷我热歌榜", "16"},
            {"抖音热歌榜", "15"},
            {"会员畅听榜", "14"}
        }
    Public Shared Function GetKuwoTopList(bangid As String) As List(Of String)
        Dim url = "http://www.kuwo.cn/api/www/bang/bang/musicList?bangId=" + bangid + "&pn=1&rn=30&httpsStatus=1&reqId=02393bc0-1be8-11eb-9cd1-cd734b4ef121"
        Dim list As New List(Of String)
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        mycookiecontainer.Add(New Cookie("_ga", "GA1.2.1809082709.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gid", "GA1.2.1028802585.1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lvt_cdb524f42f0ce19b169a8071123a4797", "1602727278") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("_gat", "1") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("Hm_lpvt_cdb524f42f0ce19b169a8071123a4797", "1602728441") With {.Domain = "www.kuwo.cn"})
        mycookiecontainer.Add(New Cookie("kw_token", "EHOKWVXE1M") With {.Domain = "www.kuwo.cn"})
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"csrf", "EHOKWVXE1M"},
        {"Cache-Control", "max-age=0"},
        {"Accept-Encoding", "identity;q=1, *;q=0"},
        {"Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8"}
        }
        Dim Headerdics As New Dictionary(Of String, Object) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Host", "www.kuwo.cn"},
            {"Referer", "http://www.kuwo.cn/rankList"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }
        Dim res = RequestGet(url, Headerdics, head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Dim json As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(res)
            Dim count As Integer = DirectCast(json("data")("musicList"), ArrayList).Count
            For i = 0 To count - 1
                Dim reqId As String = json("reqId").ToString.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")
                Dim curTime As String = json("curTime")
                Dim pic_url As String = json("data")("musicList")(i)("pic")
                Dim title As String = json("data")("musicList")(i)("name")
                Dim singer As String = json("data")("musicList")(i)("artist")
                Dim rid As String = json("data")("musicList")(i)("rid")
                SongsDics.Add(rid, New Tuple(Of String, String, String, String, String)(title, singer, pic_url, curTime, reqId))
                list.Add((i + 1).ToString + ". " + title + " - " + singer)
            Next
        End If
        Return list
    End Function
    Public Shared Function GetKuwoTopNameList() As List(Of String)
        Dim list As New List(Of String)
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        Dim redirect_geturl = String.Empty
        Dim head1 = New WebHeaderCollection() From
        {
        {"Cache-Control", "max-age=0"},
        {"Accept-Encoding", "identity;q=1, *;q=0"},
        {"Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8"}
        }
        Dim Headerdics As New Dictionary(Of String, Object) From
        {
            {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3"},
            {"ContentType", "application/json, text/plain, */*"},
            {"Host", "www.kuwo.cn"},
            {"UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"}
        }

        Dim res = RequestGet("http://www.kuwo.cn/bang/index", Headerdics, head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Dim doc As New HtmlAgilityPack.HtmlDocument
            doc.LoadHtml(res)
            Dim i = 0
            For Each InputNode As HtmlNode In doc.DocumentNode.SelectNodes("//*[@id='__layout']/div/div[2]/div/div[2]/div[1]/div/ul/li")
                i = i + 1
                list.Add(i.ToString + "." + InputNode.InnerText.Replace("今日更新", "").Trim)
            Next
        End If
        Return list
    End Function
    Public Shared Function GetKuwoMusicList(songname As String) As List(Of String)
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
        Dim Headerdics As New Dictionary(Of String, Object) From
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
    Public Shared Sub PlayKuwoMusic(rid As String, reqId As String, curTime As String, title As String, singer As String, pic_url As String, thisqq As Long, GroupId As String)
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
        Dim Headerdics As New Dictionary(Of String, Object) From
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
                API.ShareMusic(Pinvoke.plugin_key, thisqq, GroupId, title, singer, jumpUrl, pic_url, jumpUrl, MusicAppTypeEnum.KuWoMusic, MusicShare_Type.GroupMsg)
            Catch ex As Exception
                If Not ex.InnerException Is Nothing Then
                    Debug.Print("调用失败: " + ex.GetBaseException.Message.ToString)
                Else
                    Debug.Print("调用失败: " + ex.Message.ToString)
                End If
            End Try

        End If
    End Sub
End Class
