namespace OpenDental{
	partial class FormFlows {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFlows));
			this.butClose = new OpenDental.UI.Button();
			this.gridFlows = new OpenDental.UI.GridOD();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.datePicker = new OpenDental.UI.ODDateRangePicker();
			this.menuSetup = new OpenDental.UI.MenuOD();
			this.butRefresh = new OpenDental.UI.Button();
			this.textBoxPatName = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.butSelectPatient = new OpenDental.UI.Button();
			this.butShowAll = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(708, 546);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// gridFlows
			// 
			this.gridFlows.Location = new System.Drawing.Point(26, 93);
			this.gridFlows.Name = "gridFlows";
			this.gridFlows.Size = new System.Drawing.Size(746, 433);
			this.gridFlows.TabIndex = 4;
			this.gridFlows.CellDoubleClick += new UI.ODGridClickEventHandler(gridFlowsCellDoubleClick);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "HQ";
			this.comboClinic.IncludeAll = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(466, 64);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(200, 21);
			this.comboClinic.TabIndex = 5;
			// 
			// datePicker
			// 
			this.datePicker.BackColor = System.Drawing.Color.Transparent;
			this.datePicker.Location = new System.Drawing.Point(26, 64);
			this.datePicker.Name = "datePicker";
			this.datePicker.Size = new System.Drawing.Size(453, 24);
			this.datePicker.TabIndex = 6;
			// 
			// menuSetup
			// 
			this.menuSetup.Location = new System.Drawing.Point(0, 0);
			this.menuSetup.Name = "menuSetup";
			this.menuSetup.Size = new System.Drawing.Size(750, 24);
			this.menuSetup.TabIndex = 7;
			this.menuSetup.Text = "menuOD1";
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(697, 61);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 8;
			this.butRefresh.Text = "&Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textBoxPatName
			// 
			this.textBoxPatName.Location = new System.Drawing.Point(91, 39);
			this.textBoxPatName.Name = "textBoxPatName";
			this.textBoxPatName.ReadOnly = true;
			this.textBoxPatName.Size = new System.Drawing.Size(165, 20);
			this.textBoxPatName.TabIndex = 9;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-9, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 11;
			this.label1.Text = "Patient";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSelectPatient
			// 
			this.butSelectPatient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSelectPatient.Location = new System.Drawing.Point(262, 39);
			this.butSelectPatient.Name = "butSelectPatient";
			this.butSelectPatient.Size = new System.Drawing.Size(20, 20);
			this.butSelectPatient.TabIndex = 12;
			this.butSelectPatient.Text = "...";
			this.butSelectPatient.Click += new System.EventHandler(this.butSelectPatient_Click);
			// 
			// butShowAll
			// 
			this.butShowAll.Location = new System.Drawing.Point(288, 39);
			this.butShowAll.Name = "butShowAll";
			this.butShowAll.Size = new System.Drawing.Size(82, 20);
			this.butShowAll.TabIndex = 13;
			this.butShowAll.Text = "Show All";
			this.butShowAll.Click += new System.EventHandler(this.butShowAll_Click);
			// 
			// FormFlows
			// 
			this.ClientSize = new System.Drawing.Size(795, 582);
			this.Controls.Add(this.butShowAll);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butSelectPatient);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxPatName);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.menuSetup);
			this.Controls.Add(this.datePicker);
			this.Controls.Add(this.gridFlows);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFlows";
			this.Text = "Flows";
			this.Load += new System.EventHandler(this.FormPatientFlows_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridFlows;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.ODDateRangePicker datePicker;
		private UI.MenuOD menuSetup;
		private UI.Button butRefresh;
		private System.Windows.Forms.TextBox textBoxPatName;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.Label label1;
		private UI.Button butSelectPatient;
		private UI.Button butShowAll;
	}
}