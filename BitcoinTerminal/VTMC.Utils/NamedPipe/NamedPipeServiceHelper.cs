using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Principal;
using System.IO;
using System.Threading;
using System.Web;

namespace VTMC.Utils
{
    public class NamedPipeServiceHelper
    {
        #region Field
        private NamedPipeServerStream pipeServer;
        private static NamedPipeServiceHelper lsNamedPipeServic;
        /// <summary>
        /// 回调方法
        /// </summary>
        public Action<NamedPipeModel> NamedPipeCallBack;
        #endregion

        private NamedPipeServiceHelper()
        {
        }

        #region Method
        /// <summary>
        /// 获取单实例
        /// </summary>
        /// <returns></returns>
        public static NamedPipeServiceHelper GetInstance(string pipeServerName)
        {
            LogHelper.WriteMethodLog(true);

            try
            {
                if (lsNamedPipeServic == null)
                {
                    lsNamedPipeServic = new NamedPipeServiceHelper();
                }

                lsNamedPipeServic.pipeServer = new NamedPipeServerStream(pipeServerName, PipeDirection.InOut, 4, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                return lsNamedPipeServic;

            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
        
        /// <summary>
        /// 接收消息
        /// </summary>
        public void ReceiverMessage()
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    pipeServer.BeginWaitForConnection((iAsyncResult) => {
                        NamedPipeServerStream server = (NamedPipeServerStream)iAsyncResult.AsyncState;
                        server.EndWaitForConnection(iAsyncResult);
                        StreamReader sr = new StreamReader(server);
                        //StreamWriter sw = new StreamWriter(server);
                        string result = null;
                        string clientName = server.GetImpersonationUserName();
                        while (true)
                        {
                            result = sr.ReadLine();
                            if (result == null || result == "End")
                                continue;

                            NamedPipeModel mod = JsonHelper.Deserialize<NamedPipeModel>(result);

                            if (this.NamedPipeCallBack != null)
                            {
                                this.NamedPipeCallBack(mod);
                            }
                        }
                    }, pipeServer);
                });
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("ReceiverMessage", ex);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        public bool SendMessage(NamedPipeModel message)
        {
            LogHelper.WriteMethodLog(true);
            try
            {

                StreamWriter sw = new StreamWriter(this.pipeServer);
                sw.AutoFlush = true;
                string json = JsonHelper.Serializer<NamedPipeModel>(message);
                sw.WriteLine(json);

                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("ReceiverMessage", ex);
                return false;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        public void ClosePipeServer()
        {
            pipeServer.Close();
        }

        #endregion
    }
}
