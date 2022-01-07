using System;
using System.Collections.Generic;
using CodeBase;
using System.Linq;

namespace OpenDentBusiness
{
	///<summary>A 271 is the eligibility response to a 270.</summary>
	public class X271:X12object{

		public X271(string messageText):base(messageText){
		
		}

		///<summary>In realtime mode, X12 limits the request to one patient.  We will always use the subscriber.  So all EB segments are for the subscriber.</summary>
		public List<EB271> GetListEB(bool isInNetwork,bool isCoinsuranceInverted) {
			List<EB271> retVal=new List<EB271>();
			EB271 eb=null;
			for(int i=0;i<Segments.Count;i++) {
				//loop until we encounter the first EB
				if(Segments[i].SegmentID!="EB" && eb==null) {
					continue;
				}
				if(Segments[i].SegmentID=="EB") {
					//add the previous eb
					if(eb!=null) {
						retVal.Add(eb);
					}
					X12Segment hsdSeg=null;
					if(Segments[i+1].SegmentID=="HSD") {
						hsdSeg=Segments[i+1];
					}
					//then, start the next one
					eb=new EB271(Segments[i],isInNetwork,isCoinsuranceInverted,hsdSeg);
					continue;
				}
				else if(Segments[i].SegmentID=="SE") {//end of benefits
					retVal.Add(eb);
					break;
				}
				else {//add to existing eb
					eb.SupplementalSegments.Add(Segments[i]);
					continue;
				}
			}
			return retVal;
		}

		public string GetGroupNum() {
			foreach(X12Segment segment in Segments) {
				if(segment.SegmentID=="REF" && segment.Elements[1]=="6P") { //Line beginning "REF*6P* indicates group num
					return segment.Elements[2]; //The group num is the third element in the line
				}
			}
			return "";
		}

