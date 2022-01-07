using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEhrSettings:FormODBase {
		private List<string> ListRecEncCodes;
		private List<string> ListRecPregCodes;
		private int OldEncListSelectedIdx;
		private int OldPregListSelectedIdx;
		private string NewEncCodeSystem;
		private string NewPregCodeSystem;

		public FormEhrSettings() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEhrSettings_Load(object sender,EventArgs e) {
			if(PrefC.GetString(PrefName.SoftwareName)!="") {
				this.Text+=" - "+PrefC.GetString(PrefName.SoftwareName);
			}
			checkAlertHighSeverity.Checked=PrefC.GetBool(PrefName.EhrRxAlertHighSeverity);
			comboMU2.Items.Add("Stage 1");
			comboMU2.Items.Add("Stage 2");
			comboMU2.Items.Add("Modified Stage 2");
			comboMU2.SelectedIndex=PrefC.GetInt(PrefName.MeaningfulUseTwo);
			checkAutoWebmails.Checked=PrefC.GetBool(PrefName.AutomaticSummaryOfCareWebmail);
			FillRecEncCodesList();
			FillDefaultEncCode();
			#region DefaultPregnancyGroup
			FillRecPregCodesList();
			string defaultPregCode=PrefC.GetString(PrefName.PregnancyDefaultCodeValue);
			string defaultPregCodeSystem=PrefC.GetString(PrefName.PregnancyDefaultCodeSystem);
			NewPregCodeSystem=defaultPregCodeSystem;
			OldPregListSelectedIdx=-1;
			int countNotInSnomedTable=0;
			for(int i=0;i<ListRecPregCodes.Count;i++) {
				if(i==0) {
					comboPregCodes.Items.Add(ListRecPregCodes[i]);
					comboPregCodes.SelectedIndex=i;
					if(defaultPregCode==ListRecPregCodes[i]) {
						comboPregCodes.SelectedIndex=i;
						OldPregListSelectedIdx=i;
					}
					continue;
				}
				if(!Snomeds.CodeExists(ListRecPregCodes[i])) {
					countNotInSnomedTable++;
					continue;
				}
				comboPregCodes.Items.Add(ListRecPregCodes[i]);
				if(ListRecPregCodes[i]==defaultPregCode && defaultPregCodeSystem=="SNOMEDCT") {
					comboPregCodes.SelectedIndex=i;
					OldPregListSelectedIdx=i;
					labelPregWarning.Visible=false;
					textPregCodeDescript.Text=Snomeds.GetByCode(ListRecPregCodes[i]).Description;//Guaranteed to exist in snomed table from above check
				}
			}
			if(countNotInSnomedTable>0) {
				MsgBox.Show(this,"The snomed table does not contain one or more codes from the list of recommended pregnancy codes.  The snomed table should be updated by running the Code System Importer tool found in Setup | Chart | EHR.");
			}
			if(comboPregCodes.SelectedIndex==-1) {//default preg code set to code not in recommended list and not 'none'
				switch(defaultPregCodeSystem) {
					case "ICD9CM":
						ICD9 i9Preg=ICD9s.GetByCode(defaultPregCode);
						if(i9Preg!=null) {
							textPregCodeValue.Text=i9Preg.ICD9Code;
							textPregCodeDescript.Text=i9Preg.Description;
						}
						break;
					case "SNOMEDCT":
						Snomed sPreg=Snomeds.GetByCode(defaultPregCode);
						if(sPreg!=null) {
							textPregCodeValue.Text=sPreg.SnomedCode;
							textPregCodeDescript.Text=sPreg.Description;
						}
						break;
					case "ICD10CM":
						Icd10 i10Preg=Icd10s.GetByCode(defaultPregCode);
						if(i10Preg!=null) {
							textPregCodeValue.Text=i10Preg.Icd10Code;
							textPregCodeDescript.Text=i10Preg.Description;
						}
						break;
					default:
						break;
				}
			}
			#endregion
		}

		private void checkAlertHighSeverity_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				checkAlertHighSeverity.Checked=PrefC.GetBool(PrefName.EhrRxAlertHighSeverity);
			}
		}

		private void FillRecEncCodesList() {
			//All of the recommended codes are SNOMEDCT codes
			ListRecEncCodes=new List<string>();
			ListRecEncCodes.Add("none");
			ListRecEncCodes.Add("90526000");//Initial evaluation and management of healthy individual (procedure)
			ListRecEncCodes.Add("185349003");//Encounter for "check-up" (procedure)
			ListRecEncCodes.Add("185463005");//Visit out of hours (procedure)
			ListRecEncCodes.Add("185465003");//Weekend visit (procedure)
			ListRecEncCodes.Add("270427003");//Patient-initiated encounter (procedure)
			ListRecEncCodes.Add("270430005");//Provider-initiated encounter (procedure)
			ListRecEncCodes.Add("308335008");//Patient encounter procedure (procedure)
			ListRecEncCodes.Add("390906007");//Follow-up encounter (procedure)
			ListRecEncCodes.Add("406547006");//Urgent follow-up (procedure)
		}

		private void FillRecPregCodesList() {
			//All of the recommended codes are SNOMEDCT codes
			ListRecPregCodes=new List<string>();
			ListRecPregCodes.Add("none");
			ListRecPregCodes.Add("72892002");//Normal pregnancy (finding)
			ListRecPregCodes.Add("77386006");//Patient currently pregnant (finding)
			ListRecPregCodes.Add("83074005");//Unplanned pregnancy (finding)
			ListRecPregCodes.Add("169560008");//Pregnant - urine test confirms (finding)
			ListRecPregCodes.Add("169563005");//Pregnant - on history (finding)
			ListRecPregCodes.Add("169565003");//Pregnant - planned (finding)
			ListRecPregCodes.Add("237238006");//Pregnancy with uncertain dates (finding)
			ListRecPregCodes.Add("248985009");//Presentation of pregnancy (finding)
			ListRecPregCodes.Add("314204000");//Early stage of pregnancy (finding)
		}

		private void FillDefaultEncCode() {
			string defaultEncCode=PrefC.GetString(PrefName.CQMDefaultEncounterCodeValue);
			string defaultEncCodeSystem=PrefC.GetString(PrefName.CQMDefaultEncounterCodeSystem);
			NewEncCodeSystem=defaultEncCodeSystem;
			OldEncListSelectedIdx=-1;
			int countNotInSnomedTable=0;
			for(int i=0;i<ListRecEncCodes.Count;i++) {
				if(i==0) {
					comboEncCodes.Items.Add(ListRecEncCodes[i]);
					comboEncCodes.SelectedIndex=i;
					if(defaultEncCode==ListRecEncCodes[i]) {
						comboEncCodes.SelectedIndex=i;
						OldEncListSelectedIdx=i;
					}
					continue;
				}
				if(!Snomeds.CodeExists(ListRecEncCodes[i])) {
					countNotInSnomedTable++;
					continue;
				}
				comboEncCodes.Items.Add(ListRecEncCodes[i]);
				if(ListRecEncCodes[i]==defaultEncCode && defaultEncCodeSystem=="SNOMEDCT") {
					comboEncCodes.SelectedIndex=i;
					OldEncListSelectedIdx=i;
					labelEncWarning.Visible=false;
					textEncCodeDescript.Text=Snomeds.GetByCode(ListRecEncCodes[i]).Description;//Guaranteed to exist in snomed table from above check
				}
			}
			if(countNotInSnomedTable>0) {
				MsgBox.Show(this,"The snomed table does not contain one or more codes from the list of recommended encounter codes.  The snomed table should be updated by running the Code System Importer tool found in Setup | Chart | EHR.");
			}
			if(comboEncCodes.SelectedIndex==-1) {//default enc code set to code not in recommended list and not 'none'
				switch(defaultEncCodeSystem) {
					case "CDT":
						textEncCodeValue.Text=ProcedureCodes.GetProcCode(defaultEncCode).ProcCode;//Will return a new ProcCode object, not null, if not found
						textEncCodeDescript.Text=ProcedureCodes.GetProcCode(defaultEncCode).Descript;
						break;
					case "CPT":
						Cpt cEnc=Cpts.GetByCode(defaultEncCode);
						if(cEnc!=null) {
							textEncCodeValue.Text=cEnc.CptCode;
							textEncCodeDescript.Text=cEnc.Description;
						}
						break;
					case "SNOMEDCT":
						Snomed sEnc=Snomeds.GetByCode(defaultEncCode);
						if(sEnc!=null) {
							textEncCodeValue.Text=sEnc.SnomedCode;
							textEncCodeDescript.Text=sEnc.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hEnc=Hcpcses.GetByCode(defaultEncCode);
						if(hEnc!=null) {
							textEncCodeValue.Text=hEnc.HcpcsCode;
							textEncCodeDescript.Text=hEnc.DescriptionShort;
						}
						break;
				}
			}
		}
		
		private void checkMU2_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				comboMU2.SelectedIndex=PrefC.GetInt(PrefName.MeaningfulUseTwo);
			}
		}

		private void comboEncCodes_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				comboEncCodes.SelectedIndex=OldEncListSelectedIdx;
				return;
			}
			NewEncCodeSystem="SNOMEDCT";
			textEncCodeValue.Text="";
			if(comboEncCodes.SelectedIndex==0) {//none
				textEncCodeDescript.Clear();
				labelEncWarning.Visible=true;
			}
			else {
				Snomed sEnc=Snomeds.GetByCode(comboEncCodes.SelectedItem.ToString());
				if(sEnc==null) {//this check may not be necessary now that we are not adding the code to the list to be selected if they do not have it in the snomed table.  Harmelss and safe.
					MsgBox.Show(this,"The snomed table does not contain this code.  The code should be added to the snomed table by running the Code System Importer tool.");
				}
				else {
					textEncCodeDescript.Text=sEnc.Description;
				}
				labelEncWarning.Visible=false;
			}
		}

		private void comboPregCodes_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				comboPregCodes.SelectedIndex=OldPregListSelectedIdx;
				return;
			}
			NewPregCodeSystem="SNOMEDCT";
			textPregCodeValue.Text="";
			if(comboPregCodes.SelectedIndex==0) {//none
				textPregCodeDescript.Clear();
				labelPregWarning.Visible=true;
			}
			else {
				Snomed sPreg=Snomeds.GetByCode(comboPregCodes.SelectedItem.ToString());
				if(sPreg==null) {
					MsgBox.Show(this,"The snomed table does not contain this code.  The code should be added to the snomed table by running the Code System Importer tool.");
				}
				else {
					textPregCodeDescript.Text=sPreg.Description;
				}
				labelPregWarning.Visible=false;
			}
		}

		private void butEncSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormS.IsSelectionMode=false;
			}
			else {
				FormS.IsSelectionMode=true;
			}
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				NewEncCodeSystem="SNOMEDCT";
				for(int i=1;i<comboEncCodes.Items.Count;i++) {
					if(FormS.SelectedSnomed.SnomedCode==comboEncCodes.Items[i].ToString()) {//if they go to snomed list and select one of the recommended codes, select in list
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
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormH.IsSelectionMode=false;
			}
			else {
				FormH.IsSelectionMode=true;
			}
			FormH.ShowDialog();
			if(FormH.DialogResult==DialogResult.OK) {
				NewEncCodeSystem="HCPCS";
				comboEncCodes.SelectedIndex=-1;
				textEncCodeValue.Text=FormH.SelectedHcpcs.HcpcsCode;
				textEncCodeDescript.Text=FormH.SelectedHcpcs.DescriptionShort;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncCdt_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormP.IsSelectionMode=false;
			}
			else {
				FormP.IsSelectionMode=true;
			}
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.OK) {
				NewEncCodeSystem="CDT";
				comboEncCodes.SelectedIndex=-1;
				ProcedureCode procCur=ProcedureCodes.GetProcCode(FormP.SelectedCodeNum);
				textEncCodeValue.Text=procCur.ProcCode;
				textEncCodeDescript.Text=procCur.Descript;
				//We might implement a CodeSystem column on the ProcCode table since it may have ICD9 and ICD10 codes in it.  If so, we can set the NewEncCodeSystem to the value in that new column.
				//NewEncCodeSystem=procCur.CodeSystem;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncCpt_Click(object sender,EventArgs e) {
			using FormCpts FormC=new FormCpts();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormC.IsSelectionMode=false;
			}
			else {
				FormC.IsSelectionMode=true;
			}
			FormC.ShowDialog();
			if(FormC.DialogResult==DialogResult.OK) {
				NewEncCodeSystem="CPT";
				comboEncCodes.SelectedIndex=-1;
				textEncCodeValue.Text=FormC.SelectedCpt.CptCode;
				textEncCodeDescript.Text=FormC.SelectedCpt.Description;
				labelEncWarning.Visible=true;
			}
		}

		private void butEncounterTool_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				return;
			}
			using FormEncounterTool FormE=new FormEncounterTool();
			if(comboEncCodes.SelectedIndex!=-1) {
				FormE.EncListSelectedIdx=comboEncCodes.SelectedIndex-1;//subtract 1 for 'none'
			}
			FormE.EncCodeValue=textEncCodeValue.Text;
			FormE.EncCodeSystem=NewEncCodeSystem;
			FormE.CodeDescription=textEncCodeDescript.Text;
			FormE.ShowDialog();
			if(FormE.DialogResult==DialogResult.OK) {
				comboEncCodes.Items.Clear();
				FillDefaultEncCode();
			}
		}

		private void butPregSnomed_Click(object sender,EventArgs e) {
			using FormSnomeds FormS=new FormSnomeds();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormS.IsSelectionMode=false;
			}
			else {
				FormS.IsSelectionMode=true;
			}
			FormS.ShowDialog();
			if(FormS.DialogResult==DialogResult.OK) {
				NewPregCodeSystem="SNOMEDCT";
				for(int i=1;i<comboPregCodes.Items.Count;i++) {
					if(FormS.SelectedSnomed.SnomedCode==comboPregCodes.Items[i].ToString()) {//if they go to snomed list and select one of the recommended codes, select in list
						comboPregCodes.SelectedIndex=i;
						textPregCodeValue.Clear();
						textPregCodeDescript.Text=FormS.SelectedSnomed.Description;
						labelPregWarning.Visible=false;
						return;
					}
				}
				comboPregCodes.SelectedIndex=-1;
				textPregCodeValue.Text=FormS.SelectedSnomed.SnomedCode;
				textPregCodeDescript.Text=FormS.SelectedSnomed.Description;
				labelPregWarning.Visible=true;
			}
		}

		private void butPregIcd9_Click(object sender,EventArgs e) {
			using FormIcd9s FormI9=new FormIcd9s();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormI9.IsSelectionMode=false;
			}
			else {
				FormI9.IsSelectionMode=true;
			}
			FormI9.ShowDialog();
			if(FormI9.DialogResult==DialogResult.OK) {
				NewPregCodeSystem="ICD9CM";
				comboPregCodes.SelectedIndex=-1;
				textPregCodeValue.Text=FormI9.SelectedIcd9.ICD9Code;
				textPregCodeDescript.Text=FormI9.SelectedIcd9.Description;
				labelPregWarning.Visible=true;
			}
		}

		private void butPregIcd10_Click(object sender,EventArgs e) {
			using FormIcd10s FormI10=new FormIcd10s();
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,false)) {
				FormI10.IsSelectionMode=false;
			}
			else {
				FormI10.IsSelectionMode=true;
			}
			FormI10.ShowDialog();
			if(FormI10.DialogResult==DialogResult.OK) {
				NewPregCodeSystem="ICD10CM";
				comboPregCodes.SelectedIndex=-1;
				textPregCodeValue.Text=FormI10.SelectedIcd10.Icd10Code;
				textPregCodeDescript.Text=FormI10.SelectedIcd10.Description;
				labelPregWarning.Visible=true;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			Prefs.UpdateBool(PrefName.EhrRxAlertHighSeverity,checkAlertHighSeverity.Checked);
			Prefs.UpdateInt(PrefName.MeaningfulUseTwo,comboMU2.SelectedIndex);
			Prefs.UpdateString(PrefName.CQMDefaultEncounterCodeSystem,NewEncCodeSystem);
			Prefs.UpdateString(PrefName.PregnancyDefaultCodeSystem,NewPregCodeSystem);
			Prefs.UpdateBool(PrefName.AutomaticSummaryOfCareWebmail,checkAutoWebmails.Checked);
			if(comboEncCodes.SelectedIndex==-1) {
				Prefs.UpdateString(PrefName.CQMDefaultEncounterCodeValue,textEncCodeValue.Text);
			}
			else {
				Prefs.UpdateString(PrefName.CQMDefaultEncounterCodeValue,comboEncCodes.SelectedItem.ToString());
			}
			if(comboPregCodes.SelectedIndex==-1) {
				Prefs.UpdateString(PrefName.PregnancyDefaultCodeValue,textPregCodeValue.Text);
			}
			else {
				Prefs.UpdateString(PrefName.PregnancyDefaultCodeValue,comboPregCodes.SelectedItem.ToString());
			}
			//A diseasedef with this default pregnancy code will be inserted if needed the first time they check the pregnant box on a vitalsign.  The DiseaseName will be "Pregnant" with the correct codevalue/system.
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}