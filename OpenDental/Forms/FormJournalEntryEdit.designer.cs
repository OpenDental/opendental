using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormJournalEntryEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJournalEntryEdit));
			this.labelMemo = new System.Windows.Forms.Label();
			this.labelDebit = new System.Windows.Forms.Label();
			this.textMemo = new System.Windows.Forms.TextBox();
			this.labelCredit = new System.Windows.Forms.Label();
			this.textCheckNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textAccount = new System.Windows.Forms.TextBox();
			this.butChange = new OpenDental.UI.Button();
			this.textCredit = new OpenDental.ValidDouble();
			this.textDebit = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelReconcile = new System.Windows.Forms.Label();
			this.textReconcile = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// labelMemo
			// 
			this.labelMemo.Location = new System.Drawing.Point(16, 94);
			this.labelMemo.Name = "labelMemo";
			this.labelMemo.Size = new System.Drawing.Size(93, 16);
			this.labelMemo.TabIndex = 0;
			this.labelMemo.Text = "Memo";
			this.labelMemo.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDebit
			// 
			this.labelDebit.Location = new System.Drawing.Point(110, 14);
			this.labelDebit.Name = "labelDebit";
			this.labelDebit.Size = new System.Drawing.Size(90, 16);
			this.labelDebit.TabIndex = 4;
			this.labelDebit.Text = "Debit";
			this.labelDebit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textMemo
			// 
			this.textMemo.Location = new System.Drawing.Point(110, 90);
			this.textMemo.Multiline = true;
			this.textMemo.Name = "textMemo";
			this.textMemo.Size = new System.Drawing.Size(230, 46);
			this.textMemo.TabIndex = 4;
			// 
			// labelCredit
			// 
			this.labelCredit.Location = new System.Drawing.Point(205, 14);
			this.labelCredit.Name = "labelCredit";
			this.labelCredit.Size = new System.Drawing.Size(90, 16);
			this.labelCredit.TabIndex = 19;
			this.labelCredit.Text = "Credit";
			this.labelCredit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// textCheckNumber
			// 
			this.textCheckNumber.Location = new System.Drawing.Point(110, 145);
			this.textCheckNumber.Name = "textCheckNumber";
			this.textCheckNumber.Size = new System.Drawing.Size(132, 20);
			this.textCheckNumber.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 149);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 16);
			this.label2.TabIndex = 20;
			this.label2.Text = "Check Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(7, 63);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 23;
			this.label5.Text = "Account";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textAccount
			// 
			this.textAccount.Location = new System.Drawing.Point(110, 60);
			this.textAccount.Name = "textAccount";
			this.textAccount.ReadOnly = true;
			this.textAccount.Size = new System.Drawing.Size(230, 20);
			this.textAccount.TabIndex = 9;
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(346, 57);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 26);
			this.butChange.TabIndex = 5;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// textCredit
			// 
			this.textCredit.Location = new System.Drawing.Point(205, 31);
			this.textCredit.MaxVal = 100000000D;
			this.textCredit.MinVal = -100000000D;
			this.textCredit.Name = "textCredit";
			this.textCredit.Size = new System.Drawing.Size(90, 20);
			this.textCredit.TabIndex = 2;
			// 
			// textDebit
			// 
			this.textDebit.Location = new System.Drawing.Point(110, 31);
			this.textDebit.MaxVal = 100000000D;
			this.textDebit.MinVal = -100000000D;
			this.textDebit.Name = "textDebit";
			this.textDebit.Size = new System.Drawing.Size(90, 20);
			this.textDebit.TabIndex = 1;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 219);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 26);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(430, 219);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(430, 181);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelReconcile
			// 
			this.labelReconcile.Location = new System.Drawing.Point(110, 199);
			this.labelReconcile.Name = "labelReconcile";
			this.labelReconcile.Size = new System.Drawing.Size(230, 21);
			this.labelReconcile.TabIndex = 27;
			this.labelReconcile.Text = "Attached to Reconcile";
			this.labelReconcile.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textReconcile
			// 
			this.textReconcile.Location = new System.Drawing.Point(110, 222);
			this.textReconcile.Name = "textReconcile";
			this.textReconcile.ReadOnly = true;
			this.textReconcile.Size = new System.Drawing.Size(112, 20);
			this.textReconcile.TabIndex = 10;
			// 
			// FormJournalEntryEdit
			// 
			this.ClientSize = new System.Drawing.Size(517, 257);
			this.Controls.Add(this.textReconcile);
			this.Controls.Add(this.labelReconcile);
			this.Controls.Add(this.butChange);
			this.Controls.Add(this.textAccount);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textCheckNumber);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textCredit);
			this.Controls.Add(this.labelCredit);
			this.Controls.Add(this.textMemo);
			this.Controls.Add(this.textDebit);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelDebit);
			this.Controls.Add(this.labelMemo);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormJournalEntryEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Journal Entry";
			this.Load += new System.EventHandler(this.FormJournalEntryEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label labelMemo;
		private System.Windows.Forms.Label labelDebit;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ValidDouble textDebit;
		private TextBox textMemo;
		private ValidDouble textCredit;
		private Label labelCredit;
		private TextBox textCheckNumber;
		private Label label2;
		private Label label5;
		private TextBox textAccount;
		private OpenDental.UI.Button butChange;
		private Label labelReconcile;
		private TextBox textReconcile;
	}
}
