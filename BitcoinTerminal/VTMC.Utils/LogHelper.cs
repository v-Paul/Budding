/*************************************************
*Author:yeshaowei
*Date:2016/12/27 15:43:28
*Des:日志
************************************************/ 
using log4net;
using log4net.Appender;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace VTMC.Utils
{
    /// <summary>
    /// 系统日志处理
    /// </summary>
    public static class LogHelper
    {
        #region Consts
        /// <summary>
        /// Log类型
        /// </summary>
        public enum LogType
        {
            /// <summary>
            /// 消息
            /// </summary>
            Info,
            /// <summary>
            /// 警告
            /// </summary>
            Warn,
            /// <summary>
            /// 调试
            /// </summary>
            Debug,
            /// <summary>
            /// 错误
            /// </summary>
            Error,
            /// <summary>
            /// 致命
            /// </summary>
            Fatal
        }
        #endregion

        #region Private Members
        /// <summary>
        /// 日志Log4
        /// </summary>
        private static ILog log = LogManager.GetLogger(ConstHelper.MVTMClientLoggerName);


        /// <summary>
        /// 视频录制日志Log
        /// </summary>
        private static ILog recordLog = LogManager.GetLogger(ConstHelper.RecordToolLoggerName);

        /// <summary>
        /// log4net实例用于写前端log
        /// </summary>
        private static ILog fslog = LogManager.GetLogger(ConstHelper.FSLoggerName);
        #endregion

        #region Properties

        #endregion

        #region Public Methods
        /// <summary>
        /// 函数调用入出口日志记录
        /// </summary>
        /// <param name="IsStart"></param>
        /// <param name="meth"></param>
        public static void WriteMethodLog(bool IsStart)
        {
            string logInfo = string.Empty;

            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();
            string paraStr = string.Empty;
            foreach (ParameterInfo para in mb.GetParameters())
            {
                paraStr += para.ParameterType.Name + " " + para.Name + ",";
            }
            if (!string.IsNullOrEmpty(paraStr))
            {
                paraStr = paraStr.Substring(0, paraStr.Length - 1);
            }

            if (IsStart)
            {
                logInfo = "开始调用[" + mb.DeclaringType.FullName
                            + "." + mb.Name + "(" + paraStr + ")" + "]方法。";
            }
            else
            {
                logInfo = "结束调用[" + mb.DeclaringType.FullName
                            + "." + mb.Name + "(" + paraStr + ")" + "]方法。";
            }

            log.Info(logInfo);
        }

        /// <summary>
        /// 信息Log日志记录
        /// </summary>
        /// <param name="Info"></param>
        public static void WriteInfoLog(string Info)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = "[" + mb.DeclaringType.FullName
               + "." + mb.Name + "]：" + Info;

            log.Info(logInfo);
        }

        /// <summary>
        /// 警告Log日志记录
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="Info"></param>
        public static void WriteWarnLog(string Info)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = "[" + mb.DeclaringType.FullName
               + "." + mb.Name + "]：" + Info;

            log.Warn(logInfo);
        }

        ///// <summary>
        ///// 异常Log日志记录
        ///// </summary>
        ///// <param name="meth"></param>
        ///// <param name="Info"></param>
        //public static void WriteErrorLog(Exception ex)
        //{
        //    StackTrace stack = new StackTrace(true);
        //    MethodBase mb = stack.GetFrame(1).GetMethod();

        //    string logInfo = "[" + mb.DeclaringType.FullName
        //      + "." + mb.Name + "]：";

        //    log.Error(logInfo, ex);
        //}

        /// <summary>
        /// 异常Log日志记录
        /// </summary>
        /// <param name="Info"></param>
        public static void WriteErrorLog(string info)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = "[" + mb.DeclaringType.FullName
              + "." + mb.Name + "]：" + info;

            log.Error(logInfo);
        }

        /// <summary>
        /// 异常Log日志记录
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="Info"></param>
        public static void WriteErrorCodeLog(string ErrCode,Exception ex)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = "[" + mb.DeclaringType.FullName
              + "." + mb.Name + "]：" + ErrCode + ":"+ ResourceHelper.GetResourceInfo(ErrCode);

            log.Error(logInfo,ex);
        }

        /// <summary>
        /// 异常Log日志记录
        /// </summary>
        /// <param name="meth"></param>
        /// <param name="Info"></param>
        public static void WriteErrorInfoLog(string info, Exception ex)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = "[" + mb.DeclaringType.FullName
              + "." + mb.Name + "]：" + info;

            log.Error(logInfo, ex);
        }

        /// <summary>
        /// 写Log
        /// </summary>
        /// <param name="enuLogType">Log类型</param>
        /// <param name="objMsg">Log内容</param>
        public static void WriteLog(LogType enuLogType, object objMsg, Exception ex = null)
        {
            switch (enuLogType)
            {
                case LogType.Info:
                    log.Info(objMsg, ex);
                    break;
                case LogType.Warn:
                    log.Warn(objMsg, ex);
                    break;
                case LogType.Debug:
                    log.Debug(objMsg, ex);
                    break;
                case LogType.Error:
                    log.Error(objMsg, ex);
                    break;
                case LogType.Fatal:
                    log.Fatal(objMsg, ex);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 输出方法信息(包括参数值)
        /// </summary>
        /// <param name="args"></param>
        public static void WriteMethodInfoLog(params object[] args)
        {
            StackTrace stackTrace = new StackTrace();
            ParameterInfo[] parameterInfo = stackTrace.GetFrame(1).GetMethod().GetParameters();
            StringBuilder logMessage = new StringBuilder();
            logMessage.Append("调用方法：" + stackTrace.GetFrame(1).GetMethod().Name);
            if (parameterInfo != null && parameterInfo.Length > 0)
            {
                if (args != null && args.Length > 0)
                {
                    if (parameterInfo.Length == args.Length)
                    {
                        for (int index = 0; index < parameterInfo.Length; index++)
                        {
                            logMessage.Append("\t参数" + parameterInfo[index].Name + ":" + args[index]);
                        }
                    }
                    else
                    {
                        logMessage.Append("\t日志获取参数和方法参数不一致");
                    }
                }
                else
                {
                    logMessage.Append("\t日志获取参数和方法参数不一致");
                }

            }
            log.Info(logMessage.ToString());
        }

        /// <summary>
        /// 前端log写入方法
        /// </summary>
        /// <param name="logStr"></param>
        public static void WriteFrontSideLog(string logStr)
        {
            fslog.Info(logStr);
        }

        /// <summary>
        /// 写Log
        /// </summary>
        /// <param name="logType">Log类型</param>
        /// <param name="objMsg">Log内容</param>
        public static void WriteFrontSideLog(string logType, string logStr)
        {
            switch (logType)
            {
                case "Info":
                    fslog.Info(logStr);
                    break;
                case "Warn":
                    fslog.Warn(logStr);
                    break;
                case "Debug":
                    fslog.Debug(logStr);
                    break;
                case "Error":
                    fslog.Error(logStr);
                    break;
                case "Fatal":
                    fslog.Fatal(logStr);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// VideoRecord函数调用入出口日志记录
        /// </summary>
        /// <param name="IsStart"></param>
        /// <param name="meth"></param>
        public static void WriteMethodRecordtoolLog(bool IsStart)
        {
            string logInfo = string.Empty;

            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();
            string paraStr = string.Empty;
            foreach (ParameterInfo para in mb.GetParameters())
            {
                paraStr += para.ParameterType.Name + " " + para.Name + ",";
            }
            if (!string.IsNullOrEmpty(paraStr))
            {
                paraStr = paraStr.Substring(0, paraStr.Length - 1);
            }

            if (IsStart)
            {
                logInfo = "开始调用[" + mb.DeclaringType.FullName
                            + "." + mb.Name + "(" + paraStr + ")" + "]方法。";
            }
            else
            {
                logInfo = "结束调用[" + mb.DeclaringType.FullName
                            + "." + mb.Name + "(" + paraStr + ")" + "]方法。";
            }

            recordLog.Info(logInfo);
        }

        /// <summary>
        /// VideoRecord写Log
        /// </summary>
        /// <param name="enuLogType">Log类型</param>
        /// <param name="objMsg">Log内容</param>
        public static void WriteRecordtoolLog(LogType enuLogType, string info, Exception ex = null)
        {
            StackTrace stack = new StackTrace(true);
            MethodBase mb = stack.GetFrame(1).GetMethod();

            string logInfo = string.Format("[{0}.{1}]：{2}",mb.DeclaringType.FullName,
                mb.Name, info);
            switch (enuLogType)
            {
                case LogType.Info:
                    recordLog.Info(logInfo, ex);
                    break;
                case LogType.Warn:
                    recordLog.Warn(logInfo, ex);
                    break;
                case LogType.Debug:
                    recordLog.Debug(logInfo, ex);
                    break;
                case LogType.Error:
                    recordLog.Error(logInfo, ex);
                    break;
                case LogType.Fatal:
                    recordLog.Fatal(logInfo, ex);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 获取Logger appender配置信息
        /// </summary>
        /// <param name="log">ILog</param>
        public static IAppender[] GetLoggerAppenders(ILog log)
        {
            AppenderCollection appenderCollection = ((log4net.Repository.Hierarchy.Logger)((log4net.Core.LoggerWrapperImpl)log).Logger).Appenders;
            IAppender[] appenders = (IAppender[])appenderCollection.SyncRoot;

            //string file = ((FileAppender)appenders[0]).File;
            return appenders;
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
