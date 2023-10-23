namespace OpenDental{
	partial class FormDocumentSize {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDocumentSize));
			this.butSave = new OpenDental.UI.Button();
			this.checkIsFlipped = new OpenDental.UI.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textDegreesRotated = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.labelCropInfo = new System.Windows.Forms.Label();
			this.butCropReset = new OpenDental.UI.Button();
			this.labelCrop = new System.Windows.Forms.Label();
			this.butResetAll = new OpenDental.UI.Button();
			this.labelResetAll = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textRawSize = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(373, 187);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkIsFlipped
			// 
			this.checkIsFlipped.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsFlipped.Location = new System.Drawing.Point(27, 43);
			this.checkIsFlipped.Name = "checkIsFlipped";
			this.checkIsFlipped.Size = new System.Drawing.Size(152, 18);
			this.checkIsFlipped.TabIndex = 4;
			this.checkIsFlipped.Text = "Is Flipped Horizontally";
			this.checkIsFlipped.CheckedChanged += new System.EventHandler(this.checkIsFlipped_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(185, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(252, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "(to flip vertically, combine this with a 180 rotation)";
			// 
			// textDegreesRotated
			// 
			this.textDegreesRotated.Location = new System.Drawing.Point(164, 67);
			this.textDegreesRotated.Name = "textDegreesRotated";
			this.textDegreesRotated.Size = new System.Drawing.Size(43, 20);
			this.textDegreesRotated.TabIndex = 131;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(55, 68);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 16);
			this.label4.TabIndex = 130;
			this.label4.Text = "Degrees Rotated";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCropInfo
			// 
			this.labelCropInfo.Location = new System.Drawing.Point(234, 100);
			this.labelCropInfo.Name = "labelCropInfo";
			this.labelCropInfo.Size = new System.Drawing.Size(250, 18);
			this.labelCropInfo.TabIndex = 135;
			this.labelCropInfo.Text = "(this image has no crop applied)";
			// 
			// butCropReset
			// 
			this.butCropReset.Location = new System.Drawing.Point(164, 94);
			this.butCropReset.Name = "butCropReset";
			this.butCropReset.Size = new System.Drawing.Size(64, 24);
			this.butCropReset.TabIndex = 134;
			this.butCropReset.Text = "Reset";
			this.butCropReset.Click += new System.EventHandler(this.butCropReset_Click);
			// 
			// labelCrop
			// 
			this.labelCrop.Location = new System.Drawing.Point(81, 99);
			this.labelCrop.Name = "labelCrop";
			this.labelCrop.Size = new System.Drawing.Size(82, 18);
			this.labelCrop.TabIndex = 133;
			this.labelCrop.Text = "Crop";
			this.labelCrop.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butResetAll
			// 
			this.butResetAll.Location = new System.Drawing.Point(164, 125);
			this.butResetAll.Name = "butResetAll";
			this.butResetAll.Size = new System.Drawing.Size(64, 24);
			this.butResetAll.TabIndex = 136;
			this.butResetAll.Text = "Reset All";
			this.butResetAll.Click += new System.EventHandler(this.butResetAll_Click);
			// 
			// labelResetAll
			// 
			this.labelResetAll.Location = new System.Drawing.Point(234, 130);
			this.labelResetAll.Name = "labelResetAll";
			this.labelResetAll.Size = new System.Drawing.Size(235, 18);
			this.labelResetAll.TabIndex = 137;
			this.labelResetAll.Text = "(this image has no crop, flip, or rotate applied)";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(264, 18);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(113, 18);
			this.label10.TabIndex = 149;
			this.label10.Text = "(pixels)";
			// 
			// textRawSize
			// 
			this.textRawSize.Location = new System.Drawing.Point(164, 15);
			this.textRawSize.Name = "textRawSize";
			this.textRawSize.ReadOnly = true;
			this.textRawSize.Size = new System.Drawing.Size(94, 20);
			this.textRawSize.TabIndex = 148;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(58, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 18);
			this.label6.TabIndex = 147;
			this.label6.Text = "Raw Image Size";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDocumentSize
			// 
			this.ClientSize = new System.Drawing.Size(469, 223);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textRawSize);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.labelResetAll);
			this.Controls.Add(this.butResetAll);
			this.Controls.Add(this.labelCropInfo);
			this.Controls.Add(this.butCropReset);
			this.Controls.Add(this.labelCrop);
			this.Controls.Add(this.textDegreesRotated);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkIsFlipped);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDocumentSize";
			this.Text = "Item Size";
			this.Load += new System.EventHandler(this.FormDocumentSize_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkIsFlipped;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDegreesRotated;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelCropInfo;
		private UI.Button butCropReset;
		private System.Windows.Forms.Label labelCrop;
		private UI.Button butResetAll;
		private System.Windows.Forms.Label labelResetAll;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textRawSize;
		private System.Windows.Forms.Label label6;
	}
}