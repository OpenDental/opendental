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
		private long _userNumVerify=-1;
		///<summary>0 represents "Unassign".</summary>
		private long _userNumAssign;
		///<summary>-1 represents "All", and 0 represents "Unassigned".</summary>
		private List<long> _listClinicNumsVerifyClinicsFilter=new List<long>();
		///<summary>-1 and 0 represent "All".</summary>
		private List<long> _listDefNumsVerifyRegionsFilter=new List<long>() { -1 };
		private long _defNumVerifyStatusFilter;
		private long _defNumVerifyStatusAssign;
		///<summary>This will only have a selection if selecting from gridMain.</summary>
		private InsVerifyGridObject _insVerifyGridObjectRowSelected;
		private List<Userod> _listUserodsInRegionWithAssignedIns=new List<Userod>();
		/// <summary>This will contain all (including hidden) users with the InsPlanVerify permission</summary>
		private List<Userod> _listUserodsInRegion=new List<Userod>();
		private List<Def> _listDefsVerifyStatuses=new List<Def>();
		private long _userNumVerifyGrid=0;
		private Dictionary<long,Def> _dictionaryDefsStatus=new Dictionary<long,Def>();
		private ContextMenu _contextMenuRightClick=new ContextMenu();
		private List<Def> _listDefsRegion;
		///<summary>Indicates if a region is selected in the region listbox.</summary>

		public FormInsVerificationList() {
			InitializeComponent();
			InitializeLayoutManager();
			_contextMenuRightClick.Popup+=gridContextMenuStrip_Popup;
			gridMain.ContextMenu=_contextMenuRightClick;
			gridPastDue.ContextMenu=_contextMenuRightClick;
			gridAssignStandard.ContextMenu=_contextMenuRightClick;
			gridAssignMedicaid.ContextMenu=_contextMenuRightClick;
			Lan.F(this);
		}

		private void FormInsVerificationList_Load(object sender,EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				_listClinicNumsVerifyClinicsFilter.Add(-1);//-1 represents "All"
			}
			SetFilterControlsAndAction(() => FillControls(),
				textPatientEnrollmentDaysStandard,textInsBenefitEligibilityDaysStandard,textAppointmentScheduledDaysStandard,textVerifyCarrier,
				textPatientEnrollmentDaysMedicaid,textInsBenefitEligibilityDaysMedicaid,textAppointmentScheduledDaysMedicaid);
			if(PrefC.GetBool(PrefName.InsVerifyDefaultToCurrentUser)) {
				_userNumVerify=Security.CurUser.UserNum;
			}
			if(!PrefC.HasClinicsEnabled) {
				labelClinic.Visible=false;
				listBoxVerifyClinics.Visible=false;
				labelRegion.Visible=false;
				listBoxVerifyRegions.Visible=false;
			}
			List<Def> listDefsVerifyStatuses=Defs.GetDefsForCategory(DefCat.InsuranceVerificationStatus,true);
			for(int i=0;i<listDefsVerifyStatuses.Count;i++) {
				if(!_dictionaryDefsStatus.ContainsKey(listDefsVerifyStatuses[i].DefNum)) {
					_dictionaryDefsStatus.Add(listDefsVerifyStatuses[i].DefNum,listDefsVerifyStatuses[i]);
				}
			}
			textAppointmentScheduledDaysStandard.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDays));
			textInsBenefitEligibilityDaysStandard.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDays));
			textPatientEnrollmentDaysStandard.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDays));
			textAppointmentScheduledDaysMedicaid.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyAppointmentScheduledDaysMedicaid));
			textInsBenefitEligibilityDaysMedicaid.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyBenefitEligibilityDaysMedicaid));
			textPatientEnrollmentDaysMedicaid.Text=POut.Int(PrefC.GetInt(PrefName.InsVerifyPatientEnrollmentDaysMedicaid));
			InsVerifies.CleanupInsVerifyRows(DateTime.Today,
				DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDaysStandard.Text)),
				DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDaysMedicaid.Text))
			);
		}

		private void FormInsVerificationList_Shown(object sender,EventArgs e) {
			//This must be in Shown due to the progress bar forcing this window behind other windows sometimes.
			FillControls();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillControls();
		}

		public void FillControls() {
			FillComboBoxes();
			FillUsers();
			FillGrid();
		}

		private void FillDisplayInfo(InsVerifyGridObject insVerifyGridObject) {
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
			if(insVerifyGridObject==null) {
				return;
			}
			PatPlan patPlanVerify=PatPlans.GetByPatPlanNum(insVerifyGridObject.GetPatPlanNum());
			if(patPlanVerify==null) {//Should never happen, but if it does, return because it is just for display purposes.
				return;
			}
			InsSub insSubVerify=InsSubs.GetOne(patPlanVerify.InsSubNum);
			textSubscriberID.Text=insSubVerify.SubscriberID;
			Patient patientSubscriberVerify=Patients.GetPat(insSubVerify.Subscriber);
			if(patientSubscriberVerify!=null) {
				textSubscriberName.Text=patientSubscriberVerify.GetNameFL();
				textSubscriberBirthdate.Text=patientSubscriberVerify.Birthdate.ToShortDateString();
				textSubscriberSSN.Text=patientSubscriberVerify.SSN;
			}
			FillInsuranceDisplay(InsPlans.GetPlan(insSubVerify.PlanNum,null));
			textPatBirthdate.Text=Patients.GetPat(insVerifyGridObject.GetPatNum()).Birthdate.ToShortDateString();
			if(insVerifyGridObject.IsOnlyInsRow()) {
				textInsVerifyReadOnlyNote.Text=_insVerifyGridObjectRowSelected.InsVerifyPlan.Note;
				return;
			}
			//Both or Pat row
			textInsVerifyReadOnlyNote.Text=_insVerifyGridObjectRowSelected.InsVerifyPat.Note;
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
			_listDefsVerifyStatuses=Defs.GetDefsForCategory(DefCat.InsuranceVerificationStatus,true);
			for(int i=0;i<_listDefsVerifyStatuses.Count;i++) {
				comboFilterVerifyStatus.Items.Add(_listDefsVerifyStatuses[i].ItemName);
				comboSetVerifyStatus.Items.Add(_listDefsVerifyStatuses[i].ItemName);
				if(_listDefsVerifyStatuses[i].DefNum==_defNumVerifyStatusFilter) {
					comboFilterVerifyStatus.SelectedIndex=i+1;
				}
				if(_listDefsVerifyStatuses[i].DefNum==_defNumVerifyStatusAssign) {
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
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			_listDefsRegion=Defs.GetDefsForCategory(DefCat.Regions,true);
			List<Clinic> listClinicsForUser=Clinics.GetForUserod(Security.CurUser);
			if(_listDefsRegion.Count!=0) {
				_listDefsRegion.RemoveAll(x => !listClinicsForUser.Any(y => y.Region==x.DefNum));
				listBoxVerifyRegions.Items.Add(Lan.g(this,"All"));
				for(int i = 0;i<_listDefsRegion.Count;i++) {
					listBoxVerifyRegions.Items.Add(_listDefsRegion[i].ItemName);
					if(_listDefNumsVerifyRegionsFilter.Contains(_listDefsRegion[i].DefNum)) {
						listBoxVerifyRegions.SetSelected(i+1);
					}
				}
				if(listBoxVerifyRegions.SelectedIndices.Count==0) {//Will select either "All" or the restricted clinic's region.
					listBoxVerifyRegions.SetSelected(0);
				}
				FillClinicListBox(listClinicsForUser);
				return;
			}
			listBoxVerifyRegions.Visible=false;
			labelRegion.Visible=false;
			_listDefNumsVerifyRegionsFilter=new List<long>() { -1 };
			FillClinicListBox(listClinicsForUser);
		}

		///<summary>Fills the listbox for clinics. Pass in a list of clinics for the current user.</summary>
		private void FillClinicListBox(List<Clinic> listClinicsForUser) {
			listBoxVerifyClinics.Items.Clear();
			int index=0;
			bool isAllClinicsEnabled=PrefC.HasClinicsEnabled && !Security.CurUser.ClinicIsRestricted && !PrefC.GetBool(PrefName.EnterpriseApptList);
			if(isAllClinicsEnabled) {
				//"All" will only show if the user isn't restricted to a clinic and if the enterprise preference is off
				//Even so, "All" will be restricted down to whatever region(s) are selected when getting the verification list.
				listBoxVerifyClinics.Items.Add(Lan.g(this,"All"),new Clinic { ClinicNum=-1 });
				index++;
			}
			bool isRegionSelected=!_listDefNumsVerifyRegionsFilter.Contains(0) && !_listDefNumsVerifyRegionsFilter.Contains(-1);
			for(int i=0;i<listClinicsForUser.Count;i++) {
				if(isRegionSelected && !_listDefNumsVerifyRegionsFilter.Contains(listClinicsForUser[i].Region)) {
					continue;//If a region is selected and it is not part of the region, skip it
				}
				listBoxVerifyClinics.Items.Add(listClinicsForUser[i].Abbr, listClinicsForUser[i]);
				if(_listClinicNumsVerifyClinicsFilter.Contains(listClinicsForUser[i].ClinicNum)){
					listBoxVerifyClinics.SetSelected(index);
				}
				index++;
			}
			if(!Security.CurUser.ClinicIsRestricted && !isRegionSelected) {//Show "Unassigned" if user is not restricted and no region is selected.
				//Add Unassigned at the bottom. ClinicNum of 0.
				listBoxVerifyClinics.Items.Add(Lan.g(this,"Unassigned"),new Clinic { ClinicNum=0 });
				if(_listClinicNumsVerifyClinicsFilter.Contains(0)) {
					listBoxVerifyClinics.SetSelected(index);
				}
				index++;
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
			_listUserodsInRegion=Userods.GetUsersForVerifyList(listClinicNums,isAssigning:true,includeHiddenUsers:true);
			_listUserodsInRegionWithAssignedIns=Userods.GetUsersForVerifyList(listClinicNums,isAssigning:false);
			for(int i=0;i<_listUserodsInRegionWithAssignedIns.Count;i++) {
				if(_listUserodsInRegionWithAssignedIns[i].IsHidden) {
					continue;
				}
				comboVerifyUser.Items.Add(_listUserodsInRegionWithAssignedIns[i].UserName);
				if(_userNumVerify==_listUserodsInRegionWithAssignedIns[i].UserNum) {
					comboVerifyUser.SelectedIndex=i+2;//Add 2 because of the "All Users" and "Unassigned" combo items.
				}
			}
			if(_userNumVerify==-1) {
				comboVerifyUser.SelectedIndex=0;//"All Users"
			}
			if(_userNumVerify==0) {
				comboVerifyUser.SelectedIndex=1;//"Unassigned"
			}
			for(int i=0;i<_listUserodsInRegion.Count;i++) {
				if(_userNumAssign==_listUserodsInRegion[i].UserNum) {
					textAssignUserStandard.Text=_listUserodsInRegion[i].UserName;
					textAssignUserMedicaid.Text=_listUserodsInRegion[i].UserName;
				}
			}
			if(_userNumAssign==0) {
				textAssignUserStandard.Text="Unassign";
				textAssignUserMedicaid.Text="Unassign";
			}
		}

		private void PickUser(bool isAssigning) {
			using FormUserPick formUserPick=new FormUserPick();
			formUserPick.IsSelectionmode=true;
			//The list has hidden and visible users. Don't allow the user to pick a hidden user.
			formUserPick.ListUserodsFiltered=_listUserodsInRegion.FindAll(x => x.IsHidden==false); 
			if(!isAssigning) {
				formUserPick.IsPickAllAllowed=true;
			}
			formUserPick.IsPickNoneAllowed=true;
			formUserPick.ShowDialog();
			if(formUserPick.DialogResult!=DialogResult.OK) {
				return;
			}
			if(isAssigning) {//Setting the user
				_userNumAssign=formUserPick.SelectedUserNum;
				FillControls();
				return;
			}
			//Filter by user
			_userNumVerify=formUserPick.SelectedUserNum;
			FillControls();
		}

		private List<InsVerifyGridRow> GetRowsForGrid(GridOD grid) {
			List<InsVerifyGridRow> listInsVerifyGridRows=new List<InsVerifyGridRow>();
			if(!textAppointmentScheduledDaysStandard.IsValid() 
				|| !textPatientEnrollmentDaysStandard.IsValid() 
				|| !textInsBenefitEligibilityDaysStandard.IsValid()
				|| !textAppointmentScheduledDaysMedicaid.IsValid() 
				|| !textPatientEnrollmentDaysMedicaid.IsValid() 
				|| !textInsBenefitEligibilityDaysMedicaid.IsValid()) 
			{
				return listInsVerifyGridRows;
			}
			bool excludePatVerifyWhenNoIns=PrefC.GetBool(PrefName.InsVerifyExcludePatVerify);
			bool excludePatClones=(PrefC.GetBool(PrefName.ShowFeaturePatientClone)==true) && PrefC.GetBool(PrefName.InsVerifyExcludePatientClones);
			DateTime dateStartStandard=DateTime.Today;
			DateTime dateEndStandard=DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDaysStandard.Text));//Don't need to add 1 because we will be getting only the date portion of this datetime.
			DateTime dateStartMedicaid=DateTime.Today;
			DateTime dateEndMedicaid=DateTime.Today.AddDays(PIn.Int(textAppointmentScheduledDaysMedicaid.Text));
			if(grid==gridPastDue) {
				dateStartStandard=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyDaysFromPastDueAppt));
				dateEndStandard=DateTime.Today.AddDays(-1);
				dateStartMedicaid=DateTime.Today.AddDays(-PrefC.GetInt(PrefName.InsVerifyDaysFromPastDueApptMedicaid));
				dateEndMedicaid=DateTime.Today.AddDays(-1);
			}
			DateTime dateLastPatEligibilityStandard=DateTime.Today.AddDays(-PIn.Int(textPatientEnrollmentDaysStandard.Text));
			DateTime dateLastPlanBenefitsStandard=DateTime.Today.AddDays(-PIn.Int(textInsBenefitEligibilityDaysStandard.Text));
			DateTime dateLastPatEligibilityMedicaid=DateTime.Today.AddDays(-PIn.Int(textPatientEnrollmentDaysMedicaid.Text));
			DateTime dateLastPlanBenefitsMedicaid=DateTime.Today.AddDays(-PIn.Int(textInsBenefitEligibilityDaysMedicaid.Text));
			InsVerifyListType insVerifyListType=new InsVerifyListType();
			if(grid.In(gridMain,gridPastDue)) {
				insVerifyListType=InsVerifyListType.Both;
			}
			if(grid==gridAssignStandard) {
				insVerifyListType=InsVerifyListType.Standard;
			}
			if(grid==gridAssignMedicaid) {
				insVerifyListType=InsVerifyListType.Medicaid;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				List<InsVerifyGridObject> listInsVerifyGridObjects=InsVerifies.GetVerifyGridList(dateStartStandard,dateEndStandard,dateLastPatEligibilityStandard,dateLastPlanBenefitsStandard,
					_listClinicNumsVerifyClinicsFilter,_listDefNumsVerifyRegionsFilter,_defNumVerifyStatusFilter,_userNumVerify,textVerifyCarrier.Text,
					excludePatVerifyWhenNoIns,excludePatClones,
					dateStartMedicaid,dateEndMedicaid,dateLastPatEligibilityMedicaid,dateLastPlanBenefitsMedicaid,insVerifyListType);
				for(int i=0;i<listInsVerifyGridObjects.Count;i++) {
					if((grid==gridAssignStandard && listInsVerifyGridObjects[i].IsForMedicaidPlan) || (grid==gridAssignMedicaid && !listInsVerifyGridObjects[i].IsForMedicaidPlan)) {
						continue;
					}
					listInsVerifyGridRows.Add(new InsVerifyGridRow(listInsVerifyGridObjects[i],_dictionaryDefsStatus,_listUserodsInRegion));
				}
			};
			progressOD.ShowDialogProgress();
			return listInsVerifyGridRows;
		}

		private int CompareGridRows(InsVerifyGridRow insVerifyGridRowX,InsVerifyGridRow InsVerifyGridRowY) {
			if(insVerifyGridRowX.Type==InsVerifyGridRowY.Type && insVerifyGridRowX.Clinic==InsVerifyGridRowY.Clinic && insVerifyGridRowX.DateNextAppt==InsVerifyGridRowY.DateNextAppt) {
				return insVerifyGridRowX.CarrierName.CompareTo(InsVerifyGridRowY.CarrierName);
			}
			if(insVerifyGridRowX.Type==InsVerifyGridRowY.Type && insVerifyGridRowX.Clinic==InsVerifyGridRowY.Clinic) {
				return insVerifyGridRowX.DateNextAppt.CompareTo(InsVerifyGridRowY.DateNextAppt);
			}
			if(insVerifyGridRowX.Type==InsVerifyGridRowY.Type) {
				return insVerifyGridRowX.Clinic.CompareTo(InsVerifyGridRowY.Clinic);
			}
			return InsVerifyGridRowY.Type.CompareTo(insVerifyGridRowX.Type);//x and y are flipped to order by Type descending (Z-A)
		}

		private GridRow VerifyRowToODGridRow(InsVerifyGridRow insVerifyGridRow,bool isAssignGrid) {
			GridRow row=new GridRow();
			row.Cells.Add(insVerifyGridRow.Type);
			if(PrefC.HasClinicsEnabled) {
				row.Cells.Add(insVerifyGridRow.Clinic);
			}
			row.Cells.Add(insVerifyGridRow.PatientName);
			row.Cells.Add(insVerifyGridRow.DateNextAppt.ToString("g"));//"g" will exclude seconds from the DateTime.
			row.Cells.Add(insVerifyGridRow.CarrierName);
			row.Cells.Add(insVerifyGridRow.DateLastVerified.ToShortDateString());
			row.Cells.Add(insVerifyGridRow.VerifyStatus);
			if(!isAssignGrid) {
				row.Cells.Add(insVerifyGridRow.DateLastAssigned.ToShortDateString());
			}
			row.Cells.Add(insVerifyGridRow.AssignedTo);
			row.Tag=insVerifyGridRow.Tag;
			return row;
		}

		#region Grid Verify
		private GridOD GetVisibleGrid() {
			if(tabControl1.SelectedTab==tabAssignStandard) {
				return gridAssignStandard;
			}
			if(tabControl1.SelectedTab==tabAssignMedicaid) {
				return gridAssignMedicaid;
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
			List<InsVerifyGridRow> listInsVerifyGridRows=null;
			GridOD grid=GetVisibleGrid();
			listInsVerifyGridRows=GetRowsForGrid(grid);
			listInsVerifyGridRows.Sort(CompareGridRows);
			int indexSelectedRow=grid.GetSelectedIndex();
			bool isAssignGrid=grid.In(gridAssignStandard,gridAssignMedicaid);
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lans.g(this,"Type"),45);
			grid.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lans.g(this,"Clinic"),90);
				grid.Columns.Add(col);
			}
			col=new GridColumn(Lans.g(this,"Patient"),120);
			grid.Columns.Add(col);
			col=new GridColumn(Lans.g(this,"Appt Date Time"),130,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			grid.Columns.Add(col);
			col=new GridColumn(Lans.g(this,"Carrier"),160);
			grid.Columns.Add(col);
			col=new GridColumn(Lans.g(this,"Last Verified"),90,HorizontalAlignment.Center,GridSortingStrategy.DateParse);
			grid.Columns.Add(col);
			col=new GridColumn(Lans.g(this,"Status"),110);
			grid.Columns.Add(col);
			if(!isAssignGrid) {
				col=new GridColumn(Lans.g(this,"Status Date"),80,HorizontalAlignment.Center);
				grid.Columns.Add(col);
			}
			col=new GridColumn(Lans.g(this,"Assigned to"),120){ IsWidthDynamic=true };
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			for(int i=0;i<listInsVerifyGridRows.Count;i++) {
				grid.ListGridRows.Add(VerifyRowToODGridRow(listInsVerifyGridRows[i],isAssignGrid));
			}
			grid.EndUpdate();
			grid.SetSelected(indexSelectedRow,true);
		}

		private void gridMain_CellClick(object sender,UI.ODGridClickEventArgs e) {
			UpdateSelectedInfo(((InsVerifyGridObject)gridMain.ListGridRows[e.Row].Tag));
		}

		private void gridPastDue_CellClick(object sender,UI.ODGridClickEventArgs e) {
			UpdateSelectedInfo(((InsVerifyGridObject)gridPastDue.ListGridRows[e.Row].Tag));
		}

		private void UpdateSelectedInfo(InsVerifyGridObject obj) {
			_insVerifyGridObjectRowSelected=obj;
			if(_insVerifyGridObjectRowSelected.IsOnlyInsRow()) {
				butVerifyPlan.Enabled=true;
				butVerifyPat.Enabled=false;
				FillDisplayInfo(_insVerifyGridObjectRowSelected);
				return;
			}
			else if(_insVerifyGridObjectRowSelected.IsOnlyPatRow()) {
				butVerifyPlan.Enabled=false;
				butVerifyPat.Enabled=true;
				FillDisplayInfo(_insVerifyGridObjectRowSelected);
				return;
			}
			butVerifyPlan.Enabled=true;
			butVerifyPat.Enabled=true;
			FillDisplayInfo(_insVerifyGridObjectRowSelected);
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
			InsVerifyGridObject insVerifyGridObject=((InsVerifyGridObject)grid.ListGridRows[grid.GetSelectedIndex()].Tag);
			if((planToVerifyEnum==PlanToVerify.Both && !insVerifyGridObject.IsPatAndInsRow()) 
				|| (planToVerifyEnum==PlanToVerify.PatientEligibility && insVerifyGridObject.InsVerifyPat==null) 
				|| (planToVerifyEnum==PlanToVerify.InsuranceBenefits && insVerifyGridObject.InsVerifyPlan==null)) 
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
			string appointmentScheduledDays=textAppointmentScheduledDaysStandard.Text;
			string patientEnrollmentDays=textPatientEnrollmentDaysStandard.Text;
			string insBenefitEligibilityDays=textInsBenefitEligibilityDaysStandard.Text;
			if(insVerifyGridObject.IsForMedicaidPlan) {
				appointmentScheduledDays=textAppointmentScheduledDaysMedicaid.Text;
				patientEnrollmentDays=textPatientEnrollmentDaysMedicaid.Text;
				insBenefitEligibilityDays=textInsBenefitEligibilityDaysMedicaid.Text;
			}
			if(planToVerifyEnum==PlanToVerify.Both || planToVerifyEnum==PlanToVerify.PatientEligibility) {
				insVerifyGridObject.InsVerifyPat=InsVerifies.SetTimeAvailableForVerify(insVerifyGridObject.InsVerifyPat,PlanToVerify.PatientEligibility,
					PIn.Int(appointmentScheduledDays),PIn.Int(patientEnrollmentDays),PIn.Int(insBenefitEligibilityDays));
				insVerifyGridObject.InsVerifyPat.DateLastVerified=DateTime.Today;
				InsVerifyHists.InsertFromInsVerify(insVerifyGridObject.InsVerifyPat);
			}
			if(planToVerifyEnum==PlanToVerify.Both || planToVerifyEnum==PlanToVerify.InsuranceBenefits) {
				insVerifyGridObject.InsVerifyPlan=InsVerifies.SetTimeAvailableForVerify(insVerifyGridObject.InsVerifyPlan,PlanToVerify.InsuranceBenefits,
					PIn.Int(appointmentScheduledDays),PIn.Int(patientEnrollmentDays),PIn.Int(insBenefitEligibilityDays));
				insVerifyGridObject.InsVerifyPlan.DateLastVerified=DateTime.Today;
				InsVerifyHists.InsertFromInsVerify(insVerifyGridObject.InsVerifyPlan);
			}
			FillDisplayInfo(null);
			FillControls();
		}

		private void gridContextMenuStrip_Popup(object sender,EventArgs e) {
			_contextMenuRightClick.MenuItems.Clear();
			GridOD grid=GetVisibleGrid();
			if(grid.GetSelectedIndex()==-1) {
				return;
			}
			if(grid.In(gridAssignStandard,gridAssignMedicaid)) {
				gridAssign_Popup(sender,e);
				return;
			}
			//The _gridRowSelected needs to get updated before we run our menu item logic because Popup fires before CellClick.
			_insVerifyGridObjectRowSelected=(InsVerifyGridObject)grid.ListGridRows[grid.GetSelectedIndex()].Tag;
			_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Go to Patient"),gridMainRight_click));
			string verifyDescription=Lan.g(this,"Go to Insurance Plan");
			if(_insVerifyGridObjectRowSelected.InsVerifyPat!=null) {
				verifyDescription=Lan.g(this,"Go to Patient Plan");
			}
			_contextMenuRightClick.MenuItems.Add(new MenuItem(verifyDescription,gridMainRight_click));
			MenuItem menuItemAssignUserTool=new MenuItem(Lan.g(this,"Assign to User"));
			for(int i=0;i<_listUserodsInRegion.Count;i++) {
				if(_listUserodsInRegion[i].IsHidden) {
					continue;
				}
				MenuItem menuItemAssignUserDropDown=new MenuItem(_listUserodsInRegion[i].UserName);
				menuItemAssignUserDropDown.Tag=_listUserodsInRegion[i];
				menuItemAssignUserDropDown.Click+=new EventHandler(menuItemAssignUserDropDown_Click);
				menuItemAssignUserTool.MenuItems.Add(menuItemAssignUserDropDown);
			}
			_contextMenuRightClick.MenuItems.Add(menuItemAssignUserTool);
			MenuItem menuItemVerifyStatusTool=new MenuItem(Lan.g(this,"Set Verify Status to"));
			for(int i=0;i<_listDefsVerifyStatuses.Count;i++) {
				MenuItem menuItemVerifyStatusDropDown=new MenuItem(_listDefsVerifyStatuses[i].ItemName);
				menuItemVerifyStatusDropDown.Tag=_listDefsVerifyStatuses[i];
				menuItemVerifyStatusDropDown.Click+=new EventHandler(menuItemVerifyStatusDropDown_Click);
				menuItemVerifyStatusTool.MenuItems.Add(menuItemVerifyStatusDropDown);
			}
			_contextMenuRightClick.MenuItems.Add(menuItemVerifyStatusTool);
			if(_insVerifyGridObjectRowSelected.IsPatAndInsRow()) {
				_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Patient Eligibility"),gridMainRight_click));//Number 3 in gridMainRight_click
				_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Insurance Benefits"),gridMainRight_click));//Number 4 in gridMainRight_click
				_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Both"),gridMainRight_click));//Number 5 in gridMainRight_click
			}
			else if(_insVerifyGridObjectRowSelected.IsOnlyPatRow()) {
				_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Patient Eligibility"),gridMainRight_click));//Number 3 in gridMainRight_click
			}
			else if(_insVerifyGridObjectRowSelected.IsOnlyInsRow()) {
				_contextMenuRightClick.MenuItems.Add(new MenuItem(Lan.g(this,"Verify Insurance Benefits"),gridMainRight_click));//Number 3 in gridMainRight_click
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
			long patPlanNum=_insVerifyGridObjectRowSelected.GetPatPlanNum();
			if(patPlanNum==0) {				
				return;
			}
			PatPlan patPlan=null;
			InsSub insSub=null;
			InsPlan insPlan=null;
			if(_insVerifyGridObjectRowSelected.InsVerifyPat!=null) {				
				patPlan=PatPlans.GetByPatPlanNum(patPlanNum);
				if(patPlan==null) {
					MsgBox.Show(this,"The selected patient plan cannot be found.");
					//Refresh grids so we have more up to date data.
					FillControls();
					return;
				}
				insSub=InsSubs.GetOne(patPlan.InsSubNum);
				if(insSub==null) {
					MsgBox.Show(this,"The selected patient's subscriber cannot be found.");
					//Refresh grids so we have more up to date data.
					FillControls();
					return;
				}
				insPlan=InsPlans.GetPlan(insSub.PlanNum,new List<InsPlan>());
			}
			else if(_insVerifyGridObjectRowSelected.InsVerifyPlan!=null) {
				insPlan=InsPlans.GetPlan(_insVerifyGridObjectRowSelected.InsVerifyPlan.FKey,new List<InsPlan>());				
			}
			if(insPlan==null) {
				MsgBox.Show(this,"The selected insurance plan cannot be found.");
				//Refresh grids so we have more up to date data.
				FillControls();
				return;
			}
			using FormInsPlan formInsPlan=new FormInsPlan(insPlan,patPlan,insSub);
			formInsPlan.ShowDialog();
			if(formInsPlan.DialogResult==DialogResult.OK) {
				FillControls();
			}
		}

		private void gridMainRight_click(object sender,System.EventArgs e) {
			switch(_contextMenuRightClick.MenuItems.IndexOf((MenuItem)sender)) {
				case 0:
					GotoModule.GotoFamily(_insVerifyGridObjectRowSelected.GetPatNum());
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
					if(_insVerifyGridObjectRowSelected.IsOnlyInsRow()) {
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

		private List<InsVerifyGridObject> GetSelectedInsVerifyList() {
			List<InsVerifyGridObject> listInsVerifyGridObjects=new List<InsVerifyGridObject>();
			GridOD grid=GetVisibleGrid();
			if(!grid.In(gridAssignStandard,gridAssignMedicaid)) { //Shouldn't happen.
				return listInsVerifyGridObjects;
			}
			for(int i=0;i<grid.SelectedIndices.Length;i++) {
				listInsVerifyGridObjects.Add(((InsVerifyGridObject)grid.ListGridRows[grid.SelectedIndices[i]].Tag));
			}
			return listInsVerifyGridObjects;
		}

		///<summary>Retrieves the text from the currently displayed "Note" textbox.</summary>
		private string GetInsVerifyNote() {
			GridOD grid=GetVisibleGrid();
			if(grid==gridAssignStandard) {
				return textInsVerifyNoteStandard.Text;
			}
			if(grid==gridAssignMedicaid) {
				return textInsVerifyNoteMedicaid.Text;
			}
			return ""; //Shouldn't happen.
		}

		#region Assigning Logic
		private void butAssignUser_Click(object sender,EventArgs e) {
			GridOD grid=GetVisibleGrid();
			if(!grid.In(gridAssignStandard,gridAssignMedicaid)) { //Shouldn't happen.
				return;
			}
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an insurance to assign.");
				return;
			}
			if(_userNumAssign==0) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to unassign the selected plan?")) {
					return;
				}
			}
			List<InsVerifyGridObject> listInsVerifyGridObjects=GetSelectedInsVerifyList();
			for(int i=0;i<listInsVerifyGridObjects.Count;i++) {
				if(listInsVerifyGridObjects[i].InsVerifyPat!=null) {
					listInsVerifyGridObjects[i].InsVerifyPat.UserNum=_userNumAssign;
					listInsVerifyGridObjects[i].InsVerifyPat.Note=GetInsVerifyNote();
					listInsVerifyGridObjects[i].InsVerifyPat.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPat);
				}
				if(listInsVerifyGridObjects[i].InsVerifyPlan!=null) {
					listInsVerifyGridObjects[i].InsVerifyPlan.UserNum=_userNumAssign;
					listInsVerifyGridObjects[i].InsVerifyPlan.Note=GetInsVerifyNote();
					listInsVerifyGridObjects[i].InsVerifyPlan.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPlan);
				}
			}
			FillControls();
		}

		private void gridAssign_Popup(object sender,EventArgs e) {
			MenuItem menuItemAssignUserTool=new MenuItem(Lan.g(this,"Assign to User"));
			for(int i=0;i<_listUserodsInRegion.Count;i++) {
				if(_listUserodsInRegion[i].IsHidden) {
					continue;
				}
				MenuItem menuItemAssignUserDropDown=new MenuItem(_listUserodsInRegion[i].UserName);
				menuItemAssignUserDropDown.Tag=_listUserodsInRegion[i];
				menuItemAssignUserDropDown.Click+=new EventHandler(menuItemAssignUserDropDown_Click);
				menuItemAssignUserTool.MenuItems.Add(menuItemAssignUserDropDown);
			}
			_contextMenuRightClick.MenuItems.Add(menuItemAssignUserTool);
			MenuItem menuItemVerifyStatusTool=new MenuItem(Lan.g(this,"Set Verify Status to"));
			for(int i=0;i<_listDefsVerifyStatuses.Count;i++) {
				MenuItem menuItemVerifyStatusDropDown=new MenuItem(_listDefsVerifyStatuses[i].ItemName);
				menuItemVerifyStatusDropDown.Tag=_listDefsVerifyStatuses[i];
				menuItemVerifyStatusDropDown.Click+=new EventHandler(menuItemVerifyStatusDropDown_Click);
				menuItemVerifyStatusTool.MenuItems.Add(menuItemVerifyStatusDropDown);
			}
			_contextMenuRightClick.MenuItems.Add(menuItemVerifyStatusTool);
		}

		//private void gridAssignStandard_RightClick(object sender,System.EventArgs e) {
		//	switch(_contextMenuRightClick.MenuItems.IndexOf((MenuItem)sender)) {
		//		case 0:
		//			if(_assignUserNum==0) {
		//				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to unassign the selected plan?")) {
		//					return;
		//				}
		//			}
		//			List<InsVerifyGridObject> listInsVerifyGridObject=GetSelectedInsVerifyList();
		//			foreach(InsVerifyGridObject gridRowObject in listInsVerifyGridObject) {
		//				if(gridRowObject.InsVerifyPat!=null) {
		//					gridRowObject.InsVerifyPat.UserNum=_assignUserNum;
		//					gridRowObject.InsVerifyPat.DateLastAssigned=DateTime.Today;
		//					InsVerifies.Update(gridRowObject.InsVerifyPat);
		//				}
		//				if(gridRowObject.InsVerifyPlan!=null) {
		//					gridRowObject.InsVerifyPlan.UserNum=_assignUserNum;
		//					gridRowObject.InsVerifyPlan.DateLastAssigned=DateTime.Today;
		//					InsVerifies.Update(gridRowObject.InsVerifyPlan);
		//				}
		//			}
		//			break;
		//		case 1:
		//			//Not clickable due to being a dropdown menu.
		//			break;
		//	}
		//	FillControls();
		//}

		private void butAssignUserPick_Click(object sender,EventArgs e) {
			PickUser(true);
		}

		private void comboSetVerifyStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboSetVerifyStatus.SelectedIndex<1) {
				_defNumVerifyStatusAssign=0;
				comboSetVerifyStatus.Text="none";
			}
			else {
				_defNumVerifyStatusAssign=_listDefsVerifyStatuses[comboSetVerifyStatus.SelectedIndex-1].DefNum;
				comboSetVerifyStatus.Text=_listDefsVerifyStatuses[comboSetVerifyStatus.SelectedIndex-1].ItemName;
			}
			if(gridMain.GetSelectedIndex()!=-1 || gridPastDue.GetSelectedIndex()!=-1) {//Both grids cannot have a selection at the same time
				SetStatus(_defNumVerifyStatusAssign,isVerifyGrid:true);
			}
			FillControls();
		}
		#endregion

		#region Grid Filters
		private void butVerifyUserPick_Click(object sender,EventArgs e) {
			PickUser(false);
			FillControls();
		}

		private void comboFilterVerifyStatus_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboFilterVerifyStatus.SelectedIndex<1) {
				_defNumVerifyStatusFilter=0;
				FillControls();
				return;
			}
			_defNumVerifyStatusFilter=_listDefsVerifyStatuses[comboFilterVerifyStatus.SelectedIndex-1].DefNum;
			FillControls();
		}

		///<summary>Firing on MouseUp allows time for the user to select multiple values via "dragging"</summary>
		private void listBoxVerifyClinics_MouseUp(object sender,MouseEventArgs e) {
			UpdateSelectedClinicNums();
			FillControls();
		}

		///<summary>Firing on MouseUp allows time for the user to select multiple values via "dragging"</summary>
		private void listBoxVerifyRegions_MouseUp(object sender,MouseEventArgs e) {
			_listDefNumsVerifyRegionsFilter=new List<long>();
			for(int i=0;i<listBoxVerifyRegions.SelectedIndices.Count;i++) {
				if(listBoxVerifyRegions.SelectedIndices[i]<1) {
					_listDefNumsVerifyRegionsFilter.Add(-1);
					continue;
				}
				_listDefNumsVerifyRegionsFilter.Add(_listDefsRegion[listBoxVerifyRegions.SelectedIndices[i]-1].DefNum);
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
			FillControls();
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
				bool isAllClinicsEnabled=PrefC.HasClinicsEnabled && !Security.CurUser.ClinicIsRestricted && !PrefC.GetBool(PrefName.EnterpriseApptList);
				if(isAllClinicsEnabled) {
					_listClinicNumsVerifyClinicsFilter.Add(-1);
					return;
				}
				_listClinicNumsVerifyClinicsFilter.Add(0);
			}
		}

		private void comboVerifyUser_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboVerifyUser.SelectedIndex==1) {//Selected "Unassigned"
				_userNumVerify=0;
				FillControls();
				return;
			}
			if(comboVerifyUser.SelectedIndex<1) {//Selected "All Users" or selected index is invalid
				_userNumVerify=-1;
				FillControls();
				return;
			}
			//Selected a real User.
			Userod userod=_listUserodsInRegionWithAssignedIns.FirstOrDefault(x => x.UserName==comboVerifyUser.GetSelected<string>());
			_userNumVerify=userod.UserNum;
			FillControls();
		}

		#endregion

		private void SetStatus(long statusDefNum,bool isVerifyGrid) {
			string statusNote="";
			bool hasChanged=false;
			using InputBox inputBox=new InputBox(Lan.g(this,"Add a status note:"),true);
			inputBox.setTitle(Lan.g(this,"Add Status Note"));
			if(!isVerifyGrid) {
				inputBox.textResult.Text=GetInsVerifyNote();
			}
			inputBox.ShowDialog();
			if(inputBox.DialogResult==DialogResult.OK) {
				statusNote=inputBox.textResult.Text;
				hasChanged=true;
			}
			if(isVerifyGrid) {
				if(_insVerifyGridObjectRowSelected.InsVerifyPat!=null) {
					_insVerifyGridObjectRowSelected.InsVerifyPat.DefNum=statusDefNum;
					if(hasChanged) {
						_insVerifyGridObjectRowSelected.InsVerifyPat.Note=statusNote;
					}
					_insVerifyGridObjectRowSelected.InsVerifyPat.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_insVerifyGridObjectRowSelected.InsVerifyPat);
				}
				if(_insVerifyGridObjectRowSelected.InsVerifyPlan!=null) {
					_insVerifyGridObjectRowSelected.InsVerifyPlan.DefNum=statusDefNum;
					if(hasChanged) {
						_insVerifyGridObjectRowSelected.InsVerifyPlan.Note=statusNote;
						textInsVerifyReadOnlyNote.Text=statusNote;
					}
					_insVerifyGridObjectRowSelected.InsVerifyPlan.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_insVerifyGridObjectRowSelected.InsVerifyPlan);
				}
				return;
			}		
			List<InsVerifyGridObject> listInsVerifyGridObjects=GetSelectedInsVerifyList();
			for(int i=0;i<listInsVerifyGridObjects.Count;i++) {
				if(listInsVerifyGridObjects[i].InsVerifyPat!=null) { 
					listInsVerifyGridObjects[i].InsVerifyPat.DefNum=statusDefNum;
					if(hasChanged) {
						listInsVerifyGridObjects[i].InsVerifyPat.Note=statusNote;
					}
					listInsVerifyGridObjects[i].InsVerifyPat.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPat);
				}
				if(listInsVerifyGridObjects[i].InsVerifyPlan!=null) {
					listInsVerifyGridObjects[i].InsVerifyPlan.DefNum=statusDefNum;
					if(hasChanged) {
						listInsVerifyGridObjects[i].InsVerifyPlan.Note=statusNote;
					}
					listInsVerifyGridObjects[i].InsVerifyPlan.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPlan);
				}
			}
		}

		private void menuItemVerifyStatusDropDown_Click(object sender, EventArgs e) {
			Def defStatus=(Def)((MenuItem)sender).Tag;
			if(tabControl1.SelectedTab==tabVerify) {
				SetStatus(defStatus.DefNum,isVerifyGrid:true);
				FillControls();
				return;
			}
			if(tabControl1.SelectedTab.In(tabAssignStandard,tabAssignMedicaid)) {
				SetStatus(defStatus.DefNum,isVerifyGrid:false);
			}
			FillControls();
		}

		private void menuItemAssignUserDropDown_Click(object sender, EventArgs e) {
			Userod userod=(Userod)((MenuItem)sender).Tag;
			if(tabControl1.SelectedTab==tabVerify) {
				if(_insVerifyGridObjectRowSelected.InsVerifyPat!=null) {
					_insVerifyGridObjectRowSelected.InsVerifyPat.UserNum=userod.UserNum;
					_insVerifyGridObjectRowSelected.InsVerifyPat.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_insVerifyGridObjectRowSelected.InsVerifyPat);
				}
				if(_insVerifyGridObjectRowSelected.InsVerifyPlan!=null) {
					_insVerifyGridObjectRowSelected.InsVerifyPlan.UserNum=userod.UserNum;
					_insVerifyGridObjectRowSelected.InsVerifyPlan.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(_insVerifyGridObjectRowSelected.InsVerifyPlan);
				}
				FillControls();
				return;
			}
			if(!tabControl1.SelectedTab.In(tabAssignStandard,tabAssignMedicaid)) {
				FillControls();
				return;
			}
			List<InsVerifyGridObject> listInsVerifyGridObjects=GetSelectedInsVerifyList();
			for(int i=0;i<listInsVerifyGridObjects.Count;i++) { 
				if(listInsVerifyGridObjects[i].InsVerifyPat!=null) {
					listInsVerifyGridObjects[i].InsVerifyPat.UserNum=userod.UserNum;
					listInsVerifyGridObjects[i].InsVerifyPat.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPat);
				}
				if(listInsVerifyGridObjects[i].InsVerifyPlan!=null) {
					listInsVerifyGridObjects[i].InsVerifyPlan.UserNum=userod.UserNum;
					listInsVerifyGridObjects[i].InsVerifyPlan.DateLastAssigned=DateTime.Today;
					InsVerifies.Update(listInsVerifyGridObjects[i].InsVerifyPlan);
				}
			}
			FillControls();
		}

		private void tabControl1_Selected(object sender,EventArgs e) {
			if(tabControl1.SelectedTab.In(tabAssignStandard,tabAssignMedicaid)) {
				if(_userNumVerify!=0) {
					_userNumVerifyGrid=_userNumVerify;
					_userNumVerify=0;//Set filter user to Unassigned when switching to Assign tab.=
				}
			}
			else if(tabControl1.SelectedTab==tabVerify) {
				if(_userNumVerify==0) {
					_userNumVerify=_userNumVerifyGrid;
				}
			}
			FillControls();
		}

		private void tabControlVerificationList_Selected(object sender,EventArgs e) {
			FillDisplayInfo(null);
			gridMain.SetAll(false);
			gridPastDue.SetAll(false);
			FillControls();
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		///<summary>This represents a row in either the Verification List Grid, or the Assignment Grid.  The assignment grid won't use the DateLastAssigned variable.</summary>
		private class InsVerifyGridRow {
			public string Type;
			public string Clinic;
			public string PatientName;
			public DateTime DateNextAppt;
			public string CarrierName;
			public DateTime DateLastVerified;
			public string VerifyStatus;
			//DateLastAssigned isn't used if IsAssignGrid=true
			public DateTime DateLastAssigned;
			public string AssignedTo;
			public Object Tag;

			///<summary>An updated dictionary of status defs should be passed in.  
			///This is to avoid grabbing definitions cache from inside this nested class, which will be instanced in a loop.</summary>
			public InsVerifyGridRow(InsVerifyGridObject insVerifyGridObject,Dictionary<long,Def> dictionaryStatusDefs,List<Userod> listUserods) {
				if(insVerifyGridObject==null) {
					return;
				}
				Type="";
				Clinic="";
				PatientName="";
				CarrierName="";
				VerifyStatus="";
				AssignedTo="";
				if(insVerifyGridObject.IsPatAndInsRow()) {//If showing a consolidated row, use the PatInsVerify information, since they are same anyways.
					Type="Pat/Ins";
					Clinic=insVerifyGridObject.InsVerifyPat.ClinicName;
					PatientName=insVerifyGridObject.InsVerifyPat.PatientName;
					DateNextAppt=insVerifyGridObject.InsVerifyPat.AppointmentDateTime;
					CarrierName=insVerifyGridObject.InsVerifyPat.CarrierName;
					//Get the oldest DateLastVerified
					DateLastVerified=(insVerifyGridObject.InsVerifyPat.DateLastVerified<=insVerifyGridObject.InsVerifyPlan.DateLastVerified ? 
						insVerifyGridObject.InsVerifyPat.DateLastVerified : 
						insVerifyGridObject.InsVerifyPlan.DateLastVerified);
					if(dictionaryStatusDefs.ContainsKey(insVerifyGridObject.InsVerifyPat.DefNum)) {
						VerifyStatus=dictionaryStatusDefs[insVerifyGridObject.InsVerifyPat.DefNum].ItemName;
					}
					bool isPatLastAssignedNewer=insVerifyGridObject.InsVerifyPat.DateLastAssigned>=insVerifyGridObject.InsVerifyPlan.DateLastAssigned;
					//Get the most recent DateLastAssigned
					DateLastAssigned=(isPatLastAssignedNewer ? 
						insVerifyGridObject.InsVerifyPat.DateLastAssigned : 
						insVerifyGridObject.InsVerifyPlan.DateLastAssigned);
					SetAssignedTo(listUserods,insVerifyGridObject,isPatRow:true);
					Tag=insVerifyGridObject;
					return;
				}
				if(insVerifyGridObject.IsOnlyPatRow()) {
					Type="Pat";
					Clinic=insVerifyGridObject.InsVerifyPat.ClinicName;
					PatientName=insVerifyGridObject.InsVerifyPat.PatientName;
					DateNextAppt=insVerifyGridObject.InsVerifyPat.AppointmentDateTime;
					CarrierName=insVerifyGridObject.InsVerifyPat.CarrierName;
					DateLastVerified=insVerifyGridObject.InsVerifyPat.DateLastVerified;
					if(dictionaryStatusDefs.ContainsKey(insVerifyGridObject.InsVerifyPat.DefNum)) {
						VerifyStatus=dictionaryStatusDefs[insVerifyGridObject.InsVerifyPat.DefNum].ItemName;
					}
					DateLastAssigned=insVerifyGridObject.InsVerifyPat.DateLastAssigned;
					SetAssignedTo(listUserods,insVerifyGridObject,isPatRow:true);
					Tag=insVerifyGridObject;
					return;
				}
				if(!insVerifyGridObject.IsOnlyInsRow()) {
					return;
				}
				Type="Ins";
				Clinic=insVerifyGridObject.InsVerifyPlan.ClinicName;
				PatientName=insVerifyGridObject.InsVerifyPlan.PatientName;
				DateNextAppt=insVerifyGridObject.InsVerifyPlan.AppointmentDateTime;
				CarrierName=insVerifyGridObject.InsVerifyPlan.CarrierName;
				DateLastVerified=insVerifyGridObject.InsVerifyPlan.DateLastVerified;
				if(dictionaryStatusDefs.ContainsKey(insVerifyGridObject.InsVerifyPlan.DefNum)) {
					VerifyStatus=dictionaryStatusDefs[insVerifyGridObject.InsVerifyPlan.DefNum].ItemName;
				}
				DateLastAssigned=insVerifyGridObject.InsVerifyPlan.DateLastAssigned;
				SetAssignedTo(listUserods,insVerifyGridObject,isInsPlanRow:true);
				Tag=insVerifyGridObject;
			}

			/// <summary>Sets the AssignedTo property to the current username, or the current username+(hidden) if the current user is hidden</summary>
			private void SetAssignedTo(List<Userod> listUserods,InsVerifyGridObject insVerifyGridObject,bool isPatRow=false,bool isInsPlanRow=false) {
				Userod userod=new Userod();
				if(isPatRow || (isPatRow && isInsPlanRow)) {//Pat only row or pat/ins plan row
					userod=listUserods.FirstOrDefault(x => x.UserNum==insVerifyGridObject.InsVerifyPat.UserNum);
				}
				if(isInsPlanRow) {//plan only row
					userod=listUserods.FirstOrDefault(x => x.UserNum==insVerifyGridObject.InsVerifyPlan.UserNum);
				}
				if(userod!=null) {
					AssignedTo=userod.UserName;
					if(userod.IsHidden) {
							AssignedTo+=" (hidden)";
					}
				}
			}
		}
	}
}