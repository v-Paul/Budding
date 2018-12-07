using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VTMC.Utils.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class TabitemUC : TabItem
    {
        #region  Fields
        /// <summary>
        /// 关闭Tabitem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CloseButtonDelegate(object sender, RoutedEventArgs e);

        /// <summary>
        /// 选项卡TabItem关闭时事件
        /// </summary>
        public event RoutedEventHandler TabItemClosing;

        /// <summary>
        /// Header
        /// </summary>
        private TabitemHeaderUC header = null;


        #endregion

        #region  Constructor
        /// <summary>
        /// 无惨构造
        /// </summary>
        public TabitemUC()
        {
            //设定样式  
            this.Style = (Style)Application.Current.Resources["TabItemStyle"];
            //生产一个可关闭的Header    
            header = createCloseableTabItem();
            //设定Header         
            this.Header = header;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 设定标题
        /// </summary>
        public string HeaderTitle
        {
            get { return this.header.Title; }
            set { this.header.Title = value; }
        }

        #endregion

        #region  Private Mehthods

        private TabitemHeaderUC createCloseableTabItem()
        {
            //实例化一个Header 
            TabitemHeaderUC header = new TabitemHeaderUC();
            //添加关闭按钮点击事件       
            header.btnClose.Click += BtnClose_Click;
            //返回Header  
            return header;
        }

        /// <summary>
        /// 获取父控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T GetParentObject<T>(DependencyObject obj) where T : FrameworkElement
        {
            try
            {
                //返回可视对象的父对象   
                DependencyObject parent = VisualTreeHelper.GetParent(obj);
                //按层、类型提取父级         
                while (parent != null)
                {
                    if (parent is T)
                        return (T)parent;
                    parent = VisualTreeHelper.GetParent(parent);
                }
                //返回         
                return null;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                return null;
            }
        }

        #endregion

        #region  Events

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TabItemClosing != null) { TabItemClosing.Invoke(sender, e); }
                //关闭当前TabItem        
                ((TabControl)GetParentObject<TabControl>(this)).Items.Remove(this);
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelected(RoutedEventArgs e)
        {
            if (this.IsSelected)
            {
                this.header.lblTitle.Foreground = this.header.btnClose.Foreground = Brushes.White;
            }
            else
            {
                this.header.lblTitle.Foreground = this.header.btnClose.Foreground = Brushes.Black;
            }
            base.OnSelected(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUnselected(RoutedEventArgs e)
        {
            if (this.IsSelected)
            {
                this.header.lblTitle.Foreground = this.header.btnClose.Foreground = Brushes.White;
            }
            else
            {
                this.header.lblTitle.Foreground = this.header.btnClose.Foreground = Brushes.Black;
            }
            base.OnUnselected(e);
        }
        #endregion

    }
}
