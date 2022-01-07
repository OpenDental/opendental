namespace OpenDental
{
	partial class FormAnestheticMedsIntake
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAnestheticMedsIntake));
			this.textQty = new System.Windows.Forms.TextBox();
			this.labelQty = new System.Windows.Forms.Label();
			this.groupAnesthMed = new System.Windows.Forms.GroupBox();
			this.comboAnesthMedName = new System.Windows.Forms.ComboBox();
			this.textInvoiceNum = new System.Windows.Forms.TextBox();
			this.groupSupplier = new System.Windows.Forms.GroupBox();
			this.butAddSupplier = new OpenDental.UI.Button();
			this.comboSupplierName = new System.Windows.Forms.ComboBox();
			this.butClose = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelInvoice = new System.Windows.Forms.Label();
			this.textDate = new System.Windows.Forms.TextBox();
			this.labelDate = new System.Windows.Forms.Label();
			this.groupAnesthMed.SuspendLayout();
			this.groupSupplier.SuspendLayout();
			this.SuspendLayout();
			// 
			// textQty
			// 
			this.textQty.Location = new System.Drawing.Point(251, 19);
			this.textQty.Name = "textQty";
			this.textQty.Size = new System.Drawing.Size(116, 20);
			this.textQty.TabIndex = 2;
			this.textQty.TextChanged += new System.EventHandler(this.textQty_TextChanged);
			// 
			// labelQty
			// 
			this.labelQty.AutoSize = true;
			this.labelQty.Location = new System.Drawing.Point(246, 0);
			this.labelQty.Name = "labelQty";
			this.labelQty.Size = new System.Drawing.Size(173, 13);
			this.labelQty.TabIndex = 4;
			this.labelQty.Text = "Quantity (units must be in total mLs)";
			// 
			// groupAnesthMed
			// 
			this.groupAnesthMed.Controls.Add(this.comboAnesthMedName);
			this.groupAnesthMed.Controls.Add(this.textQty);
			this.groupAnesthMed.Controls.Add(this.labelQty);
			this.groupAnesthMed.Location = new System.Drawing.Point(30, 39);
			this.groupAnesthMed.Name = "groupAnesthMed";
			this.groupAnesthMed.Size = new System.Drawing.Size(482, 48);
			this.groupAnesthMed.TabIndex = 6;
			this.groupAnesthMed.TabStop = false;
			this.groupAnesthMed.Text = "Anesthetic Medication";
			// 
			// comboAnesthMedName
			// 
			this.comboAnesthMedName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAnesthMedName.FormattingEnabled = true;
			this.comboAnesthMedName.Location = new System.Drawing.Point(10, 19);
			this.comboAnesthMedName.Name = "comboAnesthMedName";
			this.comboAnesthMedName.Size = new System.Drawing.Size(231, 21);
			this.comboAnesthMedName.TabIndex = 0;
			this.comboAnesthMedName.SelectedIndexChanged += new System.EventHandler(this.comboAnesthMed_SelectedIndexChanged);
			// 
			// textInvoiceNum
			// 
			this.textInvoiceNum.Location = new System.Drawing.Point(280, 110);
			this.textInvoiceNum.Name = "textInvoiceNum";
			this.textInvoiceNum.Size = new System.Drawing.Size(216, 20);
			this.textInvoiceNum.TabIndex = 7;
			// 
			// groupSupplier
			// 
			this.groupSupplier.Controls.Add(this.butAddSupplier);
			this.groupSupplier.Controls.Add(this.comboSupplierName);
			this.groupSupplier.Controls.Add(this.butClose);
			this.groupSupplier.Controls.Add(this.butCancel);
			this.groupSupplier.Controls.Add(this.labelInvoice);
			this.groupSupplier.Location = new System.Drawing.Point(30, 93);
			this.groupSupplier.Name = "groupSupplier";
			this.groupSupplier.Size = new System.Drawing.Size(482, 95);
			this.groupSupplier.TabIndex = 8;
			this.groupSupplier.TabStop = false;
			this.groupSupplier.Text = "Supplier";
			// 
			// butAddSupplier
			// 
			this.butAddSupplier.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butAddSupplier.Autosize = true;
			this.butAddSupplier.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butAddSupplier.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butAddSupplier.CornerRadius = 4F;
			this.butAddSupplier.Image = global::OpenDental.Properties.Resources.Add;
			this.butAddSupplier.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddSupplier.Location = new System.Drawing.Point(43, 53);
			this.butAddSupplier.Name = "butAddSupplier";
			this.butAddSupplier.Size = new System.Drawing.Size(163, 26);
			this.butAddSupplier.TabIndex = 139;
			this.butAddSupplier.Text = "Add new supplier...";
			this.butAddSupplier.UseVisualStyleBackColor = true;
			this.butAddSupplier.Click += new System.EventHandler(this.butAddSupplier_Click);
			// 
			// comboSupplierName
			// 
			this.comboSupplierName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSupplierName.FormattingEnabled = true;
			this.comboSupplierName.Location = new System.Drawing.Point(10, 15);
			this.comboSupplierName.Name = "comboSupplierName";
			this.comboSupplierName.Size = new System.Drawing.Size(231, 21);
			this.comboSupplierName.TabIndex = 138;
			// 
			// butClose
			// 
			this.butClose.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butClose.Autosize = true;
			this.butClose.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butClose.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butClose.CornerRadius = 4F;
			this.butClose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClose.Location = new System.Drawing.Point(373, 53);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(90, 26);
			this.butClose.TabIndex = 137;
			this.butClose.Text = "Save and Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butCancel
			// 
			this.butCancel.AdjustImageLocation = new System.Drawing.Point(0, 0);
			this.butCancel.Autosize = true;
			this.butCancel.BtnShape = OpenDental.UI.enumType.BtnShape.Rectangle;
			this.butCancel.BtnStyle = OpenDental.UI.enumType.XPStyle.Silver;
			this.butCancel.CornerRadius = 4F;
			this.butCancel.Image = global::OpenDental.Properties.Resources.deleteX;
			this.butCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCancel.Location = new System.Drawing.Point(301, 53);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(66, 26);
			this.butCancel.TabIndex = 54;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelInvoice
			// 
			this.labelInvoice.AutoSize = true;
			this.labelInvoice.Location = new System.Drawing.Point(247, 1);
			this.labelInvoice.Name = "labelInvoice";
			this.labelInvoice.Size = new System.Drawing.Size(52, 13);
			this.labelInvoice.TabIndex = 9;
			this.labelInvoice.Text = "Invoice #";
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(73, 12);
			this.textDate.Name = "textDate";
			this.textDate.ReadOnly = true;
			this.textDate.Size = new System.Drawing.Size(115, 20);
			this.textDate.TabIndex = 9;
			// 
			// labelDate
			// 
			this.labelDate.AutoSize = true;
			this.labelDate.Location = new System.Drawing.Point(37, 15);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(30, 13);
			this.labelDate.TabIndex = 10;
			this.labelDate.Text = "Date";
			// 
			// FormAnestheticMedsIntake
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(532, 209);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.textInvoiceNum);
			this.Controls.Add(this.groupAnesthMed);
			this.Controls.Add(this.groupSupplier);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAnestheticMedsIntake";
			this.Text = "Anesthetic Medication Intake Form";
			this.Load += new System.EventHandler(this.FormAnestheticMedsIntake_Load);
			this.groupAnesthMed.ResumeLayout(false);
			this.groupAnesthMed.PerformLayout();
			this.groupSupplier.ResumeLayout(false);
			this.groupSupplier.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textQty;
		private System.Windows.Forms.Label labelQty;
		private System.Windows.Forms.GroupBox groupAnesthMed;
		private System.Windows.Forms.TextBox textInvoiceNum;
		private System.Windows.Forms.GroupBox groupSupplier;
		private System.Windows.Forms.Label labelInvoice;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.ComboBox comboAnesthMedName;
        private System.Windows.Forms.ComboBox comboSupplierName;
        private OpenDental.UI.Button butAddSupplier;
	}
}