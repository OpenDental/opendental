using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOperatoryPick:FormODBase {

		///<summary>After this window closes, this will be the OperatoryNum of the selected operatory.</summary>
		public long OperatoryNumSelected;
		///<summary>Passed in list of operatories shown to user.</summary>
		private List<Operatory> _listOperatories;

		public FormOperatoryPick(List<Operatory> listOperatories) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listOperatories=listOperatories.Select(x=>x.Copy()).ToList();
		}
		
		private void FormOperatoryPick_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!SelectOperatory()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			int opNameWidth=180;
			int clinicWidth=85;
			if(!PrefC.HasClinicsEnabled) {
				//Clinics are hidden so add the width of the clinic column to the Op Name column because the clinic column will not show.
				opNameWidth+=clinicWidth;
			}
			GridColumn col=new GridColumn(Lan.g("TableOperatories","Op Name"),opNameWidth);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Abbrev"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHidden"),64,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableOperatories","Clinic"),clinicWidth);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g("TableOperatories","Provider"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Hygienist"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHygiene"),64,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsWebSched"),74,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsNewPat"),50,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<long> listOperatoryNumsWSNPA=Operatories.GetOpsForWebSchedNewOrExistingPatAppts().Select(x => x.OperatoryNum).ToList();
			for(int i=0;i<_listOperatories.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listOperatories[i].OpName);
				row.Cells.Add(_listOperatories[i].Abbrev);
				if(_listOperatories[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listOperatories[i].ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(_listOperatories[i].ProvDentist));
				row.Cells.Add(Providers.GetAbbr(_listOperatories[i].ProvHygienist));
				if(_listOperatories[i].IsHygiene){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(_listOperatories[i].IsWebSched?"X":"");
				row.Cells.Add(listOperatoryNumsWSNPA.Contains(_listOperatories[i].OperatoryNum) ? "X" : "");
				row.Tag=_listOperatories[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		///<summary>Returns true if there was an operatory selected.</summary>
		private bool SelectOperatory() {
			if(gridMain.GetSelectedIndex()==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return false;
			}
			OperatoryNumSelected=((Operatory)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).OperatoryNum;
			return true;
		}
		
		private void butOK_Click(object sender,EventArgs e) {
			if(!SelectOperatory()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
