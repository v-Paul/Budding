using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTMC.Utils.Models;

namespace VTMC.Utils
{
    /// <summary>
    /// 程序启动公共类工具类
    /// </summary>
    public class StartupHelper
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public StartupHelper()
        {

        }
        #endregion

        /// <summary>
        /// 执行开始程序
        /// </summary>
        public void RunningStartup(StartupType startupType, params string[] args)
        {
            switch (startupType)
            {
                case StartupType.MVTMClientStartup:
                    // 读取StartupConfiguration.xml
                    AppSettings.StartupConfiguration = new VTMC.Utils.Models.StartupConfiguration();
                    //AnalysisStartupConfiguration();
                    // add by fdp 180521
                    AnalysisStartupConfiguration(args);
                    break;
                case StartupType.MVTMC:
                    // 读取ComonConfiguration.xml
                    AppSettings.CommonConfiguration = new VTMC.Utils.Models.CommonConfiguration();
                    AnalysisComonConfiguration();
                    // 读取并更新到每个User目录下客户端数据
                    AppSettings.UserConfiguration = new VTMC.Utils.Models.UserConfiguration();
                    UpdateUserConfiguration();
                    break;
                case StartupType.VideoRecordTool:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 分析StartupConfiguration.xml
        /// </summary>
        /// <param name="args">MVTM Startup 传入的参数</param>
        private void AnalysisStartupConfiguration(string[] args)
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                if (args != null && args.Length > 0)
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        LogHelper.WriteInfoLog(string.Format("AnalysisStartupConfiguration args[{0}]:{1}", i, args[i]));
                    }
                }
                ReadStartupConfiguration(args);
                InitRunProcess(AppSettings.StartupConfiguration.ProcessList);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("AnalysisStartupConfiguration Exception", ex);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 分析ComonConfiguration.xml
        /// </summary>
        public void AnalysisComonConfiguration()
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                ReadComonConfiguration();
                InitRunProcess(AppSettings.CommonConfiguration.ProcessList);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("AnalysisComonConfiguration Exception", ex);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 读取ComonConfiguration.xml
        /// </summary>
        private void ReadComonConfiguration()
        {
            VTMC.Utils.Models.ProcessStartInfo clearIECacheProcess = new VTMC.Utils.Models.ProcessStartInfo()
            { RunningMethod = "ClearIECache", FileName = "RunDll32.exe", WaitForExit = true, Args = "InetCpl.cpl,ClearMyTracksByProcess 4351" };
            List<VTMC.Utils.Models.ProcessStartInfo> processList = new List<VTMC.Utils.Models.ProcessStartInfo>();
            processList.Add(clearIECacheProcess);
            AppSettings.CommonConfiguration.ProcessList = processList;
            string xmlPath = Path.Combine(System.Windows.Forms.Application.StartupPath, AppSettings.CommonConfiguration.GetType().Name + ConstHelper.XMLExtension);
            if (!File.Exists(xmlPath))
            {
                XMLHelper.SerializeToXmlFile(xmlPath, AppSettings.CommonConfiguration);
            }
            else
            {
                AppSettings.CommonConfiguration = XMLHelper.DeserializeFromXmlFile<VTMC.Utils.Models.CommonConfiguration>(xmlPath);
            }
        }

        /// <summary>
        /// 读取StartupConfiguration.xml
        /// </summary>
        private void ReadStartupConfiguration(string[] args)
        {
            VTMC.Utils.Models.ProcessStartInfo startMVTMCProcess = new VTMC.Utils.Models.ProcessStartInfo()
            { RunningMethod = ConstHelper.MVTMClientRunningMethod, FileName = ConstHelper.MVTMClientName, WaitForExit = false, WorkingDir = ConstHelper.MVTMClientWorkingDir };
            //{ RunningMethod = "InitRunProcess", FileName = "MVTMClient.exe", WaitForExit = false,WorkingDir= @"C:\Program Files\ChinaSofti\MVTMClient\MVTMCExe"};
  
            //add by fdp 180521
            if (args?.Length >= 1)
            {
                foreach (string arg in args)
                {
                    startMVTMCProcess.Args += arg + ' ';
                }
            }

            List<VTMC.Utils.Models.ProcessStartInfo> processList = new List<VTMC.Utils.Models.ProcessStartInfo>();
            processList.Add(startMVTMCProcess);
            AppSettings.StartupConfiguration.ProcessList = processList;
            string xmlPath = Path.Combine(System.Windows.Forms.Application.StartupPath, AppSettings.StartupConfiguration.GetType().Name + ConstHelper.XMLExtension);
            if (!File.Exists(xmlPath))
            {
                XMLHelper.SerializeToXmlFile(xmlPath, AppSettings.StartupConfiguration);
            }
            else
            {
                AppSettings.StartupConfiguration = XMLHelper.DeserializeFromXmlFile<VTMC.Utils.Models.StartupConfiguration>(xmlPath);
                //add by fdp 180521, MVTMClient进程不再配置在xml中，通过程序加入参数后写入list
                AppSettings.StartupConfiguration.ProcessList.Add(startMVTMCProcess);
            }
        }


