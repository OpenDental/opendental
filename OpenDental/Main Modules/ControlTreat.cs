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
	public partial class ControlTreat : System.Windows.Forms.UserControl{
		#region Fields - Public
		public bool HasNoteChanged=false;
		public LayoutManagerForms LayoutManager=new LayoutManagerForms();
		public Patient PatientCur;
		#endregion Fields - Public

		#region Fields - Private
		private ArrayList _arrayListPreAuth;
		private Bitmap _bitmapToothChart;
		private Family _family;
		private System.Drawing.Font _fontBody=new System.Drawing.Font("Arial",9);
		private System.Drawing.Font _fontName=new System.Drawing.Font("Arial",9,FontStyle.Bold);
		private System.Drawing.Font _fontSubHeading=new System.Drawing.Font("Arial",10,FontStyle.Bold);
		private System.Drawing.Font _fontTotal=new System.Drawing.Font("Arial",9,FontStyle.Bold);
		private List<InsPlan> _listInsPlans;
		private List<InsSub> _listInsSubs;
		private List<SubstitutionLink> _listSubstitutionLinks=null;
		private List<TreatPlan> _listTreatPlans;
		private List<PatPlan> _listPatPlans;
		private List<Benefit> _listBenefits;
		private List<Procedure> _listProceduresFiltered;
		///<summary>Only used for printing graphical chart.</summary>
		private List<ToothInitial> _listToothInitials;
		///<summary>List of ClaimProcs with status of Estimate or CapEstimate for the patient.</summary>
		private List<ClaimProc> _listClaimProcs;
		///<summary>This is a list of all procedures for the patient.</summary>
		private List<Procedure> _listProcedures;
		///<summary>This is a filtered list containing only TP procedures.  It's also already sorted by priority and tooth number.</summary>
		private List<Procedure> _listProceduresTP;
		///<summary>A list of all ProcTP objects for this patient.</summary>
		private List<ProcTP> _listProcTPs;
		///<summary>A list of all ProcTP objects for the selected tp.</summary>
		private List<ProcTP> _listProcTPsSelect;
		///<summary>Only used for printing graphical chart prior to v17.  Null unless prepping for printing.</summary>
		private ToothChartWrapper _toothChartWrapper;
		///<summary>Only used for printing graphical chart prior to v17.  Relays commands to either the old SparksToothChart.ToothChartWrapper or the new Sparks3d.ToothChart.</summary>
		private ToothChartRelay _toothChartRelay;
		private List<Claim> _listClaims;
		private bool _initializedOnStartup;
		private List<ClaimProcHist> _listClaimProcHists;
		//private List<ClaimProcHist> _listClaimProcHistsLoops;
		private bool _checkShowInsNotAutomatic;
		private bool _checkShowDiscountNotAutomatic;
		private List<TpRow> _listTpRowsMain;
		///<summary>Gets updated to PatientCur.PatNum that the last security log was made with so that we don't make too many security logs for this patient.  When _patNumLast no longer matches PatCur.PatNum (e.g. switched to a different patient within a module), a security log will be entered.  Gets reset (cleared and the set back to PatCur.PatNum) any time a module button is clicked which will cause another security log to be entered.</summary>
		private long _patNumLast;
		private TPModuleData _tpModuleData;
		///<summary>Tracks the most recently selected TreatPlan in gridPlans.</summary>
		private TreatPlan _treatPlanSelected;
		private bool _hasCheckShowCompletedChangedByUser;
		///<summary>Set to true when TP Note changes.  Public so this can be checked from FormOpenDental and the note can be saved.  Necessary because in
		///some cases the leave event doesn't fire, like when a non-modal form is selected, like big phones, and the selected patient is changed from that form.</summary>
		///<summary>The data needed to load the active treatment plan. Also used for inactive treatment plans.</summary>
		private LoadActiveTPData _loadActiveTPData;
		#endregion Fields - Private

		#region Constructor
		///<summary></summary>
		public ControlTreat(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();// This call is required by the Windows.Forms Form Designer.
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		///<summary>Only called on startup, but after local data loaded from db.</summary>
		public void InitializeOnStartup() {
			if(_initializedOnStartup) {
				return;
			}
			_initializedOnStartup=true;
			checkShowCompleted.Checked=PrefC.GetBool(PrefName.TreatPlanShowCompleted);
			if(PrefC.RandomKeys) {//random PKs don't get the option or sorting by order entered
				tabControlShowSort.TabPages.Remove(tabPageSort);
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
				labelCheckInsFrequency,
				butSendToDevice,
				butInsRem,
				butPlannedAppt,
				butRefresh,
				butSaveTP,
				butNewTP,
				tabControlShowSort,
				label6,//label6 is the 'Order procedures by priority, then date, then' text.
				radioTreatPlanSortTooth,
				radioTreatPlanSortOrder,
				checkPrintClassic,
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
			tabControlShowSort.TabPages.Remove(tabPagePrint);//We may add this back in gridPlans_CellClick.
			if(Programs.UsingEcwTightOrFullMode()) {
				butPlannedAppt.Visible=false;
			}
		}

		///<summary>Called every time local data is changed from any workstation.  Refreshes priority lists and lays out the toolbar.</summary>
		public void InitializeLocalData() {
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			Def def=new Def();
			def.DefNum=0;
			def.ItemName=Lan.g(this,"no priority");
			listDefs.Insert(0,def);
			listSetPr.Items.Clear();
			for(int i=0;i<listDefs.Count;i++) {
				listSetPr.Items.Add(listDefs[i].ItemName,listDefs[i]);
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
			Plugins.HookAddCode(this,"ContrTreat.LayoutToolBar_end",PatientCur);
			UpdateToolbarButtons();
		}

		///<summary></summary>
		public void ModuleSelected(long patNum,bool hasDefaultDate=false) {
			if(hasDefaultDate) {
				dateTimeTP.Value=DateTime.Today;
			}
			RefreshModuleData(patNum);
			if(PatientCur!=null && PatientCur.PatStatus==PatientStatus.Deleted) {
				MsgBox.Show("Selected patient has been deleted by another workstation.");
				PatientL.RemoveFromMenu(PatientCur.PatNum);
				FormOpenDental.S_Contr_PatientSelected(new Patient(),false);
				RefreshModuleData(0);
			}
			RefreshModuleScreen(false);
			PatientDashboardDataEvent.Fire(ODEventType.ModuleSelected,_tpModuleData);
			Plugins.HookAddCode(this,"ContrTreat.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected(){
			UpdateTPNoteIfNeeded();//Handle this here because this happens before textNote_Leave() and we dont want anything nulled before saving;
			_family=null;
			PatientCur=null;
			_listInsPlans=null;
			//Claims.List=null;
			//ClaimProcs.List=null;
			//from FillMain:
			_listProcedures=null;
			_listProceduresTP=null;
			//Procedures.HList=null;
			//Procedures.MissingTeeth=null;
			_patNumLast=0;//Clear out the last pat num so that a security log gets entered that the module was "visited" or "refreshed".
			HasNoteChanged=false;
			Plugins.HookAddCode(this,"ContrTreat.ModuleUnselected_end");
		}

		private void RefreshModuleData(long patNum) {
			UpdateTPNoteIfNeeded();
			if(patNum==0) {
				_family=null;
				PatientCur=null;
				_listInsSubs=null;
				_listInsPlans=null;
				_listSubstitutionLinks=null;
				_listPatPlans=null;
				_listBenefits=null;
				_listClaims=null;
				_listClaimProcHists=null;
				_listProcedures=null;
				_listTreatPlans=null;
				_listProcTPs=null;
				return;
			}
			//patNum is not zero.
			bool doMakeSecLog=false;
			if(_patNumLast!=patNum) {
				doMakeSecLog=true;
				_patNumLast=patNum;
			}
			try {
				_tpModuleData=TreatmentPlanModules.GetModuleData(patNum,doMakeSecLog);
			}
			catch(ApplicationException ex) {
				if(ex.Message=="Missing codenum") {
					MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
					PatientCur=null;
					return;
				}
				throw;
			}
			_family=_tpModuleData.Fam;
			PatientCur=_tpModuleData.Pat;
			_listInsSubs=_tpModuleData.SubList;
			_listInsPlans=_tpModuleData.InsPlanList;
			_listSubstitutionLinks=_tpModuleData.ListSubstLinks;
			_listPatPlans=_tpModuleData.PatPlanList;
			_listBenefits=_tpModuleData.BenefitList;
			_listClaims=_tpModuleData.ClaimList;
			_listClaimProcHists=_tpModuleData.HistList;
			_listProcedures=_tpModuleData.ListProcedures;
			_listTreatPlans=_tpModuleData.ListTreatPlans;
			_listProcTPs=_tpModuleData.ListProcTPs;
		}

		private void RefreshModuleScreen(bool doRefreshData=true){
			//ParentForm.Text=Patients.GetMainTitle(PatCur);
			FillPlans(doRefreshData);
			UpdateToolbarButtons();
			if(PatientCur==null) {
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
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatientCur.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",PatientCur.PatNum);
			StaticTextData staticTextData=new StaticTextData();
			List<GridRow> listGridRows=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();//Only pull selected rows that are procedures
			if(listGridRows.Count()>0) {//loop through selected procedures
				StaticTextFieldDependency staticTextFieldDependencies=StaticTextData.GetStaticTextDependencies(sheet.SheetFields);
				List<long> listProcNums=listGridRows.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcedures=Procedures.GetManyProc(listProcNums,false);//get list of procedures from list of procedureNums
				List<long> listCodeNums=listProcedures.Select(x => x.CodeNum).ToList();
				if(listCodeNums.Count()>0) {
					staticTextFieldDependencies|=StaticTextFieldDependency.ListSelectedTpProcs;
				}
				staticTextData=StaticTextData.GetStaticTextData(staticTextFieldDependencies,PatientCur,_family,listCodeNums);//set static text using selected procedure CodeNums
			}
			SheetFiller.FillFields(sheet,staticTextData: staticTextData);
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
				ProgramL.Execute(((Program)e.Button.Tag).ProgramNum,PatientCur);
			}
			Plugins.HookAddCode(this,"ContrTreat.ToolBarMain_ButtonClick_end",PatientCur,e);
		}

		private void checkShowCompleted_Click(object sender,EventArgs e) {
			_hasCheckShowCompletedChangedByUser=true;
		}

		private void butNewTP_Click(object sender,EventArgs e) {
			TreatPlanType treatPlanType=TreatPlanType.Insurance;
			if(_tpModuleData.DiscountPlanSub!=null) {
				treatPlanType=TreatPlanType.Discount;
			}
			using FormTreatPlanCurEdit formTreatPlanCurEdit=new FormTreatPlanCurEdit();
			formTreatPlanCurEdit.TreatPlanCur=new TreatPlan() {
				Heading="Inactive Treatment Plan",
				Note=PrefC.GetString(PrefName.TreatmentPlanNote),
				PatNum=PatientCur.PatNum,
				TPStatus=TreatPlanStatus.Inactive,
				TPType=treatPlanType,
			};
			formTreatPlanCurEdit.ShowDialog();
			if(formTreatPlanCurEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			long tpNum=formTreatPlanCurEdit.TreatPlanCur.TreatPlanNum;
			ModuleSelected(PatientCur.PatNum);//refreshes TPs
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
			gridPlans.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTPList","Date"),70);
			gridPlans.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Status"),50);
			gridPlans.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Heading"),165);
			gridPlans.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTPList","Signed"),50,HorizontalAlignment.Center);
			gridPlans.Columns.Add(col);
			col=new GridColumn(Lan.g("TabkeTPList","eClipboard"),75,HorizontalAlignment.Center);
			gridPlans.Columns.Add(col);
			gridPlans.ListGridRows.Clear();
			if(PatientCur==null){
				gridPlans.EndUpdate();
				return;
			}
			if(doRefreshData || _listProcedures==null) {
				_listProcedures=Procedures.Refresh(PatientCur.PatNum);
			}
			_listProceduresTP=Procedures.SortListByTreatPlanPriority(_listProcedures.FindAll(x => x.ProcStatus==ProcStat.TP || x.ProcStatus==ProcStat.TPi)
				,PrefC.IsTreatPlanSortByTooth);//sorted by priority, then (conditionally) toothnum
			//_listTPCurrent=TreatPlans.Refresh(PatCur.PatNum,new[] {TreatPlanStatus.Active,TreatPlanStatus.Inactive});
			//todo: get all mobile devices here instead
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetAll(PatientCur.PatNum);
			if(doRefreshData || _listTreatPlans==null) {
				_listTreatPlans=TreatPlans.GetAllForPat(PatientCur.PatNum);
				//Check if the treatment plans are actually still on mobile devices. If eClipboard crashed, for example, then the patient is no longer on that device,
				//but treatment plans MobileAppDeviceNum wouldn't have been cleared.
				if(listMobileAppDevices.Count==0) {
					//Patient is on no devices, so clear out MobileAppDeviceNum on treatment plans
					for(int i=0;i<_listTreatPlans.Count;i++) {
						if(_listTreatPlans[i].MobileAppDeviceNum>0) {
							TreatPlans.UpdateMobileAppDeviceNum(_listTreatPlans[i],0);
							Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,_listTreatPlans[i].PatNum);
							Signalods.SetInvalid(InvalidType.EClipboard);
						}
					}
					//Patient isn't on a device, so there shouldn't be any TreatPlanParams for the pat.
					TreatPlanParams.RemoveAllByPatNum(PatientCur.PatNum);
				}
			}
			_listTreatPlans=_listTreatPlans
					.OrderBy(x => x.TPStatus!=TreatPlanStatus.Active)
					.ThenBy(x => x.TPStatus!=TreatPlanStatus.Inactive)
					.ThenBy(x => x.DateTP).ToList();
			if(doRefreshData || _listProcTPs==null) {
				_listProcTPs=ProcTPs.Refresh(PatientCur.PatNum);
			}
			if(doRefreshData) {
				_tpModuleData.DiscountPlanSub=DiscountPlanSubs.GetSubForPat(PatientCur.PatNum);
			}
			OpenDental.UI.GridRow row;
			//row=new ODGridRow();
			//row.Cells.Add("");//date empty
			//row.Cells.Add("");//date empty
			//row.Cells.Add(Lan.g(this,"Current Treatment Plans"));
			//gridPlans.Rows.Add(row);
			string str;
			for(int i=0;i<_listTreatPlans.Count;i++){
				row=new GridRow();
				TreatPlan treatPlan=_listTreatPlans[i];
				row.Cells.Add(treatPlan.TPStatus==TreatPlanStatus.Saved? treatPlan.DateTP.ToShortDateString():"");
				row.Cells.Add(treatPlan.TPStatus.ToString());
				str=treatPlan.Heading;
				if(treatPlan.ResponsParty!=0){
					str+="\r\n"+Lan.g(this,"Responsible Party: ")+Patients.GetLim(treatPlan.ResponsParty).GetNameLF();
				}
				row.Cells.Add(str);
				if(string.IsNullOrEmpty(treatPlan.Signature)) {
					row.Cells.Add("");
				}
				else{
					row.Cells.Add("X");
				}
				if(treatPlan.MobileAppDeviceNum>0 
					&& listMobileAppDevices.FirstOrDefault(x => x.MobileAppDeviceNum==treatPlan.MobileAppDeviceNum)?.PatNum==treatPlan.PatNum) 
				{
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=treatPlan;
				gridPlans.ListGridRows.Add(row);
			}
			gridPlans.EndUpdate();
			gridPlans.SetSelected(0,true);
		}

		private void FillMain() {
			//Newly selected TreatPlan, if any.
			TreatPlan treatPlan=(gridPlans.SelectedIndices.Length > 0) ? _listTreatPlans[gridPlans.SelectedIndices[0]] : null;
			//If changing selection or initial selection.
			bool isChangingTP=(treatPlan!=null && treatPlan.TreatPlanNum!=(_treatPlanSelected?.TreatPlanNum));
			if(isChangingTP) {
				_treatPlanSelected=treatPlan;//Update to new selection.
			}
			if((treatPlan!=null && treatPlan.Signature!="")//disable changing priorities for signed TPs
				|| PatientCur==null ||_listTreatPlans.Count<1)//disable if the patient has no TPs
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
			_listTpRowsMain=new List<TpRow>();
			if(PatientCur==null || gridPlans.ListGridRows.Count==0) {
				return;
			}
			TreatPlan treatPlanTemp=_listTreatPlans[gridPlans.SelectedIndices[0]];
			if(treatPlanTemp.TPStatus==TreatPlanStatus.Active || treatPlanTemp.TPStatus==TreatPlanStatus.Inactive) { //Active and Inactive Treatment Plans
				treatPlanTemp.ListProcTPs=LoadTP(treatPlanTemp);
			}
			else { //Archived TPs
				LoadArchivedTP(treatPlanTemp);
			}
		}

		private void FillMainDisplay(bool hasSelectedTpChanged=false){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			if(PatientCur==null || gridPlans.ListGridRows.Count==0 || _listTreatPlans[gridPlans.SelectedIndices[0]].TPType==TreatPlanType.Insurance) {
				listDisplayFields.RemoveAll(x => x.InternalName=="DPlan");//If patient doesn't have discount plan, don't show column.
			}
			else {
				listDisplayFields.RemoveAll(x => x.InternalName=="Pri Ins" || x.InternalName=="Sec Ins" || x.InternalName=="Allowed");//If patient does have discount plan, don't show Pri or Sec Ins or allowed fee.
			}
			bool hasSalesTax=HasSalesTax(listDisplayFields);
			//Changing to TreatPlan other than Active/Inactive.  CheckBox states have already been reset to user selections if changing to the Active/Inactive TreatPlan.
			if(hasSelectedTpChanged && !_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus.In(TreatPlanStatus.Active,TreatPlanStatus.Inactive)) {
				bool wasShowInsChecked=checkShowIns.Checked;
				bool wasShowDiscountChecked = checkShowDiscount.Checked;
				if((!wasShowInsChecked && checkShowIns.Checked) || (!wasShowDiscountChecked && checkShowDiscount.Checked)) {//checkShowIns was changed to checked, which affects how FillMainData() sets each row.Fee.
					FillMainData();//So we must refill main data.
				}
			}
			for(int i=0;i<listDisplayFields.Count;i++){
				if(listDisplayFields[i].Description==""){
					col=new GridColumn(listDisplayFields[i].InternalName,listDisplayFields[i].ColumnWidth);
				}
				else{
					col=new GridColumn(listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth);
				}
				if(listDisplayFields[i].InternalName=="Fee" && !checkShowFees.Checked){
					continue;
				}
				if((listDisplayFields[i].InternalName=="Pri Ins" || listDisplayFields[i].InternalName=="Sec Ins" || listDisplayFields[i].InternalName=="DPlan" || listDisplayFields[i].InternalName=="Allowed") 
					&& !checkShowIns.Checked){
					continue;
				}
				if(listDisplayFields[i].InternalName=="Discount" && !checkShowDiscount.Checked){
					continue;
				}
				if(listDisplayFields[i].InternalName=="Pat" && !checkShowIns.Checked && !checkShowDiscount.Checked && !hasSalesTax){
					continue;
				}
				if(listDisplayFields[i].InternalName=="Tax Est" && !hasSalesTax) {
					continue;
				}
				if(listDisplayFields[i].InternalName=="Fee" 
					|| listDisplayFields[i].InternalName=="Pri Ins"
					|| listDisplayFields[i].InternalName=="Sec Ins"
					|| listDisplayFields[i].InternalName=="DPlan"
					|| listDisplayFields[i].InternalName=="Discount"
					|| listDisplayFields[i].InternalName=="Pat"
					|| listDisplayFields[i].InternalName=="Allowed"
					|| listDisplayFields[i].InternalName=="Tax Est"
					|| listDisplayFields[i].InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR) 
				{
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(listDisplayFields[i].InternalName.In("Sub",DisplayFields.InternalNames.TreatmentPlanModule.Appt)) {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridMain.Columns.Add(col);
			}			
			gridMain.ListGridRows.Clear();
			if(PatientCur==null){
				gridMain.EndUpdate();
				return;
			}
			if(_listTpRowsMain==null || _listTpRowsMain.Count==0) {
				gridMain.EndUpdate();
				return;
			}
			GridRow row;
			for(int i=0;i<_listTpRowsMain.Count;i++){
				row=new GridRow();
				for(int j=0;j<listDisplayFields.Count;j++) {
					switch(listDisplayFields[j].InternalName) {
						case "Done":
							if(_listTpRowsMain[i].Done!=null) {
								row.Cells.Add(_listTpRowsMain[i].Done.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Priority":
							if(_listTpRowsMain[i].Priority!=null) {
								row.Cells.Add(_listTpRowsMain[i].Priority.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tth":
							if(_listTpRowsMain[i].Tth!=null) {
								row.Cells.Add(_listTpRowsMain[i].Tth.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Surf":
							if(_listTpRowsMain[i].Surf!=null) {
								row.Cells.Add(_listTpRowsMain[i].Surf.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Code":
							if(_listTpRowsMain[i].Code!=null) {
								row.Cells.Add(_listTpRowsMain[i].Code.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Sub":
							if(HasSubstCodeForTpRow(_listTpRowsMain[i])) {
								row.Cells.Add("X");//They allow substitutions.
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Description":
							string description="";
							if(_listTpRowsMain[i].Description!=null) {
								description=_listTpRowsMain[i].Description.ToString();
							}
							//Do not display tax in description if tax column is being displayed, the tax amount is zero, or in the subtotal/total rows. 
							if(!hasSalesTax && !CompareDecimal.IsZero(_listTpRowsMain[i].TaxEst) && !(_listTpRowsMain[i].Description=="Subtotal") && !(_listTpRowsMain[i].Description=="Total")) {
								description+="\nTax Est: "+_listTpRowsMain[i].TaxEst.ToString("F");
							}
							row.Cells.Add(description);
							break;
						case "Fee":
							if(checkShowFees.Checked) {
								row.Cells.Add(_listTpRowsMain[i].Fee.ToString("F"));
							}
							break;
						case "Pri Ins":
							if(checkShowIns.Checked) {
								row.Cells.Add(_listTpRowsMain[i].PriIns.ToString("F"));
							}
							break;
						case "DPlan":
							if(checkShowIns.Checked) {
								row.Cells.Add(_listTpRowsMain[i].PriIns.ToString("F"));
							}
							break;
						case "Sec Ins":
							if(checkShowIns.Checked) {
								row.Cells.Add(_listTpRowsMain[i].SecIns.ToString("F"));
							}
							break;
						case "Discount":
							if(checkShowDiscount.Checked) {
								row.Cells.Add(_listTpRowsMain[i].Discount.ToString("F"));
							}
							break;
						case "Pat":
							if(checkShowIns.Checked || checkShowDiscount.Checked || hasSalesTax) {
								row.Cells.Add(_listTpRowsMain[i].Pat.ToString("F"));
							}
							break;
						case "Prognosis":
							if(_listTpRowsMain[i].Prognosis!=null) {
								row.Cells.Add(_listTpRowsMain[i].Prognosis.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Dx":
							if(_listTpRowsMain[i].Dx!=null) {
								row.Cells.Add(_listTpRowsMain[i].Dx.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Abbr":
							if(!String.IsNullOrEmpty(_listTpRowsMain[i].ProcAbbr)){
								row.Cells.Add(_listTpRowsMain[i].ProcAbbr.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Allowed":
							if(checkShowIns.Checked) {
								if(CompareDecimal.IsGreaterThan(_listTpRowsMain[i].FeeAllowed,-1)) {//-1 means the proc is DoNotBillIns
									row.Cells.Add(_listTpRowsMain[i].FeeAllowed.ToString("F"));
								}
								else {
									row.Cells.Add("X");
								}
							}
							break;
						case "Tax Est":
							if(hasSalesTax) {
								row.Cells.Add(_listTpRowsMain[i].TaxEst.ToString("F"));
							}
							break;
						case "Prov":
							if(_listTpRowsMain[i].ProvNum>0){
								row.Cells.Add(Providers.GetAbbr(_listTpRowsMain[i].ProvNum));
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "DateTP":
							if(_listTpRowsMain[i].DateTP.Year>=1880) {
								row.Cells.Add(_listTpRowsMain[i].DateTP.ToShortDateString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(_listTpRowsMain[i].ClinicNum));//Will be blank if ClinicNum not found, i.e. the 0 clinic.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.Appt:
							row.Cells.Add(_listTpRowsMain[i].Appt);//"X" if procedure has an AptNum>0 otherwise blank.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR:
							row.Cells.Add(_listTpRowsMain[i].CatPercUCR.ToString("F"));
							break;
					}
				}
				if(_listTpRowsMain[i].ColorText!=null) {
					row.ColorText=_listTpRowsMain[i].ColorText;
				}
				if(_listTpRowsMain[i].ColorLborder!=null) {
					row.ColorLborder=_listTpRowsMain[i].ColorLborder;
				}
				if(_listTpRowsMain[i].Tag!=null) {
					row.Tag=_listTpRowsMain[i].Tag;
				}
				row.Bold=_listTpRowsMain[i].Bold;
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillGridPrint() {
			this.Controls.Add(gridPrint);
			gridPrint.BeginUpdate();
			gridPrint.Columns.Clear();
			GridColumn col;
			DisplayFields.RefreshCache();//probably needs to be removed.
			List<DisplayField> listDisplayFields=DisplayFields.GetForCategory(DisplayFieldCategory.TreatmentPlanModule);
			if(PatientCur==null || gridPlans.ListGridRows.Count==0 || _listTreatPlans[gridPlans.SelectedIndices[0]].TPType==TreatPlanType.Insurance) {
				listDisplayFields.RemoveAll(x => x.InternalName=="DPlan");//If patient doesn't have discount plan, don't show column.
			}
			else {
				listDisplayFields.RemoveAll(x => x.InternalName=="Pri Ins" || x.InternalName=="Sec Ins");//If patient does have discount plan, don't show Pri or Sec Ins.
			}
			bool hasSalesTax=HasSalesTax(listDisplayFields);
			for(int i=0;i<listDisplayFields.Count;i++) {
				if(listDisplayFields[i].Description=="") {
					col=new GridColumn(listDisplayFields[i].InternalName,listDisplayFields[i].ColumnWidth);
				}
				else {
					col=new GridColumn(listDisplayFields[i].Description,listDisplayFields[i].ColumnWidth);
				}
				if(listDisplayFields[i].InternalName=="Fee" && !checkShowFees.Checked) {
					continue;
				}
				if((listDisplayFields[i].InternalName.In("Pri Ins","Sec Ins","DPlan","Allowed")) && !checkShowIns.Checked) {
					continue;
				}
				if(listDisplayFields[i].InternalName=="Discount" && !checkShowDiscount.Checked) {
					continue;
				}
				if(listDisplayFields[i].InternalName=="Pat" && !checkShowIns.Checked && !checkShowDiscount.Checked && !hasSalesTax) {
					continue;
				}
				if(listDisplayFields[i].InternalName=="Tax Est" && !hasSalesTax) {
					continue;
				}
				if(listDisplayFields[i].InternalName=="Fee" 
					|| listDisplayFields[i].InternalName=="Pri Ins"
					|| listDisplayFields[i].InternalName=="Sec Ins"
					|| listDisplayFields[i].InternalName=="DPlan"
					|| listDisplayFields[i].InternalName=="Discount"
					|| listDisplayFields[i].InternalName=="Pat"
					|| listDisplayFields[i].InternalName=="Tax Est"
					|| listDisplayFields[i].InternalName==DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR)
				{
					col.TextAlign=HorizontalAlignment.Right;
				}
				if(listDisplayFields[i].InternalName.In("Sub",DisplayFields.InternalNames.TreatmentPlanModule.Appt)) {
					col.TextAlign=HorizontalAlignment.Center;
				}
				gridPrint.Columns.Add(col);
			}
			gridPrint.ListGridRows.Clear();
			if(PatientCur==null) {
				gridPrint.EndUpdate();
				return;
			}
			GridRow row;
			for(int i=0;i<_listTpRowsMain.Count;i++) {
				row=new GridRow();
				for(int j=0;j<listDisplayFields.Count;j++) {
					switch(listDisplayFields[j].InternalName) {
						case "Done":
							if(_listTpRowsMain[i].Done!=null) {
								row.Cells.Add(_listTpRowsMain[i].Done.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Priority":
							if(_listTpRowsMain[i].Priority!=null) {
								row.Cells.Add(_listTpRowsMain[i].Priority.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tth":
							if(_listTpRowsMain[i].Tth!=null) {
								row.Cells.Add(_listTpRowsMain[i].Tth.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Surf":
							if(_listTpRowsMain[i].Surf!=null) {
								row.Cells.Add(_listTpRowsMain[i].Surf.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Code":
							if(_listTpRowsMain[i].Code!=null) {
								row.Cells.Add(_listTpRowsMain[i].Code.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Sub":
							if(HasSubstCodeForTpRow(_listTpRowsMain[i])) {
								row.Cells.Add("X");//They allow substitutions.
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Description":
							if(_listTpRowsMain[i].Description!=null) {
								row.Cells.Add(_listTpRowsMain[i].Description.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Fee":
							if(checkShowFees.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(_listTpRowsMain[i].Fee.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Pri Ins":
						case "DPlan":
							if(checkShowIns.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(_listTpRowsMain[i].PriIns.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Sec Ins":
							if(checkShowIns.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(_listTpRowsMain[i].SecIns.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Discount":
							if(checkShowDiscount.Checked) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal"))
								{
									row.Cells.Add(_listTpRowsMain[i].Discount.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Pat":
							if(checkShowIns.Checked || checkShowDiscount.Checked || hasSalesTax) {
								if(PrefC.GetBool(PrefName.TreatPlanItemized) || _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Total")
									|| _listTpRowsMain[i].Description.ToString()==Lan.g("TableTP","Subtotal")) 
								{
									row.Cells.Add(_listTpRowsMain[i].Pat.ToString("F"));
								}
								else {
									row.Cells.Add("");
								}
							}
							break;
						case "Prognosis":
							if(_listTpRowsMain[i].Prognosis!=null) {
								row.Cells.Add(_listTpRowsMain[i].Prognosis.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Dx":
							if(_listTpRowsMain[i].Dx!=null) {
								row.Cells.Add(_listTpRowsMain[i].Dx.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Abbr":
							if(!String.IsNullOrEmpty(_listTpRowsMain[i].ProcAbbr)){
								row.Cells.Add(_listTpRowsMain[i].ProcAbbr.ToString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Tax Est":
							if(hasSalesTax) {
								row.Cells.Add(_listTpRowsMain[i].TaxEst.ToString("F"));
							}
							break;
						case "Prov":
							if(_listTpRowsMain[i].ProvNum>0){
								row.Cells.Add(Providers.GetAbbr(_listTpRowsMain[i].ProvNum));
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "DateTP":
							if(_listTpRowsMain[i].DateTP.Year>=1880) {
								row.Cells.Add(_listTpRowsMain[i].DateTP.ToShortDateString());
							}
							else {
								row.Cells.Add("");
							}
							break;
						case "Clinic":
							row.Cells.Add(Clinics.GetAbbr(_listTpRowsMain[i].ClinicNum));//Will be blank if ClinicNum not found, i.e. the 0 clinic.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.Appt:
							row.Cells.Add(_listTpRowsMain[i].Appt);//"X" if procedure has an AptNum>0 otherwise blank.
							break;
						case DisplayFields.InternalNames.TreatmentPlanModule.CatPercUCR:
							if(PrefC.GetBool(PrefName.TreatPlanItemized) 
								|| _listTpRowsMain[i].Description.ToString().In(Lan.g("TableTP","Total"),Lan.g("TableTP","Subtotal"))) 
							{
								row.Cells.Add(_listTpRowsMain[i].CatPercUCR.ToString("F"));
							}
							else {
								row.Cells.Add("");
							}
							break;
					}
				}
				if(_listTpRowsMain[i].ColorText!=null) {
					row.ColorText=_listTpRowsMain[i].ColorText;
				}
				if(_listTpRowsMain[i].ColorLborder!=null) {
					row.ColorLborder=_listTpRowsMain[i].ColorLborder;
				}
				if(_listTpRowsMain[i].Tag!=null) {
					row.Tag=_listTpRowsMain[i].Tag;
				}
				row.Bold=_listTpRowsMain[i].Bold;
				gridPrint.ListGridRows.Add(row);
			}
			gridPrint.EndUpdate();
		}

		private bool HasSubstCodeForTpRow(TpRow row) {
			//If any patient insplan allows subst codes (if !plan.CodeSubstNone) and the code has a valid substitution code, then indicate the substitution.
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(row.Code);
			if(!ProcedureCodes.IsValidCode(procedureCode.ProcCode)) {
				//TpRow is not a valid procedure. Return false.
				return false;
			}
			//The lists gotten at the beginning of ContrTreat are not patient specific with the exception of the PatPlanList.
			//Get all patient-specific InsSubs
			List<InsSub> listInsSubs=_listInsSubs.FindAll(x => _listPatPlans.Any(y => y.InsSubNum==x.InsSubNum));
			//Get all patient-specific InsPlans
			List<InsPlan> listInsPlans=_listInsPlans.FindAll(x => listInsSubs.Any(y => y.PlanNum==x.PlanNum));
			return SubstitutionLinks.HasSubstCodeForProcCode(procedureCode,row.Tth.ToString(),_listSubstitutionLinks,listInsPlans);
		}

		///<summary>Helper method used to determine if sales tax is displayed.</summary>
		private bool HasSalesTax(List<DisplayField> listDisplayFields) {
			if(PatientCur!=null && listDisplayFields.Any(x => x.InternalName=="Tax Est")) {
				if(PrefC.IsODHQ) {
					return AvaTax.IsTaxable(PatientCur.PatNum);
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
			if(PatientCur==null) {
				return;
			}
			if(_tpModuleData.DiscountPlanSub!=null) {
				userControlIndDis.Visible=true;
				userControlIndDis.RefreshDiscountPlan(PatientCur,_tpModuleData.DiscountPlanSub,_tpModuleData.DiscountPlan);
			} 
			else {
				userControlFamIns.Visible=true;
				userControlIndIns.Visible=true;
				userControlFamIns.RefreshInsurance(PatientCur,_listInsPlans,_listInsSubs,_listPatPlans,_listBenefits);
				userControlIndIns.RefreshInsurance(PatientCur,_listInsPlans,_listInsSubs,_listPatPlans,_listBenefits,_listClaimProcHists,dateTimeTP.Value);
			}
		}		

		private void FillPreAuth(){
			gridPreAuth.BeginUpdate();
			gridPreAuth.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TablePreAuth","Date Sent"),80);
			gridPreAuth.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePreAuth","Carrier"),100);
			gridPreAuth.Columns.Add(col);
			col=new GridColumn(Lan.g("TablePreAuth","Status"),53);
			gridPreAuth.Columns.Add(col);
			gridPreAuth.ListGridRows.Clear();
			if(PatientCur==null){
				gridPreAuth.EndUpdate();
				return;
			}
			_arrayListPreAuth=new ArrayList();			
			for(int i=0;i<_listClaims.Count;i++){
				if(_listClaims[i].ClaimType=="PreAuth"){
					_arrayListPreAuth.Add(_listClaims[i]);
				}
			}
			OpenDental.UI.GridRow row;
			for(int i=0;i<_arrayListPreAuth.Count;i++) {
				if(_arrayListPreAuth[i]==null) {
					continue;
				}
				InsPlan insPlan=InsPlans.GetPlan(((Claim)_arrayListPreAuth[i]).PlanNum,_listInsPlans);
				row=new GridRow();
				if(((Claim)_arrayListPreAuth[i]).DateSent.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(((Claim)_arrayListPreAuth[i]).DateSent.ToShortDateString());
				}
				if(insPlan==null) {
					row.Cells.Add(Lan.g(this,"Unknown"));
				}
				else {
					row.Cells.Add(Carriers.GetName(insPlan.CarrierNum));
				}
				row.Cells.Add(((Claim)_arrayListPreAuth[i]).ClaimStatus.ToString());
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
		private void CanadianSelectedRowHelper(ProcTP procTpSelected) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return;
			}
			long selectedProcNumLab=(long)procTpSelected.TagOD;//0 or FK to parent proc
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag==null){
					continue;//skip any hightlighted subtotal lines
				}
				long rowNumProcOrig=((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig;
				long rowNumParentProc=(long)((ProcTP)gridMain.ListGridRows[i].Tag).TagOD;//0 or FK to parent proc
				if(rowNumProcOrig==selectedProcNumLab //User clicked lab, select parent proc too.
					|| (rowNumParentProc!=0 && rowNumParentProc==selectedProcNumLab)//User clicked lab, select other labs associated to same parent proc.
					|| (procTpSelected.ProcNumOrig==rowNumParentProc))//User clicked parent, select associated lab procs.
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
				TreatPlan treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
				Procedure procedure=Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[e.Row].Tag).ProcNumOrig,true); 
				//generate a new loop list containing only the procs before this one in it
				List<ClaimProcHist> listClaimProcHistsLoops=new List<ClaimProcHist>();
				for(int i=0;i<_listProceduresTP.Count;i++) {
					if(_listProceduresTP[i].ProcNum==procedure.ProcNum) {
						break;
					}
					if(treatPlan.TPStatus==TreatPlanStatus.Active && !_loadActiveTPData.ListTreatPlanAttaches
						.Any(x => x.ProcNum==_listProceduresTP[i].ProcNum && x.TreatPlanNum==treatPlan.TreatPlanNum)) 
					{
						continue;//If this is the active plan, only include TP procs that are attached to this treatment plan
					}
					listClaimProcHistsLoops.AddRange(ClaimProcs.GetHistForProc(_listClaimProcs,_listProceduresTP[i],_listProceduresTP[i].CodeNum));
				}
				using FormProcEdit formProcEdit=new FormProcEdit(procedure,PatientCur,_family,listToothInitials:_listToothInitials);
				formProcEdit.ListClaimProcHistsLoop=listClaimProcHistsLoops;
				formProcEdit.ListClaimProcHists=_listClaimProcHists;
				formProcEdit.ShowDialog();
				long treatPlanNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
				ModuleSelected(PatientCur.PatNum);
				gridPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(x=>x.TreatPlanNum==treatPlanNum)),true);
				//This only updates the grid of procedures, in case any changes were made.
				FillMain();
				for(int i=0;i<gridMain.ListGridRows.Count;i++){
					if(gridMain.ListGridRows[i].Tag !=null && ((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig==procedure.ProcNum){
						gridMain.SetSelected(i,true);
					}
				}
				return;
			}
			//any other TP
			ProcTP procTP=(ProcTP)gridMain.ListGridRows[e.Row].Tag;
			DateTime dateTP=_listTreatPlans[gridPlans.SelectedIndices[0]].DateTP;
			bool isSigned=false;
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="") {
				isSigned=true;
			}
			using FormProcTPEdit formProcTPEdit=new FormProcTPEdit(procTP,dateTP,isSigned);
			formProcTPEdit.ShowDialog();
			if(formProcTPEdit.DialogResult==DialogResult.Cancel){
				return;
			}
			int selectedPlanI=gridPlans.SelectedIndices[0];
			long procNumSelected=procTP.ProcTPNum;
			bool isDiscountChecked=checkShowDiscount.Checked;
			ModuleSelected(PatientCur.PatNum);
			gridPlans.SetSelected(selectedPlanI,true);
			checkShowDiscount.Checked=isDiscountChecked;
			if(formProcTPEdit.ProcTPCur!=null && formProcTPEdit.ProcTPCur.Discount>0) {
				checkShowDiscount.Checked=true;
			}
			FillMain();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(gridMain.ListGridRows[i].Tag !=null && ((ProcTP)gridMain.ListGridRows[i].Tag).ProcTPNum==procNumSelected){ 
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
					listProcTPs=_listProcTPs.FindAll(x => x.TreatPlanNum==treatPlan.TreatPlanNum);
				}
				if(CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.Discount)) && !_checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
				if(!_checkShowInsNotAutomatic) {
					if(CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.PriInsAmt)) 
						|| CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.SecInsAmt)) 
						|| CompareDecimal.IsGreaterThanZero(listProcTPs.Sum(x => x.FeeAllowed)))
					{
						checkShowIns.Checked=true;
					}
				}
			}
			if(gridPlans.Columns[e.Col].Heading=="eClipboard") {
				if(_listTreatPlans[gridPlans.SelectedIndices[0]].MobileAppDeviceNum>0) {
					if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to recall this treatment plan from the mobile device?")) {
						treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
						PushNotificationUtils.CI_RemoveTreatmentPlan(treatPlan.MobileAppDeviceNum,treatPlan);
						FillPlans();
						return;
					}
				}
			}
			FillMain();
			gridPreAuth.SetAll(false);
			if(gridPlans.SelectedIndices.Length <= 0) {
				return;
			}
			if(treatPlan.TPStatus==TreatPlanStatus.Saved && treatPlan.DateTP < UpdateHistories.GetDateForVersion(new Version(17,1,0,0))) {
				//In 17.1 we forced everyone to switch to using sheets for TPs. In order to avoid making it appear that historical data has changed,
				//we give the option to print using the classic view for treatment plans that were saved before updating to 17.1.
				if(!tabControlShowSort.TabPages.Contains(tabPagePrint)) {
					LayoutManager.Add(tabPagePrint,tabControlShowSort);
				}
				return;
			}
			if(tabControlShowSort.TabPages.Contains(tabPagePrint)) {
				tabControlShowSort.TabPages.Remove(tabPagePrint);
			}
		}

		private void gridPlans_CellDoubleClick(object sender, OpenDental.UI.ODGridClickEventArgs e) {
			//if(e.Row==0){
			//	return;//there is nothing to edit if user clicks on current.
			//}
			long tpNum=_listTreatPlans[e.Row].TreatPlanNum;
			TreatPlan treatPlanSelected=_listTreatPlans[e.Row];
			if(treatPlanSelected.TPStatus==TreatPlanStatus.Saved) {
				using FormTreatPlanEdit formTreatPlanEdit=new FormTreatPlanEdit(treatPlanSelected);
				formTreatPlanEdit.ShowDialog();
			}
			else {//Active or Inactive
				using FormTreatPlanCurEdit formTreatPlanCurEdit=new FormTreatPlanCurEdit();
				formTreatPlanCurEdit.TreatPlanCur=treatPlanSelected;
				formTreatPlanCurEdit.ShowDialog();
			}
			ModuleSelected(PatientCur.PatNum);
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
			TreatPlan treatPlan=gridPlans.SelectedTag<TreatPlan>();
			Sheet sheetTP=null;
			if(PatientCur==null) {
				return;
			}
			if(DoPrintUsingSheets()) {
				sheetTP=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.TreatmentPlan),PatientCur.PatNum);
			}
			bool hasPracticeSig=sheetTP?.SheetFields?.Any(x => x.FieldType==SheetFieldType.SigBoxPractice)??false;
			if(PatientCur==null){
				MsgBox.Show("Please select a patient first.");
				return;
			}
			if(treatPlan==null) {//Shouldn't happen, control auto selects when loading.
				MsgBox.Show("Please select a Saved Treatment Plan first.");
				return;
			}
			else if(treatPlan.TPStatus!=TreatPlanStatus.Saved){
				//Eventually we plan on implementing something here to save TPs for user when not saved.
				MsgBox.Show("Only Saved Treatment Plans can be sent to a mobile device.");
				return;
			}
			else if(!string.IsNullOrEmpty(treatPlan.Signature) || (hasPracticeSig && !string.IsNullOrEmpty(treatPlan.SignaturePractice)))
			{
				MsgBox.Show("This Treatment Plan has already been signed.");
				return;
			}
			if(MobileAppDevices.ShouldSendPush(PatientCur.PatNum, out MobileAppDevice device)) {
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
			using PdfDocument pdfDocument=GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig); //Cant be null due to above check.
			if(PushNotificationUtils.CI_SendTreatmentPlan(pdfDocument,treatPlan,hasPracticeSig,mobileAppDevice.MobileAppDeviceNum
				,out string errorMsg,out long mobileDataByeNum)) 
			{
				SendTreatPlanParam(treatPlan);
				//The treatment plan's MobileAppDeviceNum needs to be updated so that we know it is on a device
				FillPlans();
				MsgBox.Show($"Treatment Plan sent to device: {mobileAppDevice.DeviceName}");
				return;
			}
			//Error occurred
			//It failed to send to device, so clear out what ever device num was there
			TreatPlans.UpdateMobileAppDeviceNum(treatPlan,0);
			Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
			Signalods.SetInvalid(InvalidType.EClipboard);
			MsgBox.Show($"Error sending Treatment Plan: {errorMsg}");
		}

		///<summary>Opens a FormMobileCode window with the currently selected TP.</summary>
		private void OpenUnlockCodeForTP(){
			long treatPlanParamNum=0;
			MobileDataByte funcInsertDataForUnlockCode(string unlockCode) {
				using PdfDocument pdfDocument=GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig); 
				if(MobileDataBytes.TryInsertTreatPlanPDF(pdfDocument,treatPlan,hasPracticeSig,unlockCode,out string errorMsg,out MobileDataByte mobileDataByte)){
					treatPlanParamNum=SendTreatPlanParam(treatPlan);
					return mobileDataByte;
				}
				//Failed to insert mobile data byte and won't be retrievable in eClipboard so clear out mobile app device num
				TreatPlans.UpdateMobileAppDeviceNum(treatPlan,0);
				Signalods.SetInvalid(InvalidType.TPModule,KeyType.PatNum,treatPlan.PatNum);
				Signalods.SetInvalid(InvalidType.EClipboard);
				MsgBox.Show(errorMsg);
				return null;
			}
			using FormMobileCode formMobileCode=new FormMobileCode(funcInsertDataForUnlockCode);
			formMobileCode.ShowDialog();
			if(formMobileCode.DialogResult==DialogResult.Cancel && treatPlanParamNum>0) {
				//A TreatPlanParam was inserted into the DB already, so it needs to be removed if the unlock code isn't used.
				TreatPlanParams.Delete(treatPlanParamNum);
			}
		}

		///<summary>Inserts a new TreatPlanParam into the database if PrefName.TreatPlanSaveSignedToPdf is true.
		///This will be used when they save from eClipboard and it needs to create a signed treatment plan PDF based on these check boxes.</summary>
		private long SendTreatPlanParam(TreatPlan treatPlan) {
			if(!PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {//no need to create a treatPlanParam if not saving PDF
				return 0;
			}
			return TreatPlanParams.Insert(new TreatPlanParam() {
				IsNew=true,
				PatNum=treatPlan.PatNum,
				TreatPlanNum=treatPlan.TreatPlanNum,
				ShowCompleted=checkShowCompleted.Checked,
				ShowDiscount=checkShowDiscount.Checked,
				ShowFees=checkShowFees.Checked,
				ShowIns=checkShowIns.Checked,
				ShowMaxDed=checkShowMaxDed.Checked,
				ShowSubTotals=checkShowSubtotals.Checked,
				ShowTotals=checkShowTotals.Checked
			});
		}

		///<summary>Returns a PDF for the currently selected TreatmentPlan and sets out TreatmentPlan to selected TreatmentPlan.
		///If nothing is selected in gridPlans then returns null and out TreatPlan is set to null.</summary>
		private PdfDocument GetTreatPlanPDF(out TreatPlan treatPlan,out bool hasPracticeSig){
			treatPlan=null;
			hasPracticeSig=false;
			PdfDocument pdfDocument=null;
			if(gridPlans.SelectedIndices.Count()==1) {
				Action actionCloseProgress=ODProgress.Show(); //Immediately shows a progress window.
				//The following logic mimics ToolBarMainEmail_Click()
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
				if(DoPrintUsingSheets()) {
					if(treatPlan.TPStatus==TreatPlanStatus.Saved) {
						treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
					}
					else {
						treatPlan.ListProcTPs=LoadTP(treatPlan);
					}
					Sheet sheet=TreatPlanToSheet(treatPlan);
					hasPracticeSig=sheet.SheetFields.Any(x => x.FieldType==SheetFieldType.SigBoxPractice);
					pdfDocument=SheetPrinting.CreatePdf(sheet,"",null,null,null,null,null,false);
				}
				else {//generate and save a new document from scratch
					PrepImageForPrinting();
					PdfDocumentRenderer pdfDocumentRenderer=new PdfDocumentRenderer(true,PdfFontEmbedding.Always);
					pdfDocumentRenderer.Document=CreateDocument();
					pdfDocumentRenderer.RenderDocument();
					pdfDocument=pdfDocumentRenderer.PdfDocument;
				}
				actionCloseProgress();//Closes the progress window. 
			}
			return pdfDocument;
		}

		private void listSetPr_MouseDown(object sender,System.Windows.Forms.MouseEventArgs e) {
			int clickedRow=listSetPr.IndexFromPoint(e.Location);
			if(!clickedRow.Between(0,listSetPr.Items.Count-1)) {
				return;
			}
			Def defSelected=(Def)listSetPr.Items.GetObjectAt(clickedRow);
			if(defSelected==null) {
				return;
			}
			TreatPlan treatPlanSelected=gridPlans.SelectedIndices.Where(x => x>-1 && x<gridPlans.ListGridRows.Count)
				.Select(x => (TreatPlan)gridPlans.ListGridRows[x].Tag).FirstOrDefault();
			if(treatPlanSelected==null) {
				return;
			}
			SetPriority(defSelected,treatPlanSelected,gridMain.SelectedTags<ProcTP>());
			listSetPr.ClearSelected();
		}

		///<summary>Sets the priorities for the selected ProcTP.</summary>
		private void SetPriority(Def defSelected,TreatPlan treatPlanSelected,List<ProcTP> listProcTPsSelected) {
			List<long> listProcNumsSelected=listProcTPsSelected
				.Where(x => x!=null)
				.Select(x => x.ProcNumOrig)
				.ToList();
			TreatPlans.SetPriorityForProcs(treatPlanSelected,defSelected.DefNum,listProcNumsSelected,_listTreatPlans.Count);
			ModuleSelected(PatientCur.PatNum);//Refresh the entire module in order to get the new priorities from the database.
			//Reselect the treatment plan that the user was just looking at.
			gridPlans.SetSelected(_listTreatPlans.IndexOf(_listTreatPlans.FirstOrDefault(x => x.TreatPlanNum==treatPlanSelected.TreatPlanNum)),true);
			//Refresh the main grid if a saved TP is selected.
			if(gridPlans.GetSelectedIndex() > 0) {
				FillMain();//Refresh the Procedures grid with the newly selected treatment plan.
			}
			//Reselect any procedures that were selected prior to setting the priority.
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag!=null && listProcNumsSelected.Contains(((ProcTP)gridMain.ListGridRows[i].Tag).ProcNumOrig)) {
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
				if(!_checkShowInsNotAutomatic){
					checkShowIns.Checked=true;
				}
				if(!_checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
				checkShowSubtotals.Checked=true;
				checkShowTotals.Checked=true;
				FillMain();
				return;
			}
			//checkShowStandard.Checked=false;
			if(!_checkShowInsNotAutomatic){
				checkShowIns.Checked=false;
			}
			if(!_checkShowDiscountNotAutomatic) {
				checkShowDiscount.Checked=false;
			}
			checkShowSubtotals.Checked=false;
			checkShowTotals.Checked=false;
			FillMain();
		}

		private void checkShowStandard_Click(object sender,EventArgs e) {
			FillMain();
		}

		private void checkShowIns_Click(object sender,EventArgs e) {
			if(!checkShowIns.Checked && !_checkShowInsNotAutomatic) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Turn off automatic checking of this box for Active/Inactive Treatment Plans for the rest of this session?")) {
					_checkShowInsNotAutomatic=true;
				}
			}
			FillMain();
		}

		private void checkShowDiscount_Click(object sender,EventArgs e) {
			if(!checkShowDiscount.Checked && !_checkShowDiscountNotAutomatic) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Turn off automatic checking of this box for Active/Inactive Treatment Plans for the rest of this session?")) {
					_checkShowDiscountNotAutomatic=true;
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
				string errMsg=Documents.OpenDoc(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum);
				Cursor=Cursors.Default;
				if(errMsg!="") {
					MsgBox.Show(errMsg);
					return;
				}
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
				treatPlan.ListProcTPs=LoadTP(treatPlan);
			}
			if(DoPrintUsingSheets()) {
				sheetTP=TreatPlanToSheet(treatPlan);
				SheetPrinting.Print(sheetTP);
			}
			else { //clasic TPs
				PrepImageForPrinting();
				MigraDoc.DocumentObjectModel.Document document=CreateDocument();
				MigraDoc.Rendering.Printing.MigraDocPrintDocument migraDocPrintDocument=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
				MigraDoc.Rendering.DocumentRenderer documentRenderer=new MigraDoc.Rendering.DocumentRenderer(document);
				documentRenderer.PrepareDocument();
				migraDocPrintDocument.Renderer=documentRenderer;
				//we might want to surround some of this with a try-catch
				//TODO: Implement ODprintout pattern - MigraDoc
				if(ODBuild.IsDebug()) {
					using FormRpPrintPreview formRpPrintPreview=new FormRpPrintPreview(migraDocPrintDocument);
					formRpPrintPreview.ShowDialog();
				}
				else {
					if(PrinterL.SetPrinter(pd2,PrintSituation.TPPerio,PatientCur.PatNum,"Treatment plan for printed")){
						migraDocPrintDocument.PrinterSettings=pd2.PrinterSettings;
						migraDocPrintDocument.Print();
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
			Random random=new Random();
			string fileName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+random.Next(1000).ToString()+".pdf";
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
					treatPlan.ListProcTPs=LoadTP(treatPlan);
				}
				Sheet sheetTP=TreatPlanToSheet(treatPlan);
				SheetPrinting.CreatePdf(sheetTP,filePathAndName,null);
				SaveTPAsDocument(isSigSave:false,sheetTP);
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
			EmailMessage emailMessage=new EmailMessage();
			emailMessage.PatNum=PatientCur.PatNum;
			emailMessage.ToAddress=PatientCur.Email;
			EmailAddress emailAddress=EmailAddresses.GetByClinic(PatientCur.ClinicNum);
			emailMessage.FromAddress=emailAddress.GetFrom();
			emailMessage.Subject=Lan.g(this,"Treatment Plan");
			EmailAttach emailAttach=new EmailAttach();
			emailAttach.DisplayedFileName="TreatmentPlan.pdf";
			emailAttach.ActualFileName=fileName;
			emailMessage.Attachments.Add(emailAttach);
			emailMessage.MsgType=EmailMessageSource.TreatmentPlan;
			using FormEmailMessageEdit formEmailMessageEdit=new FormEmailMessageEdit(emailMessage,emailAddress);
			formEmailMessageEdit.IsNew=true;
			formEmailMessageEdit.ShowDialog();
			//if(FormE.DialogResult==DialogResult.OK) {
			//	RefreshCurrentModule();
			//}
		}

		private void FormSheetFillEdit_FormClosing(object sender,FormClosingEventArgs e) {
			FormSheetFillEdit formSheetFillEdit=((FormSheetFillEdit)sender);
			if(formSheetFillEdit.DialogResult==DialogResult.OK && PatientCur!=null) {
				//If the user deleted the sheet, forcefully refresh the chart module regardless of what patient is selected.
				//Otherwise; only refresh the chart module if the same patient is selected.
				if(formSheetFillEdit.SheetCur==null || formSheetFillEdit.SheetCur.PatNum==PatientCur.PatNum) {
					ModuleSelected(PatientCur.PatNum);
				}
			}
		}

		private void ToolBarConsent_Click() {
			if(PatientCur==null) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			List<SheetDef> listSheetDefs=SheetDefs.GetCustomForType(SheetTypeEnum.Consent);
			if(listSheetDefs.Count>0) {
				MsgBox.Show(this,"Please use dropdown list.");
				return;
			}
			SheetDef sheetDef=SheetsInternal.GetSheetDef(SheetInternalType.Consent);
			Sheet sheet=SheetUtil.CreateSheet(sheetDef,PatientCur.PatNum);
			SheetParameter.SetParameter(sheet,"PatNum",PatientCur.PatNum);
			StaticTextData staticTextData=new StaticTextData();
			List<GridRow> listGridRowsProcs=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();//Only pull selected rows that are procedures
			if(listGridRowsProcs.Count()>0) {//loop through selected procedures
				StaticTextFieldDependency staticTextFieldDependency=StaticTextData.GetStaticTextDependencies(sheet.SheetFields);
				List<long> listProcNums=listGridRowsProcs.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcedures=Procedures.GetManyProc(listProcNums,false);//get list of procedures from list of procedureNums
				List<long> listCodeNums=listProcedures.Select(x => x.CodeNum).ToList();
				if(listCodeNums.Count()>0) {
					staticTextFieldDependency|=StaticTextFieldDependency.ListSelectedTpProcs;
				}
				staticTextData=StaticTextData.GetStaticTextData(staticTextFieldDependency,PatientCur,_family,listCodeNums);//set static text using selected procedure CodeNums
			}
			SheetFiller.FillFields(sheet,staticTextData: staticTextData);
			SheetUtil.CalculateHeights(sheet);
			FormSheetFillEdit.ShowForm(sheet,FormSheetFillEdit_FormClosing);
		}

		private void ToolBarLabCase_Click() {
			if(PatientCur==null) {
				MsgBox.Show(this,"Please select a patient.");
				return;
			}
			LabCase labCase=new LabCase();
			labCase.PatNum=PatientCur.PatNum;
			labCase.ProvNum=Patients.GetProvNum(PatientCur);
			labCase.DateTimeCreated=MiscData.GetNowDateTime();
			LabCases.Insert(labCase);//it will be deleted inside the form if user clicks cancel.
									 //We need the primary key in order to attach lab slip.
			List<long> listCodeNums=new List<long>();
			List<GridRow> listRowsProcTPs=gridMain.SelectedGridRows.Where(x => x.Tag!=null).ToList();
			if(listRowsProcTPs.Count()>0) {//loop through selected procedures
				List<long> listProcNums=listRowsProcTPs.Select(x => ((ProcTP)x.Tag).ProcNumOrig).ToList();
				List<Procedure> listProcedures=Procedures.GetManyProc(listProcNums,false);//get list of procedures from list of procedureNums
				listCodeNums=listProcedures.Select(x => x.CodeNum).ToList();
			}
			using FormLabCaseEdit formLabCaseEdit=new FormLabCaseEdit();
			formLabCaseEdit.LabCaseCur=labCase;
			formLabCaseEdit.ListProcCodeNums=listCodeNums;//set list of procedure CodeNums in FormLabCaseEdit
			formLabCaseEdit.IsNew=true;
			formLabCaseEdit.ShowDialog();
			//needs to always refresh due to complex ok/cancel
			ModuleSelected(PatientCur.PatNum);
		}

		private void PrepImageForPrinting(){
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)){
				return;
			}
			_toothChartWrapper=new SparksToothChart.ToothChartWrapper();
			_toothChartRelay= new ToothChartRelay(false);
			_toothChartRelay.SetToothChartWrapper(_toothChartWrapper);
			if(ToothChartRelay.IsSparks3DPresent){
				_controlToothChart=_toothChartRelay.GetToothChart();
				_controlToothChart.Location=new Point(0,0);
				_controlToothChart.Size=new Size(500,370);
				_controlToothChart.Visible=true;
				//this.Controls.Add(toothChart);
				//toothChart.BringToFront();
			}
			else{
				_toothChartWrapper.Size=new Size(500,370);
				_toothChartWrapper.UseHardware=ComputerPrefs.LocalComputer.GraphicsUseHardware;
				_toothChartWrapper.PreferredPixelFormatNumber=ComputerPrefs.LocalComputer.PreferredPixelFormatNum;
				_toothChartWrapper.DeviceFormat=new ToothChartDirectX.DirectXDeviceFormat(ComputerPrefs.LocalComputer.DirectXFormat);
				//Must be last setting set for preferences, because this is the line where the device pixel format is recreated.
				//The preferred pixel format number changes to the selected pixel format number after a context is chosen.
				_toothChartWrapper.DrawMode=ComputerPrefs.LocalComputer.GraphicsSimple;
				ComputerPrefs.LocalComputer.PreferredPixelFormatNum=_toothChartWrapper.PreferredPixelFormatNumber;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
				this.Controls.Add(_toothChartWrapper);
				_toothChartWrapper.BringToFront();
			}
			_toothChartRelay.SetToothNumberingNomenclature((ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers));
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors);
			_toothChartRelay.ColorBackgroundMain=listDefs[14].ItemColor;
			_toothChartRelay.ColorText=listDefs[15].ItemColor;
			_toothChartRelay.ResetTeeth();
			_toothChartRelay.SetOrthoMode(false);
			_listToothInitials=ToothInitials.GetPatientData(PatientCur.PatNum);
			//first, primary.  That way, you can still set a primary tooth missing afterwards.
			for(int i=0;i<_listToothInitials.Count;i++) {
				if(_listToothInitials[i].InitialType==ToothInitialType.Primary) {
					_toothChartRelay.SetPrimary(_listToothInitials[i].ToothNum);
				}
			}
			for(int i=0;i<_listToothInitials.Count;i++) {
				switch(_listToothInitials[i].InitialType) {
					case ToothInitialType.Missing:
						_toothChartRelay.SetMissing(_listToothInitials[i].ToothNum);
						break;
					case ToothInitialType.Hidden:
						_toothChartRelay.SetHidden(_listToothInitials[i].ToothNum);
						break;
					case ToothInitialType.Rotate:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,_listToothInitials[i].Movement,0,0,0,0,0);
						break;
					case ToothInitialType.TipM:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,0,_listToothInitials[i].Movement,0,0,0,0);
						break;
					case ToothInitialType.TipB:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,0,0,_listToothInitials[i].Movement,0,0,0);
						break;
					case ToothInitialType.ShiftM:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,0,0,0,_listToothInitials[i].Movement,0,0);
						break;
					case ToothInitialType.ShiftO:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,0,0,0,0,_listToothInitials[i].Movement,0);
						break;
					case ToothInitialType.ShiftB:
						_toothChartRelay.MoveTooth(_listToothInitials[i].ToothNum,0,0,0,0,0,_listToothInitials[i].Movement);
						break;
					case ToothInitialType.Drawing:
						_toothChartRelay.AddDrawingSegment(_listToothInitials[i].Copy());
						break;
					case ToothInitialType.Text:
						_toothChartRelay.AddText(_listToothInitials[i].GetTextString(), _listToothInitials[i].GetTextPoint(), _listToothInitials[i].ColorDraw, _listToothInitials[i].ToothInitialNum);
						break;
				}
			}
			ComputeProcListFiltered();
			DrawProcsGraphics();
			DrawOrthoHardware();
			if(!ToothChartRelay.IsSparks3DPresent){
				_toothChartWrapper.AutoFinish=true;
			}
			_toothChartRelay.EndUpdate();
			_bitmapToothChart=_toothChartRelay.GetBitmap();
			if(ToothChartRelay.IsSparks3DPresent){
				Controls.Remove(_controlToothChart);
			}
			else{
				Controls.Remove(_toothChartWrapper);
			}
			_toothChartRelay.DisposeControl();
		}

		private List<ProcTP> LoadTP(TreatPlan treatPlan) {
			_loadActiveTPData=TreatmentPlanModules.GetLoadActiveTpData(PatientCur,treatPlan.TreatPlanNum,_listBenefits,_listPatPlans,
				_listInsPlans,dateTimeTP.Value,_listInsSubs,PrefC.GetBool(PrefName.InsChecksFrequency),PrefC.IsTreatPlanSortByTooth,_listSubstitutionLinks);
			if(_loadActiveTPData.listProcForTP.Any(x => ProcedureCodes.GetWhereFromList(y => y.CodeNum==x.CodeNum).Count==0)) {
				MsgBox.Show(this,$"Missing codenum. Please run database maintenance method {nameof(DatabaseMaintenances.ProcedurelogCodeNumInvalid)}.");
				return new List<ProcTP>();//Show an empty TP
			}
			if(PrefC.GetBool(PrefName.InsChecksFrequency)) {
				//Taking into account insurance frequency, use the date picker date when loading or when the Refresh button is pressed.  Defaults to today.
				_listClaimProcHists=_loadActiveTPData.HistList??ClaimProcs.GetHistList(PatientCur.PatNum,_listBenefits,_listPatPlans,_listInsPlans,-1,dateTimeTP.Value,_listInsSubs);
			}
			_listClaimProcs=_loadActiveTPData.ClaimProcList;
			//Get the list of TPRows.
			_listTpRowsMain.Clear();
			_listTpRowsMain=TreatmentPlanModules.GetActiveTpPlanTpRows(checkShowMaxDed.Checked,checkShowDiscount.Checked,checkShowSubtotals.Checked,checkShowTotals.Checked,treatPlan,PatientCur,dateTimeTP.Value,_loadActiveTPData,_listInsPlans,_listBenefits,
				_listPatPlans,_listSubstitutionLinks,_listInsSubs,_tpModuleData.DiscountPlanSub,_tpModuleData.DiscountPlan,_listProcedures,ref _listClaimProcs,_listClaimProcHists);
			//Disable the discount check box if there is no discount
			if(checkShowDiscount.Checked && CompareDecimal.IsZero(_listTpRowsMain.FirstOrDefault(x=>x.RowType==TpRowType.Total)?.Discount??0)) { //mimics Saved TP logic
				checkShowDiscount.Checked=false;
			}			
			//Change the note
			textNote.Text=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			HasNoteChanged=false;
			//If the TP is saved and signed or unassigned and inactive, set the note to read only.
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
			return ProcTPs.GetProcTPsFromTpRows(PatientCur.PatNum,_listTpRowsMain.FindAll(x=>x.RowType==TpRowType.TpRow),_loadActiveTPData.listProcForTP,_loadActiveTPData.ListTreatPlanAttaches);
		}

		private void LoadArchivedTP(TreatPlan treatPlan) {
			_listProcTPsSelect=_listProcTPs.FindAll(x => x.TreatPlanNum==_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum);
			_listTpRowsMain=TreatmentPlanModules.GetArchivedTpRows(checkShowSubtotals.Checked,checkShowTotals.Checked,treatPlan,_listProcTPsSelect,_listProcedures);
            if(checkShowDiscount.Checked && CompareDecimal.IsZero(_listTpRowsMain.Sum(x=>x.Discount))) { //mimics Active/Inactive TP logic
                    checkShowDiscount.Checked=false;
            }
            textNote.Text=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
            HasNoteChanged=false;
            if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Saved
                    && _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="")
            {
                    textNote.ReadOnly=true;
            }
            else if(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus==TreatPlanStatus.Inactive
                    && _listTreatPlans[gridPlans.SelectedIndices[0]].Heading==Lan.g("TreatPlan","Unassigned"))
            {
                    textNote.ReadOnly=true;
            }
            else {
                    textNote.ReadOnly=false;
            }
		}

		/// <summary>Returns in-memory TreatPlan representing the current treatplan. For displaying current treat-plan before saving it.</summary>
		private TreatPlan GetCurrentTPHelper() {
			TreatPlan treatPlanRetVal=new TreatPlan();
			treatPlanRetVal.Heading=Lan.g(this,"Proposed Treatment Plan");
			treatPlanRetVal.DateTP=DateTime.Today;
			treatPlanRetVal.PatNum=PatientCur.PatNum;
			treatPlanRetVal.Note=PrefC.GetString(PrefName.TreatmentPlanNote);
			treatPlanRetVal.ListProcTPs=new List<ProcTP>();
			ProcTP procTP;
			Procedure procedure;
			int itemNo=0;
			List<Procedure> listProcedures=new List<Procedure>();
			if(gridMain.SelectedIndices.Length==0 || gridMain.SelectedIndices.All(x=>gridMain.ListGridRows[x].Tag==null)) {
				gridMain.SetAll(true);//either no rows selected, or only total rows selected.
			}
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				if(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag==null) {
					//user must have highlighted a subtotal row.
					continue;
				}
				procedure=(Procedure)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag;
				listProcedures.Add(procedure);
				procTP=new ProcTP();
				//procTP.TreatPlanNum=tp.TreatPlanNum;
				procTP.PatNum=PatientCur.PatNum;
				procTP.ProcNumOrig=procedure.ProcNum;
				procTP.ItemOrder=itemNo;
				procTP.Priority=procedure.Priority;
				procTP.ToothNumTP=Tooth.Display(procedure.ToothNum);
				if(ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea==TreatmentArea.Surf) {
					procTP.Surf=Tooth.SurfTidyFromDbToDisplay(procedure.Surf,procedure.ToothNum);
				}
				else {
					procTP.Surf=procedure.Surf;//for UR, L, etc.
				}
				procTP.ProcCode=ProcedureCodes.GetStringProcCode(procedure.CodeNum);
				procTP.Descript=_listTpRowsMain[gridMain.SelectedIndices[i]].Description;
				if(checkShowFees.Checked) {
					procTP.FeeAmt=PIn.Double(_listTpRowsMain[gridMain.SelectedIndices[i]].Fee.ToString());
				}
				if(checkShowIns.Checked) {
					procTP.PriInsAmt=PIn.Double(_listTpRowsMain[gridMain.SelectedIndices[i]].PriIns.ToString());
					procTP.SecInsAmt=PIn.Double(_listTpRowsMain[gridMain.SelectedIndices[i]].SecIns.ToString());
				}
				if(checkShowDiscount.Checked) {
					procTP.Discount=PIn.Double(_listTpRowsMain[gridMain.SelectedIndices[i]].Discount.ToString());
				}
				procTP.PatAmt=PIn.Double(_listTpRowsMain[gridMain.SelectedIndices[i]].Pat.ToString());
				procTP.Prognosis=_listTpRowsMain[gridMain.SelectedIndices[i]].Prognosis;
				procTP.Dx=_listTpRowsMain[gridMain.SelectedIndices[i]].Dx;
				treatPlanRetVal.ListProcTPs.Add(procTP);
				//ProcTPs.InsertOrUpdate(procTP,true);
				itemNo++;
			}
			return treatPlanRetVal;
		}

		///<summary>Simply creates a new sheet from a given treatment plan and adds parameters to the sheet based on which checkboxes are checked.</summary>
		private Sheet TreatPlanToSheet(TreatPlan treatPlan) {
			Sheet sheetTP=SheetUtil.CreateSheet(SheetDefs.GetSheetsDefault(SheetTypeEnum.TreatmentPlan,Clinics.ClinicNum),PatientCur.PatNum);
			sheetTP.Parameters.Add(new SheetParameter(true,"TreatPlan") { ParamValue=treatPlan });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscountNotAutomatic") { ParamValue=_checkShowDiscountNotAutomatic });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowDiscount") { ParamValue=checkShowDiscount.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowMaxDed") { ParamValue=checkShowMaxDed.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowSubTotals") { ParamValue=checkShowSubtotals.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowTotals") { ParamValue=checkShowTotals.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowCompleted") { ParamValue=checkShowCompleted.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowFees") { ParamValue=checkShowFees.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"checkShowIns") { ParamValue=checkShowIns.Checked });
			sheetTP.Parameters.Add(new SheetParameter(true,"toothChartImg") { ParamValue=SheetPrinting.GetToothChartHelper(PatientCur.PatNum,checkShowCompleted.Checked,treatPlan) });
			//FormSheetFillEdit FormSFE=new FormSheetFillEdit(sheetTP);
			SheetFiller.FillFields(sheetTP);
			SheetUtil.CalculateHeights(sheetTP);
			return sheetTP;
		}

		private MigraDoc.DocumentObjectModel.Document CreateDocument(){
			MigraDoc.DocumentObjectModel.Document document=new MigraDoc.DocumentObjectModel.Document();
			document.DefaultPageSetup.PageWidth=Unit.FromInch(8.5);
			document.DefaultPageSetup.PageHeight=Unit.FromInch(11);
			document.DefaultPageSetup.TopMargin=Unit.FromInch(.5);
			document.DefaultPageSetup.LeftMargin=Unit.FromInch(.5);
			document.DefaultPageSetup.RightMargin=Unit.FromInch(.5);
			MigraDoc.DocumentObjectModel.Section section=document.AddSection();
			string text;
			MigraDoc.DocumentObjectModel.Font headingFont=MigraDocHelper.CreateFont(13,true);
			MigraDoc.DocumentObjectModel.Font bodyFontx=MigraDocHelper.CreateFont(9,false);
			MigraDoc.DocumentObjectModel.Font nameFontx=MigraDocHelper.CreateFont(9,true);
			MigraDoc.DocumentObjectModel.Font totalFontx=MigraDocHelper.CreateFont(9,true);
			//Heading---------------------------------------------------------------------------------------------------------------
			#region printHeading
			Paragraph paragraph=section.AddParagraph();
			ParagraphFormat paragraphFormat=new ParagraphFormat();
			paragraphFormat.Alignment=ParagraphAlignment.Center;
			paragraphFormat.Font=MigraDocHelper.CreateFont(10,true);
			paragraph.Format=paragraphFormat;
			text=_listTreatPlans[gridPlans.SelectedIndices[0]].Heading;
			paragraph.AddFormattedText(text,headingFont);
			paragraph.AddLineBreak();
			if(PatientCur.ClinicNum==0 || !PrefC.HasClinicsEnabled) {
				text=PrefC.GetString(PrefName.PracticeTitle);
				paragraph.AddText(text);
				paragraph.AddLineBreak();
				text=PrefC.GetString(PrefName.PracticePhone);
			}
			else {
				Clinic clinic=Clinics.GetClinic(PatientCur.ClinicNum);
				text=clinic.Description;
				paragraph.AddText(text);
				paragraph.AddLineBreak();
				text=clinic.Phone;
			}
			if(text.Length==10) {
				text=TelephoneNumbers.ReFormat(text);
			}
			paragraph.AddText(text);
			paragraph.AddLineBreak();
			text=PatientCur.GetNameFLFormal()+", DOB "+PatientCur.Birthdate.ToShortDateString();
			paragraph.AddText(text);
			paragraph.AddLineBreak();
			if(gridPlans.SelectedIndices[0]>0){//not the default plan
				if(_listTreatPlans[gridPlans.SelectedIndices[0]].ResponsParty!=0){
					text=Lan.g(this,"Responsible Party: ")
						+Patients.GetLim(_listTreatPlans[gridPlans.SelectedIndices[0]].ResponsParty).GetNameFL();
					paragraph.AddText(text);
					paragraph.AddLineBreak();
				}
			}
			if(new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {//Active/Inactive TP
				text=DateTime.Today.ToShortDateString();
			}
			else {
				text=_listTreatPlans[gridPlans.SelectedIndices[0]].DateTP.ToShortDateString();
			}
			paragraph.AddText(text);
			#endregion
			//Graphics---------------------------------------------------------------------------------------------------------------
			#region PrintGraphics
			TextFrame textFrame;
			int widthDoc=MigraDocHelper.GetDocWidth();
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum))
			{	
				textFrame=MigraDocHelper.CreateContainer(section);
				MigraDocHelper.DrawString(textFrame,Lan.g(this,"Your")+"\r\n"+Lan.g(this,"Right"),bodyFontx,
					new RectangleF(widthDoc/2-_toothChartRelay.Width/2-50,_toothChartRelay.Height/2-10,50,100));
				MigraDocHelper.DrawBitmap(textFrame,_bitmapToothChart,widthDoc/2-_toothChartRelay.Width/2,0);
				MigraDocHelper.DrawString(textFrame,Lan.g(this,"Your")+"\r\n"+Lan.g(this,"Left"),bodyFontx,
					new RectangleF(widthDoc/2+_toothChartRelay.Width/2+17,_toothChartRelay.Height/2-10,50,100));
				if(checkShowCompleted.Checked) {
					List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
					float yPos=_toothChartRelay.Height+15;
					float xPos=225;
					MigraDocHelper.FillRectangle(textFrame,listDefs[3].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(textFrame,Lan.g(this,"Existing"),bodyFontx,xPos,yPos);
					Graphics g=this.CreateGraphics();//for measuring strings.
					xPos+=(int)g.MeasureString(Lan.g(this,"Existing"),_fontBody).Width+23;
					//The Complete work is actually a combination of EC and C. Usually same color.
					//But just in case they are different, this will show it.
					MigraDocHelper.FillRectangle(textFrame,listDefs[2].ItemColor,xPos,yPos,7,14);
					xPos+=7;
					MigraDocHelper.FillRectangle(textFrame,listDefs[1].ItemColor,xPos,yPos,7,14);
					xPos+=9;
					MigraDocHelper.DrawString(textFrame,Lan.g(this,"Complete"),bodyFontx,xPos,yPos);
					xPos+=(int)g.MeasureString(Lan.g(this,"Complete"),_fontBody).Width+23;
					MigraDocHelper.FillRectangle(textFrame,listDefs[4].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(textFrame,Lan.g(this,"Referred Out"),bodyFontx,xPos,yPos);
					xPos+=(int)g.MeasureString(Lan.g(this,"Referred Out"),_fontBody).Width+23;
					MigraDocHelper.FillRectangle(textFrame,listDefs[0].ItemColor,xPos,yPos,14,14);
					xPos+=16;
					MigraDocHelper.DrawString(textFrame,Lan.g(this,"Treatment Planned"),bodyFontx,xPos,yPos);
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
				gridFamIns.Columns.Clear();
				GridColumn col=new GridColumn("",140);
				gridFamIns.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Primary"),70,HorizontalAlignment.Right);
				gridFamIns.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Secondary"),70,HorizontalAlignment.Right);
				gridFamIns.Columns.Add(col);
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
				paragraph=section.AddParagraph();
				paragraph.Format.Alignment=ParagraphAlignment.Center;
				paragraph.AddFormattedText(Lan.g(this,"Family Insurance Benefits"),totalFontx);
				MigraDocHelper.InsertSpacer(section,2);
				MigraDocHelper.DrawGrid(section,gridFamIns);
				gridFamIns.Dispose();
				//Individual Insurance---------------------
				GridOD gridIns=new GridOD();
				gridIns.TranslationName="";
				this.Controls.Add(gridIns);
				gridIns.BeginUpdate();
				gridIns.Columns.Clear();
				col=new GridColumn("",140);
				gridIns.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Primary"),70,HorizontalAlignment.Right);
				gridIns.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Secondary"),70,HorizontalAlignment.Right);
				gridIns.Columns.Add(col);
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
				paragraph=section.AddParagraph();
				paragraph.Format.Alignment=ParagraphAlignment.Center;
				paragraph.AddFormattedText(Lan.g(this,"Individual Insurance Benefits"),totalFontx);
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
				paragraph=section.AddParagraph(note);
				paragraph.Format.Font=bodyFontx;
				paragraph.Format.Borders.Color=Colors.Gray;
				paragraph.Format.Borders.DistanceFromLeft=Unit.FromInch(.05);
				paragraph.Format.Borders.DistanceFromRight=Unit.FromInch(.05);
				paragraph.Format.Borders.DistanceFromTop=Unit.FromInch(.05);
				paragraph.Format.Borders.DistanceFromBottom=Unit.FromInch(.05);
			}
			#endregion
			//Signature-----------------------------------------------------------------------------------------------------------
			#region signature
			if(gridPlans.SelectedIndices[0]!=0//can't be default TP
				&& _listTreatPlans[gridPlans.SelectedIndices[0]].Signature!="")
			{
				TreatPlan treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]];
				List<ProcTP> listProcTPs=ProcTPs.RefreshForTP(_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum);
				System.Drawing.Bitmap sigBitmap=null;
				SignatureBoxWrapper signatureBoxWrapper=new SignatureBoxWrapper();
				signatureBoxWrapper.SignatureMode=SignatureBoxWrapper.SigMode.TreatPlan;
				string keyData=TreatPlans.GetKeyDataForSignatureHash(treatPlan,listProcTPs);
				signatureBoxWrapper.FillSignature(treatPlan.SigIsTopaz,keyData,treatPlan.Signature);
				sigBitmap=signatureBoxWrapper.GetSigImage();  //Previous tp code did not care if signature is valid or not.
				if(sigBitmap!=null) { 
					textFrame=MigraDocHelper.CreateContainer(section);
					MigraDocHelper.DrawBitmap(textFrame,sigBitmap,widthDoc/2-sigBitmap.Width/2,20);
				}
			}
			#endregion
			return document;
		}

		///<summary>Just used for printing the 3D chart.</summary>
		private void ComputeProcListFiltered() {
			_listProceduresFiltered=new List<Procedure>();
			//first, add all completed work and conditions. C,EC,EO, and Referred
			for(int i=0;i<_listProcedures.Count;i++) {
				if(_listProcedures[i].ProcStatus==ProcStat.C
					|| _listProcedures[i].ProcStatus==ProcStat.EC
					|| _listProcedures[i].ProcStatus==ProcStat.EO) 
				{
					if(checkShowCompleted.Checked) {
						_listProceduresFiltered.Add(_listProcedures[i]);
					}
				}
				if(_listProcedures[i].ProcStatus==ProcStat.R) { //always show all referred
					_listProceduresFiltered.Add(_listProcedures[i]);
				}
				if(_listProcedures[i].ProcStatus==ProcStat.Cn) { //always show all conditions.
					_listProceduresFiltered.Add(_listProcedures[i]);
				}
			}
			//then add whatever is showing on the selected TP
			//Always select all procedures in TP.
			gridMain.SetAll(true);
			if(new[] {TreatPlanStatus.Active,TreatPlanStatus.Inactive}.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) { //current plan
				_listProcTPsSelect=gridMain.SelectedIndices.Where(x => gridMain.ListGridRows[x].Tag!=null).Select(x => (ProcTP)gridMain.ListGridRows[x].Tag).ToList();
			}
			Procedure procedureDummy; //not a real procedure.  Just used to help display on graphical chart
			for(int i=0;i<_listProcTPsSelect.Count;i++) {
				procedureDummy=new Procedure();
				//this next loop is a way to get missing fields like tooth range.  Could be improved.
				for(int j=0;j<_listProcedures.Count;j++) {
					if(_listProcedures[j].ProcNum==_listProcTPsSelect[i].ProcNumOrig) {
						//but remember that even if the procedure is found, Status might have been altered
						procedureDummy=_listProcedures[j].Copy();
					}
				}
				if(Tooth.IsValidEntry(_listProcTPsSelect[i].ToothNumTP)) {
					procedureDummy.ToothNum=Tooth.Parse(_listProcTPsSelect[i].ToothNumTP);
				}
				if(ProcedureCodes.GetProcCode(_listProcTPsSelect[i].ProcCode).TreatArea==TreatmentArea.Surf) {
					procedureDummy.Surf=Tooth.SurfTidyFromDisplayToDb(_listProcTPsSelect[i].Surf,procedureDummy.ToothNum);
				}
				else {
					procedureDummy.Surf=_listProcTPsSelect[i].Surf; //for quad, arch, etc.
				}
				if(procedureDummy.ToothRange==null) {
					procedureDummy.ToothRange="";
				}
				//procDummy.HideGraphical??
				procedureDummy.ProcStatus=ProcStat.TP;
				procedureDummy.CodeNum=ProcedureCodes.GetProcCode(_listProcTPsSelect[i].ProcCode).CodeNum;
				_listProceduresFiltered.Add(procedureDummy);
			}
			_listProceduresFiltered.Sort(CompareProcListFiltered);
		}

		private int CompareProcListFiltered(Procedure procedure1,Procedure procedure2) {
			int dateFilter=procedure1.ProcDate.CompareTo(procedure2.ProcDate);
			if(dateFilter!=0) {
				return dateFilter;
			}
			//Dates are the same, filter by ProcStatus.
			int xIdx=GetProcStatusIdx(procedure1.ProcStatus);
			int yIdx=GetProcStatusIdx(procedure2.ProcStatus);
			return xIdx.CompareTo(yIdx);
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
			Procedure procedure;
			string[] stringArrayTeeth;
			System.Drawing.Color cLight=System.Drawing.Color.White;
			System.Drawing.Color cDark=System.Drawing.Color.White;
			List<Def> listDefs=Defs.GetDefsForCategory(DefCat.ChartGraphicColors,true);
			for(int i=0;i<_listProceduresFiltered.Count;i++) {
				procedure=_listProceduresFiltered[i];
				//if(proc.ProcStatus!=procStat) {
				//  continue;
				//}
				if(procedure.HideGraphics) {
					continue;
				}
				if(ProcedureCodes.GetProcCode(procedure.CodeNum).PaintType==ToothPaintingType.Extraction && (
					procedure.ProcStatus==ProcStat.C
					|| procedure.ProcStatus==ProcStat.EC
					|| procedure.ProcStatus==ProcStat.EO
					)) {
					continue;//prevents the red X. Missing teeth already handled.
				}
				if(ProcedureCodes.GetProcCode(procedure.CodeNum).GraphicColor==System.Drawing.Color.FromArgb(0)) {
					switch(procedure.ProcStatus) {
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
					cDark=ProcedureCodes.GetProcCode(procedure.CodeNum).GraphicColor;
					cLight=ProcedureCodes.GetProcCode(procedure.CodeNum).GraphicColor;
				}
				switch(ProcedureCodes.GetProcCode(procedure.CodeNum).PaintType) {
					case ToothPaintingType.BridgeDark:
						if(ToothInitials.ToothIsMissingOrHidden(_listToothInitials,procedure.ToothNum)) {
							_toothChartRelay.SetPontic(procedure.ToothNum,cDark);
						}
						else {
							_toothChartRelay.SetCrown(procedure.ToothNum,cDark);
						}
						break;
					case ToothPaintingType.BridgeLight:
						if(ToothInitials.ToothIsMissingOrHidden(_listToothInitials,procedure.ToothNum)) {
							_toothChartRelay.SetPontic(procedure.ToothNum,cLight);
						}
						else {
							_toothChartRelay.SetCrown(procedure.ToothNum,cLight);
						}
						break;
					case ToothPaintingType.CrownDark:
						_toothChartRelay.SetCrown(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.CrownLight:
						_toothChartRelay.SetCrown(procedure.ToothNum,cLight);
						break;
					case ToothPaintingType.DentureDark:
						if(procedure.Surf=="U") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(procedure.Surf=="L") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							stringArrayTeeth=procedure.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<stringArrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(_listToothInitials,stringArrayTeeth[t])) {
								_toothChartRelay.SetPontic(stringArrayTeeth[t],cDark);
							}
							else {
								_toothChartRelay.SetCrown(stringArrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.DentureLight:
						if(procedure.Surf=="U") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+2).ToString();
							}
						}
						else if(procedure.Surf=="L") {
							stringArrayTeeth=new string[14];
							for(int t=0;t<14;t++) {
								stringArrayTeeth[t]=(t+18).ToString();
							}
						}
						else {
							stringArrayTeeth=procedure.ToothRange.Split(new char[] { ',' });
						}
						for(int t=0;t<stringArrayTeeth.Length;t++) {
							if(ToothInitials.ToothIsMissingOrHidden(_listToothInitials,stringArrayTeeth[t])) {
								_toothChartRelay.SetPontic(stringArrayTeeth[t],cLight);
							}
							else {
								_toothChartRelay.SetCrown(stringArrayTeeth[t],cLight);
							}
						}
						break;
					case ToothPaintingType.Extraction:
						_toothChartRelay.SetBigX(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.FillingDark:
						_toothChartRelay.SetSurfaceColors(procedure.ToothNum,procedure.Surf,cDark);
						break;
					case ToothPaintingType.FillingLight:
						_toothChartRelay.SetSurfaceColors(procedure.ToothNum,procedure.Surf,cLight);
						break;
					case ToothPaintingType.Implant:
						_toothChartRelay.SetImplant(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.PostBU:
						_toothChartRelay.SetBU(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.RCT:
						_toothChartRelay.SetRCT(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.RetainedRoot:
						_toothChartRelay.SetRetainedRoot(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.Sealant:
						_toothChartRelay.SetSealant(procedure.ToothNum,cDark);
						break;
					case ToothPaintingType.SpaceMaintainer:
						if(ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea==TreatmentArea.Tooth && procedure.ToothNum!=""){
							_toothChartRelay.SetSpaceMaintainer(procedure.ToothNum,cDark);
						}
						else if((ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea==TreatmentArea.ToothRange && procedure.ToothRange!="") 
							|| (ProcedureCodes.GetProcCode(procedure.CodeNum).AreaAlsoToothRange && procedure.ToothRange!=""))
						{
							stringArrayTeeth=procedure.ToothRange.Split(',');
							for(int t=0;t<stringArrayTeeth.Length;t++) {
								_toothChartRelay.SetSpaceMaintainer(stringArrayTeeth[t],cDark);
							}
						}
						else if(ProcedureCodes.GetProcCode(procedure.CodeNum).TreatArea==TreatmentArea.Quad){
							stringArrayTeeth=new string[0];
							if(procedure.Surf=="UR") {
								stringArrayTeeth=new string[] {"4","5","6","7","8"};
							}
							if(procedure.Surf=="UL") {
								stringArrayTeeth=new string[] {"9","10","11","12","13"};
							}
							if(procedure.Surf=="LL") {
								stringArrayTeeth=new string[] {"20","21","22","23","24"};
							}
							if(procedure.Surf=="LR") {
								stringArrayTeeth=new string[] {"25","26","27","28","29"};
							}							
							for(int t=0;t<stringArrayTeeth.Length;t++) {//could still be length 0
								_toothChartRelay.SetSpaceMaintainer(stringArrayTeeth[t],cDark);
							}
						}
						break;
					case ToothPaintingType.Text:
						_toothChartRelay.SetText(procedure.ToothNum,cDark,ProcedureCodes.GetProcCode(procedure.CodeNum).PaintText);
						break;
					case ToothPaintingType.Veneer:
						_toothChartRelay.SetVeneer(procedure.ToothNum,cLight);
						break;
				}
			}
		}

		private void DrawOrthoHardware(){
			List<OrthoHardware> listOrthoHardwares=OrthoHardwares.GetPatientData(PatientCur.PatNum);
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			List<OrthoHardwares.OrthoWire> listOrthoWires=new List<OrthoHardwares.OrthoWire>();//also used for elastics
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

		private void ToolBarMainUpdate_Click() {
			if(!new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {
				MsgBox.Show(this,"The update fee utility only works on current treatment plans, not any saved plans.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Update all fees and insurance estimates on this treatment plan to the current fees for this patient?")) {
				return;
			}
			Procedure procedure;
			List<ClaimProc> listClaimProcs=ClaimProcs.RefreshForTP(PatientCur.PatNum);
			List<ProcedureCode> listProcedureCodes=new List<ProcedureCode>();
			for(int i=0;i<_listProceduresTP.Count;i++) {
				listProcedureCodes.Add(ProcedureCodes.GetProcCode(_listProceduresTP[i].CodeNum));
			}
			long discountPlanNum=_tpModuleData.DiscountPlanSub?.DiscountPlanNum??0;
			List<Fee> listFees=Fees.GetListFromObjects(listProcedureCodes,_listProceduresTP.Select(x=>x.MedicalCode).ToList(),_listProceduresTP.Select(x=>x.ProvNum).ToList(),
				PatientCur.PriProv,PatientCur.SecProv,PatientCur.FeeSched,_listInsPlans,_listProceduresTP.Select(x=>x.ClinicNum).ToList(),null,//listAppts not needed because procs not based on appts
				_listSubstitutionLinks,discountPlanNum);
			//Get data for any OrthoCases that may be linked to procs in ProcList
			long patNum=PatientCur.PatNum;
			List<OrthoCase> listOrthoCases=OrthoCases.Refresh(patNum);
			List<OrthoProcLink> listOrthoProcLinksAll=OrthoProcLinks.GetManyByOrthoCases(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList());
			List<long> listProcNums=_listProcedures.Select(x=>x.ProcNum).ToList();
			List<OrthoProcLink> listOrthoProcLinks=listOrthoProcLinksAll.FindAll(x=>listProcNums.Contains(x.ProcNum));
			List<OrthoSchedule> listOrthoSchedules=new List<OrthoSchedule>();
			if(listOrthoProcLinks.Count>0) {
				List<long> listSchedulePlanLinksFKey=OrthoPlanLinks.GetAllForOrthoCasesByType(listOrthoCases.Select(x=>x.OrthoCaseNum).ToList(),OrthoPlanLinkType.OrthoSchedule).Select(x=>x.FKey).ToList();
				listOrthoSchedules=OrthoSchedules.GetMany(listSchedulePlanLinksFKey);
			}
			List<DiscountPlanProc> listDiscountPlanProcs=DiscountPlans.GetDiscountPlanProc(_listProceduresTP.ToList(),_tpModuleData.DiscountPlanSub,_tpModuleData.DiscountPlan);
			BlueBookEstimateData blueBookEstimateData=new BlueBookEstimateData(_listInsPlans,_listInsSubs,_listPatPlans,_listProceduresTP.ToList(),_listSubstitutionLinks);
			for(int i=0;i<_listProceduresTP.Count;i++) {
				procedure=_listProceduresTP[i];
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")
					&& procedure.ProcNumLab!=0) 
				{
					continue;//The proc fee for a lab is derived from the lab fee on the parent procedure.
				}
				Procedure procOld=procedure.Copy(); //Get a copy of the old proc in case we need to update & to track the old procFee
				//Try to update the proc fee
				procedure.ProcFee=Procedures.GetProcFee(PatientCur,_listPatPlans,_listInsSubs,_listInsPlans,procedure.CodeNum,procedure.ProvNum,procedure.ClinicNum,
					procedure.MedicalCode,listFees:listFees);
				OrthoCase orthoCase=null;
				OrthoSchedule orthoSchedule=null;
				List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
				OrthoProcLink orthoProcLink=listOrthoProcLinks.Find(x => x.ProcNum==_listProcedures[i].ProcNum);
				if(orthoProcLink!=null) {
					long orthoCaseNum=orthoProcLink.OrthoCaseNum;
					orthoCase=listOrthoCases.Find(x=>x.OrthoCaseNum==orthoCaseNum);
					orthoSchedule=listOrthoSchedules.Find(x=>x.OrthoScheduleNum==orthoCaseNum);
					listOrthoProcLinksForOrthoCase=listOrthoProcLinksAll.FindAll(x=>x.OrthoCaseNum==orthoCaseNum);
				}
				Procedures.ComputeEstimates(procedure,PatientCur.PatNum,listClaimProcs,false,_listInsPlans,_listPatPlans,_listBenefits,PatientCur.Age,_listInsSubs,
					orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase,listFees,blueBookEstimateData);
				procedure.DiscountPlanAmt=listDiscountPlanProcs.FirstOrDefault(x=>x.ProcNum==procedure.ProcNum)?.DiscountPlanAmt??0;
				if(AvaTax.DoSendProcToAvalara(procedure)) { //If needed, update the sales tax amount as well (checks HQ)
					procedure.TaxAmt=(double)AvaTax.GetEstimate(procedure.CodeNum,procedure.PatNum,procedure.ProcFee);
				}
				//If the proc fee changed or the tax amt changed or the discount plan amount, update the procedurelog entry
				if(procOld.ProcFee!=procedure.ProcFee || procOld.TaxAmt!=procedure.TaxAmt || procOld.DiscountPlanAmt!=procedure.DiscountPlanAmt) {
					Procedures.Update(procedure,procOld);
				}
				//no recall synch required 
			}
			long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
			ModuleSelected(PatientCur.PatNum);//refreshes TPs
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
				List<Procedure> listProcedures=Procedures.GetProcsForSingle(Bridges.ECW.AptNum,false);
				string duplicateProcs=ProcedureL.ProcsContainDuplicates(listProcedures);
				if(duplicateProcs!="") {
					MessageBox.Show(duplicateProcs);
					return;
				}
			}
			if(gridMain.SelectedIndices.Length==0){
				gridMain.SetAll(true);//Select all if none selected.
			}
			List<TreatPlanAttach> listTreatPlanAttaches=TreatPlanAttaches.GetAllForTreatPlan(_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum);
			TreatPlan treatPlan=new TreatPlan();
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
				using InputBox inputBoxHeadingName=new InputBox(Lan.g(this,$"Save Treatment Plan as"),treatPlanHeading);
				if(inputBoxHeadingName.ShowDialog()!=DialogResult.OK) {
					return;
				}
				treatPlanHeading=inputBoxHeadingName.textResult.Text;
			}
			//If any of the following logic changes, notify someone on the xam team.
			treatPlan.Heading=treatPlanHeading;
			treatPlan.DateTP=DateTime.Today;
			treatPlan.PatNum=PatientCur.PatNum;
			treatPlan.Note=_listTreatPlans[gridPlans.SelectedIndices[0]].Note;
			treatPlan.ResponsParty=PatientCur.ResponsParty;
			treatPlan.UserNumPresenter=Security.CurUser.UserNum;
			treatPlan.TPType=_listTreatPlans[gridPlans.SelectedIndices[0]].TPType;
			List<ProcTP> listProcTPsSelected=gridMain.SelectedIndices.Where(x => x>-1 && x<gridMain.ListGridRows.Count)
				.Select(x => (ProcTP)gridMain.ListGridRows[x].Tag).ToList();
			long treatPlanNum=TreatPlans.CreateArchivedTreatPlan(treatPlan,PatientCur,listProcTPsSelected,listTreatPlanAttaches);
			ModuleSelected(PatientCur.PatNum);
			for(int i=0;i<_listTreatPlans.Count;i++){
				if(_listTreatPlans[i].TreatPlanNum==treatPlanNum){
					gridPlans.SetSelected(i,true);
					FillMain();
				}
			}
			//Send TP DFT HL7 message to ECW with embedded PDF when using tight or full integration only.
			if(Programs.UsingEcwTightOrFullMode() && Bridges.ECW.AptNum!=0){
				PrepImageForPrinting();
				MigraDoc.Rendering.PdfDocumentRenderer pdfDocumentRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(true,PdfFontEmbedding.Always);
				pdfDocumentRenderer.Document=CreateDocument();
				pdfDocumentRenderer.RenderDocument();
				MemoryStream memoryStream=new MemoryStream();
				pdfDocumentRenderer.PdfDocument.Save(memoryStream);
				byte[] byteArrayPdf=memoryStream.GetBuffer();
				//#region Remove when testing is complete.
				//string tempFilePath=Path.GetTempFileName();
				//File.WriteAllBytes(tempFilePath,pdfBytes);
				//#endregion
				string pdfDataStr=Convert.ToBase64String(byteArrayPdf);
				if(HL7Defs.IsExistingHL7Enabled()) {
					//DFT messages that are PDF's only and do not include FT1 segments, so proc list can be empty
					//MessageConstructor.GenerateDFT(procList,EventTypeHL7.P03,PatCur,Patients.GetPat(PatCur.Guarantor),Bridges.ECW.AptNum,"treatment",pdfDataStr);
					MessageHL7 messageHL7=MessageConstructor.GenerateDFT(new List<Procedure>(),EventTypeHL7.P03,PatientCur,Patients.GetPat(PatientCur.Guarantor),Bridges.ECW.AptNum,"treatment",pdfDataStr);
					if(messageHL7==null) {
						MsgBox.Show(this,"There is no DFT message type defined for the enabled HL7 definition.");
						return;
					}
					HL7Msg hL7Msg=new HL7Msg();
					hL7Msg.AptNum=0;//Prevents the appt complete button from changing to the "Revise" button prematurely.
					hL7Msg.HL7Status=HL7MessageStatus.OutPending;//it will be marked outSent by the HL7 service.
					hL7Msg.MsgText=messageHL7.ToString();
					hL7Msg.PatNum=PatientCur.PatNum;
					HL7Msgs.Insert(hL7Msg);
				}
				else {
					Bridges.ECW.SendHL7(Bridges.ECW.AptNum,PatientCur.PriProv,PatientCur,pdfDataStr,"treatment",true,null);//just pdf, passing null proc list
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
				string errMsg=Documents.OpenDoc(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum);
				Cursor=Cursors.Default;
				if(errMsg!="") {
					MsgBox.Show(errMsg);
				}
				return;//cannot re-sign document.
			}
			if(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum>0 && !Documents.DocExists(_listTreatPlans[gridPlans.SelectedIndices[0]].DocNum)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Unable to open saved treatment plan. Would you like to recreate document using current information?")) {
					return;
				}
			}//TODO: Implement ODprintout pattern - MigraDoc
			using FormTPsign formTPsign=new FormTPsign();
			if(DoPrintUsingSheets()) {
				TreatPlan treatPlan;
				treatPlan=_listTreatPlans[gridPlans.SelectedIndices[0]].Copy();
				treatPlan.ListProcTPs=ProcTPs.RefreshForTP(treatPlan.TreatPlanNum);
				formTPsign.SheetTP=TreatPlanToSheet(treatPlan);
				formTPsign.TotalPages=Sheets.CalculatePageCount(formTPsign.SheetTP,SheetPrinting.PrintMargin);
			}
			else {//Classic TPs
				PrepImageForPrinting();
				MigraDoc.DocumentObjectModel.Document document=CreateDocument();
				MigraDocPrintDocument migraDocPrintDocument=new MigraDoc.Rendering.Printing.MigraDocPrintDocument();
				DocumentRenderer documentRenderer=new MigraDoc.Rendering.DocumentRenderer(document);
				documentRenderer.PrepareDocument();
				migraDocPrintDocument.Renderer=documentRenderer;
				formTPsign.Document=migraDocPrintDocument;
				formTPsign.TotalPages=documentRenderer.FormattedDocument.PageCount;
			}
			long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
			formTPsign.SaveDocDelegate=SaveTPAsDocument;
			formTPsign.TPcur=_listTreatPlans[gridPlans.SelectedIndices[0]];
			formTPsign.DoPrintUsingSheets=DoPrintUsingSheets();
			formTPsign.ShowDialog();
			ModuleSelected(PatientCur.PatNum);//refreshes TPs
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
			List<Document> listDocumentsRetVals=new List<Document>();
			//Determine each of the document categories that this TP should be saved to.
			//"R"==TreatmentPlan; see FormDefEditImages.cs
			List<Def> listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			List<long> listNumsCategories= listDefsImageCats.Where(x => x.ItemValue.Contains("R")).Select(x=>x.DefNum).ToList();
			if(isSigSave && listNumsCategories.Count==0 && PrefC.GetBool(PrefName.TreatPlanSaveSignedToPdf)) {
				//we must save at least one document, pick first non-hidden image category.
				Def defImgCat=listDefsImageCats.FirstOrDefault(x => !x.IsHidden);
				if(defImgCat==null) {
					MsgBox.Show(this,"Unable to save treatment plan because all image categories are hidden.");
					return new List<Document>();
				}
				listNumsCategories.Add(defImgCat.DefNum);
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
				MigraDoc.Rendering.PdfDocumentRenderer pdfDocumentRenderer;
				pdfDocumentRenderer=new MigraDoc.Rendering.PdfDocumentRenderer(false,PdfFontEmbedding.Always);
				pdfDocumentRenderer.Document=CreateDocument();
				pdfDocumentRenderer.RenderDocument();
				pdfDocumentRenderer.Save(tempFile);
				if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
					using MemoryStream memoryStream=new MemoryStream();
					pdfDocumentRenderer.Save(memoryStream,false);
					rawBase64=Convert.ToBase64String(memoryStream.ToArray());
					memoryStream.Close();
				}
			}
			for(int i=0;i< listNumsCategories.Count;i++) {//usually only one, but do allow them to be saved once per image category.
				OpenDentBusiness.Document documentSave=new Document();
				documentSave.DocNum=Documents.Insert(documentSave);
				string fileName="TPArchive"+documentSave.DocNum;
				documentSave.ImgType=ImageType.Document;
				documentSave.DateCreated=DateTime.Now;
				documentSave.PatNum=PatientCur.PatNum;
				documentSave.DocCategory=listNumsCategories[i];
				documentSave.Description=fileName;//no extension.
				documentSave.RawBase64=rawBase64;//blank if using AtoZfolder
				if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
					string filePath=ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath());
					while(File.Exists(filePath+"\\"+fileName+".pdf")) {
						fileName+="x";
					}
					File.Copy(tempFile,filePath+"\\"+fileName+".pdf");
				}
				else if(CloudStorage.IsCloudStorage) {
					//Upload file to patient's AtoZ folder
					using FormProgress formProgress=new FormProgress();
					formProgress.DisplayText="Uploading Treatment Plan...";
					formProgress.NumberFormat="F";
					formProgress.NumberMultiplication=1;
					formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
					formProgress.TickMS=1000;
					OpenDentalCloud.Core.TaskStateUpload taskStateUpload=CloudStorage.UploadAsync(ImageStore.GetPatientFolder(PatientCur,"")
						,fileName+".pdf"
						,File.ReadAllBytes(tempFile)
						,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
					if(formProgress.ShowDialog()==DialogResult.Cancel) {
						taskStateUpload.DoCancel=true;
						break;
					}
				}
				documentSave.FileName=fileName+".pdf";//file extension used for both DB images and AtoZ images
				Documents.Update(documentSave);
				listDocumentsRetVals.Add(documentSave);
			}
			try {
				File.Delete(tempFile); //cleanup the temp file.
			}
			catch {}
			return listDocumentsRetVals;
		}

		///<summary>Returns true if the user has not checked 'Print using classic'.</summary>
		private bool DoPrintUsingSheets() {
			//If the Printing tab is visible and the print classic box is checked, then print classic.
			return (!tabControlShowSort.TabPages.Contains(tabPagePrint) || !checkPrintClassic.Checked);
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
				List<int> listIndicesSelected=new List<int>(gridMain.SelectedIndices);
				if(gridMain.SelectedIndices.Length>7) {
					gridMain.SetAll(false);
					int selectedLabCount=0;
					for(int i=0;i<listIndicesSelected.Count;i++) {
						if(gridMain.ListGridRows[listIndicesSelected[i]].Tag==null) {
							continue;//subtotal row.
						}
						Procedure proc=(Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[listIndicesSelected[i]].Tag).ProcNumOrig,false));
						if(proc!=null && proc.ProcNumLab!=0) {
								selectedLabCount++;
						}
						gridMain.SetSelected(listIndicesSelected[i],true);
						if(gridMain.SelectedIndices.Length-selectedLabCount>=7) {
							break;//we have found seven procedures.
						}
					}
					if(listIndicesSelected.FindAll(x => gridMain.ListGridRows[x].Tag!=null).Count-selectedLabCount>7) {//only if they selected more than 7 procedures, not 7 rows.
						MsgBox.Show(this,"Only the first 7 procedures will be selected.  You will need to create another preauth for the remaining procedures.");
					}
				}
			}
			Claim claim=new Claim();
			using FormInsPlanSelect formInsPlanSelect=new FormInsPlanSelect(PatientCur.PatNum); 
			formInsPlanSelect.ViewRelat=true;
			if(formInsPlanSelect.InsPlanSelected==null) {//Won't be null if there is only one PatPlan
				formInsPlanSelect.ShowDialog();
				if(formInsPlanSelect.DialogResult!=DialogResult.OK) {
					return;
				}
			}
			claim.PatNum=PatientCur.PatNum;
			claim.ClaimNote="";
			claim.ClaimStatus="W";
			claim.DateSent=DateTime.Today;
			claim.DateSentOrig=DateTime.MinValue;
			claim.PlanNum=formInsPlanSelect.InsPlanSelected.PlanNum;
			claim.InsSubNum=formInsPlanSelect.InsSubSelected.InsSubNum;
			claim.ProvTreat=0;
			claim.ClaimForm=formInsPlanSelect.InsPlanSelected.ClaimFormNum;
			claim.MedType=EnumClaimMedType.Dental;
			if(formInsPlanSelect.InsPlanSelected.IsMedical) {
				claim.MedType=PrefC.GetBool(PrefName.ClaimMedTypeIsInstWhenInsPlanIsMedical) ? EnumClaimMedType.Institutional : EnumClaimMedType.Medical;
			}
			List<Procedure> listProceduresSelected=new List<Procedure>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				if(gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag==null){
					continue;//skip any hightlighted subtotal lines
				}
				Procedure procedure=Procedures.GetOneProc(((ProcTP)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag).ProcNumOrig,false);
				if(Procedures.NoBillIns(procedure,_listClaimProcs,claim.PlanNum)) {
					MsgBox.Show(this,"Not allowed to send procedures to insurance that are marked 'Do not bill to ins'.");
					return;
				}
				if(procedure.ProcNumLab!=0) {
					continue;//Ignore Canadian labs. Labs are indirectly attached to the claim through a parent procedure
				}
				listProceduresSelected.Add(procedure);
				if(claim.ProvTreat==0){//makes sure that at least one prov is set
					claim.ProvTreat=procedure.ProvNum;
				}
				if(!Providers.GetIsSec(procedure.ProvNum)) {
					claim.ProvTreat=procedure.ProvNum;
				}
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(procedure.CodeNum);
				//Check to see if the selected procedure is for Ortho and if the customer has preferences set to automatically check the "Is For Ortho" checkbox
				if(!claim.IsOrtho && PrefC.GetBool(PrefName.OrthoClaimMarkAsOrtho)) {//If it's already marked as Ortho (from a previous procedure), skip this
					CovCat covCatOrtho=CovCats.GetFirstOrDefault(x => x.EbenefitCat==EbenefitCategory.Orthodontics,true);
					if(covCatOrtho!=null) {
						if(CovSpans.IsCodeInSpans(procedureCode.ProcCode,CovSpans.GetWhere(x => x.CovCatNum==covCatOrtho.CovCatNum).ToArray()))	{
							claim.IsOrtho=true;
						}
					}
				}
			}
			//Make sure that all procedures share the same place of service and clinic.
			long procClinicNum=-1;
			PlaceOfService placeOfService=PlaceOfService.Office;//Old behavior was to always use Office.
			for(int i=0;i<listProceduresSelected.Count;i++) {
				if(i==0) {
					procClinicNum=listProceduresSelected[i].ClinicNum;
					placeOfService=listProceduresSelected[i].PlaceService;
					continue;
				}
				Procedure procedure=listProceduresSelected[i];
				if(PrefC.HasClinicsEnabled && procClinicNum!=procedure.ClinicNum) {
					MsgBox.Show(this,"All procedures do not have the same clinic.");
					return;
				}
				if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && procedure.PlaceService!=placeOfService) {
					MsgBox.Show(this,"All procedures do not have the same place of service.");
					return;
				}
			}
			switch(PIn.Enum<ClaimZeroDollarProcBehavior>(PrefC.GetInt(PrefName.ClaimZeroDollarProcBehavior))) {
				case ClaimZeroDollarProcBehavior.Warn:
					if(listProceduresSelected.FirstOrDefault(x => CompareDouble.IsZero(x.ProcFee))!=null
						&& !MsgBox.Show("ContrTreat",MsgBoxButtons.OKCancel,"You are about to make a claim that will include a $0 procedure.  Continue?"))
					{
						return;
					}
					break;
				case ClaimZeroDollarProcBehavior.Block:
					if(listProceduresSelected.FirstOrDefault(x => CompareDouble.IsZero(x.ProcFee))!=null) {
						MsgBox.Show("ContrTreat","You can't make a claim for a $0 procedure.");
						return;
					}
					break;
				case ClaimZeroDollarProcBehavior.Allow:
				default:
					break;
			}
			//Make the clinic on the claim match the clinic of the procedures.  Use the patients clinic if no procedures selected (shouldn't happen).
			claim.ClinicNum=(procClinicNum > -1) ? procClinicNum : PatientCur.ClinicNum;
			if(Providers.GetIsSec(claim.ProvTreat)){
				claim.ProvTreat=PatientCur.PriProv;
				//OK if 0, because auto select first in list when open claim
			}
			claim.ProvBill=Providers.GetBillingProvNum(claim.ProvTreat,claim.ClinicNum);
			Provider provider=Providers.GetProv(claim.ProvTreat);//If ever null then check this same line inside AccountModules.CreateClaim()
			if(provider.ProvNumBillingOverride!=0) {
				claim.ProvBill=provider.ProvNumBillingOverride;
			}
			claim.EmployRelated=YN.No;
			claim.ClaimType="PreAuth";
			claim.AttachedFlags="Mail";
			//this could be a little better if we automate figuring out the patrelat
			//instead of making the user enter it:
			claim.PatRelat=formInsPlanSelect.Relat;
			claim.PlaceService=placeOfService;
			claim=Claims.GetClaim(Claims.Insert(claim));
			ClaimProc claimProc;
			ClaimProc claimProcExisting;
			List<ClaimProc> listClaimProcs=new List<ClaimProc>();
			List<long> listCodeNums=new List<long>();//List of codeNums that have had their default note added to the claim.
			for(int i=0;i<listProceduresSelected.Count;i++) {
				claimProcExisting=ClaimProcs.GetEstimate(_listClaimProcs,listProceduresSelected[i].ProcNum,formInsPlanSelect.InsPlanSelected.PlanNum,formInsPlanSelect.InsSubSelected.InsSubNum);
				double insPayEst=0;
				if(claimProcExisting!=null) {
					insPayEst=claimProcExisting.InsPayEst;
				}
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listProceduresSelected[i].CodeNum);
				claimProc=new ClaimProc();
				ClaimProcs.CreateEst(claimProc,listProceduresSelected[i],formInsPlanSelect.InsPlanSelected,formInsPlanSelect.InsSubSelected,0,insPayEst,false,true);//preauth est
				claimProc.ClaimNum=claim.ClaimNum;
				claimProc.NoBillIns=(claimProcExisting!=null) ? claimProcExisting.NoBillIns : false;
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA") && claimProcExisting!=null) {
					//ClaimProc.Percentage is typically set in ClaimProcs.ComputeBaseEst(...), not for pre-auths. 
					claimProc.Percentage=claimProcExisting.Percentage;//Used for Canadian preauths with lab procedures.
				}
				claimProc.FeeBilled=listProceduresSelected[i].ProcFee;
				if(formInsPlanSelect.InsPlanSelected.UseAltCode && (procedureCode.AlternateCode1!="")){
					claimProc.CodeSent=procedureCode.AlternateCode1;
				}
				else if(formInsPlanSelect.InsPlanSelected.IsMedical && listProceduresSelected[i].MedicalCode!=""){
					claimProc.CodeSent=listProceduresSelected[i].MedicalCode;
				}
				else{
					claimProc.CodeSent=ProcedureCodes.GetStringProcCode(listProceduresSelected[i].CodeNum);
					if(claimProc.CodeSent.Length>5 && claimProc.CodeSent.Substring(0,1)=="D"){
						claimProc.CodeSent=claimProc.CodeSent.Substring(0,5);
					}
					if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
						if(claimProc.CodeSent.Length>5) { //In Canadian electronic claims, codes can contain letters or numbers and cannot be longer than 5 characters.
							claimProc.CodeSent=claimProc.CodeSent.Substring(0,5);
						}
					}
				}
				listClaimProcs.Add(claimProc);
				if(claim.ClaimNote==null) {
					claim.ClaimNote="";
				}
				if(!listCodeNums.Contains(procedureCode.CodeNum)) {
					if(claim.ClaimNote.Length > 0 && !string.IsNullOrEmpty(procedureCode.DefaultClaimNote)) {
						claim.ClaimNote+="\n";
					}
					claim.ClaimNote+=procedureCode.DefaultClaimNote;
					listCodeNums.Add(procedureCode.CodeNum);
				}
				//ProcCur.Update(ProcOld);
			}
			for(int i=0;i<listClaimProcs.Count;i++) {
				listClaimProcs[i].LineNumber=(byte)(i+1);
				ClaimProcs.Insert(listClaimProcs[i]);
			}
			_listProcedures=Procedures.Refresh(PatientCur.PatNum);
			//ClaimProcList=ClaimProcs.Refresh(PatCur.PatNum);
			Claims.CalculateAndUpdate(_listProcedures,_listInsPlans,claim,_listPatPlans,_listBenefits,PatientCur,_listInsSubs);
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,PatientCur,_family);
			//FormCE.CalculateEstimates(
			formClaimEdit.IsNew=true;//this causes it to delete the claim if cancelling.
			formClaimEdit.ShowDialog();
			ModuleSelected(PatientCur.PatNum);
		}

		private void ToolBarMainDiscount_Click() {
			Def def=Defs.GetDef(DefCat.AdjTypes,PrefC.GetLong(PrefName.TreatPlanDiscountAdjustmentType));
			if(!GroupPermissions.HasPermissionForAdjType(Permissions.AdjustmentCreate,def,false)) {
				return;
			}
			if(!new[] { TreatPlanStatus.Active,TreatPlanStatus.Inactive }.Contains(_listTreatPlans[gridPlans.SelectedIndices[0]].TPStatus)) {
				MsgBox.Show(this,"You can only create discounts from a current TP, not a saved TP.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				gridMain.SetAll(true);
			}
			List<Procedure> listProcedures=Procedures.GetManyProc(gridMain.SelectedIndices.ToList()
				.FindAll(x => gridMain.ListGridRows[x].Tag!=null)
				.Select(x => ((ProcTP)gridMain.ListGridRows[x].Tag).ProcNumOrig)
				.ToList(),false);
			if(listProcedures.Count<=0) {
				MsgBox.Show(this,"There are no procedures selected in the treatment plan. Please add to, or select from, procedures attached to the treatment plan before applying a discount");
				return;
			}
			using FormTreatmentPlanDiscount formTreatmentPlanDiscount=new FormTreatmentPlanDiscount(listProcedures);
			formTreatmentPlanDiscount.ShowDialog();
			if(formTreatmentPlanDiscount.DialogResult==DialogResult.OK) {
				long tpNum=_listTreatPlans[gridPlans.SelectedIndices[0]].TreatPlanNum;
				ModuleSelected(PatientCur.PatNum);//refreshes TPs
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
			Claim claim=Claims.GetClaim(((Claim)_arrayListPreAuth[e.Row]).ClaimNum);//gets attached images.
			if(claim==null) {
				MsgBox.Show(this,"The pre authorization has been deleted.");
				ModuleSelected(PatientCur.PatNum);
				return;
			}
			using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,PatientCur,_family);
			//FormClaimEdit2.IsPreAuth=true;
			formClaimEdit.ShowDialog();
			if(formClaimEdit.DialogResult!=DialogResult.OK){
				return;
			}
			ModuleSelected(PatientCur.PatNum);
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
			Claim claim=(Claim)_arrayListPreAuth[e.Row];
			List<ClaimProc> listClaimProcsForClaims=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			for(int i=0;i<gridMain.ListGridRows.Count;i++){//ProcListTP.Length;i++){
				if(gridMain.ListGridRows[i].Tag==null){
					continue;//must be a subtotal row
				}
				ProcTP procTP=(ProcTP)gridMain.ListGridRows[i].Tag;
				//proc=(Procedure)gridMain.Rows[i].Tag;
				for(int j=0;j<listClaimProcsForClaims.Count;j++){
					if(procTP.ProcNumOrig==listClaimProcsForClaims[j].ProcNum){
						gridMain.SetSelected(i,true);
						CanadianSelectedRowHelper(procTP);
						break;
					}
				}
			}
		}

		private void butInsRem_Click(object sender,EventArgs e) {
			if(PatientCur==null) {
				MsgBox.Show(this,"Please select a patient before attempting to view insurance remaining.");
				return;
			}
			using FormInsRemain formInsRemain=new FormInsRemain(PatientCur.PatNum);
			formInsRemain.ShowDialog();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			//Update all unscheduled procs and claimprocs on the active TP to use the currently selected date as the ProcDate
			RefreshModuleScreen();
		}
		
		private void butPlannedAppt_Click(object sender,EventArgs e) {
			if(PatientCur==null) {
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
			ChartModuleFilters chartModuleComponentsToLoad=new ChartModuleFilters(false);
			chartModuleComponentsToLoad.ShowAppointments=true;
			//Makes sure ChartModules.rawApt is filled before calling ChartModules.GetPlannedApt
			ChartModules.GetProgNotes(PatientCur.PatNum,false,chartModuleComponentsToLoad);
			int itemOrder=ChartModules.GetPlannedApt(PatientCur.PatNum).Rows.Count+1;
			List<long> listProcNumsSelected=gridMain.SelectedGridRows
				.FindAll(x => x.Tag!=null && x.Tag.GetType()==typeof(ProcTP))//ProcTP's only
				.Select(x => ((ProcTP)(x.Tag)).ProcNumOrig).ToList();//get ProcNums
			//mimic calls to CreatePlannedAppt in ContrChart, no need for FillPlanned() since no gridPlanned
			AppointmentL.CreatePlannedAppt(PatientCur,itemOrder,listProcNumsSelected);
		}

		private void dateTimeTP_OnLeave(object sender,EventArgs e) {
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
			if(PatientCur!=null && _listTreatPlans.Count>0) {
				gridMain.Enabled=true;
				tabControlShowSort.Enabled=true;
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
				if(_listPatPlans.Count==0) {//patient doesn't have insurance
					checkShowMaxDed.Visible=false;
					if(_tpModuleData.DiscountPlanSub==null) {
						checkShowIns.Checked=false;
					} 
					else if(!_checkShowInsNotAutomatic) {
						checkShowIns.Checked=true;
					}
				}
				else {//patient has insurance
					if(!PrefC.GetBool(PrefName.EasyHideInsurance)) {//if insurance isn't hidden
						checkShowMaxDed.Visible=true;
						if(checkShowFees.Checked) {//if fees are showing
							if(!_checkShowInsNotAutomatic) {
								checkShowIns.Checked=true;
							}
						}
					}
				}
				if(!_checkShowDiscountNotAutomatic) {
					checkShowDiscount.Checked=true;
				}
				return;
			}
			gridMain.Enabled=false;
			tabControlShowSort.Enabled=false;
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
