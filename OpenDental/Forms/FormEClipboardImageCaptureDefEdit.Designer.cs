namespace OpenDental{
	partial class FormEClipboardImageCaptureDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardImageCaptureDefEdit));
			this.butSave = new OpenDental.UI.Button();
			this.labelFrequencyHelp = new System.Windows.Forms.Label();
			this.labelOCRCaptureType = new System.Windows.Forms.Label();
			this.textFrequency = new OpenDental.ValidNum();
			this.butDelete = new OpenDental.UI.Button();
			this.labelImageCaptureDef = new System.Windows.Forms.Label();
			this.labelFrequency = new System.Windows.Forms.Label();
			this.textImage = new System.Windows.Forms.TextBox();
			this.textValue = new System.Windows.Forms.TextBox();
			this.labelValue = new System.Windows.Forms.Label();
			this.listOCRCaptureType = new OpenDental.UI.ListBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(334, 360);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// labelFrequencyHelp
			// 
			this.labelFrequencyHelp.Location = new System.Drawing.Point(12, 132);
			this.labelFrequencyHelp.Name = "labelFrequencyHelp";
			this.labelFrequencyHelp.Size = new System.Drawing.Size(366, 29);
			this.labelFrequencyHelp.TabIndex = 5;
			this.labelFrequencyHelp.Text = "How often should the patient be prompted to resubmit this image (In days, where 0" +
    " or blank indicates at each checkin)?";
			// 
			// labelOCRCaptureType
			// 
			this.labelOCRCaptureType.Location = new System.Drawing.Point(9, 202);
			this.labelOCRCaptureType.Name = "labelOCRCaptureType";
			this.labelOCRCaptureType.Size = new System.Drawing.Size(115, 20);
			this.labelOCRCaptureType.TabIndex = 7;
			this.labelOCRCaptureType.Text = "OCR Capture Type";
			this.labelOCRCaptureType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(130, 163);
			this.textFrequency.MaxVal = 32767;
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.ShowZero = false;
			this.textFrequency.Size = new System.Drawing.Size(71, 20);
			this.textFrequency.TabIndex = 9;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 360);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 81;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelImageCaptureDef
			// 
			this.labelImageCaptureDef.Location = new System.Drawing.Point(12, 12);
			this.labelImageCaptureDef.Name = "labelImageCaptureDef";
			this.labelImageCaptureDef.Size = new System.Drawing.Size(111, 20);
			this.labelImageCaptureDef.TabIndex = 82;
			this.labelImageCaptureDef.Text = "Image Def";
			this.labelImageCaptureDef.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequency
			// 
			this.labelFrequency.Location = new System.Drawing.Point(12, 162);
			this.labelFrequency.Name = "labelFrequency";
			this.labelFrequency.Size = new System.Drawing.Size(111, 20);
			this.labelFrequency.TabIndex = 84;
			this.labelFrequency.Text = "Frequency (Days)";
			this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textImage
			// 
			this.textImage.Location = new System.Drawing.Point(130, 12);
			this.textImage.Name = "textImage";
			this.textImage.ReadOnly = true;
			this.textImage.Size = new System.Drawing.Size(178, 20);
			this.textImage.TabIndex = 86;
			// 
			// textValue
			// 
			this.textValue.Location = new System.Drawing.Point(130, 51);
			this.textValue.MaxLength = 256;
			this.textValue.Multiline = true;
			this.textValue.Name = "textValue";
			this.textValue.ReadOnly = true;
			this.textValue.Size = new System.Drawing.Size(178, 64);
			this.textValue.TabIndex = 89;
			// 
			// labelValue
			// 
			this.labelValue.Location = new System.Drawing.Point(13, 51);
			this.labelValue.Name = "labelValue";
			this.labelValue.Size = new System.Drawing.Size(111, 20);
			this.labelValue.TabIndex = 91;
			this.labelValue.Text = "Value";
			this.labelValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listOCRCaptureType
			// 
			this.listOCRCaptureType.Location = new System.Drawing.Point(128, 204);
			this.listOCRCaptureType.Name = "listOCRCaptureType";
			this.listOCRCaptureType.Size = new System.Drawing.Size(180, 95);
			this.listOCRCaptureType.TabIndex = 92;
			this.listOCRCaptureType.Text = "listOCRCaptureType";
			// 
			// FormEClipboardImageCaptureDefEdit
			// 
			this.ClientSize = new System.Drawing.Size(421, 396);
			this.Controls.Add(this.listOCRCaptureType);
			this.Controls.Add(this.labelValue);
			this.Controls.Add(this.textValue);
			this.Controls.Add(this.textImage);
			this.Controls.Add(this.labelFrequency);
			this.Controls.Add(this.labelImageCaptureDef);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textFrequency);
			this.Controls.Add(this.labelOCRCaptureType);
			this.Controls.Add(this.labelFrequencyHelp);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEClipboardImageCaptureDefEdit";
			this.Text = "eClipboard Image Capture Def Edit";
			this.Load += new System.EventHandler(this.FormEClipboardImageCaptureDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label labelFrequencyHelp;
		private System.Windows.Forms.Label labelOCRCaptureType;
		private ValidNum textFrequency;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelImageCaptureDef;
		private System.Windows.Forms.Label labelFrequency;
		private System.Windows.Forms.TextBox textImage;
		private System.Windows.Forms.TextBox textValue;
		private System.Windows.Forms.Label labelValue;
		private UI.ListBox listOCRCaptureType;
	}
}