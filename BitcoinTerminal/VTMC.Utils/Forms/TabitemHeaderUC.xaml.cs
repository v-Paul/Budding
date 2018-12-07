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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VTMC.Utils.Forms
{
    /// <summary>
    /// TabitemUC.xaml 的交互逻辑
    /// </summary>
    public partial class TabitemHeaderUC : UserControl
    {
        #region Fields

        private string _title;

        #endregion

        #region Constructor

        /// <summary>
        /// 无惨构造
        /// </summary>
        public TabitemHeaderUC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 传入Header
        /// </summary>
        /// <param name="title"></param>
        public TabitemHeaderUC(string title)
        {
            InitializeComponent();
            Title = title;
        }

        #endregion

        #region

        /// <summary>
        /// Header Text
        /// </summary>
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;

                this.lblTitle.Content = _title;
            }
        }

        #endregion

    }
}
