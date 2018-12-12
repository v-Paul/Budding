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

    class Decision
    {
        public const string Accept = "Accept";
        public const string Reject = "Reject";
        public const string Accepted = "Accepted";
    }

    class Communication
    {

        /// <summary>
        /// Sockets操作类
        /// </summary>
        private SocketsHelper SocketsHelp;
        private SocketsHelper TransFileHelper;
        private Dictionary<string, int> dicAddressesPool;
        public Func<Transaction, string> NewTransactionCallBack;
        public Func<Block, string> NewBlockCallBack;
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
                case XXPCoinMsgType.Newtransactions:
                    refMod.Type = XXPCoinMsgType.Newtransactions;
                    refMod.Value = handleNewtransactions(mod);
                    break;
                case XXPCoinMsgType.NewBlock:
                    refMod.Type = XXPCoinMsgType.NewBlock;
                    refMod.Value = handleNewBlock(mod);
                    break;

                case XXPCoinMsgType.Message:
                    break;
                default:
                    
                    break;
            }

            return refMod;
        }

        #region DBFile
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
                if (File.Exists(tempzip)) FileIOHelper.DeleteFile(tempzip);
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
        public string RequestStartTransDB(string IP)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.DBfile;
            sendMod.Value = DBRequestType.StratTransfer;
            
            XXPSocketsModel RetMod = this.SocketsHelp.XXPSendMessage(IP, sendMod, AppSettings.XXPCommport);
            return RetMod.Value;
        }

        public long StartReceiveFile(string IP, long size, string SavePath)
        {
            if(this.TransFileHelper != null)
            {
                return -1;
            }

            this.TransFileHelper = new SocketsHelper();
            
            if(File.Exists(SavePath))
            {
                FileIOHelper.DeleteFile(SavePath);
            }
            long lTotal = TransFileHelper.StartReceivefile(SavePath, IP, AppSettings.XXPTransFilePort, size);

            return lTotal;
        }
        public void DisposeTransFileHelper()
        {
            this.TransFileHelper?.Dispose();
            this.TransFileHelper = null;
        } 

        public string StartSendDBZip(string IP)
        {
            try
            {
                Task.Run(()=>{
                    SocketsHelper SendFileHelper = new SocketsHelper();
                    bool bRet = SendFileHelper.OpenFileTransConnect(IP, AppSettings.XXPTransFilePort);
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
        #endregion

        #region Handshake
        public bool RequestHandshake(string ip)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Handshake;
            sendMod.Value = ConstHelper.BC_RequestHandshake;

            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod);
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
        #endregion

        #region NewAddresses
        public List<string> RequestMoreNodes(string ip)
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
        #endregion

        #region Newtransactions


        public string SendNewtransactions(string ip, Transaction Tx)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Newtransactions;
            sendMod.Value = JsonHelper.Serializer<Transaction>(Tx);
            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod);
            return RcvMod.Value;
        }

        public void SendNewTx2AddressLst(Transaction Tx)
        {
            foreach (var item in this.dicAddressesPool)
            {
                string str = SendNewtransactions(item.Key, Tx);
            }

        }

        private string handleNewtransactions(XXPSocketsModel socketMod)
        {
            Transaction tx = new Transaction();
            if(string.IsNullOrEmpty(socketMod.Value) )
            {
                tx = JsonHelper.Deserialize<Transaction>(socketMod.Value);
            }
            string sRet = this.NewTransactionCallBack(tx);
            if(sRet == Decision.Accept)
            {
                Task.Run(()=> {
                    this.SendNewTx2AddressLst(tx);
                });
            }
            return sRet;
        }
        #endregion

        #region NewBlock
        public string SendNewBlock(string ip,Block block)
        {
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.NewBlock;
            sendMod.Value = JsonHelper.Serializer<Block>(block);
            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod);
            return RcvMod.Value;
        }

        public void SendNewBlock2AddressLst(Block block)
        {
            foreach (var item in this.dicAddressesPool)
            {
                string str = SendNewBlock(item.Key, block);
            }

        }

        private string handleNewBlock(XXPSocketsModel socketMod)
        {
            Block block = new Block();
            if (string.IsNullOrEmpty(socketMod.Value))
            {
                block = JsonHelper.Deserialize<Block>(socketMod.Value);
            }
            string sRet = this.NewBlockCallBack(block);
            if (sRet == Decision.Accept)
            {
                Task.Run(() => {
                    this.SendNewBlock2AddressLst(block);
                });
            }
            return sRet;
        }

        #endregion


    }
}
