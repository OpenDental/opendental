namespace OpenDental{
	partial class FormDunningSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDunningSetup));
			this.butAdd = new OpenDental.UI.Button();
			this.butDuplicate = new OpenDental.UI.Button();
			this.gridDunning = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioY = new System.Windows.Forms.RadioButton();
			this.radioN = new System.Windows.Forms.RadioButton();
			this.radioU = new System.Windows.Forms.RadioButton();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.labelDaysInAdvance = new System.Windows.Forms.Label();
			this.textAdv = new OpenDental.ValidNum();
			this.radio30 = new System.Windows.Forms.RadioButton();
			this.radio90 = new System.Windows.Forms.RadioButton();
			this.radio60 = new System.Windows.Forms.RadioButton();
			this.radioAny = new System.Windows.Forms.RadioButton();
			this.listBill = new OpenDental.UI.ListBoxOD();
			this.label8 = new System.Windows.Forms.Label();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(458, 384);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 18;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butDuplicate
			// 
			this.butDuplicate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDuplicate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butDuplicate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDuplicate.Location = new System.Drawing.Point(539, 384);
			this.butDuplicate.Name = "butDuplicate";
			this.butDuplicate.Size = new System.Drawing.Size(85, 24);
			this.butDuplicate.TabIndex = 17;
			this.butDuplicate.Text = "Duplicate";
			this.butDuplicate.Click += new System.EventHandler(this.butDuplicate_Click);
			// 
			// gridDunning
			// 
			this.gridDunning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridDunning.Location = new System.Drawing.Point(458, 12);
			this.gridDunning.Name = "gridDunning";
			this.gridDunning.Size = new System.Drawing.Size(641, 366);
			this.gridDunning.TabIndex = 15;
			this.gridDunning.Title = "Messages";
			this.gridDunning.TranslationName = "TableMessages";
			this.gridDunning.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridDunning_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1024, 384);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 19;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.listBill);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.comboClinics);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(440, 266);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Filters";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.groupBox2.Controls.Add(this.radioY);
			this.groupBox2.Controls.Add(this.radioN);
			this.groupBox2.Controls.Add(this.radioU);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(170, 159);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(261, 87);
			this.groupBox2.TabIndex = 260;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Insurance Payment Pending";
			// 
			// radioY
			// 
			this.radioY.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radioY.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioY.Location = new System.Drawing.Point(12, 39);
			this.radioY.Name = "radioY";
			this.radioY.Size = new System.Drawing.Size(114, 18);
			this.radioY.TabIndex = 1;
			this.radioY.TabStop = true;
			this.radioY.Text = "Yes";
			this.radioY.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// radioN
			// 
			this.radioN.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radioN.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioN.Location = new System.Drawing.Point(12, 61);
			this.radioN.Name = "radioN";
			this.radioN.Size = new System.Drawing.Size(114, 18);
			this.radioN.TabIndex = 2;
			this.radioN.TabStop = true;
			this.radioN.Text = "No";
			this.radioN.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// radioU
			// 
			this.radioU.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radioU.Checked = true;
			this.radioU.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioU.Location = new System.Drawing.Point(12, 17);
			this.radioU.Name = "radioU";
			this.radioU.Size = new System.Drawing.Size(114, 18);
			this.radioU.TabIndex = 0;
			this.radioU.TabStop = true;
			this.radioU.Text = "Doesn\'t Matter";
			this.radioU.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.groupBox3.Controls.Add(this.labelDaysInAdvance);
			this.groupBox3.Controls.Add(this.textAdv);
			this.groupBox3.Controls.Add(this.radio30);
			this.groupBox3.Controls.Add(this.radio90);
			this.groupBox3.Controls.Add(this.radio60);
			this.groupBox3.Controls.Add(this.radioAny);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(170, 41);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(261, 110);
			this.groupBox3.TabIndex = 259;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Age of Account";
			// 
			// labelDaysInAdvance
			// 
			this.labelDaysInAdvance.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.labelDaysInAdvance.Location = new System.Drawing.Point(132, 85);
			this.labelDaysInAdvance.Name = "labelDaysInAdvance";
			this.labelDaysInAdvance.Size = new System.Drawing.Size(88, 18);
			this.labelDaysInAdvance.TabIndex = 121;
			this.labelDaysInAdvance.Text = "Days in Adv";
			this.labelDaysInAdvance.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAdv
			// 
			this.textAdv.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.textAdv.Location = new System.Drawing.Point(221, 84);
			this.textAdv.MaxVal = 2147483647;
			this.textAdv.MinVal = 0;
			this.textAdv.Name = "textAdv";
			this.textAdv.Size = new System.Drawing.Size(34, 20);
			this.textAdv.TabIndex = 4;
			this.textAdv.TextChanged += new System.EventHandler(this.OnFilterChanged);
			this.textAdv.ShowZero = false;
			// 
			// radio30
			// 
			this.radio30.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radio30.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio30.Location = new System.Drawing.Point(12, 41);
			this.radio30.Name = "radio30";
			this.radio30.Size = new System.Drawing.Size(114, 18);
			this.radio30.TabIndex = 1;
			this.radio30.Text = "Over 30 Days";
			this.radio30.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// radio90
			// 
			this.radio90.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radio90.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio90.Location = new System.Drawing.Point(12, 85);
			this.radio90.Name = "radio90";
			this.radio90.Size = new System.Drawing.Size(114, 18);
			this.radio90.TabIndex = 3;
			this.radio90.Text = "Over 90 Days";
			this.radio90.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// radio60
			// 
			this.radio60.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radio60.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio60.Location = new System.Drawing.Point(12, 63);
			this.radio60.Name = "radio60";
			this.radio60.Size = new System.Drawing.Size(114, 18);
			this.radio60.TabIndex = 2;
			this.radio60.Text = "Over 60 Days";
			this.radio60.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// radioAny
			// 
			this.radioAny.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.radioAny.Checked = true;
			this.radioAny.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAny.Location = new System.Drawing.Point(12, 19);
			this.radioAny.Name = "radioAny";
			this.radioAny.Size = new System.Drawing.Size(114, 18);
			this.radioAny.TabIndex = 0;
			this.radioAny.TabStop = true;
			this.radioAny.Text = "Any Balance";
			this.radioAny.CheckedChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// listBill
			// 
			this.listBill.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.listBill.Location = new System.Drawing.Point(6, 47);
			this.listBill.Name = "listBill";
			this.listBill.SelectionMode = UI.SelectionMode.MultiExtended;
			this.listBill.Size = new System.Drawing.Size(158, 199);
			this.listBill.TabIndex = 257;
			this.listBill.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// label8
			// 
			this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.label8.Location = new System.Drawing.Point(6, 29);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(158, 16);
			this.label8.TabIndex = 258;
			this.label8.Text = "Billing Type:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboClinics
			// 
			this.comboClinics.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(228, 16);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(180, 21);
			this.comboClinics.TabIndex = 255;
			this.comboClinics.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
			// 
			// FormDunningSetup
			// 
			this.ClientSize = new System.Drawing.Size(1111, 420);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butDuplicate);
			this.Controls.Add(this.gridDunning);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDunningSetup";
			this.Text = "Dunning Msg Setup";
			this.Load += new System.EventHandler(this.FormDunningSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.Button butAdd;
		private UI.Button butDuplicate;
		private UI.GridOD gridDunning;
		private UI.Button butClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioY;
		private System.Windows.Forms.RadioButton radioN;
		private System.Windows.Forms.RadioButton radioU;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label labelDaysInAdvance;
		private ValidNum textAdv;
		private System.Windows.Forms.RadioButton radio30;
		private System.Windows.Forms.RadioButton radio90;
		private System.Windows.Forms.RadioButton radio60;
		private System.Windows.Forms.RadioButton radioAny;
		private OpenDental.UI.ListBoxOD listBill;
		private System.Windows.Forms.Label label8;
		private UI.ComboBoxClinicPicker comboClinics;
	}
}