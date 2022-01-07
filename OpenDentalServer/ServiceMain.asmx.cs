using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Reflection;
using System.Web.Hosting;
using System.Web.Services;
using CodeBase;
using OpenDentBusiness;

namespace OpenDentalServer {
	/// <summary></summary>
	[WebService(Namespace="http://www.open-dent.com/OpenDentalServer")]
	[WebServiceBinding(ConformsTo=WsiProfiles.BasicProfile1_1)]
	[ToolboxItem(false)]
	public class ServiceMain:System.Web.Services.WebService {

		///<summary>Pass in a serialized dto.  It returns a dto which must be deserialized by the client.</summary>
		[WebMethod]
		public string ProcessRequest(string dtoString) {
			return DtoProcessor.ProcessDto(dtoString,Server.MapPath("."));
		}

	}
}
