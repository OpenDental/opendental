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
		public long SelectedOperatoryNum;
		///<summary>Passed in list of operatories shown to user.</summary>
		private List<Operatory> _listOps;

		public FormOperatoryPick(List<Operatory> listOps) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listOps=listOps.Select(x=>x.Copy()).ToList();
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
			gridMain.ListGridColumns.Clear();
			int opNameWidth=180;
			int clinicWidth=85;
			if(!PrefC.HasClinicsEnabled) {
				//Clinics are hidden so add the width of the clinic column to the Op Name column because the clinic column will not show.
				opNameWidth+=clinicWidth;
			}
			GridColumn col=new GridColumn(Lan.g("TableOperatories","Op Name"),opNameWidth);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Abbrev"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHidden"),64,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableOperatories","Clinic"),clinicWidth);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableOperatories","Provider"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","Hygienist"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsHygiene"),64,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsWebSched"),74,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableOperatories","IsNewPat"),50,HorizontalAlignment.Center){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<long> listWSNPAOperatoryNums=Operatories.GetOpsForWebSchedNewOrExistingPatAppts().Select(x => x.OperatoryNum).ToList();
			foreach(Operatory opCur in _listOps) {
				row=new GridRow();
				row.Cells.Add(opCur.OpName);
				row.Cells.Add(opCur.Abbrev);
				if(opCur.IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(opCur.ClinicNum));
				}
				row.Cells.Add(Providers.GetAbbr(opCur.ProvDentist));
				row.Cells.Add(Providers.GetAbbr(opCur.ProvHygienist));
				if(opCur.IsHygiene){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				row.Cells.Add(opCur.IsWebSched?"X":"");
				row.Cells.Add(listWSNPAOperatoryNums.Contains(opCur.OperatoryNum) ? "X" : "");
				row.Tag=opCur;
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
			SelectedOperatoryNum=((Operatory)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).OperatoryNum;
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
