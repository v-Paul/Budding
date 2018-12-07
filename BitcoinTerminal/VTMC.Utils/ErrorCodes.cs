/*************************************************
*Author:zhangdanhong
*Date:2016/12/6 18:48:37
*Des:常量
************************************************/

namespace VTMC.Utils
{
    /// <summary>
    /// 系统常量定义
    /// </summary>
    public static class ErrorCodes
    {
        #region 未知异常
        /// <summary>
        /// MVTM发生了未知异常，请和管理员联系。
        /// </summary>
        public const string E21_999 = "E21999";
        #endregion

        #region  Message Information
        /// <summary>
        /// 空消息内容
        /// </summary>
        public const string E24_9101 = "E249101";

        /// <summary>
        /// {0}成功。
        /// </summary>
        public const string E24_9102 = "E249102";

        /// <summary>
        /// 
        /// </summary>
        public const string E24_9103 = "E249103";

        #endregion

        #region  Message Warnning
        /// <summary>
        /// 空消息内容
        /// </summary>
        public const string E23_9201 = "E239201";

        /// <summary>
        /// VTM已经运行，不能再次打开。
        /// </summary>
        public const string E23_9202 = "E239202";

        #endregion

        #region  Message Question
        /// <summary>
        /// 空消息内容
        /// </summary>
        public const string E24_9401 = "E249401";


        #region EJ
        /// <summary>
        /// Please confirm to start reset this machine, E-J record will be generated and printed.
        /// *Please be aware that this operation can not be reversed. 
        /// 请确认当前清机操作，系统将在确认后生成并打印电子流水。
        /// *请注意该操作不可回退
        /// </summary>
        public const string E24_9402 = "E249402";

        /// <summary>
        /// 读取EJ文件失败。
        /// Failed to read EJ file.
        /// </summary>
        public const string E24_9403 = "E249403";

        /// <summary>
        /// 重置Kiosk失败。
        /// Failed to reset Kiosk.
        /// </summary>
        public const string E24_9404= "E249404";


        /// <summary>
        /// 重新打印EJ失败。
        /// Failed to re-print EJ file.
        /// </summary>
        public const string E24_9405 = "E249405";

        /// <summary>
        /// 完成EJ清机失败。
        /// Failed to complete EJ file.
        /// </summary>
        public const string E24_9406 = "E249406";

        /// <summary>
        /// Do you confirm to re-print current E-J record?
        /// 请确认需要重新打印电子流水？
        /// </summary>
        public const string E24_9407 = "E249407";

        /// <summary>
        /// 重新打印EJ成功。
        /// Re-print EJ file successfully.
        /// </summary>
        public const string E24_9408 = "E249408";

        /// <summary>
        /// 完成EJ清机成功。
        /// Complete EJ file successfully.
        /// </summary>
        public const string E24_9409 = "E249409";

        /// <summary>
        /// 重置Kiosk成功。
        /// Reset Kiosk successfully.
        /// </summary>
        public const string E24_9410 = "E249410";

        /// <summary>
        /// 加载EJ模块失败。
        /// Failed to load EJ model.
        /// </summary>
        public const string E24_9411 = "E249411";


        /// <summary>
        /// 打印EJ文件失败。
        /// Failed to print EJ file.
        /// </summary>
        public const string E24_9413 = "E249413";

        /// <summary>
        /// 请确认需要完成清机任务？
        /// Please confirm to complete EJ file?
        /// </summary>
        public const string E24_9414 = "E249414";

        /// <summary>
        /// 请确认需要退出Supervisor？
        /// Please confirm to exit Supervisor?
        /// </summary>
        public const string E24_9415 = "E249415";

        #region EJLogin
        /// <summary>
        /// 启动Supervisor发生了未知异常，请查看日志。
        /// Filed to launch Supervisor, please check log.
        /// </summary>
        public const string E24_9412 = "E249412";

        #endregion

        #endregion

        /// <summary>
        /// AD Check失败
        /// </summary>
        public const string E23_9501 = "E239501";
        #endregion

        #region 启动检查
        /// <summary>
        /// 远端Web服务器连接失败，请检查服务器链接地址或当前网络环境，如不能恢复请联系管理员。Error code：{0}
        /// </summary>
        public const string E21_9397 = "E219397";

        /// <summary>
        /// MVTM配置不正确,\r配置项:[{0}]\r值:[{1}]\r配置路径不存在。
        /// </summary>
        public const string E21_9398 = "E219398";

        /// <summary>
        /// 空消息内容
        /// </summary>
        public const string E21_9301 = "E219301";

        /// <summary>
        /// 获取服务器Token出现错误。
        /// </summary>
        public const string E21_9302 = "E219302";
        #endregion

        #region Camera Module [1]
        /// <summary>
        /// 拍照启动失败
        /// </summary>
        public const string E23_101 = "E23101";
        /// <summary>
        /// 没有可用的摄像头
        /// </summary>
        public const string E23_102 = "E23102";
        /// <summary>
        /// 指定摄像头名称不存在
        /// </summary>
        public const string E23_103 = "E23103";
        /// <summary>
        /// 关闭摄像头失败
        /// </summary>
        public const string E24_104 = "E24104";
        /// <summary>
        /// 拍照中发生错误。
        /// </summary>
        public const string E23_105 = "E23105";
        /// <summary>
        /// 摄像头被占用。
        /// </summary>
        public const string E23_106 = "E23106";

        /// <summary>
        /// 摄像头故障。
        /// </summary>
        public const string E23_107 = "E23107";

        /// <summary>
        /// 播放音频失败
        /// </summary>
        public const string E23_108 = "E23108";

        /// <summary>
        /// 暂停播放音频失败。
        /// </summary>
        public const string E23_109 = "E23109";

        /// <summary>
        /// 停止播放音频失败。
        /// </summary>
        public const string E23110 = "E23110";

        #region

        #endregion

        #endregion

        #region Common Module[2] 

        ///// <summary>
        ///// 文件上传失败
        ///// </summary>
        //public const string E23_201 = "E23201";

