using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VTMC.Utils
{
    /// <summary>
    /// 打印类
    /// </summary>
    public class PrintHelper
    {
        /// <summary>
        ///  文本打印
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static bool Print(string filePath, string printerName)
        {
            try
            {
                PrintDocument docToPrint = new PrintDocument();
                docToPrint.PrinterSettings.PrinterName = printerName;
                string contecxt = System.IO.File.ReadAllText(filePath);
                StringReader lineReader = new StringReader(contecxt);
                docToPrint.PrintPage += delegate (object sender, PrintPageEventArgs e)
                {
                    Graphics g = e.Graphics;
                    //打印字体
                    Font printFont = new Font("Microsoft YaHei", 8F);
                    int count = 0; //行计数器
                    float yPosition = 0;   //绘制字符串的纵向位置
                    string line = null; //行字符串
                    float linesPerPage = 0; //页面的行号
                    float leftMargin = e.MarginBounds.Left; //左边距
                    float topMargin = e.MarginBounds.Top; //上边距
                    SolidBrush myBrush = new SolidBrush(Color.Black);//刷子
                    linesPerPage = e.MarginBounds.Height / printFont.GetHeight(g);//每页可打印的行数
                                                                                  //逐行的循环打印一页
                    while (count < linesPerPage && ((line = lineReader.ReadLine()) != null))
                    {
                        yPosition = topMargin + (count * printFont.GetHeight(g));
                        g.DrawString(line, printFont, myBrush, leftMargin, yPosition, new StringFormat());
                        count++;
                    }
                    // 注意：使用本段代码前，要在该窗体的类中定义lineReader对象：
                    //       StringReader lineReader = null;
                    //如果本页打印完成而line不为空,说明还有没完成的页面,这将触发下一次的打印事件。在下一次的打印中lineReader会
                    //自动读取上次没有打印完的内容，因为lineReader是这个打印方法外的类的成员，它可以记录当前读取的位置
                    if (line != null)
                    {
                        e.HasMorePages = true;
                    }
                    else
                    {
                        e.HasMorePages = false;
                        lineReader = new StringReader(contecxt); // textBox是你要打印的文本框的内容
                    }

                };

                /*
                PrintController printController = new StandardPrintController();
                docToPrint.PrintController = printController;
             
                PrintDialog pd = new PrintDialog();
                pd.PrinterSettings.PrinterName = printerName;
                pd.Document = docToPrint;//把PrintDialog的Document属性设为上面配置好的PrintDocument的实例
               
                DialogResult result = pd.ShowDialog();//调用PrintDialog的ShowDialog函数显示打印对话框
               
                //If the result is OK then print the document.
                if (result == DialogResult.OK)
                {
                    
                }*/

                docToPrint.Print();//开始打印
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorInfoLog(ex.Message, ex);
                return false;
            }
        }

    }
}
