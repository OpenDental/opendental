using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenDentBusiness {
	///<summary>This is a helper class that allows the real OpenDentalServer.ServiceMain class implement IOpenDentalServer.
	///This also gives us a place to add code in the future if we ever need to add anything to OpenDentalServer.ServiceMain.</summary>
	public class OpenDentalServerReal:OpenDentBusiness.OpenDentalServer.ServiceMain, IOpenDentalServer {
	}
}
