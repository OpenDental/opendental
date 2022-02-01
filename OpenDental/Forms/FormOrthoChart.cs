using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoChart:FormODBase {
		private PatField[] _arrayPatientFields;
		private DateTime _dateTimeFrom;
		private DateTime _dateTimeTo;
		private List<DisplayField> _listOrthDisplayFields;
		private Patient _patCur;
		private PatientNote _patNoteCur;
		private List<string> _listDisplayFieldNames;
		///<summary>Set to true if any data changed within the grid.</summary>
		private bool _hasChanged;
		///<summary>Each item is a grid row.  Some rows can have all empty cells.  Rows include db cells as well as what the user has added and edited since loading. Rows are sorted by date and should be resorted whenever manipulated.</summary>
		private List<OrthoChartRow> _listOrthoChartRows;
		///<summary>True if there are any ortho display fields with the internal name of "Signature"</summary>
		private bool _showSigBox;
		///<summary>Keeps track of the column index of the Signature field if one is present.
		///This column index represents the the index of the grid that is displayed to the user, not the index within _tableOrtho.</summary>
		private int _sigColIdx=-1;
		///<summary>Keeps track of the column index within _tableOrtho of the Signature field if one is present.</summary>
		private int _sigTableOrthoColIdx=-1;
		///<summary>The index of the previously selected ortho chart row.  This is used to help save data.</summary>
		private int _prevRow;
		private bool _topazNeedsSaved;
		private int _indexInitialTab;
		///<summary>Keeps track of the dates that can be edited by the user currently logged in.  This is simply to save database calls.</summary>
		private Dictionary<DateTime,bool> _dictCanEditDay=new Dictionary<DateTime, bool>();
		///<summary>Users can temporarily log in on this form.  Defaults to Security.CurUser.</summary>
		private Userod _curUser=Security.CurUser;
		///<summary>UserNum that has locked this form. Should match database.</summary>
		private long _userNumLocked;
		///<summary>This instance of OD has an ortho lock.  _userNumLocked might still be current user on a different instance of OD.</summary>
		private bool _isLockedByMe;
		///<summary>True if user has been changed locally on this form. Used for signature box.</summary>
		private bool _changedUser;

		public FormOrthoChart(Patient patCur) : this(patCur,0) {
		}

		///<summary>Opens this patient's ortho chart with a specific chart tab opened.  tabIndex correlates to the index of OrthoChartTabs.Listt.</summary>
		public FormOrthoChart(Patient patCur,int tabIndex) {
			_patCur=patCur;
			_patNoteCur=PatientNotes.Refresh(_patCur.PatNum,_patCur.Guarantor);
			_prevRow=-1;
			InitializeComponent();
			InitializeLayoutManager();
			_indexInitialTab=tabIndex;
			Lan.F(this);
		}

		private void FormOrthoChart_Load(object sender,EventArgs e) {
			FillLockedInitial();
			signatureBoxWrapper.SetAllowDigitalSig(true);
			_listOrthoChartTabs=OrthoChartTabs.GetDeepCopy(true);
			LayoutMenu();
			FillTabs();
			FillDisplayFields();
			_listOrthoChartRows=OrthoChartRows.GetAllForPatient(_patCur.PatNum);
			if(_userNumLocked==_curUser.UserNum){//we will usually have control
				OrthoChartRow orthoChartRowToday=_listOrthoChartRows.FirstOrDefault(x=>x.DateTimeService.Date==DateTime.Today.Date);
				if(orthoChartRowToday==null
					|| !orthoChartRowToday.ListOrthoCharts.Any(x => ListTools.In(x.FieldName,_listDisplayFieldNames)))//Orthocharts fieldnames are not showing
				{
					OrthoChartRow orthoChartRow=new OrthoChartRow();
					orthoChartRow.PatNum=_patCur.PatNum;
					orthoChartRow.ProvNum=_patCur.PriProv;
					orthoChartRow.DateTimeService=DateTime.Today;
					orthoChartRow.UserNum=_curUser.UserNum;
					orthoChartRow.Signature="";
					orthoChartRow.IsNew=true;
					OrthoChartRows.Insert(orthoChartRow);
					_listOrthoChartRows.Add(orthoChartRow);
				}
			}
			_listOrthoChartRows=_listOrthoChartRows.OrderBy(x=>x.DateTimeService).ToList();
			FillDateRange();
			if(_indexInitialTab!=tabControl.SelectedIndex) {
				tabControl.SelectedIndex=_indexInitialTab;//triggers FillGrid()
			}
			else {//Tab index hasn't changed, fill the grid.
				Logger.LogAction("FormOrthoChart_Load",LogPath.OrthoChart,() => {
					FillGrid();
				},"FillGrid");
			}
			FillGridPat();
			if(PrefC.GetBool(PrefName.OrthoCaseInfoInOrthoChart)) {
				gridOrtho.Visible=true;
				FillOrtho();
			}
			textUser.Text="";//User text box should be blank until an orthochart is selected.
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",menuItemSetup_Click));
			menuMain.EndUpdate();
		}

		#region Methods - Lock
		private void FillLockedInitial(){
			_userNumLocked=PatientNotes.GetUserNumOrthoLocked(_patCur.PatNum);
			if(_userNumLocked==0){
				//Nobody else has control, so we will take control.
				_isLockedByMe=true;//Set _isLockedByMe to true in case another instance of OD with the same userNum tries to take control.
				_userNumLocked=_curUser.UserNum;
				buttonTakeControl.Visible=false;//No need to display take control button if we are in control.
				labelLocked.Text="Locked by: "+Userods.GetName(_userNumLocked);
				PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,_curUser.UserNum);
			}
			else{
				//The lock could be by any user, including the current user on a different instance of OD.
				LockControls();
				labelLocked.Text="Locked by: "+Userods.GetName(_userNumLocked);
				if(ODBuild.IsDebug()) {
					//Helpful to know who when you have multiple windows open for testing.
					MsgBox.Show(this,Userods.GetName(_userNumLocked)+Lan.g(this," has already locked this ortho chart for editing."));
				}
				else {
					MsgBox.Show(this,"Another user has already locked this ortho chart for editing.");
				}
			}
		}

		///<summary>There are 4 possible states. userNum checks the DB every 4 seconds.
		/// 1. userNum = -5. A user changed users locally on their form to sign, do nothing.
		/// 2. userNum = _userNumLocked. Current userNum is in control, do nothing.
		/// 3. userNum = 0. An instance of OD has closed their form which sets the DB state to 0. (Locked by: Nobody)
		/// 4. userNum = anything else. 2 different users are attempting to edit the form and a different user took control.
		/// Note: userNum is set to -1 when another user with the same username took control. This is handled the same as userNum = anything else.</summary>
		private void timerLocked_Tick(object sender, EventArgs e){
			//Every 4 seconds.
			long userNum=PatientNotes.GetUserNumOrthoLocked(_patCur.PatNum);
			if(userNum==-5){//-5 indicates that an instance of OD changed users locally to that form.
				return;
			}
			if(userNum==_userNumLocked){//There's been no change.
				return;
			}
			if(userNum==0){//Someone else just closed their ortho chart for this patient.
				LockControls();//Allows ability to take control when the same user is logged in multiple instances of OD and one closes the form.
				//We don't want to automatically take control because if we are one of multiple waiters,
				//we would all grab control at the same time.  User will need to click button to take control.
				labelLocked.Text="Locked by: Nobody";
				_userNumLocked=0;
				//They already saved their changes, so we can refresh immediately.
				//Refresh this form with the changed data from the other user
				_listOrthoChartRows=OrthoChartRows.GetAllForPatient(_patCur.PatNum);
				_listOrthoChartRows=_listOrthoChartRows.OrderBy(x=>x.DateTimeService).ToList();
				FillGrid();
				FillGridPat();
				//Ortho grid is just read only.
				return;
			}
			//Someone just took control.
			//Programmatically force the ActiveControl to null just in case the user was in the middle of typing into the grid.
			//This will cause event handlers to be invoked (e.g. gridMain_CellLeave) which will save the work that the user was just in the middle of doing.
			ActiveControl=null;
			//They might have taken control from us if we had control.
			_isLockedByMe=false;//Another user took control.
			SaveToDb();//so that other user can see our changes.
			//Remember that the above changes can happen up to 4 seconds after the other user took control
			_listOrthoChartRows=OrthoChartRows.GetAllForPatient(_patCur.PatNum);
			_listOrthoChartRows=_listOrthoChartRows.OrderBy(x=>x.DateTimeService).ToList();
			FillGrid();//This will show us true db state, including deleting any empty row.
			LockControls();
			labelLocked.Text="Locked by: "+Userods.GetName(userNum);
			_userNumLocked=userNum;
			if(ODBuild.IsDebug()) {
				//Helpful to know who when you have multiple windows open for testing.
				MsgBox.Show(this,Userods.GetName(userNum)+Lan.g(this," has taken control. If you made any changes, they were saved."));
			}
			else{
				MsgBox.Show(this,Lan.g(this,"Another user just took control.  If you had made any changes, they were saved."));
			}
		}

		private void LockControls(){
			gridMain.AllowSelection=false;
			gridPat.AllowSelection=false;
			gridOrtho.AllowSelection=false;
			butAdd.Visible=false;
			butUseAutoNote.Visible=false;
			butDelete.Visible=false;
			butOK.Enabled=false;
			buttonTakeControl.Visible=true;
		}

		private void UnlockControls(){
			gridMain.AllowSelection=true;
			gridPat.AllowSelection=true;
			gridOrtho.AllowSelection=true;
			butAdd.Visible=true;
			butUseAutoNote.Visible=true;
			butDelete.Visible=true;
			butOK.Enabled=true;
			buttonTakeControl.Visible=false;
		}

		private void buttonTakeControl_Click(object sender, EventArgs e){
			//It's been a few seconds since we looked at the db, so check again
			long userNum=PatientNotes.GetUserNumOrthoLocked(_patCur.PatNum);
			if(userNum==-5) {//We are taking control from a workstation that changed users to sign an orthochartrow.
				userNum=_userNumLocked;//Change the userNum back to the user that was in control prior to changing users on other instance of OD.
			}
			if(userNum!=_userNumLocked){
				//Someone else grabbed control in the last few seconds, so we don't want to immediately grab it.
				//Controls are already still locked.
				//If User8 still has control on different instance, and if User8 is trying to take control on this instance, 
				//then the first User8 will still have the lock and this won't be hit. (8 != 8)
				string name=Userods.GetName(userNum);
				//this would happen in a moment with the timer, but we want to show it now.
				labelLocked.Text=name;
				_userNumLocked=userNum;
				MsgBox.Show(this,"Another user just took control.  If you want to take control from them, try again.");
				return;
			}
			if(userNum==_curUser.UserNum) {//Current user already has lock on different workstation.
				PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,-1);//Set lock to -1 momentarily.
				Cursor=Cursors.WaitCursor;
				System.Threading.Thread.Sleep(5500);
				//continue below
			}
			_isLockedByMe=true;
			//take control
			PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,_curUser.UserNum);
			_userNumLocked=_curUser.UserNum;			
			labelLocked.Text="Locked by: "+Userods.GetName(_curUser.UserNum);
			//Refresh this form with the changed data from the other user
			//It could take 4 seconds for other user to save their changes, which we need before we can take control
			Cursor=Cursors.WaitCursor;
			//They won't mind waiting.  This will be fairly rare.
			System.Threading.Thread.Sleep(5500);
			//We already sent the message, so if we allow cancel, we would still need to wait to refresh.
			_listOrthoChartRows=OrthoChartRows.GetAllForPatient(_patCur.PatNum);
			_listOrthoChartRows=_listOrthoChartRows.OrderBy(x=>x.DateTimeService).ToList();
			FillGrid();
			FillGridPat();
			//User can add their own new row if they want.
			Cursor=Cursors.Default;
			UnlockControls();
		}
		#endregion Methods - Lock

		///<summary>Sets the title of the form to the TabName of the first tab in the cache and then refreshes the current tabs.</summary>
		private void FillTabs() {
			//Set the title of this form to the first tab in the list.  The button to launch the Ortho Chart from the Chart module will be the same.
			Text=_listOrthoChartTabs[0].TabName;//It is considered database corruption if this fails.
			OrthoChartTab orthoChartTabSelected=null;
			if(tabControl.SelectedIndex >= 0) {
				orthoChartTabSelected=(OrthoChartTab)tabControl.TabPages[tabControl.SelectedIndex].Tag;
			}
			tabControl.TabPages.Clear();
			for(int i=0;i<_listOrthoChartTabs.Count;i++) {
				TabPage tabPage=new TabPage(_listOrthoChartTabs[i].TabName);
				tabPage.Tag=_listOrthoChartTabs[i];
				LayoutManager.Add(tabPage,tabControl);
				if(orthoChartTabSelected!=null && _listOrthoChartTabs[i].OrthoChartTabNum==orthoChartTabSelected.OrthoChartTabNum) {
					tabControl.SelectedIndex=i;
				}
			}
			if(tabControl.SelectedIndex==-1) {
				tabControl.SelectedIndex=0;
			}
		}

		///<summary>Fills the classwide fields related to Display Fields.</summary>
		private void FillDisplayFields() {
			_listOrthDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.OrthoChart);//List of display fields and the order they should be displayed in.
			_listDisplayFieldNames=new List<string>();
			for(int i=0;i<_listOrthDisplayFields.Count;i++) {
				if(_listOrthDisplayFields[i].InternalName=="Signature") {
					_sigTableOrthoColIdx=i;
				}
				_listDisplayFieldNames.Add(_listOrthDisplayFields[i].Description);
			}
		}

		///<summary>This does NOT get fresh data from db. The data is always in _listOrthoChartRows and its item ListOrthoCharts (cells).  This method just shows that data differently (by tab, for example).</summary>
