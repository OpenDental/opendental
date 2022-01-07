using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenDentBusiness
{
	///<summary>X12 277 Unsolicited Claim Status Notification. There is only one type of 277, but a 277 can be sent out unsolicited (without sending a request) or as a response to a 276 request.</summary>
	public class X277:X12object {

		private List<X12Segment> segments;
		///<summary>NM1 of loop 2100A.</summary>
		private int segNumInfoSourceNM101;
		///<summary>NM1 of loop 2100B.</summary>
		private int segNumInfoReceiverNM101;
		///<summary>NM1 of loop 2100C.</summary>
		private List<int> segNumsBillingProviderNM1;
		///<summary>NM1 of loop 2100D.</summary>
		private List<int> segNumsPatientDetailNM1;
		///<summary>TRN of loop 2200D.</summary>
		private List<int> segNumsClaimTrackingNumberTRN;

		public static bool Is277(X12object xobj) {
			if(xobj.FunctGroups.Count!=1) {//Exactly 1 GS segment in each 277.
				return false;
			}
			if(xobj.FunctGroups[0].Header.Get(1)=="HN") {//GS01 (pgs. 139 & 7)
				return true;
			}
			return false;
		}

		public X277(string messageText)
			: base(messageText) {
			segments=FunctGroups[0].Transactions[0].Segments;//The GS segment contains exactly one ST segment below it.
			segNumInfoSourceNM101=-1;
			segNumInfoReceiverNM101=-1;
			segNumsBillingProviderNM1=new List<int>();
			segNumsPatientDetailNM1=new List<int>();
			segNumsClaimTrackingNumberTRN=new List<int>();
			for(int i=0;i<segments.Count;i++) {
				X12Segment seg=segments[i];
				if(seg.SegmentID=="NM1") {
					string entityIdentifierCode=seg.Get(1);
					if(entityIdentifierCode=="AY" || entityIdentifierCode=="PR") {
						segNumInfoSourceNM101=i;
						i+=4;
					}
					else if(entityIdentifierCode=="41") {
						segNumInfoReceiverNM101=i;
						i+=3;
						seg=segments[i];
						while(seg.SegmentID=="STC") {
							i++;
							seg=segments[i];
						}
						i+=4;
					}
					else if(entityIdentifierCode=="85") {
						segNumsBillingProviderNM1.Add(i);
					}
					else if(entityIdentifierCode=="QC") {
						segNumsPatientDetailNM1.Add(i);
						//Loop 2200D: There can be multiple TRN segments for each NM1*QC.
						do {
							i++;
							segNumsClaimTrackingNumberTRN.Add(i);//a TRN segment is required at this location.
							i++;
							seg=segments[i];//at least one STC segment is required at this location.
							while(seg.SegmentID=="STC") {//there may be multiple STC segments.
								i++;
								if(i>=segments.Count) {
									return;//End of file
								}
								seg=segments[i];
							}
							//Followed by 0 to 3 situational REF segments.
							for(int j=0;j<3 && (seg.SegmentID=="REF");j++) {
								i++;
								if(i>=segments.Count) {
									return;//End of file
								}
								seg=segments[i];
							}
							//Followed by 0 or 1 DTP segments. 
							if(seg.SegmentID=="DTP") {
								i++;
								if(i>=segments.Count) {
									return;//End of file
								}
								seg=segments[i];
							}
							//An entire iteration of loop 2200D is now finished. If another iteration is present, it will begin with a TRN segment.
						} while(seg.SegmentID=="TRN");
					}
				}
			}
		}

		///<summary>NM101 of loop 2100A.</summary>
		public string GetInformationSourceType() {
			if(segNumInfoSourceNM101!=-1) {
				if(segments[segNumInfoSourceNM101].Get(1)=="AY") {
					return "Clearinghouse";
				}
				return "Payor";
			}
			return "";
		}

		///<summary>NM103 of loop 2100A.</summary>
		public string GetInformationSourceName() {
			if(segNumInfoSourceNM101!=-1) {
				return segments[segNumInfoSourceNM101].Get(3);
			}
			return "";
		}

		///<summary>DTP03 of loop 2200A.</summary>
		public DateTime GetInformationSourceReceiptDate() {
			if(segNumInfoSourceNM101!=-1) {
				try {
					string dateStr=segments[segNumInfoSourceNM101+2].Get(3);
					int dateYear=PIn.Int(dateStr.Substring(0,4));
					int dateMonth=PIn.Int(dateStr.Substring(4,2));
					int dateDay=PIn.Int(dateStr.Substring(6,2));
					return new DateTime(dateYear,dateMonth,dateDay);
				}
				catch {
				}
			}
			return DateTime.MinValue;
		}

		///<summary>DTP03 of loop 2200A.</summary>
		public DateTime GetInformationSourceProcessDate() {
			if(segNumInfoSourceNM101!=-1) {
				try {
					string dateStr=segments[segNumInfoSourceNM101+3].Get(3);
					int dateYear=PIn.Int(dateStr.Substring(0,4));
					int dateMonth=PIn.Int(dateStr.Substring(4,2));
					int dateDay=PIn.Int(dateStr.Substring(6,2));
					return new DateTime(dateYear,dateMonth,dateDay);
				}
				catch {
				}
			}
			return DateTime.MinValue;
		}

		///<summary>Last STC segment in loop 2200B. Returns -1 on error.</summary>
		private int GetSegNumLastSTC2200B() {
			if(segNumInfoReceiverNM101!=-1) {
				int segNum=segNumInfoReceiverNM101+2;
				X12Segment seg=segments[segNum];
				while(seg.SegmentID=="STC") {
					segNum++;
					//End of message can happen because the QTY and AMT segments are situational, and so are the two HL segments after this.
					if(segNum>=segments.Count) {
						return segNum-1;
					}
					seg=segments[segNum];
				}
				return segNum-1;
			}
			return -1;
		}

		///<summary>QTY02 of loop 2200B.</summary>
		public long GetQuantityAccepted() {
			int segNum=GetSegNumLastSTC2200B();
			if(segNum!=-1) {
				segNum++;
				if(segNum<segments.Count) {
					X12Segment seg=segments[segNum];
					if(seg.SegmentID=="QTY" && seg.Get(1)=="90") {
						return long.Parse(seg.Get(2));
					}
				}
			}
			return 0;
		}

		///<summary>QTY02 of loop 2200B.</summary>
		public long GetQuantityRejected() {
			int segNum=GetSegNumLastSTC2200B();
			if(segNum!=-1) {
				segNum++;
				if(segNum<segments.Count) {
					X12Segment seg=segments[segNum];
					if(seg.SegmentID=="QTY") {
						try {
							if(seg.Get(1)=="AA") {
								return long.Parse(seg.Get(2));
							}
							else {
								segNum++;
								if(segNum<segments.Count) {
									seg=segments[segNum];
									if(seg.SegmentID=="QTY" && seg.Get(1)=="AA") {
										return long.Parse(seg.Get(2));
									}
								}
							}
						}
						catch {
						}
					}
				}
			}
			return 0;
		}

		///<summary>AMT02 of loop 2200B.</summary>
		public double GetAmountAccepted() {
			int segNum=GetSegNumLastSTC2200B();
			if(segNum!=-1) {
				segNum++;
				if(segNum<segments.Count) {
					X12Segment seg=segments[segNum];
					while(seg.SegmentID=="QTY") {
						segNum++;
						if(segNum>=segments.Count) {
							return 0;
						}
						seg=segments[segNum];
					}
					if(seg.SegmentID=="AMT" && seg.Get(1)=="YU") {
						return double.Parse(seg.Get(2));
					}
				}
			}
			return 0;
		}

		///<summary>AMT02 of loop 2200B.</summary>
		public double GetAmountRejected() {
			int segNum=GetSegNumLastSTC2200B();
			if(segNum!=-1) {
				segNum++;
				if(segNum<segments.Count) {
					X12Segment seg=segments[segNum];
					while(seg.SegmentID=="QTY") {
						segNum++;
						if(segNum>=segments.Count) {
							return 0;
						}
						seg=segments[segNum];
					}
					if(seg.SegmentID=="AMT") {
						if(seg.Get(1)=="YY") {
							return double.Parse(seg.Get(2));
						}
						else {
							segNum++;
							if(segNum<segments.Count) {
								seg=segments[segNum];
								if(seg.SegmentID=="AMT" && seg.Get(1)=="YY") {
									return double.Parse(seg.Get(2));
								}
							}
						}
					}
				}
			}
			return 0;
		}

		///<summary>TRN02 in loop 2200D. Do this first to get a list of all claim tracking numbers that are contained within this 277.  Then, for each claim tracking number, we can later retrieve the AckCode for that single claim. The claim tracking numbers correspond to CLM01 exactly as submitted in the 837. We refer to CLM01 as the claim identifier on our end. We allow more than just digits in our claim identifiers, so we must return a list of strings.</summary>
		public List<string> GetClaimTrackingNumbers() {
			List<string> retVal=new List<string>();
			for(int i=0;i<segNumsClaimTrackingNumberTRN.Count;i++) {
				X12Segment seg=segments[segNumsClaimTrackingNumberTRN[i]];//TRN segment.
				retVal.Add(seg.Get(2));
			}
			return retVal;
		}

		///<summary>Result will contain strings in the following order: 0 Patient Last Name (NM103), 1 Patient First Name (NM104), 2 Patient Middle Name (NM105), 
		///3 Claim Status (STC03), 4 Payor's Claim Control Number (REF02), 5 Institutional Type of Bill (REF02), 6 Claim Date Service Start (DTP03), 
		///7 Claim Date Service End (DTP03), 8 Reason (STC01-2), 9 Amount (STC04), 10 SubscriberId (NM109)</summary>
		public string[] GetClaimInfo(string trackingNumber) {
			string[] result=new string[11];
			for(int i=0;i<result.Length;i++) {
				result[i]="";
			}
			for(int i=0;i<segNumsClaimTrackingNumberTRN.Count;i++) {
				int segNum=segNumsClaimTrackingNumberTRN[i];
				X12Segment seg=segments[segNum];//TRN segment.
				if(seg.Get(2)==trackingNumber) { //TRN02
					//Locate the NM1 segment corresponding to the claim tracking number. One NM1 segment can be shared with multiple TRN segments.
					//The strategy is to locate the NM1 segment furthest down in the message that is above the TRN segment for the tracking number.
					int segNumNM1=segNumsPatientDetailNM1[segNumsPatientDetailNM1.Count-1];//very last NM1 segment
					for(int j=0;j<segNumsPatientDetailNM1.Count-1;j++) {
						if(segNum>segNumsPatientDetailNM1[j] && segNum<segNumsPatientDetailNM1[j+1]) {
							segNumNM1=segNumsPatientDetailNM1[j];
							break;
						}
					}
					seg=segments[segNumNM1];//NM1 segment.
					result[0]=seg.Get(3);//NM103 Last Name
					result[1]=seg.Get(4);//NM104 First Name
					result[2]=seg.Get(5);//NM105 Middle Name
					result[10]=seg.Get(9);//NM109 Identification Code.  Technically can be either patinet ID or subscriber ID from the original claim, but we never send patinet ID, so this will always end up being the subscriber ID.
					segNum++;
					seg=segments[segNum];//STC segment. At least one, maybe multiple, but we only care about the first one.
					string[] stc01=seg.Get(1).Split(new string[] { Separators.Subelement },StringSplitOptions.None);
					result[3]=GetStringForExternalSourceCode507(stc01[0]);
					if(stc01.Length>1) {
						result[8]=GetStringForExternalSourceCode508(stc01[1]);//STC01-2
					}
					result[9]=seg.Get(4);
					//Skip the remaining STC segments (if any).
					segNum++;
					if(segNum>=segments.Count) {
						return result;//End of file
					}
					seg=segments[segNum];
					while(seg.SegmentID=="STC") {
						segNum++;
						seg=segments[segNum];
					}
					while(seg.SegmentID=="REF") {
						string refIdQualifier=seg.Get(1);
						if(refIdQualifier=="1K") {
							result[4]=seg.Get(2);//REF02 Payor's Claim Control Number.
						}
						else if(refIdQualifier=="D9") {
							//REF02 Claim Identifier Number for Clearinghouse and Other Transmission Intermediary from the 837.
							//When we send this it is the same as the claim identifier/claim tracking number, so we don't use this for now.
						}
						else if(refIdQualifier=="BLT") {
							//REF02 Institutional Type of Bill that was sent in the 837.
							result[5]=seg.Get(2);
						}
						segNum++;
						if(segNum>=segments.Count) {
							return result;//End of file
						}
						seg=segments[segNum];
					}
					//The DTP segment for the date of service will not be present when an invalid date was originally sent to the carrier (even though the specifications have it marked as a required segment).
					if(seg.SegmentID=="DTP") {
						string dateServiceStr=seg.Get(3);
						int dateServiceStartYear=PIn.Int(dateServiceStr.Substring(0,4));
						int dateServiceStartMonth=PIn.Int(dateServiceStr.Substring(4,2));
						int dateServiceStartDay=PIn.Int(dateServiceStr.Substring(6,2));
						result[6]=(new DateTime(dateServiceStartYear,dateServiceStartMonth,dateServiceStartDay)).ToShortDateString();
						result[7]=result[6];//End date equals start date if the end date is not specifically defined.
						if(dateServiceStr.Length==17) { //Date range.
							int dateServiceEndYear=PIn.Int(dateServiceStr.Substring(9,4));
							int dateServiceEndMonth=PIn.Int(dateServiceStr.Substring(13,2));
							int dateServiceEndDay=PIn.Int(dateServiceStr.Substring(15,2));
							result[7]=(new DateTime(dateServiceEndYear,dateServiceEndMonth,dateServiceEndDay)).ToShortDateString();
						}
					}
				}
			}
			return result;
		}

		public string GetHumanReadable() {
			string result=
				"Claim Status Reponse From "+GetInformationSourceType()+" "+GetInformationSourceName()+Environment.NewLine
				+"Receipt Date: "+GetInformationSourceReceiptDate().ToShortDateString()+Environment.NewLine
				+"Process Date: "+GetInformationSourceProcessDate().ToShortDateString()+Environment.NewLine
				+"Quantity Accepted: "+GetQuantityAccepted()+Environment.NewLine
				+"Quantity Rejected: "+GetQuantityRejected()+Environment.NewLine
				+"Amount Accepted: "+GetAmountAccepted()+Environment.NewLine
				+"Amount Rejected: "+GetAmountRejected()+Environment.NewLine
				+"Individual Claim Status List: "+Environment.NewLine
				+"Tracking Num  LName  FName  MName  Status  PayorControlNum  InstBillType DateServiceStart  DateServiceEnd";
			List<string> claimTrackingNumbers=GetClaimTrackingNumbers();
			for(int i=0;i<claimTrackingNumbers.Count;i++) {
				string[] claimInfo=GetClaimInfo(claimTrackingNumbers[i]);
				for(int j=0;j<claimInfo.Length;j++) {
					result+=claimInfo[j]+"\\t";
				}
				result+=Environment.NewLine;
			}
			return result;
		}

		///<summary>Code source 507. Since we only send batches, we can only get status codes starting with "A" returned to us.  Returns empty string if no matches. 
		///The codes are pulled from the Washington Publishing Company website (the makers of X12). http://www.wpc-edi.com/reference/codelists/healthcare/claim-status-category-codes.
		///Returns a status which will end up inside etrans.AckCode.  Look at etrans.AckCode comments for acceptable values.</summary>
		public static string GetStringForExternalSourceCode507(string sourceCode) {
			switch(sourceCode) {
				case "A0"://Acknowledgement/Forwarded-The claim/encounter has been forwarded to another entity.
					return "A";
				case "A1"://Acknowledgement/Receipt-The claim/encounter has been received. This does not mean that the claim has been accepted for adjudication.
					return "A";//We say accepted, because if rejected later, then there should hopefully be another 277 which overwrites this one.
				case "A2"://Acknowledgement/Acceptance into adjudication system-The claim/encounter has been accepted into the adjudication system.
					return "A";
				case "A3"://Acknowledgement/Returned as unprocessable claim-The claim/encounter has been rejected and has not been entered into the adjudication system.
					return "R";
				case "A4"://Acknowledgement/Not Found-The claim/encounter can not be found in the adjudication system.
					return "R";//Not really rejected, but no way it could be accepted.
				case "A5"://Acknowledgement/Split Claim-The claim/encounter has been split upon acceptance into the adjudication system.
					return "A";
				case "A6"://Acknowledgement/Rejected for Missing Information - The claim/encounter is missing the information specified in the Status details and has been rejected.
					return "R";
				case "A7"://Acknowledgement/Rejected for Invalid Information - The claim/encounter has invalid information as specified in the Status details and has been rejected.
					return "R";
				case "A8"://Acknowledgement /Rejected for relational field in error.
					return "R";
				case "P0":  //Pending: Adjudication/Details-This is a generic message about a pended claim. A pended claim is one for which no remittance advice has been issued, or only part of the claim has been paid.
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "P1":  //Pending/In Process-The claim or encounter is in the adjudication system.
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "P2":  //Pending/Payer Review-The claim/encounter is suspended and is pending review (e.g. medical review, repricing, Third Party Administrator processing).
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "P3":  //Pending/Provider Requested Information - The claim or encounter is waiting for information that has already been requested from the provider. (Usage: A Claim Status Code identifying the type of information requested, must be reported)
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "P4":  //Pending/Patient Requested Information - The claim or encounter is waiting for information that has already been requested from the patient. (Usage: A status code identifying the type of information requested must be sent)
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "P5":  //Pending/Payer Administrative/System hold
					return "";//Perhaps we can add a "P" option for etrans.AckCode in the future to stand for "Pending"?
				case "F0":  //Finalized-The claim/encounter has completed the adjudication cycle and no more action will be taken.
					return "A";
				case "F1":  //Finalized/Payment-The claim/line has been paid.
					return "A";
				case "F2":  //Finalized/Denial-The claim/line has been denied.
					return "A";
				case "F3":  //Finalized/Revised - Adjudication information has been changed
					return "A";
				case "F3F": //Finalized/Forwarded-The claim/encounter processing has been completed. Any applicable payment has been made and the claim/encounter has been forwarded to a subsequent entity as identified on the original claim or in this payer's records.
					return "A";
				case "F3N": //Finalized/Not Forwarded-The claim/encounter processing has been completed. Any applicable payment has been made. The claim/encounter has NOT been forwarded to any subsequent entity identified on the original claim.
					return "A";
				case "F4":  //Finalized/Adjudication Complete - No payment forthcoming-The claim/encounter has been adjudicated and no further payment is forthcoming.
					return "A";
				case "R0":  //Requests for additional Information/General Requests-Requests that don't fall into other R-type categories.
					return "R";
				case "R1":  //Requests for additional Information/Entity Requests-Requests for information about specific entities (subscribers, patients, various providers).
					return "R";
				case "R3":  //Requests for additional Information/Claim/Line-Requests for information that could normally be submitted on a claim.
					return "R";
				case "R4":  //Requests for additional Information/Documentation-Requests for additional supporting documentation. Examples: certification, x-ray, notes.
					return "R";
				case "R5":  //Request for additional information/more specific detail-Additional information as a follow up to a previous request is needed. The original information was received but is inadequate. More specific/detailed information is requested.
					return "R";
				case "R6":  //Requests for additional information – Regulatory requirements
					return "R";
				case "R7":  //Requests for additional information – Confirm care is consistent with Health Plan policy coverage
					return "R";
				case "R8":  //Requests for additional information – Confirm care is consistent with health plan coverage exceptions
					return "R";
				case "R9":  //Requests for additional information – Determination of medical necessity
					return "R";
				case "R10": //Requests for additional information – Support a filed grievance or appeal
					return "R";
				case "R11": //Requests for additional information – Pre-payment review of claims
					return "R";
				case "R12": //Requests for additional information – Clarification or justification of use for specified procedure code
					return "R";
				case "R13": //Requests for additional information – Original documents submitted are not readable. Used only for subsequent request(s).
					return "R";
				case "R14": //Requests for additional information – Original documents received are not what was requested. Used only for subsequent request(s).
					return "R";
				case "R15": //Requests for additional information – Workers Compensation coverage determination.
					return "R";
				case "R16": //Requests for additional information – Eligibility determination
					return "R";
				case "R17": //Replacement of a Prior Request. Used to indicate that the current attachment request replaces a prior attachment request.
					return "R";
				case "E0":  //Response not possible - error on submitted request data
					return "R";
				case "E1":  //Response not possible - System Status
					return "R";//Perhaps we can add a "E" option for etrans.AckCode in the future to stand for "Error"?
				case "E2":  //Information Holder is not responding; resubmit at a later time.
					return "R";//Perhaps we can add a "E" option for etrans.AckCode in the future to stand for "Error"?
				case "E3":  //Correction required - relational fields in error.
					return "R";
				case "E4":  //Trading partner agreement specific requirement not met: Data correction required. (Usage: A status code identifying the type of information requested must be sent)
					return "R";
				case "D0":  //Data Search Unsuccessful - The payer is unable to return status on the requested claim(s) based on the submitted search criteria.
					return "R";
			}
			return "";//We have to return blank because this value is used to update the etrans table.
		}

		///<summary>Convert the given source code to a string. Only 0 to 56 included so far. All codes listed at http://www.wpc-edi.com/reference/codelists/healthcare/claim-status-codes. </summary>
		public static string GetStringForExternalSourceCode508(string sourceCode) {
			switch(sourceCode) {
				case "0":
					return "Cannot provide further status electronically.";
				case "1":
					return "For more detailed information, see remittance advice.";
				case "2":
					return "More detailed information in letter.";
				case "3":
					return "Claim has been adjudicated and is awaiting payment cycle.";
				case "6":
					return "Balance due from the subscriber.";
				case "12":
					return "One or more originally submitted procedure codes have been combined.";
				case "15":
					return "One or more originally submitted procedure code have been modified.";
				case "16":
					return "Claim/encounter has been forwarded to entity. Usage: This code requires use of an Entity Code.";
				case "17":
					return "Claim/encounter has been forwarded by third party entity to entity. Usage: This code requires use of an Entity Code.";
				case "18":
					return "Entity received claim/encounter, but returned invalid status. Usage: This code requires use of an Entity Code.";
				case "19":
					return "Entity acknowledges receipt of claim/encounter. Usage: This code requires use of an Entity Code.";
				case "20":
					return "Accepted for processing.";
				case "21":
					return "Missing or invalid information. Usage: At least one other status code is required to identify the missing or invalid information.";
				case "23":
					return "Returned to Entity. Usage: This code requires use of an Entity Code.";
				case "24":
					return "Entity not approved as an electronic submitter. Usage: This code requires use of an Entity Code.";
				case "25":
					return "Entity not approved. Usage: This code requires use of an Entity Code.";
				case "26":
					return "Entity not found. Usage: This code requires use of an Entity Code.";
				case "27":
					return "Policy canceled.";
				case "29":
					return "Subscriber and policy number/contract number mismatched.";
				case "30":
					return "Subscriber and subscriber id mismatched.";
				case "31":
					return "Subscriber and policyholder name mismatched.";
				case "32":
					return "Subscriber and policy number/contract number not found.";
				case "33":
					return "Subscriber and subscriber id not found.";
				case "34":
					return "Subscriber and policyholder name not found.";
				case "35":
					return "Claim/encounter not found.";
				case "37":
					return "Predetermination is on file, awaiting completion of services.";
				case "38":
					return "Awaiting next periodic adjudication cycle.";
				case "39":
					return "Charges for pregnancy deferred until delivery.";
				case "40":
					return "Waiting for final approval.";
				case "41":
					return "Special handling required at payer site.";
				case "42":
					return "Awaiting related charges.";
				case "44":
					return "Charges pending provider audit.";
				case "45":
					return "Awaiting benefit determination.";
				case "46":
					return "Internal review/audit.";
				case "47":
					return "Internal review/audit - partial payment made.";
				case "49":
					return "Pending provider accreditation review.";
				case "50":
					return "Claim waiting for internal provider verification.";
				case "51":
					return "Investigating occupational illness/accident.";
				case "52":
					return "Investigating existence of other insurance coverage.";
				case "53":
					return "Claim being researched for Insured ID/Group Policy Number error.";
				case "54":
					return "Duplicate of a previously processed claim/line.";
				case "55":
					return "Claim assigned to an approver/analyst.";
				case "56":
					return "Awaiting eligibility determination.";
				case "57":
					return "Pending COBRA information requested.";
				case "59":
					return "Information was requested by a non-electronic method. Usage: At least one other status code is required to identify the requested information.";
				case "60":
					return "Information was requested by an electronic method. Usage: At least one other status code is required to identify the requested information.";
				case "61":
					return "Eligibility for extended benefits.";
				case "64":
					return "Re-pricing information.";
				case "65":
					return "Claim/line has been paid.";
				case "66":
					return "Payment reflects usual and customary charges.";
				case "72":
					return "Claim contains split payment.";
				case "73":
					return "Payment made to entity, assignment of benefits not on file. Usage: This code requires use of an Entity Code.";
				case "78":
					return "Duplicate of an existing claim/line, awaiting processing.";
				case "81":
					return "Contract/plan does not cover pre-existing conditions.";
				case "83":
					return "No coverage for newborns.";
				case "84":
					return "Service not authorized.";
				case "85":
					return "Entity not primary. Usage: This code requires use of an Entity Code.";
				case "86":
					return "Diagnosis and patient gender mismatch.";
				case "88":
					return "Entity not eligible for benefits for submitted dates of service. Usage: This code requires use of an Entity Code.";
				case "89":
					return "Entity not eligible for dental benefits for submitted dates of service. Usage: This code requires use of an Entity Code.";
				case "90":
					return "Entity not eligible for medical benefits for submitted dates of service. Usage: This code requires use of an Entity Code.";
				case "91":
					return "Entity not eligible/not approved for dates of service. Usage: This code requires use of an Entity Code. ";
				case "92":
					return "Entity does not meet dependent or student qualification. Usage: This code requires use of an Entity Code.";
				case "93":
					return "Entity is not selected primary care provider. Usage: This code requires use of an Entity Code.";
				case "94":
					return "Entity not referred by selected primary care provider. Usage: This code requires use of an Entity Code.";
				case "95":
					return "Requested additional information not received.";
				case "96":
					return "No agreement with entity. Usage: This code requires use of an Entity Code.";
				case "97":
					return "Patient eligibility not found with entity. Usage: This code requires use of an Entity Code.";
				case "98":
					return "Charges applied to deductible.";
				case "99":
					return "Pre-treatment review.";
				case "100":
					return "Pre-certification penalty taken.";
				case "101":
					return "Claim was processed as adjustment to previous claim.";
				case "102":
					return "Newborn's charges processed on mother's claim.";
				case "103":
					return "Claim combined with other claim(s).";
				case "104":
					return "Processed according to plan provisions (Plan refers to provisions that exist between the Health Plan and the Consumer or Patient)";
				case "105":
					return "Claim/line is capitated.";
				case "106":
					return "This amount is not entity's responsibility. Usage: This code requires use of an Entity Code.";
				case "107":
					return "Processed according to contract provisions (Contract refers to provisions that exist between the Health Plan and a Provider of Health Care Services)";
				case "109":
					return "Entity not eligible. Usage: This code requires use of an Entity Code.";
				case "110":
					return "Claim requires pricing information.";
				case "111":
					return "At the policyholder's request these claims cannot be submitted electronically.";
				case "114":
					return "Claim/service should be processed by entity. Usage: This code requires use of an Entity Code.";
				case "116":
					return "Claim submitted to incorrect payer.";
				case "117":
					return "Claim requires signature-on-file indicator.";
				case "121":
					return "Service line number greater than maximum allowable for payer.";
				case "123":
					return "Additional information requested from entity. Usage: This code requires use of an Entity Code.";
				case "124":
					return "Entity's name, address, phone and id number. Usage: This code requires use of an Entity Code.";
				case "125":
					return "Entity's name. Usage: This code requires use of an Entity Code.";
				case "126":
					return "Entity's address. Usage: This code requires use of an Entity Code.";
				case "127":
					return "Entity's Communication Number. Usage: This code requires use of an Entity Code.";
				case "128":
					return "Entity's tax id. Usage: This code requires use of an Entity Code.";
				case "129":
					return "Entity's Blue Cross provider id. Usage: This code requires use of an Entity Code.";
				case "130":
					return "Entity's Blue Shield provider id. Usage: This code requires use of an Entity Code.";
				case "131":
					return "Entity's Medicare provider id. Usage: This code requires use of an Entity Code.";
				case "132":
					return "Entity's Medicaid provider id. Usage: This code requires use of an Entity Code.";
				case "133":
					return "Entity's UPIN. Usage: This code requires use of an Entity Code.";
				case "134":
					return "Entity's CHAMPUS provider id. Usage: This code requires use of an Entity Code.";
				case "135":
					return "Entity's commercial provider id. Usage: This code requires use of an Entity Code.";
				case "136":
					return "Entity's health industry id number. Usage: This code requires use of an Entity Code.";
				case "137":
					return "Entity's plan network id. Usage: This code requires use of an Entity Code.";
				case "138":
					return "Entity's site id . Usage: This code requires use of an Entity Code.";
				case "139":
					return "Entity's health maintenance provider id (HMO). Usage: This code requires use of an Entity Code.";
				case "140":
					return "Entity's preferred provider organization id (PPO). Usage: This code requires use of an Entity Code.";
				case "141":
					return "Entity's administrative services organization id (ASO). Usage: This code requires use of an Entity Code.";
				case "142":
					return "Entity's license/certification number. Usage: This code requires use of an Entity Code.";
				case "143":
					return "Entity's state license number. Usage: This code requires use of an Entity Code.";
				case "144":
					return "Entity's specialty license number. Usage: This code requires use of an Entity Code.";
				case "145":
					return "Entity's specialty/taxonomy code. Usage: This code requires use of an Entity Code.";
				case "146":
					return "Entity's anesthesia license number. Usage: This code requires use of an Entity Code.";
				case "147":
					return "Entity's qualification degree/designation (e.g. RN,PhD,MD). Usage: This code requires use of an Entity Code.";
				case "148":
					return "Entity's social security number. Usage: This code requires use of an Entity Code.";
				case "149":
					return "Entity's employer id. Usage: This code requires use of an Entity Code.";
				case "150":
					return "Entity's drug enforcement agency (DEA) number. Usage: This code requires use of an Entity Code.";
				case "152":
					return "Pharmacy processor number.";
				case "153":
					return "Entity's id number. Usage: This code requires use of an Entity Code.";
				case "154":
					return "Relationship of surgeon & assistant surgeon.";
				case "155":
					return "Entity's relationship to patient. Usage: This code requires use of an Entity Code.";
				case "156":
					return "Patient relationship to subscriber";
				case "157":
					return "Entity's Gender. Usage: This code requires use of an Entity Code.";
				case "158":
					return "Entity's date of birth. Usage: This code requires use of an Entity Code.";
				case "159":
					return "Entity's date of death. Usage: This code requires use of an Entity Code.";
				case "160":
					return "Entity's marital status. Usage: This code requires use of an Entity Code.";
				case "161":
					return "Entity's employment status. Usage: This code requires use of an Entity Code.";
				case "162":
					return "Entity's health insurance claim number (HICN). Usage: This code requires use of an Entity Code.";
				case "163":
					return "Entity's policy/group number. Usage: This code requires use of an Entity Code.";
				case "164":
					return "Entity's contract/member number. Usage: This code requires use of an Entity Code.";
				case "165":
					return "Entity's employer name, address and phone. Usage: This code requires use of an Entity Code.";
				case "166":
					return "Entity's employer name. Usage: This code requires use of an Entity Code.";
				case "167":
					return "Entity's employer address. Usage: This code requires use of an Entity Code.";
				case "168":
					return "Entity's employer phone number. Usage: This code requires use of an Entity Code.";
				case "170":
					return "Entity's employee id. Usage: This code requires use of an Entity Code.";
				case "171":
					return "Other insurance coverage information (health, liability, auto, etc.).";
				case "172":
					return "Other employer name, address and telephone number.";
				case "173":
					return "Entity's name, address, phone, gender, DOB, marital status, employment status and relation to subscriber. Usage: This code requires use of an Entity Code.";
				case "174":
					return "Entity's student status. Usage: This code requires use of an Entity Code.";
				case "175":
					return "Entity's school name. Usage: This code requires use of an Entity Code.";
				case "176":
					return "Entity's school address. Usage: This code requires use of an Entity Code.";
				case "177":
					return "Transplant recipient's name, date of birth, gender, relationship to insured.";
				case "178":
					return "Submitted charges.";
				case "179":
					return "Outside lab charges.";
				case "180":
					return "Hospital s semi-private room rate.";
				case "181":
					return "Hospital s room rate.";
				case "182":
					return "Allowable/paid from other entities coverage Usage: This code requires the use of an entity code.";
				case "183":
					return "Amount entity has paid. Usage: This code requires use of an Entity Code.";
				case "184":
					return "Purchase price for the rented durable medical equipment.";
				case "185":
					return "Rental price for durable medical equipment.";
				case "186":
					return "Purchase and rental price of durable medical equipment.";
				case "187":
					return "Date(s) of service.";
				case "188":
					return "Statement from-through dates.";
				case "189":
					return "Facility admission date";
				case "190":
					return "Facility discharge date";
				case "191":
					return "Date of Last Menstrual Period (LMP)";
				case "192":
					return "Date of first service for current series/symptom/illness.";
				case "193":
					return "First consultation/evaluation date.";
				case "194":
					return "Confinement dates.";
				case "195":
					return "Unable to work dates/Disability Dates.";
				case "196":
					return "Return to work dates.";
				case "197":
					return "Effective coverage date(s).";
				case "198":
					return "Medicare effective date.";
				case "199":
					return "Date of conception and expected date of delivery.";
				case "200":
					return "Date of equipment return.";
				case "201":
					return "Date of dental appliance prior placement.";
				case "202":
					return "Date of dental prior replacement/reason for replacement.";
				case "203":
					return "Date of dental appliance placed.";
				case "204":
					return "Date dental canal(s) opened and date service completed.";
				case "205":
					return "Date(s) dental root canal therapy previously performed.";
				case "206":
					return "Most recent date of curettage, root planing, or periodontal surgery.";
				case "207":
					return "Dental impression and seating date.";
				case "208":
					return "Most recent date pacemaker was implanted.";
				case "209":
					return "Most recent pacemaker battery change date.";
				case "210":
					return "Date of the last x-ray.";
				case "211":
					return "Date(s) of dialysis training provided to patient.";
				case "212":
					return "Date of last routine dialysis.";
				case "213":
					return "Date of first routine dialysis.";
				case "214":
					return "Original date of prescription/orders/referral.";
				case "215":
					return "Date of tooth extraction/evolution.";
				case "216":
					return "Drug information.";
				case "217":
					return "Drug name, strength and dosage form.";
				case "218":
					return "NDC number.";
				case "219":
					return "Prescription number.";
				case "222":
					return "Drug dispensing units and average wholesale price (AWP).";
				case "223":
					return "Route of drug/myelogram administration.";
				case "224":
					return "Anatomical location for joint injection.";
				case "225":
					return "Anatomical location.";
				case "226":
					return "Joint injection site.";
				case "227":
					return "Hospital information.";
				case "228":
					return "Type of bill for UB claim";
				case "229":
					return "Hospital admission source.";
				case "230":
					return "Hospital admission hour.";
				case "231":
					return "Hospital admission type.";
				case "232":
					return "Admitting diagnosis.";
				case "233":
					return "Hospital discharge hour.";
				case "234":
					return "Patient discharge status.";
				case "235":
					return "Units of blood furnished.";
				case "236":
					return "Units of blood replaced.";
				case "237":
					return "Units of deductible blood.";
				case "238":
					return "Separate claim for mother/baby charges.";
				case "239":
					return "Dental information.";
				case "240":
					return "Tooth surface(s) involved.";
				case "241":
					return "List of all missing teeth (upper and lower).";
				case "242":
					return "Tooth numbers, surfaces, and/or quadrants involved.";
				case "243":
					return "Months of dental treatment remaining.";
				case "244":
					return "Tooth number or letter.";
				case "245":
					return "Dental quadrant/arch.";
				case "246":
					return "Total orthodontic service fee, initial appliance fee, monthly fee, length of service.";
				case "247":
					return "Line information.";
				case "249":
					return "Place of service.";
				case "250":
					return "Type of service.";
				case "251":
					return "Total anesthesia minutes.";
				case "252":
					return "Entity's prior authorization/certification number. Usage: This code requires the use of an Entity Code.";
				case "254":
					return "Principal diagnosis code.";
				case "255":
					return "Diagnosis code.";
				case "256":
					return "DRG code(s).";
				case "257":
					return "ADSM-III-R code for services rendered.";
				case "258":
					return "Days/units for procedure/revenue code.";
				case "259":
					return "Frequency of service.";
				case "260":
					return "Length of medical necessity, including begin date.";
				case "261":
					return "Obesity measurements.";
				case "262":
					return "Type of surgery/service for which anesthesia was administered.";
				case "263":
					return "Length of time for services rendered.";
				case "264":
					return "Number of liters/minute & total hours/day for respiratory support.";
				case "265":
					return "Number of lesions excised.";
				case "266":
					return "Facility point of origin and destination - ambulance.";
				case "267":
					return "Number of miles patient was transported.";
				case "268":
					return "Location of durable medical equipment use.";
				case "269":
					return "Length/size of laceration/tumor.";
				case "270":
					return "Subluxation location.";
				case "271":
					return "Number of spine segments.";
				case "272":
					return "Oxygen contents for oxygen system rental.";
				case "273":
					return "Weight.";
				case "274":
					return "Height.";
				case "275":
					return "Claim.";
				case "276":
					return "UB04/HCFA-1450/1500 claim form";
				case "277":
					return "Paper claim.";
				case "279":
					return "Claim/service must be itemized";
				case "281":
					return "Related confinement claim.";
				case "282":
					return "Copy of prescription.";
				case "283":
					return "Medicare entitlement information is required to determine primary coverage";
				case "284":
					return "Copy of Medicare ID card.";
				case "286":
					return "Other payer's Explanation of Benefits/payment information.";
				case "287":
					return "Medical necessity for service.";
				case "288":
					return "Hospital late charges";
				case "290":
					return "Pre-existing information.";
				case "291":
					return "Reason for termination of pregnancy.";
				case "292":
					return "Purpose of family conference/therapy.";
				case "293":
					return "Reason for physical therapy.";
				case "294":
					return "Supporting documentation. Usage: At least one other status code is required to identify the supporting documentation.";
				case "295":
					return "Attending physician report.";
				case "296":
					return "Nurse's notes.";
				case "297":
					return "Medical notes/report.";
				case "298":
					return "Operative report.";
				case "299":
					return "Emergency room notes/report.";
				case "300":
					return "Lab/test report/notes/results.";
				case "301":
					return "MRI report.";
				case "305":
					return "Radiology/x-ray reports and/or interpretation";
				case "306":
					return "Detailed description of service.";
				case "307":
					return "Narrative with pocket depth chart.";
				case "308":
					return "Discharge summary.";
				case "310":
					return "Progress notes for the six months prior to statement date.";
				case "311":
					return "Pathology notes/report.";
				case "312":
					return "Dental charting.";
				case "313":
					return "Bridgework information.";
				case "314":
					return "Dental records for this service.";
				case "315":
					return "Past perio treatment history.";
				case "316":
					return "Complete medical history.";
				case "318":
					return "X-rays/radiology films";
				case "319":
					return "Pre/post-operative x-rays/photographs.";
				case "320":
					return "Study models.";
				case "322":
					return "Recent Full Mouth X-rays";
				case "323":
					return "Study models, x-rays, and/or narrative.";
				case "324":
					return "Recent x-ray of treatment area and/or narrative.";
				case "325":
					return "Recent fm x-rays and/or narrative.";
				case "326":
					return "Copy of transplant acquisition invoice.";
				case "327":
					return "Periodontal case type diagnosis and recent pocket depth chart with narrative.";
				case "329":
					return "Exercise notes.";
				case "330":
					return "Occupational notes.";
				case "331":
					return "History and physical.";
				case "333":
					return "Patient release of information authorization.";
				case "334":
					return "Oxygen certification.";
				case "335":
					return "Durable medical equipment certification.";
				case "336":
					return "Chiropractic certification.";
				case "337":
					return "Ambulance certification/documentation.";
				case "339":
					return "Enteral/parenteral certification.";
				case "340":
					return "Pacemaker certification.";
				case "341":
					return "Private duty nursing certification.";
				case "342":
					return "Podiatric certification.";
				case "343":
					return "Documentation that facility is state licensed and Medicare approved as a surgical facility.";
				case "344":
					return "Documentation that provider of physical therapy is Medicare Part B approved.";
				case "345":
					return "Treatment plan for service/diagnosis";
				case "346":
					return "Proposed treatment plan for next 6 months.";
				case "352":
					return "Duration of treatment plan.";
				case "353":
					return "Orthodontics treatment plan.";
				case "354":
					return "Treatment plan for replacement of remaining missing teeth.";
				case "360":
					return "Benefits Assignment Certification Indicator";
				case "363":
					return "Possible Workers' Compensation";
				case "364":
					return "Is accident/illness/condition employment related?";
				case "365":
					return "Is service the result of an accident?";
				case "366":
					return "Is injury due to auto accident?";
				case "374":
					return "Is prescribed lenses a result of cataract surgery?";
				case "375":
					return "Was refraction performed?";
				case "380":
					return "CRNA supervision/medical direction.";
				case "382":
					return "Did provider authorize generic or brand name dispensing?";
				case "383":
					return "Nerve block use (surgery vs. pain management)";
				case "384":
					return "Is prosthesis/crown/inlay placement an initial placement or a replacement?";
				case "385":
					return "Is appliance upper or lower arch & is appliance fixed or removable?";
				case "386":
					return "Orthodontic Treatment/Purpose Indicator";
				case "387":
					return "Date patient last examined by entity. Usage: This code requires use of an Entity Code.";
				case "388":
					return "Date post-operative care assumed";
				case "389":
					return "Date post-operative care relinquished";
				case "390":
					return "Date of most recent medical event necessitating service(s)";
				case "391":
					return "Date(s) dialysis conducted";
				case "394":
					return "Date(s) of most recent hospitalization related to service";
				case "395":
					return "Date entity signed certification/recertification Usage: This code requires use of an Entity Code.";
				case "396":
					return "Date home dialysis began";
				case "397":
					return "Date of onset/exacerbation of illness/condition";
				case "398":
					return "Visual field test results";
				case "400":
					return "Claim is out of balance";
				case "401":
					return "Source of payment is not valid";
				case "402":
					return "Amount must be greater than zero. Usage: At least one other status code is required to identify which amount element is in error.";
				case "403":
					return "Entity referral notes/orders/prescription. Effective 05/01/2018: Entity referral notes/orders/prescription. Usage: this code requires use of an entity code.";
				case "406":
					return "Brief medical history as related to service(s)";
				case "407":
					return "Complications/mitigating circumstances";
				case "408":
					return "Initial certification";
				case "409":
					return "Medication logs/records (including medication therapy)";
				case "414":
					return "Necessity for concurrent care (more than one physician treating the patient)";
				case "417":
					return "Prior testing, including result(s) and date(s) as related to service(s)";
				case "419":
					return "Individual test(s) comprising the panel and the charges for each test";
				case "420":
					return "Name, dosage and medical justification of contrast material used for radiology procedure";
				case "428":
					return "Reason for transport by ambulance";
				case "430":
					return "Nearest appropriate facility";
				case "431":
					return "Patient's condition/functional status at time of service.";
				case "432":
					return "Date benefits exhausted";
				case "433":
					return "Copy of patient revocation of hospice benefits";
				case "434":
					return "Reasons for more than one transfer per entitlement period";
				case "435":
					return "Notice of Admission";
				case "441":
					return "Entity professional qualification for service(s)";
				case "442":
					return "Modalities of service";
				case "443":
					return "Initial evaluation report";
				case "449":
					return "Projected date to discontinue service(s)";
				case "450":
					return "Awaiting spend down determination";
				case "451":
					return "Preoperative and post-operative diagnosis";
				case "452":
					return "Total visits in total number of hours/day and total number of hours/week";
				case "453":
					return "Procedure Code Modifier(s) for Service(s) Rendered";
				case "454":
					return "Procedure code for services rendered.";
				case "455":
					return "Revenue code for services rendered.";
				case "456":
					return "Covered Day(s)";
				case "457":
					return "Non-Covered Day(s)";
				case "458":
					return "Coinsurance Day(s)";
				case "459":
					return "Lifetime Reserve Day(s)";
				case "460":
					return "NUBC Condition Code(s)";
				case "464":
					return "Payer Assigned Claim Control Number";
				case "465":
					return "Principal Procedure Code for Service(s) Rendered";
				case "466":
					return "Entity's Original Signature. Usage: This code requires use of an Entity Code.";
				case "467":
					return "Entity Signature Date. Usage: This code requires use of an Entity Code.";
				case "468":
					return "Patient Signature Source";
				case "469":
					return "Purchase Service Charge";
				case "470":
					return "Was service purchased from another entity? Usage: This code requires use of an Entity Code.";
				case "471":
					return "Were services related to an emergency?";
				case "472":
					return "Ambulance Run Sheet";
				case "473":
					return "Missing or invalid lab indicator";
				case "474":
					return "Procedure code and patient gender mismatch";
				case "475":
					return "Procedure code not valid for patient age";
				case "476":
					return "Missing or invalid units of service";
				case "477":
					return "Diagnosis code pointer is missing or invalid";
				case "478":
					return "Claim submitter's identifier";
				case "479":
					return "Other Carrier payer ID is missing or invalid";
				case "480":
					return "Entity's claim filing indicator. Usage: This code requires use of an Entity Code.";
				case "481":
					return "Claim/submission format is invalid.";
				case "483":
					return "Maximum coverage amount met or exceeded for benefit period.";
				case "484":
					return "Business Application Currently Not Available";
				case "485":
					return "More information available than can be returned in real time mode. Narrow your current search criteria. This change effective September 1, 2017: More information available than can be returned in real-time mode. Narrow your current search criteria.";
				case "486":
					return "Principal Procedure Date";
				case "487":
					return "Claim not found, claim should have been submitted to/through 'entity'. Usage: This code requires use of an Entity Code.";
				case "488":
					return "Diagnosis code(s) for the services rendered.";
				case "489":
					return "Attachment Control Number";
				case "490":
					return "Other Procedure Code for Service(s) Rendered";
				case "491":
					return "Entity not eligible for encounter submission. Usage: This code requires use of an Entity Code.";
				case "492":
					return "Other Procedure Date";
				case "493":
					return "Version/Release/Industry ID code not currently supported by information holder";
				case "494":
					return "Real-Time requests not supported by the information holder, resubmit as batch request This change effective September 1, 2017: Real-time requests not supported by the information holder, resubmit as batch request";
				case "495":
					return "Requests for re-adjudication must reference the newly assigned payer claim control number for this previously adjusted claim. Correct the payer claim control number and re-submit.";
				case "496":
					return "Submitter not approved for electronic claim submissions on behalf of this entity. Usage: This code requires use of an Entity Code.";
				case "497":
					return "Sales tax not paid";
				case "498":
					return "Maximum leave days exhausted";
				case "499":
					return "No rate on file with the payer for this service for this entity Usage: This code requires use of an Entity Code.";
				case "500":
					return "Entity's Postal/Zip Code. Usage: This code requires use of an Entity Code.";
				case "501":
					return "Entity's State/Province. Usage: This code requires use of an Entity Code.";
				case "502":
					return "Entity's City. Usage: This code requires use of an Entity Code.";
				case "503":
					return "Entity's Street Address. Usage: This code requires use of an Entity Code.";
				case "504":
					return "Entity's Last Name. Usage: This code requires use of an Entity Code.";
				case "505":
					return "Entity's First Name. Usage: This code requires use of an Entity Code.";
				case "506":
					return "Entity is changing processor/clearinghouse. This claim must be submitted to the new processor/clearinghouse. Usage: This code requires use of an Entity Code.";
				case "507":
					return "HCPCS";
				case "508":
					return "ICD9 Usage: At least one other status code is required to identify the related procedure code or diagnosis code.";
				case "509":
					return "External Cause of Injury Code.";
				case "510":
					return "Future date. Usage: At least one other status code is required to identify the data element in error.";
				case "511":
					return "Invalid character. Usage: At least one other status code is required to identify the data element in error.";
				case "512":
					return "Length invalid for receiver's application system. Usage: At least one other status code is required to identify the data element in error.";
				case "513":
					return "HIPPS Rate Code for services Rendered";
				case "514":
					return "Entity's Middle Name Usage: This code requires use of an Entity Code.";
				case "515":
					return "Managed Care review";
				case "516":
					return "Other Entity's Adjudication or Payment/Remittance Date. Usage: An Entity code is required to identify the Other Payer Entity, i.e. primary, secondary.";
				case "517":
					return "Adjusted Repriced Claim Reference Number";
				case "518":
					return "Adjusted Repriced Line item Reference Number";
				case "519":
					return "Adjustment Amount";
				case "520":
					return "Adjustment Quantity";
				case "521":
					return "Adjustment Reason Code";
				case "522":
					return "Anesthesia Modifying Units";
				case "523":
					return "Anesthesia Unit Count";
				case "524":
					return "Arterial Blood Gas Quantity";
				case "525":
					return "Begin Therapy Date";
				case "526":
					return "Bundled or Unbundled Line Number";
				case "527":
					return "Certification Condition Indicator";
				case "528":
					return "Certification Period Projected Visit Count";
				case "529":
					return "Certification Revision Date";
				case "530":
					return "Claim Adjustment Indicator";
				case "531":
					return "Claim Disproportinate Share Amount";
				case "532":
					return "Claim DRG Amount";
				case "533":
					return "Claim DRG Outlier Amount";
				case "534":
					return "Claim ESRD Payment Amount";
				case "535":
					return "Claim Frequency Code";
				case "536":
					return "Claim Indirect Teaching Amount";
				case "537":
					return "Claim MSP Pass-through Amount";
				case "538":
					return "Claim or Encounter Identifier";
				case "539":
					return "Claim PPS Capital Amount";
				case "540":
					return "Claim PPS Capital Outlier Amount";
				case "541":
					return "Claim Submission Reason Code";
				case "542":
					return "Claim Total Denied Charge Amount";
				case "543":
					return "Clearinghouse or Value Added Network Trace";
				case "544":
					return "Clinical Laboratory Improvement Amendment (CLIA) Number";
				case "545":
					return "Contract Amount";
				case "546":
					return "Contract Code";
				case "547":
					return "Contract Percentage";
				case "548":
					return "Contract Type Code";
				case "549":
					return "Contract Version Identifier";
				case "550":
					return "Coordination of Benefits Code";
				case "551":
					return "Coordination of Benefits Total Submitted Charge";
				case "552":
					return "Cost Report Day Count";
				case "553":
					return "Covered Amount";
				case "554":
					return "Date Claim Paid";
				case "555":
					return "Delay Reason Code";
				case "556":
					return "Demonstration Project Identifier";
				case "557":
					return "Diagnosis Date";
				case "558":
					return "Discount Amount";
				case "559":
					return "Document Control Identifier";
				case "560":
					return "Entity's Additional/Secondary Identifier. Usage: This code requires use of an Entity Code.";
				case "561":
					return "Entity's Contact Name. Usage: This code requires use of an Entity Code.";
				case "562":
					return "Entity's National Provider Identifier (NPI). Usage: This code requires use of an Entity Code.";
				case "563":
					return "Entity's Tax Amount. Usage: This code requires use of an Entity Code.";
				case "564":
					return "EPSDT Indicator";
				case "565":
					return "Estimated Claim Due Amount";
				case "566":
					return "Exception Code";
				case "567":
					return "Facility Code Qualifier";
				case "568":
					return "Family Planning Indicator";
				case "569":
					return "Fixed Format Information";
				case "571":
					return "Frequency Count";
				case "572":
					return "Frequency Period";
				case "573":
					return "Functional Limitation Code";
				case "574":
					return "HCPCS Payable Amount Home Health";
				case "575":
					return "Homebound Indicator";
				case "576":
					return "Immunization Batch Number";
				case "577":
					return "Industry Code";
				case "578":
					return "Insurance Type Code";
				case "579":
					return "Investigational Device Exemption Identifier";
				case "580":
					return "Last Certification Date";
				case "581":
					return "Last Worked Date";
				case "582":
					return "Lifetime Psychiatric Days Count";
				case "583":
					return "Line Item Charge Amount";
				case "584":
					return "Line Item Control Number";
				case "585":
					return "Denied Charge or Non-covered Charge";
				case "586":
					return "Line Note Text";
				case "587":
					return "Measurement Reference Identification Code";
				case "588":
					return "Medical Record Number";
				case "589":
					return "Provider Accept Assignment Code";
				case "590":
					return "Medicare Coverage Indicator";
				case "591":
					return "Medicare Paid at 100% Amount";
				case "592":
					return "Medicare Paid at 80% Amount";
				case "593":
					return "Medicare Section 4081 Indicator";
				case "594":
					return "Mental Status Code";
				case "595":
					return "Monthly Treatment Count";
				case "596":
					return "Non-covered Charge Amount";
				case "597":
					return "Non-payable Professional Component Amount";
				case "598":
					return "Non-payable Professional Component Billed Amount";
				case "599":
					return "Note Reference Code";
				case "600":
					return "Oxygen Saturation Qty";
				case "601":
					return "Oxygen Test Condition Code";
				case "602":
					return "Oxygen Test Date";
				case "603":
					return "Old Capital Amount";
				case "604":
					return "Originator Application Transaction Identifier";
				case "605":
					return "Orthodontic Treatment Months Count";
				case "606":
					return "Paid From Part A Medicare Trust Fund Amount";
				case "607":
					return "Paid From Part B Medicare Trust Fund Amount";
				case "608":
					return "Paid Service Unit Count";
				case "609":
					return "Participation Agreement";
				case "610":
					return "Patient Discharge Facility Type Code";
				case "611":
					return "Peer Review Authorization Number";
				case "612":
					return "Per Day Limit Amount";
				case "613":
					return "Physician Contact Date";
				case "614":
					return "Physician Order Date";
				case "615":
					return "Policy Compliance Code";
				case "616":
					return "Policy Name";
				case "617":
					return "Postage Claimed Amount";
				case "618":
					return "PPS-Capital DSH DRG Amount";
				case "619":
					return "PPS-Capital Exception Amount";
				case "620":
					return "PPS-Capital FSP DRG Amount";
				case "621":
					return "PPS-Capital HSP DRG Amount";
				case "622":
					return "PPS-Capital IME Amount";
				case "623":
					return "PPS-Operating Federal Specific DRG Amount";
				case "624":
					return "PPS-Operating Hospital Specific DRG Amount";
				case "625":
					return "Predetermination of Benefits Identifier";
				case "626":
					return "Pregnancy Indicator";
				case "627":
					return "Pre-Tax Claim Amount";
				case "628":
					return "Pricing Methodology";
				case "629":
					return "Property Casualty Claim Number";
				case "630":
					return "Referring CLIA Number";
				case "631":
					return "Reimbursement Rate";
				case "632":
					return "Reject Reason Code";
				case "633":
					return "Related Causes Code (Accident, auto accident, employment)";
				case "634":
					return "Remark Code";
				case "635":
					return "Repriced Ambulatory Patient Group Code";
				case "636":
					return "Repriced Line Item Reference Number";
				case "637":
					return "Repriced Saving Amount";
				case "638":
					return "Repricing Per Diem or Flat Rate Amount";
				case "639":
					return "Responsibility Amount";
				case "640":
					return "Sales Tax Amount";
				case "642":
					return "Service Authorization Exception Code";
				case "643":
					return "Service Line Paid Amount";
				case "644":
					return "Service Line Rate";
				case "645":
					return "Service Tax Amount";
				case "646":
					return "Ship, Delivery or Calendar Pattern Code";
				case "647":
					return "Shipped Date";
				case "648":
					return "Similar Illness or Symptom Date";
				case "649":
					return "Skilled Nursing Facility Indicator";
				case "650":
					return "Special Program Indicator";
				case "651":
					return "State Industrial Accident Provider Number";
				case "652":
					return "Terms Discount Percentage";
				case "653":
					return "Test Performed Date";
				case "654":
					return "Total Denied Charge Amount";
				case "655":
					return "Total Medicare Paid Amount";
				case "656":
					return "Total Visits Projected This Certification Count";
				case "657":
					return "Total Visits Rendered Count";
				case "658":
					return "Treatment Code";
				case "659":
					return "Unit or Basis for Measurement Code";
				case "660":
					return "Universal Product Number";
				case "661":
					return "Visits Prior to Recertification Date Count CR702";
				case "662":
					return "X-ray Availability Indicator";
				case "663":
					return "Entity's Group Name. Usage: This code requires use of an Entity Code.";
				case "664":
					return "Orthodontic Banding Date";
				case "665":
					return "Surgery Date";
				case "666":
					return "Surgical Procedure Code";
				case "667":
					return "Real-Time requests not supported by the information holder, do not resubmit This change effective September 1, 2017: Real-time requests not supported by the information holder, do not resubmit";
				case "668":
					return "Missing Endodontics treatment history and prognosis";
				case "669":
					return "Dental service narrative needed.";
				case "670":
					return "Funds applied from a consumer spending account such as consumer directed/driven health plan (CDHP), Health savings account (H S A) and or other similar accounts";
				case "671":
					return "Funds may be available from a consumer spending account such as consumer directed/driven health plan (CDHP), Health savings account (H S A) and or other similar accounts";
				case "672":
					return "Other Payer's payment information is out of balance";
				case "673":
					return "Patient Reason for Visit";
				case "674":
					return "Authorization exceeded";
				case "675":
					return "Facility admission through discharge dates";
				case "676":
					return "Entity possibly compensated by facility. Usage: This code requires use of an Entity Code.";
				case "677":
					return "Entity not affiliated. Usage: This code requires use of an Entity Code.";
				case "678":
					return "Revenue code and patient gender mismatch";
				case "679":
					return "Submit newborn services on mother's claim";
				case "680":
					return "Entity's Country. Usage: This code requires use of an Entity Code.";
				case "681":
					return "Claim currency not supported";
				case "682":
					return "Cosmetic procedure";
				case "683":
					return "Awaiting Associated Hospital Claims";
				case "684":
					return "Rejected. Syntax error noted for this claim/service/inquiry. See Functional or Implementation Acknowledgement for details. (Usage: Only for use to reject claims or status requests in transactions that were 'accepted with errors' on a 997 or 999 Acknowledgement.)";
				case "685":
					return "Claim could not complete adjudication in real time. Claim will continue processing in a batch mode. Do not resubmit. This change effective September 1, 2017: Claim could not complete adjudication in real-time. Claim will continue processing in a batch mode. Do not resubmit.";
				case "686":
					return "The claim/ encounter has completed the adjudication cycle and the entire claim has been voided";
				case "687":
					return "Claim estimation can not be completed in real time. Do not resubmit. This change effective September 1, 2017: Claim predetermination/estimation could not be completed in real-time. Do not resubmit.";
				case "688":
					return "Present on Admission Indicator for reported diagnosis code(s).";
				case "689":
					return "Entity was unable to respond within the expected time frame. Usage: This code requires use of an Entity Code.";
				case "690":
					return "Multiple claims or estimate requests cannot be processed in real time. This change effective September 1, 2017: Multiple claims or estimate requests cannot be processed in real-time.";
				case "691":
					return "Multiple claim status requests cannot be processed in real time. This change effective September 1, 2017: Multiple claim status requests cannot be processed in real-time.";
				case "692":
					return "Contracted funding agreement-Subscriber is employed by the provider of services";
				case "693":
					return "Amount must be greater than or equal to zero. Usage: At least one other status code is required to identify which amount element is in error.";
				case "694":
					return "Amount must not be equal to zero. Usage: At least one other status code is required to identify which amount element is in error.";
				case "695":
					return "Entity's Country Subdivision Code. Usage: This code requires use of an Entity Code.";
				case "696":
					return "Claim Adjustment Group Code.";
				case "697":
					return "Invalid Decimal Precision. Usage: At least one other status code is required to identify the data element in error.";
				case "698":
					return "Form Type Identification";
				case "699":
					return "Question/Response from Supporting Documentation Form";
				case "700":
					return "ICD10. Usage: At least one other status code is required to identify the related procedure code or diagnosis code.";
				case "701":
					return "Initial Treatment Date";
				case "702":
					return "Repriced Claim Reference Number";
				case "703":
					return "Advanced Billing Concepts (ABC) code";
				case "704":
					return "Claim Note Text";
				case "705":
					return "Repriced Allowed Amount";
				case "706":
					return "Repriced Approved Amount";
				case "707":
					return "Repriced Approved Ambulatory Patient Group Amount";
				case "708":
					return "Repriced Approved Revenue Code";
				case "709":
					return "Repriced Approved Service Unit Count";
				case "710":
					return "Line Adjudication Information. Usage: At least one other status code is required to identify the data element in error.";
				case "711":
					return "Stretcher purpose";
				case "712":
					return "Obstetric Additional Units";
				case "713":
					return "Patient Condition Description";
				case "714":
					return "Care Plan Oversight Number";
				case "715":
					return "Acute Manifestation Date";
				case "716":
					return "Repriced Approved DRG Code";
				case "717":
					return "This claim has been split for processing.";
				case "718":
					return "Claim/service not submitted within the required timeframe (timely filing).";
				case "719":
					return "NUBC Occurrence Code(s)";
				case "720":
					return "NUBC Occurrence Code Date(s)";
				case "721":
					return "NUBC Occurrence Span Code(s)";
				case "722":
					return "NUBC Occurrence Span Code Date(s)";
				case "723":
					return "Drug days supply";
				case "724":
					return "Drug dosage. This change effective 5/01/2017: Drug Quantity";
				case "725":
					return "NUBC Value Code(s)";
				case "726":
					return "NUBC Value Code Amount(s)";
				case "727":
					return "Accident date";
				case "728":
					return "Accident state";
				case "729":
					return "Accident description";
				case "730":
					return "Accident cause";
				case "731":
					return "Measurement value/test result";
				case "732":
					return "Information submitted inconsistent with billing guidelines. Usage: At least one other status code is required to identify the inconsistent information.";
				case "733":
					return "Prefix for entity's contract/member number.";
				case "734":
					return "Verifying premium payment";
				case "735":
					return "This service/claim is included in the allowance for another service or claim.";
				case "736":
					return "A related or qualifying service/claim has not been received/adjudicated.";
				case "737":
					return "Current Dental Terminology (CDT) Code";
				case "738":
					return "Home Infusion EDI Coalition (HEIC) Product/Service Code";
				case "739":
					return "Jurisdiction Specific Procedure or Supply Code";
				case "740":
					return "Drop-Off Location";
				case "741":
					return "Entity must be a person. Usage: This code requires use of an Entity Code.";
				case "742":
					return "Payer Responsibility Sequence Number Code";
				case "743":
					return "Entity's credential/enrollment information. Usage: This code requires use of an Entity Code.";
				case "744":
					return "Services/charges related to the treatment of a hospital-acquired condition or preventable medical error.";
				case "745":
					return "Identifier Qualifier Usage: At least one other status code is required to identify the specific identifier qualifier in error.";
				case "746":
					return "Duplicate Submission Usage: use only at the information receiver level in the Health Care Claim Acknowledgement transaction.";
				case "747":
					return "Hospice Employee Indicator";
				case "748":
					return "Corrected Data Usage: Requires a second status code to identify the corrected data.";
				case "749":
					return "Date of Injury/Illness";
				case "750":
					return "Auto Accident State or Province Code";
				case "751":
					return "Ambulance Pick-up State or Province Code";
				case "752":
					return "Ambulance Drop-off State or Province Code";
				case "753":
					return "Co-pay status code.";
				case "754":
					return "Entity Name Suffix. Usage: This code requires the use of an Entity Code.";
				case "755":
					return "Entity's primary identifier. Usage: This code requires the use of an Entity Code.";
				case "756":
					return "Entity's Received Date. Usage: This code requires the use of an Entity Code.";
				case "757":
					return "Last seen date.";
				case "758":
					return "Repriced approved HCPCS code.";
				case "759":
					return "Round trip purpose description.";
				case "760":
					return "Tooth status code.";
				case "761":
					return "Entity's referral number. Usage: This code requires the use of an Entity Code.";
				case "762":
					return "Locum Tenens Provider Identifier. Code must be used with Entity Code 82 - Rendering Provider";
				case "763":
					return "Ambulance Pickup ZipCode";
				case "764":
					return "Professional charges are non covered.";
				case "765":
					return "Institutional charges are non covered.";
				case "766":
					return "Services were performed during a Health Insurance Exchange (HIX) premium payment grace period.";
				case "767":
					return "Qualifications for emergent/urgent care";
				case "768":
					return "Service date outside the accidental injury coverage period.";
				case "769":
					return "DME Repair or Maintenance";
				case "770":
					return "Duplicate of a claim processed or in process as a crossover/coordination of benefits claim.";
				case "771":
					return "Claim submitted prematurely. Please resubmit after crossover/payer to payer COB allotted waiting period.";
				case "772":
					return "The greatest level of diagnosis code specificity is required.";
				case "773":
					return "One calendar year per claim.";
				case "774":
					return "Experimental/Investigational";
				case "775":
					return "Entity Type Qualifier (Person/Non-Person Entity). Usage: this code requires use of an entity code.";
				case "776":
					return "Pre/Post-operative care";
				case "777":
					return "Processed based on multiple or concurrent procedure rules.";
				case "778":
					return "Non-Compensable incident/event. Usage: To be used for Property and Casualty only.";
				case "779":
					return "Service submitted for the same/similar service within a set timeframe.";
				case "780":
					return "Lifetime benefit maximum";
				case "781":
					return "Claim has been identified as a readmission";
				case "782":
					return "Second surgical opinion";
			}
			return sourceCode.ToString();//Return any code not recognized
		}

		///<summary>Additional modifications to source code 508.</summary>
		public static string GetStringForExternalSourceCode508AdditionalDetial(string sourceCode) {
			switch(sourceCode) {
				case "13":
					return "Contracted Service Provider";
				case "17":
					return "Consultant’s Office";
				case "1E":
					return "Health Maintenance Organization (HMO)";
				case "1G":
					return "Oncology Center";
				case "1H":
					return "Kidney Dialysis Unit";
				case "1I":
					return "Preferred Provider Organization (PPO)";
				case "1O":
					return "Acute Care Hospital";
				case "1P":
					return "Provider";
				case "1Q":
					return "Military Facility";
				case "1R":
					return "University, College or School";
				case "1S":
					return "Outpatient Surgicenter";
				case "1T":
					return "Physician, Clinic or Group Practice";
				case "1U":
					return "Long Term Care Facility";
				case "1V":
					return "Extended Care Facility";
				case "1W":
					return "Psychiatric Health Facility";
				case "1X":
					return "Laboratory";
				case "1Y":
					return "Retail Pharmacy";
				case "1Z":
					return "Home Health Care";
				case "28":
					return "Subcontractor";
				case "2A":
					return "Federal, State, County or City Facility";
				case "2B":
					return "Third-Party Administrator";
				case "2E":
					return "Non-Health Care Miscellaneous Facility";
				case "2I":
					return "Church Operated Facility";
				case "2K":
					return "Partnership";
				case "2P":
					return "Public Health Service Facility";
				case "2Q":
					return "Veterans Administration Facility";
				case "2S":
					return "Public Health Service Indian Service Facility";
				case "2Z":
					return "Hospital Unit of an Institution (prison hospital, college infirmary, etc.)";
				case "30":
					return "Service Supplier";
				case "36":
					return "Employer";
				case "3A":
					return "Hospital Unit Within an Institution for the Mentally Retarded";
				case "3C":
					return "Tuberculosis and Other Respiratory Diseases Facility";
				case "3D":
					return "Obstetrics and Gynecology Facility";
				case "3E":
					return "Eye, Ear, Nose and Throat Facility";
				case "3F":
					return "Rehabilitation Facility";
				case "3G":
					return "Orthopedic Facility";
				case "3H":
					return "Chronic Disease Facility";
				case "3I":
					return "Other Specialty Facility";
				case "3J":
					return "Children’s General Facility";
				case "3K":
					return "Children’s Hospital Unit of an Institution";
				case "3L":
					return "Children’s Psychiatric Facility";
				case "3M":
					return "Children’s Tuberculosis and Other Respiratory Diseases Facility";
				case "3N":
					return "Children’s Eye, Ear, Nose and Throat Facility";
				case "3O":
					return "Children’s Rehabilitiaion Facility";
				case "3P":
					return "Children’s Orthopedic Facility";
				case "3Q":
					return "Children’s Chronic Disease Facility";
				case "3R":
					return "Children’s Other Specialty Facility";
				case "3S":
					return "Institution for Mental Retardation";
				case "3T":
					return "Alcoholism and Other Chemical Dependency Facility";
				case "3U":
					return "General Inpatient Care for AIDS/ARC Facility";
				case "3V":
					return "AIDS/ARC Unit";
				case "3W":
					return "Specialized Outpatient Program for AIDS/ARC";
				case "3X":
					return "Alcohol/Drug Abuse or Dependency Inpatient Unit";
				case "3Y":
					return "Alcohol/Drug Abuse or Dependency Outpatient Services";
				case "3Z":
					return "Arthritis Treatment Center";
				case "40":
					return "Receiver";
				case "43":
					return "Claimant Authorized Representative";
				case "44":
					return "Data Processing Service Bureau";
				case "4A":
					return "Birthing Room/LDRP Room";
				case "4B":
					return "Burn Care Unit";
				case "4C":
					return "Cardiac Catherization Laboratory";
				case "4D":
					return "Open-Heart Surgery Facility";
				case "4E":
					return "Cardiac Intensive Care Unit";
				case "4F":
					return "Angioplasty Facility";
				case "4G":
					return "Chronic Obstructive Pulmonary Disease Service Facility";
				case "4H":
					return "Emergency Department";
				case "4I":
					return "Trauma Center (Certified)";
				case "4J":
					return "Extracorporeal Shock-Wave Lithotripter (ESWL) Unit";
				case "4L":
					return "Genetic Counseling/Screening Services";
				case "4M":
					return "Adult Day Care Program Facility";
				case "4N":
					return "Alzheimer’s Diagnostic/Assessment Services";
				case "4O":
					return "Comprehensive Geriatric Assessment Facility";
				case "4P":
					return "Emergency Response (Geriatric) Unit";
				case "4Q":
					return "Geriatric Acute Care Unit";
				case "4R":
					return "Geriatric Clinics";
				case "4S":
					return "Respite Care Facility";
				case "4U":
					return "Patient Education Unit";
				case "4V":
					return "Community Health Promotion Facility";
				case "4W":
					return "Worksite Health Promotion Facility";
				case "4X":
					return "Hemodialysis Facility";
				case "4Y":
					return "Home Health Services";
				case "4Z":
					return "Hospice";
				case "5A":
					return "Medical Surgical or Other Intensive Care Unit";
				case "5B":
					return "Hisopathology Laboratory";
				case "5C":
					return "Blood Bank";
				case "5D":
					return "Neonatal Intensive Care Unit";
				case "5E":
					return "Obstetrics Unit";
				case "5F":
					return "Occupational Health Services";
				case "5G":
					return "Organized Outpatient Services";
				case "5H":
					return "Pediatric Acute Inpatient Unit";
				case "5I":
					return "Psychiatric Child/Adolescent Services";
				case "5J":
					return "Psychiatric Consultation-Liaison Services";
				case "5K":
					return "Psychiatric Education Services";
				case "5L":
					return "Psychiatric Emergency Services";
				case "5M":
					return "Psychiatric Geriatric Services";
				case "5N":
					return "Psychiatric Inpatient Unit";
				case "5O":
					return "Psychiatric Outpatient Services";
				case "5P":
					return "Psychiatric Partial Hospitalization Program";
				case "5Q":
					return "Megavoltage Radiation Therapy Unit";
				case "5R":
					return "Radioactive Implants Unit";
				case "5S":
					return "Theraputic Radioisotope Facility";
				case "5T":
					return "X-Ray Radiation Therapy Unit";
				case "5U":
					return "CT Scanner Unit";
				case "5V":
					return "Diagnostic Radioisotope Facility";
				case "5W":
					return "Magnetic Resonance Imaging (MRI) Facility";
				case "5X":
					return "Ultrasound Unit";
				case "5Y":
					return "Rehabilitation Inpatient Unit";
				case "5Z":
					return "Rehabilitation Outpatient Services";
				case "61":
					return "Performed At";
				case "6A":
					return "Reproductive Health Services";
				case "6B":
					return "Skilled Nursing or Other Long-Term Care Unit";
				case "6C":
					return "Single Photon Emission Computerized Tomography (SPECT) Unit";
				case "6D":
					return "Organized Social Work Service Facility";
				case "6E":
					return "Outpatient Social Work Services";
				case "6F":
					return "Emergency Department Social Work Services";
				case "6G":
					return "Sports Medicine Clinic/Services";
				case "6H":
					return "Hospital Auxiliary Unit";
				case "6I":
					return "Patient Representative Services";
				case "6J":
					return "Volunteer Services Department";
				case "6K":
					return "Outpatient Surgery Services";
				case "6L":
					return "Organ/Tissue Transplant Unit";
				case "6M":
					return "Orthopedic Surgery Facility";
				case "6N":
					return "Occupational Therapy Services";
				case "6O":
					return "Physical Therapy Services";
				case "6P":
					return "Recreational Therapy Services";
				case "6Q":
					return "Respiratory Therapy Services";
				case "6R":
					return "Speech Therapy Services";
				case "6S":
					return "Women’s Health Center/Services";
				case "6U":
					return "Cardiac Rehabilitation Program Facility";
				case "6V":
					return "Non-Invasive Cardiac Assessment Services";
				case "6W":
					return "Emergency Medical Technician";
				case "6X":
					return "Disciplinary Contact";
				case "6Y":
					return "Case Manager";
				case "71":
					return "Attending Physician";
				case "72":
					return "Operating Physician";
				case "73":
					return "Other Physician";
				case "74":
					return "Corrected Insured";
				case "77":
					return "Service Location";
				case "7C":
					return "Place of Occurrence";
				case "80":
					return "Hospital";
				case "82":
					return "Rendering Provider";
				case "84":
					return "Subscriber’s Employer";
				case "85":
					return "Billing Provider";
				case "87":
					return "Pay-to Provider";
				case "95":
					return "Research Institute";
				case "CK":
					return "Pharmacist";
				case "CZ":
					return "Admitting Surgeon";
				case "D2":
					return "Commercial Insurer";
				case "DD":
					return "Assistant Surgeon";
				case "DJ":
					return "Consulting Physician";
				case "DK":
					return "Ordering Physician";
				case "DN":
					return "Referring Provider";
				case "DO":
					return "Dependent Name";
				case "DQ":
					return "Supervising Physician";
				case "E1":
					return "Person or Other Entity Legally Responsible for a Child";
				case "E2":
					return "Person or Other Entity With Whom a Child Resides";
				case "E7":
					return "Previous Employer";
				case "E9":
					return "Participating Laboratory";
				case "FA":
					return "Facility";
				case "FD":
					return "Physical Address";
				case "FE":
					return "Mail Address";
				case "G0":
					return "Dependent Insured";
				case "G3":
					return "Clinic";
				case "GB":
					return "Other Insured";
				case "GD":
					return "Guardian";
				case "GI":
					return "Paramedic";
				case "GJ":
					return "Paramedical Company";
				case "GK":
					return "Previous Insured";
				case "GM":
					return "Spouse Insured";
				case "GY":
					return "Treatment Facility";
				case "HF":
					return "Healthcare Professional Shortage Area (HPSA) Facility";
				case "HH":
					return "Home Health Agency";
				case "I3":
					return "Independent Physicians Association (IPA)";
				case "IJ":
					return "Injection Point";
				case "IL":
					return "Insured or Subscriber";
				case "IN":
					return "Insurer";
				case "LI":
					return "Independent Lab";
				case "LR":
					return "Legal Representative";
				case "MR":
					return "Medical Insurance Carrier";
				case "OB":
					return "Ordered By";
				case "OD":
					return "Doctor of Optometry";
				case "OX":
					return "Oxygen Therapy Facility";
				case "P0":
					return "Patient Facility";
				case "P2":
					return "Primary Insured or Subscriber";
				case "P3":
					return "Primary Care Provider";
				case "P4":
					return "Prior Insurance Carrier";
				case "P6":
					return "Third Party Reviewing Preferred Provider Organization (PPO)";
				case "P7":
					return "Third Party Repricing Preferred Provider Organization (PPO)";
				case "PT":
					return "Party to Receive Test Report";
				case "PV":
					return "Party performing certification";
				case "PW":
					return "Pick Up Address";
				case "QA":
					return "Pharmacy";
				case "QB":
					return "Purchase Service Provider";
				case "QC":
					return "Patient";
				case "QD":
					return "Responsible Party";
				case "QE":
					return "Policyholder";
				case "QH":
					return "Physician";
				case "QK":
					return "Managed Care";
				case "QL":
					return "Chiropractor";
				case "QN":
					return "Dentist";
				case "QO":
					return "Doctor of Osteopathy";
				case "QS":
					return "Podiatrist";
				case "QV":
					return "Group Practice";
				case "QY":
					return "Medical Doctor";
				case "RC":
					return "Receiving Location";
				case "RW":
					return "Rural Health Clinic";
				case "S4":
					return "Skilled Nursing Facility";
				case "SJ":
					return "Service Provider";
				case "SU":
					return "Supplier/Manufacturer";
				case "T4":
					return "Transfer Point 1000089 Used to identify the geographic location where a patient is transferred or deverted.";
				case "TQ":
					return "Third Party Reviewing Organization (TPO)";
				case "TT":
					return "Transfer To";
				case "TU":
					return "Third Party Repricing Organization (TPO)";
				case "UH":
					return "Nursing Home";
				case "X3":
					return "Utilization Management Organization";
				case "X4":
					return "Spouse";
				case "X5":
					return "Durable Medical Equipment Supplier";
				case "ZZ":
					return "Mutually Defined";
			}
			return sourceCode.ToString();//Return any code not recognized
		}
	}
}

