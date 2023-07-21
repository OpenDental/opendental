using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using OpenDental;

namespace OpenDental {
	public partial class FormInterventions:FormODBase {
		public Patient PatientCur;
		private List<Intervention> _listInterventions;
		private List<MedicationPat> _listMedicationPats;

		public FormInterventions() {
			InitializeComponent();
			InitializeLayoutManager();
		}
		
		private void FormInterventions_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Prov",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Intervention Type",115);
			gridMain.Columns.Add(col);
			col=new GridColumn("Code",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Code System",85);
			gridMain.Columns.Add(col);
			col=new GridColumn("Code Description",300);
			gridMain.Columns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			#region Interventions
			_listInterventions=Interventions.Refresh(PatientCur.PatNum);
			for(int i=0;i<_listInterventions.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listInterventions[i].DateEntry.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listInterventions[i].ProvNum));
				row.Cells.Add(_listInterventions[i].CodeSet.ToString());
				row.Cells.Add(_listInterventions[i].CodeValue);
				row.Cells.Add(_listInterventions[i].CodeSystem);
				//Description of Intervention---------------------------------------------
				//to get description, first determine which table the code is from.  Interventions are allowed to be SNOMEDCT, ICD9, ICD10, HCPCS, or CPT.
				string descript="";
				switch(_listInterventions[i].CodeSystem) {
					case "SNOMEDCT":
						Snomed snomed=Snomeds.GetByCode(_listInterventions[i].CodeValue);
						if(snomed!=null) {
							descript=snomed.Description;
						}
						break;
					case "ICD9CM":
						ICD9 iCD9=ICD9s.GetByCode(_listInterventions[i].CodeValue);
						if(iCD9!=null) {
							descript=iCD9.Description;
						}
						break;
					case "ICD10CM":
						Icd10 icd10=Icd10s.GetByCode(_listInterventions[i].CodeValue);
						if(icd10!=null) {
							descript=icd10.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hcpcs=Hcpcses.GetByCode(_listInterventions[i].CodeValue);
						if(hcpcs!=null) {
							descript=hcpcs.DescriptionShort;
						}
						break;
					case "CPT":
						Cpt cpt=Cpts.GetByCode(_listInterventions[i].CodeValue);
						if(cpt!=null) {
							descript=cpt.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Cells.Add(_listInterventions[i].Note);
				row.Tag=_listInterventions[i];
				gridMain.ListGridRows.Add(row);
			}
			#endregion
			#region MedicationPats
			_listMedicationPats=MedicationPats.Refresh(PatientCur.PatNum,true);
			if(_listMedicationPats.Count>0) {
				//The following medications are used as interventions for some measures.  Include them in the intervention window if they belong to these value sets.
				//Above Normal Medications RxNorm Value Set, Below Normal Medications RxNorm Value Set, Tobacco Use Cessation Pharmacotherapy Value Set
				List<string> listVS=new List<string> { "2.16.840.1.113883.3.600.1.1498","2.16.840.1.113883.3.600.1.1499","2.16.840.1.113883.3.526.3.1190" };
				List<EhrCode> listEhrCodesMeds=EhrCodes.GetForValueSetOIDs(listVS,true);
				for(int i=_listMedicationPats.Count-1;i>-1;i--) {
					bool found=false;
					for(int j=0;j<listEhrCodesMeds.Count;j++) {
						if(_listMedicationPats[i].RxCui.ToString()==listEhrCodesMeds[j].CodeValue) {
							found=true;
							break;
						}
					}
					if(!found) {
						_listMedicationPats.RemoveAt(i);
					}
				}
			}
			for(int i=0;i<_listMedicationPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listMedicationPats[i].DateStart.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listMedicationPats[i].ProvNum));
				if(_listMedicationPats[i].RxCui==314153 || _listMedicationPats[i].RxCui==692876) {
					row.Cells.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				}
				else if(_listMedicationPats[i].RxCui==577154 || _listMedicationPats[i].RxCui==860215 || _listMedicationPats[i].RxCui==860221 || _listMedicationPats[i].RxCui==860225 || _listMedicationPats[i].RxCui==860231) {
					row.Cells.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				}
				else {//There are 48 total medications that can be used as interventions.  The remaining 41 medications are tobacco cessation medications
				row.Cells.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				}
				row.Cells.Add(_listMedicationPats[i].RxCui.ToString());
				row.Cells.Add("RXNORM");
				//Medications that are used as interventions are all RxNorm codes, get description from that table
				string descript=RxNorms.GetDescByRxCui(_listMedicationPats[i].RxCui.ToString());
				row.Cells.Add(descript);
				row.Cells.Add(_listMedicationPats[i].PatNote);
				row.Tag=_listMedicationPats[i];
				gridMain.ListGridRows.Add(row);
			}
			#endregion
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			object obj=gridMain.ListGridRows[e.Row].Tag;
			if(obj.GetType().Name=="Intervention") {//grid can contain MedicationPat or Intervention objects, launch appropriate window
				using FormInterventionEdit formInterventionEdit=new FormInterventionEdit();
				formInterventionEdit.InterventionCur=(Intervention)obj;
				formInterventionEdit.IsAllTypes=false;
				formInterventionEdit.IsSelectionMode=false;
				formInterventionEdit.ShowDialog();
				FillGrid();
				return;
			}
			if(obj.GetType().Name!="MedicationPat") {
				FillGrid();
				return;
			}
			using FormMedPat formMedPat=new FormMedPat();
			formMedPat.MedicationPatCur=(MedicationPat)obj;
			formMedPat.IsNew=false;
			formMedPat.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormInterventionEdit formInterventionEdit=new FormInterventionEdit();
			formInterventionEdit.InterventionCur=new Intervention();
			formInterventionEdit.InterventionCur.IsNew=true;
			formInterventionEdit.InterventionCur.PatNum=PatientCur.PatNum;
			formInterventionEdit.InterventionCur.ProvNum=PatientCur.PriProv;
			formInterventionEdit.InterventionCur.DateEntry=DateTime.Now;
			formInterventionEdit.IsAllTypes=true;
			formInterventionEdit.IsSelectionMode=true;
			formInterventionEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}