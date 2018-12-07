using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VTMC.Utils
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BarrierBed : Window
    {
        #region 常量
        #endregion

        #region 变量
        #endregion

        #region 属性

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public BarrierBed()
        {
            InitializeComponent();
        }
        #endregion

        #region 事件
        /// <summary>
        /// 画面初始化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Owner = AppSettings.mainForm;
        }

        #endregion

        #region 方法

        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
