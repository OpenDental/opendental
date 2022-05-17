namespace OpenDental{
	partial class FormIcd9Edit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormIcd9Edit));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textCode = new System.Windows.Forms.TextBox();
			this.buttonDelete = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.service11 = new OpenDental.localhost.Service1();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(9, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(109, 45);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(317, 20);
			this.textDescription.TabIndex = 1;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(109, 19);
			this.textCode.Name = "textCode";
			this.textCode.ReadOnly = true;
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 0;
			// 
			// buttonDelete
			// 
			this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.buttonDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonDelete.Location = new System.Drawing.Point(22, 97);
			this.buttonDelete.Name = "buttonDelete";
			this.buttonDelete.Size = new System.Drawing.Size(82, 25);
			this.buttonDelete.TabIndex = 6;
			this.buttonDelete.Text = "&Delete";
			this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(352, 98);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// service11
			// 
			this.service11.Url = "http://localhost:3824/Service1.asmx";
			this.service11.UseDefaultCredentials = true;
			// 
			// FormIcd9Edit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(452, 144);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.buttonDelete);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormIcd9Edit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ICD9 Edit";
			this.Load += new System.EventHandler(this.FormIcd9Edit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescription;
		private UI.Button buttonDelete;
		private System.Windows.Forms.TextBox textCode;
		private localhost.Service1 service11;
	}
}