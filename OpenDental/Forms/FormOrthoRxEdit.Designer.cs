namespace OpenDental{
	partial class FormOrthoRxEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOrthoRxEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.comboHardwareSpec = new OpenDental.UI.ComboBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.labelComments = new System.Windows.Forms.Label();
			this.labelTeeth = new System.Windows.Forms.Label();
			this.textToothRange = new System.Windows.Forms.TextBox();
			this.butUpper = new OpenDental.UI.Button();
			this.butLower = new OpenDental.UI.Button();
			this.butAll = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(572, 185);
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
			this.butCancel.Location = new System.Drawing.Point(572, 215);
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
			this.butDelete.Location = new System.Drawing.Point(25, 215);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(88, 24);
			this.butDelete.TabIndex = 31;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// comboHardwareSpec
			// 
			this.comboHardwareSpec.Location = new System.Drawing.Point(143, 61);
			this.comboHardwareSpec.Name = "comboHardwareSpec";
			this.comboHardwareSpec.Size = new System.Drawing.Size(293, 21);
			this.comboHardwareSpec.TabIndex = 85;
			this.comboHardwareSpec.SelectionChangeCommitted += new System.EventHandler(this.comboHardwareSpec_SelectionChangeCommitted);
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(35, 63);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 16);
			this.label4.TabIndex = 84;
			this.label4.Text = "Hardware Spec";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelComments
			// 
			this.labelComments.Location = new System.Drawing.Point(140, 111);
			this.labelComments.Name = "labelComments";
			this.labelComments.Size = new System.Drawing.Size(135, 16);
			this.labelComments.TabIndex = 107;
			this.labelComments.Text = "Comments";
			this.labelComments.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelTeeth
			// 
			this.labelTeeth.Location = new System.Drawing.Point(44, 91);
			this.labelTeeth.Name = "labelTeeth";
			this.labelTeeth.Size = new System.Drawing.Size(99, 16);
			this.labelTeeth.TabIndex = 106;
			this.labelTeeth.Text = "Teeth";
			this.labelTeeth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textToothRange
			// 
			this.textToothRange.Location = new System.Drawing.Point(143, 88);
			this.textToothRange.Name = "textToothRange";
			this.textToothRange.Size = new System.Drawing.Size(426, 20);
			this.textToothRange.TabIndex = 105;
			// 
			// butUpper
			// 
			this.butUpper.Location = new System.Drawing.Point(297, 114);
			this.butUpper.Name = "butUpper";
			this.butUpper.Size = new System.Drawing.Size(75, 24);
			this.butUpper.TabIndex = 108;
			this.butUpper.Text = "Upper";
			this.butUpper.Click += new System.EventHandler(this.butUpper_Click);
			// 
			// butLower
			// 
			this.butLower.Location = new System.Drawing.Point(378, 114);
			this.butLower.Name = "butLower";
			this.butLower.Size = new System.Drawing.Size(75, 24);
			this.butLower.TabIndex = 109;
			this.butLower.Text = "Lower";
			this.butLower.Click += new System.EventHandler(this.butLower_Click);
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(459, 114);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(75, 24);
			this.butAll.TabIndex = 110;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(44, 38);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(99, 16);
			this.label1.TabIndex = 112;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(143, 35);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(426, 20);
			this.textDescription.TabIndex = 111;
			// 
			// FormOrthoRxEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(659, 251);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butAll);
			this.Controls.Add(this.butLower);
			this.Controls.Add(this.butUpper);
			this.Controls.Add(this.labelComments);
			this.Controls.Add(this.labelTeeth);
			this.Controls.Add(this.textToothRange);
			this.Controls.Add(this.comboHardwareSpec);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOrthoRxEdit";
			this.Text = "Edit Ortho Prescription";
			this.Load += new System.EventHandler(this.FormOrthoRxEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDelete;
		private UI.ComboBoxOD comboHardwareSpec;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelComments;
		private System.Windows.Forms.Label labelTeeth;
		private System.Windows.Forms.TextBox textToothRange;
		private UI.Button butUpper;
		private UI.Button butLower;
		private UI.Button butAll;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
	}
}