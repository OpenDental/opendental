using System;
using System.ComponentModel;

namespace OpenDentBusiness {
	///<summary>Used to create overrides at the insurance plan level.</summary>
	[Serializable]
  public class InsPlanPreference:TableBase {
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long InsPlanPrefNum;
    ///<summary>FK to insplan.PlanNum.</summary>
    public long PlanNum;
    ///<summary>FK to to a table associated with FKeyType</summary>
    public long FKey;
    ///<summary>Enum:InsPlanPrefFKeyType ProcCodeNoBillIns</summary>
    public InsPlanPrefFKeyType FKeyType;
    ///<summary>Used to hold the override. NoBillIns stores 0, 1.</summary>
	  [CrudColumn(SpecialType=CrudSpecialColType.IsText)]
    public string ValueString;

    ///<summary></summary>
    public InsPlanPreference Copy() {
      return (InsPlanPreference)this.MemberwiseClone();
    }
  }

  ///<summary>The different types of overrides that can be created for an insurance plan.</summary>
  public enum InsPlanPrefFKeyType {
    ///<summary>0 - Overrides the procedurecode.NoBillIns field at the insurance plan level. FKey stores the CodeNum. ValueString stores NoBillInsOverride enum value.</summary>
    ProcCodeNoBillIns,
  }

   ///<summary>Stores the different override values when FKeyType is set to ProcCodeNoBillIns. Stored as an int</summary>
  public enum NoBillInsOverride {
    ///<summary>0 - Procedures will be billed to insurance if the override exists for the insurance plan and the override value is set to 0.</summary>
    [Description("Bill to Ins")]
    BillToIns,
    ///<summary>1 - Procedures will not be billed to insurance if the override exists for the insurance plan and the override value is set to 1.</summary>
    [Description("Do not usually bill to Ins")]
    DoNotUsuallyBillToIns,
  }

}