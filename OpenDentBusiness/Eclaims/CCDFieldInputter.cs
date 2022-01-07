using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;

namespace OpenDentBusiness.Eclaims {
	/// <summary> The idea is to make reading a CCD message in each of the different forms a smaller amount of overall typing, to save time and reduce bugs.</summary>
	public class CCDFieldInputter{
		private List<CCDField> fieldList = new List<CCDField>();//List of fields that make up the message
		public bool isVersion2;
		public string messageText="";
		///<summary>Code that identifies the CCD message type.</summary>
		public string MsgType="";

		public CCDFieldInputter(){
		}

		///<summary>Throws exceptions.</summary>
		public CCDFieldInputter(string message){
			messageText=message;
			string version=message.Substring(18,2);
			MsgType=message.Substring(20,2);
			if(version=="04") {
				switch(MsgType) {
					case "01":
						ParseClaimRequest_01(message);
						break;
					case "02":
						ParseClaimReversal_02(message);
						break;
					case "03":
						ParsePredetermination_03(message);
						break;
					case "04":
						ParseRotRequest_04(message);
						break;
					case "07":
						ParseCobRequest_07(message);
						break;
					case "11":
						ParseClaimAck_11(message);
						break;
					case "21":
						ParseEOB_21(message);
						break;
					case "12":
						ParseClaimReversalResponse_12(message);
						break;
					case "18":
						ParseResponseToElegibility_18(message);
						break;
					case "24":
						ParseEmailResponse_24(message);
						break;
					case "14":
						ParseOutstandingTransactionsAck_14(message);
						break;
					case "23":
						ParsePredeterminationEOB_23(message);
						break;
					case "13":
						ParsePredeterminationAck_13(message);
						break;
					case "16":
						ParsePaymentReconciliation_16(message);
						break;
					case "15":
						ParseReconciliaiton_15(message);
						break;
					default:
						throw new ApplicationException("Invalid version 04 message type: "+MsgType+Environment.NewLine+"Entire raw message:"+Environment.NewLine+message);
				}
			}
			else {//version 02
				isVersion2=true;
				switch(MsgType) {
					case "01"://claim transaction
						ParseClaimRequest_v2_01(message);
						break;
					case "03"://predetermination (preauth) transaction
						ParsePredetermination_v2_03(message);
						break;
					case "10"://eligibility response
						ParseElegibilityResponse_v2_10(message);
						break;
					case "11"://claim response
						ParseClaimResponse_v2_11(message);
						break;
					case "21"://eob
						ParseEOB_v2_21(message);
						break;
					case "12"://reversal response
						ParseClaimReversalResponse_v2_12(message);
						break;
					case "13"://response to predetermination
						ParsePredeterminationAck_v2_13(message);
						break;
					default:
						throw new ApplicationException("Invalid version 02 message type: "+MsgType+Environment.NewLine+"Entire raw message:"+Environment.NewLine+message);
				}
			}
		}

		///<summary>
		///Currently only used for Claim EOB (MsgType 21) and Predetermination EOB (MsgType 23).
		///Returns a dictionary such that: Each entry represents a procedure and its associated fields.
		///Key:		line number
		///Value:	list of CCDFields pertaining to key/line number.
		///</summary>
		public Dictionary<int,List<CCDField>> GetPerformedProcsDict() {
			Dictionary<int,List<CCDField>> dictProcData=new Dictionary<int, List<CCDField>>();
			//The following list of fields is in order.
			string[] arrayFieldIds=new string[] { "F07","G12","G13","G14","G15","G43","G56","G57","G58","G02","G59","G60","G61","G16","G17" };
			List<CCDField> listFields=GetFieldsByIds(arrayFieldIds);
			int lineNum=0;
			for(int i=0;i<listFields.Count;i++) {
				if(listFields[i].fieldId==arrayFieldIds[0]) {//Beginning of next procedure.  Is a line number, which we use for key.
					lineNum=PIn.Int(listFields[i].valuestr);
					if(!dictProcData.ContainsKey(lineNum)) {
						dictProcData.Add(lineNum,new List<CCDField>());
					}
				}
				else {
					dictProcData[lineNum].Add(listFields[i]);
				}
			}
			return dictProcData;
		}

