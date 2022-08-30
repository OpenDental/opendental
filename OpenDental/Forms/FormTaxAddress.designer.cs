namespace OpenDental{
	partial class FormTaxAddress {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaxAddress));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelAddress = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.labelAddresTwo = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.labelCity = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.labelState = new System.Windows.Forms.Label();
			this.textZipCode = new System.Windows.Forms.TextBox();
			this.labelZip = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(271, 195);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(352, 195);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelAddress
			// 
			this.labelAddress.Location = new System.Drawing.Point(15, 51);
			this.labelAddress.Name = "labelAddress";
			this.labelAddress.Size = new System.Drawing.Size(100, 18);
			this.labelAddress.TabIndex = 8;
			this.labelAddress.Text = "Address";
			this.labelAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(121, 51);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(194, 20);
			this.textAddress.TabIndex = 0;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(121, 77);
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(194, 20);
			this.textAddress2.TabIndex = 1;
			// 
			// labelAddresTwo
			// 
			this.labelAddresTwo.Location = new System.Drawing.Point(15, 77);
			this.labelAddresTwo.Name = "labelAddresTwo";
			this.labelAddresTwo.Size = new System.Drawing.Size(100, 18);
			this.labelAddresTwo.TabIndex = 9;
			this.labelAddresTwo.Text = "Address 2";
			this.labelAddresTwo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(121, 103);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(103, 20);
			this.textCity.TabIndex = 2;
			// 
			// labelCity
			// 
			this.labelCity.Location = new System.Drawing.Point(15, 105);
			this.labelCity.Name = "labelCity";
			this.labelCity.Size = new System.Drawing.Size(100, 18);
			this.labelCity.TabIndex = 10;
			this.labelCity.Text = "City";
			this.labelCity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(121, 129);
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(34, 20);
			this.textState.TabIndex = 3;
			// 
			// labelState
			// 
			this.labelState.Location = new System.Drawing.Point(15, 131);
			this.labelState.Name = "labelState";
			this.labelState.Size = new System.Drawing.Size(100, 18);
			this.labelState.TabIndex = 11;
			this.labelState.Text = "State";
			this.labelState.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textZipCode
			// 
			this.textZipCode.Location = new System.Drawing.Point(121, 155);
			this.textZipCode.Name = "textZipCode";
			this.textZipCode.Size = new System.Drawing.Size(80, 20);
			this.textZipCode.TabIndex = 4;
			// 
			// labelZip
			// 
			this.labelZip.Location = new System.Drawing.Point(15, 155);
			this.labelZip.Name = "labelZip";
			this.labelZip.Size = new System.Drawing.Size(100, 18);
			this.labelZip.TabIndex = 12;
			this.labelZip.Text = "Zip Code";
			this.labelZip.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 195);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 7;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(415, 38);
			this.label1.TabIndex = 13;
			this.label1.Text = "We must charge sales tax based on the physical office address.  The address is on" +
    "ly entered here when the physical address and billing address are in different s" +
    "tates.";
			// 
			// FormTaxAddress
			// 
			this.ClientSize = new System.Drawing.Size(439, 231);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textZipCode);
			this.Controls.Add(this.labelZip);
			this.Controls.Add(this.textState);
			this.Controls.Add(this.labelState);
			this.Controls.Add(this.textCity);
			this.Controls.Add(this.labelCity);
			this.Controls.Add(this.textAddress2);
			this.Controls.Add(this.labelAddresTwo);
			this.Controls.Add(this.textAddress);
			this.Controls.Add(this.labelAddress);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTaxAddress";
			this.Text = "Physical Tax Address";
			this.Load += new System.EventHandler(this.FormTaxAddress_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelAddress;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.TextBox textAddress2;
		private System.Windows.Forms.Label labelAddresTwo;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.Label labelCity;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label labelState;
		private System.Windows.Forms.TextBox textZipCode;
		private System.Windows.Forms.Label labelZip;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label1;
	}
}