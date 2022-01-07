using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPractice {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPractice));
			this.listBillType = new OpenDental.UI.ListBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textBankNumber = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textZip = new System.Windows.Forms.TextBox();
			this.textST = new System.Windows.Forms.TextBox();
			this.textCity = new System.Windows.Forms.TextBox();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textPracticeTitle = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelPlaceService = new System.Windows.Forms.Label();
			this.listPlaceService = new OpenDental.UI.ListBoxOD();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.comboInsBillingProv = new OpenDental.UI.ComboBoxOD();
			this.radioInsBillingProvSpecific = new System.Windows.Forms.RadioButton();
			this.radioInsBillingProvTreat = new System.Windows.Forms.RadioButton();
			this.radioInsBillingProvDefault = new System.Windows.Forms.RadioButton();
			this.groupSwiss = new System.Windows.Forms.GroupBox();
			this.textBankAddress = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textBankRouting = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label18 = new System.Windows.Forms.Label();
			this.checkUseBillingAddressOnClaims = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textBillingZip = new System.Windows.Forms.TextBox();
			this.textBillingST = new System.Windows.Forms.TextBox();
			this.textBillingCity = new System.Windows.Forms.TextBox();
			this.textBillingAddress2 = new System.Windows.Forms.TextBox();
			this.textBillingAddress = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.textPayToZip = new System.Windows.Forms.TextBox();
			this.textPayToST = new System.Windows.Forms.TextBox();
			this.textPayToCity = new System.Windows.Forms.TextBox();
			this.textPayToAddress2 = new System.Windows.Forms.TextBox();
			this.textPayToAddress = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.checkIsMedicalOnly = new System.Windows.Forms.CheckBox();
			this.textFax = new OpenDental.ValidPhone();
			this.textPhone = new OpenDental.ValidPhone();
			this.label19 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.labelBillingPhone = new System.Windows.Forms.Label();
			this.textBillingPhone = new OpenDental.ValidPhone();
			this.labelPayToPhone = new System.Windows.Forms.Label();
			this.textPayToPhone = new OpenDental.ValidPhone();
			this.groupBox2.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupSwiss.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// listBillType
			// 
			this.listBillType.ItemStrings = new string[] {
        ""};
			this.listBillType.Location = new System.Drawing.Point(471, 28);
			this.listBillType.Name = "listBillType";
			this.listBillType.Size = new System.Drawing.Size(144, 147);
			this.listBillType.TabIndex = 12;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(470, 8);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(154, 17);
			this.label12.TabIndex = 0;
			this.label12.Text = "Default Billing Type";
			this.label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(643, 132);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(110, 16);
			this.label10.TabIndex = 0;
			this.label10.Text = "Default Provider";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textBankNumber
			// 
			this.textBankNumber.Location = new System.Drawing.Point(122, 514);
			this.textBankNumber.Multiline = true;
			this.textBankNumber.Name = "textBankNumber";
			this.textBankNumber.Size = new System.Drawing.Size(317, 49);
			this.textBankNumber.TabIndex = 7;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(4, 513);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 31);
			this.label4.TabIndex = 0;
			this.label4.Text = "Bank Deposit Acct Number and Info";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label16);
			this.groupBox2.Controls.Add(this.textZip);
			this.groupBox2.Controls.Add(this.textST);
			this.groupBox2.Controls.Add(this.textCity);
			this.groupBox2.Controls.Add(this.textAddress2);
			this.groupBox2.Controls.Add(this.textAddress);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(19, 106);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(429, 87);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Physical Treating Address";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(5, 36);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(97, 16);
			this.label16.TabIndex = 0;
			this.label16.Text = "Address 2";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(318, 57);
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(102, 20);
			this.textZip.TabIndex = 5;
			// 
			// textST
			// 
			this.textST.Location = new System.Drawing.Point(264, 57);
			this.textST.Name = "textST";
			this.textST.Size = new System.Drawing.Size(52, 20);
			this.textST.TabIndex = 4;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(103, 57);
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(159, 20);
			this.textCity.TabIndex = 3;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(103, 35);
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(317, 20);
			this.textAddress2.TabIndex = 2;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(103, 13);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(317, 20);
			this.textAddress.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(4, 15);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(98, 14);
			this.label5.TabIndex = 0;
			this.label5.Text = "Address";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(4, 59);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(98, 15);
			this.label6.TabIndex = 0;
			this.label6.Text = "City, ST, Zip";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPracticeTitle
			// 
			this.textPracticeTitle.Location = new System.Drawing.Point(122, 39);
			this.textPracticeTitle.Name = "textPracticeTitle";
			this.textPracticeTitle.Size = new System.Drawing.Size(317, 20);
			this.textPracticeTitle.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(28, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(96, 28);
			this.label3.TabIndex = 0;
			this.label3.Text = "Provider Name or Practice Title";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelPlaceService
			// 
			this.labelPlaceService.Location = new System.Drawing.Point(468, 186);
			this.labelPlaceService.Name = "labelPlaceService";
			this.labelPlaceService.Size = new System.Drawing.Size(156, 18);
			this.labelPlaceService.TabIndex = 0;
			this.labelPlaceService.Text = "Default Proc Place Service";
			this.labelPlaceService.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPlaceService
			// 
			this.listPlaceService.Location = new System.Drawing.Point(470, 207);
			this.listPlaceService.Name = "listPlaceService";
			this.listPlaceService.Size = new System.Drawing.Size(145, 160);
			this.listPlaceService.TabIndex = 13;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.comboInsBillingProv);
			this.groupBox4.Controls.Add(this.radioInsBillingProvSpecific);
			this.groupBox4.Controls.Add(this.radioInsBillingProvTreat);
			this.groupBox4.Controls.Add(this.radioInsBillingProvDefault);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox4.Location = new System.Drawing.Point(627, 201);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(235, 104);
			this.groupBox4.TabIndex = 14;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Default Insurance Billing Provider";
			// 
			// comboInsBillingProv
			// 
			this.comboInsBillingProv.Location = new System.Drawing.Point(17, 73);
			this.comboInsBillingProv.Name = "comboInsBillingProv";
			this.comboInsBillingProv.Size = new System.Drawing.Size(212, 21);
			this.comboInsBillingProv.TabIndex = 4;
			// 
			// radioInsBillingProvSpecific
			// 
			this.radioInsBillingProvSpecific.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvSpecific.Location = new System.Drawing.Point(17, 53);
			this.radioInsBillingProvSpecific.Name = "radioInsBillingProvSpecific";
			this.radioInsBillingProvSpecific.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvSpecific.TabIndex = 3;
			this.radioInsBillingProvSpecific.Text = "Specific Provider:";
			// 
			// radioInsBillingProvTreat
			// 
			this.radioInsBillingProvTreat.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvTreat.Location = new System.Drawing.Point(17, 34);
			this.radioInsBillingProvTreat.Name = "radioInsBillingProvTreat";
			this.radioInsBillingProvTreat.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvTreat.TabIndex = 2;
			this.radioInsBillingProvTreat.Text = "Treating Provider";
			// 
			// radioInsBillingProvDefault
			// 
			this.radioInsBillingProvDefault.Checked = true;
			this.radioInsBillingProvDefault.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioInsBillingProvDefault.Location = new System.Drawing.Point(17, 16);
			this.radioInsBillingProvDefault.Name = "radioInsBillingProvDefault";
			this.radioInsBillingProvDefault.Size = new System.Drawing.Size(186, 19);
			this.radioInsBillingProvDefault.TabIndex = 1;
			this.radioInsBillingProvDefault.TabStop = true;
			this.radioInsBillingProvDefault.Text = "Default Practice Provider";
			// 
			// groupSwiss
			// 
			this.groupSwiss.Controls.Add(this.textBankAddress);
			this.groupSwiss.Controls.Add(this.label2);
			this.groupSwiss.Controls.Add(this.textBankRouting);
			this.groupSwiss.Controls.Add(this.label1);
			this.groupSwiss.Location = new System.Drawing.Point(470, 382);
			this.groupSwiss.Name = "groupSwiss";
			this.groupSwiss.Size = new System.Drawing.Size(392, 146);
			this.groupSwiss.TabIndex = 8;
			this.groupSwiss.TabStop = false;
			this.groupSwiss.Text = "Switzerland";
			// 
			// textBankAddress
			// 
			this.textBankAddress.AcceptsReturn = true;
			this.textBankAddress.Location = new System.Drawing.Point(103, 43);
			this.textBankAddress.Multiline = true;
			this.textBankAddress.Name = "textBankAddress";
			this.textBankAddress.Size = new System.Drawing.Size(283, 95);
			this.textBankAddress.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(98, 40);
			this.label2.TabIndex = 0;
			this.label2.Text = "Bank Name and Address";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textBankRouting
			// 
			this.textBankRouting.Location = new System.Drawing.Point(103, 19);
			this.textBankRouting.Name = "textBankRouting";
			this.textBankRouting.Size = new System.Drawing.Size(283, 20);
			this.textBankRouting.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(98, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Bank Routing";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBillingPhone);
			this.groupBox1.Controls.Add(this.labelBillingPhone);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.checkUseBillingAddressOnClaims);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.textBillingZip);
			this.groupBox1.Controls.Add(this.textBillingST);
			this.groupBox1.Controls.Add(this.textBillingCity);
			this.groupBox1.Controls.Add(this.textBillingAddress2);
			this.groupBox1.Controls.Add(this.textBillingAddress);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(19, 196);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(429, 160);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Billing Address";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(100, 14);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(310, 29);
			this.label18.TabIndex = 0;
			this.label18.Text = "Optional.  Cannot be a PO Box if Use on Claims is checked.  Also overrides the pr" +
    "actice address on EHG statements.";
			// 
			// checkUseBillingAddressOnClaims
			// 
			this.checkUseBillingAddressOnClaims.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseBillingAddressOnClaims.Location = new System.Drawing.Point(6, 46);
			this.checkUseBillingAddressOnClaims.Name = "checkUseBillingAddressOnClaims";
			this.checkUseBillingAddressOnClaims.Size = new System.Drawing.Size(111, 16);
			this.checkUseBillingAddressOnClaims.TabIndex = 1;
			this.checkUseBillingAddressOnClaims.Text = "Use on Claims";
			this.checkUseBillingAddressOnClaims.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseBillingAddressOnClaims.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(4, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(97, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "Address 2";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingZip
			// 
			this.textBillingZip.Location = new System.Drawing.Point(318, 109);
			this.textBillingZip.Name = "textBillingZip";
			this.textBillingZip.Size = new System.Drawing.Size(102, 20);
			this.textBillingZip.TabIndex = 6;
			// 
			// textBillingST
			// 
			this.textBillingST.Location = new System.Drawing.Point(264, 109);
			this.textBillingST.Name = "textBillingST";
			this.textBillingST.Size = new System.Drawing.Size(52, 20);
			this.textBillingST.TabIndex = 5;
			// 
			// textBillingCity
			// 
			this.textBillingCity.Location = new System.Drawing.Point(103, 109);
			this.textBillingCity.Name = "textBillingCity";
			this.textBillingCity.Size = new System.Drawing.Size(159, 20);
			this.textBillingCity.TabIndex = 4;
			// 
			// textBillingAddress2
			// 
			this.textBillingAddress2.Location = new System.Drawing.Point(103, 87);
			this.textBillingAddress2.Name = "textBillingAddress2";
			this.textBillingAddress2.Size = new System.Drawing.Size(317, 20);
			this.textBillingAddress2.TabIndex = 3;
			// 
			// textBillingAddress
			// 
			this.textBillingAddress.Location = new System.Drawing.Point(103, 65);
			this.textBillingAddress.Name = "textBillingAddress";
			this.textBillingAddress.Size = new System.Drawing.Size(317, 20);
			this.textBillingAddress.TabIndex = 2;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(3, 67);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(98, 14);
			this.label8.TabIndex = 0;
			this.label8.Text = "Address";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(3, 111);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(98, 15);
			this.label11.TabIndex = 0;
			this.label11.Text = "City, ST, Zip";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textPayToPhone);
			this.groupBox3.Controls.Add(this.labelPayToPhone);
			this.groupBox3.Controls.Add(this.label17);
			this.groupBox3.Controls.Add(this.label13);
			this.groupBox3.Controls.Add(this.textPayToZip);
			this.groupBox3.Controls.Add(this.textPayToST);
			this.groupBox3.Controls.Add(this.textPayToCity);
			this.groupBox3.Controls.Add(this.textPayToAddress2);
			this.groupBox3.Controls.Add(this.textPayToAddress);
			this.groupBox3.Controls.Add(this.label14);
			this.groupBox3.Controls.Add(this.label15);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(19, 359);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(429, 142);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Pay To Address";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(103, 14);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(317, 33);
			this.label17.TabIndex = 0;
			this.label17.Text = "Optional for claims.  Can be a PO Box.  Sent in addition to treating or billing a" +
    "ddress.";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(5, 70);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(97, 16);
			this.label13.TabIndex = 0;
			this.label13.Text = "Address 2";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayToZip
			// 
			this.textPayToZip.Location = new System.Drawing.Point(318, 91);
			this.textPayToZip.Name = "textPayToZip";
			this.textPayToZip.Size = new System.Drawing.Size(102, 20);
			this.textPayToZip.TabIndex = 5;
			// 
			// textPayToST
			// 
			this.textPayToST.Location = new System.Drawing.Point(264, 91);
			this.textPayToST.Name = "textPayToST";
			this.textPayToST.Size = new System.Drawing.Size(52, 20);
			this.textPayToST.TabIndex = 4;
			// 
			// textPayToCity
			// 
			this.textPayToCity.Location = new System.Drawing.Point(103, 91);
			this.textPayToCity.Name = "textPayToCity";
			this.textPayToCity.Size = new System.Drawing.Size(159, 20);
			this.textPayToCity.TabIndex = 3;
			// 
			// textPayToAddress2
			// 
			this.textPayToAddress2.Location = new System.Drawing.Point(103, 69);
			this.textPayToAddress2.Name = "textPayToAddress2";
			this.textPayToAddress2.Size = new System.Drawing.Size(317, 20);
			this.textPayToAddress2.TabIndex = 2;
			// 
			// textPayToAddress
			// 
			this.textPayToAddress.Location = new System.Drawing.Point(103, 47);
			this.textPayToAddress.Name = "textPayToAddress";
			this.textPayToAddress.Size = new System.Drawing.Size(317, 20);
			this.textPayToAddress.TabIndex = 1;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(4, 49);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(98, 14);
			this.label14.TabIndex = 0;
			this.label14.Text = "Address";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(4, 93);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(98, 15);
			this.label15.TabIndex = 0;
			this.label15.Text = "City, ST, Zip";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsMedicalOnly
			// 
			this.checkIsMedicalOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsMedicalOnly.Location = new System.Drawing.Point(7, 12);
			this.checkIsMedicalOnly.Name = "checkIsMedicalOnly";
			this.checkIsMedicalOnly.Size = new System.Drawing.Size(129, 16);
			this.checkIsMedicalOnly.TabIndex = 0;
			this.checkIsMedicalOnly.TabStop = false;
			this.checkIsMedicalOnly.Text = "Practice is Medical";
			this.checkIsMedicalOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFax
			// 
			this.textFax.IsFormattingEnabled = false;
			this.textFax.Location = new System.Drawing.Point(122, 83);
			this.textFax.Name = "textFax";
			this.textFax.Size = new System.Drawing.Size(121, 20);
			this.textFax.TabIndex = 3;
			// 
			// textPhone
			// 
			this.textPhone.IsFormattingEnabled = false;
			this.textPhone.Location = new System.Drawing.Point(122, 61);
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(121, 20);
			this.textPhone.TabIndex = 2;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(53, 84);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(68, 17);
			this.label19.TabIndex = 0;
			this.label19.Text = "Fax";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(24, 62);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(97, 17);
			this.label9.TabIndex = 0;
			this.label9.Text = "Phone";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(785, 547);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 10;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(704, 547);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 9;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(644, 152);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(212, 21);
			this.comboProv.TabIndex = 5;
			// 
			// labelBillingPhone
			// 
			this.labelBillingPhone.Location = new System.Drawing.Point(3, 134);
			this.labelBillingPhone.Name = "labelBillingPhone";
			this.labelBillingPhone.Size = new System.Drawing.Size(98, 15);
			this.labelBillingPhone.TabIndex = 0;
			this.labelBillingPhone.Text = "Phone";
			this.labelBillingPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBillingPhone
			// 
			this.textBillingPhone.IsFormattingEnabled = false;
			this.textBillingPhone.Location = new System.Drawing.Point(103, 131);
			this.textBillingPhone.Name = "textBillingPhone";
			this.textBillingPhone.Size = new System.Drawing.Size(121, 20);
			this.textBillingPhone.TabIndex = 7;
			// 
			// labelPayToPhone
			// 
			this.labelPayToPhone.Location = new System.Drawing.Point(4, 115);
			this.labelPayToPhone.Name = "labelPayToPhone";
			this.labelPayToPhone.Size = new System.Drawing.Size(98, 15);
			this.labelPayToPhone.TabIndex = 0;
			this.labelPayToPhone.Text = "Phone";
			this.labelPayToPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPayToPhone
			// 
			this.textPayToPhone.IsFormattingEnabled = false;
			this.textPayToPhone.Location = new System.Drawing.Point(103, 113);
			this.textPayToPhone.Name = "textPayToPhone";
			this.textPayToPhone.Size = new System.Drawing.Size(121, 20);
			this.textPayToPhone.TabIndex = 6;
			// 
			// FormPractice
			// 
			this.ClientSize = new System.Drawing.Size(892, 596);
			this.Controls.Add(this.comboProv);
			this.Controls.Add(this.textFax);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.checkIsMedicalOnly);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupSwiss);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.listPlaceService);
			this.Controls.Add(this.labelPlaceService);
			this.Controls.Add(this.textBankNumber);
			this.Controls.Add(this.textPracticeTitle);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.listBillType);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label3);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPractice";
			this.ShowInTaskbar = false;
			this.Text = "Edit Practice Info";
			this.Load += new System.EventHandler(this.FormPractice_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupSwiss.ResumeLayout(false);
			this.groupSwiss.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox textBankNumber;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.TextBox textST;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.TextBox textAddress2;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textPracticeTitle;
		private System.Windows.Forms.Label label3;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listBillType;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label labelPlaceService;
		private OpenDental.UI.ListBoxOD listPlaceService;
		private System.Windows.Forms.RadioButton radioInsBillingProvTreat;
		private System.Windows.Forms.RadioButton radioInsBillingProvDefault;
		private System.Windows.Forms.RadioButton radioInsBillingProvSpecific;
		private UI.ComboBoxOD comboInsBillingProv;
		private GroupBox groupSwiss;
		private TextBox textBankAddress;
		private Label label2;
		private TextBox textBankRouting;
		private Label label1;
		private GroupBox groupBox1;
		private Label label7;
		private TextBox textBillingZip;
		private TextBox textBillingST;
		private TextBox textBillingCity;
		private TextBox textBillingAddress2;
		private TextBox textBillingAddress;
		private Label label8;
		private Label label11;
		private CheckBox checkUseBillingAddressOnClaims;
		private GroupBox groupBox3;
		private Label label13;
		private TextBox textPayToZip;
		private TextBox textPayToST;
		private TextBox textPayToCity;
		private TextBox textPayToAddress2;
		private TextBox textPayToAddress;
		private Label label14;
		private Label label15;
		private Label label18;
		private Label label17;
		private CheckBox checkIsMedicalOnly;
		private ValidPhone textFax;//Auto formatting turned off on purpose.  See validation.
		private ValidPhone textPhone;//Auto formatting turned off on purpose.  See validation.
		private Label label19;
		private Label label9;
		private System.Windows.Forms.GroupBox groupBox4;// Required designer variable.
		private UI.ComboBoxOD comboProv;
		private ValidPhone textBillingPhone;
		private Label labelBillingPhone;
		private ValidPhone textPayToPhone;
		private Label labelPayToPhone;
	}
}
