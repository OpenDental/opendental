// Laser Labels:
// Patient,Insurance Company,Custom,Birthday
//
// By Bill MacWilliams     Kapricorn Systems Inc.
//
//
// Code is broken up into Common section and a section for each Tab
//
// Some code is copied from other sections of OpenDental
//
// Report was designed in response to Client requests for more types of sheet labels
// with more selection choices.
//
// Last modification Date: 12/29/2007
//



using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using CodeBase;
using DataConnectionBase;


namespace OpenDental {
	public partial class FormRpLaserLabels:FormODBase {
		private DataTable AddrTable;
		private DataTable RptAddrTable;
		private int pagesPrinted;
		private int labelsPrinted;
		private int insRange;
		private string[] colName = new string[20];
		private OpenDental.UI.Button butOK;
		private TabPage tabCustomLabels;
		private TabPage tabInsCo;
		private TabPage tabPage1;
		private OpenDental.UI.ListBoxOD listProviders;
		private CheckBox checkAllProviders;
		private CheckBox checkActiveOnly;
		private CheckBox checkGroupByFamily;
		private TextBox textEndName;
		private TextBox textStartName;
		private Label labAllProviders;
		private Label labOnlyActive;
		private Label labGroupByFamily;
		private Label labEndName;
		private Label labStartName;
		private TabControl tabLabelSetup;
		private Label labLabelStart;
		private Label labEndingName;
		private RadioButton radioButRange;
		private RadioButton radioButSingle;
		private TextBox textInsCoStart;
		private Label labInsCoEnd;
		private TextBox textInsCoEnd;
		private Label labInsCoStart;
		private Label labInsCoSingle;
		private NumericUpDown numericInsCoSingle;
		private OpenDental.UI.Button butEndName;
		private OpenDental.UI.Button butStartName;
		private OpenDental.UI.Button butCancel;
		private Label labZip;
		private Label labState;
		private Label labCity;
		private Label labCustAddr2;
		private Label labCustAddr1;
		private Label labCustName;
		private Label labLabelCount;
		private TextBox textCusZip;
		private TextBox textCusState;
		private TextBox textCusCity;
		private TextBox textCusAddr2;
		private TextBox textCusAddr1;
		private TextBox textCusName;
		private NumericUpDown numericCusCount;
		private OpenDental.UI.ListBoxOD listCusNames;
		private int iLabelStart=0;
		private OpenDental.UI.Button butCusAdd;
		private System.Windows.Forms.PictureBox[] picLabel = new System.Windows.Forms.PictureBox[30];
		private OpenDental.UI.ListBoxOD listStatus;
		private Panel panLabels;
		private OpenDental.UI.Button butCusRemove;
		private Label labInsCoStartAddr;
		private Label labInsCoEndAddr;
		private OpenDental.UI.Button butInsCoEnd;
		private TabPage tabBirthday;
		private Label labBirthdayTo;
		private Label labBirthdayFrom;
		private CheckBox checkBirthdayActive;
		private TextBox textBirthdayFrom;
		private TextBox textBirthdayTo;
		private GroupBox groupBox1;
		private OpenDental.UI.Button butBirthdayLeft;
		private OpenDental.UI.Button butBirthdayRight;
		private OpenDental.UI.Button butBirthdayMonth;
		private OpenDental.UI.Button butInsCoStart;
		private List<Provider> _listProviders;

