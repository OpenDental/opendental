using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Linq;
using OpenDental.UI;
using OpenDentBusiness.HL7;
using SparksToothChart;
using OpenDentBusiness;
using CodeBase;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using Document=OpenDentBusiness.Document;
using OpenDentBusiness.WebTypes;
using System.Text.RegularExpressions;

namespace OpenDental{
///<summary></summary>
	public class ControlTreat : System.Windows.Forms.UserControl{
		//private AxFPSpread.AxvaSpread axvaSpread2;
		private System.Windows.Forms.Label label1;
		private System.ComponentModel.IContainer components;// Required designer variable.
		private OpenDental.UI.ListBoxOD listSetPr;
		//private int linesPrinted=0;
		///<summary></summary>
    public FormRpPrintPreview pView;
//		private System.Windows.Forms.PrintDialog printDialog2;
		//private bool headingPrinted;
		//private bool graphicsPrinted;
		//private bool mainPrinted;
		//private bool benefitsPrinted;
		//private bool notePrinted;
		//private double[] ColTotal;
		private System.Drawing.Font bodyFont=new System.Drawing.Font("Arial",9);
		private System.Drawing.Font nameFont=new System.Drawing.Font("Arial",9,FontStyle.Bold);
		//private Font headingFont=new Font("Arial",13,FontStyle.Bold);
		private System.Drawing.Font subHeadingFont=new System.Drawing.Font("Arial",10,FontStyle.Bold);
		private System.Drawing.Printing.PrintDocument pd2;
		private System.Drawing.Font totalFont=new System.Drawing.Font("Arial",9,FontStyle.Bold);
		private OpenDental.UI.ToolBarOD ToolBarMain;
    private ArrayList ALPreAuth;
		///<summary>This is a list of all procedures for the patient.</summary>
		private List<Procedure> _listProcs;
		///<summary>This is a filtered list containing only TP procedures.  It's also already sorted by priority and tooth number.</summary>
		private Procedure[] ProcListTP;
		///<summary>List of ClaimProcs with status of Estimate or CapEstimate for the patient.</summary>
		private List<ClaimProc> ClaimProcList;
		private Family FamCur;
		public Patient PatCur;
		private System.Windows.Forms.ContextMenu menuConsent;
		private OpenDental.UI.GridOD gridMain;
		private OpenDental.UI.GridOD gridPrint;
		private OpenDental.UI.GridOD gridPreAuth;
		private List<InsPlan> InsPlanList;
		private List<SubstitutionLink> _listSubstLinks=null;
		private List<InsSub> SubList;
		private DiscountPlanSub _discountPlanSub;
		private OpenDental.UI.GridOD gridPlans;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		private List<TreatPlan> _listTreatPlans;
		//private List<TreatPlan> _listTPCurrent;
		///<summary>A list of all ProcTP objects for this patient.</summary>
		private ProcTP[] ProcTPList;
		private ODtextBox textNote;
		private System.Windows.Forms.ImageList imageListMain;
		///<summary>A list of all ProcTP objects for the selected tp.</summary>
		private ProcTP[] ProcTPSelectList;
		private List <PatPlan> PatPlanList;
		private List <Benefit> BenefitList;
		private List<Procedure> ProcListFiltered;
		///<summary>Only used for printing graphical chart.</summary>
		private List<ToothInitial> ToothInitialList;
		///<summary>Only used for printing graphical chart prior to v17.  Null unless prepping for printing.</summary>
		private ToothChartWrapper toothChartWrapper;
		///<summary>Only used for printing graphical chart prior to v17.  This is the new Sparks3D toothChart.</summary>
		private Control toothChart;
		///<summary>Only used for printing graphical chart prior to v17.  Relays commands to either the old SparksToothChart.ToothChartWrapper or the new Sparks3d.ToothChart.</summary>
		private ToothChartRelay _toothChartRelay;
		///<summary>Only used for printing graphical chart.</summary>
		private Bitmap _bitmapToothChart;
		private List<Claim> ClaimList;
		private bool InitializedOnStartup;
		private List<ClaimProcHist> HistList;
		private List<ClaimProcHist> LoopList;
		private bool checkShowInsNotAutomatic;
		private bool checkShowDiscountNotAutomatic;
		private List<TpRow> RowsMain;
		private UI.Button butInsRem;
		private UI.Button butNewTP;
		private UI.Button butSaveTP;
		///<summary>Gets updated to PatCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		private DateTimePicker dateTimeTP;
		private UI.Button butRefresh;
		private TabControl tabShowSort;
		private TabPage tabPageShow;
		private CheckBox checkShowDiscount;
		private CheckBox checkShowTotals;
		private CheckBox checkShowMaxDed;
		private CheckBox checkShowSubtotals;
		private CheckBox checkShowFees;
		private CheckBox checkShowIns;
		private CheckBox checkShowCompleted;
		private TabPage tabPageSort;
		private Label label6;
		private RadioButton radioTreatPlanSortTooth;
		private RadioButton radioTreatPlanSortOrder;
		private Label labelCheckInsFrequency;
		private TabPage tabPagePrint;
		private CheckBox checkPrintClassic;
		private UI.Button butPlannedAppt;
		private DashFamilyInsurance userControlFamIns;
		private DashIndividualDiscount userControlIndDis;
		private DashIndividualInsurance userControlIndIns;
		private TPModuleData _tpData;
		///<summary>Tracks the most recently selected TreatPlan in gridPlans.</summary>
		private TreatPlan _selectedTreatPlan;
		private UI.Button butSendToDevice;
		private bool _hasCheckShowCompletedChangedByUser;
		///<summary>Set to true when TP Note changes.  Public so this can be checked from FormOpenDental and the note can be saved.  Necessary because in
		///some cases the leave event doesn't fire, like when a non-modal form is selected, like big phones, and the selected patient is changed from that form.</summary>
		public bool HasNoteChanged=false;
		///<summary>The data needed to load the active treatment plan. Also used for inactive treatment plans.</summary>
		private LoadActiveTPData _loadActiveData;