//todo: there was a warning on this method that I do not understand: Do not call unless you have saved changes to database first.
		private void FillGrid() {
			if(tabControl.SelectedIndex==-1) {
				return;//This happens when the tab pages are cleared (updating the selected index).
			}
			Cursor=Cursors.WaitCursor;
			//Get all the corresponding fields from the OrthoChartTabLink table that are associated with the currently selected ortho tab.
			OrthoChartTab orthoChartTab=_listOrthoChartTabs[tabControl.SelectedIndex];
			List<DisplayField> listDisplayFields=GetDisplayFieldsForCurrentTab();
			_sigColIdx=-1;//Clear out the signature column index cause it will most likely change or disappear (switching tabs)
			int gridMainScrollValue=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			//Set the title of the grid to the title of the currently selected ortho chart tab.  This is so that medical users don't see Ortho Chart.
			gridMain.Title=orthoChartTab.TabName;
			gridMain.Columns.Clear();
			GridColumn col;
			//First column will always be the date.  gridMain_CellLeave() depends on this fact.
			col=new GridColumn(Lan.g(this,"Date"),135);
			gridMain.Columns.Add(col);
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
						_sigColIdx=gridMain.Columns.Count;
						//col.TextAlign=HorizontalAlignment.Center;
						col.IsEditable=false;
					}
					if(listDisplayFields[i].InternalName=="Provider") {
						col.IsEditable=false;
					}
				}
				col.Tag=listDisplayFields[i].Description;
				gridMain.Columns.Add(col);
			}
			_showSigBox=_listOrthDisplayFields.Any(x => x.InternalName=="Signature");//Must be set before CanEditRow(...)
			gridMain.ListGridRows.Clear();
			GridRow row;
			DateTime dateTimeFrom=_dateTimeFrom;
			DateTime dateTimeTo=_dateTimeTo;
			if(dateTimeTo==DateTime.MinValue) {//Happens on load, when we add a row and reset grid, and when we choose 'All Dates' in the date range picker.
				dateTimeTo=DateTime.MaxValue;
			}
			for(int r=0;r<_listOrthoChartRows.Count;r++){
				if(!_listOrthoChartRows[r].DateTimeService.Date.Between(dateTimeFrom,dateTimeTo) 
					|| (_listOrthoChartRows[r].ListOrthoCharts.Count>0 && !_listOrthoChartRows[r].ListOrthoCharts.Any(x => ListTools.In(x.FieldName,_listDisplayFieldNames))))
				{
					continue;
				}
				row=new GridRow();
				//Show Date only by default.
				string dateTimeService=_listOrthoChartRows[r].DateTimeService.ToShortDateString();
				if(_listOrthoChartRows[r].DateTimeService.TimeOfDay!=TimeSpan.Zero) {
					//Show DateTime if not midnight
					dateTimeService=_listOrthoChartRows[r].DateTimeService.ToString();
				}
				row.Cells.Add(dateTimeService);
				for(int i=0;i<listDisplayFields.Count;i++) {
					string cellValue="";
					if(listDisplayFields[i].InternalName=="Signature") {
						//handled separately
					}
					else if(listDisplayFields[i].InternalName=="Provider"){
						cellValue=Providers.GetAbbr(_listOrthoChartRows[r].ProvNum);
					}
					else{
						OrthoChart orthoChart=_listOrthoChartRows[r].ListOrthoCharts.FirstOrDefault(x=>x.FieldName==listDisplayFields[i].Description);
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
				if(_listOrthoChartRows[r].IsNew || !row.Cells.Skip(1).All(x => string.IsNullOrWhiteSpace(x.Text))) {
					//Skip the first cell in row(Date field) and at least one cell has a value or the row was added today.
					row.Tag=_listOrthoChartRows[r];
					gridMain.ListGridRows.Add(row);
				}
				CanEditRow(_listOrthoChartRows[r].DateTimeService);//Function uses _showSigBox, must be set prior to calling.
			}
			gridMain.EndUpdate();
			if(gridMainScrollValue==0) {
				gridMain.ScrollToEnd();
			}
			else {
				gridMain.ScrollValue=gridMainScrollValue;
				gridMainScrollValue=0;
			}
			//Show the signature control if a signature display field is present on any tab.
			signatureBoxWrapper.Visible=_showSigBox;//Hide the signature box if this tab does not have the signature column present.
			labelUser.Visible=_showSigBox;
			textUser.Visible=_showSigBox;
			butChangeUser.Visible=_showSigBox;
			if(_showSigBox) {
				for(int i=0;i<gridMain.ListGridRows.Count;i++) {
					DisplaySignature(i,false);
				}
				signatureBoxWrapper.ClearSignature(false);
				signatureBoxWrapper.Enabled=false;//We don't want it to be enabled unless the user has clicked on a row.
				_prevRow=-1;
			}
			Cursor=Cursors.Default;
		}

		private void FillGridPat() {
			gridPat.BeginUpdate();
			gridPat.Columns.Clear();
			GridColumn col;
			col=new GridColumn("Field",150);
			gridPat.Columns.Add(col);
			col=new GridColumn("Value",200);
			gridPat.Columns.Add(col);
			gridPat.ListGridRows.Clear();
			_arrayPatientFields=PatFields.Refresh(_patCur.PatNum);
			PatFieldDefs.RefreshCache();
			PatFieldL.AddPatFieldsToGrid(gridPat,_arrayPatientFields.ToList(),FieldLocations.OrthoChart);
			gridPat.EndUpdate();
		}

		///<summary>Same as FillOrtho() in the ContrAccount.</summary>
		private void FillOrtho() {
			gridOrtho.BeginUpdate();
			gridOrtho.Columns.Clear();
			gridOrtho.Columns.Add(new GridColumn("",(gridOrtho.Width/2)-20,HorizontalAlignment.Right));
			gridOrtho.Columns.Add(new GridColumn("",(gridOrtho.Width/2)+20,HorizontalAlignment.Left));
			gridOrtho.ListGridRows.Clear();
			GridRow row = new GridRow();
			DateTime firstOrthoProc = Procedures.GetFirstOrthoProcDate(_patNoteCur);
			if(firstOrthoProc!=DateTime.MinValue) {
				row.Cells.Add(Lan.g(this,"Total Tx Time")+": "); //Number of Years/Months/Days since the first ortho procedure on this account
				DateSpan dateSpan=new DateSpan(firstOrthoProc,DateTime.Today);
				string strDateDiff="";
				if(dateSpan.YearsDiff!=0) {
					strDateDiff+=dateSpan.YearsDiff+" "+Lan.g(this,"year"+(dateSpan.YearsDiff==1 ? "" : "s"));
				}
				if(dateSpan.MonthsDiff!=0) {
					if(strDateDiff!="") {
						strDateDiff+=", ";
					}
					strDateDiff+=dateSpan.MonthsDiff+" "+Lan.g(this,"month"+(dateSpan.MonthsDiff==1 ? "" : "s"));
				}
				if(dateSpan.DaysDiff!=0 || strDateDiff=="") {
					if(strDateDiff!="") {
						strDateDiff+=", ";
					}
					strDateDiff+=dateSpan.DaysDiff+" "+Lan.g(this,"day"+(dateSpan.DaysDiff==1 ? "" : "s"));
				}
				row.Cells.Add(strDateDiff);
				gridOrtho.ListGridRows.Add(row);
				row = new GridRow();
				row.Cells.Add(Lan.g(this,"Date Start")+": "); //Date of the first ortho procedure on this account
				row.Cells.Add(firstOrthoProc.ToShortDateString());
				gridOrtho.ListGridRows.Add(row);

				row = new GridRow();
				row.Cells.Add(Lan.g(this,"Tx Months Total")+": "); //this patient's OrthoClaimMonthsTreatment, or the practice default if 0.
				int txMonthsTotal=(_patNoteCur.OrthoMonthsTreatOverride==-1?PrefC.GetByte(PrefName.OrthoDefaultMonthsTreat):_patNoteCur.OrthoMonthsTreatOverride);
				row.Cells.Add(txMonthsTotal.ToString());
				gridOrtho.ListGridRows.Add(row);

				row = new GridRow();
				int txTimeInMonths=dateSpan.YearsDiff * 12 + dateSpan.MonthsDiff + (dateSpan.DaysDiff < 15? 0: 1);
				row.Cells.Add(Lan.g(this,"Months in Treatment")+": "); //idk what the difference between this and 'Total Tx Time' is.
				row.Cells.Add(txTimeInMonths.ToString());
				gridOrtho.ListGridRows.Add(row);

				row = new GridRow();
				row.Cells.Add(Lan.g(this,"Months Rem")+": "); //Months Total - Total Tx Time
				row.Cells.Add(Math.Max(0,txMonthsTotal-txTimeInMonths).ToString());
				gridOrtho.ListGridRows.Add(row);

			}
			else {
				row = new GridRow();
				row.Cells.Add(""); //idk what the difference between this and 'Total Tx Time' is.
				row.Cells.Add(Lan.g(this,"No ortho procedures charted")+".");
				gridOrtho.ListGridRows.Add(row);
			}
			gridOrtho.EndUpdate();
		}

		///<summary>This method is used to set the Date Range filter start and stop dates for gridMain.</summary>
		private void FillDateRange() {
			textDateRange.Text="";
			if(_dateTimeFrom.Year>1880) {
				textDateRange.Text+=_dateTimeFrom.ToShortDateString();
			}
			if(_dateTimeTo.Year>1880 && _dateTimeFrom!=_dateTimeTo) {
				if(textDateRange.Text!="") {
					textDateRange.Text+="-";
				}
				textDateRange.Text+=_dateTimeTo.ToShortDateString();
			}
			if(textDateRange.Text=="") {
				textDateRange.Text=Lan.g(this,"All Dates");
			}
		}


		private void menuItemSetup_Click(object sender,EventArgs e) {
			using FormDisplayFieldsOrthoChart form=new FormDisplayFieldsOrthoChart();
			if(form.ShowDialog()==DialogResult.OK) {
				FillTabs();
				FillDisplayFields();
				Logger.LogAction("menuItemSetup_Click",LogPath.OrthoChart,() => {
					FillGrid();
				},"FillGrid");
			}
		}

		private void tabControl_TabIndexChanged(object sender,EventArgs e) {
			//This fires when user clicks and also when load.
			Logger.LogAction("tabControl_TabIndexChanged",LogPath.OrthoChart,() => {
				FillGrid();
			},"FillGrid");
		}

		private void signatureBoxWrapper_ClearSignatureClicked(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==-1) {
				return;
			}
			ClearSignature(gridMain.SelectedCell.Y);
		}

		private void ClearSignature(int idxRow) {
			OrthoChartRow orthoChartRow=(OrthoChartRow)gridMain.ListGridRows[idxRow].Tag;
			if(OrthoSignature.GetSigString(orthoChartRow.Signature)!="") {
				_hasChanged=true;
			}
			orthoChartRow.Signature="";
			DisplaySignature(idxRow);
			_prevRow=idxRow;
		}

		private void signatureBoxWrapper_SignTopazClicked(object sender,EventArgs e) {
			if(gridMain.SelectedCell.Y==-1) {
				return;
			}
			if(OrthoSignature.GetSigString(((OrthoChartRow)gridMain.ListGridRows[gridMain.SelectedCell.Y].Tag).Signature)!="") {
				_hasChanged=true;
			}
			((OrthoChartRow)gridMain.ListGridRows[gridMain.SelectedCell.Y].Tag).Signature="";
			_prevRow=gridMain.SelectedCell.Y;
			_topazNeedsSaved=true;
		}

		///<summary>Displays the signature for this row. The gridMain_CellEnter event does not fire when the column is not editable.</summary>
		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			SaveAndSetSignatures(e.Row);
			if(textUser.Text!=_curUser.UserName || !_isLockedByMe) {//Don't allow signature to be overwritten if someone else previously signed without first changing users.
				signatureBoxWrapper.Enabled=false;
			}
		}

		///<summary>This is necessary in addition to CellClick for when the user tabs or uses the arrow keys to enter a cell.</summary>
		private void gridMain_CellEnter(object sender,ODGridClickEventArgs e) {
			Logger.LogAction("gridMain_CellEnter",LogPath.OrthoChart,() => {
				SaveAndSetSignatures(e.Row);
			});
		}

		///<summary>Saves the signature to the data table if it hasn't been and displays the signature for this row.</summary>
		private void SaveAndSetSignatures(int currentRow) {
			if(!_showSigBox || currentRow<0) {
				return;
			}
			SetSignatureButtonVisibility(currentRow);
			//Try and save the previous row's signature if needed.
			SaveSignatureToDict(_prevRow);
			DisplaySignature(_prevRow);
			if(_topazNeedsSaved) {
				_topazNeedsSaved=false;
			}
			//Now that any previous sigs have been saved, display the current row's signature.
			DisplaySignature(currentRow);
			EnableSignatureBox(_curUser);
			_prevRow=currentRow;
		}

		private void gridMain_CellLeave(object sender,ODGridClickEventArgs e) {
			Logger.LogAction("gridMain_CellLeave",LogPath.OrthoChart,() => {
				OrthoChartRow orthoChartRow=(OrthoChartRow)gridMain.ListGridRows[e.Row].Tag;
				//Get the date for the ortho chart that was just edited.
				DateTime dateTime=orthoChartRow.DateTimeService;
				long provNum=orthoChartRow.ProvNum;
				string oldText=GetValueFromList(orthoChartRow,(string)gridMain.Columns[e.Col].Tag);
				string newText=gridMain.ListGridRows[e.Row].Cells[e.Col].Text;
				if(CanEditRow(dateTime)) {
					if(newText != oldText) {
						SetValueInList(orthoChartRow,newText,(string)gridMain.Columns[e.Col].Tag);
						//Cannot be placed in if statement below as we only want to clear the signature when the grid text has changed.
						//We cannot use a textchanged event to call the .dll as this causes massive slowness for certain customers.
						if(_showSigBox) {
							Logger.LogAction("gridMain_CellLeave",LogPath.OrthoChart,() => { signatureBoxWrapper.ClearSignature(false); },"signature.ClearSignature");
						}
						_hasChanged=true;//They had permission and they made a change.
					}
					if(_showSigBox) {
						SaveSignatureToDict(e.Row);
						DisplaySignature(e.Row);
					}
				}
				else {
					//User is not authorized to edit this cell.  Check if they changed the old value and if they did, put it back to the way it was and warn them about security.
					if(newText!=oldText) {
						//The user actually changed the cell's value and we need to change it back and warn them that they don't have permission.
						gridMain.ListGridRows[e.Row].Cells[e.Col].Text=oldText;
						gridMain.Invalidate();
						MsgBox.Show(this,"You need either Ortho Chart Edit (full) or Ortho Chart Edit (same user, signed) to edit this ortho chart.");
					}
				}
			});
		}
		
		///<summary>Displays the signature that is saved in the dictionary in the signature box. Colors the grid row green if the signature is valid, 
		///red if invalid, or white if blank. Puts "Valid" or "Invalid" in the grid's signature column.</summary>
		private void DisplaySignature(int idxRow,bool hasRefresh=true) {
			Logger.LogAction("DisplaySignature",LogPath.OrthoChart,() => {
				textUser.Text=_curUser.UserName;
				if(!_showSigBox || idxRow<0) {
					return;
				}
				OrthoChartRow orthoChartRow=(OrthoChartRow)gridMain.ListGridRows[idxRow].Tag;
				DateTime dateTime=orthoChartRow.DateTimeService;
				List<OrthoChart> listOrthoCharts=orthoChartRow.ListOrthoCharts;
				OrthoSignature orthoSignature=new OrthoSignature(orthoChartRow.Signature);
				if(orthoSignature.SigString=="") {
					Logger.LogAction("DisplaySignature",LogPath.OrthoChart,() => { signatureBoxWrapper.ClearSignature(false); },"signature.ClearSignature2");
					gridMain.ListGridRows[idxRow].ColorBackG=SystemColors.Window;
					//Empty out the signature column displaying to the user.
					if(_sigColIdx > 0) {//User might be vieweing a tab that does not have the signature column.  Greater than 0 because index 0 is a Date column.
						gridMain.ListGridRows[idxRow].Cells[_sigColIdx].Text="";
					}
					if(hasRefresh) {
						gridMain.Refresh();
					}
					return;
				}
				//Get the "translated" name for the signature column.
				string sigColumnName=_listOrthDisplayFields.FirstOrDefault(x => x.InternalName=="Signature")?.Description??"";
				string keyData=OrthoCharts.GetKeyDataForSignatureHash(_patCur,listOrthoCharts
				.FindAll(x => (x.DateService==dateTime || x.DateService==DateTime.MinValue) && x.FieldValue!="" && x.FieldName!=sigColumnName),dateTime);
				Logger.LogAction("DisplaySignature",LogPath.OrthoChart,() => {
					signatureBoxWrapper.FillSignature(orthoSignature.IsTopaz,keyData,orthoSignature.SigString);
				},"signature.FillSignature1 IsTopaz="+(orthoSignature.IsTopaz ? "true" : "false"));
				if(!signatureBoxWrapper.IsValid) {
					//This ortho chart may have been signed when we were using the patient name in the hash. Try hashing the signature with the patient name.
					keyData=OrthoCharts.GetKeyDataForSignatureHash(_patCur,listOrthoCharts
						.FindAll(x => x.DateService==dateTime && x.FieldValue!="" && x.FieldName!=sigColumnName),dateTime,doUsePatName:true);
					Logger.LogAction("DisplaySignature",LogPath.OrthoChart,() => {
						signatureBoxWrapper.FillSignature(orthoSignature.IsTopaz,keyData,orthoSignature.SigString);
					},"signature.FillSignature2 IsTopaz="+(orthoSignature.IsTopaz ? "true" : "false"));
				}
				if(signatureBoxWrapper.IsValid) {
					gridMain.ListGridRows[idxRow].ColorBackG=Color.FromArgb(0,245,165);//A lighter version of Color.MediumSpringGreen
					if(_sigColIdx > 0) {//User might be vieweing a tab that does not have the signature column.  Greater than 0 because index 0 is a Date column.
						gridMain.ListGridRows[idxRow].Cells[_sigColIdx].Text=Lan.g(this,"Valid");
						if(orthoSignature.SigString=="") {
							textUser.Text="";
						}
						else{
							long userNum=orthoChartRow.UserNum;
							textUser.Text=Userods.GetName(userNum);
						}
					}
				}
				else {
					gridMain.ListGridRows[idxRow].ColorBackG=Color.FromArgb(255,140,143);//A darker version of Color.LightPink
					if(_sigColIdx > 0) {//User might be vieweing a tab that does not have the signature column.  Greater than 0 because index 0 is a Date column.
						gridMain.ListGridRows[idxRow].Cells[_sigColIdx].Text=Lan.g(this,"Invalid");
					}
				}
				if(hasRefresh) {
					gridMain.Refresh();
				}
			});
		}

		///<summary>Removes the Sign Topaz button and the Clear Signature button from the signature box if the user does not have OrthoChartEdit permissions for that date.</summary>
		private void SetSignatureButtonVisibility(int idxRow) {
			DateTime dateTime=((OrthoChartRow)gridMain.ListGridRows[idxRow].Tag).DateTimeService;
			signatureBoxWrapper.SetButtonVisibility(CanEditRow(dateTime));
		}

		///<summary>Saves the signature to the dictionary. The signature is hashed using the patient name, the date of service, and all ortho chart fields (even the ones not showing).</summary>
		private void SaveSignatureToDict(int idxRow) {
			if(!_showSigBox || idxRow<0) {
				return;
			}
			OrthoChartRow orthoChartRow=(OrthoChartRow)gridMain.ListGridRows[idxRow].Tag;
			DateTime dateTime=orthoChartRow.DateTimeService;
			if(_changedUser) {//Changed user to sign signature box, so update the selected orthochart row to the changed user's UserNum.
				orthoChartRow.UserNum=_userNumLocked;
				_changedUser=false;
			}
			long provNum=orthoChartRow.ProvNum;
			if(!CanEditRow(dateTime)) {
				return;
			}			
			if(!signatureBoxWrapper.GetSigChanged() || !signatureBoxWrapper.IsValid) {
				return;
			}
			string keyData;
			//Get the "translated" name for the signature column.
			string sigColumnName=_listOrthDisplayFields.FirstOrDefault(x => x.InternalName=="Signature")?.Description??"";
			List<OrthoChart> listOrthoCharts=orthoChartRow.ListOrthoCharts.FindAll(x => x.FieldName!=sigColumnName && x.FieldValue!="");
			keyData=OrthoCharts.GetKeyDataForSignatureSaving(listOrthoCharts,dateTime);
			OrthoSignature sig=new OrthoSignature();
			sig.IsTopaz=signatureBoxWrapper.GetSigIsTopaz();
			sig.SigString=signatureBoxWrapper.GetSignature(keyData);
			if(sig.IsTopaz && !_topazNeedsSaved) {
				return;
			}
			if(OrthoSignature.GetSigString(orthoChartRow.Signature)!=sig.SigString) {
				_hasChanged=true;
				orthoChartRow.Signature=sig.ToString();
			}
		}

		private bool CanEditRow(DateTime dateTime) {
			if(_dictCanEditDay.ContainsKey(dateTime)) {
				return _dictCanEditDay[dateTime];
			}
			if(Security.IsAuthorized(Permissions.OrthoChartEditFull,dateTime,true)) {
				_dictCanEditDay[dateTime]=true;
				return true;
			}
			if(!Security.IsAuthorized(Permissions.OrthoChartEditUser,dateTime,true)) {
				_dictCanEditDay[dateTime]=false;
				return false;//User doesn't have any permission.
			}
			//User has permission to edit the ones that they have signed or ones that no one has signed.
			if(!_showSigBox) {
				_dictCanEditDay[dateTime]=true;
				return true;
			}
			OrthoChartRow orthoChartRow=gridMain.GetTags<OrthoChartRow>().FirstOrDefault(x => x.DateTimeService==dateTime);
			if(orthoChartRow is null){
				_dictCanEditDay[dateTime]=true;
				return true;
			}
			if(orthoChartRow.UserNum==_curUser.UserNum) {
				_dictCanEditDay[dateTime]=true;
				return true;
			}
			bool canEditRow;
			OrthoSignature sig=new OrthoSignature(orthoChartRow.Signature);
			canEditRow=string.IsNullOrEmpty(sig.SigString);//User has partial permission and somebody else signed it.
			_dictCanEditDay[dateTime]=canEditRow;
			return canEditRow;
		}

		///<summary>Gets the value from _listOrthoChartRows for the specified date and column heading.  Returns empty string if not found.  Note that the column name showing within the ODGrid could be different than the column heading within _listOrthoChartRows.  Use gridMain.Columns[x].Tag to get the corresponding column header that _listOrthoChartRows uses (displayfield.Description).</summary>
		private string GetValueFromList(OrthoChartRow orthoChartRow,string columnHeading) {
			if(orthoChartRow is null) {
				return "";
			}
			string sigColumnName=_listOrthDisplayFields.FirstOrDefault(x => x.InternalName=="Signature")?.Description??"";
			if(columnHeading==sigColumnName) {
				return orthoChartRow.Signature;
			}
			OrthoChart orthoChart=orthoChartRow.ListOrthoCharts.FirstOrDefault(x=>x.FieldName==columnHeading);
			if(orthoChart is null) {
				return "";
			}
			return orthoChart.FieldValue;
		}

		///<summary>Returns a list of displayfields for the current tab.</summary>
		private List<DisplayField> GetDisplayFieldsForCurrentTab() {
			OrthoChartTab orthoChartTab=_listOrthoChartTabs[tabControl.SelectedIndex];
			List<DisplayField> listDisplayFields=
				OrthoChartTabLinks.GetWhere(x => x.OrthoChartTabNum==orthoChartTab.OrthoChartTabNum)//Determines the number of items that will be returned
				.OrderBy(x => x.ItemOrder)//Each tab is ordered based on the ortho tab link entry
				.Select(x => _listOrthDisplayFields.FirstOrDefault(y => y.DisplayFieldNum==x.DisplayFieldNum))//Project all corresponding display fields in order
				.Where(x => x!=null)//Can happen when there is an OrthoChartTabLink in the database pointing to an invalid display field.
				.ToList();//Casts the projection to a list of display fields
			return listDisplayFields;
		}

		///<summary>Returns true if the display field column has a pick list</summary>
		private bool HasPickList(string colName) {
			foreach (DisplayField field in _listOrthDisplayFields) {
				if(field.Description==colName && !string.IsNullOrEmpty(field.PickList)) {
					return true;
				}
			}
			return false;
		}

		///<summary>Returns true if the display field column is provider.</summary>
		private bool HasProvider(string colName) {
			foreach(DisplayField field in _listOrthDisplayFields) {
				if(field.Description==colName && field.InternalName=="Provider") {
					return true;
				}
			}
			return false;
		}

		///<summary>Sets the value in _listOrthoChartRows for the specified date and column heading.</summary>
		private void SetValueInList(OrthoChartRow orthoChartRow,string newValue,string columnHeading) {
			string sigColumnName=_listOrthDisplayFields.FirstOrDefault(x => x.InternalName=="Signature")?.Description??"";
			orthoChartRow.UserNum=_curUser.UserNum;
			if(columnHeading==sigColumnName) {//Signature column was modified
				orthoChartRow.Signature=newValue;
				return;
			}
			OrthoChart orthoChart=orthoChartRow.ListOrthoCharts.FirstOrDefault(x=>x.FieldName==columnHeading);
			if(orthoChart is null) {
				orthoChart=new OrthoChart();
				orthoChart.FieldName=columnHeading;
				orthoChart.FieldValue=newValue;
				orthoChart.OrthoChartRowNum=orthoChartRow.OrthoChartRowNum;
				orthoChartRow.ListOrthoCharts.Add(orthoChart);
				return;
			}
			orthoChart.FieldValue=newValue;
		}

		private void gridMain_CellSelectionCommitted(object sender,ODGridClickEventArgs e) {
			DateTime dateTime=((OrthoChartRow)gridMain.ListGridRows[gridMain.SelectedCell.Y].Tag).DateTimeService;
			if(!CanEditRow(dateTime)) {
				return;
			}
			_hasChanged=true;
			if(_showSigBox) {
				signatureBoxWrapper.ClearSignature(false);
			}
		}

		private void gridPat_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridPat.ListGridRows[e.Row].Tag is PatFieldDef) {//patfield for an existing PatFieldDef
				PatFieldDef patFieldDef=(PatFieldDef)gridPat.ListGridRows[e.Row].Tag;
				PatField field=PatFields.GetByName(patFieldDef.FieldName,_arrayPatientFields);
				PatFieldL.OpenPatField(field,patFieldDef,_patCur.PatNum,true);
			}
			else if(gridPat.ListGridRows[e.Row].Tag is PatField) {//PatField for a PatFieldDef that no longer exists
				PatField field=(PatField)gridPat.ListGridRows[e.Row].Tag;
				using FormPatFieldEdit FormPF=new FormPatFieldEdit(field);
				FormPF.IsLaunchedFromOrtho=true;
				FormPF.ShowDialog();
			}
			FillGridPat();
		}

		private void butShowDateRange_Click(object sender,EventArgs e) {
			using FormChartViewDateFilter formChartViewDateFilter=new FormChartViewDateFilter();
			formChartViewDateFilter.DateStart=_dateTimeFrom;
			formChartViewDateFilter.DateEnd=_dateTimeTo;
			formChartViewDateFilter.ShowDialog();
			if(formChartViewDateFilter.DialogResult!=DialogResult.OK) {
				return;
			}
			_dateTimeFrom=formChartViewDateFilter.DateStart;
			_dateTimeTo=formChartViewDateFilter.DateEnd; 
			FillDateRange();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormOrthoChartAddDate formOrthoChartAdd=new FormOrthoChartAddDate();
			formOrthoChartAdd.ShowDialog();
			if(formOrthoChartAdd.DialogResult!=DialogResult.OK) {
				return;
			}
			SaveAndSetSignatures(gridMain.SelectedCell.Y);
			OrthoChartRow orthoChartRow=new OrthoChartRow();
			orthoChartRow.PatNum=_patCur.PatNum;
			orthoChartRow.DateTimeService=formOrthoChartAdd.DateSelected;
			orthoChartRow.ProvNum=formOrthoChartAdd.ProvNumSelected;
			orthoChartRow.UserNum=_curUser.UserNum;
			orthoChartRow.Signature="";
			orthoChartRow.IsNew=true;
			OrthoChartRows.Insert(orthoChartRow);
			_listOrthoChartRows.Add(orthoChartRow);
			_listOrthoChartRows=_listOrthoChartRows.OrderBy(x => x.DateTimeService).ToList();
			_hasChanged=true;
			//Reset date range to show new row.
			_dateTimeFrom=DateTime.MinValue;
			_dateTimeTo=DateTime.MinValue;
			FillDateRange();
			Logger.LogAction("butAdd_Click",LogPath.OrthoChart,() => {
				FillGrid();
			},"FillGrid2");
			gridMain.ScrollToEnd();//When adding a new row, scroll to it.
		}

		private void butUseAutoNote_Click(object sender,EventArgs e) {
			if(gridMain.SelectedCell.X==-1 || gridMain.SelectedCell.X==0 || gridMain.SelectedCell.X==_sigColIdx) {
				MsgBox.Show(this,"Please select an editable Ortho Chart cell first.");
				return;
			}
			if(HasPickList(gridMain.Columns[gridMain.SelectedCell.X].Heading)) {
				MsgBox.Show(this,"Cannot add auto notes to a field with a pick list.");
				return;
			}
			if(HasProvider(gridMain.Columns[gridMain.SelectedCell.X].Heading)) {
				MsgBox.Show(this,"Cannot add auto notes to a field with a provider.");
				return;
			}
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				Point selectedCell=new Point(gridMain.SelectedCell.X,gridMain.SelectedCell.Y);
				//Add text to current focused cell				
				gridMain.ListGridRows[gridMain.SelectedCell.Y].Cells[gridMain.SelectedCell.X].Text+=FormA.StrCompletedNote;
				//Since the redrawing of the row height is dependent on the edit text box built into the ODGrid, we have to manually tell the grid to redraw.
				//This will essentially "refresh" the grid.  We do not want to call FillGrid() because that will lose data in other cells that have not been saved to datatable.
				if(_showSigBox && FormA.StrCompletedNote != "") {
					signatureBoxWrapper.ClearSignature(false);
					SaveSignatureToDict(gridMain.SelectedCell.Y);
					DisplaySignature(gridMain.SelectedCell.Y);
				}
				gridMain.BeginUpdate();
				gridMain.EndUpdate();
				gridMain.SetSelected(selectedCell);
				_hasChanged=true;
			}
		}

		private void butChangeUser_Click(object sender,EventArgs e) {
			int idxRow=gridMain.SelectedCell.Y;
			if(idxRow==-1) {
				MsgBox.Show(this,"Please select a ortho chart first.");//This shouldn't happen.
				return;
			}
			DateTime date=((OrthoChartRow)gridMain.ListGridRows[idxRow].Tag).DateTimeService;
			if(!CanEditRow(date)) {
				MsgBox.Show(this,"You need either Ortho Chart Edit (full) or Ortho Chart Edit (same user, signed) to edit this ortho chart.");
				return;
			}
			using FormLogOn FormChangeUser=new FormLogOn(isSimpleSwitch:true);
			FormChangeUser.ShowDialog();
			if(FormChangeUser.DialogResult!=DialogResult.OK) {
				return;
			}
			long userNumOld=_curUser.UserNum;
			_curUser=FormChangeUser.CurUserSimpleSwitch;
			long userNumNew=_curUser.UserNum;//_curUser.UserNum is now the changed user.
			EnableSignatureBox(_curUser);
			ClearSignature(idxRow);
			textUser.Text=_curUser.UserName; //update user textbox.
			if(userNumNew==userNumOld) {//Current user is the same as the changed user. Don't update DB.
				_changedUser=true;//Someone else previously signed an orthorow and the current user wants to sign, even though they're already in control.
				return;
			}
			//Current user had a lock and lost it
			//so we need to also change the userLocked here and in db
			PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,-5);//-5 means user changed locally on form. Do not call SaveToDb() on other instance of OD.
			_userNumLocked=_curUser.UserNum;			
			labelLocked.Text="Locked by: "+Userods.GetName(_curUser.UserNum);
			_isLockedByMe=true;//Changed user now has a lock.
			_changedUser=true;//User in control has changed locally on this form.
			//controls are already unlocked
			//SaveToDb();//So that other users can see the changes.
			//Decided that the above line was too complex.  Because then we would need to FillGrid followed by adding new OrthoChartRow.
		}

		private void EnableSignatureBox(Userod curUser) {
			bool canUserSignNote=Userods.CanUserSignNote(curUser);//only show if user can sign
			signatureBoxWrapper.Enabled=canUserSignNote;
			labelPermAlert.Visible=!canUserSignNote;
			if(canUserSignNote) {
				signatureBoxWrapper.UserSig=curUser;//For electronic signature stamp. [E]
			}
		}

		private void butAudit_Click(object sender,EventArgs e) {
			SecurityLog[] orthoChartLogs;
			SecurityLog[] patientFieldLogs;
			try {
				orthoChartLogs=SecurityLogs.Refresh(_patCur.PatNum,new List<Permissions> { Permissions.OrthoChartEditFull },null);
				patientFieldLogs=SecurityLogs.Refresh(new DateTime(1,1,1),DateTime.Today,Permissions.PatientFieldEdit,_patCur.PatNum,0,
					DateTime.MinValue,DateTime.Today);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"There was a problem loading the Audit Trail."),ex);
				return;
			}
			SortedDictionary<DateTime,List<SecurityLog>> dictDatesOfServiceLogEntries=new SortedDictionary<DateTime,List<SecurityLog>>();
			//Add all dates from grid first, some may not have audit trail entries, but should be selectable from FormAO
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				DateTime date=((OrthoChartRow)gridMain.ListGridRows[i].Tag).DateTimeService;
				if(dictDatesOfServiceLogEntries.ContainsKey(date)) {
					continue;
				}
				dictDatesOfServiceLogEntries.Add(date,new List<SecurityLog>());
			}
			//Add Ortho Audit Trail Entries
			for(int i=0;i<orthoChartLogs.Length;i++) {
				DateTime dtCur=OrthoCharts.GetOrthoDateFromLog(orthoChartLogs[i]);
				if(!dictDatesOfServiceLogEntries.ContainsKey(dtCur)) {
					dictDatesOfServiceLogEntries.Add(dtCur,new List<SecurityLog>());
				}
				dictDatesOfServiceLogEntries[dtCur].Add(orthoChartLogs[i]);//add entry to existing list.
			}
			using FormAuditOrtho FormAO=new FormAuditOrtho();
			FormAO.DictDateOrthoLogs=dictDatesOfServiceLogEntries;
			FormAO.ListSecurityLogs.AddRange(patientFieldLogs);
			FormAO.ShowDialog();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			OrthoChartRow orthoChartRowSelected=gridMain.SelectedTag<OrthoChartRow>();
			if(orthoChartRowSelected==null) {
				MsgBox.Show(this,"Please select an Ortho Chart row first.");
				return;
			}
			if(!Security.IsAuthorized(Permissions.OrthoChartEditFull,orthoChartRowSelected.DateTimeService,false)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete entire row?")) {
				return;
			}
			List<DisplayField> listDisplayFields=GetDisplayFieldsForCurrentTab();
			List<string> listFieldNamesToDelete=listDisplayFields.Select(x => x.Description).ToList();
			orthoChartRowSelected.ListOrthoCharts.RemoveAll(x => ListTools.In(x.FieldName,listFieldNamesToDelete));
			if(orthoChartRowSelected.ListOrthoCharts.IsNullOrEmpty()) {
				_listOrthoChartRows.RemoveAll(x => x.OrthoChartRowNum==orthoChartRowSelected.OrthoChartRowNum);
			}
			FillGrid();
			_hasChanged=true;
		}

		private int _pagesPrinted;
		private int _headingPrintH;
		private List<OrthoChartTab> _listOrthoChartTabs;

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			PrintDocument pd=new PrintDocument();
			pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPage);
			pd.DefaultPageSettings.Margins=new Margins(25,25,40,40);
			if(pd.DefaultPageSettings.PrintableArea.Height==0) {
				pd.DefaultPageSettings.PaperSize=new PaperSize("default",850,1100);
			}
			if(gridMain.Columns.WidthAll()>800) {
				//a new feature will need to be implemented to handle when columns widths are greater than 1050
				pd.DefaultPageSettings.Landscape=true;
			}
			try {
			//TODO: Implement ODprintout pattern
				if(ODBuild.IsDebug()) {
					using FormRpPrintPreview pView = new FormRpPrintPreview(pd);
					pView.ShowDialog();
				}
				else {
					if(PrinterL.SetPrinter(pd,PrintSituation.Default,0,"Ortho chart printed")) {
						pd.Print();
					}
				}
			}
			catch {
				MessageBox.Show(Lan.g(this,"Printer not available"));
			}
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			string text;
			using Font headingFont = new Font("Arial",13,FontStyle.Bold);
			using Font subHeadingFont = new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			#region printHeading
			//========== TITLE ==========
			string title=_listOrthoChartTabs[0].TabName;
			g.DrawString(title,headingFont,Brushes.Black,400-g.MeasureString(title,headingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(title,headingFont).Height;
			if(_listOrthoChartTabs.Count>1) {
				OrthoChartTab orthoChartTab=_listOrthoChartTabs[tabControl.SelectedIndex];
				//========== SUBHEADING ==========
				text=orthoChartTab.TabName;//The tab selected will be the subheading. 
				g.DrawString(text,subHeadingFont,Brushes.Black,400-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				//========== DATE ==========
				text=DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,800-g.MeasureString(text,subHeadingFont).Width,yPos);
				//========== PATIENT NAME ==========
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				text=_patCur.GetNameFL();//name
				if(g.MeasureString(text,subHeadingFont).Width>700) {
					//extremely long name
					text=_patCur.GetNameFirst()[0]+". "+_patCur.LName;//example: J. Sparks
				}
				string[] headerText={ text };
				text=headerText[0];
				g.DrawString(text,subHeadingFont,Brushes.Black,400-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				//========== PAGE NUMBER ==========
				text="Page "+(_pagesPrinted+1);
				g.DrawString(text,subHeadingFont,Brushes.Black,800-g.MeasureString(text,subHeadingFont).Width,yPos);
			}
			else {
				//========== PATIENT NAME ==========
				text=_patCur.GetNameFL();//name
				if(g.MeasureString(text,subHeadingFont).Width>700) {
					//extremely long name
					text=_patCur.GetNameFirst()[0]+". "+_patCur.LName;//example: J. Sparks
				}
				string[] headerText={ text };
				text=headerText[0];
				g.DrawString(text,subHeadingFont,Brushes.Black,400-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				//========== DATE ==========
				text=DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,800-g.MeasureString(text,subHeadingFont).Width,yPos);
				yPos+=(int)g.MeasureString(text,subHeadingFont).Height;
				//========== PAGE NUMBER ==========
				text="Page "+(_pagesPrinted+1);
				g.DrawString(text,subHeadingFont,Brushes.Black,800-g.MeasureString(text,subHeadingFont).Width,yPos);
			}
			yPos+=20;
			_headingPrintH=yPos;
			#endregion
			//========== MAIN GRID ==========
			yPos=gridMain.PrintPage(g,_pagesPrinted,bounds,_headingPrintH,true);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			//The only way you can click ok is if you have a lock control.
			PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,0);//Unlock
			if(!_hasChanged && !signatureBoxWrapper.SigChanged) {
				DeleteRowsIfEmpty();
				DialogResult=DialogResult.OK;
				return;
			}
			SaveToDb();
			DialogResult=DialogResult.OK;
		}

		///<summary>Used in 4 places: timerLock_tick (if someone took control from us), OK_click, and FormClosing. It the first case, where the form remains open, it's immediately followed by OrthoChartRows.GetAllForPatient to get data from from db.</summary>
		private void SaveToDb(){
			if(_showSigBox && gridMain.SelectedCell.Y != -1) {
				SaveSignatureToDict(gridMain.SelectedCell.Y);
			}
			List<OrthoChart> listOrthoChartsNew=new List<OrthoChart>();
			List<long> listOrthChartRowNumsToDelete=new List<long>();
			for(int i=0;i<_listOrthoChartRows.Count;i++){
				if(_listOrthoChartRows[i].ListOrthoCharts.IsNullOrEmpty() || _listOrthoChartRows[i].ListOrthoCharts.All(x => string.IsNullOrEmpty(x.FieldValue))) {
					//Delete orthochartrows that are empty.
					listOrthChartRowNumsToDelete.Add(_listOrthoChartRows[i].OrthoChartRowNum);
					continue;
				}
				listOrthoChartsNew.AddRange(_listOrthoChartRows[i].ListOrthoCharts);//add cells
			}
			List<OrthoChartRow> listOrthoChartRowsFiltered=_listOrthoChartRows.FindAll(x => !ListTools.In(x.OrthoChartRowNum,listOrthChartRowNumsToDelete));
			OrthoCharts.Sync(_patCur,_listOrthoChartRows,listOrthoChartsNew,_listOrthDisplayFields);
			OrthoChartRows.Sync(listOrthoChartRowsFiltered,_patCur.PatNum);//This sync will delete rows not present in the list.
		}

		///<summary>Used when user clicks cancel or [X], so a full sync is not done, but we still need to get rid of any empty rows.</summary>
		private void DeleteRowsIfEmpty() {
			if(_listOrthoChartRows.Count==0) {
				return;
			}
			for(int i=0;i<_listOrthoChartRows.Count;i++) {
				if(_listOrthoChartRows[i].DateTimeService.Date!=DateTime.Today) {
					continue;
				}
				if(_listOrthoChartRows[i].ListOrthoCharts.IsNullOrEmpty() || _listOrthoChartRows[i].ListOrthoCharts.All(x => string.IsNullOrEmpty(x.FieldValue))) {
					OrthoChartRows.Delete(_listOrthoChartRows[i].OrthoChartRowNum);
				}
			}
		}

		private void butCancel_Click(object sender,EventArgs e) {
			if(_isLockedByMe) {
				PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,0);//Unlock
				DeleteRowsIfEmpty();
			}
			DialogResult=DialogResult.Cancel;
		}

		private void FormOrthoChart_CloseXClicked(object sender, CancelEventArgs e){
			if(_hasChanged || signatureBoxWrapper.SigChanged){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Unsaved changes will be lost.")) {
					e.Cancel=true;
					return;
				}
			}
			if(_isLockedByMe) {
				PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,0);//Unlock
				DeleteRowsIfEmpty();
			}
			DialogResult=DialogResult.Cancel;//to signal to FormClosing that this was a user action
		}

		private void FormOrthoChart_FormClosing(object sender,FormClosingEventArgs e) {
			signatureBoxWrapper?.SetTabletState(0);
			if(DialogResult!=DialogResult.None){
				return;
			}
			//Must be closing automatically without user input. Example: auto shutdown.
			if(!_isLockedByMe) {
				//this user does not have a lock, so nothing to save.
			}
			SaveToDb();//There may be nothing to save, but this also gets rid of extra empty row.
			PatientNotes.SetUserNumOrthoLocked(_patCur.PatNum,0);//Unlock
		}

		///<summary>Stores the signature string and whether the signature is Topaz. Use ToString() to store it in the database.
		///This class saves us from adding a IsSigTopaz column to the orthochart table.</summary>
		private class OrthoSignature {
			public bool IsTopaz;
			public string SigString;

			///<summary>Parses a string like "0:ritwq/wV8vlrgUYahhK+RH5UeBFA6W4jCkZdo0cDWd63aZb1S/W3Z4eW5LmchqfgniG23" into a Signature object. 
			///The 1st character is whether or not the signature is Topaz; the 2nd character is a separator; the rest of the string is the signature data.
			///</summary>
			public OrthoSignature(string dbString) {
				if(dbString.Length < 3) {
					IsTopaz=false;
					SigString="";
					return;
				}
				IsTopaz=false;
				if(dbString[0]==1) {
					IsTopaz=true;
				}
				SigString=dbString.Substring(2);
			}

			public OrthoSignature() {
				IsTopaz=false;
				SigString="";
			}

			///<summary>Gets the signature string from a string like "1:52222559445999975122111500485555". Passing in an empty string will return an empty
			///string.</summary>
			public static string GetSigString(string dbString) {
				if(dbString.Length < 3) {
					return "";
				}
				return dbString.Substring(2);
			}

			///<summary>Converts the object to a string like "0:ritwq/wV8vlrgUYahhK+RH5UeBFA6W4jCkZdo0cDWd63aZb1S/W3Z4eW5LmchqfgniG23". The 1st character
			///is whether or not the signature is Topaz; the 2nd character is a separator; the rest of the string is the signature data.</summary>
			public override string ToString() {
				return (IsTopaz ? "1" : "0")+":"+SigString;
			}
		}

		
	}

}
