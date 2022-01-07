namespace OpenDental{
	partial class FormScreenSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScreenSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkUsePat = new System.Windows.Forms.CheckBox();
			this.comboExamSheets = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(239, 107);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(331, 107);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkUsePat
			// 
			this.checkUsePat.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePat.Location = new System.Drawing.Point(3, 28);
			this.checkUsePat.Name = "checkUsePat";
			this.checkUsePat.Size = new System.Drawing.Size(224, 18);
			this.checkUsePat.TabIndex = 4;
			this.checkUsePat.Text = "Attach screenings to patient records";
			this.checkUsePat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUsePat.UseVisualStyleBackColor = true;
			this.checkUsePat.CheckedChanged += new System.EventHandler(this.checkUsePat_CheckedChanged);
			// 
			// comboExamSheets
			// 
			this.comboExamSheets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboExamSheets.Enabled = false;
			this.comboExamSheets.FormattingEnabled = true;
			this.comboExamSheets.Location = new System.Drawing.Point(213, 50);
			this.comboExamSheets.Name = "comboExamSheets";
			this.comboExamSheets.Size = new System.Drawing.Size(153, 21);
			this.comboExamSheets.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(115, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 14);
			this.label1.TabIndex = 6;
			this.label1.Text = "Exam Sheet";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormScreenSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(418, 143);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboExamSheets);
			this.Controls.Add(this.checkUsePat);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormScreenSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Screening Setup";
			this.Load += new System.EventHandler(this.FormScreenSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkUsePat;
		private System.Windows.Forms.ComboBox comboExamSheets;
		private System.Windows.Forms.Label label1;
	}
}