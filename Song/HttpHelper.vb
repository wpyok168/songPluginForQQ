'https://github.com/laomms

Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Security.Authentication
Imports System.Security.Cryptography.X509Certificates
Imports System.Net.Security
Imports System.Reflection
Imports System.Net.Http
Imports System.IO.Compression
Imports System.Threading
Imports System.ComponentModel
Imports System.Windows.Forms

Module HttpHelper
    Private RestrictedHeaders() As String = {"Accept", "Connection", "Content-Length", "Content-Type", "Date", "Expect", "Host", "If-Modified-Since", "Keep-Alive", "Proxy-Connection", "Range", "Referer", "Transfer-Encoding", "User-Agent"}

    ''' <summary>
    ''' Http请求
    ''' </summary>
    ''' <param name="url">请求网址</param>
    ''' <param name="Headerdics">头文件固定KEY值字典类型泛型集合</param>
    ''' <param name="heard">头文件集合</param>
    ''' <param name="cookieContainers">cookie容器</param>
    ''' <param name="redirecturl">头文件中的跳转链接</param>
    ''' <returns>返回请求字符串结果</returns>
    Public Function RequestGet(ByVal url As String, Headerdics As Dictionary(Of String, Object), ByVal heard As WebHeaderCollection, ByRef cookieContainers As CookieContainer, ByRef redirecturl As String) As String
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
        For Each pair In Headerdics
            GetType(HttpWebRequest).GetProperty(pair.Key).SetValue(myRequest, pair.Value, Nothing)
        Next
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
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        results = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try
        redirecturl = redirecturl
        Return results
    End Function
    ''' <summary>
    ''' Http响应
    ''' </summary>
    ''' <param name="url">请求网址</param>
    ''' <param name="Headerdics">头文件固定KEY值字典类型泛型集合</param>
    ''' <param name="heard">头文件集合</param>
    ''' <param name="postdata">提交的字符串型数据</param>
    ''' <param name="cookieContainers">cookie容器</param>
    ''' <param name="redirecturl">头文件中的跳转链接</param>
    ''' <returns>返回响应字符串结果</returns>
    Public Function RequestPost(ByVal url As String, Headerdics As Dictionary(Of String, Object), ByVal heard As WebHeaderCollection, ByVal postdata As String, ByRef cookieContainers As CookieContainer, ByRef ResponseHeaders As WebHeaderCollection, ByRef redirecturl As String) As String
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
            For Each pair In Headerdics
                GetType(HttpWebRequest).GetProperty(pair.Key).SetValue(myRequest, pair.Value, Nothing)
            Next
            myRequest.CookieContainer = cookieContainers
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
                ResponseHeaders = myResponse.Headers
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        results = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return results
    End Function
    ''' <summary>
    ''' Http响应
    ''' </summary>
    ''' <param name="url">下载文件</param>
    ''' <param name="Headerdics">头文件固定KEY值字典类型泛型集合</param>
    ''' <param name="heard">头文件集合</param>
    ''' <param name="filePath">保存文件的路径</param>
    ''' <param name="cookieContainers">cookie容器</param>
    ''' <param name="labelPercentage">显示下载速度及百分比的label控件</param>
    ''' <param name="ProgressBar1">进度条控件</param>
    Public Async Sub RequestDownloadFile(ByVal url As String, Headerdics As Dictionary(Of String, Object), ByVal heard As WebHeaderCollection, cookieContainers As CookieContainer, ByVal filePath As String, labelPercentage As Label, ProgressBar1 As ProgressBar)
        If url = "" Then Return
        Try
            Dim myRequest As HttpWebRequest = WebRequest.Create(url)
            myRequest.Headers = heard
            myRequest.Method = "GET"
            For Each pair In Headerdics
                GetType(HttpWebRequest).GetProperty(pair.Key).SetValue(myRequest, pair.Value, Nothing)
            Next
            myRequest.CookieContainer = cookieContainers

            Using response As HttpWebResponse = myRequest.GetResponse
                Try
                    Dim totalDownloadSize = response.ContentLength
                    Dim stream = response.GetResponseStream
                    Using fileStream = File.Create(filePath)
                        Using stream
                            Dim buffer(1023) As Byte
                            Dim totalBytesRead = 0
                            Dim length As Integer
                            length = Await stream.ReadAsync(buffer, 0, buffer.Length)
                            Do While length <> 0
                                totalBytesRead += length
                                Dim percentage As Double = totalBytesRead / totalDownloadSize * 100
                                labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = True))
                                labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Text = "下载 " & Convert.ToInt32(percentage) & "%  -  " & Convert.ToInt32(totalBytesRead / 1024) & " / " & Convert.ToInt32(totalDownloadSize / 1024) & " kB"))
                                ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = Integer.Parse(Math.Truncate(percentage).ToString())))
                                ' 写入到文件
                                fileStream.Write(buffer, 0, length)
                                length = Await stream.ReadAsync(buffer, 0, buffer.Length)
                            Loop
                            MsgBox("下载完成!")
                            ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = 0))
                            labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = False))
                        End Using
                    End Using
                Catch e As Exception
                End Try
            End Using

        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        Debug.Print(reader.ReadToEnd())
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try


    End Sub

    Public Async Function HttpClientPostAsync(ByVal url As String, Headerdics As Dictionary(Of String, Object), postdata As String, datatype As String, cookieContainers As CookieContainer, redirecturl As String) As Task(Of String)
        If url = "" Then Return ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Dim res = ""
        Try
            Using handler = New HttpClientHandler()
                handler.CookieContainer = cookieContainers
                Using client = New HttpClient(handler)
                    For Each pair In Headerdics
                        client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)
                    Next
                    Dim content As New Http.StringContent(postdata, System.Text.Encoding.UTF8, datatype)
                    Using HttpResponse As HttpResponseMessage = Await client.PostAsync(url, content)
                        HttpResponse.EnsureSuccessStatusCode()
                        If Not HttpResponse.Headers.Location Is Nothing Then
                            redirecturl = HttpResponse.Headers.Location.ToString
                        End If
                        If LCase(HttpResponse.ToString).Contains("gzip") Then
                            Using HttpResponseStream As Stream = Await client.GetStreamAsync(url)
                                Using gzipStream = New GZipStream(HttpResponseStream, CompressionMode.Decompress)
                                    Using streamReader = New StreamReader(gzipStream, Encoding.UTF8)
                                        res = streamReader.ReadToEnd()
                                    End Using
                                End Using
                            End Using
                        Else
                            Using HttpContent As HttpContent = HttpResponse.Content
                                res = Await HttpContent.ReadAsStringAsync()
                            End Using
                        End If
                    End Using
                End Using
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return res
    End Function
    Public Async Function HttpClientGetAsync(ByVal url As String, Headerdics As Dictionary(Of String, Object), cookieContainers As CookieContainer, redirecturl As String) As Task(Of String)
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Dim res = ""
        Try
            Using handler = New HttpClientHandler()
                handler.CookieContainer = cookieContainers
                Using client = New HttpClient(handler)
                    For Each pair In Headerdics
                        client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)
                    Next
                    Using HttpResponse As HttpResponseMessage = Await client.GetAsync(url)
                        If HttpResponse.StatusCode = System.Net.HttpStatusCode.OK Then
                            If Not HttpResponse.Headers.Location Is Nothing Then
                                redirecturl = HttpResponse.Headers.Location.ToString
                            End If
                            If LCase(HttpResponse.ToString).Contains("gzip") Then
                                Using HttpResponseStream As Stream = Await client.GetStreamAsync(url)
                                    Using gzipStream = New GZipStream(HttpResponseStream, CompressionMode.Decompress)
                                        Using streamReader = New StreamReader(gzipStream, Encoding.UTF8)
                                            res = streamReader.ReadToEnd()
                                        End Using
                                    End Using
                                End Using
                            Else
                                Using HttpContent As HttpContent = HttpResponse.Content
                                    res = Await HttpContent.ReadAsStringAsync()
                                End Using
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return res
    End Function
    Public Async Function HttpClientPostFormAsync(ByVal url As String, Headerdics As Dictionary(Of String, Object), form As MultipartFormDataContent, cookieContainers As CookieContainer, redirecturl As String) As Task(Of String)
        If url = "" Then Return ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Dim res = ""
        Try
            Using handler = New HttpClientHandler()
                handler.CookieContainer = cookieContainers
                Using client = New HttpClient(handler)
                    For Each pair In Headerdics
                        client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)
                    Next
                    Using HttpResponse As HttpResponseMessage = Await client.PostAsync(url, form)
                        HttpResponse.EnsureSuccessStatusCode()
                        If Not HttpResponse.Headers.Location Is Nothing Then
                            redirecturl = HttpResponse.Headers.Location.ToString
                        End If
                        If LCase(HttpResponse.ToString).Contains("gzip") Then
                            Using HttpResponseStream As Stream = Await client.GetStreamAsync(url)
                                Using gzipStream = New GZipStream(HttpResponseStream, CompressionMode.Decompress)
                                    Using streamReader = New StreamReader(gzipStream, Encoding.UTF8)
                                        res = streamReader.ReadToEnd()
                                    End Using
                                End Using
                            End Using
                        Else
                            Using HttpContent As HttpContent = HttpResponse.Content
                                res = Await HttpContent.ReadAsStringAsync()
                            End Using
                        End If
                    End Using
                End Using
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return res
    End Function
    Public Async Function HttpClientDownloadFileAsync(ByVal url As String, Headerdics As Dictionary(Of String, Object), cookieContainers As CookieContainer, filepath As String, labelPercentage As Label, ProgressBar1 As ProgressBar) As Task(Of Boolean)
        If url = "" Then Return ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Try

            Using client = New HttpClient(New HttpClientHandler() With {.CookieContainer = cookieContainers, .AutomaticDecompression = DecompressionMethods.None Or DecompressionMethods.Deflate Or DecompressionMethods.GZip})
                For Each pair In Headerdics
                    client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)
                Next

                Using HttpResponse As HttpResponseMessage = Await client.GetAsync(url)
                    If HttpResponse.StatusCode = System.Net.HttpStatusCode.OK Then
                        Try
                            Dim totalDownloadSize = HttpResponse.Content.Headers.ContentLength
                            Dim stream = Await HttpResponse.Content.ReadAsStreamAsync()
                            Using fileStream = File.Create(filepath)
                                Using stream
                                    Dim buffer(1023) As Byte
                                    Dim totalBytesRead = 0
                                    Dim length As Integer
                                    length = Await stream.ReadAsync(buffer, 0, buffer.Length)
                                    Do While length <> 0
                                        totalBytesRead += length
                                        Dim percentage As Double = totalBytesRead / totalDownloadSize * 100
                                        labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = True))
                                        labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Text = "下载 " & Convert.ToInt32(percentage) & "%  -  " & Convert.ToInt32(totalBytesRead / 1024) & " / " & Convert.ToInt32(totalDownloadSize / 1024) & " kB"))
                                        ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = Integer.Parse(Math.Truncate(percentage).ToString())))
                                        ' 写入到文件
                                        fileStream.Write(buffer, 0, length)
                                        length = Await stream.ReadAsync(buffer, 0, buffer.Length)
                                    Loop
                                    MsgBox("下载完成!")
                                    ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = 0))
                                    labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = False))
                                End Using
                            End Using
                        Catch e As Exception
                        End Try
                    End If
                End Using
            End Using

        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        Return False
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return False
    End Function
    Public Async Function HttpClientGetRedirectLink(ByVal url As String, Headerdics As Dictionary(Of String, Object), cookieContainers As CookieContainer) As Task(Of String)
        If url = "" Then Return ""
        Dim res = ""
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls Or SecurityProtocolType.Tls11
        ServicePointManager.ServerCertificateValidationCallback = Function(sender As Object, certificate As X509Certificate, chain As X509Chain, sslPolicyErrors As SslPolicyErrors)
                                                                      Return True
                                                                  End Function
        Try
            Using client = New HttpClient(New HttpClientHandler() With {.CookieContainer = cookieContainers, .AutomaticDecompression = DecompressionMethods.None Or DecompressionMethods.Deflate Or DecompressionMethods.GZip}) With {.Timeout = TimeSpan.FromSeconds(30)}
                For Each pair In Headerdics
                    client.DefaultRequestHeaders.TryAddWithoutValidation(pair.Key, pair.Value)
                Next
                Dim response As HttpResponseMessage = Await client.GetAsync(url)
                Dim statusCode = Math.Truncate(response.StatusCode)
                If statusCode >= 300 AndAlso statusCode <= 399 Then
                    Return response.Headers.Location.ToString
                ElseIf Not response.IsSuccessStatusCode Then
                    Dim responseUri As String = response.RequestMessage.RequestUri.ToString()
                    Console.Out.WriteLine(responseUri)
                Else
                    Return response.RequestMessage.RequestUri.ToString()
                End If
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try

        Return res
    End Function

    Public Function WebClientPost(url As String, Headerdics As Dictionary(Of String, Object), postdata As String, cookieContainers As CookieContainer) As String
        Dim res As String = ""
        Try
            Using client As New WebClientEx(cookieContainers)
                For Each pair In Headerdics
                    client.Headers.Add(pair.Key, pair.Value)
                Next
                res = client.UploadString(url, postdata)
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try
        Return res
    End Function
    Public Function WebClientGet(url As String, Headerdics As Dictionary(Of String, Object), cookieContainers As CookieContainer) As String
        Dim res As String = ""
        Try
            Using client As New WebClientEx(cookieContainers)
                For Each pair In Headerdics
                    client.Headers.Add(pair.Key, pair.Value)
                Next
                res = client.DownloadString(url)
            End Using
        Catch e As WebException
            Using response As WebResponse = e.Response
                Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                Using data As Stream = response.GetResponseStream()
                    Using reader = New StreamReader(data)
                        res = reader.ReadToEnd()
                    End Using
                End Using
            End Using
        Catch ex As Exception
            If Not ex.InnerException Is Nothing Then
                Debug.Print(ex.GetBaseException.Message.ToString)
            Else
                Debug.Print(ex.Message.ToString)
            End If
        End Try
        Return res
    End Function
    Public Sub WebClientDownload(url As String, Headerdics As Dictionary(Of String, Object), cookieContainers As CookieContainer, filepath As String, labelPercentage As Label, ProgressBar1 As ProgressBar)
        Dim thread As New Thread(Sub()
                                     Try
                                         Using client As New WebClientEx(cookieContainers)
                                             For Each pair In Headerdics
                                                 client.Headers.Add(pair.Key, pair.Value)
                                             Next
                                             AddHandler client.DownloadProgressChanged, Sub(sender, e) client_DownloadProgressChanged(sender, e, labelPercentage, ProgressBar1) ' AddressOf client_DownloadProgressChanged
                                             AddHandler client.DownloadFileCompleted, Sub(sender, e) client_DownloadFileCompleted(sender, e, labelPercentage, ProgressBar1) ' AddressOf client_DownloadFileCompleted
                                             client.DownloadFileAsync(New Uri(url), filepath)
                                         End Using
                                     Catch e As WebException
                                         Using response As WebResponse = e.Response
                                             Dim httpResponse As HttpWebResponse = CType(response, HttpWebResponse)
                                             Console.WriteLine("Error code: {0}", httpResponse.StatusCode)
                                             Using data As Stream = response.GetResponseStream()
                                                 Using reader = New StreamReader(data)
                                                     Debug.Print(reader.ReadToEnd())
                                                 End Using
                                             End Using
                                         End Using
                                     Catch ex As Exception
                                         If Not ex.InnerException Is Nothing Then
                                             Debug.Print(ex.GetBaseException.Message.ToString)
                                         Else
                                             Debug.Print(ex.Message.ToString)
                                         End If
                                     End Try

                                 End Sub)
        thread.Start()
    End Sub
    Private Sub client_DownloadProgressChanged(ByVal sender As Object, ByVal e As DownloadProgressChangedEventArgs, labelPercentage As Label, ProgressBar1 As ProgressBar)
        'BeginInvoke(CType(Sub()
        Dim bytesIn As Double = Double.Parse(e.BytesReceived.ToString())
        Dim totalBytes As Double = Double.Parse(e.TotalBytesToReceive.ToString())
        Dim percentage As Double = bytesIn / totalBytes * 100
        'labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = True))
        'labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Text = "下载 " & Convert.ToInt32(percentage) & "%  -  " & Convert.ToInt32(bytesIn / 1024) & " / " & Convert.ToInt32(totalBytes / 1024) & " kB"))
        'ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = Integer.Parse(Math.Truncate(percentage).ToString())))
        'End Sub, MethodInvoker))
    End Sub
    Private Sub client_DownloadFileCompleted(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs, labelPercentage As Label, ProgressBar1 As ProgressBar)
        'Me.BeginInvoke(CType(Sub()
        'ProgressBar1.Invoke(New MethodInvoker(Sub() ProgressBar1.Value = 0))
        'labelPercentage.Invoke(New MethodInvoker(Sub() labelPercentage.Visible = False))
        MsgBox("下载完成!")
        'End Sub, MethodInvoker))
    End Sub
End Module
Public Class WebClientEx
    Inherits WebClient

    Public Sub New(ByVal container As CookieContainer)
        Me.container = container
    End Sub

    Public Property CookieContainer() As CookieContainer
        Get
            Return container
        End Get
        Set(ByVal value As CookieContainer)
            container = value
        End Set
    End Property

    Shadows container As New CookieContainer()

    Protected Overrides Function GetWebRequest(ByVal address As Uri) As WebRequest
        Dim r As WebRequest = MyBase.GetWebRequest(address)
        Dim request = TryCast(r, HttpWebRequest)
        If request IsNot Nothing Then
            request.CookieContainer = container
        End If
        Return r
    End Function

    Protected Overrides Function GetWebResponse(ByVal request As WebRequest, ByVal result As IAsyncResult) As WebResponse
        Dim response As WebResponse = MyBase.GetWebResponse(request, result)
        ReadCookies(response)
        Return response
    End Function

    Protected Overrides Function GetWebResponse(ByVal request As WebRequest) As WebResponse
        Dim response As WebResponse = MyBase.GetWebResponse(request)
        ReadCookies(response)
        Return response
    End Function

    Private Sub ReadCookies(ByVal r As WebResponse)
        Dim response = TryCast(r, HttpWebResponse)
        If response IsNot Nothing Then
            Dim cookies As CookieCollection = response.Cookies
            container.Add(cookies)
        End If
    End Sub

End Class