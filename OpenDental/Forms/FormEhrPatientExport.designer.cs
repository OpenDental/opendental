namespace OpenDental{
	partial class FormEhrPatientExport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatientExport));
			this.comboProv = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.labelLName = new System.Windows.Forms.Label();
			this.labelFName = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.textLName = new System.Windows.Forms.TextBox();
			this.textFName = new System.Windows.Forms.TextBox();
			this.butSearch = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butSelectAll = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// comboProv
			// 
			this.comboProv.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProv.Location = new System.Drawing.Point(348, 12);
			this.comboProv.MaxDropDownItems = 40;
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(160, 21);
			this.comboProv.TabIndex = 23;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(256, 12);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 21);
			this.label4.TabIndex = 22;
			this.label4.Text = "Primary Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(348, 54);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(160, 21);
			this.comboSite.TabIndex = 29;
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(276, 54);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(70, 21);
			this.labelSite.TabIndex = 28;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClinic.Location = new System.Drawing.Point(348, 33);
			this.comboClinic.MaxDropDownItems = 40;
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(160, 21);
			this.comboClinic.TabIndex = 27;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(276, 33);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(70, 21);
			this.labelClinic.TabIndex = 26;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelLName
			// 
			this.labelLName.Location = new System.Drawing.Point(15, 34);
			this.labelLName.Name = "labelLName";
			this.labelLName.Size = new System.Drawing.Size(70, 21);
			this.labelLName.TabIndex = 37;
			this.labelLName.Text = "Last Name";
			this.labelLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFName
			// 
			this.labelFName.Location = new System.Drawing.Point(15, 12);
			this.labelFName.Name = "labelFName";
			this.labelFName.Size = new System.Drawing.Size(70, 21);
			this.labelFName.TabIndex = 35;
			this.labelFName.Text = "First Name";
			this.labelFName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(18, 86);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(590, 365);
			this.gridMain.TabIndex = 10;
			this.gridMain.Title = "Patient Export List";
			this.gridMain.TranslationName = "TablePatientExport";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(70, 19);
			this.label1.TabIndex = 40;
			this.label1.Text = "Patnum";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(87, 56);
			this.textPatNum.MaxLength = 2147483647;
			this.textPatNum.Multiline = true;
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(160, 21);
			this.textPatNum.TabIndex = 6;
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(87, 34);
			this.textLName.MaxLength = 2147483647;
			this.textLName.Multiline = true;
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(160, 21);
			this.textLName.TabIndex = 5;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(87, 12);
			this.textFName.MaxLength = 2147483647;
			this.textFName.Multiline = true;
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(160, 21);
			this.textFName.TabIndex = 4;
			// 
			// butSearch
			// 
			this.butSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearch.Location = new System.Drawing.Point(531, 10);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 23);
			this.butSearch.TabIndex = 33;
			this.butSearch.Text = "&Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.Location = new System.Drawing.Point(18, 462);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(100, 24);
			this.butExport.TabIndex = 30;
			this.butExport.Text = "Export Selected";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butSelectAll
			// 
			this.butSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSelectAll.Location = new System.Drawing.Point(276, 462);
			this.butSelectAll.Name = "butSelectAll";
			this.butSelectAll.Size = new System.Drawing.Size(75, 24);
			this.butSelectAll.TabIndex = 33;
			this.butSelectAll.Text = "Select All";
			this.butSelectAll.Click += new System.EventHandler(this.butSelectAll_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(531, 462);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormEhrPatientExport
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(626, 498);
			this.Controls.Add(this.textFName);
			this.Controls.Add(this.textLName);
			this.Controls.Add(this.textPatNum);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.butExport);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.labelLName);
			this.Controls.Add(this.labelSite);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelFName);
			this.Controls.Add(this.butSelectAll);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.comboProv);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatientExport";
			this.Text = "EHR Patient Export";
			this.Load += new System.EventHandler(this.FormEhrPatientExport_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.ComboBox comboProv;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboSite;
		private System.Windows.Forms.Label labelSite;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private UI.Button butExport;
		private UI.Button butSelectAll;
		private UI.Button butSearch;
		private System.Windows.Forms.Label labelLName;
		private System.Windows.Forms.Label labelFName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
	}
}