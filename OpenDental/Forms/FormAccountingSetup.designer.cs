using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormAccountingSetup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAccountingSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.listAccountsDep = new UI.ListBoxOD();
			this.butRemove = new OpenDental.UI.Button();
			this.butChange = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textAccountInc = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butAdd = new OpenDental.UI.Button();
			this.groupAutomaticPayment = new System.Windows.Forms.GroupBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butChangeCash = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textAccountCashInc = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.butAddPay = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupAutomaticPayment.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 61);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(168, 53);
			this.label1.TabIndex = 2;
			this.label1.Text = "User will get to pick from this list of accounts to deposit into";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.listAccountsDep);
			this.groupBox1.Controls.Add(this.butRemove);
			this.groupBox1.Controls.Add(this.butChange);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textAccountInc);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.butAdd);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(17, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(519, 222);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Automatic Deposit Entries";
			// 
			// listAccountsDep
			// 
			this.listAccountsDep.Location = new System.Drawing.Point(182, 61);
			this.listAccountsDep.Name = "listAccountsDep";
			this.listAccountsDep.Size = new System.Drawing.Size(230, 108);
			this.listAccountsDep.TabIndex = 0;
			// 
			// butRemove
			// 
			this.butRemove.Location = new System.Drawing.Point(417, 91);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(75, 24);
			this.butRemove.TabIndex = 2;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(418, 176);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 3;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 179);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(168, 19);
			this.label3.TabIndex = 33;
			this.label3.Text = "Income Account";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAccountInc
			// 
			this.textAccountInc.Location = new System.Drawing.Point(182, 179);
			this.textAccountInc.Name = "textAccountInc";
			this.textAccountInc.ReadOnly = true;
			this.textAccountInc.Size = new System.Drawing.Size(230, 20);
			this.textAccountInc.TabIndex = 34;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(19, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(492, 27);
			this.label2.TabIndex = 32;
			this.label2.Text = "Every time a deposit is created, an accounting transaction will also be automatic" +
    "ally created.";
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(418, 61);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 1;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// groupAutomaticPayment
			// 
			this.groupAutomaticPayment.Controls.Add(this.gridMain);
			this.groupAutomaticPayment.Controls.Add(this.butChangeCash);
			this.groupAutomaticPayment.Controls.Add(this.label4);
			this.groupAutomaticPayment.Controls.Add(this.textAccountCashInc);
			this.groupAutomaticPayment.Controls.Add(this.label5);
			this.groupAutomaticPayment.Controls.Add(this.butAddPay);
			this.groupAutomaticPayment.Location = new System.Drawing.Point(17, 250);
			this.groupAutomaticPayment.Name = "groupAutomaticPayment";
			this.groupAutomaticPayment.Size = new System.Drawing.Size(519, 353);
			this.groupAutomaticPayment.TabIndex = 1;
			this.groupAutomaticPayment.TabStop = false;
			this.groupAutomaticPayment.Text = "Automatic Payment Entries";
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(22, 112);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(471, 177);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Auto Payment Entries";
			this.gridMain.TranslationName = "TableAccountingAutoPay";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butChangeCash
			// 
			this.butChangeCash.Location = new System.Drawing.Point(418, 307);
			this.butChangeCash.Name = "butChangeCash";
			this.butChangeCash.Size = new System.Drawing.Size(75, 24);
			this.butChangeCash.TabIndex = 2;
			this.butChangeCash.Text = "Change";
			this.butChangeCash.Click += new System.EventHandler(this.butChangeCash_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 310);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 19);
			this.label4.TabIndex = 33;
			this.label4.Text = "Income Account";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAccountCashInc
			// 
			this.textAccountCashInc.Location = new System.Drawing.Point(182, 310);
			this.textAccountCashInc.Name = "textAccountCashInc";
			this.textAccountCashInc.ReadOnly = true;
			this.textAccountCashInc.Size = new System.Drawing.Size(230, 20);
			this.textAccountCashInc.TabIndex = 34;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(19, 26);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(492, 47);
			this.label5.TabIndex = 32;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// butAddPay
			// 
			this.butAddPay.Location = new System.Drawing.Point(418, 83);
			this.butAddPay.Name = "butAddPay";
			this.butAddPay.Size = new System.Drawing.Size(75, 24);
			this.butAddPay.TabIndex = 0;
			this.butAddPay.Text = "Add";
			this.butAddPay.Click += new System.EventHandler(this.butAddPay_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(380, 633);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(461, 633);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormAccountingSetup
			// 
			this.ClientSize = new System.Drawing.Size(553, 669);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupAutomaticPayment);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormAccountingSetup";
			this.ShowInTaskbar = false;
			this.Text = "Setup Accounting";
			this.Load += new System.EventHandler(this.FormAccountingSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupAutomaticPayment.ResumeLayout(false);
			this.groupAutomaticPayment.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private OpenDental.UI.Button butAdd;
		private GroupBox groupBox1;
		private Label label2;
		private OpenDental.UI.Button butChange;
		private Label label3;
		private TextBox textAccountInc;
		private OpenDental.UI.Button butRemove;
		private UI.ListBoxOD listAccountsDep;
		private GroupBox groupAutomaticPayment;
		private OpenDental.UI.Button butChangeCash;
		private Label label4;
		private TextBox textAccountCashInc;
		private Label label5;
		private OpenDental.UI.Button butAddPay;
		private OpenDental.UI.GridOD gridMain;
	}
}
