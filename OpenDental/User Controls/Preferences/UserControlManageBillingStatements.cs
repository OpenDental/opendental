using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlManageBillingStatements:UserControl {

		#region Fields - Private
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Public

		#region Constructors
		public UserControlManageBillingStatements() {
			InitializeComponent();
		}
		#endregion Constructors

		#region Methods - Event Handlers
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillManageBillingStatements() {
			checkStatementShowReturnAddress.Checked=PrefC.GetBool(PrefName.StatementShowReturnAddress);
			checkStatementShowNotes.Checked=PrefC.GetBool(PrefName.StatementShowNotes);
			checkStatementShowAdjNotes.Checked=PrefC.GetBool(PrefName.StatementShowAdjNotes);
			checkStatementShowProcBreakdown.Checked=PrefC.GetBool(PrefName.StatementShowProcBreakdown);
			comboUseChartNum.Items.Add(Lan.g(this,"PatNum"));
			comboUseChartNum.Items.Add(Lan.g(this,"ChartNumber"));
			if(PrefC.GetBool(PrefName.StatementAccountsUseChartNumber)) {
				comboUseChartNum.SelectedIndex=1;
			} 
			else {
				comboUseChartNum.SelectedIndex=0;
			}
			checkStatementsAlphabetically.Checked=PrefC.GetBool(PrefName.PrintStatementsAlphabetically);
			checkStatementsAlphabetically.Visible=PrefC.HasClinicsEnabled;
			if(PrefC.GetLong(PrefName.StatementsCalcDueDate)!=-1) {
				textStatementsCalcDueDate.Text=PrefC.GetLong(PrefName.StatementsCalcDueDate).ToString();
			}
			textPayPlansBillInAdvanceDays.Text=PrefC.GetLong(PrefName.PayPlansBillInAdvanceDays).ToString();
			textBillingElectBatchMax.Text=PrefC.GetInt(PrefName.BillingElectBatchMax).ToString();
			checkIntermingleDefault.Checked=PrefC.GetBool(PrefName.IntermingleFamilyDefault);
			checkBillingShowProgress.Checked=PrefC.GetBool(PrefName.BillingShowSendProgress);
		}

		public bool SaveManageBillingStatements(){
			if(!textStatementsCalcDueDate.IsValid()
				| !textPayPlansBillInAdvanceDays.IsValid()
				| !textBillingElectBatchMax.IsValid()) 
			{
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return false;
			}
			Changed|=Prefs.UpdateBool(PrefName.StatementShowReturnAddress,checkStatementShowReturnAddress.Checked);
			Changed|=Prefs.UpdateBool(PrefName.StatementShowNotes,checkStatementShowNotes.Checked);
			Changed|=Prefs.UpdateBool(PrefName.StatementShowAdjNotes,checkStatementShowAdjNotes.Checked);
			Changed|=Prefs.UpdateBool(PrefName.StatementShowProcBreakdown,checkStatementShowProcBreakdown.Checked);
			Changed|=Prefs.UpdateBool(PrefName.StatementAccountsUseChartNumber,comboUseChartNum.SelectedIndex==1);
			Changed|=Prefs.UpdateLong(PrefName.PayPlansBillInAdvanceDays,PIn.Long(textPayPlansBillInAdvanceDays.Text));
			Changed|=Prefs.UpdateBool(PrefName.IntermingleFamilyDefault,checkIntermingleDefault.Checked);
			Changed|=Prefs.UpdateInt(PrefName.BillingElectBatchMax,PIn.Int(textBillingElectBatchMax.Text));
			Changed|=Prefs.UpdateBool(PrefName.BillingShowSendProgress,checkBillingShowProgress.Checked);
			Changed|=Prefs.UpdateBool(PrefName.PrintStatementsAlphabetically,checkStatementsAlphabetically.Checked);
			if(textStatementsCalcDueDate.Text=="") {
				if(Prefs.UpdateLong(PrefName.StatementsCalcDueDate,-1)) {
					Changed=true;
				}
			} 
			else {
				if(Prefs.UpdateLong(PrefName.StatementsCalcDueDate,PIn.Long(textStatementsCalcDueDate.Text))) {
					Changed=true;
				}
			}
		return true;
		}
		#endregion Methods - Public
	}
}
