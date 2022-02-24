using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace OpenDentBusiness{

	///<summary>A lab case.</summary>
	[Serializable()]
	public class LabCase:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long LabCaseNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>FK to laboratory.LaboratoryNum. The lab that the case gets sent to.  Required.</summary>
		public long LaboratoryNum;
		///<summary>FK to appointment.AptNum.  This is how a lab case is attached to a scheduled appointment. 1:1 relationship for now.  Only one labcase per appointment, and (obviously) only one appointment per labcase.  Labcase can exist without being attached to any appointments at all, making this zero.</summary>
		public long AptNum;
		///<summary>FK to appointment.AptNum.  This is how a lab case is attached to a planned appointment in addition to the scheduled appointment.</summary>
		public long PlannedAptNum;
		///<summary>The due date that is put on the labslip.  NOT when you really need the labcase back, which is usually a day or two later and is the date of the appointment this case is attached to.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeDue;
		///<summary>When this lab case was created. User can edit.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeCreated;
		///<summary>Time that it actually went out to the lab.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeSent;
		///<summary>Date/time received back from the lab.  If this is filled, then the case is considered received.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeRecd;
		///<summary>Date/time that quality was checked.  It is now completely ready for the patient.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeChecked;
		///<summary>FK to provider.ProvNum.</summary>
		public long ProvNum;
		///<summary>The text instructions for this labcase.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Instructions;
		///<summary>This is used for tracking and informational purposes only. The fee is not used in any calculation.</summary>
		public double LabFee;
		///<summary>Automatically updated whenever a row is added or changed.  Not user editable.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TimeStamp)]
		public DateTime DateTStamp;
		///<summary>Optional invoice number</summary>
		public string InvoiceNum;

		public LabCase Copy(){
			return (LabCase)this.MemberwiseClone();
		}
		
	}
	
	
	

}













