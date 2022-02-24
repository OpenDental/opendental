using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental.User_Controls {
	public partial class UserControlReportSetup:UserControl {
		public List<DisplayReport> ListDisplayReportAll;
		public List<GroupPermission> ListGroupPermissionsForReports;
		public List<GroupPermission> ListGroupPermissionsOld;
		private List<UserGroup> _listUserGroups;
		private Point _selectedCell=new Point(-1,-1); //X:Col, Y:Row.
		private GridOD _selectedGrid=null;
		private bool _isPermissionMode=false;
		private long _userGroupNum;
		private bool _isCEMT;

		public UserControlReportSetup() {
			InitializeComponent();
			//set tags so that we know what grids are associated to what categories.
			gridProdInc.Tag=DisplayReportCategory.ProdInc;
			gridDaily.Tag=DisplayReportCategory.Daily;
			gridMonthly.Tag=DisplayReportCategory.Monthly;
			gridLists.Tag=DisplayReportCategory.Lists;
			gridPublicHealth.Tag=DisplayReportCategory.PublicHealth;
			gridProdInc.ColorSelectedRow=Color.FromArgb(30,55,55,55); //set a transparent colour so you see which cell is selected.
			gridDaily.ColorSelectedRow=Color.FromArgb(30,55,55,55); //set a transparent colour so you see which cell is selected.
			gridMonthly.ColorSelectedRow=Color.FromArgb(30,55,55,55); //set a transparent colour so you see which cell is selected.
			gridLists.ColorSelectedRow=Color.FromArgb(30,55,55,55); //set a transparent colour so you see which cell is selected.
			gridPublicHealth.ColorSelectedRow=Color.FromArgb(30,55,55,55); //set a transparent colour so you see which cell is selected.
		}

		public void InitializeOnStartup(bool refreshData,long userGroupNum,bool isPermissionMode,bool isCEMT=false) {
			_userGroupNum=userGroupNum;
			_isPermissionMode=isPermissionMode;
			_isCEMT=isCEMT;
			if(_isPermissionMode) {
				butUp.Visible=false;
				butDown.Visible=false;
				butSetAll.Visible=true;
				comboUserGroup.Visible=true;
				labelUserGroup.Visible=true;
				label1.Text=Lan.g(this,"The current selection's internal name is:");
			}
			else {
				butUp.Visible=true;
				butDown.Visible=true;
				butSetAll.Visible=false;
				comboUserGroup.Visible=false;
				labelUserGroup.Visible=false;
				label1.Text=Lan.g(this,"Move the selected item within its list.")+"\r\n"+Lan.g(this,"The current selection's internal name is:");
			}
			if(refreshData) {
				ListDisplayReportAll=DisplayReports.GetAll(true);
				ListGroupPermissionsForReports=GroupPermissions.GetPermsForReports();
				ListGroupPermissionsOld=new List<GroupPermission>();
				foreach(GroupPermission perm in ListGroupPermissionsForReports) {
					ListGroupPermissionsOld.Add(perm.Copy());
				}
				if(!isCEMT) {
					_listUserGroups=UserGroups.GetList();
				}
				else {
					_listUserGroups=UserGroups.GetList(true);
				}
				for(int i=0;i<_listUserGroups.Count;i++) {
					comboUserGroup.Items.Add(_listUserGroups[i].Description);
					if(_listUserGroups[i].UserGroupNum==_userGroupNum) {
						comboUserGroup.SelectedIndex=i;
					}
				}
				if(comboUserGroup.SelectedIndex==-1) {
					comboUserGroup.SelectedIndex=0;
				}
			}
			FillGrids();
		}

		 ///<summary>If any columns are reordered or added to this grid, they will need to be considered in the GridCell_Click event below.
    ///This refreshes every grid on the form.</summary>
    private void FillGrids() {
			ListDisplayReportAll=ListDisplayReportAll.OrderBy(x => x.ItemOrder).ToList();
			//Begin Update
			gridProdInc.BeginUpdate();
			gridDaily.BeginUpdate();
			gridMonthly.BeginUpdate();
			gridLists.BeginUpdate();
			gridPublicHealth.BeginUpdate();
			//Add Columns
			int widthDisplayNameCol=140;
			gridProdInc.ListGridColumns.Clear();
			string displayColumnTitle=Lans.g(this,"Display Name");
			string allowedColumnTitle=Lans.g(this,"Allowed");
			string subMenuColumnTitle=Lans.g(this,"Favorite");
			string hiddenColumnTitle=Lans.g(this,"Hidden");
			gridProdInc.ListGridColumns.Add(new GridColumn(displayColumnTitle,widthDisplayNameCol,!_isPermissionMode));
			if(_isPermissionMode) {
				gridProdInc.ListGridColumns.Add(new GridColumn(allowedColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			else {
				gridProdInc.ListGridColumns.Add(new GridColumn(subMenuColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
				gridProdInc.ListGridColumns.Add(new GridColumn(hiddenColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridDaily.ListGridColumns.Clear();
			gridDaily.ListGridColumns.Add(new GridColumn(displayColumnTitle,widthDisplayNameCol,!_isPermissionMode));
			if(_isPermissionMode) {
				gridDaily.ListGridColumns.Add(new GridColumn(allowedColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			else {
				gridDaily.ListGridColumns.Add(new GridColumn(subMenuColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
				gridDaily.ListGridColumns.Add(new GridColumn(hiddenColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridMonthly.ListGridColumns.Clear();
			gridMonthly.ListGridColumns.Add(new GridColumn(displayColumnTitle,widthDisplayNameCol,!_isPermissionMode));
			if(_isPermissionMode) {
				gridMonthly.ListGridColumns.Add(new GridColumn(allowedColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			else {
				gridMonthly.ListGridColumns.Add(new GridColumn(subMenuColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
				gridMonthly.ListGridColumns.Add(new GridColumn(hiddenColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridLists.ListGridColumns.Clear();
			gridLists.ListGridColumns.Add(new GridColumn(displayColumnTitle,widthDisplayNameCol,!_isPermissionMode));
			if(_isPermissionMode) {
				gridLists.ListGridColumns.Add(new GridColumn(allowedColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			else {
				gridLists.ListGridColumns.Add(new GridColumn(subMenuColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
				gridLists.ListGridColumns.Add(new GridColumn(hiddenColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			gridPublicHealth.ListGridColumns.Clear();
			gridPublicHealth.ListGridColumns.Add(new GridColumn(displayColumnTitle,widthDisplayNameCol,!_isPermissionMode));
			if(_isPermissionMode) {
				gridPublicHealth.ListGridColumns.Add(new GridColumn(allowedColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			else {
				gridPublicHealth.ListGridColumns.Add(new GridColumn(subMenuColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
				gridPublicHealth.ListGridColumns.Add(new GridColumn(hiddenColumnTitle,10,HorizontalAlignment.Center){ IsWidthDynamic=true });
			}
			//Add Rows
			gridProdInc.ListGridRows.Clear();
			gridDaily.ListGridRows.Clear();
			gridMonthly.ListGridRows.Clear();
			gridLists.ListGridRows.Clear();
			gridPublicHealth.ListGridRows.Clear();
			foreach(DisplayReport reportCur in ListDisplayReportAll) {
				GridRow row= new GridRow();
				if(_isPermissionMode) {
					row.Cells.Add(reportCur.Description+(reportCur.IsHidden ? " (hidden)" : ""));
					row.Cells.Add(ListGroupPermissionsForReports.Exists(x => x.FKey==reportCur.DisplayReportNum && x.UserGroupNum==_listUserGroups[comboUserGroup.SelectedIndex].UserGroupNum) ? "X" : "");
				}
				else {
					row.Cells.Add(reportCur.Description);
					row.Cells.Add(reportCur.IsVisibleInSubMenu ? "X" : "");
					row.Cells.Add(reportCur.IsHidden ? "X" : "");
				}
				row.Tag=reportCur;
				switch(reportCur.Category) {
					case DisplayReportCategory.ProdInc:
						gridProdInc.ListGridRows.Add(row);
						break;
					case DisplayReportCategory.Daily:
						gridDaily.ListGridRows.Add(row);
						break;
					case DisplayReportCategory.Monthly:
						gridMonthly.ListGridRows.Add(row);
						break;
					case DisplayReportCategory.Lists:
						gridLists.ListGridRows.Add(row);
						break;
					case DisplayReportCategory.PublicHealth:
						gridPublicHealth.ListGridRows.Add(row);
						break;
					case DisplayReportCategory.ArizonaPrimaryCare:
					default:
						break;
				}
			}
			//End Update
			gridProdInc.EndUpdate();
			gridDaily.EndUpdate();
			gridMonthly.EndUpdate();
			gridLists.EndUpdate();
			gridPublicHealth.EndUpdate();
			if(_selectedGrid != null && _selectedCell.Y != -1) {
				_selectedGrid.ListGridRows[_selectedCell.Y].ColorBackG=Color.LightCyan;
				if(_selectedCell.X < _selectedGrid.ListGridColumns.Count) {
					_selectedGrid.SetSelected(_selectedCell);
				}
				else {
					_selectedGrid.SetSelected(_selectedCell.Y,true);
				}
			}
		}
		
		///<summary>This method is used by all grids in this form. If any new grids are added, they will need to be added to this method.</summary>
		private void grid_CellClick(object sender,ODGridClickEventArgs e) {
			if(_selectedCell.Y != -1 && _selectedGrid != null) {
				//commit change before the new cell is selected to save the old cell's changes.
				CommitChange();
			}
			_selectedCell.X=e.Col;
			_selectedCell.Y=e.Row;
			_selectedGrid=(GridOD)sender;
			//this label makes sure the user always has some idea of what the selected report is, even if the DisplayName might be incomprehensible.
			labelODInternal.Text=GetInternalName((DisplayReport)_selectedGrid.ListGridRows[_selectedCell.Y].Tag);
			DisplayReportCategory selectedCat=(DisplayReportCategory)_selectedGrid.Tag;
			//de-select all but the currently selected grid
			if(selectedCat!=DisplayReportCategory.ProdInc) { gridProdInc.SetSelected(-1,true); }
			if(selectedCat!=DisplayReportCategory.Daily) { gridDaily.SetSelected(-1,true); }
			if(selectedCat!=DisplayReportCategory.Monthly) { gridMonthly.SetSelected(-1,true); }
			if(selectedCat!=DisplayReportCategory.Lists) { gridLists.SetSelected(-1,true); }
			if(selectedCat!=DisplayReportCategory.PublicHealth) { gridPublicHealth.SetSelected(-1,true); }
			DisplayReport clicked=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y);
			if(_isPermissionMode) {
				if(_selectedCell.X==1) {
					GroupPermission groupPerm=ListGroupPermissionsForReports.Find(x => x.FKey==clicked.DisplayReportNum && x.UserGroupNum==_listUserGroups[comboUserGroup.SelectedIndex].UserGroupNum);
					if(groupPerm==null) {//They don't have perm
						groupPerm=new GroupPermission();
						groupPerm.NewerDate=DateTime.MinValue;
						groupPerm.NewerDays=0;
						groupPerm.PermType=Permissions.Reports;
						groupPerm.UserGroupNum=_listUserGroups[comboUserGroup.SelectedIndex].UserGroupNum;
						groupPerm.FKey=clicked.DisplayReportNum;
						ListGroupPermissionsForReports.Add(groupPerm);
					}
					else {
						ListGroupPermissionsForReports.Remove(groupPerm);
					}
				}
			}
			else {
				if(_selectedCell.X==1) {
					clicked.IsVisibleInSubMenu = !clicked.IsVisibleInSubMenu;
					if(clicked.IsVisibleInSubMenu) {
						clicked.IsHidden=false;
					}
				}
				else if(_selectedCell.X==2) {
					clicked.IsHidden = !clicked.IsHidden;
					if(clicked.IsHidden) {
						clicked.IsVisibleInSubMenu=false;
					}
				}
			}
			FillGrids();
		}

		///<summary>Returns the appropriate user-friendly internal name.</summary>
		private string GetInternalName(DisplayReport displayReport) {
			switch(displayReport.InternalName) {
				case "ODToday":
					return "Today";
				case "ODYesterday":
					return "Yesterday";
				case "ODThisMonth":
					return "This Month";
				case "ODLastMonth":
					return "Last Month";
				case "ODThisYear":
					return "This Year";
				case "ODMoreOptions":
					return "More Options";
				case "ODProviderPayrollSummary":
					return "Provider Payroll Summary";
				case "ODProviderPayrollDetailed":
					return "Provider Payroll Detailed";
				case "ODAdjustments":
					return "Adjustments";
				case "ODPayments":
					return "Payments";
				case "ODProcedures":
					return "Procedures";
				case "ODWriteoffs":
					return "Writeoffs";
				case DisplayReports.ReportNames.IncompleteProcNotes:
					return "Incomplete Proc Notes";
				case "ODRoutingSlips":
					return "Routing Slips";
				case "ODAgingAR":
					return "Aging AR";
				case DisplayReports.ReportNames.ClaimsNotSent:
					return "Claims Not Sent";
				case "ODCapitation":
					return "Capitation";
				case "ODFinanceCharge":
					return "Finance Charge";
				case DisplayReports.ReportNames.OutstandingInsClaims:
					return "Outstanding Ins Claims";
				case "ODProcsNotBilled":
					return "Procs Not Billed";
				case "ODPPOWriteoffs":
					return "PPO Writeoffs";
				case "ODPaymentPlans":
					return "Payment Plans";
				case "ODReceivablesBreakdown":
					return "Receivables Breakdown";
				case "ODUnearnedIncome":
					return "Unearned Income";
				case "ODInsuranceOverpaid":
					return "Insurance Overpaid";
				case "ODActivePatients":
					return "Active Patients";
				case "ODAppointments":
					return "Appointments";
				case "ODBirthdays":
					return "Birthdays";
				case "ODBrokenAppointments":
					return "Broken Appointments";
				case "ODInsurancePlans":
					return "Insurance Plans";
				case "ODNewPatients":
					return "New Patients";
				case "ODPatientsRaw":
					return "Patients Raw";
				case "ODPatientNotes":
					return "Patient Notes";
				case "ODPrescriptions":
					return "Prescriptions";
				case "ODProcedureCodes":
					return "Procedure Codes";
				case DisplayReports.ReportNames.ODProcOverpaid:
					return "Procedures Overpaid";
				case "ODReferralsRaw":
					return "Referrals Raw";
				case "ODReferralAnalysis":
					return "Referral Analysis";
				case DisplayReports.ReportNames.ReferredProcTracking:
					return "Referred Proc Tracking";
				case DisplayReports.ReportNames.TreatmentFinder:
					return "Treatment Finder";
				case "ODRawScreeningData":
					return "Raw Screening Data";
				case "ODRawPopulationData":
					return "Raw Population Data";
				case "ODEligibilityFile":
					return "Eligibility File";
				case "ODEncounterFile":
					return "Encounter File";
				case "ODPresentedTreatmentProd":
					return "Presented Treatment Prod";
				case "ODTreatmentPresentationStats":
					return "Treatment Presentation Stats";
				case "ODNetProdDetailDaily":
					return "Net Prod Detail Daily";
				case "ODDentalSealantMeasure":
					return "Dental Sealant Measure";
				case "ODInsurancePayPlansPastDue":
					return "Insurance Pay Plans Past Due";
				case DisplayReports.ReportNames.UnfinalizedInsPay:
					return "Unfinalized Insurance";
				case DisplayReports.ReportNames.PatPortionUncollected:
					return "Patient Portion Uncollected";
				case DisplayReports.ReportNames.WebSchedAppointments:
					return "Web Sched Appointments";
				case DisplayReports.ReportNames.InsAging:
					return "Insurance Aging Report";
				default:
					return "";
			}
		}

		///<summary>Commit changes that the user might have made to the display name.</summary>
		private void CommitChange() {
			DisplayReportCategory selectedCat=(DisplayReportCategory)_selectedGrid.Tag;
			DisplayReport clicked=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y);
			clicked.Description=_selectedGrid.ListGridRows[_selectedCell.Y].Cells[0].Text.Replace(" (hidden)","");//Remove (hidden) text
		}
	
		private void grid_CellLeave(object sender,ODGridClickEventArgs e) {
			CommitChange();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(_selectedCell.Y == -1 || _selectedGrid == null) {
				MsgBox.Show(this,"Please select a report first.");
				return;
			}
			DisplayReportCategory selectedCat = (DisplayReportCategory)_selectedGrid.Tag;
			DisplayReport selectedReport=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y);
			if(selectedReport.ItemOrder==0) {
				return; //the item is already the first in the list and cannot go up anymore.
			}
			DisplayReport switchReport=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y-1);
			selectedReport.ItemOrder--;
			_selectedCell.Y--;
			switchReport.ItemOrder++;
			FillGrids();
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(_selectedCell.Y == -1 || _selectedGrid == null) {
				MsgBox.Show(this,"Please select a report first.");
				return;
			}
			DisplayReportCategory selectedCat = (DisplayReportCategory)_selectedGrid.Tag;
			DisplayReport selectedReport=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y);
			if(selectedReport.ItemOrder==_selectedGrid.ListGridRows.Count-1) {
				return; //the item is already the last in the list and cannot go down anymore.
			}
			DisplayReport switchReport=ListDisplayReportAll.Find(x => x.Category == selectedCat && x.ItemOrder == _selectedCell.Y+1);
			selectedReport.ItemOrder++;
			_selectedCell.Y++;
			switchReport.ItemOrder--;
			FillGrids();
		}

		private void butSetAll_Click(object sender,EventArgs e) {
			ListGroupPermissionsForReports.RemoveAll(x => x.UserGroupNum==_listUserGroups[comboUserGroup.SelectedIndex].UserGroupNum);
			foreach(DisplayReport report in ListDisplayReportAll) {
				if(report.IsHidden) {
					continue;
				}
				GroupPermission groupPerm=new GroupPermission();
				groupPerm.NewerDate=DateTime.MinValue;
				groupPerm.NewerDays=0;
				groupPerm.PermType=Permissions.Reports;
				groupPerm.UserGroupNum=_listUserGroups[comboUserGroup.SelectedIndex].UserGroupNum;
				groupPerm.FKey=report.DisplayReportNum;
				ListGroupPermissionsForReports.Add(groupPerm);
			}
			FillGrids();
		}

		private void comboUserGroup_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrids();
		}

	}
}
