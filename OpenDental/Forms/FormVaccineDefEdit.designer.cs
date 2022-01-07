namespace OpenDental{
	partial class FormVaccineDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVaccineDefEdit));
			this.comboManufacturer = new System.Windows.Forms.ComboBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textVaccineName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textCVXCode = new System.Windows.Forms.TextBox();
			this.labelCVXCode = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butCvxSelect = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// comboManufacturer
			// 
			this.comboManufacturer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboManufacturer.Location = new System.Drawing.Point(101, 83);
			this.comboManufacturer.Name = "comboManufacturer";
			this.comboManufacturer.Size = new System.Drawing.Size(173, 21);
			this.comboManufacturer.TabIndex = 3;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(202, 135);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(283, 135);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(1, 83);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(98, 18);
			this.label3.TabIndex = 22;
			this.label3.Text = "Manufacturer";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVaccineName
			// 
			this.textVaccineName.Location = new System.Drawing.Point(101, 57);
			this.textVaccineName.Name = "textVaccineName";
			this.textVaccineName.Size = new System.Drawing.Size(229, 20);
			this.textVaccineName.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 19;
			this.label1.Text = "Vaccine Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCVXCode
			// 
			this.textCVXCode.Location = new System.Drawing.Point(101, 31);
			this.textCVXCode.Name = "textCVXCode";
			this.textCVXCode.ReadOnly = true;
			this.textCVXCode.Size = new System.Drawing.Size(229, 20);
			this.textCVXCode.TabIndex = 0;
			this.textCVXCode.TabStop = false;
			// 
			// labelCVXCode
			// 
			this.labelCVXCode.Location = new System.Drawing.Point(11, 30);
			this.labelCVXCode.Name = "labelCVXCode";
			this.labelCVXCode.Size = new System.Drawing.Size(88, 20);
			this.labelCVXCode.TabIndex = 17;
			this.labelCVXCode.Text = "CVX Code";
			this.labelCVXCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 135);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCvxSelect
			// 
			this.butCvxSelect.Location = new System.Drawing.Point(336, 30);
			this.butCvxSelect.Name = "butCvxSelect";
			this.butCvxSelect.Size = new System.Drawing.Size(22, 22);
			this.butCvxSelect.TabIndex = 1;
			this.butCvxSelect.Text = "...";
			this.butCvxSelect.Click += new System.EventHandler(this.butCvxSelect_Click);
			// 
			// FormVaccineDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(370, 186);
			this.Controls.Add(this.butCvxSelect);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboManufacturer);
			this.Controls.Add(this.textVaccineName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCVXCode);
			this.Controls.Add(this.labelCVXCode);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormVaccineDefEdit";
			this.Text = "Vaccine Definition Edit";
			this.Load += new System.EventHandler(this.FormVaccineDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textVaccineName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCVXCode;
		private System.Windows.Forms.Label labelCVXCode;
		private UI.Button butDelete;
		private UI.Button butCvxSelect;
		private System.Windows.Forms.ComboBox comboManufacturer;
	}
}