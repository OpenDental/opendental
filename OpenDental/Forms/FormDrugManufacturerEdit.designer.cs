namespace OpenDental{
	partial class FormDrugManufacturerEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDrugManufacturerEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textManufacturerName = new System.Windows.Forms.TextBox();
			this.labelManufacturerName = new System.Windows.Forms.Label();
			this.textManufacturerCode = new System.Windows.Forms.TextBox();
			this.labelManufacturerCode = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(218, 115);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(299, 115);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(26, 115);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textManufacturerName
			// 
			this.textManufacturerName.Location = new System.Drawing.Point(137, 28);
			this.textManufacturerName.Name = "textManufacturerName";
			this.textManufacturerName.Size = new System.Drawing.Size(237, 20);
			this.textManufacturerName.TabIndex = 0;
			// 
			// labelManufacturerName
			// 
			this.labelManufacturerName.Location = new System.Drawing.Point(12, 27);
			this.labelManufacturerName.Name = "labelManufacturerName";
			this.labelManufacturerName.Size = new System.Drawing.Size(119, 20);
			this.labelManufacturerName.TabIndex = 111;
			this.labelManufacturerName.Text = "Manufacturer Name";
			this.labelManufacturerName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textManufacturerCode
			// 
			this.textManufacturerCode.Location = new System.Drawing.Point(137, 53);
			this.textManufacturerCode.Name = "textManufacturerCode";
			this.textManufacturerCode.Size = new System.Drawing.Size(82, 20);
			this.textManufacturerCode.TabIndex = 1;
			// 
			// labelManufacturerCode
			// 
			this.labelManufacturerCode.Location = new System.Drawing.Point(20, 53);
			this.labelManufacturerCode.Name = "labelManufacturerCode";
			this.labelManufacturerCode.Size = new System.Drawing.Size(111, 20);
			this.labelManufacturerCode.TabIndex = 109;
			this.labelManufacturerCode.Text = "Manufacturer Code";
			this.labelManufacturerCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDrugManufacturerEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(399, 166);
			this.Controls.Add(this.textManufacturerName);
			this.Controls.Add(this.labelManufacturerName);
			this.Controls.Add(this.textManufacturerCode);
			this.Controls.Add(this.labelManufacturerCode);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDrugManufacturerEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Drug Manufacturer Edit";
			this.Load += new System.EventHandler(this.FormDrugManufacturerEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textManufacturerName;
		private System.Windows.Forms.Label labelManufacturerName;
		private System.Windows.Forms.TextBox textManufacturerCode;
		private System.Windows.Forms.Label labelManufacturerCode;
	}
}