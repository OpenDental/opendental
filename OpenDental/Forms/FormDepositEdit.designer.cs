using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDepositEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDepositEdit));
			this.groupSelect = new OpenDental.UI.GroupBox();
			this.comboClassRefs = new OpenDental.UI.ComboBox();
			this.labelClassRef = new System.Windows.Forms.Label();
			this.listInsPayType = new OpenDental.UI.ListBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.textDateStart = new OpenDental.ValidDate();
			this.label5 = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelClinic = new System.Windows.Forms.Label();
			this.listPayType = new OpenDental.UI.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textBankAccountInfo = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.comboDepositAccount = new OpenDental.UI.ComboBox();
			this.labelDepositAccount = new System.Windows.Forms.Label();
			this.textDepositAccount = new System.Windows.Forms.TextBox();
			this.textMemo = new System.Windows.Forms.TextBox();
			this.labelMemo = new System.Windows.Forms.Label();
			this.textAmountSearch = new System.Windows.Forms.TextBox();
			this.textCheckNumSearch = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textItemNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.butSendQB = new OpenDental.UI.Button();
			this.gridIns = new OpenDental.UI.GridOD();
			this.butPrint = new OpenDental.UI.Button();
			this.textDate = new OpenDental.ValidDate();
			this.butDelete = new OpenDental.UI.Button();
			this.gridPat = new OpenDental.UI.GridOD();
			this.butSave = new OpenDental.UI.Button();
			this.butPDF = new OpenDental.UI.Button();
			this.butEmailPDF = new OpenDental.UI.Button();
			this.labelDepositAccountNum = new System.Windows.Forms.Label();
			this.comboDepositAccountNum = new OpenDental.UI.ComboBox();
			this.textBatch = new System.Windows.Forms.TextBox();
			this.labelBatchNum = new System.Windows.Forms.Label();
			this.groupSelect.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupSelect
			// 
			this.groupSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSelect.Controls.Add(this.comboClassRefs);
			this.groupSelect.Controls.Add(this.labelClassRef);
			this.groupSelect.Controls.Add(this.listInsPayType);
			this.groupSelect.Controls.Add(this.label6);
			this.groupSelect.Controls.Add(this.butRefresh);
			this.groupSelect.Controls.Add(this.textDateStart);
			this.groupSelect.Controls.Add(this.label5);
			this.groupSelect.Controls.Add(this.comboClinic);
			this.groupSelect.Controls.Add(this.labelClinic);
			this.groupSelect.Controls.Add(this.listPayType);
			this.groupSelect.Controls.Add(this.label2);
			this.groupSelect.Location = new System.Drawing.Point(602, 326);
			this.groupSelect.Name = "groupSelect";
			this.groupSelect.Size = new System.Drawing.Size(355, 299);
			this.groupSelect.TabIndex = 0;
			this.groupSelect.TabStop = false;
			this.groupSelect.Text = "Show";
			// 
			// comboClassRefs
			// 
			this.comboClassRefs.Location = new System.Drawing.Point(184, 68);
			this.comboClassRefs.Name = "comboClassRefs";
			this.comboClassRefs.Size = new System.Drawing.Size(165, 21);
			this.comboClassRefs.TabIndex = 0;
			this.comboClassRefs.TabStop = false;
			this.comboClassRefs.Visible = false;
			// 
			// labelClassRef
			// 
			this.labelClassRef.Location = new System.Drawing.Point(184, 51);
			this.labelClassRef.Name = "labelClassRef";
			this.labelClassRef.Size = new System.Drawing.Size(102, 16);
			this.labelClassRef.TabIndex = 0;
			this.labelClassRef.Text = "Class";
			this.labelClassRef.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelClassRef.Visible = false;
			// 
			// listInsPayType
			// 
			this.listInsPayType.Location = new System.Drawing.Point(184, 111);
			this.listInsPayType.Name = "listInsPayType";
			this.listInsPayType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listInsPayType.Size = new System.Drawing.Size(165, 147);
			this.listInsPayType.TabIndex = 0;
			this.listInsPayType.TabStop = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(184, 94);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(165, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "Insurance Payment Types";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(142, 264);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 0;
			this.butRefresh.TabStop = false;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(14, 31);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(94, 20);
			this.textDateStart.TabIndex = 0;
			this.textDateStart.TabStop = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(14, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(118, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Start Date";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(14, 68);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.ShowLabel = false;
			this.comboClinic.Size = new System.Drawing.Size(165, 21);
			this.comboClinic.TabIndex = 0;
			this.comboClinic.TabStop = false;
			// 
			// labelClinic
			// 
			this.labelClinic.Location = new System.Drawing.Point(14, 51);
			this.labelClinic.Name = "labelClinic";
			this.labelClinic.Size = new System.Drawing.Size(102, 16);
			this.labelClinic.TabIndex = 0;
			this.labelClinic.Text = "Clinic";
			this.labelClinic.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPayType
			// 
			this.listPayType.Location = new System.Drawing.Point(14, 111);
			this.listPayType.Name = "listPayType";
			this.listPayType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listPayType.Size = new System.Drawing.Size(165, 147);
			this.listPayType.TabIndex = 0;
			this.listPayType.TabStop = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 94);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(165, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Patient Payment Types";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(602, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(95, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Location = new System.Drawing.Point(601, 89);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(238, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Bank Account Info";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textBankAccountInfo
			// 
			this.textBankAccountInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBankAccountInfo.Location = new System.Drawing.Point(601, 106);
			this.textBankAccountInfo.Multiline = true;
			this.textBankAccountInfo.Name = "textBankAccountInfo";
			this.textBankAccountInfo.Size = new System.Drawing.Size(289, 59);
			this.textBankAccountInfo.TabIndex = 103;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(702, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Amount";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textAmount
			// 
			this.textAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textAmount.Location = new System.Drawing.Point(702, 25);
			this.textAmount.Name = "textAmount";
			this.textAmount.ReadOnly = true;
			this.textAmount.Size = new System.Drawing.Size(94, 20);
			this.textAmount.TabIndex = 0;
			this.textAmount.TabStop = false;
			// 
			// comboDepositAccount
			// 
			this.comboDepositAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboDepositAccount.Location = new System.Drawing.Point(602, 235);
			this.comboDepositAccount.Name = "comboDepositAccount";
			this.comboDepositAccount.Size = new System.Drawing.Size(289, 21);
			this.comboDepositAccount.TabIndex = 110;
			this.comboDepositAccount.TabStop = false;
			// 
			// labelDepositAccount
			// 
			this.labelDepositAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDepositAccount.Location = new System.Drawing.Point(602, 218);
			this.labelDepositAccount.Name = "labelDepositAccount";
			this.labelDepositAccount.Size = new System.Drawing.Size(289, 16);
			this.labelDepositAccount.TabIndex = 0;
			this.labelDepositAccount.Text = "Deposit into Account";
			this.labelDepositAccount.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDepositAccount
			// 
			this.textDepositAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDepositAccount.Location = new System.Drawing.Point(602, 261);
			this.textDepositAccount.Name = "textDepositAccount";
			this.textDepositAccount.ReadOnly = true;
			this.textDepositAccount.Size = new System.Drawing.Size(289, 20);
			this.textDepositAccount.TabIndex = 0;
			this.textDepositAccount.TabStop = false;
			// 
			// textMemo
			// 
			this.textMemo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textMemo.Location = new System.Drawing.Point(601, 183);
			this.textMemo.Multiline = true;
			this.textMemo.Name = "textMemo";
			this.textMemo.Size = new System.Drawing.Size(289, 35);
			this.textMemo.TabIndex = 104;
			// 
			// labelMemo
			// 
			this.labelMemo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMemo.Location = new System.Drawing.Point(601, 166);
			this.labelMemo.Name = "labelMemo";
			this.labelMemo.Size = new System.Drawing.Size(127, 16);
			this.labelMemo.TabIndex = 0;
			this.labelMemo.Text = "Memo";
			this.labelMemo.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textAmountSearch
			// 
			this.textAmountSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textAmountSearch.Location = new System.Drawing.Point(469, 663);
			this.textAmountSearch.Name = "textAmountSearch";
			this.textAmountSearch.Size = new System.Drawing.Size(94, 20);
			this.textAmountSearch.TabIndex = 0;
			this.textAmountSearch.TabStop = false;
			this.textAmountSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textAmountSearch_KeyUp);
			this.textAmountSearch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textAmountSearch_MouseUp);
			// 
			// textCheckNumSearch
			// 
			this.textCheckNumSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textCheckNumSearch.Location = new System.Drawing.Point(247, 663);
			this.textCheckNumSearch.Name = "textCheckNumSearch";
			this.textCheckNumSearch.Size = new System.Drawing.Size(94, 20);
			this.textCheckNumSearch.TabIndex = 0;
			this.textCheckNumSearch.TabStop = false;
			this.textCheckNumSearch.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCheckNumSearch_KeyUp);
			this.textCheckNumSearch.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textCheckNumSearch_MouseUp);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(347, 663);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(121, 20);
			this.label7.TabIndex = 0;
			this.label7.Text = "Search Amount";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(98, 663);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(148, 20);
			this.label8.TabIndex = 0;
			this.label8.Text = "Search Check Number";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textItemNum
			// 
			this.textItemNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textItemNum.Location = new System.Drawing.Point(802, 25);
			this.textItemNum.Name = "textItemNum";
			this.textItemNum.ReadOnly = true;
			this.textItemNum.Size = new System.Drawing.Size(54, 20);
			this.textItemNum.TabIndex = 0;
			this.textItemNum.TabStop = false;
			this.textItemNum.Text = "0";
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(802, 8);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(66, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Item Count";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butSendQB
			// 
			this.butSendQB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSendQB.Location = new System.Drawing.Point(897, 261);
			this.butSendQB.Name = "butSendQB";
			this.butSendQB.Size = new System.Drawing.Size(75, 20);
			this.butSendQB.TabIndex = 0;
			this.butSendQB.TabStop = false;
			this.butSendQB.Text = "&Send QB";
			this.butSendQB.Click += new System.EventHandler(this.butSendQB_Click);
			// 
			// gridIns
			// 
			this.gridIns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridIns.Location = new System.Drawing.Point(8, 319);
			this.gridIns.Name = "gridIns";
			this.gridIns.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridIns.Size = new System.Drawing.Size(584, 306);
			this.gridIns.TabIndex = 0;
			this.gridIns.TabStop = false;
			this.gridIns.Title = "Insurance Payments";
			this.gridIns.TranslationName = "TableDepositSlipIns";
			this.gridIns.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridIns_CellClick);
			this.gridIns.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridIns_MouseUp);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(602, 661);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 0;
			this.butPrint.TabStop = false;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// textDate
			// 
			this.textDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDate.Location = new System.Drawing.Point(602, 25);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(94, 20);
			this.textDate.TabIndex = 101;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(7, 660);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(85, 24);
			this.butDelete.TabIndex = 0;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridPat
			// 
			this.gridPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPat.Location = new System.Drawing.Point(8, 12);
			this.gridPat.Name = "gridPat";
			this.gridPat.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridPat.Size = new System.Drawing.Size(584, 299);
			this.gridPat.TabIndex = 0;
			this.gridPat.TabStop = false;
			this.gridPat.Title = "Patient Payments";
			this.gridPat.TranslationName = "TableDepositSlipPat";
			this.gridPat.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellClick);
			this.gridPat.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gridPat_MouseUp);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(882, 663);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 0;
			this.butSave.TabStop = false;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butPDF
			// 
			this.butPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPDF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPDF.Location = new System.Drawing.Point(683, 661);
			this.butPDF.Name = "butPDF";
			this.butPDF.Size = new System.Drawing.Size(75, 24);
			this.butPDF.TabIndex = 0;
			this.butPDF.TabStop = false;
			this.butPDF.Text = "Create PDF";
			this.butPDF.Click += new System.EventHandler(this.butPDF_Click);
			// 
			// butEmailPDF
			// 
			this.butEmailPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEmailPDF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmailPDF.Location = new System.Drawing.Point(764, 661);
			this.butEmailPDF.Name = "butEmailPDF";
			this.butEmailPDF.Size = new System.Drawing.Size(75, 24);
			this.butEmailPDF.TabIndex = 0;
			this.butEmailPDF.TabStop = false;
			this.butEmailPDF.Text = "Email PDF";
			this.butEmailPDF.Click += new System.EventHandler(this.butEmailPDF_Click);
			// 
			// labelDepositAccountNum
			// 
			this.labelDepositAccountNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDepositAccountNum.Location = new System.Drawing.Point(602, 284);
			this.labelDepositAccountNum.Name = "labelDepositAccountNum";
			this.labelDepositAccountNum.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelDepositAccountNum.Size = new System.Drawing.Size(254, 14);
			this.labelDepositAccountNum.TabIndex = 0;
			this.labelDepositAccountNum.Text = "Auto Deposit Account";
			this.labelDepositAccountNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelDepositAccountNum.Visible = false;
			// 
			// comboDepositAccountNum
			// 
			this.comboDepositAccountNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboDepositAccountNum.Location = new System.Drawing.Point(602, 299);
			this.comboDepositAccountNum.Name = "comboDepositAccountNum";
			this.comboDepositAccountNum.Size = new System.Drawing.Size(289, 21);
			this.comboDepositAccountNum.TabIndex = 0;
			this.comboDepositAccountNum.TabStop = false;
			this.comboDepositAccountNum.Visible = false;
			// 
			// textBatch
			// 
			this.textBatch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBatch.Location = new System.Drawing.Point(601, 66);
			this.textBatch.MaxLength = 25;
			this.textBatch.Name = "textBatch";
			this.textBatch.Size = new System.Drawing.Size(290, 20);
			this.textBatch.TabIndex = 102;
			// 
			// labelBatchNum
			// 
			this.labelBatchNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelBatchNum.Location = new System.Drawing.Point(602, 49);
			this.labelBatchNum.Name = "labelBatchNum";
			this.labelBatchNum.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelBatchNum.Size = new System.Drawing.Size(237, 16);
			this.labelBatchNum.TabIndex = 0;
			this.labelBatchNum.Text = "Batch #";
			this.labelBatchNum.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// FormDepositEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 696);
			this.Controls.Add(this.textBatch);
			this.Controls.Add(this.labelBatchNum);
			this.Controls.Add(this.comboDepositAccountNum);
			this.Controls.Add(this.butEmailPDF);
			this.Controls.Add(this.butPDF);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textItemNum);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textCheckNumSearch);
			this.Controls.Add(this.textAmountSearch);
			this.Controls.Add(this.textMemo);
			this.Controls.Add(this.labelMemo);
			this.Controls.Add(this.butSendQB);
			this.Controls.Add(this.textDepositAccount);
			this.Controls.Add(this.labelDepositAccount);
			this.Controls.Add(this.comboDepositAccount);
			this.Controls.Add(this.gridIns);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textBankAccountInfo);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridPat);
			this.Controls.Add(this.groupSelect);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.labelDepositAccountNum);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDepositEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Deposit Slip";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormDepositEdit_Closing);
			this.Load += new System.EventHandler(this.FormDepositEdit_Load);
			this.groupSelect.ResumeLayout(false);
			this.groupSelect.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		private OpenDental.UI.Button butSave;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.Label labelClinic;
		private OpenDental.UI.ListBox listPayType;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBankAccountInfo;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textAmount;
		private OpenDental.UI.GroupBox groupSelect;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.GridOD gridPat;
		private OpenDental.UI.GridOD gridIns;
		private OpenDental.ValidDate textDateStart;
		private System.Windows.Forms.Label label5;
		private OpenDental.UI.Button butRefresh;
		private OpenDental.UI.ComboBox comboDepositAccount;
		private Label labelDepositAccount;
		private TextBox textDepositAccount;
		private UI.Button butSendQB;
		private TextBox textMemo;
		private Label labelMemo;
		private OpenDental.UI.ListBox listInsPayType;
		private Label label6;
		private TextBox textAmountSearch;
		private TextBox textCheckNumSearch;
		private Label label7;
		private Label label8;
		private TextBox textItemNum;
		private Label label9;
		private UI.Button butPDF;
		private OpenDental.UI.ComboBox comboClassRefs;
		private Label labelClassRef;
		private UI.Button butEmailPDF;
		private Label labelDepositAccountNum;
		private UI.ComboBox comboDepositAccountNum;
		private TextBox textBatch;
		private Label labelBatchNum;
	}
}
