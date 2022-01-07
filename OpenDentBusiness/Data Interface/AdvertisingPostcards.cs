using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bridges;
using CodeBase;

namespace OpenDentBusiness {
	public class AdvertisingPostcards {
		public const string MassPostcardResponseNode="MassPostcardResponse";
		public const string MassPostcardRequestNode="MassPostcardRequestNode";

		///<summary>Gets Postcard data, including accounts for the office, chunk size for uploading lists, and the 'Advertising - Postcards' webpage url.</summary>
		public static PostcardManiaMetaData GetPostcardManiaMetaData() {
			//No need to check RemotingRole; no call to db.
			return WebServiceMainHQProxy.GetAdvertisingPostcardsAccountData();
		}

		///<summary>Create or edit HQ account for 'Advertising - Postcards'. If guid and email already exist in ServicesHQ, then the AccountTitle will be changed.</summary>
		public static PostcardManiaAccountCreateResponse ManageAccount(string accountTitle,string guid,string email,string fName,string lName,string extRefNum="",string company="",string phone="",string mobile="") {
			//No need to check RemotingRole; no call to db.
			PostcardManiaAccountCreateRequest req=new PostcardManiaAccountCreateRequest(accountTitle,guid,email,fName,lName,"",company,extRefNum,phone,mobile);
			return WebServiceMainHQProxy.ManageAdvertisingPostcardsAccount(req);
		}

		///<summary>Gets the SSO for 'Advertising - Postcards' webpage.</summary>
		public static string GetSSO(string guid,string email) {
			//No need to check RemotingRole; no call to db.
			PostcardManiaSSORequest req=new PostcardManiaSSORequest(guid,email,"");
			return WebServiceMainHQProxy.GetAdvertisingPostcardsSSO(req).SSO;
		}

		///<summary>Uploads a patient list through the postcardmania API endpoint.</summary>
		public static PostcardManiaUploadPatientsResponse UploadPatients(string guid,string email,string listName,List<PatientInfo> listPats) {
			//No need to check RemotingRole; no call to db.
			PostcardManiaUploadPatientsRequest req=new PostcardManiaUploadPatientsRequest(guid,listName:listName,email:email,listPatientData:listPats,"");
			return WebServiceMainHQProxy.UploadAdvertisingPostcardsPatientList(req);
		}
	}
}
