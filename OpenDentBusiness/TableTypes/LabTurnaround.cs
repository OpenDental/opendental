using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{

	///<summary>The amount of time it takes for a lab case to be processed at the lab.  Used to compute due dates.</summary>
	[Serializable]
	public class LabTurnaround:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LabTurnaroundNum;
		///<summary>FK to laboratory.LaboratoryNum. The lab that this item is attached to.</summary>
		public long LaboratoryNum;
		///<summary>The description of the service that the lab is performing.</summary>
		public string Description;
		///<summary>The number of days that the lab publishes as the turnaround time for the service.</summary>
		public int DaysPublished;
		///<summary>The actual number of days.  Might be longer than DaysPublished due to travel time.  This is what the actual calculations will be done on.</summary>
		public int DaysActual;

		public LabTurnaround Copy(){
			return (LabTurnaround)this.MemberwiseClone();
		}
		
	}
	
	
	

}













