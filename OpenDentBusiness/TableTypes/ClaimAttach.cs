using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDentBusiness {
	///<summary>Keeps track of one image file attached to a claim.  Multiple files can be attached to a claim using this method.</summary>
	[Serializable()]
	public class ClaimAttach:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ClaimAttachNum;
		///<summary>FK to claim.ClaimNum</summary>
		public long ClaimNum;
		///<summary>The name of the file that shows on the claim.  For example: tooth2.jpg.</summary>
		public string DisplayedFileName;
		///<summary>The actual file is stored in the A-Z folder in EmailAttachments.  (yes, even though it's not actually an email attachment)  The files are named automatically based on Date/time along with a random number.  This ensures that they will be sequential as well as unique.</summary>
		public string ActualFileName;

		public ClaimAttach Copy(){
			return (ClaimAttach)this.MemberwiseClone();
		}
	}




}
