using System;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>Parses a single incoming MedLab HL7 field.</summary>
	public class FieldParserMedLab {

		///<summary>yyyyMMdd[[[HH]mm]ss].  Hours, minutes, and seconds are optional.  Can have more precision than seconds and won't break.
		///If less than 8 digits, returns MinVal.</summary>
		public static DateTime DateTimeParse(string str) {
			if(str.Length<8) {
				return DateTime.MinValue;
			}
			int year=0;
			int month=0;
			int day=0;
			int hour=0;
			int minute=0;
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
			return new DateTime(year,month,day,hour,minute,0);
		}

		///<summary>Converts the value in the MedLab HL7 message field to its corresponding AbnormalFlag enumeration.
		///Supported values are 'L', 'H', 'LL', 'HH', '&lt;', '>', 'A', 'AA', 'S', 'R', 'I', 'NEG', and 'POS'.
		///If parsing the value into an enum fails, defaults to AbnormalFlag.None, but this won't likely ever be displayed to the user.</summary>
		public static AbnormalFlag AbnormalFlagParse(string fieldVal) {
			AbnormalFlag flagVal=AbnormalFlag.None;
			switch(fieldVal) {
				case "<":
					flagVal=AbnormalFlag._lt;
					break;
				case ">":
					flagVal=AbnormalFlag._gt;
					break;
				default:
					try {
						flagVal=(AbnormalFlag)Enum.Parse(typeof(AbnormalFlag),fieldVal,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
						//do nothing, will remain AbnormalFlag.None
					}
					break;
			}
			return flagVal;
		}

		///<summary>Converts the value in the MedLab HL7 message field to its corresponding ResultStatus enumeration.
		///Supported values are 'P', 'X', 'F', 'C', and 'I'.  If parsing the value into an enum fails, defaults to ResultStatus.F - final.</summary>
		public static ResultStatus ResultStatusParse(string fieldVal) {
			ResultStatus resultStatus=ResultStatus.F;
			try {
				resultStatus=(ResultStatus)Enum.Parse(typeof(ResultStatus),fieldVal,true);
			}
			catch(Exception ex) {
				ex.DoNothing();
				//do nothing, will remain ResultStatus.F
			}
			return resultStatus;
		}
		
		///<summary>Converts the value in the MedLab HL7 message field to its corresponding ResultAction enumeration.  Supported values are
		///'A', 'G', and blank.  If parsing the value into an enum fails, or for blank fields, the default is ResultAction.None.</summary>
		public static ResultAction ResultActionParse(string fieldVal) {
			ResultAction resultAction=ResultAction.None;
			if(fieldVal=="") {
				return resultAction;
			}
			try {
				resultAction=(ResultAction)Enum.Parse(typeof(ResultAction),fieldVal,true);
			}
			catch(Exception ex) {
				ex.DoNothing();
				//do nothing, will remain ResultAction.None
			}
			return resultAction;
		}

		///<summary>Searches the field and any repetitions for the ID from the specified source.  Possible sources are "U"=UPIN, "P"=Provider Number
		///(Medicaid or Commercial Ins Prov ID), "N"=NPI, "L"=Local Physician ID.  If the idSource is not a U, P, N, or L or if there is no ID of that
		///type in the field, this will return an empty string.  If fieldCur==null returns empty string.</summary>
		public static string OrderingProvIDParse(FieldHL7 fieldCur,string idSource) {
			if(fieldCur==null) {
				return "";
			}
			if(fieldCur.GetComponentVal(7).Trim().ToLower()==idSource.ToLower()) {
				return fieldCur.GetComponentVal(0).Trim();
			}
			for(int i=0;i<fieldCur.ListRepeatFields.Count;i++) {
				if(fieldCur.ListRepeatFields[i].GetComponentVal(7).Trim().ToLower()==idSource.ToLower()) {
					return fieldCur.ListRepeatFields[i].GetComponentVal(0).Trim();
				}
			}
			return "";//Couldn't locate the ID type in the field or any repetition
		}
	}
}
