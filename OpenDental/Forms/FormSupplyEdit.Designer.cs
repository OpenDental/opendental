namespace OpenDental{
	partial class FormSupplyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplyEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textSupplier = new System.Windows.Forms.TextBox();
			this.textCatalogNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.comboCategory = new OpenDental.UI.ComboBoxOD();
			this.textLevelDesired = new OpenDental.ValidDouble();
			this.textPrice = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textOrderQty = new OpenDental.ValidNum();
			this.textOnHand = new System.Windows.Forms.TextBox();
			this.labelOnHand = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 18);
			this.label1.TabIndex = 12;
			this.label1.Text = "Supplier";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplier
			// 
			this.textSupplier.Location = new System.Drawing.Point(166, 8);
			this.textSupplier.Name = "textSupplier";
			this.textSupplier.ReadOnly = true;
			this.textSupplier.Size = new System.Drawing.Size(295, 20);
			this.textSupplier.TabIndex = 10;
			// 
			// textCatalogNumber
			// 
			this.textCatalogNumber.Location = new System.Drawing.Point(166, 61);
			this.textCatalogNumber.Name = "textCatalogNumber";
			this.textCatalogNumber.Size = new System.Drawing.Size(144, 20);
			this.textCatalogNumber.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 18);
			this.label2.TabIndex = 14;
			this.label2.Text = "Catalog Item Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(166, 87);
			this.textDescript.MaxLength = 255;
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(401, 20);
			this.textDescript.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(157, 18);
			this.label3.TabIndex = 15;
			this.label3.Text = "Description (include units)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(31, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(132, 18);
			this.label5.TabIndex = 13;
			this.label5.Text = "Category";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(32, 122);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(132, 18);
			this.label6.TabIndex = 16;
			this.label6.Text = "Stock Level";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.Location = new System.Drawing.Point(76, 253);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(104, 18);
			this.checkIsHidden.TabIndex = 6;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(32, 228);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(132, 18);
			this.label8.TabIndex = 19;
			this.label8.Text = "Price";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(231, 116);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(286, 28);
			this.label12.TabIndex = 20;
			this.label12.Text = "Decimals allowed.  This is the level to bring the supply back up to when placing " +
    "an order.";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboCategory
			// 
			this.comboCategory.Location = new System.Drawing.Point(166, 34);
			this.comboCategory.Name = "comboCategory";
			this.comboCategory.Size = new System.Drawing.Size(228, 21);
			this.comboCategory.TabIndex = 11;
			// 
			// textLevelDesired
			// 
			this.textLevelDesired.BackColor = System.Drawing.SystemColors.Window;
			this.textLevelDesired.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textLevelDesired.Location = new System.Drawing.Point(166, 120);
			this.textLevelDesired.MaxVal = 100000000D;
			this.textLevelDesired.MinVal = -100000000D;
			this.textLevelDesired.Name = "textLevelDesired";
			this.textLevelDesired.Size = new System.Drawing.Size(62, 20);
			this.textLevelDesired.TabIndex = 3;
			// 
			// textPrice
			// 
			this.textPrice.BackColor = System.Drawing.SystemColors.Window;
			this.textPrice.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textPrice.Location = new System.Drawing.Point(166, 228);
			this.textPrice.MaxVal = 100000000D;
			this.textPrice.MinVal = -100000000D;
			this.textPrice.Name = "textPrice";
			this.textPrice.Size = new System.Drawing.Size(80, 20);
			this.textPrice.TabIndex = 5;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(20, 279);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(431, 279);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 7;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(512, 279);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(32, 194);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 18);
			this.label4.TabIndex = 18;
			this.label4.Text = "Order Qty";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(231, 186);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(282, 30);
			this.label7.TabIndex = 21;
			this.label7.Text = "The quantity to include on the next order.  Creating an order will zero this out," +
    " so it\'s just temporary.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textOrderQty
			// 
			this.textOrderQty.Location = new System.Drawing.Point(166, 192);
			this.textOrderQty.Name = "textOrderQty";
			this.textOrderQty.ShowZero = false;
			this.textOrderQty.Size = new System.Drawing.Size(62, 20);
			this.textOrderQty.TabIndex = 0;
			// 
			// textOnHand
			// 
			this.textOnHand.Location = new System.Drawing.Point(166, 156);
			this.textOnHand.Name = "textOnHand";
			this.textOnHand.Size = new System.Drawing.Size(100, 20);
			this.textOnHand.TabIndex = 4;
			// 
			// labelOnHand
			// 
			this.labelOnHand.Location = new System.Drawing.Point(49, 156);
			this.labelOnHand.Name = "labelOnHand";
			this.labelOnHand.Size = new System.Drawing.Size(115, 20);
			this.labelOnHand.TabIndex = 17;
			this.labelOnHand.Text = "On Hand Qty";
			this.labelOnHand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormSupplyEdit
			// 
			this.ClientSize = new System.Drawing.Size(599, 315);
			this.Controls.Add(this.labelOnHand);
			this.Controls.Add(this.textOnHand);
			this.Controls.Add(this.textOrderQty);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textLevelDesired);
			this.Controls.Add(this.comboCategory);
			this.Controls.Add(this.textPrice);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCatalogNumber);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textSupplier);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSupplyEdit";
			this.Text = "Supply";
			this.Load += new System.EventHandler(this.FormSupplyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSupplier;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textCatalogNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkIsHidden;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label12;
		private ValidDouble textPrice;
		private UI.ComboBoxOD comboCategory;
		private ValidDouble textLevelDesired;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label7;
		private ValidNum textOrderQty;
		private System.Windows.Forms.TextBox textOnHand;
		private System.Windows.Forms.Label labelOnHand;
	}
}