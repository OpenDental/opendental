using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness.Pearl {
//Caries - Progressed or Caries - Incipient based depends if there exists dentin in the relationships metric_value

  ///<summary>This is the one coming from Pearl</summary>
	public enum EnumCategory {
    ///<summary>0 - None.</summary>
    None=0,
    ///<summary>1 - Crown.</summary>
    Crown=1,
    ///<summary>3 - Periapical Radiolucency.</summary>
    PeriapicalRadiolucency=3,
    ///<summary>4 - Filling.</summary>
    Filling=4,
    ///<summary>5 - Anatomy.</summary>
    Anatomy=5,
    ///<summary>6 - Caries.</summary>
    Caries=6,
    ///<summary>7 - Margin Discrepancy.</summary>
    MarginDiscrepancy=7,
    ///<summary>8 - Implant.</summary>
    Implant=8,
    ///<summary>10 - Root Canal.</summary>
    RootCanal=10,
    ///<summary>11 - Bridge.</summary>
    Bridge=11,
    ///<summary>14 - Calculus.</summary>
    Calculus=14,
    ///<summary>23 - Tooth Parts.</summary>
    ToothParts=23,
  }

  ///<summary>This is for showing/hiding layers.</summary>
  public enum EnumCategoryOD {
    ///<summary>0 - None.</summary>
    None=0,
    ///<summary>1 - Crown.</summary>
    Crown=1,
    ///<summary>2 - Periapical Radiolucency.</summary>
    PeriapicalRadiolucency=2,
    ///<summary>3 - Filling.</summary>
    Filling=3,
    ///<summary>4 - Anatomy.</summary>
    Anatomy=4,
    ///<summary>5 - Caries.</summary>
    Caries=5,
    ///<summary>6 - Margin Discrepancy.</summary>
    MarginDiscrepancy=6,
    ///<summary>7 - Implant.</summary>
    Implant=7,
    ///<summary>8 - Root Canal.</summary>
    RootCanal=8,
    ///<summary>9 - Bridge.</summary>
    Bridge=9,
    ///<summary>10 - Calculus.</summary>
    Calculus=10,
    ///<summary>11 - Tooth Parts.</summary>
    ToothParts=11,
    ///<summary>12 - Measurements.</summary>
    Measurements=12,
  }
}