		///<summary></summary>
		public ControlTreat(){
			Logger.openlog.Log("Initializing treatment module...",Logger.Severity.INFO);
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
		}

		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlTreat));
			this.label1 = new System.Windows.Forms.Label();
			this.listSetPr = new OpenDental.UI.ListBoxOD();
			this.pd2 = new System.Drawing.Printing.PrintDocument();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.gridMain = new OpenDental.UI.GridOD();
			this.gridPrint = new OpenDental.UI.GridOD();
			this.gridPreAuth = new OpenDental.UI.GridOD();
			this.gridPlans = new OpenDental.UI.GridOD();
			this.dateTimeTP = new System.Windows.Forms.DateTimePicker();
			this.tabShowSort = new System.Windows.Forms.TabControl();
			this.tabPageShow = new System.Windows.Forms.TabPage();
			this.checkShowDiscount = new System.Windows.Forms.CheckBox();
			this.checkShowTotals = new System.Windows.Forms.CheckBox();
			this.checkShowMaxDed = new System.Windows.Forms.CheckBox();
			this.checkShowSubtotals = new System.Windows.Forms.CheckBox();
			this.checkShowFees = new System.Windows.Forms.CheckBox();
			this.checkShowIns = new System.Windows.Forms.CheckBox();
			this.checkShowCompleted = new System.Windows.Forms.CheckBox();
			this.tabPageSort = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.radioTreatPlanSortTooth = new System.Windows.Forms.RadioButton();
			this.radioTreatPlanSortOrder = new System.Windows.Forms.RadioButton();
			this.tabPagePrint = new System.Windows.Forms.TabPage();
			this.checkPrintClassic = new System.Windows.Forms.CheckBox();
			this.labelCheckInsFrequency = new System.Windows.Forms.Label();
			this.userControlIndIns = new OpenDental.DashIndividualInsurance();
			this.userControlFamIns = new OpenDental.DashFamilyInsurance();
			this.butPlannedAppt = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butSaveTP = new OpenDental.UI.Button();
			this.butNewTP = new OpenDental.UI.Button();
			this.butInsRem = new OpenDental.UI.Button();
			this.ToolBarMain = new OpenDental.UI.ToolBarOD();
			this.textNote = new OpenDental.ODtextBox();
			this.butSendToDevice = new OpenDental.UI.Button();
			this.menuConsent = new System.Windows.Forms.ContextMenu();
			this.userControlIndDis = new OpenDental.DashIndividualDiscount();
			this.tabShowSort.SuspendLayout();
			this.tabPageShow.SuspendLayout();
			this.tabPageSort.SuspendLayout();
			this.tabPagePrint.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(808, 178);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(97, 15);
			this.label1.TabIndex = 4;
			this.label1.Text = "Set Priority";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listSetPr
			// 
			this.listSetPr.Location = new System.Drawing.Point(810, 195);
			this.listSetPr.Name = "listSetPr";
			this.listSetPr.Size = new System.Drawing.Size(92, 199);
			this.listSetPr.TabIndex = 5;
			this.listSetPr.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listSetPr_MouseDown);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "");
			this.imageListMain.Images.SetKeyName(1, "");
			this.imageListMain.Images.SetKeyName(2, "");
			this.imageListMain.Images.SetKeyName(3, "Add.gif");
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(0, 182);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(790, 469);
			this.gridMain.TabIndex = 59;
			this.gridMain.Title = "Procedures";
			this.gridMain.TranslationName = "TableTP";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// gridPrint
			// 
			this.gridPrint.Location = new System.Drawing.Point(0, 0);
			this.gridPrint.Name = "gridPrint";
			this.gridPrint.Size = new System.Drawing.Size(150, 150);
			this.gridPrint.TabIndex = 0;
			this.gridPrint.Title = null;
			this.gridPrint.TranslationName = "TablePrint";
			// 
			// gridPreAuth
			// 
			this.gridPreAuth.Location = new System.Drawing.Point(693, 29);
			this.gridPreAuth.Name = "gridPreAuth";
			this.gridPreAuth.Size = new System.Drawing.Size(299, 146);
			this.gridPreAuth.TabIndex = 62;
			this.gridPreAuth.Title = "Pre Authorizations";
			this.gridPreAuth.TranslationName = "TablePreAuth";
			this.gridPreAuth.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPreAuth_CellDoubleClick);
			this.gridPreAuth.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPreAuth_CellClick);
			// 
			// gridPlans
			// 
			this.gridPlans.Location = new System.Drawing.Point(0, 29);
			this.gridPlans.Name = "gridPlans";
			this.gridPlans.Size = new System.Drawing.Size(426, 151);
			this.gridPlans.TabIndex = 60;
			this.gridPlans.Title = "Treatment Plans";
			this.gridPlans.TranslationName = "TableTPList";
			this.gridPlans.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPlans_CellDoubleClick);
			this.gridPlans.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPlans_CellClick);
			// 
			// dateTimeTP
			// 
			this.dateTimeTP.Format = System.Windows.Forms.DateTimePickerFormat.Short;
			this.dateTimeTP.Location = new System.Drawing.Point(911, 195);
			this.dateTimeTP.Name = "dateTimeTP";
			this.dateTimeTP.Size = new System.Drawing.Size(81, 20);
			this.dateTimeTP.TabIndex = 71;
			this.dateTimeTP.CloseUp += new System.EventHandler(this.dateTimeTP_CloseUp);
			// 
			// tabShowSort
			// 
			this.tabShowSort.Controls.Add(this.tabPageShow);
			this.tabShowSort.Controls.Add(this.tabPageSort);
			this.tabShowSort.Controls.Add(this.tabPagePrint);
			this.tabShowSort.Location = new System.Drawing.Point(525, 29);
			this.tabShowSort.Name = "tabShowSort";
			this.tabShowSort.SelectedIndex = 0;
			this.tabShowSort.Size = new System.Drawing.Size(166, 151);
			this.tabShowSort.TabIndex = 73;
			// 
			// tabPageShow
			// 
			this.tabPageShow.Controls.Add(this.checkShowDiscount);
			this.tabPageShow.Controls.Add(this.checkShowTotals);
			this.tabPageShow.Controls.Add(this.checkShowMaxDed);
			this.tabPageShow.Controls.Add(this.checkShowSubtotals);
			this.tabPageShow.Controls.Add(this.checkShowFees);
			this.tabPageShow.Controls.Add(this.checkShowIns);
			this.tabPageShow.Controls.Add(this.checkShowCompleted);
			this.tabPageShow.Location = new System.Drawing.Point(4, 22);
			this.tabPageShow.Name = "tabPageShow";
			this.tabPageShow.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageShow.Size = new System.Drawing.Size(158, 125);
			this.tabPageShow.TabIndex = 0;
			this.tabPageShow.Text = "Show";
			this.tabPageShow.UseVisualStyleBackColor = true;
			// 
			// checkShowDiscount
			// 
			this.checkShowDiscount.Checked = true;
			this.checkShowDiscount.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowDiscount.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowDiscount.Location = new System.Drawing.Point(23, 71);
			this.checkShowDiscount.Name = "checkShowDiscount";
			this.checkShowDiscount.Size = new System.Drawing.Size(128, 17);
			this.checkShowDiscount.TabIndex = 32;
			this.checkShowDiscount.Text = "Discount";
			this.checkShowDiscount.Click += new System.EventHandler(this.checkShowDiscount_Click);
			// 
			// checkShowTotals
			// 
			this.checkShowTotals.Checked = true;
			this.checkShowTotals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowTotals.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowTotals.Location = new System.Drawing.Point(23, 105);
			this.checkShowTotals.Name = "checkShowTotals";
			this.checkShowTotals.Size = new System.Drawing.Size(128, 15);
			this.checkShowTotals.TabIndex = 31;
			this.checkShowTotals.Text = "Totals";
			this.checkShowTotals.Click += new System.EventHandler(this.checkShowTotals_Click);
			// 
			// checkShowMaxDed
			// 
			this.checkShowMaxDed.Checked = true;
			this.checkShowMaxDed.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowMaxDed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowMaxDed.Location = new System.Drawing.Point(5, 20);
			this.checkShowMaxDed.Name = "checkShowMaxDed";
			this.checkShowMaxDed.Size = new System.Drawing.Size(146, 17);
			this.checkShowMaxDed.TabIndex = 30;
			this.checkShowMaxDed.Text = "Use Ins Max and Deduct";
			this.checkShowMaxDed.Click += new System.EventHandler(this.checkShowMaxDed_Click);
			// 
			// checkShowSubtotals
			// 
			this.checkShowSubtotals.Checked = true;
			this.checkShowSubtotals.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowSubtotals.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowSubtotals.Location = new System.Drawing.Point(23, 88);
			this.checkShowSubtotals.Name = "checkShowSubtotals";
			this.checkShowSubtotals.Size = new System.Drawing.Size(128, 17);
			this.checkShowSubtotals.TabIndex = 29;
			this.checkShowSubtotals.Text = "Subtotals";
			this.checkShowSubtotals.Click += new System.EventHandler(this.checkShowSubtotals_Click);
			// 
			// checkShowFees
			// 
			this.checkShowFees.Checked = true;
			this.checkShowFees.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowFees.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowFees.Location = new System.Drawing.Point(5, 37);
			this.checkShowFees.Name = "checkShowFees";
			this.checkShowFees.Size = new System.Drawing.Size(146, 17);
			this.checkShowFees.TabIndex = 28;
			this.checkShowFees.Text = "Fees";
			this.checkShowFees.Click += new System.EventHandler(this.checkShowFees_Click);
			// 
			// checkShowIns
			// 
			this.checkShowIns.Checked = true;
			this.checkShowIns.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowIns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowIns.Location = new System.Drawing.Point(23, 54);
			this.checkShowIns.Name = "checkShowIns";
			this.checkShowIns.Size = new System.Drawing.Size(128, 17);
			this.checkShowIns.TabIndex = 27;
			this.checkShowIns.Text = "Insurance Estimates";
			this.checkShowIns.Click += new System.EventHandler(this.checkShowIns_Click);
			// 
			// checkShowCompleted
			// 
			this.checkShowCompleted.Checked = true;
			this.checkShowCompleted.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkShowCompleted.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkShowCompleted.Location = new System.Drawing.Point(5, 3);
			this.checkShowCompleted.Name = "checkShowCompleted";
			this.checkShowCompleted.Size = new System.Drawing.Size(146, 17);
			this.checkShowCompleted.TabIndex = 26;
			this.checkShowCompleted.Text = "Graphical Completed Tx";
			this.checkShowCompleted.Click += new System.EventHandler(this.checkShowCompleted_Click);
			// 
			// tabPageSort
			// 
			this.tabPageSort.Controls.Add(this.label6);
			this.tabPageSort.Controls.Add(this.radioTreatPlanSortTooth);
			this.tabPageSort.Controls.Add(this.radioTreatPlanSortOrder);
			this.tabPageSort.Location = new System.Drawing.Point(4, 22);
			this.tabPageSort.Name = "tabPageSort";
			this.tabPageSort.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageSort.Size = new System.Drawing.Size(158, 125);
			this.tabPageSort.TabIndex = 1;
			this.tabPageSort.Text = "Sort by";
			this.tabPageSort.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(3, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(146, 33);
			this.label6.TabIndex = 74;
			this.label6.Text = "Order procedures by priority, then date, then";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioTreatPlanSortTooth
			// 
			this.radioTreatPlanSortTooth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortTooth.Location = new System.Drawing.Point(14, 44);
			this.radioTreatPlanSortTooth.Name = "radioTreatPlanSortTooth";
			this.radioTreatPlanSortTooth.Size = new System.Drawing.Size(116, 15);
			this.radioTreatPlanSortTooth.TabIndex = 73;
			this.radioTreatPlanSortTooth.Text = "Tooth";
			this.radioTreatPlanSortTooth.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortTooth.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioTreatPlanSortTooth_MouseClick);
			// 
			// radioTreatPlanSortOrder
			// 
			this.radioTreatPlanSortOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.radioTreatPlanSortOrder.Checked = true;
			this.radioTreatPlanSortOrder.Location = new System.Drawing.Point(14, 59);
			this.radioTreatPlanSortOrder.Name = "radioTreatPlanSortOrder";
			this.radioTreatPlanSortOrder.Size = new System.Drawing.Size(126, 19);
			this.radioTreatPlanSortOrder.TabIndex = 72;
			this.radioTreatPlanSortOrder.TabStop = true;
			this.radioTreatPlanSortOrder.Text = "Order Entered";
			this.radioTreatPlanSortOrder.UseVisualStyleBackColor = true;
			this.radioTreatPlanSortOrder.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioTreatPlanSortTooth_MouseClick);
			// 
			// tabPagePrint
			// 
			this.tabPagePrint.Controls.Add(this.checkPrintClassic);
			this.tabPagePrint.Location = new System.Drawing.Point(4, 22);
			this.tabPagePrint.Name = "tabPagePrint";
			this.tabPagePrint.Padding = new System.Windows.Forms.Padding(3);
			this.tabPagePrint.Size = new System.Drawing.Size(158, 125);
			this.tabPagePrint.TabIndex = 2;
			this.tabPagePrint.Text = "Printing";
			this.tabPagePrint.UseVisualStyleBackColor = true;
			// 
			// checkPrintClassic
			// 
			this.checkPrintClassic.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPrintClassic.Location = new System.Drawing.Point(7, 12);
			this.checkPrintClassic.Name = "checkPrintClassic";
			this.checkPrintClassic.Size = new System.Drawing.Size(146, 17);
			this.checkPrintClassic.TabIndex = 27;
			this.checkPrintClassic.Text = "Print using classic view";
			// 
			// labelCheckInsFrequency
			// 
			this.labelCheckInsFrequency.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCheckInsFrequency.Location = new System.Drawing.Point(908, 178);
			this.labelCheckInsFrequency.Name = "labelCheckInsFrequency";
			this.labelCheckInsFrequency.Size = new System.Drawing.Size(84, 15);
			this.labelCheckInsFrequency.TabIndex = 74;
			this.labelCheckInsFrequency.Text = "Estimates as of:";
			this.labelCheckInsFrequency.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// userControlIndIns
			// 
			this.userControlIndIns.Location = new System.Drawing.Point(799, 491);
			this.userControlIndIns.Name = "userControlIndIns";
			this.userControlIndIns.Size = new System.Drawing.Size(193, 160);
			this.userControlIndIns.TabIndex = 77;
			// 
			// userControlFamIns
			// 
			this.userControlFamIns.Location = new System.Drawing.Point(799, 411);
			this.userControlFamIns.Name = "userControlFamIns";
			this.userControlFamIns.Size = new System.Drawing.Size(193, 80);
			this.userControlFamIns.TabIndex = 76;
			// 
			// butPlannedAppt
			// 
			this.butPlannedAppt.Icon = OpenDental.UI.EnumIcons.Add;
			this.butPlannedAppt.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPlannedAppt.Location = new System.Drawing.Point(429, 153);
			this.butPlannedAppt.Name = "butPlannedAppt";
			this.butPlannedAppt.Size = new System.Drawing.Size(90, 23);
			this.butPlannedAppt.TabIndex = 75;
			this.butPlannedAppt.Text = "Plan Appt";
			this.butPlannedAppt.Click += new System.EventHandler(this.butPlannedAppt_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.Location = new System.Drawing.Point(429, 122);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(90, 23);
			this.butRefresh.TabIndex = 72;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butSaveTP
			// 
			this.butSaveTP.Image = global::OpenDental.Properties.Resources.Copy;
			this.butSaveTP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSaveTP.Location = new System.Drawing.Point(429, 60);
			this.butSaveTP.Name = "butSaveTP";
			this.butSaveTP.Size = new System.Drawing.Size(90, 23);
			this.butSaveTP.TabIndex = 70;
			this.butSaveTP.Text = "Save TP";
			this.butSaveTP.Click += new System.EventHandler(this.butSaveTP_Click);
			// 
			// butNewTP
			// 
			this.butNewTP.Icon = OpenDental.UI.EnumIcons.Add;
			this.butNewTP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNewTP.Location = new System.Drawing.Point(429, 29);
			this.butNewTP.Name = "butNewTP";
			this.butNewTP.Size = new System.Drawing.Size(90, 23);
			this.butNewTP.TabIndex = 69;
			this.butNewTP.Text = "New TP";
			this.butNewTP.Click += new System.EventHandler(this.butNewTP_Click);
			// 
			// butInsRem
			// 
			this.butInsRem.Location = new System.Drawing.Point(917, 400);
			this.butInsRem.Name = "butInsRem";
			this.butInsRem.Size = new System.Drawing.Size(75, 16);
			this.butInsRem.TabIndex = 68;
			this.butInsRem.Text = "Ins Rem";
			this.butInsRem.Visible = false;
			this.butInsRem.Click += new System.EventHandler(this.butInsRem_Click);
			// 
			// ToolBarMain
			// 
			this.ToolBarMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.ToolBarMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.ToolBarMain.ImageList = this.imageListMain;
			this.ToolBarMain.Location = new System.Drawing.Point(0, 0);
			this.ToolBarMain.Name = "ToolBarMain";
			this.ToolBarMain.Size = new System.Drawing.Size(1195, 25);
			this.ToolBarMain.TabIndex = 58;
			this.ToolBarMain.ButtonClick += new OpenDental.UI.ODToolBarButtonClickEventHandler(this.ToolBarMain_ButtonClick);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNote.BackColor = System.Drawing.SystemColors.Control;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(0, 654);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.TreatPlan;
			this.textNote.ReadOnly = true;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(790, 52);
			this.textNote.SpellCheckIsEnabled = false;
			this.textNote.TabIndex = 54;
			this.textNote.Text = "";
			this.textNote.TextChanged += new System.EventHandler(this.textNote_TextChanged);
			this.textNote.Leave += new System.EventHandler(this.textNote_Leave);
			// 
			// butSendToDevice
			// 
			this.butSendToDevice.Image = global::OpenDental.Properties.Resources.arrowRightLine;
			this.butSendToDevice.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSendToDevice.Location = new System.Drawing.Point(429, 91);
			this.butSendToDevice.Name = "butSendToDevice";
			this.butSendToDevice.Size = new System.Drawing.Size(90, 23);
			this.butSendToDevice.TabIndex = 78;
			this.butSendToDevice.Text = "eClipboard";
			this.butSendToDevice.Click += new System.EventHandler(this.butSendToDevice_Click);
			// 
			// menuConsent
			// 
			this.menuConsent.Popup += new System.EventHandler(this.menuConsent_Popup);
			// 
			// dashIndividualDiscount1
			// 
			this.userControlIndDis.Location = new System.Drawing.Point(799, 422);
			this.userControlIndDis.Name = "dashIndividualDiscount1";
			this.userControlIndDis.Size = new System.Drawing.Size(173, 84);
			this.userControlIndDis.TabIndex = 79;
			// 
			// ControlTreat
			// 
			this.Controls.Add(this.userControlIndDis);
			this.Controls.Add(this.butSendToDevice);
			this.Controls.Add(this.butInsRem);
			this.Controls.Add(this.userControlIndIns);
			this.Controls.Add(this.userControlFamIns);
			this.Controls.Add(this.butPlannedAppt);
			this.Controls.Add(this.labelCheckInsFrequency);
			this.Controls.Add(this.tabShowSort);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.dateTimeTP);
			this.Controls.Add(this.butSaveTP);
			this.Controls.Add(this.butNewTP);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.listSetPr);
			this.Controls.Add(this.ToolBarMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridPreAuth);
			this.Controls.Add(this.gridPlans);
			this.Controls.Add(this.textNote);
			this.Name = "ControlTreat";
			this.Size = new System.Drawing.Size(1195, 708);
			this.tabShowSort.ResumeLayout(false);
			this.tabPageShow.ResumeLayout(false);
			this.tabPageSort.ResumeLayout(false);
			this.tabPagePrint.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		///<summary>Only called on startup, but after local data loaded from db.</summary>
		public void InitializeOnStartup() {
			if(InitializedOnStartup) {
				return;
			}
			InitializedOnStartup=true;
			checkShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			if(PrefC.RandomKeys) {//random PKs don't get the option or sorting by order entered
				tabShowSort.TabPages.Remove(tabPageSort);
			}
			else {
				radioTreatPlanSortTooth.Checked=PrefC.GetBool(PrefName.TreatPlanSortByTooth);
			}
			//checkShowIns.Checked=PrefC.GetBool(PrefName.TreatPlanShowIns");
			//checkShowDiscount.Checked=PrefC.GetBool(PrefName.TreatPlanShowDiscount");
			//showHidden=true;//shows hidden priorities
			//can't use Lan.F(this);
			Lan.C(this,new Control[]
			{
				label1,
				tabShowSort,
				checkShowCompleted,
				checkShowIns,
				checkShowDiscount,
				checkShowMaxDed,
				checkShowFees,
				//checkShowStandard,
				checkShowSubtotals,
				checkShowTotals,
				gridMain,
				gridPlans,
				gridPreAuth,
				gridPrint,
				});
			LayoutToolBar();//redundant?
			tabShowSort.TabPages.Remove(tabPagePrint);//We may add this back in gridPlans_CellClick.
			if(Programs.UsingEcwTightOrFullMode()) {
				butPlannedAppt.Visible=false;
			}
		}

		///<summary>Called every time local data is changed from any workstation.  Refreshes priority lists and lays out the toolbar.</summary>
		public void InitializeLocalData() {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			listDefs.Insert(0,new Def { DefNum=0,ItemName=Lan.g(this,"no priority") });
			listSetPr.Items.Clear();
			foreach(Def def in listDefs) {
				listSetPr.Items.Add(def.ItemName,def);
			}
			LayoutToolBar();
			if(PrefC.GetBool(PrefName.EasyHideInsurance)){
				checkShowIns.Visible=false;
				checkShowIns.Checked=false;
				checkShowMaxDed.Visible=false;
				//checkShowMaxDed.Checked=false;
			}
			else{
				checkShowIns.Visible=true;
				checkShowMaxDed.Visible=true;
			}
		}

		///<summary>Causes the toolbar to be laid out again.</summary>
		public void LayoutToolBar(){
			ToolBarMain.Buttons.Clear();
			//ODToolBarButton button;
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"PreAuthorization"),-1,"","PreAuth"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Discount"),-1,"","Discount"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Update Fees"),1,"","Update"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"LabCase"),-1,"","LabCase"));
			ODToolBarButton button=new ODToolBarButton(Lan.g(this,"Consent"),-1,"","Consent");
			if(SheetDefs.GetCustomForType(SheetTypeEnum.Consent).Count>0) {
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuConsent;
			}
			ToolBarMain.Buttons.Add(button);
			//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Save TP"),3,"","Create"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Print TP"),2,"","Print"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Email TP"),-1,"","Email"));
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Sign TP"),-1,"","Sign"));
			ProgramL.LoadToolbar(ToolBarMain,ToolBarsAvail.TreatmentPlanModule);
			ToolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrTreat.LayoutToolBar_end",PatCur);
			UpdateToolbarButtons();
		}

		///<summary></summary>
		public void ModuleSelected(long patNum,bool hasDefaultDate=false) {
			if(hasDefaultDate) {
				dateTimeTP.Value=DateTime.Today;
			}
			RefreshModuleData(patNum);
			RefreshModuleScreen(false);
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,_tpData);
			Plugins.HookAddCode(this,"ContrTreat.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected(){
			UpdateTPNoteIfNeeded();//Handle this here because this happens before textNote_Leave() and we dont want anything nulled before saving;
			FamCur=null;
			PatCur=null;
			InsPlanList=null;
			//Claims.List=null;
			//ClaimProcs.List=null;
			//from FillMain:
			_listProcs=null;
			ProcListTP=null;
			//Procedures.HList=null;
			//Procedures.MissingTeeth=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			HasNoteChanged=false;
			_discountPlanSub=null;
			Plugins.HookAddCode(this,"ContrTreat.ModuleUnselected_end");
		}

		private void RefreshModuleData(long patNum) {
			UpdateTPNoteIfNeeded();
			if(patNum!=0) {
				bool doMakeSecLog=false;
				if(_patNumLast!=patNum) {
					doMakeSecLog=true;
					_patNumLast=patNum;
				}
				try {
					_tpData=TreatmentPlanModules.GetModuleData(patNum,doMakeSecLog);
				}
				catch(ApplicationException ex) {
					if(ex.Message=="Missing codenum") {
						MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
						PatCur=null;
						return;
					}
					throw;
				}
				FamCur=_tpData.Fam;
				PatCur=_tpData.Pat;
				SubList=_tpData.SubList;
				InsPlanList=_tpData.InsPlanList;
				_listSubstLinks=_tpData.ListSubstLinks;
				PatPlanList=_tpData.PatPlanList;
				BenefitList=_tpData.BenefitList;
				ClaimList=_tpData.ClaimList;
				HistList=_tpData.HistList;
				_listProcs=_tpData.ListProcedures;
				_listTreatPlans=_tpData.ListTreatPlans;
				ProcTPList=_tpData.ArrProcTPs;
				_discountPlanSub=_tpData.DiscountPlanSub;
			}
		}

		private void RefreshModuleScreen(bool doRefreshData=true){
			//ParentForm.Text=Patients.GetMainTitle(PatCur);
			FillPlans(doRefreshData);
			UpdateToolbarButtons();
			if(PatCur==null) {
				butNewTP.Enabled=false;
			}
			else {
				butNewTP.Enabled=true;
			}
			if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
				butRefresh.Visible=true;
				labelCheckInsFrequency.Visible=true;
				dateTimeTP.Visible=true;
			}
			else {
				butRefresh.Visible=false;
				labelCheckInsFrequency.Visible=false;
				dateTimeTP.Visible=false;
			}
			FillMain();
			FillSummary();
      FillPreAuth();
			//FillMisc();
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkShowCompleted.Visible=false;
			}
			else {
				checkShowCompleted.Visible=true;
				if(!_hasCheckShowCompletedChangedByUser) {
					//if the user has not manually changed checkShowCompleted, set it to the value of this default preference.  This will account for whenever
					//any user/workstation has changed the default and the current workstation has not made a change to this checkbox.
					checkShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
				}
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//Since the bonus information in FormInsRemain is currently only helpful in Canada,
				//we have decided not to show this button in other countries for now.
				butInsRem.Visible=true;
			}
			if(_listTreatPlans!=null && _listTreatPlans.Count==0) {//_listTreatPlans will be null when no patient is selected.
				textNote.Text="";
				HasNoteChanged=false;
			}
		}

		private delegate void ToolBarClick();

		private void menuConsent_Click(object sender,EventArgs e) {
			SheetDef sheetDef=(SheetDef)(((MenuItem)sender).Tag);
			SheetDefs.GetFieldsAndParameters(sheetDef);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatCur.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",PatCur.PatNum);
			StaticTextData data=new StaticTextData();
			List<GridRow> listProcTPRows=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();//Only pull selected rows that are procedures
			if(listProcTPRows.Count()>0) {//loop through selected procedures
				StaticTextFieldDependency dependencies=StaticTextData.GetStaticTextDependencies(sheet.SheetFields);
				List<long> listProcedureNums=listProcTPRows.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcs=Procedures.GetManyProc(listProcedureNums,false);//get list of procedures from list of procedureNums
				List<long> listProcCodeNums=listProcs.Select(x => x.CodeNum).ToList();
				if(listProcCodeNums.Count()>0) {
					dependencies|=StaticTextFieldDependency.ListSelectedTpProcs;
				}
				data=StaticTextData.GetStaticTextData(dependencies,PatCur,FamCur,listProcCodeNums);//set static text using selected procedure CodeNums
			}
			SheetFiller.FillFields(sheet,staticTextData: data);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void menuConsent_Popup(object sender,EventArgs e) {
			menuConsent.MenuItems.Clear();
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
			MenuItem menuItem;
			for(int i = 0;i<listSheetDefs.Count;i++) {
				menuItem=new MenuItem(listSheetDefs[i].Description);
				menuItem.Tag=listSheetDefs[i];
				menuItem.Click+=new EventHandler(menuConsent_Click);
				menuConsent.MenuItems.Add(menuItem);
			}
		}

		private void ToolBarMain_ButtonClick(object sender, OpenDental.UI.ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)){
				//standard predefined button
				switch(e.Button.Tag.ToString()){
					case "PreAuth":
						ToolBarMainPreAuth_Click();
						break;
					case "Discount":
						ToolBarMainDiscount_Click();
						break;
					case "Update":
						ToolBarMainUpdate_Click();
						break;
					//case "Create":
					//	ToolBarMainCreate_Click();
					//	break;
					case "Print":
						//The reason we are using a delegate and BeginInvoke() is because of a Microsoft bug that causes the Print Dialog window to not be in focus			
						//when it comes from a toolbar click.
						//https://social.msdn.microsoft.com/Forums/windows/en-US/681a50b4-4ae3-407a-a747-87fb3eb427fd/first-mouse-click-after-showdialog-hits-the-parent-form?forum=winforms
						ToolBarClick toolClick=ToolBarMainPrint_Click;
						this.BeginInvoke(toolClick);
						break;
					case "Email":
						ToolBarMainEmail_Click();
						break;
					case "Sign":
						ToolBarMainSign_Click();
						break;
					case "LabCase":
						ToolBarLabCase_Click();
						break;
					case "Consent":
						ToolBarConsent_Click();
						break;
				}
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,PatCur);
			}
			Plugins.HookAddCode(this,"ContrTreat.ToolBarMain_ButtonClick_end",PatCur,e);
		}

		private void checkShowCompleted_Click(object sender,EventArgs e) {
			_hasCheckShowCompletedChangedByUser=true;
		}

		private void butNewTP_Click(object sender,EventArgs e) {
			using FormTreatPlanCurEdit FormTPCE=new FormTreatPlanCurEdit();
			FormTPCE.TreatPlanCur=new TreatPlan() {
				Heading="Inactive Treatment Plan",
				Note=PrefC.GetString(PrefName.TreatmentPlanNote),
				PatNum=PatCur.PatNum,
				TPStatus=TreatPlanStatus.Inactive,
			};
			FormTPCE.ShowDialog();
			if(FormTPCE.DialogResult!=DialogResult.OK) {
				return;
			}
			long tpNum=FormTPCE.TreatPlanCur.TreatPlanNum;
			ModuleSelected(PatCur.PatNum);//refreshes TPs
			for(int i=0;i<_listTreatPlans.Count;i++) {
				if(_listTreatPlans[i].TreatPlanNum==tpNum) {
					gridPlans.SetSelected(i,true);
				}
			}
			FillMain();
		}

		private void butSaveTP_Click(object sender,EventArgs e) {
			ToolBarMainCreate_Click();
		}

		private void butSendToDevice_Click(object sender,EventArgs e) {
			TrySendTreatPlan();
		}

		private void FillPlans(bool doRefreshData=true){
			gridPlans.BeginUpdate();
			gridPlans.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTPList","Date"),70);
			gridPlans.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Status"),50);
			gridPlans.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Heading"),230);
			gridPlans.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Signed"),76,HorizontalAlignment.Center);
			gridPlans.ListGridColumns.Add(col);
			gridPlans.ListGridRows.Clear();
			if(PatCur==null){
				gridPlans.EndUpdate();
				return;
			}
			if(doRefreshData || _listProcs==null) {
				_listProcs=Procedures.Refresh(PatCur.PatNum);
			}
			ProcListTP=Procedures.SortListByTreatPlanPriority(_listProcs.FindAll(x => x.ProcStatus==ProcStat.TP || x.ProcStatus==ProcStat.TPi)
				,PrefC.IsTreatPlanSortByTooth);//sorted by priority, then (conditionally) toothnum
			//_listTPCurrent=TreatPlans.Refresh(PatCur.PatNum,new[] {TreatPlanStatus.Active,TreatPlanStatus.Inactive});
			if(doRefreshData || _listTreatPlans==null) {
				_listTreatPlans=TreatPlans.GetAllForPat(PatCur.PatNum);
			}
			_listTreatPlans=_listTreatPlans
					.OrderBy(x => x.TPStatus!=TreatPlanStatus.Active)
					.ThenBy(x => x.TPStatus!=TreatPlanStatus.Inactive)
					.ThenBy(x => x.DateTP).ToList();
			if(doRefreshData || ProcTPList==null) {
				ProcTPList=ProcTPs.Refresh(PatCur.PatNum);
			}
			if(doRefreshData) {
				_discountPlanSub=DiscountPlanSubs.GetSubForPat(PatCur.PatNum);
			}
			OpenDental.UI.GridRow row;
			//row=new ODGridRow();
			//row.Cells.Add("");//date empty
			//row.Cells.Add("");//date empty
			//row.Cells.Add(Lan.g(this,"Current Treatment Plans"));
			//gridPlans.Rows.Add(row);
			string str;
			Sheet sheetTP=null;
			if(DoPrintUsingSheets()) {
				sheetTP=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.TreatmentPlan),PatCur.PatNum);
			}
			bool hasPracticeSig=false;
			for(int i=0;i<_listTreatPlans.Count;i++){
				row=new GridRow();
				TreatPlan treatPlanCur=_listTreatPlans[i];
				row.Cells.Add(treatPlanCur.TPStatus==TreatPlanStatus.Saved? treatPlanCur.DateTP.ToShortDateString():"");
				row.Cells.Add(treatPlanCur.TPStatus.ToString());
				str=treatPlanCur.Heading;
				if(treatPlanCur.ResponsParty!=0){
					str+="\r\n"+Lan.g(this,"Responsible Party: ")+Patients.GetLim(treatPlanCur.ResponsParty).GetNameLF();
				}
				row.Cells.Add(str);
				hasPracticeSig=sheetTP?.SheetFields?.Any(x => x.FieldType==SheetFieldType.SigBoxPractice)??false;
				if(string.IsNullOrEmpty(treatPlanCur.Signature) || (hasPracticeSig && string.IsNullOrEmpty(treatPlanCur.SignaturePractice))) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				row.Tag=treatPlanCur;
				gridPlans.ListGridRows.Add(row);
			}
			gridPlans.EndUpdate();
			gridPlans.SetSelected(0,true);
		}

		private void FillMain() {
			//Newly selected TreatPlan, if any.
			TreatPlan treatPlan=(gridPlans.SelectedIndices.Length > 0) ? _listTreatPlans[gridPlans.SelectedIndices[0]] : null;
			//If changing selection or initial selection.
			bool isChangingTP=(treatPlan!=null && treatPlan.TreatPlanNum!=(_selectedTreatPlan?.TreatPlanNum));
			if(isChangingTP) {
				_selectedTreatPlan=treatPlan;//Update to new selection.
			}
			if((treatPlan!=null && treatPlan.Signature!="")//disable changing priorities for signed TPs
				|| PatCur==null ||_listTreatPlans.Count<1)//disable if the patient has no TPs
			{
				listSetPr.Enabled=false; 
			}
			else {
				listSetPr.Enabled=true;//allow changing priority for un-signed TPs
			}
			FillMainData();
			FillMainDisplay(isChangingTP);
		}

		/// <summary>Fills RowsMain list for gridMain display.</summary>
		private void FillMainData() {
			decimal subfee=0;
			decimal suballowed=0;
			decimal subpriIns=0;
			decimal subsecIns=0;
			decimal subdiscount=0;
			decimal subpat=0;
			decimal subTaxEst=0;
			decimal subCatPercUCR=0;
			decimal totFee=0;
			decimal totPriIns=0;
			decimal totSecIns=0;
			decimal totDiscount=0;
			decimal totPat=0;
			decimal totAllowed=0;
			decimal totTaxEst=0;
			decimal totCatPercUCR=0;
			RowsMain=new List<TpRow>();
			if(PatCur==null || gridPlans.ListGridRows.Count==0) {
				return;
			}
			TpRow row;
			TreatPlan treatPlanTemp=_listTreatPlans[gridPlans.SelectedIndices[0]];
			//Active and Inactive Treatment Plans========================================================================
			if(treatPlanTemp.TPStatus==TreatPlanStatus.Active
				 || treatPlanTemp.TPStatus==TreatPlanStatus.Inactive)
			{
				LoadActiveTP(ref treatPlanTemp);
				return;
			}
			//Archived TPs below this point==============================================================================
			ProcTPSelectList=ProcTPs.GetListForTP(_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum,ProcTPList);
			List<Appointment> listApts=Appointments.GetAppointmentsForProcs(_listProcs);
			bool isDone;
			for(int i=0;i<ProcTPSelectList.Length;i++) {
				row=new TpRow();
				isDone=false;
				long plannedAptNumCur=0;
				long aptNumCur=0;
				ApptStatus statusCur=ApptStatus.None;
				if(ProcTPSelectList[i].TagOD==null) {
					ProcTPSelectList[i].TagOD=0L;
				}
				for(int j=0;j<_listProcs.Count;j++) {
					if(_listProcs[j].ProcNum==ProcTPSelectList[i].ProcNumOrig) {
						if((long)ProcTPSelectList[i].TagOD==0) {
							ProcTPSelectList[i].TagOD=_listProcs[j].ProcNumLab;
						}
						if(_listProcs[j].ProcStatus==ProcStat.C) {
							isDone=true;
						}
						plannedAptNumCur=_listProcs[j].PlannedAptNum;
						aptNumCur=_listProcs[j].AptNum;
						Appointment apt=listApts.FirstOrDefault(x => x.AptNum==aptNumCur);
						if(apt!=null) {
							statusCur=apt.AptStatus;
						}
					}
				}
				if(isDone) {
					row.Done="X";
				}
				row.ProcAbbr=ProcTPSelectList[i].ProcAbbr;
				row.Priority=Defs.GetName(DefCat.TxPriorities,ProcTPSelectList[i].Priority);
				row.Tth=ProcTPSelectList[i].ToothNumTP;
				row.Surf=ProcTPSelectList[i].Surf;
				row.Code=ProcTPSelectList[i].ProcCode;
				row.Description=ProcTPSelectList[i].Descript;
				row.Fee=(decimal)ProcTPSelectList[i].FeeAmt; //Fee
				row.PriIns=(decimal)ProcTPSelectList[i].PriInsAmt; //PriIns or DiscountPlan
				row.SecIns=(decimal)ProcTPSelectList[i].SecInsAmt; //SecIns
				row.FeeAllowed=(decimal)ProcTPSelectList[i].FeeAllowed;
				row.TaxEst=(decimal)ProcTPSelectList[i].TaxAmt; //Estimated Tax
				row.ProvNum=ProcTPSelectList[i].ProvNum;
				row.DateTP=ProcTPSelectList[i].DateTP;
				row.ClinicNum=ProcTPSelectList[i].ClinicNum;
				row.Appt=row.Appt=FillApptCellForRow(plannedAptNumCur,aptNumCur,statusCur);
				row.CatPercUCR=(decimal)ProcTPSelectList[i].CatPercUCR;
				//Totals
				subfee+=row.Fee;
				if(CompareDecimal.IsGreaterThan(row.FeeAllowed,-1)) {//-1 means the proc is DoNotBillIns
					suballowed+=row.FeeAllowed;
					totAllowed+=(decimal)ProcTPSelectList[i].FeeAllowed;
				}
				totFee+=row.Fee;
				subpriIns+=(decimal)ProcTPSelectList[i].PriInsAmt;
				totPriIns+=treatPlanTemp.TPType==TreatPlanType.Insurance ? (decimal)ProcTPSelectList[i].PriInsAmt : row.PriIns;
				subsecIns+=(decimal)ProcTPSelectList[i].SecInsAmt;
				totSecIns+=(decimal)ProcTPSelectList[i].SecInsAmt;
				row.Discount=(decimal)ProcTPSelectList[i].Discount; //Discount
				subdiscount+=(decimal)ProcTPSelectList[i].Discount;
				totDiscount+=(decimal)ProcTPSelectList[i].Discount;
				row.Pat=(decimal)ProcTPSelectList[i].PatAmt; //Pat
				subpat+=(decimal)ProcTPSelectList[i].PatAmt;
				totPat+=(decimal)ProcTPSelectList[i].PatAmt;
				subTaxEst+=(decimal)ProcTPSelectList[i].TaxAmt;
				totTaxEst+=(decimal)ProcTPSelectList[i].TaxAmt;
				subCatPercUCR+=(decimal)ProcTPSelectList[i].CatPercUCR;
				totCatPercUCR+=(decimal)ProcTPSelectList[i].CatPercUCR;
				row.Prognosis=ProcTPSelectList[i].Prognosis; //Prognosis
				row.Dx=ProcTPSelectList[i].Dx;
				row.ColorText=Defs.GetColor(DefCat.TxPriorities,ProcTPSelectList[i].Priority);
				if(row.ColorText==System.Drawing.Color.White) {
					row.ColorText=System.Drawing.Color.Black;
				}
				row.Tag=ProcTPSelectList[i].Copy();
				RowsMain.Add(row);
				if(checkShowSubtotals.Checked &&
					 (i==ProcTPSelectList.Length-1 || ProcTPSelectList[i+1].Priority!=ProcTPSelectList[i].Priority)) {
					row=new TpRow();
					row.Description=Lan.g("TableTP","Subtotal");
					row.Fee=subfee;
					row.PriIns=subpriIns;
					row.SecIns=subsecIns;
					row.Discount=subdiscount;
					row.Pat=subpat;
					row.FeeAllowed=suballowed;
					row.TaxEst=subTaxEst;
					row.CatPercUCR=subCatPercUCR;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,ProcTPSelectList[i].Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					row.Bold=true;
					row.ColorLborder=System.Drawing.Color.Black;
					RowsMain.Add(row);
					subfee=0;
					subpriIns=0;
					subsecIns=0;
					subdiscount=0;
					subpat=0;
					suballowed=0;
					subTaxEst=0;
					subCatPercUCR=0;
				}
			}
			if(checkShowDiscount.Checked && CompareDecimal.IsZero(totDiscount)) { //mimics Active/Inactive TP logic
				checkShowDiscount.Checked=false;
			}
			textNote.Text=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			HasNoteChanged=false;
			if((_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved 
				&& _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="") 
				|| (_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Inactive 
				&& _listTreatPlans[gridPlans.SelectedIndices[0]].Heading==Lan.g("TreatPlan","Unassigned")))
			{
				textNote.ReadOnly=true;
			}
			else {
				textNote.ReadOnly=false;
			}
			if(checkShowTotals.Checked) {
				row=new TpRow();
				row.Description=Lan.g("TableTP","Total");
				row.Fee=totFee;
				row.PriIns=totPriIns;
				row.SecIns=totSecIns;
				row.Discount=totDiscount;
				row.Pat=totPat;
				row.FeeAllowed=totAllowed;
				row.TaxEst=totTaxEst;
				row.CatPercUCR=totCatPercUCR;
				row.Bold=true;
				row.ColorText=System.Drawing.Color.Black;
				RowsMain.Add(row);
			}
		}

		private void FillMainDisplay(bool hasSelectedTpChanged=false){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			if(PatCur==null || gridPlans.ListGridRows.Count==0 || _listTreatPlans[gridPlans.SelectedIndices[0]].TPType==TreatPlanType.Insurance) {
				fields.RemoveAll(x => x.InternalName=="DPlan");//If patient doesn't have discount plan, don't show column.
			}
			else {
				fields.RemoveAll(x => x.InternalName=="Pri Ins" || x.InternalName=="Sec Ins" || x.InternalName=="Allowed");//If patient does have discount plan, don't show Pri or Sec Ins or allowed fee.
			}
			bool hasSalesTax=HasSalesTax(fields);
			//Changing to TreatPlan other than Active/Inactive.  CheckBox states have already been reset to user selections if changing to the Active/Inactive TreatPlan.
			if(hasSelectedTpChanged && !ListTools.In(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus,TreatPlanStatus.Active,TreatPlanStatus.Inactive)) {
				bool wasShowInsChecked=checkShowIns.Checked;
				bool wasShowDiscountChecked = checkShowDiscount.Checked;
				if((!wasShowInsChecked && checkShowIns.Checked) || (!wasShowDiscountChecked && checkShowDiscount.Checked)) {//checkShowIns was changed to checked, which affects how FillMainData() sets each row.Fee.
					FillMainData();//So we must refill main data.
				}
			}
			for(int i=0;i<fields.Count;i++){
				if(fields[i].Description==""){
					col=new GridColumn(fields[i].InternalName,fields[i].ColumnWidth);
				}
				else{
					col=new GridColumn(fields[i].Description,fields[i].ColumnWidth);
				}
				if(fields[i].InternalName=="Fee" && !checkShowFees.Checked){
					continue;
				}
				if((fields[i].InternalName=="Pri Ins" || fields[i].InternalName=="Sec Ins" || fields[i].InternalName=="DPlan" || fields[i].InternalName=="Allowed") 
					&& !checkShowIns.Checked){
					continue;
				}
				if(fields[i].InternalName=="Discount" && !checkShowDiscount.Checked){
					continue;
				}
				if(fields[i].InternalName=="Pat" && !checkShowIns.Checked && !checkShowDiscount.Checked && !hasSalesTax){
					continue;
				}
				if(fields[i].InternalName=="Tax Est" && !hasSalesTax) {
					continue;
				}
				if(fields[i].InternalName=="Fee" 
					|| fields[i].InternalName=="Pri Ins"
					|| fields[i].InternalName=="Sec Ins"
					|| fields[i].InternalName=="DPlan"
					|| fields[i].InternalName=="Discount"
					|| fields[i].InternalName=="Pat"
					|| fields[i].InternalName=="Allowed"
					|| fields[i].InternalName=="Tax Est"
					|| fields[i].InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR) 
				{
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(ListTools.In(fields[i].InternalName,"Sub",DisplayFields.InternalNames.TreatmentPlanModule.Appt)) {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridMain.ListGridColumns.Add(col);
			}			
			gridMain.ListGridRows.Clear();
			if(PatCur==null){
				gridMain.EndUpdate();
				return;
			}
			if(RowsMain==null || RowsMain.Count==0) {
				gridMain.EndUpdate();
				return;
			}
			GridRow row;
			for(int i=0;i<RowsMain.Count;i++){
				row=new GridRow();
				for(int j=0;j<fields.Count;j++) {
					switch(fields[j].InternalName) {
						case "Done":
							if(RowsMain[i].Done!=null) {
								row.Cells.Add(RowsMain[i].Done.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Priority":
							if(RowsMain[i].Priority!=null) {
								row.Cells.Add(RowsMain[i].Priority.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tth":
							if(RowsMain[i].Tth!=null) {
								row.Cells.Add(RowsMain[i].Tth.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Surf":
							if(RowsMain[i].Surf!=null) {
								row.Cells.Add(RowsMain[i].Surf.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Code":
							if(RowsMain[i].Code!=null) {
								row.Cells.Add(RowsMain[i].Code.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Sub":
							if(HasSubstCodeForTpRow(RowsMain[i])) {
								row.Cells.Add("X");//They allow substitutions.
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Description":
							if(RowsMain[i].Description!=null) {
								row.Cells.Add(RowsMain[i].Description.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Fee":
							if(checkShowFees.Checked) {
								row.Cells.Add(RowsMain[i].Fee.ToString("F"));
							}
							break;
						case "Pri Ins":
							if(checkShowIns.Checked) {
								row.Cells.Add(RowsMain[i].PriIns.ToString("F"));
							}
							break;
						case "DPlan":
							if(checkShowIns.Checked) {
								row.Cells.Add(RowsMain[i].PriIns.ToString("F"));
							}
							break;
						case "Sec Ins":
							if(checkShowIns.Checked) {
								row.Cells.Add(RowsMain[i].SecIns.ToString("F"));
							}
							break;
						case "Discount":
							if(checkShowDiscount.Checked) {
								row.Cells.Add(RowsMain[i].Discount.ToString("F"));
							}
							break;
						case "Pat":
							if(checkShowIns.Checked || checkShowDiscount.Checked || hasSalesTax) {
								row.Cells.Add(RowsMain[i].Pat.ToString("F"));
							}
							break;
						case "Prognosis":
							if(RowsMain[i].Prognosis!=null) {
								row.Cells.Add(RowsMain[i].Prognosis.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Dx":
							if(RowsMain[i].Dx!=null) {
								row.Cells.Add(RowsMain[i].Dx.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Abbr":
							if(!String.IsNullOrEmpty(RowsMain[i].ProcAbbr)){
								row.Cells.Add(RowsMain[i].ProcAbbr.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Allowed":
							if(checkShowIns.Checked) {
								if(CompareDecimal.IsGreaterThan(RowsMain[i].FeeAllowed,-1)) {//-1 means the proc is DoNotBillIns
									row.Cells.Add(RowsMain[i].FeeAllowed.ToString("F"));
								}
								else {
									row.Cells.Add("X");
								}
							}
							break;
						case "Tax Est":
							if(hasSalesTax) {
								row.Cells.Add(RowsMain[i].TaxEst.ToString("F"));
							}
							break;
						case "Prov":
							if(RowsMain[i].ProvNum>0){
								row.Cells.Add(Providers.GetAbbr(RowsMain[i].ProvNum));
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "DateTP":
							if(RowsMain[i].DateTP.Year>=1880) {
								row.Cells.Add(RowsMain[i].DateTP.ToShortDateString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(RowsMain[i].ClinicNum));//Will be blank if ClinicNum not found, i.e. the 0 clinic.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.Appt:
							row.Cells.Add(RowsMain[i].Appt);//"X" if procedure has an AptNum>0 otherwise blank.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR:
							row.Cells.Add(RowsMain[i].CatPercUCR.ToString("F"));
							break;
					}
				}
				if(RowsMain[i].ColorText!=null) {
					row.ColorText=RowsMain[i].ColorText;
				}
				if(RowsMain[i].ColorLborder!=null) {
					row.ColorLborder=RowsMain[i].ColorLborder;
				}
				if(RowsMain[i].Tag!=null) {
					row.Tag=RowsMain[i].Tag;
				}
				row.Bold=RowsMain[i].Bold;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridPrint() {
			this.Controls.Add(gridPrint);
			gridPrint.BeginUpdate();
			gridPrint.ListGridColumns.Clear();
			GridColumn col;
			DisplayFields.RefreshCache();//probably needs to be removed.
			List<DisplayField> fields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			if(PatCur==null || gridPlans.ListGridRows.Count==0 || _listTreatPlans[gridPlans.SelectedIndices[0]].TPType==TreatPlanType.Insurance) {
				fields.RemoveAll(x => x.InternalName=="DPlan");//If patient doesn't have discount plan, don't show column.
			}
			else {
				fields.RemoveAll(x => x.InternalName=="Pri Ins" || x.InternalName=="Sec Ins");//If patient does have discount plan, don't show Pri or Sec Ins.
			}
			bool hasSalesTax=HasSalesTax(fields);
			for(int i=0;i<fields.Count;i++) {
				if(fields[i].Description=="") {
					col=new GridColumn(fields[i].InternalName,fields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(fields[i].Description,fields[i].ColumnWidth);
				}
				if(fields[i].InternalName=="Fee" && !checkShowFees.Checked) {
					continue;
				}
				if((ListTools.In(fields[i].InternalName,"Pri Ins","Sec Ins","DPlan","Allowed")) && !checkShowIns.Checked) {
					continue;
				}
				if(fields[i].InternalName=="Discount" && !checkShowDiscount.Checked) {
					continue;
				}
				if(fields[i].InternalName=="Pat" && !checkShowIns.Checked && !checkShowDiscount.Checked && !hasSalesTax) {
					continue;
				}
				if(fields[i].InternalName=="Tax Est" && !hasSalesTax) {
					continue;
				}
				if(fields[i].InternalName=="Fee" 
					|| fields[i].InternalName=="Pri Ins"
					|| fields[i].InternalName=="Sec Ins"
					|| fields[i].InternalName=="DPlan"
					|| fields[i].InternalName=="Discount"
					|| fields[i].InternalName=="Pat"
					|| fields[i].InternalName=="Tax Est"
					|| fields[i].InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR)
				{
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(ListTools.In(fields[i].InternalName,"Sub",DisplayFields.InternalNames.TreatmentPlanModule.Appt)) {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridPrint.ListGridColumns.Add(col);
			}
			gridPrint.ListGridRows.Clear();
			if(PatCur==null) {
				gridPrint.EndUpdate();
				return;
			}
			GridRow row;
			for(int i=0;i<RowsMain.Count;i++) {
				row=new GridRow();
				for(int j=0;j<fields.Count;j++) {
					switch(fields[j].InternalName) {
						case "Done":
							if(RowsMain[i].Done!=null) {
								row.Cells.Add(RowsMain[i].Done.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Priority":
							if(RowsMain[i].Priority!=null) {
								row.Cells.Add(RowsMain[i].Priority.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tth":
							if(RowsMain[i].Tth!=null) {
								row.Cells.Add(RowsMain[i].Tth.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Surf":
							if(RowsMain[i].Surf!=null) {
								row.Cells.Add(RowsMain[i].Surf.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Code":
							if(RowsMain[i].Code!=null) {
								row.Cells.Add(RowsMain[i].Code.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Sub":
							if(HasSubstCodeForTpRow(RowsMain[i])) {
								row.Cells.Add("X");//They allow substitutions.
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Description":
							if(RowsMain[i].Description!=null) {
								row.Cells.Add(RowsMain[i].Description.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Fee":
							if(checkShowFees.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || RowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| RowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(RowsMain[i].Fee.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Pri Ins":
						case "DPlan":
							if(checkShowIns.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || RowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| RowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(RowsMain[i].PriIns.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Sec Ins":
							if(checkShowIns.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || RowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| RowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(RowsMain[i].SecIns.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Discount":
							if(checkShowDiscount.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || RowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| RowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal"))
								{
									row.Cells.Add(RowsMain[i].Discount.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Pat":
							if(checkShowIns.Checked || checkShowDiscount.Checked || hasSalesTax) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || RowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| RowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(RowsMain[i].Pat.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Prognosis":
							if(RowsMain[i].Prognosis!=null) {
								row.Cells.Add(RowsMain[i].Prognosis.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Dx":
							if(RowsMain[i].Dx!=null) {
								row.Cells.Add(RowsMain[i].Dx.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Abbr":
							if(!String.IsNullOrEmpty(RowsMain[i].ProcAbbr)){
								row.Cells.Add(RowsMain[i].ProcAbbr.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tax Est":
							if(hasSalesTax) {
								row.Cells.Add(RowsMain[i].TaxEst.ToString("F"));
							}
							break;
						case "Prov":
							if(RowsMain[i].ProvNum>0){
								row.Cells.Add(Providers.GetAbbr(RowsMain[i].ProvNum));
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "DateTP":
							if(RowsMain[i].DateTP.Year>=1880) {
								row.Cells.Add(RowsMain[i].DateTP.ToShortDateString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(RowsMain[i].ClinicNum));//Will be blank if ClinicNum not found, i.e. the 0 clinic.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.Appt:
							row.Cells.Add(RowsMain[i].Appt);//"X" if procedure has an AptNum>0 otherwise blank.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR:
							if(PrefC.GetBool(PrefName.TreatPlanItemized) 
								|| ListTools.In(RowsMain[i].Description.ToString(),Lan.g("TableTP","Total"),Lan.g("TableTP","Subtotal"))) 
							{
								row.Cells.Add(RowsMain[i].CatPercUCR.ToString("F"));
							}
							else {
								row.Cells.Add("");
							}
							break;
					}
				}
				if(RowsMain[i].ColorText!=null) {
					row.ColorText=RowsMain[i].ColorText;
				}
				if(RowsMain[i].ColorLborder!=null) {
					row.ColorLborder=RowsMain[i].ColorLborder;
				}
				if(RowsMain[i].Tag!=null) {
					row.Tag=RowsMain[i].Tag;
				}
				row.Bold=RowsMain[i].Bold;
				gridPrint.ListGridRows.Add(row);
			}
			gridPrint.EndUpdate();
		}

		private bool HasSubstCodeForTpRow(TpRow row) {
			//If any patient insplan allows subst codes (if !plan.CodeSubstNone) and the code has a valid substitution code, then indicate the substitution.
			ProcedureCode procCode=ProcedureCodes.GetProcCode(row.Code);
			if(!ProcedureCodes.IsValidCode(procCode.ProcCode)) {
				//TpRow is not a valid procedure. Return false.
				return false;
			}
			//The lists gotten at the beginning of ContrTreat are not patient specific with the exception of the PatPlanList.
			//Get all patient-specific InsSubs
			List<InsSub> listPatInsSubs=SubList.FindAll(x => PatPlanList.Any(y => y.InsSubNum==x.InsSubNum));
			//Get all patient-specific InsPlans
			List<InsPlan> listPatInsPlans=InsPlanList.FindAll(x => listPatInsSubs.Any(y => y.PlanNum==x.PlanNum));
			return SubstitutionLinks.HasSubstCodeForProcCode(procCode,row.Tth.ToString(),_listSubstLinks,listPatInsPlans);
		}

		///<summary>Helper method used to determine if sales tax is displayed.</summary>
		private bool HasSalesTax(List<DisplayField> fields) {
			if(PatCur!=null && fields.Any(x => x.InternalName=="Tax Est")) {
				if(PrefC.IsODHQ) {
					return AvaTax.IsTaxable(PatCur.PatNum);
				}
				else {
					return true;
				}
			}
			return false;
		}

		private void FillSummary(){
			userControlFamIns.Visible=false;
			userControlIndIns.Visible=false;
			userControlIndDis.Visible=false;
			if(PatCur==null) {
				return;
			}
			if(_discountPlanSub!=null) {
				userControlIndDis.Visible=true;
				userControlIndDis.RefreshDiscountPlan(PatCur,_tpData.DiscountPlanSub,_tpData.DiscountPlan);
			} 
			else {
				userControlFamIns.Visible=true;
				userControlIndIns.Visible=true;
				userControlFamIns.RefreshInsurance(PatCur,InsPlanList,SubList,PatPlanList,BenefitList);
				userControlIndIns.RefreshInsurance(PatCur,InsPlanList,SubList,PatPlanList,BenefitList,HistList);
			}
		}		

    private void FillPreAuth(){
			gridPreAuth.BeginUpdate();
			gridPreAuth.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePreAuth","Date Sent"),80);
			gridPreAuth.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePreAuth","Carrier"),100);
			gridPreAuth.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TablePreAuth","Status"),53);
			gridPreAuth.ListGridColumns.Add(col);
			gridPreAuth.ListGridRows.Clear();
      if(PatCur==null){
				gridPreAuth.EndUpdate();
				return;
			}
      ALPreAuth=new ArrayList();			
      for(int i=0;i<ClaimList.Count;i++){
        if(ClaimList[i].ClaimType=="PreAuth"){
          ALPreAuth.Add(ClaimList[i]);
        }
      }
			OpenDental.UI.GridRow row;
      for(int i=0;i<ALPreAuth.Count;i++) {
				if(ALPreAuth[i]==null) {
					continue;
				}
				InsPlan PlanCur=InsPlans.GetPlan(((Claim)ALPreAuth[i]).PlanNum,InsPlanList);
				row=new GridRow();
				if(((Claim)ALPreAuth[i]).DateSent.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(((Claim)ALPreAuth[i]).DateSent.ToShortDateString());
				}
				if(PlanCur==null) {
					row.Cells.Add(Lan.g(this,"Unknown"));
				}
				else {
					row.Cells.Add(Carriers.GetName(PlanCur.CarrierNum));
				}
				row.Cells.Add(((Claim)ALPreAuth[i]).ClaimStatus.ToString());
				gridPreAuth.ListGridRows.Add(row);
      }
			gridPreAuth.EndUpdate();
    }

		private void gridMain_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			gridPreAuth.SetAll(false);//is this a desirable behavior?
			if(gridMain.ListGridRows[e.Row].Tag==null) {
				return;//skip any hightlighted subtotal lines
			}
			CanadianSelectedRowHelper(((ProcTP)gridMain.ListGridRows[e.Row].Tag));
		}

		///<summary>Selects any associated lab procedures for the given selectedProcTp in gridMain.</summary>
		private void CanadianSelectedRowHelper(ProcTP selectedProcTp) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return;
			}
			long selectedProcNumLab=(long)selectedProcTp.TagOD;//0 or FK to parent proc
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==null){
					continue;//skip any hightlighted subtotal lines
				}
				long rowProcNumOrig=((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig;
				long rowParentProcNum=(long)((ProcTP)gridMain.ListGridRows[i].Tag).TagOD;//0 or FK to parent proc
				if(rowProcNumOrig==selectedProcNumLab //User clicked lab, select parent proc too.
					|| (rowParentProcNum!=0 && rowParentProcNum==selectedProcNumLab)//User clicked lab, select other labs associated to same parent proc.
					|| (selectedProcTp.ProcNumOrig==rowParentProcNum))//User clicked parent, select associated lab procs.
				{
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void gridMain_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			if(gridMain.ListGridRows[e.Row].Tag==null){
				return;//user double clicked on a subtotal row
			}
			if(gridPlans.GetSelectedIndex()>-1 
				&& (_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Active 
					||_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Inactive))
			{//current plan
				TreatPlan treatPlanCur=_listTreatPlans[gridPlans.SelectedIndices[0]];
				Procedure procCur=Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[e.Row].Tag).ProcNumOrig,true); 
				//generate a new loop list containing only the procs before this one in it
				LoopList=new List<ClaimProcHist>();
				for(int i=0;i<ProcListTP.Length;i++) {
					if(ProcListTP[i].ProcNum==procCur.ProcNum) {
						break;
					}
					if(treatPlanCur.TPStatus==TreatPlanStatus.Active && !_loadActiveData.ListTreatPlanAttaches
						.Any(x => x.ProcNum==ProcListTP[i].ProcNum && x.TreatPlanNum==treatPlanCur.TreatPlanNum)) 
					{
						continue;//If this is the active plan, only include TP procs that are attached to this treatment plan
					}
					LoopList.AddRange(ClaimProcs.GetHistForProc(ClaimProcList,ProcListTP[i].ProcNum,ProcListTP[i].CodeNum));
				}
				using FormProcEdit FormPE=new FormProcEdit(procCur,PatCur,FamCur,listPatToothInitials:ToothInitialList);
				FormPE.LoopList=LoopList;
				FormPE.HistList=HistList;
				FormPE.ShowDialog();
				long treatPlanNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
				ModuleSelected(PatCur.PatNum);
				gridPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(x=>x.TreatPlanNum==treatPlanNum)),true);
				//This only updates the grid of procedures, in case any changes were made.
				FillMain();
				for(int i=0;i<gridMain.ListGridRows.Count;i++){
					if(gridMain.ListGridRows[i].Tag !=null && ((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig==procCur.ProcNum){
						gridMain.SetSelected(i,true);
					}
				}
				return;
			}
			//any other TP
			ProcTP procT=(ProcTP)gridMain.ListGridRows[e.Row].Tag;
			DateTime dateTP=_listTreatPlans[gridPlans.SelectedIndices[0]].DateTP;
			bool isSigned=false;
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="") {
				isSigned=true;
			}
			using FormProcTPEdit FormP=new FormProcTPEdit(procT,dateTP,isSigned);
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel){
				return;
			}
			int selectedPlanI=gridPlans.SelectedIndices[0];
			long selectedProc=procT.ProcTPNum;
			bool isDiscountChecked=checkShowDiscount.Checked;
			ModuleSelected(PatCur.PatNum);
			gridPlans.SetSelected(selectedPlanI,true);
			checkShowDiscount.Checked=isDiscountChecked;
			if(FormP.ProcCur!=null && FormP.ProcCur.Discount>0) {
				checkShowDiscount.Checked=true;
			}
			FillMain();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(gridMain.ListGridRows[i].Tag !=null && ((ProcTP)gridMain.ListGridRows[i].Tag).ProcTPNum==selectedProc){ 
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void gridPlans_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			TreatPlan treatPlan=null;
			if(gridPlans.SelectedIndices.Length > 0) {
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
				List<ProcTP> listProcTPs=treatPlan.ListProcTPs;
				if(treatPlan.TPStatus==TreatPlanStatus.Saved) {
					listProcTPs=ProcTPs.GetListForTP(treatPlan.TreatPlanNum,ProcTPList).ToList();
				}
				if(CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.Discount)) && !checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
				if(!checkShowInsNotAutomatic
					&& (CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.PriInsAmt)) 
						|| CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.SecInsAmt)) 
						|| CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.FeeAllowed))))
				{
					checkShowIns.Checked=true;
				}
			}
			FillMain();
			gridPreAuth.SetAll(false);
			if(gridPlans.SelectedIndices.Length > 0) {
				if(treatPlan.TPStatus==TreatPlanStatus.Saved && treatPlan.DateTP < UpdateHistories.GetDateForVersion(new Version(17,1,0,0))) {
					//In 17.1 we forced everyone to switch to using sheets for TPs. In order to avoid making it appear that historical data has changed,
					//we give the option to print using the classic view for treatment plans that were saved before updating to 17.1.
					if(!tabShowSort.TabPages.Contains(tabPagePrint)) {
						tabShowSort.TabPages.Add(tabPagePrint);
					}
				}
				else {
					if(tabShowSort.TabPages.Contains(tabPagePrint)) {
						tabShowSort.TabPages.Remove(tabPagePrint);
					}
				}
			}
		}

		private void gridPlans_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			//if(e.Row==0){
			//	return;//there is nothing to edit if user clicks on current.
			//}
			long tpNum=_listTreatPlans[e.Row].TreatPlanNum;
			TreatPlan tpSelected=_listTreatPlans[e.Row];
			if(tpSelected.TPStatus==TreatPlanStatus.Saved) {
				using FormTreatPlanEdit FormT=new FormTreatPlanEdit(_listTreatPlans[e.Row]);
				FormT.ShowDialog();
			}
			else {
				using FormTreatPlanCurEdit FormTPC=new FormTreatPlanCurEdit();
				FormTPC.TreatPlanCur=tpSelected;
				FormTPC.ShowDialog();
			}
			ModuleSelected(PatCur.PatNum);
			for(int i=0;i<_listTreatPlans.Count;i++){
				if(_listTreatPlans[i].TreatPlanNum==tpNum){
					gridPlans.SetSelected(i,true);
				}
			}
			FillMain();
		}
		
		///<summary>Attempts to send the currently selected TP in gridPlans to an eClipboard device.
		///If a device is set to the same patient as PatCur then we will automatically PUSH the data to them.
		///Otherwise we prompt for an unlock code if doPromptForDevice is not enabled.</summary>
		private void TrySendTreatPlan(){
			//Mimics FillPlans()
			TreatPlan treatPlanCur=gridPlans.SelectedTag<TreatPlan>();
			Sheet sheetTP=null;
			if(DoPrintUsingSheets()) {
				sheetTP=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.TreatmentPlan),PatCur.PatNum);
			}
			bool hasPracticeSig=sheetTP?.SheetFields?.Any(x => x.FieldType==SheetFieldType.SigBoxPractice)??false;
			if(PatCur==null){
				MsgBox.Show("Please select a patient first.");
				return;
			}
			if(treatPlanCur==null) {//Shouldn't happen, control auto selects when loading.
				MsgBox.Show("Please select a Saved Treatment Plan first.");
				return;
			}
			else if(treatPlanCur.TPStatus!=TreatPlanStatus.Saved){
				//Eventually we plan on implementing something here to save TPs for user when not saved.
				MsgBox.Show("Only Saved Treatment Plans can be sent to a mobile device.");
				return;
			}
			else if(!string.IsNullOrEmpty(treatPlanCur.Signature) || (hasPracticeSig && !string.IsNullOrEmpty(treatPlanCur.SignaturePractice)))
			{
				MsgBox.Show("This Treatment Plan has already been signed.");
				return;
			}
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll();
			MobileAppDevice device=listMobileAppDevices.FirstOrDefault(x => x.PatNum==PatCur.PatNum);
			if(device!=null && device.LastCheckInActivity>DateTime.Now.AddHours(-1)) {
				PushSelectedTpToEclipboard(device);
			}
			else {
				OpenUnlockCodeForTP();
			}
		}

		///<summary>Sends the currently selected TreatmentPlan in gridPlans to a given target mobile device.
		///If no selection is currently made then this simply returns.
		///Shows a MsgBox when done or if error occurs.</summary>
		private void PushSelectedTpToEclipboard(MobileAppDevice mobileAppDevice){
			if(gridPlans.SelectedIndices.Count()==0){
				return;//document wont be null below.
			}
			using(PdfDocument document=GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig)) {//Cant be null due to above check.
				if(PushNotificationUtils.CI_SendTreatmentPlan(document,treatPlan,hasPracticeSig,mobileAppDevice.MobileAppDeviceNum
					,out string errorMsg,out long mobileDataByeNum)) 
				{
					MsgBox.Show($"Treatment Plan sent to device: {mobileAppDevice.DeviceName}");
				}
				else {//Error occured
					MsgBox.Show($"Error sending Treatment Plan: {errorMsg}");
				}
			}
		}

		///<summary>Opens a FormMobileCode window with the currently selected TP.</summary>
		private void OpenUnlockCodeForTP(){
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				using(PdfDocument doc=GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig)) {
					if(MobileDataBytes.TryInsertTreatPlanPDF(doc,treatPlan,hasPracticeSig,unlockCode,out string errorMsg,out MobileDataByte mobileDataByte)){
						return mobileDataByte;
					}
					MsgBox.Show(errorMsg);
				}
				return null;
			}
			using(FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode)) {
				formMobileCode.ShowDialog();
			}
		}

		///<summary>Returns a PDF for the currently selected TreatmentPlan and sets out TreatmentPlan to selected TreatmentPlan.
		///If nothing is selected in gridPlans then returns null and out TreatPlan is set to null.</summary>
		private PdfDocument GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig){
			treatPlan=null;
			hasPracticeSig=false;
			PdfDocument document=null;
			if(gridPlans.SelectedIndices.Count()==1) {
				Action actionCloseProgress=ODProgress.Show(); //Immediately shows a progress window.
				//The following logic mimics ToolBarMainEmail_Click()
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
				if(DoPrintUsingSheets()) {
					if(treatPlan.TPStatus==TreatPlanStatus.Saved) {
						treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
					}
					else {
						LoadActiveTP(ref treatPlan);
					}
					Sheet sheet=TreatPlanToSheet(treatPlan);
					hasPracticeSig=sheet.SheetFields.Any(x => x.FieldType==SheetFieldType.SigBoxPractice);
					document=SheetPrinting.CreatePdf(sheet,"",null,null,null,null,null,false);
				}
				else {//generate and save a new document from scratch
					PrepImageForPrinting();
					PdfDocumentRenderer pdfRenderer=new PdfDocumentRenderer(true,PdfFontEmbedding.Always);
					pdfRenderer.Document=CreateDocument();
					pdfRenderer.RenderDocument();
					document=pdfRenderer.PdfDocument;
				}
				actionCloseProgress();//Closes the progress window. 
			}
			return document;
		}

		private void listSetPr_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			int clickedRow=listSetPr.IndexFromPoint(e.Location);
			if(!clickedRow.Between(0,listSetPr.Items.Count-1)) {
				return;
			}
			Def selectedPriority=(Def)listSetPr.Items.GetObjectAt(clickedRow);
			if(selectedPriority==null) {
				return;
			}
			TreatPlan selectedTp=gridPlans.SelectedIndices.Where(x => x>-1 && x<gridPlans.ListGridRows.Count)
				.Select(x => (TreatPlan)gridPlans.ListGridRows[x].Tag).FirstOrDefault();
			if(selectedTp==null) {
				return;
			}
			SetPriority(selectedPriority,selectedTp,gridMain.SelectedTags<ProcTP>());
			listSetPr.ClearSelected();
		}

		///<summary>Sets the priorities for the selected ProcTP.</summary>
		private void SetPriority(Def selectedPriority,TreatPlan selectedTp,List<ProcTP> listSelectedProcTps) {
			List<long> listSelectedProcNums=listSelectedProcTps
				.Where(x => x!=null)
				.Select(x => x.ProcNumOrig)
				.ToList();
			if(_listTreatPlans.Count>0
				 && (selectedTp.TPStatus==TreatPlanStatus.Active || selectedTp.TPStatus==TreatPlanStatus.Inactive)) 
			{
				TreatPlanAttaches.SetPriorityForTreatPlanProcs(selectedPriority.DefNum,selectedTp.TreatPlanNum,listSelectedProcNums);
			}
			else { //any Saved TP
				if(!Security.IsAuthorized(Permissions.TreatPlanEdit,selectedTp.DateTP)) {
					return;
				}
				ProcTPs.SetPriorityForTreatPlanProcs(selectedPriority.DefNum,selectedTp.TreatPlanNum,listSelectedProcNums);
			}
			ModuleSelected(PatCur.PatNum);//Refresh the entire module in order to get the new priorities from the database.
			//Reselect the treatment plan that the user was just looking at.
			gridPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(x => x.TreatPlanNum==selectedTp.TreatPlanNum)),true);
			//Refresh the main grid if a saved TP is selected.
			if(gridPlans.GetSelectedIndex() > 0) {
				FillMain();//Refresh the Procedures grid with the newly selected treatment plan.
			}
			//Reselect any procedures that were selected prior to setting the priority.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag!=null && listSelectedProcNums.Contains(((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig)) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void checkShowMaxDed_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void checkShowFees_Click(object sender,EventArgs e) {
			if(checkShowFees.Checked){
				//checkShowStandard.Checked=true;
				if(!checkShowInsNotAutomatic){
					checkShowIns.Checked=true;
				}
				if(!checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
				checkShowSubtotals.Checked=true;
				checkShowTotals.Checked=true;
			}
			else{
				//checkShowStandard.Checked=false;
				if(!checkShowInsNotAutomatic){
					checkShowIns.Checked=false;
				}
				if(!checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=false;
				}
				checkShowSubtotals.Checked=false;
				checkShowTotals.Checked=false;
			}
			FillMain();
		}

		private void checkShowStandard_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void checkShowIns_Click(object sender,EventArgs e) {
			if(!checkShowIns.Checked && !checkShowInsNotAutomatic) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Turn off automatic checking of this box for Active/Inactive Treatment Plans for the rest of this session?")) {
					checkShowInsNotAutomatic=true;
				}
			}
			FillMain();
		}

		private void checkShowDiscount_Click(object sender,EventArgs e) {
			if(!checkShowDiscount.Checked && !checkShowDiscountNotAutomatic) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Turn off automatic checking of this box for Active/Inactive Treatment Plans for the rest of this session?")) {
					checkShowDiscountNotAutomatic=true;
				}
			}
			FillMain();
		}

		private void checkShowSubtotals_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void checkShowTotals_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void ToolBarMainPrint_Click() {
			if(gridPlans.SelectedIndices.Length < 1) {
				MsgBox.Show(this,"Select a Treatment Plan to print.");
				return;
			}
			#region FuchsOptionOn
			if(PrefC.GetBool(PrefName.FuchsOptionsOn)) {
				if(checkShowDiscount.Checked || checkShowIns.Checked) {
					if(MessageBox.Show(this,string.Format(Lan.g(this,"Do you want to remove insurance estimates and discounts from printed treatment plan?")),"Open Dental",MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.No) {
						checkShowDiscount.Checked=false;
						checkShowIns.Checked=false;
						FillMain();
					}
				}
			}
			#endregion
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved
				&& PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)
			  && _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!=""
			  && Documents.DocExists(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum)) 
			{
				//Open PDF and allow user to print from pdf software.
				Cursor=Cursors.WaitCursor;
				Documents.OpenDoc(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum);
				Cursor=Cursors.Default;
				return;
			}
			Sheet sheetTP=null;
			TreatPlan treatPlan;
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved) {
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
				treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
			}
			else {
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
				LoadActiveTP(ref treatPlan);
			}
			if(DoPrintUsingSheets()) {
				sheetTP=TreatPlanToSheet(treatPlan);
				SheetPrinting.Print(sheetTP);
			}
			else { //clasic TPs
				PrepImageForPrinting();
				MigraDoc.DocumentObjectModel.Document doc=CreateDocument();
				MigraDoc.Rendering.Printing.MigraDocPrintDocument printdoc=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
				MigraDoc.Rendering.DocumentRenderer renderer=new MigraDoc.Rendering.DocumentRenderer(doc);
				renderer.PrepareDocument();
				printdoc.Renderer=renderer;
				//we might want to surround some of this with a try-catch
				//TODO: Implement ODprintout pattern - MigraDoc
				if(ODBuild.IsDebug()) {
					pView=new FormRpPrintPreview(printdoc);
					pView.ShowDialog();
				}
				else {
					if(PrinterL.SetPrinter(pd2,PrintSituation.TPPerio,PatCur.PatNum,"Treatment plan for printed")){
						printdoc.PrinterSettings=pd2.PrinterSettings;
						printdoc.Print();
					}
				}
			}
			SaveTPAsDocument(false,sheetTP);
		}

		private void ToolBarMainEmail_Click() {
			if(!Security.IsAuthorized(Permissions.EmailSend)) {
				return;
			}
			#region FuchsOptionOn
			if(PrefC.GetBool(PrefName.FuchsOptionsOn)) {
				if(checkShowDiscount.Checked || checkShowIns.Checked) {
					if(MessageBox.Show(this,string.Format(Lan.g(this,"Do you want to remove insurance estimates and discounts from e-mailed treatment plan?")),"Open Dental",MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.No) {
						checkShowDiscount.Checked=false;
						checkShowIns.Checked=false;
						FillMain();
					}
				}
			}
			#endregion
			PrepImageForPrinting();
			string attachPath=EmailAttaches.GetAttachPath();
			Random rnd=new Random();
			string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()+".pdf";
			string filePathAndName=FileAtoZ.CombinePaths(attachPath,fileName);
			if(CloudStorage.IsCloudStorage) {
				filePathAndName=PrefC.GetRandomTempFile("pdf");//Save the pdf to a temp file and then upload it the Email Attachment folder later.
			}
			if(gridPlans.SelectedIndices[0]>0 //not the default plan.
				&& PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf) //preference enabled
			  && _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="" //and document is signed
			  && Documents.DocExists(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum)) //and file exists
			{
				string filePathAndNameTemp=Documents.GetPath(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum);
				//copy file to email attach folder so files will be where they are exptected to be.
				File.Delete(filePathAndName);
				File.Copy(filePathAndNameTemp,filePathAndName);
			}
			else if(DoPrintUsingSheets())	{
				TreatPlan treatPlan;
				if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved) {
					treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
					treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
				}
				else {
					treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
					LoadActiveTP(ref treatPlan);
				}
				Sheet sheetTP=TreatPlanToSheet(treatPlan);
				SheetPrinting.CreatePdf(sheetTP,filePathAndName,null);
			}
			else{//generate and save a new document from scratch
				MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(true,PdfFontEmbedding.Always);
				pdfRenderer.Document=CreateDocument();
				pdfRenderer.RenderDocument();
				pdfRenderer.PdfDocument.Save(filePathAndName);
			}
			//Process.Start(filePathAndName);
			if(CloudStorage.IsCloudStorage) {
				FileAtoZ.Copy(filePathAndName,FileAtoZ.CombinePaths(attachPath,fileName),FileAtoZSourceDestination.LocalToAtoZ);
			}
			EmailMessage message=new EmailMessage();
			message.PatNum=PatCur.PatNum;
			message.ToAddress=PatCur.Email;
			EmailAddress address=EmailAddresses.GetByClinic(PatCur.ClinicNum);
			message.FromAddress=address.GetFrom();
			message.Subject=Lan.g(this,"Treatment Plan");
			EmailAttach attach=new EmailAttach();
			attach.DisplayedFileName="TreatmentPlan.pdf";
			attach.ActualFileName=fileName;
			message.Attachments.Add(attach);
			message.MsgType=EmailMessageSource.TreatmentPlan;
			using FormEmailMessageEdit FormE=new FormEmailMessageEdit(message,address);
			FormE.IsNew=true;
			FormE.ShowDialog();
			//if(FormE.DialogResult==DialogResult.OK) {
			//	RefreshCurrentModule();
			//}
		}

		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			FormSheetFillEdit formSFE=((FormSheetFillEdit)sender);
			if(formSFE.DialogResult==DialogResult.OK && PatCur!=null) {
				//If the user deleted the sheet, forcefully refresh the chart module regardless of what patient is selected.
				//Otherwise; only refresh the chart module if the same patient is selected.
				if(formSFE.SheetCur==null || formSFE.SheetCur.PatNum==PatCur.PatNum) {
					ModuleSelected(PatCur.PatNum);
				}
			}
		}

		private void ToolBarConsent_Click() {
			if(PatCur==null) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
			if(listSheetDefs.Count>0) {
				MsgBox.Show(this,"Please use dropdown list.");
				return;
			}
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.Consent);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatCur.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",PatCur.PatNum);
			StaticTextData data=new StaticTextData();
			List<GridRow> listProcTPRows=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();//Only pull selected rows that are procedures
			if(listProcTPRows.Count()>0) {//loop through selected procedures
				StaticTextFieldDependency dependencies=StaticTextData.GetStaticTextDependencies(sheet.SheetFields);
				List<long> listProcedureNums=listProcTPRows.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcs=Procedures.GetManyProc(listProcedureNums,false);//get list of procedures from list of procedureNums
				List<long> listProcCodeNums=listProcs.Select(x => x.CodeNum).ToList();
				if(listProcCodeNums.Count()>0) {
					dependencies|=StaticTextFieldDependency.ListSelectedTpProcs;
				}
				data=StaticTextData.GetStaticTextData(dependencies,PatCur,FamCur,listProcCodeNums);//set static text using selected procedure CodeNums
			}
			SheetFiller.FillFields(sheet,staticTextData: data);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void ToolBarLabCase_Click() {
			if(PatCur==null) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			LabCase labCase=new LabCase();
			labCase.PatNum=PatCur.PatNum;
			labCase.ProvNum=Patients.GetProvNum(PatCur);
			labCase.DateTimeCreated=MiscData.GetNowDateTime();
			LabCases.Insert(labCase);//it will be deleted inside the form if user clicks cancel.
									 //We need the primary key in order to attach lab slip.
			List<long> listProcCodeNums=new List<long>();
			List<GridRow> listProcTPRows=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();
			if(listProcTPRows.Count()>0) {//loop through selected procedures
				List<long> listProcedureNums=listProcTPRows.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcs=Procedures.GetManyProc(listProcedureNums,false);//get list of procedures from list of procedureNums
				listProcCodeNums=listProcs.Select(x => x.CodeNum).ToList();
			}
			using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
			formLabCaseEdit.CaseCur=labCase;
			formLabCaseEdit.ListProcCodeNums=listProcCodeNums;//set list of procedure CodeNums in FormLabCaseEdit
			formLabCaseEdit.IsNew=true;
			formLabCaseEdit.ShowDialog();
			//needs to always refresh due to complex ok/cancel
			ModuleSelected(PatCur.PatNum);
		}

		private void PrepImageForPrinting(){
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)){
				return;
			}
			toothChartWrapper=new SparksToothChart.ToothChartWrapper();
			_toothChartRelay= new ToothChartRelay(false);
			_toothChartRelay.SetToothChartWrapper(toothChartWrapper);
			if(ToothChartRelay.IsSparks3DPresent){
				toothChart=_toothChartRelay.GetToothChart();
				toothChart.Location=new Point(0,0);
				toothChart.Size=new Size(500,370);
				toothChart.Visible=true;
				//this.Controls.Add(toothChart);
				//toothChart.BringToFront();
			}
			else{
				toothChartWrapper.Size=new Size(500,370);
				toothChartWrapper.UseHardware=ComputerPrefs.LocalComputer.GraphicsUseHardware;
				toothChartWrapper.PreferredPixelFormatNumber=ComputerPrefs.LocalComputer.PreferredPixelFormatNum;
				toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
				//Must be last setting set for preferences, because this is the line where the device pixel format is recreated.
				//The preferred pixel format number changes to the selected pixel format number after a context is chosen.
				toothChartWrapper.DrawMode=ComputerPrefs.LocalComputer.GraphicsSimple;
				ComputerPrefs.LocalComputer.PreferredPixelFormatNum=toothChartWrapper.PreferredPixelFormatNumber;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
				this.Controls.Add(toothChartWrapper);
				toothChartWrapper.BringToFront();
			}
			_toothChartRelay.SetToothNumberingNomenclature((ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			_toothChartRelay.ColorBackgroundMain=listDefs[14].ItemColor;
			_toothChartRelay.ColorText=listDefs[15].ItemColor;
			_toothChartRelay.ResetTeeth();
			ToothInitialList=ToothInitials.Refresh(PatCur.PatNum);
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<ToothInitialList.Count;i++) {
				if(ToothInitialList[i].InitialType==ToothInitialType.Primary) {
					_toothChartRelay.SetPrimary(ToothInitialList[i].ToothNum);
				}
			}
			for(int i=0;i<ToothInitialList.Count;i++) {
				switch(ToothInitialList[i].InitialType) {
					case ToothInitialType.Missing:
						_toothChartRelay.SetMissing(ToothInitialList[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						_toothChartRelay.SetHidden(ToothInitialList[i].ToothNum);
						break;
					case ToothInitialType.Rotate:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,ToothInitialList[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,ToothInitialList[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,ToothInitialList[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,ToothInitialList[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,ToothInitialList[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						_toothChartRelay.MoveTooth(ToothInitialList[i].ToothNum,0,0,0,0,0,ToothInitialList[i].Movement);
						break;
					case ToothInitialType.Drawing:
						_toothChartRelay.AddDrawingSegment(ToothInitialList[i].Copy());
						break;
					case ToothInitialType.Text:
						_toothChartRelay.AddText(ToothInitialList[i].GetTextString(), ToothInitialList[i].GetTextPoint(), ToothInitialList[i].ColorDraw, ToothInitialList[i].ToothInitialNum);
						break;
				}
			}
			ComputeProcListFiltered();
			DrawProcsGraphics();
			if(!ToothChartRelay.IsSparks3DPresent){
				toothChartWrapper.AutoFinish=true;
			}
			_toothChartRelay.EndUpdate();
			_bitmapToothChart=_toothChartRelay.GetBitmap();
			if(ToothChartRelay.IsSparks3DPresent){
				Controls.Remove(toothChart);
			}
			else{
				Controls.Remove(toothChartWrapper);
			}
			_toothChartRelay.DisposeControl();
		}

		///<summary>Gets the active treatment plan as a list of ProcTP.  
		///Uses the static variable 'PrefC.IsTreatPlanSortByTooth' to determine if procedures should be sorted by tooth order.</summary>
		private List<ProcTP> LoadActiveTP(ref TreatPlan treatPlan) {
			_loadActiveData=TreatmentPlanModules.GetLoadActiveTpData(PatCur,treatPlan.TreatPlanNum,BenefitList,PatPlanList,
				InsPlanList,dateTimeTP.Value,SubList,PrefC.GetBool(PrefName.InsChecksFrequency),PrefC.IsTreatPlanSortByTooth,_listSubstLinks);
			List<TreatPlanAttach> listTreatPlanAttaches=_loadActiveData.ListTreatPlanAttaches;
			List<Procedure> listProcForTP=_loadActiveData.listProcForTP;
			List<Appointment> listAppts=Appointments.GetAppointmentsForProcs(_listProcs);
			if(listProcForTP.Any(x => ProcedureCodes.GetWhereFromList(y => y.CodeNum==x.CodeNum).Count==0)) {
				MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
				return new List<ProcTP>();//Show an empty TP
			}
			Lookup<FeeKey2,Fee> lookupFees=null;
			if(_loadActiveData.ListFees!=null){
				lookupFees=(Lookup<FeeKey2,Fee>)_loadActiveData.ListFees.ToLookup(x => new FeeKey2(x.CodeNum,x.FeeSched));
			}
			InsPlan priPlanCur=null;
			if(PatPlanList.Count>0) { //primary
				InsSub priSubCur=InsSubs.GetSub(PatPlanList[0].InsSubNum,SubList);
				priPlanCur=InsPlans.GetPlan(priSubCur.PlanNum,InsPlanList);
			}
			InsPlan secPlanCur=null;
			if(PatPlanList.Count>1) { //secondary
				InsSub secSubCur=InsSubs.GetSub(PatPlanList[1].InsSubNum,SubList);
				secPlanCur=InsPlans.GetPlan(secSubCur.PlanNum,InsPlanList);
			}
			ClaimProcList=_loadActiveData.ClaimProcList;
			List<ClaimProc> claimProcListOld=ClaimProcList.Select(x => x.Copy()).ToList();
			LoopList=new List<ClaimProcHist>();
			//foreach(Procedure tpProc in listProcForTP){
			//Get data for any OrthoCases that may be linked to procs in listProcForTP
			List<OrthoProcLink> listOrthoProcLinksAll=new List<OrthoProcLink>();
			Dictionary<long,OrthoProcLink> dictOrthoProcLinksForProcList=new Dictionary<long,OrthoProcLink>();
			Dictionary<long,OrthoCase> dictOrthoCases=new Dictionary<long,OrthoCase>();
			Dictionary<long,OrthoSchedule> dictOrthoSchedules=new Dictionary<long,OrthoSchedule>();
			OrthoCases.GetDataForListProcs(ref listOrthoProcLinksAll,ref dictOrthoProcLinksForProcList,ref dictOrthoCases,ref dictOrthoSchedules,_listProcs);
			OrthoProcLink orthoProcLink=null;
			OrthoCase orthoCase=null;
			OrthoSchedule orthoSchedule=null;
			List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
			if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
				//Taking into account insurance frequency, use the date picker date when loading or when the Refresh button is pressed.  Defaults to today.
				HistList=_loadActiveData.HistList??ClaimProcs.GetHistList(PatCur.PatNum,BenefitList,PatPlanList,InsPlanList,-1,dateTimeTP.Value,SubList);
				for(int i=0;i<listProcForTP.Count;i++) {
					listProcForTP[i].ProcDate=dateTimeTP.Value;
					if(listProcForTP[i].ProcNumLab!=0){
							//Lab fees will be calculated and added to looplist when its parent is calculated.
							continue;
					}
					OrthoCases.FillOrthoCaseObjectsForProc(listProcForTP[i].ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,
						ref listOrthoProcLinksForOrthoCase,dictOrthoProcLinksForProcList,dictOrthoCases,dictOrthoSchedules,listOrthoProcLinksAll);
					Procedures.ComputeEstimates(listProcForTP[i],PatCur.PatNum,ref ClaimProcList,false,InsPlanList,PatPlanList,BenefitList,
						HistList,LoopList,false,
						PatCur.Age,SubList,
						null,false,true,_listSubstLinks,false,
						_loadActiveData.ListFees,lookupFees,
						orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase);
					//then, add this information to loopList so that the next procedure is aware of it.
					LoopList.AddRange(ClaimProcs.GetHistForProc(ClaimProcList,listProcForTP[i].ProcNum,listProcForTP[i].CodeNum));
				}
				SyncCanadianLabs(ClaimProcList,listProcForTP);
				//We don't want to save the claimprocs if it's a date other than DateTime.Today, since they are calculated using modified date information.
				//Only save to db if this is the active TP.  Inactive TPs should not change what is stored in the db, only what is displayed in the grid.
				if(dateTimeTP.Value==DateTime.Today && treatPlan.TPStatus==TreatPlanStatus.Active) {
					ClaimProcs.Synch(ref ClaimProcList,claimProcListOld);
				}
			}
			else { 
				for(int i=0;i<listProcForTP.Count;i++) {
					if(listProcForTP[i].ProcNumLab!=0){
							//Lab fees will be calculated and added to looplist when its parent is calculated.
							continue;
					}
					OrthoCases.FillOrthoCaseObjectsForProc(listProcForTP[i].ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,
						ref listOrthoProcLinksForOrthoCase,dictOrthoProcLinksForProcList,dictOrthoCases,dictOrthoSchedules,listOrthoProcLinksAll);
					Procedures.ComputeEstimates(listProcForTP[i],PatCur.PatNum,ref ClaimProcList,false,InsPlanList,PatPlanList,BenefitList,HistList,LoopList
						,false,PatCur.Age,SubList,
						listSubstLinks:_listSubstLinks,lookupFees:lookupFees,
						orthoProcLink:orthoProcLink,orthoCase:orthoCase,orthoSchedule:orthoSchedule,listOrthoProcLinksForOrthoCase:listOrthoProcLinksForOrthoCase);
					//then, add this information to loopList so that the next procedure is aware of it.
					LoopList.AddRange(ClaimProcs.GetHistForProc(ClaimProcList,listProcForTP[i].ProcNum,listProcForTP[i].CodeNum));
				}
				SyncCanadianLabs(ClaimProcList,listProcForTP);
				//Only save to db if this is the active TP.  Inactive TPs should not change what is stored in the db, only what is displayed in the grid.
				if(treatPlan.TPStatus==TreatPlanStatus.Active) {
					ClaimProcs.Synch(ref ClaimProcList,claimProcListOld);
				}
			}
			//claimProcList=ClaimProcs.RefreshForTP(PatCur.PatNum);
			string estimateNote;
			decimal subfee,suballowed,totFee,priIns,secIns,subpriIns,allowed,totPriIns,subsecIns,totSecIns,subdiscount,totDiscount,subpat,totPat,totAllowed
				,taxAmt,subTaxAmt,totTaxAmt,subCatPercUCR,totCatPercUCR;
			subfee=suballowed=totFee=priIns=secIns=subpriIns=allowed=totPriIns=subsecIns=totSecIns=subdiscount=totDiscount=subpat=totPat=totAllowed=
				taxAmt=subTaxAmt=totTaxAmt=subCatPercUCR=totCatPercUCR=0;
			RowsMain.Clear();
			List<DisplayField> listFields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			bool doShowDiscountForCatPercent=listFields.Any(x => x.InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR)
				&& listFields.Any(x => x.InternalName==DisplayFields.InternalNames.TreatmentPlanModule.Fee);
			InsPlan insPlanPrimary=null;
			if(PatPlanList.Count>0) {
				InsSub insSubPrimary=InsSubs.GetSub(PatPlanList[0].InsSubNum,SubList);
				insPlanPrimary=InsPlans.GetPlan(insSubPrimary.PlanNum,InsPlanList);
			}
			List<ProcTP> retVal=new List<ProcTP>();
			DiscountPlan discountPlan=null;
			List<DiscountPlanProc> listDiscountPlanProcs=new List<DiscountPlanProc>();
			if(_discountPlanSub!=null && !listProcForTP.IsNullOrEmpty()) {
				listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(listProcForTP,discountPlanSub:_discountPlanSub,discountPlan:_tpData.DiscountPlan);
			}
			for(int i=0;i<listProcForTP.Count;i++) {
				ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(listProcForTP[i].CodeNum);
				TpRow row=new TpRow();
				row.ProcAbbr=procCodeCur.AbbrDesc;
				row.ProvNum=listProcForTP[i].ProvNum;
				row.DateTP=listProcForTP[i].DateTP;
				row.ClinicNum=listProcForTP[i].ClinicNum;
				ApptStatus statusCur=ApptStatus.None;
				Appointment apt=listAppts.FirstOrDefault(x => x.AptNum==listProcForTP[i].AptNum);
				if(apt!=null) {
					statusCur=apt.AptStatus;
				}
				row.Appt=FillApptCellForRow(listProcForTP[i].PlannedAptNum,listProcForTP[i].AptNum, statusCur);
				//Passing in empty lists to simulate what the fee would be if the patient did not have any insurance.
				row.CatPercUCR=(decimal)Procedures.GetProcFee(PatCur,new List<PatPlan>(),new List<InsSub>(),new List<InsPlan>(),listProcForTP[i].CodeNum,
					listProcForTP[i].ProvNum,listProcForTP[i].ClinicNum,listProcForTP[i].MedicalCode,listFees:_loadActiveData.ListFees);
				decimal fee=(decimal)listProcForTP[i].ProcFeeTotal;
				
				subfee+=fee;
				totFee+=fee;
				#region ShowMaxDed
				string showPriDeduct="";
				string showSecDeduct="";
				ClaimProc claimproc; //holds the estimate.
				DiscountPlanProc discountPlanProc=null;
				priIns=0; //We need to clear out priIns in this loop, it's not reset anywhere.
				if(_discountPlanSub!=null) {
					discountPlanProc=listDiscountPlanProcs.First(x=>x.ProcNum==listProcForTP[i].ProcNum);
					priIns=(decimal)discountPlanProc.DiscountPlanAmt;
				}
				if(PatPlanList.Count>0) { //Primary
					claimproc=ClaimProcs.GetEstimate(ClaimProcList,listProcForTP[i].ProcNum,priPlanCur.PlanNum,PatPlanList[0].InsSubNum);
					if(claimproc==null || claimproc.EstimateNote.Contains("Frequency Limitation")) {
						if(claimproc!=null && claimproc.InsEstTotalOverride!=-1) {
							priIns=(decimal)claimproc.InsEstTotalOverride;
						}
						else { 
							priIns=0;
						}
					}
					else {
						if(checkShowMaxDed.Checked) { //whether visible or not
							priIns=(decimal)ClaimProcs.GetInsEstTotal(claimproc);
							double ded=ClaimProcs.GetDeductibleDisplay(claimproc);
							if(ded>0) {
								showPriDeduct="\r\n"+Lan.g(this,"Pri Deduct Applied: ")+ded.ToString("c");
							}
						}
						else {
							priIns=(decimal)claimproc.BaseEst;
						}
					}
					if((claimproc!=null && claimproc.NoBillIns) || (claimproc==null && procCodeCur.NoBillIns)) {
						allowed=-1;
					}
					else {
						allowed=ComputeAllowedAmount(listProcForTP[i],claimproc,this._listSubstLinks,lookupFees,_loadActiveData.BlueBookEstimateData);
					}
				}
				if(PatPlanList.Count>1) { //Secondary
					claimproc=ClaimProcs.GetEstimate(ClaimProcList,listProcForTP[i].ProcNum,secPlanCur.PlanNum,PatPlanList[1].InsSubNum);
					if(claimproc==null) {
						secIns=0;
					}
					else {
						if(checkShowMaxDed.Checked) {
							secIns=(decimal)ClaimProcs.GetInsEstTotal(claimproc);
							decimal ded=(decimal)ClaimProcs.GetDeductibleDisplay(claimproc);
							if(ded>0) {
								showSecDeduct="\r\n"+Lan.g(this,"Sec Deduct Applied: ")+ded.ToString("c");
							}
						}
						else {
							secIns=(decimal)claimproc.BaseEst;
						}
					}
				} //secondary
				else { //no secondary ins
					secIns=0;
				}
				#endregion ShowMaxDed
				subpriIns+=priIns;
				totPriIns+=priIns;
				subsecIns+=secIns;
				totSecIns+=secIns;
				if(CompareDecimal.IsGreaterThan(allowed,-1)) {//-1 means the proc is DoNotBillIns
					suballowed+=allowed;
					totAllowed+=allowed;
				}
				taxAmt=(decimal)listProcForTP[i].TaxAmt;
				subTaxAmt+=taxAmt;
				totTaxAmt+=taxAmt;
				subCatPercUCR+=row.CatPercUCR;
				totCatPercUCR+=row.CatPercUCR;
				decimal discount=(decimal)ClaimProcs.GetTotalWriteOffEstimateDisplay(ClaimProcList,listProcForTP[i].ProcNum);
				decimal writeOffDiscount=discount;
				if(doShowDiscountForCatPercent && insPlanPrimary!=null && insPlanPrimary.PlanType=="") {//plan is category percentage
					discount+=Math.Max(row.CatPercUCR-fee,0);
				}
				decimal procDiscount=(decimal)listProcForTP[i].Discount;
				discount+=procDiscount;
				subdiscount+=discount;
				totDiscount+=discount;
				decimal pat=fee-priIns-secIns-writeOffDiscount-procDiscount+taxAmt;
				if(pat<0) {
					pat=0;
				}
				subpat+=pat;
				totPat+=pat;
				//Fill TpRow object with information.
				row.Priority=Defs.GetName(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProcForTP[i].ProcNum).Priority);//(Defs.GetName(DefCat.TxPriorities,listProcForTP[i].Priority));
				row.Tth=(Tooth.ToInternat(listProcForTP[i].ToothNum));
				if(ProcedureCodes.GetProcCode(listProcForTP[i].CodeNum).TreatArea==TreatmentArea.Surf) {
					row.Surf=(Tooth.SurfTidyFromDbToDisplay(listProcForTP[i].Surf,listProcForTP[i].ToothNum));
				}
				else if(ProcedureCodes.GetProcCode(listProcForTP[i].CodeNum).TreatArea==TreatmentArea.Sextant) {
					row.Surf=Tooth.GetSextant(listProcForTP[i].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
				}
				else {
					row.Surf=(listProcForTP[i].Surf); //I think this will properly allow UR, L, etc.
				}
				row.Code=procCodeCur.ProcCode;
				string descript=ProcedureCodes.GetLaymanTerm(listProcForTP[i].CodeNum);
				if(listProcForTP[i].ToothRange!="") {
					descript+=" #"+Tooth.FormatRangeForDisplay(listProcForTP[i].ToothRange);
				}
				if(checkShowMaxDed.Checked) {
					estimateNote=ClaimProcs.GetEstimateNotes(listProcForTP[i].ProcNum,ClaimProcList);
					if(estimateNote!="") {
						descript+="\r\n"+estimateNote;
					}
				}
				row.Description=(descript);
				if(listProcForTP[i].ProcNumLab!=0) {
					row.Description=$"^ ^ {descript}";
				}
				if(showPriDeduct!="") {
					row.Description+=showPriDeduct;
				}
				if(showSecDeduct!="") {
					row.Description+=showSecDeduct;
				}
				if(discountPlanProc!=null) {
					if(discountPlanProc.doesExceedFreqLimit) {
						row.Description+=" - "+Lan.g(this,"Frequency Limitation");
					}
					else if(discountPlanProc.doesExceedAnnualMax) {
						row.Description+=" - "+Lan.g(this,"Over Annual Max");
					}
				}
				row.Prognosis=Defs.GetName(DefCat.Prognosis,PIn.Long(listProcForTP[i].Prognosis.ToString()));
				row.Dx=Defs.GetValue(DefCat.Diagnosis,PIn.Long(listProcForTP[i].Dx.ToString()));
				row.Fee=fee;
				row.PriIns=priIns;
				row.SecIns=secIns;
				row.Discount=discount;
				row.Pat=pat;
				row.FeeAllowed=allowed;
				row.TaxEst=taxAmt;
				row.ColorText=Defs.GetColor(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==listProcForTP[i].ProcNum).Priority);
				if(row.ColorText==System.Drawing.Color.White) {
					row.ColorText=System.Drawing.Color.Black;
				}
				//row.Tag=listProcForTP[i].Copy();
				Procedure proc=listProcForTP[i].Copy();
				//procList.Add(proc);
				ProcTP procTP=new ProcTP();
				//procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatCur.PatNum;
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ItemOrder=i;
				procTP.Priority=listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==proc.ProcNum).Priority;//proc.Priority;
				procTP.ToothNumTP=Tooth.ToInternat(proc.ToothNum);
				if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);
				}
				else {
					procTP.Surf=proc.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				procTP.Descript=row.Description;
				procTP.FeeAmt=PIn.Double(row.Fee.ToString());
				procTP.PriInsAmt=PIn.Double(row.PriIns.ToString());
				procTP.SecInsAmt=PIn.Double(row.SecIns.ToString());
				procTP.Discount=PIn.Double(row.Discount.ToString());
				procTP.PatAmt=PIn.Double(row.Pat.ToString());
				procTP.Prognosis=row.Prognosis;
				procTP.Dx=row.Dx;
				procTP.ProcAbbr=row.ProcAbbr;
				procTP.FeeAllowed=PIn.Double(row.FeeAllowed.ToString());
				procTP.TaxAmt=PIn.Double(row.TaxEst.ToString());
				procTP.ProvNum=row.ProvNum;
				procTP.DateTP=PIn.Date(row.DateTP.ToString());
				procTP.ClinicNum=row.ClinicNum;
				procTP.CatPercUCR=(double)row.CatPercUCR;
				retVal.Add(procTP);
				procTP.TagOD=proc.ProcNumLab;//Used for selection logic. See gridMain_CellClick(...).
				row.Tag=procTP;
				RowsMain.Add(row);
				#region subtotal
				if(checkShowSubtotals.Checked &&
					 (i==listProcForTP.Count-1 || listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProcForTP[i+1].ProcNum).Priority!=procTP.Priority)) {
					row=new TpRow();
					row.Description=Lan.g("TableTP","Subtotal");
					row.Fee=subfee;
					row.PriIns=subpriIns;
					row.SecIns=subsecIns;
					row.Discount=subdiscount;
					row.Pat=subpat;
					row.FeeAllowed=suballowed;
					row.TaxEst=subTaxAmt;
					row.CatPercUCR=subCatPercUCR;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==listProcForTP[i].ProcNum).Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					row.Bold=true;
					row.ColorLborder=System.Drawing.Color.Black;
					RowsMain.Add(row);
					subfee=0;
					subpriIns=0;
					subsecIns=0;
					subdiscount=0;
					subpat=0;
					suballowed=0;
					subTaxAmt=0;
					subCatPercUCR=0;
				}
				#endregion subtotal
			}
			if(checkShowDiscount.Checked && CompareDecimal.IsZero(totDiscount)) { //mimics Saved TP logic
				checkShowDiscount.Checked=false;
			}
			textNote.Text=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			HasNoteChanged=false;
			if((_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved 
					&& _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="") 
				|| (_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Inactive 
					&& _listTreatPlans[gridPlans.SelectedIndices[0]].Heading==Lan.g(this,"Unassigned"))) 
			{
				textNote.ReadOnly=true;
			}
			else {
				textNote.ReadOnly=false;
			}
			#region Totals
			if(checkShowTotals.Checked) {
				TpRow row=new TpRow();
				row.Description=Lan.g("TableTP","Total");
				row.Fee=totFee;
				row.PriIns=totPriIns;
				row.SecIns=totSecIns;
				row.Discount=totDiscount;
				row.Pat=totPat;
				row.FeeAllowed=totAllowed;
				row.TaxEst=totTaxAmt;
				row.CatPercUCR=totCatPercUCR;
				row.Bold=true;
				row.ColorText=System.Drawing.Color.Black;
				RowsMain.Add(row);
			}
			#endregion Totals
			treatPlan.ListProcTPs=retVal;
			return retVal;
		}

		///<summary>Returns string that will occupy the procedure's 'Appt' cell in the main grid. 'X' if procedure is on scheduled appointment,
		///'P' if on an unscheduled planned appointment, or a blank string if on no appointments.</summary>
		private string FillApptCellForRow(long plannedAptNum,long aptNum, ApptStatus apptStatus) {
			string result="";
			if(plannedAptNum>0) {
				result="P";
			}
			if(aptNum>0) {
				if(apptStatus==ApptStatus.UnschedList) {
					result="U";
				}
				else {
					result="X";
				}
			}
			return result;
		}

		///<summary>Refreshes Canadian Lab Fee ClaimProcs in listClaimProcs from the Db, and adds any new Canadian Lab Fee ClaimProcs that were 
		///added to Db when computing estimates</summary>
		private static void SyncCanadianLabs(List<ClaimProc> listClaimProcs,List<Procedure> listProcTp) {
			//1. Get all lab Cp for lab proc nums from Db
			//2. Copy Db lab Cp to original listClaimProcs (Db updates in ClaimProcs.ComputeBaseEst())
			//3. Add any new lab Cp (Db inserts in ClaimProcs.ComputeBaseEst())
			List<long> listLabProcNums=listProcTp.Where(x => x.ProcNumLab!=0).Select(x => x.ProcNum).ToList();
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA") || listLabProcNums.Count==0 ){//Not Canada or no labs to consider.
				return;
			}
			List<long> listOrigLabClaimProcNums=listClaimProcs.Where(x => listLabProcNums.Contains(x.ProcNum)).Select(x => x.ClaimProcNum).ToList();
			//Contains CPs we want to refresh and any that were added. Only look at estimates and cap estimates, see ClaimProcs.RefreshForTP(...)
			List<ClaimProc> listDbLabClaimProcs=ClaimProcs.RefreshForProcs(listLabProcNums)
				.Where(x => ListTools.In(x.Status,ClaimProcStatus.Estimate,ClaimProcStatus.CapEstimate)).ToList();
			for(int i=0;i<listClaimProcs.Count;i++) {
				long claimProcNum=listClaimProcs[i].ClaimProcNum;
				if(!listOrigLabClaimProcNums.Contains(claimProcNum)) {
					continue;//listClaimProcs[i] is not associated to a lab
				}
				listClaimProcs[i]=ClaimProcs.GetFromList(listDbLabClaimProcs,claimProcNum);//Update listClaimProcs to reflect changed values.
			}
			//New estimates could have been added in ClaimProcs.CanadianLabBaseEstHelper(...).
			listClaimProcs.AddRange(listDbLabClaimProcs.Where(x => !listOrigLabClaimProcNums.Contains(x.ClaimProcNum)).ToList());
		}

		/// <summary>Returns in-memory TreatPlan representing the current treatplan. For displaying current treat-plan before saving it.</summary>
		private TreatPlan GetCurrentTPHelper() {
			TreatPlan retVal=new TreatPlan();
			retVal.Heading=Lan.g(this,"Proposed Treatment Plan");
			retVal.DateTP=DateTime.Today;
			retVal.PatNum=PatCur.PatNum;
			retVal.Note=PrefC.GetString(PrefName.TreatmentPlanNote);
			retVal.ListProcTPs=new List<ProcTP>();
			ProcTP procTP;
			Procedure proc;
			int itemNo=0;
			List<Procedure> procList=new List<Procedure>();
			if(gridMain.SelectedIndices.Length==0 || gridMain.SelectedIndices.All(x=>gridMain.ListGridRows[x].Tag==null)) {
				gridMain.SetAll(true);//either no rows selected, or only total rows selected.
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				if(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag==null) {
					//user must have highlighted a subtotal row.
					continue;
				}
				proc=(Procedure)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				procList.Add(proc);
				procTP=new ProcTP();
				//procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatCur.PatNum;
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ItemOrder=itemNo;
				procTP.Priority=proc.Priority;
				procTP.ToothNumTP=Tooth.ToInternat(proc.ToothNum);
				if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);
				}
				else {
					procTP.Surf=proc.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				procTP.Descript=RowsMain[gridMain.SelectedIndices[i]].Description;
				if(checkShowFees.Checked) {
					procTP.FeeAmt=PIn.Double(RowsMain[gridMain.SelectedIndices[i]].Fee.ToString());
				}
				if(checkShowIns.Checked) {
					procTP.PriInsAmt=PIn.Double(RowsMain[gridMain.SelectedIndices[i]].PriIns.ToString());
					procTP.SecInsAmt=PIn.Double(RowsMain[gridMain.SelectedIndices[i]].SecIns.ToString());
				}
				if(checkShowDiscount.Checked) {
					procTP.Discount=PIn.Double(RowsMain[gridMain.SelectedIndices[i]].Discount.ToString());
				}
				procTP.PatAmt=PIn.Double(RowsMain[gridMain.SelectedIndices[i]].Pat.ToString());
				procTP.Prognosis=RowsMain[gridMain.SelectedIndices[i]].Prognosis;
				procTP.Dx=RowsMain[gridMain.SelectedIndices[i]].Dx;
				retVal.ListProcTPs.Add(procTP);
				//ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;
			}
			return retVal;
		}

		///<summary>Simply creates a new sheet from a given treatment plan and adds parameters to the sheet based on which checkboxes are checked.</summary>
		private Sheet TreatPlanToSheet(TreatPlan treatPlan) {
			Sheet sheetTP=SheetUtil.CreateSheet(SheetDefs.GetSheetsDefault(SheetTypeEnum.TreatmentPlan,Clinics.ClinicNum),PatCur.PatNum);
			sheetTP.Parameters.Add(new SheetParameter(true,"TreatPlan") { ParamValue=treatPlan });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscountNotAutomatic") { ParamValue=checkShowDiscountNotAutomatic });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscount") { ParamValue=checkShowDiscount.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowMaxDed") { ParamValue=checkShowMaxDed.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowSubTotals") { ParamValue=checkShowSubtotals.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowTotals") { ParamValue=checkShowTotals.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowCompleted") { ParamValue=checkShowCompleted.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowFees") { ParamValue=checkShowFees.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowIns") { ParamValue=checkShowIns.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"toothChartImg") { ParamValue=SheetPrinting.GetToothChartHelper(PatCur.PatNum,checkShowCompleted.Checked,treatPlan) });
			//FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheetTP);
			SheetFiller.FillFields(sheetTP);
			SheetUtil.CalculateHeights(sheetTP);
			return sheetTP;
		}

		private MigraDoc.DocumentObjectModel.Document CreateDocument(){
			MigraDoc.DocumentObjectModel.Document doc= new MigraDoc.DocumentObjectModel.Document();
			doc.DefaultPageSetup.PageWidth=Unit.FromInch(8.5);
			doc.DefaultPageSetup.PageHeight=Unit.FromInch(11);
			doc.DefaultPageSetup.TopMargin=Unit.FromInch(.5);
			doc.DefaultPageSetup.LeftMargin=Unit.FromInch(.5);
			doc.DefaultPageSetup.RightMargin=Unit.FromInch(.5);
			MigraDoc.DocumentObjectModel.Section section=doc.AddSection();
			string text;
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(9,false);
			MigraDoc.DocumentObjectModel.Font nameFontx=MigraDocHelper.CreateFont(9,true);
			MigraDoc.DocumentObjectModel.Font totalFontx=MigraDocHelper.CreateFont(9,true);
			//Heading---------------------------------------------------------------------------------------------------------------
			#region printHeading
			Paragraph par=section.AddParagraph();
			ParagraphFormat parformat=new ParagraphFormat();
			parformat.Alignment=ParagraphAlignment.Center;
			parformat.Font=MigraDocHelper.CreateFont(10,true);
			par.Format=parformat;
			text=_listTreatPlans[gridPlans.SelectedIndices[0]].Heading;
			par.AddFormattedText(text,headingFont);
			par.AddLineBreak();
			if(PatCur.ClinicNum==0 || !PrefC.HasClinicsEnabled) {
				text=PrefC.GetString(PrefName.PracticeTitle);
				par.AddText(text);
				par.AddLineBreak();
				text=PrefC.GetString(PrefName.PracticePhone);
			}
			else {
				Clinic clinic=Clinics.GetClinic(PatCur.ClinicNum);
				text=clinic.Description;
				par.AddText(text);
				par.AddLineBreak();
				text=clinic.Phone;
			}
			if(text.Length==10 && Application.CurrentCulture.Name=="en-US") {
				text="("+text.Substring(0,3)+")"+text.Substring(3,3)+"-"+text.Substring(6);
			}
			par.AddText(text);
			par.AddLineBreak();
			text=PatCur.GetNameFLFormal()+", DOB "+PatCur.Birthdate.ToShortDateString();
			par.AddText(text);
			par.AddLineBreak();
			if(gridPlans.SelectedIndices[0]>0){//not the default plan
				if(_listTreatPlans[gridPlans.SelectedIndices[0]].ResponsParty!=0){
					text=Lan.g(this,"Responsible Party: ")
						+Patients.GetLim(_listTreatPlans[gridPlans.SelectedIndices[0]].ResponsParty).GetNameFL();
					par.AddText(text);
					par.AddLineBreak();
				}
			}
			if(new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {//Active/Inactive TP
				text=DateTime.Today.ToShortDateString();
			}
			else {
				text=_listTreatPlans[gridPlans.SelectedIndices[0]].DateTP.ToShortDateString();
			}
			par.AddText(text);
			#endregion
			//Graphics---------------------------------------------------------------------------------------------------------------
			#region PrintGraphics
			TextFrame frame;
			int widthDoc=MigraDocHelper.GetDocWidth();
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum))
			{	
				frame=MigraDocHelper.CreateContainer(section);
				MigraDocHelper.DrawString(frame,Lan.g(this,"Your")+"\r\n"+Lan.g(this,"Right"),bodyFontx,
					new RectangleF(widthDoc/2-_toothChartRelay.Width/2-50,_toothChartRelay.Height/2-10,50,100));
				MigraDocHelper.DrawBitmap(frame,_bitmapToothChart,widthDoc/2-_toothChartRelay.Width/2,0);
				MigraDocHelper.DrawString(frame,Lan.g(this,"Your")+"\r\n"+Lan.g(this,"Left"),bodyFontx,
					new RectangleF(widthDoc/2+_toothChartRelay.Width/2+17,_toothChartRelay.Height/2-10,50,100));
				if(checkShowCompleted.Checked) {
					List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
					float yPos=_toothChartRelay.Height+15;
					float xPos=225;
					MigraDocHelper.FillRectangle(frame,listDefs[3].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(frame,Lan.g(this,"Existing"),bodyFontx,xPos,yPos);
					Graphics g=this.CreateGraphics();//for measuring strings.
					xPos+=(int)g.MeasureString(Lan.g(this,"Existing"),bodyFont).Width+23;
					//The Complete work is actually a combination of EC and C. Usually same color.
					//But just in case they are different, this will show it.
					MigraDocHelper.FillRectangle(frame,listDefs[2].ItemColor,xPos,yPos,7,14);
					xPos+=7;
					MigraDocHelper.FillRectangle(frame,listDefs[1].ItemColor,xPos,yPos,7,14);
					xPos+=9;
					MigraDocHelper.DrawString(frame,Lan.g(this,"Complete"),bodyFontx,xPos,yPos);
					xPos+=(int)g.MeasureString(Lan.g(this,"Complete"),bodyFont).Width+23;
					MigraDocHelper.FillRectangle(frame,listDefs[4].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(frame,Lan.g(this,"Referred Out"),bodyFontx,xPos,yPos);
					xPos+=(int)g.MeasureString(Lan.g(this,"Referred Out"),bodyFont).Width+23;
					MigraDocHelper.FillRectangle(frame,listDefs[0].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(frame,Lan.g(this,"Treatment Planned"),bodyFontx,xPos,yPos);
					g.Dispose();
				}
			}	
			#endregion
			MigraDocHelper.InsertSpacer(section,10);
			if(!PrefC.GetBool(PrefName.TreatPlanItemized)) {
				FillGridPrint();
				MigraDocHelper.DrawGrid(section,gridPrint);
				gridPrint.Visible=false;
				FillMainDisplay();
			}
			else {
				MigraDocHelper.DrawGrid(section,gridMain);
			}
			//Print benefits----------------------------------------------------------------------------------------------------
			#region printBenefits
			if(checkShowIns.Checked) {
				GridOD gridFamIns=new GridOD();
				gridFamIns.TranslationName="";
				this.Controls.Add(gridFamIns);
				gridFamIns.BeginUpdate();
				gridFamIns.ListGridColumns.Clear();
				GridColumn col=new GridColumn("",140);
				gridFamIns.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Primary"),70,HorizontalAlignment.Right);
				gridFamIns.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Secondary"),70,HorizontalAlignment.Right);
				gridFamIns.ListGridColumns.Add(col);
				gridFamIns.ListGridRows.Clear();
				GridRow row;
				//Annual Family Max--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Family Maximum"));
				row.Cells.Add(userControlFamIns.GetFamPriMax());
				row.Cells.Add(userControlFamIns.GetFamSecMax());
				gridFamIns.ListGridRows.Add(row);
				//Family Deductible--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Family Deductible"));
				row.Cells.Add(userControlFamIns.GetFamPriDed());
				row.Cells.Add(userControlFamIns.GetFamSecDed());
				gridFamIns.ListGridRows.Add(row);
				//Print Family Insurance-----------------------
				MigraDocHelper.InsertSpacer(section,15);
				par=section.AddParagraph();
				par.Format.Alignment=ParagraphAlignment.Center;
				par.AddFormattedText(Lan.g(this,"Family Insurance Benefits"),totalFontx);
				MigraDocHelper.InsertSpacer(section,2);
				MigraDocHelper.DrawGrid(section,gridFamIns);
				gridFamIns.Dispose();
				//Individual Insurance---------------------
				GridOD gridIns=new GridOD();
				gridIns.TranslationName="";
				this.Controls.Add(gridIns);
				gridIns.BeginUpdate();
				gridIns.ListGridColumns.Clear();
				col=new GridColumn("",140);
				gridIns.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Primary"),70,HorizontalAlignment.Right);
				gridIns.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Secondary"),70,HorizontalAlignment.Right);
				gridIns.ListGridColumns.Add(col);
				gridIns.ListGridRows.Clear();
				//Annual Max--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Annual Maximum"));
				row.Cells.Add(userControlIndIns.GetPriMax());
				row.Cells.Add(userControlIndIns.GetSecMax());
				gridIns.ListGridRows.Add(row);
				//Deductible--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Deductible"));
				row.Cells.Add(userControlIndIns.GetPriDed());
				row.Cells.Add(userControlIndIns.GetSecDed());
				gridIns.ListGridRows.Add(row);
				//Deductible Remaining--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Deductible Remaining"));
				row.Cells.Add(userControlIndIns.GetPriDedRem());
				row.Cells.Add(userControlIndIns.GetSecDedRem());
				gridIns.ListGridRows.Add(row);
				//Insurance Used--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Insurance Used"));
				row.Cells.Add(userControlIndIns.GetPriUsed());
				row.Cells.Add(userControlIndIns.GetSecUsed());
				gridIns.ListGridRows.Add(row);
				//Pending--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Pending"));
				row.Cells.Add(userControlIndIns.GetPriPend());
				row.Cells.Add(userControlIndIns.GetSecPend());
				gridIns.ListGridRows.Add(row);
				//Remaining--------------------------
				row=new GridRow();
				row.Cells.Add(Lan.g(this,"Remaining"));
				row.Cells.Add(userControlIndIns.GetPriRem());
				row.Cells.Add(userControlIndIns.GetSecRem());
				gridIns.ListGridRows.Add(row);
				gridIns.EndUpdate();
				//Print Individual Insurance-------------------------
				MigraDocHelper.InsertSpacer(section,15);
				par=section.AddParagraph();
				par.Format.Alignment=ParagraphAlignment.Center;
				par.AddFormattedText(Lan.g(this,"Individual Insurance Benefits"),totalFontx);
				MigraDocHelper.InsertSpacer(section,2);
				MigraDocHelper.DrawGrid(section,gridIns);
				gridIns.Dispose();
			}
			#endregion
			//Note------------------------------------------------------------------------------------------------------------
			#region printNote
			string note="";
			if(gridPlans.SelectedIndices[0]==0) {//current TP
				note=PrefC.GetString(PrefName.TreatmentPlanNote);
			}
			else {
				note=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			}
			char nbsp='\u00A0';
			if(note!="") {
				//to prevent collapsing of multiple spaces to single spaces.  We only do double spaces to leave single spaces in place.
				note=note.Replace("  ",nbsp.ToString()+nbsp.ToString());
				MigraDocHelper.InsertSpacer(section,20);
				par=section.AddParagraph(note);
				par.Format.Font=bodyFontx;
				par.Format.Borders.Color=Colors.Gray;
				par.Format.Borders.DistanceFromLeft=Unit.FromInch(.05);
				par.Format.Borders.DistanceFromRight=Unit.FromInch(.05);
				par.Format.Borders.DistanceFromTop=Unit.FromInch(.05);
				par.Format.Borders.DistanceFromBottom=Unit.FromInch(.05);
			}
			#endregion
			//Signature-----------------------------------------------------------------------------------------------------------
			#region signature
			if(gridPlans.SelectedIndices[0]!=0//can't be default TP
				&& _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="")
			{
				TreatPlan treatPlan = _listTreatPlans[gridPlans.SelectedIndices[0]];
				List<ProcTP> proctpList = ProcTPs.RefreshForTP(_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum);
				System.Drawing.Bitmap sigBitmap = null;
				SignatureBoxWrapper sigBoxWrapper = new SignatureBoxWrapper();
				sigBoxWrapper.SignatureMode=SignatureBoxWrapper.SigMode.TreatPlan;
				string keyData = TreatPlans.GetKeyDataForSignatureHash(treatPlan,proctpList);
				sigBoxWrapper.FillSignature(treatPlan.SigIsTopaz,keyData,treatPlan.Signature);
				sigBitmap=sigBoxWrapper.GetSigImage();  //Previous tp code did not care if signature is valid or not.
				if(sigBitmap!=null) { 
					frame=MigraDocHelper.CreateContainer(section);
					MigraDocHelper.DrawBitmap(frame,sigBitmap,widthDoc/2-sigBitmap.Width/2,20);
				}
			}
			#endregion
			return doc;
		}

		///<summary>Just used for printing the 3D chart.</summary>
		private void ComputeProcListFiltered() {
			ProcListFiltered=new List<Procedure>();
			//first, add all completed work and conditions. C,EC,EO, and Referred
			for(int i=0;i<_listProcs.Count;i++) {
				if(_listProcs[i].ProcStatus==ProcStat.C
					|| _listProcs[i].ProcStatus==ProcStat.EC
					|| _listProcs[i].ProcStatus==ProcStat.EO) 
				{
					if(checkShowCompleted.Checked) {
						ProcListFiltered.Add(_listProcs[i]);
					}
				}
				if(_listProcs[i].ProcStatus==ProcStat.R) { //always show all referred
					ProcListFiltered.Add(_listProcs[i]);
				}
				if(_listProcs[i].ProcStatus==ProcStat.Cn) { //always show all conditions.
					ProcListFiltered.Add(_listProcs[i]);
				}
			}
			//then add whatever is showing on the selected TP
			//Always select all procedures in TP.
			gridMain.SetAll(true);
			if(new[] {TreatPlanStatus.Active,TreatPlanStatus.Inactive}.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) { //current plan
				ProcTPSelectList=gridMain.SelectedIndices.Where(x => gridMain.ListGridRows[x].Tag!=null).Select(x => (ProcTP)gridMain.ListGridRows[x].Tag).ToArray();
			}
			Procedure procDummy; //not a real procedure.  Just used to help display on graphical chart
			for(int i=0;i<ProcTPSelectList.Length;i++) {
				procDummy=new Procedure();
				//this next loop is a way to get missing fields like tooth range.  Could be improved.
				for(int j=0;j<_listProcs.Count;j++) {
					if(_listProcs[j].ProcNum==ProcTPSelectList[i].ProcNumOrig) {
						//but remember that even if the procedure is found, Status might have been altered
						procDummy=_listProcs[j].Copy();
					}
				}
				if(Tooth.IsValidEntry(ProcTPSelectList[i].ToothNumTP)) {
					procDummy.ToothNum=Tooth.FromInternat(ProcTPSelectList[i].ToothNumTP);
				}
				if(ProcedureCodes.GetProcCode(ProcTPSelectList[i].ProcCode).TreatArea==TreatmentArea.Surf) {
					procDummy.Surf=Tooth.SurfTidyFromDisplayToDb(ProcTPSelectList[i].Surf,procDummy.ToothNum);
				}
				else {
					procDummy.Surf=ProcTPSelectList[i].Surf; //for quad, arch, etc.
				}
				if(procDummy.ToothRange==null) {
					procDummy.ToothRange="";
				}
				//procDummy.HideGraphical??
				procDummy.ProcStatus=ProcStat.TP;
				procDummy.CodeNum=ProcedureCodes.GetProcCode(ProcTPSelectList[i].ProcCode).CodeNum;
				ProcListFiltered.Add(procDummy);
			}
			ProcListFiltered.Sort(CompareProcListFiltered);
		}

		private int CompareProcListFiltered(Procedure proc1,Procedure proc2) {
			int dateFilter=proc1.ProcDate.CompareTo(proc2.ProcDate);
			if(dateFilter!=0) {
				return dateFilter;
			}
			//Dates are the same, filter by ProcStatus.
			int xIdx=GetProcStatusIdx(proc1.ProcStatus);
			int yIdx=GetProcStatusIdx(proc2.ProcStatus);
			return xIdx.CompareTo(yIdx);
		}

		private decimal ComputeAllowedAmount(Procedure proc,ClaimProc claimProcCur,List<SubstitutionLink> listSubLinks,Lookup<FeeKey2,Fee> lookupFees
			,BlueBookEstimateData blueBookEstimateData)
		{
			//List<Fee> listFees) {
			decimal allowed=0;
			if(claimProcCur!=null) {				
				if(claimProcCur.AllowedOverride!=-1) {//check for allowed override
					allowed=(decimal)claimProcCur.AllowedOverride;
				}
				else {
					allowed=InsPlans.GetAllowedForProc(proc,claimProcCur,InsPlanList,listSubLinks,lookupFees,blueBookEstimateData);
					if(allowed==-1) {//Carrier does not have an allowed fee entered
						allowed=0;
					}
				}
			}
			return allowed;
		}

		///<summary>Returns index for sorting based on this order: Cn,TP,R,EO,EC,C,D</summary>
		private int GetProcStatusIdx(ProcStat procStat) {
			switch(procStat) {
				case ProcStat.Cn:
					return 0;
				case ProcStat.TPi:
				case ProcStat.TP:
					return 1;
				case ProcStat.R:
					return 2;
				case ProcStat.EO:
					return 3;
				case ProcStat.EC:
					return 4;
				case ProcStat.C:
					return 5;
				case ProcStat.D:
					return 6;
			}
			return 0;
		}

		private void DrawProcsGraphics() {
			Procedure proc;
			string[] teeth;
			System.Drawing.Color cLight=System.Drawing.Color.White;
			System.Drawing.Color cDark=System.Drawing.Color.White;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
			for(int i=0;i<ProcListFiltered.Count;i++) {
				proc=ProcListFiltered[i];
				//if(proc.ProcStatus!=procStat) {
				//  continue;
				//}
				if(proc.HideGraphics) {
					continue;
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum).PaintType==ToothPaintingType.Extraction && (
					proc.ProcStatus==ProcStat.C
					|| proc.ProcStatus==ProcStat.EC
					|| proc.ProcStatus==ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				if(ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor==System.Drawing.Color.FromArgb(0)) {
					switch(proc.ProcStatus) {
						case ProcStat.C:
							cDark=listDefs[1].ItemColor;
							cLight=listDefs[6].ItemColor;
							break;
						case ProcStat.TP:
							cDark=listDefs[0].ItemColor;
							cLight=listDefs[5].ItemColor;
							break;
						case ProcStat.EC:
							cDark=listDefs[2].ItemColor;
							cLight=listDefs[7].ItemColor;
							break;
						case ProcStat.EO:
							cDark=listDefs[3].ItemColor;
							cLight=listDefs[8].ItemColor;
							break;
						case ProcStat.R:
							cDark=listDefs[4].ItemColor;
							cLight=listDefs[9].ItemColor;
							break;
						case ProcStat.Cn:
							cDark=listDefs[16].ItemColor;
							cLight=listDefs[17].ItemColor;
							break;
						case ProcStat.D:  //Don't think this can ever happen, but skip anyway.
						default:
							continue;//Don't draw.
					}
				}
				else {
					cDark=ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor;
					cLight=ProcedureCodes.GetProcCode(proc.CodeNum).GraphicColor;
				}
				switch(ProcedureCodes.GetProcCode(proc.CodeNum).PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,proc.ToothNum)) {
							_toothChartRelay.SetPontic(proc.ToothNum,cDark);
						}
						else {
							_toothChartRelay.SetCrown(proc.ToothNum,cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,proc.ToothNum)) {
							_toothChartRelay.SetPontic(proc.ToothNum,cLight);
						}
						else {
							_toothChartRelay.SetCrown(proc.ToothNum,cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						_toothChartRelay.SetCrown(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.CrownLight:
						_toothChartRelay.SetCrown(proc.ToothNum,cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(proc.Surf=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(proc.Surf=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=proc.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,teeth[t])) {
								_toothChartRelay.SetPontic(teeth[t],cDark);
							}
							else {
								_toothChartRelay.SetCrown(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(proc.Surf=="U") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+2).ToString();
							}
						}
						else if(proc.Surf=="L") {
							teeth=new string[14];
							for(int t=0;t<14;t++) {
								teeth[t]=(t+18).ToString();
							}
						}
						else {
							teeth=proc.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<teeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(ToothInitialList,teeth[t])) {
								_toothChartRelay.SetPontic(teeth[t],cLight);
							}
							else {
								_toothChartRelay.SetCrown(teeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						_toothChartRelay.SetBigX(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.FillingDark:
						_toothChartRelay.SetSurfaceColors(proc.ToothNum,proc.Surf,cDark);
						break;
					case ToothPaintingType.FillingLight:
						_toothChartRelay.SetSurfaceColors(proc.ToothNum,proc.Surf,cLight);
						break;
					case ToothPaintingType.Implant:
						_toothChartRelay.SetImplant(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.PostBU:
						_toothChartRelay.SetBU(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.RCT:
						_toothChartRelay.SetRCT(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						_toothChartRelay.SetRetainedRoot(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.Sealant:
						_toothChartRelay.SetSealant(proc.ToothNum,cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Tooth && proc.ToothNum!=""){
							_toothChartRelay.SetSpaceMaintainer(proc.ToothNum,cDark);
						}
						else if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.ToothRange && proc.ToothRange!=""){
							teeth=proc.ToothRange.Split(',');
							for(int t=0;t<teeth.Length;t++) {
								_toothChartRelay.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						else if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Quad){
							teeth=new string[0];
							if(proc.Surf=="UR") {
								teeth=new string[] {"4","5","6","7","8"};
							}
							if(proc.Surf=="UL") {
								teeth=new string[] {"9","10","11","12","13"};
							}
							if(proc.Surf=="LL") {
								teeth=new string[] {"20","21","22","23","24"};
							}
							if(proc.Surf=="LR") {
								teeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<teeth.Length;t++) {//could still be length 0
								_toothChartRelay.SetSpaceMaintainer(teeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						_toothChartRelay.SetText(proc.ToothNum,cDark,ProcedureCodes.GetProcCode(proc.CodeNum).PaintText);
						break;
					case ToothPaintingType.Veneer:
						_toothChartRelay.SetVeneer(proc.ToothNum,cLight);
						break;
				}
			}
		}

		private void ToolBarMainUpdate_Click() {
			if(!new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {
				MsgBox.Show(this,"The update fee utility only works on current treatment plans, not any saved plans.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Update all fees and insurance estimates on this treatment plan to the current fees for this patient?")) {
				return;
			}
			Procedure procCur;
			List<ClaimProc> claimProcList=ClaimProcs.RefreshForTP(PatCur.PatNum);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			foreach(Procedure procedure in ProcListTP) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(procedure.CodeNum));
			}
			long discountPlanNum=_discountPlanSub?.DiscountPlanNum??0;
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,ProcListTP.Select(x=>x.MedicalCode).ToList(),ProcListTP.Select(x=>x.ProvNum).ToList(),
				PatCur.PriProv,PatCur.SecProv,PatCur.FeeSched,InsPlanList,ProcListTP.Select(x=>x.ClinicNum).ToList(),null,//listAppts not needed because procs not based on appts
				_listSubstLinks,discountPlanNum);
			//Get data for any OrthoCases that may be linked to procs in ProcList
			List<OrthoProcLink> listOrthoProcLinksAll=new List<OrthoProcLink>();
			Dictionary<long,OrthoProcLink> dictOrthoProcLinksForProcList=new Dictionary<long,OrthoProcLink>();
			Dictionary<long,OrthoCase> dictOrthoCases=new Dictionary<long,OrthoCase>();
			Dictionary<long,OrthoSchedule> dictOrthoSchedules=new Dictionary<long,OrthoSchedule>();
			OrthoCases.GetDataForListProcs(ref listOrthoProcLinksAll,ref dictOrthoProcLinksForProcList,ref dictOrthoCases,ref dictOrthoSchedules,_listProcs);
			OrthoProcLink orthoProcLink=null;
			OrthoCase orthoCase=null;
			OrthoSchedule orthoSchedule=null;
			List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
			for(int i=0;i<ProcListTP.Length;i++) {
				procCur=ProcListTP[i];
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")
					&& procCur.ProcNumLab!=0) 
				{
					continue;//The proc fee for a lab is derived from the lab fee on the parent procedure.
				}
				Procedure procOld=procCur.Copy(); //Get a copy of the old proc in case we need to update & to track the old procFee
				//Try to update the proc fee
				procCur.ProcFee=Procedures.GetProcFee(PatCur,PatPlanList,SubList,InsPlanList,procCur.CodeNum,procCur.ProvNum,procCur.ClinicNum,
					procCur.MedicalCode,listFees:listFees);
				OrthoCases.FillOrthoCaseObjectsForProc(procCur.ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,ref listOrthoProcLinksForOrthoCase,
					dictOrthoProcLinksForProcList,dictOrthoCases,dictOrthoSchedules,listOrthoProcLinksAll);
				Procedures.ComputeEstimates(procCur,PatCur.PatNum,claimProcList,false,InsPlanList,PatPlanList,BenefitList,PatCur.Age,SubList,
					orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase,listFees);
				if(AvaTax.DoSendProcToAvalara(procCur)) { //If needed, update the sales tax amount as well (checks HQ)
					procCur.TaxAmt=(double)AvaTax.GetEstimate(procCur.CodeNum,procCur.PatNum,procCur.ProcFee);
				}
				//If the proc fee changed or the tax amt changed, update the procedurelog entry
				if((procOld.ProcFee!=procCur.ProcFee) || (procOld.TaxAmt!=procCur.TaxAmt)) {
					Procedures.Update(procCur,procOld);
				}
				//no recall synch required 
			}
			long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
			ModuleSelected(PatCur.PatNum);//refreshes TPs
			for(int i=0;i<_listTreatPlans.Count;i++) {
				if(_listTreatPlans[i].TreatPlanNum==tpNum) {
					gridPlans.SetSelected(i,true);
				}
			}
			FillMain();
		}

		private void ToolBarMainCreate_Click(){//Save TP
			//Cannot even click this button if user has not selected one of the treatment plans; Otherwise button is disabled.
			if(!new[]{TreatPlanStatus.Active,TreatPlanStatus.Inactive}.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)){
			//if(gridPlans.SelectedIndices[0]!=0){
				MsgBox.Show(this,"An Active or Inactive TP must be selected before saving a TP.  You can highlight some procedures in the TP to save a TP with only those procedures in it.");
				return;
			}
			//Check for duplicate procedures on the appointment before sending the DFT to eCW.
			if(Programs.UsingEcwTightOrFullMode() && Bridges.ECW.AptNum!=0) {
				List<Procedure> procs=Procedures.GetProcsForSingle(Bridges.ECW.AptNum,false);
				string duplicateProcs=ProcedureL.ProcsContainDuplicates(procs);
				if(duplicateProcs!="") {
					MessageBox.Show(duplicateProcs);
					return;
				}
			}
			if(gridMain.SelectedIndices.Length==0){
				gridMain.SetAll(true);//Select all if none selected.
			}
			List<TreatPlanAttach> listTreatPlanAttaches=TreatPlanAttaches.GetAllForTreatPlan(_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum);
			TreatPlan tp=new TreatPlan();
			string treatPlanHeading=_listTreatPlans[gridPlans.SelectedIndices[0]].Heading;
			if(PrefC.GetBool(PrefName.TreatPlanPromptSave)) { 
				if(!IsSavedTPHeadingUnique(treatPlanHeading)) {
					int fileNum=0;
					while(!IsSavedTPHeadingUnique(treatPlanHeading)) {
						//If the file has (#) at the end, get the number to increment later.
						string headingEndingNum=Regex.Match(treatPlanHeading,@"\([0-9]+\)").ToString();
						if(!headingEndingNum.IsNullOrEmpty()) {
							fileNum=PIn.Int(Regex.Replace(headingEndingNum,@"[\(\)]",""));
						}
						//Remove all (#)'s from heading and any whitespace that may be present, before post-pending (#)
						treatPlanHeading=Regex.Replace(treatPlanHeading,@"\([0-9]+\)","").TrimEnd();
						treatPlanHeading+=$" ({fileNum+1})";
					}
				}
				using InputBox inputHeadingName=new InputBox(Lan.g(this,$"Save Treatment Plan as"),treatPlanHeading);
				if(inputHeadingName.ShowDialog()!=DialogResult.OK) {
					return;
				}
				treatPlanHeading=inputHeadingName.textResult.Text;
			}
			tp.Heading=treatPlanHeading;
			tp.DateTP=DateTime.Today;
			tp.PatNum=PatCur.PatNum;
			tp.Note=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			tp.ResponsParty=PatCur.ResponsParty;
			tp.UserNumPresenter=Security.CurUser.UserNum;
			tp.TPType=_listTreatPlans[gridPlans.SelectedIndices[0]].TPType;
			TreatPlans.Insert(tp);
			ProcTP procTP;
			Procedure proc;
			int itemNo=0;
			List<ProcTP> listSelectedProcTp=gridMain.SelectedIndices.Where(x => x>-1 && x<gridMain.ListGridRows.Count)
				.Select(x => (ProcTP)gridMain.ListGridRows[x].Tag).ToList();
			foreach(ProcTP selectedProcTP in listSelectedProcTp) {
				if(selectedProcTP==null){
					//user must have highlighted a subtotal row.
					continue;
				}
				proc=Procedures.GetOneProc(selectedProcTP.ProcNumOrig,true);
				procTP=new ProcTP();
				procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatCur.PatNum;
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ItemOrder=itemNo;
				TreatPlanAttach tpAttach=listTreatPlanAttaches.FirstOrDefault(x=>x.ProcNum==proc.ProcNum);
				if(tpAttach==null) {
					//This could happen if another workstation completed this procedure just now.
					procTP.Priority=0;
				}
				else {
					procTP.Priority=tpAttach.Priority;
				}
				procTP.ToothNumTP=Tooth.ToInternat(proc.ToothNum);
				if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Surf){
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);
				}
				else{
					procTP.Surf=proc.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
				procTP.Descript=selectedProcTP.Descript;
				procTP.FeeAmt=selectedProcTP.FeeAmt;
				procTP.PriInsAmt=selectedProcTP.PriInsAmt;
				procTP.SecInsAmt=selectedProcTP.SecInsAmt;
				procTP.Discount=selectedProcTP.Discount;
				procTP.PatAmt=selectedProcTP.PatAmt;
				procTP.Prognosis=selectedProcTP.Prognosis;
				procTP.Dx=selectedProcTP.Dx;
				procTP.ProcAbbr=selectedProcTP.ProcAbbr;
				procTP.FeeAllowed=selectedProcTP.FeeAllowed;
				procTP.TaxAmt=selectedProcTP.TaxAmt;
				procTP.ProvNum=selectedProcTP.ProvNum;
				procTP.DateTP=selectedProcTP.DateTP;
				procTP.ClinicNum=selectedProcTP.ClinicNum;
				procTP.CatPercUCR=selectedProcTP.CatPercUCR;
				ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;
				#region Canadian Lab Fees
				/*
				proc=(Procedure)gridMain.Rows[gridMain.SelectedIndices[i]].Tag;
				procTP=new ProcTP();
				procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatCur.PatNum;
				procTP.ProcNumOrig=proc.ProcNum;
				procTP.ItemOrder=itemNo;
				procTP.Priority=proc.Priority;
				procTP.ToothNumTP="";
				procTP.Surf="";
				procTP.Code=proc.LabProcCode;
				procTP.Descript=gridMain.Rows[gridMain.SelectedIndices[i]]
					.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Description"))].Text;
				if(checkShowFees.Checked) {
					procTP.FeeAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Fee"))].Text);
				}
				if(checkShowIns.Checked) {
					procTP.PriInsAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Pri Ins"))].Text);
					procTP.SecInsAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Sec Ins"))].Text);
					procTP.PatAmt=PIn.PDouble(gridMain.Rows[gridMain.SelectedIndices[i]]
						.Cells[gridMain.Columns.GetIndex(Lan.g("TableTP","Pat"))].Text);
				}
				ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;*/
				#endregion Canadian Lab Fees
			}
			ModuleSelected(PatCur.PatNum);
			for(int i=0;i<_listTreatPlans.Count;i++){
				if(_listTreatPlans[i].TreatPlanNum==tp.TreatPlanNum){
					gridPlans.SetSelected(i,true);
					FillMain();
				}
			}
			//Send TP DFT HL7 message to ECW with embedded PDF when using tight or full integration only.
			if(Programs.UsingEcwTightOrFullMode() && Bridges.ECW.AptNum!=0){
				PrepImageForPrinting();
				MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(true,PdfFontEmbedding.Always);
				pdfRenderer.Document=CreateDocument();
				pdfRenderer.RenderDocument();
				MemoryStream ms=new MemoryStream();
				pdfRenderer.PdfDocument.Save(ms);
				byte[] pdfBytes=ms.GetBuffer();
				//#region Remove when testing is complete.
				//string tempFilePath=Path.GetTempFileName();
				//File.WriteAllBytes(tempFilePath,pdfBytes);
				//#endregion
				string pdfDataStr=Convert.ToBase64String(pdfBytes);
				if(HL7Defs.IsExistingHL7Enabled()) {
					//DFT messages that are PDF's only and do not include FT1 segments, so proc list can be empty
					//MessageConstructor.GenerateDFT(procList,EventTypeHL7.P03,PatCur,Patients.GetPat(PatCur.Guarantor),Bridges.ECW.AptNum,"treatment",pdfDataStr);
					MessageHL7 messageHL7=MessageConstructor.GenerateDFT(new List<Procedure>(),EventTypeHL7.P03,PatCur,Patients.GetPat(PatCur.Guarantor),Bridges.ECW.AptNum,"treatment",pdfDataStr);
					if(messageHL7==null) {
						MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
						return;
					}
					HL7Msg hl7Msg=new HL7Msg();
					hl7Msg.AptNum=0;//Prevents the appt complete button from changing to the "Revise" button prematurely.
					hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hl7Msg.MsgText=messageHL7.ToString();
					hl7Msg.PatNum=PatCur.PatNum;
					HL7Msgs.Insert(hl7Msg);
				}
				else {
					Bridges.ECW.SendHL7(Bridges.ECW.AptNum,PatCur.PriProv,PatCur,pdfDataStr,"treatment",true,null);//just pdf, passing null proc list
				}
			}
		}

		///<summary>Returns true if given treatPlanHeading is not currently in use by a saved TreatPlan in _listTreatPlans.</summary>
		private bool IsSavedTPHeadingUnique(string treatPlanHeading) {
			return (_listTreatPlans.FindAll(x => x.TPStatus==TreatPlanStatus.Saved 
				&& x.Heading==treatPlanHeading).Count()==0
			);
		}

		private void ToolBarMainSign_Click() {
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus!=TreatPlanStatus.Saved) {
				MsgBox.Show(this,"You may only sign a saved TP, not an Active or Inactive TP.");
				return;
			}
			//string patFolder=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
			if(PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf) //preference enabled
			   && _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="" //and document is signed
			   && Documents.DocExists(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum)) //and file exists
			{
				MsgBox.Show(this,"Document already signed and saved to PDF. Unsign treatment plan from edit window to enable resigning.");
				Cursor=Cursors.WaitCursor;
				Documents.OpenDoc(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum);
				Cursor=Cursors.Default;
				return;//cannot re-sign document.
			}
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum>0 && !Documents.DocExists(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Unable to open saved treatment plan. Would you like to recreate document using current information?")) {
					return;
				}
			}//TODO: Implement ODprintout pattern - MigraDoc
			using FormTPsign FormT=new FormTPsign();
			if(DoPrintUsingSheets()) {
				TreatPlan treatPlan;
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
				treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
				FormT.SheetTP=TreatPlanToSheet(treatPlan);
				FormT.TotalPages=Sheets.CalculatePageCount(FormT.SheetTP,SheetPrinting.PrintMargin);
			}
			else {//Classic TPs
				PrepImageForPrinting();
				MigraDoc.DocumentObjectModel.Document doc=CreateDocument();
				MigraDocPrintDocument printdoc=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
				DocumentRenderer renderer=new MigraDoc.Rendering.DocumentRenderer(doc);
				renderer.PrepareDocument();
				printdoc.Renderer=renderer;
				FormT.Document=printdoc;
				FormT.TotalPages=renderer.FormattedDocument.PageCount;
			}
			long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
			FormT.SaveDocDelegate=SaveTPAsDocument;
			FormT.TPcur=_listTreatPlans[gridPlans.SelectedIndices[0]];
			FormT.DoPrintUsingSheets=DoPrintUsingSheets();
			FormT.ShowDialog();
			ModuleSelected(PatCur.PatNum);//refreshes TPs
			for(int i=0;i<_listTreatPlans.Count;i++) {
				if(_listTreatPlans[i].TreatPlanNum==tpNum) {
					gridPlans.SetSelected(i,true);
				}
			}
			FillMain();
		}

		///<summary>Saves TP as PDF in each image category defined as TP category. 
		/// If TreatPlanSaveSignedToPdf enabled, will default to first non-hidden category if no TP categories are explicitly defined.</summary>
		private List<Document> SaveTPAsDocument(bool isSigSave,Sheet sheet=null) {
			if(DoPrintUsingSheets() && sheet==null) {
				MsgBox.Show(this,"An error has occurred with the Treatment Plans to sheets feature.  Please contact support.");
				return new List<Document>();
			}
			List<Document> retVal=new List<Document>();
			//Determine each of the document categories that this TP should be saved to.
			//"R"==TreatmentPlan; see FormDefEditImages.cs
			List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			List<long> categories= listImageCatDefs.Where(x => x.ItemValue.Contains("R")).Select(x=>x.DefNum).ToList();
			if(isSigSave && categories.Count==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				//we must save at least one document, pick first non-hidden image category.
				Def imgCat=listImageCatDefs.FirstOrDefault(x => !x.IsHidden);
				if(imgCat==null) {
					MsgBox.Show(this,"Unable to save treatment plan because all image categories are hidden.");
					return new List<Document>();
				}
				categories.Add(imgCat.DefNum);
			}
			//Gauranteed to have at least one image category at this point.
			//Saving pdf to tempfile first simplifies this code, but can use extra bandwidth copying the file to and from the temp directory/Open Dent imgs.
			string tempFile=PrefC.GetRandomTempFile(".pdf");
			string rawBase64="";
			if(DoPrintUsingSheets()) {
				SheetPrinting.CreatePdf(sheet,tempFile,null);
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					rawBase64=Convert.ToBase64String(System.IO.File.ReadAllBytes(tempFile));//Todo test this
				}
			}
			else {//classic TPs
				MigraDoc.Rendering.PdfDocumentRenderer pdfRenderer;
				pdfRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(false,PdfFontEmbedding.Always);
				pdfRenderer.Document=CreateDocument();
				pdfRenderer.RenderDocument();
				pdfRenderer.Save(tempFile);
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					using(MemoryStream stream=new MemoryStream()) {
						pdfRenderer.Save(stream,false);
						rawBase64=Convert.ToBase64String(stream.ToArray());
						stream.Close();
					}
				}
			}
			foreach(long docCategory in categories) {//usually only one, but do allow them to be saved once per image category.
				OpenDentBusiness.Document docSave=new Document();
				docSave.DocNum=Documents.Insert(docSave);
				string fileName="TPArchive"+docSave.DocNum;
				docSave.ImgType=ImageType.Document;
				docSave.DateCreated=DateTime.Now;
				docSave.PatNum=PatCur.PatNum;
				docSave.DocCategory=docCategory;
				docSave.Description=fileName;//no extension.
				docSave.RawBase64=rawBase64;//blank if using AtoZfolder
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					string filePath=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
					while(File.Exists(filePath+"\\"+fileName+".pdf")) {
						fileName+="x";
					}
					File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
				}
				else if(CloudStorage.IsCloudStorage) {
					//Upload file to patient's AtoZ folder
					using FormProgress FormP=new FormProgress();
					FormP.DisplayText="Uploading Treatment Plan...";
					FormP.NumberFormat="F";
					FormP.NumberMultiplication=1;
					FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					FormP.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload state=CloudStorage.UploadAsync(ImageStore.GetPatientFolder(PatCur,"")
						,fileName+".pdf"
						,File.ReadAllBytes(tempFile)
						,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
					if(FormP.ShowDialog()==DialogResult.Cancel) {
						state.DoCancel=true;
						break;
					}
				}
				docSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
				Documents.Update(docSave);
				retVal.Add(docSave);
			}
			try {
				File.Delete(tempFile); //cleanup the temp file.
			}
			catch {}
			return retVal;
		}

		///<summary>Returns true if the user has not checked 'Print using classic'.</summary>
		private bool DoPrintUsingSheets() {
			//If the Printing tab is visible and the print classic box is checked, then print classic.
			return (!tabShowSort.TabPages.Contains(tabPagePrint) || !checkPrintClassic.Checked);
		}

		///<summary>Similar method in Account</summary>
		private bool CheckClearinghouseDefaults() {
			if(PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==0) {
				MsgBox.Show(this,"No default dental clearinghouse defined.");
				return false;
			}
			if(PrefC.GetBool(PrefName.ShowFeatureMedicalInsurance) && PrefC.GetLong(PrefName.ClearinghouseDefaultMed)==0) {
				MsgBox.Show(this,"No default medical clearinghouse defined.");
				return false;
			}
			return true;
		}

		private void ToolBarMainPreAuth_Click() {
			if(gridPlans.SelectedIndices.Length==0) {
				return;
			}
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			if(!CheckClearinghouseDefaults()) {
				return;
			}
			if(!new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {
				MsgBox.Show(this,"You can only send a preauth from a current TP, not a saved TP.");
				return;
			}
			if(gridMain.SelectedIndices.All(x => gridMain.ListGridRows[x].Tag==null)) {
				MessageBox.Show(Lan.g(this,"Please select procedures first."));
				return;
			}
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canada
				List<int> selectedIndices=new List<int>(gridMain.SelectedIndices);
				if(gridMain.SelectedIndices.Length>7) {
					gridMain.SetAll(false);
					int selectedLabCount=0;
					foreach(int selectedIdx in selectedIndices) {
						if(gridMain.ListGridRows[selectedIdx].Tag==null) {
							continue;//subtotal row.
						}
						Procedure proc=(Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[selectedIdx].Tag).ProcNumOrig,false));
						if(proc!=null && proc.ProcNumLab!=0) {
								selectedLabCount++;
						}
						gridMain.SetSelected(selectedIdx,true);
						if(gridMain.SelectedIndices.Length-selectedLabCount>=7) {
							break;//we have found seven procedures.
						}
					}
					if(selectedIndices.FindAll(x => gridMain.ListGridRows[x].Tag!=null).Count-selectedLabCount>7) {//only if they selected more than 7 procedures, not 7 rows.
						MsgBox.Show(this,"Only the first 7 procedures will be selected.  You will need to create another preauth for the remaining procedures.");
					}
				}
			}
			Claim ClaimCur=new Claim();
      using FormInsPlanSelect FormIPS=new FormInsPlanSelect(PatCur.PatNum); 
			FormIPS.ViewRelat=true;
			if(FormIPS.SelectedPlan==null) {//Won't be null if there is only one PatPlan
				FormIPS.ShowDialog();
				if(FormIPS.DialogResult!=DialogResult.OK) {
					return;
				}
			}
			ClaimCur.PatNum=PatCur.PatNum;
			ClaimCur.ClaimNote="";
			ClaimCur.ClaimStatus="W";
			ClaimCur.DateSent=DateTime.Today;
			ClaimCur.DateSentOrig=DateTime.MinValue;
			ClaimCur.PlanNum=FormIPS.SelectedPlan.PlanNum;
			ClaimCur.InsSubNum=FormIPS.SelectedSub.InsSubNum;
			ClaimCur.ProvTreat=0;
			ClaimCur.ClaimForm=FormIPS.SelectedPlan.ClaimFormNum;
			ClaimCur.MedType=EnumClaimMedType.Dental;
			if(FormIPS.SelectedPlan.IsMedical) {
				ClaimCur.MedType=PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical) ? EnumClaimMedType.Institutional : EnumClaimMedType.Medical;
			}
			List<Procedure> listProcsSelected=new List<Procedure>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				if(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag==null){
					continue;//skip any hightlighted subtotal lines
				}
				Procedure proc=Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag).ProcNumOrig,false);
				if(Procedures.NoBillIns(proc,ClaimProcList,ClaimCur.PlanNum)) {
					MsgBox.Show(this,"Not allowed to send procedures to insurance that are marked 'Do not bill to ins'.");
					return;
				}
				if(proc.ProcNumLab!=0) {
					continue;//Ignore Canadian labs. Labs are indirectly attached to the claim through a parent procedure
				}
				listProcsSelected.Add(proc);
				if(ClaimCur.ProvTreat==0){//makes sure that at least one prov is set
					ClaimCur.ProvTreat=proc.ProvNum;
				}
				if(!Providers.GetIsSec(proc.ProvNum)) {
					ClaimCur.ProvTreat=proc.ProvNum;
				}
				ProcedureCode procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				//Check to see if the selected procedure is for Ortho and if the customer has preferences set to automatically check the "Is For Ortho" checkbox
				if(!ClaimCur.IsOrtho && PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho)) {//If it's already marked as Ortho (from a previous procedure), skip this
					CovCat orthoCategory=CovCats.GetFirstOrDefault(x => x.EbenefitCat==EbenefitCategory.Orthodontics,true);
					if(orthoCategory!=null) {
						if(CovSpans.IsCodeInSpans(procCode.ProcCode,CovSpans.GetWhere(x => x.CovCatNum==orthoCategory.CovCatNum).ToArray()))	{
							ClaimCur.IsOrtho=true;
						}
					}
				}
			}
			//Make sure that all procedures share the same place of service and clinic.
			long procClinicNum=-1;
			PlaceOfService placeService=PlaceOfService.Office;//Old behavior was to always use Office.
			for(int i=0;i<listProcsSelected.Count;i++) {
				if(i==0) {
					procClinicNum=listProcsSelected[i].ClinicNum;
					placeService=listProcsSelected[i].PlaceService;
					continue;
				}
				Procedure proc=listProcsSelected[i];
				if(PrefC.HasClinicsEnabled && procClinicNum!=proc.ClinicNum) {
					MsgBox.Show(this,"All procedures do not have the same clinic.");
					return;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && proc.PlaceService!=placeService) {
					MsgBox.Show(this,"All procedures do not have the same place of service.");
					return;
				}
			}
			switch(PIn.Enum<ClaimZeroDollarProcBehavior>(PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior))) {
				case ClaimZeroDollarProcBehavior.Warn:
					if(listProcsSelected.FirstOrDefault(x => CompareDouble.IsZero(x.ProcFee))!=null
						&& !MsgBox.Show("ContrTreat",MsgBoxButtons.OKCancel,"You are about to make a claim that will include a $0 procedure.  Continue?"))
					{
						return;
					}
					break;
				case ClaimZeroDollarProcBehavior.Block:
					if(listProcsSelected.FirstOrDefault(x => CompareDouble.IsZero(x.ProcFee))!=null) {
						MsgBox.Show("ContrTreat","You can't make a claim for a $0 procedure.");
						return;
					}
					break;
				case ClaimZeroDollarProcBehavior.Allow:
				default:
					break;
			}
			//Make the clinic on the claim match the clinic of the procedures.  Use the patients clinic if no procedures selected (shouldn't happen).
			ClaimCur.ClinicNum=(procClinicNum > -1) ? procClinicNum : PatCur.ClinicNum;
			if(Providers.GetIsSec(ClaimCur.ProvTreat)){
				ClaimCur.ProvTreat=PatCur.PriProv;
				//OK if 0, because auto select first in list when open claim
			}
			ClaimCur.ProvBill=Providers.GetBillingProvNum(ClaimCur.ProvTreat,ClaimCur.ClinicNum);
			Provider prov=Providers.GetProv(ClaimCur.ProvTreat);//If ever null then check this same line inside AccountModules.CreateClaim()
			if(prov.ProvNumBillingOverride!=0) {
				ClaimCur.ProvBill=prov.ProvNumBillingOverride;
			}
			ClaimCur.EmployRelated=YN.No;
      ClaimCur.ClaimType="PreAuth";
			ClaimCur.AttachedFlags="Mail";
			//this could be a little better if we automate figuring out the patrelat
			//instead of making the user enter it:
			ClaimCur.PatRelat=FormIPS.PatRelat;
			ClaimCur.PlaceService=placeService;
			ClaimCur=Claims.GetClaim(Claims.Insert(ClaimCur));
			ClaimProc ClaimProcCur;
			ClaimProc cpExisting;
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			List<long> listCodeNums=new List<long>();//List of codeNums that have had their default note added to the claim.
			foreach(Procedure procCur in listProcsSelected){
				cpExisting=ClaimProcs.GetEstimate(ClaimProcList,procCur.ProcNum,FormIPS.SelectedPlan.PlanNum,FormIPS.SelectedSub.InsSubNum);
				double insPayEst=0;
				if(cpExisting!=null) {
					insPayEst=cpExisting.InsPayEst;
				}
				ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(procCur.CodeNum);
				ClaimProcCur=new ClaimProc();
				ClaimProcs.CreateEst(ClaimProcCur,procCur,FormIPS.SelectedPlan,FormIPS.SelectedSub,0,insPayEst,false,true);//preauth est
        ClaimProcCur.ClaimNum=ClaimCur.ClaimNum;
				ClaimProcCur.NoBillIns=(cpExisting!=null) ? cpExisting.NoBillIns : false;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && cpExisting!=null) {
					//ClaimProc.Percentage is typically set in ClaimProcs.ComputeBaseEst(...), not for pre-auths. 
					ClaimProcCur.Percentage=cpExisting.Percentage;//Used for Canadian preauths with lab procedures.
				}
				ClaimProcCur.FeeBilled=procCur.ProcFee;
				if(FormIPS.SelectedPlan.UseAltCode && (procCodeCur.AlternateCode1!="")){
					ClaimProcCur.CodeSent=procCodeCur.AlternateCode1;
				}
				else if(FormIPS.SelectedPlan.IsMedical && procCur.MedicalCode!=""){
					ClaimProcCur.CodeSent=procCur.MedicalCode;
				}
				else{
					ClaimProcCur.CodeSent=ProcedureCodes.GetStringProcCode(procCur.CodeNum);
					if(ClaimProcCur.CodeSent.Length>5 && ClaimProcCur.CodeSent.Substring(0,1)=="D"){
						ClaimProcCur.CodeSent=ClaimProcCur.CodeSent.Substring(0,5);
					}
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(ClaimProcCur.CodeSent.Length>5) { //In Canadian electronic claims, codes can contain letters or numbers and cannot be longer than 5 characters.
							ClaimProcCur.CodeSent=ClaimProcCur.CodeSent.Substring(0,5);
						}
					}
				}
				listClaimProcs.Add(ClaimProcCur);
				if(ClaimCur.ClaimNote==null) {
					ClaimCur.ClaimNote="";
				}
				if(!listCodeNums.Contains(procCodeCur.CodeNum)) {
					if(ClaimCur.ClaimNote.Length > 0 && !string.IsNullOrEmpty(procCodeCur.DefaultClaimNote)) {
						ClaimCur.ClaimNote+="\n";
					}
					ClaimCur.ClaimNote+=procCodeCur.DefaultClaimNote;
					listCodeNums.Add(procCodeCur.CodeNum);
				}
				//ProcCur.Update(ProcOld);
			}
			for(int i=0;i<listClaimProcs.Count;i++) {
				listClaimProcs[i].LineNumber=(byte)(i+1);
				ClaimProcs.Insert(listClaimProcs[i]);
			}
			_listProcs=Procedures.Refresh(PatCur.PatNum);
			//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			Claims.CalculateAndUpdate(_listProcs,InsPlanList,ClaimCur,PatPlanList,BenefitList,PatCur,SubList);
			using FormClaimEdit FormCE=new FormClaimEdit(ClaimCur,PatCur,FamCur);
			//FormCE.CalculateEstimates(
			FormCE.IsNew=true;//this causes it to delete the claim if cancelling.
			FormCE.ShowDialog();
			ModuleSelected(PatCur.PatNum);
		}

		private void ToolBarMainDiscount_Click() {
			if(!new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {
				MsgBox.Show(this,"You can only create discounts from a current TP, not a saved TP.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				gridMain.SetAll(true);
			}
			List<Procedure> listProcs=Procedures.GetManyProc(gridMain.SelectedIndices.ToList()
				.FindAll(x => gridMain.ListGridRows[x].Tag!=null)
				.Select(x => ((ProcTP)gridMain.ListGridRows[x].Tag).ProcNumOrig)
				.ToList(),false);
			if(listProcs.Count<=0) {
				MsgBox.Show(this,"There are no procedures selected in the treatment plan. Please add to, or select from, procedures attached to the treatment plan before applying a discount");
				return;
			}
			using FormTreatmentPlanDiscount FormTPD=new FormTreatmentPlanDiscount(listProcs);
			FormTPD.ShowDialog();
			if(FormTPD.DialogResult==DialogResult.OK) {
				long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
				ModuleSelected(PatCur.PatNum);//refreshes TPs
				for(int i=0;i<_listTreatPlans.Count;i++) {
					if(_listTreatPlans[i].TreatPlanNum==tpNum) {
						gridPlans.SetSelected(i,true);
					}
				}
				FillMain();
			}
		}

		private void gridPreAuth_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			Claim claim=Claims.GetClaim(((Claim)ALPreAuth[e.Row]).ClaimNum);//gets attached images.
			if(claim==null) {
				MsgBox.Show(this,"The pre authorization has been deleted.");
				ModuleSelected(PatCur.PatNum);
				return;
			}
			using FormClaimEdit FormC=new FormClaimEdit(claim,PatCur,FamCur);
      //FormClaimEdit2.IsPreAuth=true;
			FormC.ShowDialog();
			if(FormC.DialogResult!=DialogResult.OK){
				return;
			}
			ModuleSelected(PatCur.PatNum);    
		}

		private void gridPreAuth_CellClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			if(_listTreatPlans==null 
				|| _listTreatPlans.Count==0
				|| (_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus!=TreatPlanStatus.Active 
				&& _listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus!=TreatPlanStatus.Inactive))
			{
				return;
			}
			gridMain.SetAll(false);
			Claim ClaimCur=(Claim)ALPreAuth[e.Row];
			List<ClaimProc> ClaimProcsForClaim=ClaimProcs.RefreshForClaim(ClaimCur.ClaimNum);
			for(int i=0;i<gridMain.ListGridRows.Count;i++){//ProcListTP.Length;i++){
				if(gridMain.ListGridRows[i].Tag==null){
					continue;//must be a subtotal row
				}
				ProcTP procTP=(ProcTP)gridMain.ListGridRows[i].Tag;
				//proc=(Procedure)gridMain.Rows[i].Tag;
				for(int j=0;j<ClaimProcsForClaim.Count;j++){
					if(procTP.ProcNumOrig==ClaimProcsForClaim[j].ProcNum){
						gridMain.SetSelected(i,true);
						CanadianSelectedRowHelper(procTP);
						break;
					}
				}
			}
		}

		private void butInsRem_Click(object sender,EventArgs e) {
			if(PatCur==null) {
				MsgBox.Show(this,"Please select a patient before attempting to view insurance remaining.");
				return;
			}
			using FormInsRemain FormIR=new FormInsRemain(PatCur.PatNum);
			FormIR.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			//Update all unscheduled procs and claimprocs on the active TP to use the currently selected date as the ProcDate
			RefreshModuleScreen();
		}
		
		private void butPlannedAppt_Click(object sender,EventArgs e) {
			if(PatCur==null) {
				MsgBox.Show(this,"Please select a Patient.");
				return;
			}
			if(!gridPlans.GetSelectedIndex().Between(0,_listTreatPlans.Count-1)
				|| _listTreatPlans[gridPlans.GetSelectedIndex()].TPStatus!=TreatPlanStatus.Active || gridMain.SelectedIndices.Count()==0) 
			{
				MsgBox.Show(this,"Please select at least one procedure on an Active treatment plan.");
				return;
			}
			//We only care about ShowAppointments in the ChartModuleComponentsToLoad, reduces Db calls
			ChartModuleComponentsToLoad chartComponents=new ChartModuleComponentsToLoad(false);
			chartComponents.ShowAppointments=true;
			//Makes sure ChartModules.rawApt is filled before calling ChartModules.GetPlannedApt
			ChartModules.GetProgNotes(PatCur.PatNum,false,chartComponents);
			int itemOrder=ChartModules.GetPlannedApt(PatCur.PatNum).Rows.Count+1;
			List<long> listSelectedProcNums=gridMain.SelectedGridRows
				.FindAll(x => x.Tag!=null && x.Tag.GetType()==typeof(ProcTP))//ProcTP's only
				.Select(x => ((ProcTP)(x.Tag)).ProcNumOrig).ToList();//get ProcNums
			//mimic calls to CreatePlannedAppt in ContrChart, no need for FillPlanned() since no gridPlanned
			AppointmentL.CreatePlannedAppt(PatCur,itemOrder,listSelectedProcNums);
		}

		private void dateTimeTP_CloseUp(object sender,EventArgs e) {
			//Update all unscheduled procs and claimprocs on the active TP to use the currently selected date as the ProcDate
			RefreshModuleScreen();
		}

		private void textNote_Leave(object sender,EventArgs e) {
			UpdateTPNoteIfNeeded();
		}

		private void textNote_TextChanged(object sender,EventArgs e) {
			HasNoteChanged=true;
		}

		///<summary>Saves TP note to the database if changes were made</summary>
		public void UpdateTPNoteIfNeeded() {
			if(textNote.ReadOnly
				|| !HasNoteChanged
				|| gridPlans.SelectedIndices.Length==0
				|| _listTreatPlans.Count<=gridPlans.SelectedIndices[0]) 
			{
				return;
			}
			_listTreatPlans[gridPlans.SelectedIndices[0]].Note=PIn.String(textNote.Text);
			TreatPlans.Update(_listTreatPlans[gridPlans.SelectedIndices[0]]);
			HasNoteChanged=false;
		}

		private void radioTreatPlanSortTooth_MouseClick(object sender,MouseEventArgs e) {
			FormOpenDental.IsTreatPlanSortByTooth=radioTreatPlanSortTooth.Checked;
			FillMainData(); //need to do this so that the data is refreshed as the order of the treatment plan may have changed.
			FillMainDisplay();
		}

		/// <summary></summary>
		private void UpdateToolbarButtons() {
			if(PatCur!=null && _listTreatPlans.Count>0) {
				gridMain.Enabled=true;
				tabShowSort.Enabled=true;
				listSetPr.Enabled=true;
				//panelSide.Enabled=true;
				ToolBarMain.Buttons["Discount"].Enabled=true;
				ToolBarMain.Buttons["PreAuth"].Enabled=true;
				ToolBarMain.Buttons["Update"].Enabled=true;
				//ToolBarMain.Buttons["Create"].Enabled=true;
				ToolBarMain.Buttons["Print"].Enabled=true;
				ToolBarMain.Buttons["Email"].Enabled=true;
				ToolBarMain.Buttons["Sign"].Enabled=true;
				butSaveTP.Enabled=true;
				ToolBarMain.Invalidate();
				if(PatPlanList.Count==0) {//patient doesn't have insurance
					checkShowMaxDed.Visible=false;
					if(_discountPlanSub==null) {
						checkShowIns.Checked=false;
					} 
					else if(!checkShowInsNotAutomatic) {
						checkShowIns.Checked=true;
					}
				}
				else {//patient has insurance
					if(!PrefC.GetBool(PrefName.EasyHideInsurance)) {//if insurance isn't hidden
						checkShowMaxDed.Visible=true;
						if(checkShowFees.Checked) {//if fees are showing
							if(!checkShowInsNotAutomatic) {
								checkShowIns.Checked=true;
							}
						}
					}
				}
				if(!checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
			}
			else {
				gridMain.Enabled=false;
				tabShowSort.Enabled=false;
				listSetPr.Enabled=false;
				butSaveTP.Enabled=false;
				//panelSide.Enabled=false;
				ToolBarMain.Buttons["Discount"].Enabled=false;
				ToolBarMain.Buttons["PreAuth"].Enabled=false;
				ToolBarMain.Buttons["Update"].Enabled=false;
				//ToolBarMain.Buttons["Create"].Enabled=false;
				ToolBarMain.Buttons["Print"].Enabled=false;
				ToolBarMain.Buttons["Email"].Enabled=false;
				ToolBarMain.Buttons["Sign"].Enabled=false;
				ToolBarMain.Invalidate();
				//listPreAuth.Enabled=false;
			}
		}
	}
}
