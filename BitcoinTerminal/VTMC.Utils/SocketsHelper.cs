﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace VTMC.Utils
{
    /// <summary>
    /// Sockets通讯类
    /// </summary>
    public class SocketsHelper
    {
        #region Consts
        #endregion

        #region Fileds
        /// <summary>
        /// 回调事件
        /// </summary>
        public Func<XXPSocketsModel,XXPSocketsModel> XXPSocketsExecuteCallBack;
        private TcpClient XXPfileClient;
        private NetworkStream XXPfileStream;


        #region old fileds
        public Func<SocketsModel, SocketsModel> SocketsExecuteCallBack;
        /// <summary>
        /// Sockets通讯监控模块
        /// </summary>
        private TcpListener server;
        private TcpClient mClient;
        private NetworkStream mStream;
        #endregion 
        #endregion

        #region Property
        #endregion

        #region Public Function

        #region old function
        /// <summary>
        /// 启动Sockets端口通讯监控
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool StartReceive(int port)
        {
            this.server = new TcpListener(IPAddress.Any, port);
            this.server.Start();

            Task.Run(() => {
                while (true)
                {
                    try
                    {
                        if (this.server == null) break;

                        //2.1 收到请求
                        TcpClient client = this.server.AcceptTcpClient(); //停在这等待连接请求
                        NetworkStream stream = client.GetStream();

                        //2.2 解析数据,长度<1024字节
                        byte[] Request = new byte[2048];
                        int length = stream.Read(Request, 0, Request.Length);


                        SocketsModel reqModel = ByteHelper.BytesToObject<SocketsModel>(Request);
                        SocketsModel resModel = new SocketsModel();

                        if (this.SocketsExecuteCallBack != null)
                        {
                            resModel = this.SocketsExecuteCallBack(reqModel);
                        }

                        byte[] Response = ByteHelper.ObjectToBytes<SocketsModel>(resModel);

                        //2.3 返回状态
                        stream.Write(Response, 0, Response.Length);
                        //2.4 关闭客户端
                        stream.Close();
                    }
                    catch
                    {
                    }
                }
            });

            return true;
        }

        /// <summary>
        /// 通过Sockets端口发送数据
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Request"></param>
        /// <param name="port"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public SocketsModel SendMessage(string ip, SocketsModel Request, int port, int size = 2048)
        {
            try
            {
                //1.发送数据
                TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                byte[] bytRequest = ByteHelper.ObjectToBytes<SocketsModel>(Request);
                stream.Write(bytRequest, 0, bytRequest.Length);

                //2.接收状态,长度<1024字节
                byte[] bytes = new Byte[size];
                stream.ReadTimeout = 10000;
                stream.Read(bytes, 0, bytes.Length);

                //3.关闭对象
                stream.Close();
                client.Close();

                SocketsModel refRequest = ByteHelper.BytesToObject<SocketsModel>(bytes);

                return ByteHelper.BytesToObject<SocketsModel>(bytes);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorCodeLog(ErrorCodes.E23_1003, ex);

                SocketsModel Response = new SocketsModel();
                Response.hResult = ConstHelper.CNT_ERROR;
                // add by fan danpeng 20171116
                Response.errorCode = ErrorCodes.E23_1003;

                return Response;
            }
        }


        #endregion

        #region XXP Coin        
        public XXPSocketsModel XXPSendMessage(string ip, XXPSocketsModel Request, int port, int size = 2048)
        {
            try
            {
                //1.发送数据
                TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();

                byte[] bytRequest = ByteHelper.ObjectToBytes<XXPSocketsModel>(Request);
                stream.Write(bytRequest, 0, bytRequest.Length);

                //2.接收状态,长度<1024字节
                byte[] bytes = new Byte[size];
                stream.ReadTimeout = 10000;
                stream.Read(bytes, 0, bytes.Length);

                //3.关闭对象
                stream.Close();
                client.Close();

                XXPSocketsModel refRequest = ByteHelper.BytesToObject<XXPSocketsModel>(bytes);

                return ByteHelper.BytesToObject<XXPSocketsModel>(bytes);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);

                XXPSocketsModel Response = new XXPSocketsModel();
                Response.Type = XXPCoinMsgType.Exception;
                Response.Value = ex.Message;


                return Response;
            }
        }

        public bool XXPStartReceiveMsg(int port)
        {
            this.server = new TcpListener(IPAddress.Any, port);
            this.server.Start();

            Task.Run(() => {
                while (true)
                {
                    try
                    {
                        if (this.server == null) break;

                        //2.1 收到请求
                        TcpClient client = this.server.AcceptTcpClient(); //停在这等待连接请求
                        NetworkStream stream = client.GetStream();

                        //2.2 解析数据,长度<1024字节
                        byte[] Request = new byte[2048];
                        int length = stream.Read(Request, 0, Request.Length);


                        XXPSocketsModel reqModel = ByteHelper.BytesToObject<XXPSocketsModel>(Request);
                        //XXPSocketsModel resModel = new XXPSocketsModel();
                        string strTemp = client.Client.RemoteEndPoint.ToString();
                        reqModel.IpAddress = strTemp.Substring(0, strTemp.IndexOf(":"));
                        if (this.XXPSocketsExecuteCallBack != null)
                        {
                            reqModel = this.XXPSocketsExecuteCallBack(reqModel);
                        }

                        byte[] Response = ByteHelper.ObjectToBytes<XXPSocketsModel>(reqModel);

                        //2.3 返回状态
                        stream.Write(Response, 0, Response.Length);
                        //2.4 关闭客户端
                        stream.Close();
                    }
                    catch(Exception ex)
                    {
                        LogHelper.WriteErrorLog(ex.Message);
                    }
                }
            });

            return true;
        }

        public bool OpenFileTransConnect(string ip, int port)
        {
            try
            {
                this.XXPfileClient = new TcpClient(ip, port);
                this.XXPfileStream = this.mClient.GetStream();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

        public bool CloseFileTransConnect()
        {
            try
            {
                //3.关闭对象
                this.XXPfileClient.Close();
                this.XXPfileStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

        public int StartSendFile(string filePath)
        {
            try
            {
                //创建文件流  
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] fileBuffer = new byte[1024];
                int bytesRead;
                int totalBytes = 0;
                do
                {
                    Thread.Sleep(10);//模拟远程传输视觉效果,暂停10秒  
                    bytesRead = fs.Read(fileBuffer, 0, fileBuffer.Length);
                    this.XXPfileStream.Write(fileBuffer, 0, bytesRead);
                    totalBytes += bytesRead;

                } while (bytesRead > 0);
                return totalBytes;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return 0;
            }
        }

        public int StartReceivefile(string savePath, string IP, int port )
        {
            try
            {
                this.server = new TcpListener(IPAddress.Any, port);
                this.server.Start();
                if (this.server == null) return 0;

                //收到请求
                TcpClient client = this.server.AcceptTcpClient(); //停在这等待连接请求
                NetworkStream stream = client.GetStream();


                byte[] fileBuffer = new byte[1024];//每次收1KB  
                FileStream fs = new FileStream(savePath, FileMode.CreateNew, FileAccess.Write);

                //从缓存Buffer中读入到文件流中  
                int ibytesRead;
                int itotalBytes = 0;
                do
                {
                    ibytesRead = stream.Read(fileBuffer, 0, fileBuffer.Length);

                    fs.Write(fileBuffer, 0, ibytesRead);
                    itotalBytes += ibytesRead;

                } while (ibytesRead > 0);
                return itotalBytes;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return 0;
            }

        }




        #endregion 



        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            LogHelper.WriteMethodLog(true);
            if (this.server != null)
            {
                this.server.Stop();
                this.server = null;
            }
            LogHelper.WriteMethodLog(false);
        }
        #endregion
    }
}