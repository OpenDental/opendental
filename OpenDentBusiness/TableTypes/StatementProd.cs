using System;

namespace OpenDentBusiness {
  ///<summary>Links production items to a statement. Also tracks whether or not a late charge adjustment has been created for the production item.</summary>
  [Serializable]
  [CrudTable(IsSynchable=true,HasBatchWriteMethods=true)]
  public class StatementProd:TableBase {
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long StatementProdNum;
    ///<summary>FK to statement.StatementNum. The statement that the production item is on.</summary>
    public long StatementNum;
    ///<summary>FK to document.DocNum. The pdf document last associated to the statement. Will be 0 for statements that are sent electronically when pdfs are not saved for electronic statements.</summary>
    public long DocNum;
    ///<summary>Foreign key to linked production item.</summary>
    public long FKey;
    ///<summary>Enum:ProductionType Type of production item.</summary>
    public ProductionType ProdType;
    ///<summary>FK to adjustment.AdjNum. The late charge adjustment made for this production item. 0 if no late charge has been made.</summary>
    public long LateChargeAdjNum;

    ///<summary>Returns a copy of this StatementProd.</summary>
    public StatementProd Copy() {
      return (StatementProd)this.MemberwiseClone();
    }
  }

  public enum ProductionType {
    ///<summary>0 - Procedure</summary>
    Procedure,
    ///<summary>1 - Adjustment</summary>
    Adjustment,
    ///<summary>2 - PayPlanCharge</summary>
    PayPlanCharge,
	}
}