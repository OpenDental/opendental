using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness{
    ///<summary></summary>
    [Serializable]
    public class CloudAddress:TableBase{
      ///<summary>Primary key.</summary>
      [CrudColumn(IsPriKey=true)]
      public long CloudAddressNum;
      ///<summary>The IP address the user is connecting from.</summary>
      public string IpAddress;
      ///<summary>FK to userod.UserNum.</summary>
      public long UserNumLastConnect;
      ///<summary>DateTime of the last successful login from the address.</summary>
      [CrudColumn(SpecialType=CrudSpecialColType.DateT)]
      public DateTime DateTimeLastConnect;

      ///<summary></summary>
      public CloudAddress Copy() {
          return (CloudAddress)this.MemberwiseClone();
      }
   }
}