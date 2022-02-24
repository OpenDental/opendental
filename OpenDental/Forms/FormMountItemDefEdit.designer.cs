namespace OpenDental{
	partial class FormMountItemDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMountItemDefEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textHeight = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.textWidth = new OpenDental.ValidNum();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textYpos = new OpenDental.ValidNum();
			this.label1 = new System.Windows.Forms.Label();
			this.textXpos = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.textRotate = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textToothNumbers = new System.Windows.Forms.TextBox();
			this.labelToothNums = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(267, 196);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(348, 196);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(164, 96);
			this.textHeight.MaxVal = 10000;
			this.textHeight.MinVal = 1;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(48, 20);
			this.textHeight.TabIndex = 33;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(110, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(53, 17);
			this.label4.TabIndex = 32;
			this.label4.Text = "Height";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(164, 73);
			this.textWidth.MaxVal = 20000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(48, 20);
			this.textWidth.TabIndex = 31;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(110, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(53, 17);
			this.label3.TabIndex = 30;
			this.label3.Text = "Width";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(13, 196);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 34;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textYpos
			// 
			this.textYpos.Location = new System.Drawing.Point(164, 50);
			this.textYpos.MaxVal = 10000;
			this.textYpos.Name = "textYpos";
			this.textYpos.Size = new System.Drawing.Size(48, 20);
			this.textYpos.TabIndex = 38;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(109, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 17);
			this.label1.TabIndex = 37;
			this.label1.Text = "Y";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textXpos
			// 
			this.textXpos.Location = new System.Drawing.Point(164, 27);
			this.textXpos.MaxVal = 20000;
			this.textXpos.Name = "textXpos";
			this.textXpos.Size = new System.Drawing.Size(48, 20);
			this.textXpos.TabIndex = 36;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(109, 27);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 17);
			this.label2.TabIndex = 35;
			this.label2.Text = "X";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// textRotate
			// 
			this.textRotate.Location = new System.Drawing.Point(164, 119);
			this.textRotate.MaxVal = 300;
			this.textRotate.Name = "textRotate";
			this.textRotate.ShowZero = false;
			this.textRotate.Size = new System.Drawing.Size(48, 20);
			this.textRotate.TabIndex = 40;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 119);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(161, 17);
			this.label5.TabIndex = 39;
			this.label5.Text = "Rotate degrees when Acquire";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(215, 122);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(193, 17);
			this.label6.TabIndex = 41;
			this.label6.Text = "Valid values are 0, 90, 180, or 270";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textToothNumbers
			// 
			this.textToothNumbers.Location = new System.Drawing.Point(164, 142);
			this.textToothNumbers.Name = "textToothNumbers";
			this.textToothNumbers.Size = new System.Drawing.Size(106, 20);
			this.textToothNumbers.TabIndex = 43;
			// 
			// labelToothNums
			// 
			this.labelToothNums.Location = new System.Drawing.Point(63, 144);
			this.labelToothNums.Name = "labelToothNums";
			this.labelToothNums.Size = new System.Drawing.Size(100, 18);
			this.labelToothNums.TabIndex = 42;
			this.labelToothNums.Text = "Tooth Numbers";
			this.labelToothNums.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormMountItemDefEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(435, 232);
			this.Controls.Add(this.textToothNumbers);
			this.Controls.Add(this.labelToothNums);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textRotate);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textYpos);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textXpos);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMountItemDefEdit";
			this.Text = "Edit Mount Item Def";
			this.Load += new System.EventHandler(this.FormMountItemDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private ValidNum textHeight;
		private System.Windows.Forms.Label label4;
		private ValidNum textWidth;
		private System.Windows.Forms.Label label3;
		private UI.Button butDelete;
		private ValidNum textYpos;
		private System.Windows.Forms.Label label1;
		private ValidNum textXpos;
		private System.Windows.Forms.Label label2;
		private ValidNum textRotate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textToothNumbers;
		private System.Windows.Forms.Label labelToothNums;
	}
}