//EXAMPLE 1 - From X12 Specification
//ISA*00*          *00*          *ZZ*810624427      *ZZ*133052274      *060131*0756*^*00501*000000017*0*T*:~
//GS*HN*810624427*133052274*20060131*0756*17*X*005010X214~
//ST*277*0001*005010X214~
//BHT*0085*08*277X2140001*20060205*1635*TH~
//HL*1**20*1~
//NM1*AY*2*FIRST CLEARINGHOUSE*****46*CLHR00~
//TRN*1*200102051635S00001ABCDEF~
//DTP*050*D8*20060205~
//DTP*009*D8*20060207~
//HL*2*1*21*1~
//NM1*41*2*BEST BILLING SERVICE*****46*S00001~
//TRN*2*2002020542857~
//STC*A0:16:PR*20060205*WQ*1000~
//QTY*90*1~
//QTY*AA*2~
//AMT*YU*200~
//AMT*YY*800~
//HL*3*2*19*1~
//NM1*85*2*SMITH CLINIC*****FI*123456789~
//HL*4*3*PT~
//NM1*QC*1*DOE*JOHN****MI*00ABCD1234~
//TRN*2*4001/1339~
//STC*A0:16:PR*20060205*WQ*200~
//REF*1K*22029500123407X~
//DTP*472*RD8*20060128-20060131~
//HL*5*3*PT~
//NM1*QC*1*DOE*JANE****MI*45613027602~
//TRN*2*2890/4~
//STC*A3:21:82*20060205*U*500~
//DTP*472*D8*20060115~
//SVC*HC:22305:22*350*****1~
//STC*A3:122**U*******A3:153:82~
//REF*FJ*11~
//HL*6*3*PT~
//NM1*QC*1*VEST*HELEN****MI*45602708901~
//TRN*2*00000000000000000000~
//STC*A3:401*20060205*U*300~
//DTP*472*RD8*20060120-20060120~
//SE*37*0001~
//GE*1*17~
//IEA*1*000000017~