        /// <summary>
        /// 文件下载失败
        /// </summary>
        public const string E23_202 = "E23202";
        /// <summary>
        /// 获取Token失败
        /// </summary>
        public const string E21_203 = "E21203";
        /// <summary>
        /// 设置系统语言失败
        /// </summary>
        public const string E21_204 = "E21204";
        /// <summary>
        /// Base64文件上传失败
        /// </summary>
        public const string E23_205 = "E23205";
        /// <summary>
        /// 前端写Log失败
        /// </summary>
        public const string E20_206 = "E20206";
        /// <summary>
        /// 删除文件失败
        /// </summary>
        public const string E20_207 = "E20207";
        /// <summary>
        /// 删除文件夹失败
        /// </summary>
        public const string E20_208 = "E20208";
        /// <summary>
        /// 文件超大
        /// </summary>
        public const string E23_209 = "E23209";
        /// <summary>
        /// 文件上传失败（多文件异步上传）
        /// </summary>
        public const string E20_210 = "E20210";
        /// <summary>
        /// 获取设备信息失败
        /// </summary>
        public const string E20_211 = "E20211";
        /// <summary>
        /// 设置token值异常
        /// </summary>
        public const string E20_212 = "E20212";
        /// <summary>
        /// 上传文件Token验证失败401
        /// </summary>
        public const string E20_213 = "E20213";
        /// <summary>
        /// 用户名或密码不正确。
        /// </summary>
        public const string E23_214 = "E23214";
        /// <summary>
        /// 远程服务器内部异常。
        /// </summary>
        public const string E23_215 = "E23215";
        /// <summary>
        /// 远程服务器响应异常。
        /// </summary>
        public const string E23_216 = "E23216";
        /// <summary>
        /// 当前用户未登录。
        /// </summary>
        public const string E23_217 = "E23217";
        /// <summary>
        /// 刷新AccessToken失败。
        /// </summary>
        public const string E23_218 = "E23218";
        /// <summary>
        /// 恢复页面缩放到100%失败
        /// </summary>
        public const string E22_219 = "E22219";
        /// <summary>
        /// 将对象序列化为JSON格式出异常
        /// </summary>
        public const string E22_220 = "E22220";
        /// <summary>
        /// 反序列化JSON到给定的匿名对象出异常
        /// </summary>
        public const string E22_221 = "E22221";
        /// <summary>
        /// 传参为空/Null/空格
        /// </summary>
        public const string E22_222 = "E22222";
        /// <summary>
        /// 加密失败
        /// </summary>
        public const string E23_223 = "E23223";


        /// <summary>
        ///文件上传失败（多文件同步上传）
        /// </summary>
        public const string E20_224 = "E20224";
        /// <summary>
        /// 用户名或者密码不能为空
        /// </summary>
        public const string E23_224 = "E23224";
        #endregion

        #region  人脸识别 [4]
        /// <summary>
        /// 传入参数错误
        /// </summary>
        public const string E22_401 = "E22401";

        /// <summary>
        /// 摄像头错误
        /// </summary>
        public const string E22_402 = "E22402";

        /// <summary>
        /// 联网授权错误
        /// </summary>
        public const string E22_403 = "E22403";

        /// <summary>
        /// 其它错误
        /// </summary>
        public const string E22_404 = "E22404";

        /// <summary>
        /// 没有可用的摄像头
        /// </summary>
        public const string E23_405 = "E23405";

        /// <summary>
        /// 联网授权错误
        /// </summary>
        public const string E21_406 = "E21406";

        /// <summary>
        /// 其它错误
        /// </summary>
        public const string E21_407 = "E21407";

        /// <summary>
        /// 初始化人脸识别设备失败
        /// </summary>
        public const string E22_408 = "E22408";

        /// <summary>
        /// 获取摄像头列表失败
        /// </summary>
        public const string E22_409 = "E22409";

        /// <summary>
        /// 人脸识别开始检测启动失败
        /// </summary>
        public const string E22_410 = "E22410";
        /// <summary>
        /// 摄像头被占用
        /// </summary>
        public const string E22_411 = "E22411";
        /// <summary>
        /// 人脸识别结束失败
        /// </summary>
        public const string E20_412 = "E20412";
        #endregion

        #region PDF操作[6] 
        /// <summary>
        /// PDF文件下载失败。
        /// </summary>
        public const string E23_601 = "E23601";
        /// <summary>
        /// PDF文件不存在。
        /// </summary>
        public const string E23_602 = "E23602";
        /// <summary>
        /// PDF文件转图片失败。
        /// </summary>
        public const string E23_603 = "E23603";
        /// <summary>
        /// PDF文件签名保存失败。
        /// </summary>
        public const string E23_604 = "E23604";
        /// <summary>
        /// 客户签名信息保存失败。
        /// </summary>
        public const string E23_605 = "E23605";
        /// <summary>
        /// 签名板打开失败。
        /// </summary>
        public const string E23_606 = "E23606";
        /// <summary>
        /// PDF显示缩放变化失败。
        /// </summary>
        public const string E23_607 = "E23607";
        /// <summary>
        /// PDF显示内容切换失败。
        /// </summary>
        public const string E23_608 = "E23608";
        /// <summary>
        /// 客户签名失败。
        /// </summary>
        public const string E23_609 = "E23609";
        /// <summary>
        /// 客户签名提交失败。
        /// </summary>
        public const string E23_610 = "E23610";
        /// <summary>
        /// PDF滑动失败。
        /// </summary>
        public const string E23_611 = "E23611";
        /// <summary>
        /// PDF签名板初始化失败。
        /// </summary>
        public const string E22_612 = "E22612";
        /// <summary>
        /// PDF签名板大小变化失败。
        /// </summary>
        public const string E22_613 = "E22613";
        /// <summary>
        /// PDF签名板Cancel处理失败。
        /// </summary>
        public const string E23_614 = "E23614";
        /// <summary>
        /// PDF签名板Clear处理失败。
        /// </summary>
        public const string E23_615 = "E23615";
        /// <summary>
        /// PDF签名板Confirm处理失败。
        /// </summary>
        public const string E23_616 = "E23616";
        /// <summary>
        /// PDF签名处理失败。
        /// </summary>
        public const string E23_617 = "E23617";
        /// <summary>
        /// PDF业务终止失败。
        /// </summary>
        public const string E24_618 = "E24618";
        /// <summary>
        /// PDF显示消息窗口失败。
        /// </summary>
        public const string E23_619 = "E23619";
        /// <summary>
        /// 关闭PDF超时消息窗口失败。
        /// </summary>
        public const string E23_620 = "E23620";
        /// <summary>
        /// SMP PDF文件未准备好
        /// </summary>
        public const string E23_621 = "E23621";
        /// <summary>
        /// SMP PDF 目录包含多个PDF文件
        /// </summary>
        public const string E23_622 = "E23622";
        /// <summary>
        /// SMP PDF indicator file已存在，但无PDF文件
        /// </summary>
        public const string E23_623 = "E23623";
        /// <summary>
        /// 获取SMP PDF 路径异常失败
        /// </summary>
        public const string E23_624 = "E23624";
        /// <summary>
        /// SMPPDFSignature 接口异常失败
        /// </summary>
        public const string E23_625 = "E23625";
        /// <summary>
        /// SMP PDF文件大小异常
        /// </summary>
        public const string E23_626 = "E23626";
        /// <summary>
        /// 临时工作目录下PDF文件大小为零
        /// </summary>
        public const string E23_627 = "E23627";

        /// <summary>
        /// PDF处理异常。
        /// </summary>
        public const string E23_699 = "E23699";


        #endregion

        #region 文件系统监控操作[7] 
        /// <summary>
        /// 开始监控文件系统失败。
        /// </summary>
        public const string E22_701 = "E22701";
        /// <summary>
        /// 停止监控文件系统失败。
        /// </summary>
        public const string E22_702 = "E22702";
        /// <summary>
        /// 监控文件夹目录不存在。
        /// </summary>
        public const string E20_703 = "E20703";
        #endregion

        #region PDF操作[8] 
        /// <summary>
        /// 关闭签名窗口失败
        /// </summary>
        public const string E24_801 = "E24801";

        /// <summary>
        /// 打开签名板失败
        /// </summary>
        public const string E23_802 = "E23802";