		///<summary>
		///Currently only used for Claim EOB (MsgType 21) and Predetermination EOB (MsgType 23).
		///Returns a list of carrier issued procs such that each sub list is a list multiple carrier line numbers.
		///</summary>
		public List<List<CCDField>> GetCarrierIssuedProcs() {
			List<List<CCDField>> listProcData=new List<List<CCDField>>();
			//The following list of fields is in order.
			//All fields are present in both version 2 and version 4 except G44.
			string[] arrayFieldIds=new string[] { "G18","G19","G20","G44","G21","G22","G23","G24","G25" };
			List<CCDField> listFields=GetFieldsByIds(arrayFieldIds);//Safe if G44 is not present.
			List<CCDField> listProcFields=null;
			for(int i=0;i<listFields.Count;i++) {
				if(listFields[i].fieldId==arrayFieldIds[0]) {//Beginning of next procedure.  Is a line number, which we use for key.
					listProcFields=new List<CCDField>();
					listProcData.Add(listProcFields);
				}
				listProcFields.Add(listFields[i]);
			}
			return listProcData;
		}

		///<summary>
		///Currently only used for Claim EOB (MsgType 21) and Predetermination EOB (MsgType 23).
		///Returns a dictionary such that: Each entry represents a note line at the bottom of the EOB.
		///Key:		note number (1 based)
		///Value:	note string
		///</summary>
		public Dictionary<int,string> GetExplantionNotesDict() {
			Dictionary<int,string> dictNoteData=new Dictionary<int, string>();
			//The following list of fields is in order.
			string[] arrayFieldIds=new string[] {"G45","G26"};
			List<CCDField> listFields=GetFieldsByIds(arrayFieldIds);
			if(listFields.FirstOrDefault(x => x.fieldId=="G45")==null) {//In version 02, the note number field G45 does not exist, so you use the order listed instead.
				for(int i=0;i<listFields.Count;i++) {
					int lineNumber=i+1;//Line numbers are 1 based for Canadian claims.
					dictNoteData.Add(lineNumber,"");
					dictNoteData[lineNumber]+=listFields[i];
				}
			}
			else { //version 04
				int noteNumber=0;
				for(int i=0;i<listFields.Count;i++) {
					if(listFields[i].fieldId==arrayFieldIds[0]) {//Beginning of next procedure.  Is a line number, which we use for key.
						noteNumber=PIn.Int(listFields[i].valuestr);
						if(!dictNoteData.ContainsKey(noteNumber)) {
							dictNoteData.Add(noteNumber,"");
						}
					}
					else {
						dictNoteData[noteNumber]+=listFields[i].valuestr;
					}
				}
			}
			return dictNoteData;
		}

		public CCDField[] GetLoadedFields(){
			CCDField[] loadedFields=new CCDField[fieldList.Count];
			fieldList.CopyTo(loadedFields);
			return loadedFields;
		}

		/// <summary>Input a single field.</summary>
		public string InputField(string message,string fieldId){
			CCDField field=new CCDField(fieldId,isVersion2);
			int len=field.GetRequiredLength(this);
			if(len >= 0 && message!=null && message.Length < len) {//The remainder of the message (the very last line) is shorter than the field.
				if(fieldId=="G26") {//Note Text
					//We have seen G26 coming back without trailing spaces and instead containing a newline.  G26 is supposed to be 75 chars padded right.
					//We have only seen this issue coming from the ITRANS clearinghouse with regards to the Accerta carrier 311140.
					len=message.Length;//Read the remainder of the message as the G26.
					if(message.EndsWith("\n")) {
						len--;//Exclude the newline character, since we might not handle newlines properly elsewhere.
					}
				}
				else {//Not G26
					return null;//If another field, then we need to investigate before making a decision.  For now, the field is invalid.
				}
			}
			if(len<0 || message==null || message.Length<len){
				return null;
			}
			if(len==0){
				return message;
			}
			string substr=message.Substring(0,len);
			//if(!field.CheckValue(this,substr)){
			//  throw new ApplicationException("Invalid value for CCD message field '"+field.fieldName+"'"+((substr==null)?"":(": "+substr)));
			//}
			field.valuestr=substr;
			fieldList.Add(field);
			return message.Substring(substr.Length,message.Length-substr.Length);//Skip text that has already been read in.
		}