//EXAMPLE 2 - From X12 Specification
//ISA*00*          *00*          *ZZ*810624427      *ZZ*133052274      *060131*0756*^*00501*000000017*0*T*:~
//GS*HN*810624427*133052274*20060131*0756*17*X*005010X214~
//ST*277*0002*005010X214~
//BHT*0085*08*277X2140002*20060201*0405*TH~
//HL*1**20*1~
//NM1*AY*2*FIRST CLEARINGHOUSE*****46*CLHR00~
//TRN*1*200201312005S00002XYZABC~
//DTP*050*D8*20060131~
//DTP*009*D8*20060201~
//HL*2*1*21*0~
//NM1*41*2*LAST BILLING SERVICE*****46*S00002~
//TRN*2*20020131052389~
//STC*A3:24:41**U~
//QTY*AA*3~
//AMT*YY*800~
//SE*14*00002~
//GE*1*17~
//IEA*1*000000017~

//EXAMPLE 3 - From X12 Specification
//ISA*00*          *00*          *ZZ*810624427      *ZZ*133052274      *060131*0756*^*00501*000000017*0*T*:~
//GS*HN*810624427*133052274*20060131*0756*17*X*005010X214~
//ST*277*0003*005010X214~
//BHT*0085*08*277X2140003*20060221*1025*TH~
//HL*1**20*1~
//NM1*PR*2*YOUR INSURANCE COMPANY*****PI*YIC01~
//TRN*1*0091182~
//DTP*050*D8*20060220~
//DTP*009*D8*20060221~
//HL*2*1*21*1~
//NM1*41*1*JONES*HARRY*B**MD*46*S00003~
//TRN*2*2002022045678~
//STC*A1:19:PR*20060221*WQ*365.5~
//QTY*90*3~
//QTY*AA*2~
//AMT*YU*200.5~
//AMT*YY*165~
//HL*3*2*19*1~
//NM1*85*1*JONES*HARRY*B**MD*FI*234567894~
//HL*4*3*PT~
//NM1*QC*1*PATIENT*FEMALE****MI*2222222222~
//TRN*2*PATIENT22222~
//STC*A2:20:PR*20060221*WQ*100~
//REF*1K*220216359803X~
//DTP*472*D8*20060214~
//HL*5*3*PT~
//NM1*QC*1*PATIENT*MALE****MI*3333333333~
//TRN*2*PATIENT33333~
//STC*A3:187:PR*20060221*U*65~
//DTP*472*D8*20090221~
//HL*6*3*PT~
//NM1*QC*1*JONES*LARRY****MI*4444444444~
//TRN*2*JONES44444~
//STC*A3:21:77*20060221*U*100~
//DTP*472*D8*20060211~
//HL*7*3*PT~
//NM1*QC*1*JOHNSON*MARY****MI*5555555555~
//TRN*2*JONHSON55555~
//STC*A2:20:PR*20060221*WQ*50.5~
//REF*1K*220216359806X~
//DTP*472*D8*20060210~
//HL*8*3*PT~
//NM1*QC*1*MILLS*HARRIETT****MI*6666666666~
//TRN*2*MILLS66666~
//STC*A2:20:PR*20060221*WQ*50~
//REF*1K*220216359807X~
//DTP*472*D8*20060205~
//SE*46*0003~
//GE*1*17~
//IEA*1*000000017~