		public FormRpLaserLabels() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpLaserLabels));
			this.tabCustomLabels = new System.Windows.Forms.TabPage();
			this.butCusRemove = new OpenDental.UI.Button();
			this.butCusAdd = new OpenDental.UI.Button();
			this.listCusNames = new OpenDental.UI.ListBoxOD();
			this.numericCusCount = new System.Windows.Forms.NumericUpDown();
			this.textCusZip = new System.Windows.Forms.TextBox();
			this.textCusState = new System.Windows.Forms.TextBox();
			this.textCusCity = new System.Windows.Forms.TextBox();
			this.textCusAddr2 = new System.Windows.Forms.TextBox();
			this.textCusAddr1 = new System.Windows.Forms.TextBox();
			this.textCusName = new System.Windows.Forms.TextBox();
			this.labLabelCount = new System.Windows.Forms.Label();
			this.labZip = new System.Windows.Forms.Label();
			this.labState = new System.Windows.Forms.Label();
			this.labCity = new System.Windows.Forms.Label();
			this.labCustAddr2 = new System.Windows.Forms.Label();
			this.labCustAddr1 = new System.Windows.Forms.Label();
			this.labCustName = new System.Windows.Forms.Label();
			this.tabInsCo = new System.Windows.Forms.TabPage();
			this.butInsCoEnd = new OpenDental.UI.Button();
			this.butInsCoStart = new OpenDental.UI.Button();
			this.labInsCoEndAddr = new System.Windows.Forms.Label();
			this.labInsCoStartAddr = new System.Windows.Forms.Label();
			this.labInsCoSingle = new System.Windows.Forms.Label();
			this.numericInsCoSingle = new System.Windows.Forms.NumericUpDown();
			this.labInsCoEnd = new System.Windows.Forms.Label();
			this.textInsCoEnd = new System.Windows.Forms.TextBox();
			this.labInsCoStart = new System.Windows.Forms.Label();
			this.textInsCoStart = new System.Windows.Forms.TextBox();
			this.radioButRange = new System.Windows.Forms.RadioButton();
			this.radioButSingle = new System.Windows.Forms.RadioButton();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.labEndingName = new System.Windows.Forms.Label();
			this.listProviders = new OpenDental.UI.ListBoxOD();
			this.checkAllProviders = new System.Windows.Forms.CheckBox();
			this.checkActiveOnly = new System.Windows.Forms.CheckBox();
			this.checkGroupByFamily = new System.Windows.Forms.CheckBox();
			this.butEndName = new OpenDental.UI.Button();
			this.textEndName = new System.Windows.Forms.TextBox();
			this.textStartName = new System.Windows.Forms.TextBox();
			this.butStartName = new OpenDental.UI.Button();
			this.labAllProviders = new System.Windows.Forms.Label();
			this.labOnlyActive = new System.Windows.Forms.Label();
			this.labGroupByFamily = new System.Windows.Forms.Label();
			this.labEndName = new System.Windows.Forms.Label();
			this.labStartName = new System.Windows.Forms.Label();
			this.listStatus = new OpenDental.UI.ListBoxOD();
			this.tabLabelSetup = new System.Windows.Forms.TabControl();
			this.tabBirthday = new System.Windows.Forms.TabPage();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butBirthdayMonth = new OpenDental.UI.Button();
			this.butBirthdayRight = new OpenDental.UI.Button();
			this.butBirthdayLeft = new OpenDental.UI.Button();
			this.textBirthdayTo = new System.Windows.Forms.TextBox();
			this.labBirthdayFrom = new System.Windows.Forms.Label();
			this.textBirthdayFrom = new System.Windows.Forms.TextBox();
			this.labBirthdayTo = new System.Windows.Forms.Label();
			this.checkBirthdayActive = new System.Windows.Forms.CheckBox();
			this.labLabelStart = new System.Windows.Forms.Label();
			this.panLabels = new System.Windows.Forms.Panel();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.tabCustomLabels.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericCusCount)).BeginInit();
			this.tabInsCo.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericInsCoSingle)).BeginInit();
			this.tabPage1.SuspendLayout();
			this.tabLabelSetup.SuspendLayout();
			this.tabBirthday.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabCustomLabels
			// 
			this.tabCustomLabels.Controls.Add(this.butCusRemove);
			this.tabCustomLabels.Controls.Add(this.butCusAdd);
			this.tabCustomLabels.Controls.Add(this.listCusNames);
			this.tabCustomLabels.Controls.Add(this.numericCusCount);
			this.tabCustomLabels.Controls.Add(this.textCusZip);
			this.tabCustomLabels.Controls.Add(this.textCusState);
			this.tabCustomLabels.Controls.Add(this.textCusCity);
			this.tabCustomLabels.Controls.Add(this.textCusAddr2);
			this.tabCustomLabels.Controls.Add(this.textCusAddr1);
			this.tabCustomLabels.Controls.Add(this.textCusName);
			this.tabCustomLabels.Controls.Add(this.labLabelCount);
			this.tabCustomLabels.Controls.Add(this.labZip);
			this.tabCustomLabels.Controls.Add(this.labState);
			this.tabCustomLabels.Controls.Add(this.labCity);
			this.tabCustomLabels.Controls.Add(this.labCustAddr2);
			this.tabCustomLabels.Controls.Add(this.labCustAddr1);
			this.tabCustomLabels.Controls.Add(this.labCustName);
			this.tabCustomLabels.Location = new System.Drawing.Point(4, 22);
			this.tabCustomLabels.Name = "tabCustomLabels";
			this.tabCustomLabels.Padding = new System.Windows.Forms.Padding(3);
			this.tabCustomLabels.Size = new System.Drawing.Size(391, 319);
			this.tabCustomLabels.TabIndex = 2;
			this.tabCustomLabels.Text = "Custom Labels";
			this.tabCustomLabels.UseVisualStyleBackColor = true;
			// 
			// butCusRemove
			// 
			this.butCusRemove.Location = new System.Drawing.Point(262, 164);
			this.butCusRemove.Name = "butCusRemove";
			this.butCusRemove.Size = new System.Drawing.Size(75, 23);
			this.butCusRemove.TabIndex = 15;
			this.butCusRemove.Text = "Remove";
			this.butCusRemove.UseVisualStyleBackColor = true;
			this.butCusRemove.Visible = false;
			this.butCusRemove.Click += new System.EventHandler(this.butCusRemove_Click);
			// 
			// butCusAdd
			// 
			this.butCusAdd.Location = new System.Drawing.Point(177, 164);
			this.butCusAdd.Name = "butCusAdd";
			this.butCusAdd.Size = new System.Drawing.Size(75, 23);
			this.butCusAdd.TabIndex = 14;
			this.butCusAdd.Text = "&Add";
			this.butCusAdd.UseVisualStyleBackColor = true;
			this.butCusAdd.Click += new System.EventHandler(this.butCusAdd_Click);
			// 
			// listCusNames
			// 
			this.listCusNames.Location = new System.Drawing.Point(7, 191);
			this.listCusNames.Name = "listCusNames";
			this.listCusNames.Size = new System.Drawing.Size(330, 121);
			this.listCusNames.TabIndex = 16;
			this.listCusNames.SelectedIndexChanged += new System.EventHandler(this.listCusNames_SelectedIndexChanged);
			// 
			// numericCusCount
			// 
			this.numericCusCount.Location = new System.Drawing.Point(79, 141);
			this.numericCusCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericCusCount.Name = "numericCusCount";
			this.numericCusCount.Size = new System.Drawing.Size(120, 20);
			this.numericCusCount.TabIndex = 13;
			this.numericCusCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// textCusZip
			// 
			this.textCusZip.Location = new System.Drawing.Point(159, 112);
			this.textCusZip.MaxLength = 10;
			this.textCusZip.Name = "textCusZip";
			this.textCusZip.Size = new System.Drawing.Size(100, 20);
			this.textCusZip.TabIndex = 12;
			this.textCusZip.WordWrap = false;
			// 
			// textCusState
			// 
			this.textCusState.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.textCusState.Location = new System.Drawing.Point(79, 112);
			this.textCusState.MaxLength = 2;
			this.textCusState.Name = "textCusState";
			this.textCusState.Size = new System.Drawing.Size(37, 20);
			this.textCusState.TabIndex = 11;
			this.textCusState.WordWrap = false;
			// 
			// textCusCity
			// 
			this.textCusCity.Location = new System.Drawing.Point(79, 87);
			this.textCusCity.MaxLength = 40;
			this.textCusCity.Name = "textCusCity";
			this.textCusCity.Size = new System.Drawing.Size(258, 20);
			this.textCusCity.TabIndex = 10;
			this.textCusCity.WordWrap = false;
			// 
			// textCusAddr2
			// 
			this.textCusAddr2.Location = new System.Drawing.Point(79, 62);
			this.textCusAddr2.MaxLength = 40;
			this.textCusAddr2.Name = "textCusAddr2";
			this.textCusAddr2.Size = new System.Drawing.Size(258, 20);
			this.textCusAddr2.TabIndex = 9;
			this.textCusAddr2.WordWrap = false;
			// 
			// textCusAddr1
			// 
			this.textCusAddr1.Location = new System.Drawing.Point(79, 37);
			this.textCusAddr1.MaxLength = 40;
			this.textCusAddr1.Name = "textCusAddr1";
			this.textCusAddr1.Size = new System.Drawing.Size(258, 20);
			this.textCusAddr1.TabIndex = 8;
			this.textCusAddr1.WordWrap = false;
			// 
			// textCusName
			// 
			this.textCusName.Location = new System.Drawing.Point(79, 12);
			this.textCusName.MaxLength = 40;
			this.textCusName.Name = "textCusName";
			this.textCusName.Size = new System.Drawing.Size(258, 20);
			this.textCusName.TabIndex = 7;
			this.textCusName.WordWrap = false;
			// 
			// labLabelCount
			// 
			this.labLabelCount.AutoSize = true;
			this.labLabelCount.Location = new System.Drawing.Point(8, 149);
			this.labLabelCount.Name = "labLabelCount";
			this.labLabelCount.Size = new System.Drawing.Size(64, 13);
			this.labLabelCount.TabIndex = 6;
			this.labLabelCount.Text = "Label Count";
			// 
			// labZip
			// 
			this.labZip.AutoSize = true;
			this.labZip.Location = new System.Drawing.Point(127, 119);
			this.labZip.Name = "labZip";
			this.labZip.Size = new System.Drawing.Size(25, 13);
			this.labZip.TabIndex = 5;
			this.labZip.Text = "Zip:";
			// 
			// labState
			// 
			this.labState.AutoSize = true;
			this.labState.Location = new System.Drawing.Point(37, 119);
			this.labState.Name = "labState";
			this.labState.Size = new System.Drawing.Size(35, 13);
			this.labState.TabIndex = 4;
			this.labState.Text = "State:";
			// 
			// labCity
			// 
			this.labCity.AutoSize = true;
			this.labCity.Location = new System.Drawing.Point(45, 94);
			this.labCity.Name = "labCity";
			this.labCity.Size = new System.Drawing.Size(27, 13);
			this.labCity.TabIndex = 3;
			this.labCity.Text = "City:";
			// 
			// labCustAddr2
			// 
			this.labCustAddr2.AutoSize = true;
			this.labCustAddr2.Location = new System.Drawing.Point(18, 69);
			this.labCustAddr2.Name = "labCustAddr2";
			this.labCustAddr2.Size = new System.Drawing.Size(54, 13);
			this.labCustAddr2.TabIndex = 2;
			this.labCustAddr2.Text = "Address2:";
			// 
			// labCustAddr1
			// 
			this.labCustAddr1.AutoSize = true;
			this.labCustAddr1.Location = new System.Drawing.Point(18, 44);
			this.labCustAddr1.Name = "labCustAddr1";
			this.labCustAddr1.Size = new System.Drawing.Size(54, 13);
			this.labCustAddr1.TabIndex = 1;
			this.labCustAddr1.Text = "Address1:";
			// 
			// labCustName
			// 
			this.labCustName.AutoSize = true;
			this.labCustName.Location = new System.Drawing.Point(34, 19);
			this.labCustName.Name = "labCustName";
			this.labCustName.Size = new System.Drawing.Size(38, 13);
			this.labCustName.TabIndex = 0;
			this.labCustName.Text = "Name:";
			// 
			// tabInsCo
			// 
			this.tabInsCo.Controls.Add(this.butInsCoEnd);
			this.tabInsCo.Controls.Add(this.butInsCoStart);
			this.tabInsCo.Controls.Add(this.labInsCoEndAddr);
			this.tabInsCo.Controls.Add(this.labInsCoStartAddr);
			this.tabInsCo.Controls.Add(this.labInsCoSingle);
			this.tabInsCo.Controls.Add(this.numericInsCoSingle);
			this.tabInsCo.Controls.Add(this.labInsCoEnd);
			this.tabInsCo.Controls.Add(this.textInsCoEnd);
			this.tabInsCo.Controls.Add(this.labInsCoStart);
			this.tabInsCo.Controls.Add(this.textInsCoStart);
			this.tabInsCo.Controls.Add(this.radioButRange);
			this.tabInsCo.Controls.Add(this.radioButSingle);
			this.tabInsCo.Location = new System.Drawing.Point(4, 22);
			this.tabInsCo.Name = "tabInsCo";
			this.tabInsCo.Padding = new System.Windows.Forms.Padding(3);
			this.tabInsCo.Size = new System.Drawing.Size(391, 319);
			this.tabInsCo.TabIndex = 1;
			this.tabInsCo.Text = "Insurance Company Labels";
			this.tabInsCo.UseVisualStyleBackColor = true;
			// 
			// butInsCoEnd
			// 
			this.butInsCoEnd.Location = new System.Drawing.Point(301, 84);
			this.butInsCoEnd.Name = "butInsCoEnd";
			this.butInsCoEnd.Size = new System.Drawing.Size(21, 20);
			this.butInsCoEnd.TabIndex = 12;
			this.butInsCoEnd.Text = "...";
			this.butInsCoEnd.UseVisualStyleBackColor = true;
			this.butInsCoEnd.Visible = false;
			this.butInsCoEnd.Click += new System.EventHandler(this.butInsCo_Click);
			// 
			// butInsCoStart
			// 
			this.butInsCoStart.Location = new System.Drawing.Point(301, 34);
			this.butInsCoStart.Name = "butInsCoStart";
			this.butInsCoStart.Size = new System.Drawing.Size(21, 20);
			this.butInsCoStart.TabIndex = 11;
			this.butInsCoStart.Text = "...";
			this.butInsCoStart.UseVisualStyleBackColor = true;
			this.butInsCoStart.Click += new System.EventHandler(this.butInsCo_Click);
			// 
			// labInsCoEndAddr
			// 
			this.labInsCoEndAddr.Location = new System.Drawing.Point(4, 108);
			this.labInsCoEndAddr.Name = "labInsCoEndAddr";
			this.labInsCoEndAddr.Size = new System.Drawing.Size(333, 13);
			this.labInsCoEndAddr.TabIndex = 10;
			this.labInsCoEndAddr.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labInsCoStartAddr
			// 
			this.labInsCoStartAddr.Enabled = false;
			this.labInsCoStartAddr.Location = new System.Drawing.Point(6, 59);
			this.labInsCoStartAddr.Name = "labInsCoStartAddr";
			this.labInsCoStartAddr.Size = new System.Drawing.Size(331, 13);
			this.labInsCoStartAddr.TabIndex = 9;
			this.labInsCoStartAddr.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// labInsCoSingle
			// 
			this.labInsCoSingle.AutoSize = true;
			this.labInsCoSingle.Location = new System.Drawing.Point(36, 91);
			this.labInsCoSingle.Name = "labInsCoSingle";
			this.labInsCoSingle.Size = new System.Drawing.Size(67, 13);
			this.labInsCoSingle.TabIndex = 7;
			this.labInsCoSingle.Text = "Label Count:";
			// 
			// numericInsCoSingle
			// 
			this.numericInsCoSingle.Location = new System.Drawing.Point(113, 84);
			this.numericInsCoSingle.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
			this.numericInsCoSingle.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericInsCoSingle.Name = "numericInsCoSingle";
			this.numericInsCoSingle.Size = new System.Drawing.Size(120, 20);
			this.numericInsCoSingle.TabIndex = 6;
			this.numericInsCoSingle.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// labInsCoEnd
			// 
			this.labInsCoEnd.AutoSize = true;
			this.labInsCoEnd.Location = new System.Drawing.Point(43, 91);
			this.labInsCoEnd.Name = "labInsCoEnd";
			this.labInsCoEnd.Size = new System.Drawing.Size(60, 13);
			this.labInsCoEnd.TabIndex = 5;
			this.labInsCoEnd.Text = "End Name:";
			this.labInsCoEnd.Visible = false;
			// 
			// textInsCoEnd
			// 
			this.textInsCoEnd.Location = new System.Drawing.Point(113, 84);
			this.textInsCoEnd.Name = "textInsCoEnd";
			this.textInsCoEnd.Size = new System.Drawing.Size(181, 20);
			this.textInsCoEnd.TabIndex = 4;
			this.textInsCoEnd.Visible = false;
			this.textInsCoEnd.WordWrap = false;
			this.textInsCoEnd.Click += new System.EventHandler(this.textInsCoEnd_Click);
			// 
			// labInsCoStart
			// 
			this.labInsCoStart.AutoSize = true;
			this.labInsCoStart.Location = new System.Drawing.Point(40, 41);
			this.labInsCoStart.Name = "labInsCoStart";
			this.labInsCoStart.Size = new System.Drawing.Size(63, 13);
			this.labInsCoStart.TabIndex = 3;
			this.labInsCoStart.Text = "Start Name:";
			// 
			// textInsCoStart
			// 
			this.textInsCoStart.Location = new System.Drawing.Point(113, 34);
			this.textInsCoStart.Name = "textInsCoStart";
			this.textInsCoStart.Size = new System.Drawing.Size(181, 20);
			this.textInsCoStart.TabIndex = 2;
			this.textInsCoStart.WordWrap = false;
			this.textInsCoStart.Click += new System.EventHandler(this.textInsCoStart_Click);
			// 
			// radioButRange
			// 
			this.radioButRange.Location = new System.Drawing.Point(170, 10);
			this.radioButRange.Name = "radioButRange";
			this.radioButRange.Size = new System.Drawing.Size(130, 17);
			this.radioButRange.TabIndex = 1;
			this.radioButRange.Text = "Range of Companies";
			this.radioButRange.UseVisualStyleBackColor = true;
			this.radioButRange.CheckedChanged += new System.EventHandler(this.radioButRange_CheckedChanged);
			// 
			// radioButSingle
			// 
			this.radioButSingle.AutoSize = true;
			this.radioButSingle.Checked = true;
			this.radioButSingle.Location = new System.Drawing.Point(40, 10);
			this.radioButSingle.Name = "radioButSingle";
			this.radioButSingle.Size = new System.Drawing.Size(101, 17);
			this.radioButSingle.TabIndex = 0;
			this.radioButSingle.TabStop = true;
			this.radioButSingle.Text = "Single Company";
			this.radioButSingle.UseVisualStyleBackColor = true;
			this.radioButSingle.CheckedChanged += new System.EventHandler(this.radioButSingle_CheckedChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.labEndingName);
			this.tabPage1.Controls.Add(this.listProviders);
			this.tabPage1.Controls.Add(this.checkAllProviders);
			this.tabPage1.Controls.Add(this.checkActiveOnly);
			this.tabPage1.Controls.Add(this.checkGroupByFamily);
			this.tabPage1.Controls.Add(this.butEndName);
			this.tabPage1.Controls.Add(this.textEndName);
			this.tabPage1.Controls.Add(this.textStartName);
			this.tabPage1.Controls.Add(this.butStartName);
			this.tabPage1.Controls.Add(this.labAllProviders);
			this.tabPage1.Controls.Add(this.labOnlyActive);
			this.tabPage1.Controls.Add(this.labGroupByFamily);
			this.tabPage1.Controls.Add(this.labEndName);
			this.tabPage1.Controls.Add(this.labStartName);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(391, 319);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Patient Labels";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// labEndingName
			// 
			this.labEndingName.AutoSize = true;
			this.labEndingName.Location = new System.Drawing.Point(22, 46);
			this.labEndingName.Name = "labEndingName";
			this.labEndingName.Size = new System.Drawing.Size(87, 13);
			this.labEndingName.TabIndex = 15;
			this.labEndingName.Text = "End Name (L, F):";
			// 
			// listProviders
			// 
			this.listProviders.Location = new System.Drawing.Point(143, 152);
			this.listProviders.Name = "listProviders";
			this.listProviders.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProviders.Size = new System.Drawing.Size(172, 160);
			this.listProviders.TabIndex = 14;
			this.listProviders.Visible = false;
			// 
			// checkAllProviders
			// 
			this.checkAllProviders.AutoSize = true;
			this.checkAllProviders.Checked = true;
			this.checkAllProviders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkAllProviders.Location = new System.Drawing.Point(121, 126);
			this.checkAllProviders.Name = "checkAllProviders";
			this.checkAllProviders.Size = new System.Drawing.Size(15, 14);
			this.checkAllProviders.TabIndex = 12;
			this.checkAllProviders.UseVisualStyleBackColor = true;
			this.checkAllProviders.CheckedChanged += new System.EventHandler(this.checkAllProviders_CheckedChanged);
			// 
			// checkActiveOnly
			// 
			this.checkActiveOnly.AutoSize = true;
			this.checkActiveOnly.Checked = true;
			this.checkActiveOnly.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkActiveOnly.Location = new System.Drawing.Point(121, 102);
			this.checkActiveOnly.Name = "checkActiveOnly";
			this.checkActiveOnly.Size = new System.Drawing.Size(15, 14);
			this.checkActiveOnly.TabIndex = 11;
			this.checkActiveOnly.UseVisualStyleBackColor = true;
			this.checkActiveOnly.CheckedChanged += new System.EventHandler(this.checkActiveOnly_CheckedChanged);
			// 
			// checkGroupByFamily
			// 
			this.checkGroupByFamily.AutoSize = true;
			this.checkGroupByFamily.Checked = true;
			this.checkGroupByFamily.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkGroupByFamily.Location = new System.Drawing.Point(121, 78);
			this.checkGroupByFamily.Name = "checkGroupByFamily";
			this.checkGroupByFamily.Size = new System.Drawing.Size(15, 14);
			this.checkGroupByFamily.TabIndex = 10;
			this.checkGroupByFamily.UseVisualStyleBackColor = true;
			// 
			// butEndName
			// 
			this.butEndName.Location = new System.Drawing.Point(296, 43);
			this.butEndName.Name = "butEndName";
			this.butEndName.Size = new System.Drawing.Size(19, 20);
			this.butEndName.TabIndex = 9;
			this.butEndName.Text = "...";
			this.butEndName.UseVisualStyleBackColor = true;
			this.butEndName.Click += new System.EventHandler(this.butEndName_Click);
			// 
			// textEndName
			// 
			this.textEndName.Location = new System.Drawing.Point(121, 43);
			this.textEndName.Name = "textEndName";
			this.textEndName.Size = new System.Drawing.Size(172, 20);
			this.textEndName.TabIndex = 8;
			this.textEndName.WordWrap = false;
			// 
			// textStartName
			// 
			this.textStartName.Location = new System.Drawing.Point(121, 10);
			this.textStartName.Name = "textStartName";
			this.textStartName.Size = new System.Drawing.Size(172, 20);
			this.textStartName.TabIndex = 7;
			this.textStartName.WordWrap = false;
			// 
			// butStartName
			// 
			this.butStartName.Location = new System.Drawing.Point(296, 10);
			this.butStartName.Name = "butStartName";
			this.butStartName.Size = new System.Drawing.Size(19, 20);
			this.butStartName.TabIndex = 6;
			this.butStartName.Text = "...";
			this.butStartName.UseVisualStyleBackColor = true;
			this.butStartName.Click += new System.EventHandler(this.butStartName_Click);
			// 
			// labAllProviders
			// 
			this.labAllProviders.AutoSize = true;
			this.labAllProviders.Location = new System.Drawing.Point(46, 127);
			this.labAllProviders.Name = "labAllProviders";
			this.labAllProviders.Size = new System.Drawing.Size(68, 13);
			this.labAllProviders.TabIndex = 4;
			this.labAllProviders.Text = "All Providers:";
			// 
			// labOnlyActive
			// 
			this.labOnlyActive.AutoSize = true;
			this.labOnlyActive.Location = new System.Drawing.Point(9, 102);
			this.labOnlyActive.Name = "labOnlyActive";
			this.labOnlyActive.Size = new System.Drawing.Size(105, 13);
			this.labOnlyActive.TabIndex = 3;
			this.labOnlyActive.Text = "Only Active Patients:";
			// 
			// labGroupByFamily
			// 
			this.labGroupByFamily.AutoSize = true;
			this.labGroupByFamily.Location = new System.Drawing.Point(29, 78);
			this.labGroupByFamily.Name = "labGroupByFamily";
			this.labGroupByFamily.Size = new System.Drawing.Size(85, 13);
			this.labGroupByFamily.TabIndex = 2;
			this.labGroupByFamily.Text = "Group by Family:";
			// 
			// labEndName
			// 
			this.labEndName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labEndName.AutoSize = true;
			this.labEndName.Location = new System.Drawing.Point(-123, 50);
			this.labEndName.Name = "labEndName";
			this.labEndName.Size = new System.Drawing.Size(60, 13);
			this.labEndName.TabIndex = 1;
			this.labEndName.Text = "End Name:";
			// 
			// labStartName
			// 
			this.labStartName.AutoSize = true;
			this.labStartName.Location = new System.Drawing.Point(19, 14);
			this.labStartName.Name = "labStartName";
			this.labStartName.Size = new System.Drawing.Size(90, 13);
			this.labStartName.TabIndex = 0;
			this.labStartName.Text = "Start Name (L, F):";
			// 
			// listStatus
			// 
			this.listStatus.Location = new System.Drawing.Point(0, 0);
			this.listStatus.Name = "listStatus";
			this.listStatus.Size = new System.Drawing.Size(120, 95);
			this.listStatus.TabIndex = 0;
			// 
			// tabLabelSetup
			// 
			this.tabLabelSetup.Controls.Add(this.tabPage1);
			this.tabLabelSetup.Controls.Add(this.tabInsCo);
			this.tabLabelSetup.Controls.Add(this.tabCustomLabels);
			this.tabLabelSetup.Controls.Add(this.tabBirthday);
			this.tabLabelSetup.Location = new System.Drawing.Point(13, 13);
			this.tabLabelSetup.Name = "tabLabelSetup";
			this.tabLabelSetup.SelectedIndex = 0;
			this.tabLabelSetup.Size = new System.Drawing.Size(399, 345);
			this.tabLabelSetup.TabIndex = 3;
			// 
			// tabBirthday
			// 
			this.tabBirthday.Controls.Add(this.groupBox1);
			this.tabBirthday.Location = new System.Drawing.Point(4, 22);
			this.tabBirthday.Name = "tabBirthday";
			this.tabBirthday.Padding = new System.Windows.Forms.Padding(3);
			this.tabBirthday.Size = new System.Drawing.Size(391, 319);
			this.tabBirthday.TabIndex = 3;
			this.tabBirthday.Text = "Birthday Labels";
			this.tabBirthday.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butBirthdayMonth);
			this.groupBox1.Controls.Add(this.butBirthdayRight);
			this.groupBox1.Controls.Add(this.butBirthdayLeft);
			this.groupBox1.Controls.Add(this.textBirthdayTo);
			this.groupBox1.Controls.Add(this.labBirthdayFrom);
			this.groupBox1.Controls.Add(this.textBirthdayFrom);
			this.groupBox1.Controls.Add(this.labBirthdayTo);
			this.groupBox1.Controls.Add(this.checkBirthdayActive);
			this.groupBox1.Location = new System.Drawing.Point(7, 7);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(330, 151);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Date Range (without the year)";
			// 
			// butBirthdayMonth
			// 
			this.butBirthdayMonth.Location = new System.Drawing.Point(128, 30);
			this.butBirthdayMonth.Name = "butBirthdayMonth";
			this.butBirthdayMonth.Size = new System.Drawing.Size(75, 23);
			this.butBirthdayMonth.TabIndex = 9;
			this.butBirthdayMonth.Text = "Next Month";
			this.butBirthdayMonth.UseVisualStyleBackColor = true;
			this.butBirthdayMonth.Click += new System.EventHandler(this.butBirthdayMonth_Click);
			// 
			// butBirthdayRight
			// 
			this.butBirthdayRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butBirthdayRight.Location = new System.Drawing.Point(217, 30);
			this.butBirthdayRight.Name = "butBirthdayRight";
			this.butBirthdayRight.Size = new System.Drawing.Size(37, 23);
			this.butBirthdayRight.TabIndex = 8;
			this.butBirthdayRight.UseVisualStyleBackColor = true;
			this.butBirthdayRight.Click += new System.EventHandler(this.butBirthdayRight_Click);
			// 
			// butBirthdayLeft
			// 
			this.butBirthdayLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butBirthdayLeft.Location = new System.Drawing.Point(77, 30);
			this.butBirthdayLeft.Name = "butBirthdayLeft";
			this.butBirthdayLeft.Size = new System.Drawing.Size(37, 23);
			this.butBirthdayLeft.TabIndex = 7;
			this.butBirthdayLeft.Text = "button1";
			this.butBirthdayLeft.UseVisualStyleBackColor = true;
			this.butBirthdayLeft.Click += new System.EventHandler(this.butBirthdayLeft_Click);
			// 
			// textBirthdayTo
			// 
			this.textBirthdayTo.Location = new System.Drawing.Point(130, 96);
			this.textBirthdayTo.Name = "textBirthdayTo";
			this.textBirthdayTo.Size = new System.Drawing.Size(71, 20);
			this.textBirthdayTo.TabIndex = 6;
			this.textBirthdayTo.WordWrap = false;
			// 
			// labBirthdayFrom
			// 
			this.labBirthdayFrom.AutoSize = true;
			this.labBirthdayFrom.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.labBirthdayFrom.Location = new System.Drawing.Point(92, 73);
			this.labBirthdayFrom.Name = "labBirthdayFrom";
			this.labBirthdayFrom.Size = new System.Drawing.Size(33, 13);
			this.labBirthdayFrom.TabIndex = 1;
			this.labBirthdayFrom.Text = "From:";
			// 
			// textBirthdayFrom
			// 
			this.textBirthdayFrom.Location = new System.Drawing.Point(130, 66);
			this.textBirthdayFrom.Name = "textBirthdayFrom";
			this.textBirthdayFrom.Size = new System.Drawing.Size(71, 20);
			this.textBirthdayFrom.TabIndex = 5;
			this.textBirthdayFrom.WordWrap = false;
			// 
			// labBirthdayTo
			// 
			this.labBirthdayTo.AutoSize = true;
			this.labBirthdayTo.Location = new System.Drawing.Point(102, 103);
			this.labBirthdayTo.Name = "labBirthdayTo";
			this.labBirthdayTo.Size = new System.Drawing.Size(23, 13);
			this.labBirthdayTo.TabIndex = 2;
			this.labBirthdayTo.Text = "To:";
			// 
			// checkBirthdayActive
			// 
			this.checkBirthdayActive.AutoSize = true;
			this.checkBirthdayActive.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBirthdayActive.Checked = true;
			this.checkBirthdayActive.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBirthdayActive.Location = new System.Drawing.Point(87, 122);
			this.checkBirthdayActive.Name = "checkBirthdayActive";
			this.checkBirthdayActive.Size = new System.Drawing.Size(124, 17);
			this.checkBirthdayActive.TabIndex = 4;
			this.checkBirthdayActive.Text = "Active Patients Only:";
			this.checkBirthdayActive.UseVisualStyleBackColor = true;
			this.checkBirthdayActive.CheckedChanged += new System.EventHandler(this.checkBirthdayActive_CheckedChanged);
			// 
			// labLabelStart
			// 
			this.labLabelStart.AutoSize = true;
			this.labLabelStart.Location = new System.Drawing.Point(460, 53);
			this.labLabelStart.Name = "labLabelStart";
			this.labLabelStart.Size = new System.Drawing.Size(136, 13);
			this.labLabelStart.TabIndex = 5;
			this.labLabelStart.Text = "Choose Label Start position";
			// 
			// panLabels
			// 
			this.panLabels.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panLabels.Location = new System.Drawing.Point(442, 72);
			this.panLabels.Name = "panLabels";
			this.panLabels.Size = new System.Drawing.Size(170, 230);
			this.panLabels.TabIndex = 8;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(537, 381);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(442, 381);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 0;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormRpLaserLabels
			// 
			this.ClientSize = new System.Drawing.Size(649, 428);
			this.Controls.Add(this.panLabels);
			this.Controls.Add(this.labLabelStart);
			this.Controls.Add(this.tabLabelSetup);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormRpLaserLabels";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Laser Labels";
			this.Load += new System.EventHandler(this.FormLaserLabels_Load);
			this.tabCustomLabels.ResumeLayout(false);
			this.tabCustomLabels.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericCusCount)).EndInit();
			this.tabInsCo.ResumeLayout(false);
			this.tabInsCo.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericInsCoSingle)).EndInit();
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabLabelSetup.ResumeLayout(false);
			this.tabBirthday.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		//
		// Load initial data for providers and patient status
		//
		private void FormLaserLabels_Load(object sender,System.EventArgs e) {
			_listProviders=Providers.GetDeepCopy(true);
			listProviders.Items.AddList(_listProviders,x => x.GetLongDesc());
			if(listProviders.Items.Count>0) {
				listProviders.SelectedIndex=0;
			}
			checkAllProviders.Checked=true;
			listProviders.Visible=false;
			//If you change listStatus, be sure the change is reflected in BuildPatStatList.
			listStatus.Items.Add(Lan.g("enumPatientStatus","Patient"));
			listStatus.Items.Add(Lan.g("enumPatientStatus","NonPatient"));
			listStatus.Items.Add(Lan.g("enumPatientStatus","Inactive"));
			listStatus.Items.Add(Lan.g("enumPatientStatus","Archived"));
			listStatus.Items.Add(Lan.g("enumPatientStatus","Deceased"));
			listStatus.Items.Add(Lan.g("enumPatientStatus","Prospective"));
			displayLabels(iLabelStart);
			SetNextMonth();
		}

		//
		//Common Area for All Tabs in Laser Labels Report
		//
		private void butCancel_Click(object sender,EventArgs e) {
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			string patStat;
			string command;
			switch(tabLabelSetup.SelectedIndex) {
				//
				// Patient Tab Fill Address Table with selected information
				//
				case 0:
					if(!checkActiveOnly.Checked && listStatus.SelectedIndices.Count==0) {
						MsgBox.Show(this,"At least one patient status must be selected.");
						return;
					}
					if(checkAllProviders.Checked==true) {
						listProviders.SetAll(true);
					}
					string whereProv;//used as the provider portion of the where clauses.
					//each whereProv needs to be set up separately for each query
					whereProv="patient.PriProv in (";
					for(int i=0;i<listProviders.SelectedIndices.Count;i++) {
						if(i>0) {
							whereProv += ",";
						}
						whereProv += "'" + POut.Long(_listProviders[listProviders.SelectedIndices[i]].ProvNum) + "'";
					}
					whereProv += ") ";
					patStat = BuildPatStatList(checkActiveOnly.Checked);
					command = SetPatientBaseSelect();
					if(checkGroupByFamily.Checked) {
						command+=" INNER JOIN patient familymembers on familymembers.Guarantor=patient.Guarantor AND "+patStat.Replace("patient.","familymembers.");
						command+=" WHERE CONCAT(CONCAT(CONCAT(CONCAT(familymembers.LName,', '),familymembers.FName),' '),familymembers.MiddleI) >= ";
					}
					else {
						command+=" WHERE CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) >= ";
					}
					command+="'"+POut.String(textStartName.Text)+"'";
					if(checkGroupByFamily.Checked) {
						command+=" AND CONCAT(CONCAT(CONCAT(CONCAT(familymembers.LName,', '),familymembers.FName),' '),familymembers.MiddleI) <= ";
					}
					else {
						command+=" AND CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI) <= ";
					}
					command+="'"+POut.String(textEndName.Text)+"'";
					if(checkGroupByFamily.Checked) {
						command+=" AND patient.Guarantor=patient.PatNum";
					}
					else {
						command+=" AND "+patStat;
					}
					command+=" AND " + whereProv;
					if(checkGroupByFamily.Checked) {
						command+=" GROUP BY patient.Guarantor ";
					}
					command+=" ORDER BY CONCAT(CONCAT(CONCAT(CONCAT(patient.LName,', '),patient.FName),' '),patient.MiddleI)";
					buildLabelTable(command);
					break;
				//
				// Insurance Company Builder
				//
				case 1:
					command = "SELECT carrier.CarrierName,carrier.Address,carrier.Address2,carrier.City,carrier.State,carrier.Zip FROM carrier";
					if(radioButSingle.Checked == true) {
						if(labInsCoStartAddr.Text=="") {
							MsgBox.Show(this,"Please use the selection button first.");
							return;
						}
						command += " WHERE " + DbHelper.Concat("carrier.CarrierName","carrier.Address") + " = '" + textInsCoStart.Text + labInsCoStartAddr.Text + "'";
						RptAddrTable = Reports.GetTable(command);
						if(RptAddrTable.Rows.Count==0) {
							MsgBox.Show(this,"No matching carriers found.");
							return;
						}
						AddrTable = RptAddrTable.Clone();
						int numLabels=(int)numericInsCoSingle.Value;
						for(int i=0;i<numLabels;i++) {
							AddrTable.ImportRow(RptAddrTable.Rows[0]);
						}
						buildLabels();
					}
					else {
						command += " WHERE CONCAT(CONCAT(carrier.CarrierName,carrier.Address),carrier.City) >= '" + textInsCoStart.Text + labInsCoStartAddr.Text + "' AND CONCAT(CONCAT(carrier.CarrierName,carrier.Address),carrier.City) <= '" + textInsCoEnd.Text + labInsCoEndAddr + "'";
						command += " ORDER BY CONCAT(CONCAT(carrier.CarrierName,carrier.Address),carrier.City)";
						buildLabelTable(command);
					}
					break;

				//
				// Custom Label Builder
				//
				case 2:
					DataTable CusTable = new DataTable("CustomTable");
					DataColumn CusCol = new DataColumn();
					CusCol = new DataColumn("Name",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					CusCol = new DataColumn("Address",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					CusCol = new DataColumn("Address2",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					CusCol = new DataColumn("City",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					CusCol = new DataColumn("State",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					CusCol = new DataColumn("Zip",typeof(System.String));
					CusTable.Columns.Add(CusCol);
					string tmpString="";
					for(int i=0;i<listCusNames.Items.Count;i++) {
						tmpString=listCusNames.Items.GetTextShowingAt(i);
						string[] split=tmpString.Split(new char[] { '>' });
						int numLabels=Convert.ToInt32(split[6]);
						for(int j=0;j<numLabels;j++) {
							DataRow CusRow=CusTable.NewRow();
							for(int k=0;k<6;k++) {
								CusRow[k]=split[k].TrimStart(new char[] { ' ' });
							}
							CusTable.Rows.Add(CusRow);
						}
					}
					AddrTable = CusTable.Copy();
					buildLabels();
					break;
				//
				//Birthday Labels Builder
				//
				case 3:
					if(!checkBirthdayActive.Checked && listStatus.SelectedIndices.Count==0) {
						MsgBox.Show(this,"At least one patient status must be selected.");
						return;
					}
					DateTime dateBirthdayFrom = PIn.Date(textBirthdayFrom.Text);
					DateTime dateBirthdayTo = PIn.Date(textBirthdayTo.Text);
					if(dateBirthdayTo < dateBirthdayFrom) {
						MsgBox.Show(this,"To date cannot be before From date.");
						return;
					}
					patStat = BuildPatStatList(checkBirthdayActive.Checked);
					command = SetPatientBaseSelect();
					command += "WHERE SUBSTRING(Birthdate,6,5) >= '" + dateBirthdayFrom.ToString("MM-dd") + "' "
                    + "AND SUBSTRING(Birthdate,6,5) <= '" + dateBirthdayTo.ToString("MM-dd") + "' "
                    + "AND SUBSTRING(Birthdate,1,4) <> '0001' AND " + patStat + " ORDER BY LName,FName";
					buildLabelTable(command);
					break;
				default:
					break;
			}
		}

		private static string SetPatientBaseSelect() {
			string command;
			command = "SELECT patient.LName,patient.FName,patient.MiddleI,patient.Preferred,"//0-3
                + "patient.Address,patient.Address2,patient.City,patient.State,patient.Zip, "//4-9
                + "patient.Guarantor,"//10
                + "'' FamList ";//placeholder column: 11 for patient names and dates. If empty, then only single patient will print
			if(DataConnection.DBtype == DatabaseType.Oracle) {
				command += ",CASE WHEN patient.PatNum=patient.Guarantor THEN 1 ELSE 0 END AS isguarantor ";
			}
			command += "FROM patient ";
			return command;
		}

		private string BuildPatStatList(bool Active) {
			if(Active==true) {
				listStatus.SetAll(false);
				listStatus.SetSelected(0);
			}
			string patStat;// used as the Patient Status portion
			patStat="patient.patStatus in (";
			for(int i=0;i<listStatus.SelectedIndices.Count;i++) {
				if(i>0) {
					patStat+=",";
				}
				//patStat += "'" + listStatus.SelectedIndices[i] + "'";
				switch(listStatus.SelectedIndices[i]) {
					case 0:
						patStat+=(int)PatientStatus.Patient;
						break;
					case 1:
						patStat+=(int)PatientStatus.NonPatient;
						break;
					case 2:
						patStat+=(int)PatientStatus.Inactive;
						break;
					case 3:
						patStat+=(int)PatientStatus.Archived;
						break;
					case 4:
						patStat+=(int)PatientStatus.Deceased;
						break;
					case 5:
						patStat+=(int)PatientStatus.Prospective;
						break;
				}
			}
			patStat += ") ";
			return patStat;
		}
		private void buildLabelTable(string getData) {
			AddrTable = Reports.GetTable(getData);
			buildLabels();
		}

		private void addBlankLabels() {
			int i = 0;
			int cnt = 0;
			string valType = "";
			while(cnt < iLabelStart) {
				DataRow AddrRow = AddrTable.NewRow();
				foreach(DataColumn col in AddrTable.Columns) {
					colName[i] = col.ColumnName;
					valType = col.DataType.ToString();
					switch(valType) {
						case "System.String":
							AddrRow[colName[i]] = " ";
							break;
						default:
							AddrRow[colName[i]] = 0;
							break;
					}
					i += 1;
				}
				AddrTable.Rows.InsertAt(AddrRow,0);
				cnt += 1;
				i = 0;
			}
		}

		private void buildLabels() {
			if(AddrTable.Rows.Count > 0) {
				if(iLabelStart > 0) {
					addBlankLabels();
				}
				pagesPrinted = 0;
				labelsPrinted = 0;
				PrinterL.TryPreview(pdLabels_PrintPage,
					Lan.g(this,"Laser labels printed"),
					PrintSituation.LabelSheet,
					new Margins(0,0,0,0),
					PrintoutOrigin.AtMargin,
					totalPages:(int)Math.Ceiling((double)AddrTable.Rows.Count/30)
				);
			}
			else {
				MessageBox.Show("No Labels to Print for Selected Criteria");
			}
		}
		private void pdLabels_PrintPage(object sender,PrintPageEventArgs ev) {
			int totalPages = (int)Math.Ceiling((double)AddrTable.Rows.Count / 30);
			Graphics g = ev.Graphics;
			float yPos = 75;
			float xPos = 50;
			string text = "";
			while(yPos < 1000 && labelsPrinted < AddrTable.Rows.Count) {
				switch(tabLabelSetup.SelectedIndex) {
					case 0:
					case 3:
						text = AddrTable.Rows[labelsPrinted]["FName"].ToString() + " "
                        + AddrTable.Rows[labelsPrinted]["MiddleI"].ToString() + " "
                        + AddrTable.Rows[labelsPrinted]["LName"].ToString() + "\r\n";
						break;
					case 1:
						text = AddrTable.Rows[labelsPrinted]["CarrierName"].ToString() + "\r\n";
						break;
					case 2:
						text = AddrTable.Rows[labelsPrinted]["Name"].ToString() + "\r\n";
						break;
					default:
						return;
				}
				text += AddrTable.Rows[labelsPrinted]["Address"].ToString() + "\r\n";
				if(AddrTable.Rows[labelsPrinted]["Address2"].ToString() != "") {
					text += AddrTable.Rows[labelsPrinted]["Address2"].ToString() + "\r\n";
				}
				text += AddrTable.Rows[labelsPrinted]["City"].ToString();
				if(text.Trim().Length > 0) {
					text += ", ";
				}
				text += AddrTable.Rows[labelsPrinted]["State"].ToString() + "   "
                    + AddrTable.Rows[labelsPrinted]["Zip"].ToString() + "\r\n";
				Rectangle rect=new Rectangle((int)xPos,(int)yPos,275,100);
				MapAreaRoomControl.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rect,new StringFormat(),g);
				//reposition for next label
				xPos += 275;
				if(xPos > 850) {//drop a line
					xPos = 50;
					yPos += 100;
				}
				labelsPrinted++;
			}
			pagesPrinted++;
			if(pagesPrinted == totalPages) {
				ev.HasMorePages = false;
				pagesPrinted = 0;//because it has to print again from the print preview
				labelsPrinted = 0;
			}
			else {
				ev.HasMorePages = true;
			}
			g.Dispose();
		}
		//
		// Build Label Display and Add Click to each Label
		//
		private void displayLabels(int istartingpoint) {
			int x = 0;
			int y = 3;
			int cnt = 0;
			for(int i = 0;i < 10;i++) {
				for(int j = 0;j < 3;j++) {
					if(j == 0) x = 4;
					if(j == 1) x = 60;
					if(j == 2) x = 116;
					picLabel[cnt] = new System.Windows.Forms.PictureBox();
					picLabel[cnt].Location = new Point(x,y);
					picLabel[cnt].Size = new Size(51,17);
					picLabel[cnt].SizeMode = PictureBoxSizeMode.StretchImage;
					picLabel[cnt].Click += new EventHandler(clickLabel);
					picLabel[cnt].Tag = cnt.ToString();
					if(cnt < istartingpoint) {
						picLabel[cnt].Image = global::OpenDental.Properties.Resources.DeleteX;
					}
					else {
						picLabel[cnt].Image = global::OpenDental.Properties.Resources.butLabel;
					}
					LayoutManager.Add(picLabel[cnt],panLabels);
					cnt += 1;
				}
				y += 23;
			}
		}
		private void clickLabel(Object sender,EventArgs e) {
			System.Windows.Forms.PictureBox theLabel = (System.Windows.Forms.PictureBox)sender;
			iLabelStart = Convert.ToInt32(theLabel.Tag);
			for(int i=0;i<30;i++) {
				panLabels.Controls.Remove(picLabel[i]);
			}
			displayLabels(iLabelStart);
		}

		//
		//Add Patient Status List to Current Tab
		//
		private void displayPatStatus(int disTabIdx) {
			listStatus.Location = new System.Drawing.Point(12,190);
			listStatus.Name = "listStatus";
			listStatus.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			listStatus.Size = new System.Drawing.Size(125,108);
			listStatus.TabIndex = 13;
			listStatus.Visible = true;
			switch(disTabIdx) {
				case 0:
					tabPage1.Controls.Add(listStatus);
					break;
				case 3:
					tabBirthday.Controls.Add(listStatus);
					break;
				default:
					break;
			}
		}
		//
		//Patient Tab
		//
		private void checkActiveOnly_CheckedChanged(object sender,EventArgs e) {
			displayPatStatus(tabLabelSetup.SelectedIndex);
			if(checkActiveOnly.Checked) {
				listStatus.Visible=false;
			}
			else {
				listStatus.Visible=true;
			}
		}
		private void checkAllProviders_CheckedChanged(object sender,EventArgs e) {
			if(checkAllProviders.Checked) {
				listProviders.Visible=false;
			}
			else {
				listProviders.Visible=true;
			}
		}

		private void butStartName_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS = new FormPatientSelect();
			FormPS.SelectionModeOnly = true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult != DialogResult.OK) {
				return;
			}
			textStartName.Text=Patients.GetPat(FormPS.SelectedPatNum).GetNameLFnoPref();
		}

		private void butEndName_Click(object sender,EventArgs e) {
			using FormPatientSelect FormPS = new FormPatientSelect();
			FormPS.SelectionModeOnly = true;
			FormPS.ShowDialog();
			if(FormPS.DialogResult != DialogResult.OK) {
				return;
			}
			textEndName.Text=Patients.GetPat(FormPS.SelectedPatNum).GetNameLFnoPref();
			if(String.Compare(textStartName.Text,textEndName.Text)==1) {
				textEndName.Text = textStartName.Text.ToString();

			}
		}
		//
		//Insurance Company Tab
		//
		private void radioButSingle_CheckedChanged(object sender,EventArgs e) {
			labInsCoStart.Visible = true;
			labInsCoEnd.Visible = false;
			labInsCoSingle.Visible = true;
			textInsCoStart.Visible = true;
			textInsCoEnd.Visible = false;
			butInsCoEnd.Visible = false;
			numericInsCoSingle.Visible = true;
			insRange = 0;
		}

		private void radioButRange_CheckedChanged(object sender,EventArgs e) {
			labInsCoStart.Visible = true;
			labInsCoEnd.Visible = true;
			labInsCoSingle.Visible = false;
			textInsCoStart.Visible = true;
			textInsCoEnd.Visible = true;
			numericInsCoSingle.Visible = false;
			insRange = 1;
			butInsCoEnd.Visible = true;
		}

		private string FillFromInsCoList(long carrierNum) {
			Carrier carrier = Carriers.GetCarrier(carrierNum);
			if(insRange == 0) {
				textInsCoStart.Text = carrier.CarrierName;
				labInsCoStartAddr.Text = carrier.Address;
			}
			else {
				textInsCoEnd.Text = carrier.CarrierName;
				labInsCoEndAddr.Text = carrier.Address;
			}
			return (carrier.CarrierName);
		}
		private void textInsCoStart_Click(object sender,EventArgs e) {
			//insRange = 0;
		}
		private void textInsCoEnd_Click(object sender,EventArgs e) {
			//insRange = 1;
		}

		private void butInsCo_Click(object sender,EventArgs e) {
			using FormInsPlans FormIP = new FormInsPlans();
			FormIP.IsSelectMode = true;
			FormIP.ShowDialog();
			if(FormIP.DialogResult == DialogResult.Cancel) {
				return;
			}
			insRange=0;
			if(sender==butInsCoEnd) {
				insRange=1;
			}
			FillFromInsCoList(FormIP.SelectedPlan.CarrierNum);
		}
		//
		//Custom Tab
		//
		private void butCusAdd_Click(object sender,EventArgs e) {
			string cusLabelFormat = "";
			cusLabelFormat = textCusName.Text + ">  " + textCusAddr1.Text + ">  " + textCusAddr2.Text + ">  " + textCusCity.Text + ">  " + textCusState.Text + ">  " + textCusZip.Text + ">  " + numericCusCount.Value;
			listCusNames.Items.Add(cusLabelFormat);
			textCusName.Text = "";
			textCusAddr1.Text = "";
			textCusAddr2.Text = "";
			textCusCity.Text = "";
			textCusState.Text = "";
			textCusZip.Text = "";
			numericCusCount.Value = 1;
		}

		private void butCusRemove_Click(object sender,EventArgs e) {
			if(listCusNames.SelectedIndex==-1) {
				MsgBox.Show(this,"At least one name must be selected.");
				return;
			}
			listCusNames.Items.RemoveAt(listCusNames.SelectedIndex);
			butCusRemove.Visible=false;
		}

		private void listCusNames_SelectedIndexChanged(object sender,EventArgs e) {
			butCusRemove.Visible=true;
		}
		//
		//Birthday Tab
		//
		private void checkBirthdayActive_CheckedChanged(object sender,EventArgs e) {
			displayPatStatus(tabLabelSetup.SelectedIndex);
			if(checkBirthdayActive.Checked) {
				listStatus.Visible=false;
			}
			else {
				listStatus.Visible=true;
			}
		}
		private void butBirthdayLeft_Click(object sender,EventArgs e) {
			DateTime dateFrom=PIn.Date(textBirthdayFrom.Text);
			DateTime dateTo=PIn.Date(textBirthdayTo.Text);
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day) {
				toLastDay=true;
			}
			textBirthdayFrom.Text=dateFrom.AddMonths(-1).ToString(Lan.g(this,"MM/dd"));
			textBirthdayTo.Text=dateTo.AddMonths(-1).ToString(Lan.g(this,"MM/dd"));
			if(toLastDay) {
				dateTo=PIn.Date(textBirthdayTo.Text);
				textBirthdayTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToString(Lan.g(this,"MM/dd"));
			}
		}

		private void butBirthdayRight_Click(object sender,EventArgs e) {
			DateTime dateFrom=PIn.Date(textBirthdayFrom.Text);
			DateTime dateTo=PIn.Date(textBirthdayTo.Text);
			textBirthdayFrom.Text=dateFrom.AddMonths(-1).ToShortDateString();
			textBirthdayTo.Text=dateTo.AddMonths(-1).ToShortDateString();
			bool toLastDay=false;
			if(CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month)==dateTo.Day) {
				toLastDay=true;
			}
			textBirthdayFrom.Text=dateFrom.AddMonths(1).ToString(Lan.g(this,"MM/dd"));
			textBirthdayTo.Text=dateTo.AddMonths(1).ToString(Lan.g(this,"MM/dd"));
			if(toLastDay) {
				dateTo=PIn.Date(textBirthdayTo.Text);
				textBirthdayTo.Text=new DateTime(dateTo.Year,dateTo.Month,
					CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(dateTo.Year,dateTo.Month))
					.ToString(Lan.g(this,"MM/dd"));
			}
		}
		private void SetNextMonth() {
			textBirthdayFrom.Text
                = new DateTime(DateTime.Today.AddMonths(1).Year,DateTime.Today.AddMonths(1).Month,1)
					.ToString(Lan.g(this,"MM/dd"));
			textBirthdayTo.Text
                = new DateTime(DateTime.Today.AddMonths(2).Year,DateTime.Today.AddMonths(2).Month,1).AddDays(-1)
					.ToString(Lan.g(this,"MM/dd"));
		}

		private void butBirthdayMonth_Click(object sender,EventArgs e) {
			SetNextMonth();
		}
	}
}
