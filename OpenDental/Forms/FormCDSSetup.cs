using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Xml;
using System.Threading;
using System.Net;
using System.IO;
using Ionic.Zip;
using CodeBase;
using OpenDental.UI;

namespace OpenDental {
	public partial class FormCDSSetup:FormODBase {
		private List<CDSPermission> _listCDSPermissions;
		private List<CDSPermission> _listCDSPermissionsOld;

		public FormCDSSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCDSSetup_Load(object sender,EventArgs e) {
			_listCDSPermissions=CDSPermissions.GetAll();
			_listCDSPermissionsOld=CDSPermissions.GetAll();
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col<2) {
				return;//name and group name columns.
			}
			for(int i=0;i<_listCDSPermissions.Count;i++) {
				if(_listCDSPermissions[i].CDSPermissionNum!=(long)gridMain.ListGridRows[e.Row].Tag) {
					continue;
				}
				switch(e.Col){
					case 0:
					case 1:
						//should never happen.
						break;
					case 2:
						_listCDSPermissions[i].ShowCDS=!_listCDSPermissions[i].ShowCDS;
						break;
					case 3:
						_listCDSPermissions[i].SetupCDS=!_listCDSPermissions[i].SetupCDS;
						break;
					case 4:
						_listCDSPermissions[i].ShowInfobutton=!_listCDSPermissions[i].ShowInfobutton;
						break;
					case 5:
						_listCDSPermissions[i].EditBibliography=!_listCDSPermissions[i].EditBibliography;
						break;
					case 6:
						_listCDSPermissions[i].ProblemCDS=!_listCDSPermissions[i].ProblemCDS;
						break;
					case 7:
						_listCDSPermissions[i].MedicationCDS=!_listCDSPermissions[i].MedicationCDS;
						break;
					case 8:
						_listCDSPermissions[i].AllergyCDS=!_listCDSPermissions[i].AllergyCDS;
						break;
					case 9:
						_listCDSPermissions[i].DemographicCDS=!_listCDSPermissions[i].DemographicCDS;
						break;
					case 10:
						_listCDSPermissions[i].LabTestCDS=!_listCDSPermissions[i].LabTestCDS;
						break;
					case 11:
						_listCDSPermissions[i].VitalCDS=!_listCDSPermissions[i].VitalCDS;
						break;
					default:
						//should never happen.
						break;
				}
				break;
			}
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn column;
			column=new GridColumn("User Name",120);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Group Name",120);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Show CDS",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Show i",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Edit CDS",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Source",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Problem",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Medication",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Allergy",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Demographic",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Labs",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn("Vitals",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(column);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<Userod> listUserods=Userods.GetDeepCopy(true);
			UserGroup[] userGroupArray=UserGroups.GetDeepCopy().ToArray();
			//if(radioUser.Checked) {//by user
			for(int i=0;i<listUserods.Count;i++) {
				row=new GridRow();
				row.Cells.Add(listUserods[i].UserName);
				for(int g=0;g<userGroupArray.Length;g++) {//group name.
					if(!listUserods[i].IsInUserGroup(userGroupArray[g].UserGroupNum)) {
						continue;
					}
					row.Cells.Add(userGroupArray[g].Description);
					break;
				}
				for(int p=0;p<_listCDSPermissions.Count;p++) {
					if(listUserods[i].UserNum!=_listCDSPermissions[p].UserNum) {
						continue;
					}
					row.Cells.Add((_listCDSPermissions[p].ShowCDS						?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].SetupCDS					?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].ShowInfobutton		?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].EditBibliography	?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].ProblemCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].MedicationCDS			?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].AllergyCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].DemographicCDS		?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].LabTestCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCDSPermissions[p].VitalCDS					?"X":""));//"X" if user has permission
					row.Tag=_listCDSPermissions[p].CDSPermissionNum;//used to edit correct permission.
					break;
				}
				gridMain.ListGridRows.Add(row);
			}
			//}
			//else {//by user group
			//	for(int g=0;g<ArrayGroups.Length;g++) {
			//		row=new ODGridRow();
			//		row.Cells.Add("");//No User Name
			//		row.Cells.Add(ArrayGroups[g].Description);
						//TODO: Later. No time now for group level permission editing.
			//		gridMain.Rows.Add(row);
			//	}
			//}
			gridMain.EndUpdate();
		}

		private void butOk_Click(object sender,EventArgs e) {
			for(int i=0;i<_listCDSPermissions.Count;i++) {
				//TODO:instead of updating all permissions. Update only the permissions neccesary.
				if(_listCDSPermissions[i].UserNum!=_listCDSPermissionsOld[i].UserNum) {
					if(ODBuild.IsDebug()) {
							throw new Exception("If this ever happens, something went wrong. We can explicitly loop through both lists and match patnums.");
						}
					else {
						continue;//should never happen, but userNums were mismatched.
					}
				}
				if(_listCDSPermissions[i].SetupCDS==_listCDSPermissionsOld[i].SetupCDS
					&& _listCDSPermissions[i].ShowCDS==_listCDSPermissionsOld[i].ShowCDS
					&& _listCDSPermissions[i].ShowInfobutton==_listCDSPermissionsOld[i].ShowInfobutton
					&& _listCDSPermissions[i].EditBibliography==_listCDSPermissionsOld[i].EditBibliography
					&& _listCDSPermissions[i].ProblemCDS==_listCDSPermissionsOld[i].ProblemCDS
					&& _listCDSPermissions[i].MedicationCDS==_listCDSPermissionsOld[i].MedicationCDS
					&& _listCDSPermissions[i].AllergyCDS==_listCDSPermissionsOld[i].AllergyCDS
					&& _listCDSPermissions[i].DemographicCDS==_listCDSPermissionsOld[i].DemographicCDS
					&& _listCDSPermissions[i].LabTestCDS==_listCDSPermissionsOld[i].LabTestCDS
					&& _listCDSPermissions[i].VitalCDS==_listCDSPermissionsOld[i].VitalCDS) 
				{
					continue;//nothing to change.
				}
				CDSPermissions.Update(_listCDSPermissions[i]);
				//The following line of code should never be re-ordered, only added to if needed.  Otherwise historical security logs may not be enterpreted correctly.
				string cdsLog="CDSPermChanged,U:"+_listCDSPermissions[i].UserNum+","
					+(_listCDSPermissions[i].SetupCDS						?"T":"F")
					+(_listCDSPermissions[i].ShowCDS						?"T":"F")
					+(_listCDSPermissions[i].ShowInfobutton			?"T":"F")
					+(_listCDSPermissions[i].EditBibliography		?"T":"F")
					+(_listCDSPermissions[i].ProblemCDS					?"T":"F")
					+(_listCDSPermissions[i].MedicationCDS			?"T":"F")
					+(_listCDSPermissions[i].AllergyCDS					?"T":"F")
					+(_listCDSPermissions[i].DemographicCDS			?"T":"F")
					+(_listCDSPermissions[i].LabTestCDS					?"T":"F")
					+(_listCDSPermissions[i].VitalCDS						?"T":"F")
					;
				SecurityLogs.MakeLogEntry(Permissions.SecurityAdmin,0,cdsLog);//Log entry example: CDSPermChanged,33,TTTFFFFFF
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}


		

	}
}