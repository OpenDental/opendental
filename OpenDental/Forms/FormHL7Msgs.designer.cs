namespace OpenDental{
	partial class FormHL7Msgs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHL7Msgs));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butCurrent = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.butFind = new OpenDental.UI.Button();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.labelHL7Status = new System.Windows.Forms.Label();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.comboHL7Status = new System.Windows.Forms.ComboBox();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butCurrent);
			this.groupBox1.Controls.Add(this.butAll);
			this.groupBox1.Controls.Add(this.butFind);
			this.groupBox1.Controls.Add(this.textDateEnd);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.labelHL7Status);
			this.groupBox1.Controls.Add(this.labelEndDate);
			this.groupBox1.Controls.Add(this.textPatient);
			this.groupBox1.Controls.Add(this.labelPatient);
			this.groupBox1.Controls.Add(this.comboHL7Status);
			this.groupBox1.Controls.Add(this.labelStartDate);
			this.groupBox1.Controls.Add(this.butRefresh);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(21, 6);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(816, 62);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "View";
			// 
			// butCurrent
			// 
			this.butCurrent.Location = new System.Drawing.Point(266, 34);
			this.butCurrent.Name = "butCurrent";
			this.butCurrent.Size = new System.Drawing.Size(64, 24);
			this.butCurrent.TabIndex = 2;
			this.butCurrent.Text = "Current";
			this.butCurrent.Click += new System.EventHandler(this.butCurrent_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(418, 34);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(64, 24);
			this.butAll.TabIndex = 4;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// butFind
			// 
			this.butFind.Location = new System.Drawing.Point(342, 34);
			this.butFind.Name = "butFind";
			this.butFind.Size = new System.Drawing.Size(64, 24);
			this.butFind.TabIndex = 3;
			this.butFind.Text = "Find";
			this.butFind.Click += new System.EventHandler(this.butFind_Click);
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(102, 35);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 1;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(102, 12);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 0;
			// 
			// labelHL7Status
			// 
			this.labelHL7Status.Location = new System.Drawing.Point(487, 12);
			this.labelHL7Status.Name = "labelHL7Status";
			this.labelHL7Status.Size = new System.Drawing.Size(80, 18);
			this.labelHL7Status.TabIndex = 8;
			this.labelHL7Status.Text = "HL7Status";
			this.labelHL7Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(19, 35);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(80, 18);
			this.labelEndDate.TabIndex = 8;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatient
			// 
			this.textPatient.BackColor = System.Drawing.SystemColors.Window;
			this.textPatient.Location = new System.Drawing.Point(266, 12);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(216, 20);
			this.textPatient.TabIndex = 8;
			this.textPatient.TabStop = false;
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(184, 12);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(80, 18);
			this.labelPatient.TabIndex = 8;
			this.labelPatient.Text = "Patient";
			this.labelPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboHL7Status
			// 
			this.comboHL7Status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboHL7Status.Location = new System.Drawing.Point(569, 12);
			this.comboHL7Status.MaxDropDownItems = 40;
			this.comboHL7Status.Name = "comboHL7Status";
			this.comboHL7Status.Size = new System.Drawing.Size(155, 21);
			this.comboHL7Status.TabIndex = 5;
			this.comboHL7Status.SelectedIndexChanged += new System.EventHandler(this.comboHL7Status_SelectedIndexChanged);
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(19, 12);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(80, 18);
			this.labelStartDate.TabIndex = 8;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(736, 10);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 6;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(21, 74);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(902, 543);
			this.gridMain.TabIndex = 8;
			this.gridMain.TabStop = false;
			this.gridMain.Title = "HL7 Message Log";
			this.gridMain.TranslationName = "TableMessageLog";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(848, 623);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 7;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormHL7Msgs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(939, 663);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormHL7Msgs";
			this.Text = "HL7 Messages";
			this.Load += new System.EventHandler(this.FormHL7Msgs_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label labelPatient;
		private System.Windows.Forms.ComboBox comboHL7Status;
		private System.Windows.Forms.Label labelHL7Status;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label labelEndDate;
		private System.Windows.Forms.Label labelStartDate;
		private UI.Button butRefresh;
		private System.Windows.Forms.TextBox textPatient;
		private UI.Button butFind;
		private UI.Button butAll;
		private UI.Button butCurrent;
	}
}