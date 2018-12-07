using System;
using System.Diagnostics;

namespace VTMC.Utils
{
    /// <summary>
    /// 进程控制操作类
    /// </summary>
    public class ProcessHelper
    {
        /// <summary>
        /// 通过进程启动外部Exe文件
        /// </summary>
        /// <param name="fileName">启动外部的Exe文件或者Doc文件名称,全路径</param>
        /// <param name="wait">是否等待启动</param>
        /// <param name="args">参数</param>
        /// <param name="workingDir">默认工作目录</param>
        /// <returns></returns>
        public static bool RunProcess(string fileName, bool wait, string args = "", string workingDir = "")
        {
            LogHelper.WriteMethodLog(true);
            try
            {
                Process process = new Process();
                
                if(!string.IsNullOrEmpty(workingDir))
                {
                    process.StartInfo.WorkingDirectory = workingDir;
                }
                process.StartInfo.FileName = fileName;
                // 关闭Shell的使用
                process.StartInfo.UseShellExecute = false;
                // 重定向标准输入
                process.StartInfo.RedirectStandardInput = true;
                // 重定向标准输出
                process.StartInfo.RedirectStandardOutput = true;
                //重定向错误输出
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                if (!string.IsNullOrEmpty(args))
                {
                    process.StartInfo.Arguments = args;
                }
                process.Start();
                if (wait)
                {
                    process.WaitForExit();
                    LogHelper.WriteInfoLog(string.Format("{0} {1} -> ExitCode:{2}", fileName, args, process.ExitCode));
                    return process.ExitCode == 0 ? true : false;
                }
                else
                {
                    //LogHelper.WriteInfoLog(string.Format("{0} {1} -> HasExit:{2}", fileName, args, process.HasExited));
                    //iRet = process.HasExited ? 0:1;
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                return false;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 关闭所有打开的指定名字进程 
        /// </summary>
        /// <param name="processName"></param>
        public static void KillProcess(string processName)
        {
            LogHelper.WriteMethodLog(true);
            //获得进程对象，以用来操作
            System.Diagnostics.Process myproc = new System.Diagnostics.Process();
            //得到所有打开的进程 
            try
            {
                //获得需要杀死的进程名
                foreach (System.Diagnostics.Process thisproc in System.Diagnostics.Process.GetProcessesByName(processName))
                {
                    LogHelper.WriteInfoLog(thisproc.ProcessName + " be killed");
                    //立即杀死进程 
                    thisproc.Kill();
                    thisproc.Close();
                    thisproc.Dispose();
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog("KillProcess "+ processName, ex);
                throw ex;
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }

        /// <summary>
        /// 判断进程是否存在，0：不存在，其他值返回进程数量。
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool IsProcessRun(string processName)
        {
            System.Diagnostics.Process[] proc = Process.GetProcessesByName(processName);
            return proc.Length>0?true:false;
        }
    }
}


