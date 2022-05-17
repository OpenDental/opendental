namespace OpenDental{
	partial class FormEhrQuarterlyKeyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrQuarterlyKeyEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textKey = new System.Windows.Forms.TextBox();
			this.textYear = new OpenDental.ValidNum();
			this.textQuarter = new OpenDental.ValidNum();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(204, 119);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(294, 119);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 6;
			this.label1.Text = "Year, ex: 12";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(17, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Quarter, ex: 2";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(17, 74);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Key";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textKey
			// 
			this.textKey.Location = new System.Drawing.Point(122, 73);
			this.textKey.Name = "textKey";
			this.textKey.Size = new System.Drawing.Size(163, 20);
			this.textKey.TabIndex = 2;
			// 
			// textYear
			// 
			this.textYear.Location = new System.Drawing.Point(122, 23);
			this.textYear.MaxVal = 20;
			this.textYear.MinVal = 11;
			this.textYear.Name = "textYear";
			this.textYear.Size = new System.Drawing.Size(58, 20);
			this.textYear.TabIndex = 0;
			// 
			// textQuarter
			// 
			this.textQuarter.Location = new System.Drawing.Point(122, 48);
			this.textQuarter.MaxVal = 4;
			this.textQuarter.MinVal = 1;
			this.textQuarter.Name = "textQuarter";
			this.textQuarter.Size = new System.Drawing.Size(58, 20);
			this.textQuarter.TabIndex = 1;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 119);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 3;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormEhrQuarterlyKeyEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(388, 158);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textQuarter);
			this.Controls.Add(this.textYear);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textKey);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrQuarterlyKeyEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Ehr Quarterly Key";
			this.Load += new System.EventHandler(this.FormEhrQuarterlyKeyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textKey;
		private ValidNum textYear;
		private ValidNum textQuarter;
		private UI.Button butDelete;
	}
}