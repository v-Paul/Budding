using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Data;
using VTMC.Utils;
using System.Text.RegularExpressions;
using System.IO;

namespace BaseSturct
{
    [Serializable]
    class DBFileInfo
    {
        public long DBFileSize { get; set; }
        public int LastBlockHeight { get; set; }
        public string IP { get; set; }
        public DBFileInfo(long size, int Height)
        {
            this.DBFileSize = size;
            this.LastBlockHeight = Height;
        }
    }
    [Serializable]
    class DBRequestType
    {
        public const string RequestDBInfo = "RequestDBInfo";
        public const string StratTransfer = "StratTransfer";
    }

    class Communication
    {

        /// <summary>
        /// Sockets操作类
        /// </summary>
        private SocketsHelper SocketsHelp;
       
        private Dictionary<string, int> dicAddressesPool;

        public Communication()
        {
            this.SocketsHelp = new SocketsHelper();
            
            this.dicAddressesPool = new Dictionary<string,int>();

            this.SocketsHelp.XXPSocketsExecuteCallBack = this.XXPSocketsExecuteCallBack;
            if (!this.SocketsHelp.XXPStartReceiveMsg(AppSettings.XXPCommport));
            {
                LogHelper.WriteErrorLog("SocketsHelp.StartReceive fail");
            }
        }

        public int GetAddressCount()
        {
            return this.dicAddressesPool.Count;
        }


        public XXPSocketsModel XXPSendMessage(string ip, XXPSocketsModel Request)
        {
            return this.SocketsHelp.XXPSendMessage(ip, Request,AppSettings.XXPCommport);
        }

        public void Add2AddressPool(string Ip)
        {
            if(!this.dicAddressesPool.ContainsKey(Ip))
            {
                this.dicAddressesPool.Add(Ip, 0);
            }
        }

        private XXPSocketsModel XXPSocketsExecuteCallBack(XXPSocketsModel mod)
        {
            XXPSocketsModel refMod = new XXPSocketsModel();
            refMod.IpAddress = OSHelper.GetLocalIP();
            refMod.Type = string.Empty;

            switch (mod.Type)
            {
                case XXPCoinMsgType.DBfile:
                    refMod.Type = XXPCoinMsgType.DBfile;
                    refMod.Value = HandlleDBfileEvent(mod);
                    break;
                case XXPCoinMsgType.Handshake:
                    refMod.Type = XXPCoinMsgType.Handshake;
                    refMod.Value = this.HandlleHandshakeEvent(mod);                 
                    break;
                case XXPCoinMsgType.NewAddresses:
                    refMod.Type = XXPCoinMsgType.NewAddresses;
                    refMod.Value = handleNewAddress(mod);
                    break;
                case XXPCoinMsgType.NewBlock:
                    break;
                case XXPCoinMsgType.Newtransactions:
                    break;
                case XXPCoinMsgType.Message:
                    break;
                default:
                    
                    break;
            }

            return refMod;
        }
        private string HandlleDBfileEvent(XXPSocketsModel socketMod)
        {
            string strRet = string.Empty;
            switch (socketMod.Value)
            {
                case DBRequestType.RequestDBInfo:
                    strRet = GetDBfileInfo();
                    break;
                case DBRequestType.StratTransfer:
                    strRet = StartSendDBZip(socketMod.IpAddress); // start file transfer, todo 181211
                    break;
                default:
                    break;
            }

            return strRet;
        }


        private string GetDBfileInfo()
        {
            try
            {

                string tempzip = Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                ZipHelper.Zip(AppSettings.XXPDBFolder, tempzip);
                FileInfo tempzipInfo = new FileInfo(tempzip);

                LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
                string strLastblock = LeveldbOperator.GetValue(ConstHelper.BC_LastKey);
                Block block = JsonHelper.Deserialize<Block>(strLastblock);
                LeveldbOperator.CloseDB();

                DBFileInfo dbinfo = new DBFileInfo(tempzipInfo.Length, block.Header.Height);
                return JsonHelper.Serializer<DBFileInfo>(dbinfo); ;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return string.Empty;
            }

        }
        public DBFileInfo RequestHightestDBInfo()
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();         
            sendMod.Type = XXPCoinMsgType.DBfile;
            sendMod.Value = DBRequestType.RequestDBInfo;

            List<DBFileInfo> lstDBInfo = new List<DBFileInfo>();
            foreach (var item in this.dicAddressesPool)
            {
                
                XXPSocketsModel RetMod = this.SocketsHelp.XXPSendMessage(item.Key, sendMod, AppSettings.XXPCommport);
                if(!string.IsNullOrEmpty(RetMod.Value ))
                {
                    DBFileInfo dbInfo = JsonHelper.Deserialize<DBFileInfo>(RetMod.Value);
                    dbInfo.IP = RetMod.IpAddress;
                    lstDBInfo.Add(dbInfo);
                }
            }


