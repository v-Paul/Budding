using System;
using System.Net;
using System.Net.Sockets;
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
        public Func<XXPSocketsModel, XXPSocketsModel> XXPSocketsExecuteCallBack;

        public Func<SocketsModel, SocketsModel> SocketsExecuteCallBack;
        /// <summary>
        /// Sockets通讯监控模块
        /// </summary>
        private TcpListener server;

        private TcpClient mClient;
        private NetworkStream mStream;
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


        /// <summary>
        /// GRG P2600 存款模块发送socket命令
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Request"></param>
        /// <param name="port"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public string P2600SendMessage(string ip, P2600CmdSocketsModel Request, int port, int socketTimeout=60000, int size = 2048)
        {
            try
            {
                LogHelper.WriteMethodLog(true);

                byte[] bytes = new Byte[size];
                //1.发送数据
                using (TcpClient client = new TcpClient(ip, port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {

                        byte[] bytRequest = Request.AssembleCmdMsg();
                        stream.Write(bytRequest, 0, bytRequest.Length);

                        //2.接收状态,长度<1024字节
                        //byte[] bytes = new Byte[size];
                        // modify fdp 180927 socket超时时间有bridge层传下来
                        stream.ReadTimeout = socketTimeout;
                        stream.Read(bytes, 0, bytes.Length);

                        //3.关闭对象
                        stream.Close();
                    }
                    client.Close();
                }

                string strRcv = System.Text.Encoding.Default.GetString(bytes);
                //P2600RevSocketsModel refRequest = ByteHelper.BytesToObject<P2600RevSocketsModel>(bytes);
                if(strRcv.Length > 100 )
                {
                    LogHelper.WriteInfoLog("P2600SendMessage return :" + strRcv.Substring(0,100) );
                }
                else
                {
                    LogHelper.WriteInfoLog("P2600SendMessage return :" + strRcv);
                }
                return strRcv.Trim();
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return ConstHelper.CNT_ERROR;
            }
            finally
            {

                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// socket 短链接只发送命令，不等返回值，用于P2600 cancel 中
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="Request"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public string P2600SCSendCmd(string ip, P2600CmdSocketsModel Request, int port)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                            
                //1.发送数据
                using (TcpClient client = new TcpClient(ip, port))
                {
                    using (NetworkStream stream = client.GetStream())
                    {

                        byte[] bytRequest = Request.AssembleCmdMsg();
                        stream.Write(bytRequest, 0, bytRequest.Length);
                        //3.关闭对象
                        stream.Close();
                    }
                    client.Close();
                }

                
                return ConstHelper.CNT_SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return ConstHelper.CNT_ERROR;
            }
            finally
            {

                LogHelper.WriteMethodLog(false);
            }
        }



        /// <summary>
        /// 打开socket长连接
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool OpenLongConnect(string ip, int port)
        {
            try
            {
                this.mClient = new TcpClient(ip, port);
                this.mStream = this.mClient.GetStream();
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 关闭socket长连接
        /// </summary>
        /// <returns></returns>
        public bool CloseLongConnect()
        {
            try
            {
                //3.关闭对象
                this.mClient.Close();
                this.mStream.Close();
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 通过长连接发送socket命令
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public string P2600LCSendCmd(P2600CmdSocketsModel Request)
        {
            try
            {
                LogHelper.WriteMethodLog(true);
                //1.发送数据
                byte[] bytRequest = Request.AssembleCmdMsg();
                this.mStream.Write(bytRequest, 0, bytRequest.Length);

                return ConstHelper.CNT_SUCCESS;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                return ConstHelper.CNT_ERROR;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 建立长连接后，timer获取socket数据
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string P2600GetMessage( int size = 2048)
        {
            try
            {

                string strRcv = string.Empty;
                if (this.mStream.DataAvailable)
                {
                    byte[] bytes = new Byte[size];
                    this.mStream.ReadTimeout = 10000;
                    this.mStream.Read(bytes, 0, bytes.Length);
                    strRcv = System.Text.Encoding.Default.GetString(bytes).Trim();
                    //LogHelper.WriteInfoLog("P2600GetMessage strRcv: " + strRcv.Substring(0, 100));
                }
                if (strRcv.Length > 100)
                {
                    LogHelper.WriteInfoLog("P2600GetMessage Substring100 :" + strRcv.Substring(0, 100));
                }
                else if(strRcv.Length != 0)
                {
                    LogHelper.WriteInfoLog("P2600GetMessage return :" + strRcv);
                }
                return strRcv.Trim();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion



        public bool XXPCoinStartReceiveMsg(int port)
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
                        XXPSocketsModel resModel = new XXPSocketsModel();

                        if (this.XXPSocketsExecuteCallBack != null)
                        {
                            resModel = this.XXPSocketsExecuteCallBack(reqModel);
                        }

                        byte[] Response = ByteHelper.ObjectToBytes<XXPSocketsModel>(resModel);

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