		///<summary>Inputs fields based on a pseudo-script input string. This is possible, since each field id identifies a unique 
		///input pattern. If a field Id is seen, then that field is inputted, but if the ### sequence is encountered, then a field 
		///list is inputted based on the number in the next specified field, which has the format of the field after that. If the
		///input string leads with a "#" followed by a 2 digit number, then the string is input that many times.</summary>
		public string InputFields(string message,string fieldOrderStr) {
			fieldOrderStr=fieldOrderStr.ToUpper();
			if(fieldOrderStr.Length%3!=0) {
				throw new ApplicationException("Internal error, invalid field order string (not divisible by 3): "+fieldOrderStr);
			}
			if(fieldOrderStr.Length<1){
				return message;
			}
			for(int i=0;i<fieldOrderStr.Length;i+=3) {
				string token=fieldOrderStr.Substring(i,3);
				if(token=="###") {//Input field value by field id, then input the value number of fields with the next template.
					string valueFieldId=fieldOrderStr.Substring(i+3,3);
					if(valueFieldId==null||valueFieldId.Length!=3) {
						throw new ApplicationException("Internal error, invalid value field id in: "+fieldOrderStr);
					}
					CCDField valueField=GetFieldById(valueFieldId);
					if(valueField==null) {
						throw new ApplicationException(this.ToString()+".InputCCDFields: Internal error, could not locate value field '"+valueFieldId+"'");
					}
					if(valueField.format!="N") {
						throw new ApplicationException(this.ToString()+".InputCCDFields: Internal error, value field '"+valueFieldId+"' is not an integer");
					}
					string listFieldId=fieldOrderStr.Substring(i+6,3);
					if(listFieldId==null||listFieldId.Length!=3) {
						throw new ApplicationException("Internal error, invalid field list id in: "+fieldOrderStr);
					}
					i+=6;
					int count=0;
					Int32.TryParse(valueField.valuestr,out count);//Treat spaces as 0.
					for(int p=0;p<count;p++) {
						message=InputField(message,listFieldId);
					}
				}
				else {//Input a single field.
					message=InputField(message,token);
				}
			}
			return message;
		}

		///<summary>Get a list of loaded fields by a common field id.</summary>
		public CCDField[] GetFieldsById(string fieldId){
			//lists are short, so just use a simple list search.
			List<CCDField> fields=new List<CCDField>();
			foreach(CCDField field in fieldList){
				if(field.fieldId==fieldId){
					fields.Add(field);//(new CCDField(field,isVersion2));
				}
			}
			return fields.ToArray();
		}

		///<summary>Get a list of loaded fields by field ids. Returns a list of CCDField elements in the same order as recieved, but excludes fields which are missing, thus the result list can be shorter than the given array.</summary>
		public List<CCDField> GetFieldsByIds(params string[] arrayFieldIds) {
			//lists are short, so just use a simple list search.
			List<CCDField> listFields=new List<CCDField>();
			if(arrayFieldIds==null || arrayFieldIds.Length==0) {
				return listFields;
			}
			foreach(CCDField field in fieldList){
				if(arrayFieldIds.Contains(field.fieldId)){
					listFields.Add(field);
				}
			}
			return listFields;
		}

		///<summary>Same as GetFieldsById, but gets only a single field, or returns field with empty value if there are multiple.</summary>
		public CCDField GetFieldById(string fieldId){
			CCDField[] fields=GetFieldsById(fieldId);
			if(fields==null || fields.Length==0) {
				return null;
			}
			if(fields.Length>1) {
				throw new ApplicationException("Internal error, invalid use of ambiguous CCD field id"+((fieldId==null)?"":(": "+fieldId)));
			}
			return fields[0];
		}

