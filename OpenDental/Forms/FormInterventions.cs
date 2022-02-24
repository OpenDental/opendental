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
		public Patient PatCur;
		private List<Intervention> listIntervention;
		private List<MedicationPat> listMedPats;

		public FormInterventions() {
			InitializeComponent();
			InitializeLayoutManager();
		}
		
		private void FormInterventions_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Prov",50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Intervention Type",115);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code System",85);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code Description",300);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			#region Interventions
			listIntervention=Interventions.Refresh(PatCur.PatNum);
			for(int i=0;i<listIntervention.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listIntervention[i].DateEntry.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(listIntervention[i].ProvNum));
				row.Cells.Add(listIntervention[i].CodeSet.ToString());
				row.Cells.Add(listIntervention[i].CodeValue);
				row.Cells.Add(listIntervention[i].CodeSystem);
				//Description of Intervention---------------------------------------------
				//to get description, first determine which table the code is from.  Interventions are allowed to be SNOMEDCT, ICD9, ICD10, HCPCS, or CPT.
				string descript="";
				switch(listIntervention[i].CodeSystem) {
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listIntervention[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
					case "ICD9CM":
						ICD9 i9Cur=ICD9s.GetByCode(listIntervention[i].CodeValue);
						if(i9Cur!=null) {
							descript=i9Cur.Description;
						}
						break;
					case "ICD10CM":
						Icd10 i10Cur=Icd10s.GetByCode(listIntervention[i].CodeValue);
						if(i10Cur!=null) {
							descript=i10Cur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(listIntervention[i].CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(listIntervention[i].CodeValue);
						if(cptCur!=null) {
							descript=cptCur.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Cells.Add(listIntervention[i].Note);
				row.Tag=listIntervention[i];
				gridMain.ListGridRows.Add(row);
			}
			#endregion
			#region MedicationPats
			listMedPats=MedicationPats.Refresh(PatCur.PatNum,true);
			if(listMedPats.Count>0) {
				//The following medications are used as interventions for some measures.  Include them in the intervention window if they belong to these value sets.
				//Above Normal Medications RxNorm Value Set, Below Normal Medications RxNorm Value Set, Tobacco Use Cessation Pharmacotherapy Value Set
				List<string> listVS=new List<string> { "2.16.840.1.113883.3.600.1.1498","2.16.840.1.113883.3.600.1.1499","2.16.840.1.113883.3.526.3.1190" };
				List<EhrCode> listEhrMeds=EhrCodes.GetForValueSetOIDs(listVS,true);
				for(int i=listMedPats.Count-1;i>-1;i--) {
					bool found=false;
					for(int j=0;j<listEhrMeds.Count;j++) {
						if(listMedPats[i].RxCui.ToString()==listEhrMeds[j].CodeValue) {
							found=true;
							break;
						}
					}
					if(!found) {
						listMedPats.RemoveAt(i);
					}
				}
			}
			for(int i=0;i<listMedPats.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listMedPats[i].DateStart.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(listMedPats[i].ProvNum));
				if(listMedPats[i].RxCui==314153 || listMedPats[i].RxCui==692876) {
					row.Cells.Add(InterventionCodeSet.AboveNormalWeight.ToString()+" Medication");
				}
				else if(listMedPats[i].RxCui==577154 || listMedPats[i].RxCui==860215 || listMedPats[i].RxCui==860221 || listMedPats[i].RxCui==860225 || listMedPats[i].RxCui==860231) {
					row.Cells.Add(InterventionCodeSet.BelowNormalWeight.ToString()+" Medication");
				}
				else {//There are 48 total medications that can be used as interventions.  The remaining 41 medications are tobacco cessation medications
					row.Cells.Add(InterventionCodeSet.TobaccoCessation.ToString()+" Medication");
				}
				row.Cells.Add(listMedPats[i].RxCui.ToString());
				row.Cells.Add("RXNORM");
				//Medications that are used as interventions are all RxNorm codes, get description from that table
				string descript=RxNorms.GetDescByRxCui(listMedPats[i].RxCui.ToString());
				row.Cells.Add(descript);
				row.Cells.Add(listMedPats[i].PatNote);
				row.Tag=listMedPats[i];
				gridMain.ListGridRows.Add(row);
			}
			#endregion
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			object objCur=gridMain.ListGridRows[e.Row].Tag;
			if(objCur.GetType().Name=="Intervention") {//grid can contain MedicationPat or Intervention objects, launch appropriate window
				using FormInterventionEdit FormInt=new FormInterventionEdit();
				FormInt.InterventionCur=(Intervention)objCur;
				FormInt.IsAllTypes=false;
				FormInt.IsSelectionMode=false;
				FormInt.ShowDialog();
			}
			if(objCur.GetType().Name=="MedicationPat") {
				using FormMedPat FormMP=new FormMedPat();
				FormMP.MedicationPatCur=(MedicationPat)objCur;
				FormMP.IsNew=false;
				FormMP.ShowDialog();
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormInterventionEdit FormInt=new FormInterventionEdit();
			FormInt.InterventionCur=new Intervention();
			FormInt.InterventionCur.IsNew=true;
			FormInt.InterventionCur.PatNum=PatCur.PatNum;
			FormInt.InterventionCur.ProvNum=PatCur.PriProv;
			FormInt.InterventionCur.DateEntry=DateTime.Now;
			FormInt.IsAllTypes=true;
			FormInt.IsSelectionMode=true;
			FormInt.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}