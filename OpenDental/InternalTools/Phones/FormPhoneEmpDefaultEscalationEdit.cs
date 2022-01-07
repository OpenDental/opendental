using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormPhoneEmpDefaultEscalationEdit:FormODBase {

		///<summary>Master list of employees. Only get this once.</summary>
		List<PhoneEmpDefault> _listPED;
		///<summary>Original dictinary of subgroup types and their list of employees when form opened.</summary>
		private Dictionary<PhoneEmpSubGroupType,List<PhoneEmpSubGroup>> _dictSubGroupsOld;
		///<summary>On load this becomes a copy of _dictSubGroupsOld. Then as we make changes this is updated and used to sync with _dictSubGroupsOld.</summary>
		private Dictionary<PhoneEmpSubGroupType,List<PhoneEmpSubGroup>> _dictSubGroupsNew;

		public FormPhoneEmpDefaultEscalationEdit(PhoneEmpSubGroupType tabDefault=PhoneEmpSubGroupType.Avail) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			FillTabs();
			List<PhoneEmpSubGroup> listGroupsAll = PhoneEmpSubGroups.GetAll();
			_dictSubGroupsOld=Enum.GetValues(typeof(PhoneEmpSubGroupType)).Cast<PhoneEmpSubGroupType>().ToDictionary(x=>x,x=>listGroupsAll.FindAll(y=>y.SubGroupType==x));
			_dictSubGroupsNew=_dictSubGroupsOld.ToDictionary(x=>x.Key,x=>x.Value.Select(y=>y.Copy()).ToList());
			//Get all employees.
			_listPED=PhoneEmpDefaults.GetDeepCopy();
			//Sort by name.
			_listPED.Sort(new PhoneEmpDefaults.PhoneEmpDefaultComparer(PhoneEmpDefaults.PhoneEmpDefaultComparer.SortBy.name));
			FillGrids();
			//This can change to a switch statement on tabDefault if we ever add custom named tabs.
			tabMain.SelectTab(tabMain.TabPages[tabDefault.ToString()]);
		}
		
		///<summary>Clears tabs and populates with values from enum PhoneEmpSubGroupType.</summary>
		private void FillTabs() {
			TabPage tabPage;
			foreach(TabPage tab in tabMain.TabPages) {//Equivalent to tabMain.TabPages.Clear()
				LayoutManager.Remove(tab);
				tab.Dispose();
			}
			tabPage=new TabPage(PhoneEmpSubGroupType.Avail.ToString());
			tabPage.Name=PhoneEmpSubGroupType.Avail.ToString();
			tabPage.Tag=PhoneEmpSubGroupType.Avail;
			LayoutManager.Add(tabPage,tabMain);
			tabMain.TabPages[PhoneEmpSubGroupType.Avail.ToString()].Tag=PhoneEmpSubGroupType.Avail;
			foreach(PhoneEmpSubGroupType e in Enum.GetValues(typeof(PhoneEmpSubGroupType))) {
				if(e==PhoneEmpSubGroupType.Avail) {
					continue;//Already added above
				}
				tabPage=new TabPage(e.ToString());
				tabPage.Name=e.ToString();
				tabPage.Tag=e;
				LayoutManager.Add(tabPage,tabMain);
			}
			//Disable the Up and Down buttons when Avail is selected.
			butUp.Enabled=false;
			butDown.Enabled=false;
		}

		///<summary>Fills both grids for currently selected tab.</summary>
		private void FillGrids() {
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			List<PhoneEmpSubGroup> listEsc=_dictSubGroupsNew[typeCur];
			listEsc=listEsc.OrderBy(x => x.EscalationOrder).ToList();
			//Fill escalation grid.
			gridEscalation.BeginUpdate();
			gridEscalation.ListGridColumns.Clear();
			gridEscalation.ListGridColumns.Add(new GridColumn("Employee",gridEscalation.Width));
			gridEscalation.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEsc.Count;i++) {
				Employee empCur=Employees.GetEmp(listEsc[i].EmployeeNum);
				if(empCur==null) {
					continue;
				}
				row=new GridRow(empCur.FName+(empCur.IsHidden?"(hidden)":""));
				row.Tag=_listPED.FirstOrDefault(x => x.EmployeeNum==listEsc[i].EmployeeNum);//can be null
				if(row.Tag==null) {
					row.Tag=_dictSubGroupsNew[typeCur].IndexOf(listEsc[i]);//Since we .OrderBy(...) only in FillGrid, pass the index to the tag.
				}
				gridEscalation.ListGridRows.Add(row);
				//Set escalation order for this employee.
				//Must happen after the add in order to keep the Escalation order 1-based.
				listEsc[i].EscalationOrder=gridEscalation.ListGridRows.Count;
			}
			gridEscalation.EndUpdate();
			//Fill employee grid.
			gridEmployees.BeginUpdate();
			gridEmployees.ListGridColumns.Clear();
			gridEmployees.ListGridColumns.Add(new GridColumn("Employee",gridEmployees.Width));
			gridEmployees.ListGridRows.Clear();
			for(int i=0;i<_listPED.Count;i++) {
				row=new GridRow();
				//Omit employee who are already included in escalation grid.
				if(listEsc.Any(x => x.EmployeeNum==_listPED[i].EmployeeNum)) {
					continue;
				}
				row.Cells.Add(_listPED[i].EmpName.ToString());
				row.Tag=_listPED[i];
				gridEmployees.ListGridRows.Add(row);
			}
			gridEmployees.EndUpdate();	
		}
		
		private void tabMain_SelectedIndexChanged(object sender,EventArgs e) {
			if(!tabMain.TabPages.ContainsKey(PhoneEmpSubGroupType.Escal.ToString())) {//Control has not been initialized.
				return;
			}
			gridEmployees.ScrollValue=0;//scroll to top
			gridEscalation.ScrollValue=0;//scroll to top
			FillGrids();
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			if(typeCur==PhoneEmpSubGroupType.Avail) {
				butUp.Enabled=false;
				butDown.Enabled=false;
				labelAvail.Visible=true;
			}
			else {
				butUp.Enabled=true;
				butDown.Enabled=true;
				labelAvail.Visible=false;
			}
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(gridEmployees.SelectedIndices.Length<=0) {
				return;
			}
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			foreach(int i in gridEmployees.SelectedIndices) {
				PhoneEmpDefault pedKeep=(PhoneEmpDefault)gridEmployees.ListGridRows[i].Tag;
				_dictSubGroupsNew[typeCur].Add(new PhoneEmpSubGroup(pedKeep.EmployeeNum,typeCur,0));
			}
			FillGrids();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(gridEscalation.SelectedIndices.Length<=0) {
				return;
			}
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			foreach(int i in gridEscalation.SelectedIndices) {
				if(gridEscalation.ListGridRows[i].Tag is PhoneEmpDefault) {
					PhoneEmpDefault pedCur=(PhoneEmpDefault)gridEscalation.ListGridRows[i].Tag;
					_dictSubGroupsNew[typeCur].RemoveAll(x => x.EmployeeNum==pedCur.EmployeeNum);
				}
				else {//Tag is an index (int)
					int index=(int)gridEscalation.ListGridRows[i].Tag;
					_dictSubGroupsNew[typeCur].RemoveAt(index);
				}
			}
			FillGrids();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridEscalation.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Select 1 item from escalation list");
				return;
			}
			if(gridEscalation.SelectedIndices[0]==0) {
				return;
			}
			//Retain current selection.
			int curSelectedIndex=Math.Max(gridEscalation.SelectedIndices[0]-1,0);
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			List<int> selectedIndices=new List<int>(gridEscalation.SelectedIndices);
			for(int i=0;i<gridEscalation.ListGridRows.Count;i++) {
				if(gridEscalation.ListGridRows[i].Tag is PhoneEmpDefault) {
					PhoneEmpDefault ped=(PhoneEmpDefault)gridEscalation.ListGridRows[i].Tag;
					if(selectedIndices[0]==i+1) {
						//First should be safe to use here because it must exist in gridescalation to get here.
						_dictSubGroupsNew[typeCur].First(x => x.EmployeeNum==ped.EmployeeNum).EscalationOrder++;
					}
					else if(selectedIndices[0]==i) {
						_dictSubGroupsNew[typeCur].First(x => x.EmployeeNum==ped.EmployeeNum).EscalationOrder--;
					}
				}
				else {//Tag is an index (int)
					int index=(int)gridEscalation.ListGridRows[i].Tag;
					if(selectedIndices[0]==i+1) {
						//First should be safe to use here because it must exist in gridescalation to get here.
						_dictSubGroupsNew[typeCur][index].EscalationOrder++;
					}
					else if(selectedIndices[0]==i) {
						_dictSubGroupsNew[typeCur][index].EscalationOrder--;
					}
				}
			}
			FillGrids();
			//Reset selection so moving up the list rapidly is easier.
			gridEscalation.SetSelected(curSelectedIndex,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridEscalation.SelectedIndices.Length!=1) {
				MsgBox.Show(this,"Select 1 item from escalation list");
				return;
			}
			if(gridEscalation.SelectedIndices[0]>=(gridEscalation.ListGridRows.Count-1)) {
				return;
			}
			//Retain current selection.
			int curSelectedIndex=Math.Min(gridEscalation.SelectedIndices[0]+1,(gridEscalation.ListGridRows.Count-1));
			PhoneEmpSubGroupType typeCur=(PhoneEmpSubGroupType)tabMain.TabPages[tabMain.SelectedIndex].Tag;
			List<int> selectedIndices=new List<int>(gridEscalation.SelectedIndices);
			for(int i=0;i<gridEscalation.ListGridRows.Count;i++) {
				if(gridEscalation.ListGridRows[i].Tag is PhoneEmpDefault) {
					PhoneEmpDefault ped=(PhoneEmpDefault)gridEscalation.ListGridRows[i].Tag;
					if(selectedIndices[0]==i) {
						_dictSubGroupsNew[typeCur].First(x => x.EmployeeNum==ped.EmployeeNum).EscalationOrder++;
					}
					else if(selectedIndices[0]==i-1) {
						_dictSubGroupsNew[typeCur].First(x => x.EmployeeNum==ped.EmployeeNum).EscalationOrder--;
					}
				}
				else {//Tag is an index (int)
					int index=(int)gridEscalation.ListGridRows[i].Tag;
					if(selectedIndices[0]==i) {
						//First should be safe to use here because it must exist in gridescalation to get here.
						_dictSubGroupsNew[typeCur][index].EscalationOrder++;
					}
					else if(selectedIndices[0]==i-1) {
						_dictSubGroupsNew[typeCur][index].EscalationOrder--;
					}
				}
			}
			FillGrids();
			//Reset selection so moving down the list rapidly is easier.
			gridEscalation.SetSelected(curSelectedIndex,true);
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			PhoneEmpSubGroups.Sync(_dictSubGroupsNew.SelectMany(x=>x.Value).ToList(),_dictSubGroupsOld.SelectMany(x=>x.Value).ToList());
				DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			this.DialogResult=DialogResult.Cancel;
		}

		private void gridEmployees_CellDoubleClick(object sender,ODGridClickEventArgs e) {

		}
	}
}
