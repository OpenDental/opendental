namespace OpenDental{
	partial class FormDocumentSizeMount {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDocumentSizeMount));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkIsFlipped = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textDegreesRotated = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butFit = new OpenDental.UI.Button();
			this.labelCrop = new System.Windows.Forms.Label();
			this.textZoomFit = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textRawSize = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textMountSize = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.butExpandFill = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textZoomOrig = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.but100 = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(280, 210);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(362, 210);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkIsFlipped
			// 
			this.checkIsFlipped.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsFlipped.Location = new System.Drawing.Point(5, 73);
			this.checkIsFlipped.Name = "checkIsFlipped";
			this.checkIsFlipped.Size = new System.Drawing.Size(152, 18);
			this.checkIsFlipped.TabIndex = 4;
			this.checkIsFlipped.Text = "Is Flipped Horizontally";
			this.checkIsFlipped.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsFlipped.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(163, 74);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(252, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "(to flip vertically, combine this with a 180 rotation)";
			// 
			// textDegreesRotated
			// 
			this.textDegreesRotated.Location = new System.Drawing.Point(142, 97);
			this.textDegreesRotated.Name = "textDegreesRotated";
			this.textDegreesRotated.Size = new System.Drawing.Size(43, 20);
			this.textDegreesRotated.TabIndex = 131;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(33, 98);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 16);
			this.label4.TabIndex = 130;
			this.label4.Text = "Degrees Rotated";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(190, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(252, 18);
			this.label2.TabIndex = 132;
			this.label2.Text = "(only allowed 0,90,180, and 270)";
			// 
			// butFit
			// 
			this.butFit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butFit.Location = new System.Drawing.Point(94, 210);
			this.butFit.Name = "butFit";
			this.butFit.Size = new System.Drawing.Size(63, 24);
			this.butFit.TabIndex = 134;
			this.butFit.Text = "Fit";
			this.butFit.Click += new System.EventHandler(this.butFit_Click);
			// 
			// labelCrop
			// 
			this.labelCrop.Location = new System.Drawing.Point(59, 126);
			this.labelCrop.Name = "labelCrop";
			this.labelCrop.Size = new System.Drawing.Size(82, 18);
			this.labelCrop.TabIndex = 133;
			this.labelCrop.Text = "Zoom Fit";
			this.labelCrop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textZoomFit
			// 
			this.textZoomFit.Location = new System.Drawing.Point(142, 125);
			this.textZoomFit.Name = "textZoomFit";
			this.textZoomFit.Size = new System.Drawing.Size(43, 20);
			this.textZoomFit.TabIndex = 138;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(191, 129);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(232, 18);
			this.label5.TabIndex = 139;
			this.label5.Text = "(relative to Fit)";
			// 
			// textRawSize
			// 
			this.textRawSize.Location = new System.Drawing.Point(142, 16);
			this.textRawSize.Name = "textRawSize";
			this.textRawSize.ReadOnly = true;
			this.textRawSize.Size = new System.Drawing.Size(94, 20);
			this.textRawSize.TabIndex = 141;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(36, 17);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(105, 18);
			this.label6.TabIndex = 140;
			this.label6.Text = "Raw Image Size";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMountSize
			// 
			this.textMountSize.Location = new System.Drawing.Point(142, 42);
			this.textMountSize.Name = "textMountSize";
			this.textMountSize.ReadOnly = true;
			this.textMountSize.Size = new System.Drawing.Size(94, 20);
			this.textMountSize.TabIndex = 143;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(36, 43);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(105, 18);
			this.label7.TabIndex = 142;
			this.label7.Text = "Mount Item Size";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(242, 45);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(124, 18);
			this.label8.TabIndex = 144;
			this.label8.Text = "(pixels)";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(242, 19);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(113, 18);
			this.label10.TabIndex = 146;
			this.label10.Text = "(pixels)";
			// 
			// butExpandFill
			// 
			this.butExpandFill.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExpandFill.Location = new System.Drawing.Point(163, 210);
			this.butExpandFill.Name = "butExpandFill";
			this.butExpandFill.Size = new System.Drawing.Size(74, 24);
			this.butExpandFill.TabIndex = 147;
			this.butExpandFill.Text = "Expand Fill";
			this.butExpandFill.Click += new System.EventHandler(this.butExpandFill_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(191, 155);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(232, 18);
			this.label3.TabIndex = 150;
			this.label3.Text = "(relative to original image)";
			// 
			// textZoomOrig
			// 
			this.textZoomOrig.Location = new System.Drawing.Point(142, 151);
			this.textZoomOrig.Name = "textZoomOrig";
			this.textZoomOrig.ReadOnly = true;
			this.textZoomOrig.Size = new System.Drawing.Size(43, 20);
			this.textZoomOrig.TabIndex = 149;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(36, 152);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(105, 18);
			this.label9.TabIndex = 148;
			this.label9.Text = "Zoom Orig";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// but100
			// 
			this.but100.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.but100.Location = new System.Drawing.Point(25, 210);
			this.but100.Name = "but100";
			this.but100.Size = new System.Drawing.Size(63, 24);
			this.but100.TabIndex = 151;
			this.but100.Text = "100% Pix";
			this.but100.Click += new System.EventHandler(this.but100_Click);
			// 
			// FormDocumentSizeMount
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(449, 246);
			this.Controls.Add(this.but100);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textZoomOrig);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butExpandFill);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textMountSize);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textRawSize);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textZoomFit);
			this.Controls.Add(this.butFit);
			this.Controls.Add(this.labelCrop);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDegreesRotated);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkIsFlipped);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDocumentSizeMount";
			this.Text = "Item Size on Mount";
			this.Load += new System.EventHandler(this.FormDocumentSizeMount_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkIsFlipped;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDegreesRotated;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private UI.Button butFit;
		private System.Windows.Forms.Label labelCrop;
		private System.Windows.Forms.TextBox textZoomFit;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textRawSize;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textMountSize;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private UI.Button butExpandFill;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textZoomOrig;
		private System.Windows.Forms.Label label9;
		private UI.Button but100;
	}
}