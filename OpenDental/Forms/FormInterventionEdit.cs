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
		private List<EhrCode> listCodes;
		private Dictionary<string,string> dictValueCodeSets;

		public FormInterventionEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary></summary>
		private void FormInterventionEdit_Load(object sender,EventArgs e) {
			comboCodeSet.Enabled=IsSelectionMode;//only allow changing code set if IsSelectionMode
			gridMain.AllowSelection=IsSelectionMode;//only allow changing the selected intervention code if IsSelectionMode
			dictValueCodeSets=new Dictionary<string,string>();
			comboCodeSet.Items.Add("All");
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.AboveNormalWeight) {
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Follow-up");
				dictValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Follow-up","2.16.840.1.113883.3.600.1.1525");
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Referral");
				dictValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Referral","2.16.840.1.113883.3.600.1.1527");
				comboCodeSet.Items.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				dictValueCodeSets.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication","2.16.840.1.113883.3.600.1.1498");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.BelowNormalWeight) {
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Follow-up");
				dictValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Follow-up","2.16.840.1.113883.3.600.1.1528");
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Referral");
				dictValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Referral","2.16.840.1.113883.3.600.1.1527");
				comboCodeSet.Items.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				dictValueCodeSets.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication","2.16.840.1.113883.3.600.1.1499");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.Nutrition || InterventionCur.CodeSet==InterventionCodeSet.PhysicalActivity) {
				comboCodeSet.Items.Add(InterventionCodeSet.Nutrition.ToString()+" Counseling");
				dictValueCodeSets.Add(InterventionCodeSet.Nutrition.ToString()+" Counseling","2.16.840.1.113883.3.464.1003.195.12.1003");
				comboCodeSet.Items.Add(InterventionCodeSet.PhysicalActivity.ToString()+" Counseling");
				dictValueCodeSets.Add(InterventionCodeSet.PhysicalActivity.ToString()+" Counseling","2.16.840.1.113883.3.464.1003.118.12.1035");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.TobaccoCessation) {
				comboCodeSet.Items.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Counseling");
				dictValueCodeSets.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Counseling","2.16.840.1.113883.3.526.3.509");
				comboCodeSet.Items.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				dictValueCodeSets.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication","2.16.840.1.113883.3.526.3.1190");
			}
			if(IsAllTypes || InterventionCur.CodeSet==InterventionCodeSet.Dialysis) {
				comboCodeSet.Items.Add(InterventionCodeSet.Dialysis.ToString()+" Education");
				dictValueCodeSets.Add(InterventionCodeSet.Dialysis.ToString()+" Education","2.16.840.1.113883.3.464.1003.109.12.1016");
				comboCodeSet.Items.Add(InterventionCodeSet.Dialysis.ToString()+" Related Services");
				dictValueCodeSets.Add(InterventionCodeSet.Dialysis.ToString()+" Related Services","2.16.840.1.113883.3.464.1003.109.12.1015");
			}
			comboCodeSet.SelectedIndex=0;
			//need to set the comboCodeSet based on InterventionCur sent in
			int codeSetIdx=0;//this will be the index to set the comboCodeSet to if found in a subset of codes, otherwise defaults to All (index 0).
			foreach(string val in dictValueCodeSets.Values) {
				codeSetIdx++;
				listCodes=EhrCodes.GetForValueSetOIDs(new List<string> { val },true);
				for(int i=0;i<listCodes.Count;i++) {
					if(listCodes[i].CodeValue==InterventionCur.CodeValue && listCodes[i].CodeSystem==InterventionCur.CodeSystem) {
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Code",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("CodeSystem",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",200);
			gridMain.ListGridColumns.Add(col);
			string selectedValue=comboCodeSet.SelectedItem.ToString();
			List<string> listValSetOIDs=new List<string>();
			if(selectedValue=="All") {
				listValSetOIDs=new List<string>(dictValueCodeSets.Values);
			}
			else {//this will limit the codes to only one value set oid
				listValSetOIDs.Add(dictValueCodeSets[selectedValue]);
			}
			listCodes=EhrCodes.GetForValueSetOIDs(listValSetOIDs,true);//these codes will exist in the corresponding table or will not be in the list
			gridMain.ListGridRows.Clear();
			GridRow row;
			int selectedIdx=-1;
			for(int i=0;i<listCodes.Count;i++) {				
				row=new GridRow();
				row.Cells.Add(listCodes[i].CodeValue);
				row.Cells.Add(listCodes[i].CodeSystem);
				//Retrieve description from the associated table
				string descript="";
				switch(listCodes[i].CodeSystem) {
					case "CPT":
						Cpt cCur=Cpts.GetByCode(listCodes[i].CodeValue);
						if(cCur!=null) {
							descript=cCur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(listCodes[i].CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "ICD9CM":
						ICD9 i9Cur=ICD9s.GetByCode(listCodes[i].CodeValue);
						if(i9Cur!=null) {
							descript=i9Cur.Description;
						}
						break;
					case "ICD10CM":
						Icd10 i10Cur=Icd10s.GetByCode(listCodes[i].CodeValue);
						if(i10Cur!=null) {
							descript=i10Cur.Description;
						}
						break;
					case "RXNORM":
						descript=RxNorms.GetDescByRxCui(listCodes[i].CodeValue);
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listCodes[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				gridMain.ListGridRows.Add(row);
				if(listCodes[i].CodeValue==InterventionCur.CodeValue && listCodes[i].CodeSystem==InterventionCur.CodeSystem) {
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
				codeVal=listCodes[gridMain.GetSelectedIndex()].CodeValue;
				codeSys=listCodes[gridMain.GetSelectedIndex()].CodeSystem;
			}
			//save--------------------------------------
			//Intervention grid may contain medications, have to insert a new med if necessary and load FormMedPat for user to input data
			if(codeSys=="RXNORM" && !checkPatientDeclined.Checked) {
				//codeVal will be RxCui of medication, see if it already exists in Medication table
				Medication medCur=Medications.GetMedicationFromDbByRxCui(PIn.Long(codeVal));
				if(medCur==null) {//no med with this RxCui, create one
					medCur=new Medication();
					Medications.Insert(medCur);//so that we will have the primary key
					medCur.GenericNum=medCur.MedicationNum;
					medCur.RxCui=PIn.Long(codeVal);
					medCur.MedName=RxNorms.GetDescByRxCui(codeVal);
					Medications.Update(medCur);
					Medications.RefreshCache();//refresh cache to include new medication
				}
				MedicationPat medPatCur=new MedicationPat();
				medPatCur.PatNum=InterventionCur.PatNum;
				medPatCur.ProvNum=InterventionCur.ProvNum;
				medPatCur.MedicationNum=medCur.MedicationNum;
				medPatCur.RxCui=medCur.RxCui;
				medPatCur.DateStart=date;
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=medPatCur;
				FormMP.IsNew=true;
				FormMP.ShowDialog();
				if(FormMP.DialogResult!=DialogResult.OK) {
					return;
				}
				if(FormMP.MedicationPatCur.DateStart.Date<InterventionCur.DateEntry.AddMonths(-6).Date || FormMP.MedicationPatCur.DateStart.Date>InterventionCur.DateEntry.Date) {
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
					foreach(KeyValuePair<string,string> kvp in dictValueCodeSets) {
						List<EhrCode> listCodes=EhrCodes.GetForValueSetOIDs(new List<string> { kvp.Value },true);
						for(int i=0;i<listCodes.Count;i++) {
							if(listCodes[i].CodeValue==codeVal) {
								listVSFound.Add(kvp.Key);
								break;
							}
						}
					}
					if(listVSFound.Count>1) {//Selected code found in more than one value set, ask the user which InterventionCodeSet to assign to this intervention
						using InputBox chooseSet=new InputBox(Lan.g(this,"The selected code belongs to more than one intervention code set.  Select the code set to assign to this intervention from the list below."),listVSFound);
						if(chooseSet.ShowDialog()!=DialogResult.OK) {
							return;
						}
						if(chooseSet.comboSelection.SelectedIndex==-1) {
							MsgBox.Show(this,"You must select an intervention code set for the selected code.");
							return;
						}
						selectedCodeSet=chooseSet.comboSelection.GetSelected<string>().Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries)[0];
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
					List<EhrCode> listCodes=EhrCodes.GetForValueSetOIDs(new List<string> { dictValueCodeSets[InterventionCodeSet.PhysicalActivity.ToString()+" Counseling"] },true);
					for(int i=0;i<listCodes.Count;i++) {
						if(listCodes[i].CodeValue==codeVal) {
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