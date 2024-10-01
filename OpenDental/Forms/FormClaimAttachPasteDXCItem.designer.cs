namespace OpenDental {
	partial class FormClaimAttachPasteDXCItem {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimAttachPasteDXCItem));
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.listBoxImageType = new OpenDental.UI.ListBox();
			this.textFileName = new System.Windows.Forms.TextBox();
			this.labelDateTimeCreate = new System.Windows.Forms.Label();
			this.textDateCreated = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.labelImageType = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(200, 269);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 267);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// listBoxImageType
			// 
			this.listBoxImageType.Location = new System.Drawing.Point(107, 82);
			this.listBoxImageType.Name = "listBoxImageType";
			this.listBoxImageType.Size = new System.Drawing.Size(139, 145);
			this.listBoxImageType.TabIndex = 10;
			this.listBoxImageType.Text = "Image Type";
			// 
			// textFileName
			// 
			this.textFileName.Location = new System.Drawing.Point(107, 26);
			this.textFileName.Name = "textFileName";
			this.textFileName.Size = new System.Drawing.Size(139, 20);
			this.textFileName.TabIndex = 7;
			this.textFileName.Text = "Attachment";
			// 
			// labelDateTimeCreate
			// 
			this.labelDateTimeCreate.Location = new System.Drawing.Point(22, 56);
			this.labelDateTimeCreate.Name = "labelDateTimeCreate";
			this.labelDateTimeCreate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDateTimeCreate.Size = new System.Drawing.Size(84, 15);
			this.labelDateTimeCreate.TabIndex = 12;
			this.labelDateTimeCreate.Text = "Date Created";
			// 
			// textDateCreated
			// 
			this.textDateCreated.Location = new System.Drawing.Point(107, 53);
			this.textDateCreated.Name = "textDateCreated";
			this.textDateCreated.Size = new System.Drawing.Size(139, 20);
			this.textDateCreated.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 29);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(84, 15);
			this.label1.TabIndex = 11;
			this.label1.Text = "File Name";
			// 
			// labelImageType
			// 
			this.labelImageType.Location = new System.Drawing.Point(22, 82);
			this.labelImageType.Name = "labelImageType";
			this.labelImageType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelImageType.Size = new System.Drawing.Size(84, 15);
			this.labelImageType.TabIndex = 9;
			this.labelImageType.Text = "Image Type";
			// 
			// FormClaimAttachPasteDXCItem
			// 
			this.ClientSize = new System.Drawing.Size(294, 305);
			this.Controls.Add(this.listBoxImageType);
			this.Controls.Add(this.textFileName);
			this.Controls.Add(this.labelDateTimeCreate);
			this.Controls.Add(this.textDateCreated);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelImageType);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimAttachPasteDXCItem";
			this.Text = "Edit Attachment Item";
			this.Load += new System.EventHandler(this.FormClaimAttachPasteDXCItem_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butSave;
		private UI.Button butDelete;
		private UI.ListBox listBoxImageType;
		private System.Windows.Forms.TextBox textFileName;
		private System.Windows.Forms.Label labelDateTimeCreate;
		private ValidDate textDateCreated;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelImageType;
	}
}