            var ss = lstDBInfo.Max(x => x.LastBlockHeight);
            DBFileInfo hightestDBInfo = lstDBInfo.FirstOrDefault(x => x.LastBlockHeight == ss);

            
            return hightestDBInfo;
        }
        public bool RequestStartTransDB(string IP)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.DBfile;
            sendMod.Value = DBRequestType.StratTransfer;
            
            XXPSocketsModel RetMod = this.SocketsHelp.XXPSendMessage(IP, sendMod, AppSettings.XXPCommport);
            if(RetMod.Value == ConstHelper.BC_OK)
            {
                return true;
            }
            else
            {
                LogHelper.WriteErrorLog(RetMod.Value);
                return false;
            }
        }

        public void StartReceiveFile(string IP)
        {
            SocketsHelper TransFileHelper = new SocketsHelper();
            //bool bRet = TransFileHelper.OpenFileTransConnect(IP, AppSettings.XXPCommport);
            //if (bRet)
            //{
                string tempPath = Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                TransFileHelper.StartReceivefile(tempPath, IP, AppSettings.XXPTransFilePort );                
            //}

        }

        public string StartSendDBZip(string IP)
        {
            try
            {
                Task.Run(()=>{
                    SocketsHelper SendFileHelper = new SocketsHelper();
                    SendFileHelper.OpenFileTransConnect(IP, AppSettings.XXPTransFilePort);
                    string tempzip = Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                    SendFileHelper.StartSendFile(tempzip);

                });
                return ConstHelper.BC_OK;
            }
            catch(Exception ex )
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "exception";
            }
        }



        public bool RequestHandshake(string ip)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            XXPSocketsModel RcvMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Handshake;
            sendMod.Value = ConstHelper.BC_RequestHandshake;
            RcvMod = this.XXPSendMessage(ip, sendMod);
            if (RcvMod.Value == ConstHelper.BC_ReturnHandshake)
            {
                this.Add2AddressPool(RcvMod.IpAddress);
                return true;
            }
            return false;
        }
        private string  HandlleHandshakeEvent(XXPSocketsModel socketMod)
        {
            string sRet = string.Empty;
            if(socketMod.Value == ConstHelper.BC_RequestHandshake)
            {
                sRet = ConstHelper.BC_ReturnHandshake;
                if( !this.dicAddressesPool.ContainsKey(socketMod.IpAddress))
                {
                    this.dicAddressesPool.Add(socketMod.IpAddress, 0);
                    // send this to others todo... 181210
                }

            }
            return sRet;
        }

        public  List<string> RequestMoreNodes(string ip)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            XXPSocketsModel RcvMod = new XXPSocketsModel();

            sendMod.Type = XXPCoinMsgType.NewAddresses;
            RcvMod = this.XXPSendMessage(ip, sendMod);
            List<string> lstIpAddress = new List<string>();
            if (!string.IsNullOrEmpty(RcvMod.Value))
            {
                lstIpAddress = (from x in RcvMod.Value.Split('|')
                                  where x != ""
                                  select x).ToList();              
            }

            return lstIpAddress;

        }

        public void SendNewAddress2Others(List<string> lstNewAddress)
        {
            if(lstNewAddress.Count == 0)
            {
                return;
            }

            XXPSocketsModel sendMod = new XXPSocketsModel();
            
            sendMod.Type = XXPCoinMsgType.NewAddresses;
            foreach (var item in lstNewAddress)
            {
                sendMod.Value += item + "|";
            }

            foreach (var item in this.dicAddressesPool)
            {
                this.XXPSendMessage(item.Key, sendMod);
            }

        }

        private string handleNewAddress(XXPSocketsModel socketMod)
        {
            string strRet = string.Empty;
            if(string.IsNullOrEmpty(socketMod.Value))// give my addresspool to sender
            {
                foreach (var item in dicAddressesPool)
                {
                    strRet += item.Key + "|";
                }

                return strRet;
            }
            else // sender give his new addresses to me, handshake every one if dont contain
            {
                List<string> lstIpAddress = new List<string>();
                if (!string.IsNullOrEmpty(socketMod.Value))
                {
                    lstIpAddress = (from x in socketMod.Value.Split('|')
                                    where x != ""
                                    select x).ToList();
                }

                foreach (var item in lstIpAddress)
                {
                    if (item == OSHelper.GetLocalIP())
                    {
                        continue;
                    }
                    if (!this.dicAddressesPool.ContainsKey(item))
                    {
                        this.RequestHandshake(item);
                    }
                }
                return "ths";
            }
        }




    }
}
