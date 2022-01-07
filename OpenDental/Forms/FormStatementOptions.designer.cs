using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormStatementOptions {
		private System.ComponentModel.IContainer components;// Required designer variable.

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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStatementOptions));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkHidePayment = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label3 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.butToday = new OpenDental.UI.Button();
			this.butDatesAll = new OpenDental.UI.Button();
			this.but90days = new OpenDental.UI.Button();
			this.but45days = new OpenDental.UI.Button();
			this.labelEndDate = new System.Windows.Forms.Label();
			this.labelStartDate = new System.Windows.Forms.Label();
			this.textNoteBold = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listMode = new OpenDental.UI.ListBoxOD();
			this.checkIntermingled = new System.Windows.Forms.CheckBox();
			this.checkSinglePatient = new System.Windows.Forms.CheckBox();
			this.groupDateRange = new System.Windows.Forms.GroupBox();
			this.textDateEnd = new System.Windows.Forms.TextBox();
			this.checkBoxBillShowTransSinceZero = new System.Windows.Forms.CheckBox();
			this.textDateStart = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkIsSent = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butEmail = new OpenDental.UI.Button();
			this.butPreview = new OpenDental.UI.Button();
			this.textDate = new System.Windows.Forms.TextBox();
			this.checkIsReceipt = new System.Windows.Forms.CheckBox();
			this.groupInvoice = new System.Windows.Forms.GroupBox();
			this.textInvoiceNum = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkIsInvoiceCopy = new System.Windows.Forms.CheckBox();
			this.checkIsInvoice = new System.Windows.Forms.CheckBox();
			this.checkSuperStatement = new System.Windows.Forms.CheckBox();
			this.checkLimited = new System.Windows.Forms.CheckBox();
			this.butPatPortal = new OpenDental.UI.Button();
			this.checkShowLName = new System.Windows.Forms.CheckBox();
			this.checkSendSms = new System.Windows.Forms.CheckBox();
			this.checkExcludeTxfr = new System.Windows.Forms.CheckBox();
			this.groupDateRange.SuspendLayout();
			this.groupInvoice.SuspendLayout();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(616, 523);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 18;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(616, 493);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 17;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkHidePayment
			// 
			this.checkHidePayment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidePayment.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkHidePayment.Location = new System.Drawing.Point(1, 120);
			this.checkHidePayment.Name = "checkHidePayment";
			this.checkHidePayment.Size = new System.Drawing.Size(158, 17);
			this.checkHidePayment.TabIndex = 3;
			this.checkHidePayment.Text = "Hide payment options";
			this.checkHidePayment.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(14, 301);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(90, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Note";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(105, 300);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Statement;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(462, 130);
			this.textNote.TabIndex = 12;
			this.textNote.Text = "";
			// 
			// butToday
			// 
			this.butToday.Location = new System.Drawing.Point(156, 15);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(77, 24);
			this.butToday.TabIndex = 2;
			this.butToday.Text = "Today";
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// butDatesAll
			// 
			this.butDatesAll.Location = new System.Drawing.Point(234, 40);
			this.butDatesAll.Name = "butDatesAll";
			this.butDatesAll.Size = new System.Drawing.Size(77, 24);
			this.butDatesAll.TabIndex = 5;
			this.butDatesAll.Text = "All Dates";
			this.butDatesAll.Click += new System.EventHandler(this.butDatesAll_Click);
			// 
			// but90days
			// 
			this.but90days.Location = new System.Drawing.Point(234, 15);
			this.but90days.Name = "but90days";
			this.but90days.Size = new System.Drawing.Size(77, 24);
			this.but90days.TabIndex = 4;
			this.but90days.Text = "Last 90 Days";
			this.but90days.Click += new System.EventHandler(this.but90days_Click);
			// 
			// but45days
			// 
			this.but45days.Location = new System.Drawing.Point(156, 40);
			this.but45days.Name = "but45days";
			this.but45days.Size = new System.Drawing.Size(77, 24);
			this.but45days.TabIndex = 3;
			this.but45days.Text = "Last 45 Days";
			this.but45days.Click += new System.EventHandler(this.but45days_Click);
			// 
			// labelEndDate
			// 
			this.labelEndDate.Location = new System.Drawing.Point(5, 44);
			this.labelEndDate.Name = "labelEndDate";
			this.labelEndDate.Size = new System.Drawing.Size(69, 14);
			this.labelEndDate.TabIndex = 0;
			this.labelEndDate.Text = "End Date";
			this.labelEndDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelStartDate
			// 
			this.labelStartDate.Location = new System.Drawing.Point(5, 21);
			this.labelStartDate.Name = "labelStartDate";
			this.labelStartDate.Size = new System.Drawing.Size(69, 14);
			this.labelStartDate.TabIndex = 0;
			this.labelStartDate.Text = "Start Date";
			this.labelStartDate.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNoteBold
			// 
			this.textNoteBold.AcceptsTab = true;
			this.textNoteBold.BackColor = System.Drawing.SystemColors.Window;
			this.textNoteBold.DetectLinksEnabled = false;
			this.textNoteBold.DetectUrls = false;
			this.textNoteBold.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textNoteBold.ForeColor = System.Drawing.Color.DarkRed;
			this.textNoteBold.Location = new System.Drawing.Point(105, 436);
			this.textNoteBold.Name = "textNoteBold";
			this.textNoteBold.QuickPasteType = OpenDentBusiness.QuickPasteType.Statement;
			this.textNoteBold.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNoteBold.Size = new System.Drawing.Size(462, 74);
			this.textNoteBold.TabIndex = 13;
			this.textNoteBold.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 437);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(92, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Bold Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(64, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 14);
			this.label2.TabIndex = 0;
			this.label2.Text = "Mode";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listMode
			// 
			this.listMode.Location = new System.Drawing.Point(146, 60);
			this.listMode.Name = "listMode";
			this.listMode.Size = new System.Drawing.Size(113, 56);
			this.listMode.TabIndex = 2;
			this.listMode.Click += new System.EventHandler(this.listMode_Click);
			// 
			// checkIntermingled
			// 
			this.checkIntermingled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIntermingled.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIntermingled.Location = new System.Drawing.Point(1, 154);
			this.checkIntermingled.Name = "checkIntermingled";
			this.checkIntermingled.Size = new System.Drawing.Size(158, 17);
			this.checkIntermingled.TabIndex = 5;
			this.checkIntermingled.Text = "Intermingle family members";
			this.checkIntermingled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIntermingled.Click += new System.EventHandler(this.checkIntermingled_Click);
			// 
			// checkSinglePatient
			// 
			this.checkSinglePatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSinglePatient.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSinglePatient.Location = new System.Drawing.Point(1, 137);
			this.checkSinglePatient.Name = "checkSinglePatient";
			this.checkSinglePatient.Size = new System.Drawing.Size(158, 17);
			this.checkSinglePatient.TabIndex = 4;
			this.checkSinglePatient.Text = "Single patient only";
			this.checkSinglePatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSinglePatient.Click += new System.EventHandler(this.checkSinglePatient_Click);
			// 
			// groupDateRange
			// 
			this.groupDateRange.Controls.Add(this.textDateEnd);
			this.groupDateRange.Controls.Add(this.labelStartDate);
			this.groupDateRange.Controls.Add(this.checkBoxBillShowTransSinceZero);
			this.groupDateRange.Controls.Add(this.textDateStart);
			this.groupDateRange.Controls.Add(this.labelEndDate);
			this.groupDateRange.Controls.Add(this.but45days);
			this.groupDateRange.Controls.Add(this.but90days);
			this.groupDateRange.Controls.Add(this.butDatesAll);
			this.groupDateRange.Controls.Add(this.butToday);
			this.groupDateRange.Location = new System.Drawing.Point(326, 12);
			this.groupDateRange.Name = "groupDateRange";
			this.groupDateRange.Size = new System.Drawing.Size(339, 90);
			this.groupDateRange.TabIndex = 11;
			this.groupDateRange.TabStop = false;
			this.groupDateRange.Text = "Date Range";
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(77, 41);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(77, 20);
			this.textDateEnd.TabIndex = 1;
			this.textDateEnd.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDateEnd_KeyDown);
			this.textDateEnd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textDateEnd_KeyPress);
			this.textDateEnd.Validating += new System.ComponentModel.CancelEventHandler(this.textDateEnd_Validating);
			// 
			// checkBoxBillShowTransSinceZero
			// 
			this.checkBoxBillShowTransSinceZero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxBillShowTransSinceZero.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBoxBillShowTransSinceZero.Location = new System.Drawing.Point(20, 67);
			this.checkBoxBillShowTransSinceZero.Name = "checkBoxBillShowTransSinceZero";
			this.checkBoxBillShowTransSinceZero.Size = new System.Drawing.Size(314, 18);
			this.checkBoxBillShowTransSinceZero.TabIndex = 253;
			this.checkBoxBillShowTransSinceZero.Text = "Show all transactions since zero or negative balance";
			this.checkBoxBillShowTransSinceZero.CheckedChanged += new System.EventHandler(this.checkBoxBillShowTransSinceZero_CheckedChanged);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(77, 18);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(77, 20);
			this.textDateStart.TabIndex = 0;
			this.textDateStart.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDateStart_KeyDown);
			this.textDateStart.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textDateStart_KeyPress);
			this.textDateStart.Validating += new System.ComponentModel.CancelEventHandler(this.textDateStart_Validating);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(67, 17);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(77, 14);
			this.label4.TabIndex = 0;
			this.label4.Text = "Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsSent
			// 
			this.checkIsSent.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsSent.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsSent.Location = new System.Drawing.Point(1, 39);
			this.checkIsSent.Name = "checkIsSent";
			this.checkIsSent.Size = new System.Drawing.Size(159, 17);
			this.checkIsSent.TabIndex = 1;
			this.checkIsSent.Text = "Sent";
			this.checkIsSent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsSent.Click += new System.EventHandler(this.checkIsSent_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(18, 523);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(79, 24);
			this.butDelete.TabIndex = 19;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butPrint
			// 
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(182, 523);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(79, 24);
			this.butPrint.TabIndex = 14;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butEmail
			// 
			this.butEmail.Icon = OpenDental.UI.EnumIcons.Email;
			this.butEmail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmail.Location = new System.Drawing.Point(267, 523);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(79, 24);
			this.butEmail.TabIndex = 15;
			this.butEmail.Text = "E-mail";
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butPreview
			// 
			this.butPreview.Image = global::OpenDental.Properties.Resources.printPreview20;
			this.butPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPreview.Location = new System.Drawing.Point(435, 523);
			this.butPreview.Name = "butPreview";
			this.butPreview.Size = new System.Drawing.Size(79, 24);
			this.butPreview.TabIndex = 16;
			this.butPreview.Text = "View";
			this.butPreview.Click += new System.EventHandler(this.butPreview_Click);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(146, 15);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(77, 20);
			this.textDate.TabIndex = 0;
			this.textDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textDate_KeyDown);
			this.textDate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textDate_KeyPress);
			this.textDate.Validating += new System.ComponentModel.CancelEventHandler(this.textDate_Validating);
			// 
			// checkIsReceipt
			// 
			this.checkIsReceipt.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsReceipt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsReceipt.Location = new System.Drawing.Point(1, 171);
			this.checkIsReceipt.Name = "checkIsReceipt";
			this.checkIsReceipt.Size = new System.Drawing.Size(158, 17);
			this.checkIsReceipt.TabIndex = 6;
			this.checkIsReceipt.Text = "Receipt";
			this.checkIsReceipt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupInvoice
			// 
			this.groupInvoice.Controls.Add(this.textInvoiceNum);
			this.groupInvoice.Controls.Add(this.label5);
			this.groupInvoice.Controls.Add(this.checkIsInvoiceCopy);
			this.groupInvoice.Controls.Add(this.checkIsInvoice);
			this.groupInvoice.Location = new System.Drawing.Point(12, 212);
			this.groupInvoice.Name = "groupInvoice";
			this.groupInvoice.Size = new System.Drawing.Size(247, 82);
			this.groupInvoice.TabIndex = 9;
			this.groupInvoice.TabStop = false;
			this.groupInvoice.Text = "Invoice";
			// 
			// textInvoiceNum
			// 
			this.textInvoiceNum.Location = new System.Drawing.Point(134, 55);
			this.textInvoiceNum.Name = "textInvoiceNum";
			this.textInvoiceNum.ReadOnly = true;
			this.textInvoiceNum.Size = new System.Drawing.Size(108, 20);
			this.textInvoiceNum.TabIndex = 2;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(13, 57);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(120, 14);
			this.label5.TabIndex = 0;
			this.label5.Text = "Invoice Number";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsInvoiceCopy
			// 
			this.checkIsInvoiceCopy.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsInvoiceCopy.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsInvoiceCopy.Location = new System.Drawing.Point(5, 34);
			this.checkIsInvoiceCopy.Name = "checkIsInvoiceCopy";
			this.checkIsInvoiceCopy.Size = new System.Drawing.Size(142, 17);
			this.checkIsInvoiceCopy.TabIndex = 1;
			this.checkIsInvoiceCopy.Text = "Invoice Copy";
			this.checkIsInvoiceCopy.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsInvoice
			// 
			this.checkIsInvoice.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsInvoice.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsInvoice.Location = new System.Drawing.Point(5, 15);
			this.checkIsInvoice.Name = "checkIsInvoice";
			this.checkIsInvoice.Size = new System.Drawing.Size(142, 17);
			this.checkIsInvoice.TabIndex = 0;
			this.checkIsInvoice.Text = "Invoice";
			this.checkIsInvoice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsInvoice.Click += new System.EventHandler(this.checkIsInvoice_Click);
			// 
			// checkSuperStatement
			// 
			this.checkSuperStatement.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperStatement.Enabled = false;
			this.checkSuperStatement.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSuperStatement.Location = new System.Drawing.Point(253, 137);
			this.checkSuperStatement.Name = "checkSuperStatement";
			this.checkSuperStatement.Size = new System.Drawing.Size(158, 17);
			this.checkSuperStatement.TabIndex = 7;
			this.checkSuperStatement.Text = "Send to super family";
			this.checkSuperStatement.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSuperStatement.CheckedChanged += new System.EventHandler(this.checkSuperStatement_CheckedChanged);
			// 
			// checkLimited
			// 
			this.checkLimited.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLimited.Enabled = false;
			this.checkLimited.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkLimited.Location = new System.Drawing.Point(253, 119);
			this.checkLimited.Name = "checkLimited";
			this.checkLimited.Size = new System.Drawing.Size(158, 17);
			this.checkLimited.TabIndex = 8;
			this.checkLimited.Text = "Limited statement";
			this.checkLimited.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLimited.Visible = false;
			// 
			// butPatPortal
			// 
			this.butPatPortal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPatPortal.Location = new System.Drawing.Point(351, 523);
			this.butPatPortal.Name = "butPatPortal";
			this.butPatPortal.Size = new System.Drawing.Size(79, 24);
			this.butPatPortal.TabIndex = 20;
			this.butPatPortal.Text = "Pat Portal";
			this.butPatPortal.Click += new System.EventHandler(this.butPatPortal_Click);
			// 
			// checkShowLName
			// 
			this.checkShowLName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowLName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowLName.Location = new System.Drawing.Point(253, 154);
			this.checkShowLName.Name = "checkShowLName";
			this.checkShowLName.Size = new System.Drawing.Size(158, 17);
			this.checkShowLName.TabIndex = 254;
			this.checkShowLName.Text = "Include patient last name";
			this.checkShowLName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkSendSms
			// 
			this.checkSendSms.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkSendSms.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkSendSms.Location = new System.Drawing.Point(1, 188);
			this.checkSendSms.Name = "checkSendSms";
			this.checkSendSms.Size = new System.Drawing.Size(158, 17);
			this.checkSendSms.TabIndex = 255;
			this.checkSendSms.Text = "Send text message";
			this.checkSendSms.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkExcludeTxfr
			// 
			this.checkExcludeTxfr.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeTxfr.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeTxfr.Location = new System.Drawing.Point(253, 171);
			this.checkExcludeTxfr.Name = "checkExcludeTxfr";
			this.checkExcludeTxfr.Size = new System.Drawing.Size(158, 17);
			this.checkExcludeTxfr.TabIndex = 256;
			this.checkExcludeTxfr.Text = "Exclude income transfers";
			this.checkExcludeTxfr.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeTxfr.Visible = false;
			// 
			// FormStatementOptions
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(709, 584);
			this.Controls.Add(this.checkExcludeTxfr);
			this.Controls.Add(this.checkSendSms);
			this.Controls.Add(this.checkShowLName);
			this.Controls.Add(this.butPatPortal);
			this.Controls.Add(this.checkLimited);
			this.Controls.Add(this.groupInvoice);
			this.Controls.Add(this.checkSuperStatement);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butEmail);
			this.Controls.Add(this.butPreview);
			this.Controls.Add(this.checkIsReceipt);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.checkIsSent);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupDateRange);
			this.Controls.Add(this.checkSinglePatient);
			this.Controls.Add(this.checkIntermingled);
			this.Controls.Add(this.listMode);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textNoteBold);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkHidePayment);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormStatementOptions";
			this.ShowInTaskbar = false;
			this.Text = "Statement";
			this.Load += new System.EventHandler(this.FormStatementOptions_Load);
			this.groupDateRange.ResumeLayout(false);
			this.groupDateRange.PerformLayout();
			this.groupInvoice.ResumeLayout(false);
			this.groupInvoice.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.CheckBox checkHidePayment;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Label label3;
		private OpenDental.ODtextBox textNote;
		private OpenDental.UI.Button butToday;
		private OpenDental.UI.Button butDatesAll;
		private OpenDental.UI.Button but90days;
		private OpenDental.UI.Button but45days;
		private Label labelEndDate;
		private Label labelStartDate;
		private ODtextBox textNoteBold;
		private Label label1;
		private Label label2;
		private OpenDental.UI.ListBoxOD listMode;
		private CheckBox checkSinglePatient;
		private CheckBox checkIntermingled;
		private GroupBox groupDateRange;
		private Label label4;
		private CheckBox checkIsSent;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butEmail;
		private OpenDental.UI.Button butPreview;
		private TextBox textDateEnd;
		private TextBox textDateStart;
		private TextBox textDate;		
		private CheckBox checkIsReceipt;
		private GroupBox groupInvoice;
		private CheckBox checkIsInvoiceCopy;
		private CheckBox checkIsInvoice;
		private TextBox textInvoiceNum;
		private Label label5;
		private CheckBox checkSuperStatement;
		private CheckBox checkLimited;
		private UI.Button butPatPortal;
		private CheckBox checkBoxBillShowTransSinceZero;
		private CheckBox checkShowLName;
		private CheckBox checkSendSms;
		private CheckBox checkExcludeTxfr;
	}
}
