using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeBase;
using OpenDentBusiness.Eclaims;

namespace OpenDentBusiness
{ 
	///<summary></summary>
	public class X837_5010:X12object{

		///<summary>Data element separator character. Almost always '*', the ASCII hexadecimal value of 2A. For Denti-Cal, ASCII hexadecimal value of 1D which is an unprintable character.</summary>
		private static string s="*";
		///<summary>Component element separator character. Almost always ':', the ASCII hexadecimal value of 3A. For Denti-Cal, ASCII hexadecimal value of 22 which is '"'.</summary>
		private static string isa16=":";
		///<summary>Segment terminator character.  We also add the newline to this variable, although we plan to move that into EndSegment() eventually.  Almost always '~', the ASCII hexadecimal value of 7E. For Denti-Cal, ASCII hexadecimal value of 1C which is an unprintable character.</summary>
		private static string endSegment="~"+Environment.NewLine;
		private static int seg;//segments for a particular ST-SE transaction

		public X837_5010(string messageText):base(messageText){
		
		}
		
		public static void GenerateMessageText(StreamWriter sw,Clearinghouse clearinghouseClin,int batchNum,List<ClaimSendQueueItem> listQueueItems,EnumClaimMedType medType) {
			if(clearinghouseClin.SeparatorData=="") {
				s="*";
			}
			else {
				s=""+Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(clearinghouseClin.SeparatorData,16) })[0]; //Validated to be a 2 digit hexadecimal number in UI.
			}
			if(clearinghouseClin.ISA16=="") {
				isa16=":";
			}
			else {
				isa16=""+Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(clearinghouseClin.ISA16,16) })[0]; //Validated to be a 2 digit hexadecimal number in UI.
			}
			if(clearinghouseClin.SeparatorSegment=="") {
				endSegment="~"+Environment.NewLine;
			}
			else {
				endSegment=""+Encoding.ASCII.GetChars(new byte[] { Convert.ToByte(clearinghouseClin.SeparatorSegment,16) })[0]+Environment.NewLine; //Validated to be a 2 digit hexadecimal number in UI.
			}
			//Interchange Control Header (Interchange number tracked separately from transactionNum)
			//We set it to between 1 and 999 for simplicity
			sw.Write("ISA"+s
				+"00"+s//ISA01 2/2 Authorization Information Qualifier: 00=No Authorization Information Present (No meaningful information in ISA02).
				+Sout(clearinghouseClin.ISA02,10,10)+s//ISA02 10/10 Authorization Information: Blank
				+"00"+s//ISA03 2/2 Security Information Qualifier: 00=No Security Information Present (No meaningful information in ISA04).
				+Sout(clearinghouseClin.ISA04,10,10)+s//ISA04 10/10 Security Information: Blank
				+clearinghouseClin.ISA05+s//ISA05 2/2 Interchange ID Qualifier: ZZ=mutually defined. 30=TIN. Validated
				+X12Generator.GetISA06(clearinghouseClin)+s//ISA06 15/15 Interchange Sender ID: Sender ID(TIN) Or might be TIN of Open Dental.
				+clearinghouseClin.ISA07+s//ISA07 2/2 Interchange ID Qualifier: ZZ=mutually defined. 30=TIN. Validated
				+Sout(clearinghouseClin.ISA08,15,15,true)+s//ISA08 15/15 Interchange Receiver ID: Validated to make sure length is at least 2.
				+DateTime.Today.ToString("yyMMdd")+s//ISA09 6/6 Interchange Date: today's date.
				+DateTime.Now.ToString("HHmm")+s//ISA10 4/4 Interchange Time: current time
				+"^"+s//ISA11 1/1 Repetition Separator:
				+"00501"+s//ISA12 5/5 Interchange Control Version Number:
				+batchNum.ToString().PadLeft(9,'0')+s//ISA13 9/9 Interchange Control Number:
				+"0"+s//ISA14 1/1 Acknowledgement Requested: 0=No Interchange Acknowledgment Requested.
				+clearinghouseClin.ISA15+s//ISA15 1/1 Interchange Usage Indicator: T=Test, P=Production. Validated.
				+isa16//ISA16 1/1 Component Element Separator:
				+endSegment);
			//Just one functional group.
			WriteFunctionalGroup(sw,listQueueItems,batchNum,clearinghouseClin,medType);
			//Interchange Control Trailer
			sw.Write("IEA"+s
				+"1"+s//IEA01 1/5 Number of Included Functional Groups:
				+batchNum.ToString().PadLeft(9,'0')//IEA02 9/9 Interchange Control Number:
				+endSegment);
		}

		///<summary>Throws exception.</summary>
		private static void WriteFunctionalGroup(StreamWriter sw,List<ClaimSendQueueItem> queueItems,int batchNum,
			Clearinghouse clearinghouseClin,EnumClaimMedType medType)
		{
			#region Functional Group Header
			int transactionNum=1;//Gets incremented for each carrier. Can be reused in other functional groups and interchanges, so not persisted
			//Functional Group Header
			string groupControlNumber=batchNum.ToString();//Must be unique within file.  We will use batchNum
			string industryIdentifierCode="";
			if(medType==EnumClaimMedType.Medical) {
				industryIdentifierCode="005010X222A1";
			}
			else if(medType==EnumClaimMedType.Institutional) {
				industryIdentifierCode="005010X223A2";
			}
			else if(medType==EnumClaimMedType.Dental) {
				industryIdentifierCode="005010X224A2";
			}
			sw.Write("GS"+s
				+"HC"+s//GS01 2/2 Functional Identifier Code: Health Care Claim.
				+X12Generator.GetGS02(clearinghouseClin)+s//GS02 2/15 Application Sender's Code: Sometimes Jordan Sparks.  Sometimes the sending clinic.
				+Sout(clearinghouseClin.GS03,15,2,true)+s//GS03 2/15 Application Receiver's Code:
				+DateTime.Today.ToString("yyyyMMdd")+s//GS04 8/8 Date: today's date.
				+DateTime.Now.ToString("HHmm")+s//GS05 4/8 TIME: current time.
				+groupControlNumber+s//GS06 1/9 Group Control Number: No padding necessary.
				+"X"+s//GS07 1/2 Responsible Agency Code: X=Accredited Standards Committee X12.
				+industryIdentifierCode//GS08 1/12 Version/Release/Industry Identifier Code:
				+endSegment);
			#endregion Functional Group Header
			#region Define Variables
			int HLcount=1;
			int parentProv=0;//the HL sequence # of the current provider.
			int parentSubsc=0;//the HL sequence # of the current subscriber.
			string hasSubord="";//0 if no subordinate, 1 if at least one subordinate
			Claim claim;
			InsPlan insPlan;
			InsPlan otherPlan=null;
			InsSub sub;
			InsSub otherSub=new InsSub();
			Patient patient;
			Patient subscriber;
			Patient otherSubsc=new Patient();
			Carrier carrier;
			Carrier otherCarrier=new Carrier();
			List<Claim> listClaims;
			List<ClaimProc> claimProcList;//all claimProcs for a patient.
			List<ClaimProc> claimProcs;
			List<Procedure> procList;
			List<ToothInitial> initialList;
			List<PatPlan> patPlans;
			List<ProviderClinic> listProvClinics;
			List<long> listProvNums;
			Procedure proc;
			ProcedureCode procCode;
			Provider provTreat;//claim level treating provider.
			Provider billProv=null;
			ProviderClinic provClinicBill=null;
			ProviderClinic provClinicTreat=null;
			Clinic clinic=null;
			Site site=null;
			seg=0;
			#endregion Define Variables
			#region Transaction Set Header
			//if(i==0//if this is the first claim
			//	|| claimItems[i].PayorId0 != claimItems[i-1].PayorId0)//or the payorID has changed
			//{
			//	newTrans=true;
			//	seg=0;
			//}
			//else newTrans=false;
			//if(newTrans) {
			//Transaction Set Header (one for each carrier)
			//transactionNum gets incremented in SE section
			//ST02 Transact. control #. Must be unique within ISA
			//NO: So we used combination of transaction and group, eg 00011
			sw.Write("ST"+s
				+"837"+s//ST01 3/3 Transaction Set Identifier Code: 
				+transactionNum.ToString().PadLeft(4,'0')+s//ST02 4/9 Transaction Set Control Number: 
				+industryIdentifierCode);//ST03 1/35 Implementation Convention Reference:
			EndSegment(sw);
			sw.Write("BHT"+s
				+"0019"+s//BHT01 4/4 Hierarchical Structure Code: 0019=Information Source, Subscriber, Dependant.
				+"00"+s//BHT02 2/2 Transaction Set Purpose Code: 00=Original transmissions are transmissions which have never been sent to the reciever.
				+transactionNum.ToString().PadLeft(4,'0')+s//BHT03 1/50 Reference Identification: Can be same as ST02.
				+DateTime.Now.ToString("yyyyMMdd")+s//BHT04 8/8 Date: 
				+DateTime.Now.ToString("HHmmss")+s//BHT05 4/8 Time: 
				+"CH");//BHT06 2/2 Transaction Type Code: CH=Chargable.
			EndSegment(sw);
			//1000A Submitter is OPEN DENTAL and sometimes it's the practice
			//(depends on clearinghouse and Partnership agreements)
			//See 2010AA PER (after REF) for the new billing provider contact phone number
			//1000A NM1: 41 (medical,institutional,dental) Submitter Name.
			Write1000A_NM1(sw,clearinghouseClin);
			//1000A PER: IC (medical,institutional,dental) Submitter EDI Contact Information. Contact number.
			Write1000A_PER(sw,clearinghouseClin);
			//1000B NM1: 40 (medical,institutional,dental) Receiver Name. Always the Clearinghouse.
			sw.Write("NM1"+s
				+"40"+s//NM101 2/3 Entity Identifier Code: 40=Receiver.
				+"2"+s);//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
			if(IsClaimConnect(clearinghouseClin)) {
				sw.Write("CLAIMCONNECT"+s);//NM103 1/60 Name Last or Organization Name: Receiver Name.
			}
			else if(IsDentiCal(clearinghouseClin)) {
				sw.Write("DENTICAL"+s);//NM103 1/60 Name Last or Organization Name: Receiver Name.
			}
			else if(IsETACTICS(clearinghouseClin)) {
				sw.Write("ETACTICSINC"+s);//NM103 1/60 Name Last or Organization Name: Receiver Name.
			}
			else {
				sw.Write(Sout(clearinghouseClin.Description,60)+s);//NM103 1/60 Name Last or Organization Name: Receiver Name.
			}
			sw.Write(s//NM104 1/35 Name First: Not Used.
				+s//NM105 1/25 Name Middle: Not Used.
				+s//NM106 1/10 Name Prefix: Not Used.
				+s//NM107 1/10 Name Suffix: Not Used.
				+"46"+s);//NM108 1/2 Identification Code Qualifier: 46=Electronic Transmitter Identification Number (ETIN).
			if(IsDentiCal(clearinghouseClin)) {
				sw.Write("1822287119");//NM109 2/80 Identification Code: Receiver ID Code. DXC Technology ETIN effective immediately.  Required by 04/30/2018.
			}
			else if(IsETACTICS(clearinghouseClin)) {
				sw.Write("ETACTICSINC");//NM109 2/80 Identification Code: Receiver ID Code. aka ETIN#.
			}
			else {
				sw.Write(Sout(clearinghouseClin.ISA08,80,2,true));//NM109 2/80 Identification Code: Receiver ID Code. aka ETIN#.
			}
			EndSegment(sw);//NM110 through NM112 are not used.
			HLcount=1;
			parentProv=0;//the HL sequence # of the current provider.
			parentSubsc=0;//the HL sequence # of the current subscriber.
			hasSubord="";//0 if no subordinate, 1 if at least one subordinate
			//}
			#endregion
			listClaims=Claims.GetClaimsFromClaimNums(queueItems.Select(x => x.ClaimNum).ToList());
			listProvNums=listClaims.Select(x => x.ProvBill).Distinct().ToList();
			listProvNums.AddRange(listClaims.Select(x => x.ProvTreat).Distinct().ToList());
			listProvClinics=ProviderClinics.GetByProvNums(listProvNums.Distinct().ToList());
			for(int i=0;i<queueItems.Count;i++) {
				#region Initialize Variables
				claim=Claims.GetClaim(queueItems[i].ClaimNum);
				insPlan=InsPlans.GetPlan(claim.PlanNum,new List<InsPlan>());
				sub=InsSubs.GetSub(claim.InsSubNum,null);
				//insPlan could be null if db corruption. No error checking for that
				otherPlan=null;//Must be reset each time through because if otherPlan!=null below then we assume the current claim has a secondary plan.
				if(claim.PlanNum2>0) {
					otherPlan=InsPlans.GetPlan(claim.PlanNum2,new List<InsPlan>());
					otherSub=InsSubs.GetSub(claim.InsSubNum2,null);
					otherSubsc=Patients.GetPat(otherSub.Subscriber);
					otherCarrier=Carriers.GetCarrier(otherPlan.CarrierNum);//Returns empty Carrier if not found.
				}
				patient=Patients.GetPat(claim.PatNum);
				subscriber=Patients.GetPat(sub.Subscriber);
				carrier=Carriers.GetCarrier(insPlan.CarrierNum);
				claimProcList=ClaimProcs.Refresh(patient.PatNum);
				claimProcs=ClaimProcs.GetForSendClaim(claimProcList,claim.ClaimNum);
				procList=Procedures.Refresh(claim.PatNum);
				if(!x837Controller.HasValidProcNums(claimProcs,procList,claim,out string error)) {
					throw new ODException(error);
				}
				initialList=ToothInitials.Refresh(claim.PatNum);
				patPlans=PatPlans.Refresh(patient.PatNum);
				//Get all procedures corresponding to the claimprocs which also have a site.
				List<Procedure> listSiteProcs=procList.FindAll(x => claimProcs.Exists(y => y.ProcNum==x.ProcNum) && x.SiteNum!=0);
				//A null site is acceptable, because billing info will be used instead.
				if(listSiteProcs.Count > 0) {
					site=Sites.GetFirstOrDefault(x => x.SiteNum==listSiteProcs[0].SiteNum);
				}
				#endregion Initialize Variables
				#region Billing Provider
				//Billing address is based on clinic, not provider.  All claims in a batch are guaranteed to be from a single clinic.  That validation is done in FormClaimSend.
				//Although, now that we have a separate loop for each claim, we might be able to allow a batch with mixed clinics.
				if(PrefC.HasClinicsEnabled) {//if using clinics
					clinic=Clinics.GetClinic(claim.ClinicNum);//might be null
				}
				provClinicBill=ProviderClinics.GetFromList(claim.ProvBill,(clinic==null ? 0 : clinic.ClinicNum),listProvClinics,true);
				//2000A HL: (medical,instituational,dental) Billing Provider Hierarchical Level.
				sw.Write("HL"+s+HLcount.ToString()+s//HL01 1/12 Heirarchical ID Number: 
					+s//HL02 1/12 Hierarchical Parent ID Number: Not used.
					+"20"+s//HL03 1/2 Heirarchical Level Code: 20=Information Source.
					+"1");//HL04 1/1 Heirarchical Child Code. 1=Additional Subordinate HL Data Segment in This Hierarchical Structure.
				EndSegment(sw);
				//billProv=Providers.ListShallow[Providers.GetIndexLong(claimItems[i].ProvBill1)];
				billProv=Providers.GetProv(claim.ProvBill);
				//2000A PRV: BI (medical,institutional,dental) Billing Provider Specialty Information. Situational. Required when billing provider is treating provider.
				bool usePrvBilling=false;
				if(claim.ProvBill==claim.ProvTreat) {
					usePrvBilling=true;
				}
				else if(ListTools.In(medType,EnumClaimMedType.Medical,EnumClaimMedType.Institutional)) {
					//Previously we have had reports that all medical plans require this segment.
					usePrvBilling=true;
				}
				else if(IsPassportHealthMedicaid(clearinghouseClin,carrier) || IsWashingtonMedicaid(clearinghouseClin,carrier)) {
					usePrvBilling=true;
				}
				if(usePrvBilling) {
					sw.Write("PRV"+s
						+"BI"+s//PRV01 1/3 Provider Code: BI=Billing.
						+"PXC"+s//PRV02 2/3 Reference Identification Qualifier: PXC=Health Care Provider Taxonomy Code.
						+X12Generator.GetTaxonomy(billProv));//PRV03 1/50 Reference Identification: Provider Taxonomy Code.
					EndSegment(sw);//PRV04 through PRV06 are not used.
				}
				//2000A CUR: (medical,instituational,dental) Foreign Currency Information. Situational. We do not need to specify because united states dollars are default.
				//2010AA NM1: 85 (medical,institutional,dental) Billing Provider Name.
				if(medType==EnumClaimMedType.Institutional) {
					billProv.IsNotPerson=true;//Required by X12 specification. Cannot send a person as the billing provider.
				}
				WriteNM1Provider("85",sw,billProv);
				//2010AA N3: (medical,institutional,dental) Billing Provider Address.
				string billingAddress1="";
				string billingAddress2="";
				string billingCity="";
				string billingState="";
				string billingZip="";
				if(clinic!=null && clinic.UseBillAddrOnClaims) {
					billingAddress1=clinic.BillingAddress;
					billingAddress2=clinic.BillingAddress2;
					billingCity=clinic.BillingCity;
					billingState=clinic.BillingState;
					billingZip=clinic.BillingZip;
				}
				else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
					billingAddress1=PrefC.GetString(PrefName.PracticeBillingAddress);
					billingAddress2=PrefC.GetString(PrefName.PracticeBillingAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeBillingCity);
					billingState=PrefC.GetString(PrefName.PracticeBillingST);
					billingZip=PrefC.GetString(PrefName.PracticeBillingZip);
				}
				else if(clinic==null) {
					billingAddress1=PrefC.GetString(PrefName.PracticeAddress);
					billingAddress2=PrefC.GetString(PrefName.PracticeAddress2);
					billingCity=PrefC.GetString(PrefName.PracticeCity);
					billingState=PrefC.GetString(PrefName.PracticeST);
					billingZip=PrefC.GetString(PrefName.PracticeZip);
				}
				else {
					billingAddress1=clinic.Address;
					billingAddress2=clinic.Address2;
					billingCity=clinic.City;
					billingState=clinic.State;
					billingZip=clinic.Zip;
				}
				sw.Write("N3"+s+Sout(billingAddress1,55));//N301 1/55 Address Information:
				if(billingAddress2!="") {
					sw.Write(s+Sout(billingAddress2,55));//N302 1/55 Address Information:
				}
				EndSegment(sw);
				//2010AA N4: (medical,institutional,dental) Billing Provider City, State, Zip Code.
				sw.Write("N4"+s+Sout(billingCity,30)+s//N401 2/30 City Name: 
					+Sout(billingState,2,2)+s//N402 2/2 State or Province Code: 
					+Sout(billingZip.Replace("-",""),15));//N403 3/15 Postal Code: 
				EndSegment(sw);//NM404 through NM407 are either situational with United States as default, or not used, so we don't specify any of them.
				//2010AA REF: EI (medical,institutional,dental) Billing Provider Tax Identification.
				sw.Write("REF"+s);
				if(medType==EnumClaimMedType.Medical) {
					sw.Write((billProv.UsingTIN?"EI":"SY")+s);//REF01 2/3 Reference Identification Qualifier: EI=Employer's Identification Number (EIN). SY=Social Security Number (SSN).
				}
				else if(medType==EnumClaimMedType.Institutional) {
					sw.Write("EI"+s);//REF01 2/3 Reference Identification Qualifier: EI=Employer's Identification Number (EIN).
				}
				else if(medType==EnumClaimMedType.Dental) {
					sw.Write((billProv.UsingTIN?"EI":"SY")+s);//REF01 2/3 Reference Identification Qualifier: EI=Employer's Identification Number (EIN). SY=Social Security Number (SSN).
				}
				sw.Write(Sout(billProv.SSN,50));//REF02 1/50 Reference Identification. Tax ID #.
				EndSegment(sw);//REF03 and REF04 are not used.
				if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Dental) {
					//2010AA REF: (medical,dental) Billing Provider UIPN/License Information: Situational. We do not use. Max repeat 2.
				}
				if(medType==EnumClaimMedType.Dental) {
					//2010AA REF: (dental) State License Number: Required by RECS and Emdeon clearinghouses. We do NOT validate that it's entered because sending it with non-persons causes problems.
					//if(IsEmdeonDental(clearhouse) || clearhouse.CommBridge==EclaimsCommBridge.RECS) {
					if(!IsDentiCal(clearinghouseClin)) { //Denti-Cal never wants this.
						if(provClinicBill!=null && provClinicBill.StateLicense!="") {
							sw.Write("REF"+s
								+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number.
								+Sout(provClinicBill.StateLicense,50));//REF02 1/50 Reference Identification: 
							EndSegment(sw);//REF03 and REF04 are not used.
						}
					}
					//2010AA REF G5 (dental) Site Identification Number: NOT IN X12 5010 STANDARD DOCUMENTATION. Only required by Emdeon.
					if(IsEmdeonDental(clearinghouseClin)) {
						Write2010AASiteIDforEmdeon(sw,billProv,carrier.ElectID);
					}
				}
				//2010AA PER: IC (medical,institutional,dental) Billing Provider Contact Information: Probably required by a number of carriers and by Emdeon.
				sw.Write("PER"+s
					+"IC"+s//PER01 2/2 Contact Function Code: IC=Information Contact.
					+Sout(PrefC.GetString(PrefName.PracticeTitle),60)+s//PER02 1/60 Name: Practice Title.
					+"TE"+s);//PER03 2/2 Communication Number Qualifier: TE=Telephone.
				if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
					sw.Write(Sout(PrefC.GetString(PrefName.PracticeBillingPhone),256));//PER04  1/256 Communication Number: Telephone number.
				}
				else if(clinic==null){
					sw.Write(Sout(PrefC.GetString(PrefName.PracticePhone),256));//PER04  1/256 Communication Number: Telephone number.
				}
				else{
					sw.Write(Sout(clinic.Phone,256));//PER04  1/256 Communication Number: Telephone number.
				}
				EndSegment(sw);//PER05 through PER08 are situational and PER09 is not used. We do not use.
				if(clinic!=null && clinic.PayToAddress!="") {//A clinic Pay To address override is present.
					//2010AB NM1: 87 (medical,institutional,dental) Pay-To Address Name.
					sw.Write("NM1"+
					s+"87"+s);//NM101 2/3 Entity Identifier Code: 87=Pay-to Provider.
					if(medType==EnumClaimMedType.Institutional) {
						sw.Write("2");//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
					}
					else { //(medical,dental)
						sw.Write((billProv.IsNotPerson?"2":"1"));//NM102 1/1 Entity Type Qualifier: 1=Person, 2=Non-Person Entity.
					}
					EndSegment(sw);//NM103 through NM112 are not used.
					//2010AB N3: (medical,institutional,dental) Pay-To Address.
					sw.Write("N3"+s+Sout(clinic.PayToAddress),55);//N301 1/55 Address Information:
					if(clinic.PayToAddress2!="") {
						sw.Write(s+Sout(clinic.PayToAddress2,55));//N302 1/55 Address Information:
					}
					EndSegment(sw);
					//2010AB N4: (medical,institutional,dental) Pay-To Address City, State, Zip Code.
					sw.Write("N4"+s+Sout(clinic.PayToCity,30)+s//N401 2/30 City Name: 
					+Sout(clinic.PayToState,2,2)+s//N402 2/2 State or Province Code: 
					+Sout(clinic.PayToZip.Replace("-",""),15));//N403 3/15 Postal Code: 
					EndSegment(sw);//NM404 through NM407 are either situational with United States as default, or not used, so we don't specify any of them.
				}
				else { //Use the practice PayToAddress if clinic is null or the clinic's PayToAddress is blank.
					if(PrefC.GetString(PrefName.PracticePayToAddress)!="") {
						//2010AB NM1: 87 (medical,institutional,dental) Pay-To Address Name.
						sw.Write("NM1"+
						s+"87"+s);//NM101 2/3 Entity Identifier Code: 87=Pay-to Provider.
						if(medType==EnumClaimMedType.Institutional) {
							sw.Write("2");//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
						}
						else { //(medical,dental)
							sw.Write((billProv.IsNotPerson?"2":"1"));//NM102 1/1 Entity Type Qualifier: 1=Person, 2=Non-Person Entity.
						}
						EndSegment(sw);//NM103 through NM112 are not used.
						//2010AB N3: (medical,institutional,dental) Pay-To Address.
						sw.Write("N3"+s+Sout(PrefC.GetString(PrefName.PracticePayToAddress),55));//N301 1/55 Address Information:
						if(PrefC.GetString(PrefName.PracticePayToAddress2)!="") {
							sw.Write(s+Sout(PrefC.GetString(PrefName.PracticePayToAddress2),55));//N302 1/55 Address Information:
						}
						EndSegment(sw);
						//2010AB N4: (medical,institutional,dental) Pay-To Address City, State, Zip Code.
						sw.Write("N4"+s+Sout(PrefC.GetString(PrefName.PracticePayToCity),30)+s//N401 2/30 City Name: 
						+Sout(PrefC.GetString(PrefName.PracticePayToST),2,2)+s//N402 2/2 State or Province Code: 
						+Sout(PrefC.GetString(PrefName.PracticePayToZip).Replace("-",""),15));//N403 3/15 Postal Code: 
						EndSegment(sw);//NM404 through NM407 are either situational with United States as default, or not used, so we don't specify any of them.
					}
				}
				//2010AC NM1: 98 (medical,institutional,dental) Pay-To Plan Name. We do not use.
				//2010AC N3: (medical,institutional,dental) Pay-To Plan Address. We do not use.
				//2010AC N4: (medical,institutional,dental) Pay-To Plan City, State, Zip Code. We do not use.
				//2010AC REF: (medical,institutional,dental) Pay-To Plan Secondary Identification. We do not use.
				//2010AC REF: (medical,institutional,dental) Pay-To Plan Tax Identification Number. We do not use.
				parentProv=HLcount;
				HLcount++;
				#endregion Billing Provider
				#region Attachments
				/*if(IsTesia(clearhouse) && claim.Attachments.Count>0){//If Tesia and has attachments
					claim.ClaimNote="TESIA#"+claim.ClaimNum.ToString()+" "+claim.ClaimNote;
					string saveFolder=clearhouse.ExportPath;//we've already tested to make sure this exists.
					string saveFile;
					string storedFile;
					for(int c=0;c<claim.Attachments.Count;c++){
						saveFile=ODFileUtils.CombinePaths(saveFolder,
							claim.ClaimNum.ToString()+"_"+(c+1).ToString()+Path.GetExtension(claim.Attachments[c].DisplayedFileName)+".IMG");
						storedFile=ODFileUtils.CombinePaths(FormEmailMessageEdit.GetAttachPath(),claim.Attachments[c].ActualFileName);
						File.Clone(storedFile,saveFile,true);
					}					
				}*/
				#endregion
				#region Subscriber
				if(claim.PatRelat==Relat.Self){//if patient is the subscriber
					hasSubord="0";//-claim level will follow
					//subordinate patients will not follow in this loop.  The subscriber loop will be duplicated for them.
				}
				else {//patient is not the subscriber
					hasSubord="1";//-patient will always follow
				}
				//2000B HL: (medical,institutional,dental) Subscriber Hierarchical Level.
				sw.Write("HL"+s+HLcount.ToString()+s//HL01 1/12 Hierarchical ID Number:
					+parentProv.ToString()+s//HL02 1/12 Hierarchical Parent ID Number: parent HL is always the billing provider HL.
					+"22"+s//HL03 1/2 Hierarchical Level Code: 22=Subscriber.
					+hasSubord);//HL04 1/1 Hierarchical Child Code: 0=No subordinate HL segment in this hierarchical structure. 1=Additional subordinate HL data segment in this hierarchical structure.
				EndSegment(sw);
				//2000B SBR: (medical,institutional,dental) Subscriber Information.
				sw.Write("SBR"+s);
				bool claimIsPrimary=true;//we currently support only primary and secondary.  Plan2, if present, must be the opposite.
				if(claim.ClaimType=="PreAuth") {
					if(otherPlan!=null && PatPlans.GetOrdinal(claim.InsSubNum2,patPlans) < PatPlans.GetOrdinal(claim.InsSubNum,patPlans)) { //When there are two plans and the secondary plan is in the primary ins position.
						claimIsPrimary=false;
					}
				}
				else if(otherPlan!=null && (otherPlan.IsMedical || insPlan.IsMedical)) { //When there are two plans and at least one plan is medical.
					if(PatPlans.GetOrdinal(claim.InsSubNum2,patPlans) < PatPlans.GetOrdinal(claim.InsSubNum,patPlans)) { //The insurance plan order is determined by patplan.ordinal
						claimIsPrimary=false;
					}
				}
				else if(claim.ClaimType=="P") {
				}
				else if(claim.ClaimType=="S") { //Verification ensures that there are two plans associated with the claim in this case.
					claimIsPrimary=false;
				}
				else if(claim.ClaimType=="Other") { //Also used for a medical claim with only one insurance.
					//We always send medical plans as primary.  They are totally separate from the ordinals for dental.
				}
				else if(claim.ClaimType=="Cap") {
					//All captication claims are treated as primary claims.
					//todo: is secondary capitation possible?
				}
				sw.Write((claimIsPrimary?"P":"S")+s);//SBR01 1/1 Payer Responsibility Sequence Number Code: 
				string relationshipCode="";//empty if patient is not subscriber.
				if(claim.PatRelat==Relat.Self) {//if patient is the subscriber
					relationshipCode="18";
				}
				sw.Write(relationshipCode+s//SBR02 2/2 Individual Relationship Code: 18=Self (The only option besides blank).
					+Sout(insPlan.GroupNum,50)+s);//SBR03 1/50 Reference Identification: Does not need to be validated because group number is optional.
				//SBR04 1/60 Name: Situational. Required when SBR03 is not used. Does not need to be validated because group name is optional.
				if(insPlan.GroupNum!="") {
					sw.Write(s);
				} 
				else {
					sw.Write(Sout(insPlan.GroupName,60)+s);
				}
				sw.Write(s//SBR05 1/3 Insurance Type Code. Situational.  Skip because we don't support secondary Medicare.
					+s//SBR06 1/1 Coordination of Benefits Code: Not used.
					+s//SBR07 1/1 Yes/No Condition or Respose Code: Not used.
					+s//SBR08 2/2 Employment Status Code: Not used.
					+GetFilingCode(insPlan));//SBR09: 12=PPO,17=DMO,BL=BCBS,CI=CommercialIns,FI=FEP,HM=HMO. Will no longer be required when HIPPA National Plan ID is mandated.
				EndSegment(sw);
				if(medType==EnumClaimMedType.Medical) {
					//2000B PAT: (medical) Patient Information. Situational. Required when the patient is the subscriber or considered to be the subscriber and at least one of the element requirements are met. Element requirements include: when the patient is deceased and the date of death is available; or when the claim involves Medicare Durable Medical Equipment Regional Carriers Certificate of Medical Necessity (DMERC CMN) 02.03, 10.02, or DMA MAC 10.03; or when law requires to know if the patient is pregnant or not. We do not use, because we do not track death date, durable medical equipment information, nor do we know weather or not the patient is pregnant. Some of these fields may be necessary in the future, but not likely since our medical claims are usually pretty simple.
				}
				//2010BA NM1: IL (medical,institutional,dental) Subscriber Name.
				sw.Write("NM1"+s
					+"IL"+s//NM101 2/3 Entity Identifier Code: IL=Insured or Subscriber.
					+"1"+s//NM102 1/1 Entity Type Qualifier: 1=Person, 2=Non-Person Entity.
					+Sout(subscriber.LName,60)+s//NM103 1/60 Name Last or Organization Name: Never blank, because validated in the patient edit window when a patient is added/edited.
					+Sout(subscriber.FName,35)+s//NM104 1/35 Name First:
					+Sout(subscriber.MiddleI,25)+s//NM105 1/25 Name Middle:
					+s//NM106 1/10 Name Prefix: Not Used.
					+s//NM107 1/10 Name Suffix: Situational. Not present in Open Dental yet so we leave blank.
					+"MI"+s//NM108 1/2 Identification Code Qualifier: MI=Member Identification Number.
					+Sout(sub.SubscriberID.Replace("-",""),80,2));//NM109 2/80 Identification Code: Situational. Required when NM102=1.
				EndSegment(sw);//NM110 through NM112 are not used.
				//In 4010s, at the request of Emdeon, we always include N3,N4,and DMG even if patient is not subscriber.  This did not make the transaction non-compliant, and they found it useful.
				//In 5010s, we will follow the X12 specification for most clearinghouses and only include subsc address when subscriber=patient.
				//But for any clearinghouse who requests it, we will always include, [js 3/14/13 even when subscriber!=patient].  List them here:
				//Apex, ClaimConnect, EmdeonMed, EmdeonDent, LindsayTechnicalConsultants, OfficeAlly: Always include.
				//PostNTrack, BCBSIdaho: Only include when subscriber=patient.
				bool subscIncludeAddressAndGender=false;
				if(IsApex(clearinghouseClin) || IsClaimConnect(clearinghouseClin) || IsEmdeonDental(clearinghouseClin) || IsEmdeonMedical(clearinghouseClin) || IsLindsayTechnicalConsultants(clearinghouseClin) || IsOfficeAlly(clearinghouseClin) || IsEDS(clearinghouseClin)) {
					subscIncludeAddressAndGender=true;
				}
				else {//X12 standard behavior for everyone else, including: BCBSIdaho, ColoradoMedicaid, Denti-Cal, PostNTrack, WashingtonMedicaid.
					if(claim.PatRelat==Relat.Self) {//[js 3/14/13 only include when subscriber==patient]
						subscIncludeAddressAndGender=true;
					}
					//[js 3/14/13 when subscriber!=patient, don't include address]
				}
				if(subscIncludeAddressAndGender) {
					//2010BA N3: (medical,institutional,dental) Subscriber Address. Situational. Required when the patient is the subscriber.
					sw.Write("N3"+s+Sout(subscriber.Address,55));//N301 1/55 Address Information:
					if(subscriber.Address2!="") {
						sw.Write(s+Sout(subscriber.Address2,55));//N302 1/55 Address Information:
					}
					EndSegment(sw);
					//2010BA N4: (medical,institutional,dental) Subscriber City, State, Zip Code. Situational. Required when the patient is the subscriber.
					sw.Write("N4"+s
						+Sout(subscriber.City,30)+s//N401 2/30 City Name:
						+Sout(subscriber.State,2,2)+s//N402 2/2 State or Provice Code:
						+Sout(subscriber.Zip.Replace("-",""),15));//N403 3/15 Postal Code:
					EndSegment(sw);//N404 through N407 either not used or required for addresses outside of the United States.
					//2010BA DMG: (medical,institutional,dental) Subscriber Demographic Information. Situational. Required when the patient is the subscriber.
					//Carriers tend to complain if the BD is missing for the subscriber even though it's not strictly required.  So we will require it from users.
					sw.Write("DMG"+s
						+"D8"+s//DMG01 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
						+subscriber.Birthdate.ToString("yyyyMMdd")+s//DMG02 1/35 Date Time Period: Birthdate. The subscriber birtdate is validated.
						+GetGender(subscriber.Gender));//DMG03 1/1 Gender Code: F=Female, M=Male, U=Unknown.
					EndSegment(sw);
				}
				//2010BA REF: SY (medical,institutional,dental) Secondary Secondary Identification: Situational. Required when an additional identification number to that provided in NM109 of this loop is necessary. We do not use this.
				//2010BA REF: Y4 (medical,institutional,dental) Property and Casualty Claim Number: Required when the services included in this claim are to be considered as part of a property and casualty claim. We do not use this.
				//2010BA PER: IC (medical) Property and Casualty Subscriber Contact information: Situational. We do not use this.
				//2010BB NM1: PR (medical,institutional,dental) Payer Name.
				sw.Write("NM1"+s
					+"PR"+s//NM101 2/3 Entity Identifier Code: PR=Payer.
					+"2"+s);//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
				//NM103 1/60 Name Last or Organization Name:
				if(IsEMS(clearinghouseClin)) {
					//This is a special situation requested by EMS.  This tacks the employer onto the end of the carrier.
					sw.Write(Sout(carrier.CarrierName,30)+"|"+Sout(Employers.GetName(insPlan.EmployerNum),30)+s);
				}
				else if(IsDentiCal(clearinghouseClin)) {
					sw.Write("DENTICAL"+s);
				}
				else {
					sw.Write(Sout(carrier.CarrierName,60)+s);
				}
				sw.Write(s//NM104 1/35 Name First: Not used.
					+s//NM105 1/25 Name Middle: Not used.
					+s//NM106 1/10 Name Prefix: Not used.
					+s//NM107 1/10 Name Suffix: Not used.
					+"PI"+s);//NM108 1/2 Identification Code Qualifier: PI=Payor Identification.
				sw.Write(Sout(GetCarrierElectID(carrier,clearinghouseClin),80,2));//NM109 2/80 Identification Code: PayorID.
				EndSegment(sw);//NM110 through NM112 Not Used.
				//2010BB N3: (medical,institutional,dental) Payer Address.
				sw.Write("N3"+s+Sout(carrier.Address,55));//N301 1/55 Address Information:
				if(carrier.Address2!="") {
					sw.Write(s+Sout(carrier.Address2,55));//N302 1/55 Address Information: Required when there is a second address line.
				}
				EndSegment(sw);
				//2010BB N4: (medical,institutional,dental) Payer City, State, Zip Code.
				sw.Write("N4"+s
					+Sout(carrier.City,30)+s//N401 2/30 City Name:
					+Sout(carrier.State,2)+s//N402 2/2 State or Province Code:
					+Sout(carrier.Zip.Replace("-",""),15));//N403 3/15 Postal Code:
				EndSegment(sw);//N404 through N407 are either not used or are for addresses outside of the United States.
				//2010BB REF 2U,EI,FY,NF (dental) Payer Secondary Identificaiton. Situational.
				//2010BB REF G2,LU Billing Provider Secondary Identification. Situational. Not required because we always send NPI.
				if(!IsDentiCal(clearinghouseClin)){//DentiCal complained that they don't usually want this (except for non-subparted NPIs, which we don't handle).  So far, nobody else has complained.
					//Always required by Emdeon Dental.
					WriteProv_REFG2orLU(sw,billProv,carrier.ElectID);
				}
				parentSubsc=HLcount;
				HLcount++;
				#endregion
				#region Patient
				if(claim.PatRelat!=Relat.Self){//if patient is not the subscriber
					//2000C HL: (medical,institutional,dental) Patient Hierarchical Level.
					sw.Write("HL"+s+HLcount.ToString()+s//HL01 1/12 Hierarchical ID Number:
						+parentSubsc.ToString()+s//HL02 1/12 Hierarchical Parent ID Number: Parent is always the subscriber HL.
						+"23"+s//HL03 1/2 Hierarchical Level Code: 23=Dependent.
						+"0");//HL04 1/1 Hierarchical Child Code: 0=No subordinate HL segment in this hierarchical structure.
					EndSegment(sw);
					//2000C PAT: (medical,institutional,dental) Patient Information.
					if(IsEmdeonDental(clearinghouseClin)) {
						sw.Write("PAT"+s
							+GetRelat(claim.PatRelat)+s//PAT01 2/2 Individual Relationship Code:
							+s//PAT02 1/1 Patient Location Code: Not used.
							+s//PAT03 2/2 Employment Status Code: Not used.
							+GetStudentEmdeon(patient.StudentStatus));//PAT04 1/1 Student Status Code: Not used. Emdeon wants us to sent this code corresponding to version 4010, even through it is not standard X12.
						EndSegment(sw);//PAT05 through PAT08 not used in institutional or dental, but is sometimes used in medical. We do not use.
					}
					else {
						sw.Write("PAT"+s
							+GetRelat(claim.PatRelat));//PAT01 2/2 Individual Relationship Code:
						EndSegment(sw);//PAT02 through PAT04 Not used. PAT05 through PAT08 not used in institutional or dental, but is sometimes used in medical. We do not use.
					}
					//2010CA NM1: QC (medical,institutional,dental) Patient Name.
					sw.Write("NM1"+s
						+"QC"+s//NM101 2/3 Entity Identifier Code: QC=Patient.
						+"1"+s//NM102 1/1 Entity Type Qualifier: 1=Person.
						+Sout(patient.LName,60)+s//NM103 1/60 Name Last or Organization Name:
						+Sout(patient.FName,35));//NM104 1/35 Name First: Never blank, because validated in the patient edit window when a patient is added/edited.
						if(patient.MiddleI!="") {
							sw.Write(s+Sout(patient.MiddleI,25));//NM105 1/25 Name Middle
						}
					EndSegment(sw);
					//NM106 not used. NM107, No suffix field in Open Dental. NM108 through NM112 not used.
					//2010CA N3: (medical,institutional,dental) Patient Address.
					sw.Write("N3"+s+
						Sout(patient.Address,55));//N301 1/55 Address Information
					if(patient.Address2!="") {
						sw.Write(s+Sout(patient.Address2,55));//N302 1/55 Address Information:
					}
					EndSegment(sw);
					//2010CA N4: (medical,institutional,dental) Patient City, State, Zip Code.
					sw.Write("N4"+s
						+Sout(patient.City,30)+s//N401 2/30 City Name:
						+Sout(patient.State,2,2)+s//N402 2/2 State or Provice Code: 
						+Sout(patient.Zip.Replace("-",""),15));//N403 3/15 Postal Code: 
					EndSegment(sw);//N404 through N407 are either not used or only required for addresses outside the United States.
					//2010CA DMG: (medical,institutional,dental) Patient Demographic Information.
					sw.Write("DMG"+s
						+"D8"+s//DMG01 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
						+patient.Birthdate.ToString("yyyyMMdd")+s//DMG02 1/35 Date Time Period:
						+GetGender(patient.Gender));//DMG03 1/1 Gender Code: F=Female,M=Male,U=Unknown.
					EndSegment(sw);//DMG04 through DMG11 are not used.
					//2010CA REF: (medical,instituional,dental) Property and Casualty Claim Number. Situational. We do not use this.
					//2010CA REF: (medical,institutional) Property and Casualty Patient Identifier. Situational.  We do not use.
					//2010CA PER: (medical) Property and Casualty Patient Contact Information. Situational. We do not use.
					HLcount++;
				}
				#endregion
				#region Claim CLM
				//2300 CLM: (medical,institutional,dental) Claim Information.
				string clm01=claim.ClaimIdentifier;//Typically PatNum/ClaimNum. Check for uniqueness is performed in UI.
				if(IsDentiCal(clearinghouseClin)) {
					clm01=Sout(clm01,17);//Denti-Cal has a maximum of 17 chars here. This field is what Denti-Cal refers to as the PDCN.
				}
				else if(IsEmdeonMedical(clearinghouseClin)) {
					clm01=clm01.Replace('/','-');//Emdeon Medical only allows letters, numbers, dashes, periods, spaces and asterisks. The claim identifier will contain / inside the claim edit window still.
				}
				string claimFrequencyTypeCode="1";
				if(claim.CorrectionType==ClaimCorrectionType.Original) {
					claimFrequencyTypeCode="1";
				}
				else if(claim.CorrectionType==ClaimCorrectionType.Replacement) {
					claimFrequencyTypeCode="7";
				}
				else if(claim.CorrectionType==ClaimCorrectionType.Void) {
					claimFrequencyTypeCode="8";
				}
				//the next 8 lines fix a rare bug where the total doesn't match the sum of the procs.  This would result in invalid X12
				decimal claimFeeBilled=0;
				for(int j=0;j<claimProcs.Count;j++) {
					claimFeeBilled+=(decimal)claimProcs[j].FeeBilled;
				}
				if(claimFeeBilled!=(decimal)claim.ClaimFee) {
					claim.ClaimFee=(double)claimFeeBilled;
				  Claims.Update(claim);
				}
				sw.Write("CLM"+s
					+Sout(clm01,20)+s//CLM01 1/38 Claim Submitter's Identifier: A unique id. Carriers are not required to handle more than 20 char. 
					+claimFeeBilled.ToString("f2")+s//CLM02 1/18 Monetary Amount:
					+s//CLM03 1/2 Claim Filing Indicator Code: Not used.
					+s);//CLM04 1/2 Non-Institutional Claim Type Code: Not used.
				//CLM05 (medical,institutional,dental) Health Care Services Location Information.
				if(medType==EnumClaimMedType.Medical) {
					sw.Write(GetPlaceService(claim.PlaceService)+isa16//CLM05-1 1/2  Facility Code Value: Place of Service.
						+"B"+isa16//CLM05-2 1/2 Facility Code Qualifier, B=Place of Service Codes.
						+claimFrequencyTypeCode+s);//CLM05-3 1/1 Claim Frequency Type Code: 1=original, 7=replacement, 8=void(in limited jurisdictions).
				}
				else if(medType==EnumClaimMedType.Institutional) {
					//claim.UniformBillType validated to be exactly 3 char
					//Example: 771: 7=clinic, 7=FQHC, 1=Only claim.  713: 7=clinic, 1=rural health clinic, 3=continuing claim.
					sw.Write(claim.UniformBillType.Substring(0,2)+isa16//CLM05-1 1/2  Facility Code Value: First and second position of UniformBillType.
						+"A"+isa16//CLM05-2 1/2 Facility Code Qualifier, A=Uniform Billing Claim Form Bill Type.
						+claim.UniformBillType.Substring(2,1)+s);//CLM05-3 1/1 Claim Frequency Type Code: Third position of UniformBillType.
				}
				else{//dental.
					sw.Write(GetPlaceService(claim.PlaceService)+isa16//CLM05-1 1/2  Facility Code Value: Place of Service.
						+"B"+isa16//CLM05-2 1/2 Facility Code Qualifier, B=Place of Service Codes.
						+claimFrequencyTypeCode+s);//CLM05-3 1/1 Claim Frequency Type Code: 1=original, 7=replacement, 8=void(in limited jurisdictions).
				}
				if(medType==EnumClaimMedType.Medical) {
					sw.Write("Y"+s);//CLM06 1/1 Yes/No Condition or Response Code: prov sig on file (always yes)
				}
				else if(medType==EnumClaimMedType.Institutional) {
					sw.Write(s);//CLM06 1/1 Yes/No Condition or Response Code: Not used.
				}
				else if(medType==EnumClaimMedType.Dental) {
					sw.Write("Y"+s);//CLM06 1/1 Yes/No Condition or Response Code: prov sig on file (always yes)
				}
				sw.Write("A"+s//CLM07 1/1 Provider Accept Assignment Code: Prov accepts medicaid assignment. OD has no field for this, so no choice.
					+(Claims.GetAssignmentOfBenefits(claim,sub)?"Y":"N")+s//CLM08 1/1 Yes/No Condition or Response Code: We do not support W.
					+(sub.ReleaseInfo?"Y":"I"));//CLM09 1/1 Release of Information Code: Y or I(which is equivalent to No)
				if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Dental) {
					if(claim.AccidentDate.Year>1880 || claim.SpecialProgramCode!=EnumClaimSpecialProgram.none || claim.ClaimType=="PreAuth") {//if val for 11,12, or 19
						sw.Write(s+s//end of CLM09. CLM10 not used
							+GetRelatedCauses(claim));//CLM11 2/3:2/3:2/3:2/2:2/3 Related Causes Information: Situational. Accident related, including state. Might be blank.
					}
					if(claim.SpecialProgramCode!=EnumClaimSpecialProgram.none || claim.ClaimType=="PreAuth") {//if val for 12, or 19
						sw.Write(s//nothing for CLM11.
							+GetSpecialProgramCode(claim.SpecialProgramCode));//CLM12 2/3 Special Program Code: Situational. Example EPSTD.
					}
					if(claim.ClaimType=="PreAuth") {//if val for 19
						sw.Write(s//nothing for CLM12.
							+s+s+s+s+s+s//CLM13-18 not used
							+"PB");//CLM19 2/2 Claim Submission Reason Code: PB=Predetermination of Benefits. TODO: Not allowed in medical claims. What is the replacement??
						//CLM20 1/2 Delay Reason Code: Situational. Required when claim is submitted late. Not supported.
					}
				}
				//CLM10-19 not used, 20 not supported for institutional.
				EndSegment(sw);
				#endregion Claim CLM
				#region Claim DTP
				if(medType==EnumClaimMedType.Medical) {
					//2300 DTP: 431 (medical) Date Onset of Current Illness or Symptom. Situational. We do not use.
					Write2300DTP_Onset(sw,claim,"431");
					//2300 DTP: 454 (medical) Initial Treatment Date. Situational. We do not use. (spinal manipulation).
					Write2300DTP_Other(sw,claim,"454");
					//2300 DTP: 304 (medical) Date Last Seen. Situational. We do not use. (foot care)
					Write2300DTP_Other(sw,claim,"304");
					//2300 DTP: 453 (medical) Date Accute Manifestation. Situational. We do not use. (spinal manipulation)
					Write2300DTP_Other(sw,claim,"453");
					//2300 DTP: 439 (medical) Date Accident. Situational.
					if(claim.AccidentDate.Year>1880) {
						sw.Write("DTP"+s
							+"439"+s//DTP01 3/3 Date/Time Qualifier: 439=accident
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifer: D8=Date Expressed in Format CCYYMMDD.
							+claim.AccidentDate.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					else {
						Write2300DTP_Other(sw,claim,"439");
					}
					//2300 DTP: 484 (medical) Last Menstrual Period Date. Situational. We do not use.
					Write2300DTP_Onset(sw,claim,"484");
					//2300 DTP: 455 (medical) Last X-ray Date. Situational. We do not use.
					Write2300DTP_Other(sw,claim,"455");
					//2300 DTP: 471 (medical) Hearing and Vision Prescription Date. Situational. We do not use.
					Write2300DTP_Other(sw,claim,"471");
					//2300 DTP: 314,360,361 (medical) Disability Dates. Situational. We do not use.
					//2300 DTP: 297 (medical) Date Last Worked. Situational. We do not use.
					//2300 DTP: 296 (medical) Date Authorized Return to Work. Situational. We do not use.
					//2300 DTP: 435 (medical) Date Admission. Situational. We do not use. Inpatient. Request #2843.
					//2300 DTP: 096 (medical) Date Discharge. Situational. We do not use. Inpatient. Request #2843.
					//2300 DTP: 090 (medical) Date Assumed and Relinquished Care Dates. Situational. We do not use.
					Write2300DTP_Other(sw,claim,"090");
					Write2300DTP_Other(sw,claim,"091");
					//2300 DTP: 444 (medical) Date Property and Casualty Date of First Contact. Situational. We do not use.
					Write2300DTP_Other(sw,claim,"444");
					//2300 DTP: 050 (medical) Repricer Received Date. Situational. We do not use.
				}
				else if(medType==EnumClaimMedType.Institutional) {
					//2300 DTP 096 (institutional) Discharge Hour. Situational. We do not use. Inpatient. 
					//2300 DTP 434 (instititional) Statement Dates.
//todo:how to handle preauths?
					if(claim.DateService.Year>1880) {//DateService validated
						sw.Write("DTP"+s
							+"434"+s//DTP01 3/3 Date/Time Qualifier: 434=Statement.
							+"RD8"+s//DTP02 2/3 Date Time Period Format Qualifier: RD8=Range of Dates Expressed in Format CCYYMMDD-CCYYMMDD.
							+claim.DateService.ToString("yyyyMMdd")+"-"+claim.DateService.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2300 DTP 435 (institutional) Admission Date/Hour. Situational. We do not use. Inpatient.
					//For the UB04 we are using claim.DateService for this field.
					//2300 DTP 050 (institutional) Repricer Received Date. Situational. Not supported.
				}
				else if(medType==EnumClaimMedType.Dental) {
					//2300 DTP 439 (dental) Date accident. Situational. Required when there was an accident.
					if(claim.AccidentDate.Year>1880) {
						sw.Write("DTP"+s
							+"439"+s//DTP01 3/3 Date/Time Qualifier: 439=accident
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifer: D8=Date Expressed in Format CCYYMMDD.
							+claim.AccidentDate.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2300 DTP 452 (dental) Date Appliance Placement. Situational. Values can be overriden in loop 2400 for individual service items, but we don't support that.
					if(claim.OrthoDate.Year>1880) {
						sw.Write("DTP"+s
							+"452"+s//DTP01 3/3 Date/Time Qualifier: 452=Appliance Placement.
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
							+claim.OrthoDate.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2300 DTP 472 (dental) Service Date. Not used if predeterm.
					if(claim.ClaimType!="PreAuth") {
						if(claim.DateService.Year>1880) {
							sw.Write("DTP"+s
								+"472"+s//DTP01 3/3 Date/Time Qualifier: 472=Service.
								+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
								+claim.DateService.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
							EndSegment(sw);
						}
					}
					//2300 DTP 050 (dental) Repricer Received Date.  Not supported.
				}
				#endregion Claim DTP
				#region Claim DN CL1
				if(medType==EnumClaimMedType.Dental) {
					//2300 DN1: Orthodontic Total Months of Treatment.
					if(claim.IsOrtho) {
						sw.Write("DN1"+s
							+((claim.OrthoTotalM>0)?claim.OrthoTotalM.ToString():"")+s//DN101 1/15 Quantity: Orthodontic Treatment Months Count.  Estimated number of treatment months.
							+claim.OrthoRemainM.ToString());//DN102 1/15 Quantity: Number of treatment months remaining.
						EndSegment(sw);//DN103 is not used and DN104 is situational but we do not use it.
					}
					//2300 DN2: Tooth Status. Missing teeth.
					List<string> missingTeeth=ToothInitials.GetMissingOrHiddenTeeth(initialList);
					bool doSkip;
					int countMissing=0;
					for(int j=0;j<missingTeeth.Count;j++) {
						//if the missing tooth is missing because of an extraction being billed here, then exclude it
						//still needed, even though missing teeth are not based on procedures any longer
						doSkip=false;
						for(int p=0;p<claimProcs.Count;p++) {
							proc=Procedures.GetProcFromList(procList,claimProcs[p].ProcNum);
							procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
							if(procCode.PaintType==ToothPaintingType.Extraction && proc.ToothNum==missingTeeth[j]) {
								doSkip=true;
								break;
							}
						}
						if(doSkip) {
							continue;
						}
						countMissing++;
						if(countMissing>35) {//segment max use 35
							continue;
						}
						sw.Write("DN2"+s
							+missingTeeth[j]+s//DN201 1/50 Reference Identification: Tooth number.
							+"M"+s//DN202: M=Missing, I=Impacted, E=To be extracted.
							+s//DN203 1/15 Quantity: Not used.
							+s//DN204 2/3 Date Time Period Format Qualifier: Not used.
							+s//DN205 1/35 Date Time Period: Not used.
							+"JP");//DN206 1/3 Code List Qualifier Code: Required. JP=JP Universal National Tooth Designation System.
						EndSegment(sw);
					}
				}
				if(medType==EnumClaimMedType.Institutional) {
					//2300 CL1: Institutional Claim Code. Required
					sw.Write("CL1"+s
						+claim.AdmissionTypeCode+s//CL101 1/1 Admission Type Code: Validated.
						+claim.AdmissionSourceCode+s//CL102 1/1 Admission Source Code: Required for inpatient services. Validated.
						+claim.PatientStatusCode);//CL103 1/2 Patient Status Code: Validated.
					EndSegment(sw);//CL104 1/1 Nursing Home Residential Status Code: Not used.
				}
				#endregion Claim DN CL1
				#region Claim PWK
				//2300 PWK: (medical,institutional,dental) Claim Supplemental Information. Paperwork. Used to identify attachments.
				/*if(IsTesia(clearhouse) && claim.Attachments.Count>0) {//If Tesia and has attachments
					sw.Write("PWK"+s
						+"OZ"+s//PWK01: ReportTypeCode. OZ=Support data for claim.
						+"EL"+s//PWK02: Report Transmission Code. EL=Electronic
						+s+s//PWK03 and 04: not used
						+"AC"+s//PWK05: Identification Code Qualifier. AC=Attachment Control Number
						+"TES"+claim.ClaimNum.ToString());//PWK06: Identification Code.
				 	EndSegment(sw);
				}*/
				//If Attachment ID # is present but no attachment flag, then sending is blocked.
				//No other validation is done.  However, warnings are displayed if:
				//Warning if attachments are listed as Mail even though we are sending electronically.
				//Warning if any PWK segments are needed, and there is no ID code.
				//PWK can repeat max 10 times.  However, we only write one PWK segment, because the user can only enter one Attachment Control Number.
				string pwk01="  ";//This double blank will never actually get sent because of the trim and if statement around the PWK creation.
				if(claim.AttachedFlags.Contains("EoB")) {
					pwk01="EB";//Explaination of Benefits
				}
				if(claim.AttachedFlags.Contains("Perio")) {
					if(medType==EnumClaimMedType.Dental) {
						pwk01="P6";//Periodontal Charts
					}
					else {//Medical and institutional
						pwk01="OZ";//Support Data for Claim. There is no "P6" nor a code for perio charts.
					}
				}
				if(claim.AttachedFlags.Contains("Misc") || claim.AttachedFlags.Contains("Note") || claim.AttachedImages>0) {
					pwk01="OZ";//Support Data for Claim
				}
				if(claim.Radiographs>0) {
					pwk01="RB";//Radiology Films
				}
				if(claim.AttachedModels>0) {
					pwk01="DA";//Dental Models
				}
				if(claim.AttachmentID!="" && pwk01.Trim()=="") {
					pwk01="OZ";
				}
				string pwk02="  ";
				if(IsDentiCal(clearinghouseClin)) {
					pwk02="FT";//"File Transfer". Might be electronic or mail, but Denti-Cal requires a value of FT here.
				}
				else {
					if(claim.AttachedFlags.Contains("Mail")) {
						pwk02="BM";//By Mail
					}
					else {
						pwk02="EL";//Elect
					}
				}
				string pwk06=claim.AttachmentID;
				if(pwk02=="BM" && pwk06=="") {//If By Mail is checked, they will typically leave attachment ID blank, but X12 requires a value, so we make one up.
					pwk06="00";
				}
				pwk06=Sout(pwk06,80,2);
				if(pwk01.Trim()!="" && pwk06!="") {
					sw.Write("PWK"+s
						+pwk01+s//PWK01 2/2 Report Type Code:
						+pwk02+s//PWK02 1/2 Report Transmission Code: EL=Electronically Only, BM=By Mail.
						+s+s//PWK03 and PWK04: Not Used.
						+"AC"+s//PWK05 1/2 Identification Code Qualifier: AC=Attachment Control Number.
						+pwk06);//PWK06 2/80 Identification Code:
					EndSegment(sw);//PWK07 through PWK09 are not used.
				}
				#endregion Claim PWK
				#region Claim CN1 AMT
				//2300 CN1: (medical,institutional,dental)Contract Information. Situational. We do not use this.
				//2300 AMT: (institutional) Patient Estimated Amount Due.
				//2300 AMT: (medical,dental) Patient Amount Paid.  Sum of all amounts paid specifically to this claim by the patient or family Situational.
				if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Dental) {
					if(claim.ShareOfCost > 0) {
						sw.Write("AMT"+s
						  +"F5"+s//AMT01 1/3 Amount Qualifier Code: F5=Patient Paid Amount.
						  +claim.ShareOfCost.ToString("F2"));//AMT02 1/18 Monetary Amount:
						EndSegment(sw);//AMT03 is not used.
					}
				}
				#endregion Claim CN1 AMT
				#region Claim REF
				if(medType==EnumClaimMedType.Dental) {
					//2300 REF: G3 (dental) Predetermination Identification. Situational.  Required when sending claim for previously predetermined services. Do not send prior authorization number here.
					if(claim.PreAuthString!="") {//validated to be empty for medical and inst
						sw.Write("REF"+s
							+"G3"+s//REF01 2/3 G3=Predetermination of Benefits Identification Number.
							+Sout(claim.PreAuthString,50));//REF02 1/50 Predeterm of Benfits Identifier.
						EndSegment(sw);//REF03 and REF04 are not used.
					}
				}
				//2300 REF: 4N (medical,institutional,dental) Service Authorization Exception Code. Situational. Required if services were performed without authorization.
//todo: ServiceAuthException
				//2300 REF: F5 (medical) Mandatory Medicare (Section 4081) Crossover Indicator. Situational. Required when submitter is Medicare and the claim is a Medigap or COB crossover claim. We do not use.
				//2300 REF: EW (medical) Mammography Certification Number. Situational. We do not use.
				//2300 REF: F8 (medical,institutional,dental) Payer Claim Control Number: Situational. Required if this is a replacement or void claim. Might not be required for corrected, but we require anyway since it is required in 4010.
					//aka Original Document Control Number/Internal Control Number (DCN/ICN).
					//aka Transaction Control Number (TCN).  
					//aka Claim Reference Number. 
					//Seems to be required by Medicaid when voiding a claim or resubmitting a claim by setting the CLM05-3.
				if(claim.CorrectionType!=ClaimCorrectionType.Original 
					|| claim.UniformBillType.Length>2 && (claim.UniformBillType.Substring(2,1)=="6" || claim.UniformBillType.Substring(2,1)=="7" || claim.UniformBillType.Substring(2,1)=="8")) { //correction, replacement or void.
					sw.Write("REF"+s
						+"F8"+s//REF01 2/3 Reference Identification Qualifier: F8=Original Reference Number.
						+Sout(claim.OrigRefNum,50));//REF02 1/50 Reference Identification: Payer Claim Control Number.
					EndSegment(sw);//REF03 and REF04 are not used.
				}
				//2300 REF: 9F (medical,institutional,dental) Referral Number. Situational. Required when a referral number is assigned by the payer or Utilization Management Organization (UMO) AND a referral is involved.
				if(claim.RefNumString!="") {
					sw.Write("REF"+s
						+"9F"+s//REF01 2/3 Reference Identification Qualifier: 9F=Referral Number.
						+Sout(claim.RefNumString,30));//REF02 1/50 Reference Identification:
					EndSegment(sw);//REF03 and REF04 are not used.
				}
				//2300 REF: X4 (medical) Clinical Laboratory Improvement Amendment (CLIA) Number. Situational. We do not use.
				//2300 REF: G1 (medical,institutional,dental) Prior Authorization. Situational. Do not report predetermination of benefits id number here. G1 and G3 were muddled in 4010.  
				if(claim.PriorAuthorizationNumber!="") {
					sw.Write("REF"+s
						+"G1"+s//REF01 2/3 Reference Identification Qualifier: G1=Prior Authorization Number.
						+Sout(claim.PriorAuthorizationNumber,50));//REF02 1/50 Reference Identification: Prior Authorization Number.
					EndSegment(sw);//REF03 and REF04 are not used.
				}					
				//2300 REF: 9A (medical,institutional,dental) Repriced Claim Number. Situational. We do not use. 
				//2300 REF: 9C (medical,institutional,dental) Adjusted Repriced Claim Number. Situational. We do not use.
				//2300 REF: D9 (medical,institutional,dental) Claim Identifier For Transmission Intermediaries. Situational.
				if(IsClaimConnect(clearinghouseClin)) { //Since this information has only been requested by ClaimConnect and is optional in the specification, we should only add specific clearinghouses here when requested.
					sw.Write("REF"+s
						+"D9"+s//REF01 2/3 Reference Identification Qualifier: D9=Claim Number.
						+Sout(claim.ClaimIdentifier,20));//REF02 1/50 Reference Identification: Value Added Network Trace Number. From specification, maximum of 20 characters even though there is space for 50.
					EndSegment(sw);//REF03 and REF04 are not used.
				}
				//2300 REF: LX (medical,institutional) Investigational Device Exemption Number. Situational. Required for FDA IDE.
				//2300 REF: LU (institutional) Auto Accident State. Situational. Seems to me to be a duplicate of the info in CLM11.
				//2300 REF: EA (medical,institutional) Medical Record Number. Situational. We do not use.
				//2300 REF: P4 (medical,institutional) Demonstration Project Identifier. Situational. We do not use. Seems very unimportant.
				//2300 REF: G4 (institutional) Peer Review Organization (PRO) Approval Number. Situational. We do not use.
				//2300 REF: 1J (medical) Care Plan Oversight. Situational. We do not use.
				#endregion Claim REF
				#region Claim K3 NTE CRx
				//2300 K3: File info (medical,institutional,dental). Situational. We do not use this.
				//NTE loops------------------------------------------------------------------------------------------------------
				//2300 NTE: (medical,institutional,dental) Claim Note. Situational. A number of NTE01 codes other than 'ADD', which we don't support.
				string note="";
				if(claim.AttachmentID!="" && !claim.ClaimNote.StartsWith(claim.AttachmentID)) {
					//The AttachmentID is sent in the PWK segment.  There is no longer a need to send this information in the NTE segment anymore.
					//Denti-Cal and ClaimConnect have asked us to remove the AttachmentID from the NTE segment, since the segment is very short.
					if(!IsDentiCal(clearinghouseClin) && !IsClaimConnect(clearinghouseClin)) {
						note=claim.AttachmentID+" ";
					}
				}
				note+=claim.ClaimNote;
				int maxNoteLength=0;
				if(medType==EnumClaimMedType.Medical) {
					maxNoteLength=80;//Segment repeat only 1 time for a total of 1*80=80 chars.
				}
				else if(medType==EnumClaimMedType.Institutional) {
					maxNoteLength=800;//Segment repeats up to 10 times for a total of 10*80=800 chars.  The UI only allows 400 characters.
				}
				else if(medType==EnumClaimMedType.Dental) {
					maxNoteLength=400;//Segment repeats up to 5 times for a total of 5*80=400 chars.
				}
				for(int j=0;j<Math.Min(maxNoteLength,note.Length);j+=80) {
					sw.Write("NTE"+s
						+"ADD"+s//NTE01 3/3 Note Reference Code: ADD=Additional information.
						+Sout(note.Substring(j),80));//NTE02 1/80 Description:
					EndSegment(sw);
				}
				//2300 NTE: (institutional) Billing Note. Situational. We do not use.
				//CRx loops------------------------------------------------------------------------------------------------------
				//2300 CR1: LB (medical) Ambulance Transport Information. Situational. We do not use.
				//2300 CR2: (medical) Spinal Manipulation Service Information. Situational. We do not use.
				//2300 CRC: (medical) Ambulance Certification. Situational. We do not use.
				//2300 CRC: (medical) Patient Condition Information Vision. Situational. We do not use.
				//2300 CRC: (medcial) Homebound Indicator. Situational. We do not use.
				//2300 CRC: (medical,institutional) EPSDT Referral. Situational. Required on EPSDT claims when the screening service is being billed in this claim. We do not use.
				#endregion Claim K3 NTE CRx
				#region Claim HI HCP
				//HI loops-------------------------------------------------------------------------------------------------------
				List<byte> listDiagnosesVersions=new List<byte>();
				List<string> listDiagnoses=Procedures.GetUniqueDiagnosticCodes(Procedures.GetProcsFromClaimProcs(claimProcs),false,listDiagnosesVersions);
				//2300 HI: BK (medical,institutional,dental) Health Care Diagnosis Code. Situational.  
				//todo: validate that diagnoses are actual ICD9 or ICD10 codes.
				if(medType==EnumClaimMedType.Institutional && listDiagnoses.Count>0 && listDiagnoses[0]!="") {//Ensure at least one diagnosis.
					sw.Write("HI"+s
						+((listDiagnosesVersions[0]==9)?"BK":"ABK")+isa16//HI01-1 1/3 Code List Qualifier Code: BK=ICD-9,ABK=ICD-10 Principal Diagnosis
						+Sout(listDiagnoses[0].Replace(".",""),30));//HI01-2 1/30 Industry Code: Diagnosis code. No periods.
					EndSegment(sw);
					//Institutional claims only support 1 diagnosis code here, the principal diagnosis.
				}
				else if(medType==EnumClaimMedType.Medical) {//Validation guarantees there is at least one (principal) diagnosis code as required
					sw.Write("HI"+s
						+((listDiagnosesVersions[0]==9)?"BK":"ABK")+isa16//HI01-1 1/3 Code List Qualifier Code: BK=ICD-9,ABK=ICD-10 Principal Diagnosis
						+Sout(listDiagnoses[0].Replace(".",""),30));//HI01-2 1/30 Industry Code: Diagnosis code. No periods.
					for(int j=1;j<listDiagnoses.Count;j++) {//Validated below that there are 12 or fewer common diagnoses for the entire claim.
						if(listDiagnoses[j]=="") {
							continue;
						}
						sw.Write(s//this is the separator from the _previous_ field.
							+((listDiagnosesVersions[j]==9)?"BF":"ABF")+isa16//HI0#-1 1/3 Code List Qualifier Code: BF=ICD-9,ABF=ICD-10 Diagnosis
							+Sout(listDiagnoses[j].Replace(".",""),30));//HI0#-2 1/30 Industry Code: Diagnosis code. No periods.
					}
					EndSegment(sw);
				}
				else if(medType==EnumClaimMedType.Dental && listDiagnoses.Count>0 && listDiagnoses[0]!="") {//Ensure at least one diagnosis.
					sw.Write("HI"+s
						+((listDiagnosesVersions[0]==9)?"BK":"ABK")+isa16//HI01-1 1/3 Code List Qualifier Code: BK=ICD-9,ABK=ICD-10 Principal Diagnosis
						+Sout(listDiagnoses[0].Replace(".",""),30));//HI01-2 1/30 Industry Code: Diagnosis code. No periods.
					for(int j=1;j<listDiagnoses.Count;j++) {//We allow up to 12 diagnosis codes per claim (for medical), but the dental format only allows up to 4 (see validation).
						if(listDiagnoses[j]=="") {
							continue;
						}
						sw.Write(s//this is the separator from the _previous_ field.
							+((listDiagnosesVersions[j]==9)?"BF":"ABF")+isa16//HI0#-1 1/3 Code List Qualifier Code: BF=ICD-9,ABF=ICD-10 Diagnosis
							+Sout(listDiagnoses[j].Replace(".",""),30));//HI0#-2 1/30 Industry Code: Diagnosis code. No periods.
					}
					EndSegment(sw);
				}
				//2300 HI: BP (medical) Anesthesia Related Procedure. Situational. We do not use.
				//2300 HI: BJ (institutional) Admitting Diagnosis. Situational. Required for inpatient admission. We do not use.
				//2300 HI: PR (institutional) Patient's Reason for Visit. Situational. Required for outpatient visits.
				if(medType==EnumClaimMedType.Institutional && listDiagnoses.Count>0 && listDiagnoses[0]!="") {//Ensure at least one (principal) diagnosis exists.
					sw.Write("HI"+s
						+((listDiagnosesVersions[0]==9)?"PR":"APR")+isa16//HI01-1 1/3 Code List Qualifier Code: PR=ICD-9,APR=ICD-10 Patient's Reason for Visit.
						+Sout(listDiagnoses[0].Replace(".",""),30));//HI01-2 1/30 Industry Code: No periods. This is not really principal diagnosis but is close to the same, so someday we will add this field to claim.
					EndSegment(sw);
				}
				//2300 HI: BN (institutional) External Cause of Injury. Situational. We do not use.
				//2300 HI: (institutional) Diagnosis Related Group (DRG) Information. Situational. We do not use. For inpatient hospital under DRG contract.
				//2300 HI: BF (institutional) Other Diagnosis Information. Situational. We do not use. When other conditions coexist or develop.
				//2300 HI: BR (institutional) Principal Procedure Information. Situational. We do not use. Required on inpatient claims when a procedure was performed.
				//2300 HI: BQ (institutional) Other Procedure Information. Situational. We do not use. Inpatient claims for additional procedures.
				//2300 HI: BI (institutional) Occurence Span Information. Situational. We do not use. For an occurence span code.
				//2300 HI: BH (institutional) Occurence Information. Situational. We do not use. For an occurence code.
				//2300 HI: BE (institutional) Value Information. Situational. We do not use. For a value code.
				//2300 HI: BG (medical,institutional) Condition Information. Situational. We do not use. For a condition code.
				//2300 HI: TC (institutional) Treatment Code Information. Situational. We do not use. When home health agencies need to report plan of treatment information under contracts.
				//2300 HCP: (medical,institutional,dental) Claim Pricing/Repricing Information. Situational. We do not use.
				#endregion Claim HI HCP
				bool sendFacilityNameAndAddress=false;
				if(medType==EnumClaimMedType.Medical) {
					//Code added to send the segment below, but no logic here for sending yet, so it is never sent.
				}
				else if(medType==EnumClaimMedType.Institutional) {
					//Code added to send the segment below, but no logic here for sending yet, so it is never sent.
				}
				else if(medType==EnumClaimMedType.Dental) {
					if(IsClaimConnect(clearinghouseClin) || IsDentiCal(clearinghouseClin)) {
						//Osvaldo Ferrer, VIP account manager for DentalXChange, says we need the segment whenever the place of service is not office.
						//Denti-Cal now requires the ability to send facility information, as of the last round of testing for a customer on 05/11/2015.
						sendFacilityNameAndAddress=true;
					}
					else {//for other clearinghouses, the X12 specs say that we don't send it if it's the same as the billing prov.
						//and since we always set it the same as the billing prov, we shouldn't send it.
					}
				}
				Provider facilityProv=billProv;//If this provider changes in the future, then the validation section will also need to be updated.
				string facilityAddress1=billingAddress1;
				string facilityAddress2=billingAddress2;
				string facilityCity=billingCity;
				string facilityState=billingState;
				string facilityZip=billingZip;
				if(site!=null && site.ProvNum > 0 && site.ProvNum!=billProv.ProvNum) {
					facilityProv=Providers.GetProv(site.ProvNum);
					facilityAddress1=site.Address;
					facilityAddress2=site.Address2;
					facilityCity=site.City;
					facilityState=site.State;
					facilityZip=site.Zip;
					sendFacilityNameAndAddress=true;
				}
				//For medical, instititional, and dental, sending facility NPI must be for an organization (non-person).  Facility must also be external.
				if(!facilityProv.IsNotPerson || claim.PlaceService==PlaceOfService.Office) {//Validated also
					sendFacilityNameAndAddress=false;
				}
				provTreat=Providers.GetProv(claim.ProvTreat);
				provClinicTreat=ProviderClinics.GetFromList(claim.ProvTreat,(clinic==null ? 0 : clinic.ClinicNum),listProvClinics,true);
				#region 2310 Claim Providers (medical)
				//Since order might be important, we have to handle medical, institutional, and dental separately.
				if(medType==EnumClaimMedType.Medical) {
					//2310A NM1: DN/P3 (medical) Referring Provider Name. Situational.
					WriteNM1_DN(sw,claim.ReferringProv);
					//2310A REF: (medical) Referring Provider Secondary Identification. Situational. We do not use.
					//Always included for Emdeon Medical, because Emdeon Medical will automatically remove this segment if the payer does not want it
					//Some payers reject if not specified, even when the claim.ProvTreat=claim.ProvBill. For example, Texas Medicaid.
					//if(claim.ProvTreat!=claim.ProvBill || IsEmdeonMedical(clearhouse)) {
					//2310B NM1: 82 (medical) Rendering Provider Name. Required when treating provider is different from billing provider.
					WriteNM1Provider("82",sw,provTreat);
					//2310B PRV: PE (medical) Rendering Provider Specialty Information. Situational.
					WritePRV_PE(sw,provTreat);
					//2310B REF: (medical) Rendering Provider Secondary Identification. Situational. We do not use.					
					//2310C NM1: 77 (medical) Service Facility Location Name. Conditions different than 4010.  
					//Not supposed to send if same location as 2010AA Billing Provider, but some clearinghouses want this segment anyway.
					if(sendFacilityNameAndAddress) {
						sw.Write("NM1"+s
							+"77"+s//NM101 2/3 Entity Identifier Code: 77=Service Location.
							+"2"+s//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
							+Sout(facilityProv.LName,60)+s//NM103 1/60 Name Last or Organization Name: Laboratory or Facility Name.
							+s//NM104 1/35 Name First: Not used.
							+s//NM105 1/25 Name Middle: Not used.
							+s//NM106 1/10 Name Prefix: Not used.
							+s//NM107 1/10 Name Suffix: Not used.
							+"XX"+s//NM108 1/2 Identification Code Qualifier: XX=NPI.
							+Sout(facilityProv.NationalProvID,80));//NM109 2/80 Identification Code: Laboratory or Facility Identifier. Validated.
						EndSegment(sw);//NM110 through NM112 not used.
						//2310C N3: (medical) Service Facility Location Address.
						sw.Write("N3"+s+Sout(facilityAddress1,55));//N301 1/55 Address Information: Laboratory or Facility Address Line.
						if(facilityAddress2!="") {
							sw.Write(s+Sout(facilityAddress2,55));//N302 1/55 Address Information: Laboratory or Facility Address Line 2. Only required when there is a secondary address line.
						}
						EndSegment(sw);
						//2310C N4: (medical) Service Facility Location City, State, Zip Code.
						sw.Write("N4"+s
							+Sout(facilityCity,30)+s//N401 2/30 City Name: Laboratory or Facility City Name.
							+Sout(facilityState,2,2)+s//N402 2/2 State or Provice Code: Laboratory or Facility State or Province Code.
							+Sout(facilityZip.Replace("-",""),15));//N403 3/15 Postal Code: Laboratory or Facility Postal Zone or ZIP Code.
						EndSegment(sw);//N404 through N407 are either not used or only required when outside of the United States.
						//2310C REF: (medical) Service Facility Location Secondary Identification. Situational. We do not use this.
						//2310C PER: (medical) Service Facility Contact Information. Situational. Required for property and casualty claims. We do not use.
					}
					//2310D NM1: (medical) Supervising Provider Name. Situational. We do not use.
					//2310D REF: (medical) Supervising Provider Secondary Identification. Situational. We do not use.
					//2310E NM1: (medical) Ambulance Pick-up Location. Situational. We do not use.
					//2310E N3: (medical) Ambulance Pick-up Location Address. We do not use.
					//2310E N4: (medical) Ambulance Pick-up Location City, State, Zip Code. We do not use.
					//2310F NM1: (medical) Ambulance Drop-off Location. Situational. We do not use.
					//2310F N3: (medical) Ambulance Drop-off Location Address. We do not use.
					//2310F N4: (medical) Ambulance Drop-off Location City, State, Zip Code. We do not use.
				}
				#endregion 2310 Claim Providers (medical)
				#region 2310 Claim Providers (inst)
				if(medType==EnumClaimMedType.Institutional) {
					//2310A NM1: 71 (institutional) Attending Provider Name. Situational. Always a person according to the specification (cannot be non-person).
					WriteNM1Provider("71",sw,provTreat.FName,provTreat.MI,provTreat.LName,provTreat.NationalProvID,false);
					//2310A PRV: AT (institutional) Attending Provider Specialty Information. Situational.
					sw.Write("PRV"+s
						+"AT"+s//PRV01 1/3 Provider Code: AT=Attending.
						+"PXC"+s//PRV02 2/3 Reference Identification Qualifier: PXC=Health Care Provider Taxonomy Code.
						+X12Generator.GetTaxonomy(provTreat));//PRV03 1/50 Reference Identification: Provider Taxonomy Code.
					EndSegment(sw);//PRV04 through PRV06 are not used.
					//2310A REF: (institutional) Attending Provider Secondary Identification. Situational.
					//2310B NM1: 72 (institutional) Operating Physician Name. Situational. For surgical procedure codes.
					//According to Emdeon Medical/Change Medical tech, the surgical code range is 10000 - 69990.
					//This issue arose for the carrier Humana.  See task #2712788.
					if(IsEmdeonMedical(clearinghouseClin)//For now, only for EmdeonMedical as no one else has reported an issue with this loop being missing.
						&& claimProcs.Any(x => int.TryParse(x.CodeSent,out int codeSent) && codeSent.Between(10000,69990)))
					{
						WriteNM1Provider("72",sw,provTreat.FName,provTreat.MI,provTreat.LName,provTreat.NationalProvID,false);
					}
					//2310C REF: ZZ (institutional) Secondary Physician Secondary Identification. Situational.
					//2310C NM1: ZZ (institutional) Other Operating Physician Name. Situational.
					//2310C REF: ZZ (institutional) Other Operating Physician Secondary Identification. Situational.
					//2310D NM1: 82 (institutional) Rendering Provider Name. Situational. If different from attending provider AND when regulations require both facility and professional components.
					//2310D REF: ZZ (institutional) Rendering Provider Secondary Identificaiton. Situational.
					//2310E NM1: 77 (institutional) Service Facility Location Name. Conditions different than 4010.  
					//Not supposed to send if same location as 2010AA Billing Provider, but some clearinghouses want this segment anyway.
					if(sendFacilityNameAndAddress) {
						sw.Write("NM1"+s
							+"77"+s//NM101 2/3 Entity Identifier Code: 77=Service Location.
							+"2"+s//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
							+Sout(facilityProv.LName,60)+s//NM103 1/60 Name Last or Organization Name: Laboratory or Facility Name.
							+s//NM104 1/35 Name First: Not used.
							+s//NM105 1/25 Name Middle: Not used.
							+s//NM106 1/10 Name Prefix: Not used.
							+s//NM107 1/10 Name Suffix: Not used.
							+"XX"+s//NM108 1/2 Identification Code Qualifier: XX=NPI.
							+Sout(facilityProv.NationalProvID,80));//NM109 2/80 Identification Code: Laboratory or Facility Primary Identifier. Validated.
						EndSegment(sw);//NM110 through NM112 not used.
						//2310E N3: (institutional) Service Facility Location Address.
						sw.Write("N3"+s+Sout(facilityAddress1,55));//N301 1/55 Address Information: Laboratory or Facility Address Line.
						if(facilityAddress2!="") {
							sw.Write(s+Sout(facilityAddress2,55));//N302 1/55 Address Information: Laboratory or Facility Address Line 2. Only required when there is a secondary address line.
						}
						EndSegment(sw);
						//2310E N4: (institutional) Service Facility Location City, State, Zip Code.
						sw.Write("N4"+s
							+Sout(facilityCity,30)+s//N401 2/30 City Name: Laboratory or Facility City Name.
							+Sout(facilityState,2,2)+s//N402 2/2 State or Provice Code: Laboratory or Facility State or Province Code.
							+Sout(facilityZip.Replace("-",""),15));//N403 3/15 Postal Code: Laboratory or Facility Postal Zone or ZIP Code.
						EndSegment(sw);//N404 through N407 are either not used or only required when outside of the United States.
						//2310E REF: ZZ (institutional) Service Facility Location Secondary Identificiation. Situational. We do not use.
					}
					//2310F NM1: DN (institutional) Referring Provider Name. Situational. Required when referring provider is different from attending provider.
					if(claim.ReferringProv!=claim.ProvTreat) {
						WriteNM1_DN(sw,claim.ReferringProv);
					}
					//2310F REF: (institutional) Referring Provider Secondary Identification. Situational. 
				}
				#endregion 2310 Claim Providers (inst)
				#region 2310 Claim Providers (dental)
				if(medType==EnumClaimMedType.Dental) {
					//2310A NM1: DN (dental) Referring Provider Name. Situational.
					WriteNM1_DN(sw,claim.ReferringProv);
					//2310A PRV: (dental) Referring Provider Specialty Information. Situational.
					//2310A REF: G2 (dental) Referring Provider Secondary Identification. Situational.
					bool sendClaimTreatProv=false;
					if(claim.ProvTreat!=claim.ProvBill) { //Emdeon will reject the claim if this segment is the same as the billing provider for all claims in the batch.
						sendClaimTreatProv=true;//Standard X12 behavior to only include the treating provider if it is different than the billing provider.
					}
					//The following clearinghouses always want the claim treating provider, even if it is the same as the billing provider.
					if(IsOfficeAlly(clearinghouseClin) || IsWashingtonMedicaid(clearinghouseClin,carrier)) {
						sendClaimTreatProv=true;
					}
					if(sendClaimTreatProv) {
						//2310B NM1: 82 (dental) Rendering Provider Name. Situational. Only required if different from the billing provider.
						WriteNM1Provider("82",sw,provTreat);
						//2310B PRV: PE (dental) Rendering Provider Specialty Information.
						WritePRV_PE(sw,provTreat);
						//2310B REF: (dental) Rendering Provider Secondary Identification. Situational. Not required because we always send NPI. Max repeat of 4.
						if(IsClaimConnect(clearinghouseClin) || IsEmdeonDental(clearinghouseClin) || IsTesia(clearinghouseClin) || IsEDS(clearinghouseClin)) {
							//The state licence number can be anywhere between 4 and 14 characters depending on state, and most states have more than one state license format. 
							//Therefore, we only validate that the state license is present or not.
							if(provClinicTreat!=null && !provClinicTreat.StateLicense.IsNullOrEmpty()) { 
								sw.Write("REF"+s
									+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number.
									+Sout(provClinicTreat.StateLicense,50));//REF02 1/50 Reference Identification:
								EndSegment(sw);//REF03 and REF04 are not used.
							}
						}
						if(IsEmdeonDental(clearinghouseClin)) { //Always required by Emdeon Dental.
							WriteProv_REFG2orLU(sw,provTreat,carrier.ElectID);
						}
					}
					//2310C NM1: 77 (dental) Service Facility Location Name. Conditions different than 4010.  
					//Not supposed to send if same location as 2010AA Billing Provider, but ClaimConnect wants this segment anyway.
					if(sendFacilityNameAndAddress) {
						sw.Write("NM1"+s
							+"77"+s//NM101 2/3 Entity Identifier Code: 77=Service Location.
							+"2"+s//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
							+Sout(facilityProv.LName,60)+s//NM103 1/60 Name Last or Organization Name: Laboratory or Facility Name.
							+s//NM104 1/35 Name First: Not used.
							+s//NM105 1/25 Name Middle: Not used.
							+s//NM106 1/10 Name Prefix: Not used.
							+s//NM107 1/10 Name Suffix: Not used.
							+"XX"+s//NM108 1/2 Identification Code Qualifier: XX=NPI.
							+Sout(facilityProv.NationalProvID,80));//NM109 2/80 Identification Code: Laboratory or Facility Identifier. Validated.
						EndSegment(sw);//NM110 through NM112 not used.
						//2310C N3: (dental) Service Facility Location Address.
						sw.Write("N3"+s+Sout(facilityAddress1,55));//N301 1/55 Address Information: Laboratory or Facility Address Line.
						if(facilityAddress2!="") {
							sw.Write(s+Sout(facilityAddress2,55));//N302 1/55 Address Information: Laboratory or Facility Address Line 2. Only required when there is a secondary address line.
						}
						EndSegment(sw);
						//2310C N4: (dental) Service Facility Location City, State, Zip Code.
						sw.Write("N4"+s
							+Sout(facilityCity,30)+s//N401 2/30 City Name: Laboratory or Facility City Name.
							+Sout(facilityState,2,2)+s//N402 2/2 State or Provice Code: Laboratory or Facility State or Province Code.
							+Sout(facilityZip.Replace("-",""),15));//N403 3/15 Postal Code: Laboratory or Facility Postal Zone or ZIP Code.
						EndSegment(sw);//N404 through N407 are either not used or only required when outside of the United States.
						//2310C REF: (dental) Service Facility Location Secondary Identification. Situational. We do not use this.
					}
					//2310D NM1: (dental) Assistant Surgeon Name. Situational. We do not support.
					//2310D PRV: (dental) Assistant Surgeon Specialty Information. We do not support.
					//2310D REF: (dental) Assistant Surgeon Secondary Identification. We do not support.
					//2310E NM1: (dental) Supervising Provider Name. Situational. We do not support.
					//2310E REF: (dental) Supervising Provider Secondary Identification. Situational. We do not support.
				}
				#endregion 2310 Claim Providers (dental)
				#region 2320 Other subscriber information
				double claimWriteoffAmt=0;
				List<List <ClaimProc>> listOtherClaimProcs=new List<List<ClaimProc>>();
				List<double> listProcWriteoffAmts=new List<double>();
				List<double> listProcDeductibleAmts=new List<double>();
				List<double> listProcPaidOtherInsAmts=new List<double>();
				bool hasAdjForOtherPlans=false;
				DateTime datePaidOtherIns=DateTime.MinValue;
				EclaimCobInsPaidBehavior cobBehavior=PrefC.GetEnum<EclaimCobInsPaidBehavior>(PrefName.ClaimCobInsPaidBehavior);
				if(carrier.CobInsPaidBehaviorOverride!=EclaimCobInsPaidBehavior.Default) {
					cobBehavior=carrier.CobInsPaidBehaviorOverride;
				}
				bool hasClaimLevelCob=ListTools.In(cobBehavior,EclaimCobInsPaidBehavior.ClaimLevel,EclaimCobInsPaidBehavior.Both);
				bool hasProcedureLevelCob=ListTools.In(cobBehavior,EclaimCobInsPaidBehavior.ProcedureLevel,EclaimCobInsPaidBehavior.Both);
				//2320 Other subscriber------------------------------------------------------------------------------------------
				if(otherPlan!=null) {
					//2320 SBR: Other Subscriber Information. Situational.
					sw.Write("SBR"+s);
					sw.Write((claimIsPrimary?"S":"P")+s);//SBR01 1/1 Payer Responsibility Sequence Number Code: When the claim is primary then the other insurance is secondary, and vice versa.
					sw.Write(GetRelat(claim.PatRelat2)+s//SBR02 2/2 Individual Relationship Code:
						+Sout(otherPlan.GroupNum,50)+s);//SBR03 1/50 Reference Identification:
					//SBR04 1/60 Name: Situational. Required when SBR03 is not specified.
					if(otherPlan.GroupNum!="") {
						sw.Write(s);
					}
					else {
						sw.Write(Sout(otherPlan.GroupName,60)+s);
					}
					sw.Write(s//SBR05 1/3 Insurance Type Code: Situational. Required when the payer in loop 2330B is Medicare and Medicare is not the primary payer. Medical and Dental only. TODO: implement.
						+s//SBR06 1/1 Coordination of Benefits Code: Not used.
						+s//SBR07 1/1 Yes/No Condition or Response Code: Not Used.
						+s//SBR08 2/2 Employment Status Code: Not Used.
						+GetFilingCode(otherPlan));//SBR09 1/2 Claim Filing Indicator Code: 12=PPO,17=DMO,BL=BCBS,CI=CommercialIns,FI=FEP,HM=HMO. Will no longer be required when HIPPA National Plan ID is mandated.
					EndSegment(sw);
					claimWriteoffAmt=0;
					double claimDeductibleAmt=0;
					double claimPaidOtherInsAmt=0;
					//In addition to the claimprocs attached to the procedures going out on this claim, we must also include amounts for Total Payments from other insurance.
					//Total payments will go out at claim level and not procedure level.
					List<long> listProcNums=claimProcs.Select(x => x.ProcNum).Distinct().ToList();
					List<long> listOtherClaimNums=claimProcList.FindAll(x => x.ClaimNum!=claim.ClaimNum 
						&& ListTools.In(x.Status,ClaimProcStatus.CapClaim, ClaimProcStatus.Received, ClaimProcStatus.Supplemental)
						&& listProcNums.Contains(x.ProcNum)).Select(x => x.ClaimNum).Distinct().ToList();
					List<ClaimProc> listTotalPaymentProcs=claimProcList.FindAll(x => listOtherClaimNums.Contains(x.ClaimNum) && x.ProcNum==0);
					foreach(ClaimProc cpTotalPayment in listTotalPaymentProcs) {
						claimWriteoffAmt+=cpTotalPayment.WriteOff;
						claimDeductibleAmt+=cpTotalPayment.DedApplied;
						claimPaidOtherInsAmt+=cpTotalPayment.InsPayAmt;
					}
					for(int j=0;j<claimProcs.Count;j++) {//Claim procs for this claim
						listOtherClaimProcs.Add(new List<ClaimProc>());
						double procWriteoffAmt=0;
						double procDeductibleAmt=0;
						double procPaidOtherInsAmt=0;
						for(int k=0;k<claimProcList.Count;k++) {//All claim procs for patient
							if(ClaimProcs.IsValidClaimAdj(claimProcList[k],claimProcs[j].ProcNum,claimProcs[j].InsSubNum)) {//Adjustment due to other insurance plans.
								hasAdjForOtherPlans=true;
								claimWriteoffAmt+=claimProcList[k].WriteOff;
								claimDeductibleAmt+=claimProcList[k].DedApplied;
								claimPaidOtherInsAmt+=claimProcList[k].InsPayAmt;
								listOtherClaimProcs[listOtherClaimProcs.Count-1].Add(claimProcList[k]);
								procWriteoffAmt+=claimProcList[k].WriteOff;
								procDeductibleAmt+=claimProcList[k].DedApplied;
								procPaidOtherInsAmt+=claimProcList[k].InsPayAmt;
							}
						}
						listProcWriteoffAmts.Add(procWriteoffAmt);
						listProcDeductibleAmts.Add(procDeductibleAmt);
						listProcPaidOtherInsAmts.Add(procPaidOtherInsAmt);
					}
					double claimPatientPortionAmt=Math.Max(0,claim.ClaimFee-claimWriteoffAmt-claimDeductibleAmt-claimPaidOtherInsAmt);
					//If sending the primary claim, then hasAdjForOtherPlans will be false, because all claimprocs for any other plans (secondary) will be estimates.
					//If sending the secondary claim, then hasAdjForOtherPlans will be true, because the primary claimprocs will be received.
					//This strategy works for dental and medical plans in any combination: D, M, DD, DM, MD, MM
					//Apex requires line level CAS segments and does not accept claim level CAS segments.
					if(hasAdjForOtherPlans && !IsApex(clearinghouseClin) && hasClaimLevelCob) {
						//2320 CAS: (medical,institutional,dental) Claim Level Adjustments. Situational. We use this to show patient responsibility, because the adjustments here plus AMT D below must equal claim amount in CLM02 for Emdeon.
						//Claim Adjustment Reason Codes can be found on the Washington Publishing Company website at: http://www.wpc-edi.com/reference/codelists/healthcare/claim-adjustment-reason-codes/
						if(claimWriteoffAmt>0) {
							sw.Write("CAS"+s
								+"CO"+s//CAS01 1/2 Claim Adjustment Group Code: CO=Contractual Obligations.
								+"45"+s//CAS02 1/5 Claim Adjustment Reason Code: 45=Charge exceeds fee schedule/maximum allowable or contracted/legislated fee arrangement.
								+AmountToStrNoLeading(claimWriteoffAmt));//CAS03 1/18 Monetary Amount:
							EndSegment(sw);
						}
						if(claimDeductibleAmt>0 || claimPatientPortionAmt>0) {
							sw.Write("CAS"+s
								+"PR");//CAS01 1/2 Claim Adjustment Group Code: PR=Patient Responsibility.
							if(claimDeductibleAmt>0) {
								sw.Write(s//end of previous field
									+"1"+s//CAS02 1/5 Claim Adjustment Reason Code: 1=Deductible.
									+AmountToStrNoLeading(claimDeductibleAmt)+s//CAS03 1/18 Monetary Amount:
									+"1");//CAS04 1/15 Quantity:
							}
							if(claimPatientPortionAmt>0) {
								sw.Write(s//end of previous field
									+"3"+s//CAS02 or CAS05 1/5 Claim Adjustment Reason Code: 3=Co-payment Amount.
									+AmountToStrNoLeading(claimPatientPortionAmt));//CAS03 or CAS06 1/18 Monetary Amount:
							}
							EndSegment(sw);
						}
						//2320 AMT: D (medical,institutional,dental) COB Payer Paid Amount. Situational. Required when the claim has been adjudicated by payer in loop 2330B.
						sw.Write("AMT"+s
							+"D"+s//AMT01 1/3 Amount Qualifier Code: D=Payor Amount Paid.
							+AmountToStrNoLeading(claimPaidOtherInsAmt));//AMT02 1/18 Monetary Amount:
						EndSegment(sw);//AMT03 Not used.
						//2320 AMT: EAF (medical,institutional,dental) Remaining Patient Liability. Situational. Required when claim has been adjudicated by payer in loop 2330B.
						sw.Write("AMT"+s
							+"EAF"+s//AMT01 1/3 Amount Qualifier Code: EAF=Amount Owed.
							+AmountToStrNoLeading(claimPatientPortionAmt));//AMT02 1/18 Monetary Amount:
						EndSegment(sw);//AMT03 Not used.
						//2320 AMT: A8 (medical,institutional,dental) COB Total Non-Covered Amount. Situational. Can be set when primary claim was not adjudicated. We do not use.
					}
					if(IsClaimConnect(clearinghouseClin) || IsOfficeAlly(clearinghouseClin)) {
						//2320 DMG: Other subscriber demographics. This segment is not allowed in X12. ClaimConnect requires this information anyway. They will fix their validator later.
						sw.Write("DMG"+s
							+"D8"+s//DMG01 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
							+otherSubsc.Birthdate.ToString("yyyyMMdd")+s//DMG02 1/35 Date Time Period: Birthdate. The othet subscriber birtdate is validated.
							+GetGender(otherSubsc.Gender));//DMG03 1/1 Gender Code: F=Female, M=Male, U=Unknown.
							EndSegment(sw);
					}
					//2320 OI: (medical,institutional,dental) Other Insurance Coverage Information.
					sw.Write("OI"+s
						+s//OI01 1/2 Claim Filing Indicator Code: Not used.
						+s//OI02 2/2 Claim Submission Reason Code: Not used.
						+(Claims.GetAssignmentOfBenefits(claim,otherSub)?"Y":"N")+s//OI03 1/1 Yes/No Condition or Response Code:
						+s//OI04 1/1 Patient Signature Source Code: Not used in institutional or dental. Situational in medical, but we do not support.
						+s//OI05 1/1 Provider Agreement Code: Not used.
						+(otherSub.ReleaseInfo?"Y":"I"));//OI06 1/1 Release of Information Code:
					EndSegment(sw);
					//2320 MIA: (institutional) Inpatient Adjudication Information. Situational. We do not support.
					//2320 MOA: (medical,institutional,dental) Outpatient Adjudication Information. Situational. For reporting remark codes from ERAs. We don't support.
					#endregion 2320 Other subscriber information
					#region 2330A Other subscriber Name
					//2330A NM1: IL (medical,institutional,dental) Other Subscriber Name.
					sw.Write("NM1"+s
						+"IL"+s//NM101 2/3 Entity Identifier Code: IL=Insured or Subscriber.
						+"1"+s//NM102 1/1 Entity Type Qualifier: 1=Person.
						+Sout(otherSubsc.LName,60)+s//NM103 1/60 Name Last or Organization Name: Never blank, because validated in the patient edit window when a patient is added/edited.
						+Sout(otherSubsc.FName,35)+s//NM104 1/35 Name First:
						+Sout(otherSubsc.MiddleI,25)+s//NM105 1/25 Middle Name:
						+s//NM106 1/10 Name Prefix: Not used.
						+s//NM107 1/10 Name Suffix: Situational. No corresponding field in OD.
						+"MI"+s//NM108 1/2 Identification Code Qualifier: MI=Member Identification Number.
						+Sout(otherSub.SubscriberID,80));//NM109 2/80 Identification Code:
					EndSegment(sw);//NM110 through NM112 are not used.
					//2330A N3: Other Subscriber Address.
					sw.Write("N3"+s+Sout(otherSubsc.Address,55));//N301 1/55 Address Information:
					if(otherSubsc.Address2!="") {
						sw.Write(s+Sout(otherSubsc.Address2,55));
					}
					EndSegment(sw);//N302 1/55 Address Information:
					//2330A N4: (medical,institutional,dental) Other Subscriber City, State, Zip Code.
					sw.Write("N4"+s
						+Sout(otherSubsc.City,30)+s//N401 2/30 City Name:
						+Sout(otherSubsc.State,2,2)+s//N402 2/2 State or Province Code:
						+Sout(otherSubsc.Zip.Replace("-",""),15));//N403 3/15 Postal Code:
					EndSegment(sw);//N404 through N407 are either not required or are required when the address is outside of the United States.
					//2330A REF: (medical,institutional,dental) Other Subscriber Secondary Identification. Situational. Not supported.
					#endregion 2330A Other subscriber Name
					#region Other payer
					//2330B NM1: (medical,institutional,dental) Other Payer Name.
					sw.Write("NM1"+s
						+"PR"+s//NM101 2/3 Entity Code Identifier: PR=Payer.
						+"2"+s);//NM102 1/1 Entity Type Qualifier: 2=Non-Person.
					//NM103 1/60 Name Last or Organization Name:
					if(IsEMS(clearinghouseClin)) {
						long employerNum=0;
						if(otherPlan!=null) {
							employerNum=otherPlan.EmployerNum;
						}
						//This is a special situation requested by EMS.  This tacks the employer onto the end of the carrier.
						sw.Write(Sout(otherCarrier.CarrierName,30)+"|"+Sout(Employers.GetName(employerNum),30)+s);
					}
					else if(IsDentiCal(clearinghouseClin)) {
						sw.Write("DENTICAL"+s);
					}
					else {
						sw.Write(Sout(otherCarrier.CarrierName,60)+s);
					}
					sw.Write(s//NM104 1/35 Name First: Not used.
						+s//NM105 1/25 Name Middle: Not used.
						+s//NM106 1/10 Name Prefix: Not used.
						+s//NM107 1/10 Name Suffix: Not used.
						+"PI"+s);//NM108 1/2 Identification Code Qualifier: PI=Payor Identification. XV must be used after national plan ID mandated.
					sw.Write(Sout(GetCarrierElectID(otherCarrier,clearinghouseClin),80,2));//NM109 2/80 Identification Code:
					EndSegment(sw);//NM110 through NM112 not used.
					//2230B N3: (medical,institutional,dental) Other Payer Address. Situational.
					sw.Write("N3"+s+Sout(otherCarrier.Address,55));//N301 1/55 Address Information:
					if(otherCarrier.Address2!="") {
						sw.Write(s+Sout(otherCarrier.Address2,55));//N302 1/55 Address Information: Required when there is a second address line.
					}
					EndSegment(sw);
					//2330B N4: (medical,institutional,dental) Other Payer City, State, Zip Code. Situational.
					sw.Write("N4"+s
						+Sout(otherCarrier.City,30)+s//N401 2/30 City Name:
						+Sout(otherCarrier.State,2)+s//N402 2/2 State or Province Code:
						+Sout(otherCarrier.Zip.Replace("-",""),15));//N403 3/15 Postal Code:
					EndSegment(sw);//N404 through N407 are either not used or are for addresses outside of the United States.
					//2330B DTP: 573 (medical,institutional,dental) Claim Check or Remittance Date. Situational. Claim Paid date.
					if(hasAdjForOtherPlans) {
						//In the future, we should consider getting the datePaidOtherIns from claimpayment.DateIssued instead,
						//since this is the date on the check from the carrier, which is what the format is asking for.
						//The claimpayment.DateIssued field is currently optional in UI, thus we would need to validate it below.
						//There will always be at least 1 claimproc in listOtherClaimProcs since hasAdjForOtherPlans is true.
						datePaidOtherIns=listOtherClaimProcs.SelectMany(x => x).Concat(listTotalPaymentProcs).Max(x => x.DateCP);
						//it's a required segment, so always include it.
						sw.Write("DTP"+s
							+"573"+s//DTP01 3/3 Date/Time Qualifier: 573=Date Claim Paid.
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
							+datePaidOtherIns.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2330B REF: (medical,institutional,dental) Other Payer Secondary Identifier. Situational. We do not use.
					//2330B REF: G1 (medical,institutional,dental) Other Payer Prior Authorization Number. Situational. We do not use.
					//2330B REF: 9F (medical,institutional,dental) Other Payer Referral Number. Situational. We do not use.
					//2330B REF: T4 (medical,institutional,dental) Other Payer Claim Adjustment Indicator. Situational. We do not use.
					//2330B REF: G3 (dental) Other Payer Predetermination Identification. Situational. We do not use.
					//2230B REF: F8 (medical,institutional,dental) Other Payer Claim Control Number. Situational. We do not use.					
					if(medType==EnumClaimMedType.Medical) {
						//2330C NM1: (medical) Other Payer Referring Provider. Situational. Only used in crosswalking COBs. We do not use.
						//2330C REF: (medical) Other Payer Referring Provider Secondary Identification. We do not use.
						//2330D NM1: 82 (medical) Other Payer Rendering Provider. Situational. Only used in crosswalking COBs. We do not use.
						//2330D REF: (medical) Other Payer Rendering Provider Secondary Identificaiton. We do not use.
						//2330E NM1: 77 (medical) Other Payer Service Facility Location. Situational. We do not use.
						//2330E REF: (medical) Other Payer Service Facility Location Secondary Identification. We do not use.
						//2330F NM1: DQ (medical) Other Payer Supervising Provider. Situational. We do not use.
						//2330F REF: (medical) Other Payer Supervising Provider Secondary Identificaiton. We do not use.
						//2330G NM1: 85 (medical) Other Payer Billing Provider. Situational. We do not use.
						//2330G REF: (medical) Other Payer Billing Provider Secondary Identification. We do not use.
					}
					else if(medType==EnumClaimMedType.Institutional) {
						//2330C NM1: 71 (institutional) Other Payer Attending Provider. Situational. Only used in crosswalking COBs. We do not use.
						//2330C REF: (institutional) Other Payer Attending Provider Secondary Identification. We do not use.
						//2330D NM1: 72 (institutional) Other Payer Operating Physician. Situational.
						//2330D REF: (institutional) Other Payer Operating Physician Secondary Identificaiton. We do not use.
						//2330E NM1: ZZ (institutional) Other Payer Other Operating Physician. Situational. We do not use.
						//2330E REF: (institutional) Other Payer Other Operating Physician Secondary Identificaiton. We do not use.
						//2330F NM1: 77 (institutional) Other Payer Service Facility Location. Situational. We do not use.
						//2330F REF: (institutional) Other Payer Service Facility Location Secondary Identification. We do not use.
						//2330G NM1: 82 (institutional) Other Payer Rendering Provider Name. Situatiuonal. We do not use.
						//2330G REF: (institutional) Other Payer Rendering Provider Secondary Identificaiton. We do not use.
						//2330H NM1: DN (institutional) Other Payer Referring Provider. Situational. We do not use.
						//2330H REF: (institutional) Other Payer Referring Provider Secondary Identificaiton. We do not use.
						//2330I NM1: 85 (institutional) Other Payer Billing Provider. Situational. We do not use.
						//2330I REF: (institutional) Other Payer Billing Provider Secondary Identification. We do not use.
					}
					else if(medType==EnumClaimMedType.Dental) {
						//2330C NM1: (dental) Other Payer Referring Provider. Situational. Only used in crosswalking COBs. We do not use.
						//2330C REF: (dental) Other Payer Referring Provider Secondary Identification. We do not use.
						//2330D NM1: 82 (dental) Other Payer Rendering Provider. Situational. Only used in crosswalking COBs. We do not use.
						//2330D REF: (dental) Other Payer Rendering Provider Secondary Identificaiton. We do not use.
						//2330E NM1: DQ (dental) Other Payer Supervising Provider. Situational. We do not use.
						//2330E REF: (dental) Other Payer Supervising Provider Secondary Identificaiton. We do not use.
						//2330F NM1: 85 (dental) Other Payer Billing Provider. Situational. We do not use.
						//2330F REF: (dental) Other Payer Billing Provider Secondary Identification. We do not use.
						//2330G NM1: 77 (dental) Other Payer Service Facility Location. Situational. We do not use.
						//2330G REF: (dental) Other Payer Service Facility Location Secondary Identification. We do not use.
						//2330H NM1: DD (dental) Other Payer Assistant Sugeon. Situational. We do not use.
						//2330H REF: (dental) Other Payer Assistant Surgeon Secondary Identifier. We do not use.
					}
					#endregion Other payer
				}
				for(int j=0;j<claimProcs.Count;j++) {//Claim procs for this claim
					#region Service Line
					proc=Procedures.GetProcFromList(procList,claimProcs[j].ProcNum);
					procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
					//2400 LX: Service Line Number.
					sw.Write("LX"+s+(j+1).ToString());//LX01 1/6 Assigned Number:
					EndSegment(sw);
					if(medType==EnumClaimMedType.Medical) {
						//2400 SV1: Professional Service.
						sw.Write("SV1"+s
							//SV101 Composite Medical Procedure Identifier
							+"HC"+isa16//SV101-1 2/2 Product/Service ID Qualifier: HC=Health Care.
							+Sout(claimProcs[j].CodeSent));//SV101-2 1/48 Product/Service ID: Procedure code.
						if(proc.CodeMod1!="" || proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod1));//SV101-3 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod2));//SV101-4 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod3));//SV101-5 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod4));//SV101-6 2/2 Procedure Modifier: Situational.
						}
						if(proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.ClaimNote,80));//SV101-7 1/80 Description: Situational.
						}
						sw.Write(s//SV101-8 is not used.
							+claimProcs[j].FeeBilled.ToString()+s);//SV102 1/18 Monetary Amount: Charge Amt.
						if(proc.UnitQtyType==ProcUnitQtyType.MinutesAnesth) {
							sw.Write("MJ"+s);//SV103 2/2 Unit or Basis for Measurement Code: MJ=minutes, UN=Unit.
						}
						else {
							sw.Write("UN"+s);//SV103 2/2 Unit or Basis for Measurement Code: MJ=minutes, UN=Unit.
						}
						sw.Write(proc.UnitQty+s);//SV104 1/15 Quantity: Service Unit Count or Anesthesia Minutes.
						if(proc.PlaceService!=claim.PlaceService) {
							sw.Write(GetPlaceService(proc.PlaceService));
						}
						sw.Write(s//SV105 1/2 Facility Code Value: Place of Service Code if different from claim.
							+s);//SV106 1/2 Service Type Code: Not used.
						//SV107: Composite Diagnosis Code Pointer. Required when 2300HI(Health Care Diagnosis Code) is used (always).
						//SV107-1: Primary diagnosis. Only allowed pointers 1-12.
						//SV107-2 through SV107-4: Other diagnoses.
						//If the diagnosis we need is not in the first 12, then we will use the principal.
						if(proc.DiagnosticCode=="") {//If the all 4 procedure diagnoses are blank, we will use the primary diagnosis for entire claim.
							if(listDiagnoses[0]!="") {//Ensure that a primary diagnosis exists.
								sw.Write("1");//use primary.
							}
						}
						else {//There is at least one diagnostic code on the proc, and also at least one proc on the claim (at least the principal diagnosis).
							int diagMatchCount=0;
							for(int d=0;d<listDiagnoses.Count;d++) {//this list is filled with unique diagnosis codes so the following logic will create 4 correct entries.								
								if(listDiagnoses[d]=="") {
									continue;
								}
								//Validation is performed below to ensure that here are no more than 12 unique diagnoses per claim.
								if(listDiagnoses[d]==proc.DiagnosticCode || listDiagnoses[d]==proc.DiagnosticCode2 ||
									listDiagnoses[d]==proc.DiagnosticCode3 || listDiagnoses[d]==proc.DiagnosticCode4)
								{
									if(diagMatchCount>0) {
										sw.Write(isa16);
									}
									sw.Write(d+1);//1 through 12
									diagMatchCount++;
								}
							}
						}
						if(proc.Urgency==ProcUrgency.Emergency) {
							//SV108 Monetary Amount 1/18: Not used.
							sw.Write(s+s);
							//SV109 1/1 Yes/No Condition or Response Code.  If procedure is emergency related then "Y" otherwise "N".
							sw.Write("Y");
						}
						EndSegment(sw);//SV110 through SV121 are not used or situational. We do not use.
					}
					else if(medType==EnumClaimMedType.Institutional) {
						//2400 SV2: Institutional Service Line.
						sw.Write("SV2"+s
							+Sout(proc.RevCode,48)+s//SV201 1/48 Product/Service ID: Revenue Code, validated.
							//SV202 Composite Medical Procedure Identifier
							+"HC"+isa16//SV202-1 2/2 Product/Service ID Qualifier: HC=Health Care. Includes CPT codes.
							+Sout(claimProcs[j].CodeSent));//SV202-2 1/48 Product/Service ID: Procedure code. 
						//mods validated to be exactly 2 char long or else blank.
						if(proc.CodeMod1!="" || proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod1));//SV202-3 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod2));//SV202-4 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod3));//SV202-5 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod4));//SV202-6 2/2 Procedure Modifier: Situational.
						}
						if(proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.ClaimNote,80));//SV202-7 1/80 Description: Situational.
						}
						sw.Write(s//SV202-8 is not used.
							+claimProcs[j].FeeBilled.ToString()+s);//SV203 1/18 Monetary Amount: Charge Amt.
						if(proc.UnitQtyType==ProcUnitQtyType.Days) {
							sw.Write("DA"+s);//SV204 2/2 Unit or Basis for Measurement Code: DA=Days, UN=Unit.
						}
						else {
							sw.Write("UN"+s);//SV204 2/2 Unit or Basis for Measurement Code: DA=Days, UN=Unit.
						}
						sw.Write(proc.UnitQty.ToString());//SV205 1/15 Quantity:
						EndSegment(sw);//SV206,208,209 and 210 are not used, SV207 is situational but we do not use.
					}
					else if(medType==EnumClaimMedType.Dental) {
						//2400 SV3: Dental Service.
						sw.Write("SV3"+s
								+"AD"+isa16//SV301-1 2/2 Product/Service ID Qualifier: AD=American Dental Association Codes
								+Sout(claimProcs[j].CodeSent,5));//SV301-2 1/48 Product/Service ID: Procedure code
						if(proc.CodeMod1!="" || proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod1));//SV301-3 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod2!="" || proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod2));//SV301-4 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod3!="" || proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod3));//SV301-5 2/2 Procedure Modifier: Situational.
						}
						if(proc.CodeMod4!="" || proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.CodeMod4));//SV301-6 2/2 Procedure Modifier: Situational.
						}
						if(proc.ClaimNote!="") {
							sw.Write(isa16+Sout(proc.ClaimNote,80));//SV301-7 1/80 Description: Situational.
						}
						sw.Write(s//SV301-8 is not used.
							+claimProcs[j].FeeBilled.ToString());//SV302 1/18 Monetary Amount: Charge Amount.
						string placeService="";
						if(proc.PlaceService!=claim.PlaceService) {
							placeService=GetPlaceService(proc.PlaceService);
						}
						string area=GetArea(proc,procCode);
						int unitQty=Math.Max(proc.UnitQty,1);//Minimum of 1
						bool includeUnits=false;
						if(unitQty>=2) {//Standard behavior based on the X12 guide.
							includeUnits=true;
						}
						//The following carriers always want to see the unit quantity, even if it is only 1.
						if(IsColoradoMedicaid(clearinghouseClin) || IsWashingtonMedicaid(clearinghouseClin,carrier)) {
							includeUnits=true;
						}
						bool isDiagnosisIncluded=false;
						if(proc.IcdVersion!=9 && (proc.DiagnosticCode!="" || proc.DiagnosticCode2!="" || proc.DiagnosticCode3!="" || proc.DiagnosticCode4!="")) {
							isDiagnosisIncluded=true;
						}
						if(placeService!="" || area!="" || proc.Prosthesis!="" || includeUnits || isDiagnosisIncluded) {
							sw.Write(s+placeService);//SV303 1/2 Facility Code Value: Location Code if different from claim.
						}
						if(area!="" || proc.Prosthesis!="" || includeUnits || isDiagnosisIncluded) {
							sw.Write(s+area);//SV304 Oral Cavity Designation: SV304-1 1/3 Oral Cavity Designation Code: Area. SV304-2 through SV304-5 are situational and we do not use.
						}
						if(proc.Prosthesis!="" || includeUnits || isDiagnosisIncluded) {
							sw.Write(s+proc.Prosthesis);//SV305 1/1 Prothesis, Crown or Inlay Code: I=Initial Placement. R=Replacement.
						}
						if(includeUnits || isDiagnosisIncluded) {
							sw.Write(s);
							if(includeUnits) {
								sw.Write(unitQty.ToString());//SV306 1/15 Quantity: Situational. Procedure count.
							}
						}
						if(isDiagnosisIncluded) {
							sw.Write(s);
							//SV307 1/80 Description.  Not used.
							sw.Write(s);
							//SV308 1/1 Copay Status Code.  Not used.
							sw.Write(s);
							//SV309 1/1 Provider Agreement Code.  Not used.
							sw.Write(s);
							//SV310 1/1 Yes No Condition or Response Code.  Not used.
							sw.Write(s);
							//SV311 Composite Diagnosis Code Pointer.  Situational.  Use when diagnosis codes are needed to adjudicate the claim.
							//SV311-1 through SV311-4 1/2 Diagnosis Code Pointer(s).
							//If the diagnosis we need is not in the first 4, then we will use the principal.
							int diagMatchCount=0;
							for(int d=0;d<listDiagnoses.Count;d++) {//this list is filled with unique diagnosis codes so the following logic will create 4 correct entries.								
								if(listDiagnoses[d]=="") {
									continue;
								}
								//Validation is performed below to ensure that here are no more than 4 unique diagnoses per claim.
								if(listDiagnoses[d]==proc.DiagnosticCode || listDiagnoses[d]==proc.DiagnosticCode2 ||
									listDiagnoses[d]==proc.DiagnosticCode3 || listDiagnoses[d]==proc.DiagnosticCode4) 
								{
									if(diagMatchCount>0) {
										sw.Write(isa16);
									}
									sw.Write(d+1);//1 through 12
									diagMatchCount++;
								}
							}
						}
						EndSegment(sw);
						//2400 TOO: Tooth Information. Number/Surface. Multiple iterations of the TOO segment are allowed only when the quantity reported in Loop ID-2400 SV306 is equal to one.
						if(procCode.TreatArea==TreatmentArea.Tooth) {
							sw.Write("TOO"+s
								+"JP"+s//TOO01 1/3 Code List Qualifier Code: JP=Universal National Tooth Designation System.
								+proc.ToothNum);//TOO02 1/30 Industry Code: Tooth number.
							EndSegment(sw);//TOO03 Tooth Surface: Situational. Not applicable.
						}
						else if(procCode.TreatArea==TreatmentArea.Surf) {
							sw.Write("TOO"+s
								+"JP"+s//TOO01 1/3 Code List Qualifier Code: JP=Universal National Tooth Designation System.
								+proc.ToothNum+s);//TOO02 1/30 Industry Code: Tooth number.
							string validSurfaces=Tooth.SurfTidyForClaims(proc.Surf,proc.ToothNum);
							for(int k=0;k<validSurfaces.Length;k++) {
								if(k>0) {
									sw.Write(isa16);
								}
								sw.Write(validSurfaces.Substring(k,1));//TOO03 Tooth Surface: TOO03-1 through TOO03-5 are for individual surfaces.
							}
							EndSegment(sw);
						}
						else if(procCode.TreatArea==TreatmentArea.ToothRange || procCode.AreaAlsoToothRange) {
							string[] individTeeth=proc.ToothRange.Split(',');
							for(int t=0;t<individTeeth.Length;t++) {
								sw.Write("TOO"+s
									+"JP"+s//TOO01 1/3 Code List Qualifier Code: JP=Universal National Tooth Designation System.
									+individTeeth[t]);//TOO02 1/30 Industry Code: Tooth number.
								EndSegment(sw);//TOO03 Tooth Surface: Situational. Not applicable.
							}
						}
					}//dental
					#endregion Service Line
					//2400 PWK: (institutional) Line Supplemental Information. Situational. We do not use.
					//2400 CRC: (medical) Condition Indicator/Durable Medical Equipment. Situational. We do not use.
					#region Service DTP
					//2400 DTP: 472 (medical,institutional,dental) Service Date. Situaitonal. Required for medical. Required if different from claim for dental and inst.
					if(claim.ClaimType!="PreAuth") {
						bool useProcDateService=false;
						//Always required for medical because there is no date of service at the claim level.
						if(medType==EnumClaimMedType.Medical) {
							useProcDateService=true;
						}
						else { //Institutional and dental.
							//Standard X12 behavior, preferred by the following clearinghouses: EmdeonDental.
							//Required for institutional and dental when procedure date of service is different from the claim date of service.
							if(claimProcs[j].ProcDate!=claim.DateService) {
								useProcDateService=true;
							}
						}
						//The following clearinghouses always want this segment no matter what: Apex, Inmediata.
						if(IsApex(clearinghouseClin) || IsInmediata(clearinghouseClin)) {
							useProcDateService=true;
						}
						if(useProcDateService) {
							sw.Write("DTP"+s
								+"472"+s//DTP01 3/3 Date/Time Qualifier: 472=Service.
								+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
								+claimProcs[j].ProcDate.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
							EndSegment(sw);
						}
					}
					//2400 DTP: 139/441 (dental) Date Prior Placement. Situational. Required when replacement.
					if(proc.Prosthesis=="R") {//already validated date
						sw.Write("DTP"+s
							+(proc.IsDateProsthEst?"139":"441")+s//DTP01 3/3 Date/Time Qualifier: 139=Estimated, 441=Prior Placement.
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
							+proc.DateOriginalProsth.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2400 DTP: 452 (dental) Date Appliance Placement. Situational. Ortho appliance placement. We do not use.
					//2400 DTP: 446 (dental) Date Replacement. Date ortho appliance replaced. We do not use.
					//2400 DTP: 196 (medical,dental) Date Treatment Start. Situational. Rx date. We do not use.
					//2400 DTP: 198 (dental) Date Treatment Completion. Situational. We do not use.
					//2400 DTP: 471 (medical) Prescription Date: Situational. We do not use.
					//2400 DTP: 607 (medical) Date Certification Revision/Recertification. Situational. Not supported.
					//2400 DTP: 463 (medical) Date Begin Therapy. Situational. Not supported.
					//2400 DTP: 461 (medical) Date Last Certification. Situational. Not supported.
					//2400 DTP: 304 (medical) Date Last Seen. Situational. Not supported.
					//2400 DTP: 738/739 (medical) Test Date. Situational. For Dialysis. Not supported.
					//2400 DTP: 011 (medical) Date Shipped. Situational. Not supported.
					//2400 DTP: 455 (medical) Date Last X-Ray. Situational. Not supported.
					//2400 DTP: 454 (medical) Date Initial Treatment. Situational. Not supported.					
					#endregion Service DTP
					#region Service QTY MEA CN1
					//2400 QTY: PT (medical) Ambulance Patient Count. Situational. Not supported.
					//2400 QTY: FL (medical) Obstetric Anesthesia Additional Units. Situational. Anesthesia quantity. We do not use.
					//2400 MEA: (medical) Test Result. Situational. We do not use.
					//2400 CN1: (medical,dental) Contract Information. Situational. We do not use.
					#endregion Service QTY MEA CN1
					#region Service REF
					//2400 REF: G3 (dental) Service Predetermination Identification. Situational. Pretermination ID. We do not use.
					//2400 REF: G1 (medical,dental) Prior Authorization. Situational. We do not use.
					//2400 REF: 9F (medical,dental) Referral Number. Situational. We do not use.
					//2400 REF: 9A (dental) Repriced Claim Number. Situational. We do not use.
					//2400 REF: 9B (medical,institutional) Repriced Line Item Reference Number. Situational. We do not use.
					//2400 REF: 9C (dental) Adjusted Repriced claim Number. Situational. We do not use.
					//2400 REF: 9D (medical,instituitonal) Adjusted Repriced Line Item Reference Number. Situational. We do not use.
					//2400 REF: 6R (medical,institutional,dental) Line Item Control Number (ProcNum). Used in 835s (electronic EOBs) to match payment to the claimproc.
					//In older versions of OD, we did not send the REF*6R segment, and for these claims, the Line Item Control Number that will show on the 835 is the Service Line Number from LX01 above.
					int ordinal=PatPlans.GetOrdinal(claim.InsSubNum,patPlans);
					string ref02=("x"+proc.ProcNum.ToString()+"/"+ordinal+"/"+insPlan.PlanNum);//Version 3:
					if(ref02.Length>30) {
						//Even though the field allows 1-50 characters the 837 5010 documentation states:
						//"... the HIPAA maximum requirements to be supported by any reciving system is '30'.
						//Characters beyond 30 are not required to be stored nor returned by any 837-receiving system." page 438 in 837 standard.
						int overflowCount=(ref02.Length-30);
						string insPlanRightMost=insPlan.PlanNum.ToString().Substring(overflowCount);//Remove the leading digits, returns right most digits.
						ref02=("y"+proc.ProcNum.ToString()+"/"+ordinal+"/"+insPlanRightMost);
						//Version 4: Implemented in 19.1,18.4,18.3
					}
					//Version 3: Implemented in 18.3 => string ref02=("x"+proc.ProcNum.ToString()+"/"+PatPlans.GetOrdinal(claim.InsSubNum,patPlans)+"/"+insPlan.PlanNum);
					//Version 2: Implemented in 14.2 => string ref02=("p"+proc.ProcNum.ToString());
					//Version 1: Implemented in 00.0 => string ref02=(proc.ProcNum.ToString());
					sw.Write("REF"+s
						+"6R"+s//REF01 2/3 Reference Identification Qualifier: 6R=Procedure Control Number.
						+ref02);//REF02 1/50 Reference Identification: Custom format to help identify procedures on 835s, even if claim/claimproc/patplan is deleted and recreated.
					EndSegment(sw);//REF03 and REF04 are not used.
					//2400 REF: EW (medical) Mammography Certification Number. Situational. We do not use.
					//2400 REF: X4 (medical) Clinical Laboratory Improvement Amendment (CLIA) Number. Situational. We do not use.
					//2400 REF: F4 (medical) Referring Clinical Laboratory Improvement Amendment (CLIA) Facility Identification. Situational. We do not use.
					//2400 REF: BT (medical) Immunization Batch Number. Situational. We do not use.
					#endregion Service REF
					#region Service AMT K3 NTE
					//2400 AMT: T (medical,dental) Sales Tax Amount. Situational. Not supported.
					//2400 AMT: F4 (medical) Postage Claimed Amount. Situational. We do not use.
					//2400 AMT GT (institutional) Service Tax Amount. Situational. Not supported.
					//2400 AMT N8 (institutional) Facility Tax Amount. Situational. Not supported.
					//2400 K3: (medical,dental) File Information. Situational. Not supported.
					//2400 NTE: ADD/DCP (medical) Line Note. Situational. We do not use.
					//2400 NTE: TPO (medical,institutional) Third Party Organization Notes. Situational. Not sent by providers. Not supported.
					#endregion Service AMT K3 NTE
					Provider provTreatProc=provTreat;//procedure level treating provider.
					ProviderClinic provClinicProc=provClinicTreat;
					if(medType==EnumClaimMedType.Medical && claim.ProvTreat!=proc.ProvNum && PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)) {
						provTreatProc=Providers.GetProv(proc.ProvNum);
						provClinicProc=ProviderClinics.GetOneOrDefault(proc.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
					}
					Provider provOrderProc=null;
					if(medType==EnumClaimMedType.Medical) {
						provOrderProc=GetOrderingProviderForProc(claim,carrier,proc,provTreatProc);
					}
					#region Service PS1
					//2400 PS1: (medical) Purchased Service Information. Situational.
					if(medType==EnumClaimMedType.Medical && claim.IsOutsideLab) {
						string provID=provClinicProc?.StateLicense;
						if(string.IsNullOrEmpty(provID)) {
							provID=(provOrderProc??provTreatProc)?.NationalProvID??"";
						}
						sw.Write("PS1"+s+provID+s+claimFeeBilled.ToString("f2"));
						EndSegment(sw);
					}
					#endregion Service PS1
					//2400 HCP: (medical,institutional,dental) Line Pricing/Repricing Information. Situational. Not used by providers. Not supported.
					#region 2410 Service Drug Identification
					//2410 LIN,CTP,REF: (medical) ?
					if(medType==EnumClaimMedType.Medical || medType==EnumClaimMedType.Institutional) {
						//2410 LIN: (medical,institutional) Drug Identification
						if(procCode.DrugNDC!="" && proc.DrugQty>0){
							sw.Write("LIN"+s+s//LIN01 1/20 Assigned Identification: Not used.
								+"N4"+s//LIN02 2/2 Product/Service ID Qualifier: N4=NDC code in 5-4-2 format, no dashes.
								+procCode.DrugNDC);//LIN03 1/48 Product/Service ID: NDC.
							EndSegment(sw);//LIN04 through LIN31 not used.
							//2410 CTP: (medical,institutional) Drug Quantity.
							sw.Write("CTP"+s+s+s+s//CTP01 through CTP03 not used.
								+proc.DrugQty.ToString()+s//CTP04 1/15 Quantity:
								+GetDrugUnitCode(proc.DrugUnit));//CTP05-1 2/2 Unit or Basis for Measurement Code: Code Qualifier, validated to not be None.
							EndSegment(sw);//CTP05-2 through CTP05-15 not used. CTP06 through CTP11 not used.
							//2410 REF (inst) Rx or compound drug association number.  Not supported.
						}
					}
					#endregion 2410 Service Drug Identification
					//2410 REF: VY/XZ (medical,institutional) Prescription or Compound Drug Association Number. Situational. We do not use.
					#region 2420 Service Providers (medical)
					if(medType==EnumClaimMedType.Medical) {
						if(claim.ProvTreat!=proc.ProvNum
							&& PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)) {
							//2420A NM1: 82 (medical) Rendering Provider Name. Only if different from the claim.
							WriteNM1Provider("82",sw,provTreatProc);
							//2420A PRV: (medical) Rendering Provider Specialty Information.
							sw.Write("PRV"+s
								+"PE"+s//PRV01 1/3 Provider Code: PE=Performing.
								+"PXC"+s//PRV02 2/3 Reference Identification Qualifier: PXC=Health Care Provider Taxonomy Code.
								+X12Generator.GetTaxonomy(provTreatProc));//PRV03 1/50 Reference Identification: Taxonomy Code.
							EndSegment(sw);//PRV04 through PRV06 not used.
							//2420A REF: (medical) Rendering Provider Secondary Identification.
							sw.Write("REF"+s
								+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number.
								+Sout((provClinicProc==null ? "" : provClinicProc.StateLicense),50));//REF02 1/50 Reference Identification: 
							EndSegment(sw);//REF03 1/80 Description: Not used. REF04 Reference Identifier: Situational. Not used when REF01 is 0B or 1G.
						}
						if(claim.IsOutsideLab) {
							//2420B NM1: Purchased Service Provider Name. Situational.  Required if outside lab.
							WriteNM1Provider("82",sw,provOrderProc??provTreatProc);
							//2420B REF: Purchased Service Provider Secondary Identificaiton. Situational.
							if(!string.IsNullOrEmpty(provClinicProc?.StateLicense)) {
								sw.Write("REF"+s
									+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number
									+Sout(provClinicProc.StateLicense,50));//REF02 1/50 Reference Indentification
								EndSegment(sw);//REF03 1/80 Description: Not used. REF04 Reference Identifier: Situational. Not used when REF01 is 0B or 1G.
							}
						}
						//2420C NM1: 77 (medical) Service Facility Location Name. Situational.  If site is different on selected procs, then likely unintentional.
						//2420C N3: (medical) Service Facility Location Address. We do not use.
						//2420C N4: (medical) Service Facility Location City, State, Zip Code. We do not use.
						//2420C REF: (medical) Service Facility Location Secondary Identification. Situational. We do not use.
						//2420D NM1: DQ (medical) Supervising Provider Name. Situational. We do not support.
						//2420D REF: (medical) Supervising Provider Secondary Identification. Situational. We do not support.
						//This loop can only be used for a provider that is a person, not an organization, so we don't send this loop if not a person.
						if(provOrderProc!=null && !provOrderProc.IsNotPerson) { //Ordering provider is a person.
							//2420E NM1: DK (medical) Ordering Provider Name. Situational. Required to be a person.
							WriteNM1Provider("DK",sw,provOrderProc);
							//2420E N3: (medical) Ordering Provider Address. Situational.
							sw.Write("N3"+s+Sout(billingAddress1,55));//N301 1/55 Address Information:
							if(billingAddress2!="") {
								sw.Write(s+Sout(billingAddress2,55));//N302 1/55 Address Information: Only required when there is a secondary address line.
							}
							EndSegment(sw);
							//2420E N4: (medical) Ordering Provider City, State, Zip Code. Situational.
							sw.Write("N4"+s
								+Sout(billingCity,30)+s//N401 2/30 City Name:
								+Sout(billingState,2,2)+s//N402 2/2 State or Provice Code:
								+Sout(billingZip.Replace("-",""),15));//N403 3/15 Postal Code:
							EndSegment(sw);//N404 through N407 are either not used or only required when outside of the United States.						
							//2420E REF: (medical) Ordering Provider Secondary Identification. Situational. Required before NPIs were in effect. We do not use this segment because we require NPI.
							//2420E PER: (medical) Ordering Provider Contact Information. Situational.
							sw.Write("PER"+s
								+"IC"+s//PER01 2/2 Contact Function Code: IC=Information Contact.
								+Sout(PrefC.GetString(PrefName.PracticeTitle),60)+s//PER02 1/60 Name: Practice Title.
								+"TE"+s);//PER03 2/2 Communication Number Qualifier: TE=Telephone.
							if(clinic==null) {
								sw.Write(Sout(PrefC.GetString(PrefName.PracticePhone),256));//PER04 1/256 Communication Number: Telephone number.
							}
							else {
								sw.Write(Sout(clinic.Phone,256));//PER04 1/256 Communication Number: Telephone number.
							}
							EndSegment(sw);//PER05 through PER08 are situational and PER09 is not used. We do not use.
						}
						//2420F NM1: (medical) Referring Provider Name. Situational. We do not use.
						//2420F REF: (medical) Referring Provider Secondary Identification. Situational. We do not use.
						//2420G NM1: PW (medical) Ambulance Pick-up Location. Situational. We do not use.
						//2420G N3: (medical) Ambulance Pick-up Location Address. We do not use.
						//2420G N4: (medical) Ambulance Pick-up Location City, State, Zip Code. We do not use.
						//2420H NM1: (medical) Ambulance Drop-off Location. Situational. We do not use.
						//2420H N3: (medical) Ambulance Drop-off Location Address. We do not use.
						//2420H N4: (medical) Ambulance Drop-off Location City, State, Zip Code. We do not use.
					}
					#endregion 2420 Service Providers (medical)
					#region 2420 Service Providers (inst)
					if(medType==EnumClaimMedType.Institutional) {
						//2420A NM1: 72 (institutional) Operating Physician Name. Situational. Only for surgical procedures.
						if(IsEmdeonMedical(clearinghouseClin)//Do this for Emdeon Medical because we also include Loop 2310B for them.
							&& PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)
							&& claim.ProvTreat!=proc.ProvNum
							&& int.TryParse(claimProcs[j].CodeSent,out int codeSent) && codeSent.Between(10000,69990))//See task #2712788
						{
							provTreatProc=Providers.GetProv(proc.ProvNum);
							WriteNM1Provider("72",sw,provTreatProc.FName,provTreatProc.MI,provTreatProc.LName,provTreatProc.NationalProvID,false);
						}
						//2420A REF: (instititional) Operating Physician Secondary Identification. Situational. Only for surgical procedures. We don't support.						
						//2420B NM1: ZZ (institutional) Other Operating Physician Name. Situational. We don't support.
						//2420B REF: (institutional) Other Operating Physician Secondary Identification. Situational. We don't support.
						if(claim.ProvTreat!=proc.ProvNum
							&& PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)) 
						{
							provTreatProc=Providers.GetProv(proc.ProvNum);
							provClinicProc=ProviderClinics.GetOneOrDefault(proc.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
							//2420C NM1: 82 (institutional) Rendering Provider Name. Situational. Only if different than claim attending (treating) prov. Person only, non-person not allowed.
							WriteNM1Provider("82",sw,provTreatProc.FName,provTreatProc.MI,provTreatProc.LName,provTreatProc.NationalProvID,false);
							//2420C REF: Rendering Provider Secondary Identification. Situational.
							sw.Write("REF"+s
								+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number.
								+Sout((provClinicProc==null ? "" : provClinicProc.StateLicense),50));//REF02 1/50 Reference Identification: Valided to be present.
							EndSegment(sw);//REF03 through REF04 are not used or situational.
						}
						//2420D NM1: DN (institutional) Referring Provider Name. Situational. We do not use.
						//2420D REF: (institutional) Referring Provider Secondary Identification. Situational. We do not use.
					}
					#endregion 2420 Service Providers (inst)
					#region 2420 Service Providers (dental)
					if(medType==EnumClaimMedType.Dental) {
						if(claim.ProvTreat!=proc.ProvNum
							&& PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)) 
						{
							//2420A NM1: 82 (dental) Rendering Provider Name. Only if different from the claim.
							provTreatProc=Providers.GetProv(proc.ProvNum);
							provClinicProc=ProviderClinics.GetOneOrDefault(proc.ProvNum,(clinic==null ? 0 : clinic.ClinicNum));
							WriteNM1Provider("82",sw,provTreatProc);
							//2420A PRV: (dental) Rendering Provider Specialty Information.
							sw.Write("PRV"+s
								+"PE"+s//PRV01 1/3 Provider Code: PE=Performing.
								+"PXC"+s//PRV02 2/3 Reference Identification Qualifier: PXC=Health Care Provider Taxonomy Code.
								+X12Generator.GetTaxonomy(provTreatProc));//PRV03 1/50 Reference Identification: Taxonomy Code.
							EndSegment(sw);//PRV04 through PRV06 not used.
							//2420A REF: (dental) Rendering Provider Secondary Identification. Never required because we always send NPI (validated).
							if(!IsDentiCal(clearinghouseClin)) { //Denti-Cal never wants this.
								if(provClinicProc!=null && provClinicProc.StateLicense!="") {
									sw.Write("REF"+s
										+"0B"+s//REF01 2/3 Reference Identification Qualifier: 0B=State License Number.
										+Sout(provClinicProc.StateLicense,50));//REF02 1/50 Reference Identification: 
									EndSegment(sw);//REF03 1/80 Description: Not used. REF04 Reference Identifier: Situational. Not used when REF01 is 0B or 1G.
								}
							}
						}
						//2420B NM1: DD (dental) Assistant Surgeon Name. Situational. We do not support.
						//2420B PRV: AS (dental) Assistant Surgeon Specialty Information. Situational. We do not support.
						//2420B REF: (dental) Assistant Surgeon Secondary Identification. Situational. We do not support.
						//2420C NM1: DQ (dental) Supervising Provider Name. Situational. We do not support.
						//2420C REF: (dental) Supervising Provider Secondary Identification. Situational. We do not support.
						//2420D NM1: 77 (dental) Service Facility Location Name. Situational. We enforce all procs on a claim being performed at the same location so we don't need this.
						//2420D N3: (dental) Service Facility Location Address. We do not use.
						//2420D N4: (dental) Service Facility Location City, State, Zip Code. We do not use.
						//2420D REF: (dental) Service Facility Location Secondary Identification. Situational. We do not use.
					}
					#endregion 2420 Service Providers (dental)
					//2430 SVD: (medical,institutional,dental) Line Adjudication Information. Situational.  Required when the claim has been previously adjudicated by payer identified in loop 2330B and this service line has payments and/or adjustments applied to it.
					//These SVD segments at the procedure level should add up to the loop 2320 AMT*D (COB Payer Paid Amount).
					//This section of code might work for other clearinghouses, but has not yet been tested, and nobody else has requested this information yet.
					if(hasAdjForOtherPlans && (IsApex(clearinghouseClin) || hasProcedureLevelCob)) {
						foreach(ClaimProc claimProcOther in listOtherClaimProcs[j]) {
							sw.Write("SVD"+s
								+Sout(GetCarrierElectID(otherCarrier,clearinghouseClin),80,2)+s//SVD01 2/80 Identification Code.  Other payer primary identifier.
								+AmountToStrNoLeading(claimProcOther.InsPayAmt)+s//SVD02 1/18 Monetary Amount: Service line paid amount.
								+"AD"+isa16//SVD03-1 2/2 Product/Service ID Qualifier.  Required.
								+procCode.ProcCode+s//SVD03-2 1/48 Product/Service ID.  Procedure Code.  Required.
								//SVD03-3 2/2 Procedure Modifier.  Situational.  We do not use.
								//SVD03-4 2/2 Procedure Modifier.  Situational.  We do not use.
								//SVD03-5 2/2 Procedure Modifier.  Situational.  We do not use.
								//SVD03-6 2/2 Procedure Modifier.  Situational.  We do not use.
								//SVD03-7 2/2 Description. Procedure code description.  Situational.  We do not use.
								//SVD03-8 1/48 Product/Service ID.  Not used.
								+s//SVD04 1/48 Product/Service ID.  Not used.
								+Sout(proc.UnitQty.ToString(),15)//SVD05 1/15 Quantity.  Required.
								//SVD06 1/6 Assigned Number.  Situational.  Bundled or unbundled line number.  We do not use.
							);
							EndSegment(sw);
						}
					}
					//2430 CAS: (medical,institutional,dental) Line Adjustment. Situational. Required when the payer identified in Loop 2330B made line level adjustments which caused the amount paid to differ from the amount originally charged.
					//These CAS segments at the procedure level should add up to their respective claim level 2320 CAS segments.
					//Claim Adjustment Reason Codes can be found on the Washington Publishing Company website at: http://www.wpc-edi.com/reference/codelists/healthcare/claim-adjustment-reason-codes/
					//This section of code might work for other clearinghouses, but has not yet been tested, and nobody else has requested this information yet.
					//See task #2351950: DentalXChange vouched that we can send both claim-level and line-level CAS segments on 01/06/2020.
					if(hasAdjForOtherPlans && (IsApex(clearinghouseClin) || hasProcedureLevelCob)) {
						double procPatientPortionAmt=Math.Max(0,claimProcs[j].FeeBilled-listProcWriteoffAmts[j]-listProcDeductibleAmts[j]-listProcPaidOtherInsAmts[j]);
						//ClaimConnect sometimes expects zero value contractual obligations for line adjustments. Excluding them can cause an error on their end.
						if(listProcWriteoffAmts[j]>0 || (IsClaimConnect(clearinghouseClin) && claimWriteoffAmt>0)) {
							sw.Write("CAS"+s
								+"CO"+s//CAS01 1/2 Claim Adjustment Group Code: CO=Contractual Obligations.
								+"45"+s//CAS02 1/5 Claim Adjustment Reason Code: 45=Charge exceeds fee schedule/maximum allowable or contracted/legislated fee arrangement.
								+AmountToStrNoLeading(listProcWriteoffAmts[j]));//CAS03 1/18 Monetary Amount:
							EndSegment(sw);
						}
						if(listProcDeductibleAmts[j]>0) {
							sw.Write("CAS"+s
								+"PR"+s//CAS01 1/2 Claim Adjustment Group Code: PR=Patient Responsibility.
								+"1"+s//CAS02 1/5 Claim Adjustment Reason Code: 1=Deductible.
								+AmountToStrNoLeading(listProcDeductibleAmts[j])+s//CAS03 1/18 Monetary Amount:
								+"1");//CAS04 1/15 Quantity:
							EndSegment(sw);
						}
						if(procPatientPortionAmt>0) {
							sw.Write("CAS"+s
								+"PR"+s//CAS01 1/2 Claim Adjustment Group Code: PR=Patient Responsibility.
								+"3"+s//CAS02 or CAS05 1/5 Claim Adjustment Reason Code: 3=Co-payment Amount.
								+AmountToStrNoLeading(procPatientPortionAmt));//CAS03 or CAS06 1/18 Monetary Amount:
							EndSegment(sw);
						}
					}
					//2430 DTP: (medical,institutional,dental) Line Check or Remittance Date.  Required.
					//Apex has not required this segment in the past and we will not send it to them until they require it of us.
					if(hasAdjForOtherPlans && !IsApex(clearinghouseClin) && hasProcedureLevelCob) {
						//When selecting date, include dates for Total Payments on any other claim that the current proc has completed claimprocs on.
						List<long> listOtherClaimNumsProcCur=claimProcList.FindAll(x => x.ClaimNum!=claim.ClaimNum
							&& ListTools.In(x.Status,ClaimProcStatus.CapClaim,ClaimProcStatus.Received,ClaimProcStatus.Supplemental)
							&& x.ProcNum==proc.ProcNum).Select(x => x.ClaimNum).Distinct().ToList();
						List<ClaimProc> listTotalPaymentClaimprocsForProcCur=claimProcList.FindAll(x => listOtherClaimNumsProcCur
							.Contains(x.ClaimNum) && x.ProcNum==0);
						List<ClaimProc> listOtherClaimProcsForProcCur=listOtherClaimProcs.SelectMany(x => x).Where(x => x.ProcNum==proc.ProcNum)
							.Concat(listTotalPaymentClaimprocsForProcCur).ToList();
						DateTime datePaidOtherInsProcCur=DateTime.MinValue;
						if(listOtherClaimProcsForProcCur.Count > 0) {
							//This is rare and due to unusual workflow.  Create a primary claim with two procs.
							//Split the claim, then receive one of them.  Create a secondary claim with both procs on it and send it.
							//When sending, the list we are calling Max() against is empty.
							//The real issue is, why did the user send a secondary claim containing procs from two different primary claims?
							//We allow the secondary claim to send,
							//but we set the date paid to 0001/01/01 to let the insurance company know that the claim is bogus.
							datePaidOtherInsProcCur=listOtherClaimProcsForProcCur.Max(x => x.DateCP);
						}
						sw.Write("DTP"+s
							+"573"+s//DTP01 3/3 Date/Time Qualifier: 573=Date Claim Paid.
							+"D8"+s//DTP02 2/3 Date Time Period Format Qualifier: D8=Date Expressed in Format CCYYMMDD.
							+datePaidOtherInsProcCur.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period:
						EndSegment(sw);
					}
					//2430 AMT: (medical,institutional,dental) Remaining Patient Liability. We do not support.
					//2440 LQ: (medical) Form Identification Code. Situational. We do not use.
					//2440 FRM: (medical) Supporting Documentation. We do not use.
				}
			}
			#region Trailers
			//Transaction Trailer
			sw.Write("SE"+s
				+(seg+1).ToString()+s//SE01 1/10 Number of Included Segments: Total segments, including ST & SE. We add 1 for this SE segment, since the seg variable is not incremented until after this segment is written.
				+transactionNum.ToString().PadLeft(4,'0'));//SE02 4/9 Transaction Set Control Number:
			EndSegment(sw);
			//Functional Group Trailer
			sw.Write("GE"+s+transactionNum.ToString()+s//GE01 1/6 Number of Transaction Sets Included.  Always 1 for us.
				+groupControlNumber//GE02 1/9 Group Control Number: Must be identical to GS06.
				+endSegment);
			#endregion Trailers
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsApex(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="99999");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsClaimConnect(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="330989922");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsColoradoMedicaid(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="100000" && clearinghouse.GS03=="77016");
		}

		///<summary>DentiCal is a carrier.  Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsDentiCal(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="DENTICAL");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsEmdeonDental(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="0135WCH00");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsEmdeonMedical(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="133052274");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsEMS(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="EMS");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsETACTICS(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="ETACTICSINC");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsInmediata(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="660610220");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsLindsayTechnicalConsultants(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="LTC");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsOfficeAlly(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="330897513");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsEDS(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="EDS");
		}

		///<summary>Contact information for this carrier is: (800)578-0775, P.O. BOX 7114 London Kentucky 40742-7114.
		///Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsPassportHealthMedicaid(Clearinghouse clearinghouse,Carrier carrier) {
			return (clearinghouse.ISA08=="61129" || carrier.ElectID=="61129");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsTesia(Clearinghouse clearinghouse) {
			return (clearinghouse.ISA08=="113504607");
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static bool IsWashingtonMedicaid(Clearinghouse clearinghouse,Carrier carrier) {
			return ((clearinghouse.ISA08=="77045" && clearinghouse.ISA02=="00") || carrier.ElectID=="CKWA1" || carrier.ElectID=="77045");
		}

		///<summary>Sometimes writes the name information for Open Dental. Sometimes it writes practice info.</summary>
		private static void Write1000A_NM1(StreamWriter sw,Clearinghouse clearinghouseClin) {
			string name="OPEN DENTAL SOFTWARE";
			string idCode="810624427";
			if(IsEmdeonMedical(clearinghouseClin)) {
				name="Open Dental Software Inc.";
				idCode="204944520";
			}
			if(clearinghouseClin.SenderTIN!="") {
				name=clearinghouseClin.SenderName;
				idCode=clearinghouseClin.SenderTIN;
			}
			sw.Write("NM1"+s
				+"41"+s//NM101 2/3 Entity Indentifier Code: 41=submitter
				+"2"+s//NM102 1/1 Entity Type Qualifier: 2=Non-Person Entity.
				+Sout(name,60)+s//NM103 1/60 Name Last or Organization Name: 
				+s//NM104 1/35 Name First: Situational.
				+s//NM105 1/25 Name Middle: Situational.
				+s//NM106 1/10 Name Prefix: Not Used.
				+s//NM107 1/10 Name Suffix: Not Used.
				+"46"+s//NM108 1/2 Identification Code Qualifier: 46=Electronic Transmitter Identification Number (ETIN).
				+Sout(idCode,80,2));//NM109 2/80 Identification Code: ETIN#. Validated to be at least 2.
			EndSegment(sw);//NM110 through NM112 are not used.
		}

		///<summary>Usually writes the contact information for Open Dental. But for inmediata and AOS clearinghouses, it writes practice contact info.</summary>
		private static void Write1000A_PER(StreamWriter sw,Clearinghouse clearinghouseClin) {
			string name="OPEN DENTAL SOFTWARE";
			string phone="8776861248";
			if(IsEmdeonMedical(clearinghouseClin)) {
				name="Open Dental Software Inc.";
				phone="8662390469";
			}
			if(clearinghouseClin.SenderTIN!="") {
				name=clearinghouseClin.SenderName;
				phone=clearinghouseClin.SenderTelephone;
			}
			sw.Write("PER"+s
				+"IC"+s);//PER01 2/2 Contact Function Code: IC=Information Contact.
			//The following clearinghouses always want PER02, even though the X12 speficiation says not to send if same as NM103 of loop 1000A. They don't seem to care if the value is the same as NM103.
			if(IsClaimConnect(clearinghouseClin) || IsEmdeonMedical(clearinghouseClin)) {
				sw.Write(Sout(name,60)+s);//PER02 1/60 Name: Situational.
			}
			else {
				sw.Write(s);//PER02 1/60 Name: Situational. Do not send since same as NM103 for loop 1000A.
			}
			sw.Write("TE"+s//PER03 2/2 Communication Number Qualifier: TE=Telephone.
				+phone);//PER04 1/256 Communication Number: Telephone Number. Validated to be exactly 10 digits.
			EndSegment(sw);//PER05 through PER08 are situational. We do not use. PER09 is not used.
		}

		///<summary>Generates SiteID REF G5 for Emdeon only.</summary>
		private static void Write2010AASiteIDforEmdeon(StreamWriter sw,Provider prov,string payorID) {
			ProviderIdent[] provIdents=ProviderIdents.GetForPayor(prov.ProvNum,payorID);
			for(int i=0;i<provIdents.Length;i++) {
				if(provIdents[i].SuppIDType==ProviderSupplementalID.SiteNumber) {
					sw.Write("REF"+s
						+"G5"+s//REF01 2/3 Reference Identification Qualifier: 
						+Sout(provIdents[i].IDNumber,50));//REF02 1/50 Reference Identification:
					EndSegment(sw);//REF03 and REF04 are not used.
				}
			}
		}

		///<summary>This is depedent only on the electronic payor id # rather than the clearinghouse. Used for billing prov and also for treating prov.</summary>
		private static void WriteProv_REFG2orLU(StreamWriter sw,Provider prov,string payorID) {
			string segmentType="G2";
			string provID="";
			ElectID electID=ElectIDs.GetID(payorID);
			if(electID!=null && electID.IsMedicaid) {
				provID=prov.MedicaidID;
			}
			else {
				ProviderIdent[] provIdents=ProviderIdents.GetForPayor(prov.ProvNum,payorID);
				//Should always return 1 or 0 values unless user set it up wrong.
				if(provIdents.Length>0) {
					if(provIdents[0].SuppIDType==ProviderSupplementalID.SiteNumber) {
						segmentType="LU";
					}
					provID=provIdents[0].IDNumber;
				}
			}
			if(provID!="") {
				sw.Write("REF"+s
		      +segmentType+s//REF01 2/3 Reference Identification Qualifier: G2=all payers including Medicare, Medicaid, Blue Cross, etc. LU=Location Number.
		      +Sout(provID,50));//REF02 1/50 Reference Identification:
		    EndSegment(sw);//REF03 and REF04 are not used.
			}
		}

		///<summary>A referring provider.  The loop has different numbers depending on med/inst/dent.</summary>
		private static void WriteNM1_DN(StreamWriter sw,long referringProv) {
			Referral referral;
			Referrals.TryGetReferral(referringProv,out referral);
			if(referral==null || !referral.IsDoctor || referral.NotPerson) {//Could be null if referringProv=0 (common)
				return;
			}
			WriteNM1Provider("DN",sw,referral.FName,referral.MName,referral.LName,referral.NationalProvID,false);
		}

		///<summary>A generic function that is reused because there are many identical provider NM1 segments within the X12 specification.</summary>
		private static void WriteNM1Provider(string entityIdentifierCode,StreamWriter sw,Provider prov) {
			WriteNM1Provider(entityIdentifierCode,sw,prov.FName,prov.MI,prov.LName,prov.NationalProvID,prov.IsNotPerson);
		}

		///<summary>A generic function that is reused because there are many identical provider NM1 segments within the X12 specification.</summary>
		private static void WriteNM1Provider(string entityIdentifierCode,StreamWriter sw,string FName,string middleName,string LName,string NPI,bool isNotPerson) {
			if(LName=="") {
				return;
			}
			sw.Write("NM1"+s
				+entityIdentifierCode+s//NM101 2/3 Entity Identifier Code: Used to identify the type of provider being specified.
				+(isNotPerson?"2":"1")+s//NM102 1/1 Entity Type Qualifier: 1=Person, 2=Non-Person.
				+Sout(LName,60));//NM103 1/60 Name Last or Organization Name:
			if((FName!="" && !isNotPerson) || (middleName!="" && !isNotPerson) || NPI.Length>1) {
				sw.Write(s//end of NM103.
					+(isNotPerson?"":Sout(FName,35)));//NM104 1/35 Name First:
			}
			if((middleName!="" && !isNotPerson) || NPI.Length>1) {
				sw.Write(s//end of NM104.
					+(isNotPerson?"":Sout(middleName,25)));//NM105 1/25 Name Middle:
			}
			if(NPI.Length>1) {
				sw.Write(s//end of NM105.
					+s//NM106 1/10 Name Prefix: Not Used.
					+s//NM107 1/10 Name Suffix: We don't support.
					+"XX"+s//NM108 1/2 Identification Code Qualifier: Situational. Required since after the HIPAA date. XX=NPI.
					+Sout(NPI,80));//NM109 2/80 Identification Code:  NPI validated.
			}
			EndSegment(sw);//NM110 through NM112 are not used.
		}

		///<summary>Rendering provider specialty information. The loop has different numbers in med/dent. Not used in inst.</summary>
		private static void WritePRV_PE(StreamWriter sw,Provider provTreat) {
			//2310B PRV: PE (dental) Rendering Provider Specialty Information.
			sw.Write("PRV"+s
				+"PE"+s//PRV01 1/3 Provider Code: PE=Performing.
				+"PXC"+s//PRV02 2/3 Reference Identification Qualifier: PXC=Health Care Provider Taxonomy Code.
				+X12Generator.GetTaxonomy(provTreat));//PRV03 1/50 Reference Identification: Taxonomy Code.
			EndSegment(sw);//PRV04 through PRV06 are not used.
		}

		private static void Write2300DTP_Onset(StreamWriter sw,Claim claim,string dtp01) {
			if(claim.DateIllnessInjuryPreg.Year>1880 && dtp01==((int)claim.DateIllnessInjuryPregQualifier).ToString()) {
				sw.Write("DTP"+s
					+((int)claim.DateIllnessInjuryPregQualifier).ToString()+s//DTP01 3/3 Date/Time Qualifier: 431=Onset of acute symptoms or 484=last menstrual period
					+"D8"+s//DTP02 2/3 Date Time Period Format Qualifer: D8=Date Expressed in Format CCYYMMDD.
					+claim.DateIllnessInjuryPreg.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period
				EndSegment(sw);
			}
		}

		private static void Write2300DTP_Other(StreamWriter sw,Claim claim,string dtp01) {
			if(claim.DateOther.Year>1880 && dtp01==((int)claim.DateOtherQualifier).ToString("000")) {
				sw.Write("DTP"+s
					+((int)claim.DateOtherQualifier).ToString("000")+s//DTP01 3/3 Date/Time Qualifier: 090,091,304,444,453,454,455,471
					+"D8"+s//DTP02 2/3 Date Time Period Format Qualifer: D8=Date Expressed in Format CCYYMMDD.
					+claim.DateOther.ToString("yyyyMMdd"));//DTP03 1/35 Date Time Period
				EndSegment(sw);
			}
		}

		///<summary>Writes the segment terminator and a newline char to the stream and then increments the segment count.</summary>
		private static void EndSegment(StreamWriter sw) {
			sw.Write(endSegment);
			seg++;
		}

		///<summary>Pass in either a clinic or HQ-level clearinghouse.</summary>
		private static string GetCarrierElectID(Carrier carrier,Clearinghouse clearinghouse) {
			string electid=carrier.ElectID;
			if(electid=="" && IsApex(clearinghouse)) {//only for Apex
				return "PAPRM";//paper claims
			}
			if(electid=="" && IsTesia(clearinghouse)) {//only for Tesia
				return "00000";//paper claims
			}
			if(electid.Length<3) {
				return "06126";//paper claims
			}
			return electid;
		}

		private static string GetGender(PatientGender patGender) {
			switch(patGender) {
				case PatientGender.Male:
					return "M";
				case PatientGender.Female:
					return "F";
				case PatientGender.Unknown:
					return "U";
			}
			return "";
		}

		///<summary>01 Spouse, 18 Self, 19 Child, 20 Employee, 21 Unknown, 39 Organ Donor, 40 Cadaver Donor, 53 Life Partner, G8 Other Relationship.</summary>
		private static string GetRelat(Relat relat) {
			switch(relat) {
				case (Relat.Self):
					return "18";
				case (Relat.Child):
					return "19";
				case (Relat.Dependent):
					return "G8";//Other relationship
				case (Relat.Employee):
					return "20";
				case (Relat.HandicapDep):
					return "G8";//Other relationship
				case (Relat.InjuredPlaintiff):
					return "G8";//Other relationship
				case (Relat.LifePartner):
					return "53";
				case (Relat.SignifOther):
					return "G8";//Other relationship
				case (Relat.Spouse):
					return "01";
			}
			return "";
		}

		private static string GetStudentEmdeon(string studentStatus) {
			if(studentStatus=="P") {
				return "P";
			}
			if(studentStatus=="F") {
				return "F";
			}
			return "N";//either N or blank
		}

		private static string GetRelatedCauses(Claim claim) {
			if(claim.AccidentRelated=="") {
				return "";
			}
			//even though the specs let you submit all three types at once, we only allow one of the three
			if(claim.AccidentRelated=="A") {//auto accident
				return "AA"+isa16+isa16+isa16+Sout(claim.AccidentST,2,2);
			}
			else if(claim.AccidentRelated=="E") {//employment
				return "EM";
			}
			else {// if(claim.AccidentRelated=="O"){ //other accident
				return "OA";
			}
		}

		///<summary>Will return blank if no special code.</summary>
		private static string GetSpecialProgramCode(EnumClaimSpecialProgram code) {
			switch(code){
				default:
					return "";
				case EnumClaimSpecialProgram.EPSDT_1:
					return "01";//only valid for dental.
				case EnumClaimSpecialProgram.Handicapped_2:
					return "02";
				case EnumClaimSpecialProgram.SpecialFederal_3:
					return "03";
				case EnumClaimSpecialProgram.Disability_5:
					return "05";
				case EnumClaimSpecialProgram.SecondOpinion_9:
					return "09";//only valid for medical.
			}
		}		

		///<summary>This used to be an enumeration.</summary>
		private static string GetFilingCode(InsPlan plan) {
			string filingcode=InsFilingCodes.GetEclaimCode(plan.FilingCode);
			//must be one or two char in length.
			if(filingcode=="" || filingcode.Length>2) {
				return "CI";
			}
			return Sout(filingcode,2,1);
			/*
			switch(plan.FilingCode){
				case InsFilingCodeOld.SelfPay:
					return "09";
				case InsFilingCodeOld.OtherNonFed:
					return "11";
				case InsFilingCodeOld.PPO:
					return "12";
				case InsFilingCodeOld.POS:
					return "13";
				case InsFilingCodeOld.EPO:
					return "14";
				case InsFilingCodeOld.Indemnity:
					return "15";
				case InsFilingCodeOld.HMO_MedicareRisk:
					return "16";
				case InsFilingCodeOld.DMO:
					return "17";
				case InsFilingCodeOld.BCBS:
					return "BL";
				case InsFilingCodeOld.Champus:
					return "CH";
				case InsFilingCodeOld.Commercial_Insurance:
					return "CI";
				case InsFilingCodeOld.Disability:
					return "DS";
				case InsFilingCodeOld.FEP:
					return "FI";
				case InsFilingCodeOld.HMO:
					return "HM";
				case InsFilingCodeOld.LiabilityMedical:
					return "LM";
				case InsFilingCodeOld.MedicarePartB:
					return "MB";
				case InsFilingCodeOld.Medicaid:
					return "MC";
				case InsFilingCodeOld.ManagedCare_NonHMO:
					return "MH";
				case InsFilingCodeOld.OtherFederalProgram:
					return "OF";
				case InsFilingCodeOld.SelfAdministered:
					return "SA";
				case InsFilingCodeOld.Veterans:
					return "VA";
				case InsFilingCodeOld.WorkersComp:
					return "WC";
				case InsFilingCodeOld.MutuallyDefined:
					return "ZZ";
				default:
					return "CI";
			}
			*/
		}

		///<summary>Used in SV304 (dental only).</summary>
		private static string GetArea(Procedure proc,ProcedureCode procCode) {
			//"Required when the nomenclature associated with the procedure reported in SV301-2 refers to quadrant or arch
			//and the area of the oral cavity is not uniquely defined by the procedure description.
			//Report individual tooth numbers in one or more TOO segments.
			//Do not use this element for reporting of individual teeth.
			//If it is necessary to report one or more individual teeth, use the Tooth Information (TOO) segment in this loop."
			if(procCode.TreatArea==TreatmentArea.Arch) {
				if(proc.Surf=="U") {
					return "01";
				}
				if(proc.Surf=="L") {
					return "02";
				}
			}
			if(procCode.TreatArea==TreatmentArea.Mouth){
				return "00";
			}
			if(procCode.TreatArea==TreatmentArea.None){
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Quad) {
				if(proc.Surf=="UR") {
					return "10";
				}
				if(proc.Surf=="UL") {
					return "20";
				}
				if(proc.Surf=="LR") {
					return "40";
				}
				if(proc.Surf=="LL") {
					return "30";
				}
			}
			if(procCode.TreatArea==TreatmentArea.Sextant) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Surf) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.Tooth) {
				return "";
			}
			if(procCode.TreatArea==TreatmentArea.ToothRange) {
				return "";
			}
			return "";
		}

		///<summary></summary>
		public static string GetDrugUnitCode(EnumProcDrugUnit drugUnit){
			switch(drugUnit){
				//case EnumProcDrugUnit.None://validated so it won't happen
				case EnumProcDrugUnit.Gram:
					return "GR";
				case EnumProcDrugUnit.InternationalUnit:
					return "F2";
				case EnumProcDrugUnit.Milligram:
					return "ME";
				case EnumProcDrugUnit.Milliliter:
					return "ML";
				case EnumProcDrugUnit.Unit:
					return "UN";
				default:
					return "UN";//just in case
			}
		}

		///<summary>Returns null if not found.  The logic for getting the ordering provider is complex.
		///This helper function simplifies getting the ordering provider so we can ensure the same logic for validation and eclaim output.</summary>
		private static Provider GetOrderingProviderForProc(Claim claim,Carrier carrier,Procedure proc,Provider provTreatProc) {
			Provider provOrderProc=null;
			if(proc.ProvOrderOverride!=0) {
				provOrderProc=Providers.GetProv(proc.ProvOrderOverride);//Override ordering provider at procedure level.
			}
			else if(proc.OrderingReferralNum!=0) {
				Referral referralOrdering=null;
				if(Referrals.TryGetReferral(proc.OrderingReferralNum,out referralOrdering)) {
					provOrderProc=new Provider();
					provOrderProc.IsNotPerson=referralOrdering.NotPerson;
					provOrderProc.Abbr="Referral: "+referralOrdering.GetNameFL();//Only shows to user if validation failed.
					provOrderProc.FName=referralOrdering.FName;
					provOrderProc.MI=referralOrdering.MName;
					provOrderProc.LName=referralOrdering.LName;
					provOrderProc.NationalProvID=referralOrdering.NationalProvID;
				}
			}
			else if(claim.ProvOrderOverride!=0) {
				provOrderProc=Providers.GetProv(claim.ProvOrderOverride);//Override ordering provider at claim level.
			}
			else if(claim.OrderingReferralNum!=0) {
				Referral referralOrdering=null;
				if(Referrals.TryGetReferral(claim.OrderingReferralNum,out referralOrdering)) {
					provOrderProc=new Provider();
					provOrderProc.IsNotPerson=referralOrdering.NotPerson;
					provOrderProc.Abbr="Referral: "+referralOrdering.GetNameFL();//Only shows to user if validation failed.
					provOrderProc.FName=referralOrdering.FName;
					provOrderProc.MI=referralOrdering.MName;
					provOrderProc.LName=referralOrdering.LName;
					provOrderProc.NationalProvID=referralOrdering.NationalProvID;
				}
			}
			else if(PrefC.GetBool(PrefName.ClaimMedProvTreatmentAsOrdering)) {
				provOrderProc=provTreatProc;//Even though using the treating provider is against the standard.  This is the old solution.
			}
			else if(Carriers.IsMedicaid(carrier)) {//Emdeon Medical requires loop 2420E when the claim is sent to DMERC (Medicaid) carriers.
				provOrderProc=provTreatProc;//Even though using the treating provider is against the standard, it works in this scenario.
			}
			return provOrderProc;
		}

		///<summary>X12 "String" output scrubber, for data element type "AN".  Converts any string to an acceptable format for X12.
		///Converts to all caps and strips off all invalid characters.
		///Optionally shortens the string to the specified length
		///and/or makes sure the string is long enough by padding with spaces.</summary>
		private static string Sout(string inputStr,int maxL=-1,int minL=-1,bool hasUnderscores=false) {
			//The "Basic Character Set" is described in the standard on page 387 as: A...Z 0...9 ! & ( ) + * , - . / : ; ? = (space)
			//The "Extended Character Set" is described in the standard on page 387 as: a...z % ~ @ [ ] _ { } \ | < > # $
			//An X12 "String" is defined on page 393 as: "A string data element is a sequence of any characters from the basic or extended character sets."
			string retStr=inputStr.ToUpper();
			retStr=retStr.Replace(s,"");//Remove any instances of data element separator in inputStr to protect integrity of overall output.
			retStr=retStr.Replace(isa16,"");//Remove any instances of data component separator in inputStr to protect integrity of overall output.  Example: " for Dentical.
			retStr=retStr.Replace(endSegment.Substring(0),"");//Remove any instances of segment separator in inputStr to protect integrity of overall output. The endSegment has \r\n tacked on end, so we only want Substring(0).
			retStr=Regex.Replace(retStr,//replaces characters in this input string
				//Allowed: !"&'()+,-./;?=(space)#   # is actually part of extended character set. For Denti-Cal, the component element separator is double-quote, which is removed above.
				"[^\\w!\"&'\\(\\)\\+,-\\./;\\?= #]",//[](any single char)^(that is not)\w(A-Z or 0-9) or one of the above chars.
				"");//\w is "word character" https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-classes-in-regular-expressions#WordCharacter
			//We have been removing '_' since 08/2011 (7 years) without issue until now.
			//One customer had an issue with GS03 and ISA08 requiring an '_'.  The standard says '_' is allowed as part of the extended character set.
			//For backwards compatibility and to fix the issue with GS03 and ISA08,
			//we have decided to allow the caller of Sout to decide if they would like to include '_' via the hasUnderscores flag.
			if(!hasUnderscores) {
				retStr=Regex.Replace(retStr,"[_]","");//replaces _
			}
			retStr=retStr.Trim();//removes leading and trailing spaces.
			if(maxL!=-1) {
				if(retStr.Length>maxL) {
					retStr=retStr.Substring(0,maxL);
				}
			}
			if(minL!=-1) {
				if(retStr.Length<minL) {
					retStr=retStr.PadRight(minL,' ');
				}
			}
			//Debug.WriteLine(retStr);
			return retStr;
		}

		///<summary>Fills the missing data field on the queueItem that was passed in.  This contains all missing data on this claim.
		///Claim will not be allowed to be sent electronically unless this string comes back empty.
		///Also fills the warnings field on the queueItem that was passed in.  Warnings will not block sending.</summary>
		public static void Validate(Clearinghouse clearinghouseClin,ClaimSendQueueItem queueItem){//,out string warning) {
			StringBuilder strb=new StringBuilder();
			string warning="";
			if(clearinghouseClin==null) {
				Comma(strb);
				strb.Append("Clearinghouse not found.");
				queueItem.MissingData=strb.ToString();
				return;
			}
			Claim claim=Claims.GetClaim(queueItem.ClaimNum);
			//If office turns the clinic feature on and creates claim then turns the feature off and tries to send it later
			//then we do not want to use the claims.ClaimNum for validation reasons, we want to use HQ settings.
			Clinic clinic=null;
			if(PrefC.HasClinicsEnabled) {
				clinic=Clinics.GetClinic(claim.ClinicNum);
			}
			//if(clearhouse.Eformat==ElectronicClaimFormat.X12){//not needed since this is always true
			X12Validate.ISA(clearinghouseClin,strb);
			if(clearinghouseClin.GS03.Length<2) {
				Comma(strb);
				strb.Append("Clearinghouse GS03");
			}
			List<X12TransactionItem> claimItems=Claims.GetX12TransactionInfo(((ClaimSendQueueItem)queueItem).ClaimNum);//just to get prov. Needs work.
			if(claimItems.Count==0) {
				Comma(strb);
				strb.Append("Claim not found.");
				queueItem.MissingData=strb.ToString();
				return;
			}
			Provider providerFirst=Providers.GetFirst();//Used in order to preserve old behavior...  If this fails, then old code would have failed.
			Provider billProv=Providers.GetFirstOrDefault(x => x.ProvNum==claimItems[0].ProvBill1)??providerFirst;
			Provider treatProv=Providers.GetFirstOrDefault(x => x.ProvNum==claim.ProvTreat)??providerFirst;
			Referral referral;
			Referrals.TryGetReferral(claim.ReferringProv,out referral);
			InsPlan insPlan=InsPlans.GetPlan(claim.PlanNum,null);
			InsSub sub=InsSubs.GetSub(claim.InsSubNum,null);
			List<PatPlan> patPlans=PatPlans.Refresh(claim.PatNum);
			if(claim.MedType==EnumClaimMedType.Medical) {
				if(referral!=null && referral.IsDoctor && referral.NotPerson) {
					Comma(strb);
					strb.Append("Referring Prov must be a person.");
				}
			}
			else if(claim.MedType==EnumClaimMedType.Institutional) {
				if(referral!=null && referral.IsDoctor && referral.NotPerson && claim.ReferringProv!=claim.ProvTreat) {
					Comma(strb);
					strb.Append("Referring Prov must be a person.");
				}
				if(!billProv.IsNotPerson) {
					Comma(strb);
					strb.Append("Billing Prov cannot be a person.");
				}
			}
			else if(claim.MedType==EnumClaimMedType.Dental) {
				if(referral!=null && referral.IsDoctor && referral.NotPerson) {
					Comma(strb);
					strb.Append("Referring Prov must be a person.");
				}
			}
			//billProv
			X12Validate.BillProv(billProv,strb);
			if(IsEmdeonMedical(clearinghouseClin)) {//The X12 standard has a basic character set, but Emdeon Medical only allows a subset of the basic character set, as seen in error messages within their interface.
				if(!billProv.IsNotPerson && !Regex.IsMatch(billProv.FName,"^[A-Za-z ']+$")) {//If not a person, then X12 generation will leave this blank, regardless of what user entered.
					Comma(strb);
					strb.Append("Billing Prov FName may contain letters spaces and apostrophes only");
				}
				if(!Regex.IsMatch(billProv.LName,"^[A-Za-z ']+$")) {
					Comma(strb);
					strb.Append("Billing Prov LName may contain letters spaces and apostrophes only");
				}
				if(!billProv.IsNotPerson && !Regex.IsMatch(billProv.MI,"^[A-Za-z ']*$")) {//If not a person, then X12 generation will leave this blank, regardless of what user entered.
					Comma(strb);
					strb.Append("Billing Prov Middle Name may contain letters spaces and apostrophes only");
				}
			}
			if(clinic!=null && clinic.UseBillAddrOnClaims) {
				X12Validate.Clinic(clinic,strb);//Tests for 5 or 9 digit zip.
				if(Regex.IsMatch(clinic.BillingZip,"^[0-9]{5}\\-?$")) {
					//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
					Comma(strb);
					strb.Append("Clinic billing zip must contain nine digits.");
				}
				if(HasPOBox(clinic.BillingAddress)) {
					Comma(strb);
					strb.Append("Clinic billing address cannot be a P.O. BOX when used for e-claims.");
				}
			}
			else if(PrefC.GetBool(PrefName.UseBillingAddressOnClaims)) {
				X12Validate.BillingAddress(strb);//Tests for 5 or 9 digit zip.
				string zip=PrefC.GetString(PrefName.PracticeBillingZip);
				if(Regex.IsMatch(zip,"^[0-9]{5}\\-?$")) {//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
					Comma(strb);
					strb.Append("Practice billing zip must contain nine digits");//this is more restrictive than in the check above.
				}
				if(HasPOBox(PrefC.GetString(PrefName.PracticeBillingAddress))) {
					Comma(strb);
					strb.Append("Practice billing address cannot be a P.O. BOX when used for e-claims.");
				}
			}
			else if(clinic==null) {
				X12Validate.PracticeAddress(strb);//Tests for 5 or 9 digit zip.
				string zip=PrefC.GetString(PrefName.PracticeZip);
				if(Regex.IsMatch(zip,"^[0-9]{5}\\-?$")) {//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
					Comma(strb);
					strb.Append("Practice zip must contain nine digits");
				}
				if(HasPOBox(PrefC.GetString(PrefName.PracticeAddress))) {
					Comma(strb);
					strb.Append("Practice address cannot be a P.O. BOX when used for e-claims.");
				}
			}
			else { //Using a clinic
				X12Validate.Clinic(clinic,strb);//Tests for 5 or 9 digit zip.
				if(Regex.IsMatch(clinic.Zip,"^[0-9]{5}\\-?$")) {
					//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
					Comma(strb);
					strb.Append("Clinic zip must contain nine digits.");
				}
				if(HasPOBox(clinic.Address)) {
					Comma(strb);
					strb.Append("Clinic address cannot be a P.O. BOX when used for e-claims.");
				}
			}
			if(clinic==null || (clinic!=null && clinic.PayToAddress=="")) {//No clinic Pay To address.
				//If PayTo options exist, we're using them always for PayTo information, so always check them for validity.
				//We don't check the PayToAddress for validation because simply not being blank is good enough.
				if(PrefC.GetString(PrefName.PracticePayToAddress).Trim()!="") {
					if(PrefC.GetString(PrefName.PracticePayToCity).Trim().Length<2) {
						Comma(strb);
						strb.Append("Practice Pay To City");
					}
					if(PrefC.GetString(PrefName.PracticePayToST).Trim().Length!=2) {
						Comma(strb);
						strb.Append("Practice Pay To State");
					}
					if(Regex.IsMatch(PrefC.GetString(PrefName.PracticePayToZip),"^[0-9]{5}\\-?$")) {
						//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
						Comma(strb);
						strb.Append("Practice Pay To zip must contain nine digits");
					}
				}
			}
			else {//Clinic Pay To address is present.
				//If PayTo options exist, we're using them always for PayTo information, so always check them for validity.
				if(clinic.PayToCity.Trim().Length<2) {
					Comma(strb);
					strb.Append("Clinic Pay To City");
				}
				if(clinic.PayToState.Trim().Length!=2) {
					Comma(strb);
					strb.Append("Clinic Pay To State");
				}
				if(Regex.IsMatch(clinic.PayToZip.Trim(),"^[0-9]{5}\\-?$")) {
					//If the zip is 5 digits in format ##### or #####-, then it passed the first test, but 9 digits zips are required.
					Comma(strb);
					strb.Append("Clinic Pay To zip must contain nine digits");
				}
			}
			//treatProv
			if(treatProv.LName=="") {
				Comma(strb);
				strb.Append("Treating Prov LName");
			}
			if(treatProv.FName=="" && !treatProv.IsNotPerson) {
				Comma(strb);
				strb.Append("Treating Prov FName");
			}
			if(treatProv.NationalProvID.Length<2) {
				Comma(strb);
				strb.Append("Treating Prov NPI");
			}
			if(treatProv.TaxonomyCodeOverride.Length>0 && treatProv.TaxonomyCodeOverride.Length!=10) {
				Comma(strb);
				strb.Append("Treating Prov Taxonomy Code must be 10 characters");
			}
			//Treating prov SSN/TIN is not sent on paper or eclaims. Do not verify or block.
			if(!Regex.IsMatch(treatProv.NationalProvID,"^(80840)?[0-9]{10}$")) {
				Comma(strb);
				strb.Append("Treating Prov NPI for claim must be a 10 digit number with an optional prefix of 80840");
			}
			//facility provider, used for facility name and facility NPI
			Provider facilityProv=billProv;
			if(claim.MedType==EnumClaimMedType.Dental) {
				if(claim.PlaceService!=PlaceOfService.Office) {
					//Only specific clearinghouses want the facility information.
					if(IsClaimConnect(clearinghouseClin)) {
						if(!facilityProv.IsNotPerson) {//In medical, institutional and dental, the facility provider must be a non-person.
							Comma(strb);
							strb.Append("Billing/Facility Prov cannot be a person when claim Place of Service is not Office");
						}
					}
				}
			}
			//Addresses
			if(PrefC.GetString(PrefName.PracticeTitle)=="") {
				Comma(strb);
				strb.Append("Practice Title");
			}
			if(!sub.ReleaseInfo) {
				Comma(strb);
				strb.Append("InsPlan Release of Info");
			}
			Carrier carrier=Carriers.GetCarrier(insPlan.CarrierNum);
			PatPlan patPlan=PatPlans.GetFromList(patPlans,claim.InsSubNum);//can be null
			if(patPlan!=null && patPlan.PatID!="") {
				Comma(strb);
				strb.Append("Create a new insurance plan instead of using the optional patient ID");
			}
			if(IsDentiCal(clearinghouseClin)) {
				if(GetFilingCode(insPlan)!="MC") {
					Comma(strb);
					strb.Append("InsPlan Filing Code must be Medicaid for Denti-Cal");
				}
				if(sub.Subscriber!=claim.PatNum
					&& !Patients.ArePatientsClonesOfEachOther(claim.PatNum,sub.Subscriber))//and the patient is not a clone of the subscriber
				{ 
					Comma(strb);
					strb.Append("Subscriber must be the same as the patient for Denti-Cal");//for everyone, we also check patplan.PatID.
				}
				if(claim.PatRelat!=Relat.Self) {
					Comma(strb);
					strb.Append("Insurance relationship must be self for Denti-Cal");
				}
				//We cannot perform this check, because the user must enter this ID in the carrier payor id box before this block is even triggered, defeating the purpose of this check.
				//if(carrier.ElectID!="94146") {
				//  Comma(strb);
				//  strb.Append("Carrier ID must be set to 94146 for Denti-Cal.");
				//}
			}
			X12Validate.Carrier(carrier,strb);
			ElectID electID=ElectIDs.GetID(carrier.ElectID);
			if(electID!=null && electID.IsMedicaid && billProv.MedicaidID=="") {
				Comma(strb);
				strb.Append("BillProv Medicaid ID");
			}
			Patient patient=Patients.GetPat(claim.PatNum);
			if(claim.PlanNum2>0) {
				InsPlan insPlan2=InsPlans.GetPlan(claim.PlanNum2,new List<InsPlan>());
				InsSub sub2=InsSubs.GetSub(claim.InsSubNum2,null);
				if(sub2.SubscriberID.Length<2) {
					Comma(strb);
					strb.Append("Other Insurance SubscriberID");
				}
				Patient subscriber2=Patients.GetPat(sub2.Subscriber); //Always exists because validated in UI.
				if(subscriber2.PatNum!=patient.PatNum) {//Patient address is validated below, so we only need to check if subscriber is not the patient.
					X12Validate.Subscriber2(subscriber2,strb);
				}
				if(subscriber2.Birthdate.Year<1880) {
					Comma(strb);
					strb.Append("Other Insurance Subscriber Birthdate");
				}
				Carrier otherCarrier=Carriers.GetCarrier(insPlan2.CarrierNum);
				X12Validate.Carrier(otherCarrier,strb,"Other Insurance ");//Carrier 2 validation
				if(claim.PatNum != sub2.Subscriber//if patient is not subscriber
					&& claim.PatRelat2==Relat.Self//and relat is self
					&& !Patients.ArePatientsClonesOfEachOther(claim.PatNum,sub2.Subscriber))//and the patient is not a clone of the subscriber
				{ 
					Comma(strb);
					strb.Append("Other Insurance Relationship");
				}
				PatPlan patPlan2=PatPlans.GetFromList(patPlans,claim.InsSubNum2);//can be null
				if(patPlan2!=null && patPlan2.PatID!="") {
					Comma(strb);
					strb.Append("Create a new insurance plan instead of using the optional patient ID for the other insurance plan");
				}
			}
			else { //other insurance not specified
				if(claim.ClaimType=="S") {
					Comma(strb);
					strb.Append("Secondary claim missing other insurance");
				}
			}
			//Provider Idents:
			/*ProviderSupplementalID[] providerIdents=ElectIDs.GetRequiredIdents(carrier.ElectID);
				//No longer any required supplemental IDs
			for(int i=0;i<providerIdents.Length;i++){
				if(!ProviderIdents.IdentExists(providerIdents[i],billProv.ProvNum,carrier.ElectID)){
					if(retVal!="")
						strb.Append(",";
					strb.Append("Billing Prov Supplemental ID:"+providerIdents[i].ToString();
				}
			}*/
			if(sub.SubscriberID.Length<2) {
				Comma(strb);
				strb.Append("SubscriberID");
			}
			Patient subscriber=Patients.GetPat(sub.Subscriber);
			if(subscriber.PatNum!=patient.PatNum) {//Patient address is validated below, so we only need to check if subscriber is not the patient.
				X12Validate.Subscriber(subscriber,strb);
			}
			if(subscriber.Birthdate.Year<1880) {
				Comma(strb);
				strb.Append("Subscriber Birthdate");
			}
			if(claim.PatNum != sub.Subscriber//if patient is not subscriber
				&& claim.PatRelat==Relat.Self//and relat is self
				&& !Patients.ArePatientsClonesOfEachOther(claim.PatNum,sub.Subscriber))//and the patient is not a clone of the subscriber
			{ 
				Comma(strb);
				strb.Append("Claim Relationship");
			}
			if(patient.Address.Trim()=="") {
				Comma(strb);
				strb.Append("Patient Address");
			}
			if(patient.City.Trim().Length<2) {
				Comma(strb);
				strb.Append("Patient City");
			}
			if(patient.State.Trim().Length!=2) {
				Comma(strb);
				strb.Append("Patient State");
			}
			if(!Regex.IsMatch(patient.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
				Comma(strb);
				strb.Append("Patient Zip");
			}
			if(patient.Birthdate.Year<1880) {
				Comma(strb);
				strb.Append("Patient Birthdate");
			}
			if(claim.AccidentRelated=="A" && claim.AccidentST.Length!=2) {//auto accident with no state
				Comma(strb);
				strb.Append("Auto accident State");
			}
			/*if(IsTesia(clearhouse) && claim.Attachments.Count>0) {//If Tesia and has attachments
				string storedFile;
				for(int c=0;c<claim.Attachments.Count;c++) {
					storedFile=ODFileUtils.CombinePaths(FormEmailMessageEdit.GetAttachPath(),claim.Attachments[c].ActualFileName);
					if(!File.Exists(storedFile)){
						if(retVal!="")
							strb.Append(",";
						strb.Append("attachments missing";
						break;
					}
				}
			}*/
			//Warning if attachments are listed as Mail even though we are sending electronically.
			bool pwkNeeded=false;
			if(claim.AttachedFlags!="Mail") {//in other words, if there are additional flags.
				pwkNeeded=true;
			}
			if(claim.Radiographs>0 || claim.AttachedImages>0 || claim.AttachedModels>0) {
				pwkNeeded=true;
			}
			if(claim.AttachedFlags.Contains("Mail") && pwkNeeded) {
				if(warning!="")
					warning+=",";
				warning+="Attachments set to Mail";
			}
			//Warning if any PWK segments are needed, and there is no ID code.
			if(pwkNeeded && claim.AttachmentID=="") {
				if(warning!="")
					warning+=",";
				warning+="Attachment ID missing";
			}
			if(queueItem.HasIcd9) {
				if(warning!="")
					warning+=",";
				warning+="Claim has procedures with ICD-9 codes";
			}
			//If Attachment ID # is present but no attachment flag, then sending is blocked.
			if(!pwkNeeded && claim.AttachmentID!="") {
				Comma(strb);
				strb.Append("Attachment type missing");
			}
			if(claim.MedType==EnumClaimMedType.Institutional) {
				if(claim.UniformBillType.Length!=3) {
					Comma(strb);
					strb.Append("BillType");
				}
				if(claim.AdmissionTypeCode.Length!=1) {
					Comma(strb);
					strb.Append("AdmissionType");
				}
				if(claim.AdmissionSourceCode.Length!=1) {
					Comma(strb);
					strb.Append("AdmissionSource");
				}
				if(claim.PatientStatusCode.Length!=2) {
					Comma(strb);
					strb.Append("PatientStatusCode");
				}
			}
			if(claim.ClaimType!="PreAuth") {
				if(claim.DateService.Year<1880) {
					Comma(strb);
					strb.Append("DateService");
				}
			}
			if(claim.MedType==EnumClaimMedType.Institutional
				|| claim.MedType==EnumClaimMedType.Medical) 
			{
				if(claim.PreAuthString!="") {
					Comma(strb);
					strb.Append("Predeterm number not allowed");
				}
			}
			if((claim.CorrectionType!=ClaimCorrectionType.Original)
				&& claim.OrigRefNum.Trim()=="") {
				Comma(strb);
				strb.Append("Original reference num needed when correction type is not set to original");
			}
			if(claim.UniformBillType.Length>2 
				&& (claim.UniformBillType.Substring(2,1)=="6" || claim.UniformBillType.Substring(2,1)=="7" || claim.UniformBillType.Substring(2,1)=="8")//correction, replacement, void
				&& claim.OrigRefNum.Trim()=="") {
				Comma(strb);
				strb.Append("Original reference num needed when type of bill ends in 6, 7 or 8");
			}
			if(claim.ClaimIdentifier.Trim()=="") {
				Comma(strb);
				strb.Append("Claim identifier missing open Edit Claim window and save to fix");
			}
			else if(Claims.IsClaimIdentifierInUse(claim.ClaimIdentifier,claim.ClaimNum,claim.ClaimType)) {
				Comma(strb);
				strb.Append("Claim identifier already in use for another claim");
			}
			List<ClaimProc> claimProcList=ClaimProcs.RefreshForClaim(claim.ClaimNum);
			List<ClaimProc> claimProcs=ClaimProcs.GetForSendClaim(claimProcList,claim.ClaimNum);
			if(claimProcs.Count==0) {
				Comma(strb);
				strb.Append(Eclaims.Eclaims.GetNoProceduresOnClaimMessage());
			}
			if(claim.MedType==EnumClaimMedType.Institutional && claimProcs.Count>999) {
				Comma(strb);
				strb.Append("More than 999 procedures create multiple claims instead");
			}
			if((claim.MedType==EnumClaimMedType.Medical || claim.MedType==EnumClaimMedType.Dental) && claimProcs.Count>50) {
				Comma(strb);
				strb.Append("More than 50 procedures create multiple claims instead");
			}
			List<Procedure> procList=Procedures.GetProcsFromClaimProcs(claimProcs);
			if(claim.MedType==EnumClaimMedType.Dental && Procedures.GetUniqueDiagnosticCodes(procList,false).Count>4) {
				Comma(strb);
				strb.Append("Claim has more than 4 unique diagnosis codes.  Create multiple claims instead.");
			}
			if(Procedures.GetUniqueDiagnosticCodes(procList,true).Count>12) {
				Comma(strb);
				strb.Append("Claim has more than 12 unique diagnosis codes.  Create multiple claims instead.");
			}
			if(claim.MedType==EnumClaimMedType.Dental && claim.IsOrtho && claim.OrthoTotalM>0 && claim.OrthoTotalM<claim.OrthoRemainM) {
				Comma(strb);
				strb.Append("Ortho months total must be greater than or equal to ortho months remaining.");
			}
			Procedure proc;
			ProcedureCode procCode;
			bool princDiagExists=false;
			for(int i=0;i<claimProcs.Count;i++) {
				string p="proc"+(i+1).ToString()+"-";
				proc=Procedures.GetProcFromList(procList,claimProcs[i].ProcNum);
				procCode=ProcedureCodes.GetProcCode(proc.CodeNum);
				if(ProcMultiVisits.IsProcInProcess(proc.ProcNum)) {
					//The UI blocks claims from being created with In Process procedures attached.
					//However, it is possible to create a claim when the procedures are complete,
					//then to change the status of a procedure in the group to revert the attached procedure back into In Process status.
					//This is our last line of defense to block the user from sending an incomplete claim.
					strb.Append(procCode.AbbrDesc+" is In Process");
				}				
				if(claim.MedType==EnumClaimMedType.Medical) {
					if(proc.IcdVersion!=9 && proc.IsPrincDiag && proc.DiagnosticCode!="") {
						princDiagExists=true;
					}
					if(proc.CodeMod1.Length!=0 && proc.CodeMod1.Length!=2) {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod1");
					}
					if(proc.CodeMod2.Length!=0 && proc.CodeMod2.Length!=2) {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod2");
					}
					if(proc.CodeMod3.Length!=0 && proc.CodeMod3.Length!=2) {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod3");
					}
					if(proc.CodeMod4.Length!=0 && proc.CodeMod4.Length!=2) {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod4");
					}
					if(Regex.IsMatch(claimProcs[i].CodeSent,"^[0-9]{3}99$") && proc.ClaimNote.Trim()=="") { //CPT codes ending in 99.
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" proc e-claim note missing");
					}
				}
				else if(claim.MedType==EnumClaimMedType.Institutional) {
					if(proc.RevCode==""){
						Comma(strb);
						strb.Append(p+"RevenueCode");
					}
					if(proc.CodeMod1.Length!=0 && proc.CodeMod1.Length!=2){
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod1");
					}
					if(proc.CodeMod2.Length!=0 && proc.CodeMod2.Length!=2){
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod2");
					}
					if(proc.CodeMod3.Length!=0 && proc.CodeMod3.Length!=2){
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod3");
					}
					if(proc.CodeMod4.Length!=0 && proc.CodeMod4.Length!=2){
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" mod4");
					}
					if(procCode.DrugNDC!="" && proc.DrugQty>0){
						if(proc.DrugUnit==EnumProcDrugUnit.None){
							Comma(strb);
							strb.Append(procCode.AbbrDesc+" drug unit");
						}
					}
				}
				else if(claim.MedType==EnumClaimMedType.Dental) {
					if(procCode.TreatArea==TreatmentArea.Arch && proc.Surf=="") {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" missing arch");
					}
					if((procCode.TreatArea==TreatmentArea.ToothRange || procCode.AreaAlsoToothRange) && proc.ToothRange=="") {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" tooth range");
					}
					if(procCode.TreatArea==TreatmentArea.ToothRange && proc.UnitQty>1) {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" unit quantity must be 1 since area is tooth range");
					}
					if((procCode.TreatArea==TreatmentArea.Tooth || procCode.TreatArea==TreatmentArea.Surf)
						&& !Tooth.IsValidDB(proc.ToothNum)) 
					{
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" tooth number");
					}
					if(procCode.TreatArea==TreatmentArea.Surf && proc.Surf=="") {
						Comma(strb);
						strb.Append(procCode.AbbrDesc+" surface missing");
					}
					if(procCode.IsProsth) {
						if(proc.Prosthesis=="") {//they didn't enter whether Initial or Replacement
							Comma(strb);
							strb.Append("procedure "+procCode.ProcCode+" must indicate prosthesis Initial or Replacement");
						}
						if(proc.Prosthesis=="R"	&& proc.DateOriginalProsth.Year<1880) {//if a replacement, they didn't enter a date
							Comma(strb);
							strb.Append("procedure "+procCode.ProcCode+" must indicate prosthesis Original Date");
						}
					}
				}
				//if(proc.PlaceService!=claim.PlaceService) {
				//  Comma(strb);
				//  strb.Append("Proc place of service does not match claim "+procCode.ProcCode);
				//}
				//Providers
				Provider provTreatProc=treatProv;
				if(claim.ProvTreat!=proc.ProvNum && PrefC.GetBool(PrefName.EclaimsSeparateTreatProv)) {
					provTreatProc=Providers.GetFirstOrDefault(x => x.ProvNum==proc.ProvNum)??providerFirst;
					if(provTreatProc.LName=="") {
						Comma(strb);
						strb.Append("Treat Prov LName for proc "+procCode.ProcCode);
					}
					if(provTreatProc.FName=="" && !provTreatProc.IsNotPerson) {
						Comma(strb);
						strb.Append("Treat Prov FName for proc "+procCode.ProcCode);
					}
					if(provTreatProc.NationalProvID.Length<2) {
						Comma(strb);
						strb.Append("Treat Prov NPI for proc "+procCode.ProcCode);
					}
					if(claim.MedType!=EnumClaimMedType.Institutional) { //Medical and Dental only. No where to send taxonomy code for instituational procedures.
						if(provTreatProc.TaxonomyCodeOverride.Length>0 && provTreatProc.TaxonomyCodeOverride.Length!=10) {
							Comma(strb);
							strb.Append("Treating Prov Taxonomy Code for proc "+procCode.ProcCode+" must be 10 characters");
						}
					}
					//Treating prov SSN/TIN is not sent on paper or eclaims. Do not verify or block.
					if(!Regex.IsMatch(provTreatProc.NationalProvID,"^(80840)?[0-9]{10}$")) {
						Comma(strb);
						strb.Append("Treat Prov NPI for proc "+procCode.ProcCode+" must be a 10 digit number with an optional prefix of 80840");
					}
					//will add any other checks as needed. Can't think of any others at the moment.
				}
				if(claim.MedType==EnumClaimMedType.Medical) {
					Provider provOrderProc=GetOrderingProviderForProc(claim,carrier,proc,provTreatProc);
					//Do not validate ordering provider override name or NPI if will not go out on eclaim or if already validated elsewhere.
					if(provOrderProc!=null && !provOrderProc.IsNotPerson && provOrderProc.ProvNum!=proc.ProvNum && 
						provOrderProc.ProvNum!=claim.ProvTreat && provOrderProc.ProvNum!=claim.ProvBill)
					{
						if(provOrderProc.LName=="") {
							Comma(strb);
							strb.Append("Ordering prov "+provOrderProc.Abbr+" LName for proc "+procCode.ProcCode);
						}
						if(provOrderProc.FName=="") {
							Comma(strb);
							strb.Append("Ordering prov "+provOrderProc.Abbr+" FName for proc "+procCode.ProcCode);
						}
						if(!Regex.IsMatch(provOrderProc.NationalProvID,"^(80840)?[0-9]{10}$")) {
							Comma(strb);
							strb.Append("Ordering Prov "+provOrderProc.Abbr+" NPI for proc "+procCode.ProcCode+" must be a 10 digit number with an optional prefix of 80840");
						}
					}
				}
				//Site validation (if specified).  Currently used for Service Facility Location.
				if(proc.SiteNum > 0) {
					Site site=Sites.GetFirstOrDefault(x => x.SiteNum==proc.SiteNum);
					//Provider
					if(site.ProvNum > 0) {
						Provider provFacility=Providers.GetFirst(x => x.ProvNum==site.ProvNum);
						if(provFacility.LName=="") {
							Comma(strb);
							strb.Append("Site prov "+provFacility.Abbr+" LName for proc "+procCode.ProcCode);
						}
						if(!Regex.IsMatch(provFacility.NationalProvID,"^(80840)?[0-9]{10}$")) {
							Comma(strb);
							strb.Append("Site prov "+provFacility.Abbr+" NPI for proc "+procCode.ProcCode
								+" must be a 10 digit number with an optional prefix of 80840");
						}
						//Address
						if(site.Address.Trim()=="") {
							Comma(strb);
							strb.Append("Site address for proc "+procCode.ProcCode);
						}
						if(site.City.Trim().Length<2) {
							Comma(strb);
							strb.Append("Site city for proc "+procCode.ProcCode);
						}
						if(site.State.Trim().Length!=2) {
							Comma(strb);
							strb.Append("Site state for proc "+procCode.ProcCode);
						}
						if(!Regex.IsMatch(site.Zip.Trim(),"^[0-9]{5}\\-?([0-9]{4})?$")) {//#####, or #####-, or #####-####, or #########. Dashes are removed when X12 is generated.
							Comma(strb);
							strb.Append("Site zip for proc "+procCode.ProcCode);
						}
					}
				}
			}//for int i claimProcs
			if(claim.MedType==EnumClaimMedType.Medical && !princDiagExists) {
				Comma(strb);
				strb.Append("Princ Diagnosis");
			}
			queueItem.Warnings=warning;
			queueItem.MissingData=strb.ToString();
		}

		public static bool HasPOBox(string address) {
			string regExp=@"\bbox(?:\b$|([\s|\-]+)?[0-9]+)|(p[\-\.\s]*o[\-\.\s]*|(post office|post)\s)b(\.|ox|in)?\b|(^p[\.]?(o|b)[\.]?$)";
			return Regex.IsMatch(address,regExp,RegexOptions.IgnoreCase);
		}

		private static void Comma(StringBuilder strb){
			if(strb.Length!=0) {
				strb.Append(", ");
			}
		}
		
		/////<summary>Loops through the 837 to find the transaction number for the specified claim. Will return 0 if can't find.</summary>
		//public int GetTransNum(long claimNum) {
		//  string curTransNumStr="";
		//  for(int i=0;i<Segments.Count;i++) {
		//    if(Segments[i].SegmentID=="ST"){
		//      curTransNumStr=Segments[i].Get(2);
		//    }
		//    if(Segments[i].SegmentID=="CLM"){
		//      if(Segments[i].Get(1).TrimStart(new char[] {'0'})==claimNum.ToString()){//if for specified claim
		//        try {
		//          return PIn.Int(curTransNumStr);
		//        }
		//        catch {
		//          return 0;
		//        }
		//      }
		//    }
		//  }
		//  return 0;
		//}

		///<summary>Loops through the 837 to see if attachments were sent.</summary>
		public bool AttachmentsWereSent(long claimNum) {
			bool isCurrentClaim=false;
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].SegmentID=="CLM") {
					//The following check is currently broken, because we need to compare the claimNum to the portion of the CLM01 which is after the separator (/ or -). CLM01 is typically formatted like PatNum/ClaimNum
					if(Segments[i].Get(1).TrimStart(new char[] { '0' })==claimNum.ToString()) {//if for specified claim.
						isCurrentClaim=true;
					}
					else{
						isCurrentClaim=false;
					}
				}
				if(Segments[i].SegmentID=="PWK" && isCurrentClaim) {
					return true;
				}
			}
			return false;
		}

		///<summary>Removes leading zeros in numbers such as 0.00 or 0.99, for Emdeon and maybe others.</summary>
		private static string AmountToStrNoLeading(double amount) {
			string result=amount.ToString("F");
			int i=0;
			while(i<result.Length-1 && result[i]=='0') {
				i++;
			}
			return result.Substring(i);
		}

	}
}
