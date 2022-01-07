namespace OpenDental {
	partial class FormPayPlanCredits {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanCredits));
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupCreditInfo = new System.Windows.Forms.GroupBox();
			this.textCode = new OpenDental.ODtextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butAddOrUpdate = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textAmt = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.butClear = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.textTotal = new OpenDental.ValidDouble();
			this.checkHideUnattached = new System.Windows.Forms.CheckBox();
			this.butPrint = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.checkShowImplicit = new System.Windows.Forms.CheckBox();
			this.groupCreditInfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 34);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(749, 432);
			this.gridMain.TabIndex = 0;
			this.gridMain.Title = "Available Procedures                                                 Payment Plan" +
    " Credits              ";
			this.gridMain.TranslationName = "TablePaymentPlanProcsAndCreds";
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(769, 474);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(850, 474);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupCreditInfo
			// 
			this.groupCreditInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupCreditInfo.Controls.Add(this.textCode);
			this.groupCreditInfo.Controls.Add(this.label6);
			this.groupCreditInfo.Controls.Add(this.butAddOrUpdate);
			this.groupCreditInfo.Controls.Add(this.butDelete);
			this.groupCreditInfo.Controls.Add(this.textNote);
			this.groupCreditInfo.Controls.Add(this.label4);
			this.groupCreditInfo.Controls.Add(this.textDate);
			this.groupCreditInfo.Controls.Add(this.label2);
			this.groupCreditInfo.Controls.Add(this.textAmt);
			this.groupCreditInfo.Controls.Add(this.label1);
			this.groupCreditInfo.Location = new System.Drawing.Point(766, 28);
			this.groupCreditInfo.Name = "groupCreditInfo";
			this.groupCreditInfo.Size = new System.Drawing.Size(167, 240);
			this.groupCreditInfo.TabIndex = 1;
			this.groupCreditInfo.TabStop = false;
			this.groupCreditInfo.Text = "Credit Information";
			// 
			// textCode
			// 
			this.textCode.AcceptsTab = true;
			this.textCode.BackColor = System.Drawing.SystemColors.Control;
			this.textCode.DetectLinksEnabled = false;
			this.textCode.DetectUrls = false;
			this.textCode.Location = new System.Drawing.Point(65, 22);
			this.textCode.Name = "textCode";
			this.textCode.QuickPasteType = OpenDentBusiness.QuickPasteType.ReadOnly;
			this.textCode.ReadOnly = true;
			this.textCode.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCode.Size = new System.Drawing.Size(96, 20);
			this.textCode.SpellCheckIsEnabled = false;
			this.textCode.TabIndex = 0;
			this.textCode.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 21);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(54, 20);
			this.label6.TabIndex = 26;
			this.label6.Text = "Code:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAddOrUpdate
			// 
			this.butAddOrUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddOrUpdate.Image = global::OpenDental.Properties.Resources.Left;
			this.butAddOrUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddOrUpdate.Location = new System.Drawing.Point(8, 208);
			this.butAddOrUpdate.Name = "butAddOrUpdate";
			this.butAddOrUpdate.Size = new System.Drawing.Size(73, 23);
			this.butAddOrUpdate.TabIndex = 4;
			this.butAddOrUpdate.Text = "Add";
			this.butAddOrUpdate.UseVisualStyleBackColor = true;
			this.butAddOrUpdate.Click += new System.EventHandler(this.butAddOrUpdate_Click);
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(88, 208);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(73, 23);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(8, 110);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.PayPlan;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(153, 83);
			this.textNote.SpellCheckIsEnabled = false;
			this.textNote.TabIndex = 3;
			this.textNote.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 89);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 20);
			this.label4.TabIndex = 24;
			this.label4.Text = "Note:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(65, 45);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(96, 20);
			this.textDate.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 20);
			this.label2.TabIndex = 20;
			this.label2.Text = "Amt:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAmt
			// 
			this.textAmt.Location = new System.Drawing.Point(65, 68);
			this.textAmt.MaxVal = 100000000D;
			this.textAmt.MinVal = -100000000D;
			this.textAmt.Name = "textAmt";
			this.textAmt.Size = new System.Drawing.Size(96, 20);
			this.textAmt.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(10, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 20);
			this.label1.TabIndex = 18;
			this.label1.Text = "Date:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// toolTip1
			// 
			this.toolTip1.AutoPopDelay = 0;
			this.toolTip1.InitialDelay = 10;
			this.toolTip1.ReshowDelay = 100;
			// 
			// butClear
			// 
			this.butClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(810, 274);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(73, 23);
			this.butClear.TabIndex = 4;
			this.butClear.Text = "Clear";
			this.butClear.UseVisualStyleBackColor = true;
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(409, 471);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(237, 20);
			this.label7.TabIndex = 29;
			this.label7.Text = "Total Credits Attached:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotal
			// 
			this.textTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textTotal.Location = new System.Drawing.Point(646, 472);
			this.textTotal.MaxVal = 100000000D;
			this.textTotal.MinVal = -100000000D;
			this.textTotal.Name = "textTotal";
			this.textTotal.ReadOnly = true;
			this.textTotal.Size = new System.Drawing.Size(96, 20);
			this.textTotal.TabIndex = 28;
			// 
			// checkHideUnattached
			// 
			this.checkHideUnattached.Location = new System.Drawing.Point(93, 16);
			this.checkHideUnattached.Name = "checkHideUnattached";
			this.checkHideUnattached.Size = new System.Drawing.Size(203, 18);
			this.checkHideUnattached.TabIndex = 30;
			this.checkHideUnattached.Text = "Hide Unattached Procedures";
			this.checkHideUnattached.UseVisualStyleBackColor = true;
			this.checkHideUnattached.CheckedChanged += new System.EventHandler(this.checkHideUnattached_CheckedChanged);
			// 
			// butPrint
			// 
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(12, 8);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 31;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(767, 300);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(166, 48);
			this.label3.TabIndex = 32;
			this.label3.Text = "Deselects everything in the grid.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// checkShowImplicit
			// 
			this.checkShowImplicit.Location = new System.Drawing.Point(302, 16);
			this.checkShowImplicit.Name = "checkShowImplicit";
			this.checkShowImplicit.Size = new System.Drawing.Size(394, 18);
			this.checkShowImplicit.TabIndex = 33;
			this.checkShowImplicit.Text = "Show procedures that have not been explicitly paid off";
			this.checkShowImplicit.UseVisualStyleBackColor = true;
			this.checkShowImplicit.CheckedChanged += new System.EventHandler(this.checkShowImplicit_CheckedChanged);
			// 
			// FormPayPlanCredits
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(937, 510);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.checkShowImplicit);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.checkHideUnattached);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.groupCreditInfo);
			this.Controls.Add(this.gridMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPlanCredits";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Payment Plan Procedures and Credits";
			this.Load += new System.EventHandler(this.FormPayPlanCredits_Load);
			this.groupCreditInfo.ResumeLayout(false);
			this.groupCreditInfo.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupCreditInfo;
		private System.Windows.Forms.Label label2;
		private ValidDouble textAmt;
		private System.Windows.Forms.Label label1;
		private ValidDate textDate;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label4;
		private UI.Button butDelete;
		private System.Windows.Forms.ToolTip toolTip1;
		private UI.Button butAddOrUpdate;
		private ODtextBox textCode;
		private System.Windows.Forms.Label label6;
		private UI.Button butClear;
		private System.Windows.Forms.Label label7;
		private ValidDouble textTotal;
		private System.Windows.Forms.CheckBox checkHideUnattached;
		private UI.Button butPrint;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkShowImplicit;
	}
}