		public string GetValue(string fieldId) {
			CCDField[] fields=GetFieldsById(fieldId);
			if(fields==null || fields.Length==0) {
				return "";//Doesn't exist, return with empty value, so at least some information can be used.
			}
			if(fields.Length>1) {
				throw new ApplicationException("Internal error, invalid use of ambiguous CCD field id"+((fieldId==null)?"":(": "+fieldId)));
			}
			return fields[0].valuestr;
		}

		///<summary>Used to read primary eclaim data which has already been sent out to CDAnet for processing.
		///Also used in the internal project CanadaFakeClearinghouse for testing basic eclaim functionality.</summary>
		private void ParseClaimRequest_01(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A10A07A08A09B01B02B03B04B05B06C01C11C02C17C03C04"+
				"C05C06C07C08C09C10C12C18D01D02D03D04D05D06D07D08D09D10D11E18E20F06F22");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldE20=GetFieldById("E20");
			int e20Val=0;
			if(fieldE20!=null) {
				e20Val=Convert.ToInt32(fieldE20.valuestr);
			}
			if(e20Val==1) {
				message=InputFields(message,"E19E01E02E05E03E17E06E04E08E09E10E11E12E13E14E15E16E07");
			}
			message=InputFields(message,"F01F02F03F15F04F18F19F05F20F21");
			CCDField fieldF22=GetFieldById("F22");
			int f22Val=0;
			if(fieldF22!=null) {
				f22Val=Convert.ToInt32(fieldF22.valuestr);
			}
			for(int i=0;i<f22Val;i++) {
				message=InputFields(message,"F23F24");
			}
			CCDField fieldF06=GetFieldById("F06");
			int f06Val=0;
			if(fieldF06!=null) {
				f06Val=Convert.ToInt32(fieldF06.valuestr);
			}
			for(int i=0;i<f06Val;i++) {
				message=InputFields(message,"F07F08F09F10F11F12F34F13F35F36F16F17");
			}
			CCDField fieldC18=GetFieldById("C18");
			int c18Val=0;
			if(fieldC18!=null) {
				c18Val=Convert.ToInt32(fieldC18.valuestr);
			}
			if(c18Val==1) {
				message=InputFields(message,"C19");
			}
		}

		///<summary>Parses a version 04 "Claim Reversal Transaction Format - Trxn Type 02" message.
		///Used by the CanadaFakeClearinghouse internal project for testing.</summary>
		private void ParseClaimReversal_02(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A10A07A09B01B02B03B04C01C11C02C03D02D03D04G01");
		}

		///<summary>Parses a version 04 "Request for Outstanding Transactions - Trxn Type 04" message.
		///Used by the CanadaFakeClearinghouse internal project for testing.</summary>
		private void ParseRotRequest_04(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A10A07A09B01B02B03");
		}

