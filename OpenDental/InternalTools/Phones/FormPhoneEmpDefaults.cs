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
		public bool IsSelectionMode;
		///<summary>Only used when IsSelectionMode.</summary>
		public PhoneEmpDefault PhoneEmpDefaultSelected;
		private List<PhoneEmpDefault> _listPhoneEmpDefaults;

		///<summary></summary>
		public FormPhoneEmpDefaults() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPhoneEmpDefaults_Load(object sender,EventArgs e) {
			if(Security.IsAuthorized(EnumPermType.Setup,true)) {
				butPhoneComps.Visible=true;
			}
			FillGrid();
			gridMain.SortForced(1,true);
			if(!IsSelectionMode){
				butOK.Visible=false;
			}
		}

		private void FillGrid(){
			int sortedColumnIdx=gridMain.GetSortedByColumnIdx();
			bool isSortAsc=gridMain.IsSortedAscending();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn("EmployeeNum",80,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn("EmpName",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("IsGraphed",65,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("HasColor",60,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("Queue",65);
			gridMain.Columns.Add(col);
			col=new GridColumn("PhoneExt",55,GridSortingStrategy.AmountParse);
			gridMain.Columns.Add(col);
			col=new GridColumn("StatusOverride",90);
			gridMain.Columns.Add(col);
			col=new GridColumn("Notes",250);
			gridMain.Columns.Add(col);
			col=new GridColumn("Private",50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn("Triage",50,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			_listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listPhoneEmpDefaults.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listPhoneEmpDefaults[i].EmployeeNum.ToString());
				row.Cells.Add(_listPhoneEmpDefaults[i].EmpName);
				row.Cells.Add(_listPhoneEmpDefaults[i].IsGraphed?"X":"");
				row.Cells.Add(_listPhoneEmpDefaults[i].HasColor?"X":"");
				row.Cells.Add(_listPhoneEmpDefaults[i].RingGroups.ToString());
				row.Cells.Add(_listPhoneEmpDefaults[i].PhoneExt.ToString());
				if(_listPhoneEmpDefaults[i].StatusOverride==PhoneEmpStatusOverride.None) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listPhoneEmpDefaults[i].StatusOverride.ToString());
				}
				row.Cells.Add(_listPhoneEmpDefaults[i].Notes);
				row.Cells.Add(_listPhoneEmpDefaults[i].IsPrivateScreen?"X":"");
				row.Cells.Add(_listPhoneEmpDefaults[i].IsTriageOperator?"X":"");
				row.Tag=_listPhoneEmpDefaults[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			gridMain.SortForced(sortedColumnIdx,isSortAsc);
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				PhoneEmpDefaultSelected=(PhoneEmpDefault)gridMain.ListGridRows[e.Row].Tag;
				DialogResult=DialogResult.OK;
				return;
			}
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

		private void butOK_Click(object sender,EventArgs e) {
			//only visible in IsSelectionMode
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show("Please select a row first.");
				return;
			}
			PhoneEmpDefaultSelected=(PhoneEmpDefault)gridMain.ListGridRows[idx].Tag;
			DialogResult=DialogResult.OK;
		}

	}
}