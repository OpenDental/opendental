namespace OpenDental{
	partial class FormAutomationConditionEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAutomationConditionEdit));
			this.labelCompareString = new System.Windows.Forms.Label();
			this.textCompareString = new System.Windows.Forms.TextBox();
			this.labelCompareField = new System.Windows.Forms.Label();
			this.labelComparison = new System.Windows.Forms.Label();
			this.listCompareField = new OpenDental.UI.ListBoxOD();
			this.listComparison = new OpenDental.UI.ListBoxOD();
			this.labelWarning = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butBillingType = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelCompareString
			// 
			this.labelCompareString.Location = new System.Drawing.Point(397, 20);
			this.labelCompareString.Name = "labelCompareString";
			this.labelCompareString.Size = new System.Drawing.Size(195, 20);
			this.labelCompareString.TabIndex = 4;
			this.labelCompareString.Text = "Text";
			this.labelCompareString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textCompareString
			// 
			this.textCompareString.Location = new System.Drawing.Point(397, 43);
			this.textCompareString.Name = "textCompareString";
			this.textCompareString.Size = new System.Drawing.Size(316, 20);
			this.textCompareString.TabIndex = 5;
			// 
			// labelCompareField
			// 
			this.labelCompareField.Location = new System.Drawing.Point(24, 20);
			this.labelCompareField.Name = "labelCompareField";
			this.labelCompareField.Size = new System.Drawing.Size(175, 20);
			this.labelCompareField.TabIndex = 31;
			this.labelCompareField.Text = "Field";
			this.labelCompareField.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelComparison
			// 
			this.labelComparison.Location = new System.Drawing.Point(231, 20);
			this.labelComparison.Name = "labelComparison";
			this.labelComparison.Size = new System.Drawing.Size(138, 20);
			this.labelComparison.TabIndex = 32;
			this.labelComparison.Text = "Comparison";
			this.labelComparison.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listCompareField
			// 
			this.listCompareField.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listCompareField.Location = new System.Drawing.Point(24, 43);
			this.listCompareField.Name = "listCompareField";
			this.listCompareField.Size = new System.Drawing.Size(181, 212);
			this.listCompareField.TabIndex = 71;
			this.listCompareField.Click += new System.EventHandler(this.listCompareField_Click);
			// 
			// listComparison
			// 
			this.listComparison.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listComparison.Location = new System.Drawing.Point(234, 43);
			this.listComparison.Name = "listComparison";
			this.listComparison.Size = new System.Drawing.Size(138, 212);
			this.listComparison.TabIndex = 72;
			// 
			// labelWarning
			// 
			this.labelWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelWarning.Location = new System.Drawing.Point(234, 43);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(489, 212);
			this.labelWarning.TabIndex = 73;
			this.labelWarning.Text = resources.GetString("labelWarning.Text");
			this.labelWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelWarning.Visible = false;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(24, 302);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 69;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(648, 261);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(648, 302);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butBillingType
			// 
			this.butBillingType.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBillingType.Location = new System.Drawing.Point(719, 43);
			this.butBillingType.Name = "butBillingType";
			this.butBillingType.Size = new System.Drawing.Size(23, 20);
			this.butBillingType.TabIndex = 74;
			this.butBillingType.Text = "...";
			this.butBillingType.Visible = false;
			this.butBillingType.Click += new System.EventHandler(this.butBillingType_Click);
			// 
			// FormAutomationConditionEdit
			// 
			this.ClientSize = new System.Drawing.Size(748, 353);
			this.Controls.Add(this.butBillingType);
			this.Controls.Add(this.textCompareString);
			this.Controls.Add(this.listComparison);
			this.Controls.Add(this.listCompareField);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.labelComparison);
			this.Controls.Add(this.labelCompareField);
			this.Controls.Add(this.labelCompareString);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelWarning);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAutomationConditionEdit";
			this.Text = "Automation Condition Edit";
			this.Load += new System.EventHandler(this.FormAutomationConditionEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelCompareString;
		private System.Windows.Forms.TextBox textCompareString;
		private System.Windows.Forms.Label labelCompareField;
		private System.Windows.Forms.Label labelComparison;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.ListBoxOD listCompareField;
		private OpenDental.UI.ListBoxOD listComparison;
		private System.Windows.Forms.Label labelWarning;
		private UI.Button butBillingType;
	}
}