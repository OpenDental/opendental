using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormEncounterEdit:FormODBase {
		private Encounter _encounter;
		private Patient _patient;
		private long _provNum;
		private List<Provider> _listProviders;

		public FormEncounterEdit(Encounter encounter) {
			InitializeComponent();
			InitializeLayoutManager();
			_encounter=encounter;
		}

		public void FormEncounters_Load(object sender,EventArgs e) {
			_patient=Patients.GetPat(_encounter.PatNum);
			this.Text+=" - "+_patient.GetNameLF();
			textDateEnc.Text=_encounter.DateEncounter.ToShortDateString();
			_provNum=_encounter.ProvNum;
			comboProv.Items.Clear();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());//Only visible provs added to combobox.
				if(_listProviders[i].ProvNum==_encounter.ProvNum) {
					comboProv.SelectedIndex=i;//Sets combo text too.
				}
			}
			if(_provNum==0) {//Is new
				comboProv.SelectedIndex=0;
				_provNum=_listProviders[0].ProvNum;
			}
			if(comboProv.SelectedIndex==-1) {//The provider exists but is hidden
				comboProv.Text=Providers.GetLongDesc(_provNum);//Appends "(hidden)" to the end of the long description.
			}
			textNote.Text=_encounter.Note;
			textCodeValue.Text=_encounter.CodeValue;
			textCodeSystem.Text=_encounter.CodeSystem;
			//to get description, first determine which table the code is from.  Encounter is only allowed to be a CDT, CPT, HCPCS, and SNOMEDCT.  Will be null if new encounter.
			switch(_encounter.CodeSystem) {
				case "CDT":
					textCodeDescript.Text=ProcedureCodes.GetProcCode(_encounter.CodeValue).Descript;
					break;
				case "CPT":
					Cpt cpt=Cpts.GetByCode(_encounter.CodeValue);
					if(cpt!=null) {
						textCodeDescript.Text=cpt.Description;
					}
					break;
				case "HCPCS":
					Hcpcs hcpcs=Hcpcses.GetByCode(_encounter.CodeValue);
					if(hcpcs!=null) {
						textCodeDescript.Text=hcpcs.DescriptionShort;
					}
					break;
				case "SNOMEDCT":
					Snomed snomed=Snomeds.GetByCode(_encounter.CodeValue);
					if(snomed!=null) {
						textCodeDescript.Text=snomed.Description;
					}
					break;
				case null:
					textCodeDescript.Text="";
					break;
				default:
					MsgBox.Show(this,"Error: Unknown code system");
					break;
			}
		}

		private void comboProv_SelectionChangeCommitted(object sender,EventArgs e) {
			_provNum=_listProviders[comboProv.SelectedIndex].ProvNum;
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick formProviderPick=new FormProviderPick();
			if(comboProv.SelectedIndex>-1) {
				formProviderPick.SelectedProvNum=_listProviders[comboProv.SelectedIndex].ProvNum;
			}
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SelectedIndex=Providers.GetIndex(formProviderPick.SelectedProvNum);
			_provNum=formProviderPick.SelectedProvNum;
		}

		private void butSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds formSnomeds=new FormSnomeds();
			formSnomeds.IsSelectionMode=true;
			if(formSnomeds.ShowDialog()==DialogResult.OK) {
				_encounter.CodeSystem="SNOMEDCT";
				_encounter.CodeValue=formSnomeds.SelectedSnomed.SnomedCode;
				textCodeSystem.Text="SNOMEDCT";
				textCodeValue.Text=formSnomeds.SelectedSnomed.SnomedCode;
				textCodeDescript.Text=formSnomeds.SelectedSnomed.Description;
			}
		}

		private void butHcpcs_Click(object sender,EventArgs e) {
			using FormHcpcs formHcpcs=new FormHcpcs();
			formHcpcs.IsSelectionMode=true;
			if(formHcpcs.ShowDialog()==DialogResult.OK) {
				_encounter.CodeSystem="HCPCS";
				_encounter.CodeValue=formHcpcs.HcpcsSelected.HcpcsCode;
				textCodeSystem.Text="HCPCS";
				textCodeValue.Text=formHcpcs.HcpcsSelected.HcpcsCode;
				textCodeDescript.Text=formHcpcs.HcpcsSelected.DescriptionShort;
			}
		}

		private void butCdt_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			if(formProcCodes.ShowDialog()==DialogResult.OK) {
				_encounter.CodeSystem="CDT";
				ProcedureCode procedureCode=ProcedureCodes.GetProcCode(formProcCodes.SelectedCodeNum);
				_encounter.CodeValue=procedureCode.ProcCode;
				textCodeSystem.Text="CDT";
				textCodeValue.Text=procedureCode.ProcCode;
				textCodeDescript.Text=procedureCode.Descript;
			}
		}

		private void butCpt_Click(object sender,EventArgs e) {
			using FormCpts formCpts=new FormCpts();
			formCpts.IsSelectionMode=true;
			if(formCpts.ShowDialog()==DialogResult.OK) {
				_encounter.CodeSystem="CPT";
				_encounter.CodeValue=formCpts.CptSelected.CptCode;
				textCodeSystem.Text="CPT";
				textCodeValue.Text=formCpts.CptSelected.CptCode;
				textCodeDescript.Text=formCpts.CptSelected.Description;
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(_encounter.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")!=true) {
				return;
			}
			Encounters.Delete(_encounter.EncounterNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//TODO: valid date box, validation here.
			if(!textDateEnc.IsValid()) {
				MsgBox.Show(this,"You must enter a valid date");
				return;
			}
			_encounter.ProvNum=_provNum;
			_encounter.Note=textNote.Text; //PIn.String(textNote.Text);
			_encounter.DateEncounter=PIn.Date(textDateEnc.Text);
			if(_encounter.CodeValue==null || _encounter.CodeSystem==null) {
				MsgBox.Show(this,"You must select a code");
				return;
			}
			if(_encounter.ProvNum==0) { //Should never be hit, defaults to index 1
				MsgBox.Show(this,"You must select a provider");
				return;
			}
			if(_encounter.IsNew){
				Encounters.Insert(_encounter);
			}
			else {
				Encounters.Update(_encounter);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


	}
}