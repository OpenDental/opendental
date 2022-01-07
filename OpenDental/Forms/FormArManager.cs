using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentalCloud;
using OpenDentalCloud.Core;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormArManager:FormODBase {
		private List<Provider> _listProviders;
		private List<Clinic> _listClinics;
		private long _selectedClinicNum;
		///<summary>Stores the root title of the form on load. The name of the selected clinic will be appended to this string.</summary>
		private string _formText;
		private List<Def> _listBillTypesNoColl;
		private Def _collectionBillType;
		private Def _excludedBilltype;
		private List<PatAging> _listPatAgingUnsentAll;
		private List<PatAging> _listPatAgingSentAll;
		private List<PatAging> _listPatAgingExcludedAll;
		private List<TsiTransType> _listNewStatuses;
		private bool _isResizing;
		private bool _hasResizeBegan;
		///<summary>Used to reselect rows after sorting the grid by column.  Filled on grid MouseDown event and used in grid OnSortByColumn event to reselect.</summary>
		private List<long> _listSelectedPatNums;
		private Program _tsiProg;
		private Dictionary<long,List<ProgramProperty>> _dictClinicProgProps;
		private ToolTip _toolTipUnsentErrors;
		private Point _lastCursorPos;
		private bool _headingPrinted;
		private int _pagesPrinted;
		private int _headingPrintH;
		///<summary>Stores DateTime of earliest balance>0.005 for the pat's grouped by clinic.  Will grow as user selects clinics.</summary>
		private List<ClinicBalBegans> _listClinicBalBegans;

		public FormArManager() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormArManager_Load(object sender,EventArgs e) {
			#region Get Variables for Both Tabs
			List<Def> billTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			_collectionBillType=billTypeDefs.FirstOrDefault(x => x.ItemValue.ToLower()=="c")?.Copy();
			_excludedBilltype=billTypeDefs.FirstOrDefault(x => x.ItemValue.ToLower()=="ce")?.Copy();
			_listBillTypesNoColl=billTypeDefs.Where(x => x.ItemValue.ToLower()!="c" && x.ItemValue.ToLower()!="ce").Select(x => x.Copy()).ToList();//This probably needs to change
			_listClinics=new List<Clinic>();
			if(PrefC.HasClinicsEnabled) {
				_listClinics.AddRange(Clinics.GetForUserod(Security.CurUser,true,Lan.g(this,"Unassigned")));
			}
			else {//clinics disabled
				_listClinics.Add(Clinics.GetPracticeAsClinicZero(Lan.g(this,"Unassigned")));
			}
			_selectedClinicNum=Clinics.ClinicNum;
			_formText=Text;
			SetFormText();
			LayoutMenu();
			_listProviders=Providers.GetDeepCopy(true);
			_tsiProg=Programs.GetCur(ProgramName.Transworld);
			_dictClinicProgProps=new Dictionary<long,List<ProgramProperty>>();
			if(_tsiProg!=null && _tsiProg.Enabled) {
				_dictClinicProgProps=ProgramProperties.GetForProgram(_tsiProg.ProgramNum)
					.FindAll(x => _listClinics.Any(y => y.ClinicNum==x.ClinicNum))//will contain the HQ "clinic" if clinics are disabled or user unrestricted
					.GroupBy(x => x.ClinicNum).ToDictionary(x => x.Key,x => x.ToList());
				if(PrefC.HasClinicsEnabled && !Security.CurUser.ClinicIsRestricted && _dictClinicProgProps.ContainsKey(0)) {
					//if clinics are enabled and the user is not restricted, any clinic without prog props will use the HQ prog props for the sftp connection
					_listClinics.FindAll(x => !_dictClinicProgProps.ContainsKey(x.ClinicNum)).ForEach(x => _dictClinicProgProps[x.ClinicNum]=_dictClinicProgProps[0]);
				}
			}
			_toolTipUnsentErrors=new ToolTip() { InitialDelay=1000,ReshowDelay=1000,ShowAlways=true };
			_lastCursorPos=gridUnsent.PointToClient(Cursor.Position);
			#endregion Get Variables for Both Tabs
			#region Fill Unsent Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Unsent Tab Prov Combo
			comboProvsUnsent.IncludeAll=true;
			comboProvsUnsent.IsAllSelected=true;
			comboProvsUnsent.Items.AddProvsFull(_listProviders);
			#endregion Unsent Tab Prov Combo
			#region Unsent Tab Bill Type Combo
			List<long> listDefaultBillTypes=PrefC.GetString(PrefName.ArManagerBillingTypes)
				.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Long(x)).ToList();
			comboBillTypes.IncludeAll=true;
			if(_listBillTypesNoColl.All(x => !listDefaultBillTypes.Contains(x.DefNum))) {
				comboBillTypes.IsAllSelected=true;//select All if no valid defaults are set
			}
			for(int i=0;i<_listBillTypesNoColl.Count;i++) {
				comboBillTypes.Items.Add(_listBillTypesNoColl[i].ItemName,_listBillTypesNoColl[i]);
				if(listDefaultBillTypes.Contains(_listBillTypesNoColl[i].DefNum)) {
					comboBillTypes.SetSelected(comboBillTypes.Items.Count-1,true);
				}
			}
			#endregion Unsent Tab Bill Type Combo
			#region Unsent Tab Account Age Combo
			comboUnsentAccountAge.Items.Add(Lan.g(this,"Any Balance"));
			comboUnsentAccountAge.Items.Add(Lan.g(this,"Over 30 Days"));
			comboUnsentAccountAge.Items.Add(Lan.g(this,"Over 60 Days"));
			comboUnsentAccountAge.Items.Add(Lan.g(this,"Over 90 Days"));
			comboUnsentAccountAge.SelectedIndexChanged-=comboUnsentAccountAge_SelectedIndexChanged;
			comboUnsentAccountAge.SelectedIndex=new List<string>() { "30","60","90" }.IndexOf(PrefC.GetString(PrefName.ArManagerUnsentAgeOfAccount))+1;//+1 for any bal
			comboUnsentAccountAge.SelectedIndexChanged+=comboUnsentAccountAge_SelectedIndexChanged;
			#endregion Unsent Tab Account Age Combo
			#region Unsent Tab Textbox Filters
			//text min bal
			textUnsentMinBal.TextChanged-=textUnsentMinBal_TextChanged;
			textUnsentMinBal.Text=PrefC.GetDouble(PrefName.ArManagerUnsentMinBal).ToString();
			textUnsentMinBal.TextChanged+=textUnsentMinBal_TextChanged;
			//text days since last payment
			textUnsentDaysLastPay.TextChanged-=textUnsentDaysLastPay_TextChanged;
			textUnsentDaysLastPay.Text=PrefC.GetInt(PrefName.ArManagerUnsentDaysSinceLastPay).ToString();
			textUnsentDaysLastPay.TextChanged+=textUnsentDaysLastPay_TextChanged;
			#endregion Unsent Tab Textbox Filters
			#region Unsent Tab Checkbox Filters
			//exclude if ins pending
			checkExcludeInsPending.CheckedChanged-=checkExcludeInsPending_CheckedChanged;
			checkExcludeInsPending.Checked=PrefC.GetBool(PrefName.ArManagerExcludeInsPending);
			checkExcludeInsPending.CheckedChanged+=checkExcludeInsPending_CheckedChanged;
			//exclude if unsent procs
			checkExcludeIfProcs.CheckedChanged-=checkExcludeIfProcs_CheckedChanged;
			checkExcludeIfProcs.Checked=PrefC.GetBool(PrefName.ArManagerExcludeIfUnsentProcs);
			checkExcludeIfProcs.CheckedChanged+=checkExcludeIfProcs_CheckedChanged;
			//exclude if bad address (no zipcode)
			checkExcludeBadAddress.CheckedChanged-=checkExcludeBadAddress_CheckedChanged;
			checkExcludeBadAddress.Checked=PrefC.GetBool(PrefName.ArManagerExcludeBadAddresses);
			checkExcludeBadAddress.CheckedChanged+=checkExcludeBadAddress_CheckedChanged;
			#endregion Unsent Tab Checkbox Filters
			#region Unsent Tab Service Type Combo
			FillServiceTypes();
			#endregion Unsent Tab Service Type Combo
			#region Unsent Tab Show PatNums
			checkUnsentShowPatNums.CheckedChanged-=checkUnsentShowPatNums_CheckedChanged;
			checkUnsentShowPatNums.Checked=PrefC.GetBool(PrefName.ReportsShowPatNum);
			checkUnsentShowPatNums.CheckedChanged+=checkUnsentShowPatNums_CheckedChanged;
			#endregion Unsent Tab Show PatNums
			#endregion Fill Unsent Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Fill Excluded Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Excluded Tab Prov Combo
			comboProvsExcluded.IncludeAll=true;
			comboProvsExcluded.IsAllSelected=true;
			comboProvsExcluded.Items.AddProvsFull(_listProviders);
			#endregion Excluded Tab Prov Combo
			#region Excluded Tab Account Age Combo
			comboExcludedAccountAge.Items.Add(Lan.g(this,"Any Balance"));
			comboExcludedAccountAge.Items.Add(Lan.g(this,"Over 30 Days"));
			comboExcludedAccountAge.Items.Add(Lan.g(this,"Over 60 Days"));
			comboExcludedAccountAge.Items.Add(Lan.g(this,"Over 90 Days"));
			comboExcludedAccountAge.SelectedIndexChanged-=comboExcludedAccountAge_SelectedIndexChanged;
			comboExcludedAccountAge.SelectedIndex=new List<string>() { "30","60","90" }.IndexOf(PrefC.GetString(PrefName.ArManagerExcludedAgeOfAccount))+1;//+1 for any bal
			comboExcludedAccountAge.SelectedIndexChanged+=comboExcludedAccountAge_SelectedIndexChanged;
			#endregion Excluded Tab Account Age Combo
			#region Excluded Tab Textbox Filters
			//text min bal
			textExcludedMinBal.TextChanged-=textExcludedMinBal_TextChanged;
			textExcludedMinBal.Text=PrefC.GetDouble(PrefName.ArManagerExcludedMinBal).ToString();
			textExcludedMinBal.TextChanged+=textExcludedMinBal_TextChanged;
			//text days since last payment
			textExcludedDaysLastPay.TextChanged-=textExcludedDaysLastPay_TextChanged;
			textExcludedDaysLastPay.Text=PrefC.GetInt(PrefName.ArManagerExcludedDaysSinceLastPay).ToString();
			textExcludedDaysLastPay.TextChanged+=textExcludedDaysLastPay_TextChanged;
			#endregion Excluded Tab Textbox Filters
			#region Excluded Tab Checkbox Filters
			//exclude if ins pending
			checkExcludedExcludeInsPending.CheckedChanged-=checkExcludeExcludedInsPending_CheckedChanged;
			checkExcludedExcludeInsPending.Checked=PrefC.GetBool(PrefName.ArManagerExcludedExcludeInsPending);
			checkExcludedExcludeInsPending.CheckedChanged+=checkExcludeExcludedInsPending_CheckedChanged;
			//exclude if unsent procs
			checkExcludedExcludeIfProcs.CheckedChanged-=checkExcludeExcludedIfProcs_CheckedChanged;
			checkExcludedExcludeIfProcs.Checked=PrefC.GetBool(PrefName.ArManagerExcludedExcludeIfUnsentProcs);
			checkExcludedExcludeIfProcs.CheckedChanged+=checkExcludeExcludedIfProcs_CheckedChanged;
			//exclude if bad address (no zipcode)
			checkExcludedExcludeBadAddress.CheckedChanged-=checkExcludeExcludedBadAddress_CheckedChanged;
			checkExcludedExcludeBadAddress.Checked=PrefC.GetBool(PrefName.ArManagerExcludedExcludeBadAddresses);
			checkExcludedExcludeBadAddress.CheckedChanged+=checkExcludeExcludedBadAddress_CheckedChanged;
			#endregion Excluded Tab Checkbox Filters
			#region Excluded Tab Service Type Combo
			FillExcludedServiceTypes();
			#endregion Excluded Tab Service Type Combo
			#region Excluded Tab Show PatNums
			checkExcludedShowPatNums.CheckedChanged-=checkExcludedShowPatNums_CheckedChanged;
			checkExcludedShowPatNums.Checked=PrefC.GetBool(PrefName.ReportsShowPatNum);
			checkExcludedShowPatNums.CheckedChanged+=checkExcludedShowPatNums_CheckedChanged;
			#endregion Excluded Tab Show PatNums
			#endregion Fill Excluded Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Fill Sent Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Sent Tab Provs Combo
			comboProvsSent.IncludeAll=true;
			comboProvsSent.IsAllSelected=true;
			comboProvsSent.Items.AddProvsFull(_listProviders);
			#endregion Sent Tab Provs Combo
			#region Sent Tab Trans Type Combo
			List<TsiTransType> listSentTabTransTypes=Enum.GetValues(typeof(TsiTransType)).OfType<TsiTransType>()
				.Where(x => !ListTools.In(x,TsiTransType.PF,TsiTransType.PT,TsiTransType.SS,TsiTransType.CN,TsiTransType.Agg)).ToList();
			List<TsiTransType> listDefaultLastTransTypes=PrefC.GetString(PrefName.ArManagerLastTransTypes)
				.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries)
				.Select(x => PIn.Enum<TsiTransType>(x,true))
				.Where(x => !ListTools.In(x,TsiTransType.PF,TsiTransType.PT,TsiTransType.SS,TsiTransType.CN,TsiTransType.Agg)).ToList();
			comboLastTransType.IncludeAll=true;
			if(listSentTabTransTypes.All(x => !listDefaultLastTransTypes.Contains(x))) {
				comboLastTransType.IsAllSelected=true;
			}
			for(int i=0;i<listSentTabTransTypes.Count;i++) {
				comboLastTransType.Items.Add(listSentTabTransTypes[i].GetDescription(),listSentTabTransTypes[i]);
				if(listDefaultLastTransTypes.Contains(listSentTabTransTypes[i])) {
					comboLastTransType.SetSelected(comboLastTransType.Items.Count-1,true);
				}
			}
			#endregion Sent Tab Trans Type Combo
			#region Sent Tab Account Age Combo
			comboSentAccountAge.Items.Add(Lan.g(this,"Any Balance"));
			comboSentAccountAge.Items.Add(Lan.g(this,"Over 30 Days"));
			comboSentAccountAge.Items.Add(Lan.g(this,"Over 60 Days"));
			comboSentAccountAge.Items.Add(Lan.g(this,"Over 90 Days"));
			comboSentAccountAge.SelectedIndexChanged-=comboSentAccountAge_SelectedIndexChanged;
			comboSentAccountAge.SelectedIndex=new List<string>() { "30","60","90" }.IndexOf(PrefC.GetString(PrefName.ArManagerSentAgeOfAccount))+1;//+1 for any bal
			comboSentAccountAge.SelectedIndexChanged+=comboSentAccountAge_SelectedIndexChanged;
			#endregion Sent Tab Account Age Combo
			#region Sent Tab Textbox Filters
			//text min bal
			textSentMinBal.TextChanged-=textSentMinBal_TextChanged;
			textSentMinBal.Text=PrefC.GetDouble(PrefName.ArManagerSentMinBal).ToString();
			textSentMinBal.TextChanged+=textSentMinBal_TextChanged;
			//text days since last payment
			textSentDaysLastPay.TextChanged-=textSentDaysLastPay_TextChanged;
			textSentDaysLastPay.Text=PrefC.GetInt(PrefName.ArManagerSentDaysSinceLastPay).ToString();
			textSentDaysLastPay.TextChanged+=textSentDaysLastPay_TextChanged;
			#endregion Sent Tab Textbox Filters
			#region Sent Tab New Statuses Combo
			_listNewStatuses=new List<TsiTransType>() { TsiTransType.SS,TsiTransType.CN };
			_listNewStatuses.ForEach(x => comboNewStatus.Items.Add(x.GetDescription()));
			#endregion Sent Tab New Statuses Combo
			#region Sent Tab New Bill Types Combo
			_listBillTypesNoColl.ForEach(x => comboNewBillType.Items.Add(x.ItemName));
			errorProvider1.SetError(comboNewBillType,"");
			#endregion Sent Tab New Bill Types Combo
			#region Sent Tab Show PatNums
			checkSentShowPatNums.CheckedChanged-=checkSentShowPatNums_CheckedChanged;
			checkSentShowPatNums.Checked=PrefC.GetBool(PrefName.ReportsShowPatNum);
			checkSentShowPatNums.CheckedChanged+=checkSentShowPatNums_CheckedChanged;
			#endregion Sent Tab Show PatNums
			#endregion Fill Sent Tab Filter ComboBoxes, CheckBoxes, and Fields
			#region Run Aging if Necessary
			string msgText="There was a problem running aging.  Would you like to load the accounts grid with currently existing account information?";
			try {
				if(!RunAgingIfNecessary(isAutomatic:true) && !MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)) {
					Close();
					return;
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)) {
					Close();
					return;
				}
			}
			#endregion Run Aging if Necessary
			#region Get Aging List and Fill Grids
			RefreshAll();
			FillGrids();
			#endregion Get Aging List and Fill Grids
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			MenuItemOD menuItemClinicsMain=new MenuItemOD("&Clinics");
			menuMain.Add(menuItemClinicsMain);
			menuMain.EndUpdate();
			if(!PrefC.HasClinicsEnabled) {//clinics disabled the menu will be hidden, but will still contain ClinicNum 0
				menuItemClinicsMain.Available=false;
				return;
			}
			menuItemClinicsMain.DropDown.Items.Clear();
			if(_listClinics.Count<30) { //This number of clinics will fit in a 990x735 form.
				MenuItemOD menuItem;
				for(int i=0;i<_listClinics.Count;i++) {
					menuItem=new MenuItemOD(_listClinics[i].Abbr,menuClinic_Click) { Tag=_listClinics[i] };
					if(_selectedClinicNum==_listClinics[i].ClinicNum) {
						menuItem.Checked=true;
					}
					menuItemClinicsMain.Add(menuItem);
					if(i==0 && _listClinics[i].ClinicNum==0) {
						menuItemClinicsMain.AddSeparator();
					}
				}
			}
			else {//too many clinics to put in a menu drop down
				menuItemClinicsMain.Click+=menuClick_OpenPickList;
			}
		}

		private void menuClinic_Click(object sender,System.EventArgs e) {
			if(!(sender is MenuItemOD menuItemCur) || !(menuItemCur.Tag is Clinic clinicCur) || _selectedClinicNum==clinicCur.ClinicNum) {
				return;
			}
			_selectedClinicNum=clinicCur.ClinicNum;
			SetFormText();
			RefreshAll();
			FillServiceTypes();
			FillExcludedServiceTypes();
			FillGrids();
		}
		
		private void menuClick_OpenPickList(object sender,EventArgs e) {
			using FormClinics FormC=new FormClinics();
			FormC.IsSelectionMode=true;
			FormC.ListClinics=_listClinics;
			if(FormC.ShowDialog()!=DialogResult.OK || _selectedClinicNum==FormC.SelectedClinicNum) {
				return;
			}
			_selectedClinicNum=FormC.SelectedClinicNum;
			SetFormText();
			RefreshAll();
			FillServiceTypes();
			FillExcludedServiceTypes();
			FillGrids();
		}

		private void SetFormText() {
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			Text=_formText;
			if(_listClinics.Any(x => x.ClinicNum==_selectedClinicNum)) {
				Text+=" - "+Lan.g(this,"Clinic")+": ";
				if(PrefC.GetBool(PrefName.TitleBarClinicUseAbbr)) {
					Text+=Clinics.GetAbbr(_selectedClinicNum,_listClinics);
				}
				else {
					Text+=Clinics.GetDesc(_selectedClinicNum,_listClinics);
				}
			}
		}

		#region Methods For All Tabs

		///<summary>If aging happened automatically, like on load, pass in isAutomatic=true.  Returns true if aging was processed successfully.</summary>
		private bool RunAgingIfNecessary(bool isAutomatic) {
			_listClinicBalBegans=new List<ClinicBalBegans>();
			string msgText="";
			if(PrefC.GetBool(PrefName.AgingIsEnterprise)) {
				return RunAgingEnterprise(isAutomatic);
			}
			else if(!PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - AR Manager");
				msgText=Lan.g(this,"Calculating aging for all patients as of")+" "+DateTime.Today.ToShortDateString()+"...";
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=() =>Ledgers.RunAging();
				progressOD.StartingMessage=msgText;
				progressOD.MessageCancel=Lan.g(this,"You should not Cancel because this might leave some or all patient balances set to 0. If you do cancel, make sure to run Aging again. Cancel anyway?");
				progressOD.TestSleep=true;
				try{
					progressOD.ShowDialogProgress();
				}
				catch(Exception e){
					Ledgers.AgingExceptionHandler(e,this);
				}
				if(!progressOD.IsSuccess){//whether cancel or exception
					return false;
				}
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging complete - AR Manager");
				AgingRanAuditHelper(isAutomatic);
			}
			msgText="Last aging date seems old.  Would you like to run aging now?  The account list will load whether or not aging gets updated.";
			//All places in the program where aging can be run for all patients, the Setup permission is required because it can take a long time.
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily) 
				&& Security.IsAuthorized(Permissions.Setup,true)
				&& PrefC.GetDate(PrefName.DateLastAging)<DateTime.Today.AddDays(-15)
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,msgText))
			{
				using FormAging FormA=new FormAging();
				FormA.BringToFront();
				FormA.ShowDialog();
				//FormAging has its own Audit Trail entries.
				//The success or failure of aging in this window does not affect the return result here.
			}
			return true;
		}

		///<summary>If aging happened automatically, like on load, pass in isAutomatic=true</summary>
		private bool RunAgingEnterprise(bool isAutomatic) {			
			DateTime dtNow=MiscData.GetNowDateTime();
			DateTime dtToday=dtNow.Date;
			DateTime dateLastAging=PrefC.GetDate(PrefName.DateLastAging);
			string msgText=Lan.g(this,"Aging has already been calculated for")+" "+dtToday.ToShortDateString()+" "
				+Lan.g(this,"and does not normally need to run more than once per day.")+"\r\n\r\n"+Lan.g(this,"Run anway?");
			if(dateLastAging.Date==dtToday.Date && MessageBox.Show(this,msgText,"",MessageBoxButtons.YesNo)!=DialogResult.Yes) {
				return true;
			}
			Prefs.RefreshCache();
			DateTime dateTAgingBeganPref=PrefC.GetDateT(PrefName.AgingBeginDateTime);
			if(dateTAgingBeganPref>DateTime.MinValue) {
				msgText=Lan.g(this,"In order to manage accounts receivable, aging must be calculated, but you cannot run aging until it has finished the current "
					+"calculations which began on")+" "+dateTAgingBeganPref.ToString()+".\r\n"+Lans.g(this,"If you believe the current aging process has finished, "
					+"a user with SecurityAdmin permission can manually clear the date and time by going to Setup | Miscellaneous and pressing the 'Clear' button.");
				MessageBox.Show(this,msgText);
				return false;
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Starting Aging - AR Manager");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,POut.DateT(dtNow,false));//get lock on pref to block others
			Signalods.SetInvalid(InvalidType.Prefs);//signal a cache refresh so other computers will have the updated pref as quickly as possible
			msgText=Lan.g(this,"Calculating enterprise aging for all patients as of")+" "+dtToday.ToShortDateString()+"...";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				Ledgers.ComputeAging(0,dtToday);
				Prefs.UpdateString(PrefName.DateLastAging,POut.Date(dtToday,false));
			};
			progressOD.StartingMessage=msgText;
			progressOD.TestSleep=true;
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex){
				Ledgers.AgingExceptionHandler(ex,this);
			}
			SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging Complete - AR Manager");
			Prefs.UpdateString(PrefName.AgingBeginDateTime,"");//clear lock on pref whether aging was successful or not
			Signalods.SetInvalid(InvalidType.Prefs);
			AgingRanAuditHelper(isAutomatic);
			return progressOD.IsSuccess;
		}

		///<summary>Creates "Aging Ran" audit trail entries.</summary>
		private void AgingRanAuditHelper(bool isAutomatic) {
			if(isAutomatic) {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging Ran Automatically - AR Manager");
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.AgingRan,0,"Aging Ran - AR Manager");
			}
		}

		private void RefreshAll() {
			//clear rows here because the rows are tagged with the PatAging objects and we want to dispose of them before we get the 
			//aging list again so we don't have an out of memory error for large db's
			//Also preservers sorting by column and vertical scroll position
			int vScrollValue=gridSent.VerticalScroll.Value;
			int sortedByColumnIndexOld=gridSent.SortedByColumnIdx;
			bool isAscendingOld=gridSent.SortedIsAscending;
			gridSent.BeginUpdate();//Clears selected indices.
			gridSent.ListGridRows.Clear();
			gridSent.EndUpdate();
			gridSent.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridSent.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			vScrollValue=gridUnsent.VerticalScroll.Value;
			sortedByColumnIndexOld=gridUnsent.SortedByColumnIdx;
			isAscendingOld=gridUnsent.SortedIsAscending;
			gridUnsent.BeginUpdate();//Clears selected indices.
			gridUnsent.ListGridRows.Clear();
			gridUnsent.EndUpdate();
			gridUnsent.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridUnsent.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			vScrollValue=gridExcluded.VerticalScroll.Value;
			sortedByColumnIndexOld=gridExcluded.SortedByColumnIdx;
			isAscendingOld=gridExcluded.SortedIsAscending;
			gridExcluded.BeginUpdate();//Clears selected indices.
			gridExcluded.ListGridRows.Clear();
			gridExcluded.EndUpdate();
			gridExcluded.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridExcluded.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			string msgText=Lan.g(this,"Retrieving aging list as of")+" "+MiscData.GetNowDateTime().ToShortDateString()+"...";
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				_listPatAgingSentAll=new List<PatAging>();
				_listPatAgingUnsentAll=new List<PatAging>();
				_listPatAgingExcludedAll=new List<PatAging>();
				List<PatAging> listPatAgingAll=Patients.GetAgingList(_selectedClinicNum);
				GC.Collect();//necessary because we are in a thread and need to garbage collect before starting the next method or we may get an OOM error
				Patients.SetDateBalBegan(_selectedClinicNum,ref listPatAgingAll,ref _listClinicBalBegans);
				GC.Collect();//to reclaim the temporary memory used by the above method
				foreach(PatAging ptAgeCur in listPatAgingAll) {
					if(_collectionBillType!=null && ptAgeCur.BillingType==_collectionBillType.DefNum) {
						_listPatAgingSentAll.Add(ptAgeCur);
					}
					else if(_excludedBilltype!=null && ptAgeCur.BillingType==_excludedBilltype.DefNum){
						_listPatAgingExcludedAll.Add(ptAgeCur);
					}
					else {
						_listPatAgingUnsentAll.Add(ptAgeCur);
					}
				}
			};
			progressOD.StartingMessage=msgText;
			progressOD.ShowDialogProgress();
		}
		
		private void FillGrids(bool retainSelection=true) {
			FillGridUnsent(retainSelection);
			FillGridSent(retainSelection);
			FillGridExcluded(retainSelection);
		}

		///<summary>Line the totals textboxes up with their corresponding grid column.  Since the columns resize dynamically and can be visible or hidden
		///based on selected display fields, we need to move the location of the textboxes and set visibility.  This is called when the grid is filled as
		///well as when the user horizontally scrolls.</summary>
		private void SetTotalsLocAndVisible(DisplayFieldCategory displayCat,int hScrollValue) {
			//list of selected display fields for the grid
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(displayCat);
			//dictionary linking display field internal names to the corresponding textboxes
			Dictionary<string,TextBox> dictColTextBox = new Dictionary<string,TextBox>() {
				{ "Guarantor",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentTotalNumAccts:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedTotalNumAccts:textSentTotalNumAccts)) },
				{ "0-30 Days",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsent0to30:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcluded0to30:textSent0to30)) },
				{ "31-60 Days",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsent31to60:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcluded31to60:textSent31to60)) },
				{ "61-90 Days",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsent61to90:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcluded61to90:textSent61to90)) },
				{ "> 90 Days",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentOver90:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedOver90:textSentOver90)) },
				{ "Total",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentTotal:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedTotal:textSentTotal)) },
				{ "-Ins Est",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentInsEst:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedInsEst:textSentInsEst)) },
				{ "=Patient",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentPatient:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedPatient:textSentPatient)) },
				{ "PayPlan Due",(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?textUnsentPayPlanDue:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?textExcludedPayPlanDue:textSentPayPlanDue)) }
			};
			Label labelTotals=(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?labelUnsentTotals:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?labelExcludedTotals:labelSentTotals));
			labelTotals.Visible=false;//set to visible if there are any of the dollar amount columns showing
			Label labelTotalNumAccounts=(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?labelUnsentTotalNumAccts:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?labelExcludedTotalNumAccts:labelSentTotalNumAccts));
			labelTotalNumAccounts.Visible=false;//set to visible if the Guarantor column is showing
			GridOD gridCur=(displayCat==DisplayFieldCategory.ArManagerUnsentGrid?gridUnsent:(displayCat==DisplayFieldCategory.ArManagerExcludedGrid?gridExcluded:gridSent));
			if(listDisplayFields.Any(x => dictColTextBox.ContainsKey(x.InternalName))) {//if any of the display fields that have a textbox are selected
				Dictionary<string,string> dictColInternalNames=listDisplayFields
					.ToDictionary(x => (string.IsNullOrEmpty(x.Description)?x.InternalName:x.Description),x => x.InternalName);
				Dictionary<string,Tuple<int,int>> dictColPosAndWidth=GetXPosAndWidths(gridCur,hScrollValue);//key=column heading, value=Tuple<x-pos,width>
				bool firstCol=true;//used to set location and visibility of the Totals label, set to the left of the first textbox
				foreach(string colName in dictColPosAndWidth.Keys.ToList()) {//go through all columns in the grid
					Tuple<int,int> colLocWidthCur=dictColPosAndWidth[colName];
					string colInternalName;
					if(!dictColInternalNames.TryGetValue(colName,out colInternalName)) {
						//if the grid column heading is not in the list of display fields continue (shouldn't be possible)
						continue;
					}
					TextBox textBoxCur;
					if(!dictColTextBox.TryGetValue(colInternalName,out textBoxCur)) {//if the grid column heading is not associated to a textbox, skip it
						continue;
					}
					if(colInternalName=="Guarantor") {
						//+1 so label doesn't cover right side of textbox if it happens to follow a totals textbox
						LayoutManager.MoveLocation(labelTotalNumAccounts,new Point(colLocWidthCur.Item1+1,gridCur.Bottom+6));
						labelTotalNumAccounts.Visible=true;
						//+2 to account for +1 above and a 1 pixel space between label and textbox
						LayoutManager.MoveLocation(textBoxCur,new Point(colLocWidthCur.Item1+labelTotalNumAccounts.Width+2,gridCur.Bottom+4));
						//width of label+width of textbox+2 will be width of Guarantor column up to a max textbox width of 65, which will hold at least 10 digits,
						//so works for all numbers of accounts up to 9,999,999,999.  It is highly unlikely that any customer will ever have 10 billion guarantors
						//in their aging list.  (That's more than the number of people on the planet, as of 6/27/2018 anyway.)
						LayoutManager.MoveWidth(textBoxCur,Math.Max(Math.Min(colLocWidthCur.Item2-labelTotalNumAccounts.Width-1,65),1));
						continue;
					}
					if(firstCol) {
						//left of the first textbox label width+1 for a space between label and textbox
						LayoutManager.MoveLocation(labelTotals,new Point(colLocWidthCur.Item1-labelTotals.Width-1,gridCur.Bottom+6));
						labelTotals.Visible=true;
						firstCol=false;
					}
					LayoutManager.MoveLocation(textBoxCur,new Point(colLocWidthCur.Item1,gridCur.Bottom+4));//set location and width to the column's x-pos and width
					LayoutManager.MoveWidth(textBoxCur,colLocWidthCur.Item2);
				}
			}
			//hide/unhide textboxes based on the associated display fields
			dictColTextBox.ToList().ForEach(x => x.Value.Visible=listDisplayFields.Any(y => y.InternalName==x.Key));
		}

		private void menuItemGoTo_Click(object sender,EventArgs e) {
			List<PatAging> listPatAgingsAll;
			GridOD gridCur;
			if(tabControlMain.SelectedTab==tabUnsent) {
				gridCur=gridUnsent;
				listPatAgingsAll=_listPatAgingUnsentAll;
			}
			else if(tabControlMain.SelectedTab==tabExcluded) {
				gridCur=gridExcluded;
				listPatAgingsAll=_listPatAgingExcludedAll;
			}
			else {
				gridCur=gridSent;
				listPatAgingsAll=_listPatAgingSentAll;
			}
			if(gridCur.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Please select one patient first.");
				return;
			}
			object pAgeIndex=gridCur.SelectedGridRows[0].Tag;
			if(!(pAgeIndex is int) || (int)pAgeIndex<0 || (int)pAgeIndex>=listPatAgingsAll.Count) {
				return;
			}
			FormOpenDental.S_Contr_PatientSelected(Patients.GetPat(listPatAgingsAll[(int)pAgeIndex].PatNum),false);
			GotoModule.GotoAccount(0);
			SendToBack();
		}

		private void timerFillGrid_Tick(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			ValidateChildren(ValidationConstraints.Enabled|ValidationConstraints.Visible|ValidationConstraints.Selectable);
			if(tabControlMain.SelectedTab==tabUnsent) {
				FillGridUnsent();
			}
			else if(tabControlMain.SelectedTab==tabExcluded) {
				FillGridExcluded();
			}
			else {
				FillGridSent();
			}
		}

		private void tabControlMain_SelectedIndexChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			if(tabControlMain.SelectedTab==tabUnsent){
				SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerUnsentGrid,gridUnsent.HScrollValue);
			}
			if(tabControlMain.SelectedTab==tabSent){
				SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerSentGrid,gridSent.HScrollValue);
			}
			if(tabControlMain.SelectedTab==tabExcluded){
				SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerExcludedGrid,gridExcluded.HScrollValue);
			}
			LayoutManager.LayoutControlBoundsAndFonts(tabControlMain);
		}

		///<summary>Fills the grid, but only if not preceded by a ResizeBegin event, i.e. the form is being manually resized or moved around the screen.</summary>
		private void FormArManager_Resize(object sender,EventArgs e) {
			_isResizing=true;
			if(_hasResizeBegan || WindowState==FormWindowState.Minimized) {
				return;//handle fill grid in ResizeEnd
			}
			//Don't attempt to fill the grid if any of these lists are null.  They're set on load and should never be null once load has finished, but
			//this was firing before load and caused an error on one customer PC.
			if(_listPatAgingUnsentAll==null || _listPatAgingSentAll==null || _listPatAgingExcludedAll==null) {
				_isResizing=false;
				return;
			}
			FillGrids();
			_isResizing=false;
		}

		///<summary>Fires when manual resizing begins.  Sets _hasResizeBegan so the grid is only filled once manual resizing is finished.</summary>
		private void FormArManager_ResizeBegin(object sender,EventArgs e) {
			_hasResizeBegan=true;
		}

		///<summary>Fires when manual resizing is complete, NOT when changing window state. i.e. this is not fired when the window is maximized, minimized
		///or restored.  But this also fires when just moving the form around, so we will use both bools to determine if we have a ResizeBegin and actual 
		///resize events and only then refill the grid.</summary>
		private void FormArManager_ResizeEnd(object sender,EventArgs e) {
			if(_isResizing && _hasResizeBegan) {
				FillGrids();
			}
			_isResizing=false;
			_hasResizeBegan=false;
		}

		private void SaveDefaults() {
			if(!textUnsentMinBal.IsValid() || !textUnsentDaysLastPay.IsValid()
				|| !textSentMinBal.IsValid() || !textSentDaysLastPay.IsValid()
				|| !textExcludedMinBal.IsValid() || !textExcludedDaysLastPay.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			#region Sent Defaults
			string selectedTransTypes="";//indicates all.
			if(comboLastTransType.SelectedIndices.Count>0 && !comboLastTransType.IsAllSelected) {
				selectedTransTypes=string.Join(",",comboLastTransType.GetListSelected<TsiTransType>());
			}
			string sentAgeOfAccount="";//indicates any age
			if(ListTools.In(comboSentAccountAge.SelectedIndex,1,2,3)) {
				sentAgeOfAccount=(30*comboSentAccountAge.SelectedIndex).ToString();//ageOfAccount is 30, 60, or 90
			}
			int sentDaysSinceLastPay=0;
			if(!string.IsNullOrWhiteSpace(textSentDaysLastPay.Text)) {
				sentDaysSinceLastPay=PIn.Int(textSentDaysLastPay.Text);
			}
			double sentMinBal=0.00;
			if(!string.IsNullOrWhiteSpace(textSentMinBal.Text)) {
				sentMinBal=PIn.Double(textSentMinBal.Text);
			}
			#endregion Sent Defaults
			#region Unsent Defaults
			string selectedBillTypes="";//indicates all.
			if(comboBillTypes.SelectedIndices.Count>0 && !comboBillTypes.IsAllSelected) {
				selectedBillTypes=string.Join(",",comboBillTypes.GetListSelected<Def>().Select(x => x.DefNum));
			}
			string unsentAgeOfAccount="";//indicates any age
			if(ListTools.In(comboUnsentAccountAge.SelectedIndex,1,2,3)) {
				unsentAgeOfAccount=(30*comboUnsentAccountAge.SelectedIndex).ToString();//ageOfAccount is 30, 60, or 90
			}
			int unsentDaysSinceLastPay=0;
			if(!string.IsNullOrWhiteSpace(textUnsentDaysLastPay.Text)) {
				unsentDaysSinceLastPay=PIn.Int(textUnsentDaysLastPay.Text);
			}
			double unsentMinBal=0.00;
			if(!string.IsNullOrWhiteSpace(textUnsentMinBal.Text)) {
				unsentMinBal=PIn.Double(textUnsentMinBal.Text);
			}
			#endregion Unsent Defaults
			#region Excluded Defaults
			string excludedAgeOfAccount="";//indicates any age
			if(ListTools.In(comboExcludedAccountAge.SelectedIndex,1,2,3)) {
				excludedAgeOfAccount=(30*comboExcludedAccountAge.SelectedIndex).ToString();//ageOfAccount is 30, 60, or 90
			}
			int excludedDaysSinceLastPay=0;
			if(!string.IsNullOrWhiteSpace(textExcludedDaysLastPay.Text)) {
				excludedDaysSinceLastPay=PIn.Int(textExcludedDaysLastPay.Text);
			}
			double excludedMinBal=0.00;
			if(!string.IsNullOrWhiteSpace(textExcludedMinBal.Text)) {
				excludedMinBal=PIn.Double(textExcludedMinBal.Text);
			}
			#endregion Excluded Defaults
			if( Prefs.UpdateString(PrefName.ArManagerBillingTypes,selectedBillTypes)
				| Prefs.UpdateBool(PrefName.ArManagerExcludeBadAddresses,checkExcludeBadAddress.Checked)
				| Prefs.UpdateBool(PrefName.ArManagerExcludeIfUnsentProcs,checkExcludeIfProcs.Checked)
				| Prefs.UpdateBool(PrefName.ArManagerExcludeInsPending,checkExcludeInsPending.Checked)
				| Prefs.UpdateString(PrefName.ArManagerLastTransTypes,selectedTransTypes)
				| Prefs.UpdateString(PrefName.ArManagerSentAgeOfAccount,sentAgeOfAccount)
				| Prefs.UpdateInt(PrefName.ArManagerSentDaysSinceLastPay,sentDaysSinceLastPay)
				| Prefs.UpdateString(PrefName.ArManagerSentMinBal,POut.Double(sentMinBal))
				| Prefs.UpdateString(PrefName.ArManagerUnsentAgeOfAccount,unsentAgeOfAccount)
				| Prefs.UpdateInt(PrefName.ArManagerUnsentDaysSinceLastPay,unsentDaysSinceLastPay)
				| Prefs.UpdateString(PrefName.ArManagerUnsentMinBal,POut.Double(unsentMinBal))
				| Prefs.UpdateBool(PrefName.ArManagerExcludedExcludeBadAddresses,checkExcludedExcludeBadAddress.Checked)
				| Prefs.UpdateBool(PrefName.ArManagerExcludedExcludeIfUnsentProcs,checkExcludedExcludeIfProcs.Checked)
				| Prefs.UpdateBool(PrefName.ArManagerExcludedExcludeInsPending,checkExcludedExcludeInsPending.Checked)
				| Prefs.UpdateString(PrefName.ArManagerExcludedAgeOfAccount,excludedAgeOfAccount)
				| Prefs.UpdateInt(PrefName.ArManagerExcludedDaysSinceLastPay,excludedDaysSinceLastPay)
				| Prefs.UpdateString(PrefName.ArManagerExcludedMinBal,POut.Double(excludedMinBal)))
			{
				DataValid.SetInvalid(InvalidType.Prefs);
			}
		}

		private void butTsiOcp_Click(object sender,EventArgs e) {
			Process.Start("https://opendental.com/resources/redirects/redirecttransworldocp.html");
		}

		///<summary>Fills listClinicsSkipped with ClinicNums for those clinics that fail connection detail validation.</summary>
		///<param name="listClinicNums">The list of ClinicNums whose connection details should be validated by connecting to the sftp server.</param>
		private bool ValidateSendUpdateData(List<long> listClinicNums,out List<long> listClinicsSkipped) {
			listClinicsSkipped=new List<long>();
			if(_tsiProg==null) {
				MsgBox.Show(this,"The Transworld program link does not exist.  Please contact support.");
				return false;
			}
			if(!_tsiProg.Enabled) {
				MsgBox.Show(this,"The Transworld program link is not enabled.");
				return false;
			}
			if(_dictClinicProgProps.Count==0) {
				MsgBox.Show(this,"The Transworld program link is not setup.  Try again after entering the program link properties.");
				return false;
			}
			Cursor=Cursors.WaitCursor;
			foreach(long clinicNum in listClinicNums.Distinct()) {
				if(!PrefC.HasClinicsEnabled && clinicNum>0) {
					continue;//Only test the HQ clinic (ClinicNum=0) if clinics are not enabled
				}
				if(!_dictClinicProgProps.TryGetValue(clinicNum,out List<ProgramProperty> listProgProps) //if the clinic doesn't have prog props, use HQ's
					&& (clinicNum==0 || !PrefC.HasClinicsEnabled || Security.CurUser.ClinicIsRestricted || !_dictClinicProgProps.TryGetValue(0,out listProgProps)))
				{
					listClinicsSkipped.Add(clinicNum);
					continue;
				}
				if(!TsiTransLogs.ValidateClinicSftpDetails(listProgProps)) {
					listClinicsSkipped.Add(clinicNum);
					continue;
				}
			}
			if(PrefC.HasClinicsEnabled && listClinicsSkipped.Contains(0)) {
				listClinicsSkipped.AddRange(_listClinics.FindAll(x => !_dictClinicProgProps.ContainsKey(x.ClinicNum)).Select(x => x.ClinicNum));
			}
			Cursor=Cursors.Default;
			return true;
		}

		///<summary>Gets the selected PatAgings for the given grid.</summary>
		private List<PatAging> GetSelectedPatAgings(GridOD grid) {
			List<PatAging> listPatAgingsAll=_listPatAgingSentAll;
			if(grid==gridUnsent) {
				listPatAgingsAll=_listPatAgingUnsentAll;
			}
			else if(grid==gridExcluded) {
				listPatAgingsAll=_listPatAgingExcludedAll;
			}
			return grid.SelectedGridRows
					.Where(x => x.Tag is int && (int)x.Tag>=0 && (int)x.Tag<listPatAgingsAll.Count)
					.Select(x => listPatAgingsAll[(int)x.Tag]).ToList();
		}

		///<summary>Selects the given pat nums for the given grid.</summary>
		private void SetSelectedRows(List<long> listPatNums,GridOD grid) {
			if(listPatNums==null || listPatNums.Count==0) {
				return;
			}
			List<PatAging> listPatAgingsAll=_listPatAgingSentAll;
			if(grid==gridUnsent) {
				listPatAgingsAll=_listPatAgingUnsentAll;
			}
			else if(grid==gridExcluded) {
				listPatAgingsAll=_listPatAgingExcludedAll;
			}
			grid.SetSelected(listPatNums.Select(x => grid.ListGridRows.ToList()
					.FindIndex(y => y.Tag is int && (int)y.Tag>=0 && (int)y.Tag<listPatAgingsAll.Count && x==listPatAgingsAll[(int)y.Tag].PatNum))
					.Where(x => x>-1)//Ignore any entries within listPatNums that were not found in our grid.
					.ToArray(),true);
		}

		private void butHistory_Click(object sender,EventArgs e) {
			FormODBase FormTHist=Application.OpenForms.OfType<FormODBase>().Where(x => x!=this).FirstOrDefault(x => x.Name=="FormTsiHistory");
			if(FormTHist==null) {
				FormTHist=new FormTsiHistory();
			}
			FormTHist.Restore();
			FormTHist.Show();
			FormTHist.BringToFront();
		}

		private void menuItemMarkExcluded_Click(object sender,EventArgs e) {
			List<PatAging> listPatAgingsAll;
			GridOD gridCur;
			if(_excludedBilltype==null) {
				MsgBox.Show(this,"There is no \"Excluded\" billing type.  Please designate one in Setup -> Definitions -> Billing Types.");
				return;
			}
			if(tabControlMain.SelectedTab==tabUnsent) {
				gridCur=gridUnsent;
				listPatAgingsAll=_listPatAgingUnsentAll;
			}
			else {//Grid excluded.  GridSent can't use the Excluded menu item.
				gridCur=gridExcluded;
				listPatAgingsAll=_listPatAgingExcludedAll;
			}
			if(gridCur.SelectedGridRows.Count!=1) {
				MsgBox.Show(this,"Please select one patient first.");
				return;
			}
			object pAgeIndex=gridCur.SelectedGridRows[0].Tag;
			if(!(pAgeIndex is int) || (int)pAgeIndex<0 || (int)pAgeIndex>=listPatAgingsAll.Count) {
				return;
			}
			PatAging aging=listPatAgingsAll[(int)pAgeIndex];
			listPatAgingsAll.Remove(aging);
			//If the current grid is the Unsent grid, then they're marking the entry as Excluded.  Otherwise it's the Excluded grid and they're marking it Unsent
			//Add the aging entry to the proper list, set its defnum, and refill grids.
			string strNot="";
			if(gridCur==gridUnsent) {
				_listPatAgingExcludedAll.Add(aging);
				aging.BillingType=_excludedBilltype.DefNum;
			}
			else {
				_listPatAgingUnsentAll.Add(aging);
				aging.BillingType=PrefC.GetLong(PrefName.TransworldPaidInFullBillingType);
				strNot="not ";
			}
			Family famCur=Patients.GetFamily(aging.Guarantor);
			Patients.UpdateFamilyBillingType(aging.BillingType,aging.Guarantor);
			foreach(Patient pat in famCur.ListPats) {
				if(pat.BillingType==aging.BillingType) {
					continue;
				}
				string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+"' to '"
					+Defs.GetName(DefCat.BillingTypes,aging.BillingType)+"' due to the account being marked as "+strNot+"excluded from being sent to "
					+"Transworld from the A/R Manager.";
				SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,pat.PatNum,logTxt);
			}
			FillGridExcluded(false);
			FillGridUnsent(false);
		}

		#endregion Methods For All Tabs
		#region Unsent Tab Methods

		///<summary>Returns a dictionary of key=column heading, value=tuple of Item1=xPos,Item2=width for the given column heading.  Used to position
		///totals texboxes below the corresponding column and to resize/reposition as the form is resized.</summary>
		private Dictionary<string,Tuple<int,int>> GetXPosAndWidths(GridOD grid,int hScrollValue) {
			Dictionary<string,Tuple<int,int>> retval=new Dictionary<string,Tuple<int,int>>();
			int xPos=grid.Location.X-hScrollValue;
			foreach(GridColumn column in grid.ListGridColumns) {
				retval[column.Heading]=Tuple.Create(xPos+1,column.ColWidth+1);//+1 because the textbox lines seem to be slightly thinner than the grid column lines
				xPos+=column.ColWidth;
			}
			return retval;
		}

		private void FillGridUnsent(bool retainSelection=true) {
			Cursor=Cursors.WaitCursor;
			List<long> listSelectedPatNums=new List<long>();
			if(retainSelection) {
				listSelectedPatNums=GetSelectedPatAgings(gridUnsent).Select(x => x.PatNum).ToList();
			}
			List<int> listPatAgingIndexFiltered=GetPatAgingIndexUnsentFiltered();
			#region Set Grid Title and Columns
			int vScrollValue=gridUnsent.VerticalScroll.Value;
			int sortedByColumnIndexOld=gridUnsent.SortedByColumnIdx;
			bool isAscendingOld=gridUnsent.SortedIsAscending;
			gridUnsent.BeginUpdate();
			gridUnsent.ListGridColumns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ArManagerUnsentGrid);
			foreach(DisplayField fieldCur in listDisplayFields) {
				if(fieldCur.InternalName=="Clinic" && !PrefC.HasClinicsEnabled) {
					continue;//skip the clinic column if clinics are not enabled
				}
				GridSortingStrategy sortingStrat=GridSortingStrategy.StringCompare;
				HorizontalAlignment hAlign=HorizontalAlignment.Left;
				if(ListTools.In(fieldCur.InternalName,"0-30 Days","31-60 Days","61-90 Days","> 90 Days","Total","-Ins Est","=Patient","PayPlan Due",
					DisplayFields.InternalNames.ArManagerUnsentGrid.DaysBalBegan)) 
				{
					sortingStrat=GridSortingStrategy.AmountParse;
					hAlign=HorizontalAlignment.Right;
				}
				else if(ListTools.In(fieldCur.InternalName,"Last Paid","Last Proc","DateTime Suspended",DisplayFields.InternalNames.ArManagerUnsentGrid.DateBalBegan)) {
					sortingStrat=GridSortingStrategy.DateParse;
					hAlign=HorizontalAlignment.Center;
				}
				else if(!ListTools.In(fieldCur.InternalName,"Guarantor","Clinic","Prov","Billing Type")) {
					continue;//shouldn't happen, but the loop to fill the rows will skip any columns that aren't one of these so we'll skip here as well
				}
				gridUnsent.ListGridColumns.Add(new GridColumn(string.IsNullOrEmpty(fieldCur.Description)?fieldCur.InternalName:fieldCur.Description,
					fieldCur.ColumnWidth,hAlign,sortingStrat));
			}
			//this form initially set to the max allowed (by OD) form size 1246, which is also the minimum size for this form.  If the user resizes the form
			//to be larger, increase each column width by the same ratio to spread out the additional real estate
			int widthColsAndScroll=gridUnsent.ListGridColumns.Sum(x => x.ColWidth)+20;//+20 for vertical scroll bar
			//widthColsAndScroll is width of columns as set in the display fields, i.e. haven't grown or shrunk due to form resizing so won't be shrunk to
			//less than the sizes set in display fields, only grown from there.  Thus the display field sizes are basically a minimum size.
			if(widthColsAndScroll<gridUnsent.Width) {
				gridUnsent.ListGridColumns.ToList().ForEach(x => x.ColWidth=(int)Math.Round((float)x.ColWidth*gridUnsent.Width/widthColsAndScroll,MidpointRounding.AwayFromZero));
				//adjust the last col width to take any remaining pixels so that the cols take the full width of the grid (to account for rounding above)
				gridUnsent.ListGridColumns[gridUnsent.ListGridColumns.Count-1].ColWidth-=gridUnsent.ListGridColumns.Sum(x => x.ColWidth)+20-gridUnsent.Width;
			}
			#endregion Set Grid Title and Columns
			#region Fill Grid Rows
			gridUnsent.ListGridRows.Clear();
			Dictionary<long,string> dictClinicAbbrs=_listClinics.ToDictionary(x => x.ClinicNum,x => x.Abbr);
			Dictionary<long,string> dictProvAbbrs=_listProviders.ToDictionary(x => x.ProvNum,x => x.Abbr);
			Dictionary<long,string> dictBillTypeNames=Defs.GetDefsForCategory(DefCat.BillingTypes).ToDictionary(x => x.DefNum,x => x.ItemName);
			Dictionary<long,DateTime> dictSuspendDateTimes=new Dictionary<long,DateTime>();
			foreach(PatAging pAgeCur in listPatAgingIndexFiltered.Select(x => _listPatAgingUnsentAll[x])) {
				TsiTransLog tsiLogMostRecentStatusChange=pAgeCur.ListTsiLogs
					.Find(x => ListTools.In(x.TransType,TsiTransType.CN,TsiTransType.PF,TsiTransType.PL,TsiTransType.PT,TsiTransType.RI,TsiTransType.SS));
				if(tsiLogMostRecentStatusChange!=null && tsiLogMostRecentStatusChange.TransType==TsiTransType.SS) {
					dictSuspendDateTimes.Add(pAgeCur.PatNum,tsiLogMostRecentStatusChange.TransDateTime);
				}
			}
			double bal0_30=0;
			double bal31_60=0;
			double bal61_90=0;
			double balOver90=0;
			double balTotal=0;
			double insEst=0;
			double amtDue=0;
			double ppDue=0;
			GridRow row;
			List<int> listIndicesToReselect=new List<int>();
			foreach(int i in listPatAgingIndexFiltered) {
				PatAging patAgeCur=_listPatAgingUnsentAll[i];
				bal0_30+=patAgeCur.Bal_0_30;
				bal31_60+=patAgeCur.Bal_31_60;
				bal61_90+=patAgeCur.Bal_61_90;
				balOver90+=patAgeCur.BalOver90;
				balTotal+=patAgeCur.BalTotal;
				insEst+=patAgeCur.InsEst;
				amtDue+=patAgeCur.AmountDue;
				ppDue+=patAgeCur.PayPlanDue;
				row=new GridRow();
				foreach(DisplayField field in listDisplayFields) {
					switch(field.InternalName) {
						case "Guarantor":
							row.Cells.Add((checkUnsentShowPatNums.Checked?(patAgeCur.PatNum.ToString()+" - "):"")+patAgeCur.PatName);
							continue;
						case "Clinic":
							if(!PrefC.HasClinicsEnabled) {
								continue;//skip the clinic column if clinics are not enabled
							}
							string clinicAbbr;
							row.Cells.Add(dictClinicAbbrs.TryGetValue(patAgeCur.ClinicNum,out clinicAbbr)?clinicAbbr:"");
							continue;
						case "Prov":
							string provAbbr;
							row.Cells.Add(dictProvAbbrs.TryGetValue(patAgeCur.PriProv,out provAbbr)?provAbbr:"");
							continue;
						case "Billing Type":
							string billTypeName;
							row.Cells.Add(dictBillTypeNames.TryGetValue(patAgeCur.BillingType,out billTypeName)?billTypeName:"");
							continue;
						case "0-30 Days":
							row.Cells.Add(patAgeCur.Bal_0_30.ToString("n"));
							continue;
						case "31-60 Days":
							row.Cells.Add(patAgeCur.Bal_31_60.ToString("n"));
							continue;
						case "61-90 Days":
							row.Cells.Add(patAgeCur.Bal_61_90.ToString("n"));
							continue;
						case "> 90 Days":
							row.Cells.Add(patAgeCur.BalOver90.ToString("n"));
							continue;
						case "Total":
							row.Cells.Add(patAgeCur.BalTotal.ToString("n"));
							continue;
						case "-Ins Est":
							row.Cells.Add(patAgeCur.InsEst.ToString("n"));
							continue;
						case "=Patient":
							row.Cells.Add(patAgeCur.AmountDue.ToString("n"));
							continue;
						case "PayPlan Due":
							row.Cells.Add(patAgeCur.PayPlanDue.ToString("n"));
							continue;
						case "Last Paid":
							row.Cells.Add(patAgeCur.DateLastPay.Year>1880?patAgeCur.DateLastPay.ToString("d"):"");
							continue;
						case "DateTime Suspended":
							TsiTransLog suspendLog=patAgeCur.ListTsiLogs.Find(x => x.TransType==TsiTransType.SS);
							row.Cells.Add(suspendLog!=null?suspendLog.TransDateTime.ToString():"");
							continue;
						case "Last Proc":
							row.Cells.Add(patAgeCur.DateLastProc.Year>1880?patAgeCur.DateLastProc.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerUnsentGrid.DateBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?patAgeCur.DateBalBegan.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerUnsentGrid.DaysBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?(DateTime.Today-patAgeCur.DateBalBegan).Days.ToString():"");
							continue;
						default:
							continue;//skip any columns not defined here, as we did above when adding the columns
					}
				}
				if(patAgeCur.Birthdate.Year<1880//invalid bday
					|| patAgeCur.Birthdate>DateTime.Today.AddYears(-18)//under 18 years old
					|| new[] { patAgeCur.Address,patAgeCur.City,patAgeCur.State,patAgeCur.Zip }.Any(x => string.IsNullOrWhiteSpace(x)))//missing address information
				{
					//color row light red/pink, using cell color so selecting row still shows color
					row.Cells.OfType<GridCell>().ToList().ForEach(x => x.ColorBackG=Color.FromArgb(255,255,230,234));
				}
				row.Tag=i;//tag the row with the index in the class-wide list of all unsent PatAgings
				gridUnsent.ListGridRows.Add(row);
				if(retainSelection && listSelectedPatNums.Contains(patAgeCur.PatNum)) {
					listIndicesToReselect.Add(gridUnsent.ListGridRows.Count-1);
				}
			}
			gridUnsent.EndUpdate();
			gridUnsent.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridUnsent.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			#endregion Fill Grid Rows
			groupPlaceAccounts.Enabled=(gridUnsent.ListGridRows.Count>0);
			if(retainSelection && listIndicesToReselect.Count>0) {
				gridUnsent.SetSelected(listIndicesToReselect.ToArray(),true);
			}
			SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerUnsentGrid,gridUnsent.HScrollValue);
			textUnsentTotalNumAccts.Text=listPatAgingIndexFiltered.Count.ToString();
			textUnsent0to30.Text=bal0_30.ToString("n");
			textUnsent31to60.Text=bal31_60.ToString("n");
			textUnsent61to90.Text=bal61_90.ToString("n");
			textUnsentOver90.Text=balOver90.ToString("n");
			textUnsentTotal.Text=balTotal.ToString("n");
			textUnsentInsEst.Text=insEst.ToString("n");
			textUnsentPatient.Text=amtDue.ToString("n");
			textUnsentPayPlanDue.Text=ppDue.ToString("n");
			Cursor=Cursors.Default;
		}

		private List<int> GetPatAgingIndexUnsentFiltered() {
			List<int> retval=new List<int>();
			#region Validate Inputs
			if(!textUnsentMinBal.IsValid() || !textUnsentDaysLastPay.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors in Unsent tab first.");
				return retval;//return empty list, filter inputs cannot be applied since there are errors
			}
			#endregion Validate Inputs
			#region Get Filter Data
			double minBalance=Math.Round(PIn.Double(textUnsentMinBal.Text),3);
			DateTime dtLastPay=DateTime.Today.AddDays(-PIn.Int(textUnsentDaysLastPay.Text));
			AgeOfAccount accountAge=new[] { AgeOfAccount.Any,AgeOfAccount.Over30,AgeOfAccount.Over60,AgeOfAccount.Over90 }[comboUnsentAccountAge.SelectedIndex];
			List<long> listBillTypes=new List<long>();
			if(!comboBillTypes.IsAllSelected) {
				listBillTypes=comboBillTypes.GetListSelected<Def>().Select(x => x.DefNum).ToList();
			}
			List<long> listProvNums=new List<long>();
			if(!comboProvsUnsent.IsAllSelected) {
				listProvNums=comboProvsUnsent.GetSelectedProvNums();
			}
			#endregion Get Filter Data
			#region Apply Filter Data to PatAging List
			for(int i=0;i<_listPatAgingUnsentAll.Count;i++) {
				PatAging pAge=_listPatAgingUnsentAll[i];
				if(Math.Round(pAge.AmountDue,3) >= minBalance
					&& (dtLastPay.Date>=DateTime.Today.Date || pAge.DateLastPay.Date<dtLastPay.Date)
					&& (listBillTypes.Count==0 || listBillTypes.Contains(pAge.BillingType))
					&& (listProvNums.Count==0 || listProvNums.Contains(pAge.PriProv))
					&& (!checkExcludeBadAddress.Checked || new[] { pAge.Address,pAge.City,pAge.State,pAge.Zip }.All(y => !string.IsNullOrWhiteSpace(y)))
					&& (!checkExcludeIfProcs.Checked || !pAge.HasUnsentProcs)
					&& (!checkExcludeInsPending.Checked || !pAge.HasInsPending)
					&& ( ((int)accountAge < 4 && pAge.BalOver90 > 0.005)//if Any, Over30, Over60 or Over90 are selected, check BalOver90
						|| ((int)accountAge < 3 && pAge.Bal_61_90 > 0.005)//if Any, Over30 or Over60 are selected, check Bal_61_90
						|| ((int)accountAge < 2 && pAge.Bal_31_60 > 0.005)//if Any or Over30 are selected, check Bal_31_60
						|| (int)accountAge < 1 ))//or if Any bal is selected
				{
					retval.Add(i);
				}
			}
			#endregion Apply Filter Data to PatAging List
			return retval;
		}

		private void gridUnsentMain_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right){
				gridUnsent.SetAll(false);
				menuItemMarkExcluded.Visible=true;
				menuItemMarkExcluded.Text="Mark Excluded";
			}
			else {
				_listSelectedPatNums=GetSelectedPatAgings(gridUnsent).Select(x => x.PatNum).ToList();
			}
		}

		private void gridUnsentMain_ColumnSorted(object sender,EventArgs e) {
				SetSelectedRows(_listSelectedPatNums,gridUnsent);
		}

		private void gridUnsentMain_HorizScrolled(object sender,ScrollEventArgs e) {
			SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerUnsentGrid,e.NewValue);
		}

		private void gridUnsent_MouseMove(object sender,MouseEventArgs e) {
			try {
				if(_lastCursorPos==e.Location) {
					return;
				}
				if(e.Button!=MouseButtons.None) {
					_toolTipUnsentErrors.RemoveAll();
					return;
				}
				int colIndex=gridUnsent.PointToCol(e.X);
				if(colIndex<0) {
					return;
				}
				int rowIndex=gridUnsent.PointToRow(e.Y);
				if(rowIndex<0) {
					return;
				}
				int lastRowIndex=gridUnsent.PointToRow(_lastCursorPos.Y);
				if(rowIndex==lastRowIndex) {
					return;
				}
				object pAgeIndex=gridUnsent.ListGridRows[rowIndex].Tag;
				if(!(pAgeIndex is int) || (int)pAgeIndex<0 || (int)pAgeIndex>=_listPatAgingUnsentAll.Count) {
					_toolTipUnsentErrors.RemoveAll();
					return;
				}
				PatAging pAgeCur=_listPatAgingUnsentAll[(int)pAgeIndex];
				List<string> listErrors=new List<string>();
				if(pAgeCur.Birthdate.Year<1880 || pAgeCur.Birthdate>DateTime.Today.AddYears(-18)) {
					listErrors.Add(Lan.g(this,"Birthdate"));
				}
				if(new[] { pAgeCur.Address,pAgeCur.City,pAgeCur.State,pAgeCur.Zip }.Any(x => string.IsNullOrWhiteSpace(x))) {
					listErrors.Add(Lan.g(this,"Address"));
				}
				if(listErrors.Count==0) {
					_toolTipUnsentErrors.RemoveAll();
					return;
				}
				_toolTipUnsentErrors.SetToolTip(gridUnsent,Lan.g(this,"Invalid")+" "+string.Join(" "+Lan.g(this,"and")+" ",listErrors));
			}
			catch(Exception ex) {
				_toolTipUnsentErrors.RemoveAll();
				ex.DoNothing();
			}
			finally {
				_lastCursorPos=e.Location;
			}
		}

		private void comboBoxMultiUnsentProvs_Leave(object sender,EventArgs e) {
			if(comboProvsUnsent.SelectedIndices.Count==0) {
				comboProvsUnsent.IsAllSelected=true;
			}
		}

		private void comboBoxMultiBillTypes_Leave(object sender,EventArgs e) {
			if(comboBillTypes.SelectedIndices.Count==0) {
				comboBillTypes.IsAllSelected=true;
			}
		}

		private void FillServiceTypes() {
			TsiServiceType selectedType=comboServiceType.GetSelected<TsiServiceType>();//If they have nothing selected, this will default to 'Accelerator'.
			comboServiceType.Enabled=true;
			butSend.Enabled=true;
			comboServiceType.Items.Clear();
			Dictionary<long,string[]> dictClinicSelectedServices=_dictClinicProgProps
				.Where(x => x.Value.Any(y => y.PropertyDesc=="SelectedServices" && !string.IsNullOrEmpty(y.PropertyValue)))
				.ToDictionary(x => x.Key,x => x.Value.Find(y => y.PropertyDesc=="SelectedServices").PropertyValue.Split(','));
			if(dictClinicSelectedServices.TryGetValue(_selectedClinicNum,out string[] arrayPropVals)) {
				if(arrayPropVals.Contains(((int)TsiServiceType.Accelerator).ToString())) {
					comboServiceType.Items.Add(TsiServiceType.Accelerator.GetDescription(),TsiServiceType.Accelerator);
				}
				if(arrayPropVals.Contains(((int)TsiServiceType.ProfitRecovery).ToString())) {
					comboServiceType.Items.Add(TsiServiceType.ProfitRecovery.GetDescription(),TsiServiceType.ProfitRecovery);
				}
				if(arrayPropVals.Contains(((int)TsiServiceType.ProfessionalCollections).ToString())) {
					comboServiceType.Items.Add(TsiServiceType.ProfessionalCollections.GetDescription(),TsiServiceType.ProfessionalCollections);
				}
			}
			if(comboServiceType.Items.Count>0) {
				comboServiceType.SetSelectedEnum(selectedType);
				if(comboServiceType.SelectedIndex==-1) {
					comboServiceType.SetSelected(0);
				}
			}
			else {
				comboServiceType.Enabled=false;
				butSend.Enabled=false;
			}
		}

		private void comboBoxMultiUnsentProvs_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void comboBoxMultiUnsentBillTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void comboUnsentAccountAge_SelectedIndexChanged(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void textUnsentMinBal_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void textUnsentDaysLastPay_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void checkExcludeInsPending_CheckedChanged(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void checkExcludeIfProcs_CheckedChanged(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void checkExcludeBadAddress_CheckedChanged(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void checkUnsentShowPatNums_CheckedChanged(object sender,EventArgs e) {
			FillGridUnsent();
		}

		private void butUnsentSaveDefault_Click(object sender,EventArgs e) {
			SaveDefaults();
		}

		private void butUnsentAll_Click(object sender,EventArgs e) {
			gridUnsent.SetAll(true);
		}

		private void butUnsentNone_Click(object sender,EventArgs e) {
			gridUnsent.SetAll(false);
		}

		private void butUnsentPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"TSI collections unsent printed"), PrintoutOrientation.Landscape);
		}

		//Copied from FormRpOutstandingIns.cs and edited
		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			GridOD gridCur;
			if(tabControlMain.SelectedTab==tabUnsent) {
				gridCur=gridUnsent;
			}
			else if(tabControlMain.SelectedTab==tabSent) {
				gridCur=gridSent;
			}
			else {
				gridCur=gridExcluded;
			}
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_headingPrinted) {
				g.DrawString(Text,headingFont,Brushes.Black,center-g.MeasureString(Text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(Text,headingFont).Height;
				g.DrawString(gridCur.Title,subHeadingFont,Brushes.Black,center-g.MeasureString(gridCur.Title,subHeadingFont).Width/2,yPos);
				yPos+=20;
				//print today's date
				text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				_headingPrinted=true;
				_headingPrintH=yPos;
			}
			#endregion
			yPos=gridCur.PrintPage(g,_pagesPrinted,bounds,_headingPrintH);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butRunAging_Click(object sender,EventArgs e) {
			RunAgingIfNecessary(isAutomatic:false);
			RefreshAll();
			FillGrids();
		}

		private void butSend_Click(object sender,EventArgs e) {
			#region Get and Validate Data
			if(!Security.IsAuthorized(Permissions.Billing)) {
				return;
			}
			GridOD gridCur=(tabControlMain.SelectedTab==tabUnsent ? gridUnsent : gridExcluded);
			if(gridCur.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Please select accounts to send to TSI first.");
				return;
			}
			if(_collectionBillType==null) {
				if(Security.IsAuthorized(Permissions.Setup)
					&& MsgBox.Show(this,MsgBoxButtons.YesNo,"There must be a collections billing type defined in order to send accounts to TSI.  Would you like "
						+"to open the definitions window now to create a collections billing type?"))
				{
					using FormDefinitions FormDefs=new FormDefinitions(DefCat.BillingTypes);
					FormDefs.ShowDialog();//no OK button, only Close which returns DialogResult.Cancel, just get the billing type again in case they created it
					_collectionBillType=Defs.GetDefsForCategory(DefCat.BillingTypes,true).FirstOrDefault(x => x.ItemValue.ToLower()=="c");
				}
				using FormDefinitions FormD=new FormDefinitions(DefCat.BillingTypes);
				FormD.ShowDialog();//no OK button, only Close which returns DialogResult.Cancel, just get the billing type again in case they created it
				_collectionBillType=Defs.GetDefsForCategory(DefCat.BillingTypes,true).FirstOrDefault(x => x.ItemValue.ToLower()=="c");
				if(_collectionBillType==null) {//still no collections billing type
					MsgBox.Show(this,"Please create a collections billing type and try again later.");
					return;
				}
			}
			List<PatAging> listPatAging=GetSelectedPatAgings(gridCur);
			List<long> listClinicsSkipped;
			if(!ValidateSendUpdateData(listPatAging.Select(x => x.ClinicNum).ToList(),out listClinicsSkipped)) {
				return;
			}
			if(_dictClinicProgProps.All(x => listClinicsSkipped.Contains(x.Key))) {
				MsgBox.Show(this,"An SFTP connection could not be made using the connection details "+(PrefC.HasClinicsEnabled ? "for any clinic " : "")
					+"in the enabled Transworld (TSI) program link.  Accounts cannot be sent to collection until the program link is setup.");
				return;
			}
			//Warn of any outstanding claims
			List<Family> listFamiliesOutstanding=new List<Family>();
			bool outstandingFound=false;
			Dictionary<long,Family> dictFams=Patients.GetFamilies(listPatAging.Select(x => x.PatNum).ToList()).ToDictionary(x => x.Guarantor.PatNum);
			foreach(long guarCur in listPatAging.Select(x => x.PatNum)) {//for each guarantor selected, get the family
				if(!dictFams.TryGetValue(guarCur,out Family famCur)) {
					continue;
				}
				foreach(long patCur in famCur.ListPats.Select(x => x.PatNum)) {//for each patient in the family, check for oustanding claims
					List<Claim> claimsForPat=Claims.GetForPat(patCur);
					if(!outstandingFound && claimsForPat.Any(x => x.ClaimStatus!="R" && !Claims.IsClaimPreAuth(x))) {//true if outstanding claim found
						string message=Lan.g(this,"There are outstanding claims on one or more of the selected accounts. Send this patient"
							+" to Transworld for account collection services anyway?");
						if(!MsgBox.Show(MsgBoxButtons.YesNo,message)) {
							return;
						}
						listFamiliesOutstanding.Add(famCur);
						outstandingFound=true;
					}
					if(outstandingFound) {
						break;
					}
				}
			}
			Cursor=Cursors.WaitCursor;
			//TSI connection details validated, at least one clinic the user has access to is setup with valid connection details
			#region Get Age of Accounts Dictionary
			DateTime dateAsOf=DateTime.Today;//used to determine when the balance on this date began
			if(PrefC.GetBool(PrefName.AgingCalculatedMonthlyInsteadOfDaily)) {//if aging calculated monthly, use the last aging date instead of today
				dateAsOf=PrefC.GetDate(PrefName.DateLastAging);
			}
			#endregion Get PatAgings and Age of Accounts Dictionary
			#region Validate Selected Pats and Service Type
			Dictionary<long,string[]> dictClinicSelectedServices=_dictClinicProgProps
				.Where(x => x.Value.Any(y => y.PropertyDesc=="SelectedServices" && !string.IsNullOrEmpty(y.PropertyValue)))
				.ToDictionary(x => x.Key,x => x.Value.Find(y => y.PropertyDesc=="SelectedServices").PropertyValue.Split(','));
			TsiServiceType serviceType=(tabControlMain.SelectedTab==tabUnsent?comboServiceType.GetSelected<TsiServiceType>():comboExcludedServiceType.GetSelected<TsiServiceType>());
			List<long> listPatNumsToReselect=listPatAging.FindAll(x => !dictClinicSelectedServices.ContainsKey(PrefC.HasClinicsEnabled?x.ClinicNum:0)
						|| !dictClinicSelectedServices[PrefC.HasClinicsEnabled?x.ClinicNum:0].Contains(((int)serviceType).ToString())).Select(x => x.PatNum).ToList();
			string msgTxt="";
			if(listPatNumsToReselect.Count > 0)	{
				Cursor=Cursors.Default;
				msgTxt=Lan.g(this,"At least one of the selected guarantors is assigned to a clinic that does not have the")+" "+serviceType.GetDescription()
					+" "+Lan.g(this,"service enabled.  Those account(s) will not be sent to TSI and will remain in the unsent grid.");
				MessageBox.Show(msgTxt);
			}
			List<long> listPatNumsWrongService=new List<long>();
			if(serviceType==TsiServiceType.Accelerator) {
				listPatNumsWrongService=listPatAging.FindAll(x => !listPatNumsToReselect.Contains(x.PatNum) && x.DateBalBegan.Date<dateAsOf.AddDays(-180).Date)
					.Select(x => x.PatNum).ToList();
				msgTxt=Lan.g(this,"The accelerator service is recommended for any accounts less than 180 days old and one or more of the selected account balances "
					+"is over 180 days old.  You may wish to consider sending those accounts directly to the Profit Recovery or Professional Collections service.")+"\r\n"
					+Lan.g(this,"Would you like to send the accounts to Accelerator?")+"\r\n\r\n"+Lan.g(this,"Press Yes to send all accounts to Accelerator.")
					+"\r\n\r\n"+Lan.g(this,"Press No to send only the accounts 180 days old or less to Accelerator and leave the older accounts in the unsent "
					+"grid to send to Profit Recovery or Professional Collections later.")+"\r\n\r\n"+Lan.g(this,"Press Cancel to cancel sending all accounts.");
			}
			else if(serviceType==TsiServiceType.ProfitRecovery) {
				listPatNumsWrongService=listPatAging.FindAll(x => !listPatNumsToReselect.Contains(x.PatNum) && x.DateBalBegan.Date<dateAsOf.AddDays(-365).Date)
					.Select(x => x.PatNum).ToList();
				msgTxt=Lan.g(this,"The Profit Recovery service is recommended for any accounts less than 365 days old and one or more of the selected account "
					+"balances is over 365 days old.  You may wish to consider sending those accounts directly to the Professional Collections service.")+"\r\n"
					+Lan.g(this,"Would you like to send the accounts to Profit Recovery?")+"\r\n\r\n"+Lan.g(this,"Press Yes to send all accounts to Profit "
					+"Recovery.")+"\r\n\r\n"+Lan.g(this,"Press No to send only the accounts 365 days old or less to Profit Recovery and leave the older "
					+"accounts in the unsent grid to send to Professional Collections later.")+"\r\n\r\n"+Lan.g(this,"Press Cancel to cancel sending all accounts.");
			}
			if(listPatNumsWrongService.Count > 0) {
				Cursor=Cursors.Default;
				switch(MessageBox.Show(msgTxt,"",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.No:
						listPatNumsToReselect.AddRange(listPatNumsWrongService);
						break;
					case DialogResult.Cancel:
						return;
					default:
						break;
				}
			}
			#endregion Validate Selected Pats and Service Type
			#region Validate Birthdate and Address
			List<string> listErrorMsgs=new List<string>();
			List<long> listPatNumsBadBday=listPatAging
				.FindAll(x => !listPatNumsToReselect.Contains(x.PatNum) && (x.Birthdate.Year<1880 || x.Birthdate>DateTime.Today.AddYears(-18)))
				.Select(x => x.PatNum).ToList();
			if(listPatNumsBadBday.Count>0) {
				listErrorMsgs.Add(Lan.g(this,"Invalid birthdate or under the age of 18"));
				listPatNumsToReselect.AddRange(listPatNumsBadBday);
			}
			List<long> listPatNumsBadAddress=listPatAging
				.FindAll(x => !listPatNumsToReselect.Contains(x.PatNum)
					&& new[] { x.Address,x.City,x.State,x.Zip }.Any(y => string.IsNullOrEmpty(y)))
				.Select(x => x.PatNum).ToList();
			if(listPatNumsBadAddress.Count>0) {
				listErrorMsgs.Add(Lan.g(this,"Bad address"));
				listPatNumsToReselect.AddRange(listPatNumsBadAddress);
			}
			if(listErrorMsgs.Count>0) {
				Cursor=Cursors.Default;
				msgTxt=Lan.g(this,"One or more of the selected guarantors has the following error(s) and will not be sent to TSI")+":\r\n\r\n"
					+string.Join("\r\n",listErrorMsgs);
				MessageBox.Show(msgTxt);
			}
			#endregion Validate Birthdate and Address
			#region Validate Balances
			List<long> listPatNumsNegBal=listPatAging
				.FindAll(x => !listPatNumsToReselect.Contains(x.PatNum) && Math.Round(x.AmountDue,3) < 0.005)
				.Select(x => x.PatNum).ToList();
			if(listPatNumsNegBal.Count>0) {
				Cursor=Cursors.Default;
				msgTxt=listPatNumsNegBal.Count+" "+Lan.g(this,"of the selected guarantor(s) have a balance less than or equal to 0.  Are you sure you want "
					+"to send the account(s) to TSI?")+"\r\n\r\n"
					+Lan.g(this,"Press Yes to send the account(s) with a balance less than or equal to 0 anyway.")+"\r\n\r\n"
					+Lan.g(this,"Press No to skip the account(s) with a balance less than or equal to 0 and send the remaining account(s) to TSI.")+"\r\n\r\n"
					+Lan.g(this,"Press Cancel to cancel sending all accounts.");
				switch(MessageBox.Show(msgTxt,"",MessageBoxButtons.YesNoCancel)) {
					case DialogResult.No:
						listPatNumsToReselect.AddRange(listPatNumsNegBal);
						break;
					case DialogResult.Cancel:
						return;
					default:
						break;
				}
			}
			#endregion Validate Balances
			Cursor=Cursors.WaitCursor;
			listPatAging.RemoveAll(x => listPatNumsToReselect.Contains(x.PatNum));
			if(listPatAging.Count==0) {
				SetSelectedRows(listPatNumsToReselect,gridCur);
				Cursor=Cursors.Default;
				return;
			}
			//dictionary key=PatNum, value=dictionary key=Tuple<TsiFKeyType,long>, value=TsiTrans with that type and key for that pat.  Used for placement
			//msgs to keep all trans for a fam associated with the placement msg to determine later if the past account details were changed after being
			//placed for collection with Transworld.
			Dictionary<long,Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>> dictPatTrans=Ledgers.GetDictTransForGuars(listPatAging.Select(x => x.PatNum).ToList());
			#endregion Get and Validate Data
			#region Create Messages and TsiTransLogs
			Dictionary<long,Dictionary<long,string>> dictClinicUpdateMsgs=new Dictionary<long,Dictionary<long,string>>();
			Dictionary<long,Dictionary<long,string>> dictClinicPlacementMsgs=new Dictionary<long,Dictionary<long,string>>();
			Dictionary<long,List<TsiTransLog>> dictClinicNumListTransLogs=new Dictionary<long,List<TsiTransLog>>();
			List<long> listFailedPatNums=new List<long>();
			foreach(PatAging pAgingCur in listPatAging) {
				long clinicNum=PrefC.HasClinicsEnabled?pAgingCur.ClinicNum:0;
				if(listClinicsSkipped.Contains(clinicNum)) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					continue;
				}
				string clientID="";
				List<ProgramProperty> listProgProps;
				if(!_dictClinicProgProps.TryGetValue(clinicNum,out listProgProps) && !_dictClinicProgProps.TryGetValue(0,out listProgProps)) {
					listClinicsSkipped.Add(clinicNum);
					listFailedPatNums.Add(pAgingCur.PatNum);
					continue;
				}
				if(serviceType==TsiServiceType.Accelerator) {
					clientID=listProgProps.Find(x => x.PropertyDesc=="ClientIdAccelerator")?.PropertyValue??"";
				}
				else {
					clientID=listProgProps.Find(x => x.PropertyDesc=="ClientIdCollection")?.PropertyValue??"";
				}
				try {
					//find most recent account change log less than 50 days ago and if it was a suspend trans send reinstate update msg instead of placement msg
					TsiTransLog logMostRecentAcctChange=pAgingCur.ListTsiLogs
						.Find(x => ListTools.In(x.TransType,TsiTransType.CN,TsiTransType.PF,TsiTransType.PL,TsiTransType.PT,TsiTransType.RI,TsiTransType.SS)
							&& x.TransDateTime>=DateTime.Today.AddDays(-50));
					if(logMostRecentAcctChange!=null && logMostRecentAcctChange.TransType==TsiTransType.SS) {
						//most recent account change trans was less than 50 days ago and was to suspend the account, so generate and send update message to reinstate
						string updateStatusMsg=TsiMsgConstructor.GenerateUpdate(pAgingCur.PatNum,clientID,TsiTransType.RI,0.00,pAgingCur.AmountDue);
						if(!dictClinicUpdateMsgs.ContainsKey(clinicNum)) {
							dictClinicUpdateMsgs[clinicNum]=new Dictionary<long,string>();
						}
						dictClinicUpdateMsgs[clinicNum].Add(pAgingCur.PatNum,updateStatusMsg);
						if(!dictClinicNumListTransLogs.ContainsKey(clinicNum)) {
							dictClinicNumListTransLogs[clinicNum]=new List<TsiTransLog>();
						}
						dictClinicNumListTransLogs[clinicNum].Add(new TsiTransLog() {
							PatNum=pAgingCur.PatNum,
							UserNum=Security.CurUser.UserNum,
							TransType=TsiTransType.RI,
							//TransDateTime=DateTime.Now,//set on insert, not editable by user
							//ServiceType=TsiServiceType.Accelerator,//only used for placement messages
							//ServiceCode=TsiServiceCode.Diplomatic,//only used for placement messages
							ClientId=clientID,
							TransAmt=0.00,
							AccountBalance=pAgingCur.AmountDue,
							FKeyType=TsiFKeyType.None,//only used for account trans updates
							FKey=0,//only used for account trans updates
							RawMsgText=updateStatusMsg,
							ClinicNum=clinicNum,
							DictTransByType=new Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>()//sets string field TransJson to empty string
						});
					}
					else {
						string msg=TsiMsgConstructor.GeneratePlacement(pAgingCur,clientID,serviceType);
						if(!dictClinicPlacementMsgs.ContainsKey(clinicNum)) {
							dictClinicPlacementMsgs[clinicNum]=new Dictionary<long,string>();
						}
						dictClinicPlacementMsgs[clinicNum].Add(pAgingCur.PatNum,msg);
						TsiTransLog logCur=new TsiTransLog() {
							PatNum=pAgingCur.PatNum,
							UserNum=Security.CurUser.UserNum,
							TransType=TsiTransType.PL,
							//TransDateTime=DateTime.Now,//set on insert, not editable by user
							ServiceType=serviceType,
							ServiceCode=TsiServiceCode.Diplomatic,
							ClientId=clientID,
							TransAmt=0.00,
							AccountBalance=pAgingCur.AmountDue,
							FKeyType=TsiFKeyType.None,//not used for placement messages
							FKey=0,//not used for placement messages
							RawMsgText=msg,
							ClinicNum=clinicNum,
							DictTransByType=new Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>()//sets string field TransJson to empty string
						};
						Dictionary<Tuple<TsiFKeyType,long>,TsiTrans> dictPatCurTrans;
						if(dictPatTrans.TryGetValue(logCur.PatNum,out dictPatCurTrans)) {
							logCur.DictTransByType=new Dictionary<Tuple<TsiFKeyType,long>,TsiTrans>(dictPatCurTrans);//sets TransJson to Json serialized string
						}
						if(!dictClinicNumListTransLogs.ContainsKey(clinicNum)) {
							dictClinicNumListTransLogs[clinicNum]=new List<TsiTransLog>();
						}
						dictClinicNumListTransLogs[clinicNum].Add(logCur);
						if(!CompareDouble.IsEqual(logCur.AccountBalance,logCur.DictTransByType.Sum(x => x.Value.TranAmt))) {
							throw new ApplicationException("The guarantor's amount due does not match the sum of ledger transactions.  The following guarantor was "
								+"not sent to Transworld.  Try running aging and/or Database Maintenance and then try sending this guarantor again.\r\n"
								+"Patient: "+pAgingCur.PatNum+" - "+pAgingCur.PatName);
						}
					}
				}
				catch(ApplicationException ex) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					if(dictClinicUpdateMsgs.ContainsKey(clinicNum)) {
						dictClinicUpdateMsgs[clinicNum].Remove(pAgingCur.PatNum);
					}
					if(dictClinicPlacementMsgs.ContainsKey(clinicNum)) {
						dictClinicPlacementMsgs[clinicNum].Remove(pAgingCur.PatNum);
					}
					if(dictClinicNumListTransLogs.ContainsKey(clinicNum)) {
						dictClinicNumListTransLogs[clinicNum].RemoveAll(x => x.PatNum==pAgingCur.PatNum);
					}
					Cursor=Cursors.Default;
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,ex.Message+"\r\nDo you want to continue attempting to send the remaining accounts?")) {
						Cursor=Cursors.WaitCursor;
						continue;
					}
					else {
						break;
					}
				}
			}
			#endregion Create Messages and TsiTransLogs
			#region Send Clinic Batch Placement Files, Insert TsiTransLogs, and Update Patient Billing Types
			foreach(KeyValuePair<long,Dictionary<long,string>> kvp in dictClinicPlacementMsgs) {
				if(kvp.Value.Count<1) {
					continue;
				}
				List<ProgramProperty> listProps=new List<ProgramProperty>();
				if(!_dictClinicProgProps.TryGetValue(kvp.Key,out listProps) && !_dictClinicProgProps.TryGetValue(0,out listProps)) {
					//should never happen, dictClinicProps should contain all clinicNums the user has access to, including clinicnum 0
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				string sftpAddress=listProps.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
				int sftpPort;
				if(!int.TryParse(listProps.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
					sftpPort=22;//default to port 22
				}
				string userName=listProps.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
				string userPassword=CDT.Class1.TryDecrypt(listProps.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
				byte[] fileContents=Encoding.ASCII.GetBytes(TsiMsgConstructor.GetPlacementFileHeader()+"\r\n"+string.Join("\r\n",kvp.Value.Values));
				try {
					TaskStateUpload state=new Sftp.Upload(sftpAddress,userName,userPassword,sftpPort) {
						Folder="/xfer/incoming",
						FileName="TsiPlacements_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt",
						FileContent=fileContents,
						HasExceptions=true
					};
					state.Execute(false);
				}
				catch(Exception ex) {
					ex.DoNothing();
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				//Upload was successful
				List<TsiTransLog> listLogsForInsert=new List<TsiTransLog>();
				//dictClinicNumListTransLogs should always contain the same clinicNums as dictClinicMsgs, so this should always insert the messages,
				//i.e. TryGetValue never returns false
				if(dictClinicNumListTransLogs.TryGetValue(kvp.Key,out listLogsForInsert)) {
					TsiTransLogs.InsertMany(listLogsForInsert);
				}
				//update all family billing types to the collection bill type
				List<Patient> listAllPats=Patients.GetFamilies(kvp.Value.Keys.ToList()).SelectMany(x => x.ListPats).ToList();
				Patients.UpdateAllFamilyBillingTypes(_collectionBillType.DefNum,kvp.Value.Keys.ToList());
				foreach(Patient pat in listAllPats) {
					if(pat.BillingType==_collectionBillType.DefNum) {
						continue;
					}
					string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+"' to '"+_collectionBillType.ItemName
						+"' due to the account being sent to Transworld from the A/R Manager.";
					SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,pat.PatNum,logTxt);
				}
			}
			#endregion Send Clinic Batch Placement Files, Insert TsiTransLogs, and Update Patient Billing Types
			#region Send Clinic Batch Update Files, Insert TsiTransLogs, and Update Patient Billing Types
			foreach(KeyValuePair<long,Dictionary<long,string>> kvp in dictClinicUpdateMsgs) {
				if(kvp.Value.Count<1) {
					continue;
				}
				List<ProgramProperty> listProps=new List<ProgramProperty>();
				if(!_dictClinicProgProps.TryGetValue(kvp.Key,out listProps) && !_dictClinicProgProps.TryGetValue(0,out listProps)) {
					//should never happen, dictClinicProps should contain all clinicNums the user has access to, including clinicnum 0
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				string sftpAddress=listProps.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
				int sftpPort;
				if(!int.TryParse(listProps.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
					sftpPort=22;//default to port 22
				}
				string userName=listProps.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
				string userPassword=CDT.Class1.TryDecrypt(listProps.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
				byte[] fileContents=Encoding.ASCII.GetBytes(TsiMsgConstructor.GetUpdateFileHeader()+"\r\n"+string.Join("\r\n",kvp.Value.Values));
				try {
					TaskStateUpload state=new Sftp.Upload(sftpAddress,userName,userPassword,sftpPort) {
						Folder="/xfer/incoming",
						FileName="TsiUpdates_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt",
						FileContent=fileContents,
						HasExceptions=true
					};
					state.Execute(false);
				}
				catch(Exception ex) {
					ex.DoNothing();
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				//Upload was successful
				List<TsiTransLog> listLogsForInsert=new List<TsiTransLog>();
				//dictClinicNumListTransLogs should always contain the same clinicNums as dictClinicMsgs, so this should always insert the messages,
				//i.e. TryGetValue never returns false
				if(dictClinicNumListTransLogs.TryGetValue(kvp.Key,out listLogsForInsert)) {
					TsiTransLogs.InsertMany(listLogsForInsert);
				}
				//update all family billing types to the collection bill type
				List<Patient> listAllPats=Patients.GetFamilies(kvp.Value.Keys.ToList()).SelectMany(x => x.ListPats).ToList();
				Patients.UpdateAllFamilyBillingTypes(_collectionBillType.DefNum,kvp.Value.Keys.ToList());
				foreach(Patient pat in listAllPats) {
					if(pat.BillingType==_collectionBillType.DefNum) {
						continue;
					}
					string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+"' to '"+_collectionBillType.ItemName
						+"' due to a status update message being sent to Transworld from the A/R manager.";
					SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,pat.PatNum,logTxt);
				}
			}
			#endregion Send Clinic Batch Update Files, Insert TsiTransLogs, and Update Patient Billing Types
			if(outstandingFound) {
				string message=Lan.g(this,"Would you like to restrict appointment scheduling for the patients with outstanding claims?");
				if(MsgBox.Show(MsgBoxButtons.YesNo,message)) {
					listFamiliesOutstanding.ForEach(x => PatRestrictions.InsertForFam(x,PatRestrict.ApptSchedule));
				}
			}
			#region FillGrids With Updated Info
			RefreshAll();
			FillGrids(false);
			#endregion FillGrids With Updated Info
			SetSelectedRows((listPatNumsToReselect.Union(listFailedPatNums)).ToList(),gridCur);
			Cursor=Cursors.Default;
			if(listFailedPatNums.Count>0) {
				MessageBox.Show(listFailedPatNums.Count+" "+Lan.g(this,"accounts did not upload successfully.  They have not been marked as sent to "
					+"TSI and will have to be resent."));
			}
		}

		#endregion Unsent Tab Methods
		#region Sent Tab Methods

		private void FillGridSent(bool retainSelection=true) {
			Cursor=Cursors.WaitCursor;
			List<long> listSelectedPatNums=new List<long>();
			if(retainSelection) {
				listSelectedPatNums=GetSelectedPatAgings(gridSent).Select(x => x.PatNum).ToList();
			}
			List<int> listPatAgingIndexFiltered=GetPatAgingIndexSentFiltered();
			#region Set Grid Title and Columns
			int vScrollValue=gridSent.VerticalScroll.Value;
			int sortedByColumnIndexOld=gridSent.SortedByColumnIdx;
			bool isAscendingOld=gridSent.SortedIsAscending;
			gridSent.BeginUpdate();
			gridSent.ListGridColumns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ArManagerSentGrid);
			foreach(DisplayField fieldCur in listDisplayFields) {
				if(fieldCur.InternalName=="Clinic" && !PrefC.HasClinicsEnabled) {
					continue;//skip the clinic column if clinics are not enabled
				}
				GridSortingStrategy sortingStrat=GridSortingStrategy.StringCompare;
				HorizontalAlignment hAlign=HorizontalAlignment.Left;
				if(ListTools.In(fieldCur.InternalName,"0-30 Days","31-60 Days","61-90 Days","> 90 Days","Total","-Ins Est","=Patient","PayPlan Due",
					DisplayFields.InternalNames.ArManagerSentGrid.DaysBalBegan)) 
				{
					sortingStrat=GridSortingStrategy.AmountParse;
					hAlign=HorizontalAlignment.Right;
				}
				else if(ListTools.In(fieldCur.InternalName,"Last Paid","Last Proc",DisplayFields.InternalNames.ArManagerSentGrid.DateBalBegan)) {
					sortingStrat=GridSortingStrategy.DateParse;
					hAlign=HorizontalAlignment.Center;
				}
				else if(!ListTools.In(fieldCur.InternalName,"Guarantor","Clinic","Prov","Service Type","Last Transaction")) {
					continue;//shouldn't happen, but the loop to fill the rows will skip any columns that aren't one of these so we'll skip here as well
				}
				gridSent.ListGridColumns.Add(new GridColumn(string.IsNullOrEmpty(fieldCur.Description)?fieldCur.InternalName:fieldCur.Description,
					fieldCur.ColumnWidth,hAlign,sortingStrat));
			}
			//this form initially set to the max allowed (by OD) form size 1246, which is also the minimum size for this form.  If the user resizes the form
			//to be larger, increase each column width by the same ratio to spread out the additional real estate
			int widthColsAndScroll=gridSent.ListGridColumns.Sum(x => x.ColWidth)+20;//+20 for vertical scroll bar
			if(widthColsAndScroll<gridSent.Width) {
				//don't grow/shrink column widths until all columns are visible, i.e no horizontal scroll bar active, all columns fully visible
				gridSent.ListGridColumns.ToList().ForEach(x => x.ColWidth=(int)Math.Round((float)x.ColWidth*gridSent.Width/widthColsAndScroll,MidpointRounding.AwayFromZero));
				//increase the last col width to be the full width of the grid (to account for rounding away from zero above)
				gridSent.ListGridColumns[gridSent.ListGridColumns.Count-1].ColWidth-=gridSent.ListGridColumns.Sum(x => x.ColWidth)+20-gridSent.Width;
			}
			#endregion Set Grid Title and Columns
			#region Fill Grid Rows
			gridSent.ListGridRows.Clear();
			Dictionary<long,string> dictClinicAbbrs=_listClinics.ToDictionary(x => x.ClinicNum,x => x.Abbr);
			Dictionary<long,string> dictProvAbbrs=_listProviders.ToDictionary(x => x.ProvNum,x => x.Abbr);
			double bal0_30=0;
			double bal31_60=0;
			double bal61_90=0;
			double balOver90=0;
			double balTotal=0;
			double insEst=0;
			double amtDue=0;
			double ppDue=0;
			GridRow row;
			List<int> listIndicesToReselect=new List<int>();
			foreach(int i in listPatAgingIndexFiltered) {
				PatAging patAgeCur=_listPatAgingSentAll[i];
				bal0_30+=patAgeCur.Bal_0_30;
				bal31_60+=patAgeCur.Bal_31_60;
				bal61_90+=patAgeCur.Bal_61_90;
				balOver90+=patAgeCur.BalOver90;
				balTotal+=patAgeCur.BalTotal;
				insEst+=patAgeCur.InsEst;
				amtDue+=patAgeCur.AmountDue;
				ppDue+=patAgeCur.PayPlanDue;
				row=new GridRow();
				foreach(DisplayField field in listDisplayFields) {
					switch(field.InternalName) {
						case "Guarantor":
							row.Cells.Add((checkSentShowPatNums.Checked?(patAgeCur.PatNum.ToString()+" - "):"")+patAgeCur.PatName);
							continue;
						case "Clinic":
							if(!PrefC.HasClinicsEnabled) {
								continue;//skip the clinic column if clinics are not enabled
							}
							string clinicAbbr;
							row.Cells.Add(dictClinicAbbrs.TryGetValue(patAgeCur.ClinicNum,out clinicAbbr)?clinicAbbr:"");
							continue;
						case "Prov":
							string provAbbr;
							row.Cells.Add(dictProvAbbrs.TryGetValue(patAgeCur.PriProv,out provAbbr)?provAbbr:"");
							continue;
						case "0-30 Days":
							row.Cells.Add(patAgeCur.Bal_0_30.ToString("n"));
							continue;
						case "31-60 Days":
							row.Cells.Add(patAgeCur.Bal_31_60.ToString("n"));
							continue;
						case "61-90 Days":
							row.Cells.Add(patAgeCur.Bal_61_90.ToString("n"));
							continue;
						case "> 90 Days":
							row.Cells.Add(patAgeCur.BalOver90.ToString("n"));
							continue;
						case "Total":
							row.Cells.Add(patAgeCur.BalTotal.ToString("n"));
							continue;
						case "-Ins Est":
							row.Cells.Add(patAgeCur.InsEst.ToString("n"));
							continue;
						case "=Patient":
							row.Cells.Add(patAgeCur.AmountDue.ToString("n"));
							continue;
						case "PayPlan Due":
							row.Cells.Add(patAgeCur.PayPlanDue.ToString("n"));
							continue;
						case "Last Paid":
							row.Cells.Add(patAgeCur.DateLastPay.Year>1880?patAgeCur.DateLastPay.ToString("d"):"");
							continue;
						case "Service Type":
							TsiTransLog placementlog=patAgeCur.ListTsiLogs.FirstOrDefault(x => x.TransType==TsiTransType.PL);
							row.Cells.Add(placementlog!=null?placementlog.ServiceType.GetDescription():"");
							continue;
						case "Last Transaction":
							string lastTransTypeDate="";
							if(patAgeCur.ListTsiLogs.Count>0) {
								lastTransTypeDate=patAgeCur.ListTsiLogs[0].TransType.GetDescription()+" - "+patAgeCur.ListTsiLogs[0].TransDateTime.ToString("g");
							}
							row.Cells.Add(lastTransTypeDate);
							continue;
						case "Last Proc":
							row.Cells.Add(patAgeCur.DateLastProc.Year>1880?patAgeCur.DateLastProc.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerSentGrid.DateBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?patAgeCur.DateBalBegan.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerSentGrid.DaysBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?(DateTime.Today-patAgeCur.DateBalBegan).Days.ToString():"");
							continue;
						default:
							continue;//skip any columns not defined here, as we did above when adding the columns
					}
				}
				row.Tag=i;
				gridSent.ListGridRows.Add(row);
				if(retainSelection && listSelectedPatNums.Contains(patAgeCur.PatNum)) {
					listIndicesToReselect.Add(gridSent.ListGridRows.Count-1);
				}
			}
			gridSent.EndUpdate();
			gridSent.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridSent.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			#endregion Fill Grid Rows
			groupUpdateAccounts.Enabled=(gridSent.ListGridRows.Count>0);
			if(retainSelection && listIndicesToReselect.Count>0) {
				gridSent.SetSelected(listIndicesToReselect.ToArray(),true);
			}
			SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerSentGrid,gridSent.HScrollValue);
			textSentTotalNumAccts.Text=listPatAgingIndexFiltered.Count.ToString();
			textSent0to30.Text=bal0_30.ToString("n");
			textSent31to60.Text=bal31_60.ToString("n");
			textSent61to90.Text=bal61_90.ToString("n");
			textSentOver90.Text=balOver90.ToString("n");
			textSentTotal.Text=balTotal.ToString("n");
			textSentInsEst.Text=insEst.ToString("n");
			textSentPatient.Text=amtDue.ToString("n");
			textSentPayPlanDue.Text=ppDue.ToString("n");
			Cursor=Cursors.Default;
		}

		private List<int> GetPatAgingIndexSentFiltered() {
			List<int> retval=new List<int>();
			#region Validate Inputs
			if(!textSentMinBal.IsValid() || !textSentDaysLastPay.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors in Sent tab first.");//return empty list, filter inputs cannot be applied since there are errors
				return retval;
			}
			#endregion Validate Inputs
			#region Get Filter Data
			double minBalance=Math.Round(PIn.Double(textSentMinBal.Text),3);
			DateTime dtLastPay=DateTime.Today.AddDays(-PIn.Int(textSentDaysLastPay.Text));
			AgeOfAccount accountAge=new[] { AgeOfAccount.Any,AgeOfAccount.Over30,AgeOfAccount.Over60,AgeOfAccount.Over90 }[comboSentAccountAge.SelectedIndex];
			List<TsiTransType> listTranTypes=new List<TsiTransType>();
			if(!comboLastTransType.IsAllSelected) {
				listTranTypes=comboLastTransType.GetListSelected<TsiTransType>();
			}
			List<long> listProvNums=new List<long>();
			if(!comboProvsSent.IsAllSelected) {
				listProvNums=comboProvsSent.GetSelectedProvNums();
			}
			#endregion Get Filter Data
			#region Apply Filter Data to PatAging List
			for(int i=0;i<_listPatAgingSentAll.Count;i++) {
				PatAging pAge=_listPatAgingSentAll[i];
				if(Math.Round(pAge.AmountDue,3) >= minBalance
					&& (dtLastPay.Date>=DateTime.Today.Date || pAge.DateLastPay.Date<dtLastPay.Date)
					&& (listTranTypes.Count==0 || pAge.ListTsiLogs.Count<1 || listTranTypes.Contains(pAge.ListTsiLogs[0].TransType))
					&& (listProvNums.Count==0 || listProvNums.Contains(pAge.PriProv))
					&& ( ((int)accountAge < 4 && pAge.BalOver90 > 0.005)//if Any, Over30, Over60 or Over90 are selected, check BalOver90
						|| ((int)accountAge < 3 && pAge.Bal_61_90 > 0.005)//if Any, Over30 or Over60 are selected, check Bal_61_90
						|| ((int)accountAge < 2 && pAge.Bal_31_60 > 0.005)//if Any or Over30 are selected, check Bal_31_60
						|| (int)accountAge < 1 ))//or if Any bal is selected
				{
					retval.Add(i);
				}
			}
			#endregion Apply Filter Data to PatAging List
			return retval;
		}

		private void gridSentMain_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right){
				gridSent.SetAll(false);
				menuItemMarkExcluded.Visible=false;
			}
			else {
				_listSelectedPatNums=GetSelectedPatAgings(gridSent).Select(x => x.PatNum).ToList();
			}
		}

		private void gridSentMain_HorizScrolled(object sender,ScrollEventArgs e) {
			SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerSentGrid,e.NewValue);
		}

		private void gridSentMain_ColumnSorted(object sender,EventArgs e) {
			SetSelectedRows(_listSelectedPatNums,gridSent);
		}

		private void comboBoxMultiSentProvs_Leave(object sender,EventArgs e) {
			if(comboProvsSent.SelectedIndices.Count==0) {
				comboProvsSent.IsAllSelected=true;
			}
		}

		private void comboBoxMultiLastTransType_Leave(object sender,EventArgs e) {
			if(comboLastTransType.SelectedIndices.Count==0) {
				comboLastTransType.IsAllSelected=true;
			}
		}

		private void comboBoxMultiSentProvs_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridSent();
		}

		private void comboBoxMultiLastTransType_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridSent();
		}

		private void comboSentAccountAge_SelectedIndexChanged(object sender,EventArgs e) {
			FillGridSent();
		}

		private void textSentMinBal_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void textSentDaysLastPay_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void comboNewStatus_SelectedIndexChanged(object sender,EventArgs e) {
			if(comboNewStatus.SelectedIndex<0 || comboNewStatus.SelectedIndex>_listNewStatuses.Count-1) {
				return;
			}
			if(comboNewBillType.SelectedIndex<0 || comboNewBillType.SelectedIndex>_listBillTypesNoColl.Count-1) {
				errorProvider1.SetError(comboNewBillType,Lan.g(this,"Select billing type for accounts that will no longer be managed by Transworld."));
			}
		}

		private void comboNewBillType_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboNewBillType.SelectedIndex<0 || comboNewBillType.SelectedIndex>_listBillTypesNoColl.Count-1) {
				errorProvider1.SetError(comboNewBillType,Lan.g(this,"Select billing type for accounts that will no longer be managed by Transworld."));
				return;
			}
			errorProvider1.SetError(comboNewBillType,"");
		}

		private void checkSentShowPatNums_CheckedChanged(object sender,EventArgs e) {
			FillGridSent();
		}

		private void butSentSaveDefaults_Click(object sender,EventArgs e) {
			SaveDefaults();
		}

		private void butSentAll_Click(object sender,EventArgs e) {
			gridSent.SetAll(true);
		}

		private void butSentNone_Click(object sender,EventArgs e) {
			gridSent.SetAll(false);
		}

		private void butSentPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"TSI collections sent printed"), PrintoutOrientation.Landscape);
		}

		private void butExcludedPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"TSI collections Excluded printed"), PrintoutOrientation.Landscape);
		}

		private void butUpdateStatus_Click(object sender,EventArgs e) {
			#region Get and Validate Data
			if(!Security.IsAuthorized(Permissions.Billing)) {
				return;
			}
			if(comboNewStatus.SelectedIndex<0 || comboNewStatus.SelectedIndex>=_listNewStatuses.Count) {
				MsgBox.Show(this,"Please select a new status first.");
				return;
			}
			if(gridSent.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Please select accounts to update first.");
				return;
			}
			List<PatAging> listPatAging=GetSelectedPatAgings(gridSent);
			List<long> listClinicsSkipped;
			if(!ValidateSendUpdateData(listPatAging.Select(x => x.ClinicNum).ToList(),out listClinicsSkipped)) {
				return;
			}
			if(_dictClinicProgProps.All(x => listClinicsSkipped.Contains(x.Key))) {
				MsgBox.Show(this,"An SFTP connection could not be made using the connection details "+(PrefC.HasClinicsEnabled ? "for any clinic " : "")
					+"in the enabled Transworld (TSI) program link.  Account statuses cannot be updated with TSI until the program link is setup.");
				return;
			}
			TsiTransType transType=_listNewStatuses[comboNewStatus.SelectedIndex];
			if(transType==TsiTransType.SS) {
				string msgTxt=Lan.g(this,"The account(s) will be suspended for a maximum of 50 days but only if they are in the Accelerator or Profit Recovery "
					+"stage.  Accounts in the Transworld Systems Collection stage will NOT be suspended and will have to be reinstated from the unsent grid.  "
					+"During the 50 day suspension you may reinstate the account(s) at any time.  However, after 50 days has passed, the account(s) will expire "
					+"and will no longer be available to reinstate.")+"\r\n\r\n"+Lan.g(this,"Do you want to suspend the service for the selected account(s)?");
				if(MessageBox.Show(msgTxt,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
					return;
				}
			}
			if(comboNewBillType.SelectedIndex<0 || comboNewBillType.SelectedIndex>=_listBillTypesNoColl.Count) {
				MsgBox.Show(this,"Please select a new billing type to assign to the guarantors that are no longer going to be managed by Transworld.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			Def newBillType=_listBillTypesNoColl[comboNewBillType.SelectedIndex];
			#endregion Get and Validate Data
			#region Create Messages and TsiTransLogs
			//TSI connection details validated, at least one clinic the user has access to is setup with valid connection details
			Dictionary<long,Dictionary<long,string>> dictClinicMsgs=new Dictionary<long,Dictionary<long,string>>();
			Dictionary<long,List<TsiTransLog>> dictClinicNumListTransLogs=new Dictionary<long,List<TsiTransLog>>();
			List<long> listFailedPatNums=new List<long>();
			foreach(PatAging pAgingCur in listPatAging) {
				long clinicNum=PrefC.HasClinicsEnabled?pAgingCur.ClinicNum:0;
				if(listClinicsSkipped.Contains(clinicNum)) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					continue;
				}
				List<ProgramProperty> listProgProps;
				if(!_dictClinicProgProps.TryGetValue(clinicNum,out listProgProps) && !_dictClinicProgProps.TryGetValue(0,out listProgProps)) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					listClinicsSkipped.Add(clinicNum);
					continue;
				}
				string clientId="";
				if(pAgingCur.ListTsiLogs.Count>0) {
					clientId=pAgingCur.ListTsiLogs[0].ClientId;
				}
				if(string.IsNullOrEmpty(clientId)) {
					clientId=listProgProps.Find(x => x.PropertyDesc=="ClientIdAccelerator")?.PropertyValue;
				}
				if(string.IsNullOrEmpty(clientId)) {
					clientId=listProgProps.Find(x => x.PropertyDesc=="ClientIdCollection")?.PropertyValue;
				}
				if(string.IsNullOrEmpty(clientId)) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					listClinicsSkipped.Add(clinicNum);
					continue;
				}
				try {
					string msg=TsiMsgConstructor.GenerateUpdate(pAgingCur.PatNum,clientId,transType,0.00,pAgingCur.AmountDue);
					if(!dictClinicMsgs.ContainsKey(clinicNum)) {
						dictClinicMsgs[clinicNum]=new Dictionary<long,string>();
					}
					dictClinicMsgs[clinicNum].Add(pAgingCur.PatNum,msg);
					if(!dictClinicNumListTransLogs.ContainsKey(clinicNum)) {
						dictClinicNumListTransLogs[clinicNum]=new List<TsiTransLog>();
					}
					dictClinicNumListTransLogs[clinicNum].Add(new TsiTransLog() {
						PatNum=pAgingCur.PatNum,
						UserNum=Security.CurUser.UserNum,
						TransType=transType,
						//TransDateTime=DateTime.Now,//set on insert, not editable by user
						//ServiceType=TsiServiceType.Accelerator,//only valid for placement msgs
						//ServiceCode=TsiServiceCode.Diplomatic,//only valid for placement msgs
						ClientId=clientId,
						TransAmt=0.00,
						AccountBalance=pAgingCur.AmountDue,
						FKeyType=TsiFKeyType.None,//only used for account trans updates
						FKey=0,//only used for account trans updates
						RawMsgText=msg,
						ClinicNum=clinicNum
						//,TransJson=""//only valid for placement msgs
					});
				}
				catch(ApplicationException ex) {
					listFailedPatNums.Add(pAgingCur.PatNum);
					if(dictClinicMsgs.ContainsKey(clinicNum)) {
						dictClinicMsgs[clinicNum].Remove(pAgingCur.PatNum);
					}
					if(dictClinicNumListTransLogs.ContainsKey(clinicNum)) {
						dictClinicNumListTransLogs[clinicNum].RemoveAll(x => x.PatNum==pAgingCur.PatNum);
					}
					Cursor=Cursors.Default;
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,ex.Message+"\r\nDo you want to continue attempting to send any remaining accounts?")) {
						Cursor=Cursors.WaitCursor;
						continue;
					}
					else {
						break;
					}
				}
			}
			#endregion Create Messages and TsiTransLogs
			#region Send Clinic Batch Files, Insert TsiTransLogs, and Update Patient Billing Types
			foreach(KeyValuePair<long,Dictionary<long,string>> kvp in dictClinicMsgs) {
				if(kvp.Value.Count<1) {
					continue;
				}
				List<ProgramProperty> listProps=new List<ProgramProperty>();
				if(!_dictClinicProgProps.TryGetValue(kvp.Key,out listProps) && !_dictClinicProgProps.TryGetValue(0,out listProps)) {
					//should never happen, dictClinicProps should contain all clinicNums the user has access to, including clinicnum 0
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				string sftpAddress=listProps.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
				int sftpPort;
				if(!int.TryParse(listProps.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
					sftpPort=22;//default to port 22
				}
				string userName=listProps.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
				string userPassword=CDT.Class1.TryDecrypt(listProps.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
				byte[] fileContents=Encoding.ASCII.GetBytes(TsiMsgConstructor.GetUpdateFileHeader()+"\r\n"+string.Join("\r\n",kvp.Value.Values));
				try {
					TaskStateUpload state=new Sftp.Upload(sftpAddress,userName,userPassword,sftpPort) {
						Folder="/xfer/incoming",
						FileName="TsiUpdates_"+DateTime.Now.ToString("yyyyMMddhhmmss")+".txt",
						FileContent=fileContents,
						HasExceptions=true
					};
					state.Execute(false);
				}
				catch(Exception ex) {
					ex.DoNothing();
					listFailedPatNums.AddRange(kvp.Value.Keys);
					continue;
				}
				//Upload was successful
				List<TsiTransLog> listLogsForInsert=new List<TsiTransLog>();
				//dictClinicNumListTransLogs should always contain the same clinicNums as dictClinicMsgs, so this should always insert the messages,
				//i.e. TryGetValue never returns false
				if(dictClinicNumListTransLogs.TryGetValue(kvp.Key,out listLogsForInsert)) {
					TsiTransLogs.InsertMany(listLogsForInsert);
				}
				//update all family billing types to the collection bill type if transtype is reinstated, otherwise to the selected new billing type
				List<Patient> listAllPats=Patients.GetFamilies(kvp.Value.Keys.ToList()).SelectMany(x => x.ListPats).ToList();
				Def billTypeUpdate=transType==TsiTransType.RI?_collectionBillType:newBillType;
				Patients.UpdateAllFamilyBillingTypes(billTypeUpdate.DefNum,kvp.Value.Keys.ToList());
				foreach(Patient pat in listAllPats) {
					if(pat.BillingType==billTypeUpdate.DefNum) {
						continue;
					}
					string logTxt="Patient billing type changed from '"+Defs.GetName(DefCat.BillingTypes,pat.BillingType)+"' to '"+billTypeUpdate.ItemName
						+"' due to a status update message being sent to Transworld from the A/R manager.";
					SecurityLogs.MakeLogEntry(Permissions.PatientBillingEdit,pat.PatNum,logTxt);
				}
			}
			#endregion Send Clinic Batch Files, Insert TsiTransLogs, and Update Patient Billing Types
			RefreshAll();
			FillGrids(false);
			Cursor=Cursors.Default;
			if(listFailedPatNums.Count>0) {
				MessageBox.Show(listFailedPatNums.Count+" "+Lan.g(this,"accounts did not upload successfully.  They have not been marked as sent to "
					+"collection and will have to be resent."));
				SetSelectedRows(listFailedPatNums,gridSent);
			}
		}

		#endregion Sent Tab Methods
		#region Excluded Tab Methods
		private void FillGridExcluded(bool retainSelection=true) {
			Cursor=Cursors.WaitCursor;
			List<long> listSelectedPatNums=new List<long>();
			if(retainSelection) {
				listSelectedPatNums=GetSelectedPatAgings(gridExcluded).Select(x => x.PatNum).ToList();
			}
			List<int> listPatAgingIndexFiltered=GetPatAgingIndexExcludedFiltered();
			#region Set Grid Title and Columns
			int vScrollValue=gridExcluded.VerticalScroll.Value;
			int sortedByColumnIndexOld=gridExcluded.SortedByColumnIdx;
			bool isAscendingOld=gridExcluded.SortedIsAscending;
			gridExcluded.BeginUpdate();
			gridExcluded.ListGridColumns.Clear();
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.ArManagerExcludedGrid);
			foreach(DisplayField fieldCur in listDisplayFields) {
				if(fieldCur.InternalName=="Clinic" && !PrefC.HasClinicsEnabled) {
					continue;//skip the clinic column if clinics are not enabled
				}
				GridSortingStrategy sortingStrat=GridSortingStrategy.StringCompare;
				HorizontalAlignment hAlign=HorizontalAlignment.Left;
				if(ListTools.In(fieldCur.InternalName,"0-30 Days","31-60 Days","61-90 Days","> 90 Days","Total","-Ins Est","=Patient","PayPlan Due",
					DisplayFields.InternalNames.ArManagerExcludedGrid.DaysBalBegan)) 
				{
					sortingStrat=GridSortingStrategy.AmountParse;
					hAlign=HorizontalAlignment.Right;
				}
				else if(ListTools.In(fieldCur.InternalName,"Last Paid","Last Proc","DateTime Suspended",DisplayFields.InternalNames.ArManagerExcludedGrid.DateBalBegan)) {
					sortingStrat=GridSortingStrategy.DateParse;
					hAlign=HorizontalAlignment.Center;
				}
				else if(!ListTools.In(fieldCur.InternalName,"Guarantor","Clinic","Prov","Billing Type")) {
					continue;//shouldn't happen, but the loop to fill the rows will skip any columns that aren't one of these so we'll skip here as well
				}
				gridExcluded.ListGridColumns.Add(new GridColumn(string.IsNullOrEmpty(fieldCur.Description)?fieldCur.InternalName:fieldCur.Description,
					fieldCur.ColumnWidth,hAlign,sortingStrat));
			}
			//this form initially set to the max allowed (by OD) form size 1246, which is also the minimum size for this form.  If the user resizes the form
			//to be larger, increase each column width by the same ratio to spread out the additional real estate
			int widthColsAndScroll=gridExcluded.ListGridColumns.Sum(x => x.ColWidth)+20;//+20 for vertical scroll bar
			//widthColsAndScroll is width of columns as set in the display fields, i.e. haven't grown or shrunk due to form resizing so won't be shrunk to
			//less than the sizes set in display fields, only grown from there.  Thus the display field sizes are basically a minimum size.
			if(widthColsAndScroll<gridExcluded.Width) {
				gridExcluded.ListGridColumns.ToList().ForEach(x => x.ColWidth=(int)Math.Round((float)x.ColWidth*gridExcluded.Width/widthColsAndScroll,MidpointRounding.AwayFromZero));
				//adjust the last col width to take any remaining pixels so that the cols take the full width of the grid (to account for rounding above)
				gridExcluded.ListGridColumns[gridExcluded.ListGridColumns.Count-1].ColWidth-=gridExcluded.ListGridColumns.Sum(x => x.ColWidth)+20-gridExcluded.Width;
			}
			#endregion Set Grid Title and Columns
			#region Fill Grid Rows
			gridExcluded.ListGridRows.Clear();
			Dictionary<long,string> dictClinicAbbrs=_listClinics.ToDictionary(x => x.ClinicNum,x => x.Abbr);
			Dictionary<long,string> dictProvAbbrs=_listProviders.ToDictionary(x => x.ProvNum,x => x.Abbr);
			Dictionary<long,string> dictBillTypeNames=Defs.GetDefsForCategory(DefCat.BillingTypes).ToDictionary(x => x.DefNum,x => x.ItemName);
			Dictionary<long,DateTime> dictSuspendDateTimes=new Dictionary<long,DateTime>();
			foreach(PatAging pAgeCur in listPatAgingIndexFiltered.Select(x => _listPatAgingExcludedAll[x])) {
				TsiTransLog tsiLogMostRecentStatusChange=pAgeCur.ListTsiLogs
					.Find(x => ListTools.In(x.TransType,TsiTransType.CN,TsiTransType.PF,TsiTransType.PL,TsiTransType.PT,TsiTransType.RI,TsiTransType.SS));
				if(tsiLogMostRecentStatusChange!=null && tsiLogMostRecentStatusChange.TransType==TsiTransType.SS) {
					dictSuspendDateTimes.Add(pAgeCur.PatNum,tsiLogMostRecentStatusChange.TransDateTime);
				}
			}
			double bal0_30=0;
			double bal31_60=0;
			double bal61_90=0;
			double balOver90=0;
			double balTotal=0;
			double insEst=0;
			double amtDue=0;
			double ppDue=0;
			GridRow row;
			List<int> listIndicesToReselect=new List<int>();
			foreach(int i in listPatAgingIndexFiltered) {
				PatAging patAgeCur=_listPatAgingExcludedAll[i];
				bal0_30+=patAgeCur.Bal_0_30;
				bal31_60+=patAgeCur.Bal_31_60;
				bal61_90+=patAgeCur.Bal_61_90;
				balOver90+=patAgeCur.BalOver90;
				balTotal+=patAgeCur.BalTotal;
				insEst+=patAgeCur.InsEst;
				amtDue+=patAgeCur.AmountDue;
				ppDue+=patAgeCur.PayPlanDue;
				row=new GridRow();
				foreach(DisplayField field in listDisplayFields) {
					switch(field.InternalName) {
						case "Guarantor":
							row.Cells.Add((checkExcludedShowPatNums.Checked?(patAgeCur.PatNum.ToString()+" - "):"")+patAgeCur.PatName);
							continue;
						case "Clinic":
							if(!PrefC.HasClinicsEnabled) {
								continue;//skip the clinic column if clinics are not enabled
							}
							string clinicAbbr;
							row.Cells.Add(dictClinicAbbrs.TryGetValue(patAgeCur.ClinicNum,out clinicAbbr)?clinicAbbr:"");
							continue;
						case "Prov":
							string provAbbr;
							row.Cells.Add(dictProvAbbrs.TryGetValue(patAgeCur.PriProv,out provAbbr)?provAbbr:"");
							continue;
						case "Billing Type":
							string billTypeName;
							row.Cells.Add(dictBillTypeNames.TryGetValue(patAgeCur.BillingType,out billTypeName)?billTypeName:"");
							continue;
						case "0-30 Days":
							row.Cells.Add(patAgeCur.Bal_0_30.ToString("n"));
							continue;
						case "31-60 Days":
							row.Cells.Add(patAgeCur.Bal_31_60.ToString("n"));
							continue;
						case "61-90 Days":
							row.Cells.Add(patAgeCur.Bal_61_90.ToString("n"));
							continue;
						case "> 90 Days":
							row.Cells.Add(patAgeCur.BalOver90.ToString("n"));
							continue;
						case "Total":
							row.Cells.Add(patAgeCur.BalTotal.ToString("n"));
							continue;
						case "-Ins Est":
							row.Cells.Add(patAgeCur.InsEst.ToString("n"));
							continue;
						case "=Patient":
							row.Cells.Add(patAgeCur.AmountDue.ToString("n"));
							continue;
						case "PayPlan Due":
							row.Cells.Add(patAgeCur.PayPlanDue.ToString("n"));
							continue;
						case "Last Paid":
							row.Cells.Add(patAgeCur.DateLastPay.Year>1880?patAgeCur.DateLastPay.ToString("d"):"");
							continue;
						case "DateTime Suspended":
							TsiTransLog suspendLog=patAgeCur.ListTsiLogs.Find(x => x.TransType==TsiTransType.SS);
							row.Cells.Add(suspendLog!=null?suspendLog.TransDateTime.ToString():"");
							continue;
						case "Last Proc":
							row.Cells.Add(patAgeCur.DateLastProc.Year>1880?patAgeCur.DateLastProc.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerExcludedGrid.DateBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?patAgeCur.DateBalBegan.ToString("d"):"");
							continue;
						case DisplayFields.InternalNames.ArManagerExcludedGrid.DaysBalBegan:
							row.Cells.Add(patAgeCur.DateBalBegan.Year>1880?(DateTime.Today-patAgeCur.DateBalBegan).Days.ToString():"");
							continue;
						default:
							continue;//skip any columns not defined here, as we did above when adding the columns
					}
				}
				if(patAgeCur.Birthdate.Year<1880//invalid bday
					|| patAgeCur.Birthdate>DateTime.Today.AddYears(-18)//under 18 years old
					|| new[] { patAgeCur.Address,patAgeCur.City,patAgeCur.State,patAgeCur.Zip }.Any(x => string.IsNullOrWhiteSpace(x)))//missing address information
				{
					//color row light red/pink, using cell color so selecting row still shows color
					row.Cells.OfType<GridCell>().ToList().ForEach(x => x.ColorBackG=Color.FromArgb(255,255,230,234));
				}
				row.Tag=i;//tag the row with the index in the class-wide list of all excluded PatAgings
				gridExcluded.ListGridRows.Add(row);
				if(retainSelection && listSelectedPatNums.Contains(patAgeCur.PatNum)) {
					listIndicesToReselect.Add(gridExcluded.ListGridRows.Count-1);
				}
			}
			gridExcluded.EndUpdate();
			gridExcluded.VerticalScroll.Value=vScrollValue;
			if(sortedByColumnIndexOld>-1) {//If the grid was being sorted by a column prior to BeginUpdate() then re-apply the sorting
				gridExcluded.SortForced(sortedByColumnIndexOld,isAscendingOld);
			}
			#endregion Fill Grid Rows
			groupExcludedPlaceAccounts.Enabled=(gridExcluded.ListGridRows.Count>0);
			if(retainSelection && listIndicesToReselect.Count>0) {
				gridExcluded.SetSelected(listIndicesToReselect.ToArray(),true);
			}
			SetTotalsLocAndVisible(DisplayFieldCategory.ArManagerExcludedGrid,gridExcluded.HScrollValue);
			textExcludedTotalNumAccts.Text=listPatAgingIndexFiltered.Count.ToString();
			textExcluded0to30.Text=bal0_30.ToString("n");
			textExcluded31to60.Text=bal31_60.ToString("n");
			textExcluded61to90.Text=bal61_90.ToString("n");
			textExcludedOver90.Text=balOver90.ToString("n");
			textExcludedTotal.Text=balTotal.ToString("n");
			textExcludedInsEst.Text=insEst.ToString("n");
			textExcludedPatient.Text=amtDue.ToString("n");
			textExcludedPayPlanDue.Text=ppDue.ToString("n");
			Cursor=Cursors.Default;
		}

		private List<int> GetPatAgingIndexExcludedFiltered() {
			List<int> retval=new List<int>();
			#region Validate Inputs
			if(!textExcludedMinBal.IsValid() || !textExcludedDaysLastPay.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors in Excluded tab first.");
				return retval;//return empty list, filter inputs cannot be applied since there are errors
			}
			#endregion Validate Inputs
			#region Get Filter Data
			double minBalance=Math.Round(PIn.Double(textExcludedMinBal.Text),3);
			DateTime dtLastPay=DateTime.Today.AddDays(-PIn.Int(textExcludedDaysLastPay.Text));
			AgeOfAccount accountAge=new[] { AgeOfAccount.Any,AgeOfAccount.Over30,AgeOfAccount.Over60,AgeOfAccount.Over90 }[comboExcludedAccountAge.SelectedIndex];
			List<long> listProvNums=new List<long>();
			if(!comboProvsExcluded.IsAllSelected) {
				listProvNums=comboProvsExcluded.GetSelectedProvNums();
			}
			#endregion Get Filter Data
			#region Apply Filter Data to PatAging List
			for(int i=0;i<_listPatAgingExcludedAll.Count;i++) {
				PatAging pAge=_listPatAgingExcludedAll[i];
				if(Math.Round(pAge.AmountDue,3) >= minBalance
					&& (dtLastPay.Date>=DateTime.Today.Date || pAge.DateLastPay.Date<dtLastPay.Date)
					&& (listProvNums.Count==0 || listProvNums.Contains(pAge.PriProv))
					&& (!checkExcludedExcludeBadAddress.Checked || new[] { pAge.Address,pAge.City,pAge.State,pAge.Zip }.All(y => !string.IsNullOrWhiteSpace(y)))
					&& (!checkExcludedExcludeIfProcs.Checked || !pAge.HasUnsentProcs)
					&& (!checkExcludedExcludeInsPending.Checked || !pAge.HasInsPending)
					&& ( ((int)accountAge < 4 && pAge.BalOver90 > 0.005)//if Any, Over30, Over60 or Over90 are selected, check BalOver90
						|| ((int)accountAge < 3 && pAge.Bal_61_90 > 0.005)//if Any, Over30 or Over60 are selected, check Bal_61_90
						|| ((int)accountAge < 2 && pAge.Bal_31_60 > 0.005)//if Any or Over30 are selected, check Bal_31_60
						|| (int)accountAge < 1 ))//or if Any bal is selected
				{
					retval.Add(i);
				}
			}
			#endregion Apply Filter Data to PatAging List
			return retval;
		}

		private void FillExcludedServiceTypes() {
			TsiServiceType selectedType=comboExcludedServiceType.GetSelected<TsiServiceType>();//If they have nothing selected, this will default to 'Accelerator'.
			comboExcludedServiceType.Items.Clear();
			Dictionary<long,string[]> dictClinicSelectedServices=_dictClinicProgProps
				.Where(x => x.Value.Any(y => y.PropertyDesc=="SelectedServices" && !string.IsNullOrEmpty(y.PropertyValue)))
				.ToDictionary(x => x.Key,x => x.Value.Find(y => y.PropertyDesc=="SelectedServices").PropertyValue.Split(','));
			if(dictClinicSelectedServices.TryGetValue(_selectedClinicNum,out string[] arrayVals)) {
				if(arrayVals.Contains(((int)TsiServiceType.Accelerator).ToString())) {
					comboExcludedServiceType.Items.Add(TsiServiceType.Accelerator.GetDescription(),TsiServiceType.Accelerator);
				}
				if(arrayVals.Contains(((int)TsiServiceType.ProfitRecovery).ToString())) {
					comboExcludedServiceType.Items.Add(TsiServiceType.ProfitRecovery.GetDescription(),TsiServiceType.ProfitRecovery);
				}
				if(arrayVals.Contains(((int)TsiServiceType.ProfessionalCollections).ToString())) {
					comboExcludedServiceType.Items.Add(TsiServiceType.ProfessionalCollections.GetDescription(),TsiServiceType.ProfessionalCollections);
				}
			}
			if(comboExcludedServiceType.Items.Count>0) {
				comboExcludedServiceType.SetSelectedEnum(selectedType);
			}
		}

		private void comboBoxMultiExcludedProvs_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void comboBoxMultiExcludedBillTypes_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void comboExcludedAccountAge_SelectedIndexChanged(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void textExcludedMinBal_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void textExcludedDaysLastPay_TextChanged(object sender,EventArgs e) {
			timerFillGrid.Enabled=false;
			timerFillGrid.Enabled=true;
		}

		private void checkExcludeExcludedInsPending_CheckedChanged(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void checkExcludeExcludedIfProcs_CheckedChanged(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void checkExcludeExcludedBadAddress_CheckedChanged(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void checkExcludedShowPatNums_CheckedChanged(object sender,EventArgs e) {
			FillGridExcluded();
		}

		private void butExcludedSaveDefault_Click(object sender,EventArgs e) {
			SaveDefaults();
		}

		private void butExcludedNone_Click(object sender,EventArgs e) {
			gridExcluded.SetAll(false);
		}

		private void butExcludedAll_Click(object sender,EventArgs e) {
			gridExcluded.SetAll(true);
		}

		private void butExcludedRunAging_Click(object sender,EventArgs e) {
			RunAgingIfNecessary(isAutomatic:false);
			RefreshAll();
			FillGrids();
		}

		private void gridExcluded_MouseDown(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right){
				gridExcluded.SetAll(false);
				menuItemMarkExcluded.Visible=true;
				menuItemMarkExcluded.Text="Mark Unsent";
			}
			else {
				_listSelectedPatNums=GetSelectedPatAgings(gridExcluded).Select(x => x.PatNum).ToList();
			}
		}
		#endregion Excluded Tab Methods

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormArManager_FormClosing(object sender,FormClosingEventArgs e) {
			timerFillGrid?.Dispose();
			_toolTipUnsentErrors?.Dispose();
			_listPatAgingSentAll?.Clear();
			_listPatAgingSentAll=null;
			_listPatAgingUnsentAll?.Clear();
			_listPatAgingUnsentAll=null;
		}

		private void butUnsentExport_Click(object sender,EventArgs e) {
			gridUnsent.Export("TSI Unsent Accounts");
		}

		private void butSentExport_Click(object sender,EventArgs e) {
			gridSent.Export("TSI Sent Accounts");
		}

		private void butExcludedExport_Click(object sender,EventArgs e) {
			gridExcluded.Export("TSI Excluded Accounts");
		}
	}
}