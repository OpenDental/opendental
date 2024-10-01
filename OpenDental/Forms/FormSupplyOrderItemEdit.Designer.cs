namespace OpenDental{
	partial class FormSupplyOrderItemEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplyOrderItemEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textSupplier = new System.Windows.Forms.TextBox();
			this.textCatalogNumber = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textPrice = new OpenDental.ValidDouble();
			this.butDelete = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.textQty = new OpenDental.ValidNum();
			this.textCategory = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textSubtotal = new System.Windows.Forms.TextBox();
			this.textDateReceived = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butToday = new OpenDental.UI.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 18);
			this.label1.TabIndex = 9;
			this.label1.Text = "Supplier";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplier
			// 
			this.textSupplier.Location = new System.Drawing.Point(166, 8);
			this.textSupplier.Name = "textSupplier";
			this.textSupplier.ReadOnly = true;
			this.textSupplier.Size = new System.Drawing.Size(295, 20);
			this.textSupplier.TabIndex = 0;
			// 
			// textCatalogNumber
			// 
			this.textCatalogNumber.Location = new System.Drawing.Point(166, 61);
			this.textCatalogNumber.Name = "textCatalogNumber";
			this.textCatalogNumber.ReadOnly = true;
			this.textCatalogNumber.Size = new System.Drawing.Size(144, 20);
			this.textCatalogNumber.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(7, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 18);
			this.label2.TabIndex = 11;
			this.label2.Text = "Catalog Item Number";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(166, 87);
			this.textDescript.MaxLength = 255;
			this.textDescript.Name = "textDescript";
			this.textDescript.ReadOnly = true;
			this.textDescript.Size = new System.Drawing.Size(401, 20);
			this.textDescript.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 88);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(157, 18);
			this.label3.TabIndex = 12;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(31, 35);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(132, 18);
			this.label5.TabIndex = 10;
			this.label5.Text = "Category";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(32, 113);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(132, 18);
			this.label6.TabIndex = 13;
			this.label6.Text = "Quantity";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(32, 139);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(132, 18);
			this.label8.TabIndex = 14;
			this.label8.Text = "Price";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPrice
			// 
			this.textPrice.BackColor = System.Drawing.SystemColors.Window;
			this.textPrice.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textPrice.Location = new System.Drawing.Point(166, 139);
			this.textPrice.MaxVal = 100000000D;
			this.textPrice.MinVal = -100000000D;
			this.textPrice.Name = "textPrice";
			this.textPrice.Size = new System.Drawing.Size(80, 20);
			this.textPrice.TabIndex = 5;
			this.textPrice.TextChanged += new System.EventHandler(this.textPrice_TextChanged);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(22, 234);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "Remove";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(526, 234);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 6;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textQty
			// 
			this.textQty.Location = new System.Drawing.Point(166, 113);
			this.textQty.MaxVal = 10000;
			this.textQty.Name = "textQty";
			this.textQty.ShowZero = false;
			this.textQty.Size = new System.Drawing.Size(56, 20);
			this.textQty.TabIndex = 4;
			this.textQty.TextChanged += new System.EventHandler(this.textQty_TextChanged);
			// 
			// textCategory
			// 
			this.textCategory.Location = new System.Drawing.Point(166, 35);
			this.textCategory.Name = "textCategory";
			this.textCategory.ReadOnly = true;
			this.textCategory.Size = new System.Drawing.Size(225, 20);
			this.textCategory.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(32, 165);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(132, 18);
			this.label4.TabIndex = 15;
			this.label4.Text = "Subtotal";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubtotal
			// 
			this.textSubtotal.Location = new System.Drawing.Point(166, 165);
			this.textSubtotal.Name = "textSubtotal";
			this.textSubtotal.ReadOnly = true;
			this.textSubtotal.Size = new System.Drawing.Size(80, 20);
			this.textSubtotal.TabIndex = 16;
			// 
			// textDateReceived
			// 
			this.textDateReceived.Location = new System.Drawing.Point(166, 191);
			this.textDateReceived.Name = "textDateReceived";
			this.textDateReceived.Size = new System.Drawing.Size(100, 20);
			this.textDateReceived.TabIndex = 18;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(32, 191);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(132, 18);
			this.label7.TabIndex = 17;
			this.label7.Text = "Date Received";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(268, 191);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(50, 20);
			this.butToday.TabIndex = 19;
			this.butToday.Text = "Today";
			this.butToday.UseVisualStyleBackColor = true;
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(322, 192);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(225, 18);
			this.label9.TabIndex = 20;
			this.label9.Text = "Optional. Normally use date on parent order.";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormSupplyOrderItemEdit
			// 
			this.AcceptButton = this.butSave;
			this.ClientSize = new System.Drawing.Size(613, 270);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.textDateReceived);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textSubtotal);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textCategory);
			this.Controls.Add(this.textQty);
			this.Controls.Add(this.textPrice);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCatalogNumber);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textSupplier);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSupplyOrderItemEdit";
			this.Text = "Supply Order Item";
			this.Load += new System.EventHandler(this.FormSupplyOrderItemEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSupplier;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textCatalogNumber;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label8;
		private ValidDouble textPrice;
		private ValidNum textQty;
		private System.Windows.Forms.TextBox textCategory;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textSubtotal;
		private System.Windows.Forms.TextBox textDateReceived;
		private System.Windows.Forms.Label label7;
		private UI.Button butToday;
		private System.Windows.Forms.Label label9;
	}
}