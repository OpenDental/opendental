using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
  ///<summary>Ehr. Lab and radiology orders.  Medication orders are simply fields in medicationPat.</summary>
  [Serializable]
  public class MedicalOrder:TableBase {
    ///<summary>Primary key.</summary>
    [CrudColumn(IsPriKey=true)]
    public long MedicalOrderNum;
    ///<summary>Enum:MedicalOrderType Laboratory=0,Radiology=1.</summary>
    public MedicalOrderType MedOrderType;
    ///<summary>FK to patient.PatNum</summary>
    public long PatNum;
    ///<summary>Date and time of order.</summary>
    [CrudColumn(SpecialType=CrudSpecialColType.DateT)]
    public DateTime DateTimeOrder;
		///<summary>User will be required to type entire order out from scratch.</summary>
		public string Description;
		///<summary>EHR requires Active/Discontinued status. 0=Active, 1=Discontinued.</summary>
		public bool IsDiscontinued;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;

    ///<summary></summary>
    public MedicalOrder Copy() {
      return (MedicalOrder)this.MemberwiseClone();
    }
  }

	///<summary></summary>
  public enum MedicalOrderType {
    ///<summary>0- Laboratory</summary>
    Laboratory,
    ///<summary>1- Radiology</summary>
    Radiology
    //<summary>2- Medication</summary>
    //Medication
  }
}