using System;
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

        private TcpListener server;

        // for transfer file
        private TcpClient XXPfileClient;
        private NetworkStream XXPfileStream;
        /// <summary>
        /// 回调事件
        /// </summary>
        public Func<XXPSocketsModel,XXPSocketsModel> XXPSocketsExecuteCallBack;

        #endregion

        #region Property
        #endregion

        #region Public Function

        #region XXP Coin        
        public XXPSocketsModel XXPSendMessage(string ip, XXPSocketsModel Request, int port, int iTimeout = 10000,int size = 2048)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                //1.发送数据
                TcpClient client = new TcpClient(ip, port);
                NetworkStream stream = client.GetStream();


                byte[] bytRequest = ByteHelper.ObjectToBytes<XXPSocketsModel>(Request);
                
                byte[] bylength = BitConverter.GetBytes(bytRequest.Length);
                stream.Write(bylength, 0, bylength.Length);

                //byte[] tmp = new byte[bylength.Length + bytRequest.Length];
                //System.Buffer.BlockCopy(bylength, 0, tmp, 0, bylength.Length);
                //System.Buffer.BlockCopy(bytRequest, 0, tmp, bylength.Length, bytRequest.Length);
                if(bytRequest.Length>2048)
                {
                    int iTimes = bytRequest.Length / 2048;
                    int iLast = bytRequest.Length % 2048;
                    for (int i = 0; i < iTimes; i++)
                    {
                        stream.Write(bytRequest, i * 2048, 2048);
                        Thread.Sleep(10);
                    }
                    if (iLast != 0)
                    {
                        stream.Write(bytRequest, iTimes * 2048, iLast);
                    }
                }
                else
                {
                    stream.Write(bytRequest, 0, bytRequest.Length);
                }




                //2.接收状态,长度<1024字节
                byte[] bytes = new Byte[size];
                stream.ReadTimeout = iTimeout;
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
            finally
            {
                LogHelper.WriteMethodLog(false);
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

                        byte[] byLength = new byte[4];
                        int retLength = stream.Read(byLength, 0, byLength.Length);


                        int iReqLength = BitConverter.ToInt32(byLength, 0);
                        byte[] Request = new byte[iReqLength];
                        if (iReqLength>2048)
                        {
                            int itotalLength = 0;
                            while (itotalLength != iReqLength)
                            {
                                int length = stream.Read(Request, itotalLength, 2048);
                                itotalLength += length;

                            }
                        }
                        else
                        {
                        int length = stream.Read(Request, 0, Request.Length);
                        }

                        XXPSocketsModel reqModel = ByteHelper.BytesToObject<XXPSocketsModel>(Request);
                       
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
                this.XXPfileStream = this.XXPfileClient.GetStream();
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
                this.XXPfileClient?.Close();
                this.XXPfileStream?.Close();
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
                    Thread.Sleep(10);
                    bytesRead = fs.Read(fileBuffer, 0, fileBuffer.Length);
                    this.XXPfileStream.Write(fileBuffer, 0, bytesRead);
                    totalBytes += bytesRead;

                } while (bytesRead > 0);
                fs.Close();
                return totalBytes;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return 0;
            }
        }

        public long StartReceivefile(string savePath, string IP, int port , long Size)
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
                long itotalBytes = 0;
                while (itotalBytes != Size)
                {
                    ibytesRead = stream.Read(fileBuffer, 0, fileBuffer.Length);

                    fs.Write(fileBuffer, 0, ibytesRead);
                    itotalBytes += ibytesRead;

                }
                fs.Close();
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
            this.CloseFileTransConnect();
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
