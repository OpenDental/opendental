namespace OpenDental{
	partial class FormSheetFieldChart {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetFieldChart));
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.radioPermanent = new System.Windows.Forms.RadioButton();
			this.radioPrimary = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(316, 137);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Image = global::OpenDental.Properties.Resources.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 137);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(77, 24);
			this.butDelete.TabIndex = 100;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioPermanent);
			this.groupBox1.Controls.Add(this.radioPrimary);
			this.groupBox1.Location = new System.Drawing.Point(88, 37);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(225, 74);
			this.groupBox1.TabIndex = 101;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Chart Type";
			// 
			// radioPermanent
			// 
			this.radioPermanent.Location = new System.Drawing.Point(17, 44);
			this.radioPermanent.Name = "radioPermanent";
			this.radioPermanent.Size = new System.Drawing.Size(202, 17);
			this.radioPermanent.TabIndex = 1;
			this.radioPermanent.TabStop = true;
			this.radioPermanent.Text = "Permanent Teeth";
			this.radioPermanent.UseVisualStyleBackColor = true;
			// 
			// radioPrimary
			// 
			this.radioPrimary.Location = new System.Drawing.Point(17, 21);
			this.radioPrimary.Name = "radioPrimary";
			this.radioPrimary.Size = new System.Drawing.Size(202, 17);
			this.radioPrimary.TabIndex = 0;
			this.radioPrimary.TabStop = true;
			this.radioPrimary.Text = "Primary Teeth";
			this.radioPrimary.UseVisualStyleBackColor = true;
			// 
			// FormSheetFieldChart
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(403, 173);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetFieldChart";
			this.Text = "Edit Screen Chart";
			this.Load += new System.EventHandler(this.FormSheetFieldChart_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioPermanent;
		private System.Windows.Forms.RadioButton radioPrimary;
	}
}