using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenDentBusiness {
	///<summary>X12 834 Benefit Enrollment and Maintenance.  This transaction is used to push insurance plan information to pseudo clearinghouses.</summary>
	public class X834:X12object {

		///<summary>All segments within the current transaction set (ST) of the 834 report.</summary>
    private List<X12Segment> _listSegments;
		///<summary>The current segment within _listSegments.</summary>
		private int _segNum;
		///<summary>List of all transactions (ST loops) within the 834.</summary>
		public List<Hx834_Tran> ListTransactions=new List<Hx834_Tran>();
		///<summary>The current transaction within ListTransactions.</summary>
		private Hx834_Tran _tranCur;
		
		///<summary>Shortcut to get current segment based on _segNum.</summary>
		private X12Segment _segCur {
			get{
				if(_segNum < _listSegments.Count) {
					return _listSegments[_segNum];
				}
				return new X12Segment();
			}
		}

		#region Static Globals

		public static bool Is834(X12object xobj) {
      if(xobj.FunctGroups.Count==0) {//At least 1 GS segment in each 834.
        return false;
      }
      if(xobj.FunctGroups[0].Header.Get(1)=="BE") {//GS01 (pg. 237 and pg. 7)
        return true;
      }
      return false;
    }

		#endregion Static Globals

		public X834(X12object x12other):base(x12other) {
			ReadMessage();
		}

		///<summary>See guide pages 22 and 208 for overview.</summary>
		private void ReadMessage() {
			try {
				ReadLoopGS();
			}
			catch(ApplicationException aex) {
				throw new ApplicationException("Error on line "+(_segNum+1)+": "+aex.Message);
			}
		}

		///<summary>GS: Functional Group.  Required.  Repeat unlimited.  Guide page 208.</summary>
		private void ReadLoopGS() {
			for(int i=0;i<FunctGroups.Count;i++) {
				ReadLoopST(FunctGroups[i].Transactions);
			}
		}

		///<summary>ST: Transaction Set Header.  Required.  Repeat 1.  Guide pages 22, 31, 208.</summary>
		private void ReadLoopST(List <X12Transaction> listTrans) {
			for(int i=0;i<listTrans.Count;i++) {
				_listSegments=listTrans[i].Segments;
				_segNum=0;
				_tranCur=new Hx834_Tran();
				ListTransactions.Add(_tranCur);
				ReadLoopST_BGN();
				ReadLoopST_REF();
				ReadLoopST_DTP();
				ReadLoopST_QTY();
				ReadLoop1000A();
				ReadLoop1000B();
				ReadLoop1000C();
				ReadLoop2000();
			}
		}

		///<summary>BGN: Beginning Segment.  Required.  Repeat 1.  Guide page 32.</summary>
		private void ReadLoopST_BGN() {
			_tranCur.BeginningSegment=new X12_BGN(_segCur);
			_segNum++;
		}

		///<summary>REF: Transaction Set Policy Number.  Situational.  Repeat 1.  Guide page 36.</summary>
		private void ReadLoopST_REF() {
			if(!_segCur.IsType("REF","38")) {
				return;
			}
			_tranCur.TransactionSetPolicyNumber=new X12_REF(_segCur);
			_segNum++;
		}

		///<summary>DTP: File Effictive Date.  Situational.  Repeat >1.  Guide page 37.</summary>
		private void ReadLoopST_DTP() {
			_tranCur.ListFileEffectiveDates.Clear();
			while(_segCur.IsType("DTP","007","090","091","303","382","388")) {
				_tranCur.ListFileEffectiveDates.Add(new X12_DTP(_segCur));
				_segNum++;
			}
		}

		///<summary>QTY: Transaction Set Control Totals.  Situational.  Repeat 3.  Guide page 38.</summary>
		private void ReadLoopST_QTY() {
			_tranCur.ListTransactionSetControlTotals.Clear();
			while(_segCur.IsType("QTY","DT","ET","TO")) {
				_tranCur.ListTransactionSetControlTotals.Add(new X12_QTY(_segCur));
				_segNum++;
			}
		}

		///<summary>Loop 1000A: Sponsor Name.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop1000A() {
			ReadLoop1000A_N1();
		}

		///<summary>N1: Sponsor Name.  Required.  Repeat 1.  Guide page 39.</summary>
		private void ReadLoop1000A_N1() {
			_tranCur.SponsorName=new X12_N1(_segCur,"P5");
			_segNum++;
		}

		///<summary>Loop 1000B: Payer.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop1000B() {
			ReadLoop1000B_N1();
		}

		///<summary>N1: Payer.  Required.  Repeat 1.  Guide page 41.</summary>
		private void ReadLoop1000B_N1() {
			_tranCur.Payer=new X12_N1(_segCur,"IN");
			_segNum++;
		}

		///<summary>Loop 1000C: TPA/Broker Name.  Repeat 2.  Guide page 22.</summary>
		private void ReadLoop1000C() {
			_tranCur.ListBrokers.Clear();
			while(_segCur.IsType("N1","BO","TV")) {
				Hx834_Broker broker=new Hx834_Broker();
				ReadLoop1000C_N1(broker);
				ReadLoop1100C(broker);
				_tranCur.ListBrokers.Add(broker);
			}
		}

		///<summary>N1: TPA/Broker Name.  Situational.  Repeat 1.  Guide page 43.</summary>
		private void ReadLoop1000C_N1(Hx834_Broker broker) {
			if(!_segCur.IsType("N1","BO","TV")) {
				return;
			}
			broker.Name=new X12_N1(_segCur);
			_segNum++;
		}

		///<summary>Loop 1100C: TPA/Broker Account.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop1100C(Hx834_Broker broker) {
			ReadLoop1100C_ACT(broker);
		}

		///<summary>ACT: TPA/Broker Account Information.  Situational.  Repeat 1.  Guide page 45.</summary>
		private void ReadLoop1100C_ACT(Hx834_Broker broker) {
			if(!_segCur.IsType("ACT")) {
				return;
			}
			broker.TpaBrokerAccountInformation=new X12_ACT(_segCur);
			_segNum++;
		}

		///<summary>Loop 2000: Member Level Detail.  Repeat >1.  Guide page 22.</summary>
		private void ReadLoop2000() {
			_tranCur.ListMembers.Clear();
			while(_segCur.IsType("INS")) {
				Hx834_Member member=new Hx834_Member();
				member.Tran=_tranCur;
				ReadLoop2000_INS(member);
				ReadLoop2000_REF_1(member);
				ReadLoop2000_REF_2(member);
				ReadLoop2000_REF_3(member);
				ReadLoop2000_DTP(member);
				ReadLoop2100A(member);
				ReadLoop2100B(member);
				ReadLoop2100C(member);
				ReadLoop2100D(member);
				ReadLoop2100E(member);
				ReadLoop2100F(member);
				ReadLoop2100G(member);
				ReadLoop2100H(member);
				ReadLoop2200(member);
				ReadLoop2300(member);
				ReadLoop2000_LS(member);
				ReadLoop2700(member);
				ReadLoop2000_LE(member);
				_tranCur.ListMembers.Add(member);
			}
		}

		///<summary>INS: Member Level Detail.  Required.  Repeat 1.  Guide page 47.</summary>
		private void ReadLoop2000_INS(Hx834_Member member) {
			member.MemberLevelDetail=new X12_INS(_segCur);
			_segNum++;
			//INS01: Useless, because INS02 provides the same information with more detail.
			//INS02:
			member.PlanRelat=Relat.Dependent;
			if(member.MemberLevelDetail.IndividualRelationshipCode=="01") {//Spouse
				member.PlanRelat=Relat.Spouse;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="03") {//Father or Mother
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="04") {//Grandfather or Grandmother
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="05") {//Grandson or Granddaughter
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="06") {//Uncle or Aunt
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="07") {//Niece or Nephew
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="08") {//Cousin
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="09") {//Adopted Child
				member.PlanRelat=Relat.Child;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="10") {//Foster Child
				member.PlanRelat=Relat.Child;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="11") {//Son-in-law or Daugther-in-law
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="12") {//Brother-in-law or Sister-in-law
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="13") {//Mother-in-law or Father-in-law
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="14") {//Brother or Sister
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="15") {//Ward
				member.PlanRelat=Relat.HandicapDep;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="16") {//Stepparent
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="17") {//Stepson or Stepdaughter
				member.PlanRelat=Relat.Child;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="18") {//Self
				member.PlanRelat=Relat.Self;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="19") {//Child
				member.PlanRelat=Relat.Child;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="23") {//Sponsored Dependent
				member.PlanRelat=Relat.Dependent;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="24") {//Dependent of Minor Dependent
				member.PlanRelat=Relat.Dependent;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="25") {//Ex-spouse
				member.PlanRelat=Relat.SignifOther;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="26") {//Guardian
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="31") {//Court Appointed Guardian
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="38") {//Collateral Dependent
				member.PlanRelat=Relat.Dependent;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="53") {//Life Partner
				member.PlanRelat=Relat.LifePartner;
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="60") {//Annuitant
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="D2") {//Trustee
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="G8") {//Other Relationship
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			else if(member.MemberLevelDetail.IndividualRelationshipCode=="G9") {//Other Relative
				member.PlanRelat=Relat.Dependent;//No direct correlation to the standard.
			}
			//INS03: Useful when importing.  No conversion needed.
			//INS04: Nowhere to store this information, and nobody cares why the change has occurred, only that it did occur.
			//INS05: Nowhere to store this information.  Plus this does not tell us if the subscriber is inactive.
			//INS06:
			bool isMedicareA=false;
			bool isMedicareB=false;
			if(member.MemberLevelDetail.MedicareStatusCode.StartsWith("A")) {//Medicare Part A (Hospital Insurance)
				isMedicareA=true;
			}
			else if(member.MemberLevelDetail.MedicareStatusCode.StartsWith("B")) {//Medicare Part B (Medical Insurance)
				isMedicareB=true;
			}
			else if(member.MemberLevelDetail.MedicareStatusCode.StartsWith("C")) {//Medicare Part A (Hospital Insurance) and Part B (Medical Insurance)
				//We can only specify one filing code, so we have to pick either A or B.
				isMedicareB=true;//Medical insurance is more likely for OD users than Hospital insurance is.
			}
			else if(member.MemberLevelDetail.MedicareStatusCode.StartsWith("D")) {//Medicare.  Presumably this is the same as "C" above.
				//We can only specify one filing code, so we have to pick either A or B.
				isMedicareB=true;//Medical insurance is more likely for OD users than Hospital insurance is.
			}
			else {//"E"
				//Not Medicare.  Do not set filing code, since we do not know what to set it to.  Will go out as Commercial Ins in 837, since unspecified.
			}
			member.InsFiling=null;
			if(isMedicareA) {
				member.InsFiling=InsFilingCodes.GetOrInsertForEclaimCode("MedicarePartA","MA");
			}
			else if(isMedicareB) {
				member.InsFiling=InsFilingCodes.GetOrInsertForEclaimCode("MedicarePartB","MB");
			}
			//INS07: Nowhere to store this information, and nobody cares why the change has occurred, only that it did occur.
			//INS08: Nowhere to store Employment Status, but there is a place to store the Employer Name.
			member.Pat.StudentStatus=member.MemberLevelDetail.StudentStatusCode;//INS09: Student Status Code.  One-to-one conversion.
			//INS10: Nowhere to store this information.  If Handicapped Dependent, then the information was already given to us in INS02.
			//INS11: All dates are always D8 format.
			member.Pat.DateTimeDeceased=X12Parse.ToDate(member.MemberLevelDetail.DateOfDeath);//INS12:
			//INS13: Confidentiality Code.  Situational.
			member.IsReleaseInfo=true;
			if(member.MemberLevelDetail.ConfidentialityCode=="R") {
				member.IsReleaseInfo=false;//Only restricted if specified as such.  Otherwise assume unrestricted.
			}
			//INS14: Not used.
			//INS15: Not used.
			//INS16: Not used.
			//INS17: Birth Sequence Number.  Nowhere to store this information.	
		}

		///<summary>REF: Subscriber Identifier.  Required.  Repeat 1.  Guide page 55.</summary>
		private void ReadLoop2000_REF_1(Hx834_Member member) {
			member.SubscriberIdentifier=new X12_REF(_segCur,"0F");
			_segNum++;
			//REF01: Useless
			//REF02:
			member.SubscriberId=member.SubscriberIdentifier.ReferenceId;
			//REF03: Not used
			//REF04: Not used
		}

		///<summary>REF: Member Policy Number.  Situational.  Repeat 1.  Guide page 56.</summary>
		private void ReadLoop2000_REF_2(Hx834_Member member) {
			member.GroupNum="";
			if(!_segCur.IsType("REF","1L")) {
				return;
			}
			member.MemberPolicyNumber=new X12_REF(_segCur);
			_segNum++;
			//REF01: Useless
			//REF02:
			member.GroupNum=member.MemberPolicyNumber.ReferenceId;
			//REF03: Not used
			//REF04: Not used
		}

		///<summary>REF: Member Supplemental Identifier.  Situational.  Repeat 13.  Guide page 57.</summary>
		private void ReadLoop2000_REF_3(Hx834_Member member) {
			member.ListMemberSupplementalIdentifiers.Clear();
			while(_segCur.IsType("REF","17","23","3H","4A","6O","ABB","D3","DX","F6","P5","Q4","QQ","ZZ")) {
				X12_REF href=new X12_REF(_segCur);
				member.ListMemberSupplementalIdentifiers.Add(href);
				_segNum++;
			}
			//Nowhere to store this information.  Perhaps this is a way to receive PatNum?
		}

		///<summary>DTP: Member Level Dates.  Situational.  Repeat 24.  Guide page 59.</summary>
		private void ReadLoop2000_DTP(Hx834_Member member) {
			member.ListMemberLevelDates.Clear();
			while(_segCur.IsType("DTP",
				"050","286","296","297","300","301","303","336","337","338","339","340","341","350","351","356","357","383","385","386","393","394","473","474"))
			{
				X12_DTP dtp=new X12_DTP(_segCur);
				member.ListMemberLevelDates.Add(dtp);
				_segNum++;
				if(dtp.DateTimeQualifier=="050") {//Recieved
					//Used to identify the date an enrollment application is recieved.
				}
				else if(dtp.DateTimeQualifier=="286") {//Retirement
				}
				else if(dtp.DateTimeQualifier=="296") {//Initial Disability Period Return To Work
				}
				else if(dtp.DateTimeQualifier=="297") {//Initial Disability Period Last Day Worked
				}
				else if(dtp.DateTimeQualifier=="300") {//Enrollment Signature Date
				}
				else if(dtp.DateTimeQualifier=="301") {//Consolidated Omnibus Budget Reconciliation Act (COBRA) Qualifying Event
				}
				else if(dtp.DateTimeQualifier=="303") {//Maintenance Effective
					//This code is used to send the effective date of a change to an existing member's information, excluding changed made in loop 2300.
				}
				else if(dtp.DateTimeQualifier=="336") {//Employment Begin
				}
				else if(dtp.DateTimeQualifier=="337") {//Employment End
				}
				else if(dtp.DateTimeQualifier=="338") {//Medicare Begin
				}
				else if(dtp.DateTimeQualifier=="339") {//Medicare End
				}
				else if(dtp.DateTimeQualifier=="340") {//Consolidated Omnibus Budget Reconciliation Act (COBRA) Begin
				}
				else if(dtp.DateTimeQualifier=="341") {//Consolidated Omnibus Budget Reconciliation Act (COBRA) End
				}
				else if(dtp.DateTimeQualifier=="350") {//Education Begin
					//This is the start date for the student at the current educational institution.
				}
				else if(dtp.DateTimeQualifier=="351") {//Education End
					//This is the expected graduation date the student at the current educational institution.
				}
				else if(dtp.DateTimeQualifier=="356") {//Eligibility Begin
					//The date when a member could elect to enroll or begin benefits in any health care plan through the employer.
					//This is not the actual begin date of coverage, which is conveyed in the DTP segment at position 2700.
					//The effective date is communicated in the DTP of loop 2300.
				}
				else if(dtp.DateTimeQualifier=="357") {//Eligibility End
					//The eligibility end date represents the last date of coverage for which claims will be paid for the individual being terminated.
					//For example, if a date of 02/28/2001 is passed then claims for this individual will be paid through 11:59 p.m. on 02/28/2001.
					//The termination date is communicated in the DTP of loop 2300.
				}
				else if(dtp.DateTimeQualifier=="383") {//Adjusted Hire
				}
				else if(dtp.DateTimeQualifier=="385") {//Credited Service Begin
					//The start date from which an employee's length of service, as defined in the plan document, will be calculated.
				}
				else if(dtp.DateTimeQualifier=="386") {//Credited Service End
					//The end date to be used in the calculation of an employee's length of service, as defined in the plan document.
				}
				else if(dtp.DateTimeQualifier=="393") {//Plan Participation Suspension
				}
				else if(dtp.DateTimeQualifier=="394") {//Rehire
				}
				else if(dtp.DateTimeQualifier=="473") {//Medicaid Begin
				}
				else if(dtp.DateTimeQualifier=="474") {//Medicaid End
				}
			}
		}

		///<summary>Loop 2100A: Member Name.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop2100A(Hx834_Member member) {
			ReadLoop2100A_NM1(member);
			ReadLoop2100A_PER(member);
			ReadLoop2100A_N3(member);
			ReadLoop2100A_N4(member);
			ReadLoop2100A_DMG(member);
			ReadLoop2100A_EC(member);
			ReadLoop2100A_ICM(member);
			ReadLoop2100A_AMT(member);
			ReadLoop2100A_HLH(member);
			ReadLoop2100A_LUI(member);
		}

		///<summary>NM1: Member Name.  Required.  Repeat 1.  Guide page 62.</summary>
		private void ReadLoop2100A_NM1(Hx834_Member member) {
			member.MemberName=new X12_NM1(_segCur,"74","IL");
			_segNum++;
			//NM101: Useless, because this information is provided INS02.
			//NM102: Useless
			//NM103:
			member.Pat.LName=member.MemberName.NameLast;
			//NM104:
			member.Pat.FName=member.MemberName.NameFirst;
			//NM105:
			member.Pat.MiddleI=member.MemberName.NameMiddle;
			//NM106: Nowhere to store this information.
			//NM107: Nowhere to store this information.
			//NM108 & NM109:
			member.Pat.SSN="";
			if(member.MemberName.IdentificationCodeQualifier=="34") {//The only other code is "ZZ" which is useless to us.
				//Ignore puncuation (digits only) as required for this column when saved to the database.
				member.Pat.SSN=Regex.Replace(member.MemberName.IdentificationCode,"[^0-9]","");
			}
			//NM110 through NM112: Not used
			member.Pat.Preferred="";
		}

		///<summary>PER: Member Communications Numbers.  Situational.  Repeat 1.  Guide page 65.</summary>
		private void ReadLoop2100A_PER(Hx834_Member member) {
			if(!_segCur.IsType("PER","IP")) {
				return;
			}
			member.MemberCommunicationsNumbers=new X12_PER(_segCur);
			_segNum++;
			string[] arrayNumbers=new string[] {
				member.MemberCommunicationsNumbers.CommunicationNumberQualifier1,member.MemberCommunicationsNumbers.CommunicationNumber1,
				member.MemberCommunicationsNumbers.CommunicationNumberQualifier2,member.MemberCommunicationsNumbers.CommunicationNumber2,
				member.MemberCommunicationsNumbers.CommunicationNumberQualifier3,member.MemberCommunicationsNumbers.CommunicationNumber3,
			};
			member.Pat.AddrNote=null;
			member.Pat.WirelessPhone=null;
			member.Pat.Email=null;
			member.Pat.HmPhone=null;
			member.Pat.WkPhone=null;
			for(int i=0;i<arrayNumbers.Length;i+=2) {
				string qualifier=arrayNumbers[i];
				string number=arrayNumbers[i+1];
				if(qualifier=="AP") {//Alternate Phone
					if(member.Pat.AddrNote!="") {
						member.Pat.AddrNote+="\r\n";
					}
					member.Pat.AddrNote+="Alternate Phone: "+number;//No specific patient field for this information.  
				}
				else if(qualifier=="BN") {//Beeper Number
					if(member.Pat.AddrNote!="") {
						member.Pat.AddrNote+="\r\n";
					}
					member.Pat.AddrNote+="Beeper: "+number;//No specific patient field for this information. 
				}
				else if(qualifier=="CP") {//Cellular Phone
					member.Pat.WirelessPhone=number;
				}
				else if(qualifier=="EM") {//Electronic Mail
					member.Pat.Email=number;
				}
				else if(qualifier=="EX") {//Telephone Extension
					if(member.Pat.AddrNote!="") {
						member.Pat.AddrNote+="\r\n";
					}
					member.Pat.AddrNote+="Extension: "+number;//No specific patient field for this information. 
				}
				else if(qualifier=="FX") {//Facsimile
					if(member.Pat.AddrNote!="") {
						member.Pat.AddrNote+="\r\n";
					}
					member.Pat.AddrNote+="Fax: "+number;//No specific patient field for this information. 
				}
				else if(qualifier=="HP") {//Home Phone Number
					member.Pat.HmPhone=number;
				}
				else if(qualifier=="TE") {//Telephone
					if(member.Pat.AddrNote!="") {
						member.Pat.AddrNote+="\r\n";
					}
					member.Pat.AddrNote+="Phone: "+number;//No specific patient field for this information. 
				}
				else if(qualifier=="WP") {//Work Phone Number
					member.Pat.WkPhone=number;
				}
			}
		}

		///<summary>N3: Member Residence Street Address.  Situational.  Repeat 1.  Guide page 68.</summary>
		private void ReadLoop2100A_N3(Hx834_Member member) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			member.MemberResidenceStreetAddress=new X12_N3(_segCur);
			_segNum++;
			//N301:
			member.Pat.Address=member.MemberResidenceStreetAddress.AddressInformation1;
			//N302:
			member.Pat.Address2=member.MemberResidenceStreetAddress.AddressInformation2;
		}
		
		///<summary>N4: Member City, State, Zip Code.  Situational.  Repeat 1.  Guide page 69.</summary>
		private void ReadLoop2100A_N4(Hx834_Member member) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			member.MemberCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
			member.Pat.City=member.MemberCityStateZipCode.CityName;
			member.Pat.State=member.MemberCityStateZipCode.StateOrProvinceCode;
			member.Pat.Zip=member.MemberCityStateZipCode.PostalCode;
		}

		///<summary>DMG: Member Demographics.  Situational.  Repeat 1.  Guide page 72.</summary>
		private void ReadLoop2100A_DMG(Hx834_Member member) {
			if(!_segCur.IsType("DMG")) {
				return;
			}
			member.MemberDemographics=new X12_DMG(_segCur);
			_segNum++;
			//DMG01: Useless
			//DMG02:
			member.Pat.Birthdate=X12Parse.ToDate(member.MemberDemographics.DateTimePeriod);
			//DMG03:
			member.Pat.Gender=PatientGender.Unknown;
			if(member.MemberDemographics.GenderCode=="F") {
				member.Pat.Gender=PatientGender.Female;
			}
			else if(member.MemberDemographics.GenderCode=="M") {
				member.Pat.Gender=PatientGender.Male;
			}			
			//DMG04:
			if(member.MemberDemographics.MaritalStatusCode=="B") {//Registered Domestic Partner
				member.Pat.Position=PatientPosition.Married;//We do not have this status currently.  Closest match used instead.
			}
			else if(member.MemberDemographics.MaritalStatusCode=="D") {//Divorced
				member.Pat.Position=PatientPosition.Divorced;
			}
			else if(member.MemberDemographics.MaritalStatusCode=="I") {//Single
				member.Pat.Position=PatientPosition.Single;
			}
			else if(member.MemberDemographics.MaritalStatusCode=="M") {//Married
				member.Pat.Position=PatientPosition.Married;
			}
			else if(member.MemberDemographics.MaritalStatusCode=="R") {//Unreported
				member.Pat.Position=PatientPosition.Single;//We do not have this status currently.  Closest match used instead.
			}
			else if(member.MemberDemographics.MaritalStatusCode=="S") {//Separated
				member.Pat.Position=PatientPosition.Married;//We do not have this status currently.  Closest match used instead.
			}
			else if(member.MemberDemographics.MaritalStatusCode=="U") {//Unmarried
				member.Pat.Position=PatientPosition.Single;//We do not have this status currently.  Closest match used instead.
			}
			else if(member.MemberDemographics.MaritalStatusCode=="W") {//Widowed
				member.Pat.Position=PatientPosition.Widowed;
			}
			else if(member.MemberDemographics.MaritalStatusCode=="X") {//Legally Separated
				member.Pat.Position=PatientPosition.Married;//We do not have this status currently.  Closest match used instead.
			}
			//DMG05:
			member.ListPatRaces.Clear();
			if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("7")) {//Not Provided
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));//Declined to specify race
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,PatientRace.DECLINE_SPECIFY_RACE_CODE));//Declined to specify ethnicity
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("8")) {//Not Applicable
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("A")) {//Asian or Pacific Islander
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2028-9"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("B")) {//Black
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2054-5"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("C")) {//Caucasian
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2106-3"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("D")) {//Subcontinent Asian American
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("E")) {//Other Race or Ethnicity
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2131-1"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("F")) {//Asian Pacific American
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2028-9"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("G")) {//Native American
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"1002-5"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("H")) {//Hispanic
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2135-2"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("I")) {//American Indian or Alaskan Native
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"1002-5"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("J")) {//Native Hawaiian
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2076-8"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("N")) {//Black (Non-Hispanic)
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2054-5"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("O")) {//White (Non-Hispanic)
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2106-3"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("P")) {//Pacific Islander
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2076-8"));
			}
			else if(member.MemberDemographics.CompositeRaceOrEthnicityInformation.StartsWith("Z")) {//Mutually Defined
				member.ListPatRaces.Add(new PatientRace(member.Pat.PatNum,"2131-1"));
			}
			//DMG06: Nowhere to put this information.
			//DMG07 through DMG09: Not used
			//DMG10: Nowhere to put this information.
			//DMG11: Nowhere to put this information.
		}

		///<summary>EC: Employment Class.  Situational.  Repeat >1.  Guide page 76.</summary>
		private void ReadLoop2100A_EC(Hx834_Member member) {
			member.ListEmploymentClass.Clear();
			while(_segCur.IsType("EC")) {
				member.ListEmploymentClass.Add(new X12_EC(_segCur));
				_segNum++;
			}
			//Nowhere to store this information.
		}

		///<summary>ICM: Member Income.  Situational.  Repeat 1.  Guide page 79.</summary>
		private void ReadLoop2100A_ICM(Hx834_Member member) {
			if(!_segCur.IsType("ICM")) {
				return;
			}
			member.MemberIncome=new X12_ICM(_segCur);
			_segNum++;
			//Nowhere to store this information.
		}

		///<summary>AMT: Member Policy Amounts.  Situational.  Repeat 7.  Guide page 81.</summary>
		private void ReadLoop2100A_AMT(Hx834_Member member) {
			member.ListMemberPolicyAmounts.Clear();
			while(_segCur.IsType("AMT","B9","C1","D2","EBA","FK","P3","R")) {
				member.ListMemberPolicyAmounts.Add(new X12_AMT(_segCur));
				_segNum++;
			}
			//D3 does not seem to need this information, based on the example data provided.
		}

		///<summary>HLH: Member Health Information.  Situational.  Repeat 1.  Guide page 82.</summary>
		private void ReadLoop2100A_HLH(Hx834_Member member) {
			if(!_segCur.IsType("HLH")) {
				return;
			}
			member.MemberHealthInformation=new X12_HLH(_segCur);
			_segNum++;
			//Could be useful information for EHR, but difficult to convert, and not useful enough for now.
		}

		///<summary>LUI: Member Language.  Situational.  Repeat >1.  Guide page 84.</summary>
		private void ReadLoop2100A_LUI(Hx834_Member member) {
			member.ListMemberLanguages.Clear();
			List<string> listLangRead=new List<string>();
			List<string> listLangWrite=new List<string>();
			List<string> listLangSpeak=new List<string>();
			List<string> listLangNative=new List<string>();
			List<string> listLangPrimary=new List<string>();
			while(_segCur.IsType("LUI")) {
				X12_LUI lui=new X12_LUI(_segCur);
				member.ListMemberLanguages.Add(lui);
				_segNum++;
				string lang="";
				//LUI01 & LUI02:
				if(lui.IdentificationCodeQualifier=="LD") {//NISO Z39.53 Language Code
					lang=lui.IdentificationCode;//I think this is the same as ISO 639-2.  We support ISO 639-2 direclty.
				}
				else if(lui.IdentificationCodeQualifier=="LE") {//ISO 639 Langauge Code.
					lang=lui.IdentificationCode;//We support ISO 639 direclty.
				}
				//LUI03:
				if(lang=="") {
					lang=lui.Description;
				}
				//LUI04:
				if(lui.UseOfLanguageIndicator=="5") {//Language Reading
					listLangRead.Add(lang);
				}
				else if(lui.UseOfLanguageIndicator=="6") {//Language Writing
					listLangWrite.Add(lang);
				}
				else if(lui.UseOfLanguageIndicator=="7") {//Langauge Speaking
					listLangSpeak.Add(lang);
				}
				else if(lui.UseOfLanguageIndicator=="8") {//Native Language
					listLangNative.Add(lang);
				}
				else {//Unspecified usage.  Assume primary.
					listLangPrimary.Add(lang);
				}
				//LUI05: Not used.
			}
			//We currently only support one language per patient.  Try to figure out what their primary language is.
			member.Pat.Language="";
			if(listLangPrimary.Count > 0) {
				member.Pat.Language=listLangPrimary[0];
			}
			else if(listLangNative.Count > 0) {
				member.Pat.Language=listLangNative[0];
			}
			else if(listLangSpeak.Count > 0) {
				member.Pat.Language=listLangSpeak[0];
			}
			else if(listLangRead.Count > 0) {
				member.Pat.Language=listLangRead[0];
			}
			else if(listLangWrite.Count > 0) {
				member.Pat.Language=listLangWrite[0];
			}
		}

		///<summary>Loop 2100B: Incorrect Member Name.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop2100B(Hx834_Member member) {
			if(!_segCur.IsType("NM1","70")) {
				return;
			}
			ReadLoop2100B_NM1(member);
			ReadLoop2100B_DMG(member);
			//I doubt we can do anything with this information except display it somewhere.
		}

		///<summary>NM1: Incorrect Member Name.  Situational.  Repeat 1.  Guide page 86.</summary>
		private void ReadLoop2100B_NM1(Hx834_Member member) {
			if(!_segCur.IsType("NM1","70")) {
				return;
			}
			member.IncorrectMemberName=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>DMG: Incorrect Member Demographics.  Situational.  Repeat 1.  Guide page 89.</summary>
		private void ReadLoop2100B_DMG(Hx834_Member member) {
			if(!_segCur.IsType("DMG")) {
				return;
			}
			member.IncorrectMemberDemographics=new X12_DMG(_segCur);
			_segNum++;
		}

		///<summary>Loop 2100C: Member Mailing Address.  Repeat 1.  Guide page 22.</summary>
		private void ReadLoop2100C(Hx834_Member member) {
			if(!_segCur.IsType("NM1","31")) {
				return;
			}
			ReadLoop2100C_NM1(member);
			ReadLoop2100C_N3(member);
			ReadLoop2100C_N4(member);
		}

		///<summary>NM1: Member Mailing Address.  Situational.  Repeat 1.  Guide page 93.</summary>
		private void ReadLoop2100C_NM1(Hx834_Member member) {
			if(!_segCur.IsType("NM1","31")) {
				return;
			}
			member.MemberMailingAddress=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>N3: Member Mail Street Address.  Required.  Repeat 1.  Guide page 95.  Overrides Loop 2100A if present.</summary>
		private void ReadLoop2100C_N3(Hx834_Member member) {
			member.MemberMailStreetAddress=new X12_N3(_segCur);
			_segNum++;
			//N301:
			member.Pat.Address=member.MemberMailStreetAddress.AddressInformation1;
			//N302:
			member.Pat.Address2=member.MemberMailStreetAddress.AddressInformation2;
		}

		///<summary>N4: Member Mail City, State, Zip Code.  Required.  Repeat 1.  Guide page 96.  Overrides Loop 2100A if present.</summary>
		private void ReadLoop2100C_N4(Hx834_Member member) {
			member.MemberMailCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
			member.Pat.City=member.MemberMailCityStateZipCode.CityName;
			member.Pat.State=member.MemberMailCityStateZipCode.StateOrProvinceCode;
			member.Pat.Zip=member.MemberMailCityStateZipCode.PostalCode;
		}

		///<summary>Loop 2100D: Member Employer.  Repeat 3.  Guide page 23.</summary>
		private void ReadLoop2100D(Hx834_Member member) {
			member.ListMemberEmployers.Clear();
			while(_segCur.IsType("NM1","36")) {
				Hx834_Employer employer=new Hx834_Employer();
				ReadLoop2100D_NM1(employer);
				ReadLoop2100D_PER(employer);
				ReadLoop2100D_N3(employer);
				ReadLoop2100D_N4(employer);
				member.ListMemberEmployers.Add(employer);
			}
			//TODO: This could be attached via Pat and InsPlan, but we would need insert into Employer table if not present.
		}

		///<summary>NM1: Member Employer.  Situational.  Repeat 1.  Guide page 98.</summary>
		private void ReadLoop2100D_NM1(Hx834_Employer employer) {
			if(!_segCur.IsType("NM1","36")) {
				return;
			}
			employer.MemberEmployer=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>PER: Member Employer Communications Numbers.  Situational.  Repeat 1.  Guide page 101.</summary>
		private void ReadLoop2100D_PER(Hx834_Employer employer) {
			if(!_segCur.IsType("PER","EP")) {
				return;
			}
			employer.MemberEmployerCommunicationsNumbers=new X12_PER(_segCur);
			_segNum++;
		}

		///<summary>N3: Member Employer Street Address.  Situational.  Repeat 1.  Guide page 104.</summary>
		private void ReadLoop2100D_N3(Hx834_Employer employer) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			employer.MemberEmployerStreetAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Member Employer City, State, Zip Code.  Situational.  Repeat 1.  Guide page 105.</summary>
		private void ReadLoop2100D_N4(Hx834_Employer employer) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			employer.MemberEmployerCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>Loop 2100E: Member School.  Repeat 3.  Guide page 23.</summary>
		private void ReadLoop2100E(Hx834_Member member) {
			member.ListMemberSchools.Clear();
			while(_segCur.IsType("NM1","M8")) {
				Hx834_School school=new Hx834_School();
				ReadLoop2100E_NM1(school);
				ReadLoop2100E_PER(school);
				ReadLoop2100E_N3(school);
				ReadLoop2100E_N4(school);
				member.ListMemberSchools.Add(school);
			}
			//D3 does not seem to need this information, based on the example data provided.
			if(member.ListMemberSchools.Count > 0) {
				member.Pat.SchoolName=member.ListMemberSchools[0].MemberSchool.NameLast;
			}
		}

		///<summary>NM1: Member School.  Situational.  Repeat 1.  Guide page 107.</summary>
		private void ReadLoop2100E_NM1(Hx834_School school) {
			if(!_segCur.IsType("NM1","M8")) {
				return;
			}
			school.MemberSchool=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>PER: Member School Communications Numbers.  Situational.  Repeat 1.  Guide page 109.</summary>
		private void ReadLoop2100E_PER(Hx834_School school) {
			if(!_segCur.IsType("PER","SK")) {
				return;
			}
			school.MemberSchoolCommunicationsNumbers=new X12_PER(_segCur);
			_segNum++;
		}

		///<summary>N3: Member School Street Address.  Situational.  Repeat 1.  Guide page 112.</summary>
		private void ReadLoop2100E_N3(Hx834_School school) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			school.MemberSchoolStreetAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Member School City, State, Zip Code.  Repeat 1.  Guide page 113.</summary>
		private void ReadLoop2100E_N4(Hx834_School school) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			school.MemberSchoolCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>Loop 2100F: Custodial Parent.  Repeat 1.  Guide page 23.</summary>
		private void ReadLoop2100F(Hx834_Member member) {
			if(!_segCur.IsType("NM1","S3")) {
				return;
			}
			ReadLoop2100F_NM1(member);
			ReadLoop2100F_PER(member);
			ReadLoop2100F_N3(member);
			ReadLoop2100F_N4(member);
			//TODO: Create a new patient for this person in family if not present.
		}

		///<summary>NM1: Custodial Parent.  Situational.  Repeat 1.  Guide page 115.</summary>
		private void ReadLoop2100F_NM1(Hx834_Member member) {
			if(!_segCur.IsType("NM1","S3")) {
				return;
			}
			member.CustodialParent=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>PER: Custodial Parent Communications Numbers.  Situational.  Repeat 1.  Guide page 118.</summary>
		private void ReadLoop2100F_PER(Hx834_Member member) {
			if(!_segCur.IsType("PER","PQ")) {
				return;
			}
			member.CustodialParentCommunicationsNumbers=new X12_PER(_segCur);
			_segNum++;
		}

		///<summary>N3: Custodial Parent Street Address.  Situational.  Repeat 1.  Guide page 121.</summary>
		private void ReadLoop2100F_N3(Hx834_Member member) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			member.CustodialParentStreetAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Custodial Parent City, State, Zip Code.  Situational.  Repeat 1.  Guide page 122.</summary>
		private void ReadLoop2100F_N4(Hx834_Member member) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			member.CustodialParentCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>Loop 2100G: Responsible Person.  Repeat 13.  Guide page 23.</summary>
		private void ReadLoop2100G(Hx834_Member member) {
			member.ListResponsiblePerson.Clear();
			while(_segCur.IsType("NM1","6Y","9K","E1","EI","EXS","GB","GD","J6","LR","QD","S1","TZ","X4")) {
				Hx834_ResponsiblePerson person=new Hx834_ResponsiblePerson();
				ReadLoop2100G_NM1(person);
				ReadLoop2100G_PER(person);
				ReadLoop2100G_N3(person);
				ReadLoop2100G_N4(person);
				member.ListResponsiblePerson.Add(person);
			}
			//TODO: Create a new patient for this person in family if not present.  Use Pat.ResponsParty to link FK.
		}

		///<summary>NM1: Responsible Person.  Situational.  Repeat 1.  Guide page 124.</summary>
		private void ReadLoop2100G_NM1(Hx834_ResponsiblePerson person) {
			if(!_segCur.IsType("NM1","6Y","9K","E1","EI","EXS","GB","GD","J6","LR","QD","S1","TZ","X4")) {
				return;
			}
			person.ResponsiblePerson=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>PER: Responsible Person Communications Numbers.  Situational.  Repeat 1.  Guide page 127.</summary>
		private void ReadLoop2100G_PER(Hx834_ResponsiblePerson person) {
			if(!_segCur.IsType("PER","IP")) {
				return;
			}
			person.ResponsiblePersonCommunicationsNumbers=new X12_PER(_segCur);
			_segNum++;
		}

		///<summary>N3: Responsible Person Street Address.  Situational.  Repeat 1.  Guide page 130.</summary>
		private void ReadLoop2100G_N3(Hx834_ResponsiblePerson person) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			person.ResponsiblePersonStreetAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Responsible Person City, State, Zip Code.  Situational.  Repeat 1.  Guide page 131.</summary>
		private void ReadLoop2100G_N4(Hx834_ResponsiblePerson person) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			person.ResponsiblePersonCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>Loop 2100H: Drop Off Location.  Repeat 1.  Guide page 23.</summary>
		private void ReadLoop2100H(Hx834_Member member) {
			if(!_segCur.IsType("NM1","45")) {
				return;
			}
			ReadLoop2100H_NM1(member);
			ReadLoop2100H_N3(member);
			ReadLoop2100H_N4(member);
			//Address where patient would like shipments sent.  Nowhere to store this information.
		}

		///<summary>NM1: Drop Off Location.  Situational.  Repeat 1.  Guide page 133.</summary>
		private void ReadLoop2100H_NM1(Hx834_Member member) {
			if(!_segCur.IsType("NM1","45")) {
				return;
			}
			member.DropOffLocation=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>N3: Drop Off Location Street Address.  Situational.  Repeat 1.  Guide page 135.</summary>
		private void ReadLoop2100H_N3(Hx834_Member member) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			member.DropOffLocationStreetAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Drop Off Location City, State, Zip Code.  Situational.  Repeat 1.  Guide page 136.</summary>
		private void ReadLoop2100H_N4(Hx834_Member member) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			member.DropOffLocationCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>Loop 2200: Disability Information.  Repeat >1.  Guide page 23.</summary>
		private void ReadLoop2200(Hx834_Member member) {
			member.ListDisabilityInformation.Clear();
			while(_segCur.IsType("DSB")) {
				Hx834_DisabilityInformation disabilityInfo=new Hx834_DisabilityInformation();
				ReadLoop2200_DSB(disabilityInfo);
				ReadLoop2200_DTP(disabilityInfo);
				member.ListDisabilityInformation.Add(disabilityInfo);
			}
			//Nowhere to store this information.
		}

		///<summary>DSB: Disability Information.  Situational.  Repeat 1.  Guide page 138.</summary>
		private void ReadLoop2200_DSB(Hx834_DisabilityInformation disabilityInfo) {
			if(!_segCur.IsType("DSB")) {
				return;
			}
			disabilityInfo.DisabilityInformation=new X12_DSB(_segCur);
			_segNum++;
		}

		///<summary>DTP: Disability Eligibility Dates.  Situational.  Repeat 2.  Guide page 140.</summary>
		private void ReadLoop2200_DTP(Hx834_DisabilityInformation disabilityInfo) {
			disabilityInfo.ListDisabilityEligibilityDates.Clear();
			while(_segCur.IsType("DTP","360","361")) {
				disabilityInfo.ListDisabilityEligibilityDates.Add(new X12_DTP(_segCur));
				_segNum++;
			}
		}

		///<summary>Loop 2300: Health Coverage.  Repeat 99.  Guide page 23.</summary>
		private void ReadLoop2300(Hx834_Member member) {
			member.ListHealthCoverage.Clear();
			while(_segCur.IsType("HD")) {
				Hx834_HealthCoverage healthCoverage=new Hx834_HealthCoverage();
				healthCoverage.Member=member;
				ReadLoop2300_HD(healthCoverage);
				ReadLoop2300_DTP(healthCoverage);
				ReadLoop2300_AMT(healthCoverage);
				ReadLoop2300_REF_1(healthCoverage);
				ReadLoop2300_REF_2(healthCoverage);
				ReadLoop2300_IDC(healthCoverage);
				ReadLoop2310(healthCoverage);
				ReadLoop2320(healthCoverage);
				member.ListHealthCoverage.Add(healthCoverage);
			}
		}

		///<summary>HD: Health Coverage.  Situational.  Repeat 1.  Guide page 141.</summary>
		private void ReadLoop2300_HD(Hx834_HealthCoverage healthCoverage) {
			if(!_segCur.IsType("HD")) {
				return;
			}
			healthCoverage.HealthCoverage=new X12_HD(_segCur);
			_segNum++;
			//HD01: Maintenance Type Code.  Useful when importing.  No conversion needed.
			//HD02: Not used
			//HD03: Insurance Line Code.  TODO: This might be useful, although the IDC segment identifies Dental/Health insurance.
			//HD04: Plan Coverage Descriptiopn.  Useless
			//HD05: Coverage Level Code.  Might be useful, although nowhere to store this information in specific.
			//HD06 through HD08: Not used
			//HD09: Late Enrollment Indicator.  Nowhere to store this information and it is useless anyway.
			//HD10 through HD11: Not used
		}

		///<summary>DTP: Health Coverage Dates.  Required.  Repeat 6.  Guide page 145.</summary>
		private void ReadLoop2300_DTP(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListHealthCoverageDates.Clear();
			while(_segCur.IsType("DTP","300","303","343","348","349","543","695")) {
				X12_DTP dtp=new X12_DTP(_segCur);
				healthCoverage.ListHealthCoverageDates.Add(dtp);
				_segNum++;
				if(dtp.DateTimeQualifier=="300") {//Enrollment Signature Date
				}
				else if(dtp.DateTimeQualifier=="303") {//Maintenance Effective
					//This is the effective date of a change where a member's coverage is not being added or removed.
				}
				else if(dtp.DateTimeQualifier=="343") {//Premium Paid to Date End
				}
				else if(dtp.DateTimeQualifier=="348") {//Benefit Begin
					//This is the effective date of coverage.  This code must always be sent when adding or reinstating coverage.
					healthCoverage.DateEffective=X12Parse.ToDate(dtp.DateTimePeriod);
				}
				else if(dtp.DateTimeQualifier=="349") {//Benefit End
					//The termination date represents the last date of coverage in which claims will be paid for the individual being terminated.
					//For example, if a date of 02/28/2001 is passed then claims for this individual will be paid through 11:50 p.m. on 02/28/2001.
					healthCoverage.DateTerm=X12Parse.ToDate(dtp.DateTimePeriod);
				}
				else if(dtp.DateTimeQualifier=="543") {//Last Premium Paid Date
				}
				else if(dtp.DateTimeQualifier=="695") {//Pervious Period
					//This value is only to be used when reporting Previous Coverage Months.
				}
			}
		}

		///<summary>AMT: Health Coverage Policy.  Situational.  Repeat 9.  Guide page 147.</summary>
		private void ReadLoop2300_AMT(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListHealthCoveragePolicies.Clear();
			while(_segCur.IsType("AMT","B9","C1","D2","EBA","FK","P3","R")) {
				healthCoverage.ListHealthCoveragePolicies.Add(new X12_AMT(_segCur));
				_segNum++;
			}
			//D3 does not seem to need this information, based on the example data provided.
		}

		///<summary>REF: Health Coverage Policy Number.  Situational.  Repeat 14.  Guide page 148.</summary>
		private void ReadLoop2300_REF_1(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListHealthCoveragePolicyNumbers.Clear();
			while(_segCur.IsType("REF","17","1L","9V","CE","E8","M7","PID","RB","X9","XM","XX1","XX2","ZX","ZZ")) {
				X12_REF policyNum=new X12_REF(_segCur);
				healthCoverage.ListHealthCoveragePolicyNumbers.Add(policyNum);
				_segNum++;
				if(policyNum.ReferenceIdQualifier=="17") {//Client Reporting Category
					//This seems to be the only code D3 cares about.  Not sure how to import this information yet, or if it is even useful.
				}
				else if(policyNum.ReferenceIdQualifier=="1L") {//Group or Policy Number
					//Required when a group number that applies to this individual's participation in the coverage passed in this HD loop
					//is required by the terms of the contract between the sponsor (sender) and payer (receiver);
					//if not required may be sent at the sender's discretion.
				}
				else if(policyNum.ReferenceIdQualifier=="9V") {//Payment Category
				}
				else if(policyNum.ReferenceIdQualifier=="CE") {//Class of Contract Code
				}
				else if(policyNum.ReferenceIdQualifier=="E8") {//Service Contract (Coverage) Number
				}
				else if(policyNum.ReferenceIdQualifier=="M7") {//Medical Assistance Cateogory
				}
				else if(policyNum.ReferenceIdQualifier=="PID") {//Program Identification Number
				}
				else if(policyNum.ReferenceIdQualifier=="RB") {//Rate code number
				}
				else if(policyNum.ReferenceIdQualifier=="X9") {//Internal Control Number
				}
				else if(policyNum.ReferenceIdQualifier=="XM") {//Issuer Number
				}
				else if(policyNum.ReferenceIdQualifier=="XX1") {//Special Program Code
				}
				else if(policyNum.ReferenceIdQualifier=="XX2") {//Service Area Code
				}
				else if(policyNum.ReferenceIdQualifier=="ZX") {//Country Code
				}
				else if(policyNum.ReferenceIdQualifier=="ZZ") {//Mutually Defined
					//Use this code for the Payment Plan Type Code (Annual or Quarterly) until a standard code is assigned.
				}
			}
		}

		///<summary>REF: Prior Coverage Months.  Situational.  Repeat 1.  Guide page 150.</summary>
		private void ReadLoop2300_REF_2(Hx834_HealthCoverage healthCoverage) {
			if(!_segCur.IsType("REF","QQ")) {
				return;
			}
			healthCoverage.PriorCoverageMonths=new X12_REF(_segCur);
			_segNum++;
			//D3 does not seem to need this information, based on the example data provided.
		}

		///<summary>IDC: IDentification Card.  Situational.  Repeat 3.  Guide page 152.</summary>
		private void ReadLoop2300_IDC(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListIdentificationCards.Clear();
			while(_segCur.IsType("IDC")) {
				healthCoverage.ListIdentificationCards.Add(new X12_IDC(_segCur));
				_segNum++;
			}
			//D3 does not seem to need this information, based on the example data provided.
		}

		///<summary>Loop 2310: Provider Information.  Repeat 30.  Guide page 23.</summary>
		private void ReadLoop2310(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListProviderInformation.Clear();
			//There are two different LX segments which could be at this spot.
			//The LX segments have the same simple format, so we have to look at the following segment to figure out which LX we are looking at.
			while(_segCur.IsType("LX") && (_segNum+1) < _listSegments.Count 
				&& _listSegments[_segNum+1].IsType("NM1","1X","3D","80","FA","OD","P3","QA","QN","Y2"))
			{
				Hx834_Provider prov=new Hx834_Provider();
				ReadLoop2310_LX(prov);
				ReadLoop2310_NM1(prov);
				ReadLoop2310_N3(prov);
				ReadLoop2310_N4(prov);
				ReadLoop2310_PER(prov);
				ReadLoop2310_PLA(prov);
				healthCoverage.ListProviderInformation.Add(prov);
			}
			//This information is useless, because the customer will already have provider information in their system.  Do not import.
		}

		///<summary>LX: Provider Information.  Situational.  Repeat 1.  Guide page 154.</summary>
		private void ReadLoop2310_LX(Hx834_Provider prov) {
			if(!_segCur.IsType("LX")) {
				return;
			}
			prov.ProviderInformation=new X12_LX(_segCur);
			_segNum++;
		}

		///<summary>NM1: Provider Name.  Required.  Repeat 1.  Guide page 155.</summary>
		private void ReadLoop2310_NM1(Hx834_Provider prov) {
			prov.ProviderName=new X12_NM1(_segCur,"1X","3D","80","FA","OD","P3","QA","QN","Y2");
			_segNum++;
		}

		///<summary>N3: Provider Address. Situational.  Repeat 2.  Guide page 158.</summary>
		private void ReadLoop2310_N3(Hx834_Provider prov) {
			prov.ListProviderAddresses.Clear();
			while(_segCur.IsType("N3")) {
				prov.ListProviderAddresses.Add(new X12_N3(_segCur));
				_segNum++;
			}
		}

		///<summary>N4: Provider City, State, Zip Code.  Situational.  Repeat 1.  Guide page 159.</summary>
		private void ReadLoop2310_N4(Hx834_Provider prov) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			prov.ProviderCityStateZipCode=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>PER: Provider Communications Numbers.  Situational.  Repeat 2.  Guide page 161.</summary>
		private void ReadLoop2310_PER(Hx834_Provider prov) {
			prov.ListProviderCommunicationsNumbers.Clear();
			while(_segCur.IsType("PER","IC")) {
				prov.ListProviderCommunicationsNumbers.Add(new X12_PER(_segCur));
				_segNum++;
			}
		}

		///<summary>PLA: Provider Change Reason.  Situational.  Repeat 1.  Guide page 164.</summary>
		private void ReadLoop2310_PLA(Hx834_Provider prov) {
			if(!_segCur.IsType("PLA")) {
				return;
			}
			prov.ProviderChangeReason=new X12_PLA(_segCur);
			_segNum++;
		}

		///<summary>Loop 2320: Coordination of Benefits.  Repeat 5.  Guide page 23.</summary>
		private void ReadLoop2320(Hx834_HealthCoverage healthCoverage) {
			healthCoverage.ListCoordinationOfBeneifts.Clear();
			while(_segCur.IsType("COB")) {
				Hx834_Cob cob=new Hx834_Cob();
				ReadLoop2320_COB(cob);
				ReadLoop2320_REF(cob);
				ReadLoop2320_DTP(cob);
				ReadLoop2330(cob);
				healthCoverage.ListCoordinationOfBeneifts.Add(cob);
			}
			//Nowhere to store this information.  Is this information available in other Health Coverage segments for the current member?
		}

		///<summary>COB: Coordination of Benefits.  Situational.  Repeat 1.  Guide page 166.</summary>
		private void ReadLoop2320_COB(Hx834_Cob cob) {
			if(!_segCur.IsType("COB")) {
				return;
			}
			cob.CoordinationOfBenefits=new X12_COB(_segCur);
			_segNum++;
		}

		///<summary>REF: Additional Coordination of Benefits Identifiers.  Situational.  Repeat 4.  Guide page 168.</summary>
		private void ReadLoop2320_REF(Hx834_Cob cob) {
			cob.ListAdditionalCobIdentifiers.Clear();
			while(_segCur.IsType("REF","60","6P","SY","ZZ")) {
				cob.ListAdditionalCobIdentifiers.Add(new X12_REF(_segCur));
				_segNum++;
			}
		}

		///<summary>DTP: Coordination of Benefits Eligibility Dates.  Situational.  Repeat 2.  Guide page 170.</summary>
		private void ReadLoop2320_DTP(Hx834_Cob cob) {
			cob.ListCobEligibilityDates.Clear();
			while(_segCur.IsType("DTP","344","345")) {
				cob.ListCobEligibilityDates.Add(new X12_DTP(_segCur));
				_segNum++;
			}
		}

		///<summary>Loop 2330: Coordination of Benefits Related Entity.  Repeat 3.  Guide page 23.</summary>
		private void ReadLoop2330(Hx834_Cob cob) {
			cob.ListCobRelatedEntities.Clear();
			while(_segCur.IsType("NM1","36","GW","IN")) {
				Hx834_CobRelatedEntity cobre=new Hx834_CobRelatedEntity();
				ReadLoop2330_NM1(cobre);
				ReadLoop2330_N3(cobre);
				ReadLoop2330_N4(cobre);
				ReadLoop2330_PER(cobre);
				cob.ListCobRelatedEntities.Add(cobre);
			}
		}

		///<summary>NM1: Coordination of Benefits Releated Entity.  Situational.  Repeat 1.  Guide page 171.</summary>
		private void ReadLoop2330_NM1(Hx834_CobRelatedEntity cobre) {
			if(!_segCur.IsType("NM1","36","GW","IN")) {
				return;
			}
			cobre.CobRelatedEntity=new X12_NM1(_segCur);
			_segNum++;
		}

		///<summary>N3: Coordination of Benefits Related Entity Address.  Situational.  Repeat 1.  Guide page 173.</summary>
		private void ReadLoop2330_N3(Hx834_CobRelatedEntity cobre) {
			if(!_segCur.IsType("N3")) {
				return;
			}
			cobre.CobRelatedEntityAddress=new X12_N3(_segCur);
			_segNum++;
		}

		///<summary>N4: Coordination of Benefits Other Insurance Company City, State, Zip Code.  Repeat 1.  Guide page 174.</summary>
		private void ReadLoop2330_N4(Hx834_CobRelatedEntity cobre) {
			if(!_segCur.IsType("N4")) {
				return;
			}
			cobre.CobOtherInsurance=new X12_N4(_segCur);
			_segNum++;
		}

		///<summary>PER: Administrative Communications Contact.  Situational.  Repeat 1.  Guide page 176.</summary>
		private void ReadLoop2330_PER(Hx834_CobRelatedEntity cobre) {
			if(!_segCur.IsType("PER","CN")) {
				return;
			}
			cobre.AdministrativeCommunicationsContact=new X12_PER(_segCur);
			_segNum++;
		}

		///<summary>LS: Additional Reporting Categories.  Situational.  Repeat 1.  Guide page 178.</summary>
		private void ReadLoop2000_LS(Hx834_Member member) {
			if(!_segCur.IsType("LS")) {
				return;
			}
			member.AdditionalReportingCategories=new X12_LS(_segCur);
			_segNum++;
		}

		///<summary>Loop 2700: Member Reporting Categories.  Repeat >1.  Guide page 24.</summary>
		private void ReadLoop2700(Hx834_Member member) {
			member.ListMemberReportingCategories.Clear();
			while(_segCur.IsType("LX")) {
				Hx834_MemberReportingCategory mrc=new Hx834_MemberReportingCategory();
				ReadLoop2700_LX(mrc);
				ReadLoop2750(mrc);
				member.ListMemberReportingCategories.Add(mrc);
			}
			//D3 provides this information.  We may be able to use some of it.  Not sure if this information is useful or where to store it.
		}

		///<summary>LX: Member Reporting Categories.  Situational.  Repeat 1.  Guide page 179.</summary>
		private void ReadLoop2700_LX(Hx834_MemberReportingCategory mrc) {
			if(!_segCur.IsType("LX")) {
				return;
			}
			mrc.MemberReportingCategories=new X12_LX(_segCur);
			_segNum++;
		}

		///<summary>Loop 2750: Reporting Category.  Repeat 1.  Guide page 24.</summary>
		private void ReadLoop2750(Hx834_MemberReportingCategory mrc) {
			mrc.ReportingCategory=new Hx834_ReportingCategory();
			ReadLoop2750_N1(mrc.ReportingCategory);
			ReadLoop2750_REF(mrc.ReportingCategory);
			ReadLoop2750_DTP(mrc.ReportingCategory);
		}

		///<summary>N1: Reporting Category.  Situational.  Repeat 1.  Guide page 180.</summary>
		private void ReadLoop2750_N1(Hx834_ReportingCategory rc) {
			if(!_segCur.IsType("N1","75")) {
				return;
			}
			rc.ReportingCategory=new X12_N1(_segCur);
			_segNum++;
		}

		///<summary>REF: Reporting Category Reference.  Situational.  Repeat 1.  Guide page 181.</summary>
		private void ReadLoop2750_REF(Hx834_ReportingCategory rc) {
			if(!_segCur.IsType("REF","00","17","18","19","26","3L","6M","9V","9X","GE","LU","PID","XX1","XX2","YY","ZZ")) {
				return;
			}
			rc.ReportingCategoryReference=new X12_REF(_segCur);
			_segNum++;
		}

		///<summary>DTP: Reporting Category Date.  Situational.  Repeat 1.  Guide page 183.</summary>
		private void ReadLoop2750_DTP(Hx834_ReportingCategory rc) {
			if(!_segCur.IsType("DTP","007")) {
				return;
			}
			rc.ReportingCategoryDate=new X12_DTP(_segCur);
			_segNum++;
		}

		///<summary>LE: Additional Reporting Categories Loop Termination.  Situational.  Repeat 1.  Guide page 185.</summary>
		private void ReadLoop2000_LE(Hx834_Member member) {
			if(!_segCur.IsType("LE")) {
				return;
			}
			member.AdditionalReportingCategoriesLoopTermination=new X12_LE(_segCur);
			_segNum++;
		}

	}

	#region Helper Classes

	///<summary>Loop 1000C</summary>
	public class Hx834_Broker {
		///<summary>Loop 1000C N1</summary>
		public X12_N1 Name;
		///<summary>Loop 1100C</summary>
		public X12_ACT TpaBrokerAccountInformation;
	}

	///<summary>Loop 2320</summary>
	public class Hx834_Cob {
		///<summary>Loop 2320 COB</summary>
		public X12_COB CoordinationOfBenefits;
		///<summary>Loop 2320 REF.  Repeat 4.</summary>
		public List <X12_REF> ListAdditionalCobIdentifiers=new List<X12_REF>();
		///<summary>Loop 2320 DTP.  Repeat 2.</summary>
		public List <X12_DTP> ListCobEligibilityDates=new List<X12_DTP>();
		///<summary>Loop 2330.  Repeat 3.</summary>
		public List <Hx834_CobRelatedEntity> ListCobRelatedEntities=new List<Hx834_CobRelatedEntity>();
	}

	///<summary>Loop 2330</summary>
	public class Hx834_CobRelatedEntity {
		///<summary>Loop 2330 NM1</summary>
		public X12_NM1 CobRelatedEntity;
		///<summary>Loop 2330 N3</summary>
		public X12_N3 CobRelatedEntityAddress;
		///<summary>Loop 2330 N4</summary>
		public X12_N4 CobOtherInsurance;
		///<summary>Loop 2330 PER</summary>
		public X12_PER AdministrativeCommunicationsContact;
	}

	///<summary>Loop 2200</summary>
	public class Hx834_DisabilityInformation {
		///<summary>Loop 2200 DSB</summary>
		public X12_DSB DisabilityInformation;
		///<summary>Loop 2200 DTP.  Repeat 2.</summary>
		public List <X12_DTP> ListDisabilityEligibilityDates=new List<X12_DTP>();
	}

	///<summary>Loop 2100D</summary>
	public class Hx834_Employer {
		///<summary>Loop 2100D NM1</summary>
		public X12_NM1 MemberEmployer;
		///<summary>Loop 2100D PER</summary>
		public X12_PER MemberEmployerCommunicationsNumbers;
		///<summary>Loop 2100D N3</summary>
		public X12_N3 MemberEmployerStreetAddress;
		///<summary>Loop 2100D N4</summary>
		public X12_N4 MemberEmployerCityStateZipCode;
	}

	///<summary>Loop 2300</summary>
	public class Hx834_HealthCoverage {
		///<summary>A reference to the member who owns this health coverage.</summary>
		public Hx834_Member Member;
		///<summary>Loop 2300 HD</summary>
		public X12_HD HealthCoverage;
		///<summary>Loop 2300 DTP.  Repeat 6.</summary>
		public List<X12_DTP> ListHealthCoverageDates=new List<X12_DTP>();
		///<summary>Loop 2300 AMT.  Repeat 9.</summary>
		public List<X12_AMT> ListHealthCoveragePolicies=new List<X12_AMT>();
		///<summary>Loop 2300 REF_1.  Repeat 14.</summary>
		public List<X12_REF> ListHealthCoveragePolicyNumbers=new List<X12_REF>();
		///<summary>Loop 2300 REF_2.</summary>
		public X12_REF PriorCoverageMonths;
		///<summary>Loop2300 IDC.  Repeat 3.</summary>
		public List<X12_IDC> ListIdentificationCards=new List<X12_IDC>();
		///<summary>Loop 2310.  Repeat 30.</summary>
		public List<Hx834_Provider> ListProviderInformation=new List<Hx834_Provider>();
		///<summary>Loop 2320.  Repeat 5.</summary>
		public List<Hx834_Cob> ListCoordinationOfBeneifts=new List<Hx834_Cob>();
		///<summary>The date in which insurance coverage begins.</summary>
		public DateTime DateEffective;
		///<summary>The date in which insurance coverage ends.</summary>
		public DateTime DateTerm;

		public string GetCoverageMaintTypeDescript() {
			if(HealthCoverage==null) {
				return "";
			}
			if(HealthCoverage.MaintenanceTypeCode=="001") {//Change
				return "Change";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="002") {//Delete
				return "Delete";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="021") {//Addition
				return "Addition";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="024") {//Cancellation or Termination
				return "Cancel";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="025") {//Reinstatement
				return "Reinstate";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="026") {//Correction
				return "Correct";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="030") {//Audit or Compare
				return "Audit";
			}
			else if(HealthCoverage.MaintenanceTypeCode=="032") {//Employee Information Not Applicable
				return "NA";
			}
			return "";
		}

	}

	///<summary>Loop 2000</summary>
	public class Hx834_Member {
		///<summary>A reference to the transaction who owns this member.</summary>
		public Hx834_Tran Tran;
		///<summary>Loop 2000 INS</summary>
		public X12_INS MemberLevelDetail;
		///<summary>Loop 2000 REF_1</summary>
		public X12_REF SubscriberIdentifier;
		///<summary>Loop 2000 REF_2</summary>
		public X12_REF MemberPolicyNumber;
		///<summary>Loop 2000 REF_3 (repeat 13)</summary>
		public List <X12_REF> ListMemberSupplementalIdentifiers=new List<X12_REF>();
		///<summary>Loop 2000 DTP (repeat 24)</summary>
		public List <X12_DTP> ListMemberLevelDates=new List<X12_DTP>();
		///<summary>Loop 2100A NM1</summary>
		public X12_NM1 MemberName;
		///<summary>Loop 2100A PER.</summary>
		public X12_PER MemberCommunicationsNumbers;
		///<summary>Loop 2100A N3</summary>
		public X12_N3 MemberResidenceStreetAddress;
		///<summary>Loop 2100A N4</summary>
		public X12_N4 MemberCityStateZipCode;
		///<summary>Loop 2100A DMG</summary>
		public X12_DMG MemberDemographics;
		///<summary>Loop 2100A EC</summary>
		public List<X12_EC> ListEmploymentClass=new List<X12_EC>();
		///<summary>Loop 2100A ICM</summary>
		public X12_ICM MemberIncome;
		///<summary>Loop 2100A AMT</summary>
		public List<X12_AMT> ListMemberPolicyAmounts=new List<X12_AMT>();
		///<summary>Loop 2100A HLH</summary>
		public X12_HLH MemberHealthInformation;
		///<summary>Loop 2100A LUI</summary>
		public List<X12_LUI> ListMemberLanguages=new List<X12_LUI>();
		///<summary>Loop 2100B NM1</summary>
		public X12_NM1 IncorrectMemberName;
		///<summary>Loop 2100B DMG</summary>
		public X12_DMG IncorrectMemberDemographics;
		///<summary>Loop 2100C NM1</summary>
		public X12_NM1 MemberMailingAddress;
		///<summary>Loop 2100C N3</summary>
		public X12_N3 MemberMailStreetAddress;
		///<summary>Loop 2100C N4</summary>
		public X12_N4 MemberMailCityStateZipCode;
		///<summary>Loop 2100D</summary>
		public List <Hx834_Employer> ListMemberEmployers=new List<Hx834_Employer>();
		///<summary>Loop 2100E</summary>
		public List <Hx834_School> ListMemberSchools=new List<Hx834_School>();
		///<summary>Loop 2100F NM1</summary>
		public X12_NM1 CustodialParent;
		///<summary>Loop 2100F PER</summary>
		public X12_PER CustodialParentCommunicationsNumbers;
		///<summary>Loop 2100F N3</summary>
		public X12_N3 CustodialParentStreetAddress;
		///<summary>Loop 2100F N4</summary>
		public X12_N4 CustodialParentCityStateZipCode;
		///<summary>Loop 2100G</summary>
		public List<Hx834_ResponsiblePerson> ListResponsiblePerson=new List<Hx834_ResponsiblePerson>();
		///<summary>Loop 2100H NM1</summary>
		public X12_NM1 DropOffLocation;
		///<summary>Loop 2100H N3</summary>
		public X12_N3 DropOffLocationStreetAddress;
		///<summary>Loop 2100H N4</summary>
		public X12_N4 DropOffLocationCityStateZipCode;
		///<summary>Loop 2200</summary>
		public List<Hx834_DisabilityInformation> ListDisabilityInformation=new List<Hx834_DisabilityInformation>();
		///<summary>Loop 2300</summary>
		public List<Hx834_HealthCoverage> ListHealthCoverage=new List<Hx834_HealthCoverage>();
		///<summary>Loop 2000 LS</summary>
		public X12_LS AdditionalReportingCategories;
		///<summary>Loop 2700.  Repeat >1.</summary>
		public List<Hx834_MemberReportingCategory> ListMemberReportingCategories=new List<Hx834_MemberReportingCategory>();
		///<summary>Loop 2000 LE</summary>
		public X12_LE AdditionalReportingCategoriesLoopTermination;
		///<summary>The patient when converted to an OD object.</summary>
		public Patient Pat=new Patient();
		///<summary>List of patient races.  Update db with these races when the patient is being updated.</summary>
		public List<PatientRace> ListPatRaces=new List<PatientRace>();
		///<summary>Reltaionship to subscriber.  Specified at member level in format.</summary>
		public Relat PlanRelat;
		///<summary>Insurance filing code.  Specified at member level in format.</summary>
		public InsFilingCode InsFiling;
		///<summary>Flag indicating if patient desires to release medical information.  Corresponds to inssub.ReleaseInfo.
		///Specified at member level in format.</summary>
		public bool IsReleaseInfo;
		///<summary>Subscriber ID for ins plan.  Specified at member level in format.</summary>
		public string SubscriberId;
		///<summary>The insurance plan group number.  Specified at member level in format.</summary>
		public string GroupNum;

		///<summary>Converts the code in MemberLevelDetail.MaintenanceTypeCode to a human readable string.</summary>
		public string GetPatMaintTypeDescript() {
			if(MemberLevelDetail==null) {
				return "";
			}
			if(MemberLevelDetail.MaintenanceTypeCode=="001") {//Change
				return "Change";
			}
			else if(MemberLevelDetail.MaintenanceTypeCode=="021") {//Addition
				return "Addition";
			}
			else if(MemberLevelDetail.MaintenanceTypeCode=="024") {//Cancellation or Termination
				return "Cancel";
			}
			else if(MemberLevelDetail.MaintenanceTypeCode=="025") {//Reinstatement
				return "Reinstate";
			}
			else if(MemberLevelDetail.MaintenanceTypeCode=="030") {//Audit or Compare
				return "Audit";
			}
			return "";
		}

		///<summary>Copies the data from applicable member fields into the given patDb and updates patDb in the database.</summary>
		public Patient MergePatientIntoDbPatient(Patient patDb) {
			Patient patDbOld=patDb.Copy();
			if(Pat.StudentStatus!=null) {//Student status is situational information.  Only overwrite existing value if a new value was specified.
				patDb.StudentStatus=Pat.StudentStatus;
			}
			if(Pat.DateTimeDeceased.Year > 1880) {//Date deceased is situational information.  Only overwrite existing value if a new value was specified.
				patDb.DateTimeDeceased=Pat.DateTimeDeceased;
			}
			patDb.LName=Pat.LName;//Last Name is a required field.  This will only change the patDb if the patDb was matched based on SSN.
			if(Pat.SSN!="") {//SSN is situational information.  Only overwrite existing value if a new value was specified.
				patDb.SSN=Pat.SSN;
				patDb.MiddleI=Pat.MiddleI;//The patinet Middle Initial must have been specified, as required by the format.  May be blank.
				patDb.FName=Pat.FName;//The patinet First Name must have been specified, as required by the format.  May be blank.
			}
			else if(Pat.MiddleI!="") {//Middle Name is situational information.  Only overwrite existing value if a new value was specified.
				patDb.MiddleI=Pat.MiddleI;
				patDb.FName=Pat.FName;//The patinet First Name must have been specified, as required by the format.  May be blank.
			}
			else if(Pat.FName!="") {//First Name is situational information.  Only overwrite existing value if a new value was specified.
				patDb.FName=Pat.FName;
			}			
			if(Pat.AddrNote!=null) {//Additinal contact information is situational information.  Only overwrite existing value if a new value was specified.
				//For now we will not import because this information is not very useful.
				//Additionally, importing this data is problematic because we do not know if we should overwrite, or if we should append to the existing data.
				//patDb.AddrNote=Pat.AddrNote;
			}
			if(Pat.WirelessPhone!=null) {//Wireless Phone is situational information.  Only overwrite existing value if a new value was specified.
				patDb.WirelessPhone=Pat.WirelessPhone;
			}
			if(Pat.Email!=null) {//Email is situational information.  Only overwrite existing value if a new value was specified.
				patDb.Email=Pat.Email;
			}
			if(Pat.HmPhone!=null) {//Home Phone is situational information.  Only overwrite existing value if a new value was specified.
				patDb.HmPhone=Pat.HmPhone;
			}
			if(Pat.WkPhone!=null) {//Work Phone is situational information.  Only overwrite existing value if a new value was specified.
				patDb.WkPhone=Pat.WkPhone;
			}
			if(Pat.Address2!=null) {//Address is situational information.  Only overwrite existing value if a new value was specified.
				patDb.Address2=Pat.Address2;
				patDb.Address=Pat.Address;//The patient Address must have been specified, as required by the format.  May be blank.
			}
			else if(Pat.Address!=null) {//Address is situational information.  Only overwrite existing value if a new value was specified.
				patDb.Address=Pat.Address;
			}
			if(Pat.Zip!=null) {
				patDb.Zip=Pat.Zip;
				patDb.State=Pat.State;//The patient State must have been specified, as required by the format.  May be blank.
				patDb.City=Pat.City;//The patient City must have been specified, as required by the format.  May be blank.
			}
			else if(Pat.State!=null) {
				patDb.State=Pat.State;
				patDb.City=Pat.City;//The patient City must have been specified, as required by the format.  May be blank.
			}
			else if(Pat.City!=null) {
				patDb.City=Pat.City;
			}
			if(MemberDemographics!=null) {
				patDb.Birthdate=Pat.Birthdate;//Birthdate is a required field.
				patDb.Gender=Pat.Gender;//Gender is a required field.
				if(MemberDemographics.MaritalStatusCode!="") {//The patient marital status is situational information.  Only overwrite if specified.
					patDb.Position=Pat.Position;
				}
				if(MemberDemographics.CompositeRaceOrEthnicityInformation!="") {
					PatientRaces.Reconcile(patDb.PatNum,ListPatRaces);//Insert, Update, Delete if needed.
				}
			}
			if(ListMemberLanguages.Count > 0) {//The patient language is situational information.  Only overwrite if specified.
				patDb.Language=Pat.Language;
			}
			if(ListMemberSchools.Count > 0) {//The patient school is situational information.  Only overwrite if specified.
				patDb.SchoolName=Pat.SchoolName;
			}
			if(Crud.PatientCrud.UpdateComparison(patDb,patDbOld)) {
				Patients.Update(patDb,patDbOld);
				SecurityLogs.MakeLogEntry(Permissions.PatientEdit,patDb.PatNum,"Demographics edited from Import Ins Plans 834.",LogSources.InsPlanImport834);
			}
			return patDb;
		}

	}

	///<summary>Loop 2700</summary>
	public class Hx834_MemberReportingCategory {
		///<summary>Loop 2700 LX</summary>
		public X12_LX MemberReportingCategories;
		///<summary>Loop 2750.  Repeat 1.</summary>
		public Hx834_ReportingCategory ReportingCategory;
	}

	///<summary>Loop 2310</summary>
	public class Hx834_Provider {
		///<summary>Loop 2310 LX</summary>
		public X12_LX ProviderInformation;
		///<summary>Loop 2310 NM1</summary>
		public X12_NM1 ProviderName;
		///<summary>Loop 2310 N3.  Repeat 2.</summary>
		public List<X12_N3> ListProviderAddresses=new List<X12_N3>();
		///<summary>Loop 2310 N4</summary>
		public X12_N4 ProviderCityStateZipCode;
		///<summary>Loop 2310 PER.  Repeat 2.</summary>
		public List <X12_PER> ListProviderCommunicationsNumbers=new List<X12_PER>();
		///<summary>Loop 2310 PLA</summary>
		public X12_PLA ProviderChangeReason;
	}

	///<summary>Loop 2750</summary>
	public class Hx834_ReportingCategory {
		///<summary>Loop 2750 N1</summary>
		public X12_N1 ReportingCategory;
		///<summary>Loop 2750 REF</summary>
		public X12_REF ReportingCategoryReference;
		///<summary>Loop 2750 DTP</summary>
		public X12_DTP ReportingCategoryDate;
	}

	///<summary>Loop 2100G</summary>
	public class Hx834_ResponsiblePerson {
		///<summary>Loop 2100G NM1</summary>
		public X12_NM1 ResponsiblePerson;
		///<summary>Loop 2100G PER</summary>
		public X12_PER ResponsiblePersonCommunicationsNumbers;
		///<summary>Loop 2100G N3</summary>
		public X12_N3 ResponsiblePersonStreetAddress;
		///<summary>Loop 2100G N4</summary>
		public X12_N4 ResponsiblePersonCityStateZipCode;
	}

	///<summary>Loop 2100E</summary>
	public class Hx834_School {
		///<summary>Loop 2100E NM1</summary>
		public X12_NM1 MemberSchool;
		///<summary>Loop 2100E PER</summary>
		public X12_PER MemberSchoolCommunicationsNumbers;
		///<summary>Loop 2100E N3</summary>
		public X12_N3 MemberSchoolStreetAddress;
		///<summary>Loop 2100E N4</summary>
		public X12_N4 MemberSchoolCityStateZipCode;
	}

	///<summary>Loop ST</summary>
	public class Hx834_Tran {
		///<summary>Loop ST BGN</summary>
		public X12_BGN BeginningSegment;
		///<summary>Loop ST REF</summary>
		public X12_REF TransactionSetPolicyNumber;
		///<summary>Loop ST DTP.  Repeat >1.</summary>
		public List<X12_DTP> ListFileEffectiveDates=new List<X12_DTP>();
		///<summary>Loop ST QTY</summary>
		public List<X12_QTY> ListTransactionSetControlTotals=new List<X12_QTY>();
		///<summary>Loop 1000A N1</summary>
		public X12_N1 SponsorName;
		///<summary>Loop 1000B N1</summary>
		public X12_N1 Payer;
		///<summary>Loop 1000C N1 and Loop 1100C ACT.</summary>
		public List<Hx834_Broker> ListBrokers=new List<Hx834_Broker>();
		///<summary>Loop 2000</summary>
		public List<Hx834_Member> ListMembers=new List<Hx834_Member>();
	}

	#endregion Helper Classes
}