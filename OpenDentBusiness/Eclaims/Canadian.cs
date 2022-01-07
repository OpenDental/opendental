using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;
using System.Xml;

namespace OpenDentBusiness.Eclaims {
	public class Canadian {

		public delegate void FormCCDPrintDelegate(Etrans pEtrans,string messageText,bool pAutoPrint);
		public delegate void ShowProviderTransferWindowDelegate(Claim claimCur,Patient patCur,Family famCur);
		public delegate void PrintCdaClaimFormDelegate(Claim claim);

#if DEBUG
		public static int testNumber=-1;
#endif

		///<summary>Field A06. The third character is for version of OD. Code OD1 corresponds to all versions 11.x.x.x, code OD2 corresponds to all versions 12.x.x.x, etc...</summary>
		public static string SoftwareSystemId() {
			//Version appVersion=new Version(Application.ProductVersion);
			//return "OD"+(appVersion.Major%10);
			return "OD1";
		}

		///<summary>Throws exceptions.  Called directly instead of from Eclaims.SendBatches.  Includes one claim.
		///Sets claim status internally if successfully sent.  Returns the EtransNum of the ack.
		///Includes various user interaction such as displaying of messages, printing, triggering of COB claims, etc.
		///The queueItem.ClearinghouseNum must refer to a valid Canadian clearinghouse.</summary>
		public static long SendClaim(Clearinghouse clearinghouseClin,ClaimSendQueueItem queueItem,bool doPrint,bool isAutomatic,
			FormCCDPrintDelegate formCCDPrint,ShowProviderTransferWindowDelegate showProviderTransferWindow,
			PrintCdaClaimFormDelegate printCdaClaimFormDelegate) 
		{
//Warning: this path is not handled properly if trailing slash is missing:
			string saveFolder=clearinghouseClin.ExportPath;
			if(!Directory.Exists(saveFolder)) {
				throw new ApplicationException(saveFolder+" not found.");
			}
			Etrans etrans;
			Claim claim;
			Clinic clinic;
			Provider billProv;
			Provider treatProv;
			InsPlan insPlan;
			InsSub insSub;
			Carrier carrier;
			InsPlan insPlan2=null;
			InsSub insSub2=null;
			Carrier carrier2=null;
			List <PatPlan> patPlansForPatient;
			Patient patient;
			Patient subscriber;
			List<ClaimProc> claimProcList;//all claimProcs for a patient.
			List<ClaimProc> claimProcsClaim;
			List<Procedure> procListAll;
			List<Procedure> extracted;
			List<Procedure> procListLabForOne;//Lab fees for one procedure
			Patient subscriber2=null;
			Procedure proc;
			ProcedureCode procCode;
			StringBuilder strb;
			string primaryEOBResponse="";
			string primaryClaimRequestMessage="";
			claimProcList=ClaimProcs.Refresh(queueItem.PatNum);
			List<Claim> listClaims=Claims.GetClaimsFromClaimNums(claimProcList.Select(x => x.ClaimNum)
				.Union(new List<long> { queueItem.ClaimNum }).Distinct().ToList());
			claim=listClaims.FirstOrDefault(x => x.ClaimNum==queueItem.ClaimNum);
			claimProcsClaim=ClaimProcs.GetForSendClaim(claimProcList,claim.ClaimNum);
			long planNum=claim.PlanNum;
			long insSubNum=claim.InsSubNum;
			Relat patRelat=claim.PatRelat;
			long planNum2=claim.PlanNum2;
			long insSubNum2=claim.InsSubNum2;
			Relat patRelat2=claim.PatRelat2;
			if(claim.ClaimType=="PreAuth") {
				etrans=CreateEtransForSendClaim(queueItem.ClaimNum,queueItem.PatNum,clearinghouseClin.HqClearinghouseNum,EtransType.Predeterm_CA);//Can throw exception
			}
			else if(claim.ClaimType=="S") {//Secondary
				//We first need to verify that the claimprocs on the secondary/cob claim are the same as the claimprocs on the primary claim.
				etrans=CreateEtransForSendClaim(queueItem.ClaimNum,queueItem.PatNum,clearinghouseClin.HqClearinghouseNum,EtransType.ClaimCOB_CA);//Can throw exception
				long claimNumPrimary=0;
				for(int i=0;i<claimProcsClaim.Count;i++) {
					List<ClaimProc> claimProcsForProc=ClaimProcs.GetForProc(claimProcList,claimProcsClaim[i].ProcNum);
					bool matchingPrimaryProc=false;
					for(int j=0;j<claimProcsForProc.Count;j++) {
						if(claimProcsForProc[j].ClaimNum!=0 && claimProcsForProc[j].ClaimNum!=claim.ClaimNum 
							&& (claimNumPrimary==0 || claimNumPrimary==claimProcsForProc[j].ClaimNum)) 
						{
							Claim claimPri=listClaims.FirstOrDefault(x => x.ClaimNum==claimProcsForProc[j].ClaimNum);
							if(claimPri!=null && claimPri.ClaimType!="P") {//Make sure this claimproc isn't for a PreAuth or tertiary claim
								continue;
							}
							claimNumPrimary=claimProcsForProc[j].ClaimNum;
							matchingPrimaryProc=true;
							break;
						}
					}
					if(!matchingPrimaryProc) {
						throw new ApplicationException(Lans.g("Canadian","The procedures attached to this COB claim must be the same as the procedures attached to the primary claim."));
					}
				}
				if(ClaimProcs.GetForSendClaim(claimProcList,claimNumPrimary).Count!=claimProcsClaim.Count) {
					throw new ApplicationException(Lans.g("Canadian","The procedures attached to this COB claim must be the same as the procedures attached to the primary claim."));
				}
				//Now ensure that the primary claim received an EOB response, or else we cannot send a COB.
				List <Etrans> etransPrimary=Etranss.GetHistoryOneClaim(claimNumPrimary);
				for(int i=0;i<etransPrimary.Count;i++) {
					primaryClaimRequestMessage=EtransMessageTexts.GetMessageText(etransPrimary[i].EtransMessageTextNum,false);
					Etrans etransPrimaryAck=Etranss.GetEtrans(etransPrimary[i].AckEtransNum);
					if(etransPrimaryAck==null) {
						continue;
					}
					if(etransPrimaryAck.AckCode.ToUpper()=="R") {
						continue;
					}
					primaryEOBResponse=EtransMessageTexts.GetMessageText(etransPrimaryAck.EtransMessageTextNum,false);
					break;
				}
				if(primaryEOBResponse=="") {
					throw new ApplicationException(Lans.g("Canadian","Cannot send secondary claim electronically until primary EOB has been received electronically."));
				}
				else if(primaryEOBResponse.Length<22) {
					throw new ApplicationException(Lans.g("Canadian","Cannot send secondary claim electronically, because primary claim electronic response is malformed. Try sending the primary claim again."));
				}
				else {//primaryEOBResponse.Length>=22
					string messageVersion=primaryEOBResponse.Substring(18,2);//Field A03 always exists on all messages and is always in the same location.
					string messageType=primaryEOBResponse.Substring(20,2);//Field A04 always exists on all messages and is always in the same location.
					if(messageVersion!="04") {
						throw new ApplicationException(Lans.g("Canadian","Cannot send secondary claim electronically, because primary claim electronic response is in an older format. The secondary claim must be printed instead."));
					}
					if(messageType!="21") {//message type 21 is EOB
						throw new ApplicationException(Lans.g("Canadian","Cannot send secondary claim electronically until primary EOB has been received electronically. The existing primary claim electronic response is not an EOB."));
					}
				}
				Claim claimPrimary=Claims.GetClaim(claimNumPrimary);
				planNum=claimPrimary.PlanNum;
				insSubNum=claimPrimary.InsSubNum;
				patRelat=claimPrimary.PatRelat;
				planNum2=claimPrimary.PlanNum2;
				insSubNum2=claimPrimary.InsSubNum2;
				patRelat2=claimPrimary.PatRelat2;
			}
			else { //primary claim
				etrans=CreateEtransForSendClaim(queueItem.ClaimNum,queueItem.PatNum,clearinghouseClin.HqClearinghouseNum,EtransType.Claim_CA);//Can throw exception
			}
			claim=Claims.GetClaim(claim.ClaimNum);
			clinic=Clinics.GetClinic(claim.ClinicNum);
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			billProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvBill)??providerFirst;
			treatProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat)??providerFirst;
			insPlan=InsPlans.GetPlan(planNum,new List <InsPlan> ());
			insSub=InsSubs.GetSub(insSubNum,new List<InsSub>());
			if(planNum2>0) {
				insPlan2=InsPlans.GetPlan(planNum2,new List<InsPlan>());
				insSub2=InsSubs.GetSub(insSubNum2,new List<InsSub>());
				carrier2=Carriers.GetCarrier(insPlan2.CarrierNum);
				subscriber2=Patients.GetPat(insSub2.Subscriber);
			}
			carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			Carrier carrierReceiver=carrier;//This is the value used for primary eclaims and preauths.
			if(carrierReceiver==null) {
				throw new ODException("Invalid carrier associated to insurance plan.  Please run Database Maintenance to fix this.");
			}
			if(claim.ClaimType=="S") {//cob
				if(!carrier.CanadianSupportedTypes.HasFlag(CanSupTransTypes.CobClaimTransaction_07)) {
					throw new ApplicationException(Lans.g("Canadian","This carrier does not accept electronic secondary claims (COB transactions).  Try printing and mailing the claim instead."));
				}
				carrierReceiver=carrier2;
				if(carrierReceiver==null) {
					if(planNum2==0) {
						throw new ODException("The secondary insurance was added after the primary claim was sent.  "
							+"Call the carrier to reverse and resend primary claim with secondary insurance information before sending secondary claim.");
					}
					else {
						throw new ODException("Invalid secondary carrier associated to Other Coverage of the primary claim.  Please run Database Maintenance to fix this.");
					}
				}
			}
			else if(claim.ClaimType=="PreAuth") {
				if(!carrier.CanadianSupportedTypes.HasFlag(CanSupTransTypes.PredeterminationSinglePage_03) && 
					!carrier.CanadianSupportedTypes.HasFlag(CanSupTransTypes.PredeterminationMultiPage_03)
					&& carrier.ElectID!="610099")//ClaimSecure says to iTrans that they support PreAuths, iTrans has not reflected this in their carrier json.
				{//We will consider removing carrier specific cases here in the future.
					throw new ApplicationException(Lans.g("Canadian","This carrier does not accept electronic Pre Authorizations (predeterminations)."));
				}
			}
			CanadianNetwork network=CanadianNetworks.GetNetwork(carrierReceiver.CanadianNetworkNum,clearinghouseClin);
			patPlansForPatient=PatPlans.Refresh(claim.PatNum);
			patient=Patients.GetPat(claim.PatNum);
			subscriber=Patients.GetPat(insSub.Subscriber);
			procListAll=Procedures.Refresh(claim.PatNum);
			extracted=Procedures.GetCanadianExtractedTeeth(procListAll);
			strb=new StringBuilder();
			#region Construct outoing message
			//A01 transaction prefix 12 AN
			strb.Append(TidyAN(network.CanadianTransactionPrefix,12));
			//A02 office sequence number 6 N
			strb.Append(TidyN(etrans.OfficeSequenceNumber,6));
			//A03 format version number 2 N
			if(carrierReceiver.CDAnetVersion=="") {
				strb.Append("04");
			}
			else {
				strb.Append(carrierReceiver.CDAnetVersion);
			}
			//A04 transaction code 2 N
			if(claim.ClaimType=="PreAuth") {
				strb.Append("03");//Predetermination
			}
			else {
				if(claim.ClaimType=="S") {
					strb.Append("07");//cob
				}
				else {
					strb.Append("01");//claim
				}
			}
			//A05 carrier id number 6 N
			strb.Append(carrier.ElectID);//already validated as 6 digit number.  Must always be the primary carrier, even for secondary (COB) eclaims.
			//A06 software system id 3 AN
			strb.Append(SoftwareSystemId());
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//A10 encryption method 1 N
				strb.Append(carrierReceiver.CanadianEncryptionMethod);//validated in UI
			}
			//A07 message length. 5 N in version 04, 4 N in version 02
			//We simply create a place holder here. We come back at the end of message construction and record the actual final message length.
			if(carrierReceiver.CDAnetVersion=="02") {
				strb.Append("0000");
			}
			else { //version 04
				strb.Append("00000");
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//A08 email flag 1 N
				if(claim.CanadianMaterialsForwarded=="") {
					strb.Append("0"); //no additional information
				}
				else if(claim.CanadianMaterialsForwarded.Contains("E")) {
					strb.Append("1"); //E-Mail to follow.
				}
				else {
					strb.Append("2"); //Letter to follow
				}
			}
			else { //version 04
				//A08 materials forwarded 1 AN
				strb.Append(GetMaterialsForwarded(claim.CanadianMaterialsForwarded));
			}
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//A09 carrier transaction counter 5 N
				if(ODBuild.IsDebug()) {
					strb.Append("00001");
				}
				else {		
					strb.Append(TidyN(etrans.CarrierTransCounter,5));
				}
			}
			//B01 CDA provider number 9 AN
			strb.Append(TidyAN(treatProv.NationalProvID,9));//already validated
			//B02 (treating) provider office number 4 AN
			strb.Append(TidyAN(treatProv.CanadianOfficeNum,4));//already validated	
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//B03 billing provider number 9 AN
				//might need to account for possible 5 digit prov id assigned by carrier
				strb.Append(TidyAN(billProv.NationalProvID,9));//already validated
				//B04 billing provider office number 4 AN
				strb.Append(TidyAN(billProv.CanadianOfficeNum,4));//already validated	
				//B05 referring provider 10 AN
				strb.Append(TidyAN(claim.CanadianReferralProviderNum,10));
				//B06 referral reason 2 N
				strb.Append(TidyN(claim.CanadianReferralReason,2));
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//C01 primary policy/plan number 8 AN
				//only validated to ensure that it's not blank and is less than 8. Also that no spaces.
				strb.Append(TidyAN(insPlan.GroupNum,8));
			}
			else { //version 04
				//C01 primary policy/plan number 12 AN
				//only validated to ensure that it's not blank and is less than 12. Also that no spaces.
				strb.Append(TidyAN(insPlan.GroupNum,12));
			}
			//C11 primary division/section number 10 AN
			strb.Append(TidyAN(insPlan.DivisionNo,10));
			if(carrierReceiver.CDAnetVersion=="02") {
				//C02 subscriber id number 11 AN
				strb.Append(TidyAN(insSub.SubscriberID.Replace("-",""),11));//validated
			}
			else { //version 04
				//C02 subscriber id number 12 AN
				strb.Append(TidyAN(insSub.SubscriberID.Replace("-",""),12));//validated
			}
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//C17 primary dependant code 2 N
				string patID="";
				for(int p=0;p<patPlansForPatient.Count;p++) {
					if(patPlansForPatient[p].InsSubNum==insSubNum) {
						patID=patPlansForPatient[p].PatID;
					}
				}
				strb.Append(TidyN(patID,2));
			}
			//C03 relationship code 1 N
			//User interface does not only show Canadian options, but all options are handled.
			strb.Append(GetRelationshipCode(patRelat));
			//C04 patient's sex 1 A
			//validated to not include "unknown"
			if(patient.Gender==PatientGender.Male){
				strb.Append("M");
			}
			else{
				strb.Append("F");
			}
			//C05 patient birthday 8 N
			strb.Append(patient.Birthdate.ToString("yyyyMMdd"));//validated
			if(carrierReceiver.CDAnetVersion=="02") {
				//C06 patient last name 25 A
				strb.Append(TidyA(patient.LName,25));//validated
			}
			else { //version 04
				//C06 patient last name 25 AE
				strb.Append(TidyAE(patient.LName,25,true));//validated
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//C07 patient first name 15 A
				strb.Append(TidyA(patient.FName,15));//validated
			}
			else { //version 04
				//C07 patient first name 15 AE
				strb.Append(TidyAE(patient.FName,15,true));//validated
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//C08 patient middle initial 1 A
				strb.Append(TidyA(patient.MiddleI,1));
			}
			else { //version 04
				//C08 patient middle initial 1 AE
				strb.Append(TidyAE(patient.MiddleI,1));
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//C09 eligibility exception code 1 N
				string exceptionCode=TidyN(patient.CanadianEligibilityCode,1);//Validated.
				if(exceptionCode=="4") {
					exceptionCode="0";//Code 4 in version 04 means "code not applicable", but in version 02, value 0 means "code not applicable".
				}
				strb.Append(exceptionCode);//validated
			}
			else { //version 04
				//C09 eligibility exception code 1 N
				strb.Append(TidyN((patient.CanadianEligibilityCode==0)?4:patient.CanadianEligibilityCode,1));//Validated. Use "code not applicable" when no value has been specified by the user.
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//C10 name of school 25 A
				//validated if patient 18yrs or older and full-time student (or disabled student)
				strb.Append(TidyA(patient.SchoolName,25));
			}
			else { //version 04
				//C10 name of school 25 AEN
				//validated if patient 18yrs or older and full-time student (or disabled student)
				strb.Append(TidyAEN(patient.SchoolName,25));
			}
			bool C19PlanRecordPresent=(insPlan.CanadianPlanFlag=="A" || insPlan.CanadianPlanFlag=="N" || insPlan.CanadianPlanFlag=="V");
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//C12 plan flag 1 A
				strb.Append(Canadian.GetPlanFlag(insPlan.CanadianPlanFlag));
				//C18 plan record count 1 N
				if(C19PlanRecordPresent) {
					strb.Append("1");
				}
				else {
					strb.Append("0");
				}
			}
			CCDFieldInputter primaryClaimData=null;
			if(claim.ClaimType=="S") {
				primaryClaimData=new CCDFieldInputter(primaryClaimRequestMessage);
				if(primaryClaimData.GetFieldById("E04")==null) {
					throw new ApplicationException(Lans.g("Canadian",
						"Cannot send secondary claim because primary claim was sent without specifying secondary insurance information.  "
						+"Reverse and resend primary claim with secondary insurance information before sending secondary claim."));
				}
			}
			//D01 subscriber birthday 8 N
			strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D01").valuestr:subscriber.Birthdate.ToString("yyyyMMdd"));//validated
			if(carrierReceiver.CDAnetVersion=="02") {
				//D02 subscriber last name 25 A
				strb.Append(TidyA(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D02").valuestr:subscriber.LName,25));//validated
			}
			else { //version 04
				//D02 subscriber last name 25 AE
				strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D02").valuestr:subscriber.LName,25,true));//validated
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//D03 subscriber first name 15 A
				strb.Append(TidyA(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D03").valuestr:subscriber.FName,15));//validated
			}
			else { //version 04
				//D03 subscriber first name 15 AE
				strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D03").valuestr:subscriber.FName,15,true));//validated
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//D04 subscriber middle initial 1 A
				strb.Append(TidyA(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D04").valuestr:subscriber.MiddleI,1));
			}
			else { //version 04
				//D04 subscriber middle initial 1 AE
				strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D04").valuestr:subscriber.MiddleI,1));
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//D05 subscriber address line one 30 AN
				strb.Append(TidyAN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D05").valuestr:subscriber.Address,30,true));//validated
			}
			else { //version 04
				//D05 subscriber address line one 30 AEN
				strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D05").valuestr:subscriber.Address,30,true));//validated
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//D06 subscriber address line two 30 AN
				strb.Append(TidyAN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D06").valuestr:subscriber.Address2,30,true));
			}
			else { //version 04
				//D06 subscriber address line two 30 AEN
				strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D06").valuestr:subscriber.Address2,30,true));
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//D07 subscriber city 20 A
				strb.Append(TidyA(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D07").valuestr:subscriber.City,20));//validated
			}
			else { //version 04
				//D07 subscriber city 20 AEN
				strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D07").valuestr:subscriber.City,20,true));//validated
			}
			//D08 subscriber province/state 2 A
			strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D08").valuestr:subscriber.State);//very throroughly validated previously
			if(carrierReceiver.CDAnetVersion=="02") {
				//D09 subscriber postal/zip code 6 AN
				strb.Append(TidyAN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D09").valuestr:subscriber.Zip.Replace("-","").Replace(" ",""),6));//validated.
			}
			else { //version 04
				//D09 subscriber postal/zip code 9 AN
				strb.Append(TidyAN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D09").valuestr:subscriber.Zip.Replace("-","").Replace(" ",""),9));//validated.
			}
			//D10 language of insured 1 A
			strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("D10").valuestr:(subscriber.Language=="fr"?"F":"E"));
			bool orthoRecordFlag=false;
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//D11 card sequence/version number 2 N
				//Not validated against type of carrier yet. Might need to check if Dentaide.
				strb.Append(TidyN(insPlan.DentaideCardSequence,2));
				//E18 secondary coverage flag 1 A
				if(planNum2>0) {
					strb.Append("Y");
				}
				else {
					strb.Append("N");
				}
				//E20 secondary record count 1 N
				if(planNum2==0) {
					strb.Append("0");
				}
				else {
					strb.Append("1");
				}
				//F06 number of procedures performed 1 N. Must be between 1 and 7.  UI prevents attaching more than 7.
				strb.Append(TidyN(claimProcsClaim.Count,1));//number validated
				//F22 extracted teeth count 2 N
				strb.Append(TidyN(extracted.Count,2));//validated against matching prosthesis
				if(claim.ClaimType=="PreAuth") {
					orthoRecordFlag=(claim.CanadaEstTreatStartDate.Year>1880 || claim.CanadaInitialPayment!=0 || claim.CanadaPaymentMode!=0 ||
						claim.CanadaTreatDuration!=0 || claim.CanadaNumAnticipatedPayments!=0 || claim.CanadaAnticipatedPayAmount!=0);
					//F25 Orthodontic Record Flag 1 N
					if(orthoRecordFlag) {
						strb.Append("1");
					}
					else {
						strb.Append("0");
					}
				}
				if(claim.ClaimType=="S") { //cob
					//G39 Embedded Transaction Length N 4
					strb.Append(Canadian.TidyN(primaryEOBResponse.Length,4));
				}
			}
			//Secondary carrier fields (E19 to E07) ONLY included if E20=1----------------------------------------------------
			if(planNum2!=0) {
				if(carrierReceiver.CDAnetVersion!="02") { //version 04
					//E19 secondary carrier transaction number 6 N
					strb.Append(TidyN(etrans.CarrierTransCounter2,6));
				}
				//E01 sec carrier id number 6 N
				if(carrier2.IsCDA) {
					strb.Append(carrier2.ElectID);//Validated as 6 digit number.  Must always be the secondary carrier, even for secondary (COB) eclaims.
				}
				else {
					strb.Append("999999");//See page 74 in version 4.1 document, page 26 in version 2.4 document.
				}
				if(carrierReceiver.CDAnetVersion=="02") {
					//E02 sec carrier policy/plan num 8 AN
					//only validated to ensure that it's not blank and is less than 8. Also that no spaces.
					//We might later allow 999999 if sec carrier is unlisted or unknown.
					strb.Append(TidyAN(insPlan2.GroupNum,8));
				}
				else { //version 04
					//E02 sec carrier policy/plan num 12 AN
					//only validated to ensure that it's not blank and is less than 12. Also that no spaces.
					//We might later allow 999999 if sec carrier is unlisted or unknown.
					strb.Append(TidyAN(insPlan2.GroupNum,12));
				}
				//E05 sec division/section num 10 AN
				strb.Append(TidyAN(insPlan2.DivisionNo,10));
				if(carrierReceiver.CDAnetVersion=="02") {
					//E03 sec plan subscriber id 11 AN
					strb.Append(TidyAN(insSub2.SubscriberID.Replace("-",""),11));//validated
				}
				else { //version 04
					//E03 sec plan subscriber id 12 AN
					strb.Append(TidyAN(insSub2.SubscriberID.Replace("-",""),12));//validated
				}
				if(carrierReceiver.CDAnetVersion!="02") { //version 04
					//E17 sec dependent code 2 N
					string patID="";
					for(int p=0;p<patPlansForPatient.Count;p++) {
						if(patPlansForPatient[p].InsSubNum==insSubNum2) {
							patID=patPlansForPatient[p].PatID;
						}
					}
					strb.Append(TidyN(patID,2));
					//E06 sec relationship code 1 N
					//User interface does not only show Canadian options, but all options are handled.
					strb.Append(GetRelationshipCode(patRelat2));
				}
				//E04 sec subscriber birthday 8 N
				strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E04").valuestr:subscriber2.Birthdate.ToString("yyyyMMdd"));//validated
				if(carrierReceiver.CDAnetVersion!="02") { //version 04
					//E08 sec subscriber last name 25 AE
					strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E08").valuestr:subscriber2.LName,25,true));//validated
					//E09 sec subscriber first name 15 AE
					strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E09").valuestr:subscriber2.FName,15,true));//validated
					//E10 sec subscriber middle initial 1 AE
					strb.Append(TidyAE(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E10").valuestr:subscriber2.MiddleI,1));
					//E11 sec subscriber address one 30 AEN
					strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E11").valuestr:subscriber2.Address,30,true));//validated
					//E12 sec subscriber address two 30 AEN
					strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E12").valuestr:subscriber2.Address2,30,true));
					//E13 sec subscriber city 20 AEN
					strb.Append(TidyAEN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E13").valuestr:subscriber2.City,20,true));//validated
					//E14 sec subscriber province/state 2 A
					strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E14").valuestr:subscriber2.State);//very throroughly validated previously
					//E15 sec subscriber postal/zip code 9 AN
					strb.Append(TidyAN(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E15").valuestr:subscriber2.Zip.Replace("-","").Replace(" ",""),9));//validated
					//E16 sec language 1 A
					strb.Append(claim.ClaimType=="S"?primaryClaimData.GetFieldById("E16").valuestr:(subscriber2.Language=="fr"?"F":"E"));
					//E07 sec card sequence/version num 2 N
					//todo Not validated yet.
					strb.Append(TidyN(claim.ClaimType=="S"?PIn.Int(primaryClaimData.GetFieldById("E07").valuestr):insPlan2.DentaideCardSequence,2));
				}
				//End of secondary subscriber fields---------------------------------------------------------------------------
			}
			else { //There is no secondary plan.
				if(carrierReceiver.CDAnetVersion=="02") { 
					//Secondary subscriber fields are always available in version 2. Since there is no plan, put blank data as a filler.
					//E01 N 6
					strb.Append("000000");
					//E02 AN 8
					strb.Append("        ");
					//E05 AN 10
					strb.Append("          ");
					//E03 AN 11
					strb.Append("           ");
					//E04 N 8
					strb.Append("00000000");
				}
			}
			if(claim.ClaimType!="PreAuth") {
				//F01 payee code 1 N
				bool assignBenefitsInsSub=false;
				if(claim.ClaimType!="S") {
					assignBenefitsInsSub=Claims.GetAssignmentOfBenefits(claim,insSub);
				}
				bool assignBenefitsInsSub2=false;
				if(claim.ClaimType=="S") {
					assignBenefitsInsSub2=Claims.GetAssignmentOfBenefits(claim,insSub2);
				}
				if((claim.ClaimType!="S" && assignBenefitsInsSub) || (claim.ClaimType=="S" && assignBenefitsInsSub2)) {
					if(carrierReceiver.CDAnetVersion=="02") {
						strb.Append("0");//pay dentist
					}
					else { //version 04
						strb.Append("4");//pay dentist
					}
				}
				else {
					strb.Append("1");//pay subscriber
				}
			}
			//F02 accident date 8 N
			if(claim.AccidentDate.Year>1900){//if accident related
				strb.Append(claim.AccidentDate.ToString("yyyyMMdd"));//validated
			}
			else{
				strb.Append(TidyN(0,8));
			}
			if(claim.ClaimType!="PreAuth") {
				//F03 predetermination number 14 AN
				strb.Append(TidyAN(claim.PreAuthString,14));
			}
			if(carrierReceiver.CDAnetVersion=="02") {
				//F15 Is this an Initial Replacement? A 1
				string initialPlacement="Y";
				DateTime initialPlacementDate=DateTime.MinValue;
				if(claim.CanadianIsInitialUpper=="Y"){
					initialPlacement="Y";
					initialPlacementDate=claim.CanadianDateInitialUpper;
				}
				else if(claim.CanadianIsInitialLower=="Y"){
					initialPlacement="Y";
					initialPlacementDate=claim.CanadianDateInitialLower;
				}
				else if(claim.CanadianIsInitialUpper=="N") {
					initialPlacement="N";
					initialPlacementDate=claim.CanadianDateInitialUpper;
				}
				else if(claim.CanadianIsInitialLower=="N"){
					initialPlacement="N";
					initialPlacementDate=claim.CanadianDateInitialLower;
				}
				strb.Append(initialPlacement);
				//F04 date of initial placement 8 N
				if(initialPlacementDate.Year>1900) {
					strb.Append(initialPlacementDate.ToString("yyyyMMdd"));
				}
				else {
					strb.Append("00000000");
				}
				//F05 tx req'd for ortho purposes 1 A
				if(claim.IsOrtho) {
					strb.Append("Y");
				}
				else {
					strb.Append("N");
				}
				//F06 number of procedures performed 1 N. Must be between 1 and 7.  UI prevents attaching more than 7.
				strb.Append(TidyN(claimProcsClaim.Count,1));//number validated
			}
			else { //version 04
				//F15 initial placement upper 1 A  Y or N or X
				strb.Append(Canadian.TidyA(claim.CanadianIsInitialUpper,1));//validated
				//F04 date of initial placement upper 8 N
				if(claim.CanadianDateInitialUpper.Year>1900) {
					strb.Append(claim.CanadianDateInitialUpper.ToString("yyyyMMdd"));
				}
				else {
					strb.Append("00000000");
				}
				//F18 initial placement lower 1 A
				strb.Append(Canadian.TidyA(claim.CanadianIsInitialLower,1));//validated
				//F19 date of initial placement lower 8 N
				if(claim.CanadianDateInitialLower.Year>1900) {
					strb.Append(claim.CanadianDateInitialLower.ToString("yyyyMMdd"));
				}
				else {
					strb.Append("00000000");
				}
				//F05 tx req'd for ortho purposes 1 A
				if(claim.IsOrtho) {
					strb.Append("Y");
				}
				else {
					strb.Append("N");
				}
				//F20 max prosth material 1 N
				if(claim.CanadianMaxProsthMaterial==7) {//our fake crown code
					strb.Append("0");
				}
				else {
					strb.Append(claim.CanadianMaxProsthMaterial.ToString());//validated
				}
				//F21 mand prosth material 1 N
				if(claim.CanadianMandProsthMaterial==7) {//our fake crown code
					strb.Append("0");
				}
				else {
					strb.Append(claim.CanadianMandProsthMaterial.ToString());//validated
				}
			}
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//If F22 is non-zero. Repeat for the number of times specified by F22.----------------------------------------------
				for(int t=0;t<extracted.Count;t++) {
					//F23 extracted tooth num 2 N
					strb.Append(TidyN(Tooth.ToInternat(extracted[t].ToothNum),2));//validated
					//F24 extraction date 8 N
					strb.Append(extracted[t].ProcDate.ToString("yyyyMMdd"));//validated
				}
			}
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				if(claim.ClaimType=="PreAuth") {
					if(ODBuild.IsDebug()) {
						//We are required to test multi-page (up to 7 procs per page) predeterminations for certification. We do not actually do this in the real world.
						//We will use the claim.PreAuthString here to pass these useless numbers in for testing purposes, since this field is not used for predetermination claims for any other reason.
						int currentPredeterminationPageNumber=1;
						int lastPredeterminationPageNumber=1;
						if(claim.PreAuthString!="") {
							string[] predetermNums=claim.PreAuthString.Split(new char[] { ',' });
							currentPredeterminationPageNumber=PIn.Int(predetermNums[0]);
							lastPredeterminationPageNumber=PIn.Int(predetermNums[1]);
						}
						//G46 Current Predetermination Page Number N 1
						strb.Append(Canadian.TidyN(currentPredeterminationPageNumber,1));
						//G47 Last Predetermination Page Number N 1
						strb.Append(Canadian.TidyN(lastPredeterminationPageNumber,1));
					}
					else {
						//G46 Current Predetermination Page Number N 1
						strb.Append("1");//Always 1 page, because UI prevents attaching more than 7 procedures per claim in Canadian mode.
						//G47 Last Predetermination Page Number N 1
						strb.Append("1");//Always 1 page, because UI prevents attaching more than 7 procedures per claim in Canadian mode.
					}
					if(orthoRecordFlag) { //F25 is set
						//F37 Estimated Treatment Starting Date N 8
						strb.Append(Canadian.TidyN(claim.CanadaEstTreatStartDate.ToString("yyyyMMdd"),8));
						double firstExamFee=0;
						double diagnosticPhaseFee=0;
						if(ODBuild.IsDebug()) {
							//Fields F26 and F27 are not required in the real world, but there are a few certification tests that require this information in order for the test to pass.
							if(claim.PreAuthString!="") {
								string[] preauthData=claim.PreAuthString.Split(new char[] { ',' });
								if(preauthData.Length>2) {
									firstExamFee=PIn.Double(preauthData[2]);
								}
								if(preauthData.Length>3) {
									diagnosticPhaseFee=PIn.Double(preauthData[3]);
								}
							}
						}
						//F26 First Examination Fee D 6
						strb.Append(Canadian.TidyD(firstExamFee,6));//optional
						//F27 Diagnostic Phase Fee D 6
						strb.Append(Canadian.TidyD(diagnosticPhaseFee,6));//optional
						//F28 Initial Payment D 6
						strb.Append(Canadian.TidyD(claim.CanadaInitialPayment,6));
						//F29 Payment Mode N 1
						strb.Append(Canadian.TidyN(claim.CanadaPaymentMode,1));//Validated in UI.
						//F30 Treatment Duration N 2
						strb.Append(Canadian.TidyN(claim.CanadaTreatDuration,2));
						//F31 Number of Anticipated Payments N 2
						strb.Append(Canadian.TidyN(claim.CanadaNumAnticipatedPayments,2));
						//F32 Anticipated Payment Amount D 6
						strb.Append(Canadian.TidyD(claim.CanadaAnticipatedPayAmount,6));
					}
				}
			}
			//Procedures: Repeat for number of times specified by F06.----------------------------------------------------------
			for(int p=0;p<claimProcsClaim.Count;p++) {
				//claimProcsClaim already excludes any claimprocs with ProcNum=0, so no payments etc.
				proc=Procedures.GetProcFromList(procListAll,claimProcsClaim[p].ProcNum);
				procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				procListLabForOne=Procedures.GetCanadianLabFees(proc.ProcNum,procListAll);
				//F07 proc line number 1 N
				strb.Append((p+1).ToString());
				if(carrierReceiver.CDAnetVersion=="02") {
					//F08 procedure code 5 N
					strb.Append(TidyN(claimProcsClaim[p].CodeSent,5).Trim().PadLeft(5,'0'));
				}
				else { //version 04
					//F08 procedure code 5 AN
					strb.Append(TidyAN(claimProcsClaim[p].CodeSent,5).Trim().PadLeft(5,'0'));
				}
				if(claim.ClaimType!="PreAuth") {
					//F09 date of service 8 N
					strb.Append(claimProcsClaim[p].ProcDate.ToString("yyyyMMdd"));//validated
				}
				//F10 international tooth, sextant, quad, or arch 2 N
				strb.Append(GetToothQuadOrArch(proc,procCode));
				//F11 tooth surface 5 A
				//the SurfTidy function is very thorough, so it's OK to use TidyAN
				if(procCode.TreatArea==TreatmentArea.Surf) {
					if(ODBuild.IsDebug()) {
						//since the scripts use impossible surfaces, we need to just use raw database here
						strb.Append(TidyAN(proc.Surf,5));
					}
					else {
						strb.Append(TidyAN(Tooth.SurfTidyForClaims(proc.Surf,proc.ToothNum),5));
					}
				}
				else {
					strb.Append("     ");
				}
				//F12 dentist's fee claimed 6 D
				strb.Append(TidyD(claimProcsClaim[p].FeeBilled,6));
				if(carrierReceiver.CDAnetVersion!="02") { //version 04
					//F34 lab procedure code #1 5 AN
					if(procListLabForOne.Count>0) {
						strb.Append(TidyAN(ProcedureCodes.GetProcCode(procListLabForOne[0].CodeNum).ProcCode,5).Trim().PadLeft(5,'0'));
					}
					else {
						strb.Append("     ");
					}
				}
				//F13 lab procedure fee #1 6 D
				if(procListLabForOne.Count>0){
					strb.Append(TidyD(procListLabForOne[0].ProcFee,6));
				}
				else{
					strb.Append("000000");
				}
				if(carrierReceiver.CDAnetVersion=="02") {
					//F14 Unit of Time D 4
					//This is a somewhat deprecated field becacuse it no longer exists in version 04. Some carriers reject claims 
					//if there is a time specified for a procedure that does not require a time. It is safest for now to just set
					//this value to zero always.
					double procHours=0;
					//procHours=(PrefC.GetInt(PrefName.AppointmentTimeIncrement)*procCode.ProcTime.Length)/60.0;
					strb.Append(TidyD(procHours,4));
				}
				else { //version 04
					//F35 lab procedure code #2 5 AN
					if(procListLabForOne.Count>1) {
						strb.Append(TidyAN(ProcedureCodes.GetProcCode(procListLabForOne[1].CodeNum).ProcCode,5).Trim().PadLeft(5,'0'));
					}
					else {
						strb.Append("     ");
					}
					//F36 lab procedure fee #2 6 D
					if(procListLabForOne.Count>1) {
						strb.Append(TidyD(procListLabForOne[1].ProcFee,6));
					}
					else {
						strb.Append("000000");
					}
					//F16 procedure type codes 5 A
					strb.Append(TidyA((proc.CanadianTypeCodes==null || proc.CanadianTypeCodes=="")?"X":proc.CanadianTypeCodes,5));
					//F17 remarks code 2 N
					//optional.  PMP field.  See C12. Zeros when not used.
					strb.Append("00");
				}
			}
			if(carrierReceiver.CDAnetVersion!="02") { //version 04
				//C19 plan record 30 AN
				if(C19PlanRecordPresent) {
					if(insPlan.CanadianPlanFlag=="A") {
						//insPlan.CanadianDiagnosticCode and insPlan.CanadianInstitutionCode are validated in the UI.
						strb.Append(Canadian.TidyAN(Canadian.TidyAN(insPlan.CanadianDiagnosticCode,6,true)+Canadian.TidyAN(insPlan.CanadianInstitutionCode,6,true),30,true));
					}
					else { //N or V. These two plan flags are not yet in use. Presumably, for future use.
						strb.Append(Canadian.TidyAN("",30));
					}
				}
			}
			//We are required to append the primary EOB. This is not a data dictionary field.
			if(claim.ClaimType=="S") {
				strb.Append(ConvertEOBVersion(primaryEOBResponse,carrierReceiver.CDAnetVersion));
			}
			//Now we go back and fill in the actual message length now that we know the number for sure.
			if(carrierReceiver.CDAnetVersion=="02") {
				strb.Replace("0000",Canadian.TidyN(strb.Length,4),31,4);
			}
			else { //version 04
				strb.Replace("00000",Canadian.TidyN(strb.Length,5),32,5);
			}
			//end of creating the message
			//this is where we attempt the actual sending:
			#if DEBUG
				if(claim.ClaimType=="PreAuth") { //Predeterminations
					if(testNumber==3) { //Predetermination test #3
						strb.Replace("Y","N",563,1);//We use claim.IsOrtho for fields F05 and F25, but for some reason in this example the values expected are opposite. We think this is a problem with the CDANet test.
						strb.Replace("00000000","35000025",577,8);//These are optional fields (F26 and F27), so we have not implemented them, but the test does not work without them for some reason.
					}
				}
			#endif
			#endregion Construct Outgoing Message
			string errorMsg="";
			string result=PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
			//Attach an ack to the etrans
			Etrans etransAck=new Etrans();
			string embeddedMsg="";//Will be set later on and checked to see if a second claim needs to be created
			etransAck.PatNum=etrans.PatNum;
			etransAck.PlanNum=etrans.PlanNum;
			etransAck.InsSubNum=etrans.InsSubNum;
			etransAck.CarrierNum=etrans.CarrierNum;
			etransAck.ClaimNum=etrans.ClaimNum;
			etransAck.DateTimeTrans=DateTime.Now;
			etransAck.UserNum=Security.CurUser.UserNum;
			CCDFieldInputter fieldInputter=null;
			if(errorMsg!=""){
				etransAck.Etype=EtransType.AckError;
				etransAck.Note=errorMsg;
				etrans.Note="failed";
			}
			else{
				fieldInputter=new CCDFieldInputter(result);
				CCDField fieldG05=fieldInputter.GetFieldById("G05");
				if(fieldG05!=null) {
					etransAck.AckCode=fieldG05.valuestr;
					if(etransAck.AckCode=="M") { //Manually print the claim form.
						printCdaClaimFormDelegate(claim);
					}
				}
				etransAck.Etype=fieldInputter.GetEtransType();
				//Called after new CCDFieldInputter(...) so that we know the response is valid response format, would throw exception otherwise (try/catch?).
				Claims.SetClaimSent(queueItem.ClaimNum);//No error, safe to set sent.
				claim.DateSent=MiscData.GetNowDateTime();
				claim.ClaimStatus="S";//Reflect changes in cached object.
			}
			bool canCreateSecClaim=(claim.ClaimType!="PreAuth" && claim.ClaimType!="S" && etransAck.Etype==EtransType.ClaimEOB_CA && planNum2>0);
			//If we intend to create a secondary claim and there is an embedded secondary EOB inside of the primary EOB response.
			if(canCreateSecClaim && fieldInputter.GetValue("G39")!="" && fieldInputter.GetValue("G39")!="0000") {
				embeddedMsg=fieldInputter.GetValue("G40");
				//remove the embedded response from primary eTrans	
				StringBuilder sbPrimaryOnly=new StringBuilder();
				foreach(CCDField field in fieldInputter.GetLoadedFields()) {
					if(field.fieldId=="G39") {
						sbPrimaryOnly.Append("0000");
					}
					else if(field.fieldId!="G40") {
						sbPrimaryOnly.Append(field.valuestr);
					}
				}
				result=sbPrimaryOnly.ToString();
			}
			Etranss.Insert(etransAck);
			Etranss.SetMessage(etransAck.EtransNum,result);//Save incomming history.
			etrans.AckEtransNum=etransAck.EtransNum;
			Etranss.Update(etrans);
			Etranss.SetMessage(etrans.EtransNum,strb.ToString());//Save outgoing history.
			Family fam=null;
			List<InsSub> subList=null;
			List<InsPlan> planList=null;
			List<Benefit> benefitList=null;
			bool assignBenInsSub=Claims.GetAssignmentOfBenefits(claim,insSub);
			if(errorMsg!="") {
				throw new ApplicationException(errorMsg);
			}
			else if(assignBenInsSub && clearinghouseClin.IsEraDownloadAllowed!=EraBehaviors.None && (ListTools.In(fieldInputter.MsgType,"21","23"))) {
				//EOB and Predetermination EOB
				fam=Patients.GetFamily(claim.PatNum);
				subList=InsSubs.RefreshForFam(fam);
				planList=InsPlans.RefreshForSubList(subList);
				benefitList=Benefits.Refresh(patPlansForPatient,subList);
				EOBImportHelper(fieldInputter,claimProcsClaim,procListAll,claimProcList,claim,false,showProviderTransferWindow,clearinghouseClin.IsEraDownloadAllowed
					,planList,benefitList,subList,patient);
				SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,claim.PatNum
					,"Claim for service date "+POut.Date(claim.DateService)+" amounts overwritten using recieved EOB amounts."
					,LogSources.CanadaEobAutoImport);
			}
			if(claim.ClaimType!="PreAuth") {
				CCDField fieldTransRefNum=fieldInputter.GetFieldById("G01");
				if(fieldTransRefNum!=null && fieldTransRefNum.valuestr!=null) {
					if(etransAck.AckCode!="R") {
						if(fieldTransRefNum.valuestr.Trim()=="") {
							//Sometimes the transaction reference number is not set in the response.  Will come back as all spaces.
							//In this case we have to leave the reference number on the claim empty so that the Reverse button in the Claim Edit window is disabled.
							claim.CanadaTransRefNum="";
						}
						else {
							//The transaction reference number is left justified and padded with spaces on the right.
							//We have to save this number exactly as reported to us (including spaces) so the insurance company can recognize claim reversals.
							claim.CanadaTransRefNum=fieldTransRefNum.valuestr;
						}
						Claims.Update(claim);
					}
				}
			}
			if(doPrint && formCCDPrint!=null) {
				formCCDPrint(etrans,result,true);//Physically print the form.
			}
			if(canCreateSecClaim) {
				//If an eob was returned and patient has secondary insurance, we'll create secondary claim and then check of we need to consider an embedded response.
				Claim claim2=new Claim();
				claim2.DateSent=DateTime.Today;
				claim2.DateSentOrig=DateTime.MinValue;
				claim2.ClaimStatus="W";
				claim2.AccidentDate=claim.AccidentDate;
				claim2.IsOrtho=claim.IsOrtho;
				claim2.CanadianDateInitialLower=claim.CanadianDateInitialLower;
				claim2.CanadianDateInitialUpper=claim.CanadianDateInitialUpper;
				claim2.CanadianIsInitialLower=claim.CanadianIsInitialLower;
				claim2.CanadianIsInitialUpper=claim.CanadianIsInitialUpper;
				claim2.CanadianMandProsthMaterial=claim.CanadianMandProsthMaterial;
				claim2.CanadianMaterialsForwarded=claim.CanadianMaterialsForwarded;
				claim2.CanadianMaxProsthMaterial=claim.CanadianMaxProsthMaterial;
				claim2.CanadianReferralProviderNum=claim.CanadianReferralProviderNum;
				claim2.CanadianReferralReason=claim.CanadianReferralReason;
				List<Procedure> listSelectedProcs=new List<Procedure>();
				for(int i=0;i<claimProcsClaim.Count;i++) {//loop through the procs from claim1
					proc=Procedures.GetProcFromList(procListAll,claimProcsClaim[i].ProcNum);
					if(proc.ProcNum==0) { 
						//If a carrier inserts its own claimprocs, indicated by field G10 in the response, those claimprocs will be on the list of claimprocs attached to this specific claim
						//but wouldn't be in the list of all procedures for the patient that we're comparing to. 
						//See Carrier Issued Procedures region in EOBImportHelper(...) for more detailed explaination of how these carrier issued procedures might occur
						continue;
					}
					listSelectedProcs.Add(proc);
				}
				if(fam==null) {//Could have been loaded already above.  If fam is null, then so is subList, planList, and benefitList.
					fam=Patients.GetFamily(claim.PatNum);
					subList=InsSubs.RefreshForFam(fam);
					planList=InsPlans.RefreshForSubList(subList);
				}
				PatientNote patNote=PatientNotes.Refresh(patient.PatNum,patient.Guarantor);
				ODTuple<bool,Claim,string> claimResult=AccountModules.CreateClaim(claim2,"S",patPlansForPatient,planList,claimProcList,procListAll,
					subList,patient,patNote,listSelectedProcs,"",insPlan2,insSub2,patRelat2);
				if(claimResult.Item1) {//If claim2 was created successfully and passed validation, then consider sending it right now.
					claim2=claimResult.Item2;
					if(claim.ProvBill!=claim2.ProvBill || claim.ProvTreat!=claim2.ProvTreat) {
						//The billing and treating providers need to be the same on the secondary claim as they were on the primary claim.
						//This is particularly necessary when the primary claim has a secondary provider set as the Treating Provider.
						claim2.ProvBill=claim.ProvBill;
						claim2.ProvTreat=claim.ProvTreat;
						Claims.Update(claim2);
					}
					ClaimSendQueueItem queueItem2=Claims.GetQueueList(claim2.ClaimNum,claim2.ClinicNum,0)[0];
					if(embeddedMsg.IsNullOrEmpty()) {//No embedded secondary response
						string responseMessageVersion=result.Substring(18,2);//Field A03 always exists on all messages and is always in the same location.
						//We can only send an electronic secondary claim when the EOB received from the primary insurance is a version 04 message and when
						//the secondary carrier accepts secondary claims electronically (COBs). Otherwise, the user must send the claim by paper.
						if(responseMessageVersion!="02" && carrier2.IsCDA && carrier2.NoSendElect==NoSendElectType.SendElect
							&& carrier2.CanadianSupportedTypes.HasFlag(CanSupTransTypes.CobClaimTransaction_07))
						{
							long etransNum=SendClaim(clearinghouseClin,queueItem2,doPrint,isAutomatic,formCCDPrint,showProviderTransferWindow,
								printCdaClaimFormDelegate);//recursive
							return etransNum;//for now, we'll return the etransnum of the secondary ack.
						}
						//The secondary carrier does not support COB claim transactions. We must print a manual claim form so the user can send manually.
						if(doPrint) {
							printCdaClaimFormDelegate(claim2);
						}
					}
					else {//Embedded secondary response
						Etrans etrans2=CreateEtransForSendClaim(queueItem2.ClaimNum,queueItem2.PatNum,clearinghouseClin.HqClearinghouseNum,EtransType.ClaimCOB_CA);//Can throw exception
						Etrans etransAck2=new Etrans();
						etransAck2.PatNum=etrans2.PatNum;
						etransAck2.PlanNum=etrans2.PlanNum;
						etransAck2.InsSubNum=etrans2.InsSubNum;
						etransAck2.CarrierNum=etrans2.CarrierNum;
						etransAck2.ClaimNum=etrans2.ClaimNum;
						etransAck2.DateTimeTrans=DateTime.Now;
						etransAck2.UserNum=Security.CurUser.UserNum;
						CCDFieldInputter fieldInputter2=new CCDFieldInputter(embeddedMsg);
						//embedded response occurs when patient has the same carrier for primary and secondary so an embedded response is a COB by default
						etransAck2.Etype=EtransType.ClaimCOB_CA;
						Claims.SetClaimSent(queueItem2.ClaimNum);//No error, safe to set sent.
						claim2.ClaimStatus="S";//Reflect changes in cached object.
						CCDField embeddedFieldG05=fieldInputter2.GetFieldById("G05");
						if(embeddedFieldG05!=null) {
							etransAck.AckCode=embeddedFieldG05.valuestr;
							if(etransAck.AckCode=="M") { //Manually print the claim form.
								printCdaClaimFormDelegate(claim2);
							}
						}
						Etranss.Insert(etransAck2);
						Etranss.SetMessage(etransAck2.EtransNum,embeddedMsg);//Save incomming history.
						etrans2.AckEtransNum=etransAck2.EtransNum;
						Etranss.Update(etrans2);
						Etranss.SetMessage(etrans2.EtransNum,strb.ToString());//Save outgoing history.  Same as sent primary eclaim.
						//This mimics logic earlier in SendClaim(...) to make sure we're handling the embedded response appropriately
						bool assignBenInsSub2=Claims.GetAssignmentOfBenefits(claim,insSub2);
						if(assignBenInsSub2 && clearinghouseClin.IsEraDownloadAllowed!=EraBehaviors.None && ListTools.In(fieldInputter2.MsgType,"21","23")) {
							benefitList=Benefits.Refresh(patPlansForPatient,subList);
							claimProcList=ClaimProcs.Refresh(queueItem2.PatNum);
							List<ClaimProc> claimProcsClaim2=ClaimProcs.GetForSendClaim(claimProcList,claim2.ClaimNum);
							EOBImportHelper(fieldInputter2,claimProcsClaim2,procListAll,claimProcList,claim2,false,showProviderTransferWindow,clearinghouseClin.IsEraDownloadAllowed
								,planList,benefitList,subList,patient);
							SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,claim2.PatNum
								,"Claim for service date "+POut.Date(claim2.DateService)+" amounts overwritten using recieved EOB amounts."
								,LogSources.CanadaEobAutoImport);
						}
						if(doPrint && formCCDPrint!=null) {
							formCCDPrint(etrans2,embeddedMsg,true);//Physically print the form.
						}
					}
				}
			}
			return etransAck.EtransNum;
		}

		///<summary>Throws exception. Similar to Etrans.SetClaimSentOrPrinted(...) but not set the claim sent in the DB.
		///Creates and inserts an etrans using given information then returns it.</summary>
		public static Etrans CreateEtransForSendClaim(long claimNum,long patNum,long clearinghouseNum,EtransType etype) {
			Etrans etrans=Etranss.CreateEtransForClaim(claimNum,patNum,clearinghouseNum,etype,batchNumber:0,Security.CurUser.UserNum);
			try {
				etrans=Etranss.SetCanadianEtransFields(etrans);//etrans.CarrierNum, etrans.CarrierNum2 and etrans.EType all set prior to calling this.
			}
			catch(Exception ex){
				BugSubmissions.SubmitException(ex);//Inform HQ if this ever happens.
				throw ex;//Throw exception still since currently calling methods also throw other exceptions. Previously this was not try/caught.
			}
			Etranss.Insert(etrans);
			return Etranss.GetEtrans(etrans.EtransNum);//Since the DateTimeTrans is set upon insert, we need to read the record again in order to get the date.
		}

		///<summary>Helper method that loops through given listClaimProcsForClaim and does various claimProc related updates depending on fieldInputter.MsgType.
		///Currently only applies to EOB and Predetermination EOBs.  listLabProcs and listClaimProcs are used to pull information regarding lab procedures.
		///For eraBehavior, do not pass in None.</summary>
		public static void EOBImportHelper(CCDFieldInputter fieldInputter,List<ClaimProc> listClaimProcsForClaim,List<Procedure> listPatProcs,
			List<ClaimProc> listClaimProcs,Claim claim,bool isAutomatic,ShowProviderTransferWindowDelegate showProviderTransferWindow,EraBehaviors eobBehavior,Patient patient)
		{
			Family fam=Patients.GetFamily(claim.PatNum);
			List<InsSub> listInsSubs=InsSubs.RefreshForFam(fam);
			List<InsPlan> listInsPlans=InsPlans.RefreshForSubList(listInsSubs);
			List<Benefit> listBenefits=Benefits.Refresh(PatPlans.Refresh(claim.PatNum),listInsSubs);
			Canadian.EOBImportHelper(fieldInputter,listClaimProcsForClaim,listPatProcs,listClaimProcs,claim,isAutomatic,showProviderTransferWindow,eobBehavior,listInsPlans,listBenefits,listInsSubs,patient);
		}
		
		///<summary>Helper method that loops through given listClaimProcsForClaim and does various claimProc related updates depending on fieldInputter.MsgType.
		///Currently only applies to EOB and Predetermination EOBs. listLabProcs and listClaimProcs are used to pull information regarding lab procedures.
		///The following parameters need to be set if eobBehavior is DownloadDoNotReceive: listInsPlans, listBenefits, listInsSubs, patAge.
		///For eraBehavior, do not pass in None.</summary>
		public static void EOBImportHelper(CCDFieldInputter fieldInputter,List<ClaimProc> listClaimProcsForClaim,List<Procedure> listPatProcs,
			List<ClaimProc> listClaimProcs,Claim claim,bool isAutomatic,ShowProviderTransferWindowDelegate showProviderTransferWindow,EraBehaviors eobBehavior,
			List<InsPlan> listInsPlans,List<Benefit> listBenefits,List<InsSub> listInsSubs,Patient patient)//int patAge)
		{
			if(!ListTools.In(fieldInputter.MsgType,"21","23") //Currently only supports EOB and predetermination EOBs.
				|| (listClaimProcsForClaim.Exists(x => ListTools.In(x.Status,ClaimProcStatus.Received,ClaimProcStatus.Supplemental,ClaimProcStatus.CapComplete) || x.ClaimPaymentNum!=0)))//Mimics FormClaimEdit.cs by proc button.
			{
				return;
			}
			if(!fieldInputter.HasValidPaymentLines()) {
				return;
			}
			//Delete any pre existing claimProc total payment rows if any since we will be creating our own.
			for(int i=listClaimProcsForClaim.Count-1;i>=0;i--) {
				ClaimProc cp=listClaimProcsForClaim[i];
				if(cp.ProcNum==0) {
					ClaimProcs.Delete(cp);
					listClaimProcsForClaim.Remove(cp);
				}
			}
			bool isPreEob=(fieldInputter.MsgType=="23");
			//This code mimics the By Total button from the Edit Claim window.
			//Automatically set PayPlanNum if there is a payplan with matching PatNum, PlanNum, and InsSubNum that has not been paid in full.
			//By sending in ClaimNum, we ensure that we only get the payplan a claimproc from this claim was already attached to or payplans with no claimprocs attached.
			List<PayPlan> listPayPlans=PayPlans.GetValidInsPayPlans(listClaimProcsForClaim[0].PatNum,listClaimProcsForClaim[0].PlanNum,listClaimProcsForClaim[0].InsSubNum,listClaimProcsForClaim[0].ClaimNum);
			Dictionary<int,List<CCDField>> dictProcDataClaim=fieldInputter.GetPerformedProcsDict();
			Dictionary<int,string> listNoteFields=fieldInputter.GetExplantionNotesDict();//Note number and note text fields
			#region Missing ClaimProcs - Imported as total payment
			//List of line numbers from listClaimProcsForClaim, used to identify any missing claimProcs that are apart of the EOB.
			List<int> listExistingLineNums=listClaimProcsForClaim.Select(x => (int)x.LineNumber).ToList();
			//List of keys that are associated to EOB procs that can not be found in listClaimProcsForClaim
			List<int> listMissingEOBProcs=dictProcDataClaim.Keys.ToList().FindAll(x => !listExistingLineNums.Contains(x));
			foreach(int lineNumber in listMissingEOBProcs) {
				//VERY RARE
				//If the EOB procs can not find an associated claimProc based on the reported line number we will insert a by total claimProc.
				//We do this so that reporting and income is accurately totaled.
				ClaimProc cpByTotal=ByTotClaimProcHelper(claim,listPayPlans.Count==1?listPayPlans[0].PayPlanNum:0,isPreEob,eobBehavior);
				cpByTotal.LineNumber=(byte)lineNumber;//Normal claimProc Total Payments do not have a linenumber.
				listClaimProcsForClaim.Add(cpByTotal);//This will be inserted into the database below.
			}
			#endregion Missing ClaimProcs - Imported as total payment
			#region Procedures paid on EOB which correspond to claimproc line number.
			List<ClaimProc> listSavedLabClaimProcs=new List<ClaimProc>();
			foreach(ClaimProc claimProcCur in listClaimProcsForClaim) {
				//Canadian lab procedures do not have line number.  Existing claimprocs and newly added by total claimprocs will have a number.
				if(claimProcCur.LineNumber==0) {
					continue;
				}
				//List of lab ProcNums for the current parent procedure if any.  Excludes by total payment rows.  Contains at most 2 lab proc nums.
				List<long> listLabProcNums=listPatProcs.FindAll(x => claimProcCur.ProcNum!=0 && x.ProcNumLab==claimProcCur.ProcNum).Select(x => x.ProcNum).ToList();
				//List of claim procs associated to labs that are associated to the current procedure/claimProcsClaim2[i] and claim. At most 2 items.
				List<ClaimProc> listLabClaimProcs=listClaimProcs.FindAll(x => listLabProcNums.Contains(x.ProcNum) //Only consider claim procs associated to labs
					&& x.InsSubNum==claimProcCur.InsSubNum
					&& x.PlanNum==claimProcCur.PlanNum
					&& x.ClaimNum==claimProcCur.ClaimNum);
				ClaimProc claimProcLabOne=(listLabClaimProcs.Count>=1)?listLabClaimProcs[0]:null;
				ClaimProc claimProcLabTwo=(listLabClaimProcs.Count==2)?listLabClaimProcs[1]:null;
				claimProcCur.Remarks="";
				if(isPreEob) {
					//ClaimProcCur status is already set to Preauth prior to coming into this function.
					//claimProcCur.Status=ClaimProcStatus.Preauth;
				}
				else if(eobBehavior==EraBehaviors.DownloadAndReceive) {//Preauth claimproc retain the "Preauth" status.
					//Mimics FormClaimEdit.butOK_Click(...)
					claimProcCur.Status=ClaimProcStatus.Received;
					claimProcCur.DateCP=DateTime.Today;
					claimProcCur.DateEntry=DateTime.Now;//date it was set rec'd
				}
				foreach(CCDField field in dictProcDataClaim[claimProcCur.LineNumber]) {
					#region claimProcCur updates
					switch(field.fieldId) {
						case "G12"://Eligible Amount
							break;
						case "G13"://Deductible Amount
							if(isPreEob) {
								claimProcCur.DedEst=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							}
							else {
								claimProcCur.DedApplied=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							}
							break;
						case "G14"://Eligible Percentage
							claimProcCur.Percentage=PIn.Int(RawPercentToDisplayPercent(field.valuestr));
							break;
						case "G15"://Benefit Amount for the Procedure
							if(isPreEob) {
								claimProcCur.InsEstTotal=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							}
							else {
								claimProcCur.InsEstTotalOverride=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));//Set this so if it's not marked received, pat port is still reflected correctly
								if(eobBehavior==EraBehaviors.DownloadAndReceive) {
									claimProcCur.InsPayAmt=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
								}
							}
							break;
						case "G43"://Eligible Amount for Lab Proc #1
							break;
						case "G56"://Deductible Amount for Lab Proc #1
							if(claimProcLabOne!=null) {
								if(isPreEob) {
									claimProcLabOne.DedEst=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
								}
								else {
									claimProcLabOne.DedApplied=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
								}
								continue;
							}
							break;
						case "G57"://Eligible Percentage for Lab Proc #1
							if(claimProcLabOne!=null) {
								claimProcLabOne.Percentage=PIn.Int(RawPercentToDisplayPercent(field.valuestr));
								continue;
							}
							break;
						case "G58"://Benefit Amount for Lab Proc #1
							if(claimProcLabOne!=null) {
								if(isPreEob) {
									claimProcLabOne.InsEstTotal=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
									//The following 2 commented out lines should not be necessary as they were taken care of before calling this function.
									//ClaimProcs.CanadianLabBaseEstHelper(...) ensures that lab procs and parent procs have the same status.
									//claimProcLabOne.Status=ClaimProcStatus.Preauth;
								}
								else {
									claimProcLabOne.InsEstTotalOverride=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));//Set this so if it's not marked received, pat port is still reflected correctly
									if(eobBehavior==EraBehaviors.DownloadAndReceive){
										claimProcLabOne.InsPayAmt=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
										claimProcLabOne.Status=ClaimProcStatus.Received;
										claimProcLabOne.DateCP=DateTime.Today;
										claimProcLabOne.DateEntry=DateTime.Now;//date it was set rec'd
									}
								}
								ClaimProcs.Update(claimProcLabOne);//Update only required on the last field for the first lab
								listSavedLabClaimProcs.Add(claimProcLabOne);//Adds to end of the list but not considered.
								continue;
							}
							break;
						case "G02"://Eligible Amount for Lab Proc #2
							break;
						case "G59"://Deductible Amount for Lab Proc #2
							if(claimProcLabTwo!=null) {
								if(isPreEob) {
									claimProcLabTwo.DedEst=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
								}
								else {
									claimProcLabTwo.DedApplied=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
								}
								continue;
							}
							break;
						case "G60"://Eligible Percentage for Lab Proc #2
							if(claimProcLabTwo!=null) {
								claimProcLabTwo.Percentage=PIn.Int(RawPercentToDisplayPercent(field.valuestr));
								continue;
							}
							break;
						case "G61"://Benefit Amount for Lab Proc #2
							if(claimProcLabTwo!=null) {
								if(isPreEob) {
									claimProcLabTwo.InsEstTotal=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
									//The following 2 commented out lines should not be necessary as they were taken care of before calling this function.
									//ClaimProcs.CanadianLabBaseEstHelper(...) ensures that lab procs and parent procs have the same status.
									//claimProcLabOne.Status=ClaimProcStatus.Preauth;
								}
								else {
									claimProcLabTwo.InsEstTotalOverride=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));//Set this so if it's not marked received, pat port is still reflected correctly
									if(eobBehavior==EraBehaviors.DownloadAndReceive){
										claimProcLabTwo.InsPayAmt=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
										claimProcLabTwo.Status=ClaimProcStatus.Received;
										claimProcLabTwo.DateCP=DateTime.Today;
										claimProcLabTwo.DateEntry=DateTime.Now;//date it was set rec'd
									}
								}
								ClaimProcs.Update(claimProcLabTwo);//Update only required on the last field for the second lab
								listSavedLabClaimProcs.Add(claimProcLabTwo);//Adds to end of the list but not considered.
								continue;
							}
							break;
						case "G16"://Explanation Note Number 1
						case "G17"://Explanation Note Number 2
							int noteNumber=PIn.Int(field.valuestr);
							if(listNoteFields.ContainsKey(noteNumber)) {
								if(claimProcCur.Remarks!="") {
									claimProcCur.Remarks+=",";
								}
								claimProcCur.Remarks+=listNoteFields[noteNumber];//Guarenteed 1 item in list
							}
							break;
					}
					#endregion
				}
				if(claimProcCur.IsNew) {//When we identify a missing proc from the EOB we must insert a new claimProc.
					claimProcCur.Remarks=Lans.g("Canadian","Revived from EOB");
					ClaimProcs.Insert(claimProcCur);
				}
				else {
					ClaimProcs.Update(claimProcCur);
				}
			}
			#endregion Procedures paid on EOB which correspond to claimproc line number.
			#region Carrier Issued Procedures - Procedures paid on EOB which were added by the carrier and do not exist on the claim sent.
			//It is acceptable for carriers to take a singular procedure (e.g. one with multiple units) and unbundle it into multiple procedures.
			//E.g. A real life response replaced a 11114 code with 2 carrier issued codes; 11113 and 11111.
			//The fields directly related to the 11114 procedure were all zeroed out (E.g. the Benefit Amount).
			//However, the 2 carrier issued procedures had fields with value which were linked back to the procedure that was zeroed out.
			//The G10 section is where the carrier will indicate how many of these new procedures are present within the response.
			if(PIn.Int(fieldInputter.GetFieldById("G10").valuestr,false)!=0) {//Number of Carrier Issued Procedure Codes
				List<CarrierIssuedProcedure> listCarrierIssuedProcs=new List<CarrierIssuedProcedure>();
				foreach(List<CCDField> listFieldsCur in fieldInputter.GetCarrierIssuedProcs()) {
					listCarrierIssuedProcs.Add(new CarrierIssuedProcedure(listFieldsCur,claim,
						listPayPlans.Count==1?listPayPlans[0].PayPlanNum:0,isPreEob,eobBehavior,listNoteFields));
				}
				//Loop through each carrier issued procedure and try to move its value to the corresponding procedure that it references.
				//Do not move any values around if the referenced procedure cannot be found or if there are multiple bundled procedures being referenced.
				for(int i=0;i<listCarrierIssuedProcs.Count;i++) {
					//Only manipulate a claim procedure that was submitted when the carrier issued procedure references a singular procedure.
					if(listCarrierIssuedProcs[i].ArrayLineNumbers.Length==1) {
						ClaimProc claimProcReferenced=listClaimProcsForClaim.FirstOrDefault(x => x.LineNumber==listCarrierIssuedProcs[i].ArrayLineNumbers[0]);
						//Only manipulate the claim procedure that correlates to the line number specified.
						if(claimProcReferenced!=null) {
							ClaimProc claimProcReferencedOld=claimProcReferenced.Copy();
							if(isPreEob) {
								claimProcReferenced.DedEst+=listCarrierIssuedProcs[i].Deductible;
								claimProcReferenced.InsPayEst+=listCarrierIssuedProcs[i].BenefitAmount;
								listCarrierIssuedProcs[i].ClaimProcByTotal.DedEst=0;
								listCarrierIssuedProcs[i].ClaimProcByTotal.InsPayEst=0;
							}
							else {
								claimProcReferenced.DedApplied+=listCarrierIssuedProcs[i].Deductible;
								//Set this so if it's not marked received, pat port is still reflected correctly
								claimProcReferenced.InsEstTotalOverride+=listCarrierIssuedProcs[i].BenefitAmount;
								listCarrierIssuedProcs[i].ClaimProcByTotal.DedApplied=0;
								listCarrierIssuedProcs[i].ClaimProcByTotal.InsEstTotalOverride=0;
								if(eobBehavior==EraBehaviors.DownloadAndReceive) {
									claimProcReferenced.InsPayAmt+=listCarrierIssuedProcs[i].BenefitAmount;
									listCarrierIssuedProcs[i].ClaimProcByTotal.InsPayAmt=0;
								}
							}
							ClaimProcs.Update(claimProcReferenced,claimProcReferencedOld);
						}
					}
					//Always insert these new 'As Total' claimprocs that were created due to these carrier issued procedures to retain at least the remarks.
					ClaimProcs.Insert(listCarrierIssuedProcs[i].ClaimProcByTotal);
					listClaimProcsForClaim.Add(listCarrierIssuedProcs[i].ClaimProcByTotal);
				}
			}
			#endregion Carier Issued Procedures - Procedures paid on EOB which were added by the carrier and do not exist on the claim sent.
			#region Updating the claimproc estimates based on the preauth information
			if(isPreEob) {//Mimics FormClaimEdit preauth by procedure logic.
				List<ClaimProc> procListAll=ClaimProcs.Refresh(claim.PatNum);
				for(int i=0;i<listClaimProcsForClaim.Count;i++) {
					ClaimProcs.SetInsEstTotalOverride(listClaimProcsForClaim[i].ProcNum,listClaimProcsForClaim[i].PlanNum,listClaimProcsForClaim[i].InsSubNum,
						listClaimProcsForClaim[i].InsEstTotal,procListAll);
				}
			}
			#endregion
			#region Claim Update
			if(eobBehavior==EraBehaviors.DownloadAndReceive && claim.ClaimStatus!="R") {
				claim.ClaimStatus="R";
				claim.DateReceived=MiscData.GetNowDateTime();
			}
			claim.ClaimFee=listClaimProcsForClaim.Sum(x => x.FeeBilled)+listSavedLabClaimProcs.Sum(x => x.FeeBilled);
			claim.DedApplied=listClaimProcsForClaim.Sum(x => x.DedApplied)+listSavedLabClaimProcs.Sum(x => x.DedApplied);
			claim.InsPayEst=listClaimProcsForClaim.Sum(x => x.InsPayEst)+listSavedLabClaimProcs.Sum(x => x.InsPayEst);
			claim.InsPayAmt=listClaimProcsForClaim.Sum(x => x.InsPayAmt)+listSavedLabClaimProcs.Sum(x => x.InsPayAmt);
			claim.WriteOff=listClaimProcsForClaim.Sum(x => x.WriteOff)+listSavedLabClaimProcs.Sum(x => x.WriteOff);
			Claims.Update(claim);
			#endregion
			if(!isAutomatic && showProviderTransferWindow != null) {
				showProviderTransferWindow(claim,Patients.GetPat(claim.PatNum),Patients.GetFamily(claim.PatNum));
			}
			//Now that all claims and claimProcs are updated with payment information, update the account to show accurate patient portion amounts.
			List<PatPlan> patPlansForPatient=PatPlans.Refresh(claim.PatNum);
			Claims.CalculateAndUpdate(listPatProcs,listInsPlans,claim,patPlansForPatient,listBenefits,patient,listInsSubs);
			InsBlueBooks.SynchForClaimNums(claim.ClaimNum);
		}

		///<summary>The given number must be in the format of: [+-]?[0-9]*</summary>
		public static string RawMoneyStrToDisplayMoney(string number){
			string sign="";
			if(number.Length>0 && (number[0]=='+' || number[0]=='-')){
				sign=number[0].ToString();
				number=number.Substring(1,number.Length-1);
			}
			number=number.PadLeft(2,'0');//Guarantee at least 2 digits of length.
			return sign+number.Substring(0,number.Length-2).TrimStart('0').PadLeft(1,'0')+"."+
				number.Substring(number.Length-2,2);
		}

		///<summary>The rawPercent string should be of length 3 and should be numerical digits only.</summary>
		public static string RawPercentToDisplayPercent(string rawPercent){
			return rawPercent.TrimStart('0').PadLeft(1,'0');
		}

		/// <summary>Mimics FormClaimEdit.cs By Total button.</summary>
		private static ClaimProc ByTotClaimProcHelper(Claim claimCur,long payPlanNum,bool isPreEob,EraBehaviors eraBehavior) {
			ClaimProc cp=new ClaimProc();
			cp.IsNew=true;
			cp.ClaimNum=claimCur.ClaimNum;
			cp.PatNum=claimCur.PatNum;
			cp.ProvNum=claimCur.ProvTreat;
			cp.DedApplied=0;
			cp.Status=ClaimProcStatus.NotReceived;
			if(isPreEob) {
				cp.Status=ClaimProcStatus.Preauth;
			}
			else if(eraBehavior==EraBehaviors.DownloadAndReceive){
				cp.Status=ClaimProcStatus.Received;
				cp.DateEntry=DateTime.Now;//will get set anyway
				cp.DateCP=DateTime.Today;
			}
			cp.InsPayAmt=0;
			cp.PlanNum=claimCur.PlanNum;
			cp.InsSubNum=claimCur.InsSubNum;
			cp.ProcDate=claimCur.DateService;
			cp.ClinicNum=claimCur.ClinicNum;
			cp.PayPlanNum=payPlanNum;
			return cp;
		}

		private static string ConvertEOBVersion(string primaryEOB,string versionTo) {
			try {
				CCDFieldInputter primaryEOBfields=new CCDFieldInputter(primaryEOB);
				CCDField fieldA03=primaryEOBfields.GetFieldById("A03");
				if(fieldA03!=null && fieldA03.valuestr!=versionTo) {
					StringBuilder strb=new StringBuilder(primaryEOB);//todo: perform format conversion here.
					return strb.ToString();
				}
			}
			catch {
				//There was an error converting the primary EOB to the correct version. Just return the original and hope it works.
			}
			return primaryEOB;//No conversion necessary/possible.
		}

		///<summary>Throws exception in rare cases.  Used for ITRANS and Claimstream.
		///Takes a string, creates a file, and drops it into the clearinghouse export path.
		///Waits for the response, and then returns it as a string. Always returns the response if one comes back even when it is an error response.
		///The errorMsg will provide translated error details if an error occurred, otherwise errorMsg will be blank to indicate success.
		///Set network to null for ITRANS.  Set the network to Alberta Blue Cross (ABC), Telus A, or Telus B for ClaimStream.</summary>
		public static string PassToIca(string msgText,Clearinghouse clearinghouseClin,CanadianNetwork network,bool isAutomatic,out string errorMsg) {
			errorMsg="";
			if(clearinghouseClin==null){
				errorMsg=Lans.g("Canadian","A CDAnet compatible clearinghouse could not be found.");
				return "";//Return empty response, since we never received one.
			}
			bool isItrans=(clearinghouseClin.CommBridge==EclaimsCommBridge.ITRANS);
			bool isClaimstream=(clearinghouseClin.CommBridge==EclaimsCommBridge.Claimstream);
			string saveFolder=clearinghouseClin.ExportPath;
			if(isClaimstream) {
				DirectoryInfo dirInfo=new DirectoryInfo(saveFolder);
				if(dirInfo.Name=="abc") {//Is pointing to the "abc" sub-folder.
					//For backwards compatibility, if the export path is pointing to the "abc" sub-folder,
					//then automatically move up one directory to the parent directory before deciding which sub-folder to use (see below for sub-folder).
					saveFolder=dirInfo.Parent.FullName;
				}
				string subDir="";
				if(network.Abbrev=="ABC") {//Alberta Blue Cross
					subDir="abc";
				}
				else if(network.Abbrev=="TELUS A") {
					subDir="telusa";
				}
				else if(network.Abbrev=="TELUS B") {
					subDir="telusb";
				}
				else {
					errorMsg=Lans.g("Canadian","ClaimStream does not support this transaction for network")+" "+network.Descript;
					return "";//Return empty response, since we never received one.
				}
				saveFolder=ODFileUtils.CombinePaths(saveFolder,subDir);
			}
			if(!Directory.Exists(saveFolder)) {
				errorMsg=saveFolder+" "+Lans.g("Canadian","not found.");
				return "";//Return empty response, since we never received one.
			}
			if(isClaimstream) {
				string certFileName="";
				if(network.Abbrev=="ABC") {//Alberta Blue Cross
					certFileName="OPENDENTAL.pem";
				}
				else if(network.Abbrev=="TELUS A" || network.Abbrev=="TELUS B") {
					if(ODBuild.IsDebug()) {
						certFileName="OD_2018-02-26_2023-03-02_staging.pem";
					}
					else {
						certFileName="OD_2018-05-17_2023-05-21_prod.pem";
					}
				}
				string certFilePath=ODFileUtils.CombinePaths(saveFolder,certFileName);
				if(!File.Exists(certFilePath)) {
					byte[] arrayCertFileBytes=null;
					try {
						XmlWriterSettings settings=new XmlWriterSettings();
						settings.Indent=true;
						settings.IndentChars=("    ");
						StringBuilder strbuild=new StringBuilder();
						using(XmlWriter writer=XmlWriter.Create(strbuild,settings)){
							writer.WriteElementString("RegistrationKey",PrefC.GetString(PrefName.RegistrationKey));
						}
						string response=null;
						if(network.Abbrev=="ABC") {//Alberta Blue Cross
							response=CustomerUpdatesProxy.GetWebServiceInstance().RequestCertCanadaABC(strbuild.ToString());
						}
						else if(network.Abbrev=="TELUS A" || network.Abbrev=="TELUS B") {
							if(ODBuild.IsDebug()) {
								response=CustomerUpdatesProxy.GetWebServiceInstance().RequestCertCanadaTelusAandBtest(strbuild.ToString());
							}
							else{
								response=CustomerUpdatesProxy.GetWebServiceInstance().RequestCertCanadaTelusAandB(strbuild.ToString());
							}
						}
						XmlDocument doc=new XmlDocument();
						doc.LoadXml(response);
						XmlNode node=doc.SelectSingleNode("//Error");
						if(node!=null) {
							errorMsg=node.InnerText;
							return "";//Return empty response, since we never received one.
						}
						node=doc.SelectSingleNode("//KeyDisabled");
						if(node!=null) {
							if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,true)) {
								Signalods.Insert(new Signalod() { IType=InvalidType.Prefs });
								Prefs.RefreshCache();
							}
							errorMsg=node.InnerText;
							return "";//Return empty response, since we never received one.
						}
						//no error, and no disabled message
						if(Prefs.UpdateBool(PrefName.RegistrationKeyIsDisabled,false)) {
							Signalods.Insert(new Signalod() { IType=InvalidType.Prefs });
							Prefs.RefreshCache();
						}
						node=doc.SelectSingleNode("//FileData64");
						arrayCertFileBytes=Convert.FromBase64String(node.InnerText);
					}
					catch(Exception ex) {
						errorMsg=Lans.g("Canadian","Failed to download certificate file")+"\r\n  "+ex.Message;
						return "";//Return empty response, since we never received one.
					}
					try {
						File.WriteAllBytes(certFilePath,arrayCertFileBytes);
					}
					catch(Exception ex) {
						errorMsg=Lans.g("Canadian","Failed to export certificate file to path")+" '"+certFilePath+"'\r\n  "+ex.Message;
						return "";//Return empty response, since we never received one.
					}
				}
			}
			string officeSequenceNumber=msgText.Substring(12,6);//Field A02. Office Sequence Number is always part of every message type and is always in the same place.
			int fileNum=PIn.Int(officeSequenceNumber)%1000;
			//first, delete the result file from previous communication so that no such files can affect the loop logic below.
			string outputFile=ODFileUtils.CombinePaths(saveFolder,"output."+fileNum.ToString().PadLeft(3,'0'));
			if(File.Exists(outputFile)) {
				try {
					File.Delete(outputFile);//no exception thrown if file does not exist.
				}
				catch(Exception ex) {//Will throw if the file does exist but cannot be deleted.
					errorMsg=Lans.g("Canadian","Failed to remove old output file to make room for new output file.  Please try again.  File: ")+" '"+outputFile+"'\r\n"+ex.Message;
					return "";//Return empty response, since we never received one.
				}
			}
			try {
				//create the input file with data:
				string tempInputFile=ODFileUtils.CombinePaths(saveFolder,"tempinput."+fileNum.ToString().PadLeft(3,'0'));
				//First, write to a temp file so that the clearinghouse software does not try to send the file while it is still being written.
				File.WriteAllText(tempInputFile,msgText,Encoding.GetEncoding(850));
				//Now that the file is completely written, rename it to the input format that the clearinghouse will recognize and process.
				string inputFile=ODFileUtils.CombinePaths(saveFolder,"input."+fileNum.ToString().PadLeft(3,'0'));
				File.Move(tempInputFile,inputFile);//The input file should not exist, because the clearinghouse software should process input files promptly, unless the clearinghouse service is off for 1000 transactions in a row. We want an exception to be thrown if this file already exists.
			}
			catch(Exception ex) {
				errorMsg=Lans.g("Canadian","Failed to save outgoing message to file.")+"  "+ex.Message;
				return "";//Return empty response, since we never received one.
			}
			DateTime start=DateTime.Now;
			while(DateTime.Now<start.AddSeconds(120)){//We wait for up to 120 seconds. Responses can take up to 95 seconds and we need some extra time to be sure.
				if(File.Exists(outputFile)){
					break;
				}
				Thread.Sleep(200);//2/10 second
				Application.DoEvents();
			}
			//The _nput.### file is just the input.### renamed after it is processed. The clearinghouse service renames the file so that it is not processed more than once.
			string nputFile=ODFileUtils.CombinePaths(saveFolder,"_nput."+fileNum.ToString().PadLeft(3,'0'));
			//We delete the intermediate file so that claim data is not just lying around.
			if(File.Exists(nputFile)) {//The file would not appear to exist if there was a permission issue.
				ODException.SwallowAnyException(() => {//Will throw if the file exists but cannot be deleted (ex if ITRANS is still using it).
					File.Delete(nputFile);//no exception thrown if file does not exist.
				});
			}
			if(!File.Exists(outputFile)) {
				if(isItrans) {
					errorMsg=Lans.g("Canadian","No response from iCAService (ITRANS) or ICDService (ITRANS 2.0). Ensure that the correct service for your version of ITRANS is started and the corresponding folder has the necessary permissions.");
				}
				else if(isClaimstream) {
					errorMsg=Lans.g("Canadian","No response from the CCDWS service. Ensure that the CCDWS service is started and the ccd folder has the necessary permissions.");
				}
				else {//Other clearinghouses, if we ever support them.
					errorMsg=Lans.g("Canadian","No response from clearinghouse service. Ensure that the clearinghouse service is started and the export folder has the necessary permissions.");
				}
				return "";//Return empty response, since we never received one.
			}
			byte[] resultBytes=null;
			try {
				resultBytes=File.ReadAllBytes(outputFile);
			}
			catch(Exception ex) {
				errorMsg=Lans.g("Canadian","Failed to read response from file.")+"  "+ex.Message;
				return "";//Return empty response, since we never received one.
			}
			string result=Encoding.GetEncoding(850).GetString(resultBytes);
			//strip the prefix.  Example prefix: 123456,0,000,
			string resultPrefix="";
			//Find position of third comma
			Match match=Regex.Match(result,@"^\d*,\d+,\d+,");
			if(!match.Success) {
				match=Regex.Match(result,@"^\d*,\d+,\d+$");//The 3rd comma is not present when the result message is blank.  Matches #,#,# only.
			}
			if(match.Success){
				resultPrefix=result.Substring(0,match.Length);
				result=result.Substring(resultPrefix.Length);
			}
			//We delete the output file so that claim data is not just lying around.
			if(File.Exists(outputFile)) {//The file would not appear to exist if there was a permission issue.
				ODException.SwallowAnyException(() => {//Will throw if the file exists but cannot be deleted (ex if ITRANS is still using it).
					File.Delete(outputFile);//no exception thrown if file does not exist.
				});
			}
			if(result.Length>=25 && result.Substring(12).StartsWith("NO MORE ITEMS")) {
				//Since the message does not have a well defined format, we skip the code below for parsing the result to check for a mailbox indicator.
				//Additionally, there is no need to check for a mailbox indicator because this message from ITRANS tells us that the mailbox is empty anyway.
				return result;
			}
			if(result.Length<42) {//The shortest message is a version 02 Request for Pended Claims with length 42. Any message shorter is considered to be an error message.
				//The only valid message less than 42 characters in length is an Outstanding Transactions Acknowledgement indicating that the mailbox is empty. 
				string[] responses=resultPrefix.Split(',');
				string errorCode=Lans.g("Canadian","UNKNOWN");
				if(responses.Length >= 2) {
					errorCode=responses[1];
				}
				errorMsg=Lans.g("Canadian","Error")+" "+errorCode+"\r\n\r\n"+Lans.g("Canadian","Raw Response")+":\r\n"+resultPrefix+result+"\r\n\r\n";
				if(isItrans) {
					if(errorCode=="1013") {
						errorMsg+=Lans.g("Canadian","The CDA digital certificate for the provider is either missing, not exportable, expired, or invalid.")+"\r\n";
					}
					errorMsg+="\r\n"+Lans.g("Canadian","Please see http://www.goitrans.com/itrans-support-error-codes/ for more details.")+"\r\n";
					string dirClientProgram="";
					ODException.SwallowAnyException(() => dirClientProgram=Path.GetDirectoryName(clearinghouseClin.ClientProgram));
					string errorFile=ODFileUtils.CombinePaths(dirClientProgram,"ica.log");
					string errorlog="";
					if(File.Exists(errorFile)) {
						try { 
							errorlog=File.ReadAllText(errorFile);
							errorMsg+=Lans.g("Canadian","Error log")+":\r\n"+errorlog;
						}
						catch(Exception ex) {
							errorMsg=Lans.g("Canadian","Failed to read error log file")+$" '{errorFile}':"+"\r\n"+ex.Message;
						}
					}
				}
				else if(isClaimstream) {					
					string errorDescription="";
					string errorMessage=GetErrorMessageForCodeClaimstream(errorCode,ref errorDescription);
					errorMsg+=Lans.g("Canadian","Error Message")+": "+Lans.g("Canadian",errorMessage)+"\r\n";
					errorMsg+=Lans.g("Canadian","Error Description")+": "+Lans.g("Canadian",errorDescription)+"\r\n\r\n";
					errorMsg+=Lans.g("Canadian","For further error details, read the log file ccdws.log.");
				}
				return result;
			}
			else {//Message is long enough to possibly be a valid response.  Try parsing it so we can check the mailbox indicator.
				try {
					CCDFieldInputter messageData=new CCDFieldInputter(result);
					//We check the mailbox indicator here, only when the response is returned the first time instead of showing it in FormCCDPrint, because 
					//we do not want the mailbox indicator to be examined multiple times, which could happen if a transaction is viewed again using FormCCDPrint.
					CCDField mailboxIndicator=messageData.GetFieldById("A11");
					if(mailboxIndicator!=null) { //Field A11 should exist in all response types, but just in case.
						if(mailboxIndicator.valuestr.ToUpper()=="Y" || mailboxIndicator.valuestr.ToUpper()=="O"
							&& !isAutomatic) {
							MessageBox.Show(Lans.g("Canadian","NOTIFICATION: Items are waiting in the mailbox. Retrieve these items by going to the Manage module, click the Send Claims button, "
								+"then click the Outstanding button. This box will continue to show each time a claim is sent until the mailbox is cleared."));
						}
					}
				}
				catch(Exception ex) {
					errorMsg+=Lans.g("Canadian","Response is not formatted according to message standards.")+"\r\n"+ex.Message;
				}
			}
			return result;
		}

		///<summary>Examines the errorCode passed in and returns a short not translated error message string and sets the errorDescription.
		///Works for status codes and internal codes.</summary>
		private static string GetErrorMessageForCodeClaimstream(string errorCode,ref string errorDescription) {
			if(errorCode=="0") {//Status code
				errorDescription="The request was sent to the remote server and the response was successfully received and stored in the output file.";
				return "Success";
			}
			else if(errorCode=="1001") {//Status code
				errorDescription="A general error occurred. Check the log file for details, including internal error.";
				return "General error";
			}
			else if(errorCode=="1013") {
				errorDescription="The CDA digital certificate for the provider is either missing, not exportable, expired, or invalid.";
				return "Could not find valid certificate for provider";
			}
			else if(errorCode=="1026") {//Status code
				errorDescription="CCDWS could not connect to the remote server.";
				return "No answer";
			}
			else if(errorCode=="1033") {//Status code
				errorDescription="The input file could not be read.";
				return "Error reading input";
			}
			else if(errorCode=="1034") {//Status code
				errorDescription="The input request does not conform to CDAnet v2, v3 or v4 message standards. See the log file for details.";
				return "Request invalid";
			}
			else if(errorCode=="1042") {//Status code
				errorDescription="A timeout occurred either sending the message to the server (write timeout) or receiving a response from the server (read timeout).";
				return "Server timeout";
			}
			else if(errorCode=="1043") {//Status code
				errorDescription="The server responded with unexpected data. This may happen if the server address is incorrect or the server is not correctly configured.";
				return "Invalid characters";
			}
			else if(errorCode=="1045") {//Status code
				errorDescription="The server disconnected unexpectedly. This will most frequently occur if a connection was made, but an SSL error was detected.";
				return "Server disconnect";
			}
			else if(errorCode=="2") {//Internal error code
				errorDescription="The specified file or directory does not exist or cannot be found. This message can occur whenever a specified file does not exist or a component of a path does not specify an existing directory.";
				return "No such file or directory";
			}
			else if(errorCode=="8") {//Internal error code
				//errorDescription="Not enough memory on CCDWS host, or there is a memory leak. Try rebooting workstation. Contact support personnel if problem persists.";
				errorDescription="Not enough memory on CCDWS host, or there is a memory leak. Try rebooting workstation.";
				return "Out of memory";
			}
			else if(errorCode=="13") {//Internal error code
				errorDescription="Application may not have permissions to read, write or execute one or more files, such as a log file, configuration file etc. Ensure that CCDWS user has rights to all of its read and write locations.";
				return "Permission denied";
			}
			else if(errorCode=="21") {//Internal error code
				errorDescription="The file attempting to be accessed is a directory, not a regular file. Check the configuration option of the file being used, and check the file system to ensure that a directory of the same name does not exist.";
				return "Is a directory";
			}
			else if(errorCode=="24") {//Internal error code
				errorDescription="No more file descriptors are available, so no more files can be opened.";
				return "Too many open files";
			}
			else if(errorCode=="112") {//Internal error code
				//errorDescription="Log files and/or temporary files cannot be written because there is no space left on the hard disk. Free up space by deleting old files, check the disk partitions to be certain that it is correctly partitioned, and look into purchasing a larger disk if necessary.";
				errorDescription="Log files and/or temporary files cannot be written because there is no space left on the hard disk.";
				return "There is not enough space on the disk";
			}
			else if(errorCode=="6012") {//Internal error code
				errorDescription="Payload contents were not in the expected format. Remote client is sending invalid data.";
				return "Payload parse error";
			}
			else if(errorCode=="7001") {//Internal error code
				errorDescription="A fault occurred, and the server has determined it is a problem with CCDWS. Ensure the configuration for the destination is correct.";
				return "SOAP: The service returned a client fault";
			}
			else if(errorCode=="7002") {//Internal error code
				//errorDescription="A fault occurred, and the server has determined it is a problem with the server itself. If the problem continues, contact support personnel.";
				errorDescription="A fault occurred, and the server has determined it is a problem with the server itself.";
				return "SOAP: The service returned a server fault";
			}
			else if(errorCode=="7011") {//Internal error code
				//errorDescription="Contact support personnel.";
				errorDescription="Contact CCDWS support personnel.";
				return "SOAP: Internal error";
			}
			else if(errorCode=="7012") {//Internal error code
				errorDescription="General SOAP fault. Neither client nor server has been specified. Look for further information in the log file.";
				return "SOAP: An exception raised by the service";
			}
			else if(errorCode=="7014") {//Internal error code
				errorDescription="An HTTP response was returned but did not contain a body.";
				return "SOAP: No data in HTTP message";
			}
			else if(errorCode=="7020") {//Internal error code
				//errorDescription="Not enough memory on CCDWS to create SOAP objects, or there is a memory leak. If host has sufficient memory ensure other memory intensive applications are not using it up. Try rebooting workstation. Contact support personnel if problem persists.";
				errorDescription="Not enough memory on CCDWS to create SOAP objects. Try rebooting workstation.";
				return "SOAP: Out of memory";
			}
			else if(errorCode=="7023") {//Internal error code
				//errorDescription="The WSDL expects a non-null element, but a null element was found. Contact support personnel.";
				errorDescription="The WSDL expects a non-null element, but a null element was found.";
				return "SOAP: An element was null while it is not supposed to be null";
			}
			else if(errorCode=="7028") {//Internal error code
				errorDescription="The endpoint is not specified in the configuration file or the destination host is not accepting connections on the expected port. Confirm the endpoint with the destination administrator. Ensure firewall is allowing access to the endpoint.";
				return "SOAP: A connection error occurred";
			}
			else if(errorCode=="7030") {//Internal error code
				//errorDescription="A general SSL occurred. Details may be displayed a fault message in the log.";
				errorDescription="A general SSL occurred. Details may be displayed in a fault message in the log.";
				return "SOAP: An SSL error occurred";
			}
			else if(errorCode=="7039") {//Internal error code
				errorDescription="Check the endpoint for the destination refers to a webservice destination. HTTP responses that contain data but not a SOAP response will generate this error.";
				return "SOAP: SOAP version mismatch or no SOAP message";
			}
			else if(errorCode=="7098") {//Internal error code
				//errorDescription="A timeout occurred waiting for a response. Contact support personnel if problem persists.";
				errorDescription="A timeout occurred waiting for a response.";
				return "SOAP: Read timeout";
			}
			else if(errorCode=="7099") {//Internal error code
				//errorDescription="This can occur if the destination server terminated the connection. If the error continues contact support personnel.";
				errorDescription="This can occur if the destination server terminated the connection.";
				return "SOAP: Server disconnect";
			}
			else if(errorCode=="7400") {//Internal error code
				errorDescription="The web service appears to be malformed. Check that all of the configuration options for the destination are appropriate.";
				return "HTTP: Bad Request";
			}
			else if(errorCode=="7401") {//Internal error code
				errorDescription="The endpoint attempting to be accessed requires authentication. Check that all of the configuration options for the destination are appropriate.";
				return "HTTP: Unauthorized";
			}
			else if(errorCode=="7403") {//Internal error code
				errorDescription="The server has forbidden access to the endpoint attempting to be accessed. Check that all of the configuration options for the destination are appropriate.";
				return "HTTP: Forbidden";
			}
			else if(errorCode=="7404") {//Internal error code
				errorDescription="The endpoint's path is not valid. Check that all of the configuration options for the destination are appropriate.";
				return "HTTP: Not Found";
			}
			else if(errorCode=="7405") {//Internal error code
				errorDescription="Some servers are poorly configured where the endpoint is a path or is to be redirected. If it is specified with a trailing slash, ensure that this is provided on the endpoint.";
				return "HTTP: Method Not Allowed";
			}
			else if(errorCode=="7407") {//Internal error code
				errorDescription="Ensure that the ProxyPassword configuration option is populated correctly, and that all other proxy related options are valid.";
				return "HTTP: Proxy Authentication. Required";
			}
			else if(errorCode=="7408") {//Internal error code
				//errorDescription="Problem writing data to the destination host. Try again, and contact support personnel if the problem persists.";
				errorDescription="Problem writing data to the destination host.";
				return "HTTP: Request Timeout";
			}
			else if(errorCode=="7415") {//Internal error code
				//errorDescription="This may be the result of an incompatibility between the SOAP version submitted by CCDWS and that of the destination server. Contact support personnel.";
				errorDescription="This may be the result of an incompatibility between the SOAP version submitted by CCDWS and that of the destination server.";
				return "HTTP: Unsupported Media Type";
			}
			else if(errorCode=="7500") {//Internal error code
				//errorDescription="An internal error occurred at the destination server. Contact support personnel.";
				errorDescription="An internal error occurred at the destination server.";
				return "HTTP: Internal Server Error";
			}
			else if(errorCode=="7501") {//Internal error code
				//errorDescription="The destination server does not support the functionality to fulfil the request. Confirm that the destination endpoint is correct. Contact support personnel if the problem persists.";
				errorDescription="The destination server does not support the functionality to fulfil the request. Confirm that the destination endpoint is correct.";
				return "HTTP: Not Implemented";
			}
			else if(errorCode=="7502") {//Internal error code
				//errorDescription="The destination server received an invalid response from an upstream server. Contact support personnel if the problem persists.";
				errorDescription="The destination server received an invalid response from an upstream server.";
				return "HTTP: Bad Gateway";
			}
			else if(errorCode=="7503") {//Internal error code
				//errorDescription="The destination server is unable to handle the request due to temporary overloading or maintenance of the server. Contact support personnel if the problem persists.";
				errorDescription="The destination server is unable to handle the request due to temporary overloading or maintenance of the server.";
				return "HTTP: Service Unavailable";
			}
			else if(errorCode=="7504") {//Internal error code
				//errorDescription="The destination server did not receive a timely response from an upstream server. Contact support personnel if the problem persists.";
				errorDescription="The destination server did not receive a timely response from an upstream server.";
				return "HTTP: Gateway Timeout";
			}
			else if(errorCode=="7505") {//Internal error code
				//errorDescription="The SOAP version supported by CCDWS and that of the destination are not compatible. Contact support personnel.";
				errorDescription="The SOAP version supported by CCDWS and that of the destination are not compatible.";
				return "HTTP: HTTP Version not supported";
			}
			errorDescription="UNKNOWN";
			return "UNKNOWN";
		}

		public static char GetCanadianChar(byte b){
			return Encoding.GetEncoding(850).GetString(new byte[] {b})[0];
		}

		///<summary>Only used for Canadian messages. If carrier is not null, then returns the first clearinghouse with a payorid matching the carrier 
		///if one exists. If no carrier specific clearinghouses are located, then returns the default clearinghouse if it is Canadian. Otherwise, in the
		///worst case, will return the first Canadian clearinghouse that it can find. If for some reason no Canadian clearinghouses exist, then null is
		///returned.  Returns the HQ level clearinghouse.</summary>
		public static Clearinghouse GetCanadianClearinghouseHq(Carrier carrier){
			string payerid="";
			if(carrier!=null) {
				payerid=carrier.ElectID;
			}
			//Returns the default dental clearinghouse if the payerid is not associated to any clearinghouses.
			long hqClearinghouseNum=Clearinghouses.AutomateClearinghouseHqSelection(payerid,EnumClaimMedType.Dental);
			if(hqClearinghouseNum!=0) {
				Clearinghouse clearinghouseHq=Clearinghouses.GetClearinghouse(hqClearinghouseNum);
				if(clearinghouseHq.Eformat==ElectronicClaimFormat.Canadian) {//Might not be Canadian, if for instance they accidentally set their default clearinghouse to an American clearinghouse.
					return clearinghouseHq;
				}
			}
			//Return the default dental clearinghouse if it is a Canadian clearinghouse.
			Clearinghouse clearinghouse=Clearinghouses.GetFirstOrDefault(x => PrefC.GetLong(PrefName.ClearinghouseDefaultDent)==x.ClearinghouseNum 
				&& x.Eformat==ElectronicClaimFormat.Canadian);
			if(clearinghouse==null) {
				//Return the first Canadian clearinghouse if one exists.
				clearinghouse=Clearinghouses.GetFirstOrDefault(x => x.Eformat==ElectronicClaimFormat.Canadian);
			}
			//Can return null if no Canadian clearinghouses exists.
			//Should never happen, unless the user manually changes the ITRANS and ClaimStream clearinghouses to another electronic format.
			return clearinghouse;
		}

		///<summary>Decimal.</summary>
		public static string TidyD(double number,int width){
			string retVal=(number*100).ToString("F0");
			if(retVal.Length>width) {
				return retVal.Substring(0,width);//this should never happen, but it might prevent a malformed claim.
			}
			return retVal.PadLeft(width,'0');
		}

		///<summary>Numeric</summary>
		public static string TidyN(int number,int width){
			string retVal=number.ToString();
			if(retVal.Length>width){
				return retVal.Substring(0,width);//this should never happen, but it might prevent a malformed claim.
			}
			return retVal.PadLeft(width,'0');
		}

		///<summary>Numeric</summary>
		public static string TidyN(string numText,int width) {
			string retVal="";
			try{
				int number=Convert.ToInt32(numText);
				retVal=number.ToString();
			}
			catch{
				retVal="";
			}
			if(retVal.Length>width) {
				return retVal.Substring(0,width);//this should never happen, but it might prevent a malformed claim.
			}
			return retVal.PadLeft(width,'0');
		}

		///<summary>This should never involve user input and is rarely used.  It only handles width and padding.</summary>
		public static string TidyA(string text,int width){
			if(text.Length>width) {
				return text.Substring(0,width);
			}
			return text.PadRight(width,' ');
		}

		///<summary>Alphabetic in addition to apost, dash, comma, space, and extended characters (128-255). No numbers. Returns string will letters all upper-case. For testing, here are some extended chars: à â ç é è ê ë î ï ô û ù ü ÿ</summary>
		public static string TidyAE(string text,int width) {
			return TidyAE(text,width,false);
		}

		///<summary>Alphabetic in addition to apost, dash, comma, space, and extended characters (128-255). No numbers. For testing, here are some extended chars: à â ç é è ê ë î ï ô û ù ü ÿ</summary>
		public static string TidyAE(string text,int width,bool allowLowercase) {
			if(!allowLowercase) {
				text=text.ToUpper();
			}
			text=Regex.Replace(text,//replace
				@"[0-9]",//any character that's a number
				"");//with nothing (delete it). This extra replace is needed, because \w in the replace below includes numerical digits.
			text=Regex.Replace(text,//replace
				@"[^\w'\-, ]",//any character that's not a word character or an apost, dash, comma, or space
				"");//with nothing (delete it)
			if(text.Length>width) {
				return text.Substring(0,width);
			}
			return text.PadRight(width,' ');
		}

		///<summary>Alphabetic/Numeric, no extended characters (128-255).  Returns string with letters all upper-case.</summary>
		public static string TidyAN(string text,int width) {
			return TidyAN(text,width,false);
		}

		///<summary>Alphabetic/Numeric, no extended characters (128-255).</summary>
		public static string TidyAN(string text,int width,bool allowLowercase) {
			if(!allowLowercase){
				text=text.ToUpper();
			}
			text=Regex.Replace(text,//replace
				@"[^a-zA-Z0-9 '\-,]",//any char that is not A-Z,0-9,space,apost,dash,or comma
				"");//with nothing (delete it)
			if(text.Length>width) {
				return text.Substring(0,width);
			}
			return text.PadRight(width,' ');
		}

		///<summary>Alphabetic/Numeric, with extended characters (128-255).</summary>
		public static string TidyAEN(string text,int width) {
			return TidyAEN(text,width,false);
		}

		///<summary>Alphabetic/Numeric with extended characters (128-255). Only handles width and padding. For testing, here are some: à â ç é è ê ë î ï ô û ù ü ÿ</summary>
		public static string TidyAEN(string text,int width,bool allowLowercase) {
			if(!allowLowercase) {
				text=text.ToUpper();
			}
			if(text.Length>width) {
				return text.Substring(0,width);
			}
			return text.PadRight(width,' ');
		}

		///<summary>Must always return single char.</summary>
		private static string GetMaterialsForwarded(string materials){
			bool E=materials.Contains("E");
			bool C=materials.Contains("C");
			bool M=materials.Contains("M");
			bool X=materials.Contains("X");
			bool I=materials.Contains("I");
			if(E&&C&&M&&X&&I){
				return "Z";
			}
			if(C&&M&&X&&I) {
				return "Y";
			}
			if(E&&C&&X&&I) {
				return "W";
			}
			if(E&&C&&M&&I) {
				return "V";
			}
			if(E&&C&&M&&X) {
				return "U";
			}
			if(M&&X&&I) {
				return "T";
			}
			if(C&&M&&X) {
				return "R";
			}
			if(C&&M&&I) {
				return "R";
			}
			if(E&&C&&I) {
				return "Q";
			}
			if(E&&C&&X) {
				return "P";
			}
			if(E&&C&&M) {
				return "O";
			}
			if(X&&I) {
				return "N";
			}
			if(M&&I) {
				return "L";
			}
			if(M&&X) {
				return "K";
			}
			if(C&&I) {
				return "J";
			}
			if(C&&X) {
				return "H";
			}
			if(C&&M) {
				return "G";
			}
			if(E&&I) {
				return "F";
			}
			if(E&&X) {
				return "D";
			}
			if(E&&M) {
				return "B";
			}
			if(E&&C) {
				return "A";
			}
			if(I) {
				return "I";
			}
			if(X) {
				return "X";
			}
			if(M) {
				return "M";
			}
			if(C) {
				return "C";
			}
			if(E) {
				return "E";
			}
			return " ";
		}

		///<summary>Used in C03 and E06</summary>
		public static string GetRelationshipCode(Relat relat){
			switch (relat){
				case Relat.Self:
					return "1";
				case Relat.Spouse:
					return "2";
				case Relat.Child:
					return "3";
				case Relat.LifePartner:
				case Relat.SignifOther:
					return "4";//commonlaw spouse
				default://other (ex elderly care)
					return "5";
			}
		}

		public static string GetPlanFlag(string planFlag){
			if(planFlag=="A" || planFlag=="V"){
				return planFlag;
			}
			return " ";
		}

		///<summary>Because the numberins scheme is slightly different for version 2, this field (C09) should always be passed through here.</summary>
		public static string GetEligibilityCode(byte rawCode,bool isVersion02) {
			if(isVersion02 && rawCode==4) {
				return "0";
			}
			return rawCode.ToString();
		}

		///<summary>Checks for either valid USA state or valid Canadian territory.</summary>
		private static bool IsValidST(string ST){
			if(IsValidTerritory(ST) || IsValidState(ST)){
				return true;
			}
			return false;
		}

		///<summary>Checks for valid USA state.</summary>
		private static bool IsValidState(string ST){
			string[] validStates=new string[] {
				"AL","AK","AZ","AR","CA","CO","CT","DE","DC","FL","GA","HI","ID","IL","IN","IA","KS","KY","LA","ME","MD","MA","MI",
				"MN","MS","MO","MT","NE","NV","NH","NJ","NM","NY","NC","ND","OH","OK","OR","PA","RI","SC","SD","TN","TX","UT","VA",
				"WA","WV","WI","WY"};
			for(int i=0;i<validStates.Length;i++){
				if(validStates[i]==ST){
					return true;
				}
			}
			return false;
		}

		///<summary>Checks for valid Canadian terriroty.</summary>
		private static bool IsValidTerritory(string ST){
			//http://www.nrcan.gc.ca/earth-sciences/geography-boundary/geographical-name/translators/5782
			string[] validStates=new string[] { 
				//Canadian province codes.
				"AB",//Alberta
				"BC",//Britich Columbia
				"MB",//Manitoba
				"NB",//New Brunswick
				"NL",//Newfoundland and Labrador
				"NS",//Nova Scotia
				"NT",//Northwest Territories
				"NU",//Nunavut
				"ON",//Ontario
				"PE",//Prince Edward Island
				"QC",//Quebec
				"SK",//Saskatchewan
				"YT", //Yukon
				//Traditional Canadian province codes which somehow made it into our application, but we are going to leave them because they are probably harmless.
				"LB",//Newfoundland and Labrador - This appeared in Canada Post publications (e.g., The Canadian Postal Code Directory) for the mainland section of the province of Newfoundland and Labrador.
				"NF",//Newfoundland and Labrador - Nfld. and later NF (the two-letter abbreviation used before the province's name changed to Newfoundland and Labrador) and T.-N. (French version, for Terre-Neuve)
				"PQ",//Quebec	- Que. and P.Q. (French version, for Province du Québec); later, PQ evolved from P.Q. as the first two-letter non-punctuated abbreviation.
			};
			for(int i=0;i<validStates.Length;i++){
				if(validStates[i]==ST){
					return true;
				}
			}
			return false;
		}

		///<summary>Validates USA and Canadian postal codes.</summary>
		private static bool IsValidZip(string zip){
			if(Regex.IsMatch(zip.Trim(),@"^[0-9]{5}$")) {//USA 5 digit zip code.
				return true;
			}
			if(Regex.IsMatch(zip.Trim().Replace("-",""),@"^[0-9]{9}$")) {//USA 9 digit zip code.
				return true;
			}
			if(Regex.IsMatch(zip.Trim(),@"^[A-Z][0-9][A-Z][ ]?[0-9][A-Z][0-9]$")) {//Canadian postal code. ANANAN or ANA NAN, with whitespace in front or behind.
				return true;
			}
			return false;
		}

		///<summary>The province/territory of Quebec in Canada has many different rules and regulations than the rest of Canada.
		///We need this helper function in order to identify when special circumstances are necessary.
		///Be sure to also check that the region is set to Canada.</summary>
		public static bool IsQuebec() {
			string state=PrefC.GetString(PrefName.PracticeST).ToLower();
			if(state=="qc" || state=="quebec" || state=="québec") {//Alt code 0233 for the é
				return true;
			}
			return false;
		}

		private static string GetToothQuadOrArch(Procedure proc,ProcedureCode procCode){
			//See the ODA Suggested Fee Guide for General Practitioners for year 2017 page 71 for the
			//Identification System for Arches, Quadrants, Sextants, Joints.
			switch(procCode.TreatArea){
				case TreatmentArea.Arch:
					if(proc.Surf.ToUpper()=="U"){
						return "01";
					}
					else if(proc.Surf.ToUpper()=="L") {
						return "02";
					}
					return "00";//Full mouth.
				case TreatmentArea.Mouth:
				case TreatmentArea.None:
					return "00";
				case TreatmentArea.Quad:
					if(proc.Surf=="UR"){
						return "10";
					}
					else if(proc.Surf=="UL") {
						return "20";
					}
					else if(proc.Surf=="LR") {
						return "40";
					}
					else{//LL
						return "30";
					}
				case TreatmentArea.Sextant:
					return Tooth.GetSextant(proc.Surf,ToothNumberingNomenclature.FDI);
				case TreatmentArea.Surf:
				case TreatmentArea.Tooth:
					return Tooth.ToInternat(proc.ToothNum);
				case TreatmentArea.ToothRange:
					string[] range=proc.ToothRange.Split(',');
					if(range.Length==0 || !Tooth.IsValidDB(range[0])){
						return "00";
					}
					else if(Tooth.IsMaxillary(range[0])){
						return "00";
					}
					return "00";
			}
			return "00";//will never happen
		}

		///<summary>Returns a string describing all missing data on this claim.  Claim will not be allowed to be sent electronically unless this string comes back empty.</summary>
		public static string GetMissingData(ClaimSendQueueItem queueItem) {
			string retVal="";
			Claim claim=Claims.GetClaim(queueItem.ClaimNum);
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider billProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvBill)??providerFirst;
			Provider treatProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat)??providerFirst;
			InsSub insSub=InsSubs.GetSub(claim.InsSubNum,new List<InsSub>());
			InsPlan insPlan=InsPlans.GetPlan(claim.PlanNum,new List <InsPlan> ());
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			if(carrier.CanadianNetworkNum==0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Primary carrier network not set";
			}
			if(carrier.CDAnetVersion!="02") {
				if(carrier.CDAnetVersion!="04") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Primary carrier CDANet version must be 02 or 04";
				}
				if(carrier.CanadianEncryptionMethod!=1 && carrier.CanadianEncryptionMethod!=2 && carrier.CanadianEncryptionMethod!=3) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Primary carrier encryption method must be 1, 2 or 3";
				}
			}
			InsSub insSub2=null;
			InsPlan insPlan2=null;
			Carrier carrier2=null;
			Patient subscriber2=null;
			if(claim.ClaimType!="S" && claim.PlanNum2>0) {//Is primary claim and the patient has secondary insurance.
				insSub2=InsSubs.GetSub(claim.InsSubNum2,new List<InsSub>());
				insPlan2=InsPlans.GetPlan(claim.PlanNum2,new List <InsPlan> ());
				carrier2=Carriers.GetCarrier(insPlan2.CarrierNum);
				subscriber2=Patients.GetPat(insSub2.Subscriber);
			}
			Patient patient=Patients.GetPat(claim.PatNum);
			Patient subscriber=Patients.GetPat(insSub.Subscriber);
			List<ClaimProc> claimProcList=ClaimProcs.Refresh(patient.PatNum);//all claimProcs for a patient.
			List<ClaimProc> claimProcsClaim=ClaimProcs.GetForSendClaim(claimProcList,claim.ClaimNum);
			List<Procedure> procListAll=Procedures.Refresh(claim.PatNum);
			Procedure proc;
			ProcedureCode procCode;
			List<Procedure> extracted=Procedures.GetCanadianExtractedTeeth(procListAll);
			if(!Regex.IsMatch(carrier.ElectID,@"^[0-9]{6}$")) {
				if(retVal!="")
					retVal+=", ";
				retVal+="CarrierId 6 digits";
			}
			if(treatProv.NationalProvID.Length!=9) {
				if(retVal!="")
					retVal+=", ";
				retVal+="TreatingProv CDA num 9 digits";
			}
			if(treatProv.CanadianOfficeNum.Length!=4) {
				if(retVal!="")
					retVal+=", ";
				retVal+="TreatingProv office num 4 char";
			}
			if(billProv.NationalProvID.Length!=9) {
				if(retVal!="")
					retVal+=", ";
				retVal+="BillingProv CDA num 9 digits";
			}
			if(billProv.CanadianOfficeNum.Length!=4) {
				if(retVal!="")
					retVal+=", ";
				retVal+="BillingProv office num 4 char";
			}
			if(insPlan.GroupNum.Length==0 || insPlan.GroupNum.Length>12 || insPlan.GroupNum.Contains(" ")) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Plan Number";
			}
			if(insSub.SubscriberID=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="SubscriberID";
			}
			if(claim.PatNum != insSub.Subscriber//if patient is not subscriber
				&& claim.PatRelat==Relat.Self//and relat is self
				&& !Patients.ArePatientsClonesOfEachOther(claim.PatNum,insSub.Subscriber))//and the patient is not a clone of the subscriber
			{
				if(retVal!="")
					retVal+=", ";
				retVal+="Claim Relationship";
			}
			if(patient.Gender==PatientGender.Unknown) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Patient gender";
			}
			if(patient.Birthdate.Year<1880 || patient.Birthdate>DateTime.Today) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Patient birthdate";
			}
			if(patient.LName=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Patient lastname";
			}
			if(patient.FName=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Patient firstname";
			}
			if(patient.Age>=18 && patient.CanadianEligibilityCode==1){//fulltimeStudent
				if(patient.SchoolName=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Patient school name";
				}
			}
			if(subscriber.Birthdate.Year<1880 || subscriber.Birthdate>DateTime.Today) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber birthdate";
			}
			if(subscriber.LName=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber lastname";
			}
			if(subscriber.FName=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber firstname";
			}
			if(subscriber.Address=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber address";
			}
			if(subscriber.City=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber city";
			}
			if(!IsValidST(subscriber.State)) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber Province";
			}
			if(!IsValidZip(subscriber.Zip)) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Subscriber Postalcode";
			}
			if(claimProcsClaim.Count>7) {//user interface enforces prevention of claim with 0 procs.
				if(retVal!="")
					retVal+=", ";
				retVal+="Over 7 procs";
			}
