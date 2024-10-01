namespace OpenDental{
	partial class FormInsBlueBookRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInsBlueBookRuleEdit));
			this.butSave = new OpenDental.UI.Button();
			this.labelLimitType = new System.Windows.Forms.Label();
			this.listLimitType = new OpenDental.UI.ListBox();
			this.labelLimitValue = new System.Windows.Forms.Label();
			this.textRule = new System.Windows.Forms.TextBox();
			this.labelRule = new System.Windows.Forms.Label();
			this.textLimitValue = new OpenDental.ValidNum();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(164, 203);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.ButSave_Click);
			// 
			// labelLimitType
			// 
			this.labelLimitType.Location = new System.Drawing.Point(99, 43);
			this.labelLimitType.Name = "labelLimitType";
			this.labelLimitType.Size = new System.Drawing.Size(124, 18);
			this.labelLimitType.TabIndex = 269;
			this.labelLimitType.Text = "Limit Type";
			this.labelLimitType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listLimitType
			// 
			this.listLimitType.Location = new System.Drawing.Point(99, 62);
			this.listLimitType.Name = "listLimitType";
			this.listLimitType.Size = new System.Drawing.Size(124, 82);
			this.listLimitType.TabIndex = 268;
			this.listLimitType.SelectedIndexChanged += new System.EventHandler(this.listLimitType_SelectedIndexChanged);
			// 
			// labelLimitValue
			// 
			this.labelLimitValue.Location = new System.Drawing.Point(22, 155);
			this.labelLimitValue.Name = "labelLimitValue";
			this.labelLimitValue.Size = new System.Drawing.Size(76, 20);
			this.labelLimitValue.TabIndex = 271;
			this.labelLimitValue.Text = "Limit Value";
			this.labelLimitValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRule
			// 
			this.textRule.BackColor = System.Drawing.SystemColors.Window;
			this.textRule.Enabled = false;
			this.textRule.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textRule.Location = new System.Drawing.Point(99, 12);
			this.textRule.Name = "textRule";
			this.textRule.ReadOnly = true;
			this.textRule.Size = new System.Drawing.Size(124, 20);
			this.textRule.TabIndex = 272;
			// 
			// labelRule
			// 
			this.labelRule.Location = new System.Drawing.Point(2, 12);
			this.labelRule.Name = "labelRule";
			this.labelRule.Size = new System.Drawing.Size(96, 20);
			this.labelRule.TabIndex = 273;
			this.labelRule.Text = "Rule matches on";
			this.labelRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLimitValue
			// 
			this.textLimitValue.Location = new System.Drawing.Point(99, 156);
			this.textLimitValue.MaxVal = 999999;
			this.textLimitValue.MinVal = 1;
			this.textLimitValue.Name = "textLimitValue";
			this.textLimitValue.Size = new System.Drawing.Size(124, 20);
			this.textLimitValue.TabIndex = 274;
			// 
			// FormInsBlueBookRuleEdit
			// 
			this.ClientSize = new System.Drawing.Size(251, 239);
			this.Controls.Add(this.textLimitValue);
			this.Controls.Add(this.labelRule);
			this.Controls.Add(this.textRule);
			this.Controls.Add(this.labelLimitValue);
			this.Controls.Add(this.labelLimitType);
			this.Controls.Add(this.listLimitType);
			this.Controls.Add(this.butSave);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormInsBlueBookRuleEdit";
			this.Text = "Blue Book Rule Edit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormInsBlueBookRuleEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormInsBlueBookRuleEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private System.Windows.Forms.Label labelLimitType;
		private OpenDental.UI.ListBox listLimitType;
		private System.Windows.Forms.Label labelLimitValue;
		private System.Windows.Forms.TextBox textRule;
		private System.Windows.Forms.Label labelRule;
		private ValidNum textLimitValue;
	}
}