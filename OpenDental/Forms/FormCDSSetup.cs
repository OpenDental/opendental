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
		private List<CDSPermission> _listCdsPermissions;
		private List<CDSPermission> _listCdsPermissionsOld;

		public FormCDSSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormCDSSetup_Load(object sender,EventArgs e) {
			_listCdsPermissions=CDSPermissions.GetAll();
			_listCdsPermissionsOld=CDSPermissions.GetAll();
			FillGrid();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(e.Col<2) {
				return;//name and group name columns.
			}
			for(int i=0;i<_listCdsPermissions.Count;i++) {
				if(_listCdsPermissions[i].CDSPermissionNum!=(long)gridMain.ListGridRows[e.Row].Tag) {
					continue;
				}
				switch(e.Col){
					case 0:
					case 1:
						//should never happen.
						break;
					case 2:
						_listCdsPermissions[i].ShowCDS=!_listCdsPermissions[i].ShowCDS;
						break;
					case 3:
						_listCdsPermissions[i].SetupCDS=!_listCdsPermissions[i].SetupCDS;
						break;
					case 4:
						_listCdsPermissions[i].ShowInfobutton=!_listCdsPermissions[i].ShowInfobutton;
						break;
					case 5:
						_listCdsPermissions[i].EditBibliography=!_listCdsPermissions[i].EditBibliography;
						break;
					case 6:
						_listCdsPermissions[i].ProblemCDS=!_listCdsPermissions[i].ProblemCDS;
						break;
					case 7:
						_listCdsPermissions[i].MedicationCDS=!_listCdsPermissions[i].MedicationCDS;
						break;
					case 8:
						_listCdsPermissions[i].AllergyCDS=!_listCdsPermissions[i].AllergyCDS;
						break;
					case 9:
						_listCdsPermissions[i].DemographicCDS=!_listCdsPermissions[i].DemographicCDS;
						break;
					case 10:
						_listCdsPermissions[i].LabTestCDS=!_listCdsPermissions[i].LabTestCDS;
						break;
					case 11:
						_listCdsPermissions[i].VitalCDS=!_listCdsPermissions[i].VitalCDS;
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
			GridColumn col;
			col=new GridColumn("User Name",120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Group Name",120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Show CDS",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Show i",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Edit CDS",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Source",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Problem",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Medication",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Allergy",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Demographic",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Labs",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Vitals",80,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			List<Userod> ListUsers=Userods.GetDeepCopy(true);
			UserGroup[] ArrayGroups=UserGroups.GetDeepCopy().ToArray();
			//if(radioUser.Checked) {//by user
			for(int i=0;i<ListUsers.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ListUsers[i].UserName);
				for(int g=0;g<ArrayGroups.Length;g++) {//group name.
					if(!ListUsers[i].IsInUserGroup(ArrayGroups[g].UserGroupNum)) {
						continue;
					}
					row.Cells.Add(ArrayGroups[g].Description);
					break;
				}
				for(int p=0;p<_listCdsPermissions.Count;p++) {
					if(ListUsers[i].UserNum!=_listCdsPermissions[p].UserNum) {
						continue;
					}
					row.Cells.Add((_listCdsPermissions[p].ShowCDS						?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].SetupCDS					?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].ShowInfobutton		?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].EditBibliography	?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].ProblemCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].MedicationCDS			?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].AllergyCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].DemographicCDS		?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].LabTestCDS				?"X":""));//"X" if user has permission
					row.Cells.Add((_listCdsPermissions[p].VitalCDS					?"X":""));//"X" if user has permission
					row.Tag=_listCdsPermissions[p].CDSPermissionNum;//used to edit correct permission.
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
			for(int i=0;i<_listCdsPermissions.Count;i++) {
				//TODO:instead of updating all permissions. Update only the permissions neccesary.
				if(_listCdsPermissions[i].UserNum!=_listCdsPermissionsOld[i].UserNum) {
					if(ODBuild.IsDebug()) {
							throw new Exception("If this ever happens, something went wrong. We can explicitly loop through both lists and match patnums.");
						}
					else {
						continue;//should never happen, but userNums were mismatched.
					}
				}
				if(_listCdsPermissions[i].SetupCDS==_listCdsPermissionsOld[i].SetupCDS
					&& _listCdsPermissions[i].ShowCDS==_listCdsPermissionsOld[i].ShowCDS
					&& _listCdsPermissions[i].ShowInfobutton==_listCdsPermissionsOld[i].ShowInfobutton
					&& _listCdsPermissions[i].EditBibliography==_listCdsPermissionsOld[i].EditBibliography
					&& _listCdsPermissions[i].ProblemCDS==_listCdsPermissionsOld[i].ProblemCDS
					&& _listCdsPermissions[i].MedicationCDS==_listCdsPermissionsOld[i].MedicationCDS
					&& _listCdsPermissions[i].AllergyCDS==_listCdsPermissionsOld[i].AllergyCDS
					&& _listCdsPermissions[i].DemographicCDS==_listCdsPermissionsOld[i].DemographicCDS
					&& _listCdsPermissions[i].LabTestCDS==_listCdsPermissionsOld[i].LabTestCDS
					&& _listCdsPermissions[i].VitalCDS==_listCdsPermissionsOld[i].VitalCDS) 
				{
					continue;//nothing to change.
				}
				CDSPermissions.Update(_listCdsPermissions[i]);
				//The following line of code should never be re-ordered, only added to if needed.  Otherwise historical security logs may not be enterpreted correctly.
				string cdsLog="CDSPermChanged,U:"+_listCdsPermissions[i].UserNum+","
					+(_listCdsPermissions[i].SetupCDS						?"T":"F")
					+(_listCdsPermissions[i].ShowCDS						?"T":"F")
					+(_listCdsPermissions[i].ShowInfobutton			?"T":"F")
					+(_listCdsPermissions[i].EditBibliography		?"T":"F")
					+(_listCdsPermissions[i].ProblemCDS					?"T":"F")
					+(_listCdsPermissions[i].MedicationCDS			?"T":"F")
					+(_listCdsPermissions[i].AllergyCDS					?"T":"F")
					+(_listCdsPermissions[i].DemographicCDS			?"T":"F")
					+(_listCdsPermissions[i].LabTestCDS					?"T":"F")
					+(_listCdsPermissions[i].VitalCDS						?"T":"F")
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