using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsVerificationList:FormODBase {
		///<summary>-1 represents "All", and 0 represents "none".</summary>
		private long _verifyUserNum=-1;
		///<summary>0 represents "Unassign".</summary>
		private long _assignUserNum;
		///<summary>-1 represents "All", and 0 represents "Unassigned".</summary>
		private List<long> _listClinicNumsVerifyClinicsFilter=new List<long>();
		///<summary>-1 and 0 represent "All".</summary>
		private List<long> _listDefNumsVerifyRegionsFilter=new List<long>() { -1 };
		private long _defNumVerifyStatusFilter;
		private long _defNumVerifyStatusAssign;
		///<summary>This will only have a selection if selecting from gridMain.</summary>
		private InsVerifyGridObject _gridRowSelected;
		private List<Userod> _listUsersInRegionWithAssignedIns=new List<Userod>();
		private List<Userod> _listUsersInRegion=new List<Userod>();
		private List<Def> _listVerifyStatuses=new List<Def>();
		private long _userNumVerifyGrid=0;
		private int _selectedRowVerifyGrid;
		private int _selectedRowAssignGrid;
		private Dictionary<long,Def> _dictStatusDefs=new Dictionary<long,Def>();
		private ContextMenu menuRightClick=new ContextMenu();
		private List<Def> _listRegionDefs;

		///<summary>Indicates whether the "All" option is enabled for the listbox for clinics.</summary>
		private bool _isAllClinicsEnabled {
			get{
				return PrefC.HasClinicsEnabled && !Security.CurUser.ClinicIsRestricted && !PrefC.GetBool(PrefName.EnterpriseApptList);
			}
		}

		///<summary>Indicates if a region is selected in the region listbox.</summary>
		private bool _isRegionSelected {
			get {
				return !_listDefNumsVerifyRegionsFilter.Contains(0) && !_listDefNumsVerifyRegionsFilter.Contains(-1);
			}
		}

		public FormInsVerificationList() {
			InitializeComponent();
			InitializeLayoutManager();
			menuRightClick.Popup+=gridContextMenuStrip_Popup;
			gridMain.ContextMenu=menuRightClick;
			gridPastDue.ContextMenu=menuRightClick;
			gridAssign.ContextMenu=menuRightClick;
			Lan.F(this);
		}

		private void FormInsVerificationList_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				_listClinicNumsVerifyClinicsFilter.Add(-1);//-1 represents "All"
			}
			SetFilterControlsAndAction(() => FillGrids(),
				textPatientEnrollmentDays,textInsBenefitEligibilityDays,textAppointmentScheduledDays,textVerifyCarrier);
			if(PrefC.GetBool(PrefName.InsVerifyDefaultToCurrentUser)) {
				_verifyUserNum=Security.CurUser.UserNum;
			}
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				listBoxVerifyClinics.Visible=false;
				labelRegion.Visible=false;
				listBoxVerifyRegions.Visible=false;
			}
			List<Def> listVerifyStatuses=Defs.GetDefsForCategory(DefCat.InsuranceVerificationStatus,true);
			foreach(Def defCur in listVerifyStatuses) {
				if(!_dictStatusDefs.ContainsKey(defCur.DefNum)) {
					_dictStatusDefs.Add(defCur.DefNum,defCur);
				}
			}
			textAppointmentScheduledDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDays));
			textInsBenefitEligibilityDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
			textPatientEnrollmentDays.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
			InsVerifies.CleanupInsVerifyRows(DateTime.Today,DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDays.Text)));
		}

		private void FormInsVerificationList_Shown(object sender,EventArgs e) {
			//This must be in Shown due to the progress bar forcing this window behind other windows sometimes.
			FillGrids();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrids();
		}

		public void FillGrids() {
			FillComboBoxes();
			FillUsers();
			if(tabControl1.SelectedTab==tabVerify) {
				FillGrid();
			}
			else if(tabControl1.SelectedTab==tabAssign) {
				FillGridAssign();
			}
		}

		private void FillDisplayInfo(InsVerifyGridObject gridRowObject) {
			//Always blank out the old selected information before filling with the new information.
			textSubscriberName.Text="";
			textSubscriberBirthdate.Text="";
			textSubscriberSSN.Text="";
			textSubscriberID.Text="";
			textInsPlanGroupName.Text="";
			textInsPlanGroupNumber.Text="";
			textInsPlanNote.Text="";
			textCarrierName.Text="";
			textCarrierPhoneNumber.Text="";
			textInsPlanEmployer.Text="";
			textInsVerifyReadOnlyNote.Text="";
			if(gridRowObject==null) {
				return;
			}
			PatPlan patPlanVerify=PatPlans.GetByPatPlanNum(gridRowObject.GetPatPlanNum());
			if(patPlanVerify==null) {//Should never happen, but if it does, return because it is just for display purposes.
				return;
			}
			InsSub insSubVerify=InsSubs.GetOne(patPlanVerify.InsSubNum);
			textSubscriberID.Text=insSubVerify.SubscriberID;
			Patient patSubscriberVerify=Patients.GetPat(insSubVerify.Subscriber);
			if(patSubscriberVerify!=null) {
				textSubscriberName.Text=patSubscriberVerify.GetNameFL();
				textSubscriberBirthdate.Text=patSubscriberVerify.Birthdate.ToShortDateString();
				textSubscriberSSN.Text=patSubscriberVerify.SSN;
			}
			FillInsuranceDisplay(InsPlans.GetPlan(insSubVerify.PlanNum,null));
			textPatBirthdate.Text=Patients.GetPat(gridRowObject.GetPatNum()).Birthdate.ToShortDateString();
			if(gridRowObject.IsOnlyInsRow()) {
				textInsVerifyReadOnlyNote.Text=_gridRowSelected.PlanInsVerify.Note;
			}
			else {//Both or Pat row
				textInsVerifyReadOnlyNote.Text=_gridRowSelected.PatInsVerify.Note;
			}
		}

		private void FillInsuranceDisplay(InsPlan insPlanVerify) {
			if(insPlanVerify==null) {//Should never happen, but if it does, return because it is just for display purposes.
				return;
			}
			textInsPlanGroupName.Text=insPlanVerify.GroupName;
			textInsPlanGroupNumber.Text=insPlanVerify.GroupNum;
			textInsPlanNote.Text=insPlanVerify.PlanNote;
			Employer employer=Employers.GetEmployer(insPlanVerify.EmployerNum);
			if(employer!=null) {
				textInsPlanEmployer.Text=employer.EmpName;
			}
			Carrier carrierVerify=Carriers.GetCarrier(insPlanVerify.CarrierNum);
			if(carrierVerify!=null) {
				textCarrierName.Text=carrierVerify.CarrierName;
				textCarrierPhoneNumber.Text=carrierVerify.Phone;
			}
		}

		///<summary>Does not fill the Clinic Combo Box</summary>
		private void FillComboBoxes() {
			comboSetVerifyStatus.Items.Clear();
			comboFilterVerifyStatus.Items.Clear();
			comboFilterVerifyStatus.Items.Add("All");
			comboSetVerifyStatus.Items.Add("none");
			_listVerifyStatuses=Defs.GetDefsForCategory(DefCat.InsuranceVerificationStatus,true);
			for(int i=0;i<_listVerifyStatuses.Count;i++) {
				comboFilterVerifyStatus.Items.Add(_listVerifyStatuses[i].ItemName);
				comboSetVerifyStatus.Items.Add(_listVerifyStatuses[i].ItemName);
				if(_listVerifyStatuses[i].DefNum==_defNumVerifyStatusFilter) {
					comboFilterVerifyStatus.SelectedIndex=i+1;
				}
				if(_listVerifyStatuses[i].DefNum==_defNumVerifyStatusAssign) {
					comboSetVerifyStatus.SelectedIndex=i+1;
				}
			}
			if(comboFilterVerifyStatus.SelectedIndex==-1) {
				comboFilterVerifyStatus.SelectedIndex=0;
			}
			if(comboSetVerifyStatus.SelectedIndex==-1) {
				comboSetVerifyStatus.SelectedIndex=0;
			}
			listBoxVerifyRegions.Items.Clear();
			if(PrefC.HasClinicsEnabled) {
				_listRegionDefs=Defs.GetDefsForCategory(DefCat.Regions,true);
				List<Clinic> listClinicsForUser=Clinics.GetForUserod(Security.CurUser);
				if(_listRegionDefs.Count!=0) {
					_listRegionDefs.RemoveAll(x => !listClinicsForUser.Any(y => y.Region==x.DefNum));
					listBoxVerifyRegions.Items.Add(Lan.g(this,"All"));
					for(int i = 0;i<_listRegionDefs.Count;i++) {
						listBoxVerifyRegions.Items.Add(_listRegionDefs[i].ItemName);
						if(_listDefNumsVerifyRegionsFilter.Contains(_listRegionDefs[i].DefNum)) {
							listBoxVerifyRegions.SetSelected(i+1);
						}
					}
					if(listBoxVerifyRegions.SelectedIndices.Count==0) {//Will select either "All" or the restricted clinic's region.
						listBoxVerifyRegions.SetSelected(0);
					}
				}
				else {
					listBoxVerifyRegions.Visible=false;
					labelRegion.Visible=false;
					_listDefNumsVerifyRegionsFilter=new List<long>() { -1 };
				}
				FillClinicListBox(listClinicsForUser);
			}
		}

		///<summary>Fills the listbox for clinics. Pass in a list of clinics for the current user.</summary>
		private void FillClinicListBox(List<Clinic> listClinicsForUser) {
			listBoxVerifyClinics.Items.Clear();
			int indexCur=0;
			if(_isAllClinicsEnabled) {
				//"All" will only show if the user isn't restricted to a clinic and if the enterprise preference is off
				//Even so, "All" will be restricted down to whatever region(s) are selected when getting the verification list.
				listBoxVerifyClinics.Items.Add(Lan.g(this,"All"),new Clinic { ClinicNum=-1 });
				indexCur++;
			}
			foreach(Clinic clinic in listClinicsForUser) {
				if(_isRegionSelected && !_listDefNumsVerifyRegionsFilter.Contains(clinic.Region)) {
					continue;//If a region is selected and it is not part of the region, skip it
				}
				listBoxVerifyClinics.Items.Add(clinic.Abbr,clinic);
				if(_listClinicNumsVerifyClinicsFilter.Contains(clinic.ClinicNum)) {
					listBoxVerifyClinics.SetSelected(indexCur);
				}
				indexCur++;
			}
			if(!Security.CurUser.ClinicIsRestricted && !_isRegionSelected) {//Show "Unassigned" if user is not restricted and no region is selected.
				//Add Unassigned at the bottom. ClinicNum of 0.
				listBoxVerifyClinics.Items.Add(Lan.g(this,"Unassigned"),new Clinic { ClinicNum=0 });
				if(_listClinicNumsVerifyClinicsFilter.Contains(0)) {
					listBoxVerifyClinics.SetSelected(indexCur);
				}
				indexCur++;
			}
			if(listBoxVerifyClinics.GetListSelected<Clinic>().Count==0 && listBoxVerifyClinics.Items.Count>0) {//Select the first one if none are selected.
				listBoxVerifyClinics.SetSelected(0);
			}
			//Reset the selected list because some of the previously selected clinics could've been hidden since a specific region was selected.
			UpdateSelectedClinicNums();
		}

		private void FillUsers() {
			comboVerifyUser.Items.Clear();
			comboVerifyUser.SelectedIndex=-1;
			comboVerifyUser.Items.Add(Lan.g(this,"All Users"));
			comboVerifyUser.Items.Add(Lan.g(this,"Unassigned"));
			List<long> listClinicNums=new List<long>();
			if(!_listClinicNumsVerifyClinicsFilter.Contains(-1)) {
				listClinicNums=new List<long>();
				listClinicNums.AddRange(_listClinicNumsVerifyClinicsFilter);
			}
			else if(_listDefNumsVerifyRegionsFilter.Count(x => x!=0 && x!=-1)>0) {
				//This will get every clinic associated to any of the currently selected regions.
				listClinicNums=Clinics.GetListByRegion(_listDefNumsVerifyRegionsFilter);
			}
			_listUsersInRegion=Userods.GetUsersForVerifyList(listClinicNums,true);
			_listUsersInRegionWithAssignedIns=Userods.GetUsersForVerifyList(listClinicNums,false);
			for(int i=0;i<_listUsersInRegionWithAssignedIns.Count;i++) {
				comboVerifyUser.Items.Add(_listUsersInRegionWithAssignedIns[i].UserName);
				if(_verifyUserNum==_listUsersInRegionWithAssignedIns[i].UserNum) {
					comboVerifyUser.SelectedIndex=i+2;//Add 2 because of the "All Users" and "Unassigned" combo items.
				}
			}
			if(_verifyUserNum==-1) {
				comboVerifyUser.SelectedIndex=0;//"All Users"
			}
			if(_verifyUserNum==0) {
				comboVerifyUser.SelectedIndex=1;//"Unassigned"
			}
			for(int i=0;i<_listUsersInRegion.Count;i++) {
				if(_assignUserNum==_listUsersInRegion[i].UserNum) {
					textAssignUser.Text=_listUsersInRegion[i].UserName;
				}
			}
			if(_assignUserNum==0) {
				textAssignUser.Text="Unassign";
			}
		}

		private void PickUser(bool isAssigning) {
			using FormUserPick FormUP=new FormUserPick();
			FormUP.IsSelectionmode=true;
			FormUP.ListUserodsFiltered=_listUsersInRegion;
			if(!isAssigning) {
				FormUP.IsPickAllAllowed=true;
			}
			FormUP.IsPickNoneAllowed=true;
			FormUP.ShowDialog();
			if(FormUP.DialogResult==DialogResult.OK) {
				if(isAssigning) {//Setting the user
					_assignUserNum=FormUP.SelectedUserNum;
				}
				else {//Filter by user
					_verifyUserNum=FormUP.SelectedUserNum;
				}
				FillGrids();
			}
		}

		private List<InsVerifyGridRow> GetRowsForGrid(bool isAssignGrid) {
			List<InsVerifyGridRow> listGridRows=new List<InsVerifyGridRow>();
			if(!textAppointmentScheduledDays.IsValid() 
				|| !textPatientEnrollmentDays.IsValid() 
				|| !textInsBenefitEligibilityDays.IsValid()) 
			{
				return listGridRows;
			}
			bool excludePatVerifyWhenNoIns=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
			bool excludePatClones=(PrefC.GetBool(PrefName.ShowFeaturePatientClone)==true) && PrefC.GetBool(PrefName.InsVerifyExcludePatientClones);
			DateTime dateTimeStart=DateTime.Today;
			DateTime dateTimeEnd=DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDays.Text));//Don't need to add 1 because we will be getting only the date portion of this datetime.
			if(!isAssignGrid && tabControlVerificationList.SelectedTab==tabPastDue) {
				dateTimeStart=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyDaysFromPastDueAppt));
				dateTimeEnd=DateTime.Today.AddDays(-1);
			}
			DateTime dateTimeLastPatEligibility=DateTime.Today.AddDays(-PIn.Int(textPatientEnrollmentDays.Text));
			DateTime dateTimeLastPlanBenefits=DateTime.Today.AddDays(-PIn.Int(textInsBenefitEligibilityDays.Text));
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				List<InsVerifyGridObject> listGridObjs=InsVerifies.GetVerifyGridList(dateTimeStart,dateTimeEnd,dateTimeLastPatEligibility,dateTimeLastPlanBenefits,
					_listClinicNumsVerifyClinicsFilter,_listDefNumsVerifyRegionsFilter,_defNumVerifyStatusFilter,_verifyUserNum,textVerifyCarrier.Text,
					excludePatVerifyWhenNoIns,excludePatClones);
				foreach(InsVerifyGridObject gridObjCur in listGridObjs) {
					listGridRows.Add(new InsVerifyGridRow(gridObjCur,_dictStatusDefs,_listUsersInRegion,isAssignGrid));
				}
			};
			progressOD.ShowDialogProgress();
			return listGridRows;
		}

		private int CompareGridRows(InsVerifyGridRow x,InsVerifyGridRow y) {
			if(x.Type==y.Type && x.Clinic==y.Clinic && x.NextApptDate==y.NextApptDate) {
				return x.CarrierName.CompareTo(y.CarrierName);
			}
			if(x.Type==y.Type && x.Clinic==y.Clinic) {
				return x.NextApptDate.CompareTo(y.NextApptDate);
			}
			if(x.Type==y.Type) {
				return x.Clinic.CompareTo(y.Clinic);
			}
			return y.Type.CompareTo(x.Type);//x and y are flipped to order by Type descending (Z-A)
		}

		private GridRow VerifyRowToODGridRow(InsVerifyGridRow vrow,bool isAssignGrid) {
			GridRow row=new GridRow();
			row.Cells.Add(vrow.Type);
			if(PrefC.HasClinicsEnabled) {
				row.Cells.Add(vrow.Clinic);
			}
			row.Cells.Add(vrow.PatientName);
			row.Cells.Add(vrow.NextApptDate.ToString("g"));//"g" will exclude seconds from the DateTime.
			row.Cells.Add(vrow.CarrierName);
			row.Cells.Add(vrow.DateLastVerified.ToShortDateString());
			row.Cells.Add(vrow.VerifyStatus);
			if(!isAssignGrid) {
				row.Cells.Add(vrow.DateLastAssigned.ToShortDateString());
			}
			row.Cells.Add(vrow.AssignedTo);
			row.Tag=vrow.Tag;
			return row;
		}

		#region Grid Verify
		private GridOD GetVisibleGrid(bool includeGridAssign=false) {
			if(includeGridAssign && tabControl1.SelectedTab==tabAssign) {
				return gridAssign;
			}
			if(tabControlVerificationList.SelectedTab==tabPastDue) {
				return gridPastDue;
			}
			return gridMain;
		}

		private void FillGrid() {
			//One of the following text boxes that we are about to validate may still have focus and thus has not yet been validated.
			//this.Validate() will verify the value of the control losing focus by causing the Validating and Validated events to occur, in that order.
			this.Validate();
			List<InsVerifyGridRow> listGridRows=null;
			listGridRows=GetRowsForGrid(false);
			listGridRows.Sort(CompareGridRows);
			GridOD grid=GetVisibleGrid();
			_selectedRowVerifyGrid=grid.GetSelectedIndex();
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lans.g(this,"Type"),45);
			grid.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lans.g(this,"Clinic"),90);
				grid.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lans.g(this,"Patient"),120);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Appt Date Time"),130,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Carrier"),160);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Last Verified"),90,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Status"),110);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Status Date"),80,HorizontalAlignment.Center);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Assigned to"),120){ IsWidthDynamic=true };
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			for(int i=0;i<listGridRows.Count;i++) {
				grid.ListGridRows.Add(VerifyRowToODGridRow(listGridRows[i],false));
			}
			grid.EndUpdate();
			grid.SetSelected(_selectedRowVerifyGrid,true);
		}

		private void gridMain_CellClick(object sender,UI.ODGridClickEventArgs e) {
			UpdateSelectedInfo(((InsVerifyGridObject)gridMain.ListGridRows[e.Row].Tag));
		}

		private void gridPastDue_CellClick(object sender,UI.ODGridClickEventArgs e) {
			UpdateSelectedInfo(((InsVerifyGridObject)gridPastDue.ListGridRows[e.Row].Tag));
		}

		private void UpdateSelectedInfo(InsVerifyGridObject obj) {
			_gridRowSelected=obj;
			if(_gridRowSelected.IsOnlyInsRow()) {
				butVerifyPlan.Enabled=true;
				butVerifyPat.Enabled=false;
			}
			else if(_gridRowSelected.IsOnlyPatRow()) {
				butVerifyPlan.Enabled=false;
				butVerifyPat.Enabled=true;
			}
			else {
				butVerifyPlan.Enabled=true;
				butVerifyPat.Enabled=true;
			}
			FillDisplayInfo(_gridRowSelected);
		}
		#endregion

		#region Verify Logic

		private void butVerifyPlan_Click(object sender,EventArgs e) {
			OnVerify(PlanToVerify.InsuranceBenefits);
		}

		private void butVerifyPat_Click(object sender,EventArgs e) {
			OnVerify(PlanToVerify.PatientEligibility);
		}

		private void OnVerify(PlanToVerify planToVerifyEnum) {
			GridOD grid=GetVisibleGrid();
			if(grid.SelectedIndices.Length<1) {
				MsgBox.Show(this,"Please select an insurance to verify.");
				return;
			}
			InsVerifyGridObject selectedRowObject=((InsVerifyGridObject)grid.ListGridRows[grid.GetSelectedIndex()].Tag);
			if((planToVerifyEnum==PlanToVerify.Both && !selectedRowObject.IsPatAndInsRow()) 
				|| (planToVerifyEnum==PlanToVerify.PatientEligibility && selectedRowObject.PatInsVerify==null) 
				|| (planToVerifyEnum==PlanToVerify.InsuranceBenefits && selectedRowObject.PlanInsVerify==null)) 
			{
				//This will only happen if somehow the selected grid row differed from the passed in PlanToVerify.
				MsgBox.Show(this,"Something went wrong with your selection.  Click on the refresh button and try again.");
				return;
			}
			string verifyType="";
			if(planToVerifyEnum==PlanToVerify.Both) {
				verifyType="patient eligibility and insurance benefits";
			}
			else if(planToVerifyEnum==PlanToVerify.PatientEligibility) {
				verifyType="patient eligibility";
			}
			else if(planToVerifyEnum==PlanToVerify.InsuranceBenefits) {
				verifyType="insurance benefits";
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you want to verify the selected "+verifyType+"?")) {
				return;
			}
			if(planToVerifyEnum==PlanToVerify.Both || planToVerifyEnum==PlanToVerify.PatientEligibility) {
				selectedRowObject.PatInsVerify=InsVerifies.SetTimeAvailableForVerify(selectedRowObject.PatInsVerify,
					PlanToVerify.PatientEligibility,PIn.Int(textAppointmentScheduledDays.Text),PIn.Int(textPatientEnrollmentDays.Text),
					PIn.Int(textInsBenefitEligibilityDays.Text));
				selectedRowObject.PatInsVerify.DateLastVerified=DateTime.Today;
				InsVerifyHists.InsertFromInsVerify(selectedRowObject.PatInsVerify);
			}
			if(planToVerifyEnum==PlanToVerify.Both || planToVerifyEnum==PlanToVerify.InsuranceBenefits) {
				selectedRowObject.PlanInsVerify=InsVerifies.SetTimeAvailableForVerify(selectedRowObject.PlanInsVerify,
					PlanToVerify.InsuranceBenefits,PIn.Int(textAppointmentScheduledDays.Text),PIn.Int(textPatientEnrollmentDays.Text),
					PIn.Int(textInsBenefitEligibilityDays.Text));
				selectedRowObject.PlanInsVerify.DateLastVerified=DateTime.Today;
				InsVerifyHists.InsertFromInsVerify(selectedRowObject.PlanInsVerify);
			}
			FillDisplayInfo(null);
			FillGrids();
		}

		private void gridContextMenuStrip_Popup(object sender,EventArgs e) {
			menuRightClick.MenuItems.Clear();
			GridOD grid=GetVisibleGrid(true);
			if(grid.GetSelectedIndex()==-1) {
				return;
			}
			if(grid==gridAssign) {
				gridAssign_Popup(sender,e);
				return;
			}
			//The _gridRowSelected needs to get updated before we run our menu item logic because Popup fires before CellClick.
			_gridRowSelected=(InsVerifyGridObject)grid.ListGridRows[grid.GetSelectedIndex()].Tag;
			menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Go to Patient"),gridMainRight_click));
			string verifyDescription=Lan.g(this,"Go to Insurance Plan");
			if(_gridRowSelected.PatInsVerify!=null) {
				verifyDescription=Lan.g(this,"Go to Patient Plan");
			}
			menuRightClick.MenuItems.Add(new MenuItem(verifyDescription,gridMainRight_click));
			MenuItem assignUserToolItem=new MenuItem(Lan.g(this,"Assign to User"));
			foreach(Userod user in _listUsersInRegion) {
				MenuItem assignUserDropDownCur=new MenuItem(user.UserName);
				assignUserDropDownCur.Tag=user;
				assignUserDropDownCur.Click+=new EventHandler(assignUserToolItemDropDown_Click);
				assignUserToolItem.MenuItems.Add(assignUserDropDownCur);
			}
			menuRightClick.MenuItems.Add(assignUserToolItem);
			MenuItem verifyStatusToolItem=new MenuItem(Lan.g(this,"Set Verify Status to"));
			foreach(Def status in _listVerifyStatuses) {
				MenuItem verifyStatusDropDownCur=new MenuItem(status.ItemName);
				verifyStatusDropDownCur.Tag=status;
				verifyStatusDropDownCur.Click+=new EventHandler(verifyStatusToolItemDropDown_Click);
				verifyStatusToolItem.MenuItems.Add(verifyStatusDropDownCur);
			}
			menuRightClick.MenuItems.Add(verifyStatusToolItem);
			if(_gridRowSelected.IsPatAndInsRow()) {
				menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Patient Eligibility"),gridMainRight_click));//Number 3 in gridMainRight_click
				menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Insurance Benefits"),gridMainRight_click));//Number 4 in gridMainRight_click
				menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Both"),gridMainRight_click));//Number 5 in gridMainRight_click
			}
			else if(_gridRowSelected.IsOnlyPatRow()) {
				menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Patient Eligibility"),gridMainRight_click));//Number 3 in gridMainRight_click
			}
			else if(_gridRowSelected.IsOnlyInsRow()) {
				menuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Insurance Benefits"),gridMainRight_click));//Number 3 in gridMainRight_click
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OnOpenInsPlan();
		}

		private void gridPastDue_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			OnOpenInsPlan();
		}

		private void OnOpenInsPlan() {
			//This returns the PatPlanNum if patInsVerify is not null, else the planInsVerify if it's not null. Returns 0 if both are null.
			long patPlanNum=_gridRowSelected.GetPatPlanNum();
			if(patPlanNum==0) {				
				return;
			}
			PatPlan pp=null;
			InsSub insSub=null;
			InsPlan ip=null;
			if(_gridRowSelected.PatInsVerify!=null) {				
				pp=PatPlans.GetByPatPlanNum(patPlanNum);
				if(pp==null) {
					MsgBox.Show(this,"The selected patient plan cannot be found.");
					//Refresh grids so we have more up to date data.
					FillGrids();
					return;
				}
				insSub=InsSubs.GetOne(pp.InsSubNum);
				if(insSub==null) {
					MsgBox.Show(this,"The selected patient's subscriber cannot be found.");
					//Refresh grids so we have more up to date data.
					FillGrids();
					return;
				}
				ip=InsPlans.GetPlan(insSub.PlanNum,new List<InsPlan>());
			}
			else if(_gridRowSelected.PlanInsVerify!=null) {
				ip=InsPlans.GetPlan(_gridRowSelected.PlanInsVerify.FKey,new List<InsPlan>());				
			}
			if(ip==null) {
				MsgBox.Show(this,"The selected insurance plan cannot be found.");
				//Refresh grids so we have more up to date data.
				FillGrids();
				return;
			}
			using FormInsPlan FormIP=new FormInsPlan(ip,pp,insSub);
			FormIP.ShowDialog();
			if(FormIP.DialogResult==DialogResult.OK) {
				FillGrids();
			}
		}

		private void gridMainRight_click(object sender,System.EventArgs e) {
			switch(menuRightClick.MenuItems.IndexOf((MenuItem)sender)) {
				case 0:
					GotoModule.GotoFamily(_gridRowSelected.GetPatNum());
					break;
				case 1:
					OnOpenInsPlan();
					break;
				case 2:
					//No need for action on Assign click
					break;
				case 3:
					break;
				case 4:
					if(_gridRowSelected.IsOnlyInsRow()) {
						OnVerify(PlanToVerify.InsuranceBenefits);
					}
					else {
						OnVerify(PlanToVerify.PatientEligibility);//If both or only pat, then 3 will be patient eligibility verification
					}
					break;
				case 5:
					OnVerify(PlanToVerify.InsuranceBenefits);//This will only be visible if selecting a row with both ins and pat
					break;
				case 6:
					OnVerify(PlanToVerify.Both);//This will only be visible if selecting a row with both ins and pat
					break;
			}
		}
		#endregion

		#region Grid Assign
		private void FillGridAssign() {
			//One of the following text boxes that we are about to validate may still have focus and thus has not yet been validated.
			//this.Validate() will verify the value of the control losing focus by causing the Validating and Validated events to occur, in that order.
			this.Validate();
			List<InsVerifyGridRow> listGridRows=null;
			listGridRows=GetRowsForGrid(isAssignGrid:true);
			listGridRows.Sort(CompareGridRows);
			_selectedRowAssignGrid=gridAssign.GetSelectedIndex();
			gridAssign.BeginUpdate();
			gridAssign.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lans.g(this,"Type"),45);
			gridAssign.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lans.g(this,"Clinic"),90);
				gridAssign.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lans.g(this,"Patient"),120);
			gridAssign.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Appt Date Time"),130,GridSortingStrategy.DateParse);
			col.TextAlign=HorizontalAlignment.Center;
			gridAssign.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Carrier"),160);
			gridAssign.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Last Verified"),90,GridSortingStrategy.DateParse);
			col.TextAlign=HorizontalAlignment.Center;
			gridAssign.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Status"),110);
			gridAssign.ListGridColumns.Add(col);
			col=new GridColumn(Lans.g(this,"Assigned to"),120){ IsWidthDynamic=true };
			gridAssign.ListGridColumns.Add(col);
			gridAssign.ListGridRows.Clear();
			for(int i=0;i<listGridRows.Count;i++) {
				gridAssign.ListGridRows.Add(VerifyRowToODGridRow(listGridRows[i],true));
			}
			gridAssign.EndUpdate();
			gridAssign.SetSelected(_selectedRowAssignGrid,true);
		}

		private List<InsVerifyGridObject> GetSelectedInsVerifyList() {
			List<InsVerifyGridObject> selectedGridObjectRows=new List<InsVerifyGridObject>();
			for(int i=0;i<gridAssign.SelectedIndices.Length;i++) {
				selectedGridObjectRows.Add(((InsVerifyGridObject)gridAssign.ListGridRows[gridAssign.SelectedIndices[i]].Tag));
			}
			return selectedGridObjectRows;
		}
		#endregion

		#region Assigning Logic
		private void butAssignUser_Click(object sender,EventArgs e) {
			if(gridAssign.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an insurance to assign.");
				return;
			}
			if(_assignUserNum==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to unassign the selected plan?")) {
					return;
				}
			}
			List<InsVerifyGridObject> listRowsSelected=GetSelectedInsVerifyList();
			foreach(InsVerifyGridObject gridRowObject in listRowsSelected) {
				if(gridRowObject.PatInsVerify!=null) {
					gridRowObject.PatInsVerify.UserNum=_assignUserNum;
					gridRowObject.PatInsVerify.Note=textInsVerifyNote.Text;
					gridRowObject.PatInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(gridRowObject.PatInsVerify);
				}
				if(gridRowObject.PlanInsVerify!=null) {
					gridRowObject.PlanInsVerify.UserNum=_assignUserNum;
					gridRowObject.PlanInsVerify.Note=textInsVerifyNote.Text;
					gridRowObject.PlanInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(gridRowObject.PlanInsVerify);
				}
			}
			FillGrids();
		}

		private void gridAssign_Popup(object sender,EventArgs e) {
			MenuItem assignUserToolItem=new MenuItem(Lan.g(this,"Assign to User"));
			foreach(Userod user in _listUsersInRegion) {
				MenuItem assignUserDropDownCur=new MenuItem(user.UserName);
				assignUserDropDownCur.Tag=user;
				assignUserDropDownCur.Click+=new EventHandler(assignUserToolItemDropDown_Click);
				assignUserToolItem.MenuItems.Add(assignUserDropDownCur);
			}
			menuRightClick.MenuItems.Add(assignUserToolItem);
			MenuItem verifyStatusToolItem=new MenuItem(Lan.g(this,"Set Verify Status to"));
			foreach(Def status in _listVerifyStatuses) {
				MenuItem verifyStatusDropDownCur=new MenuItem(status.ItemName);
				verifyStatusDropDownCur.Tag=status;
				verifyStatusDropDownCur.Click+=new EventHandler(verifyStatusToolItemDropDown_Click);
				verifyStatusToolItem.MenuItems.Add(verifyStatusDropDownCur);
			}
			menuRightClick.MenuItems.Add(verifyStatusToolItem);
		}

		private void gridAssignRight_click(object sender,System.EventArgs e) {
			switch(menuRightClick.MenuItems.IndexOf((MenuItem)sender)) {
				case 0:
					if(_assignUserNum==0) {
						if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to unassign the selected plan?")) {
							return;
						}
					}
					List<InsVerifyGridObject> listRowsSelected=GetSelectedInsVerifyList();
					foreach(InsVerifyGridObject gridRowObject in listRowsSelected) {
						if(gridRowObject.PatInsVerify!=null) {
							gridRowObject.PatInsVerify.UserNum=_assignUserNum;
							gridRowObject.PatInsVerify.DateLastAssigned=DateTime.Today;
							InsVerifies.Update(gridRowObject.PatInsVerify);
						}
						if(gridRowObject.PlanInsVerify!=null) {
							gridRowObject.PlanInsVerify.UserNum=_assignUserNum;
							gridRowObject.PlanInsVerify.DateLastAssigned=DateTime.Today;
							InsVerifies.Update(gridRowObject.PlanInsVerify);
						}
					}
					break;
				case 1:
					//Not clickable due to being a dropdown menu.
					break;
			}
			FillGrids();
		}

		private void butAssignUserPick_Click(object sender,EventArgs e) {
			PickUser(true);
		}

		private void comboSetVerifyStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSetVerifyStatus.SelectedIndex<1) {
				_defNumVerifyStatusAssign=0;
				comboSetVerifyStatus.Text="none";
			}
			else {
				_defNumVerifyStatusAssign=_listVerifyStatuses[comboSetVerifyStatus.SelectedIndex-1].DefNum;
				comboSetVerifyStatus.Text=_listVerifyStatuses[comboSetVerifyStatus.SelectedIndex-1].ItemName;
			}
			if(gridMain.GetSelectedIndex()!=-1 || gridPastDue.GetSelectedIndex()!=-1) {//Both grids cannot have a selection at the same time
				SetStatus(_defNumVerifyStatusAssign,true);
			}
			FillGrids();
		}
		#endregion

		#region Grid Filters
		private void butVerifyUserPick_Click(object sender,EventArgs e) {
			PickUser(false);
			FillGrids();
		}

		private void comboFilterVerifyStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboFilterVerifyStatus.SelectedIndex<1) {
				_defNumVerifyStatusFilter=0;
			}
			else {
				_defNumVerifyStatusFilter=_listVerifyStatuses[comboFilterVerifyStatus.SelectedIndex-1].DefNum;
			}
			FillGrids();
		}

		///<summary>Firing on MouseUp allows time for the user to select multiple values via "dragging"</summary>
		private void listBoxVerifyClinics_MouseUp(object sender,MouseEventArgs e) {
			UpdateSelectedClinicNums();
			FillGrids();
		}

		///<summary>Firing on MouseUp allows time for the user to select multiple values via "dragging"</summary>
		private void listBoxVerifyRegions_MouseUp(object sender,MouseEventArgs e) {
			_listDefNumsVerifyRegionsFilter=new List<long>();
			for(int i=0;i<listBoxVerifyRegions.SelectedIndices.Count;i++) {
				if(listBoxVerifyRegions.SelectedIndices[i]<1) {
					_listDefNumsVerifyRegionsFilter.Add(-1);
				}
				else {
					_listDefNumsVerifyRegionsFilter.Add(_listRegionDefs[listBoxVerifyRegions.SelectedIndices[i]-1].DefNum);
				}
			}
			//Has "All" selected as well as specific clinics
			if((_listDefNumsVerifyRegionsFilter.Contains(-1) || _listDefNumsVerifyRegionsFilter.Contains(0)) 
				&& _listDefNumsVerifyRegionsFilter.Count(x => x!=0 && x!=-1)>0) 
			{
				//Remove "All" from the list because specific clinics are selected and the listbox can't have "All" selected as the same time as a specific clinic for the listbox.
				_listDefNumsVerifyRegionsFilter.Remove(-1);
				_listDefNumsVerifyRegionsFilter.Remove(0);
			}
			//Add "All" if the user tried deselecting the only selected item in the listbox.
			if(_listDefNumsVerifyRegionsFilter.Count==0) {
				_listDefNumsVerifyRegionsFilter.Add(-1);
			}
			FillGrids();
		}

		///<summary>Updates the global list of selected clinicNums from listBoxVerifyClinics.</summary>
		private void UpdateSelectedClinicNums() {
			_listClinicNumsVerifyClinicsFilter=listBoxVerifyClinics.GetListSelected<Clinic>().Select(x => x.ClinicNum).ToList();
			//If they have all and something else selected, remove all.
			if(_listClinicNumsVerifyClinicsFilter.Contains(-1) && _listClinicNumsVerifyClinicsFilter.Count > 1) {
				_listClinicNumsVerifyClinicsFilter.Remove(-1);
			}
			//Add "All" (or "Unassigned" if "All" is not enabled) if the user tried deselecting the only selected item in the listbox.
			if(_listClinicNumsVerifyClinicsFilter.Count==0) {
				_listClinicNumsVerifyClinicsFilter.Add(_isAllClinicsEnabled ? -1 : 0);
			}
		}

		private void comboVerifyUser_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboVerifyUser.SelectedIndex==1) {//Selected "Unassigned"
				_verifyUserNum=0;
			}
			else if(comboVerifyUser.SelectedIndex<1) {//Selected "All Users" or selected index is invalid
				_verifyUserNum=-1;
			}
			else {//Selected a real User.
				_verifyUserNum=_listUsersInRegionWithAssignedIns[comboVerifyUser.SelectedIndex-2].UserNum;
			}
			FillGrids();
		}

		#endregion

		private void SetStatus(long statusDefNum,bool isVerifyGrid) {
			string statusNote="";
			bool hasChanged=false;
			using InputBox ib=new InputBox(Lan.g(this,"Add a status note:"),true);
			ib.setTitle(Lan.g(this,"Add Status Note"));
			if(!isVerifyGrid) {
				ib.textResult.Text=textInsVerifyNote.Text;
			}
			ib.ShowDialog();
			if(ib.DialogResult==DialogResult.OK) {
				statusNote=ib.textResult.Text;
				hasChanged=true;
			}
			if(isVerifyGrid) {
				if(_gridRowSelected.PatInsVerify!=null) {
					_gridRowSelected.PatInsVerify.DefNum=statusDefNum;
					if(hasChanged) {
						_gridRowSelected.PatInsVerify.Note=statusNote;
					}
					_gridRowSelected.PatInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_gridRowSelected.PatInsVerify);
				}
				if(_gridRowSelected.PlanInsVerify!=null) {
					_gridRowSelected.PlanInsVerify.DefNum=statusDefNum;
					if(hasChanged) {
						_gridRowSelected.PlanInsVerify.Note=statusNote;
						textInsVerifyReadOnlyNote.Text=statusNote;
					}
					_gridRowSelected.PlanInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_gridRowSelected.PlanInsVerify);
				}
			}
			else {
				List<InsVerifyGridObject> listRowsSelected=GetSelectedInsVerifyList();
				foreach(InsVerifyGridObject gridRowObject in listRowsSelected) {
					if(gridRowObject.PatInsVerify!=null) {
						gridRowObject.PatInsVerify.DefNum=statusDefNum;
						if(hasChanged) {
							gridRowObject.PatInsVerify.Note=statusNote;
						}
						gridRowObject.PatInsVerify.DateLastAssigned=DateTime.Today;
						InsVerifies.Update(gridRowObject.PatInsVerify);
					}
					if(gridRowObject.PlanInsVerify!=null) {
						gridRowObject.PlanInsVerify.DefNum=statusDefNum;
						if(hasChanged) {
							gridRowObject.PlanInsVerify.Note=statusNote;
						}
						gridRowObject.PlanInsVerify.DateLastAssigned=DateTime.Today;
						InsVerifies.Update(gridRowObject.PlanInsVerify);
					}
				}
			}
		}

		private void verifyStatusToolItemDropDown_Click(object sender, EventArgs e) {
			Def status=(Def)((MenuItem)sender).Tag;
			if(tabControl1.SelectedTab==tabVerify) {
				SetStatus(status.DefNum,true);
			}
			if(tabControl1.SelectedTab==tabAssign) {
				SetStatus(status.DefNum,false);
			}
			FillGrids();
		}

		private void assignUserToolItemDropDown_Click(object sender, EventArgs e) {
			Userod user=(Userod)((MenuItem)sender).Tag;
			if(tabControl1.SelectedTab==tabVerify) {
				if(_gridRowSelected.PatInsVerify!=null) {
					_gridRowSelected.PatInsVerify.UserNum=user.UserNum;
					_gridRowSelected.PatInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_gridRowSelected.PatInsVerify);
				}
				if(_gridRowSelected.PlanInsVerify!=null) {
					_gridRowSelected.PlanInsVerify.UserNum=user.UserNum;
					_gridRowSelected.PlanInsVerify.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_gridRowSelected.PlanInsVerify);
				}
			}
			if(tabControl1.SelectedTab==tabAssign) {
				List<InsVerifyGridObject> listRowsSelected=GetSelectedInsVerifyList();
				foreach(InsVerifyGridObject gridRowObject in listRowsSelected) {
					if(gridRowObject.PatInsVerify!=null) {
						gridRowObject.PatInsVerify.UserNum=user.UserNum;
						gridRowObject.PatInsVerify.DateLastAssigned=DateTime.Today;
						InsVerifies.Update(gridRowObject.PatInsVerify);
					}
					if(gridRowObject.PlanInsVerify!=null) {
						gridRowObject.PlanInsVerify.UserNum=user.UserNum;
						gridRowObject.PlanInsVerify.DateLastAssigned=DateTime.Today;
						InsVerifies.Update(gridRowObject.PlanInsVerify);
					}
				}
			}
			FillGrids();
		}

		private void tabControl1_Selected(object sender,TabControlEventArgs e) {
			if(e.TabPage==tabAssign) {
				if(_verifyUserNum!=0) {
					_userNumVerifyGrid=_verifyUserNum;
					_verifyUserNum=0;//Set filter user to Unassigned when switching to Assign tab.=
				}
			}
			else if(e.TabPage==tabVerify) {
				if(_verifyUserNum==0) {
					_verifyUserNum=_userNumVerifyGrid;
				}
			}
			FillGrids();
		}

		private void tabControlVerificationList_Selected(object sender,TabControlEventArgs e) {
			FillDisplayInfo(null);
			gridMain.SetAll(false);
			gridPastDue.SetAll(false);
			FillGrids();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		///<summary>This represents a row in either the Verification List Grid, or the Assignment Grid.  The assignment grid won't use the DateLastAssigned variable.</summary>
		private class InsVerifyGridRow {
			public string Type;
			public string Clinic;
			public string PatientName;
			public DateTime NextApptDate;
			public string CarrierName;
			public DateTime DateLastVerified;
			public string VerifyStatus;
			//DateLastAssigned isn't used if IsAssignGrid=true
			public DateTime DateLastAssigned;
			public string AssignedTo;
			public Object Tag;
			
			public bool IsAssignGrid;

			///<summary>An updated dictionary of status defs should be passed in.  
			///This is to avoid grabbing definitions cache from inside this nested class, which will be instanced in a loop.</summary>
			public InsVerifyGridRow(InsVerifyGridObject gridObj,Dictionary<long,Def> dictStatusDefs,List<Userod> listUsers,bool isAssignGrid) {
				if(gridObj==null) {
					return;
				}
				Type="";
				Clinic="";
				PatientName="";
				CarrierName="";
				VerifyStatus="";
				AssignedTo="";
				IsAssignGrid=isAssignGrid;
				if(gridObj.IsPatAndInsRow()) {//If showing a consolidated row, use the PatInsVerify information, since they are same anyways.
					Type="Pat/Ins";
					Clinic=gridObj.PatInsVerify.ClinicName;
					PatientName=gridObj.PatInsVerify.PatientName;
					NextApptDate=gridObj.PatInsVerify.AppointmentDateTime;
					CarrierName=gridObj.PatInsVerify.CarrierName;
					//Get the oldest DateLastVerified
					DateLastVerified=(gridObj.PatInsVerify.DateLastVerified<=gridObj.PlanInsVerify.DateLastVerified ? 
						gridObj.PatInsVerify.DateLastVerified : 
						gridObj.PlanInsVerify.DateLastVerified);
					if(dictStatusDefs.ContainsKey(gridObj.PatInsVerify.DefNum)) {
						VerifyStatus=dictStatusDefs[gridObj.PatInsVerify.DefNum].ItemName;
					}
					bool isPatLastAssignedNewer=gridObj.PatInsVerify.DateLastAssigned>=gridObj.PlanInsVerify.DateLastAssigned;
					//Get the most recent DateLastAssigned
					DateLastAssigned=(isPatLastAssignedNewer ? 
						gridObj.PatInsVerify.DateLastAssigned : 
						gridObj.PlanInsVerify.DateLastAssigned);
					if(isPatLastAssignedNewer) {
						Userod userCur=listUsers.FirstOrDefault(x => x.UserNum==gridObj.PatInsVerify.UserNum);
						if(userCur!=null) {
							AssignedTo=userCur.UserName;
						}
					}
					else {
						Userod userCur=listUsers.FirstOrDefault(x => x.UserNum==gridObj.PlanInsVerify.UserNum);
						if(userCur!=null) {
							AssignedTo=userCur.UserName;
						}
					}
					Tag=gridObj;
				}
				else if(gridObj.IsOnlyPatRow()) {
					Type="Pat";
					Clinic=gridObj.PatInsVerify.ClinicName;
					PatientName=gridObj.PatInsVerify.PatientName;
					NextApptDate=gridObj.PatInsVerify.AppointmentDateTime;
					CarrierName=gridObj.PatInsVerify.CarrierName;
					DateLastVerified=gridObj.PatInsVerify.DateLastVerified;
					if(dictStatusDefs.ContainsKey(gridObj.PatInsVerify.DefNum)) {
						VerifyStatus=dictStatusDefs[gridObj.PatInsVerify.DefNum].ItemName;
					}
					DateLastAssigned=gridObj.PatInsVerify.DateLastAssigned;
					Userod userCur=listUsers.FirstOrDefault(x => x.UserNum==gridObj.PatInsVerify.UserNum);
					if(userCur!=null) {
						AssignedTo=userCur.UserName;
					}
					Tag=gridObj;
				}
				else if(gridObj.IsOnlyInsRow()) {
					Type="Ins";
					Clinic=gridObj.PlanInsVerify.ClinicName;
					PatientName=gridObj.PlanInsVerify.PatientName;
					NextApptDate=gridObj.PlanInsVerify.AppointmentDateTime;
					CarrierName=gridObj.PlanInsVerify.CarrierName;
					DateLastVerified=gridObj.PlanInsVerify.DateLastVerified;
					if(dictStatusDefs.ContainsKey(gridObj.PlanInsVerify.DefNum)) {
						VerifyStatus=dictStatusDefs[gridObj.PlanInsVerify.DefNum].ItemName;
					}
					DateLastAssigned=gridObj.PlanInsVerify.DateLastAssigned;
					Userod userCur=listUsers.FirstOrDefault(x => x.UserNum==gridObj.PlanInsVerify.UserNum);
					if(userCur!=null) {
						AssignedTo=userCur.UserName;
					}
					Tag=gridObj;
				}
			}
		}

	}
}