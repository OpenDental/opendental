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
	public partial class FormEncounters:FormODBase {
		private List<Encounter> listEncs;
		public Patient PatCur;

		public FormEncounters() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		public void FormEncounters_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Prov",70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Code",110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Description",180);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.ListGridColumns.Add(col);
			listEncs=Encounters.Refresh(PatCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEncs.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listEncs[i].DateEncounter.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(listEncs[i].ProvNum));
				row.Cells.Add(listEncs[i].CodeValue);
				string descript="";
				//to get description, first determine which table the code is from.  Encounter is only allowed to be a CDT, CPT, HCPCS, and SNOMEDCT.
				switch(listEncs[i].CodeSystem) {
					case "CDT":
						descript=ProcedureCodes.GetProcCode(listEncs[i].CodeValue).Descript;
						break;
					case "CPT":
						Cpt cptCur=Cpts.GetByCode(listEncs[i].CodeValue);
						if(cptCur!=null) {
							descript=cptCur.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hCur=Hcpcses.GetByCode(listEncs[i].CodeValue);
						if(hCur!=null) {
							descript=hCur.DescriptionShort;
						}
						break;
					case "SNOMEDCT":
						Snomed sCur=Snomeds.GetByCode(listEncs[i].CodeValue);
						if(sCur!=null) {
							descript=sCur.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Cells.Add(listEncs[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEncounterEdit FormEE=new FormEncounterEdit(listEncs[e.Row]);
			FormEE.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Encounter EncCur=new Encounter();
			EncCur.PatNum=PatCur.PatNum;
			EncCur.ProvNum=PatCur.PriProv;
			EncCur.DateEncounter=DateTime.Today;
			EncCur.IsNew=true;
			using FormEncounterEdit FormEE=new FormEncounterEdit(EncCur);
			FormEE.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}