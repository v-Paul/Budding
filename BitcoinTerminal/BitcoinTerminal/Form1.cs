using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using BaseSturct;
using VTMC.Utils;


namespace BitcoinTerminal
{
    public partial class Form1 : Form
    {


        private BkHandler bkHandler;
        private TxHandler txHandler;
        private KeyHandler keyHandler;
        private Communication commHandler;
        private string CurrentBkHash;
        //private string MyAddressScript;

        public Form1()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            LogHelper.WriteInfoLog(string.Format("当前产品名：{0}，当前产品版本：{1}", new object[] { System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion }));

            InitializeComponent();
            


            //this.InitFromDB(false);

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.InitAppseting();


            this.bkHandler = new BkHandler();
            this.txHandler = new TxHandler();
            this.keyHandler = new KeyHandler();
            this.commHandler = new Communication();
            this.commHandler.NewTransactionCallBack = this.NewTransactionCallBack;
            this.commHandler.NewBlockCallBack = this.NewBlockCallBack;
            this.commHandler.RefresfNodeCountCallBack = this.RefresfNodeCountCallBack;

            this.CurrentBkHash = string.Empty;

            // modify by fdp 加快form显示速度
            Task.Run(() =>
            {
                this.InitFromDB();
                if(this.ReserchNodes() != 0)
                {
                    this.ReqSyncBlock();
                }


            });
            //this.InitFromDB();
            ////this.textBoxConnectedNodes.Text = this.commHandler.GetAddressCount().ToString();
            //this.commHandler.ReserchNodes();

        }