        /// <summary>
        /// 签名中出现错误。
        /// </summary>
        public const string E23_803 = "E23803";

        /// <summary>
        /// 签名板Clear处理失败。
        /// </summary>
        public const string E23_804 = "E23804";

        /// <summary>
        /// 签名板Cancel处理失败。
        /// </summary>
        public const string E23_805 = "E23805";
        /// <summary>
        /// 保存签名信息失败。
        /// </summary>
        public const string E23_806 = "E23806";
        /// <summary>
        /// 签名板大小变化时出错。
        /// </summary>
        public const string E22_807 = "E22807";
        /// <summary>
        /// 签名板初始化失败。
        /// </summary>
        public const string E22_808 = "E22808";
        /// <summary>
        /// 关闭签名板失败
        /// </summary>
        public const string E22_809 = "E22809";
        #endregion

        #region OCR护照扫描操作[9] 
        /// <summary>
        /// 护照认证失败。
        /// </summary>
        public const string E23_901 = "E23901";
        /// <summary>
        /// 数据传输事件失败。
        /// </summary>
        public const string E23_902 = "E23902";
        /// <summary>
        /// 护照扫描仪连接失败。
        /// </summary>
        public const string E23_903 = "E23903";
        /// <summary>
        /// 护照扫描硬件设置失败。
        /// </summary>
        public const string E23_904 = "E23904";
        /// <summary>
        /// 护照扫描失败。
        /// </summary>
        public const string E23_905 = "E23905";
        /// <summary>
        /// 护照扫描断开连接失败。
        /// </summary>
        public const string E23_906 = "E23906";
        /// <summary>
        /// 护照信息检查不通过。
        /// </summary>
        public const string E23_907 = "E23907";
        /// <summary>
        /// 护照扫描仪未连接。
        /// </summary>
        public const string E23_908 = "E23908";

        #endregion

        #region Sockets操作[10] 
        /// <summary>
        /// 建立通讯通道失败
        /// </summary>
        public const string E23_1001 = "E231001";
        /// <summary>
        /// 关闭通讯通道失败
        /// </summary>
        public const string E24_1002 = "E241002";
        /// <summary>
        /// 发送消息失败
        /// </summary>
        public const string E23_1003 = "E231003";
        /// <summary>
        /// RecordTool通讯异常发生。
        /// </summary>
        public const string E22_1099 = "E221099";

        /// <summary>
        /// 进程启动失败
        /// </summary>
        public const string E23_1004 = "E231004";
        /// <summary>
        /// 录屏进程不存在
        /// </summary>
        public const string E23_1005 = "E231005";
        /// <summary>
        /// record 未OEPN
        /// </summary>
        public const string E23_1006 = "E231006";
        /// <summary>
        /// record已Open
        /// </summary>
        public const string E23_1007 = "E231007";
        /// <summary>
        /// 未stop保存视频
        /// </summary>
        public const string E23_1008 = "E231008";
        /// <summary>
        /// 非法操作，当前操作不可执行。
        /// </summary>
        public const string E23_1009 = "E231009";
        /// <summary>
        /// 当前环境无临时文件
        /// </summary>
        public const string E23_1010 = "E231010";
        ///// <summary>
        ///// Monitor窗口异常
        ///// </summary>
        //public const string E23_1011 = "E231011";
        ///// <summary>
        ///// Record已经发生错误，不能正常录制
        ///// </summary>
        //public const string E23_1012 = "E231012";

        /// <summary>
        /// 视频录制未开始，不可停止
        /// </summary>
        public const string E23_1011 = "E231011";
        /// <summary>
        /// 视频json文件不存在
        /// </summary>
        public const string E23_1012 = "E231012";
        /// <summary>
        /// 视频json文件存在多个
        /// </summary>
        public const string E23_1013 = "E231013";
        /// <summary>
        /// 视频json文件读取失败
        /// </summary>
        public const string E23_1014 = "E231014";
        /// <summary>
        /// OpenRecord接口异常失败
        /// </summary>
        public const string E23_1020 = "E231020";
        /// <summary>
        /// StartRecord接口异常失败
        /// </summary>
        public const string E23_1021 = "E231021";
        /// <summary>
        /// PauseRecord接口异常失败
        /// </summary>
        public const string E23_1022 = "E231022";
        /// <summary>
        /// ContinueRecord接口异常失败
        /// </summary>
        public const string E23_1023 = "E231023";
        /// <summary>
        /// StopRecord接口异常失败
        /// </summary>
        public const string E23_1024 = "E231024";
        /// <summary>
        /// GetStatus接口异常失败
        /// </summary>
        public const string E23_1025 = "E231025";
        /// <summary>
        /// SendCommand接口异常失败
        /// </summary>
        public const string E23_1026 = "E231026";
        /// <summary>
        /// CloseRecord接口异常失败
        /// </summary>
        public const string E23_1027 = "E231027";
        /// <summary>
        /// 清除临时视频文件失败
        /// </summary>
        public const string E23_1028 = "E231028";
        /// <summary>
        /// 添加视频状态接口异常
        /// </summary>
        public const string E23_1029 = "E231029";

        #endregion

        #region 视频监视[11]
        /// <summary>
        /// 打开监视视频窗口失败
        /// </summary>
        public const string E23_1101 = "E231101";
        /// <summary>
        /// 关闭监视视频窗口失败
        /// </summary>
        public const string E23_1102 = "E231102";
        /// <summary>
        /// 没有可用的监视摄像头
        /// </summary>
        public const string E23_1103 = "E231103";

        /// <summary>
        /// 指定监视摄像头名称不存在
        /// </summary>
        public const string E23_1104 = "E231104";

        /// <summary>
        /// 指定监视摄像头被占用。
        /// </summary>
        public const string E23_1105 = "E231105";

        /// <summary>
        /// 监视摄像头状态不正。
        /// </summary>
        public const string E23_1106 = "E231106";

        /// <summary>
        /// 摄像头故障。
        /// </summary>
        public const string E23_1107 = "E231107";
        #endregion

        #region 录制视频[12]
        /// <summary>
        /// 打开视频录制工具VideoRecordTool失败
        /// </summary>
        public const string E22_1201 = "E221201";
        /// <summary>
        /// 关闭视频录制工具VideoRecordTool失败
        /// </summary>
        public const string E22_1202 = "E221202";
        /// <summary>
        /// 开始录制失败
        /// </summary>
        public const string E22_1203 = "E221203";

        /// <summary>
        /// 多次点击开始录制
        /// </summary>
        public const string E22_1204 = "E221204";

        /// <summary>
        /// 暂停/恢复录制失败
        /// </summary>
        public const string E22_1205 = "E221205";
        /// <summary>
        /// 停止录制失败
        /// </summary>
        public const string E22_1206 = "E221206";

        /// <summary>
        /// 不满足命令执行条件的异常
        /// </summary>
        public const string E22_1207 = "E221207";
        /// <summary>
        /// 暂停过程中状态异常
        /// </summary>
        public const string E22_1208 = "E221208";

        /// <summary>
        /// 正在录制中，关闭前请先停止录制Stop！
        /// </summary>
        public const string E22_1209 = "E221209";

        /// <summary>
        /// 关闭录制程序异常
        /// </summary>
        public const string E22_1210 = "E221210";

