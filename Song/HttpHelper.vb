Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Authentication
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security

Module HttpHelper

    Public Function GetAllCookiesFromHeader(ByVal strHeader As String, ByVal strHost As String) As CookieCollection
        Dim al As ArrayList = New ArrayList()
        Dim cc As CookieCollection = New CookieCollection()

        If strHeader <> String.Empty Then
            al = ConvertCookieHeaderToArrayList(strHeader)
            cc = ConvertCookieArraysToCookieCollection(al, strHost)
        End If

        Return cc
    End Function
    Private Function ConvertCookieHeaderToArrayList(ByVal strCookHeader As String) As ArrayList
        If strCookHeader Is Nothing Then
            Return Nothing
        End If

        strCookHeader = strCookHeader.Replace(vbCr, "")
        strCookHeader = strCookHeader.Replace(vbLf, "")
        Dim strCookTemp As String() = strCookHeader.Split(","c)
        Dim al As ArrayList = New ArrayList()
        Dim i As Integer = 0
        Dim n As Integer = strCookTemp.Length

        While i < n

            If strCookTemp(i).IndexOf("expires=", StringComparison.OrdinalIgnoreCase) > 0 Then
                al.Add(strCookTemp(i) & "," & strCookTemp(i + 1))
                i = i + 1
            Else
                al.Add(strCookTemp(i))
            End If

            i = i + 1
        End While

        Return al
    End Function
    Private Function ConvertCookieArraysToCookieCollection(ByVal al As ArrayList, ByVal strHost As String) As CookieCollection

        Dim cc As CookieCollection = New CookieCollection()
        Dim alcount As Integer = al.Count
        Dim strEachCook As String
        Dim strEachCookParts As String()

        For i As Integer = 0 To alcount - 1
            strEachCook = al(i).ToString()
            strEachCookParts = strEachCook.Split(";"c)
            Dim intEachCookPartsCount As Integer = strEachCookParts.Length
            Dim strCNameAndCValue As String = String.Empty
            Dim strPNameAndPValue As String = String.Empty
            Dim strDNameAndDValue As String = String.Empty
            Dim NameValuePairTemp As String()
            Dim cookTemp As Cookie = New Cookie()

            For j As Integer = 0 To intEachCookPartsCount - 1

                If j = 0 Then
                    strCNameAndCValue = strEachCookParts(j)

                    If strCNameAndCValue <> String.Empty Then
                        Dim firstEqual As Integer = strCNameAndCValue.IndexOf("=")
                        Dim firstName As String = strCNameAndCValue.Substring(0, firstEqual)
                        Dim allValue As String = strCNameAndCValue.Substring(firstEqual + 1, strCNameAndCValue.Length - (firstEqual + 1)).Trim()
                        cookTemp.Name = firstName
                        cookTemp.Value = allValue
                    End If

                    Continue For
                End If

                If strEachCookParts(j).IndexOf("path", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    strPNameAndPValue = strEachCookParts(j)

                    If strPNameAndPValue <> String.Empty Then
                        NameValuePairTemp = strPNameAndPValue.Split("="c)

                        If NameValuePairTemp(1) <> String.Empty Then
                            cookTemp.Path = NameValuePairTemp(1)
                        Else
                            cookTemp.Path = "/"
                        End If
                    End If

                    Continue For
                End If

                If strEachCookParts(j).IndexOf("domain", StringComparison.OrdinalIgnoreCase) >= 0 Then
                    strPNameAndPValue = strEachCookParts(j)

                    If strPNameAndPValue <> String.Empty Then
                        NameValuePairTemp = strPNameAndPValue.Split("="c)

                        If NameValuePairTemp(1) <> String.Empty Then
                            cookTemp.Domain = NameValuePairTemp(1)
                        Else
                            cookTemp.Domain = strHost
                        End If
                    End If

                    Continue For
                End If
            Next

            If cookTemp.Path = String.Empty Then
                cookTemp.Path = "/"
            End If

            If cookTemp.Domain = String.Empty Then
                cookTemp.Domain = strHost
            End If

            If cookTemp.Value <> "" Then
                cc.Add(cookTemp)
            End If
        Next

        Return cc
    End Function
    Public Function RequestGet(ByVal url As String, ByVal headeraccept As String, ByVal contentype As String, ByVal referer As String, ByVal heard As WebHeaderCollection, ByRef cookieContainers As CookieContainer, ByRef redirecturl As String) As String
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function

        'Dim domain = CStr(Regex.Match(url, "^(?:\w+://)?([^/?]*)").Groups(1).Value)

        'If domain.Contains("www.") = True Then
        '    domain = domain.Replace("www.", "")
        'Else
        '    domain = "." & domain
        'End If
        If url = "" Then Return ""
        Dim myRequest As HttpWebRequest = WebRequest.Create(url)
        myRequest.Headers = heard
        myRequest.Method = "GET"
        'myRequest.KeepAlive = True
        myRequest.Accept = headeraccept
        myRequest.ContentType = contentype
        myRequest.Referer = referer
        myRequest.AllowAutoRedirect = False
        myRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"
        myRequest.CookieContainer = cookieContainers
        Dim results As String = ""

        Try
            Using myResponse As HttpWebResponse = myRequest.GetResponse()
                If myResponse.ContentEncoding.ToLower().Contains("gzip") Then
                    Using stream As Stream = New System.IO.Compression.GZipStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)
                        Using reader = New StreamReader(stream)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                ElseIf myResponse.ContentEncoding.ToLower().Contains("deflate") Then
                    Using stream As Stream = New System.IO.Compression.DeflateStream(myResponse.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress)
                        Using reader = New StreamReader(stream)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                Else
                    Using stream As Stream = myResponse.GetResponseStream()
                        Using reader = New StreamReader(stream, Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                End If
                If myResponse.Headers("Location") IsNot Nothing Then
                    redirecturl = myResponse.Headers("Location")
                End If
            End Using

        Catch exp As Exception
            Debug.Print(exp.ToString)
        End Try
        redirecturl = redirecturl
        Return results
    End Function
    Public Function RequestPost(ByVal url As String, ByVal headeraccept As String, ByVal contentype As String, ByVal referer As String, ByVal heard As WebHeaderCollection, ByVal postdata As String, ByRef cookieContainers As CookieContainer, ByRef myWebHeaderCollection As WebHeaderCollection, ByRef redirecturl As String) As String
        If url = "" Then Return ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function

        'Dim domain = CStr(Regex.Match(url, "^(?:\w+://)?([^/?]*)").Groups(1).Value)

        'If domain.Contains("www.") = True Then
        '    domain = domain.Replace("www.", "")
        'Else
        '    domain = domain
        'End If
        Dim results As String = ""
        Try

            Dim myRequest = CType(WebRequest.Create(url), HttpWebRequest)
            Dim data = Encoding.UTF8.GetBytes(postdata)
            myRequest.Headers = heard
            myRequest.Method = "POST"
            'myRequest.KeepAlive = True
            myRequest.Accept = headeraccept
            myRequest.ContentType = contentype
            myRequest.Referer = referer
            myRequest.AllowAutoRedirect = False
            'myRequest.Headers.Add("Upgrade-Insecure-szRequests", "1")
            myRequest.Headers.Add("Cache-Control", "max-age=0")
            myRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/55.0.2883.87 UBrowser/6.2.4098.3 Safari/537.36"
            'myRequest.CookieContainer = cookieContainers
            myRequest.Headers.Add(HttpRequestHeader.Cookie, "os=pc;osver=Microsoft-Windows-10-Professional-build-16299.125-64bit;appver=2.0.3.131777;channel=netease;__remember_me=true")
            myRequest.ContentLength = data.Length
            Using stream = myRequest.GetRequestStream()
                stream.Write(data, 0, data.Length)
            End Using

            Using myResponse As HttpWebResponse = myRequest.GetResponse()
                If myResponse.ContentEncoding.ToLower().Contains("gzip") Then
                    Using stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(New System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress), Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                ElseIf myResponse.ContentEncoding.ToLower().Contains("deflate") Then
                    Using stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(New System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress), Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                Else
                    Using stream As Stream = myResponse.GetResponseStream()
                        Using reader As New StreamReader(stream, Encoding.UTF8)
                            results = reader.ReadToEnd()
                        End Using
                    End Using
                End If
                If myResponse.Headers("Location") IsNot Nothing Then
                    redirecturl = myResponse.Headers("Location")
                End If
                myWebHeaderCollection = myResponse.Headers
            End Using

        Catch exp As Exception
            Debug.Print(exp.ToString)
        End Try

        Return results
    End Function

    Public Function WebClientPost(ByVal url As String, ByVal parms As Dictionary(Of String, String), headers As WebHeaderCollection) As String
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Dim result As String = ""
        Using client As New WebClient()
            Try
                client.Headers = headers
                client.Encoding = Encoding.UTF8
                Dim reqparm = New System.Collections.Specialized.NameValueCollection()
                For Each keyPair In parms
                    reqparm.Add(keyPair.Key, keyPair.Value)
                Next keyPair
                Dim responsebytes() As Byte = client.UploadValues(url, "POST", reqparm)
                result = Encoding.UTF8.GetString(responsebytes)
            Catch ex As WebException
                Using r As New StreamReader(ex.Response.GetResponseStream())
                    Dim responseContent As String = r.ReadToEnd()
                    Debug.Print(responseContent)
                End Using
            End Try
        End Using
        Return result
    End Function

    Public Function WebClientGet(ByVal url As String, headers As WebHeaderCollection, host As String) As String
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Dim result As String = ""
        Using client As New WebClient()
            Try
                client.Headers = headers
                client.Headers("Host") = host
                'client.Headers.Add("Accept-Encoding", "gzip, deflate")
                'client.Headers.Add("Connection", "Keep-Alive")
                client.Encoding = Encoding.UTF8
                result = client.DownloadString(url)
            Catch ex As WebException
                Using r As New StreamReader(ex.Response.GetResponseStream())
                    Dim responseContent As String = r.ReadToEnd()
                    Debug.Print(responseContent)
                End Using
            End Try
        End Using
        Return result
    End Function

End Module
