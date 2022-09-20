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
	public partial class FormAdjustmentPicker:FormODBase {
		private bool _isUnattachedMode;
		private long _patNum;
		private long _clinicNum;
		private long _provNum;
		private List<Adjustment> _listAdjustments;
		private List<Adjustment> _listAdjustmentsFiltered;
		public Adjustment AdjustmentSelected;

		public FormAdjustmentPicker(long patNum,bool isUnattachedMode=false,List<Adjustment> listAdjustments=null,long clinicNum=-1,long provNum=0) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_patNum=patNum;
			_clinicNum=clinicNum;
			_provNum=provNum;
			_isUnattachedMode=isUnattachedMode;
			_listAdjustments=listAdjustments;
		}

		private void FormAdjustmentPicker_Load(object sender,EventArgs e) {
			if(_isUnattachedMode) {
				checkUnattached.Checked=true;
				checkUnattached.Enabled=false;
			}
			if(_listAdjustments==null) {
				_listAdjustments=Adjustments.Refresh(_patNum).ToList();
			}
			//Because this window is only opened when linking adjustments, and we cannot link those associated with a paysplit, we filter those out.
			List<PaySplit> listPaySplits=PaySplits.GetForAdjustments(_listAdjustments.Select(x => x.AdjNum).ToList());
			_listAdjustments.RemoveAll(x => listPaySplits.Exists(y => y.AdjNum==x.AdjNum));
			//Because this window is only opened when linking adjustments, and we cannot link those associated with a payplan, we filter those out.
			List<long> listPayPlanAdjNums=PayPlanLinks.GetListForLinkTypeAndFKeys(PayPlanLinkType.Adjustment,_listAdjustments.Select(x => x.AdjNum).ToList());
			_listAdjustments.RemoveAll(x => listPayPlanAdjNums.Contains(x.AdjNum));
			FillGrid();
		}

		private void FillGrid(){
			_listAdjustmentsFiltered=_listAdjustments;
			if(checkUnattached.Checked) {
				_listAdjustmentsFiltered=_listAdjustments.FindAll(x => x.ProcNum==0);
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),75);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"PatNum"),65);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Provider"),75);
			gridMain.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.Columns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Type"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Amount"),100);
			gridMain.Columns.Add(col);
			if(!_isUnattachedMode) { 
				col=new GridColumn(Lan.g(this,"Has Proc"),0,HorizontalAlignment.Center);
				gridMain.Columns.Add(col);
			}
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listAdjustmentsFiltered.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listAdjustmentsFiltered[i].AdjDate.ToShortDateString());
				row.Cells.Add(_listAdjustmentsFiltered[i].PatNum.ToString());
				row.Cells.Add(Providers.GetAbbr(_listAdjustmentsFiltered[i].ProvNum));
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(Clinics.GetAbbr(_listAdjustmentsFiltered[i].ClinicNum));
				}
				row.Cells.Add(Defs.GetName(DefCat.AdjTypes,_listAdjustmentsFiltered[i].AdjType));
				row.Cells.Add(_listAdjustmentsFiltered[i].AdjAmt.ToString("F"));
				string attachedProc="";
				if(_listAdjustmentsFiltered[i].ProcNum!=0){
					attachedProc="X";
				}
				if(!_isUnattachedMode) { 
					row.Cells.Add(attachedProc);
				}
				row.Tag=_listAdjustmentsFiltered[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private bool IsSameProvClinic(Adjustment adjustment) {
			if(_provNum==0 && _clinicNum==-1) {
				return true; //calling method for form didn't pass in the values, so we don't care about checking them
			}
			if(adjustment.ProvNum==_provNum && _clinicNum == -1) {
				return true;
			}
			return adjustment.ProvNum==_provNum && adjustment.ClinicNum==_clinicNum;

		}

		private void checkUnattached_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			AdjustmentSelected=_listAdjustmentsFiltered[gridMain.GetSelectedIndex()];
			if(!IsSameProvClinic(AdjustmentSelected)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected adjustment's provider and/or clinic do not match the procedure. Would you like to continue?")) {
					return;
				}
			}
				DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			int index=gridMain.GetSelectedIndex();
			if(index<0) {
				MsgBox.Show("You must select an adjustment.");
				return;
			}
			AdjustmentSelected=_listAdjustmentsFiltered[index];
			if(!IsSameProvClinic(AdjustmentSelected)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"The selected adjustment's provider and/or clinic do not match the procedure. Would you like to continue?")) {
					return;
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}