namespace OpenDental{
	partial class FormReference {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReference));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.menuRightReferences = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.goToAccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textCountry = new System.Windows.Forms.TextBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textSuperFamily = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textSpecialty = new System.Windows.Forms.TextBox();
			this.textAge = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textAreaCode = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textLName = new System.Windows.Forms.TextBox();
			this.textZip = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkBadRefs = new System.Windows.Forms.CheckBox();
			this.groupFilter = new System.Windows.Forms.GroupBox();
			this.listBillingType = new OpenDental.UI.ListBoxOD();
			this.label10 = new System.Windows.Forms.Label();
			this.checkUsedRefs = new System.Windows.Forms.CheckBox();
			this.checkGuarOnly = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkRefresh = new System.Windows.Forms.CheckBox();
			this.butGetAll = new OpenDental.UI.Button();
			this.butSearch = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.menuRightReferences.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupFilter.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(983, 633);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1064, 633);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.ContextMenuStrip = this.menuRightReferences;
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(847, 642);
			this.gridMain.TabIndex = 0;
			this.gridMain.TabStop = false;
			this.gridMain.Title = "Select  References";
			this.gridMain.TranslationName = "FormPatientSelect";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gridMain_KeyDown);
			// 
			// menuRightReferences
			// 
			this.menuRightReferences.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.goToAccountToolStripMenuItem});
			this.menuRightReferences.Name = "menuRightReferences";
			this.menuRightReferences.Size = new System.Drawing.Size(142, 26);
			// 
			// goToAccountToolStripMenuItem
			// 
			this.goToAccountToolStripMenuItem.Name = "goToAccountToolStripMenuItem";
			this.goToAccountToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
			this.goToAccountToolStripMenuItem.Text = "Go to Family";
			this.goToAccountToolStripMenuItem.Click += new System.EventHandler(this.goToFamilyToolStripMenuItem_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.textCountry);
			this.groupBox2.Controls.Add(this.labelCountry);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textSuperFamily);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textSpecialty);
			this.groupBox2.Controls.Add(this.textAge);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.textAreaCode);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textLName);
			this.groupBox2.Controls.Add(this.textZip);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.textFName);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.textState);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textCity);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(870, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(269, 236);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Search by:";
			// 
			// textCountry
			// 
			this.textCountry.Location = new System.Drawing.Point(163, 70);
			this.textCountry.Name = "textCountry";
			this.textCountry.Size = new System.Drawing.Size(90, 20);
			this.textCountry.TabIndex = 4;
			this.textCountry.TextChanged += new System.EventHandler(this.textCountry_TextChanged);
			// 
			// labelCountry
			// 
			this.labelCountry.Location = new System.Drawing.Point(65, 70);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(99, 20);
			this.labelCountry.TabIndex = 0;
			this.labelCountry.Text = "Country";
			this.labelCountry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 130);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(154, 20);
			this.label6.TabIndex = 0;
			this.label6.Text = "# (or more) in Super Family";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSuperFamily
			// 
			this.textSuperFamily.Location = new System.Drawing.Point(163, 130);
			this.textSuperFamily.Name = "textSuperFamily";
			this.textSuperFamily.Size = new System.Drawing.Size(90, 20);
			this.textSuperFamily.TabIndex = 7;
			this.textSuperFamily.TextChanged += new System.EventHandler(this.textSuperFamily_TextChanged);
			this.textSuperFamily.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textSuperFamily_KeyDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(59, 110);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(103, 20);
			this.label5.TabIndex = 0;
			this.label5.Text = "Specialty";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSpecialty
			// 
			this.textSpecialty.Location = new System.Drawing.Point(163, 110);
			this.textSpecialty.Name = "textSpecialty";
			this.textSpecialty.Size = new System.Drawing.Size(90, 20);
			this.textSpecialty.TabIndex = 6;
			this.textSpecialty.TextChanged += new System.EventHandler(this.textSpecialty_TextChanged);
			this.textSpecialty.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textSpecialty_KeyDown);
			// 
			// textAge
			// 
			this.textAge.Location = new System.Drawing.Point(163, 210);
			this.textAge.Name = "textAge";
			this.textAge.Size = new System.Drawing.Size(90, 20);
			this.textAge.TabIndex = 11;
			this.textAge.TextChanged += new System.EventHandler(this.textAge_TextChanged);
			this.textAge.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textAge_KeyDown);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(65, 210);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "Age At Least";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAreaCode
			// 
			this.textAreaCode.Location = new System.Drawing.Point(163, 90);
			this.textAreaCode.Name = "textAreaCode";
			this.textAreaCode.Size = new System.Drawing.Size(90, 20);
			this.textAreaCode.TabIndex = 5;
			this.textAreaCode.TextChanged += new System.EventHandler(this.textAreaCode_TextChanged);
			this.textAreaCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textAreaCode_KeyDown);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(34, 90);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(129, 20);
			this.label4.TabIndex = 0;
			this.label4.Text = "Area Code";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(59, 150);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(163, 150);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(90, 20);
			this.textLName.TabIndex = 8;
			this.textLName.TextChanged += new System.EventHandler(this.textLName_TextChanged);
			this.textLName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textLName_KeyDown);
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(163, 50);
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(90, 20);
			this.textZip.TabIndex = 3;
			this.textZip.TextChanged += new System.EventHandler(this.textZip_TextChanged);
			this.textZip.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textZip_KeyDown);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(62, 170);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 0;
			this.label3.Text = "First Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(62, 50);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(101, 20);
			this.label12.TabIndex = 0;
			this.label12.Text = "Zip";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(163, 170);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(90, 20);
			this.textFName.TabIndex = 9;
			this.textFName.TextChanged += new System.EventHandler(this.textFName_TextChanged);
			this.textFName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textFName_KeyDown);
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(163, 190);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(90, 20);
			this.textPatNum.TabIndex = 10;
			this.textPatNum.TextChanged += new System.EventHandler(this.textPatNum_TextChanged);
			this.textPatNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textPatNum_KeyDown);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(63, 190);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(101, 20);
			this.label9.TabIndex = 0;
			this.label9.Text = "Patient Number";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(163, 30);
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(90, 20);
			this.textState.TabIndex = 2;
			this.textState.TextChanged += new System.EventHandler(this.textState_TextChanged);
			this.textState.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textState_KeyDown);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(62, 30);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 20);
			this.label8.TabIndex = 0;
			this.label8.Text = "State";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(163, 10);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(90, 20);
			this.textCity.TabIndex = 1;
			this.textCity.TextChanged += new System.EventHandler(this.textCity_TextChanged);
			this.textCity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textCity_KeyDown);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(62, 10);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(98, 20);
			this.label7.TabIndex = 0;
			this.label7.Text = "City";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkBadRefs
			// 
			this.checkBadRefs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBadRefs.Location = new System.Drawing.Point(11, 240);
			this.checkBadRefs.Name = "checkBadRefs";
			this.checkBadRefs.Size = new System.Drawing.Size(187, 17);
			this.checkBadRefs.TabIndex = 2;
			this.checkBadRefs.Text = "Include bad references";
			this.checkBadRefs.Click += new System.EventHandler(this.checkBadRefs_Click);
			// 
			// groupFilter
			// 
			this.groupFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupFilter.Controls.Add(this.listBillingType);
			this.groupFilter.Controls.Add(this.label10);
			this.groupFilter.Controls.Add(this.checkUsedRefs);
			this.groupFilter.Controls.Add(this.checkGuarOnly);
			this.groupFilter.Controls.Add(this.checkBadRefs);
			this.groupFilter.Location = new System.Drawing.Point(870, 239);
			this.groupFilter.Name = "groupFilter";
			this.groupFilter.Size = new System.Drawing.Size(269, 283);
			this.groupFilter.TabIndex = 2;
			this.groupFilter.TabStop = false;
			this.groupFilter.Text = "Filters:";
			// 
			// listBillingType
			// 
			this.listBillingType.Location = new System.Drawing.Point(11, 32);
			this.listBillingType.Name = "listBillingType";
			this.listBillingType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillingType.Size = new System.Drawing.Size(252, 186);
			this.listBillingType.TabIndex = 0;
			this.listBillingType.TabStop = false;
			this.listBillingType.SelectionChangeCommitted += new System.EventHandler(this.listBillingType_SelectionChangeCommitted);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 15);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(121, 16);
			this.label10.TabIndex = 59;
			this.label10.Text = "Billing Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkUsedRefs
			// 
			this.checkUsedRefs.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUsedRefs.Location = new System.Drawing.Point(11, 224);
			this.checkUsedRefs.Name = "checkUsedRefs";
			this.checkUsedRefs.Size = new System.Drawing.Size(187, 17);
			this.checkUsedRefs.TabIndex = 1;
			this.checkUsedRefs.Text = "Used references only";
			this.checkUsedRefs.Click += new System.EventHandler(this.checkUsedRefs_Click);
			// 
			// checkGuarOnly
			// 
			this.checkGuarOnly.Checked = true;
			this.checkGuarOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkGuarOnly.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGuarOnly.Location = new System.Drawing.Point(11, 257);
			this.checkGuarOnly.Name = "checkGuarOnly";
			this.checkGuarOnly.Size = new System.Drawing.Size(187, 17);
			this.checkGuarOnly.TabIndex = 3;
			this.checkGuarOnly.Text = "Guarantors only";
			this.checkGuarOnly.Click += new System.EventHandler(this.checkBadRefs_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkRefresh);
			this.groupBox1.Controls.Add(this.butGetAll);
			this.groupBox1.Controls.Add(this.butSearch);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(870, 523);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(269, 69);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// checkRefresh
			// 
			this.checkRefresh.Checked = true;
			this.checkRefresh.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkRefresh.Location = new System.Drawing.Point(43, 47);
			this.checkRefresh.Name = "checkRefresh";
			this.checkRefresh.Size = new System.Drawing.Size(195, 18);
			this.checkRefresh.TabIndex = 3;
			this.checkRefresh.Text = "Refresh while typing";
			this.checkRefresh.UseVisualStyleBackColor = true;
			// 
			// butGetAll
			// 
			this.butGetAll.Location = new System.Drawing.Point(163, 18);
			this.butGetAll.Name = "butGetAll";
			this.butGetAll.Size = new System.Drawing.Size(75, 23);
			this.butGetAll.TabIndex = 2;
			this.butGetAll.Text = "Get All";
			this.butGetAll.Click += new System.EventHandler(this.butGetAll_Click);
			// 
			// butSearch
			// 
			this.butSearch.Location = new System.Drawing.Point(43, 18);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 23);
			this.butSearch.TabIndex = 1;
			this.butSearch.Text = "&Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label11.Location = new System.Drawing.Point(913, 598);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(146, 39);
			this.label11.TabIndex = 0;
			this.label11.Text = "(use selected as reference for this customer)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormReference
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1151, 666);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupFilter);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReference";
			this.Text = " Reference Select";
			this.Load += new System.EventHandler(this.FormReference_Load);
			this.TextChanged += new System.EventHandler(this.textCountry_TextChanged);
			this.menuRightReferences.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupFilter.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textAge;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textAreaCode;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBadRefs;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textSuperFamily;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textSpecialty;
		private System.Windows.Forms.GroupBox groupFilter;
		private System.Windows.Forms.CheckBox checkUsedRefs;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox checkRefresh;
		private UI.Button butGetAll;
		private UI.Button butSearch;
		private System.Windows.Forms.ContextMenuStrip menuRightReferences;
		private System.Windows.Forms.ToolStripMenuItem goToAccountToolStripMenuItem;
		private System.Windows.Forms.CheckBox checkGuarOnly;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private OpenDental.UI.ListBoxOD listBillingType;
		private System.Windows.Forms.TextBox textCountry;
		private System.Windows.Forms.Label labelCountry;
	}
}