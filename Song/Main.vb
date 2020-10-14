Imports System.IO
Imports System.Net
Imports System.Runtime.InteropServices
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
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
                    Dim res = RequestGet("https://c.y.qq.com/soso/fcgi-bin/client_search_cp?ct=24&qqmusic_ver=1298&new_json=1&remoteplace=txt.yqq.song&searchid=66137917959202681&t=0&aggr=1&cr=1&catZhida=1&lossless=0&flag_qc=0&p=1&n=10&w=” + songname + “&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "", "", head1, mycookiecontainer, redirect_geturl)
                    If res <> "" Then
                        Dim json As Object = New JavaScriptSerializer().DeserializeObject(res)
                        Dim id As String = json("data")("song")("list")(0)("id")
                        Dim mid As String = json("data")("song")("list")(0)("mid")
                        Dim pmid As String = json("data")("song")("list")(0)("album")("pmid")
                        Dim title As String = json("data")("song")("list")(0)("title")
                        Dim singer As String = json("data")("song")("list")(0)("singer")(0)("name")
                        Dim jumpUrl As String = "https://y.qq.com/n/yqq/song/" + mid + ".html"
                        Dim pic_url As String = "http://y.gtimg.cn/music/photo_new/T002R300x300M000" + pmid + ".jpg?max_age=2592000"
                        res = RequestGet("https://api.qq.jsososo.com/song/url?id=" + mid, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3", "", "", head1, mycookiecontainer, redirect_geturl)
                        If res <> "" Then
                            Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(res)
                            Dim song_url As String = jsonstring("data")
                            '小栗子不支持某些json消息
                            'Dim jsonstr = "{""app"":""com.tencent.structmsg"",""config"":{""autosize"":true,""ctime"":1587018511,""forward"":true,""token"":""4e1ddb75bc9b780b4eda43d937a9b721"",""type"":""normal""},""desc"":""音乐"",""meta"":{""music"":{""action"":"""",""android_pkg_name"":"""",""app_type"":1,""appid"":100497308,""desc"":""" + singer + """,""jumpUrl"":""" + jumpUrl + """,""musicUrl"":""" + song_url + """,""preview"":""" + pic_url + """,""sourceMsgId"":""0"",""source_icon"":"""",""source_url"":"""",""tag"":""QQ音乐"",""title"":""" + title + """}},""prompt"":""[分享]" + title + " - QQ音乐"",""ver"":""0.0.0.1"",""view"":""music""}"
                            'API.SendGroupJSONMessage(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, jsonstr, False)
                            Dim sucess As Boolean = API.ShareMusic(Pinvoke.plugin_key, sMsg.ThisQQ, sMsg.MessageGroupQQ, title, singer, jumpUrl, pic_url, song_url, 0, 1)
                            If sucess Then

                            End If
                        End If
                    End If

                End If
            End If
        End If
        Return 0
    End Function
    'Public Function ReadNetEasySong(SongName As String) As String
    '    Dim res As String = ""
    '    Dim mycookiecontainer As CookieContainer = New CookieContainer()
    '    Dim myWebHeaderCollection As WebHeaderCollection = New WebHeaderCollection()
    '    Dim head1 As WebHeaderCollection = New WebHeaderCollection()
    '    Dim redirect_geturl = String.Empty
    '    res = RequestGet("http://music.163.com/api/search/get?csrf_token=hlpretag=&hlposttag=&s={" + SongName + "}&type=1&offset=0&total=true&limit=20", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, mycookiecontainer, redirect_geturl)
    '    If res <> "" Then
    '        Dim jsonstring As Object = New JavaScriptSerializer().DeserializeObject(res)
    '        Dim songID As String = jsonstring("result")("songs")(0)("id")
    '        Dim data As String = "{""ids"":""[" + songID + "]"",""br"":128000,""csrf_token"":""""}"
    '        Dim retDictionary As List(Of String) = EncrytData(data)
    '        Dim postdata = String.Join("&", retDictionary)
    '        Dim url = "http://music.163.com/api/song/enhance/player/url?id=" + songID + "&ids=" + "%5B" + songID + "%5D" + "&br=3200000&" + postdata
    '        'mycookiecontainer.Add(New Cookie("_ntes_nnid", "3ddf2945dfda076e60b4ee0162f3c0cd,1488291659575; _ntes_nuid=3ddf2945dfda076e60b4ee0162f3c0cd; vjuids=76bd0b1bc.15acc1b713d.0.73360b94f7956; vjlast=1489483035.1489483035.30; vinfo_n_f_l_n3=e441d5bafac7c9b7.1.0.1489483034993.0.1489483124982; JSESSIONID-WYYY=m%2FO4x3rjCh4e1xfBjmOCZ52hbV7rD7M9U77ZX9qn%2BMw5WeKyU0vnw1zGmpbn%5Ci5ZWmdaRmVjQromGw%2BxForePwG3mBf6jOy27vj1IMOv%5ClM3%2BXkUrPOeM7qPP9HhgO%2F%2Fd%5C1nWUq3mDUIicmtDvWMsUbEeWS%5CbNAJyUO8t%5CbHCy731FHp%3A1489661205597; _iuqxldmzr_=32; __utma=94650624.1301580310.1488291660.1489644772.1489659406.9; __utmb=94650624.4.10.1489659406; __utmc=94650624; __utmz=94650624.1489581581.6.4.utmcsr=baidu|utmccn=(organic)|utmcmd=organic") With {.Domain = "music.163.com"})
    '        'res = RequestPost("http://music.163.com/api/song/enhance/player/url?id=" + songID + "&ids=" + "%5B" + songID + "%5D" + "&br=3200000", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, postdata, mycookiecontainer, myWebHeaderCollection, redirect_geturl)
    '        res = RequestGet("http://music.163.com/api/song/detail/?id=" + songID + "&ids=%5B" + songID + "%5D&csrf_token=", "application/json, text/javascript, */*; q=0.01", "application/json;charset=UTF-8", "http://music.163.com/search/", head1, mycookiecontainer, redirect_geturl)
    '        If res <> "" Then
    '            Dim json As Object = New JavaScriptSerializer().DeserializeObject(res)

    '        End If
    '    End If
    '    Return res
    'End Function
    'Private Function AESEncode(ByVal secretData As String, Optional ByVal secret As String = "0CoJUm6Qyw8W8jud") As String
    '    Dim encrypted() As Byte
    '    Dim IV() As Byte = Encoding.UTF8.GetBytes("0102030405060708")
    '    Using aesen = Aes.Create()
    '        aesen.Key = Encoding.UTF8.GetBytes(secret)
    '        aesen.IV = IV
    '        aesen.Mode = CipherMode.CBC
    '        Using encryptor = aesen.CreateEncryptor()
    '            Using stream = New MemoryStream()
    '                Using cstream = New CryptoStream(stream, encryptor, CryptoStreamMode.Write)
    '                    Using sw = New StreamWriter(cstream)
    '                        sw.Write(secretData)
    '                    End Using
    '                    encrypted = stream.ToArray()
    '                End Using
    '            End Using
    '        End Using
    '    End Using
    '    Return Convert.ToBase64String(encrypted)
    'End Function
    'Public Function EncrytData(encrypt As String) As List(Of String)
    '    Dim retValue As New List(Of String)
    '    Dim secKey As String = CreateRandomAlphanumericString(16)
    '    Dim encText As String = AESEncode(encrypt, "0CoJUm6Qyw8W8jud")
    '    encText = AESEncode(encText, secKey)
    '    Dim params As String = "params=" + HttpUtility.UrlEncode(encText)
    '    retValue.Add(params)
    '    Dim encSecKey = "encSecKey=" + RSAEncode(secKey)
    '    retValue.Add(encSecKey)
    '    Return retValue
    'End Function
    'Private Function RSAEncode(ByVal text As String) As String
    '    Dim srtext As New String(text.Reverse().ToArray())

    '    Dim a = BCHexDec(BitConverter.ToString(Encoding.Default.GetBytes(srtext)).Replace("-", ""))
    '    Dim b = BCHexDec("010001")
    '    Dim c = BCHexDec("00e0b509f6259df8642dbc35662901477df22677ec152b5ff68ace615bb7b725152b3ab17a876aea8a5aa76d2e417629ec4ee341f56135fccf695280104e0312ecbda92557c93870114af6c9d05c4f7f0c3685b7a46bee255932575cce10b424d813cfe4875d3e82047b97ddef52741d546b8e289dc6935b3ece0462db0a22b8e7")
    '    Dim key = BigInteger.ModPow(a, b, c).ToString("x")
    '    key = key.PadLeft(256, "0"c)
    '    If key.Length > 256 Then
    '        Return key.Substring(key.Length - 256, 256)
    '    Else
    '        Return key
    '    End If
    'End Function
    'Private Function BCHexDec(ByVal hex As String) As BigInteger
    '    Dim dec As New BigInteger(0)
    '    Dim len As Integer = hex.Length
    '    For i As Integer = 0 To len - 1
    '        dec += BigInteger.Multiply(New BigInteger(Convert.ToInt32(hex.Chars(i).ToString(), 16)), BigInteger.Pow(New BigInteger(16), len - i - 1))
    '    Next i
    '    Return dec
    'End Function
    'Public Function CreateRandomAlphanumericString(size As Integer) As String
    '    Dim allowedChars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray()
    '    Dim bytes = New Byte(0) {}
    '    Dim crypto = New RNGCryptoServiceProvider()
    '    crypto.GetNonZeroBytes(bytes)
    '    bytes = New Byte(size - 1) {}
    '    crypto.GetNonZeroBytes(bytes)
    '    Dim retVal = New StringBuilder(size)
    '    For Each b As Byte In bytes
    '        retVal.Append(allowedChars(b Mod (allowedChars.Length)))
    '    Next
    '    Return retVal.ToString()
    'End Function


#End Region

End Module
