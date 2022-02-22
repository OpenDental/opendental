using System;
using System.Collections;

namespace OpenDentBusiness.Mobile{
	[Serializable()]
	[CrudTable(IsMobile=true)]
	public class Recallm:TableBase{
		
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long RecallNum;
				///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
				///<summary>This is the date that is actually used when doing reports for recall. It will usually be the same as DateDueCalc unless user has changed it. System will only update this field if it is the same as DateDueCalc.  Otherwise, it will be left alone.  Gets cleared along with DateDueCalc when resetting recall.  When setting disabled, this field will also be cleared.  This is the field to use if converting from another software.</summary>
		public DateTime DateDue;
		///<summary>Not editable. Previous date that procedures were done to trigger this recall. It is calculated and enforced automatically.  If you want to affect this date, add a procedure to the chart with a status of C, EC, or EO.</summary>
		public DateTime DatePrevious;
		///<summary>FK to definition.DefNum, or 0 for none.</summary>
		public long RecallStatus;
		///<summary>An administrative note for staff use.</summary>
		public string Note;
		///<summary>If true, this recall type will be disabled (there's only one type right now). This is usually used rather than deleting the recall type from the patient because the program must enforce the trigger conditions for all patients.</summary>
		public bool IsDisabled;
		///<summary>Default is 0.  If a positive number is entered, then the family balance must be less in order for this recall to show in the recall list.</summary>
		public double DisableUntilBalance;
		///<summary>If a date is entered, then this recall will be disabled until that date.</summary>
		public DateTime DisableUntilDate;

		///<summary>Returns a copy of the Recallm.</summary>
    public Recallm Clone(){
			return (Recallm)this.MemberwiseClone();
		}

		
	}
}