Imports System.IO
Imports System.Net
Imports System.Numerics
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization

Public Class NetEasyMusic
    Public Shared Function GetNetEasyMusicList(SongName As String) As List(Of String)
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
    Public Shared Sub PlayNetEasyMusic(songID As String, title As String, singer As String, pic_url As String, thisqq As Long, GroupId As String)
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
                        API.SendGroupMsg(Pinvoke.plugin_key, thisqq, GroupId, "不支持该歌曲播放", False)
                    Else
                        API.ShareMusic(Pinvoke.plugin_key, thisqq, GroupId, title, singer, jumpUrl, pic_url, jumpUrl, MusicAppTypeEnum.WangYiMusic, MusicShare_Type.GroupMsg)
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
    Private Shared Function AESEncode(ByVal secretData As String, Optional ByVal secret As String = "0CoJUm6Qyw8W8jud") As String
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
    Public Shared Function EncrytData(encrypt As String) As List(Of String)
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
    Private Shared Function RSAEncode(ByVal text As String) As String
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
    Private Shared Function BCHexDec(ByVal hex As String) As BigInteger
        Dim dec As New BigInteger(0)
        Dim len As Integer = hex.Length
        For i As Integer = 0 To len - 1
            dec += BigInteger.Multiply(New BigInteger(Convert.ToInt32(hex.Chars(i).ToString(), 16)), BigInteger.Pow(New BigInteger(16), len - i - 1))
        Next i
        Return dec
    End Function
    Public Shared Function CreateRandomAlphanumericString(size As Integer) As String
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

End Class
