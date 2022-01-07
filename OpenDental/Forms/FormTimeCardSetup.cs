using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormTimeCardSetup:FormODBase {
		private bool changed;

		///<summary>Locally cached list of pay periods.</summary>
		private List<PayPeriod> _listPayPeriods;

		///<summary></summary>
		public FormTimeCardSetup() {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPayPeriods_Load(object sender,System.EventArgs e) {
			checkUseDecimal.Checked=PrefC.GetBool(PrefName.TimeCardsUseDecimalInsteadOfColon);
			checkAdjOverBreaks.Checked=PrefC.GetBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks);
			checkShowSeconds.Checked=PrefC.GetBool(PrefName.TimeCardShowSeconds);
			Employees.RefreshCache();
			FillGrid();
			FillRules();
			textADPRunIID.Text=PrefC.GetString(PrefName.ADPRunIID);
			textADPCompanyCode.Text=PrefC.GetString(PrefName.ADPCompanyCode);
		}

		///<summary>Does not refresh the cached list.  Make sure any updates to _listPayPeriods are done before calling this method.</summary>
		private void FillGrid() {
			PayPeriods.RefreshCache();
			_listPayPeriods=PayPeriods.GetDeepCopy().OrderBy(x => x.DateStart).ToList();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Start Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("End Date",80);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Paycheck Date",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			foreach(PayPeriod payPeriodCur in _listPayPeriods) {
				if(checkHideOlder.Checked && payPeriodCur.DateStart < DateTime.Today.AddMonths(-6)) {
					continue;
				}
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(payPeriodCur.DateStart.ToShortDateString());
				row.Cells.Add(payPeriodCur.DateStop.ToShortDateString());
				if(payPeriodCur.DatePaycheck.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(payPeriodCur.DatePaycheck.ToShortDateString());
				}
				row.Tag=payPeriodCur;
				if(payPeriodCur.DateStart<=DateTime.Today && payPeriodCur.DateStop >=DateTime.Today) {
					row.ColorBackG=Color.LightCyan;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillRules() {
			TimeCardRules.RefreshCache();
			//Start with a convenient sorting of all employees on top, followed by a last name sort.
			List<TimeCardRule> listSorted=TimeCardRules.GetDeepCopy().OrderBy(x => x.IsOvertimeExempt)
				.ThenBy(x => x.EmployeeNum!=0)
				.ThenBy(x => (Employees.GetEmp(x.EmployeeNum)??new Employee()).LName)
				.ToList();
			gridRules.BeginUpdate();
			gridRules.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Employee",150,GridSortingStrategy.StringCompare);
			gridRules.ListGridColumns.Add(col);
			col=new GridColumn("OT before x Time",105,GridSortingStrategy.TimeParse);
			gridRules.ListGridColumns.Add(col);
			col=new GridColumn("OT after x Time",100,GridSortingStrategy.TimeParse);
			gridRules.ListGridColumns.Add(col);
			col=new GridColumn("OT after x Hours",110,GridSortingStrategy.TimeParse);
			gridRules.ListGridColumns.Add(col);
			col=new GridColumn("Min Clock In Time",105,GridSortingStrategy.TimeParse);
			gridRules.ListGridColumns.Add(col);
			col=new GridColumn("Is OT Exempt",100,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridRules.ListGridColumns.Add(col);
			gridRules.ListGridRows.Clear();
			UI.GridRow row;
			for(int i=0;i<listSorted.Count;i++) {
				row=new GridRow();
				if(listSorted[i].EmployeeNum==0) {
					row.Cells.Add(Lan.g(this,"All Employees"));
				}
				else {
					Employee emp=Employees.GetEmp(listSorted[i].EmployeeNum);
					row.Cells.Add(emp.FName+" "+emp.LName);
				}
				row.Cells.Add(listSorted[i].BeforeTimeOfDay.ToStringHmm());
				row.Cells.Add(listSorted[i].AfterTimeOfDay.ToStringHmm());
				row.Cells.Add(listSorted[i].OverHoursPerDay.ToStringHmm());
				row.Cells.Add(listSorted[i].MinClockInTime.ToStringHmm());
				row.Cells.Add(listSorted[i].IsOvertimeExempt ? "X" : "");
				row.Tag=listSorted[i];
				gridRules.ListGridRows.Add(row);
			}
			gridRules.EndUpdate();
		}

		///<summary>Makes sure that the pay periods that the user has selected are safe to delete.
		///A pay period cannot be deleted in bulk if: 
		///a) It is in the past OR 
		///b) There are clockevents tied to it and there are no other pay periods for the date of the clockevent.</summary>
		private bool IsSafeToDelete(List<PayPeriod> listSelectedPayPeriods,out List<PayPeriod> retListToDelete) {
			if(listSelectedPayPeriods.Where(x => x.DateStop < DateTime.Today).Count() > 0) {
				MsgBox.Show(this,"You may not delete past pay periods from here. Delete them individually by double clicking them instead.");
				retListToDelete=new List<PayPeriod>();
				return false;
			}
			List<PayPeriod> listPayPeriodsToDelete = new List<PayPeriod>();
			List<ClockEvent> listClockEventsAll = ClockEvents.GetAllForPeriod(listSelectedPayPeriods.Min(x => x.DateStart), listSelectedPayPeriods.Max(x => x.DateStop));
			foreach(PayPeriod payPeriod in listSelectedPayPeriods) {
				List<ClockEvent> listClockEventsForPeriod = listClockEventsAll.Where(x => x.TimeDisplayed1 >= payPeriod.DateStart && x.TimeDisplayed2 <= payPeriod.DateStop).ToList();
				if(listClockEventsForPeriod.Count == 0) {
					//there are no clock events for this period.
					listPayPeriodsToDelete.Add(payPeriod);
					continue;
				}
				//there ARE clock events for this period. now are there other periods that are *not* in the selected list?
				foreach(ClockEvent clockEvent in listClockEventsForPeriod) {
					if(_listPayPeriods.Where(x => x.DateStart <= clockEvent.TimeDisplayed1 && x.DateStop >= clockEvent.TimeDisplayed1 && !listSelectedPayPeriods.Contains(x)).Count() < 1) {
						//if no, then kick out.
						MsgBox.Show(this,"You may not delete all pay periods where a clock event exists.");
						retListToDelete=new List<PayPeriod>();
						return false;
					}
					//otherwise, the add this pay period to the list to delete and continue
					listPayPeriodsToDelete.Add(payPeriod);
				}
			}
			retListToDelete=listPayPeriodsToDelete;
			return true;
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormPayPeriodEdit FormP=new FormPayPeriodEdit((PayPeriod)gridMain.ListGridRows[e.Row].Tag);
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillGrid();
			changed=true;
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			PayPeriod payPeriodCur=new PayPeriod();
			if(PayPeriods.GetCount()==0) {
				payPeriodCur.DateStart=DateTime.Today;
			}
			else {
				payPeriodCur.DateStart=PayPeriods.GetLast().DateStop.AddDays(1);
			}
			payPeriodCur.DateStop=payPeriodCur.DateStart.AddDays(13);//payPeriodCur.DateStop is inclusive, this is effectively a 14 day default pay period. This only affects default date of newly created pay periods.
			payPeriodCur.DatePaycheck=payPeriodCur.DateStop.AddDays(4);
			using FormPayPeriodEdit FormP=new FormPayPeriodEdit(payPeriodCur);
			FormP.IsNew=true;
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillGrid();
			changed=true;
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			//Automatically generate payperiods based on settings.
			using FormPayPeriodManager FormPPM=new FormPayPeriodManager();
			FormPPM.ShowDialog();
			if(FormPPM.DialogResult == DialogResult.Cancel) {
				return;
			}
			FillGrid();
		}

		private void checkUseDecimal_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardsUseDecimalInsteadOfColon,checkUseDecimal.Checked)) {
				changed=true;
			}
		}

		private void checkAdjOverBreaks_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,checkAdjOverBreaks.Checked)) {
				changed=true;
			}
		}

		private void checkShowSeconds_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardShowSeconds,checkShowSeconds.Checked)) {
				changed=true;
			}
		}

		private void butAddRule_Click(object sender,EventArgs e) {
			using FormTimeCardRuleEdit FormT=new FormTimeCardRuleEdit();
			FormT.ShowDialog();
			FillRules();
			changed=true;
		}
		
		private void butDeleteRules_Click(object sender,EventArgs e) {
			if(gridRules.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select one or more Rules to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete all selected Rules?")) {
				return;
			}
			List<long> listTimeCardRuleNums=gridRules.SelectedTags<TimeCardRule>().Select(x => x.TimeCardRuleNum).ToList();
			TimeCardRules.DeleteMany(listTimeCardRuleNums);
			DataValid.SetInvalid(InvalidType.TimeCardRules);
			FillRules();
		}

		private void gridRules_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormTimeCardRuleEdit FormT=new FormTimeCardRuleEdit((TimeCardRule)gridRules.ListGridRows[e.Row].Tag);
			FormT.ShowDialog();
			FillRules();
			changed=true;
		}

		private void checkHideOlder_CheckedChanged(object sender,EventArgs e) {
			FillGrid();
			if(checkHideOlder.Checked) {
				gridMain.ScrollToEnd();
			}
		}

		///<summary>Deletes all the selected pay periods. Performs validation to make sure the delete is safe.</summary>
		private void butDelete_Click(object sender,EventArgs e) {
			//validation
			if(gridMain.SelectedIndices.Length == 0) {
				MsgBox.Show(this,"Please select one or more Pay Periods to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you want to delete all selected pay periods?")) {
				return;
			}
			List<PayPeriod> listSelectedPayPeriods = new List<PayPeriod>();
			for(int i = 0;i < gridMain.SelectedIndices.Length;i++) {
				listSelectedPayPeriods.Add((PayPeriod)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag);
			}
			List<PayPeriod> listPayPeriodsToDelete;
			if(!IsSafeToDelete(listSelectedPayPeriods,out listPayPeriodsToDelete)) {
				return;
			}
			if(listPayPeriodsToDelete == null || listPayPeriodsToDelete.Count == 0) {
				return;
			}
			//Actual deletion logic below.
			foreach(PayPeriod payPeriod in listSelectedPayPeriods) {
				PayPeriods.Delete(payPeriod);
			}
			FillGrid();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		private void FormPayPeriods_FormClosing(object sender,FormClosingEventArgs e) {
			//validation on Form_Closing to account for if the user doesn't use the close button to close the form.
			string errors=TimeCardRules.ValidateOvertimeRules();
			if(!string.IsNullOrEmpty(errors)) {
				errors="Fix the following errors:\r\n"+errors;
				MessageBox.Show(errors);
				e.Cancel=true;
			}
			if(textADPCompanyCode.Text!="" && !Regex.IsMatch(textADPCompanyCode.Text,"^[a-zA-Z0-9]{2,3}$")) {
				MsgBox.Show(this,"ADP Company Code must be two or three alpha-numeric characters.\r\nFix or clear before continuing.");
				e.Cancel=true;
			}
			if(Prefs.UpdateString(PrefName.ADPCompanyCode,textADPCompanyCode.Text)) {
				changed=true;
			}
			if(Prefs.UpdateString(PrefName.ADPRunIID,textADPRunIID.Text)) {
				changed=true;
			}
			if(changed) {
				DataValid.SetInvalid(InvalidType.Employees,InvalidType.Prefs,InvalidType.TimeCardRules);
			}
		}
	}
}





















