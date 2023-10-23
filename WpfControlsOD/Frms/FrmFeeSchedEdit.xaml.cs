using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmFeeSchedEdit:FrmODBase {
		public FeeSched FeeSchedCur;
		public Clinic ClinicCur;

		///<summary></summary>
		public FrmFeeSchedEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			//Lan.F(this);
			Load+=FrmFeeSchedEdit_Load;
		}

		private void FrmFeeSchedEdit_Load(object sender,EventArgs e) {
			textDescription.Text=FeeSchedCur.Description;
			if(!FeeSchedCur.IsNew){
				listType.IsEnabled=false;
			}
			string[] stringArrayEnumNames = Enum.GetNames(typeof(FeeScheduleType));
			for(int i=0;i<stringArrayEnumNames.Length;i++) {
				FeeScheduleType feeSchedType=(FeeScheduleType)i;
				if(feeSchedType==FeeScheduleType.OutNetwork) {
					listType.Items.Add("Out of Network");
				}
				else {
					listType.Items.Add(stringArrayEnumNames[i]);
				}
				if(FeeSchedCur.FeeSchedType==feeSchedType) {
					listType.SetSelected(i);
				}
			}
			checkIsHidden.Checked=FeeSchedCur.IsHidden;
			if(Clinics.ClinicNum==0) {//HQ clinic, let them change if a fee sched can be localized or not.
				checkIsGlobal.IsEnabled=true;
			}
			if(FeeSchedCur.IsNew) {
				checkIsGlobal.Checked=true;
			}
			else {
				checkIsGlobal.Checked=FeeSchedCur.IsGlobal;
			}
		}

		private void checkIsGlobal_Click(object sender,EventArgs e) {
			if(checkIsGlobal.Checked==true) {//Checking IsGlobal (They want to delete their local fees for the feeschedule and use the HQ ones)
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Checking this option will use the global HQ fees and hide any clinic or provider specific fees.  Are you sure?")) {
					checkIsGlobal.Checked=false;
				}
			}
			else {//Unchecking IsGlobal (They want to create local fees for the feeschedule and override the HQ ones)
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Unchecking this option will allow:"+Environment.NewLine
					+"Fees to be different for other clinics or other providers for this same fee schedule. Are you sure?"))
				{
					checkIsGlobal.Checked=true;
				}
			}
		}

		private void checkIsHidden_Click(object sender,EventArgs e) {
			//Don't allow fees to be hidden if they are in use by a provider or in use by a patient.
			if(checkIsHidden.Checked==false) {
				return;//Unhiding a fee. OK.
			}
			if(FeeSchedCur.FeeSchedNum==0) {
				return;//Allow new fee schedules to be hidden.
			}
			List<InsPlan> listInsPlansForFeeSched = InsPlans.GetForFeeSchedNum(FeeSchedCur.FeeSchedNum);
			if(listInsPlansForFeeSched.Count > 0) {
				string insPlanMsg = Lans.g(this,"This fee schedule is tied to")+" "
					+listInsPlansForFeeSched.Count+" "+Lans.g(this,"insurance plans.")+" "+Lans.g(this,"Continue?");
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,insPlanMsg)) {
					checkIsHidden.Checked=false;
					return;
				}
			}
			string providersUsingFee="";
			List<Provider> listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<listProviders.Count;i++) {
				if(FeeSchedCur.FeeSchedNum==listProviders[i].FeeSched) {
					if(providersUsingFee!=""){//There is a name before this on the list
						providersUsingFee+=", ";
					}
					providersUsingFee+=listProviders[i].Abbr;
				}
			}
			if(providersUsingFee!="") {
				MessageBox.Show(Lans.g(this,"Cannot hide. Fee schedule is currently in use by the following providers")+":\r\n"+providersUsingFee);
				checkIsHidden.Checked=false;
			}
			string patsUsingFee="";
			//Don't allow fee schedules to be hidden if they are in use by a non-deleted patient.
			List<Patient> listPatients=Patients.GetForFeeSched(FeeSchedCur.FeeSchedNum).FindAll(x => x.PatStatus!=PatientStatus.Deleted);
			patsUsingFee=string.Join("\r\n",listPatients.Select(x => x.LName+", "+x.FName));
			if(patsUsingFee!="") {
				using MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(Lans.g(this,"Cannot hide. Fee schedule currently in use by the following non-deleted patients")
					+":\r\n"+patsUsingFee);
				msgBoxCopyPaste.ShowDialog();
				checkIsHidden.Checked=false;
			}
		}

		///<summary>Returns true if selected FeeScheduleType is invalid.</summary>
		private bool IsFeeSchedTypeInvalid() {
			FeeScheduleType feeScheduleTypeSelected=(FeeScheduleType)listType.SelectedIndex;
			AllowedFeeSchedsAutomate allowedFeeSchedsAutomatePref=(AllowedFeeSchedsAutomate)PrefC.GetInt(PrefName.AllowedFeeSchedsAutomate);
			if(FeeSchedCur.IsNew) {
				if(feeScheduleTypeSelected==FeeScheduleType.ManualBlueBook && allowedFeeSchedsAutomatePref!=AllowedFeeSchedsAutomate.BlueBook) {
					MsgBox.Show(this,"Manual Blue Book fee schedules can only be created when the Blue Book feature is on.");
					return true;
				}
			}
			else {//FeeSchedCur is not new
				//listType is disabled for existing fee schedules.
			}
			return false;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			//We do not allow global fee schedules to be associated to FeeSchedGroups.
			//Prevent a fee sched that is associated to a group to be turned into a global fee schedule.
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && checkIsGlobal.Checked==true) {
				if(FeeSchedCur.FeeSchedNum>0 && FeeSchedGroups.GetAllForFeeSched(FeeSchedCur.FeeSchedNum).Count()>0) {
					MsgBox.Show(this,"Not allowed to make Fee Schedule global, a Fee Schedule Group exists for this Fee Schedule.");
					return;
				}
			}
			if(IsFeeSchedTypeInvalid()) {
				return;
			}
			FeeSched feeSchedOld=FeeSchedCur.Copy();//make a copy of the old FeeSched so we can log any changes
			FeeSchedCur.Description=textDescription.Text;
			FeeSchedCur.FeeSchedType=(FeeScheduleType)listType.SelectedIndex;
			FeeSchedCur.IsHidden=(bool)checkIsHidden.Checked;
			FeeSchedCur.IsGlobal=(bool)checkIsGlobal.Checked;
			if(FeeSchedCur.IsNew) {
				FeeScheds.Insert(FeeSchedCur);
			}
			else {
				FeeScheds.Update(FeeSchedCur);
			}
			string log;
			if(FeeSchedCur.IsNew) {
				log="NEW Fee Schedule: \""+FeeSchedCur.Description
					+"\", Num:"+FeeSchedCur.FeeSchedNum
					+", Type:"+FeeSchedCur.FeeSchedType
					+", Global:"+FeeSchedCur.IsGlobal
					+", Hidden:"+FeeSchedCur.IsHidden;
			} else {
				log="EDIT Fee Schedule: \""+feeSchedOld.Description+"\"";//If this was changed in FeeSchedCur we'll log that next
				if(FeeSchedCur.Description!=feeSchedOld.Description) {
					log+=" to \""+FeeSchedCur.Description+"\"";
				}
				if(FeeSchedCur.FeeSchedType!=feeSchedOld.FeeSchedType) {
					log+=", from Type:"+feeSchedOld.FeeSchedType+" to Type:"+FeeSchedCur.FeeSchedType;
				}
				if(FeeSchedCur.IsGlobal!=feeSchedOld.IsGlobal) {
					log+=", Global:"+FeeSchedCur.IsGlobal;
				}
				if(FeeSchedCur.IsHidden!=feeSchedOld.IsHidden) {
					log+=", Hidden:"+FeeSchedCur.IsHidden;
				}
			}
			SecurityLogs.MakeLogEntry(EnumPermType.FeeSchedEdit,0,log,FeeSchedCur.FeeSchedNum,DateTime.Now);
			IsDialogOK=true;
		}
	}
}





















