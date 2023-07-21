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
	/// <summary></summary>
	public partial class FormTimeCardSetup:FormODBase {
		private bool _hasChanged;

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
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("Start Date",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("End Date",80);
			gridMain.Columns.Add(col);
			col=new GridColumn("Paycheck Date",100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			UI.GridRow row;
			for (int i=0;i<_listPayPeriods.Count;i++) {
				PayPeriod payPeriod = _listPayPeriods[i];
				if(checkHideOlder.Checked && payPeriod.DateStart < DateTime.Today.AddMonths(-6)) {
					continue;
				}
				row=new OpenDental.UI.GridRow();
				row.Cells.Add(payPeriod.DateStart.ToShortDateString());
				row.Cells.Add(payPeriod.DateStop.ToShortDateString());
				if(payPeriod.DatePaycheck.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(payPeriod.DatePaycheck.ToShortDateString());
				}
				row.Tag=payPeriod;
				if(payPeriod.DateStart<=DateTime.Today && payPeriod.DateStop >=DateTime.Today) {
					row.ColorBackG=Color.LightCyan;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FillRules() {
			TimeCardRules.RefreshCache();
			//Start with a convenient sorting of all employees on top, followed by a last name sort.
			List<TimeCardRule> listTimeCardRules=TimeCardRules.GetDeepCopy().OrderBy(x => x.IsOvertimeExempt)
				.ThenBy(x => x.EmployeeNum!=0)
				.ThenBy(x => (Employees.GetEmp(x.EmployeeNum)??new Employee()).LName)
				.ToList();
			gridRules.BeginUpdate();
			gridRules.Columns.Clear();
			GridColumn col=new GridColumn("Employee",150,GridSortingStrategy.StringCompare);
			gridRules.Columns.Add(col);
			col=new GridColumn("Rate 2 before x Time",90,GridSortingStrategy.TimeParse);
			gridRules.Columns.Add(col);
			col=new GridColumn("Rate 2 after x Time",90,GridSortingStrategy.TimeParse);
			gridRules.Columns.Add(col);
			col=new GridColumn("OT after x Hours",90,GridSortingStrategy.TimeParse);
			gridRules.Columns.Add(col);
			col=new GridColumn("Min Clock In Time",90,GridSortingStrategy.TimeParse);
			gridRules.Columns.Add(col);
			col=new GridColumn("Is OT Exempt",80,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridRules.Columns.Add(col);
			col=new GridColumn("Has Weekend Rate 3",80,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridRules.Columns.Add(col);
			gridRules.ListGridRows.Clear();
			UI.GridRow row;
			for(int i=0;i<listTimeCardRules.Count;i++) {
				row=new GridRow();
				if(listTimeCardRules[i].EmployeeNum==0) {
					row.Cells.Add(Lan.g(this,"All Employees"));
				}
				else {
					Employee emp=Employees.GetEmp(listTimeCardRules[i].EmployeeNum);
					row.Cells.Add(emp.FName+" "+emp.LName);
				}
				row.Cells.Add(listTimeCardRules[i].BeforeTimeOfDay.ToStringHmm());
				row.Cells.Add(listTimeCardRules[i].AfterTimeOfDay.ToStringHmm());
				row.Cells.Add(listTimeCardRules[i].OverHoursPerDay.ToStringHmm());
				row.Cells.Add(listTimeCardRules[i].MinClockInTime.ToStringHmm());
				row.Cells.Add(listTimeCardRules[i].IsOvertimeExempt ? "X" : "");
				row.Cells.Add(listTimeCardRules[i].HasWeekendRate3  ? "X" : "");
				row.Tag=listTimeCardRules[i];
				gridRules.ListGridRows.Add(row);
			}
			gridRules.EndUpdate();
		}

		///<summary>Makes sure that the pay periods that the user has selected are safe to delete.
		///A pay period cannot be deleted in bulk if: 
		///a) It is in the past OR 
		///b) There are clockevents tied to it and there are no other pay periods for the date of the clockevent.</summary>
		private bool IsSafeToDelete(List<PayPeriod> listPayPeriodsSelected) {
			if(listPayPeriodsSelected.FindAll(x => x.DateStop < DateTime.Today).Count() > 0) {
				MsgBox.Show(this,"You may not delete past pay periods from here. Delete them individually by double clicking them instead.");
				return false;
			}
			List<ClockEvent> listClockEventsAll = ClockEvents.GetAllForPeriod(listPayPeriodsSelected.Min(x => x.DateStart), listPayPeriodsSelected.Max(x => x.DateStop));
			for(int i=0;i<listPayPeriodsSelected.Count;i++) {
				List<ClockEvent> listClockEventsForPeriod = listClockEventsAll.FindAll(x => x.TimeDisplayed1 >= listPayPeriodsSelected[i].DateStart 
					&& x.TimeDisplayed2 <= listPayPeriodsSelected[i].DateStop);
				if(listClockEventsForPeriod.Count() == 0) {
					continue;
				}
				//there ARE clock events for this period. now are there other periods that are *not* in the selected list?
				for(int j=0;j<listClockEventsForPeriod.Count;j++) {
					if(_listPayPeriods.FindAll(x => x.DateStart <= listClockEventsForPeriod[j].TimeDisplayed1
						&& x.DateStop >= listClockEventsForPeriod[j].TimeDisplayed1
						&& !listPayPeriodsSelected.Contains(x)).Count() < 1)
					{
						//if no, then kick out.
						MsgBox.Show(this,"You may not delete all pay periods where a clock event exists.");
						return false;
					}
				}
			}
			return true;
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormPayPeriodEdit formPayPeriodEdit=new FormPayPeriodEdit((PayPeriod)gridMain.ListGridRows[e.Row].Tag);
			formPayPeriodEdit.ShowDialog();
			if(formPayPeriodEdit.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillGrid();
			_hasChanged=true;
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			PayPeriod payPeriod=new PayPeriod();
			if(PayPeriods.GetCount()==0) {
				payPeriod.DateStart=DateTime.Today;
			}
			else {
				payPeriod.DateStart=PayPeriods.GetLast().DateStop.AddDays(1);
			}
			payPeriod.DateStop=payPeriod.DateStart.AddDays(13);//payPeriodCur.DateStop is inclusive, this is effectively a 14 day default pay period. This only affects default date of newly created pay periods.
			payPeriod.DatePaycheck=payPeriod.DateStop.AddDays(4);
			using FormPayPeriodEdit formPayPeriodEdit=new FormPayPeriodEdit(payPeriod);
			formPayPeriodEdit.IsNew=true;
			formPayPeriodEdit.ShowDialog();
			if(formPayPeriodEdit.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillGrid();
			_hasChanged=true;
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			//Automatically generate payperiods based on settings.
			using FormPayPeriodManager formPayPeriodManager=new FormPayPeriodManager();
			formPayPeriodManager.ShowDialog();
			if(formPayPeriodManager.DialogResult == DialogResult.Cancel) {
				return;
			}
			FillGrid();
		}

		private void checkUseDecimal_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardsUseDecimalInsteadOfColon,checkUseDecimal.Checked)) {
				_hasChanged=true;
			}
		}

		private void checkAdjOverBreaks_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardsMakesAdjustmentsForOverBreaks,checkAdjOverBreaks.Checked)) {
				_hasChanged=true;
			}
		}

		private void checkShowSeconds_Click(object sender,EventArgs e) {
			if(Prefs.UpdateBool(PrefName.TimeCardShowSeconds,checkShowSeconds.Checked)) {
				_hasChanged=true;
			}
		}

		private void butAddRule_Click(object sender,EventArgs e) {
			using FormTimeCardRuleEdit formTimeCardRuleEdit=new FormTimeCardRuleEdit();
			formTimeCardRuleEdit.ShowDialog();
			FillRules();
			_hasChanged=true;
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
			using FormTimeCardRuleEdit formTimeCardRuleEdit=new FormTimeCardRuleEdit((TimeCardRule)gridRules.ListGridRows[e.Row].Tag);
			formTimeCardRuleEdit.ShowDialog();
			FillRules();
			_hasChanged=true;
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
			List<PayPeriod> listPayPeriodsSelected = new List<PayPeriod>();
			for(int i = 0;i < gridMain.SelectedIndices.Length;i++) {
				listPayPeriodsSelected.Add((PayPeriod)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag);
			}
			if(!IsSafeToDelete(listPayPeriodsSelected)) {
				return;
			}
			//Actual deletion logic below.
			for(int i=0;i<listPayPeriodsSelected.Count;i++) {
				PayPeriods.Delete(listPayPeriodsSelected[i]);
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
				_hasChanged=true;
			}
			if(Prefs.UpdateString(PrefName.ADPRunIID,textADPRunIID.Text)) {
				_hasChanged=true;
			}
			if(_hasChanged) {
				DataValid.SetInvalid(InvalidType.Employees,InvalidType.Prefs,InvalidType.TimeCardRules);
			}
		}
	}
}