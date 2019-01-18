using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VTMC.Utils;
namespace Bitcoiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            LogHelper.WriteMethodLog(true);

            try
            {
                if (ProcessHelper.IsProcessRun(ConstHelper.BC_ProcessName))
                {
                    MessageHelper.Info_001.Show("Bitcoiner is Running, Please call from system tray");

                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {

                Environment.Exit(0);
            }
            finally
            {
                LogHelper.WriteMethodLog(false);
            }
        }
    }
}
