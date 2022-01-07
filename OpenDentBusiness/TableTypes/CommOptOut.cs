using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;
using CodeBase;

namespace OpenDentBusiness {
	///<summary>The patient does not want to recieve messages for a particular type of communication.</summary>
	[Serializable]
	[CrudTable(HasBatchWriteMethods=true)]
	public class CommOptOut:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long CommOptOutNum;
		///<summary>FK to patient.PatNum. The patient who is opting out of this form of communication.</summary>
		public long PatNum;
		///<summary>Enum:CommOptOutType The type of communication for which this patient does not want to receive automated sms.</summary>
		public CommOptOutType OptOutSms;
		///<summary>Enum:CommOptOutType The type of communication for which this patient does not want to receive automated email.</summary>
		public CommOptOutType OptOutEmail;

		///<summary></summary>
		public CommOptOut Clone() {
			return (CommOptOut)this.MemberwiseClone();
		}

		public bool IsOptedOut(CommOptOutMode mode,CommOptOutType type) {
			if(type==0) {//None
				return false;
			}
			return mode switch {
				CommOptOutMode.Text => OptOutSms.HasFlag(CommOptOutType.All) || OptOutSms.HasFlag(type),
				CommOptOutMode.Email => OptOutEmail.HasFlag(CommOptOutType.All) || OptOutEmail.HasFlag(type),
				_ => false,
			};
		}
		
	}

	[Flags]
	public enum CommOptOutType  {
		//None - 0 , left out intentionally, implied with value 0 for a [Flags] enum.
		///<summary>1 - All.  Allows adding new entries to this enum without requiring a convert script.</summary>
		All=									0b1,
		///<summary></summary>
		eConfirm=							0b10,
		///<summary></summary> 
		eReminder=						0b100,
		///<summary></summary> 
		eThankYou=						0b1000,
		///<summary></summary> 
		WebSchedRecall=				0b10000,
		///<summary></summary> 
		WebSchedASAP=					0b100000,
		///<summary></summary> 
		PatientPortalInvites=	0b1000000,
		///<summary></summary> 
		Verify=								0b10000000,
		///<summary></summary> 
		Statements=						0b100000000,
		///<summary></summary> 
		Arrivals=							0b1000000000,
		///<summary></summary> 
		Birthdays=						0b10000000000,
		///<summary></summary>
		GeneralMessages=                0b100000000000,
	}

	public enum CommOptOutMode {
		///<summary>0</summary>
		Text,
		///<summary>1</summary>
		Email,
	}
}




