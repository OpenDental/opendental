using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProcCodeEdit {
		private System.ComponentModel.IContainer components = null;// Required designer variable.

		///<summary></summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing ){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		private void InitializeComponent(){
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProcCodeEdit));
			this.label1 = new System.Windows.Forms.Label();
			this.labelTreatArea = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textProcCode = new System.Windows.Forms.TextBox();
			this.textAbbrev = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.listTreatArea = new OpenDental.UI.ListBoxOD();
			this.checkNoBillIns = new System.Windows.Forms.CheckBox();
			this.listCategory = new OpenDental.UI.ListBoxOD();
			this.label9 = new System.Windows.Forms.Label();
			this.butSlider = new System.Windows.Forms.Button();
			this.textTime2 = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.checkIsHygiene = new System.Windows.Forms.CheckBox();
			this.textAlternateCode1 = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.checkIsProsth = new System.Windows.Forms.CheckBox();
			this.textMedicalCode = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.listPaintType = new OpenDental.UI.ListBoxOD();
			this.labelColor = new System.Windows.Forms.Label();
			this.butColor = new System.Windows.Forms.Button();
			this.textLaymanTerm = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkIsCanadianLab = new System.Windows.Forms.CheckBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textBaseUnits = new OpenDental.ValidNum();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.textSubstitutionCode = new System.Windows.Forms.TextBox();
			this.label19 = new System.Windows.Forms.Label();
			this.comboSubstOnlyIf = new System.Windows.Forms.ComboBox();
			this.checkMultiVisit = new System.Windows.Forms.CheckBox();
			this.labelDrugNDC = new System.Windows.Forms.Label();
			this.labelRevenueCode = new System.Windows.Forms.Label();
			this.textDrugNDC = new System.Windows.Forms.TextBox();
			this.textRevenueCode = new System.Windows.Forms.TextBox();
			this.label20 = new System.Windows.Forms.Label();
			this.label21 = new System.Windows.Forms.Label();
			this.comboProvNumDefault = new OpenDental.UI.ComboBoxOD();
			this.label22 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.labelTimeUnits = new System.Windows.Forms.Label();
			this.textTimeUnits = new OpenDental.ValidDouble();
			this.checkIsRadiology = new System.Windows.Forms.CheckBox();
			this.label24 = new System.Windows.Forms.Label();
			this.butMore = new OpenDental.UI.Button();
			this.butAddNote = new OpenDental.UI.Button();
			this.tbTime = new OpenDental.TableTimeBar();
			this.butColorClear = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAuditTrail = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butAutoNote = new OpenDental.UI.Button();
			this.DefaultTP = new System.Windows.Forms.Label();
			this.butAutoTpNote = new OpenDental.UI.Button();
			this.butAddTpNote = new OpenDental.UI.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.textNote = new OpenDental.ODtextBox();
			this.gridNotes = new OpenDental.UI.GridOD();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.gridTpNotes = new OpenDental.UI.GridOD();
			this.textTpNote = new OpenDental.ODtextBox();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.labelDefaultPreauthNote = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.textDefaultClaimNote = new OpenDental.ODtextBox();
			this.checkBypassLockDate = new System.Windows.Forms.CheckBox();
			this.gridFees = new OpenDental.UI.GridOD();
			this.textPaintText = new System.Windows.Forms.TextBox();
			this.labelPaintText = new System.Windows.Forms.Label();
			this.checkIsAutoTaxed = new System.Windows.Forms.CheckBox();
			this.checkToothRange = new System.Windows.Forms.CheckBox();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(123, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(82, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Proc Code";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelTreatArea
			// 
			this.labelTreatArea.Location = new System.Drawing.Point(614, 2);
			this.labelTreatArea.Name = "labelTreatArea";
			this.labelTreatArea.Size = new System.Drawing.Size(100, 14);
			this.labelTreatArea.TabIndex = 0;
			this.labelTreatArea.Text = "Treatment Area";
			this.labelTreatArea.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(740, 2);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 14);
			this.label5.TabIndex = 0;
			this.label5.Text = "Category";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, -6);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(53, 39);
			this.label6.TabIndex = 0;
			this.label6.Text = "Time Pattern";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(109, 102);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(94, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "Abbreviation";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(109, 83);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(94, 14);
			this.label8.TabIndex = 0;
			this.label8.Text = "Description";
			this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label10.Location = new System.Drawing.Point(6, 3);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(162, 14);
			this.label10.TabIndex = 0;
			this.label10.Text = "Default Note";
			this.label10.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textProcCode
			// 
			this.textProcCode.Location = new System.Drawing.Point(205, 1);
			this.textProcCode.Name = "textProcCode";
			this.textProcCode.ReadOnly = true;
			this.textProcCode.Size = new System.Drawing.Size(100, 20);
			this.textProcCode.TabIndex = 0;
			this.textProcCode.TabStop = false;
			// 
			// textAbbrev
			// 
			this.textAbbrev.Location = new System.Drawing.Point(205, 101);
			this.textAbbrev.MaxLength = 20;
			this.textAbbrev.Name = "textAbbrev";
			this.textAbbrev.Size = new System.Drawing.Size(100, 20);
			this.textAbbrev.TabIndex = 6;
			// 
			// textDescription
			// 
			this.textDescription.Location = new System.Drawing.Point(205, 81);
			this.textDescription.MaxLength = 255;
			this.textDescription.Name = "textDescription";
			this.textDescription.Size = new System.Drawing.Size(287, 20);
			this.textDescription.TabIndex = 5;
			// 
			// listTreatArea
			// 
			this.listTreatArea.ItemStrings = new string[] {
        "None",
        "Surface",
        "Tooth",
        "Mouth",
        "Quadrant",
        "Sextant",
        "Arch",
        "Tooth Range"};
			this.listTreatArea.Location = new System.Drawing.Point(616, 17);
			this.listTreatArea.Name = "listTreatArea";
			this.listTreatArea.Size = new System.Drawing.Size(118, 108);
			this.listTreatArea.TabIndex = 24;
			this.listTreatArea.SelectedIndexChanged += new System.EventHandler(this.listTreatArea_SelectedIndexChanged);
			// 
			// checkNoBillIns
			// 
			this.checkNoBillIns.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNoBillIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoBillIns.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkNoBillIns.Location = new System.Drawing.Point(18, 240);
			this.checkNoBillIns.Name = "checkNoBillIns";
			this.checkNoBillIns.Size = new System.Drawing.Size(200, 18);
			this.checkNoBillIns.TabIndex = 13;
			this.checkNoBillIns.Text = "Do not usually bill to Ins";
			this.checkNoBillIns.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listCategory
			// 
			this.listCategory.Location = new System.Drawing.Point(737, 17);
			this.listCategory.Name = "listCategory";
			this.listCategory.Size = new System.Drawing.Size(120, 355);
			this.listCategory.TabIndex = 23;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label9.Location = new System.Drawing.Point(850, 662);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(135, 28);
			this.label9.TabIndex = 0;
			this.label9.Text = "Clicking cancel will not undo changes to fees.";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butSlider
			// 
			this.butSlider.BackColor = System.Drawing.SystemColors.ControlDark;
			this.butSlider.Location = new System.Drawing.Point(10, 42);
			this.butSlider.Name = "butSlider";
			this.butSlider.Size = new System.Drawing.Size(12, 15);
			this.butSlider.TabIndex = 31;
			this.butSlider.UseVisualStyleBackColor = false;
			this.butSlider.MouseDown += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseDown);
			this.butSlider.MouseMove += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseMove);
			this.butSlider.MouseUp += new System.Windows.Forms.MouseEventHandler(this.butSlider_MouseUp);
			// 
			// textTime2
			// 
			this.textTime2.Enabled = false;
			this.textTime2.Location = new System.Drawing.Point(8, 609);
			this.textTime2.Name = "textTime2";
			this.textTime2.Size = new System.Drawing.Size(60, 20);
			this.textTime2.TabIndex = 19;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(5, 632);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(63, 16);
			this.label11.TabIndex = 0;
			this.label11.Text = "Minutes";
			// 
			// checkIsHygiene
			// 
			this.checkIsHygiene.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHygiene.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHygiene.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkIsHygiene.Location = new System.Drawing.Point(44, 258);
			this.checkIsHygiene.Name = "checkIsHygiene";
			this.checkIsHygiene.Size = new System.Drawing.Size(174, 18);
			this.checkIsHygiene.TabIndex = 14;
			this.checkIsHygiene.Text = "Is Hygiene procedure";
			this.checkIsHygiene.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAlternateCode1
			// 
			this.textAlternateCode1.Location = new System.Drawing.Point(205, 21);
			this.textAlternateCode1.MaxLength = 15;
			this.textAlternateCode1.Name = "textAlternateCode1";
			this.textAlternateCode1.Size = new System.Drawing.Size(100, 20);
			this.textAlternateCode1.TabIndex = 1;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(126, 23);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(79, 14);
			this.label12.TabIndex = 0;
			this.label12.Text = "Alt Code";
			this.label12.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(311, 23);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(161, 19);
			this.label13.TabIndex = 0;
			this.label13.Text = "(For some Medicaid)";
			// 
			// checkIsProsth
			// 
			this.checkIsProsth.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsProsth.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsProsth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkIsProsth.Location = new System.Drawing.Point(44, 276);
			this.checkIsProsth.Name = "checkIsProsth";
			this.checkIsProsth.Size = new System.Drawing.Size(174, 18);
			this.checkIsProsth.TabIndex = 15;
			this.checkIsProsth.Text = "Is Prosthesis";
			this.checkIsProsth.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMedicalCode
			// 
			this.textMedicalCode.Location = new System.Drawing.Point(205, 41);
			this.textMedicalCode.MaxLength = 15;
			this.textMedicalCode.Name = "textMedicalCode";
			this.textMedicalCode.Size = new System.Drawing.Size(100, 20);
			this.textMedicalCode.TabIndex = 2;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(126, 43);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(79, 14);
			this.label14.TabIndex = 0;
			this.label14.Text = "Medical Code";
			this.label14.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(493, 2);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(100, 14);
			this.label15.TabIndex = 0;
			this.label15.Text = "Paint Type";
			this.label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listPaintType
			// 
			this.listPaintType.Location = new System.Drawing.Point(495, 17);
			this.listPaintType.Name = "listPaintType";
			this.listPaintType.Size = new System.Drawing.Size(118, 238);
			this.listPaintType.TabIndex = 22;
			this.listPaintType.SelectedIndexChanged += new System.EventHandler(this.listPaintType_SelectedIndexChanged);
			// 
			// labelColor
			// 
			this.labelColor.Location = new System.Drawing.Point(87, 203);
			this.labelColor.Name = "labelColor";
			this.labelColor.Size = new System.Drawing.Size(116, 16);
			this.labelColor.TabIndex = 0;
			this.labelColor.Text = "Color Override";
			this.labelColor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(205, 202);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(21, 19);
			this.butColor.TabIndex = 0;
			this.butColor.TabStop = false;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// textLaymanTerm
			// 
			this.textLaymanTerm.Location = new System.Drawing.Point(205, 121);
			this.textLaymanTerm.MaxLength = 255;
			this.textLaymanTerm.Name = "textLaymanTerm";
			this.textLaymanTerm.Size = new System.Drawing.Size(178, 20);
			this.textLaymanTerm.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(79, 122);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(124, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Layman\'s Term";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsCanadianLab
			// 
			this.checkIsCanadianLab.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsCanadianLab.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsCanadianLab.Location = new System.Drawing.Point(44, 331);
			this.checkIsCanadianLab.Name = "checkIsCanadianLab";
			this.checkIsCanadianLab.Size = new System.Drawing.Size(174, 18);
			this.checkIsCanadianLab.TabIndex = 16;
			this.checkIsCanadianLab.Text = "Is Lab Fee";
			this.checkIsCanadianLab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(100, 144);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(103, 13);
			this.label16.TabIndex = 0;
			this.label16.Text = "Base Units";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBaseUnits
			// 
			this.textBaseUnits.Location = new System.Drawing.Point(205, 141);
			this.textBaseUnits.Name = "textBaseUnits";
			this.textBaseUnits.Size = new System.Drawing.Size(30, 20);
			this.textBaseUnits.TabIndex = 8;
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(241, 144);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(251, 17);
			this.label17.TabIndex = 0;
			this.label17.Text = "(zero unless for some medical claims)";
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(82, 64);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(121, 13);
			this.label18.TabIndex = 0;
			this.label18.Text = "Ins. Subst Code";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSubstitutionCode
			// 
			this.textSubstitutionCode.Location = new System.Drawing.Point(205, 61);
			this.textSubstitutionCode.MaxLength = 255;
			this.textSubstitutionCode.Name = "textSubstitutionCode";
			this.textSubstitutionCode.Size = new System.Drawing.Size(100, 20);
			this.textSubstitutionCode.TabIndex = 3;
			// 
			// label19
			// 
			this.label19.Location = new System.Drawing.Point(306, 62);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(46, 18);
			this.label19.TabIndex = 0;
			this.label19.Text = "Only if";
			this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboSubstOnlyIf
			// 
			this.comboSubstOnlyIf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSubstOnlyIf.FormattingEnabled = true;
			this.comboSubstOnlyIf.Location = new System.Drawing.Point(347, 60);
			this.comboSubstOnlyIf.Name = "comboSubstOnlyIf";
			this.comboSubstOnlyIf.Size = new System.Drawing.Size(145, 21);
			this.comboSubstOnlyIf.TabIndex = 4;
			// 
			// checkMultiVisit
			// 
			this.checkMultiVisit.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMultiVisit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMultiVisit.Location = new System.Drawing.Point(60, 222);
			this.checkMultiVisit.Name = "checkMultiVisit";
			this.checkMultiVisit.Size = new System.Drawing.Size(158, 18);
			this.checkMultiVisit.TabIndex = 12;
			this.checkMultiVisit.Text = "Multi Visit";
			this.checkMultiVisit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkMultiVisit.UseVisualStyleBackColor = true;
			// 
			// labelDrugNDC
			// 
			this.labelDrugNDC.Location = new System.Drawing.Point(100, 164);
			this.labelDrugNDC.Name = "labelDrugNDC";
			this.labelDrugNDC.Size = new System.Drawing.Size(103, 13);
			this.labelDrugNDC.TabIndex = 0;
			this.labelDrugNDC.Text = "Drug NDC";
			this.labelDrugNDC.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelRevenueCode
			// 
			this.labelRevenueCode.Location = new System.Drawing.Point(57, 184);
			this.labelRevenueCode.Name = "labelRevenueCode";
			this.labelRevenueCode.Size = new System.Drawing.Size(146, 13);
			this.labelRevenueCode.TabIndex = 0;
			this.labelRevenueCode.Text = "Default Revenue Code";
			this.labelRevenueCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDrugNDC
			// 
			this.textDrugNDC.Location = new System.Drawing.Point(205, 161);
			this.textDrugNDC.Name = "textDrugNDC";
			this.textDrugNDC.Size = new System.Drawing.Size(100, 20);
			this.textDrugNDC.TabIndex = 9;
			// 
			// textRevenueCode
			// 
			this.textRevenueCode.Location = new System.Drawing.Point(205, 181);
			this.textRevenueCode.Name = "textRevenueCode";
			this.textRevenueCode.Size = new System.Drawing.Size(100, 20);
			this.textRevenueCode.TabIndex = 10;
			// 
			// label20
			// 
			this.label20.Location = new System.Drawing.Point(311, 164);
			this.label20.Name = "label20";
			this.label20.Size = new System.Drawing.Size(181, 17);
			this.label20.TabIndex = 0;
			this.label20.Text = "(11 digits or blank)";
			// 
			// label21
			// 
			this.label21.Location = new System.Drawing.Point(224, 276);
			this.label21.Name = "label21";
			this.label21.Size = new System.Drawing.Size(181, 17);
			this.label21.TabIndex = 0;
			this.label21.Text = "(crown, bridge, denture, RPD)";
			this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboProvNumDefault
			// 
			this.comboProvNumDefault.Location = new System.Drawing.Point(205, 349);
			this.comboProvNumDefault.Name = "comboProvNumDefault";
			this.comboProvNumDefault.Size = new System.Drawing.Size(121, 21);
			this.comboProvNumDefault.TabIndex = 17;
			// 
			// label22
			// 
			this.label22.Location = new System.Drawing.Point(100, 354);
			this.label22.Name = "label22";
			this.label22.Size = new System.Drawing.Size(103, 13);
			this.label22.TabIndex = 0;
			this.label22.Text = "Assign To Prov";
			this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label4.Location = new System.Drawing.Point(947, 512);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(122, 28);
			this.label4.TabIndex = 0;
			this.label4.Text = "View provider and clinic specific fees";
			this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label23
			// 
			this.label23.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label23.Location = new System.Drawing.Point(939, 570);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(122, 17);
			this.label23.TabIndex = 0;
			this.label23.Text = "View all fee changes";
			this.label23.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// labelTimeUnits
			// 
			this.labelTimeUnits.Location = new System.Drawing.Point(126, 376);
			this.labelTimeUnits.Name = "labelTimeUnits";
			this.labelTimeUnits.Size = new System.Drawing.Size(77, 13);
			this.labelTimeUnits.TabIndex = 0;
			this.labelTimeUnits.Text = "Time Units";
			this.labelTimeUnits.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelTimeUnits.Visible = false;
			// 
			// textTimeUnits
			// 
			this.textTimeUnits.Location = new System.Drawing.Point(205, 372);
			this.textTimeUnits.MaxVal = 100000000D;
			this.textTimeUnits.MinVal = -100000000D;
			this.textTimeUnits.Name = "textTimeUnits";
			this.textTimeUnits.Size = new System.Drawing.Size(30, 20);
			this.textTimeUnits.TabIndex = 18;
			this.textTimeUnits.Visible = false;
			// 
			// checkIsRadiology
			// 
			this.checkIsRadiology.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsRadiology.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsRadiology.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.checkIsRadiology.Location = new System.Drawing.Point(44, 294);
			this.checkIsRadiology.Name = "checkIsRadiology";
			this.checkIsRadiology.Size = new System.Drawing.Size(174, 18);
			this.checkIsRadiology.TabIndex = 32;
			this.checkIsRadiology.Text = "Is Radiology";
			this.checkIsRadiology.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(224, 294);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(181, 17);
			this.label24.TabIndex = 33;
			this.label24.Text = "(bitewing, panoramic, FMX)";
			this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butMore
			// 
			this.butMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butMore.Location = new System.Drawing.Point(985, 541);
			this.butMore.Name = "butMore";
			this.butMore.Size = new System.Drawing.Size(75, 26);
			this.butMore.TabIndex = 25;
			this.butMore.Text = "More";
			this.butMore.Click += new System.EventHandler(this.butMore_Click);
			// 
			// butAddNote
			// 
			this.butAddNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNote.Location = new System.Drawing.Point(687, 108);
			this.butAddNote.Name = "butAddNote";
			this.butAddNote.Size = new System.Drawing.Size(88, 26);
			this.butAddNote.TabIndex = 20;
			this.butAddNote.Text = "Add Note";
			this.butAddNote.Click += new System.EventHandler(this.butAddNote_Click);
			// 
			// tbTime
			// 
			this.tbTime.BackColor = System.Drawing.SystemColors.Window;
			this.tbTime.Location = new System.Drawing.Point(8, 44);
			this.tbTime.Name = "tbTime";
			this.tbTime.ScrollValue = 150;
			this.tbTime.SelectedIndices = new int[0];
			this.tbTime.SelectionMode = System.Windows.Forms.SelectionMode.None;
			this.tbTime.Size = new System.Drawing.Size(15, 561);
			this.tbTime.TabIndex = 0;
			this.tbTime.TabStop = false;
			// 
			// butColorClear
			// 
			this.butColorClear.Location = new System.Drawing.Point(230, 202);
			this.butColorClear.Name = "butColorClear";
			this.butColorClear.Size = new System.Drawing.Size(50, 20);
			this.butColorClear.TabIndex = 11;
			this.butColorClear.Text = "none";
			this.butColorClear.Click += new System.EventHandler(this.butColorClear_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(986, 661);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 26);
			this.butCancel.TabIndex = 28;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAuditTrail
			// 
			this.butAuditTrail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAuditTrail.Location = new System.Drawing.Point(985, 594);
			this.butAuditTrail.Name = "butAuditTrail";
			this.butAuditTrail.Size = new System.Drawing.Size(75, 26);
			this.butAuditTrail.TabIndex = 26;
			this.butAuditTrail.Text = "Audit Trail";
			this.butAuditTrail.Click += new System.EventHandler(this.butAuditTrail_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(985, 628);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 26);
			this.butOK.TabIndex = 27;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butAutoNote
			// 
			this.butAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAutoNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAutoNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAutoNote.Location = new System.Drawing.Point(687, 20);
			this.butAutoNote.Name = "butAutoNote";
			this.butAutoNote.Size = new System.Drawing.Size(88, 26);
			this.butAutoNote.TabIndex = 37;
			this.butAutoNote.Text = "Auto Note";
			this.butAutoNote.Click += new System.EventHandler(this.butAutoNote_Click);
			// 
			// DefaultTP
			// 
			this.DefaultTP.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DefaultTP.Location = new System.Drawing.Point(6, 3);
			this.DefaultTP.Name = "DefaultTP";
			this.DefaultTP.Size = new System.Drawing.Size(164, 14);
			this.DefaultTP.TabIndex = 39;
			this.DefaultTP.Text = "Default TP Note";
			this.DefaultTP.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butAutoTpNote
			// 
			this.butAutoTpNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAutoTpNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAutoTpNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAutoTpNote.Location = new System.Drawing.Point(687, 20);
			this.butAutoTpNote.Name = "butAutoTpNote";
			this.butAutoTpNote.Size = new System.Drawing.Size(88, 26);
			this.butAutoTpNote.TabIndex = 40;
			this.butAutoTpNote.Text = "Auto Note";
			this.butAutoTpNote.Click += new System.EventHandler(this.butAutoTPNote_Click);
			// 
			// butAddTpNote
			// 
			this.butAddTpNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddTpNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTpNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTpNote.Location = new System.Drawing.Point(687, 108);
			this.butAddTpNote.Name = "butAddTpNote";
			this.butAddTpNote.Size = new System.Drawing.Size(88, 26);
			this.butAddTpNote.TabIndex = 42;
			this.butAddTpNote.Text = "Add Note";
			this.butAddTpNote.Click += new System.EventHandler(this.butAddTpNote_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Location = new System.Drawing.Point(72, 416);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(789, 275);
			this.tabControl1.TabIndex = 43;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.label10);
			this.tabPage1.Controls.Add(this.textNote);
			this.tabPage1.Controls.Add(this.gridNotes);
			this.tabPage1.Controls.Add(this.butAddNote);
			this.tabPage1.Controls.Add(this.butAutoNote);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(781, 249);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Completed Note";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(6, 20);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(675, 81);
			this.textNote.TabIndex = 21;
			this.textNote.Text = "";
			// 
			// gridNotes
			// 
			this.gridNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridNotes.Location = new System.Drawing.Point(6, 107);
			this.gridNotes.Name = "gridNotes";
			this.gridNotes.Size = new System.Drawing.Size(675, 136);
			this.gridNotes.TabIndex = 0;
			this.gridNotes.TabStop = false;
			this.gridNotes.Title = "Notes and Times for Specific Providers";
			this.gridNotes.TranslationName = "TableProcedureNotes";
			this.gridNotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridNotes_CellDoubleClick);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.DefaultTP);
			this.tabPage2.Controls.Add(this.butAddTpNote);
			this.tabPage2.Controls.Add(this.butAutoTpNote);
			this.tabPage2.Controls.Add(this.gridTpNotes);
			this.tabPage2.Controls.Add(this.textTpNote);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(781, 249);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "TP\'d Note";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// gridTpNotes
			// 
			this.gridTpNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridTpNotes.Location = new System.Drawing.Point(6, 107);
			this.gridTpNotes.Name = "gridTpNotes";
			this.gridTpNotes.Size = new System.Drawing.Size(675, 136);
			this.gridTpNotes.TabIndex = 41;
			this.gridTpNotes.TabStop = false;
			this.gridTpNotes.Title = "Notes and Times for Specific Providers";
			this.gridTpNotes.TranslationName = "TableTpProcedureNotes";
			this.gridTpNotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridTpNotes_CellDoubleClick);
			// 
			// textTpNote
			// 
			this.textTpNote.AcceptsTab = true;
			this.textTpNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textTpNote.BackColor = System.Drawing.SystemColors.Window;
			this.textTpNote.DetectLinksEnabled = false;
			this.textTpNote.DetectUrls = false;
			this.textTpNote.Location = new System.Drawing.Point(6, 20);
			this.textTpNote.Name = "textTpNote";
			this.textTpNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Procedure;
			this.textTpNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textTpNote.Size = new System.Drawing.Size(675, 81);
			this.textTpNote.TabIndex = 38;
			this.textTpNote.Text = "";
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.labelDefaultPreauthNote);
			this.tabPage3.Controls.Add(this.label25);
			this.tabPage3.Controls.Add(this.textDefaultClaimNote);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(781, 249);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Default Claim Note";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// labelDefaultPreauthNote
			// 
			this.labelDefaultPreauthNote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDefaultPreauthNote.Location = new System.Drawing.Point(147, 3);
			this.labelDefaultPreauthNote.Name = "labelDefaultPreauthNote";
			this.labelDefaultPreauthNote.Size = new System.Drawing.Size(301, 15);
			this.labelDefaultPreauthNote.TabIndex = 41;
			this.labelDefaultPreauthNote.Text = "The default claim note will also be used in preauths.";
			this.labelDefaultPreauthNote.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label25
			// 
			this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label25.Location = new System.Drawing.Point(6, 3);
			this.label25.Name = "label25";
			this.label25.Size = new System.Drawing.Size(137, 14);
			this.label25.TabIndex = 40;
			this.label25.Text = "Default Claim Note";
			this.label25.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDefaultClaimNote
			// 
			this.textDefaultClaimNote.AcceptsTab = true;
			this.textDefaultClaimNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDefaultClaimNote.BackColor = System.Drawing.SystemColors.Window;
			this.textDefaultClaimNote.DetectLinksEnabled = false;
			this.textDefaultClaimNote.DetectUrls = false;
			this.textDefaultClaimNote.Location = new System.Drawing.Point(9, 20);
			this.textDefaultClaimNote.Name = "textDefaultClaimNote";
			this.textDefaultClaimNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Claim;
			this.textDefaultClaimNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDefaultClaimNote.Size = new System.Drawing.Size(758, 211);
			this.textDefaultClaimNote.TabIndex = 35;
			this.textDefaultClaimNote.Text = "";
			// 
			// checkBypassLockDate
			// 
			this.checkBypassLockDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBypassLockDate.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBypassLockDate.Location = new System.Drawing.Point(31, 393);
			this.checkBypassLockDate.Name = "checkBypassLockDate";
			this.checkBypassLockDate.Size = new System.Drawing.Size(187, 18);
			this.checkBypassLockDate.TabIndex = 44;
			this.checkBypassLockDate.Text = "Bypass Global Lock Date";
			this.checkBypassLockDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridFees
			// 
			this.gridFees.Location = new System.Drawing.Point(860, 17);
			this.gridFees.Name = "gridFees";
			this.gridFees.Size = new System.Drawing.Size(199, 355);
			this.gridFees.TabIndex = 0;
			this.gridFees.TabStop = false;
			this.gridFees.Title = "Default Fees";
			this.gridFees.TranslationName = "TableProcFee";
			this.gridFees.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFees_CellDoubleClick);
			// 
			// textPaintText
			// 
			this.textPaintText.Location = new System.Drawing.Point(495, 256);
			this.textPaintText.Name = "textPaintText";
			this.textPaintText.Size = new System.Drawing.Size(118, 20);
			this.textPaintText.TabIndex = 46;
			// 
			// labelPaintText
			// 
			this.labelPaintText.Location = new System.Drawing.Point(407, 259);
			this.labelPaintText.Name = "labelPaintText";
			this.labelPaintText.Size = new System.Drawing.Size(86, 13);
			this.labelPaintText.TabIndex = 45;
			this.labelPaintText.Text = "Paint Text";
			this.labelPaintText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsTaxed
			// 
			this.checkIsAutoTaxed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAutoTaxed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsAutoTaxed.Location = new System.Drawing.Point(114, 312);
			this.checkIsAutoTaxed.Name = "checkIsAutoTaxed";
			this.checkIsAutoTaxed.Size = new System.Drawing.Size(104, 18);
			this.checkIsAutoTaxed.TabIndex = 47;
			this.checkIsAutoTaxed.Text = "Auto Tax";
			this.checkIsAutoTaxed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsAutoTaxed.UseVisualStyleBackColor = true;
			// 
			// checkToothRange
			// 
			this.checkToothRange.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkToothRange.Location = new System.Drawing.Point(616, 127);
			this.checkToothRange.Name = "checkToothRange";
			this.checkToothRange.Size = new System.Drawing.Size(118, 18);
			this.checkToothRange.TabIndex = 48;
			this.checkToothRange.Text = "&& Tooth Range";
			this.checkToothRange.UseVisualStyleBackColor = true;
			// 
			// FormProcCodeEdit
			// 
			this.ClientSize = new System.Drawing.Size(1072, 696);
			this.Controls.Add(this.checkToothRange);
			this.Controls.Add(this.checkIsAutoTaxed);
			this.Controls.Add(this.textPaintText);
			this.Controls.Add(this.labelPaintText);
			this.Controls.Add(this.checkBypassLockDate);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.checkIsRadiology);
			this.Controls.Add(this.textTimeUnits);
			this.Controls.Add(this.labelTimeUnits);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.butMore);
			this.Controls.Add(this.label22);
			this.Controls.Add(this.comboProvNumDefault);
			this.Controls.Add(this.label21);
			this.Controls.Add(this.checkMultiVisit);
			this.Controls.Add(this.comboSubstOnlyIf);
			this.Controls.Add(this.label19);
			this.Controls.Add(this.textSubstitutionCode);
			this.Controls.Add(this.butSlider);
			this.Controls.Add(this.tbTime);
			this.Controls.Add(this.label18);
			this.Controls.Add(this.label20);
			this.Controls.Add(this.label17);
			this.Controls.Add(this.textRevenueCode);
			this.Controls.Add(this.textDrugNDC);
			this.Controls.Add(this.textBaseUnits);
			this.Controls.Add(this.labelRevenueCode);
			this.Controls.Add(this.labelDrugNDC);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.checkIsCanadianLab);
			this.Controls.Add(this.textLaymanTerm);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butColorClear);
			this.Controls.Add(this.labelColor);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.listPaintType);
			this.Controls.Add(this.gridFees);
			this.Controls.Add(this.textMedicalCode);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.checkIsProsth);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textAlternateCode1);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.checkIsHygiene);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textTime2);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.textAbbrev);
			this.Controls.Add(this.textProcCode);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.listCategory);
			this.Controls.Add(this.checkNoBillIns);
			this.Controls.Add(this.listTreatArea);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butAuditTrail);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.labelTreatArea);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProcCodeEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Procedure Code";
			this.Load += new System.EventHandler(this.FormProcCodeEdit_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelTreatArea;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label10;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ListBoxOD listTreatArea;
		private OpenDental.UI.ListBoxOD listCategory;
		private System.Windows.Forms.TextBox textProcCode;
		private System.Windows.Forms.TextBox textAbbrev;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.CheckBox checkNoBillIns;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button butSlider;
		private System.Windows.Forms.TextBox textTime2;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkIsHygiene;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textAlternateCode1;
		private OpenDental.ODtextBox textNote;
		private System.Windows.Forms.CheckBox checkIsProsth;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.TextBox textMedicalCode;
		private OpenDental.UI.GridOD gridFees;
		private Label label15;
		private OpenDental.UI.ListBoxOD listPaintType;
		private Label labelColor;
		private System.Windows.Forms.Button butColor;
		private OpenDental.UI.Button butColorClear;
		private TextBox textLaymanTerm;
		private Label label2;
		private CheckBox checkIsCanadianLab;
		private Label label16;
		private ValidNum textBaseUnits;
		private Label label17;
		private Label label18;
		private TextBox textSubstitutionCode;
		private UI.GridOD gridNotes;
		private OpenDental.UI.Button butAddNote;
		private Label label19;
		private ComboBox comboSubstOnlyIf;
		private CheckBox checkMultiVisit;
		private Label labelDrugNDC;
		private Label labelRevenueCode;
		private TextBox textDrugNDC;
		private TextBox textRevenueCode;
		private Label label20;
		private Label label21;
		private UI.ComboBoxOD comboProvNumDefault;
		private Label label22;
		private UI.Button butAuditTrail;
		private UI.Button butMore;
		private Label label4;
		private Label label23;
		private Label labelTimeUnits;
		private ValidDouble textTimeUnits;
		private CheckBox checkIsRadiology;
		private Label label24;
		private ODtextBox textDefaultClaimNote;
		private UI.Button butAutoNote;
		private ODtextBox textTpNote;
		private Label DefaultTP;
		private UI.Button butAutoTpNote;
		private UI.GridOD gridTpNotes;
		private UI.Button butAddTpNote;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private TabPage tabPage3;
		private Label label25;
		private CheckBox checkBypassLockDate;
		private Label labelDefaultPreauthNote;
		private TextBox textPaintText;
		private Label labelPaintText;
		private CheckBox checkIsAutoTaxed;
		private OpenDental.TableTimeBar tbTime;
		private CheckBox checkToothRange;
	}
}
