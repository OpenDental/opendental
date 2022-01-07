using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	///<summary>This class is for the bridge to DentalXCharge. Specifically, this is for patient credit Score.</summary>
	public class DentalXChange {

		///<summary>Sends data for the DXC credit check program link.</summary>
		///<param name="prog">The program calling this. Should be DXC.</param>
		///<param name="pat">The current patient.</param>
		public static void SendData(Program prog,Patient pat) {
			string url="https://";
			if(ODBuild.IsDebug()) {
				url+="prelive.dentalxchange.com";
			}
			else {
				url+="register.dentalxchange.com";
			}
			url+="/reg/pmslogin";
			Clearinghouse claimConnectClearingHouse=Clearinghouses.GetFirstOrDefault(x => x.CommBridge==EclaimsCommBridge.ClaimConnect);
			if(claimConnectClearingHouse==null) {
				MsgBox.Show("ContrAccount","ClaimConnect clearinghouse not found.");
				return;
			}
			//This is the postdata
			string postData=$"username={ claimConnectClearingHouse.LoginID }&pwd={ claimConnectClearingHouse.Password }&app=pfs&pagename=creditcheck";
			FormWebBrowser browser=new FormWebBrowser(url:url,postData:postData,additionalHeaders:"Content-Type: application/x-www-form-urlencoded\r\n");
			browser.Show();
		}

	}
}







