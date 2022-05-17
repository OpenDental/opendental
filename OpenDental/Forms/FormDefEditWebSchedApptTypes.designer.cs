namespace OpenDental{
	partial class FormDefEditWebSchedApptTypes {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDefEditWebSchedApptTypes));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butSelect = new OpenDental.UI.Button();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.butColor = new System.Windows.Forms.Button();
			this.textApptType = new System.Windows.Forms.TextBox();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelValue = new System.Windows.Forms.Label();
			this.labelName = new System.Windows.Forms.Label();
			this.labelColor = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.textRestrictToBlockouts = new System.Windows.Forms.TextBox();
			this.butSelectBlockouts = new OpenDental.UI.Button();
			this.labelRestrictToBlockouts = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(311, 135);
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
			this.butCancel.Location = new System.Drawing.Point(392, 135);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butSelect
			// 
			this.butSelect.Location = new System.Drawing.Point(371, 54);
			this.butSelect.Name = "butSelect";
			this.butSelect.Size = new System.Drawing.Size(21, 22);
			this.butSelect.TabIndex = 209;
			this.butSelect.Text = "...";
			this.butSelect.Click += new System.EventHandler(this.butSelect_Click);
			// 
			// checkHidden
			// 
			this.checkHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidden.Location = new System.Drawing.Point(12, 12);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(157, 18);
			this.checkHidden.TabIndex = 207;
			this.checkHidden.Text = "Hidden";
			this.checkHidden.Visible = false;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(437, 57);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(30, 20);
			this.butColor.TabIndex = 206;
			this.butColor.Visible = false;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// textApptType
			// 
			this.textApptType.Location = new System.Drawing.Point(190, 55);
			this.textApptType.MaxLength = 256;
			this.textApptType.Multiline = true;
			this.textApptType.Name = "textApptType";
			this.textApptType.ReadOnly = true;
			this.textApptType.Size = new System.Drawing.Size(178, 22);
			this.textApptType.TabIndex = 204;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(12, 55);
			this.textName.Multiline = true;
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(178, 64);
			this.textName.TabIndex = 202;
			// 
			// labelValue
			// 
			this.labelValue.Location = new System.Drawing.Point(190, 25);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(178, 28);
			this.labelValue.TabIndex = 205;
			this.labelValue.Text = "Appointment Type";
			this.labelValue.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(12, 36);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(178, 17);
			this.labelName.TabIndex = 203;
			this.labelName.Text = "Reason";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(373, 39);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(95, 15);
			this.labelColor.TabIndex = 208;
			this.labelColor.Text = "Color";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelColor.Visible = false;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 135);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 211;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// colorDialog1
			// 
			this.colorDialog1.FullOpen = true;
			// 
			// textRestrictToBlockouts
			// 
			this.textRestrictToBlockouts.Location = new System.Drawing.Point(190, 99);
			this.textRestrictToBlockouts.Name = "textRestrictToBlockouts";
			this.textRestrictToBlockouts.ReadOnly = true;
			this.textRestrictToBlockouts.Size = new System.Drawing.Size(178, 20);
			this.textRestrictToBlockouts.TabIndex = 212;
			// 
			// butSelectBlockouts
			// 
			this.butSelectBlockouts.Location = new System.Drawing.Point(371, 98);
			this.butSelectBlockouts.Name = "butSelectBlockouts";
			this.butSelectBlockouts.Size = new System.Drawing.Size(21, 22);
			this.butSelectBlockouts.TabIndex = 213;
			this.butSelectBlockouts.Text = "...";
			this.butSelectBlockouts.UseVisualStyleBackColor = true;
			this.butSelectBlockouts.Click += new System.EventHandler(this.butSelectBlockouts_Click);
			// 
			// labelRestrictToBlockouts
			// 
			this.labelRestrictToBlockouts.Location = new System.Drawing.Point(190, 82);
			this.labelRestrictToBlockouts.Name = "labelRestrictToBlockouts";
			this.labelRestrictToBlockouts.Size = new System.Drawing.Size(201, 16);
			this.labelRestrictToBlockouts.TabIndex = 214;
			this.labelRestrictToBlockouts.Text = "Restrict to Specific Blockouts";
			// 
			// FormDefEditWSNPApptTypes
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(479, 171);
			this.Controls.Add(this.labelRestrictToBlockouts);
			this.Controls.Add(this.butSelectBlockouts);
			this.Controls.Add(this.textRestrictToBlockouts);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSelect);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.textApptType);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelValue);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelColor);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDefEditWSNPApptTypes";
			this.Text = "";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butSelect;
		private System.Windows.Forms.CheckBox checkHidden;
		private System.Windows.Forms.Button butColor;
		private System.Windows.Forms.TextBox textApptType;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelValue;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.Label labelColor;
		private UI.Button butDelete;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.TextBox textRestrictToBlockouts;
		private UI.Button butSelectBlockouts;
		private System.Windows.Forms.Label labelRestrictToBlockouts;
	}
}