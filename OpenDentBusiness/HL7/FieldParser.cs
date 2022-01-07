using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Diagnostics;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>Parses a single incoming HL7 field.</summary>
	public class FieldParser {
		//HL7 has very specific data types.  Each data type that we use will have a corresponding parser method here.
		//Data types are listed in 2.15.

		///<summary>yyyyMMddHHmmss.  Can have more precision than seconds and won't break.  If less than 8 digits, returns MinVal.</summary>
		public static DateTime DateTimeParse(string str) {
			int year=0;
			int month=0;
			int day=0;
			int hour=0;
			int minute=0;
			if(str.Length<8) {
				return DateTime.MinValue;
			}
			try {
				year=PIn.Int(str.Substring(0,4));
				month=PIn.Int(str.Substring(4,2));
				day=PIn.Int(str.Substring(6,2));
			}
			catch(Exception ex) {//PIn.Int could fail if not able to parse into an Int32
				ex.DoNothing();
				return DateTime.MinValue;
			}
			if(str.Length>=10) {
				try {
					hour=PIn.Int(str.Substring(8,2));
				}
				catch(Exception ex) {
					ex.DoNothing();
					//do nothing, hour will remain 0
				}
			}
			if(str.Length>=12) {
				try {
					minute=PIn.Int(str.Substring(10,2));
				}
				catch(Exception ex) {
					ex.DoNothing();
					//do nothing, minute will remain 0
				}
			}
			//skip seconds and any trailing numbers
			DateTime retVal=new DateTime(year,month,day,hour,minute,0);
			return retVal;
		}

		///<summary>M,F,U</summary>
		public static PatientGender GenderParse(string str) {
			if(str.ToLower()=="m" || str.ToLower()=="male") {
				return PatientGender.Male;
			}
			else if(str.ToLower()=="f" || str.ToLower()=="female") {
				return PatientGender.Female;
			}
			else {
				return PatientGender.Unknown;
			}
		}

		public static PatientPosition MaritalStatusParse(string str) {
			switch(str.ToLower()) {
				case "b"://Unmarried
				case "g"://Living together
				case "n"://Annulled
				case "o"://Other
				case "p"://Domestic partner
				case "partner":
				case "r"://Registered domestic partner
				case "s"://Single
				case "single":
				case "t"://Unreported
				case "u"://Unknown
				case "unknown":
					return PatientPosition.Single;
				case "a"://Separated
				case "c"://Common law
				case "e"://Legally Separated
				case "i"://Interlocutory (divorce is not yet final)
				case "legally separated":
				case "m"://Married
				case "married":
					return PatientPosition.Married;
				case "d"://Divorced
				case "divorced":
					return PatientPosition.Divorced;
				case "w"://Widowed
				case "widowed":
					return PatientPosition.Widowed;
				default:
					return PatientPosition.Single;
			}
		}

		/// <summary>If it's exactly 10 digits, it will be formatted like this: (###)###-####.  Otherwise, no change.</summary>
		public static string PhoneParse(string str) {
			if(str.Length != 10) {
				return str;//no change
			}
			return "("+str.Substring(0,3)+")"+str.Substring(3,3)+"-"+str.Substring(6);
		}

		public static string ProcessPattern(DateTime startTime,DateTime stopTime) {
			int minutes=(int)((stopTime-startTime).TotalMinutes);
			if(minutes<=0) {
				return "//";//we don't want it to be zero minutes
			}
			int increments5=minutes/5;
			StringBuilder pattern=new StringBuilder();
			for(int i=0;i<increments5;i++) {
				pattern.Append("X");//make it all provider time, I guess.
			}
			return pattern.ToString();
		}
		
		///<summary>Used by eCW.  This will locate a provider by EcwID and update the FName, LName, and MI if necessary.  If no provider is found by EcwID, than a new provider is inserted and the FName, LName, and MI are set.  Supply in format UPIN^LastName^FirstName^MI (PV1 or AIP) or UPIN^LastName, FirstName MI (AIG).  If UPIN(abbr) does not exist, provider gets created.  If name has changed, provider gets updated.  ProvNum is returned.  If blank, then returns 0.  If field is NULL, returns 0. For PV1, the provider.LName field will hold "LastName, FirstName MI". They can manually change later.</summary>
		public static long ProvProcessEcw(FieldHL7 field) {
			if(field==null) {
				return 0;
			}
			string eID=field.GetComponentVal(0);
			eID=eID.Trim();
			if(eID=="") {
				return 0;
			}
			Provider prov=Providers.GetProvByEcwID(eID);
			bool isNewProv=false;
			bool provChanged=false;
			if(prov==null) {
				isNewProv=true;
				prov=new Provider();
				prov.Abbr=eID;//They can manually change this later.
				prov.EcwID=eID;
				prov.FeeSched=FeeScheds.GetFirst(true).FeeSchedNum;
			}
			if(field.Components.Count==4) {//PV1 segment in format UPIN^LastName^FirstName^MI
				if(prov.LName!=field.GetComponentVal(1)) {
					provChanged=true;
					prov.LName=field.GetComponentVal(1);
				}
				if(prov.FName!=field.GetComponentVal(2)) {
					provChanged=true;
					prov.FName=field.GetComponentVal(2);
				}
				if(prov.MI!=field.GetComponentVal(3)) {
					provChanged=true;
					prov.MI=field.GetComponentVal(3);
				}
			}
			else if(field.Components.Count==2) {//AIG segment in format UPIN^LastName, FirstName MI
				string[] components=field.GetComponentVal(1).Split(' ');
				if(components.Length>0) {
					components[0]=components[0].TrimEnd(',');
					if(prov.LName!=components[0]) {
						provChanged=true;
						prov.LName=components[0];
					}
				}
				if(components.Length>1 && prov.FName!=components[1]) {
					provChanged=true;
					prov.FName=components[1];
				}
				if(components.Length>2 && prov.MI!=components[2]) {
					provChanged=true;
					prov.MI=components[2];
				}
			}
			if(isNewProv) {
				Providers.Insert(prov);
				Providers.RefreshCache();
			}
			else if(provChanged) {
				Providers.Update(prov);
				Providers.RefreshCache();
			}
			return prov.ProvNum;
		}

		///<summary>This field could be a CWE data type or a XCN data type, depending on if it came from an AIG segment, an AIP segment, or a PV1 segment.  The AIG segment would have this as a CWE data type in the format ProvID^LName, FName^^Abbr.  For the AIP and PV1 segments, the data type is XCN and the format would be ProvID^LName^FName^^^Abbr.  The ProvID is used first.  This will contain the root OID and ProvNum extension.  If it has the OD root OID for a provider, the number is assumed to be the OD ProvNum and used to find the provider.  If the root OID is not the OD root, it is used for on oidexternal table lookup.  If the provider is not found from the ID in either the provider table or the oidexternals table, then an attempt is made to find the provider by name and abbreviation.  This will return 0 if the field or segName are null or if no provider can be found.  A new provider will not be inserted with the information provided if not found by ProvID or name and abbr.  This field is repeatable, so we will check all repetitions for valid provider ID's or name/abbr combinations.</summary>
		public static long ProvParse(FieldHL7 field,SegmentNameHL7 segName,bool isVerbose,bool isV2_3=false) {
			long provNum=0;
			List<OIDExternal> listOidExt=new List<OIDExternal>();
			if(field==null) {
				return 0;
			}
			#region Attempt to Get Provider From ProvIDs
			//Example of an ID using the hierarchic designation would be 2.16.840.1.113883.3.4337.1486.6566.3.1
			//Where 2.16.840.1.113883.3.4337.1486.6566 is the office oidroot, the .3 is to identify this as a provider
			//2.16.840.1.113883.3.4337.1486.6566.3 would be the office's oidinternal entry for IDType=Provider
			//The .1 is "."+ProvNum, where the ProvNum in this example is 1 and is considered the extension
			//We will strip off the ProvNum and if it is connected to the office's oidinternal entry for a provider, we will use it as the OD ProvNum
			//If it is attached to a different hierarchic root, we will try to find it in the oidexternals table linked to an OD ProvNum
			string [] provIdHierarch=field.GetComponentVal(0).Split(new string[] {"."},StringSplitOptions.RemoveEmptyEntries);
			string strProvId="";
			string strProvIdRoot="";
			if(provIdHierarch.Length>1) {//must have a root and an ID
				strProvId=provIdHierarch[provIdHierarch.Length-1];
				strProvIdRoot=field.GetComponentVal(0).Substring(0,field.GetComponentVal(0).Length-strProvId.Length-1);//-1 for the last "."
			}
			if(strProvId!="" && strProvIdRoot!="") {
				if(strProvIdRoot==OIDInternals.GetForType(IdentifierType.Provider).IDRoot) {//The office's root OID for a provider object, ProvId should be the OD ProvNum
					try {
						if(Providers.GetProv(PIn.Long(strProvId))!=null) {
							provNum=PIn.Long(strProvId);//if component is empty string, provNum will be 0
						}
					}
					catch(Exception ex) {
						ex.DoNothing();
						//PIn.Long failed to convert the component to a long, provNum will remain 0 and we will attempt to get by name and abbr below
					}
				}
				else {//there was a ProvID and a ProvID root, but the root is not the office's root OID for a provider object, check the oidexternals table
					OIDExternal oidExtProv=OIDExternals.GetByRootAndExtension(strProvIdRoot,strProvId);
					if(oidExtProv==null) {//add to the list of oid's to add to the oidexternal table if we find a provider
						OIDExternal oidExtCur=new OIDExternal();
						oidExtCur.IDType=IdentifierType.Provider;
						oidExtCur.rootExternal=strProvIdRoot;
						oidExtCur.IDExternal=strProvId;
						//oidExtCur.IDInteral may not have been found yet
						listOidExt.Add(oidExtCur);
					}
					if(oidExtProv!=null && oidExtProv.IDType==IdentifierType.Provider) {
						//possibly some other validation of name match?
						provNum=oidExtProv.IDInternal;
					}
				}
			}
			for(int i=0;i<field.ListRepeatFields.Count;i++) {//could be repetitions of this field with other IDs
				strProvId="";
				strProvIdRoot="";
				provIdHierarch=field.ListRepeatFields[i].GetComponentVal(0).Split(new string[] { "." },StringSplitOptions.RemoveEmptyEntries);
				if(provIdHierarch.Length<2) {//must be a root and an ID
					continue;
				}
				strProvId=provIdHierarch[provIdHierarch.Length-1];
				strProvIdRoot=field.ListRepeatFields[i].GetComponentVal(0).Substring(0,field.ListRepeatFields[i].GetComponentVal(0).Length-strProvId.Length-1);//-1 for the last "."
				if(strProvId=="" || strProvIdRoot=="") {
					continue;
				}
				if(provNum==0 && strProvIdRoot==OIDInternals.GetForType(IdentifierType.Provider).IDRoot) {//The office's root OID for a provider object, ProvId should be the OD ProvNum
					try {
						if(Providers.GetProv(PIn.Long(strProvId))!=null) {
							provNum=PIn.Long(strProvId);//if component is empty string, provNum will be 0
						}
					}
					catch(Exception ex) {
						ex.DoNothing();
						//PIn.Long failed to convert the component to a long, provNum will remain 0 and we will attempt to get by name and abbr below
					}
				}
				else if(strProvIdRoot!=OIDInternals.GetForType(IdentifierType.Provider).IDRoot) {//there was a ProvID and a ProvID root, but the root is not the office's root OID for a provider object, check the oidexternals table
					OIDExternal oidExtProv=OIDExternals.GetByRootAndExtension(strProvIdRoot,strProvId);
					if(oidExtProv==null) {//add to the list of oid's to add to the oidexternal table if we find a provider
						OIDExternal oidExtCur=new OIDExternal();
						oidExtCur.IDType=IdentifierType.Provider;
						oidExtCur.rootExternal=strProvIdRoot;
						oidExtCur.IDExternal=strProvId;
						//oidExtCur.IDInteral may not have been found yet
						listOidExt.Add(oidExtCur);
					}
					else {
						if(provNum==0 && oidExtProv.IDType==IdentifierType.Provider) {
							//possibly some other validation of name match?
							provNum=oidExtProv.IDInternal;
						}
					}
				}
			}
			if(provNum>0) {
				string verboseMsg="";
				for(int i=0;i<listOidExt.Count;i++) {
					listOidExt[i].IDInternal=provNum;
					OIDExternals.Insert(listOidExt[i]);
					verboseMsg+="\r\nProvNum: "+provNum.ToString()+", External root: "+strProvIdRoot+", External Provider ID: "+strProvId;
				}
				if(isVerbose) {
					EventLog.WriteEntry("OpenDentHL7","Added an external provider ID to the oidexternals table due to an incoming "
						+segName.ToString()+" segment."+verboseMsg+".",EventLogEntryType.Information);
				}
				return provNum;
			}
			#endregion Attempt to Get Provider From ProvIDs
			#region Attempt to Get Provider From Name and Abbr
			//Couldn't find the provider with the ProvNum provided, we will attempt to find by FName, LName, and Abbr
			string provLName="";
			string provFName="";
			string provAbbr="";
			if(segName==SegmentNameHL7.AIG) {//AIG is the data type CWE with format ProvNum^LName, FName^^Abbr (for isV2_3 there will be no Abbr)
				//GetComponentVal will return an empty string if the index is greater than the number of the components for this field minus 1
				string[] components=field.GetComponentVal(1).Split(new char[] {' '},StringSplitOptions.RemoveEmptyEntries);
				if(components.Length>0) {
					provLName=components[0].TrimEnd(',');
				}
				if(components.Length>1) {
					provFName=components[1];
				}
				if(!isV2_3) {
					provAbbr=field.GetComponentVal(3);
				}
			}
			else if(segName==SegmentNameHL7.AIP || segName==SegmentNameHL7.PV1) {//AIP and PV1 are the data type XCN with the format ProvNum^LName^FName^^^Abbr
				provLName=field.GetComponentVal(1);
				provFName=field.GetComponentVal(2);
				if(!isV2_3) {
					provAbbr=field.GetComponentVal(5);
				}
			}
			if(!string.IsNullOrEmpty(provAbbr) || isV2_3) {
				List<Provider> listProvs=Providers.GetProvsByFLName(provLName,provFName);
				//There should be only 1 prov with this Abbr, but we only warn them about the duplication and allow more than 1 with the same Abbr.
				//With the LName, FName, and Abbr we can be more certain we retrieve the correct provider.
				//For V2_3 we will just get the first prov with matching first and last name and won't require the Abbr to be included.
				provNum=listProvs.Find(x => isV2_3 || x.Abbr.ToLower()==provAbbr.ToLower())?.ProvNum??0;
			}
			//provider not found by provID, or first name/abbr combination, try the name/abbr combos in the repetitions (or name only for V2_3)
			for(int i=0;i<field.ListRepeatFields.Count;i++) {//could be repetitions of this field with other IDs
				if(provNum>0) {
					break;
				}
				provLName="";
				provFName="";
				provAbbr="";
				if(segName==SegmentNameHL7.AIG) {
					string[] components=field.ListRepeatFields[i].GetComponentVal(1).Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
					if(components.Length>0) {
						provLName=components[0].TrimEnd(',');
					}
					if(components.Length>1) {
						provFName=components[1];
					}
					if(!isV2_3) {
						provAbbr=field.ListRepeatFields[i].GetComponentVal(3);
					}
				}
				else if(segName==SegmentNameHL7.AIP || segName==SegmentNameHL7.PV1) {//AIP and PV1 are the data type XCN with the format ProvNum^LName^FName^^^Abbr
					provLName=field.ListRepeatFields[i].GetComponentVal(1);
					provFName=field.ListRepeatFields[i].GetComponentVal(2);
					if(!isV2_3) {
						provAbbr=field.ListRepeatFields[i].GetComponentVal(5);
					}
				}
				if(!isV2_3 && string.IsNullOrEmpty(provAbbr)) {//Abbr not required for v2_3
					continue;//there has to be a LName, FName, and Abbr if we are trying to match without a ProvNum.  LName and FName empty string check happens in GetProvsByFLName
				}
				List<Provider> listProvs=Providers.GetProvsByFLName(provLName,provFName);
				provNum=listProvs.Find(x => isV2_3 || x.Abbr.ToLower()==provAbbr.ToLower())?.ProvNum??0;
			}
			if(provNum>0) {
				string verboseMsg="";
				for(int i=0;i<listOidExt.Count;i++) {
					listOidExt[i].IDInternal=provNum;
					OIDExternals.Insert(listOidExt[i]);
					verboseMsg+="\r\nProvNum: "+provNum.ToString()+", External root: "+strProvIdRoot+", External Provider ID: "+strProvId;
				}
				if(isVerbose) {
					EventLog.WriteEntry("OpenDentHL7","Added an external provider ID to the oidexternals table due to an incoming "
						+segName.ToString()+" segment."+verboseMsg+".",EventLogEntryType.Information);
				}
			}
			#endregion Attempt to Get Provider From Name and Abbr
			return provNum;
		}		

		///<summary>Returns the depricated PatientRaceOld enum.  It gets converted to new patient race entries where it's called.  This is the old way of receiving the race, just a string that matches exactly.</summary>
		public static PatientRaceOld RaceParseOld(string str) {
			switch(str) {
				case "American Indian Or Alaska Native":
					return PatientRaceOld.AmericanIndian;
				case "Asian":
					return PatientRaceOld.Asian;
				case "Native Hawaiian or Other Pacific":
					return PatientRaceOld.HawaiiOrPacIsland;
				case "Black or African American":
					return PatientRaceOld.AfricanAmerican;
				case "White":
					return PatientRaceOld.White;
				case "Hispanic":
					return PatientRaceOld.HispanicLatino;
				case "Other Race":
					return PatientRaceOld.Other;
				default:
					return PatientRaceOld.Other;
			}
		}

		public static double SecondsToMinutes(string secs) {
			double retVal;
			try {
				retVal=double.Parse(secs);
			}
			catch {//couldn't parse the value to a double so just return 0
				return 0;
			}
			return retVal/60;
		}

		/// <summary>Will return 0 if string cannot be parsed to a number.  Will return 0 if the fee schedule passed in does not exactly match the description of a regular fee schedule.</summary>
		public static long FeeScheduleParse(string str) {
			if(str=="") {
				return 0;
			}
			FeeSched feeSched=FeeScheds.GetByExactName(str,FeeScheduleType.Normal);
			if(feeSched==null) {
				return 0;
			}
			return feeSched.FeeSchedNum;
		}

		///<summary>A string supplied with new line escape commands (\.br\) will be converted to a string with \r\n in it.  The escapeChar supplied will have been retrieved from the escape characters defined in the message, usually "\".  Example: string supplied - line 1\.br\line2\.br\line3; string returned - line 1\r\nline2\r\nline3.</summary>
		public static string StringNewLineParse(string str,char escapeChar) {
			if(string.IsNullOrWhiteSpace(str) || string.IsNullOrWhiteSpace(escapeChar.ToString())) {
				return str;
			}
			return str.Replace(escapeChar.ToString()+".br"+escapeChar.ToString(),"\r\n");
		}

		public static ProcStat ProcStatusParse(string procStat) {
			ProcStat retval=ProcStat.TP;//default to TP status
			switch(procStat.ToLower()) {
				case "c":
				case "complete":
					retval=ProcStat.C;
					break;
				case "ec":
				case "existing current provider":
					retval=ProcStat.EC;
					break;
				case "eo":
				case "existing other provider":
					retval=ProcStat.EO;
					break;
				case "r":
				case "referred out":
					retval=ProcStat.R;
					break;
				case "cn":
				case "condition":
					retval=ProcStat.Cn;
					break;
				//case "tp"://not necessary, just default to TP if either of these values or if unsupported value
				//case "treatment planned":
				default:
					retval=ProcStat.TP;
					break;
			}
			return retval;
		}
	}
}
