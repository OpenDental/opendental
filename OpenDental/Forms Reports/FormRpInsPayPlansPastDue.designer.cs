namespace OpenDental{
	partial class FormRpInsPayPlansPastDue {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpInsPayPlansPastDue));
			this.butClose = new OpenDental.UI.Button();
			this.labelProv = new System.Windows.Forms.Label();
			this.comboProvs = new OpenDental.UI.ComboBoxOD();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.textDaysPastDue = new System.Windows.Forms.TextBox();
			this.butExport = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(758, 463);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelProv
			// 
			this.labelProv.Location = new System.Drawing.Point(135, 20);
			this.labelProv.Name = "labelProv";
			this.labelProv.Size = new System.Drawing.Size(70, 16);
			this.labelProv.TabIndex = 61;
			this.labelProv.Text = "Provs:";
			this.labelProv.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboProvs
			// 
			this.comboProvs.BackColor = System.Drawing.SystemColors.Window;
			this.comboProvs.Location = new System.Drawing.Point(206, 18);
			this.comboProvs.Name = "comboProvs";
			this.comboProvs.SelectionModeMulti = true;
			this.comboProvs.Size = new System.Drawing.Size(160, 21);
			this.comboProvs.TabIndex = 62;
			this.comboProvs.SelectionChangeCommitted += new System.EventHandler(this.ComboProvs_SelectionChangeCommitted);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(8, 40);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(825, 417);
			this.gridMain.TabIndex = 65;
			this.gridMain.Title = "Ins Pay Plans Past Due";
			this.gridMain.TranslationName = "TableInsPayPlanPastDue";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-2, 1);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(87, 34);
			this.label2.TabIndex = 66;
			this.label2.Text = "Days past due:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textDaysPastDue
			// 
			this.textDaysPastDue.Location = new System.Drawing.Point(83, 19);
			this.textDaysPastDue.Name = "textDaysPastDue";
			this.textDaysPastDue.Size = new System.Drawing.Size(52, 20);
			this.textDaysPastDue.TabIndex = 69;
			this.textDaysPastDue.Text = "30";
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.Image = global::OpenDental.Properties.Resources.butExport;
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(93, 463);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(79, 24);
			this.butExport.TabIndex = 71;
			this.butExport.Text = "&Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(8, 463);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 70;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(416, 18);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(200, 21);
			this.comboClinics.TabIndex = 72;
			this.comboClinics.SelectionChangeCommitted += new System.EventHandler(this.ComboClinics_SelectionChangeCommitted);
			// 
			// FormRpInsPayPlansPastDue
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(845, 499);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.textDaysPastDue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelProv);
			this.Controls.Add(this.comboProvs);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpInsPayPlansPastDue";
			this.Text = "Insurance Payment Plans Past Due";
			this.Load += new System.EventHandler(this.FormRpInsPayPlansPastDue_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label labelProv;
		private UI.ComboBoxOD comboProvs;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDaysPastDue;
		private UI.Button butExport;
		private UI.Button butPrint;
		private UI.ComboBoxClinicPicker comboClinics;
	}
}