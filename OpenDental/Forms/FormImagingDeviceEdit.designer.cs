namespace OpenDentalImaging{
	partial class FormImagingDeviceEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormImagingDeviceEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textComputerName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.listDeviceType = new OpenDental.UI.ListBoxOD();
			this.butThis = new OpenDental.UI.Button();
			this.comboTwainName = new System.Windows.Forms.ComboBox();
			this.checkShowTwainUI = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(390, 211);
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
			this.butCancel.Location = new System.Drawing.Point(471, 211);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 211);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(43, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(126, 33);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(339, 20);
			this.textDescription.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 58);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(101, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Computer Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textComputerName
			// 
			this.textComputerName.Location = new System.Drawing.Point(126, 59);
			this.textComputerName.Name = "textComputerName";
			this.textComputerName.Size = new System.Drawing.Size(182, 20);
			this.textComputerName.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(44, 87);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(82, 18);
			this.label5.TabIndex = 13;
			this.label5.Text = "Device Type";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(372, 54);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(154, 29);
			this.label6.TabIndex = 17;
			this.label6.Text = "Leave blank to make available on all computers";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(43, 119);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(82, 18);
			this.label9.TabIndex = 21;
			this.label9.Text = "Twain Name";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// listDeviceType
			// 
			this.listDeviceType.Location = new System.Drawing.Point(126, 85);
			this.listDeviceType.Name = "listDeviceType";
			this.listDeviceType.Size = new System.Drawing.Size(119, 30);
			this.listDeviceType.TabIndex = 22;
			this.listDeviceType.SelectionChangeCommitted += new System.EventHandler(this.listDeviceType_SelectionChangeCommitted);
			// 
			// butThis
			// 
			this.butThis.Location = new System.Drawing.Point(314, 57);
			this.butThis.Name = "butThis";
			this.butThis.Size = new System.Drawing.Size(55, 24);
			this.butThis.TabIndex = 23;
			this.butThis.Text = "This";
			this.butThis.Click += new System.EventHandler(this.butThis_Click);
			// 
			// comboTwainName
			// 
			this.comboTwainName.DropDownHeight = 150;
			this.comboTwainName.IntegralHeight = false;
			this.comboTwainName.Location = new System.Drawing.Point(126, 121);
			this.comboTwainName.MaxDropDownItems = 100;
			this.comboTwainName.Name = "comboTwainName";
			this.comboTwainName.Size = new System.Drawing.Size(339, 21);
			this.comboTwainName.TabIndex = 25;
			this.comboTwainName.DropDown += new System.EventHandler(this.comboTwainName_DropDown);
			// 
			// checkShowTwainUI
			// 
			this.checkShowTwainUI.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowTwainUI.Location = new System.Drawing.Point(24, 146);
			this.checkShowTwainUI.Name = "checkShowTwainUI";
			this.checkShowTwainUI.Size = new System.Drawing.Size(116, 20);
			this.checkShowTwainUI.TabIndex = 26;
			this.checkShowTwainUI.Text = "Show Twain UI";
			this.checkShowTwainUI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowTwainUI.UseVisualStyleBackColor = true;
			// 
			// FormImagingDeviceEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(558, 247);
			this.Controls.Add(this.checkShowTwainUI);
			this.Controls.Add(this.comboTwainName);
			this.Controls.Add(this.butThis);
			this.Controls.Add(this.listDeviceType);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textComputerName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormImagingDeviceEdit";
			this.Text = "Edit Imaging Device";
			this.Load += new System.EventHandler(this.FormImagingDeviceEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textComputerName;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label9;
		private OpenDental.UI.ListBoxOD listDeviceType;
		private OpenDental.UI.Button butThis;
		private System.Windows.Forms.ComboBox comboTwainName;
		private System.Windows.Forms.CheckBox checkShowTwainUI;
	}
}