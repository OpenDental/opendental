using System;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary></summary>
	public class X997:X12object{

		public X997(string messageText):base(messageText){
		
		}
		
		///<summary>In X12 lingo, the batchNumber is known as the functional group.</summary>
		public int GetBatchNumber(){
			if(this.FunctGroups[0].Transactions.Count!=1) {
				return 0;
			}
			X12Segment seg=FunctGroups[0].Transactions[0].GetSegmentByID("AK1");
			if(seg==null) {
				return 0;
			}
			string num=seg.Get(2);
			try{
				return PIn.Int(num);
			}
			catch{
				return 0;
			}
		}

		///<summary>Do this first to get a list of all trans nums that are contained within this 997.  Then, for each trans num, we can later retrieve the AckCode for that single trans num.</summary>
		public List<int> GetTransNums(){
			List<int> retVal=new List<int>();
			X12Segment seg;
			int transNum=0;
			for(int i=0;i<FunctGroups[0].Transactions[0].Segments.Count;i++){
				seg=FunctGroups[0].Transactions[0].Segments[i];
				if(seg.SegmentID=="AK2"){
					transNum=0;
					try{
						transNum=PIn.Int(seg.Get(2));
					}
					catch{
						transNum=0;
					}
					if(transNum!=0){
						retVal.Add(transNum);
					}
				}
			}
			return retVal;
		}

		///<summary>Use after GetTransNums.  Will return A=Accepted, R=Rejected, or "" if can't determine.</summary>
		public string GetAckForTrans(int transNum){
			X12Segment seg;
			bool foundTransNum=false;
			int thisTransNum=0;
			for(int i=0;i<FunctGroups[0].Transactions[0].Segments.Count;i++){
				seg=FunctGroups[0].Transactions[0].Segments[i];
				if(foundTransNum){
					if(seg.SegmentID!="AK5"){
						continue;
					}
					string code=seg.Get(1);
					if(code=="A" || code=="E") {//Accepted or accepted with Errors.
						return "A";
					}
					return "R";//rejected
				}
				if(seg.SegmentID=="AK2"){
					thisTransNum=0;
					try {
						thisTransNum=PIn.Int(seg.Get(2));
					}
					catch {
						thisTransNum=0;
					}
					if(thisTransNum==transNum) {
						foundTransNum=true;
					}
				}
			}
			return "";
		}

		///<summary>Will return "" if unable to determine.  But would normally return A=Accepted or R=Rejected or P=Partially accepted if only some of the transactions were accepted.</summary>
		public string GetBatchAckCode(){
			if(this.FunctGroups[0].Transactions.Count!=1){
				return "";
			}
			X12Segment seg=FunctGroups[0].Transactions[0].GetSegmentByID("AK9");
			if(seg==null){
				return "";
			}
			string code=seg.Get(1);
			if(code=="A" || code=="E"){//Accepted or accepted with Errors.
				return "A";
			}
			if(code=="P") {//Partially accepted
				return "P";
			}
			return "R";//rejected
		}		

		/*Example 997
		ISA*00*          *00*          *ZZ*113504607      *ZZ*               *070813*0930*U*00401*705547511*0*P*:~
		GS*FA*113504607**20070813*0930*705547511*X*004010X097A1~
		ST*997*0001~
		AK1*HC*0001~
		AK2*837*0001~
		AK5*A~
		AK9*A*1*1*1~
		SE*6*0001~
		GE*1*705547511~
		IEA*1*705547511~
		*/
		//the only rows that we evaluate are AK2, which has transaction# (batchNumber), and AK5 which has ack code.

		///<summary></summary>
		public string GetHumanReadable() {
			string retVal="";
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].SegmentID!="AK3"
					&& Segments[i].SegmentID!="AK4") {
					continue;
				}
				if(retVal != "") {//if multiple errors
					retVal+="\r\n";
				}
				if(Segments[i].SegmentID=="AK3") {
					retVal+="Segment "+Segments[i].Get(1)+": "+GetSegmentSyntaxError(Segments[i].Get(4));
				}
				if(Segments[i].SegmentID=="AK4") {
					retVal+="Element "+Segments[i].Get(1)+": "+GetElementSyntaxError(Segments[i].Get(3));
				}
				//retVal+=GetRejectReason(Segments[i].Get(3))+", "
				//	+GetFollowupAction(Segments[i].Get(4));
			}
			return retVal;
		}

		/*Example of 997 from failed 270 request.
		ISA*00*          *00*          *30*330989922      *ZZ*810624427      *090819*1501*U*00401*000000000*0*T*:~
		GS*FA*330989922*330989922*20090819*1501*0*X*004010~
		ST*997*0001~
		AK1*HS*26~
		AK2*270*0001~
		AK3*NM1*4**8~
		AK4*9*725*4*1~
		AK5*R*5~
		AK9*R*1*1*0~
		SE*8*0001~
		GE*1*0~
		IEA*1*000000000~
		 */

		private string GetSegmentSyntaxError(string code) {
			switch(code) {
				case "1": return "Unrecognized segment ID";
				case "2": return "Unexpected segment";
				case "3": return "Mandatory segment missing";
				case "4": return "Loop Occurs Over Maximum Times";
				case "5": return "Segment Exceeds Maximum Use";
				case "6": return "Segment Not in Defined Transaction Set";
				case "7": return "Segment Not in Proper Sequence";
				case "8": return "Segment Has Data Element Errors";
				default: return code;//will never happen
			}
		}

		private string GetElementSyntaxError(string code) {
			switch(code) {
				case "1": return "Mandatory data element missing";
				case "2": return "Conditional required data element missing";
				case "3": return "Too many data elements";
				case "4": return "Data element too short";
				case "5": return "Data element too long";
				case "6": return "Invalid character in data element";
				case "7": return "Invalid code value";
				case "8": return "Invalid Date";
				case "9": return "Invalid Time";
				case "10": return "Exclusion Condition Violated";
				default: return code;//will never happen
			}
		}
	

	}
}
