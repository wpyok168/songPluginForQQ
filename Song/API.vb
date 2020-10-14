Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports System.Web.Script.Serialization
Imports System.Windows.Forms


Public Class API

#Region "导出函数给框架并取到两个参数值"
    <DllExport(CallingConvention:=System.Runtime.InteropServices.CallingConvention.StdCall)>
    Public Shared Function apprun(<MarshalAs(UnmanagedType.LPStr)> ByVal apidata As String, <MarshalAs(UnmanagedType.LPStr)> ByVal pluginkey As String) As IntPtr
        jsonstr = apidata
        plugin_key = pluginkey

        Dim json = ""
        Dim JosnDict As Dictionary(Of String, String) = (New JavaScriptSerializer()).Deserialize(Of Dictionary(Of String, String))(apidata)
        For Each KeyList As KeyValuePair(Of String, String) In JosnDict
            json = AddPermission(KeyList.Key, json)
        Next KeyList
        'json = AddPermission("输出日志", json)
        'json = AddPermission("发送好友消息", json)
        'json = AddPermission("查询好友信息", json)
        'json = AddPermission("发送群消息", json)
        'json = AddPermission("取图片下载地址", json)
        'json = AddPermission("取好友列表", json)
        'json = AddPermission("取群成员列表", json)
        'json = AddPermission("取群列表", json)
        'json = AddPermission("取框架QQ", json)
        'json = AddPermission("框架重启", json)

        Dim jsonkey As Object = New JavaScriptSerializer().DeserializeObject(json)
        Dim resultJson As String = New JavaScriptSerializer().Serialize(New With {Key .needapilist = jsonkey})

        Dim App_Info = New AppInfo()
        App_Info.data = New JavaScriptSerializer().Deserialize(Of Object)(resultJson)
        App_Info.sdkv = "2.7.5"
        App_Info.appname = "点歌"
        App_Info.author = "网中行"
        App_Info.describe = "点歌插件"
        App_Info.appv = "1.0.0"
        GC.KeepAlive(appEnableFunc)
        App_Info.useproaddres = Marshal.GetFunctionPointerForDelegate(appEnableFunc).ToInt64()
        GC.KeepAlive(appEnableFunc)
        App_Info.friendmsaddres = Marshal.GetFunctionPointerForDelegate(funRecvicePrivateMsg).ToInt64()
        GC.KeepAlive(funRecviceGroupMsg)
        App_Info.groupmsaddres = Marshal.GetFunctionPointerForDelegate(funRecviceGroupMsg).ToInt64()
        GC.KeepAlive(funEvent)
        App_Info.eventmsaddres = Marshal.GetFunctionPointerForDelegate(funEvent).ToInt64()
        GC.KeepAlive(AppSettingEvent)
        App_Info.setproaddres = Marshal.GetFunctionPointerForDelegate(AppSettingEvent).ToInt64()
        GC.KeepAlive(AppUninstallEvent)
        App_Info.unitproaddres = Marshal.GetFunctionPointerForDelegate(AppUninstallEvent).ToInt64()
        GC.KeepAlive(AppDisabledEvent)
        App_Info.banproaddres = Marshal.GetFunctionPointerForDelegate(AppDisabledEvent).ToInt64()
        Dim jsonstring As String = New JavaScriptSerializer().Serialize(App_Info)
        Return Marshal.StringToHGlobalAnsi(jsonstring)
    End Function
    Public Shared Function AddPermission(ByVal desc As String, ByVal json As String) As String
        Dim Permission = New MyData With {
            .PermissionList = New Needapilist With {
                .state = "1",
                .safe = "1",
                .desc = desc
            }
        }
        Dim serializer As New JavaScriptSerializer()
        Dim jsonstring = serializer.Serialize(Permission).Replace("PermissionList", desc)
        If json = "" Then Return jsonstring Else Return (json + jsonstring).Replace("}{", ",")
    End Function
#End Region
#Region "插件启动"
    Public Shared appEnableFunc As DelegateAppEnable = New DelegateAppEnable(AddressOf appEnable)
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DelegateAppEnable() As UInteger
    Public Shared Function appEnable() As UInteger
        AddHandler Application.ThreadException, AddressOf Application_ThreadException
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf CurrentDomain_UnhandledException
        InitFunction()
        'Dim res As String = CallGetLoginQQ()
        Return 0
    End Function
#End Region
#Region "插件卸载"
    Public Shared AppUninstallEvent As DelegateAppUnInstall = New DelegateAppUnInstall(AddressOf AppUnInstall)
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DelegateAppUnInstall() As UInteger
    Public Shared Function AppUnInstall() As UInteger
        Return 0
    End Function
    Public Sub Disposed()
        GC.SuppressFinalize(Me)
    End Sub
#End Region
#Region "插件禁用"
    Public Shared AppDisabledEvent As DelegateAppDisabled = New DelegateAppDisabled(AddressOf appDisable)
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DelegateAppDisabled() As UInteger
    Public Shared Function appDisable() As UInteger
        Return 0
    End Function
