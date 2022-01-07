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
		private List<Provider> _listProvs;
		private List<Clinic> _listClinics;

		///<summary>A list of security logs that should be inserted.</summary>
		private List<string> _listSecurityLogEntries=new List<string>();

		///<summary>Supply the fee schedule num(DefNum) to which all these changes will apply</summary>
		public FormFeeSchedTools(long schedNum,List<FeeSched> listFeeScheds,List<Provider> listProvs,List<Clinic> listClinics) {
			// Required for Windows Form Designer support
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_schedNum=schedNum;
			_listFeeScheds=listFeeScheds;
			_listProvs=listProvs;
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
			LayoutManager.MoveLocation(comboGroup,new Point(100,46));
			LayoutManager.MoveLocation(comboGroupTo,new Point(100,45));
			FillComboBoxes();
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				butImportCanada.Visible=false;
			}
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
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
			long feeSchedGroupNum=comboGroup.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
			long feeSchedGroupToNum=comboGroupTo.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
			comboFeeSched.Items.Clear();
			comboFeeSchedTo.Items.Clear();
			comboGroup.Items.Clear();
			comboGroupTo.Items.Clear();
			comboProvider.Items.Clear();
			comboProviderTo.Items.Clear();
			//Fee Schedules
			string str;
			for(int i=0;i<_listFeeScheds.Count;i++) {
				str=_listFeeScheds[i].Description;
				if(_listFeeScheds[i].FeeSchedType!=FeeScheduleType.Normal) {
					str+=" ("+_listFeeScheds[i].FeeSchedType.ToString()+")";
				}
				comboFeeSched.Items.Add(str);
				comboFeeSchedTo.Items.Add(str);
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
			for(int i=0;i<_listProvs.Count;i++) {
				comboProvider.Items.Add(_listProvs[i].Abbr);
				comboProviderTo.Items.Add(_listProvs[i].Abbr);
			}
			//comboClinic.SelectedClinicNum=comboClinic.SelectedClinicNum > -1 ? comboClinic.SelectedClinicNum:0;
			comboProvider.SelectedIndex=comboProvIdx > -1 ? comboProvIdx:0;
			comboProviderTo.SelectedIndex=comboProvToIdx > -1 ? comboProvToIdx:0;
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

		private void FillFeeSchedGroupComboBox(ComboBoxOD comboFeeSchedGroup,long feeSchedNumSelected,long feeSchedGroupNum) {
			List<long> listComboGroupNums=new List<long>();
			List<FeeSchedGroup> listGroups=FeeSchedGroups.GetAllForFeeSched(feeSchedNumSelected);
			foreach(FeeSchedGroup feeSchedGroupCur in listGroups) {
				if(!listComboGroupNums.Contains(feeSchedGroupCur.FeeSchedGroupNum) && feeSchedGroupCur.ListClinicNumsAll.Count>0) {//Skip duplicate/empty groups.
					comboFeeSchedGroup.Items.Add(feeSchedGroupCur.Description,feeSchedGroupCur);
					listComboGroupNums.Add(feeSchedGroupCur.FeeSchedGroupNum);
					//Set this fee sched group as the selection if it matches the previous selection before refreshing
					if(feeSchedGroupCur.FeeSchedGroupNum==feeSchedGroupNum) {
						comboFeeSchedGroup.SetSelectedKey<FeeSchedGroup>(feeSchedGroupCur.FeeSchedGroupNum, x => x.FeeSchedGroupNum);
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
				FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,comboClinic.SelectedClinicNum);//get the selected clinic num
				if(groupCur!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+groupCur.Description
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
				provNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
			}
			//ODProgress.ShowAction(() => {
			string logText="";
			foreach(long clinicNum in listClinicNums) {
				Fees.DeleteFees(feeSchedNum,clinicNum,provNum);
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
				toProvNum=_listProvs[comboProviderTo.SelectedIndex-1].ProvNum;
			}
			FeeSched toFeeSched=_listFeeScheds[comboFeeSchedTo.SelectedIndex];
			long fromClinicNum=0;
			if(PrefC.HasClinicsEnabled && !comboClinic.IsUnassignedSelected){
				fromClinicNum=comboClinic.SelectedClinicNum;//get the current clinic num if it is not unassigned
			}
			long fromProvNum=0;
			if(comboProvider.SelectedIndex!=0) {
				fromProvNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
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
			FeeSched fromFeeSched=_listFeeScheds[comboFeeSched.SelectedIndex];
			if(fromFeeSched.FeeSchedNum==toFeeSched.FeeSchedNum
				&& fromProvNum==toProvNum
				&& (!PrefC.HasClinicsEnabled || ListTools.In(fromClinicNum,listClinicNumsTo)))//If clinics disabled, can cause false negative so shortcircuit
			{
				MsgBox.Show(this,"Fee Schedules are not allowed to be copied into themselves. Please choose another fee schedule to copy.");
				return;
			}
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && !checkShowGroups.Checked) {
				//Pref is on but we are copying clinics.
				foreach(long clinicNumTo in listClinicNumsTo) {
					FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(toFeeSched.FeeSchedNum,clinicNumTo);
					if(groupCur!=null) {
						Clinic clinicCur=Clinics.GetClinic(clinicNumTo);
						MsgBox.Show(this,Lans.g(this,"Clinic: ")+clinicCur.Abbr+Lans.g(this," is a member of Fee Schedule Group: ")+groupCur.Description
							+Lans.g(this," for the selected Fee Schedule and must be copied at the Fee Schedule Group level."));
						return;
					}
				}
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"All fees that exactly match the \"Copy To\" fee schedule/clinic/provider combination will be deleted.  Then new fees will be copied in.  Are you sure you want to continue?")){
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => FeeScheds.CopyFeeSchedule(fromFeeSched,fromClinicNum,fromProvNum,toFeeSched,listClinicNumsTo,toProvNum);
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
			long feeSchedGroupToNum=comboGroupTo.GetSelected<FeeSchedGroup>()?.FeeSchedGroupNum??0;
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
				FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,clinicNum);
				if(groupCur!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+groupCur.Description
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
				provNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
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
						FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSchedNum,clinicNum);
						msgText="There are "+listFees.Count+" override fees for group '"+groupCur.Description+"' and there are"
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
				provNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
			}
			string fileName="Fees"+feeSchedDesc+".txt";
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
			}
			else {
				Cursor=Cursors.WaitCursor;
				using SaveFileDialog Dlg=new SaveFileDialog();
				if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					Dlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				else if(Directory.Exists("C:\\")) {
					Dlg.InitialDirectory="C:\\";
				}
				Dlg.FileName=fileName;
				if(Dlg.ShowDialog()!=DialogResult.OK) {
					Cursor=Cursors.Default;
					return;
				}
				filePath=Dlg.FileName;
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
			using OpenFileDialog Dlg=new OpenFileDialog();
			if(Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
				Dlg.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
			}
			else if(Directory.Exists("C:\\")) {
				Dlg.InitialDirectory="C:\\";
			}
			if(Dlg.ShowDialog()!=DialogResult.OK) {
				return;
			}
			if(!File.Exists(Dlg.FileName)){
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
				FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(groupCur!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+groupCur.Description
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
				provNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => FeeL.ImportFees(Dlg.FileName,feeSched.FeeSchedNum,clinicNum,provNum);
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
			using FormFeeSchedPickRemote formPick=new FormFeeSchedPickRemote();
			formPick.Url=@"http://www.opendental.com/feescanada/";//points to index.php file
			if(formPick.ShowDialog()!=DialogResult.OK) {
				Cursor=Cursors.Default;
				return;
			}
			Cursor=Cursors.WaitCursor;//original wait cursor seems to go away for some reason.
			Application.DoEvents();
			string feeData="";
			Action actionCloseFeeSchedImportCanadaProgress=ODProgress.Show(ODEventType.FeeSched,typeof(FeeSchedEvent));
			if(formPick.IsFileChosenProtected) {
				actionCloseFeeSchedImportCanadaProgress?.Invoke();//Hide the progress window so it does not cover up the authorization form.
				string memberNumberODA="";
				string memberPasswordODA="";
				if(formPick.FileChosenName.StartsWith("ON_")) {//Any and all Ontario fee schedules
					using FormFeeSchedPickAuthOntario formAuth=new FormFeeSchedPickAuthOntario();
					if(formAuth.ShowDialog()!=DialogResult.OK) {
						Cursor=Cursors.Default;
						return;
					}
					actionCloseFeeSchedImportCanadaProgress=ODProgress.Show(ODEventType.FeeSched,typeof(FeeSchedEvent));
					memberNumberODA=formAuth.ODAMemberNumber;
					memberPasswordODA=formAuth.ODAMemberPassword;
				}
				//prepare the xml document to send--------------------------------------------------------------------------------------
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Indent = true;
				settings.IndentChars = ("    ");
				StringBuilder strbuild=new StringBuilder();
				using(XmlWriter writer=XmlWriter.Create(strbuild,settings)) {
					writer.WriteStartElement("RequestFeeSched");
					writer.WriteStartElement("RegistrationKey");
					writer.WriteString(PrefC.GetString(PrefName.RegistrationKey));
					writer.WriteEndElement();//RegistrationKey
					writer.WriteStartElement("FeeSchedFileName");
					writer.WriteString(formPick.FileChosenName);
					writer.WriteEndElement();//FeeSchedFileName
					if(memberNumberODA!="") {
						writer.WriteStartElement("ODAMemberNumber");
						writer.WriteString(memberNumberODA);
						writer.WriteEndElement();//ODAMemberNumber
						writer.WriteStartElement("ODAMemberPassword");
						writer.WriteString(memberPasswordODA);
						writer.WriteEndElement();//ODAMemberPassword
					}
					writer.WriteEndElement();//RequestFeeSched
				}
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
					result=updateService.RequestFeeSched(strbuild.ToString());
				}
				catch(Exception ex) {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					Cursor=Cursors.Default;
					MessageBox.Show("Error: "+ex.Message);
					return;
				}
				Cursor=Cursors.Default;
				XmlDocument doc=new XmlDocument();
				doc.LoadXml(result);
				//Process errors------------------------------------------------------------------------------------------------------------
				XmlNode node=doc.SelectSingleNode("//Error");
				if(node!=null) {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					MessageBox.Show(node.InnerText,"Error");
					return;
				}
				node=doc.SelectSingleNode("//KeyDisabled");
				if(node==null) {
					//no error, and no disabled message
					if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
						DataValid.SetInvalid(InvalidType.Prefs);
					}
				}
				else {
					actionCloseFeeSchedImportCanadaProgress?.Invoke();
					MessageBox.Show(node.InnerText);
					if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
						DataValid.SetInvalid(InvalidType.Prefs);
					}
					return;
				}
				//Process a valid return value------------------------------------------------------------------------------------------------
				node=doc.SelectSingleNode("//ResultCSV64");
				string feeData64=node.InnerXml;
				byte[] feeDataBytes=Convert.FromBase64String(feeData64);
				feeData=Encoding.UTF8.GetString(feeDataBytes);
			}
			else {
				FeeSchedEvent.Fire(ODEventType.FeeSched,Lan.g(this,"Downloading fee schedule")+"...");
				string tempFile=PrefC.GetRandomTempFile(".tmp");
				WebClient myWebClient=new WebClient();
				try {
					myWebClient.DownloadFile(formPick.FileChosenUrl,tempFile);
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
				FeeSchedGroup groupCur=FeeSchedGroups.GetOneForFeeSchedAndClinic(feeSched.FeeSchedNum,clinicNum);
				if(groupCur!=null) {
					MsgBox.Show(Lans.g(this,"Selected clinic is a member of Fee Schedule Group: ")+groupCur.Description
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
				provNum=_listProvs[comboProvider.SelectedIndex-1].ProvNum;
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
			ODProgressExtended progExtended=new ODProgressExtended(ODEventType.FeeSched,new FeeSchedEvent(),this,
				tag:new ProgressBarHelper("Fee Schedule Update Progress",progressBarEventType:ProgBarEventType.Header),cancelButtonText:Lan.g(this,"Close"));
			Cursor=Cursors.WaitCursor;
			List<Fee> listFeesHQ=Fees.GetByClinicNum(0);//All HQ fees
			try {
				if(PrefC.HasClinicsEnabled) {
					List<long> listFeeClinics=comboGlobalUpdateClinics.ListSelectedClinicNums;
					for(int i=0;i<listFeeClinics.Count;i++) {
						//Clinic clinicCur=listFeeClinics[i];
						while(progExtended.IsPaused) {
							Thread.Sleep(10);
							if(progExtended.IsCanceled) {
								break;
							}
						}
						if(progExtended.IsCanceled) {
							break;
						}
						double percentComplete=(((double)i)/listFeeClinics.Count*100);
						if(listFeeClinics.Count>1) {
							progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper("Overall",(int)percentComplete+"%",i,
								listFeeClinics.Count,tagString:"OverallStatus"));
							progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper(Clinics.GetAbbr(listFeeClinics[i]),"0%",1,100,tagString:"Clinic"));
						}
						else {
							progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper(Clinics.GetAbbr(listFeeClinics[i]),"0%",1,100,tagString:"Clinic"));
							progExtended.HideButtons();//can't pause or cancel with 1 clinic. This event needs to be called after the bar is instantiated. 
						}
						rowsChanged+=Procedures.GlobalUpdateFees(listFeesHQ,listFeeClinics[i],Clinics.GetAbbr(listFeeClinics[i]));
						if(progExtended.IsPaused) {
							progExtended.AllowResume();
						}
					}
					if(listFeeClinics.Count>1) {
						progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper("Overall","100%",100,100,tagString:"OverallStatus"));
					}
				}
				else {//no clinic - "Clinic" here is just a reference to the progress bar that updates Clinic progress instead of overall progress
					progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper("Updating...","0%",1,100,tagString:"Clinic"));
					progExtended.HideButtons();
					rowsChanged=Procedures.GlobalUpdateFees(listFeesHQ,0,"Updating...");
				}
				progExtended.OnProgressDone();
				progExtended.Fire(ODEventType.FeeSched,new ProgressBarHelper("Treatment planned procedure fees changed: "+rowsChanged.ToString()+"\r\nDone.",
					progressBarEventType:ProgBarEventType.TextMsg));
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				progExtended.Close();
				MessageBox.Show(ex.Message);
				return;	
			}
			finally {
				if(progExtended.IsCanceled) {//close
					progExtended.Close();
					DialogResult=DialogResult.OK;
				}
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
				DialogResult diagRes=MessageBox.Show(this,msgText,"",MessageBoxButtons.YesNoCancel);
				if(diagRes==DialogResult.Cancel) {
					return;
				}
				else if(diagRes==DialogResult.Yes) {//pick up where last run left off and overwrite that last clinic with the progress from this run
					listWriteoffClinics.RemoveRange(0,indexPrevClinic+1);
				}
				else {
					//diagRes==DialogResult.No, run tool for all clinics and replace the previous run progress with the progress from this run
				}
			}
			ODProgressExtended progress=new ODProgressExtended(ODEventType.FeeSched,new FeeSchedEvent(),this,
				tag:new ProgressBarHelper(Lan.g(this,"Write-off Update Progress"),progressBarEventType:ProgBarEventType.Header),
				cancelButtonText:Lan.g(this,"Close"));
			progress.Fire(ODEventType.FeeSched,new ProgressBarHelper("","0%",0,100,ProgBarStyle.Blocks,"WriteoffProgress"));
			Cursor=Cursors.WaitCursor;
			try {
				FeeScheds.GlobalUpdateWriteoffs(listWriteoffClinics,progress,doUpdatePrevClinicPref);
			}
			catch(ApplicationException ex) {
				Cursor=Cursors.Default;
				progress.Close();
				MessageBox.Show(ex.Message);
			}
			finally {
				if(progress.IsCanceled) {
					progress.Close();
				}
				Cursor=Cursors.Default;
			}
		}

		private void butPickFeeSched_Click(object sender,EventArgs e) {
			//int selectedIndex=GetFeeSchedIndexFromPicker();
			//No need to check security because we are launching the form in selection mode.
			using FormFeeScheds FormFS=new FormFeeScheds(true);
			FormFS.ShowDialog();
			int selectedIndex= _listFeeScheds.FindIndex(x => x.FeeSchedNum==FormFS.SelectedFeeSchedNum);//Returns index of the found element or -1.
			//If the selectedIndex is -1, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==-1) {
				return;
			}
			UI.Button pickerButton=(UI.Button)sender;
			if(pickerButton==butPickSched) { //First FeeSched combobox doesn't have "None" option.
				comboFeeSched.SelectedIndex=selectedIndex;
			}
			else if(pickerButton==butPickSchedTo) {
				comboFeeSchedTo.SelectedIndex=selectedIndex;
			}
			FillComboBoxes();
		}

		private void butPickGroup_Click(object sender,EventArgs e){
			if(checkShowGroups.Checked) {
				List<FeeSchedGroup> listGroupsToShow=comboGroup.Items.GetAll<FeeSchedGroup>();//s.AsEnumerable<ODBoxItem<FeeSchedGroup>>()).Select(x => x.Tag).ToList();
				List<GridColumn> listColumnHeaders=new List<GridColumn>() {
					new GridColumn(Lan.g(this,"Description"),50){ IsWidthDynamic=true }
				};
				List<GridRow> listRowValues=new List<GridRow>();
				listGroupsToShow.ForEach(x => {
					GridRow row=new GridRow(x.Description);
					row.Tag=x;
					listRowValues.Add(row);
				});
				string formTitle=Lan.g(this,"Fee Schedule Group Picker");
				string gridTitle=Lan.g(this,"Fee Schedule Groups");
				using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
				if(form.ShowDialog()==DialogResult.OK) {
					comboGroup.SelectedIndex=listGroupsToShow.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)form.ListSelectedTags[0]).FeeSchedGroupNum));
					return;
				}
				//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
				return;
			}
		}

		private void butPickGroupTo_Click(object sender,EventArgs e) {
			if(checkShowGroups.Checked) {
				List<FeeSchedGroup> listGroupsToShow=comboGroupTo.Items.GetAll<FeeSchedGroup>();//Items.AsEnumerable<ODBoxItem<FeeSchedGroup>>()).Select(x => x.Tag).ToList();
				List<GridColumn> listColumnHeaders=new List<GridColumn>() {
					new GridColumn(Lan.g(this,"Description"),50){ IsWidthDynamic=true }
				};
				List<GridRow> listRowValues=new List<GridRow>();
				listGroupsToShow.ForEach(x => {
					GridRow row=new GridRow(x.Description);
					row.Tag=x;
					listRowValues.Add(row);
				});
				string formTitle=Lan.g(this,"Fee Schedule Group Picker");
				string gridTitle=Lan.g(this,"Fee Schedule Groups");
				using FormGridSelection form=new FormGridSelection(listColumnHeaders,listRowValues,formTitle,gridTitle);
				if(form.ShowDialog()==DialogResult.OK) {
					comboGroupTo.SelectedIndex=listGroupsToShow.FindIndex((x => x.FeeSchedGroupNum==((FeeSchedGroup)form.ListSelectedTags[0]).FeeSchedGroupNum));
					return;
				}
				//Nothing was selected. This matches what happens with GetClinicIndexFromPicker.
				return;
			}
		}

		private void butPickProvider_Click(object sender,EventArgs e){
			//int selectedIndex=-1;//GetProviderIndexFromPicker()+1;//All provider combo boxes have a none option, so always add 1.
			using FormProviderPick FormP=new FormProviderPick();
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;// -1;
			}
			int selectedIndex= Providers.GetIndex(FormP.SelectedProvNum)+1;//All provider combo boxes have a none option, so always add 1.
			//If the selectedIndex is 0, simply return and do not do anything.  There is no such thing as picking 'None' from the picker window.
			if(selectedIndex==0) {
				return;
			}
			UI.Button pickerButton=(UI.Button)sender;
			if(pickerButton==butPickProv) {
				comboProvider.SelectedIndex=selectedIndex;
			}
			else if(pickerButton==butPickProvTo) {
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