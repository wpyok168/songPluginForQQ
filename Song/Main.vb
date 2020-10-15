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
            If sMsg.MessageContent.Contains("点歌") Then
                Dim songname As String = sMsg.MessageContent.Replace("点歌", "").Trim
                If songname = "" Then Return 0
                If MusicType = 1 Then
                    TencentMusic(songname, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                ElseIf MusicType = 2 Then
                    KugouMusic(songname, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                ElseIf MusicType = 3 Then
                    KuwoMusic(songname, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                Else
                    NetEasyMusic(songname, sMsg.ThisQQ, sMsg.MessageGroupQQ)
                End If
            End If
        End If
        Return 0
    End Function

    Public Sub TencentMusic(songname As String, QQId As Long, GroupId As Long)
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
            Dim url = "https://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.song&searchid=66137917959202681&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=10&w=” + songname + “&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0"
            Dim res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "", "", head1, mycookiecontainer, redirect_geturl)
            If res <> "" Then
                Try
                    Dim json As Object = New JavaScriptSerializer().DeserializeObject(res)
                    Dim id As String = json("data")("song")("list")(0)("id")
                    Dim mid As String = json("data")("song")("list")(0)("mid")
                    Dim pmid As String = json("data")("song")("list")(0)("album")("pmid")
                    Dim title As String = json("data")("song")("list")(0)("title")
                    Dim singer As String = json("data")("song")("list")(0)("singer")(0)("name")
                    Dim jumpUrl As String = "https://y.qq.com/n/yqq/song/" + mid + ".html"
                    Dim pic_url As String = "http://y.gtimg.cn/music/photo_new/T002R300x300M000" + pmid + ".jpg?max_age=2592000"
                    url = "https://api.qq.jsososo.com/song/url?id=" + mid
                    res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "", "", head1, mycookiecontainer, redirect_geturl)
                    If res <> "" Then
                        Try
                            Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(res)
                            Dim song_url As String = jsonstring("data")
                            '小栗子不支持某些json消息
                            'Dim jsonstr = "{""app"":""com.tencent.structmsg"",""config"":{""autosize"":true,""ctime"":1587018511,""forward"":true,""token"":""4e1ddb75bc9b780b4eda43d937a9b721"",""type"":""normal""},""desc"":""音乐"",""meta"":{""music"":{""action"":"""",""android_pkg_name"":"""",""app_type"":1,""appid"":100497308,""desc"":""" + singer + """,""jumpUrl"":""" + jumpUrl + """,""musicUrl"":""" + song_url + """,""preview"":""" + pic_url + """,""sourceMsgId"":""0"",""source_icon"":"""",""source_url"":"""",""tag"":""QQ音乐"",""title"":""" + title + """}},""prompt"":""[分享]" + title + " - QQ音乐"",""ver"":""0.0.0.1"",""view"":""music""}"
                            'API.SendGroupJSONMessage(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, jsonstr, False)
                            Dim sucess As Boolean = API.ShareMusic(Pinvoke.plugin_key, QQId, GroupId, title, singer, jumpUrl, pic_url, song_url, 0, 1)
                            If sucess Then

                            End If
                        Catch ex As Exception

                        End Try

                    End If
                Catch ex As Exception

                End Try

            End If
        End If
    End Sub
    Public Sub KugouMusic(songname As String, QQId As Long, GroupId As Long)
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
        Dim FileHash As String = ""
        Dim AlbumID As String = ""
        Dim url = "http://mobilecdn.kugou.com/api/v3/search/song?format=json&keyword=" + UCase(HttpUtility.UrlEncode(songname)) + "&page=1&pagesize=20&showtype=1"
        Dim res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "application/json; charset=utf-8", "", head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Try
                Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(res)
                AlbumID = jsonstring("data")("info")(0)("album_id")
                FileHash = jsonstring("data")("info")(0)("hash")
            Catch ex As Exception

            End Try

        End If
        Dim mycookiecontainer2 As CookieContainer = New CookieContainer()
        mycookiecontainer2.Add(New Cookie("kg_mid", "37e7d0becb4297c1a1bb8d59a956fa46") With {.Domain = "www.kugou.com"})
        mycookiecontainer2.Add(New Cookie("Hm_lpvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "www.kugou.com"})
        mycookiecontainer2.Add(New Cookie("Hm_lvt_aedee6983d4cfc62f509129360d6bb3d", "1602680303") With {.Domain = "www.kugou.com"})
        mycookiecontainer2.Add(New Cookie("KuGooRandom", "6671602680303344") With {.Domain = "www.kugou.com"})
        url = "http://www.kugou.com/yy/index.php?r=play/getdata&hash=" + FileHash
        res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "application/json; charset=utf-8", "", head1, mycookiecontainer2, redirect_geturl)
        If res <> "" Then
            Try
                Dim json = New JavaScriptSerializer().DeserializeObject(res)
                Dim pic_url As String = json("data")("img")
                Dim title As String = json("data")("song_name")
                Dim singer As String = json("data")("author_name")
                Dim jumpUrl As String = json("data")("play_url")
                Dim song_url As String = json("data")("play_backup_url")
                API.ShareMusic(Pinvoke.plugin_key, QQId, GroupId, title, singer, jumpUrl, pic_url, song_url, 0, 1)
            Catch ex As Exception

            End Try
        End If
    End Sub
    Public Sub KuwoMusic(songname As String, QQId As Long, GroupId As Long)

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
        Dim url = "http://www.kuwo.cn/api/www/search/searchMusicBykeyWord?key=" + UCase(HttpUtility.UrlEncode(songname)) + "&pn=1&rn=10&httpsStatus=1&reqId=06427b90-0e8d-11eb-a5da-975a5becfc1e"
        Dim res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "application/json, text/plain, */*", "http://www.kuwo.cn/search/list?key=" + UCase(HttpUtility.UrlEncode(songname)), head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Try
                Dim json = New JavaScriptSerializer().DeserializeObject(res)
                Dim reqId As String = json("reqId").ToString.Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")
                Dim curTime As String = json("curTime")
                Dim pic_url As String = json("data")("list")(0)("pic")
                Dim title As String = json("data")("list")(0)("name")
                Dim singer As String = json("data")("list")(0)("artist")
                Dim rid As String = json("data")("list")(0)("rid")
                url = "http://www.kuwo.cn/url?format=mp3&rid=" + rid + "&response=url&type=convert_url3&br=128kmp3&from=web&t=" + curTime + "&httpsStatus=1&reqId=" + reqId
                res = RequestGet(url, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "application/json, text/plain, */*", "http://www.kuwo.cn/search/list?key=" + UCase(HttpUtility.UrlEncode(songname)), head1, mycookiecontainer, redirect_geturl)
                If res <> "" Then
                    Dim jsonstring = New JavaScriptSerializer().DeserializeObject(res)
                    Dim jumpUrl As String = jsonstring("url")
                    API.ShareMusic(Pinvoke.plugin_key, QQId, GroupId, title, singer, jumpUrl, pic_url, jumpUrl, 0, 1)
                End If
            Catch ex As Exception

            End Try

        End If


        Dim host = "http://www.kuwo.cn"
        ' 根据关键字key获取歌曲的rid值的json数据的接口
        Dim rid_url = "/api/www/search/searchMusicBykeyWord?key={}"
        ' 根据rid获取歌曲下载链接的json数据的接口
        Dim mp3_url = "/url?rid={}&type=convert_url3&br=128kmp3"
        ' 获取音乐榜 可以得到sourceid
        Dim bang_menu = "/api/www/bang/bang/bangMenu"
        ' 获取音乐信息的接口
        Dim music_info = "/api/www/music/musicInfo?mid={}"
        ' 根据 musicid 获取歌词信息
        Dim song_lyric = "http://m.kuwo.cn/newh5/singles/songinfoandlrc?musicId={}"
        ' 根据bangid 获取音乐列表
        Dim music_list = "/api/www/bang/bang/musicList?bangId={}&pn={}&rn={}"
        ' 一些必要的请求头
        Dim headers = New Dictionary(Of Object, Object) From {
                {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/79.0.3945.130 Safari/537.36"},
                {"Referer", "http://www.kuwo.cn/search/list"},
                {"csrf", "0HQ0UGKNAKR"},
                {"Cookie", "Hm_lvt_cdb524f42f0ce19b169a8071123a4797=1584003311; _ga=GA1.2.208068437.1584003311; _gid=GA1.2.1613688009.1584003311; Hm_lpvt_cdb524f42f0ce19b169a8071123a4797=1584017980; kw_token=0HQ0UGKNAKR; _gat=1"}
            }

    End Sub
    Public Function NetEasyMusic(SongName As String, QQId As Long, GroupId As Long) As String
        Dim res As String = ""
        Dim mycookiecontainer As CookieContainer = New CookieContainer()
        Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
        Dim head1 As WebHeaderCollection = New WebHeaderCollection()
        Dim redirect_geturl = String.Empty
        res = RequestGet("http://music.163.com/api/search/get?csrf_token=hlpretag=&hlposttag=&s={" + SongName + "}&type=1&offset=0&total=true&limit=20", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, mycookiecontainer, redirect_geturl)
        If res <> "" Then
            Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(res)
            Dim songID As String = jsonstring("result")("songs")(0)("id")
            Dim data As String = "{""ids"":""[" + songID + "]"",""br"":128000,""csrf_token"":""""}"
            Dim retDictionary As List(Of String) = EncrytData(data)
            Dim postdata = String.Join("&", retDictionary)
            'Dim url = "http://music.163.com/api/song/enhance/player/url?id=" + songID + "&ids=" + "%5B" + songID + "%5D" + "&br=3200000&" + postdata
            'mycookiecontainer.Add(New Cookie("_ntes_nnid", "3ddf2945dfda076e60b4ee0162f3c0cd,1488291659575; _ntes_nuid=3ddf2945dfda076e60b4ee0162f3c0cd; vjuids=76bd0b1bc.15acc1b713d.0.73360b94f7956; vjlast=1489483035.1489483035.30; vinfo_n_f_l_n3=e441d5bafac7c9b7.1.0.1489483034993.0.1489483124982; JSESSIONID-WYYY=m%2FO4x3rjCh4e1xfBjmOCZ52hbV7rD7M9U77ZX9qn%2BMw5WeKyU0vnw1zGmpbn%5Ci5ZWmdaRmVjQromGw%2BxForePwG3mBf6jOy27vj1IMOv%5ClM3%2BXkUrPOeM7qPP9HhgO%2F%2Fd%5C1nWUq3mDUIicmtDvWMsUbEeWS%5CbNAJyUO8t%5CbHCy731FHp%3A1489661205597; _iuqxldmzr_=32; __utma=94650624.1301580310.1488291660.1489644772.1489659406.9; __utmb=94650624.4.10.1489659406; __utmc=94650624; __utmz=94650624.1489581581.6.4.utmcsr=baidu|utmccn=(organic)|utmcmd=organic") With {.Domain = "music.163.com"})
            'res = RequestPost("http://music.163.com/api/song/enhance/player/url?id=" + songID + "&ids=" + "%5B" + songID + "%5D" + "&br=3200000", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, postdata, mycookiecontainer, myWebHeaderCollection, redirect_geturl)
            res = RequestGet("http://music.163.com/api/song/detail/?id=" + songID + "&ids=%5B" + songID + "%5D&csrf_token=", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, mycookiecontainer, redirect_geturl)
            If res <> "" Then
                Try
                    Dim json = New JavaScriptSerializer().DeserializeObject(res)
                    Dim id As String = json("songs")(0)("id")
                    Dim pic_url As String = json("songs")(0)("artists")(0)("picUrl")
                    Dim title As String = json("songs")(0)("name")
                    Dim singer As String = json("songs")(0)("artists")(0)("name")
                    Dim jumpUrl As String = json("songs")(0)("rurl")
                    Dim song_url As String = json("songs")(0)("mp3Url")
                    API.ShareMusic(Pinvoke.plugin_key, QQId, GroupId, title, singer, jumpUrl, pic_url, song_url, 0, 1)
                Catch ex As Exception

                End Try

            End If
        End If
        Return res
    End Function
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
