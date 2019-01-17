using System;
using System.Collections.Generic;
using VTMC.Utils.Models;

namespace VTMC.Utils
{
    /// <summary>
    /// Sockets通讯模型
    /// </summary>
    [Serializable]
    public class SocketsModel : ResultEntity
    {
        /// <summary>
        /// 视频时长
        /// </summary>
    //    public RecordVideoInfor videoInfo { get; set; }
    }

    /// <summary>
    /// 发送给P2600的命令model
    /// </summary>
    [Serializable]
    public class P2600CmdSocketsModel 
    {
        /// <summary>
        /// 命令消息长度
        /// </summary>
        public int iMsgLentgh { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public string strMsgHeader { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public int iCmdType { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string strCurrency { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public string strTimeStamp { get; set; }
        /// <summary>
        /// 终端ID
        /// </summary>
        public string strTerminalID { get; set; }
        /// <summary>
        /// 序列码
        /// </summary>
        public string strSquNum { get; set; }


        public bool SetMesLenght()
        {
            string strTemp = string.Format("{0}{1}{2}{3}{4}{5}",
                                       strMsgHeader, iCmdType, strCurrency,
                                       strTimeStamp, strTerminalID, strSquNum);
            iMsgLentgh = strTemp.Length - 1 ;
            return true;
        }
        /// <summary>
        /// 拼接命令
        /// </summary>
        /// <returns></returns>
        public byte[] AssembleCmdMsg()
        {
            string strCmdMsg = string.Empty;
            strCmdMsg = string.Format("{0}{1}{2}{3}{4}{5}{6}", iMsgLentgh.ToString("000"),
                                       strMsgHeader, iCmdType, strCurrency,
                                       strTimeStamp, strTerminalID, strSquNum );

            return System.Text.Encoding.Default.GetBytes(strCmdMsg);
        } 

        public string GetMsgString()
        {
           return string.Format("{0}{1}{2}{3}{4}{5}{6}", iMsgLentgh.ToString("000"),
                                       strMsgHeader, iCmdType, strCurrency,
                                       strTimeStamp, strTerminalID, strSquNum);
        }

    }

    /// <summary>
    /// P2600的返回的数据model
    /// </summary>
    [Serializable]
    public class P2600RevSocketsModel<T>
    {
        /// <summary>
        /// 命令消息长度
        /// </summary>
        public int iMsgLentgh { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public string strMsgHeader { get; set; }
        /// <summary>
        /// 命令类型
        /// </summary>
        public int iCmdType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T DetailsInfo { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string strTimeStamp { get; set; }
        /// <summary>
        /// 终端ID
        /// </summary>
        public string strTerminalID { get; set; }
        /// <summary>
        /// 序列码
        /// </summary>
        public string strSquNum { get; set; }
    }
    /// <summary>
    /// P2600点钞或存款结构
    /// </summary>
    public class P2600ResultInfo
    {   
        /// <summary>
        /// 交易结果
        /// </summary>
        public string strTransResult { get; set; }
        /// <summary>
        /// 详细信息的长度
        /// </summary>
        public int strLengthOfInfo { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public string strCurrency { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public int iTotalAmount { get; set; }
        /// <summary>
        /// Details note information
        /// Each denomination:
        /// 4 length denomination 
        /// 4 length pieces
        /// </summary>
        public List<NoteNumber> DetailNoteInfo { get; set; }

    }



    [Serializable]
    public class XXPSocketsModel
    {
        public string IpAddress { set; get; }
        public string Type { set; get; }
        public string Value { set; get; }

    }

    [Serializable]
    public static class XXPCoinMsgType
    {
        public const string DBfile = "DBfile";
        public const string Handshake = "Handshake";
        public const string SyncBlocks = "SyncBlocks";
        public const string NewBlock = "NewBlock";
        public const string Newtransactions = "Newtransactions";
        public const string NewAddresses = "NewAddresses";
        public const string Message = "Message";
        public const string Exception = "Exception";
        public const string Pritransaction = "Pritransaction";
        public const string SignedPriTx = "SignedPriTx";
        public const string Unkonw = "Unkonw";
    }


}
