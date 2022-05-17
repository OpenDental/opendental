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
		private List<Encounter> _listEncounters;
		public Patient PatientCur;

		public FormEncounters() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		public void FormEncounters_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Date",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Prov",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Code",110);
			gridMain.Columns.Add(col);
			col=new GridColumn("Description",180);
			gridMain.Columns.Add(col);
			col=new GridColumn("Note",100);
			gridMain.Columns.Add(col);
			_listEncounters=Encounters.Refresh(PatientCur.PatNum);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEncounters.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEncounters[i].DateEncounter.ToShortDateString());
				row.Cells.Add(Providers.GetAbbr(_listEncounters[i].ProvNum));
				row.Cells.Add(_listEncounters[i].CodeValue);
				string descript="";
				//to get description, first determine which table the code is from.  Encounter is only allowed to be a CDT, CPT, HCPCS, and SNOMEDCT.
				switch(_listEncounters[i].CodeSystem) {
					case "CDT":
						descript=ProcedureCodes.GetProcCode(_listEncounters[i].CodeValue).Descript;
						break;
					case "CPT":
						Cpt cpt=Cpts.GetByCode(_listEncounters[i].CodeValue);
						if(cpt!=null) {
							descript=cpt.Description;
						}
						break;
					case "HCPCS":
						Hcpcs hcpcs=Hcpcses.GetByCode(_listEncounters[i].CodeValue);
						if(hcpcs!=null) {
							descript=hcpcs.DescriptionShort;
						}
						break;
					case "SNOMEDCT":
						Snomed snomed=Snomeds.GetByCode(_listEncounters[i].CodeValue);
						if(snomed!=null) {
							descript=snomed.Description;
						}
						break;
				}
				row.Cells.Add(descript);
				row.Cells.Add(_listEncounters[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormEncounterEdit formEncounterEdit=new FormEncounterEdit(_listEncounters[e.Row]);
			formEncounterEdit.ShowDialog();
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Encounter encounter=new Encounter();
			encounter.PatNum=PatientCur.PatNum;
			encounter.ProvNum=PatientCur.PriProv;
			encounter.DateEncounter=DateTime.Today;
			encounter.IsNew=true;
			using FormEncounterEdit formEncounterEdit=new FormEncounterEdit(encounter);
			formEncounterEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {
			this.Close();
		}
	}
}