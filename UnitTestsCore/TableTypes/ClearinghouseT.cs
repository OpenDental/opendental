using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenDentBusiness;

namespace UnitTestsCore {
	public class ClearinghouseT {

		///<summary></summary>
		public static Clearinghouse CreateClearinghouse(string description,long clinicNum=0,EclaimsCommBridge commBridge=EclaimsCommBridge.None,
			ElectronicClaimFormat eFormat=ElectronicClaimFormat.None,long hqClearinghouseNum=0,bool isAttachmentSendAllowed=false,string loginID="",string password="",
			string isa05="",string isa07="",string isa08="",string isa15="")
		{
			Clearinghouse clearinghouse=new Clearinghouse() {
				Description=description,
				ClinicNum=clinicNum,
				CommBridge=commBridge,
				Eformat=eFormat,
				HqClearinghouseNum=hqClearinghouseNum,
				IsAttachmentSendAllowed=isAttachmentSendAllowed,
				LoginID=loginID,
				Password=password,
				ISA05=isa05,
				ISA07=isa07,
				ISA08=isa08,
				ISA15=isa15
			};
			Clearinghouses.Insert(clearinghouse);//Automatically sets HqClearinghouseNum.
			if(hqClearinghouseNum > 0) {
				clearinghouse.HqClearinghouseNum=hqClearinghouseNum;
				Clearinghouses.Update(clearinghouse);
			}
			Clearinghouses.RefreshCache();
			return clearinghouse;
		}

		///<summary>Deletes everything from the Clearinghouse table.  Does not truncate the table so that PKs are not reused on accident.</summary>
		public static void ClearClearinghouseTable() {
			string command="DELETE FROM clearinghouse WHERE ClearinghouseNum > 0";
			DataCore.NonQ(command);
		}

	}
}
