using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormFinanceCharges {
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

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFinanceCharges));
			this.textDate = new OpenDental.ValidDate();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radio30 = new System.Windows.Forms.RadioButton();
			this.radio90 = new System.Windows.Forms.RadioButton();
			this.radio60 = new System.Windows.Forms.RadioButton();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textAPR = new OpenDental.ValidNum();
			this.textDateLastRun = new OpenDental.ValidDate();
			this.label5 = new System.Windows.Forms.Label();
			this.butUndo = new OpenDental.UI.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textDateUndo = new OpenDental.ValidDate();
			this.label6 = new System.Windows.Forms.Label();
			this.listBillType = new OpenDental.UI.ListBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.labelCompound = new System.Windows.Forms.Label();
			this.checkCompound = new System.Windows.Forms.CheckBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textOver = new OpenDental.ValidDouble();
			this.textAtLeast = new OpenDental.ValidDouble();
			this.labelOver = new System.Windows.Forms.Label();
			this.labelAtLeast = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.radioFinanceCharge = new System.Windows.Forms.RadioButton();
			this.textBillingCharge = new OpenDental.ValidDouble();
			this.radioBillingCharge = new System.Windows.Forms.RadioButton();
			this.checkBadAddress = new System.Windows.Forms.CheckBox();
			this.groupBoxFilters = new System.Windows.Forms.GroupBox();
			this.textExcludeLessThan = new OpenDental.ValidDouble();
			this.textExcludeNotBilledSince = new OpenDental.ValidDate();
			this.labelExcludeNotBilledSince = new System.Windows.Forms.Label();
			this.labelExcludeBalanceLessThan = new System.Windows.Forms.Label();
			this.checkExcludeAccountNoTil = new System.Windows.Forms.CheckBox();
			this.checkIgnoreInPerson = new System.Windows.Forms.CheckBox();
			this.checkExcludeInsPending = new System.Windows.Forms.CheckBox();
			this.checkExcludeInactive = new System.Windows.Forms.CheckBox();
			this.groupBoxAssignCharge = new System.Windows.Forms.GroupBox();
			this.comboSpecificProv = new OpenDental.UI.ComboBoxOD();
			this.radioSpecificProv = new System.Windows.Forms.RadioButton();
			this.radioPatPriProv = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.panel1.SuspendLayout();
			this.groupBoxFilters.SuspendLayout();
			this.groupBoxAssignCharge.SuspendLayout();
			this.SuspendLayout();
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(133, 42);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(78, 20);
			this.textDate.TabIndex = 15;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(11, 46);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 14);
			this.label1.TabIndex = 20;
			this.label1.Text = "Date of new charges";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radio30);
			this.groupBox1.Controls.Add(this.radio90);
			this.groupBox1.Controls.Add(this.radio60);
			this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox1.Location = new System.Drawing.Point(20, 226);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(161, 82);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Calculate on balances aged";
			// 
			// radio30
			// 
			this.radio30.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio30.Location = new System.Drawing.Point(13, 17);
			this.radio30.Name = "radio30";
			this.radio30.Size = new System.Drawing.Size(104, 17);
			this.radio30.TabIndex = 1;
			this.radio30.Text = "Over 30 Days";
			// 
			// radio90
			// 
			this.radio90.Checked = true;
			this.radio90.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio90.Location = new System.Drawing.Point(13, 56);
			this.radio90.Name = "radio90";
			this.radio90.Size = new System.Drawing.Size(104, 17);
			this.radio90.TabIndex = 3;
			this.radio90.TabStop = true;
			this.radio90.Text = "Over 90 Days";
			// 
			// radio60
			// 
			this.radio60.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radio60.Location = new System.Drawing.Point(13, 36);
			this.radio60.Name = "radio60";
			this.radio60.Size = new System.Drawing.Size(104, 17);
			this.radio60.TabIndex = 2;
			this.radio60.Text = "Over 60 Days";
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(486, 486);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 19;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(405, 486);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 18;
			this.butOK.Text = "Run";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(67, 46);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 14);
			this.label2.TabIndex = 22;
			this.label2.Text = "APR";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(194, 46);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(12, 14);
			this.label3.TabIndex = 23;
			this.label3.Text = "%";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(212, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(102, 14);
			this.label4.TabIndex = 24;
			this.label4.Text = "(For Example: 18)";
			// 
			// textAPR
			// 
			this.textAPR.Location = new System.Drawing.Point(147, 43);
			this.textAPR.Name = "textAPR";
			this.textAPR.Size = new System.Drawing.Size(42, 20);
			this.textAPR.TabIndex = 26;
			// 
			// textDateLastRun
			// 
			this.textDateLastRun.Location = new System.Drawing.Point(133, 16);
			this.textDateLastRun.Name = "textDateLastRun";
			this.textDateLastRun.ReadOnly = true;
			this.textDateLastRun.Size = new System.Drawing.Size(78, 20);
			this.textDateLastRun.TabIndex = 27;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(8, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(123, 14);
			this.label5.TabIndex = 28;
			this.label5.Text = "Date last run";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butUndo
			// 
			this.butUndo.Location = new System.Drawing.Point(75, 45);
			this.butUndo.Name = "butUndo";
			this.butUndo.Size = new System.Drawing.Size(78, 25);
			this.butUndo.TabIndex = 30;
			this.butUndo.Text = "Undo";
			this.butUndo.Click += new System.EventHandler(this.butUndo_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textDateUndo);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.butUndo);
			this.groupBox2.Location = new System.Drawing.Point(20, 314);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(161, 79);
			this.groupBox2.TabIndex = 31;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Undo billing/finance charges";
			// 
			// textDateUndo
			// 
			this.textDateUndo.Location = new System.Drawing.Point(75, 19);
			this.textDateUndo.Name = "textDateUndo";
			this.textDateUndo.ReadOnly = true;
			this.textDateUndo.Size = new System.Drawing.Size(78, 20);
			this.textDateUndo.TabIndex = 31;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(4, 22);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(69, 14);
			this.label6.TabIndex = 32;
			this.label6.Text = "Date to undo";
			this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// listBillType
			// 
			this.listBillType.Location = new System.Drawing.Point(397, 34);
			this.listBillType.Name = "listBillType";
			this.listBillType.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listBillType.Size = new System.Drawing.Size(158, 186);
			this.listBillType.TabIndex = 32;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(397, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(164, 16);
			this.label7.TabIndex = 33;
			this.label7.Text = "Only apply to these Billing Types";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.labelCompound);
			this.panel1.Controls.Add(this.checkCompound);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label11);
			this.panel1.Controls.Add(this.textOver);
			this.panel1.Controls.Add(this.textAtLeast);
			this.panel1.Controls.Add(this.labelOver);
			this.panel1.Controls.Add(this.labelAtLeast);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.radioFinanceCharge);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.textBillingCharge);
			this.panel1.Controls.Add(this.textAPR);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.radioBillingCharge);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Location = new System.Drawing.Point(20, 68);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(319, 152);
			this.panel1.TabIndex = 34;
			// 
			// labelCompound
			// 
			this.labelCompound.Location = new System.Drawing.Point(28, 124);
			this.labelCompound.Name = "labelCompound";
			this.labelCompound.Size = new System.Drawing.Size(105, 14);
			this.labelCompound.TabIndex = 39;
			this.labelCompound.Text = "Compound interest";
			this.labelCompound.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkCompound
			// 
			this.checkCompound.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCompound.Checked = true;
			this.checkCompound.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkCompound.Location = new System.Drawing.Point(145, 124);
			this.checkCompound.Margin = new System.Windows.Forms.Padding(0);
			this.checkCompound.Name = "checkCompound";
			this.checkCompound.Size = new System.Drawing.Size(16, 14);
			this.checkCompound.TabIndex = 35;
			this.checkCompound.UseVisualStyleBackColor = true;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(135, 73);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(12, 14);
			this.label12.TabIndex = 38;
			this.label12.Text = "$";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(135, 100);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(12, 14);
			this.label11.TabIndex = 37;
			this.label11.Text = "$";
			this.label11.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textOver
			// 
			this.textOver.BackColor = System.Drawing.SystemColors.Window;
			this.textOver.Location = new System.Drawing.Point(147, 97);
			this.textOver.MaxVal = 100000000D;
			this.textOver.MinVal = -100000000D;
			this.textOver.Name = "textOver";
			this.textOver.Size = new System.Drawing.Size(42, 20);
			this.textOver.TabIndex = 36;
			// 
			// textAtLeast
			// 
			this.textAtLeast.BackColor = System.Drawing.SystemColors.Window;
			this.textAtLeast.Location = new System.Drawing.Point(147, 70);
			this.textAtLeast.MaxVal = 100000000D;
			this.textAtLeast.MinVal = -100000000D;
			this.textAtLeast.Name = "textAtLeast";
			this.textAtLeast.Size = new System.Drawing.Size(42, 20);
			this.textAtLeast.TabIndex = 35;
			// 
			// labelOver
			// 
			this.labelOver.Location = new System.Drawing.Point(28, 99);
			this.labelOver.Name = "labelOver";
			this.labelOver.Size = new System.Drawing.Size(95, 14);
			this.labelOver.TabIndex = 33;
			this.labelOver.Text = "Only if over";
			this.labelOver.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAtLeast
			// 
			this.labelAtLeast.Location = new System.Drawing.Point(28, 73);
			this.labelAtLeast.Name = "labelAtLeast";
			this.labelAtLeast.Size = new System.Drawing.Size(95, 14);
			this.labelAtLeast.TabIndex = 34;
			this.labelAtLeast.Text = "Charge at least";
			this.labelAtLeast.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(135, 14);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(12, 14);
			this.label8.TabIndex = 28;
			this.label8.Text = "$";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// radioFinanceCharge
			// 
			this.radioFinanceCharge.AutoSize = true;
			this.radioFinanceCharge.Checked = true;
			this.radioFinanceCharge.Location = new System.Drawing.Point(11, 44);
			this.radioFinanceCharge.Name = "radioFinanceCharge";
			this.radioFinanceCharge.Size = new System.Drawing.Size(100, 17);
			this.radioFinanceCharge.TabIndex = 0;
			this.radioFinanceCharge.TabStop = true;
			this.radioFinanceCharge.Text = "Finance Charge";
			this.radioFinanceCharge.UseVisualStyleBackColor = true;
			this.radioFinanceCharge.CheckedChanged += new System.EventHandler(this.radioFinanceCharge_CheckedChanged);
			// 
			// textBillingCharge
			// 
			this.textBillingCharge.BackColor = System.Drawing.SystemColors.Window;
			this.textBillingCharge.Location = new System.Drawing.Point(147, 12);
			this.textBillingCharge.MaxVal = 100000000D;
			this.textBillingCharge.MinVal = -100000000D;
			this.textBillingCharge.Name = "textBillingCharge";
			this.textBillingCharge.ReadOnly = true;
			this.textBillingCharge.Size = new System.Drawing.Size(42, 20);
			this.textBillingCharge.TabIndex = 27;
			// 
			// radioBillingCharge
			// 
			this.radioBillingCharge.Location = new System.Drawing.Point(11, 12);
			this.radioBillingCharge.Name = "radioBillingCharge";
			this.radioBillingCharge.Size = new System.Drawing.Size(95, 17);
			this.radioBillingCharge.TabIndex = 1;
			this.radioBillingCharge.TabStop = true;
			this.radioBillingCharge.Text = "Billing Charge";
			this.radioBillingCharge.UseVisualStyleBackColor = true;
			this.radioBillingCharge.CheckedChanged += new System.EventHandler(this.radioBillingCharge_CheckedChanged);
			// 
			// checkBadAddress
			// 
			this.checkBadAddress.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBadAddress.Location = new System.Drawing.Point(6, 17);
			this.checkBadAddress.Name = "checkBadAddress";
			this.checkBadAddress.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkBadAddress.Size = new System.Drawing.Size(295, 17);
			this.checkBadAddress.TabIndex = 39;
			this.checkBadAddress.Text = "Exclude bad addresses (no zip code)";
			this.checkBadAddress.UseVisualStyleBackColor = true;
			// 
			// groupBoxFilters
			// 
			this.groupBoxFilters.Controls.Add(this.textExcludeLessThan);
			this.groupBoxFilters.Controls.Add(this.textExcludeNotBilledSince);
			this.groupBoxFilters.Controls.Add(this.labelExcludeNotBilledSince);
			this.groupBoxFilters.Controls.Add(this.labelExcludeBalanceLessThan);
			this.groupBoxFilters.Controls.Add(this.checkExcludeAccountNoTil);
			this.groupBoxFilters.Controls.Add(this.checkIgnoreInPerson);
			this.groupBoxFilters.Controls.Add(this.checkExcludeInsPending);
			this.groupBoxFilters.Controls.Add(this.checkExcludeInactive);
			this.groupBoxFilters.Controls.Add(this.checkBadAddress);
			this.groupBoxFilters.Location = new System.Drawing.Point(187, 226);
			this.groupBoxFilters.Name = "groupBoxFilters";
			this.groupBoxFilters.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.groupBoxFilters.Size = new System.Drawing.Size(375, 167);
			this.groupBoxFilters.TabIndex = 40;
			this.groupBoxFilters.TabStop = false;
			this.groupBoxFilters.Text = "Billing/Finance Filters";
			// 
			// textExcludeLessThan
			// 
			this.textExcludeLessThan.BackColor = System.Drawing.SystemColors.Window;
			this.textExcludeLessThan.Location = new System.Drawing.Point(288, 116);
			this.textExcludeLessThan.MaxVal = 100000000D;
			this.textExcludeLessThan.MinVal = -100000000D;
			this.textExcludeLessThan.Name = "textExcludeLessThan";
			this.textExcludeLessThan.Size = new System.Drawing.Size(42, 20);
			this.textExcludeLessThan.TabIndex = 54;
			// 
			// textExcludeNotBilledSince
			// 
			this.textExcludeNotBilledSince.Location = new System.Drawing.Point(288, 140);
			this.textExcludeNotBilledSince.Name = "textExcludeNotBilledSince";
			this.textExcludeNotBilledSince.Size = new System.Drawing.Size(70, 20);
			this.textExcludeNotBilledSince.TabIndex = 47;
			// 
			// labelExcludeNotBilledSince
			// 
			this.labelExcludeNotBilledSince.Location = new System.Drawing.Point(6, 142);
			this.labelExcludeNotBilledSince.Name = "labelExcludeNotBilledSince";
			this.labelExcludeNotBilledSince.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelExcludeNotBilledSince.Size = new System.Drawing.Size(281, 16);
			this.labelExcludeNotBilledSince.TabIndex = 48;
			this.labelExcludeNotBilledSince.Text = "Exclude accounts not billed since";
			this.labelExcludeNotBilledSince.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelExcludeBalanceLessThan
			// 
			this.labelExcludeBalanceLessThan.Location = new System.Drawing.Point(6, 118);
			this.labelExcludeBalanceLessThan.Name = "labelExcludeBalanceLessThan";
			this.labelExcludeBalanceLessThan.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.labelExcludeBalanceLessThan.Size = new System.Drawing.Size(281, 16);
			this.labelExcludeBalanceLessThan.TabIndex = 53;
			this.labelExcludeBalanceLessThan.Text = "Exclude if balance is less than";
			this.labelExcludeBalanceLessThan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkExcludeAccountNoTil
			// 
			this.checkExcludeAccountNoTil.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeAccountNoTil.Location = new System.Drawing.Point(6, 97);
			this.checkExcludeAccountNoTil.Name = "checkExcludeAccountNoTil";
			this.checkExcludeAccountNoTil.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkExcludeAccountNoTil.Size = new System.Drawing.Size(295, 17);
			this.checkExcludeAccountNoTil.TabIndex = 44;
			this.checkExcludeAccountNoTil.Text = "Exclude accounts (guarantor) without Truth in Lending";
			this.checkExcludeAccountNoTil.UseVisualStyleBackColor = true;
			// 
			// checkIgnoreInPerson
			// 
			this.checkIgnoreInPerson.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIgnoreInPerson.Location = new System.Drawing.Point(6, 77);
			this.checkIgnoreInPerson.Name = "checkIgnoreInPerson";
			this.checkIgnoreInPerson.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIgnoreInPerson.Size = new System.Drawing.Size(295, 17);
			this.checkIgnoreInPerson.TabIndex = 43;
			this.checkIgnoreInPerson.Text = "Ignore walkout (In person) Statements";
			this.checkIgnoreInPerson.UseVisualStyleBackColor = true;
			// 
			// checkExcludeInsPending
			// 
			this.checkExcludeInsPending.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeInsPending.Location = new System.Drawing.Point(6, 57);
			this.checkExcludeInsPending.Name = "checkExcludeInsPending";
			this.checkExcludeInsPending.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkExcludeInsPending.Size = new System.Drawing.Size(295, 17);
			this.checkExcludeInsPending.TabIndex = 41;
			this.checkExcludeInsPending.Text = "Exclude if insurance pending";
			this.checkExcludeInsPending.UseVisualStyleBackColor = true;
			// 
			// checkExcludeInactive
			// 
			this.checkExcludeInactive.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExcludeInactive.Location = new System.Drawing.Point(6, 37);
			this.checkExcludeInactive.Name = "checkExcludeInactive";
			this.checkExcludeInactive.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkExcludeInactive.Size = new System.Drawing.Size(295, 17);
			this.checkExcludeInactive.TabIndex = 40;
			this.checkExcludeInactive.Text = "Exclude inactive families";
			this.checkExcludeInactive.UseVisualStyleBackColor = true;
			// 
			// groupBoxAssignCharge
			// 
			this.groupBoxAssignCharge.Controls.Add(this.comboSpecificProv);
			this.groupBoxAssignCharge.Controls.Add(this.radioSpecificProv);
			this.groupBoxAssignCharge.Controls.Add(this.radioPatPriProv);
			this.groupBoxAssignCharge.Location = new System.Drawing.Point(20, 399);
			this.groupBoxAssignCharge.Name = "groupBoxAssignCharge";
			this.groupBoxAssignCharge.Size = new System.Drawing.Size(301, 76);
			this.groupBoxAssignCharge.TabIndex = 49;
			this.groupBoxAssignCharge.TabStop = false;
			this.groupBoxAssignCharge.Text = "Assign charges to:";
			// 
			// comboSpecificProv
			// 
			this.comboSpecificProv.Location = new System.Drawing.Point(121, 45);
			this.comboSpecificProv.Name = "comboSpecificProv";
			this.comboSpecificProv.Size = new System.Drawing.Size(174, 21);
			this.comboSpecificProv.TabIndex = 4;
			// 
			// radioSpecificProv
			// 
			this.radioSpecificProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioSpecificProv.Location = new System.Drawing.Point(13, 47);
			this.radioSpecificProv.Name = "radioSpecificProv";
			this.radioSpecificProv.Size = new System.Drawing.Size(105, 17);
			this.radioSpecificProv.TabIndex = 1;
			this.radioSpecificProv.Text = "Specific Provider";
			this.radioSpecificProv.UseVisualStyleBackColor = true;
			// 
			// radioPatPriProv
			// 
			this.radioPatPriProv.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioPatPriProv.Location = new System.Drawing.Point(13, 24);
			this.radioPatPriProv.Name = "radioPatPriProv";
			this.radioPatPriProv.Size = new System.Drawing.Size(144, 17);
			this.radioPatPriProv.TabIndex = 0;
			this.radioPatPriProv.Text = "Patient\'s Primary Provider";
			this.radioPatPriProv.UseVisualStyleBackColor = true;
			this.radioPatPriProv.CheckedChanged += new System.EventHandler(this.RadioPatPriProv_CheckedChanged);
			// 
			// FormFinanceCharges
			// 
			this.AcceptButton = this.butOK;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(573, 523);
			this.Controls.Add(this.groupBoxAssignCharge);
			this.Controls.Add(this.groupBoxFilters);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.listBillType);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.textDateLastRun);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormFinanceCharges";
			this.ShowInTaskbar = false;
			this.Text = "Billing/Finance Charges";
			this.Load += new System.EventHandler(this.FormFinanceCharges_Load);
			this.Shown += new System.EventHandler(this.FormFinanceCharges_Shown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupBoxFilters.ResumeLayout(false);
			this.groupBoxFilters.PerformLayout();
			this.groupBoxAssignCharge.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.ValidDate textDate;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radio30;
		private System.Windows.Forms.RadioButton radio90;
		private System.Windows.Forms.RadioButton radio60;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private OpenDental.ValidNum textAPR;
		private ValidDate textDateLastRun;
		private Label label5;
		private OpenDental.UI.Button butUndo;
		private GroupBox groupBox2;
		private ValidDate textDateUndo;
		private Label label6;
		private OpenDental.UI.ListBoxOD listBillType;
		private Panel panel1;
		private Label label8;
		private ValidDouble textBillingCharge;
		private RadioButton radioBillingCharge;
		private RadioButton radioFinanceCharge;
		private Label label12;
		private Label label11;
		private ValidDouble textOver;
		private ValidDouble textAtLeast;
		private Label labelOver;
		private Label labelAtLeast;
		private CheckBox checkCompound;
		private Label labelCompound;
		private Label label7;
		private CheckBox checkBadAddress;
		private GroupBox groupBoxFilters;
		private ValidDouble textExcludeLessThan;
		private Label labelExcludeBalanceLessThan;
		private CheckBox checkExcludeAccountNoTil;
		private CheckBox checkIgnoreInPerson;
		private CheckBox checkExcludeInsPending;
		private CheckBox checkExcludeInactive;
		private Label labelExcludeNotBilledSince;
		private ValidDate textExcludeNotBilledSince;
		private GroupBox groupBoxAssignCharge;
		private RadioButton radioSpecificProv;
		private RadioButton radioPatPriProv;
		private UI.ComboBoxOD comboSpecificProv;
	}
}
