using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormFeeSchedTools :FormODBase {
		///<summary>The defNum of the fee schedule that is currently displayed in the main window.</summary>
		private long _schedNum;
		private List<FeeSched> _listFeeScheds;
		private List<Provider> _listProviders;
		private List<Clinic> _listClinics;

		///<summary>A list of security logs that should be inserted.</summary>
		private List<string> _listSecurityLogEntries=new List<string>();

		///<summary>Supply the fee schedule num(DefNum) to which all these changes will apply</summary>
		public FormFeeSchedTools(long schedNum,List<FeeSched> listFeeScheds,List<Provider> listProviders,List<Clinic> listClinics) {
			// Required for Windows Form Designer support
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_schedNum=schedNum;
			_listFeeScheds=listFeeScheds;
			_listProviders=listProviders;
			_listClinics=listClinics;//this was just simply Clinics.GetForUserod(Security.CurUser) from the parent form
		}

		private void FormFeeSchedTools_Load(object sender, System.EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FeeSchedEdit)) {
				DialogResult=DialogResult.Cancel;
				Close();
				return;
			}
			//Only unrestricted users should be using fee tools with feeschedgroups.
			checkShowGroups.Visible=(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !Security.CurUser.ClinicIsRestricted);
			LayoutManager.MoveLocation(comboGroup,new Point(LayoutManager.Scale(100),LayoutManager.Scale(46)));
			LayoutManager.MoveLocation(comboGroupTo,new Point(LayoutManager.Scale(100),LayoutManager.Scale(45)));
			FillComboBoxes();
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				butImportCanada.Visible=false;
			}
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				groupGlobalUpdateFees.Enabled=false;
			}
		}

		private void FillComboBoxes() {
			long feeSchedNum1Selected=0;//Default to the first 
			if(comboFeeSched.SelectedIndex > -1) {
				feeSchedNum1Selected=_listFeeScheds[comboFeeSched.SelectedIndex].FeeSchedNum;
			}
			long feeSchedNum2Selected=0;//Default to the first
			if(comboFeeSchedTo.SelectedIndex > -1) {
				feeSchedNum2Selected=_listFeeScheds[comboFeeSchedTo.SelectedIndex].FeeSchedNum;
			}
			//The number of clinics and providers cannot change while inside this window.  Always reselect exactly what the user had before.
			int comboProvIdx=comboProvider.SelectedIndex;
			int comboProvToIdx=comboProviderTo.SelectedIndex;
			long feeSchedGroupNum=0; 
			if(comboGroup.GetSelected<FeeSchedGroup>()!=null) {
				feeSchedGroupNum=comboGroup.GetSelected<FeeSchedGroup>().FeeSchedGroupNum;
			}
			long feeSchedGroupToNum=0;
			if(comboGroupTo.GetSelected<FeeSchedGroup>()!=null) {
				feeSchedGroupToNum=comboGroupTo.GetSelected<FeeSchedGroup>().FeeSchedGroupNum;
			}
			comboFeeSched.Items.Clear();
			comboFeeSchedTo.Items.Clear();
			comboGroup.Items.Clear();
			comboGroupTo.Items.Clear();
			comboProvider.Items.Clear();
			comboProviderTo.Items.Clear();
			//Fee Schedules
			string description;
			for(int i=0;i<_listFeeScheds.Count;i++) {
				description=_listFeeScheds[i].Description;
				if(_listFeeScheds[i].FeeSchedType!=FeeScheduleType.Normal) {
					description+=" ("+_listFeeScheds[i].FeeSchedType.ToString()+")";
				}
				comboFeeSched.Items.Add(description);
				comboFeeSchedTo.Items.Add(description);
				if(_listFeeScheds[i].FeeSchedNum==feeSchedNum1Selected) {
					comboFeeSched.SelectedIndex=i;
				}
				if(_listFeeScheds[i].FeeSchedNum==feeSchedNum2Selected) {
					comboFeeSchedTo.SelectedIndex=i;
				}
			}
			if(_listFeeScheds.Count==0) {//No fee schedules in the database so set the first item to none.
				comboFeeSched.Items.Add(Lan.g(this,"None"));
				comboFeeSchedTo.Items.Add(Lan.g(this,"None"));
			}
			if(comboFeeSched.SelectedIndex==-1) {
				comboFeeSched.SelectedIndex=0;
			}
			if(comboFeeSchedTo.SelectedIndex==-1) {
				comboFeeSchedTo.SelectedIndex=0;
			}
			if(!PrefC.HasClinicsEnabled) {//No clinics
				//For UI reasons, leave the clinic combo boxes visible for users not using clinics and they will just say "none".
				comboClinic.Enabled=false;
				comboClinic.HqDescription="None";
			}
			//Fee Sched Groups
			if(checkShowGroups.Visible) {//Always run the fill logic if they are using the groups feaure, not just if the combobox is showing.
				FillFeeSchedGroupComboBox(comboGroup,_listFeeScheds[comboFeeSched.SelectedIndex].FeeSchedNum,feeSchedGroupNum);
				FillFeeSchedGroupComboBox(comboGroupTo,_listFeeScheds[comboFeeSchedTo.SelectedIndex].FeeSchedNum,feeSchedGroupToNum);
			}
			//Providers
			comboProvider.Items.Add(Lan.g(this,"None"));
			comboProviderTo.Items.Add(Lan.g(this,"None"));
			for(int i=0;i<_listProviders.Count;i++) {
				comboProvider.Items.Add(_listProviders[i].Abbr);
				comboProviderTo.Items.Add(_listProviders[i].Abbr);
			}
			//comboClinic.SelectedClinicNum=comboClinic.SelectedClinicNum > -1 ? comboClinic.SelectedClinicNum:0;
			comboProvider.SelectedIndex=0;
			if(comboProvIdx > -1) {
				comboProvider.SelectedIndex=comboProvIdx;
			}
			comboProviderTo.SelectedIndex=0;
			if(comboProvToIdx > -1 ) {
				comboProviderTo.SelectedIndex=comboProvToIdx;
			}
			//Global----------------------------------------------------------------------------------------------------
			if(_listFeeScheds[comboFeeSched.SelectedIndex].IsGlobal) {
				comboClinic.Enabled=false;
				comboClinic.HqDescription="None";
				comboClinic.IsUnassignedSelected=true;
				butPickGroup.Enabled=false;
				comboProvider.Enabled=false;
				butPickProv.Enabled=false;
				comboProvider.SelectedIndex=0;
				comboGroup.Enabled=false;
				comboGroup.SelectedIndex=-1;
			}
			else {
				if(PrefC.HasClinicsEnabled) {
					comboClinic.Enabled=true;
					comboClinic.HqDescription="Default";
					butPickGroup.Enabled=true;
					comboGroup.Enabled=true;
				}
				comboProvider.Enabled=true;
				butPickProv.Enabled=true;
			}
			if(_listFeeScheds[comboFeeSchedTo.SelectedIndex].IsGlobal) {
				comboClinicTo.Enabled=false;
				comboClinicTo.HqDescription="None";
				comboClinicTo.IsUnassignedSelected=true;
				comboProviderTo.Enabled=false;
				butPickProvTo.Enabled=false;
				butPickGroupTo.Enabled=false;
				comboProviderTo.SelectedIndex=0;
				comboGroupTo.Enabled=false;
				comboGroupTo.SelectedIndex=-1;
			}
			else {
				if(PrefC.HasClinicsEnabled) {
					comboClinicTo.Enabled=true;
					comboClinicTo.HqDescription="Default";
					comboGroupTo.Enabled=true;
					butPickGroupTo.Enabled=true;
				}
				comboProviderTo.Enabled=true;
				butPickProvTo.Enabled=true;
			}
		}

		private void FillFeeSchedGroupComboBox(UI.ComboBox comboFeeSchedGroup,long feeSchedNumSelected,long feeSchedGroupNum) {
			List<long> listComboGroupNums=new List<long>();
			List<FeeSchedGroup> listFeeSchedGroups=FeeSchedGroups.GetAllForFeeSched(feeSchedNumSelected);
			for(int i=0;i<listFeeSchedGroups.Count;i++)  {
				if(!listComboGroupNums.Contains(listFeeSchedGroups[i].FeeSchedGroupNum) && listFeeSchedGroups[i].ListClinicNumsAll.Count>0) {//Skip duplicate/empty groups.
					comboFeeSchedGroup.Items.Add(listFeeSchedGroups[i].Description,listFeeSchedGroups);
					listComboGroupNums.Add(listFeeSchedGroups[i].FeeSchedGroupNum);
					//Set this fee sched group as the selection if it matches the previous selection before refreshing
					if(listFeeSchedGroups[i].FeeSchedGroupNum==feeSchedGroupNum) {
						comboFeeSchedGroup.SetSelectedKey<FeeSchedGroup>(listFeeSchedGroups[i].FeeSchedGroupNum, x => x.FeeSchedGroupNum);
					}
				}
			}
		}

		private void butClear_Click(object sender, System.EventArgs e) {
			if(PrefC.HasClinicsEnabled) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear all values from the selected fee schedule for the currently selected clinic and provider.  Are you sure you want to continue?")) {
					return;
				}
			}
			else if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear all values from the selected fee schedule for the currently selected provider.  Are you sure you want to continue?")) {
				return;
			}
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled && !comboClinic.IsUnassignedSelected){
				listClinicNums.Add(comboClinic.SelectedClinicNum);
			}
			long feeSchedNum=_listFeeScheds[comboFeeSched.SelectedIndex].FeeSchedNum;
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,comboClinic.SelectedClinicNum);//get the selected clinic num
				if(feeSchedGroup!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+feeSchedGroup.Description
						+Lans.g(this," and must be cleared at the group level."));
					return;
				}
			}
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select a Fee Schedule Group.");
					return;
				}
				//Fees.ImportFees() will update the rest of the group.
				listClinicNums.AddRange(comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll);
			}
			if(listClinicNums.IsNullOrEmpty()) {
				listClinicNums.Add(0);
			}
			long provNum=0;
			if(comboProvider.SelectedIndex!=0) {
				provNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			//ODProgress.ShowAction(() => {
			string logText="";
			for(int i=0; i<listClinicNums.Count;i++)  {
				Fees.DeleteFees(feeSchedNum,listClinicNums[i],provNum);
				logText+=Lan.g(this,"Procedures for Fee Schedule")+" "+FeeScheds.GetDescription(feeSchedNum);
				if(PrefC.HasClinicsEnabled) {
					if(Clinics.GetAbbr(Clinics.ClinicNum)=="") {
						logText+=Lan.g(this," at Headquarters");
					}
					else {
						logText+=Lan.g(this," at clinic")+" "+Clinics.GetAbbr(Clinics.ClinicNum);
					}
				}
				logText+=" "+Lan.g(this,"were all cleared.")+"\r\n";
			}
			SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,logText);
			//	});
			MsgBox.Show(this,"Done");
		}

		private void butCopy_Click(object sender, System.EventArgs e) {
			List<long> listClinicNumsTo=comboClinicTo.ListSelectedClinicNums;
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex<0 || comboGroupTo.SelectedIndex<0) {
					MsgBox.Show(this,"Please select a Fee Schedule group.");
					return;
				}
				listClinicNumsTo=comboGroupTo.GetSelected<FeeSchedGroup>().ListClinicNumsAll;
			}
			if(PrefC.HasClinicsEnabled && listClinicNumsTo.Count==0) {
				MsgBox.Show(this,"At least one \"Clinic To\" clinic must be selected.");
				return;
			}
			long toProvNum=0;
			if(comboProviderTo.SelectedIndex!=0) {
				toProvNum=_listProviders[comboProviderTo.SelectedIndex-1].ProvNum;
			}
			FeeSched feeSchedTo=_listFeeScheds[comboFeeSchedTo.SelectedIndex];
			long fromClinicNum=0;
			if(PrefC.HasClinicsEnabled && !comboClinic.IsUnassignedSelected){
				fromClinicNum=comboClinic.SelectedClinicNum;//get the current clinic num if it is not unassigned
			}
			long fromProvNum=0;
			if(comboProvider.SelectedIndex!=0) {
				fromProvNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			if(checkShowGroups.Checked) {
				//verify we aren't copying the same group into itself
				if(comboGroup.GetSelected<FeeSchedGroup>().FeeSchedGroupNum==comboGroupTo.GetSelected<FeeSchedGroup>().FeeSchedGroupNum && fromProvNum==toProvNum) {
					MsgBox.Show(this,"Fee Schedule Groups are not allowed to be copied into themselves. Please choose another fee schedule group to copy.");
					return;
				}
				//Get fromclinicnum from list of group clinics.
				fromClinicNum=comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
			}
			FeeSched feeSched=_listFeeScheds[comboFeeSched.SelectedIndex];
			if(feeSched.FeeSchedNum==feeSchedTo.FeeSchedNum && fromProvNum==toProvNum) {
				if(!PrefC.HasClinicsEnabled || listClinicNumsTo.Contains(fromClinicNum)) {//If clinics disabled, can cause false negative so shortcircuit
					MsgBox.Show(this,"Fee Schedules are not allowed to be copied into themselves. Please choose another fee schedule to copy.");
					return;
				}
			}
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				//Pref is on but we are copying clinics.
				for(int i=0;i<listClinicNumsTo.Count;i++) {
					FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedTo.FeeSchedNum,listClinicNumsTo[i]);
					if(feeSchedGroup!=null) {
						Clinic clinic=Clinics.GetClinic(listClinicNumsTo[i]);
						MsgBox.Show(this,Lans.g(this,"Clinic: ")+clinic.Abbr+Lans.g(this," is a member of Fee Schedule Group: ")+feeSchedGroup.Description
							+Lans.g(this," for the selected Fee Schedule and must be copied at the Fee Schedule Group level."));
						return;
					}
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All fees that exactly match the \"Copy To\" fee schedule/clinic/provider combination will be deleted.  Then new fees will be copied in.  Are you sure you want to continue?")){
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => FeeScheds.CopyFeeSchedule(feeSched,fromClinicNum,fromProvNum,feeSchedTo,listClinicNumsTo,toProvNum);
			progressOD.StartingMessage="Preparing to copy fees...";
			progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			//After finishing, clear the Copy To section, but leave the Copy From section as is.
			comboFeeSchedTo.SelectedIndex=0;
			comboClinicTo.IsNothingSelected=true;
			comboProviderTo.SelectedIndex=0;
			long feeSchedGroupToNum=0;
			if(comboGroupTo.GetSelected<FeeSchedGroup>()!=null) {
				feeSchedGroupToNum=comboGroupTo.GetSelected<FeeSchedGroup>().FeeSchedGroupNum;
			}
			comboGroupTo.Items.Clear();
			FillFeeSchedGroupComboBox(comboGroupTo,_listFeeScheds[comboFeeSchedTo.SelectedIndex].FeeSchedNum,feeSchedGroupToNum);
			MsgBox.Show(this,"Done.");
		}

		private void butIncrease_Click(object sender, System.EventArgs e) {
			int percent=0;
			if(textPercent.Text==""){
				MsgBox.Show(this,"Please enter a percent first.");
				return;
			}
			try{
				percent=System.Convert.ToInt32(textPercent.Text);
			}
			catch{
				MsgBox.Show(this,"Percent is not a valid number.");
				return;
			}
			if(percent<-99 || percent>99){
				MsgBox.Show(this,"Percent must be between -99 and 99.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will overwrite all values of the selected fee schedule/clinic/provider combo.  Previously entered fee "
				+"amounts will be lost.  It is recommended to first create a backup copy of the original fee schedule, then update the original fee schedule "
				+"with the new fees.  Are you sure you want to continue?"))
			{
				return;
			}
			long clinicNum=0;
			if(PrefC.HasClinicsEnabled && !comboClinic.IsUnassignedSelected){
				clinicNum=comboClinic.SelectedClinicNum;
			}
			long feeSchedNum=_listFeeScheds[comboFeeSched.SelectedIndex].FeeSchedNum;
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,clinicNum);
				if(feeSchedGroup!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+feeSchedGroup.Description
						+Lans.g(this," and must be increased at the group level."));
					return;
				}
			}
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select a Fee Schedule Group.");
					return;
				}
				//Fees.ImportFees() will update the rest of the group.
				clinicNum=comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
			}
			long provNum=0;
			if(comboProvider.SelectedIndex>0){
				provNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			List<Fee> listFees=Fees.GetListExact(feeSchedNum,clinicNum,provNum);
			bool doIncreaseFees=EvaluateOverrides(clinicNum,provNum,feeSchedNum,listFees);
			if(!doIncreaseFees) {
				return;//either no fees would be updated or the user chose to cancel and review so don't increase fees.
			}
			int round=0;//Default to dollar
			if(radioDime.Checked){
				round=1;
			}
			if(radioPenny.Checked){
				round=2;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => {
				listFees=Fees.IncreaseNew(feeSchedNum,percent,round,listFees,clinicNum,provNum);
				if(PrefC.GetBool(PrefName.ShowFeeSchedGroups)) {
					FeeSchedGroups.UpsertGroupFees(listFees.FindAll(x => x.Amount!=0));
				}
				string procCode;
				for(int i=0;i<listFees.Count;i++) {
					if(listFees[i].Amount==0) {
						continue;
					}
					try {
						procCode=ProcedureCodes.GetStringProcCode(listFees[i].CodeNum);
					}
					catch(Exception) {//if CodeNum is not in the procedurecode table, don't make securitylog entry
						continue;
					}
					Fees.Update(listFees[i],doCheckFeeSchedGroups:false);//only a few hundred calls, max
					string logText="Procedure: "+procCode+", "
						+"Fee: "+listFees[i].Amount.ToString("c")+", "
						+"Fee Schedule: "+FeeScheds.GetDescription(listFees[i].FeeSched);
					if(PrefC.HasClinicsEnabled) {
						if(Clinics.GetAbbr(clinicNum)=="") {
							logText+=", at Headquarters";
						}
						else {
							logText+=", at clinic: "+Clinics.GetAbbr(clinicNum);
						}
					}
					if(provNum!=0) {
						logText+=", for provider: "+Providers.GetAbbr(provNum);
					}
					logText+=". Fee increased by "+((float)percent/100.0f).ToString("p")+" using the increase "
						+"button in the Fee Tools window.";
					SecurityLogs.MakeLogEntry(Permissions.ProcFeeEdit,0,logText,listFees[i].CodeNum,DateTime.MinValue);
					SecurityLogs.MakeLogEntry(Permissions.LogFeeEdit,0,"Fee Updated",listFees[i].FeeNum,listFees[i].SecDateTEdit);
					ProgressBarEvent.Fire(ODEventType.ProgressBar,"Modifying fees, please wait...");
				}
			};
			progressOD.StartingMessage=Lan.g(this,"Preparing to modify fees")+"...";
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			MsgBox.Show(this,"Done.");
		}

		///<summary>Determines if there are overrides being updated, or just a regular feeSchedule. Returns true if there are overrides and user wants to 
		///continue, or if it is a regular fee schedule. Returns false is it is an override schedule and there are no overrides to update or if user 
		///chooses to cancel to review.</summary>
		private bool EvaluateOverrides(long clinicNum,long provNum,long feeSchedNum,List<Fee> listFees) {
			//listFees only includes exact matches 
			int countTotalFeesForSched=Fees.GetCountByFeeSchedNum(feeSchedNum);
			string msgText="";
			string clinicName=Clinics.GetAbbr(clinicNum);
			string provName=Providers.GetAbbr(provNum);
			string feeSchedDesc=_listFeeScheds.FirstOrDefault(x => x.FeeSchedNum==feeSchedNum).Description;
			if(clinicNum==0 && provNum==0){
				return true;
			}
			if(clinicNum!=0 && provNum!=0) {//user seems to be trying to increase fee overrides for clinic and prov
				if(listFees.Count==0) {//but there aren't any.
					msgText="There are no overrides for clinic '"+clinicName+"' and provider '"+provName+"' "
						+"so no fees will be updated. If you want to create overrides, first enter or copy fees into that override fee schedule.";
					MessageBox.Show(msgText);
					return false;//don't run increase tool
				}
				if(listFees.Count!=countTotalFeesForSched) {//
					msgText="There are "+listFees.Count+" override fees for clinic '"+clinicName+"' and provider"
						+" '"+provName+"' and there are "+countTotalFeesForSched+" total fees for fee schedule '"
						+feeSchedDesc+"'. Only the "+listFees.Count+" fees will be increased.  Cancel if you want to review first.";
					if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return false;
					}
				}
				return true;
			}
			else if(clinicNum!=0) {
				if(listFees.Count==0) {
					msgText="There are no overrides for clinic '"+clinicName+"' "
						+"so no fees will be updated. If you want to create overrides, first enter or copy fees into that override fee schedule.";
					MessageBox.Show(msgText);
					return false;
				}
				if(listFees.Count!=countTotalFeesForSched) {
					if(checkShowGroups.Checked){
						//We know at this point we already selected a valid feeschedgroup and don't need a null check.
						FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,clinicNum);
						msgText="There are "+listFees.Count+" override fees for group '"+feeSchedGroup.Description+"' and there are"
							+" "+countTotalFeesForSched+" total fees for fee schedule '"+feeSchedDesc+"'. Only the "
							+listFees.Count+" fees will be increased. Cancel if you want to review first.";
						if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false;
						}
					}
					else {
						msgText="There are "+listFees.Count+" override fees for clinic '"+clinicName+"' and there are"
							+" "+countTotalFeesForSched+" total fees for fee schedule '"+feeSchedDesc+"'. Only the "
							+listFees.Count+" fees will be increased. Cancel if you want to review first.";
						if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
							return false;
						}
					}
				}
				return true;	
			}
			else if(provNum!=0) {
				if(listFees.Count==0) {
					msgText="There are no overrides for provider '"+provName+"' "
						+"so no fees will be updated. If you want to create overrides, first enter or copy fees into that override fee schedule.";
					MessageBox.Show(msgText);
					return false;
				}
				if(listFees.Count!=countTotalFeesForSched) {
					msgText="There are "+listFees.Count+" override fees for provider '"+provName+"' and there are"
						+" "+countTotalFeesForSched+" total fees for fee schedule '"+feeSchedDesc+"'. Only the "
						+listFees.Count+" fees will be increased. Cancel if you want to review first.";
					if(MessageBox.Show(msgText,"",MessageBoxButtons.OKCancel)==DialogResult.Cancel) {
						return false;
					}
				}
				return true;
			}
			return true;//will never hit
		}

		private void butExport_Click(object sender,EventArgs e) {
			long feeSchedNum=_listFeeScheds[comboFeeSched.SelectedIndex].FeeSchedNum;
			string feeSchedDesc=FeeScheds.GetDescription(feeSchedNum);
			//scrub out any non-AlphaNumeric characters.
			feeSchedDesc=Regex.Replace(feeSchedDesc,"(?:[^a-z0-9 ]|(?<=['\"])s)","",RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
			FeeSched feeSched=_listFeeScheds[comboFeeSched.SelectedIndex];
			long clinicNum=0;
			if(!comboClinic.IsUnassignedSelected) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select a Fee Schedule Group.");
					return;
				}
				clinicNum=comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
			}
			long provNum=0;
			if(comboProvider.SelectedIndex!=0) {
				provNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			string fileName="Fees"+feeSchedDesc+".txt";
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				Cursor=Cursors.WaitCursor;
				using SaveFileDialog saveFileDialog=new SaveFileDialog();
				if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					saveFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				else if(Directory.Exists("C:\\")) {
					saveFileDialog.InitialDirectory="C:\\";
				}
				saveFileDialog.FileName=fileName;
				if(saveFileDialog.ShowDialog()!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
				filePath=saveFileDialog.FileName;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => FeeScheds.ExportFeeSchedule(feeSched.FeeSchedNum,clinicNum,provNum,filePath);
			progressOD.StartingMessage=Lan.g(this,"Preparing to export fees")+"...";
			progressOD.ProgStyle=ProgressBarStyle.Blocks;
			progressOD.ShowDialogProgress();
			if(progressOD.IsCancelled){
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Fee schedule exported.");
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If you want a clean slate, you should clear the current fee schedule first.  When imported, any fees that are found in the text file will overwrite values of the selected fee schedule/clinic/provider combo.  Are you sure you want to continue?")) 
			{
				return;
			}
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				openFileDialog.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			else if(Directory.Exists("C:\\")) {
				openFileDialog.InitialDirectory="C:\\";
			}
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(!File.Exists(openFileDialog.FileName)){
				MsgBox.Show(this,"File not found");
				return;
			}
			//Import deletes fee if it exists and inserts new fees based on fee settings.
			long clinicNum=0;
			if(!comboClinic.IsUnassignedSelected) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			FeeSched feeSched=_listFeeScheds[comboFeeSched.SelectedIndex];
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(feeSchedGroup!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+feeSchedGroup.Description
						+Lans.g(this," and must be imported at the group level."));
					return;
				}
			}
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select a Fee Schedule Group.");
					return;
				}
				//Fees.ImportFees() will update the rest of the group.
				clinicNum=comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
			}
			long provNum=0;
			if(comboProvider.SelectedIndex!=0) {
				provNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => FeeL.ImportFees(openFileDialog.FileName,feeSched.FeeSchedNum,clinicNum,provNum);
			progressOD.StartingMessage="Importing fees...";
			progressOD.ProgStyle=ProgressBarStyle.Blocks;
			try{
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				FriendlyException.Show("Error importing fees.",ex);
				return;
			}
			if(progressOD.IsCancelled){
				return;
			}
			MsgBox.Show(this,"Fee schedule imported.");
		}

		private void butImportCanada_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"If you want a clean slate, the current fee schedule should be cleared first.  When imported, any fees that are found in the text file will overwrite values of the current fee schedule showing in the main window.  Are you sure you want to continue?")) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			using FormFeeSchedPickRemote formFeeSchedPickRemote=new FormFeeSchedPickRemote();
			formFeeSchedPickRemote.Url=@"http://www.opendental.com/feescanada/";//points to index.php file
			if(formFeeSchedPickRemote.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.WaitCursor;//original wait cursor seems to go away for some reason.
			Application.DoEvents();
			string feeData="";
			Action actionCloseFeeSchedImportCanadaProgress=ODProgress.Show(ODEventType.FeeSched,typeof(FeeSchedEvent));
			if(formFeeSchedPickRemote.IsFileChosenProtected()) {
				actionCloseFeeSchedImportCanadaProgress?.Invoke();//Hide the progress window so it does not cover up the authorization form.
				using FormFeeSchedPickAuthOntario formFeeSchedPickAuthOntario=new FormFeeSchedPickAuthOntario(formFeeSchedPickRemote.GetSelectedFeeDA());
				if(formFeeSchedPickAuthOntario.ShowDialog()!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
				actionCloseFeeSchedImportCanadaProgress=ODProgress.Show(ODEventType.FeeSched,typeof(FeeSchedEvent));
				string memberNumberODA=formFeeSchedPickAuthOntario.getODAMemberNumber();
				string memberPasswordODA=formFeeSchedPickAuthOntario.getODAMemberPassword(); 
				//prepare the xml document to send--------------------------------------------------------------------------------------
				XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
				xmlWriterSettings.Indent = true;
				xmlWriterSettings.IndentChars = ("    ");
				StringBuilder stringBuilder=new StringBuilder();
				using XmlWriter xmlWriter=XmlWriter.Create(stringBuilder,xmlWriterSettings);
				xmlWriter.WriteStartElement("RequestFeeSched");
				xmlWriter.WriteStartElement("RegistrationKey");
				xmlWriter.WriteString(PrefC.GetString(PrefName.RegistrationKey));
				xmlWriter.WriteEndElement();//RegistrationKey
				xmlWriter.WriteStartElement("FeeSchedFileName");
				xmlWriter.WriteString(formFeeSchedPickRemote.getFileChosenName());
				xmlWriter.WriteEndElement();//FeeSchedFileName
				if(memberNumberODA!="") {
					xmlWriter.WriteStartElement("ODAMemberNumber");
					xmlWriter.WriteString(memberNumberODA);
					xmlWriter.WriteEndElement();//ODAMemberNumber
					xmlWriter.WriteStartElement("ODAMemberPassword");
					xmlWriter.WriteString(memberPasswordODA);
					xmlWriter.WriteEndElement();//ODAMemberPassword
				}
				xmlWriter.WriteEndElement();//RequestFeeSched
				xmlWriter.Close();
		
#if DEBUG
				OpenDental.localhost.Service1 updateService=new OpenDental.localhost.Service1();
#else
				OpenDental.customerUpdates.Service1 updateService=new OpenDental.customerUpdates.Service1();
				updateService.Url=PrefC.GetString(PrefName.UpdateServerAddress);
#endif
				//Send the message and get the result-------------------------------------------------------------------------------------
				string result="";
				try {
					FeeSchedEvent.Fire(ODEventType.FeeSched,Lan.g(this,"Retrieving fee schedule")+"...");
					result=updateService.RequestFeeSched(stringBuilder.ToString());
				}
				catch(Exception ex) {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					Cursor=Cursors.Default;
					MessageBox.Show("Error: "+ex.Message);
					return;
				}
				Cursor=Cursors.Default;
				XmlDocument xmlDocument=new XmlDocument();
				xmlDocument.LoadXml(result);
				//Process errors------------------------------------------------------------------------------------------------------------
				XmlNode xmlNode=xmlDocument.SelectSingleNode("//Error");
				if(xmlNode!=null) {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					MessageBox.Show(xmlNode.InnerText,"Error");
					return;
				}
				xmlNode=xmlDocument.SelectSingleNode("//KeyDisabled");
				if(xmlNode==null) {
					//no error, and no disabled message
					if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
						DataValid.SetInvalid(InvalidType.Prefs);
					}
				}
				else {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					MessageBox.Show(xmlNode.InnerText);
					if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
						DataValid.SetInvalid(InvalidType.Prefs);
					}
					return;
				}
				//Process a valid return value------------------------------------------------------------------------------------------------
				xmlNode=xmlDocument.SelectSingleNode("//ResultCSV64");
				string feeData64=xmlNode.InnerXml;
				byte[] byteArrayFeeData=Convert.FromBase64String(feeData64);
				feeData=Encoding.UTF8.GetString(byteArrayFeeData);
			}
			else {
				FeeSchedEvent.Fire(ODEventType.FeeSched,Lan.g(this,"Downloading fee schedule")+"...");
				string tempFile=PrefC.GetRandomTempFile(".tmp");
				WebClient webClient=new WebClient();
				try {
					webClient.DownloadFile(formFeeSchedPickRemote.getFileChosenUrl(),tempFile);
				}
				catch(Exception ex) {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					MessageBox.Show(Lan.g(this,"Failed to download fee schedule file")+": "+ex.Message);
					Cursor=Cursors.Default;
					return;
				}
				feeData=File.ReadAllText(tempFile);
				File.Delete(tempFile);
			}
			int numImported;
			int numSkipped;
			long clinicNum=0;
			if(!comboClinic.IsUnassignedSelected) {
				clinicNum=comboClinic.SelectedClinicNum;
			}
			FeeSched feeSched=_listFeeScheds[comboFeeSched.SelectedIndex];
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				FeeSchedGroup feeSchedGroup=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(feeSchedGroup!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+feeSchedGroup.Description
						+Lans.g(this," and must be imported at the group level."));
					return;
				}
			}
			if(checkShowGroups.Checked) {
				if(comboGroup.SelectedIndex==-1) {
					MsgBox.Show(this,"Please select a Fee Schedule Group.");
					return;
				}
				//Fees.ImportFees() will update the rest of the group.
				clinicNum=comboGroup.GetSelected<FeeSchedGroup>().ListClinicNumsAll.FirstOrDefault();
			}
			long provNum=0;
			if(comboProvider.SelectedIndex!=0) {
				provNum=_listProviders[comboProvider.SelectedIndex-1].ProvNum;
			}
			FeeScheds.ImportCanadaFeeSchedule2(feeSched,feeData,clinicNum,provNum,out numImported,out numSkipped);
			actionCloseFeeSchedImportCanadaProgress?.Invoke();
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
			string outputMessage="Done. Number imported: "+numImported;
			if(numSkipped>0) {
				outputMessage+=" Number skipped: "+numSkipped;
			}
			MessageBox.Show(outputMessage);
		}

		private void butUpdateFees_Click(object sender,EventArgs e) {
			long rowsChanged=0;
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,"All treatment planned procedures for all patients will be updated.  Only the fee will be updated, not the insurance "
				+"estimate.  It might take a few minutes.  Continue?")) {
				return;
			}
			ProgressBarHelper progressBarHelper=new ProgressBarHelper("Fee Schedule Update Progress",progressBarEventType:ProgBarEventType.Header);
			ODProgressExtended progressExtended=new ODProgressExtended(ODEventType.FeeSched,new FeeSchedEvent(),this,tag:progressBarHelper,cancelButtonText:Lan.g(this,"Close"));
			Cursor=Cursors.WaitCursor;
			List<Fee> listFeesHQ=Fees.GetByClinicNum(0);//All HQ fees
			if(PrefC.HasClinicsEnabled) {
				List<long> listFeeClinics=comboGlobalUpdateClinics.ListSelectedClinicNums;
				for(int i=0;i<listFeeClinics.Count;i++) {
					//Clinic clinicCur=listFeeClinics[i];
					while(progressExtended.IsPaused) {
						Thread.Sleep(10);
						if(progressExtended.IsCanceled) {
							break;
						}
					}
					if(progressExtended.IsCanceled) {
						break;
					}
					double percentComplete=(((double)i)/listFeeClinics.Count*100);
					if(listFeeClinics.Count>1) {
						progressBarHelper=new ProgressBarHelper("Overall",(int)percentComplete+"%",i,listFeeClinics.Count,tagString:"OverallStatus");
						progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
						progressBarHelper=new ProgressBarHelper(Clinics.GetAbbr(listFeeClinics[i]),"0%",1,100,tagString:"Clinic");
						progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
					}
					else {
						progressBarHelper=new ProgressBarHelper(Clinics.GetAbbr(listFeeClinics[i]),"0%",1,100,tagString:"Clinic");
						progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
						progressExtended.HideButtons();//can't pause or cancel with 1 clinic. This event needs to be called after the bar is instantiated. 
					}
					try {
						rowsChanged+=Procedures.GlobalUpdateFees(listFeesHQ,listFeeClinics[i],Clinics.GetAbbr(listFeeClinics[i]));
					}
					catch(ApplicationException ex) {
						Cursor=Cursors.Default;
						progressExtended.Close();
						MessageBox.Show(ex.Message);
						return;	
					}
					if(progressExtended.IsPaused) {
						progressExtended.AllowResume();
					}
				}
				if(listFeeClinics.Count>1) {
					progressBarHelper=new ProgressBarHelper("Overall","100%",100,100,tagString:"OverallStatus");
					progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
				}
			}
			else {//no clinic - "Clinic" here is just a reference to the progress bar that updates Clinic progress instead of overall progress
				progressBarHelper=new ProgressBarHelper("Updating...","0%",1,100,tagString:"Clinic");
				progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
				progressExtended.HideButtons();
				try {
					rowsChanged=Procedures.GlobalUpdateFees(listFeesHQ,0,"Updating...");
				}
				catch(ApplicationException ex) {
					Cursor=Cursors.Default;
					progressExtended.Close();
					MessageBox.Show(ex.Message);
					return;	
				}
			}
			progressExtended.OnProgressDone();
			progressBarHelper=new ProgressBarHelper("Treatment planned procedure fees changed: "+rowsChanged.ToString()+"\r\nDone.",
				progressBarEventType:ProgBarEventType.TextMsg);
			progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
			if(progressExtended.IsCanceled) {//close
				progressExtended.Close();
				DialogResult=DialogResult.OK;
			}
			Cursor=Cursors.Default;
		}

		private void butUpdateWriteoffs_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Write-off estimates will be recalculated for all treatment planned procedures.  This tool should only "
				+"be run if you have updated fee schedules and want to run reports on write-off estimates for patients that have not been viewed."
				+"\r\n\r\nThis could take a very long time.  Continue?"))
			{
				return;
			}
			List<long> listWriteoffClinics=comboGlobalUpdateClinics.ListSelectedClinicNums;
			bool doUpdatePrevClinicPref=comboGlobalUpdateClinics.IsAllSelected;//keeps track of current ClinicNum in db to resume if interrupted
			//MUST be in primary key order so that we will resume on the correct clinic and update the remaining clinics in the list
			if(doUpdatePrevClinicPref){
				listWriteoffClinics.Sort(); 
			}
			if(listWriteoffClinics.Count==0) {//This is assuming clinics are turned off
				listWriteoffClinics.Add(0);
			}
			int indexPrevClinic=-1;
			if(PrefC.HasClinicsEnabled
				&& !Security.CurUser.ClinicIsRestricted
				&& comboGlobalUpdateClinics.IsAllSelected 
				&& !string.IsNullOrEmpty(PrefC.GetString(PrefName.GlobalUpdateWriteOffLastClinicCompleted)))//previous 'All' run was interrupted, resume
			{
				try {
					long prevClinicNum=PrefC.GetLong(PrefName.GlobalUpdateWriteOffLastClinicCompleted);
					indexPrevClinic=listWriteoffClinics.FindIndex(x => x==prevClinicNum);
				}
				catch(Exception ex) {
					ex.DoNothing();//if pref is not a long, leave prevClinic as -1 so it will run as if it was not previously interrupted
				}
			}
			if(indexPrevClinic>-1 //only true if clinics are enabled, the user is not restricted, updating all clinics, and the pref has been set from previous run
				&& listWriteoffClinics.Count>indexPrevClinic+1) //we will skip indexPrevClinic+1 items and there needs to be at least one more clinic to process
			{
				string msgText=Lan.g(this,"This tool was paused or interrupted during a previous run.  Would you like to resume the previous run?")+"\r\n\r\n"
					+Lan.g(this,"Yes - Run the tool beginning where the previous run left off.")+"\r\n\r\n"
					+Lan.g(this,"No - Run the tool for all clinics and replace the previous run progress with the progress of this run.")+"\r\n\r\n"
					+Lan.g(this,"Cancel - Don't run the tool and retain the previous run progress.");
				DialogResult dialogResult=MessageBox.Show(this,msgText,"",MessageBoxButtons.YesNoCancel);
				if(dialogResult==DialogResult.Cancel) {
					return;
				}
				else if(dialogResult==DialogResult.Yes) {//pick up where last run left off and overwrite that last clinic with the progress from this run
					listWriteoffClinics.RemoveRange(0,indexPrevClinic+1);
				}
				else {
					//diagRes==DialogResult.No, run tool for all clinics and replace the previous run progress with the progress from this run
				}
			}
			ProgressBarHelper progressBarHelper=new ProgressBarHelper(Lan.g(this,"Write-off Update Progress"),progressBarEventType:ProgBarEventType.Header);
			ODProgressExtended progressExtended=new ODProgressExtended(ODEventType.FeeSched,new FeeSchedEvent(),this,tag:progressBarHelper
				,cancelButtonText:Lan.g(this,"Close"));
			progressBarHelper=new ProgressBarHelper("","0%",0,100,ProgBarStyle.Blocks,"WriteoffProgress");
			progressExtended.Fire(ODEventType.FeeSched,progressBarHelper);
			Cursor=Cursors.WaitCursor;
			try {
				FeeScheds.GlobalUpdateWriteoffs(listWriteoffClinics,progressExtended,doUpdatePrevClinicPref);
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				progressExtended.Close();
				MessageBox.Show(ex.Message);
			}
			finally {
				if(progressExtended.IsCanceled) {
					progressExtended.Close();
				}
				Cursor=Cursors.Default;
			}
		}

		private void butPickFeeSched_Click(object sender,EventArgs e) {
			//int selectedIndex=GetFeeSchedIndexFromPicker();
			//No need to check security because we are launching the form in selection mode.
			using FormFeeScheds formFeeScheds=new FormFeeScheds(true);
			formFeeScheds.ShowDialog();
			int selectedIndex= _listFeeScheds.FindIndex(x => x.FeeSchedNum==formFeeScheds.SelectedFeeSchedNum);//Returns index of the found element or -1.
			//If the selectedIndex is -1, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==-1) {
				return;
			}
			UI.Button buttonPick=(UI.Button)sender;
			if(buttonPick==butPickSched) { //First FeeSched combobox doesn't have "None" option.
				comboFeeSched.SelectedIndex=selectedIndex;
			}
			else if(buttonPick==butPickSchedTo) {
				comboFeeSchedTo.SelectedIndex=selectedIndex;
			}
			FillComboBoxes();
		}

		private void butPickGroup_Click(object sender,EventArgs e){
			if(checkShowGroups.Checked) {
				List<FeeSchedGroup> listFeeSchedGroupsToShow=comboGroup.Items.GetAll<FeeSchedGroup>();//s.AsEnumerable<ODBoxItem<FeeSchedGroup>>()).Select(x => x.Tag).ToList();
				List<GridColumn> listGridColumnsHeaders=new List<GridColumn>();
				GridColumn col=new GridColumn(Lan.g(this,"Description"),50);
				col.IsWidthDynamic=true;
				listGridColumnsHeaders.Add(col);
				List<GridRow> listGridRowValues=new List<GridRow>();
				for(int i=0;i<listFeeSchedGroupsToShow.Count;i++) {
					GridRow row=new GridRow(listFeeSchedGroupsToShow[i].Description);
					row.Tag=listFeeSchedGroupsToShow[i];
					listGridRowValues.Add(row);
				}
				string formTitle=Lan.g(this,"Fee Schedule Group Picker");
				string gridTitle=Lan.g(this,"Fee Schedule Groups");
				using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnsHeaders,listGridRowValues,formTitle,gridTitle);
				if(formGridSelection.ShowDialog()==DialogResult.OK) {
					comboGroup.SelectedIndex=listFeeSchedGroupsToShow.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)formGridSelection.ListSelectedTags[0]).FeeSchedGroupNum));
					return;
				}
				//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
				return;
			}
		}

		private void butPickGroupTo_Click(object sender,EventArgs e) {
			if(checkShowGroups.Checked) {
				List<FeeSchedGroup> listFeeSchedGroupsToShow=comboGroupTo.Items.GetAll<FeeSchedGroup>();//Items.AsEnumerable<ODBoxItem<FeeSchedGroup>>()).Select(x => x.Tag).ToList();
				List<GridColumn> listGridColumnsHeaders=new List<GridColumn>();
				GridColumn col=new GridColumn(Lan.g(this,"Description"),50);
				col.IsWidthDynamic=true;
				listGridColumnsHeaders.Add(col);
				List<GridRow> listGridRowValues=new List<GridRow>();
				for(int i=0;i<listFeeSchedGroupsToShow.Count;i++) {
					GridRow row=new GridRow(listFeeSchedGroupsToShow[i].Description);
					row.Tag=listFeeSchedGroupsToShow[i];
					listGridRowValues.Add(row);
				}
				string formTitle=Lan.g(this,"Fee Schedule Group Picker");
				string gridTitle=Lan.g(this,"Fee Schedule Groups");
				using FormGridSelection formGridSelection=new FormGridSelection(listGridColumnsHeaders,listGridRowValues,formTitle,gridTitle);
				if(formGridSelection.ShowDialog()==DialogResult.OK) {
					comboGroupTo.SelectedIndex=listFeeSchedGroupsToShow.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)formGridSelection.ListSelectedTags[0]).FeeSchedGroupNum));
					return;
				}
				//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
				return;
			}
		}

		private void butPickProvider_Click(object sender,EventArgs e){
			//int selectedIndex=-1;//GetProviderIndexFromPicker()+1;//All provider combo boxes have a none option, so always add 1.
			using FormProviderPick formProviderPick=new FormProviderPick();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;// -1;
			}
			int selectedIndex= Providers.GetIndex(formProviderPick.ProvNumSelected)+1;//All provider combo boxes have a none option, so always add 1.
			//If the selectedIndex is 0, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==0) {
				return;
			}
			UI.Button butPick=(UI.Button)sender;
			if(butPick==butPickProv) {
				comboProvider.SelectedIndex=selectedIndex;
			}
			else if(butPick==butPickProvTo) {
				comboProviderTo.SelectedIndex=selectedIndex;
			}
		}

		private void checkShowGroups_CheckedChanged(object sender,EventArgs e) {
			if(checkShowGroups.Checked) {
				labelGroup.BringToFront();
				labelGroup.Visible=true;
				labelGroupTo.Visible=true;
				comboGroup.Visible=true;
				comboGroupTo.Visible=true;
				comboClinic.Visible=false;
				comboClinicTo.Visible=false;
				butPickGroup.Visible=true;
				butPickGroupTo.Visible=true;
			}
			else {
				labelGroup.Visible=false;
				labelGroupTo.Visible=false;
				comboGroup.Visible=false;
				comboGroupTo.Visible=false;
				if(PrefC.HasClinicsEnabled) {
					comboClinic.Visible=true;
					comboClinicTo.Visible=true;
				}
				butPickGroup.Visible=false;
				butPickGroupTo.Visible=false;
			}
		}

		///<summary>If either of the FeeSched combos change, we fill the combos.</summary>
//todo:  js Why?
		private void comboFeeCombos_SelectionChangeCommitted(object sender,EventArgs e) {
			FillComboBoxes();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		/*private void FormFeeSchedTools_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK && _changed) {
				Cursor=Cursors.WaitCursor;
				_feeCache.SaveToDb();
				SecurityLogs.MakeLogEntries(Permissions.FeeSchedEdit,0,_listSecurityLogEntries);
				Cursor=Cursors.Default;
			}
		}*/
	}
}