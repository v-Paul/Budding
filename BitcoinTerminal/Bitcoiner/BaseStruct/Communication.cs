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
using Newtonsoft.Json;

namespace BaseSturct
{

    [Serializable]
    class LastBlockInfo
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LastBlockHash { get; set; }
        public int LastBlockHeight { get; set; }
        public string IP { get; set; }

    }

    [Serializable]
    class DBFileInfo: LastBlockInfo
    {
        public long DBFileSize { get; set; }
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

        // MultiSign
        public const string Informed = "Informed";
        public const string RejectSign = "RejectSign";
    }

    [Serializable]
    class RequestBlock:LastBlockInfo
    {
        public string RequestType { get; set; }
    }

    [Serializable]
    class ResponseBlock : LastBlockInfo
    {
        /// <summary>
        /// others opinions of your lastblock
        /// </summary>
        public string BlockResult { get; set; }
    }
    class BlockRequestType
    {
        public const string RequestBlockInfo = "RequestBlockInfo";
        public const string GetNewBlocks = "GetNewBlocks";
    }
    class BlockResultType
    {
        public const string OrphanBlock = "OrphanBlock";
        public const string Higher = "Higher";
        public const string Lower = "Lower";
        public const string Sameheight = "Sameheight";
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
        public Func<Transaction, string> PriTxCallBack;
        public Func<Block, string> NewBlockCallBack;
        public Action<int> RefresfNodeCountCallBack;
        public Action<string> PushTxhsPoolCallBack;
        public Action<string> PushLastBlockCallBack;
        public Communication()
        {
            this.SocketsHelp = new SocketsHelper();
           
            this.dicAddressesPool = new Dictionary<string,int>();

            this.SocketsHelp.XXPSocketsExecuteCallBack = this.XXPSocketsExecuteCallBack;

            if (!this.SocketsHelp.XXPStartReceiveMsg(AppSettings.XXPCommport))
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
        public XXPSocketsModel XXPSendMessage(string ip, XXPSocketsModel Request,int iTimeout)
        {
            return this.SocketsHelp.XXPSendMessage(ip, Request, AppSettings.XXPCommport, iTimeout);
        }

        public void Add2AddressPool(string Ip)
        {
            LogHelper.WriteMethodLog(true);
            if (!this.dicAddressesPool.ContainsKey(Ip))
            {
                LogHelper.WriteInfoLog("Add new IP:" + Ip);
                this.dicAddressesPool.Add(Ip, 0);
                this.RefresfNodeCountCallBack(this.GetAddressCount());
            }
            LogHelper.WriteMethodLog(false);
        }

        public void RemoveAddressFromPool(string Ip)
        {
            LogHelper.WriteMethodLog(true);
            if (this.dicAddressesPool.ContainsKey(Ip))
            {
                LogHelper.WriteInfoLog("Offline IP:" + Ip);
                this.dicAddressesPool.Remove(Ip);
                this.RefresfNodeCountCallBack(this.GetAddressCount());
            }
            LogHelper.WriteMethodLog(false);
        }

        private XXPSocketsModel XXPSocketsExecuteCallBack(XXPSocketsModel mod)
        {
            LogHelper.WriteMethodLog(true);
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
                case XXPCoinMsgType.SyncBlocks:
                    refMod.Type = XXPCoinMsgType.SyncBlocks;
                    refMod.Value = handleSyncBlocks(mod);
                    break;
                case XXPCoinMsgType.Newtransactions:
                    refMod.Type = XXPCoinMsgType.Newtransactions;
                    refMod.Value = handleNewtransactions(mod);
                    break;
                case XXPCoinMsgType.NewBlock:
                    refMod.Type = XXPCoinMsgType.NewBlock;
                    refMod.Value = handleNewBlock(mod);
                    break;
                case XXPCoinMsgType.Pritransaction:
                    refMod.Type = XXPCoinMsgType.Pritransaction;
                    refMod.Value = handlePriTx(mod);
                    break;
                case XXPCoinMsgType.Message:
                    break;
                default:
                    
                    break;
            }
            LogHelper.WriteMethodLog(false);
            return refMod;
        }

        #region DBFile
        private string HandlleDBfileEvent(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            string strRet = string.Empty;
            switch (socketMod.Value)
            {
                case DBRequestType.RequestDBInfo:
                    strRet = GetDBfileInfo();
                    break;
                case DBRequestType.StratTransfer:
                    strRet = StartSendDBZip(socketMod.IpAddress); // start file transfer,  181211
                    break;
                default:
                    break;
            }
            LogHelper.WriteMethodLog(true);
            return strRet;
        }


        private string GetDBfileInfo()
        {
            try
            {
                LogHelper.WriteMethodLog(true);
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
            finally
            {
                LogHelper.WriteMethodLog(false);
            }

        }
        public DBFileInfo RequestHightestDBInfo()
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();         
            sendMod.Type = XXPCoinMsgType.DBfile;
            sendMod.Value = DBRequestType.RequestDBInfo;

            List<DBFileInfo> lstDBInfo = new List<DBFileInfo>();
            foreach (var item in this.dicAddressesPool)
            {
                
                XXPSocketsModel RetMod = this.XXPSendMessage(item.Key, sendMod);
                if(!string.IsNullOrEmpty(RetMod.Value ))
                {
                    DBFileInfo dbInfo = JsonHelper.Deserialize<DBFileInfo>(RetMod.Value);
                    dbInfo.IP = RetMod.IpAddress;
                    lstDBInfo.Add(dbInfo);
                }
            }


            var ss = lstDBInfo.Max(x => x.LastBlockHeight);
            DBFileInfo hightestDBInfo = lstDBInfo.FirstOrDefault(x => x.LastBlockHeight == ss);

            LogHelper.WriteInfoLog("HightestDBInfo: " + JsonHelper.Serializer<DBFileInfo>(hightestDBInfo));
            return hightestDBInfo;
        }
        public string RequestStartTransDB(string IP)
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.DBfile;
            sendMod.Value = DBRequestType.StratTransfer;
            
            XXPSocketsModel RetMod = this.XXPSendMessage(IP, sendMod);
            LogHelper.WriteInfoLog("RequestStartTransDB ret: " + RetMod.Value);
            return RetMod.Value;
        }

        public long StartReceiveFile(string IP, long size, string SavePath)
        {
            LogHelper.WriteMethodLog(true);
            if (this.TransFileHelper != null)
            {
                return -1;
            }

            this.TransFileHelper = new SocketsHelper();
            
            if(File.Exists(SavePath))
            {
                FileIOHelper.DeleteFile(SavePath);
            }
            long lTotal = TransFileHelper.StartReceivefile(SavePath, IP, AppSettings.XXPTransFilePort, size);
            LogHelper.WriteInfoLog(string.Format("Receive File size:{0}", lTotal));
            return lTotal;
        }
        public void DisposeTransFileHelper()
        {
            LogHelper.WriteMethodLog(true);
            this.TransFileHelper?.Dispose();
            this.TransFileHelper = null;
            LogHelper.WriteMethodLog(false);
        } 

        public string StartSendDBZip(string IP)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
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
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        #endregion

        #region Handshake
        public bool RequestHandshake(string ip)
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Handshake;
            sendMod.Value = ConstHelper.BC_RequestHandshake;

            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod,5000);
            if (RcvMod.Value == ConstHelper.BC_ReturnHandshake)
            {
                this.Add2AddressPool(RcvMod.IpAddress);
                LogHelper.WriteInfoLog("RequestHandshake succ" );
                return true;
            }
            LogHelper.WriteInfoLog("RequestHandshake failed");
            return false;
        }

        public void NotifyOffline()
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Handshake;
            sendMod.Value = ConstHelper.BC_NotifyOffline;

            foreach (var item in this.dicAddressesPool)
            {
                XXPSocketsModel RcvMod = this.XXPSendMessage(item.Key, sendMod, 3000);
            }
        }
        private string  HandlleHandshakeEvent(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            string sRet = string.Empty;
            if(socketMod.Value == ConstHelper.BC_RequestHandshake)
            {
                sRet = ConstHelper.BC_ReturnHandshake;
                if( !this.dicAddressesPool.ContainsKey(socketMod.IpAddress))
                {
                    this.Add2AddressPool(socketMod.IpAddress);
                    // send this to others ... 181210
                    List<string> lstTemp = new List<string>();
                    lstTemp.Add(socketMod.IpAddress);
                    Task.Run(()=> {
                        //modify by fdp 181231 放在外面push，只要握手成功就push，避免对方掉线重新连接，没有push
                        //this.PushLastBlockCallBack(socketMod.IpAddress);
                        //this.PushTxhsPoolCallBack(socketMod.IpAddress);                        
                        this.SendNewAddress2Others(lstTemp);
                    });
                }

                Task.Run(() =>
                {
                    this.PushLastBlockCallBack(socketMod.IpAddress);
                    this.PushTxhsPoolCallBack(socketMod.IpAddress);
                });
            }
            else if(socketMod.Value == ConstHelper.BC_NotifyOffline)
            {
                this.RemoveAddressFromPool(socketMod.IpAddress);
                sRet = ConstHelper.BC_ResponseOffline;
            }
            LogHelper.WriteMethodLog(false);
            return sRet;
        }
        #endregion

        #region NewAddresses

        public string ReserchNodes()
        {
            LogHelper.WriteMethodLog(true);
            AppSettings.SeedNodes = AppConfigHelper.GetConfigValByKey("SeedNodes");
            var lstSeeds = (from x in AppSettings.SeedNodes.Split('|')
                            where x != ""
                            select x).ToList();
            //foreach (var item in lstSeeds)
            //{
            //    if (item != OSHelper.GetLocalIP() && item != "127.0.0.1")
            //        this.RequestHandshake(item);
            //}

            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 4;
            
            Parallel.ForEach(lstSeeds, po, (item) =>
            {
                if (item != OSHelper.GetLocalIP() && item != "127.0.0.1")
                    this.RequestHandshake(item);
            });


            if (this.dicAddressesPool.Count == 0)
            {
                return "Connect to XXPCoin Net failed";
            }

            HashSet<string> hsNew = new HashSet<string>();
            foreach (var item in this.dicAddressesPool)
            {
                List<string> lstAddress = this.RequestMoreNodes(item.Key);


                foreach (var Adds in lstAddress)
                {
                    if (Adds != OSHelper.GetLocalIP() && !this.dicAddressesPool.ContainsKey(Adds))
                    {
                        if (!hsNew.Contains(Adds))
                        {
                            hsNew.Add(Adds);
                        }

                    }
                }
            }
            foreach (var item in hsNew)
            {
                if (this.RequestHandshake(item))
                {
                    this.Add2AddressPool(item);
                }
            }
            LogHelper.WriteMethodLog(false);
            return ConstHelper.BC_OK;

        }
        public List<string> RequestMoreNodes(string ip)
        {
            LogHelper.WriteMethodLog(true);
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
            LogHelper.WriteMethodLog(false);
            return lstIpAddress;

        }

        public void SendNewAddress2Others(List<string> lstNewAddress)
        {
            LogHelper.WriteMethodLog(true);
            if (lstNewAddress.Count == 0)
            {
                LogHelper.WriteMethodLog(false);
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
            LogHelper.WriteMethodLog(false);

        }

        private string handleNewAddress(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
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
                LogHelper.WriteMethodLog(true);
                return "ths";
            }
        }
        #endregion

        #region Newtransactions
        public string SendNewtransactions(string ip, Transaction Tx)
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Newtransactions;
            sendMod.Value = JsonHelper.Serializer<Transaction>(Tx);
            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod);
            if(RcvMod.Type == XXPCoinMsgType.Exception)
            {
                return Decision.Reject;
            }
            else
            {
                LogHelper.WriteInfoLog("SendNewtransactions ret: " + RcvMod.Value);
                return RcvMod.Value;
            }

        }



        public void SendNewTx2AddressLst(Transaction Tx)
        {
            LogHelper.WriteMethodLog(true);
            foreach (var item in this.dicAddressesPool)
            {
                string str = SendNewtransactions(item.Key, Tx);
            }
            LogHelper.WriteMethodLog(false);

        }

        public void SendNewTx2AddressLst(Transaction Tx, List<string> lstIP)
        {
            LogHelper.WriteMethodLog(true);
            foreach (var item in lstIP)
            {
                string str = SendNewtransactions(item, Tx);
            }
            LogHelper.WriteMethodLog(false);

        }

        private string handleNewtransactions(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            Transaction tx = new Transaction();
            if(!string.IsNullOrEmpty(socketMod.Value) )
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
            LogHelper.WriteInfoLog("handleNewtransactions ret: " + sRet);
            return sRet;
        }
        #endregion

        #region SyncBlocks
        public ResponseBlock RequestNewBlockInfo( Block lastBlock)
        {
            LogHelper.WriteMethodLog(true);
            RequestBlock ReqBkInfo = new RequestBlock();
            ReqBkInfo.RequestType = BlockRequestType.RequestBlockInfo;
            ReqBkInfo.LastBlockHash = lastBlock.Hash;
            ReqBkInfo.LastBlockHeight = lastBlock.Header.Height;
            string strValue = JsonHelper.Serializer<LastBlockInfo>(ReqBkInfo);

            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.SyncBlocks;
            sendMod.Value = strValue;

            ResponseBlock ResponseBkInfo = new ResponseBlock();
            List<ResponseBlock> lstResponse = new List<ResponseBlock>();
            foreach (var item in this.dicAddressesPool)
            {

                XXPSocketsModel RetMod = this.SocketsHelp.XXPSendMessage(item.Key, sendMod, AppSettings.XXPCommport);
                if(RetMod.Type == XXPCoinMsgType.Exception)
                {
                    int lostCount = item.Value;
                    //this.dicAddressesPool[item.Key] = lostCount++;

                    return ResponseBkInfo;
                }
                else if (!string.IsNullOrEmpty(RetMod.Value))
                {
                    ResponseBlock bkInfo = JsonHelper.Deserialize<ResponseBlock>(RetMod.Value);
                    bkInfo.IP = RetMod.IpAddress;
                    lstResponse.Add(bkInfo);
                }
            }

           

            int OrphanCount = lstResponse.Count(x=> x.BlockResult == BlockResultType.OrphanBlock);

            if(OrphanCount> lstResponse.Count/2)
            {
                ResponseBkInfo.BlockResult = BlockResultType.OrphanBlock;
                
            }
            else
            {
                int iHighest = lstResponse.Max(x => x.LastBlockHeight);
                ResponseBkInfo = lstResponse.FirstOrDefault(x => x.LastBlockHeight == iHighest);
            }

            LogHelper.WriteInfoLog("RequestNewBlockInfo ret: " + ResponseBkInfo.BlockResult);
            return ResponseBkInfo;


        }

        public string GetNewBlocks(string ip, Block lastBlock)
        {
            LogHelper.WriteMethodLog(true);
            RequestBlock ReqBkInfo = new RequestBlock();
            ReqBkInfo.RequestType = BlockRequestType.GetNewBlocks;
            ReqBkInfo.LastBlockHash = lastBlock.Hash;
            ReqBkInfo.LastBlockHeight = lastBlock.Header.Height;


            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.SyncBlocks;
            sendMod.Value = JsonHelper.Serializer<RequestBlock>(ReqBkInfo);

            XXPSocketsModel RetMod = this.SocketsHelp.XXPSendMessage(ip, sendMod, AppSettings.XXPCommport);
            LogHelper.WriteInfoLog("GetNewBlocks return: " + RetMod.Value);
            return RetMod.Value;

        }

        private string handleSyncBlocks(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            RequestBlock ReqBkInfo = JsonHelper.Deserialize<RequestBlock>(socketMod.Value);
            ReqBkInfo.IP = socketMod.IpAddress;
            string strRet = string.Empty;
            switch (ReqBkInfo.RequestType)
            {
                case BlockRequestType.RequestBlockInfo:
                    strRet = CheckBlock(ReqBkInfo);
                    break;
                case BlockRequestType.GetNewBlocks:
                    strRet = StartSendBlocks(ReqBkInfo);
                    break;
                default:
                    break;
            }
            LogHelper.WriteInfoLog("handleSyncBlocks ret: " + strRet);
            return strRet;
        }


        private string CheckBlock(RequestBlock ReqBkInfo)
        {
            LogHelper.WriteMethodLog(true);
            LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
            string strLastblock = LeveldbOperator.GetValue(ConstHelper.BC_LastKey);
            Block block = JsonHelper.Deserialize<Block>(strLastblock);
           

            ResponseBlock RetBkInfo = new ResponseBlock();
            RetBkInfo.LastBlockHash = block.Hash;
            RetBkInfo.LastBlockHeight = block.Header.Height;

            if (ReqBkInfo.LastBlockHeight > block.Header.Height)
            {
                RetBkInfo.BlockResult = BlockResultType.Higher;
                // request new block todo 181213 
            }
            else
            {
                string strBlock = LeveldbOperator.GetValue(ReqBkInfo.LastBlockHash);
                if(string.IsNullOrEmpty(strBlock))
                {
                    RetBkInfo.BlockResult = BlockResultType.OrphanBlock;
                }
                else
                {
                    if(ReqBkInfo.LastBlockHeight == block.Header.Height)
                    {
                        RetBkInfo.BlockResult = BlockResultType.Sameheight;
                    }
                    else
                    {
                        RetBkInfo.BlockResult = BlockResultType.Lower;
                    }
                }

            }

            LeveldbOperator.CloseDB();
            LogHelper.WriteMethodLog(false);
            return JsonHelper.Serializer<ResponseBlock>(RetBkInfo);
  
        }

        public string StartSendBlocks( RequestBlock ReqBkInfo)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                List<Block> lstTobeSendBlocks = new List<Block>();
                LeveldbOperator.OpenDB(AppSettings.XXPDBFolder);
                string strReqBlock = LeveldbOperator.GetValue(ReqBkInfo.LastBlockHash);
                if(!string.IsNullOrEmpty(strReqBlock))
                {
                    string strblock = LeveldbOperator.GetValue(ConstHelper.BC_LastKey);
                    Block block = JsonHelper.Deserialize<Block>(strblock);
                    
                    while (block.Hash != ReqBkInfo.LastBlockHash)
                    {
                        lstTobeSendBlocks.Add(block);
                        strblock = LeveldbOperator.GetValue(block.Header.PreHash);
                        block = JsonHelper.Deserialize<Block>(strblock);                        
                    }
                }
                LeveldbOperator.CloseDB();
               
                Task.Run(() => {
                    //foreach (var item in tobeSendBlocks)
                    for(int i= lstTobeSendBlocks.Count; i>0; i--)
                    {
                        string str = SendNewBlock(ReqBkInfo.IP, lstTobeSendBlocks[i-1]);
                        LogHelper.WriteInfoLog(str);
                    }

                });
                LogHelper.WriteMethodLog(false);
                return ConstHelper.BC_OK;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return "exception";
            }
        }

        #endregion

        #region NewBlock
        public string SendNewBlock(string ip,Block block)
        {
            LogHelper.WriteMethodInfoLog(ip, block.Hash);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.NewBlock;
            sendMod.Value = JsonHelper.Serializer<Block>(block);
            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod,20000);
            //add by fdp 181228
            if(RcvMod.Type == XXPCoinMsgType.Exception)
            {
                return Decision.Reject;
            }
            else
            {
                LogHelper.WriteInfoLog(string.Format("SendNewBlock to {0} return: {1}", ip, RcvMod.Value));
                return RcvMod.Value;
            }
            
        }

        public string SendNewBlock2AddressLst(Block block)
        {
            LogHelper.WriteMethodLog(true);
            string strRet = string.Empty;
            List<string> listResult = new List<string>();
            foreach (var item in this.dicAddressesPool)
            {
                string str = SendNewBlock(item.Key, block);
                listResult.Add(str);
            }
           
            var countAccept = listResult.Count(x=> x!= Decision.Reject);
            if(countAccept> listResult.Count/2)
            {
                strRet = Decision.Accept;
            }
            else
            {
                strRet = Decision.Reject;
            }
            
            LogHelper.WriteInfoLog("SendNewBlock2AddressLst result: " + strRet);
            return strRet;
        }

        private string handleNewBlock(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            Block block = new Block();
            if (!string.IsNullOrEmpty(socketMod.Value))
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
            LogHelper.WriteInfoLog("handleNewBlock retrun: " + sRet);
            return sRet;
        }



        #endregion

        #region Primitive Tx
        public string SendPriTx(string ip, Transaction Tx)
        {
            LogHelper.WriteMethodLog(true);
            XXPSocketsModel sendMod = new XXPSocketsModel();
            sendMod.Type = XXPCoinMsgType.Pritransaction;
            sendMod.Value = JsonHelper.Serializer<Transaction>(Tx);
            XXPSocketsModel RcvMod = this.XXPSendMessage(ip, sendMod);
            if (RcvMod.Type == XXPCoinMsgType.Exception)
            {
                return "Exception";
            }
            else
            {
                LogHelper.WriteInfoLog("SendPriTx ret: " + RcvMod.Value);
                return RcvMod.Value;
            }

        }

        private string handlePriTx(XXPSocketsModel socketMod)
        {
            LogHelper.WriteMethodLog(true);
            Transaction tx = new Transaction();
            if (!string.IsNullOrEmpty(socketMod.Value))
            {
                tx = JsonHelper.Deserialize<Transaction>(socketMod.Value);
            }
            string sRet = this.PriTxCallBack(tx);
           
            LogHelper.WriteInfoLog("handlePriTx ret: " + sRet);
            return sRet;
        }
        #endregion


    }
}
