using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormProviderSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProviderSetup));
			this.butClose = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.groupDentalSchools = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textProvNum = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textFirstName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textLastName = new System.Windows.Forms.TextBox();
			this.radioInstructors = new System.Windows.Forms.RadioButton();
			this.radioStudents = new System.Windows.Forms.RadioButton();
			this.radioAll = new System.Windows.Forms.RadioButton();
			this.comboClass = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butCreateUsers = new OpenDental.UI.Button();
			this.groupCreateUsers = new System.Windows.Forms.GroupBox();
			this.comboUserGroup = new OpenDental.UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.groupMovePats = new System.Windows.Forms.GroupBox();
			this.butMoveSec = new OpenDental.UI.Button();
			this.butProvPick = new OpenDental.UI.Button();
			this.textMoveTo = new System.Windows.Forms.TextBox();
			this.butReassign = new OpenDental.UI.Button();
			this.labelReassign = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.butMovePri = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butStudBulkEdit = new OpenDental.UI.Button();
			this.checkShowDeleted = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butAlphabetize = new OpenDental.UI.Button();
			this.checkShowHidden = new System.Windows.Forms.CheckBox();
			this.checkShowPatientCount = new System.Windows.Forms.CheckBox();
			this.labelSearch = new System.Windows.Forms.Label();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.groupDentalSchools.SuspendLayout();
			this.groupCreateUsers.SuspendLayout();
			this.groupMovePats.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(885, 665);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(82, 24);
			this.butClose.TabIndex = 8;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(6, 58);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(82, 24);
			this.butDown.TabIndex = 5;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(6, 19);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(82, 24);
			this.butUp.TabIndex = 4;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(885, 522);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(82, 24);
			this.butAdd.TabIndex = 6;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// groupDentalSchools
			// 
			this.groupDentalSchools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupDentalSchools.Controls.Add(this.label8);
			this.groupDentalSchools.Controls.Add(this.label7);
			this.groupDentalSchools.Controls.Add(this.textProvNum);
			this.groupDentalSchools.Controls.Add(this.label6);
			this.groupDentalSchools.Controls.Add(this.textFirstName);
			this.groupDentalSchools.Controls.Add(this.label4);
			this.groupDentalSchools.Controls.Add(this.textLastName);
			this.groupDentalSchools.Controls.Add(this.radioInstructors);
			this.groupDentalSchools.Controls.Add(this.radioStudents);
			this.groupDentalSchools.Controls.Add(this.radioAll);
			this.groupDentalSchools.Controls.Add(this.comboClass);
			this.groupDentalSchools.Controls.Add(this.label1);
			this.groupDentalSchools.Location = new System.Drawing.Point(703, 12);
			this.groupDentalSchools.Name = "groupDentalSchools";
			this.groupDentalSchools.Size = new System.Drawing.Size(273, 174);
			this.groupDentalSchools.TabIndex = 1;
			this.groupDentalSchools.TabStop = false;
			this.groupDentalSchools.Text = "Dental Schools Search by:";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(116, 48);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(148, 50);
			this.label8.TabIndex = 26;
			this.label8.Text = "These selections will also affect the functionality of the Add button.";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 146);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(90, 18);
			this.label7.TabIndex = 25;
			this.label7.Text = "ProvNum";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textProvNum
			// 
			this.textProvNum.Location = new System.Drawing.Point(98, 145);
			this.textProvNum.MaxLength = 15;
			this.textProvNum.Name = "textProvNum";
			this.textProvNum.Size = new System.Drawing.Size(166, 20);
			this.textProvNum.TabIndex = 6;
			this.textProvNum.TextChanged += new System.EventHandler(this.textProvNum_TextChanged);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(8, 124);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(90, 18);
			this.label6.TabIndex = 23;
			this.label6.Text = "First Name";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFirstName
			// 
			this.textFirstName.Location = new System.Drawing.Point(98, 123);
			this.textFirstName.MaxLength = 15;
			this.textFirstName.Name = "textFirstName";
			this.textFirstName.Size = new System.Drawing.Size(166, 20);
			this.textFirstName.TabIndex = 5;
			this.textFirstName.TextChanged += new System.EventHandler(this.textFirstName_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(8, 102);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(90, 18);
			this.label4.TabIndex = 21;
			this.label4.Text = "Last Name";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLastName
			// 
			this.textLastName.Location = new System.Drawing.Point(98, 101);
			this.textLastName.MaxLength = 15;
			this.textLastName.Name = "textLastName";
			this.textLastName.Size = new System.Drawing.Size(166, 20);
			this.textLastName.TabIndex = 4;
			this.textLastName.TextChanged += new System.EventHandler(this.textLastName_TextChanged);
			// 
			// radioInstructors
			// 
			this.radioInstructors.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioInstructors.Location = new System.Drawing.Point(6, 80);
			this.radioInstructors.Name = "radioInstructors";
			this.radioInstructors.Size = new System.Drawing.Size(104, 18);
			this.radioInstructors.TabIndex = 3;
			this.radioInstructors.Text = "Instructors";
			this.radioInstructors.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioInstructors.UseVisualStyleBackColor = true;
			this.radioInstructors.Click += new System.EventHandler(this.radioInstructors_Click);
			// 
			// radioStudents
			// 
			this.radioStudents.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStudents.Location = new System.Drawing.Point(6, 63);
			this.radioStudents.Name = "radioStudents";
			this.radioStudents.Size = new System.Drawing.Size(104, 18);
			this.radioStudents.TabIndex = 2;
			this.radioStudents.Text = "Students";
			this.radioStudents.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStudents.UseVisualStyleBackColor = true;
			this.radioStudents.Click += new System.EventHandler(this.radioStudents_Click);
			// 
			// radioAll
			// 
			this.radioAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAll.Checked = true;
			this.radioAll.Location = new System.Drawing.Point(6, 46);
			this.radioAll.Name = "radioAll";
			this.radioAll.Size = new System.Drawing.Size(104, 18);
			this.radioAll.TabIndex = 1;
			this.radioAll.TabStop = true;
			this.radioAll.Text = "All";
			this.radioAll.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioAll.UseVisualStyleBackColor = true;
			this.radioAll.Click += new System.EventHandler(this.radioAll_Click);
			// 
			// comboClass
			// 
			this.comboClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboClass.FormattingEnabled = true;
			this.comboClass.Location = new System.Drawing.Point(98, 19);
			this.comboClass.Name = "comboClass";
			this.comboClass.Size = new System.Drawing.Size(166, 21);
			this.comboClass.TabIndex = 0;
			this.comboClass.SelectionChangeCommitted += new System.EventHandler(this.comboClass_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 18);
			this.label1.TabIndex = 16;
			this.label1.Text = "Classes";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butCreateUsers
			// 
			this.butCreateUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCreateUsers.Location = new System.Drawing.Point(182, 42);
			this.butCreateUsers.Name = "butCreateUsers";
			this.butCreateUsers.Size = new System.Drawing.Size(82, 24);
			this.butCreateUsers.TabIndex = 15;
			this.butCreateUsers.Text = "Create";
			this.butCreateUsers.Click += new System.EventHandler(this.butCreateUsers_Click);
			// 
			// groupCreateUsers
			// 
			this.groupCreateUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupCreateUsers.Controls.Add(this.comboUserGroup);
			this.groupCreateUsers.Controls.Add(this.label3);
			this.groupCreateUsers.Controls.Add(this.butCreateUsers);
			this.groupCreateUsers.Location = new System.Drawing.Point(703, 192);
			this.groupCreateUsers.Name = "groupCreateUsers";
			this.groupCreateUsers.Size = new System.Drawing.Size(273, 76);
			this.groupCreateUsers.TabIndex = 2;
			this.groupCreateUsers.TabStop = false;
			this.groupCreateUsers.Text = "Create Users";
			// 
			// comboUserGroup
			// 
			this.comboUserGroup.BackColor = System.Drawing.SystemColors.Window;
			this.comboUserGroup.Location = new System.Drawing.Point(98, 14);
			this.comboUserGroup.Name = "comboUserGroup";
			this.comboUserGroup.SelectionModeMulti = true;
			this.comboUserGroup.Size = new System.Drawing.Size(166, 21);
			this.comboUserGroup.TabIndex = 19;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 14);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(91, 18);
			this.label3.TabIndex = 18;
			this.label3.Text = "User Group";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupMovePats
			// 
			this.groupMovePats.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupMovePats.Controls.Add(this.butMoveSec);
			this.groupMovePats.Controls.Add(this.butProvPick);
			this.groupMovePats.Controls.Add(this.textMoveTo);
			this.groupMovePats.Controls.Add(this.butReassign);
			this.groupMovePats.Controls.Add(this.labelReassign);
			this.groupMovePats.Controls.Add(this.label2);
			this.groupMovePats.Controls.Add(this.butMovePri);
			this.groupMovePats.Location = new System.Drawing.Point(703, 273);
			this.groupMovePats.Name = "groupMovePats";
			this.groupMovePats.Size = new System.Drawing.Size(273, 132);
			this.groupMovePats.TabIndex = 3;
			this.groupMovePats.TabStop = false;
			this.groupMovePats.Text = "Move Patients";
			// 
			// butMoveSec
			// 
			this.butMoveSec.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMoveSec.Location = new System.Drawing.Point(182, 46);
			this.butMoveSec.Name = "butMoveSec";
			this.butMoveSec.Size = new System.Drawing.Size(82, 24);
			this.butMoveSec.TabIndex = 15;
			this.butMoveSec.Text = "Move Sec";
			this.butMoveSec.UseVisualStyleBackColor = true;
			this.butMoveSec.Click += new System.EventHandler(this.butMoveSec_Click);
			// 
			// butProvPick
			// 
			this.butProvPick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butProvPick.Location = new System.Drawing.Point(237, 17);
			this.butProvPick.Name = "butProvPick";
			this.butProvPick.Size = new System.Drawing.Size(27, 26);
			this.butProvPick.TabIndex = 23;
			this.butProvPick.Text = "...";
			this.butProvPick.Click += new System.EventHandler(this.butProvPick_Click);
			// 
			// textMoveTo
			// 
			this.textMoveTo.Location = new System.Drawing.Point(98, 19);
			this.textMoveTo.MaxLength = 15;
			this.textMoveTo.Name = "textMoveTo";
			this.textMoveTo.ReadOnly = true;
			this.textMoveTo.Size = new System.Drawing.Size(135, 20);
			this.textMoveTo.TabIndex = 22;
			// 
			// butReassign
			// 
			this.butReassign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butReassign.Location = new System.Drawing.Point(182, 98);
			this.butReassign.Name = "butReassign";
			this.butReassign.Size = new System.Drawing.Size(82, 24);
			this.butReassign.TabIndex = 15;
			this.butReassign.Text = "Reassign";
			this.butReassign.Click += new System.EventHandler(this.butReassign_Click);
			// 
			// labelReassign
			// 
			this.labelReassign.Location = new System.Drawing.Point(8, 98);
			this.labelReassign.Name = "labelReassign";
			this.labelReassign.Size = new System.Drawing.Size(168, 31);
			this.labelReassign.TabIndex = 18;
			this.labelReassign.Text = "Reassigns primary provider to most-used provider\r\n";
			this.labelReassign.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(3, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(94, 18);
			this.label2.TabIndex = 18;
			this.label2.Text = "To Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butMovePri
			// 
			this.butMovePri.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMovePri.Location = new System.Drawing.Point(94, 46);
			this.butMovePri.Name = "butMovePri";
			this.butMovePri.Size = new System.Drawing.Size(82, 24);
			this.butMovePri.TabIndex = 15;
			this.butMovePri.Text = "Move Pri";
			this.butMovePri.Click += new System.EventHandler(this.butMovePri_Click);
			// 
			// gridMain
			// 
			this.gridMain.AllowSortingByColumn = true;
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(7, 31);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(688, 664);
			this.gridMain.TabIndex = 13;
			this.gridMain.Title = "Providers";
			this.gridMain.TranslationName = "TableProviderSetup";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butStudBulkEdit
			// 
			this.butStudBulkEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butStudBulkEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butStudBulkEdit.Location = new System.Drawing.Point(865, 554);
			this.butStudBulkEdit.Name = "butStudBulkEdit";
			this.butStudBulkEdit.Size = new System.Drawing.Size(102, 24);
			this.butStudBulkEdit.TabIndex = 7;
			this.butStudBulkEdit.Text = "Student Bulk Edit";
			this.butStudBulkEdit.Click += new System.EventHandler(this.butStudBulkEdit_Click);
			// 
			// checkShowDeleted
			// 
			this.checkShowDeleted.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowDeleted.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDeleted.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowDeleted.Location = new System.Drawing.Point(561, 12);
			this.checkShowDeleted.Name = "checkShowDeleted";
			this.checkShowDeleted.Size = new System.Drawing.Size(134, 14);
			this.checkShowDeleted.TabIndex = 27;
			this.checkShowDeleted.Text = "Show Deleted";
			this.checkShowDeleted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowDeleted.CheckedChanged += new System.EventHandler(this.checkShowDeleted_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butAlphabetize);
			this.groupBox1.Controls.Add(this.butUp);
			this.groupBox1.Controls.Add(this.butDown);
			this.groupBox1.Location = new System.Drawing.Point(703, 411);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(273, 91);
			this.groupBox1.TabIndex = 19;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Provider Order";
			// 
			// butAlphabetize
			// 
			this.butAlphabetize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAlphabetize.Location = new System.Drawing.Point(133, 38);
			this.butAlphabetize.Name = "butAlphabetize";
			this.butAlphabetize.Size = new System.Drawing.Size(131, 26);
			this.butAlphabetize.TabIndex = 16;
			this.butAlphabetize.Text = "Alphabetize Providers";
			this.butAlphabetize.Click += new System.EventHandler(this.butAlphabetize_Click);
			// 
			// checkShowHidden
			// 
			this.checkShowHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Checked = true;
			this.checkShowHidden.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowHidden.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowHidden.Location = new System.Drawing.Point(421, 12);
			this.checkShowHidden.Name = "checkShowHidden";
			this.checkShowHidden.Size = new System.Drawing.Size(134, 14);
			this.checkShowHidden.TabIndex = 28;
			this.checkShowHidden.Text = "Show Hidden";
			this.checkShowHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowHidden.Click += new System.EventHandler(this.checkShowHidden_Click);
			// 
			// checkShowPatientCount
			// 
			this.checkShowPatientCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkShowPatientCount.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPatientCount.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowPatientCount.Location = new System.Drawing.Point(249, 12);
			this.checkShowPatientCount.Name = "checkShowPatientCount";
			this.checkShowPatientCount.Size = new System.Drawing.Size(166, 14);
			this.checkShowPatientCount.TabIndex = 29;
			this.checkShowPatientCount.Text = "Show Patient Count";
			this.checkShowPatientCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowPatientCount.CheckedChanged += new System.EventHandler(this.checkShowPatientCount_CheckedChanged);
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(6, 7);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelSearch.Size = new System.Drawing.Size(55, 13);
			this.labelSearch.TabIndex = 45;
			this.labelSearch.Text = "Search";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(62, 5);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(181, 20);
			this.textSearch.TabIndex = 34;
			// 
			// FormProviderSetup
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(982, 707);
			this.Controls.Add(this.labelSearch);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.checkShowPatientCount);
			this.Controls.Add(this.checkShowHidden);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.checkShowDeleted);
			this.Controls.Add(this.butStudBulkEdit);
			this.Controls.Add(this.groupMovePats);
			this.Controls.Add(this.groupCreateUsers);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.groupDentalSchools);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormProviderSetup";
			this.ShowInTaskbar = false;
			this.Text = "Provider Setup";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.FormProviderSelect_Closing);
			this.Load += new System.EventHandler(this.FormProviderSetup_Load);
			this.Shown += new System.EventHandler(this.FormProviderSetup_Shown);
			this.groupDentalSchools.ResumeLayout(false);
			this.groupDentalSchools.PerformLayout();
			this.groupCreateUsers.ResumeLayout(false);
			this.groupMovePats.ResumeLayout(false);
			this.groupMovePats.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private OpenDental.UI.Button butDown;
		private OpenDental.UI.Button butUp;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupDentalSchools;
		private ComboBox comboClass;
		private Label label1;
		private OpenDental.UI.Button butCreateUsers;
		private GroupBox groupCreateUsers;
		private Label label3;
		private GroupBox groupMovePats;
		private Label label2;
		private UI.Button butMovePri;
		private UI.Button butReassign;
		private Label labelReassign;
		private RadioButton radioInstructors;
		private RadioButton radioStudents;
		private RadioButton radioAll;
		private Label label4;
		private TextBox textLastName;
		private UI.Button butProvPick;
		private TextBox textMoveTo;
		private Label label7;
		private TextBox textProvNum;
		private Label label6;
		private TextBox textFirstName;
		private UI.Button butStudBulkEdit;
		private Label label8;
		private UI.Button butMoveSec;
		private CheckBox checkShowDeleted;
		private GroupBox groupBox1;
		private UI.Button butAlphabetize;
		private CheckBox checkShowHidden;
		private UI.ComboBoxOD comboUserGroup;
		private CheckBox checkShowPatientCount;
		private Label labelSearch;
		private TextBox textSearch;
	}
}