		///<summary>Only the DTP segments that come before the EB segments.  X12 loop 2100C.</summary>
		public List<DTP271> GetListDtpSubscriber() {
			List<DTP271> retVal=new List<DTP271>();
			DTP271 dtp;
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].SegmentID=="EB") {
					break;
				}
				if(Segments[i].SegmentID!="DTP") {
					continue;
				}
				dtp=new DTP271(Segments[i]);
				retVal.Add(dtp);
			}
			return retVal;
		}

		///<summary>If there was no processing error (2100A, 2100B, 2100C, 2110C AAA segment), then this will return empty string.</summary>
		public string GetProcessingError() {
			string retVal="";
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].SegmentID!="AAA") {
					continue;
				}
				if(retVal != "") {//if multiple errors
					retVal+=", ";
				}
				retVal+=GetRejectReason(Segments[i].Get(3))+", "
					+GetFollowupAction(Segments[i].Get(4));
			}
			return retVal;
		}

		///<summary>Some of these codes are only found in certain loops.</summary>
		private string GetRejectReason(string code) {
			switch(code) {
				case "04": return "Authorized Quantity Exceeded (too many patients in request)"; 
				case "15": return "Required application data missing";
				case "41": return "Authorization Access Restriction (not allowed to submit requests)"; 
				case "42": return "Unable to Respond at Current Time"; 
				case "43": return "Invalid/Missing Provider Identification";
				case "44": return "Invalid/Missing Provider Name"; 
				case "45": return "Invalid/Missing Provider Specialty";
				case "46": return "Invalid/Missing Provider Phone Number";
				case "47": return "Invalid/Missing Provider State"; 
				case "48": return "Invalid/Missing Referring Provider Identification Number"; 
				case "49": return "Provider is Not Primary Care Physician";
				case "50": return "Provider Ineligible for Inquiries"; 
				case "51": return "Provider Not on File"; 
				case "52": return "Service Dates Not Within Provider Plan Enrollment";
				case "53": return "Inquired Benefit Inconsistent with Provider Type";
				case "54": return "Inappropriate Product/Service ID Qualifier";
				case "55": return "Inappropriate Product/Service ID";
				case "56": return "Inappropriate Date"; 
				case "57": return "Invalid/Missing Date(s) of Service"; 
				case "58": return "Invalid/Missing Date-of-Birth"; 
				case "60": return "Date of Birth Follows Date(s) of Service"; 
				case "61": return "Date of Death Precedes Date(s) of Service"; 
				case "62": return "Date of Service Not Within Allowable Inquiry Period"; 
				case "63": return "Date of Service in Future"; 
				case "64": return "Invalid/Missing Patient ID"; 
				case "65": return "Invalid/Missing Patient Name"; 
				case "66": return "Invalid/Missing Patient Gender Code"; 
				case "67": return "Patient Not Found"; 
				case "68": return "Duplicate Patient ID Number";
				case "69": return "Inconsistent with Patient’s Age";
				case "70": return "Inconsistent with Patient’s Gender";
				case "71": return "Patient Birth Date Does Not Match That for the Patient on the Database"; 
				case "72": return "Invalid/Missing Subscriber/Insured ID"; 
				case "73": return "Invalid/Missing Subscriber/Insured Name"; 
				case "74": return "Invalid/Missing Subscriber/Insured Gender Code"; 
				case "75": return "Subscriber/Insured Not Found"; 
				case "76": return "Duplicate Subscriber/Insured ID Number"; 
				case "77": return "Subscriber Found, Patient Not Found"; 
				case "78": return "Subscriber/Insured Not in Group/Plan Identified";
				case "79": return "Invalid Participant Identification (this payer does not provide e-benefits)";
				case "80": return "No Response received - Transaction Terminated";
				case "97": return "Invalid or Missing Provider Address";
				case "T4": return "Payer Name or Identifier Missing";
				default: return "Error code '"+code+"' not valid.";
			} 
		}

		private string GetFollowupAction(string code) {
			switch(code) {
				case "C": return "Please Correct and Resubmit";
				case "N": return "Resubmission Not Allowed";
				case "P": return "Please Resubmit Original Transaction";
				case "R": return "Resubmission Allowed";
				case "S": return "Do Not Resubmit; Inquiry Initiated to a Third Party";
				case "W": return "Please Wait 30 Days and Resubmit";
				case "X": return "Please Wait 10 Days and Resubmit";
				case "Y": return "Do Not Resubmit; We Will Hold Your Request and Respond Again Shortly";
				default: return "Error code '"+code+"' not valid.";
			}
		}

		///<summary>Returns a non-empty string if there would be a display issue due to invalid settings.
		///Use the result to block the display from the user when needed.</summary>
		public static string ValidateSettings() {
			string validationErrors="";
			Array arrayEbenetitCats=Enum.GetValues(typeof(EbenefitCategory));
			for(int i=0;i<arrayEbenetitCats.Length;i++) {
				EbenefitCategory ebenCat=(EbenefitCategory)arrayEbenetitCats.GetValue(i);
				if(ebenCat==EbenefitCategory.None) {
					continue;
				}
				CovCat covCat=CovCats.GetForEbenCat(ebenCat);
				if(covCat==null) {
					if(validationErrors!="") {
						validationErrors+=", ";
					}
					validationErrors+=ebenCat.ToString();
				}
			}
			if(validationErrors!="") {
				validationErrors="Missing or hidden insurance category for each of the following E-benefits:"+"\r\n"
					+validationErrors+"\r\n"
					+"Go to Setup then Insurance Categories to add or edit.";
			}
			return validationErrors;
		}

		internal bool IsValidForBatchVerification(List<EB271> listBenefits,bool isCoinsuranceInverted,out string errorMsg) {
			errorMsg=this.GetProcessingError();
			if(errorMsg!=""){
				return false;
			}
			if(listBenefits.Count==0){
				errorMsg="No benefits reported.";
				return false;
			}
			else if(listBenefits.Count==1){
				EB271 eb271=listBenefits[0];
				switch(eb271.Segment.Elements[1]){
					case "U"://Contact Following Entity for Information for Eligibility or Benefit Information
						errorMsg="Contact carrier for more information.";//There will be an MSG segment following this.
						X12Segment msgSegment=eb271.SupplementalSegments.FirstOrDefault(x => x.SegmentID=="MSG");
						if(msgSegment!=null){
							errorMsg+="\r\n"+msgSegment.Get(1);
						}
						break;
					//The following codes have not been reported as of yet.
					case "6"://Inactive
					case "7"://Inactive - Pending Eligibility Update
					case "8"://Inactive - Pending Investigation
					case "T"://Card(s) Reported Lost/Stolen
					case "V"://Cannot Process
						errorMsg=eb271.GetEB01Description(eb271.Segment.Elements[1]);//Returns null if given code is not known.
						break;
					default:
						//Intentionally blank, most other EB01 codes are not easily identified as errors.
						break;
				}
			}
			return (errorMsg.IsNullOrEmpty());
		}
	}
}