        /// <summary>
        /// 当前不在录制中Recording状态，不能调用暂停
        /// </summary>
        public const string E22_1211 = "E221211";

        /// <summary>
        /// 当前不在暂停Pause状态，不能调用continue按钮
        /// </summary>
        public const string E22_1212 = "E221212";

        /// <summary>
        /// VideoRecord录屏控件初始化失败
        /// </summary>
        public const string E22_1213 = "E221213";

        ///// <summary>
        ///// 录制摄像头过程中异常
        ///// </summary>
        //public const string E22_1214 = "E221214";

        ///// <summary>
        ///// 录制音频过程中异常
        ///// </summary>
        //public const string E22_1215 = "E221215";
        /// <summary>
        /// 录制视频过程中异常
        /// </summary>
        public const string E22_1216 = "E221216";
        ///// <summary>
        ///// 录制麦克风过程中异常
        ///// </summary>
        //public const string E22_1217 = "E221217";
        /// <summary>
        /// 录像工具处于错误状态，不能做{0}，如正在录制请尝试先做停止后做关闭动作
        /// </summary>
        public const string E22_1218 = "E221218";

        ///// <summary>
        ///// 采集到的视频图像异常
        ///// </summary>
        //public const string E22_1219 = "E221219";
        /// <summary>
        /// 保存的视频时长异常
        /// </summary>
        public const string E22_1220 = "E221220";
        /// <summary>
        /// 保存的视频大小异常
        /// </summary>
        public const string E22_1221 = "E221221";
        /// <summary>
        /// VideoRecord遇到未处理系统的异常
        /// </summary>
        public const string E22_1222 = "E221222";
        /// <summary>
        /// VideoRecord遇到当前程序域无法扑捉异常处理
        /// </summary>
        public const string E22_1223 = "E221223";
        #endregion

        #region SFTP视频上传[13]
        /// <summary>
        /// 开始异步上传文件失败。
        /// </summary>
        public const string E23_1301 = "E231301";

        /// <summary>
        /// 停止上传失败。
        /// </summary>
        public const string E23_1302 = "E231302";

        /// <summary>
        /// 获取当前状态失败。
        /// </summary>
        public const string E22_1303 = "E221303";

        /// <summary>
        /// 连接服务器失败。
        /// </summary>
        public const string E23_1304 = "E231304";

        /// <summary>
        /// 调用更新上传视频状态Java接口返回执行失败。
        /// </summary>
        public const string E23_1305 = "E231305";

        /// <summary>
        /// 调用更新上传视频状态Java接口无法连接或验证不通过。
        /// </summary>
        public const string E23_1306 = "E231306";


        #endregion

        #region 指纹仪[30] 
        /// <summary>
        /// 指纹仪设备打开失败
        /// </summary>
        public const string E23_3001 = "E233001";
        /// <summary>
        /// 指纹仪设备关闭失败
        /// </summary>
        public const string E23_3002 = "E233002";
        /// <summary>
        /// 获取设备状态失败
        /// </summary>
        public const string E23_3003 = "E233003";
        /// <summary>
        /// 重置设备失败
        /// </summary>
        public const string E23_3004 = "E233004";
        /// <summary>
        /// 获取指纹仪的能力集失败
        /// </summary>
        public const string E23_3005 = "E233005";
        /// <summary>
        /// 录取指纹模板失败
        /// </summary>
        public const string E23_3006 = "E233006";
        /// <summary>
        /// 获取指纹特征数据失败
        /// </summary>
        public const string E23_3007 = "E233007";
        /// <summary>
        /// 对照指纹信息失败
        /// </summary>
        public const string E23_3008 = "E233008";
        /// <summary>
        /// 取消操作失败
        /// </summary>
        public const string E23_3009 = "E233009";


        /// <summary>
        /// 设备正忙，请稍后再试
        /// </summary>
        public const string E23_3010 = "E233010";
        /// <summary>
        /// 设备故障正在重启，请稍后再试
        /// </summary>
        public const string E23_3011 = "E233011";

        #region 私有错误

        /// <summary>
        /// WFS_ERR_PTR_NOMEDIAPRESENT
        /// </summary>
        public const string E23_3020 = "E233020";

        /// <summary>
        /// WFS_ERR_PTR_MEDIAOVERFLOW
        /// </summary>
        public const string E23_3021 = "E233021";

        /// <summary>
        /// WFS_ERR_PTR_MEDIANOTFOUND
        /// </summary>
        public const string E23_3022 = "E233022";

        /// <summary>
        /// WFS_ERR_PTR_MEDIAINVALID
        /// </summary>
        public const string E23_3023 = "E233023";

        /// <summary>
        /// WFS_ERR_PTR_SEQUENCEINVALID
        /// </summary>
        public const string E23_3024 = "E233024";

        #endregion

        #endregion

        #region MYKAD大马卡[14]
        /// <summary>
        /// ReadMYKADData接口异常
        /// </summary>
        public const string E23_1401 = "E231401";
        /// <summary>
        /// 对比指纹接口异常
        /// </summary>
        public const string E23_1402 = "E231402";
        /// <summary>
        /// 清除本地临时信息接口异常
        /// </summary>
        public const string E23_1403 = "E231403";
        /// <summary>
        /// 与Mykad建立连接失败
        /// </summary>
        public const string E23_1421 = "E231421";
        /// <summary>
        /// 指纹对比失败
        /// </summary>
        public const string E23_1422 = "E231422";

        //public const string E23_1423 = "E231423";

        #endregion

        #region Offline业务[23]
        /// <summary>
        /// Json文件写入失败。
        /// </summary>
        public const string E23_2301 = "E232301";
        /// <summary>
        /// 文件不存在。
        /// </summary>
        public const string E23_2302 = "E232302";
        /// <summary>
        /// Json文件读取失败。
        /// </summary>
        public const string E23_2303 = "E232303";
        /// <summary>
        /// 文件储存失败。
        /// </summary>
        public const string E23_2304 = "E232304";
        /// <summary>
        /// 文件打开失败。
        /// </summary>
        public const string E23_2305 = "E232305";
        #endregion

        #region 身份证读卡器[31] 

        #region 执行命令错误码

        /// <summary>
        /// 身份证读卡器打开失败
        /// </summary>
        public const string E23_3101 = "E233101";

        /// <summary>
        /// 身份证读卡器关闭失败
        /// </summary>
        public const string E23_3102 = "E233102";

        /// <summary>
        /// 获取身份证读卡器状态失败
        /// </summary>
        public const string E23_3103 = "E233103";

        /// <summary>
        /// 重置身份证读卡器失败
        /// </summary>
        public const string E23_3104 = "E233104";

        /// <summary>
        /// 异步扫描身份证失败
        /// </summary>
        public const string E23_3105 = "E233105";

        /// <summary>
        /// 退卡失败
        /// </summary>
        public const string E23_3106 = "E233106";

        /// <summary>
        /// 取消操作失败
        /// </summary>
        public const string E23_3107 = "E233107";
        /// <summary>
        /// 同步对比指纹失败
        /// </summary>
        public const string E23_3108 = "E233108";
        /// <summary>
        /// 操作超时
        /// </summary>
        public const string E23_3109= "E233109";
        /// <summary>
        /// 未检测到MyKad设备
        /// </summary>
        public const string E23_3110 = "E233110";
        #endregion

