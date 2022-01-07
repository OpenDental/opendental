namespace OpenDental {
	partial class FormAdjMulti {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAdjMulti));
			this.label5 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.label3 = new System.Windows.Forms.Label();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.labelAdjDate = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butCancel = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.butPickProv = new OpenDental.UI.Button();
			this.dateAdjustment = new OpenDental.ValidDate();
			this.radioFixedAmt = new System.Windows.Forms.RadioButton();
			this.radioPercentRemBal = new System.Windows.Forms.RadioButton();
			this.textAmt = new OpenDental.ValidDouble();
			this.radioPercentProcFee = new System.Windows.Forms.RadioButton();
			this.labelAmount = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listTypePos = new OpenDental.UI.ListBoxOD();
			this.labelAdditions = new System.Windows.Forms.Label();
			this.labelSubtractions = new System.Windows.Forms.Label();
			this.listTypeNeg = new OpenDental.UI.ListBoxOD();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioIncludeAll = new System.Windows.Forms.RadioButton();
			this.radioAllocatedOnly = new System.Windows.Forms.RadioButton();
			this.radioExcludeAll = new System.Windows.Forms.RadioButton();
			this.groupCreditLogic = new System.Windows.Forms.GroupBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupCreditLogic.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label5.Location = new System.Drawing.Point(30, 514);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 14);
			this.label5.TabIndex = 51;
			this.label5.Text = "Note:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboClinic
			// 
			this.comboClinic.ForceShowUnassigned = true;
			this.comboClinic.HqDescription = "Inherit";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(42, 43);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(224, 21);
			this.comboClinic.TabIndex = 45;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(73, 17);
			this.label3.TabIndex = 44;
			this.label3.Text = "Provider";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProv
			// 
			//this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			//this.comboProv.FormattingEnabled = true;
			this.comboProv.Location = new System.Drawing.Point(79, 19);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(187, 21);
			this.comboProv.TabIndex = 42;
			// 
			// labelAdjDate
			// 
			this.labelAdjDate.Location = new System.Drawing.Point(3, 19);
			this.labelAdjDate.Name = "labelAdjDate";
			this.labelAdjDate.Size = new System.Drawing.Size(89, 20);
			this.labelAdjDate.TabIndex = 36;
			this.labelAdjDate.Text = "Date";
			this.labelAdjDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(31, 263);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(712, 246);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Available Procedures";
			this.gridMain.TranslationName = "TableMultiAdjs";
			this.gridMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseUp);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(780, 577);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 53;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(780, 547);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 52;
			this.butOk.Text = "&OK";
			this.butOk.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAdd
			// 
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(612, 234);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(131, 23);
			this.butAdd.TabIndex = 49;
			this.butAdd.Text = "Add Adjustments";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(780, 486);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(76, 23);
			this.butDelete.TabIndex = 50;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(33, 531);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Adjustment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(446, 70);
			this.textNote.SpellCheckIsEnabled = false;
			this.textNote.TabIndex = 48;
			this.textNote.Text = "";
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(268, 19);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(23, 21);
			this.butPickProv.TabIndex = 43;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// dateAdjustment
			// 
			this.dateAdjustment.Location = new System.Drawing.Point(97, 20);
			this.dateAdjustment.Name = "dateAdjustment";
			this.dateAdjustment.Size = new System.Drawing.Size(80, 20);
			this.dateAdjustment.TabIndex = 54;
			// 
			// radioFixedAmt
			// 
			this.radioFixedAmt.Checked = true;
			this.radioFixedAmt.Location = new System.Drawing.Point(152, 20);
			this.radioFixedAmt.Name = "radioFixedAmt";
			this.radioFixedAmt.Size = new System.Drawing.Size(191, 17);
			this.radioFixedAmt.TabIndex = 161;
			this.radioFixedAmt.TabStop = true;
			this.radioFixedAmt.Text = "Fixed Amount";
			this.radioFixedAmt.UseVisualStyleBackColor = true;
			this.radioFixedAmt.CheckedChanged += new System.EventHandler(this.radioFixedAmt_CheckedChanged);
			// 
			// radioPercentRemBal
			// 
			this.radioPercentRemBal.Location = new System.Drawing.Point(152, 38);
			this.radioPercentRemBal.Name = "radioPercentRemBal";
			this.radioPercentRemBal.Size = new System.Drawing.Size(191, 17);
			this.radioPercentRemBal.TabIndex = 160;
			this.radioPercentRemBal.Text = "Percent of Remaining Balance";
			this.radioPercentRemBal.UseVisualStyleBackColor = true;
			// 
			// textAmt
			// 
			this.textAmt.Location = new System.Drawing.Point(77, 33);
			this.textAmt.MaxVal = 100000000D;
			this.textAmt.MinVal = -100000000D;
			this.textAmt.Name = "textAmt";
			this.textAmt.Size = new System.Drawing.Size(68, 20);
			this.textAmt.TabIndex = 0;
			// 
			// radioPercentProcFee
			// 
			this.radioPercentProcFee.Location = new System.Drawing.Point(152, 56);
			this.radioPercentProcFee.Name = "radioPercentProcFee";
			this.radioPercentProcFee.Size = new System.Drawing.Size(190, 17);
			this.radioPercentProcFee.TabIndex = 160;
			this.radioPercentProcFee.Text = "Percent of Fee";
			this.radioPercentProcFee.UseVisualStyleBackColor = true;
			// 
			// labelAmount
			// 
			this.labelAmount.Location = new System.Drawing.Point(6, 33);
			this.labelAmount.Name = "labelAmount";
			this.labelAmount.Size = new System.Drawing.Size(70, 20);
			this.labelAmount.TabIndex = 41;
			this.labelAmount.Text = "Amount";
			this.labelAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textAmt);
			this.groupBox1.Controls.Add(this.radioPercentRemBal);
			this.groupBox1.Controls.Add(this.radioFixedAmt);
			this.groupBox1.Controls.Add(this.labelAmount);
			this.groupBox1.Controls.Add(this.radioPercentProcFee);
			this.groupBox1.Location = new System.Drawing.Point(20, 47);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(379, 80);
			this.groupBox1.TabIndex = 162;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Adjust Per Procedure";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.comboProv);
			this.groupBox2.Controls.Add(this.butPickProv);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.comboClinic);
			this.groupBox2.Location = new System.Drawing.Point(20, 130);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(302, 75);
			this.groupBox2.TabIndex = 163;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Override";
			// 
			// listTypePos
			// 
			this.listTypePos.Location = new System.Drawing.Point(421, 37);
			this.listTypePos.Name = "listTypePos";
			this.listTypePos.Size = new System.Drawing.Size(202, 160);
			this.listTypePos.TabIndex = 164;
			this.listTypePos.SelectedIndexChanged += new System.EventHandler(this.listTypePos_SelectedIndexChanged);
			// 
			// labelAdditions
			// 
			this.labelAdditions.Location = new System.Drawing.Point(421, 17);
			this.labelAdditions.Name = "labelAdditions";
			this.labelAdditions.Size = new System.Drawing.Size(202, 16);
			this.labelAdditions.TabIndex = 166;
			this.labelAdditions.Text = "Additions";
			this.labelAdditions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labelSubtractions
			// 
			this.labelSubtractions.Location = new System.Drawing.Point(650, 17);
			this.labelSubtractions.Name = "labelSubtractions";
			this.labelSubtractions.Size = new System.Drawing.Size(182, 16);
			this.labelSubtractions.TabIndex = 167;
			this.labelSubtractions.Text = "Subtractions";
			this.labelSubtractions.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// listTypeNeg
			// 
			this.listTypeNeg.Location = new System.Drawing.Point(637, 37);
			this.listTypeNeg.Name = "listTypeNeg";
			this.listTypeNeg.Size = new System.Drawing.Size(206, 160);
			this.listTypeNeg.TabIndex = 165;
			this.listTypeNeg.SelectedIndexChanged += new System.EventHandler(this.listTypeNeg_SelectedIndexChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.listTypePos);
			this.groupBox3.Controls.Add(this.listTypeNeg);
			this.groupBox3.Controls.Add(this.dateAdjustment);
			this.groupBox3.Controls.Add(this.labelSubtractions);
			this.groupBox3.Controls.Add(this.labelAdjDate);
			this.groupBox3.Controls.Add(this.labelAdditions);
			this.groupBox3.Controls.Add(this.groupBox1);
			this.groupBox3.Controls.Add(this.groupBox2);
			this.groupBox3.Location = new System.Drawing.Point(7, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(853, 214);
			this.groupBox3.TabIndex = 168;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Adjustment Info";
			// 
			// radioIncludeAll
			// 
			this.radioIncludeAll.Checked = true;
			this.radioIncludeAll.Location = new System.Drawing.Point(21, 31);
			this.radioIncludeAll.Name = "radioIncludeAll";
			this.radioIncludeAll.Size = new System.Drawing.Size(137, 22);
			this.radioIncludeAll.TabIndex = 169;
			this.radioIncludeAll.TabStop = true;
			this.radioIncludeAll.Text = "Include all credits";
			this.radioIncludeAll.UseVisualStyleBackColor = true;
			this.radioIncludeAll.Click += new System.EventHandler(this.radioCredits_Click);
			// 
			// radioAllocatedOnly
			// 
			this.radioAllocatedOnly.Location = new System.Drawing.Point(21, 14);
			this.radioAllocatedOnly.Name = "radioAllocatedOnly";
			this.radioAllocatedOnly.Size = new System.Drawing.Size(137, 20);
			this.radioAllocatedOnly.TabIndex = 171;
			this.radioAllocatedOnly.Text = "Only allocated credits";
			this.radioAllocatedOnly.UseVisualStyleBackColor = true;
			this.radioAllocatedOnly.Click += new System.EventHandler(this.radioCredits_Click);
			// 
			// radioExcludeAll
			// 
			this.radioExcludeAll.Location = new System.Drawing.Point(21, 49);
			this.radioExcludeAll.Name = "radioExcludeAll";
			this.radioExcludeAll.Size = new System.Drawing.Size(136, 22);
			this.radioExcludeAll.TabIndex = 170;
			this.radioExcludeAll.Text = "Exclude all credits";
			this.radioExcludeAll.UseVisualStyleBackColor = true;
			this.radioExcludeAll.Click += new System.EventHandler(this.radioCredits_Click);
			// 
			// groupCreditLogic
			// 
			this.groupCreditLogic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupCreditLogic.Controls.Add(this.radioAllocatedOnly);
			this.groupCreditLogic.Controls.Add(this.radioIncludeAll);
			this.groupCreditLogic.Controls.Add(this.radioExcludeAll);
			this.groupCreditLogic.Location = new System.Drawing.Point(485, 525);
			this.groupCreditLogic.Name = "groupCreditLogic";
			this.groupCreditLogic.Size = new System.Drawing.Size(258, 76);
			this.groupCreditLogic.TabIndex = 172;
			this.groupCreditLogic.TabStop = false;
			this.groupCreditLogic.Text = "Credit Filter";
			// 
			// FormAdjMulti
			// 
			this.ClientSize = new System.Drawing.Size(873, 618);
			this.Controls.Add(this.groupCreditLogic);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.gridMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "FormAdjMulti";
			this.Text = "Add Multiple Adjustments";
			this.Load += new System.EventHandler(this.FormMultiAdj_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupCreditLogic.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelAdjDate;
		private UI.Button butPickProv;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ComboBoxOD comboProv;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.Button butAdd;
		private UI.Button butDelete;
		private ODtextBox textNote;
		private System.Windows.Forms.Label label5;
		private UI.Button butOk;
		private UI.Button butCancel;
		private ValidDate dateAdjustment;
		private System.Windows.Forms.RadioButton radioPercentRemBal;
		private ValidDouble textAmt;
		private System.Windows.Forms.RadioButton radioPercentProcFee;
		private System.Windows.Forms.Label labelAmount;
		private System.Windows.Forms.RadioButton radioFixedAmt;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.ListBoxOD listTypePos;
		private System.Windows.Forms.Label labelAdditions;
		private System.Windows.Forms.Label labelSubtractions;
		private OpenDental.UI.ListBoxOD listTypeNeg;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton radioIncludeAll;
		private System.Windows.Forms.RadioButton radioAllocatedOnly;
		private System.Windows.Forms.RadioButton radioExcludeAll;
		private System.Windows.Forms.GroupBox groupCreditLogic;
	}
}