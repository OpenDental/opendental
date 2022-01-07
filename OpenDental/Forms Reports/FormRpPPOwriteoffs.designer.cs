using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpPPOwriteoffs {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpPPOwriteoffs));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.date2 = new System.Windows.Forms.MonthCalendar();
			this.date1 = new System.Windows.Forms.MonthCalendar();
			this.labelTO = new System.Windows.Forms.Label();
			this.radioIndividual = new System.Windows.Forms.RadioButton();
			this.radioGroup = new System.Windows.Forms.RadioButton();
			this.textCarrier = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.radioWriteoffClaimDate = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioWriteoffProcDate = new System.Windows.Forms.RadioButton();
			this.radioWriteoffInsPayDate = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(597, 330);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(597, 298);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// date2
			// 
			this.date2.Location = new System.Drawing.Point(289, 36);
			this.date2.MaxSelectionCount = 1;
			this.date2.Name = "date2";
			this.date2.TabIndex = 30;
			// 
			// date1
			// 
			this.date1.Location = new System.Drawing.Point(33, 36);
			this.date1.MaxSelectionCount = 1;
			this.date1.Name = "date1";
			this.date1.TabIndex = 29;
			// 
			// labelTO
			// 
			this.labelTO.Location = new System.Drawing.Point(249, 36);
			this.labelTO.Name = "labelTO";
			this.labelTO.Size = new System.Drawing.Size(51, 23);
			this.labelTO.TabIndex = 31;
			this.labelTO.Text = "TO";
			this.labelTO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// radioIndividual
			// 
			this.radioIndividual.Checked = true;
			this.radioIndividual.Location = new System.Drawing.Point(33, 227);
			this.radioIndividual.Name = "radioIndividual";
			this.radioIndividual.Size = new System.Drawing.Size(200, 19);
			this.radioIndividual.TabIndex = 32;
			this.radioIndividual.TabStop = true;
			this.radioIndividual.Text = "Individual Claims";
			this.radioIndividual.UseVisualStyleBackColor = true;
			// 
			// radioGroup
			// 
			this.radioGroup.Location = new System.Drawing.Point(33, 250);
			this.radioGroup.Name = "radioGroup";
			this.radioGroup.Size = new System.Drawing.Size(200, 19);
			this.radioGroup.TabIndex = 33;
			this.radioGroup.Text = "Group by Carrier";
			this.radioGroup.UseVisualStyleBackColor = true;
			// 
			// textCarrier
			// 
			this.textCarrier.Location = new System.Drawing.Point(33, 309);
			this.textCarrier.Name = "textCarrier";
			this.textCarrier.Size = new System.Drawing.Size(178, 20);
			this.textCarrier.TabIndex = 34;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(29, 283);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(257, 22);
			this.label1.TabIndex = 35;
			this.label1.Text = "Enter a few letters of the carrier to limit results";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.radioWriteoffClaimDate);
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.radioWriteoffProcDate);
			this.groupBox3.Controls.Add(this.radioWriteoffInsPayDate);
			this.groupBox3.Location = new System.Drawing.Point(289, 227);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(281, 129);
			this.groupBox3.TabIndex = 48;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Show Insurance Write-offs";
			// 
			// radioWriteoffClaimDate
			// 
			this.radioWriteoffClaimDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaimDate.Location = new System.Drawing.Point(9, 62);
			this.radioWriteoffClaimDate.Name = "radioWriteoffClaimDate";
			this.radioWriteoffClaimDate.Size = new System.Drawing.Size(244, 40);
			this.radioWriteoffClaimDate.TabIndex = 3;
			this.radioWriteoffClaimDate.Text = "Using initial claim date for write-off estimates, insurance pay date for write-of" +
    "f adjustments";
			this.radioWriteoffClaimDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffClaimDate.UseVisualStyleBackColor = true;
			this.radioWriteoffClaimDate.CheckedChanged += new System.EventHandler(this.radioWriteoffClaimDate_CheckedChanged);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 103);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(269, 17);
			this.label5.TabIndex = 2;
			this.label5.Text = "(This is discussed in the PPO section of the manual)";
			// 
			// radioWriteoffProcDate
			// 
			this.radioWriteoffProcDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProcDate.Location = new System.Drawing.Point(9, 41);
			this.radioWriteoffProcDate.Name = "radioWriteoffProcDate";
			this.radioWriteoffProcDate.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffProcDate.TabIndex = 1;
			this.radioWriteoffProcDate.Text = "Using procedure date.";
			this.radioWriteoffProcDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffProcDate.UseVisualStyleBackColor = true;
			// 
			// radioWriteoffInsPayDate
			// 
			this.radioWriteoffInsPayDate.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffInsPayDate.Checked = true;
			this.radioWriteoffInsPayDate.Location = new System.Drawing.Point(9, 20);
			this.radioWriteoffInsPayDate.Name = "radioWriteoffInsPayDate";
			this.radioWriteoffInsPayDate.Size = new System.Drawing.Size(244, 23);
			this.radioWriteoffInsPayDate.TabIndex = 0;
			this.radioWriteoffInsPayDate.TabStop = true;
			this.radioWriteoffInsPayDate.Text = "Using insurance payment date.";
			this.radioWriteoffInsPayDate.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.radioWriteoffInsPayDate.UseVisualStyleBackColor = true;
			// 
			// FormRpPPOwriteoffs
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(697, 379);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCarrier);
			this.Controls.Add(this.radioGroup);
			this.Controls.Add(this.radioIndividual);
			this.Controls.Add(this.date2);
			this.Controls.Add(this.date1);
			this.Controls.Add(this.labelTO);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpPPOwriteoffs";
			this.ShowInTaskbar = false;
			this.Text = "PPO Writeoffs";
			this.Load += new System.EventHandler(this.FormRpPPOwriteoffs_Load);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private MonthCalendar date2;
		private MonthCalendar date1;
		private Label labelTO;
		private RadioButton radioIndividual;
		private RadioButton radioGroup;
		private TextBox textCarrier;
		private Label label1;
		private GroupBox groupBox3;
		private Label label5;
		private RadioButton radioWriteoffProcDate;
		private RadioButton radioWriteoffInsPayDate;
		private RadioButton radioWriteoffClaimDate;
	}
}
