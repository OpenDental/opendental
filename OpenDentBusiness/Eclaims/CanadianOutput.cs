using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using Health.Direct.Common.Metadata;
using OpenDentBusiness.FileIO;
using static OpenDentBusiness.Eclaims.Canadian;
using Word;
using OpenDentalCloud.Core;
using System.Transactions;
using System.Windows.Media.Imaging;

namespace OpenDentBusiness.Eclaims {
	public class CanadianOutput {

#if DEBUG
		public static int testNumber=-1;
#endif

		public delegate void PrintCdaClaimForm(Claim claim);
		public delegate void PrintCCD(Etrans pEtrans,string messageText,bool pAutoPrint);

		public static List<long> SendAttachments(Clearinghouse clearinghouseClin,long patNum,InsPlan plan,InsSub insSub,Claim claim) {
			Canadian.ValidateExportPath(clearinghouseClin);
			List<FileInfo> listFileInfoAttachments=GetInfoForAttachments(claim.Attachments);//Loaded here as part of validation.  Throws exceptions.
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			if(carrier==null) {
				throw new ODException("Invalid carrier.");
			}
			if((carrier.CanadianSupportedTypes&CanSupTransTypes.Attachment_09)!=CanSupTransTypes.Attachment_09) {
				throw new ODException("The carrier does not support attachment transactions.");
			}
			if(carrier.CanadianNetworkNum==0) {
				throw new ODException("Carrier network not set.");
			}
			if(carrier.CDAnetVersion=="02") {
				throw new ODException("CDAnet version 02 does not support attachment transactions.");
			}
			int claimOfficeSequenceNumber=0;
			try {
				claimOfficeSequenceNumber=GetOriginalOfficeSequenceNumber(claim,out _);
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			if(claimOfficeSequenceNumber==0) {
				throw new ODException("Cannot send attachments electronically until after the claim is sent electronically.");
			}
			if(claim.Attachments.Count>30) {
				throw new ODException("Cannot send more than 30 electronic attachments.");
			}
			if(claim.Attachments.Count==0) {
				throw new ODException("No electronic attachments on this claim to send.");
			}
			CanadianNetwork network=CanadianNetworks.GetNetwork(carrier.CanadianNetworkNum,clearinghouseClin,claim);
			Patient patient=Patients.GetPat(patNum);
			Provider provDefaultTreat=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			if(clearinghouseClin==null) {
				throw new ODException("Canadian clearinghouse not found.");
			}
			string error="";
			if(!provDefaultTreat.IsCDAnet) {
				error+="Prov not setup as CDA provider";
			}
			if(provDefaultTreat.NationalProvID.Length!=9) {
				if(error!="") error+=", ";
				error+="Prov CDA num 9 digits";
			}
			if(provDefaultTreat.CanadianOfficeNum.Length!=4) {
				if(error!="") error+=", ";
				error+="Prov office num 4 char";
			}
			if(error!="") {
				throw new ODException(error);
			}
			Etrans etrans=Etranss.CreateCanadianOutput(patNum,carrier.CarrierNum,0,
				clearinghouseClin.HqClearinghouseNum,EtransType.Attachment_CA,plan.PlanNum,insSub.InsSubNum,Security.CurUser.UserNum);
			int index=0;
			List<long> listEtransNums=new List<long>();
			while(listFileInfoAttachments.Count!=0) {
				#region Construct outoing message
				StringBuilder strb=new StringBuilder();
				//A01 transaction prefix 12 AN
				strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
				//A02 office sequence number 6 N
				strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
				//A03 format version number 2 N. Must be the value of A04
				strb.Append(carrier.CDAnetVersion);
				//A04 transaction code 2 N. Must be the value of A09
				strb.Append("09");
				//A05 carrier id number 6 N
				strb.Append(carrier.ElectID);//already validated as 6 digit number.  Must always be the primary carrier, even for secondary (COB) eclaims.
				//A06 software system id 3 AN
				strb.Append(Canadian.SoftwareSystemId());
				//A10 encryption method 1 N
				strb.Append(carrier.CanadianEncryptionMethod);//validated in UI
				//A07 message length. 7 N. Max Message Length 9999999
				strb.Append("0000000");
				//A09 carrier transaction counter 5 N, only version 04
				strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
				//B01 CDA provider number 9 AN
				strb.Append(Canadian.TidyAN(provDefaultTreat.NationalProvID,9));//already validated
				//B02 (treating) provider office number 4 AN
				strb.Append(Canadian.TidyAN(provDefaultTreat.CanadianOfficeNum,4));//already validated
				//Note: The preamble to the Attachment transaction indicates it can be used to send attachments to other providers as well as claims processors. To date the ability
				//to send to other providers has not been implemented, and there are no known plans to introduce this functionality—fields B07 and B08 will always be empty.
				//B07 Receiving Provider Number 9 AN
				strb.Append(Canadian.TidyAN("         ",9));
				//B08 Receiving Office Number 4 AN
				strb.Append(Canadian.TidyAN("    ",4));
				//"For Future Use" 1 A
				strb.Append(Canadian.TidyA(" ",1));
				//C05 patient birthday 8 N
				strb.Append(patient.Birthdate.ToString("yyyyMMdd"));//validated
				//C06 patient last name 25 AE
				strb.Append(Canadian.TidyAE(patient.LName,25,true));//validated
				//C07 patient first name 15 AE
				strb.Append(Canadian.TidyAE(patient.FName,15,true));//validated
				//C08 patient middle initial 1 AE
				strb.Append(Canadian.TidyAE(patient.MiddleI,1));
				//This value will be taken from etrans.OfficeSequenceNumber of the latest etrans record for a sent claim where etrans.ClaimNum matches the claim.
				//F41 Original Office Seq. Number 6 N
				strb.Append(Canadian.TidyN(claimOfficeSequenceNumber,6));
				//F42 Original Transaction Reference Number 14 AN
				strb.Append(Canadian.TidyAN(claim.CanadaTransRefNum,14));
				//F43 Attachment Source 1 A
				strb.Append(Canadian.TidyA("I",1));
				//F44 Attachment Count 2 N. Max is 30 attachments under 9999999 bytes
				strb.Append(Canadian.TidyN(claim.Attachments.Count,2));
				//Need to loop through these codes for the number of times equal to F44
				long totalBytes=0;
				for(int i=0;i<listFileInfoAttachments.Count;i++) {
					long lengthBase64=((listFileInfoAttachments[i].Length*4)/3)+1;//Calculate total size when raw bytes are converted to base64 for outgoing format.
					if(totalBytes+lengthBase64>9999999) {
						continue; //Go to next attachmentBase64 to see if it will fit
					}
					index++;
					//F49 Attachment Number 2 N
					strb.Append(Canadian.TidyN(index,2));
					//F45 Attachment Type 3 AN. We can infer this from file extension instead of creating a UI
					string attachmentType=listFileInfoAttachments[i].Extension.ToUpper().TrimStart('.');
					//From the 'CDAnet Note 2020-002 Attachment.pdf':
					//"Note to software vendors: MS Word files now have the 4-character extension .docx. When including these files in the attachmentBase64 transaction,
					//use the file type “DOC”."
					if(attachmentType=="DOCX") {
						attachmentType="DOC";
					}
					strb.Append(Canadian.TidyAN(attachmentType,3));
					//F46 Attachment Length 7 N
					byte[] attachmentBytes=FileAtoZ.ReadAllBytes(listFileInfoAttachments[i].FullName);
					string attachmentBase64=Convert.ToBase64String(attachmentBytes);
					totalBytes+=attachmentBase64.Length;
					strb.Append(Canadian.TidyN(attachmentBase64.Length,7));
					//F48 Attachment File Date 8 N
					strb.Append(Canadian.TidyN(listFileInfoAttachments[i].CreationTime.ToString("yyyyMMdd"),8));
					//F47 Attachment X AN. X will be the number of bytes.
					strb.Append(attachmentBase64);
					listFileInfoAttachments.RemoveAt(i);
					i--;
				}
				//Now we go back and fill in the actual message length now that we know the number for sure.
				strb.Replace("0000000",Canadian.TidyN(strb.Length,7),32,7);
				#endregion Construct Outgoing Message
				string errorMsg;
				string result=PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic:false,out errorMsg);
				//Attach an ack to the etrans
				Etrans etransAck=new Etrans();
				etransAck.PatNum=etrans.PatNum;
				etransAck.PlanNum=etrans.PlanNum;
				etransAck.InsSubNum=etrans.InsSubNum;
				etransAck.CarrierNum=etrans.CarrierNum;
				etransAck.ClaimNum=etrans.ClaimNum;
				etransAck.DateTimeTrans=DateTime.Now;
				etransAck.UserNum=Security.CurUser.UserNum;
				if(errorMsg!=""){
					etransAck.Etype=EtransType.AckError;
					etransAck.Note=errorMsg;
					etrans.Note="failed";
				}
				else{
					CCDFieldInputter fieldInputter=new CCDFieldInputter(result);
					CCDField fieldG05=fieldInputter.GetFieldById("G05");
					if(fieldG05!=null) {
						etransAck.AckCode=fieldG05.valuestr;
					}
					CCDField fieldG06=fieldInputter.GetFieldById("G06");
					if(fieldG06?.valuestr!="00") {
						errorMsg=string.Join("\r\n",fieldInputter.GetFieldsById("G08")
							.Select(x => CCDerror.message(PIn.Int(x.valuestr),IsDentalOfficeFrench())));
					}
					etransAck.Etype=fieldInputter.GetEtransType();
				}
				Etranss.Insert(etransAck);
				Etranss.SetMessage(etransAck.EtransNum,result);//Save incomming history.
				etrans.AckEtransNum=etransAck.EtransNum;
				Etranss.Update(etrans);
				Etranss.SetMessage(etrans.EtransNum,strb.ToString());//Save outgoing history.
				listEtransNums.Add(etransAck.EtransNum);
				if(errorMsg!="") {
					throw new ApplicationException(errorMsg);
				}
			}
			return listEtransNums;
		}

