namespace OpenDental{
	partial class FormCertEmployee{
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCertEmployee));
			this.labelEmployee = new System.Windows.Forms.Label();
			this.textEmployee = new System.Windows.Forms.TextBox();
			this.labelCertCategories = new System.Windows.Forms.Label();
			this.textCertCategories = new System.Windows.Forms.TextBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.labelDate = new System.Windows.Forms.Label();
			this.labelCertification = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textCertification = new System.Windows.Forms.TextBox();
			this.textDateCompleted = new OpenDental.ValidDate();
			this.butToday = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelEmployee
			// 
			this.labelEmployee.Location = new System.Drawing.Point(12, 27);
			this.labelEmployee.Name = "labelEmployee";
			this.labelEmployee.Size = new System.Drawing.Size(102, 17);
			this.labelEmployee.TabIndex = 9;
			this.labelEmployee.Text = "Employee";
			this.labelEmployee.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEmployee
			// 
			this.textEmployee.Location = new System.Drawing.Point(115, 23);
			this.textEmployee.Name = "textEmployee";
			this.textEmployee.ReadOnly = true;
			this.textEmployee.Size = new System.Drawing.Size(227, 20);
			this.textEmployee.TabIndex = 6;
			// 
			// labelCertCategories
			// 
			this.labelCertCategories.Location = new System.Drawing.Point(12, 52);
			this.labelCertCategories.Name = "labelCertCategories";
			this.labelCertCategories.Size = new System.Drawing.Size(102, 17);
			this.labelCertCategories.TabIndex = 10;
			this.labelCertCategories.Text = "Cert Categories";
			this.labelCertCategories.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCertCategories
			// 
			this.textCertCategories.Location = new System.Drawing.Point(115, 48);
			this.textCertCategories.Name = "textCertCategories";
			this.textCertCategories.ReadOnly = true;
			this.textCertCategories.Size = new System.Drawing.Size(227, 20);
			this.textCertCategories.TabIndex = 7;
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(12, 127);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(102, 17);
			this.labelNote.TabIndex = 13;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(115, 123);
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(227, 20);
			this.textNote.TabIndex = 1;
			// 
			// labelDate
			// 
			this.labelDate.Location = new System.Drawing.Point(12, 102);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(102, 17);
			this.labelDate.TabIndex = 12;
			this.labelDate.Text = "Date Completed";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelCertification
			// 
			this.labelCertification.Location = new System.Drawing.Point(12, 77);
			this.labelCertification.Name = "labelCertification";
			this.labelCertification.Size = new System.Drawing.Size(102, 17);
			this.labelCertification.TabIndex = 11;
			this.labelCertification.Text = "Certification";
			this.labelCertification.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 158);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(209, 158);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(290, 158);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textCertification
			// 
			this.textCertification.Location = new System.Drawing.Point(115, 73);
			this.textCertification.Name = "textCertification";
			this.textCertification.ReadOnly = true;
			this.textCertification.Size = new System.Drawing.Size(227, 20);
			this.textCertification.TabIndex = 8;
			// 
			// textDateCompleted
			// 
			this.textDateCompleted.Location = new System.Drawing.Point(115, 98);
			this.textDateCompleted.Name = "textDateCompleted";
			this.textDateCompleted.Size = new System.Drawing.Size(87, 20);
			this.textDateCompleted.TabIndex = 0;
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(208, 95);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(75, 24);
			this.butToday.TabIndex = 4;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// FormCertEmployee
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(377, 199);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelEmployee);
			this.Controls.Add(this.textEmployee);
			this.Controls.Add(this.labelCertCategories);
			this.Controls.Add(this.textCertCategories);
			this.Controls.Add(this.labelCertification);
			this.Controls.Add(this.textCertification);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.textDateCompleted);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.textNote);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCertEmployee";
			this.Text = "Employee Certification";
			this.Load += new System.EventHandler(this.FormCertEmployee_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.Label labelEmployee;
		private System.Windows.Forms.TextBox textEmployee;
		private System.Windows.Forms.Label labelNote;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label labelDate;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelCertification;
		private System.Windows.Forms.Label labelCertCategories;
		private System.Windows.Forms.TextBox textCertCategories;
		private System.Windows.Forms.TextBox textCertification;
		private ValidDate textDateCompleted;
		private UI.Button butToday;
	}
}