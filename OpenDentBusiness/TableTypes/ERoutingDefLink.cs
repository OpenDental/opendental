using System;

namespace OpenDentBusiness {
	///<summary>There can be multiple eRoutingDefLinks for each eRoutingDef. For example, one eRoutingDef could have 4 appointment types as well as a billing type, for a total of 5 eRoutingDefLinks.  If an appointment has a matching AppointmentType and BillingType, then that eRouting is used.</summary>
	public class ERoutingDefLink:TableBase {
		///<summary>PK</summary>
		[CrudColumn(IsPriKey=true)]
		public long ERoutingDefLinkNum;
		/// <summary>FK to eRoutingdef.eRoutingDefNum.</summary>
		public long ERoutingDefNum;
		/// <summary>FK to other tables. Dictated by the FKey Type.</summary>
		public long Fkey;//Future uses might include BillingType, ProvNum, PatStatus, etc.
		/// <summary>Enum:EnumeRoutingType </summary>
		public EnumERoutingType ERoutingType;

		internal ERoutingDefLink Copy() {
			return (ERoutingDefLink)this.MemberwiseClone();
		}
	}

	public enum EnumERoutingType {
		///<summary>0. Equivalent to no eRoutingDefLink.</summary>
		General,
		///<summary>1. Can have an Fkey to AppointmentTypeNum.</summary>
		Appointment,
		///<summary>2. Always an Fkey to DefNum for billing type.</summary>
		BillingType,
		/// <summary>3. Not currently in use.</summary>
		Task,
		/// <summary>4. Not currently in use.</summary>
		TaskList,
		/// <summary>5. Not currenlty in use.</summary>
		Commlog,
		
	}

}