#End Region
#Region "取框架QQ"
    Public Shared Function CallGetLoginQQ() As String
        Dim RetJson = GetLoginQQ(plugin_key)
        Try
            Dim root = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Dictionary(Of String, Object)))(RetJson)
            Dim QQlist = root(root.Keys(0))
            For i = 0 To root.Count
                If QQlist.Keys(i) = "111111111" Then   '控制插件被滥用,如果不是该QQ号码登录就禁用发送信息功能
                    RobotQQ = QQlist.Keys(i)
                    PluginStatus = True
                    Return RetJson
                ElseIf QQlist.Keys(i) <> "22222222" Then
                    RobotQQ = QQlist.Keys(i)
                    PluginStatus = True
                    Return RetJson
                ElseIf QQlist.Keys(i) <> "3333333" Then
                    RobotQQ = QQlist.Keys(i)
                    PluginStatus = True
                    Return RetJson
                End If
            Next
        Catch ex As Exception

        End Try
        PluginStatus = False
        Return ""
    End Function
#End Region
#Region "插件设置"
    Public Shared AppSettingEvent As DelegateAppSetting = New DelegateAppSetting(AddressOf AppSetting)
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub DelegateAppSetting()
    Public Shared Sub AppSetting()
        'Dim func As New QQMsg.ShowForm()
        'func.FormShow()
        Dim frm As New Form1
        frm.Show()
    End Sub
#End Region
#Region "插件事件"
    Public Shared funEvent As DelegatefunEvent = New DelegatefunEvent(AddressOf OnEvent)
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub DelegatefunEvent(ByRef EvenType As EventTypeBase)
    Public Shared Sub OnEvent(ByRef EvenType As EventTypeBase)

        Select Case EvenType.EventType
            Case EventTypeEnum.This_SignInSuccess
                Console.WriteLine("登录成功")
                RobotQQ = EvenType.ThisQQ
            Case EventTypeEnum.Friend_NewFriend
                Console.WriteLine("有新好友")
            Case EventTypeEnum.Friend_FriendRequest
                Console.WriteLine("好友请求")
            Case EventTypeEnum.Friend_FriendRequestAccepted
                Console.WriteLine("对方同意了您的好友请求")
            Case EventTypeEnum.Friend_FriendRequestRefused
                Console.WriteLine("对方拒绝了您的好友请求")
            Case EventTypeEnum.Friend_Removed
                Console.WriteLine("被好友删除")
            Case EventTypeEnum.Friend_Blacklist
                'Console.WriteLine("被拉入黑名单")
            Case EventTypeEnum.Group_MemberVerifying
                Console.WriteLine("加群验证")
            Case EventTypeEnum.Group_Invited
                Console.WriteLine("我被邀请加入群")
            Case EventTypeEnum.Group_MemberJoined
                Console.WriteLine("某人加入了群")
            Case EventTypeEnum.Group_MemberQuit
                Console.WriteLine("某人退出了群")
            Case EventTypeEnum.Group_MemberUndid
                Console.WriteLine("群会员撤回消息")
            Case EventTypeEnum.Group_MemberInvited
                Console.WriteLine("某人被邀请入群")
            Case EventTypeEnum.Group_AllowUploadFile
                Console.WriteLine("群事件_允许上传群文件")
            Case EventTypeEnum.Group_ForbidUploadFile
                Console.WriteLine("群事件_禁止上传群文件")
            Case EventTypeEnum.Group_AllowUploadPicture
                Console.WriteLine("群事件_允许上传相册")
            Case EventTypeEnum.Group_ForbidUploadPicture
                Console.WriteLine("群事件_禁止上传相册")
            Case Else
                Console.WriteLine(EvenType.EventType.ToString())
        End Select


    End Sub
#End Region
#Region "发送好友图片"
    Public Function SendFriendImage(ByVal thisQQ As Long, ByVal friendQQ As Long, ByVal picpath As String, ByVal is_flash As Boolean) As String
        Dim bitmap As New Bitmap(picpath)
        Dim picture() As Byte = GetByteArrayByImage(bitmap)
        Dim piccode As String = Marshal.PtrToStringAnsi(UploadFriendImage(plugin_key, thisQQ, friendQQ, is_flash, picture, picture.Length))
        Dim MessageRandom As Long = 0
        Dim MessageReq As UInteger = 0
        Return SendPrivateMsg(plugin_key, thisQQ, friendQQ, piccode, MessageRandom, MessageReq)
    End Function
    Private Function GetByteArrayByImage(ByVal bitmap As Bitmap) As Byte()
        Dim result() As Byte = Nothing
        Try
            Dim memoryStream As New MemoryStream()
            bitmap.Save(memoryStream, ImageFormat.Jpeg)
            Dim array(memoryStream.Length - 1) As Byte
            memoryStream.Position = 0L
            memoryStream.Read(array, 0, CInt(memoryStream.Length))
            memoryStream.Close()
            result = array
        Catch
            result = Nothing
        End Try
        Return result
    End Function

#End Region
#Region "发送群图片"
    Public Function SendGroupImage(ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal picpath As String, ByVal is_flash As Boolean) As String
        Dim bitmap As New Bitmap(picpath)
        Dim picture() As Byte = GetByteArrayByImage(bitmap)
        Dim piccode As String = Marshal.PtrToStringAnsi(UploadGroupImage(plugin_key, thisQQ, groupQQ, is_flash, picture, picture.Length))
        Return SendGroupMsg(plugin_key, thisQQ, groupQQ, piccode, False)
    End Function
