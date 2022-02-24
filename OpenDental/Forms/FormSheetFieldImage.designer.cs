namespace OpenDental{
	partial class FormSheetFieldImage {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetFieldImage));
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textFullPath = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.comboFieldName = new System.Windows.Forms.ComboBox();
			this.textWidth2 = new System.Windows.Forms.TextBox();
			this.textHeight2 = new System.Windows.Forms.TextBox();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkRatio = new System.Windows.Forms.CheckBox();
			this.butShrink = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textHeight = new OpenDental.ValidNum();
			this.textWidth = new OpenDental.ValidNum();
			this.textYPos = new OpenDental.ValidNum();
			this.textXPos = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(70, 332);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 16);
			this.label5.TabIndex = 90;
			this.label5.Text = "X Pos";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(70, 358);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 16);
			this.label6.TabIndex = 92;
			this.label6.Text = "Y Pos";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(70, 384);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(71, 16);
			this.label7.TabIndex = 94;
			this.label7.Text = "Width";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(70, 410);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(71, 16);
			this.label8.TabIndex = 96;
			this.label8.Text = "Height";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 16);
			this.label1.TabIndex = 101;
			this.label1.Text = "File Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFullPath
			// 
			this.textFullPath.Location = new System.Drawing.Point(141, 43);
			this.textFullPath.Name = "textFullPath";
			this.textFullPath.ReadOnly = true;
			this.textFullPath.Size = new System.Drawing.Size(434, 20);
			this.textFullPath.TabIndex = 104;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(26, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 16);
			this.label2.TabIndex = 103;
			this.label2.Text = "Full Path";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboFieldName
			// 
			this.comboFieldName.FormattingEnabled = true;
			this.comboFieldName.Location = new System.Drawing.Point(141, 16);
			this.comboFieldName.MaxDropDownItems = 30;
			this.comboFieldName.Name = "comboFieldName";
			this.comboFieldName.Size = new System.Drawing.Size(257, 21);
			this.comboFieldName.TabIndex = 106;
			this.comboFieldName.SelectionChangeCommitted += new System.EventHandler(this.comboFieldName_SelectionChangeCommitted);
			this.comboFieldName.TextUpdate += new System.EventHandler(this.comboFieldName_TextUpdate);
			// 
			// textWidth2
			// 
			this.textWidth2.Location = new System.Drawing.Point(6, 14);
			this.textWidth2.Name = "textWidth2";
			this.textWidth2.ReadOnly = true;
			this.textWidth2.Size = new System.Drawing.Size(51, 20);
			this.textWidth2.TabIndex = 110;
			// 
			// textHeight2
			// 
			this.textHeight2.Location = new System.Drawing.Point(6, 40);
			this.textHeight2.Name = "textHeight2";
			this.textHeight2.ReadOnly = true;
			this.textHeight2.Size = new System.Drawing.Size(51, 20);
			this.textHeight2.TabIndex = 111;
			// 
			// pictureBox
			// 
			this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox.Location = new System.Drawing.Point(141, 69);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(255, 255);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBox.TabIndex = 112;
			this.pictureBox.TabStop = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textWidth2);
			this.groupBox1.Controls.Add(this.textHeight2);
			this.groupBox1.Location = new System.Drawing.Point(198, 369);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(63, 66);
			this.groupBox1.TabIndex = 113;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "File Size";
			// 
			// checkRatio
			// 
			this.checkRatio.Checked = true;
			this.checkRatio.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkRatio.Location = new System.Drawing.Point(267, 413);
			this.checkRatio.Name = "checkRatio";
			this.checkRatio.Size = new System.Drawing.Size(104, 20);
			this.checkRatio.TabIndex = 115;
			this.checkRatio.Text = "Maintain Ratio";
			this.checkRatio.UseVisualStyleBackColor = true;
			// 
			// butShrink
			// 
			this.butShrink.Location = new System.Drawing.Point(267, 381);
			this.butShrink.Name = "butShrink";
			this.butShrink.Size = new System.Drawing.Size(79, 24);
			this.butShrink.TabIndex = 114;
			this.butShrink.Text = "ShrinkToFit";
			this.butShrink.Click += new System.EventHandler(this.butShrink_Click);
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(404, 14);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 24);
			this.butImport.TabIndex = 105;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Image = global::OpenDental.Properties.Resources.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(15, 525);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(77, 24);
			this.butDelete.TabIndex = 100;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(141, 409);
			this.textHeight.MaxVal = 4000;
			this.textHeight.MinVal = 1;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(51, 20);
			this.textHeight.TabIndex = 97;
			this.textHeight.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textHeight_KeyUp);
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(141, 383);
			this.textWidth.MaxVal = 4000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(51, 20);
			this.textWidth.TabIndex = 95;
			this.textWidth.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textWidth_KeyUp);
			// 
			// textYPos
			// 
			this.textYPos.Location = new System.Drawing.Point(141, 357);
			this.textYPos.MaxVal = 20000;
			this.textYPos.MinVal = -100;
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(51, 20);
			this.textYPos.TabIndex = 93;
			// 
			// textXPos
			// 
			this.textXPos.Location = new System.Drawing.Point(141, 331);
			this.textXPos.MaxVal = 2000;
			this.textXPos.MinVal = -100;
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(51, 20);
			this.textXPos.TabIndex = 91;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(514, 495);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(514, 525);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormSheetFieldImage
			// 
			this.ClientSize = new System.Drawing.Size(601, 561);
			this.Controls.Add(this.checkRatio);
			this.Controls.Add(this.butShrink);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.comboFieldName);
			this.Controls.Add(this.butImport);
			this.Controls.Add(this.textFullPath);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textYPos);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textXPos);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetFieldImage";
			this.Text = "Edit Image Field";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSheetFieldImage_FormClosing);
			this.Load += new System.EventHandler(this.FormSheetFieldImage_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label5;
		private ValidNum textXPos;
		private ValidNum textYPos;
		private System.Windows.Forms.Label label6;
		private ValidNum textWidth;
		private System.Windows.Forms.Label label7;
		private ValidNum textHeight;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFullPath;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butImport;
		private System.Windows.Forms.ComboBox comboFieldName;
		private System.Windows.Forms.TextBox textWidth2;
		private System.Windows.Forms.TextBox textHeight2;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.UI.Button butShrink;
		private System.Windows.Forms.CheckBox checkRatio;
	}
}