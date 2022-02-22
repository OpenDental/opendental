using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>Not actually specific to eCW.  Using this for all HL7 interfaces where hl7def.ShowDemographics=Hide.</summary>
	public partial class ControlFamilyEcw:UserControl {
		private Patient PatCur;
		private Family FamCur;
		private List<FeeSched> _listFeeSchedShort;

		public ControlFamilyEcw() {
			InitializeComponent();
		}

		public void ModuleSelected(long patNum) {
			RefreshModuleData(patNum);
			RefreshModuleScreen();
		}

		private void RefreshModuleData(long patNum) {
			if(patNum==0) {
				PatCur=null;
				FamCur=null;
				return;
			}
			FamCur=Patients.GetFamily(patNum);
			PatCur=FamCur.GetPatient(patNum);
		}

		private void RefreshModuleScreen() {
			if(PatCur==null){
				return;
			}
			if(PIn.Bool(ProgramProperties.GetPropVal(ProgramName.eClinicalWorks,"FeeSchedulesSetManually"))) {
				comboFeeSched.Enabled=true;
			}
			else {
				comboFeeSched.Enabled=false;
			}
			comboFeeSched.Items.Clear();
			comboFeeSched.Items.Add(Lan.g(this,"none"));
			comboFeeSched.SelectedIndex=0;
			_listFeeSchedShort=FeeScheds.GetDeepCopy(true);
			for(int i=0;i<_listFeeSchedShort.Count;i++) {
				comboFeeSched.Items.Add(_listFeeSchedShort[i].Description);
				if(_listFeeSchedShort[i].FeeSchedNum==PatCur.FeeSched) {
					comboFeeSched.SelectedIndex=i+1;
				}
			}
		} 

		private void comboFeeSched_SelectionChangeCommitted(object sender,EventArgs e) {
			Patient oldPat=PatCur.Copy();
			if(comboFeeSched.SelectedIndex==0) {
				PatCur.FeeSched=0;
			}
			else {
				PatCur.FeeSched=_listFeeSchedShort[comboFeeSched.SelectedIndex-1].FeeSchedNum;
			}
			Patients.Update(PatCur,oldPat);
		}
	}
}
