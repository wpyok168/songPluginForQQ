Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

Public Module Pinvoke
#Region "全局变量"
    Public jsonstr, plugin_key As String
    Public PluginStatus As Boolean
    Public RobotQQ As String

#End Region


#Region "结构体"
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure EventTypeBase
        Public ThisQQ As Long
        Public SourceGroupQQ As Long
        Public OperateQQ As Long
        Public TriggerQQ As Long
        Public MessageSeq As Long
        Public MessageTimestamp As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public SourceGroupName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public OperateQQName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public TriggerQQName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public MessageContent As String
        Public EventType As EventTypeEnum
        Public EventSubType As UInteger
    End Structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure PrivateMessageEvent
        Public SenderQQ As Long
        Public ThisQQ As Long
        Public MessageReq As UInteger
        Public MessageSeq As Long
        Public MessageReceiveTime As UInteger
        Public MessageGroupQQ As Long
        Public MessageSendTime As UInteger
        Public MessageRandom As Long
        Public MessageClip As UInteger
        Public MessageClipCount As UInteger
        Public MessageClipID As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public MessageContent As String
        Public BubbleID As UInteger
        Public MessageType As MessageTypeEnum
        Public MessageSubType As MessageSubTypeEnum
        Public MessageSubTemporaryType As MessageSubTypeEnum
        Public RedEnvelopeType As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public SessionToken As String
        Public SourceEventQQ As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public SourceEventQQName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileID As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileMD5 As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileName As String
        Public FileSize As Long
    End Structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure GroupMessageEvent
        Public SenderQQ As Long
        Public ThisQQ As Long
        Public MessageReq As UInteger
        Public MessageReceiveTime As UInteger
        Public MessageGroupQQ As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public SourceGroupName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public SenderNickname As String
        Public MessageSendTime As UInteger
        Public MessageRandom As Long
        Public MessageClip As UInteger
        Public MessageClipCount As UInteger
        Public MessageClipID As Long
        Public MessageType As MessageTypeEnum
        <MarshalAs(UnmanagedType.LPStr)>
        Public SenderTitle As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public MessageContent As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public ReplyMessageContent As String
        Public BubbleID As UInteger
        Public ThisQQAnonymousID As UInteger
        Public reserved_ As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileID As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileMD5 As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public FileName As String
        Public FileSize As Long
        Public MessageAppID As UInteger
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure FriendDataList
        Public count As UInteger
        Public pFriendInfo As FriendInfo
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure ServiceInfo
        Public ServiceList As ServiceInformation
        Public ServiceLevel As UInteger
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GetFriendDataInfo '一维数组
        Public friendInfo As FriendInfo
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GetGroupDataInfo '一维数组
        Public GroupInfo As GroupInfo
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure DataArray
        Public index As UInteger
        Public Amount As UInteger
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1024)>
        Public pAddrList() As Byte
    End Structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure FriendInfo
        <MarshalAs(UnmanagedType.LPStr)>
        Public Email As String
        Public QQNumber As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public Name As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Note As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Status As String
        Public Likes As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public Signature As String
        Public Gender As UInteger
        Public Level As UInteger
        Public Age As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public Nation As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Province As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public City As String
        Public serviceInfos As ServiceInfo
        Public ContinuousOnlineTime As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public QQTalent As String
        Public LikesToday As UInteger
        Public LikesAvailableToday As UInteger
    End Structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure GroupMemberInfo
        <MarshalAs(UnmanagedType.LPStr)>
        Public QQNumber As String
        Public Age As UInteger
        Public Gender As UInteger
        <MarshalAs(UnmanagedType.LPStr)>
        Public Name As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Email As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Nickname As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Note As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Title As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public Phone As String
        Public TitleTimeout As Long
        Public ShutUpTimestamp As Long
        Public JoinTime As Long
        Public ChatTime As Long
        Public Level As Long
    End Structure
    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Ansi, Pack:=1)>
    Public Structure GroupInfo
        Public GroupID As Long
        Public GroupQQ As Long
        Public CFlag As Long
        Public GroupInfoSeq As Long
        Public GroupFlagExt As Long
        Public GroupRankSeq As Long
        Public CertificationType As Long
        Public ShutUpTimestamp As Long
        Public ThisShutUpTimestamp As Long
        Public CmdUinUinFlag As Long
        Public AdditionalFlag As Long
        Public GroupTypeFlag As Long
        Public GroupSecType As Long
        Public GroupSecTypeInfo As Long
        Public GroupClassExt As Long
        Public AppPrivilegeFlag As Long
        Public SubscriptionUin As Long
        Public GroupMemberCount As Long
        Public MemberNumSeq As Long
        Public MemberCardSeq As Long
        Public GroupFlagExt3 As Long
        Public GroupOwnerUin As Long
        Public IsConfGroup As Long
        Public IsModifyConfGroupFace As Long
        Public IsModifyConfGroupName As Long
        Public CmduinJoinTime As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public GroupName As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public GroupMemo As String
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GroupCardInfoDatList
        Public groupCardInfo As GroupCardInfo
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GroupCardInfo
        ' 群名称
        <MarshalAs(UnmanagedType.LPTStr)>
        Public GroupName As String
        ' 群地点
        <MarshalAs(UnmanagedType.LPTStr)>
        Public GroupLocation As String
        ' 群分类
        <MarshalAs(UnmanagedType.LPTStr)>
        Public GroupClassification As String
        ' 群标签 以|分割
        <MarshalAs(UnmanagedType.LPTStr)>
        Public GroupTags As String
        ' 群介绍
        <MarshalAs(UnmanagedType.LPTStr)>
        Public GroupDescription As String
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GroupFileInfoDataList
        Public index As UInteger '数组索引
        Public Amount As UInteger '数组元素数量
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1024)>
        Public pAddrList() As Byte '每个元素的指针
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure OrderDetaildDataList
        Public orderDetail As OrderDetail
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure OrderDetail
        ' 订单时间
        <MarshalAs(UnmanagedType.LPStr)>
        Public OrderTime As String
        ' 订单说明			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OrderDescription As String
        ' 订单类名			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OrderClassification As String
        ' 订单类型			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OrderType As String
        ' 订单手续费			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OrderCommission As String
        ' 操作人QQ			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OperatorQQ As String
        ' 操作人昵称			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OperatorName As String
        ' 接收人QQ			
        <MarshalAs(UnmanagedType.LPStr)>
        Public ReceiverQQ As String
        ' 接收人昵称			
        <MarshalAs(UnmanagedType.LPStr)>
        Public ReceiverName As String
        ' 操作金额			
        <MarshalAs(UnmanagedType.LPStr)>
        Public OperateAmount As String
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure QQWalletInfoDataList
        Public qQWalletInformation As QQWalletInformation
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure QQWalletInformation
        ' 余额
        <MarshalAs(UnmanagedType.LPStr)>
        Public Balance As String
        ' 身份证号
        <MarshalAs(UnmanagedType.LPStr)>
        Public ID As String
        ' 实名
        <MarshalAs(UnmanagedType.LPStr)>
        Public RealName As String
        ' 银行卡列表
        Public CardList() As CardInfoDataList
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure CardInfoDataList
        Public index As Integer '数组索引
        Public Amount As Integer '数组元素数量
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=100)>
        Public pAddrList() As Byte '每个元素的指针
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure CardInformation
        ' 序列
        Public Serial As Integer
        ' 尾号
        <MarshalAs(UnmanagedType.LPStr)>
        Public TailNumber As String
        ' 银行
        <MarshalAs(UnmanagedType.LPStr)>
        Public Bank As String
        ' 绑定手机
        <MarshalAs(UnmanagedType.LPStr)>
        Public BindPhone As String
        ' bind_serial
        <MarshalAs(UnmanagedType.LPStr)>
        Public BindSerial As String
        ' bank_type
        <MarshalAs(UnmanagedType.LPStr)>
        Public BankType As String
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure RetQQWalletInformation
        ' 余额
        <MarshalAs(UnmanagedType.LPStr)>
        Public Balance As String
        ' 身份证号
        <MarshalAs(UnmanagedType.LPStr)>
        Public ID As String
        ' 实名
        <MarshalAs(UnmanagedType.LPStr)>
        Public RealName As String
        ' 银行卡列表
        Public CardList As List(Of CardInformation)
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure RedEnvelopesDataList
        'public int index;
        Public notReRedEnvelopes As NotReRedEnvelopes
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure NotReRedEnvelopes
        Public SourceQQ As Long
        <MarshalAs(UnmanagedType.LPStr)>
        Public listid As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public authkey As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public title As String
        <MarshalAs(UnmanagedType.LPStr)>
        Public channel As String
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GroupFileInformation
        Public FileID As IntPtr '文件夹fileid或者文件fileid
        Public FileName As IntPtr ' 文件夹名或文件名
        Public FileSize As Long ' 文件大小，文件夹时表示有多少个文件
        Public FileMd5 As IntPtr ' 文件md5，文件夹时为空，部分文件类型也可能是空
        Public FileFromUin As Long ' 创建文件夹或上传文件的QQ
        Public FileFromNick As IntPtr ' 创建文件夹或上传文件的QQ
        Public FileType As FiletypeEnum ' 文件类型 1: 文件, 2: 文件夹
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GMBriefDataList
        Public groupMemberBriefInfo As GMBriefInfo
    End Structure

    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GMBriefInfo
        Public GroupMAax As UInteger
        Public GruoupNum As UInteger
        Public GroupOwner As Long
        Public AdminiList As IntPtr
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure AdminListDataList
        Public index As Integer '数组索引
        Public Amount As Integer '数组元素数量
        'public byte[] pdatalist;
        <MarshalAs(UnmanagedType.ByValArray, SizeConst:=1024)>
        Public pdatalist() As Long
    End Structure
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Class GroupMemberBriefInfo
        Public GroupMAax As UInteger
        Public GruoupNum As UInteger
        Public GroupOwner As Long
        Public AdminiList() As Long
    End Class
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure GetCaptchaInfoDataList
        'public int index;
        Public CaptchaInfo As CaptchaInformation
    End Structure
    ''' <summary>
    ''' 验证码信息
    ''' </summary>
    <StructLayout(LayoutKind.Sequential, Pack:=1)>
    Public Structure CaptchaInformation
        ' token_id			
        <MarshalAs(UnmanagedType.LPStr)>
        Public TokenID As String
        ' skey		
        <MarshalAs(UnmanagedType.LPStr)>
        Public SKey As String
        ' bank_type			
        <MarshalAs(UnmanagedType.LPStr)>
        Public BankType As String
        ' mobile			
        <MarshalAs(UnmanagedType.LPStr)>
        Public Mobile As String
        ' business_type			
        <MarshalAs(UnmanagedType.LPStr)>
        Public BusinessType As String
        ' random			
        Public Random As Integer
        ' transaction_id			
        <MarshalAs(UnmanagedType.LPStr)>
        Public TransactionID As String
        ' purchaser_id			
        <MarshalAs(UnmanagedType.LPStr)>
        Public PurchaserID As String
        ' token			
        <MarshalAs(UnmanagedType.LPStr)>
        Public Token As String
        ' auth_params			
        <MarshalAs(UnmanagedType.LPStr)>
        Public AuthParams As String
    End Structure
    Public Structure AppInfo
        Public sdkv As String
        Public appname As String
        Public author As String
        Public describe As String
        Public appv As String
        Public groupmsaddres As Long
        Public friendmsaddres As Long
        Public eventmsaddres As Long
        Public unitproaddres As Long
        Public setproaddres As Long
        Public useproaddres As Long
        Public banproaddres As Long
        Public data As Object
    End Structure
#End Region

#Region "枚举"
    Public Enum FiletypeEnum
        file = 1
        folder = 2
    End Enum

    Public Enum FriendVerificationOperateEnum
        Agree = 1 '同意
        Deny = 2 '拒绝
    End Enum
    Public Enum GroupVerificationOperateEnum
        Agree = 11 '同意
        Deny = 12 '拒绝
        Ignore = 14 '忽略
    End Enum
    Public Enum EventTypeEnum
        Friend_Removed = 100 ' 好友事件_被好友删除
        Friend_SignatureChanged = 101 ' 好友事件_签名变更
        Friend_NameChanged = 102 ' 好友事件_昵称改变
        Friend_UserUndid = 103 ' 好友事件_某人撤回事件
        Friend_NewFriend = 104 ' 好友事件_有新好友
        Friend_FriendRequest = 105 ' 好友事件_好友请求
        Friend_FriendRequestAccepted = 106 '好友事件_对方同意了您的好友请求
        Friend_FriendRequestRefused = 107 ' 好友事件_对方拒绝了您的好友请求
        Friend_InformationLiked = 108 ' 好友事件_资料卡点赞
        Friend_SignatureLiked = 109 ' 好友事件_签名点赞
        Friend_SignatureReplied = 110 ' 好友事件_签名回复
        Friend_TagLiked = 111 ' 好友事件_个性标签点赞
        Friend_StickerLiked = 112 ' 好友事件_随心贴回复
        Friend_StickerAdded = 113 ' 好友事件_随心贴增添
        Friend_Blacklist = 64 ' 好友事件_加入黑名单
        Group_Invited = 1 ' 群事件_我被邀请加入群
        Group_MemberJoined = 2 ' 群事件_某人加入了群
        Group_MemberVerifying = 3 ' 群事件_某人申请加群
        Group_GroupDissolved = 4 ' 群事件_群被解散
        Group_MemberQuit = 5 ' 群事件_某人退出了群
        Group_MemberKicked = 6 ' 群事件_某人被踢出群
        Group_MemberShutUp = 7 ' 群事件_某人被禁言
        Group_MemberUndid = 8 ' 群事件_某人撤回事件
        Group_AdministratorTook = 9 ' 群事件_某人被取消管理
        Group_AdministratorGave = 10 ' 群事件_某人被赋予管理
        Group_EnableAllShutUp = 11 ' 群事件_开启全员禁言
        Group_DisableAllShutUp = 12 ' 群事件_关闭全员禁言
        Group_EnableAnonymous = 13 ' 群事件_开启匿名聊天
        Group_DisableAnonymous = 14 ' 群事件_关闭匿名聊天
        Group_EnableChatFrankly = 15 ' 群事件_开启坦白说
        Group_DisableChatFrankly = 16 ' 群事件_关闭坦白说
        Group_AllowGroupTemporary = 17 ' 群事件_允许群临时会话
        Group_ForbidGroupTemporary = 18 ' 群事件_禁止群临时会话
        Group_AllowCreateGroup = 19 ' 群事件_允许发起新的群聊
        Group_ForbidCreateGroup = 20 ' 群事件_允许发起新的群聊
        Group_AllowUploadFile = 21 ' 群事件_允许上传群文件
        Group_ForbidUploadFile = 22 ' 群事件_禁止上传群文件
        Group_AllowUploadPicture = 23 ' 群事件_允许上传相册
        Group_ForbidUploadPicture = 24 ' 群事件_禁止上传相册
        Group_MemberInvited = 25 ' 群事件_某人被邀请入群
        Group_ShowMemberTitle = 26 ' 群事件_展示成员群头衔
        Group_HideMemberTitle = 27 ' 群事件_隐藏成员群头衔
        Group_MemberNotShutUp = 28 ' 群事件_某人被解除禁言
        QZone_Related = 29 ' 空间事件_与我相关
        Group_MemberKickOut = 30 ' 群事件_我被踢出
        This_SignInSuccess = 31 ' 框架事件_登录成功
        Group_GroupNameUpdate = 32 ' 群事件_群名变更
    End Enum
    Public Enum PermissionEnum
        OutputLog = 0 ' 输出日志
        SendFriendMessage = 1 ' 发送好友消息
        SendGroupMessage = 2 ' 发送群消息
        SendGroupTemporaryMessage = 3 ' 发送群临时消息
        AddFriend = 4 ' 添加好友
        AddGroup = 5 ' 添加群
        RemoveFriend = 6 ' 删除好友！
        SetBlockFriend = 7 ' 置屏蔽好友！
        SetSpecialFriend = 8 ' 置特别关心好友
        SendFriendJSONMessage = 11 ' 发送好友json消息
        SendGroupJSONMessage = 12 ' 发送群json消息
        UploadFriendPicture = 13 ' 上传好友图片
        UploadGroupPicture = 14 ' 上传群图片
        UploadFriendAudio = 15 ' 上传好友语音
        UploadGroupAudio = 16 ' 上传群语音
        UploadAvatar = 17 ' 上传头像！
        SetGroupMemberNickname = 18 ' 设置群名片
        GetNameFromCache = 19 ' 取昵称_从缓存
        GetNameForce = 20 ' 强制取昵称
        GetSKey = 21 ' 获取skey！
        GetPSKey = 22 ' 获取pskey！
        GetClientKey = 23 ' 获取clientkey！
        GetThisQQ = 24 ' 取框架QQ
        GetFriendList = 25 ' 取好友列表
        GetGroupList = 26 ' 取群列表
        GetGroupMemberList = 27 ' 取群成员列表
        SetAdministrator = 28 ' 设置管理员
        GetAdministratorList = 29 ' 取管理层列表
        GetGroupMemberNickname = 30 ' 取群名片
        GetSignature = 31 ' 取个性签名
        SetName = 32 ' 修改昵称！
        SetSignature = 33 ' 修改个性签名！
        KickGroupMember = 34 ' 删除群成员
        BanGroupMember = 35 ' 禁言群成员
        QuitGroup = 36 ' 退群！
        DissolveGroup = 37 ' 解散群！
        UploadGroupAvatar = 38 ' 上传群头像
        BanAll = 39 ' 全员禁言
        Group_Create = 40 ' 群权限_发起新的群聊
        Group_CreateTemporary = 41 ' 群权限_发起临时会话
        Group_UploadFile = 42 ' 群权限_上传文件
        Group_UploadPicture = 43 ' 群权限_上传相册
        Group_InviteFriend = 44 ' 群权限_邀请好友加群
        Group_Anonymous = 45 ' 群权限_匿名聊天
        Group_ChatFrankly = 46 ' 群权限_坦白说
        Group_NewMemberReadChatHistory = 47 ' 群权限_新成员查看历史消息
        Group_SetInviteMethod = 48 ' 群权限_邀请方式设置
        Undo_Group = 49 ' 撤回消息_群聊
        Undo_Private = 50 ' 撤回消息_私聊本身
        SetLocationShare = 51 ' 设置位置共享
        ReportCurrentLocation = 52 ' 上报当前位置
        IsShutUp = 53 ' 是否被禁言
        ProcessFriendVerification = 54 ' 处理好友验证事件
        ProcessGroupVerification = 55 ' 处理群验证事件
        ReadForwardedChatHistory = 56 ' 查看转发聊天记录内容
        UploadGroupFile = 57 ' 上传群文件
        CreateGroupFolder = 58 ' 创建群文件夹
        SetStatus = 59 ' 设置在线状态
        QQLike = 60 ' QQ点赞！
        GetImageDownloadLink = 61 ' 取图片下载地址
        QueryFriendInformation = 63 ' 查询好友信息
        QueryGroupInformation = 64 ' 查询群信息
        Reboot = 65 ' 框架重启！
        GroupFileForwardToGroup = 66 ' 群文件转发至群
        GroupFileForwardToFriend = 67 ' 群文件转发至好友
        FriendFileForwardToFriend = 68 ' 好友文件转发至好友
        SetGroupMessageReceive = 69 ' 置群消息接收
        GetGroupNameFromCache = 70 ' 取群名称_从缓存
        SendFreeGift = 71 ' 发送免费礼物
        GetFriendStatus = 72 ' 取好友在线状态
        GetQQWalletPersonalInformation = 73 ' 取QQ钱包个人信息！
        GetOrderDetail = 74 ' 获取订单详情
        SubmitPaymentCaptcha = 75 ' 提交支付验证码
        ShareMusic = 77 ' 分享音乐
        ModifyGroupMessageContent = 78 ' 更改群聊消息内容！
        ModifyPrivateMessageContent = 79 ' 更改私聊消息内容！
        GroupPasswordRedEnvelope = 80 ' 群聊口令红包
        GroupRandomRedEnvelope = 81 ' 群聊拼手气红包
        GroupNormalRedEnvelope = 82 ' 群聊普通红包
        GroupDrawRedEnvelope = 83 ' 群聊画图红包
        GroupAudioRedEnvelope = 84 ' 群聊语音红包
        GroupFollowRedEnvelope = 85 ' 群聊接龙红包
        GroupExclusiveRedEnvelope = 86 ' 群聊专属红包
        FriendPasswordRedEnvelope = 87 ' 好友口令红包
        FriendNormalRedEnvelope = 88 ' 好友普通红包
        FriendDrawRedEnvelope = 89 ' 好友画图红包
        FriendAudioRedEnvelope = 90 ' 好友语音红包
        FriendFollowRedEnvelope = 91 ' 好友接龙红包
    End Enum
    Public Enum StatusOnlineTypeEnum
        Normal = 0
        Battery = 1000
        WeakSignal = 1011
        Sleeping = 1016
        Gaming = 1017
        Studying = 1018
        Eating = 1019
        WatchingTVSeries = 1021
        OnVacation = 1022
        OnlineStudying = 1024
        TravelAtHome = 1025
        TiMiing = 1027
        ListeningToMusic = 1028
        StayingUpLate = 1032
        PlayingBall = 1050
        FallInLove = 1051
        ImOK = 1052
    End Enum
    Public Enum StatusTypeEnum
        Online = 11
        Away = 31
        Invisible = 41
        Busy = 50
        TalkToMe = 60
        DoNotDisturb = 70
    End Enum
    Public Enum MessageTypeEnum
        Temporary = 141 ' 临时会话
        FriendUsualMessage = 166 ' 好友通常消息
        FriendFile = 529 ' 好友文件
        FriendAudio = 208 ' 好友语音
        GroupRedEnvelope = 78 ' 群红包
        GroupUsualMessage = 134 ' 群聊通常消息
    End Enum
    Public Enum MessageSubTypeEnum
        Temporary_Group = 0 ' 临时会话_群
        Temporary_PublicAccount = 129 ' 临时会话_公众号
        Temporary_WebQQConsultation = 201 ' 临时会话_网页QQ咨询
    End Enum
    Public Enum ServiceInformation
        SVIP = 1 'SVIP
        VIDEO_VIP = 4 '视频会员
        MUSIC_PACK = 6 '音乐包
        STAR = 105 'star
        YELLOW_DIAMOND = 102 '黄钻
        GREEN_DIAMOND = 103 '绿钻
        RED_DIAMOND = 101 '红钻
        YELLOWLOVE = 104 'yellowlove
        SVIP_WITH_VIDEO = 107 'SVIP&视频会员
        SVIP_WITH_GREEN = 109 'SVIP&绿钻
        SVIP_WITH_MUSIC = 110 'SVIP&音乐包
    End Enum
    Public Enum FreeGiftEnum
        Gift_280 = 280 ' 牵你的手
        Gift_281 = 281 ' 可爱猫咪
        Gift_284 = 284 ' 神秘面具
        Gift_285 = 285 ' 甜wink
        Gift_286 = 286 ' 我超忙的
        Gift_289 = 289 ' 快乐肥宅水
        Gift_290 = 290 ' 幸运手链
        Gift_299 = 299 ' 卡布奇诺
        Gift_302 = 302 ' 猫咪手表
        Gift_307 = 307 ' 绒绒手套
        Gift_308 = 308 ' 彩虹糖果
        Gift_312 = 312 ' 爱心口罩
        Gift_313 = 313 ' 坚强
        Gift_367 = 367 ' 告白话筒
    End Enum
    Public Enum EventProcessEnum
        Block = 1
        Ignore = 0
    End Enum
    Public Enum AudioTypeEnum
        Normal = 0 ' 普通语音
        Change = 1 ' 变声语音
        Text = 2 ' 文字语音
        Match = 3 ' (红包)匹配语音
    End Enum
    Public Enum MusicAppTypeEnum
        QQMusic = 0
        XiaMiMusic = 1
        KuWoMusic = 2
        KuGouMusic = 3
        WangYiMusic = 4
    End Enum
    Public Enum MusicShare_Type
        PrivateMsg = 0
        GroupMsg = 1
    End Enum
    Public Enum ApiPermissionEnum 'Api权限列表
        <Description("输出日志")>
        ApiPermission0
        <Description("发送好友消息")>
        ApiPermission1
        <Description("发送群消息")>
        ApiPermission2
        <Description("发送群临时消息")>
        ApiPermission3
        <Description("添加好友")>
        ApiPermission4
        <Description("添加群")>
        ApiPermission5
        <Description("删除好友")>
        ApiPermission6
        <Description("置屏蔽好友")>
        ApiPermission7
        <Description("置特别关心好友")>
        ApiPermission8
        <Description("发送好友json消息")>
        ApiPermission9
        <Description("发送群json消息")>
        ApiPermission10
        <Description("上传好友图片")>
        ApiPermission11
        <Description("上传群图片")>
        ApiPermission12
        <Description("上传好友语音")>
        ApiPermission13
        <Description("上传群语音")>
        ApiPermission14
        <Description("上传头像")>
        ApiPermission15
        <Description("设置群名片")>
        ApiPermission16
        <Description("取昵称_从缓存")>
        ApiPermission17
        <Description("强制取昵称")>
        ApiPermission18
        <Description("获取skey")>
        ApiPermission19
        <Description("获取pskey")>
        ApiPermission20
        <Description("获取clientkey")>
        ApiPermission21
        <Description("取框架QQ")>
        ApiPermission22
        <Description("取好友列表")>
        ApiPermission23
        <Description("取群列表")>
        ApiPermission24
        <Description("取群成员列表")>
        ApiPermission25
        <Description("设置管理员")>
        ApiPermission26
        <Description("取管理层列表")>
        ApiPermission27
        <Description("取群名片")>
        ApiPermission28
        <Description("取个性签名")>
        ApiPermission29
        <Description("修改昵称")>
        ApiPermission30
        <Description("修改个性签名")>
        ApiPermission31
        <Description("删除群成员")>
        ApiPermission32
        <Description("禁言群成员")>
        ApiPermission33
        <Description("退群")>
        ApiPermission34
        <Description("解散群")>
        ApiPermission35
        <Description("上传群头像")>
        ApiPermission36
        <Description("全员禁言")>
        ApiPermission37
        <Description("群权限_发起新的群聊")>
        ApiPermission38
        <Description("群权限_发起临时会话")>
        ApiPermission39
        <Description("群权限_上传文件")>
        ApiPermission40
        <Description("群权限_上传相册")>
        ApiPermission41
        <Description("群权限_邀请好友加群")>
        ApiPermission42
        <Description("群权限_匿名聊天")>
        ApiPermission43
        <Description("群权限_坦白说")>
        ApiPermission44
        <Description("群权限_新成员查看历史消息")>
        ApiPermission45
        <Description("群权限_邀请方式设置")>
        ApiPermission46
        <Description("撤回消息_群聊")>
        ApiPermission47
        <Description("撤回消息_私聊本身")>
        ApiPermission48
        <Description("设置位置共享")>
        ApiPermission49
        <Description("上报当前位置")>
        ApiPermission50
        <Description("是否被禁言")>
        ApiPermission51
        <Description("处理群验证事件")>
        ApiPermission52
        <Description("处理好友验证事件")>
        ApiPermission53
        <Description("查看转发聊天记录内容")>
        ApiPermission54
        <Description("上传群文件")>
        ApiPermission55
        <Description("创建群文件夹")>
        ApiPermission56
        <Description("设置在线状态")>
        ApiPermission57
        <Description("api是否有权限")>
        ApiPermission58
        <Description("重载自身")>
        ApiPermission59
        <Description("取插件数据目录")>
        ApiPermission60
        <Description("QQ点赞")>
        ApiPermission61
        <Description("取图片下载地址")>
        ApiPermission62
        <Description("查询好友信息")>
        ApiPermission63
        <Description("查询群信息")>
        ApiPermission64
        <Description("框架重启")>
        ApiPermission65
        <Description("群文件转发至群")>
        ApiPermission66
        <Description("群文件转发至好友")>
        ApiPermission67
        <Description("好友文件转发至好友")>
        ApiPermission68
        <Description("置群消息接收")>
        ApiPermission69
        <Description("取群名称_从缓存")>
        ApiPermission70
        <Description("发送免费礼物")>
        ApiPermission71
        <Description("取好友在线状态")>
        ApiPermission72
        <Description("取QQ钱包个人信息")>
        ApiPermission73
        <Description("获取订单详情")>
        ApiPermission74
        <Description("提交支付验证码")>
        ApiPermission75
        <Description("分享音乐")>
        ApiPermission76
        <Description("更改群聊消息内容")>
        ApiPermission77
        <Description("更改私聊消息内容")>
        ApiPermission78
        <Description("群聊口令红包")>
        ApiPermission79
        <Description("群聊拼手气红包")>
        ApiPermission80
        <Description("群聊普通红包")>
        ApiPermission81
        <Description("群聊画图红包")>
        ApiPermission82
        <Description("群聊语音红包")>
        ApiPermission83
        <Description("群聊接龙红包")>
        ApiPermission84
        <Description("群聊专属红包")>
        ApiPermission85
        <Description("好友口令红包")>
        ApiPermission86
        <Description("好友普通红包")>
        ApiPermission87
        <Description("好友画图红包")>
        ApiPermission88
        <Description("好友语音红包")>
        ApiPermission89
        <Description("好友接龙红包")>
        ApiPermission90
        <Description("重命名群文件夹")>
        ApiPermission91
        <Description("删除群文件夹")>
        ApiPermission92
        <Description("删除群文件")>
        ApiPermission93
        <Description("保存文件到微云")>
        ApiPermission94
        <Description("移动群文件")>
        ApiPermission95
        <Description("取群文件列表")>
        ApiPermission96

    End Enum



#End Region
    Public Class MyData
        Public PermissionList As Needapilist
    End Class
    Public Class Needapilist
        Public Property state As String
        Public Property safe As String
        Public Property desc As String

    End Class



    Public Function GetAt(ByVal qq As Long) As String
        Return $"[@{qq.ToString()}]"
    End Function
    Public Function GetAtAll() As String ' 艾特全体
        Return "[@all]"
    End Function
    Public Function GetFace(ByVal id As UInteger, ByVal name As String) As String  ' 表情
        Return $"[Face,Id={id},name={name}]"
    End Function
    Public Function GetFace(ByVal id As UInteger, ByVal name As String, ByVal hash As String, ByVal flag As String) As String ' 大表情
        Return $"[bigFace,Id={id},name={name},hash={hash},flag={flag}]"
    End Function
    Public Function GetSmallFace(ByVal id As UInteger, ByVal name As String) As String ' 小表情
        Return $"[smallFace,name={name},Id={id}]"
    End Function
    Public Function GetShake(ByVal name As String, ByVal id As UInteger, ByVal type As UInteger) As String ' 抖一抖
        Return $"[Shake,name={name} ,Type={type},Id={id}]"
    End Function
    Public Function GetLimiShow(ByVal name As String, ByVal id As UInteger, ByVal type As UInteger, ByVal QQ As Long) As String ' 厘米秀 目前不支持3D消息
        Return $"[limiShow,Id={id},name={name},type={type},object={QQ}]"
    End Function
    Public Function GetFlashPicFile(ByVal path As String) As String ' 闪照_本地
        Return $"[flashPicFile,path={path}]"
    End Function
    Public Function GetFlashWord(ByVal desc As String, ByVal resid As String, ByVal prompt As String) As String ' 闪字
        Return $"[flashWord,Desc={desc},Resid={resid},Prompt={prompt}]"
    End Function
    Public Function GetHonest(ByVal QQ As Long, ByVal name As String, ByVal desc As String, ByVal time As String, ByVal Random As String, ByVal backgroundtype As String) As String  ' 坦白说
        Return $"[Honest,ToUin={QQ},ToNick={name},Desc={desc},Time={time},Random={Random},Bgtype={backgroundtype}]"
    End Function
    Public Function GetPicFile(ByVal path As String) As String ' 图片_本地
        Return $"[picFile,path={path}]"
    End Function
    Public Function GetGraffiti(ByVal id As UInteger, ByVal hash As String, ByVal address As String) As String ' 涂鸦
        Return $"[Graffiti,ModelId={id},hash={hash},url={address}]"
    End Function
    Public Function Getbq(ByVal id As UInteger) As String ' 小黄豆表情
        Return $"[bq{id}]"
    End Function
    Public Function GetLitleVideo(ByVal linkParam As String, ByVal hash1 As String, ByVal hash2 As String) As String  ' 小视频
        Return $"[litleVideo,linkParam={linkParam},hash1={hash1},hash2={hash2}]"
    End Function
    Public Function GetAudioFile(ByVal path As String) As String ' 语音_本地 必须是silk_v3编码的文件
        Return $"[AudioFile,path={path}]"
    End Function

End Module