		///<summary>The result is the etransNum of the response message.  Or it might throw an exception if invalid data.  
		///This class is also responsible for saving the returned message to the etrans table, and for printing out the required form.</summary>
		public static long SendElegibility(Clearinghouse clearinghouseClin,long patNum,InsPlan plan,DateTime date,Relat relat,string patID,bool doPrint,
			InsSub insSub,bool isAutomatic,PrintCCD printCCD) {
			//string electID,long patNum,string groupNumber,string divisionNo,
			//string subscriberID,string patID,Relat patRelat,long subscNum,string dentaideCardSequence)
			//Note: This might be the only class of this kind that returns a string.  It's a special situation.
			//We are simply not going to bother with language translation here.
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			if(carrier==null) {
				throw new ApplicationException("Invalid carrier.");
			}
			if((carrier.CanadianSupportedTypes&CanSupTransTypes.EligibilityTransaction_08)!=CanSupTransTypes.EligibilityTransaction_08) {
				throw new ApplicationException("The carrier does not support eligibility transactions.");
			}
			if(carrier.CanadianNetworkNum==0) {
				throw new ApplicationException("Carrier network not set.");
			}
			CanadianNetwork network=CanadianNetworks.GetNetwork(carrier.CanadianNetworkNum,clearinghouseClin);
			Patient patient=Patients.GetPat(patNum);
			Patient subscriber=Patients.GetPat(insSub.Subscriber);
			Provider provDefaultTreat=Providers.GetProv(PrefC.GetLong(PrefName.PracticeDefaultProv));
			if(clearinghouseClin==null) {
				throw new ApplicationException("Canadian clearinghouse not found.");
			}
			Canadian.ValidateExportPath(clearinghouseClin);
			//validate----------------------------------------------------------------------------------------------------
			string error="";
			//if(carrier.CanadianNetworkNum==0){
			//	if(error!="") error+=", ";
			//	error+="Carrier does not have network specified";
			//}
			if(!Regex.IsMatch(carrier.ElectID,@"^[0-9]{6}$")) {//not necessary, but nice
				if(error!="") error+=", ";
				error+="CarrierId 6 digits";
			}
			if(!provDefaultTreat.IsCDAnet) {
				error+="Prov not setup as CDA provider";
			}
			if(provDefaultTreat.NationalProvID.Length!=9) {
				if(error!="") error+=", ";
				error+="Prov CDA num 9 digits";
			}
			if(provDefaultTreat.CanadianOfficeNum.Length!=4) {
				if(error!="") error+=", ";
				error+="Prov office num 4 char";
			}
			//if(plan.GroupNum.Length==0 || groupNumber.Length>12 || groupNumber.Contains(" ")){
			//	if(error!="") error+=", ";
			//	error+="Plan Number";
			//}
			//if(subscriberID==""){//already validated.  And it's allowed to be blank sometimes
			//	if(error!="") error+=", ";
			//	error+="SubscriberID";
			//}
			if(patNum != insSub.Subscriber && relat==Relat.Self) {//if patient is not subscriber, and relat is self
				if(error!="") error+=", ";
				error+="Relationship cannot be self";
			}
			if(patient.Gender==PatientGender.Unknown) {
				if(error!="") error+=", ";
				error+="Patient gender";
			}
			if(patient.Birthdate.Year<1880 || patient.Birthdate>DateTime.Today) {
				if(error!="") error+=", ";
				error+="Patient birthdate";
			}
			if(patient.LName=="") {
				if(error!="") error+=", ";
				error+="Patient lastname";
			}
			if(patient.FName=="") {
				if(error!="") error+=", ";
				error+="Patient firstname";
			}
			if(patient.CanadianEligibilityCode==0) {
				if(error!="") error+=", ";
				error+="Patient eligibility exception code";
			}
			if(subscriber.Birthdate.Year<1880 || subscriber.Birthdate>DateTime.Today) {
				if(error!="") error+=", ";
				error+="Subscriber birthdate";
			}
			if(subscriber.LName=="") {
				if(error!="") error+=", ";
				error+="Subscriber lastname";
			}
			if(subscriber.FName=="") {
				if(error!="") error+=", ";
				error+="Subscriber firstname";
			}
			if(error!="") {
				throw new ApplicationException(error);
			}
			Etrans etrans=Etranss.CreateCanadianOutput(patNum,carrier.CarrierNum,0,
				clearinghouseClin.HqClearinghouseNum,EtransType.Eligibility_CA,plan.PlanNum,insSub.InsSubNum,Security.CurUser.UserNum);
			StringBuilder strb=new StringBuilder();
			//create message----------------------------------------------------------------------------------------------
			//A01 transaction prefix 12 AN
			strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
			//A02 office sequence number 6 N
			strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
			//A03 format version number 2 N
			strb.Append(carrier.CDAnetVersion);//eg. "04", validated in UI
																				 //A04 transaction code 2 N
			if(carrier.CDAnetVersion=="02") {
				strb.Append("00");//eligibility
			}
			else {
				strb.Append("08");//eligibility
			}
			//A05 carrier id number 6 N
			strb.Append(carrier.ElectID);//already validated as 6 digit number.
																	 //A06 software system id 3 AN
			strb.Append(Canadian.SoftwareSystemId());
			if(carrier.CDAnetVersion=="04") {
				//A10 encryption method 1 N
				strb.Append(carrier.CanadianEncryptionMethod);//validated in UI
			}
			//A07 message length 5 N
			int len;
			bool C19PlanRecordPresent=false;
			if(carrier.CDAnetVersion=="02") {
				len=178;
				strb.Append(Canadian.TidyN(len,4));
			}
			else {
				len=214;
				if(plan.CanadianPlanFlag=="A") {// || plan.CanadianPlanFlag=="N"){
					C19PlanRecordPresent=true;
				}
				if(C19PlanRecordPresent) {
					len+=30;
				}
				strb.Append(Canadian.TidyN(len,5));
				//A09 carrier transaction counter 5 N, only version 04
				strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
			}
			//B01 CDA provider number 9 AN
			strb.Append(Canadian.TidyAN(provDefaultTreat.NationalProvID,9));//already validated
																																			//B02 provider office number 4 AN
			strb.Append(Canadian.TidyAN(provDefaultTreat.CanadianOfficeNum,4));//already validated
			if(carrier.CDAnetVersion=="04") {
				//B03 billing provider number 9 AN
				Provider provBilling=Providers.GetProv(Providers.GetBillingProvNum(provDefaultTreat.ProvNum,patient.ClinicNum));
				strb.Append(Canadian.TidyAN(provBilling.NationalProvID,9));//already validated
			}
			if(carrier.CDAnetVersion=="02") {
				//C01 primary policy/plan number 8 AN (group number)
				//No special validation for version 02
				strb.Append(Canadian.TidyAN(plan.GroupNum,8));
			}
			else {
				//C01 primary policy/plan number 12 AN (group number)
				//only validated to ensure that it's not blank and is less than 12. Also that no spaces.
				strb.Append(Canadian.TidyAN(plan.GroupNum,12));
			}
			//C11 primary division/section number 10 AN
			strb.Append(Canadian.TidyAN(plan.DivisionNo,10));
			if(carrier.CDAnetVersion=="02") {
				//C02 subscriber id number 11 AN
				strb.Append(Canadian.TidyAN(insSub.SubscriberID.Replace("-",""),11));//no extra validation for version 02
			}
			else {
				//C02 subscriber id number 12 AN
				strb.Append(Canadian.TidyAN(insSub.SubscriberID.Replace("-",""),12));//validated
			}
			if(carrier.CDAnetVersion=="04") {
				//C17 primary dependant code 2 N. Optional
				strb.Append(Canadian.TidyN(patID,2));
			}
			//C03 relationship code 1 N
			//User interface does not only show Canadian options, but all options are handled.
			strb.Append(Canadian.GetRelationshipCode(relat));
			//C04 patient's sex 1 A
			//validated to not include "unknown"
			if(patient.Gender==PatientGender.Male) {
				strb.Append("M");
			}
			else if(patient.Gender==PatientGender.Female) {
				strb.Append("F");
			}
			else {
				strb.Append(" ");
			}
			//C05 patient birthday 8 N
			strb.Append(patient.Birthdate.ToString("yyyyMMdd"));//validated
																													//C06 patient last name 25 AE
			strb.Append(Canadian.TidyAE(patient.LName,25,true));//validated
																													//C07 patient first name 15 AE
			strb.Append(Canadian.TidyAE(patient.FName,15,true));//validated
																													//C08 patient middle initial 1 AE
			strb.Append(Canadian.TidyAE(patient.MiddleI,1));
			//C09 eligibility exception code 1 N
			strb.Append(Canadian.GetEligibilityCode(patient.CanadianEligibilityCode,carrier.CDAnetVersion=="02"));//validated
			if(carrier.CDAnetVersion=="04") {
				//C12 plan flag 1 A
				strb.Append(Canadian.GetPlanFlag(plan.CanadianPlanFlag));
				//C18 plan record count 1 N
				if(C19PlanRecordPresent) {
					strb.Append("1");
				}
				else {
					strb.Append("0");
				}
				//C16 Eligibility date. 8 N.
				strb.Append(date.ToString("yyyyMMdd"));
			}
			//D01 subscriber birthday 8 N
			strb.Append(subscriber.Birthdate.ToString("yyyyMMdd"));//validated
																														 //D02 subscriber last name 25 AE
			strb.Append(Canadian.TidyAE(subscriber.LName,25,true));//validated
																														 //D03 subscriber first name 15 AE
			strb.Append(Canadian.TidyAE(subscriber.FName,15,true));//validated
																														 //D04 subscriber middle initial 1 AE
			strb.Append(Canadian.TidyAE(subscriber.MiddleI,1));
			if(carrier.CDAnetVersion=="04") {
				//D10 language of insured 1 A
				if(subscriber.Language=="fr") {
					strb.Append("F");
				}
				else {
					strb.Append("E");
				}
				//D11 card sequence/version number 2 N
				//Not validated against type of carrier.  Might need to check if Dentaide.
				strb.Append(Canadian.TidyN(plan.DentaideCardSequence,2));
				//C19 plan record 30 AN
				if(C19PlanRecordPresent) {
					//todo: what text goes here?  Not documented
					strb.Append(Canadian.TidyAN("",30));
				}
			}
			string errorMsg="";
			string result=Canadian.PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
			//Attach an ack to the etrans
			Etrans etransAck=new Etrans();
			etransAck.PatNum=etrans.PatNum;
			etransAck.PlanNum=etrans.PlanNum;
			etransAck.InsSubNum=etrans.InsSubNum;
			etransAck.CarrierNum=etrans.CarrierNum;
			etransAck.DateTimeTrans=DateTime.Now;
			etransAck.UserNum=Security.CurUser.UserNum;
			CCDFieldInputter fieldInputter=null;
			if(errorMsg!="") {
				etransAck.Etype=EtransType.AckError;
				etransAck.Note=errorMsg;
				etrans.Note="failed";
			}
			else {
				fieldInputter=new CCDFieldInputter(result);
				CCDField fieldG05=fieldInputter.GetFieldById("G05");
				if(fieldG05!=null) {
					etransAck.AckCode=fieldG05.valuestr;
				}
				etransAck.Etype=fieldInputter.GetEtransType();
			}
			Etranss.Insert(etransAck);
			Etranss.SetMessage(etransAck.EtransNum,result);
			etrans.AckEtransNum=etransAck.EtransNum;
			Etranss.Update(etrans);
			Etranss.SetMessage(etrans.EtransNum,strb.ToString());
			if(errorMsg!="") {
				throw new ApplicationException(errorMsg);
			}
			if(doPrint && printCCD != null) {
				printCCD(etrans,result,true);//Physically print the form.
			}
			//Now we will process the 'result' here to extract the important data.  Basically Yes or No on the eligibility.
			//We might not do this for any other trans type besides eligibility.
			string strResponse="";//"Eligibility check on "+DateTime.Today.ToShortDateString()+"\r\n";
														//CCDField field=fieldInputter.GetFieldById("G05");//response status
			string valuestr=fieldInputter.GetValue("G05");//response status
			switch(valuestr) {
				case "E":
					strResponse+="Patient is eligible.";
					break;
				case "R":
					strResponse+="Patient not eligible, or error in data.";
					break;
				case "M":
					strResponse+="Manual claimform should be submitted for employer certified plan.";
					break;
			}
			etrans=Etranss.GetEtrans(etrans.EtransNum);
			etrans.Note=strResponse;
			Etranss.Update(etrans);
			return etransAck.EtransNum;
			/*
			CCDField[] fields=fieldInputter.GetFieldsById("G08");//Error Codes
			for(int i=0;i<fields.Length;i++){
				retVal+="\r\n";
				retVal+=fields[i].valuestr;//todo: need to turn this into a readable string.
			}
			fields=fieldInputter.GetFieldsById("G32");//Display messages
			for(int i=0;i<fields.Length;i++) {
				retVal+="\r\n";
				retVal+=fields[i].valuestr;
			}
			return retVal;*/
		}

