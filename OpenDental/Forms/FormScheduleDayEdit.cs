using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	///<summary></summary>
	public class FormScheduleDayEdit:FormODBase {
		private OpenDental.UI.Button butAddTime;
		private OpenDental.UI.Button butCloseOffice;
		private OpenDental.UI.Button butCancel;
		private System.ComponentModel.Container components = null;
		private DateTime _dateSched;
		private OpenDental.UI.GridOD gridMain;
		private Label labelDate;
		private GroupBox groupBox3;
		private Label label1;
		private OpenDental.UI.ListBoxOD listProv;
		private OpenDental.UI.Button butOK;
		private Label label2;
		private GroupBox groupPractice;
		private OpenDental.UI.Button butNote;
		private OpenDental.UI.Button butHoliday;
		private OpenDental.UI.Button butProvNote;
		private OpenDental.UI.ListBoxOD listEmp;
		private UI.ComboBoxOD comboProv;
		private TabControl tabControl1;
		private TabPage tabPage1;
		private TabPage tabPage2;
		private GraphScheduleDay graphScheduleDay;
		///<summary>Working copy of schedule entries.</summary>
		private List<Schedule> _listScheds;
		///<summary>Stale copy of schedule entries.</summary>
		private List<Schedule> _listSchedsOld;
		private List<Provider> _listProvs;
		private List<Employee> _listEmps;
		private TabControl tabControl2;
		private TabPage tabPageProv;
		private TabPage tabPageEmp;
		private Label label3;
		private UI.ComboBoxClinicPicker comboClinic;
		private List<Clinic> _listClinics;
		///<summary>Only used in schedule sorting. Greatly increases speed of large databases.</summary>
		private Dictionary<long,Employee> _dictEmpNumEmployee;
		///<summary>Only used in schedule sorting. Greatly increases speed of large databases.</summary>
		private Dictionary<long,Provider> _dictProvNumProvider;
		///<summary>Only used in schedule sorting. Greatly increases speed of large databases.</summary>
		private Dictionary<long,Clinic> _dictClinicNumClinic;
		private UI.Button butForward;
		private UI.Button butBack;
		private UI.Button butOkSchedule;
		public bool ShowOkSchedule = false;
		///<summary>Set by butOkSchedule only.</summary>
		public bool GotoScheduleOnClose = false;
		private UI.Button butViewGraph;
		private List<long> _listSelectedProvNums;
		private TextBox textSearch;
		private Label labelSearch;
		private List<Provider> _listProviders;
		///<summary>True if the called from FromSchedule, else False. </summary>
		private bool _isFromSchedule;
		///<summary>The provider that was selected in FormSchedule. Will be blank if no provider or multiple providers were selected. </summary>
		private string _provAbbrFilter;
		///<summary>The employee that was selected in FormSchedule. Will be blank if no employee or multiple employees were selected. </summary>
		private string _employeeNameFilter;
		///<summary>Only used once, on startup.</summary>
		private long _clinicNumInitial;

		///<summary></summary>
		public FormScheduleDayEdit(DateTime dateSched) : this(dateSched,0) {
		}

		///<summary>When clinics are enabled, this will filter the employee list box by the clinic passed in.  Pass clinicNum 0 to only show employees not assigned to a clinic.</summary>
		public FormScheduleDayEdit(DateTime dateSched,long clinicNum,string provAbbr="",string empFName="",bool isFromSchedule=false) {
			InitializeComponent();
			InitializeLayoutManager();
			_dateSched=dateSched.Date;
			_clinicNumInitial=clinicNum;
			_isFromSchedule=isFromSchedule;
			_provAbbrFilter=provAbbr;
			_employeeNameFilter=empFName;
			Lan.F(this);
		}

		///<summary></summary>
		protected override void Dispose(bool disposing){
			if(disposing){
				if(components != null){
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormScheduleDayEdit));
			this.butForward = new OpenDental.UI.Button();
			this.butBack = new OpenDental.UI.Button();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.gridMain = new OpenDental.UI.GridOD();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.graphScheduleDay = new OpenDental.GraphScheduleDay();
			this.groupPractice = new System.Windows.Forms.GroupBox();
			this.butHoliday = new OpenDental.UI.Button();
			this.butNote = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.tabControl2 = new System.Windows.Forms.TabControl();
			this.tabPageProv = new System.Windows.Forms.TabPage();
			this.label3 = new System.Windows.Forms.Label();
			this.listProv = new OpenDental.UI.ListBoxOD();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.tabPageEmp = new System.Windows.Forms.TabPage();
			this.listEmp = new OpenDental.UI.ListBoxOD();
			this.butProvNote = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butAddTime = new OpenDental.UI.Button();
			this.labelDate = new System.Windows.Forms.Label();
			this.butCloseOffice = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOkSchedule = new OpenDental.UI.Button();
			this.butViewGraph = new OpenDental.UI.Button();
			this.textSearch = new System.Windows.Forms.TextBox();
			this.labelSearch = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupPractice.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabControl2.SuspendLayout();
			this.tabPageProv.SuspendLayout();
			this.tabPageEmp.SuspendLayout();
			this.SuspendLayout();
			// 
			// butForward
			// 
			this.butForward.Image = global::OpenDental.Properties.Resources.Right;
			this.butForward.Location = new System.Drawing.Point(199, 7);
			this.butForward.Name = "butForward";
			this.butForward.Size = new System.Drawing.Size(39, 24);
			this.butForward.TabIndex = 39;
			this.butForward.Click += new System.EventHandler(this.butForward_Click);
			// 
			// butBack
			// 
			this.butBack.Image = global::OpenDental.Properties.Resources.Left;
			this.butBack.Location = new System.Drawing.Point(12, 6);
			this.butBack.Name = "butBack";
			this.butBack.Size = new System.Drawing.Size(39, 24);
			this.butBack.TabIndex = 38;
			this.butBack.Click += new System.EventHandler(this.butBack_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "Headquarters";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(782, 9);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(216, 21);
			this.comboClinic.TabIndex = 37;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 32);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(801, 635);
			this.tabControl1.TabIndex = 17;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.gridMain);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(793, 609);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "List";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// gridMain
			// 
			this.gridMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridMain.Location = new System.Drawing.Point(3, 3);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(787, 603);
			this.gridMain.TabIndex = 8;
			this.gridMain.Title = "Edit Day";
			this.gridMain.TranslationName = "TableEditDay";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.graphScheduleDay);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(793, 609);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Graph";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// graphScheduleDay
			// 
			this.graphScheduleDay.BarHeightPixels = 17;
			this.graphScheduleDay.BarSpacingPixels = 3;
			this.graphScheduleDay.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphScheduleDay.EmployeeBarColor = System.Drawing.Color.LightSkyBlue;
			this.graphScheduleDay.EmployeeTextColor = System.Drawing.Color.Black;
			this.graphScheduleDay.EndHour = 19;
			this.graphScheduleDay.ExteriorPaddingPixels = 11;
			this.graphScheduleDay.GraphBackColor = System.Drawing.Color.White;
			this.graphScheduleDay.LineWidthPixels = 1;
			this.graphScheduleDay.Location = new System.Drawing.Point(3, 3);
			this.graphScheduleDay.Name = "graphScheduleDay";
			this.graphScheduleDay.PracticeBarColor = System.Drawing.Color.Salmon;
			this.graphScheduleDay.PracticeTextColor = System.Drawing.Color.Black;
			this.graphScheduleDay.ProviderBarColor = System.Drawing.Color.LightGreen;
			this.graphScheduleDay.ProviderTextColor = System.Drawing.Color.Black;
			this.graphScheduleDay.Size = new System.Drawing.Size(787, 603);
			this.graphScheduleDay.StartHour = 4;
			this.graphScheduleDay.TabIndex = 0;
			this.graphScheduleDay.TickHeightPixels = 5;
			this.graphScheduleDay.XAxisBackColor = System.Drawing.Color.White;
			// 
			// groupPractice
			// 
			this.groupPractice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.groupPractice.Controls.Add(this.butHoliday);
			this.groupPractice.Controls.Add(this.butNote);
			this.groupPractice.Location = new System.Drawing.Point(854, 551);
			this.groupPractice.Name = "groupPractice";
			this.groupPractice.Size = new System.Drawing.Size(110, 89);
			this.groupPractice.TabIndex = 15;
			this.groupPractice.TabStop = false;
			this.groupPractice.Text = "Practice";
			// 
			// butHoliday
			// 
			this.butHoliday.Location = new System.Drawing.Point(14, 53);
			this.butHoliday.Name = "butHoliday";
			this.butHoliday.Size = new System.Drawing.Size(80, 24);
			this.butHoliday.TabIndex = 15;
			this.butHoliday.Text = "Holiday";
			this.butHoliday.Click += new System.EventHandler(this.butHoliday_Click);
			// 
			// butNote
			// 
			this.butNote.Location = new System.Drawing.Point(14, 20);
			this.butNote.Name = "butNote";
			this.butNote.Size = new System.Drawing.Size(80, 24);
			this.butNote.TabIndex = 14;
			this.butNote.Text = "Note";
			this.butNote.Click += new System.EventHandler(this.butNote_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(12, 670);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(450, 44);
			this.label2.TabIndex = 14;
			this.label2.Text = resources.GetString("label2.Text");
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(819, 680);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.tabControl2);
			this.groupBox3.Controls.Add(this.butProvNote);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.butAddTime);
			this.groupBox3.Location = new System.Drawing.Point(819, 36);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(179, 472);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Add Time Block";
			// 
			// tabControl2
			// 
			this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.tabControl2.Controls.Add(this.tabPageProv);
			this.tabControl2.Controls.Add(this.tabPageEmp);
			this.tabControl2.Location = new System.Drawing.Point(5, 45);
			this.tabControl2.Name = "tabControl2";
			this.tabControl2.SelectedIndex = 0;
			this.tabControl2.Size = new System.Drawing.Size(168, 391);
			this.tabControl2.TabIndex = 16;
			// 
			// tabPageProv
			// 
			this.tabPageProv.BackColor = System.Drawing.SystemColors.Control;
			this.tabPageProv.Controls.Add(this.label3);
			this.tabPageProv.Controls.Add(this.listProv);
			this.tabPageProv.Controls.Add(this.comboProv);
			this.tabPageProv.Location = new System.Drawing.Point(4, 22);
			this.tabPageProv.Name = "tabPageProv";
			this.tabPageProv.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageProv.Size = new System.Drawing.Size(160, 365);
			this.tabPageProv.TabIndex = 0;
			this.tabPageProv.Text = "Providers (0)";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(3, 319);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(147, 21);
			this.label3.TabIndex = 17;
			this.label3.Text = "Default Prov for Unassigned*";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listProv
			// 
			this.listProv.Location = new System.Drawing.Point(0, 0);
			this.listProv.Name = "listProv";
			this.listProv.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listProv.Size = new System.Drawing.Size(160, 316);
			this.listProv.TabIndex = 6;
			this.listProv.SelectedIndexChanged += new System.EventHandler(this.listProv_SelectedIndexChanged);
			// 
			// comboProv
			// 
			this.comboProv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.comboProv.Location = new System.Drawing.Point(3, 342);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(155, 21);
			this.comboProv.TabIndex = 16;
			// 
			// tabPageEmp
			// 
			this.tabPageEmp.Controls.Add(this.listEmp);
			this.tabPageEmp.Location = new System.Drawing.Point(4, 22);
			this.tabPageEmp.Name = "tabPageEmp";
			this.tabPageEmp.Size = new System.Drawing.Size(160, 365);
			this.tabPageEmp.TabIndex = 1;
			this.tabPageEmp.Text = "Employees (0)";
			this.tabPageEmp.UseVisualStyleBackColor = true;
			// 
			// listEmp
			// 
			this.listEmp.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listEmp.Location = new System.Drawing.Point(0, 0);
			this.listEmp.Name = "listEmp";
			this.listEmp.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listEmp.Size = new System.Drawing.Size(160, 365);
			this.listEmp.TabIndex = 6;
			this.listEmp.SelectedIndexChanged += new System.EventHandler(this.listEmp_SelectedIndexChanged);
			// 
			// butProvNote
			// 
			this.butProvNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butProvNote.Location = new System.Drawing.Point(93, 442);
			this.butProvNote.Name = "butProvNote";
			this.butProvNote.Size = new System.Drawing.Size(80, 24);
			this.butProvNote.TabIndex = 15;
			this.butProvNote.Text = "Note";
			this.butProvNote.Click += new System.EventHandler(this.butProvNote_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(2, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(170, 30);
			this.label1.TabIndex = 7;
			this.label1.Text = "Select One or More Providers or Employees";
			// 
			// butAddTime
			// 
			this.butAddTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddTime.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddTime.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddTime.Location = new System.Drawing.Point(9, 442);
			this.butAddTime.Name = "butAddTime";
			this.butAddTime.Size = new System.Drawing.Size(80, 24);
			this.butAddTime.TabIndex = 4;
			this.butAddTime.Text = "&Add";
			this.butAddTime.Click += new System.EventHandler(this.butAddTime_Click);
			// 
			// labelDate
			// 
			this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDate.Location = new System.Drawing.Point(57, 2);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(136, 29);
			this.labelDate.TabIndex = 9;
			this.labelDate.Text = "labelDate";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butCloseOffice
			// 
			this.butCloseOffice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCloseOffice.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butCloseOffice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCloseOffice.Location = new System.Drawing.Point(866, 521);
			this.butCloseOffice.Name = "butCloseOffice";
			this.butCloseOffice.Size = new System.Drawing.Size(80, 24);
			this.butCloseOffice.TabIndex = 5;
			this.butCloseOffice.Text = "Delete";
			this.butCloseOffice.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(906, 680);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOkSchedule
			// 
			this.butOkSchedule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOkSchedule.Location = new System.Drawing.Point(636, 680);
			this.butOkSchedule.Name = "butOkSchedule";
			this.butOkSchedule.Size = new System.Drawing.Size(119, 24);
			this.butOkSchedule.TabIndex = 40;
			this.butOkSchedule.Text = "OK + Goto Schedules";
			this.butOkSchedule.Click += new System.EventHandler(this.butOkSchedule_Click);
			// 
			// butViewGraph
			// 
			this.butViewGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butViewGraph.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butViewGraph.Location = new System.Drawing.Point(539, 680);
			this.butViewGraph.Name = "butViewGraph";
			this.butViewGraph.Size = new System.Drawing.Size(91, 24);
			this.butViewGraph.TabIndex = 41;
			this.butViewGraph.Text = "View Graph";
			this.butViewGraph.Visible = false;
			this.butViewGraph.Click += new System.EventHandler(this.butViewGraph_Click);
			// 
			// textSearch
			// 
			this.textSearch.Location = new System.Drawing.Point(564, 10);
			this.textSearch.Name = "textSearch";
			this.textSearch.Size = new System.Drawing.Size(181, 20);
			this.textSearch.TabIndex = 42;
			// 
			// labelSearch
			// 
			this.labelSearch.Location = new System.Drawing.Point(508, 12);
			this.labelSearch.Name = "labelSearch";
			this.labelSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelSearch.Size = new System.Drawing.Size(55, 13);
			this.labelSearch.TabIndex = 43;
			this.labelSearch.Text = "Search";
			this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormScheduleDayEdit
			// 
			this.ClientSize = new System.Drawing.Size(1003, 720);
			this.Controls.Add(this.labelSearch);
			this.Controls.Add(this.textSearch);
			this.Controls.Add(this.butViewGraph);
			this.Controls.Add(this.butOkSchedule);
			this.Controls.Add(this.butForward);
			this.Controls.Add(this.butBack);
			this.Controls.Add(this.comboClinic);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.groupPractice);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.butCloseOffice);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormScheduleDayEdit";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Edit Day";
			this.Load += new System.EventHandler(this.FormScheduleDay_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupPractice.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.tabControl2.ResumeLayout(false);
			this.tabPageProv.ResumeLayout(false);
			this.tabPageEmp.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormScheduleDay_Load(object sender,System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrid(),textSearch);
			butViewGraph.Visible=PrefC.IsODHQ;
			butOkSchedule.Visible=ShowOkSchedule;
			_listClinics=new List<Clinic>();
			_listClinics.Add(new Clinic() { Abbr="" });//so HQ always comes before other clinics in the sort order; only for notes and holidays
			Clinics.GetForUserod(Security.CurUser).ForEach(x => _listClinics.Add(x));
			//filled here instead of FillGrid since the list of clinics doesn't change when the grid is filtered and refilled.
			_dictClinicNumClinic=_listClinics.ToDictionary(x => x.ClinicNum);//speed up sorting of schedules.
			comboClinic.SelectedClinicNum=_clinicNumInitial;
			comboClinicChanged();//fills provs and emps and also fills grid
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(PrefC.GetLong(PrefName.ScheduleProvUnassigned));
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			if(_isFromSchedule && (_provAbbrFilter!="" || _employeeNameFilter!="")) {//single person was passed in from the schedule window.
				if(_provAbbrFilter!="") {//it was a provider schedule
					textSearch.Text=_provAbbrFilter;
				}
				else {//it was an employee schedule
					textSearch.Text=_employeeNameFilter;
				}
			}
			FillGrid(); //Fill grid with text from search bar automatically
			textSearch.Select();//focus and select the filter so it can be easily changed
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			comboClinicChanged();
		}

		private void comboClinicChanged(){
			long clinicNumOrig=comboClinic.SelectedClinicNum;
			this.Text=Lan.g(this,"Edit Day")+" - "+comboClinic.Text;
			if(comboClinic.SelectedClinicNum==0) {
				groupPractice.Text=Lan.g(this,"Practice");
			}
			else {
				groupPractice.Text=Lan.g(this,"Clinic");
			}
			if(_listScheds!=null) {
				//skips on startup
				//Sync changes to Db because we are going to change our in memory lists.
				try {
					Schedules.SetForDay(_listScheds,_listSchedsOld);
				}
				catch(Exception ex) {
					MsgBox.Show(this,ex.Message);
					return;
				}
			}
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listScheds=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProvs.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmps.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedsOld=_listScheds.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void FillProvsAndEmps() {
			tabPageProv.Text=Lan.g(this,"Providers")+" (0)";
			tabPageEmp.Text=Lan.g(this,"Employees")+" (0)";
			//Seed emp list and prov list with a dummy emp/prov with 'none' for the field that fills the list, FName and Abbr respectively.
			//That way we don't have to add/subtract one in order when selecting from the list based on selected indexes.
			_listEmps=new List<Employee>() { new Employee() { EmployeeNum=0,FName="none" } };
			_listProvs=new List<Provider>() { new Provider() { ProvNum=0,Abbr="none" } };
			if(PrefC.HasClinicsEnabled) {
				_listProvs.AddRange(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
				_listEmps.AddRange(Employees.GetEmpsForClinic(comboClinic.SelectedClinicNum));
			}
			else {
				_listProvs.AddRange(Providers.GetDeepCopy(true));
				_listEmps.AddRange(Employees.GetDeepCopy(true));
			}
			//Prov Listbox
			List<long> listPreviouslySelectedProvNums=listProv.GetListSelected<Provider>().Select(x => x.ProvNum).ToList();
			listProv.Items.Clear();
			listProv.Items.AddList(_listProvs,x => x.Abbr);
			for(int i=0; i<_listProvs.Count;i++) {
				if(listPreviouslySelectedProvNums.Contains(_listProvs[i].ProvNum)) {
					listProv.SetSelected(i,true);
				}
			}
			if(listProv.SelectedIndices.Count==0) {
				listProv.SelectedIndex=0;//select the 'none' entry
			}
			//Emp Listbox
			List<long> listPreviouslySelectedEmpNums=listEmp.GetListSelected<Employee>().Select(x => x.EmployeeNum).ToList();
			listEmp.Items.Clear();
			listEmp.Items.AddList(_listEmps,x => x.FName);
			for(int i=0; i<_listEmps.Count;i++) {
				if(listPreviouslySelectedEmpNums.Contains(_listEmps[i].EmployeeNum)) {
					listEmp.SetSelected(i,true);
				}
			}
			if(listEmp.SelectedIndices.Count==0) {
				listEmp.SelectedIndex=0;//select the 'none' entry
			}
		}

		private void listProv_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageProv.Text=Lan.g(this,"Providers")+" ("+listProv.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		private void listEmp_SelectedIndexChanged(object sender,EventArgs e) {
			tabPageEmp.Text=Lan.g(this,"Employees")+" ("+listEmp.SelectedIndices.OfType<int>().Count(x => x>0)+")";
		}

		private void FillGrid() {
			//do not refresh from db
			_dictEmpNumEmployee=_listScheds.Select(x => x.EmployeeNum).Distinct().Select(x => Employees.GetEmp(x))//returns null if EmployeeNum==0 or invalid
				.Where(x => x!=null).ToDictionary(x => x.EmployeeNum);//speed up sort.
			_dictProvNumProvider=_listScheds.Select(x => x.ProvNum).Distinct().Select(x => Providers.GetProv(x))//returns null if ProvNum==0 or invalid
				.Where(x => x!=null).ToDictionary(x => x.ProvNum);//speed up sort.
			if(PrefC.IsODHQ) {
				//HQ wants their own sort, so instead of complicating the comparer we will just do the comparer on four seperate lists.
				List<Schedule> listPracticeNotes=_listScheds.Where(x => x.EmployeeNum==0 && x.ProvNum==0).ToList();
				listPracticeNotes.Sort(CompareSchedule);
				List<Schedule> listEmpNotes=_listScheds.Where(x => x.EmployeeNum!=0 && x.ProvNum==0 && x.StartTime==TimeSpan.Zero).ToList();
				listEmpNotes.Sort(CompareSchedule);
				List<Schedule> listProvSched=_listScheds.Where(x => x.EmployeeNum==0 && x.ProvNum!=0).ToList();
				listProvSched.Sort(CompareSchedule);
				List<Schedule> listEmpSched=_listScheds.Where(x => x.EmployeeNum!=0 && x.ProvNum==0 && x.StartTime!=TimeSpan.Zero).ToList();
				listEmpSched.Sort(CompareSchedule);
				_listScheds=new List<Schedule>();
				_listScheds.AddRange(listPracticeNotes);
				_listScheds.AddRange(listEmpNotes);
				_listScheds.AddRange(listProvSched);
				_listScheds.AddRange(listEmpSched);
				_listScheds.Distinct();
			}
			else {
				_listScheds.Sort(CompareSchedule);
			}
			graphScheduleDay.SetSchedules(_listScheds);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableSchedDay","Provider"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Employee"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Start Time"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Stop Time"),80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Ops"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableSchedDay","Note"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			string note;
			string opdesc;
			foreach(Schedule schedCur in _listScheds) {
				row=new GridRow();
				//Prov
				if(schedCur.ProvNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Providers.GetAbbr(schedCur.ProvNum));
				}
				//Employee
				if(schedCur.EmployeeNum==0) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(Employees.GetEmp(schedCur.EmployeeNum).FName);
				}
				//times
				if(schedCur.StartTime==TimeSpan.Zero && schedCur.StopTime==TimeSpan.Zero) {
					row.Cells.Add("");
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(schedCur.StartTime.ToShortTimeString());
					row.Cells.Add(schedCur.StopTime.ToShortTimeString());
				}
				//ops
				opdesc="";
				foreach(long opNumCur in schedCur.Ops) {
					Operatory opCur=Operatories.GetOperatory(opNumCur);
					if(opCur==null || opCur.IsHidden) {//Skip hidden operatories because it just confuses users.
						continue;
					}
					if(opdesc!="") {
						opdesc+=",";
					}
					opdesc+=opCur.Abbrev;
				}
				row.Cells.Add(opdesc);
				//note
				note="";
				if(schedCur.SchedType==ScheduleType.Practice) {//note or holiday
					string clinicAbbr="";
					if(PrefC.HasClinicsEnabled) {
						clinicAbbr=Clinics.GetAbbr(schedCur.ClinicNum);
						if(string.IsNullOrEmpty(clinicAbbr)) {
							clinicAbbr="Headquarters";
						}
						clinicAbbr=" ("+clinicAbbr+")";
						if(schedCur.Status!=SchedStatus.Holiday) {//must be a Note, only add 'Note' if clinics are enabled
							note=Lan.g(this,"Note")+clinicAbbr+": ";
						}
					}
					if(schedCur.Status==SchedStatus.Holiday) {
						note=Lan.g(this,"Holiday")+clinicAbbr+": ";
					}
				}
				note+=schedCur.Note;
				row.Cells.Add(note);
				row.Tag=schedCur;
				//Do not add the row if the user typed something into the search box and no cell contains the text that was typed in.
				string searchTextLower=textSearch.Text.ToLower().Trim();
				if(!string.IsNullOrEmpty(searchTextLower)) {
					//Go through every cell in the row and make sure that there is at least one cell that contains the search text.
					bool hasMatch=false;
					foreach(GridCell cell in row.Cells) {
						if(cell.Text.ToLower().Trim().Contains(searchTextLower)) {
							hasMatch=true; 
							break;
						}
					}
					if(!hasMatch) {
						continue;//Do not add this row to gridMain.
					}
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private int CompareSchedule(Schedule x,Schedule y){
			if(x==y){//also handles null==null
				return 0;
			}
			if(y==null){
				return 1;
			}
			if(x==null){
				return -1;
			}
			if(x.SchedType!=y.SchedType){
				return x.SchedType.CompareTo(y.SchedType);
			}
			if(x.ProvNum!=y.ProvNum){
				return _dictProvNumProvider[x.ProvNum].ItemOrder.CompareTo(_dictProvNumProvider[y.ProvNum].ItemOrder);
			}
			if(x.EmployeeNum!=y.EmployeeNum) {
				Employee empx= _dictEmpNumEmployee[x.EmployeeNum];//use dictionary to greatly speed up sort
				Employee empy= _dictEmpNumEmployee[y.EmployeeNum];//use dictionary to greatly speed up sort
				if(empx.FName!=empy.FName) {
					return empx.FName.CompareTo(empy.FName);
				}
				if(empx.LName!=empy.LName) {
					return empx.LName.CompareTo(empy.LName);
				}
				return x.EmployeeNum.CompareTo(y.EmployeeNum);
			}
			if(x.StartTime!=y.StartTime) {
				return x.StartTime.CompareTo(y.StartTime);
			}
			if(x.ClinicNum!=y.ClinicNum
				&& _dictClinicNumClinic.ContainsKey(x.ClinicNum) 
				&& _dictClinicNumClinic.ContainsKey(y.ClinicNum)) 
			{//if clinics not enabled, both schedules should have ClinicNum 0 or one schedule's ClinicNum will not be present in the dictionary
				//and this comparison will be skipped.
				return _dictClinicNumClinic[x.ClinicNum].Abbr.CompareTo(_dictClinicNumClinic[y.ClinicNum].Abbr);
			}
			if(!x.Status.Equals(y.Status)) {
				return -x.Status.CompareTo(y.Status);//holiday to the top
			}
			if(x.Note!=y.Note) {
				return x.Note.CompareTo(y.Note);
			}
			if(x.ScheduleNum!=y.ScheduleNum) {
				return x.ScheduleNum.CompareTo(y.ScheduleNum);
			}
			return x.GetHashCode().CompareTo(y.GetHashCode()); //to ensure deterministric sorting, even when PK==0
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Schedule selectedSchedule=(Schedule)gridMain.ListGridRows[e.Row].Tag;
			if(selectedSchedule==null) {
				return;//nothing to do
			}
			using FormScheduleEdit FormS=new FormScheduleEdit();
			//Remove clicked on date so that it does not cause itself to be blocked if holiday.
			FormS.ListScheds=_listScheds.FindAll(x => x!=selectedSchedule)
				.Select(x => x.Copy()).ToList();//Deep copy for safety
			FormS.SchedCur=selectedSchedule;
			FormS.SchedCur.IsNew=false;
			FormS.ClinicNum=comboClinic.SelectedClinicNum;
			FormS.ListProvNums=new List<long>();
			if(selectedSchedule.ProvNum>0) {//Don't look for conflicts against a schedule with no provider.
				FormS.ListProvNums.Add(selectedSchedule.ProvNum);
			}
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			//Sync changes because the user may have changed the clinic of a schedule in form
			try {
				Schedules.SetForDay(_listScheds,_listSchedsOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_listScheds=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProvs.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmps.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedsOld=_listScheds.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		///<summary>Returns true if date text boxes have no errors and the emp and prov lists don't have 'none' selected with other emps/provs.
		///Set isQuiet to true to suppress the message box with the warning.</summary>
		private bool ValidateLists() {
			List<string> listErrorMsgs=new List<string>();
			if(listProv.SelectedIndices.Count>1 && listProv.SelectedIndices.Contains(0)) {//'none' selected with additional provs
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of providers."));
			}
			if(listEmp.SelectedIndices.Count>1 && listEmp.SelectedIndices.Contains(0)) {//'none' selected with additional emps
				listErrorMsgs.Add(Lan.g(this,"Invalid selection of employees."));
			}
			if(listProv.SelectedIndices.OfType<int>().Count(x => x>0)==0 && listEmp.SelectedIndices.OfType<int>().Count(x => x>0)==0) {
				listErrorMsgs.Add(Lan.g(this,"Please select at least one provider or one employee first."));
			}
			if(listErrorMsgs.Count>0) {
				MessageBox.Show(string.Join("\r\n",listErrorMsgs));
			}
			if(listErrorMsgs.Count==0 //only perform this check if everything else is okay.
				&& listProv.SelectedIndices.Count>0 && !listProv.SelectedIndices.Contains(0) //at least one valid provider selected
				&& listEmp.SelectedIndices.Count>0 && !listEmp.SelectedIndices.Contains(0) //at least one valid employee selected
				&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"Both providers and employees are selected, would you like to continue?"))
			{
				return false;
			}
			return listErrorMsgs.Count==0;
		}

		private void butAddTime_Click(object sender, System.EventArgs e) {
			if(!ValidateLists()) {
				return;
			}
			_listSelectedProvNums=new List<long>();//list of provNums selected
			if(!listProv.SelectedIndices.Contains(0)) {//add selected operatories into listSelectedOps
				listProv.SelectedIndices.OfType<int>().ToList().ForEach(x => _listSelectedProvNums.Add(_listProvs[x].ProvNum));
			}
			Schedule schedCur=new Schedule();
			schedCur.SchedDate=_dateSched;
			schedCur.Status=SchedStatus.Open;
			schedCur.StartTime=new TimeSpan(8,0,0);//8am
			schedCur.StopTime=new TimeSpan(17,0,0);//5pm
			//schedtype, provNum, and empnum will be set down below
			using FormScheduleEdit FormS=new FormScheduleEdit();
			FormS.SchedCur=schedCur;
			FormS.ListScheds=_listScheds;
			FormS.ListProvNums=_listSelectedProvNums;
			FormS.SchedCur.IsNew=true;
			FormS.ClinicNum=comboClinic.SelectedClinicNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK){
				return;
			}
			Schedule schedTemp;
			for(int i=0;i<listProv.SelectedIndices.Count;i++){
				if(listProv.SelectedIndices[i]==0) {
					continue;
				}
				schedTemp=new Schedule();
				schedTemp=schedCur.Copy();
				schedTemp.SchedType=ScheduleType.Provider;
				schedTemp.ProvNum=_listProvs[listProv.SelectedIndices[i]].ProvNum;
				_listScheds.Add(schedTemp);
			}
			for(int i=0;i<listEmp.SelectedIndices.Count;i++) {
				if(listEmp.SelectedIndices[i]==0) {
					continue;
				}
				schedTemp=new Schedule();
				schedTemp=schedCur.Copy();
				schedTemp.SchedType=ScheduleType.Employee;
				schedTemp.EmployeeNum=_listEmps[listEmp.SelectedIndices[i]].EmployeeNum;
				_listScheds.Add(schedTemp);
			}
			FillGrid();
		}

		private void butProvNote_Click(object sender,EventArgs e) {
			if(!ValidateLists()) {
				return;
			}
			Schedule schedCur=new Schedule();
			schedCur.SchedDate=_dateSched;
			schedCur.Status=SchedStatus.Open;
			//this is so we can differentiate between a practice note and a prov/emp note in FormScheduleEdit.  Updated if necessary below when inserting.
			schedCur.SchedType=ScheduleType.Provider;
			//schedtype, provNum, and empnum will be set down below
			using FormScheduleEdit FormS=new FormScheduleEdit();
			FormS.SchedCur=schedCur;
			FormS.ClinicNum=comboClinic.SelectedClinicNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			Schedule schedTemp;
			foreach(int listProvIndex in listProv.SelectedIndices.OfType<int>().Where(x => x>0)) {//validation of selected indices happens above
				schedTemp=new Schedule();
				schedTemp=schedCur.Copy();
				schedTemp.SchedType=ScheduleType.Provider;
				schedTemp.ProvNum=_listProvs[listProvIndex].ProvNum;
				_listScheds.Add(schedTemp);
			}
			foreach(int listEmpIndex in listEmp.SelectedIndices.OfType<int>().Where(x => x>0)) {//validation of selected indices happens above
				schedTemp=new Schedule();
				schedTemp=schedCur.Copy();
				schedTemp.SchedType=ScheduleType.Employee;
				schedTemp.EmployeeNum=_listEmps[listEmpIndex].EmployeeNum;
				_listScheds.Add(schedTemp);
			}
			FillGrid();
		}

		private void butNote_Click(object sender,EventArgs e) {
			Schedule SchedCur=new Schedule();
			SchedCur.SchedDate=_dateSched;
			SchedCur.Status=SchedStatus.Open;
			SchedCur.SchedType=ScheduleType.Practice;
			SchedCur.ClinicNum=comboClinic.SelectedClinicNum;
			using FormScheduleEdit FormS=new FormScheduleEdit();
			FormS.SchedCur=SchedCur;
			FormS.ClinicNum=comboClinic.SelectedClinicNum;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			_listScheds.Add(SchedCur);
			FillGrid();
		}

		private void butHoliday_Click(object sender,System.EventArgs e) {
		  Schedule SchedCur=new Schedule();
      SchedCur.SchedDate=_dateSched;
      SchedCur.Status=SchedStatus.Holiday;
			SchedCur.SchedType=ScheduleType.Practice;
			SchedCur.ClinicNum=comboClinic.SelectedClinicNum;
		  using FormScheduleEdit FormS=new FormScheduleEdit();
			FormS.ListScheds=_listScheds;
			FormS.SchedCur=SchedCur;
			FormS.ClinicNum=comboClinic.SelectedClinicNum;
      FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			_listScheds.Add(SchedCur);
      FillGrid();
		}

		private void butBack_Click(object sender,EventArgs e) {
			//Sync changes because we are going to change our in memory lists.
			try {
				Schedules.SetForDay(_listScheds,_listSchedsOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_dateSched=_dateSched.AddDays(-1);
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listScheds=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProvs.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmps.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedsOld=_listScheds.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void butForward_Click(object sender,EventArgs e) {
			//Sync changes because we are going to change our in memory lists.
			try {
				Schedules.SetForDay(_listScheds,_listSchedsOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			_dateSched=_dateSched.AddDays(1);
			labelDate.Text=_dateSched.ToString("dddd")+"\r\n"+_dateSched.ToShortDateString();
			//Fill lists with new information from new clinic
			FillProvsAndEmps();
			_listScheds=Schedules.RefreshDayEditForPracticeProvsEmps(_dateSched,_listProvs.Select(x => x.ProvNum).Where(x => x>0).ToList(),
				_listEmps.Select(x => x.EmployeeNum).Where(x => x>0).ToList(),comboClinic.SelectedClinicNum);
			_listSchedsOld=_listScheds.Select(x => x.Copy()).ToList();
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			int countProviders;
			//Select all rows if none are currently selected.
			if(gridMain.SelectedIndices.Length==0) {
				gridMain.SetAll(true);
			}
			List<Schedule> listSchedsToRemove=gridMain.SelectedTags<Schedule>();
			if(listSchedsToRemove.IsNullOrEmpty()) {
				return;//Nothing to do.
			}
			countProviders=listSchedsToRemove.Where(x => x.ProvNum!=0).Select(y => y.ProvNum).Distinct().Count();
			if(countProviders>0) {
				string message=Lan.g(this,"Delete schedules on this day for")+" "+countProviders+" "+Lan.g(this,"provider(s)?");
				if(MessageBox.Show(message,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
			}
			_listScheds.RemoveAll(x => listSchedsToRemove.Contains(x));
			FillGrid();
		}

		private void butOkSchedule_Click(object sender,EventArgs e) {
			GotoScheduleOnClose=true;
			butOK_Click(sender,e);
		}

		private void butViewGraph_Click(object sender,EventArgs e) {
			using FormGraphEmployeeTime formGraphEmployeeTime=new FormGraphEmployeeTime(_dateSched);
			formGraphEmployeeTime.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			try {
				Schedules.SetForDay(_listScheds,_listSchedsOld);
			}
			catch(Exception ex) {
				MsgBox.Show(this,ex.Message);
				return;
			}
			if(comboProv.SelectedIndex!=-1
				&& Prefs.UpdateLong(PrefName.ScheduleProvUnassigned,comboProv.GetSelectedProvNum()))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void tabControl1_SelectedIndexChanged(object sender, EventArgs e){
			LayoutManager.LayoutControlBoundsAndFonts(tabControl1);
		}
	}
}







