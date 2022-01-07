using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPhoneEmpDefaultEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPhoneEmpDefaultEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.listRingGroup = new OpenDental.UI.ListBoxOD();
			this.checkIsGraphed = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkHasColor = new System.Windows.Forms.CheckBox();
			this.textEmpName = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textNotes = new OpenDental.ODtextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkIsPrivateScreen = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.listStatusOverride = new OpenDental.UI.ListBoxOD();
			this.label17 = new System.Windows.Forms.Label();
			this.checkIsTriageOperator = new System.Windows.Forms.CheckBox();
			this.textPhoneExt = new OpenDental.ValidNum();
			this.textEmployeeNum = new OpenDental.ValidNum();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboSite = new OpenDental.UI.ComboBoxOD();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textReportsTo = new System.Windows.Forms.TextBox();
			this.label21 = new System.Windows.Forms.Label();
			this.checkIsWorkingHome = new System.Windows.Forms.CheckBox();
			this.checkIsFurloughed = new System.Windows.Forms.CheckBox();
			this.textEmailPersonal = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textEmailWork = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textWirelessPhone = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 20);
			this.label1.TabIndex = 11;
			this.label1.Text = "EmployeeNum";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(1, 272);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(139, 20);
			this.label2.TabIndex = 13;
			this.label2.Text = "Default Queue";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listRingGroup
			// 
			this.listRingGroup.Location = new System.Drawing.Point(144, 272);
			this.listRingGroup.Name = "listRingGroup";
			this.listRingGroup.Size = new System.Drawing.Size(120, 56);
			this.listRingGroup.TabIndex = 4;
			this.listRingGroup.SelectedIndexChanged += new System.EventHandler(this.listRingGroup_SelectedIndexChanged);
			// 
			// checkIsGraphed
			// 
			this.checkIsGraphed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGraphed.Location = new System.Drawing.Point(3, 222);
			this.checkIsGraphed.Name = "checkIsGraphed";
			this.checkIsGraphed.Size = new System.Drawing.Size(155, 20);
			this.checkIsGraphed.TabIndex = 2;
			this.checkIsGraphed.Text = "Is Graphed (default)";
			this.checkIsGraphed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsGraphed.UseVisualStyleBackColor = true;
			this.checkIsGraphed.Click += new System.EventHandler(this.checkIsGraphed_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(40, 335);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 23;
			this.label5.Text = "Extension";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHasColor
			// 
			this.checkHasColor.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHasColor.Location = new System.Drawing.Point(3, 248);
			this.checkHasColor.Name = "checkHasColor";
			this.checkHasColor.Size = new System.Drawing.Size(155, 20);
			this.checkHasColor.TabIndex = 3;
			this.checkHasColor.Text = "Has Color";
			this.checkHasColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHasColor.UseVisualStyleBackColor = true;
			this.checkHasColor.Click += new System.EventHandler(this.checkHasColor_Click);
			// 
			// textEmpName
			// 
			this.textEmpName.Location = new System.Drawing.Point(144, 48);
			this.textEmpName.Name = "textEmpName";
			this.textEmpName.Size = new System.Drawing.Size(170, 20);
			this.textEmpName.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 47);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(137, 20);
			this.label6.TabIndex = 26;
			this.label6.Text = "Employee First Name";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNotes
			// 
			this.textNotes.AcceptsTab = true;
			this.textNotes.BackColor = System.Drawing.SystemColors.Window;
			this.textNotes.DetectLinksEnabled = false;
			this.textNotes.DetectUrls = false;
			this.textNotes.Location = new System.Drawing.Point(144, 415);
			this.textNotes.Name = "textNotes";
			this.textNotes.QuickPasteType = OpenDentBusiness.QuickPasteType.EmployeeStatus;
			this.textNotes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNotes.Size = new System.Drawing.Size(352, 51);
			this.textNotes.TabIndex = 7;
			this.textNotes.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 414);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 29;
			this.label3.Text = "Notes";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsPrivateScreen
			// 
			this.checkIsPrivateScreen.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPrivateScreen.Enabled = false;
			this.checkIsPrivateScreen.Location = new System.Drawing.Point(3, 504);
			this.checkIsPrivateScreen.Name = "checkIsPrivateScreen";
			this.checkIsPrivateScreen.Size = new System.Drawing.Size(155, 20);
			this.checkIsPrivateScreen.TabIndex = 9;
			this.checkIsPrivateScreen.Text = "Private Screen";
			this.checkIsPrivateScreen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsPrivateScreen.UseVisualStyleBackColor = true;
			this.checkIsPrivateScreen.Click += new System.EventHandler(this.checkIsPrivateScreen_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(200, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(366, 20);
			this.label7.TabIndex = 34;
			this.label7.Text = "This number must be looked up in the employee table";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(161, 220);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(428, 27);
			this.label8.TabIndex = 35;
			this.label8.Text = "This employee\'s default \'Graph\' status. Should be checked for most phone techs.\r\n" +
    "Use Phone Graph Edits grid to create exceptions.";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(161, 247);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(414, 20);
			this.label9.TabIndex = 36;
			this.label9.Text = "Show the red and green phone status colors in the phone panel";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(268, 277);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(359, 47);
			this.label10.TabIndex = 37;
			this.label10.Text = "The normal queue for this employee when clocked in.  If you change this value, th" +
    "e change will not immediately show on each workstation, but will instead require" +
    " a restart of OD.";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(204, 329);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(498, 32);
			this.label11.TabIndex = 38;
			this.label11.Text = "Phone extension for this employee.  Change this number to 0 if you are going to b" +
    "e floating.  Changing the extension to 0 will allow you to use the manage module" +
    " to clock in and out.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(315, 48);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(366, 20);
			this.label12.TabIndex = 39;
			this.label12.Text = "This is the name that will show in the phone panel.";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(268, 366);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(321, 20);
			this.label13.TabIndex = 40;
			this.label13.Text = "Mark yourself unavailable only if approved by manager";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(502, 424);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(157, 35);
			this.label14.TabIndex = 41;
			this.label14.Text = "Why unavailable?\r\nWhy offline assist?";
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(161, 501);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(194, 31);
			this.label16.TabIndex = 43;
			this.label16.Text = "Halts screen captures.  Only used/allowed by managers. ";
			// 
			// listStatusOverride
			// 
			this.listStatusOverride.Location = new System.Drawing.Point(144, 366);
			this.listStatusOverride.Name = "listStatusOverride";
			this.listStatusOverride.Size = new System.Drawing.Size(120, 43);
			this.listStatusOverride.TabIndex = 6;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(2, 368);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(139, 20);
			this.label17.TabIndex = 46;
			this.label17.Text = "StatusOverride";
			this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsTriageOperator
			// 
			this.checkIsTriageOperator.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsTriageOperator.Location = new System.Drawing.Point(3, 526);
			this.checkIsTriageOperator.Name = "checkIsTriageOperator";
			this.checkIsTriageOperator.Size = new System.Drawing.Size(155, 20);
			this.checkIsTriageOperator.TabIndex = 10;
			this.checkIsTriageOperator.Text = "Triage Operator";
			this.checkIsTriageOperator.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsTriageOperator.UseVisualStyleBackColor = true;
			// 
			// textPhoneExt
			// 
			this.textPhoneExt.Location = new System.Drawing.Point(144, 336);
			this.textPhoneExt.MaxVal = 99999;
			this.textPhoneExt.MinVal = 0;
			this.textPhoneExt.Name = "textPhoneExt";
			this.textPhoneExt.Size = new System.Drawing.Size(54, 20);
			this.textPhoneExt.TabIndex = 5;
			// 
			// textEmployeeNum
			// 
			this.textEmployeeNum.Location = new System.Drawing.Point(144, 24);
			this.textEmployeeNum.MaxVal = 99999;
			this.textEmployeeNum.MinVal = 0;
			this.textEmployeeNum.Name = "textEmployeeNum";
			this.textEmployeeNum.Size = new System.Drawing.Size(54, 20);
			this.textEmployeeNum.TabIndex = 0;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(28, 615);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(84, 24);
			this.butDelete.TabIndex = 13;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(739, 615);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 11;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(832, 615);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboSite
			// 
			this.comboSite.Location = new System.Drawing.Point(144, 474);
			this.comboSite.Name = "comboSite";
			this.comboSite.Size = new System.Drawing.Size(213, 21);
			this.comboSite.TabIndex = 49;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(361, 474);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(359, 30);
			this.label18.TabIndex = 50;
			this.label18.Text = "Set site to the physical location of the phone associated to the extension above." +
    "  Used in MapHQ and reserving conference rooms.";
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(41, 473);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(100, 20);
			this.label19.TabIndex = 51;
			this.label19.Text = "Site";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textReportsTo);
			this.groupBox1.Controls.Add(this.label21);
			this.groupBox1.Controls.Add(this.checkIsWorkingHome);
			this.groupBox1.Controls.Add(this.checkIsFurloughed);
			this.groupBox1.Controls.Add(this.textEmailPersonal);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.textEmailWork);
			this.groupBox1.Controls.Add(this.label15);
			this.groupBox1.Controls.Add(this.textWirelessPhone);
			this.groupBox1.Controls.Add(this.label20);
			this.groupBox1.Location = new System.Drawing.Point(6, 73);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(427, 143);
			this.groupBox1.TabIndex = 52;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "From Employee table, read only";
			// 
			// textReportsTo
			// 
			this.textReportsTo.Location = new System.Drawing.Point(138, 16);
			this.textReportsTo.MaxLength = 100;
			this.textReportsTo.Name = "textReportsTo";
			this.textReportsTo.ReadOnly = true;
			this.textReportsTo.Size = new System.Drawing.Size(120, 20);
			this.textReportsTo.TabIndex = 57;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(47, 20);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(90, 14);
			this.label21.TabIndex = 58;
			this.label21.Text = "Reports To";
			this.label21.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkIsWorkingHome
			// 
			this.checkIsWorkingHome.AutoCheck = false;
			this.checkIsWorkingHome.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsWorkingHome.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsWorkingHome.Location = new System.Drawing.Point(34, 123);
			this.checkIsWorkingHome.Name = "checkIsWorkingHome";
			this.checkIsWorkingHome.Size = new System.Drawing.Size(117, 18);
			this.checkIsWorkingHome.TabIndex = 53;
			this.checkIsWorkingHome.Text = "Working From Home";
			this.checkIsWorkingHome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsFurloughed
			// 
			this.checkIsFurloughed.AutoCheck = false;
			this.checkIsFurloughed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsFurloughed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsFurloughed.Location = new System.Drawing.Point(34, 104);
			this.checkIsFurloughed.Name = "checkIsFurloughed";
			this.checkIsFurloughed.Size = new System.Drawing.Size(117, 18);
			this.checkIsFurloughed.TabIndex = 52;
			this.checkIsFurloughed.Text = "Furloughed";
			this.checkIsFurloughed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmailPersonal
			// 
			this.textEmailPersonal.Location = new System.Drawing.Point(138, 82);
			this.textEmailPersonal.MaxLength = 100;
			this.textEmailPersonal.Name = "textEmailPersonal";
			this.textEmailPersonal.ReadOnly = true;
			this.textEmailPersonal.Size = new System.Drawing.Size(272, 20);
			this.textEmailPersonal.TabIndex = 51;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(47, 86);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 14);
			this.label4.TabIndex = 56;
			this.label4.Text = "Email Personal";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textEmailWork
			// 
			this.textEmailWork.Location = new System.Drawing.Point(138, 60);
			this.textEmailWork.MaxLength = 100;
			this.textEmailWork.Name = "textEmailWork";
			this.textEmailWork.ReadOnly = true;
			this.textEmailWork.Size = new System.Drawing.Size(272, 20);
			this.textEmailWork.TabIndex = 50;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(47, 64);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(90, 14);
			this.label15.TabIndex = 55;
			this.label15.Text = "Email Work";
			this.label15.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textWirelessPhone
			// 
			this.textWirelessPhone.Location = new System.Drawing.Point(138, 38);
			this.textWirelessPhone.MaxLength = 100;
			this.textWirelessPhone.Name = "textWirelessPhone";
			this.textWirelessPhone.ReadOnly = true;
			this.textWirelessPhone.Size = new System.Drawing.Size(242, 20);
			this.textWirelessPhone.TabIndex = 49;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(47, 42);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(90, 14);
			this.label20.TabIndex = 54;
			this.label20.Text = "Wireless Phone";
			this.label20.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormPhoneEmpDefaultEdit
			// 
			this.ClientSize = new System.Drawing.Size(924, 652);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.comboSite);
			this.Controls.Add(this.checkIsTriageOperator);
			this.Controls.Add(this.listStatusOverride);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.textPhoneExt);
			this.Controls.Add(this.textEmployeeNum);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.checkIsPrivateScreen);
			this.Controls.Add(this.textNotes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textEmpName);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.checkHasColor);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.checkIsGraphed);
			this.Controls.Add(this.listRingGroup);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPhoneEmpDefaultEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Phone Employee Defaults";
			this.Load += new System.EventHandler(this.FormPhoneEmpDefaultEdit_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private Label label1;
		private Label label2;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.ListBoxOD listRingGroup;
		private CheckBox checkIsGraphed;
		private Label label5;
		private CheckBox checkHasColor;
		private TextBox textEmpName;
		private Label label6;
		private OpenDental.ODtextBox textNotes;
		private Label label3;
		private CheckBox checkIsPrivateScreen;
		private Label label7;
		private Label label8;
		private Label label9;
		private Label label10;
		private Label label11;
		private Label label12;
		private Label label13;
		private Label label14;
		private Label label16;
		private ValidNum textEmployeeNum;
		private ValidNum textPhoneExt;
		private OpenDental.UI.ListBoxOD listStatusOverride;
		private Label label17;
		private CheckBox checkIsTriageOperator;
		private OpenDental.UI.ComboBoxOD comboSite;
		private Label label18;
		private Label label19;
		private GroupBox groupBox1;
		private CheckBox checkIsWorkingHome;
		private CheckBox checkIsFurloughed;
		private TextBox textEmailPersonal;
		private Label label4;
		private TextBox textEmailWork;
		private Label label15;
		private TextBox textWirelessPhone;
		private Label label20;
		private TextBox textReportsTo;
		private Label label21;
	}
}
