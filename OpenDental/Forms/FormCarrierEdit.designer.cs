using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormCarrierEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCarrierEdit));
			this.textCarrierName = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.label3 = new System.Windows.Forms.Label();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textAddress2 = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.labelElectID = new System.Windows.Forms.Label();
			this.textElectID = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.comboPlans = new System.Windows.Forms.ComboBox();
			this.textPlans = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textCity = new System.Windows.Forms.TextBox();
			this.textState = new System.Windows.Forms.TextBox();
			this.textZip = new System.Windows.Forms.TextBox();
			this.labelCitySt = new System.Windows.Forms.Label();
			this.checkIsCDAnet = new System.Windows.Forms.CheckBox();
			this.groupCDAnet = new OpenDental.UI.GroupBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.textEncryptionMethod = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.check01 = new System.Windows.Forms.CheckBox();
			this.check03m = new System.Windows.Forms.CheckBox();
			this.check03 = new System.Windows.Forms.CheckBox();
			this.check07 = new System.Windows.Forms.CheckBox();
			this.check06 = new System.Windows.Forms.CheckBox();
			this.check04 = new System.Windows.Forms.CheckBox();
			this.check05 = new System.Windows.Forms.CheckBox();
			this.check02 = new System.Windows.Forms.CheckBox();
			this.check08 = new System.Windows.Forms.CheckBox();
			this.textModemReconcile = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.textModemSummary = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textModem = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboNetwork = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textVersion = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkIsHidden = new System.Windows.Forms.CheckBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textCarrierNum = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.labelCarrierGroupName = new System.Windows.Forms.Label();
			this.labelColor = new System.Windows.Forms.Label();
			this.comboCarrierGroupName = new OpenDental.UI.ComboBoxOD();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioBenefitSendsPat = new System.Windows.Forms.RadioButton();
			this.radioBenefitSendsIns = new System.Windows.Forms.RadioButton();
			this.labelSendElectronically = new System.Windows.Forms.Label();
			this.comboSendElectronically = new OpenDental.UI.ComboBoxOD();
			this.checkRealTimeEligibility = new System.Windows.Forms.CheckBox();
			this.odColorPickerBack = new OpenDental.UI.ODColorPicker();
			this.comboCobSendPaidByInsAt = new OpenDental.UI.ComboBoxOD();
			this.labelCobSendPaidByOtherInsAt = new System.Windows.Forms.Label();
			this.comboEraAutomation = new OpenDental.UI.ComboBoxOD();
			this.labelEraAutomation = new System.Windows.Forms.Label();
			this.comboOrthoConsolidate = new OpenDental.UI.ComboBoxOD();
			this.labelOrthoConsolidate = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupCDAnet.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// textCarrierName
			// 
			this.textCarrierName.Location = new System.Drawing.Point(179, 30);
			this.textCarrierName.MaxLength = 255;
			this.textCarrierName.Name = "textCarrierName";
			this.textCarrierName.Size = new System.Drawing.Size(226, 20);
			this.textCarrierName.TabIndex = 1;
			this.textCarrierName.TextChanged += new System.EventHandler(this.textCarrierName_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 51);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 18);
			this.label2.TabIndex = 0;
			this.label2.Text = "Phone";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(179, 50);
			this.textPhone.MaxLength = 255;
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(157, 20);
			this.textPhone.TabIndex = 2;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 71);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(171, 18);
			this.label3.TabIndex = 0;
			this.label3.Text = "Address";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAddress
			// 
			this.textAddress.Location = new System.Drawing.Point(179, 70);
			this.textAddress.MaxLength = 255;
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(291, 20);
			this.textAddress.TabIndex = 3;
			this.textAddress.TextChanged += new System.EventHandler(this.textAddress_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(171, 18);
			this.label4.TabIndex = 0;
			this.label4.Text = "Address2";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAddress2
			// 
			this.textAddress2.Location = new System.Drawing.Point(179, 90);
			this.textAddress2.MaxLength = 255;
			this.textAddress2.Name = "textAddress2";
			this.textAddress2.Size = new System.Drawing.Size(291, 20);
			this.textAddress2.TabIndex = 4;
			this.textAddress2.TextChanged += new System.EventHandler(this.textAddress2_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 31);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(171, 18);
			this.label6.TabIndex = 0;
			this.label6.Text = "Carrier Name";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelElectID
			// 
			this.labelElectID.Location = new System.Drawing.Point(6, 131);
			this.labelElectID.Name = "labelElectID";
			this.labelElectID.Size = new System.Drawing.Size(171, 18);
			this.labelElectID.TabIndex = 0;
			this.labelElectID.Text = "Electronic ID";
			this.labelElectID.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textElectID
			// 
			this.textElectID.Location = new System.Drawing.Point(179, 130);
			this.textElectID.Name = "textElectID";
			this.textElectID.Size = new System.Drawing.Size(59, 20);
			this.textElectID.TabIndex = 8;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.comboPlans);
			this.groupBox1.Controls.Add(this.textPlans);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(12, 331);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(450, 41);
			this.groupBox1.TabIndex = 17;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "In Use By";
			// 
			// comboPlans
			// 
			this.comboPlans.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlans.Location = new System.Drawing.Point(206, 14);
			this.comboPlans.MaxDropDownItems = 30;
			this.comboPlans.Name = "comboPlans";
			this.comboPlans.Size = new System.Drawing.Size(238, 21);
			this.comboPlans.TabIndex = 1;
			// 
			// textPlans
			// 
			this.textPlans.BackColor = System.Drawing.Color.White;
			this.textPlans.Location = new System.Drawing.Point(167, 14);
			this.textPlans.Name = "textPlans";
			this.textPlans.ReadOnly = true;
			this.textPlans.Size = new System.Drawing.Size(35, 20);
			this.textPlans.TabIndex = 0;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 15);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(160, 18);
			this.label9.TabIndex = 0;
			this.label9.Text = "Ins Plan Subscribers";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCity
			// 
			this.textCity.Location = new System.Drawing.Point(179, 110);
			this.textCity.MaxLength = 255;
			this.textCity.Name = "textCity";
			this.textCity.Size = new System.Drawing.Size(155, 20);
			this.textCity.TabIndex = 5;
			this.textCity.TextChanged += new System.EventHandler(this.textCity_TextChanged);
			// 
			// textState
			// 
			this.textState.Location = new System.Drawing.Point(334, 110);
			this.textState.MaxLength = 255;
			this.textState.Name = "textState";
			this.textState.Size = new System.Drawing.Size(65, 20);
			this.textState.TabIndex = 6;
			this.textState.TextChanged += new System.EventHandler(this.textState_TextChanged);
			// 
			// textZip
			// 
			this.textZip.Location = new System.Drawing.Point(399, 110);
			this.textZip.MaxLength = 255;
			this.textZip.Name = "textZip";
			this.textZip.Size = new System.Drawing.Size(71, 20);
			this.textZip.TabIndex = 7;
			// 
			// labelCitySt
			// 
			this.labelCitySt.Location = new System.Drawing.Point(6, 111);
			this.labelCitySt.Name = "labelCitySt";
			this.labelCitySt.Size = new System.Drawing.Size(171, 18);
			this.labelCitySt.TabIndex = 0;
			this.labelCitySt.Text = "City, ST, Zip";
			this.labelCitySt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsCDAnet
			// 
			this.checkIsCDAnet.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsCDAnet.Location = new System.Drawing.Point(12, 376);
			this.checkIsCDAnet.Name = "checkIsCDAnet";
			this.checkIsCDAnet.Size = new System.Drawing.Size(168, 17);
			this.checkIsCDAnet.TabIndex = 18;
			this.checkIsCDAnet.Text = "Is CDAnet Carrier";
			this.checkIsCDAnet.Click += new System.EventHandler(this.checkIsCDAnet_Click);
			// 
			// groupCDAnet
			// 
			this.groupCDAnet.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupCDAnet.Controls.Add(this.label14);
			this.groupCDAnet.Controls.Add(this.label12);
			this.groupCDAnet.Controls.Add(this.textEncryptionMethod);
			this.groupCDAnet.Controls.Add(this.label13);
			this.groupCDAnet.Controls.Add(this.label11);
			this.groupCDAnet.Controls.Add(this.groupBox3);
			this.groupCDAnet.Controls.Add(this.textModemReconcile);
			this.groupCDAnet.Controls.Add(this.label10);
			this.groupCDAnet.Controls.Add(this.textModemSummary);
			this.groupCDAnet.Controls.Add(this.label8);
			this.groupCDAnet.Controls.Add(this.textModem);
			this.groupCDAnet.Controls.Add(this.label7);
			this.groupCDAnet.Controls.Add(this.comboNetwork);
			this.groupCDAnet.Controls.Add(this.label5);
			this.groupCDAnet.Controls.Add(this.textVersion);
			this.groupCDAnet.Controls.Add(this.label1);
			this.groupCDAnet.Location = new System.Drawing.Point(12, 399);
			this.groupCDAnet.Name = "groupCDAnet";
			this.groupCDAnet.Size = new System.Drawing.Size(620, 222);
			this.groupCDAnet.TabIndex = 19;
			this.groupCDAnet.TabStop = false;
			this.groupCDAnet.Text = "CDAnet";
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(6, 89);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(325, 34);
			this.label14.TabIndex = 0;
			this.label14.Text = "The values in this section are set and maintained by clicking \"Update Carriers\" i" +
    "n the Carriers list";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(235, 68);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(92, 17);
			this.label12.TabIndex = 0;
			this.label12.Text = "(1, 2, or 3)";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textEncryptionMethod
			// 
			this.textEncryptionMethod.Enabled = false;
			this.textEncryptionMethod.Location = new System.Drawing.Point(191, 65);
			this.textEncryptionMethod.Name = "textEncryptionMethod";
			this.textEncryptionMethod.Size = new System.Drawing.Size(42, 20);
			this.textEncryptionMethod.TabIndex = 2;
			this.textEncryptionMethod.TabStop = false;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(38, 70);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(151, 17);
			this.label13.TabIndex = 0;
			this.label13.Text = "Encryption Method";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(235, 42);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(92, 17);
			this.label11.TabIndex = 0;
			this.label11.Text = "(02 or 04)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox3.Controls.Add(this.check01);
			this.groupBox3.Controls.Add(this.check03m);
			this.groupBox3.Controls.Add(this.check03);
			this.groupBox3.Controls.Add(this.check07);
			this.groupBox3.Controls.Add(this.check06);
			this.groupBox3.Controls.Add(this.check04);
			this.groupBox3.Controls.Add(this.check05);
			this.groupBox3.Controls.Add(this.check02);
			this.groupBox3.Controls.Add(this.check08);
			this.groupBox3.Location = new System.Drawing.Point(337, 35);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(273, 181);
			this.groupBox3.TabIndex = 6;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Supported Transaction Types";
			// 
			// check01
			// 
			this.check01.Checked = true;
			this.check01.CheckState = System.Windows.Forms.CheckState.Checked;
			this.check01.Enabled = false;
			this.check01.Location = new System.Drawing.Point(16, 16);
			this.check01.Name = "check01";
			this.check01.Size = new System.Drawing.Size(251, 18);
			this.check01.TabIndex = 0;
			this.check01.TabStop = false;
			this.check01.Text = "Claim";
			this.check01.UseVisualStyleBackColor = true;
			// 
			// check03m
			// 
			this.check03m.Enabled = false;
			this.check03m.Location = new System.Drawing.Point(16, 106);
			this.check03m.Name = "check03m";
			this.check03m.Size = new System.Drawing.Size(251, 18);
			this.check03m.TabIndex = 5;
			this.check03m.TabStop = false;
			this.check03m.Text = "Predetermination Multi-page";
			this.check03m.UseVisualStyleBackColor = true;
			// 
			// check03
			// 
			this.check03.Enabled = false;
			this.check03.Location = new System.Drawing.Point(16, 88);
			this.check03.Name = "check03";
			this.check03.Size = new System.Drawing.Size(251, 18);
			this.check03.TabIndex = 4;
			this.check03.TabStop = false;
			this.check03.Text = "Predetermination Single Page";
			this.check03.UseVisualStyleBackColor = true;
			// 
			// check07
			// 
			this.check07.Enabled = false;
			this.check07.Location = new System.Drawing.Point(16, 52);
			this.check07.Name = "check07";
			this.check07.Size = new System.Drawing.Size(251, 18);
			this.check07.TabIndex = 2;
			this.check07.TabStop = false;
			this.check07.Text = "COB Claim Transaction";
			this.check07.UseVisualStyleBackColor = true;
			// 
			// check06
			// 
			this.check06.Enabled = false;
			this.check06.Location = new System.Drawing.Point(16, 160);
			this.check06.Name = "check06";
			this.check06.Size = new System.Drawing.Size(251, 18);
			this.check06.TabIndex = 8;
			this.check06.TabStop = false;
			this.check06.Text = "Request for Payment Reconciliation";
			this.check06.UseVisualStyleBackColor = true;
			// 
			// check04
			// 
			this.check04.Enabled = false;
			this.check04.Location = new System.Drawing.Point(16, 124);
			this.check04.Name = "check04";
			this.check04.Size = new System.Drawing.Size(251, 18);
			this.check04.TabIndex = 6;
			this.check04.TabStop = false;
			this.check04.Text = "Request for Outstanding Transactions [Mailbox]";
			this.check04.UseVisualStyleBackColor = true;
			// 
			// check05
			// 
			this.check05.Enabled = false;
			this.check05.Location = new System.Drawing.Point(16, 142);
			this.check05.Name = "check05";
			this.check05.Size = new System.Drawing.Size(251, 18);
			this.check05.TabIndex = 7;
			this.check05.TabStop = false;
			this.check05.Text = "Request for Summary Reconciliation";
			this.check05.UseVisualStyleBackColor = true;
			// 
			// check02
			// 
			this.check02.Enabled = false;
			this.check02.Location = new System.Drawing.Point(16, 70);
			this.check02.Name = "check02";
			this.check02.Size = new System.Drawing.Size(251, 18);
			this.check02.TabIndex = 3;
			this.check02.TabStop = false;
			this.check02.Text = "Claim Reversal";
			this.check02.UseVisualStyleBackColor = true;
			// 
			// check08
			// 
			this.check08.Enabled = false;
			this.check08.Location = new System.Drawing.Point(16, 34);
			this.check08.Name = "check08";
			this.check08.Size = new System.Drawing.Size(251, 18);
			this.check08.TabIndex = 1;
			this.check08.TabStop = false;
			this.check08.Text = "Eligibility Transaction";
			this.check08.UseVisualStyleBackColor = true;
			// 
			// textModemReconcile
			// 
			this.textModemReconcile.Location = new System.Drawing.Point(191, 188);
			this.textModemReconcile.Name = "textModemReconcile";
			this.textModemReconcile.Size = new System.Drawing.Size(121, 20);
			this.textModemReconcile.TabIndex = 5;
			this.textModemReconcile.TabStop = false;
			this.textModemReconcile.Visible = false;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(4, 187);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(185, 32);
			this.label10.TabIndex = 0;
			this.label10.Text = "Modem Phone Number - Request for Payment Reconciliation";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label10.Visible = false;
			// 
			// textModemSummary
			// 
			this.textModemSummary.Location = new System.Drawing.Point(191, 152);
			this.textModemSummary.Name = "textModemSummary";
			this.textModemSummary.Size = new System.Drawing.Size(121, 20);
			this.textModemSummary.TabIndex = 4;
			this.textModemSummary.TabStop = false;
			this.textModemSummary.Visible = false;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(1, 152);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(188, 33);
			this.label8.TabIndex = 0;
			this.label8.Text = "Modem Phone Number - Request for Summary Reconciliation";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label8.Visible = false;
			// 
			// textModem
			// 
			this.textModem.Location = new System.Drawing.Point(191, 126);
			this.textModem.Name = "textModem";
			this.textModem.Size = new System.Drawing.Size(121, 20);
			this.textModem.TabIndex = 3;
			this.textModem.TabStop = false;
			this.textModem.Visible = false;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(38, 131);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(151, 17);
			this.label7.TabIndex = 0;
			this.label7.Text = "Modem Phone Number";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label7.Visible = false;
			// 
			// comboNetwork
			// 
			this.comboNetwork.Enabled = false;
			this.comboNetwork.FormattingEnabled = true;
			this.comboNetwork.Location = new System.Drawing.Point(191, 11);
			this.comboNetwork.Name = "comboNetwork";
			this.comboNetwork.Size = new System.Drawing.Size(259, 21);
			this.comboNetwork.TabIndex = 0;
			this.comboNetwork.TabStop = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(38, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(151, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Network";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textVersion
			// 
			this.textVersion.Enabled = false;
			this.textVersion.Location = new System.Drawing.Point(191, 39);
			this.textVersion.Name = "textVersion";
			this.textVersion.Size = new System.Drawing.Size(42, 20);
			this.textVersion.TabIndex = 1;
			this.textVersion.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Version Number";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsHidden
			// 
			this.checkIsHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHidden.Location = new System.Drawing.Point(6, 273);
			this.checkIsHidden.Name = "checkIsHidden";
			this.checkIsHidden.Size = new System.Drawing.Size(186, 17);
			this.checkIsHidden.TabIndex = 13;
			this.checkIsHidden.Text = "Hidden";
			this.checkIsHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 639);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(78, 26);
			this.butDelete.TabIndex = 20;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(470, 639);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(78, 26);
			this.butOK.TabIndex = 21;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(554, 639);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(78, 26);
			this.butCancel.TabIndex = 22;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textCarrierNum
			// 
			this.textCarrierNum.BackColor = System.Drawing.SystemColors.Control;
			this.textCarrierNum.Location = new System.Drawing.Point(179, 10);
			this.textCarrierNum.Name = "textCarrierNum";
			this.textCarrierNum.ReadOnly = true;
			this.textCarrierNum.Size = new System.Drawing.Size(157, 20);
			this.textCarrierNum.TabIndex = 0;
			this.textCarrierNum.TabStop = false;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(6, 11);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(171, 18);
			this.label21.TabIndex = 0;
			this.label21.Text = "Carrier ID";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCarrierGroupName
			// 
			this.labelCarrierGroupName.Location = new System.Drawing.Point(6, 244);
			this.labelCarrierGroupName.Name = "labelCarrierGroupName";
			this.labelCarrierGroupName.Size = new System.Drawing.Size(171, 18);
			this.labelCarrierGroupName.TabIndex = 0;
			this.labelCarrierGroupName.Text = "Carrier Group";
			this.labelCarrierGroupName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelCarrierGroupName.Visible = false;
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(-3, 311);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(182, 18);
			this.labelColor.TabIndex = 0;
			this.labelColor.Text = "Appt Text Back Color (black=none)";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCarrierGroupName
			// 
			this.comboCarrierGroupName.Location = new System.Drawing.Point(179, 243);
			this.comboCarrierGroupName.Name = "comboCarrierGroupName";
			this.comboCarrierGroupName.Size = new System.Drawing.Size(226, 21);
			this.comboCarrierGroupName.TabIndex = 12;
			this.comboCarrierGroupName.Visible = false;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioBenefitSendsPat);
			this.groupBox2.Controls.Add(this.radioBenefitSendsIns);
			this.groupBox2.Location = new System.Drawing.Point(292, 273);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 58);
			this.groupBox2.TabIndex = 16;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Import Benefit Coinsurance";
			// 
			// radioBenefitSendsPat
			// 
			this.radioBenefitSendsPat.Location = new System.Drawing.Point(12, 16);
			this.radioBenefitSendsPat.Name = "radioBenefitSendsPat";
			this.radioBenefitSendsPat.Size = new System.Drawing.Size(182, 17);
			this.radioBenefitSendsPat.TabIndex = 0;
			this.radioBenefitSendsPat.TabStop = true;
			this.radioBenefitSendsPat.Text = "Carrier sends patient % (default)";
			this.radioBenefitSendsPat.UseVisualStyleBackColor = true;
			// 
			// radioBenefitSendsIns
			// 
			this.radioBenefitSendsIns.Location = new System.Drawing.Point(12, 35);
			this.radioBenefitSendsIns.Name = "radioBenefitSendsIns";
			this.radioBenefitSendsIns.Size = new System.Drawing.Size(182, 17);
			this.radioBenefitSendsIns.TabIndex = 1;
			this.radioBenefitSendsIns.TabStop = true;
			this.radioBenefitSendsIns.Text = "Carrier sends insurance %";
			this.radioBenefitSendsIns.UseVisualStyleBackColor = true;
			// 
			// labelSendElectronically
			// 
			this.labelSendElectronically.Location = new System.Drawing.Point(6, 153);
			this.labelSendElectronically.Name = "labelSendElectronically";
			this.labelSendElectronically.Size = new System.Drawing.Size(171, 18);
			this.labelSendElectronically.TabIndex = 0;
			this.labelSendElectronically.Text = "Send Electronically";
			this.labelSendElectronically.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSendElectronically
			// 
			this.comboSendElectronically.Location = new System.Drawing.Point(179, 152);
			this.comboSendElectronically.Name = "comboSendElectronically";
			this.comboSendElectronically.Size = new System.Drawing.Size(291, 21);
			this.comboSendElectronically.TabIndex = 9;
			// 
			// checkRealTimeEligibility
			// 
			this.checkRealTimeEligibility.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkRealTimeEligibility.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkRealTimeEligibility.Location = new System.Drawing.Point(6, 290);
			this.checkRealTimeEligibility.Name = "checkRealTimeEligibility";
			this.checkRealTimeEligibility.Size = new System.Drawing.Size(186, 17);
			this.checkRealTimeEligibility.TabIndex = 14;
			this.checkRealTimeEligibility.Text = "Is trusted for real-time eligibility";
			this.checkRealTimeEligibility.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// odColorPickerBack
			// 
			this.odColorPickerBack.BackgroundColor = System.Drawing.Color.Empty;
			this.odColorPickerBack.Location = new System.Drawing.Point(179, 310);
			this.odColorPickerBack.Name = "odColorPickerBack";
			this.odColorPickerBack.Size = new System.Drawing.Size(74, 21);
			this.odColorPickerBack.TabIndex = 15;
			// 
			// comboCobSendPaidByInsAt
			// 
			this.comboCobSendPaidByInsAt.Location = new System.Drawing.Point(179, 175);
			this.comboCobSendPaidByInsAt.Name = "comboCobSendPaidByInsAt";
			this.comboCobSendPaidByInsAt.Size = new System.Drawing.Size(128, 21);
			this.comboCobSendPaidByInsAt.TabIndex = 10;
			// 
			// labelCobSendPaidByOtherInsAt
			// 
			this.labelCobSendPaidByOtherInsAt.Location = new System.Drawing.Point(6, 176);
			this.labelCobSendPaidByOtherInsAt.Name = "labelCobSendPaidByOtherInsAt";
			this.labelCobSendPaidByOtherInsAt.Size = new System.Drawing.Size(171, 18);
			this.labelCobSendPaidByOtherInsAt.TabIndex = 272;
			this.labelCobSendPaidByOtherInsAt.Text = "Send Paid By Other Insurance At";
			this.labelCobSendPaidByOtherInsAt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboEraAutomation
			// 
			this.comboEraAutomation.Location = new System.Drawing.Point(179, 198);
			this.comboEraAutomation.Name = "comboEraAutomation";
			this.comboEraAutomation.Size = new System.Drawing.Size(180, 21);
			this.comboEraAutomation.TabIndex = 11;
			// 
			// labelEraAutomation
			// 
			this.labelEraAutomation.Location = new System.Drawing.Point(6, 199);
			this.labelEraAutomation.Name = "labelEraAutomation";
			this.labelEraAutomation.Size = new System.Drawing.Size(171, 18);
			this.labelEraAutomation.TabIndex = 274;
			this.labelEraAutomation.Text = "ERA Automation";
			this.labelEraAutomation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboOrthoConsolidate
			// 
			this.comboOrthoConsolidate.Location = new System.Drawing.Point(179, 220);
			this.comboOrthoConsolidate.Name = "comboOrthoConsolidate";
			this.comboOrthoConsolidate.Size = new System.Drawing.Size(180, 21);
			this.comboOrthoConsolidate.TabIndex = 277;
			// 
			// labelOrthoConsolidate
			// 
			this.labelOrthoConsolidate.Location = new System.Drawing.Point(6, 221);
			this.labelOrthoConsolidate.Name = "labelOrthoConsolidate";
			this.labelOrthoConsolidate.Size = new System.Drawing.Size(171, 18);
			this.labelOrthoConsolidate.TabIndex = 276;
			this.labelOrthoConsolidate.Text = "Consolidate Ortho Ins Payments";
			this.labelOrthoConsolidate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCarrierEdit
			// 
			this.ClientSize = new System.Drawing.Size(644, 677);
			this.Controls.Add(this.comboOrthoConsolidate);
			this.Controls.Add(this.labelOrthoConsolidate);
			this.Controls.Add(this.comboEraAutomation);
			this.Controls.Add(this.labelEraAutomation);
			this.Controls.Add(this.comboCobSendPaidByInsAt);
			this.Controls.Add(this.labelCobSendPaidByOtherInsAt);
			this.Controls.Add(this.odColorPickerBack);
			this.Controls.Add(this.checkRealTimeEligibility);
			this.Controls.Add(this.labelSendElectronically);
			this.Controls.Add(this.comboSendElectronically);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.comboCarrierGroupName);
			this.Controls.Add(this.labelColor);
			this.Controls.Add(this.labelCarrierGroupName);
			this.Controls.Add(this.textCarrierNum);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.checkIsHidden);
			this.Controls.Add(this.groupCDAnet);
			this.Controls.Add(this.textCity);
			this.Controls.Add(this.textState);
			this.Controls.Add(this.textZip);
			this.Controls.Add(this.textElectID);
			this.Controls.Add(this.textAddress2);
			this.Controls.Add(this.textAddress);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.textCarrierName);
			this.Controls.Add(this.labelCitySt);
			this.Controls.Add(this.checkIsCDAnet);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.labelElectID);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormCarrierEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Carrier";
			this.Load += new System.EventHandler(this.FormCarrierEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupCDAnet.ResumeLayout(false);
			this.groupCDAnet.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label labelElectID;
		private System.Windows.Forms.TextBox textCarrierName;
		private ValidPhone textPhone;
		private System.Windows.Forms.TextBox textAddress;
		private System.Windows.Forms.TextBox textAddress2;
		private System.Windows.Forms.TextBox textElectID;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboPlans;
		private System.Windows.Forms.TextBox textPlans;
		private System.Windows.Forms.Label label9;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textCity;
		private System.Windows.Forms.TextBox textState;
		private System.Windows.Forms.TextBox textZip;
		private System.Windows.Forms.Label labelCitySt;
		private CheckBox checkIsCDAnet;
		private UI.GroupBoxOD groupCDAnet;
		private TextBox textModemReconcile;
		private Label label10;
		private TextBox textModemSummary;
		private Label label8;
		private TextBox textModem;
		private Label label7;
		private ComboBox comboNetwork;
		private Label label5;
		private TextBox textVersion;
		private Label label1;
		private GroupBox groupBox3;
		private CheckBox check08;
		private CheckBox check03;
		private CheckBox check07;
		private CheckBox check06;
		private CheckBox check04;
		private CheckBox check05;
		private CheckBox check02;
		private CheckBox checkIsHidden;
		private Label label11;
		private CheckBox check03m;
		private Label label12;
		private TextBox textEncryptionMethod;
		private Label label13;
		private CheckBox check01;
		private Label label14;
		private TextBox textCarrierNum;
		private Label label21;
		private Label labelCarrierGroupName;
		private Label labelColor;
		private UI.ComboBoxOD comboCarrierGroupName;
		private GroupBox groupBox2;
		private RadioButton radioBenefitSendsIns;
		private RadioButton radioBenefitSendsPat;
		private Label labelSendElectronically;
		private UI.ComboBoxOD comboSendElectronically;
		private UI.ODColorPicker odColorPickerBack;
		private UI.ComboBoxOD comboCobSendPaidByInsAt;
		private Label labelCobSendPaidByOtherInsAt;
		private CheckBox checkRealTimeEligibility;
		private UI.ComboBoxOD comboEraAutomation;
		private Label labelEraAutomation;
		private UI.ComboBoxOD comboOrthoConsolidate;
		private Label labelOrthoConsolidate;
	}
}
