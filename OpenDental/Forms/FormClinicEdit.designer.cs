using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClinicEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClinicEdit));
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.textMedLabAcctNum = new System.Windows.Forms.TextBox();
			this.labelMedLabAcctNum = new System.Windows.Forms.Label();
			this.textClinicAbbr = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.checkExcludeFromInsVerifyList = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.PhysicalAddress = new System.Windows.Forms.TabPage();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.textZip = new System.Windows.Forms.TextBox();
			this.BillingAddress = new System.Windows.Forms.TabPage();
			this.checkUseBillingAddressOnClaims = new System.Windows.Forms.CheckBox();
			this.label18 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.textBillingZip = new System.Windows.Forms.TextBox();
			this.textBillingAddress = new System.Windows.Forms.TextBox();
			this.textBillingST = new System.Windows.Forms.TextBox();
			this.textBillingAddress2 = new System.Windows.Forms.TextBox();
			this.textBillingCity = new System.Windows.Forms.TextBox();
			this.PayToAddress = new System.Windows.Forms.TabPage();
			this.label17 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.textPayToZip = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.textPayToST = new System.Windows.Forms.TextBox();
			this.textPayToAddress = new System.Windows.Forms.TextBox();
			this.textPayToCity = new System.Windows.Forms.TextBox();
			this.textPayToAddress2 = new System.Windows.Forms.TextBox();
			this.tabSpecialty = new System.Windows.Forms.TabPage();
			this.butRemove = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.gridSpecialty = new OpenDental.UI.GridOD();
			this.labelTimeZone = new System.Windows.Forms.Label();
			this.comboBoxTimeZone = new OpenDental.UI.ComboBoxOD();
			this.comboRegion = new System.Windows.Forms.ComboBox();
			this.label22 = new System.Windows.Forms.Label();
			this.textClinicNum = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.checkIsMedicalOnly = new System.Windows.Forms.CheckBox();
			this.butNone = new OpenDental.UI.Button();
			this.butPickDefaultProv = new OpenDental.UI.Button();
			this.comboDefaultProvider = new OpenDental.UI.ComboBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.textFax = new OpenDental.ValidPhone();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.butPickInsBillingProv = new OpenDental.UI.Button();
			this.comboInsBillingProv = new OpenDental.UI.ComboBoxOD();
			this.radioInsBillingProvSpecific = new System.Windows.Forms.RadioButton();
			this.radioInsBillingProvTreat = new System.Windows.Forms.RadioButton();
			this.radioInsBillingProvDefault = new System.Windows.Forms.RadioButton();
			this.label10 = new System.Windows.Forms.Label();
			this.comboPlaceService = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.textBankNumber = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.label2 = new System.Windows.Forms.Label();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butEmail = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textExternalID = new System.Windows.Forms.TextBox();
			this.labelExternalID = new System.Windows.Forms.Label();
			this.butEmailNone = new OpenDental.UI.Button();
			this.textSchedRules = new System.Windows.Forms.TextBox();
			this.labelSchedRules = new System.Windows.Forms.Label();
			this.checkProcCodeRequired = new System.Windows.Forms.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.comboDefaultBillingType = new OpenDental.UI.ComboBoxOD();
			this.labelDefaultBillingType = new System.Windows.Forms.Label();
			this.checkAlwaysAssignBenToPatient = new System.Windows.Forms.CheckBox();
			this.label25 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.PhysicalAddress.SuspendLayout();
			this.BillingAddress.SuspendLayout();
			this.PayToAddress.SuspendLayout();
			this.tabSpecialty.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkHidden
			// 
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(76, 392);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(163, 16);
			this.checkHidden.TabIndex = 17;
			this.checkHidden.Text = "Is Hidden";
			this.checkHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.CheckedChanged += new System.EventHandler(this.checkHidden_CheckedChanged);
			// 
			// textMedLabAcctNum
			// 
			this.textMedLabAcctNum.Location = new System.Drawing.Point(225, 272);
			this.textMedLabAcctNum.MaxLength = 255;
			this.textMedLabAcctNum.Name = "textMedLabAcctNum";
			this.textMedLabAcctNum.Size = new System.Drawing.Size(216, 20);
			this.textMedLabAcctNum.TabIndex = 14;
			this.textMedLabAcctNum.Visible = false;
			// 
			// labelMedLabAcctNum
			// 
			this.labelMedLabAcctNum.Location = new System.Drawing.Point(26, 273);
			this.labelMedLabAcctNum.Name = "labelMedLabAcctNum";
			this.labelMedLabAcctNum.Size = new System.Drawing.Size(198, 17);
			this.labelMedLabAcctNum.TabIndex = 26;
			this.labelMedLabAcctNum.Text = "MedLab Account Number";
			this.labelMedLabAcctNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelMedLabAcctNum.Visible = false;
			// 
			// textClinicAbbr
			// 
			this.textClinicAbbr.Location = new System.Drawing.Point(225, 93);
			this.textClinicAbbr.Name = "textClinicAbbr";
			this.textClinicAbbr.Size = new System.Drawing.Size(157, 20);
			this.textClinicAbbr.TabIndex = 3;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(17, 93);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(207, 17);
			this.label23.TabIndex = 24;
			this.label23.Text = "Abbreviation";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkExcludeFromInsVerifyList
			// 
			this.checkExcludeFromInsVerifyList.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeFromInsVerifyList.Location = new System.Drawing.Point(7, 199);
			this.checkExcludeFromInsVerifyList.Name = "checkExcludeFromInsVerifyList";
			this.checkExcludeFromInsVerifyList.Size = new System.Drawing.Size(232, 16);
			this.checkExcludeFromInsVerifyList.TabIndex = 8;
			this.checkExcludeFromInsVerifyList.Text = "Hide From Insurance Verification List";
			this.checkExcludeFromInsVerifyList.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.PhysicalAddress);
			this.tabControl1.Controls.Add(this.BillingAddress);
			this.tabControl1.Controls.Add(this.PayToAddress);
			this.tabControl1.Controls.Add(this.tabSpecialty);
			this.tabControl1.Location = new System.Drawing.Point(587, 36);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(460, 141);
			this.tabControl1.TabIndex = 18;
			this.tabControl1.TabStop = false;
			// 
			// PhysicalAddress
			// 
			this.PhysicalAddress.BackColor = System.Drawing.Color.Transparent;
			this.PhysicalAddress.Controls.Add(this.textAddress);
			this.PhysicalAddress.Controls.Add(this.label3);
			this.PhysicalAddress.Controls.Add(this.label4);
			this.PhysicalAddress.Controls.Add(this.textCity);
			this.PhysicalAddress.Controls.Add(this.label11);
			this.PhysicalAddress.Controls.Add(this.textState);
			this.PhysicalAddress.Controls.Add(this.textAddress2);
			this.PhysicalAddress.Controls.Add(this.textZip);
			this.PhysicalAddress.Location = new System.Drawing.Point(4, 22);
			this.PhysicalAddress.Name = "PhysicalAddress";
			this.PhysicalAddress.Padding = new System.Windows.Forms.Padding(3);
			this.PhysicalAddress.Size = new System.Drawing.Size(452, 115);
			this.PhysicalAddress.TabIndex = 0;
			this.PhysicalAddress.Text = "Physical Treating Address";
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(105, 42);
			this.textAddress.MaxLength = 255;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(291, 20);
			this.textAddress.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.BackColor = System.Drawing.Color.Transparent;
			this.label3.Location = new System.Drawing.Point(9, 43);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(95, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Address";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.Transparent;
			this.label4.Location = new System.Drawing.Point(9, 64);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(95, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Address 2";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(105, 84);
			this.textCity.MaxLength = 255;
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(155, 20);
			this.textCity.TabIndex = 2;
			// 
			// label11
			// 
			this.label11.BackColor = System.Drawing.Color.Transparent;
			this.label11.Location = new System.Drawing.Point(9, 86);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(95, 15);
			this.label11.TabIndex = 0;
			this.label11.Text = "City, ST, Zip";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(259, 84);
			this.textState.MaxLength = 255;
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(66, 20);
			this.textState.TabIndex = 3;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(105, 63);
			this.textAddress2.MaxLength = 255;
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(291, 20);
			this.textAddress2.TabIndex = 1;
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(324, 84);
			this.textZip.MaxLength = 255;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(72, 20);
			this.textZip.TabIndex = 4;
			// 
			// BillingAddress
			// 
			this.BillingAddress.BackColor = System.Drawing.Color.Transparent;
			this.BillingAddress.Controls.Add(this.checkUseBillingAddressOnClaims);
			this.BillingAddress.Controls.Add(this.label18);
			this.BillingAddress.Controls.Add(this.label20);
			this.BillingAddress.Controls.Add(this.label16);
			this.BillingAddress.Controls.Add(this.label19);
			this.BillingAddress.Controls.Add(this.textBillingZip);
			this.BillingAddress.Controls.Add(this.textBillingAddress);
			this.BillingAddress.Controls.Add(this.textBillingST);
			this.BillingAddress.Controls.Add(this.textBillingAddress2);
			this.BillingAddress.Controls.Add(this.textBillingCity);
			this.BillingAddress.Location = new System.Drawing.Point(4, 22);
			this.BillingAddress.Name = "BillingAddress";
			this.BillingAddress.Padding = new System.Windows.Forms.Padding(3);
			this.BillingAddress.Size = new System.Drawing.Size(452, 115);
			this.BillingAddress.TabIndex = 1;
			this.BillingAddress.Text = "Billing Address";
			// 
			// checkUseBillingAddressOnClaims
			// 
			this.checkUseBillingAddressOnClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseBillingAddressOnClaims.Location = new System.Drawing.Point(8, 26);
			this.checkUseBillingAddressOnClaims.Name = "checkUseBillingAddressOnClaims";
			this.checkUseBillingAddressOnClaims.Size = new System.Drawing.Size(111, 16);
			this.checkUseBillingAddressOnClaims.TabIndex = 1;
			this.checkUseBillingAddressOnClaims.Text = "Use on Claims";
			this.checkUseBillingAddressOnClaims.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseBillingAddressOnClaims.UseVisualStyleBackColor = true;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(6, 5);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(440, 17);
			this.label18.TabIndex = 0;
			this.label18.Text = "Optional, for E-Claims.  Cannot be a PO Box.";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(6, 86);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(98, 15);
			this.label20.TabIndex = 0;
			this.label20.Text = "City, ST, Zip";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(7, 64);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(97, 16);
			this.label16.TabIndex = 0;
			this.label16.Text = "Address 2";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(6, 44);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(98, 14);
			this.label19.TabIndex = 0;
			this.label19.Text = "Address";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingZip
			// 
			this.textBillingZip.Location = new System.Drawing.Point(323, 84);
			this.textBillingZip.Name = "textBillingZip";
			this.textBillingZip.Size = new System.Drawing.Size(73, 20);
			this.textBillingZip.TabIndex = 6;
			// 
			// textBillingAddress
			// 
			this.textBillingAddress.Location = new System.Drawing.Point(105, 42);
			this.textBillingAddress.Name = "textBillingAddress";
			this.textBillingAddress.Size = new System.Drawing.Size(291, 20);
			this.textBillingAddress.TabIndex = 2;
			// 
			// textBillingST
			// 
			this.textBillingST.Location = new System.Drawing.Point(259, 84);
			this.textBillingST.Name = "textBillingST";
			this.textBillingST.Size = new System.Drawing.Size(65, 20);
			this.textBillingST.TabIndex = 5;
			// 
			// textBillingAddress2
			// 
			this.textBillingAddress2.Location = new System.Drawing.Point(105, 63);
			this.textBillingAddress2.Name = "textBillingAddress2";
			this.textBillingAddress2.Size = new System.Drawing.Size(291, 20);
			this.textBillingAddress2.TabIndex = 3;
			// 
			// textBillingCity
			// 
			this.textBillingCity.Location = new System.Drawing.Point(105, 84);
			this.textBillingCity.Name = "textBillingCity";
			this.textBillingCity.Size = new System.Drawing.Size(155, 20);
			this.textBillingCity.TabIndex = 4;
			// 
			// PayToAddress
			// 
			this.PayToAddress.BackColor = System.Drawing.Color.Transparent;
			this.PayToAddress.Controls.Add(this.label17);
			this.PayToAddress.Controls.Add(this.label13);
			this.PayToAddress.Controls.Add(this.label15);
			this.PayToAddress.Controls.Add(this.textPayToZip);
			this.PayToAddress.Controls.Add(this.label14);
			this.PayToAddress.Controls.Add(this.textPayToST);
			this.PayToAddress.Controls.Add(this.textPayToAddress);
			this.PayToAddress.Controls.Add(this.textPayToCity);
			this.PayToAddress.Controls.Add(this.textPayToAddress2);
			this.PayToAddress.Location = new System.Drawing.Point(4, 22);
			this.PayToAddress.Name = "PayToAddress";
			this.PayToAddress.Size = new System.Drawing.Size(452, 115);
			this.PayToAddress.TabIndex = 2;
			this.PayToAddress.Text = "Pay To Address";
			// 
			// label17
			// 
			this.label17.BackColor = System.Drawing.Color.Transparent;
			this.label17.Location = new System.Drawing.Point(6, 5);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(440, 17);
			this.label17.TabIndex = 0;
			this.label17.Text = "Optional for claims.  Can be a PO Box.  Sent in addition to treating or billing a" +
    "ddress.";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(7, 64);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(97, 16);
			this.label13.TabIndex = 0;
			this.label13.Text = "Address 2";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(6, 86);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(98, 15);
			this.label15.TabIndex = 0;
			this.label15.Text = "City, ST, Zip";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayToZip
			// 
			this.textPayToZip.Location = new System.Drawing.Point(324, 84);
			this.textPayToZip.Name = "textPayToZip";
			this.textPayToZip.Size = new System.Drawing.Size(72, 20);
			this.textPayToZip.TabIndex = 5;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 44);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(98, 14);
			this.label14.TabIndex = 0;
			this.label14.Text = "Address";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayToST
			// 
			this.textPayToST.Location = new System.Drawing.Point(259, 84);
			this.textPayToST.Name = "textPayToST";
			this.textPayToST.Size = new System.Drawing.Size(66, 20);
			this.textPayToST.TabIndex = 4;
			// 
			// textPayToAddress
			// 
			this.textPayToAddress.Location = new System.Drawing.Point(105, 42);
			this.textPayToAddress.Name = "textPayToAddress";
			this.textPayToAddress.Size = new System.Drawing.Size(291, 20);
			this.textPayToAddress.TabIndex = 1;
			// 
			// textPayToCity
			// 
			this.textPayToCity.Location = new System.Drawing.Point(105, 84);
			this.textPayToCity.Name = "textPayToCity";
			this.textPayToCity.Size = new System.Drawing.Size(155, 20);
			this.textPayToCity.TabIndex = 3;
			// 
			// textPayToAddress2
			// 
			this.textPayToAddress2.Location = new System.Drawing.Point(105, 63);
			this.textPayToAddress2.Name = "textPayToAddress2";
			this.textPayToAddress2.Size = new System.Drawing.Size(291, 20);
			this.textPayToAddress2.TabIndex = 2;
			// 
			// tabSpecialty
			// 
			this.tabSpecialty.Controls.Add(this.butRemove);
			this.tabSpecialty.Controls.Add(this.butAdd);
			this.tabSpecialty.Controls.Add(this.gridSpecialty);
			this.tabSpecialty.Location = new System.Drawing.Point(4, 22);
			this.tabSpecialty.Name = "tabSpecialty";
			this.tabSpecialty.Padding = new System.Windows.Forms.Padding(3);
			this.tabSpecialty.Size = new System.Drawing.Size(452, 115);
			this.tabSpecialty.TabIndex = 3;
			this.tabSpecialty.Text = "Specialty";
			// 
			// butRemove
			// 
			this.butRemove.Location = new System.Drawing.Point(376, 36);
			this.butRemove.Name = "butRemove";
			this.butRemove.Size = new System.Drawing.Size(73, 23);
			this.butRemove.TabIndex = 77;
			this.butRemove.Text = "Remove";
			this.butRemove.UseVisualStyleBackColor = true;
			this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
			// 
			// butAdd
			// 
			this.butAdd.Location = new System.Drawing.Point(376, 7);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(73, 23);
			this.butAdd.TabIndex = 76;
			this.butAdd.Text = "Add";
			this.butAdd.UseVisualStyleBackColor = true;
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridSpecialty
			// 
			this.gridSpecialty.Location = new System.Drawing.Point(6, 7);
			this.gridSpecialty.Name = "gridSpecialty";
			this.gridSpecialty.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridSpecialty.Size = new System.Drawing.Size(368, 105);
			this.gridSpecialty.TabIndex = 0;
			this.gridSpecialty.Title = "Clinic Specialty";
			this.gridSpecialty.TranslationName = "TableClinicSpecialty";
			// 
			// labelTimeZone
			// 
			this.labelTimeZone.Location = new System.Drawing.Point(17, 356);
			this.labelTimeZone.Name = "labelTimeZone";
			this.labelTimeZone.Size = new System.Drawing.Size(207, 17);
			this.labelTimeZone.TabIndex = 21;
			this.labelTimeZone.Text = "Time Zone (additional info for FHIR)";
			this.labelTimeZone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxTimeZone
			// 
			this.comboBoxTimeZone.Location = new System.Drawing.Point(225, 362);
			this.comboBoxTimeZone.Name = "comboBoxTimeZone";
			this.comboBoxTimeZone.Size = new System.Drawing.Size(260, 21);
			this.comboBoxTimeZone.TabIndex = 16;
			// 
			// comboRegion
			// 
			this.comboRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboRegion.FormattingEnabled = true;
			this.comboRegion.Location = new System.Drawing.Point(225, 177);
			this.comboRegion.Name = "comboRegion";
			this.comboRegion.Size = new System.Drawing.Size(157, 21);
			this.comboRegion.TabIndex = 7;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(14, 180);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(210, 17);
			this.label22.TabIndex = 20;
			this.label22.Text = "Region";
			this.label22.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textClinicNum
			// 
			this.textClinicNum.BackColor = System.Drawing.SystemColors.Control;
			this.textClinicNum.Location = new System.Drawing.Point(225, 51);
			this.textClinicNum.Name = "textClinicNum";
			this.textClinicNum.ReadOnly = true;
			this.textClinicNum.Size = new System.Drawing.Size(157, 20);
			this.textClinicNum.TabIndex = 1;
			this.textClinicNum.TabStop = false;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(17, 52);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(207, 17);
			this.label21.TabIndex = 0;
			this.label21.Text = "Clinic ID";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsMedicalOnly
			// 
			this.checkIsMedicalOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsMedicalOnly.Location = new System.Drawing.Point(82, 36);
			this.checkIsMedicalOnly.Name = "checkIsMedicalOnly";
			this.checkIsMedicalOnly.Size = new System.Drawing.Size(157, 16);
			this.checkIsMedicalOnly.TabIndex = 0;
			this.checkIsMedicalOnly.Text = "Is Medical";
			this.checkIsMedicalOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNone
			// 
			this.butNone.Location = new System.Drawing.Point(999, 301);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(48, 21);
			this.butNone.TabIndex = 23;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butPickDefaultProv
			// 
			this.butPickDefaultProv.Location = new System.Drawing.Point(973, 301);
			this.butPickDefaultProv.Name = "butPickDefaultProv";
			this.butPickDefaultProv.Size = new System.Drawing.Size(23, 21);
			this.butPickDefaultProv.TabIndex = 22;
			this.butPickDefaultProv.Text = "...";
			this.butPickDefaultProv.Click += new System.EventHandler(this.butPickDefaultProv_Click);
			// 
			// comboDefaultProvider
			// 
			this.comboDefaultProvider.Location = new System.Drawing.Point(755, 301);
			this.comboDefaultProvider.Name = "comboDefaultProvider";
			this.comboDefaultProvider.Size = new System.Drawing.Size(216, 21);
			this.comboDefaultProvider.TabIndex = 21;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(573, 303);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(180, 17);
			this.label12.TabIndex = 0;
			this.label12.Text = "Default Provider";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFax
			// 
			this.textFax.Location = new System.Drawing.Point(225, 156);
			this.textFax.MaxLength = 255;
			this.textFax.Name = "textFax";
			this.textFax.Size = new System.Drawing.Size(157, 20);
			this.textFax.TabIndex = 6;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(14, 159);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(210, 17);
			this.label8.TabIndex = 0;
			this.label8.Text = "Fax";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(385, 136);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(144, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "(###)###-####";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.butPickInsBillingProv);
			this.groupBox4.Controls.Add(this.comboInsBillingProv);
			this.groupBox4.Controls.Add(this.radioInsBillingProvSpecific);
			this.groupBox4.Controls.Add(this.radioInsBillingProvTreat);
			this.groupBox4.Controls.Add(this.radioInsBillingProvDefault);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox4.Location = new System.Drawing.Point(742, 178);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(262, 100);
			this.groupBox4.TabIndex = 19;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Default Insurance Billing Provider";
			// 
			// butPickInsBillingProv
			// 
			this.butPickInsBillingProv.Location = new System.Drawing.Point(231, 73);
			this.butPickInsBillingProv.Name = "butPickInsBillingProv";
			this.butPickInsBillingProv.Size = new System.Drawing.Size(23, 21);
			this.butPickInsBillingProv.TabIndex = 4;
			this.butPickInsBillingProv.Text = "...";
			this.butPickInsBillingProv.Click += new System.EventHandler(this.butPickInsBillingProv_Click);
			// 
			// comboInsBillingProv
			// 
			this.comboInsBillingProv.Location = new System.Drawing.Point(13, 73);
			this.comboInsBillingProv.Name = "comboInsBillingProv";
			this.comboInsBillingProv.Size = new System.Drawing.Size(216, 21);
			this.comboInsBillingProv.TabIndex = 3;
			// 
			// radioInsBillingProvSpecific
			// 
			this.radioInsBillingProvSpecific.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvSpecific.Location = new System.Drawing.Point(13, 53);
			this.radioInsBillingProvSpecific.Name = "radioInsBillingProvSpecific";
			this.radioInsBillingProvSpecific.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvSpecific.TabIndex = 2;
			this.radioInsBillingProvSpecific.Text = "Specific Provider:";
			// 
			// radioInsBillingProvTreat
			// 
			this.radioInsBillingProvTreat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvTreat.Location = new System.Drawing.Point(13, 35);
			this.radioInsBillingProvTreat.Name = "radioInsBillingProvTreat";
			this.radioInsBillingProvTreat.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvTreat.TabIndex = 1;
			this.radioInsBillingProvTreat.Text = "Treating Provider";
			// 
			// radioInsBillingProvDefault
			// 
			this.radioInsBillingProvDefault.Checked = true;
			this.radioInsBillingProvDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvDefault.Location = new System.Drawing.Point(13, 17);
			this.radioInsBillingProvDefault.Name = "radioInsBillingProvDefault";
			this.radioInsBillingProvDefault.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvDefault.TabIndex = 0;
			this.radioInsBillingProvDefault.TabStop = true;
			this.radioInsBillingProvDefault.Text = "Default Practice Provider";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(55, 232);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(168, 17);
			this.label10.TabIndex = 0;
			this.label10.Text = "Email Address";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboPlaceService
			// 
			this.comboPlaceService.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlaceService.Location = new System.Drawing.Point(755, 279);
			this.comboPlaceService.MaxDropDownItems = 30;
			this.comboPlaceService.Name = "comboPlaceService";
			this.comboPlaceService.Size = new System.Drawing.Size(216, 21);
			this.comboPlaceService.TabIndex = 20;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(574, 280);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(180, 17);
			this.label7.TabIndex = 0;
			this.label7.Text = "Default Proc Place of Service";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.BackColor = System.Drawing.SystemColors.Window;
			this.textEmail.Location = new System.Drawing.Point(225, 230);
			this.textEmail.MaxLength = 255;
			this.textEmail.Name = "textEmail";
			this.textEmail.ReadOnly = true;
			this.textEmail.Size = new System.Drawing.Size(266, 20);
			this.textEmail.TabIndex = 10;
			// 
			// textBankNumber
			// 
			this.textBankNumber.Location = new System.Drawing.Point(225, 251);
			this.textBankNumber.MaxLength = 255;
			this.textBankNumber.Name = "textBankNumber";
			this.textBankNumber.Size = new System.Drawing.Size(291, 20);
			this.textBankNumber.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(73, 252);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(151, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Bank Account Number";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(225, 135);
			this.textPhone.MaxLength = 255;
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(157, 20);
			this.textPhone.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(14, 138);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(210, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Phone";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(225, 114);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(263, 20);
			this.textDescription.TabIndex = 4;
			// 
			// butEmail
			// 
			this.butEmail.Location = new System.Drawing.Point(493, 230);
			this.butEmail.Name = "butEmail";
			this.butEmail.Size = new System.Drawing.Size(24, 21);
			this.butEmail.TabIndex = 11;
			this.butEmail.Text = "...";
			this.butEmail.Click += new System.EventHandler(this.butEmail_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(903, 417);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 26;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(984, 417);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 27;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 115);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(207, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(385, 156);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(144, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "(###)###-####";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textExternalID
			// 
			this.textExternalID.Location = new System.Drawing.Point(225, 72);
			this.textExternalID.Name = "textExternalID";
			this.textExternalID.Size = new System.Drawing.Size(157, 20);
			this.textExternalID.TabIndex = 2;
			// 
			// labelExternalID
			// 
			this.labelExternalID.Location = new System.Drawing.Point(17, 72);
			this.labelExternalID.Name = "labelExternalID";
			this.labelExternalID.Size = new System.Drawing.Size(207, 17);
			this.labelExternalID.TabIndex = 0;
			this.labelExternalID.Text = "External ID";
			this.labelExternalID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEmailNone
			// 
			this.butEmailNone.Enabled = false;
			this.butEmailNone.Location = new System.Drawing.Point(520, 230);
			this.butEmailNone.Name = "butEmailNone";
			this.butEmailNone.Size = new System.Drawing.Size(48, 21);
			this.butEmailNone.TabIndex = 12;
			this.butEmailNone.Text = "None";
			this.butEmailNone.UseVisualStyleBackColor = true;
			this.butEmailNone.Click += new System.EventHandler(this.buttDetachEmail_Click);
			// 
			// textSchedRules
			// 
			this.textSchedRules.Location = new System.Drawing.Point(225, 293);
			this.textSchedRules.MaxLength = 255;
			this.textSchedRules.Multiline = true;
			this.textSchedRules.Name = "textSchedRules";
			this.textSchedRules.Size = new System.Drawing.Size(291, 60);
			this.textSchedRules.TabIndex = 15;
			// 
			// labelSchedRules
			// 
			this.labelSchedRules.Location = new System.Drawing.Point(26, 295);
			this.labelSchedRules.Name = "labelSchedRules";
			this.labelSchedRules.Size = new System.Drawing.Size(198, 14);
			this.labelSchedRules.TabIndex = 266;
			this.labelSchedRules.Text = "Scheduling Note";
			this.labelSchedRules.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkProcCodeRequired
			// 
			this.checkProcCodeRequired.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkProcCodeRequired.Enabled = false;
			this.checkProcCodeRequired.Location = new System.Drawing.Point(7, 214);
			this.checkProcCodeRequired.Name = "checkProcCodeRequired";
			this.checkProcCodeRequired.Size = new System.Drawing.Size(232, 16);
			this.checkProcCodeRequired.TabIndex = 9;
			this.checkProcCodeRequired.Text = "Proc code required on Rx from this clinic";
			this.checkProcCodeRequired.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(17, 371);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(207, 19);
			this.label24.TabIndex = 267;
			this.label24.Text = "This will not change any DateTimes";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDefaultBillingType
			// 
			this.comboDefaultBillingType.Location = new System.Drawing.Point(755, 323);
			this.comboDefaultBillingType.Name = "comboDefaultBillingType";
			this.comboDefaultBillingType.Size = new System.Drawing.Size(216, 21);
			this.comboDefaultBillingType.TabIndex = 24;
			// 
			// labelDefaultBillingType
			// 
			this.labelDefaultBillingType.Location = new System.Drawing.Point(573, 325);
			this.labelDefaultBillingType.Name = "labelDefaultBillingType";
			this.labelDefaultBillingType.Size = new System.Drawing.Size(180, 17);
			this.labelDefaultBillingType.TabIndex = 269;
			this.labelDefaultBillingType.Text = "Default Billing Type";
			this.labelDefaultBillingType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAlwaysAssignBenToPatient
			// 
			this.checkAlwaysAssignBenToPatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAlwaysAssignBenToPatient.Location = new System.Drawing.Point(548, 345);
			this.checkAlwaysAssignBenToPatient.Name = "checkAlwaysAssignBenToPatient";
			this.checkAlwaysAssignBenToPatient.Size = new System.Drawing.Size(221, 17);
			this.checkAlwaysAssignBenToPatient.TabIndex = 25;
			this.checkAlwaysAssignBenToPatient.Text = "Always Assign Benefits to the Patient";
			this.checkAlwaysAssignBenToPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label25
			// 
			this.label25.Location = new System.Drawing.Point(771, 343);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(180, 17);
			this.label25.TabIndex = 273;
			this.label25.Text = "(ignore the Ins Plan setting)";
			this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormClinicEdit
			// 
			this.ClientSize = new System.Drawing.Size(1071, 453);
			this.Controls.Add(this.checkAlwaysAssignBenToPatient);
			this.Controls.Add(this.labelDefaultBillingType);
			this.Controls.Add(this.comboDefaultBillingType);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.labelTimeZone);
			this.Controls.Add(this.comboBoxTimeZone);
			this.Controls.Add(this.checkProcCodeRequired);
			this.Controls.Add(this.textSchedRules);
			this.Controls.Add(this.labelSchedRules);
			this.Controls.Add(this.butEmailNone);
			this.Controls.Add(this.textExternalID);
			this.Controls.Add(this.labelExternalID);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.textMedLabAcctNum);
			this.Controls.Add(this.labelMedLabAcctNum);
			this.Controls.Add(this.textClinicAbbr);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.checkExcludeFromInsVerifyList);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.comboRegion);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.textClinicNum);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.checkIsMedicalOnly);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.butPickDefaultProv);
			this.Controls.Add(this.comboDefaultProvider);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.textFax);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.comboPlaceService);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.textBankNumber);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butEmail);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label25);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClinicEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Clinic";
			this.Load += new System.EventHandler(this.FormClinicEdit_Load);
			this.tabControl1.ResumeLayout(false);
			this.PhysicalAddress.ResumeLayout(false);
			this.PhysicalAddress.PerformLayout();
			this.BillingAddress.ResumeLayout(false);
			this.BillingAddress.PerformLayout();
			this.PayToAddress.ResumeLayout(false);
			this.PayToAddress.PerformLayout();
			this.tabSpecialty.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textDescription;
		private ValidPhone textPhone;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBankNumber;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ComboBox comboPlaceService;
		private GroupBox groupBox4;
		private UI.ComboBoxOD comboInsBillingProv;
		private RadioButton radioInsBillingProvSpecific;
		private RadioButton radioInsBillingProvTreat;
		private RadioButton radioInsBillingProvDefault;
		private ValidPhone textFax;
		private Label label8;
		private Label label9;
		private Label label10;
		private TextBox textEmail;
		private UI.Button butNone;
		private CheckBox checkIsMedicalOnly;
		private TextBox textCity;
		private TextBox textState;
		private TextBox textZip;
		private TextBox textAddress2;
		private Label label11;
		private Label label4;
		private TextBox textAddress;
		private Label label3;
		private Label label17;
		private Label label13;
		private TextBox textPayToZip;
		private TextBox textPayToST;
		private TextBox textPayToCity;
		private TextBox textPayToAddress2;
		private TextBox textPayToAddress;
		private Label label14;
		private Label label15;
		private Label label18;
		private Label label16;
		private TextBox textBillingZip;
		private TextBox textBillingST;
		private TextBox textBillingCity;
		private TextBox textBillingAddress2;
		private TextBox textBillingAddress;
		private Label label19;
		private Label label20;
		private TextBox textClinicNum;
		private Label label21;
		private CheckBox checkUseBillingAddressOnClaims;
		private Label label22;
		private ComboBox comboRegion;
		private TabControl tabControl1;
		private TabPage PhysicalAddress;
		private TabPage BillingAddress;
		private TabPage PayToAddress;
		private CheckBox checkExcludeFromInsVerifyList;
		private TextBox textClinicAbbr;
		private Label label23;
		private TextBox textMedLabAcctNum;
		private Label labelMedLabAcctNum;
		private CheckBox checkHidden;
		private TextBox textExternalID;
		private Label labelExternalID;
		private UI.Button butEmailNone;
		private TabPage tabSpecialty;
		private UI.Button butAdd;
		private UI.GridOD gridSpecialty;
		private UI.Button butRemove;
		private TextBox textSchedRules;
		private Label labelSchedRules;
		private Label label12;
		private UI.ComboBoxOD comboDefaultProvider;
		private UI.Button butPickDefaultProv;
		private UI.Button butEmail;
		private UI.Button butPickInsBillingProv;
		private CheckBox checkProcCodeRequired;
		private Label labelTimeZone;
		private OpenDental.UI.ComboBoxOD comboBoxTimeZone;
		private Label label24;
		private UI.ComboBoxOD comboDefaultBillingType;
		private Label labelDefaultBillingType;
		private CheckBox checkAlwaysAssignBenToPatient;
		private Label label25;
	}
}
