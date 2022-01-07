namespace OpenDental{
	partial class FormDoseSpotPropertyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDoseSpotPropertyEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textClinicID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textClinicKey = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textClinicAbbr = new System.Windows.Forms.TextBox();
			this.butRegisterClinic = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(150, 163);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(91, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(247, 163);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(91, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textClinicID
			// 
			this.textClinicID.Location = new System.Drawing.Point(101, 63);
			this.textClinicID.Name = "textClinicID";
			this.textClinicID.Size = new System.Drawing.Size(120, 20);
			this.textClinicID.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 63);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 20);
			this.label1.TabIndex = 5;
			this.label1.Text = "Clinic ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 89);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 20);
			this.label2.TabIndex = 7;
			this.label2.Text = "Clinic Key";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClinicKey
			// 
			this.textClinicKey.Location = new System.Drawing.Point(101, 89);
			this.textClinicKey.Multiline = true;
			this.textClinicKey.Name = "textClinicKey";
			this.textClinicKey.Size = new System.Drawing.Size(120, 48);
			this.textClinicKey.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(13, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 20);
			this.label3.TabIndex = 9;
			this.label3.Text = "Clinic";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textClinicAbbr
			// 
			this.textClinicAbbr.Location = new System.Drawing.Point(101, 37);
			this.textClinicAbbr.Name = "textClinicAbbr";
			this.textClinicAbbr.ReadOnly = true;
			this.textClinicAbbr.Size = new System.Drawing.Size(120, 20);
			this.textClinicAbbr.TabIndex = 8;
			// 
			// butRegisterClinic
			// 
			this.butRegisterClinic.Location = new System.Drawing.Point(247, 34);
			this.butRegisterClinic.Name = "butRegisterClinic";
			this.butRegisterClinic.Size = new System.Drawing.Size(91, 24);
			this.butRegisterClinic.TabIndex = 10;
			this.butRegisterClinic.Text = "Register Clinic";
			this.butRegisterClinic.Click += new System.EventHandler(this.butRegisterClinic_Click);
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(247, 59);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(91, 24);
			this.butClear.TabIndex = 11;
			this.butClear.Text = "Clear ID/Key";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(350, 24);
			this.menuMain.TabIndex = 12;
			// 
			// FormDoseSpotPropertyEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(350, 199);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butRegisterClinic);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textClinicAbbr);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textClinicKey);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textClinicID);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDoseSpotPropertyEdit";
			this.Text = "DoseSpot Property Edit";
			this.Load += new System.EventHandler(this.FormDoseSpotPropertyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textClinicID;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textClinicKey;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textClinicAbbr;
		private UI.Button butRegisterClinic;
		private UI.Button butClear;
		private UI.MenuOD menuMain;
	}
}