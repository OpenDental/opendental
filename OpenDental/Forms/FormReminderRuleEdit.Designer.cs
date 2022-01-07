namespace OpenDental {
	partial class FormReminderRuleEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReminderRuleEdit));
			this.butDelete = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.comboReminderCriterion = new System.Windows.Forms.ComboBox();
			this.textCriterionValue = new System.Windows.Forms.TextBox();
			this.textReminderMessage = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelCriterionValue = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.labelCriterionFK = new System.Windows.Forms.Label();
			this.textCriterionFK = new System.Windows.Forms.TextBox();
			this.butSelectFK = new System.Windows.Forms.Button();
			this.labelExample = new System.Windows.Forms.Label();
			this.textICD9 = new System.Windows.Forms.TextBox();
			this.labelICD9 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 178);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 0;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(278, 178);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 1;
			this.butOk.Text = "Ok";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(359, 178);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboReminderCriterion
			// 
			this.comboReminderCriterion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReminderCriterion.FormattingEnabled = true;
			this.comboReminderCriterion.Location = new System.Drawing.Point(114, 28);
			this.comboReminderCriterion.Name = "comboReminderCriterion";
			this.comboReminderCriterion.Size = new System.Drawing.Size(121, 21);
			this.comboReminderCriterion.TabIndex = 3;
			this.comboReminderCriterion.SelectedIndexChanged += new System.EventHandler(this.comboReminderCriterion_SelectedIndexChanged);
			// 
			// textCriterionValue
			// 
			this.textCriterionValue.Location = new System.Drawing.Point(114, 55);
			this.textCriterionValue.Name = "textCriterionValue";
			this.textCriterionValue.Size = new System.Drawing.Size(100, 20);
			this.textCriterionValue.TabIndex = 4;
			// 
			// textReminderMessage
			// 
			this.textReminderMessage.Location = new System.Drawing.Point(114, 133);
			this.textReminderMessage.Name = "textReminderMessage";
			this.textReminderMessage.Size = new System.Drawing.Size(320, 20);
			this.textReminderMessage.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 29);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 17);
			this.label1.TabIndex = 6;
			this.label1.Text = "Reminder Criterion";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCriterionValue
			// 
			this.labelCriterionValue.Location = new System.Drawing.Point(5, 56);
			this.labelCriterionValue.Name = "labelCriterionValue";
			this.labelCriterionValue.Size = new System.Drawing.Size(107, 17);
			this.labelCriterionValue.TabIndex = 7;
			this.labelCriterionValue.Text = "Criterion Value";
			this.labelCriterionValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 134);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(107, 17);
			this.label3.TabIndex = 8;
			this.label3.Text = "Reminder Message";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCriterionFK
			// 
			this.labelCriterionFK.Location = new System.Drawing.Point(5, 82);
			this.labelCriterionFK.Name = "labelCriterionFK";
			this.labelCriterionFK.Size = new System.Drawing.Size(107, 17);
			this.labelCriterionFK.TabIndex = 10;
			this.labelCriterionFK.Text = "Criterion FK";
			this.labelCriterionFK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCriterionFK
			// 
			this.textCriterionFK.Location = new System.Drawing.Point(114, 81);
			this.textCriterionFK.Name = "textCriterionFK";
			this.textCriterionFK.ReadOnly = true;
			this.textCriterionFK.Size = new System.Drawing.Size(290, 20);
			this.textCriterionFK.TabIndex = 9;
			// 
			// butSelectFK
			// 
			this.butSelectFK.Location = new System.Drawing.Point(410, 78);
			this.butSelectFK.Name = "butSelectFK";
			this.butSelectFK.Size = new System.Drawing.Size(24, 24);
			this.butSelectFK.TabIndex = 11;
			this.butSelectFK.Text = "...";
			this.butSelectFK.UseVisualStyleBackColor = true;
			this.butSelectFK.Click += new System.EventHandler(this.butSelectFK_Click);
			// 
			// labelExample
			// 
			this.labelExample.Location = new System.Drawing.Point(220, 56);
			this.labelExample.Name = "labelExample";
			this.labelExample.Size = new System.Drawing.Size(107, 17);
			this.labelExample.TabIndex = 12;
			this.labelExample.Text = "For example, <18";
			this.labelExample.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textICD9
			// 
			this.textICD9.Location = new System.Drawing.Point(114, 107);
			this.textICD9.Name = "textICD9";
			this.textICD9.ReadOnly = true;
			this.textICD9.Size = new System.Drawing.Size(139, 20);
			this.textICD9.TabIndex = 9;
			this.textICD9.Visible = false;
			// 
			// labelICD9
			// 
			this.labelICD9.Location = new System.Drawing.Point(5, 108);
			this.labelICD9.Name = "labelICD9";
			this.labelICD9.Size = new System.Drawing.Size(107, 17);
			this.labelICD9.TabIndex = 10;
			this.labelICD9.Text = "ICD-9";
			this.labelICD9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelICD9.Visible = false;
			// 
			// FormReminderRuleEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(446, 213);
			this.Controls.Add(this.labelExample);
			this.Controls.Add(this.butSelectFK);
			this.Controls.Add(this.labelICD9);
			this.Controls.Add(this.labelCriterionFK);
			this.Controls.Add(this.textICD9);
			this.Controls.Add(this.textCriterionFK);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelCriterionValue);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textReminderMessage);
			this.Controls.Add(this.textCriterionValue);
			this.Controls.Add(this.comboReminderCriterion);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butDelete);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReminderRuleEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Reminder Rule";
			this.Load += new System.EventHandler(this.FormReminderRuleEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.ComboBox comboReminderCriterion;
		private System.Windows.Forms.TextBox textCriterionValue;
		private System.Windows.Forms.TextBox textReminderMessage;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelCriterionValue;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelCriterionFK;
		private System.Windows.Forms.TextBox textCriterionFK;
		private System.Windows.Forms.Button butSelectFK;
		private System.Windows.Forms.Label labelExample;
		private System.Windows.Forms.TextBox textICD9;
		private System.Windows.Forms.Label labelICD9;
	}
}