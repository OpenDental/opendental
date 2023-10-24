namespace OpenDental{
	partial class FormBenefitFrequencyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormBenefitFrequencyEdit));
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.comboFrequencyOptions = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textNumber = new OpenDental.ValidNum();
			this.label24 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.listBoxCodeGroup = new OpenDental.UI.ListBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(333, 526);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 526);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 24);
			this.butDelete.TabIndex = 76;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// comboFrequencyOptions
			// 
			this.comboFrequencyOptions.Location = new System.Drawing.Point(148, 489);
			this.comboFrequencyOptions.Name = "comboFrequencyOptions";
			this.comboFrequencyOptions.Size = new System.Drawing.Size(120, 21);
			this.comboFrequencyOptions.TabIndex = 195;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(41, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 21);
			this.label3.TabIndex = 196;
			this.label3.Text = "Code Group";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNumber
			// 
			this.textNumber.Location = new System.Drawing.Point(148, 463);
			this.textNumber.MinVal = 1;
			this.textNumber.Name = "textNumber";
			this.textNumber.Size = new System.Drawing.Size(39, 20);
			this.textNumber.TabIndex = 194;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(70, 463);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(76, 20);
			this.label24.TabIndex = 197;
			this.label24.Text = "#";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(42, 490);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 21);
			this.label1.TabIndex = 198;
			this.label1.Text = "Time Period";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listBoxCodeGroup
			// 
			this.listBoxCodeGroup.Location = new System.Drawing.Point(148, 17);
			this.listBoxCodeGroup.Name = "listBoxCodeGroup";
			this.listBoxCodeGroup.Size = new System.Drawing.Size(173, 439);
			this.listBoxCodeGroup.TabIndex = 199;
			this.listBoxCodeGroup.TabStop = false;
			// 
			// FormBenefitFrequencyEdit
			// 
			this.ClientSize = new System.Drawing.Size(420, 562);
			this.Controls.Add(this.listBoxCodeGroup);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboFrequencyOptions);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textNumber);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormBenefitFrequencyEdit";
			this.Text = "Edit Benefit Frequency";
			this.Load += new System.EventHandler(this.FormBenefitFrequencyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.Button butDelete;
		private UI.ComboBox comboFrequencyOptions;
		private System.Windows.Forms.Label label3;
		private ValidNum textNumber;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label1;
		private UI.ListBox listBoxCodeGroup;
	}
}