using Microsoft.VisualStudio.TestTools.UnitTesting;
using VTMC.Utils;
/*************************************************
*Author:Paul Wang
*Date:12/27/2018 9:59:58 AM
*Des:  
************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Tests
{
    [TestClass()]
    public class MessageInfoTests
    {
        [TestMethod()]
        public void ShowTest()
        {
            MessageHelper.Info_001.Show(new object[] { "Paul" });
            MessageHelper.Error_001.Show(new object[] { "Paul" });
            
        }
    }
}