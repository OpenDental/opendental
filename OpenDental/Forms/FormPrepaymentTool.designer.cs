namespace OpenDental {
	partial class FormPrepaymentTool {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrepaymentTool));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.GridPrepayment = new OpenDental.UI.GridOD();
			this.butAddMonths = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textMonths = new System.Windows.Forms.TextBox();
			this.button4 = new OpenDental.UI.Button();
			this.button5 = new OpenDental.UI.Button();
			this.butAdd12 = new OpenDental.UI.Button();
			this.GridCodes = new OpenDental.UI.GridOD();
			this.label3 = new System.Windows.Forms.Label();
			this.groupAddProcedures = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textTotal = new OpenDental.ValidDouble();
			this.textOrigSub = new OpenDental.ValidDouble();
			this.textTaxSub = new OpenDental.ValidDouble();
			this.labelUnsentTotals = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDiscount = new OpenDental.ValidDouble();
			this.label5 = new System.Windows.Forms.Label();
			this.GridCompletedProcs = new OpenDental.UI.GridOD();
			this.butPreviousProc = new OpenDental.UI.Button();
			this.groupAddProcedures.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(515, 544);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(596, 544);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// GridPrepayment
			// 
			this.GridPrepayment.Location = new System.Drawing.Point(22, 352);
			this.GridPrepayment.Name = "GridPrepayment";
			this.GridPrepayment.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.GridPrepayment.Size = new System.Drawing.Size(593, 153);
			this.GridPrepayment.TabIndex = 9;
			this.GridPrepayment.Title = "Prepayments";
			this.GridPrepayment.TranslationName = "gridPrepayments";
			this.GridPrepayment.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.GridPrepayment_CellDoubleClick);
			// 
			// butAddMonths
			// 
			this.butAddMonths.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddMonths.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddMonths.Location = new System.Drawing.Point(32, 135);
			this.butAddMonths.Name = "butAddMonths";
			this.butAddMonths.Size = new System.Drawing.Size(73, 24);
			this.butAddMonths.TabIndex = 73;
			this.butAddMonths.Text = "Add";
			this.butAddMonths.Click += new System.EventHandler(this.butAddMonths_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 112);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(52, 13);
			this.label1.TabIndex = 74;
			this.label1.Text = "Custom #";
			// 
			// textMonths
			// 
			this.textMonths.Location = new System.Drawing.Point(67, 109);
			this.textMonths.Name = "textMonths";
			this.textMonths.Size = new System.Drawing.Size(38, 20);
			this.textMonths.TabIndex = 75;
			// 
			// button4
			// 
			this.button4.Icon = OpenDental.UI.EnumIcons.Add;
			this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button4.Location = new System.Drawing.Point(32, 19);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(73, 24);
			this.button4.TabIndex = 84;
			this.button4.Text = "Add 1";
			this.button4.Click += new System.EventHandler(this.butAdd1_Click);
			// 
			// button5
			// 
			this.button5.Icon = OpenDental.UI.EnumIcons.Add;
			this.button5.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.button5.Location = new System.Drawing.Point(32, 49);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(73, 24);
			this.button5.TabIndex = 85;
			this.button5.Text = "Add 6";
			this.button5.Click += new System.EventHandler(this.butAdd6_Click);
			// 
			// butAdd12
			// 
			this.butAdd12.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd12.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd12.Location = new System.Drawing.Point(32, 79);
			this.butAdd12.Name = "butAdd12";
			this.butAdd12.Size = new System.Drawing.Size(73, 24);
			this.butAdd12.TabIndex = 86;
			this.butAdd12.Text = "Add 12";
			this.butAdd12.Click += new System.EventHandler(this.butAdd12_Click);
			// 
			// GridCodes
			// 
			this.GridCodes.Location = new System.Drawing.Point(22, 21);
			this.GridCodes.Name = "GridCodes";
			this.GridCodes.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.GridCodes.Size = new System.Drawing.Size(383, 189);
			this.GridCodes.TabIndex = 87;
			this.GridCodes.Title = "Codes";
			this.GridCodes.TranslationName = "GridSupport";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(22, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(60, 20);
			this.label3.TabIndex = 94;
			this.label3.Text = "Step 1:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupAddProcedures
			// 
			this.groupAddProcedures.Controls.Add(this.butAddMonths);
			this.groupAddProcedures.Controls.Add(this.label1);
			this.groupAddProcedures.Controls.Add(this.textMonths);
			this.groupAddProcedures.Controls.Add(this.button4);
			this.groupAddProcedures.Controls.Add(this.button5);
			this.groupAddProcedures.Controls.Add(this.butAdd12);
			this.groupAddProcedures.Location = new System.Drawing.Point(411, 21);
			this.groupAddProcedures.Name = "groupAddProcedures";
			this.groupAddProcedures.Size = new System.Drawing.Size(146, 163);
			this.groupAddProcedures.TabIndex = 96;
			this.groupAddProcedures.TabStop = false;
			this.groupAddProcedures.Text = "Number of Months";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(411, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 20);
			this.label4.TabIndex = 95;
			this.label4.Text = "Step 2:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butDelete
			// 
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(35, 511);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 26);
			this.butDelete.TabIndex = 97;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textTotal
			// 
			this.textTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textTotal.Location = new System.Drawing.Point(523, 512);
			this.textTotal.MaxVal = 100000000D;
			this.textTotal.MinVal = -100000000D;
			this.textTotal.Name = "textTotal";
			this.textTotal.ReadOnly = true;
			this.textTotal.Size = new System.Drawing.Size(75, 20);
			this.textTotal.TabIndex = 121;
			this.textTotal.TabStop = false;
			this.textTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textOrigSub
			// 
			this.textOrigSub.Location = new System.Drawing.Point(301, 512);
			this.textOrigSub.MaxVal = 100000000D;
			this.textOrigSub.MinVal = -100000000D;
			this.textOrigSub.Name = "textOrigSub";
			this.textOrigSub.ReadOnly = true;
			this.textOrigSub.Size = new System.Drawing.Size(75, 20);
			this.textOrigSub.TabIndex = 123;
			this.textOrigSub.TabStop = false;
			this.textOrigSub.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textTaxSub
			// 
			this.textTaxSub.Location = new System.Drawing.Point(449, 512);
			this.textTaxSub.MaxVal = 100000000D;
			this.textTaxSub.MinVal = -100000000D;
			this.textTaxSub.Name = "textTaxSub";
			this.textTaxSub.ReadOnly = true;
			this.textTaxSub.Size = new System.Drawing.Size(75, 20);
			this.textTaxSub.TabIndex = 119;
			this.textTaxSub.TabStop = false;
			this.textTaxSub.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelUnsentTotals
			// 
			this.labelUnsentTotals.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUnsentTotals.Location = new System.Drawing.Point(234, 514);
			this.labelUnsentTotals.Name = "labelUnsentTotals";
			this.labelUnsentTotals.Size = new System.Drawing.Size(60, 17);
			this.labelUnsentTotals.TabIndex = 122;
			this.labelUnsentTotals.Text = "Totals";
			this.labelUnsentTotals.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(328, 671);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(0, 13);
			this.label2.TabIndex = 124;
			// 
			// textDiscount
			// 
			this.textDiscount.Location = new System.Drawing.Point(375, 512);
			this.textDiscount.MaxVal = 100000000D;
			this.textDiscount.MinVal = -100000000D;
			this.textDiscount.Name = "textDiscount";
			this.textDiscount.ReadOnly = true;
			this.textDiscount.Size = new System.Drawing.Size(75, 20);
			this.textDiscount.TabIndex = 125;
			this.textDiscount.TabStop = false;
			this.textDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(289, 579);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(382, 128);
			this.label5.TabIndex = 126;
			this.label5.Text = resources.GetString("label5.Text");
			// 
			// GridCompletedProcs
			// 
			this.GridCompletedProcs.Location = new System.Drawing.Point(22, 214);
			this.GridCompletedProcs.Name = "GridCompletedProcs";
			this.GridCompletedProcs.Size = new System.Drawing.Size(593, 132);
			this.GridCompletedProcs.TabIndex = 127;
			this.GridCompletedProcs.Title = "Previously Completed Procedures";
			this.GridCompletedProcs.TranslationName = "gridPreviousProc";
			// 
			// butPreviousProc
			// 
			this.butPreviousProc.Location = new System.Drawing.Point(411, 187);
			this.butPreviousProc.Name = "butPreviousProc";
			this.butPreviousProc.Size = new System.Drawing.Size(146, 23);
			this.butPreviousProc.TabIndex = 128;
			this.butPreviousProc.Text = "Previously Completed Proc";
			this.butPreviousProc.UseVisualStyleBackColor = true;
			this.butPreviousProc.Click += new System.EventHandler(this.butPreviousProc_Click);
			// 
			// FormPrepaymentTool
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(683, 709);
			this.Controls.Add(this.butPreviousProc);
			this.Controls.Add(this.GridCompletedProcs);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.GridCodes);
			this.Controls.Add(this.textDiscount);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.groupAddProcedures);
			this.Controls.Add(this.textOrigSub);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textTaxSub);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelUnsentTotals);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.GridPrepayment);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPrepaymentTool";
			this.Text = "Prepayment Tool";
			this.Load += new System.EventHandler(this.FormPrepaymentTool_Load);
			this.groupAddProcedures.ResumeLayout(false);
			this.groupAddProcedures.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD GridPrepayment;
		private UI.Button butAddMonths;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textMonths;
		private UI.Button button4;
		private UI.Button button5;
		private UI.Button butAdd12;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupAddProcedures;
		private UI.Button butDelete;
		private ValidDouble textTotal;
		private ValidDouble textOrigSub;
		private ValidDouble textTaxSub;
		private System.Windows.Forms.Label labelUnsentTotals;
		private System.Windows.Forms.Label label2;
		private ValidDouble textDiscount;
		private UI.GridOD GridCodes;
		private System.Windows.Forms.Label label5;
		private UI.GridOD GridCompletedProcs;
		private UI.Button butPreviousProc;
	}
}