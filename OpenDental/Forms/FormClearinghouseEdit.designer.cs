using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormClearinghouseEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClearinghouseEdit));
			this.checkAllowAttachSend = new System.Windows.Forms.CheckBox();
			this.listBoxEraBehavior = new OpenDental.UI.ListBoxOD();
			this.checkIsClaimExportAllowed = new System.Windows.Forms.CheckBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelInfo = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label37 = new System.Windows.Forms.Label();
			this.textSeparatorSegment = new System.Windows.Forms.TextBox();
			this.label38 = new System.Windows.Forms.Label();
			this.label35 = new System.Windows.Forms.Label();
			this.textISA16 = new System.Windows.Forms.TextBox();
			this.label36 = new System.Windows.Forms.Label();
			this.label33 = new System.Windows.Forms.Label();
			this.textSeparatorData = new System.Windows.Forms.TextBox();
			this.label34 = new System.Windows.Forms.Label();
			this.label31 = new System.Windows.Forms.Label();
			this.textISA02 = new System.Windows.Forms.TextBox();
			this.label32 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textISA04 = new System.Windows.Forms.TextBox();
			this.label28 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textGS03 = new System.Windows.Forms.TextBox();
			this.label24 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label30 = new System.Windows.Forms.Label();
			this.label29 = new System.Windows.Forms.Label();
			this.textSenderTelephone = new OpenDental.ValidPhone();
			this.textSenderName = new System.Windows.Forms.TextBox();
			this.radioSenderBelow = new System.Windows.Forms.RadioButton();
			this.radioSenderOD = new System.Windows.Forms.RadioButton();
			this.labelSenderTelephone = new System.Windows.Forms.Label();
			this.labelSenderName = new System.Windows.Forms.Label();
			this.labelTIN = new System.Windows.Forms.Label();
			this.textSenderTIN = new System.Windows.Forms.TextBox();
			this.label22 = new System.Windows.Forms.Label();
			this.textISA15 = new System.Windows.Forms.TextBox();
			this.label23 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.textISA07 = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.textISA05 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textISA08 = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textLoginID = new System.Windows.Forms.TextBox();
			this.textModemPort = new System.Windows.Forms.TextBox();
			this.textClientProgram = new System.Windows.Forms.TextBox();
			this.textPassword = new System.Windows.Forms.TextBox();
			this.textResponsePath = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textPayors = new System.Windows.Forms.TextBox();
			this.textExportPath = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelLoginID = new System.Windows.Forms.Label();
			this.comboCommBridge = new OpenDental.UI.ComboBoxOD();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.labelClientProgram = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.comboFormat = new OpenDental.UI.ComboBoxOD();
			this.labelPassword = new System.Windows.Forms.Label();
			this.labelReportPath = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.labelExportPath = new System.Windows.Forms.Label();
			this.checkSaveDXC = new System.Windows.Forms.CheckBox();
			this.checkSaveDXCSoap = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkAllowAttachSend
			// 
			this.checkAllowAttachSend.Enabled = false;
			this.checkAllowAttachSend.Location = new System.Drawing.Point(61, 499);
			this.checkAllowAttachSend.Name = "checkAllowAttachSend";
			this.checkAllowAttachSend.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAllowAttachSend.Size = new System.Drawing.Size(204, 20);
			this.checkAllowAttachSend.TabIndex = 21;
			this.checkAllowAttachSend.Text = "Allow sending attachments";
			this.checkAllowAttachSend.UseVisualStyleBackColor = true;
			this.checkAllowAttachSend.CheckedChanged += new System.EventHandler(this.checkAllowAttachSend_CheckedChanged);
			// 
			// listBoxEraBehavior
			// 
			this.listBoxEraBehavior.Enabled = false;
			this.listBoxEraBehavior.Location = new System.Drawing.Point(583, 434);
			this.listBoxEraBehavior.Name = "listBoxEraBehavior";
			this.listBoxEraBehavior.Size = new System.Drawing.Size(264, 43);
			this.listBoxEraBehavior.TabIndex = 20;
			// 
			// checkIsClaimExportAllowed
			// 
			this.checkIsClaimExportAllowed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsClaimExportAllowed.Checked = true;
			this.checkIsClaimExportAllowed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkIsClaimExportAllowed.Enabled = false;
			this.checkIsClaimExportAllowed.Location = new System.Drawing.Point(583, 411);
			this.checkIsClaimExportAllowed.Name = "checkIsClaimExportAllowed";
			this.checkIsClaimExportAllowed.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsClaimExportAllowed.Size = new System.Drawing.Size(157, 20);
			this.checkIsClaimExportAllowed.TabIndex = 18;
			this.checkIsClaimExportAllowed.Text = "Use Claim Export Path";
			this.checkIsClaimExportAllowed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsClaimExportAllowed.UseVisualStyleBackColor = true;
			// 
			// comboClinic
			// 
			this.comboClinic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinic.HqDescription = "Unassigned/Default";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(728, 34);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(205, 21);
			this.comboClinic.TabIndex = 15;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.comboClinic_SelectionChangeCommitted);
			// 
			// labelInfo
			// 
			this.labelInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelInfo.Location = new System.Drawing.Point(248, 6);
			this.labelInfo.Name = "labelInfo";
			this.labelInfo.Size = new System.Drawing.Size(698, 17);
			this.labelInfo.TabIndex = 0;
			this.labelInfo.Text = "Bolded fields are unique for each clinic.  Other fields are not editable unless U" +
    "nassigned/Default is selected.";
			this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelInfo.Visible = false;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(479, 28);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(243, 15);
			this.label17.TabIndex = 0;
			this.label17.Text = "Also used in X12 1000B NM103";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.label31);
			this.groupBox1.Controls.Add(this.textISA02);
			this.groupBox1.Controls.Add(this.label32);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.textISA04);
			this.groupBox1.Controls.Add(this.label28);
			this.groupBox1.Controls.Add(this.label18);
			this.groupBox1.Controls.Add(this.textGS03);
			this.groupBox1.Controls.Add(this.label24);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.label22);
			this.groupBox1.Controls.Add(this.textISA15);
			this.groupBox1.Controls.Add(this.label23);
			this.groupBox1.Controls.Add(this.label21);
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.textISA07);
			this.groupBox1.Controls.Add(this.label20);
			this.groupBox1.Controls.Add(this.label16);
			this.groupBox1.Controls.Add(this.textISA05);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.textISA08);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Location = new System.Drawing.Point(9, 50);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(924, 298);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "X12 Required Fields - Provided by Clearinghouse or Carrier";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label37);
			this.groupBox3.Controls.Add(this.textSeparatorSegment);
			this.groupBox3.Controls.Add(this.label38);
			this.groupBox3.Controls.Add(this.label35);
			this.groupBox3.Controls.Add(this.textISA16);
			this.groupBox3.Controls.Add(this.label36);
			this.groupBox3.Controls.Add(this.label33);
			this.groupBox3.Controls.Add(this.textSeparatorData);
			this.groupBox3.Controls.Add(this.label34);
			this.groupBox3.Location = new System.Drawing.Point(484, 83);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(435, 124);
			this.groupBox3.TabIndex = 5;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Hexadecimal Delimiters (Always blank except for Denti-Cal)";
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(318, 62);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(107, 15);
			this.label37.TabIndex = 0;
			this.label37.Text = "\"1C\" for Denti-Cal.";
			this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSeparatorSegment
			// 
			this.textSeparatorSegment.Location = new System.Drawing.Point(221, 60);
			this.textSeparatorSegment.MaxLength = 255;
			this.textSeparatorSegment.Name = "textSeparatorSegment";
			this.textSeparatorSegment.Size = new System.Drawing.Size(96, 20);
			this.textSeparatorSegment.TabIndex = 3;
			// 
			// label38
			// 
			this.label38.Location = new System.Drawing.Point(6, 61);
			this.label38.Name = "label38";
			this.label38.Size = new System.Drawing.Size(209, 16);
			this.label38.TabIndex = 0;
			this.label38.Text = "Segment Terminator";
			this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label35
			// 
			this.label35.Location = new System.Drawing.Point(318, 40);
			this.label35.Name = "label35";
			this.label35.Size = new System.Drawing.Size(107, 15);
			this.label35.TabIndex = 0;
			this.label35.Text = "\"22\" for Denti-Cal.";
			this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA16
			// 
			this.textISA16.Location = new System.Drawing.Point(221, 38);
			this.textISA16.MaxLength = 255;
			this.textISA16.Name = "textISA16";
			this.textISA16.Size = new System.Drawing.Size(96, 20);
			this.textISA16.TabIndex = 2;
			// 
			// label36
			// 
			this.label36.Location = new System.Drawing.Point(6, 39);
			this.label36.Name = "label36";
			this.label36.Size = new System.Drawing.Size(209, 16);
			this.label36.TabIndex = 0;
			this.label36.Text = "Component Element Separator (ISA16)";
			this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label33
			// 
			this.label33.Location = new System.Drawing.Point(318, 18);
			this.label33.Name = "label33";
			this.label33.Size = new System.Drawing.Size(107, 18);
			this.label33.TabIndex = 0;
			this.label33.Text = "\"1D\" for Denti-Cal.";
			this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSeparatorData
			// 
			this.textSeparatorData.Location = new System.Drawing.Point(221, 16);
			this.textSeparatorData.MaxLength = 255;
			this.textSeparatorData.Name = "textSeparatorData";
			this.textSeparatorData.Size = new System.Drawing.Size(96, 20);
			this.textSeparatorData.TabIndex = 1;
			// 
			// label34
			// 
			this.label34.Location = new System.Drawing.Point(9, 17);
			this.label34.Name = "label34";
			this.label34.Size = new System.Drawing.Size(206, 16);
			this.label34.TabIndex = 0;
			this.label34.Text = "Data Element Separator";
			this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label31
			// 
			this.label31.Location = new System.Drawing.Point(339, 17);
			this.label31.Name = "label31";
			this.label31.Size = new System.Drawing.Size(231, 15);
			this.label31.TabIndex = 0;
			this.label31.Text = "Usually blank. \"DENTICAL\" for Denti-Cal.";
			this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA02
			// 
			this.textISA02.Location = new System.Drawing.Point(242, 15);
			this.textISA02.MaxLength = 255;
			this.textISA02.Name = "textISA02";
			this.textISA02.Size = new System.Drawing.Size(96, 20);
			this.textISA02.TabIndex = 1;
			// 
			// label32
			// 
			this.label32.Location = new System.Drawing.Point(6, 16);
			this.label32.Name = "label32";
			this.label32.Size = new System.Drawing.Size(235, 17);
			this.label32.TabIndex = 0;
			this.label32.Text = "Authorization Information (ISA02)";
			this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(339, 39);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(231, 15);
			this.label3.TabIndex = 0;
			this.label3.Text = "Usually blank. \"NONE\" for Denti-Cal.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA04
			// 
			this.textISA04.Location = new System.Drawing.Point(242, 37);
			this.textISA04.MaxLength = 255;
			this.textISA04.Name = "textISA04";
			this.textISA04.Size = new System.Drawing.Size(96, 20);
			this.textISA04.TabIndex = 2;
			// 
			// label28
			// 
			this.label28.Location = new System.Drawing.Point(6, 38);
			this.label28.Name = "label28";
			this.label28.Size = new System.Drawing.Size(235, 17);
			this.label28.TabIndex = 0;
			this.label28.Text = "Security Information (ISA04)";
			this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(339, 254);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(231, 15);
			this.label18.TabIndex = 0;
			this.label18.Text = "Usually the same as ISA08.";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textGS03
			// 
			this.textGS03.Location = new System.Drawing.Point(242, 252);
			this.textGS03.MaxLength = 255;
			this.textGS03.Name = "textGS03";
			this.textGS03.Size = new System.Drawing.Size(96, 20);
			this.textGS03.TabIndex = 8;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(9, 253);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(232, 17);
			this.label24.TabIndex = 0;
			this.label24.Text = "GS03";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label30);
			this.groupBox2.Controls.Add(this.label29);
			this.groupBox2.Controls.Add(this.textSenderTelephone);
			this.groupBox2.Controls.Add(this.textSenderName);
			this.groupBox2.Controls.Add(this.radioSenderBelow);
			this.groupBox2.Controls.Add(this.radioSenderOD);
			this.groupBox2.Controls.Add(this.labelSenderTelephone);
			this.groupBox2.Controls.Add(this.labelSenderName);
			this.groupBox2.Controls.Add(this.labelTIN);
			this.groupBox2.Controls.Add(this.textSenderTIN);
			this.groupBox2.Location = new System.Drawing.Point(12, 83);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(466, 124);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Sender ID - Used in ISA06, GS02, 1000A NM1, and 1000A PER";
			// 
			// label30
			// 
			this.label30.Location = new System.Drawing.Point(248, 17);
			this.label30.Name = "label30";
			this.label30.Size = new System.Drawing.Size(208, 15);
			this.label30.TabIndex = 0;
			this.label30.Text = "(use this for Emdeon)";
			this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label29
			// 
			this.label29.Location = new System.Drawing.Point(248, 37);
			this.label29.Name = "label29";
			this.label29.Size = new System.Drawing.Size(208, 17);
			this.label29.TabIndex = 0;
			this.label29.Text = "(much more common)";
			this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSenderTelephone
			// 
			this.textSenderTelephone.Location = new System.Drawing.Point(230, 99);
			this.textSenderTelephone.MaxLength = 255;
			this.textSenderTelephone.Name = "textSenderTelephone";
			this.textSenderTelephone.Size = new System.Drawing.Size(145, 20);
			this.textSenderTelephone.TabIndex = 5;
			// 
			// textSenderName
			// 
			this.textSenderName.Location = new System.Drawing.Point(230, 78);
			this.textSenderName.MaxLength = 255;
			this.textSenderName.Name = "textSenderName";
			this.textSenderName.Size = new System.Drawing.Size(145, 20);
			this.textSenderName.TabIndex = 4;
			// 
			// radioSenderBelow
			// 
			this.radioSenderBelow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSenderBelow.Checked = true;
			this.radioSenderBelow.Location = new System.Drawing.Point(1, 36);
			this.radioSenderBelow.Name = "radioSenderBelow";
			this.radioSenderBelow.Size = new System.Drawing.Size(242, 18);
			this.radioSenderBelow.TabIndex = 2;
			this.radioSenderBelow.TabStop = true;
			this.radioSenderBelow.Text = "The information below identifies the sender";
			this.radioSenderBelow.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSenderBelow.UseVisualStyleBackColor = true;
			this.radioSenderBelow.Click += new System.EventHandler(this.radio_Click);
			// 
			// radioSenderOD
			// 
			this.radioSenderOD.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSenderOD.Location = new System.Drawing.Point(22, 17);
			this.radioSenderOD.Name = "radioSenderOD";
			this.radioSenderOD.Size = new System.Drawing.Size(221, 18);
			this.radioSenderOD.TabIndex = 1;
			this.radioSenderOD.TabStop = true;
			this.radioSenderOD.Text = "This software is the \"sender\"";
			this.radioSenderOD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSenderOD.UseVisualStyleBackColor = true;
			this.radioSenderOD.Click += new System.EventHandler(this.radio_Click);
			// 
			// labelSenderTelephone
			// 
			this.labelSenderTelephone.Location = new System.Drawing.Point(37, 100);
			this.labelSenderTelephone.Name = "labelSenderTelephone";
			this.labelSenderTelephone.Size = new System.Drawing.Size(191, 17);
			this.labelSenderTelephone.TabIndex = 0;
			this.labelSenderTelephone.Text = "Telephone Number";
			this.labelSenderTelephone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSenderName
			// 
			this.labelSenderName.Location = new System.Drawing.Point(37, 79);
			this.labelSenderName.Name = "labelSenderName";
			this.labelSenderName.Size = new System.Drawing.Size(191, 17);
			this.labelSenderName.TabIndex = 0;
			this.labelSenderName.Text = "Name";
			this.labelSenderName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTIN
			// 
			this.labelTIN.Location = new System.Drawing.Point(37, 58);
			this.labelTIN.Name = "labelTIN";
			this.labelTIN.Size = new System.Drawing.Size(191, 17);
			this.labelTIN.TabIndex = 0;
			this.labelTIN.Text = "Tax ID Number";
			this.labelTIN.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSenderTIN
			// 
			this.textSenderTIN.Location = new System.Drawing.Point(230, 57);
			this.textSenderTIN.MaxLength = 255;
			this.textSenderTIN.Name = "textSenderTIN";
			this.textSenderTIN.Size = new System.Drawing.Size(145, 20);
			this.textSenderTIN.TabIndex = 3;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(290, 275);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(280, 15);
			this.label22.TabIndex = 0;
			this.label22.Text = "\"P\" for Production,  \"T\" for Test.";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA15
			// 
			this.textISA15.Location = new System.Drawing.Point(242, 273);
			this.textISA15.MaxLength = 255;
			this.textISA15.Name = "textISA15";
			this.textISA15.Size = new System.Drawing.Size(42, 20);
			this.textISA15.TabIndex = 9;
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(9, 274);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(232, 17);
			this.label23.TabIndex = 0;
			this.label23.Text = "Test or Production (ISA15)";
			this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(339, 233);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(231, 15);
			this.label21.TabIndex = 0;
			this.label21.Text = "Also used in 1000B NM109. ";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(339, 212);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(231, 15);
			this.label19.TabIndex = 0;
			this.label19.Text = "Usually \"ZZ\", sometimes \"30\".";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA07
			// 
			this.textISA07.Location = new System.Drawing.Point(242, 210);
			this.textISA07.MaxLength = 255;
			this.textISA07.Name = "textISA07";
			this.textISA07.Size = new System.Drawing.Size(96, 20);
			this.textISA07.TabIndex = 6;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(6, 211);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(235, 17);
			this.label20.TabIndex = 0;
			this.label20.Text = "Receiver ID Qualifier (ISA07)";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(339, 61);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(231, 15);
			this.label16.TabIndex = 0;
			this.label16.Text = "Usually \"ZZ\", sometimes \"30\".";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textISA05
			// 
			this.textISA05.Location = new System.Drawing.Point(242, 59);
			this.textISA05.MaxLength = 255;
			this.textISA05.Name = "textISA05";
			this.textISA05.Size = new System.Drawing.Size(96, 20);
			this.textISA05.TabIndex = 3;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 60);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(235, 17);
			this.label9.TabIndex = 0;
			this.label9.Text = "Sender ID Qualifier (ISA05)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textISA08
			// 
			this.textISA08.Location = new System.Drawing.Point(242, 231);
			this.textISA08.MaxLength = 255;
			this.textISA08.Name = "textISA08";
			this.textISA08.Size = new System.Drawing.Size(96, 20);
			this.textISA08.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(9, 232);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(232, 17);
			this.label5.TabIndex = 0;
			this.label5.Text = "Clearinghouse ID (ISA08)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLoginID
			// 
			this.textLoginID.Location = new System.Drawing.Point(251, 371);
			this.textLoginID.MaxLength = 255;
			this.textLoginID.Name = "textLoginID";
			this.textLoginID.Size = new System.Drawing.Size(96, 20);
			this.textLoginID.TabIndex = 3;
			// 
			// textModemPort
			// 
			this.textModemPort.Location = new System.Drawing.Point(251, 537);
			this.textModemPort.MaxLength = 255;
			this.textModemPort.Name = "textModemPort";
			this.textModemPort.Size = new System.Drawing.Size(32, 20);
			this.textModemPort.TabIndex = 9;
			this.textModemPort.Visible = false;
			// 
			// textClientProgram
			// 
			this.textClientProgram.Location = new System.Drawing.Point(251, 558);
			this.textClientProgram.MaxLength = 255;
			this.textClientProgram.Name = "textClientProgram";
			this.textClientProgram.Size = new System.Drawing.Size(317, 20);
			this.textClientProgram.TabIndex = 10;
			// 
			// textPassword
			// 
			this.textPassword.Location = new System.Drawing.Point(251, 392);
			this.textPassword.MaxLength = 255;
			this.textPassword.Name = "textPassword";
			this.textPassword.PasswordChar = '*';
			this.textPassword.Size = new System.Drawing.Size(96, 20);
			this.textPassword.TabIndex = 4;
			// 
			// textResponsePath
			// 
			this.textResponsePath.Location = new System.Drawing.Point(251, 434);
			this.textResponsePath.MaxLength = 255;
			this.textResponsePath.Name = "textResponsePath";
			this.textResponsePath.Size = new System.Drawing.Size(317, 20);
			this.textResponsePath.TabIndex = 6;
			// 
			// textBox2
			// 
			this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox2.Location = new System.Drawing.Point(251, 642);
			this.textBox2.MaxLength = 255;
			this.textBox2.Multiline = true;
			this.textBox2.Name = "textBox2";
			this.textBox2.ReadOnly = true;
			this.textBox2.Size = new System.Drawing.Size(390, 44);
			this.textBox2.TabIndex = 0;
			this.textBox2.TabStop = false;
			this.textBox2.Text = "The list of payor IDs should be separated by commas with no spaces or other punct" +
    "uation.  For instance: 01234,23456 is valid.  You do not have to enter any payor" +
    " ID\'s for a default Clearinghouse.";
			// 
			// textPayors
			// 
			this.textPayors.Location = new System.Drawing.Point(251, 579);
			this.textPayors.MaxLength = 255;
			this.textPayors.Multiline = true;
			this.textPayors.Name = "textPayors";
			this.textPayors.Size = new System.Drawing.Size(377, 60);
			this.textPayors.TabIndex = 11;
			// 
			// textExportPath
			// 
			this.textExportPath.Location = new System.Drawing.Point(251, 413);
			this.textExportPath.MaxLength = 255;
			this.textExportPath.Name = "textExportPath";
			this.textExportPath.Size = new System.Drawing.Size(317, 20);
			this.textExportPath.TabIndex = 5;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(251, 26);
			this.textDescription.MaxLength = 255;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(226, 20);
			this.textDescription.TabIndex = 1;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(9, 658);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(90, 26);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(784, 658);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(78, 26);
			this.butOK.TabIndex = 12;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(868, 658);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(78, 26);
			this.butCancel.TabIndex = 13;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelLoginID
			// 
			this.labelLoginID.Location = new System.Drawing.Point(98, 374);
			this.labelLoginID.Name = "labelLoginID";
			this.labelLoginID.Size = new System.Drawing.Size(151, 17);
			this.labelLoginID.TabIndex = 0;
			this.labelLoginID.Text = "Login ID";
			this.labelLoginID.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboCommBridge
			// 
			this.comboCommBridge.Location = new System.Drawing.Point(251, 477);
			this.comboCommBridge.Name = "comboCommBridge";
			this.comboCommBridge.Size = new System.Drawing.Size(145, 21);
			this.comboCommBridge.TabIndex = 8;
			this.comboCommBridge.SelectionChangeCommitted += new System.EventHandler(this.comboCommBridge_SelectionChangeCommitted);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(287, 541);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(267, 17);
			this.label14.TabIndex = 0;
			this.label14.Text = "(only if dialup)";
			this.label14.Visible = false;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(78, 540);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(172, 17);
			this.label13.TabIndex = 0;
			this.label13.Text = "Modem Port (1-4)";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.label13.Visible = false;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(248, 350);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(358, 18);
			this.label12.TabIndex = 0;
			this.label12.Text = "Not all values are required by each clearinghouse / carrier.";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClientProgram
			// 
			this.labelClientProgram.Location = new System.Drawing.Point(78, 561);
			this.labelClientProgram.Name = "labelClientProgram";
			this.labelClientProgram.Size = new System.Drawing.Size(172, 17);
			this.labelClientProgram.TabIndex = 0;
			this.labelClientProgram.Text = "Launch Client Program";
			this.labelClientProgram.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(98, 480);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(151, 17);
			this.label7.TabIndex = 0;
			this.label7.Text = "Comm Bridge";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboFormat
			// 
			this.comboFormat.Location = new System.Drawing.Point(251, 455);
			this.comboFormat.Name = "comboFormat";
			this.comboFormat.Size = new System.Drawing.Size(145, 21);
			this.comboFormat.TabIndex = 7;
			this.comboFormat.SelectedIndexChanged += new System.EventHandler(this.comboFormat_SelectedIndexChanged);
			// 
			// labelPassword
			// 
			this.labelPassword.Location = new System.Drawing.Point(98, 395);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(151, 17);
			this.labelPassword.TabIndex = 0;
			this.labelPassword.Text = "Password";
			this.labelPassword.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelReportPath
			// 
			this.labelReportPath.Location = new System.Drawing.Point(78, 437);
			this.labelReportPath.Name = "labelReportPath";
			this.labelReportPath.Size = new System.Drawing.Size(172, 17);
			this.labelReportPath.TabIndex = 0;
			this.labelReportPath.Text = "Report Path";
			this.labelReportPath.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(99, 457);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(151, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Format";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(99, 582);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(151, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Payors";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(35, 29);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(214, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "Description";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelExportPath
			// 
			this.labelExportPath.Location = new System.Drawing.Point(78, 416);
			this.labelExportPath.Name = "labelExportPath";
			this.labelExportPath.Size = new System.Drawing.Size(172, 17);
			this.labelExportPath.TabIndex = 0;
			this.labelExportPath.Text = "Claim Export Path";
			this.labelExportPath.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkSaveDXC
			// 
			this.checkSaveDXC.Location = new System.Drawing.Point(9, 517);
			this.checkSaveDXC.Name = "checkSaveDXC";
			this.checkSaveDXC.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSaveDXC.Size = new System.Drawing.Size(256, 20);
			this.checkSaveDXC.TabIndex = 22;
			this.checkSaveDXC.Text = "Save DXC Attachments to Images Module";
			this.checkSaveDXC.UseVisualStyleBackColor = true;
			this.checkSaveDXC.Visible = false;
			// 
			// checkSaveDXCSoap
			// 
			this.checkSaveDXCSoap.Location = new System.Drawing.Point(271, 499);
			this.checkSaveDXCSoap.Name = "checkSaveDXCSoap";
			this.checkSaveDXCSoap.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSaveDXCSoap.Size = new System.Drawing.Size(283, 20);
			this.checkSaveDXCSoap.TabIndex = 23;
			this.checkSaveDXCSoap.Text = "Save DXC Transmissions (Troubleshooting Only)";
			this.checkSaveDXCSoap.UseVisualStyleBackColor = true;
			this.checkSaveDXCSoap.Visible = false;
			// 
			// FormClearinghouseEdit
			// 
			this.ClientSize = new System.Drawing.Size(958, 696);
			this.Controls.Add(this.checkSaveDXCSoap);
			this.Controls.Add(this.checkSaveDXC);
			this.Controls.Add(this.checkAllowAttachSend);
			this.Controls.Add(this.listBoxEraBehavior);
			this.Controls.Add(this.checkIsClaimExportAllowed);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.labelInfo);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textLoginID);
			this.Controls.Add(this.textModemPort);
			this.Controls.Add(this.textClientProgram);
			this.Controls.Add(this.textPassword);
			this.Controls.Add(this.textResponsePath);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textPayors);
			this.Controls.Add(this.textExportPath);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelLoginID);
			this.Controls.Add(this.comboCommBridge);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.labelClientProgram);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.comboFormat);
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.labelReportPath);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.labelExportPath);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormClearinghouseEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Clearinghouse or Direct Carrier";
			this.Load += new System.EventHandler(this.FormClearinghouseEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label labelExportPath;
		private System.Windows.Forms.Label label6;
		private OpenDental.UI.Button butDelete;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textPayors;
		private System.Windows.Forms.TextBox textExportPath;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textISA08;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelReportPath;
		private System.Windows.Forms.Label labelPassword;
		private UI.ComboBoxOD comboFormat;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textPassword;
		private System.Windows.Forms.Label labelClientProgram;
		private System.Windows.Forms.TextBox textResponsePath;
		private UI.ComboBoxOD comboCommBridge;
		private System.Windows.Forms.TextBox textClientProgram;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textModemPort;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textLoginID;
		private System.Windows.Forms.Label labelLoginID;
		private GroupBox groupBox1;
		private Label label9;
		private TextBox textISA05;
		private Label label16;
		private TextBox textSenderTIN;
		private Label label19;
		private TextBox textISA07;
		private Label label20;
		private Label label21;
		private Label label22;
		private TextBox textISA15;
		private Label label23;
		private GroupBox groupBox2;
		private Label labelSenderTelephone;
		private Label labelSenderName;
		private Label labelTIN;
		private ValidPhone textSenderTelephone;
		private TextBox textSenderName;
		private RadioButton radioSenderBelow;
		private RadioButton radioSenderOD;
		private Label label17;
		private Label label18;
		private TextBox textGS03;
		private Label label24;
		private Label label29;
		private Label label30;
		private Label label31;
		private TextBox textISA02;
		private Label label32;
		private Label label3;
		private TextBox textISA04;
		private Label label28;
		private GroupBox groupBox3;
		private Label label33;
		private TextBox textSeparatorData;
		private Label label34;
		private Label label35;
		private TextBox textISA16;
		private Label label36;
		private Label label37;
		private TextBox textSeparatorSegment;
		private Label label38;
		private Label labelInfo;
		private UI.ComboBoxClinicPicker comboClinic;
		private CheckBox checkIsClaimExportAllowed;
		private UI.ListBoxOD listBoxEraBehavior;
		private CheckBox checkAllowAttachSend;
		private CheckBox checkSaveDXC;
		private CheckBox checkSaveDXCSoap;
	}
}
