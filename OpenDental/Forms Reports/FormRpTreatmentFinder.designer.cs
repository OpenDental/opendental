using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormRpTreatmentFinder {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRpTreatmentFinder));
			this.label1 = new System.Windows.Forms.Label();
			this.checkIncludeNoIns = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.checkUseTreatingProvider = new System.Windows.Forms.CheckBox();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.codeRangeFilter = new OpenDental.UI.ODCodeRangeFilter();
			this.checkBenefitAssumeGeneral = new System.Windows.Forms.CheckBox();
			this.comboBoxMultiBilling = new OpenDental.UI.ComboBoxOD();
			this.comboBoxMultiProv = new OpenDental.UI.ComboBoxOD();
			this.textOverAmount = new OpenDental.ValidDouble();
			this.comboMonthStart = new System.Windows.Forms.ComboBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.checkIncludePatsWithApts = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelCodeRange = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.datePickerEnd = new OpenDental.UI.ODDatePicker();
			this.datePickerStart = new OpenDental.UI.ODDatePicker();
			this.contextRightClick = new System.Windows.Forms.ContextMenu();
			this.menuItemFamily = new System.Windows.Forms.MenuItem();
			this.menuItemAccount = new System.Windows.Forms.MenuItem();
			this.gridMain = new OpenDental.UI.GridOD();
			this.buttonExport = new OpenDental.UI.Button();
			this.butLettersPreview = new OpenDental.UI.Button();
			this.butLabelSingle = new OpenDental.UI.Button();
			this.butLabelPreview = new OpenDental.UI.Button();
			this.butGotoAccount = new OpenDental.UI.Button();
			this.butGotoFamily = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkIncludeHiddenBillingTypes = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(872, 29);
			this.label1.TabIndex = 29;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// checkIncludeNoIns
			// 
			this.checkIncludeNoIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeNoIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeNoIns.Location = new System.Drawing.Point(558, 47);
			this.checkIncludeNoIns.Name = "checkIncludeNoIns";
			this.checkIncludeNoIns.Size = new System.Drawing.Size(239, 16);
			this.checkIncludeNoIns.TabIndex = 8;
			this.checkIncludeNoIns.Text = "Include secondary insurance and no insurance";
			this.checkIncludeNoIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeNoIns.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.checkIncludeHiddenBillingTypes);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.checkUseTreatingProvider);
			this.groupBox1.Controls.Add(this.comboClinics);
			this.groupBox1.Controls.Add(this.codeRangeFilter);
			this.groupBox1.Controls.Add(this.checkBenefitAssumeGeneral);
			this.groupBox1.Controls.Add(this.comboBoxMultiBilling);
			this.groupBox1.Controls.Add(this.comboBoxMultiProv);
			this.groupBox1.Controls.Add(this.textOverAmount);
			this.groupBox1.Controls.Add(this.comboMonthStart);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.checkIncludeNoIns);
			this.groupBox1.Controls.Add(this.checkIncludePatsWithApts);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.labelCodeRange);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.butRefresh);
			this.groupBox1.Controls.Add(this.datePickerEnd);
			this.groupBox1.Controls.Add(this.datePickerStart);
			this.groupBox1.Location = new System.Drawing.Point(5, 41);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(1040, 109);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.Text = "View";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(10, 89);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(110, 16);
			this.label5.TabIndex = 48;
			this.label5.Text = "TP Date To";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkUseTreatingProvider
			// 
			this.checkUseTreatingProvider.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseTreatingProvider.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseTreatingProvider.Location = new System.Drawing.Point(558, 15);
			this.checkUseTreatingProvider.Name = "checkUseTreatingProvider";
			this.checkUseTreatingProvider.Size = new System.Drawing.Size(239, 16);
			this.checkUseTreatingProvider.TabIndex = 6;
			this.checkUseTreatingProvider.Text = "Use Treating Provider";
			this.checkUseTreatingProvider.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUseTreatingProvider.UseVisualStyleBackColor = true;
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeHiddenInAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(326, 8);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.Size = new System.Drawing.Size(197, 21);
			this.comboClinics.TabIndex = 5;
			// 
			// codeRangeFilter
			// 
			this.codeRangeFilter.Location = new System.Drawing.Point(885, 14);
			this.codeRangeFilter.Name = "codeRangeFilter";
			this.codeRangeFilter.Size = new System.Drawing.Size(147, 37);
			this.codeRangeFilter.TabIndex = 10;
			// 
			// checkBenefitAssumeGeneral
			// 
			this.checkBenefitAssumeGeneral.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBenefitAssumeGeneral.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBenefitAssumeGeneral.Location = new System.Drawing.Point(558, 31);
			this.checkBenefitAssumeGeneral.Name = "checkBenefitAssumeGeneral";
			this.checkBenefitAssumeGeneral.Size = new System.Drawing.Size(239, 16);
			this.checkBenefitAssumeGeneral.TabIndex = 7;
			this.checkBenefitAssumeGeneral.Text = "Assume procedures are General";
			this.checkBenefitAssumeGeneral.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBenefitAssumeGeneral.UseVisualStyleBackColor = true;
			// 
			// comboBoxMultiBilling
			// 
			this.comboBoxMultiBilling.BackColor = System.Drawing.SystemColors.Window;
			this.comboBoxMultiBilling.Location = new System.Drawing.Point(363, 58);
			this.comboBoxMultiBilling.Name = "comboBoxMultiBilling";
			this.comboBoxMultiBilling.SelectionModeMulti = true;
			this.comboBoxMultiBilling.Size = new System.Drawing.Size(160, 21);
			this.comboBoxMultiBilling.TabIndex = 4;
			this.comboBoxMultiBilling.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiBilling_SelectionChangeCommitted);
			// 
			// comboBoxMultiProv
			// 
			this.comboBoxMultiProv.BackColor = System.Drawing.SystemColors.Window;
			this.comboBoxMultiProv.Location = new System.Drawing.Point(363, 35);
			this.comboBoxMultiProv.Name = "comboBoxMultiProv";
			this.comboBoxMultiProv.SelectionModeMulti = true;
			this.comboBoxMultiProv.Size = new System.Drawing.Size(160, 21);
			this.comboBoxMultiProv.TabIndex = 3;
			this.comboBoxMultiProv.SelectionChangeCommitted += new System.EventHandler(this.comboBoxMultiProv_SelectionChangeCommitted);
			// 
			// textOverAmount
			// 
			this.textOverAmount.Location = new System.Drawing.Point(138, 35);
			this.textOverAmount.MaxVal = 100000000D;
			this.textOverAmount.MinVal = -100000000D;
			this.textOverAmount.Name = "textOverAmount";
			this.textOverAmount.Size = new System.Drawing.Size(98, 20);
			this.textOverAmount.TabIndex = 2;
			// 
			// comboMonthStart
			// 
			this.comboMonthStart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMonthStart.Items.AddRange(new object[] {
            "Calendar Year",
            "01 - January",
            "02 - February",
            "03 - March",
            "04 - April",
            "05 - May",
            "06 - June",
            "07 - July",
            "08 - August",
            "09 - September",
            "10 - October",
            "11 - November",
            "12 - December"});
			this.comboMonthStart.Location = new System.Drawing.Point(138, 8);
			this.comboMonthStart.MaxDropDownItems = 40;
			this.comboMonthStart.Name = "comboMonthStart";
			this.comboMonthStart.Size = new System.Drawing.Size(98, 21);
			this.comboMonthStart.TabIndex = 1;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 37);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(130, 16);
			this.label8.TabIndex = 46;
			this.label8.Text = "Amount remaining over";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(265, 61);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(97, 16);
			this.label7.TabIndex = 43;
			this.label7.Text = "Billing Type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIncludePatsWithApts
			// 
			this.checkIncludePatsWithApts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludePatsWithApts.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludePatsWithApts.Location = new System.Drawing.Point(558, 63);
			this.checkIncludePatsWithApts.Name = "checkIncludePatsWithApts";
			this.checkIncludePatsWithApts.Size = new System.Drawing.Size(239, 16);
			this.checkIncludePatsWithApts.TabIndex = 9;
			this.checkIncludePatsWithApts.Text = "Include patients with upcoming appointments";
			this.checkIncludePatsWithApts.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludePatsWithApts.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(130, 16);
			this.label3.TabIndex = 37;
			this.label3.Text = "Ins Month Start";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelCodeRange
			// 
			this.labelCodeRange.Location = new System.Drawing.Point(807, 14);
			this.labelCodeRange.Name = "labelCodeRange";
			this.labelCodeRange.Size = new System.Drawing.Size(77, 17);
			this.labelCodeRange.TabIndex = 40;
			this.labelCodeRange.Text = "Code Range";
			this.labelCodeRange.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(268, 38);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(94, 16);
			this.label4.TabIndex = 35;
			this.label4.Text = "Provider";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(122, 16);
			this.label2.TabIndex = 33;
			this.label2.Text = "TP Date From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(954, 78);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(80, 24);
			this.butRefresh.TabIndex = 11;
			this.butRefresh.Text = "&Refresh List";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// datePickerEnd
			// 
			this.datePickerEnd.BackColor = System.Drawing.Color.Transparent;
			this.datePickerEnd.Location = new System.Drawing.Point(71, 86);
			this.datePickerEnd.Name = "datePickerEnd";
			this.datePickerEnd.Size = new System.Drawing.Size(202, 23);
			this.datePickerEnd.TabIndex = 49;
			// 
			// datePickerStart
			// 
			this.datePickerStart.BackColor = System.Drawing.Color.Transparent;
			this.datePickerStart.Location = new System.Drawing.Point(71, 61);
			this.datePickerStart.Name = "datePickerStart";
			this.datePickerStart.Size = new System.Drawing.Size(188, 23);
			this.datePickerStart.TabIndex = 50;
			// 
			// contextRightClick
			// 
			this.contextRightClick.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFamily,
            this.menuItemAccount});
			// 
			// menuItemFamily
			// 
			this.menuItemFamily.Index = 0;
			this.menuItemFamily.Text = "See Family";
			this.menuItemFamily.Click += new System.EventHandler(this.menuItemFamily_Click);
			// 
			// menuItemAccount
			// 
			this.menuItemAccount.Index = 1;
			this.menuItemAccount.Text = "See Account";
			this.menuItemAccount.Click += new System.EventHandler(this.menuItemAccount_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(3, 156);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(1043, 467);
			this.gridMain.TabIndex = 1;
			this.gridMain.Title = "Treatment Finder";
			this.gridMain.TranslationName = "TableTreatmentFinder";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// buttonExport
			// 
			this.buttonExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.buttonExport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.buttonExport.Location = new System.Drawing.Point(7, 653);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(119, 24);
			this.buttonExport.TabIndex = 3;
			this.buttonExport.Text = "Export to File";
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// butLettersPreview
			// 
			this.butLettersPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLettersPreview.Image = global::OpenDental.Properties.Resources.butPreview;
			this.butLettersPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLettersPreview.Location = new System.Drawing.Point(7, 627);
			this.butLettersPreview.Name = "butLettersPreview";
			this.butLettersPreview.Size = new System.Drawing.Size(119, 24);
			this.butLettersPreview.TabIndex = 2;
			this.butLettersPreview.Text = "Letters Preview";
			this.butLettersPreview.Click += new System.EventHandler(this.butLettersPreview_Click);
			// 
			// butLabelSingle
			// 
			this.butLabelSingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabelSingle.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabelSingle.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabelSingle.Location = new System.Drawing.Point(132, 627);
			this.butLabelSingle.Name = "butLabelSingle";
			this.butLabelSingle.Size = new System.Drawing.Size(119, 24);
			this.butLabelSingle.TabIndex = 4;
			this.butLabelSingle.Text = "Single Labels";
			this.butLabelSingle.Click += new System.EventHandler(this.butLabelSingle_Click);
			// 
			// butLabelPreview
			// 
			this.butLabelPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butLabelPreview.Image = global::OpenDental.Properties.Resources.butLabel;
			this.butLabelPreview.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butLabelPreview.Location = new System.Drawing.Point(132, 653);
			this.butLabelPreview.Name = "butLabelPreview";
			this.butLabelPreview.Size = new System.Drawing.Size(119, 24);
			this.butLabelPreview.TabIndex = 5;
			this.butLabelPreview.Text = "Label Preview";
			this.butLabelPreview.Click += new System.EventHandler(this.butLabelPreview_Click);
			// 
			// butGotoAccount
			// 
			this.butGotoAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butGotoAccount.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoAccount.Location = new System.Drawing.Point(787, 653);
			this.butGotoAccount.Name = "butGotoAccount";
			this.butGotoAccount.Size = new System.Drawing.Size(96, 24);
			this.butGotoAccount.TabIndex = 8;
			this.butGotoAccount.Text = "Go to Account";
			this.butGotoAccount.Click += new System.EventHandler(this.butGotoAccount_Click);
			// 
			// butGotoFamily
			// 
			this.butGotoFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butGotoFamily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGotoFamily.Location = new System.Drawing.Point(787, 627);
			this.butGotoFamily.Name = "butGotoFamily";
			this.butGotoFamily.Size = new System.Drawing.Size(96, 24);
			this.butGotoFamily.TabIndex = 7;
			this.butGotoFamily.Text = "Go to Family";
			this.butGotoFamily.Click += new System.EventHandler(this.butGotoFamily_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrintSmall;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(544, 653);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(87, 24);
			this.butPrint.TabIndex = 6;
			this.butPrint.Text = "Print List";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(970, 653);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 9;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkIncludeHiddenBillingTypes
			// 
			this.checkIncludeHiddenBillingTypes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeHiddenBillingTypes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIncludeHiddenBillingTypes.Location = new System.Drawing.Point(284, 85);
			this.checkIncludeHiddenBillingTypes.Name = "checkIncludeHiddenBillingTypes";
			this.checkIncludeHiddenBillingTypes.Size = new System.Drawing.Size(239, 16);
			this.checkIncludeHiddenBillingTypes.TabIndex = 51;
			this.checkIncludeHiddenBillingTypes.Text = "Include Hidden Billing Types";
			this.checkIncludeHiddenBillingTypes.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeHiddenBillingTypes.UseVisualStyleBackColor = true;
			this.checkIncludeHiddenBillingTypes.CheckedChanged += new System.EventHandler(this.checkIncludeHiddenBillingTypes_CheckedChanged);
			// 
			// FormRpTreatmentFinder
			// 
			this.ClientSize = new System.Drawing.Size(1049, 681);
			this.Controls.Add(this.buttonExport);
			this.Controls.Add(this.butLettersPreview);
			this.Controls.Add(this.butLabelSingle);
			this.Controls.Add(this.butLabelPreview);
			this.Controls.Add(this.butGotoAccount);
			this.Controls.Add(this.butGotoFamily);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormRpTreatmentFinder";
			this.Text = "Treatment Finder";
			this.Load += new System.EventHandler(this.FormRpTreatmentFinder_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private Label label1;
		private CheckBox checkIncludeNoIns;
		private UI.GridOD gridMain;
		private GroupBox groupBox1;
		private UI.Button butPrint;
		private UI.Button butRefresh;
		private ContextMenu contextRightClick;
		private MenuItem menuItemFamily;
		private MenuItem menuItemAccount;
		private UI.Button butGotoFamily;
		private UI.Button butGotoAccount;
		private Label label2;
		private Label label4;
		private Label label3;
		private Label labelCodeRange;
		private UI.Button butLabelSingle;
		private UI.Button butLabelPreview;
		private Label label7;
		private UI.Button butLettersPreview;
		private UI.Button buttonExport;
		private Label label8;
		private ComboBox comboMonthStart;
		private ValidDouble textOverAmount;
		private UI.ComboBoxOD comboBoxMultiProv;
		private UI.ComboBoxOD comboBoxMultiBilling;
		private CheckBox checkIncludePatsWithApts;
		private CheckBox checkBenefitAssumeGeneral;
		private UI.ODCodeRangeFilter codeRangeFilter;
		private CheckBox checkUseTreatingProvider;
		private UI.ComboBoxClinicPicker comboClinics;
		private Label label5;
		private UI.ODDatePicker datePickerEnd;
		private UI.ODDatePicker datePickerStart;
		private CheckBox checkIncludeHiddenBillingTypes;
	}
}
