using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormPhoneEmpDefaults:FormODBase {
		private List<PhoneEmpDefault> ListPED;

		///<summary></summary>
		public FormPhoneEmpDefaults()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAccountPick_Load(object sender,EventArgs e) {
			if(Security.IsAuthorized(Permissions.Setup,true)) {
				butPhoneComps.Visible=true;
			}
			FillGrid();
			gridMain.SortForced(1,true);
		}

		private void FillGrid(){
			int sortedColumnIdx=gridMain.SortedByColumnIdx;
      bool isSortAsc=gridMain.SortedIsAscending;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn("EmployeeNum",80,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("EmpName",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("IsGraphed",65,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("HasColor",60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Queue",65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("PhoneExt",55,GridSortingStrategy.AmountParse);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("StatusOverride",90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Notes",250);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Private",50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Triage",50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			ListPED=PhoneEmpDefaults.GetDeepCopy();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ListPED.Count;i++){
				row=new GridRow();
				row.Cells.Add(ListPED[i].EmployeeNum.ToString());
				row.Cells.Add(ListPED[i].EmpName);
				row.Cells.Add(ListPED[i].IsGraphed?"X":"");
				row.Cells.Add(ListPED[i].HasColor?"X":"");
				row.Cells.Add(ListPED[i].RingGroups.ToString());
				row.Cells.Add(ListPED[i].PhoneExt.ToString());
				if(ListPED[i].StatusOverride==PhoneEmpStatusOverride.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(ListPED[i].StatusOverride.ToString());
				}
				row.Cells.Add(ListPED[i].Notes);
				row.Cells.Add(ListPED[i].IsPrivateScreen?"X":"");
				row.Cells.Add(ListPED[i].IsTriageOperator?"X":"");
				row.Tag=ListPED[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SortForced(sortedColumnIdx,isSortAsc);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormPhoneEmpDefaultEdit FormPED=new FormPhoneEmpDefaultEdit();
			FormPED.PedCur=(PhoneEmpDefault)gridMain.ListGridRows[e.Row].Tag;
			FormPED.ShowDialog();
			FillGrid();
		}

		private void butPhoneComps_Click(object sender,EventArgs e) {
			using FormPhoneComps FormPC=new FormPhoneComps();
			FormPC.ShowDialog();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaultEdit FormPED=new FormPhoneEmpDefaultEdit();
			FormPED.PedCur=new PhoneEmpDefault();
			FormPED.IsNew=true;
			FormPED.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		

		

		

	

		

		

	


	}
}





















