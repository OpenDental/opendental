namespace OpenDental{
	partial class FormEquipmentEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEquipmentEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.textSerialNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textModelYear = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textLocation = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textMarketValue = new OpenDental.ValidDouble();
			this.textDateSold = new OpenDental.ValidDate();
			this.textDatePurchased = new OpenDental.ValidDate();
			this.textPurchaseCost = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDateEntry = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textStatus = new OpenDental.ODtextBox();
			this.butGenerate = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 47);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 18);
			this.label1.TabIndex = 4;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(135, 47);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(401, 20);
			this.textDescription.TabIndex = 0;
			// 
			// textSerialNumber
			// 
			this.textSerialNumber.Location = new System.Drawing.Point(135, 75);
			this.textSerialNumber.Name = "textSerialNumber";
			this.textSerialNumber.Size = new System.Drawing.Size(144, 20);
			this.textSerialNumber.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(132, 18);
			this.label2.TabIndex = 8;
			this.label2.Text = "Serial Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textModelYear
			// 
			this.textModelYear.Location = new System.Drawing.Point(136, 103);
			this.textModelYear.MaxLength = 2;
			this.textModelYear.Name = "textModelYear";
			this.textModelYear.Size = new System.Drawing.Size(42, 20);
			this.textModelYear.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 103);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(132, 18);
			this.label3.TabIndex = 10;
			this.label3.Text = "Model Yr (2 digit)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(3, 132);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(132, 18);
			this.label5.TabIndex = 14;
			this.label5.Text = "Date Purchased";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 160);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(132, 18);
			this.label6.TabIndex = 16;
			this.label6.Text = "Date Sold";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLocation
			// 
			this.textLocation.Location = new System.Drawing.Point(135, 243);
			this.textLocation.Name = "textLocation";
			this.textLocation.Size = new System.Drawing.Size(401, 20);
			this.textLocation.TabIndex = 7;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(3, 244);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(132, 18);
			this.label7.TabIndex = 18;
			this.label7.Text = "Location";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(3, 188);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 18);
			this.label4.TabIndex = 19;
			this.label4.Text = "Purchase Cost";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 216);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(132, 18);
			this.label8.TabIndex = 20;
			this.label8.Text = "Estimated Market Value";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMarketValue
			// 
			this.textMarketValue.BackColor = System.Drawing.SystemColors.Window;
			this.textMarketValue.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textMarketValue.Location = new System.Drawing.Point(135, 215);
			this.textMarketValue.MaxVal = 100000000D;
			this.textMarketValue.MinVal = -100000000D;
			this.textMarketValue.Name = "textMarketValue";
			this.textMarketValue.Size = new System.Drawing.Size(100, 20);
			this.textMarketValue.TabIndex = 6;
			// 
			// textDateSold
			// 
			this.textDateSold.Location = new System.Drawing.Point(135, 159);
			this.textDateSold.Name = "textDateSold";
			this.textDateSold.Size = new System.Drawing.Size(100, 20);
			this.textDateSold.TabIndex = 4;
			// 
			// textDatePurchased
			// 
			this.textDatePurchased.Location = new System.Drawing.Point(135, 131);
			this.textDatePurchased.Name = "textDatePurchased";
			this.textDatePurchased.Size = new System.Drawing.Size(100, 20);
			this.textDatePurchased.TabIndex = 3;
			// 
			// textPurchaseCost
			// 
			this.textPurchaseCost.BackColor = System.Drawing.SystemColors.Window;
			this.textPurchaseCost.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textPurchaseCost.Location = new System.Drawing.Point(135, 187);
			this.textPurchaseCost.MaxVal = 100000000D;
			this.textPurchaseCost.MinVal = -100000000D;
			this.textPurchaseCost.Name = "textPurchaseCost";
			this.textPurchaseCost.Size = new System.Drawing.Size(100, 20);
			this.textPurchaseCost.TabIndex = 5;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(27, 347);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(402, 347);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(491, 347);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(135, 20);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(100, 20);
			this.textDateEntry.TabIndex = 25;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(3, 20);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(132, 18);
			this.label9.TabIndex = 26;
			this.label9.Text = "Date Entry";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(239, 20);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(132, 18);
			this.label10.TabIndex = 27;
			this.label10.Text = "(for security purposes)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(3, 272);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(132, 18);
			this.label11.TabIndex = 29;
			this.label11.Text = "Status";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStatus
			// 
			this.textStatus.AcceptsTab = true;
			this.textStatus.BackColor = System.Drawing.SystemColors.Window;
			this.textStatus.DetectLinksEnabled = false;
			this.textStatus.DetectUrls = false;
			this.textStatus.Location = new System.Drawing.Point(135, 271);
			this.textStatus.Name = "textStatus";
			this.textStatus.QuickPasteType = OpenDentBusiness.QuickPasteType.Equipment;
			this.textStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textStatus.Size = new System.Drawing.Size(401, 60);
			this.textStatus.TabIndex = 24;
			this.textStatus.Text = "";
			// 
			// butGenerate
			// 
			this.butGenerate.Location = new System.Drawing.Point(285, 75);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(65, 20);
			this.butGenerate.TabIndex = 30;
			this.butGenerate.Text = "Generate";
			this.butGenerate.UseVisualStyleBackColor = true;
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// FormEquipmentEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(591, 398);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.textStatus);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textMarketValue);
			this.Controls.Add(this.textDateSold);
			this.Controls.Add(this.textDatePurchased);
			this.Controls.Add(this.textPurchaseCost);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textLocation);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textModelYear);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textSerialNumber);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEquipmentEdit";
			this.Text = "Equipment";
			this.Load += new System.EventHandler(this.FormEquipmentEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textSerialNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textModelYear;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textLocation;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label8;
		private ValidDouble textPurchaseCost;
		private ValidDate textDatePurchased;
		private ValidDate textDateSold;
		private ValidDouble textMarketValue;
		private System.Windows.Forms.TextBox textDateEntry;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private ODtextBox textStatus;
		private UI.Button butGenerate;
	}
}