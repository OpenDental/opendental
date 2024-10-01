using System;
using System.Collections.Generic;
using System.Text;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class EobAttachT {

		public static EobAttach CreateEobAttach(DateTime dateTcreated,long cliamPaymentNum,string filmename,string rawBase64="") 
		{
			EobAttach eobAttach=new EobAttach();
			eobAttach.DateTCreated=dateTcreated;
			eobAttach.ClaimPaymentNum=cliamPaymentNum;
			eobAttach.FileName=filmename;
			eobAttach.RawBase64=rawBase64;
			EobAttaches.Insert(eobAttach);
			return eobAttach;
		}

		///<summary>Deletes everything from the eobattach table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearEobAttachTable() {
			string command="DELETE FROM eobattach WHERE EobAttachNum > 0";
			DataCore.NonQ(command);
		}

	}
}
