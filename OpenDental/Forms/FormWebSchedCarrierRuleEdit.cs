using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormWebSchedCarrierRuleEdit:FormODBase {
		public WebSchedCarrierRule WebSchedCarrierRule;
		public bool IsOtherDefaultCarrier=false;

		public FormWebSchedCarrierRuleEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormWebSchedCarrierRuleEdit_Load(object sender,EventArgs e) {
			textCarrierName.Text=WebSchedCarrierRule.CarrierName;
			textDisplayName.Text=WebSchedCarrierRule.DisplayName;
			if(WebSchedCarrierRule.Rule==RuleType.Allow) {
				radioAllow.Checked=true;
			}
			else if(WebSchedCarrierRule.Rule==RuleType.AllowWithInput) {
				radioAllowWithInput.Checked=true;
			} 
			else if(WebSchedCarrierRule.Rule==RuleType.AllowWithMessage) {
				radioAllowWithMessage.Checked=true;
			} 
			else {
				radioBlock.Checked=true;
			}
			textMessage.Text=WebSchedCarrierRule.Message;
			if(IsOtherDefaultCarrier) {
				radioAllowWithInput.Enabled=true;
			}
			labelCount.Text=textMessage.Text.Length.ToString()+"/100";
		}

		private void radioAllowWithInput_CheckedChanged(object sender,EventArgs e) {
			if(radioAllowWithInput.Checked) {
				textMessage.Enabled=false;
				textMessage.Text="";
			}
			else {
				textMessage.Enabled=true;
			}
		}

		private void textMessage_TextChanged(object sender,EventArgs e) {
			labelCount.Text=textMessage.Text.Length.ToString()+"/100";
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(radioAllowWithMessage.Checked || radioBlock.Checked) {
				if(string.IsNullOrEmpty(textMessage.Text.Trim())) {
					MsgBox.Show(this,"Rules that have the Rule Type of \"Allow With Message\" or \"Block\" must contain a message.");
					return;
				}
			}
			if(string.IsNullOrEmpty(textDisplayName.Text.Trim())) {
				MsgBox.Show(this,"Display Name can not be blank.");
				return;
			}
			WebSchedCarrierRule.DisplayName=PIn.String(textDisplayName.Text);
			if(radioAllow.Checked) {
				WebSchedCarrierRule.Rule=RuleType.Allow;
			}
			else if(radioAllowWithInput.Checked) {
				WebSchedCarrierRule.Rule=RuleType.AllowWithInput;
			}
			else if(radioAllowWithMessage.Checked) {
				WebSchedCarrierRule.Rule=RuleType.AllowWithMessage;
			} 
			else {
				WebSchedCarrierRule.Rule=RuleType.Block;
			}
			WebSchedCarrierRule.Message=PIn.String(textMessage.Text);
			WebSchedCarrierRules.Update(WebSchedCarrierRule);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}