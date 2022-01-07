using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	public class X12Validate {
		///<summary>StringBuilder does not get altered if no invalid data.</summary>
		public static void ISA(Clearinghouse clearinghouseClin,StringBuilder strb) {
			if(clearinghouseClin.ISA05!="01" && clearinghouseClin.ISA05!="14" && clearinghouseClin.ISA05!="20" && clearinghouseClin.ISA05!="27" 
				&& clearinghouseClin.ISA05!="28"	&& clearinghouseClin.ISA05!="29" && clearinghouseClin.ISA05!="30" && clearinghouseClin.ISA05!="33"
				&& clearinghouseClin.ISA05!="ZZ")
			{
				Comma(strb);
				strb.Append("Clearinghouse ISA05");
			}
			if(clearinghouseClin.SenderTIN!="") {//if it IS blank, then we'll be using OD's info as the sender, so no need to validate the rest
				if(clearinghouseClin.SenderTIN.Length<2) {
					Comma(strb);
					strb.Append("Clearinghouse SenderTIN");
				}
				if(clearinghouseClin.SenderName=="") {//1000A NM103 min length=1
					Comma(strb);
					strb.Append("Clearinghouse Sender Name");
				}
				if(!Regex.IsMatch(clearinghouseClin.SenderTelephone,@"^\d{10}$")) {//1000A PER04 min length=1
					Comma(strb);
					strb.Append("Clearinghouse Sender Phone");
				}
			}
			if(clearinghouseClin.ISA07!="01" && clearinghouseClin.ISA07!="14" && clearinghouseClin.ISA07!="20" && clearinghouseClin.ISA07!="27" 
				&& clearinghouseClin.ISA07!="28"	&& clearinghouseClin.ISA07!="29" && clearinghouseClin.ISA07!="30" && clearinghouseClin.ISA07!="33"
				&& clearinghouseClin.ISA07!="ZZ") 
			{
				Comma(strb);
				strb.Append("Clearinghouse ISA07");
			}
			if(clearinghouseClin.ISA08.Length<2) {
				Comma(strb);
				strb.Append("Clearinghouse ISA08");
			}
			if(clearinghouseClin.ISA15!="T" && clearinghouseClin.ISA15!="P") {
				Comma(strb);
				strb.Append("Clearinghouse ISA15");
			}
		}

		///<summary>StringBuilder does not get altered if no invalid data.</summary>
		public static void Carrier(Carrier carrier,StringBuilder strb,string msgPrepend="") {
			if(carrier.Address.Trim()=="") {
				Comma(strb);
				strb.Append(msgPrepend+"Carrier Address");
			}
			if(carrier.City.Trim().Length<2) {
				Comma(strb);
				strb.Append(msgPrepend+"Carrier City");
			}
			if(carrier.State.Trim().Length!=2) {
				Comma(strb);
				strb.Append(msgPrepend+"Carrier State(2 char)");
			}
			if(!Regex.IsMatch(carrier.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append(msgPrepend+"Carrier Zip");
			}
		}

		///<summary>StringBuilder does not get altered if no invalid data.</summary>
		public static void BillProv(Provider billProv,StringBuilder strb) {
			if(billProv.LName=="") {
				Comma(strb);
				strb.Append("Billing Prov LName");
			}
			if(!billProv.IsNotPerson && billProv.FName=="") {//if is a person, first name cannot be blank.
				Comma(strb);
				strb.Append("Billing Prov FName");
			}
			if(!Regex.IsMatch(billProv.SSN,"^[0-9]{9}$")) {//must be exactly 9 in length (no dashes)
				Comma(strb);
				strb.Append("Billing Prov SSN/TIN must be a 9 digit number");
			}
			if(!Regex.IsMatch(billProv.NationalProvID,"^(80840)?[0-9]{10}$")) {
				Comma(strb);
				strb.Append("Billing Prov NPI must be a 10 digit number with an optional prefix of 80840");
			}
			if(billProv.TaxonomyCodeOverride.Length>0 && billProv.TaxonomyCodeOverride.Length!=10) {
				Comma(strb);
				strb.Append("Billing Prov Taxonomy Code must be 10 characters");
			}
			//Always check provider name variables regardless of IsNotPerson.
			if(!billProv.IsNotPerson) {//The first name and middle name are only required if the billing provider is a person. For an organization, these fields are never sent.
				string fNameInvalidChars=GetNonAN(billProv.FName);
				if(fNameInvalidChars!="") {
					Comma(strb);
					strb.Append("Billing Prov First Name contains invalid characters: "+fNameInvalidChars);
				}
				string middleInvalidChars=GetNonAN(billProv.MI);
				if(middleInvalidChars!="") {
					Comma(strb);
					strb.Append("Billing Prov MI contains invalid characters: "+middleInvalidChars);
				}
			}
			string lNameInvalidChars=GetNonAN(billProv.LName);
			if(lNameInvalidChars!="") {
				Comma(strb);
				strb.Append("Billing Prov Last Name contains invalid characters: "+lNameInvalidChars);
			}
			/* This was causing problems when dummy providers were used for office and no license number was applicable.
			 * Delta carriers key off this number and start assigning to wrong provider. Customer: ATD.
			if(billProv.StateLicense=="") {
				if(strb.Length!=0) {
					strb.Append(",");
				}
				strb.Append("Billing Prov Lic #");
			}*/
		}

		public static void PracticeAddress(StringBuilder strb) {
			if(PrefC.GetString(PrefName.PracticePhone).Length!=10) {
				//10 digit phone is required by WebMD (Emdeon Dental) and is universally assumed
				Comma(strb);
				strb.Append("Practice Phone");
			}
			if(PrefC.GetString(PrefName.PracticeAddress).Trim()=="") {
				Comma(strb);
				strb.Append("Practice Address");
			}
			if(PrefC.GetString(PrefName.PracticeCity).Trim().Length<2) {
				Comma(strb);
				strb.Append("Practice City");
			}
			if(PrefC.GetString(PrefName.PracticeST).Trim().Length!=2) {
				Comma(strb);
				strb.Append("Practice State(2 char)");
			}
			if(!Regex.IsMatch(PrefC.GetString(PrefName.PracticeZip).Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append("Practice Zip");
			}
		}

		public static void BillingAddress(StringBuilder strb) {
			if(PrefC.GetString(PrefName.PracticePhone).Length!=10) { //There is no billing phone, so the practice phone is sent electronically.
				//10 digit phone is required by WebMD and is universally assumed 
				Comma(strb);
				strb.Append("Practice Phone");
			}
			if(PrefC.GetString(PrefName.PracticeBillingAddress).Trim()=="") {
				Comma(strb);
				strb.Append("Practice Billing Address");
			}
			if(PrefC.GetString(PrefName.PracticeBillingCity).Trim().Length<2) {
				Comma(strb);
				strb.Append("Practice Billing City");
			}
			if(PrefC.GetString(PrefName.PracticeBillingST).Trim().Length!=2) {
				Comma(strb);
				strb.Append("Practice Billing State(2 char)");
			}
			if(!Regex.IsMatch(PrefC.GetString(PrefName.PracticeBillingZip).Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {
				//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append("Practice Billing Zip");
			}
		}

		///<summary>Clinic passed in must not be null.</summary>
		public static void Clinic(Clinic clinic,StringBuilder strb) {
			if(clinic.UseBillAddrOnClaims) { //If we're using billing address, check the clinic's billing info for validity.
				if(clinic.BillingCity.Trim().Length<2) {
					Comma(strb);
					strb.Append("Clinic Billing City");
				}
				if(clinic.BillingState.Trim().Length!=2) {
					Comma(strb);
					strb.Append("Clinic Billing State(2 char)");
				}
				if(!Regex.IsMatch(clinic.BillingZip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {
					//#####, or #####-, or #####-####, or #########.  Dashes are removed when X12 is generated.
					Comma(strb);
					strb.Append("Clinic Billing Zip");
				}
			}
			else { //If we're not using billing address, check the clinic's regular info for validity.
				if(clinic.Address.Trim()=="") {
					Comma(strb);
					strb.Append("Clinic Address");
				}
				if(clinic.City.Trim().Length<2) {
					Comma(strb);
					strb.Append("Clinic City");
				}
				if(clinic.State.Trim().Length!=2) {
					Comma(strb);
					strb.Append("Clinic State(2 char)");
				}
				if(!Regex.IsMatch(clinic.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {
					//#####, or #####-, or #####-####, or #########.  Dashes are removed when X12 is generated.
					Comma(strb);
					strb.Append("Clinic Zip");
				}
			}
			if(clinic.Phone.Length!=10) {//1000A PER04 min length=1.
				//But 10 digit phone is required in 2010AA and is universally assumed 
				Comma(strb);
				strb.Append("Clinic Phone");
			}
		}

		///<summary>Just subscriber address for now. Other fields (ex subscriber id) are checked elsewhere. We might want to move all subscriber checks here some day.</summary>
		public static void Subscriber(Patient subscriber,StringBuilder strb) {
			if(subscriber.Address.Trim()=="") {
				Comma(strb);
				strb.Append("Subscriber Address");
			}
			if(subscriber.City.Trim()=="") {
				Comma(strb);
				strb.Append("Subscriber City");
			}
			if(subscriber.State.Trim()=="") {
				Comma(strb);
				strb.Append("Subscriber State");
			}
			if(!Regex.IsMatch(subscriber.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append("Subscriber Zip");
			}
		}

		///<summary>Just subscriber address for now. Other fields (ex subscriber id) are checked elsewhere. We might want to move all subscriber checks here some day.</summary>
		public static void Subscriber2(Patient subscriber2,StringBuilder strb) {
			if(subscriber2.Address.Trim()=="") {
				Comma(strb);
				strb.Append("Secondary Subscriber Address");
			}
			if(subscriber2.City.Trim()=="") {
				Comma(strb);
				strb.Append("Secondary Subscriber City");
			}
			if(subscriber2.State.Trim()=="") {
				Comma(strb);
				strb.Append("Secondary Subscriber State");
			}
			if(!Regex.IsMatch(subscriber2.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append("Secondary Subscriber Zip");
			}
		}

		private static void Comma(StringBuilder strb){
			if(strb.Length!=0) {
				strb.Append(",");
			}
		}

		///<summary>Returns a string containing all characters not in the Basic Character Set from the given input.  AN stands for alphanumeric.</summary>
		private static string GetNonAN(string str) {
			string strUpper=str.ToUpper();//All strings in our X12s are set to uppercase.
			//Basic Character Set, includes those selected from the uppercase letters, digits, space, and special characters as specified below.
			string validChars="ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!\"&'()*+,-./:;?= ";
			StringBuilder retVal=new StringBuilder();
			for(int i=0;i<strUpper.Length;i++) {
				if(validChars.IndexOf(strUpper[i])==-1) {//Not found.
					retVal.Append(strUpper[i]);
				}
			}
			return retVal.ToString();
		}




	}
}
