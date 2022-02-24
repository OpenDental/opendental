namespace OpenDental{
	partial class FormProcEditExplain {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcEditExplain));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textSummary = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textExplanation = new System.Windows.Forms.TextBox();
			this.radioButtonNewProv = new System.Windows.Forms.RadioButton();
			this.radioButtonReAssign = new System.Windows.Forms.RadioButton();
			this.radioButtonError = new System.Windows.Forms.RadioButton();
			this.radioButtonOther = new System.Windows.Forms.RadioButton();
			this.groupBoxDPC = new System.Windows.Forms.GroupBox();
			this.groupBoxDPC.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(625, 485);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(625, 526);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(159, 16);
			this.label1.TabIndex = 4;
			this.label1.Text = "Summary of Changes Made";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSummary
			// 
			this.textSummary.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSummary.Location = new System.Drawing.Point(15, 28);
			this.textSummary.Multiline = true;
			this.textSummary.Name = "textSummary";
			this.textSummary.ReadOnly = true;
			this.textSummary.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textSummary.Size = new System.Drawing.Size(685, 200);
			this.textSummary.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 236);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(159, 16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Explanation";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textExplanation
			// 
			this.textExplanation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textExplanation.Location = new System.Drawing.Point(15, 255);
			this.textExplanation.Multiline = true;
			this.textExplanation.Name = "textExplanation";
			this.textExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textExplanation.Size = new System.Drawing.Size(685, 200);
			this.textExplanation.TabIndex = 7;
			// 
			// radioButtonNewProv
			// 
			this.radioButtonNewProv.AutoSize = true;
			this.radioButtonNewProv.Location = new System.Drawing.Point(89, 29);
			this.radioButtonNewProv.Name = "radioButtonNewProv";
			this.radioButtonNewProv.Size = new System.Drawing.Size(88, 17);
			this.radioButtonNewProv.TabIndex = 11;
			this.radioButtonNewProv.Text = "New provider";
			this.radioButtonNewProv.UseVisualStyleBackColor = true;
			this.radioButtonNewProv.CheckedChanged += new System.EventHandler(this.radioButtonNewProv_CheckedChanged);
			// 
			// radioButtonReAssign
			// 
			this.radioButtonReAssign.Location = new System.Drawing.Point(183, 29);
			this.radioButtonReAssign.Name = "radioButtonReAssign";
			this.radioButtonReAssign.Size = new System.Drawing.Size(97, 17);
			this.radioButtonReAssign.TabIndex = 10;
			this.radioButtonReAssign.Text = "Re-assignment";
			this.radioButtonReAssign.UseVisualStyleBackColor = true;
			this.radioButtonReAssign.CheckedChanged += new System.EventHandler(this.radioButtonReAssign_CheckedChanged);
			// 
			// radioButtonError
			// 
			this.radioButtonError.AutoSize = true;
			this.radioButtonError.Location = new System.Drawing.Point(10, 29);
			this.radioButtonError.Name = "radioButtonError";
			this.radioButtonError.Size = new System.Drawing.Size(73, 17);
			this.radioButtonError.TabIndex = 9;
			this.radioButtonError.Text = "Entry error";
			this.radioButtonError.UseVisualStyleBackColor = true;
			this.radioButtonError.CheckedChanged += new System.EventHandler(this.radioButtonError_CheckedChanged);
			// 
			// radioButtonOther
			// 
			this.radioButtonOther.AutoSize = true;
			this.radioButtonOther.Location = new System.Drawing.Point(284, 29);
			this.radioButtonOther.Name = "radioButtonOther";
			this.radioButtonOther.Size = new System.Drawing.Size(51, 17);
			this.radioButtonOther.TabIndex = 12;
			this.radioButtonOther.Text = "Other";
			this.radioButtonOther.UseVisualStyleBackColor = true;
			this.radioButtonOther.CheckedChanged += new System.EventHandler(this.radioButtonOther_CheckedChanged);
			// 
			// groupBoxDPC
			// 
			this.groupBoxDPC.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBoxDPC.Controls.Add(this.radioButtonOther);
			this.groupBoxDPC.Controls.Add(this.radioButtonError);
			this.groupBoxDPC.Controls.Add(this.radioButtonNewProv);
			this.groupBoxDPC.Controls.Add(this.radioButtonReAssign);
			this.groupBoxDPC.Enabled = false;
			this.groupBoxDPC.Location = new System.Drawing.Point(15, 484);
			this.groupBoxDPC.Name = "groupBoxDPC";
			this.groupBoxDPC.Size = new System.Drawing.Size(355, 66);
			this.groupBoxDPC.TabIndex = 13;
			this.groupBoxDPC.TabStop = false;
			this.groupBoxDPC.Text = "Reason for DPC change:";
			// 
			// FormProcEditExplain
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(725, 577);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBoxDPC);
			this.Controls.Add(this.textExplanation);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textSummary);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcEditExplain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Procedure Edit Explanation";
			this.Load += new System.EventHandler(this.FormProcEditExplain_Load);
			this.groupBoxDPC.ResumeLayout(false);
			this.groupBoxDPC.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textSummary;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textExplanation;
		private System.Windows.Forms.RadioButton radioButtonNewProv;
		private System.Windows.Forms.RadioButton radioButtonReAssign;
		private System.Windows.Forms.RadioButton radioButtonError;
		private System.Windows.Forms.RadioButton radioButtonOther;
		private System.Windows.Forms.GroupBox groupBoxDPC;
	}
}