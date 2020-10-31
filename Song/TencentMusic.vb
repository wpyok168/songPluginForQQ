﻿Imports System.Net
Imports System.Web.Script.Serialization

Public Class TencentMusic
    Public Shared Function GetTencentMusicList(songname As String) As List(Of String)
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
    Public Shared Sub PlayTencentMusic(id As String, mid As String, thisqq As Long, GroupId As Long)
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
                API.ShareMusic(Pinvoke.plugin_key, thisqq, GroupId, title, singer, jumpUrl, pic_url, song_url, MusicAppTypeEnum.QQMusic, MusicShare_Type.GroupMsg)
            Catch ex As Exception
            End Try
        End If
    End Sub
End Class
