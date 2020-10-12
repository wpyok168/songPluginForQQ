Imports System.Net
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization

Public Class QQMusicAPI
    Public Enum MusicType
        Netease = 1
        qqMusic = 2
    End Enum
    Private Shared qQMusic As QQMusicAPI
    Public Shared Function Instance() As QQMusicAPI
        If qQMusic Is Nothing Then
            qQMusic = New QQMusicAPI()
        End If
        Return qQMusic
    End Function
    Public Shared Function GetSong() As List(Of QQMusicInfo)
        '获取热歌
        Dim url = "https://u.y.qq.com/cgi-bin/musicu.fcg?-=getradiosonglist4871281737878832&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0&data=%7B%22songlist%22%3A%7B%22module%22%3A%22pf.radiosvr%22%2C%22method%22%3A%22GetRadiosonglist%22%2C%22param%22%3A%7B%22id%22%3A199%2C%22firstplay%22%3A1%2C%22num%22%3A10%7D%7D%2C%22radiolist%22%3A%7B%22module%22%3A%22pf.radiosvr%22%2C%22method%22%3A%22GetRadiolist%22%2C%22param%22%3A%7B%22ct%22%3A%2224%22%7D%7D%2C%22comm%22%3A%7B%22ct%22%3A%2224%22%7D%7D"
        Dim client As New WebClient()
        client.Encoding = Encoding.UTF8
        Dim result = client.DownloadString(url)
        Dim obj = MusicTool.DeserializeStringToDictionary(Of Object, Object)(result)
        Dim songlist = MusicTool.DeserializeStringToDictionary(Of Object, Object)(obj("songlist").ToString())
        Dim data = MusicTool.DeserializeStringToDictionary(Of Object, Object)(songlist("data").ToString())
        Dim track_list = TryCast(data("track_list"), List(Of Object))
        Dim list As New List(Of QQMusicInfo)()
        'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
        'For Each item As Object In track_list.Children()
        '    Dim songurl = GetSongUrl(item.mid.ToString())
        '    list.Add(New QQMusicInfo With {
        '        .SongId = item.mid.ToString(),
        '        .SongName = item.title,
        '        .Songer = GetSonger(item.singer),
        '        .SongUrl = songurl
        '    })
        'Next item
        Return list
    End Function
    Public Shared Function Search(ByVal keyword As String) As List(Of QQMusicInfo)
        Dim utf8 As Encoding = Encoding.UTF8
        '首先用utf-8进行解码                    
        Dim code As String = System.Web.HttpUtility.UrlEncode(keyword, utf8)
        Dim url = $"https://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.center&searchid=37728264098360808&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=20&w={code.ToUpper()}&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0"
        Dim client As New WebClient()
        client.Encoding = Encoding.UTF8
        Dim result = client.DownloadString(url)
        Dim obj = MusicTool.DeserializeStringToDictionary(Of Object, Object)(result)
        Dim data = MusicTool.DeserializeStringToDictionary(Of Object, Object)(obj("data").ToString())
        Dim song = MusicTool.DeserializeStringToDictionary(Of Object, Object)(data("song").ToString())
        Dim songlist = TryCast(song("list"), List(Of Object))
        Dim list As New List(Of QQMusicInfo)()
        'For Each item As Object In songlist.Children()
        '    Dim songurl = GetSongUrl(item.mid.ToString())
        '    list.Add(New QQMusicInfo With {
        '        .SongId = item.mid.ToString(),
        '        .SongName = item.title,
        '        .Songer = GetSonger(item.singer),
        '        .SongUrl = songurl
        '    })
        'Next item
        Return list
    End Function
    Private Shared Function GetSongUrl(ByVal mid As String) As String
        Dim url = $"https://u.y.qq.com/cgi-bin/musicu.fcg?-=getplaysongvkey7202785251801513&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0&data=%7B%22req%22%3A%7B%22module%22%3A%22CDN.SrfCdnDispatchServer%22%2C%22method%22%3A%22GetCdnDispatch%22%2C%22param%22%3A%7B%22guid%22%3A%229756534816%22%2C%22calltype%22%3A0%2C%22userip%22%3A%22%22%7D%7D%2C%22req_0%22%3A%7B%22module%22%3A%22vkey.GetVkeyServer%22%2C%22method%22%3A%22CgiGetVkey%22%2C%22param%22%3A%7B%22guid%22%3A%226541487400%22%2C%22songmid%22%3A%5B%22{mid}%22%5D%2C%22songtype%22%3A%5B0%5D%2C%22uin%22%3A%220%22%2C%22loginflag%22%3A1%2C%22platform%22%3A%2220%22%7D%7D%2C%22comm%22%3A%7B%22uin%22%3A0%2C%22format%22%3A%22json%22%2C%22ct%22%3A20%2C%22cv%22%3A0%7D%7D"
        Dim client As New WebClient()
        client.Encoding = Encoding.UTF8
        Dim result = client.DownloadString(url)
        Dim obj = MusicTool.DeserializeStringToDictionary(Of Object, Object)(result)
        Dim songlist = MusicTool.DeserializeStringToDictionary(Of Object, Object)(obj("req").ToString())
        Dim data = MusicTool.DeserializeStringToDictionary(Of Object, Object)(songlist("data").ToString())
        Dim ips = TryCast(data("freeflowsip"), List(Of Object))
        Dim purl As String = String.Empty
        '			#Region "获取参数"
        If True Then
            Dim req_0 = MusicTool.DeserializeStringToDictionary(Of Object, Object)(obj("req_0").ToString())
            Dim req_0_data = MusicTool.DeserializeStringToDictionary(Of Object, Object)(req_0("data").ToString())
            Dim midure = TryCast(req_0_data("midurlinfo"), List(Of Object))
            If midure.Count > 0 Then
                Dim urls = MusicTool.DeserializeStringToDictionary(Of Object, Object)(midure(0).ToString())
                purl = urls("purl").ToString()
            End If
        End If
        '			#End Region
        If ips.Count > 0 AndAlso Not String.IsNullOrEmpty(purl) Then
            Return ips(0) & purl
        End If
        Return ""
    End Function
    Private Shared Function GetSonger(ByVal singer As List(Of Object)) As String
        Dim name = New StringBuilder()
        'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
        For Each item As Object In singer
            name.Append(item.name & ",")
        Next item
        Return name.ToString().TrimEnd(","c)
    End Function
    ''' <summary>
    ''' 获取排行榜歌曲
    ''' </summary>
    ''' <param name="Id"></param>
    ''' <param name="update_key"></param>
    ''' <returns></returns>
    Public Shared Function GetTopList(ByVal Id As String, ByVal update_key As String) As List(Of QQMusicInfo)
        Dim pagesize As Integer = 100
        Dim url = $"https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_cp.fcg?tpl=3&page=detail&date={update_key}&topid={Id}&type=top&song_begin=0&song_num={pagesize}&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0"
        Dim client As New WebClient()
        client.Encoding = Encoding.UTF8
        Dim result = client.DownloadString(url)
        'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
        Dim data = New JavaScriptSerializer().Deserialize(Of Object)(result)
        Dim track_list = TryCast(data.songlist, List(Of Object))
        Dim list As New List(Of QQMusicInfo)()
        'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
        'For Each item As Object In track_list.Children()
        '    Dim songurl = GetSongUrl(item.data.songmid.ToString())
        '    list.Add(New QQMusicInfo With {
        '        .SongId = item.data.songmid.ToString(),
        '        .SongName = item.data.songname,
        '        .Songer = GetSonger(item.data.singer),
        '        .SongUrl = songurl
        '    })
        'Next item
        Return list
    End Function
    ''' <summary>
    ''' 获取排行榜类别
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetTopListCategory() As List(Of TopList)
        Dim key As String = "qqmusic-toplist"
        If GetCache(key) IsNot Nothing Then
            Return TryCast(GetCache(key), List(Of TopList))
        Else
            Dim url = "https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_opt.fcg?page=index&format=html&tpl=macv4&v8debug=1&jsonCallback=jsonCallback"
            Dim client As New WebClient()
            client.Encoding = Encoding.UTF8
            Dim result = client.DownloadString(url)
            result = result.Substring(result.IndexOf("("c) + 1)
            result = result.Substring(0, result.LastIndexOf(")"))
            Dim des = New JavaScriptSerializer().Deserialize(Of List(Of TopList))(result)
            SetCache(key, des)
            Return des
        End If
    End Function
    Public Shared Function GetLyric(ByVal songmid As String) As List(Of String)
        Dim key As String = "qqmusic-lyric-" & songmid
        If GetCache(key) IsNot Nothing Then
            Return TryCast(GetCache(key), List(Of String))
        Else
            Dim url = $"https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?songmid={songmid}&g_tk=5381"
            Dim client As New WebClient()

            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36")
            client.Headers.Add("Accept", "*/*")
            client.Headers.Add("Referer", "https://y.qq.com/portal/player.html")
            client.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8")
            client.Headers.Add("Cookie", "tvfe_boss_uuid=c3db0dcc4d677c60; pac_uid=1_2728578956; qq_slist_autoplay=on; ts_refer=ADTAGh5_playsong; RK=pKOOKi2f1O; pgv_pvi=8927113216; o_cookie=2728578956; pgv_pvid=5107924810; ptui_loginuin=2728578956; ptcz=897c17d7e17ae9009e018ebf3f818355147a3a26c6c67a63ae949e24758baa2d; pt2gguin=o2728578956; pgv_si=s5715204096; qqmusic_fromtag=66; yplayer_open=1; ts_last=y.qq.com/portal/player.html; ts_uid=996779984; yq_index=0")

            client.Headers.Add("Host", "c.y.qq.com")
            Dim result = client.DownloadString(url)
            result = result.Substring(result.IndexOf("("c) + 1)
            result = result.Substring(0, result.LastIndexOf(")"c))
            'INSTANT VB NOTE: In the following line, Instant VB substituted 'Object' for 'dynamic' - this will work in VB with Option Strict Off:
            Dim data = New JavaScriptSerializer().Deserialize(Of Object)(result)
            Dim lyric As String = MusicTool.Base64Decode(data.lyric.ToString())
            Dim list As New List(Of String)()
            For Each item In lyric.Split(ControlChars.Lf)
                list.Add(item)
            Next item
            Return list
        End If
    End Function
    Public Shared Function GetCache(ByVal CacheKey As String) As Object
        Dim objCache As System.Web.Caching.Cache = HttpRuntime.Cache
        Return objCache.Get(CacheKey)
    End Function

    Public Shared Sub SetCache(ByVal CacheKey As String, ByVal objObject As Object)
        Dim objCache As System.Web.Caching.Cache = HttpRuntime.Cache
        If objCache.Get(CacheKey) IsNot Nothing Then
            objCache.Remove(CacheKey)
        End If
        objCache.Insert(CacheKey, objObject)
    End Sub

    Public Class QQMusicInfo
        Public Property SongId() As String
        Public Property SongName() As String
        ''' <summary>
        ''' 时长("yyyy-MM-dd")
        ''' </summary>
        Public Property SongTime() As String

        ''' <summary>
        ''' 时长（秒）
        ''' </summary>
        Public Property SongTimeLength() As Integer
        ''' <summary>
        ''' 
        ''' </summary>
        Public Property Songer() As String
        ''' <summary>
        ''' 文件大小
        ''' </summary>
        Public Property SongSize() As String
        Public Property SongUrl() As String

    End Class

    Public Class TopList
        Public Property GroupID() As String
        Public Property GroupName() As String
        Public Property List() As List(Of TopListCategory)
        Public Property Type() As Integer
    End Class
    Public Class TopListCategory
        Public Property ListName() As String
        Public Property topID() As Integer
        Public Property update_key() As String
    End Class
End Class

