using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;

namespace OpenDentBusiness.Eclaims {
	class Lantek {
		public static string ErrorMessage="";
		public Lantek() {
			
		}
		public static bool Launch(Clearinghouse clearinghouseClin,int batchNum){ //called from Eclaims.cs. Clinic-level clearinghouse passed in.
			try{
				//call the client program
				ODFileUtils.ProcessStart(clearinghouseClin.ClientProgram,doWaitForODCloudClientResponse:true);
			}
			catch(Exception ex){
				//X12.Rollback(clearinghouseClin,batchNum);//doesn't actually do anything
				ErrorMessage=ex.Message;
				return false;
			}
			return true;
		}
	}
}
