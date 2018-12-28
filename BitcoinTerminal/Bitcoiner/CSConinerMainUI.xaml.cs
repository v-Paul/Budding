using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.IO;
using BaseSturct;
using VTMC.Utils;
namespace Bitcoiner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class CSConinerMainUI : Window
    {

        private BkHandler bkHandler;
        private TxHandler txHandler;
        private KeyHandler keyHandler;
        private Communication commHandler;
        private string CurrentBkHash;

        public CSConinerMainUI()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            //LogHelper.WriteInfoLog(string.Format("当前产品名：{0}，当前产品版本：{1}", new object[] { System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion }));

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.commHandler.NotifyOffline();
            this.Close();
        }

        private void spTtile_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.InitAppseting();
                this.bkHandler = new BkHandler();
                this.txHandler = new TxHandler();
                this.keyHandler = new KeyHandler();
                this.commHandler = new Communication();
                this.commHandler.NewTransactionCallBack = this.NewTransactionCallBack;
                this.commHandler.NewBlockCallBack = this.NewBlockCallBack;
                this.commHandler.RefresfNodeCountCallBack = this.RefresfNodeCountCallBack;
                this.commHandler.PushTxhsPoolCallBack = this.PushTxhsPoolCallBack;
                this.commHandler.PushLastBlockCallBack = this.PushLastBlockCallBack;

                this.CurrentBkHash = string.Empty;

                // modify by fdp 加快form显示速度
                Task.Run(() =>
                {
                    this.InitFromDB();
                    if (this.ReserchNodes() != 0)
                    {
                        this.ReqSyncBlock();
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                Task.Run(()=> {
                    MessageHelper.Error_001.Show(ex.Message);
                });
            }
            
        }

        #region Initiate function
        private void InitAppseting()
        {
            LogHelper.WriteMethodLog(true);
            #region init peizhi
            //应用程序版本号
            AppSettings.ProductVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            LogHelper.WriteInfoLog(string.Format("当前产品版本：{0}", AppSettings.ProductVersion));

            AppSettings.XXPCommonFolder = string.Format(AppConfigHelper.GetConfigValByKey("XXPCommonFolder"), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            AppSettings.XXPCommport = Convert.ToInt32(AppConfigHelper.GetConfigValByKey("XXPCommPort"));
            AppSettings.SeedNodes = AppConfigHelper.GetConfigValByKey("SeedNodes");


            AppSettings.XXPTransFilePort = Convert.ToInt32(AppConfigHelper.GetConfigValByKey("XXPTransFilePort"));
            if (!System.IO.Directory.Exists(AppSettings.XXPCommonFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPCommonFolder);
            }

            AppSettings.XXPDBFolder = System.IO.Path.Combine(AppSettings.XXPCommonFolder, "leveldb");
            if (!System.IO.Directory.Exists(AppSettings.XXPDBFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPDBFolder);
            }

            AppSettings.XXPKeysFolder = Path.Combine(AppSettings.XXPCommonFolder, "keys");
            if (!System.IO.Directory.Exists(AppSettings.XXPKeysFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPKeysFolder);
            }

            AppSettings.XXPLogFolder = Path.Combine(AppSettings.XXPCommonFolder, "log");
            if (!System.IO.Directory.Exists(AppSettings.XXPLogFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPLogFolder);
            }

            AppSettings.XXPTempFolder = Path.Combine(AppSettings.XXPCommonFolder, "temp");
            if (!System.IO.Directory.Exists(AppSettings.XXPTempFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPTempFolder);
            }
            #endregion
            LogHelper.WriteMethodLog(false);
        }

        private void InitFromDB()
        {
            LogHelper.WriteMethodLog(true);

            this.bkHandler.GetLastBlock();
            //this.textBox1.Text = this.bkHandler.strPuzzle;
            this.txHandler.CreatUTPoolFromDB(AppSettings.XXPDBFolder);
            this.keyHandler.RefKUtxoList(true, this.txHandler.GetUtxoPool(true));
            this.keyHandler.RefKUtxoList(false, this.txHandler.GetUtxoPool(false));


            this.RefreshInterfaceItem();
            LogHelper.WriteMethodLog(false);

        }

        private void InitKeyValues()
        {
            LogHelper.WriteMethodLog(true);
            this.Dispatcher.Invoke(()=> {
                this.cmbKeyList.Items.Clear();
                this.cmbKeyList.Items.Add("All");

                Dictionary<string, string> dickeyValue = this.keyHandler.GetDicKeyHash();
                this.txtKeyCount.Text = dickeyValue.Count().ToString();
                foreach (var dicItem in dickeyValue)
                {
                    this.cmbKeyList.Items.Add(dicItem.Key);

                }
                this.cmbKeyList.SelectedIndex = 0;
            });
           
            LogHelper.WriteMethodLog(false);
        }
        #endregion

        #region Callbacks 


        private string NewTransactionCallBack(Transaction Ts)
        {
            LogHelper.WriteMethodLog(true);
            string sRet = string.Empty;
            if (this.txHandler.handleTxs(Ts) == ConstHelper.BC_OK)
            {
                sRet = this.bkHandler.AddTx2hsPool(Ts);

                this.keyHandler.RefKUtxoList(true, this.txHandler.GetUtxoPool(true));
                this.keyHandler.RefKUtxoList(false, this.txHandler.GetUtxoPool(false));
                this.RefreshKeyValueBox();

                this.RefreshInterfaceTxCount();

            }
            else
            {
                sRet = Decision.Reject;
            }
            LogHelper.WriteInfoLog("NewTransactionCallBack ret: " + sRet);
            return sRet;


        }

        private string NewBlockCallBack(Block block)
        {
            LogHelper.WriteMethodLog(true);
            string sRet = string.Empty;
            if (this.CurrentBkHash == block.Hash ||
                block.Hash == this.bkHandler.GetLastBKHash())
            {
                LogHelper.WriteInfoLog("Accepted");
                return Decision.Accepted;
            }
            int iRet = this.bkHandler.IsValidBlock(block);
            if (iRet == 0)
            {
                if (this.bkHandler.WriteLastblock(block) == ConstHelper.BC_OK)
                {
                    Task.Run(() =>
                    {

                        RefreshByNewBlock(block);
                    });
                }
                LogHelper.WriteInfoLog("Accept");
                return Decision.Accept;
            }
            else
            {
                if (iRet == 1)
                {
                    Task.Run(() =>
                    {
                        this.ReqSyncBlock(false);
                    });
                }
                LogHelper.WriteInfoLog("Reject");
                return Decision.Reject;
            }
            LogHelper.WriteMethodLog(false);
        }

        void RefresfNodeCountCallBack(int iNodesCount)
        {
            LogHelper.WriteMethodLog(true);
            //this.textBoxConnectedNodes.Text = iNodesCount.ToString();

            this.Dispatcher.Invoke(() =>
            {
                this.txtConectNodes.Text = iNodesCount.ToString();

            });
            LogHelper.WriteMethodLog(false);
        }


        void PushLastBlockCallBack(string ip)
        {
            LogHelper.WriteMethodLog(true);
            Block lstBlock = new Block();
            lstBlock = this.bkHandler.GetLastBlock();

            this.commHandler.SendNewBlock(ip, lstBlock);
            LogHelper.WriteMethodLog(false);

        }
        void PushTxhsPoolCallBack(string ip)
        {
            LogHelper.WriteMethodLog(true);
            List<Transaction> lstTx = new List<Transaction>();
            lstTx = this.bkHandler.GetlstPoolTx();
            foreach (var item in lstTx)
            {
                this.commHandler.SendNewtransactions(ip, item);
            }
            LogHelper.WriteMethodLog(false);
        }
        #endregion

        #region Sync fucntions

        private int ReserchNodes()
        {
            LogHelper.WriteMethodLog(true);
            this.commHandler.ReserchNodes();
            return this.commHandler.GetAddressCount();
            LogHelper.WriteMethodLog(false);
        }
        private void ReqSyncBlock(bool bCheckemptyDB = true)
        {
            LogHelper.WriteMethodLog(true);
            #region empty DB，Sync all db
            if (bCheckemptyDB)
            {
                if (LeveldbOperator.OpenDB(AppSettings.XXPDBFolder) != ConstHelper.BC_OK)
                {
                    DBFileInfo df = this.commHandler.RequestHightestDBInfo();
                    string str = string.Format("Your DB is empty, Sync DB size:{2}MB height:{1} from Ip:{0},  ", df.IP, df.LastBlockHeight, df.DBFileSize/1024.0/1024.0);
                    MessageHelper.Info_001.Show(str);
                    Task.Run(() =>
                    {

                        string SavePath = System.IO.Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                        long lRet = this.commHandler.StartReceiveFile(df.IP, df.DBFileSize, SavePath);
                        if (lRet == -1)
                        {
                            MessageHelper.Info_001.Show("Try later, there is a file transfering now");
                        }
                        else
                        {
                            MessageHelper.Info_001.Show("Received: " + (lRet/1024.0/1024.0).ToString() + "MB");
                            FileIOHelper.DeleteDir(AppSettings.XXPDBFolder);
                            Directory.CreateDirectory(AppSettings.XXPDBFolder);
                            ZipHelper.UnZip(SavePath, AppSettings.XXPDBFolder);
                            this.InitFromDB();
                        }
                        this.commHandler.DisposeTransFileHelper();

                    });

                    if (this.commHandler.RequestStartTransDB(df.IP) != ConstHelper.BC_OK)
                    {

                    }
                }
                LeveldbOperator.CloseDB();

            }
            #endregion

            ResponseBlock BkInfo = this.commHandler.RequestNewBlockInfo(this.bkHandler.GetLastBlock());
            if (BkInfo.BlockResult == BlockResultType.Lower)
            {
                string sRet = this.commHandler.GetNewBlocks(BkInfo.IP, this.bkHandler.GetLastBlock());
            }
            LogHelper.WriteMethodLog(false);
        }


        private void RemoveComitedTx(List<Transaction> lstTX)
        {
            LogHelper.WriteMethodLog(true);
            List<Transaction> lstTem = new List<Transaction>();

            foreach (var item in lstTX)
            {
                if (this.bkHandler.hsTxPoolContains(item))
                {
                    lstTem.Add(item);
                }
            }
            foreach (var item in lstTem)
            {
                this.bkHandler.hsTxPoolRemove(item);
            }
            LogHelper.WriteMethodLog(false);
        }


        #endregion

        #region Refresh User interface


        /// <summary>
        /// 
        /// </summary>
        /// <param name="block"></param>
        private void RefreshByNewBlock(Block block)
        {
            LogHelper.WriteMethodLog(true);

            this.RemoveComitedTx(block.listTransactions);

            this.bkHandler.RefreshLastBlock(block);
            this.txHandler.RefreshUTPoolByBlock(block);
            this.keyHandler.RefKUtxoList(true, this.txHandler.GetUtxoPool(true));

            this.txHandler.ClearUnCommitUtxoPool();
            this.keyHandler.RefKUtxoList(false, this.txHandler.GetUtxoPool(false));

            this.RefreshKeyValueBox();
            this.RefreshInterfaceItem();

            LogHelper.WriteMethodLog(false);
        }

        private void RefreshInterfaceItem()
        {
            LogHelper.WriteMethodLog(true);
            this.Dispatcher.Invoke(() =>
            {
                this.txtLastPuzzle.Text = this.bkHandler.GetLastPuzzleStr();
                this.txtLastBlockheight.Text = this.bkHandler.GetLastBkHeight().ToString();
                this.txtPuzzleExpress.Text = "";//express
                this.txtTxUncomitCount.Text = this.bkHandler.GetHsTxPoolCount().ToString();
                this.InitKeyValues();
            });

            LogHelper.WriteMethodLog(false);
        }

        private void RefreshInterfaceTxCount()
        {
            LogHelper.WriteMethodLog(true);
            this.Dispatcher.Invoke(() =>
            {
                this.txtTxUncomitCount.Text = this.bkHandler.GetHsTxPoolCount().ToString();
            });
            LogHelper.WriteMethodLog(false);
        }

        private void ResetInterfacePayItem()
        {
            LogHelper.WriteMethodLog(true);
            this.Dispatcher.Invoke(() =>
            {
                this.txtAmount.Text = "";
                this.txtAcount.Text = "";
            });
            LogHelper.WriteMethodLog(false);
        }

        private void RefreshKeyValueBox()
        {
            LogHelper.WriteMethodLog(true);

            this.Dispatcher.Invoke(() =>
            {
                if (this.cmbKeyList.SelectedItem != null)
                {
                string strChoice = this.cmbKeyList.SelectedItem.ToString();

                double dCommitedValue = this.keyHandler.GetValue(true, strChoice, this.txHandler.GetUtxoPool(true));
                double dUnCommitedValue = this.keyHandler.GetValue(false, strChoice, this.txHandler.GetUtxoPool(false));

                this.txtKeyHash.Text = this.keyHandler.GetKeyHash(strChoice);
                this.txtComitBalance.Text = dCommitedValue.ToString("F2");
                this.txtUnComitBalance.Text = dUnCommitedValue.ToString("F2");
                }
                
            });

            LogHelper.WriteMethodLog(false);
        }
        #endregion

        #region Ui Click functions

        private void btnCreatekey_Click(object sender, RoutedEventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            Task.Run(()=> {
                string newPubKeyName = this.keyHandler.GernerateKeypairs();
                this.Dispatcher.Invoke(()=> {
                    MessageHelper.Info_001.Show(string.Format("Generate {0} success", newPubKeyName));
                });
                this.InitKeyValues();
            });
            
            LogHelper.WriteMethodLog(false);
        }

        private void btnCreateBlock_Click(object sender, RoutedEventArgs e)
        {
            //MessageHelper.Info_001.Show(new object[] { "PaulInfoInfoInfoInfoInfoInfoInfoInfoInfoInfo" });
            //MessageHelper.Warn_001.Show(new object[] { "PaulWarnWarnWarnWarnWarnWarnWarnWarnWarnWarn" });
            //MessageHelper.Error_001.Show(new object[] { "PaulErrorErrorErrorErrorErrorErrorErrorError" });
            //MessageHelper.Question_001.Show(new object[] { "PaulQuestionQuestionQuestionQuestionQuestion" });
            //return;

            LogHelper.WriteMethodLog(true);
            if (this.commHandler.GetAddressCount() == 0)
            {
                MessageHelper.Error_001.Show("Offline, No nodes connected.");
                return;
            }
            string sBaseCoinScript = this.keyHandler.PubKeyHash2Script(this.txtKeyHash.Text);

            if (!Cryptor.Verify24Puzzel(this.bkHandler.GetlastBkPuzzleArr(), this.txtPuzzleExpress.Text))
            {
                
                MessageHelper.Info_001.Show("Verifying 24-point expression failed, Please re-enter the expression. ");
                return;
            }

            Block newBlock = this.bkHandler.CreatBlock(this.txtPuzzleExpress.Text, sBaseCoinScript);
            if (newBlock == null)
            {
                MessageHelper.Error_001.Show("Create Block failed.");
                return;
            }
            this.CurrentBkHash = newBlock.Hash;

            string strRet = this.commHandler.SendNewBlock2AddressLst(newBlock);
            if (strRet == Decision.Reject)
            {
                MessageHelper.Info_001.Show("Other nodes rejected this block.");
                return;
            }
            strRet = this.bkHandler.WriteLastblock(newBlock);
            if (strRet == ConstHelper.BC_OK)
            {
                this.RefreshByNewBlock(newBlock);
            }
            else
            {
                MessageHelper.Info_001.Show(strRet);
            }
            LogHelper.WriteMethodLog(false);
        }

        private void btnTransfer_Click(object sender, RoutedEventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            if (this.commHandler.GetAddressCount() == 0)
            {
                MessageHelper.Error_001.Show("Offline, No nodes connected.");
                return;
            }
            if (string.IsNullOrEmpty(this.txtAmount.Text))
            {
                MessageHelper.Info_001.Show("Please enter transfer AMOUNT.");
                return;
            }
            if (string.IsNullOrEmpty(this.txtAcount.Text) || this.txtAcount.Text.Length != 64)
            {
                MessageHelper.Info_001.Show("Please enter receiver's right publicKey hash. ");
                return;
            }
            double dPaytoAmount = 0;       
            if (!Double.TryParse(this.txtAmount.Text, out dPaytoAmount))
            {
                MessageHelper.Info_001.Show("Please enter transfer amount NUMBER");
                return;
            }

            string strChoice = this.cmbKeyList.SelectedItem.ToString();
            string strRet = this.keyHandler.CheckBalance(strChoice, dPaytoAmount, this.txHandler.GetUtxoPool(true));

            if (strRet != ConstHelper.BC_OK)
            {
                MessageHelper.Info_001.Show(strRet);
                return;
            }

            string strPaytoHash = this.keyHandler.PubKeyHash2Script(this.txtAcount.Text);
            string strChangePuKScript = this.keyHandler.PubKeyHash2Script(this.txtKeyHash.Text);


            double dInputTatolAmount = 0;
            Dictionary<UTXO, keyPair> dicInputUtxo = this.keyHandler.FindInputUtxo(strChoice, dPaytoAmount,
                                                        this.txHandler.GetUtxoPool(true), ref dInputTatolAmount);


            Transaction Tx = this.txHandler.CreatTransaction(dicInputUtxo, dInputTatolAmount, dPaytoAmount,
                                                                 strPaytoHash, strChangePuKScript);
            LogHelper.WriteInfoLog(JsonHelper.Serializer<Transaction>(Tx));
            strRet = this.txHandler.handleTxs(Tx);

            if (strRet != ConstHelper.BC_OK)
            {
                MessageHelper.Info_001.Show(strRet);
                return;
            }
            this.bkHandler.AddTx2hsPool(Tx);

            this.RefreshInterfaceTxCount();

            this.keyHandler.RefKUtxoList(true, this.txHandler.GetUtxoPool(true));
            this.keyHandler.RefKUtxoList(false, this.txHandler.GetUtxoPool(false));
            this.RefreshKeyValueBox();
            this.ResetInterfacePayItem();

            Task.Run(() =>
            {
                this.commHandler.SendNewTx2AddressLst(Tx);

            });

            LogHelper.WriteMethodLog(false);
        }

        //选择key
        private void cmbKeyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.cmbKeyList.SelectedValue
            LogHelper.WriteMethodLog(true);
            this.RefreshKeyValueBox();
            LogHelper.WriteMethodLog(false);

        }


        #endregion

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }

   
    }
