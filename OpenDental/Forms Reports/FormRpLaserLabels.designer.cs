using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpLaserLabels {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
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
		private OpenDental.UI.Button butCusAdd;
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
	}
}
