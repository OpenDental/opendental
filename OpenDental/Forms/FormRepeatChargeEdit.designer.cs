using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = OpenDental.UI.Button;

namespace OpenDental {
	public partial class FormRepeatChargeEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRepeatChargeEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.textCode = new System.Windows.Forms.TextBox();
			this.labelChargeAmount = new System.Windows.Forms.Label();
			this.labelDateStart = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.textDesc = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.checkCopyNoteToProc = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.checkCreatesClaim = new System.Windows.Forms.CheckBox();
			this.checkIsEnabled = new System.Windows.Forms.CheckBox();
			this.textTotalAmount = new System.Windows.Forms.TextBox();
			this.textNumOfCharges = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butCalculate = new OpenDental.UI.Button();
			this.butManual = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.textDateStop = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.textChargeAmt = new OpenDental.ValidDouble();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelBillingCycleDay = new System.Windows.Forms.Label();
			this.textBillingDay = new OpenDental.ValidNum();
			this.labelPatNum = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.butMoveTo = new OpenDental.UI.Button();
			this.labelNpi = new System.Windows.Forms.Label();
			this.labelErxAccountId = new System.Windows.Forms.Label();
			this.textNpi = new System.Windows.Forms.TextBox();
			this.textErxAccountId = new System.Windows.Forms.TextBox();
			this.checkUseUnearned = new System.Windows.Forms.CheckBox();
			this.labelProviderName = new System.Windows.Forms.Label();
			this.textProvName = new System.Windows.Forms.TextBox();
			this.textZipwhipChargeAmount = new OpenDental.ValidDouble();
			this.labelZipwhipAmt = new System.Windows.Forms.Label();
			this.comboUnearnedTypes = new OpenDental.UI.ComboBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(156, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(162, 17);
			this.textCode.MaxLength = 15;
			this.textCode.Name = "textCode";
			this.textCode.ReadOnly = true;
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 3;
			this.textCode.TabStop = false;
			// 
			// labelChargeAmount
			// 
			this.labelChargeAmount.Location = new System.Drawing.Point(4, 139);
			this.labelChargeAmount.Name = "labelChargeAmount";
			this.labelChargeAmount.Size = new System.Drawing.Size(156, 16);
			this.labelChargeAmount.TabIndex = 4;
			this.labelChargeAmount.Text = "Charge Amount";
			this.labelChargeAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateStart
			// 
			this.labelDateStart.Location = new System.Drawing.Point(4, 168);
			this.labelDateStart.Name = "labelDateStart";
			this.labelDateStart.Size = new System.Drawing.Size(156, 16);
			this.labelDateStart.TabIndex = 7;
			this.labelDateStart.Text = "Date Start";
			this.labelDateStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 196);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(156, 16);
			this.label4.TabIndex = 9;
			this.label4.Text = "Date Stop";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(162, 302);
			this.textNote.MaxLength = 10000;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(424, 114);
			this.textNote.TabIndex = 6;
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(4, 305);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(156, 16);
			this.labelNote.TabIndex = 10;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDesc
			// 
			this.textDesc.BackColor = System.Drawing.SystemColors.Control;
			this.textDesc.Location = new System.Drawing.Point(267, 17);
			this.textDesc.Name = "textDesc";
			this.textDesc.Size = new System.Drawing.Size(241, 20);
			this.textDesc.TabIndex = 40;
			this.textDesc.TabStop = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(265, 1);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(224, 16);
			this.label6.TabIndex = 39;
			this.label6.Text = "Procedure Description:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(128, 513);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(238, 29);
			this.label7.TabIndex = 42;
			this.label7.Text = "It\'s OK to delete an obsolete repeating charge.   It does not affect any charges " +
    "already billed.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkCopyNoteToProc
			// 
			this.checkCopyNoteToProc.Location = new System.Drawing.Point(162, 420);
			this.checkCopyNoteToProc.Name = "checkCopyNoteToProc";
			this.checkCopyNoteToProc.Size = new System.Drawing.Size(250, 18);
			this.checkCopyNoteToProc.TabIndex = 7;
			this.checkCopyNoteToProc.Text = "Copy note to procedure billing note.";
			this.checkCopyNoteToProc.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(17, 22);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(136, 16);
			this.label8.TabIndex = 44;
			this.label8.Text = "Total Amount";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(17, 48);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(136, 16);
			this.label9.TabIndex = 46;
			this.label9.Text = "Number of Charges";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkCreatesClaim
			// 
			this.checkCreatesClaim.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCreatesClaim.Location = new System.Drawing.Point(7, 218);
			this.checkCreatesClaim.Name = "checkCreatesClaim";
			this.checkCreatesClaim.Size = new System.Drawing.Size(169, 18);
			this.checkCreatesClaim.TabIndex = 4;
			this.checkCreatesClaim.Text = "Creates Claim";
			this.checkCreatesClaim.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCreatesClaim.UseVisualStyleBackColor = true;
			// 
			// checkIsEnabled
			// 
			this.checkIsEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEnabled.Location = new System.Drawing.Point(7, 282);
			this.checkIsEnabled.Name = "checkIsEnabled";
			this.checkIsEnabled.Size = new System.Drawing.Size(169, 18);
			this.checkIsEnabled.TabIndex = 5;
			this.checkIsEnabled.Text = "Enabled";
			this.checkIsEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsEnabled.UseVisualStyleBackColor = true;
			// 
			// textTotalAmount
			// 
			this.textTotalAmount.Location = new System.Drawing.Point(155, 19);
			this.textTotalAmount.Name = "textTotalAmount";
			this.textTotalAmount.Size = new System.Drawing.Size(100, 20);
			this.textTotalAmount.TabIndex = 0;
			// 
			// textNumOfCharges
			// 
			this.textNumOfCharges.Location = new System.Drawing.Point(155, 45);
			this.textNumOfCharges.Name = "textNumOfCharges";
			this.textNumOfCharges.Size = new System.Drawing.Size(100, 20);
			this.textNumOfCharges.TabIndex = 1;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(245, 461);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(225, 29);
			this.label10.TabIndex = 53;
			this.label10.Text = "This will add a completed procedure of the code listed above to this patient\'s ac" +
    "count.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butCalculate);
			this.groupBox1.Controls.Add(this.textTotalAmount);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.textNumOfCharges);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Location = new System.Drawing.Point(7, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(359, 79);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Calculate Charge Amount (optional)";
			// 
			// butCalculate
			// 
			this.butCalculate.Location = new System.Drawing.Point(261, 44);
			this.butCalculate.Name = "butCalculate";
			this.butCalculate.Size = new System.Drawing.Size(75, 24);
			this.butCalculate.TabIndex = 2;
			this.butCalculate.Text = "Calculate";
			this.butCalculate.Click += new System.EventHandler(this.butCalculate_Click);
			// 
			// butManual
			// 
			this.butManual.Location = new System.Drawing.Point(162, 463);
			this.butManual.Name = "butManual";
			this.butManual.Size = new System.Drawing.Size(75, 24);
			this.butManual.TabIndex = 12;
			this.butManual.Text = "Manual";
			this.butManual.Click += new System.EventHandler(this.butManual_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(35, 515);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 26);
			this.butDelete.TabIndex = 11;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(162, 194);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 3;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(162, 165);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 2;
			// 
			// textChargeAmt
			// 
			this.textChargeAmt.Location = new System.Drawing.Point(162, 136);
			this.textChargeAmt.MaxVal = 100000000D;
			this.textChargeAmt.MinVal = -100000000D;
			this.textChargeAmt.Name = "textChargeAmt";
			this.textChargeAmt.Size = new System.Drawing.Size(100, 20);
			this.textChargeAmt.TabIndex = 1;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(595, 472);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(595, 513);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelBillingCycleDay
			// 
			this.labelBillingCycleDay.Location = new System.Drawing.Point(372, 67);
			this.labelBillingCycleDay.Name = "labelBillingCycleDay";
			this.labelBillingCycleDay.Size = new System.Drawing.Size(137, 16);
			this.labelBillingCycleDay.TabIndex = 55;
			this.labelBillingCycleDay.Text = "Billing Cycle Day";
			this.labelBillingCycleDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelBillingCycleDay.Visible = false;
			// 
			// textBillingDay
			// 
			this.textBillingDay.Location = new System.Drawing.Point(511, 66);
			this.textBillingDay.MaxVal = 31;
			this.textBillingDay.MinVal = 1;
			this.textBillingDay.Name = "textBillingDay";
			this.textBillingDay.Size = new System.Drawing.Size(75, 20);
			this.textBillingDay.TabIndex = 8;
			this.textBillingDay.Visible = false;
			this.textBillingDay.ShowZero = false;
			// 
			// labelPatNum
			// 
			this.labelPatNum.Location = new System.Drawing.Point(400, 93);
			this.labelPatNum.Name = "labelPatNum";
			this.labelPatNum.Size = new System.Drawing.Size(110, 16);
			this.labelPatNum.TabIndex = 58;
			this.labelPatNum.Text = "PatNum";
			this.labelPatNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelPatNum.Visible = false;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(511, 92);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.ReadOnly = true;
			this.textPatNum.Size = new System.Drawing.Size(75, 20);
			this.textPatNum.TabIndex = 57;
			this.textPatNum.Visible = false;
			// 
			// butMoveTo
			// 
			this.butMoveTo.Location = new System.Drawing.Point(588, 90);
			this.butMoveTo.Name = "butMoveTo";
			this.butMoveTo.Size = new System.Drawing.Size(75, 24);
			this.butMoveTo.TabIndex = 56;
			this.butMoveTo.Text = "Move To";
			this.butMoveTo.Visible = false;
			this.butMoveTo.Click += new System.EventHandler(this.butMoveTo_Click);
			// 
			// labelNpi
			// 
			this.labelNpi.Location = new System.Drawing.Point(372, 119);
			this.labelNpi.Name = "labelNpi";
			this.labelNpi.Size = new System.Drawing.Size(137, 16);
			this.labelNpi.TabIndex = 60;
			this.labelNpi.Text = "NPI";
			this.labelNpi.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelNpi.Visible = false;
			// 
			// labelErxAccountId
			// 
			this.labelErxAccountId.Location = new System.Drawing.Point(372, 145);
			this.labelErxAccountId.Name = "labelErxAccountId";
			this.labelErxAccountId.Size = new System.Drawing.Size(137, 16);
			this.labelErxAccountId.TabIndex = 62;
			this.labelErxAccountId.Text = "ErxAccountId";
			this.labelErxAccountId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelErxAccountId.Visible = false;
			// 
			// textNpi
			// 
			this.textNpi.Location = new System.Drawing.Point(511, 118);
			this.textNpi.Name = "textNpi";
			this.textNpi.Size = new System.Drawing.Size(75, 20);
			this.textNpi.TabIndex = 63;
			this.textNpi.Visible = false;
			// 
			// textErxAccountId
			// 
			this.textErxAccountId.Location = new System.Drawing.Point(511, 144);
			this.textErxAccountId.Name = "textErxAccountId";
			this.textErxAccountId.Size = new System.Drawing.Size(75, 20);
			this.textErxAccountId.TabIndex = 64;
			this.textErxAccountId.Visible = false;
			// 
			// checkUseUnearned
			// 
			this.checkUseUnearned.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseUnearned.Location = new System.Drawing.Point(7, 238);
			this.checkUseUnearned.Name = "checkUseUnearned";
			this.checkUseUnearned.Size = new System.Drawing.Size(169, 18);
			this.checkUseUnearned.TabIndex = 66;
			this.checkUseUnearned.Text = "Use Unearned";
			this.checkUseUnearned.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseUnearned.UseVisualStyleBackColor = true;
			this.checkUseUnearned.CheckedChanged += new System.EventHandler(this.checkUseUnearned_CheckedChanged);
			// 
			// labelProviderName
			// 
			this.labelProviderName.Location = new System.Drawing.Point(375, 171);
			this.labelProviderName.Name = "labelProviderName";
			this.labelProviderName.Size = new System.Drawing.Size(135, 16);
			this.labelProviderName.TabIndex = 68;
			this.labelProviderName.Text = "ProviderName";
			this.labelProviderName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelProviderName.Visible = false;
			// 
			// textProvName
			// 
			this.textProvName.Location = new System.Drawing.Point(511, 170);
			this.textProvName.Name = "textProvName";
			this.textProvName.Size = new System.Drawing.Size(152, 20);
			this.textProvName.TabIndex = 67;
			this.textProvName.Visible = false;
			// 
			// textZipwhipChargeAmount
			// 
			this.textZipwhipChargeAmount.Location = new System.Drawing.Point(362, 136);
			this.textZipwhipChargeAmount.MaxVal = 100000000D;
			this.textZipwhipChargeAmount.MinVal = -100000000D;
			this.textZipwhipChargeAmount.Name = "textZipwhipChargeAmount";
			this.textZipwhipChargeAmount.Size = new System.Drawing.Size(75, 20);
			this.textZipwhipChargeAmount.TabIndex = 69;
			this.textZipwhipChargeAmount.Visible = false;
			// 
			// labelZipwhipAmt
			// 
			this.labelZipwhipAmt.Location = new System.Drawing.Point(270, 139);
			this.labelZipwhipAmt.Name = "labelZipwhipAmt";
			this.labelZipwhipAmt.Size = new System.Drawing.Size(90, 16);
			this.labelZipwhipAmt.TabIndex = 70;
			this.labelZipwhipAmt.Text = "Zipwhip Amount";
			this.labelZipwhipAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelZipwhipAmt.Visible = false;
			// 
			// comboUnearnedTypes
			// 
			this.comboUnearnedTypes.BackColor = System.Drawing.SystemColors.Window;
			this.comboUnearnedTypes.Enabled = false;
			this.comboUnearnedTypes.Location = new System.Drawing.Point(162, 258);
			this.comboUnearnedTypes.Name = "comboUnearnedTypes";
			this.comboUnearnedTypes.SelectionModeMulti = true;
			this.comboUnearnedTypes.Size = new System.Drawing.Size(120, 21);
			this.comboUnearnedTypes.TabIndex = 71;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 261);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 16);
			this.label2.TabIndex = 72;
			this.label2.Text = "Unearned Types";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormRepeatChargeEdit
			// 
			this.ClientSize = new System.Drawing.Size(705, 567);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboUnearnedTypes);
			this.Controls.Add(this.textZipwhipChargeAmount);
			this.Controls.Add(this.labelZipwhipAmt);
			this.Controls.Add(this.labelProviderName);
			this.Controls.Add(this.textProvName);
			this.Controls.Add(this.checkUseUnearned);
			this.Controls.Add(this.textErxAccountId);
			this.Controls.Add(this.textNpi);
			this.Controls.Add(this.labelErxAccountId);
			this.Controls.Add(this.labelNpi);
			this.Controls.Add(this.labelPatNum);
			this.Controls.Add(this.textPatNum);
			this.Controls.Add(this.butMoveTo);
			this.Controls.Add(this.textBillingDay);
			this.Controls.Add(this.labelBillingCycleDay);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.checkIsEnabled);
			this.Controls.Add(this.checkCreatesClaim);
			this.Controls.Add(this.butManual);
			this.Controls.Add(this.checkCopyNoteToProc);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textDesc);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelNote);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textDateStop);
			this.Controls.Add(this.labelDateStart);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.textChargeAmt);
			this.Controls.Add(this.labelChargeAmount);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRepeatChargeEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Repeat Charge";
			this.Load += new System.EventHandler(this.FormRepeatChargeEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private Button butCancel;
		private Button butOK;
		private Label label1;
		private TextBox textCode;
		private Label labelChargeAmount;
		private ValidDouble textChargeAmt;
		private ValidDate textDateStart;
		private Label labelDateStart;
		private Label label4;
		private ValidDate textDateStop;
		private TextBox textNote;
		private Label labelNote;
		private TextBox textDesc;
		private Label label6;
		private Button butDelete;
		private Label label7;
		private CheckBox checkCopyNoteToProc;
		private Label label8;
		private Label label9;
		private Button butManual;
		private CheckBox checkCreatesClaim;
		private CheckBox checkIsEnabled;
		private TextBox textTotalAmount;
		private TextBox textNumOfCharges;
		private Label label10;
		private GroupBox groupBox1;
		private Button butCalculate;
		private Label labelBillingCycleDay;
		private ValidNum textBillingDay;
		private Label labelPatNum;
		private TextBox textPatNum;
		private Button butMoveTo;
		private Label labelNpi;
		private Label labelErxAccountId;
		private TextBox textNpi;
		private TextBox textErxAccountId;
		private CheckBox checkUseUnearned;
		private Label labelProviderName;
		private TextBox textProvName;
		private ValidDouble textZipwhipChargeAmount;
		private Label labelZipwhipAmt;
		private UI.ComboBoxOD comboUnearnedTypes;
		private Label label2;
	}
}
