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
		private AppointmentRule ApptRuleCur;

		///<summary></summary>
		public FormApptRuleEdit(AppointmentRule apptRuleCur)
		{
			//
			// Required for Windows Form Designer support
			//
			ApptRuleCur=apptRuleCur.Clone();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptRuleEdit_Load(object sender, System.EventArgs e) {
			textRuleDesc.Text=ApptRuleCur.RuleDesc;
			textCodeStart.Text=ApptRuleCur.CodeStart;
			textCodeEnd.Text=ApptRuleCur.CodeEnd;
			checkIsEnabled.Checked=ApptRuleCur.IsEnabled;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			AppointmentRules.Delete(ApptRuleCur);
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
			ApptRuleCur.RuleDesc=textRuleDesc.Text;
			ApptRuleCur.CodeStart=textCodeStart.Text;
			ApptRuleCur.CodeEnd=textCodeEnd.Text;
			ApptRuleCur.IsEnabled=checkIsEnabled.Checked;
			if(IsNew){
				AppointmentRules.Insert(ApptRuleCur);
			}
			else{
				AppointmentRules.Update(ApptRuleCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		

		


	}
}





