        #region 私有错误

        /// <summary>
        /// 卡被夹住需要操作人员干预
        /// </summary>
        public const string E23_3120 = "E233120";

        /// <summary>
        /// 由于操作失误或者硬件故障，而未能成功打开或关闭卡口，需要操作人员的干预
        /// </summary>
        public const string E23_3121 = "E233121";

        /// <summary>
        /// 输入的磁道无效导致读取数据失败
        /// </summary>
        public const string E23_3122 = "E233122";

        /// <summary>
        /// 完成之前卡被移除
        /// </summary>
        public const string E23_3123 = "E233123";

        /// <summary>
        /// 没有发现磁道，插卡或者刷卡的方式错误
        /// </summary>
        public const string E23_3124 = "E233124";

        /// <summary>
        /// 没有找到指定的表单
        /// </summary>
        public const string E23_3125 = "E233125";

        /// <summary>
        /// 安全模式没有成功读取卡的标志
        /// </summary>
        public const string E23_3126 = "E233126";

        /// <summary>
        /// 插入的卡太短
        /// </summary>
        public const string E23_3127 = "E233127";

        /// <summary>
        /// 插入的卡太长
        /// </summary>
        public const string E23_3128 = "E233128";

        /// <summary>
        /// 在退卡时候卡被吞
        /// </summary>
        public const string E23_3129 = "E233129";
        /// <summary>
        /// 身份证模块SP Open同步返回失败，请检测蓝牙连接或重启手持终端
        /// </summary>
        public const string E23_3130 = "E233130";


        #endregion

        #region XFS公共错误
        /// <summary>
        /// 请求被WFSCancelBlockingCall取消
        /// </summary>
        public const string E23_3140 = "E233140";
        /// <summary>
        /// 硬件故障
        /// </summary>
        public const string E23_3141 = "E233141";
        /// <summary>
        /// 内部错误
        /// </summary>
        public const string E23_3142 = "E233142";
        /// <summary>
        /// 不支持此命令
        /// </summary>
        public const string E23_3143 = "E233143";
        /// <summary>
        /// 命令执行超时
        /// </summary>
        public const string E23_3144 = "E233144";
        /// <summary>
        /// 参数是无效的服务句柄。
        /// </summary>
        public const string E23_3145 = "E233145";
        /// <summary>
        /// SP连接丢失
        /// </summary>
        public const string E23_3146 = "E233146";
        /// <summary>
        /// 逻辑名不正确或SP服务没有找到
        /// </summary>
        public const string E23_3147 = "E233147";


        #endregion

        #region 设备状态错误码

        /// <summary>
        /// 身份证模块处于忙碌状态此时不能运行执行命令
        /// </summary>
        public const string E23_3160 = "E233160";
        /// <summary>
        /// 身份证模块发生故障正在重启，请稍后再试
        /// </summary>
        public const string E23_3161 = "E233161";

        #endregion

        /// <summary>
        /// 身份证读卡器发生未知错误
        /// </summary>
        public const string E23_3199 = "E233199";
        #endregion

        #region 护照扫描器[32]    
        #region 执行命令错误码

        /// <summary>
        /// 护照扫描器打开失败。
        /// </summary>
        public const string E23_3201 = "E233201";

        /// <summary>
        /// 护照扫描器关闭失败。
        /// </summary>
        public const string E23_3202 = "E233202";

        /// <summary>
        /// 获取护照扫描器状态失败
        /// </summary>
        public const string E23_3203 = "E233203";

        /// <summary>
        /// 重置护照扫描器失败。
        /// </summary>
        public const string E23_3204 = "E233204";

        /// <summary>
        /// 异步扫描护照失败。
        /// </summary>
        public const string E23_3205 = "E233205";

        /// <summary>
        /// 退护照失败
        /// </summary>
        public const string E23_3206 = "E233206";

        /// <summary>
        /// 护照取消操作失败。
        /// </summary>
        public const string E23_3207 = "E233207";

        /// <summary>
        /// 异步扫描护照失败（文通）
        /// </summary>
        public const string E23_3208 = "E233208";

        /// <summary>
        /// 异步设置护照自动扫描失败
        /// </summary>
        public const string E23_3209 = "E233209";
        #endregion

        #region 私有错误

        /// <summary>
        /// 护照被夹住需要操作人员干预
        /// </summary>
        public const string E23_3220 = "E233220";

        /// <summary>
        /// 由于操作失误或者硬件故障，而未能成功打开或关闭卡口，需要操作人员的干预
        /// </summary>
        public const string E23_3221 = "E233221";

        /// <summary>
        /// 输入的磁道无效导致读取数据失败
        /// </summary>
        public const string E23_3222 = "E233222";

        /// <summary>
        /// 完成之前护照被移除
        /// </summary>
        public const string E23_3223 = "E233223";

        /// <summary>
        /// 没有发现磁道，放置护照或者刷护照的方式错误
        /// </summary>
        public const string E23_3224 = "E233224";

        /// <summary>
        /// 没有找到指定的表单
        /// </summary>
        public const string E23_3225 = "E233225";

        /// <summary>
        /// 安全模式没有成功读取卡的标志
        /// </summary>
        public const string E23_3226 = "E233226";

        /// <summary>
        /// 插入的护照太短
        /// </summary>
        public const string E23_3227 = "E233227";

        /// <summary>
        /// 插入的护照太长
        /// </summary>
        public const string E23_3228 = "E233228";

        /// <summary>
        /// 在退卡时候卡被吞
        /// </summary>
        public const string E23_3229 = "E233229";

        /// <summary>
        /// 同步获取护照扫描仪状态。
        /// </summary>
        public const string E23_3230 = "E233230";
        #endregion

        #region XFS公共错误
        /// <summary>
        /// 请求被WFSCancelBlockingCall取消
        /// </summary>
        public const string E23_3240 = "E233240";
        /// <summary>
        /// 硬件故障
        /// </summary>
        public const string E23_3241 = "E233241";
        /// <summary>
        /// 内部错误
        /// </summary>
        public const string E23_3242 = "E233242";
        /// <summary>
        /// 不支持此命令
        /// </summary>
        public const string E23_3243 = "E233243";
        /// <summary>
        /// 命令执行超时
        /// </summary>
        public const string E23_3244 = "E233244";
        /// <summary>
        /// 参数是无效的服务句柄。
        /// </summary>
        public const string E23_3245 = "E233245";
        /// <summary>
        /// SP连接丢失
        /// </summary>
        public const string E23_3246 = "E233246";

        #endregion

        #region 设备状态错误码

        /// <summary>
        /// 护照扫描模块处于忙碌状态此时不能运行执行命令
        /// </summary>
        public const string E23_3260 = "E233260";
        /// <summary>
        /// 护照扫描模块发生故障正在重启，请稍后再试
        /// </summary>
        public const string E23_3261 = "E233261";

        #endregion

        /// <summary>
        /// 护照扫描器发生未知错误
        /// </summary>
        public const string E23_3299 = "E233299";
        #endregion

