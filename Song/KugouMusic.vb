Imports System.Net
Imports System.Web
Imports System.Web.Script.Serialization

Public Class KugouMusic

    Public Shared Function GetKugouMusicList(songname As String) As List(Of String)
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
    Public Shared Sub PlayKugouMusic(FileHash As String, thisqq As Long, GroupId As String)
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
                API.ShareMusic(Pinvoke.plugin_key, thisqq, GroupId, title, singer, jumpUrl, pic_url, song_url, MusicAppTypeEnum.KuGouMusic, MusicShare_Type.GroupMsg)
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
