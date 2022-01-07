namespace OpenDental{
	partial class FormEditTestModeOverrides {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEditTestModeOverrides));
			this.butSave = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.comboSelectOverride = new OpenDental.UI.ComboBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textEnterOverrideValue = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Enabled = false;
			this.butSave.Location = new System.Drawing.Point(316, 425);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(397, 425);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboSelectOverride
			// 
			this.comboSelectOverride.Location = new System.Drawing.Point(12, 25);
			this.comboSelectOverride.Name = "comboSelectOverride";
			this.comboSelectOverride.Size = new System.Drawing.Size(207, 21);
			this.comboSelectOverride.TabIndex = 4;
			this.comboSelectOverride.SelectionChangeCommitted += new System.EventHandler(this.comboSelectOverride_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Select Override";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 63);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(62, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Enter Value";
			// 
			// textEnterOverrideValue
			// 
			this.textEnterOverrideValue.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEnterOverrideValue.Location = new System.Drawing.Point(12, 80);
			this.textEnterOverrideValue.Multiline = true;
			this.textEnterOverrideValue.Name = "textEnterOverrideValue";
			this.textEnterOverrideValue.Size = new System.Drawing.Size(460, 339);
			this.textEnterOverrideValue.TabIndex = 8;
			this.textEnterOverrideValue.TextChanged += new System.EventHandler(this.textEnterOverrideValue_TextChanged);
			// 
			// FormEditTestModeOverrides
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(484, 461);
			this.Controls.Add(this.textEnterOverrideValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboSelectOverride);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butClose);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEditTestModeOverrides";
			this.Text = "Edit Test Mode Overrides";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEditTestModeOverrides_FormClosing);
			this.Load += new System.EventHandler(this.FormEditTestModeOverrides_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.Button butClose;
		private UI.ComboBoxOD comboSelectOverride;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textEnterOverrideValue;
	}
}