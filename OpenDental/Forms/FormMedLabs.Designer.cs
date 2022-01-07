namespace OpenDental {
	partial class FormMedLabs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedLabs));
			this.checkIncludeNoPat = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.checkOnlyNoPat = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butCurrent = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.butClose = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkIncludeNoPat
			// 
			this.checkIncludeNoPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeNoPat.Location = new System.Drawing.Point(567, 14);
			this.checkIncludeNoPat.Name = "checkIncludeNoPat";
			this.checkIncludeNoPat.Size = new System.Drawing.Size(150, 17);
			this.checkIncludeNoPat.TabIndex = 7;
			this.checkIncludeNoPat.Text = "Include unattached labs";
			this.checkIncludeNoPat.UseVisualStyleBackColor = true;
			this.checkIncludeNoPat.Click += new System.EventHandler(this.checkIncludeNoPat_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.comboClinic);
			this.groupBox1.Controls.Add(this.labelClinic);
			this.groupBox1.Controls.Add(this.checkOnlyNoPat);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.butCurrent);
			this.groupBox1.Controls.Add(this.checkIncludeNoPat);
			this.groupBox1.Controls.Add(this.butRefresh);
			this.groupBox1.Controls.Add(this.butAll);
			this.groupBox1.Controls.Add(this.butFind);
			this.groupBox1.Controls.Add(this.textDateEnd);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.labelEndDate);
			this.groupBox1.Controls.Add(this.textPatient);
			this.groupBox1.Controls.Add(this.labelPatient);
			this.groupBox1.Controls.Add(this.labelStartDate);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(12, 7);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(950, 68);
			this.groupBox1.TabIndex = 337;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(784, 40);
			this.comboClinic.MaxDropDownItems = 30;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(160, 21);
			this.comboClinic.TabIndex = 76;
			this.comboClinic.SelectedIndexChanged += new System.EventHandler(this.comboClinic_SelectedIndexChanged);
			// 
			// labelClinic
			// 
			this.labelClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelClinic.Location = new System.Drawing.Point(723, 42);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(60, 17);
			this.labelClinic.TabIndex = 77;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkOnlyNoPat
			// 
			this.checkOnlyNoPat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOnlyNoPat.Location = new System.Drawing.Point(567, 40);
			this.checkOnlyNoPat.Name = "checkOnlyNoPat";
			this.checkOnlyNoPat.Size = new System.Drawing.Size(150, 17);
			this.checkOnlyNoPat.TabIndex = 11;
			this.checkOnlyNoPat.Text = "Only unattached labs";
			this.checkOnlyNoPat.UseVisualStyleBackColor = true;
			this.checkOnlyNoPat.Click += new System.EventHandler(this.checkOnlyNoPat_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(151, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(127, 29);
			this.label1.TabIndex = 10;
			this.label1.Text = "Filtered by most recent date and time reported.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(356, 37);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(64, 24);
			this.butCurrent.TabIndex = 4;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(880, 12);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(64, 24);
			this.butRefresh.TabIndex = 9;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(492, 37);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(64, 24);
			this.butAll.TabIndex = 6;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(424, 37);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(64, 24);
			this.butFind.TabIndex = 5;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(71, 40);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 2;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(71, 14);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 1;
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(6, 41);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(64, 17);
			this.labelEndDate.TabIndex = 8;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.BackColor = System.Drawing.SystemColors.Window;
			this.textPatient.Location = new System.Drawing.Point(356, 13);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(200, 20);
			this.textPatient.TabIndex = 3;
			this.textPatient.TabStop = false;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(296, 14);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(59, 17);
			this.labelPatient.TabIndex = 8;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(6, 15);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(64, 17);
			this.labelStartDate.TabIndex = 8;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(887, 526);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 80);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(950, 436);
			this.gridMain.TabIndex = 5;
			this.gridMain.Title = "Labs";
			this.gridMain.TranslationName = "TableLabs";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// FormMedLabs
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(974, 561);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMedLabs";
			this.Text = "Medical Labs";
			this.Load += new System.EventHandler(this.FormMedLabs_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.GridOD gridMain;
		private UI.Button butClose;
		private System.Windows.Forms.CheckBox checkIncludeNoPat;
		private System.Windows.Forms.GroupBox groupBox1;
		private UI.Button butCurrent;
		private UI.Button butAll;
		private UI.Button butFind;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label labelEndDate;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.Label labelStartDate;
		private UI.Button butRefresh;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkOnlyNoPat;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
	}
}