        #region PIN 密码键盘[35]
        #region Pin接口异常错误码
        /// <summary>
        /// PIN密码键盘打开失败
        /// </summary>
        public const string E23_3501 = "E233501";
        /// <summary>
        /// PIN密码键盘关闭失败
        /// </summary>
        public const string E23_3502 = "E233502";
        /// <summary>
        /// PIN密码键盘取状态失败
        /// </summary>
        public const string E23_3503 = "E233503";
        /// <summary>
        /// PIN密码键盘取能力失败
        /// </summary>
        public const string E23_3504 = "E233504";
        /// <summary>
        /// PIN密码键盘重置失败
        /// </summary>
        public const string E23_3505 = "E233505";
        /// <summary>
        /// 取消操作失败
        /// </summary>
        public const string E23_3506 = "E233506";
        /// <summary>
        /// 明文输入失败
        /// </summary>
        public const string E23_3507 = "E233507";
        /// <summary>
        /// 密文输入失败
        /// </summary>
        public const string E23_3508 = "E233508";
        /// <summary>
        /// 初始化加密模块失败
        /// </summary>
        public const string E23_3509 = "E233509";
        /// <summary>
        /// 导入密钥失败
        /// </summary>
        public const string E23_3510 = "E233510";
        /// <summary>
        /// GetPinBlock失败
        /// </summary>
        public const string E23_3511 = "E233511";
        /// <summary>
        /// 数据加密失败
        /// </summary>
        public const string E23_3512 = "E233512";
        /// <summary>
        /// 数据解密失败
        /// </summary>
        public const string E23_3513 = "E233513";
        /// <summary>
        /// 软加密获取卡密码密文失败
        /// </summary>
        public const string E23_3514 = "E233514";
        /// <summary>
        /// 软加密获取电话银行密码密文失败
        /// </summary>
        public const string E23_3515 = "E233515";

        // E233514~E233519 为后续可能新增接口预留错误码
        #endregion

        #region Pin密码键盘模块特有错误码
        /// <summary>
        /// 密钥没有找到
        /// WFS_ERR_PIN_KEYNOTFOUND
        /// </summary>
        public const string E23_3520 = "E233520";
        /// <summary>
        /// 密钥没有值
        /// WFS_ERR_PIN_KEYNOVALUE
        /// </summary>
        public const string E23_3521 = "E233521";
        /// <summary>
        /// GetPinBlock前未输入密钥
        /// WFS_ERR_PIN_NOPIN
        /// </summary>
        public const string E23_3522 = "E233522";
        /// <summary>
        /// 密钥长度无效
        /// WFS_ERR_PIN_INVALIDKEYLENGTH
        /// </summary>
        public const string E23_3523 = "E233523";
        /// <summary>
        /// GetPin、GetData选择的激活按键无效
        /// WFS_ERR_PIN_KEYINVALID
        /// </summary>
        public const string E23_3524 = "E233524";
        /// <summary>
        /// GetPin、GetData选择的激活按键不支持
        /// WFS_ERR_PIN_KEYNOTSUPPORTED
        /// </summary>
        public const string E23_3525 = "E233525";
        /// <summary>
        /// GetPin、GetData没有激活按键
        /// WFS_ERR_PIN_NOACTIVEKEYS
        /// </summary>
        public const string E23_3526 = "E233526";
        /// <summary>
        /// 未达到最小输入长度
        /// WFS_ERR_PIN_MINIMUMLENGTH
        /// </summary>
        public const string E23_3527 = "E233527";
        /// <summary>
        /// GetPinblock Format不支持
        /// WFS_ERR_PIN_FORMATNOTSUPP
        /// </summary>
        public const string E23_3528 = "E233528";
        /// <summary>
        /// WFS_ERR_PIN_ACCESSDENIED 
        /// 操作被拒绝，加密模块初始化后未导入任何密钥
        /// </summary>
        public const string E23_3529 = "E233529";
        /// <summary>
        /// 密码键盘模块SP Open同步返回失败，请检测蓝牙连接或重启手持终端
        /// </summary>
        public const string E23_3530 = "E233530";

        // E233529~E233539 模块特有错误码预留
        #endregion

        #region XFS公共错误码
        /// <summary>
        /// 命令被取消
        /// WFS_ERR_CANCELED
        /// </summary>
        public const string E23_3540 = "E233540";
        /// <summary>
        /// 密码键盘硬件故障
        /// WFS_ERR_HARDWARE_ERROR
        /// </summary>
        public const string E23_3541 = "E233541";
        /// <summary>
        /// 密码键盘内部错误
        /// WFS_ERR_INTERNAL_ERROR
        /// </summary>
        public const string E23_3542 = "E233542";
        /// <summary>
        /// 密码键盘不支持此命令
        /// WFS_ERR_INVALID_COMMAND
        /// </summary>
        public const string E23_3543 = "E233543";
        /// <summary>
        /// 密码键盘命令执行超时
        /// WFS_ERR_TIMEOUT
        /// </summary>
        public const string E23_3544 = "E233544";
        /// <summary>
        /// 未OPEN 密码键盘
        /// WFS_ERR_INVALID_HSERVICE
        /// </summary>
        public const string E23_3545 = "E233545";
        /// <summary>
        /// 密码键盘SP连接丢失
        /// WFS_ERR_CONNECTION_LOST
        /// </summary>
        public const string E23_3546 = "E233546";

        /// <summary>
        /// 输入参数无效
        /// WFS_ERR_INVALID_DATA
        /// </summary>
        public const string E23_3547 = "E233547";
        // E233548~E233559 XFS公共错误码预留
        #endregion

        #region Pin密码键盘状态解析错误码
        /// <summary>
        /// 设备正忙，请稍后再试
        /// </summary>
        public const string E23_3560 = "E233560";
        /// <summary>
        /// 设备故障正在重启，请稍后再试
        /// </summary>
        public const string E23_3561 = "E233561";

        /// <summary>
        /// 密码键盘其他错误
        /// </summary>
        public const string E23_3599 = "E233599";
        #endregion
        #endregion

        #region TrackReader刷卡刷折器[39]
        #region TrackReader接口异常错误码
        /// <summary>
        /// TrackReader刷卡刷折器打开失败
        /// </summary>
        public const string E23_3901 = "E233901";
        /// <summary>
        /// TrackReader刷卡刷折器关闭失败
        /// </summary>
        public const string E23_3902 = "E233902";
        /// <summary>
        /// TrackReader刷卡刷折器取状态失败
        /// </summary>
        public const string E23_3903 = "E233903";
        /// <summary>
        /// TrackReader刷卡刷折器取能力失败
        /// </summary>
        public const string E23_3904 = "E233904";
        /// <summary>
        /// TrackReader刷卡刷折器重置失败
        /// </summary>
        public const string E23_3905 = "E233905";
        /// <summary>
        /// 取消操作失败
        /// </summary>
        public const string E23_3906 = "E233906";
        /// <summary>
        /// 读磁卡接口失败
        /// </summary>
        public const string E23_3907 = "E233907";
        /// <summary>
        /// 吐卡接口失败
        /// </summary>
        public const string E23_3908 = "E233908";
        /// <summary>
        /// 吞卡接口失败
        /// </summary>
        public const string E23_3909 = "E233909";
        /// <summary>
        /// 读芯片信息失败
        /// </summary>
        public const string E23_3910 = "E233910";
        // E233909~E233919 为后续可能新增接口预留错误码

