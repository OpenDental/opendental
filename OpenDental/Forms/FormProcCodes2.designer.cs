using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcCodes {
		private System.ComponentModel.IContainer components = null;

		///<summary></summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcCodes));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butEditFeeSched = new OpenDental.UI.Button();
			this.butTools = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.listCategories = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.butEditCategories = new OpenDental.UI.Button();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butAll = new OpenDental.UI.Button();
			this.textCode = new System.Windows.Forms.TextBox();
			this.textAbbreviation = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.butShowHiddenDefault = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboSort = new System.Windows.Forms.ComboBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.label5 = new System.Windows.Forms.Label();
			this.butNew = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butProcTools = new OpenDental.UI.Button();
			this.groupProcCodeSetup = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.comboFeeSchedGroup1 = new OpenDental.UI.ComboBoxOD();
			this.checkGroups1 = new System.Windows.Forms.CheckBox();
			this.butPickProv1 = new OpenDental.UI.Button();
			this.butPickClinic1 = new OpenDental.UI.Button();
			this.butPickSched1 = new OpenDental.UI.Button();
			this.labelSched1 = new System.Windows.Forms.Label();
			this.labelClinic1 = new System.Windows.Forms.Label();
			this.labelProvider1 = new System.Windows.Forms.Label();
			this.comboProvider1 = new System.Windows.Forms.ComboBox();
			this.comboClinic1 = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboFeeSched1 = new OpenDental.UI.ComboBoxOD();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.comboFeeSchedGroup2 = new OpenDental.UI.ComboBoxOD();
			this.checkGroups2 = new System.Windows.Forms.CheckBox();
			this.butPickProv2 = new OpenDental.UI.Button();
			this.butPickClinic2 = new OpenDental.UI.Button();
			this.butPickSched2 = new OpenDental.UI.Button();
			this.labelSched2 = new System.Windows.Forms.Label();
			this.comboProvider2 = new System.Windows.Forms.ComboBox();
			this.labelClinic2 = new System.Windows.Forms.Label();
			this.comboClinic2 = new OpenDental.UI.ComboBoxClinicPicker();
			this.labelProvider2 = new System.Windows.Forms.Label();
			this.comboFeeSched2 = new OpenDental.UI.ComboBoxOD();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.comboFeeSchedGroup3 = new OpenDental.UI.ComboBoxOD();
			this.checkGroups3 = new System.Windows.Forms.CheckBox();
			this.butPickProv3 = new OpenDental.UI.Button();
			this.butPickClinic3 = new OpenDental.UI.Button();
			this.butPickSched3 = new OpenDental.UI.Button();
			this.labelSched3 = new System.Windows.Forms.Label();
			this.comboProvider3 = new System.Windows.Forms.ComboBox();
			this.labelClinic3 = new System.Windows.Forms.Label();
			this.labelProvider3 = new System.Windows.Forms.Label();
			this.comboClinic3 = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboFeeSched3 = new OpenDental.UI.ComboBoxOD();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butColorClinicProv = new System.Windows.Forms.Button();
			this.butColorProvider = new System.Windows.Forms.Button();
			this.butColorClinic = new System.Windows.Forms.Button();
			this.butColorDefault = new System.Windows.Forms.Button();
			this.label21 = new System.Windows.Forms.Label();
			this.label22 = new System.Windows.Forms.Label();
			this.label20 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupProcCodeSetup.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(820, 665);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 20;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(900, 665);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 21;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butEditFeeSched
			// 
			this.butEditFeeSched.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditFeeSched.Location = new System.Drawing.Point(790, 630);
			this.butEditFeeSched.Name = "butEditFeeSched";
			this.butEditFeeSched.Size = new System.Drawing.Size(81, 26);
			this.butEditFeeSched.TabIndex = 18;
			this.butEditFeeSched.Text = "Fee Scheds";
			this.butEditFeeSched.Click += new System.EventHandler(this.butEditFeeSched_Click);
			// 
			// butTools
			// 
			this.butTools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butTools.Location = new System.Drawing.Point(876, 630);
			this.butTools.Name = "butTools";
			this.butTools.Size = new System.Drawing.Size(81, 26);
			this.butTools.TabIndex = 19;
			this.butTools.Text = "Fee Tools";
			this.butTools.Click += new System.EventHandler(this.butTools_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(2, 42);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(79, 20);
			this.label2.TabIndex = 17;
			this.label2.Text = "By Descript";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listCategories
			// 
			this.listCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listCategories.Location = new System.Drawing.Point(10, 149);
			this.listCategories.Name = "listCategories";
			this.listCategories.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listCategories.Size = new System.Drawing.Size(145, 329);
			this.listCategories.TabIndex = 15;
			this.listCategories.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listCategories_MouseUp);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(2, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(79, 20);
			this.label3.TabIndex = 19;
			this.label3.Text = "By Code";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 123);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 23);
			this.label1.TabIndex = 16;
			this.label1.Text = "By Category";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butEditCategories
			// 
			this.butEditCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEditCategories.Location = new System.Drawing.Point(10, 499);
			this.butEditCategories.Name = "butEditCategories";
			this.butEditCategories.Size = new System.Drawing.Size(94, 26);
			this.butEditCategories.TabIndex = 23;
			this.butEditCategories.Text = "Edit Categories";
			this.butEditCategories.Click += new System.EventHandler(this.butEditCategories_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkShowHidden.Location = new System.Drawing.Point(10, 531);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(90, 17);
			this.checkShowHidden.TabIndex = 24;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.UseVisualStyleBackColor = true;
			this.checkShowHidden.Click += new System.EventHandler(this.checkShowHidden_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(2, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(79, 20);
			this.label4.TabIndex = 22;
			this.label4.Text = "By Abbrev";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butAll
			// 
			this.butAll.Location = new System.Drawing.Point(93, 123);
			this.butAll.Name = "butAll";
			this.butAll.Size = new System.Drawing.Size(62, 25);
			this.butAll.TabIndex = 22;
			this.butAll.Text = "All";
			this.butAll.Click += new System.EventHandler(this.butAll_Click);
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(82, 69);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(73, 20);
			this.textCode.TabIndex = 2;
			this.textCode.TextChanged += new System.EventHandler(this.textCode_TextChanged);
			// 
			// textAbbreviation
			// 
			this.textAbbreviation.Location = new System.Drawing.Point(82, 17);
			this.textAbbreviation.Name = "textAbbreviation";
			this.textAbbreviation.Size = new System.Drawing.Size(73, 20);
			this.textAbbreviation.TabIndex = 0;
			this.textAbbreviation.TextChanged += new System.EventHandler(this.textAbbreviation_TextChanged);
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(82, 43);
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(73, 20);
			this.textDescription.TabIndex = 1;
			this.textDescription.TextChanged += new System.EventHandler(this.textDescription_TextChanged);
			// 
			// butShowHiddenDefault
			// 
			this.butShowHiddenDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butShowHiddenDefault.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butShowHiddenDefault.Location = new System.Drawing.Point(100, 528);
			this.butShowHiddenDefault.Name = "butShowHiddenDefault";
			this.butShowHiddenDefault.Size = new System.Drawing.Size(56, 20);
			this.butShowHiddenDefault.TabIndex = 25;
			this.butShowHiddenDefault.Text = "default";
			this.butShowHiddenDefault.Click += new System.EventHandler(this.butShowHiddenDefault_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.comboSort);
			this.groupBox1.Controls.Add(this.butShowHiddenDefault);
			this.groupBox1.Controls.Add(this.textDescription);
			this.groupBox1.Controls.Add(this.textAbbreviation);
			this.groupBox1.Controls.Add(this.textCode);
			this.groupBox1.Controls.Add(this.butAll);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.checkShowHidden);
			this.groupBox1.Controls.Add(this.butEditCategories);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.listCategories);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(2, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(165, 560);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(2, 94);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(79, 20);
			this.label6.TabIndex = 34;
			this.label6.Text = "Sort Order";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSort
			// 
			this.comboSort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSort.FormattingEnabled = true;
			this.comboSort.Location = new System.Drawing.Point(82, 95);
			this.comboSort.Name = "comboSort";
			this.comboSort.Size = new System.Drawing.Size(73, 21);
			this.comboSort.TabIndex = 33;
			this.comboSort.SelectionChangeCommitted += new System.EventHandler(this.comboSort_SelectionChangeCommitted);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.EditableEnterMovesDown = true;
			this.gridMain.Location = new System.Drawing.Point(170, 8);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridMain.Size = new System.Drawing.Size(604, 675);
			this.gridMain.TabIndex = 19;
			this.gridMain.Title = "Procedures";
			this.gridMain.TranslationName = "TableProcedures";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellLeave);
			this.gridMain.CellEnter += new OpenDental.UI.ODGridClickEventHandler(this.GridMain_CellEnter);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(779, 2);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(199, 17);
			this.label5.TabIndex = 21;
			this.label5.Text = "Compare Fee Schedules";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butNew
			// 
			this.butNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butNew.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNew.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNew.Location = new System.Drawing.Point(85, 57);
			this.butNew.Name = "butNew";
			this.butNew.Size = new System.Drawing.Size(75, 26);
			this.butNew.TabIndex = 29;
			this.butNew.Text = "&New";
			this.butNew.Click += new System.EventHandler(this.butNew_Click);
			// 
			// butExport
			// 
			this.butExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butExport.Location = new System.Drawing.Point(85, 19);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75, 26);
			this.butExport.TabIndex = 27;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// butImport
			// 
			this.butImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butImport.Location = new System.Drawing.Point(6, 19);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(75, 26);
			this.butImport.TabIndex = 26;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butProcTools
			// 
			this.butProcTools.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butProcTools.Location = new System.Drawing.Point(6, 57);
			this.butProcTools.Name = "butProcTools";
			this.butProcTools.Size = new System.Drawing.Size(75, 26);
			this.butProcTools.TabIndex = 28;
			this.butProcTools.Text = "Tools";
			this.butProcTools.Click += new System.EventHandler(this.butProcTools_Click);
			// 
			// groupProcCodeSetup
			// 
			this.groupProcCodeSetup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.groupProcCodeSetup.Controls.Add(this.butProcTools);
			this.groupProcCodeSetup.Controls.Add(this.butImport);
			this.groupProcCodeSetup.Controls.Add(this.butExport);
			this.groupProcCodeSetup.Controls.Add(this.butNew);
			this.groupProcCodeSetup.Location = new System.Drawing.Point(2, 592);
			this.groupProcCodeSetup.Name = "groupProcCodeSetup";
			this.groupProcCodeSetup.Size = new System.Drawing.Size(165, 91);
			this.groupProcCodeSetup.TabIndex = 26;
			this.groupProcCodeSetup.TabStop = false;
			this.groupProcCodeSetup.Text = "Procedure Codes";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.comboFeeSchedGroup1);
			this.groupBox2.Controls.Add(this.checkGroups1);
			this.groupBox2.Controls.Add(this.butPickProv1);
			this.groupBox2.Controls.Add(this.butPickClinic1);
			this.groupBox2.Controls.Add(this.butPickSched1);
			this.groupBox2.Controls.Add(this.labelSched1);
			this.groupBox2.Controls.Add(this.labelClinic1);
			this.groupBox2.Controls.Add(this.labelProvider1);
			this.groupBox2.Controls.Add(this.comboProvider1);
			this.groupBox2.Controls.Add(this.comboClinic1);
			this.groupBox2.Controls.Add(this.comboFeeSched1);
			this.groupBox2.Location = new System.Drawing.Point(780, 24);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 175);
			this.groupBox2.TabIndex = 27;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Fee 1";
			// 
			// comboFeeSchedGroup1
			// 
			this.comboFeeSchedGroup1.BackColor = System.Drawing.SystemColors.Window;
			this.comboFeeSchedGroup1.ForeColor = System.Drawing.SystemColors.WindowText;
			this.comboFeeSchedGroup1.Location = new System.Drawing.Point(157, 81);
			this.comboFeeSchedGroup1.Name = "comboFeeSchedGroup1";
			this.comboFeeSchedGroup1.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSchedGroup1.TabIndex = 41;
			this.comboFeeSchedGroup1.Visible = false;
			this.comboFeeSchedGroup1.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedGroup_SelectionChangeCommitted);
			// 
			// checkGroups1
			// 
			this.checkGroups1.Location = new System.Drawing.Point(14, 16);
			this.checkGroups1.Name = "checkGroups1";
			this.checkGroups1.Size = new System.Drawing.Size(176, 17);
			this.checkGroups1.TabIndex = 40;
			this.checkGroups1.Text = "Show Fee Schedule Groups";
			this.checkGroups1.UseVisualStyleBackColor = true;
			this.checkGroups1.CheckedChanged += new System.EventHandler(this.checkGroups1_CheckedChanged);
			// 
			// butPickProv1
			// 
			this.butPickProv1.Location = new System.Drawing.Point(167, 136);
			this.butPickProv1.Name = "butPickProv1";
			this.butPickProv1.Size = new System.Drawing.Size(23, 21);
			this.butPickProv1.TabIndex = 5;
			this.butPickProv1.Text = "...";
			this.butPickProv1.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// butPickClinic1
			// 
			this.butPickClinic1.Location = new System.Drawing.Point(167, 96);
			this.butPickClinic1.Name = "butPickClinic1";
			this.butPickClinic1.Size = new System.Drawing.Size(23, 21);
			this.butPickClinic1.TabIndex = 3;
			this.butPickClinic1.Text = "...";
			this.butPickClinic1.Click += new System.EventHandler(this.butPickClinic_Click);
			// 
			// butPickSched1
			// 
			this.butPickSched1.Location = new System.Drawing.Point(167, 54);
			this.butPickSched1.Name = "butPickSched1";
			this.butPickSched1.Size = new System.Drawing.Size(23, 21);
			this.butPickSched1.TabIndex = 1;
			this.butPickSched1.Text = "...";
			this.butPickSched1.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// labelSched1
			// 
			this.labelSched1.Location = new System.Drawing.Point(14, 36);
			this.labelSched1.Name = "labelSched1";
			this.labelSched1.Size = new System.Drawing.Size(174, 17);
			this.labelSched1.TabIndex = 32;
			this.labelSched1.Text = "Fee Schedule";
			this.labelSched1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelClinic1
			// 
			this.labelClinic1.Location = new System.Drawing.Point(14, 78);
			this.labelClinic1.Name = "labelClinic1";
			this.labelClinic1.Size = new System.Drawing.Size(174, 17);
			this.labelClinic1.TabIndex = 30;
			this.labelClinic1.Text = "Clinic";
			this.labelClinic1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvider1
			// 
			this.labelProvider1.Location = new System.Drawing.Point(14, 118);
			this.labelProvider1.Name = "labelProvider1";
			this.labelProvider1.Size = new System.Drawing.Size(174, 17);
			this.labelProvider1.TabIndex = 28;
			this.labelProvider1.Text = "Provider";
			this.labelProvider1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboProvider1
			// 
			this.comboProvider1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvider1.FormattingEnabled = true;
			this.comboProvider1.Location = new System.Drawing.Point(14, 136);
			this.comboProvider1.Name = "comboProvider1";
			this.comboProvider1.Size = new System.Drawing.Size(151, 21);
			this.comboProvider1.TabIndex = 4;
			this.comboProvider1.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// comboClinic1
			// 
			this.comboClinic1.ForceShowUnassigned = true;
			this.comboClinic1.IncludeUnassigned = true;
			this.comboClinic1.Location = new System.Drawing.Point(14, 96);
			this.comboClinic1.Name = "comboClinic1";
			this.comboClinic1.ShowLabel = false;
			this.comboClinic1.Size = new System.Drawing.Size(151, 21);
			this.comboClinic1.TabIndex = 2;
			this.comboClinic1.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// comboFeeSched1
			// 
			this.comboFeeSched1.Location = new System.Drawing.Point(14, 54);
			this.comboFeeSched1.Name = "comboFeeSched1";
			this.comboFeeSched1.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSched1.TabIndex = 0;
			this.comboFeeSched1.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSched_SelectionChangeCommitted);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.comboFeeSchedGroup2);
			this.groupBox3.Controls.Add(this.checkGroups2);
			this.groupBox3.Controls.Add(this.butPickProv2);
			this.groupBox3.Controls.Add(this.butPickClinic2);
			this.groupBox3.Controls.Add(this.butPickSched2);
			this.groupBox3.Controls.Add(this.labelSched2);
			this.groupBox3.Controls.Add(this.comboProvider2);
			this.groupBox3.Controls.Add(this.labelClinic2);
			this.groupBox3.Controls.Add(this.comboClinic2);
			this.groupBox3.Controls.Add(this.labelProvider2);
			this.groupBox3.Controls.Add(this.comboFeeSched2);
			this.groupBox3.Location = new System.Drawing.Point(780, 202);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(200, 175);
			this.groupBox3.TabIndex = 28;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Fee 2";
			// 
			// comboFeeSchedGroup2
			// 
			this.comboFeeSchedGroup2.BackColor = System.Drawing.SystemColors.Window;
			this.comboFeeSchedGroup2.ForeColor = System.Drawing.SystemColors.WindowText;
			this.comboFeeSchedGroup2.Location = new System.Drawing.Point(157, 81);
			this.comboFeeSchedGroup2.Name = "comboFeeSchedGroup2";
			this.comboFeeSchedGroup2.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSchedGroup2.TabIndex = 42;
			this.comboFeeSchedGroup2.Visible = false;
			this.comboFeeSchedGroup2.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedGroup_SelectionChangeCommitted);
			// 
			// checkGroups2
			// 
			this.checkGroups2.Location = new System.Drawing.Point(14, 16);
			this.checkGroups2.Name = "checkGroups2";
			this.checkGroups2.Size = new System.Drawing.Size(173, 17);
			this.checkGroups2.TabIndex = 39;
			this.checkGroups2.Text = "Show Fee Schedule Groups";
			this.checkGroups2.UseVisualStyleBackColor = true;
			this.checkGroups2.CheckedChanged += new System.EventHandler(this.checkGroups2_CheckedChanged);
			// 
			// butPickProv2
			// 
			this.butPickProv2.Location = new System.Drawing.Point(167, 136);
			this.butPickProv2.Name = "butPickProv2";
			this.butPickProv2.Size = new System.Drawing.Size(23, 21);
			this.butPickProv2.TabIndex = 11;
			this.butPickProv2.Text = "...";
			this.butPickProv2.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// butPickClinic2
			// 
			this.butPickClinic2.Location = new System.Drawing.Point(167, 96);
			this.butPickClinic2.Name = "butPickClinic2";
			this.butPickClinic2.Size = new System.Drawing.Size(23, 21);
			this.butPickClinic2.TabIndex = 9;
			this.butPickClinic2.Text = "...";
			this.butPickClinic2.Click += new System.EventHandler(this.butPickClinic_Click);
			// 
			// butPickSched2
			// 
			this.butPickSched2.Location = new System.Drawing.Point(167, 54);
			this.butPickSched2.Name = "butPickSched2";
			this.butPickSched2.Size = new System.Drawing.Size(23, 21);
			this.butPickSched2.TabIndex = 7;
			this.butPickSched2.Text = "...";
			this.butPickSched2.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// labelSched2
			// 
			this.labelSched2.Location = new System.Drawing.Point(14, 36);
			this.labelSched2.Name = "labelSched2";
			this.labelSched2.Size = new System.Drawing.Size(174, 17);
			this.labelSched2.TabIndex = 35;
			this.labelSched2.Text = "Fee Schedule";
			this.labelSched2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboProvider2
			// 
			this.comboProvider2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvider2.FormattingEnabled = true;
			this.comboProvider2.Location = new System.Drawing.Point(14, 136);
			this.comboProvider2.Name = "comboProvider2";
			this.comboProvider2.Size = new System.Drawing.Size(151, 21);
			this.comboProvider2.TabIndex = 10;
			this.comboProvider2.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// labelClinic2
			// 
			this.labelClinic2.Location = new System.Drawing.Point(14, 78);
			this.labelClinic2.Name = "labelClinic2";
			this.labelClinic2.Size = new System.Drawing.Size(174, 17);
			this.labelClinic2.TabIndex = 34;
			this.labelClinic2.Text = "Clinic";
			this.labelClinic2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboClinic2
			// 
			this.comboClinic2.ForceShowUnassigned = true;
			this.comboClinic2.IncludeUnassigned = true;
			this.comboClinic2.Location = new System.Drawing.Point(14, 96);
			this.comboClinic2.Name = "comboClinic2";
			this.comboClinic2.ShowLabel = false;
			this.comboClinic2.Size = new System.Drawing.Size(151, 21);
			this.comboClinic2.TabIndex = 8;
			this.comboClinic2.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// labelProvider2
			// 
			this.labelProvider2.Location = new System.Drawing.Point(14, 118);
			this.labelProvider2.Name = "labelProvider2";
			this.labelProvider2.Size = new System.Drawing.Size(174, 17);
			this.labelProvider2.TabIndex = 33;
			this.labelProvider2.Text = "Provider";
			this.labelProvider2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboFeeSched2
			// 
			this.comboFeeSched2.Location = new System.Drawing.Point(14, 54);
			this.comboFeeSched2.Name = "comboFeeSched2";
			this.comboFeeSched2.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSched2.TabIndex = 6;
			this.comboFeeSched2.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSched_SelectionChangeCommitted);
			// 
			// groupBox4
			// 
			this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox4.Controls.Add(this.comboFeeSchedGroup3);
			this.groupBox4.Controls.Add(this.checkGroups3);
			this.groupBox4.Controls.Add(this.butPickProv3);
			this.groupBox4.Controls.Add(this.butPickClinic3);
			this.groupBox4.Controls.Add(this.butPickSched3);
			this.groupBox4.Controls.Add(this.labelSched3);
			this.groupBox4.Controls.Add(this.comboProvider3);
			this.groupBox4.Controls.Add(this.labelClinic3);
			this.groupBox4.Controls.Add(this.labelProvider3);
			this.groupBox4.Controls.Add(this.comboClinic3);
			this.groupBox4.Controls.Add(this.comboFeeSched3);
			this.groupBox4.Location = new System.Drawing.Point(780, 380);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(200, 175);
			this.groupBox4.TabIndex = 29;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Fee 3";
			// 
			// comboFeeSchedGroup3
			// 
			this.comboFeeSchedGroup3.BackColor = System.Drawing.SystemColors.Window;
			this.comboFeeSchedGroup3.ForeColor = System.Drawing.SystemColors.WindowText;
			this.comboFeeSchedGroup3.Location = new System.Drawing.Point(157, 81);
			this.comboFeeSchedGroup3.Name = "comboFeeSchedGroup3";
			this.comboFeeSchedGroup3.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSchedGroup3.TabIndex = 42;
			this.comboFeeSchedGroup3.Visible = false;
			this.comboFeeSchedGroup3.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSchedGroup_SelectionChangeCommitted);
			// 
			// checkGroups3
			// 
			this.checkGroups3.Location = new System.Drawing.Point(14, 16);
			this.checkGroups3.Name = "checkGroups3";
			this.checkGroups3.Size = new System.Drawing.Size(176, 17);
			this.checkGroups3.TabIndex = 40;
			this.checkGroups3.Text = "Show Fee Schedule Groups";
			this.checkGroups3.UseVisualStyleBackColor = true;
			this.checkGroups3.CheckedChanged += new System.EventHandler(this.checkGroups3_CheckedChanged);
			// 
			// butPickProv3
			// 
			this.butPickProv3.Location = new System.Drawing.Point(167, 136);
			this.butPickProv3.Name = "butPickProv3";
			this.butPickProv3.Size = new System.Drawing.Size(23, 21);
			this.butPickProv3.TabIndex = 17;
			this.butPickProv3.Text = "...";
			this.butPickProv3.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// butPickClinic3
			// 
			this.butPickClinic3.Location = new System.Drawing.Point(167, 96);
			this.butPickClinic3.Name = "butPickClinic3";
			this.butPickClinic3.Size = new System.Drawing.Size(23, 21);
			this.butPickClinic3.TabIndex = 15;
			this.butPickClinic3.Text = "...";
			this.butPickClinic3.Click += new System.EventHandler(this.butPickClinic_Click);
			// 
			// butPickSched3
			// 
			this.butPickSched3.Location = new System.Drawing.Point(167, 54);
			this.butPickSched3.Name = "butPickSched3";
			this.butPickSched3.Size = new System.Drawing.Size(23, 21);
			this.butPickSched3.TabIndex = 13;
			this.butPickSched3.Text = "...";
			this.butPickSched3.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// labelSched3
			// 
			this.labelSched3.Location = new System.Drawing.Point(14, 36);
			this.labelSched3.Name = "labelSched3";
			this.labelSched3.Size = new System.Drawing.Size(174, 17);
			this.labelSched3.TabIndex = 38;
			this.labelSched3.Text = "Fee Schedule";
			this.labelSched3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboProvider3
			// 
			this.comboProvider3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvider3.FormattingEnabled = true;
			this.comboProvider3.Location = new System.Drawing.Point(14, 136);
			this.comboProvider3.Name = "comboProvider3";
			this.comboProvider3.Size = new System.Drawing.Size(151, 21);
			this.comboProvider3.TabIndex = 16;
			this.comboProvider3.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// labelClinic3
			// 
			this.labelClinic3.Location = new System.Drawing.Point(14, 78);
			this.labelClinic3.Name = "labelClinic3";
			this.labelClinic3.Size = new System.Drawing.Size(174, 17);
			this.labelClinic3.TabIndex = 37;
			this.labelClinic3.Text = "Clinic";
			this.labelClinic3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvider3
			// 
			this.labelProvider3.Location = new System.Drawing.Point(14, 118);
			this.labelProvider3.Name = "labelProvider3";
			this.labelProvider3.Size = new System.Drawing.Size(174, 17);
			this.labelProvider3.TabIndex = 36;
			this.labelProvider3.Text = "Provider";
			this.labelProvider3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboClinic3
			// 
			this.comboClinic3.ForceShowUnassigned = true;
			this.comboClinic3.IncludeUnassigned = true;
			this.comboClinic3.Location = new System.Drawing.Point(14, 96);
			this.comboClinic3.Name = "comboClinic3";
			this.comboClinic3.ShowLabel = false;
			this.comboClinic3.Size = new System.Drawing.Size(151, 21);
			this.comboClinic3.TabIndex = 14;
			this.comboClinic3.SelectionChangeCommitted += new System.EventHandler(this.comboClinicProv_SelectionChangeCommitted);
			// 
			// comboFeeSched3
			// 
			this.comboFeeSched3.Location = new System.Drawing.Point(14, 54);
			this.comboFeeSched3.Name = "comboFeeSched3";
			this.comboFeeSched3.Size = new System.Drawing.Size(151, 21);
			this.comboFeeSched3.TabIndex = 12;
			this.comboFeeSched3.SelectionChangeCommitted += new System.EventHandler(this.comboFeeSched_SelectionChangeCommitted);
			// 
			// groupBox5
			// 
			this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox5.Controls.Add(this.butColorClinicProv);
			this.groupBox5.Controls.Add(this.butColorProvider);
			this.groupBox5.Controls.Add(this.butColorClinic);
			this.groupBox5.Controls.Add(this.butColorDefault);
			this.groupBox5.Controls.Add(this.label21);
			this.groupBox5.Controls.Add(this.label22);
			this.groupBox5.Controls.Add(this.label20);
			this.groupBox5.Controls.Add(this.label19);
			this.groupBox5.Location = new System.Drawing.Point(780, 558);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(200, 70);
			this.groupBox5.TabIndex = 30;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Fee Colors";
			// 
			// butColorClinicProv
			// 
			this.butColorClinicProv.Enabled = false;
			this.butColorClinicProv.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butColorClinicProv.Location = new System.Drawing.Point(89, 44);
			this.butColorClinicProv.Name = "butColorClinicProv";
			this.butColorClinicProv.Size = new System.Drawing.Size(20, 20);
			this.butColorClinicProv.TabIndex = 163;
			// 
			// butColorProvider
			// 
			this.butColorProvider.Enabled = false;
			this.butColorProvider.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butColorProvider.Location = new System.Drawing.Point(10, 44);
			this.butColorProvider.Name = "butColorProvider";
			this.butColorProvider.Size = new System.Drawing.Size(20, 20);
			this.butColorProvider.TabIndex = 162;
			// 
			// butColorClinic
			// 
			this.butColorClinic.Enabled = false;
			this.butColorClinic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butColorClinic.Location = new System.Drawing.Point(89, 18);
			this.butColorClinic.Name = "butColorClinic";
			this.butColorClinic.Size = new System.Drawing.Size(20, 20);
			this.butColorClinic.TabIndex = 161;
			// 
			// butColorDefault
			// 
			this.butColorDefault.Enabled = false;
			this.butColorDefault.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butColorDefault.Location = new System.Drawing.Point(10, 17);
			this.butColorDefault.Name = "butColorDefault";
			this.butColorDefault.Size = new System.Drawing.Size(20, 20);
			this.butColorDefault.TabIndex = 160;
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(110, 19);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(48, 17);
			this.label21.TabIndex = 48;
			this.label21.Text = "= Clinic";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(31, 47);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(55, 17);
			this.label22.TabIndex = 46;
			this.label22.Text = "= Provider";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(110, 47);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(88, 17);
			this.label20.TabIndex = 44;
			this.label20.Text = "= Provider+Clinic";
			this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(31, 19);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(60, 17);
			this.label19.TabIndex = 43;
			this.label19.Text = "= Default";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormProcCodes
			// 
			this.ClientSize = new System.Drawing.Size(982, 696);
			this.Controls.Add(this.butTools);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.butEditFeeSched);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupProcCodeSetup);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormProcCodes";
			this.ShowInTaskbar = false;
			this.Text = "Procedure Codes - Fee Schedules";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormProcedures_Closing);
			this.Load += new System.EventHandler(this.FormProcCodes_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupProcCodeSetup.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private UI.Button butOK;
		private UI.Button butCancel;
		private UI.Button butEditFeeSched;
		private UI.Button butTools;
		private Label label2;
		private UI.ListBoxOD listCategories;
		private Label label3;
		private Label label1;
		private UI.Button butEditCategories;
		private CheckBox checkShowHidden;
		private Label label4;
		private UI.Button butAll;
		private TextBox textCode;
		private TextBox textAbbreviation;
		private TextBox textDescription;
		private UI.Button butShowHiddenDefault;
		private GroupBox groupBox1;
		private UI.GridOD gridMain;
		private Label label5;
		private UI.Button butNew;
		private UI.Button butExport;
		private UI.Button butImport;
		private UI.Button butProcTools;
		private GroupBox groupProcCodeSetup;
		private GroupBox groupBox2;
		private ComboBox comboProvider1;
		private UI.ComboBoxClinicPicker comboClinic1;
		private UI.ComboBoxOD comboFeeSched1;
		private GroupBox groupBox3;
		private ComboBox comboProvider2;
		private UI.ComboBoxClinicPicker comboClinic2;
		private UI.ComboBoxOD comboFeeSched2;
		private GroupBox groupBox4;
		private ComboBox comboProvider3;
		private UI.ComboBoxClinicPicker comboClinic3;
		private UI.ComboBoxOD comboFeeSched3;
		private Label labelSched1;
		private Label labelClinic1;
		private Label labelProvider1;
		private Label labelSched2;
		private Label labelClinic2;
		private Label labelProvider2;
		private Label labelSched3;
		private Label labelClinic3;
		private Label labelProvider3;
		private GroupBox groupBox5;
		private Label label22;
		private Label label20;
		private Label label19;
		private Label label21;
		private UI.Button butPickProv1;
		private UI.Button butPickClinic1;
		private UI.Button butPickSched1;
		private UI.Button butPickProv2;
		private UI.Button butPickClinic2;
		private UI.Button butPickSched2;
		private UI.Button butPickProv3;
		private UI.Button butPickClinic3;
		private UI.Button butPickSched3;
		private System.Windows.Forms.Button butColorClinicProv;
		private System.Windows.Forms.Button butColorProvider;
		private System.Windows.Forms.Button butColorClinic;
		private System.Windows.Forms.Button butColorDefault;
		private ComboBox comboSort;
		private Label label6;
		private CheckBox checkGroups1;
		private CheckBox checkGroups2;
		private CheckBox checkGroups3;
		private UI.ComboBoxOD comboFeeSchedGroup1;
		private UI.ComboBoxOD comboFeeSchedGroup2;
		private UI.ComboBoxOD comboFeeSchedGroup3;
	}
}
