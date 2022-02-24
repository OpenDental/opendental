using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEncounterTool:FormODBase {
		private List<string> _listRecEncCodes;
		public int EncListSelectedIdx=-1;
		public string EncCodeValue;
		public string CodeDescription;
		public string EncCodeSystem;

		public FormEncounterTool() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEncounterTool_Load(object sender,EventArgs e) {
			FillRecEncCodesList();
			int countNotInSnomedTable=0;
			for(int i=0;i<_listRecEncCodes.Count;i++) {
				if(!Snomeds.CodeExists(_listRecEncCodes[i])) {
					countNotInSnomedTable++;
					continue;
				}
				comboEncCodes.Items.Add(_listRecEncCodes[i]);
			}
			if(countNotInSnomedTable>0) {
				MsgBox.Show(this,"The snomed table does not contain one or more codes from the list of recommended encounter codes.  The snomed table should "
					+"be updated by running the Code System Importer tool found in Setup | Chart | EHR.");
			}
			comboEncCodes.SelectedIndex=EncListSelectedIdx;
			textEncCodeValue.Text=EncCodeValue;
			textEncCodeDescript.Text=CodeDescription;
			labelEncWarning.Visible=false;
		}

		private void FillRecEncCodesList() {
			//All of the recommended codes are SNOMEDCT codes
			_listRecEncCodes=new List<string>();
			_listRecEncCodes.Add("90526000");//Initial evaluation and management of healthy individual (procedure)
			_listRecEncCodes.Add("185349003");//Encounter for "check-up" (procedure)
			_listRecEncCodes.Add("185463005");//Visit out of hours (procedure)
			_listRecEncCodes.Add("185465003");//Weekend visit (procedure)
			_listRecEncCodes.Add("270427003");//Patient-initiated encounter (procedure)
			_listRecEncCodes.Add("270430005");//Provider-initiated encounter (procedure)
			_listRecEncCodes.Add("308335008");//Patient encounter procedure (procedure)
			_listRecEncCodes.Add("390906007");//Follow-up encounter (procedure)
			_listRecEncCodes.Add("406547006");//Urgent follow-up (procedure)
		}

		private void comboEncCodes_SelectionChangeCommitted(object sender,EventArgs e) {
			EncCodeSystem="SNOMEDCT";
			textEncCodeValue.Text="";
			Snomed sEnc=Snomeds.GetByCode(comboEncCodes.SelectedItem.ToString());
			textEncCodeDescript.Text=sEnc.Description;
			labelEncWarning.Visible=false;
		}

		private void butEncSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			FormS.IsSelectionMode=true;
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				EncCodeSystem="SNOMEDCT";
				for(int i=1;i<comboEncCodes.Items.Count;i++) {
					if(FormS.SelectedSnomed.SnomedCode==comboEncCodes.Items[i].ToString()) {//if they selected one of the recommended codes, select in list
						comboEncCodes.SelectedIndex=i;
						textEncCodeValue.Clear();
						textEncCodeDescript.Text=FormS.SelectedSnomed.Description;
						labelEncWarning.Visible=false;
						return;
					}
				}
				comboEncCodes.SelectedIndex=-1;
				textEncCodeValue.Text=FormS.SelectedSnomed.SnomedCode;
				textEncCodeDescript.Text=FormS.SelectedSnomed.Description;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncHcpcs_Click(object sender,EventArgs e) {
			using FormHcpcs FormH=new FormHcpcs();
			FormH.IsSelectionMode=true;
			FormH.ShowDialog();
			if(FormH.DialogResult==DialogResult.OK) {
				EncCodeSystem="HCPCS";
				comboEncCodes.SelectedIndex=-1;
				textEncCodeValue.Text=FormH.SelectedHcpcs.HcpcsCode;
				textEncCodeDescript.Text=FormH.SelectedHcpcs.DescriptionShort;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncCdt_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.OK) {
				EncCodeSystem="CDT";
				comboEncCodes.SelectedIndex=-1;
				ProcedureCode procCur=ProcedureCodes.GetProcCode(FormP.SelectedCodeNum);
				textEncCodeValue.Text=procCur.ProcCode;
				textEncCodeDescript.Text=procCur.Descript;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncCpt_Click(object sender,EventArgs e) {
			using FormCpts FormC=new FormCpts();
			FormC.IsSelectionMode=true;
			FormC.ShowDialog();
			if(FormC.DialogResult==DialogResult.OK) {
				EncCodeSystem="CPT";
				comboEncCodes.SelectedIndex=-1;
				textEncCodeValue.Text=FormC.SelectedCpt.CptCode;
				textEncCodeDescript.Text=FormC.SelectedCpt.Description;
				labelEncWarning.Visible=true;
			}
		}

		private void butRun_Click(object sender,EventArgs e) {
			if(!textDateStart.IsValid()
				|| !textDateEnd.IsValid()
				|| textDateStart.Text==""
				|| textDateEnd.Text=="") 
			{
				MsgBox.Show(this,"Please enter a valid date range.");
				return;
			}
			if(comboEncCodes.SelectedIndex==-1
				&& textEncCodeValue.Text=="") {
				MsgBox.Show(this,"Please select a code.");
				return;
			}
			string codeValue;
			if(comboEncCodes.SelectedIndex==-1) {
				codeValue=textEncCodeValue.Text;
			}
			else {
				codeValue=comboEncCodes.SelectedItem.ToString();
			}
			long encInserted=Encounters.InsertEncsFromProcDates(PIn.Date(textDateStart.Text),PIn.Date(textDateEnd.Text),codeValue,EncCodeSystem);
			MessageBox.Show(Lan.g("FormEncounterTool","Number of encounters inserted:")+" "+encInserted.ToString());
			if(PrefC.GetString(PrefName.CQMDefaultEncounterCodeValue)=="none") {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Do you want to set this code as the default encounter code?")) {
					Prefs.UpdateString(PrefName.CQMDefaultEncounterCodeValue,codeValue);
					Prefs.UpdateString(PrefName.CQMDefaultEncounterCodeSystem,EncCodeSystem);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}