//EXAMPLE 4 - From X12 Specification
//ISA*00*          *00*          *ZZ*810624427      *ZZ*133052274      *060131*0756*^*00501*000000017*0*T*:~
//GS*HN*810624427*133052274*20060131*0756*17*X*005010X214~
//ST*277*0004*005010X214~
//BHT*0085*08*277X2140004*20060321*1025*TH~
//HL*1**20*1~
//NM1*PR*2*OUR INSURANCE COMPANY*****PI*OIC02~
//TRN*1*00911232~
//DTP*050*D8*20060320~
//DTP*009*D8*20060321~
//HL*2*1*21*1~
//NM1*41*1*KING*EWELL*B**MD*46*S00005~
//TRN*2*200203207890~
//STC*A1:19:PR*20060321*WQ*455~
//QTY*90*3~
//QTY*AA*5~
//AMT*YU*155~
//AMT*YY*300~
//HL*3*2*19*1~
//NM1*85*1*KING*EWELL*B**MD*XX*5365432101~
//TRN*2*00098765432~
//STC*A1:19:PR**WQ*305~
//HL*4*3*PT~
//NM1*QC*1*PATIENT*FEMALE****MI*2222222222~
//TRN*2*PATIENT22222~
//STC*A2:20:PR*20060321*WQ*55~
//REF*1K*220216359803X~
//DTP*472*D8*20060314~
//HL*5*3*PT~
//NM1*QC*1*PATIENT*MALE****MI*3333333333~
//TRN*2*PATIENT33333~
//STC*A3:187:PR*20060321*U*50~
//HL*6*3*PT~
//NM1*QC*1*JONES*MARY****MI*4444444444~
//TRN*2*JONES44444~
//STC*A3:116*20060321*U*100~
//DTP*472*D8*20060311~
//HL*7*3*PT~
//NM1*QC*1*JOHNSON*JIMMY****MI*5555555555~
//TRN*2*JOHNSON55555~
//STC*A2:20:PR*20060321*WQ*50~
//REF*1K*220216359806X~
//DTP*472*D8*20060310~
//HL*8*3*PT~
//NM1*QC*1*MILLS*HALEY****MI*6666666666~
//TRN*2*MILLS66666~
//STC*A2:20:PR*20060321*WQ*50~
//REF*1K*200216359807X~
//DTP*472*D8*20060305~
//HL*9*2*19*0~
//NM1*85*1*REED*I*B**MD*FI*567012345~
//TRN*2*00023456789~
//STC*A3:24:85*20060321*U*150~
//QTY*QC*3~
//AMT*YY*150~
//SE*53*0004~
//GE*1*17~
//IEA*1*000000017~