		///<summary>Throws exceptions. Returns the OfficeSequenceNumber of the given claim if found, 0 if not.</summary>
		private static int GetOriginalOfficeSequenceNumber(Claim claim,out DateTime originalEtransDateTime) {
			int officeSequenceNumber=0;//Clear the randomly generated office sequence number.
			originalEtransDateTime=DateTime.MinValue;//So we can get the latest matching transaction.
			List<Etrans> listEtrans=Etranss.GetAllForOneClaim(claim.ClaimNum).FindAll(x => x.Etype.In(EtransType.Claim_CA,EtransType.ClaimCOB_CA,EtransType.Predeterm_CA));
			for(int i=0;i<listEtrans.Count;i++) {
				Etrans ack=Etranss.GetEtrans(listEtrans[i].AckEtransNum);
				if(ack==null) {//For those claims sent that didn't receive a response (i.e. when there is an exception while sending a claim).
					continue;
				}
				string messageText=EtransMessageTexts.GetMessageText(ack.EtransMessageTextNum);
				CCDFieldInputter messageData=new CCDFieldInputter(messageText);
				CCDField transRefNum=messageData.GetFieldById("G01");
				if(transRefNum!=null && transRefNum.valuestr==claim.CanadaTransRefNum && listEtrans[i].DateTimeTrans>originalEtransDateTime) {
					officeSequenceNumber=PIn.Int(messageData.GetFieldById("A02").valuestr);
					originalEtransDateTime=listEtrans[i].DateTimeTrans;
				}
			}
			return officeSequenceNumber;
		}