#End Region
#Region "获取图片地址"
    Public Shared Function GetImageLink(ByVal thisQQ As Long, ByVal sendQQ As Long, ByVal groupQQ As Long, ByVal ImgGuid As String) As String
        Dim ImgUrl As String = GetImageDownloadLink(plugin_key, ImgGuid, thisQQ, groupQQ)
        If groupQQ = 0 Then
            Dim MessageRandom As Long = 0
            Dim MessageReq As UInteger = 0
            SendPrivateMsg(plugin_key, thisQQ, sendQQ, "图片地址为:" & ImgUrl & vbCrLf, MessageRandom, MessageReq)
        Else
            SendGroupMsg(plugin_key, thisQQ, groupQQ, "图片地址为:" & ImgUrl & vbCrLf, False)
        End If
        Return ImgUrl
    End Function
#End Region
#Region "取好友列表        "
    Public Shared Function GetFriendLists(ByVal thisQQ As Long, ByVal sendQQ As Long) As Integer
        Dim ptrArray(1) As DataArray
        Dim count As Integer = GetFriendList(plugin_key, thisQQ, ptrArray)
        If count > 0 Then
            Dim list As New List(Of String)()
            Dim pAddrBytes() As Byte = ptrArray(0).pAddrList
            For i As Integer = 0 To count - 1
                Dim readByte(3) As Byte
                Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length)
                Dim StuctPtr As New IntPtr(BitConverter.ToInt32(readByte, 0))
                Dim info As FriendInfo = CType(Marshal.PtrToStructure(StuctPtr, GetType(FriendInfo)), FriendInfo)
                list.Add(info.QQNumber.ToString() & "-" & info.Name)
            Next i
            Dim MessageRandom As Long = 0
            Dim MessageReq As UInteger = 0
            SendPrivateMsg(plugin_key, thisQQ, sendQQ, "好友列表:" & vbCrLf & String.Join(vbCrLf, list), MessageRandom, MessageReq)
        End If
        Return count
    End Function
#End Region
#Region "查询好友信息"
    Public Shared Sub GetFriendData(ByVal thisQQ As Long, ByVal otherQQ As Long)
        Dim MessageRandom As Long = 0
        Dim MessageReq As UInteger = 0
        Dim pFriendInfo(1) As GetFriendDataInfo
        Dim res As Boolean = GetFriendInfo(plugin_key, thisQQ, otherQQ, pFriendInfo)
        If res = True Then
            Dim result = (New JavaScriptSerializer()).Serialize(pFriendInfo(0).friendInfo)
            SendPrivateMsg(plugin_key, thisQQ, otherQQ, result, MessageRandom, MessageReq)
        Else
            SendPrivateMsg(plugin_key, thisQQ, otherQQ, "查询好友信息失败", MessageRandom, MessageReq)
        End If
    End Sub
#End Region
#Region "取群成员列表"
    Public Shared Function GetGroupMemberlists(ByVal thisQQ As Long, ByVal groupQQ As Long) As Integer
        Dim ptrArray(1) As DataArray
        Dim count As Integer = GetGroupMemberlist(plugin_key, thisQQ, groupQQ, ptrArray)
        If count > 0 Then
            Dim list As New List(Of String)()
            Dim pAddrBytes() As Byte = ptrArray(0).pAddrList
            For i As Integer = 0 To count - 1
                Dim readByte(3) As Byte
                Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length)
                Dim StuctPtr As New IntPtr(BitConverter.ToInt32(readByte, 0))
                Dim info As GroupMemberInfo = CType(Marshal.PtrToStructure(StuctPtr, GetType(GroupMemberInfo)), GroupMemberInfo)
                list.Add(info.QQNumber & "-" & info.Name)
            Next i
            SendGroupMsg(plugin_key, thisQQ, groupQQ, "群列表:" & vbCrLf & String.Join(vbCrLf, list), False)
        End If
        Return count
    End Function
#End Region
#Region "取群列表"
    Public Shared Function GetGroupLists(ByVal thisQQ As Long, ByVal groupQQ As Long) As Integer
        Dim ptrArray(1) As DataArray
        Dim count As Integer = GetGroupList(plugin_key, thisQQ, ptrArray)
        If count > 0 Then
            Dim list As New List(Of String)()
            Dim pAddrBytes() As Byte = ptrArray(0).pAddrList
            For i As Integer = 0 To count - 1
                Dim readByte(3) As Byte
                Array.Copy(pAddrBytes, i * 4, readByte, 0, readByte.Length)
                Dim StuctPtr As New IntPtr(BitConverter.ToInt32(readByte, 0))
                Dim info As GroupInfo = CType(Marshal.PtrToStructure(StuctPtr, GetType(GroupInfo)), GroupInfo)
                list.Add(info.GroupID.ToString() & "-" & info.GroupName)
            Next i
            SendGroupMsg(plugin_key, thisQQ, groupQQ, "群列表:" & vbCrLf & String.Join(vbCrLf, list), False)
        End If
        Return count
    End Function
