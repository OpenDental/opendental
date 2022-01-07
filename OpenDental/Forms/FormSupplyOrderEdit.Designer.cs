namespace OpenDental{
	partial class FormSupplyOrderEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSupplyOrderEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textSupplier = new System.Windows.Forms.TextBox();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textDatePlaced = new OpenDental.ValidDate();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textAmountTotal = new OpenDental.ValidDouble();
			this.label8 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textShippingCharge = new OpenDental.ValidDouble();
			this.labelShippingCharge = new System.Windows.Forms.Label();
			this.comboUser = new OpenDental.UI.ComboBoxOD();
			this.label5 = new System.Windows.Forms.Label();
			this.textDateReceived = new OpenDental.ValidDate();
			this.labelDateReceived = new System.Windows.Forms.Label();
			this.butToday = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(31, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(132, 18);
			this.label1.TabIndex = 10;
			this.label1.Text = "Supplier";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSupplier
			// 
			this.textSupplier.Location = new System.Drawing.Point(166, 8);
			this.textSupplier.Name = "textSupplier";
			this.textSupplier.ReadOnly = true;
			this.textSupplier.Size = new System.Drawing.Size(295, 20);
			this.textSupplier.TabIndex = 7;
			this.textSupplier.TabStop = false;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(166, 164);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(401, 132);
			this.textNote.TabIndex = 10;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(33, 163);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(132, 18);
			this.label7.TabIndex = 14;
			this.label7.Text = "Note";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(31, 34);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(132, 18);
			this.label2.TabIndex = 11;
			this.label2.Text = "Date Placed";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(272, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(443, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "Leave date and user blank to indicate order has not been placed yet and is pendin" +
    "g.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textDatePlaced
			// 
			this.textDatePlaced.Location = new System.Drawing.Point(166, 34);
			this.textDatePlaced.Name = "textDatePlaced";
			this.textDatePlaced.Size = new System.Drawing.Size(100, 20);
			this.textDatePlaced.TabIndex = 0;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(22, 268);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(81, 24);
			this.butDelete.TabIndex = 25;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(628, 238);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 15;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(628, 268);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 20;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textAmountTotal
			// 
			this.textAmountTotal.BackColor = System.Drawing.SystemColors.Window;
			this.textAmountTotal.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textAmountTotal.Location = new System.Drawing.Point(166, 86);
			this.textAmountTotal.MaxVal = 100000000D;
			this.textAmountTotal.MinVal = -100000000D;
			this.textAmountTotal.Name = "textAmountTotal";
			this.textAmountTotal.Size = new System.Drawing.Size(80, 20);
			this.textAmountTotal.TabIndex = 2;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(32, 86);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(132, 18);
			this.label8.TabIndex = 12;
			this.label8.Text = "Total Amount";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(247, 86);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(319, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Auto calculates unless some items have zero amount.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textShippingCharge
			// 
			this.textShippingCharge.BackColor = System.Drawing.SystemColors.Window;
			this.textShippingCharge.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textShippingCharge.Location = new System.Drawing.Point(166, 112);
			this.textShippingCharge.MaxVal = 100000000D;
			this.textShippingCharge.MinVal = 0D;
			this.textShippingCharge.Name = "textShippingCharge";
			this.textShippingCharge.Size = new System.Drawing.Size(80, 20);
			this.textShippingCharge.TabIndex = 3;
			// 
			// labelShippingCharge
			// 
			this.labelShippingCharge.Location = new System.Drawing.Point(32, 112);
			this.labelShippingCharge.Name = "labelShippingCharge";
			this.labelShippingCharge.Size = new System.Drawing.Size(132, 18);
			this.labelShippingCharge.TabIndex = 13;
			this.labelShippingCharge.Text = "Shipping Charge";
			this.labelShippingCharge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUser
			// 
			this.comboUser.Location = new System.Drawing.Point(166, 60);
			this.comboUser.Name = "comboUser";
			this.comboUser.Size = new System.Drawing.Size(121, 21);
			this.comboUser.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(56, 61);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(106, 18);
			this.label5.TabIndex = 37;
			this.label5.Text = "Placed By";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateReceived
			// 
			this.textDateReceived.Location = new System.Drawing.Point(166, 138);
			this.textDateReceived.Name = "textDateReceived";
			this.textDateReceived.Size = new System.Drawing.Size(100, 20);
			this.textDateReceived.TabIndex = 4;
			// 
			// labelDateReceived
			// 
			this.labelDateReceived.Location = new System.Drawing.Point(32, 138);
			this.labelDateReceived.Name = "labelDateReceived";
			this.labelDateReceived.Size = new System.Drawing.Size(132, 18);
			this.labelDateReceived.TabIndex = 16;
			this.labelDateReceived.Text = "Date Received";
			this.labelDateReceived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(268, 138);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(50, 20);
			this.butToday.TabIndex = 5;
			this.butToday.Text = "Today";
			this.butToday.UseVisualStyleBackColor = true;
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// FormSupplyOrderEdit
			// 
			this.ClientSize = new System.Drawing.Size(715, 304);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.labelDateReceived);
			this.Controls.Add(this.textDateReceived);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboUser);
			this.Controls.Add(this.textShippingCharge);
			this.Controls.Add(this.labelShippingCharge);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textAmountTotal);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDatePlaced);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textSupplier);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSupplyOrderEdit";
			this.Text = "Supply Order";
			this.Load += new System.EventHandler(this.FormSupplyOrderEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSupplier;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label7;
		private ValidDate textDatePlaced;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private ValidDouble textAmountTotal;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label4;
		private ValidDouble textShippingCharge;
		private System.Windows.Forms.Label labelShippingCharge;
		private UI.ComboBoxOD comboUser;
		private System.Windows.Forms.Label label5;
		private ValidDate textDateReceived;
		private System.Windows.Forms.Label labelDateReceived;
		private UI.Button butToday;
	}
}