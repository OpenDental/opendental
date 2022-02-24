using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormEmployeeSelect : FormODBase {
		//private ArrayList ALemployees;
		private bool isChanged;
		///<summary>Unfiltered.</summary>
		private List<Employee> _listEmployeesFull;
		///<summary></summary>
		private List<Employee> _listEmployeesShowing;

		///<summary></summary>
		public FormEmployeeSelect(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEmployeeSelect_Load(object sender, System.EventArgs e) {
			RefreshList();
			FillGrid();
			Height=System.Windows.Forms.Screen.FromControl(this).WorkingArea.Height-2;
			Top=2;
		}

		private void RefreshList(){
			Employees.RefreshCache();
			_listEmployeesFull=Employees.GetDeepCopy();
		}

		private void FillGrid(){
			_listEmployeesShowing=new List<Employee>();
			for(int i=0;i<_listEmployeesFull.Count;i++){
				if(textSearch.Text!=""){
					if(!_listEmployeesFull[i].LName.ToLower().Contains(textSearch.Text.ToLower())
						&& !_listEmployeesFull[i].FName.ToLower().Contains(textSearch.Text.ToLower())
						//&&!_listEmployeesFull[i].EmployeeNum.ToString().Contains(textSearch.Text)
						&&!_listEmployeesFull[i].EmailWork.ToLower().Contains(textSearch.Text.ToLower())
						&&!_listEmployeesFull[i].EmailPersonal.ToLower().Contains(textSearch.Text.ToLower())
						&&!_listEmployeesFull[i].WirelessPhone.Replace("-","").Replace("(","").Replace(")","").Contains(textSearch.Text))
					{
						continue;
					}
				}
				if(!checkHidden.Checked && _listEmployeesFull[i].IsHidden){
					continue;
				}
				if(!checkFurloughed.Checked && _listEmployeesFull[i].IsFurloughed){
					continue;
				}
				if(!checkNonFurloughed.Checked && !_listEmployeesFull[i].IsFurloughed){
					continue;
				}
				if(!checkWorkingHome.Checked && _listEmployeesFull[i].IsWorkingHome){
					continue;
				}
				if(!checkWorkingOffice.Checked && !_listEmployeesFull[i].IsWorkingHome){
					continue;
				}
				_listEmployeesShowing.Add(_listEmployeesFull[i]);
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("FormEmployeeSelect","FName"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","LName"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","MI"),30);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Hid"),30,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Reports To"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Wireless"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Email Work"),220);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Email Personal"),220);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Furlo"),35,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("FormEmployeeSelect","Home"),30,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEmployeesShowing.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listEmployeesShowing[i].FName);
				row.Cells.Add(_listEmployeesShowing[i].LName);
				row.Cells.Add(_listEmployeesShowing[i].MiddleI);
				if(_listEmployeesShowing[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				Employee employee=Employees.GetEmp(_listEmployeesShowing[i].ReportsTo);
				if(employee is null){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(employee.FName);
				}
				row.Cells.Add(_listEmployeesShowing[i].WirelessPhone);
				row.Cells.Add(_listEmployeesShowing[i].EmailWork);
				row.Cells.Add(_listEmployeesShowing[i].EmailPersonal);
				row.Cells.Add(_listEmployeesShowing[i].IsFurloughed?"X":"");
				row.Cells.Add(_listEmployeesShowing[i].IsWorkingHome?"X":"");
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void textSearch_KeyUp(object sender, KeyEventArgs e){
			FillGrid();
		}

		private void checkHidden_CheckedChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void checkFurloughed_CheckedChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void checkNonFurloughed_CheckedChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void checkWorkingHome_CheckedChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void checkWorkingOffice_CheckedChanged(object sender, EventArgs e){
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormEmployeeEdit formEmployeeEdit=new FormEmployeeEdit();
			formEmployeeEdit.EmployeeCur=new Employee();
			formEmployeeEdit.IsNew=true;
			formEmployeeEdit.ShowDialog();
			if(formEmployeeEdit.DialogResult!=DialogResult.OK){
				return;
			}
			RefreshList();
			FillGrid();
			isChanged=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			long empNum=_listEmployeesShowing[e.Row].EmployeeNum;
			using FormEmployeeEdit formEmployeeEdit=new FormEmployeeEdit();
			formEmployeeEdit.EmployeeCur=_listEmployeesShowing[e.Row];
			formEmployeeEdit.ShowDialog();
			if(formEmployeeEdit.DialogResult!=DialogResult.OK){
				return;
			}
			RefreshList();
			FillGrid();
			isChanged=true;
			for(int i=0;i<_listEmployeesShowing.Count;i++){
				if(_listEmployeesShowing[i].EmployeeNum==empNum){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Schedules may be lost.  Continue?")){
				return;
			}
			for(int i=0;i<_listEmployeesShowing.Count;i++){
				try{
					Employees.Delete(_listEmployeesShowing[i].EmployeeNum);
				}
				catch{}
			}
			RefreshList();
			FillGrid();
		}

		private void butExport_Click(object sender, EventArgs e){
			gridMain.Export("Employees");
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			this.Close();
		}

		private void FormEmployee_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(isChanged){
				DataValid.SetInvalid(InvalidType.Employees);
			}
		}

	
	}
}