        /// <summary>
        /// 用户未插入卡
        /// </summary>
        public const string E23_3911 = "E233911";
        #endregion

        #region 刷卡器特有错误码
        /// <summary>
        /// 没有卡片在读卡位置
        /// WFS_ERR_IDC_NOMEDIA
        /// </summary>
        public const string E23_3920 = "E233920";
        /// <summary>
        /// 无效的数据
        /// WFS_ERR_IDC_INVALIDDATA
        /// </summary>
        public const string E23_3921 = "E233921";
        /// <summary>
        /// 无效的卡片
        /// WFS_ERR_IDC_INVALIDMEDIA
        /// </summary>
        public const string E23_3922 = "E233922";

        /// <summary>
        /// 刷卡器SP Open同步返回失败，请检测蓝牙连接或重启手持终端
        /// </summary>
        public const string E23_3930 = "E233930";

        // E233924~E233939 模块特有错误码预留
        #endregion

        #region XFS公共错误码
        /// <summary>
        /// 命令被取消
        /// WFS_ERR_CANCELED
        /// </summary>
        public const string E23_3940 = "E233940";
        /// <summary>
        /// 刷卡刷折器硬件故障
        /// WFS_ERR_HARDWARE_ERROR
        /// </summary>
        public const string E23_3941 = "E233941";
        /// <summary>
        /// 刷卡刷折器内部错误
        /// WFS_ERR_INTERNAL_ERROR
        /// </summary>
        public const string E23_3942 = "E233942";
        /// <summary>
        /// 刷卡刷折器不支持此命令
        /// WFS_ERR_INVALID_COMMAND
        /// </summary>
        public const string E23_3943 = "E233943";
        /// <summary>
        /// 刷卡刷折器命令执行超时
        /// WFS_ERR_TIMEOUT
        /// </summary>
        public const string E23_3944 = "E233944";
        /// <summary>
        /// 未OPEN 刷卡器
        /// WFS_ERR_INVALID_HSERVICE
        /// </summary>
        public const string E23_3945 = "E233945";
        /// <summary>
        /// 刷卡刷折器SP连接丢失
        /// WFS_ERR_CONNECTION_LOST
        /// </summary>
        public const string E23_3946 = "E233946";

        // E233946~E233959 XFS公共错误码预留
        #endregion

        #region 模块状态解析错误码
        /// <summary>
        /// 设备正忙，请稍后再试
        /// </summary>
        public const string E23_3960 = "E233960";
        /// <summary>
        /// 设备故障正在重启，请稍后再试
        /// </summary>
        public const string E23_3961 = "E233961";

        /// <summary>
        /// 刷卡刷折器其他错误
        /// </summary>
        public const string E23_3999 = "E233999";
        #endregion

        #endregion

        #region CIM存款模块[40]

        #region 存款模块接口异常错误码
        /// <summary>
        /// CashAcceptor打开失败
        /// </summary>
        public const string E23_4001 = "E234001";
        /// <summary>
        /// CashAcceptor关闭失败
        /// </summary>
        public const string E23_4002 = "E234002";
        /// <summary>
        /// CashAcceptor取状态失败
        /// </summary>
        public const string E23_4003 = "E234003";
        /// <summary>
        /// CashAcceptor取能力失败
        /// </summary>
        public const string E23_4004 = "E234004";
        /// <summary>
        /// CashInstart失败
        /// </summary>
        public const string E23_4005 = "E234005";

        /// <summary>
        /// CashIn失败
        /// </summary>
        public const string E23_4006 = "E234006";

        /// <summary>
        /// CashInEnd失败
        /// </summary>
        public const string E23_4007 = "E234007";

        /// <summary>
        /// CashRollback失败
        /// </summary>
        public const string E23_4008 = "E234008";
        // E234009~E234019 为后续可能新增接口预留错误码
        #endregion

        #region 存款模块特有错误码

        /// <summary>
        /// WFS_ERR_CIM_INVALIDCURRENCY
        /// </summary>
        public const string E23_4020 = "E234020";
        /// <summary>
        /// WFS_ERR_CIM_CASHUNITERROR
        /// </summary>
        public const string E23_4021 = "E234021";
        /// <summary>
        /// WFS_ERR_CIM_TOOMANYITEMS
        /// </summary>
        public const string E23_4022 = "E234022";
        /// <summary>
        /// WFS_ERR_CIM_UNSUPPOSITION
        /// </summary>
        public const string E23_4023 = "E234023";
        /// <summary>
        /// WFS_ERR_CIM_SAFEDOOROPEN
        /// </summary>
        public const string E23_4024 = "E234024";
        /// <summary>
        /// WFS_ERR_CIM_SHUTTERNOTOPEN
        /// </summary>
        public const string E23_4025 = "E234025";
        /// <summary>
        /// WFS_ERR_CIM_SHUTTERCLOSED
        /// </summary>
        public const string E23_4026 = "E234026";

        /// <summary>
        /// WFS_ERR_CIM_INVALIDCASHUNIT
        /// </summary>
        public const string E23_4027 = "E234027";
        /// <summary>
        /// WFS_ERR_CIM_NOITEMS
        /// </summary>
        public const string E23_4028 = "E234028";
        /// <summary>
        /// WFS_ERR_CIM_EXCHANGEACTIVE
        /// </summary>
        public const string E23_4029 = "E234029";
        /// <summary>
        /// WFS_ERR_CIM_NOEXCHANGEACTIVE
        /// </summary>
        public const string E23_4030 = "E234030";
        /// <summary>
        /// WFS_ERR_CIM_SHUTTERNOTCLOSED
        /// </summary>
        public const string E23_4031 = "E234031";
        /// <summary>
        /// WFS_ERR_CIM_ITEMSTAKEN
        /// </summary>
        public const string E23_4032 = "E234032";
        /// <summary>
        /// WFS_ERR_CIM_CASHINACTIVE
        /// </summary>
        public const string E23_4033 = "E234033";
        /// <summary>
        /// WFS_ERR_CIM_NOCASHINACTIVE
        /// </summary>
        public const string E23_4034 = "E234034";
        /// <summary>
        /// WFS_ERR_CIM_POSITION_NOT_EMPTY
        /// </summary>
        public const string E23_4035 = "E234035";
        /// <summary>
        /// WFS_ERR_CIM_INVALIDRETRACTPOSITION
        /// </summary>
        public const string E23_4036 = "E234036";
        /// <summary>
        /// WFS_ERR_CIM_NOTRETRACTAREA
        /// </summary>
        public const string E23_4037 = "E234037";

        // E234024~E234039 模块特有错误码预留
        #endregion

