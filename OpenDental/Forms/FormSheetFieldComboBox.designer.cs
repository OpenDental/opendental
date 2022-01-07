namespace OpenDental{
	partial class FormSheetFieldComboBox {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetFieldComboBox));
			this.textUiLabelMobile = new System.Windows.Forms.TextBox();
			this.labelUiLabelMobile = new System.Windows.Forms.Label();
			this.textReportable = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listComboType = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.textTabOrder = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textOption = new System.Windows.Forms.TextBox();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butRemove = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.listboxComboOptions = new OpenDental.UI.ListBoxOD();
			this.textHeight = new OpenDental.ValidNum();
			this.labelHeight = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.textWidth = new OpenDental.ValidNum();
			this.label7 = new System.Windows.Forms.Label();
			this.textYPos = new OpenDental.ValidNum();
			this.label6 = new System.Windows.Forms.Label();
			this.textXPos = new OpenDental.ValidNum();
			this.label5 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textUiLabelMobile
			// 
			this.textUiLabelMobile.Location = new System.Drawing.Point(100, 114);
			this.textUiLabelMobile.Name = "textUiLabelMobile";
			this.textUiLabelMobile.Size = new System.Drawing.Size(250, 20);
			this.textUiLabelMobile.TabIndex = 125;
			// 
			// labelUiLabelMobile
			// 
			this.labelUiLabelMobile.Location = new System.Drawing.Point(5, 114);
			this.labelUiLabelMobile.Name = "labelUiLabelMobile";
			this.labelUiLabelMobile.Size = new System.Drawing.Size(89, 16);
			this.labelUiLabelMobile.TabIndex = 126;
			this.labelUiLabelMobile.Text = "Mobile Caption";
			this.labelUiLabelMobile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReportable
			// 
			this.textReportable.Location = new System.Drawing.Point(302, 32);
			this.textReportable.Name = "textReportable";
			this.textReportable.Size = new System.Drawing.Size(69, 20);
			this.textReportable.TabIndex = 5;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(77, 163);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(149, 71);
			this.label4.TabIndex = 124;
			this.label4.Text = "Choose a combo box type.  Patient Race, Patient Grade, and Urgency come with pre-" +
    "set options.\r\n";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listComboType
			// 
			this.listComboType.Items.AddRange(new object[] {
            "General",
            "Patient Race",
            "Patient Grade",
            "Urgency"});
			this.listComboType.Location = new System.Drawing.Point(80, 237);
			this.listComboType.Name = "listComboType";
			this.listComboType.Size = new System.Drawing.Size(146, 95);
			this.listComboType.TabIndex = 123;
			this.listComboType.SelectionChangeCommitted += new System.EventHandler(this.listComboType_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(175, 37);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(126, 16);
			this.label3.TabIndex = 121;
			this.label3.Text = "Reportable Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTabOrder
			// 
			this.textTabOrder.Location = new System.Drawing.Point(302, 10);
			this.textTabOrder.MaxVal = 2000;
			this.textTabOrder.MinVal = -100;
			this.textTabOrder.Name = "textTabOrder";
			this.textTabOrder.Size = new System.Drawing.Size(69, 20);
			this.textTabOrder.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(175, 11);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(126, 16);
			this.label2.TabIndex = 119;
			this.label2.Text = "Tab Order";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(23, 141);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 16);
			this.label1.TabIndex = 118;
			this.label1.Text = "Option";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOption
			// 
			this.textOption.Location = new System.Drawing.Point(100, 140);
			this.textOption.Name = "textOption";
			this.textOption.Size = new System.Drawing.Size(250, 20);
			this.textOption.TabIndex = 6;
			this.textOption.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textOption_KeyDown);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.Location = new System.Drawing.Point(513, 347);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(69, 24);
			this.butUp.TabIndex = 116;
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.Location = new System.Drawing.Point(589, 347);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(69, 24);
			this.butDown.TabIndex = 115;
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butRemove
			// 
			this.butRemove.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRemove.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butRemove.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRemove.Location = new System.Drawing.Point(438, 347);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(69, 24);
			this.butRemove.TabIndex = 113;
			this.butRemove.Text = "Remove";
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butAdd
			// 
			this.butAdd.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAdd.Image = global::OpenDental.Properties.Resources.Right;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.butAdd.Location = new System.Drawing.Point(356, 138);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(76, 24);
			this.butAdd.TabIndex = 112;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// listboxComboOptions
			// 
			this.listboxComboOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listboxComboOptions.Location = new System.Drawing.Point(438, 12);
			this.listboxComboOptions.Name = "listboxComboOptions";
			this.listboxComboOptions.Size = new System.Drawing.Size(220, 329);
			this.listboxComboOptions.TabIndex = 110;
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(100, 88);
			this.textHeight.MaxVal = 2000;
			this.textHeight.MinVal = -100;
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(69, 20);
			this.textHeight.TabIndex = 3;
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(23, 88);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(71, 16);
			this.labelHeight.TabIndex = 108;
			this.labelHeight.Text = "Height";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Image = global::OpenDental.Properties.Resources.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(16, 403);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(69, 24);
			this.butDelete.TabIndex = 100;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textWidth
			// 
			this.textWidth.Location = new System.Drawing.Point(100, 62);
			this.textWidth.MaxVal = 2000;
			this.textWidth.MinVal = 1;
			this.textWidth.Name = "textWidth";
			this.textWidth.Size = new System.Drawing.Size(69, 20);
			this.textWidth.TabIndex = 2;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(23, 62);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(71, 16);
			this.label7.TabIndex = 94;
			this.label7.Text = "Width";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textYPos
			// 
			this.textYPos.Location = new System.Drawing.Point(100, 36);
			this.textYPos.MaxVal = 2000;
			this.textYPos.MinVal = -100;
			this.textYPos.Name = "textYPos";
			this.textYPos.Size = new System.Drawing.Size(69, 20);
			this.textYPos.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(23, 37);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(71, 16);
			this.label6.TabIndex = 92;
			this.label6.Text = "Y Pos";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textXPos
			// 
			this.textXPos.Location = new System.Drawing.Point(100, 10);
			this.textXPos.MaxVal = 2000;
			this.textXPos.MinVal = -100;
			this.textXPos.Name = "textXPos";
			this.textXPos.Size = new System.Drawing.Size(69, 20);
			this.textXPos.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(23, 11);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(71, 16);
			this.label5.TabIndex = 90;
			this.label5.Text = "X Pos";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(513, 403);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(69, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(589, 403);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(69, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormSheetFieldComboBox
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(670, 441);
			this.Controls.Add(this.textUiLabelMobile);
			this.Controls.Add(this.labelUiLabelMobile);
			this.Controls.Add(this.textReportable);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listComboType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textTabOrder);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textOption);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butRemove);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.listboxComboOptions);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.labelHeight);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textWidth);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textYPos);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textXPos);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetFieldComboBox";
			this.Text = "Edit ComboBox";
			this.Load += new System.EventHandler(this.FormSheetFieldComboBox_Load);
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
		private UI.Button butDelete;
		private ValidNum textHeight;
		private System.Windows.Forms.Label labelHeight;
		private OpenDental.UI.ListBoxOD listboxComboOptions;
		private UI.Button butAdd;
		private UI.Button butRemove;
		private UI.Button butDown;
		private UI.Button butUp;
		private System.Windows.Forms.TextBox textOption;
		private ValidNum textTabOrder;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.ListBoxOD listComboType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textReportable;
		private System.Windows.Forms.TextBox textUiLabelMobile;
		private System.Windows.Forms.Label labelUiLabelMobile;
	}
}