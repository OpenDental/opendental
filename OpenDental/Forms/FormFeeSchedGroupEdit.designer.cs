namespace OpenDental{
	partial class FormFeeSchedGroupEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedGroupEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.gridGroup = new OpenDental.UI.GridOD();
			this.gridAvailable = new OpenDental.UI.GridOD();
			this.butDelete = new OpenDental.UI.Button();
			this.comboFeeSched = new OpenDental.UI.ComboBoxOD();
			this.labelFeeSched = new System.Windows.Forms.Label();
			this.butPickFeeSched = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(411, 388);
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
			this.butCancel.Location = new System.Drawing.Point(492, 388);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(230, 19);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(152, 20);
			this.textDescription.TabIndex = 4;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(124, 19);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(100, 20);
			this.labelDescription.TabIndex = 5;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(275, 173);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(35, 26);
			this.butRight.TabIndex = 54;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.AdjustImageLocation = new System.Drawing.Point(-1, 0);
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(275, 205);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(35, 26);
			this.butLeft.TabIndex = 55;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// gridGroup
			// 
			this.gridGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridGroup.Location = new System.Drawing.Point(334, 81);
			this.gridGroup.Name = "gridGroup";
			this.gridGroup.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridGroup.Size = new System.Drawing.Size(233, 291);
			this.gridGroup.TabIndex = 56;
			this.gridGroup.Title = "Clinics in Group";
			this.gridGroup.TranslationName = "Table Group Clinics ";
			// 
			// gridAvailable
			// 
			this.gridAvailable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridAvailable.Location = new System.Drawing.Point(18, 81);
			this.gridAvailable.Name = "gridAvailable";
			this.gridAvailable.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAvailable.Size = new System.Drawing.Size(233, 291);
			this.gridAvailable.TabIndex = 57;
			this.gridAvailable.Title = "Available Clinics";
			this.gridAvailable.TranslationName = "Table Clinics Available";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(18, 386);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 26);
			this.butDelete.TabIndex = 58;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// comboFeeSched
			// 
			this.comboFeeSched.Location = new System.Drawing.Point(230, 45);
			this.comboFeeSched.Name = "comboFeeSched";
			this.comboFeeSched.Size = new System.Drawing.Size(126, 21);
			this.comboFeeSched.TabIndex = 6;
			this.comboFeeSched.SelectedIndexChanged += new System.EventHandler(this.comboFeeSched_SelectedIndexChanged);
			// 
			// labelFeeSched
			// 
			this.labelFeeSched.Location = new System.Drawing.Point(124, 45);
			this.labelFeeSched.Name = "labelFeeSched";
			this.labelFeeSched.Size = new System.Drawing.Size(100, 21);
			this.labelFeeSched.TabIndex = 7;
			this.labelFeeSched.Text = "Fee Schedule";
			this.labelFeeSched.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickFeeSched
			// 
			this.butPickFeeSched.Location = new System.Drawing.Point(359, 45);
			this.butPickFeeSched.Name = "butPickFeeSched";
			this.butPickFeeSched.Size = new System.Drawing.Size(23, 21);
			this.butPickFeeSched.TabIndex = 59;
			this.butPickFeeSched.Text = "...";
			this.butPickFeeSched.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// FormFeeSchedGroupEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(589, 426);
			this.Controls.Add(this.butPickFeeSched);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridAvailable);
			this.Controls.Add(this.gridGroup);
			this.Controls.Add(this.butLeft);
			this.Controls.Add(this.butRight);
			this.Controls.Add(this.labelFeeSched);
			this.Controls.Add(this.comboFeeSched);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeeSchedGroupEdit";
			this.Text = "Fee Schedule Group Edit";
			this.Load += new System.EventHandler(this.FormFeeSchedGroupEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelDescription;
		private UI.Button butRight;
		private UI.Button butLeft;
		private UI.GridOD gridGroup;
		private UI.GridOD gridAvailable;
		private UI.Button butDelete;
		private UI.ComboBoxOD comboFeeSched;
		private System.Windows.Forms.Label labelFeeSched;
		private UI.Button butPickFeeSched;
	}
}