        #region XFS公共错误码
        /// <summary>
        /// 命令被取消
        /// WFS_ERR_CANCELED
        /// </summary>
        public const string E23_4040 = "E234040";
        /// <summary>
        /// 硬件故障
        /// WFS_ERR_HARDWARE_ERROR
        /// </summary>
        public const string E23_4041 = "E234041";
        /// <summary>
        /// 内部错误
        /// WFS_ERR_INTERNAL_ERROR
        /// </summary>
        public const string E23_4042 = "E234042";
        /// <summary>
        /// 不支持此命令
        /// WFS_ERR_INVALID_COMMAND
        /// </summary>
        public const string E23_4043 = "E234043";
        /// <summary>
        /// 命令执行超时
        /// WFS_ERR_TIMEOUT
        /// </summary>
        public const string E23_4044 = "E234044";
        /// <summary>
        /// 未OPEN 
        /// WFS_ERR_INVALID_HSERVICE
        /// </summary>
        public const string E23_4045 = "E234045";
        /// <summary>
        /// SP连接丢失
        /// WFS_ERR_CONNECTION_LOST
        /// </summary>
        public const string E23_4046 = "E234046";

        // E234046~E234059 XFS公共错误码预留
        #endregion

        #region 模块状态解析错误码


        /// <summary>
        /// 其他错误
        /// </summary>
        public const string E23_4099 = "E234099";
        #endregion
        #endregion

        #region 发卡器[41]
        #region 接口异常错误码
        /// <summary>
        /// 发卡器打开失败。
        /// </summary>
        public const string E23_4101 = "E234101";

        /// <summary>
        /// 异步发卡失败。
        /// </summary>
        public const string E23_4102 = "E234102";

        /// <summary>
        /// 异步读卡失败。(Magnetic Card)
        /// </summary>
        public const string E23_4103 = "E234103";

        /// <summary>
        /// 异步获取设备信息失败
        /// </summary>
        public const string E23_4104 = "E234104";

        /// <summary>
        /// 异步吞卡失败
        /// </summary>
        public const string E23_4105 = "E234105";

        /// <summary>
        /// 异步退卡失败
        /// </summary>
        public const string E23_4106 = "E234106";

        /// <summary>
        /// 发卡器关闭失败。
        /// </summary>
        public const string E23_4107 = "E234107";

        /// <summary>
        /// 获取卡箱信息失败
        /// </summary>
        public const string E23_4108 = "E234108";
        /// <summary>
        /// 设置卡箱失败
        /// </summary>
        public const string E23_4109 = "E234109";

        /// <summary>
        /// 移动卡从出口到读卡器失败。
        /// </summary>
        public const string E23_4110 = "E234110";
        #endregion

        #region CardDispenser 专有错误码
        /// <summary>
        /// WFS_ERR_CRD_MEDIAJAM
        /// </summary>
        public const string E23_4120 = "E234120";
        /// <summary>
        /// WFS_ERR_CRD_NOMEDIA
        /// </summary>
        public const string E23_4121 = "E234121";
        /// <summary>
        /// WFS_ERR_CRD_MEDIARETAINED
        /// </summary>
        public const string E23_4122 = "E234122";
        /// <summary>
        /// WFS_ERR_CRD_RETAINBINFULL
        /// </summary>
        public const string E23_4123 = "E234123";
        /// <summary>
        /// WFS_ERR_CRD_SHUTTERFAIL
        /// </summary>
        public const string E23_4124 = "E234124";
        /// <summary>
        /// WFS_ERR_CRD_DEVICE_OCCUPIED
        /// </summary>
        public const string E23_4125 = "E234125";
        /// <summary>
        /// WFS_ERR_CRD_CARDUNITERROR
        /// </summary>
        public const string E23_4126 = "E234126";
        /// <summary>
        /// WFS_ERR_CRD_INVALIDCARDUNIT
        /// </summary>
        public const string E23_4127 = "E234127";
        /// <summary>
        /// WFS_ERR_CRD_INVALID_PORT
        /// </summary>
        public const string E23_4128 = "E234128";
        /// <summary>
        /// WFS_ERR_CRD_INVALIDRETAINBIN
        /// </summary>
        public const string E23_4129 = "E234129";
        /// <summary>
        /// WFS_ERR_CRD_POWERSAVETOOSHORT
        /// </summary>
        public const string E23_4130 = "E234130";
        /// <summary>
        /// WFS_ERR_CRD_POWERSAVEMEDIAPRESENT
        /// </summary>
        public const string E23_4131 = "E234131";

        #endregion

        #region Wosa功能公共错误码
        /// <summary>
        /// WFS_ERR_CANCELED
        /// </summary>
        public const string E23_4140 = "E234140";
        /// <summary>
        /// WFS_ERR_HARDWARE_ERROR
        /// </summary>
        public const string E23_4141 = "E234141";
        /// <summary>
        /// WFS_ERR_INTERNAL_ERROR
        /// </summary>
        public const string E23_4142 = "E234142";
        /// <summary>
        /// WFS_ERR_INVALID_COMMAND
        /// </summary>
        public const string E23_4143 = "E234143";
        /// <summary>
        /// WFS_ERR_TIMEOUT
        /// </summary>
        public const string E23_4144 = "E234144";
        /// <summary>
        /// WFS_ERR_INVALID_HSERVICE
        /// </summary>
        public const string E23_4145 = "E234145";
        /// <summary>
        /// WFS_ERR_CONNECTION_LOST
        /// </summary>
        public const string E23_4146 = "E234146";

        #endregion
        /// <summary>
        /// 存款模块其他错误
        /// </summary>
        public const string E23_4199 = "E234199";

        #endregion

        #region 凭条打印[42]

        /// <summary>
        /// 凭条打印机打开失败。
        /// </summary>
        public const string E23_4201 = "E234201";

        /// <summary>
        /// 凭条打印机关闭失败。
        /// </summary>
        public const string E23_4202 = "E234202";

        /// <summary>
        /// 凭条打印机切纸失败。
        /// </summary>
        public const string E23_4203 = "E234203";

        /// <summary>
        /// 获取设备状态失败
        /// </summary>
        public const string E23_4204 = "E234204";

        /// <summary>
        /// 获取设备能力失败
        /// </summary>
        public const string E23_4205 = "E234205";

        /// <summary>
        /// 获取可用的Form及media列表失败
        /// </summary>
        public const string E23_4206 = "E234206";

        /// <summary>
        /// 重置失败
        /// </summary>
        public const string E23_4207 = "E234207";
        /// <summary>
        /// 取消操作失败
        /// </summary>
        public const string E23_4208 = "E234208";
        /// <summary>
        /// 凭条打印失败
        /// </summary>
        public const string E23_4209 = "E234209";
        /// <summary>
        /// 凭条回收失败
        /// </summary>
        public const string E23_4210 = "E234210";

        /// <summary>
        /// 设备正忙，请稍后再试
        /// </summary>
        public const string E23_4260 = "E234260";
        /// <summary>
        /// 设备故障正在重启，请稍后再试
        /// </summary>
        public const string E23_4261 = "E234261";
        /// <summary>
        /// 凭条打印其他错误
        /// </summary>
        public const string E23_4299 = "E234299";
        #endregion

        #region EJ打印[43]
        /// <summary>
        /// EJ的Log写失败
        /// </summary>
        public const string E20_4301 = "E204301";
        /// <summary>
        /// EJ文件不存在
        /// </summary>
        public const string E20_4302 = "E204302";


        #endregion
    }
}
