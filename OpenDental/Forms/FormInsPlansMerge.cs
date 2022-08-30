using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormInsPlansMerge:FormODBase {
		///<summary>After closing this form, if OK, then this will contain the Plan that the others will be merged into.</summary>
		public InsPlan InsPlanToMergeTo;
		///<summary>This list must be set before loading the form.  All of the PlanNums will be 0.</summary>
		public InsPlan[] InsPlanArrayAll;

		///<summary></summary>
		public FormInsPlansMerge(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsPlansMerge_Load(object sender, System.EventArgs e) {
			FillGrid();
		}

		///<summary></summary>
		private void FillGrid(){
			Cursor=Cursors.WaitCursor;
			int indexSelected=0;
			if(gridMain.SelectedIndices.Length>0) {
				indexSelected=gridMain.GetSelectedIndex();
			}
			//ListAll: Set externally before loading.
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Employer",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Carrier",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("Phone",82);
			gridMain.Columns.Add(col);
			col=new GridColumn("Address",100);
			gridMain.Columns.Add(col);
			col=new GridColumn("City",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("ST",25);
			gridMain.Columns.Add(col);
			col=new GridColumn("Zip",50);
			gridMain.Columns.Add(col);
			col=new GridColumn("Group#",70);
			gridMain.Columns.Add(col);
			col=new GridColumn("Group Name",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("Subs",40);
			gridMain.Columns.Add(col);
			col=new GridColumn("Plan Note",180);
			gridMain.Columns.Add(col);
			//TrojanID and PlanNote not shown
			gridMain.ListGridRows.Clear();
			GridRow row;
			Carrier carrier;
			for(int i=0;i<InsPlanArrayAll.Length;i++) {
				row=new GridRow();
				row.Cells.Add(Employers.GetName(InsPlanArrayAll[i].EmployerNum));
				carrier=Carriers.GetCarrier(InsPlanArrayAll[i].CarrierNum);
				row.Cells.Add(carrier.CarrierName);
				row.Cells.Add(carrier.Phone);
				row.Cells.Add(carrier.Address);
				row.Cells.Add(carrier.City);
				row.Cells.Add(carrier.State);
				row.Cells.Add(carrier.Zip);
				row.Cells.Add(InsPlanArrayAll[i].GroupNum);
				row.Cells.Add(InsPlanArrayAll[i].GroupName);
				row.Cells.Add(InsPlanArrayAll[i].NumberSubscribers.ToString());
				row.Cells.Add(InsPlanArrayAll[i].PlanNote);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(indexSelected,true);
			Cursor=Cursors.Default;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e){
			InsPlan insPlan=InsPlanArrayAll[e.Row].Copy();
			using FormInsPlan formInsPlan=new FormInsPlan(insPlan,null,null);
			//FormIP.IsForAll=true;
			//FormIP.IsReadOnly=true;
			formInsPlan.ShowDialog();
			if(formInsPlan.DialogResult==DialogResult.OK) {
				FillGrid();
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Once insurance plans have been merged, it is not possible to unmerge them. Do not merge insurance plans from multiple copies of the program at the same time. Continue?")) {
				return;
			}
			InsPlanToMergeTo=InsPlanArrayAll[gridMain.GetSelectedIndex()].Copy();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}


















