using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcGroup {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcGroup));
			this.label7 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label26 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textProcDate = new OpenDental.ValidDate();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.gridProc = new OpenDental.UI.GridOD();
			this.textDateEntry = new OpenDental.ValidDate();
			this.buttonUseAutoNote = new OpenDental.UI.Button();
			this.textNotes = new OpenDental.ODtextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.gridPat = new OpenDental.UI.GridOD();
			this.butRx = new OpenDental.UI.Button();
			this.butExamSheets = new OpenDental.UI.Button();
			this.labelRepair = new System.Windows.Forms.Label();
			this.butRepairY = new System.Windows.Forms.Button();
			this.butRepairN = new System.Windows.Forms.Button();
			this.butOnCallY = new System.Windows.Forms.Button();
			this.butOnCallN = new System.Windows.Forms.Button();
			this.butEffectiveCommN = new System.Windows.Forms.Button();
			this.labelOnCall = new System.Windows.Forms.Label();
			this.labelEffectiveComm = new System.Windows.Forms.Label();
			this.butEffectiveCommY = new System.Windows.Forms.Button();
			this.gridPlanned = new OpenDental.UI.GridOD();
			this.butNew = new OpenDental.UI.Button();
			this.butClear = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.panelPlanned = new System.Windows.Forms.Panel();
			this.labelDPCpost = new System.Windows.Forms.Label();
			this.comboDPCpost = new System.Windows.Forms.ComboBox();
			this.butLock = new OpenDental.UI.Button();
			this.butInvalidate = new OpenDental.UI.Button();
			this.butAppend = new OpenDental.UI.Button();
			this.labelLocked = new System.Windows.Forms.Label();
			this.labelInvalid = new System.Windows.Forms.Label();
			this.butChangeUser = new OpenDental.UI.Button();
			this.labelPermAlert = new System.Windows.Forms.Label();
			this.butEditAutoNote = new OpenDental.UI.Button();
			this.panelPlanned.SuspendLayout();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(23, 78);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(73, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "&Notes";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(5, 264);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(91, 41);
			this.label15.TabIndex = 79;
			this.label15.Text = "Signature /\r\nInitials";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(23, 55);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(73, 16);
			this.label16.TabIndex = 80;
			this.label16.Text = "User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Location = new System.Drawing.Point(98, 52);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(119, 20);
			this.textUser.TabIndex = 101;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(-25, 34);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(125, 14);
			this.label12.TabIndex = 96;
			this.label12.Text = "Date Entry";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label26
			// 
			this.label26.Location = new System.Drawing.Point(177, 32);
			this.label26.Name = "label26";
			this.label26.Size = new System.Drawing.Size(83, 18);
			this.label26.TabIndex = 97;
			this.label26.Text = "(for security)";
			this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 14);
			this.label2.TabIndex = 101;
			this.label2.Text = "Procedure Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProcDate
			// 
			this.textProcDate.Location = new System.Drawing.Point(98, 12);
			this.textProcDate.Name = "textProcDate";
			this.textProcDate.ReadOnly = true;
			this.textProcDate.Size = new System.Drawing.Size(76, 20);
			this.textProcDate.TabIndex = 100;
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(98, 264);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(394, 81);
			this.signatureBoxWrapper.TabIndex = 194;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.SignatureChanged += new System.EventHandler(this.signatureBoxWrapper_SignatureChanged);
			// 
			// gridProc
			// 
			this.gridProc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProc.HScrollVisible = true;
			this.gridProc.Location = new System.Drawing.Point(10, 375);
			this.gridProc.Name = "gridProc";
			this.gridProc.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridProc.Size = new System.Drawing.Size(883, 222);
			this.gridProc.TabIndex = 193;
			this.gridProc.Title = "Procedures";
			this.gridProc.TranslationName = "TableProg";
			// 
			// textDateEntry
			// 
			this.textDateEntry.Location = new System.Drawing.Point(98, 32);
			this.textDateEntry.Name = "textDateEntry";
			this.textDateEntry.ReadOnly = true;
			this.textDateEntry.Size = new System.Drawing.Size(76, 20);
			this.textDateEntry.TabIndex = 95;
			// 
			// buttonUseAutoNote
			// 
			this.buttonUseAutoNote.Location = new System.Drawing.Point(329, 50);
			this.buttonUseAutoNote.Name = "buttonUseAutoNote";
			this.buttonUseAutoNote.Size = new System.Drawing.Size(82, 22);
			this.buttonUseAutoNote.TabIndex = 106;
			this.buttonUseAutoNote.Text = "Auto Note";
			this.buttonUseAutoNote.Click += new System.EventHandler(this.buttonUseAutoNote_Click);
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.HasAutoNotes = true;
			this.textNotes.Location = new System.Drawing.Point(98, 72);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(397, 188);
			this.textNotes.TabIndex = 1;
			this.textNotes.Text = "";
			this.textNotes.TextChanged += new System.EventHandler(this.textNotes_TextChanged);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(19, 606);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(817, 609);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 24);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(735, 609);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 24);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridPat
			// 
			this.gridPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPat.Location = new System.Drawing.Point(501, 276);
			this.gridPat.Name = "gridPat";
			this.gridPat.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridPat.Size = new System.Drawing.Size(392, 81);
			this.gridPat.TabIndex = 195;
			this.gridPat.Title = "Patient Fields";
			this.gridPat.TranslationName = "TablePatient";
			this.gridPat.Visible = false;
			this.gridPat.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPat_CellDoubleClick);
			// 
			// butRx
			// 
			this.butRx.Location = new System.Drawing.Point(792, 41);
			this.butRx.Name = "butRx";
			this.butRx.Size = new System.Drawing.Size(75, 24);
			this.butRx.TabIndex = 106;
			this.butRx.Text = "Rx";
			this.butRx.Visible = false;
			this.butRx.Click += new System.EventHandler(this.butRx_Click);
			// 
			// butExamSheets
			// 
			this.butExamSheets.Location = new System.Drawing.Point(792, 14);
			this.butExamSheets.Name = "butExamSheets";
			this.butExamSheets.Size = new System.Drawing.Size(76, 24);
			this.butExamSheets.TabIndex = 106;
			this.butExamSheets.Text = "Exam Sheets";
			this.butExamSheets.Visible = false;
			this.butExamSheets.Click += new System.EventHandler(this.butExamSheets_Click);
			// 
			// labelRepair
			// 
			this.labelRepair.Location = new System.Drawing.Point(498, 85);
			this.labelRepair.Name = "labelRepair";
			this.labelRepair.Size = new System.Drawing.Size(90, 16);
			this.labelRepair.TabIndex = 196;
			this.labelRepair.Text = "Repair";
			this.labelRepair.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelRepair.Visible = false;
			// 
			// butRepairY
			// 
			this.butRepairY.Location = new System.Drawing.Point(590, 83);
			this.butRepairY.Name = "butRepairY";
			this.butRepairY.Size = new System.Drawing.Size(23, 20);
			this.butRepairY.TabIndex = 198;
			this.butRepairY.Text = "Y";
			this.butRepairY.UseVisualStyleBackColor = true;
			this.butRepairY.Visible = false;
			this.butRepairY.Click += new System.EventHandler(this.butRepairY_Click);
			// 
			// butRepairN
			// 
			this.butRepairN.Location = new System.Drawing.Point(614, 83);
			this.butRepairN.Name = "butRepairN";
			this.butRepairN.Size = new System.Drawing.Size(23, 20);
			this.butRepairN.TabIndex = 198;
			this.butRepairN.Text = "N";
			this.butRepairN.UseVisualStyleBackColor = true;
			this.butRepairN.Visible = false;
			this.butRepairN.Click += new System.EventHandler(this.butRepairN_Click);
			// 
			// butOnCallY
			// 
			this.butOnCallY.Location = new System.Drawing.Point(590, 41);
			this.butOnCallY.Name = "butOnCallY";
			this.butOnCallY.Size = new System.Drawing.Size(23, 20);
			this.butOnCallY.TabIndex = 198;
			this.butOnCallY.Text = "Y";
			this.butOnCallY.UseVisualStyleBackColor = true;
			this.butOnCallY.Visible = false;
			this.butOnCallY.Click += new System.EventHandler(this.butOnCallY_Click);
			// 
			// butOnCallN
			// 
			this.butOnCallN.Location = new System.Drawing.Point(614, 41);
			this.butOnCallN.Name = "butOnCallN";
			this.butOnCallN.Size = new System.Drawing.Size(23, 20);
			this.butOnCallN.TabIndex = 198;
			this.butOnCallN.Text = "N";
			this.butOnCallN.UseVisualStyleBackColor = true;
			this.butOnCallN.Visible = false;
			this.butOnCallN.Click += new System.EventHandler(this.butOnCallN_Click);
			// 
			// butEffectiveCommN
			// 
			this.butEffectiveCommN.Location = new System.Drawing.Point(614, 62);
			this.butEffectiveCommN.Name = "butEffectiveCommN";
			this.butEffectiveCommN.Size = new System.Drawing.Size(23, 20);
			this.butEffectiveCommN.TabIndex = 198;
			this.butEffectiveCommN.Text = "N";
			this.butEffectiveCommN.UseVisualStyleBackColor = true;
			this.butEffectiveCommN.Visible = false;
			this.butEffectiveCommN.Click += new System.EventHandler(this.butEffectiveCommN_Click);
			// 
			// labelOnCall
			// 
			this.labelOnCall.Location = new System.Drawing.Point(498, 43);
			this.labelOnCall.Name = "labelOnCall";
			this.labelOnCall.Size = new System.Drawing.Size(90, 16);
			this.labelOnCall.TabIndex = 196;
			this.labelOnCall.Text = "On Call";
			this.labelOnCall.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelOnCall.Visible = false;
			// 
			// labelEffectiveComm
			// 
			this.labelEffectiveComm.Location = new System.Drawing.Point(498, 64);
			this.labelEffectiveComm.Name = "labelEffectiveComm";
			this.labelEffectiveComm.Size = new System.Drawing.Size(90, 16);
			this.labelEffectiveComm.TabIndex = 196;
			this.labelEffectiveComm.Text = "Effective Comm";
			this.labelEffectiveComm.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelEffectiveComm.Visible = false;
			// 
			// butEffectiveCommY
			// 
			this.butEffectiveCommY.Location = new System.Drawing.Point(590, 62);
			this.butEffectiveCommY.Name = "butEffectiveCommY";
			this.butEffectiveCommY.Size = new System.Drawing.Size(23, 20);
			this.butEffectiveCommY.TabIndex = 198;
			this.butEffectiveCommY.Text = "Y";
			this.butEffectiveCommY.UseVisualStyleBackColor = true;
			this.butEffectiveCommY.Visible = false;
			this.butEffectiveCommY.Click += new System.EventHandler(this.butEffectiveCommY_Click);
			// 
			// gridPlanned
			// 
			this.gridPlanned.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPlanned.Location = new System.Drawing.Point(0, 28);
			this.gridPlanned.Name = "gridPlanned";
			this.gridPlanned.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridPlanned.Size = new System.Drawing.Size(315, 131);
			this.gridPlanned.TabIndex = 204;
			this.gridPlanned.Title = "Planned Appointments";
			this.gridPlanned.TranslationName = "TablePlannedAppts";
			this.gridPlanned.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPlanned_CellDoubleClick);
			// 
			// butNew
			// 
			this.butNew.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(43, 3);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(75, 23);
			this.butNew.TabIndex = 205;
			this.butNew.Text = "Add";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// butClear
			// 
			this.butClear.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(123, 3);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 23);
			this.butClear.TabIndex = 206;
			this.butClear.Text = "Delete";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(203, 3);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 23);
			this.butUp.TabIndex = 207;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butDown
			// 
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(283, 3);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 23);
			this.butDown.TabIndex = 208;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// panelPlanned
			// 
			this.panelPlanned.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelPlanned.Controls.Add(this.butDown);
			this.panelPlanned.Controls.Add(this.butUp);
			this.panelPlanned.Controls.Add(this.butClear);
			this.panelPlanned.Controls.Add(this.butNew);
			this.panelPlanned.Controls.Add(this.gridPlanned);
			this.panelPlanned.Location = new System.Drawing.Point(501, 111);
			this.panelPlanned.Name = "panelPlanned";
			this.panelPlanned.Size = new System.Drawing.Size(392, 159);
			this.panelPlanned.TabIndex = 199;
			this.panelPlanned.Visible = false;
			// 
			// labelDPCpost
			// 
			this.labelDPCpost.Location = new System.Drawing.Point(488, 15);
			this.labelDPCpost.Name = "labelDPCpost";
			this.labelDPCpost.Size = new System.Drawing.Size(100, 16);
			this.labelDPCpost.TabIndex = 201;
			this.labelDPCpost.Text = "DPC Post Visit";
			this.labelDPCpost.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelDPCpost.Visible = false;
			// 
			// comboDPCpost
			// 
			this.comboDPCpost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDPCpost.DropDownWidth = 177;
			this.comboDPCpost.FormattingEnabled = true;
			this.comboDPCpost.Location = new System.Drawing.Point(590, 14);
			this.comboDPCpost.MaxDropDownItems = 30;
			this.comboDPCpost.Name = "comboDPCpost";
			this.comboDPCpost.Size = new System.Drawing.Size(177, 21);
			this.comboDPCpost.TabIndex = 200;
			this.comboDPCpost.Visible = false;
			this.comboDPCpost.SelectionChangeCommitted += new System.EventHandler(this.comboDPCpost_SelectionChangeCommitted);
			// 
			// butLock
			// 
			this.butLock.Location = new System.Drawing.Point(413, 22);
			this.butLock.Name = "butLock";
			this.butLock.Size = new System.Drawing.Size(80, 22);
			this.butLock.TabIndex = 204;
			this.butLock.Text = "Lock";
			this.butLock.Click += new System.EventHandler(this.butLock_Click);
			// 
			// butInvalidate
			// 
			this.butInvalidate.Location = new System.Drawing.Point(412, 3);
			this.butInvalidate.Name = "butInvalidate";
			this.butInvalidate.Size = new System.Drawing.Size(80, 22);
			this.butInvalidate.TabIndex = 205;
			this.butInvalidate.Text = "Invalidate";
			this.butInvalidate.Visible = false;
			this.butInvalidate.Click += new System.EventHandler(this.butInvalidate_Click);
			// 
			// butAppend
			// 
			this.butAppend.Location = new System.Drawing.Point(413, 50);
			this.butAppend.Name = "butAppend";
			this.butAppend.Size = new System.Drawing.Size(80, 22);
			this.butAppend.TabIndex = 203;
			this.butAppend.Text = "Append";
			this.butAppend.Visible = false;
			this.butAppend.Click += new System.EventHandler(this.butAppend_Click);
			// 
			// labelLocked
			// 
			this.labelLocked.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelLocked.ForeColor = System.Drawing.Color.DarkRed;
			this.labelLocked.Location = new System.Drawing.Point(372, 29);
			this.labelLocked.Name = "labelLocked";
			this.labelLocked.Size = new System.Drawing.Size(123, 18);
			this.labelLocked.TabIndex = 202;
			this.labelLocked.Text = "Locked";
			this.labelLocked.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.labelLocked.Visible = false;
			// 
			// labelInvalid
			// 
			this.labelInvalid.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInvalid.ForeColor = System.Drawing.Color.DarkRed;
			this.labelInvalid.Location = new System.Drawing.Point(304, 7);
			this.labelInvalid.Name = "labelInvalid";
			this.labelInvalid.Size = new System.Drawing.Size(102, 18);
			this.labelInvalid.TabIndex = 206;
			this.labelInvalid.Text = "Invalid";
			this.labelInvalid.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			this.labelInvalid.Visible = false;
			// 
			// butChangeUser
			// 
			this.butChangeUser.Location = new System.Drawing.Point(218, 50);
			this.butChangeUser.Name = "butChangeUser";
			this.butChangeUser.Size = new System.Drawing.Size(23, 22);
			this.butChangeUser.TabIndex = 207;
			this.butChangeUser.Text = "...";
			this.butChangeUser.Click += new System.EventHandler(this.butChangeUser_Click);
			// 
			// labelPermAlert
			// 
			this.labelPermAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPermAlert.ForeColor = System.Drawing.Color.DarkRed;
			this.labelPermAlert.Location = new System.Drawing.Point(95, 348);
			this.labelPermAlert.Name = "labelPermAlert";
			this.labelPermAlert.Size = new System.Drawing.Size(367, 24);
			this.labelPermAlert.TabIndex = 209;
			this.labelPermAlert.Text = "Notes and Signature locked. Need GroupNoteUser permission.";
			this.labelPermAlert.Visible = false;
			// 
			// butEditAutoNote
			// 
			this.butEditAutoNote.Location = new System.Drawing.Point(245, 51);
			this.butEditAutoNote.Name = "butEditAutoNote";
			this.butEditAutoNote.Size = new System.Drawing.Size(82, 21);
			this.butEditAutoNote.TabIndex = 210;
			this.butEditAutoNote.Text = "Edit Auto Note";
			this.butEditAutoNote.Click += new System.EventHandler(this.ButEditAutoNote_Click);
			// 
			// FormProcGroup
			// 
			this.ClientSize = new System.Drawing.Size(905, 645);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butEditAutoNote);
			this.Controls.Add(this.labelPermAlert);
			this.Controls.Add(this.butChangeUser);
			this.Controls.Add(this.labelInvalid);
			this.Controls.Add(this.butLock);
			this.Controls.Add(this.butInvalidate);
			this.Controls.Add(this.butAppend);
			this.Controls.Add(this.labelLocked);
			this.Controls.Add(this.buttonUseAutoNote);
			this.Controls.Add(this.labelDPCpost);
			this.Controls.Add(this.comboDPCpost);
			this.Controls.Add(this.panelPlanned);
			this.Controls.Add(this.butRepairN);
			this.Controls.Add(this.butEffectiveCommN);
			this.Controls.Add(this.butOnCallN);
			this.Controls.Add(this.butRepairY);
			this.Controls.Add(this.butEffectiveCommY);
			this.Controls.Add(this.butOnCallY);
			this.Controls.Add(this.labelRepair);
			this.Controls.Add(this.labelEffectiveComm);
			this.Controls.Add(this.labelOnCall);
			this.Controls.Add(this.gridPat);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textProcDate);
			this.Controls.Add(this.signatureBoxWrapper);
			this.Controls.Add(this.label26);
			this.Controls.Add(this.gridProc);
			this.Controls.Add(this.textDateEntry);
			this.Controls.Add(this.butRx);
			this.Controls.Add(this.butExamSheets);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butDelete);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcGroup";
			this.ShowInTaskbar = false;
			this.Text = "Group Note";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormProcGroup_FormClosing);
			this.Load += new System.EventHandler(this.FormProcGroup_Load);
			this.panelPlanned.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label7;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ODtextBox textNotes;
		private Label label15;
		private Label label16;
		private TextBox textUser;
		private OpenDental.UI.Button buttonUseAutoNote;
		private UI.GridOD gridProc;
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private Label label12;
		private ValidDate textDateEntry;
		private Label label26;
		private ValidDate textProcDate;
		private Label label2;
		private UI.GridOD gridPat;
		private UI.Button butRx;
		private UI.Button butExamSheets;
		private Label labelRepair;
		private System.Windows.Forms.Button butRepairY;
		private System.Windows.Forms.Button butRepairN;
		private System.Windows.Forms.Button butOnCallY;
		private System.Windows.Forms.Button butOnCallN;
		private System.Windows.Forms.Button butEffectiveCommN;
		private Label labelOnCall;
		private Label labelEffectiveComm;
		private System.Windows.Forms.Button butEffectiveCommY;
		private UI.GridOD gridPlanned;
		private UI.Button butNew;
		private UI.Button butClear;
		private UI.Button butUp;
		private UI.Button butDown;
		private Panel panelPlanned;
		private Label labelDPCpost;
		private ComboBox comboDPCpost;
		private UI.Button butLock;
		private UI.Button butInvalidate;
		private UI.Button butAppend;
		private Label labelLocked;
		private Label labelInvalid;
		private UI.Button butChangeUser;
		private UI.Button butEditAutoNote;
		private Label labelPermAlert;
	}
}
