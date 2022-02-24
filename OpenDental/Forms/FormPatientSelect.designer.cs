using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPatientSelect {
		private System.ComponentModel.IContainer components=null;

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if (components != null){
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientSelect));
			this.textLName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupAddPt = new System.Windows.Forms.GroupBox();
			this.butAddAll = new OpenDental.UI.Button();
			this.butAddPt = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.butOnScreenKeyboard = new OpenDental.UI.Button();
			this.textInvoiceNumber = new System.Windows.Forms.TextBox();
			this.labelInvoiceNumber = new System.Windows.Forms.Label();
			this.checkShowMerged = new System.Windows.Forms.CheckBox();
			this.textRegKey = new System.Windows.Forms.TextBox();
			this.labelRegKey = new System.Windows.Forms.Label();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.textCountry = new System.Windows.Forms.TextBox();
			this.labelCountry = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textSubscriberID = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.comboSite = new System.Windows.Forms.ComboBox();
			this.labelSite = new System.Windows.Forms.Label();
			this.comboBillingType = new System.Windows.Forms.ComboBox();
			this.textBirthdate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkShowArchived = new System.Windows.Forms.CheckBox();
			this.textChartNumber = new System.Windows.Forms.TextBox();
			this.textSSN = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textPatNum = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textState = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.checkGuarantors = new System.Windows.Forms.CheckBox();
			this.checkShowInactive = new System.Windows.Forms.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.label4 = new System.Windows.Forms.Label();
			this.textFName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.checkRefresh = new System.Windows.Forms.CheckBox();
			this.butGetAll = new OpenDental.UI.Button();
			this.butSearch = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.timerFillGrid = new System.Windows.Forms.Timer(this.components);
			this.labelMatchingRecords = new System.Windows.Forms.Label();
			this.groupAddPt.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// textLName
			// 
			this.textLName.Location = new System.Drawing.Point(166, 55);
			this.textLName.Name = "textLName";
			this.textLName.Size = new System.Drawing.Size(90, 20);
			this.textLName.TabIndex = 0;
			this.textLName.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textLName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(154, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "Last Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupAddPt
			// 
			this.groupAddPt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAddPt.Controls.Add(this.butAddAll);
			this.groupAddPt.Controls.Add(this.butAddPt);
			this.groupAddPt.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupAddPt.Location = new System.Drawing.Point(672, 562);
			this.groupAddPt.Name = "groupAddPt";
			this.groupAddPt.Size = new System.Drawing.Size(262, 45);
			this.groupAddPt.TabIndex = 2;
			this.groupAddPt.TabStop = false;
			this.groupAddPt.Text = "Add New Family:";
			// 
			// butAddAll
			// 
			this.butAddAll.Location = new System.Drawing.Point(148, 16);
			this.butAddAll.Name = "butAddAll";
			this.butAddAll.Size = new System.Drawing.Size(75, 23);
			this.butAddAll.TabIndex = 1;
			this.butAddAll.Text = "Add Many";
			this.butAddAll.Click += new System.EventHandler(this.butAddAll_Click);
			// 
			// butAddPt
			// 
			this.butAddPt.Location = new System.Drawing.Point(42, 16);
			this.butAddPt.Name = "butAddPt";
			this.butAddPt.Size = new System.Drawing.Size(75, 23);
			this.butAddPt.TabIndex = 0;
			this.butAddPt.Text = "&Add Pt";
			this.butAddPt.Click += new System.EventHandler(this.butAddPt_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(775, 667);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 26);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(857, 667);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 26);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.butOnScreenKeyboard);
			this.groupBox2.Controls.Add(this.textInvoiceNumber);
			this.groupBox2.Controls.Add(this.labelInvoiceNumber);
			this.groupBox2.Controls.Add(this.checkShowMerged);
			this.groupBox2.Controls.Add(this.textRegKey);
			this.groupBox2.Controls.Add(this.labelRegKey);
			this.groupBox2.Controls.Add(this.comboClinic);
			this.groupBox2.Controls.Add(this.textCountry);
			this.groupBox2.Controls.Add(this.labelCountry);
			this.groupBox2.Controls.Add(this.textEmail);
			this.groupBox2.Controls.Add(this.labelEmail);
			this.groupBox2.Controls.Add(this.textSubscriberID);
			this.groupBox2.Controls.Add(this.label13);
			this.groupBox2.Controls.Add(this.comboSite);
			this.groupBox2.Controls.Add(this.labelSite);
			this.groupBox2.Controls.Add(this.comboBillingType);
			this.groupBox2.Controls.Add(this.textBirthdate);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.checkShowArchived);
			this.groupBox2.Controls.Add(this.textChartNumber);
			this.groupBox2.Controls.Add(this.textSSN);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.textPatNum);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.textState);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.textCity);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.checkGuarantors);
			this.groupBox2.Controls.Add(this.checkShowInactive);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.textAddress);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.textPhone);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.textFName);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.textLName);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(672, 2);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(262, 499);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Search by:";
			// 
			// butOnScreenKeyboard
			// 
			this.butOnScreenKeyboard.Location = new System.Drawing.Point(166, 10);
			this.butOnScreenKeyboard.Name = "butOnScreenKeyboard";
			this.butOnScreenKeyboard.Size = new System.Drawing.Size(90, 23);
			this.butOnScreenKeyboard.TabIndex = 54;
			this.butOnScreenKeyboard.Text = "Keyboard";
			this.butOnScreenKeyboard.UseVisualStyleBackColor = true;
			this.butOnScreenKeyboard.Click += new System.EventHandler(this.butOnScreenKeyboard_Click);
			// 
			// textInvoiceNumber
			// 
			this.textInvoiceNumber.Location = new System.Drawing.Point(166, 295);
			this.textInvoiceNumber.Name = "textInvoiceNumber";
			this.textInvoiceNumber.Size = new System.Drawing.Size(90, 20);
			this.textInvoiceNumber.TabIndex = 12;
			this.textInvoiceNumber.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textInvoiceNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// labelInvoiceNumber
			// 
			this.labelInvoiceNumber.Location = new System.Drawing.Point(11, 296);
			this.labelInvoiceNumber.Name = "labelInvoiceNumber";
			this.labelInvoiceNumber.Size = new System.Drawing.Size(156, 17);
			this.labelInvoiceNumber.TabIndex = 53;
			this.labelInvoiceNumber.Text = "Invoice Number";
			this.labelInvoiceNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowMerged
			// 
			this.checkShowMerged.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowMerged.Location = new System.Drawing.Point(11, 477);
			this.checkShowMerged.Name = "checkShowMerged";
			this.checkShowMerged.Size = new System.Drawing.Size(236, 17);
			this.checkShowMerged.TabIndex = 21;
			this.checkShowMerged.Text = "Show Merged Patients";
			this.checkShowMerged.Visible = false;
			this.checkShowMerged.CheckedChanged += new System.EventHandler(this.OnDataEntered);
			// 
			// textRegKey
			// 
			this.textRegKey.Location = new System.Drawing.Point(166, 335);
			this.textRegKey.Name = "textRegKey";
			this.textRegKey.Size = new System.Drawing.Size(90, 20);
			this.textRegKey.TabIndex = 14;
			this.textRegKey.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textRegKey.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// labelRegKey
			// 
			this.labelRegKey.Location = new System.Drawing.Point(11, 336);
			this.labelRegKey.Name = "labelRegKey";
			this.labelRegKey.Size = new System.Drawing.Size(156, 17);
			this.labelRegKey.TabIndex = 50;
			this.labelRegKey.Text = "RegKey";
			this.labelRegKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboClinic
			// 
			this.comboClinic.IncludeAll = true;
			this.comboClinic.Location = new System.Drawing.Point(61, 398);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(195, 21);
			this.comboClinic.TabIndex = 17;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.OnDataEntered);
			// 
			// textCountry
			// 
			this.textCountry.Location = new System.Drawing.Point(166, 315);
			this.textCountry.Name = "textCountry";
			this.textCountry.Size = new System.Drawing.Size(90, 20);
			this.textCountry.TabIndex = 13;
			this.textCountry.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textCountry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// labelCountry
			// 
			this.labelCountry.Location = new System.Drawing.Point(11, 316);
			this.labelCountry.Name = "labelCountry";
			this.labelCountry.Size = new System.Drawing.Size(156, 17);
			this.labelCountry.TabIndex = 46;
			this.labelCountry.Text = "Country";
			this.labelCountry.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(166, 275);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(90, 20);
			this.textEmail.TabIndex = 11;
			this.textEmail.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textEmail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(11, 279);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(156, 12);
			this.labelEmail.TabIndex = 43;
			this.labelEmail.Text = "E-mail";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubscriberID
			// 
			this.textSubscriberID.Location = new System.Drawing.Point(166, 255);
			this.textSubscriberID.Name = "textSubscriberID";
			this.textSubscriberID.Size = new System.Drawing.Size(90, 20);
			this.textSubscriberID.TabIndex = 10;
			this.textSubscriberID.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textSubscriberID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(11, 259);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(156, 12);
			this.label13.TabIndex = 41;
			this.label13.Text = "Subscriber ID";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSite
			// 
			this.comboSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSite.Location = new System.Drawing.Point(98, 377);
			this.comboSite.MaxDropDownItems = 40;
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(158, 21);
			this.comboSite.TabIndex = 16;
			this.comboSite.SelectionChangeCommitted += new System.EventHandler(this.OnDataEntered);
			// 
			// labelSite
			// 
			this.labelSite.Location = new System.Drawing.Point(11, 381);
			this.labelSite.Name = "labelSite";
			this.labelSite.Size = new System.Drawing.Size(86, 14);
			this.labelSite.TabIndex = 38;
			this.labelSite.Text = "Site";
			this.labelSite.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBillingType
			// 
			this.comboBillingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBillingType.FormattingEnabled = true;
			this.comboBillingType.Location = new System.Drawing.Point(98, 356);
			this.comboBillingType.Name = "comboBillingType";
			this.comboBillingType.Size = new System.Drawing.Size(158, 21);
			this.comboBillingType.TabIndex = 15;
			this.comboBillingType.SelectionChangeCommitted += new System.EventHandler(this.OnDataEntered);
			// 
			// textBirthdate
			// 
			this.textBirthdate.Location = new System.Drawing.Point(166, 235);
			this.textBirthdate.Name = "textBirthdate";
			this.textBirthdate.Size = new System.Drawing.Size(90, 20);
			this.textBirthdate.TabIndex = 9;
			this.textBirthdate.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textBirthdate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(11, 239);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(156, 12);
			this.label2.TabIndex = 27;
			this.label2.Text = "Birthdate";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowArchived
			// 
			this.checkShowArchived.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowArchived.Location = new System.Drawing.Point(11, 460);
			this.checkShowArchived.Name = "checkShowArchived";
			this.checkShowArchived.Size = new System.Drawing.Size(245, 16);
			this.checkShowArchived.TabIndex = 20;
			this.checkShowArchived.Text = "Show Archived/Deceased/Hidden Clinics";
			this.checkShowArchived.CheckedChanged += new System.EventHandler(this.checkShowArchived_CheckedChanged);
			// 
			// textChartNumber
			// 
			this.textChartNumber.Location = new System.Drawing.Point(166, 215);
			this.textChartNumber.Name = "textChartNumber";
			this.textChartNumber.Size = new System.Drawing.Size(90, 20);
			this.textChartNumber.TabIndex = 8;
			this.textChartNumber.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textChartNumber.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// textSSN
			// 
			this.textSSN.Location = new System.Drawing.Point(166, 175);
			this.textSSN.Name = "textSSN";
			this.textSSN.Size = new System.Drawing.Size(90, 20);
			this.textSSN.TabIndex = 6;
			this.textSSN.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textSSN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(11, 179);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(155, 12);
			this.label12.TabIndex = 24;
			this.label12.Text = "SSN";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(11, 360);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(87, 14);
			this.label11.TabIndex = 21;
			this.label11.Text = "Billing Type";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(11, 219);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(156, 12);
			this.label10.TabIndex = 20;
			this.label10.Text = "Chart Number";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatNum
			// 
			this.textPatNum.Location = new System.Drawing.Point(166, 195);
			this.textPatNum.Name = "textPatNum";
			this.textPatNum.Size = new System.Drawing.Size(90, 20);
			this.textPatNum.TabIndex = 7;
			this.textPatNum.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textPatNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(11, 199);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(156, 12);
			this.label9.TabIndex = 18;
			this.label9.Text = "Patient Number";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(166, 155);
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(90, 20);
			this.textState.TabIndex = 5;
			this.textState.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textState.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(11, 159);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(154, 12);
			this.label8.TabIndex = 16;
			this.label8.Text = "State";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(166, 135);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(90, 20);
			this.textCity.TabIndex = 4;
			this.textCity.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textCity.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(11, 137);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(152, 14);
			this.label7.TabIndex = 14;
			this.label7.Text = "City";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkGuarantors
			// 
			this.checkGuarantors.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkGuarantors.Location = new System.Drawing.Point(11, 426);
			this.checkGuarantors.Name = "checkGuarantors";
			this.checkGuarantors.Size = new System.Drawing.Size(245, 16);
			this.checkGuarantors.TabIndex = 18;
			this.checkGuarantors.Text = "Guarantors Only";
			this.checkGuarantors.CheckedChanged += new System.EventHandler(this.OnDataEntered);
			// 
			// checkShowInactive
			// 
			this.checkShowInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowInactive.Location = new System.Drawing.Point(11, 443);
			this.checkShowInactive.Name = "checkShowInactive";
			this.checkShowInactive.Size = new System.Drawing.Size(245, 16);
			this.checkShowInactive.TabIndex = 19;
			this.checkShowInactive.Text = "Show Inactive Patients";
			this.checkShowInactive.CheckedChanged += new System.EventHandler(this.OnDataEntered);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(11, 38);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(245, 14);
			this.label6.TabIndex = 10;
			this.label6.Text = "Hint: enter values in multiple boxes.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(166, 115);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(90, 20);
			this.textAddress.TabIndex = 3;
			this.textAddress.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(11, 118);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(154, 12);
			this.label5.TabIndex = 9;
			this.label5.Text = "Address";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(166, 95);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(90, 20);
			this.textPhone.TabIndex = 2;
			this.textPhone.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textPhone.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(11, 97);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(155, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Phone (any)";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFName
			// 
			this.textFName.Location = new System.Drawing.Point(166, 75);
			this.textFName.Name = "textFName";
			this.textFName.Size = new System.Drawing.Size(90, 20);
			this.textFName.TabIndex = 1;
			this.textFName.TextChanged += new System.EventHandler(this.textbox_TextChanged);
			this.textFName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textbox_KeyDown);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(11, 79);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(154, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "First Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkRefresh);
			this.groupBox1.Controls.Add(this.butGetAll);
			this.groupBox1.Controls.Add(this.butSearch);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(672, 501);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(262, 61);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// checkRefresh
			// 
			this.checkRefresh.Location = new System.Drawing.Point(11, 41);
			this.checkRefresh.Name = "checkRefresh";
			this.checkRefresh.Size = new System.Drawing.Size(245, 18);
			this.checkRefresh.TabIndex = 72;
			this.checkRefresh.Text = "Refresh while typing";
			this.checkRefresh.UseVisualStyleBackColor = true;
			this.checkRefresh.Click += new System.EventHandler(this.checkRefresh_Click);
			// 
			// butGetAll
			// 
			this.butGetAll.Location = new System.Drawing.Point(148, 15);
			this.butGetAll.Name = "butGetAll";
			this.butGetAll.Size = new System.Drawing.Size(75, 23);
			this.butGetAll.TabIndex = 1;
			this.butGetAll.Text = "Get All";
			this.butGetAll.Click += new System.EventHandler(this.butGetAll_Click);
			// 
			// butSearch
			// 
			this.butSearch.Location = new System.Drawing.Point(42, 15);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 23);
			this.butSearch.TabIndex = 0;
			this.butSearch.Text = "&Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(3, 2);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(665, 687);
			this.gridMain.TabIndex = 9;
			this.gridMain.Title = "Select Patient";
			this.gridMain.TranslationName = "FormPatientSelect";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			this.gridMain.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gridMain_MouseDown);
			// 
			// timerFillGrid
			// 
			this.timerFillGrid.Interval = 1;
			this.timerFillGrid.Tick += new System.EventHandler(this.OnDataEntered);
			// 
			// labelMatchingRecords
			// 
			this.labelMatchingRecords.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMatchingRecords.Location = new System.Drawing.Point(693, 628);
			this.labelMatchingRecords.Name = "labelMatchingRecords";
			this.labelMatchingRecords.Size = new System.Drawing.Size(220, 17);
			this.labelMatchingRecords.TabIndex = 10;
			this.labelMatchingRecords.Text = "0 Records Displayed";
			this.labelMatchingRecords.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// FormPatientSelect
			// 
			this.AcceptButton = this.butSearch;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(944, 696);
			this.Controls.Add(this.labelMatchingRecords);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.groupAddPt);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientSelect";
			this.ShowInTaskbar = false;
			this.Text = "Select Patient";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormPatientSelect_FormClosing);
			this.Load += new System.EventHandler(this.FormSelectPatient_Load);
			this.groupAddPt.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butAddPt;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textLName;
		private System.Windows.Forms.TextBox textFName;
		private System.Windows.Forms.TextBox textAddress;
		private ValidPhone textPhone;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkShowInactive;
		private System.Windows.Forms.GroupBox groupAddPt;
		private System.Windows.Forms.CheckBox checkGuarantors;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textPatNum;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textChartNumber;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textSSN;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox1;
		private OpenDental.UI.Button butSearch;
		private OpenDental.UI.GridOD gridMain;
		private CheckBox checkShowArchived;
		private TextBox textBirthdate;
		private Label label2;
		private ComboBox comboBillingType;
		private OpenDental.UI.Button butGetAll;
		private CheckBox checkRefresh;
		private OpenDental.UI.Button butAddAll;
		private ComboBox comboSite;
		private Label labelSite;
		private TextBox textSubscriberID;
		private Label label13;
		private TextBox textEmail;
		private Label labelEmail;
		private TextBox textCountry;
		private Label labelCountry;
		private UI.ComboBoxClinicPicker comboClinic;
		private TextBox textRegKey;
		private Label labelRegKey;
		private CheckBox checkShowMerged;
		private TextBox textInvoiceNumber;
		private Label labelInvoiceNumber;
		private UI.Button butOnScreenKeyboard;
		private Timer timerFillGrid;
		private Label labelMatchingRecords;
	}
}
