using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClaimPayEdit {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimPayEdit));
			this.textAmount = new OpenDental.ValidDouble();
			this.textDate = new OpenDental.ValidDate();
			this.textBankBranch = new System.Windows.Forms.TextBox();
			this.textCheckNum = new System.Windows.Forms.TextBox();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelClinic = new System.Windows.Forms.Label();
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.labelDateIssued = new System.Windows.Forms.Label();
			this.textDateIssued = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.butCarrierSelect = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.comboPayType = new System.Windows.Forms.ComboBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butPickPaymentGroup = new OpenDental.UI.Button();
			this.comboPayGroup = new System.Windows.Forms.ComboBox();
			this.labelClaimPaymentGroup = new System.Windows.Forms.Label();
			this.panelXcharge = new System.Windows.Forms.Panel();
			this.groupPrepaid = new System.Windows.Forms.GroupBox();
			this.butPaySimple = new System.Windows.Forms.Panel();
			this.butPayConnect = new OpenDental.UI.Button();
			this.groupBoxDeposit = new System.Windows.Forms.GroupBox();
			this.labelDepositAccountNum = new System.Windows.Forms.Label();
			this.comboDepositAccountNum = new OpenDental.UI.ComboBoxOD();
			this.textBoxBatchNum = new System.Windows.Forms.TextBox();
			this.butDepositEdit = new OpenDental.UI.Button();
			this.labelBatchNum = new System.Windows.Forms.Label();
			this.labelDepositDate = new System.Windows.Forms.Label();
			this.validDepositDate = new OpenDental.ValidDate();
			this.labelDepositAmount = new System.Windows.Forms.Label();
			this.validDoubleDepositAmt = new OpenDental.ValidDouble();
			this.labelRequiredFields = new System.Windows.Forms.Label();
			this.panelEdgeExpress = new System.Windows.Forms.Panel();
			this.groupPrepaid.SuspendLayout();
			this.groupBoxDeposit.SuspendLayout();
			this.SuspendLayout();
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(159, 135);
			this.textAmount.MaxVal = 100000000D;
			this.textAmount.MinVal = -100000000D;
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(68, 20);
			this.textAmount.TabIndex = 0;
			this.textAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.textAmount.Leave += new System.EventHandler(this.TextAmount_Leave);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(159, 93);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(68, 20);
			this.textDate.TabIndex = 6;
			this.textDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textBankBranch
			// 
			this.textBankBranch.Location = new System.Drawing.Point(159, 177);
			this.textBankBranch.MaxLength = 25;
			this.textBankBranch.Name = "textBankBranch";
			this.textBankBranch.Size = new System.Drawing.Size(100, 20);
			this.textBankBranch.TabIndex = 2;
			// 
			// textCheckNum
			// 
			this.textCheckNum.Location = new System.Drawing.Point(159, 156);
			this.textCheckNum.MaxLength = 25;
			this.textCheckNum.Name = "textCheckNum";
			this.textCheckNum.Size = new System.Drawing.Size(100, 20);
			this.textCheckNum.TabIndex = 1;
			// 
			// textNote
			// 
			this.textNote.AcceptsReturn = true;
			this.textNote.Location = new System.Drawing.Point(159, 247);
			this.textNote.MaxLength = 255;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(335, 73);
			this.textNote.TabIndex = 4;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 97);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(144, 16);
			this.label6.TabIndex = 37;
			this.label6.Text = "Payment Posting Date";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(37, 139);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(120, 16);
			this.label5.TabIndex = 36;
			this.label5.Text = "Amount";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(37, 158);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(119, 16);
			this.label4.TabIndex = 35;
			this.label4.Text = "Check #";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(37, 180);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(121, 16);
			this.label3.TabIndex = 34;
			this.label3.Text = "Bank-Branch";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 248);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(118, 16);
			this.label2.TabIndex = 33;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(425, 519);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(334, 519);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.ForceShowUnassigned = true;
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(159, 21);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.ShowLabel = false;
			this.comboClinic.Size = new System.Drawing.Size(209, 21);
			this.comboClinic.TabIndex = 0;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(37, 25);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(118, 14);
			this.labelClinic.TabIndex = 91;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(159, 198);
			this.textCarrierName.MaxLength = 25;
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.Size = new System.Drawing.Size(263, 20);
			this.textCarrierName.TabIndex = 3;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(37, 201);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(121, 16);
			this.label7.TabIndex = 94;
			this.label7.Text = "Carrier Name";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelDateIssued
			// 
			this.labelDateIssued.Location = new System.Drawing.Point(15, 118);
			this.labelDateIssued.Name = "labelDateIssued";
			this.labelDateIssued.Size = new System.Drawing.Size(142, 16);
			this.labelDateIssued.TabIndex = 37;
			this.labelDateIssued.Text = "Check EFT Issue Date";
			this.labelDateIssued.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateIssued
			// 
			this.textDateIssued.Location = new System.Drawing.Point(159, 114);
			this.textDateIssued.Name = "textDateIssued";
			this.textDateIssued.Size = new System.Drawing.Size(68, 20);
			this.textDateIssued.TabIndex = 7;
			this.textDateIssued.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(228, 115);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(94, 16);
			this.label1.TabIndex = 95;
			this.label1.Text = "(optional)";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butCarrierSelect
			// 
			this.butCarrierSelect.Location = new System.Drawing.Point(425, 196);
			this.butCarrierSelect.Name = "butCarrierSelect";
			this.butCarrierSelect.Size = new System.Drawing.Size(69, 23);
			this.butCarrierSelect.TabIndex = 8;
			this.butCarrierSelect.Text = "Pick";
			this.butCarrierSelect.Click += new System.EventHandler(this.butCarrierSelect_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(156, 221);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(200, 16);
			this.label8.TabIndex = 96;
			this.label8.Text = "(does not need to be exact)";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(262, 178);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(94, 16);
			this.label9.TabIndex = 97;
			this.label9.Text = "(optional)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(262, 157);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(94, 16);
			this.label10.TabIndex = 98;
			this.label10.Text = "(optional)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboPayType
			// 
			this.comboPayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayType.Location = new System.Drawing.Point(159, 44);
			this.comboPayType.MaxDropDownItems = 30;
			this.comboPayType.Name = "comboPayType";
			this.comboPayType.Size = new System.Drawing.Size(209, 21);
			this.comboPayType.TabIndex = 99;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(32, 47);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(118, 14);
			this.label11.TabIndex = 100;
			this.label11.Text = "Payment Type";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butPickPaymentGroup
			// 
			this.butPickPaymentGroup.Location = new System.Drawing.Point(369, 67);
			this.butPickPaymentGroup.Name = "butPickPaymentGroup";
			this.butPickPaymentGroup.Size = new System.Drawing.Size(23, 21);
			this.butPickPaymentGroup.TabIndex = 103;
			this.butPickPaymentGroup.Text = "...";
			this.butPickPaymentGroup.Click += new System.EventHandler(this.butPickPaymentGroup_Click);
			// 
			// comboPayGroup
			// 
			this.comboPayGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPayGroup.Location = new System.Drawing.Point(159, 67);
			this.comboPayGroup.MaxDropDownItems = 40;
			this.comboPayGroup.Name = "comboPayGroup";
			this.comboPayGroup.Size = new System.Drawing.Size(209, 21);
			this.comboPayGroup.TabIndex = 102;
			// 
			// labelClaimPaymentGroup
			// 
			this.labelClaimPaymentGroup.Location = new System.Drawing.Point(12, 70);
			this.labelClaimPaymentGroup.Name = "labelClaimPaymentGroup";
			this.labelClaimPaymentGroup.Size = new System.Drawing.Size(143, 14);
			this.labelClaimPaymentGroup.TabIndex = 101;
			this.labelClaimPaymentGroup.Text = "Claim Payment Group";
			this.labelClaimPaymentGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelXcharge
			// 
			this.panelXcharge.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelXcharge.BackgroundImage")));
			this.panelXcharge.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panelXcharge.Location = new System.Drawing.Point(9, 21);
			this.panelXcharge.Name = "panelXcharge";
			this.panelXcharge.Size = new System.Drawing.Size(59, 26);
			this.panelXcharge.TabIndex = 119;
			this.panelXcharge.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelXcharge_MouseClick);
			// 
			// groupPrepaid
			// 
			this.groupPrepaid.Controls.Add(this.butPayConnect);
			this.groupPrepaid.Controls.Add(this.panelEdgeExpress);
			this.groupPrepaid.Controls.Add(this.butPaySimple);
			this.groupPrepaid.Controls.Add(this.panelXcharge);
			this.groupPrepaid.Location = new System.Drawing.Point(159, 326);
			this.groupPrepaid.Name = "groupPrepaid";
			this.groupPrepaid.Size = new System.Drawing.Size(335, 57);
			this.groupPrepaid.TabIndex = 120;
			this.groupPrepaid.TabStop = false;
			this.groupPrepaid.Text = "Virtual Credit Card Payment";
			// 
			// butPaySimple
			// 
			this.butPaySimple.BackgroundImage = global::OpenDental.Properties.Resources.PaySimple_Button_2019_26x75;
			this.butPaySimple.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.butPaySimple.Location = new System.Drawing.Point(250, 21);
			this.butPaySimple.Name = "butPaySimple";
			this.butPaySimple.Size = new System.Drawing.Size(76, 26);
			this.butPaySimple.TabIndex = 131;
			this.butPaySimple.MouseClick += new System.Windows.Forms.MouseEventHandler(this.butPaySimple_MouseClick);
			// 
			// butPayConnect
			// 
			this.butPayConnect.Location = new System.Drawing.Point(159, 22);
			this.butPayConnect.Name = "butPayConnect";
			this.butPayConnect.Size = new System.Drawing.Size(75, 24);
			this.butPayConnect.TabIndex = 130;
			this.butPayConnect.Text = "PayConnect";
			this.butPayConnect.Click += new System.EventHandler(this.butPayConnect_Click);
			// 
			// groupBoxDeposit
			// 
			this.groupBoxDeposit.Controls.Add(this.labelDepositAccountNum);
			this.groupBoxDeposit.Controls.Add(this.comboDepositAccountNum);
			this.groupBoxDeposit.Controls.Add(this.textBoxBatchNum);
			this.groupBoxDeposit.Controls.Add(this.butDepositEdit);
			this.groupBoxDeposit.Controls.Add(this.labelBatchNum);
			this.groupBoxDeposit.Controls.Add(this.labelDepositDate);
			this.groupBoxDeposit.Controls.Add(this.validDepositDate);
			this.groupBoxDeposit.Controls.Add(this.labelDepositAmount);
			this.groupBoxDeposit.Controls.Add(this.validDoubleDepositAmt);
			this.groupBoxDeposit.Location = new System.Drawing.Point(15, 389);
			this.groupBoxDeposit.Name = "groupBoxDeposit";
			this.groupBoxDeposit.Size = new System.Drawing.Size(493, 118);
			this.groupBoxDeposit.TabIndex = 121;
			this.groupBoxDeposit.TabStop = false;
			this.groupBoxDeposit.Text = "Deposit Details";
			// 
			// labelDepositAccountNum
			// 
			this.labelDepositAccountNum.Location = new System.Drawing.Point(6, 89);
			this.labelDepositAccountNum.Name = "labelDepositAccountNum";
			this.labelDepositAccountNum.Size = new System.Drawing.Size(137, 14);
			this.labelDepositAccountNum.TabIndex = 111;
			this.labelDepositAccountNum.Text = "Auto Deposit Account";
			this.labelDepositAccountNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDepositAccountNum
			// 
			this.comboDepositAccountNum.Location = new System.Drawing.Point(144, 87);
			this.comboDepositAccountNum.Name = "comboDepositAccountNum";
			this.comboDepositAccountNum.Size = new System.Drawing.Size(209, 21);
			this.comboDepositAccountNum.TabIndex = 110;
			// 
			// textBoxBatchNum
			// 
			this.textBoxBatchNum.Location = new System.Drawing.Point(144, 62);
			this.textBoxBatchNum.MaxLength = 25;
			this.textBoxBatchNum.Name = "textBoxBatchNum";
			this.textBoxBatchNum.Size = new System.Drawing.Size(209, 20);
			this.textBoxBatchNum.TabIndex = 109;
			// 
			// butDepositEdit
			// 
			this.butDepositEdit.Location = new System.Drawing.Point(410, 14);
			this.butDepositEdit.Name = "butDepositEdit";
			this.butDepositEdit.Size = new System.Drawing.Size(69, 23);
			this.butDepositEdit.TabIndex = 107;
			this.butDepositEdit.Text = "Edit";
			this.butDepositEdit.UseVisualStyleBackColor = true;
			this.butDepositEdit.Visible = false;
			this.butDepositEdit.Click += new System.EventHandler(this.butDepositEdit_Click);
			// 
			// labelBatchNum
			// 
			this.labelBatchNum.Location = new System.Drawing.Point(54, 63);
			this.labelBatchNum.Name = "labelBatchNum";
			this.labelBatchNum.Size = new System.Drawing.Size(87, 16);
			this.labelBatchNum.TabIndex = 108;
			this.labelBatchNum.Text = "Batch #";
			this.labelBatchNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDepositDate
			// 
			this.labelDepositDate.Location = new System.Drawing.Point(60, 21);
			this.labelDepositDate.Name = "labelDepositDate";
			this.labelDepositDate.Size = new System.Drawing.Size(82, 16);
			this.labelDepositDate.TabIndex = 105;
			this.labelDepositDate.Text = "Date";
			this.labelDepositDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// validDepositDate
			// 
			this.validDepositDate.Location = new System.Drawing.Point(144, 20);
			this.validDepositDate.Name = "validDepositDate";
			this.validDepositDate.Size = new System.Drawing.Size(68, 20);
			this.validDepositDate.TabIndex = 102;
			this.validDepositDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelDepositAmount
			// 
			this.labelDepositAmount.Location = new System.Drawing.Point(57, 42);
			this.labelDepositAmount.Name = "labelDepositAmount";
			this.labelDepositAmount.Size = new System.Drawing.Size(84, 16);
			this.labelDepositAmount.TabIndex = 106;
			this.labelDepositAmount.Text = "Amount";
			this.labelDepositAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// validDoubleDepositAmt
			// 
			this.validDoubleDepositAmt.Enabled = false;
			this.validDoubleDepositAmt.Location = new System.Drawing.Point(144, 41);
			this.validDoubleDepositAmt.MaxVal = 100000000D;
			this.validDoubleDepositAmt.MinVal = -100000000D;
			this.validDoubleDepositAmt.Name = "validDoubleDepositAmt";
			this.validDoubleDepositAmt.Size = new System.Drawing.Size(68, 20);
			this.validDoubleDepositAmt.TabIndex = 104;
			this.validDoubleDepositAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRequiredFields
			// 
			this.labelRequiredFields.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRequiredFields.Location = new System.Drawing.Point(142, 525);
			this.labelRequiredFields.Name = "labelRequiredFields";
			this.labelRequiredFields.Size = new System.Drawing.Size(180, 14);
			this.labelRequiredFields.TabIndex = 122;
			this.labelRequiredFields.Text = "* Indicates Required Field";
			this.labelRequiredFields.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelRequiredFields.Visible = false;
			// 
			// panelEdgeExpress
			// 
			this.panelEdgeExpress.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panelEdgeExpress.BackgroundImage")));
			this.panelEdgeExpress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panelEdgeExpress.Location = new System.Drawing.Point(84, 21);
			this.panelEdgeExpress.Name = "panelEdgeExpress";
			this.panelEdgeExpress.Size = new System.Drawing.Size(59, 26);
			this.panelEdgeExpress.TabIndex = 132;
			this.panelEdgeExpress.Visible = false;
			this.panelEdgeExpress.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelEdgeExpress_MouseClick);
			// 
			// FormClaimPayEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(520, 554);
			this.Controls.Add(this.labelRequiredFields);
			this.Controls.Add(this.groupBoxDeposit);
			this.Controls.Add(this.groupPrepaid);
			this.Controls.Add(this.butPickPaymentGroup);
			this.Controls.Add(this.comboPayGroup);
			this.Controls.Add(this.labelClaimPaymentGroup);
			this.Controls.Add(this.comboPayType);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCarrierName);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelClinic);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.textDateIssued);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.textBankBranch);
			this.Controls.Add(this.textCheckNum);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelDateIssued);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butCarrierSelect);
			this.Controls.Add(this.butOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClaimPayEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Insurance Payment";
			this.Load += new System.EventHandler(this.FormClaimPayEdit_Load);
			this.groupPrepaid.ResumeLayout(false);
			this.groupBoxDeposit.ResumeLayout(false);
			this.groupBoxDeposit.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.ValidDouble textAmount;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.TextBox textBankBranch;
		private System.Windows.Forms.TextBox textCheckNum;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private System.Windows.Forms.TextBox textCarrierName;
		private System.Windows.Forms.Label label7;
		private Label labelDateIssued;
		private ValidDate textDateIssued;
		private UI.Button butCarrierSelect;
		private Label label8;
		private Label label9;
		private Label label10;
		private ComboBox comboPayType;
		private Label label11;
		private Label label1;
		private UI.Button butPickPaymentGroup;
		private ComboBox comboPayGroup;
		private Label labelClaimPaymentGroup;
		private Panel panelXcharge;
		private GroupBox groupPrepaid;
		private UI.Button butPayConnect;
		private GroupBox groupBoxDeposit;
		private ValidDate validDepositDate;
		private ValidDouble validDoubleDepositAmt;
		private Label labelDepositAmount;
		private Label labelDepositDate;
		private UI.Button butDepositEdit;
		private Label labelBatchNum;
		private TextBox textBoxBatchNum;
		private Label labelDepositAccountNum;
		private UI.ComboBoxOD comboDepositAccountNum;
		private Panel butPaySimple;
		private Label labelRequiredFields;
		private Panel panelEdgeExpress;
	}
}
