namespace OpenDental{
	partial class FormEhrQuarterlyKeyEditCust {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrQuarterlyKeyEditCust));
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textNotes = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textPracticeTitle = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textEhrKey = new System.Windows.Forms.TextBox();
			this.labelEhrKey = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butGenerate = new OpenDental.UI.Button();
			this.textQuarter = new OpenDental.ValidNum();
			this.textYear = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(42, 93);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 13;
			this.label2.Text = "Quarter, ex: 2";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(42, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 18);
			this.label1.TabIndex = 12;
			this.label1.Text = "Year, ex: 12";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNotes
			// 
			this.textNotes.Location = new System.Drawing.Point(147, 170);
			this.textNotes.Multiline = true;
			this.textNotes.Name = "textNotes";
			this.textNotes.Size = new System.Drawing.Size(319, 92);
			this.textNotes.TabIndex = 126;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(59, 174);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(88, 14);
			this.label4.TabIndex = 127;
			this.label4.Text = "Notes";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPracticeTitle
			// 
			this.textPracticeTitle.Location = new System.Drawing.Point(147, 117);
			this.textPracticeTitle.MaxLength = 100;
			this.textPracticeTitle.Name = "textPracticeTitle";
			this.textPracticeTitle.Size = new System.Drawing.Size(319, 20);
			this.textPracticeTitle.TabIndex = 130;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(19, 121);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(127, 14);
			this.label8.TabIndex = 131;
			this.label8.Text = "Practice Title";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEhrKey
			// 
			this.textEhrKey.Location = new System.Drawing.Point(147, 143);
			this.textEhrKey.MaxLength = 15;
			this.textEhrKey.Name = "textEhrKey";
			this.textEhrKey.Size = new System.Drawing.Size(161, 20);
			this.textEhrKey.TabIndex = 128;
			// 
			// labelEhrKey
			// 
			this.labelEhrKey.Location = new System.Drawing.Point(22, 147);
			this.labelEhrKey.Name = "labelEhrKey";
			this.labelEhrKey.Size = new System.Drawing.Size(125, 14);
			this.labelEhrKey.TabIndex = 129;
			this.labelEhrKey.Text = "Quarterly EHR Key";
			this.labelEhrKey.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(146, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(422, 38);
			this.label3.TabIndex = 133;
			this.label3.Text = "The quarterly key is tied to the practice title as entered in THEIR system.  It\'s" +
    " best to copy/paste from their practice setup window.";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(25, 292);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 134;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butGenerate
			// 
			this.butGenerate.Location = new System.Drawing.Point(311, 141);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(75, 24);
			this.butGenerate.TabIndex = 132;
			this.butGenerate.Text = "Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// textQuarter
			// 
			this.textQuarter.Location = new System.Drawing.Point(147, 91);
			this.textQuarter.MaxVal = 4;
			this.textQuarter.MinVal = 1;
			this.textQuarter.Name = "textQuarter";
			this.textQuarter.Size = new System.Drawing.Size(58, 20);
			this.textQuarter.TabIndex = 17;
			// 
			// textYear
			// 
			this.textYear.Location = new System.Drawing.Point(147, 66);
			this.textYear.MaxVal = 20;
			this.textYear.MinVal = 11;
			this.textYear.Name = "textYear";
			this.textYear.Size = new System.Drawing.Size(58, 20);
			this.textYear.TabIndex = 16;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(430, 292);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(523, 292);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEhrQuarterlyKeyEditCust
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(610, 328);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.textPracticeTitle);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textEhrKey);
			this.Controls.Add(this.labelEhrKey);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textQuarter);
			this.Controls.Add(this.textYear);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrQuarterlyKeyEditCust";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Ehr Quarterly Key for Customer";
			this.Load += new System.EventHandler(this.FormEhrQuarterlyKeyEditCust_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidNum textQuarter;
		private ValidNum textYear;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textNotes;
		private System.Windows.Forms.Label label4;
		private UI.Button butGenerate;
		private System.Windows.Forms.TextBox textPracticeTitle;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textEhrKey;
		private System.Windows.Forms.Label labelEhrKey;
		private System.Windows.Forms.Label label3;
		private UI.Button butDelete;
	}
}