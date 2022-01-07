namespace OpenDental{
	partial class FormXDRSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXDRSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelLocationID = new System.Windows.Forms.Label();
			this.textLocationID = new System.Windows.Forms.TextBox();
			this.comboClinic = new System.Windows.Forms.ComboBox();
			this.labelClinic = new System.Windows.Forms.Label();
			this.textOverride = new System.Windows.Forms.TextBox();
			this.labelOverride = new System.Windows.Forms.Label();
			this.textPath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkEnabled = new System.Windows.Forms.CheckBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.butClear = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.textButtonText = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.listToolBars = new OpenDental.UI.ListBoxOD();
			this.labelInfoFile = new System.Windows.Forms.Label();
			this.textInfoFile = new System.Windows.Forms.TextBox();
			this.radioPatient = new System.Windows.Forms.RadioButton();
			this.radioChart = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(397, 441);
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
			this.butCancel.Location = new System.Drawing.Point(397, 471);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelLocationID
			// 
			this.labelLocationID.Location = new System.Drawing.Point(7, 103);
			this.labelLocationID.Name = "labelLocationID";
			this.labelLocationID.Size = new System.Drawing.Size(143, 18);
			this.labelLocationID.TabIndex = 120;
			this.labelLocationID.Text = "Location ID";
			this.labelLocationID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLocationID
			// 
			this.textLocationID.Location = new System.Drawing.Point(152, 102);
			this.textLocationID.Name = "textLocationID";
			this.textLocationID.Size = new System.Drawing.Size(275, 20);
			this.textLocationID.TabIndex = 119;
			// 
			// comboClinic
			// 
			this.comboClinic.FormattingEnabled = true;
			this.comboClinic.Location = new System.Drawing.Point(152, 128);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(158, 21);
			this.comboClinic.TabIndex = 122;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(10, 129);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(140, 18);
			this.labelClinic.TabIndex = 121;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOverride
			// 
			this.textOverride.Location = new System.Drawing.Point(152, 50);
			this.textOverride.Name = "textOverride";
			this.textOverride.Size = new System.Drawing.Size(275, 20);
			this.textOverride.TabIndex = 133;
			// 
			// labelOverride
			// 
			this.labelOverride.Location = new System.Drawing.Point(1, 52);
			this.labelOverride.Name = "labelOverride";
			this.labelOverride.Size = new System.Drawing.Size(149, 18);
			this.labelOverride.TabIndex = 132;
			this.labelOverride.Text = "Local path override";
			this.labelOverride.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPath
			// 
			this.textPath.Location = new System.Drawing.Point(152, 25);
			this.textPath.Name = "textPath";
			this.textPath.Size = new System.Drawing.Size(275, 20);
			this.textPath.TabIndex = 129;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(4, 27);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(146, 18);
			this.label3.TabIndex = 128;
			this.label3.Text = "Path of file to open";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkEnabled
			// 
			this.checkEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnabled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkEnabled.Location = new System.Drawing.Point(67, 4);
			this.checkEnabled.Name = "checkEnabled";
			this.checkEnabled.Size = new System.Drawing.Size(98, 18);
			this.checkEnabled.TabIndex = 123;
			this.checkEnabled.Text = "Enabled";
			this.checkEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(288, 412);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(22, 22);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBox.TabIndex = 141;
			this.pictureBox.TabStop = false;
			// 
			// butClear
			// 
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(235, 440);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 26);
			this.butClear.TabIndex = 140;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.ButClear_Click);
			// 
			// butImport
			// 
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(152, 439);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 26);
			this.butImport.TabIndex = 139;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.ButImport_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(143, 412);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(138, 22);
			this.label10.TabIndex = 138;
			this.label10.Text = "Button Image (22x22)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textButtonText
			// 
			this.textButtonText.Location = new System.Drawing.Point(152, 227);
			this.textButtonText.Name = "textButtonText";
			this.textButtonText.Size = new System.Drawing.Size(158, 20);
			this.textButtonText.TabIndex = 137;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(5, 228);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(146, 18);
			this.label7.TabIndex = 136;
			this.label7.Text = "Text on button";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(151, 256);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(223, 22);
			this.label6.TabIndex = 135;
			this.label6.Text = "Add a button to these toolbars";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// listToolBars
			// 
			this.listToolBars.Location = new System.Drawing.Point(152, 282);
			this.listToolBars.Name = "listToolBars";
			this.listToolBars.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listToolBars.Size = new System.Drawing.Size(158, 121);
			this.listToolBars.TabIndex = 134;
			// 
			// labelInfoFile
			// 
			this.labelInfoFile.Location = new System.Drawing.Point(7, 77);
			this.labelInfoFile.Name = "labelInfoFile";
			this.labelInfoFile.Size = new System.Drawing.Size(143, 18);
			this.labelInfoFile.TabIndex = 143;
			this.labelInfoFile.Text = "InfoFile path";
			this.labelInfoFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInfoFile
			// 
			this.textInfoFile.Location = new System.Drawing.Point(152, 76);
			this.textInfoFile.Name = "textInfoFile";
			this.textInfoFile.Size = new System.Drawing.Size(275, 20);
			this.textInfoFile.TabIndex = 142;
			// 
			// radioPatient
			// 
			this.radioPatient.Location = new System.Drawing.Point(12, 17);
			this.radioPatient.Name = "radioPatient";
			this.radioPatient.Size = new System.Drawing.Size(89, 24);
			this.radioPatient.TabIndex = 144;
			this.radioPatient.TabStop = true;
			this.radioPatient.Text = "PatientNum";
			this.radioPatient.UseVisualStyleBackColor = true;
			// 
			// radioChart
			// 
			this.radioChart.Location = new System.Drawing.Point(12, 37);
			this.radioChart.Name = "radioChart";
			this.radioChart.Size = new System.Drawing.Size(86, 24);
			this.radioChart.TabIndex = 145;
			this.radioChart.TabStop = true;
			this.radioChart.Text = "ChartNum";
			this.radioChart.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioPatient);
			this.groupBox1.Controls.Add(this.radioChart);
			this.groupBox1.Location = new System.Drawing.Point(152, 155);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(158, 66);
			this.groupBox1.TabIndex = 146;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "XDR Patient ID";
			// 
			// FormXDRSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(484, 507);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelInfoFile);
			this.Controls.Add(this.textInfoFile);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textButtonText);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.listToolBars);
			this.Controls.Add(this.textOverride);
			this.Controls.Add(this.labelOverride);
			this.Controls.Add(this.textPath);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkEnabled);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.labelLocationID);
			this.Controls.Add(this.textLocationID);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormXDRSetup";
			this.Text = "XDR Setup";
			this.Load += new System.EventHandler(this.FormXDRSetup_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelLocationID;
		private System.Windows.Forms.TextBox textLocationID;
		private System.Windows.Forms.ComboBox comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.TextBox textOverride;
		private System.Windows.Forms.Label labelOverride;
		private System.Windows.Forms.TextBox textPath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkEnabled;
		private System.Windows.Forms.PictureBox pictureBox;
		private UI.Button butClear;
		private UI.Button butImport;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textButtonText;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.ListBoxOD listToolBars;
		private System.Windows.Forms.Label labelInfoFile;
		private System.Windows.Forms.TextBox textInfoFile;
		private System.Windows.Forms.RadioButton radioPatient;
		private System.Windows.Forms.RadioButton radioChart;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}