		///<summary>Parses a version 04 predetermination (preauth type 03) message.
		///Is used by the CanadaFakeClearinghouse internal project for testing.</summary>
		private void ParsePredetermination_03(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A10A07A08A09B01B02B03B04B05B06C01C11C02C17C03C04"+
				"C05C06C07C08C09C10C12C18D01D02D03D04D05D06D07D08D09D10D11E18E20F06F22F25");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldE20=GetFieldById("E20");
			int e20Val=0;
			if(fieldE20!=null) {
				e20Val=Convert.ToInt32(fieldE20.valuestr);
			}
			if(e20Val==1) {
				message=InputFields(message,"E19E01E02E05E03E17E06E04E08E09E10E11E12E13E14E15E16E07");
			}
			message=InputFields(message,"F02F15F04F18F19F05F20F21");
			CCDField fieldF22=GetFieldById("F22");
			int f22Val=0;
			if(fieldF22!=null) {
				f22Val=Convert.ToInt32(fieldF22.valuestr);
			}
			for(int i=0;i<f22Val;i++) {
				message=InputFields(message,"F23F24");
			}
			message=InputFields(message,"G46G47");
			CCDField fieldF25=GetFieldById("F25");
			int f25Val=0;
			if(fieldF25!=null) {
				f25Val=Convert.ToInt32(fieldF25.valuestr);
			}
			if(f25Val==1) {
				message=InputFields(message,"F37F26F27F28F29F30F31F32");
			}
			CCDField fieldF06=GetFieldById("F06");
			int f06Val=0;
			if(fieldF06!=null) {
				f06Val=Convert.ToInt32(fieldF06.valuestr);
			}
			for(int i=0;i<f06Val;i++) {
				message=InputFields(message,"F07F08F10F11F12F34F13F35F36F16F17");
			}
			CCDField fieldC18=GetFieldById("C18");
			int c18Val=0;
			if(fieldC18!=null) {
				c18Val=Convert.ToInt32(fieldC18.valuestr);
			}
			if(c18Val==1) {
				message=InputFields(message,"C19");
			}
		}

		///<summary>Used to read secondary eclaim data which has already been sent out to CDAnet for processing.
		///Also used in the internal project CanadaFakeClearinghouse for testing basic eclaim functionality.</summary>
		private void ParseCobRequest_07(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A10A07A08A09B01B02B03B04B05B06C01C11C02C17C03C04"+
				"C05C06C07C08C09C10C12C18D01D02D03D04D05D06D07D08D09D10D11E18E20F06F22G39");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldE20=GetFieldById("E20");
			int e20Val=0;
			if(fieldE20!=null) {
				e20Val=Convert.ToInt32(fieldE20.valuestr);
			}
			if(e20Val==1) {
				message=InputFields(message,"E19E01E02E05E03E17E06E04E08E09E10E11E12E13E14E15E16E07");
			}
			message=InputFields(message,"F01F02F03F15F04F18F19F05F20F21");
			CCDField fieldF22=GetFieldById("F22");
			int f22Val=0;
			if(fieldF22!=null) {
				f22Val=Convert.ToInt32(fieldF22.valuestr);
			}
			for(int i=0;i<f22Val;i++) {
				message=InputFields(message,"F23F24");
			}
			CCDField fieldF06=GetFieldById("F06");
			int f06Val=0;
			if(fieldF06!=null) {
				f06Val=Convert.ToInt32(fieldF06.valuestr);
			}
			for(int i=0;i<f06Val;i++) {
				message=InputFields(message,"F07F08F09F10F11F12F34F13F35F36F16F17");
			}
			CCDField fieldC18=GetFieldById("C18");
			int c18Val=0;
			if(fieldC18!=null) {
				c18Val=Convert.ToInt32(fieldC18.valuestr);
			}
			if(c18Val==1) {
				message=InputFields(message,"C19");
			}
			message=this.InputField(message,"G40");
		}

		private void ParseClaimAck_11(string message) {
			message=this.InputFields(message,
																		"A01A02A03A04A05A07A11B01B02G01G05G06G07G04G27G31G39"+
																		"###G31G32"+//Read field G32 the number of times equal to the value in G31.
																		"G42");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldG06=this.GetFieldById("G06");
			if(fieldG06==null) {
				return;//error
			}
			if(fieldG06.format!="N") {
				MessageBox.Show(this.ToString()+"PrintClaimAck_11: Internal error, field G06 is not an integer");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG06.valuestr);i++) {//Input a list of sub-records.
				message=this.InputFields(message,"F07G08");
				if(message==null) {
					return;//error
				}
			}
			message=this.InputField(message,"G40");
		}

		private void ParseEOB_21(string message) {
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02G01G03G04G27F06G10G11G28G29G30F01G33G55G39G42");
			CCDField fieldF06=this.GetFieldById("F06");
			if(fieldF06==null) {
				return;//error
			}
			if(fieldF06.format!="N") {
				MessageBox.Show(this.ToString()+".PrintEOB_21: Internal error, field F06 is not of integer type!");
				return;
			}
			for(int i=0;i<Convert.ToInt32(fieldF06.valuestr);i++) {
				message=this.InputFields(message,"F07G12G13G14G15G43G56G57G58G02G59G60G61G16G17");
			}
			CCDField fieldG10=this.GetFieldById("G10");
			if(fieldG10==null) {
				return;//error	
			}
			if(fieldG10.format!="N") {
				MessageBox.Show(this.ToString()+".PrintEOB_21: Internal error, field G10 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG10.valuestr);i++) {
				message=this.InputFields(message,"G18G19G20G44G21G22G23G24G25");
			}
			CCDField fieldG11=this.GetFieldById("G11");
			if(fieldG11==null) {
				return;//error
			}
			if(fieldG11.format!="N") {
				MessageBox.Show(this.ToString()+".PrintEOB_21: Internal error, field G11 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG11.valuestr);i++) {
				message=this.InputFields(message,"G41G45G26");
			}
			message=this.InputFields(message,"G40");
		}

		private void ParseClaimReversalResponse_12(string message) {			
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02E19G01G05G06G07G04G31"+
																					"###G31G32"+
																					"###G06G08");
			return;
		}

		private void ParseResponseToElegibility_18(string message) {
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02G01G05G06G07G31G42"+
																					"###G06G08"+
																					"###G31G32");
			return;
		}

		private void ParseEmailResponse_24(string message) {			
			message=this.InputFields(message,"A01A02A03A04A07A11G48G54G49G50G51G52"+
																					"###G52G53");
			return;
		}

		private void ParseOutstandingTransactionsAck_14(string message) {			
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02B03G05G06G07"+
																					"###G06G08");
			return;
		}

		private void ParsePredeterminationEOB_23(string message) {
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02G01G04G27F06G10G11G28G29G30G39G42G46G47");
			CCDField fieldF06=this.GetFieldById("F06");
			if(fieldF06==null) {
				return;//error, but return as much of the form as we were able to understand.
			}
			if(fieldF06.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePredeterminationEOB_23: Internal error, field F06 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldF06.valuestr);i++) {
				message=this.InputFields(message,"F07G12G13G14G15G43G56G57G58G02G59G60G61G16G17");
			}
			CCDField fieldG10=this.GetFieldById("G10");
			if(fieldG10==null) {
				return;//error
			}
			if(fieldG10.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePredeterminationEOB_23: Internal error, field G10 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG10.valuestr);i++) {
				message=this.InputFields(message,"G18G19G20G44G21G22G23G24G25");
			}
			CCDField fieldG11=this.GetFieldById("G11");
			if(fieldG11==null) {
				return;//error
			}
			if(fieldG11.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePredeterminationEOB_23: Internal error, field G11 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG11.valuestr);i++) {
				message=this.InputFields(message,"G41G45G26");
			}
			message=this.InputFields(message,"G40");
			return;
		}

		private void ParsePredeterminationAck_13(string message) {			
			message=this.InputFields(message,"A01A02A03A04A05A07A11B01B02G01G05G06G07G04G27G31G39"+
																					"###G31G32"+
																					"G42G46G47");
			CCDField fieldG06=this.GetFieldById("G06");
			if(fieldG06==null) {
				return;//error
			}
			if(fieldG06.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePredeterminationAck_13: Internal error, field G06 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG06.valuestr);i++) {
				message=this.InputFields(message,"F07G08");
			}
			message=this.InputFields(message,"G40");
			return;
		}

		private void ParsePaymentReconciliation_16(string message) {			
			message=this.InputFields(message,"A01A02A03A04A07A11B04G01G05G06G11G34G35G36G37G33F38G62");
			CCDField fieldG37=this.GetFieldById("G37");
			if(fieldG37==null) {
				return;//error
			}
			if(fieldG37.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePaymentReconciliation_16: Internal error, field G37 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG37.valuestr);i++) {
				message=this.InputFields(message,"B01B02B03A05A02G01G38");
			}
			CCDField fieldG11=this.GetFieldById("G11");
			if(fieldG11==null) {
				return;//error
			}
			if(fieldG11.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePaymentReconciliation_16: Internal error, field G11 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG11.valuestr);i++) {
				message=this.InputFields(message,"G41G26");
			}
			message=this.InputFields(message,"###G06G08");
			return;
		}

		private void ParseReconciliaiton_15(string message) {			
			message=this.InputFields(message,"A01A02A03A04A07A11B02G01G05G06G11G34G35G36G37G33");
			CCDField fieldG37=this.GetFieldById("G37");
			if(fieldG37==null) {
				return;//error
			}
			if(fieldG37.format!="N") {
				MessageBox.Show(this.ToString()+".ParseReconciliaiton_15: Internal error, field G37 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG37.valuestr);i++) {
				message=this.InputFields(message,"B01A05A02G01G38");
			}
			CCDField fieldG11=this.GetFieldById("G11");
			if(fieldG11==null) {
				return;//error
			}
			if(fieldG11.format!="N") {
				MessageBox.Show(this.ToString()+".ParseReconciliaiton_15: Internal error, field G11 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG11.valuestr);i++) {
				message=this.InputFields(message,"G41G26");
			}
			message=this.InputFields(message,"###G06G08");
			return;
		}

		private void ParseClaimRequest_v2_01(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A07A08B01B02C01C11C02C03C04C05C06C07C08C09C10D01D02D03D04D05D06D07D08D09D10E01E02E05E03E04"+
				"F01F02F03F15F04F05F06");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldF06=GetFieldById("F06");
			int f06Val=0;
			if(fieldF06!=null) {
				f06Val=Convert.ToInt32(fieldF06.valuestr);
			}
			for(int i=0;i<f06Val;i++) {
				message=InputFields(message,"F07F08F09F10F11F12F13F14");
			}
		}

		///<summary>Parses a version 02 predetermination (preauth type 03) message.
		///Is used by the CanadaFakeClearinghouse internal project for testing.</summary>
		private void ParsePredetermination_v2_03(string message) {
			message=InputFields(message,"A01A02A03A04A05A06A07A08B01B02C01C11C02C03C04"+
				"C05C06C07C08C09C10D01D02D03D04D05D06D07D08D09D10E01E02E05E03E04F02F15F04F05F06");
			if(message==null) {
				return;//error, but print what we have.
			}
			CCDField fieldF06=GetFieldById("F06");
			int f06Val=0;
			if(fieldF06!=null) {
				f06Val=Convert.ToInt32(fieldF06.valuestr);
			}
			for(int i=0;i<f06Val;i++) {
				message=InputFields(message,"F07F08F10F11F12F13F14");
			}
		}

		private void ParseElegibilityResponse_v2_10(string message) {
			message=this.InputFields(message,"A01A02A03A04A05A07B01B02G01G05G06G07G02"+
																				"###G06G08");
			return;
		}

		private void ParseClaimResponse_v2_11(string message){
			message=this.InputFields(message,"A01A02A03A04A05A07B01B02G01G05G06G07G02G04G27");
			CCDField fieldG06=this.GetFieldById("G06");
			if(fieldG06==null) {
				return;//error
			}
			if(fieldG06.format!="N") {
				MessageBox.Show(this.ToString()+".ParseClaimResponse_v2_11: Internal error, field G06 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG06.valuestr);i++) {
				message=this.InputFields(message,"F07G08");
			}
		}

		private void ParseEOB_v2_21(string message){
			message=this.InputFields(message,"A01A02A03A04A05A07B01B02G01G03G04G27G09F06G10G11G28G29G30");
			CCDField fieldF06=this.GetFieldById("F06");
			if(fieldF06==null) {
				return;//error
			}
			if(fieldF06.format!="N") {
				MessageBox.Show(this.ToString()+".ParseEOB_v2_21: Internal error, field F06 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldF06.valuestr);i++) {
				message=this.InputFields(message,"F07G12G13G14G15G16G17");
			}
			CCDField fieldG10=this.GetFieldById("G10");
			if(fieldG10==null) {
				return;//error
			}
			if(fieldG10.format!="N") {
				MessageBox.Show(this.ToString()+".ParseEOB_v2_21: Internal error, field G10 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG10.valuestr);i++) {
				message=this.InputFields(message,"G18G19G20G21G22G23G24G25");
			}
			message=this.InputFields(message,"###G11G26");
		}

		private void ParseClaimReversalResponse_v2_12(string message){
			message=this.InputFields(message,"A01A02A03A04A05A07B01B02G01G05G06G07G04###G06G08");
		}

		private void ParsePredeterminationAck_v2_13(string message){
			message=this.InputFields(message,"A01A02A03A04A05A07B01B02G01G05G06G07G02G04");
			CCDField fieldG06=this.GetFieldById("G06");
			if(fieldG06==null) {
				return;//error
			}
			if(fieldG06.format!="N") {
				MessageBox.Show(this.ToString()+".ParsePredeterminationAck_v2_13: Internal error, field G06 is not of integer type!");
				return;//error
			}
			for(int i=0;i<Convert.ToInt32(fieldG06.valuestr);i++) {
				message=this.InputFields(message,"F07G08");
			}
		}

		///<summary>Probably some missing types.  Mostly focussed on response types.</summary>
		public EtransType GetEtransType() {
			string msgType=GetValue("A04");
			if(!isVersion2) {//version 4
				switch(msgType) {
					case "01":
						return EtransType.Claim_CA;
					case "11":
						return EtransType.ClaimAck_CA;
					case "21":
						return EtransType.ClaimEOB_CA;
					case "12":
						return EtransType.ReverseResponse_CA;
					case "18":
						return EtransType.EligResponse_CA;
					case "24":
						return EtransType.EmailResponse_CA;
					case "04":
						return EtransType.RequestOutstand_CA;
					case "14":
						return EtransType.OutstandingAck_CA;
					case "23":
						return EtransType.PredetermEOB_CA;
					case "13":
						return EtransType.PredetermAck_CA;
					case "16":
						return EtransType.PaymentResponse_CA;
					case "15":
						return EtransType.SummaryResponse_CA;
					default:
						throw new ApplicationException("Version 4 message type not recognized: "+msgType);
				}
			}
			else {//version 02
				switch(msgType) {
					case "01":
						return EtransType.Claim_CA; 
					case "10"://eligibility response
						return EtransType.EligResponse_CA;
					case "11"://claim response
						return EtransType.ClaimAck_CA;
					case "21"://eob
						return EtransType.ClaimEOB_CA;
					case "12"://reversal response
						return EtransType.ReverseResponse_CA;
					case "13"://response to predetermination
						return EtransType.PredetermAck_CA;
					default:
						throw new ApplicationException("Version 2 message type not recognized: "+msgType);
				}
			}
		}

		///<summary>Returns true if all proc line numbers have associated payment lines.</summary>
		public bool HasValidPaymentLines() {
			CCDField[] arrayProcedureLineNumbers=this.GetFieldsById("F07");
			CCDField[] arrayEligibleAmounts=this.GetFieldsById("G12");
			CCDField[] arrayDeductibleAmounts=this.GetFieldsById("G13");
			CCDField[] arrayEligiblePercentage=this.GetFieldsById("G14");
			CCDField[] arrayBenefitAmountForTheProcedures=this.GetFieldsById("G15");
			if(arrayProcedureLineNumbers==null
				|| arrayEligibleAmounts==null
				|| arrayDeductibleAmounts==null
				|| arrayEligiblePercentage==null
				|| arrayBenefitAmountForTheProcedures==null
				|| arrayProcedureLineNumbers.Length!=arrayEligibleAmounts.Length
				|| arrayProcedureLineNumbers.Length!=arrayDeductibleAmounts.Length
				|| arrayProcedureLineNumbers.Length!=arrayEligiblePercentage.Length
				|| arrayProcedureLineNumbers.Length!=arrayBenefitAmountForTheProcedures.Length) 
			{
				return false;
			}
			return true;
		}

		///<summary>Throws exceptions.  Returns null if there is no embedded transaction.</summary>
		public CCDFieldInputter GetEmbeddedTransaction() {
			CCDField fieldG40=GetFieldById("G40");
			if(fieldG40!=null) {//An embedded transaction exists.
				return new CCDFieldInputter(fieldG40.valuestr);
			}
			return null;
		}

	}
}