using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFeeSchedTools {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedTools));
			this.checkShowGroups = new System.Windows.Forms.CheckBox();
			this.comboClinicTo = new OpenDental.UI.ComboBoxClinicPicker();
			this.groupBox7 = new System.Windows.Forms.GroupBox();
			this.comboProvider = new System.Windows.Forms.ComboBox();
			this.comboGroup = new OpenDental.UI.ComboBoxOD();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboFeeSched = new System.Windows.Forms.ComboBox();
			this.butPickProv = new OpenDental.UI.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.butPickGroup = new OpenDental.UI.Button();
			this.butPickSched = new OpenDental.UI.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.labelGroup = new System.Windows.Forms.Label();
			this.groupGlobalUpdateFees = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.butUpdateWriteoffs = new OpenDental.UI.Button();
			this.comboGlobalUpdateClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.label4 = new System.Windows.Forms.Label();
			this.butUpdate = new OpenDental.UI.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butImportCanada = new OpenDental.UI.Button();
			this.butImport = new OpenDental.UI.Button();
			this.butExport = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioPenny = new System.Windows.Forms.RadioButton();
			this.radioDime = new System.Windows.Forms.RadioButton();
			this.radioDollar = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.butIncrease = new OpenDental.UI.Button();
			this.textPercent = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label11 = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butPickGroupTo = new OpenDental.UI.Button();
			this.labelGroupTo = new System.Windows.Forms.Label();
			this.comboGroupTo = new OpenDental.UI.ComboBoxOD();
			this.comboProviderTo = new System.Windows.Forms.ComboBox();
			this.comboFeeSchedTo = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butPickProvTo = new OpenDental.UI.Button();
			this.butPickSchedTo = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox7.SuspendLayout();
			this.groupGlobalUpdateFees.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkShowGroups
			// 
			this.checkShowGroups.Location = new System.Drawing.Point(12, 8);
			this.checkShowGroups.Name = "checkShowGroups";
			this.checkShowGroups.Size = new System.Drawing.Size(173, 17);
			this.checkShowGroups.TabIndex = 40;
			this.checkShowGroups.Text = "Show Fee Schedule Groups";
			this.checkShowGroups.UseVisualStyleBackColor = true;
			this.checkShowGroups.Visible = false;
			this.checkShowGroups.CheckedChanged += new System.EventHandler(this.checkShowGroups_CheckedChanged);
			// 
			// comboClinicTo
			// 
			this.comboClinicTo.HqDescription = "Default";
			this.comboClinicTo.IncludeUnassigned = true;
			this.comboClinicTo.Location = new System.Drawing.Point(63, 45);
			this.comboClinicTo.Name = "comboClinicTo";
			this.comboClinicTo.SelectionModeMulti = true;
			this.comboClinicTo.Size = new System.Drawing.Size(210, 21);
			this.comboClinicTo.TabIndex = 2;
			// 
			// groupBox7
			// 
			this.groupBox7.Controls.Add(this.comboProvider);
			this.groupBox7.Controls.Add(this.comboGroup);
			this.groupBox7.Controls.Add(this.comboClinic);
			this.groupBox7.Controls.Add(this.comboFeeSched);
			this.groupBox7.Controls.Add(this.butPickProv);
			this.groupBox7.Controls.Add(this.label12);
			this.groupBox7.Controls.Add(this.butPickGroup);
			this.groupBox7.Controls.Add(this.butPickSched);
			this.groupBox7.Controls.Add(this.label8);
			this.groupBox7.Controls.Add(this.labelGroup);
			this.groupBox7.Location = new System.Drawing.Point(12, 31);
			this.groupBox7.Name = "groupBox7";
			this.groupBox7.Size = new System.Drawing.Size(328, 107);
			this.groupBox7.TabIndex = 0;
			this.groupBox7.TabStop = false;
			this.groupBox7.Text = "Select Fees";
			// 
			// comboProvider
			// 
			this.comboProvider.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProvider.FormattingEnabled = true;
			this.comboProvider.Location = new System.Drawing.Point(100, 72);
			this.comboProvider.Name = "comboProvider";
			this.comboProvider.Size = new System.Drawing.Size(174, 21);
			this.comboProvider.TabIndex = 4;
			// 
			// comboGroup
			// 
			this.comboGroup.Location = new System.Drawing.Point(-160, 43);
			this.comboGroup.Name = "comboGroup";
			this.comboGroup.Size = new System.Drawing.Size(174, 21);
			this.comboGroup.TabIndex = 41;
			this.comboGroup.Visible = false;
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Default";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(63, 46);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(211, 21);
			this.comboClinic.TabIndex = 2;
			// 
			// comboFeeSched
			// 
			this.comboFeeSched.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFeeSched.FormattingEnabled = true;
			this.comboFeeSched.Location = new System.Drawing.Point(100, 19);
			this.comboFeeSched.Name = "comboFeeSched";
			this.comboFeeSched.Size = new System.Drawing.Size(174, 21);
			this.comboFeeSched.TabIndex = 0;
			this.comboFeeSched.SelectionChangeCommitted += new System.EventHandler(this.comboFeeCombos_SelectionChangeCommitted);
			// 
			// butPickProv
			// 
			this.butPickProv.Location = new System.Drawing.Point(279, 72);
			this.butPickProv.Name = "butPickProv";
			this.butPickProv.Size = new System.Drawing.Size(23, 21);
			this.butPickProv.TabIndex = 5;
			this.butPickProv.Text = "...";
			this.butPickProv.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(14, 20);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(87, 17);
			this.label12.TabIndex = 35;
			this.label12.Text = "Fee Schedule";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickGroup
			// 
			this.butPickGroup.Location = new System.Drawing.Point(279, 46);
			this.butPickGroup.Name = "butPickGroup";
			this.butPickGroup.Size = new System.Drawing.Size(23, 21);
			this.butPickGroup.TabIndex = 3;
			this.butPickGroup.Text = "...";
			this.butPickGroup.Visible = false;
			this.butPickGroup.Click += new System.EventHandler(this.butPickGroup_Click);
			// 
			// butPickSched
			// 
			this.butPickSched.Location = new System.Drawing.Point(279, 19);
			this.butPickSched.Name = "butPickSched";
			this.butPickSched.Size = new System.Drawing.Size(23, 21);
			this.butPickSched.TabIndex = 1;
			this.butPickSched.Text = "...";
			this.butPickSched.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(11, 73);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(90, 17);
			this.label8.TabIndex = 33;
			this.label8.Text = "Provider";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelGroup
			// 
			this.labelGroup.Location = new System.Drawing.Point(9, 47);
			this.labelGroup.Name = "labelGroup";
			this.labelGroup.Size = new System.Drawing.Size(90, 17);
			this.labelGroup.TabIndex = 34;
			this.labelGroup.Text = "Group";
			this.labelGroup.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelGroup.Visible = false;
			// 
			// groupGlobalUpdateFees
			// 
			this.groupGlobalUpdateFees.Controls.Add(this.label9);
			this.groupGlobalUpdateFees.Controls.Add(this.butUpdateWriteoffs);
			this.groupGlobalUpdateFees.Controls.Add(this.comboGlobalUpdateClinics);
			this.groupGlobalUpdateFees.Controls.Add(this.label4);
			this.groupGlobalUpdateFees.Controls.Add(this.butUpdate);
			this.groupGlobalUpdateFees.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupGlobalUpdateFees.Location = new System.Drawing.Point(368, 209);
			this.groupGlobalUpdateFees.Name = "groupGlobalUpdateFees";
			this.groupGlobalUpdateFees.Size = new System.Drawing.Size(248, 230);
			this.groupGlobalUpdateFees.TabIndex = 5;
			this.groupGlobalUpdateFees.TabStop = false;
			this.groupGlobalUpdateFees.Text = "Global Updates";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 114);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(208, 77);
			this.label9.TabIndex = 6;
			this.label9.Text = "Only for offices that run reports after updating fee schedules and before selecti" +
    "ng patients. Recalculates treatment planned estimates, write-offs, ortho cases, " +
    "and discount plan amounts.";
			this.label9.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butUpdateWriteoffs
			// 
			this.butUpdateWriteoffs.Location = new System.Drawing.Point(6, 194);
			this.butUpdateWriteoffs.Name = "butUpdateWriteoffs";
			this.butUpdateWriteoffs.Size = new System.Drawing.Size(130, 24);
			this.butUpdateWriteoffs.TabIndex = 3;
			this.butUpdateWriteoffs.Text = "Update Estimates Only";
			this.butUpdateWriteoffs.Click += new System.EventHandler(this.butUpdateWriteoffs_Click);
			// 
			// comboGlobalUpdateClinics
			// 
			this.comboGlobalUpdateClinics.IncludeAll = true;
			this.comboGlobalUpdateClinics.IncludeUnassigned = true;
			this.comboGlobalUpdateClinics.Location = new System.Drawing.Point(6, 26);
			this.comboGlobalUpdateClinics.Name = "comboGlobalUpdateClinics";
			this.comboGlobalUpdateClinics.SelectionModeMulti = true;
			this.comboGlobalUpdateClinics.Size = new System.Drawing.Size(231, 21);
			this.comboGlobalUpdateClinics.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 60);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(208, 18);
			this.label4.TabIndex = 5;
			this.label4.Text = "Update fees for all patients";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butUpdate
			// 
			this.butUpdate.Location = new System.Drawing.Point(6, 84);
			this.butUpdate.Name = "butUpdate";
			this.butUpdate.Size = new System.Drawing.Size(130, 24);
			this.butUpdate.TabIndex = 2;
			this.butUpdate.Text = "Update Proc Fees Only";
			this.butUpdate.Click += new System.EventHandler(this.butUpdateFees_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.butImportCanada);
			this.groupBox5.Controls.Add(this.butImport);
			this.groupBox5.Controls.Add(this.butExport);
			this.groupBox5.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox5.Location = new System.Drawing.Point(12, 302);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(214, 81);
			this.groupBox5.TabIndex = 3;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Export/Import";
			// 
			// butImportCanada
			// 
			this.butImportCanada.Location = new System.Drawing.Point(87, 51);
			this.butImportCanada.Name = "butImportCanada";
			this.butImportCanada.Size = new System.Drawing.Size(84, 24);
			this.butImportCanada.TabIndex = 2;
			this.butImportCanada.Text = "Import Canada";
			this.butImportCanada.Click += new System.EventHandler(this.butImportCanada_Click);
			// 
			// butImport
			// 
			this.butImport.Location = new System.Drawing.Point(87, 21);
			this.butImport.Name = "butImport";
			this.butImport.Size = new System.Drawing.Size(84, 24);
			this.butImport.TabIndex = 1;
			this.butImport.Text = "Import";
			this.butImport.Click += new System.EventHandler(this.butImport_Click);
			// 
			// butExport
			// 
			this.butExport.Location = new System.Drawing.Point(6, 21);
			this.butExport.Name = "butExport";
			this.butExport.Size = new System.Drawing.Size(75, 24);
			this.butExport.TabIndex = 0;
			this.butExport.Text = "Export";
			this.butExport.Click += new System.EventHandler(this.butExport_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label5);
			this.groupBox3.Controls.Add(this.groupBox4);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.butIncrease);
			this.groupBox3.Controls.Add(this.textPercent);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox3.Location = new System.Drawing.Point(368, 32);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(248, 167);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Increase by %";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(87, 140);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(90, 18);
			this.label5.TabIndex = 11;
			this.label5.Text = "(or decrease)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.radioPenny);
			this.groupBox4.Controls.Add(this.radioDime);
			this.groupBox4.Controls.Add(this.radioDollar);
			this.groupBox4.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox4.Location = new System.Drawing.Point(6, 49);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(208, 75);
			this.groupBox4.TabIndex = 1;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Round to nearest";
			// 
			// radioPenny
			// 
			this.radioPenny.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioPenny.Location = new System.Drawing.Point(14, 52);
			this.radioPenny.Name = "radioPenny";
			this.radioPenny.Size = new System.Drawing.Size(104, 17);
			this.radioPenny.TabIndex = 2;
			this.radioPenny.Text = "$.01";
			// 
			// radioDime
			// 
			this.radioDime.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioDime.Location = new System.Drawing.Point(14, 35);
			this.radioDime.Name = "radioDime";
			this.radioDime.Size = new System.Drawing.Size(104, 17);
			this.radioDime.TabIndex = 1;
			this.radioDime.Text = "$.10";
			// 
			// radioDollar
			// 
			this.radioDollar.Checked = true;
			this.radioDollar.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioDollar.Location = new System.Drawing.Point(14, 18);
			this.radioDollar.Name = "radioDollar";
			this.radioDollar.Size = new System.Drawing.Size(104, 17);
			this.radioDollar.TabIndex = 0;
			this.radioDollar.TabStop = true;
			this.radioDollar.Text = "$1";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(92, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(109, 18);
			this.label3.TabIndex = 6;
			this.label3.Text = "for example: 5";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butIncrease
			// 
			this.butIncrease.Location = new System.Drawing.Point(6, 137);
			this.butIncrease.Name = "butIncrease";
			this.butIncrease.Size = new System.Drawing.Size(75, 24);
			this.butIncrease.TabIndex = 2;
			this.butIncrease.Text = "Increase";
			this.butIncrease.Click += new System.EventHandler(this.butIncrease_Click);
			// 
			// textPercent
			// 
			this.textPercent.Location = new System.Drawing.Point(42, 23);
			this.textPercent.Name = "textPercent";
			this.textPercent.Size = new System.Drawing.Size(46, 20);
			this.textPercent.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(38, 18);
			this.label2.TabIndex = 5;
			this.label2.Text = "%";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.butClear);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Location = new System.Drawing.Point(12, 393);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(214, 79);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Clear";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(200, 30);
			this.label11.TabIndex = 7;
			this.label11.Text = "Clears all values from selected fee sched for selected prov and clinic";
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(6, 49);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(75, 24);
			this.butClear.TabIndex = 0;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.butPickGroupTo);
			this.groupBox1.Controls.Add(this.labelGroupTo);
			this.groupBox1.Controls.Add(this.comboGroupTo);
			this.groupBox1.Controls.Add(this.comboClinicTo);
			this.groupBox1.Controls.Add(this.comboProviderTo);
			this.groupBox1.Controls.Add(this.comboFeeSchedTo);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.butPickProvTo);
			this.groupBox1.Controls.Add(this.butPickSchedTo);
			this.groupBox1.Controls.Add(this.butCopy);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(12, 149);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(328, 141);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Copy To";
			// 
			// butPickGroupTo
			// 
			this.butPickGroupTo.Location = new System.Drawing.Point(279, 45);
			this.butPickGroupTo.Name = "butPickGroupTo";
			this.butPickGroupTo.Size = new System.Drawing.Size(23, 21);
			this.butPickGroupTo.TabIndex = 43;
			this.butPickGroupTo.Text = "...";
			this.butPickGroupTo.Visible = false;
			this.butPickGroupTo.Click += new System.EventHandler(this.butPickGroupTo_Click);
			// 
			// labelGroupTo
			// 
			this.labelGroupTo.Location = new System.Drawing.Point(9, 47);
			this.labelGroupTo.Name = "labelGroupTo";
			this.labelGroupTo.Size = new System.Drawing.Size(90, 17);
			this.labelGroupTo.TabIndex = 42;
			this.labelGroupTo.Text = "Group";
			this.labelGroupTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelGroupTo.Visible = false;
			// 
			// comboGroupTo
			// 
			this.comboGroupTo.Location = new System.Drawing.Point(-160, 47);
			this.comboGroupTo.Name = "comboGroupTo";
			this.comboGroupTo.Size = new System.Drawing.Size(173, 21);
			this.comboGroupTo.TabIndex = 42;
			this.comboGroupTo.Visible = false;
			// 
			// comboProviderTo
			// 
			this.comboProviderTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboProviderTo.FormattingEnabled = true;
			this.comboProviderTo.Location = new System.Drawing.Point(100, 71);
			this.comboProviderTo.Name = "comboProviderTo";
			this.comboProviderTo.Size = new System.Drawing.Size(173, 21);
			this.comboProviderTo.TabIndex = 4;
			// 
			// comboFeeSchedTo
			// 
			this.comboFeeSchedTo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboFeeSchedTo.Location = new System.Drawing.Point(100, 19);
			this.comboFeeSchedTo.Name = "comboFeeSchedTo";
			this.comboFeeSchedTo.Size = new System.Drawing.Size(173, 21);
			this.comboFeeSchedTo.TabIndex = 0;
			this.comboFeeSchedTo.SelectionChangeCommitted += new System.EventHandler(this.comboFeeCombos_SelectionChangeCommitted);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(26, 74);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(75, 16);
			this.label7.TabIndex = 39;
			this.label7.Text = "Provider";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPickProvTo
			// 
			this.butPickProvTo.Location = new System.Drawing.Point(279, 71);
			this.butPickProvTo.Name = "butPickProvTo";
			this.butPickProvTo.Size = new System.Drawing.Size(23, 21);
			this.butPickProvTo.TabIndex = 5;
			this.butPickProvTo.Text = "...";
			this.butPickProvTo.Click += new System.EventHandler(this.butPickProvider_Click);
			// 
			// butPickSchedTo
			// 
			this.butPickSchedTo.Location = new System.Drawing.Point(279, 19);
			this.butPickSchedTo.Name = "butPickSchedTo";
			this.butPickSchedTo.Size = new System.Drawing.Size(23, 21);
			this.butPickSchedTo.TabIndex = 1;
			this.butPickSchedTo.Text = "...";
			this.butPickSchedTo.Click += new System.EventHandler(this.butPickFeeSched_Click);
			// 
			// butCopy
			// 
			this.butCopy.Location = new System.Drawing.Point(100, 106);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 6;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Fee Schedule";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(567, 455);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormFeeSchedTools
			// 
			this.ClientSize = new System.Drawing.Size(654, 491);
			this.Controls.Add(this.checkShowGroups);
			this.Controls.Add(this.groupBox7);
			this.Controls.Add(this.groupGlobalUpdateFees);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFeeSchedTools";
			this.ShowInTaskbar = false;
			this.Text = "Fee Tools";
			this.Load += new System.EventHandler(this.FormFeeSchedTools_Load);
			this.groupBox7.ResumeLayout(false);
			this.groupGlobalUpdateFees.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox4.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.Button butCopy;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ComboBox comboFeeSchedTo;
		private System.Windows.Forms.GroupBox groupBox2;
		private OpenDental.UI.Button butClear;
		private System.Windows.Forms.GroupBox groupBox3;
		private OpenDental.UI.Button butIncrease;
		private System.Windows.Forms.TextBox textPercent;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioDollar;
		private System.Windows.Forms.RadioButton radioDime;
		private System.Windows.Forms.RadioButton radioPenny;
		private GroupBox groupBox5;
		private OpenDental.UI.Button butExport;
		private OpenDental.UI.Button butImport;
		private GroupBox groupGlobalUpdateFees;
		private Label label4;
		private OpenDental.UI.Button butUpdate;
		private Label label5;
		private UI.Button butImportCanada;
		private GroupBox groupBox7;
		private ComboBox comboProvider;
		private OpenDental.UI.ComboBoxClinicPicker comboClinic;
		private ComboBox comboFeeSched;
		private Label label12;
		private Label labelGroup;
		private Label label8;
		private UI.Button butPickProv;
		private UI.Button butPickSched;
		private Label label7;
		private UI.Button butPickProvTo;
		private ComboBox comboProviderTo;
		private UI.ComboBoxClinicPicker comboClinicTo;
		private UI.Button butPickSchedTo;
		private Label label9;
		private UI.Button butUpdateWriteoffs;
		private UI.ComboBoxClinicPicker comboGlobalUpdateClinics;
		private Label label11;
		private CheckBox checkShowGroups;
		private OpenDental.UI.ComboBoxOD comboGroup;
		private OpenDental.UI.ComboBoxOD comboGroupTo;
		private Label labelGroupTo;
		private UI.Button butPickGroupTo;
		private UI.Button butPickGroup;
	}
}