#End Region
#Region "取管理列表"
    Public Function GetAdministratorLists(ByVal thisQQ As Long, ByVal gruopNumber As Long) As String()
        Dim ret = Marshal.PtrToStringAnsi(GetAdministratorList(plugin_key, thisQQ, gruopNumber))
        Dim adminlist() As String = ret.Split(New String() {vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
        Return adminlist
    End Function
#End Region
#Region "查询群信息"
    Public Shared Sub GetGroupData(ByVal thisQQ As Long, ByVal otherGroupQQ As Long)
        Dim MessageRandom As Long = 0
        Dim MessageReq As UInteger = 0
        Dim pGroupInfo(1) As GroupCardInfoDatList
        Dim res As Boolean = GetGroupInfo(plugin_key, thisQQ, otherGroupQQ, pGroupInfo)
        If res = True Then
            Dim result = (New JavaScriptSerializer()).Serialize(pGroupInfo(0).groupCardInfo)
            SendPrivateMsg(plugin_key, thisQQ, otherGroupQQ, result, MessageRandom, MessageReq)
        Else
            SendPrivateMsg(plugin_key, thisQQ, otherGroupQQ, "查询好友信息失败", MessageRandom, MessageReq)
        End If
    End Sub
#End Region
#Region "取群文件列表"
    Public Delegate Function GetGroupFileLists(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String, ByRef groupFileInfoDataLists() As GroupFileInfoDataList) As String
    Public Function GetGroupFileListEvent(ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal folder As String) As List(Of GroupFileInformation)

        Dim pdatalist(1) As GroupFileInfoDataList
        Dim ret As String = GetGroupFileList(plugin_key, thisQQ, groupQQ, folder, pdatalist)
        If pdatalist(0).Amount > 0 Then
            Dim list As New List(Of GroupFileInformation)()
            Dim i As Integer = 0
            Do While i < pdatalist(0).Amount
                Dim recbyte(3) As Byte
                Array.Copy(pdatalist(0).pAddrList, i * 4, recbyte, 0, recbyte.Length)
                Dim pStruct As New IntPtr(BitConverter.ToInt32(recbyte, 0))
                Dim gf As GroupFileInformation = CType(Marshal.PtrToStructure(pStruct, GetType(GroupFileInformation)), GroupFileInformation)
                list.Add(gf)
                i += 1
            Loop
            Return list
        End If
        Return Nothing
    End Function
#End Region
#Region "全局异常"
    Private Shared Sub Application_ThreadException(ByVal sender As Object, ByVal e As System.Threading.ThreadExceptionEventArgs)
        Dim ex As Exception = e.Exception
        MessageBox.Show(String.Format("捕获到未处理异常：{0}" & vbCrLf & "异常信息：{1}" & vbCrLf & "异常堆栈：{2}", ex.GetType(), ex.Message, ex.StackTrace))
    End Sub
    Shared Sub CurrentDomain_UnhandledException(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim ex As Exception = TryCast(e.ExceptionObject, Exception)
        MessageBox.Show(String.Format("捕获到未处理异常：{0}" & vbCrLf & "异常信息：{1}" & vbCrLf & "异常堆栈：{2}" & vbCrLf & "CLR即将退出：{3}", ex.GetType(), ex.Message, ex.StackTrace, e.IsTerminating))
    End Sub
#End Region
#Region "初始化传入的函数指针"
    Public Shared Sub InitFunction()
        Dim jss As New JavaScriptSerializer()
        Dim json As Object = jss.Deserialize(Of Object)(jsonstr)
        Dim ReStartAPI As RestartDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("框架重启"))), GetType(RestartDelegate)), RestartDelegate)
        restart = ReStartAPI
        GC.KeepAlive(restart)
        Dim GetLoginQQAPI As GetLoginQQDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取框架QQ"))), GetType(GetLoginQQDelegate)), GetLoginQQDelegate)
        GetLoginQQ = GetLoginQQAPI
        GC.KeepAlive(GetLoginQQ)
        Dim SendPrivateMsgAPI As SendPrivateMsgDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("发送好友消息"))), GetType(SendPrivateMsgDelegate)), SendPrivateMsgDelegate)
        SendPrivateMsg = SendPrivateMsgAPI
        GC.KeepAlive(SendPrivateMsg)
        Dim SendGroupMsgAPI As SendGroupMsgDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("发送群消息"))), GetType(SendGroupMsgDelegate)), SendGroupMsgDelegate)
        SendGroupMsg = SendGroupMsgAPI
        GC.KeepAlive(SendGroupMsg)
        Dim FriendverificationEventAPI As FriendverificationEventDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("处理好友验证事件"))), GetType(FriendverificationEventDelegate)), FriendverificationEventDelegate)
        FriendverificationEvent = FriendverificationEventAPI
        GC.KeepAlive(FriendverificationEvent)
        Dim GroupVerificationEventAPI As GroupVerificationEventDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("处理群验证事件"))), GetType(GroupVerificationEventDelegate)), GroupVerificationEventDelegate)
        GroupVerificationEvent = GroupVerificationEventAPI
        GC.KeepAlive(GroupVerificationEvent)
        Dim UploadFriendImageAPI As UploadFriendImageDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("上传好友图片"))), GetType(UploadFriendImageDelegate)), UploadFriendImageDelegate)
        UploadFriendImage = UploadFriendImageAPI
        GC.KeepAlive(UploadFriendImage)
        Dim UploadGroupImageAPI As UploadGroupImageDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("上传群图片"))), GetType(UploadGroupImageDelegate)), UploadGroupImageDelegate)
        UploadGroupImage = UploadGroupImageAPI
        GC.KeepAlive(UploadGroupImage)
        Dim UploadFriendAudioAPI As UploadFriendAudioDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("上传好友语音"))), GetType(UploadFriendAudioDelegate)), UploadFriendAudioDelegate)
        UploadFriendAudio = UploadFriendAudioAPI
        GC.KeepAlive(UploadFriendAudio)
        Dim UploadGroupAudioAPI As UploadGroupAudioDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("上传群语音"))), GetType(UploadGroupAudioDelegate)), UploadGroupAudioDelegate)
        UploadGroupAudio = UploadGroupAudioAPI
        GC.KeepAlive(UploadGroupAudio)
        Dim GetImageDownloadLinkAPI As GetImageDownloadLinkDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取图片下载地址"))), GetType(GetImageDownloadLinkDelegate)), GetImageDownloadLinkDelegate)
        GetImageDownloadLink = GetImageDownloadLinkAPI
        GC.KeepAlive(GetImageDownloadLink)
        Dim GetFriendListAPI As GetFriendListDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取好友列表"))), GetType(GetFriendListDelegate)), GetFriendListDelegate)
        GetFriendList = GetFriendListAPI
        GC.KeepAlive(GetFriendList)
        Dim GetFriendInfoAPI As GetFriendInfoDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("查询好友信息"))), GetType(GetFriendInfoDelegate)), GetFriendInfoDelegate)
        GetFriendInfo = GetFriendInfoAPI
        GC.KeepAlive(GetFriendInfo)
        Dim GetGroupMemberlistAPI As GetGroupMemberlistDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取群成员列表"))), GetType(GetGroupMemberlistDelegate)), GetGroupMemberlistDelegate)
        GetGroupMemberlist = GetGroupMemberlistAPI
        GC.KeepAlive(GetGroupMemberlist)
        Dim GetGroupListAPI As GetGroupListDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取群列表"))), GetType(GetGroupListDelegate)), GetGroupListDelegate)
        GetGroupList = GetGroupListAPI
        GC.KeepAlive(GetGroupList)
        Dim GetGroupInfoAPI As GetGroupInfoDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("查询群信息"))), GetType(GetGroupInfoDelegate)), GetGroupInfoDelegate)
        GetGroupInfo = GetGroupInfoAPI
        GC.KeepAlive(GetGroupInfo)
        Dim UndoPrivateAPI As PrivateUndoDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("撤回消息_私聊本身"))), GetType(PrivateUndoDelegate)), PrivateUndoDelegate)
        Undo_PrivateEvent = UndoPrivateAPI
        GC.KeepAlive(Undo_PrivateEvent)
        Dim UndoGroupApi As UndoGroupDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("撤回消息_群聊"))), GetType(UndoGroupDelegate)), UndoGroupDelegate)
        Undo_GroupEvent = UndoGroupApi
        GC.KeepAlive(Undo_GroupEvent)
        Dim GetAdministratorListAPI As GetAdministratorListDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取管理层列表"))), GetType(GetAdministratorListDelegate)), GetAdministratorListDelegate)
        GetAdministratorList = GetAdministratorListAPI
        GC.KeepAlive(GetAdministratorList)
        Dim SendFriendJSONMessageAPI As SendFriendJSONMessageDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("发送好友json消息"))), GetType(SendFriendJSONMessageDelegate)), SendFriendJSONMessageDelegate)
        SendFriendJSONMessage = SendFriendJSONMessageAPI
        GC.KeepAlive(SendFriendJSONMessage)
        Dim SendGroupJSONMessageAPI As SendGroupJSONMessageDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("发送群json消息"))), GetType(SendGroupJSONMessageDelegate)), SendGroupJSONMessageDelegate)
        SendGroupJSONMessage = SendGroupJSONMessageAPI
        GC.KeepAlive(SendGroupJSONMessage)
        Dim SaveFileToWeiYunAPI As SaveFileToWeiYunDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("保存文件到微云"))), GetType(SaveFileToWeiYunDelegate)), SaveFileToWeiYunDelegate)
        SaveFileToWeiYun = SaveFileToWeiYunAPI
        GC.KeepAlive(SaveFileToWeiYun)
        Dim ReadForwardedChatHistoryAPI As ReadForwardedChatHistoryDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("查看转发聊天记录内容"))), GetType(ReadForwardedChatHistoryDelegate)), ReadForwardedChatHistoryDelegate)
        ReadForwardedChatHistory = ReadForwardedChatHistoryAPI
        GC.KeepAlive(ReadForwardedChatHistory)
        Dim UploadGroupFileAPI As UploadGroupFileDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("上传群文件"))), GetType(UploadGroupFileDelegate)), UploadGroupFileDelegate)
        UploadGroupFile = UploadGroupFileAPI
        GC.KeepAlive(UploadGroupFile)
        Dim GetGroupFileListAPI As GetGroupFileListDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("取群文件列表"))), GetType(GetGroupFileListDelegate)), GetGroupFileListDelegate)
        GetGroupFileList = GetGroupFileListAPI
        GC.KeepAlive(GetGroupFileList)
        Dim ShareMusicAPI As ShareMusicDelegate = CType(Marshal.GetDelegateForFunctionPointer(New IntPtr(CInt(json("分享音乐"))), GetType(ShareMusicDelegate)), ShareMusicDelegate)
        ShareMusic = ShareMusicAPI
        GC.KeepAlive(ShareMusic)
    End Sub