        private void InitAppseting()
        {
            LogHelper.WriteMethodLog(true);
            #region init peizhi
            //应用程序版本号
            AppSettings.ProductVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AppSettings.XXPCommonFolder = string.Format(AppConfigHelper.GetConfigValByKey("XXPCommonFolder"), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            AppSettings.XXPCommport = Convert.ToInt32(AppConfigHelper.GetConfigValByKey("XXPCommPort"));
            AppSettings.SeedNodes = AppConfigHelper.GetConfigValByKey("SeedNodes");


            AppSettings.XXPTransFilePort = Convert.ToInt32(AppConfigHelper.GetConfigValByKey("XXPTransFilePort"));
            if (!System.IO.Directory.Exists(AppSettings.XXPCommonFolder))
            {
                Directory.CreateDirectory(AppSettings.XXPCommonFolder);
            }

            AppSettings.XXPDBFolder = Path.Combine(AppSettings.XXPCommonFolder, "leveldb");
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
            //this.BeginInvoke(new MethodInvoker(() =>
            //{
            this.bkHandler.GetLastBlock();
                //this.textBox1.Text = this.bkHandler.strPuzzle;
                this.txHandler.CreatUTPoolFromDB(AppSettings.XXPDBFolder);
                this.keyHandler.RefKVFromUtxopool(this.txHandler.GetUtxoPool());
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.textBox1.Text = this.bkHandler.strPuzzle;
                this.InitKeyValues();

            }));
            LogHelper.WriteMethodLog(false);
            
        }

        private int ReserchNodes()
        {
            LogHelper.WriteMethodLog(true);
            this.commHandler.ReserchNodes();
            return this.commHandler.GetAddressCount();
            LogHelper.WriteMethodLog(false);
        }


        /// <summary>
        /// Creat Block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Digcoin_Click(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            string sBaseCoinScript = this.keyHandler.PubKeyHash2Script(this.textBoxKeyHash.Text);
            
            if (!Cryptor.Verify24Puzzel(this.bkHandler.GetlastblockPuzzle(), this.textBox2.Text))
            {
                MessageBox.Show("Verify24Puzzel fail");
                return;
            }

            Block newBlock = this.bkHandler.CreatBlock(this.textBox2.Text, sBaseCoinScript);
            if (newBlock == null)
            {
                MessageBox.Show("CreatBlock fail");
                return;
            }
            this.CurrentBkHash = newBlock.Hash;

            string strRet = this.commHandler.SendNewBlock2AddressLst(newBlock);
            if (strRet == Decision.Reject)
            {
                MessageBox.Show("Other nodes rejected this block");
                return;
            }
            strRet = this.bkHandler.WriteLastblock(newBlock);
            if (strRet == ConstHelper.BC_OK)
            {
                this.bkHandler.RefreshLastBlock(newBlock);

                this.txHandler.RefreshUTPoolByBlock(newBlock);

                //this.txHandler.AddBaseCoin2UTxoPool(newBlock.GetBaseCoinTx());
                this.keyHandler.RefKVFromUtxopool(this.txHandler.GetUtxoPool());
                this.RefreshKeyValueBox();



                this.textBox1.Text = this.bkHandler.strPuzzle;
                this.textBox2.Text = "";
            }
            else
            {
                MessageBox.Show(strRet);
            }
            LogHelper.WriteMethodLog(false);
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = "";
        }

       

        private void buttonSpent_Click(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            if (this.commHandler.GetAddressCount() == 0)
            {
                MessageBox.Show("Offline, Please search seeds first");
                return;
            }
            if (string.IsNullOrEmpty(this.TextBoxAmount.Text))
            {
                MessageBox.Show("Please enter the transfer amount");
                return;
            }
            if (string.IsNullOrEmpty(this.textBoxPaytoHash.Text))
            {
                MessageBox.Show("Please enter receiver publicKey hash ");
                return;
            }

            string strChoice = this.comboBox1.SelectedItem.ToString();
            string strPaytoHash = this.keyHandler.PubKeyHash2Script( this.textBoxPaytoHash.Text);
            double dPaytoAmount = Convert.ToDouble(this.TextBoxAmount.Text);
            string strChangePuKScript = this.keyHandler.PubKeyHash2Script(this.textBoxKeyHash.Text);

            string strRet = this.keyHandler.CheckBalance(strChoice, dPaytoAmount);
            if(strRet != ConstHelper.BC_OK)
            {
                MessageBox.Show(strRet);
                return;
            }
            double dInputTatolAmount = 0;
            Dictionary<UTXO, keyPair> dicInputUtxo = this.keyHandler.FindInputUtxo( strChoice, dPaytoAmount, 
                                                        this.txHandler.GetUtxoPool(), ref dInputTatolAmount );


            Transaction Tx = this.txHandler.CreatTransaction( dicInputUtxo, dInputTatolAmount, dPaytoAmount, 
                                                                 strPaytoHash, strChangePuKScript );
            LogHelper.WriteInfoLog(JsonHelper.Serializer<Transaction>(Tx));
            strRet = this.txHandler.handleTxs(Tx);

            if(strRet != ConstHelper.BC_OK)
            {
                MessageBox.Show(strRet);
                return;
            }
            this.bkHandler.AddTransaction(Tx);
            this.keyHandler.RefKVFromUtxopool(this.txHandler.GetUtxoPool());
            this.RefreshKeyValueBox();
            this.TextBoxAmount.Text = "";
            this.textBoxPaytoHash.Text = "";

            Task.Run(()=> {
                this.commHandler.SendNewTx2AddressLst(Tx);

            });

            LogHelper.WriteMethodLog(false);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void GenerateKeypair_Click(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            string nePubKeyName = this.keyHandler.GernerateKeypairs();
            this.comboBox1.Items.Add(nePubKeyName);
            LogHelper.WriteMethodLog(false);

        }

        private void InitKeyValues()
        {
            LogHelper.WriteMethodLog(true);
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("All");
            Dictionary<string, double> dickeyValue = this.keyHandler.GetDicKeyValue();
            foreach(var dicItem in dickeyValue)
            {
                this.comboBox1.Items.Add(dicItem.Key);
               
            }
            this.comboBox1.SelectedIndex = 0;
            LogHelper.WriteMethodLog(false);
        }

        //选择key
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            this.RefreshKeyValueBox();
            LogHelper.WriteMethodLog(false);
        }

        private void RefreshKeyValueBox()
        {
            LogHelper.WriteMethodLog(true);
            string strChoice = this.comboBox1.SelectedItem.ToString();
            string strPubKeyName = string.Empty;

            Dictionary<string, double> dickeyValue = this.keyHandler.GetDicKeyValue();
            double dVal = 0;
            if(dickeyValue.Count == 0)
            {
                this.textBoxValue.Text = dVal.ToString("0.0000");
                return;
            }

            if (strChoice == ConstHelper.BC_All)
            {
                KeyValuePair<string, double> firstKV = dickeyValue.First();
                strPubKeyName = firstKV.Key;
                foreach (var dicItem in dickeyValue)
                {

                    dVal += dicItem.Value;
                }
            }
            else
            {
                strPubKeyName = Path.Combine(AppSettings.XXPKeysFolder, strChoice);
                dickeyValue.TryGetValue(strChoice, out dVal);
            }

            string logKeyValue = string.Empty;
            foreach (var item in dickeyValue)
            {
                logKeyValue += string.Format("{0}:{1}", item.Key,item.Value.ToString("0.0000")) + Environment.NewLine;
            }
            LogHelper.WriteInfoLog(logKeyValue);

            string strPath = Path.Combine(AppSettings.XXPKeysFolder, strPubKeyName);          
            this.textBoxKeyHash.Text = this.keyHandler.pubkey2Hash(strPath);
            this.textBoxValue.Text = dVal.ToString("0.0000");
            LogHelper.WriteMethodLog(false);
        }

        private void ResearchNodes_Click(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            string Ip = this.textBoxSeedIP.Text;
            //step1 handshake
            bool bRet = this.commHandler.RequestHandshake(Ip);
            if(!bRet)
            {
                MessageBox.Show("Invalid seed IP");
                return;
            }
            //step2 get more Nodes IP
            List<string> lstAddress= this.commHandler.RequestMoreNodes(Ip);
            List<string> lstNew = new List<string>();           
            // step3 handshake lstAddress
            foreach (var item in lstAddress)
            {
                if(item == OSHelper.GetLocalIP())
                {
                    continue;
                }
                if(this.commHandler.RequestHandshake(item))
                {
                    lstNew.Add(item);
                }

            }
            // step4 send new addresses to all I know
            this.commHandler.SendNewAddress2Others(lstNew);
            this.textBoxConnectedNodes.Text = this.commHandler.GetAddressCount().ToString();

           if( LeveldbOperator.OpenDB(AppSettings.XXPDBFolder) != ConstHelper.BC_OK)
            {
                DBFileInfo df = this.commHandler.RequestHightestDBInfo();
                string str = string.Format("Ip:{0}, highest:{1} size:{2}", df.IP, df.LastBlockHeight, df.DBFileSize);
                
                Task.Run(() => {

                    string SavePath = Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                    long lRet = this.commHandler.StartReceiveFile(df.IP, df.DBFileSize, SavePath);
                    if(lRet == -1)
                    {
                        MessageBox.Show("try later, there is a file transfering now");                      
                    }
                    else
                    {
                        MessageBox.Show("Received: " + lRet.ToString());
                        FileIOHelper.DeleteDir(AppSettings.XXPDBFolder);
                        Directory.CreateDirectory(AppSettings.XXPDBFolder);
                        ZipHelper.UnZip(SavePath, AppSettings.XXPDBFolder);
                        this.InitFromDB();
                    }
                    this.commHandler.DisposeTransFileHelper();
                   


                });
                MessageBox.Show(str);
                if (this.commHandler.RequestStartTransDB(df.IP) != ConstHelper.BC_OK)
                {
                    
                }
            }
            LeveldbOperator.CloseDB();

            ResponseBlock BkInfo = this.commHandler.RequestNewBlockInfo(this.bkHandler.GetLastBlock());
            if(BkInfo.BlockResult == BlockResultType.Lower)
            {
               string sRet =  this.commHandler.GetNewBlocks(BkInfo.IP, this.bkHandler.GetLastBlock());
            }

            LogHelper.WriteMethodLog(false);
        }

        private void ReqSyncBlock(bool bCheckemptyDB=true)
        {
            LogHelper.WriteMethodLog(true);
            if (bCheckemptyDB)
            {
                if (LeveldbOperator.OpenDB(AppSettings.XXPDBFolder) != ConstHelper.BC_OK)
                {
                    DBFileInfo df = this.commHandler.RequestHightestDBInfo();
                    string str = string.Format("Ip:{0}, highest:{1} size:{2}", df.IP, df.LastBlockHeight, df.DBFileSize);
                    MessageBox.Show(str);
                    Task.Run(() => {

                        string SavePath = Path.Combine(AppSettings.XXPTempFolder, ConstHelper.BC_DBZipName);
                        long lRet = this.commHandler.StartReceiveFile(df.IP, df.DBFileSize, SavePath);
                        if (lRet == -1)
                        {
                            MessageBox.Show("try later, there is a file transfering now");
                        }
                        else
                        {
                            MessageBox.Show("Received: " + lRet.ToString());
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
           

            ResponseBlock BkInfo = this.commHandler.RequestNewBlockInfo(this.bkHandler.GetLastBlock());
            if (BkInfo.BlockResult == BlockResultType.Lower)
            {
                string sRet = this.commHandler.GetNewBlocks(BkInfo.IP, this.bkHandler.GetLastBlock());
            }
            LogHelper.WriteMethodLog(false);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //LeveldbOperator.CloseDB();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           // LeveldbOperator.CloseDB();
        }


        private string NewTransactionCallBack(Transaction Ts)
        {
            LogHelper.WriteMethodLog(true);
            if (this.txHandler.isValidTx(Ts))
            {
                return this.bkHandler.AddTransaction(Ts);
            }
            else
            {
                return Decision.Reject;
            }
            LogHelper.WriteMethodLog(false);

        }

        private string NewBlockCallBack(Block block)
        {
            LogHelper.WriteMethodLog(true);
            if (this.CurrentBkHash == block.Hash)
            {
                return Decision.Accepted;
            }
            int iRet = this.bkHandler.IsValidBlock(block);
            if (iRet==0)
            {
                if( this.bkHandler.WriteLastblock(block) == ConstHelper.BC_OK)
                {
                    Task.Run(()=> {
                        RefreshByNewBlock(block);
                    });
                }
                
                return Decision.Accept;
            }
            else
            {
                if(iRet==1)
                {
                    Task.Run(() => {
                        this.ReqSyncBlock(false);
                    });
                }
                return Decision.Reject;
            }
            LogHelper.WriteMethodLog(false);
        }

        void RefresfNodeCountCallBack(int iNodesCount)
        {
            LogHelper.WriteMethodLog(true);
            //this.textBoxConnectedNodes.Text = iNodesCount.ToString();

            this.textBoxConnectedNodes.Invoke(new MethodInvoker(() =>
            {
                this.textBoxConnectedNodes.Text = iNodesCount.ToString();

            }));
            LogHelper.WriteMethodLog(false);
        }

        private void RefreshByNewBlock(Block block)
        {
            LogHelper.WriteMethodLog(true);
            this.BeginInvoke(new MethodInvoker(() =>
            {
                this.bkHandler.RefreshLastBlock(block);
                this.textBox1.Text = this.bkHandler.strPuzzle;
                UTXOPool sigleBlockPool = this.txHandler.BlockData2UTXOPool(block);
                List<PubKeyValue> lstPubKeyValue = this.keyHandler.RefKVFromSigUTxpool(sigleBlockPool);
                if(lstPubKeyValue.Count > 0)
                {
                    string str = string.Empty;
                    foreach (var item in lstPubKeyValue)
                    {
                        str = str + string.Format("{0}Received: {1}", item.PubKeyNmae, item.Value) + Environment.NewLine;
                    }

                    MessageBox.Show(str);
                    this.InitKeyValues();
                }                
            }));

            LogHelper.WriteMethodLog(false);
        }

        private void button_printAlldb_Click(object sender, EventArgs e)
        {
            LogHelper.WriteMethodLog(true);
            LeveldbOperator.PrintAlldb();
            LogHelper.WriteMethodLog(false);
        }
    }
}
