using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDentBusiness.Eclaims {
	///<summary>Handles all 270/270 logic.  Contains UI elements.  Passes off the 270 to the correct clearinghouse.</summary>
	public class x270Controller {
		///<summary>Only used by unit tests to spoof a fake 271 response.</summary>
		public static string FakeResponseOverride271="";

		///<summary>Throws exceptions. The insplan that's passed in need not be properly updated to the database first.</summary>
		///<returns>The Etrans created from the request. Will be null if the request failed in any way.</returns>
		public static Etrans RequestBenefits(Clearinghouse clearinghouseClin,InsPlan plan,long patNum,Carrier carrier,InsSub insSub,out string error) {
			error="";
			Patient pat=Patients.GetPat(patNum);
			Patient subsc=Patients.GetPat(insSub.Subscriber);
			Clinic clinic=Clinics.GetClinic(pat.ClinicNum);
			Provider billProv=Providers.GetProv(Providers.GetBillingProvNum(pat.PriProv,pat.ClinicNum));
			//validation.  Throw exception if missing info----------------------------------------
			string validationResult=X270.Validate(clearinghouseClin,carrier,billProv,clinic,plan,subsc,insSub,pat);
			if(validationResult != "") {
				throw new Exception(Lans.g("FormInsPlan","Please fix the following errors first:")+"\r\n"+validationResult);
			}
			//create a 270 message---------------------------------------------------------------
			string x12message=X270.GenerateMessageText(clearinghouseClin,carrier,billProv,clinic,plan,subsc,insSub,pat);
			EtransMessageText etransMessageText=new EtransMessageText();
			etransMessageText.MessageText=x12message;
			EtransMessageTexts.Insert(etransMessageText);
			//attach it to an etrans-------------------------------------------------------------
			Etrans etrans=new Etrans();
			etrans.PatNum=patNum;
			etrans.DateTimeTrans=DateTime.Now;
			etrans.ClearingHouseNum=clearinghouseClin.HqClearinghouseNum;
			etrans.Etype=EtransType.BenefitInquiry270;
			etrans.PlanNum=plan.PlanNum;
			etrans.InsSubNum=insSub.InsSubNum;
			etrans.EtransMessageTextNum=etransMessageText.EtransMessageTextNum;
			Etranss.Insert(etrans);
			//send the 270----------------------------------------------------------------------
			string x12response="";
			Etrans etransHtml=null;
			//a connection error here needs to bubble up
			try {
				if(!String.IsNullOrWhiteSpace(FakeResponseOverride271)) {
					x12response=FakeResponseOverride271;
				}
				else if(clearinghouseClin.CommBridge==EclaimsCommBridge.ClaimConnect) {
					x12response=ClaimConnect.Benefits270(clearinghouseClin,x12message);
				}
				else if(clearinghouseClin.CommBridge==EclaimsCommBridge.EDS) {
					x12response=EDS.Benefits270(clearinghouseClin,x12message,out etransHtml);
				}
				else if(clearinghouseClin.CommBridge==EclaimsCommBridge.WebMD) {
					x12response=WebMD.Benefits270(clearinghouseClin,x12message);
				}
				object[] parameters={clearinghouseClin,x12message,x12response};
				Plugins.HookAddCode(null,"x270Controller.RequestBenefits_benefits270",parameters);
				x12response=(string)parameters[2];
			}
			catch(Exception ex) {
				EtransMessageTexts.Delete(etrans.EtransMessageTextNum);
				Etranss.Delete(etrans.EtransNum);
				throw new ApplicationException(Lans.g("FormInsPlan","Connection Error:")+"\r\n"+ex.GetType().Name+"\r\n"+ex.Message);
			}
			//start to process the 271----------------------------------------------------------
			X271 x271=null;
			if(X12object.IsX12(x12response)) {
				X12object x12obj=new X12object(x12response);
				if(x12obj.Is271()) {
					x271=new X271(x12response);
				}
			}
			else {//not a 997, 999, 277 or 271
				EtransMessageTexts.Delete(etrans.EtransMessageTextNum);
				Etranss.Delete(etrans.EtransNum);
				throw new ApplicationException(Lans.g("FormInsPlan","Error:")+"\r\n"+x12response);
			}
			/*
			//In realtime mode, X12 limits the request to one patient.
			//We will always use the subscriber.
			//So all EB segments are for the subscriber.
			List<EB271> listEB=new List<EB271>();
			EB271 eb;
			if(x271 != null) {
				for(int i=0;i<x271.Segments.Count;i++) {
					if(x271.Segments[i].SegmentID != "EB") {
						continue;
					}
					eb=new EB271(x271.Segments[i]);
					listEB.Add(eb);
				}
			}*/
			//create an etrans for the 271------------------------------------------------------
			etransMessageText=new EtransMessageText();
			etransMessageText.MessageText=x12response;
			EtransMessageTexts.Insert(etransMessageText);
			Etrans etrans271=new Etrans();
			etrans271.PatNum=patNum;
			etrans271.DateTimeTrans=DateTime.Now;
			etrans271.ClearingHouseNum=clearinghouseClin.HqClearinghouseNum;
			etrans271.Etype=EtransType.TextReport;
			if(X12object.IsX12(x12response)) {//this shouldn't need to be tested because it was tested above.
				if(x271==null){
					X12object Xobj=new X12object(x12response);
					if(Xobj.Is997()) {
						etrans271.Etype=EtransType.Acknowledge_997;
					}
					else if(Xobj.Is999()) {
						etrans271.Etype=EtransType.Acknowledge_999;
					}
					else if(X277.Is277(Xobj)) {
						etrans271.Etype=EtransType.StatusNotify_277;
					}
					else if(X835.Is835(Xobj)) {
						etrans271.Etype=EtransType.ERA_835;
					}
					else if(Xobj.IsAckInterchange()) {
						etrans271.Etype=EtransType.Ack_Interchange;
					}
				}
				else{
					etrans271.Etype=EtransType.BenefitResponse271;
				}
			}
			etrans271.PlanNum=plan.PlanNum;
			etrans271.InsSubNum=insSub.InsSubNum;
			etrans271.EtransMessageTextNum=etransMessageText.EtransMessageTextNum;
			etrans271.MessageText=etransMessageText.MessageText;//Not a DB column, used to save queries for some calling methods (OpenDentalService).
			if(etransHtml!=null) {
				etrans271.AckEtransNum=etransHtml.EtransNum;
			}
			Etranss.Insert(etrans271);
			etrans.AckEtransNum=etrans271.EtransNum;
			etrans.AckEtrans=etrans271;//Not a DB column, used to save queries for some calling methods (OpenDentalService).
			if(etrans271.Etype==EtransType.Acknowledge_997) {
				X997 x997=new X997(x12response);
				string error997=x997.GetHumanReadable();
				etrans.Note="Error: "+error997;//"Malformed document sent.  997 error returned.";
				Etranss.Update(etrans);
				error=etrans.Note;
				return null;
			}
			else if(etrans271.Etype==EtransType.Acknowledge_999) {
				X999 x999=new X999(x12response);
				string error999=x999.GetHumanReadable();
				etrans.Note="Error: "+error999;//"Malformed document sent.  999 error returned.";
				Etranss.Update(etrans);
				error=etrans.Note;
				return null;
			}
			else if(etrans271.Etype==EtransType.StatusNotify_277) { 
				X277 x277=new X277(x12response);
				string error277=x277.GetHumanReadable();
				etrans.Note="Error: "+error277;//"Malformed document sent.  277 error returned.";
				Etranss.Update(etrans);
				error=etrans.Note;
				return null;
			}
			else if(etrans271.Etype==EtransType.ERA_835) {
				X835 x835=new X835(etrans271,x12response,"");
				string error835=x835.GetHumanReadable();
				etrans.Note="Error: "+error835;//"Malformed document sent.  835 error returned.";
				Etranss.Update(etrans);
				error=etrans.Note;
				return null;
			}
			else if(etrans271.Etype==EtransType.BenefitResponse271) { //271
				string processingerror=x271.GetProcessingError();
				if(processingerror != "") {
					etrans.Note=processingerror;
					Etranss.Update(etrans);
					error=etrans.Note;
					return null;
				}
				else {
					etrans.Note="Normal 271 response.";//change this later to be explanatory of content.
				}
			}
			else if(etrans271.Etype==EtransType.Ack_Interchange) {//See document "X092 Elig 270-271.pdf" pages 388 and 401.
				X12object xobj=new X12object(x12response);
				X12Segment segTa1=xobj.GetNextSegmentById(0,"TA1");
				if(segTa1.Get(4)=="A") {
					etrans.Note="The request was accepted, but the response is empty.";
				}
				else {
					if(segTa1.Get(4)=="E") {
						etrans.Note="The request was accepted with errors: ";
					}
					else if(segTa1.Get(4)=="R") {
						etrans.Note="The request was rejected with errors: ";
					}
					switch(segTa1.Get(5)) {
						case "000": etrans.Note+="No error"; break;
						case "001": etrans.Note+="The Interchange Control Number in the Header and Trailer Do Not Match. "
							+"The Value From the Header is Used in the Acknowledgment."; break;
						case "002": etrans.Note+="This Standard as Noted in the Control Standards Identifier is Not Supported."; break;
						case "003": etrans.Note+="This Version of the Controls is Not Supported"; break;
						case "004": etrans.Note+="The Segment Terminator is Invalid"; break;
						case "005": etrans.Note+="Invalid Interchange ID Qualifier for Sender"; break;
						case "006": etrans.Note+="Invalid Interchange Sender ID"; break;
						case "007": etrans.Note+="Invalid Interchange ID Qualifier for Receiver"; break;
						case "008": etrans.Note+="Invalid Interchange Receiver ID"; break;
						case "009": etrans.Note+="Unknown Interchange Receiver ID"; break;
						case "010": etrans.Note+="Invalid Authorization Information Qualifier Value"; break;
						case "011": etrans.Note+="Invalid Authorization Information Value"; break;
						case "012": etrans.Note+="Invalid Security Information Qualifier Value"; break;
						case "013": etrans.Note+="Invalid Security Information Value"; break;
						case "014": etrans.Note+="Invalid Interchange Date Value"; break;
						case "015": etrans.Note+="Invalid Interchange Time Value"; break;
						case "016": etrans.Note+="Invalid Interchange Standards Identifier Value"; break;
						case "017": etrans.Note+="Invalid Interchange Version ID Value"; break;
						case "018": etrans.Note+="Invalid Interchange Control Number Value"; break;
						case "019": etrans.Note+="Invalid Acknowledgment Requested Value"; break;
						case "020": etrans.Note+="Invalid Test Indicator Value"; break;
						case "021": etrans.Note+="Invalid Number of Included Groups Value"; break;
						case "022": etrans.Note+="Invalid Control Structure"; break;
						case "023": etrans.Note+="Improper (Premature) End-of-File (Transmission)"; break;
						case "024": etrans.Note+="Invalid Interchange Content (e.g., Invalid GS Segment)"; break;
						case "025": etrans.Note+="Duplicate Interchange Control Number"; break;
						case "026": etrans.Note+="Invalid Data Element Separator"; break;
						case "027": etrans.Note+="Invalid Component Element Separator"; break;
						case "028": etrans.Note+="Invalid Delivery Date in Deferred Delivery Request"; break;
						case "029": etrans.Note+="Invalid Delivery Time in Deferred Delivery Request"; break;
						case "030": etrans.Note+="Invalid Delivery Time Code in Deferred Delivery Request"; break;
						case "031": etrans.Note+="Invalid Grade of Service Code"; break;
					}
				}				
			}
			else {
				throw new Exception("Unknown response");
			}
			Etranss.Update(etrans);
			return etrans;
		}

		///<summary>Attempts to request benefits.  If successful the 270 request is returned.
		///Otherwise null is returned and the out error string will contain more information.  Mimics FormInsPlan.butGetElectronic_Click().</summary>
		public static Etrans TryInsVerifyRequest(InsVerify insVerify,InsPlan insPlan,Carrier carrier,InsSub insSub,out string error) {
			error="";
			Etrans etrans270Request=null;
			Clearinghouse clearinghouseHq=Clearinghouses.GetDefaultEligibility();
			if(clearinghouseHq==null) {
				error="No clearinghouse is set as default.";
				return null;
			}
			if(!ListTools.In(clearinghouseHq.CommBridge,EclaimsCommBridge.ClaimConnect,EclaimsCommBridge.EDS,EclaimsCommBridge.WebMD)
				&& !Plugins.HookMethod(null,"x270Controller.TryInsVerifyRequest_is270Supported",clearinghouseHq))
			{
				error="So far, eligibility checks only work with ClaimConnect, EDS, WebMD (Emdeon Dental), and CDAnet.";
				object[] parameters={error};
				Plugins.HookAddCode(null,"x270Controller.TryInsVerifyRequest_270NotSupportedError",parameters);
				error=(string)parameters[0];
				return null;
			}
			error=X271.ValidateSettings();
			if(error.IsNullOrEmpty()) {
				Clearinghouse clearinghouse=Clearinghouses.OverrideFields(clearinghouseHq,insVerify.ClinicNum);//ClinicNum pulled from appointment.
				try {
					//Can return null, can throw exceptions
					etrans270Request=x270Controller.RequestBenefits(clearinghouse,insPlan,insVerify.PatNum,carrier,insSub,out error);
				}
				catch(Exception ex) {
					error=ex.Message;
				}
			}
			return etrans270Request;
		}
	}
}
