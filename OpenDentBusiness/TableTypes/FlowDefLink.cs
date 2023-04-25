using System;

namespace OpenDentBusiness {
	///<summary>There can be multiple FlowDefLinks for each FlowDef. For example, one flowDef could have 4 appointment types as well as a billing type, for a total of 5 FlowDefLinks.  If an appointment has a matching AppointmentType and BillingType, then that flow is used.</summary>
	public class FlowDefLink:TableBase {
		///<summary>PK</summary>
		[CrudColumn(IsPriKey=true)]
		public long FlowDefLinkNum;
		/// <summary>FK to flowdef.FlowDefNum.</summary>
		public long FlowDefNum;
		/// <summary>FK to other tables. Dictated by the FKey Type.</summary>
		public long Fkey;//Future uses might include BillingType, ProvNum, PatStatus, etc.
		/// <summary>Enum:EnumFlowType </summary>
		public EnumFlowType FlowType;

		internal FlowDefLink Copy() {
			return (FlowDefLink)this.MemberwiseClone();
		}
	}

	public enum EnumFlowType {
		///<summary>0. Equivalent to no FlowDefLink.</summary>
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