        /// <summary>
        /// 启动配置文件指定程序
        /// </summary>
        private void InitRunProcess(List<ProcessStartInfo> ProcessList)
        {
            if (ProcessList != null && ProcessList.Count > 0)
            {
                foreach (VTMC.Utils.Models.ProcessStartInfo processStartInfo in ProcessList)
                {
                    if (processStartInfo.RunningMethod == System.Reflection.MethodBase.GetCurrentMethod().Name)
                    {
                        //判断是否要Kill
                        if (processStartInfo.KillProcessBeforeStart == "1")
                        {
                            //如果存在该进程就Kill
                            if (ProcessHelper.IsProcessRun(Path.GetFileNameWithoutExtension(processStartInfo.FileName)))
                            {
                                ProcessHelper.KillProcess(Path.GetFileNameWithoutExtension(processStartInfo.FileName));
                            }
                        }
                        //判断该进程存在否，不存在，则起动，如果存在则不启动
                        if (!ProcessHelper.IsProcessRun(Path.GetFileNameWithoutExtension(processStartInfo.FileName)))
                        {
                            bool exitCode = ProcessHelper.RunProcess(processStartInfo.FileName, processStartInfo.WaitForExit, processStartInfo.Args, processStartInfo.WorkingDir);
                            LogHelper.WriteInfoLog(string.Format("{0}:{1}", processStartInfo.RunningMethod, processStartInfo.FileName));
                        }
                        // add by fdp 180521 进程已存在，如果是MVTMClient，则继续run
                        else
                        {
                            if(processStartInfo.FileName == ConstHelper.MVTMClientName)
                            {
                                bool exitCode = ProcessHelper.RunProcess(processStartInfo.FileName, processStartInfo.WaitForExit, processStartInfo.Args, processStartInfo.WorkingDir);
                                LogHelper.WriteInfoLog(string.Format("{0}:{1}", processStartInfo.RunningMethod, processStartInfo.FileName));
                            }
                        }

                        //判断该进程已启动
                        bool isProcessRun = ProcessHelper.IsProcessRun(Path.GetFileNameWithoutExtension(processStartInfo.FileName));
                        LogHelper.WriteInfoLog(string.Format("{0},{1},{2}", "ProcessHelper.IsProcessRun", isProcessRun, processStartInfo.FileName));
                    }
                }
            }
        }

        /// <summary>
        /// 读取并更新到每个User目录下客户端数据
        /// </summary>
        public void UpdateUserConfiguration()
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                string xmlPath = Path.Combine(AppSettings.ConfigurationSaveFolder, AppSettings.UserConfiguration.GetType().Name + ConstHelper.XMLExtension);
                if (!File.Exists(xmlPath))
                {
                    XMLHelper.SerializeToXmlFile(xmlPath, AppSettings.UserConfiguration);
                }
                else
                {
                    AppSettings.UserConfiguration = XMLHelper.DeserializeFromXmlFile<VTMC.Utils.Models.UserConfiguration>(xmlPath);
                }

                if (UpdateJSVersion(xmlPath))
                {
                    XMLHelper.SerializeToXmlFile(xmlPath, AppSettings.UserConfiguration);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("UpdateUserConfiguration Exception", ex);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }


        /// <summary>
        /// Client端本地版本对比服务端js版本，来决定是否清缓
        /// </summary>
        public bool UpdateJSVersion(string xmlPath)
        {
            #region 清理IE缓存,失败如果，retry 3次
            bool updateXML = false;
            try
            {
                string requestServerUrl = AppSettings.GetJSVersionService.Replace("{JavaHost}", AppSettings.JavaHost);
                string httpResponseJson = string.Empty;
                System.Net.HttpStatusCode httpStatusCode = HttpHelper.HttpPostRequest(requestServerUrl, string.Empty, ref httpResponseJson);
                if (httpStatusCode.Equals(System.Net.HttpStatusCode.OK))
                {
                    JObject obj = JObject.Parse(httpResponseJson);
                    if (obj.GetValue("status").ToString().ToUpperInvariant().Equals("000"))
                    {
                        string data = obj.GetValue("data").ToString().Replace("[", "").Replace("]", "");
                        JObject objdata = JObject.Parse(data);
                        httpResponseJson = objdata.GetValue("jsVersion").ToString();
                        if (!httpResponseJson.Equals(AppSettings.UserConfiguration.JSInformation.JSVersion))
                        {
                            ClearIECache();
                            AppSettings.UserConfiguration.JSInformation.JSVersion = httpResponseJson;
                            updateXML = true;
                        }
                    }
                }
                else
                {
                    LogHelper.WriteErrorLog(string.Format("Post HttpResponseCode:{0},Url:{1},Response:{2}", httpStatusCode.ToString(), requestServerUrl, httpResponseJson));
                    ClearIECache();
                }
                return updateXML;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(" 清理IE缓存异常", ex);
                return updateXML;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
            #endregion
        }

        /// <summary>
        /// 清理IE缓存
        /// </summary>
        private void ClearIECache()
        {
            string currentMethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            VTMC.Utils.Models.ProcessStartInfo clearIECacheProcess = AppSettings.CommonConfiguration.ProcessList.FirstOrDefault(it => it.RunningMethod == currentMethodName);
            if (clearIECacheProcess != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    bool exitCode = ProcessHelper.RunProcess(clearIECacheProcess.FileName, clearIECacheProcess.WaitForExit, clearIECacheProcess.Args);
                    if (exitCode)
                    {
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

    }

    /// <summary>
    /// 开始类型
    /// </summary>
    public enum StartupType
    {
        /// <summary>
        /// MVTMC开始程序
        /// </summary>
        MVTMClientStartup,
        /// <summary>
        /// MVTMC程序
        /// </summary>
        MVTMC,
        /// <summary>
        /// 录屏工具
        /// </summary>
        VideoRecordTool
    }
}
