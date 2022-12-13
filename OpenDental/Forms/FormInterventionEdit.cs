using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInterventionEdit:FormODBase {
		public Intervention InterventionCur;
		///<summary>This bool will determine if we show every intervention type we support or only a specific subset related to the form we launch from.  Currently only set to true when adding an intervention from FormInterventions when pressing the Add button.</summary>
		public bool IsAllTypes;
		///<summary>Do not let them edit historical interventions, the OK button will be disabled if this is false.</summary>
		public bool IsSelectionMode;
		private List<EhrCode> _listEhrCodes;
		private Dictionary<string,string> _dictionaryValueCodeSets;

		public FormInterventionEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		private void FormInterventionEdit_Load(object sender,EventArgs e) {
			comboCodeSet.Enabled=IsSelectionMode;//only allow changing code set if IsSelectionMode
			gridMain.AllowSelection=IsSelectionMode;//only allow changing the selected intervention code if IsSelectionMode
			_dictionaryValueCodeSets=new Dictionary<string,string>();
			comboCodeSet.Items.Add("All");
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.AboveNormalWeight) {
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Follow-up");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Follow-up","2.16.840.1.113883.3.600.1.1525");
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Referral");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Referral","2.16.840.1.113883.3.600.1.1527");
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication","2.16.840.1.113883.3.600.1.1498");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.BelowNormalWeight) {
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Follow-up");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Follow-up","2.16.840.1.113883.3.600.1.1528");
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Referral");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Referral","2.16.840.1.113883.3.600.1.1527");
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication","2.16.840.1.113883.3.600.1.1499");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.Nutrition || InterventionCur.CodeSet==InterventionCodeSet.PhysicalActivity) {
				comboCodeSet.Items.Add(InterventionCodeSet.Nutrition.ToString()+" Counseling");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.Nutrition.ToString()+" Counseling","2.16.840.1.113883.3.464.1003.195.12.1003");
				comboCodeSet.Items.Add(InterventionCodeSet.PhysicalActivity.ToString()+" Counseling");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.PhysicalActivity.ToString()+" Counseling","2.16.840.1.113883.3.464.1003.118.12.1035");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.TobaccoCessation) {
				comboCodeSet.Items.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Counseling");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Counseling","2.16.840.1.113883.3.526.3.509");
				comboCodeSet.Items.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication","2.16.840.1.113883.3.526.3.1190");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.Dialysis) {
				comboCodeSet.Items.Add(InterventionCodeSet.Dialysis.ToString()+" Education");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.Dialysis.ToString()+" Education","2.16.840.1.113883.3.464.1003.109.12.1016");
				comboCodeSet.Items.Add(InterventionCodeSet.Dialysis.ToString()+" Related Services");
				_dictionaryValueCodeSets.Add(InterventionCodeSet.Dialysis.ToString()+" Related Services","2.16.840.1.113883.3.464.1003.109.12.1015");
			}
			comboCodeSet.SelectedIndex=0;
			//need to set the comboCodeSet based on InterventionCur sent in
			int codeSetIdx=0;//this will be the index to set the comboCodeSet to if found in a subset of codes, otherwise defaults to All (index 0).
			foreach(string val in _dictionaryValueCodeSets.Values) {
				codeSetIdx++;
				_listEhrCodes=EhrCodes.GetForValueSetOIDs(new List<string> { val },true);
				for(int i=0;i<_listEhrCodes.Count;i++) {
					if(_listEhrCodes[i].CodeValue==InterventionCur.CodeValue && _listEhrCodes[i].CodeSystem==InterventionCur.CodeSystem) {
						comboCodeSet.SelectedIndex=codeSetIdx;
						break;
					}
				}
			}
			textDate.Text=InterventionCur.DateEntry.ToShortDateString();
			textNote.Text=InterventionCur.Note;
			checkPatientDeclined.Checked=InterventionCur.IsPatDeclined;
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Code",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("CodeSystem",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",200);
			gridMain.Columns.Add(col);
			string selectedValue=comboCodeSet.SelectedItem.ToString();
			List<string> listValSetOIDs=new List<string>();
			if(selectedValue=="All") {
				listValSetOIDs=new List<string>(_dictionaryValueCodeSets.Values);
			}
			else {//this will limit the codes to only one value set oid
				listValSetOIDs.Add(_dictionaryValueCodeSets[selectedValue]);
			}
			_listEhrCodes=EhrCodes.GetForValueSetOIDs(listValSetOIDs,true);//these codes will exist in the corresponding table or will not be in the list
			gridMain.ListGridRows.Clear();
			GridRow row;
			int selectedIdx=-1;
			for(int i=0;i<_listEhrCodes.Count;i++) {				
				row=new GridRow();
				row.Cells.Add(_listEhrCodes[i].CodeValue);
				row.Cells.Add(_listEhrCodes[i].CodeSystem);
				//Retrieve description from the associated table
				string descript="";
				switch(_listEhrCodes[i].CodeSystem) {
					case "CPT":
						Cpt cpt=Cpts.GetByCode(_listEhrCodes[i].CodeValue);
						if(cpt!=null) {
							descript=cpt.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hcpcs=Hcpcses.GetByCode(_listEhrCodes[i].CodeValue);
						if(hcpcs!=null) {
							descript=hcpcs.DescriptionShort;
						}
						break;
					case "ICD9CM":
						ICD9 iCD9=ICD9s.GetByCode(_listEhrCodes[i].CodeValue);
						if(iCD9!=null) {
							descript=iCD9.Description;
						}
						break;
					case "ICD10CM":
						Icd10 icd10=Icd10s.GetByCode(_listEhrCodes[i].CodeValue);
						if(icd10!=null) {
							descript=icd10.Description;
						}
						break;
					case "RXNORM":
						descript=RxNorms.GetDescByRxCui(_listEhrCodes[i].CodeValue);
						break;
					case "SNOMEDCT":
						Snomed snomed=Snomeds.GetByCode(_listEhrCodes[i].CodeValue);
						if(snomed!=null) {
							descript=snomed.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				gridMain.ListGridRows.Add(row);
				if(_listEhrCodes[i].CodeValue==InterventionCur.CodeValue && _listEhrCodes[i].CodeSystem==InterventionCur.CodeSystem) {
					selectedIdx=i;
				}
			}
			gridMain.EndUpdate();
			if(selectedIdx>-1) {
				gridMain.SetSelected(selectedIdx,true);
				gridMain.ScrollToIndex(selectedIdx);
			}
		}

		private void comboCodeSet_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(InterventionCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			Interventions.Delete(InterventionCur.InterventionNum);
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//validate--------------------------------------
			DateTime date;
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			try {
				date=DateTime.Parse(textDate.Text);
			}
			catch {
				MsgBox.Show(this,"Please fix date first.");
				return;
			}
			string codeVal="";
			string codeSys="";
			if(gridMain.GetSelectedIndex()==-1) {//no intervention code selected
				MsgBox.Show(this,"You must select a code for this intervention.");
				return;
			}
			else {
				codeVal=_listEhrCodes[gridMain.GetSelectedIndex()].CodeValue;
				codeSys=_listEhrCodes[gridMain.GetSelectedIndex()].CodeSystem;
			}
			//save--------------------------------------
			//Intervention grid may contain medications, have to insert a new med if necessary and load FormMedPat for user to input data
			if(codeSys=="RXNORM" && !checkPatientDeclined.Checked) {
				//codeVal will be RxCui of medication, see if it already exists in Medication table
				Medication medication=Medications.GetMedicationFromDbByRxCui(PIn.Long(codeVal));
				if(medication==null) {//no med with this RxCui, create one
					medication=new Medication();
					Medications.Insert(medication);//so that we will have the primary key
					medication.GenericNum=medication.MedicationNum;
					medication.RxCui=PIn.Long(codeVal);
					medication.MedName=RxNorms.GetDescByRxCui(codeVal);
					Medications.Update(medication);
					Medications.RefreshCache();//refresh cache to include new medication
				}
				MedicationPat medicationPat=new MedicationPat();
				medicationPat.PatNum=InterventionCur.PatNum;
				medicationPat.ProvNum=InterventionCur.ProvNum;
				medicationPat.MedicationNum=medication.MedicationNum;
				medicationPat.RxCui=medication.RxCui;
				medicationPat.DateStart=date;
				using FormMedPat formMedPat=new FormMedPat();
				formMedPat.MedicationPatCur=medicationPat;
				formMedPat.IsNew=true;
				formMedPat.ShowDialog();
				if(formMedPat.DialogResult!=DialogResult.OK) {
					return;
				}
				if(formMedPat.MedicationPatCur.DateStart.Date<InterventionCur.DateEntry.AddMonths(-6).Date || formMedPat.MedicationPatCur.DateStart.Date>InterventionCur.DateEntry.Date) {
					MsgBox.Show(this,"The medication order just entered is not within the 6 months prior to the date of this intervention.  You can modify the date of the medication order in the patient's medical history section.");
				}
				DialogResult=DialogResult.OK;
				return;
			}
			InterventionCur.DateEntry=date;
			InterventionCur.CodeValue=codeVal;
			InterventionCur.CodeSystem=codeSys;
			InterventionCur.Note=textNote.Text;
			InterventionCur.IsPatDeclined=checkPatientDeclined.Checked;
			string selectedCodeSet=comboCodeSet.SelectedItem.ToString().Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries)[0];
			if(IsAllTypes) {//CodeSet will be set by calling function unless showing all types, in which case we need to determine which InterventionCodeSet to assign
				if(selectedCodeSet=="All") {//All types showing and set to All, have to determine which InterventionCodeSet this code belongs to
					List<string> listVSFound=new List<string>();
					foreach(KeyValuePair<string,string> kvp in _dictionaryValueCodeSets) {
						List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(new List<string> { kvp.Value },true);
						for(int i=0;i<listEhrCodes.Count;i++) {
							if(listEhrCodes[i].CodeValue==codeVal) {
								listVSFound.Add(kvp.Key);
								break;
							}
						}
					}
					if(listVSFound.Count>1) {//Selected code found in more than one value set, ask the user which InterventionCodeSet to assign to this intervention
						using InputBox inputBox=new InputBox(Lan.g(this,"The selected code belongs to more than one intervention code set.  Select the code set to assign to this intervention from the list below."),listVSFound);
						if(inputBox.ShowDialog()!=DialogResult.OK) {
							return;
						}
						if(inputBox.comboSelection.SelectedIndex==-1) {
							MsgBox.Show(this,"You must select an intervention code set for the selected code.");
							return;
						}
						selectedCodeSet=inputBox.comboSelection.GetSelected<string>().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries)[0];
					}
					else {//the code must belong to at least one value set, since count in listVSFound is not greater than 1, it must be a code from exactly one set, use that for the InterventionCodeSet
						selectedCodeSet=listVSFound[0].Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries)[0];
					}
				}
				InterventionCur.CodeSet=(InterventionCodeSet)Enum.Parse(typeof(InterventionCodeSet),selectedCodeSet);
			}
			//Nutrition is used for both nutrition and physical activity counseling for children, we have to determine which set this code belongs to
			else if(InterventionCur.CodeSet==InterventionCodeSet.Nutrition && selectedCodeSet!="Nutrition") {//Nutrition set by calling form, user is showing all or physical activity codes only
				if(selectedCodeSet=="All") {//showing all codes from Nutrition and PhysicalActivity interventions, determine which set it belongs to
					//No codes exist in both code sets, so if it is not in the PhysicalActivity code set, we can safely assume this is a Nutrition intervention
					List<EhrCode> listEhrCodes=EhrCodes.GetForValueSetOIDs(new List<string> { _dictionaryValueCodeSets[InterventionCodeSet.PhysicalActivity.ToString()+" Counseling"] },true);
					for(int i=0;i<listEhrCodes.Count;i++) {
						if(listEhrCodes[i].CodeValue==codeVal) {
							InterventionCur.CodeSet=InterventionCodeSet.PhysicalActivity;
							break;
						}
					}
				}
				else {
					InterventionCur.CodeSet=InterventionCodeSet.PhysicalActivity;
				}
			}
			else {
				//if not all types, and not Nutrition with All or PhysicalActivity selected in combo box, the code set sent in by calling form will remain the code set for this intervention
			}
			if(InterventionCur.IsNew) {
				Interventions.Insert(InterventionCur);
			}
			else {
				Interventions.Update(InterventionCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}