namespace OpenDental{
	partial class FormElectIDEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormElectIDEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textPayerID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkIsMedicaid = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textComments = new System.Windows.Forms.TextBox();
			this.labelPayerService = new System.Windows.Forms.Label();
			this.textPayerService = new System.Windows.Forms.TextBox();
			this.gridAttributes = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(576, 355);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(657, 355);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textPayerID
			// 
			this.textPayerID.Location = new System.Drawing.Point(91, 15);
			this.textPayerID.Name = "textPayerID";
			this.textPayerID.Size = new System.Drawing.Size(111, 20);
			this.textPayerID.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 16);
			this.label1.TabIndex = 5;
			this.label1.Text = "Payer ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(91, 41);
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.Size = new System.Drawing.Size(331, 20);
			this.textCarrierName.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 19);
			this.label2.TabIndex = 7;
			this.label2.Text = "Carrier Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsMedicaid
			// 
			this.checkIsMedicaid.Location = new System.Drawing.Point(91, 242);
			this.checkIsMedicaid.Name = "checkIsMedicaid";
			this.checkIsMedicaid.Size = new System.Drawing.Size(80, 17);
			this.checkIsMedicaid.TabIndex = 8;
			this.checkIsMedicaid.Text = "Is Medicaid";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 20);
			this.label3.TabIndex = 10;
			this.label3.Text = "Comments";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textComments
			// 
			this.textComments.Location = new System.Drawing.Point(91, 67);
			this.textComments.Multiline = true;
			this.textComments.Name = "textComments";
			this.textComments.Size = new System.Drawing.Size(331, 169);
			this.textComments.TabIndex = 9;
			// 
			// labelPayerService
			// 
			this.labelPayerService.Location = new System.Drawing.Point(494, 19);
			this.labelPayerService.Name = "labelPayerService";
			this.labelPayerService.Size = new System.Drawing.Size(82, 16);
			this.labelPayerService.TabIndex = 12;
			this.labelPayerService.Text = "Payer Service";
			this.labelPayerService.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayerService
			// 
			this.textPayerService.Location = new System.Drawing.Point(582, 15);
			this.textPayerService.Name = "textPayerService";
			this.textPayerService.ReadOnly = true;
			this.textPayerService.Size = new System.Drawing.Size(111, 20);
			this.textPayerService.TabIndex = 11;
			// 
			// gridAttributes
			// 
			this.gridAttributes.Location = new System.Drawing.Point(434, 41);
			this.gridAttributes.Name = "gridAttributes";
			this.gridAttributes.Size = new System.Drawing.Size(259, 285);
			this.gridAttributes.TabIndex = 13;
			this.gridAttributes.Title = "Attributes";
			// 
			// FormElectIDEdit
			// 
			this.ClientSize = new System.Drawing.Size(750, 391);
			this.Controls.Add(this.gridAttributes);
			this.Controls.Add(this.labelPayerService);
			this.Controls.Add(this.textPayerService);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textComments);
			this.Controls.Add(this.checkIsMedicaid);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textCarrierName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPayerID);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormElectIDEdit";
			this.Text = "Edit Elect ID";
			this.Load += new System.EventHandler(this.FormElectIDEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textPayerID;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textCarrierName;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.CheckBox checkIsMedicaid;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textComments;
		private System.Windows.Forms.Label labelPayerService;
		private System.Windows.Forms.TextBox textPayerService;
		private UI.GridOD gridAttributes;
	}
}