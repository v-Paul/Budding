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
    public class NamedPipeClientHelper
    {
        #region Field
        //通讯管道客户端
        private NamedPipeClientStream pipeClient;
        private static NamedPipeClientHelper lsNamedPipeClient;
        /// <summary>
        /// 回调方法
        /// </summary>
        public Action<NamedPipeModel> NamedPipeCallBack;
        #endregion

        private NamedPipeClientHelper()
        {
        }

        #region Method
        /// <summary>
        /// 获取单实例
        /// </summary>
        /// <returns></returns>
        public static NamedPipeClientHelper GetInstance(string pipeServerName)
        {
            try
            {
                if (lsNamedPipeClient == null)
                {
                    lsNamedPipeClient = new NamedPipeClientHelper();
                }
                lsNamedPipeClient.pipeClient = new NamedPipeClientStream("127.0.0.1", pipeServerName,
                            PipeDirection.InOut, PipeOptions.Asynchronous,
                            TokenImpersonationLevel.None);

                return lsNamedPipeClient;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("GetInstance", ex);
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
        public bool SendMessage(NamedPipeModel message)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                StreamWriter sw = new StreamWriter(this.pipeClient);
                //StreamReader sr = new StreamReader(this.pipeClient);
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

        /// <summary>
        /// 接收消息
        /// </summary>
        public void ReceiverMessage()
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                pipeClient.Connect(5000);

                ThreadPool.QueueUserWorkItem(delegate
                {
                    StreamReader sr = new StreamReader(pipeClient);
                    
                    string result = null;
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

        public void ClosePipeClient()
        {
            this.pipeClient.Close();
        }
        #endregion
    }
}
