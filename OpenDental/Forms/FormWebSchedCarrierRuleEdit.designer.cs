namespace OpenDental {
	partial class FormWebSchedCarrierRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebSchedCarrierRuleEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.labelCarrierName = new System.Windows.Forms.Label();
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.labelDisplayName = new System.Windows.Forms.Label();
			this.textDisplayName = new System.Windows.Forms.TextBox();
			this.labelMessage = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.groupRuleType = new OpenDental.UI.GroupBoxOD();
			this.radioAllowWithInput = new System.Windows.Forms.RadioButton();
			this.radioAllow = new System.Windows.Forms.RadioButton();
			this.radioAllowWithMessage = new System.Windows.Forms.RadioButton();
			this.radioBlock = new System.Windows.Forms.RadioButton();
			this.labelCount = new System.Windows.Forms.Label();
			this.groupRuleType.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(212, 350);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(131, 350);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 8;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// labelCarrierName
			// 
			this.labelCarrierName.Location = new System.Drawing.Point(16, 13);
			this.labelCarrierName.Name = "labelCarrierName";
			this.labelCarrierName.Size = new System.Drawing.Size(150, 17);
			this.labelCarrierName.TabIndex = 11;
			this.labelCarrierName.Text = "Insurance Carrier Name";
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(19, 31);
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.ReadOnly = true;
			this.textCarrierName.Size = new System.Drawing.Size(268, 20);
			this.textCarrierName.TabIndex = 1;
			// 
			// labelDisplayName
			// 
			this.labelDisplayName.Location = new System.Drawing.Point(16, 56);
			this.labelDisplayName.Name = "labelDisplayName";
			this.labelDisplayName.Size = new System.Drawing.Size(150, 17);
			this.labelDisplayName.TabIndex = 12;
			this.labelDisplayName.Text = "Display Name for Patients";
			// 
			// textDisplayName
			// 
			this.textDisplayName.Location = new System.Drawing.Point(19, 74);
			this.textDisplayName.Name = "textDisplayName";
			this.textDisplayName.Size = new System.Drawing.Size(268, 20);
			this.textDisplayName.TabIndex = 2;
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(16, 195);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(150, 17);
			this.labelMessage.TabIndex = 14;
			this.labelMessage.Text = "Message to Patients";
			// 
			// textMessage
			// 
			this.textMessage.Location = new System.Drawing.Point(19, 213);
			this.textMessage.MaxLength = 100;
			this.textMessage.Multiline = true;
			this.textMessage.Name = "textMessage";
			this.textMessage.Size = new System.Drawing.Size(268, 108);
			this.textMessage.TabIndex = 7;
			this.textMessage.TextChanged += new System.EventHandler(this.textMessage_TextChanged);
			// 
			// groupRuleType
			// 
			this.groupRuleType.Controls.Add(this.radioAllowWithInput);
			this.groupRuleType.Controls.Add(this.radioAllow);
			this.groupRuleType.Controls.Add(this.radioAllowWithMessage);
			this.groupRuleType.Controls.Add(this.radioBlock);
			this.groupRuleType.Location = new System.Drawing.Point(19, 100);
			this.groupRuleType.Name = "groupRuleType";
			this.groupRuleType.Size = new System.Drawing.Size(268, 92);
			this.groupRuleType.TabIndex = 3;
			this.groupRuleType.Text = "Rule";
			// 
			// radioAllowWithInput
			// 
			this.radioAllowWithInput.Enabled = false;
			this.radioAllowWithInput.Location = new System.Drawing.Point(15, 34);
			this.radioAllowWithInput.Name = "radioAllowWithInput";
			this.radioAllowWithInput.Size = new System.Drawing.Size(146, 19);
			this.radioAllowWithInput.TabIndex = 4;
			this.radioAllowWithInput.TabStop = true;
			this.radioAllowWithInput.Text = "Allow With Input";
			this.radioAllowWithInput.UseVisualStyleBackColor = true;
			this.radioAllowWithInput.CheckedChanged += new System.EventHandler(this.radioAllowWithInput_CheckedChanged);
			// 
			// radioAllow
			// 
			this.radioAllow.Location = new System.Drawing.Point(15, 17);
			this.radioAllow.Name = "radioAllow";
			this.radioAllow.Size = new System.Drawing.Size(60, 19);
			this.radioAllow.TabIndex = 3;
			this.radioAllow.TabStop = true;
			this.radioAllow.Text = "Allow";
			this.radioAllow.UseVisualStyleBackColor = true;
			// 
			// radioAllowWithMessage
			// 
			this.radioAllowWithMessage.Location = new System.Drawing.Point(15, 51);
			this.radioAllowWithMessage.Name = "radioAllowWithMessage";
			this.radioAllowWithMessage.Size = new System.Drawing.Size(146, 19);
			this.radioAllowWithMessage.TabIndex = 5;
			this.radioAllowWithMessage.TabStop = true;
			this.radioAllowWithMessage.Text = "Allow With Message";
			this.radioAllowWithMessage.UseVisualStyleBackColor = true;
			// 
			// radioBlock
			// 
			this.radioBlock.Location = new System.Drawing.Point(15, 68);
			this.radioBlock.Name = "radioBlock";
			this.radioBlock.Size = new System.Drawing.Size(147, 19);
			this.radioBlock.TabIndex = 6;
			this.radioBlock.TabStop = true;
			this.radioBlock.Text = "Block";
			this.radioBlock.UseVisualStyleBackColor = true;
			// 
			// labelCount
			// 
			this.labelCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCount.Location = new System.Drawing.Point(212, 324);
			this.labelCount.Name = "labelCount";
			this.labelCount.Size = new System.Drawing.Size(75, 17);
			this.labelCount.TabIndex = 17;
			this.labelCount.Text = "0 / 100";
			this.labelCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormWebSchedCarrierRuleEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(306, 386);
			this.Controls.Add(this.labelCount);
			this.Controls.Add(this.groupRuleType);
			this.Controls.Add(this.textMessage);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.labelCarrierName);
			this.Controls.Add(this.textCarrierName);
			this.Controls.Add(this.labelDisplayName);
			this.Controls.Add(this.textDisplayName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebSchedCarrierRuleEdit";
			this.Text = "Web Sched Carrier Rule Edit";
			this.Load += new System.EventHandler(this.FormWebSchedCarrierRuleEdit_Load);
			this.groupRuleType.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.Label labelCarrierName;
		private System.Windows.Forms.TextBox textCarrierName;
		private System.Windows.Forms.Label labelDisplayName;
		private System.Windows.Forms.TextBox textDisplayName;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.TextBox textMessage;
		private UI.GroupBoxOD groupRuleType;
		private System.Windows.Forms.RadioButton radioAllow;
		private System.Windows.Forms.RadioButton radioAllowWithMessage;
		private System.Windows.Forms.RadioButton radioBlock;
		private System.Windows.Forms.RadioButton radioAllowWithInput;
		private System.Windows.Forms.Label labelCount;
	}
}