using System;
using System.Collections.Generic;

namespace OpenDentBusiness{
	///<summary></summary>
	public class X999:X12object{

		public X999(string messageText):base(messageText){		
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

		///<summary>Do this first to get a list of all trans nums that are contained within this 999.  Then, for each trans num, we can later retrieve the AckCode for that single trans num.</summary>
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
					if(seg.SegmentID!="IK5"){
						continue;
					}
					string code=seg.Get(1);
					string ack="";
					if(code=="A" || code=="E") {//Accepted or accepted with Errors.
						ack="A";
					}
					else { //M, R, W or X
						ack="R";
					}
					return ack;
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
			string ack="";
			if(code=="A" || code=="E"){//Accepted or accepted with Errors.
				ack="A";
			}
			else if(code=="P") {//Partially accepted
				ack="P";
			}
			else { //M, R, W, X
				ack="R";//rejected
			}
			return ack;
		}

		///<summary></summary>
		public string GetHumanReadable() {
			string retVal="";
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].SegmentID!="IK3"
					&& Segments[i].SegmentID!="IK4") {
					continue;
				}
				if(retVal != "") {//if multiple errors
					retVal+="\r\n";
				}
				if(Segments[i].SegmentID=="IK3") {
					retVal+="Segment "+Segments[i].Get(1)+": "+GetSegmentSyntaxError(Segments[i].Get(4));
				}
				if(Segments[i].SegmentID=="IK4") {
					retVal+="Element "+Segments[i].Get(1)+": "+GetElementSyntaxError(Segments[i].Get(3));
				}
				//retVal+=GetRejectReason(Segments[i].Get(3))+", "
				//	+GetFollowupAction(Segments[i].Get(4));
			}
			return retVal;
		}

		private string GetSegmentSyntaxError(string code) {
			switch(code) {
				case "1": return "Unrecognized segment ID";
				case "2": return "Unexpected segment";
				case "3": return "Required segment missing";
				case "4": return "Loop occurs over maximum times";
				case "5": return "Segment exceeds maximum use";
				case "6": return "Segment not in defined transaction set";
				case "7": return "Segment not in proper sequence";
				case "8": return "Segment has data element errors";
				case "I4": return "Implementation \"not used\" segment present";
				case "I6": return "Implementation dependent segment missing";
				case "I7": return "Implementation loop occurs under minimum times";
				case "I8": return "Implementation segment below minimum use";
				case "I9": return "Implementation dependent \"not used\" segment present";
				default: return code;//will never happen
			}
		}

		private string GetElementSyntaxError(string code) {
			switch(code) {
				case "1": return "Required data element missing";
				case "2": return "Conditional required data element missing";
				case "3": return "Too many data elements";
				case "4": return "Data element too short";
				case "5": return "Data element too long";
				case "6": return "Invalid character in data element";
				case "7": return "Invalid code value";
				case "8": return "Invalid date";
				case "9": return "Invalid time";
				case "10": return "Exclusion condition violated";
				case "12": return "Too many repetitions";
				case "13": return "Too many components";
				case "I10": return "Implementation \"not used\" data element present";
				case "I11": return "Implementation too few repetitions";
				case "I12": return "Implementation pattern match failure";
				case "I13": return "Implementation dependent \"not used\" data element present";
				case "I6": return "Code value not used in implementation";
				case "I9": return "Implementation dependent data element missing";
				default: return code;//will never happen
			}
		}
	

	}
}
