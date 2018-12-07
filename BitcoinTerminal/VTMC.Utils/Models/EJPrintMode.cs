using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTMC.Utils.Models
{

    /// <summary>
    /// EJ统计Model
    /// </summary>
    public class EJPrintMode
    {
        /// <summary>
        /// 正常交易
        /// </summary>
        public List<string> NormalData { get; set; }
        /// <summary>
        /// 异常交易
        /// </summary>
        public List<string> ExceptionData { get; set; }
        /// <summary>
        /// ATMC上账流水
        /// </summary>
        public List<string> ATMCData { get; set; }
        /// <summary>
        /// ATMP上账流水
        /// </summary>
        public List<string> ATMPData { get; set; }
    }

    /// <summary>
    /// EJ全信息
    /// </summary>
    public class EJInfoMode
    {
        /// <summary>
        /// EJ头信息
        /// </summary>
        public EJKioskMode EFKioskInfo { get; set; }
        /// <summary>
        /// EJTran信息
        /// </summary>
        public List<EJTranInfoMode> EJTranInfos { get; set; }
    }

    /// <summary>
    /// 业务单号明细
    /// </summary>
    public class EJTranInfoMode
    {
        /// <summary>
        /// 业务区分 1:NewApp 2:TopUp
        /// </summary>
        public int TranType { get; set; }
        /// <summary>
        /// 办理状态 0:失败 1:成功
        /// </summary>
        public int TranStatus { get; set; }
        /// <summary>
        /// 交易编号
        /// </summary>
        public string TranID { get; set; }
        /// <summary>
        /// 马来货币上帐信息
        /// </summary>
        public EJMoneyMode EJMYRMoneyInfo { get; set; }
        /// <summary>
        /// 现金存入信息
        /// </summary>
        public EJMoneyMode EJCIMMoneyInfo { get; set; }
    }

    /// <summary>
    /// 货币信息
    /// </summary>
    public class EJMoneyMode
    {
        /// <summary>
        /// 货币种类 CNY MYR USD 
        /// </summary>
        public string MoneyType { get; set; }
        /// <summary>
        /// 货币信息
        /// </summary>
        public string MoneyInfo { get; set; }
        /// <summary>
        /// 总额
        /// </summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class EJKioskMode
    {
        /// <summary>
        /// Ej开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        ///  App名称
        /// </summary>
        public string APPName { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// Kiosk No
        /// </summary>
        public string KioskNo { get; set; }

        /// <summary>
        /// Branch No
        /// </summary>
        public string BranchNo { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string LOCIP { get; set; }

    }
}
