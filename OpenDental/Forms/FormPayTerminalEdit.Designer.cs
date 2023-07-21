namespace OpenDental{
	partial class FormPayTerminalEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayTerminalEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textId = new System.Windows.Forms.TextBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(162, 130);
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
			this.butCancel.Location = new System.Drawing.Point(243, 130);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(118, 26);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(159, 20);
			this.textName.TabIndex = 4;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(12, 24);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(100, 23);
			this.labelName.TabIndex = 5;
			this.labelName.Text = "Terminal Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 7;
			this.label1.Text = "Terminal ID";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textId
			// 
			this.textId.Location = new System.Drawing.Point(118, 52);
			this.textId.Name = "textId";
			this.textId.Size = new System.Drawing.Size(159, 20);
			this.textId.TabIndex = 6;
			// 
			// comboClinic
			// 
			this.comboClinic.ForceShowUnassigned = true;
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(81, 78);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(196, 21);
			this.comboClinic.TabIndex = 93;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 130);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(73, 24);
			this.butDelete.TabIndex = 998;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormPayTerminalEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(336, 166);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textId);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayTerminalEdit";
			this.Text = "Edit Payment Terminal";
			this.Load += new System.EventHandler(this.FormPayTerminalEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textId;
		private UI.ComboBoxClinicPicker comboClinic;
		private UI.Button butDelete;
	}
}