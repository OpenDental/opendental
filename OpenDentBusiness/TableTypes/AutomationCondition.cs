using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace OpenDentBusiness{
	///<summary>Each condition evaluates to true or false.  A series of conditions for a single automation is ANDed together.</summary>
	[Serializable]
	public class AutomationCondition:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long AutomationConditionNum;
		///<summary>FK to automation.AutomationNum. </summary>
		public long AutomationNum;
		///<summary>Enum:AutoCondField </summary>
		public AutoCondField CompareField;
		///<summary>Enum:AutoCondComparison Not all comparisons are allowed with all data types.</summary>
		public AutoCondComparison Comparison;
		///<summary>.</summary>
		public string CompareString;

		///<summary></summary>
		public AutomationCondition Clone() {
			return (AutomationCondition)this.MemberwiseClone();
		}

	}

	///<summary></summary>
	public enum AutoCondField {
		///<summary>Typically specify Equals the exact name/description of the sheet.</summary>
		[Description("Needs Sheet")]
		NeedsSheet,
		///<summary>disease</summary>
		Problem,
		///<summary></summary>
		Medication,
		///<summary></summary>
		Allergy,
		///<summary>Example, 23</summary>
		Age,
		///<summary>Allowed values are M or F, not case sensitive.  Enforce at entry time.</summary>
		Gender,
		///<summary></summary>
		[Description("Lab Result")]
		Labresult,
		///<summary></summary>
		[Description("Insurance Not Effective")]
		InsuranceNotEffective,
		///<summary></summary>
		[Description("Billing Type")]
		BillingType,
		///<summary></summary>
		[Description("Is Proc Required")]
		IsProcRequired,
		///<summary></summary>
		[Description("Is Controlled")]
		IsControlled,
		///<summary></summary>
		[Description("Is Patient Instruction Present")]
		IsPatientInstructionPresent,
	}

	///<summary></summary>
	public enum AutoCondComparison{
		///<summary>Not sensitive to capitalization.</summary>
		Equals,
		///<summary></summary>
		GreaterThan,
		///<summary></summary>
		LessThan,
		///<summary>aka Like</summary>
		Contains,
		///<summary>Should not be displayed to users to choose from.  Used when the condition has one and only one 'comparison' to trigger it.  E.g. ins not effective.</summary>
		None
		//Exists,
		//NotEquals,
		//
	}
}