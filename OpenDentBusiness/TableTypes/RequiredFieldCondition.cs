using System;

namespace OpenDentBusiness {
	///<summary>When one of these conditions is true, the corresponding requiredfield will be triggered.</summary>
	[Serializable]
	public class RequiredFieldCondition:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long RequiredFieldConditionNum;
		///<summary>FK to requiredfield.RequiredFieldNum.</summary>
		public long RequiredFieldNum;
		///<summary>Enum:RequiredFieldName </summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public RequiredFieldName ConditionType;
		///<summary>Enum:ConditionOperator . The operator that is being applied to the ConditionType.</summary>
		public ConditionOperator Operator;
		///<summary>The value that the condition is being compared against. Could be 18, Fulltime, Male, etc.</summary>
		public string ConditionValue;
		///<summary>Enum:LogicalOperator 0-None,1-And,2-Or. This field is only used when comparing continuous values such as age or date.</summary>
		public LogicalOperator ConditionRelationship;

		///<summary></summary>
		public RequiredFieldCondition Clone() {
			return (RequiredFieldCondition)this.MemberwiseClone();
		}
	}

	///<summary></summary>
	public enum ConditionOperator {
		///<summary>0: =</summary>
		Equals,
		///<summary>1: !=</summary>
		NotEquals,
		///<summary>2: ></summary>
		GreaterThan,
		///<summary>3: &lt;</summary>
		LessThan,
		///<summary>4: >=</summary>
		GreaterThanOrEqual,
		///<summary>5: &lt;=</summary>
		LessThanOrEqual
	}

	///<summary></summary>
	public enum LogicalOperator {
		///<summary>0</summary>
		None,
		///<summary>1</summary>
		And,
		///<summary>2</summary>
		Or
	}
}
