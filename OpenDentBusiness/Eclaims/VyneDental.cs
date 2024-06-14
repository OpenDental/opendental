using System;
using System.Diagnostics;
using System.IO;
using CodeBase;
using OpenDentBusiness;

namespace OpenDentBusiness.Eclaims {
	///<summary></summary>
	public class VyneDental {

		public static string ErrorMessage="";

		public VyneDental(){}

		///<summary>Returns true if the exe was launched, false otherwise.</summary>
		public static bool Launch(Clearinghouse clearinghouse,int batchNum){//Called from Eclaims.cs. Clinic-level clearinghouse passed in.
			try{
				ODFileUtils.ProcessStart(clearinghouse.ClientProgram,doWaitForODCloudClientResponse:true);
			}
			catch(Exception ex) {
				ErrorMessage=ex.Message;
				return false;
			}
			return true;
		}
	}
}
