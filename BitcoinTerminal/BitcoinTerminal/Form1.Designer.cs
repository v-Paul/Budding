namespace BitcoinTerminal
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Digcoin = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.buttonSpent = new System.Windows.Forms.Button();
            this.GenerateKeypair = new System.Windows.Forms.Button();
            this.textBoxValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TextBoxAmount = new System.Windows.Forms.TextBox();
            this.textBoxPaytoHash = new System.Windows.Forms.TextBox();
            this.textBoxKeyHash = new System.Windows.Forms.TextBox();
            this.textBoxConnectedNodes = new System.Windows.Forms.TextBox();
            this.textBoxSeedIP = new System.Windows.Forms.TextBox();
            this.ResearchNodes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Digcoin
            // 
            this.Digcoin.Location = new System.Drawing.Point(47, 155);
            this.Digcoin.Name = "Digcoin";
            this.Digcoin.Size = new System.Drawing.Size(100, 29);
            this.Digcoin.TabIndex = 0;
            this.Digcoin.Text = "CreatBlock";
            this.Digcoin.UseVisualStyleBackColor = true;
            this.Digcoin.Click += new System.EventHandler(this.Digcoin_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(163, 160);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(291, 160);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(153, 20);
            this.textBox2.TabIndex = 2;
            // 
            // buttonSpent
            // 
            this.buttonSpent.Location = new System.Drawing.Point(47, 217);
            this.buttonSpent.Name = "buttonSpent";
            this.buttonSpent.Size = new System.Drawing.Size(100, 32);
            this.buttonSpent.TabIndex = 3;
            this.buttonSpent.Text = "Spent";
            this.buttonSpent.UseVisualStyleBackColor = true;
            this.buttonSpent.Click += new System.EventHandler(this.buttonSpent_Click);
            // 
            // GenerateKeypair
            // 
            this.GenerateKeypair.Location = new System.Drawing.Point(47, 30);
            this.GenerateKeypair.Name = "GenerateKeypair";
            this.GenerateKeypair.Size = new System.Drawing.Size(100, 30);
            this.GenerateKeypair.TabIndex = 4;
            this.GenerateKeypair.Text = "GenerateKeypair";
            this.GenerateKeypair.UseVisualStyleBackColor = true;
            this.GenerateKeypair.Click += new System.EventHandler(this.GenerateKeypair_Click);
            // 
            // textBoxValue
            // 
            this.textBoxValue.Location = new System.Drawing.Point(291, 40);
            this.textBoxValue.Name = "textBoxValue";
            this.textBoxValue.Size = new System.Drawing.Size(100, 20);
            this.textBoxValue.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(201, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "KeyPairs";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(204, 39);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(74, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(288, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Value";
            // 
            // TextBoxAmount
            // 
            this.TextBoxAmount.Location = new System.Drawing.Point(163, 228);
            this.TextBoxAmount.Name = "TextBoxAmount";
            this.TextBoxAmount.Size = new System.Drawing.Size(44, 20);
            this.TextBoxAmount.TabIndex = 9;
            // 
            // textBoxPaytoHash
            // 
            this.textBoxPaytoHash.Location = new System.Drawing.Point(222, 228);
            this.textBoxPaytoHash.Name = "textBoxPaytoHash";
            this.textBoxPaytoHash.Size = new System.Drawing.Size(222, 20);
            this.textBoxPaytoHash.TabIndex = 10;
            // 
            // textBoxKeyHash
            // 
            this.textBoxKeyHash.Location = new System.Drawing.Point(47, 83);
            this.textBoxKeyHash.Name = "textBoxKeyHash";
            this.textBoxKeyHash.Size = new System.Drawing.Size(397, 20);
            this.textBoxKeyHash.TabIndex = 11;
            // 
            // textBoxConnectedNodes
            // 
            this.textBoxConnectedNodes.Location = new System.Drawing.Point(400, 294);
            this.textBoxConnectedNodes.Name = "textBoxConnectedNodes";
            this.textBoxConnectedNodes.Size = new System.Drawing.Size(44, 20);
            this.textBoxConnectedNodes.TabIndex = 12;
            // 
            // textBoxSeedIP
            // 
            this.textBoxSeedIP.Location = new System.Drawing.Point(222, 294);
            this.textBoxSeedIP.Name = "textBoxSeedIP";
            this.textBoxSeedIP.Size = new System.Drawing.Size(169, 20);
            this.textBoxSeedIP.TabIndex = 13;
            // 
            // ResearchNodes
            // 
            this.ResearchNodes.Location = new System.Drawing.Point(99, 294);
            this.ResearchNodes.Name = "ResearchNodes";
            this.ResearchNodes.Size = new System.Drawing.Size(108, 23);
            this.ResearchNodes.TabIndex = 14;
            this.ResearchNodes.Text = "ResearchNodes";
            this.ResearchNodes.UseVisualStyleBackColor = true;
            this.ResearchNodes.Click += new System.EventHandler(this.ResearchNodes_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 375);
            this.Controls.Add(this.ResearchNodes);
            this.Controls.Add(this.textBoxSeedIP);
            this.Controls.Add(this.textBoxConnectedNodes);
            this.Controls.Add(this.textBoxKeyHash);
            this.Controls.Add(this.textBoxPaytoHash);
            this.Controls.Add(this.TextBoxAmount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxValue);
            this.Controls.Add(this.GenerateKeypair);
            this.Controls.Add(this.buttonSpent);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Digcoin);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Digcoin;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button buttonSpent;
        private System.Windows.Forms.Button GenerateKeypair;
        private System.Windows.Forms.TextBox textBoxValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TextBoxAmount;
        private System.Windows.Forms.TextBox textBoxPaytoHash;
        private System.Windows.Forms.TextBox textBoxKeyHash;
        private System.Windows.Forms.TextBox textBoxConnectedNodes;
        private System.Windows.Forms.TextBox textBoxSeedIP;
        private System.Windows.Forms.Button ResearchNodes;
    }
}

