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

        //private string MyAddressScript;

        public Form1()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.xml"));
            LogHelper.WriteInfoLog(string.Format("当前产品名：{0}，当前产品版本：{1}", new object[] { System.Windows.Forms.Application.ProductName, System.Windows.Forms.Application.ProductVersion }));

            InitializeComponent();
            this.InitAppseting();


            this.bkHandler = new BkHandler();
            this.txHandler = new TxHandler();
            this.keyHandler = new KeyHandler();
            this.commHandler = new Communication();


            this.bkHandler.GetLastBlock();          
            this.textBox1.Text = this.bkHandler.strPuzzle;
            this.txHandler.CreatUTPoolFromDB(AppSettings.XXPDBFolder);
            this.keyHandler.RefreshKeyvalFromUtxopool(this.txHandler.GetUtxoPool());
            this.InitKeyValues();

            this.textBoxConnectedNodes.Text = this.commHandler.GetAddressCount().ToString();
        }


        private void InitAppseting()
        {

            #region init peizhi
            //应用程序版本号
            AppSettings.ProductVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            AppSettings.XXPCommonFolder = string.Format(AppConfigHelper.GetConfigValByKey("XXPCommonFolder"), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            AppSettings.XXPCommport = Convert.ToInt32(AppConfigHelper.GetConfigValByKey("XXPCommPort"));

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
        }

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    LeveldbOperator.CloseDB();
            
        //}

        /// <summary>
        /// Creat Block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Digcoin_Click(object sender, EventArgs e)
        {
            //string strChangePuKScript = this.keyHandler.PubKeyHash2Script(this.textBoxKeyHash.Text);
            //this.bkHandler.CreatBaseCoinBlock(strChangePuKScript);

            string sBaseCoinScript = this.keyHandler.PubKeyHash2Script(this.textBoxKeyHash.Text);
            Transaction basecoinTrans = this.bkHandler.CoinBaseTX(sBaseCoinScript);
            this.bkHandler.InsertBasecoin(basecoinTrans);
            string strRet = this.bkHandler.CreatBlock(this.textBox2.Text);
            if (strRet == ConstHelper.BC_OK)
            {
                this.txHandler.handleBaseCoin(basecoinTrans);
                this.keyHandler.RefreshKeyvalFromUtxopool(this.txHandler.GetUtxoPool());
                this.RefreshKeyValueBox();
                this.bkHandler.ClearTransPool();

                Block lastBlock = this.bkHandler.GetLastBlock();
                this.txHandler.RefreshUTPoolByBlock(lastBlock);
                this.textBox1.Text = this.bkHandler.strPuzzle;
                this.textBox2.Text = "";
            }
            else
            {
                MessageBox.Show(strRet);
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.textBox2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonSpent_Click(object sender, EventArgs e)
        {
            
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


            Transaction trans = this.txHandler.CreatTransaction( dicInputUtxo, dInputTatolAmount, dPaytoAmount, 
                                                                 strPaytoHash, strChangePuKScript );

            strRet = this.txHandler.handleTxs(trans);

            if(strRet != ConstHelper.BC_OK)
            {
                MessageBox.Show(strRet);
                return;
            }
            this.bkHandler.AddTransaction(trans);
            this.keyHandler.RefreshKeyvalFromUtxopool(this.txHandler.GetUtxoPool());
            this.RefreshKeyValueBox();
            this.TextBoxAmount.Text = "";
            this.textBoxPaytoHash.Text = "";

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void GenerateKeypair_Click(object sender, EventArgs e)
        {
            string nePubKeyName = this.keyHandler.GernerateKeypairs();
            this.comboBox1.Items.Add(nePubKeyName);


            //LeveldbOperator.PrintAlldb();

        }

        private void InitKeyValues()
        {
            this.comboBox1.Items.Clear();
            this.comboBox1.Items.Add("All");
            Dictionary<string, double> dickeyValue = this.keyHandler.GetDicKeyValue();
            foreach(var dicItem in dickeyValue)
            {
                this.comboBox1.Items.Add(dicItem.Key);
               
            }
            this.comboBox1.SelectedIndex = 0;
        }

        //选择key
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RefreshKeyValueBox();
        }

        private void RefreshKeyValueBox()
        {
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

            string strPath = Path.Combine(AppSettings.XXPKeysFolder, strPubKeyName);
            //this.MyAddressScript = this.keyHandler.pubkey2Script(strPath);
            this.textBoxKeyHash.Text = this.keyHandler.pubkey2Hash(strPath);
            this.textBoxValue.Text = dVal.ToString("0.0000");
        }

        private void ResearchNodes_Click(object sender, EventArgs e)
        {
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
                MessageBox.Show(str);
                Task.Run(() => {

                    this.commHandler.StartReceiveFile(df.IP);
                });

                if (this.commHandler.RequestStartTransDB(df.IP))
                {

                }
            }

        }
    }
}
