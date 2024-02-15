using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>Stores information about credit card terminals used for taking payments. Only used for PayConnect.</summary>
	[Serializable()]
	public class PayTerminal:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PayTerminalNum;
		///<summary>User defined name for the payterminal. E.g. Front Desk.</summary>
		public string Name;
		///<summary>FK to clinic.ClinicNum.</summary>
		public long ClinicNum;
		///<summary>Serial number of physical device, typically provided by the card processor.</summary>
		public string TerminalID;
	}
}