//incomplete. Also duplicate for max
			//user interface also needs to be improved to prompt and remind about extracted teeth
			/*if(isInitialLowerProsth && MandProsthMaterial!=0 && CountLower(extracted.Count)==0){
				if(retVal!="")
					retVal+=",";
				retVal+="Missing teeth not entered";
			}*/
			if(carrier.ElectID=="000064") { //Checks for Pacific Blue Cross (PBC) for primary as required for certification.
				List<PatPlan> patPlansForPatient=PatPlans.Refresh(claim.PatNum);
				for(int p=0;p<patPlansForPatient.Count;p++) {
					if(patPlansForPatient[p].InsSubNum==claim.InsSubNum) {
						retVal+=GetMissingDataForPatPlanPacificBlueCross(patPlansForPatient[p],insPlan);
						break;
					}
				}
			}
			if(carrier2!=null && carrier2.ElectID=="000064") { //Checks for Pacific Blue Cross (PBC) for secondary as required for certification.
				List<PatPlan> patPlansForPatient=PatPlans.Refresh(claim.PatNum);
				for(int p=0;p<patPlansForPatient.Count;p++) {
					if(patPlansForPatient[p].InsSubNum==claim.InsSubNum2) {
						retVal+=GetMissingDataForPatPlanPacificBlueCross(patPlansForPatient[p],insPlan2);
						break;
					}
				}
			}
			if(claim.ClaimType!="S" && claim.PlanNum2>0){//Is primary claim and the patient has secondary insurance.
				if(carrier2.IsCDA && !Regex.IsMatch(carrier2.ElectID,@"^[0-9]{6}$")) {//If not CDA, then we send "999999" as per specification.
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec CarrierId 6 digits";
				}
				if(insPlan2.GroupNum.Length==0 || insPlan2.GroupNum.Length>12 || insPlan2.GroupNum.Contains(" ")) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Plan Number";
				}
				if(insSub2.SubscriberID=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec SubscriberID";
				}
				if(claim.PatNum != insSub2.Subscriber//if patient is not subscriber
					&& claim.PatRelat2==Relat.Self) {//and relat is self
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Relationship";
				}
				if(subscriber2.Birthdate.Year<1880 || subscriber2.Birthdate>DateTime.Today) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber birthdate";
				}
				if(subscriber2.LName=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber lastname";
				}
				if(subscriber2.FName=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber firstname";
				}
				if(subscriber2.Address=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber address";
				}
				if(subscriber2.City=="") {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber city";
				}
				if(!IsValidST(subscriber2.State)) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber Province";
				}
				if(!IsValidZip(subscriber2.Zip)) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Sec Subscriber Postalcode";
				}
			}
			if(claim.CanadianReferralProviderNum!="" && claim.CanadianReferralReason==0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Referral reason";
			}
			if(claim.CanadianReferralProviderNum=="" && claim.CanadianReferralReason!=0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Referral provider";
			}
			//Max Prosth--------------------------------------------------------------------------------------------------
			if(claim.CanadianIsInitialUpper=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Max prosth";
			}
			if(claim.CanadianDateInitialUpper>DateTime.MinValue) {
				if(claim.CanadianDateInitialUpper.Year<1900 || claim.CanadianDateInitialUpper>=DateTime.Today) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Date initial upper";
				}
			}
			if(claim.CanadianIsInitialUpper=="N" && claim.CanadianDateInitialUpper.Year<1900) {//missing date
				if(retVal!="")
					retVal+=", ";
				retVal+="Date initial upper";
			}
			if(claim.CanadianIsInitialUpper=="N" && claim.CanadianMaxProsthMaterial==0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Max prosth material";
			}
			if(claim.CanadianIsInitialUpper=="X" && claim.CanadianMaxProsthMaterial!=0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Max prosth material";
			}
			//Mand prosth--------------------------------------------------------------------------------------------------
			if(claim.CanadianIsInitialLower=="") {
				if(retVal!="")
					retVal+=", ";
				retVal+="Mand prosth";
			}
			if(claim.CanadianDateInitialLower>DateTime.MinValue) {
				if(claim.CanadianDateInitialLower.Year<1900 || claim.CanadianDateInitialLower>=DateTime.Today) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Date initial lower";
				}
			}
			if(claim.CanadianIsInitialLower=="N" && claim.CanadianDateInitialLower.Year<1900) {//missing date
				if(retVal!="")
					retVal+=", ";
				retVal+="Date initial lower";
			}
			if(claim.CanadianIsInitialLower=="N" && claim.CanadianMandProsthMaterial==0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Mand prosth material";
			}
			if(claim.CanadianIsInitialLower=="X" && claim.CanadianMandProsthMaterial!=0) {
				if(retVal!="")
					retVal+=", ";
				retVal+="Mand prosth material";
			}
			//missing teeth---------------------------------------------------------------------------------------------------
			/*Can't do this because extracted teeth count is allowed to be zero
			if(claim.CanadianIsInitialLower=="Y" && claim.CanadianMandProsthMaterial!=7) {//initial lower, but not crown
				if(extracted.Count==0) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Missing teeth not entered";
				}
			}
			if(claim.CanadianIsInitialUpper=="Y" && claim.CanadianMaxProsthMaterial!=7) {//initial upper, but not crown
				if(extracted.Count==0) {
					if(retVal!="")
						retVal+=", ";
					retVal+="Missing teeth not entered";
				}
			}
			*/			
			if(claim.AccidentDate>DateTime.MinValue){
				if(claim.AccidentDate.Year<1900 || claim.AccidentDate>DateTime.Today){
					if(retVal!="")
						retVal+=",";
					retVal+="Accident date";
				}
			}
			if(!billProv.IsCDAnet) {
				retVal+="Billing provider is not setup as a CDANet provider.";
			}
			if(!treatProv.IsCDAnet) {
				retVal+="Treating provider is not setup as a CDANet provider.";
			}
			for(int i=0;i<claimProcsClaim.Count;i++) {
				//claimProcsClaim already excludes any claimprocs with ProcNum=0, so no payments etc.
				proc=Procedures.GetProcFromList(procListAll,claimProcsClaim[i].ProcNum);
				procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				//Procedure dates are not included in predetermination requests so we do not need to check the dates for claimtype 'PreAuth'.
				if(claim.ClaimType!="PreAuth") {
					if(claimProcsClaim[i].ProcDate.Year<1970 || claimProcsClaim[i].ProcDate>DateTime.Today) {
						if(retVal!="") {
							retVal+=", ";
						}
						retVal+="proc "+procCode.ProcCode+" procedure date";
					}
				}
				if(procCode.TreatArea==TreatmentArea.Arch && proc.Surf==""){
					if(retVal!="") {
						retVal+=", ";
					}
					retVal+="proc "+procCode.ProcCode+" missing arch";
				}
				if(procCode.TreatArea==TreatmentArea.ToothRange && proc.ToothRange==""){
					if(retVal!="") {
						retVal+=", ";
					}
					retVal+="proc "+procCode.ProcCode+" tooth range";
				}
				if((procCode.TreatArea==TreatmentArea.Tooth || procCode.TreatArea==TreatmentArea.Surf)
					&& !Tooth.IsValidDB(proc.ToothNum)) {
					if(retVal!="") {
						retVal+=", ";
					}
					retVal+="proc "+procCode.ProcCode+" tooth number";
				}
				if(claim.ClaimType!="PreAuth") {
					List<Procedure> labFeesForProc=Procedures.GetCanadianLabFees(proc.ProcNum,procListAll);
					for(int j=0;j<labFeesForProc.Count;j++) {
						if(labFeesForProc[j].ProcStatus!=ProcStat.C) {
							ProcedureCode procCodeLab=ProcedureCodes.GetProcCode(labFeesForProc[j].CodeNum);
							if(retVal!="") {
								retVal+=", ";
							}
							retVal+="proc "+procCode.ProcCode+" lab fee "+procCodeLab.ProcCode+" not complete";
						}
					}
				}
			}
			for(int i=0;i<extracted.Count;i++) {
				if(extracted[i].ProcDate.Date>DateTime.Today) {
					retVal+="extraction date in future";
				}
			}
			return retVal;
		}

		///<summary>Only call this function for a patPlan such that the carrier has an electid of 000064, which signified that it is for Pacific Blue Cross (PBC).</summary>
		private static string GetMissingDataForPatPlanPacificBlueCross(PatPlan patPlan,InsPlan insPlan) {
			string retVal="";
			//On 10/27/2015 we removed the Dependent Code requirements, based on customer feedback.  Some customers called PBC and were told that
			//the dependent codes for children are no longer a requirement, and we take this as meaning that Dependent Codes in general are no longer
			//a requirement.
			//string dependantCode=patPlan.PatID;//C17
			//int dependantNum=-1;
			//if(dependantCode!="") {
			//	try {
			//		dependantNum=PIn.Int(dependantCode);
			//	}
			//	catch {
			//	}
			//}
			//string relationshipCode=GetRelationshipCode(patPlan.Relationship);//C03
			//if(relationshipCode=="1") {//self
			//	if(dependantCode!="00") {
			//		if(retVal!="")
			//			retVal+=", ";
			//		retVal+="Dependant code must be 00 for Self with Pacific Blue Cross";
			//	}
			//}
			//else if(relationshipCode=="2") {//spouse
			//	if(dependantCode!="01" && !(dependantNum>=90 && dependantNum<=99)) {
			//		if(retVal!="")
			//			retVal+=", ";
			//		retVal+="Dependant code must be 01, or between 90 and 99 for Spouse with Pacific Blue Cross";
			//	}
			//}
			////else if(relationshipCode=="3") {//child - some of our customers have called PBC and were told that this is no longer a requirement.
			////	if(dependantNum<2 || dependantNum>89 || dependantCode.Length!=2) {
			////		retVal+="Dependant code must be between 02 and 89 for Child with Pacific Blue Cross";
			////	}
			////}
			//else if(relationshipCode=="4") {//common law spouse
			//	if(dependantCode!="01" && !(dependantNum>=90 && dependantNum<=99)) {
			//		if(retVal!="")
			//			retVal+=", ";
			//		retVal+="Dependant code must be 01, or between 90 and 99 for Common-law with Pacific Blue Cross";
			//	}
			//}
			//else {
			//	retVal+="Relationship code must be Self, Spouse, Child, LifePartner, or SignifOther with Pacific Blue Cross";
			//}
			return retVal;
		}

		///<summary>Returns true if in Cananda and given plan is either percentage or a PPO and PrefName.CanadaCreatePpoLabEst is enabled.</summary>
		public static bool IsValidForLabEstimates(InsPlan plan) {
			if(!CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return false;
			}
			return (plan.PlanType=="" || (plan.PlanType=="p" && PrefC.GetBool(PrefName.CanadaCreatePpoLabEst)));
		}

		///<summary>A procedure issued by the carrier that references procedures via their line number from the original submission.</summary>
		private class CarrierIssuedProcedure {
			///<summary>The CCDFields that were used to compose this object. Helpful for visual validation when debugging.</summary>
			public List<CCDField> ListFields;
			///<summary>An 'As Total' payment created via logic that mimics the FormClaimEdit.cs By Total button. This claimproc will be used to indicate to the user that a carrier issued procedure was sent in the response. The Remarks field will contain helpful information to display to the user.</summary>
			public ClaimProc ClaimProcByTotal;
			///<summary>The G18 field which is a reference to the line number(s) of the submitted procedure(s). There cannot be a digit that is greater than 7. E.g. a value of 12 means that this carrier proc is related to lines 1 and 2.</summary>
			public byte[] ArrayLineNumbers;
			///<summary>G21 field: Deductible converted to a double via RawMoneyStrToDisplayMoney().</summary>
			public double Deductible;
			///<summary>G23 field: Benefit Amount converted to a double via RawMoneyStrToDisplayMoney().</summary>
			public double BenefitAmount;

			public CarrierIssuedProcedure(List<CCDField> listFields,Claim claimCur,long payPlanNum,bool isPreEob,EraBehaviors eraBehavior,
				Dictionary<int,string> listNoteFields)
			{
				ListFields=listFields;
				ClaimProcByTotal=ByTotClaimProcHelper(claimCur,payPlanNum,isPreEob,eraBehavior);
				#region Process CCDFields
				List<string> listClaimProcNotes=new List<string>();
				foreach(CCDField field in listFields) {
					switch(field.fieldId) {
						case "G18"://Reference to Line Number of the Submitted Procedure
							//The value of this field will typically be '0000002' but it has been sent to us like '      2' by some carriers.
							//The only helpful information from this field are the references to actual line numbers, 1-7.
							ArrayLineNumbers=Regex.Replace(field.valuestr,"[^1-7]","").Select(x => PIn.Byte(x.ToString())).ToArray();
							break;
						case "G19"://Additional Procedure Code
							listClaimProcNotes.Add(Lans.g("Canadian","Additional Procedure Code")+": "+field.valuestr);
							break;
						case "G20"://Eligible Amount
							break;
						case "G44"://Eligible Amount for additional Lab procedure
							double eligibleAmount=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							listClaimProcNotes.Add(Lans.g("Canadian","Lab Procedure Eligible Amount")+": "+eligibleAmount.ToString("N"));
							break;
						case "G21"://Deductible
							Deductible=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							listClaimProcNotes.Add(Lans.g("Canadian","Deductible")+": "+Deductible.ToString("N"));
							if(isPreEob) {
								ClaimProcByTotal.DedEst=Deductible;
							}
							else {
								ClaimProcByTotal.DedApplied=Deductible;
							}
							break;
						case "G22"://Eligible percentage
							ClaimProcByTotal.Percentage=PIn.Int(RawPercentToDisplayPercent(field.valuestr));
							break;
						case "G23"://Benefit Amount
							BenefitAmount=PIn.Double(RawMoneyStrToDisplayMoney(field.valuestr));
							listClaimProcNotes.Add(Lans.g("Canadian","Benefit Amount")+": "+BenefitAmount.ToString("N"));
							if(isPreEob) {
								ClaimProcByTotal.InsPayEst=BenefitAmount;
							}
							else {
								ClaimProcByTotal.InsEstTotalOverride=BenefitAmount;//Set this so if it's not marked received, pat port is still reflected correctly
								if(eraBehavior==EraBehaviors.DownloadAndReceive) {
									ClaimProcByTotal.InsPayAmt=BenefitAmount;
								}
							}
							break;
						case "G24"://Explanation Note Number 1
						case "G25"://Explanation Note Number 2
							int noteNumber=PIn.Int(field.valuestr);
							if(listNoteFields.ContainsKey(noteNumber)) {
								listClaimProcNotes.Add(listNoteFields[noteNumber]);//Guarenteed 1 item in list
							}
							break;
					}
				}
				ClaimProcByTotal.Remarks+=string.Join("\r\n",listClaimProcNotes);
				#endregion
			}
		}

	}
}
