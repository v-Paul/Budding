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
using VTMC.Utils;
namespace BaseSturct
{
    class Communication
    {

        /// <summary>
        /// Sockets操作类
        /// </summary>
        private SocketsHelper SocketsHelp;
        public Communication()
        {
            this.SocketsHelp = new SocketsHelper();

           // this.SocketsHelp.SocketsExecuteCallBack = this.SocketsExecuteCallBack;
            if (!this.SocketsHelp.StartReceive(Convert.ToInt32(AppSettings.XXPCommport)));
            {
                LogHelper.WriteErrorLog("SocketsHelp.StartReceive fail");
            }
        }

        private SocketsModel SocketsExecuteCallBack(SocketsModel mod)
        {
            switch (mod.Code)
            {
                case RecordEvent.OpenComplete:
                    
                    break;
                case RecordEvent.StartComplete:
                    break;
                case RecordEvent.StopComplete:
                   
                    break;
                case RecordEvent.StatusEvent:
                    break;
                case RecordEvent.ErrorEvent:
                    break;
                default:
                    mod.Code = RecordEvent.ErrorEvent;
                    break;
            }

           // this.ExecuteCallBack(mod.Code, mod);

            SocketsModel refMod = new SocketsModel();
            refMod.hResult = ConstHelper.CNT_SUCCESS;
            return refMod;
        }




    }
}
