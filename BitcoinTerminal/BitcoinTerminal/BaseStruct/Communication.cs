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

            this.SocketsHelp.XXPSocketsExecuteCallBack = this.XXPSocketsExecuteCallBack;
            if (!this.SocketsHelp.XXPCoinStartReceiveMsg(Convert.ToInt32(AppSettings.XXPCommport)));
            {
                LogHelper.WriteErrorLog("SocketsHelp.StartReceive fail");
            }
        }

        private XXPSocketsModel XXPSocketsExecuteCallBack(XXPSocketsModel mod)
        {
            XXPSocketsModel refMod = new XXPSocketsModel();
            refMod.Type = XXPCoinEvent.ResponseEvent;

            switch (mod.Type)
            {
                case XXPCoinEvent.HandshakeEvent:
                    
                    break;
                case XXPCoinEvent.MessageEvent:
                    break;
                case XXPCoinEvent.ResponseEvent:
                   
                    break;
                case XXPCoinEvent.NewtransactionsEvent:
                    break;
                case XXPCoinEvent.NewNodeEvent:
                    break;
                default:
                    
                    break;
            }

           // this.ExecuteCallBack(mod.Code, mod);


            return refMod;
        }




    }
}
