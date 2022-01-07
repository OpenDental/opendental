using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>The insbluebookrule table represents an ordered hierarchy of rules that the program will attempt to apply when determining insurance estimates for out of network plans. If the highest priority rule does not produce an estimate, the program attempts to apply the second rule, and so on, until an estimate is obtained.</summary>
	[Serializable]
	public class InsBlueBookRule:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long InsBlueBookRuleNum;
		///<summary>0 based. This rule's priority in the hierarchy of all insbluebookrules. 0 is highest priority.</summary>
		public int ItemOrder;
		///<summary>Enum:InsBlueBookRuleType Types 0 to 3 are for rules that determine estimates by looking at the payment history of an insurance plan, insurance plan group, carrier, or carrier group. Type 4 utilizes fee schedules that are attached to out of network plans that are manually maintained by the user. Type 5 bases estimates off of the UCR fee.</summary>
		public InsBlueBookRuleType RuleType;
		///<summary>The number of years, months, weeks, or days of insurance payment history that will be considered when generating a Blue Book estimate. Will be 0 if the RuleType is 4-ManualBlueBookSchedule or 5-UcrFee as limits do not apply to these rule types.</summary>
		public int LimitValue;
		///<summary>Enum:InsBlueBookRuleLimitType Determines the unit of time that InsBlueBookRule.LimitValue represents. Will be 0-None if the RuleType is 4-ManualBlueBookSchedule or 5-UcrFee as limits do not apply to these rule types.</summary>
		public InsBlueBookRuleLimitType LimitType;

		///<summary>Returns a copy of this InsBlueBookRule.</summary>
		public InsBlueBookRule Copy() {
			return (InsBlueBookRule)this.MemberwiseClone();
		}
	}

	public enum InsBlueBookRuleType {
		///<summary>0 - Insurance Plan</summary>
		[Description("Insurance Plan")]
		InsurancePlan,
		///<summary>1 - Group Number</summary>
		[Description("Group Number")]
		GroupNumber,
		///<summary>2 - Insurance Carrier</summary>
		[Description("Insurance Carrier")]
		InsuranceCarrier,
		///<summary>3 - Insurance Carrier Group</summary>
		[Description("Insurance Carrier Group")]
		InsuranceCarrierGroup,
		///<summary>4 - Manual Blue Book Schedule</summary>
		[Description("Manual Blue Book Fee Schedule")]
		ManualBlueBookSchedule,
		///<summary>5 - UCR Fee</summary>
		[Description("UCR Fee")]
		UcrFee,
	}

	public enum InsBlueBookRuleLimitType {
		///<summary>0 - None</summary>
		[Description("None")]
		None,
		///<summary>1 - Years</summary>
		[Description("Years")]
		Years,
		///<summary>2 - Months</summary>
		[Description("Months")]
		Months,
		///<summary>3 - Weeks</summary>
		[Description("Weeks")]
		Weeks,
		///<summary>4 - Days</summary>
		[Description("Days")]
		Days,
	}

}