using System;
using System.Collections;
using System.Drawing;

namespace OpenDentBusiness {
	///<summary>Can also be a CCR.  Received CCDs/CCRs are stored both here and in emailattach.  Sent CCDs are not saved here, but are only stored in emailattach.  To display a saved Ccd, it is combined with an internal stylesheet.</summary>
	[Serializable]
	public class EhrSummaryCcd:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EhrSummaryCcdNum;
		///<summary>FK to patient.PatNum.</summary>
		public long PatNum;
		///<summary>Date that this Ccd was received.</summary>
		public DateTime DateSummary;
		///<summary>The xml content of the received text file.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ContentSummary;
		///<summary>FK to emailattach.EmailAttachNum.  The Direct email attachment where the CCD xml message came from.  Needed to sync PatNum with the email PatNum if the PatNum is changed on the email.</summary>
		public long EmailAttachNum;

		///<summary></summary>
		public EhrSummaryCcd Copy() {
			return (EhrSummaryCcd)MemberwiseClone();
		}

	}

	

}
