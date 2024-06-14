using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmApptRuleEdit : FrmODBase {
		///<summary></summary>
		public bool IsNew;
		private AppointmentRule _appointmentRule;

		///<summary></summary>
		public FrmApptRuleEdit(AppointmentRule appointmentRule)
		{
			//
			// Required for Windows Form Designer support
			//
			_appointmentRule=appointmentRule.Clone();
			InitializeComponent();
			Load+=FrmApptRuleEdit_Load;
			PreviewKeyDown+=FrmApptRuleEdit_PreviewKeyDown;
		}

		private void FrmApptRuleEdit_Load(object sender, EventArgs e) {
			Lang.F(this);
			textRuleDesc.Text=_appointmentRule.RuleDesc;
			textCodeStart.Text=_appointmentRule.CodeStart;
			textCodeEnd.Text=_appointmentRule.CodeEnd;
			checkIsEnabled.Checked=_appointmentRule.IsEnabled;
		}

		private void FrmApptRuleEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butDelete.IsAltKey(Key.D,e)) {
				butDelete_Click(this,new EventArgs());
			}
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				IsDialogOK=false;
				return;
			}
			AppointmentRules.Delete(_appointmentRule);
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
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
			_appointmentRule.IsEnabled=checkIsEnabled.Checked.Value;
			if(IsNew){
				AppointmentRules.Insert(_appointmentRule);
			}
			else{
				AppointmentRules.Update(_appointmentRule);
			}
			IsDialogOK=true;
		}

	}
}