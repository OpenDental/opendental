using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using OpenDentBusiness;

namespace OpenDentBusiness.Eclaims {
	/// <summary>
	/// United Kindgdom National Health Service (NHS).
	/// </summary>
	public class NHS {

		public static string ErrorMessage="";//Note for reviewer: I know this will never get used right now but just in case we enhance this later I thought it best to include it.

		///<summary></summary>
		public NHS() {

		}

		///<summary>Returns true if the communications were successful, and false if they failed. If they failed, a rollback will happen automatically by deleting the previously created FP17 file. The batchnum is supplied for the possible rollback.</summary>
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum) {//called from Eclaims.cs. Clinic-level clearinghouse passed in.
			bool retVal=true;


			return retVal;
		}
	}
}
