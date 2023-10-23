using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace WpfControls.UI {//using this namespace for now so that it's easily available in Frms but doesn't conflict with the other one in OD.exe
	public class DataValid {
		///<summary>FormOpenDental subscribes to this event. Frms fire this event to cause event handler to run.</summary>
		public static event EventHandler<InvalidType[]> EventInvalid;

		public static void SetInvalid(params InvalidType[] invalidTypeArray) {
			EventInvalid?.Invoke(null,invalidTypeArray);
		}
	}
}
