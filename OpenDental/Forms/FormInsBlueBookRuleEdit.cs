using System;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsBlueBookRuleEdit:FormODBase {
		///<summary>A copy of the rule passed to the form's constructor that is OK to edit.</summary>
		private InsBlueBookRule _ruleCur;
		///<summary>A copy of the rule passed to the form's constructor for preserving the rule's original state.</summary>
		private InsBlueBookRule _ruleOld;
		///<summary>Allows parent form to access the rule that the user has modified.</summary>
		public InsBlueBookRule RuleCur {
			get => _ruleCur;
		}

		public FormInsBlueBookRuleEdit(InsBlueBookRule rule) {
			_ruleCur=rule.Copy();
			_ruleOld=rule.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsBlueBookRuleEdit_Load(object sender,EventArgs e) {
			textRule.Text=_ruleCur.RuleType.GetDescription();
			textRule.ReadOnly=true;
			listLimitType.Items.Clear();
			listLimitType.Items.AddEnums<InsBlueBookRuleLimitType>();
			listLimitType.SetSelectedEnum(_ruleCur.LimitType);
			textLimitValue.Text=_ruleCur.LimitValue.ToString();
		}

		private void listLimitType_SelectedIndexChanged(object sender,EventArgs e) {
			EnableTextLimitValue();
		}

		///<summary>Enables textLimitValue unless limit type "None" is selected.</summary>
		private void EnableTextLimitValue() {
			textLimitValue.ReadOnly=false;
			if(listLimitType.GetSelected<InsBlueBookRuleLimitType>()==InsBlueBookRuleLimitType.None) {
				textLimitValue.Text="0";
				textLimitValue.ReadOnly=true;
			}
		}

		private void ButOK_Click(object sender,EventArgs e) {
			if(!textLimitValue.ReadOnly && !textLimitValue.IsValid()) {
				MsgBox.Show(this,"Limit value is invalid.");
				return;
			}
			_ruleCur.LimitType=listLimitType.GetSelected<InsBlueBookRuleLimitType>();
			_ruleCur.LimitValue=PIn.Int(textLimitValue.Text);
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			_ruleCur=_ruleOld;
			DialogResult=DialogResult.Cancel;
		}

	}
}