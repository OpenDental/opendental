namespace OpenDental{
	partial class FormDrugUnitEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDrugUnitEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textUnitText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textUnitIdentifier = new System.Windows.Forms.TextBox();
			this.labelCVXCode = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(185, 98);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(266, 98);
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
			this.butDelete.Location = new System.Drawing.Point(26, 98);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 4;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textUnitText
			// 
			this.textUnitText.Location = new System.Drawing.Point(113, 53);
			this.textUnitText.Name = "textUnitText";
			this.textUnitText.Size = new System.Drawing.Size(228, 20);
			this.textUnitText.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(23, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 20);
			this.label1.TabIndex = 112;
			this.label1.Text = "Unit Text";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUnitIdentifier
			// 
			this.textUnitIdentifier.Location = new System.Drawing.Point(113, 27);
			this.textUnitIdentifier.Name = "textUnitIdentifier";
			this.textUnitIdentifier.Size = new System.Drawing.Size(77, 20);
			this.textUnitIdentifier.TabIndex = 0;
			// 
			// labelCVXCode
			// 
			this.labelCVXCode.Location = new System.Drawing.Point(23, 26);
			this.labelCVXCode.Name = "labelCVXCode";
			this.labelCVXCode.Size = new System.Drawing.Size(88, 20);
			this.labelCVXCode.TabIndex = 110;
			this.labelCVXCode.Text = "Unit Identifier";
			this.labelCVXCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDrugUnitEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(366, 149);
			this.Controls.Add(this.textUnitText);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textUnitIdentifier);
			this.Controls.Add(this.labelCVXCode);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDrugUnitEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Drug Unit Edit";
			this.Load += new System.EventHandler(this.FormDrugUnitEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.TextBox textUnitText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textUnitIdentifier;
		private System.Windows.Forms.Label labelCVXCode;
	}
}