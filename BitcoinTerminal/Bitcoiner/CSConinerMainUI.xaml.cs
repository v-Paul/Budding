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
using System.Windows.Media.Animation;
using System.Windows.Forms;

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
        private NotifyIcon notifyIcon;

        private Transaction mPrimitiveTx;
        MultiSignViewModel vm = new MultiSignViewModel();

        public CSConinerMainUI()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            LogHelper.WriteInfoLog("####################XXPCoin Starting ##############################");
            //MultiSignViewModel vm = new MultiSignViewModel();
            //MockData(vm);
            //this.DataContext = vm;
        }

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //this.commHandler.NotifyOffline();
            //this.Close();
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
                this.commHandler.PriTxCallBack = this.PriTxCallBack;

                this.CurrentBkHash = string.Empty;

                // modify by fdp 加快form显示速度
                Task.Run(() =>
                {
                    this.InitFromDB();
                    if (this.keyHandler.GetDicKeyCount() == 0)
                    {

                        Info001Show("You don't have any Keys,Please Greate a Key first.");


                    }
                    if (this.ReserchNodes() != 0)
                    {
                        this.ReqSyncBlock();
                    }
                });

                InitNotifyIcon();

                txtExpress.Text = @"Use puzzle's four numbers Calculate 24 point, you can use any + - * / ()," + Environment.NewLine + "but each Number must and only be used once.";
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex.Message);
                Task.Run(() =>
                {
                    MessageHelper.Error_001.Show(ex.Message);
                });
            }

        }

        private void Story_Completed(object sender, EventArgs e)
        {
            border1.Visibility = Visibility.Collapsed;
        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            ((MediaElement)sender).Position = ((MediaElement)sender).Position.Add(TimeSpan.FromMilliseconds(1));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            this.commHandler.NotifyOffline();
        }
        #endregion

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
            this.Dispatcher.Invoke(() =>
            {
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

        private string PriTxCallBack(Transaction Tx)
        {
            LogHelper.WriteMethodLog(true);
            string sRet = string.Empty;


            if (this.txHandler.handleTxs(Tx) == ConstHelper.BC_OK)
            {
                sRet = this.bkHandler.AddTx2hsPool(Tx);

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
                    string str = string.Format("Your DB is empty, Sync DB size:{2}MB height:{1} from Ip:{0},  ", df.IP, df.LastBlockHeight, (df.DBFileSize/1024.0/1024.0).ToString("F2"));
                    Info001Show(str);
                    Task.Run(() =>
                    {

                        string SavePath = System.IO.Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                        long lRet = this.commHandler.StartReceiveFile(df.IP, df.DBFileSize, SavePath);
                        if (lRet == -1)
                        {
                            Info001Show("Try later, there is a file transfering now");
                        }
                        else
                        {
                            Info001Show("Received: " + (lRet/1024.0/1024.0).ToString("F2") + "MB");
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


                    string strComitValue = dCommitedValue.ToString("F2");
                    if (!string.Equals(strComitValue, this.txtComitBalance.Text))
                    {
                        this.Test_Double();
                        this.txtComitBalance.Text = strComitValue;
                    }

                    string strUnComitValue = dUnCommitedValue.ToString("F2");
                    if (!string.Equals(strUnComitValue, this.txtUnComitBalance.Text) && dUnCommitedValue != 0)
                    {
                        this.Test_Double();
                    }
                    this.txtUnComitBalance.Text = strUnComitValue;
                }

                // MultiSign
                // todo add condition 
                
                vm = this.keyHandler.MultiSignUTXOPool2VM(true);
                this.DataContext = vm;

            });

            LogHelper.WriteMethodLog(false);
        }
       
        /// <summary>
        /// Set status of CheckBox
        /// </summary>
        /// <param name="isEnable"></param>
        private void SetCheckEnable(bool isEnable)
        {
            foreach (var item in this.vm.MultiSignShows)
            {
                item.IsCheckbBoxEnable = isEnable;
            }
            this.btnCheckAll.IsEnabled = isEnable;
        }
        #endregion

        #region Ui Click functions

        private void btnCreatekey_Click(object sender, RoutedEventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            Task.Run(() =>
            {
                string newPubKeyName = this.keyHandler.GernerateKeypairs();

                Info001Show(string.Format("Generate {0} success", newPubKeyName));

                this.InitKeyValues();
            });
            
            LogHelper.WriteMethodLog(false);
        }

        private void btnCreateBlock_Click(object sender, RoutedEventArgs e)
        {
            //Info001Show(new object[] { "PaulInfoInfoInfoInfoInfoInfoInfoInfoInfoInfo" });
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
                
                Info001Show("Verifying 24-point expression failed, Please re-enter the expression. ");
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
                Info001Show("Other nodes rejected this block.");
                return;
            }
            strRet = this.bkHandler.WriteLastblock(newBlock);
            if (strRet == ConstHelper.BC_OK)
            {
                this.RefreshByNewBlock(newBlock);
            }
            else
            {
                Info001Show(strRet);
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
                Info001Show("Please enter transfer AMOUNT.");
                return;
            }
            double dPaytoAmount = 0;
            if (!Double.TryParse(this.txtAmount.Text, out dPaytoAmount))
            {
                Info001Show("Please enter transfer amount NUMBER");
                return;
            }
            int N = this.cmbNList.SelectedIndex;
            int M = this.cmbMList.SelectedIndex;
            // m=n=0 single sign Tx
            if ((M==0 && N==0) || (M == -1 && N == -1))
            {
                #region single sign Tx
                if (string.IsNullOrEmpty(this.txtAcount.Text) || this.txtAcount.Text.Length != 64)
                {
                    Info001Show("Please enter receiver's right publicKey hash. ");
                    return;
                }


                string strChoice = this.cmbKeyList.SelectedItem.ToString();
                string strRet = this.keyHandler.CheckBalance(strChoice, dPaytoAmount, this.txHandler.GetUtxoPool(true));

                if (strRet != ConstHelper.BC_OK)
                {
                    Info001Show(strRet);
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
                    Info001Show(strRet);
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
                #endregion
            }
            else
            {
                this.CreateMutiSignTrans(N, M, dPaytoAmount, this.txtAcount.Text);
            }

           

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

        #region wfp UI
        private void Test_Double()
        {
            if (border1.Visibility != Visibility.Visible)
                border1.Visibility = Visibility.Visible;
            //Canvas.SetTop(this.border1, -this.border1.ActualHeight / 2);
            //Canvas.SetLeft(this.border1, -this.border1.ActualWidth / 2);

            //this.border1.RenderTransformOrigin = new Point(0.5, 0.5);

            TranslateTransform translate = new TranslateTransform();
            RotateTransform rotate = new RotateTransform();

            TransformGroup group = new TransformGroup();
            //group.Children.Add(rotate);//先旋转
            group.Children.Add(translate);//再平移


            this.border1.RenderTransform = group;

            NameScope.SetNameScope(this, new NameScope());
            this.RegisterName("translate", translate);
            ///this.RegisterName("rotate", rotate);

            DoubleAnimationUsingPath animationX = new DoubleAnimationUsingPath();
            animationX.PathGeometry = this.path1.Data.GetFlattenedPathGeometry();
            animationX.Source = PathAnimationSource.X;
            animationX.Duration = new Duration(TimeSpan.FromSeconds(1));

            DoubleAnimationUsingPath animationY = new DoubleAnimationUsingPath();
            animationY.PathGeometry = this.path1.Data.GetFlattenedPathGeometry();
            animationY.Source = PathAnimationSource.Y;
            animationY.Duration = animationX.Duration;

            DoubleAnimationUsingPath animationAngle = new DoubleAnimationUsingPath();
            animationAngle.PathGeometry = this.path1.Data.GetFlattenedPathGeometry();
            animationAngle.Source = PathAnimationSource.Angle;
            animationAngle.Duration = animationX.Duration;


            Storyboard story = new Storyboard();
            story.Completed += Story_Completed;
            // story.RepeatBehavior = RepeatBehavior.Forever;
            story.AutoReverse = false;
            story.Children.Add(animationX);
            story.Children.Add(animationY);

            //story.Children.Add(animationAngle);
            Storyboard.SetTargetName(animationX, "translate");
            Storyboard.SetTargetName(animationY, "translate");
            // Storyboard.SetTargetName(animationAngle, "rotate");
            Storyboard.SetTargetProperty(animationX, new PropertyPath(TranslateTransform.XProperty));
            Storyboard.SetTargetProperty(animationY, new PropertyPath(TranslateTransform.YProperty));
            //Storyboard.SetTargetProperty(animationAngle, new PropertyPath(RotateTransform.AngleProperty));
            MediaPlayer player = new MediaPlayer();
            player.Open(new Uri(@"Resources/gold.mp3", UriKind.Relative));
            player.Play();
            System.Threading.Thread.Sleep(500);
            story.Begin(this);

        }

        private void InitNotifyIcon()
        {
            this.notifyIcon = new NotifyIcon();
            this.notifyIcon.BalloonTipText = "西夏普币";
            this.notifyIcon.ShowBalloonTip(2000);
            this.notifyIcon.Icon = new System.Drawing.Icon(@"Resources\CSharpCoin.ico");
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
            {
                if (e.Button == MouseButtons.Left) this.Show(o, e);
            });


            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.BackColor = System.Drawing.Color.Black;
            contextMenuStrip.ShowImageMargin = false;
            //contextMenuStrip.DropShadowEnabled = false;
            //contextMenuStrip.BackColor = System.Drawing.Color.Black;

            //打开菜单项
            ToolStripMenuItem tsOpen = new ToolStripMenuItem();
            tsOpen.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsOpen.BackColor = System.Drawing.Color.Black;
            tsOpen.Font = new System.Drawing.Font("Microsoft YaHei", 10);
            tsOpen.ForeColor = System.Drawing.Color.FromArgb(250, 185, 21);
            tsOpen.Text = "Open";
            tsOpen.Click += new EventHandler(Show);
            //tsOpen.MouseHover += Ts_MouseHover;
            //tsOpen.MouseLeave += Ts_MouseLeave;

            ToolStripSeparator ts = new ToolStripSeparator();
            ts.BackColor = System.Drawing.Color.FromArgb(250, 185, 21);

            //退出菜单项
            ToolStripMenuItem tsClose = new ToolStripMenuItem();
            tsClose.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsClose.BackColor = System.Drawing.Color.Black;
            tsClose.Font = new System.Drawing.Font("Microsoft YaHei", 10);
            tsClose.ForeColor = System.Drawing.Color.FromArgb(250, 185, 21);
            tsClose.Text = "Exit";
            tsClose.Click += new EventHandler(Close);
            //tsClose.MouseHover += Ts_MouseHover;
            //tsClose.MouseLeave += Ts_MouseLeave;

            contextMenuStrip.Items.Add(tsOpen);
            contextMenuStrip.Items.Add(ts);
            contextMenuStrip.Items.Add(tsClose);


            this.notifyIcon.ContextMenuStrip = contextMenuStrip;
        }

        private void Ts_MouseLeave(object sender, EventArgs e)
        {
            ToolStripMenuItem tm = (ToolStripMenuItem)sender;
            tm.BackColor = System.Drawing.Color.Black;
            tm.ForeColor = System.Drawing.Color.FromArgb(250, 185, 21);
        }

        private void Ts_MouseHover(object sender, EventArgs e)
        {
            ToolStripMenuItem tm = (ToolStripMenuItem)sender;
            tm.BackColor = System.Drawing.Color.WhiteSmoke;
            tm.ForeColor = System.Drawing.Color.Black;
        }

        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }


        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Info001Show(string msg)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessageHelper.Info_001.Show(msg);

            });
        }

        private void MockData(MultiSignViewModel vm)
        {
            MultiSignShowModel ms = new MultiSignShowModel();
            ms.BIsAdd2PriTx = true;
            ms.ID = "1";
            ms.TxHash = "1234567812345678123456781234567812345678123456781234567812345678";
            ms.OutputIndex = 0;
            ms.Value = 121.1;
            ms.OutScriptPKHash = "1234567812345678"+ Environment.NewLine +"1234567812345678" + Environment.NewLine + "1234567812345678";
            vm.MultiSignShows.Add(ms);
            vm.MultiSignShows.Add(ms);
        }

        private void btnCheckAll_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
                bool ret = (cb.IsChecked == null ? false : (bool)cb.IsChecked);
                if (this.vm.MultiSignShows?.Count != 0)
                {
                    foreach (MultiSignShowModel item in this.vm.MultiSignShows)
                    {
                        item.BIsAdd2PriTx = ret;
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;

                MultiSignShowModel ms = (MultiSignShowModel)cb.DataContext;
                if (ms != null)
                {
                    ms.BIsAdd2PriTx = (cb.IsChecked == null ? false : (bool)cb.IsChecked);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region MultiSign


        public void CreateMutiSignTrans(int N, int M, double dPaytoAmount, string MutiPkHash)
        {
            LogHelper.WriteMethodLog(true);

            //int N = 2;
            //int M = 3;
            //string MutiPkHash = "69C79662330838155EF3474300908FAC9F14D912AF52F5863E5143B551F5D869 01314092DF4A3BDBA944E458CED99ED34A88ED4F0B9F179A5AEFC62017BB990D 4F54AC9D55552C633360FEA08A4322EED29138E8025D582F9F6F58E5905FD63B";

            List<string> lstPkHashs = MutiPkHash.Split(' ').ToList<string>();

            string strMutiScript = string.Empty;
            string strRet = this.txHandler.AssumbleMutiSignScript(N, M, lstPkHashs, ref strMutiScript);
            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                return;
            }

            string strChoice = this.cmbKeyList.SelectedItem.ToString();
            strRet = this.keyHandler.CheckBalance(strChoice, dPaytoAmount, this.txHandler.GetUtxoPool(true));

            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                return;
            }

            //string strPaytoHash = this.keyHandler.PubKeyHash2Script(this.txtAcount.Text);
            string strChangePuKScript = this.keyHandler.PubKeyHash2Script(this.txtKeyHash.Text);


            double dInputTatolAmount = 0;
            Dictionary<UTXO, keyPair> dicInputUtxo = this.keyHandler.FindInputUtxo(strChoice, dPaytoAmount,
                                                        this.txHandler.GetUtxoPool(true), ref dInputTatolAmount);


            Transaction Tx = this.txHandler.CreatTransaction(dicInputUtxo, dInputTatolAmount, dPaytoAmount,
                                                                 strMutiScript, strChangePuKScript);
            LogHelper.WriteInfoLog(JsonHelper.Serializer<Transaction>(Tx));
            strRet = this.txHandler.handleTxs(Tx);

            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
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

        private void btnCreatePriTx_Click(object sender, RoutedEventArgs e)
        {
            //SetCheckEnable(false);
            //return;
            #region CreatePrimitiveTX
            LogHelper.WriteMethodLog(false);

            if (string.IsNullOrEmpty(this.txtMultiSignAmount.Text))
            {
                Info001Show("Please enter MultiSign AMOUNT.");
                return;
            }
            double dPaytoAmount = 0;
            if (!Double.TryParse(this.txtMultiSignAmount.Text, out dPaytoAmount))
            {
                Info001Show("Please enter MultiSign amount NUMBER");
                return;
            }

            if (string.IsNullOrEmpty(this.txtMultiSignPay2Hash.Text) || this.txtMultiSignPay2Hash.Text.Length != 64)
            {
                Info001Show("Please enter receiver's right publicKey hash. ");
                return;
            }


            List<Input> lstInput = new List<Input>();
            foreach (var item in this.vm.GetMultiSignShows())
            {
                if (item.BIsAdd2PriTx)
                {
                    Input input = new Input(item.TxHash, item.OutputIndex);
                    lstInput.Add(input);
                }

            }
            if (lstInput.Count == 0)
            {
                Info001Show("No checked input");
                return;
            }
            string strp2Script = string.Format("OP_DUP OP_HASH160 {0} OP_EQUALVERIFY OP_CHECKSIG", this.txtAcount);





            //// test data
            //List<Input> lstInput = new List<Input>();
            //Input input = new Input("C832E985B1D16ABFD7949589D4CFED1F4A19623B440ED1FD61AFD584C217463D", 0);
            //lstInput.Add(input);
            //string strp2Script = "OP_DUP OP_HASH160 69C79662330838155EF3474300908FAC9F14D912AF52F5863E5143B551F5D869 OP_EQUALVERIFY OP_CHECKSIG";
            //// end test


            Transaction PriTx = new Transaction();
            string strRet = this.txHandler.CreatPrimitiveTx(lstInput, 10, strp2Script, ref PriTx);
            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                LogHelper.WriteErrorLog("CreatPrimitiveTx fail");
            }
            else
            {
                this.mPrimitiveTx = PriTx;
            }
            #endregion
        }

        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            #region MultiSign
            Dictionary<string, keyPair> dickHKeyPair = new Dictionary<string, keyPair>();
            this.keyHandler.GetdicPkHKeypair(ref dickHKeyPair);

            string strRet = this.txHandler.SignPrimitiveTx(dickHKeyPair, ref this.mPrimitiveTx);
            if(strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                return;
            }
            LogHelper.WriteInfoLog("MultiSign TX: " + JsonHelper.Serializer<Transaction>(this.mPrimitiveTx));
            LogHelper.WriteMethodLog(false);
            #endregion
        }

        private void btnRequestSign_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtIpAddress.Text))
            {
                Info001Show("Please enter IP address, if there are multiple IP addesses , please separate by space.");
                return;
            }
            List<string> lstIP = this.txtIpAddress.Text.Split(' ').ToList<string>();
            foreach (var item in lstIP)
            {
                var seperateIPNum = (from x in item.Split('.')
                                     where x != ""
                                     select x).ToList();

                if (seperateIPNum.Count != 4)
                {
                    Info001Show(string.Format("{0} Please enter a valid IP address", item) );
                    return;
                }
                foreach (var item1 in seperateIPNum)
                {
                    int a = -1;
                    bool bsucc = int.TryParse(item1, out a);
                    if(a<0 || a>255 || !bsucc)
                    {
                        Info001Show(string.Format("{0} Please enter a valid IP address", item));
                        return;
                    }
                }
            }




        }

        private void btnCreateRedeemTx_Click(object sender, RoutedEventArgs e)
        {
            #region CreateRedeemTx

            if(this.mPrimitiveTx == null)
            {
                Info001Show("You haven't create redeem primitive Tx! ");
                return;
            }
            Transaction PrimitiveTx = new Transaction();
            PrimitiveTx = this.mPrimitiveTx;

            string strRet = this.txHandler.CreateRedeemTx(ref PrimitiveTx);
            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                return;
            }
            strRet = this.txHandler.handleTxs(PrimitiveTx);

            if (strRet != ConstHelper.BC_OK)
            {
                Info001Show(strRet);
                return;
            }

            this.bkHandler.AddTx2hsPool(PrimitiveTx);

            this.RefreshInterfaceTxCount();

            this.keyHandler.RefKUtxoList(true, this.txHandler.GetUtxoPool(true));
            this.keyHandler.RefKUtxoList(false, this.txHandler.GetUtxoPool(false));
            this.RefreshKeyValueBox();
            this.ResetInterfacePayItem();

            Task.Run(() =>
            {
                this.commHandler.SendNewTx2AddressLst(PrimitiveTx);

            });
            // Create redeem tx success dispose mPrimitiveTx
            this.mPrimitiveTx = null;
            LogHelper.WriteMethodLog(false);

            #endregion
        }
        #endregion

      
    }

}
