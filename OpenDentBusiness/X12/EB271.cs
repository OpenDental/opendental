using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OpenDentBusiness
{
	///<summary>One EB segment from a 271.  Helps to organize a 271 for presentation to the user.</summary>
	public class EB271{
		public X12Segment Segment;
		///<summary>Can be null if the segment can't be translated to an appropriate benefit.  Many fields of the Benefit won't be used.  Just ones needed for display.</summary>
		public Benefit Benefitt;
		private static List<EB01> eb01;
		private static List<EB02> eb02;
		private static List<EB03> eb03;
		///<summary>Since element 4 is descriptive rather than useful for import, we will leave it like this</summary>
		private static Dictionary<string,string> EB04;
		private static List<EB06> eb06;
		private static List<EB09> eb09;
		/// <summary>Some EB segments have a few segments that follow them which should all be considered together as one "benefit".  Eg dates, addresses.</summary>
		public List<X12Segment> SupplementalSegments;

		public EB271(X12Segment segment,bool isInNetwork,bool isCoinsuranceInverted,X12Segment segHsd=null) {
			if(eb01==null) {
				FillDictionaries();
			}
			if(segment==null) {//Should never happen except in rare scenarios where we want to spoof an EB271 object.
				return;
			}
			Segment=segment;
			SupplementalSegments=new List<X12Segment>();
			//start pattern matching to generate closest Benefit
			EB01 eb01val=eb01.Find(EB01MatchesCode);
			EB02 eb02val=eb02.Find(EB02MatchesCode);
			EB03 eb03val=eb03.Find(EB03MatchesCode);
			EB06 eb06val=eb06.Find(EB06MatchesCode);
			EB09 eb09val=eb09.Find(EB09MatchesCode);
			ProcedureCode proccode=null;
			if(ProcedureCodes.IsValidCode(Segment.Get(13,2))) {
				proccode=ProcedureCodes.GetProcCode(Segment.Get(13,2));
			}
			if(!eb01val.IsSupported
				|| (eb02val!=null && !eb02val.IsSupported)
				|| (eb03val!=null && !eb03val.IsSupported)
				|| (eb06val!=null && !eb06val.IsSupported)
				|| (eb09val!=null && !eb09val.IsSupported)) 
			{
				Benefitt=null;
				return;
			}
			if(eb01val.BenefitType==InsBenefitType.ActiveCoverage	&& Segment.Get(3)=="30") {
				Benefitt=null;
				return;
			}
			if(eb01val.BenefitType==InsBenefitType.ActiveCoverage && proccode!=null) {
				//A code is covered.  Informational only.
				Benefitt=null;
				return;
			}
			if(Segment.Get(8)!="") {//if percentage
				//must have either a category or a proc code
				if(proccode==null) {//if no proc code is specified
					if(eb03val==null || eb03val.ServiceType==EbenefitCategory.None || eb03val.ServiceType==EbenefitCategory.General) {//and no category specified
						Benefitt=null;
						return;
					}
				}
			}
			//coinsurance amounts are handled with fee schedules rather than benefits
			if(eb01val.BenefitType==InsBenefitType.CoPayment || eb01val.BenefitType==InsBenefitType.CoInsurance) {
				if(Segment.Get(7)!="") {//and a monetary amount specified
					Benefitt=null;
					return;
				}
			}
			//a limitation without an amount is meaningless
			if(eb01val.BenefitType==InsBenefitType.Limitations
				&& segHsd==null)//Some benefits do not have monetary value but limit service in a time period.  Originally done for customer 27936.
			{
				if(Segment.Get(7)=="") {//no monetary amount specified
					Benefitt=null;
					return;
				}
			}
			if(isInNetwork && (Segment.Get(12)=="N" || Segment.Get(12)=="U")) {
				Benefitt=null;
				return;
			}
			if(!isInNetwork && Segment.Get(12)=="Y") {
				Benefitt=null;
				return;
			}
			//if only a quantity is specified with no qualifier, it's meaningless
			if(Segment.Get(10)!="" && eb09val==null) {
				Benefitt=null;
				return;
			}
			//if only a qualifier is specified with no quantity, it's meaningless
			if(eb09val!=null && Segment.Get(10)=="") {
				Benefitt=null;
				return;
			}
			//if quantity is larger than the largest quantity we support, then ignore.
			//we have only seen 1 case ever where the quantity reported was "2000" procedure limitation per calendar year.
			if(PIn.Double(Segment.Get(10)) > (double)byte.MaxValue) {
				Benefitt=null;
				return;
			}
			Benefitt=new Benefit();
			//1
			Benefitt.BenefitType=eb01val.BenefitType;
			//2
			if(eb02val!=null) {
				Benefitt.CoverageLevel=eb02val.CoverageLevel;
			}
			//3
			if(eb03val!=null) {
				Benefitt.CovCatNum=CovCats.GetForEbenCat(eb03val.ServiceType).CovCatNum;
			}
			//4-Insurance type - we ignore.
			//5-Plan description - we ignore.
			//6
			if(eb06val!=null) {
				Benefitt.TimePeriod=eb06val.TimePeriod;
			}
			//7
			if(Segment.Get(7)!="") {
				Benefitt.MonetaryAmt=PIn.Double(Segment.Get(7));//Monetary amount. Situational
			}
			//8
			if(Segment.Get(8)!="") {
				if(isCoinsuranceInverted && Benefitt.BenefitType==InsBenefitType.CoInsurance) {//Some carriers incorrectly send insurance percentage.
					Benefitt.Percent=(int)(PIn.Double(Segment.Get(8))*100);//Percent. Came to us inverted, do Not Invert.
				}
				else {
					//OD shows the percentage paid by Insurance by default.
					//Some carriers submit 271s to us showing percentage paid by Patient, so we need to invert this case to match OD expectations.
					Benefitt.Percent=100-(int)(PIn.Double(Segment.Get(8))*100);//Percent. Invert.	
				}
				Benefitt.CoverageLevel=BenefitCoverageLevel.None;
			}
			//9-Quantity qualifier
			if(eb09val!=null) {
				Benefitt.QuantityQualifier=eb09val.QuantityQualifier;
			}
			//10-Quantity
			if(Segment.Get(10)!="") {
				Benefitt.Quantity=(byte)PIn.Double(Segment.Get(10));//Example: "19.0" with Quantity qualifier "S7" (age).
			}
			//11-Authorization. Ignored.
			//12-In network. Ignored.
			//13-proc
			if(proccode!=null) {
				Benefitt.CodeNum=proccode.CodeNum;//element 13,2
			}
			if(Benefitt.BenefitType==InsBenefitType.Limitations
				&& proccode!=null //Valid ADA code.
				&& segHsd!=null)
			{
				if(segHsd.Elements.Length < 6 || segHsd.Elements[2]=="" || segHsd.Elements[5]=="") {
					Benefitt=null;
					return;
				}
				Benefitt.Quantity=PIn.Byte(segHsd.Elements[2]);//HSD02: Quantity.
				Benefitt.TimePeriod=eb06.FirstOrDefault(x => x.Code==segHsd.Elements[5]).TimePeriod;//HSD05: Frequency.
			}
		}

		///<summary>The most human-readable description possible.  This is only used in one place, the 270/271 window.</summary>
		public string GetDescription(bool isMessageMode,bool isCoinsurancePatPays) {
			bool containsAddress=false;
			bool containsDate=false;
			for(int i=0;i<SupplementalSegments.Count;i++) {
				if(SupplementalSegments[i].SegmentID=="LS") {
					containsAddress=true;
				}
				if(SupplementalSegments[i].SegmentID=="DTP") {
					containsDate=true;
				}
			}
			if(containsAddress) {
				return GetDescriptionForAddress();
			}
			if(containsDate) {
				return GetDescriptionForDate();
			}
			if(Segment.Get(1)=="1" && Segment.Get(13)!="") {//active coverage and a proc code indicated
				//informational only
				return GetDescriptionForCodeCovered();
			}
			//if a co-insurance, and a percentage, and a proc code, then special display
			if(Segment.Get(1)=="A" && Segment.Get(8)!="" && Segment.Get(13)!="") {
				return GetDescriptionForPercentCode();
			}
			string retVal="";
			string txt;
			txt=GetDescript(1,isMessageMode);//Eligibility or benefit information. Required
			if(txt!="") {
				retVal+=txt;
			}
			/*//only show coverage level for things like deductible and annual max
			txt=GetDescript(2);//Coverage level code. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}*/
			if(Segment.Get(3)!="30") {//we don't want to show the generic 30 because of clutter.
				txt=GetDescript(3);//Service type code. Situational
				if(txt!="") {
					retVal+=", "+txt;
				}
			}
			txt=GetDescript(4);//Insurance type code. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(5);//Plan coverage description. Situational
			if(isMessageMode && txt!="") {//Causes clutter.
				if(retVal!="") {
					retVal+=", ";
				}
				retVal+=txt;
			}
			txt=GetDescript(6);//Time period qualifier. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(7);//Monetary amount. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(8,isMessageMode,isCoinsurancePatPays);//Percent. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(9);//Quantity qualifier. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(10);//Quantity. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(11);//Situational.
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(12);//Situational.
			if(txt!="") {
				retVal+=", "+txt;
			}
			txt=GetDescript(13);//Procedure identifier. Situational
			if(txt!="") {
				retVal+=", "+txt;
			}
			for(int i=0;i<SupplementalSegments.Count;i++) {
				//addresses already handled
				//messages are just annoying and cluttery
				if(!isMessageMode) {
					continue;
				}
				if(SupplementalSegments[i].SegmentID=="MSG") {
					retVal+=", "+SupplementalSegments[i].Get(1);
				}
			}
			return retVal;
		}

		private string GetDescriptionForAddress() {
			string retVal=GetDescript(1)+"\r\n";//tells us what kind of address
			for(int i=0;i<SupplementalSegments.Count;i++) {
				if(SupplementalSegments[i].SegmentID=="NM1") {
					retVal+=SupplementalSegments[i].Get(3)+" "+SupplementalSegments[i].Get(4)+"\r\n";//LName and optional FName
				}
				if(SupplementalSegments[i].SegmentID=="N3") {
					retVal+=SupplementalSegments[i].Get(1)+" "+SupplementalSegments[i].Get(2)+"\r\n";//Address 1 and 2
				}
				if(SupplementalSegments[i].SegmentID=="N4") {
					retVal+=SupplementalSegments[i].Get(1)+", "+SupplementalSegments[i].Get(2)+SupplementalSegments[i].Get(3);//City, State Zip
				}
			}
			return retVal;
		}

		private string GetDescriptionForDate() {
			string retVal="";
			for(int i=0;i<SupplementalSegments.Count;i++) {
				if(SupplementalSegments[i].SegmentID=="DTP") {
					retVal+=DTP271.GetQualifierDescript(SupplementalSegments[i].Get(1))+": "
						+DTP271.GetDateStr(SupplementalSegments[i].Get(2),SupplementalSegments[i].Get(3));
				}
			}
			return retVal;
		}

		///<summary>Informational only</summary>
		private string GetDescriptionForCodeCovered() {
			string descript=GetDescript(13);
			if(descript=="") {
				return "";
			}
			return "Covered: "+descript;
		}

		///<summary></summary>
		private string GetDescriptionForPercentCode() {
			string descript=GetDescript(8);//patient pays 0%
			string txt=GetDescript(12);//in or out of network
			if(txt!="") {
				descript+=", "+txt;
			}
			descript+=", "+GetDescript(13);//proc
			return descript;
		}

		///<summary>The most human-readable description possible for a single element.</summary>
		public string GetDescript(int elementPos) {
			return GetDescript(elementPos,false);
		}

		///<summary>The most human-readable description possible for a single element.</summary>
		public string GetDescript(int elementPos,bool isMessageMode,bool isCoinsurancePatPays=true) {
			string elementCode=Segment.Get(elementPos);
			if(elementCode=="") {
				return "";
			}
			switch(elementPos) {
				case 1:
					//This is a required element, but we still won't assume it's found
					EB01 eb01val=eb01.Find(EB01MatchesCode);
					if(eb01val==null) {
						return "";
					}
					if(eb01val.Code=="D" && isMessageMode) {//D is for benefit description, which is already obvious
						return "";
					}
					return eb01val.Descript;
				case 2:
					EB02 eb02val=eb02.Find(EB02MatchesCode);
					if(eb02val==null) {
						return "";
					}
					return eb02val.Descript;
				case 3:
					EB03 eb03val=eb03.Find(EB03MatchesCode);
					if(eb03val==null) {
						return "";
					}
					return eb03val.Descript;
				case 4:
					if(!EB04.ContainsKey(elementCode)){
						return "";
					}
					return EB04[elementCode];
				case 5:
					return Segment.Get(5);
				case 6:
					EB06 eb06val=eb06.Find(EB06MatchesCode);
					if(eb06val==null) {
						return "";
					}
					return eb06val.Descript;
				case 7:
					return PIn.Double(elementCode).ToString("c");//Monetary amount. Situational
				case 8:
					if(isMessageMode) {//delta sends 80% instead of 20% like they should
						return (PIn.Double(elementCode)*100).ToString()+"%";//Percent.
					}
					else {
						string leadingStr="Patient pays ";
						if(!isCoinsurancePatPays) { 
							leadingStr="Insurance pays ";
						}
						return leadingStr+(PIn.Double(elementCode)*100).ToString()+"%";//Percent. Situational						
					}
				case 9://Quantity qualifier. Situational
					EB09 eb09val=eb09.Find(EB09MatchesCode);
					if(eb09val==null) {
						return "";
					}
					return eb09val.Descript;
				case 10:
					return elementCode;//Quantity. Situational
				case 11:
					return "Authorization Required-"+elementCode;//Situational.
				case 12://Situational.
					if(elementCode=="Y") {
						return "In network";
					}
					else if(elementCode=="N") {
						return "Out of network";
					}
					else {//elementCode=="U"
						return "Unknown if in network";
					}
				case 13:
					string procStr=Segment.Get(13,2);
					if(procStr=="") {
						return "";
					}
					ProcedureCode procCode=ProcedureCodes.GetProcCode(procStr);
					return procStr+" - "+procCode.AbbrDesc;//ProcedureCodes.GetLaymanTerm(procCode.CodeNum);
					//Even though we don't make requests about specific procedure codes, some ins co's will send back codes.
				default:
					return "";
			}
		}

		/// <summary>Search predicate returns true if code matches.</summary>
		private bool EB01MatchesCode(EB01 eb01val) {
			if(Segment.Get(1)==eb01val.Code) {
				return true;
			}
			return false;
		}

		/// <summary>Search predicate returns true if code matches.</summary>
		private bool EB02MatchesCode(EB02 eb02val) {
			if(Segment.Get(2)==eb02val.Code) {
				return true;
			}
			return false;
		}

		/// <summary>Search predicate returns true if code matches.</summary>
		private bool EB03MatchesCode(EB03 eb03val) {
			if(Segment.Get(3)==eb03val.Code) {
				return true;
			}
			return false;
		}

		/// <summary>Search predicate returns true if code matches.</summary>
		private bool EB06MatchesCode(EB06 eb06val) {
			if(Segment.Get(6)==eb06val.Code) {
				return true;
			}
			return false;
		}

		/// <summary>Search predicate returns true if code matches.</summary>
		private bool EB09MatchesCode(EB09 eb09val) {
			if(Segment.Get(9)==eb09val.Code) {
				return true;
			}
			return false;
		}

		///<summary>Attempts to return the DTP*304 dateTime associated to this limitation EB segment, otherwise returns DateTime.MinValue.</summary>
		///<param name="prefName">The out param that is set to a matched PrefName based on this EB segments first MSG01 value.
		///Only applicable when return value is not DateTime.MinValue.</param>
		///<returns>Returns valid DateTime when there is an DTP*304 segment and MSG segment that can be matched to internal PrefName,
		///otherwise DateTime.MinValue.</returns>
		public DateTime GetInsHistDate(out PrefName prefName) {
			prefName=PrefName.NotApplicable;
			if(Segment.Get(1)!="F") {//Not a limitation.
				return DateTime.MinValue;
			}
			//DTP*(Qualifier)*(Format)*(DateTime) page 240
			//'DTP*304'	: 2110C 'Subscriber Eligibility/Benefit Date'*'Latest Visit or Consultation' page 240-241
			//'DTP*304'	: 2110D 'Dependent Eligibility/Benefit Date'*'Latest Visit or Consultation' page 316
			X12Segment segmentDtp=SupplementalSegments.FirstOrDefault(x => x.SegmentID=="DTP" && x.Elements[1]=="304");
			X12Segment segmentMsg=SupplementalSegments.FirstOrDefault(x => x.SegmentID=="MSG");
			DateTime result;
			if(segmentDtp==null
				|| segmentMsg==null
				|| !TryGetMatchedMsg(segmentMsg.Get(1),out prefName)//DTP and MSG found but MSG not associated to internal PrefName
				|| !DateTime.TryParseExact(segmentDtp.Get(3),"yyyymmdd",CultureInfo.InvariantCulture,DateTimeStyles.None,out result)//Should not fail
				|| result.Year<1880//Invalid date
				|| result>DateTime.Today)//Invalid date
			{
				return DateTime.MinValue;
			}
			return result;
		}

		///<summary>Gives us a raw string of the original EB segment as well as all supplemental segments.  It's somewhat reconstructed rather than strictly the original.
		///If hasFreeFormText==false then the free form text element is cleared out (not removed) and the segment is reconstructed.
		///This is useful for finding duplicate EB271 objects.</summary>
		public string ToString(bool hasFreeFormText = true) {
			string retVal=Segment.ToString()+"~";
			if(!hasFreeFormText) {//Surgically clear the free form text element and reconstruct the segment string.
				string[] arraySegmentElements=retVal.Split('*');
				if(arraySegmentElements.Length>=6) {//Only clear it out if the element exists.
					arraySegmentElements[5]="";
				}
				retVal=String.Join("*",arraySegmentElements);
			}
			//Add any supplemental segments to retVal
			for(int i = 0;i<SupplementalSegments.Count;i++) {
				retVal+="\r\n"+SupplementalSegments[i].ToString()+"~";
			}
			return retVal;
		}

		private static void FillDictionaries(){
			eb01=new List<EB01>();
			eb01.Add(new EB01("1","Active Coverage",InsBenefitType.ActiveCoverage));
			eb01.Add(new EB01("2","Active - Full Risk Capitation",InsBenefitType.ActiveCoverage));
			eb01.Add(new EB01("3","Active - Services Capitated",InsBenefitType.ActiveCoverage));
			eb01.Add(new EB01("4","Active - Services Capitated to Primary Care Physician"));
			eb01.Add(new EB01("5","Active - Pending Investigation"));
			eb01.Add(new EB01("6","Inactive"));
			eb01.Add(new EB01("7","Inactive - Pending Eligibility Update"));
			eb01.Add(new EB01("8","Inactive - Pending Investigation"));
			eb01.Add(new EB01("A","Co-Insurance",InsBenefitType.CoInsurance));
			eb01.Add(new EB01("B","Co-Payment",InsBenefitType.CoPayment));
			eb01.Add(new EB01("C","Deductible",InsBenefitType.Deductible));
			eb01.Add(new EB01("CB","Coverage Basis"));
			eb01.Add(new EB01("D","Benefit Description"));
			eb01.Add(new EB01("E","Exclusions",InsBenefitType.Exclusions));
			eb01.Add(new EB01("F","Limitations",InsBenefitType.Limitations));
			eb01.Add(new EB01("G","Out of Pocket (Stop Loss)"));
			eb01.Add(new EB01("H","Unlimited"));
			eb01.Add(new EB01("I","Non-Covered",InsBenefitType.Exclusions));
			eb01.Add(new EB01("J","Cost Containment"));
			eb01.Add(new EB01("K","Reserve"));
			eb01.Add(new EB01("L","Primary Care Provider"));
			eb01.Add(new EB01("M","Pre-existing Condition"));
			eb01.Add(new EB01("MC","Managed Care Coordinator"));
			eb01.Add(new EB01("N","Services Restricted to Following Provider"));
			eb01.Add(new EB01("O","Not Deemed a Medical Necessity"));
			eb01.Add(new EB01("P","Benefit Disclaimer"));
			eb01.Add(new EB01("Q","Second Surgical Opinion Required"));
			eb01.Add(new EB01("R","Other or Additional Payor"));
			eb01.Add(new EB01("S","Prior Year(s) History"));
			eb01.Add(new EB01("T","Card(s) Reported Lost/Stolen"));
			eb01.Add(new EB01("U","Contact Following Entity for Information"));//Too long: ...for Eligibility or Benefit Information"));
			eb01.Add(new EB01("V","Cannot Process"));
			eb01.Add(new EB01("W","Other Source of Data"));
			eb01.Add(new EB01("X","Health Care Facility"));
			eb01.Add(new EB01("Y","Spend Down"));
			//------------------------------------------------------------------------------------------------------
			eb02=new List<EB02>();
			eb02.Add(new EB02("CHD","Children Only"));
			eb02.Add(new EB02("DEP","Dependents Only"));
			eb02.Add(new EB02("ECH","Employee and Children",BenefitCoverageLevel.Family));
			eb02.Add(new EB02("EMP","Employee Only"));
			eb02.Add(new EB02("ESP","Employee and Spouse",BenefitCoverageLevel.Family));
			eb02.Add(new EB02("FAM","Family",BenefitCoverageLevel.Family));
			eb02.Add(new EB02("IND","Individual",BenefitCoverageLevel.Individual));
			eb02.Add(new EB02("SPC","Spouse and Children"));
			eb02.Add(new EB02("SPO","Spouse Only"));
			//------------------------------------------------------------------------------------------------------
			eb03=new List<EB03>();
			eb03.Add(new EB03("1","Medical Care"));
			eb03.Add(new EB03("2","Surgical"));
			eb03.Add(new EB03("3","Consultation"));
			eb03.Add(new EB03("4","Diagnostic X-Ray",EbenefitCategory.DiagnosticXRay));
			eb03.Add(new EB03("5","Diagnostic Lab"));
			eb03.Add(new EB03("6","Radiation Therapy"));
			eb03.Add(new EB03("7","Anesthesia"));
			eb03.Add(new EB03("8","Surgical Assistance"));
			eb03.Add(new EB03("9","Other Medical"));
			eb03.Add(new EB03("10","Blood Charges"));
			eb03.Add(new EB03("11","Used Durable Medical Equipment"));
			eb03.Add(new EB03("12","Durable Medical Equipment Purchase"));
			eb03.Add(new EB03("13","Ambulatory Service Center Facility"));
			eb03.Add(new EB03("14","Renal Supplies in the Home"));
			eb03.Add(new EB03("15","Alternate Method Dialysis"));
			eb03.Add(new EB03("16","Chronic Renal Disease (CRD) Equipment"));
			eb03.Add(new EB03("17","Pre-Admission Testing"));
			eb03.Add(new EB03("18","Durable Medical Equipment Rental"));
			eb03.Add(new EB03("19","Pneumonia Vaccine"));
			eb03.Add(new EB03("20","Second Surgical Opinion"));
			eb03.Add(new EB03("21","Third Surgical Opinion"));
			eb03.Add(new EB03("22","Social Work"));
			eb03.Add(new EB03("23","Diagnostic Dental",EbenefitCategory.Diagnostic));
			eb03.Add(new EB03("24","Periodontics",EbenefitCategory.Periodontics));
			eb03.Add(new EB03("25","Restorative",EbenefitCategory.Restorative));
			eb03.Add(new EB03("26","Endodontics",EbenefitCategory.Endodontics));
			eb03.Add(new EB03("27","Maxillofacial Prosthetics",EbenefitCategory.MaxillofacialProsth));
			eb03.Add(new EB03("28","Adjunctive Dental Services",EbenefitCategory.Adjunctive));
			eb03.Add(new EB03("30","Health Benefit Plan Coverage",EbenefitCategory.General));
			eb03.Add(new EB03("32","Plan Waiting Period"));
			eb03.Add(new EB03("33","Chiropractic"));
			eb03.Add(new EB03("34","Chiropractic Office Visits"));
			eb03.Add(new EB03("35","Dental Care",EbenefitCategory.General));
			eb03.Add(new EB03("36","Dental Crowns",EbenefitCategory.Crowns));
			eb03.Add(new EB03("37","Dental Accident",EbenefitCategory.Accident));
			eb03.Add(new EB03("38","Orthodontics",EbenefitCategory.Orthodontics));
			eb03.Add(new EB03("39","Prosthodontics",EbenefitCategory.Prosthodontics));
			eb03.Add(new EB03("40","Oral Surgery",EbenefitCategory.OralSurgery));
			eb03.Add(new EB03("41","Routine (Preventive) Dental",EbenefitCategory.RoutinePreventive));
			eb03.Add(new EB03("42","Home Health Care"));
			eb03.Add(new EB03("43","Home Health Prescriptions"));
			eb03.Add(new EB03("44","Home Health Visits"));
			eb03.Add(new EB03("45","Hospice"));
			eb03.Add(new EB03("46","Respite Care"));
			eb03.Add(new EB03("47","Hospital"));
			eb03.Add(new EB03("48","Hospital - Inpatient"));
			eb03.Add(new EB03("49","Hospital - Room and Board"));
			eb03.Add(new EB03("50","Hospital - Outpatient"));
			eb03.Add(new EB03("51","Hospital - Emergency Accident"));
			eb03.Add(new EB03("52","Hospital - Emergency Medical"));
			eb03.Add(new EB03("53","Hospital - Ambulatory Surgical"));
			eb03.Add(new EB03("54","Long Term Care"));
			eb03.Add(new EB03("55","Major Medical"));
			eb03.Add(new EB03("56","Medically Related Transportation"));
			eb03.Add(new EB03("57","Air Transportation"));
			eb03.Add(new EB03("58","Cabulance"));
			eb03.Add(new EB03("59","Licensed Ambulance"));
			eb03.Add(new EB03("60","General Benefits"));
			eb03.Add(new EB03("61","In-vitro Fertilization"));
			eb03.Add(new EB03("62","MRI/CAT Scan"));
			eb03.Add(new EB03("63","Donor Procedures"));
			eb03.Add(new EB03("64","Acupuncture"));
			eb03.Add(new EB03("65","Newborn Care"));
			eb03.Add(new EB03("66","Pathology"));
			eb03.Add(new EB03("67","Smoking Cessation"));
			eb03.Add(new EB03("68","Well Baby Care"));
			eb03.Add(new EB03("69","Maternity"));
			eb03.Add(new EB03("70","Transplants"));
			eb03.Add(new EB03("71","Audiology Exam"));
			eb03.Add(new EB03("72","Inhalation Therapy"));
			eb03.Add(new EB03("73","Diagnostic Medical"));
			eb03.Add(new EB03("74","Private Duty Nursing"));
			eb03.Add(new EB03("75","Prosthetic Device"));
			eb03.Add(new EB03("76","Dialysis"));
			eb03.Add(new EB03("77","Otological Exam"));
			eb03.Add(new EB03("78","Chemotherapy"));
			eb03.Add(new EB03("79","Allergy Testing"));
			eb03.Add(new EB03("80","Immunizations"));
			eb03.Add(new EB03("81","Routine Physical"));
			eb03.Add(new EB03("82","Family Planning"));
			eb03.Add(new EB03("83","Infertility"));
			eb03.Add(new EB03("84","Abortion"));
			eb03.Add(new EB03("85","AIDS"));
			eb03.Add(new EB03("86","Emergency Services"));
			eb03.Add(new EB03("87","Cancer"));
			eb03.Add(new EB03("88","Pharmacy"));
			eb03.Add(new EB03("89","Free Standing Prescription Drug"));
			eb03.Add(new EB03("90","Mail Order Prescription Drug"));
			eb03.Add(new EB03("91","Brand Name Prescription Drug"));
			eb03.Add(new EB03("92","Generic Prescription Drug"));
			eb03.Add(new EB03("93","Podiatry"));
			eb03.Add(new EB03("94","Podiatry - Office Visits"));
			eb03.Add(new EB03("95","Podiatry - Nursing Home Visits"));
			eb03.Add(new EB03("96","Professional (Physician)"));
			eb03.Add(new EB03("97","Anesthesiologist"));
			eb03.Add(new EB03("98","Professional (Physician) Visit - Office"));
			eb03.Add(new EB03("99","Professional (Physician) Visit - Inpatient"));
			eb03.Add(new EB03("A0","Professional (Physician) Visit - Outpatient"));
			eb03.Add(new EB03("A1","Professional (Physician) Visit - Nursing Home"));
			eb03.Add(new EB03("A2","Professional (Physician) Visit - Skilled Nursing Facility"));
			eb03.Add(new EB03("A3","Professional (Physician) Visit - Home"));
			eb03.Add(new EB03("A4","Psychiatric"));
			eb03.Add(new EB03("A5","Psychiatric - Room and Board"));
			eb03.Add(new EB03("A6","Psychotherapy"));
			eb03.Add(new EB03("A7","Psychiatric - Inpatient"));
			eb03.Add(new EB03("A8","Psychiatric - Outpatient"));
			eb03.Add(new EB03("A9","Rehabilitation"));
			eb03.Add(new EB03("AA","Rehabilitation - Room and Board"));
			eb03.Add(new EB03("AB","Rehabilitation - Inpatient"));
			eb03.Add(new EB03("AC","Rehabilitation - Outpatient"));
			eb03.Add(new EB03("AD","Occupational Therapy"));
			eb03.Add(new EB03("AE","Physical Medicine"));
			eb03.Add(new EB03("AF","Speech Therapy"));
			eb03.Add(new EB03("AG","Skilled Nursing Care"));
			eb03.Add(new EB03("AH","Skilled Nursing Care - Room and Board"));
			eb03.Add(new EB03("AI","Substance Abuse"));
			eb03.Add(new EB03("AJ","Alcoholism"));
			eb03.Add(new EB03("AK","Drug Addiction"));
			eb03.Add(new EB03("AL","Vision (Optometry)"));
			eb03.Add(new EB03("AM","Frames"));
			eb03.Add(new EB03("AN","Routine Exam"));
			eb03.Add(new EB03("AO","Lenses"));
			eb03.Add(new EB03("AQ","Nonmedically Necessary Physical"));
			eb03.Add(new EB03("AR","Experimental Drug Therapy"));
			eb03.Add(new EB03("BA","Independent Medical Evaluation"));
			eb03.Add(new EB03("BB","Partial Hospitalization (Psychiatric)"));
			eb03.Add(new EB03("BC","Day Care (Psychiatric)"));
			eb03.Add(new EB03("BD","Cognitive Therapy"));
			eb03.Add(new EB03("BE","Massage Therapy"));
			eb03.Add(new EB03("BF","Pulmonary Rehabilitation"));
			eb03.Add(new EB03("BG","Cardiac Rehabilitation"));
			eb03.Add(new EB03("BH","Pediatric"));
			eb03.Add(new EB03("BI","Nursery"));
			eb03.Add(new EB03("BJ","Skin"));
			eb03.Add(new EB03("BK","Orthopedic"));
			eb03.Add(new EB03("BL","Cardiac"));
			eb03.Add(new EB03("BM","Lymphatic"));
			eb03.Add(new EB03("BN","Gastrointestinal"));
			eb03.Add(new EB03("BP","Endocrine"));
			eb03.Add(new EB03("BQ","Neurology"));
			eb03.Add(new EB03("BR","Eye"));
			eb03.Add(new EB03("BS","Invasive Procedures"));
			//------------------------------------------------------------------------------------------------------
			EB04=new Dictionary<string,string>();
			EB04.Add("12","Medicare Secondary Working Aged Beneficiary or Spouse with Employer Group Health Plan");
			EB04.Add("13","Medicare Secondary End-Stage Renal Disease Beneficiary in the 12 month coordination period with an employer’s group health plan");
			EB04.Add("14","Medicare Secondary, No-fault Insurance including Auto is Primary");
			EB04.Add("15","Medicare Secondary Worker’s Compensation");
			EB04.Add("16","Medicare Secondary Public Health Service (PHS)or Other Federal Agency");
			EB04.Add("41","Medicare Secondary Black Lung");
			EB04.Add("42","Medicare Secondary Veteran’s Administration");
			EB04.Add("43","Medicare Secondary Disabled Beneficiary Under Age 65 with Large Group Health Plan (LGHP)");
			EB04.Add("47","Medicare Secondary, Other Liability Insurance is Primary");
			EB04.Add("AP","Auto Insurance Policy");
			EB04.Add("C1","Commercial");
			EB04.Add("CO","Consolidated Omnibus Budget Reconciliation Act (COBRA)");
			EB04.Add("CP","Medicare Conditionally Primary");
			EB04.Add("D","Disability");
			EB04.Add("DB","Disability Benefits");
			EB04.Add("EP","Exclusive Provider Organization");
			EB04.Add("FF","Family or Friends");
			EB04.Add("GP","Group Policy");
			EB04.Add("HM","Health Maintenance Organization (HMO)");
			EB04.Add("HN","Health Maintenance Organization (HMO) - Medicare Risk");
			EB04.Add("HS","Special Low Income Medicare Beneficiary");
			EB04.Add("IN","Indemnity");
			EB04.Add("IP","Individual Policy");
			EB04.Add("LC","Long Term Care");
			EB04.Add("LD","Long Term Policy");
			EB04.Add("LI","Life Insurance");
			EB04.Add("LT","Litigation");
			EB04.Add("MA","Medicare Part A");
			EB04.Add("MB","Medicare Part B");
			EB04.Add("MC","Medicaid");
			EB04.Add("MH","Medigap Part A");
			EB04.Add("MI","Medigap Part B");
			EB04.Add("MP","Medicare Primary");
			EB04.Add("OT","Other");
			EB04.Add("PE","Property Insurance - Personal");
			EB04.Add("PL","Personal");
			EB04.Add("PP","Personal Payment (Cash - No Insurance)");
			EB04.Add("PR","Preferred Provider Organization (PPO)");
			EB04.Add("PS","Point of Service (POS)");
			EB04.Add("QM","Qualified Medicare Beneficiary");
			EB04.Add("RP","Property Insurance - Real");
			EB04.Add("SP","Supplemental Policy");
			EB04.Add("TF","Tax Equity Fiscal Responsibility Act (TEFRA)");
			EB04.Add("WC","Workers Compensation");
			EB04.Add("WU","Wrap Up Policy");
			//------------------------------------------------------------------------------------------------------
			eb06=new List<EB06>();
			eb06.Add(new EB06("6","Hour"));
			eb06.Add(new EB06("7","Day"));
			eb06.Add(new EB06("13","24 Hours"));
			eb06.Add(new EB06("21","Years",BenefitTimePeriod.Years));
			eb06.Add(new EB06("22","Service Year",BenefitTimePeriod.ServiceYear));
			eb06.Add(new EB06("23","Calendar Year",BenefitTimePeriod.CalendarYear));
			eb06.Add(new EB06("24","Year to Date"));
			eb06.Add(new EB06("25","Contract"));
			eb06.Add(new EB06("26","Episode"));
			eb06.Add(new EB06("27","Visit"));
			eb06.Add(new EB06("28","Outlier"));
			eb06.Add(new EB06("29","Remaining"));
			eb06.Add(new EB06("30","Exceeded"));
			eb06.Add(new EB06("31","Not Exceeded"));
			eb06.Add(new EB06("32","Lifetime",BenefitTimePeriod.Lifetime));
			eb06.Add(new EB06("33","Lifetime Remaining"));
			eb06.Add(new EB06("34","Month"));
			eb06.Add(new EB06("35","Week"));
			eb06.Add(new EB06("36","Admisson"));
			//------------------------------------------------------------------------------------------------------
			eb09=new List<EB09>();
			eb09.Add(new EB09("99","Quantity Used"));
			eb09.Add(new EB09("CA","Covered - Actual"));
			eb09.Add(new EB09("CE","Covered - Estimated"));
			eb09.Add(new EB09("DB","Deductible Blood Units"));
			eb09.Add(new EB09("DY","Days"));
			eb09.Add(new EB09("HS","Hours"));
			eb09.Add(new EB09("LA","Life-time Reserve - Actual"));
			eb09.Add(new EB09("LE","Life-time Reserve - Estimated"));
			eb09.Add(new EB09("MN","Month",BenefitQuantity.Months));
			eb09.Add(new EB09("P6","Number of Services or Procedures",BenefitQuantity.NumberOfServices));
			eb09.Add(new EB09("QA","Quantity Approved"));
			eb09.Add(new EB09("S7","Age, High Value",BenefitQuantity.AgeLimit));
			eb09.Add(new EB09("S8","Age, Low Value"));
			eb09.Add(new EB09("VS","Visits",BenefitQuantity.Visits));
			eb09.Add(new EB09("YY","Years",BenefitQuantity.Years));
		}

		///<summary>Returns the description for the given EB01 code.
		///If given code is not valid then null is returned.</summary>
		public string GetEB01Description(string code){
			EB01 eb=eb01.FirstOrDefault(x => x.Code==code);
			if(eb==null){
				return null;
			}
			return eb.Descript;
		}
		
		///<summary>Attempts to return a matched PrefName based on given value from 271 MSG segment.</summary>
		///<param name="msgVal">The MSG01 value.</param>
		///<returns>Returns true if given value could be matched and sets out pref to matched PrefName,
		///otherwise returns false and out pref is PrefName.Notapplicable.</returns>
		private static bool TryGetMatchedMsg(string msgVal,out PrefName pref){
			msgVal=msgVal.ToUpper().Replace("BENEFITCLASS=","");
			switch(msgVal) {
				case "BITEWING X-RAYS":
					pref=PrefName.InsHistBWCodes;
					break;
				case "EXAMS":
					pref=PrefName.InsHistExamCodes;
					break;
				case "FULL MOUTH/PANOREX":
					pref=PrefName.InsHistPanoCodes;
					break;
				case "PROPHYLAXIS":
					pref=PrefName.InsHistProphyCodes;
					break;
				//Observed in valid 271 response, but we have nowhere to put this information for now
				case "BASIC":
				case "TMJ":
				case "AMALGAM & COMPOSITE":
				case "BUILDUPS/POST AND CORE":
				case "COMPOSITES":
				case "CONSULTATION":
				case "DENTURE ADJUSTMENTS":
				case "DENTURE REBASE":
				case "DENTURE RELINE":
				case "DENTURE RELINE/REBASE":
				case "DENTURE REPAIR":
				case "DENTURES":
				case "DIAGNOSTIC X-RAY":
				case "FLUORIDE":
				case "HARMFUL HABITS APPLIANCE":
				case "IMPLANTS":
				case "INLAY/ONLAY":
				case "PERIO SURGERY":
				case "SEALANTS":
				case "TMJ TREATMENT":
				default:
					pref=PrefName.NotApplicable;
					break;
			}
			return (pref!=PrefName.NotApplicable);
		}
		
		///<summary>Inserts or updates proc insurance history based on given 271 EB segments.</summary>
		///<param name="listEb271s">List of 271 EB segments to look for ins history dates</param>
		///<param name="patNum">The patNum for the given EB segments.</param>
		///<param name="insSub">The patients InsSub associated to the plan of the retrieved 271 EB segments.</param>
		///<returns>Count of inserted/updated insurance histories from given list of EB segments.</returns>
		public static int SetInsuranceHistoryDates(List<EB271> listEb271s,long patNum,InsSub insSub) {
			if(listEb271s==null || listEb271s.Count==0){//Avoid queries.
				return 0;
			}
			int countValidInsHist=0;
			Patient patient=Patients.GetLim(patNum);
			List<ClaimProc> listClaimProcsForInsHistProcs=null;
			Dictionary<PrefName,Procedure> dictInsHistProcs=Procedures.GetDictInsHistProcs(patient.PatNum,insSub.InsSubNum,out listClaimProcsForInsHistProcs);
			foreach(EB271 eb in listEb271s) {
				PrefName prefName;
				DateTime insHistDate=eb.GetInsHistDate(out prefName);
				if(insHistDate==DateTime.MinValue) {//When insHistDate is not MinValue, prefName is guarenteed to be valid.
					continue;//No insurance history date found for this eb271.
				}
				Procedure proc=dictInsHistProcs[prefName];
				List<ClaimProc> listClaimProcsForProc=new List<ClaimProc>();
				if(proc!=null) {
					//Get all of the claimprocs for this procedure.
					listClaimProcsForProc=listClaimProcsForInsHistProcs.FindAll(x => x.ProcNum==proc.ProcNum);
				}
				Procedures.InsertOrUpdateInsHistProcedure(patient,prefName,insHistDate,insSub.PlanNum,insSub.InsSubNum,proc,listClaimProcsForProc);
				countValidInsHist++;
			}
			return countValidInsHist;
		}

	}

	public class EB01 {
		private string code;
		private string descript;
		private InsBenefitType benefitType;
		private bool isSupported;

		public EB01(string code,string descript,InsBenefitType benefitType) {
			this.code=code;
			this.descript=descript;
			this.benefitType=benefitType;
			this.isSupported=true;
		}

		public EB01(string code,string descript) {
			this.code=code;
			this.descript=descript;
			this.benefitType=InsBenefitType.ActiveCoverage;//ignored
			this.isSupported=false;
		}

		public InsBenefitType BenefitType {
			get { return benefitType; }
			set { benefitType = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}
		
		public string Descript {
			get { return descript; }
			set { descript = value; }
		}

		public bool IsSupported {
			get { return isSupported; }
			set { isSupported = value; }
		}
	}

	public class EB02 {
		private string code;
		private string descript;
		private BenefitCoverageLevel coverageLevel;
		private bool isSupported;

		public EB02(string code,string descript,BenefitCoverageLevel coverageLevel) {
			this.code=code;
			this.descript=descript;
			this.coverageLevel=coverageLevel;
			this.isSupported=true;
		}

		public EB02(string code,string descript) {
			this.code=code;
			this.descript=descript;
			this.coverageLevel=BenefitCoverageLevel.Individual;//ignored
			this.isSupported=false;
		}

		public BenefitCoverageLevel CoverageLevel {
			get { return coverageLevel; }
			set { coverageLevel = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}

		public string Descript {
			get { return descript; }
			set { descript = value; }
		}

		public bool IsSupported {
			get { return isSupported; }
			set { isSupported = value; }
		}
	}

	public class EB03 {
		private string code;
		private string descript;
		private EbenefitCategory serviceType;
		private bool isSupported;

		public EB03(string code,string descript,EbenefitCategory serviceType) {
			this.code=code;
			this.descript=descript;
			this.serviceType=serviceType;
			this.isSupported=true;
		}

		public EB03(string code,string descript) {
			this.code=code;
			this.descript=descript;
			this.serviceType=EbenefitCategory.None;//ignored
			this.isSupported=false;
		}

		public EbenefitCategory ServiceType {
			get { return serviceType; }
			set { serviceType = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}

		public string Descript {
			get { return descript; }
			set { descript = value; }
		}

		public bool IsSupported {
			get { return isSupported; }
			set { isSupported = value; }
		}
	}

	///<summary>Time period qualifier</summary>
	public class EB06 {
		private string code;
		private string descript;
		private BenefitTimePeriod timePeriod;
		private bool isSupported;

		public EB06(string code,string descript,BenefitTimePeriod timePeriod) {
			this.code=code;
			this.descript=descript;
			this.timePeriod=timePeriod;
			this.isSupported=true;
		}

		public EB06(string code,string descript) {
			this.code=code;
			this.descript=descript;
			this.timePeriod=BenefitTimePeriod.None;//ignored
			this.isSupported=false;
		}

		public BenefitTimePeriod TimePeriod {
			get { return timePeriod; }
			set { timePeriod = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}

		public string Descript {
			get { return descript; }
			set { descript = value; }
		}

		public bool IsSupported {
			get { return isSupported; }
			set { isSupported = value; }
		}
	}

	///<summary>Quantity qualifier</summary>
	public class EB09 {
		private string code;
		private string descript;
		private BenefitQuantity quantityQualifier;
		private bool isSupported;

		public EB09(string code,string descript,BenefitQuantity quantityQualifier) {
			this.code=code;
			this.descript=descript;
			this.quantityQualifier=quantityQualifier;
			this.isSupported=true;
		}

		public EB09(string code,string descript) {
			this.code=code;
			this.descript=descript;
			this.quantityQualifier=BenefitQuantity.None;//ignored
			this.isSupported=false;
		}

		public BenefitQuantity QuantityQualifier {
			get { return quantityQualifier; }
			set { quantityQualifier = value; }
		}

		public string Code {
			get { return code; }
			set { code = value; }
		}

		public string Descript {
			get { return descript; }
			set { descript = value; }
		}

		public bool IsSupported {
			get { return isSupported; }
			set { isSupported = value; }
		}
	}

}
