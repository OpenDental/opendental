using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormFeeSchedEdit:FormODBase {
		public FeeSched FeeSchedCur;
		public Clinic ClinicCur;

		///<summary></summary>
		public FormFeeSchedEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormFeeSchedEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=FeeSchedCur.Description;
			if(!FeeSchedCur.IsNew){
				listType.Enabled=false;
			}
			Array arrayValues=Enum.GetValues(typeof(FeeScheduleType));
			for(int i=0;i<arrayValues.Length;i++) {
				FeeScheduleType feeSchedType=((FeeScheduleType)arrayValues.GetValue(i));
				if(feeSchedType==FeeScheduleType.OutNetwork) {
					listType.Items.Add("Out of Network");
				}
				else {
					listType.Items.Add(arrayValues.GetValue(i).ToString());
				}
				if(FeeSchedCur.FeeSchedType==feeSchedType) {
					listType.SetSelected(i);
				}
			}
			checkIsHidden.Checked=FeeSchedCur.IsHidden;
			if(Clinics.ClinicNum==0) {//HQ clinic, let them change if a fee sched can be localized or not.
				checkIsGlobal.Enabled=true;
			}
			if(FeeSchedCur.IsNew) {
				checkIsGlobal.Checked=true;
			}
			else {
				checkIsGlobal.Checked=FeeSchedCur.IsGlobal;
			}
		}

		private void checkIsGlobal_Click(object sender,EventArgs e) {
			if(checkIsGlobal.Checked) {//Checking IsGlobal (They want to delete their local fees for the feeschedule and use the HQ ones)
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
			if(!checkIsHidden.Checked) {
				return;//Unhiding a fee. OK.
			}
			if(FeeSchedCur.FeeSchedNum==0) {
				return;//Allow new fee schedules to be hidden.
			}
			List<InsPlan> listInsPlanForFeeSched = InsPlans.GetForFeeSchedNum(FeeSchedCur.FeeSchedNum);
			if(listInsPlanForFeeSched.Count > 0) {
				string insPlanMsg = Lan.g(this,"This fee schedule is tied to")+" "
					+listInsPlanForFeeSched.Count+" "+Lan.g(this,"insurance plans.")+" "+Lan.g(this,"Continue?");
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
				MessageBox.Show(Lan.g(this,"Cannot hide. Fee schedule is currently in use by the following providers")+":\r\n"+providersUsingFee);
				checkIsHidden.Checked=false;
			}
			string patsUsingFee="";
			//Don't allow fee schedules to be hidden if they are in use by a non-deleted patient.
			List<Patient> listPats=Patients.GetForFeeSched(FeeSchedCur.FeeSchedNum).FindAll(x => x.PatStatus!=PatientStatus.Deleted);
			patsUsingFee=string.Join("\r\n",listPats.Select(x => x.LName+", "+x.FName));
			if(patsUsingFee!="") {
				using MsgBoxCopyPaste msgBoxCP=new MsgBoxCopyPaste(Lan.g(this,"Cannot hide. Fee schedule currently in use by the following non-deleted patients")
					+":\r\n"+patsUsingFee);
				msgBoxCP.ShowDialog();
				checkIsHidden.Checked=false;
			}
		}

		///<summary>Returns true if selected FeeScheduleType is invalid.</summary>
		private bool IsFeeSchedTypeInvalid() {
			FeeScheduleType typeSelected=(FeeScheduleType)listType.SelectedIndex;
			AllowedFeeSchedsAutomate pref=(AllowedFeeSchedsAutomate)PrefC.GetInt(PrefName.AllowedFeeSchedsAutomate);
			if(FeeSchedCur.IsNew) {
				if(typeSelected==FeeScheduleType.OutNetwork && pref==AllowedFeeSchedsAutomate.BlueBook) {
					MsgBox.Show(this,"Out Of Network fee schedules cannot be created when the Blue Book feature is on.");
					return true;
				}
				if(typeSelected==FeeScheduleType.ManualBlueBook && pref!=AllowedFeeSchedsAutomate.BlueBook) {
					MsgBox.Show(this,"Manual Blue Book fee schedules can only be created when the Blue Book feature is on.");
					return true;
				}
			}
			else {//FeeSchedCur is not new
				//listType is disabled for existing fee schedules.
			}
			return false;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Description cannot be blank.");
				return;
			}
			//We do not allow global fee schedules to be associated to FeeSchedGroups.
			//Prevent a fee sched that is associated to a group to be turned into a global fee schedule.
			if(PrefC.GetBool(PrefName.ShowFeeSchedGroups) && checkIsGlobal.Checked) {
				if(FeeSchedCur.FeeSchedNum>0 && FeeSchedGroups.GetAllForFeeSched(FeeSchedCur.FeeSchedNum).Count()>0) {
					MsgBox.Show(this,"Not allowed to make Fee Schedule global, a Fee Schedule Group exists for this Fee Schedule.");
					return;
				}
			}
			if(IsFeeSchedTypeInvalid()) {
				return;
			}
			FeeSchedCur.Description=textDescription.Text;
			FeeSchedCur.FeeSchedType=(FeeScheduleType)listType.SelectedIndex;
			FeeSchedCur.IsHidden=checkIsHidden.Checked;
			bool isGlobalOld=FeeSchedCur.IsGlobal;
			FeeSchedCur.IsGlobal=checkIsGlobal.Checked;
			if(FeeSchedCur.IsNew) {
				FeeScheds.Insert(FeeSchedCur);
			}
			else {
				FeeScheds.Update(FeeSchedCur);
			}
			if(isGlobalOld!=FeeSchedCur.IsGlobal) {
				string log;
				if(FeeSchedCur.IsNew) {
					log="Created Fee Schedule \""+textDescription.Text+"\": \"Use Global Fees\" is ";
					if(FeeSchedCur.IsGlobal) {
						log+="Checked";
					}
					else {
						log+="Unchecked";
					}
				}
				else {
					log="Edited Fee Schedule \""+textDescription.Text+"\": Changed \"Use Global Fees\" from ";
					if(isGlobalOld) {
						log+="Checked ";
					}
					else {
						log+="Unchecked ";
					}
					if(FeeSchedCur.IsGlobal) {
						log+="to Checked";
					}
					else {
						log+="to Unchecked";
					}
				}
				SecurityLogs.MakeLogEntry(Permissions.FeeSchedEdit,0,log);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}










	}
}





















