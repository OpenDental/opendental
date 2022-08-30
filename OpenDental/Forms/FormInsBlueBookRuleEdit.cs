using System;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormInsBlueBookRuleEdit:FormODBase {
		///<summary>A copy of the rule passed to the form's constructor that is OK to edit.</summary>
		private InsBlueBookRule _insBlueBookRule;
		///<summary>A copy of the rule passed to the form's constructor for preserving the rule's original state.</summary>
		private InsBlueBookRule _insBlueBookRuleOld;

		public FormInsBlueBookRuleEdit(InsBlueBookRule insBlueBookRule) {
			_insBlueBookRule=insBlueBookRule.Copy();
			_insBlueBookRuleOld=insBlueBookRule.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsBlueBookRuleEdit_Load(object sender,EventArgs e) {
			textRule.Text=_insBlueBookRule.RuleType.GetDescription();
			textRule.ReadOnly=true;
			listLimitType.Items.Clear();
			listLimitType.Items.AddEnums<InsBlueBookRuleLimitType>();
			listLimitType.SetSelectedEnum(_insBlueBookRule.LimitType);
			textLimitValue.Text=_insBlueBookRule.LimitValue.ToString();
		}

		private void listLimitType_SelectedIndexChanged(object sender,EventArgs e) {
			EnableTextLimitValue();
		}

			///<summary>Allows parent form to access the rule that the user has modified.</summary>
		public InsBlueBookRule GetInsBlueBookRuleCur() {
			return _insBlueBookRule;
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
			_insBlueBookRule.LimitType=listLimitType.GetSelected<InsBlueBookRuleLimitType>();
			_insBlueBookRule.LimitValue=PIn.Int(textLimitValue.Text);
			DialogResult=DialogResult.OK;
		}

		private void ButCancel_Click(object sender,EventArgs e) {
			_insBlueBookRule=_insBlueBookRuleOld;
			DialogResult=DialogResult.Cancel;
		}
	}
}