#region using
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.HL7;
using SHDocVw;
using SparksToothChart;
using OpenDental.Bridges;
using System.Drawing.Imaging;
//using System.Threading;//no threading in UI
using SharpDX;
#if EHRTEST
using EHR;
#endif
#endregion using

namespace OpenDental {
	///<summary></summary>
	public partial class ControlChart:UserControl	{
		#region Fields - Public
		///<summary>public for plugins.</summary>
		public bool TreatmentNoteChanged;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		///<summary>This is a reference to the _pd in FormOpenDental.</summary>
		public PatientData Pd;
		#endregion Fields - Public

		#region Fields - Private
		///<summary>Locker for Apteryx Thumbnail downloads</summary>
		private object _apteryxLocker=new object();
		///<summary>This is a list that is 1:1 with what's showing in current tab.</summary>
		private List<ImageInfo> _listImageInfos;
		///<summary>a list of the hidden teeth as strings. Includes "1"-"32", and "A"-"Z"</summary>
		private ArrayList _arrayListHiddenTeeth;
		///<summary>A list of the DefNums that is 1:1 with the image tabs that are showing.</summary>
		private List<long> _listDefNumsVisImageCats;
		private ProcButton[] _arrayProcButtons;
		///<summary>This is a filtered list containing only TP procedures.  It's also already sorted by priority and tooth number.</summary>
		private List<Procedure> _listProceduresTPs;
		///<summary>A specific reference to the "eRx" button.  This special reference helps us refresh the notification text on the button after the user changes.</summary>
		private ODToolBarButton _butErx;
		private int _chartScrollVal;
		///<summary>Can be null if user has not set up any views.  Defaults to first in list when starting up.</summary>
		private ChartView _chartViewCurDisplay;
		///<summary>The time that we started our last prog note search.</summary>
		private DateTime _dateTimeLastSearch;
		///<summary>Dictionary of every open FormErx per PatNum.  This is to ensure we only open 1 FormErx per PatNum.</summary>
		private Dictionary<long,FormErx> _dictFormErxSessions=new Dictionary<long,FormErx>();
		///<summary>Dictionary linking a TreatPlanNum key to the list of TpRows for that TP.</summary>
		private Dictionary<long,List<TpRow>> _dictTpNumListTpRows;
		//private Family _family;
		private FormImageViewer _formImageViewer;
		///<summary>Store the default max value since we adjust this when sorting the grid after clicking on a column header.</summary>
		private int _gridProgMaxPageRowsDefault;
		private int _headingPrintH;
		private bool _headingPrinted;
		private float _heightTabProcOrig96=0;
		private int _imageSplitterOriginalY;
		private bool _initializedOnStartup;
		[DllImport("wininet.dll",CharSet = CharSet.Auto,SetLastError = true)]
		static extern bool InternetSetCookie(string lpszUrlName,string lbszCookieName,string lpszCookieData);
		private bool _isDistributorKey;
		///<summary>This keeps track of if the progress notes are being filled.</summary>
		private bool _isFillingProgNotes;
		///<summary>For various reasons, this module can become unselected while clicking around, and there are certain actions we shouldn't take if this happens. See IsPatientNull for further comments.</summary>
		private bool _isModuleSelected;
		/// <summary>TextSearch has changed, but we are waiting for a short time before performing the search.</summary>
		private bool _isSearchPending;
		///<summary>List of images that have been downloaded from Apteryx.</summary>
		private List<ApteryxThumbnail> _listApteryxThumbnails;
		private List<ChartView> _listChartViews;
		///<summary>List of procedures added via quick add or procedure button.  Cleared for each user click before adding.</summary>
		private List<Procedure> _listChartedProcs=null;
		///<summary>List of all procedures (except deleted status) for the current patient.</summary>
		private List<Procedure> _listPatProcs;
		///<summary>List containing only rows showing in gridPlanned, can be the same as _tablePlannedAll</summary>
		private List<DataRow> _listPlannedAppt;
		private List<ProcButtonQuick> _listProcButtonQuicks;
		///<summary>Used to determine what date the Tooth Chart should display.  Each unique date will have a tick on the time bar.</summary>
		private List<DateTime> _listProcDates;
		///<summary>A subset of DataSetMain.  The procedures with valid dates that were skipped when drawing the graphical toothchart</summary>
		private List<DataRow> _listProcsSkipped;
		///<summary>A copy of ProcList used to revert list of DataRows back to normal ChartModule after switching to IsTpCharting view.</summary>
		private List<DataRow> _listDataRowsProcsOrig;
		///<summary>A subset of DataSetMain.  The procedures that need to be drawn in the graphical tooth chart.</summary>
		private List<DataRow> _listDataRowsProcsForGraphical;
		///<summary>The rows in the prog notes table that matched the last time we searched.</summary>
		private List<DataRow> _listSearchResults=new List<DataRow>();
		///<summary>Used to cache the selected AptNums of the items in the main grid, to reselect them after a refresh.</summary>
		private List<long> _listSelectedAptNums=new List<long>();
		///<summary>This gets lazy loaded from db the first time a user clicks to add a procedure.  That way, it can be reused a few times for each new procedure added without going back to db.  Needed in Procedures.ComputeEstimates and Fees.GetListFromObjects.</summary>
		private List<SubstitutionLink> _listSubstitutionLinks;
		///<summary>Deep copy of ToothInitials that is used to fill the Tooth Chart when the track bar date changes. Currently only adds / removes missing teeth due to extractions.</summary>
		private List<ToothInitial> _listToothInitialsCopy;
		///<summary>A subset of _listToothInitials containing just the text entries.</summary>
		private List<ToothInitial> _listToothInitialsText;
		///<summary>List of all TPs for the current patient.  Does not include Saved status.</summary>
		private List<TreatPlan> _listTreatPlans;
		private bool _isMouseDownOnImageSplitter;
		private int _originalImageMousePos;
		private int _pagesPrinted;
		//private Patient _patient;
		///<summary>Full path to the patient folder, including \ on the end.  Could be null if a patient folder could not be created / found.</summary>
		private string _patFolder;
		///<summary>Gets updated to PatCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		///<summary>Used for MenuItemPopup() to tell which row the user clicked on.  Currently only for gridPtInfo</summary>
		private Point _pointLastClicked;
		///<summary>The previous text we used when searching prog notes. Used in optimizing the search results</summary>
		private string _previousSearchText="";
		private long _prevPatNum;
		private ProcStat _procStatusNew;
		///<summary>Keeps track of which tab is selected. It's the index of the selected tab.</summary>
		private int _selectedImageTab=0;
		private int _selectedProcTab;
		///<summary>The class that controls all child control placement and anchoring based off of selected sheetDef layout.</summary>
		private SheetLayoutController _sheetLayoutController;
		private DateTime _dateTimeShowDateEnd;
		private DateTime _dateTimeShowDateStart;
		private DataTable _tablePlannedAll;
		//<summary>Thread that downloads Apteryx images.</summary>
		//private ODThread _threadImageRequest;
		private System.Windows.Forms.Timer _timerSearch;
		///<summary>This is the new Sparks3D toothChart.</summary>
		private Control toothChart;
		///<summary>Relays commands to either the old SparksToothChart.ToothChartWrapper or the new Sparks3d.ToothChart.</summary>
		private ToothChartRelay _toothChartRelay;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public ControlChart() {
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			this.toothChartWrapper = new SparksToothChart.ToothChartWrapper();//these lines were causing designer to crash
			this.toothChartWrapper.AutoFinish = false;
			this.toothChartWrapper.ColorBackground = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(145)))), ((int)(((byte)(152)))));
			this.toothChartWrapper.Cursor = System.Windows.Forms.Cursors.Default;
			this.toothChartWrapper.CursorTool = SparksToothChart.CursorTool.Pointer;
			this.toothChartWrapper.DeviceFormat = null;
			this.toothChartWrapper.DrawMode = OpenDentBusiness.DrawingMode.Simple2D;
			this.toothChartWrapper.Location = new System.Drawing.Point(0, 26);
			this.toothChartWrapper.Name = "toothChartWrapper";
			this.toothChartWrapper.PerioMode = false;
			this.toothChartWrapper.PreferredPixelFormatNumber = 0;
			this.toothChartWrapper.Size = new System.Drawing.Size(410, 307);
			this.toothChartWrapper.TabIndex = 194;
			this.toothChartWrapper.Visible = false;
			this.toothChartWrapper.SegmentDrawn += new SparksToothChart.ToothChartDrawEventHandler(this.toothChart_SegmentDrawn);
			this.toothChartWrapper.ToothSelectionsChanged += new SparksToothChart.ToothChartSelectionEventHandler(this.toothChart_ToothSelectionsChanged);
			LayoutManager.Add(toothChartWrapper,this);
			tabControlImages.DrawItem += new DrawItemEventHandler(DrawTabItem);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				//panelQuickButtons.Enabled=false;
				butBF.Text=Lan.g(this,"B/V");//vestibular instead of facial
				butV.Text=Lan.g(this,"5");
			}
			else {
				menuItemLabFee.Visible=false;
				menuItemLabFeeDetach.Visible=false;
			}
			_timerSearch=new System.Windows.Forms.Timer();
			_timerSearch.Interval=500;
			_timerSearch.Tick += _timerSearch_Tick;
			_timerSearch.Enabled=true;
			//no need to remove event handler... ContrChart always exists 1:1 per instance of the program.
			ODEvent.Fired+=ErxBrowserClosed;
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Events
		///<summary>Used when an image thumbnail is clicked at the bottom of the Chart. This event bubbles up to the form, which then calls ControlImagesJ.LaunchFloater().</summary>
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public event EventHandler<EventArgsImageClick> EventImageClick=null;
		#endregion Events

		#region Properties
		///<summary>List of tab titles for the TabProc control. Used to get accurate preview in sheet layout design view. Returns a list of one item called "Tab" if something goes wrong.</summary>
		public List<string> ListTabProcPageTitles {
			get {
				List<string> listTabTitles=tabProc.TabPages.OfType<TabPage>().Select(x => x.Text).ToList();
				if(listTabTitles.IsNullOrEmpty()) {
					listTabTitles=new List<string>() { "Tab" };
				}
				return listTabTitles;
			}
		}

		///<summary>True if the chart view allows TP Charting or "Is TP View" is checked.</summary>
		private bool IsTPChartingAvailable {
			get { return (_chartViewCurDisplay!=null && _chartViewCurDisplay.IsTpCharting) || checkTPChart.Checked; }
		}
		#endregion Properties

		#region Methods - Event Handlers - Buttons
		private void butAddKey_Click(object sender,EventArgs e) {
			RegistrationKey key=new RegistrationKey();
			key.PatNum=Pd.PatNum;
			//Notes are not commonly necessary, because most customers have only one office (thus only 1 key is necessary).
			//A tech can edit the note later after it is added if necessary.
			key.Note="";
			key.DateStarted=DateTime.Today;
			key.IsForeign=false;
			key.VotesAllotted=100;
			RegistrationKeys.Insert(key);
			FillPtInfo();//Refresh registration key list in patient info grid.
		}

		private void butBig_Click(object sender,EventArgs e) {		
		}

		private void butChartViewAdd_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			int selectedIndex=gridChartViews.GetSelectedIndex();
			using FormChartView formChartView=new FormChartView();
			formChartView.ChartViewCur=new ChartView();
			formChartView.ChartViewCur.IsNew=true;
			formChartView.ChartViewCur.ItemOrder=int.MaxValue;
			if(checkAppt.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=1;
			}
			if(checkComm.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=2;
			}
			if(checkCommFamily.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=4;
			}
			if(checkCommSuperFamily.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=256;
			}
			if(checkTasks.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=8;
			}
			if(checkEmail.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=16;
			}
			if(checkLabCase.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=32;
			}
			if(checkRx.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=64;
			}
			if(checkSheets.Checked) {
				formChartView.ChartViewCur.ObjectTypes+=128;
			}
			if(checkShowTP.Checked) {
				formChartView.ChartViewCur.ProcStatuses+=1;
			}
			if(checkShowC.Checked) {
				formChartView.ChartViewCur.ProcStatuses+=2;
			}
			if(checkShowE.Checked) {
				formChartView.ChartViewCur.ProcStatuses+=4;
			}
			if(checkShowR.Checked) {
				formChartView.ChartViewCur.ProcStatuses+=16;
			}
			if(checkShowCn.Checked) {
				formChartView.ChartViewCur.ProcStatuses+=64;
			}
			if(formChartView.ChartViewCur.IsNew) {
				formChartView.ChartViewCur.IsTpCharting=true;//default to TP view for new chart views
			}
			formChartView.ChartViewCur.SelectedTeethOnly=checkShowTeeth.Checked;
			formChartView.ChartViewCur.ShowProcNotes=checkNotes.Checked;
			formChartView.ChartViewCur.IsAudit=checkAudit.Checked;
			formChartView.ShowDialog();
			FillChartViewsGrid();
			if(_isDistributorKey) {
				FillCustomerViewsGrid();
			}
			int index=_listChartViews.Count-1;
			if(index<0) { //If there are no chart views, then we do not set a selected chart view, simply return.
				return;
			}
			if(formChartView.DialogResult==DialogResult.Cancel) {
				if(selectedIndex.Between(0,index)) { //if the user cancelled, i.e. did not add a chart, set to the originally selected value
					index=selectedIndex;
				}
				else { 
					index=0;//if the originally selected index is no longer a valid index, set to the first chart view.
				}
			}
			gridChartViews.SetSelected(index,true);
			if(_isDistributorKey) {
				gridCustomerViews.SetSelected(index,true);
			}
			SetChartView(_listChartViews[index]);
		}

		private void butChartViewDown_Click(object sender,EventArgs e) {
			MoveChartView(Direction.Down);
		}

		private void butChartViewUp_Click(object sender,EventArgs e) {
			MoveChartView(Direction.Up);
		}

		private void MoveChartView(Direction direction) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			if(gridChartViews.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a view first.");
				return;
			}
			if(gridChartViews.GetSelectedIndex()!=-1) {
				int oldIdx=gridChartViews.GetSelectedIndex();
				int newIdx=oldIdx+(int)direction;
				SwapChartViews(newIdx,oldIdx);
			}
		}

		private enum Direction {
			Up=-1,
			Down=1,
		}

		private void SwapChartViews(int newIdx,int oldIdx) {
			if(!newIdx.Between(0,_listChartViews.Count-1)) {
				return;//can't move up or down any more
			}
			ChartView chartViewTemp=_listChartViews[newIdx];
			_listChartViews[newIdx]=_listChartViews[oldIdx];
			_listChartViews[oldIdx]=chartViewTemp;
			_listChartViews[newIdx].ItemOrder=newIdx;
			_listChartViews[oldIdx].ItemOrder=oldIdx;
			ChartViews.Update(_listChartViews[newIdx]);
			ChartViews.Update(_listChartViews[oldIdx]);
			FillChartViewsGrid();
			gridChartViews.SetSelected(newIdx,true);
			if(_isDistributorKey) {
				FillCustomerViewsGrid();
				gridCustomerViews.SetSelected(newIdx,true);
			}
			SetChartView(_listChartViews[newIdx]);
		}

		private void butErxAccess_Click(object sender,EventArgs e) {
			using FormErxAccess formErxAccess=new FormErxAccess(Pd.Patient);
			formErxAccess.ShowDialog();
		}

		private void butForeignKey_Click(object sender,EventArgs e) {
			RegistrationKey key=new RegistrationKey();
			key.PatNum=Pd.PatNum;
			key.Note="";
			key.DateStarted=DateTime.Today;
			key.IsForeign=true;
			key.VotesAllotted=100;
			RegistrationKeys.Insert(key);
			FillPtInfo();
		}

		private void butLoadDirectX_Click(object sender,EventArgs e) {
			//toothChart.LoadDirectX();
		}

		private void butNewTP_Click(object sender,EventArgs e) {
			using FormTreatPlanCurEdit formTPCE=new FormTreatPlanCurEdit();
			formTPCE.TreatPlanCur=new TreatPlan() {
				Heading="Inactive Treatment Plan",
				Note=PrefC.GetString(PrefName.TreatmentPlanNote),
				PatNum=Pd.PatNum,
				TPStatus=TreatPlanStatus.Inactive,
			};
			formTPCE.ShowDialog();
			if(formTPCE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillTreatPlans();
			_listTreatPlans.ForEach(x => gridTreatPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(y => y.TreatPlanNum==x.TreatPlanNum)),
				formTPCE.TreatPlanCur.TreatPlanNum==x.TreatPlanNum));
			if(gridTreatPlans.GetSelectedIndex()>-1) {
				gridTreatPlans.ScrollToIndex(gridTreatPlans.GetSelectedIndex());
			}
			ModuleSelected(Pd.PatNum);//refreshes TPs
		}

		private void butPhoneNums_Click(object sender,EventArgs e) {
			if(FormOpenDental.PatNumCur==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			using FormPhoneNumbersManage formPNM=new FormPhoneNumbersManage();
			formPNM.PatNum=FormOpenDental.PatNumCur;
			formPNM.ShowDialog();			
		}

		private void butShowDateRange_Click(object sender,EventArgs e) {
			using FormChartViewDateFilter formCVDF=new FormChartViewDateFilter();
			formCVDF.DateStart=_dateTimeShowDateStart;
			formCVDF.DateEnd=_dateTimeShowDateEnd;
			formCVDF.ShowDialog();
			if(formCVDF.DialogResult!=DialogResult.OK) {
				return;
			}
			_dateTimeShowDateStart=formCVDF.DateStart;
			_dateTimeShowDateEnd=formCVDF.DateEnd;
			if(gridChartViews.ListGridRows.Count>0) {//enable custom view label
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true; 
			FillDateRange();
			FillProgNotes();
		}
		#endregion Methods - Event Handlers - Buttons

		#region Methods - Event Handlers - ChartViews Clicked
		private void ChartViewsCellClicked(ODGridClickEventArgs e) {
			SetChartView(_listChartViews[e.Row]);
			gridChartViews.SetSelected(e.Row,true);
			if(_isDistributorKey) {
				gridCustomerViews.SetSelected(e.Row,true);
			}
			RefreshModuleScreen(false);//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();//Changes the progress notes grid width, which changes the layout.
		}

		private void ChartViewsDoubleClicked(ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormChartView formChartView=new FormChartView();
			formChartView.ChartViewCur=_listChartViews[e.Row];
			formChartView.ShowDialog();
			FillChartViewsGrid();
			if(_isDistributorKey) {
				FillCustomerViewsGrid();
			}
			int count=_listChartViews.Count;
			if(count==0) {
				FillProgNotes();
				return;//deleted last view, so display default
			}
			if(formChartView.ChartViewCur.ItemOrder!=0 && e.Row < count) {
				gridChartViews.SetSelected(e.Row,true);
				SetChartView(_listChartViews[e.Row]);
			}
			else {
				gridChartViews.SetSelected(0,true);
				SetChartView(_listChartViews[0]);
			}
			if(_isDistributorKey) {
				gridCustomerViews.SetSelected(gridChartViews.GetSelectedIndex(),true);
			}
			RefreshModuleScreen(false);//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();
		}

		private bool SyncChartViewItemOrders() {
			bool isChanged=false;
			for(int i=0;i<_listChartViews.Count;i++) {
				ChartView chartView=_listChartViews[i];
				ChartView chartViewOld=chartView.Copy();
				chartView.ItemOrder=i;
				isChanged|=ChartViews.Update(chartView,chartViewOld);
			}
			return isChanged;
		}
		#endregion Methods - Event Handlers - ChartViews Clicked

		#region Methods - Event Handlers - Checkboxes
		private void checkToday_CheckedChanged(object sender,EventArgs e) {
			if(checkToday.Checked) {
				textDate.Text=DateTime.Today.ToShortDateString();
			}
			else {
				//
			}
		}

		private void checkTPChart_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			if(_chartViewCurDisplay!=null) {
				_chartViewCurDisplay.IsTpCharting=((CheckBox)sender).Checked;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		///<summary>Uses the UserODPref to store ShowAutomatedCommlog separately from the chart module.</summary>
		private void checkShowCommAuto_Click(object sender,EventArgs e) {
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetFirstOrNewByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog);
			userOdPrefShowAutoCommlog.ValueString=POut.Bool(checkShowCommAuto.Checked);
			UserOdPrefs.Upsert(userOdPrefShowAutoCommlog);
			DataValid.SetInvalid(InvalidType.UserOdPrefs);
			if(!_isModuleSelected) {
				return;
			}
			ModuleSelected(Pd.PatNum);
		}
		#endregion Methods - Event Handlers - Checkboxes

		#region Methods - Event Handlers - ErxBrowsers
		///<summary>CRITICAL: If we ever decide to launch eRx in an eternal browser window again, then we will need another way to sync the medications from eRx.  If we use an external browser window, then we have no way to know when the user is done with the exernal browser, and therefore we would not know when to sync.  Currently this event function knows when the browser closes, so we know when to sync.</summary>
		private void ErxBrowserClosed(ODEventArgs e) {
			if(e.EventType!=ODEventType.ErxBrowserClosed) {
				return;
			}
			Patient pat=(Patient)e.Tag;
			if(pat==null) {
				return;//A subwindow of FormErx was opened due to a link being clicked into a new window instance.
			}
			//Remove the FormErx session from the dictionary so that another FormErx can be opened for this patient if the user tries again.
			_dictFormErxSessions.Remove(pat.PatNum);
			if(IsPatientNull() || Pd.PatNum!=pat.PatNum) {
				return;//FormErx was closed for another patient.
			}
			//Refresh prescriptions from NewCrop, since the user probably just added at least one.
			this.Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			if(NewCropRefreshPrescriptions()) {
				ModuleSelected(Pd.PatNum);
			}
			RefreshDoseSpotNotifications();
			this.Cursor=Cursors.Default;
		}
		#endregion Methods - Event Handlers - ErxBrowsers

		#region Methods - Event Handlers - Forms
		/// <summary>Event handler for closing FormExamSheets when it is non-modal.</summary>
		private void FormExamSheets_FormClosing(object sender,FormClosingEventArgs e) {
			long formPatNum=((FormExamSheets)sender).PatNum;
			if(!IsPatientNull() && Pd.PatNum==formPatNum) { //Only refresh if we have a patient selected and are currently on the chart module matching this exam sheet.
				Pd.TableProgNotes=ChartModules.GetProgNotes(formPatNum,checkAudit.Checked,GetChartModuleComponents());
				RefreshModuleScreen();
			}
		}

		/// <summary>Event handler for closing FormSheetFillEdit when it is non-modal.</summary>
		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			FormSheetFillEdit formSFE=((FormSheetFillEdit)sender);
			if(formSFE.DialogResult==DialogResult.OK && !IsPatientNull()) {
				//If the user deleted the sheet, forcefully refresh the chart module regardless of what patient is selected.
				//Otherwise; only refresh the chart module if the same patient is selected.
				if(formSFE.SheetCur==null || formSFE.SheetCur.PatNum==Pd.PatNum) {
					ModuleSelected(Pd.PatNum);
				}
			}
		}
		#endregion Methods - Event Handlers - Forms

		#region Methods - Event Handlers - Grids
		private void gridChartViews_CellClick(object sender,ODGridClickEventArgs e) {
			ChartViewsCellClicked(e);
		}

		private void gridChartViews_DoubleClick(object sender,ODGridClickEventArgs e) {
			ChartViewsDoubleClicked(e);
		}

		private void gridCustomerViews_CellClick(object sender,ODGridClickEventArgs e) {
			ChartViewsCellClicked(e);
		}

		private void gridCustomerViews_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ChartViewsDoubleClicked(e);
		}

		private void gridProg_CellClick(object sender,ODGridClickEventArgs e) {
			DataTable tableProgNotes=Pd.TableProgNotes;
			//DataRow rowClicked=progNotes.Rows[e.Row];
			DataRow rowClicked=(DataRow)gridProg.ListGridRows[e.Row].Tag;
			long procNum=PIn.Long(rowClicked["ProcNum"].ToString());
			if(procNum==0) {//if not a procedure
				return;
			}
			long codeNum=PIn.Long(rowClicked["CodeNum"].ToString());
			if(ProcedureCodes.GetStringProcCode(codeNum,doThrowIfMissing:false)!=ProcedureCodes.GroupProcCode) {//if not a group note
				return;
			}
			List<ProcGroupItem> listGroupItems=ProcGroupItems.GetForGroup(procNum);
			//for(int i=0;i<progNotes.Rows.Count;i++){
			for(int i=0;i<gridProg.ListGridRows.Count;i++) {
				DataRow row=(DataRow)gridProg.ListGridRows[i].Tag;
				if(row["ProcNum"].ToString()=="0") {
					continue;
				}
				long procNum2=PIn.Long(row["ProcNum"].ToString());
				for(int j=0;j<listGroupItems.Count;j++) {
					if(procNum2==listGroupItems[j].ProcNum) {
						gridProg.SetSelected(i,true);
					}
				}
			}
		}

		private void gridProg_ColumnSortClick(object sender,EventArgs e) {
			FillProgNotes(hasPageNav:false);
		}

		private void gridProg_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right) {
				//This hook was here before we changed this method to be MouseDown instead of MouseUp
				Plugins.HookAddCode(this,"ContrChart.gridProg_MouseUp_end",menuProgRight,gridProg,Pd.Patient);
			}
		}

		private void gridPtInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(Plugins.HookMethod(this,"ContrChart.gridPtInfo_CellDoubleClick",Pd.Patient,Pd.Family,e,Pd.PatientNote)) {
				return;
			}
			if(TerminalActives.PatIsInUse(Pd.PatNum)) {
				MsgBox.Show(this,"Patient is currently entering info at a reception terminal.  Please try again later.");
				return;
			}
			if(gridPtInfo.ListGridRows[e.Row].Tag!=null && gridPtInfo.ListGridRows[e.Row].Tag.ToString()!="DOB") {
				if(new[] { "tabMedical","tabProblems","tabMedications","tabAllergies","tabTobaccoUse" }.Contains(gridPtInfo.ListGridRows[e.Row].Tag.ToString())) {
					using FormMedical formMedical=new FormMedical(Pd.PatientNote,Pd.Patient,gridPtInfo.ListGridRows[e.Row].Tag.ToString());
					formMedical.ShowDialog();
					ModuleSelected(Pd.PatNum);
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.GetType()==typeof(RegistrationKey)) {
					using FormRegistrationKeyEdit formRKE=new FormRegistrationKeyEdit();
					formRKE.RegKey=(RegistrationKey)gridPtInfo.ListGridRows[e.Row].Tag;
					formRKE.ShowDialog();
					FillPtInfo();
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="EhrProvKeys") {
					using FormEhrProvKeysCustomer formEhrPKC=new FormEhrProvKeysCustomer();
					formEhrPKC.Guarantor=Pd.Patient.Guarantor;
					formEhrPKC.ShowDialog();
					ModuleSelected(Pd.PatNum);
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="Referral") {
					//RefAttach refattach=(RefAttach)gridPat.Rows[e.Row].Tag;
					using FormReferralsPatient formRP=new FormReferralsPatient();
					formRP.PatNum=Pd.PatNum;
					formRP.ShowDialog();
					ModuleSelected(Pd.PatNum);
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="References") {
					using FormReference formReference=new FormReference();
					formReference.ShowDialog();
					if(formReference.GotoPatNum!=0) {
						Patient pat=Patients.GetPat(formReference.GotoPatNum);
						FormOpenDental.S_Contr_PatientSelected(pat,false);
						GotoModule.GotoFamily(formReference.GotoPatNum);
						return;
					}
					if(formReference.DialogResult!=DialogResult.OK) {
						return;
					}
					for(int i=0;i<formReference.SelectedCustRefs.Count;i++) {
						CustRefEntry custRefEntry=new CustRefEntry();
						custRefEntry.DateEntry=DateTime.Now;
						custRefEntry.PatNumCust=Pd.PatNum;
						custRefEntry.PatNumRef=formReference.SelectedCustRefs[i].PatNum;
						CustRefEntries.Insert(custRefEntry);
					}
					FillPtInfo();
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="Patient Portal") {
					using FormPatientPortal formPP=new FormPatientPortal(Pd.Patient);
					formPP.ShowDialog();
					if(formPP.DialogResult==DialogResult.OK) {
						FillPtInfo();
					}
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="Payor Types") {
					using FormPayorTypes formPayorTypes=new FormPayorTypes();
					formPayorTypes.PatCur=Pd.Patient;
					formPayorTypes.ShowDialog();
					if(formPayorTypes.DialogResult==DialogResult.OK) {
						FillPtInfo();
					}
					return;
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.ToString()=="Broken Appts") {					
					return;//This row is just for display; it can't be edited.
				}
				if(gridPtInfo.ListGridRows[e.Row].Tag.GetType()==typeof(CustRefEntry)) {
					using FormReferenceEntryEdit formREE=new FormReferenceEntryEdit((CustRefEntry)gridPtInfo.ListGridRows[e.Row].Tag);
					formREE.ShowDialog();
					FillPtInfo();
					return;
				}
				else if(gridPtInfo.ListGridRows[e.Row].Tag is PatFieldDef patFieldDef) {//patfield for an existing PatFieldDef
					Pd.FillIfNeeded(EnumPdTable.PatField);
					PatField patField=Pd.ListPatFields.Find(x=>x.FieldName==patFieldDef.FieldName);
					PatFieldL.OpenPatField(patField,patFieldDef,Pd.PatNum);
				}
				else if(gridPtInfo.ListGridRows[e.Row].Tag is PatField patField) {//PatField for a PatFieldDef that no longer exists
					using FormPatFieldEdit formPatFieldEdit=new FormPatFieldEdit(patField);
					formPatFieldEdit.ShowDialog();
				}
			}
			else {
				if(!Security.IsAuthorized(Permissions.PatientEdit)) {
					return;
				}
				using FormPatientEdit formPatientEdit=new FormPatientEdit(Pd.Patient,Pd.Family);
				formPatientEdit.IsNew=false;
				formPatientEdit.ShowDialog();
				if(formPatientEdit.DialogResult==DialogResult.OK) {
					FormOpenDental.S_Contr_PatientSelected(Pd.Patient,false);
				}
			}
			ModuleSelected(Pd.PatNum);
		}

		private void gridPtInfo_MouseDown(object sender,MouseEventArgs e) {
			_pointLastClicked=e.Location;
		}

		private void gridTpProcs_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridTpProcs.ListGridRows[e.Row].Tag==null) {
				return;//clicked on header row
			}
			ProcTP procTpCur=(ProcTP)gridTpProcs.ListGridRows[e.Row].Tag;
			Procedure procCur=Procedures.GetOneProc(procTpCur.ProcNumOrig,true);
			List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(Pd.PatNum);
			//generate a new loop list containing only the procs before this one in it
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			for(int i=0;i<_listProceduresTPs.Count;i++) {
				if(_listProceduresTPs[i].ProcNum==procCur.ProcNum) {
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,_listProceduresTPs[i].ProcNum,_listProceduresTPs[i].CodeNum));
			}
			using FormProcEdit formProcEdit=new FormProcEdit(procCur,Pd.Patient,Pd.Family,listPatToothInitials:Pd.ListToothInitials);
			formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			formProcEdit.ListClaimProcHists=Pd.ListClaimProcHists;
			formProcEdit.ShowDialog();
			List<long> listSelectedTpNums=gridTreatPlans.SelectedIndices.Select(x => _listTreatPlans[x].TreatPlanNum).ToList();
			RefreshModuleData(Pd.PatNum,true);
			FillProgNotes();
			gridTreatPlans.SetAll(false);
			listSelectedTpNums.ForEach(x => gridTreatPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(y => y.TreatPlanNum==x)),true));
			FillTpProcs();
			for(int i=0;i<gridTpProcs.ListGridRows.Count;i++) {
				if(gridTpProcs.ListGridRows[i].Tag==null) {
					continue;
				}
				ProcTP procTp=(ProcTP)gridTpProcs.ListGridRows[i].Tag;
				gridTpProcs.SetSelected(i,(procTp.ProcNumOrig==procTpCur.ProcNumOrig && procTp.TreatPlanNum==procTpCur.TreatPlanNum));
			}
		}

		private void gridTreatPlans_CellClick(object sender,ODGridClickEventArgs e) {
			gridTpProcs.SetAll(false);
			FillTpProcs();
			FillToothChart(false);
		}

		private void gridTreatPlans_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			TreatPlan treatPlanSelected=_listTreatPlans[e.Row];
			using FormTreatPlanCurEdit formTPCE=new FormTreatPlanCurEdit();
			formTPCE.TreatPlanCur=treatPlanSelected;
			formTPCE.ShowDialog();
			if(formTPCE.DialogResult!=DialogResult.OK) {
				return;
			}
			FillTreatPlans();
			_listTreatPlans.ForEach(x => gridTreatPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(y => y.TreatPlanNum==x.TreatPlanNum)),
				formTPCE.TreatPlanCur.TreatPlanNum==x.TreatPlanNum));
			if(gridTreatPlans.GetSelectedIndex()>-1) {
				gridTreatPlans.ScrollToIndex(gridTreatPlans.GetSelectedIndex());
			}
			ModuleSelected(Pd.PatNum);
		}
		#endregion Methods - Event Handlers - Grids

		#region Methods - Event Handlers - LayoutMenuItems
		private void LayoutMenuItem_Click(object sender,EventArgs e) {
			MenuItem menuItem=(sender as MenuItem);
			if(menuItem.Tag is SheetDef sheetDef) {
				menuItem.Parent.MenuItems.OfType<MenuItem>().ForEach(x => x.Checked=(x==menuItem));
				_sheetLayoutController.InitLayoutForSheetDef(sheetDef,LayoutSheet_GetMode(),LayoutSheet_GetControlDict(),isUserSelection:true);
				LayoutControls();
			}
			else {
				Tool_Layout_Click();
			}
		}
		#endregion Methods - Event Handlers - LayoutMenuItems

		#region Methods - Event Handlers - Lists
		private void listCommonProcs_MouseDown(object sender,MouseEventArgs e) {
			if(listCommonProcs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a procedure.");
				return;
			}
			string procCode="";
			double procFee=0;
			//Hard coded internal procedures.
			switch(listCommonProcs.SelectedIndex) {
				case 0://Monthly Maintenance
					procCode="001";
					procFee=149;
					break;
				case 1://Monthly Mobile
					procCode="027";
					procFee=10;
					break;
				case 2://Monthly E-Mail Support
					procCode="008";
					procFee=89;
					break;
				case 3://Monthly EHR
					procCode="029";
					break;
				case 4://Data Conversion
					procCode="005";
					procFee=700;
					break;
				case 5://Trial Conversion
					procCode="N5641";
					break;
				case 6://Demo
					procCode="018";
					break;
				case 7://Online Training
					procCode="N1254";
					break;
				case 8://Additional Online Training
					procCode="N8989";
					procFee=50;
					break;
				case 9://eCW Online Training
					procCode="eCW1";
					break;
				case 10://eCW Installation Verification
					procCode="eCW2";
					break;
				case 11://Programming
					procCode="007";
					break;
				case 12://Query Programming
					procCode="023";
					procFee=90;
					break;
			}
			//Simply add the procedure to the customers account.
			Procedure proc=new Procedure();
			proc.CodeNum=ProcedureCodes.GetCodeNum(procCode);
			proc.DateEntryC=DateTime.Today;
			proc.PatNum=Pd.PatNum;
			proc.ProcDate=DateTime.Now;
			proc.DateTP=DateTime.Now;
			proc.ProcFee=procFee;
			proc.ProcStatus=ProcStat.TP;
			proc.ProvNum=PrefC.GetLong(PrefName.PracticeDefaultProv);
			proc.MedicalCode=ProcedureCodes.GetProcCode(proc.CodeNum).MedicalCode;
			proc.BaseUnits=ProcedureCodes.GetProcCode(proc.CodeNum).BaseUnits;
			proc.PlaceService=Clinics.GetPlaceService(proc.ClinicNum);
			Procedures.Insert(proc);//no recall synch needed because dental offices don't use this feature
			listCommonProcs.SelectedIndex=-1;
			FillProgNotes();
		}

		///<summary>Updates priority of all selected procedures to the selected priority.</summary>
		private void listPriorities_MouseClick(object sender,MouseEventArgs e) {
			int clickedRow=listPriorities.IndexFromPoint(e.Location);
			if(clickedRow==-1) {
				return;//nothing clicked, do nothing.
			}
			List<long> listSelectedTpNums=new List<long>();
			listSelectedTpNums.AddRange(gridTreatPlans.SelectedIndices.Select(x => _listTreatPlans[x].TreatPlanNum));
			//Priority of Procedures is dependent on which TP it is attached to. Track selected procedures by TPNum and ProcNum
			List<Tuple<long,long>> listSelectedTpNumProcNums=new List<Tuple<long,long>>();
			listSelectedTpNumProcNums.AddRange(gridTpProcs.SelectedIndices.Where(x => gridTpProcs.ListGridRows[x].Tag!=null).Select(x => (ProcTP)gridTpProcs.ListGridRows[x].Tag)
				.Select(x => new Tuple<long,long>(x.TreatPlanNum,x.ProcNumOrig)));
			List<TreatPlanAttach> listAllTpAttaches=gridTreatPlans.SelectedIndices.ToList().SelectMany(x => (List<TreatPlanAttach>)gridTreatPlans.ListGridRows[x].Tag).ToList();
			for(int i=0;i<gridTpProcs.SelectedIndices.Length;i++){
			//foreach(int selectedIdx in gridTpProcs.SelectedIndices) {
				if(gridTpProcs.ListGridRows[gridTpProcs.SelectedIndices[i]].Tag==null) {
					continue;//must be a header row.
				}
				ProcTP procTpCur=(ProcTP)gridTpProcs.ListGridRows[gridTpProcs.SelectedIndices[i]].Tag;
				TreatPlanAttach treatPlanAttach=listAllTpAttaches.FirstOrDefault(x => x.ProcNum==procTpCur.ProcNumOrig && x.TreatPlanNum==procTpCur.TreatPlanNum);
				if(treatPlanAttach==null) {
					continue;//should never happen.
				}
				treatPlanAttach.Priority=0;
				if(clickedRow>0) {//Not 'no priority'
					treatPlanAttach.Priority=(listPriorities.Items.GetObjectAt(clickedRow) as Def).DefNum;
				}
			}
			listAllTpAttaches.Select(x => x.TreatPlanNum).Distinct().ToList()
				.ForEach(x => TreatPlanAttaches.Sync(listAllTpAttaches.FindAll(y => y.TreatPlanNum==x),x));//sync each TP seperately
			TreatPlanType treatPlanTypeCur=DiscountPlanSubs.HasDiscountPlan(Pd.PatNum)?TreatPlanType.Discount:TreatPlanType.Insurance;
			TreatPlans.AuditPlans(Pd.PatNum,treatPlanTypeCur);//consider adding logic here to update active plan priorities instead of calling the entire AuditPlans function
			listSelectedTpNums.ForEach(x => gridTreatPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(y => y.TreatPlanNum==x)),true));
			FillTpProcs();
			//Reselect TPs and Procs.
			for(int i=0;i<gridTpProcs.ListGridRows.Count;i++) {
				if(gridTpProcs.ListGridRows[i].Tag==null) {
					continue;
				}
				ProcTP procTpCur=(ProcTP)gridTpProcs.ListGridRows[i].Tag;
				gridTpProcs.SetSelected(i,listSelectedTpNumProcNums.Contains(new Tuple<long,long>(procTpCur.TreatPlanNum,procTpCur.ProcNumOrig)));
			}
		}

		private void listProcStatusCodes_MouseUp(object sender,MouseEventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void listViewImages_DoubleClick(object sender, EventArgs e) {
			if(listViewImages.SelectedIndices.Count==0) {
				return;//clicked on white space.
			}
			/*
			ApteryxImage apteryxImg=listViewImages.SelectedItems[0].Tag as ApteryxImage;
			if(apteryxImg!=null) {//can be other images or documents here. XVWeb downloads have tags to store info.
				string text=listViewImages.SelectedItems[0].Text;
				Bitmap bitmapApiImage=null;
				double fileSizeMB=(double)apteryxImg.FileSize/1024/1024;
				using FormProgress formProgress=new FormProgress(maxVal:fileSizeMB);
				formProgress.DisplayText="?currentVal MB of ?maxVal MB copied";
				ODThread ODThreadGetBitmap=new ODThread(new ODThread.WorkerDelegate((o) => {
					bitmapApiImage=XVWeb.GetBitmap(apteryxImg,formProgress);
				}));
				ODThreadGetBitmap.Name="DownloadApteryxImage"+apteryxImg.Id;
				ODThreadGetBitmap.Start(true);
				//display the progress dialog to the user:
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					ODThreadGetBitmap.QuitAsync();
					return;
				}
				ODThreadGetBitmap.Join(2000);//give thread some time to finish before trying to display the image.
				if(_formImageViewer==null || !_formImageViewer.Visible) {
					_formImageViewer=new FormImageViewer();
					_formImageViewer.Show();
				}
				if(_formImageViewer.WindowState==FormWindowState.Minimized) {
					_formImageViewer.WindowState=FormWindowState.Normal;
				}
				_formImageViewer.BringToFront();
				_formImageViewer.SetImage(bitmapApiImage,text);
				//if they want to save in the db & image doesn't already exist
				Document docSavedImage=XVWeb.SaveApteryxImageToDoc(apteryxImg,bitmapApiImage,_patCur);
				if(docSavedImage!=null) {
					listViewImages.SelectedItems[0].Tag=null;
					_listApteryxThumbnails.Remove(_listApteryxThumbnails.Find(x => x.Image.Id==apteryxImg.Id));//So that this image is not displayed twice
					int docListIndex=_arrayDocuments.GetLength(0);
					Array.Resize(ref _arrayDocuments,docListIndex+1);
					_arrayDocuments[docListIndex]=Documents.GetByNum(docSavedImage.DocNum);
					FillImages();
				}
				bitmapApiImage.Dispose();
				return;
			}*/
			ImageInfo imageInfo=_listImageInfos[listViewImages.SelectedIndices[0]];
			if(imageInfo.MountNum==0) {//Not a mount
				Document document=Pd.ListDocuments.Find(x=>x.DocNum==imageInfo.DocNum);
				if(document is null) {
					return;
				}
				if(!ImageHelper.HasImageExtension(document.FileName)) {//Not a picture file, try to launch the process
					try {
						FileAtoZ.StartProcess(ODFileUtils.CombinePaths(_patFolder,document.FileName));
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
					}
					return;
				}
			}
			EventImageClick?.Invoke(this,new EventArgsImageClick(Pd.PatNum,imageInfo.MountNum,imageInfo.DocNum));
			/*
			[(int)_arrayListVisImages[listViewImages.SelectedIndices[0]]];
			if(_formImageViewer==null || !_formImageViewer.Visible) {
				_formImageViewer=new FormImageViewer();
				_formImageViewer.Show();
			}
			if(_formImageViewer.WindowState==FormWindowState.Minimized) {
				_formImageViewer.WindowState=FormWindowState.Normal;
			}
			_formImageViewer.BringToFront();
			_formImageViewer.SetImage(document,_patCur.GetNameLF()+" - "
				+document.DateCreated.ToShortDateString()+": "+document.Description);*/
		}

		private void listViewImages_ItemMouseHover(object sender,ListViewItemMouseHoverEventArgs e) {
			Cursor=Cursors.Default;
		}
		#endregion Methods - Event Handlers - Lists

		#region Methods - Event Handlers - Menu Items
		private void menuItemChartBig_Click(object sender,EventArgs e) {
			//Check for patient because the tooth chart will be expecting data from a patient's chart.
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			FormToothChartingBig formToothChartingBig=new FormToothChartingBig(checkShowTeeth.Checked);
			formToothChartingBig.ListToothInitials=Pd.ListToothInitials;
			formToothChartingBig.ListDataRowsProcs=_listDataRowsProcsForGraphical;
			formToothChartingBig.ListOrthoHardwares=Pd.ListOrthoHardwares;
			formToothChartingBig.PatCur=Pd.Patient.Copy();
			formToothChartingBig.ListAppts=Pd.ListAppointments;
			formToothChartingBig.IsOrthoMode=checkOrthoMode.Checked;
			formToothChartingBig.Show();
		}

		private void menuItemChartSave_Click(object sender,EventArgs e) {
			//Check for patient because the tooth chart will be expecting data from a patient's chart.
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			long defNum=Defs.GetImageCat(ImageCategorySpecial.T);
			if(defNum==0) {//no category set for Tooth Charts.
				MessageBox.Show(Lan.g(this,"No Def set for Tooth Charts."));
				return;
			}
			Bitmap bitmapChart=null;
			try {
				//If current user is a provider with chart color preference turned on, we need to recolor the toothchart so that this isn't taken
				// into consideration when generating a printout. Then immediately switch it back.
				if(_toothChartRelay.HasToothColorUserPref()) {
					DrawProcGraphics(true);
					bitmapChart=_toothChartRelay.GetBitmap();
					DrawProcGraphics(false);
				}
				else {
					bitmapChart=_toothChartRelay.GetBitmap();
				}
				ImageStore.Import(bitmapChart,defNum,ImageType.Photo,Pd.Patient);
			}
			catch(SharpDXException sdxEx) {
				using MsgBoxCopyPaste errorMsg=new MsgBoxCopyPaste(Lan.g(this,"Failed to capture tooth chart image from graphics card. \r\n"
					+"Please contact support to help with graphics troubleshooting:\r\n")
					+sdxEx.Message+"\r\n"
					+sdxEx.StackTrace
				);
				errorMsg.ShowDialog();
				return;
			}
			catch(Exception ex) {
				MessageBox.Show(Lan.g(this,"Unable to save file: ")+ex.Message);
				return;
			}
			finally {//Executes regardles of above returns in the catches, "Saved." msgbox will not show.
				if(bitmapChart!=null) {
					bitmapChart.Dispose();
					bitmapChart=null;
				}
			}
			MsgBox.Show(this,"Saved.");
		}

		private void menuItemDelete_Click(object sender,EventArgs e) {
			DeleteRows();
		}

		private void menuItemDoseSpotPendingPescr_Click(object sender,EventArgs e) {
			Tool_eRx_Click(true);
		}

		private void menuItemDoseSpotRefillReqs_Click(object sender,EventArgs e) {
			Tool_eRx_Click(true);
		}

		private void menuItemDoseSpotTransactionErrors_Click(object sender,EventArgs e) {
			Tool_eRx_Click(true);
		}

		private void menuItemEditSelected_Click(object sender,EventArgs e) {
			if(gridProg.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select procedures first.");
				return;
			}
			DataRow row;
			List<Procedure> listProcs=new List<Procedure>();
			for(int i=0;i<gridProg.SelectedIndices.Length;i++) {
				row=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[i]].Tag;
				if(!CanEditRow(row,doCheckDb:true,isSilent:false,listProcs)) {
					return;
				}
			}
			using FormProcEditAll formProcEditAll=new FormProcEditAll();
			formProcEditAll.ProcList=listProcs;
			formProcEditAll.ShowDialog();
			if(formProcEditAll.DialogResult==DialogResult.OK) {
				ModuleSelected(Pd.PatNum);
			}
		}

		///<summary>Manuall refresh prescriptions from eRx.</summary>
		private void menuItemErxRefresh_Click(object sender,EventArgs e) {
			this.Cursor=Cursors.WaitCursor;
			Application.DoEvents();
			if(NewCropRefreshPrescriptions()) {
				ModuleSelected(Pd.PatNum);
			}
			RefreshDoseSpotNotifications();
			this.Cursor=Cursors.Default;
		}

		private void menuItemGroupMultiVisit_Click(object sender,EventArgs e) {
			FillProgNotes(retainToothSelection:true);//Refresh to ensure data is not stale before grouping.
			if(gridProg.SelectedIndices.Length==0) {
				return;
			}
			List<Procedure> listProcs=new List<Procedure>();
			foreach(int i in gridProg.SelectedIndices) {
				DataRow row=(DataRow)gridProg.ListGridRows[i].Tag;
				if(!CanGroupMultiVisit(row,doCheckDb:true,isSilent:false)) {
					return;
				}
				Procedure proc=new Procedure();
				proc.ProcNum=PIn.Long(row["ProcNum"].ToString());
				proc.ProcStatus=PIn.Enum<ProcStat>(row["ProcStatus"].ToString());
				proc.PatNum=Pd.PatNum;
				listProcs.Add(proc);
			}
			ProcMultiVisits.CreateGroup(listProcs);
			Pd.ClearAndFill(EnumPdTable.ProcMultiVisit);
			FillProgNotes(retainToothSelection:true,doRefreshData:false);//Refresh to show potential status change.
			MsgBox.Show(this,"Done.");
		}

		private void menuItemGroupSelected_Click(object sender,EventArgs e) {
			List<Procedure> listProcs=new List<Procedure>();
			if(!CanAddGroupNote(doCheckDb:true,isSilent:false,listProcs)) {
				return;
			}
			DateTime procDate=listProcs[0].ProcDate;
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=listProcs[0].ClinicNum;
			}
			long provNum=listProcs[0].ProvNum;
			Procedure procGroup=new Procedure();
			procGroup.PatNum=Pd.PatNum;
			procGroup.ProcStatus=ProcStat.EC;
			procGroup.DateEntryC=DateTime.Now;
			procGroup.ProcDate=procDate;
			procGroup.ProvNum=provNum;
			procGroup.ClinicNum=clinicNum;//Will be 0 above if clinics disabled.
			procGroup.CodeNum=ProcedureCodes.GetCodeNum(ProcedureCodes.GroupProcCode);
			if(PrefC.GetBool(PrefName.ProcGroupNoteDoesAggregate)) {
				List<string> listProcNotes=new List<string>();
				for(int i=0;i<listProcs.Count;i++) {
					Procedure proc=listProcs[i];
					if(!string.IsNullOrWhiteSpace(proc.Note)) {
						listProcNotes.Add(proc.Note);
					}
				}
				procGroup.Note=string.Join("\r\n",listProcNotes);
			}
			else {
				//group notes are special; they have a status of EC but still get their procedure notes populated.
				procGroup.Note=ProcCodeNotes.GetNote(procGroup.ProvNum,procGroup.CodeNum,procGroup.ProcStatus,true); 
				if(!PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {
					//Users do not want to be prompted for auto notes, so remove them all from the procedure note.
					procGroup.Note=Regex.Replace(procGroup.Note,@"\[\[.+?\]\]","");
				}
			}
			procGroup.IsNew=true;
			Procedures.Insert(procGroup);
			List<ProcGroupItem> listProcGroupItems=new List<ProcGroupItem>();
			ProcGroupItem procGroupItem;
			for(int i=0;i<listProcs.Count;i++){
				procGroupItem=new ProcGroupItem();
				procGroupItem.ProcNum=listProcs[i].ProcNum;
				procGroupItem.GroupNum=procGroup.ProcNum;
				ProcGroupItems.Insert(procGroupItem);
				listProcGroupItems.Add(procGroupItem);
			}
			if(Programs.UsingOrion) {
				OrionProc orionProc=new OrionProc();
				orionProc.ProcNum=procGroup.ProcNum;
				orionProc.Status2=OrionStatus.C;
				OrionProcs.Insert(orionProc);
			}
			using FormProcGroup formProcGroup=new FormProcGroup();
			formProcGroup.GroupCur=procGroup;
			formProcGroup.GroupItemList=listProcGroupItems;
			formProcGroup.ProcList=listProcs;
			formProcGroup.ShowDialog();
			if(formProcGroup.DialogResult!=DialogResult.OK){
				return;
			}
			if(PrefC.GetBool(PrefName.ProcGroupNoteDoesAggregate)) {
				//remove the notes from all the attached procs where there          is a note           and           there is no signature
				List<Procedure> listProcsToUpdate=listProcs.Where(x=>!string.IsNullOrWhiteSpace(x.Note) && string.IsNullOrWhiteSpace(x.Signature)).ToList();
				for(int i=0;i<listProcsToUpdate.Count;i++) {
					Procedure oldProc=listProcsToUpdate[i].Copy();
					Procedure changedProc=listProcsToUpdate[i].Copy();
					changedProc.Note="";
					Procedures.Update(changedProc,oldProc);
				}
			}
			ModuleSelected(Pd.PatNum);
		}

		private void menuItemLabFee_Click(object sender,EventArgs e) {
			List<long> listProcNumsReg=new List<long>();
			List<long> listProcNumsLab=new List<long>();
			if(!CanAttachLabFee(isSilent:false,listProcNumsReg,listProcNumsLab)) {
				return;
			}
			List<ClaimProc> listClaimProcsForProc = ClaimProcs.RefreshForProc(listProcNumsReg[0]);
			Procedure procedure = Procedures.GetOneProc(listProcNumsReg[0],false);
			if(Procedures.IsAttachedToClaim(procedure,listClaimProcsForProc)) {
				MsgBox.Show(this,Lan.g(this,"Cannot attach a lab fee to a procedure already on a claim."));
				return;
			}
			//We only alter the lab procedure(s), not the regular procedure.
			Procedure procLab=null;
			Procedure procOld;
			for(int i=0;i<listProcNumsLab.Count;i++) {
				procLab=Procedures.GetOneProc(listProcNumsLab[i],false);
				procOld=procLab.Copy();
				procLab.ProcNumLab=listProcNumsReg[0];
				Procedures.Update(procLab,procOld);
			}
			if(procLab!=null) {
				CanadianLabFeeHelper(procLab.ProcNumLab);
			}
			ModuleSelected(Pd.PatNum);
		}

		private void menuItemLabFeeDetach_Click(object sender,EventArgs e) {
			if(gridProg.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Please select exactly one lab procedure first.");
				return;
			}
			DataRow row=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[0]].Tag;
			if(!CanDetachLabFee(row,isSilent:false)) {
				return;
			}
			Procedure procLab=Procedures.GetOneProc(PIn.Long(row["ProcNum"].ToString()),false);
			List<ClaimProc> listClaimProcsForProc=ClaimProcs.RefreshForProc(procLab.ProcNum);
			if(Procedures.IsAttachedToClaim(procLab,listClaimProcsForProc)) {
				MsgBox.Show(this,"Cannot detach a lab procedure already on a claim.");
				return;
			}
			Procedure procOld=procLab.Copy();
			procLab.ProcNumLab=0;
			Procedures.Update(procLab,procOld);
			CanadianLabFeeHelper(procOld.ProcNumLab);
			ModuleSelected(Pd.PatNum);
		}

		///<summary>Just prior to displaying the context menu, enable or disables the UnmaskSSN option</summary>
		private void MenuItemPopupUnmaskDOB(object sender,EventArgs e) {
			MenuItem menuItemDOB=gridPtInfo.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Name=="ViewDOB");
			if(menuItemDOB==null) { 
				return;//Should not happen
			}
			MenuItem menuItemSeperator=gridPtInfo.ContextMenu.MenuItems.OfType<MenuItem>().FirstOrDefault(x => x.Text=="-");
			if(menuItemSeperator==null) { 
				return;//Should not happen
			}
			int idxRowClick=gridPtInfo.PointToRow(_pointLastClicked.Y);
			int idxColClick=gridPtInfo.PointToCol(_pointLastClicked.X);//Make sure the user clicked within the bounds of the grid.
			if(idxRowClick>-1 && idxColClick>-1 && (gridPtInfo.ListGridRows[idxRowClick].Tag!=null)
				&& gridPtInfo.ListGridRows[idxRowClick].Tag is string
				&& ((string)gridPtInfo.ListGridRows[idxRowClick].Tag=="DOB")) 
			{
				if(Security.IsAuthorized(Permissions.PatientDOBView,true) 
					&& gridPtInfo.ListGridRows[idxRowClick].Cells[gridPtInfo.ListGridRows[idxRowClick].Cells.Count-1].Text!="") 
				{
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=true;
				}
				else {
					menuItemDOB.Visible=true;
					menuItemDOB.Enabled=false;
				}
				menuItemSeperator.Visible=true;
				menuItemSeperator.Enabled=true;
			}
			else {
				menuItemDOB.Visible=false;
				menuItemDOB.Enabled=false;
				if(gridPtInfo.ContextMenu.MenuItems.OfType<MenuItem>().Count(x => x.Visible==true && x.Text!="-")>1) {
					//There is more than one item showing, we want the seperator.
					menuItemSeperator.Visible=true;
					menuItemSeperator.Enabled=true;
				}
				else {
					//We dont want the seperator to be there with only one option.
					menuItemSeperator.Visible=false;
					menuItemSeperator.Enabled=false;
				}
			}
		}

		private void menuItemPrintDay_Click(object sender,EventArgs e) {
			if(gridProg.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			DataRow row=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[0]].Tag;
			//hospitalDate=PIn.DateT(row["ProcDate"].ToString());
			//Store the state of all checkboxes in temporary variables
			bool showRx=this.checkRx.Checked;
			bool showComm=this.checkComm.Checked;
			bool showApt=this.checkAppt.Checked;
			bool showEmail=this.checkEmail.Checked;
			bool showTask=this.checkTasks.Checked;
			bool showLab=this.checkLabCase.Checked;
			bool showSheets=this.checkSheets.Checked;
			bool showTeeth=this.checkShowTeeth.Checked;
			bool showAudit=this.checkAudit.Checked;
			DateTime showDateStart=_dateTimeShowDateStart;
			DateTime showDateEnd=_dateTimeShowDateEnd;
			bool showTP=this.checkShowTP.Checked;
			bool showComplete=this.checkShowC.Checked;
			bool showExist=this.checkShowE.Checked;
			bool showRefer=this.checkShowR.Checked;
			bool showCond=this.checkShowCn.Checked;
			bool showProcNote=this.checkNotes.Checked;
			bool customView=this.chartCustViewChanged;
			//Set the checkboxes to desired values for print out
			checkRx.Checked=false;
			checkComm.Checked=false;
			checkAppt.Checked=false;
			checkEmail.Checked=false;
			checkTasks.Checked=false;
			checkLabCase.Checked=false;
			checkSheets.Checked=false;
			checkShowTeeth.Checked=false;
			checkAudit.Checked=false;
			_dateTimeShowDateStart=PIn.DateT(row["ProcDate"].ToString());
			_dateTimeShowDateEnd=PIn.DateT(row["ProcDate"].ToString());
			checkShowTP.Checked=false;
			checkShowC.Checked=true;
			checkShowE.Checked=false;
			checkShowR.Checked=false;
			checkShowCn.Checked=false;
			checkNotes.Checked=true;
			chartCustViewChanged=true;//custom view will not reset the check boxes so we force it true.
			//Fill progress notes with only desired rows to be printed, then print.
			FillProgNotes();
			if(gridProg.ListGridRows.Count==0) {
				MsgBox.Show(this,"No completed procedures or notes to print");
			}
			else {
				try {
					_pagesPrinted=0;
					_headingPrinted=false;
					PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPageDay,
						Lan.g(this,"Day report for hospital printed"),
						auditPatNum:Pd.PatNum,
						margins:new Margins(0,0,0,0),
						printoutOrigin:PrintoutOrigin.AtMargin
					);
				}
				catch {

				}
			}
			//Set Date values and checkboxes back to original values, then refill progress notes.
			//hospitalDate=DateTime.MinValue;
			checkRx.Checked=showRx;
			checkComm.Checked=showComm;
			checkAppt.Checked=showApt;
			checkEmail.Checked=showEmail;
			checkTasks.Checked=showTask;
			checkLabCase.Checked=showLab;
			checkSheets.Checked=showSheets;
			checkShowTeeth.Checked=showTeeth;
			checkAudit.Checked=showAudit;
			_dateTimeShowDateStart=showDateStart;
			_dateTimeShowDateEnd=showDateEnd;
			checkShowTP.Checked=showTP;
			checkShowC.Checked=showComplete;
			checkShowE.Checked=showExist;
			checkShowR.Checked=showRefer;
			checkShowCn.Checked=showCond;
			checkNotes.Checked=showProcNote;
			chartCustViewChanged=customView;
			FillProgNotes();
		}

		private void menuItemPrintProg_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd2_PrintPage,
				Lan.g(this,"Progress notes printed"),
				auditPatNum:Pd.PatNum
			);
		}

		private void menuItemPrintRouteSlip_Click(object sender,EventArgs e) {
			if(!CanPrintRoutingSlip(isSilent: false)) {
				return;
			}
			Appointment apt=Appointments.GetOneApt(PIn.Long(((DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[0]].Tag)["AptNum"].ToString()));
			//for now, this only allows one type of routing slip.  But it could be easily changed.
			using FormRpRouting formRpRouting=new FormRpRouting();
			formRpRouting.AptNum=apt.AptNum;
			List<SheetDef> listCustomSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.RoutingSlip);
			if(listCustomSheetDefs.Count==0) {
				formRpRouting.SheetDefNum=0;
			}
			else {
				formRpRouting.SheetDefNum=listCustomSheetDefs[0].SheetDefNum;
			}
			formRpRouting.ShowDialog();
		}

		private void menuItemRxManage_Click(object sender,EventArgs e) {
			using FormRxManage formRxManage=new FormRxManage(Pd.Patient);
			formRxManage.ShowDialog();
			ModuleSelected(Pd.PatNum);
		}

		private void menuItemSetComplete_Click(object sender,EventArgs e) {
			if(!IsAuditMode(isSilent:false)) {
				return;
			}
			//get list of DataRows from the selected row's tags in gridProg, in case the grid is refilled and our selections are cleared
			//(happens if right-clicking and marking a task done and with the prompt message box up another task is edited before pressing the OK button)
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			long patNum=Pd.PatNum;//local patNum variable to make sure the patient hasn't changed since the above list was created
			#region One Appointment
			bool isSilent=!CanDisplayAppointment();
			if(CanCompleteAppointment(doCheckDb:true,isSilent:isSilent)) {
				if(!isSilent && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"Set appointment complete?")) {
					return;
				}
				DataRow rowApt=listSelectedRows.First();
				Appointment apt=Appointments.GetOneApt(PIn.Long(rowApt["AptNum"].ToString()));
				DateTime datePrevious=apt.DateTStamp;
				InsSub insSub1=InsSubs.GetSub(PatPlans.GetInsSubNum(Pd.ListPatPlans,PatPlans.GetOrdinal
					(PriSecMed.Primary,Pd.ListPatPlans,Pd.ListInsPlans,Pd.ListInsSubs)),Pd.ListInsSubs);
				InsSub insSub2=InsSubs.GetSub(PatPlans.GetInsSubNum(Pd.ListPatPlans,PatPlans.GetOrdinal(PriSecMed.Secondary,Pd.ListPatPlans,Pd.ListInsPlans,Pd.ListInsSubs)),Pd.ListInsSubs);
				Appointments.SetAptStatusComplete(apt,insSub1.PlanNum,insSub2.PlanNum);
				AppointmentEvent.Fire(ODEventType.AppointmentEdited,apt);
				List<Procedure> listProcsForAppt=Procedures.GetProcsForSingle(apt.AptNum,false);
				bool removeCompletedProcs=ProcedureL.DoRemoveCompletedProcs(apt,listProcsForAppt.FindAll(x => x.AptNum==apt.AptNum && x.ProcStatus==ProcStat.C));
				ProcedureL.SetCompleteInAppt(apt,Pd.ListInsPlans,Pd.ListPatPlans,Pd.Patient,Pd.ListInsSubs,removeCompletedProcs);//loops through each proc, also makes completed security logs
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit, apt.PatNum,
					apt.ProcDescript+", "+apt.AptDateTime.ToString()+", Set Complete",
					apt.AptNum,datePrevious);
				//If there is an existing HL7 def enabled, send a SIU message if there is an outbound SIU message defined
				if(HL7Defs.IsExistingHL7Enabled()) {
					//S14 - Appt Modification event
					MessageHL7 messageHL7=MessageConstructor.GenerateSIU(Pd.Patient,Pd.Family.GetPatient(Pd.Patient.Guarantor),EventTypeHL7.S14,apt);
					//Will be null if there is no outbound SIU message defined, so do nothing
					if(messageHL7!=null) {
						HL7Msg hl7Msg=new HL7Msg();
						hl7Msg.AptNum=apt.AptNum;
						hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
						hl7Msg.MsgText=messageHL7.ToString();
						hl7Msg.PatNum=Pd.PatNum;
						HL7Msgs.Insert(hl7Msg);
						if(ODBuild.IsDebug()) { 
							MessageBox.Show(this,messageHL7.ToString());
						}
					}
				}
				if(HieClinics.IsEnabled()) {
					HieQueues.Insert(new HieQueue(Pd.PatNum));
				}
				Recalls.Synch(Pd.PatNum);
				Recalls.SynchScheduledApptFull(Pd.PatNum);
				ModuleSelected(Pd.PatNum);
				//If necessary, prompt the user to ask the patient to opt in to using Short Codes.
				FormShortCodeOptIn.PromptIfNecessary(Pd.Patient,apt.ClinicNum);
				return;
			}
			#endregion One Appointment
			#region One task
			isSilent=!CanDisplayTask();
			if(CanCompleteTask(doCheckDb:true,isSilent: isSilent)) {
				if(!isSilent && !MsgBox.Show(this,MsgBoxButtons.OKCancel,"The selected task will be marked Done and will affect all users.")) {
					return;
				}
				long taskNum=PIn.Long(listSelectedRows[0]["TaskNum"].ToString());
				Task taskCur=Tasks.GetOne(taskNum);
				Task taskOld=taskCur.Copy();
				taskCur.TaskStatus=TaskStatusEnum.Done;//global even if new status is tracked by user
				if(taskOld.TaskStatus!=TaskStatusEnum.Done) {
					taskCur.DateTimeFinished=DateTime.Now;
				}
				TaskUnreads.DeleteForTask(taskCur);//clear out taskunreads. We have too many tasks to read the done ones.
				Tasks.Update(taskCur,taskOld);
				TaskHist taskHist=new TaskHist(taskOld);
				taskHist.UserNumHist=Security.CurUser.UserNum;
				TaskHists.Insert(taskHist);
				long signalNum=Signalods.SetInvalid(InvalidType.Task,KeyType.Task,taskCur.TaskNum);
				UserControlTasks.RefillLocalTaskGrids(taskCur,TaskNotes.GetForTask(taskCur.TaskNum),new List<long>() { signalNum });
				ModuleSelected(Pd.PatNum);
				return;
			}
			#endregion One task
			//Sometimes we get to this point with a single appointment, which will cause MenuItemSetSelectedProcsStatus() to give an incorrect MessageBox error.
			//Check to make sure the selected rows are one or more procedures before continuing.
			if(listSelectedRows.Count==1 && listSelectedRows[0]["ProcNum"].ToString()=="0") {
				return;
			}
			#region Multiple procedures
			MenuItemSetSelectedProcsStatus(ProcStat.C);
			#endregion
		}

		private void menuItemSetEC_Click(object sender,EventArgs e) {
			MenuItemSetSelectedProcsStatus(ProcStat.EC);
		}

		private void menuItemSetEO_Click(object sender,EventArgs e) {
			MenuItemSetSelectedProcsStatus(ProcStat.EO);
		}

		private void menuItemViewMultiVisitGroup_Click(object sender,EventArgs e) {
			List<DisplayField> listDisplayFields;
			//Get the display fields to pass to the form
			if(gridChartViews.ListGridRows.Count==0) {
				listDisplayFields=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			}
			else {
				listDisplayFields=DisplayFields.GetForChartView(_chartViewCurDisplay.ChartViewNum);
			}
			//Get the list of selected procedures' ProcNums (already verified to be part of multi-visit groups)
			List<long> listProcNumsSelected=gridProg.SelectedIndices.Select(x => PIn.Long(((DataRow)gridProg.ListGridRows[x].Tag)["ProcNum"].ToString())).ToList();
			//Get all PMVs from all selected procedures' MV groups
			List<long> listGroupProcMultiVisitNums=Pd.ListProcMultiVisits.FindAll(x => listProcNumsSelected.Contains(x.ProcNum))
				.Select(x=>x.GroupProcMultiVisitNum).ToList();//usually just one GroupProcMultiVisitNum because there's usually just one row selected.
			//Now find all ProcMultiVisits with a matching GroupProcMultiVisitNum
			List<ProcMultiVisit> listProcMultiVisits=Pd.ListProcMultiVisits.FindAll(x => listGroupProcMultiVisitNums.Contains(x.GroupProcMultiVisitNum));
			List<long> listProcNumsInGroup=listProcMultiVisits.Select(x=>x.ProcNum).ToList();
			List<DataRow> listDataRowsProcs=Pd.TableProgNotes.Select()
				.Where(x => listProcNumsInGroup.Contains(PIn.Long(x["ProcNum"].ToString())))
				.ToList();
			//Get all procedures per PMV group
			Dictionary<long,List<DataRow>> dictProceduresPerGroup=new Dictionary<long,List<DataRow>>();
			for(int i=0;i<listDataRowsProcs.Count;i++) {
				DataRow dataRow=listDataRowsProcs[i];
				if(!dictProceduresPerGroup.ContainsKey(listProcMultiVisits[i].GroupProcMultiVisitNum)) {
					dictProceduresPerGroup.Add(listProcMultiVisits[i].GroupProcMultiVisitNum,new List<DataRow>());
				}
				dictProceduresPerGroup[listProcMultiVisits[i].GroupProcMultiVisitNum].Add(dataRow);
			}
			//Store the location of each new FormMultiVisitGroup so the windows can all be cascaded
			int xPrevious=0;
			int yPrevious=0;
			const int cascadeWindowOffset=25;//The X and Y amount to offset each subsequent window by
			foreach(KeyValuePair<long,List<DataRow>> keyValuePair in dictProceduresPerGroup) {
				FormMultiVisitGroup formMultiVisitGroup=new FormMultiVisitGroup();
				formMultiVisitGroup.GridColumns=gridProg.Columns.ToList();
				formMultiVisitGroup.GridTitle=gridProg.Title;
				formMultiVisitGroup.ListGridDisplayFields=listDisplayFields;
				formMultiVisitGroup.ListDataRows=keyValuePair.Value;
				formMultiVisitGroup.FuncBuildGridRow=(t) => GridProgRowConstruction(t,listDisplayFields);
				//formMultiVisitGroup.ListProcMultiVisits=LoadData.ListProcMultiVisits;
				formMultiVisitGroup.FormClosing+=FormMultiVisitGroup_FormClosing;
				formMultiVisitGroup.Show();
				//Cascade the windows if there are multiple
				if(!(xPrevious==0&&yPrevious==0)) {
					formMultiVisitGroup.Location=new Point(xPrevious+cascadeWindowOffset,yPrevious+cascadeWindowOffset);
				}
				xPrevious=formMultiVisitGroup.Location.X;
				yPrevious=formMultiVisitGroup.Location.Y;
			}
		}

		private void FormMultiVisitGroup_FormClosing(object sender,FormClosingEventArgs e) {
			//Refresh if we've changed something
			FormMultiVisitGroup form=(FormMultiVisitGroup)sender;
			if(form.Changed) {
				Pd.TableProgNotes=ChartModules.GetProgNotes(Pd.PatNum,checkAudit.Checked);
				FillProgNotes(retainToothSelection:true,doRefreshData:true);
				Pd.ClearAndFill(EnumPdTable.ProcMultiVisit);
			}
		}

		private void MenuItemUnmaskDOB_Click(object sender,EventArgs e) {
			//Preference and permissions check has already happened by this point.
			//Guaranteed to be clicking on a valid row & column.
			int rowClick = gridPtInfo.PointToRow(_pointLastClicked.Y);
			gridPtInfo.BeginUpdate();
			GridRow row=gridPtInfo.ListGridRows[rowClick];
			row.Cells[row.Cells.Count-1].Text=Patients.DOBFormatHelper(Pd.Patient.Birthdate,false);
			gridPtInfo.EndUpdate();
			string logText="Date of birth unmasked in Chart Module";
			SecurityLogs.MakeLogEntry(Permissions.PatientDOBView,Pd.PatNum,logText);
		}
		#endregion Methods - Event Handlers - Menu Items

		#region Methods - Event Handlers - Menu Other
		private void menuConsent_Click(object sender,EventArgs e) {
			SheetDef sheetDef=(SheetDef)(((MenuItem)sender).Tag);
			SheetDefs.GetFieldsAndParameters(sheetDef);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,Pd.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",Pd.PatNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void menuConsent_Popup(object sender,EventArgs e) {
			menuConsent.MenuItems.Clear();
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
			MenuItem menuItem;
			for(int i=0;i<listSheetDefs.Count;i++) {
				menuItem=new MenuItem(listSheetDefs[i].Description);
				menuItem.Tag=listSheetDefs[i];
				menuItem.Click+=new EventHandler(menuConsent_Click);
				menuConsent.MenuItems.Add(menuItem);
			}
		}

		private void menuOrthoChart_Click(object sender,EventArgs e) {
			int orthoChartTabIndex=(int)(((MenuItem)sender).Tag);
			using FormOrthoChart formOrthoChart=new FormOrthoChart(Pd.Patient,orthoChartTabIndex);
			formOrthoChart.ShowDialog();
			ModuleSelected(Pd.PatNum);
		}

		private void menuOrthoChart_Popup(object sender,EventArgs e) {
			menuOrthoChart.MenuItems.Clear();
			List<OrthoChartTab> listOrthoChartTabs=OrthoChartTabs.GetDeepCopy(true);
			for(int i=1;i<listOrthoChartTabs.Count;i++) {//Start i at 1 so tha we do not duplicate the first ortho tab (the main button)
				MenuItem menuItem=new MenuItem(listOrthoChartTabs[i].TabName);
				menuItem.Tag=i;
				menuItem.Click+=new EventHandler(menuOrthoChart_Click);
				menuOrthoChart.MenuItems.Add(menuItem);
			}
		}

		///<summary>The menu will display all items that are relevant to at least one selected row. If there is at least one selected row that is not relevant, the menu item will be disabled but visible.</summary>
		private void menuProgRight_Popup(object sender,EventArgs e) {
			//Permissions on some menu items will not get checked until clicking on the menu item which will then stop the user if not allowed. 
			ShowMenuItemHelper(menuItemDelete,x => CanDeleteRow(x.Row));
			ShowMenuItemHelper(menuItemSetEO,x => CanChangeProcsStatus(ProcStat.EO,x.Row,doCheckDb:false,isSilent:true));
			ShowMenuItemHelper(menuItemSetEC,x => CanChangeProcsStatus(ProcStat.EC,x.Row,doCheckDb:false,isSilent:true));
			ShowMenuItemHelper(menuItemSetComplete,x => CanChangeProcsStatus(ProcStat.C,x.Row,doCheckDb:false,isSilent:true));
			ShowMenuItemHelper(menuItemEditSelected,x => CanEditRow(x.Row,doCheckDb:false,isSilent:true,new List<Procedure> { }));
			ShowMenuItemHelper(menuItemGroupSelected,x => CanGroupRow(x.Row,doCheckDb:false,isSilent:true,new List<Procedure> { }));
			ShowMenuItemHelper(menuItemPrintDay,x => CanPrintDay(x.Row,x.Index));
			ShowMenuItemHelper(menuItemLabFeeDetach,x => CanDetachLabFee(x.Row,isSilent:true));
			ShowMenuItemHelper(menuItemLabFee,x => CanAttachLabFee(isSilent:true,new List<long> { },new List<long> { }));
			if(CanDisplayRoutingSlip()) {
				menuItemPrintRouteSlip.Visible=true;
				menuItemPrintRouteSlip.Enabled=CanPrintRoutingSlip(isSilent:true);
			}
			else {
				menuItemPrintRouteSlip.Visible=false;
			}
			menuItemGroupSelected.Enabled=CanAddGroupNote(doCheckDb:false,isSilent:true,new List<Procedure>());
			if(CanDisplayAppointment() || CanDisplayTask()) {
				menuItemSetComplete.Visible=true;
				menuItemSetComplete.Enabled=(CanCompleteAppointment(doCheckDb:false,isSilent:true) || CanCompleteTask(doCheckDb:false,isSilent:true));
			}
			ShowMenuItemHelper(menuItemGroupMultiVisit,x => CanGroupMultiVisit(x.Row,doCheckDb:false,isSilent:true));
			ShowMenuItemHelper(menuItemViewMultiVisitGroup,x => CanUngroupMultiVisit(x.Row,doCheckDb:false,isSilent:true));
		}

		private void menuToothChart_Popup(object sender,EventArgs e) {
			//ComputerPref computerPref=ComputerPrefs.GetForLocalComputer();
			//only enable the big button if 3D graphics
			/*if(computerPref.GraphicsSimple) {
				menuItemChartBig.Enabled=false;
			}
			else {
				menuItemChartBig.Enabled=true;
			}*/
		}
		#endregion Methods - Event Handlers - Menu Other

		#region Methods - Event Handlers - OnDrawItems
		///<summary>Draws one tab item on tabControlImages.</summary>
		private void DrawTabItem(object sender,DrawItemEventArgs e) {
      Graphics g=e.Graphics;
      //Pen penBlue=new Pen(Color.FromArgb(97,136,173));
			//Pen penRed=new Pen(Color.FromArgb(140,51,46));
			Pen penOrange=new Pen(Color.FromArgb(250,176,3),2);
			Pen penDkOrange=new Pen(Color.FromArgb(227,119,4));
			SolidBrush brBlack=new SolidBrush(Color.Black);
			int selected=-1;
			if(tabControlImages.SelectedTab!=null) {
				selected=tabControlImages.TabPages.IndexOf(tabControlImages.SelectedTab);
			}
			Rectangle bounds=e.Bounds;
			Rectangle rect=new Rectangle(bounds.X+2,bounds.Y+1,bounds.Width-5,bounds.Height-4);
			if(e.Index==selected) {
				g.FillRectangle(new SolidBrush(Color.White),rect);
				//g.DrawRectangle(penBlue,rect);
				g.DrawLine(penOrange,rect.X,rect.Bottom-1,rect.Right,rect.Bottom-1);
				g.DrawLine(penDkOrange,rect.X+1,rect.Bottom,rect.Right-2,rect.Bottom);
				//tabs do not resize because variable tab sizes are not supported by MS
				if(LayoutManager.GetScaleMS()>1){
					g.DrawString(tabControlImages.TabPages[e.Index].Text,Font,brBlack,bounds.X+3,bounds.Y-1);
				}
				else{
					g.DrawString(tabControlImages.TabPages[e.Index].Text,Font,brBlack,bounds.X+3,bounds.Y+6);
				}
			}
			else {
				if(LayoutManager.GetScaleMS()>1){
					g.DrawString(tabControlImages.TabPages[e.Index].Text,Font,brBlack,bounds.X,bounds.Y-4);
				}
				else{
					g.DrawString(tabControlImages.TabPages[e.Index].Text,Font,brBlack,bounds.X,bounds.Y);
				}
			}
    }
		#endregion Methods - Event Handlers - OnDrawItems

		#region Methods - Event Handlers - Panels
		private void panelImages_MouseDown(object sender,MouseEventArgs e) {
			if(e.Y>3) {
				return;
			}
			_isMouseDownOnImageSplitter=true;
			_imageSplitterOriginalY=panelImages.Top;
			_originalImageMousePos=panelImages.Top+e.Y;
		}

		private void panelImages_MouseLeave(object sender,EventArgs e) {
			panelImages.Cursor=Cursors.Default;
		}

		private void panelImages_MouseMove(object sender,MouseEventArgs e) {
			if(!_isMouseDownOnImageSplitter) {
				if(e.Y<=3) {
					panelImages.Cursor=Cursors.HSplit;
				}
				else {
					panelImages.Cursor=Cursors.Default;
				}
				return;
			}
			//panelNewTop
			int panelOldH=panelImages.Height;//Keep track of panelImages old height
			int panelNewH=panelImages.Bottom-(_imageSplitterOriginalY+(panelImages.Top+e.Y)-_originalImageMousePos);//-top
			if(panelNewH<10){//cTeeth.Bottom)
				panelNewH=10;//cTeeth.Bottom;//keeps it from going too low
			}
			if(panelNewH>panelImages.Bottom-_toothChartRelay.Bottom){
				panelNewH=panelImages.Bottom-_toothChartRelay.Bottom;//keeps it from going too high
			}
			//The top of the panel needs to adjust based on its current value minus the difference of its new height and hold height
			panelImages.Top=panelImages.Top-(panelNewH-panelOldH);
			LayoutManager.MoveHeight(panelImages,panelNewH);
			if(UsingEcwTightOrFull()) {
				if(panelImages.Visible) {
					LayoutManager.MoveHeight(panelEcw,tabControlImages.Top-panelEcw.Top+1-(panelImages.Height+2));
				}
				else {
					LayoutManager.MoveHeight(panelEcw,tabControlImages.Top-panelEcw.Top+1);
				}
			}
		}

		private void panelImages_MouseUp(object sender,MouseEventArgs e) {
			if(!_isMouseDownOnImageSplitter) {
				return;
			}
			_isMouseDownOnImageSplitter=false;
			LayoutControls();//Re-Layout the controls after we have adjusted the image panel
		}

		///<summary>Handles single clicks that occur on button items. Not double clicks, not labels, and not empty space.</summary>
		private void panelQuickButtons_ItemClickBut(object sender,ODButtonPanelEventArgs e) {
			ProcButtonQuick procButtonQuick=null;
			for(int i=0;i<e.Item.Tags.Count;i++) {
				if(e.Item.Tags[i].GetType()!=typeof(ProcButtonQuick)) {
					continue;
				}
				procButtonQuick=(ProcButtonQuick)e.Item.Tags[i];//should always happen
			}
			if(procButtonQuick==null) {
				return;//should never happen.
			}
			ProcButtonClicked(null,procButtonQuick);
		}
		#endregion Methods - Event Handlers - Panels

		#region Methods - Event Handlers - Printing
		private void pd2_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=new Rectangle(25,40,800,1000);//1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			text="Chart Progress Notes";//heading
			g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
			text=DateTime.Today.ToShortDateString();//date
			g.DrawString(text,fontSubHeading,Brushes.Black,bounds.Right-g.MeasureString(text,fontSubHeading).Width,yPos);
			yPos+=(int)g.MeasureString(text,fontHeading).Height;
			text=Pd.Patient.GetNameFL();//name
			if(g.MeasureString(text,fontSubHeading).Width>700) {
				//extremely long name
				text=Pd.Patient.GetNameFirst()[0]+". "+Pd.Patient.LName;//example: J. Sparks
			}
			string[] arrayHeaderText={ text };
			Plugins.HookAddCode(this,"ContrChart.pd2_PrintPage_middle",Pd.Patient,e,g,arrayHeaderText);
			text=arrayHeaderText[0];
			g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
			text="Page "+(_pagesPrinted+1);
			g.DrawString(text,fontSubHeading,Brushes.Black,bounds.Right-g.MeasureString(text,fontSubHeading).Width,yPos);
			yPos+=30;
			_headingPrinted=true;
			_headingPrintH=yPos;
			#endregion
			//If using zoom, we need to adjust the gridProg's size back to it's non-zoomed state before printing.
			//This is only because we are using GridOld, which is only used in this one place.
			//Once we refactor to move this over to GridOD, then this is not needed.
			Size sizeGirdProg=gridProg.Size;
			if(LayoutManager.ScaleMy()!=1) {				
				LayoutManager.MoveSize(gridProg,new Size((int)(gridProg.Size.Width/LayoutManager.ScaleMy()),(int)(gridProg.Size.Height/LayoutManager.ScaleMy())));
				gridProg.ScaleMy=1;
			}
			yPos=gridProg.PrintPage(g,_pagesPrinted,bounds,_headingPrintH,true);
			//Restore the grid's size back to it's original state.
			if(LayoutManager.ScaleMy()!=1) {
				LayoutManager.MoveSize(gridProg,sizeGirdProg);
				gridProg.ScaleMy=LayoutManager.ScaleMy();
			}
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void pd2_PrintPageDay(object sender,PrintPageEventArgs e) {
			Rectangle bounds=new Rectangle(25,40,800,1000);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				text="Chart Progress Notes";
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				//practice
				text=PrefC.GetString(PrefName.PracticeTitle);
				if(PrefC.HasClinicsEnabled) {
					DataRow row;
					long procNum;
					long clinicNum;
					for(int i=0;i<gridProg.ListGridRows.Count;i++) {
						row=(DataRow)gridProg.ListGridRows[i].Tag;
						procNum=PIn.Long(row["ProcNum"].ToString());
						if(procNum==0) {
							continue;
						}
						clinicNum=Procedures.GetClinicNum(procNum);
						if(clinicNum!=0) {//The first clinicNum that's encountered
							//Description is used here because it can be printed and shown to the patient.
							text=Clinics.GetDesc(clinicNum);
							break;
						}
					}
				}
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				//name
				text=Pd.Patient.GetNameFL();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				text="Birthdate: "+Pd.Patient.Birthdate.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				text="Printed: "+DateTime.Today.ToShortDateString();
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontSubHeading).Height;
				text="Ward: "+Pd.Patient.Ward;
				g.DrawString(text,fontSubHeading,Brushes.Black,center-g.MeasureString(text,fontSubHeading).Width/2,yPos);
				yPos+=20;
				//Patient images are not shown when the A to Z folders are disabled.
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
					Bitmap bitmapPatPic=Documents.GetPatPict(Pd.PatNum,ImageStore.GetPatientFolder(Pd.Patient,ImageStore.GetPreferredAtoZpath()));
					if(bitmapPatPic!=null) {
						Bitmap bitmap80=ImageHelper.GetBitmapSquare(bitmapPatPic,80);
						g.DrawImage(bitmap80,center-40,yPos,80,80);
						bitmap80.Dispose();
						bitmapPatPic.Dispose();
						yPos+=80;
					}
					yPos+=30;
					_headingPrinted=true;
					_headingPrintH=yPos;
				}
			}
			#endregion
			yPos=gridProg.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				g.DrawString("Signature_________________________________________________________",
								fontSubHeading,Brushes.Black,160,yPos+20);
				e.HasMorePages=false;
			}
			g.Dispose();
		}
		#endregion Methods - Event Handlers - Printing

		#region Methods - Event Handlers - Tabs General
		private void tabControlImages_MouseDown(object sender,MouseEventArgs e) {
			if(_selectedImageTab==-1) {
				_selectedImageTab=tabControlImages.SelectedIndex;
				return;
			}
			bool isTabAlreadySelected=false;
			if(_selectedImageTab.Between(0,tabControlImages.TabCount-1)) {
				Rectangle rect=tabControlImages.GetTabRect(_selectedImageTab);
				isTabAlreadySelected=rect.Contains(e.X,e.Y);
			}
			if(isTabAlreadySelected) {
				if(panelImages.Visible) {
					panelImages.Visible=false;
				}
				else {
					panelImages.Visible=true;
				}
			}
			else {//clicked on a new tab
				if(!panelImages.Visible) {
					panelImages.Visible=true;
				}
			}
			_selectedImageTab=tabControlImages.SelectedIndex;
			FillImages();//it will not actually fill the images unless panelImages is visible.
			if(UsingEcwTightOrFull()) {
				if(panelImages.Visible) {
					LayoutManager.MoveHeight(panelEcw,tabControlImages.Top-panelEcw.Top+1-(panelImages.Height+2));
				}
				else {
					LayoutManager.MoveHeight(panelEcw,tabControlImages.Top-panelEcw.Top+1);
				}
			}
			RefreshModuleScreen(false);//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();
		}

		private void tabProc_MouseDown(object sender,MouseEventArgs e) {
			ToggleCheckTreatPlans();
			if(_heightTabProcOrig96==0) {
				//Save original height. It will get reset to 0 every time it is expanded.
				//We reset it in case user changes layouts with differenct tabProc heights.
				_heightTabProcOrig96=LayoutManager.UnscaleF(tabProc.Height);
			}
			//selected tab will have changed, so we need to test the original selected tab:
			Rectangle rect=tabProc.GetTabRect(_selectedProcTab);
			if(rect.Contains(e.X,e.Y) && tabProc.Height>LayoutManager.Scale(28)) {//clicked on the already selected tab which was maximized
				LayoutManager.MoveHeight(tabProc,LayoutManager.Scale(27));//tabs are ~27 high, but we have to assume rounding errors
				tabProc.Refresh();
			}
			else if(tabProc.Height<LayoutManager.Scale(29)) {//clicked on a minimized tab
				LayoutManager.MoveHeight(tabProc,LayoutManager.Scale(_heightTabProcOrig96));
				_heightTabProcOrig96=0;//Set 0 so that we save height again next time. Height could change with different layout.
				tabProc.Refresh();
			}
			else {//clicked on a new tab
				//height will have already been set, so do nothing
			}
			_selectedProcTab=tabProc.SelectedIndex;
			if(PrefC.GetBool(PrefName.ChartOrthoTabAutomaticCheckboxes)){
				if(tabProc.SelectedTab==tabOrtho){
					//see the click events for these two checkboxes
					if(!checkOrthoMode.Checked){
						checkOrthoMode.Checked=true;
						FillToothChart(false);
					}
					if(!checkShowOrtho.Checked){
						checkShowOrtho.Checked=true;
						LayoutControls();
						FillOrthoTabs();
						Pd.ClearAndFill(EnumPdTable.OrthoChart);
						FillGridOrtho();
					}
				}
				else{//any other tab
					if(checkOrthoMode.Checked){
						checkOrthoMode.Checked=false;
						FillToothChart(false);
					}
					if(checkShowOrtho.Checked){
						checkShowOrtho.Checked=false;
						LayoutControls();//this hides the ortho grids and instead shows the progress notes
					}
				}
			}
			//Now that tabProc has collapsed or expanded, we need to refresh the layout to account for Chart Module sheet growth behavior of other controls
			SheetFieldLayoutMode sheetFieldLayoutMode=LayoutSheet_GetMode();
			Dictionary<string,Control> dictionaryStringControl=LayoutSheet_GetControlDict();
			List<SheetFieldDef> listSheetFieldDefs=_sheetLayoutController.GetPertinentSheetFieldDefs(sheetFieldLayoutMode);
			_sheetLayoutController.RefreshGridVerticalSpace(sheetFieldLayoutMode,dictionaryStringControl,listSheetFieldDefs);
			for(int i=0;i<dictionaryStringControl.Count;i++){
				LayoutManager.Synch(dictionaryStringControl.Values.ElementAt(i));//The controls were just moved around programmatically by the sheetLayoutController.
				//This will also pick up any change in anchor and move that over to the LayoutManager
			}
		}
		#endregion Methods - Event Handlers - Tabs General

		#region Methods - Event Handlers - Tab EnterTx
		private void butAddProc_Click(object sender,EventArgs e) {
			if(_procStatusNew==ProcStat.C) {
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show("Set the procedure complete by setting the appointment complete.  "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
					return;
				}
				//We will call Security.IsAuthorized again once we know the ProcCode and the ProcFee.
				if(!ProcedureCodes.DoAnyBypassLockDate() && !Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text))) {
					return;
				}
			}
			bool isValid;
			TreatmentArea treatmentArea;
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(formProcCodes.SelectedCodeNum);
			if(_listSubstitutionLinks==null){
				_listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(Pd.ListInsPlans);
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(Pd.PatNum);
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){procedureCode },null,//no proc, so medical code won't have changed
				null,//listProvNumsTreat: providers will instead be set from other places
				Pd.Patient.PriProv,Pd.Patient.SecProv,Pd.Patient.FeeSched,Pd.ListInsPlans,new List<long>(){Pd.Patient.ClinicNum},Pd.ListAppointments, _listSubstitutionLinks, discountPlanNum);
			List<string> listProcCodes=new List<string>();
			//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
			if(ProcedureCodes.GetStringProcCode(formProcCodes.SelectedCodeNum)!="D9986" && ProcedureCodes.GetStringProcCode(formProcCodes.SelectedCodeNum)!="D9987") {
				Procedures.SetDateFirstVisit(DateTime.Today,1,Pd.Patient);
			}
			Procedure procCur;
			for(int n=0;n==0 || n<_toothChartRelay.SelectedTeeth.Count;n++) {
				isValid=true;
				procCur=new Procedure();//going to be an insert, so no need to set Procedures.CurOld
				//Procedure
				procCur.CodeNum = formProcCodes.SelectedCodeNum;
				//Procedures.Cur.ProcCode=ProcButtonItems.CodeList[i];
				treatmentArea=procedureCode.TreatArea;
				if((treatmentArea==TreatmentArea.None
					|| treatmentArea==TreatmentArea.Arch
					|| treatmentArea==TreatmentArea.Mouth
					|| treatmentArea==TreatmentArea.Quad
					|| treatmentArea==TreatmentArea.Sextant
					|| treatmentArea==TreatmentArea.ToothRange)
					&& n>0) {//the only two left are tooth and surf
					continue;//only entered if n=0, so they don't get entered more than once.
				}
				else if(procedureCode.AreaAlsoToothRange) { // if AreaAlsoToothRange==true and procedureCode set for Quad or Arch
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						AddProcedure(procCur,listFees);
					}
					else { 
						if(treatmentArea==TreatmentArea.Arch) { 
							AddArchProcsWithToothRange(procCur,listFees);
						}
						else if(treatmentArea==TreatmentArea.Quad) {
							AddQuadProcsWithToothRange(procCur,listFees);
						}
					}
				}
				else if(treatmentArea==TreatmentArea.Quad) {
					AddQuadProcs(procCur,listFees);
				}
				else if(treatmentArea==TreatmentArea.Surf) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						isValid=false;
					}
					else {
						procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
						//Procedures.Cur=ProcCur;
					}
					if(textSurf.Text=="") {
						isValid=false;
					}
					else {
						procCur.Surf=Tooth.SurfTidyFromDisplayToDb(textSurf.Text,procCur.ToothNum);
					}
					if(isValid) {
						AddQuick(procCur,listFees);
					}
					else {
						AddProcedure(procCur,listFees);
					}
				}
				else if(treatmentArea==TreatmentArea.Tooth) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						//Procedures.Cur=ProcCur;
						AddProcedure(procCur,listFees);
					}
					else {
						procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
						//Procedures.Cur=ProcCur;
						AddQuick(procCur,listFees);
					}
				}
				else if(treatmentArea==TreatmentArea.ToothRange) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						//Procedures.Cur=ProcCur;
						AddProcedure(procCur,listFees);
					}
					else {
						procCur.ToothRange="";
						for(int b=0;b<_toothChartRelay.SelectedTeeth.Count;b++) {
							if(b!=0) procCur.ToothRange+=",";
							procCur.ToothRange+=_toothChartRelay.SelectedTeeth[b];
						}
						//Procedures.Cur=ProcCur;
						AddProcedure(procCur,listFees);//it's nice to see the procedure to verify the range
					}
				}
				else if(treatmentArea==TreatmentArea.Arch) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						procCur.Surf=CodeMapping.GetArchSurfaceFromProcCode(ProcedureCodes.GetProcCode(procCur.CodeNum));
						//Procedures.Cur=ProcCur;
						AddProcedure(procCur,listFees);
						continue;
					}
					foreach(string arch in Tooth.GetArchesForTeeth(_toothChartRelay.SelectedTeeth)) {
						Procedure proc=procCur.Copy();
						proc.Surf=arch;
						AddQuick(proc,listFees);
					}
				}
				else if(treatmentArea==TreatmentArea.Sextant) {
					//Procedures.Cur=ProcCur;
					AddProcedure(procCur,listFees);
				}
				else {//mouth
					//Procedures.Cur=ProcCur;
					AddQuick(procCur,listFees);
				}
				listProcCodes.Add(ProcedureCodes.GetProcCode(procCur.CodeNum).ProcCode);
			}//for n
			//this was requiring too many irrelevant queries and going too slowly   //ModuleSelected(PatCur.PatNum);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
			ClearButtons();
			FillProgNotes();
			if(_procStatusNew==ProcStat.C) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,listProcCodes,Pd.PatNum);
			}
		}

		private void butBF_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butBF.BackColor==Color.White) {
				butBF.BackColor=SystemColors.Control;
			}
			else {
				butBF.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butD_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butD.BackColor==Color.White) {
				butD.BackColor=SystemColors.Control;
			}
			else {
				butD.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butL_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butL.BackColor==Color.White) {
				butL.BackColor=SystemColors.Control;
			}
			else {
				butL.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butM_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butM.BackColor==Color.White) {
				butM.BackColor=SystemColors.Control;
			}
			else {
				butM.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butOI_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butOI.BackColor==Color.White) {
				butOI.BackColor=SystemColors.Control;
			}
			else {
				butOI.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void butOK_Click(object sender,EventArgs e) {
			EnterTypedCode();
		}

		private void butV_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butV.BackColor==Color.White) {
				butV.BackColor=SystemColors.Control;
			}
			else {
				butV.BackColor=Color.White;
			}
			UpdateSurf();
		}

		private void checkTreatPlans_Click(object sender,EventArgs e) {
			gridTreatPlans.SetAll(false);
			if(_listTreatPlans!=null && _listTreatPlans.Count>0) {
				gridTreatPlans.SetSelected(0,true);
			}
			FillProgNotes();
			RefreshModuleScreen(isClinicRefresh:false);//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();
		}

		private void ComboPrognosis_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboPrognosis.SelectedIndex<1//0 index is 'no prognosis' and does not need to be verified.yy
				|| Defs.GetDefsForCategory(DefCat.Prognosis,true).Any(x => x.DefNum==comboPrognosis.GetSelectedDefNum()))
			{
				return;
			}
			MsgBox.Show(this,"The selected Prognosis has been hidden.");
			ModuleSelected(Pd.PatNum);
		}

		private void comboPriority_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboPriority.SelectedIndex<1//0 is 'no priority'
				|| Defs.GetDefsForCategory(DefCat.TxPriorities,true).Any(x => x.DefNum==comboPriority.GetSelectedDefNum())) {
				return;
			}
			//this only happens if the user selects a priority that was just hidden by someone else
			MsgBox.Show(this,"The selected Priority has been hidden.");
			ModuleSelected(Pd.PatNum);
		}

		private void gridProg_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			_chartScrollVal=gridProg.ScrollValue;
			DataRow row=(DataRow)gridProg.ListGridRows[e.Row].Tag;
			switch(ChartModules.GetRowType(row,out long rowPk)){
				case ProgNotesRowType.Proc:
					if(checkAudit.Checked){
						MsgBox.Show(this,"Not allowed to edit procedures when in audit mode.");
						return;
					}
					Procedure proc=Procedures.GetOneProc(rowPk,true);
					if(ProcedureCodes.GetStringProcCode(proc.CodeNum,doThrowIfMissing:false)==ProcedureCodes.GroupProcCode) {
						using FormProcGroup formProcGroup=new FormProcGroup();		
						List<ProcGroupItem> listProcGroupItems=ProcGroupItems.GetForGroup(proc.ProcNum);
						List<Procedure> listProcs=new List<Procedure>();
						for(int i=0;i<listProcGroupItems.Count;i++) {
							listProcs.Add(Procedures.GetOneProc(listProcGroupItems[i].ProcNum,false));
						}
						formProcGroup.GroupCur=proc;
						formProcGroup.GroupItemList=listProcGroupItems;
						formProcGroup.ProcList=listProcs;
						formProcGroup.ShowDialog();
						if(formProcGroup.DialogResult==DialogResult.OK) {
							ModuleSelected(Pd.PatNum);
						}
						return;
					}
					else {
						List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
						List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(Pd.PatNum);
						List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){Pd.PatNum});
						for(int i=0;i<listProcedures.Count;i++) {
							if(listProcedures[i].ProcNum==proc.ProcNum) {
								break;
							}
							listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
						}
						using FormProcEdit formProcEdit=new FormProcEdit(proc,Pd.Patient,Pd.Family,listPatToothInitials:Pd.ListToothInitials);
						formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
						formProcEdit.ListClaimProcHists=Pd.ListClaimProcHists;
						Plugins.HookAddCode(this, "ContrChart.gridProg_CellDoubleClick_proc", proc, formProcEdit);
						if(!formProcEdit.IsDisposed) { //Form might be disposed by the above hook.
							formProcEdit.ShowDialog();
						} 
						Plugins.HookAddCode(this, "ContrChart.gridProg_CellDoubleClick_proc2", proc, formProcEdit);
						if(formProcEdit.DialogResult!=DialogResult.OK) {
							return;
						}
					}
					break;
				case ProgNotesRowType.CommLog:
					Commlog commlog=Commlogs.GetOne(rowPk);
					if(commlog==null) {
						MsgBox.Show(this,"This commlog has been deleted by another user.");
					}
					else {
						using FormCommItem formCommItem=new FormCommItem(commlog);
						if(formCommItem.ShowDialog()!=DialogResult.OK) {
							return;
						}
					}
					break;
				case ProgNotesRowType.WebChatSession:
					long webChatSessionNum=PIn.Long(row["WebChatSessionNum"].ToString());
					WebChatSession webChatSession=WebChatSessions.GetOne(webChatSessionNum);
					using(FormWebChatSession formWebChatSession=new FormWebChatSession(webChatSession,() => { })){//We don't usually refresh prognotes after a double click
						formWebChatSession.ShowDialog();
					}
					break;
				case ProgNotesRowType.Rx:
					if(PIn.Enum<RxTypes>(row["RxType"].ToString(),isEnumAsString:true)!=RxTypes.Rx) {
						MsgBox.Show(this,"This is an access log for a prescription drug management program and cannot be edited or deleted.");
						return;
					}
					RxPat rx=RxPats.GetRx(rowPk);
					if(rx==null) {
						MsgBox.Show(this,"This prescription has been deleted by another user.");
					}
					else {
						using FormRxEdit formRxEdit=new FormRxEdit(Pd.Patient,rx);
						formRxEdit.ShowDialog();
						if(formRxEdit.DialogResult!=DialogResult.OK) {
							return;
						}
					}
					break;
				case ProgNotesRowType.LabCase:
					LabCase labCase=LabCases.GetOne(rowPk);
					if(labCase==null) {
						MsgBox.Show(this,"This LabCase has been deleted by another user.");
					}
					else {
						using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
						formLabCaseEdit.LabCaseCur=labCase;
						formLabCaseEdit.ShowDialog();
						//needs to always refresh due to complex ok/cancel
					}
					break;
				case ProgNotesRowType.Task:
					Task task=Tasks.GetOne(rowPk);
					if(task==null) {
						MsgBox.Show(this,"This task has been deleted by another user.");
					}
					else {
						FormTaskEdit formTaskEdit=new FormTaskEdit(task);
						formTaskEdit.Show();//non-modal
					}
					break;
				case ProgNotesRowType.Apt:
					FormApptEdit formApptEdit=new FormApptEdit(rowPk);//Disposed below because of variable.
					//PinIsVisible=false
					formApptEdit.IsInChartModule=true;
					formApptEdit.ShowDialog();
					if(formApptEdit.IsEcwCloseOD) {//Only true when using ECW and user clicks FormApptEdit.butComplete.
						this.FindForm().Close();
						return;
					}
					if(formApptEdit.DialogResult!=DialogResult.OK) {
						return;
					}
					formApptEdit.Dispose();
					if(!_isModuleSelected) {
						//Example: Sent the appt to the pinboard from the chart.
						//There is no need to perform a module selected at the end of this method since the module has been refreshed already.
						return;
					}
					break;
				case ProgNotesRowType.EmailMessage:
					EmailMessage emailMsg=EmailMessages.GetOne(rowPk);
					if(EmailMessages.IsSecureWebMail(emailMsg.SentOrReceived)) {
						//web mail uses special secure messaging portal
						using FormWebMailMessageEdit formWMME=new FormWebMailMessageEdit(Pd.PatNum,emailMsg);
						if(formWMME.ShowDialog()==DialogResult.Cancel) {//This will cause an unneccesary refresh in the case of a validation error with the webmail
							return;
						}
					}
					else {
						using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMsg);
						formEmailMessageEdit.ShowDialog();
						if(formEmailMessageEdit.DialogResult!=DialogResult.OK) {
							return;
						}
					}
					break;
				case ProgNotesRowType.Sheet:
					Sheet sheet=Sheets.GetSheet(rowPk);
					SheetUtilL.ShowSheet(sheet,Pd.Patient,FormSheetFillEdit_FormClosing);
					break;
				case ProgNotesRowType.FormPat:
					FormPat formPat=FormPats.GetOne(rowPk);
					using(FormFormPatEdit formFormPatEdit=new FormFormPatEdit()) {
						formFormPatEdit.FormPatCur=formPat;
						formFormPatEdit.ShowDialog();
						if(formFormPatEdit.DialogResult==DialogResult.OK) {
							ModuleSelected(Pd.PatNum);//Why is this called here and down 3 lines? Do we need the Allocator, or should we return here?
						}
					}
					break;
			}
			ModuleSelected(Pd.PatNum);
		}

		private void gridProg_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Delete || e.KeyCode==Keys.Back) {
				DeleteRows();
			}
		}

		private void listButtonCats_Click(object sender,EventArgs e) {
			FillProcButtons();
		}

		private void listDx_SelectionChangeCommitted(object sender,EventArgs e) {
			if(listDx.SelectedIndex==-1 //Just in case
				|| Defs.GetDefsForCategory(DefCat.Diagnosis,true).Any(x => x.DefNum==listDx.GetSelected<Def>().DefNum))//Cached def was hidden by another user.
			{
				return;
			}
			MsgBox.Show(this,"The selected Diagnosis has been hidden.");
			ModuleSelected(Pd.PatNum);
		}

		private void listViewButtons_Click(object sender,EventArgs e) {
			if(_procStatusNew==ProcStat.C) {
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show(this,"Set the procedure complete by setting the appointment complete.  "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
					return;
				}
				if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text))) {
					return;
				}
			}
			if(listViewButtons.SelectedIndices.Count==0) {
				return;
			}
			ProcButton procButtonCur=_arrayProcButtons[listViewButtons.SelectedIndices[0]];
			ProcButtonClicked(procButtonCur);
		}

		private void radioEntryC_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.C;
		}

		private void radioEntryCn_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.Cn;
		}

		private void radioEntryEC_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.EC;
		}

		private void radioEntryEO_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.EO;
		}

		private void radioEntryR_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.R;
		}

		private void radioEntryTP_CheckedChanged(object sender,EventArgs e) {
			_procStatusNew=ProcStat.TP;
		}

		public void TaskGoToEvent(object sender,CancelEventArgs e) {
			if(!_isModuleSelected) {
				return;
			}
			FormTaskEdit formTaskEdit=(FormTaskEdit)sender;
			TaskObjectType gotoType=formTaskEdit.GotoType;
			long keyNum=formTaskEdit.GotoKeyNum;
			if(gotoType==TaskObjectType.None) {
				ModuleSelected(Pd.PatNum);
				return;
			}
			if(gotoType==TaskObjectType.Patient) {
				if(keyNum!=0) {
					Patient pat=Patients.GetPat(keyNum);
					FormOpenDental.S_Contr_PatientSelected(pat,false);
					ModuleSelected(pat.PatNum);
					return;
				}
			}
			if(gotoType==TaskObjectType.Appointment) {
				//There's nothing to do here, since we're not in the appt module.
				return;
			}
		}

		private void textProcCode_Enter(object sender,EventArgs e) {
			if(textProcCode.Text==Lan.g(this,"Type Proc Code")) {
				textProcCode.Text="";
			}
		}

		private void textProcCode_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Return) {
				EnterTypedCode();
				e.Handled=true;
				e.SuppressKeyPress=true;
			}
		}

		private void textProcCode_TextChanged(object sender,EventArgs e) {
			if(textProcCode.Text=="d") {
				textProcCode.Text="D";
				textProcCode.SelectionStart=1;
			}
		}
		#endregion Methods - Event Handlers - Tab EnterTx

		#region Methods - Event Handlers - Tab MissingTeeth
		private void butEdentulous_Click(object sender,EventArgs e) {
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.Missing);
			for(int i=1;i<=32;i++) {
				ToothInitials.SetValueQuick(Pd.PatNum,i.ToString(),ToothInitialType.Missing,0);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butHidden_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Hidden);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butMissing_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Missing);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butNotMissing_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Missing);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butUnhide_Click(object sender,EventArgs e) {
			if(listHidden.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select an item from the list first.");
				return;
			}
			ToothInitials.ClearValue(Pd.PatNum,(string)_arrayListHiddenTeeth[listHidden.SelectedIndex],ToothInitialType.Hidden);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}		
		#endregion Methods - Event Handlers - Tab MissingTeeth

		#region Methods - Event Handlers - Tab Movements
		private void butApplyMovements_Click(object sender,EventArgs e) {
			if(!textShiftM.IsValid()
				|| !textShiftO.IsValid()
				|| !textShiftB.IsValid()
				|| !textRotate.IsValid()
				|| !textTipM.IsValid()
				|| !textTipB.IsValid())
			{
				MsgBox.Show(this,"Please fix errors first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				if(textShiftM.Text!=""){
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftM,PIn.Float(textShiftM.Text));
				}
				if(textShiftO.Text!="") {
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftO,PIn.Float(textShiftO.Text));
				}
				if(textShiftB.Text!="") {
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftB,PIn.Float(textShiftB.Text));
				}
				if(textRotate.Text!="") {
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Rotate,PIn.Float(textRotate.Text));
				}
				if(textTipM.Text!="") {
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipM,PIn.Float(textTipM.Text));
				}
				if(textTipB.Text!="") {
					ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipB,PIn.Float(textTipB.Text));
				}
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butClearAllMovements_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Clear all movements on all teeth for this patient?")) {
				return;
			}
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.Rotate);
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.ShiftB);
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.ShiftM);
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.ShiftO);
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.TipB);
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.TipM);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butClearSelectedMovements_Click(object sender, EventArgs e) {
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Rotate);
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftB);
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftM);
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftO);
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipB);
				ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipM);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butRotateMinus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Rotate,-20);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butRotatePlus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Rotate,20);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftBminus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftB,-2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftBplus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftB,2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftMminus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftM,-2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftMplus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftM,2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftOminus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftO,-2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butShiftOplus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftO,2);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butTipBminus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipB,-10);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butTipBplus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipB,10);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butTipMminus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipM,-10);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butTipMplus_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.AddMovement(Pd.ListToothInitials,Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipM,10);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}
		#endregion Methods - Event Handlers - Tab Movements

		#region Methods - Event Handlers - Tab Primary
		private void butAllPerm_Click(object sender,EventArgs e) {
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.Primary);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butAllPrimary_Click(object sender,EventArgs e) {
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.Primary);
			for(int i=1;i<=32;i++){
				ToothInitials.SetValueQuick(Pd.PatNum,i.ToString(),ToothInitialType.Primary,0);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butMixed_Click(object sender,EventArgs e) {
			ToothInitials.ClearAllValuesForType(Pd.PatNum,ToothInitialType.Primary);
			string[] arrayPriTeeth=new string[] 
				{"1","2","4","5","6","11","12","13","15","16","17","18","20","21","22","27","28","29","31","32"};
			for(int i=0;i<arrayPriTeeth.Length;i++) {
				ToothInitials.SetValueQuick(Pd.PatNum,arrayPriTeeth[i],ToothInitialType.Primary,0);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butPerm_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				if(Tooth.IsPrimary(_toothChartRelay.SelectedTeeth[i])){
					ToothInitials.ClearValue(Pd.PatNum,Tooth.PriToPerm(_toothChartRelay.SelectedTeeth[i])
						,ToothInitialType.Primary);
				}
				else{
					ToothInitials.ClearValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i]
						,ToothInitialType.Primary);
				}
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}

		private void butPrimary_Click(object sender,EventArgs e) {
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
				ToothInitials.SetValue(Pd.PatNum,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Primary);
			}
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(false);
		}
		#endregion Methods - Event Handlers - Tab Primary

		#region Methods - Event Handlers - Tab Planned Appts
		///<summary>This is the listener for the Delete button.</summary>
		private void butClear_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete planned appointment(s)?")) {
				return;
			}
			for(int i=0;i<gridPlanned.SelectedIndices.Length;i++) {			
				Appointments.Delete(PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["AptNum"].ToString()),true);
			}
			ModuleSelected(Pd.PatNum);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(gridPlanned.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please only select one item first.");
				return;
			}
			int idx=gridPlanned.SelectedIndices[0];
			if(idx==_listPlannedAppt.Count-1) {
				return;
			}
			DataRow rowSelectedAppt=_listPlannedAppt[idx];//Get selected data row
			DataRow rowBelowSelectedAppt=_listPlannedAppt[idx+1];//Get data row below the selected, since we are moving down we are going to need it to adjust its item order
			moveItemOrderHelper(rowSelectedAppt,PIn.Int(rowBelowSelectedAppt["ItemOrder"].ToString()));//Sets the selected rows item order = the above rows and adjust everything inbetween
			saveChangesToDBHelper();//Loops through list, gets PlannedAppt, sets the new ItemOrder and then updates if needed
			Pd.TableProgNotes=ChartModules.GetProgNotes(Pd.PatNum,checkAudit.Checked);
			Pd.TablePlannedAppts=ChartModules.GetPlannedApt(Pd.PatNum);
			_listSelectedAptNums.Clear();
			FillPlanned();
			gridPlanned.SetSelected(idx+1,true);
		}

		private void butNew_Click(object sender,EventArgs e) {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a Patient.");
				return;
			}
			List<long> listProcNums=null;
			if(checkTreatPlans.Checked && gridTpProcs.SelectedGridRows.Count>0) {//Showing TPs and user has proc selections.
				if( _listTreatPlans[gridTreatPlans.GetSelectedIndex()].TPStatus!=TreatPlanStatus.Active) {//Only allow pre selecting procs on active TP.
					string msgText=Lans.g(this,"Planned appointments can only be created using an Active treatment plan when selecting Procedures.")+"\r\n"
						+Lans.g(this,"Continue without selections?");
					if(MessageBox.Show(msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
						return;	
					}
				}
				else {
					listProcNums=gridTpProcs.SelectedGridRows
						.FindAll(x => x.Tag!=null && x.Tag.GetType()==typeof(ProcTP))//ProcTP's only
						.Select(x => ((ProcTP)(x.Tag)).ProcNumOrig).ToList();//get ProcNums
				}
			}
			int itemOrder=Pd.TablePlannedAppts.Rows.Count+1;
			if(AppointmentL.CreatePlannedAppt(Pd.Patient,itemOrder,listProcNums)==PlannedApptStatus.FillGridNeeded) {
				FillPlanned();
			}
		}

		///<summary></summary>
		private void butPin_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first");
				return;
			}
			if(Pd.Patient.PatStatus.In(PatientStatus.Archived,PatientStatus.Deceased)) {
				MsgBox.Show(this,"Appointments cannot be scheduled for "+Pd.Patient.PatStatus.ToString().ToLower()+" patients.");
				return;
			}
			List<long> listAptNums=new List<long>();
			for(int i=0;i<gridPlanned.SelectedIndices.Length;i++) {
				long aptNum=PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["AptNum"].ToString());
				if(Procedures.GetProcsForSingle(aptNum,true).Count(x => x.ProcStatus==ProcStat.C)>0) {
					MsgBox.Show(this,"Not allowed to send a planned appointment to the pinboard if completed procedures are attached. Edit the planned "
						+"appointment first.");
					return;
				}
				ApptStatus aptStatus=(ApptStatus)(PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["AptStatus"].ToString()));
				if(aptStatus==ApptStatus.Complete) {
					//Warn the user they are moving a completed appointment.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to move an already completed appointment.  Continue?")) {
						return;
					}
					listAptNums.Add(PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["SchedAptNum"].ToString()));
				}
				else if(aptStatus==ApptStatus.Scheduled) {
					//Warn the user they are moving an already scheduled appointment.
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"You are about to move an appointment already on the schedule.  Continue?")) {
						return;
					}
					listAptNums.Add(PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["SchedAptNum"].ToString()));
				}
				else if(aptStatus==ApptStatus.UnschedList || aptStatus==ApptStatus.Broken) {
					//Dont need to warn user, just put onto the pinboard.
					listAptNums.Add(PIn.Long(_listPlannedAppt[gridPlanned.SelectedIndices[i]]["SchedAptNum"].ToString())); 
				}
				else { //No appointment
					listAptNums.Add(aptNum);
				}
			}
			GotoModule.PinToAppt(listAptNums,0);
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridPlanned.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			if(gridPlanned.SelectedIndices.Length>1) {
				MsgBox.Show(this,"Please only select one item first.");
				return;
			}
			int idx=gridPlanned.SelectedIndices[0];
			if(idx==0) {
				return;
			}
			DataRow rowSelectedAppt=_listPlannedAppt[idx];//Get selected data row
			//Get data row above the selected, since we are moving up we are going to need it to adjust its item order
			DataRow rowAboveSelectedAppt=_listPlannedAppt[idx-1];//idx guaranteed to be >0
			moveItemOrderHelper(rowSelectedAppt,PIn.Int(rowAboveSelectedAppt["ItemOrder"].ToString()));//Sets the selected rows item order = the above rows and adjust everything inbetween
			saveChangesToDBHelper();//Loops through list, gets PlannedAppt, sets the new ItemOrder and then updates if needed
			Pd.TableProgNotes=ChartModules.GetProgNotes(Pd.PatNum,checkAudit.Checked);
			Pd.TablePlannedAppts=ChartModules.GetPlannedApt(Pd.PatNum);
			_listSelectedAptNums.Clear();
			FillPlanned();
			gridPlanned.SetSelected(idx-1,true);
		}

		private void checkDone_Click(object sender,EventArgs e) {
			Patient patOld=Pd.Patient.Copy();
			if(checkDone.Checked) {
				if(_tablePlannedAll.Rows.Count>0) {
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"ALL planned appointment(s) will be deleted. Continue?")) {
						checkDone.Checked=false;
						return; 
					}
					for(int i=0;i<_tablePlannedAll.Rows.Count;i++) {
						Appointments.Delete(PIn.Long(_tablePlannedAll.Rows[i]["AptNum"].ToString()),true);
					}
				}
				Pd.Patient.PlannedIsDone=true;
				Patients.Update(Pd.Patient,patOld);
			}
			else{
				Pd.Patient.PlannedIsDone=false;
				Patients.Update(Pd.Patient,patOld);
			}
			ModuleSelected(Pd.PatNum);
		}

		private void checkShowCompleted_CheckedChanged(object sender,EventArgs e) {
			_listSelectedAptNums.Clear();
			foreach(int index in gridPlanned.SelectedIndices) {
				_listSelectedAptNums.Add(PIn.Long(_listPlannedAppt[index]["AptNum"].ToString()));
			}
			FillPlanned();
		}

		private void gridPlanned_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long aptNum=(long)gridPlanned.ListGridRows[e.Row].Tag;
			using FormApptEdit formApptEdit=new FormApptEdit(aptNum);
			formApptEdit.ShowDialog();
			if(Programs.UsingOrion) {
				if(formApptEdit.DialogResult==DialogResult.OK) {
					if(_isModuleSelected){//module might have changed, such as when clicking GoTo on a task launched from the appointment edit window.
						ModuleSelected(Pd.PatNum);//if procs were added in appt, then this will display them
					}
				}
			}
			else {
				if(_isModuleSelected) {
					ModuleSelected(Pd.PatNum);//if procs were added in appt, then this will display them
				}
			}
			gridPlanned.SetAll(false);
			for(int i=0;i<gridPlanned.ListGridRows.Count;i++) {
				if((long)gridPlanned.ListGridRows[i].Tag==aptNum) {
					gridPlanned.SetSelected(i,true);
				}
			}
		}
		#endregion Methods - Event Handlers - Tab Planned Appts

		#region Methods - Event Handlers - Tab Show
		private void button1_Click(object sender, System.EventArgs e) {
			//sometimes used for testing purposes
		}

		private void butShowAll_Click(object sender,System.EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			checkShowTP.Checked=true;
			checkShowC.Checked=true;
			checkShowE.Checked=true;
			checkShowR.Checked=true;
			checkShowCn.Checked=true;
			checkNotes.Checked=true;
			checkAppt.Checked=true;
			checkComm.Checked=true;
			checkCommFamily.Checked=true;
			if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
				checkCommSuperFamily.Checked=true;
			}
			checkLabCase.Checked=true;
			checkRx.Checked=true;
			checkShowTeeth.Checked=false;
			checkTasks.Checked=true;
			checkEmail.Checked=true;
			checkSheets.Checked=true;
			FillProgNotes();
		}

		private void butShowNone_Click(object sender,System.EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			checkShowTP.Checked=false;
			checkShowC.Checked=false;
			checkShowE.Checked=false;
			checkShowR.Checked=false;
			checkShowCn.Checked=false;
			checkNotes.Checked=false;
			checkAppt.Checked=false;
			checkComm.Checked=false;
			checkCommFamily.Checked=false;
			checkCommSuperFamily.Checked=false;
			checkLabCase.Checked=false;
			checkRx.Checked=false;
			checkShowTeeth.Checked=false;
			checkTasks.Checked=false;
			checkEmail.Checked=false;
			checkSheets.Checked=false;
			FillProgNotes();
		}

		private void checkAppt_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkAudit_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkComm_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			if(!checkComm.Checked) {
				checkCommFamily.Checked=false;//uncheck family when comm is unchecked
				checkCommSuperFamily.Checked=false;//uncheck super family when comm is unchecked
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkCommFamily_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			if(checkCommFamily.Checked) {
				checkComm.Checked=true;//check comm when family is checked
			}
			else {
				checkCommSuperFamily.Checked=false;//uncheck super family when family is unchecked
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkCommSuperFamily_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			if(checkCommSuperFamily.Checked) {
				checkCommFamily.Checked=true;//check family when super family is checked
				checkComm.Checked=true;//check comm when super family is checked
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkEmail_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkLabCase_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkNotes_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkRx_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkSheets_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowC_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowCn_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowE_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowR_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowTP_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void checkShowTeeth_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			if(checkShowTeeth.Checked) {
				checkShowTP.Checked=true;
				checkShowC.Checked=true;
				checkShowE.Checked=true;
				checkShowR.Checked=true;
				checkShowCn.Checked=true;
				checkNotes.Checked=true;
				checkAppt.Checked=false;
				checkComm.Checked=false;
				checkCommFamily.Checked=false;
				checkCommSuperFamily.Checked=false;
				checkLabCase.Checked=false;
				checkRx.Checked=false;
				checkEmail.Checked=false;
				checkTasks.Checked=false;
				checkSheets.Checked=false;
			}
			else {
				checkShowTP.Checked=true;
				checkShowC.Checked=true;
				checkShowE.Checked=true;
				checkShowR.Checked=true;
				checkShowCn.Checked=true;
				checkNotes.Checked=true;
				checkAppt.Checked=true;
				checkComm.Checked=true;
				checkCommFamily.Checked=true;
				if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
					checkCommSuperFamily.Checked=true;
				}
				checkLabCase.Checked=true;
				checkRx.Checked=true;
				checkEmail.Checked=true;
				checkTasks.Checked=true;
				checkSheets.Checked=true;
			}
			FillProgNotes(true);
		}

		private void checkTasks_Click(object sender,EventArgs e) {
			if(gridChartViews.ListGridRows.Count>0) {
				labelCustView.Visible=true;
			}
			chartCustViewChanged=true;
			FillProgNotes();
		}

		private void labelSearchClear_Click(object sender,EventArgs e) {
			textSearch.Text="";
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			labelSearchClear.Visible=(textSearch.Text!="");
			_isSearchPending=true;
			//search will happen when the timer ticks
		}

		private void _timerSearch_Tick(object sender, EventArgs e){
			//jordan 2020-08-05-All threading for this search has been removed. Timers always work better in UI.
			if(!_isSearchPending){
				return;
			}
			_isSearchPending=false;
			if(textSearch.Text=="") {
				_dateTimeLastSearch=DateTime.Now;//This is so that if a previous search is running, it won't fill the grid when it finishes.
				_previousSearchText="";
				_listSearchResults?.Clear();
				_listSearchResults=null;
				if(!_isFillingProgNotes) {//if we are currently filling the progress notes, we do not want to do so again
					FillProgNotes();
				}
				return;
			}
			SearchProgNotes();
		}
		#endregion Methods - Event Handlers - Tab Show

		#region Methods - Event Handlers - Tab Draw
		private void butAddText_Click(object sender,EventArgs e) {
			using InputBox inputBox=new InputBox("Text");
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK){
				return;
			}
			ToothInitial toothInitial=new ToothInitial();
			toothInitial.SetText(new PointF(),inputBox.textResult.Text);
			toothInitial.InitialType=ToothInitialType.Text;
			toothInitial.PatNum=Pd.PatNum;
			toothInitial.ColorDraw=panelDrawColor.BackColor;
			ToothInitials.Insert(toothInitial);
			radioMoveText.Checked=true;
			_toothChartRelay.CursorTool=CursorTool.MoveText;
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void butColorOther_Click(object sender,EventArgs e) {
			ColorDialog colorDialog=new ColorDialog();
			colorDialog.Color=butColorOther.BackColor;
			if(colorDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			panelDrawColor.BackColor=colorDialog.Color;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void butDeleteText_Click(object sender,EventArgs e) {
			if(listBoxText.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item above, first.");
				return;
			}
			ToothInitials.Delete(listBoxText.GetSelected<ToothInitial>());
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void listBoxText_DoubleClick(object sender,EventArgs e) {
			if(listBoxText.SelectedIndex==-1){
				return;
			}
			ToothInitial toothInitial=listBoxText.GetSelected<ToothInitial>();
			using InputBox inputBox=new InputBox("Text",toothInitial.GetTextString());
			inputBox.ShowDialog();
			if(inputBox.DialogResult!=DialogResult.OK){
				return;
			}
			toothInitial.SetText(toothInitial.GetTextPoint(),inputBox.textResult.Text);
			ToothInitials.Update(toothInitial);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void panelBlack_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=Color.Black;
			_toothChartRelay.ColorDrawing=Color.Black;
		}

		private void panelCdark_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelCdark.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelClight_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelClight.BackColor;
		}		
		
		private void panelDrawColor_DoubleClick(object sender,EventArgs e) {
			//do nothing
		}

		private void panelECdark_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelECdark.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelEClight_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelEClight.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelEOdark_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelEOdark.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelEOlight_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelEOlight.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelRdark_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelRdark.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelRlight_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelRlight.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelTPdark_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelTPdark.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void panelTPlight_Click(object sender,EventArgs e) {
			panelDrawColor.BackColor=panelTPlight.BackColor;
			_toothChartRelay.ColorDrawing=panelDrawColor.BackColor;
		}

		private void radioColorChanger_Click(object sender,EventArgs e) {
			_toothChartRelay.CursorTool=CursorTool.ColorChanger;
		}

		private void radioEraser_Click(object sender,EventArgs e) {
			_toothChartRelay.CursorTool=CursorTool.Eraser;
		}

		private void radioMoveText_Click(object sender,EventArgs e) {
			_toothChartRelay.CursorTool=CursorTool.MoveText;
		}

		private void radioPen_Click(object sender,EventArgs e) {
			_toothChartRelay.CursorTool=CursorTool.Pen;
		}

		private void radioPointer_Click(object sender,EventArgs e) {
			_toothChartRelay.CursorTool=CursorTool.Pointer;
		}
		#endregion Methods - Event Handlers - Tab Draw

		#region Methods - Event Handlers - Text Fields
		private void textTreatmentNotes_TextChanged(object sender,EventArgs e) {
			TreatmentNoteChanged=true;
		}

		private void textTreatmentNotes_Leave(object sender,EventArgs e) {
			UpdateTreatmentNote();
		}

		private void textTreatmentNotes_MouseLeave(object sender,EventArgs e) {
			UpdateTreatmentNote();
		}
		#endregion Methods - Event Handlers - Text Fields

		#region Methods - Event Handlers - Toolbars
		private void ToolBarMain_ButtonClick(object sender,ODToolBarButtonClickEventArgs e) {
			if(e.Button.Tag.GetType()==typeof(string)) {
				//standard predefined button
				switch(e.Button.Tag.ToString()) {
					case "Rx":
						Tool_Rx_Click();
						break;
					case "eRx":
						Tool_eRx_Click();
						break;
					case "LabCase":
						Tool_LabCase_Click();
						break;
					case "Perio":
						Tool_Perio_Click();
						break;
					case "Ortho":
						Tool_Ortho_Click();
						break;
					case "Anesthesia":
						Tool_Anesthesia_Click();
						break;
					case "CCD":
						Tool_CCD_Click();
						break;
					case "Consent":
						Tool_Consent_Click();
						break;
					case "Commlog"://only for eCW
						Tool_Commlog_Click();
						break;
					case "ToothChart":
						Tool_ToothChart_Click();
						break;
					case "ExamSheet":
						Tool_ExamSheet_Click();
						break;
					case "EHR":
						Tool_EHR_Click(false);
						break;
					case "HL7":
						Tool_HL7_Click();
						break;
					case "MedLab":
						Tool_MedLab_Click();
						break;
					case "Layout":
						Tool_Layout_Click();
						break;
				}
			}
			else if(e.Button.Tag.GetType()==typeof(Program)) {
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,Pd.Patient);
			}
		}
		#endregion Methods - Event Handlers - Toolbars

		#region Methods - Event Handlers - toothChart
		private void toothChart_SegmentDrawn(object sender,ToothChartDrawEventArgs e) {
			string stringSegment=e.DrawingSegement;
			if(radioPen.Checked) {
				ToothInitial toothInitial=new ToothInitial();
				toothInitial.DrawingSegment=stringSegment;
				toothInitial.InitialType=ToothInitialType.Drawing;
				toothInitial.PatNum=Pd.PatNum;
				toothInitial.ColorDraw=panelDrawColor.BackColor;
				ToothInitials.Insert(toothInitial);
				Pd.ClearAndFill(EnumPdTable.ToothInitial);
				FillToothChart(true);
			}
			else if(radioMoveText.Checked){
				//shouldn't happen
			}
			else if(radioEraser.Checked) {
				//for(int i=0;i<ToothInitialList.Count;i++) {
				for(int i=Pd.ListToothInitials.Count-1;i>=0;i--) {//go backwards
					if(Pd.ListToothInitials[i].InitialType!=ToothInitialType.Drawing) {
						continue;
					}
					if(Pd.ListToothInitials[i].DrawingSegment!=stringSegment) {
						continue;
					}
					ToothInitials.Delete(Pd.ListToothInitials[i]);
					Pd.ListToothInitials.RemoveAt(i);
					//no need to refresh since that's handled by the toothchart.
				}
			}
			else if(radioColorChanger.Checked) {
				for(int i=0;i<Pd.ListToothInitials.Count;i++) {
					if(Pd.ListToothInitials[i].InitialType!=ToothInitialType.Drawing) {
						continue;
					}
					if(Pd.ListToothInitials[i].DrawingSegment!=stringSegment) {
						continue;
					}
					Pd.ListToothInitials[i].ColorDraw=panelDrawColor.BackColor;
					ToothInitials.Update(Pd.ListToothInitials[i]);
					FillToothChart(true);//this annoyingly changes the cursor to pointer, but there is not a simple solution because the cursors are embedded in the chart.
				}
			}
		}

		private void _toothChart_TextMoved(object sender,TextMovedEventArgs e) {
			ToothInitial toothInitial=_listToothInitialsText.FirstOrDefault(x=>x.ToothInitialNum==e.ToothInitialNum);
			if(toothInitial==null){
				return;//shouldn't happen
			}
			if(e.LocationNew!=new PointF()){//0,0
				toothInitial.DrawText=e.LocationNew.X.ToString()+","+e.LocationNew.Y.ToString()+";"+toothInitial.GetTextString();
			}
			if(e.ColorNew!=Color.Empty){//ARGB=0,0,0,0
				toothInitial.ColorDraw=e.ColorNew;
			}
			ToothInitials.Update(toothInitial);
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			FillToothChart(true);
		}

		private void toothChart_ToothSelectionsChanged(object sender) {
			if(checkShowTeeth.Checked) {
				FillProgNotes(true,false);
			}
			FillMovementsAndHidden();
		}

		private void trackToothProcDates_ValueChanged(object sender,EventArgs e) {
			if(PrefC.IsODHQ) {
				return;//ODInternalCustomerGrid is filled instead
			}
			textToothProcDate.Text=_listProcDates[trackToothProcDates.Value].ToShortDateString();
			FillToothChart(true,_listProcDates[trackToothProcDates.Value]);
		}
		#endregion Methods - Event Handlers - toothChart

		#region Methods - Public
		public void FillProgNotes(bool retainToothSelection=false,bool doRefreshData=true,bool isSearch=false,bool isForceFirstPage=false,
			bool hasPageNav=true)
		{
			_isFillingProgNotes=true;
			FillProgNotesInternal(retainToothSelection,doRefreshData,isSearch,isForceFirstPage,hasPageNav);
			_isFillingProgNotes=false;
		}

		///<summary>Do not call this method. Call FillProgNotes instead.</summary>
		public void FillProgNotesInternal(bool retainToothSelection=false,bool doRefreshData=true,bool isSearch=false,bool isForceFirstPage=false,
			bool hasPageNav=true)
		{
			Plugins.HookAddCode(this,"ContrChart.FillProgNotes_begin");
			//Make a reference to all of the tags (custom DataRow) that are currently selected within gridProg for reselecting.
			List<DataRow> listSelectedDataRows=gridProg.SelectedTags<DataRow>();
			gridProg.BeginUpdate();
			gridProg.Columns.Clear();
			GridColumn col;
			List<DisplayField> listDisplayFields;
			//DisplayFields.RefreshCache();
			if(gridChartViews.ListGridRows.Count==0) {//No chart views, Use default values.
				listDisplayFields=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
				gridProg.Title=Lan.g("TableProg","Progress Notes");
				if(!chartCustViewChanged) {
					checkSheets.Checked=true;
					checkTasks.Checked=true;
					checkEmail.Checked=true;
					checkCommFamily.Checked=true;
					if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
						checkCommSuperFamily.Checked=true;
					}
					checkAppt.Checked=true;
					checkLabCase.Checked=true;
					checkRx.Checked=true;
					checkComm.Checked=true;
					checkShowTP.Checked=true;
					checkShowC.Checked=true;
					checkShowE.Checked=true;
					checkShowR.Checked=true;
					checkShowCn.Checked=true;
					checkNotes.Checked=true;
					checkShowTeeth.Checked=false;
					checkAudit.Checked=false;
					textShowDateRange.Text=Lan.g(this,"All Dates");
				}
			}
			else {
				if(_chartViewCurDisplay==null) {
					_chartViewCurDisplay=ChartViews.GetFirst();
				}
				listDisplayFields=DisplayFields.GetForChartView(_chartViewCurDisplay.ChartViewNum);
				gridProg.Title=_chartViewCurDisplay.Description;
				if(!chartCustViewChanged) {
					checkSheets.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.Sheets)==ChartViewObjs.Sheets;
					checkTasks.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.Tasks)==ChartViewObjs.Tasks;
					checkEmail.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.Email)==ChartViewObjs.Email;
					checkCommFamily.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.CommLogFamily)==ChartViewObjs.CommLogFamily;
					if(PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {
						checkCommSuperFamily.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.CommLogSuperFamily)==ChartViewObjs.CommLogSuperFamily;
					}
					checkAppt.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.Appointments)==ChartViewObjs.Appointments;
					checkLabCase.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.LabCases)==ChartViewObjs.LabCases;
					checkRx.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.Rx)==ChartViewObjs.Rx;
					checkComm.Checked=(_chartViewCurDisplay.ObjectTypes & ChartViewObjs.CommLog)==ChartViewObjs.CommLog;
					checkShowTP.Checked=(_chartViewCurDisplay.ProcStatuses & ChartViewProcStat.TP)==ChartViewProcStat.TP;
					checkShowC.Checked=(_chartViewCurDisplay.ProcStatuses & ChartViewProcStat.C)==ChartViewProcStat.C;
					checkShowE.Checked=(_chartViewCurDisplay.ProcStatuses & ChartViewProcStat.EC)==ChartViewProcStat.EC;
					checkShowR.Checked=(_chartViewCurDisplay.ProcStatuses & ChartViewProcStat.R)==ChartViewProcStat.R;
					checkShowCn.Checked=(_chartViewCurDisplay.ProcStatuses & ChartViewProcStat.Cn)==ChartViewProcStat.Cn;
					checkShowTeeth.Checked=_chartViewCurDisplay.SelectedTeethOnly;
					checkNotes.Checked=_chartViewCurDisplay.ShowProcNotes;
					checkAudit.Checked=_chartViewCurDisplay.IsAudit;
					checkTPChart.Checked=_chartViewCurDisplay.IsTpCharting;
					SetDateRange();
					FillDateRange();
					gridChartViews.SetSelected(_chartViewCurDisplay.ItemOrder,true);
					if(_isDistributorKey) {
						gridCustomerViews.SetSelected(_chartViewCurDisplay.ItemOrder,true);
					}
				}
				else {
					gridChartViews.SetAll(false);
					if(_isDistributorKey) {
						gridCustomerViews.SetAll(false);
					}
				}
			}
			bool showSelectedTeeth=checkShowTeeth.Checked;
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				checkShowTeeth.Checked=false;
			}
			if(_isModuleSelected && doRefreshData) {
				Pd.TableProgNotes=ChartModules.GetProgNotes(Pd.PatNum,checkAudit.Checked,GetChartModuleComponents());
				Pd.TablePlannedAppts=ChartModules.GetPlannedApt(Pd.PatNum);
			}
			if(checkTreatPlans.Checked) {
				checkShowTP.Enabled=false;
				checkShowC.Enabled=false;
				checkShowE.Enabled=false;
				checkShowR.Enabled=false;
				checkShowCn.Enabled=false;
				checkNotes.Enabled=false;
			}
			else {
				checkShowTP.Enabled=true;
				checkShowC.Enabled=true;
				checkShowE.Enabled=true;
				checkShowR.Enabled=true;
				checkShowCn.Enabled=true;
				checkNotes.Enabled=true;
			}
			for(int i=0;i<listDisplayFields.Count;i++) {
				if(listDisplayFields[i].Description=="") {
					col=new GridColumn(listDisplayFields[i].InternalName,listDisplayFields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth);
				}
				if(listDisplayFields[i].InternalName=="Th") {
					col.SortingStrategy=GridSortingStrategy.ToothNumberParse;
				}
				if(listDisplayFields[i].InternalName=="Date") {
					col.SortingStrategy=GridSortingStrategy.DateParse;
				}
				if(listDisplayFields[i].InternalName=="Amount") {
					col.SortingStrategy=GridSortingStrategy.AmountParse;
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(listDisplayFields[i].InternalName=="Proc Code"
					|| listDisplayFields[i].InternalName=="User"
					|| listDisplayFields[i].InternalName=="Signed"
					|| listDisplayFields[i].InternalName=="Locked"
					|| listDisplayFields[i].InternalName=="HL7 Sent")
				{
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridProg.Columns.Add(col);
			}
			if(gridProg.Columns.Count<3) {//0 wouldn't be possible.
				gridProg.NoteSpanStart=0;
				gridProg.NoteSpanStop=gridProg.Columns.Count-1;
			}
			else {
				gridProg.NoteSpanStart=2;
				if(gridProg.Columns.Count>7) {
					gridProg.NoteSpanStop=7;
				}
				else {
					gridProg.NoteSpanStop=gridProg.Columns.Count-1;
				}
			}
			gridProg.ListGridRows.Clear();//Needed even when paging handles rows for us due to the change in columns and EndUpdate() logic.
			gridProg.EndUpdate();
			//Type type;
			if(IsPatientNull()) {
				FillToothChart(false);//?
				if(IsTPChartingAvailable) {
					FillListPriorities();//Mimics old ChartLayoutHelper logic
				}
				ToggleCheckTreatPlans();//Mimics old ChartLayoutHelper logic
				FillTreatPlans();
				FillTpProcs();
				return;
			}
			FillOrthoDateIfNeeded(forceRefresh:true);
			DataTable table=new DataTable();
			if(isSearch && _listSearchResults != null) {
				table=Pd.TableProgNotes.Clone();
				for(int i=0;i<_listSearchResults.Count;i++) {
					table.Rows.Add(_listSearchResults[i].ItemArray);
				}
			}
			else {
				table=Pd.TableProgNotes;
				textSearch.Text="";
			}
			if(doRefreshData || Pd.ListProcGroupItems==null) {
				if(_isModuleSelected) {
					Pd.ListProcGroupItems=ProcGroupItems.GetPatientData(Pd.PatNum);
				}
				else {
					Pd.ListProcGroupItems=new List<ProcGroupItem>();
				}
			}
			_listDataRowsProcsForGraphical=new List<DataRow>();
			List<long> listProcNums=new List<long>();//a list of all procNums of procs that will be visible
			if(checkShowTeeth.Checked) {
				//we will want to see groupnotes that are attached to any procs that should be visible.
				foreach(DataRow rowCur in table.Rows) {
					string procNumStr=rowCur["ProcNum"].ToString();
					if(procNumStr=="0") {//if this is not a procedure
						continue;
					}
					if(rowCur["ProcCode"].ToString()==ProcedureCodes.GroupProcCode) {
						continue;//skip procgroups
					}
					if(ShouldDisplayProc(rowCur)) {
						listProcNums.Add(PIn.Long(procNumStr));//remember that procnum
					}
				}
			}
			bool isSamePat=false;
			if(_isModuleSelected) {
				if(gridProg.Tag!=null && ((long)gridProg.Tag)==Pd.PatNum) {
					isSamePat=true;
				}
				gridProg.Tag=Pd.PatNum;
			}
			List<int> listSelectedIndicies;
			//Filter out any DataRows that should not be displayed to the user.
			List<DataRow> listDataRows=table.Select()
				.Where(x => DoesGridProgRowPassFilter(x,Pd.ListProcGroupItems,listProcNums))
				.ToList();
			if(isSamePat && listSelectedDataRows.Count==0 && !isForceFirstPage) {//Same patient without previously selected rows.
				listSelectedIndicies=new List<int>();//By passing in an empty list gridProg paging system will attempt to select the current page.
			}
			else if(isSamePat && listSelectedDataRows.Count>0 && !isForceFirstPage) {
				ODDataRowComparer comparer=new ODDataRowComparer();
				//Compare the current tag of every row in our grid to the previously selected rows.
				listSelectedIndicies=listSelectedDataRows.Select(x =>
					listDataRows.FindIndex(y => comparer.Equals(y,x))
				).ToList();
				//Remove any rows that were not found so that we either select a known row or try our best to preserve the current page.
				listSelectedIndicies.RemoveAll(x => x<0);
			}
			else {//New patient selected or same patient and forcing to go to first page, do not maintain page or selection.
				listSelectedIndicies=null;//null list will result in gridProd paging to go to first page.
			}
			Func<object,GridRow> funcConstructGridRow= dataRow => GridProgRowConstruction(dataRow as DataRow,listDisplayFields);
			gridProg.FuncConstructGridRow=funcConstructGridRow;
			//gridProg.FuncConstructGridRow=((t) => GridProgRowConstruction((t as DataRow),listDisplayFields));
			gridProgPageNav.Enabled=hasPageNav;//If disabled fill the entire grid with all gridRows and sort by the column header.
			if(hasPageNav) {
				gridProg.MaxPageRows=_gridProgMaxPageRowsDefault;
			}
			else {
				gridProg.MaxPageRows=listDataRows.Count;
			}
			gridProg.SetPagingData(listDataRows,listSelectedIndicies);
			if(IsTPChartingAvailable) {
				FillListPriorities();//Mimics old ChartLayoutHelper logic
			}
			ToggleCheckTreatPlans();//Mimics old ChartLayoutHelper logic
			List<long> listTreatPlanNums=new List<long>();
			if(_isModuleSelected && gridTreatPlans.SelectedIndices.Length>0) {
				listTreatPlanNums=gridTreatPlans.SelectedIndices
					.Where(x => _listTreatPlans[x].PatNum==Pd.PatNum)//must check PatNum because _listTreatPlans might be from previous patient
					.Select(x => _listTreatPlans[x].TreatPlanNum).ToList();
			}
			FillTreatPlans();
			for(int i=0;i<gridTreatPlans.ListGridRows.Count && listTreatPlanNums.Count>0;i++) {
				gridTreatPlans.SetSelected(i,listTreatPlanNums.Contains(_listTreatPlans[i].TreatPlanNum));
			}
			if(gridTreatPlans.GetSelectedIndex()>-1) {
				gridTreatPlans.ScrollToIndex(gridTreatPlans.GetSelectedIndex());
			}
			FillTpProcs();
			//create a copy of the original _procList for filling the tooth chart with all procs. Deep copy of filtered list required.
			List<string> listProcNumStrs=_listDataRowsProcsForGraphical.Select(x => x["ProcNum"].ToString()).ToList();
			_listDataRowsProcsOrig=Pd.TableProgNotes.Copy().Select().Where(x => listProcNumStrs.Contains(x["ProcNum"].ToString())).ToList();
			FillTrackSlider();
			FillToothChart(retainToothSelection);
			checkShowTeeth.Checked=showSelectedTeeth;
		}

		public void FillPtInfo(bool doRefreshData=true) {
			if(Plugins.HookMethod(this,"ContrChart.FillPtInfo",Pd.Patient)) {
				return;
			}
			textTreatmentNotes.Text="";
			if(IsPatientNull()) {
				gridPtInfo.BeginUpdate();
				gridPtInfo.ListGridRows.Clear();
				gridPtInfo.Columns.Clear();
				gridPtInfo.EndUpdate();
				TreatmentNoteChanged=false;
				return;
			}
			else {
				textTreatmentNotes.Text=Pd.PatientNote.Treatment;
				textTreatmentNotes.Enabled=true;
				textTreatmentNotes.Select(textTreatmentNotes.Text.Length+2,1);
				textTreatmentNotes.ScrollToCaret();
				TreatmentNoteChanged=false;
			}
			gridPtInfo.BeginUpdate();
			gridPtInfo.Columns.Clear();
			GridColumn col=new GridColumn("",100);//Lan.g("TableChartPtInfo",""),);
			gridPtInfo.Columns.Add(col);
			col=new GridColumn("",50){ IsWidthDynamic=true };//HScrollVisible is false, dynamic col width.
			gridPtInfo.Columns.Add(col);
			gridPtInfo.ListGridRows.Clear();
			GridCell cell;
			GridRow row;
			List<Def> listMiscColorDefs=Defs.GetDefsForCategory(DefCat.MiscColors);
			List<Def> listMiscColorShortDefs=Defs.GetDefsForCategory(DefCat.MiscColors,true);//Preserving old behavior.
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ChartPatientInformation);
			DisplayField fieldCur;
			for(int f=0;f<listDisplayFields.Count;f++) {
				fieldCur=listDisplayFields[f];
				row=new GridRow();
				//within a case statement, the row may be re-instantiated if needed, effectively removing the first cell added here:
				if(fieldCur.Description=="") {
					row.Cells.Add(fieldCur.InternalName);
				}
				else {
					row.Cells.Add(fieldCur.Description);
				}
				int ordinal=0;
				switch(fieldCur.InternalName) {
					#region ABC0
					case "ABC0":
						row.Cells.Add(Pd.Patient.CreditType);
						break;
					#endregion ABC0
					#region Age
					case "Age":
						row.Cells.Add(PatientLogic.DateToAgeString(Pd.Patient.Birthdate,Pd.Patient.DateTimeDeceased));
						break;
					#endregion Age
					#region Allergies
					case "Allergies":
						if(doRefreshData || Pd.ListAllergies==null) {
							Pd.ClearAndFill(EnumPdTable.Allergy);
						}
						List<Allergy> listAllergies=Pd.ListAllergies.FindAll(x=>x.StatusIsActive);
						row=new GridRow();
						cell=new GridCell();
						if(fieldCur.Description=="") {
							cell.Text=fieldCur.InternalName;
						}
						else {
							cell.Text=fieldCur.Description;
						}
						cell.Bold=YN.Yes;
						row.Cells.Add(cell);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabAllergies";
						if(listAllergies.Count>0) {
							row.Cells.Add("");
							gridPtInfo.ListGridRows.Add(row);
						}
						else {
							row.Cells.Add(Lan.g("TableChartPtInfo","none"));
						}
						for(int i=0;listAllergies.Count>i;i++) {
							row=new GridRow();
							//In the instance that an AllergyDef is somehow deleted/removed from the DB, create an AllergyDef - display only - based upon the selected 
							//number. Populate it with a description that tells the user that the Allergy is missing. 
							//The user can click on the allergy in FormMedical and remedy the issue on their own.
							AllergyDef allergyDef=AllergyDefs.GetOne(listAllergies[i].AllergyDefNum);
							if(allergyDef==null) {
								allergyDef=new AllergyDef() {
									AllergyDefNum=listAllergies[i].AllergyDefNum,
									Description="MISSING ALLERGY"
								};
							}
							cell=new GridCell(allergyDef.Description);
							cell.Bold=YN.Yes;
							cell.ColorText=Color.Red;
							row.Cells.Add(cell);
							row.Cells.Add(listAllergies[i].Reaction);
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
							row.Tag="tabAllergies";
							if(i!=listAllergies.Count-1) {
								gridPtInfo.ListGridRows.Add(row);
							}
						}
						break;
					#endregion Allergies
					#region AskToArriveEarly
					case "AskToArriveEarly":
						if(Pd.Patient.AskToArriveEarly==0) {
							row.Cells.Add("");
						}
						else {
							row.Cells.Add(Pd.Patient.AskToArriveEarly.ToString()+" minute(s)");
						}
						break;
					#endregion AskToArriveEarly
					#region Billing Type
					case "Billing Type":
						row.Cells.Add(Defs.GetName(DefCat.BillingTypes,Pd.Patient.BillingType));
						break;
					#endregion Billing Type
					#region Birthdate
					case "Birthdate":
						if(PrefC.GetBool(PrefName.PatientDOBMasked) || !Security.IsAuthorized(Permissions.PatientDOBView,true)) {
							row.Cells.Add(Patients.DOBFormatHelper(Pd.Patient.Birthdate,true));
							row.Tag="DOB";//Used later to tell if we're right clicking on the DOB row
						}
						else {
							row.Cells.Add(Patients.DOBFormatHelper(Pd.Patient.Birthdate,false));
						}
						break;
					#endregion Birthdate
					#region Broken Appts
					case "Broken Appts":
						row.Tag="Broken Appts";
						int count=0;
						DataTable table=Pd.TableProgNotes;
						if(ProcedureCodes.IsValidCode("D9986")) {
							foreach(DataRow rowCur in table.Rows.OfType<DataRow>().Where(x => x["ProcNum"].ToString()!="0")) {
								if(PIn.String(rowCur["ProcCode"].ToString())=="D9986") {
									count++;
								}
							}
						}
						else {
							count=Adjustments.GetAdjustForPatByType(Pd.PatNum,PrefC.GetLong(PrefName.BrokenAppointmentAdjustmentType)).Count;
						}
						row.Cells.Add(count.ToString());
						break;
					#endregion Broken Appts
					#region City
					case "City":
						row.Cells.Add(Pd.Patient.City);
						break;
					#endregion City
					#region Date First Visit
					case "Date First Visit":
						if(Pd.Patient.DateFirstVisit.Year<1880) {
							row.Cells.Add("??");
						}
						else if(Pd.Patient.DateFirstVisit==DateTime.Today) {
							row.Cells.Add(Lan.g("TableChartPtInfo","NEW PAT"));
						}
						else {
							row.Cells.Add(Pd.Patient.DateFirstVisit.ToShortDateString());
						}
						row.Tag=null;
						break;
					#endregion Date First Visit
					#region Ehr Provider Keys
					case "Ehr Provider Keys":
						//Not even available to most users.
						List<EhrProvKey> listEhrProvKeys=EhrProvKeys.RefreshForFam(Pd.Patient.Guarantor);
						string desc="";
						for(int i = 0;i<listEhrProvKeys.Count;i++) {
							if(i>0) {
								desc+="\r\n";
							}
							desc+=listEhrProvKeys[i].LName+", "+listEhrProvKeys[i].FName+", "
								+listEhrProvKeys[i].YearValue+", "+listEhrProvKeys[i].ProvKey;
						}
						row.Cells.Add(desc);
						row.ColorBackG=Color.PowderBlue;
						row.Tag="EhrProvKeys";
						break;
					#endregion Ehr Provider Keys
					#region Med Urgent
					case "Med Urgent":
						cell=new GridCell();
						cell.Text=Pd.Patient.MedUrgNote;
						cell.ColorText=Color.Red;
						cell.Bold=YN.Yes;
						row.Cells.Add(cell);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabMedical";
						break;
					#endregion Med Urgent
					#region Medical Summary
					case "Medical Summary":
						row.Cells.Add(Pd.PatientNote.Medical);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabMedical";
						break;
					#endregion Medical Summary
					#region Medications
					case "Medications":
						if(doRefreshData || Pd.ListMedicationPats==null) {
							Pd.ClearAndFill(EnumPdTable.MedicationPat);
						}
						List<MedicationPat> listMedicationPats=Pd.ListMedicationPats.FindAll(x=>x.DateStop.Year<1880 || x.DateStop.Date>=DateTime.Today);;
						row=new GridRow();
						cell=new GridCell();
						if(fieldCur.Description=="") {
							cell.Text=fieldCur.InternalName;
						}
						else {
							cell.Text=fieldCur.Description;
						}
						cell.Bold=YN.Yes;
						row.Cells.Add(cell);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabMedications";
						if(listMedicationPats.Count>0) {
							row.Cells.Add("");
							gridPtInfo.ListGridRows.Add(row);
						}
						else {
							row.Cells.Add(Lan.g("TableChartPtInfo","none"));
						}
						string text;
						Medication medication;
						for(int i=0;i<listMedicationPats.Count;i++) {
							row=new GridRow();
							if(listMedicationPats[i].MedicationNum==0) {//NewCrop medication order.
								row.Cells.Add(listMedicationPats[i].MedDescript);
							}
							else {
								medication=Medications.GetMedication(listMedicationPats[i].MedicationNum);
								text=medication.MedName;
								if(medication.MedicationNum != medication.GenericNum) {
									text+="("+Medications.GetMedication(medication.GenericNum).MedName+")";
								}
								row.Cells.Add(text);
							}
							text=listMedicationPats[i].PatNote;
							string noteMedGeneric="";
							if(listMedicationPats[i].MedicationNum!=0) {
								noteMedGeneric=Medications.GetGeneric(listMedicationPats[i].MedicationNum).Notes;
							}
							if(noteMedGeneric!="") {
								text+="("+noteMedGeneric+")";
							}
							row.Cells.Add(text);
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
							row.Tag="tabMedications";
							if(i!=listMedicationPats.Count-1) {
								gridPtInfo.ListGridRows.Add(row);
							}
						}
						break;
					#endregion Medications
					#region PatFields
					case "PatFields":
						if(doRefreshData){
							Pd.ClearAndFill(EnumPdTable.PatField);
						}
						PatFieldL.AddPatFieldsToGrid(gridPtInfo,Pd.ListPatFields,FieldLocations.Chart);
						break;
					#endregion PatFields
					#region Pat Restrictions
					case "Pat Restrictions":
						if(doRefreshData || Pd.ListPatRestrictions==null) {
							Pd.ClearAndFill(EnumPdTable.PatRestriction);
						}
						if(Pd.ListPatRestrictions.Count==0) {
							row.Cells.Add(Lan.g(this,"None"));//row added outside of switch statement
						}
						for(int i=0;i<Pd.ListPatRestrictions.Count;i++) {
							row=new GridRow();
							if(string.IsNullOrWhiteSpace(fieldCur.Description)) {
								row.Cells.Add(fieldCur.InternalName);
							}
							else {
								row.Cells.Add(fieldCur.Description);
							}
							row.Cells.Add(PatRestrictions.GetPatRestrictDesc(Pd.ListPatRestrictions[i].PatRestrictType));
							row.ColorBackG=listMiscColorShortDefs[(int)DefCatMiscColors.FamilyModPatRestrict].ItemColor;//Patient Restrictions (hard coded in convertdatabase4)
							if(i==Pd.ListPatRestrictions.Count-1) {//last row added outside of switch statement
								break;
							}
							gridPtInfo.ListGridRows.Add(row);
						}
						break;
					#endregion Pat Restrictions
					#region Patient Portal
					case "Patient Portal":
						row.Tag="Patient Portal";
						if(doRefreshData){
							Pd.ClearAndFill(EnumPdTable.UserWebHasPortalAccess);
						}
						if(Pd.UserWebHasPortalAccess) {
							row.Cells.Add(Lan.g(this,"Online"));
						}
						else {
							row.Cells.Add(Lan.g(this,"No access"));
						}
						break;
					#endregion Patient Portal
					#region Payor Types
					case "Payor Types":
						row.Tag="Payor Types";
						if(doRefreshData || Pd.ListPayorTypes==null) {
							Pd.ClearAndFill(EnumPdTable.PayorType);
						}
						List<PayorType> listPayorTypes=Pd.ListPayorTypes;
						string strPayorType="";
						if(listPayorTypes.Count>0){
							PayorType payorTypeRecent=listPayorTypes[listPayorTypes.Count-1];
							strPayorType=payorTypeRecent.SopCode+" - "+Sops.GetDescriptionFromCode(payorTypeRecent.SopCode);
						}
						row.Cells.Add(strPayorType);
						break;
					#endregion Payor Types
					#region Premedicate
					case "Premedicate":
						if(Pd.Patient.Premed) {
							row=new GridRow();
							row.Cells.Add("");
							cell=new GridCell();
							if(fieldCur.Description=="") {
								cell.Text=fieldCur.InternalName;
							}
							else {
								cell.Text=fieldCur.Description;
							}
							cell.ColorText=Color.Red;
							cell.Bold=YN.Yes;
							row.Cells.Add(cell);
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
							row.Tag="tabMedical";
							gridPtInfo.ListGridRows.Add(row);
						}
						break;
					#endregion Premedicate
					#region Pri Ins
					case "Pri Ins":
						string name;
						ordinal=PatPlans.GetOrdinal(PriSecMed.Primary,Pd.ListPatPlans,Pd.ListInsPlans,Pd.ListInsSubs);
						if(ordinal>0) {
							InsSub insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(Pd.ListPatPlans,ordinal),Pd.ListInsSubs);
							name=InsPlans.GetCarrierName(insSub.PlanNum,Pd.ListInsPlans);
							if(Pd.ListPatPlans[0].IsPending) {
								name+=Lan.g("TableChartPtInfo"," (pending)");
							}
							row.Cells.Add(name);
						}
						else {
							row.Cells.Add("");
						}
						row.Tag=null;
						break;
					#endregion Pri Ins
					#region Problems
					case "Problems":
						if(doRefreshData || Pd.ListDiseases==null) {
							Pd.ClearAndFill(EnumPdTable.Disease);
						}
						List<Disease> listDiseases=Pd.ListDiseases.FindAll(x=>x.ProbStatus==ProblemStatus.Active);
						row=new GridRow();
						cell=new GridCell();
						if(fieldCur.Description=="") {
							cell.Text=fieldCur.InternalName;
						}
						else {
							cell.Text=fieldCur.Description;
						}
						cell.Bold=YN.Yes;
						row.Cells.Add(cell);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabProblems";
						if(listDiseases.Count>0) {
							row.Cells.Add("");
							gridPtInfo.ListGridRows.Add(row);
						}
						else {
							row.Cells.Add(Lan.g("TableChartPtInfo","none"));
						}
						//Add a new row for each med.
						for(int i=0;i<listDiseases.Count;i++) {
							row=new GridRow(); 
							if(listDiseases[i].DiseaseDefNum!=0) {
								cell=new GridCell(DiseaseDefs.GetName(listDiseases[i].DiseaseDefNum));
								cell.ColorText=Color.Red;
								cell.Bold=YN.Yes;
								row.Cells.Add(cell);
								row.Cells.Add(listDiseases[i].PatNote);
							}
							else {
								row.Cells.Add("");
								cell=new GridCell(DiseaseDefs.GetItem(listDiseases[i].DiseaseDefNum)?.DiseaseName??Lan.g(this,"INVALID PROBLEM"));
								cell.ColorText=Color.Red;
								cell.Bold=YN.Yes;
								row.Cells.Add(cell);
								//row.Cells.Add(DiseaseList[i].PatNote);//no place to show a pat note
							}
							row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
							row.Tag="tabProblems";
							if(i!=listDiseases.Count-1) {
								gridPtInfo.ListGridRows.Add(row);
							}
						}
						break;
					#endregion Problems
					#region Prov. (Pri, Sec)
					case "Prov. (Pri, Sec)":
						string provText="";
						if(Pd.Patient.PriProv!=0) {
							provText+=Providers.GetAbbr(Pd.Patient.PriProv)+", ";							
						}
						else {
							provText+=Lan.g("TableChartPtInfo","None")+", ";
						}
						if(Pd.Patient.SecProv != 0) {
							provText+=Providers.GetAbbr(Pd.Patient.SecProv);
						}
						else {
							provText+=Lan.g("TableChartPtInfo","None");
						}
						row.Cells.Add(provText);
						row.Tag = null;
						break;
					#endregion Prov. (Pri, Sec)
					#region References
					case "References":
						List<CustRefEntry> listCustRefEntries=CustRefEntries.GetEntryListForCustomer(Pd.PatNum);
						if(listCustRefEntries.Count==0) {
							row.Cells.Add(Lan.g("TablePatient","None"));
							row.Tag="References";
							row.ColorBackG=listMiscColorShortDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
						}
						else {
							row.Cells.Add(Lan.g("TablePatient",""));
							row.Tag="References";
							row.ColorBackG=listMiscColorShortDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							gridPtInfo.ListGridRows.Add(row);
						}
						for(int i=0;i<listCustRefEntries.Count;i++) {
							row=new GridRow();
							row.Cells.Add(listCustRefEntries[i].DateEntry.ToShortDateString());
							row.Cells.Add(CustReferences.GetCustNameFL(listCustRefEntries[i].PatNumRef));
							row.Tag=listCustRefEntries[i];
							row.ColorBackG=listMiscColorShortDefs[(int)DefCatMiscColors.FamilyModuleReferral].ItemColor;
							if(i<listCustRefEntries.Count-1) {
								gridPtInfo.ListGridRows.Add(row);
							}
						}
						break;
					#endregion References
					#region Referred From
					case "Referred From":
						if(doRefreshData || Pd.ListRefAttaches==null) {
							Pd.ClearAndFill(EnumPdTable.RefAttach);
						}
						List<RefAttach> listRefAttaches=Pd.ListRefAttaches.DistinctBy(x => x.ReferralNum).ToList();
						string referral="";
						for(int i=0;i<listRefAttaches.Count;i++) {
							if(listRefAttaches[i].RefType==ReferralType.RefFrom) {
								referral=Referrals.GetNameLF(listRefAttaches[i].ReferralNum);
								break;
							}
						}
						if(referral=="") {
							referral="??";
						}
						row.Cells.Add(referral);
						row.Tag="Referral";
						break;
					#endregion Referred From
					#region Registration Keys
					case "Registration Keys":
						//Not even available to most users.
						RegistrationKey[] arrayKeys=RegistrationKeys.GetForPatient(Pd.PatNum);
						for(int i=0;i<arrayKeys.Length;i++) {
							//For non-guarantors with reseller keys, we do not want to show other family member reseller keys (there will be a lot of them).
							if(Pd.PatNum!=Pd.Patient.Guarantor
								&& arrayKeys[i].IsResellerCustomer 
								&& arrayKeys[i].PatNum!=Pd.PatNum) 
							{
								//The current patient selected is not the guarantor and this is a reseller key for another family member.  Do not show it in this patient's chart module.
								continue;
							}
							row=new GridRow();
							row.Cells.Add(Lan.g("TableChartPtInfo","Registration Key"));
							string str=arrayKeys[i].RegKey.Substring(0,4)+"-"+arrayKeys[i].RegKey.Substring(4,4)+"-"
								+arrayKeys[i].RegKey.Substring(8,4)+"-"+arrayKeys[i].RegKey.Substring(12,4);
							str+="  |  PatNum: "+arrayKeys[i].PatNum.ToString();//Always show the PatNum
							if(arrayKeys[i].IsForeign) {
								str+="\r\nForeign";
							}
							else {
								str+="\r\nUSA";
							}
							str+="\r\nStarted: "+arrayKeys[i].DateStarted.ToShortDateString();
							if(arrayKeys[i].DateDisabled.Year>1880) {
								str+="\r\nDisabled: "+arrayKeys[i].DateDisabled.ToShortDateString();
							}
							if(arrayKeys[i].DateEnded.Year>1880) {
								str+="\r\nEnded: "+arrayKeys[i].DateEnded.ToShortDateString();
							}
							if(arrayKeys[i].Note!="") {
								str+=arrayKeys[i].Note;
							}
							row.Cells.Add(str);
							row.Tag=arrayKeys[i].Copy();
							gridPtInfo.ListGridRows.Add(row);
						}
						break;
					#endregion Registration Keys
					#region Sec Ins
					case "Sec Ins":
						ordinal=PatPlans.GetOrdinal(PriSecMed.Secondary,Pd.ListPatPlans,Pd.ListInsPlans,Pd.ListInsSubs);
						if(ordinal>0) {
							InsSub insSub=InsSubs.GetSub(PatPlans.GetInsSubNum(Pd.ListPatPlans,ordinal),Pd.ListInsSubs);
							name=InsPlans.GetCarrierName(insSub.PlanNum,Pd.ListInsPlans);
							if(Pd.ListPatPlans[1].IsPending) {
								name+=Lan.g("TableChartPtInfo"," (pending)");
							}
							row.Cells.Add(name);
						}
						else {
							row.Cells.Add("");
						}
						row.Tag=null;
						break;
					#endregion Sec Ins
					#region Service Notes
					case "Service Notes":
						row.Cells.Add(Pd.PatientNote.Service);
						row.ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor;
						row.Tag="tabMedical";
						break;
					#endregion Service Notes
					#region Specialty
					case "Specialty":
						row.Cells.Add(Patients.GetPatientSpecialtyDef(Pd.PatNum)?.ItemName??"");
						row.Tag=null;
						break;
					#endregion Specialty
					#region Super Head
					case "Super Head":
						if(Pd.Patient.SuperFamily==0) {
							continue;//don't add empty row
						}
						row.Cells.Add(Pd.PatientSuperFamHead.GetNameLF()+" ("+Pd.PatientSuperFamHead.PatNum+")");
						break;
					#endregion Super Head
					#region Tobacco Use (Patient Smoking Status)
					case "Tobacco Use":
						if(!PrefC.GetBool(PrefName.ShowFeatureEhr)) {
							continue;
						}
						if(doRefreshData || Pd.ListEhrMeasureEvents==null) {
							Pd.ClearAndFill(EnumPdTable.EhrMeasureEvent);
							//LoadData.ListTobaccoStatuses=EhrMeasureEvents.RefreshByType(Pd.PatNum,EhrMeasureEventType.TobaccoUseAssessed);
						}
						List<EhrMeasureEvent> listTobaccoStatuses=Pd.ListEhrMeasureEvents
							.OrderByDescending(x => x.DateTEvent).Take(3).ToList();//only display the last three assessments at most
						row=new GridRow() { ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor,Tag="tabTobaccoUse" };
						row.Cells.Add(new GridCell(Text=fieldCur.Description==""?fieldCur.InternalName:fieldCur.Description) { Bold=YN.Yes });
						row.Cells.Add(listTobaccoStatuses.Count>0?"":Lan.g("TableChartPtInfo","none"));
						if(listTobaccoStatuses.Count>0) {
							gridPtInfo.ListGridRows.Add(row);
						}
						Snomed snmCur;
						for(int i=0;i<listTobaccoStatuses.Count;i++) {//show the last three tobacco use assessments at most
							EhrMeasureEvent ehrCur=listTobaccoStatuses[i];
							row=new GridRow() { ColorBackG=listMiscColorDefs[(int)DefCatMiscColors.ChartModuleMedical].ItemColor,Tag="tabTobaccoUse" };
							snmCur=Snomeds.GetByCode(ehrCur.CodeValueResult);
							row.Cells.Add(snmCur!=null?snmCur.Description:"");
							row.Cells.Add(ehrCur.DateTEvent.ToShortDateString()+(ehrCur.MoreInfo==""?"":(" - "+ehrCur.MoreInfo)));
							if(i==listTobaccoStatuses.Count-1) {
								break;//don't add last row here, handled outside of switch statement
							}
							gridPtInfo.ListGridRows.Add(row);
						}
						break;
						#endregion Tobacco Use (Patient Smoking Status)
				}
				if(new[] { "PatFields","Premedicate","Registration Keys" }.Contains(fieldCur.InternalName)) {
					//For fields that might have zero rows, we can't add the row here.  Adding rows is instead done in the case clause.
					//But some fields that are based on lists will always have one row, even if there are no items in the list.
					//Do not add those kinds here.
				}
				else {
					gridPtInfo.ListGridRows.Add(row);
				}
			}
			gridPtInfo.EndUpdate();
		}

		public void FunctionKeyPressContrChart(Keys keys) {
			List<ChartView> listChartViews=ChartViews.GetDeepCopy();
			switch(keys) {
				case Keys.F1: 
					if(gridChartViews.ListGridRows.Count>0) {
						gridChartViews.SetSelected(0,true);
						SetChartView(listChartViews[0]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>0) {
						gridCustomerViews.SetSelected(0,true);
					}
					break;
				case Keys.F2:
					if(gridChartViews.ListGridRows.Count>1) {
						gridChartViews.SetSelected(1,true);
						SetChartView(listChartViews[1]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>1) {
						gridCustomerViews.SetSelected(1,true);
					}
					break;
				case Keys.F3:
					if(gridChartViews.ListGridRows.Count>2) {
						gridChartViews.SetSelected(2,true);
						SetChartView(listChartViews[2]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>2) {
						gridCustomerViews.SetSelected(2,true);
					}
					break;
				case Keys.F4:
					if(gridChartViews.ListGridRows.Count>3) {
						gridChartViews.SetSelected(3,true);
						SetChartView(listChartViews[3]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>3) {
						gridCustomerViews.SetSelected(3,true);
					}
					break;
				case Keys.F5:
					if(gridChartViews.ListGridRows.Count>4) {
						gridChartViews.SetSelected(4,true);
						SetChartView(listChartViews[4]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>4) {
						gridCustomerViews.SetSelected(4,true);
					}
					break;
				case Keys.F6:
					if(gridChartViews.ListGridRows.Count>5) {
						gridChartViews.SetSelected(5,true);
						SetChartView(listChartViews[5]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>5) {
						gridCustomerViews.SetSelected(5,true);
					}
					break;
				case Keys.F7:
					if(gridChartViews.ListGridRows.Count>6) {
						gridChartViews.SetSelected(6,true);
						SetChartView(listChartViews[6]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>6) {
						gridCustomerViews.SetSelected(6,true);
					}
					break;
				case Keys.F8:
					if(gridChartViews.ListGridRows.Count>7) {
						gridChartViews.SetSelected(7,true);
						SetChartView(listChartViews[7]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>7) {
						gridCustomerViews.SetSelected(7,true);
					}
					break;
				case Keys.F9:
					if(gridChartViews.ListGridRows.Count>8) {
						gridChartViews.SetSelected(8,true);
						SetChartView(listChartViews[8]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>8) {
						gridCustomerViews.SetSelected(8,true);
					}
					break;
				case Keys.F10:
					if(gridChartViews.ListGridRows.Count>9) {
						gridChartViews.SetSelected(9,true);
						SetChartView(listChartViews[9]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>9) {
						gridCustomerViews.SetSelected(9,true);
					}
					break;
				case Keys.F11:
					if(gridChartViews.ListGridRows.Count>10) {
						gridChartViews.SetSelected(10,true);
						SetChartView(listChartViews[10]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>10) {
						gridCustomerViews.SetSelected(10,true);
					}
					break;
				case Keys.F12:
					if(gridChartViews.ListGridRows.Count>11) {
						gridChartViews.SetSelected(11,true);
						SetChartView(listChartViews[11]);
					}
					if(_isDistributorKey && gridCustomerViews.ListGridRows.Count>11) {
						gridCustomerViews.SetSelected(11,true);
					}
					break;
			}
		}

		///<summary>Called every time prefs are changed from any workstation.</summary>
		public void InitializeLocalData() {
			_isDistributorKey=PrefC.GetBool(PrefName.DistributorKey);
			if(!PrefC.GetBool(PrefName.OrthoShowInChart)){
				if(tabProc.Contains(tabOrtho)){
					tabProc.TabPages.Remove(tabOrtho);
				}
			}
			else{
				if(!tabProc.Contains(tabOrtho)){
					tabProc.TabPages.Add(tabOrtho);
				}
			}
			if(!_isDistributorKey) {
				butAddKey.Visible=false;
				butForeignKey.Visible=false;
				butPhoneNums.Visible=false;
				butErxAccess.Visible=false;
				tabProc.TabPages.Remove(tabCustomer);
			}
			if(!ToothChartRelay.IsSparks3DPresent) {
				//ComputerPref computerPref=ComputerPrefs.GetForLocalComputer();
				toothChartWrapper.UseHardware=ComputerPrefs.LocalComputer.GraphicsUseHardware;
				toothChartWrapper.PreferredPixelFormatNumber=ComputerPrefs.LocalComputer.PreferredPixelFormatNum;
				toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
				//Must be last preference set here, because this causes the pixel format to be recreated.																											
				toothChartWrapper.DrawMode=ComputerPrefs.LocalComputer.GraphicsSimple;
				//The preferred pixel format number changes to the selected pixel format number after a context is chosen.
				ComputerPrefs.LocalComputer.PreferredPixelFormatNum=toothChartWrapper.PreferredPixelFormatNumber;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
			}
			if(_isModuleSelected) {
				FillToothChart(retainSelection:true);
			}
			//if(PrefC.GetBoolSilent(PrefName.ChartQuickAddHideAmalgam,true)){ //Preference is Deprecated.
			//	panelQuickPasteAmalgam.Visible=false;
			//}
			//else{
			//	panelQuickPasteAmalgam.Visible=true;
			//}
			if(!ToolButItems.GetCacheIsNull()) {
				LayoutToolBar();
				if(!_isModuleSelected) {
					if(HasHideRxButtonsEcw()) {
						//Don't show the Rx and eRx buttons.
					}
					else {
						if(UsingEcwTightOrFull()) {
							if(!Environment.Is64BitOperatingSystem) {
								ToolBarMain.Buttons["Rx"].Enabled=false;
							}
							//eRx already disabled because it is never enabled for eCW Tight or Full
						}
						else {
							ToolBarMain.Buttons["Rx"].Enabled=false;
							ToolBarMain.Buttons["eRx"].Enabled=false;
						}
					}
					ToolBarMain.Buttons["LabCase"].Enabled=false;
					if(ToolBarMain.Buttons["Perio"]!=null) {
						ToolBarMain.Buttons["Perio"].Enabled=false;
					}
					if(ToolBarMain.Buttons["Ortho"]!=null) {
						ToolBarMain.Buttons["Ortho"].Enabled=false;
					}
					ToolBarMain.Buttons["Consent"].Enabled=false;
					if(ToolBarMain.Buttons["ToothChart"]!=null) {
						ToolBarMain.Buttons["ToothChart"].Enabled=false;
					}
					ToolBarMain.Buttons["ExamSheet"].Enabled=false;
					if(UsingEcwTight()) {
						ToolBarMain.Buttons["Commlog"].Enabled=false;
						webBrowserEcw.Url=null;
					}
					if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
						ToolBarMain.Buttons["EHR"].Enabled=false;
					}
					if(ToolBarMain.Buttons["HL7"]!=null) {
						ToolBarMain.Buttons["HL7"].Enabled=false;
					}
				}
			}
		}

		///<summary></summary>
		public void InitializeOnStartup() {
			if(_initializedOnStartup) {
				return;
			}
			_initializedOnStartup=true;
			string appDir=Application.StartupPath;
			if(File.Exists(Path.Combine(appDir,"Sparks3D\\Sparks3D.dll"))) {
				if(PrefC.GetBoolSilent(PrefName.DirectX11ToothChartUseIfAvail,true)){
					ToothChartRelay.IsSparks3DPresent=true;
				}
				if(ComputerPrefs.LocalComputer.GraphicsUseDirectX11==YN.No) {
					ToothChartRelay.IsSparks3DPresent=false;
				}
				if(ComputerPrefs.LocalComputer.GraphicsUseDirectX11==YN.Yes) {
					ToothChartRelay.IsSparks3DPresent=true;
				}
				//We'll just go ahead and let everyone use it, whether on support or not.
				//The code below might be useful if we decide to restrict usage a little more.
				//if(ODBuild.IsTrial()){
				//	ToothChartRelay.IsSparks3DPresent=true;
				//}
				//else if(OpenDentalHelp.ODHelp.IsEncryptedKeyValid()){//always true in debug
				//	ToothChartRelay.IsSparks3DPresent=true;
				//}
			}
			_toothChartRelay= new ToothChartRelay();//IsSparks3DPresent could have been set back to false here
			_toothChartRelay.SetToothChartWrapper(toothChartWrapper);
			if(ToothChartRelay.IsSparks3DPresent){
				try{
					toothChartWrapper.Visible=false;//already not visible
					toothChart=_toothChartRelay.GetToothChart();
					toothChart.Location=toothChartWrapper.Location;
					toothChart.Size=toothChartWrapper.Size;
					_toothChartRelay.SegmentDrawn+=toothChart_SegmentDrawn;
					_toothChartRelay.TextMoved+=_toothChart_TextMoved;
					_toothChartRelay.ToothSelectionsChanged += toothChart_ToothSelectionsChanged;
					LayoutManager.Add(toothChart,this);
					_toothChartRelay.ResetTeeth();
					_toothChartRelay.EndUpdate();//Initializes various pointers so that closing OD won't cause null memory error
				}
				catch{
					//EndUpdate can fail.
					ToothChartRelay.IsSparks3DPresent=false;
					if(toothChart.Visible){
						toothChart.Visible=false;//might need to be more aggressive and actually delete it
					}
				}
			}
			if(!ToothChartRelay.IsSparks3DPresent){
				toothChartWrapper.Visible=true;
				//ComputerPref localComputerPrefs=ComputerPrefs.GetForLocalComputer();
				this.toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
				this.toothChartWrapper.DrawMode=ComputerPrefs.LocalComputer.GraphicsSimple;//triggers ResetControls.
			}
			_procStatusNew=ProcStat.TP;
			if(IsTPChartingAvailable) {
				FillListPriorities();//Mimics old ChartLayoutHelper logic
			}
			ToggleCheckTreatPlans();//Mimics old ChartLayoutHelper logic
			//can't use Lan.F
			//Use the recursive overload of Lan.C to dynamically translate controls within tabs and other grouping controls.
			//Be careful not to add controls that will automatically get translated otherwise the translated text will show up within the Translation window.
			Lan.C(this,new Control[] {
				//textProcCode is handled in ClearButtons()
				tabEnterTx,
				tabMissing,
				tabMovements,
				tabPlanned,
				tabShow,
				tabDraw,
				gridChartViews,
				gridCustomerViews,
				gridPlanned,
				gridProg,
				gridPtInfo,
				gridTpProcs,
				gridTreatPlans,
			},true);
			Lan.C(this,menuProgRight,menuErx,menuToothChart);
			LayoutToolBar();
			//Passed-in controls will maintain their location and be shown but are not part of the dynamic layout fields.
			_sheetLayoutController=new SheetLayoutController(LayoutManager,this,ToolBarMain,tabControlImages,panelImages);
			LayoutControls();//First time loading.
			if(UsingEcwTightOrFull()) {
				tabProc.TabPages.Remove(tabPlanned);
			}
			Plugins.HookAddCode(this,"ContrChart.InitializeOnStartup_end",Pd.Patient);
			_gridProgMaxPageRowsDefault=gridProg.MaxPageRows;
		}

		///<summary>Causes the toolbars to be laid out again.</summary>
		public void LayoutToolBar() {
			ToolBarMain.Buttons.Clear();
			ODToolBarButton button;
			if(HasHideRxButtonsEcw()) {
				//Don't show the Rx and eRx buttons.
			}
			else {
				if(UsingEcwTightOrFull()) {
					if(!Environment.Is64BitOperatingSystem) {
						//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"New Rx"),1,"","Rx"));
						button=new ODToolBarButton(Lan.g(this,"New Rx"),1,"","Rx");
						button.Style=ODToolBarButtonStyle.DropDownButton;
						button.DropDownMenu=_contextMenuRxManage;
						ToolBarMain.Buttons.Add(button);
					}
					//don't add eRx
				}
				else {
					//ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"New Rx"),1,"","Rx"));
					button=new ODToolBarButton(Lan.g(this,"New Rx"),1,"","Rx");
					button.Style=ODToolBarButtonStyle.DropDownButton;
					button.DropDownMenu=_contextMenuRxManage;
					ToolBarMain.Buttons.Add(button);
					if(_butErx==null) {
						_butErx=new ODToolBarButton(Lan.g(this,"eRx"),1,"","eRx");
						_butErx.Style=ODToolBarButtonStyle.DropDownButton;
						_butErx.DropDownMenu=menuErx;
					}
					ToolBarMain.Buttons.Add(_butErx);
				}
			}
			ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"LabCase"),-1,"","LabCase"));
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Perio Chart"),2,"","Perio"));
			}
			if(PrefC.GetBool(PrefName.OrthoShowInChart)){
				button=new ODToolBarButton(OrthoChartTabs.GetFirst(true).TabName,-1,"","Ortho");
				if(OrthoChartTabs.GetCount(true)>1) {
					button.Style=ODToolBarButtonStyle.DropDownButton;
					button.DropDownMenu=menuOrthoChart;
				}
				ToolBarMain.Buttons.Add(button);
			}
			button=new ODToolBarButton(Lan.g(this,"Consent"),-1,"","Consent");
			if(SheetDefs.GetCustomForType(SheetTypeEnum.Consent).Count>0) {
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuConsent;
			}
			ToolBarMain.Buttons.Add(button);
			//if(PrefC.GetBool(PrefName.ToothChartMoveMenuToRight)) {
			//	ToolBarMain.Buttons.Add(new ODToolBarButton(".",-1,"",""));
			//}
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				button=new ODToolBarButton(Lan.g(this,"Tooth Chart"),-1,"","ToothChart");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				button.DropDownMenu=menuToothChart;
				ToolBarMain.Buttons.Add(button);
			}
			button=new ODToolBarButton(Lan.g(this,"Exam Sheet"),-1,"","ExamSheet");
			button.Style=ODToolBarButtonStyle.PushButton;
			ToolBarMain.Buttons.Add(button);
			if(UsingEcwTight()) {//button will show in this toolbar instead of the usual one.
				ToolBarMain.Buttons.Add(new ODToolBarButton(Lan.g(this,"Commlog"),4,Lan.g(this,"New Commlog Entry"),"Commlog"));
			}
			if(CultureInfo.CurrentCulture.Name=="en-US") {
				button=new ODToolBarButton(Lan.g(this,"CCD"),-1,"","CCD");
				button.Style=ODToolBarButtonStyle.PushButton;
				ToolBarMain.Buttons.Add(button);
			}
			if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
				ToolBarMain.Buttons.Add(new ODToolBarButton("EHR",-1,"","EHR"));
			}
			HL7Def hl7Def=HL7Defs.GetOneDeepEnabled();
			if(hl7Def!=null && !UsingEcwTightOrFull()) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(hl7Def.Description,-1,"","HL7"));
			}
			HL7Def hl7DefMedLab=HL7Defs.GetOneDeepEnabled(true);
			if(hl7DefMedLab!=null) {
				ToolBarMain.Buttons.Add(new ODToolBarButton(hl7DefMedLab.Description,-1,"","MedLab"));
			}
			if(_sheetLayoutController!=null && _sheetLayoutController.ListSheetDefsLayout.Count>0) {
				button=new ODToolBarButton("Layout",-1,"","Layout");
				button.Style=ODToolBarButtonStyle.DropDownButton;
				List<MenuItem> listMenuItems=new List<MenuItem>(new[] { new MenuItem(Lan.g(this,"Add/Edit Layouts"),LayoutMenuItem_Click),new MenuItem("-") });
				long selectedLayoutSheetDefNum=_sheetLayoutController.GetLayoutForUser().SheetDefNum;
				listMenuItems.AddRange(
					_sheetLayoutController.ListSheetDefsLayout.FindAll(x => x.SheetDefNum>0)//add all custom SheetDefs
						.Select(x => new MenuItem(x.Description,LayoutMenuItem_Click) { Tag=x,Checked=(selectedLayoutSheetDefNum==x.SheetDefNum) })
				);
				if(_sheetLayoutController.ListSheetDefsLayout.Any(x => x.SheetDefNum>0)) {//Menu has at least one custom layout def
					listMenuItems.Add(new MenuItem("-"));//add separator between custom and internal
				}
				SheetDef sheetDefInternalLayout=_sheetLayoutController.ListSheetDefsLayout.FirstOrDefault(x => x.SheetDefNum==0);
				if(sheetDefInternalLayout!=null) {//Not sure how this could be null, but we've had bug submissions for it.
					listMenuItems.Add(
						new MenuItem(sheetDefInternalLayout.Description,LayoutMenuItem_Click) { Tag=sheetDefInternalLayout,Checked=(selectedLayoutSheetDefNum==0) }
					);
				}
				button.DropDownMenu=new ContextMenu(listMenuItems.ToArray());
				ToolBarMain.Buttons.Add(button);
			}
			ProgramL.LoadToolbar(ToolBarMain,ToolBarsAvail.ChartModule);
			ToolBarMain.Invalidate();
			Plugins.HookAddCode(this,"ContrChart.LayoutToolBar_end",Pd.Patient);
		}

		/// <summary></summary>
		public void ModuleSelected(long patNum) {
			ModuleSelected(patNum,true,false);
		}

		/// <summary>Only use this overload when isClinicRefresh is set to true.  This is only used when calling ModuleSelected from FromOpenDental. When isClinicRefresh is true the tab control tabProc is redrawn and only needs to be done when the clinic is changed or the module is selected for the first time.</summary>
		public void ModuleSelected(long patNum,bool isClinicRefresh) {
			ModuleSelected(patNum,true,isClinicRefresh);
		}

		///<summary>Only use this overload when isFullRefresh is set to false.  This is ONLY in a few places and only for eCW at this point.  Speeds things up by refreshing less data.</summary>
		public void ModuleSelected(long patNum,bool isFullRefresh,bool isClinicRefresh) {
			_isModuleSelected=true;
			EasyHideClinicalData();
			UserOdPref userOdPrefShowAutoCommlog=UserOdPrefs.GetByUserAndFkeyType(Security.CurUser.UserNum,UserOdFkeyType.ShowAutomatedCommlog).FirstOrDefault();
			if(userOdPrefShowAutoCommlog==null) {
				checkShowCommAuto.Checked=true;
			}
			else {
				checkShowCommAuto.Checked=PIn.Bool(userOdPrefShowAutoCommlog.ValueString);
			}
			checkCommSuperFamily.Visible=PrefC.GetBool(PrefName.ShowFeatureSuperfamilies);
			Logger.LogAction("RefreshModuleData",LogPath.ChartModule,() => RefreshModuleData(patNum,isFullRefresh));
			if(!IsPatientNull() && Pd.Patient.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(Pd.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0,isFullRefresh);
			}
			if(PrefC.IsODHQ) {
				odInternalCustomerGrids.Visible=true;
				odInternalCustomerGrids.BringToFront();
				odInternalCustomerGrids.PatCur=Pd.Patient;
			}
			Logger.LogAction("RefreshModuleScreen",LogPath.ChartModule,() => RefreshModuleScreen(isClinicRefresh));//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();
			if(gridProg.VScrollVisible) {
				gridProg.ScrollToEnd();
			}
			Plugins.HookAddCode(this,"ContrChart.ModuleSelected_end",patNum);
		}

		///<summary>This function does not follow our usual pattern. This function is just like ModuleSelected() but also syncs any eRx data which needs to be checked frequently.  Only called from FormOpenDental when the Chart module button is clicked or a new patient is selected while inside the Chart.</summary>
		public void ModuleSelectedErx(long patNum) {
			ModuleSelected(patNum,true);
			RefreshDoseSpotNotifications();
		}

		///<summary></summary>
		public void ModuleUnselected(bool isLoggingOff=false) {
			//toothChart.Dispose();?
			UpdateTreatmentNote();
			if(!isLoggingOff) {
				PlannedApptPromptHelper();
			}
			_isModuleSelected=false;
			Pd.Clear();
			_listSubstitutionLinks=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			if(odInternalCustomerGrids.Visible) {
				odInternalCustomerGrids.TryHideGrids();
			}
			gridPtInfo.ContextMenu=new ContextMenu();//This module is never really disposed. Get rid of any menu options we added, to avoid duplicates.
			Plugins.HookAddCode(this,"ContrChart.ModuleUnselected_end");
		}

		public void RefreshModuleScreen(bool isClinicRefresh=false) {
			//ParentForm.Text=Patients.GetMainTitle(PatCur);
			LayoutToolBar();
			if(IsPatientNull()) {
				//groupShow.Enabled=false;
				gridPtInfo.Enabled=false;
				//tabPlanned.Enabled=false;
				_toothChartRelay.Enabled=false;
				gridProg.Enabled=false;
				if(HasHideRxButtonsEcw()) {
					//Don't show the Rx and eRx buttons.
				}
				else {
					//if(UsingEcwTight()) {
					if(UsingEcwTightOrFull()) {
						if(!Environment.Is64BitOperatingSystem) {
							ToolBarMain.Buttons["Rx"].Enabled=false;
						}
						//eRx already disabled because it is never enabled for eCW Tight or Full
					}
					else {
						ToolBarMain.Buttons["Rx"].Enabled=false;
						ToolBarMain.Buttons["eRx"].Enabled=false;
					}
				}
				ToolBarMain.Buttons["LabCase"].Enabled=false;
				if(ToolBarMain.Buttons["Perio"]!=null) {
					ToolBarMain.Buttons["Perio"].Enabled=false;
				}
				if(ToolBarMain.Buttons["Ortho"]!=null) {
					ToolBarMain.Buttons["Ortho"].Enabled=false;
				}
				ToolBarMain.Buttons["Consent"].Enabled=false;
				if(ToolBarMain.Buttons["ToothChart"]!=null) {
					ToolBarMain.Buttons["ToothChart"].Enabled=false;
				}
				ToolBarMain.Buttons["ExamSheet"].Enabled=false;
				if(UsingEcwTight()) {
					ToolBarMain.Buttons["Commlog"].Enabled=false;
					webBrowserEcw.Url=null;
				}
				if(ToolBarMain.Buttons["CCD"]!=null) {
					ToolBarMain.Buttons["CCD"].Enabled=false;
				}
				//if(FormOpenDental.ObjSomeEhrSuperClass!=null) {//didn't work
				if(ToolBarMain.Buttons["EHR"]!=null) {
					ToolBarMain.Buttons["EHR"].Enabled=false;
				}
				if(ToolBarMain.Buttons["HL7"]!=null) {
					ToolBarMain.Buttons["HL7"].Enabled=false;
				}
				tabProc.Enabled=false;
				butAddKey.Enabled=false;
				butForeignKey.Enabled=false;
				butPhoneNums.Enabled=false;
				butErxAccess.Enabled=false;
				trackToothProcDates.Enabled=false;
				textToothProcDate.Enabled=false;
				textSearch.Text="";
			}
			else {
				trackToothProcDates.Enabled=true;
				textToothProcDate.Enabled=true;
				//groupShow.Enabled=true;
				gridPtInfo.Enabled=true;
				//groupPlanned.Enabled=true;
				_toothChartRelay.Enabled=true;
				gridProg.Enabled=true;
				if(HasHideRxButtonsEcw()) {
					//Don't show the Rx and eRx buttons.
				}
				else {
					//if(UsingEcwTight()) {
					if(UsingEcwTightOrFull()) {
						if(!Environment.Is64BitOperatingSystem) {
							ToolBarMain.Buttons["Rx"].Enabled=true;
						}
						//don't enable eRx
					}
					else {
						ToolBarMain.Buttons["Rx"].Enabled=true;
						ToolBarMain.Buttons["eRx"].Enabled=true;
					}
				}
				ToolBarMain.Buttons["LabCase"].Enabled=true;
				if(ToolBarMain.Buttons["Perio"]!=null) {
					ToolBarMain.Buttons["Perio"].Enabled=true;
				}
				if(ToolBarMain.Buttons["Ortho"]!=null) {
					ToolBarMain.Buttons["Ortho"].Enabled=true;
				}
				ToolBarMain.Buttons["Consent"].Enabled=true;
				if(ToolBarMain.Buttons["ToothChart"]!=null) {
					ToolBarMain.Buttons["ToothChart"].Enabled=true;
				}
				ToolBarMain.Buttons["ExamSheet"].Enabled=true;
				if(UsingEcwTightOrFull()) {
					if(UsingEcwTight()) {
						ToolBarMain.Buttons["Commlog"].Enabled=true;
					}
					//the following sequence also gets repeated after exiting the Rx window to refresh.
					String strAppServer="";
					try {
						if(Bridges.ECW.UserId==0 || String.IsNullOrEmpty(Bridges.ECW.EcwConfigPath)) {
							webBrowserEcw.Url=null;
							labelECWerror.Text="This panel does not display unless\r\nOpen Dental is launched from inside eCW";
							labelECWerror.Visible=true;
						}
						else {
							//this property will not exist if using Oracle, eCW will never use Oracle
							string uriString=ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"MedicalPanelUrl");
							string path="";
							if(uriString=="") {
								XmlTextReader xmlTextReader=new XmlTextReader(Bridges.ECW.EcwConfigPath);
								while(xmlTextReader.Read()) {
									if(xmlTextReader.Name.ToString()=="server") {
										strAppServer=xmlTextReader.ReadString().Trim();
									}
								}
								path="http://"+strAppServer+"/mobiledoc/jsp/dashboard/Overview.jsp"
									+"?ptId="+Pd.PatNum.ToString()+"&panelName=overview&pnencid="
									+Bridges.ECW.AptNum.ToString()+"&context=progressnotes&TrUserId="+Bridges.ECW.UserId.ToString();
								//set cookie
								if(!String.IsNullOrEmpty(Bridges.ECW.JSessionId)) {
									InternetSetCookie("http://"+strAppServer,null,"JSESSIONID = "+Bridges.ECW.JSessionId);
								}
								if(!String.IsNullOrEmpty(Bridges.ECW.JSessionIdSSO)) {
									InternetSetCookie("http://"+strAppServer,null,"JSESSIONIDSSO = "+Bridges.ECW.JSessionIdSSO);
								}
								if(!String.IsNullOrEmpty(Bridges.ECW.LBSessionId)) {
									if(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"IsLBSessionIdExcluded")=="0") {
										InternetSetCookie("http://"+strAppServer,null,"LBSESSIONID = "+Bridges.ECW.LBSessionId);
									}
									else {
										InternetSetCookie("http://"+strAppServer,null,Bridges.ECW.LBSessionId);
									}
								}
							}
							else {
								path=uriString
									+"?ptId="+Pd.PatNum.ToString()+"&panelName=overview&pnencid="
									+Bridges.ECW.AptNum.ToString()+"&context=progressnotes&TrUserId="+Bridges.ECW.UserId.ToString();
								//parse out with regex: uristring
								Match match=Regex.Match(uriString,@"\b([^:]+://[^/]+)/");//http://servername
								if(!match.Success || match.Groups.Count<2) {//if no match, or match but no group 1 to grab
									throw new Exception("Invalid URL saved in the Medical Panel URL field of the eClinicalWorks program link.");
								}
								string cookieUrl=match.Groups[1].Value;
								//set cookie
								if(!String.IsNullOrEmpty(Bridges.ECW.JSessionId)) {
									InternetSetCookie(cookieUrl,null,"JSESSIONID = "+Bridges.ECW.JSessionId);
								}
								if(!String.IsNullOrEmpty(Bridges.ECW.JSessionIdSSO)) {
									InternetSetCookie(cookieUrl,null,"JSESSIONIDSSO = "+Bridges.ECW.JSessionIdSSO);
								}
								if(!String.IsNullOrEmpty(Bridges.ECW.LBSessionId)) {
									if(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"IsLBSessionIdExcluded")=="0") {
										InternetSetCookie(cookieUrl,null,"LBSESSIONID = "+Bridges.ECW.LBSessionId);
									}
									else {
										InternetSetCookie(cookieUrl,null,Bridges.ECW.LBSessionId);
									}
								}
							}
							//navigate
							webBrowserEcw.Navigate(path); 
							labelECWerror.Visible=false;
						}
					}
					catch(Exception ex) {
						webBrowserEcw.Url=null;
						labelECWerror.Text="Error: "+ex.Message;
						labelECWerror.Visible=true;
					}
				}
				if(ToolBarMain.Buttons["CCD"]!=null) {
					ToolBarMain.Buttons["CCD"].Enabled=true;
				}
				if(PrefC.GetBool(PrefName.ShowFeatureEhr)) { //didn't work either
				//if(ToolBarMain.Buttons["EHR"]!=null) {
					ToolBarMain.Buttons["EHR"].Enabled=true;
				}
				if(ToolBarMain.Buttons["HL7"]!=null) {
					ToolBarMain.Buttons["HL7"].Enabled=true;
				}
				tabProc.Enabled=true;
				butAddKey.Enabled=true;
				butForeignKey.Enabled=true;
				butPhoneNums.Enabled=true;
				butErxAccess.Enabled=true;
				if(_prevPatNum!=Pd.PatNum) {//reset to TP status on every new patient selected
					if(PrefC.GetBool(PrefName.AutoResetTPEntryStatus)) {
						radioEntryTP.Select();
					}
					textSearch.Text="";
					_prevPatNum=Pd.PatNum;
				}
				if(PrefC.GetBool(PrefName.PatientDOBMasked)) {
					//Add "View DOB" right click option, MenuItemPopupUnmaskDOB will show and hide it as needed.
					if(gridPtInfo.ContextMenu==null) {
						gridPtInfo.ContextMenu=new ContextMenu();//ODGrid will automatically attach the defaut Popups
					}
					ContextMenu contextMenu=gridPtInfo.ContextMenu;
					MenuItem menuItemUnmaskDOB=new MenuItem();
					menuItemUnmaskDOB.Enabled=false;
					menuItemUnmaskDOB.Visible=false;
					menuItemUnmaskDOB.Name="ViewDOB";
					menuItemUnmaskDOB.Text="View DOB";
					menuItemUnmaskDOB.Click+=new System.EventHandler(this.MenuItemUnmaskDOB_Click);
					contextMenu.MenuItems.Add(menuItemUnmaskDOB);
					contextMenu.Popup+=MenuItemPopupUnmaskDOB;
				}
			}
			if(!UsingEcwTightOrFull() && isClinicRefresh) {
				if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
					tabProc.TabPages.Remove(tabMissing);
					tabProc.TabPages.Remove(tabMovements);
					if(tabProc.SelectedTab==tabMissing || tabProc.SelectedTab==tabMovements) {
						tabProc.SelectedTab=tabEnterTx;
					}
					_selectedProcTab=tabProc.SelectedIndex;
					textSurf.Visible=false;
					butBF.Visible=false;
					butOI.Visible=false;
					butV.Visible=false;
					butM.Visible=false;
					butD.Visible=false;
					butL.Visible=false;
					checkShowTeeth.Visible=false;
				}
				else {
					TabPage tabPageSelected=tabProc.SelectedTab;
					/*This looks dumb, unless there's a good note as to why
					tabProc.TabPages.Remove(tabMissing);
					tabProc.TabPages.Remove(tabMovements);
					tabProc.TabPages.Remove(tabPlanned);
					tabProc.TabPages.Remove(tabShow);
					tabProc.TabPages.Remove(tabDraw);
					tabProc.TabPages.Add(tabMissing);
					tabProc.TabPages.Add(tabMovements);
					tabProc.TabPages.Add(tabPlanned);
					tabProc.TabPages.Add(tabShow);
					tabProc.TabPages.Add(tabDraw);*/
					tabProc.SelectedTab=tabPageSelected;
					_selectedProcTab=tabProc.SelectedIndex;
					textSurf.Visible=true;
					butBF.Visible=true;
					butOI.Visible=true;
					butV.Visible=true;
					butM.Visible=true;
					butD.Visible=true;
					butL.Visible=true;
					checkShowTeeth.Visible=true;
				}
			}
			ToolBarMain.Invalidate();
			ClearButtons();
			FillMovementsAndHidden();
			Logger.LogAction("FillChartViewsGrid",LogPath.ChartModule,() => FillChartViewsGrid(false));
			if(_isDistributorKey) {
				FillCustomerTab();
			}
			if(textSearch.Text!="") {
				_listSearchResults?.Clear();
				_listSearchResults=null;
				SearchProgNotes();
			}
			else {
				Logger.LogAction("FillProgNotes",LogPath.ChartModule,() => FillProgNotes(doRefreshData: false));
			}
			Logger.LogAction("FillPlanned",LogPath.ChartModule,() => FillPlanned());
			Logger.LogAction("FillPtInfo",LogPath.ChartModule,() => FillPtInfo(false));
			Logger.LogAction("FillDxProcImage",LogPath.ChartModule,() => FillDxProcImage(false));
			Logger.LogAction("FillImages",LogPath.ChartModule,() => FillImages());
			if(checkShowOrtho.Checked){
				Logger.LogAction("FillOrtho",LogPath.ChartModule,() => FillGridOrtho());
			}
		}

		public void UpdateTreatmentNote() {
			//revisit and test this. Since Pd will not necessarily be cleared when ModuleUnselected,
			//this test is probably not valid. This gets called from
			if(!_isModuleSelected) {
				return;
			}
			if(TreatmentNoteChanged) {
				Pd.PatientNote.Treatment=textTreatmentNotes.Text;
				PatientNotes.Update(Pd.PatientNote,Pd.Patient.Guarantor);
				TreatmentNoteChanged=false;
			}
		}
		#endregion Methods - Public

		#region Methods - Private - General
		///<summary>Inserts TreatPlanAttaches that allows the procedure to be charted to one or more treatment plans at the same time.</summary>
		private void AttachProcToTPs(Procedure proc) {
			if(proc.ProcStatus!=ProcStat.TP && proc.ProcStatus!=ProcStat.TPi) {
				return;
			}
			List<long> listTpNums=new List<long>();
			if(gridTreatPlans.GetSelectedIndex()>=0) {
				listTpNums=gridTreatPlans.SelectedIndices.Select(x => _listTreatPlans[x].TreatPlanNum).ToList();
			}
			_listTreatPlans=TreatPlans.GetAllCurrentForPat(Pd.PatNum);
			//If there is no active TP, make sure to add an active treatment plan.
			if(_listTreatPlans.All(x => x.TPStatus!=TreatPlanStatus.Active)) {
				TreatPlan activeTreatPlan=new TreatPlan() {
					Heading=Lans.g("TreatPlans","Active Treatment Plan"),
					Note=PrefC.GetString(PrefName.TreatmentPlanNote),
					TPStatus=TreatPlanStatus.Active,
					PatNum=Pd.PatNum,
					TPType=DiscountPlanSubs.HasDiscountPlan(Pd.PatNum) ? TreatPlanType.Discount : TreatPlanType.Insurance
				};
				activeTreatPlan.TreatPlanNum=TreatPlans.Insert(activeTreatPlan);
				_listTreatPlans=new List<TreatPlan>() { activeTreatPlan };
				listTpNums.Add(activeTreatPlan.TreatPlanNum);
			}
			else if(listTpNums.Count==0) {//NOT treat plan charting so no TP selected, active plan exists so get TPNum from active plan
				listTpNums.Add(_listTreatPlans.FirstOrDefault(x => x.TPStatus==TreatPlanStatus.Active).TreatPlanNum);
			}
			long priorityNum=0;
			if(comboPriority.SelectedIndex>0) {
				priorityNum=comboPriority.GetSelectedDefNum();
			}
			listTpNums.ForEach(x => TreatPlanAttaches.Insert(new TreatPlanAttach() { TreatPlanNum=x,ProcNum=proc.ProcNum,Priority=priorityNum }));
			//if all treatplans selected are not the active treatplan, then chart proc as status TPi
			//we know there is an active plan for the patient at this point
			if(_listTreatPlans.FindAll(x => listTpNums.Contains(x.TreatPlanNum)).All(x => x.TPStatus!=TreatPlanStatus.Active)) {
				Procedure procOld=proc.Copy();
				proc.ProcStatus=ProcStat.TPi;//change proc status to TPi if all selected plans are Inactive status
				Procedures.Update(proc,procOld);
			}
		}

		///<summary>Updates estimates for given parent procedure.</summary
		private void CanadianLabFeeHelper(long procNumParent) {
			if(procNumParent==0){
				return;//Should not happen.
			}
			if(_listPatProcs==null) {
				_listPatProcs=Procedures.Refresh(Pd.PatNum);
			}
			Procedure procParent=Procedures.GetProcFromList(_listPatProcs,procNumParent);
			if(procParent==null) {//A null parent proc could happen in rare cases for older databases.
				return;
			}
			if(procParent.ProcNum==0) {//Should never happen.
				return;
			}
			if(Procedures.IsAttachedToClaim(procParent.ProcNum)) {//If attached to a claim, then user should recreate claim because estimates will be inaccurate not matter what.
				return;
			}
			Procedures.ComputeEstimates(procParent,Pd.PatNum,ClaimProcs.RefreshForProc(procParent.ProcNum),false,Pd.ListInsPlans,Pd.ListPatPlans,Pd.ListBenefits,Pd.Patient.Age,Pd.ListInsSubs);
		}

		private bool CanAddGroupNote(bool doCheckDb,bool isSilent,List<Procedure> listProcs) {
			DataRow row;
			if(gridProg.SelectedIndices.Length==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Please select procedures to attach a group note to."); 
				}
				return false;
			}
			for(int i=0;i<gridProg.SelectedIndices.Length;i++) {
				row=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[i]].Tag;
				if(!CanGroupRow(row,doCheckDb:doCheckDb,isSilent:isSilent,listProcs)) {
					return false;
				}
			}
			//Validate the list of procedures------------------------------------------------------------------------------------
			DateTime procDate=listProcs[0].ProcDate;
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				clinicNum=listProcs[0].ClinicNum;
			}
			long provNum=listProcs[0].ProvNum;
			for(int i=0;i<listProcs.Count;i++) {//starts at 0 to check procStatus
				if(listProcs[i].ProcDate!=procDate) {
					if(!isSilent) {
						MsgBox.Show(this,"Procedures must have the same date to attach a group note."); 
					}
					return false;
				}
				if(PrefC.HasClinicsEnabled && listProcs[i].ClinicNum!=clinicNum) {
					if(!isSilent) {
						MsgBox.Show(this,"Procedures must have the same clinic to attach a group note."); 
					}
					return false;
				}
				if(listProcs[i].ProvNum!=provNum) {
					if(!isSilent) {
						MsgBox.Show(this,"Procedures must have the same provider to attach a group note."); 
					}
					return false;
				}
			}
			return true;
		}

		///<summary>Returns true if the 'Attach Lab Fee' menu item is applicable. Adds selected regular procedures to procNumsReg. Adds selected lab procedures to procNumsLab.</summary>
		private bool CanAttachLabFee(bool isSilent,List<long> procNumsReg,List<long> procNumsLab) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return false;
			}
			if(gridProg.SelectedIndices.Length<2 || gridProg.SelectedIndices.Length>3) {
				if(!isSilent) {
					MsgBox.Show(this,"Please select two or three procedures, one regular and the other one or two lab.");
				}
				return false;
			}
			//One check that is not made is whether a lab proc is already attached to a different proc.
			DataRow row1=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[0]].Tag;
			DataRow row2=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[1]].Tag;
			DataRow row3=null;
			if(gridProg.SelectedIndices.Length==3) {
				row3=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[2]].Tag;
			}
			if(row1["ProcNum"].ToString()=="0" || row2["ProcNum"].ToString()=="0" || (row3!=null && row3["ProcNum"].ToString()=="0")) {
				if(!isSilent) {
					MsgBox.Show(this,"All selected items must be procedures.");
				}
				return false;
			}
			if(ProcedureCodes.GetProcCode(row1["ProcCode"].ToString()).IsCanadianLab) {
				procNumsLab.Add(PIn.Long(row1["ProcNum"].ToString()));
			}
			else {
				procNumsReg.Add(PIn.Long(row1["ProcNum"].ToString()));
			}
			if(ProcedureCodes.GetProcCode(row2["ProcCode"].ToString()).IsCanadianLab) {
				procNumsLab.Add(PIn.Long(row2["ProcNum"].ToString()));
			}
			else {
				procNumsReg.Add(PIn.Long(row2["ProcNum"].ToString()));
			}
			if(row3!=null) {
				if(ProcedureCodes.GetProcCode(row3["ProcCode"].ToString()).IsCanadianLab) {
					procNumsLab.Add(PIn.Long(row3["ProcNum"].ToString()));
				}
				else {
					procNumsReg.Add(PIn.Long(row3["ProcNum"].ToString()));
				}
			}
			if(procNumsReg.Count==0) {
				if(!isSilent) {
					MsgBox.Show(this,"One of the selected procedures must be a regular non-lab procedure as defined in Procedure Codes.");
				}
				return false;
			}
			if(procNumsReg.Count>1) {
				if(!isSilent) {
					MsgBox.Show(this,"Only one of the selected procedures may be a regular non-lab procedure as defined in Procedure Codes.");
				}
				return false;
			}
			return true;
		}

		///<summary>Checks if the procedure can be changed to newProcStatus.  If doCheckDb, it will get fresh data from the database when checking, otherwise, it will use data from LoadData.  If isSilent, a message box will popup if the status cannot be changed</summary>
		private bool CanChangeProcsStatus(ProcStat newProcStatus,DataRow row,bool doCheckDb,bool isSilent) {
			if(!IsAuditMode(isSilent)) {
				return false;
			}
			if(newProcStatus==ProcStat.C && !PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
				if(!isSilent) {
					MsgBox.Show(this,"Only single appointments and tasks may be set complete.  If you want to be able to set procedures complete, you must turn "
						+"on that option in Setup | Chart | Chart Preferences.");
				}
				return false;
			}
			//check to make sure we don't have non-procedures
			if(row["ProcNum"].ToString()=="0" || row["ProcCode"].ToString()=="~GRP~") {
				if(!isSilent) {
					MsgBox.Show(this,"Only procedures, single appointments, or single tasks may be set complete.");
				}
				return false;
			}
			//Check for procedures in hidden categories.
			List<string> listHiddenProcCodes=ProcedureCodes.GetProcCodesInHiddenCats(PIn.Long(row["CodeNum"].ToString()));
			if(!isSilent && newProcStatus==ProcStat.C && listHiddenProcCodes.Count > 0) {
				MsgBox.Show(Lan.g(this,"Procedure is in a hidden category:")+" "+listHiddenProcCodes[0]);
				return false;
			}
			if(doCheckDb){
				Pd.Clear(EnumPdTable.ClaimProc,EnumPdTable.PaySplit,EnumPdTable.Adjustment,EnumPdTable.Procedure);
			}
			Pd.FillIfNeeded(EnumPdTable.ClaimProc,EnumPdTable.PaySplit,EnumPdTable.Adjustment,EnumPdTable.Procedure);
			long procNum=PIn.Long(row["ProcNum"].ToString());
			Procedure procOld=Pd.ListProcedures.FirstOrDefault(x => x.ProcNum==procNum);
			OrthoProcLink orthoProcLink=OrthoProcLinks.GetByProcNum(procNum);
			if(procOld.IsLocked) {
				if(!isSilent) {
					MsgBox.Show(this,"Locked procedures cannot be edited.");
				}
				return false;
			}
			#region Validation
			if(procOld.ProcStatus==newProcStatus) {
				if(!isSilent) {
					MsgBox.Show(this,"Procedure's status already is "+newProcStatus);
				}
				return false;
			}
			if(ProcedureCodes.GetWhere(x => x.CodeNum==procOld.CodeNum).Count==0) {
				if(!isSilent) {
					MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
				}
				return false;
			}
			Procedure procedure=procOld.Copy();
			procedure.ProcStatus=newProcStatus;
			DateTime procDate=PIn.Date(textDate.Text);//Mimics how procCur.ProcDate would be changed after validation below.
			//Appt is either set to a claim from the DB, claim from cached list in LoadData.ArrAppts or new Appointment() object.
			//The query that fills LoadData.ArrAppts excludes appts with AptDateTime==MinVal.
			//When set to a new Appointment object this means the user clicked an appt with AptDateTime==MinVal (i.e. unscheduled).
			//Currently all 'appt' and 'procdate' references below are OK with using a new Appointment() obj.
			Appointment appointment=null;
			if(procedure.AptNum!=0) {//if attached to an appointment
				if(doCheckDb){//doCheckDb is only false when creating right-click menu.  MenuItem click will use doCheckDb=true.
					Pd.Clear(EnumPdTable.Appointment);
				}
				Pd.FillIfNeeded(EnumPdTable.Appointment);
				appointment=Pd.ListAppointments.Find(x => x.AptNum==procedure.AptNum);
				if(appointment is null){
					appointment=new Appointment();
				}
				DateTime dateNow=doCheckDb ? MiscData.GetNowDateTime() : DateTime.Now;
				if(appointment.AptDateTime.Date > dateNow.Date) {
					if(!isSilent) {
						MessageBox.Show(Lan.g(this,"Not allowed because a procedure is attached to a future appointment with a date of ")
							+appointment.AptDateTime.ToShortDateString());
					}
					return false;
				}
				procDate=appointment.AptDateTime;
			}
			if(procDate.Date.Year<1880) {//If not textDate entered or if appt.AptDateTime is invalid.
				procDate=doCheckDb ? MiscData.GetNowDateTime() : DateTime.Now;
			}
			if(procOld.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				if(doCheckDb && !ProcedureL.CheckPermissionsAndGlobalLockDate(procOld,procedure,procDate)) {
					return false;
				}
			}
			//At this point we do not need to check if a procedure status is changing. FormProcEdit.EntriesAreValid() does however.
			if(procOld.ProcStatus==ProcStat.C) {
				#region Changing from completed to something else.
				List<Adjustment> listAdjustments=Pd.ListAdjustments.FindAll(x=>x.ProcNum==procedure.ProcNum);
				if(listAdjustments.Count>0 && !isSilent
					&& !MsgBox.Show(this,MsgBoxButtons.YesNo,"This procedure has adjustments attached to it. Changing the status from completed will delete any "
						+"adjustments for the procedure. Continue?")) 
				{
					return false;
				}
				List<PaySplit> listPaySplits=Pd.ListPaySplits.FindAll(x=>x.ProcNum==procedure.ProcNum);
				double sumPaySplits=listPaySplits.Sum(x => x.SplitAmt);
				if(sumPaySplits!=0) {
					if(!isSilent) {
						MsgBox.Show(this,"Not allowed to modify the status of a procedure that has payments attached to it. Detach payments from the procedure first.");
					}
					return false;
				}
				//Cannot set EC or EO on completed procs on a claim.
				if(ProcedureL.IsProcCompleteAttachedToClaim(procOld,Pd.ListClaimProcs,isSilent)) {
					return false;
				}
				if(orthoProcLink!=null) {
					if(!isSilent) {
						MsgBox.Show(this,"Cannot change the status of completed procedures that are linked to an ortho cases. Detach the procedure from the ortho case first.");
					}
					return false;
				}
				#endregion
			}
			if(procedure.ProcStatus==ProcStat.C) {//User is trying to change status to complete.
				#region Setting proc to complete
				if(ProcedureL.IsProcCompleteAttachedToClaim(procedure,Pd.ListClaimProcs,isSilent)) {
					return false;
				}
				if(appointment!=null) {
					if(doCheckDb && !Security.IsAuthorized(Permissions.ProcComplCreate,appointment.AptDateTime,procedure.CodeNum,procedure.ProcFee)) {
						return false;
					}
				}
				else if(doCheckDb && !Security.IsAuthorized(Permissions.ProcComplCreate,procDate,procedure.CodeNum,procedure.ProcFee)) {
					return false;
				}
				if(procDate.Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
					MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
					return false;
				}
				#endregion
			}
			#endregion
			return true;
		}

		///<summary>Returns true if the row is an appointment and can be set complete.</summary>
		private bool CanCompleteAppointment(bool doCheckDb,bool isSilent) {
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(!CanDisplayAppointment() || listSelectedRows.Count!=1) {
				if(!isSilent) {
					MsgBox.Show(this,"Only procedures, single appointments, or single tasks may be set complete.");
				}
				//Row selected is not an appoitment or the rows selected != 1
				return false;
			}
			//Only one appointment row selected at this point.
			DataRow rowApt=listSelectedRows.First();
			if(doCheckDb && !Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return false;
			}
			long aptNum=PIn.Long(rowApt["AptNum"].ToString());
			if(doCheckDb){
				Pd.Clear(EnumPdTable.Appointment);
			}
			Pd.FillIfNeeded(EnumPdTable.Appointment);
			Appointment appointment=Pd.ListAppointments.Find(x => x.AptNum==aptNum);
			if(appointment==null) {
				if(!isSilent) {
					MsgBox.Show(this,"Appointment does not exist.");
				}
				return false;
			}
			if(appointment.AptStatus==ApptStatus.Complete) {
				if(!isSilent) {
					MsgBox.Show(this,"Already complete.");
				}
				return false;
			}
			if(appointment.AptStatus==ApptStatus.PtNote
				|| appointment.AptStatus==ApptStatus.PtNoteCompleted
				|| appointment.AptStatus==ApptStatus.Planned
				|| appointment.AptStatus==ApptStatus.UnschedList) 
			{
				if(!isSilent) {
					MsgBox.Show(this,"Not allowed for that status.");
				}
				return false;
			}
			if(doCheckDb) { 
				List<Procedure> listProcs=Procedures.GetProcsMultApts(new List<long> { appointment.AptNum });
				List<string> listHiddenProcCodes=ProcedureCodes.GetProcCodesInHiddenCats(listProcs.Select(x => x.CodeNum).ToArray());
				if(listHiddenProcCodes.Count > 0) {
					MsgBox.Show(Lan.g(this,"Cannot complete appointment because the following procedures are in a hidden category:")+" "+string.Join(", ",listHiddenProcCodes));
					return false;
				}
				if(ProcedureCodes.DoAnyBypassLockDate()) {
					foreach(Procedure proc in listProcs) {
						if(!Security.IsAuthorized(Permissions.ProcComplCreate,appointment.AptDateTime,proc.CodeNum,proc.ProcFee)) {
							return false;
						}
					}
				}
				else if(!Security.IsAuthorized(Permissions.ProcComplCreate,appointment.AptDateTime)) {
					return false;
				}
			}
			if(appointment.AptDateTime.Date>DateTime.Today.Date) {
				if(!PrefC.GetBool(PrefName.ApptAllowFutureComplete)) {
					if(!isSilent) {
						MsgBox.Show(this,"Not allowed to set future appointments complete."); 
					}
					return false;
				}
				if(!PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
					if(!isSilent) {
						MsgBox.Show(this,"Not allowed to set procedures complete with future dates."); 
					}
					return false;
				}
			}
			if(doCheckDb){
				Pd.Clear(EnumPdTable.Procedure);
			}
			Pd.FillIfNeeded(EnumPdTable.Procedure);
			bool hasProcsAttached=Pd.ListProcedures.Any(x => x.AptNum==appointment.AptNum);
			if(!appointment.AptStatus.In(ApptStatus.PtNote,ApptStatus.PtNoteCompleted)  //PtNote blocked above, added here in case we ever enhance
				&& !PrefC.GetBool(PrefName.ApptAllowEmptyComplete)
				&& !hasProcsAttached)
			{
				if(!isSilent) {
					MsgBox.Show(this,"Appointments without procedures attached cannot be set complete."); 
				}
				return false;
			}
			#region Provider Term Date Check
			string message=Providers.CheckApptProvidersTermDates(appointment);
			if(message!="") {
				if(!isSilent) {
					MessageBox.Show(this,message);//translated in Providers S class method 
				}
				return false;
			}
			#endregion Provider Term Date Check
			return true;//Appointment row can be completed
		}

		///<summary>Returns true if the selected task row can be set complete.</summary>
		private bool CanCompleteTask(bool doCheckDb,bool isSilent) {
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(!CanDisplayTask() || listSelectedRows.Count!=1) {
				if(!isSilent) {
					MsgBox.Show(this,"Only procedures, single appointments, or single tasks may be set complete.");
				}
				return false;
			}
			if(listSelectedRows.Count!=gridProg.SelectedIndices.Length) {
				return false;
			}
			if(doCheckDb) {
				long taskNum=PIn.Long(listSelectedRows[0]["TaskNum"].ToString());
				Task taskCur=Tasks.GetOne(taskNum);
				if(taskCur==null) {
					if(!isSilent) {
						MsgBox.Show(this,"The task has been deleted or moved.  Try again.");
					}
					return false;
				} 
			}
			return true;
		}

		///<summary>Returns true if the row can be deleted.</summary>
		private bool CanDeleteRow(DataRow row) {
			int skippedSecurity=0;
			int skippedRxSecurity=0;
			int skippedC=0;
			int skippedAttached=0;
			int skippedLinkedToOrthoCase=0;
			int skippedAttachedToPreauth=0;
			return CanDeleteRow(row,doCheckDb:false,ref skippedSecurity,ref skippedRxSecurity,ref skippedC,ref skippedAttached,
				ref skippedLinkedToOrthoCase,ref skippedAttachedToPreauth,null);			
		}

		///<summary>Returns true if the row can be deleted. This overload increments the arguments keeping track of skip reasons.</summary>
		private bool CanDeleteRow(DataRow row,bool doCheckDb,ref int skippedSecurity,ref int skippedRxSecurity,ref int skippedC,ref int skippedAttached,
			ref int skippedLinkedToOrthoCase,ref int skippedAttachedToPreauth,OrthoProcLink orthoProcLink) 
		{
			long procNum=PIn.Long(row["ProcNum"].ToString(),false);
			if(procNum!=0) {
				ProcStat procStat=PIn.Enum<ProcStat>(PIn.Int(row["ProcStatus"].ToString()));
				if(procStat.In(ProcStat.C,ProcStat.EC,ProcStat.EO)
					|| PIn.Bool(row["IsLocked"].ToString()))//takes care of locked group notes and invalidated (deleted and locked) procs
				{
					skippedC++;
					return false;
				}
				if(orthoProcLink!=null) {
					skippedLinkedToOrthoCase++;
					return false;
				}
				DateTime procDate=PIn.DateT(row["ProcDate"].ToString());
				if(procStat.In(ProcStat.TP,ProcStat.TPi)) {
					procDate=PIn.DateT(row["DateEntryC"].ToString());
				}
				long codeNum=PIn.Long(row["CodeNum"].ToString());
				if(ProcedureCodes.GetStringProcCode(codeNum)==ProcedureCodes.GroupProcCode) {//If a group note
					//Check DB to see if attached to any completed procedures. This isn't pulled from datasetmain because we want to be 100% up to date.
					//Note that if multiple rows were selected it might have already deleted some procedures, but we do not delete completed
					//procedures in this loop.
					if(doCheckDb) {
						if(ProcGroupItems.GetCountCompletedProcsForGroup(procNum)==0) { //If not attached to completed procs
							if(!Security.IsAuthorized(Permissions.ProcDelete,procDate)) {
								skippedSecurity++;
								return false;
							}
						}
						else {
							skippedC++;
							return false;
						}
					}
				}
				else {//Not a group note
					if(doCheckDb && !Security.IsAuthorized(Permissions.ProcDelete,procDate)) {
						skippedSecurity++;
						return false;
					}
				}
				//Check if there is an allocated payment to the TP proc or if the proc is attached to a preauth.
				if(PIn.Enum<ProcStat>(PIn.Int(row["ProcStatus"].ToString()))==ProcStat.TP || PIn.Enum<ProcStat>(PIn.Int(row["ProcStatus"].ToString()))==ProcStat.TPi) {
					if(doCheckDb) {
						List<long> listSelectedProcNums=gridProg.SelectedTags<DataRow>().Select(x => PIn.Long(x["ProcNum"].ToString())).ToList();
						List<ClaimProc> listClaimProcsForProc=ClaimProcs.GetForProcs(new List<long>() { procNum }).Where(x => x.ClaimNum!=0 && x.Status==ClaimProcStatus.Preauth).ToList();
						foreach(ClaimProc claimProc in listClaimProcsForProc.DistinctBy(x=>x.ClaimNum)) {
							List<long> listProcNumsForClaim=ClaimProcs.RefreshForClaim(claimProc.ClaimNum).Select(x => x.ProcNum).ToList();
							//If you're trying to delete all the procedures on a preauth, we will block you. We do some thing from the claim edit window (as a preauth).
							if(listProcNumsForClaim.Except(listSelectedProcNums).Count()==0) {
								skippedAttachedToPreauth++;
								return false;							
							}
						}
					}
					Pd.FillIfNeeded(EnumPdTable.PaySplit);
					List<PaySplit> listPaySplits=Pd.ListPaySplits.FindAll(x=>x.ProcNum==procNum);
					double totForProc=listPaySplits.Sum(x => x.SplitAmt);
					if(doCheckDb){
						totForProc=PIn.Double(PaySplits.GetTotForProc(procNum));
					}
					if(!CompareDouble.IsEqual(totForProc,0)) {
						skippedAttached++;
						return false;
					}
				}
				return true;
			}
			else if(row["RxNum"].ToString()!="0") {
				if(PIn.Enum<RxTypes>(row["RxType"].ToString(),isEnumAsString:true)!=RxTypes.Rx) {
					//Users cannot delete rxpat where RxType is not an pdmp access log.
					return false;
				}
				if(doCheckDb && !Security.IsAuthorized(Permissions.RxEdit)) {
					skippedRxSecurity++;
					return false;
				}
				return true;
			}
			//Not a proc or a prescription
			return false;
		}

		///<summary>Returns true if the 'Detach Lab Fee' menu item is applicable.</summary>
		private bool CanDetachLabFee(DataRow row,bool isSilent) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				return false;
			}
			if(row["ProcNum"].ToString()=="0") {
				if(!isSilent) {
					MsgBox.Show(this,"Please select a lab procedure first.");
				}
				return false;
			}
			if(row["ProcNumLab"].ToString()=="0") {
				if(!isSilent) {
					MsgBox.Show(this,"The selected procedure is not attached as a lab procedure.");
				}
				return false;
			}
			return true;
		}

		///<summary>Returns true if at least one appointment row is selected.</summary>
		private bool CanDisplayAppointment() {
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(listSelectedRows.Any(x => x["AptNum"].ToString()!="0")) {
				return true;
			}
			return false;
		}

		///<summary>Returns true if the 'Print Routing Slip' menu item should be displayed.</summary>
		private bool CanDisplayRoutingSlip() {
			if(checkAudit.Checked) {
				return false;
			}
			return gridProg.SelectedIndices.Any(x => ((DataRow)gridProg.ListGridRows[x].Tag)["AptNum"].ToString()!="0");
		}

		///<summary>Returns true if at least one task is selected.</summary>
		private bool CanDisplayTask() {
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(listSelectedRows.Any(x => x["TaskNum"].ToString()=="0")) {
				//Row selected is not a task.
				return false;
			}
			return true;
		}

		///<summary>Returns true if the 'Edit All' menu item is applicable to this row. For the procedures that can be edited, adds them to listProcsToEdit.</summary>
		private bool CanEditRow(DataRow row,bool doCheckDb,bool isSilent,List<Procedure> listProcsToEdit) {
			if(checkAudit.Checked) {
				if(!isSilent) {
					MsgBox.Show(this,"Not allowed to edit procedures when in audit mode.");
				}
				return false;
			}
			long procNum=PIn.Long(row["ProcNum"].ToString());
			if(procNum==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Only procedures may be edited.");
				}
				return false;
			}
			if(doCheckDb){
				Pd.Clear(EnumPdTable.Procedure);
			}
			Pd.FillIfNeeded(EnumPdTable.Procedure);
			Procedure procedure=Pd.ListProcedures.Find(x => x.ProcNum==procNum);
			if(procedure==null) {
				if(!isSilent) {
					MsgBox.Show(this,"Procedure does not exist.");
				}
				return false;
			}
			if(procedure.IsLocked) {
				if(!isSilent) {
					MsgBox.Show(this,"Locked procedures cannot be edited.");
				}
				return false;
			}
			listProcsToEdit.Add(procedure);
			return true;
		}

		///<summary>Returns true if the 'Group for Multi Visit' menu item is applicable.</summary>
		private bool CanGroupMultiVisit(DataRow row,bool doCheckDb,bool isSilent) {
			long procNum=PIn.Long(row["ProcNum"].ToString());
			if(procNum==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Some of the selected items are not procedures.\r\n"
					+"Select only procedures and try again.");
				}
				return false;
			}
			if(gridProg.SelectedTags<DataRow>().Count(x => x["ProcNum"].ToString()!="0")<2) {
				if(!isSilent) {
					MsgBox.Show(this,"At least two procedures must be selected to create a multiple visit group.");
				}
				return false;
			}
			if(doCheckDb){
				Pd.ClearAndFill(EnumPdTable.ProcMultiVisit);
			}
			List<ProcMultiVisit> listProcMultiVisits=Pd.ListProcMultiVisits.FindAll(x => x.ProcNum==procNum);
			if(listProcMultiVisits.Count>0) {
				if(!isSilent) {
					MsgBox.Show(this,"Some of the selected items belong to existing multiple visit groups.\r\n"
					+"Select only procedures which are not part of a multiple visit group and try again.\r\n"
					+"Consider ungrouping an existing group before adding new procedures to the group if needed.");
				}
				return false;
			}
			return true;
		}		
		
		///<summary>Returns true if the row can be put into a group note. Adds procedure to listProcsToGroup if it can be.</summary>
		private bool CanGroupRow(DataRow row,bool doCheckDb,bool isSilent,List<Procedure> listProcsToGroup) {
			long procNum=PIn.Long(row["ProcNum"].ToString());
			if(procNum==0) { //This is not a procedure.
				if(!isSilent) {
					MsgBox.Show(this,"You may only attach a group note to procedures.");
				}
				return false;
			}
			if(doCheckDb){
				Pd.Clear(EnumPdTable.Procedure);
			}
			Pd.FillIfNeeded(EnumPdTable.Procedure);
			Procedure procedure=Pd.ListProcedures.Find(x => x.ProcNum==procNum);
			if(procedure==null) {
				return false;
			}
			if(ProcedureCodes.GetStringProcCode(procedure.CodeNum,doThrowIfMissing: false)==ProcedureCodes.GroupProcCode) {
				if(!isSilent) {
					MsgBox.Show(this,"You cannot attach a group note to another group note.");
				}
				return false;
			}
			if(procedure.IsLocked) {
				if(!isSilent) {
					MsgBox.Show(this,"Locked procedures cannot be attached to a group note.");
				}
				return false;
			}
			if(procedure.ProcStatus!=ProcStat.C) {
				if(!isSilent) {
					MsgBox.Show(this,"Procedures must be complete to attach a group note.");
				}
				return false;
			}
			if(procedure.ProcStatus==ProcStat.C && procedure.ProcDate.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				if(!isSilent) {
					MsgBox.Show(this,"Completed procedures cannot be set complete for days in the future.");
				}
				return false;
			}
			listProcsToGroup.Add(procedure);
			return true;
		}

		///<summary>Returns true if the 'Print Day for Hospital' menu item is applicable. This method returns true if there is at least one completed procedure with the same date as the row.</summary>
		private bool CanPrintDay(DataRow row,int rowIdx) {
			if(PrefC.GetBool(PrefName.EasyHideHospitals)) {
				return false;
			}
			bool isCompletedProc(DataRow rowToCheck) {
				return PIn.Long(rowToCheck["ProcNum"].ToString())!=0 && PIn.Enum<ProcStat>(rowToCheck["ProcStatus"].ToString())==ProcStat.C;
			}
			if(isCompletedProc(row)) {
				return true;
			}
			DateTime dateRow=PIn.Date(row["ProcDate"].ToString()).Date;
			//Look at all the rows of the same date before this row.
			for(int i=rowIdx-1;i>=0;i--) {
				DataRow otherRow=(DataRow)gridProg.ListGridRows[i].Tag;
				DateTime dateOtherRow=PIn.Date(otherRow["ProcDate"].ToString());
				if(dateRow!=dateOtherRow) {
					break;
				}
				if(isCompletedProc(otherRow)) {
					return true;
				}
			}
			//Look at all the rows of the same date after this row.
			for(int i=rowIdx+1;i<gridProg.ListGridRows.Count;i++) {
				DataRow otherRow=(DataRow)gridProg.ListGridRows[i].Tag;
				DateTime dateOtherRow=PIn.Date(otherRow["ProcDate"].ToString()).Date;
				if(dateRow!=dateOtherRow) {
					break;
				}
				if(isCompletedProc(otherRow)) {
					return true;
				}
			}
			return false;//No completed procs found for this date.
		}

		///<summary>Returns true if the 'Print Routing Slip' menu item should be enabled.</summary>
		private bool CanPrintRoutingSlip(bool isSilent) {
			if(gridProg.SelectedIndices.Length==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Please select an appointment first.");
				}
				return false;
			}
			if(checkAudit.Checked) {
				if(!isSilent) {
					MsgBox.Show(this,"Not allowed in audit mode.");
				}
				return false;
			}
			if(gridProg.SelectedIndices.Length!=1
				|| ((DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[0]].Tag)["AptNum"].ToString()=="0") 
			{
				if(!isSilent) {
					MsgBox.Show(this,"Routing slips can only be printed for single appointments.");
				}
				return false;
			}
			return true;
		}

		///<summary>Returns true if the 'Ungroup for Multi Visit' menu item is applicable.</summary>
		private bool CanUngroupMultiVisit(DataRow row,bool doCheckDb,bool isSilent) {
			long procNum=PIn.Long(row["ProcNum"].ToString());
			if(procNum==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Some of the selected items are not procedures.\r\n"
					+"Select only procedures and try again.");
				}
				return false;
			}
			if(!doCheckDb) {//We'll check the db later before ungrouping.
				List<ProcMultiVisit> listProcMVs=Pd.ListProcMultiVisits.FindAll(x => x.ProcNum==procNum);
				if(listProcMVs.Count==0) {
					if(!isSilent) {
						MsgBox.Show(this,"Selected procedures are not part of multi visit group.");
					}
					return false;
				}
			}
			return true;
		}

		/*
		///<summary>Displays all XVWeb images to the chart module for the patient selected and the thumbnails we have in our list.</summary>
		private void DisplayXVWebImages(long patNum) {
			if(InvokeRequired) {
				Invoke((Action)(() => DisplayXVWebImages(patNum)));
				return;
			}
			if(_listApteryxThumbnails==null || _patCur==null
				|| patNum!=_patCur.PatNum) //In case the patient was changed while we were downloading images
			{
				return;
			}
			long imageCatNum=PIn.Long(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),XVWeb.ProgramProps.ImageCategory));
			if(tabControlImages.SelectedIndex>0 //any category except 'all'
				&& imageCatNum!=Defs.GetDefsForCategory(DefCat.ImageCats,true)[(int)_arrayListVisImageCats[tabControlImages.SelectedIndex-1]].DefNum) 
			{
				return;//if the currently selected tab is not for XVWeb
			}
			for(int i=listViewImages.Items.Count-1;i>=0;i--) {
				ApteryxImage imageTag=listViewImages.Items[i].Tag as ApteryxImage;
				if(imageTag!=null) {
					imageListThumbnails.Images.RemoveAt(i);
					listViewImages.Items.RemoveAt(i);
				}
			}
			lock(_apteryxLocker) {
				for(int i=0;i<_listApteryxThumbnails.Count;i++) {
					ApteryxImage imgCur=_listApteryxThumbnails[i].Image;
					imageListThumbnails.Images.Add(_listApteryxThumbnails[i].Thumbnail);
					ListViewItem item=new ListViewItem(imgCur.AcquisitionDate.ToShortDateString()+": "+imgCur.FormattedTeeth,imageListThumbnails.Images.Count-1);
					item.Tag=imgCur;
					listViewImages.Items.Add(item);
				}
			}
		}*/

		///<summary>Returns true if the row passed in should be displayed.  Otherwise; false.</summary>
		private bool DoesGridProgRowPassFilter(DataRow rowCur,List<ProcGroupItem> procGroupItems,List<long> procNumList) {
			long procNumCur=PIn.Long(rowCur["ProcNum"].ToString());//increase code efficiency
			long patNumCur=PIn.Long(rowCur["PatNum"].ToString());//increase code efficiency
			if(procNumCur!=0) {//if this is a procedure 
				//if it's a group note and we are viewing by tooth number
				if(rowCur["ProcCode"].ToString()==ProcedureCodes.GroupProcCode && checkShowTeeth.Checked) {
					//consult the list of previously obtained procedures and ProcGroupItems to see if this procgroup should be visible.
					bool showGroupNote=false;
					for(int j=0;j<procGroupItems.Count;j++) {//loop through all procGroupItems for the patient. 
						if(procGroupItems[j].GroupNum==procNumCur) {//if this item is associated with this group note
							for(int k=0;k<procNumList.Count;k++) {//check all of the visible procs
								if(procNumList[k]==procGroupItems[j].ProcNum) {//if this group note is associated with a visible proc
									showGroupNote=true;
								}
							}
						}
					}
					if(!showGroupNote) {
						return false;//don't show it in the grid
					}
				}
				else {//procedure or group note, not viewing by tooth number
					if(ShouldDisplayProc(rowCur)) {
						_listDataRowsProcsForGraphical.Add(rowCur);//show it in the graphical tooth chart
						//show it in the grid below
					}
					else {
						return false;//don't show it in the grid
					}
				}
			}
			else if(rowCur["CommlogNum"].ToString()!="0") {//if this is a commlog
				if(!checkComm.Checked) {
					return false;
				}
				if(!checkShowCommAuto.Checked
					&& Commlogs.IsAutomated(rowCur["commType"].ToString(),PIn.Enum<CommItemSource>(rowCur["CommSource"].ToString()))) //If this is an automated commlog.
				{
					return false;
				}
				if(!IsPatientNull() && patNumCur!=Pd.PatNum) {//if this is a different family member
					if(!checkCommFamily.Checked) {
						return false;
					}
					if(!checkCommSuperFamily.Checked && PrefC.GetBool(PrefName.ShowFeatureSuperfamilies)) {//Don't show super family members
						if(!Pd.ListPatients.Any(x => x.PatNum==patNumCur)) {//if this is a different super family member (outside family)
							return false;
						}
					}
				}
			}
			//ODHQ only - rows should only exist (have WebChatNums) if at HQ
			else if(rowCur["WebChatSessionNum"].ToString()!="0") {//if this is a web chat
				if(!checkComm.Checked) {
					return false;
				}
				if(patNumCur!=Pd.PatNum) {//if this is a different family member
					return false;
				}
			}
			else if(rowCur["RxNum"].ToString()!="0") {//if this is an Rx
				if(!checkRx.Checked) {
					return false;
				}
			}
			else if(rowCur["LabCaseNum"].ToString()!="0") {//if this is a LabCase
				if(!checkLabCase.Checked) {
					return false;
				}
			}
			else if(rowCur["TaskNum"].ToString()!="0") {//if this is a TaskItem
				if(!checkTasks.Checked) {
					return false;
				}
				if(!IsPatientNull() && patNumCur!=Pd.PatNum) {//if this is a different family member
					if(!checkCommFamily.Checked) { //uses same check box as commlog
						return false;
					}
				}
			}
			else if(rowCur["EmailMessageNum"].ToString()!="0") {//if this is an Email
				if(!checkEmail.Checked || ((HideInFlags)PIn.Int(rowCur["EmailMessageHideIn"].ToString())).HasFlag(HideInFlags.ChartProgNotes)) {
					return false;
				}
				EmailType type=(EmailType)PIn.Int(rowCur["EmailMessageHtmlType"].ToString());
				if(type==EmailType.Html) {
					//HTML emails can be massive.
					//GridOD controls will only display so many characters in the Note section so no need to waste time processing more than that limit.
					string noteLimited=rowCur["note"].ToString();
					if(!string.IsNullOrEmpty(noteLimited) && noteLimited.Length > GridOD.TEXT_LENGTH_LIMIT) {
						noteLimited=noteLimited.Substring(0,GridOD.TEXT_LENGTH_LIMIT);
					}
					rowCur["note"]=MarkupEdit.ConvertMarkupToPlainText(noteLimited);
				}
				else if(type==EmailType.RawHtml) {
					rowCur["note"]="Raw HTML Email";//user needs to double click to see contents. Currently no way to strip out all code.
				}
			}
			else if(rowCur["AptNum"].ToString()!="0") {//if this is an Appointment
				if(!checkAppt.Checked) {
					return false;
				}
			}
			else if(rowCur["SheetNum"].ToString()!="0") {//if this is a sheet
				if(!checkSheets.Checked) {
					return false;
				}
			}
			if(_dateTimeShowDateStart.Year>1880 && PIn.Date(rowCur["ProcDate"].ToString()).Date<_dateTimeShowDateStart.Date) {
				return false;
			}
			if(_dateTimeShowDateEnd.Year>1880 && PIn.Date(rowCur["ProcDate"].ToString()).Date>_dateTimeShowDateEnd.Date) {
				return false;
			}
			return true;
		}		
		
		private void DrawProcGraphics(bool isPrinting=false) {
			//this requires: ProcStatus, ProcCode, ToothNum, HideGraphics, Surf, and ToothRange.  All need to be raw database values.
			string[] arrayTeeth;
			List<long> listCurProvNums=_toothChartRelay.GetPertinentProvNumsForToothColorPref(Security.CurUser,Pd.Patient,Pd.ListAppointments);
			for(int i=0;i<_listDataRowsProcsForGraphical.Count;i++) {
				if(_listDataRowsProcsForGraphical[i]["HideGraphics"].ToString()=="1") {
					continue;
				}
				ProcedureCode procCode=ProcedureCodes.GetProcCode(_listDataRowsProcsForGraphical[i]["ProcCode"].ToString());
				if(procCode.PaintType==ToothPaintingType.None || procCode.TreatArea==TreatmentArea.Mouth) {
					continue;
				}
				if(procCode.PaintType==ToothPaintingType.Extraction && (
					PIn.Long(_listDataRowsProcsForGraphical[i]["ProcStatus"].ToString())==(int)ProcStat.C
					|| PIn.Long(_listDataRowsProcsForGraphical[i]["ProcStatus"].ToString())==(int)ProcStat.EC
					|| PIn.Long(_listDataRowsProcsForGraphical[i]["ProcStatus"].ToString())==(int)ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				ProcStat curStatus=(ProcStat)PIn.Long(_listDataRowsProcsForGraphical[i]["ProcStatus"].ToString());
				long procProvNum=PIn.Long(_listDataRowsProcsForGraphical[i]["ProvNum"].ToString());
				bool doApplyColorPref=_toothChartRelay.DoesToothColorPrefApply(listCurProvNums,procProvNum) && !isPrinting;
				_toothChartRelay.GetToothColors(procCode,curStatus,doApplyColorPref,out Color cDark,out Color cLight);
				switch(procCode.PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(_listToothInitialsCopy,_listDataRowsProcsForGraphical[i]["ToothNum"].ToString())){
							_toothChartRelay.SetPontic(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						}
						else{
							_toothChartRelay.SetCrown(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(_listToothInitialsCopy,_listDataRowsProcsForGraphical[i]["ToothNum"].ToString())) {
							_toothChartRelay.SetPontic(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cLight);
						}
						else {
							_toothChartRelay.SetCrown(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						_toothChartRelay.SetCrown(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.CrownLight:
						_toothChartRelay.SetCrown(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="U") {
							arrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								arrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="L") {
							arrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								arrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							arrayTeeth=_listDataRowsProcsForGraphical[i]["ToothRange"].ToString().Split(new char[] {','});
						}
						for(int t=0;t<arrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(_listToothInitialsCopy,arrayTeeth[t])) {
								_toothChartRelay.SetPontic(arrayTeeth[t],cDark);
							}
							else {
								_toothChartRelay.SetCrown(arrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="U") {
							arrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								arrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="L") {
							arrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								arrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							arrayTeeth=_listDataRowsProcsForGraphical[i]["ToothRange"].ToString().Split(new char[] { ',' });
						}
						for(int t=0;t<arrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(_listToothInitialsCopy,arrayTeeth[t])) {
								_toothChartRelay.SetPontic(arrayTeeth[t],cLight);
							}
							else {
								_toothChartRelay.SetCrown(arrayTeeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						_toothChartRelay.SetBigX(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.FillingDark:
						_toothChartRelay.SetSurfaceColors(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),_listDataRowsProcsForGraphical[i]["Surf"].ToString(),cDark);
						break;
				  case ToothPaintingType.FillingLight:
						_toothChartRelay.SetSurfaceColors(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),_listDataRowsProcsForGraphical[i]["Surf"].ToString(),cLight);
						break;
					case ToothPaintingType.Implant:
						_toothChartRelay.SetImplant(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.PostBU:
						_toothChartRelay.SetBU(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RCT:
						_toothChartRelay.SetRCT(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						_toothChartRelay.SetRetainedRoot(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.Sealant:
						_toothChartRelay.SetSealant(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(procCode.TreatArea==TreatmentArea.Tooth && _listDataRowsProcsForGraphical[i]["ToothNum"].ToString()!=""){
							_toothChartRelay.SetSpaceMaintainer(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark);
						}
						else if((procCode.TreatArea==TreatmentArea.ToothRange && _listDataRowsProcsForGraphical[i]["ToothRange"].ToString()!="") 
							|| (procCode.AreaAlsoToothRange==true && _listDataRowsProcsForGraphical[i]["ToothRange"].ToString()!=""))
						{
							arrayTeeth=_listDataRowsProcsForGraphical[i]["ToothRange"].ToString().Split(',');
							for(int t=0;t<arrayTeeth.Length;t++) {
								_toothChartRelay.SetSpaceMaintainer(arrayTeeth[t],cDark);
							}
						}
						else if(procCode.TreatArea==TreatmentArea.Quad){
							arrayTeeth=new string[0];
							if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="UR") {
								arrayTeeth=new string[] {"4","5","6","7","8"};
							}
							if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="UL") {
								arrayTeeth=new string[] {"9","10","11","12","13"};
							}
							if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="LL") {
								arrayTeeth=new string[] {"20","21","22","23","24"};
							}
							if(_listDataRowsProcsForGraphical[i]["Surf"].ToString()=="LR") {
								arrayTeeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<arrayTeeth.Length;t++) {//could still be length 0
								_toothChartRelay.SetSpaceMaintainer(arrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						_toothChartRelay.SetText(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cDark,procCode.PaintText);
						break;
					case ToothPaintingType.Veneer:
						_toothChartRelay.SetVeneer(_listDataRowsProcsForGraphical[i]["ToothNum"].ToString(),cLight);
						break;
				}
			}
		}

		private void DrawOrthoHardware(){
			if(!checkOrthoGraphics.Checked){
				return;
			}
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			List<OrthoHardwares.OrthoWire> listOrthoWires=new List<OrthoHardwares.OrthoWire>();//also used for elastics
			FillOrthoDateIfNeeded();
			List<OrthoHardware> listOrthoHardwares=Pd.ListOrthoHardwares.FindAll(x=>x.DateExam==comboOrthoDate.GetSelected<DateTime>());
			for(int i=0;i<listOrthoHardwares.Count;i++){
				OrthoHardwareSpec orthoHardwareSpec=listOrthoHardwareSpecs.Find(x=>x.OrthoHardwareSpecNum==listOrthoHardwares[i].OrthoHardwareSpecNum);
				if(listOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Bracket){
					_toothChartRelay.SetBracket(listOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor);
				}
				if(listOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Wire){
					listOrthoWires.AddRange(OrthoHardwares.GetWires(listOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor));
				}
				if(listOrthoHardwares[i].OrthoHardwareType==EnumOrthoHardwareType.Elastic){
					listOrthoWires.AddRange(OrthoHardwares.GetElastics(listOrthoHardwares[i].ToothRange,orthoHardwareSpec.ItemColor));
				}
			}
			for(int i=0;i<listOrthoWires.Count;i++){
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.BetweenBrackets){
					_toothChartRelay.AddOrthoWireBetweenBrackets(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ToothIDend,listOrthoWires[i].ColorDraw);
				}
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.InBracket){
					_toothChartRelay.AddOrthoWireInBracket(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ColorDraw);
				}
				if(listOrthoWires[i].OrthoWireType==OrthoHardwares.EnumOrthoWireType.Elastic){
					_toothChartRelay.AddOrthoElastic(listOrthoWires[i].ToothIDstart,listOrthoWires[i].ToothIDend,listOrthoWires[i].ColorDraw);
				}
			}
		}

		private void EasyHideClinicalData() {
			if(PrefC.GetBool(PrefName.EasyHideClinical)) {
				gridPtInfo.Visible=false;
				checkShowE.Visible=false;
				checkShowR.Visible=false;
				checkRx.Visible=false;
				checkComm.Visible=false;
				checkNotes.Visible=false;
				butShowNone.Visible=false;
				butShowAll.Visible=false;
				//panelEnterTx.Visible=false;//next line changes it, though
				radioEntryEC.Visible=false;
				radioEntryEO.Visible=false;
				radioEntryR.Visible=false;
				labelDx.Visible=false;
				listDx.Visible=false;
				labelPrognosis.Visible=false;
				comboPrognosis.Visible=false;
			}
			else {
				gridPtInfo.Visible=true;
				checkShowE.Visible=true;
				checkShowR.Visible=true;
				checkRx.Visible=true;
				checkComm.Visible=true;
				checkNotes.Visible=true;
				butShowNone.Visible=true;
				butShowAll.Visible=true;
				radioEntryEC.Visible=true;
				radioEntryEO.Visible=true;
				radioEntryR.Visible=true;
				labelDx.Visible=true;
				listDx.Visible=true;
				labelPrognosis.Visible=true;
				comboPrognosis.Visible=true;
			}
		}

		private void FillChartViewsGrid(bool doRefreshViews=true) {
			if(IsPatientNull()) {
				butChartViewAdd.Enabled=false;
				butChartViewDown.Enabled=false;
				butChartViewUp.Enabled=false;
				gridChartViews.Enabled=false;
				return;
			}
			else {
				butChartViewAdd.Enabled=true;
				butChartViewDown.Enabled=true;
				butChartViewUp.Enabled=true;
				gridChartViews.Enabled=true;
			}
			gridChartViews.BeginUpdate();
			gridChartViews.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableChartViews","F#"),25);
			gridChartViews.Columns.Add(col);
			col=new GridColumn(Lan.g("TableChartViews","View"),50){ IsWidthDynamic=true };
			gridChartViews.Columns.Add(col);
			gridChartViews.ListGridRows.Clear();
			GridRow row;
			if(doRefreshViews) {
				ChartViews.RefreshCache();//Ideally this would use signals to refresh
			}
			_listChartViews=ChartViews.GetDeepCopy();
			if(SyncChartViewItemOrders()) {
				ChartViews.RefreshCache();
				_listChartViews=ChartViews.GetDeepCopy();
			}
			for(int i=0;i<_listChartViews.Count;i++) {
				row=new GridRow();
				//assign hot keys F1-F12
				if(i<=11) {
					row.Cells.Add("F"+(i+1));
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(_listChartViews[i].Description);
				gridChartViews.ListGridRows.Add(row);
			}
			gridChartViews.EndUpdate();
		}

		private void FillCustomerTab() {
			FillCustomerViewsGrid();
			if(IsPatientNull()) {
				gridCustomerViews.Enabled=false;
				listCommonProcs.Enabled=false;
				labelMonth1.Text="";
				labelMonth2.Text="";
				labelMonth3.Text="";
			}
			else {
				//Monthly call time breakdown.
				DateTime startDate=new DateTime(1,1,1);
				Procedure firstProc=Procedures.GetFirstCompletedProcForFamily(Pd.Patient.Guarantor);
				if(firstProc!=null) {
					startDate=firstProc.ProcDate;
				}
				DateTime month0Date=DateTime.Now;
				DateTime month1Date=DateTime.Now.AddMonths(-1);
				DateTime month2Date=DateTime.Now.AddMonths(-2);
				DateTime month3Date=DateTime.Now.AddMonths(-3);
				//Set the month labels.
				labelMonth0.Text=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month0Date.Month);
				labelMonth1.Text=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month1Date.Month);
				labelMonth2.Text=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month2Date.Month);
				labelMonth3.Text=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month3Date.Month);
				List<Commlog>	listCommlogs=Commlogs.GetTimedCommlogsForPat(Pd.Patient.Guarantor);
				TimeSpan month0Span=new TimeSpan();
				TimeSpan month1Span=new TimeSpan();
				TimeSpan month2Span=new TimeSpan();
				TimeSpan month3Span=new TimeSpan();
				TimeSpan totalSpan=new TimeSpan();
				int avgCount=0;
				bool addToAvg=true;
				//Add up the length of time each call took within the corresponding month.
				for(int i=0;i<listCommlogs.Count;i++) {
					DateTime tempDateTime=listCommlogs[i].CommDateTime;
					DateTime tempTimeEnd=listCommlogs[i].DateTimeEnd;
					TimeSpan tempSpan=tempTimeEnd-tempDateTime;
					if(tempDateTime.Year==month0Date.Year && tempDateTime.Month==month0Date.Month) {
						month0Span=month0Span.Add(tempSpan);
						addToAvg=false;//Avg should not include this months numbers.
					}
					else if(tempDateTime.Year==month1Date.Year && tempDateTime.Month==month1Date.Month) {
						month1Span=month1Span.Add(tempSpan);
					}
					else if(tempDateTime.Year==month2Date.Year && tempDateTime.Month==month2Date.Month) {
						month2Span=month2Span.Add(tempSpan);
					}
					else if(tempDateTime.Year==month3Date.Year && tempDateTime.Month==month3Date.Month) {
						month3Span=month3Span.Add(tempSpan);
					}
					//Take current commlog and see if its greater than or equal to two months after first completed proc date.
					if(new DateTime(tempDateTime.Year,tempDateTime.Month,1)>=new DateTime(startDate.AddMonths(2).Year,startDate.AddMonths(2).Month,1)
						&& addToAvg) {
						totalSpan=totalSpan.Add(tempSpan);
						avgCount++;
					}
					addToAvg=true;
				}
				if(month0Span.Hours>=3) {
					textMonth0.BackColor=Color.Red;
					textMonth0.ForeColor=Color.White;
					textMonth0.Font=new Font(textMonth1.Font,FontStyle.Bold);
				}
				else {
					textMonth0.ForeColor=Color.Black;
					textMonth0.BackColor=SystemColors.Control;
					textMonth0.Font=new Font(textMonth1.Font,FontStyle.Regular);
				}
				if(month1Span.Hours>=3) {
					textMonth1.BackColor=Color.Red;
					textMonth1.ForeColor=Color.White;
					textMonth1.Font=new Font(textMonth1.Font,FontStyle.Bold);
				}
				else {
					textMonth1.ForeColor=Color.Black;
					textMonth1.BackColor=SystemColors.Control;
					textMonth1.Font=new Font(textMonth1.Font,FontStyle.Regular);
				}
				if(month2Span.Hours>=3) {
					textMonth2.BackColor=Color.Red;
					textMonth2.ForeColor=Color.White;
					textMonth2.Font=new Font(textMonth2.Font,FontStyle.Bold);
				}
				else {
					textMonth2.ForeColor=Color.Black;
					textMonth2.BackColor=SystemColors.Control;
					textMonth2.Font=new Font(textMonth2.Font,FontStyle.Regular);
				}
				if(month3Span.Hours>=3) {
					textMonth3.BackColor=Color.Red;
					textMonth3.ForeColor=Color.White;
					textMonth3.Font=new Font(textMonth3.Font,FontStyle.Bold);
				}
				else {
					textMonth3.ForeColor=Color.Black;
					textMonth3.BackColor=SystemColors.Control;
					textMonth3.Font=new Font(textMonth3.Font,FontStyle.Regular);
				}
				//Set the text of the boxes.
				textMonth0.Text=month0Span.ToStringHmm();
				textMonth1.Text=month1Span.ToStringHmm();
				textMonth2.Text=month2Span.ToStringHmm();
				textMonth3.Text=month3Span.ToStringHmm();
				if(avgCount>0) {
					int test=(int)totalSpan.TotalMinutes/avgCount;
					textMonthAvg.Text=new TimeSpan(0,(int)totalSpan.TotalMinutes/avgCount,0).ToStringHmm();
				}
				else {
					textMonthAvg.Text="";
				}
			}
		}

		///<summary>FillChartViewsGrid should be called first to synch the grids thus having the chart view cache already refreshed.</summary>
		private void FillCustomerViewsGrid() {
			if(_listChartViews==null) {
				return;//There is no patient selected or FillChartViewsGrid was not correctly called before invoking this method.
			}
			gridCustomerViews.BeginUpdate();
			gridCustomerViews.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableCustomerViews","F#"),25);
			gridCustomerViews.Columns.Add(col);
			col=new GridColumn(Lan.g("TableCustomerViews","View"),50){ IsWidthDynamic=true };
			gridCustomerViews.Columns.Add(col);
			gridCustomerViews.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listChartViews.Count;i++) {
				row=new GridRow();
				//assign hot keys F1-F12
				if(i<=11) {
					row.Cells.Add("F"+(i+1));
				}
				else {
					row.Cells.Add("");
				}
				row.Cells.Add(_listChartViews[i].Description);
				gridCustomerViews.ListGridRows.Add(row);
			}
			gridCustomerViews.EndUpdate();
		}

		///<summary>This method is used to set the Date Range filter start and stop dates based on either a custom date range or DatesShowing property of chart view.</summary>
		private void FillDateRange() {
			textShowDateRange.Text="";
			if(_dateTimeShowDateStart.Year>1880) {
				textShowDateRange.Text+=_dateTimeShowDateStart.ToShortDateString();
			}
			if(_dateTimeShowDateEnd.Year>1880 && _dateTimeShowDateStart!=_dateTimeShowDateEnd) {
				if(textShowDateRange.Text!="") {
					textShowDateRange.Text+="-";
				}
				textShowDateRange.Text+=_dateTimeShowDateEnd.ToShortDateString();
			}
			if(textShowDateRange.Text=="") {
				textShowDateRange.Text=Lan.g(this,"All Dates");
			}
		}

		///<summary>Gets run with each ModuleSelected.  Fills Dx, Prognosis, Priorities, ProcButtons, Date, and Image categories</summary>
		private void FillDxProcImage(bool doRefreshData=true) {
			//if(textDate.errorProvider1.GetError(textDate)==""){
			if(checkToday.Checked) {//textDate.Text=="" || 
				textDate.Text=DateTime.Today.ToShortDateString();
			}
			//}
			List<Def> listChartGraphicColorDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			List<Def> listProcButtonCatDefs=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true);
			List<Def> listDiagnosisDefs=Defs.GetDefsForCategory(DefCat.Diagnosis,true);
			List<Def> listPrognosisDefs=Defs.GetDefsForCategory(DefCat.Prognosis,true);
			List<Def> listTxPrioritiesDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			listDx.Items.Clear();
			foreach(Def diagnosisDef in listDiagnosisDefs) {
				listDx.Items.Add(diagnosisDef.ItemName,diagnosisDef);
			}
			int selectedPrognosis=comboPrognosis.SelectedIndex;//retain prognosis selection
			comboPrognosis.Items.Clear();
			comboPrognosis.Items.AddDefNone(Lan.g(this,"no prognosis"));
			comboPrognosis.Items.AddDefs(listPrognosisDefs);
			int selectedPriority=comboPriority.SelectedIndex;//retain current selection
			comboPriority.Items.Clear();
			comboPriority.Items.AddDefNone(Lan.g(this,"no priority"));//0
			comboPriority.Items.AddDefs(listTxPrioritiesDefs);
			if(selectedPrognosis>0 && selectedPrognosis<comboPrognosis.Items.Count) {
				comboPrognosis.SelectedIndex=selectedPrognosis;
			}
			else {
				comboPrognosis.SelectedIndex=0;
			}
			if(selectedPriority>0 && selectedPriority<comboPriority.Items.Count) {
				//set the selected to what it was before. Don't let the combo remember the old one, in case defs were just edited.
				comboPriority.SelectedIndex=selectedPriority;
			}
			else {
				comboPriority.SelectedIndex=0;
				//or just set to no priority
			}
			int selectedButtonCat=listButtonCats.SelectedIndex;
			listButtonCats.Items.Clear();
			listButtonCats.Items.Add(Lan.g(this,"Quick Buttons"));
			foreach(Def procButtonCatDef in listProcButtonCatDefs) {
				listButtonCats.Items.Add(procButtonCatDef.ItemName,procButtonCatDef);
			}
			if(selectedButtonCat<listButtonCats.Items.Count) {
				listButtonCats.SelectedIndex=selectedButtonCat;
			}
			if(listButtonCats.SelectedIndex==-1	&& listButtonCats.Items.Count>0) {
				listButtonCats.SelectedIndex=0;
			}
			FillProcButtons(doRefreshData);
			int selectedImageTab=tabControlImages.SelectedIndex;//retains current selection
			for(int i=tabControlImages.TabPages.Count-1;i>=0;i--){//backward to remove from end
				LayoutManager.Remove(tabControlImages.TabPages[i]);
			}
			TabPage tabPage;
			tabPage=new TabPage();
			tabPage.Text=Lan.g(this,"All");
			LayoutManager.Add(tabPage,tabControlImages);
			_listDefNumsVisImageCats=new List<long>();
			List<Def> listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<listImageCatDefs.Count;i++) {
				if(listImageCatDefs[i].ItemValue.Contains("X")) {//if tagged to show in Chart
					_listDefNumsVisImageCats.Add(listImageCatDefs[i].DefNum);
					tabPage=new TabPage();
					tabPage.Text=listImageCatDefs[i].ItemName;
					LayoutManager.Add(tabPage,tabControlImages);
				}
			}
			if(selectedImageTab<tabControlImages.TabCount) {
				tabControlImages.SelectedIndex=selectedImageTab;
			}
			panelTPdark.BackColor=listChartGraphicColorDefs[0].ItemColor;
			panelCdark.BackColor=listChartGraphicColorDefs[1].ItemColor;
			panelECdark.BackColor=listChartGraphicColorDefs[2].ItemColor;
			panelEOdark.BackColor=listChartGraphicColorDefs[3].ItemColor;
			panelRdark.BackColor=listChartGraphicColorDefs[4].ItemColor;
			panelTPlight.BackColor=listChartGraphicColorDefs[5].ItemColor;
			panelClight.BackColor=listChartGraphicColorDefs[6].ItemColor;
			panelEClight.BackColor=listChartGraphicColorDefs[7].ItemColor;
			panelEOlight.BackColor=listChartGraphicColorDefs[8].ItemColor;
			panelRlight.BackColor=listChartGraphicColorDefs[9].ItemColor;
    }

		///<summary>Gets run on ModuleSelected and each time a different images tab is selected. It first creates any missing thumbnails, then displays them. So it will be faster after the first time.</summary>
		private void FillImages() {
			_listImageInfos=new List<ImageInfo>();
			listViewImages.Items.Clear();
			for(int i=0;i<imageListThumbnails.Images.Count;i++){
				imageListThumbnails.Images[i].Dispose();
			}
			imageListThumbnails.Images.Clear();
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				//Don't show any images if there is no document path.
				return;
			}
			if(IsPatientNull()) {
				return;
			}
			if(string.IsNullOrEmpty(_patFolder)) {
				return;
			}
			if(!panelImages.Visible) {
				return;
			}
			Pd.FillIfNeeded(EnumPdTable.Mount,EnumPdTable.Document);
			for(int i=0;i<Pd.ListMounts.Count;i++) {
				if(!_listDefNumsVisImageCats.Contains(Pd.ListMounts[i].DocCategory)){
					continue;//category doesn't show in chart
				}
				if(tabControlImages.SelectedIndex>0) {//any category except 'all'
					if(Pd.ListMounts[i].DocCategory!=_listDefNumsVisImageCats[tabControlImages.SelectedIndex-1]){
						continue;//not in current category
					}
				}
				ImageInfo imageInfo=new ImageInfo();
				imageInfo.MountNum=Pd.ListMounts[i].MountNum;
				imageInfo.DateCreated=Pd.ListMounts[i].DateCreated;
				_listImageInfos.Add(imageInfo);
			}
			for(int i=0;i<Pd.ListDocuments.Count;i++) {
				if(Pd.ListDocuments[i].MountItemNum>0){
					continue;//skip images attached to mounts
				}
				if(!_listDefNumsVisImageCats.Contains(Pd.ListDocuments[i].DocCategory)){
					continue;//category doesn't show in chart
				}
				if(tabControlImages.SelectedIndex>0) {//any category except 'all'
					if(Pd.ListDocuments[i].DocCategory!=_listDefNumsVisImageCats[tabControlImages.SelectedIndex-1]){
						continue;//not in current category
					}
				}
				ImageInfo imageInfo=new ImageInfo();
				imageInfo.DocNum=Pd.ListDocuments[i].DocNum;
				_listImageInfos.Add(imageInfo);
			}
			_listImageInfos=_listImageInfos.OrderBy(x=>x.DateCreated).ToList();
			for(int i=0;i<_listImageInfos.Count;i++){
				if(_listImageInfos[i].DocNum>0){
					Document document=Pd.ListDocuments.Find(x=>x.DocNum==_listImageInfos[i].DocNum);
					Bitmap bitmap=Documents.GetThumbnail(document,_patFolder);
					imageListThumbnails.Images.Add(bitmap);
					ListViewItem listViewItem=new ListViewItem(document.DateCreated.ToShortDateString()+": "
						+document.Description,imageListThumbnails.Images.Count-1);
					//item.ToolTipText=patFolder+DocumentList[i].FileName;//not supported by Mono
					listViewImages.Items.Add(listViewItem);
				}
				else{//mount
					Mount mount=Pd.ListMounts.Find(x=>x.MountNum==_listImageInfos[i].MountNum);
					//Bitmap bitmap=new Bitmap(100,100);
					//using Graphics g=Graphics.FromImage(bitmap);
					//g.Clear(mount.ColorBack);
					Bitmap bitmap=Mounts.GetThumbnail(mount.MountNum,_patFolder);
					imageListThumbnails.Images.Add(bitmap);
					ListViewItem listViewItem=new ListViewItem(mount.DateCreated.ToShortDateString()+": "
						+mount.Description,imageListThumbnails.Images.Count-1);
					listViewImages.Items.Add(listViewItem);
				}
			}
			//DisplayXVWebImages(_patCur.PatNum);	
		}

		private void FillListPriorities() {
			listPriorities.Items.Clear();
			listPriorities.Items.Add(Lan.g("ContrChart","No Priority"));
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			foreach(Def txPriorityDef in listDefs) {
				listPriorities.Items.Add(txPriorityDef.ItemName,txPriorityDef);
			}
		}

		private void fillPanelQuickButtons(bool doRefreshData=true) {
			panelQuickButtons.BeginUpdate();
			panelQuickButtons.ListODPanelItems.Clear();
			if(doRefreshData || _listProcButtonQuicks==null) {
				_listProcButtonQuicks=ProcButtonQuicks.GetAll();
			}
			_listProcButtonQuicks.Sort(ProcButtonQuicks.sortYX);
			ODPanelItem panelItem;
			for(int i=0;i<_listProcButtonQuicks.Count;i++) {
				panelItem=new ODPanelItem();
				panelItem.Text=_listProcButtonQuicks[i].Description;
				panelItem.YPos=_listProcButtonQuicks[i].YPos;
				panelItem.ItemOrder=i;
				panelItem.ItemType=(_listProcButtonQuicks[i].IsLabel?ODPanelItemType.Label:ODPanelItemType.Button);
				panelItem.Tags.Add(_listProcButtonQuicks[i]);
				panelQuickButtons.ListODPanelItems.Add(panelItem);
			}
			panelQuickButtons.EndUpdate();
		}

		private void FillProcButtons(bool doRefreshData=true) {
			listViewButtons.Items.Clear();
			imageListProcButtons.Images.Clear();
			panelQuickButtons.Visible=false;
			if(listButtonCats.SelectedIndex==-1) {
				_arrayProcButtons=new ProcButton[0];
				return;
			}
			if(listButtonCats.SelectedIndex==0) {
				panelQuickButtons.Visible=true;
				LayoutManager.MoveLocation(panelQuickButtons,listViewButtons.Location);
				LayoutManager.MoveSize(panelQuickButtons,listViewButtons.Size);
				fillPanelQuickButtons(doRefreshData);
				_arrayProcButtons=new ProcButton[0];
				return;
			}
			if(doRefreshData) {
				ProcButtons.RefreshCache();
			}
			Def selectedButtonCatDef=listButtonCats.GetSelected<Def>();//Will not be null if 'Quick Buttons' is selected due to above if statement
			List<long> listProcButtonDefNums=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true).Select(x => x.DefNum).ToList();
			if(!listProcButtonDefNums.Contains(selectedButtonCatDef.DefNum)) {
				MsgBox.Show(this,"The Procedue Button Category has been hidden.");
				ModuleSelected(Pd.PatNum);
				return;
			}
			_arrayProcButtons=ProcButtons.GetForCat(selectedButtonCatDef.DefNum);
			ListViewItem item;
			for(int i=0;i<_arrayProcButtons.Length;i++) {
				if(_arrayProcButtons[i].ButtonImage!="") {
					//image keys are simply the ProcButtonNum
					imageListProcButtons.Images.Add(_arrayProcButtons[i].ProcButtonNum.ToString(),PIn.Bitmap(_arrayProcButtons[i].ButtonImage));
				}
				item=new ListViewItem(new string[] {_arrayProcButtons[i].Description},_arrayProcButtons[i].ProcButtonNum.ToString());
				listViewButtons.Items.Add(item);
			}
    }

		private void FillToothChart(bool retainSelection) {
			if(PrefC.IsODHQ) {
				return;//ODInternalCustomerGrid is filled instead
			}
			if(IsPatientNull()) {
				FillToothChart(retainSelection,DateTime.Today);
			}
			else {
				FillToothChart(retainSelection,_listProcDates[trackToothProcDates.Value]);
			}
		}

		///<summary>This is, of course, called when module refreshed.  But it's also called when user sets missing teeth or tooth movements.  In that case, the Progress notes are not refreshed, so it's a little faster.  This also fills in the movement amounts.</summary>
		private void FillToothChart(bool retainSelection,DateTime dateLimit) {
			if(PrefC.IsODHQ) {
				return;//ODInternalCustomerGrid is filled instead
			}
			Cursor=Cursors.WaitCursor;
			_toothChartRelay.BeginUpdate();
			_toothChartRelay.SetOrthoMode(checkOrthoMode.Checked);
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(checkOrthoMode.Checked && toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				_toothChartRelay.SetToothNumberingNomenclature(ToothNumberingNomenclature.Palmer);
			}
			else{
				_toothChartRelay.SetToothNumberingNomenclature(toothNumberingNomenclature);
			}
			List<Def> listChartGraphicColorDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			_toothChartRelay.ColorBackgroundMain=listChartGraphicColorDefs[10].ItemColor;
			_toothChartRelay.ColorText=listChartGraphicColorDefs[11].ItemColor;
			_toothChartRelay.ColorTextHighlightFore=listChartGraphicColorDefs[12].ItemColor;
			_toothChartRelay.ColorTextHighlightBack=listChartGraphicColorDefs[13].ItemColor;
			//remember which teeth were selected
			List<string> selectedTeeth=new List<string>(_toothChartRelay.SelectedTeeth);
			_toothChartRelay.ResetTeeth();
			if(IsPatientNull()) {
				_toothChartRelay.EndUpdate();
				FillMovementsAndHidden();
				Cursor=Cursors.Default;
				return;
			}
			//primary teeth need to be set before resetting selected teeth, because some of them might be primary.
			//primary teeth also need to be set before initial list so that we can set a primary tooth missing.
			for(int i=0;i<Pd.ListToothInitials.Count;i++) {
				if(Pd.ListToothInitials[i].InitialType==ToothInitialType.Primary) {
					_toothChartRelay.SetPrimary(Pd.ListToothInitials[i].ToothNum);
				}
			}
			if(checkShowTeeth.Checked || retainSelection) {
				for(int i=0;i<selectedTeeth.Count;i++) {
					_toothChartRelay.SetSelected(selectedTeeth[i],true);
				}
			}
			DataTable tableCur=Pd.TableProgNotes.Copy();
			if(checkTreatPlans.Checked) {
				//filter list of DataRows to only include completed work and work for the selected treatment plans
				List<long> listProcNumsAll=gridTreatPlans.SelectedIndices.SelectMany(x => _listTreatPlans[x].ListProcTPs).Select(x => x.ProcNumOrig).ToList();
				_listDataRowsProcsForGraphical=new List<DataRow>();
				_listProcsSkipped=new List<DataRow>();
				foreach(DataRow rowCur in tableCur.Rows) {
					//If proc status is anything except TP and TPi
					if(new[] { ProcStat.C,ProcStat.Cn,ProcStat.EC,ProcStat.EO,ProcStat.R }.Contains((ProcStat)PIn.Long(rowCur["ProcStatus"].ToString()))
						|| listProcNumsAll.Contains(PIn.Long(rowCur["ProcNum"].ToString())))
					{
						if(!ShouldRowShowGraphical(rowCur,dateLimit)) {
							continue;
						}
						_listDataRowsProcsForGraphical.Add(rowCur);
					}
				}
			}
			else {
				//put list back to the original list of DataRows
				_listProcsSkipped=new List<DataRow>();
				List<string> listOrigProcNumStrs=_listDataRowsProcsOrig.Select(x => x["ProcNum"].ToString()).ToList();
				_listDataRowsProcsForGraphical=tableCur.Select().Where(x => listOrigProcNumStrs.Contains(x["ProcNum"].ToString()) && ShouldRowShowGraphical(x,dateLimit)).ToList();
			}
			_listToothInitialsCopy=Pd.ListToothInitials.Select(x => x.Copy()).ToList();
			foreach(DataRow row in _listProcsSkipped) {
				if(((ProcStat)PIn.Long(row["ProcStatus"].ToString())).In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
					ProcedureCode procCode=ProcedureCodes.GetProcCode(row["ProcCode"].ToString());
					if(procCode.PaintType==ToothPaintingType.Extraction) {
						_listToothInitialsCopy.RemoveAll(x => x.InitialType==ToothInitialType.Missing && x.ToothNum==row["ToothNum"].ToString());
					}
				}
			}
			//Also remove any extractions for TP procs that were set to TP by the ShouldRowShowGraphical
			foreach(DataRow row in _listDataRowsProcsForGraphical) {
				DateTime dateTP;
				DateTime dateComplete;
				ProcStat procStatus=(ProcStat)PIn.Int(row["ProcStatus"].ToString());
				if(!procStatus.In(ProcStat.TP)) {
					continue;
				}
				if(!DateTime.TryParse(row["DateTP"].ToString(),out dateTP)) {
					continue;
				}
				if(!DateTime.TryParse(row["DateEntryC"].ToString(),out dateComplete)) {
					continue;
				}
				ProcedureCode procCode=ProcedureCodes.GetProcCode(row["ProcCode"].ToString());
				if(procCode.PaintType==ToothPaintingType.Extraction 
						&& dateLimit<dateComplete && dateLimit>=dateTP) 
				{//Procedure is C and the slider date is after or equal to the TP date, but before the completion date
					_listToothInitialsCopy.RemoveAll(x => x.InitialType==ToothInitialType.Missing && x.ToothNum==row["ToothNum"].ToString());//Pretend the row is TP for the tooth chart
				}
			}
			for(int i=0;i<_listToothInitialsCopy.Count;i++) {
				switch(_listToothInitialsCopy[i].InitialType) {
					case ToothInitialType.Missing:
						_toothChartRelay.SetMissing(_listToothInitialsCopy[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						_toothChartRelay.SetHidden(_listToothInitialsCopy[i].ToothNum);
						break;
					//case ToothInitialType.Primary:
					//	break;
					case ToothInitialType.Rotate:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,_listToothInitialsCopy[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,0,_listToothInitialsCopy[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,0,0,_listToothInitialsCopy[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,0,0,0,_listToothInitialsCopy[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,0,0,0,0,_listToothInitialsCopy[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						_toothChartRelay.MoveTooth(_listToothInitialsCopy[i].ToothNum,0,0,0,0,0,_listToothInitialsCopy[i].Movement);
						break;
					case ToothInitialType.Drawing:
						_toothChartRelay.AddDrawingSegment(_listToothInitialsCopy[i].Copy());
						break;
					case ToothInitialType.Text:
						_toothChartRelay.AddText(_listToothInitialsCopy[i].GetTextString(),_listToothInitialsCopy[i].GetTextPoint(),_listToothInitialsCopy[i].ColorDraw,_listToothInitialsCopy[i].ToothInitialNum);
						break;
				}
			}
			DrawProcGraphics();
			DrawOrthoHardware();
			_toothChartRelay.EndUpdate();
			FillMovementsAndHidden();
			//By firing this event here, we propogate any charted procs/missing/movements immediately to the Patient Dashboard.  FillToothChart() is always
			//called via FillProgNotes() when ModuleSelected() is invoked, therefore, this event will fire when ModuleSelected() is called.  By only 
			//firing this event here, we avoid duplicate events.
			PatientDashboardDataEventArgs patientDashboardDataEventArgs=new PatientDashboardDataEventArgs();
			if(dateLimit==DateTime.Today) {
				try {
					patientDashboardDataEventArgs.ImageToothChart=_toothChartRelay.GetBitmap();
				}
				catch(Exception ex) {
					ex.DoNothing();//Failing to get the toothchart bitmap would only mean being unable to update the Patient Dashboard.  Since this is a fairly
					//rare exception, we can consider this to be not important enough to crash the program; the next module refresh should update the view.
				}
			}
			patientDashboardDataEventArgs.Pat=Pd.Patient;
			patientDashboardDataEventArgs.Fam=Pd.Family;
			patientDashboardDataEventArgs.ListAppts=Pd.ListAppointments;
			patientDashboardDataEventArgs.ListBenefits=Pd.ListBenefits;
			patientDashboardDataEventArgs.ListDocuments=Pd.ListDocuments;
			patientDashboardDataEventArgs.ListInsPlans=Pd.ListInsPlans;
			patientDashboardDataEventArgs.ListInsSubs=Pd.ListInsSubs;
			patientDashboardDataEventArgs.ListPatPlans=Pd.ListPatPlans;
			patientDashboardDataEventArgs.ListPlannedAppts=patientDashboardDataEventArgs.ExtractPlannedAppts(Pd.Patient,Pd.TablePlannedAppts);
			patientDashboardDataEventArgs.ListToothInitials=Pd.ListToothInitials;
			patientDashboardDataEventArgs.PatNote=Pd.PatientNote;
			patientDashboardDataEventArgs.SuperFamHead=Pd.PatientSuperFamHead;
			patientDashboardDataEventArgs.TableProgNotes=Pd.TableProgNotes;
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,patientDashboardDataEventArgs);
			Cursor=Cursors.Default;
		}

		private void FillTrackSlider() {
			if(PrefC.IsODHQ) {
				return;//ODInternalCustomerGrid is filled instead
			}
			//This method can be called from many places and it would be annoying to the user if their slider always reset to today's date, so allow retaining selection.
			trackToothProcDates.Minimum=0;
			//FillToothChart is called after FillTrackSlider.  We don't need to fire the ValueChanged event, otherwise it calls FillToothChart unnecessarily
			trackToothProcDates.ValueChanged-=trackToothProcDates_ValueChanged;
			trackToothProcDates.Value=0;
			List<string> listProcNumStrsOrig=_listDataRowsProcsOrig.Select(y => y["ProcNum"].ToString()).ToList();
			//Fill the list of unique procedure dates with the new values found in the ProgNotes data table.
			//The proc dates can include TP, Completed, and Scheduled dates.  Each of these dates are significant in the patients history (visually).
			_listProcDates=Pd.TableProgNotes.Select().Where(x => listProcNumStrsOrig.Contains(x["ProcNum"].ToString()))
				.SelectMany(x => new[] { x["DateTP"].ToString(),x["DateEntryC"].ToString(),((DateTime)x["ProcDate"]).ToShortDateString() }.Distinct())
				.Concat(new[] { DateTime.Today.ToShortDateString() })
				.Distinct()
				.Where(x => x!=DateTime.MinValue.ToShortDateString())
				.Select(x => PIn.Date(x))
				.OrderBy(x => x)
				.ToList();
			trackToothProcDates.Maximum=_listProcDates.Count()-1;
			trackToothProcDates.Value=_listProcDates.FindIndex(x => x==DateTime.Today);//Default to today's date which is guaranteed to be in our track bar
			trackToothProcDates.ValueChanged+=trackToothProcDates_ValueChanged;//Add the ValueChanged event handler back after setting the track bar value
			textToothProcDate.Text=_listProcDates[trackToothProcDates.Value].ToShortDateString();
		}

		private void FillTreatPlans() {
			gridTreatPlans.BeginUpdate();
			gridTreatPlans.Columns.Clear();
			gridTreatPlans.Columns.Add(new GridColumn(Lan.g("ChartTPList","Status"),50));
			gridTreatPlans.Columns.Add(new GridColumn(Lan.g("ChartTPList","Heading"),60){ IsWidthDynamic=true });
			gridTreatPlans.Columns.Add(new GridColumn(Lan.g("ChartTPList","Procs"),50,HorizontalAlignment.Center));
			gridTreatPlans.ListGridRows.Clear();
			if(IsPatientNull() || !checkTreatPlans.Checked) {
				gridTreatPlans.EndUpdate();
				return;
			}
			_listPatProcs=Procedures.Refresh(Pd.PatNum);
			_listProceduresTPs=Procedures.GetListTPandTPi(_listPatProcs);//sorted by priority, then toothnum
			_listTreatPlans=TreatPlans.GetAllCurrentForPat(Pd.PatNum);
			GridRow row;
			List<TreatPlanAttach> listTpAttaches=TreatPlanAttaches.GetAllForPatNum(Pd.PatNum);
			for(int i=0;i<_listTreatPlans.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listTreatPlans[i].TPStatus.ToString());
				row.Cells.Add(_listTreatPlans[i].Heading);
				//if(_listTreatPlans[i].ResponsParty!=0) {
				//	//This should never be used for Active or Inactive treatment plans. Saved TPs only.
				//	str+="\r\n"+Lan.g(this,"Responsible Party: ")+Patients.GetLim(_listTreatPlans[i].ResponsParty).GetNameLF();
				//}
				row.Cells.Add(listTpAttaches.FindAll(x=>x.TreatPlanNum==_listTreatPlans[i].TreatPlanNum).Count.ToString());
				row.Tag=listTpAttaches.FindAll(x => x.TreatPlanNum==_listTreatPlans[i].TreatPlanNum);
				gridTreatPlans.ListGridRows.Add(row);
			}
			gridTreatPlans.EndUpdate();
			gridTreatPlans.SetSelected(0,true);
		}

		///<summary>Calls FillTpProcData and FillTpProcDisplay as well as showing checkTreatPlans and filling the priority list.</summary>
		private void FillTpProcs() {
			if(!checkTreatPlans.Checked) {
				return;
			}
			FillTpProcData();
			FillTpProcDisplay();
		}
		
		/// <summary>Fills _dictTpNumListTpRows with TreatPlanNums linked to the list of TpRows for the TP, used to fill gridTpProcs.</summary>
		private void FillTpProcData() {
			_dictTpNumListTpRows=new Dictionary<long,List<TpRow>>();
			List<TpRow> listTpRows;
			for(int i=0;i<gridTreatPlans.SelectedIndices.Length;i++) {
				listTpRows=new List<TpRow>();
				long treatPlanNumCur=_listTreatPlans[gridTreatPlans.SelectedIndices[i]].TreatPlanNum;
				List<TreatPlanAttach> listTreatPlanAttaches=(List<TreatPlanAttach>)gridTreatPlans.ListGridRows[gridTreatPlans.SelectedIndices[i]].Tag;
				List<Procedure> listProcsForTP=Procedures.GetManyProc(listTreatPlanAttaches.Select(x => x.ProcNum).ToList(),false)
					.OrderBy(x => Defs.GetOrder(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==x.ProcNum).Priority)<0)
					.ThenBy(x => Defs.GetOrder(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==x.ProcNum).Priority))
					.ThenBy(x => Tooth.ToInt(x.ToothNum))
					.ThenBy(x => x.ProcDate).ToList();
				List<ProcTP> listProcTPsCur=new List<ProcTP>();
				TpRow row;
				for(int j=0;j<listProcsForTP.Count;j++) {
					row=new TpRow();
					//Fill TpRow object with information.
					row.Priority=Defs.GetName(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==listProcsForTP[j].ProcNum).Priority);
					row.Tth=Tooth.Display(listProcsForTP[j].ToothNum);
					if(ProcedureCodes.GetProcCode(listProcsForTP[j].CodeNum).TreatArea==TreatmentArea.Surf) {
						row.Surf=Tooth.SurfTidyFromDbToDisplay(listProcsForTP[j].Surf,listProcsForTP[j].ToothNum);
					}
					else if(ProcedureCodes.GetProcCode(listProcsForTP[j].CodeNum).TreatArea==TreatmentArea.Sextant) {
						row.Surf=Tooth.GetSextant(listProcsForTP[j].Surf,(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
					}
					else {
						row.Surf=listProcsForTP[j].Surf; //I think this will properly allow UR, L, etc.
					}
					row.Code=ProcedureCodes.GetProcCode(listProcsForTP[j].CodeNum).ProcCode;//returns new ProcedureCode if not found
					string descript=ProcedureCodes.GetLaymanTerm(listProcsForTP[j].CodeNum);
					if(listProcsForTP[j].ToothRange!="") {
						descript+=" #"+Tooth.DisplayRange(listProcsForTP[j].ToothRange);
					}
					row.Description=descript;
					row.ColorText=Defs.GetColor(DefCat.TxPriorities,listTreatPlanAttaches.FirstOrDefault(y => y.ProcNum==listProcsForTP[j].ProcNum).Priority);
					if(row.ColorText==System.Drawing.Color.White) {
						row.ColorText=System.Drawing.Color.Black;
					}
					Procedure proc=listProcsForTP[j];
					ProcTP procTP=new ProcTP();//dummy ProcTP for local list listProcTPsCur, used as the tag on this TP grid row
					procTP.PatNum=Pd.PatNum;
					procTP.TreatPlanNum=treatPlanNumCur;
					procTP.ProcNumOrig=proc.ProcNum;
					procTP.ItemOrder=i;
					procTP.Priority=listTreatPlanAttaches.FirstOrDefault(x => x.ProcNum==proc.ProcNum).Priority;
					procTP.ToothNumTP=Tooth.Display(proc.ToothNum);
					if(ProcedureCodes.GetProcCode(proc.CodeNum).TreatArea==TreatmentArea.Surf) {
						procTP.Surf=Tooth.SurfTidyFromDbToDisplay(proc.Surf,proc.ToothNum);
					}
					else {
						procTP.Surf=proc.Surf;//for UR, L, etc.
					}
					procTP.ProcCode=ProcedureCodes.GetStringProcCode(proc.CodeNum);
					procTP.Descript=row.Description;
					procTP.Prognosis=row.Prognosis;
					procTP.Dx=row.Dx;
					listProcTPsCur.Add(procTP);
					row.Tag=procTP;
					listTpRows.Add(row);
				}
				//if there is another treatment plan after this one, add a row with just the TP Header and a bold lower line
				if(i<gridTreatPlans.SelectedIndices.Length-1 && listTpRows.Count>0) {
					listTpRows[listTpRows.Count-1].ColorLborder=Color.FromArgb(102,102,122);
				}
				_listTreatPlans[gridTreatPlans.SelectedIndices[i]].ListProcTPs=listProcTPsCur;
				_dictTpNumListTpRows[_listTreatPlans[gridTreatPlans.SelectedIndices[i]].TreatPlanNum]=listTpRows;
			}
		}

		///<summary>Fills gridTpProcs with data in _dictTpNumListTpRows.  Could be filled with procs from more than one TP.</summary>
		private void FillTpProcDisplay() {
			gridTpProcs.BeginUpdate();
			gridTpProcs.Columns.Clear();
			gridTpProcs.Columns.Add(new GridColumn(Lan.g("TableTP","Priority"),50));
			gridTpProcs.Columns.Add(new GridColumn(Lan.g("TableTP","Tth"),35));
			gridTpProcs.Columns.Add(new GridColumn(Lan.g("TableTP","Surf"),40));
			gridTpProcs.Columns.Add(new GridColumn(Lan.g("TableTP","Code"),50));
			gridTpProcs.Columns.Add(new GridColumn(Lan.g("TableTP","Description"),50){ IsWidthDynamic=true });
			gridTpProcs.ListGridRows.Clear();
			if(IsPatientNull() || _dictTpNumListTpRows==null || gridTreatPlans.ListGridRows.Count==0) {
				gridTpProcs.EndUpdate();
				return;
			}
			GridRow row;
			foreach(KeyValuePair<long,List<TpRow>> kvPair in _dictTpNumListTpRows) {
				row=new GridRow();
				if(_dictTpNumListTpRows.Count>1) {
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add(_listTreatPlans.FindAll(x => x.TreatPlanNum==kvPair.Key).DefaultIfEmpty(new TreatPlan() { Heading="" }).FirstOrDefault().Heading);
					row.Bold=true;
					row.ColorLborder=Color.FromArgb(102,102,122);//from odGrid painting logic
					row.ColorBackG=Color.FromArgb(224,223,227);//from odGrid painting logic
					gridTpProcs.ListGridRows.Add(row);
					row=new GridRow();
				}
				foreach(TpRow tpRow in kvPair.Value) {
					ProcTP procTp=new ProcTP();
					if(tpRow.Tag!=null) {
						procTp=(ProcTP)tpRow.Tag;
					}
					row.Cells.Add(tpRow.Priority??"");
					row.Cells.Add(tpRow.Tth??"");
					row.Cells.Add(tpRow.Surf??"");
					row.Cells.Add(tpRow.Code??"");
					row.Cells.Add(tpRow.Description??"");
					row.ColorText=tpRow.ColorText;
					row.ColorLborder=tpRow.ColorLborder;
					row.Tag=tpRow.Tag;//Tag is a ProcTP
					row.Bold=tpRow.Bold;
					gridTpProcs.ListGridRows.Add(row);
					row=new GridRow();
				}
			}
			gridTpProcs.EndUpdate();
		}

		///<summary>Returns the appropriate ChartModuleComponentsToLoad.</summary>
		private ChartModuleFilters GetChartModuleComponents() {
			if(UsingEcwTight()) {//ecw customers
				return new ChartModuleFilters(
					checkAppt.Checked,										//showAppointments
					checkComm.Checked,										//showCommLog.  The button is in a different toolbar.
					checkShowC.Checked,										//showCompleted
					checkShowCn.Checked,									//showConditions
					false, //checkEmail.Checked,					//showEmail
					checkShowE.Checked,										//showExisting
					false, //checkCommFamily.Checked,			//showFamilyCommLog
					false, //checkCommSuperFamily.Checked	//showSuperFamilyCommLog
					false,																//showFormPat
					checkLabCase.Checked,									//showLabCases
					checkNotes.Checked,										//showProcNotes
					checkShowR.Checked,										//showReferred
					checkRx.Checked,											//showRX
					checkSheets.Checked,									//showSheets, consent
					false, //checkTasks.Checked,					//showTasks (for now)
					checkShowTP.Checked);									//showTreatPlan
			}
			//all other customers and ecw full users
			return new ChartModuleFilters(
				checkAppt.Checked,						//showAppointments
				checkComm.Checked,						//showCommLog
				checkShowC.Checked,						//showCompleted
				checkShowCn.Checked,					//showConditions
				checkEmail.Checked,						//showEmail
				checkShowE.Checked,						//showExisting
				checkCommFamily.Checked,			//showFamilyCommLog
				checkCommSuperFamily.Checked,	//showSuperFamilyCommLog
				true,													//showFormPat
				checkLabCase.Checked,					//showLabCases
				checkNotes.Checked,						//showProcNotes
				checkShowR.Checked,						//showReferred
				checkRx.Checked,							//showRX
				checkSheets.Checked,					//showSheets, consent
				checkTasks.Checked,						//showTasks
				checkShowTP.Checked);					//showTreatPlan
		}

		/*
		private void GetXVWebImages(ODThread thread) {
			Patient patient=(Patient)thread.Parameters[0];
			List<string> listIdsToExclude=(List<string>)thread.Parameters[1];
			lock(_apteryxLocker) {
				if(_listApteryxThumbnails==null) {
					_listApteryxThumbnails=new List<ApteryxThumbnail>();
				}
				_listApteryxThumbnails.RemoveAll(x => x.PatNum!=patient.PatNum);
				listIdsToExclude.AddRange(_listApteryxThumbnails.Select(x => x.Image.Id.ToString()));
			}
			bool doDisplayXVWebInChart=XVWeb.IsDisplayingImagesInProgram
				&& Defs.GetDefsForCategory(DefCat.ImageCats,true).Any(x => x.ItemValue.Contains("X") //if tagged to show in Chart
				&& x.DefNum==PIn.Long(ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.XVWeb),XVWeb.ProgramProps.ImageCategory)));
			if(!doDisplayXVWebInChart) {
				return;
			}
			//make requests to the XVWeb Api to get a list of images for this patient.
			List<ApteryxThumbnail> listAT=new List<ApteryxThumbnail>();
			foreach(ApteryxThumbnail thumbnail in XVWeb.GetListThumbnails(patient,listIdsToExclude)) {
				lock(_apteryxLocker) {
					_listApteryxThumbnails.Add(thumbnail);
				}
				DisplayXVWebImages(patient.PatNum);
			}
		}*/

		///<summary>Returns an ODGridRow object which dictates how the row passed in should be displayed.</summary>
		private GridRow GridProgRowConstruction(DataRow rowCur,List<DisplayField> listDisplayFields) {
			long procNumCur=PIn.Long(rowCur["ProcNum"].ToString());//increase code efficiency
			GridRow row=new GridRow();
			row.ColorLborder=Color.Black;
			//remember that columns that start with lowercase are already altered for display rather than being raw data.
			for(int f=0;f<listDisplayFields.Count;f++) {
				switch(listDisplayFields[f].InternalName) {
					case "Date":
						row.Cells.Add(rowCur["procDate"].ToString());
						break;
					case "Time":
						row.Cells.Add(rowCur["procTime"].ToString());
						break;
					case "Th":
						row.Cells.Add(rowCur["toothNum"].ToString());
						break;
					case "Surf":
						row.Cells.Add(rowCur["surf"].ToString());
						break;
					case "Dx":
						row.Cells.Add(rowCur["dx"].ToString());
						break;
					case "Description":
						row.Cells.Add(rowCur["description"].ToString());
						break;
					case "Stat":
						long procNum=PIn.Long(rowCur["ProcNum"].ToString());
						if(ProcMultiVisits.IsProcInProcess(procNum)) {
							row.Cells.Add(Lan.g("enumProcStat",ProcStatExt.InProcess));
						}
						else {
							row.Cells.Add(rowCur["procStatus"].ToString());//Already translated for display
						}
						break;
					case "Prov":
						row.Cells.Add(rowCur["prov"].ToString());
						break;
					case "Amount":
						row.Cells.Add(rowCur["procFee"].ToString());
						break;
					case "Proc Code":
						row.Cells.Add(rowCur["ProcCode"].ToString());
						break;
					case "User":
						row.Cells.Add(rowCur["user"].ToString());
						break;
					case "Signed":
						row.Cells.Add(rowCur["signature"].ToString());
						break;
					case "Priority":
						row.Cells.Add(rowCur["priority"].ToString());
						break;
					case "Date Entry":
						row.Cells.Add(rowCur["dateEntryC"].ToString());
						break;
					case "Prognosis":
						row.Cells.Add(rowCur["prognosis"].ToString());
						break;
					case "Date TP":
						row.Cells.Add(rowCur["dateTP"].ToString());
						break;
					case "End Time":
						row.Cells.Add(rowCur["procTimeEnd"].ToString());
						break;
					case "Quadrant":
						row.Cells.Add(rowCur["quadrant"].ToString());
						break;
					case "Schedule By":
						row.Cells.Add(rowCur["orionDateScheduleBy"].ToString());
						break;
					case "Stop Clock":
						row.Cells.Add(rowCur["orionDateStopClock"].ToString());
						break;
					case "DPC":
						row.Cells.Add(rowCur["orionDPC"].ToString());
						break;
					case "Effective Comm":
						row.Cells.Add(rowCur["orionIsEffectiveComm"].ToString());
						break;
					case "On Call":
						row.Cells.Add(rowCur["orionIsOnCall"].ToString());
						break;
					case "Stat 2":
						row.Cells.Add(rowCur["orionStatus2"].ToString());
						break;
					case "DPCpost":
						row.Cells.Add(rowCur["orionDPCpost"].ToString());
						break;
					case "Length":
						row.Cells.Add(rowCur["length"].ToString());
						break;
					case "Abbr": //abbreviation for procedures
						row.Cells.Add(rowCur["AbbrDesc"].ToString());
						break;
					case "Locked":
						row.Cells.Add(rowCur["isLocked"].ToString());
						break;
					case "HL7 Sent":
						row.Cells.Add(rowCur["hl7Sent"].ToString());
						break;
					case "Clinic":
						row.Cells.Add(Clinics.GetAbbr(PIn.Long(rowCur["ClinicNum"].ToString())));
						break;
					case "ClinicDesc":
						row.Cells.Add(Clinics.GetDesc(PIn.Long(rowCur["ClinicNum"].ToString())));
						break;
					//If you add something here, you should also add it to SearchProgNotesMethod.
					default:
						row.Cells.Add("");
						break;
				}
			}
			if(checkNotes.Checked) {
				//If it's an automated commlog, show only the first line.
				if(Commlogs.IsAutomated(rowCur["commType"].ToString(),PIn.Enum<CommItemSource>(rowCur["CommSource"].ToString()))) {
					row.Note=Commlogs.GetNoteFirstLine(rowCur["note"].ToString());
				}
				else {
					row.Note=rowCur["note"].ToString();
				}
			}
			row.ColorText=Color.FromArgb(PIn.Int(rowCur["colorText"].ToString()));
			long provNum=PIn.Long(rowCur["ProvNum"].ToString());
			if(PrefC.GetBool(PrefName.UseProviderColorsInChart)
					&& procNumCur>0
					&& provNum>0
					&& new[] { ProcStat.C,ProcStat.EC }.Contains((ProcStat)PIn.Int(rowCur["ProcStatus"].ToString())))
			{
				row.ColorBackG=Providers.GetColor(provNum);
			}
			else {
				row.ColorBackG=Color.FromArgb(PIn.Int(rowCur["colorBackG"].ToString()));
			}
			row.Tag=rowCur;
			return row;
		}

		///<summary>Returns true if eCW is enabled and they turned on the Hide Chart Rx Buttons setting within the program link.</summary>
		private bool HasHideRxButtonsEcw() {
			if(Programs.IsEnabled(ProgramName.eClinicalWorks) 
				&& ProgramProperties.GetPropVal(Programs.GetProgramNum(ProgramName.eClinicalWorks),"HideChartRxButtons")=="1") 
			{
				return true;
			}
			return false;
		}

		private bool IsAuditMode(bool isSilent) {
			if(gridProg.SelectedIndices.Count(x => x>-1 && x<gridProg.ListGridRows.Count)==0) {
				if(!isSilent) {
					MsgBox.Show(this,"Please select an item first."); 
				}
				return false;
			}
			if(checkAudit.Checked) {
				if(!isSilent) {
					MsgBox.Show(this,"Not allowed in audit mode."); 
				}
				return false;
			}
			return true;
		}

		///<summary>This returns true if the patient is null or the module is not selected. Previously, we only tested for patient null, but that's not good enough anymore because we won't be setting patient to null when unselecting module. We still do set the patient to null when certain errors happen.</summary>
		private bool IsPatientNull(){
			if(!_isModuleSelected){
				return true;
			}
			if(Pd.Patient is null){
				return true;
			}
			if(Pd.PatNum==0){
				return true;
			}
			return false;
		}

		///<summary>Creates log entries for completed procedures</summary>
		private void logComplCreate(Procedure procCur) {
			if(_procStatusNew!=ProcStat.C) {
				return;
			}
			string teeth=String.Join(", ",_toothChartRelay.SelectedTeeth);
			Procedures.LogProcComplCreate(Pd.PatNum,procCur,teeth);
		}

		///<summary>Sets the selected rows to the ProcStatus passed in.</summary>
		private void MenuItemSetSelectedProcsStatus(ProcStat newProcStatus) {
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(!listSelectedRows.All(x => CanChangeProcsStatus(newProcStatus,x,doCheckDb:true,isSilent:false))) {
				return;
			}
			List<string> listProcCodes=new List<string>();//for automation
			List<Procedure> listProceduresSetComplete=new List<Procedure>();
			List<ClaimProc> listClaimProcs=ClaimProcs.Refresh(Pd.PatNum) ;
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(Pd.PatNum);
			foreach(DataRow row in listSelectedRows) {
				Procedure procOld=Procedures.GetOneProc(PIn.Long(row["ProcNum"].ToString()),true);
				if(procOld.ProcStatus==newProcStatus) {
					continue;
				}
				Procedure procNew=procOld.Copy();
				procNew.ProcStatus=newProcStatus;
				if(procNew.ProcStatus==ProcStat.C) {//Proc set complete.
					#region Setting proc complete
					#region Proc note
					//if procedure was already complete, then don't add more notes.
					//Prompt for default note if the preference is true.
					string procNoteDefault=ProcCodeNotes.GetNote(procNew.ProvNum,procNew.CodeNum,ProcStat.C);
					if(procNew.Note!="" && procNoteDefault!="") {
						procNew.Note+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
					}
					procNew.Note+=procNoteDefault;
					if(!PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {
						//Users do not want to be prompted for auto notes, so remove them all from the procedure note.
						procNew.Note=Regex.Replace(procNew.Note,@"\[\[.+?\]\]","");
					}
					#endregion
					procNew.DateEntryC=DateTime.Now;//Should this be server date?
					if(procNew.DiagnosticCode=="") {
						procNew.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
						procNew.IcdVersion=PrefC.GetByte(PrefName.DxIcdVersion);
					}
					//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
					if(ProcedureCodes.GetStringProcCode(procNew.CodeNum).In("D9986","D9987")) {
						Procedures.SetDateFirstVisit(procNew.ProcDate,2,Pd.Patient);
					}
					listProcCodes.Add(ProcedureCodes.GetStringProcCode(procNew.CodeNum));
					listProceduresSetComplete.Add(procNew);
					#endregion
				}
				if(procNew.AptNum!=0) {//if attached to an appointment
					Appointment appt=Appointments.GetOneApt(procNew.AptNum);
					procNew.ClinicNum=appt.ClinicNum;
					procNew.ProcDate=appt.AptDateTime;
					procNew.PlaceService=Clinics.GetPlaceService(appt.ClinicNum);
				}
				else {
					procNew.ProcDate=PIn.Date(textDate.Text);
					procNew.PlaceService=Clinics.GetPlaceService(Pd.Patient.ClinicNum);
				}
				if(procNew.ProcDate.Year<1880) {
					procNew.ProcDate=MiscData.GetNowDateTime();
				}
				procNew.SiteNum=Pd.Patient.SiteNum;
				if(procNew.ProcStatus.In(ProcStat.EC,ProcStat.EO)) {
					procNew.DiagnosticCode="";
				}
				if(Userods.IsUserCpoe(Security.CurUser)) {
					//Only change the status of IsCpoe to true.  Never set it back to false for any reason.  Once true, always true.
					procNew.IsCpoe=true;
				}
				Plugins.HookAddCode(this,"ContrChart.menuItemSetComplete_Click_procLoop",procNew,procOld);
				ProcedureCode procCode=ProcedureCodes.GetProcCode(procNew.CodeNum);
				OrthoProcLink orthoProcLink=OrthoProcLinks.TryLinkProcForActiveOrthoCase(orthoCaseProcLinkingData,procNew);
				bool isProcLinkedToOrthoCase=orthoProcLink!=null;
				Procedures.FormProcEditUpdate(procNew,procOld,procCode,isProcLinkedToOrthoCase);
				if(isProcLinkedToOrthoCase) {//If proc was linked to an ortho case, pass ortho case data to ComputeEstimates.
					Procedures.ComputeEstimates(procNew,procNew.PatNum,listClaimProcs,false,Pd.ListInsPlans,Pd.ListPatPlans,Pd.ListBenefits,Pd.Patient.Age,Pd.ListInsSubs,
						orthoProcLink,orthoCaseProcLinkingData.ActiveOrthoCase,orthoCaseProcLinkingData.OrthoSchedule,orthoCaseProcLinkingData.ListProcLinksForCase);
				}
				else {
					List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
					List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(Pd.PatNum);
					List<Procedure> listProcedures=Procedures.GetTpForPats(new List<long>(){Pd.PatNum});
					for(int i=0;i<listProcedures.Count;i++) {
						if(listProcedures[i].ProcNum==procNew.ProcNum) {
							break;
						}
						listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
					}
					Procedures.ComputeEstimates(procNew,procNew.PatNum,ref listClaimProcs,false,Pd.ListInsPlans,Pd.ListPatPlans,Pd.ListBenefits,
						Pd.ListClaimProcHists,listClaimProcHistsLoop,true,Pd.Patient.Age,Pd.ListInsSubs);
				}
			}
			//Mimics FormProcEdit.SaveAndClose()
			Recalls.Synch(Pd.PatNum);
			if(listProcCodes.Count>0) {//Only do if we are completing the procedures. 
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,listProcCodes,Pd.PatNum);
				ProcedureL.AfterProcsSetComplete(listProceduresSetComplete);
			}
			ModuleSelected(Pd.PatNum);
		}

		private void ModuleSelectedDoseSpot() {
			if(this.InvokeRequired) {
				this.BeginInvoke((Action)delegate () {
					ModuleSelectedDoseSpot();
				});
				return;
			}
			if(_isModuleSelected) {//User might have switched to another module.
				ModuleSelected(Pd.PatNum);//Always use PatCur because by the time this gets called the patient has been changed to reflect in PatCur.
			}
		}

		///<summary>Returns false if account ID is blank or not in format of 1 or more digits, followed by 3 random alpha-numberic characters, followed by a 2 digit checksum. Only returns true when the NewCrop Account ID is one that was created by OD.</summary>
 		private bool NewCropIsAccountIdValid() {
			bool validKey=false;
			string newCropAccountId=PrefC.GetString(PrefName.NewCropAccountId);
			if(Regex.IsMatch(newCropAccountId,"[0-9]+\\-[0-9A-Za-z]{3}[0-9]{2}")) { //Must contain at least 1 digit for patnum, 1 dash, 3 random alpha-numeric characters, then 2 digits for checksum.
				//Verify key checksum to make certain that this key was generated by OD and not a reseller.
				long patNum=PIn.Long(newCropAccountId.Substring(0,newCropAccountId.IndexOf('-')));
				long checkSum=patNum;
				checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+1])*3;
				checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+2])*5;
				checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+3])*7;
				if((checkSum%100).ToString().PadLeft(2,'0')==newCropAccountId.Substring(newCropAccountId.Length-2)) {
					validKey=true;
				}
			}
			return validKey;
		}

		///<summary>Returns true if new information was pulled back from NewCrop.</summary>
		private bool NewCropRefreshPrescriptions() {
			Program programNewCrop=Programs.GetCur(ProgramName.eRx);
			if(ToolBarMain.Buttons["eRx"]!=null) {//Hidden for eCW
				ToolBarMain.Buttons["eRx"].IsRed=false; //Set the eRx button back to default color.
				ToolBarMain.Invalidate();
			}
			if(!Programs.IsEnabledByHq(ProgramName.eRx,out string err)) {
				MessageBox.Show(err);
				return false;
			}
			if(IsPatientNull()) {
				return false;
			}
			ErxOption erxOption=PIn.Enum<ErxOption>(ProgramProperties.GetPropForProgByDesc(programNewCrop.ProgramNum,Erx.PropertyDescs.ErxOption).PropertyValue);
			if(erxOption!=ErxOption.Legacy && erxOption!=ErxOption.DoseSpotWithLegacy) {
				return false;
			}
			string newCropAccountId=PrefC.GetString(PrefName.NewCropAccountId);
			if(newCropAccountId=="") {//We check for NewCropAccountID validity below, but we also need to be sure to exit this check for resellers if blank.
				return false;
			}
			if(!NewCropIsAccountIdValid()) {
				//The NewCropAccountID will be invalid for resellers, because the checksum will be wrong.
				//Therefore, resellers should be allowed to continue if both the NewCropName and NewCropPassword are specified. NewCrop does not allow blank passwords.
				if(PrefC.GetString(PrefName.NewCropName)=="" || PrefC.GetString(PrefName.NewCropPassword)=="") {
					return false;
				}
			}
			NewCrop.Update1 wsNewCrop=new NewCrop.Update1();//New Crop web services interface.
			NewCrop.Credentials credentials=new NewCrop.Credentials();
			NewCrop.AccountRequest accountRequest=new NewCrop.AccountRequest();
			NewCrop.PatientRequest patientRequest=new NewCrop.PatientRequest();
			NewCrop.PrescriptionHistoryRequest prescriptionHistoryRequest=new NewCrop.PrescriptionHistoryRequest();
			NewCrop.PatientInformationRequester patientInfoRequester=new NewCrop.PatientInformationRequester();
			NewCrop.Result response=new NewCrop.Result();
			string newCropURL=Introspection.GetOverride(Introspection.IntrospectionEntity.NewCropUpdate1URL,"https://secure.newcropaccounts.com/v7/WebServices/Update1.asmx");
			if(ODBuild.IsDebug()) {
				newCropURL="https://preproduction.newcropaccounts.com/v7/WebServices/Update1.asmx";
			}
			wsNewCrop.Url=newCropURL;
			credentials.PartnerName=OpenDentBusiness.NewCrop.NewCropPartnerName;
			credentials.Name=OpenDentBusiness.NewCrop.NewCropAccountName;
			credentials.Password=OpenDentBusiness.NewCrop.NewCropAccountPasssword;
			accountRequest.AccountId=newCropAccountId;
			accountRequest.SiteId="1";//Accounts are always created with SiteId=1.
			patientRequest.PatientId=POut.Long(Pd.PatNum);
			prescriptionHistoryRequest.StartHistory=new DateTime(2012,11,2);//Only used for archived prescriptions. This is the date of first release for NewCrop integration.
			prescriptionHistoryRequest.EndHistory=DateTime.Now;//Only used for archived prescriptions.
			//Prescription Archive Status Values:
			//N = Not archived (i.e. Current Medication) 
			//Y = Archived (i.e. Previous Mediation)
			//% = Both Not Archived and Archived
			//Note: This field will contain values other than Y,N in future releases.
			prescriptionHistoryRequest.PrescriptionArchiveStatus="N";
			//Prescription Status Values:
			//C = Completed Prescription
			//P = Pending Medication
			//% = Both C and P.
			prescriptionHistoryRequest.PrescriptionStatus="C";
			//Prescription Sub Status Values:
			//% = All meds (Returns all meds regardless of the sub status)
			//A = NS (Returns only meds that have a 'NS' - Needs staff sub status)
			//U = DR (Returns only meds that have a 'DR' - Needs doctor review sub status)
			//P = Renewal Request that has been selected for processing on the NewCrop screens - it has not yet been denied, denied and re-written or accepted
			//S = Standard Rx (Returns only meds that have an 'InProc' - InProcess sub status)
			//D = DrugSet source - indicates the prescription was created by selecting the medication from the DrugSet selection box on the ComposeRx page
			//O = Outside Prescription - indicates the prescription was created on the MedEntry page, not prescribed.
			prescriptionHistoryRequest.PrescriptionSubStatus="S";
			patientInfoRequester.UserType="Staff";//Allowed values: Doctor,Staff
			if(Security.CurUser.ProvNum!=0) {//If the current OD user is associated to a doctor, then the request is from a doctor, otherwise from a staff member.
			  patientInfoRequester.UserType="Doctor";
			}
			patientInfoRequester.UserId=POut.Long(Security.CurUser.UserNum);
			//Send the request to NewCrop. Always returns all current medications, and returns medications between the StartHistory and EndHistory dates if requesting archived medications.
			//The patientIdType parameter was added for another vendor and is not often used. We do not use this field. We must pass empty string.
			//The includeSchema parameter is useful for first-time debugging, but in release mode, we should pass N for no.
			wsNewCrop.Timeout=3000;//3 second. The default is 100 seconds, but we cannot wait that long, because prescriptions are checked each time the Chart is refreshed. 1 second is too little, 2 seconds works most of the time. 3 seconds is safe.
			try {
				//throw new Exception("Test communication error in debug mode.");
				response=wsNewCrop.GetPatientFullMedicationHistory6(credentials,accountRequest,patientRequest,prescriptionHistoryRequest,patientInfoRequester,"","N");
			}
			catch { //An exception is thrown when the timeout is reached, or when the NewCrop servers are not accessible (because the servers are down, or because local internet is down).
				//We used to show a popup here each time the refresh failed, but users found it annoying when the NewCrop severs were down, because the popup would show each time they visited the Chart and impeded user workflow.
				//We tried silently logging a warning message into the Application log within system Event Viewer, but we found out that a decent number of users do not have permission to write to the Application log, which causes UEs sometimes.
				//We tried showing a popup exactly 1 time for each instance of OD launched, to avoid the permission issue, but users were still complaining about it popping up and they didn't know what to do to fix it.
				//We now change the background color of the eRx button red, and show an error message when user click the eRx button to alert them that interactions may be out of date.
				if(ToolBarMain.Buttons["eRx"]!=null) {//Hidden for eCW
					ToolBarMain.Buttons["eRx"].IsRed=true; //Marks the eRx button to be drawn with a red color.
					ToolBarMain.Invalidate();
				}
				return false;
			}
			//response.Message = Error message if error.
			//response.RowCount = Number of prescription records returned.
			//response.Status = Status of request. "OK" = success.
			//response.Timing = Not sure what this is for. Tells us how quickly the server responded to the request?
			//response.XmlResponse = The XML data returned, encoded in base 64.
			if(response.Status!=NewCrop.StatusType.OK) {//Other statuses include Fail (ex if credentials are invalid), NotFound (ex if patientId invalid or accoundId invalid), Unknown (no known examples yet)
				//For now we simply abort gracefully.
				return false;
			}
			byte[] xmlResponseBytes=Convert.FromBase64String(response.XmlResponse);
			string xmlResponse=Encoding.UTF8.GetString(xmlResponseBytes);
			if(xmlResponse=="") {//An empty result means that the patient does not currently have any active medications in eRx.
				xmlResponse="<emptyResult/>";//At least one node is needed below to prevent crashing.
				//We need to continue to the bottom of this function even when there are no active medications,
				//so that we can discontinue any medications in the database which were active that are now discontinued in eRx.
			}
			if(ODBuild.IsDebug()) {
				//For capturing the xmlReponse with the newlines properly showing.
				string tempFile=PrefC.GetRandomTempFile(".txt");
				File.WriteAllText(tempFile,xmlResponse);
			}
			XmlDocument xml=new XmlDocument();
			try {
				xml.LoadXml(xmlResponse);
			}
			catch { //In case NewCrop returns invalid XML.
				return false;//abort gracefully
			}
			DateTime rxStartDateT=PrefC.GetDateT(PrefName.ElectronicRxDateStartedUsing131);
			XmlNode nodeNewDataSet=xml.FirstChild;
			List <long> listActiveMedicationPatNums=new List<long>();
			List<RxPat> listNewRx=new List<RxPat>();
			foreach(XmlNode nodeTable in nodeNewDataSet.ChildNodes) {
				RxPat rxOld=null;
				MedicationPat medOrderOld=null;
				RxPat rx=new RxPat();
				//rx.IsControlled not important.  Only used in sending, but this Rx was already sent.
				rx.Disp="";
				rx.DosageCode="";
				rx.Drug="";
				rx.Notes="";
				rx.Refills="";
				rx.SendStatus=RxSendStatus.Unsent;
				rx.Sig="";
				rx.ErxPharmacyInfo="";
				string additionalSig="";
				bool isProv=true;
				long rxCui=0;
				string strDrugName="";
				string strGenericName="";
				string strProvNumOrNpi="";//We used to send ProvNum in LicensedPrescriber.ID to NewCrop, but now we send NPI. We will receive ProvNum for older prescriptions.
				string drugInfo="";
				string externalDrugConcept="";
				foreach(XmlNode nodeRxFieldParent in nodeTable.ChildNodes) {
					XmlNode nodeRxField=nodeRxFieldParent.FirstChild;
					if(nodeRxField==null) {
						continue;
					}
					switch(nodeRxFieldParent.Name.ToLower()) {
						case "deaclasscode":
							//According to Brian from NewCrop:
							//"Possible values are 0 = unscheduled, schedules 1-5, and 9 = unknown.
							//Some states categorize a drug as scheduled, but do not assign a particular level."
							rx.IsControlled=false;
							if(nodeRxField.Value!="0") {
								rx.IsControlled=true;
							}
							break;
						case "dispense"://ex 5.555
							rx.Disp=nodeRxField.Value;
							break;
						case "druginfo"://ex lisinopril 5 mg Tab
							drugInfo=nodeRxField.Value;
							break;
						case "drugname"://ex lisinopril
							strDrugName=nodeRxField.Value;
							break;
						case "externaldrugconcept":
							externalDrugConcept=nodeRxField.Value;//ex "ingredient1, ingredient 2"
							break;
						case "externalpatientid"://patnum passed back from the compose request that initiated this prescription
							rx.PatNum=PIn.Long(nodeRxField.Value);
							break;
						case "externalphysicianid"://NPI passed back from the compose request that initiated this prescription.  For older prescriptions, this will be ProvNum.
							strProvNumOrNpi=nodeRxField.Value;
							break;
						case "externaluserid"://The person who ordered the prescription. Is a ProvNum when provider, or an EmployeeNum when an employee. If EmployeeNum, then is prepended with "emp" because of how we sent it to NewCrop in the first place.
							if(nodeRxField.Value.StartsWith("emp")) {
								isProv=false;
							}
							break;
						case "finaldestinationtype":
							//According to Brian from NewCrop:
							//FinalDestinationType - Indicates the transmission method from NewCrop to the receiving entity.
							//0=Not Transmitted
							//1=Print
							//2=Fax
							//3=Electronic/Surescripts Retail
							//4=Electronic/Surescripts Mail Order
							//5=Test
							if(nodeRxField.Value=="0") {//Not Transmitted
								rx.SendStatus=RxSendStatus.Unsent;
							}
							else if(nodeRxField.Value=="1") {//Print
								rx.SendStatus=RxSendStatus.Printed;
							}
							else if(nodeRxField.Value=="2") {//Fax
								rx.SendStatus=RxSendStatus.Faxed;
							}
							else if(nodeRxField.Value=="3") {//Electronic/Surescripts Retail
								rx.SendStatus=RxSendStatus.SentElect;
							}
							else if(nodeRxField.Value=="4") {//Electronic/Surescripts Mail Order
								rx.SendStatus=RxSendStatus.SentElect;
							}
							else if(nodeRxField.Value=="5") {//Test
								rx.SendStatus=RxSendStatus.Unsent;
							}
							break;
						case "genericname":
							strGenericName=nodeRxField.Value;
							break;
						case "patientfriendlysig"://The concat of all the codified fields.
							rx.Sig=nodeRxField.Value;
							break;
						case "pharmacyncpdp"://ex 9998888
							//We will use this information in the future to find a pharmacy already entered into OD, or to create one dynamically if it does not exist.
							//rx.PharmacyNum;//Get the pharmacy where pharmacy.PharmID = node.Value
							break;
						case "prescriptiondate":
							rx.RxDate=PIn.DateT(nodeRxField.Value);
							break;
						case "prescriptionguid"://32 characters with 4 hyphens. ex ba4d4a84-af0a-4cbf-9437-36feda97d1b6
							rx.ErxGuid=nodeRxField.Value;
							rxOld=RxPats.GetErxByIdForPat(nodeRxField.Value);
							medOrderOld=MedicationPats.GetMedicationOrderByErxIdAndPat(nodeRxField.Value,Pd.PatNum);
							break;
						case "prescriptionnotes"://from the Additional Sig box at the bottom
							additionalSig=nodeRxField.Value;
							break;
						case "refills"://ex 1
							rx.Refills=nodeRxField.Value;
							break;
						case "rxcui"://ex 311354
							rxCui=PIn.Long(nodeRxField.Value);//The RxCui is not returned with all prescriptions, so it can be zero (not set).
							break;
						case "pharmacyfullinfo":
							rx.ErxPharmacyInfo=nodeRxField.Value;	
							break;					
					}
				}//end inner foreach
				if(rx.RxDate<rxStartDateT) {//Ignore prescriptions created before version 13.1.14, because those prescriptions were entered manually by the user.
					continue;
				}
				if(additionalSig!="") {
					if(rx.Sig!="") {//If patient friend SIG is present.
						rx.Sig+=" ";
					}
					rx.Sig+=additionalSig;
				}
				rx.Drug=drugInfo;
				if((drugInfo=="" || drugInfo.ToLower()=="none") && externalDrugConcept!="") {
					rx.Drug=externalDrugConcept;
				}
				//Determine the provider. This is a mess, because we used to send ProvNum in the outgoing XML LicensedPrescriber.ID,
				//but now we send NPI to avoid multiple billing charges for two provider records with the same NPI
				//(the same doctor entered multiple times, for example, one provider for each clinic).
				ErxLog erxLog=ErxLogs.GetLatestForPat(rx.PatNum,rx.RxDate);//Locate the original request corresponding to this prescription.
				if(erxLog!=null && erxLog.ProvNum!=0 && erxLog.DateTStamp.Date==rx.RxDate.Date) {
					Provider provErxLog=Providers.GetFirstOrDefault(x => x.ProvNum==erxLog.ProvNum);
					if((strProvNumOrNpi.Length==10 && provErxLog.NationalProvID==strProvNumOrNpi) || erxLog.ProvNum.ToString()==strProvNumOrNpi) {
						rx.ProvNum=erxLog.ProvNum;
					}
				}
				if(rx.ProvNum==0) {//Not found or the provnum is unknown.
					//The erxLog.ProvNum will be 0 for prescriptions fetched from NewCrop before version 13.3. Could also happen if
					//prescriptions were created when NewCrop was brand new (right before ErxLog was created),
					//or if someone lost a database and they are downloading all the prescriptions from scratch again.
					if(rxOld==null) {//The prescription is being dowloaded for the first time, or is being downloaded again after it was deleted manually by the user.
						List<Provider> listProviders=Providers.GetDeepCopy(true);
						for(int j=0;j<listProviders.Count;j++) {//Try to locate a visible provider matching the NPI on the prescription.
							if(strProvNumOrNpi.Length==10 && listProviders[j].NationalProvID==strProvNumOrNpi) {
								rx.ProvNum=listProviders[j].ProvNum;
								break;
							}
						}
						if(rx.ProvNum==0) {//No visible provider found matching the NPI on the prescription.
							//Try finding a hidden provider matching the NPI on the prescription, or a matching provnum.
							Provider provider=Providers.GetFirstOrDefault(x => x.NationalProvID==strProvNumOrNpi);
							if(provider==null) {
								provider=Providers.GetFirstOrDefault(x => x.ProvNum.ToString()==strProvNumOrNpi);
							}
							if(provider!=null) {
								rx.ProvNum=provider.ProvNum;
							}
						}
						//If rx.ProvNum is still zero, then that means the provider NPI/ProvNum has been modified or somehow deleted (for example, database was lost) for the provider record originally used.
						if(rx.ProvNum==0) {//Catch all
							Provider provUnknown=Providers.GetFirstOrDefault(x => x.FName=="ERX" && x.LName=="UNKNOWN");
							if(provUnknown!=null) {
								rx.ProvNum=provUnknown.ProvNum;
							}
							if(provUnknown==null) {
								provUnknown=new Provider();
								provUnknown.Abbr="UNK";
								provUnknown.FName="ERX";
								provUnknown.LName="UNKNOWN";
								provUnknown.IsHidden=true;
								rx.ProvNum=Providers.Insert(provUnknown);
								Providers.RefreshCache();
							}
						}
					}
					else {//The prescription has already been downloaded in the past.
						rx.ProvNum=rxOld.ProvNum;//Preserve the provnum if already in the database, because it may have already been corrected by the user after the previous download.
					}
				}
				long medicationPatNum=Erx.InsertOrUpdateErxMedication(rxOld,rx,rxCui,strDrugName,strGenericName,isProv);
				listActiveMedicationPatNums.Add(medicationPatNum);
				if(rxOld==null) {//Only add the rx if it is new.  We don't want to trigger automation for existing prescriptions.
					listNewRx.Add(rx);
				}
			}//end foreach
			List<MedicationPat> listAllMedicationsForPatient=MedicationPats.Refresh(Pd.PatNum,false);
			foreach(MedicationPat medication in listAllMedicationsForPatient) {
				if(!Erx.IsFromNewCrop(medication.ErxGuid)) {
					continue;//This medication is not an eRx medicaiton.  It was entered manually inside OD.
				}
				if(listActiveMedicationPatNums.Contains(medication.MedicationPatNum)) {
					continue;//The medication is still active.
				}
				//The medication was discontinued inside the eRx interface.
				medication.DateStop=DateTime.Today.AddDays(-1);//Discontinue the medication as of yesterday so that it will immediately show as discontinued.
				MedicationPats.Update(medication);//Discontinue the medication inside OD to match what shows in the eRx interface.
			}//end foreach
			if(listNewRx.Count>0) {
				AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),Pd.PatNum,0,listNewRx);
			}
			return true;
		}

		private bool OrionProcStatDesired(string status2) {
			//We ought to include procs with no status2 in case one slips through the cracks and for testing.
			if(status2==OrionStatus.None.ToString()) {
				return true;
			}
			//Convert the graphical status "os" into a single string status "status2".
			//Not needed because we never translate orion fields to other languages.
			/*
			string status2="";
			if(os==Lans.g("enumStatus2",OrionStatus.TP.ToString())) {
				status2=OrionStatus.TP.ToString();
			}
			 * etc*/
			return false;
		}

		private void PlannedApptPromptHelper() {
			if(IsPatientNull() || !PrefC.GetBool(PrefName.ShowPlannedAppointmentPrompt) || UsingEcwTightOrFull()) {
				return;
			}
			List<string> listExcludedCodes=CovCats.GetValidCodesForEbenCat(EbenefitCategory.Diagnostic)
				.Union(CovCats.GetValidCodesForEbenCat(EbenefitCategory.DiagnosticXRay))
				.Union(CovCats.GetValidCodesForEbenCat(EbenefitCategory.RoutinePreventive)).ToList();
			List<Procedure> listEligibleProcs=Procedures.RefreshForStatus(Pd.PatNum,ProcStat.TP)
				.Where(x => !listExcludedCodes.Contains(ProcedureCodes.GetProcCode(x.CodeNum).ProcCode))
				.ToList();
			if(listEligibleProcs.Count==0 || listEligibleProcs.Any(x => x.PlannedAptNum!=0)) {//No eligible procs or already an existing planned appt
				return;
			}
			if(!Procedures.RefreshForStatus(Pd.PatNum,ProcStat.TP,false).Any(x => x.DateTP==DateTime.Now.Date)) {
				return;//Patient does not have any work that was TP today
			}
			//Make sure patient has no future scheduled non-recall appointment
			List<Appointment> listAppts=Appointments.GetFutureSchedApts(Pd.PatNum).FindAll(x => x.AptDateTime.Date>DateTime.Now.Date);
			foreach(Appointment apt in listAppts) {
				List<Procedure> listProcsOnAppt=Procedures.GetProcsForSingle(apt.AptNum,false);
				if(listProcsOnAppt.Any(x => !listExcludedCodes.Contains(ProcedureCodes.GetProcCode(x.CodeNum).ProcCode))) {
					return;//Patient has a future scheduled appt that is not Diagnostic,Xray,or Preventative
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Create Planned Appointment with highest priority planned treatment selected?")) {
				return;
			}
			List<Def> listTreatPlanPriorities=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			List<Procedure> listProcsHighestPriority=listEligibleProcs
				.GroupBy(x => listTreatPlanPriorities.Find(y => y.DefNum==x.Priority)?.ItemOrder??int.MaxValue,x => x)
				.OrderBy(x => x.Key).First()?.ToList();
			int itemOrder=Pd.TablePlannedAppts.Rows.Count+1;
			List<long> listProcNums=listProcsHighestPriority.Select(x => x.ProcNum).ToList();
			PlannedApptStatus plannedApptResult=AppointmentL.CreatePlannedAppt(Pd.Patient,itemOrder,listProcNums);
			if(plannedApptResult==PlannedApptStatus.FillGridNeeded) {
				FillPlanned();
			}
		}

		/// <summary> Checks ProcStat passed to see if one of the check boxes on the form contains a check for the ps passed. For example if ps is TP and the checkShowTP.Checked is true it will return true.</summary>
		private bool ProcStatDesired(ProcStat ps,bool isLocked) {
			switch(ps) {
				case ProcStat.TP:
					if(checkShowTP.Checked) {
						return true;
					}
					break;
				case ProcStat.C:
					if(checkShowC.Checked) {
						return true;
					}
					break;
				case ProcStat.EC:
					if(checkShowE.Checked) {
						return true;
					}
					break;
				case ProcStat.EO:
					if(checkShowE.Checked) {
						return true;
					}
					break;
				case ProcStat.R:
					if(checkShowR.Checked) {
						return true;
					}
					break;
				case ProcStat.D:
					if(checkAudit.Checked || (checkShowC.Checked && isLocked)) {
						return true;
					}
					break;
				case ProcStat.Cn:
					if(checkShowCn.Checked) {
						return true;
					}
					break;
				case ProcStat.TPi:
					if(checkTreatPlans.Checked) {
						return true;
					}
					break;
			}
			//TODO: if proc Date is within show date range; return true;
			return false;
		}

		private void RefreshDoseSpotNotifications() {
			if(_butErx==null || IsPatientNull()) {
				return;
			}
			if(!Programs.IsEnabledByHq(ProgramName.eRx,out string err)) {
				MessageBox.Show(err);
				return;
			}
			Program program=Programs.GetCur(ProgramName.eRx);
			if(program==null || !program.Enabled) {
				return;
			}
			ErxOption erxOption=PIn.Enum<ErxOption>(ProgramProperties.GetPropForProgByDesc(program.ProgramNum,Erx.PropertyDescs.ErxOption).PropertyValue);
			if(erxOption!=ErxOption.DoseSpot && erxOption!=ErxOption.DoseSpotWithLegacy) {
				return;
			}
			//Set the menu items for DoseSpot to visible.
			//Setting the menu items visible before this doesn't matter because this method is the only way to make the menu items valid any ways.
			menuItemDoseSpotPendingPescr.Visible=true;
			menuItemDoseSpotRefillReqs.Visible=true;
			menuItemDoseSpotTransactionErrors.Visible=true;
			ODThread threadRefreshDoseSpotNotifications=new ODThread((odThread) => {
				long clinicNum=Clinics.ClinicNum;
				if(PrefC.HasClinicsEnabled && !PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
					clinicNum=Pd.Patient.ClinicNum;
				}
				string doseSpotClinicID="";
				string doseSpotClinicKey="";
				string doseSpotUserID="";
				int countRefillRequests=0;
				int countErrors=0;
				int countPendingPrescriptions=0;
				try {
					doseSpotUserID=DoseSpot.GetUserID(Security.CurUser,clinicNum);
					DoseSpot.GetClinicIdAndKey(clinicNum,doseSpotUserID,null,null,out doseSpotClinicID,out doseSpotClinicKey);
				}
				catch(Exception ex) {
					ex.DoNothing();
					SetErxButtonNotification(-1,-1,-1,true);
					return;
				}
				//We have valid DoseSpot credentials.  Try to access information from DoseSpot's API.  Catch independently to ensure as much data is gathered as possible.
				try {
					DoseSpot.GetPrescriberNotificationCounts(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,out countRefillRequests,out countErrors,out countPendingPrescriptions);
					SetErxButtonNotification(countRefillRequests,countErrors,countPendingPrescriptions,false);
				}
				catch(Exception ex) {
					ex.DoNothing();
					SetErxButtonNotification(-1,-1,-1,true,(ex is ODException oDException && oDException.ErrorCodeAsEnum==ODException.ErrorCodes.DoseSpotNotAuthorized));
				}
				try {
					Action<List<RxPat>> onRxAdd=new Action<List<RxPat>>((listRx) => {
						AutomationL.Trigger(AutomationTrigger.RxCreate,new List<string>(),Pd.PatNum,0,listRx);
					});
					//Consent for DoseSpot to share medication history must be renewed every 24 hours. Once we have patient's consent stored in DB, we renew
					//consent each time we refresh notifications.
					if(Pd.PatientNote.Consent.HasFlag(PatConsentFlags.ShareMedicationHistoryErx)) {
						DoseSpot.SetMedicationHistConsent(Pd.Patient,clinicNum);
					}
					//We should push any changes made locally to DS first.
					DoseSpot.SyncPrescriptionsToDoseSpot(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,Pd.PatNum);
					if(DoseSpot.SyncPrescriptionsFromDoseSpot(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,Pd.PatNum,onRxAdd)) {
						ModuleSelectedDoseSpot();
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
					SetErxButtonNotification(countRefillRequests,countErrors,countPendingPrescriptions,true);
				}
			});
			threadRefreshDoseSpotNotifications.Start();
		}

		///<summary>isFullRefresh is only set to false for eCW.</summary>
		private void RefreshModuleData(long patNum,bool isFullRefresh) {
			UpdateTreatmentNote();
			if(patNum==0) {
				Pd.ClearAll();
				Pd.PatNum=0;
				return;
			}
			if(!isFullRefresh) {
				return;
			}
			bool doMakeSecLog=false;
			if(_patNumLast!=patNum) {
				doMakeSecLog=true;
				_patNumLast=patNum;
			}
			Pd.ClearAll();
			Pd.PatNum=patNum;
			List<EnumPdTable> listPatDataTypes=new List<EnumPdTable>();
			listPatDataTypes.Add(EnumPdTable.Appointment);
			listPatDataTypes.Add(EnumPdTable.Benefit);
			listPatDataTypes.Add(EnumPdTable.ClaimProcHist);
			if(panelImages.Visible){
				listPatDataTypes.Add(EnumPdTable.Document);
				listPatDataTypes.Add(EnumPdTable.Mount);
			}
			listPatDataTypes.Add(EnumPdTable.InsPlan);
			listPatDataTypes.Add(EnumPdTable.InsSub);
			if(checkShowOrtho.Checked){
				listPatDataTypes.Add(EnumPdTable.OrthoChart);
			}
			listPatDataTypes.Add(EnumPdTable.OrthoHardware);
			listPatDataTypes.Add(EnumPdTable.Patient);
			listPatDataTypes.Add(EnumPdTable.PatientNote);
			listPatDataTypes.Add(EnumPdTable.PatPlan);
			listPatDataTypes.Add(EnumPdTable.ProcGroupItem);
			listPatDataTypes.Add(EnumPdTable.ProcMultiVisit);
			listPatDataTypes.Add(EnumPdTable.TableProgNotes);
			listPatDataTypes.Add(EnumPdTable.TablePlannedAppts);
			listPatDataTypes.Add(EnumPdTable.ToothInitial);
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ChartPatientInformation);
			if(listDisplayFields.Exists(x=>x.InternalName=="Allergies")){
				listPatDataTypes.Add(EnumPdTable.Allergy);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Medications")){
				listPatDataTypes.Add(EnumPdTable.MedicationPat);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Patient Portal")){
				listPatDataTypes.Add(EnumPdTable.UserWebHasPortalAccess);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="PatFields")){
				listPatDataTypes.Add(EnumPdTable.PatField);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Pat Restrictions")){
				listPatDataTypes.Add(EnumPdTable.PatRestriction);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Payor Types")){
				listPatDataTypes.Add(EnumPdTable.PayorType);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Problems")){
				listPatDataTypes.Add(EnumPdTable.Disease);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Referred From")){
				listPatDataTypes.Add(EnumPdTable.RefAttach);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Super Head")){
				listPatDataTypes.Add(EnumPdTable.PatientSuperFamHead);
			}
			if(listDisplayFields.Exists(x=>x.InternalName=="Tobacco Use")){
				listPatDataTypes.Add(EnumPdTable.EhrMeasureEvent);
			}
			Pd.FillIfNeededChart(checkAudit.Checked,GetChartModuleComponents(),listPatDataTypes.ToArray());
			if(doMakeSecLog){
				SecurityLogs.MakeLogEntry(Permissions.ChartModule,patNum,"");
			}
			//_listSubstitutionLinks not filled here.
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ || CloudStorage.IsCloudStorage) {
				ODException.SwallowAnyException(() => {
					_patFolder=ImageStore.GetPatientFolder(Pd.Patient,ImageStore.GetPreferredAtoZpath());
				});
			}
			_listProcButtonQuicks=ProcButtonQuicks.GetAll();//this would be better in cache
		}		

		private void SetChartView(ChartView chartView) {
			_chartViewCurDisplay=chartView;
			labelCustView.Visible=false;
			chartCustViewChanged=false;
			FillProgNotes(isForceFirstPage:true);
			LayoutControls();
		}

		///<summary>This does not currently handle custom views.</summary>
		private void SetDateRange() {
			switch(_chartViewCurDisplay.DatesShowing) {
				case ChartViewDates.All:
					_dateTimeShowDateStart=DateTime.MinValue;
					_dateTimeShowDateEnd=DateTime.MinValue;//interpreted as empty.  We want to show all future dates.
					break;
				case ChartViewDates.Today:
					_dateTimeShowDateStart=DateTime.Today;
					_dateTimeShowDateEnd=DateTime.Today;
					break;
				case ChartViewDates.Yesterday:
					_dateTimeShowDateStart=DateTime.Today.AddDays(-1);
					_dateTimeShowDateEnd=DateTime.Today.AddDays(-1);
					break;
				case ChartViewDates.ThisYear:
					_dateTimeShowDateStart=new DateTime(DateTime.Today.Year,1,1);
					_dateTimeShowDateEnd=new DateTime(DateTime.Today.Year,12,31);
					break;
				case ChartViewDates.LastYear:
					_dateTimeShowDateStart=new DateTime(DateTime.Today.Year-1,1,1);
					_dateTimeShowDateEnd=new DateTime(DateTime.Today.Year-1,12,31);
					break;
			}
		}

		///<summary>Currently only used for DoseSpot.</summary>
		private void SetErxButtonNotification(int countRefillRequests,int countErrors,int countPendingPrescriptions,bool isError,bool wasNotAuthorized=false) {
			if(this.InvokeRequired) {
				this.BeginInvoke((Action)delegate () {
					SetErxButtonNotification(countRefillRequests,countErrors,countPendingPrescriptions,isError,wasNotAuthorized);
				});
				return;
			}
			menuItemDoseSpotPendingPescr.Enabled=(!isError);
			menuItemDoseSpotRefillReqs.Enabled=(!isError);
			menuItemDoseSpotTransactionErrors.Enabled=(!isError);
			if(wasNotAuthorized) {
				_butErx.IsRed=false;//Not authorized errors shouldn't make the button red.
			}
			else {
				_butErx.IsRed=isError;//Set the eRx button back to default color.
			}
			_butErx.NotificationText="";
			menuItemDoseSpotPendingPescr.Text="Pending Prescriptions";
			menuItemDoseSpotRefillReqs.Text="Refill Requests";
			menuItemDoseSpotTransactionErrors.Text="Transaction Errors";
			//Has valid counts to display to the user.  There may have been an error, but if we have valid counts we should show them to the user.
			if(countRefillRequests>=0 && countErrors>=0 && countPendingPrescriptions>=0) {
				int numberOfNotifications=Math.Min(99,countRefillRequests+countErrors+countPendingPrescriptions);
				_butErx.NotificationText=(numberOfNotifications==0) ? "" : numberOfNotifications.ToString();
				menuItemDoseSpotPendingPescr.Text+=" ("+countPendingPrescriptions+")";
				menuItemDoseSpotRefillReqs.Text+=" ("+countRefillRequests+")";
				menuItemDoseSpotTransactionErrors.Text+=" ("+countErrors+")";
			}
			ToolBarMain.Invalidate();//Cause the notification text on the eRx button to update as soon as possible.
		}

		///<summary>The supplied procedure row must include these columns: isLocked,ProcDate,ProcStatus,ProcCode,Surf,ToothNum, and ToothRange, all in raw database format.</summary>
		private bool ShouldDisplayProc(DataRow row) {
			//if printing for hospital
			/*
			if(hospitalDate.Year > 1880) {
				if(hospitalDate.Date != PIn.DateT(row["ProcDate"].ToString()).Date) {
					return false;
				}
				if(row["ProcStatus"].ToString() != ((int)ProcStat.C).ToString()) {
					return false;
				}
			}*/
			if(checkShowTeeth.Checked) {//Only show selected teeth
				bool showProc=false;
				//ArrayList selectedTeeth = new ArrayList();//integers 1-32
				//for(int i = 0;i < toothChart.SelectedTeeth.Count;i++) {
				//	selectedTeeth.Add(Tooth.ToInt(toothChart.SelectedTeeth[i]));
				//}
				switch(ProcedureCodes.GetProcCode(row["ProcCode"].ToString()).TreatArea) {
					case TreatmentArea.Arch:
						for(int s=0;s<_toothChartRelay.SelectedTeeth.Count;s++) {
							if(row["Surf"].ToString()=="U" && Tooth.IsMaxillary(_toothChartRelay.SelectedTeeth[s])) {
								showProc=true;
							}
							else if(row["Surf"].ToString()=="L" && !Tooth.IsMaxillary(_toothChartRelay.SelectedTeeth[s])) {
								showProc = true;
							}
						}
						break;
					case TreatmentArea.Mouth:
					case TreatmentArea.None:
					case TreatmentArea.Sextant://nobody will miss it
						showProc=false;
						break;
					case TreatmentArea.Quad:
						for(int s=0;s<_toothChartRelay.SelectedTeeth.Count;s++) {
							if(row["Surf"].ToString()=="UR" && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])>=1 && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])<=8) {
								showProc=true;
							}
							else if(row["Surf"].ToString()=="UL" && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])>=9 && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])<= 16) {
								showProc=true;
							}
							else if(row["Surf"].ToString()=="LL" && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])>=17 && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])<=24) {
								showProc=true;
							}
							else if(row["Surf"].ToString()=="LR" && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])>=25 && Tooth.ToInt(_toothChartRelay.SelectedTeeth[s])<=32) {
								showProc=true;
							}
						}
						break;
					case TreatmentArea.Surf:
					case TreatmentArea.Tooth:
						for(int s=0;s<_toothChartRelay.SelectedTeeth.Count;s++) {
							if(row["ToothNum"].ToString()==_toothChartRelay.SelectedTeeth[s]) {
								showProc=true;
							}
						}
						break;
					case TreatmentArea.ToothRange:
						string[] range=row["ToothRange"].ToString().Split(',');
						for(int s=0;s<_toothChartRelay.SelectedTeeth.Count;s++) {
							for(int r=0;r<range.Length;r++) {
								if(range[r]==_toothChartRelay.SelectedTeeth[s]) {
									showProc=true;
								}
							}
						}
						break;
				}
				if(!showProc) {
					return false;
				}
			}
			bool isLocked=(row["isLocked"].ToString()=="X");
			if(!ProcStatDesired((ProcStat)PIn.Long(row["ProcStatus"].ToString()),isLocked)) {
				return false;
			}
			if(Programs.IsEnabled(ProgramName.Orion)) {
				if(!OrionProcStatDesired((row["orionStatus2"].ToString()))) {
					return false;
				}
			}
			// Put check for showing hygine in here
			// Put check for showing films in here
			return true;
		}		

		///<summary>Returns true if rowCur represents a valid procedure that should be shown on the tooth chart based on the current track bar date. Returns false if rowCur does not have a valid DateTP, DateEntryC, and ProcDate set. Will also return false if rowCur should not be drawn on the tooth chart (based on the track bar's currently selected date / status of rowCur). If the row needs to be skipped, it gets added to _listProcsSkipped so make sure to manage it correctly before calling this method.</summary>
		private bool ShouldRowShowGraphical(DataRow rowCur,DateTime dateLimit) {
			DateTime dateTP;
			DateTime dateComplete;
			DateTime dateScheduled;
			ProcStat procStatus=(ProcStat)PIn.Int(rowCur["ProcStatus"].ToString());
			if(!DateTime.TryParse(rowCur["DateTP"].ToString(),out dateTP)) {
				return false;
			}
			if(!DateTime.TryParse(rowCur["DateEntryC"].ToString(),out dateComplete)) {
				return false;
			}
			if(!DateTime.TryParse(rowCur["aptDateTime"].ToString(),out dateScheduled)) {
				return false;
			}
			if(dateLimit<dateTP) {//slider date is before the TP date
				_listProcsSkipped.Add(rowCur);
				return false;//Skip the proc
			}
			if(!procStatus.In(ProcStat.C,ProcStat.TP) && dateLimit<dateComplete) {//slider date is before the completion date and the procedure is not C or TP
				_listProcsSkipped.Add(rowCur);
				return false;//Skip the proc
			}
			if(procStatus==ProcStat.C && dateLimit<dateComplete && dateLimit>=dateTP) {//Procedure is C and the slider date is after or equal to the TP date, but before the completion date
				rowCur["ProcStatus"]=POut.Int((int)ProcStat.TP);//Pretend the row is TP for the tooth chart
			}
			else if(procStatus==ProcStat.TP && dateLimit>=dateScheduled.Date //Procedure is TP and the slider date is after or equal to the Scheduled date
				&& dateScheduled.Year>1880 && dateLimit!=DateTime.Today) //If the slider is at today, then we want to show what things are like at the current time.
			{
				rowCur["ProcStatus"]=POut.Int((int)ProcStat.C);//Pretend the row is C for the tooth chart
			}
			return true;
		}

		///<summary>Displays the menu item as enabled if all the selected rows return true for isRowRelevant. Displays the menu item as disabled if at least one but not all return true. Hides the menu item if no rows return true.</summary>
		private void ShowMenuItemHelper(MenuItem menuItem,Func<DataRowWithIdx,bool> isRowRelevant) {
			List<DataRowWithIdx> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => new DataRowWithIdx((DataRow)gridProg.ListGridRows[x].Tag,x)).ToList();
			int countRelevant=listSelectedRows.Count(x => isRowRelevant(x));
			if(countRelevant==0) {
				menuItem.Visible=false;
			}
			else {
				menuItem.Visible=true;
				menuItem.Enabled=listSelectedRows.Count==countRelevant;
			}
		}
		
		/*
		private void StartXVWebThread() {
			if(_threadImageRequest!=null) {
				return;
			}
			listViewImages.Scrollable=false;//Setting the scroll bar invisible in order to reduce the amount of blinking that goes on when filling the view.
			long patNum=_patCur.PatNum;
			_threadImageRequest=new ODThread(GetXVWebImages,_patCur.Copy()
				,_arrayDocuments.Where(x => x.ExternalSource==ExternalSourceType.XVWeb).Select(x => x.ExternalGUID).ToList());
			_threadImageRequest.AddExitHandler(o => {
				_threadImageRequest=null;
				this.Invoke(() => listViewImages.Scrollable=true);					
			});
			_threadImageRequest.AddExceptionHandler(e => {
				if(e is ApplicationException) {
					FriendlyException.Show(e.Message,e);
				}
				else {
					FriendlyException.Show("Unable to download images from XVWeb.",e);
				}
			});
			_threadImageRequest.Name="XVWebImageDownload";
			_threadImageRequest.Start(true);
		}*/

		///<summary>Mimics old ChartLayoutHelper logic. Called in various places when we want to ensure that checkTreatPlans is shown when it needs to.</summary>
		private void ToggleCheckTreatPlans() {
			listButtonCats.BringToFront();
			if(IsTPChartingAvailable) {
				//adjust listBtnCats height so it will be 1 pixel above checkTPs Y pos
				LayoutManager.MoveHeight(listButtonCats,checkTreatPlans.Location.Y-listButtonCats.Location.Y-1);
			}
			else {
				checkTreatPlans.Checked=false;//TP charting not avaliable, make sure TP sheet layout is not selected.
				//set listBtnCats height so it will be 2 pixels below checkTPs Y pos + checkTPs height
				LayoutManager.MoveHeight(listButtonCats,checkTreatPlans.Location.Y+checkTreatPlans.Height+2-listButtonCats.Location.Y);
			}
		}

		private void Tool_Anesthesia_Click() {
			/*
			AnestheticData AnestheticDataCur;
			AnestheticDataCur = new AnestheticData();
			FormAnestheticRecord FormAR = new FormAnestheticRecord(PatCur, AnestheticDataCur);
			FormAR.ShowDialog();

			PatCur = Patients.GetPat(Convert.ToInt32(PatCur.PatNum));
			OnPatientSelected(Convert.ToInt32(PatCur.PatNum), Convert.ToString(PatCur), true, Convert.ToString(PatCur));
			FillPtInfo();
			return;*/
		}

		private void Tool_CCD_Click() {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			using FormContinuityOfCareDocument formContinuityOfCareDocument=new FormContinuityOfCareDocument();
			formContinuityOfCareDocument.PatCur=Pd.Patient;
			formContinuityOfCareDocument.ShowDialog();
		}

		///<summary>Only used for eCW tight.  Everyone else has the commlog button up in the main toolbar.</summary>
		private void Tool_Commlog_Click() {
			Commlog commlogCur=new Commlog();
			commlogCur.PatNum=Pd.PatNum;
			commlogCur.CommDateTime=DateTime.Now;
			commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.MISC);
			commlogCur.Mode_=CommItemMode.Phone;
			commlogCur.SentOrReceived=CommSentOrReceived.Received;
			commlogCur.UserNum=Security.CurUser.UserNum;
			commlogCur.IsNew=true;
			using FormCommItem formCommItem=new FormCommItem(commlogCur);
			if(formCommItem.ShowDialog()==DialogResult.OK) {
				ModuleSelected(Pd.PatNum);
			}
		}

		private void Tool_Consent_Click() {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
			if(listSheetDefs.Count>0) {
				MsgBox.Show(this,"Please use dropdown list.");
				return;
			}
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.Consent);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,Pd.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",Pd.PatNum);
			SheetFiller.FillFields(sheet);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void Tool_EHR_Click(bool onLoadShowOrders) {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			//Quarterly key check was removed from here so that any customer can use EHR tools
			//But we require a EHR subscription for them to obtain their MU reports.
			if(Providers.GetProv(Pd.Patient.PriProv)==null) {
				MsgBox.Show(this,"Please set the patient's primary provider first.");
				return;
			}
			using FormEHR formEHR=new FormEHR();
			formEHR.PatNum=Pd.PatNum;
			formEHR.PatientNoteCur=Pd.PatientNote;
			formEHR.FamilyCur=Pd.Family;
			formEHR.ShowDialog();
			if(formEHR.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formEHR.EhrFormResultOnClose==EhrFormResult.PatientSelect) {
				FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(formEHR.PatNum),false);
				ModuleSelected(formEHR.PatNum);
			}
		}

		private void Tool_eRx_Click(bool isShowRefillsAndErrors=false) {
			if(!Security.IsAuthorized(Permissions.RxCreate)) {
				return;
			}
			if(_dictFormErxSessions.ContainsKey(Pd.PatNum) && _dictFormErxSessions[Pd.PatNum]!=null) {
				_dictFormErxSessions[Pd.PatNum].Restore();
				_dictFormErxSessions[Pd.PatNum].BringToFront();
				return;//FormErx is already open for this patient.  Simply bring it to the front to make the user aware that it is still there.
			}
			if(!Programs.IsEnabledByHq(ProgramName.eRx,out string err)) {
				MessageBox.Show(err);
				return;
			}
			Program programErx=Programs.GetCur(ProgramName.eRx);
			ProgramProperty ppErxOption=ProgramProperties.GetPropForProgByDesc(programErx.ProgramNum,Erx.PropertyDescs.ErxOption);
			ErxOption erxOption=PIn.Enum<ErxOption>(ppErxOption.PropertyValue);
			string doseSpotClinicID="";
			string doseSpotClinicKey="";
			string doseSpotUserID="";
			bool isEmp=Erx.IsUserAnEmployee(Security.CurUser);
			Provider prov=null;
			if(!isEmp && Security.CurUser.ProvNum!=0) {
				prov=Providers.GetProv(Security.CurUser.ProvNum);
			}
			else {
				prov=Providers.GetProv(Pd.Patient.PriProv);
			}
			if(erxOption==ErxOption.DoseSpotWithLegacy) {
				//ODCloud does not support Legacy eRx so just use DoseSpot if in web mode.
				if(!ODBuild.IsWeb() && prov.IsErxEnabled==ErxEnabledStatus.EnabledWithLegacy) {
					using InputBox pickErxOption=
						new InputBox(Lan.g(this,"Which eRx option would you like to use?"),new List<string>() { "Legacy","DoseSpot"},false);
					if(pickErxOption.ShowDialog()==DialogResult.Cancel) {
						return;
					}
					if(pickErxOption.SelectedIndex==0) {//Legacy
						erxOption=ErxOption.Legacy;
					}
					else {
						erxOption=ErxOption.DoseSpot;
					}
				}
				else {//It's fine that the provider might not be enabled.  We will check it later and they will be blocked.
					erxOption=ErxOption.DoseSpot;
				}
			}
			#region Provider Term Date Check
			//Prevents prescriptions from being added that have a provider selected that is past their term date
			string message="";
			List<long> listInvalidProvs=Providers.GetInvalidProvsByTermDate(new List<long> { prov.ProvNum },DateTime.Now);
			if(listInvalidProvs.Count>0) {
				if(!isEmp && Security.CurUser.ProvNum!=0) {
					message="The provider attached to this user has a Term Date that has expired. "
						+"Please select another user or change the provider's term date.";
				}
				else {
					message="The primary provider for this patient has a Term Date that has expired. "
						+"Please change the primary provider for this patient or change the provider's term date.";
				}
				MsgBox.Show(this,message);
				return;
			}
			#endregion Provider Term Date Check
			if(erxOption==ErxOption.Legacy) {
				if(ODBuild.IsWeb()) {
					//ODCloud does not support Legacy eRx.
					MsgBox.Show(this,"Legacy eRx is not available while viewing through the web.");
					return;
				}
				string newCropAccountId=PrefC.GetString(PrefName.NewCropAccountId);
				if(newCropAccountId=="") {//NewCrop has not been enabled yet.
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Continuing will enable basic Electronic Rx (eRx).  Fees are associated with this secure e-prescribing system.  See our online manual for details.  At this time, eRx only works for the United States and its territories, including Puerto Rico.  Continue?")) {
						return;
					}
					//prepare the xml document to send--------------------------------------------------------------------------------------
					XmlWriterSettings xmlWSettings=new XmlWriterSettings();
					xmlWSettings.Indent=true;
					xmlWSettings.IndentChars=("    ");
					StringBuilder strBuild=new StringBuilder();
					using(XmlWriter xmlWriter=XmlWriter.Create(strBuild,xmlWSettings)) {
						xmlWriter.WriteStartElement("CustomerIdRequest");
						xmlWriter.WriteStartElement("RegistrationKey");
						xmlWriter.WriteString(PrefC.GetString(PrefName.RegistrationKey));
						xmlWriter.WriteEndElement();
						xmlWriter.WriteEndElement();
					}
#if DEBUG
					OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
					updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
					if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
						IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
						ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
						proxy.Credentials=cred;
						updateService.Proxy=proxy;
					}
					string patNum="";
					try {
						string result=updateService.RequestCustomerID(strBuild.ToString());//may throw error
						XmlDocument xmlDoc=new XmlDocument();
						xmlDoc.LoadXml(result);
						XmlNode node=xmlDoc.SelectSingleNode("//CustomerIdResponse");
						if(node!=null) {
							patNum=node.InnerText;
						}
						if(patNum=="") {
							throw new ApplicationException("Failed to validate registration key.");
						}
						newCropAccountId=patNum;
						newCropAccountId+="-"+CodeBase.MiscUtils.CreateRandomAlphaNumericString(3);
						long checkSum=PIn.Long(patNum);
						checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+1])*3;
						checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+2])*5;
						checkSum+=Convert.ToByte(newCropAccountId[newCropAccountId.IndexOf('-')+3])*7;
						newCropAccountId+=(checkSum%100).ToString().PadLeft(2,'0');
						Prefs.UpdateString(PrefName.NewCropAccountId,newCropAccountId);
						programErx.Enabled=true;
						Programs.Update(programErx);
					}
					catch(Exception ex) {
						FormFriendlyException formFriendlyException=new FormFriendlyException(ex.Message,ex,false);
						formFriendlyException.Show();
						return;
					}
				}
				else { //newCropAccountId!=""
					if(!programErx.Enabled) {
						MessageBox.Show(Lan.g(this,"eRx is currently disabled.")+"\r\n"+Lan.g(this,"To enable, see our online manual for instructions."));
						return;
					}
					if(!NewCropIsAccountIdValid()) {
						string newCropName=PrefC.GetString(PrefName.NewCropName);
						string newCropPassword=PrefC.GetString(PrefName.NewCropPassword);
						if(newCropName=="" || newCropPassword=="") { //NewCrop does not allow blank passwords.
							MsgBox.Show(this,"NewCropName preference and NewCropPassword preference must not be blank when using a NewCrop AccountID provided by a reseller.");
							return;
						}
					}
				}
			}
			else if(erxOption==ErxOption.DoseSpot) {
				if(!programErx.Enabled) {
					MessageBox.Show(Lan.g(this,"eRx is currently disabled.")+"\r\n"+Lan.g(this,"To enable, see our online manual for instructions."));
					return;
				}
				if(Security.CurUser.EmployeeNum==0 && Security.CurUser.ProvNum==0) {
					MsgBox.Show(this,"This user must be associated with either a provider or an employee.  The security admin must make this change before this user can submit prescriptions.");
					return;
				}
				//clinicNum should be 0 for offices not using clinics.
				//This will work properly when retreiving the clinicKey and clinicID
				long clinicNum=0;
				if(PrefC.HasClinicsEnabled) {
					clinicNum=Clinics.ClinicNum;
					if(!isShowRefillsAndErrors && !PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
						clinicNum=Pd.Patient.ClinicNum;
					}
				}
				List<ProgramProperty> listDoseSpotProperties=ProgramProperties.GetForProgram(programErx.ProgramNum)
					.FindAll(x => x.ClinicNum==clinicNum 
						&& (x.PropertyDesc==Erx.PropertyDescs.ClinicID || x.PropertyDesc==Erx.PropertyDescs.ClinicKey));
				byte[] arrayPostData=new byte[1];
				string queryString="";
				bool isDoseSpotAccessAllowed=true;
				try {
					doseSpotUserID=DoseSpot.GetUserID(Security.CurUser,clinicNum);
					DoseSpot.GetClinicIdAndKey(clinicNum,doseSpotUserID,programErx,listDoseSpotProperties,out doseSpotClinicID,out doseSpotClinicKey);
					if(!isShowRefillsAndErrors) {
						string token=DoseSpotREST.GetToken(doseSpotUserID,doseSpotClinicID,doseSpotClinicKey);
						//BuildDoseSpotPostDataBytes will validate patient information and throw exceptions.
						DoseSpot.ValidatePatientData(Pd.Patient);
						OIDExternal oIdExternal=DoseSpot.GetDoseSpotPatID(Pd.PatNum);
						if(oIdExternal==null) {
							DoseSpot.CreateOIDForPatient(PIn.Int(DoseSpotREST.AddPatient(token,Pd.Patient)),Pd.PatNum);
						} 
						else {
							DoseSpotREST.EditPatient(token,Pd.Patient,oIdExternal.IDExternal);
						}
					}
					if(isShowRefillsAndErrors) {
						arrayPostData=ErxXml.BuildDoseSpotPostDataBytesRefillsErrors(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,out queryString);
					}
					else {
						string onBehalfOfUserId="";
						if(isEmp) {
							List<Provider> listProviders=Providers.GetProvsScheduledToday(clinicNum);
							if(!listProviders.Any(x => x.ProvNum==prov.ProvNum)) {
								listProviders.Add(prov);
							}
							using FormProviderPick formProviderPick=new FormProviderPick(listProviders);
							formProviderPick.SelectedProvNum=prov.ProvNum;
							formProviderPick.IsNoneAvailable=false;
							formProviderPick.IsShowAllAvailable=true;
							formProviderPick.ShowDialog();
							if(formProviderPick.DialogResult==DialogResult.Cancel) {
								return;
							}
							List<Userod> listDoseUsers=Userods.GetWhere(x => x.ProvNum==formProviderPick.SelectedProvNum,true);//Only consider non-hidden users.
							listDoseUsers=listDoseUsers.FindAll(x => {//Finds users that have a DoseSpot ID
								try {
									return !string.IsNullOrWhiteSpace(DoseSpot.GetUserID(x,clinicNum));
								}
								catch(Exception) {
									return false;
								}
							});
							Userod userOnBehalfOf=null;
							if(listDoseUsers.Count==1) {
								userOnBehalfOf=listDoseUsers[0];
							}
							else if(listDoseUsers.Count==0) {
								throw new ODException(Lan.g(this,"Could not find DoseSpot User ID for the selected provider."));
							}
							else {
								throw new ODException(Lan.g(this,"There are too many Open Dental users associated to the selected provider."));
							}
							prov=Providers.GetProv(formProviderPick.SelectedProvNum);
							#region Provider Term Date Check
							//Prevents prescriptions from being added that have a provider selected that is past their term date
							listInvalidProvs=Providers.GetInvalidProvsByTermDate(new List<long> { prov.ProvNum },DateTime.Now);
							if(listInvalidProvs.Count>0) {
								message="The provider selected has a Term Date that has expired. Please select another provider.";
								MsgBox.Show(this,message);
								return;
							}
							#endregion Provider Term Date Check
							onBehalfOfUserId=(DoseSpot.GetUserID(userOnBehalfOf,clinicNum));
						}
						arrayPostData=ErxXml.BuildDoseSpotPostDataBytes(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,onBehalfOfUserId,Pd.Patient,out queryString);
					}
					//Running this block in debug won't work.
					if(!ODBuild.IsDebug()) {
						if(!isEmp && Security.CurUser.ProvNum!=0) {//Not a proxy clinician, so we want to validate that they are allowed access.
							DoseSpot.ValidateProvider(prov,clinicNum);
							//hook for additional authorization before prescription is saved
							bool[] arrayAuthorized=new bool[1] { false };
							if(Plugins.HookMethod(this,"ContrChart.Tool_eRx_Click_Authorize",arrayAuthorized,prov)) {
								if(!arrayAuthorized[0]) {
									isDoseSpotAccessAllowed=false;
								}
							}
							string provNpi=Regex.Replace(prov.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
							UpdateErxAccess(provNpi,doseSpotUserID,clinicNum,doseSpotClinicID,doseSpotClinicKey,erxOption);
							ProviderErx provErxDoseSpot=ProviderErxs.GetOneForNpiAndOption(provNpi,erxOption);
							if(provErxDoseSpot.IsEnabled!=ErxStatus.Enabled) {
								MessageBox.Show(Lan.g(this,"Contact support to enable eRx for provider")+" "+prov.Abbr);
								isDoseSpotAccessAllowed=false;
							}
						}
						else {
							//Proxy users still need to have their clinic synced with ODHQ.
							//This call mimics what would happen in UpdateErxAccess above
							if(DoseSpot.SyncClinicErxsWithHQ()) {
								DataValid.SetInvalid(InvalidType.ClinicErxs);
							}
						}
						ClinicErx clinicErxCur=ClinicErxs.GetByClinicIdAndKey(doseSpotClinicID,doseSpotClinicKey);
						if(clinicErxCur.EnabledStatus!=ErxStatus.Enabled) {
							string clinicAbbr="";
							if(clinicErxCur.ClinicNum==-1) {//ClinicErx was inserted from ODHQ, use the ClinicDesc given by an ODHQ staff
								clinicAbbr=clinicErxCur.ClinicDesc;
							}
							else if(clinicErxCur.ClinicNum==0) {//Office Headquarters
								clinicAbbr="Headquarters";
							}
							else {
								clinicAbbr=Clinics.GetAbbr(clinicErxCur.ClinicNum);
							}
							MessageBox.Show(Lan.g(this,"Contact support to enable eRx for clinic")+" "+clinicAbbr);
							isDoseSpotAccessAllowed=false;
						}
					}
					//Try to add any self reported medications to DoseSpot before the user gets views their list.
					DoseSpot.SyncPrescriptionsToDoseSpot(doseSpotClinicID,doseSpotClinicKey,doseSpotUserID,Pd.PatNum);
				}
				catch(ODException odException) {
					FormFriendlyException formFriendlyException=new FormFriendlyException(odException.Message,odException,false);
					formFriendlyException.Show();//The ODExceptions thrown in this context have already been translated.
					return;
				}
				catch(Exception ex) {
					FormFriendlyException formFriendlyException=new FormFriendlyException("Error: "+ex.Message,ex,false);
					formFriendlyException.Show();
					return;
				}
				if(isDoseSpotAccessAllowed) {
					//Enforce Latest IE Version Available.
					if(MiscUtils.TryUpdateIeEmulation()) {
						MsgBox.Show(this,"Browser emulation version updated.\r\nYou must restart this application before using eRx.");
						return;
					}
					if(ODBuild.IsWeb()) {
						//If ODCloud, open a new browser tab instead of using FormErx because the WebView2 control does not work with VirtualUI.
						Process.Start(Erx.GetRxDoseSpotUrl(queryString));
					}
					else {
						//The user is either a provider with granted access, or a proxy clinician
						FormErx formErx=new FormErx();
						formErx.PatientCur=Pd.Patient;
						formErx.ByteArray=arrayPostData;
						formErx.StringSSOQuery=queryString;
						formErx.ErxOptionCur=erxOption;
						_dictFormErxSessions[Pd.PatNum]=formErx;
						formErx.Show();//Non-modal so user can browse OD while writing prescription.  When form is closed, ErxBrowserClosed() is called below.
					}
				}
				ErxLog erxDoseSpotLog=new ErxLog();
				erxDoseSpotLog.PatNum=Pd.PatNum;
				erxDoseSpotLog.MsgText=queryString;
				erxDoseSpotLog.ProvNum=prov.ProvNum;
				erxDoseSpotLog.UserNum=Security.CurUser.UserNum;
				SecurityLogs.MakeLogEntry(Permissions.RxCreate,erxDoseSpotLog.PatNum,Lan.g(this,"eRx DoseSpot entry made for provider")+" "+Providers.GetAbbr(erxDoseSpotLog.ProvNum));
				ErxLogs.Insert(erxDoseSpotLog);
				return;
			}
			//Validation------------------------------------------------------------------------------------------------------------------------------------------------------
			if(Security.CurUser.EmployeeNum==0 && Security.CurUser.ProvNum==0) {
				MsgBox.Show(this,"This user must be associated with either a provider or an employee.  The security admin must make this change before this user can submit prescriptions.");
				return;
			}
			if(IsPatientNull()) {
				MsgBox.Show(this,"No patient selected.");
				return;
			}
			Employee emp=null;
			Clinic clinic=null;
			try {
				Erx.ValidatePracticeInfo();
				//Clinic Validation
				if(PrefC.HasClinicsEnabled) {
					if(PrefC.GetBool(PrefName.ElectronicRxClinicUseSelected)) {
						clinic=Clinics.GetClinic(Clinics.ClinicNum);
					}
					else if(Pd.Patient.ClinicNum!=0) {//Use patient default clinic if the patient has one.
						clinic=Clinics.GetClinic(Pd.Patient.ClinicNum);
					}
					if(clinic!=null) {
						Erx.ValidateClinic(clinic);
					}
				}
				if(isEmp) {
					emp=Employees.GetEmp(Security.CurUser.EmployeeNum);
					if(emp.LName=="") {//Checked in UI, but check here just in case this database was converted from another software.
						MessageBox.Show(Lan.g(this,"Employee last name missing for user")+": "+Security.CurUser.UserName);
						return;
					}
					if(emp.FName=="") {//Not validated in UI.
						MessageBox.Show(Lan.g(this,"Employee first name missing for user")+": "+Security.CurUser.UserName);
						return;
					}
				}
				Erx.ValidateProv(prov,clinic);
				//hook for additional authorization before prescription is saved
				bool[] arrayAuthorized=new bool[1] { false };
				if(Plugins.HookMethod(this,"ContrChart.Tool_eRx_Click_Authorize2",arrayAuthorized,prov)) {
					if(!arrayAuthorized[0]) {
						throw new ODException(Lans.g("Erx","Provider is not authenticated"));
					}
				}
				Erx.ValidatePat(Pd.Patient);
			}
			catch(ODException ex) {//Purposefully only catch exceptions we throw due to validation
				FormFriendlyException formFriendlyException=new FormFriendlyException(ex.Message,ex,false);
				formFriendlyException.Show();
				return;
			}
			#region ProviderErx Validation
			string npi=Regex.Replace(prov.NationalProvID,"[^0-9]*","");//NPI with all non-numeric characters removed.
			bool isAccessAllowed=true;
			UpdateErxAccess(npi,"",0,"","",erxOption);//0/blank/blank for clinicNum/clinicid/clinickey is fine because we don't enable/disable the clinic for NewCrop.
			ProviderErx provErx=ProviderErxs.GetOneForNpiAndOption(npi,erxOption);
			if(!PrefC.GetBool(PrefName.NewCropIsLegacy) && !provErx.IsIdentifyProofed) {
				if(PrefC.GetString(PrefName.NewCropPartnerName)!="" || PrefC.GetString(PrefName.NewCropPassword)!="") {//Customer of a distributor
					MessageBox.Show(Lan.g(this,"Provider")+" "+prov.Abbr+" "
						+Lan.g(this,"must complete Identity Proofing (IDP) before using eRx.  Call support for details."));
				}
				else {//Customer of OD proper or customer of a reseller
					MessageBox.Show(Lan.g(this,"Provider")+" "+prov.Abbr+" "+Lan.g(this,"must complete Identity Proofing (IDP) before using eRx.  "
						+"Please call support to schedule an IDP appointment."));
				}
				isAccessAllowed=false;
			}
			if(provErx.IsEnabled!=ErxStatus.Enabled) {
				MessageBox.Show(Lan.g(this,"Contact support to enable eRx for provider")+" "+prov.Abbr);
				isAccessAllowed=false;
			}
			#endregion ProviderErx Validation
			string clickThroughXml="";
			byte[] arrayPostDataBytes=ErxXml.BuildNewCropPostDataBytes(prov,emp,Pd.Patient,out clickThroughXml);
			string tempString="";//This is only used for introspection
			if(Introspection.TryGetOverride(Introspection.IntrospectionEntity.NewCropRxEntryURL, out tempString)) { 
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(clickThroughXml);
				msgBoxCopyPaste.ShowDialog();
			}
			#region Launch eRx in external browser window.
//			string xmlBase64=System.Web.HttpUtility.HtmlEncode(Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(clickThroughXml)));
//			xmlBase64=xmlBase64.Replace("+","%2B");//A common base 64 character which needs to be escaped within URLs.
//			xmlBase64=xmlBase64.Replace("/","%2F");//A common base 64 character which needs to be escaped within URLs.
//			xmlBase64=xmlBase64.Replace("=","%3D");//Base 64 strings usually end in '=', which could mean a new parameter definition within the URL so we escape.
//			String postdata="RxInput=base64:"+xmlBase64;
//			byte[] PostDataBytes=System.Text.Encoding.UTF8.GetBytes(postdata);
//			string additionalHeaders="Content-Type: application/x-www-form-urlencoded\r\n";
//			IWebBrowserApp IE=(IWebBrowserApp)IEControl;
//			IE.Visible=true;
//			if(ODBuild.IsDebug()) {
//				string newCropUrl="http://preproduction.newcropaccounts.com/interfaceV7/rxentry.aspx";
//			}
//			else {
//				string newCropUrl="https://secure.newcropaccounts.com/interfacev7/rxentry.aspx";
//			}
//			IE.Navigate(newCropUrl,null,null,PostDataBytes,additionalHeaders);
			#endregion Launch eRx in external browser window.
			try {
				//Enforce Latest IE Version Available.
				if(MiscUtils.TryUpdateIeEmulation()) {
					MsgBox.Show(this,"Browser emulation version updated.\r\nYou must restart this application before using eRx.");
					return;
				}
				FormErx formErx=new FormErx();
				formErx.PatientCur=Pd.Patient;
				formErx.ByteArray=arrayPostDataBytes;
				formErx.ErxOptionCur=erxOption;
				if(isAccessAllowed) {
					_dictFormErxSessions[Pd.PatNum]=formErx;
					formErx.Show();//Non-modal so user can browse OD while writing prescription.  When form is closed, ErxBrowserClosed() is called below.
				}
				else {
					//This is how we send the provider information to NewCrop without allowing the provider to use NewCrop.
					//NewCrop requires the provider information on their server in order to complete Identity Proofing (IDP).
					Erx.ComposeNewRxLegacyWebRequest(arrayPostDataBytes);
				}
			}
			catch(Exception ex) {
				FormFriendlyException formFriendlyException=new FormFriendlyException(Lan.g(this,"Error launching browser window.  Internet Explorer might not be installed.")+" "+ex.Message,ex,false);
				formFriendlyException.Show();
				return;
			}
			ErxLog erxLog=new ErxLog();
			erxLog.PatNum=Pd.PatNum;
			erxLog.MsgText=clickThroughXml;
			erxLog.ProvNum=prov.ProvNum;
			erxLog.UserNum=Security.CurUser.UserNum;
			SecurityLogs.MakeLogEntry(Permissions.RxCreate,erxLog.PatNum,Lan.g(this,"eRx entry made for provider")+" "+Providers.GetAbbr(erxLog.ProvNum));
			ErxLogs.Insert(erxLog);
		}

		private void Tool_ExamSheet_Click() {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			FormExamSheets formExamSheets=new FormExamSheets();
			formExamSheets.PatNum=Pd.PatNum;
			formExamSheets.FormClosing+=FormExamSheets_FormClosing;
			formExamSheets.Show();
		}

		private void Tool_HL7_Click() {
			DataRow row;
			if(gridProg.SelectedIndices.Length==0) {
				//autoselect procedures
				for(int i=0;i<gridProg.ListGridRows.Count;i++) {//loop through every line showing in progress notes
					row=(DataRow)gridProg.ListGridRows[i].Tag;
					if(row["ProcNum"].ToString()=="0") {
						continue;//ignore non-procedures
					}
					//May want to ignore procs with zero fee?
					//if((decimal)row["chargesDouble"]==0) {
					//  continue;//ignore zero fee procedures, but user can explicitly select them
					//}
					if(PIn.Date(row["ProcDate"].ToString())==DateTime.Today && PIn.Int(row["ProcStatus"].ToString())==(int)ProcStat.C) {
						gridProg.SetSelected(i,true);
					}
				}
				if(gridProg.SelectedIndices.Length==0) {//if still none selected
					MsgBox.Show(this,"Please select procedures first.");
					return;
				}
			}
			List<Procedure> listProcs=new List<Procedure>();
			bool allAreProcedures=true;
			for(int i=0;i<gridProg.SelectedIndices.Length;i++) {
				row=(DataRow)gridProg.ListGridRows[gridProg.SelectedIndices[i]].Tag;
				if(row["ProcNum"].ToString()=="0") {
					allAreProcedures=false;
				}
				else {
					listProcs.Add(Procedures.GetOneProc(PIn.Long(row["ProcNum"].ToString()),false));
				}
			}
			if(!allAreProcedures) {
				MsgBox.Show(this,"You can only select procedures.");
				return;
			}
			long aptNum=0;
			for(int i=0;i<listProcs.Count;i++) {
				if(listProcs[i].AptNum==0) {
					continue;
				}
				aptNum=listProcs[i].AptNum;
				break;
			}
			if(HL7Defs.GetOneDeepEnabled().IsProcApptEnforced && listProcs.Any(x => x.AptNum==0)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"At least one of these procedures is not attached to an appointment. Send anyway?")) {
					return;
				}
			}
//todo: compare with: Bridges.ECW.AptNum, no need to generate PDF segment, pdfs only with eCW and this button not available with eCW integration
			MessageHL7 messageHL7=MessageConstructor.GenerateDFT(listProcs,EventTypeHL7.P03,Pd.Patient,Pd.ListPatients[0],aptNum,"treatment","PDF Segment");
			if(messageHL7==null) {
				MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
				return;
			}
			HL7Msg hl7Msg=new HL7Msg();
			hl7Msg.AptNum=aptNum;
			hl7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
			hl7Msg.MsgText=messageHL7.ToString();
			hl7Msg.PatNum=Pd.PatNum;
			HL7ProcAttach hl7ProcAttach=new HL7ProcAttach();
			hl7ProcAttach.HL7MsgNum=HL7Msgs.Insert(hl7Msg);
			foreach(Procedure proc in listProcs) {
				hl7ProcAttach.ProcNum=proc.ProcNum;
				HL7ProcAttaches.Insert(hl7ProcAttach);
			}
			if(ODBuild.IsDebug()) {
				MsgBox.Show(this,messageHL7.ToString());
			}
			else {
				MessageBox.Show(listProcs.Count+" "+(listProcs.Count==1?Lan.g(this,"procedure"):Lan.g(this,"procedures"))
					+" "+Lan.g(this,"queued to be sent by the HL7 service."));
			}
		}

		private void Tool_LabCase_Click() {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			LabCase labCase=new LabCase();
			labCase.PatNum=Pd.PatNum;
			labCase.ProvNum=Patients.GetProvNum(Pd.Patient);
			labCase.DateTimeCreated=MiscData.GetNowDateTime();
			LabCases.Insert(labCase);//it will be deleted inside the form if user clicks cancel.
			//We need the primary key in order to attach lab slip.
			using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
			formLabCaseEdit.LabCaseCur=labCase;
			formLabCaseEdit.IsNew=true;
			formLabCaseEdit.ShowDialog();
			//needs to always refresh due to complex ok/cancel
			ModuleSelected(Pd.PatNum);
		}

		private void Tool_Layout_Click() {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormSheetDefs formSheetDefs=new FormSheetDefs(SheetTypeEnum.ChartModule);
			formSheetDefs.ShowDialog();
			SecurityLogs.MakeLogEntry(Permissions.Setup,0,"Sheets");
			RefreshModuleScreen(false);//Update UI to reflect any changed dynamic SheetDefs.
			LayoutControls();
		}

		private void Tool_MedLab_Click() {
			FormMedLabs formMedLabs=new FormMedLabs();
			formMedLabs.PatCur=Pd.Patient;
			formMedLabs.Show();
		}

		private void Tool_Perio_Click() {
			if(IsPatientNull() || Pd.TableProgNotes==null) {
				MsgBox.Show(this,"Please select a patient and try again.");
				return;
			}
			List<Procedure> listProcedures=new List<Procedure>();
			DataTable table=Pd.TableProgNotes;
			//Find rows which are procedures (ProcNum!=0) and use the CodeNum and ToothNum columns to create a list of pseudo "Procedures".
			//We pull the procedures from the ProgNotes in memory so that we do not have to run a query to get the procedure data.
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["ProcNum"].ToString()=="0") {//jsalmon - this might need to be enhanced to consider proc status?
					continue;//Not a procedure row.
				}
				Procedure procTemp=new Procedure();
				procTemp.ToothNum=PIn.String(table.Rows[i]["ToothNum"].ToString());
				procTemp.CodeNum=PIn.Long(table.Rows[i]["CodeNum"].ToString());
				listProcedures.Add(procTemp);
			}
			if(ToothChartRelay.IsSparks3DPresent) {
				using FormPerio formPerio=new FormPerio(Pd.Patient,null);
				formPerio.ShowDialog();
			}
			else{
				using FormPerio formPerio=new FormPerio(Pd.Patient,listProcedures);
				formPerio.ShowDialog();
			}
		}

		private void Tool_Ortho_Click() {
			if(IsPatientNull()) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			//We store the current patNum because previously we've seen PatCur become null prior to ModuleSelected(...) being called somehow.
			long curPatNum=Pd.PatNum;
			using FormOrthoChart formOrthoChart=new FormOrthoChart(Pd.Patient);
			formOrthoChart.ShowDialog();
			ModuleSelected(curPatNum);
		}

		private void Tool_Rx_Click() {
			//This code is a copy of FormRxManage.butRxNew_Click().  Any changes to this code need to be changed there too.
			if(!Security.IsAuthorized(Permissions.RxCreate)) {
				return;
			}
			if(UsingEcwTightOrFull() && Bridges.ECW.UserId!=0) {
				VBbridges.Ecw.LoadRxForm((int)Bridges.ECW.UserId,Bridges.ECW.EcwConfigPath,(int)Bridges.ECW.AptNum);
				//refresh the right panel:
				try {
					string strAppServer=VBbridges.Ecw.GetAppServer((int)Bridges.ECW.UserId,Bridges.ECW.EcwConfigPath);
					webBrowserEcw.Url=new Uri("http://"+strAppServer+"/mobiledoc/jsp/dashboard/Overview.jsp?ptId="
							+Pd.PatNum.ToString()+"&panelName=overview&pnencid="
							+Bridges.ECW.AptNum.ToString()+"&context=progressnotes&TrUserId="+Bridges.ECW.UserId.ToString());
					labelECWerror.Visible=false;
				}
				catch(Exception ex) {
					webBrowserEcw.Url=null;
					labelECWerror.Text="Error: "+ex.Message;
					labelECWerror.Visible=true;
				}
			}
			else {
				using FormRxSelect formRxSelect=new FormRxSelect(Pd.Patient);
				formRxSelect.ShowDialog();
				if(formRxSelect.DialogResult!=DialogResult.OK) return;
				ModuleSelected(Pd.PatNum);
				SecurityLogs.MakeLogEntry(Permissions.RxCreate,Pd.PatNum,"Created prescription.");
			}
		}
		
		private void Tool_ToothChart_Click() {
			if(Programs.UsingOrion) {
				menuItemChartSave_Click(this,new EventArgs());
				return;
			}
			MsgBox.Show(this,"Please use dropdown list.");
			return;
		}

		///<summary>Returns an error message upon error.  Otherwise returns empty string.</summary>
		private void UpdateErxAccess(string npi,string userId,long clinicNum,string clinicId,string clinicKey,ErxOption erxOption) {
			ProviderErx provErxCur=ProviderErxs.GetOneForNpiAndOption(npi,erxOption);
			if(provErxCur==null) {
				//The provider is not yet part of the providererx table.  This extra refresh will only happen one time for each new provider.
				//First refresh cache to verify the provider was not added within the last signal interval.  Prevents duplicates for long signal intervals.
				ProviderErxs.RefreshCache();
				provErxCur=ProviderErxs.GetOneForNpiAndOption(npi,erxOption);
			}
			if(provErxCur==null) {
				provErxCur=new ProviderErx();
				provErxCur.PatNum=0;
				provErxCur.NationalProviderID=npi;
				if(erxOption==ErxOption.Legacy) {
					provErxCur.IsEnabled=ErxStatus.Disabled;
					if(PrefC.GetBool(PrefName.NewCropIsLegacy)) {
						provErxCur.IsEnabled=ErxStatus.Enabled;
					}
				}
				else {//DoseSpot
					provErxCur.IsEnabled=ErxStatus.PendingAccountId;
				}
				provErxCur.IsIdentifyProofed=false;
				provErxCur.IsSentToHq=false;
				provErxCur.ErxType=erxOption;
				provErxCur.UserId=userId;
				ProviderErxs.Insert(provErxCur);
				DataValid.SetInvalid(InvalidType.ProviderErxs);
			}
			//Make sure that there is a UserId associated to the providererx if the erx option utilized these ids (DoseSpot)
			ProviderErx provOld=provErxCur.Clone();
			provErxCur.UserId=userId;
			if(ProviderErxs.Update(provErxCur,provOld)) {
				DataValid.SetInvalid(InvalidType.ProviderErxs);
			}
			if(erxOption==ErxOption.DoseSpot) {
				if(DoseSpot.SyncClinicErxsWithHQ()) {
					DataValid.SetInvalid(InvalidType.ClinicErxs);
				}
			}
			bool isDistributorCustomer=false;
			if(PrefC.GetString(PrefName.NewCropPartnerName)!="" || PrefC.GetString(PrefName.NewCropPassword)!="") {
				isDistributorCustomer=true;
			}
			bool isOdUpdateAddress=false;
			if(PrefC.GetString(PrefName.UpdateServerAddress).ToLower().Contains("opendentalsoft.com") || 
				PrefC.GetString(PrefName.UpdateServerAddress).ToLower().Contains("open-dent.com"))
			{
				isOdUpdateAddress=true;
			}
			DateTime dateLastAccessMonth=DateTime.MinValue;
			if(erxOption==ErxOption.Legacy) {
				dateLastAccessMonth=PrefC.GetDate(PrefName.NewCropDateLastAccessCheck);
			}
			else {//DoseSpot
				dateLastAccessMonth=PrefC.GetDate(PrefName.DoseSpotDateLastAccessCheck);
			}
			dateLastAccessMonth=new DateTime(dateLastAccessMonth.Year,dateLastAccessMonth.Month,1);
			if(erxOption==ErxOption.Legacy && isDistributorCustomer && isOdUpdateAddress) {
				//The distributor forgot to change the "Server Address for Updates" inside of the Update Setup window for this customer.
				//Do not contact the OD web service.
			}
			else if(provErxCur.IsEnabled!=ErxStatus.Enabled //If prov is not yet enabled, always check with OD HQ to see if the prov has been enabled yet.
				|| (erxOption==ErxOption.Legacy && !PrefC.GetBool(PrefName.NewCropIsLegacy) && !provErxCur.IsIdentifyProofed)//If new prov is not yet identity proofed, send to OD HQ.
				|| !provErxCur.IsSentToHq//If prov has not been sent to OD HQ yet, always send to OD HQ so we can track our providers using eRx.
				|| dateLastAccessMonth<new DateTime(DateTime.Today.Year,DateTime.Today.Month,1))//If it has been over a month since sent to OD HQ, send.
			{
				//An OD customer, or a Distributor customer if the distributor has a custom web service for updates.
				//For distributors who implement this feature, you will be able to use FormErxAccess at your office to control individual provider access.
				//We compare the last access date by month above, because eRx charges are based on monthly usage.  Avoid extra charges for disabled providers.
				XmlWriterSettings xmlWSettings=new XmlWriterSettings();
				xmlWSettings.Indent=true;
				xmlWSettings.IndentChars=("    ");
				StringBuilder strBuild=new StringBuilder();
				using(XmlWriter xmlWriter=XmlWriter.Create(strBuild,xmlWSettings)) {
					xmlWriter.WriteStartElement("ErxAccessRequest");
						xmlWriter.WriteStartElement("RegistrationKey");
						xmlWriter.WriteString(PrefC.GetString(PrefName.RegistrationKey));
						xmlWriter.WriteEndElement();//End reg key
					List<ProviderErx> listUnsentProviders=ProviderErxs.GetAllUnsent();
					for(int i=0;i<listUnsentProviders.Count;i++) {
						xmlWriter.WriteStartElement("Prov");
							xmlWriter.WriteAttributeString("NPI",listUnsentProviders[i].NationalProviderID);
							xmlWriter.WriteAttributeString("IsEna",((int)listUnsentProviders[i].IsEnabled).ToString());
							xmlWriter.WriteAttributeString("ErxType",((int)listUnsentProviders[i].ErxType).ToString());
							xmlWriter.WriteAttributeString("UserId",listUnsentProviders[i].UserId);
						xmlWriter.WriteEndElement();//End Prov
					}
					xmlWriter.WriteEndElement();//End ErxAccessRequest
				}
#if DEBUG
				localhost.Service1 updateService=new localhost.Service1();
#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
					updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
				if(PrefC.GetString(PrefName.UpdateWebProxyAddress)!="") {
					IWebProxy proxy=new WebProxy(PrefC.GetString(PrefName.UpdateWebProxyAddress));
					ICredentials cred=new NetworkCredential(PrefC.GetString(PrefName.UpdateWebProxyUserName),PrefC.GetString(PrefName.UpdateWebProxyPassword));
					proxy.Credentials=cred;
					updateService.Proxy=proxy;
				}
				try {
					string result=updateService.GetErxAccess(strBuild.ToString());//may throw error
					XmlDocument xmlDoc=new XmlDocument();
					xmlDoc.LoadXml(result);
					XmlNodeList listXmlNodes=xmlDoc.SelectNodes("//Prov");
					List<ProviderErx> listProviderErxs=ProviderErxs.GetDeepCopy();
					bool[] arrayIsSentToHq=new bool[listProviderErxs.Count];
					bool isCacheRefreshNeeded=false;
					for(int i=0;i<listXmlNodes.Count;i++) {//Loop through providers.
						XmlNode nodeProv=listXmlNodes[i];
						string provNpi="";
						string provErxUserId="";
						ErxStatus provEnabledStatus=ErxStatus.Disabled;
						bool isProvIdp=false;
						bool isCurrentErxType=true;
						for(int j=0;j<nodeProv.Attributes.Count;j++) {//Loop through the attributes for the current provider.
							XmlAttribute xmlAttribute=nodeProv.Attributes[j];
							if(xmlAttribute.Name=="NPI") {
								provNpi=Regex.Replace(xmlAttribute.Value,"[^0-9]*","");//NPI with all non-numeric characters removed.
								if(provNpi.Length!=10) {
									provNpi="";
									break;//Invalid NPI
								}
							}
							else if(xmlAttribute.Name=="IsEna") {
								provEnabledStatus=PIn.Enum<ErxStatus>(xmlAttribute.Value,false,ErxStatus.Undefined);
							}
							else if(xmlAttribute.Name=="IsIdp" && xmlAttribute.Value=="1") {
								isProvIdp=true;
							}
							else if(xmlAttribute.Name=="ErxType" && PIn.Enum<ErxOption>(PIn.Int(xmlAttribute.Value))!=erxOption) {
								isCurrentErxType=false;
							}
							else if(xmlAttribute.Name=="UserId") {
								provErxUserId=xmlAttribute.Value;
							}
						}
						if(!isCurrentErxType) {//We don't want to change records for DoseSpot if the user is using NewCrop and vice versa.
							continue;
						}
						if(provNpi=="") {
							continue;
						}
						ProviderErx oldProvErx=ProviderErxs.GetOneForNpiAndOption(provNpi,erxOption);
						if(oldProvErx==null) {
							continue;
						}
						arrayIsSentToHq[listProviderErxs.Select(x => x.ProviderErxNum).ToList().IndexOf(oldProvErx.ProviderErxNum)]=true;
						ProviderErx provErx=oldProvErx.Clone();
						provErx.IsEnabled=provEnabledStatus;
						provErx.IsIdentifyProofed=isProvIdp;
						provErx.IsSentToHq=true;
						provErx.UserId=provErxUserId;
						//Dont need to set the ErxType here because it's not something that can be changed by HQ.
						if(ProviderErxs.Update(provErx,oldProvErx)) {
							isCacheRefreshNeeded=true;
						}
					}
					//Any proverxs which are in the local customer database but not sent to HQ, flag as unsent.
					//Providererx records were being deleted from HQ due to a sync issue at HQ.
					for(int i=0;i<arrayIsSentToHq.Length;i++) {
						if(arrayIsSentToHq[i]) {
							continue;
						}
						ProviderErx provErx=listProviderErxs[i];
						ProviderErx oldProvErx=provErx.Clone();
						provErx.IsSentToHq=false;
						if(ProviderErxs.Update(provErx,oldProvErx)) {
							isCacheRefreshNeeded=true;
						}	
					}
					if(isCacheRefreshNeeded) {
						DataValid.SetInvalid(InvalidType.ProviderErxs);
					}
					if(erxOption==ErxOption.Legacy) {
						if(Prefs.UpdateDateT(PrefName.NewCropDateLastAccessCheck,DateTime.Today)) {
							DataValid.SetInvalid(InvalidType.Prefs);
						}
					}
					else {//DoseSpot
						if(Prefs.UpdateDateT(PrefName.DoseSpotDateLastAccessCheck,DateTime.Today)) {
							DataValid.SetInvalid(InvalidType.Prefs);
						}
					}
				}
				catch(Exception ex) {
					ex.DoNothing();
					//Failed to contact server and/or update provider IsEnabled statuses.  We will simply use what we already know in the local database.
				}
			}
		}

		///<summary>This reduces the number of places where Programs.UsingEcwTight() is called.  This helps with organization.  All calls from ContrChart must pass through here.  They also must have been checked to not involve the Orion bridge or layout logic.</summary>
		private bool UsingEcwTight() {
			return Programs.UsingEcwTightMode();
		}

		///<summary>This reduces the number of places where Programs.UsingEcwTightOrFull() is called.  This helps with organization.  All calls from ContrChart must pass through here.  They also must have been checked to not involve the Orion bridge or layout logic.</summary>
		private bool UsingEcwTightOrFull() {
			return Programs.UsingEcwTightOrFullMode();
		}
		#endregion Methods - Private - General

		#region Methods - Private - SheetLayout
		///<summary>Returns a dictionary such that the key is a sheetFieldDef.FieldName and the value is a corresponding control from this instance of ContrChart. Dictionary values can be null if control is not matched or it is matched to an HQ only control and not at HQ.</summary>
		private Dictionary<string,Control> LayoutSheet_GetControlDict() {
			SheetFieldLayoutMode layoutModeCur=LayoutSheet_GetMode();
			Dictionary<string,Control> dictControls=new Dictionary<string,Control>();
			//Use internal list because it implements all sheetFieldDefs.  We expect the def to always match the controls in the Chart.
			List<SheetFieldDef> listSheetFieldDefsAll=SheetsInternal.GetSheetDef(SheetInternalType.ChartModule).SheetFieldDefs;
			for(int i=0;i<listSheetFieldDefsAll.Count;i++){
				if(listSheetFieldDefsAll[i].LayoutMode!=layoutModeCur) {
					continue;
				}
				Control control=null;
				bool isHqOrDistibutorControl=false;
				switch(listSheetFieldDefsAll[i].FieldName) {
					#region Set control based on matching FieldName
					case "PatientInfo":
						control=this.gridPtInfo;
						if(control.Parent!=this) {//Depending on the mode this control can be moved into tabProc
							LayoutManager.Add(control,this);
						}
						break;
					case "ProgressNotes":
						control=this.panelGridProg;
						break;
					case "ChartModuleTabs":
						control=this.tabProc;
						break;
					case "TreatmentNotes":
						control=textTreatmentNotes;
						break;
					case "TrackToothProcDates":
						control=panelToothTrackBar;
						break;
					case "toothChart":
						if(odInternalCustomerGrids.Visible) {//Replace toothchart with HQ bug submission control
							control=odInternalCustomerGrids;
							isHqOrDistibutorControl=true;
						}
						else if(ToothChartRelay.IsSparks3DPresent) {
							control=toothChart;
						}
						else {
							control=toothChartWrapper;
						}
						break;
					case "PanelEcw":
						control=panelEcw;
						break;
					case "ButtonErxAccess":
						control=butErxAccess;
						isHqOrDistibutorControl=true;
						break;
					case "ButtonPhoneNums":
						control=butPhoneNums;
						isHqOrDistibutorControl=true;
						break;
					case "ButtonForeignKey":
						control=butForeignKey;
						isHqOrDistibutorControl=true;
						break;
					case "ButtonUSAKey":
						control=butAddKey;
						isHqOrDistibutorControl=true;
						break;
					//These 4 FieldNames can still be found in the DB and in xml, but they are deprecated:
					/*case "ButtonNewTP":
						control=butNewTP;
						break;
					case "Procedures":
						control=gridTpProcs;
						break;
					case "TreatmentPlans":
						control=gridTreatPlans;
						break;
					case "SetPriorityListBox":
						control=panelTPpriority;
						break;*/
					#endregion
					default:
						//Control not matched
						break;
				}
				if(isHqOrDistibutorControl && !(PrefC.IsODHQ || PrefC.GetBool(PrefName.DistributorKey))) {
					control?.Dispose();
					control=null;
				}
				dictControls.Add(listSheetFieldDefsAll[i].FieldName,control);//Can add null
			}
			return dictControls;
		}

		///<summary>This mimics FormSheetDefEdit.InitLayoutModes() logic.</summary>
		private SheetFieldLayoutMode LayoutSheet_GetMode() {
			SheetFieldLayoutMode sheetFieldLayoutMode;
			if(UsingEcwTightOrFull()) {
				//if(checkTreatPlans.Checked) {
				//	sheetFieldLayoutMode=SheetFieldLayoutMode.EcwTreatPlan;
				//}
				//else {
				sheetFieldLayoutMode=SheetFieldLayoutMode.Ecw;
				//}
				//tabProc.TabPages.Remove(tabPatInfo);
			}
			else if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				//if(checkTreatPlans.Checked) {
				//	sheetFieldLayoutMode=SheetFieldLayoutMode.MedicalPracticeTreatPlan;
				//}
				//else {
				sheetFieldLayoutMode=SheetFieldLayoutMode.MedicalPractice;
				//}
			}
			//else if(checkTreatPlans.Checked) {
			//	sheetFieldLayoutMode=SheetFieldLayoutMode.TreatPlan;
			//}
			else {
				sheetFieldLayoutMode=SheetFieldLayoutMode.Default;
				//tabProc.TabPages.Remove(tabPatInfo);
			}
			return sheetFieldLayoutMode;
		}

		///<summary>Does a fresh layout of the controls, based on the Sheet, the ChartView, and TP mode.</summary>
		public void LayoutControls() {
			SheetFieldLayoutMode sheetFieldLayoutMode=LayoutSheet_GetMode();
			Dictionary<string,Control> dictionaryStringControl=LayoutSheet_GetControlDict();
			_sheetLayoutController.ReloadSheetLayout(sheetFieldLayoutMode,dictionaryStringControl);
			for(int i=0;i<dictionaryStringControl.Count;i++){
				LayoutManager.Synch(dictionaryStringControl.Values.ElementAt(i));//The controls were just moved around programmatically by the sheetLayoutController.
				//This will also pick up any change in anchor and move that over to the LayoutManager
			}
			if(checkShowOrtho.Checked){
				panelGridProg.Visible=false;
				tabOrthoCategories.Visible=true;
				LayoutManager.MoveLocation(tabOrthoCategories,new Point(panelGridProg.Left,panelGridProg.Top));
				LayoutManager.MoveSize(tabOrthoCategories,new Size(this.Width-panelGridProg.Left,LayoutManager.Scale(23)));
				gridOrtho.Visible=true;
				LayoutManager.MoveLocation(gridOrtho,new Point(panelGridProg.Left,tabOrthoCategories.Bottom));
				LayoutManager.MoveSize(gridOrtho,new Size(this.Width-panelGridProg.Left,panelGridProg.Bottom-gridOrtho.Top));
			}
			else if(checkTreatPlans.Checked){
				panelGridProg.Visible=false;
				//These 4 controls are not visible to user during layout in SheetDef because they are no longer SheetFieldDefs, but they are needed if in TP mode.
				//It might work a little better to put them all in a panel, and then just plop the panel in place, but this works fine, too.
				gridTreatPlans.Visible=true;
				LayoutManager.MoveLocation(gridTreatPlans,new Point(panelGridProg.Left,panelGridProg.Top));//412,263
				LayoutManager.MoveSize(gridTreatPlans,LayoutManager.ScaleSize(new Size(435,120)));
				butNewTP.Visible=true;
				LayoutManager.MoveLocation(butNewTP,new Point(gridTreatPlans.Right+LayoutManager.Scale(5),gridTreatPlans.Top-LayoutManager.Scale(1)));//852,262
				LayoutManager.MoveSize(butNewTP,LayoutManager.ScaleSize(new Size(77,23)));
				gridTpProcs.Visible=true;
				LayoutManager.MoveLocation(gridTpProcs,new Point(gridTreatPlans.Left,gridTreatPlans.Bottom+LayoutManager.Scale(2)));//412,385
				LayoutManager.MoveSize(gridTpProcs,new Size(LayoutManager.Scale(460),panelGridProg.Bottom-gridTreatPlans.Bottom-LayoutManager.Scale(2)));
				panelTPpriority.Visible=true;
				LayoutManager.MoveLocation(panelTPpriority,new Point(gridTpProcs.Right+LayoutManager.Scale(3),gridTpProcs.Top));//875,385
				LayoutManager.MoveSize(panelTPpriority,new Size(LayoutManager.Scale(77),panelGridProg.Bottom-gridTpProcs.Top));
			}
			else{
				panelGridProg.Visible=true;
				gridTreatPlans.Visible=false;
				butNewTP.Visible=false;
				gridTpProcs.Visible=false;
				panelTPpriority.Visible=false;
			}
		}
		#endregion Methods - Private - SheetLayout

		#region Methods - Private - Tab EnterTx
		///<summary>Sets many fields for a new procedure, then displays it for editing before inserting it into the db.  No need to worry about ProcOld because it's an insert, not an update.  AddProcedure and AddQuick both call AddProcHelper, where most of the logic for setting the fields for a new procedure is located.</summary>
		private void AddProcedure(Procedure procedure,List<Fee> listFees) {
			Pd.FillIfNeeded(EnumPdTable.Procedure);
			if(!AddProcHelper(procedure,listFees)) { //Procedure was deleted.
				return;
			}
			//Get from DB to get updated timestamps for permission checks and to initialize nullable variables, like strings, before filling FormProcEdit.
			bool isAdditionalProc=procedure.IsAdditional;
			procedure=Procedures.GetOneProc(procedure.ProcNum,true);//This breaks the reference to the original Procedure object in the calling method.
			procedure.IsAdditional=isAdditionalProc;
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(Pd.PatNum);
			List<Procedure> listProceduresTP=Pd.ListProcedures.FindAll(x=>x.ProcStatus==ProcStat.TP);
			for(int i=0;i<listProceduresTP.Count;i++) {
				if(listProceduresTP[i].ProcNum==procedure.ProcNum) {
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProceduresTP[i].ProcNum,listProceduresTP[i].CodeNum));
			}
			using FormProcEdit formProcEdit=new FormProcEdit(procedure,Pd.Patient.Copy(),Pd.Family,listPatToothInitials:Pd.ListToothInitials);
			formProcEdit.IsNew=true;
			formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
			formProcEdit.ListClaimProcHists=Pd.ListClaimProcHists;
			formProcEdit.ShowDialog();
			if(formProcEdit.DialogResult==DialogResult.Cancel) {
				try {
					Procedures.Delete(procedure.ProcNum);//also deletes the claimprocs
					Pd.ListProcedures.RemoveAll(x=>x.ProcNum==procedure.ProcNum);
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
				}
				return;//cancelled insert
			}
			if(_procStatusNew==ProcStat.C) {//User didn't cancel (delete) in FormProcEdit
				Encounters.InsertDefaultEncounter(procedure.PatNum,procedure.ProvNum,procedure.ProcDate);//Auto-insert default encounter
			}
			//The status may have been edited in formProcEdit, should rely on ProcCur.ProcStatus
			if(procedure.ProcStatus==ProcStat.C) {
				ProcedureL.AfterProcsSetComplete(new List<Procedure>() { procedure });
			}
			if(!UsingEcwTightOrFull() //no need to synch with eCW
				&& !Programs.UsingOrion //no need to synch with Orion
				&& new[] { ProcStat.C,ProcStat.EC,ProcStat.EO }.Contains(_procStatusNew)) //only run Recalls for completed, existing current, or existing other
			{
				Recalls.Synch(Pd.PatNum);
			}
			logComplCreate(procedure);
		}

		///<summary>Called by AddProcedure and AddQuick.  Both methods contained versions of this code and a bug was introduced in version 15.3 because the order of the regions changed in the two methods and no longer matched.  This helper method prevents bugs caused by trying to keep duplicate code blocks synced.</summary>
		private bool AddProcHelper(Procedure procedure,List<Fee> listFees) {
			if(ProcedureCodes.AreAnyProcCodesHidden(procedure.CodeNum)) {
				MessageBox.Show(this,Lan.g(this,"Cannot add procedure because it is in a hidden category")+$": {ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode}");
				return false;
			}
			procedure.PatNum=Pd.PatNum;
			//ProcCur.CodeNum=ProcedureCodes.GetProcCode(ProcCur.OldCode).CodeNum;//already set
			if(textDate.Text=="" || !textDate.IsValid()) {
				procedure.DateTP=DateTime.Today;
			}
			else {
				procedure.DateTP=PIn.Date(textDate.Text);
			}
			if(_procStatusNew!=ProcStat.EO) {
				procedure.ProcDate=procedure.DateTP;
			}
			ProcedureCode procCodeCur=ProcedureCodes.GetProcCode(procedure.CodeNum);
			#region ProvNum
			//This strategy for assigning procnum is consistent here, in the Chart Module, but it doesn't seem to be used anywhere else.
			//For example, in the Appt Module, for recall, etc, the proc is simply whatever appointment we are in.
			procedure.ProvNum=procCodeCur.ProvNumDefault;//use proc default prov if set
			if(procedure.ProvNum==0) {//no proc default prov set, check for appt prov, then use pri prov
				long provPri=Pd.Patient.PriProv;
				long provSec=Pd.Patient.SecProv;
				List<Appointment> listAppointmentsToday=Pd.ListAppointments.FindAll(x => x.AptDateTime.Date==DateTime.Today && x.AptStatus!=ApptStatus.Planned);
				if(listAppointmentsToday.Count>0) {
					provPri=listAppointmentsToday[0].ProvNum;
					provSec=listAppointmentsToday[0].ProvHyg;
				}
				if(Providers.GetProv(provPri).IsHidden) {
					//If the Patient's Primary Provider is hidden, use the patient's clinic's default provider, or practice default provider
					if(PrefC.HasClinicsEnabled){
						provPri=Providers.GetDefaultProvider(Pd.Patient.ClinicNum).ProvNum;
					}
					else {
						provPri=Providers.GetDefaultProvider().ProvNum;
					}
				}
				if(procCodeCur.IsHygiene && provSec!=0 && !Providers.GetProv(provSec).IsHidden) {//Do not assign Sec. Provider's to Procedures when hidden
					procedure.ProvNum=provSec;
				}
				else {
					procedure.ProvNum=provPri;
				}
			}
			#endregion ProvNum
			if(_procStatusNew==ProcStat.C) {
				if(procedure.ProcDate.Date>DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
					MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
					return false;
				}
				Procedures.SetOrthoProcComplete(procedure,procCodeCur); //does nothing if not an ortho proc
			}
			#region Note
			if(_procStatusNew==ProcStat.C || _procStatusNew==ProcStat.TP) {
				string procNoteDefault=ProcCodeNotes.GetNote(procedure.ProvNum,procedure.CodeNum,_procStatusNew);
				if(procedure.Note!="" && procNoteDefault!="") {
					procedure.Note+="\r\n"; //add a new line if there was already a ProcNote on the procedure.
				}
				procedure.Note+=procNoteDefault;
				if(!PrefC.GetBool(PrefName.ProcPromptForAutoNote)) {
					//Users do not want to be prompted for auto notes, so remove them all from the procedure note.
					procedure.Note=Regex.Replace(procedure.Note,@"\[\[.+?\]\]","");
				}
			}
			else {
				procedure.Note="";
			}
			#endregion
			procedure.ClinicNum=Pd.Patient.ClinicNum;
			if(_procStatusNew==ProcStat.R || _procStatusNew==ProcStat.EO || _procStatusNew==ProcStat.EC) {
				procedure.ProcFee=0;
			}
			else {
				procedure.MedicalCode=procCodeCur.MedicalCode;
				procedure.ProcFee=Procedures.GetProcFee(Pd.Patient,Pd.ListPatPlans,Pd.ListInsSubs,Pd.ListInsPlans,procedure.CodeNum,procedure.ProvNum,procedure.ClinicNum,
					procedure.MedicalCode,listFees:listFees);
			}
			if(_procStatusNew==ProcStat.C 
				&& !Security.IsAuthorized(Permissions.ProcComplCreate,procedure.ProcDate,procedure.CodeNum,procedure.ProcFee)) 
			{			
				return false;
			}
			//surf
			//toothnum
			if(comboPriority.SelectedIndex==0) {
				procedure.Priority=0;
			}
			else {
				procedure.Priority=comboPriority.GetSelectedDefNum();
			}
			procedure.ProcStatus=_procStatusNew;
			if(listDx.SelectedIndex!=-1) {
				procedure.Dx=listDx.GetSelected<Def>().DefNum;
			}
			if(comboPrognosis.SelectedIndex==0) {
				procedure.Prognosis=0;
			}
			else {
				procedure.Prognosis=comboPrognosis.GetSelectedDefNum();
			}
			procedure.BaseUnits=procCodeCur.BaseUnits;
			procedure.SiteNum=Pd.Patient.SiteNum;
			procedure.RevCode=procCodeCur.RevenueCodeDefault;
			procedure.DiagnosticCode=PrefC.GetString(PrefName.ICD9DefaultForNewProcs);
			procedure.PlaceService=Clinics.GetPlaceService(procedure.ClinicNum);
			if(Userods.IsUserCpoe(Security.CurUser)) {
				//This procedure is considered CPOE because the provider is the one that has added it.
				procedure.IsCpoe=true;
			}
			//Find out if we are going to link the procedure to an ortho case.
			OrthoCaseProcLinkingData orthoCaseProcLinkingData=new OrthoCaseProcLinkingData(procedure.PatNum);
			procedure.ProcNum=Procedures.Insert(procedure,skipDiscountPlanAdjustment:orthoCaseProcLinkingData.CanProcLinkToOrthoCase(procedure));
			//we already did Pd.FillIfNeeded(EnumPdTable.Procedure); in both of the places where this method is called.
			Pd.ListProcedures.Add(procedure);//Just in case a data refresh won't been executed
			OrthoProcLink orthoProcLink=OrthoProcLinks.TryLinkProcForActiveOrthoCaseAndUpdate(orthoCaseProcLinkingData,procedure);
			if(_listChartedProcs!=null) {
				_listChartedProcs.Add(procedure);
			}
			if(_procStatusNew==ProcStat.TP) {
				AttachProcToTPs(procedure);
			}
			if((procedure.ProcStatus==ProcStat.C || procedure.ProcStatus==ProcStat.EC || procedure.ProcStatus==ProcStat.EO)
				&& procCodeCur.PaintType==ToothPaintingType.Extraction) {
				//if an extraction, then mark previous procs hidden
				//Procedures.SetHideGraphical(ProcCur);//might not matter anymore
				ToothInitials.SetValue(Pd.PatNum,procedure.ToothNum,ToothInitialType.Missing);
			}
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			List<ClaimProcHist> listClaimProcHistsLoop=new List<ClaimProcHist>();
			List<ClaimProc> listClaimProc=ClaimProcs.RefreshForTP(Pd.PatNum);
			List<Procedure> listProcedures=Pd.ListProcedures.FindAll(x=>x.ProcStatus==ProcStat.TP);
			for(int i=0;i<listProcedures.Count;i++) {
				if(listProcedures[i].ProcNum==procedure.ProcNum) {								
					break;
				}
				listClaimProcHistsLoop.AddRange(ClaimProcs.GetHistForProc(listClaimProc,listProcedures[i].ProcNum,listProcedures[i].CodeNum));
			}
			Procedures.ComputeEstimates(procedure,Pd.PatNum,ref listClaimProcs,true,Pd.ListInsPlans,Pd.ListPatPlans,Pd.ListBenefits,
				Pd.ListClaimProcHists,listClaimProcHistsLoop,true,Pd.Patient.Age,Pd.ListInsSubs,null,false,false,_listSubstitutionLinks,false,
				listFees,orthoProcLink:orthoProcLink,orthoCase:orthoCaseProcLinkingData.ActiveOrthoCase,orthoSchedule:orthoCaseProcLinkingData.OrthoSchedule
				,listOrthoProcLinksForOrthoCase:orthoCaseProcLinkingData.ListProcLinksForCase);
			listClaimProcs=ClaimProcs.GetForProc(ClaimProcs.Refresh(Pd.PatNum),procedure.ProcNum);
			long verifyCode;
			bool isMandibular=false;
			if(!string.IsNullOrEmpty(procedure.ToothRange)) {
				isMandibular=procedure.ToothRange.Split(',').Any(x => !Tooth.IsMaxillary(x));
			}
			if(AutoCodeItems.ShouldPromptForCodeChange(procedure,procCodeCur,Pd.Patient,isMandibular,listClaimProcs,out verifyCode)) {
				using FormAutoCodeLessIntrusive FormACLI=new FormAutoCodeLessIntrusive(Pd.Patient,procedure,procCodeCur,verifyCode,Pd.ListPatPlans,Pd.ListInsSubs,Pd.ListInsPlans,
					Pd.ListBenefits,listClaimProcs);
				if(FormACLI.ShowDialog()!=DialogResult.OK
					&& PrefC.GetBool(PrefName.ProcEditRequireAutoCodes))
				{
					using FormProcEdit formProcEdit=new FormProcEdit(procedure,Pd.Patient,Pd.Family,listPatToothInitials:Pd.ListToothInitials);//ProcCur may be modified in this form due to passing by reference. Intentional.
					formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoop;
					formProcEdit.ListClaimProcHists=Pd.ListClaimProcHists;
					formProcEdit.ShowDialog();
					if(formProcEdit.DialogResult!=DialogResult.OK) {
						try {
							Procedures.Delete(procedure.ProcNum,true);//also deletes the claimprocs
							Pd.ListProcedures.RemoveAll(x=>x.ProcNum==procedure.ProcNum);
						}
						catch(Exception ex) {
							MessageBox.Show(ex.Message);
						}
						return false;
					}					
				}
			}
			return true;
		}

		///<summary>Add a procedure for all selected quadrants.</summary>
		private void AddQuadProcs(Procedure procCur,List<Fee> listFees) {
			if(_toothChartRelay.SelectedTeeth.Count>0) {
				List<string> listSelectedQuads=Tooth.GetQuadsForTeeth(_toothChartRelay.SelectedTeeth);
				foreach(string quad in listSelectedQuads) {
					Procedure proc=procCur.Copy();
					proc.Surf=quad;
					AddQuick(proc,listFees);
				}
			}
			else {
				//Don't know what to set the surface to, so we have the user enter it.
				AddProcedure(procCur,listFees);
			}
		}

		///<summary>Add a procedure for all selected quadrants and maintains the selected tooth range for each quadrant.</summary>
		private void AddQuadProcsWithToothRange(Procedure procCur,List<Fee> listFees) {
			if(_toothChartRelay.SelectedTeeth.Count>0) {
				List<string> listSelectedQuads=Tooth.GetQuadsForTeeth(_toothChartRelay.SelectedTeeth);
				List<string> listTeethSelected=_toothChartRelay.SelectedTeeth;
				List<string> listQuadTeeth;
				List<string> listFinalToothRange=new List<string>();
				for(int i=0;i<listSelectedQuads.Count();i++) {
					Procedure proc=procCur.Copy();
					proc.Surf=listSelectedQuads[i];
					if(listSelectedQuads[i]=="UR") {
						listQuadTeeth=new List<string>{"1","2","3","4","5","6","7","8"};
						listFinalToothRange=listTeethSelected.Intersect(listQuadTeeth).ToList();
					}
					if(listSelectedQuads[i]=="UL") {
						listQuadTeeth=new List<string>{"9","10","11","12","13","14","15","16"};
						listFinalToothRange=listTeethSelected.Intersect(listQuadTeeth).ToList();
					}
					if(listSelectedQuads[i]=="LL") {
						listQuadTeeth=new List<string>{"17","18","19","20","21","22","23","24"};
						listFinalToothRange=listTeethSelected.Intersect(listQuadTeeth).ToList();
					}
					if(listSelectedQuads[i]=="LR") {
						listQuadTeeth=new List<string>{"25","26","27","28","29","30","31","32"};
						listFinalToothRange=listTeethSelected.Intersect(listQuadTeeth).ToList();
					}
					proc.ToothRange="";
					for(int b=0;b<listFinalToothRange.Count;b++) {
						if(b!=0) {
							proc.ToothRange+=",";
						}
						proc.ToothRange+=listFinalToothRange[b];
					}
					AddQuick(proc,listFees);
				}
			}
			else {
				AddProcedure(procCur,listFees); //Don't know what to set the surface to, so we have the user enter it.
			}
		}

		///<summary>Add a procedure for all selected arches and maintains the selected tooth range for each arch.</summary>
		private void AddArchProcsWithToothRange(Procedure procCur,List<Fee> listFees) {
			if(_toothChartRelay.SelectedTeeth.Count>0) {
				List<string> listSelectedArches=Tooth.GetArchesForTeeth(_toothChartRelay.SelectedTeeth);
				List<string> listTeethSelected=_toothChartRelay.SelectedTeeth;
				List<string> listArchTeeth;
				List<string> listFinalToothRange=new List<string>();
				for(int i=0;i<listSelectedArches.Count;i++) {
					Procedure proc=procCur.Copy();
					proc.Surf=listSelectedArches[i];
					if(listSelectedArches[i]=="U") {
						listArchTeeth=new List<string>{"1","2","3","4","5","6","7","8","9","10","11","12","13","14","15","16"};
						listFinalToothRange=listTeethSelected.Intersect(listArchTeeth).ToList();
					}
					if(listSelectedArches[i]=="L") {
						listArchTeeth=new List<string>{"17","18","19","20","21","22","23","24","25","26","27","28","29","30","31","32"};
						listFinalToothRange=listTeethSelected.Intersect(listArchTeeth).ToList();
					}
					proc.ToothRange="";
					for(int b=0;b<listFinalToothRange.Count;b++) {
						if(b!=0) {
							proc.ToothRange+=",";
						}
						proc.ToothRange+=listFinalToothRange[b];
					}
					AddQuick(proc,listFees);
				}
			}
			else {
				AddProcedure(procCur,listFees); //Don't know what to set the surface to, so we have the user enter it.
			}
		}

		///<summary>No user dialog is shown.  This only works for some kinds of procedures.  Set the codeNum first. AddProcedure and AddQuick both call AddProcHelper, where most of the logic for setting the fields for a new procedure is located. No validation is done before adding the procedure so check all permissions and such prior to calling this method.</summary>
		private void AddQuick(Procedure ProcCur,List<Fee> listFees) {
			Plugins.HookAddCode(this,"ContrChart.AddQuick_begin",ProcCur,listFees);
			Pd.FillIfNeeded(EnumPdTable.Procedure);
			if(!AddProcHelper(ProcCur,listFees)) { //Procedure was deleted.
				return;
			}
			if(ProcCur.ProcStatus==ProcStat.C) {//either not using Orion, or user didn't cancel (delete) in FormProcEdit
				Encounters.InsertDefaultEncounter(ProcCur.PatNum,ProcCur.ProvNum,ProcCur.ProcDate);//Auto-insert default encounter
				ProcedureL.AfterProcsSetComplete(new List<Procedure>() { ProcCur });
			}
			if(!UsingEcwTightOrFull() && !Programs.UsingOrion //no need to synch with eCW or Orion
				&& new[] { ProcStat.C,ProcStat.EC,ProcStat.EO }.Contains(_procStatusNew)) //only run Recalls for completed, existing current, or existing other
			{
				Recalls.Synch(Pd.PatNum);
			}
			logComplCreate(ProcCur);
		}

		private void ClearButtons() {
			//unfortunately, these colors no longer show since the XP button style was introduced.
			butM.BackColor=Color.FromName("Control"); ;
			butOI.BackColor=Color.FromName("Control");
			butD.BackColor=Color.FromName("Control");
			butL.BackColor=Color.FromName("Control");
			butBF.BackColor=Color.FromName("Control");
			butV.BackColor=Color.FromName("Control");
			textSurf.Text="";
			listDx.SelectedIndex=-1;
			//listProcButtons.SelectedIndex=-1;
			listViewButtons.SelectedIndices.Clear();
			textProcCode.Text=Lan.g(this,"Type Proc Code");
		}

		private void DeleteRows() {
			if(gridProg.SelectedIndices.Length==0) {
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			if(MessageBox.Show(Lan.g(this,"Delete Selected Item(s)?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK) {
				return;
			}
			int skippedSecurity=0;
			int skippedRxSecurity=0;
			int skippedC=0;
			int skippedAttached=0;
			int skippedLinkedToOrthoCase=0;
			int skippedAttachedToPreauth=0;
			List<DataRow> listSelectedRows=gridProg.SelectedIndices.Where(x => x>-1 && x<gridProg.ListGridRows.Count)
				.Select(x => (DataRow)gridProg.ListGridRows[x].Tag).ToList();
			if(PrefC.GetBool(PrefName.ApptsRequireProc)) {
				List<long> listProcNumsSelected=listSelectedRows
					.Where(x => PIn.Long(x["ProcNum"].ToString())!=0)
					.Select(x => PIn.Long(x["ProcNum"].ToString()))
					.ToList();
				List<Procedure> listProcsSelected=Procedures.GetManyProc(listProcNumsSelected,false);
				bool areApptsGoingToBeEmpty=Appointments.AreApptsGoingToBeEmpty(listProcsSelected);
				if(areApptsGoingToBeEmpty && !MsgBox.Show(this,MsgBoxButtons.YesNo,
					"One or more procedures being deleted are attached to appointments. If you permanently delete all selected procedure(s), it will leave one or more appointments empty. Continue?"))
				{
					return;
				}
			}
			OrthoProcLink orthoProcLink=null;
			Dictionary<long,OrthoProcLink> dictOrthoProcLinks=OrthoProcLinks.GetManyForProcs(listSelectedRows.Where(x => PIn.Long(x["ProcNum"].ToString())!=0)
				.ToList().Select(y => PIn.Long(y["ProcNum"].ToString())).ToList()).ToDictionary(z => z.ProcNum,z => z);
			foreach(DataRow row in listSelectedRows) {
				long procNum=PIn.Long(row["ProcNum"].ToString());
				if(procNum!=0) {
					dictOrthoProcLinks.TryGetValue(procNum,out orthoProcLink);
				}
				if(!CanDeleteRow(row,doCheckDb:true,ref skippedSecurity,ref skippedRxSecurity,ref skippedC,ref skippedAttached,ref skippedLinkedToOrthoCase,
					ref skippedAttachedToPreauth,orthoProcLink)) 
				{
					continue;
				}
				if(row["ProcNum"].ToString()!="0") {
					try {
						Procedures.Delete(PIn.Long(row["ProcNum"].ToString()));//also deletes the claimprocs
						CanadianLabFeeHelper(Procedures.GetOneProc(PIn.Long(row["ProcNum"].ToString()),false).ProcNumLab);
						SecurityLogs.MakeLogEntry(Permissions.ProcDelete,Pd.PatNum,row["ProcCode"].ToString()+" ("+row["procStatus"]+"), "
							+PIn.Double(row["procFee"].ToString()).ToString("c"));
					}
					catch(Exception ex) {
						MessageBox.Show(ex.Message);
						//continue;//continues to next in loop from here implicitly, no explicit call to continue necessary
					}
				}
				else if(row["RxNum"].ToString()!="0") {
					RxPat rxPat=RxPats.GetRx(PIn.Long(row["RxNum"].ToString()));
					SecurityLogs.MakeLogEntry(Permissions.RxEdit,Pd.PatNum,"FROM("+rxPat.RxDate.ToShortDateString()+","+rxPat.Drug+","+rxPat.ProvNum+","
						+rxPat.Disp+","+rxPat.Refills+")"+"\r\nTO('deleted')",rxPat.RxNum,rxPat.DateTStamp);
					RxPats.Delete(PIn.Long(row["RxNum"].ToString()));
				}
			}
			Recalls.Synch(Pd.PatNum);
			if(skippedC>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete completed procedures from here.")+"\r"
					+skippedC.ToString()+" "+Lan.g(this,"item(s) skipped."));
			}
			if(skippedLinkedToOrthoCase>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete procedures that are linked to ortho cases. "+
					"Detach the procedure or delete the ortho case first.")+"\r"+skippedLinkedToOrthoCase.ToString()+" "+Lan.g(this,"item(s) skipped."));
			}
			if(skippedSecurity>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete procedures due to security.")+"\r"
					+skippedSecurity.ToString()+" "+Lan.g(this,"item(s) skipped."));
			}
			if(skippedRxSecurity>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete Rx due to security.")+"\r"
					+skippedRxSecurity.ToString()+" "+Lan.g(this,"item(s) skipped."));
			}
			if(skippedAttached>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete TP procedures with payments attached.")+"\r"
					+skippedAttached.ToString()+" "+Lan.g(this,"item(s) skipped. "));
			}
			if(skippedAttachedToPreauth>0) {
				MessageBox.Show(Lan.g(this,"Not allowed to delete the last TP procedure attached to a preauth")+"\r"
					+skippedAttachedToPreauth.ToString()+" "+Lan.g(this,"item(s) skipped. "));
			}
			ModuleSelected(Pd.PatNum);
		}

		private void EnterTypedCode() {
			//orionProcNum=0;
			if(_procStatusNew==ProcStat.C) {
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show(this,"Set the procedure complete by setting the appointment complete.  "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
					return;
				}
				//We will call this method again with the real ProcFee once we know it.
				if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text),ProcedureCodes.GetCodeNum(textProcCode.Text),0)) {
					return;
				}
			}
			if(CultureInfo.CurrentCulture.Name=="en-US" && Regex.IsMatch(textProcCode.Text,@"^\d{4}$")) {//if exactly 4 digits
				if(!ProcedureCodes.GetContainsKey(textProcCode.Text)) {//4 digit code is not found
					textProcCode.Text="D"+textProcCode.Text;
				}
				else { //or if it's a 4 digit code that's hidden, also add the D
					ProcedureCode procCode=ProcedureCodes.GetProcCode(textProcCode.Text);
					if(Defs.GetHidden(DefCat.ProcCodeCats,procCode.ProcCat)) {
						textProcCode.Text="D"+textProcCode.Text;
					}
				}
			}
			if(!ProcedureCodes.GetContainsKey(textProcCode.Text)) {
				MessageBox.Show(Lan.g(this,"Invalid code."));
				//textProcCode.Text="";
				textProcCode.SelectionStart=textProcCode.Text.Length;
				return;
			}
			if(Defs.GetHidden(DefCat.ProcCodeCats,ProcedureCodes.GetProcCode(textProcCode.Text).ProcCat)) {//if the category is hidden
				MessageBox.Show($"{Lan.g(this,"Code is in a hidden category and cannot be added from here")}: {textProcCode.Text}");
				textProcCode.SelectionStart=textProcCode.Text.Length;
				return;
			}
			//Do not return past this point---------------------------------------------------------------------------------
			if(_listSubstitutionLinks==null) {
				_listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(Pd.ListInsPlans);
			}
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(textProcCode.Text);
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(Pd.PatNum);
			List<Fee> listFees=Fees.GetListFromObjects(new List<ProcedureCode>(){ procedureCode },null,//no proc, so medical code won't have changed
				null,//listProvNumsTreat: providers will instead be set from other places
				Pd.Patient.PriProv,Pd.Patient.SecProv,Pd.Patient.FeeSched,Pd.ListInsPlans,new List<long>(){Pd.Patient.ClinicNum}, Pd.ListAppointments ,_listSubstitutionLinks,discountPlanNum);
			List<string> procCodes=new List<string>();
			//broken appointment procedure codes shouldn't trigger DateFirstVisit update.
			if(textProcCode.Text!="D9986" && textProcCode.Text!="D9987") {
				Procedures.SetDateFirstVisit(DateTime.Today,1,Pd.Patient);
			}
			TreatmentArea tArea;
			Procedure ProcCur;
			for(int n=0;n==0 || n<_toothChartRelay.SelectedTeeth.Count;n++) {//always loops at least once.
				ProcCur=new Procedure();//this will be an insert, so no need to set CurOld
				ProcCur.CodeNum=ProcedureCodes.GetCodeNum(textProcCode.Text);
				bool isValid=true;
				tArea=procedureCode.TreatArea;//ProcedureCodes.GetProcCode(ProcCur.CodeNum).TreatArea;
				if((tArea==TreatmentArea.None
					|| tArea==TreatmentArea.Arch
					|| tArea==TreatmentArea.Mouth
					|| tArea==TreatmentArea.Quad
					|| tArea==TreatmentArea.Sextant
					|| tArea==TreatmentArea.ToothRange)
					&& n>0) {//the only two left are tooth and surf
					continue;//only entered if n=0, so they don't get entered more than once.
				}
				else if(procedureCode.AreaAlsoToothRange) { // if AreaAlsoToothRange==true and procedureCode set for Quad or Arch
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						AddProcedure(ProcCur,listFees);
					}
					else { 
						if(tArea==TreatmentArea.Arch) { 
							AddArchProcsWithToothRange(ProcCur,listFees);
						}
						else if(tArea==TreatmentArea.Quad) {
							AddQuadProcsWithToothRange(ProcCur,listFees);
						}
					}
				}
				else if(tArea==TreatmentArea.Quad) {
					AddQuadProcs(ProcCur,listFees);
				}
				else if(tArea==TreatmentArea.Surf) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						isValid=false;
					}
					else {
						ProcCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
					}
					if(textSurf.Text=="") {
						isValid=false;
					}
					else {
						ProcCur.Surf=Tooth.SurfTidyFromDisplayToDb(textSurf.Text,ProcCur.ToothNum);//it's ok if toothnum is invalid
					}
					if(isValid) {
						AddQuick(ProcCur,listFees);
					}
					else {
						AddProcedure(ProcCur,listFees);
					}
				}
				else if(tArea==TreatmentArea.Tooth) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						AddProcedure(ProcCur,listFees);
					}
					else {
						ProcCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
						AddQuick(ProcCur,listFees);
					}
				}
				else if(tArea==TreatmentArea.ToothRange) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						AddProcedure(ProcCur,listFees);
					}
					else {
						ProcCur.ToothRange="";
						for(int b=0;b<_toothChartRelay.SelectedTeeth.Count;b++) {
							if(b!=0) ProcCur.ToothRange+=",";
							ProcCur.ToothRange+=_toothChartRelay.SelectedTeeth[b];
						}
						AddQuick(ProcCur,listFees);
					}
				}
				else if(tArea==TreatmentArea.Arch) {
					if(_toothChartRelay.SelectedTeeth.Count==0) {
						AutoCodeItem autoCodeItem=null;
						if(AutoCodeItems.GetContainsKey(ProcCur.CodeNum)) { 
							autoCodeItem=AutoCodeItems.GetListForCode(AutoCodeItems.GetOne(ProcCur.CodeNum).AutoCodeNum)
								.FirstOrDefault(x => x.CodeNum==ProcCur.CodeNum);
						}
						List<AutoCodeCond> listAutoCodeCond=new List<AutoCodeCond>();
						if(autoCodeItem!=null) {
							listAutoCodeCond=AutoCodeConds.GetListForItem(autoCodeItem.AutoCodeItemNum);
						}
						if(listAutoCodeCond.Count==1) {
							if(listAutoCodeCond[0].Cond==AutoCondition.Maxillary) {
								ProcCur.Surf="U";
							}
							else if(listAutoCodeCond[0].Cond==AutoCondition.Mandibular) {
								ProcCur.Surf="L";
							}
						}
						else {
							ProcCur.Surf=CodeMapping.GetArchSurfaceFromProcCode(ProcedureCodes.GetProcCode(ProcCur.CodeNum));
						}
						AddProcedure(ProcCur,listFees);
						continue;
					}
					foreach(string arch in Tooth.GetArchesForTeeth(_toothChartRelay.SelectedTeeth)) {
						Procedure proc=ProcCur.Copy();
						proc.Surf=arch;
						AddQuick(proc,listFees);
					}
				}
				else if(tArea==TreatmentArea.Sextant) {
					AddProcedure(ProcCur,listFees);
				}
				else {//mouth
					AddQuick(ProcCur,listFees);
				}
				procCodes.Add(ProcedureCodes.GetProcCode(ProcCur.CodeNum).ProcCode);
			}//n selected teeth
			//this was requiring too many irrelevant queries and going too slowly   //ModuleSelected(PatCur.PatNum);			
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			ClearButtons();
			FillProgNotes();
			textProcCode.Text="";
			textProcCode.Select();
			if(_procStatusNew==ProcStat.C) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,procCodes,Pd.PatNum);
			}
		}

		///<summary>If quickbutton, then pass in procButtonQuick and set procButton to null.</summary>
		private void ProcButtonClicked(ProcButton procButton,ProcButtonQuick procButtonQuick=null) {
			if(_procStatusNew==ProcStat.C) {
				if(!PrefC.GetBool(PrefName.AllowSettingProcsComplete)) {
					MsgBox.Show(this,"Set the procedure complete by setting the appointment complete.  "
						+"If you want to be able to set procedures complete, you must turn on that option in Setup | Chart | Chart Preferences.");
					return;
				}
				if(!Security.IsAuthorized(Permissions.ProcComplCreate,PIn.Date(textDate.Text))) {
					return;
				}
			}
			bool isValid;
			TreatmentArea tArea;
			int quadCount=0;//automates quadrant codes.
			long[] arrayCodeList;
			long[] arrayAutoCodeList;
			if(procButton==null) {//Quick Button
				arrayCodeList=new long[1];
				arrayCodeList[0]=ProcedureCodes.GetCodeNum(procButtonQuick.CodeValue);
				if(arrayCodeList[0]==0) {
					MessageBox.Show(this,Lan.g(this,"Procedure code does not exist in database")+" : "+procButtonQuick.CodeValue);
					return;
				}
				arrayAutoCodeList=new long[0];
			}
			else {//Proc Button
				arrayCodeList=ProcButtonItems.GetCodeNumListForButton(procButton.ProcButtonNum);
				arrayAutoCodeList=ProcButtonItems.GetAutoListForButton(procButton.ProcButtonNum);
				//if(codeList.
			}
			//It is very important that we stop users here before entering any procedures or doing any automation.
			foreach(long autoCodeNum in arrayAutoCodeList) {
				if(!AutoCodes.GetContainsKey(autoCodeNum)) {
					MsgBox.Show(this,$"The procedure button '{procButton.Description}' contains an invalid AutoCode.\r\n" +
						$"Run {nameof(DatabaseMaintenances.ProcButtonItemsDeleteWithInvalidAutoCode)} in the Database Maintenance Tool and try again.");
					return;
				}
				AutoCode autoCode=AutoCodes.GetOne(autoCodeNum);
				if(AutoCodeItems.GetListForCode(autoCode.AutoCodeNum).Count==0) {
					//AutoCode is not setup correctly.
					MessageBox.Show(this,Lan.g(this,"The following AutoCode has no associated Procedure Codes: ")+"\r\n"+autoCode.Description+"\r\n"
						+Lan.g(this,"AutoCode must be setup correctly before it can be used with a Quick Proc Button."));
					return;
				}
			}
			//Do not return past this point---------------------------------------------------------------------------------
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();//just for the fee info
			for(int i=0;i<arrayCodeList.Length;i++) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(arrayCodeList[i]));//could be a harmless empty code
			}
			string toothNumString;
			for(int i=0;i<arrayAutoCodeList.Length;i++) {//this is just a quick loop for fees. The real one is down further
				for(int n=0;n==0 || n<_toothChartRelay.SelectedTeeth.Count;n++) {
					isValid=true;
					if(_toothChartRelay.SelectedTeeth.Count!=0) toothNumString=_toothChartRelay.SelectedTeeth[n]; else toothNumString="";
					string surf="";
					if(textSurf.Text=="5" && CultureInfo.CurrentCulture.Name.EndsWith("CA")) surf="V"; else surf=Tooth.SurfTidyForClaims(textSurf.Text,toothNumString);
					bool isAdditional=n>0;
					bool willBeMissing=Procedures.WillBeMissing(toothNumString,Pd.PatNum);//db call, but this NEEDS to happen.
					long codeNum=AutoCodeItems.GetCodeNum(arrayAutoCodeList[i],toothNumString,surf,isAdditional,Pd.PatNum,Pd.Patient.Age,willBeMissing);
					listProcedureCodes.Add(ProcedureCodes.GetProcCode(codeNum));
				}
			}
			if(_listSubstitutionLinks==null) {
				_listSubstitutionLinks=SubstitutionLinks.GetAllForPlans(Pd.ListInsPlans);
			}
			long discountPlanNum=DiscountPlanSubs.GetDiscountPlanNumForPat(Pd.PatNum);
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,null,//no proc, so medical code won't have changed
				null,//listProvNumsTreat: providers will instead be set from other places
				Pd.Patient.PriProv,Pd.Patient.SecProv,Pd.Patient.FeeSched,Pd.ListInsPlans,new List<long>(){Pd.Patient.ClinicNum}, Pd.ListAppointments ,_listSubstitutionLinks,discountPlanNum);
			//If there are any codes in the list that are NOT 9986s and 9987s, then set the date first visit.
			if(arrayCodeList.Any(x => ProcedureCodes.GetStringProcCode(x) != "D9986" && ProcedureCodes.GetStringProcCode(x) != "D9987")) {
				Procedures.SetDateFirstVisit(DateTime.Today,1,Pd.Patient);
			}
			List<string> listProcCodes=new List<string>();
			//"Bug fix" for Dr. Lazar-------------
			bool isPeriapicalSix=false;
			if(arrayCodeList.Length==6) {//quick check before checking all codes. So that the program isn't slowed down too much.
				string tempVal="";
				foreach(long code in arrayCodeList) {
					tempVal+=ProcedureCodes.GetProcCode(code).AbbrDesc;
				}
				if(tempVal=="PAPA+PA+PA+PA+PA+") {
					isPeriapicalSix=true;
					_toothChartRelay.SelectedTeeth.Clear();//set tooth numbers later
				}
			}
			Procedure procCur=null;
			_listChartedProcs=new List<Procedure>();
			for(int i=0;i<arrayCodeList.Length;i++) {
				//needs to loop at least once, regardless of whether any teeth are selected.	
				for(int n=0;n==0 || n<_toothChartRelay.SelectedTeeth.Count;n++) {
					isValid=true;
					procCur=new Procedure();//insert, so no need to set CurOld
					procCur.CodeNum=ProcedureCodes.GetProcCode(arrayCodeList[i]).CodeNum;
					tArea=ProcedureCodes.GetProcCode(procCur.CodeNum).TreatArea;
					//"Bug fix" for Dr. Lazar-------------
					if(isPeriapicalSix) {
						//PA code is already set to treatment area mouth by default.
						procCur.ToothNum=",8,14,19,24,30".Split(',')[i];//first code has tooth num "";
						if(i==0) {
							tArea=TreatmentArea.Mouth;
						}
						else {
							tArea=TreatmentArea.Tooth;
						}
					}
					if((tArea==TreatmentArea.None
						|| tArea==TreatmentArea.Arch
						|| tArea==TreatmentArea.Mouth
						|| tArea==TreatmentArea.Quad
						|| tArea==TreatmentArea.Sextant
						|| tArea==TreatmentArea.ToothRange)
						&& n>0) 
					{//the only two left are tooth and surf
						continue;//only entered if n=0, so they don't get entered more than once.
					}
					else if(ProcedureCodes.GetProcCode(procCur.CodeNum).AreaAlsoToothRange) { // if AreaAlsoToothRange==true and procedureCode set for Quad or Arch
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							AddProcedure(procCur,listFees);
						}
						else { 
							if(tArea==TreatmentArea.Arch) { 
								AddArchProcsWithToothRange(procCur,listFees);
							}
							else if(tArea==TreatmentArea.Quad) {
								AddQuadProcsWithToothRange(procCur,listFees);
							}
						}
					}
					else if(tArea==TreatmentArea.Quad) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							switch(quadCount%4) {
								case 0: procCur.Surf="UR"; break;
								case 1: procCur.Surf="UL"; break;
								case 2: procCur.Surf="LL"; break;
								case 3: procCur.Surf="LR"; break;
							}
							quadCount++;
							if(procButtonQuick!=null && !string.IsNullOrWhiteSpace(procButtonQuick.Surf)) {//from quick buttons only.
								procCur.Surf=Tooth.SurfTidyFromDisplayToDb(procButtonQuick.Surf,procCur.ToothNum);
							}
							AddQuick(procCur,listFees);
						}
						else {
							AddQuadProcs(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Surf) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							isValid=false;
						}
						else {
							procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
						}
						if(textSurf.Text=="" && procButtonQuick==null) {
							isValid=false;// Pre-ODButtonPanel behavior
						}
						else if(procButtonQuick!=null && procButtonQuick.Surf=="") {
							isValid=false; // ODButtonPanel behavior
						}
						else {
							procCur.Surf=Tooth.SurfTidyFromDisplayToDb(textSurf.Text,procCur.ToothNum);//it's ok if toothnum is not valid.
							if(procButtonQuick!=null && !string.IsNullOrWhiteSpace(procButtonQuick.Surf)) {//from quick buttons only.
								procCur.Surf=Tooth.SurfTidyFromDisplayToDb(procButtonQuick.Surf,procCur.ToothNum);
								if(string.IsNullOrWhiteSpace(procCur.Surf)) {
									//QuickButton setup with a surface that is invalid for the selected tooth.  User should manually select surfaces via FormProcEdit.
									isValid=false;
								}
								//ProcCur.Surf=pbq.Surf;
							}
						}
						if(isValid) {
							AddQuick(procCur,listFees);
						}
						else {
							AddProcedure(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Tooth) {
						if(isPeriapicalSix) {
							AddQuick(procCur,listFees);
						}
						else if(_toothChartRelay.SelectedTeeth.Count==0) {
							AddProcedure(procCur,listFees);
						}
						else {
							procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
							AddQuick(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.ToothRange) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							AddProcedure(procCur,listFees);
						}
						else {
							procCur.ToothRange="";
							for(int b=0;b<_toothChartRelay.SelectedTeeth.Count;b++) {
								if(b!=0) procCur.ToothRange+=",";
								procCur.ToothRange+=_toothChartRelay.SelectedTeeth[b];
							}
							AddQuick(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Arch) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							procCur.Surf=CodeMapping.GetArchSurfaceFromProcCode(ProcedureCodes.GetProcCode(procCur.CodeNum));
							AddProcedure(procCur,listFees);
							continue;
						}
						foreach(string arch in Tooth.GetArchesForTeeth(_toothChartRelay.SelectedTeeth)) {
							Procedure proc=procCur.Copy();
							proc.Surf=arch;
							AddQuick(proc,listFees);
						}
					}
					else if(tArea==TreatmentArea.Sextant) {
						AddProcedure(procCur,listFees);
					}
					else {//mouth
						AddQuick(procCur,listFees);
					}
					listProcCodes.Add(ProcedureCodes.GetProcCode(procCur.CodeNum).ProcCode);
				}//n selected teeth
			}//end Part 1 checking for ProcCodes, now will check for AutoCodes
			//long orionProvNum=0;
			for(int i=0;i<arrayAutoCodeList.Length;i++) {
				for(int n=0;n==0 || n<_toothChartRelay.SelectedTeeth.Count;n++) {
					isValid=true;
					if(_toothChartRelay.SelectedTeeth.Count!=0) {
						toothNumString=_toothChartRelay.SelectedTeeth[n];
					}
					else {
						toothNumString="";
					}
					procCur=new Procedure();//this will be an insert, so no need to set CurOld
					//Clean to db
					string surf="";
					//For Canadians, when the only surface charted is 5, we need to not remove the 5 so that the correct one surface auto code is found.
					//However, if multiple surfaces are chated with the 5 then we need to remove the 5 because the surface is redundant.  E.g. B5 -> B
					if(textSurf.Text=="5" && CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						//5 is the Canadian equivalent of V and V is how we save it to the database.
						//We have to do this little extra step right here because SurfTidyForClaims() ignores the 5 surface because it Converts db to claim value.
						surf="V";
					}
					else {
						surf=Tooth.SurfTidyForClaims(textSurf.Text,toothNumString);
					}
					procCur.IsAdditional=n>0;	//This is used for determining the correct autocode in a little bit.
					bool willBeMissing=Procedures.WillBeMissing(toothNumString,Pd.PatNum);
					procCur.CodeNum=AutoCodeItems.GetCodeNum(arrayAutoCodeList[i],toothNumString,surf,procCur.IsAdditional,Pd.PatNum,Pd.Patient.Age,willBeMissing);
					tArea=ProcedureCodes.GetProcCode(procCur.CodeNum).TreatArea;
					if((tArea==TreatmentArea.None
						|| tArea==TreatmentArea.Arch
						|| tArea==TreatmentArea.Mouth
						|| tArea==TreatmentArea.Quad
						|| tArea==TreatmentArea.Sextant
						|| tArea==TreatmentArea.ToothRange)
						&& n>0)
					{//the only two left are tooth and surf
						continue;//only entered if n=0, so they don't get entered more than once.
					}
					else if(tArea==TreatmentArea.Quad) {
						if(toothNumString=="") {
							switch(quadCount%4) {
								case 0: procCur.Surf="UR"; break;
								case 1: procCur.Surf="UL"; break;
								case 2: procCur.Surf="LL"; break;
								case 4: procCur.Surf="LR"; break;
							}
							quadCount++;
							AddQuick(procCur,listFees);
						}
						else {
							AddQuadProcs(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Surf) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							isValid=false;
						}
						else {
							procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
						}
						if(textSurf.Text=="") {
							isValid=false;
						}
						else {
							procCur.Surf=Tooth.SurfTidyFromDisplayToDb(textSurf.Text,procCur.ToothNum);//it's ok if toothnum is invalid
						}
						
						if(isValid) {
							AddQuick(procCur,listFees);
						}
						else {
							AddProcedure(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Tooth) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							AddProcedure(procCur,listFees);
						}
						else {
							procCur.ToothNum=_toothChartRelay.SelectedTeeth[n];
							AddQuick(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.ToothRange) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							AddProcedure(procCur,listFees);
						}
						else {
							procCur.ToothRange="";
							for(int b=0;b<_toothChartRelay.SelectedTeeth.Count;b++) {
								if(b!=0) procCur.ToothRange+=",";
								procCur.ToothRange+=_toothChartRelay.SelectedTeeth[b];
							}
							AddQuick(procCur,listFees);
						}
					}
					else if(tArea==TreatmentArea.Arch) {
						if(_toothChartRelay.SelectedTeeth.Count==0) {
							procCur.Surf=CodeMapping.GetArchSurfaceFromProcCode(ProcedureCodes.GetProcCode(procCur.CodeNum));
							AddProcedure(procCur,listFees);
							continue;
						}
						foreach(string arch in Tooth.GetArchesForTeeth(_toothChartRelay.SelectedTeeth)) {
							Procedure proc=procCur.Copy();
							proc.Surf=arch;
							AddQuick(proc,listFees);
						}
					}
					else if(tArea==TreatmentArea.Sextant) {
						AddProcedure(procCur,listFees);
					}
					else {//mouth
						AddQuick(procCur,listFees);
					}
					listProcCodes.Add(ProcedureCodes.GetProcCode(procCur.CodeNum).ProcCode);
				}//n selected teeth
				//orionProvNum=ProcCur.ProvNum;
			}//for i
			//this was requiring too many irrelevant queries and going too slowly   //ModuleSelected(PatCur.PatNum);			
			Pd.ClearAndFill(EnumPdTable.ToothInitial);
			if(procButton!=null && procButton.IsMultiVisit) {
				//There are many complicated paths which might cause some of the procedures to be deleted (such as user cancel).
				//Refresh the procedures from the database to ensure the ones that we group together actually exist.
				_listChartedProcs=Procedures.GetManyProc(_listChartedProcs.Select(x => x.ProcNum).Where(x => x!=0).ToList(),false)
					.FindAll(x => x.ProcStatus!=ProcStat.D);
				ProcMultiVisits.CreateGroup(_listChartedProcs);
			}
			_listChartedProcs=null;
			ClearButtons();
			FillProgNotes();
			if(_procStatusNew==ProcStat.C) {
				AutomationL.Trigger(AutomationTrigger.CompleteProcedure,listProcCodes,Pd.PatNum);
				Pd.FillIfNeeded(EnumPdTable.Procedure);
			}
		}

		private void UpdateSurf() {
			textSurf.Text="";
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				return;
			}
			if(butM.BackColor==Color.White) {
				textSurf.AppendText("M");
			}
			if(butOI.BackColor==Color.White) {
				if(ToothGraphic.IsAnterior(_toothChartRelay.SelectedTeeth[0])) {
					textSurf.AppendText("I");
				}
				else {	
					textSurf.AppendText("O");
				}
			}
			if(butD.BackColor==Color.White) {
				textSurf.AppendText("D");
			}
			if(butV.BackColor==Color.White) {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					textSurf.AppendText("5");
				}
				else {
					textSurf.AppendText("V");
				}
			}
			if(butBF.BackColor==Color.White) {
				if(ToothGraphic.IsAnterior(_toothChartRelay.SelectedTeeth[0])) {
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						textSurf.AppendText("V");//vestibular
					}
					else {
						textSurf.AppendText("F");
					}
				}
				else {
					textSurf.AppendText("B");
				}
			}
			if(butL.BackColor==Color.White) {
				textSurf.AppendText("L");
			}
		}

		public void UserLogOffCommited() {
			_sheetLayoutController?.UserLogOffCommited();//Can be null if user never visted the chart module.
		}
		#endregion Methods - Private Tab - EnterTx

		#region Methods - Private - Tab Movements
		///<summary>Fills all of the toothinitial controls in the various tabs, including movements, hidden, and text drawing.</summary>
		private void FillMovementsAndHidden() {
			#region Movement
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				textShiftM.Text="";
				textShiftO.Text="";
				textShiftB.Text="";
				textRotate.Text="";
				textTipM.Text="";
				textTipB.Text="";
			}
			else {
				textShiftM.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.ShiftM).ToString();
				textShiftO.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.ShiftO).ToString();
				textShiftB.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.ShiftB).ToString();
				textRotate.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.Rotate).ToString();
				textTipM.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.TipM).ToString();
				textTipB.Text=
					ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[0],ToothInitialType.TipB).ToString();
				//At this point, all 6 blanks have either a number or 0.
				//As we go through this loop, none of the values will change.
				//The only thing that will happen is that some of them will become blank.
				string move;
				for(int i=1;i<_toothChartRelay.SelectedTeeth.Count;i++) {
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftM).ToString();
					if(textShiftM.Text!=move) {
						textShiftM.Text="";
					}
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftO).ToString();
					if(textShiftO.Text!=move) {
						textShiftO.Text="";
					}
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.ShiftB).ToString();
					if(textShiftB.Text!=move) {
						textShiftB.Text="";
					}
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.Rotate).ToString();
					if(textRotate.Text!=move) {
						textRotate.Text="";
					}
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipM).ToString();
					if(textTipM.Text!=move) {
						textTipM.Text="";
					}
					move=ToothInitials.GetMovement(Pd.ListToothInitials,_toothChartRelay.SelectedTeeth[i],ToothInitialType.TipB).ToString();
					if(textTipB.Text!=move) {
						textTipB.Text="";
					}
				}
			}
			#endregion Movement
			#region Hidden
			listHidden.Items.Clear();
			_arrayListHiddenTeeth=ToothInitials.GetHiddenTeeth(Pd.ListToothInitials);
			for(int i=0;i<_arrayListHiddenTeeth.Count;i++) {
				listHidden.Items.Add(Tooth.Display((string)_arrayListHiddenTeeth[i]));
			}
			#endregion Hidden
			#region TextDraw
			listBoxText.Items.Clear();
			_listToothInitialsText=new List<ToothInitial>();
			if(Pd.ListToothInitials!=null){
				_listToothInitialsText=Pd.ListToothInitials.FindAll(x=>x.InitialType==ToothInitialType.Text);
			}
			for(int i=0;i<_listToothInitialsText.Count;i++) {
				listBoxText.Items.Add(_listToothInitialsText[i].GetTextString(),_listToothInitialsText[i]);
			}
			#endregion TextDraw
		}
		#endregion Methods - Private - Tab Movements

		#region Methods - Private - Tab Planned
		private void FillPlanned() {
			if(IsPatientNull()) {
				//clear patient data, might be left over if login sessions changed
				gridPlanned.BeginUpdate();
				gridPlanned.ListGridRows.Clear();
				gridPlanned.EndUpdate();
				checkDone.Checked=false;
				butNew.Enabled=false;
				butPin.Enabled=false;
				butClear.Enabled=false;
				butUp.Enabled=false;
				butDown.Enabled=false;
				gridPlanned.Enabled=false;
				return;
			}
			else {
				butNew.Enabled=true;
				butPin.Enabled=!Programs.UsingEcwTightOrFullMode();
				butClear.Enabled=true;
				butUp.Enabled=true;
				butDown.Enabled=true;
				gridPlanned.Enabled=true;
			}
			if(Pd.Patient.PlannedIsDone) {
				checkDone.Checked=true;
			}
			else {
				checkDone.Checked=false;
			}
			//Fill grid
			gridPlanned.BeginUpdate();
			gridPlanned.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TablePlannedAppts","#"),25,HorizontalAlignment.Center);
			gridPlanned.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Min"),35);
			gridPlanned.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Procedures"),175);
			gridPlanned.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePlannedAppts","Note"),175);
			gridPlanned.Columns.Add(col);
			if(Programs.UsingOrion) {
				col=new GridColumn(Lan.g("TablePlannedAppts","SchedBy"),80);
			}
			else {
				col=new GridColumn(Lan.g("TablePlannedAppts","DateSched"),80);
			}
			gridPlanned.Columns.Add(col);
			gridPlanned.ListGridRows.Clear();
			GridRow row;
			_tablePlannedAll=Pd.TablePlannedAppts;
			//This gets done in the business layer:
			/*
			bool iochanged=false;
			for(int i=0;i<table.Rows.Count;i++) {
				if(table.Rows[i]["ItemOrder"].ToString()!=i.ToString()) {
					PlannedAppt planned=PlannedAppts.CreateObject(PIn.PLong(table.Rows[i]["PlannedApptNum"].ToString()));
					planned.ItemOrder=i;
					PlannedAppts.InsertOrUpdate(planned);
					iochanged=true;
				}
			}
			if(iochanged) {
				DataSetMain=ChartModules.GetAll(PatCur.PatNum,checkAudit.Checked);
				table=_loadData.TablePlannedAppts;
			}*/
			_listPlannedAppt=new List<DataRow>();
			for(int i=0;i<_tablePlannedAll.Rows.Count;i++) {
				if(_tablePlannedAll.Rows[i]["AptStatus"].ToString()=="2" && !checkShowCompleted.Checked) {
					continue;
				}
				_listPlannedAppt.Add(_tablePlannedAll.Rows[i]);//List containing only rows we are showing, can be the same as _tablePlannedAll
				row=new GridRow();
				row.Cells.Add((gridPlanned.ListGridRows.Count+1).ToString());
				row.Cells.Add(_tablePlannedAll.Rows[i]["minutes"].ToString());
				row.Cells.Add(_tablePlannedAll.Rows[i]["ProcDescript"].ToString());
				row.Cells.Add(_tablePlannedAll.Rows[i]["Note"].ToString());
				if(Programs.UsingOrion) {
					string text;
					List<Procedure> listProcs=Procedures.Refresh(Pd.PatNum);
					DateTime newDateSched=new DateTime();
					for(int p=0;p<listProcs.Count;p++) {
						if(listProcs[p].PlannedAptNum==PIn.Long(_tablePlannedAll.Rows[i]["AptNum"].ToString())) {
							OrionProc orionProc=OrionProcs.GetOneByProcNum(listProcs[p].ProcNum);
							if(orionProc!=null && orionProc.DateScheduleBy.Year>1880) {
								if(newDateSched.Year<1880) {
									newDateSched=orionProc.DateScheduleBy;
								}
								else {
									if(orionProc.DateScheduleBy<newDateSched) {
										newDateSched=orionProc.DateScheduleBy;
									}
								}
							}
						}
					}
					if(newDateSched.Year>1880) {
						text=newDateSched.ToShortDateString();
					}
					else {
						text="None";
					}
					row.Cells.Add(text);
				}
				else {//Not Orion
					ApptStatus aptStatus=(ApptStatus)(PIn.Long(_tablePlannedAll.Rows[i]["AptStatus"].ToString()));
					if(aptStatus==ApptStatus.UnschedList) {
						row.Cells.Add(Lan.g(this,"Unsched"));
					}
					else if(aptStatus==ApptStatus.Broken) {
						row.Cells.Add(Lan.g(this,"Broken"));
					}
					else {//scheduled, complete and ASAP
						row.Cells.Add(_tablePlannedAll.Rows[i]["dateSched"].ToString());
					}
				}
				row.ColorText=Color.FromArgb(PIn.Int(_tablePlannedAll.Rows[i]["colorText"].ToString()));
				row.ColorBackG=Color.FromArgb(PIn.Int(_tablePlannedAll.Rows[i]["colorBackG"].ToString()));
				row.Tag=PIn.Long(_tablePlannedAll.Rows[i]["AptNum"].ToString());
				gridPlanned.ListGridRows.Add(row);
			}
			gridPlanned.EndUpdate();
			for(int i=0;i<_listPlannedAppt.Count;i++) {
				if(_listSelectedAptNums.Contains(PIn.Long(_listPlannedAppt[i]["AptNum"].ToString()))) {
					gridPlanned.SetSelected(i,true);
				}
			}
		}

		///<summary>Sets item orders appropriately. Does not reorder list, and does not repaint/refill grid.</summary>
		private void moveItemOrderHelper(DataRow plannedAppt,int newItemOrder) {
			int plannedApptItemOrder=PIn.Int(plannedAppt["ItemOrder"].ToString());
			if(plannedApptItemOrder>newItemOrder) {//moving item up, itterate down through list
				for(int i=0;i<_tablePlannedAll.Rows.Count;i++) {
					int itemOrderCur=PIn.Int(_tablePlannedAll.Rows[i]["ItemOrder"].ToString());
					if(_tablePlannedAll.Rows[i]["PlannedApptNum"].ToString()==plannedAppt["PlannedApptNum"].ToString()) {
						_tablePlannedAll.Rows[i]["ItemOrder"]=newItemOrder;//set item order of this PlannedAppt.
						continue;
					}
					if(itemOrderCur>=newItemOrder && itemOrderCur<plannedApptItemOrder) {//all items between newItemOrder and oldItemOrder
						_tablePlannedAll.Rows[i]["ItemOrder"]=itemOrderCur+1;
					}
				}
			}
			else {//moving item down, itterate up through list
				for(int i=_tablePlannedAll.Rows.Count-1;i>=0;i--) {
					int itemOrderCur=PIn.Int(_tablePlannedAll.Rows[i]["ItemOrder"].ToString());
					if(_tablePlannedAll.Rows[i]["PlannedApptNum"].ToString()==plannedAppt["PlannedApptNum"].ToString()) {
						_tablePlannedAll.Rows[i]["ItemOrder"]=newItemOrder;//set item order of this PlannedAppt.
						continue;
					}
					if(itemOrderCur<=newItemOrder && itemOrderCur>plannedApptItemOrder) {//all items between newItemOrder and oldItemOrder
						_tablePlannedAll.Rows[i]["ItemOrder"]=itemOrderCur-1;
					}
				}
			}
			//tablePlannedAll has correct itemOrder values, which we need to copy to _listPlannedAppt without changing the actual order of _listPlannedAppt.
			for(int i=0;i<_listPlannedAppt.Count;i++) {
				for(int j=0;j<_tablePlannedAll.Rows.Count;j++) {
					if(_listPlannedAppt[i]["PlannedApptNum"].ToString()!=_tablePlannedAll.Rows[j]["PlannedApptNum"].ToString()) {
						continue;
					}
					_listPlannedAppt[i]=_tablePlannedAll.Rows[j];//update order.
				}
			}
		}

		///<summary>Updates database based on the values in _tablePlannedAll.Rows.</summary>
		private void saveChangesToDBHelper() {
			//Get all PlannedAppts from db to check for changes
			List<PlannedAppt> listPlannedAllDB=PlannedAppts.Refresh(Pd.PatNum);
			//Itterate through current PlannedAppts list in memory and compare to db list
			for(int i=0;i<_tablePlannedAll.Rows.Count;i++) {
				//find db version of PlannedAppt to update
				PlannedAppt oldPlannedAppt=null;
				PlannedAppt plannedAppt=null;
				for(int j=0;j<listPlannedAllDB.Count;j++) {
					if(PIn.Long(_tablePlannedAll.Rows[i]["PlannedApptNum"].ToString())!=listPlannedAllDB[j].PlannedApptNum) {
						continue;//not the correct PlannedAppt
					}
					//found the correct PlannedAppt
					oldPlannedAppt=PlannedAppts.GetOne(PIn.Long(_tablePlannedAll.Rows[i]["PlannedApptNum"].ToString()));
					plannedAppt=oldPlannedAppt.Copy();
					plannedAppt.ItemOrder=PIn.Int(_tablePlannedAll.Rows[i]["ItemOrder"].ToString());
					break;//found match
				}
				if(plannedAppt==null) {//should never happen, this would mean a planned appt in our local list doesn't exist in the db
					continue;
				}
				PlannedAppts.Update(plannedAppt,oldPlannedAppt);
			}
		}
		#endregion Methods - Private - Tab Planned

		#region Methods - Private - Tab Show
		///<summary>Searches the given row at the given column for any matching search terms in searchInput. If a match is found, the search term is removed from searchInput.</summary>
		private void CheckForSearchMatch(string columnName,DataRow rowCur,ref List<string> searchInput,bool isClinicDesc=false,bool isClinicAbbr=false) {
			for(int i=searchInput.Count-1;i>=0;--i) {
				if(isClinicAbbr) {
					if(Clinics.GetAbbr(PIn.Long((rowCur["ClinicNum"].ToString().ToLower()))).Contains(searchInput[i])) {
						searchInput.RemoveAt(i);
					}
				}
				else if(isClinicDesc) {
					if(Clinics.GetDesc(PIn.Long((rowCur["ClinicNum"].ToString().ToLower()))).Contains(searchInput[i])) {
						searchInput.RemoveAt(i);
					}
				}
				else if((rowCur[columnName].ToString().ToLower()).Contains(searchInput[i])) {
					searchInput.RemoveAt(i);
				}
			}
		}

		///<summary>Searchs the progress notes for the entered search text.</summary>
		private void SearchProgNotes() {
			List<string> listSearchWords=new List<string>();
			string currentSearchText=textSearch.Text.ToLower();
			foreach(string word in textSearch.Text.ToLower().Split(' ')) {
				if(word!="") {
					listSearchWords.Add(word);
				}
			}
			_dateTimeLastSearch=DateTime.Now;
			//Create copy of list results to loop through within the search function
			List<DataRow> listSearchResults=new List<DataRow>();
			if(_listSearchResults!=null) {
				foreach(DataRow curRow in _listSearchResults) {
					DataRow row=Pd.TableProgNotes.NewRow();
					row.ItemArray=curRow.ItemArray;
					listSearchResults.Add(row);
				}
			}
			else {
				listSearchResults=null;
			}
			SearchProgNotes(listSearchWords,_dateTimeLastSearch,listSearchResults,currentSearchText);
		}
		
		///<summary>Searches the current view of the progress notes for any search terms passed in.</summary>
		private void SearchProgNotes(List<string> listSearchWords,DateTime dateTimeStartSearch,List<DataRow>listSearchResults, 
			string currentSearchText) 
		{
			string previousSearchText=_previousSearchText;
			List<DisplayField> listDisFields;
			if(_chartViewCurDisplay==null) {//No chart views, Use default values.
				listDisFields=DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			}
			else {
				listDisFields=DisplayFields.GetForChartView(_chartViewCurDisplay.ChartViewNum);
			}
			DataTable table=new DataTable();
			//If the current search is the last search with more added on, only search the currently selected rows. Helps with speed for long progress notes.
			if(currentSearchText.StartsWith(previousSearchText) && previousSearchText!="" && listSearchResults!=null) {
				table=Pd.TableProgNotes.Clone();
				for(int i=0;i<listSearchResults.Count;i++) {
					table.Rows.Add(listSearchResults[i].ItemArray);
				}
			}
			else {
				//Otherwise search all of the progress notes.
				table=Pd.TableProgNotes;
			}
			listSearchResults?.Clear();
			listSearchResults=new List<DataRow>();
			foreach(DataRow rowCur in table.Rows) {
				//We are going to remove words as they are found from this list. If the list is empty, then that means it matches all the search terms.
				List<string> listSearchInput=new List<string>(listSearchWords);
				if(checkNotes.Checked) {
					CheckForSearchMatch("note",rowCur,ref listSearchInput);
				}
				for(int f=0;f<listDisFields.Count;f++) {
					switch(listDisFields[f].InternalName) {
						case DisplayFields.InternalNames.ChartView.Date:
							CheckForSearchMatch("procDate",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Time:
							CheckForSearchMatch("procTime",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Th:
							CheckForSearchMatch("toothNum",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Surf:
							CheckForSearchMatch("surf",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Dx:
							CheckForSearchMatch("dx",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Description:
							CheckForSearchMatch("description",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Stat:
							CheckForSearchMatch("procStatus",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Prov:
							CheckForSearchMatch("prov",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Amount:
							CheckForSearchMatch("procFee",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.ProcCode:
							CheckForSearchMatch("ProcCode",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.User:
							CheckForSearchMatch("user",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Signed:
							CheckForSearchMatch("signature",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Priority:
							CheckForSearchMatch("priority",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.DateEntry:
							CheckForSearchMatch("dateEntryC",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Prognosis:
							CheckForSearchMatch("prognosis",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.DateTP:
							CheckForSearchMatch("dateTP",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.EndTime:
							CheckForSearchMatch("procTimeEnd",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Quadrant:
							CheckForSearchMatch("quadrant",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.ScheduleBy:
							CheckForSearchMatch("orionDateScheduleBy",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.StopClock:
							CheckForSearchMatch("orionDateStopClock",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.DPC:
							CheckForSearchMatch("orionDPC",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.EffectiveComm:
							CheckForSearchMatch("orionIsEffectiveComm",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.OnCall:
							CheckForSearchMatch("orionIsOnCall",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Stat2:
							CheckForSearchMatch("orionStatus2",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.DPCpost:
							CheckForSearchMatch("orionDPCpost",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Length:
							CheckForSearchMatch("length",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Abbr: //abbreviation for procedures
							CheckForSearchMatch("AbbrDesc",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Locked:
							CheckForSearchMatch("isLocked",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.HL7Sent:
							CheckForSearchMatch("hl7Sent",rowCur,ref listSearchInput);
							break;
						case DisplayFields.InternalNames.ChartView.Clinic:
							CheckForSearchMatch("ClinicNum",rowCur,ref listSearchInput,isClinicAbbr:true);
							break;
						case DisplayFields.InternalNames.ChartView.ClinicDesc:
							CheckForSearchMatch("ClinicNum",rowCur,ref listSearchInput,isClinicDesc:true);
							break;
						default:
							break;
					}
					if(listSearchInput.Count==0) {//All the passed in search terms match.
						listSearchResults.Add(rowCur);
						break;
					}
				}
			}
			//Thread.Sleep(200);//This is so that we don't fill the grid after the user has only typed one search character.
			if(_dateTimeLastSearch==dateTimeStartSearch) {//If the user has not typed something while the search occurred
				_previousSearchText=currentSearchText;
				_listSearchResults=listSearchResults;
				FillProgNotes(isSearch: true);
			}
		}
		#endregion Methods - Private - Tab Show

		#region Methods - Private - Tab Ortho
		private void butOrthoAdd_Click(object sender,EventArgs e) {
			if(!checkShowOrtho.Checked){
				MsgBox.Show(this,"First, check the box to Show Ortho grids.");
				return;
			}
			if(_toothChartRelay.SelectedTeeth.Count==0) {
				MsgBox.Show(this,"Please select teeth first.");
				return;
			}
			List<OrthoHardware> listOrthoHardwaresToday=Pd.ListOrthoHardwares.FindAll(x=>x.DateExam==DateTime.Today);
			if(Pd.ListOrthoHardwares.Count>0 && listOrthoHardwaresToday.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Normally, you would make a copy of a previous exam date before adding more hardware. Continue anyway?")){
					return;
				}
			}
			if(listOrthoHardwaresToday.Count>0 && comboOrthoDate.GetSelected<DateTime>() !=DateTime.Today) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Normally, you would show today's exam date before adding more hardware. Continue anyway?")){
					return;
				}
			}
			using FormOrthoHardwareAdd formOrthoHardwareAdd=new FormOrthoHardwareAdd();
			formOrthoHardwareAdd.ShowDialog();
			if(formOrthoHardwareAdd.DialogResult!=DialogResult.OK){
				return;
			}
			for(int p=0;p<formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected.Count;p++){
				if(formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType==EnumOrthoHardwareType.Bracket){
					for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
						OrthoHardware orthoHardware=new OrthoHardware();
						orthoHardware.PatNum=Pd.PatNum;
						orthoHardware.OrthoHardwareType=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType;
						orthoHardware.DateExam=DateTime.Today;
						orthoHardware.OrthoHardwareSpecNum=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareSpecNum;
						orthoHardware.ToothRange=_toothChartRelay.SelectedTeeth[i];
						OrthoHardwares.Insert(orthoHardware);
					}
				}
				if(formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType==EnumOrthoHardwareType.Wire){
					List<string> listStringsUpper=new List<string>();
					List<string> listStringsLower=new List<string>();
					for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
						if(Tooth.IsMaxillary(_toothChartRelay.SelectedTeeth[i])){
							listStringsUpper.Add(_toothChartRelay.SelectedTeeth[i]);
						}
						else{
							listStringsLower.Add(_toothChartRelay.SelectedTeeth[i]);
						}
					}
					//They should still be in order
					if(listStringsUpper.Count>1){
						OrthoHardware orthoHardware=new OrthoHardware();
						orthoHardware.PatNum=Pd.PatNum;
						orthoHardware.OrthoHardwareType=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType;
						orthoHardware.DateExam=DateTime.Today;
						orthoHardware.OrthoHardwareSpecNum=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareSpecNum;
						orthoHardware.ToothRange=listStringsUpper[0]+"-"+listStringsUpper[listStringsUpper.Count-1];
						OrthoHardwares.Insert(orthoHardware);
					}
					if(listStringsLower.Count>1){
						OrthoHardware orthoHardware=new OrthoHardware();
						orthoHardware.PatNum=Pd.PatNum;
						orthoHardware.OrthoHardwareType=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType;
						orthoHardware.DateExam=DateTime.Today;
						orthoHardware.OrthoHardwareSpecNum=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareSpecNum;
						orthoHardware.ToothRange=listStringsLower[0]+"-"+listStringsLower[listStringsLower.Count-1];
						OrthoHardwares.Insert(orthoHardware);
					}
				}
				if(formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType==EnumOrthoHardwareType.Elastic){
					string teeth="";
					for(int i=0;i<_toothChartRelay.SelectedTeeth.Count;i++) {
						if(i>0){
							teeth+=",";
						}
						teeth+=_toothChartRelay.SelectedTeeth[i];
					}
					OrthoHardware orthoHardware=new OrthoHardware();
					orthoHardware.PatNum=Pd.PatNum;
					orthoHardware.OrthoHardwareType=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareType;
					orthoHardware.DateExam=DateTime.Today;
					orthoHardware.OrthoHardwareSpecNum=formOrthoHardwareAdd.ListOrthoHardwareSpecsSelected[p].OrthoHardwareSpecNum;
					orthoHardware.ToothRange=teeth;
					OrthoHardwares.Insert(orthoHardware);
				}
			}
			Pd.ClearAndFill(EnumPdTable.OrthoHardware);
			FillOrthoDateIfNeeded(true);
			FillToothChart(false);
			FillGridOrtho();
		}

		private void butOrthoCopy_Click(object sender,EventArgs e) {
			if(!checkShowOrtho.Checked){
				MsgBox.Show(this,"First, check the box to Show Ortho grids.");
				return;
			}
			if(tabOrthoCategories.SelectedTab.Tag is OrthoChartTab){
				MsgBox.Show(this,"Please select the Hardware tab first.");
				return;
			}
			if(Pd.ListOrthoHardwares.Count==0){
				MsgBox.Show(this,"There are no existing hardware items to copy.");
				return;
			}
			if(_toothChartRelay.SelectedTeeth.Count>0) {
				MsgBox.Show(this,"Please deselect teeth in the tooth chart first. The copy is based on items in the grid below, not selected teeth in the tooth chart.");
				return;
			}
			if(comboOrthoDate.GetSelected<DateTime>()==DateTime.Today){
				MsgBox.Show(this,"You must select an exam date other than today before copying.");
				return;
			}
			List<OrthoHardware> listOrthoHardwaresToday=Pd.ListOrthoHardwares.FindAll(x=>x.DateExam==DateTime.Today);
			if(listOrthoHardwaresToday.Count>0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"There are already hardware items for today's date.  Normally, there would be no existing items with today's date.  Continue anyway?")){
					return;
				}
			}
			if(gridOrtho.SelectedIndices.Length>0){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Copy selected items to today's date?")){
					return;
				}
			}
			else{
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Copy all hardware showing to today's date?")){
					return;
				}
			}
			//because of how the date combo works, there are always guaranteed to be hardware items for the selected date.
			if(gridOrtho.SelectedIndices.Length>0){
				for(int i=0;i<gridOrtho.SelectedIndices.Length;i++){
					OrthoHardware orthoHardware=(OrthoHardware)gridOrtho.ListGridRows[gridOrtho.SelectedIndices[i]].Tag;
					orthoHardware.DateExam=DateTime.Today;
					OrthoHardwares.Insert(orthoHardware);//creates new pk
				}
			}
			else{
				for(int i=0;i<gridOrtho.ListGridRows.Count;i++){
					OrthoHardware orthoHardware=(OrthoHardware)gridOrtho.ListGridRows[i].Tag;
					orthoHardware.DateExam=DateTime.Today;
					OrthoHardwares.Insert(orthoHardware);//creates new pk
				}
			}
			ModuleSelected(Pd.PatNum);
			MsgBox.Show(this,"Done.");
		}

		private void butOrthoDelete_Click(object sender,EventArgs e) {
			if(!checkShowOrtho.Checked){
				MsgBox.Show(this,"First, check the box to Show Ortho grids.");
				return;
			}
			if(tabOrthoCategories.SelectedTab.Tag is OrthoChartTab){
				MsgBox.Show(this,"Please select items in the Hardware tab first.");
				return;
			}
			if(gridOrtho.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select hardware items in the grid first.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete selected?")){
				return;
			}
			for(int i=0;i<gridOrtho.SelectedIndices.Length;i++){
				OrthoHardware orthoHardware=(OrthoHardware)gridOrtho.ListGridRows[gridOrtho.SelectedIndices[i]].Tag;
				OrthoHardwares.Delete(orthoHardware.OrthoHardwareNum);
			}
			ModuleSelected(Pd.PatNum);
		}

		private void butOrthoRx_Click(object sender,EventArgs e) {
			List<OrthoHardware> listOrthoHardwaresToday=Pd.ListOrthoHardwares.FindAll(x=>x.DateExam==DateTime.Today);
			if(Pd.ListOrthoHardwares.Count>0 && listOrthoHardwaresToday.Count==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Normally, you would make a copy of a previous exam date before adding more hardware. Continue anyway?")){
					return;
				}
			}
			if(listOrthoHardwaresToday.Count>0 && comboOrthoDate.GetSelected<DateTime>() !=DateTime.Today) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Normally, you would show today's exam date before adding more hardware. Continue anyway?")){
					return;
				}
			}
			FormOrthoRxSelect formOrthoRxSelect=new FormOrthoRxSelect();
			formOrthoRxSelect.ShowDialog();
			if(formOrthoRxSelect.DialogResult!=DialogResult.OK){
				return;
			}
			for(int i=0;i<formOrthoRxSelect.ListOrthoRxsSelected.Count;i++){
				OrthoHardwareSpec orthoHardwareSpec=OrthoHardwareSpecs.GetFirstOrDefault(x=>x.OrthoHardwareSpecNum==formOrthoRxSelect.ListOrthoRxsSelected[i].OrthoHardwareSpecNum);
				if(orthoHardwareSpec is null){
					continue;
				}
				if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Bracket){
					string[] stringArrayTeeth=formOrthoRxSelect.ListOrthoRxsSelected[i].ToothRange.Split(',');
					for(int t=0;t<stringArrayTeeth.Length;t++){
						if(!Tooth.IsValidDB(stringArrayTeeth[t])){
							MsgBox.Show(this,"");
							break;
						}
						OrthoHardware orthoHardware=new OrthoHardware();
						orthoHardware.PatNum=Pd.PatNum;
						orthoHardware.OrthoHardwareType=EnumOrthoHardwareType.Bracket;
						orthoHardware.DateExam=DateTime.Today;
						orthoHardware.OrthoHardwareSpecNum=formOrthoRxSelect.ListOrthoRxsSelected[i].OrthoHardwareSpecNum;
						orthoHardware.ToothRange=stringArrayTeeth[t];
						OrthoHardwares.Insert(orthoHardware);
					}
				}
				if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Elastic){
					OrthoHardware orthoHardware=new OrthoHardware();
					orthoHardware.PatNum=Pd.PatNum;
					orthoHardware.OrthoHardwareType=orthoHardwareSpec.OrthoHardwareType;
					orthoHardware.DateExam=DateTime.Today;
					orthoHardware.OrthoHardwareSpecNum=formOrthoRxSelect.ListOrthoRxsSelected[i].OrthoHardwareSpecNum;
					orthoHardware.ToothRange=formOrthoRxSelect.ListOrthoRxsSelected[i].ToothRange;//already in correct format with commas
					OrthoHardwares.Insert(orthoHardware);
				}
				if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
					OrthoHardware orthoHardware=new OrthoHardware();
					orthoHardware.PatNum=Pd.PatNum;
					orthoHardware.OrthoHardwareType=EnumOrthoHardwareType.Wire;
					orthoHardware.DateExam=DateTime.Today;
					orthoHardware.OrthoHardwareSpecNum=formOrthoRxSelect.ListOrthoRxsSelected[i].OrthoHardwareSpecNum;
					orthoHardware.ToothRange=formOrthoRxSelect.ListOrthoRxsSelected[i].ToothRange;//already in correct format with hypen
					OrthoHardwares.Insert(orthoHardware);
				}
			}
			Pd.ClearAndFill(EnumPdTable.OrthoHardware);
			FillToothChart(false);
			FillGridOrtho();
		}

		private void checkOrthoMode_Click(object sender,EventArgs e) {
			//also see tabProc_MouseDown
			FillToothChart(false);
		}

		private void checkShowOrtho_Click(object sender,EventArgs e) {
			//also see tabProc_MouseDown
			LayoutControls();
			if(checkShowOrtho.Checked){
				FillOrthoTabs();
				Pd.ClearAndFill(EnumPdTable.OrthoChart);
				FillGridOrtho();
			}
		}

		private void checkOrthoGraphics_Click(object sender,EventArgs e) {
			FillToothChart(retainSelection:true);
		}

		private void tabOrthoCategories_Click(object sender,EventArgs e) {
			//no need to get fresh data
			FillGridOrtho();
		}

		private void FillOrthoDateIfNeeded(bool forceRefresh=false){
			if(comboOrthoDate.Items.Count>0 && !forceRefresh){
				return;
			}
			comboOrthoDate.Items.Clear();
			List<DateTime> listDateTimes=Pd.ListOrthoHardwares.Select(x=>x.DateExam).Distinct().OrderByDescending(x=>x).ToList();//most recent date is first
			if(listDateTimes.Count==0){
				comboOrthoDate.Items.Add(DateTime.Today.ToShortDateString(),DateTime.Today);
			}
			else{
				comboOrthoDate.Items.AddList(listDateTimes,x=>x.ToShortDateString());
			}
			comboOrthoDate.SetSelected(0);
		}

		private void comboOrthoDate_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridOrthoHardware();
			FillToothChart(retainSelection:true);
		}

		///<summary></summary>
		private void FillOrthoTabs() {
			List<OrthoChartTab> listOrthoChartTabs=OrthoChartTabs.GetDeepCopy(isShort:true);
			object objectTabSelected=null;
			if(tabOrthoCategories.SelectedIndex >= 0) {
				objectTabSelected=tabOrthoCategories.TabPages[tabOrthoCategories.SelectedIndex].Tag;
			}
			tabOrthoCategories.TabPages.Clear();
			TabPage tabPage=new TabPage("Hardware");
			tabPage.Tag="Hardware";
			LayoutManager.Add(tabPage,tabOrthoCategories);
			if(objectTabSelected!=null && objectTabSelected.ToString()=="Hardware") {
				tabOrthoCategories.SelectedIndex=0;
			}
			for(int i=0;i<listOrthoChartTabs.Count;i++) {
				tabPage=new TabPage(listOrthoChartTabs[i].TabName);
				tabPage.Tag=listOrthoChartTabs[i];
				LayoutManager.Add(tabPage,tabOrthoCategories);
				if(objectTabSelected!=null && objectTabSelected is OrthoChartTab orthoChartTab){
					if(listOrthoChartTabs[i].OrthoChartTabNum==orthoChartTab.OrthoChartTabNum) {
						tabOrthoCategories.SelectedIndex=i+1;
					}
				}
			}
			if(tabOrthoCategories.SelectedIndex==-1) {
				tabOrthoCategories.SelectedIndex=0;
			}
		}

		///<summary>Like the corresponding method in FormOrthoChart, this one does not get the data.</summary>
		private void FillGridOrtho() {
			if(tabOrthoCategories.SelectedIndex==-1) {
				return;//This happens when the tab pages are cleared (updating the selected index).
			}
			if(tabOrthoCategories.SelectedTab.Tag is OrthoChartTab){
				FillGridOrthoChart();
			}
			else{
				FillGridOrthoHardware();
			}
		}

		private void FillGridOrthoHardware(){
			Cursor=Cursors.WaitCursor;
			gridOrtho.SelectionMode=GridSelectionMode.MultiExtended;
			ContextMenu contextMenu=new ContextMenu();
			contextMenu.MenuItems.Add("Delete",MenuItemOrthoDelete_Click);
			gridOrtho.ContextMenu=contextMenu;
			gridOrtho.BeginUpdate();
			gridOrtho.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableOrtho","Date"),70);
			gridOrtho.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrtho","Type"),70);
			gridOrtho.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrtho","Teeth"),70);
			gridOrtho.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrtho","Description"),250);
			gridOrtho.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOrtho","Note"),300);
			gridOrtho.Columns.Add(col);
			gridOrtho.ListGridRows.Clear();
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			FillOrthoDateIfNeeded();
			List<OrthoHardware> listOrthoHardwares=Pd.ListOrthoHardwares.FindAll(x=>x.DateExam==comboOrthoDate.GetSelected<DateTime>());
			for(int i=0;i<listOrthoHardwares.Count;i++){
				GridRow row=new GridRow();
				row.Cells.Add(listOrthoHardwares[i].DateExam.ToShortDateString());
				row.Cells.Add(listOrthoHardwares[i].OrthoHardwareType.ToString());
				switch(listOrthoHardwares[i].OrthoHardwareType){
					case EnumOrthoHardwareType.Bracket:
						row.Cells.Add(Tooth.Display(listOrthoHardwares[i].ToothRange,toothNumberingNomenclature));
						break;
					case EnumOrthoHardwareType.Elastic:
						row.Cells.Add(Tooth.DisplayOrthoCommas(listOrthoHardwares[i].ToothRange,toothNumberingNomenclature));
						break;
					case EnumOrthoHardwareType.Wire:
						row.Cells.Add(Tooth.DisplayOrthoDash(listOrthoHardwares[i].ToothRange,toothNumberingNomenclature));
						break;
				}
				OrthoHardwareSpec orthoHardwareSpec=listOrthoHardwareSpecs.Find(x=>x.OrthoHardwareSpecNum==listOrthoHardwares[i].OrthoHardwareSpecNum);
				row.Cells.Add(orthoHardwareSpec.Description);//can't be null
				row.Cells.Add(listOrthoHardwares[i].Note);
				row.Tag=listOrthoHardwares[i];
				gridOrtho.ListGridRows.Add(row);
			}
			gridOrtho.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void FillGridOrthoChart(){
			Cursor=Cursors.WaitCursor;
			gridOrtho.SelectionMode=GridSelectionMode.None;
			//Get all the corresponding fields from the OrthoChartTabLink table that are associated with the currently selected ortho tab.
			OrthoChartTab orthoChartTab=(OrthoChartTab)tabOrthoCategories.SelectedTab.Tag;
			List<DisplayField> listDisplayFieldsAll=DisplayFields.GetForCategory(DisplayFieldCategory.OrthoChart);
			List<string> listDisplayFieldNames=new List<string>();
			for(int i=0;i<listDisplayFieldsAll.Count;i++) {
				listDisplayFieldNames.Add(listDisplayFieldsAll[i].Description);
			}
			List<DisplayField> listDisplayFields=//GetDisplayFieldsForCurrentTab();
				OrthoChartTabLinks.GetWhere(x => x.OrthoChartTabNum==orthoChartTab.OrthoChartTabNum)//Determines the number of items that will be returned
				.OrderBy(x => x.ItemOrder)//Each tab is ordered based on the ortho tab link entry
				.Select(x => listDisplayFieldsAll.FirstOrDefault(y => y.DisplayFieldNum==x.DisplayFieldNum))//Project all corresponding display fields in order
				.Where(x => x!=null)//Can happen when there is an OrthoChartTabLink in the database pointing to an invalid display field.
				.ToList();//Casts the projection to a list of display fields
			int sigColIdx=-1;
			int gridMainScrollValue=gridOrtho.ScrollValue;
			gridOrtho.BeginUpdate();
			//Set the title of the grid to the title of the currently selected ortho chart tab.  This is so that medical users don't see Ortho Chart.
			gridOrtho.Title=orthoChartTab.TabName;
			gridOrtho.Columns.Clear();
			GridColumn col;
			//First column will always be the date.  gridMain_CellLeave() depends on this fact.
			col=new GridColumn(Lan.g(this,"Date"),135);
			gridOrtho.Columns.Add(col);
			for(int i=0;i<listDisplayFields.Count;i++) {
				string columnHeader=listDisplayFields[i].Description;
				if(listDisplayFields[i].DescriptionOverride!=""){
					columnHeader=listDisplayFields[i].DescriptionOverride;
				}
				int colWidth=listDisplayFields[i].ColumnWidth;
				OrthoChartTabLink orthoChartTabLink=OrthoChartTabLinks.GetWhere(x => x.OrthoChartTabNum==orthoChartTab.OrthoChartTabNum 
					&& x.DisplayFieldNum==listDisplayFields[i].DisplayFieldNum).FirstOrDefault();
				if(orthoChartTabLink?.ColumnWidthOverride>0) {
					colWidth=orthoChartTabLink.ColumnWidthOverride;
				}
				if(!string.IsNullOrEmpty(listDisplayFields[i].PickList)) {
					List<string> listComboOptions=listDisplayFields[i].PickList.Split(new string[] { "\r\n" },StringSplitOptions.None).ToList();
					col=new GridColumn(columnHeader,colWidth){ListDisplayStrings=listComboOptions };
				}
				else {
					col=new GridColumn(columnHeader,colWidth,true);
					if(listDisplayFields[i].InternalName=="Signature") {
						sigColIdx=gridOrtho.Columns.Count;
						col.IsEditable=false;
					}
					if(listDisplayFields[i].InternalName=="Provider") {
						col.IsEditable=false;
					}
				}
				col.Tag=listDisplayFields[i].Description;
				gridOrtho.Columns.Add(col);
			}
			gridOrtho.ListGridRows.Clear();
			GridRow row;
			DateTime dateTimeFrom=DateTime.MinValue;
			DateTime dateTimeTo=DateTime.MinValue;
			if(dateTimeTo==DateTime.MinValue) {//Happens on load, when we add a row and reset grid, and when we choose 'All Dates' in the date range picker.
				dateTimeTo=DateTime.MaxValue;
			}
			SignatureBoxWrapper signatureBoxWrapper=new SignatureBoxWrapper();
			signatureBoxWrapper.SignatureMode=SignatureBoxWrapper.SigMode.OrthoChart;
			for(int r=0;r<Pd.ListOrthoChartRows.Count;r++){
				if(!Pd.ListOrthoChartRows[r].DateTimeService.Date.Between(dateTimeFrom,dateTimeTo) ){
					continue;
				}
				if(Pd.ListOrthoChartRows[r].ListOrthoCharts.Count>0){ 
					if(!Pd.ListOrthoChartRows[r].ListOrthoCharts.Any(x => listDisplayFieldNames.Contains(x.FieldName))){
						continue;
					}
				}
				row=new GridRow();
				//Show Date only by default.
				string dateTimeService=Pd.ListOrthoChartRows[r].DateTimeService.ToShortDateString();
				if(Pd.ListOrthoChartRows[r].DateTimeService.TimeOfDay!=TimeSpan.Zero) {
					//Show DateTime if not midnight
					dateTimeService=Pd.ListOrthoChartRows[r].DateTimeService.ToString();
				}
				row.Cells.Add(dateTimeService);
				for(int i=0;i<listDisplayFields.Count;i++) {
					string cellValue="";
					if(listDisplayFields[i].InternalName=="Signature") {
						//handled separately
					}
					else if(listDisplayFields[i].InternalName=="Provider"){
						cellValue=Providers.GetAbbr(Pd.ListOrthoChartRows[r].ProvNum);
					}
					else{
						OrthoChart orthoChart=Pd.ListOrthoCharts.Find(x => x.OrthoChartRowNum==Pd.ListOrthoChartRows[r].OrthoChartRowNum 
							&& x.FieldName==listDisplayFields[i].Description);
						if(orthoChart!=null) {
							cellValue=orthoChart.FieldValue;
						}
					}
					if(!string.IsNullOrEmpty(listDisplayFields[i].PickList)) {
						List<string> listComboOptions=listDisplayFields[i].PickList.Split(new string[] { "\r\n" },StringSplitOptions.None).ToList();
						GridCell cell=new GridCell(cellValue);
						cell.ComboSelectedIndex=listComboOptions.FindIndex(x => x==cellValue);
						row.Cells.Add(cell);
					}
					else {
						row.Cells.Add(cellValue);
					}
				}
				FormOrthoChart.OrthoSignature orthoSignature=new FormOrthoChart.OrthoSignature(Pd.ListOrthoChartRows[r].Signature);
				string sigColumnName="";
				DisplayField displayFieldSig=listDisplayFields.Find(x => x.InternalName=="Signature");
				if(displayFieldSig!=null){
					sigColumnName=displayFieldSig.Description;
				}
				List<OrthoChart> listOrthoCharts=Pd.ListOrthoCharts.FindAll(x => x.OrthoChartRowNum==Pd.ListOrthoChartRows[r].OrthoChartRowNum && x.FieldValue!="" && x.FieldName!=sigColumnName);
				DateTime dateTime=Pd.ListOrthoChartRows[r].DateTimeService;
				string keyData=OrthoCharts.GetKeyDataForSignatureHash(Pd.Patient,listOrthoCharts,dateTime);
				signatureBoxWrapper.FillSignature(orthoSignature.IsTopaz,keyData,orthoSignature.SigString);
				if(orthoSignature.SigString!="") {
					if(!signatureBoxWrapper.IsValid) {
						//This ortho chart may have been signed when we were using the patient name in the hash. Try hashing the signature with the patient name.
						keyData=OrthoCharts.GetKeyDataForSignatureHash(Pd.Patient,listOrthoCharts
							.FindAll(x => x.FieldValue!="" && x.FieldName!=sigColumnName),dateTime,doUsePatName:true);
						signatureBoxWrapper.FillSignature(orthoSignature.IsTopaz,keyData,orthoSignature.SigString);
					}
					if(signatureBoxWrapper.IsValid) {
						row.ColorBackG=Color.FromArgb(220,255,225);//pale green
						if(sigColIdx > 0) {//User might be viewing a tab that does not have the signature column.  Greater than 0 because index 0 is a Date column.
							row.Cells[sigColIdx].Text=Lan.g(this,"Signed");
						}
					}
					else {
						row.ColorBackG=Color.FromArgb(255,220,220);//pale pink
						if(sigColIdx > 0) {//User might be viewing a tab that does not have the signature column.  Greater than 0 because index 0 is a Date column.
							row.Cells[sigColIdx].Text=Lan.g(this,"Invalid");
						}
					}
				}
				if(!row.Cells.Skip(1).All(x => string.IsNullOrWhiteSpace(x.Text))) {
					//Skip the first cell in row(Date field). If at least one of the other cells has a value, then add the row.
					gridOrtho.ListGridRows.Add(row);
				}
			}
			gridOrtho.EndUpdate();
			signatureBoxWrapper?.Dispose();
			if(gridMainScrollValue==0) {
				gridOrtho.ScrollToEnd();
			}
			else {
				gridOrtho.ScrollValue=gridMainScrollValue;
				gridMainScrollValue=0;
			}
			Cursor=Cursors.Default;
		}

		private void gridOrtho_DoubleClick(object sender,EventArgs e) {
			//this is not a CellDoubleClick. They can click in the area below the rows.
			int idx=tabOrthoCategories.SelectedIndex;
			if(idx==0){
				return;//clicks in the hardware grid use CellDoubleClick
			}
			using FormOrthoChart formOrthoChart=new FormOrthoChart(Pd.Patient,idx-1);
			formOrthoChart.ShowDialog();
			ModuleSelected(Pd.PatNum);
		}

		private void gridOrtho_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int idx=tabOrthoCategories.SelectedIndex;
			if(idx>0){
				return;//tabs other than the hardware grid use DoubleClick
			}
			using FormOrthoHardwareEdit formOrthoHardwareEdit=new FormOrthoHardwareEdit();
			formOrthoHardwareEdit.OrthoHardwareCur=(OrthoHardware)gridOrtho.ListGridRows[e.Row].Tag;
			formOrthoHardwareEdit.ShowDialog();
			ModuleSelected(Pd.PatNum);
		}

		private void MenuItemOrthoDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			for(int i=0;i<gridOrtho.SelectedIndices.Length;i++){
				OrthoHardware orthoHardware=(OrthoHardware)gridOrtho.ListGridRows[gridOrtho.SelectedIndices[i]].Tag;
				OrthoHardwares.Delete(orthoHardware.OrthoHardwareNum);
			}
			ModuleSelected(Pd.PatNum);
		}
		#endregion Methods - Private - Tab Ortho

		#region Classes - Nested
		///<summary>Class that holds a DataRow along with the index of the table it is a part of.</summary>
		private class DataRowWithIdx {
			public DataRow Row;
			public int Index;

			public DataRowWithIdx(DataRow row,int index) {
				Row=row;
				Index=index;
			}
		}

		///<summary>Each item can be either a Doc or a Mount</summary>
		private class ImageInfo{
			public long DocNum;
			public long MountNum;
			public DateTime DateCreated;
		}







		#endregion Classes - Nested

		
	}//end class

	#region Class - ODDataRowComparer
	/// <summary>Compares two given DataRows by their associated row types and PK column.</summary>
	public class ODDataRowComparer : IEqualityComparer<DataRow> {
		public bool Equals(DataRow x,DataRow y) {
			ProgNotesRowType rowTypeX=ChartModules.GetRowType(x,out long rowXPK);
			ProgNotesRowType rowTypeY=ChartModules.GetRowType(y,out long rowYPK);
			return (rowTypeX==rowTypeY && rowXPK==rowYPK);
		}

		public int GetHashCode(DataRow obj) {
			return 0;
		}
	}
	#endregion Class - ODDataRowComparer


}





