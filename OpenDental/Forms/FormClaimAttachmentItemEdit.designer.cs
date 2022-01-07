namespace OpenDental{
	partial class FormClaimAttachmentItemEdit {
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
			this.labelImageType = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDateCreated = new OpenDental.ValidDate();
			this.labelDateTimeCreate = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textFileName = new System.Windows.Forms.TextBox();
			this.listBoxImageType = new OpenDental.UI.ListBoxOD();
			this.checkIsXrayMirrored = new System.Windows.Forms.CheckBox();
			this.pictureBoxImagePreview = new System.Windows.Forms.PictureBox();
			this.butNewSnip = new OpenDental.UI.Button();
			this.labelNewSnip = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).BeginInit();
			this.SuspendLayout();
			// 
			// labelImageType
			// 
			this.labelImageType.Location = new System.Drawing.Point(54, 97);
			this.labelImageType.Name = "labelImageType";
			this.labelImageType.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelImageType.Size = new System.Drawing.Size(84, 15);
			this.labelImageType.TabIndex = 1;
			this.labelImageType.Text = "Image Type";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(54, 44);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.label1.Size = new System.Drawing.Size(84, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "File Name";
			// 
			// textDateCreated
			// 
			this.textDateCreated.Location = new System.Drawing.Point(139, 68);
			this.textDateCreated.Name = "textDateCreated";
			this.textDateCreated.Size = new System.Drawing.Size(121, 20);
			this.textDateCreated.TabIndex = 1;
			// 
			// labelDateTimeCreate
			// 
			this.labelDateTimeCreate.Location = new System.Drawing.Point(54, 71);
			this.labelDateTimeCreate.Name = "labelDateTimeCreate";
			this.labelDateTimeCreate.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDateTimeCreate.Size = new System.Drawing.Size(84, 15);
			this.labelDateTimeCreate.TabIndex = 6;
			this.labelDateTimeCreate.Text = "Date Created";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(390, 302);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(471, 302);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// textFileName
			// 
			this.textFileName.Location = new System.Drawing.Point(139, 41);
			this.textFileName.Name = "textFileName";
			this.textFileName.Size = new System.Drawing.Size(121, 20);
			this.textFileName.TabIndex = 0;
			this.textFileName.Text = "Attachment";
			// 
			// listBoxImageType
			// 
			this.listBoxImageType.Location = new System.Drawing.Point(139, 97);
			this.listBoxImageType.Name = "listBoxImageType";
			this.listBoxImageType.Size = new System.Drawing.Size(120, 134);
			this.listBoxImageType.TabIndex = 2;
			this.listBoxImageType.Text = "Image Type";
			// 
			// checkIsXrayMirrored
			// 
			this.checkIsXrayMirrored.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsXrayMirrored.Location = new System.Drawing.Point(29, 238);
			this.checkIsXrayMirrored.Name = "checkIsXrayMirrored";
			this.checkIsXrayMirrored.Size = new System.Drawing.Size(124, 18);
			this.checkIsXrayMirrored.TabIndex = 3;
			this.checkIsXrayMirrored.Text = "Is xray mirror image";
			this.checkIsXrayMirrored.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsXrayMirrored.UseVisualStyleBackColor = true;
			// 
			// pictureBoxImagePreview
			// 
			this.pictureBoxImagePreview.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBoxImagePreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxImagePreview.Location = new System.Drawing.Point(323, 41);
			this.pictureBoxImagePreview.Name = "pictureBoxImagePreview";
			this.pictureBoxImagePreview.Size = new System.Drawing.Size(190, 190);
			this.pictureBoxImagePreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxImagePreview.TabIndex = 9;
			this.pictureBoxImagePreview.TabStop = false;
			// 
			// butNewSnip
			// 
			this.butNewSnip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butNewSnip.Location = new System.Drawing.Point(390, 262);
			this.butNewSnip.Name = "butNewSnip";
			this.butNewSnip.Size = new System.Drawing.Size(75, 24);
			this.butNewSnip.TabIndex = 6;
			this.butNewSnip.Text = "OK";
			this.butNewSnip.UseVisualStyleBackColor = true;
			this.butNewSnip.Click += new System.EventHandler(this.butNewSnip_Click);
			// 
			// labelNewSnip
			// 
			this.labelNewSnip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelNewSnip.Location = new System.Drawing.Point(469, 265);
			this.labelNewSnip.Name = "labelNewSnip";
			this.labelNewSnip.Size = new System.Drawing.Size(116, 15);
			this.labelNewSnip.TabIndex = 10;
			this.labelNewSnip.Text = "(and Snip Another)";
			this.labelNewSnip.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormClaimAttachmentItemEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(584, 338);
			this.Controls.Add(this.labelNewSnip);
			this.Controls.Add(this.butNewSnip);
			this.Controls.Add(this.pictureBoxImagePreview);
			this.Controls.Add(this.checkIsXrayMirrored);
			this.Controls.Add(this.listBoxImageType);
			this.Controls.Add(this.textFileName);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelDateTimeCreate);
			this.Controls.Add(this.textDateCreated);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.labelImageType);
			this.Name = "FormClaimAttachmentItemEdit";
			this.Text = "Image Info";
			this.Load += new System.EventHandler(this.FormClaimAttachmentItemEdit_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxImagePreview)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label labelImageType;
		private System.Windows.Forms.Label label1;
		private ValidDate textDateCreated;
		private System.Windows.Forms.Label labelDateTimeCreate;
		private UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textFileName;
		private UI.ListBoxOD listBoxImageType;
		private System.Windows.Forms.CheckBox checkIsXrayMirrored;
		private System.Windows.Forms.PictureBox pictureBoxImagePreview;
		private UI.Button butNewSnip;
		private System.Windows.Forms.Label labelNewSnip;
	}
}