		///<summary></summary>
		public static long SendClaimReversal(Clearinghouse clearinghouseClin,Claim claim,InsPlan plan,InsSub insSub,bool isAutomatic,PrintCCD printCCD) {
			StringBuilder strb=new StringBuilder();
			Carrier carrier=Carriers.GetCarrier(plan.CarrierNum);
			if(clearinghouseClin==null) {
				throw new ApplicationException(Lans.g("CanadianOutput","Canadian clearinghouse not found."));
			}
			Canadian.ValidateExportPath(clearinghouseClin);
			if((carrier.CanadianSupportedTypes&CanSupTransTypes.ClaimReversal_02)!=CanSupTransTypes.ClaimReversal_02) {
				throw new ApplicationException(Lans.g("CanadianOutput","The carrier does not support reversal transactions."));
			}
			if(carrier.CanadianNetworkNum==0) {
				throw new ApplicationException("Carrier network not set.");
			}
			CanadianNetwork network=CanadianNetworks.GetNetwork(carrier.CanadianNetworkNum,clearinghouseClin,claim);
			Etrans etrans=Etranss.CreateCanadianOutput(claim.PatNum,carrier.CarrierNum,carrier.CanadianNetworkNum,
				clearinghouseClin.HqClearinghouseNum,EtransType.ClaimReversal_CA,plan.PlanNum,insSub.InsSubNum,Security.CurUser.UserNum);
			etrans.ClaimNum=claim.ClaimNum;//We don't normally use a claim number with Etranss.CreateCanadianOutput(), but here we need the claim number so that we can show the claim reversal in the claim history.
			Etranss.Update(etrans);
			Patient patient=Patients.GetPat(claim.PatNum);
			Provider prov=Providers.GetProv(claim.ProvTreat);
			if(!prov.IsCDAnet) {
				throw new ApplicationException(Lans.g("CanadianOutput","Treating provider is not setup to use CDANet."));
			}
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider billProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvBill)??providerFirst;
			if(!billProv.IsCDAnet) {
				throw new ApplicationException(Lans.g("CanadianOutput","Billing provider is not setup to use CDANet."));
			}
			InsPlan insPlan=InsPlans.GetPlan(claim.PlanNum,new List<InsPlan>());
			Patient subscriber=Patients.GetPat(insSub.Subscriber);
			//create message----------------------------------------------------------------------------------------------
			//A01 transaction prefix 12 AN
			strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
			//A02 office sequence number 6 N
			//We are required to use the same office sequence number as the original claim.
			etrans.OfficeSequenceNumber=GetOriginalOfficeSequenceNumber(claim,out DateTime originalEtransDateTime);
			DateTime serverDate=MiscData.GetNowDateTime().Date;
			if(originalEtransDateTime.Date!=serverDate) {
				throw new ApplicationException(Lans.g("CanadianOutput","Claims can only be reversed on the day that they were sent. The claim can only be manually reversed."));
			}
			strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
			//A03 format version number 2 N
			strb.Append(carrier.CDAnetVersion);//eg. "04", validated in UI
																				 //A04 transaction code 2 N
			strb.Append("02");//Same for both versions 02 and 04.
												//A05 carrier id number 6 N
			strb.Append(carrier.ElectID);//already validated as 6 digit number.
																	 //A06 software system id 3 AN
			strb.Append(Canadian.SoftwareSystemId());
			if(carrier.CDAnetVersion!="02") { //version 04
																				//A10 encryption method 1 N
				strb.Append(carrier.CanadianEncryptionMethod);//validated in UI
			}
			if(carrier.CDAnetVersion=="02") {
				//A07 message length N4
				strb.Append(Canadian.TidyN("133",4));
			}
			else { //version 04
						 //A07 message length N 5
				strb.Append(Canadian.TidyN("164",5));
			}
			if(carrier.CDAnetVersion!="02") { //version 04
																				//A09 carrier transaction counter 5 N
				if(ODBuild.IsDebug()) {
					strb.Append("11111");
				}
				else {
					strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
				}
			}
			//B01 CDA provider number 9 AN
			strb.Append(Canadian.TidyAN(prov.NationalProvID,9));//already validated
																													//B02 provider office number 4 AN
			strb.Append(Canadian.TidyAN(prov.CanadianOfficeNum,4));//already validated
			if(carrier.CDAnetVersion!="02") { //version 04
																				//B03 billing provider number 9 AN
																				//might need to account for possible 5 digit prov id assigned by carrier
				strb.Append(Canadian.TidyAN(billProv.NationalProvID,9));//already validated
																																//B04 billing provider office number 4 AN
				strb.Append(Canadian.TidyAN(billProv.CanadianOfficeNum,4));//already validated
			}
			if(carrier.CDAnetVersion=="02") {
				//C01 primary policy/plan number 8 AN
				//only validated to ensure that it's not blank and is less than 8. Also that no spaces.
				strb.Append(Canadian.TidyAN(insPlan.GroupNum,8));
			}
			else { //version 04
						 //C01 primary policy/plan number 12 AN
						 //only validated to ensure that it's not blank and is less than 12. Also that no spaces.
				strb.Append(Canadian.TidyAN(insPlan.GroupNum,12));
			}
			//C11 primary division/section number 10 AN
			strb.Append(Canadian.TidyAN(insPlan.DivisionNo,10));
			if(carrier.CDAnetVersion=="02") {
				//C02 subscriber id number 11 AN
				strb.Append(Canadian.TidyAN(insSub.SubscriberID.Replace("-",""),11));//validated
			}
			else { //version 04
						 //C02 subscriber id number 12 AN
				strb.Append(Canadian.TidyAN(insSub.SubscriberID.Replace("-",""),12));//validated
			}
			//C03 relationship code 1 N
			//User interface does not only show Canadian options, but all options are handled.
			strb.Append(Canadian.GetRelationshipCode(claim.PatRelat));
			if(carrier.CDAnetVersion=="02") {
				//D02 subscriber last name 25 A
				strb.Append(Canadian.TidyA(subscriber.LName,25));//validated
			}
			else { //version 04
						 //D02 subscriber last name 25 AE
				strb.Append(Canadian.TidyAE(subscriber.LName,25,true));//validated
			}
			if(carrier.CDAnetVersion=="02") {
				//D03 subscriber first name 15 A
				strb.Append(Canadian.TidyA(subscriber.FName,15));//validated
			}
			else { //version 04
						 //D03 subscriber first name 15 AE
				strb.Append(Canadian.TidyAE(subscriber.FName,15,true));//validated
			}
			if(carrier.CDAnetVersion=="02") {
				//D04 subscriber middle initial 1 A
				strb.Append(Canadian.TidyA(subscriber.MiddleI,1));
			}
			else { //version 04
						 //D04 subscriber middle initial 1 AE
				strb.Append(Canadian.TidyAE(subscriber.MiddleI,1));
			}
			if(carrier.CDAnetVersion!="02") { //version 04
																				//For Future Use
				strb.Append("000000");
			}
			//G01 transaction reference number of original claim AN 14
			strb.Append(Canadian.TidyAN(claim.CanadaTransRefNum,14));
			string errorMsg="";
			string result=Canadian.PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
			//Attach an ack to the etrans
			Etrans etransAck=new Etrans();
			etransAck.PatNum=etrans.PatNum;
			etransAck.PlanNum=etrans.PlanNum;
			etransAck.InsSubNum=etrans.InsSubNum;
			etransAck.CarrierNum=etrans.CarrierNum;
			etransAck.DateTimeTrans=DateTime.Now;
			etransAck.UserNum=Security.CurUser.UserNum;
			if(errorMsg!="") {
				etransAck.AckCode="R";//To allow the user to try and reverse the claim again.
				etransAck.Etype=EtransType.AckError;
				etransAck.Note=errorMsg;
				etrans.Note="failed";
			}
			else {
				try {
					CCDFieldInputter fieldInputter=new CCDFieldInputter(result);
					CCDField fieldG05=fieldInputter.GetFieldById("G05");
					if(fieldG05!=null) {
						etransAck.AckCode=fieldG05.valuestr;
					}
					etransAck.Etype=fieldInputter.GetEtransType();
				}
				catch {
					etransAck.AckCode="R";//To allow the user to try and reverse the claim again.
					etransAck.Etype=EtransType.AckError;
					etrans.Note="Could not parse response from ITRANS.";
				}
			}
			Etranss.Insert(etransAck);
			Etranss.SetMessage(etransAck.EtransNum,result);
			etrans.AckEtransNum=etransAck.EtransNum;
			Etranss.Update(etrans);
			Etranss.SetMessage(etrans.EtransNum,strb.ToString());
			if(errorMsg!="") {
				throw new ApplicationException(errorMsg);
			}
			if(!isAutomatic && printCCD != null) {
				printCCD(etrans,result,true);//Physically print the form.
			}
			if(etrans.AckCode=="R") {
				throw new ApplicationException(Lans.g("CanadianOutput","Reversal was rejected by clearinghouse. The claim must be reversed manually."));
			}
			return etransAck.EtransNum;
		}

