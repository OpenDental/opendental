namespace OpenDental {
	partial class FormMapAreaEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMapAreaEdit));
			this.labelField1 = new System.Windows.Forms.Label();
			this.labelField2 = new System.Windows.Forms.Label();
			this.labelHeight = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.labelDescription = new System.Windows.Forms.Label();
			this.labelExtension = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textXPos = new OpenDental.ValidDouble();
			this.textYPos = new OpenDental.ValidDouble();
			this.textWidth = new OpenDental.ValidDouble();
			this.textHeight = new OpenDental.ValidDouble();
			this.textExtension = new OpenDental.ValidNum();
			this.butPick = new OpenDental.UI.Button();
			this.textName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.panelCubicle = new System.Windows.Forms.Panel();
			this.labelDescriptionExample = new System.Windows.Forms.Label();
			this.panelLabel = new System.Windows.Forms.Panel();
			this.label3 = new System.Windows.Forms.Label();
			this.textFontSize = new OpenDental.ValidDouble();
			this.label1 = new System.Windows.Forms.Label();
			this.panelCubicle.SuspendLayout();
			this.panelLabel.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelField1
			// 
			this.labelField1.Location = new System.Drawing.Point(23, 45);
			this.labelField1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelField1.Name = "labelField1";
			this.labelField1.Size = new System.Drawing.Size(77, 16);
			this.labelField1.TabIndex = 0;
			this.labelField1.Text = "X Position";
			this.labelField1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelField2
			// 
			this.labelField2.Location = new System.Drawing.Point(23, 67);
			this.labelField2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelField2.Name = "labelField2";
			this.labelField2.Size = new System.Drawing.Size(77, 16);
			this.labelField2.TabIndex = 2;
			this.labelField2.Text = "Y Position";
			this.labelField2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(5, 24);
			this.labelHeight.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(71, 16);
			this.labelHeight.TabIndex = 8;
			this.labelHeight.Text = "Height";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 1);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Width";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(104, 88);
			this.textDescription.Margin = new System.Windows.Forms.Padding(2);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(113, 20);
			this.textDescription.TabIndex = 2;
			// 
			// labelDescription
			// 
			this.labelDescription.Location = new System.Drawing.Point(23, 88);
			this.labelDescription.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(77, 16);
			this.labelDescription.TabIndex = 10;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelExtension
			// 
			this.labelExtension.Location = new System.Drawing.Point(5, 46);
			this.labelExtension.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelExtension.Name = "labelExtension";
			this.labelExtension.Size = new System.Drawing.Size(71, 16);
			this.labelExtension.TabIndex = 12;
			this.labelExtension.Text = "Extension";
			this.labelExtension.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 276);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 18;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(268, 276);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 16;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(352, 276);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 17;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textXPos
			// 
			this.textXPos.Location = new System.Drawing.Point(104, 44);
			this.textXPos.MaxVal = 100000000D;
			this.textXPos.MinVal = 0D;
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(43, 20);
			this.textXPos.TabIndex = 19;
			// 
			// textYPos
			// 
			this.textYPos.Location = new System.Drawing.Point(104, 66);
			this.textYPos.MaxVal = 100000000D;
			this.textYPos.MinVal = 0D;
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(43, 20);
			this.textYPos.TabIndex = 20;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(80, 1);
			this.textWidth.MaxVal = 100000000D;
			this.textWidth.MinVal = 0D;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(43, 20);
			this.textWidth.TabIndex = 21;
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(80, 23);
			this.textHeight.MaxVal = 100000000D;
			this.textHeight.MinVal = 0D;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(43, 20);
			this.textHeight.TabIndex = 25;
			// 
			// textExtension
			// 
			this.textExtension.Location = new System.Drawing.Point(80, 45);
			this.textExtension.MaxVal = 1000000000;
			this.textExtension.Name = "textExtension";
			this.textExtension.Size = new System.Drawing.Size(58, 20);
			this.textExtension.TabIndex = 27;
			this.textExtension.TextChanged += new System.EventHandler(this.textBoxExtension_TextChanged);
			// 
			// butPick
			// 
			this.butPick.Location = new System.Drawing.Point(144, 43);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(58, 24);
			this.butPick.TabIndex = 28;
			this.butPick.Text = "Pick";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(80, 73);
			this.textName.Margin = new System.Windows.Forms.Padding(2);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(113, 20);
			this.textName.TabIndex = 29;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 73);
			this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 16);
			this.label6.TabIndex = 30;
			this.label6.Text = "Name";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(197, 77);
			this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(139, 17);
			this.label7.TabIndex = 31;
			this.label7.Text = "derived from extension";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(79, 18);
			this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(212, 16);
			this.label8.TabIndex = 32;
			this.label8.Text = "All dimensions are in feet";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panelCubicle
			// 
			this.panelCubicle.Controls.Add(this.textWidth);
			this.panelCubicle.Controls.Add(this.label2);
			this.panelCubicle.Controls.Add(this.label7);
			this.panelCubicle.Controls.Add(this.labelHeight);
			this.panelCubicle.Controls.Add(this.textName);
			this.panelCubicle.Controls.Add(this.labelExtension);
			this.panelCubicle.Controls.Add(this.label6);
			this.panelCubicle.Controls.Add(this.textHeight);
			this.panelCubicle.Controls.Add(this.butPick);
			this.panelCubicle.Controls.Add(this.textExtension);
			this.panelCubicle.Location = new System.Drawing.Point(24, 109);
			this.panelCubicle.Name = "panelCubicle";
			this.panelCubicle.Size = new System.Drawing.Size(340, 99);
			this.panelCubicle.TabIndex = 33;
			// 
			// labelDescriptionExample
			// 
			this.labelDescriptionExample.Location = new System.Drawing.Point(221, 91);
			this.labelDescriptionExample.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.labelDescriptionExample.Name = "labelDescriptionExample";
			this.labelDescriptionExample.Size = new System.Drawing.Size(161, 17);
			this.labelDescriptionExample.TabIndex = 32;
			this.labelDescriptionExample.Text = "example: B2 5:3";
			// 
			// panelLabel
			// 
			this.panelLabel.Controls.Add(this.label3);
			this.panelLabel.Controls.Add(this.textFontSize);
			this.panelLabel.Controls.Add(this.label1);
			this.panelLabel.Location = new System.Drawing.Point(24, 218);
			this.panelLabel.Name = "panelLabel";
			this.panelLabel.Size = new System.Drawing.Size(340, 28);
			this.panelLabel.TabIndex = 34;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(128, 7);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(191, 17);
			this.label3.TabIndex = 32;
			this.label3.Text = "Default 14. Must be 5 or bigger.";
			// 
			// textFontSize
			// 
			this.textFontSize.Location = new System.Drawing.Point(80, 4);
			this.textFontSize.MaxVal = 100000000D;
			this.textFontSize.MinVal = 5D;
			this.textFontSize.Name = "textFontSize";
			this.textFontSize.Size = new System.Drawing.Size(43, 20);
			this.textFontSize.TabIndex = 21;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 4);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 16);
			this.label1.TabIndex = 6;
			this.label1.Text = "Font Size";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormMapAreaEdit
			// 
			this.ClientSize = new System.Drawing.Size(434, 305);
			this.Controls.Add(this.panelLabel);
			this.Controls.Add(this.labelDescriptionExample);
			this.Controls.Add(this.panelCubicle);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textYPos);
			this.Controls.Add(this.textXPos);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.labelDescription);
			this.Controls.Add(this.labelField2);
			this.Controls.Add(this.labelField1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMapAreaEdit";
			this.Text = "Edit Map Area";
			this.Load += new System.EventHandler(this.FormMapAreaEdit_Load);
			this.panelCubicle.ResumeLayout(false);
			this.panelCubicle.PerformLayout();
			this.panelLabel.ResumeLayout(false);
			this.panelLabel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelField1;
		private System.Windows.Forms.Label labelField2;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.Label labelExtension;
		private UI.Button butDelete;
		private UI.Button butOK;
		private UI.Button butCancel;
		private ValidDouble textXPos;
		private ValidDouble textYPos;
		private ValidDouble textWidth;
		private ValidDouble textHeight;
		private ValidNum textExtension;
		private UI.Button butPick;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel panelCubicle;
		private System.Windows.Forms.Label labelDescriptionExample;
		private System.Windows.Forms.Panel panelLabel;
		private System.Windows.Forms.Label label3;
		private ValidDouble textFontSize;
		private System.Windows.Forms.Label label1;
	}
}