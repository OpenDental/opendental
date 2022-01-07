using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRxEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRxEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textSig = new System.Windows.Forms.TextBox();
			this.textDisp = new System.Windows.Forms.TextBox();
			this.textRefills = new System.Windows.Forms.TextBox();
			this.textDrug = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textDate = new OpenDental.ValidDate();
			this.butPrint = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.butPick = new OpenDental.UI.Button();
			this.textPharmacy = new System.Windows.Forms.TextBox();
			this.checkControlled = new System.Windows.Forms.CheckBox();
			this.butView = new OpenDental.UI.Button();
			this.labelView = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.comboSendStatus = new System.Windows.Forms.ComboBox();
			this.textDosageCode = new System.Windows.Forms.TextBox();
			this.labelDosageCode = new System.Windows.Forms.Label();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.butPickProv = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.textNotes = new OpenDental.ODtextBox();
			this.labelCPOE = new System.Windows.Forms.Label();
			this.butAudit = new OpenDental.UI.Button();
			this.label10 = new System.Windows.Forms.Label();
			this.textPharmInfo = new OpenDental.ODtextBox();
			this.checkProcRequired = new System.Windows.Forms.CheckBox();
			this.comboProcCode = new System.Windows.Forms.ComboBox();
			this.labelProcedure = new System.Windows.Forms.Label();
			this.labelDaysOfSupply = new System.Windows.Forms.Label();
			this.textDaysOfSupply = new OpenDental.ValidDouble();
			this.textPatInstructions = new OpenDental.ODtextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butPrintPatInstructions = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(550, 586);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 18;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(550, 546);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 17;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// textSig
			// 
			this.textSig.AcceptsReturn = true;
			this.textSig.Location = new System.Drawing.Point(138, 158);
			this.textSig.Multiline = true;
			this.textSig.Name = "textSig";
			this.textSig.Size = new System.Drawing.Size(254, 44);
			this.textSig.TabIndex = 6;
			// 
			// textDisp
			// 
			this.textDisp.Location = new System.Drawing.Point(138, 203);
			this.textDisp.Name = "textDisp";
			this.textDisp.Size = new System.Drawing.Size(114, 20);
			this.textDisp.TabIndex = 7;
			// 
			// textRefills
			// 
			this.textRefills.Location = new System.Drawing.Point(138, 224);
			this.textRefills.Name = "textRefills";
			this.textRefills.Size = new System.Drawing.Size(114, 20);
			this.textRefills.TabIndex = 8;
			// 
			// textDrug
			// 
			this.textDrug.Location = new System.Drawing.Point(138, 137);
			this.textDrug.Name = "textDrug";
			this.textDrug.Size = new System.Drawing.Size(254, 20);
			this.textDrug.TabIndex = 5;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(49, 162);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(89, 14);
			this.label6.TabIndex = 0;
			this.label6.Text = "Sig";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(39, 207);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(99, 14);
			this.label5.TabIndex = 0;
			this.label5.Text = "Disp";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(39, 228);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(99, 14);
			this.label4.TabIndex = 0;
			this.label4.Text = "Refills";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(31, 289);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(107, 36);
			this.label3.TabIndex = 0;
			this.label3.Text = "Notes (will not show on printout)";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(45, 139);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Drug";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(34, 37);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(105, 14);
			this.label2.TabIndex = 0;
			this.label2.Text = "Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(138, 33);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(100, 20);
			this.textDate.TabIndex = 0;
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(323, 586);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(81, 24);
			this.butPrint.TabIndex = 20;
			this.butPrint.Text = "&Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 586);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(88, 24);
			this.butDelete.TabIndex = 23;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(39, 510);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(99, 14);
			this.label8.TabIndex = 0;
			this.label8.Text = "Pharmacy";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butPick
			// 
			this.butPick.Location = new System.Drawing.Point(338, 505);
			this.butPick.Name = "butPick";
			this.butPick.Size = new System.Drawing.Size(58, 23);
			this.butPick.TabIndex = 15;
			this.butPick.Text = "Pick";
			this.butPick.Click += new System.EventHandler(this.butPick_Click);
			// 
			// textPharmacy
			// 
			this.textPharmacy.AcceptsReturn = true;
			this.textPharmacy.Location = new System.Drawing.Point(138, 507);
			this.textPharmacy.Name = "textPharmacy";
			this.textPharmacy.ReadOnly = true;
			this.textPharmacy.Size = new System.Drawing.Size(198, 20);
			this.textPharmacy.TabIndex = 0;
			this.textPharmacy.TabStop = false;
			// 
			// checkControlled
			// 
			this.checkControlled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkControlled.Location = new System.Drawing.Point(7, 54);
			this.checkControlled.Name = "checkControlled";
			this.checkControlled.Size = new System.Drawing.Size(145, 20);
			this.checkControlled.TabIndex = 1;
			this.checkControlled.Text = "Controlled Substance";
			this.checkControlled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkControlled.UseVisualStyleBackColor = true;
			// 
			// butView
			// 
			this.butView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butView.Image = global::OpenDental.Properties.Resources.printPreview20;
			this.butView.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butView.Location = new System.Drawing.Point(410, 586);
			this.butView.Name = "butView";
			this.butView.Size = new System.Drawing.Size(81, 24);
			this.butView.TabIndex = 19;
			this.butView.Text = "&View";
			this.butView.Click += new System.EventHandler(this.butView_Click);
			// 
			// labelView
			// 
			this.labelView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelView.Location = new System.Drawing.Point(335, 613);
			this.labelView.Name = "labelView";
			this.labelView.Size = new System.Drawing.Size(199, 14);
			this.labelView.TabIndex = 0;
			this.labelView.Text = "This Rx has already been printed.";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(39, 533);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(99, 14);
			this.label9.TabIndex = 0;
			this.label9.Text = "Send Status";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboSendStatus
			// 
			this.comboSendStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSendStatus.FormattingEnabled = true;
			this.comboSendStatus.Location = new System.Drawing.Point(138, 530);
			this.comboSendStatus.Name = "comboSendStatus";
			this.comboSendStatus.Size = new System.Drawing.Size(198, 21);
			this.comboSendStatus.TabIndex = 16;
			// 
			// textDosageCode
			// 
			this.textDosageCode.Location = new System.Drawing.Point(138, 267);
			this.textDosageCode.Name = "textDosageCode";
			this.textDosageCode.Size = new System.Drawing.Size(114, 20);
			this.textDosageCode.TabIndex = 11;
			// 
			// labelDosageCode
			// 
			this.labelDosageCode.Location = new System.Drawing.Point(44, 271);
			this.labelDosageCode.Name = "labelDosageCode";
			this.labelDosageCode.Size = new System.Drawing.Size(94, 14);
			this.labelDosageCode.TabIndex = 0;
			this.labelDosageCode.Text = "Dosage Code";
			this.labelDosageCode.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(138, 245);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(254, 21);
			this.comboProv.TabIndex = 9;
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(394, 245);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(18, 21);
			this.butPickProv.TabIndex = 10;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProv_Click);
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(37, 249);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 14);
			this.label7.TabIndex = 0;
			this.label7.Text = "Provider";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.Location = new System.Drawing.Point(138, 288);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Rx;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(396, 77);
			this.textNotes.TabIndex = 12;
			this.textNotes.Text = "";
			// 
			// labelCPOE
			// 
			this.labelCPOE.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCPOE.Location = new System.Drawing.Point(61, 9);
			this.labelCPOE.Name = "labelCPOE";
			this.labelCPOE.Size = new System.Drawing.Size(249, 14);
			this.labelCPOE.TabIndex = 0;
			this.labelCPOE.Text = "Computerized Provider Order Entry";
			this.labelCPOE.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.labelCPOE.Visible = false;
			// 
			// butAudit
			// 
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAudit.Location = new System.Drawing.Point(138, 586);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(92, 24);
			this.butAudit.TabIndex = 22;
			this.butAudit.Text = "&Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(12, 456);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(125, 14);
			this.label10.TabIndex = 0;
			this.label10.Text = "Erx Pharmacy Info";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPharmInfo
			// 
			this.textPharmInfo.AcceptsTab = true;
			this.textPharmInfo.BackColor = System.Drawing.SystemColors.Control;
			this.textPharmInfo.DetectLinksEnabled = false;
			this.textPharmInfo.DetectUrls = false;
			this.textPharmInfo.Location = new System.Drawing.Point(138, 451);
			this.textPharmInfo.Name = "textPharmInfo";
			this.textPharmInfo.QuickPasteType = OpenDentBusiness.QuickPasteType.Rx;
			this.textPharmInfo.ReadOnly = true;
			this.textPharmInfo.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPharmInfo.Size = new System.Drawing.Size(396, 50);
			this.textPharmInfo.TabIndex = 14;
			this.textPharmInfo.TabStop = false;
			this.textPharmInfo.Text = "";
			// 
			// checkProcRequired
			// 
			this.checkProcRequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcRequired.Location = new System.Drawing.Point(7, 74);
			this.checkProcRequired.Name = "checkProcRequired";
			this.checkProcRequired.Size = new System.Drawing.Size(145, 20);
			this.checkProcRequired.TabIndex = 2;
			this.checkProcRequired.Text = "Is Proc Required";
			this.checkProcRequired.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcRequired.UseVisualStyleBackColor = true;
			// 
			// comboProcCode
			// 
			this.comboProcCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProcCode.FormattingEnabled = true;
			this.comboProcCode.Location = new System.Drawing.Point(138, 94);
			this.comboProcCode.Name = "comboProcCode";
			this.comboProcCode.Size = new System.Drawing.Size(254, 21);
			this.comboProcCode.TabIndex = 3;
			// 
			// labelProcedure
			// 
			this.labelProcedure.Location = new System.Drawing.Point(44, 94);
			this.labelProcedure.Name = "labelProcedure";
			this.labelProcedure.Size = new System.Drawing.Size(93, 21);
			this.labelProcedure.TabIndex = 0;
			this.labelProcedure.Text = "Procedure";
			this.labelProcedure.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDaysOfSupply
			// 
			this.labelDaysOfSupply.Location = new System.Drawing.Point(14, 116);
			this.labelDaysOfSupply.Name = "labelDaysOfSupply";
			this.labelDaysOfSupply.Size = new System.Drawing.Size(123, 20);
			this.labelDaysOfSupply.TabIndex = 21;
			this.labelDaysOfSupply.Text = "Days of Supply";
			this.labelDaysOfSupply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDaysOfSupply
			// 
			this.textDaysOfSupply.Location = new System.Drawing.Point(138, 116);
			this.textDaysOfSupply.MaxVal = 99999999D;
			this.textDaysOfSupply.MinVal = 0D;
			this.textDaysOfSupply.Name = "textDaysOfSupply";
			this.textDaysOfSupply.Size = new System.Drawing.Size(114, 20);
			this.textDaysOfSupply.TabIndex = 4;
			// 
			// textPatInstructions
			// 
			this.textPatInstructions.AcceptsTab = true;
			this.textPatInstructions.BackColor = System.Drawing.SystemColors.Window;
			this.textPatInstructions.DetectLinksEnabled = false;
			this.textPatInstructions.DetectUrls = false;
			this.textPatInstructions.Location = new System.Drawing.Point(138, 367);
			this.textPatInstructions.Name = "textPatInstructions";
			this.textPatInstructions.QuickPasteType = OpenDentBusiness.QuickPasteType.Rx;
			this.textPatInstructions.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPatInstructions.Size = new System.Drawing.Size(396, 82);
			this.textPatInstructions.TabIndex = 13;
			this.textPatInstructions.Text = "";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(12, 375);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(126, 14);
			this.label11.TabIndex = 24;
			this.label11.Text = "Patient Instructions";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butPrintPatInstructions
			// 
			this.butPrintPatInstructions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrintPatInstructions.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrintPatInstructions.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrintPatInstructions.Location = new System.Drawing.Point(233, 586);
			this.butPrintPatInstructions.Name = "butPrintPatInstructions";
			this.butPrintPatInstructions.Size = new System.Drawing.Size(84, 24);
			this.butPrintPatInstructions.TabIndex = 21;
			this.butPrintPatInstructions.Text = "Pat &Instr.";
			this.butPrintPatInstructions.Click += new System.EventHandler(this.butPrintPatInstructions_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.ForceShowUnassigned = true;
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(101, 554);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(235, 21);
			this.comboClinic.TabIndex = 25;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// FormRxEdit
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(637, 630);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.butPrintPatInstructions);
			this.Controls.Add(this.textPatInstructions);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textDaysOfSupply);
			this.Controls.Add(this.labelDaysOfSupply);
			this.Controls.Add(this.labelProcedure);
			this.Controls.Add(this.comboProcCode);
			this.Controls.Add(this.checkProcRequired);
			this.Controls.Add(this.textPharmInfo);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.labelCPOE);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.butPickProv);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textDosageCode);
			this.Controls.Add(this.labelDosageCode);
			this.Controls.Add(this.comboSendStatus);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.labelView);
			this.Controls.Add(this.butView);
			this.Controls.Add(this.checkControlled);
			this.Controls.Add(this.butPick);
			this.Controls.Add(this.textPharmacy);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.textSig);
			this.Controls.Add(this.textDisp);
			this.Controls.Add(this.textRefills);
			this.Controls.Add(this.textDrug);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRxEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Rx";
			this.Load += new System.EventHandler(this.FormRxEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.TextBox textSig;
		private System.Windows.Forms.TextBox textDisp;
		private System.Windows.Forms.TextBox textRefills;
		private System.Windows.Forms.TextBox textDrug;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private OpenDental.ValidDate textDate;
		private OpenDental.UI.Button butPrint;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ODtextBox textNotes;
		private Label label8;
		private OpenDental.UI.Button butPick;
		private TextBox textPharmacy;
		private CheckBox checkControlled;
		private OpenDental.UI.Button butView;
		private Label labelView;
		private Label label9;
		private ComboBox comboSendStatus;
		private TextBox textDosageCode;
		private Label labelDosageCode;
		private UI.ComboBoxOD comboProv;
		private UI.Button butPickProv;
		private Label label7;
		private Label labelCPOE;
		private UI.Button butAudit;
		private Label label10;
		private ODtextBox textPharmInfo;
		private CheckBox checkProcRequired;
		private ComboBox comboProcCode;
		private Label labelProcedure;
		private Label labelDaysOfSupply;
		private ValidDouble textDaysOfSupply;
		private ODtextBox textPatInstructions;
		private Label label11;
		private UI.Button butPrintPatInstructions;
		private UI.ComboBoxClinicPicker comboClinic;
	}
}