		///<summary>Can throw exceptions. Returns a list of file info for each attachmentBase64 in descending order by size.</summary>
		public static List<FileInfo> GetInfoForAttachments(List<ClaimAttach> listAttachments) {
			List<FileInfo> listFileInfos=new List<FileInfo>();
			for(int i=0;i<listAttachments.Count;i++) {
				string attachmentPath=FileAtoZ.CombinePaths(EmailAttaches.GetAttachPath(),listAttachments[i].ActualFileName);
				//Mimics WebFormL.LoadImagesToSheetDef()
				if((PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ && !File.Exists(attachmentPath)) ||
					(CloudStorage.IsCloudStorage && !CloudStorage.FileExists(attachmentPath)))
				{
					throw new ODException($"The file {listAttachments[i].DisplayedFileName} could not be found in {attachmentPath}");
				}
				string extension=Path.GetExtension(attachmentPath).ToLower();
				Bitmap bitmap=null;
				//The standard format requires one of these types JPG=JPG file, DIC=DICOM, TXT=ASCII text file, DOC=Microsoft Word. Images will be converted to JPG.
				if(extension.In(".txt",".doc",".docx")) {
					//Good to include raw bytes as base64 in outgoing format.
				}
				else {
					try {
						bitmap=new Bitmap(attachmentPath);
					}
					catch(Exception ex) {
						throw new ODException($"{listAttachments[i].ActualFileName} is not a supported file type.",ex);
					}
				}
				if(bitmap!=null) {//Convert images to to JPG.
					string tempFilePath=ODFileUtils.CreateRandomFile(PrefC.GetTempFolderPath(),".jpg");
					ImageCodecInfo codec=ImageCodecInfo.GetImageEncoders().FirstOrDefault(x => x.MimeType=="image/jpeg");
					EncoderParameters encoderParams=new EncoderParameters(2);
					encoderParams.Param[0]=new EncoderParameter(System.Drawing.Imaging.Encoder.Quality,(long)100);
					encoderParams.Param[1]=new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth,(long)24);//RGB, no alpha.
					bitmap.Save(tempFilePath,codec,encoderParams);
					bitmap.Dispose();
					attachmentPath=tempFilePath;
				}
				FileInfo fileInfo=new FileInfo(attachmentPath);
				long attachmentSize=((fileInfo.Length*4)/3)+1;//Calculate total size when raw bytes are converted to base64 for outgoing format.
				if(attachmentSize>9999999) {
					throw new ODException($"The file {listAttachments[i].DisplayedFileName} exceeds the size limit of 10MB.");
				}
				listFileInfos.Add(fileInfo);
			}
			listFileInfos=listFileInfos.OrderByDescending(x => x.Length).ToList();//Sort by size to get largest first.
			return listFileInfos;
		}

		///<summary>Each payment reconciliation request can return up to 9 pages. Each request is to one carrier only, so carrier cannot be null. This function will return one etrans ack for each page in the result, since each page must be requested individually. Only for version 04, no such transaction exists for version 02. The provTreat and provBilling must be validated as CDANet providers before calling this function.</summary>
		public static List<Etrans> GetPaymentReconciliations(Clearinghouse clearinghouseClin,Carrier carrier,Provider provTreat,Provider provBilling,
			DateTime reconciliationDate,long clinicNum,bool isAutomatic,PrintCCD printCCD) {
			if(clearinghouseClin==null) {
				throw new ApplicationException("Canadian clearinghouse not found.");
			}
			Canadian.ValidateExportPath(clearinghouseClin);
			List<Etrans> etransAcks=new List<Etrans>();
			int pageNumber=1;
			int totalPages=1;
			do {
				StringBuilder strb=new StringBuilder();
				if((carrier.CanadianSupportedTypes&CanSupTransTypes.RequestForPaymentReconciliation_06)!=CanSupTransTypes.RequestForPaymentReconciliation_06) {
					throw new ApplicationException("The carrier does not support payment reconciliation transactions.");
				}
				if(carrier.CanadianNetworkNum==0) {
					throw new ApplicationException("Carrier network not set.");
				}
				CanadianNetwork network=CanadianNetworks.GetNetwork(carrier.CanadianNetworkNum,clearinghouseClin);
				Etrans etrans=Etranss.CreateCanadianOutput(0,carrier.CarrierNum,carrier.CanadianNetworkNum,
					clearinghouseClin.HqClearinghouseNum,EtransType.RequestPay_CA,0,0,Security.CurUser.UserNum);
				//A01 transaction prefix 12 AN
				strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
				//A02 office sequence number 6 N
				strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
				//A03 format version number 2 N
				strb.Append("04");
				//A04 transaction code 2 N
				strb.Append("06");//payment reconciliation request
													//A05 carrier id number 6 N
				if(network.CanadianIsRprHandler) {
					strb.Append("999999");//Always 999999 if the network handles the RPR requests instead of the carriers in the network.
				}
				else {
					strb.Append(carrier.ElectID);//already validated as 6 digit number.
				}
				//A06 software system id 3 AN
				strb.Append(Canadian.SoftwareSystemId());
				//A10 encryption method 1 N
				if(carrier!=null) {
					strb.Append(carrier.CanadianEncryptionMethod);//validated in UI
				}
				else {
					strb.Append("1");//No encryption when sending to a network.
				}
				//A07 message length N4
				strb.Append(Canadian.TidyN("77",5));
				//A09 carrier transaction counter 5 N
				strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
				//B01 CDA provider number 9 AN
				strb.Append(Canadian.TidyAN(provTreat.NationalProvID,9));//already validated
																																 //B02 (treating) provider office number 4 AN
				strb.Append(Canadian.TidyAN(provTreat.CanadianOfficeNum,4));//already validated
																																		//B03 billing provider number 9 AN
																																		//might need to account for possible 5 digit prov id assigned by carrier
				strb.Append(Canadian.TidyAN(provBilling.NationalProvID,9));//already validated
																																	 //B04 billing provider office number 4 AN
				strb.Append(Canadian.TidyAN(provBilling.CanadianOfficeNum,4));//already validated
																																			//F33 Reconciliation Date 8 N
				strb.Append(reconciliationDate.ToString("yyyyMMdd"));
				//F38 Current Reconciliation Page Number N 1
				strb.Append(Canadian.TidyN(pageNumber,1));
				//End of message construction.
				string errorMsg="";
				string result=Canadian.PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
				//Attach an ack to the etrans
				Etrans etransAck=new Etrans();
				etransAck.PatNum=etrans.PatNum;
				etransAck.PlanNum=etrans.PlanNum;
				etransAck.InsSubNum=etrans.InsSubNum;
				etransAck.CarrierNum=etrans.CarrierNum;
				etransAck.DateTimeTrans=DateTime.Now;
				etransAck.UserNum=Security.CurUser.UserNum;
				CCDFieldInputter fieldInputter=null;
				if(errorMsg!="") {
					etransAck.Etype=EtransType.AckError;
					etransAck.Note=errorMsg;
					etrans.Note="failed";
				}
				else {
					fieldInputter=new CCDFieldInputter(result);
					CCDField fieldG05=fieldInputter.GetFieldById("G05");
					if(fieldG05!=null) {
						etransAck.AckCode=fieldG05.valuestr;
					}
					etransAck.Etype=fieldInputter.GetEtransType();
				}
				Etranss.Insert(etransAck);
				Etranss.SetMessage(etransAck.EtransNum,result);
				etrans.AckEtransNum=etransAck.EtransNum;
				Etranss.Update(etrans);
				Etranss.SetMessage(etrans.EtransNum,strb.ToString());
				etransAcks.Add(etransAck);
				if(errorMsg!="") {
					throw new ApplicationException(errorMsg);
				}
				CCDField fieldG62=fieldInputter.GetFieldById("G62");//Last reconciliation page number.
				totalPages=PIn.Int(fieldG62.valuestr);
				if(!isAutomatic && printCCD != null) {
					printCCD(etrans,result,true);//Physically print the form.
				}
				pageNumber++;
			} while(pageNumber<=totalPages);
			return etransAcks;
		}

		///<summary>Does not exist in version 02 so only supported for version 04. Returns the request Etrans record. Usually pass in a carrier with network null.  If sending to a network, carrier will be null and we still don't see anywhere in the message format to specify network.  We expect to get clarification on this issue later. Validate provTreat as a CDANet provider before calling this function.</summary>
		public static Etrans GetSummaryReconciliation(Clearinghouse clearinghouseClin,Carrier carrier,CanadianNetwork network,Provider provTreat,
			DateTime reconciliationDate,bool isAutomatic,PrintCCD printCCD) {
			if(clearinghouseClin==null) {
				throw new ApplicationException("Canadian clearinghouse not found.");
			}
			Canadian.ValidateExportPath(clearinghouseClin);
			StringBuilder strb=new StringBuilder();
			Etrans etrans=null;
			if(carrier!=null) {
				if((carrier.CanadianSupportedTypes&CanSupTransTypes.RequestForSummaryReconciliation_05)!=CanSupTransTypes.RequestForSummaryReconciliation_05) {
					throw new ApplicationException("The carrier does not support summary reconciliation transactions.");
				}
				etrans=Etranss.CreateCanadianOutput(0,carrier.CarrierNum,carrier.CanadianNetworkNum,
					clearinghouseClin.HqClearinghouseNum,EtransType.RequestSumm_CA,0,0,Security.CurUser.UserNum);
			}
			else {//Assume network!=null
				etrans=Etranss.CreateCanadianOutput(0,0,network.CanadianNetworkNum,
					clearinghouseClin.HqClearinghouseNum,EtransType.RequestSumm_CA,0,0,Security.CurUser.UserNum);
			}
			//A01 transaction prefix 12 AN
			strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
			//A02 office sequence number 6 N
			strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
			//A03 format version number 2 N
			strb.Append("04");
			//A04 transaction code 2 N
			strb.Append("05");//payment reconciliation request
												//A05 carrier id number 6 N
			if(carrier!=null) {
				strb.Append(carrier.ElectID);//already validated as 6 digit number.
			}
			else { //Assume network!=null
				strb.Append("999999");//Always 999999 when sending to a network.
			}
			//A06 software system id 3 AN
			strb.Append(Canadian.SoftwareSystemId());
			//A10 encryption method 1 N
			if(carrier!=null) {
				strb.Append(carrier.CanadianEncryptionMethod);//validated in UI
			}
			else { //Assume network!=null
				strb.Append("1");//No encryption when sending to a network.
			}
			//A07 message length N4
			strb.Append(Canadian.TidyN("63",5));
			//A09 carrier transaction counter 5 N
			strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
			//B01 CDA provider number 9 AN
			strb.Append(Canadian.TidyAN(provTreat.NationalProvID,9));//already validated
																															 //B02 (treating) provider office number 4 AN
			strb.Append(Canadian.TidyAN(provTreat.CanadianOfficeNum,4));//already validated
																																	//F33 Reconciliation Date 8 N
			strb.Append(reconciliationDate.ToString("yyyyMMdd"));
			//End of message construction.
			string errorMsg="";
			string result=Canadian.PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
			//Attach an ack to the etrans
			Etrans etransAck=new Etrans();
			etransAck.PatNum=etrans.PatNum;
			etransAck.PlanNum=etrans.PlanNum;
			etransAck.InsSubNum=etrans.InsSubNum;
			etransAck.CarrierNum=etrans.CarrierNum;
			etransAck.DateTimeTrans=DateTime.Now;
			etransAck.UserNum=Security.CurUser.UserNum;
			CCDFieldInputter fieldInputter=null;
			if(errorMsg!="") {
				etransAck.Etype=EtransType.AckError;
				etransAck.Note=errorMsg;
				etrans.Note="failed";
			}
			else {
				fieldInputter=new CCDFieldInputter(result);
				CCDField fieldG05=fieldInputter.GetFieldById("G05");
				if(fieldG05!=null) {
					etransAck.AckCode=fieldG05.valuestr;
				}
				etransAck.Etype=fieldInputter.GetEtransType();
			}
			Etranss.Insert(etransAck);
			Etranss.SetMessage(etransAck.EtransNum,result);
			etrans.AckEtransNum=etransAck.EtransNum;
			Etranss.Update(etrans);
			Etranss.SetMessage(etrans.EtransNum,strb.ToString());
			if(errorMsg!="") {
				throw new ApplicationException(errorMsg);
			}
			if(!isAutomatic && printCCD != null) {
				printCCD(etrans,result,true);//Physically print the form.
			}
			return etrans;
		}

		///<summary>Throws exception.
		///Returns the list of etrans requests for the given clearinghouse, or for a single carrier if specified.
		///The etrans.AckEtransNum can be used to get the etrans ack.
		///The following are the only possible formats that can be returned in the acks:
		///21 EOB Response, 11 Claim Ack, 14 Outstanding Transactions Response, 23 Predetermination EOB, 13 Predetermination Ack, 24 E-Mail Response.
		///The given provider can be any CDANet provider for the selected office.
		///Valid values for formatVersion are "02" or "04".
		///Send to a specific carrier by setting the carrier varaible, or leave null if sending to entire network.
		///If the user initiated this call, then set the form and ccd delegate so the output can be printed.
		///Otherwise, set form and ccd delegate to null to supress UI and printer output.</summary>
		private static List<Etrans> GetOutstandingForClearinghouse(Clearinghouse clearinghouseClin,Provider prov,string formatVersion,Carrier carrier,
			CanadianNetwork network,PrintCdaClaimForm printForm,PrintCCD printCCD) 
		{
			if(clearinghouseClin==null) {
				throw new ApplicationException("Canadian clearinghouse not found.");
			}
			Canadian.ValidateExportPath(clearinghouseClin);
			List<Carrier> listCarriers=new List<Carrier>() { carrier };
			//Version 02 is the only version allowed to have a null carrier.
			if(carrier==null && formatVersion!="02") {
				//Override the list of carriers with CDA carriers that are in use.
				listCarriers=Carriers.GetCdaCarriersInUse();
				//Remove all carriers that do not support ROT request transactions or that do not match the format version.
				listCarriers.RemoveAll(x => !x.CanadianSupportedTypes.HasFlag(CanSupTransTypes.RequestForOutstandingTrans_04)
					|| x.CDAnetVersion!=formatVersion);
				if(listCarriers.IsNullOrEmpty()) {
					throw new ApplicationException("No carriers found that have sent a claim before, "
						+"are flagged as 'Is CDAnet Carrier', and support requests for outstanding transactions.\r\n\r\n"
						+"Send a claim before requesting outstanding transactions or manually pick a specific carrier.");
				}
			}
			List<Etrans> listEtrans=new List<Etrans>();
			for(int i=0;i<listCarriers.Count;i++) {
				try {
					listEtrans.AddRange(GetOutstandingForCarrier(clearinghouseClin,prov,formatVersion,listCarriers[i],network,printForm,printCCD));
				}
				catch(Exception ex) {
					string error="";
					if(listCarriers[i]!=null) {
						error+=$"'{listCarriers[i].CarrierName}' error:\r\n";
					}
					error+=ex.Message;
					throw new ApplicationException(error);
				}
			}
			return listEtrans;
		}

		private static List<Etrans> GetOutstandingForCarrier(Clearinghouse clearinghouseClin,Provider prov,string formatVersion,Carrier carrier,
			CanadianNetwork network,PrintCdaClaimForm printForm,PrintCCD printCCD)
		{
			//We are required to send the request for outstanding transactions over and over until we get back an outstanding transactions ack format (Transaction type 14), because
			//there may be more than one item in the mailbox and we can only get one item at time.
			if(carrier==null && formatVersion!="02") {//Version 02 is the only version allowed to have a null carrier.
				throw new ApplicationException("A carrier is required in order to request for outstanding transactions.");
			}
			if(carrier!=null && !carrier.CanadianSupportedTypes.HasFlag(CanSupTransTypes.RequestForOutstandingTrans_04)) {
				throw new ApplicationException($"This carrier does not support request for outstanding transactions.");
			}
			List<Etrans> etransAcks=new List<Etrans>();
			bool isAutomatic=(printForm==null && printCCD==null);
			bool exit=false;
			do {
				StringBuilder strb=new StringBuilder();
				Etrans etrans=Etranss.CreateCanadianOutput(0,(carrier==null)?0:carrier.CarrierNum,(network==null)?0:network.CanadianNetworkNum,
					clearinghouseClin.HqClearinghouseNum,EtransType.RequestOutstand_CA,0,0,Security.CurUser.UserNum);
				//A01 transaction prefix 12 AN
				if(network==null) {//iTrans will always hit this, or running for Version 2 or Version 4 for all carriers.
					strb.Append("            ");
				}
				else {//To specific ITRANS version 04 carrier, or ClaimStream Telus A network or Telus B network.
					strb.Append(Canadian.TidyAN(network.CanadianTransactionPrefix,12));
				}
				//A02 office sequence number 6 N
				strb.Append(Canadian.TidyN(etrans.OfficeSequenceNumber,6));
				//A03 format version number 2 N
				strb.Append(formatVersion);
				//A04 transaction code 2 N
				strb.Append("04");//outstanding transactions request
				if(formatVersion=="04") {
					//A05 carrier id number 6 N
					strb.Append(carrier.ElectID);//already validated as 6 digit number.
				}
				//A06 software system id 3 AN
				strb.Append(Canadian.SoftwareSystemId());
				if(formatVersion=="04") {
					//A10 encryption method 1 N
					strb.Append(carrier.CanadianEncryptionMethod);//validated in UIs
				}
				//A07 message length N4
				if(formatVersion=="04") {
					strb.Append(Canadian.TidyN("64",5));
				}
				else if(formatVersion=="02") {
					strb.Append(Canadian.TidyN("42",4));
				}
				if(formatVersion=="04") {
					//A09 carrier transaction counter 5 N
					strb.Append(Canadian.TidyN(etrans.CarrierTransCounter,5));
				}
				//According to the documentation for the outstanding transactions ack format, B01 only has to be a valid provider for the practice,
				//and that will trigger acknowledgements for all providers of the practice. I am assuming here that the same is true for the 
				//billing provider in field B03, because there is no real reason to limit the request to any particular provider.
				//B01 CDA provider number 9 AN
				strb.Append(Canadian.TidyAN(prov.NationalProvID,9));//already validated
				//B02 (treating) provider office number 4 AN
				strb.Append(Canadian.TidyAN(prov.CanadianOfficeNum,4));//already validated
				if(formatVersion=="04") {
					//B03 billing provider number 9 AN
					//might need to account for possible 5 digit prov id assigned by carrier
					strb.Append(Canadian.TidyAN(prov.NationalProvID,9));//already validated
				}
				string errorMsg="";
				string result=Canadian.PassToIca(strb.ToString(),clearinghouseClin,network,isAutomatic,out errorMsg);
				//Attach an ack to the etrans
				Etrans etransAck=new Etrans();
				etransAck.PatNum=etrans.PatNum;
				etransAck.PlanNum=etrans.PlanNum;
				etransAck.InsSubNum=etrans.InsSubNum;
				etransAck.CarrierNum=etrans.CarrierNum;
				etransAck.DateTimeTrans=DateTime.Now;
				CCDFieldInputter fieldInputter=null;
				if(errorMsg!="") {
					etransAck.Etype=EtransType.AckError;
					etransAck.Note=errorMsg;
					etrans.Note="failed";
				}
				else {
					if(result.Substring(12).StartsWith("NO MORE ITEMS")) {
						etransAck.Etype=EtransType.OutstandingAck_CA;
						exit=true;
					}
					else {
						fieldInputter=new CCDFieldInputter(result);
						CCDField fieldG05=fieldInputter.GetFieldById("G05");
						if(fieldG05!=null) {
							etransAck.AckCode=fieldG05.valuestr;
						}
						etransAck.Etype=fieldInputter.GetEtransType();
						if(!EnumTools.GetAttributeOrDefault<EtransTypeAttr>(etransAck.Etype).IsRotResponseType) {
							string invalidTypeMsg="Invalid outstanding transaction response type returned.";
							etransAck.Etype=EtransType.AckError;
							etransAck.Note=invalidTypeMsg;
							etrans.Note="failed";
							errorMsg=$"{invalidTypeMsg}"
								+"\r\nThe only valid response types according to message standards are:"
								+"\r\n  11 - A Claim Acknowledgment"
								+"\r\n  13 - A Predetermination Acknowledgment"
								+"\r\n  14 - An Outstanding Transaction Response"
								+"\r\n  21 - Explanation of Benefit Response"
								+"\r\n  23 - A Predetermination Explanation of Benefits"
								+"\r\n  24 - An E-Mail Response"
								+$"\r\n\r\nThe invalid type in the response was '{fieldInputter.GetValue("A04")}'";
						}
					}
				}
				Etranss.Insert(etransAck);
				//Update etransAck and etrans here with EtransMessageTextNum as these objects are sometimes used again later to update the DB.
				etransAck.EtransMessageTextNum=Etranss.SetMessage(etransAck.EtransNum,result);
				etrans.AckEtransNum=etransAck.EtransNum;
				Etranss.Update(etrans);
				etrans.EtransMessageTextNum=Etranss.SetMessage(etrans.EtransNum,strb.ToString());
				etransAcks.Add(etransAck);
				if(errorMsg!="") {
					//A good amount of error messages from within Canadian.PassToIca() require manual intervention from the user.
					//Throw an exception with the error message in order to notify the user of the specific error message and break out of the loop.
					//If the while loop was allowed to continue and this same error occurred again the database will quickly fill up with etrans entries.
					throw new ApplicationException(errorMsg);
				}
				if(fieldInputter==null) {//happens in version 02 when a terminating message containing the text "NO MORE ITEMS" is received.
					break;
				}
				string responseFormatVersion=fieldInputter.GetFieldById("A03").valuestr;
				CCDField fieldA04=fieldInputter.GetFieldById("A04");//message format
				if(responseFormatVersion=="02") {
					//In this case, there are only 4 possible responses:
					//EOB, Claim Ack, Claim Ack with an error code, or Claim Ack with literal "NO MORE ITEMS" starting at character 13.
					if(fieldA04.valuestr=="11") {
						CCDField fieldG08=fieldInputter.GetFieldById("G08");
						if(fieldG08!=null && (fieldG08.valuestr=="004" || fieldG08.valuestr=="049")) { //Exit conditions specified in the documentation.
							etransAck.Etype=EtransType.OutstandingAck_CA;
							exit=true;
						}
					}
				}
				else if(responseFormatVersion=="04") {
					//Remember, the only allowed response transaction types are:
					//21 EOB Response, 11 Claim Ack, 14 Outstanding Transactions Response, 23 Predetermination EOB, 13 Predetermination Ack, 24 E-Mail Response
					if(fieldA04.valuestr=="14") {//Outstanding Transaction Ack Format
						CCDField fieldG05=fieldInputter.GetFieldById("G05");//response status
						if(fieldG05.valuestr=="R") {//We only expect the result to be 'R' or 'X' as specified in the documentation.
							CCDField fieldG07=fieldInputter.GetFieldById("G07");//disposition message
							CCDField fieldG08=fieldInputter.GetFieldById("G08");//error code
							if(!isAutomatic) {
								MessageBox.Show(Lans.g("","Failed to receive outstanding transactions. Messages from CDANet")+": "+Environment.NewLine+
									fieldG07.valuestr.Trim()+Environment.NewLine+((fieldG08!=null) ? CCDerror.message(PIn.Int(fieldG08.valuestr),IsDentalOfficeFrench()) : ""));
							}
						}
						etransAck.Etype=EtransType.OutstandingAck_CA;
						exit=true;
					}
				}
				//Field A02 exists in all of the possible formats (21,11,14,23,13,24).
				CCDField fieldA02=fieldInputter.GetFieldById("A02");//office sequence number
				//We use the Office Sequence Number to find the original etrans entry so that we can discover which patient the response is referring to.
				Etrans etranOriginal=Etranss.GetForSequenceNumberCanada(fieldA02.valuestr);
				if(etranOriginal!=null) {//Null will happen when testing, but should not happen in production.
					etrans.PatNum=etranOriginal.PatNum;
					etrans.PlanNum=etranOriginal.PlanNum;
					etrans.InsSubNum=etranOriginal.InsSubNum;
					etrans.ClaimNum=etranOriginal.ClaimNum;
					Etranss.Update(etrans);
					etransAck.PatNum=etranOriginal.PatNum;
					etransAck.PlanNum=etranOriginal.PlanNum;
					etransAck.InsSubNum=etranOriginal.InsSubNum;
					etransAck.ClaimNum=etranOriginal.ClaimNum;
					CCDField fieldA05=fieldInputter.GetFieldById("A05");//CarrierID, exists in all formats but 24-Email, and 16-Payment Reconciliation Response
					if(fieldA05!=null) {
						Carrier carrierA05=Carriers.GetAllByElectId(fieldA05.valuestr).FirstOrDefault(x => x.IsCDA && !x.IsHidden);//Get Carrier from ElectID
						etransAck.CarrierNum=carrierA05.CarrierNum;
					}
					Etranss.Update(etransAck);
					if(!exit) {
						Claim claim=null;
						if(etransAck.ClaimNum!=0) {
							claim=Claims.GetClaim(etransAck.ClaimNum);
						}
						if(claim!=null) {
							if(etransAck.AckCode=="A") {
								claim.ClaimStatus="R";
								claim.DateReceived=MiscData.GetNowDateTime();
							}
							else if(etransAck.AckCode=="H" || etransAck.AckCode=="B" || etransAck.AckCode=="C" || etransAck.AckCode=="N") {
								//We don't need to worry about auto receiving the claim with $0 payment for the InsAutoReceiveNoAssign preference
								//here because we know the claim was previously sent and this will already have been done.
								claim.ClaimStatus="S";
							}
							else if(etransAck.AckCode=="M") {
								if(!isAutomatic) {
									printForm(claim);
								}
							}
							Claims.Update(claim);
							if((fieldA04.valuestr=="21" || fieldA04.valuestr=="23")//EOB or Predetermination EOB
								&& clearinghouseClin.IsEraDownloadAllowed!=EraBehaviors.None 
								&& InsSubs.GetOne(claim.InsSubNum).AssignBen)
							{
								List<Procedure> listAllProcs=Procedures.Refresh(claim.PatNum);
								List<ClaimProc> listAllClaimProcs=ClaimProcs.Refresh(claim.PatNum);
								List<ClaimProc> listClaimProcsForClaim=listAllClaimProcs.FindAll(x => x.ClaimNum==claim.ClaimNum);
								//int patAge=0;
								//if(clearinghouseClin.IsEraDownloadAllowed==EraBehaviors.DownloadDoNotReceive) {
									Patient pat=Patients.GetPat(claim.PatNum);
								//	patAge=pat.Age;
								//}
								Canadian.EOBImportHelper(fieldInputter,listClaimProcsForClaim,listAllProcs,listAllClaimProcs,claim,true,null,clearinghouseClin.IsEraDownloadAllowed,pat);
								SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,claim.PatNum
									,"Claim for service date "+POut.Date(claim.DateService)+" amounts overwritten using received EOB amounts."
									,LogSources.CanadaEobAutoImport);
							}
						}
					}
				}
				if(!exit && !isAutomatic) {
					try {
						printCCD(etrans,result,true);//Physically print the form.
					}
					catch {
						using MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(Lans.g("CanadianOutput","Failed to display one of the ROT responses, here is the raw message")+": "+Environment.NewLine+result);
						msgbox.ShowDialog();
					}
				}
			} while(!exit);
			return etransAcks;
		}

		///<summary>Returns the list of etrans requests for the default clearinghouse, or for a single carrier if specified.
		///The etrans.AckEtransNum can be used to get the etrans ack.
		///The following are the only possible formats that can be returned in the acks:
		///21 EOB Response, 11 Claim Ack, 14 Outstanding Transactions Response, 23 Predetermination EOB, 13 Predetermination Ack, 24 E-Mail Response.
		///The given provider can be any CDANet provider for the selected office.
		///Valid values for formatVersion are "02" or "04", or empty string for all versions.
		///Send to a specific carrier by setting the carrier varaible, or leave null if sending to entire network.
		///If the user initiated this call, then set the form and ccd delegate so the output can be printed.
		///Otherwise, set form and ccd delegate to null to supress UI and printer output.</summary>
		public static List<Etrans> GetOutstandingForDefault(Provider prov,string formatVersion="",Carrier carrier=null,
			PrintCdaClaimForm printForm=null,PrintCCD printCCD=null)
		{
			List<Etrans> listEtrans=new List<Etrans>();
			//If carrier is Alberta Blue Cross (ABC), then ClaimStream will be selected as the clearinghouseHq if the user has the recommended setup.
			Clearinghouse clearinghouseHq=Canadian.GetCanadianClearinghouseHq(carrier);
			Clearinghouse clearinghouseClin=Clearinghouses.OverrideFields(clearinghouseHq,Clinics.ClinicNum);
			CanadianNetwork netTelusA=CanadianNetworks.GetFirstOrDefault(x => x.Abbrev=="TELUS A");
			CanadianNetwork netTelusB=CanadianNetworks.GetFirstOrDefault(x => x.Abbrev=="TELUS B");
			//Check version 04 reports first, in case there is an error.  Most carrier use version 04.
			//If there is an error with version 02, then we would not want it to stop version 04 reports from running.
			//Therefore, version 02 reports should come after version 04 reports.
			if(String.IsNullOrEmpty(formatVersion) || formatVersion=="04") {//Version 04 request.
				if(carrier==null) {//Version 04 request for all networks.
					if(clearinghouseHq.CommBridge.In(EclaimsCommBridge.ITRANS,EclaimsCommBridge.ITRANS2)) {
						listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"04",null,null,printForm,printCCD));
					}
					else if(clearinghouseHq.CommBridge==EclaimsCommBridge.Claimstream) {
						//Alberta Blue Cross (ABC) only accepts requests with a carrier specified (since there is only 1 carrier in their network).
						listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"04",null,netTelusA,printForm,printCCD));
						listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"04",null,netTelusB,printForm,printCCD));
					}
				}
				else {//Version 04 request for a specific carrier.  Could be for either ITRANS or ClaimStream, depending on clearinghouse selected above.
					//Alberta Blue Cross (ABC) always has to be checked this way.
					CanadianNetwork net=CanadianNetworks.GetNetwork(carrier.CanadianNetworkNum,clearinghouseClin);//We always know which network if the carrier is specified.
					listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"04",carrier,net,printForm,printCCD));
				}
			}
			if((String.IsNullOrEmpty(formatVersion) || formatVersion=="02") && carrier==null) {//Version 02 request.  Always an entire clearinghouse, not a carrier.
				if(clearinghouseHq.CommBridge.In(EclaimsCommBridge.ITRANS,EclaimsCommBridge.ITRANS2)) {
					listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"02",null,null,printForm,printCCD));
				}
				else if(clearinghouseHq.CommBridge==EclaimsCommBridge.Claimstream) {
					//Alberta Blue Cross (ABC) uses version 04.  Therefore, no need to ask for version 02 reports.
					listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"02",null,netTelusA,printForm,printCCD));
					listEtrans.AddRange(CanadianOutput.GetOutstandingForClearinghouse(clearinghouseClin,prov,"02",null,netTelusB,printForm,printCCD));
				}
			}
			return listEtrans;
		}

	}
}
