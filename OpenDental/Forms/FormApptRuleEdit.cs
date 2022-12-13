using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormApptRuleEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private AppointmentRule _appointmentRule;

		///<summary></summary>
		public FormApptRuleEdit(AppointmentRule appointmentRule)
		{
			//
			// Required for Windows Form Designer support
			//
			_appointmentRule=appointmentRule.Clone();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptRuleEdit_Load(object sender, System.EventArgs e) {
			textRuleDesc.Text=_appointmentRule.RuleDesc;
			textCodeStart.Text=_appointmentRule.CodeStart;
			textCodeEnd.Text=_appointmentRule.CodeEnd;
			checkIsEnabled.Checked=_appointmentRule.IsEnabled;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			AppointmentRules.Delete(_appointmentRule);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textRuleDesc.Text==""){
				MsgBox.Show(this,"Description not allowed to be blank.");
				return;
			}
			if(!ProcedureCodes.IsValidCode(textCodeStart.Text)
				|| !ProcedureCodes.IsValidCode(textCodeEnd.Text))
			{
				MsgBox.Show(this,"Start and end codes must be valid procedure codes.");
				return;
			}
			_appointmentRule.RuleDesc=textRuleDesc.Text;
			_appointmentRule.CodeStart=textCodeStart.Text;
			_appointmentRule.CodeEnd=textCodeEnd.Text;
			_appointmentRule.IsEnabled=checkIsEnabled.Checked;
			if(IsNew){
				AppointmentRules.Insert(_appointmentRule);
			}
			else{
				AppointmentRules.Update(_appointmentRule);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		


	}
}





















