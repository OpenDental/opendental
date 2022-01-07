namespace OpenDental {
	partial class FormPhoneGraphAdminEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneGraphAdminEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDateEntry = new OpenDental.ValidDate();
			this.label10 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textMax = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(62, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(86, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 199);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 15;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(331, 199);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(415, 199);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(154, 14);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(82, 20);
			this.textDateEntry.TabIndex = 10;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(88, 40);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(62, 17);
			this.label10.TabIndex = 90;
			this.label10.Text = "Note";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsReturn = true;
			this.textNote.Location = new System.Drawing.Point(154, 40);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(304, 86);
			this.textNote.TabIndex = 0;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(19, 132);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(134, 20);
			this.label8.TabIndex = 92;
			this.label8.Text = "Max Prescheduled Off";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMax
			// 
			this.textMax.Location = new System.Drawing.Point(154, 132);
			this.textMax.Name = "textMax";
			this.textMax.Size = new System.Drawing.Size(46, 20);
			this.textMax.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(203, 132);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(261, 20);
			this.label2.TabIndex = 93;
			this.label2.Text = "(this limit only applies to those normally graphed)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormPhoneGraphAdminEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(502, 235);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textMax);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPhoneGraphAdminEdit";
			this.Text = "Phone Graph Admin Edit";
			this.Load += new System.EventHandler(this.FormPhoneGraphAdmin_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private ValidDate textDateEntry;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textMax;
		private System.Windows.Forms.Label label2;
	}
}