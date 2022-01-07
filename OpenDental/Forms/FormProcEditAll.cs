using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormProcEditAll:FormODBase {
		public List<Procedure> ProcList;
		private List<Procedure> _procOldList;
		///<summary>True when any proc in ProcList has a Proc Status of C.</summary>
		private bool _hasCompletedProc;
		///<summary>True when any proc in ProcList has a Proc Status of EO or EC.</summary>
		private bool _hasExistingProc;
		///<summary>True when all procs in ProcList Can Bypass Lock Date.</summary>
		private bool _canAllBypass;
		///<summary>List of providers shown in comboProv. Excludes blank(multi) and provider abbr text if user does not have access to a clinic from ProcList.</summary>>
		private List<Provider> _listProvsForClinic;
		///<summary>List of clinics shown in comboClincs. Excludes blank(multi) and clinic abbr text if user does not have access to a clinic from ProcList.</summary>
		private List<Clinic> _listClinics;

		public FormProcEditAll() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcEditAll_Load(object sender,EventArgs e) {
			_procOldList=new List<Procedure>();
			for(int i=0;i<ProcList.Count;i++){
				_procOldList.Add(ProcList[i].Copy());
			}
			_hasCompletedProc=false;
			_hasExistingProc=false;
			_canAllBypass=true;
			DateTime oldestProcDate=DateTime.Today;
			bool dateLoadedWithValue=true;
			foreach(Procedure proc in ProcList){
				if(proc.ProcStatus==ProcStat.C) {
					_hasCompletedProc=true;
					if(proc.ProcDate < oldestProcDate){
						oldestProcDate=proc.ProcDate;
					}
				}
				else if(ListTools.In(proc.ProcStatus,ProcStat.EO,ProcStat.EC)){
					_hasExistingProc=true;
					//Eo/Ec procs check DateEntryC for permissions.
				}
				if(ProcList[0].ProcDate!=proc.ProcDate){
					dateLoadedWithValue=false;
				}
				if(!ProcedureCodes.CanBypassLockDate(proc.CodeNum,proc.ProcFee)) {
					_canAllBypass=false;
				}
			}
			if(!IsUserAuthorizedForProcDate(oldestProcDate)) {
				butOK.Enabled=false;
				butEditAnyway.Enabled=false;
			}
			List<ClaimProc> ClaimProcList=ClaimProcs.Refresh(ProcList[0].PatNum);
			if(Procedures.IsAttachedToClaim(ProcList,ClaimProcList)){
				//StartedAttachedToClaim=true;
				//however, this doesn't stop someone from creating a claim while this window is open,
				//so this is checked at the end, too.
				textDate.Enabled=false;
				butToday.Enabled=false;
				butEditAnyway.Visible=true;
				labelClaim.Visible=true;
			}
			if(dateLoadedWithValue){
				textDate.Text=ProcList[0].ProcDate.ToShortDateString();
			}
			if(PrefC.HasClinicsEnabled) {
				labelClinic.Visible=true;
				comboClinic.Visible=true;
				FillComboClinic();//Must be called before FillProviderCombo().
			}
			FillComboProv();
		}

		///<summary>Determines if the current user had sufficient permissions to change the ProcDate to 'date' on the procedures in ProcList.</summary>
		private bool IsUserAuthorizedForProcDate(DateTime dateCProcs) {
			bool isAuthorized=true;
			if(_hasCompletedProc || _hasExistingProc){
				DateTime dateEoEcOldest=_hasExistingProc ? ProcList.Where(x => ListTools.In(x.ProcStatus,ProcStat.EO,ProcStat.EC)).Min(x => x.DateEntryC)
					: DateTime.Today;
				if(_canAllBypass) {
					if((_hasCompletedProc && !Security.IsAuthorized(Permissions.ProcCompleteEdit,dateCProcs,ProcList[0].CodeNum,0))
						|| (_hasExistingProc && !Security.IsAuthorized(Permissions.ProcExistingEdit,dateEoEcOldest,ProcList[0].CodeNum,0)))
					{
						isAuthorized=false;
					}
				}
				else if((_hasCompletedProc && !Security.IsAuthorized(Permissions.ProcCompleteEdit,dateCProcs))
					|| (_hasExistingProc && !Security.IsAuthorized(Permissions.ProcExistingEdit,dateEoEcOldest)))
				{
					isAuthorized=false;
				}
			}
			return isAuthorized;
		}

		private void butToday_Click(object sender,EventArgs e) {
			if(textDate.Enabled){
				textDate.Text=DateTime.Today.ToShortDateString();
			}
		}

		private void FillComboClinic() {
			//Not using ComboBoxClinicPicker because we need to add a blank "" for no selection logic when procedures have different clinics.
			comboClinic.Items.Clear();
			comboClinic.Items.Add("",null);
			comboClinic.SelectedIndex=0;//Selection is not changed if isAllProcsForSameClinic is false.
			bool isAllProcsForSameClinic=ProcList.Select(x => x.ClinicNum).Distinct().ToList().Count==1;
			bool isListAlpha = PrefC.GetBool(PrefName.ClinicListIsAlphabetical);
			_listClinics=Clinics.GetForUserod(Security.CurUser);
			if(isListAlpha) {
				_listClinics=_listClinics.OrderBy(x => x.Abbr).ToList();
			}
			else {
				_listClinics=_listClinics.OrderBy(x => x.ItemOrder).ToList();
			}
			_listClinics.Insert(0,Clinics.GetPracticeAsClinicZero(Lan.g(this,"None")));
			for(int i=0;i<_listClinics.Count;i++) {//None mimics FormProcEdit
				comboClinic.Items.Add(_listClinics[i].Abbr,_listClinics[i]);
				if(isAllProcsForSameClinic && _listClinics[i].ClinicNum==ProcList[0].ClinicNum) {
					comboClinic.SetSelected(i+1);
				}
			}
			if(isAllProcsForSameClinic && !_listClinics.Any(x => x.ClinicNum==ProcList[0].ClinicNum)) {
				//All procedure clinics are the same but value is missing from our list.
				//We might eventaully check to see how many clincs from proc list do not exists in listClinics.
				comboClinic.SetSelectedKey<Clinic>(ProcList[0].ClinicNum,x=>x.ClinicNum,x=>Clinics.GetAbbr(x));//selectedIndex -1
			}
		}

		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboProv(true);
		}
		
		private void FillComboProv(bool tryMaintainOldSelection=false) {
			_listProvsForClinic=new List<Provider>();
			if(comboClinic.GetSelected<Clinic>()==null) {//Dummy clinic is selected either directly or on load due to procs having multiple clinics.
				//default the list to use all providers
				//We might want to change this to instead load all providers for all ClinicNums in ProcList.
				_listProvsForClinic=Providers.GetDeepCopy(true);
			}
			else {
				_listProvsForClinic=Providers.GetProvsForClinic(comboClinic.GetSelected<Clinic>().ClinicNum);
			}
			_listProvsForClinic=_listProvsForClinic.Where(x => !x.IsHidden).OrderBy(x => x.ItemOrder).ToList();
			Provider provOldSelection=null;
			if(tryMaintainOldSelection && comboProv.GetSelected<Provider>()!=null){//Only true on manual selection, not on load.
				provOldSelection=_listProvsForClinic.FirstOrDefault(x => x.ProvNum==comboProv.GetSelectedProvNum());
			}
			comboProv.Items.Clear();
			comboProv.Items.Add("",null);
			comboProv.Items.AddProvsFull(_listProvsForClinic);
			comboProv.SelectedIndex=0;//default selected index to blank/original values, override if there is a different match below.
			bool isAllProcsForSameProv=ProcList.Select(x => x.ProvNum).Distinct().ToList().Count==1;
			if(tryMaintainOldSelection && provOldSelection!=null){
				comboProv.SetSelectedProvNum(provOldSelection.ProvNum);//set to previous selection
			}
			else if(isAllProcsForSameProv){
				comboProv.SetSelectedProvNum(ProcList[0].ProvNum);//default to the proc's prov if all procs have same prov
			}
			if(isAllProcsForSameProv && !_listProvsForClinic.Any(x => x.ProvNum==ProcList[0].ProvNum)) {
				//All procedure clinics are the same but value is missing from our list.
				//We might eventaully check to see how many clincs from proc list do not exists in listClinics.
				comboProv.SetSelectedProvNum(ProcList[0].ProvNum);//selectedIndex -1
			}
		}

		private void butMoreProvs_Click(object sender,EventArgs e) {
			using FormProviderPick FormPP=new FormProviderPick(_listProvsForClinic);
			if(FormPP.ShowDialog(this)!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormPP.SelectedProvNum);
		}

		private void butEditAnyway_Click(object sender,EventArgs e) {
			textDate.Enabled=true;
			butToday.Enabled=true;
		}

		private bool EntriesAreValid() {
			//Get a new recent list of claimprocs for pat to be able to validate for provider and procedure status change.
			if(textDate.Text!="" && !textDate.IsValid()) {
				//Either loaded blank or user deleted date. Either way blank will not make it to DB.
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			DateTime procDate=PIn.Date(textDate.Text);
			if(textDate.Text!="" && ProcList.Any(x => x.ProcDate!=procDate)){
				if(!IsUserAuthorizedForProcDate(procDate)) {//Do not allow new ProcDate outside of date limitations.  Mimics behavior in FormProcEdit.
					return false;
				}
				Appointment apt;
				foreach(Procedure proc in ProcList){//first validate for all dates. 
					#region Future dating completed procedures validation.
					if(!PrefC.GetBool(PrefName.FutureTransDatesAllowed) && proc.ProcStatus==ProcStat.C && procDate>DateTime.Today) {
						MsgBox.Show(this,"Completed procedures cannot have future dates.");
						return false;
					}
					#endregion
					#region Procedures attached to appointments date validation.
					if(proc.AptNum==0){
						continue;
					}
					apt=Appointments.GetOneApt(proc.AptNum);
					if(proc.ProcDate.Date!=apt.AptDateTime.Date) {
						string error=Lan.g(this,"Date does not match appointment date for a procedure dated:")+" "+proc.ProcDate.ToShortDateString()
							+"\r\n"+Lan.g(this,"Continue anyway?");
						if(MessageBox.Show(error,"",MessageBoxButtons.YesNo)==DialogResult.No) {
							return false;
						}
						break;
					}
					#endregion
				}
			}
			List<ClaimProc> listClaimProcsForPat=ClaimProcs.RefreshForProcs(ProcList.Select(x => x.ProcNum).ToList());
			foreach(Procedure proc in ProcList) {
				if(proc.IsLocked) {
					MsgBox.Show(this,"Locked procedures cannot be edited.");
					return false;
				}
				#region Provider change validation.
				List<ClaimProc> listClaimProcsForProc=ClaimProcs.GetForProc(listClaimProcsForPat,proc.ProcNum);
				long selectedProvNum=(comboProv.GetSelected<Provider>()?.ProvNum??0);//0 if no selection made
				if(selectedProvNum!=0 && !ProcedureL.ValidateProvider(listClaimProcsForProc,selectedProvNum,proc.ProvNum)) {
					return false;
				}
				#endregion
			}
			return true;
		}

		private string ConstructSecurityLogForProcType(Procedure proc,Procedure procOld) {
			string logTextForProc="";
			string code=ProcedureCodes.GetProcCode(proc.CodeNum).ProcCode;
			string procDateStrOld=POut.Date(procOld.ProcDate);
			string procDateStrNew=POut.Date(proc.ProcDate);
			logTextForProc+=SecurityLogEntryHelper(code,SecurityLogFields.ProcDate,procDateStrOld,procDateStrNew);
			string provNumStrOld=Providers.GetAbbr(procOld.ProvNum);
			string provNumStrNew=Providers.GetAbbr(proc.ProvNum);
			logTextForProc+=SecurityLogEntryHelper(code,SecurityLogFields.ProvNum,provNumStrOld,provNumStrNew);
			string clinicNumStrOld=Clinics.GetAbbr(procOld.ClinicNum);
			string clinicNumStrNew=Clinics.GetAbbr(proc.ClinicNum);
			logTextForProc+=SecurityLogEntryHelper(code,SecurityLogFields.ClinicNum,clinicNumStrOld,clinicNumStrNew);
			return logTextForProc;
		}

		///<summary>Returns blank string if oldVal and newVal are the same.</summary>
		private string SecurityLogEntryHelper(string procCodeVal,SecurityLogFields logField,string oldVal,string newVal) {
			if(oldVal==newVal) {
				return "";
			}
			string field="";
			switch(logField) {
				case SecurityLogFields.ProcDate:
						field=Lan.g(this,"Proc Date");
						break;
				case SecurityLogFields.ProvNum:
						field=Lan.g(this,"Provider");
						break;
				case SecurityLogFields.ClinicNum:
						field=Lan.g(this,"Clinic");
						break;
			}
			return Lan.g(this,"Procedure code")+" "+procCodeVal+" "+field+" "+Lan.g(this,"changed from")+" '"+oldVal+"' "+Lan.g(this,"to")+" '"+newVal+"'\r\n";
		}

		///<summary>Runs Procedures.ComputeEstimates for given proc and listClaimProcs.
		///Does not update existing ClaimProcs or insert new ClaimProcs in the database.
		///Outs new ClaimProcs added by Procedures.ComputeEstimates, listClaimProcs will contain any added claimProcs.</summary>
		private void RecomputeEstimates(Procedure proc,List<ClaimProc> listClaimProcs,out List<ClaimProc> listNewClaimProcs,
			OrthoProcLink orthoProcLink,OrthoCase orthoCase,OrthoSchedule orthoSchedule,List<OrthoProcLink> listOrthoProcLinksForOrthoCase)
		{ 
			List<ClaimProc> listOrigClaimProcs=new List<ClaimProc>(listClaimProcs);
			Patient pat=Patients.GetPat(proc.PatNum);
			List<PatPlan> listPatPlans=PatPlans.GetPatPlansForPat(pat.PatNum);
			List<InsSub> listInsSubs=InsSubs.GetMany(listPatPlans.Select(x => x.InsSubNum).ToList());
			List<InsPlan> listInsPlans=InsPlans.GetByInsSubs(listInsSubs.Select(x => x.InsSubNum).ToList());
			List<Benefit> listBenefits=Benefits.Refresh(listPatPlans,listInsSubs);//Same method used in FormProcEdit.Load()->GetLoadData()
			//FormProcEdit uses GetHistList(....,procDate:DateTime.Today,...)
			List<ClaimProcHist> listHist=ClaimProcs.GetHistList(pat.PatNum,listBenefits,listPatPlans,listInsPlans,DateTime.Today,listInsSubs);
			bool isSaveToDb=false;
			//When isSaveToDb is true any estimates that are missing will be inserted into DB and it refreshes listClaimProcs from the Db.
			//Refreshing the list results in losing the changes to ProvNum and ProcDate.
			Procedures.ComputeEstimates(proc,pat.PatNum,ref listClaimProcs,false,listInsPlans,listPatPlans,listBenefits,listHist,
				new List<ClaimProcHist> { },isSaveToDb,pat.Age,listInsSubs
				,useProcDateOnProc:true,
				orthoProcLink:orthoProcLink,orthoCase:orthoCase,orthoSchedule:orthoSchedule,listOrthoProcLinksForOrthoCase:listOrthoProcLinksForOrthoCase);
			listNewClaimProcs=listClaimProcs.Except(listOrigClaimProcs).ToList();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!EntriesAreValid()) {
				return;
			}
			//This list should only contain a single patNum, but just in case.
			List<long> listDistinctPatNums=ProcList.Select(x => x.PatNum).Distinct().ToList();
			DateTime procDate=DateTime.MinValue;
			if(textDate.Text!="" && ProcList.Any(x => x.ProcDate!=PIn.Date(textDate.Text))) {
				procDate=PIn.Date(textDate.Text);
			}
			#region Update ProcList and DB to reflect selections
			List<ClaimProc> listClaimProcsAll=ClaimProcs.RefreshForProcs(ProcList.Select(x => x.ProcNum).ToList());
			//Get data for any OrthoCases that may be linked to procs in ProcList
			List<OrthoProcLink> listOrthoProcLinksAllForPat=new List<OrthoProcLink>();
			Dictionary<long,OrthoProcLink> dictOrthoProcLinksForProcList=new Dictionary<long,OrthoProcLink>();
			Dictionary<long,OrthoCase> dictOrthoCases=new Dictionary<long,OrthoCase>();
			Dictionary<long,OrthoSchedule> dictOrthoSchedules=new Dictionary<long,OrthoSchedule>();
			OrthoCases.GetDataForListProcs(ref listOrthoProcLinksAllForPat,ref dictOrthoProcLinksForProcList,ref dictOrthoCases,ref dictOrthoSchedules,ProcList);
			OrthoCase orthoCase=null;
			OrthoSchedule orthoSchedule=null;
			List<OrthoProcLink> listOrthoProcLinksForOrthoCase=null;
			foreach(Procedure proc in ProcList){
				bool hasChanged=false;
				bool hasDateChanged=false;
				List<ClaimProc> listClaimProcsForProc=ClaimProcs.GetForProc(listClaimProcsAll,proc.ProcNum);
				Procedure procOld=_procOldList.Find(x => x.ProcNum==proc.ProcNum);//this shouldn't fail, it needs to be in this list.
				if(procDate!=DateTime.MinValue && proc.ProcDate!=procDate) {//Using value entered in the textbox.
					proc.ProcDate=procDate;
					//ClaimProc.ProcDate will be "synched" in Procedures.ComputeEstimates(), but only if not attached to a claim.  Mimics FormprocEdit.
					hasDateChanged=true;
					hasChanged=true;
				}
				if(comboProv.GetSelected<Provider>()!=null && comboProv.GetSelected<Provider>().ProvNum!=proc.ProvNum) {//Using selection
					proc.ProvNum=comboProv.GetSelected<Provider>().ProvNum;
					//Mimics FormProcEdit, uses different criteria than Procedures.ComputeEstimates().
					ClaimProcs.TrySetProvFromProc(proc,listClaimProcsForProc);
					hasChanged=true;
				}
				if(comboClinic.GetSelected<Clinic>()!=null && comboClinic.GetSelected<Clinic>().ClinicNum!=proc.ClinicNum) {//Using selection
					proc.ClinicNum=comboClinic.GetSelected<Clinic>().ClinicNum;
					listClaimProcsForProc.ForEach(x => x.ClinicNum=proc.ClinicNum);
					hasChanged=true;
				}
				if(hasChanged) {
					Procedures.Update(proc,procOld);
					if(hasDateChanged) {
						List<ClaimProc> listNewClaimProcs;
						dictOrthoProcLinksForProcList.TryGetValue(proc.ProcNum,out OrthoProcLink orthoProcLink);
						//If proc is linked to an OrthoCase, update dates for OrthoCase.
						if(orthoProcLink!=null) {
							OrthoCases.UpdateDatesByLinkedProc(orthoProcLink,proc);
						}
						OrthoCases.FillOrthoCaseObjectsForProc(proc.ProcNum,ref orthoProcLink,ref orthoCase,ref orthoSchedule,ref listOrthoProcLinksForOrthoCase,
					dictOrthoProcLinksForProcList,dictOrthoCases,dictOrthoSchedules,listOrthoProcLinksAllForPat);
						RecomputeEstimates(proc,listClaimProcsForProc,out listNewClaimProcs,orthoProcLink,orthoCase,orthoSchedule,listOrthoProcLinksForOrthoCase);
						ClaimProcs.InsertMany(listNewClaimProcs);//Only insert claimProcs that were newly added.
					}
				}
			}
			ClaimProcs.UpdateMany(listClaimProcsAll);//Does not contain new claimProcs.
			foreach(long patNum in listDistinctPatNums) {//Should be a single patient.
				Recalls.Synch(patNum);
			}
			#endregion
			#region Security Log Entries
			string logTextProcComplete ="";//To make security log for procs that were C (specific permission)
			string logTextProcExisting="";//To make a security log for procs that were EO,EC (specific permission) 
			string logTextProcOther="";//All other procedures (general permission)
			foreach(long patNum in listDistinctPatNums) {
				foreach(Procedure proc in ProcList) {//necessary because of different original values
					Procedure procOld=_procOldList.FirstOrDefault(x => x.ProcNum==proc.ProcNum);
					if(procOld==null) {
						continue;
					}
					//If proc has not been changed then a blank string will be returned.
					//Only changed procs will be reflected in security log entries.
					switch(procOld.ProcStatus) {
						case ProcStat.C:
							logTextProcComplete+=ConstructSecurityLogForProcType(proc,procOld);
							break;
						case ProcStat.EO:
						case ProcStat.EC:
							logTextProcExisting+=ConstructSecurityLogForProcType(proc,procOld);
							break;
						default:
							logTextProcOther+=ConstructSecurityLogForProcType(proc,procOld);
							break;
					}
				}
				if(logTextProcComplete!="") {
					SecurityLogs.MakeLogEntry(Permissions.ProcCompleteEdit,patNum,logTextProcComplete);
				}
				if(logTextProcExisting!="") {
					SecurityLogs.MakeLogEntry(Permissions.ProcExistingEdit,patNum,logTextProcExisting);
				}
				if(logTextProcOther!="") {
					SecurityLogs.MakeLogEntry(Permissions.ProcEdit,patNum,logTextProcOther);
				}
			}
			#endregion
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private enum SecurityLogFields {
			ProcDate,
			ProvNum,
			ClinicNum
		}

		
	}
}