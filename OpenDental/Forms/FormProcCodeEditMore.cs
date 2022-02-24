using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormProcCodeEditMore:FormODBase {
		private List<Fee> _listFees;
		private ProcedureCode _procCode;
		private List<FeeSched> _listFeeScheds;
		private bool _isFeeChanged=false;

		public FormProcCodeEditMore(ProcedureCode procCode) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_procCode=procCode;
		}

		private void FormProcCodeEditMore_Load(object sender,EventArgs e) {
			_listFeeScheds=FeeScheds.GetDeepCopy(true);//js not sure why this is being used at all.  Looks like it's supposed to show all fee scheds.
			FillAndSortListFees();
			FillGrid();
		}

		///<summary></summary>
		private void FillAndSortListFees() {
			List<long> listClinicNums=Clinics.GetForUserod(Security.CurUser,true).Select(x => x.ClinicNum).ToList();
			_listFees=Fees.GetFeesForCode(_procCode.CodeNum,listClinicNums);//already sorted
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			if(!PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g("TableProcCodeEditMore","Schedule"),200);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableProcCodeEditMore","Provider"),135);
				gridMain.ListGridColumns.Add(col);
			}
			else {//Using clinics.
				col=new GridColumn(Lan.g("TableProcCodeEditMore","Schedule"),130);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableProcCodeEditMore","Clinic"),130);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g("TableProcCodeEditMore","Provider"),75);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g("TableProcCodeEditMore","Amount"),100,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			long lastFeeSched=0;
			for(int i=0;i<_listFees.Count;i++) {
				row=new GridRow();
				if(_listFees[i].FeeSched!=lastFeeSched) {
					row.Cells.Add(FeeScheds.GetDescription(_listFees[i].FeeSched));
					row.Bold=true;
					lastFeeSched=_listFees[i].FeeSched;
					row.ColorBackG=Color.LightBlue;
					if(_listFees[i].ClinicNum!=0 || _listFees[i].ProvNum!=0) { //FeeSched change, but not with a default fee. Insert placeholder row.
						if(PrefC.HasClinicsEnabled) {
							row.Cells.Add("");
						}
						row.Cells.Add("");
						row.Cells.Add("");
						Fee fee=new Fee();
						fee.FeeSched=_listFees[i].FeeSched;
						row.Tag=fee;
						gridMain.ListGridRows.Add(row);
						//Now that we have a placeholder for the default fee (none was found), go about adding the next row (non-default fee).
						row=new GridRow();
						row.Cells.Add("");
					}
				}
				else {
					row.Cells.Add("");
				}
				row.Tag=_listFees[i];
				if(PrefC.HasClinicsEnabled) { //Using clinics
					row.Cells.Add(Clinics.GetAbbr(_listFees[i].ClinicNum)); //Returns "" if invalid clinicnum (ie. 0)
				}
				row.Cells.Add(Providers.GetAbbr(_listFees[i].ProvNum)); //Returns "" if invalid provnum (ie. 0)
				row.Cells.Add(_listFees[i].Amount.ToString("n"));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,UI.ODGridClickEventArgs e) {
			Fee fee=(Fee)gridMain.ListGridRows[e.Row].Tag;
			using FormFeeEdit FormFE=new FormFeeEdit();
			if(fee.FeeNum==0) {
				FormFE.IsNew=true;
				fee.CodeNum=_procCode.CodeNum;
				Fees.Insert(fee);//Pre-insert the fee before opening the edit window.
			}
			FormFE.FeeCur=fee;
			FormFE.ShowDialog();
			if(FormFE.DialogResult==DialogResult.OK) {
				//FormFE could have manipulated the fee.  Refresh our local cache and grids to reflect the changes.
				FillAndSortListFees();
				FillGrid();
				_isFeeChanged=true;
			}
		}

		private void butClose_Click(object sender,EventArgs e) {
			if(_isFeeChanged) {
				DialogResult=DialogResult.OK;
			}
			else {
				DialogResult=DialogResult.Cancel;
			}
		}

	}
}