#End Region
#Region "函数委托指针"
    '输出日志
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function OutputLog(ByVal pkey As String, <MarshalAs(UnmanagedType.LPStr)> ByVal message As String, ByVal text_color As Integer, ByVal background_color As Integer) As IntPtr
    '发送好友消息
    Public Shared SendPrivateMsg As SendPrivateMsgDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendPrivateMsgDelegate(ByVal pkey As String, ByVal ThisQQ As Long, ByVal SenderQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal MessageContent As String, ByRef MessageRandom As Long, ByRef MessageReq As UInteger) As IntPtr
    '发送群消息
    Public Shared SendGroupMsg As SendGroupMsgDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendGroupMsgDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal msgcontent As String, ByVal anonymous As Boolean) As IntPtr
    '撤回私人消息
    Public Shared Undo_PrivateEvent As PrivateUndoDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function PrivateUndoDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, ByVal message_random As Long, ByVal message_req As Integer, ByVal time As Integer) As Boolean
    '撤回群消息
    Public Shared Undo_GroupEvent As UndoGroupDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UndoGroupDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal message_random As Long, ByVal message_req As Integer) As Boolean
    '收到网络图片
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function RecviceimageDelegate(ByVal pkey As String, ByVal guid As String, ByVal thisQQ As Long, ByVal groupQQ As Long) As IntPtr
    '获取好友列表
    Public Shared GetFriendList As GetFriendListDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetFriendListDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByRef DataInfo() As DataArray) As Integer
    '获取群列表
    Public Shared GetGroupList As GetGroupListDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupListDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByRef DataInfo() As DataArray) As Integer
    '获取群会员列表
    Public Shared GetGroupMemberlist As GetGroupMemberlistDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupMemberlistDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByRef DataInfo() As DataArray) As Integer
    '获取管理员列表
    Public Shared GetAdministratorList As GetAdministratorListDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetAdministratorListDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal gruopQQ As Long) As IntPtr
    '设置管理员
    Public Shared SetupAdministrator As SetupAdministratorDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetupAdministratorDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal gruopQQ As Long, ByVal memberQQ As Long, ByVal SetupOrCancel As Boolean) As IntPtr
    '重启框架
    Public Shared restart As RestartDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub RestartDelegate(ByVal pkey As String)
    '获取框架QQ
    Public Shared GetLoginQQ As GetLoginQQDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetLoginQQDelegate(ByVal pkey As String) As IntPtr
    '处理好友验证事件
    Public Shared FriendverificationEvent As FriendverificationEventDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub FriendverificationEventDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal triggerQQ As Long, ByVal message_seq As Long, ByVal operate_type As FriendVerificationOperateEnum)
    '处理群验证事件
    Public Shared GroupVerificationEvent As GroupVerificationEventDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupVerificationEventDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal source_groupQQ As Long, ByVal triggerQQ As Long, ByVal message_seq As Long, ByVal operate_type As GroupVerificationOperateEnum, ByVal event_type As EventTypeEnum, <MarshalAs(UnmanagedType.LPStr)> ByVal refuse_reason As String) As Boolean
    '获取图片下载链接
    Public Shared GetImageDownloadLink As GetImageDownloadLinkDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetImageDownloadLinkDelegate(ByVal pkey As String, ByVal guid As String, ByVal thisQQ As Long, ByVal groupQQ As Long) As IntPtr
    '查询好友信息
    Public Shared GetFriendInfo As GetFriendInfoDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetFriendInfoDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, ByRef friendInfos() As GetFriendDataInfo) As Boolean
    '查询群信息
    Public Shared GetGroupInfo As GetGroupInfoDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupInfoDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherGroupQQ As Long, ByRef GroupInfos() As GroupCardInfoDatList) As Boolean
    '取群名片
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupCardInfoDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherGroupQQ As Long, ByRef groupCardInfo() As GroupCardInfoDatList) As Boolean
    '设置群名片
    Public Shared SetupGroupCardInfo As SetupGroupCardInfoDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetupGroupCardInfoDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherGroupQQ As Long, ByVal memberQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal newCard As String) As Boolean
    '创建群文件夹
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function CreateGroupFolderDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String) As IntPtr
    '发送好友json消息
    Public Shared SendFriendJSONMessage As SendFriendJSONMessageDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendFriendJSONMessageDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal json_content As String) As IntPtr
    '发送群json消息
    Public Shared SendGroupJSONMessage As SendGroupJSONMessageDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendGroupJSONMessageDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal json_content As String, ByVal anonymous As Boolean) As IntPtr
    '发送免费礼物
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendFreeGiftDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal otherQQ As Long, ByVal gift As Integer) As IntPtr
    '删除群成员
    Public Shared DeleteGroupMember As DeleteGroupMemberDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DeleteGroupMemberDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal memberQQ As Long, ByVal ifAddAgain As Boolean) As Boolean
    '删除好友
    Public Shared DeleteFriend As DeleteFriendDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DeleteFriendDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long) As Boolean
    '发送临时消息
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendGroupTemporaryMessage(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal otherQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal content As String, ByRef random As Long, ByRef req As Integer) As IntPtr
    '查看转发聊天记录内容
    Public Shared ReadForwardedChatHistory As ReadForwardedChatHistoryDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub ReadForwardedChatHistoryDelegate(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal resID As String, <MarshalAs(UnmanagedType.LPStr)> ByRef retPtr As String)
    '分享音乐
    Public Shared ShareMusic As ShareMusicDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ShareMusicDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal music_name As String, <MarshalAs(UnmanagedType.LPStr)> ByVal artist_name As String, <MarshalAs(UnmanagedType.LPStr)> ByVal redirect_link As String, <MarshalAs(UnmanagedType.LPStr)> ByVal cover_link As String, <MarshalAs(UnmanagedType.LPStr)> ByVal file_path As String, ByVal app_type As Integer, ByVal share_type As Integer) As Boolean
    '更改群聊消息内容
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ModifyGroupMessageContent(ByVal pkey As String, <MarshalAs(UnmanagedType.SysInt)> ByVal data_pointer As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal new_message_content As String) As Boolean
    '更改私聊消息内容
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ModifyPrivateMessageContent(ByVal pkey As String, <MarshalAs(UnmanagedType.SysInt)> ByVal data_pointer As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal new_message_content As String) As Boolean
    '群聊画图红包
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupDrawRedEnvelope(ByVal pkey As String, ByVal thisQQ As Long, ByVal total_number As Integer, ByVal total_amount As Integer, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal question As String, <MarshalAs(UnmanagedType.LPStr)> ByVal payment_password As String, ByVal card_serial As Integer, ByRef captchaInfo() As GetCaptchaInfoDataList) As IntPtr
    '好友普通红包
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function FriendNormalRedEnvelope(ByVal pkey As String, ByVal thisQQ As Long, ByVal total_number As Integer, ByVal total_amount As Integer, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal question As String, ByVal skinID As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal payment_password As String, ByVal card_serial As Integer, ByRef ciDataLists() As GetCaptchaInfoDataList) As IntPtr
    ' 好友文件转发至好友
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function FriendFileToFriend(ByVal pkey As String, ByVal thisQQ As Long, ByVal sourceQQ As Long, ByVal targetQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal fileID As String, <MarshalAs(UnmanagedType.LPStr)> ByVal file_name As String, ByVal file_size As Long, ByRef msgReq As Integer, ByRef Random As Long, ByRef time As Integer) As Boolean
    ' 获取插件目录
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetPluginDataDirectory(ByVal pkey As String) As IntPtr
    ' 获取ClientKey
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetClientKey(ByVal pkey As String, ByVal thisQQ As Long) As IntPtr
    ' 获取PSKey
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetPSKey(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal domain As String) As IntPtr
    ' 获取订单信息
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetOrderDetail(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal orderID As String, ByRef data() As OrderDetaildDataList) As IntPtr
    ' 解散群
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DissolveGroup(ByVal pkey As String, ByVal thisQQ As Long, ByVal gruopNumber As Long) As Boolean
    ' 强制取昵称
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetNameForce(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long) As IntPtr
    ' 取QQ钱包个人信息
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetQQWalletPersonalInformation(ByVal pkey As String, ByVal thisQQ As Long, ByRef qQWalletInfoDataLists() As QQWalletInfoDataList) As IntPtr
    ' 从缓存获取昵称
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetNameFromCache(ByVal pkey As String, ByVal otherQQ As Long) As IntPtr
    ' 取群名片
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupNickname(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal otherQQ As Long) As IntPtr
    '获取群文件列表
    Public Shared GetGroupFileList As GetGroupFileListDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetGroupFileListDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String, ByRef groupFileInfoDataLists() As GroupFileInfoDataList) As IntPtr
    ' 群权限_新成员查看历史消息
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupPermission_SetInviteMethod(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal method As Integer) As Boolean
    ' 转发群文件至好友
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ForwardGroupFileToFriend(ByVal pkey As String, ByVal thisQQ As Long, ByVal source_groupQQ As Long, ByVal target_groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal fileID As String, <MarshalAs(UnmanagedType.LPStr)> ByVal filename As String, ByVal filesize As Long, ByRef msgReq As Integer, ByRef Random As Long, ByRef time As Integer) As Boolean
    ' 群文件转发至群
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ForwardGroupFileToGroup(ByVal pkey As String, ByVal thisQQ As Long, ByVal source_groupQQ As Long, ByVal target_groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal fileID As String) As Boolean
    ' 删除群文件
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DeleteGroupFile(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal file_id As String, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String) As IntPtr
    ' 删除群文件夹
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DeleteGroupFolder(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String) As IntPtr
    ' 重命名群文件夹
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function RenameGroupFolder(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal old_folder As String, <MarshalAs(UnmanagedType.LPStr)> ByVal new_folder As String) As IntPtr
    ' 上传群文件
    Public Shared UploadGroupFile As UploadGroupFileDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadGroupFileDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal path As String, <MarshalAs(UnmanagedType.LPStr)> ByVal folder As String) As IntPtr
    ' 移动群文件
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function MoveGroupFile(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal file_id As String, <MarshalAs(UnmanagedType.LPStr)> ByVal old_folder As String, <MarshalAs(UnmanagedType.LPStr)> ByVal new_folder As String) As IntPtr
    ' 上传好友图片
    Public Shared UploadFriendImage As UploadFriendImageDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadFriendImageDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long, ByVal is_flash As Boolean, <MarshalAs(UnmanagedType.LPArray)> ByVal pic() As Byte, ByVal picsize As Integer) As IntPtr
    ' 上传群图片
    Public Shared UploadGroupImage As UploadGroupImageDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadGroupImageDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long, ByVal is_flash As Boolean, <MarshalAs(UnmanagedType.LPArray)> ByVal pic() As Byte, ByVal picsize As Integer) As IntPtr
    ' 上传群头像
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadGroupAvatar(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPArray)> ByVal pic() As Byte, ByVal picsize As Integer) As Boolean
    ' 上传头像
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadAvatar(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPArray)> ByVal pic() As Byte, ByVal picsize As Integer) As IntPtr
    ' 上传好友语音
    Public Shared UploadFriendAudio As UploadFriendAudioDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadFriendAudioDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long, ByVal audio_type As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal audio_text As String, <MarshalAs(UnmanagedType.LPArray)> ByVal audio() As Byte, ByVal audiosize As Integer) As IntPtr
    ' 上传群语音
    Public Shared UploadGroupAudio As UploadGroupAudioDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function UploadGroupAudioDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal friendQQ As Long, ByVal audio_type As Integer, <MarshalAs(UnmanagedType.LPStr)> ByVal audio_text As String, <MarshalAs(UnmanagedType.LPArray)> ByVal audio() As Byte, ByVal audiosize As Integer) As IntPtr
    ' 保存文件到微云
    Public Shared SaveFileToWeiYun As SaveFileToWeiYunDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SaveFileToWeiYunDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal file_id As String) As IntPtr
    '禁言群成员
    Public Shared MuteGroupMember As MuteGroupMemberDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function MuteGroupMemberDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal memberQQ As Long, ByVal muteTime As UInteger) As Boolean
    '全员禁言
    Public Shared MuteGroupAll As MuteGroupAllDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function MuteGroupAllDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal ifOpen As Boolean) As Boolean
    '是否被禁言
    Public Shared IfMuted As IfMutedDelegate = Nothing
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function IfMutedDelegate(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long) As Boolean
    ' 上报当前位置
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function ReportCurrent(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal Longitude As Double, ByVal Latitude As Double) As Boolean
    ' 设置群名片
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetGroupNickname(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal otherQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal nickname As String) As IntPtr
    ' 设置位置共享
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetLocationShare(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal Longitude As Double, ByVal Latitude As Double, ByVal is_enabled As Boolean) As Boolean
    ' 设置在线状态
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetStatus(ByVal pkey As String, ByVal thisQQ As Long, ByVal main As Integer, ByVal sun As Integer, ByVal battery As Integer) As Boolean
    ' 设置专属头衔
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function Setexclusivetitle(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal otherQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal name As String) As Boolean
    ' 添加好友
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function AddFriend(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal verification As String, <MarshalAs(UnmanagedType.LPStr)> ByVal comment As String) As IntPtr
    ' 添加群
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function AddGroup(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal verification As String) As IntPtr
    ' 退群
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function QuitGroup(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long) As Boolean
    ' 修改个性签名
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetSignature(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal signature As String, <MarshalAs(UnmanagedType.LPStr)> ByVal location As String) As Boolean
    ' 修改昵称
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetName(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal name As String) As Boolean
    ' 置屏蔽好友
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetBlockFriend(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, ByVal is_blocked As Boolean) As IntPtr
    ' 置群消息接收
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetGroupMessageReceive(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal set_type As Integer) As Boolean
    ' 置特别关心好友
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetSpecialFriend(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, ByVal is_special As Boolean) As IntPtr
    ' 提交支付验证码
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SubmitPaymentCaptcha(ByVal pkey As String, ByVal thisQQ As Long, ByVal captcha_information As IntPtr, <MarshalAs(UnmanagedType.LPStr)> ByVal captcha As String, <MarshalAs(UnmanagedType.LPStr)> ByVal payment_password As String) As IntPtr
    ' 登录指定QQ
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function LoginSpecifyQQ(ByVal pkey As String, ByVal otherQQ As Long) As Boolean
    ' 发送输入状态
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SendIMEStatus(ByVal pkey As String, ByVal thisQQ As Long, ByVal ohterQQ As Long, ByVal iMEStatus As Integer) As Boolean
    ' api是否有权限
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function CheckPermission(ByVal pkey As String, ByVal permission As Integer) As Boolean
    ' QQ点赞
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function QQLike(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long) As IntPtr
    ' 修改资料
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function Modifyinformation(ByVal pkey As String, ByVal thisQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal json As String) As Boolean
    ' 取群未领红包
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GetRedEnvelope(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByRef reDataList() As RedEnvelopesDataList) As IntPtr
    ' 打好友电话
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Sub CallPhone(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long)
    ' 取群文件下载地址
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupFileDownloadLink(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal FileID As String, <MarshalAs(UnmanagedType.LPStr)> ByVal FileName As String) As IntPtr
    ' 头像双击_群
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function DoubleclickGroupFace(ByVal pkey As String, ByVal thisQQ As Long, ByVal otherQQ As Long, ByVal groupQQ As Long) As Boolean
    ' 群聊置顶
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupTop(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByVal istop As Boolean) As Boolean
    ' 设为精华
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetEssence(ByVal pkey As String, ByVal thisQQ As Long, ByVal groupQQ As Long, ByVal message_req As Integer, ByVal message_random As Long) As Boolean
    ' 群权限_设置群昵称规则
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetGroupNickRules(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, <MarshalAs(UnmanagedType.LPWStr)> ByVal rules As String) As Boolean
    ' 群权限_设置群发言频率
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function SetGroupLimitNumber(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByVal LimitNumber As Integer) As Boolean
    ' 群权限_设置群查找方式
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function FriendjoinGroup(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByVal otherQQ As Long, ByVal otherGroupQQ As Long) As Boolean
    ' 置群内消息通知
    <UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet:=CharSet.Ansi)>
    Public Delegate Function GroupNoticeMethod(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByVal otherQQ As Long, ByVal metohd As Integer) As Boolean
    ' 修改群名称
    Public Delegate Function GetGroupMemberBriefInfo(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, ByRef gMBriefDataLists() As GMBriefDataList) As IntPtr
    Public Delegate Function UpdataGroupName(ByVal pkey As String, ByVal thisQQ As Long, ByVal GroupQQ As Long, <MarshalAs(UnmanagedType.LPStr)> ByVal NewGroupName As String) As Boolean

#End Region

End Class
