namespace OpenDental {
	partial class FormEhrPatListElementEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatListElementEdit));
			this.labelOperand = new System.Windows.Forms.Label();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOK = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.labelRestriction = new System.Windows.Forms.Label();
			this.labelCompareString = new System.Windows.Forms.Label();
			this.textCompareString = new System.Windows.Forms.TextBox();
			this.labelLabValue = new System.Windows.Forms.Label();
			this.checkOrderBy = new System.Windows.Forms.CheckBox();
			this.textLabValue = new System.Windows.Forms.TextBox();
			this.listRestriction = new OpenDental.UI.ListBoxOD();
			this.listOperand = new OpenDental.UI.ListBoxOD();
			this.labelExample = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelOperand
			// 
			this.labelOperand.Location = new System.Drawing.Point(87, 87);
			this.labelOperand.Name = "labelOperand";
			this.labelOperand.Size = new System.Drawing.Size(61, 17);
			this.labelOperand.TabIndex = 10;
			this.labelOperand.Text = "Operand";
			this.labelOperand.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(312, 236);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(231, 236);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(24, 236);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 8;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelRestriction
			// 
			this.labelRestriction.Location = new System.Drawing.Point(73, 12);
			this.labelRestriction.Name = "labelRestriction";
			this.labelRestriction.Size = new System.Drawing.Size(75, 17);
			this.labelRestriction.TabIndex = 13;
			this.labelRestriction.Text = "Restriction";
			this.labelRestriction.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCompareString
			// 
			this.labelCompareString.Location = new System.Drawing.Point(2, 137);
			this.labelCompareString.Name = "labelCompareString";
			this.labelCompareString.Size = new System.Drawing.Size(146, 17);
			this.labelCompareString.TabIndex = 9;
			this.labelCompareString.Text = "Compare string";
			this.labelCompareString.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCompareString
			// 
			this.textCompareString.Location = new System.Drawing.Point(149, 136);
			this.textCompareString.Name = "textCompareString";
			this.textCompareString.Size = new System.Drawing.Size(128, 20);
			this.textCompareString.TabIndex = 1;
			// 
			// labelLabValue
			// 
			this.labelLabValue.Location = new System.Drawing.Point(55, 163);
			this.labelLabValue.Name = "labelLabValue";
			this.labelLabValue.Size = new System.Drawing.Size(93, 17);
			this.labelLabValue.TabIndex = 14;
			this.labelLabValue.Text = "Lab value";
			this.labelLabValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelLabValue.Visible = false;
			// 
			// checkOrderBy
			// 
			this.checkOrderBy.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderBy.Location = new System.Drawing.Point(52, 187);
			this.checkOrderBy.Name = "checkOrderBy";
			this.checkOrderBy.Size = new System.Drawing.Size(112, 24);
			this.checkOrderBy.TabIndex = 16;
			this.checkOrderBy.Text = "Order by";
			this.checkOrderBy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOrderBy.UseVisualStyleBackColor = true;
			// 
			// textLabValue
			// 
			this.textLabValue.Location = new System.Drawing.Point(149, 161);
			this.textLabValue.Name = "textLabValue";
			this.textLabValue.Size = new System.Drawing.Size(128, 20);
			this.textLabValue.TabIndex = 2;
			this.textLabValue.Visible = false;
			// 
			// listRestriction
			// 
			this.listRestriction.ItemStrings = new string[] {
            "Birthdate",
            "Disease",
            "Medication",
            "Lab result",
            "Gender"};
			this.listRestriction.Location = new System.Drawing.Point(149, 12);
			this.listRestriction.Name = "listRestriction";
			this.listRestriction.Size = new System.Drawing.Size(75, 69);
			this.listRestriction.TabIndex = 18;
			this.listRestriction.SelectedIndexChanged += new System.EventHandler(this.listRestriction_SelectedIndexChanged);
			// 
			// listOperand
			// 
			this.listOperand.ItemStrings = new string[] {
            "GreaterThan",
            "LessThan",
            "Equals"};
			this.listOperand.Location = new System.Drawing.Point(149, 87);
			this.listOperand.Name = "listOperand";
			this.listOperand.Size = new System.Drawing.Size(75, 43);
			this.listOperand.TabIndex = 19;
			// 
			// labelExample
			// 
			this.labelExample.Location = new System.Drawing.Point(278, 137);
			this.labelExample.Name = "labelExample";
			this.labelExample.Size = new System.Drawing.Size(119, 17);
			this.labelExample.TabIndex = 20;
			this.labelExample.Text = "Example";
			this.labelExample.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormEhrPatListElementEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(399, 271);
			this.Controls.Add(this.labelExample);
			this.Controls.Add(this.listOperand);
			this.Controls.Add(this.listRestriction);
			this.Controls.Add(this.textLabValue);
			this.Controls.Add(this.checkOrderBy);
			this.Controls.Add(this.labelLabValue);
			this.Controls.Add(this.labelRestriction);
			this.Controls.Add(this.textCompareString);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelCompareString);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelOperand);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatListElementEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PatList Element Edit";
			this.Load += new System.EventHandler(this.FormPatListElementEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelOperand;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Label labelRestriction;
		private System.Windows.Forms.Label labelCompareString;
		private System.Windows.Forms.TextBox textCompareString;
		private System.Windows.Forms.Label labelLabValue;
		private System.Windows.Forms.CheckBox checkOrderBy;
		private System.Windows.Forms.TextBox textLabValue;
		private OpenDental.UI.ListBoxOD listRestriction;
		private OpenDental.UI.ListBoxOD listOperand;
		private System.Windows.Forms.Label labelExample;
	}
}