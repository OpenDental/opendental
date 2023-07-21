using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormReferralEdit {
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
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReferralEdit));
			this.textLName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textMName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textST = new System.Windows.Forms.TextBox();
			this.groupSSN = new OpenDental.UI.GroupBox();
			this.radioTIN = new System.Windows.Forms.RadioButton();
			this.radioSSN = new System.Windows.Forms.RadioButton();
			this.textSSN = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textPhone3 = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.textPhone2 = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textPhone1 = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.textOtherPhone = new OpenDental.ValidPhone();
			this.textZip = new System.Windows.Forms.TextBox();
			this.textCity = new System.Windows.Forms.TextBox();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.checkHidden = new OpenDental.UI.CheckBox();
			this.labelPatient = new System.Windows.Forms.Label();
			this.checkNotPerson = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.textPatientsNumFrom = new System.Windows.Forms.TextBox();
			this.comboPatientsFrom = new OpenDental.UI.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textPatientsNumTo = new System.Windows.Forms.TextBox();
			this.comboPatientsTo = new OpenDental.UI.ComboBox();
			this.textNationalProvID = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.comboSlip = new OpenDental.UI.ComboBox();
			this.butDelete = new OpenDental.UI.Button();
			this.textNotes = new OpenDental.ODtextBox();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkIsDoctor = new OpenDental.UI.CheckBox();
			this.checkEmailTrustDirect = new OpenDental.UI.CheckBox();
			this.checkIsPreferred = new OpenDental.UI.CheckBox();
			this.labelBusinessName = new System.Windows.Forms.Label();
			this.textBusinessName = new System.Windows.Forms.TextBox();
			this.textDisplayNote = new OpenDental.ODtextBox();
			this.labelDisplayNote = new System.Windows.Forms.Label();
			this.comboClinicPicker = new OpenDental.UI.ComboBoxClinicPicker();
			this.gridComm = new OpenDental.UI.GridOD();
			this.butAddComm = new OpenDental.UI.Button();
			this.checkHiddenComms = new OpenDental.UI.CheckBox();
			this.comboSpecialty = new OpenDental.UI.ComboBox();
			this.groupSSN.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(149, 47);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(297, 20);
			this.textLName.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(44, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(44, 71);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "First Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(149, 69);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(297, 20);
			this.textFName.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(44, 93);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Middle Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textMName
			// 
			this.textMName.Location = new System.Drawing.Point(149, 91);
			this.textMName.Name = "textMName";
			this.textMName.Size = new System.Drawing.Size(169, 20);
			this.textMName.TabIndex = 2;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(44, 203);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "ST";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textST
			// 
			this.textST.Location = new System.Drawing.Point(149, 201);
			this.textST.Name = "textST";
			this.textST.Size = new System.Drawing.Size(118, 20);
			this.textST.TabIndex = 7;
			// 
			// groupSSN
			// 
			this.groupSSN.ColorBackLabel = System.Drawing.Color.Empty;
			this.groupSSN.Controls.Add(this.radioTIN);
			this.groupSSN.Controls.Add(this.radioSSN);
			this.groupSSN.Controls.Add(this.textSSN);
			this.groupSSN.Location = new System.Drawing.Point(141, 354);
			this.groupSSN.Name = "groupSSN";
			this.groupSSN.Size = new System.Drawing.Size(141, 86);
			this.groupSSN.TabIndex = 29;
			this.groupSSN.Text = "SSN or TIN (no dashes)";
			// 
			// radioTIN
			// 
			this.radioTIN.Location = new System.Drawing.Point(9, 39);
			this.radioTIN.Name = "radioTIN";
			this.radioTIN.Size = new System.Drawing.Size(104, 16);
			this.radioTIN.TabIndex = 1;
			this.radioTIN.Text = "TIN";
			this.radioTIN.Click += new System.EventHandler(this.radioTIN_Click);
			// 
			// radioSSN
			// 
			this.radioSSN.Checked = true;
			this.radioSSN.Location = new System.Drawing.Point(9, 17);
			this.radioSSN.Name = "radioSSN";
			this.radioSSN.Size = new System.Drawing.Size(104, 16);
			this.radioSSN.TabIndex = 0;
			this.radioSSN.TabStop = true;
			this.radioSSN.Text = "SSN";
			this.radioSSN.Click += new System.EventHandler(this.radioSSN_Click);
			// 
			// textSSN
			// 
			this.textSSN.Location = new System.Drawing.Point(8, 61);
			this.textSSN.Name = "textSSN";
			this.textSSN.Size = new System.Drawing.Size(100, 20);
			this.textSSN.TabIndex = 2;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(51, 508);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(99, 16);
			this.label10.TabIndex = 0;
			this.label10.Text = "Specialty";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPhone3
			// 
			this.textPhone3.Location = new System.Drawing.Point(230, 245);
			this.textPhone3.MaxLength = 4;
			this.textPhone3.Name = "textPhone3";
			this.textPhone3.Size = new System.Drawing.Size(39, 20);
			this.textPhone3.TabIndex = 11;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(220, 247);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(6, 16);
			this.label13.TabIndex = 0;
			this.label13.Text = "-";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textPhone2
			// 
			this.textPhone2.Location = new System.Drawing.Point(190, 245);
			this.textPhone2.MaxLength = 3;
			this.textPhone2.Name = "textPhone2";
			this.textPhone2.Size = new System.Drawing.Size(28, 20);
			this.textPhone2.TabIndex = 10;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(178, 247);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(6, 16);
			this.label11.TabIndex = 0;
			this.label11.Text = ")";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(139, 247);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(11, 16);
			this.label12.TabIndex = 0;
			this.label12.Text = "(";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPhone1
			// 
			this.textPhone1.Location = new System.Drawing.Point(149, 245);
			this.textPhone1.MaxLength = 3;
			this.textPhone1.Name = "textPhone1";
			this.textPhone1.Size = new System.Drawing.Size(28, 20);
			this.textPhone1.TabIndex = 9;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(44, 247);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(95, 16);
			this.label14.TabIndex = 0;
			this.label14.Text = "Phone";
			this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textTitle
			// 
			this.textTitle.Location = new System.Drawing.Point(149, 113);
			this.textTitle.MaxLength = 100;
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(70, 20);
			this.textTitle.TabIndex = 3;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(60, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Title (DDS)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(149, 289);
			this.textEmail.MaxLength = 100;
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(297, 20);
			this.textEmail.TabIndex = 13;
			// 
			// textOtherPhone
			// 
			this.textOtherPhone.Location = new System.Drawing.Point(149, 267);
			this.textOtherPhone.MaxLength = 30;
			this.textOtherPhone.Name = "textOtherPhone";
			this.textOtherPhone.Size = new System.Drawing.Size(161, 20);
			this.textOtherPhone.TabIndex = 12;
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(149, 223);
			this.textZip.MaxLength = 10;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(161, 20);
			this.textZip.TabIndex = 8;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(149, 179);
			this.textCity.MaxLength = 50;
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(190, 20);
			this.textCity.TabIndex = 6;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(149, 157);
			this.textAddress2.MaxLength = 100;
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(297, 20);
			this.textAddress2.TabIndex = 5;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(149, 135);
			this.textAddress.MaxLength = 100;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(297, 20);
			this.textAddress.TabIndex = 4;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(44, 291);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(104, 16);
			this.label22.TabIndex = 0;
			this.label22.Text = "E-mail";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(44, 269);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(104, 16);
			this.label16.TabIndex = 0;
			this.label16.Text = "Other Phone";
			this.label16.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(44, 225);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(104, 16);
			this.label15.TabIndex = 0;
			this.label15.Text = "Zip";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(44, 181);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(104, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "City";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(44, 159);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(104, 16);
			this.label8.TabIndex = 0;
			this.label8.Text = "Address2";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(44, 137);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(104, 16);
			this.label9.TabIndex = 0;
			this.label9.Text = "Address";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(503, 121);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(101, 14);
			this.label17.TabIndex = 0;
			this.label17.Text = "Notes";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkHidden
			// 
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(47, 24);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkHidden.Size = new System.Drawing.Size(115, 18);
			this.checkHidden.TabIndex = 27;
			this.checkHidden.Text = "Hidden  ";
			// 
			// labelPatient
			// 
			this.labelPatient.Location = new System.Drawing.Point(63, 8);
			this.labelPatient.Name = "labelPatient";
			this.labelPatient.Size = new System.Drawing.Size(612, 17);
			this.labelPatient.TabIndex = 0;
			this.labelPatient.Text = "This referral is a patient.  Some information can only be changed from the patien" +
    "t\'s edit form.";
			this.labelPatient.Visible = false;
			// 
			// checkNotPerson
			// 
			this.checkNotPerson.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNotPerson.Location = new System.Drawing.Point(167, 24);
			this.checkNotPerson.Name = "checkNotPerson";
			this.checkNotPerson.Size = new System.Drawing.Size(115, 18);
			this.checkNotPerson.TabIndex = 28;
			this.checkNotPerson.Text = "Not Person";
			// 
			// groupBox2
			// 
			this.groupBox2.ColorBackLabel = System.Drawing.Color.Empty;
			this.groupBox2.Controls.Add(this.label18);
			this.groupBox2.Controls.Add(this.textPatientsNumFrom);
			this.groupBox2.Controls.Add(this.comboPatientsFrom);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textPatientsNumTo);
			this.groupBox2.Controls.Add(this.comboPatientsTo);
			this.groupBox2.Location = new System.Drawing.Point(590, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(383, 109);
			this.groupBox2.TabIndex = 30;
			this.groupBox2.Text = "Used By Patients";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(15, 57);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(323, 19);
			this.label18.TabIndex = 0;
			this.label18.Text = "Patients referred FROM this referral";
			this.label18.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPatientsNumFrom
			// 
			this.textPatientsNumFrom.BackColor = System.Drawing.Color.White;
			this.textPatientsNumFrom.Location = new System.Drawing.Point(17, 78);
			this.textPatientsNumFrom.Name = "textPatientsNumFrom";
			this.textPatientsNumFrom.ReadOnly = true;
			this.textPatientsNumFrom.Size = new System.Drawing.Size(35, 20);
			this.textPatientsNumFrom.TabIndex = 3;
			// 
			// comboPatientsFrom
			// 
			this.comboPatientsFrom.Location = new System.Drawing.Point(61, 78);
			this.comboPatientsFrom.Name = "comboPatientsFrom";
			this.comboPatientsFrom.Size = new System.Drawing.Size(299, 21);
			this.comboPatientsFrom.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(15, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(323, 19);
			this.label6.TabIndex = 0;
			this.label6.Text = "Patients referred TO this referral";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textPatientsNumTo
			// 
			this.textPatientsNumTo.BackColor = System.Drawing.Color.White;
			this.textPatientsNumTo.Location = new System.Drawing.Point(17, 35);
			this.textPatientsNumTo.Name = "textPatientsNumTo";
			this.textPatientsNumTo.ReadOnly = true;
			this.textPatientsNumTo.Size = new System.Drawing.Size(35, 20);
			this.textPatientsNumTo.TabIndex = 1;
			// 
			// comboPatientsTo
			// 
			this.comboPatientsTo.Location = new System.Drawing.Point(61, 35);
			this.comboPatientsTo.Name = "comboPatientsTo";
			this.comboPatientsTo.Size = new System.Drawing.Size(299, 21);
			this.comboPatientsTo.TabIndex = 0;
			// 
			// textNationalProvID
			// 
			this.textNationalProvID.Location = new System.Drawing.Point(149, 444);
			this.textNationalProvID.Name = "textNationalProvID";
			this.textNationalProvID.Size = new System.Drawing.Size(100, 20);
			this.textNationalProvID.TabIndex = 16;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(31, 444);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(117, 20);
			this.label19.TabIndex = 0;
			this.label19.Text = "National Provider ID";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(177, 248);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(11, 16);
			this.label20.TabIndex = 0;
			this.label20.Text = ")";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(3, 530);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(140, 44);
			this.label21.TabIndex = 0;
			this.label21.Text = "Referral Slip\r\n(custom referral slips may be added in Sheets)";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboSlip
			// 
			this.comboSlip.Location = new System.Drawing.Point(149, 539);
			this.comboSlip.Name = "comboSlip";
			this.comboSlip.Size = new System.Drawing.Size(275, 21);
			this.comboSlip.TabIndex = 20;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 623);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 31;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.Location = new System.Drawing.Point(606, 121);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.Referral;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(588, 47);
			this.textNotes.TabIndex = 22;
			this.textNotes.Text = "";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1143, 623);
			this.butCancel.Name = "butCancel";
			this.butCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 32;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1054, 623);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 33;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkIsDoctor
			// 
			this.checkIsDoctor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsDoctor.Location = new System.Drawing.Point(76, 485);
			this.checkIsDoctor.Name = "checkIsDoctor";
			this.checkIsDoctor.Size = new System.Drawing.Size(86, 18);
			this.checkIsDoctor.TabIndex = 18;
			this.checkIsDoctor.Text = "Is Doctor";
			// 
			// checkEmailTrustDirect
			// 
			this.checkEmailTrustDirect.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEmailTrustDirect.Location = new System.Drawing.Point(6, 334);
			this.checkEmailTrustDirect.Name = "checkEmailTrustDirect";
			this.checkEmailTrustDirect.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkEmailTrustDirect.Size = new System.Drawing.Size(156, 16);
			this.checkEmailTrustDirect.TabIndex = 15;
			this.checkEmailTrustDirect.Text = "E-mail Trust for Direct";
			// 
			// checkIsPreferred
			// 
			this.checkIsPreferred.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPreferred.Location = new System.Drawing.Point(12, 467);
			this.checkIsPreferred.Name = "checkIsPreferred";
			this.checkIsPreferred.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsPreferred.Size = new System.Drawing.Size(150, 16);
			this.checkIsPreferred.TabIndex = 17;
			this.checkIsPreferred.Text = "Preferred Referral";
			// 
			// labelBusinessName
			// 
			this.labelBusinessName.Location = new System.Drawing.Point(44, 313);
			this.labelBusinessName.Name = "labelBusinessName";
			this.labelBusinessName.Size = new System.Drawing.Size(104, 16);
			this.labelBusinessName.TabIndex = 84;
			this.labelBusinessName.Text = "Business Name";
			this.labelBusinessName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBusinessName
			// 
			this.textBusinessName.Location = new System.Drawing.Point(149, 311);
			this.textBusinessName.MaxLength = 100;
			this.textBusinessName.Name = "textBusinessName";
			this.textBusinessName.Size = new System.Drawing.Size(297, 20);
			this.textBusinessName.TabIndex = 14;
			// 
			// textDisplayNote
			// 
			this.textDisplayNote.AcceptsTab = true;
			this.textDisplayNote.BackColor = System.Drawing.SystemColors.Window;
			this.textDisplayNote.DetectLinksEnabled = false;
			this.textDisplayNote.DetectUrls = false;
			this.textDisplayNote.Location = new System.Drawing.Point(606, 170);
			this.textDisplayNote.Name = "textDisplayNote";
			this.textDisplayNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Referral;
			this.textDisplayNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDisplayNote.Size = new System.Drawing.Size(273, 47);
			this.textDisplayNote.TabIndex = 23;
			this.textDisplayNote.Text = "";
			// 
			// labelDisplayNote
			// 
			this.labelDisplayNote.Location = new System.Drawing.Point(469, 170);
			this.labelDisplayNote.Name = "labelDisplayNote";
			this.labelDisplayNote.Size = new System.Drawing.Size(137, 31);
			this.labelDisplayNote.TabIndex = 0;
			this.labelDisplayNote.Text = "Display Notes\r\n(shows in Family module)";
			this.labelDisplayNote.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboClinicPicker
			// 
			this.comboClinicPicker.IncludeUnassigned = true;
			this.comboClinicPicker.Location = new System.Drawing.Point(112, 581);
			this.comboClinicPicker.Name = "comboClinicPicker";
			this.comboClinicPicker.SelectionModeMulti = true;
			this.comboClinicPicker.Size = new System.Drawing.Size(200, 21);
			this.comboClinicPicker.TabIndex = 21;
			// 
			// gridComm
			// 
			this.gridComm.Location = new System.Drawing.Point(493, 245);
			this.gridComm.Name = "gridComm";
			this.gridComm.Size = new System.Drawing.Size(701, 357);
			this.gridComm.TabIndex = 26;
			this.gridComm.Title = "Communications Log - Referral Details";
			this.gridComm.TranslationName = "TableCommLog";
			this.gridComm.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridComm_CellDoubleClick);
			// 
			// butAddComm
			// 
			this.butAddComm.Icon = OpenDental.UI.EnumIcons.CommLog;
			this.butAddComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddComm.Location = new System.Drawing.Point(493, 217);
			this.butAddComm.Name = "butAddComm";
			this.butAddComm.Size = new System.Drawing.Size(100, 24);
			this.butAddComm.TabIndex = 24;
			this.butAddComm.Text = "Add Comm";
			this.butAddComm.Click += new System.EventHandler(this.butAddComm_Click);
			// 
			// checkHiddenComms
			// 
			this.checkHiddenComms.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHiddenComms.Location = new System.Drawing.Point(1079, 223);
			this.checkHiddenComms.Name = "checkHiddenComms";
			this.checkHiddenComms.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkHiddenComms.Size = new System.Drawing.Size(115, 18);
			this.checkHiddenComms.TabIndex = 25;
			this.checkHiddenComms.Text = "Show Hidden  ";
			this.checkHiddenComms.Click += new System.EventHandler(this.checkHiddenComms_Click);
			// 
			// comboSpecialty
			// 
			this.comboSpecialty.Location = new System.Drawing.Point(149, 505);
			this.comboSpecialty.Name = "comboSpecialty";
			this.comboSpecialty.Size = new System.Drawing.Size(201, 21);
			this.comboSpecialty.TabIndex = 19;
			// 
			// FormReferralEdit
			// 
			this.ClientSize = new System.Drawing.Size(1230, 659);
			this.Controls.Add(this.comboSpecialty);
			this.Controls.Add(this.checkHiddenComms);
			this.Controls.Add(this.butAddComm);
			this.Controls.Add(this.gridComm);
			this.Controls.Add(this.comboClinicPicker);
			this.Controls.Add(this.labelDisplayNote);
			this.Controls.Add(this.textDisplayNote);
			this.Controls.Add(this.textBusinessName);
			this.Controls.Add(this.labelBusinessName);
			this.Controls.Add(this.checkIsPreferred);
			this.Controls.Add(this.checkEmailTrustDirect);
			this.Controls.Add(this.checkIsDoctor);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.comboSlip);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.textNationalProvID);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.checkNotPerson);
			this.Controls.Add(this.labelPatient);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.textOtherPhone);
			this.Controls.Add(this.textZip);
			this.Controls.Add(this.textCity);
			this.Controls.Add(this.textAddress2);
			this.Controls.Add(this.textAddress);
			this.Controls.Add(this.textTitle);
			this.Controls.Add(this.textPhone3);
			this.Controls.Add(this.textPhone2);
			this.Controls.Add(this.textPhone1);
			this.Controls.Add(this.textST);
			this.Controls.Add(this.textMName);
			this.Controls.Add(this.textFName);
			this.Controls.Add(this.textLName);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.groupSSN);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReferralEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Referral";
			this.Load += new System.EventHandler(this.FormReferralEdit_Load);
			this.groupSSN.ResumeLayout(false);
			this.groupSSN.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.GroupBox groupSSN;
		private System.Windows.Forms.RadioButton radioTIN;
		private System.Windows.Forms.RadioButton radioSSN;
		private System.Windows.Forms.TextBox textSSN;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textPhone3;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textPhone2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textPhone1;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textMName;
		private System.Windows.Forms.TextBox textST;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textEmail;
		private ValidPhone textOtherPhone;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.TextBox textAddress2;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label labelPatient;
		private OpenDental.UI.CheckBox checkNotPerson;
		private OpenDental.ODtextBox textNotes;
		private OpenDental.UI.CheckBox checkHidden;
		private OpenDental.UI.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textPatientsNumTo;
		private OpenDental.UI.ComboBox comboPatientsTo;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textPatientsNumFrom;
		private OpenDental.UI.ComboBox comboPatientsFrom;
		private TextBox textNationalProvID;
		private Label label19;
		private Label label20;
		private Label label21;
		private OpenDental.UI.ComboBox comboSlip;
		private UI.Button butDelete;
		private OpenDental.UI.CheckBox checkIsDoctor;
		private OpenDental.UI.CheckBox checkEmailTrustDirect;
		private OpenDental.UI.CheckBox checkIsPreferred;
		private Label labelBusinessName;
		private TextBox textBusinessName;
		private ODtextBox textDisplayNote;
		private Label labelDisplayNote;
		private UI.ComboBoxClinicPicker comboClinicPicker;
		private UI.GridOD gridComm;
		private UI.Button butAddComm;
		private UI.CheckBox checkHiddenComms;
		private UI.ComboBox comboSpecialty;
	}
}
