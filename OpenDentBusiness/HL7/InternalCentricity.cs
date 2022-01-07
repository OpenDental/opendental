using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness.HL7 {
	///<summary></summary>
	public class InternalCentricity {

		public static HL7Def GetDeepInternal(HL7Def def) {
			//ok to pass in null
			if(def==null) {//wasn't in the database
				def=new HL7Def();
				def.IsNew=true;
				def.Description="Centricity";
				def.ModeTx=ModeTxHL7.File;
				def.IncomingFolder="";
				def.OutgoingFolder="";
				def.IncomingPort="";
				def.OutgoingIpPort="";
				def.SftpInSocket="";
				def.SftpUsername="";
				def.SftpPassword="";
				def.FieldSeparator="|";
				def.ComponentSeparator="^";
				def.SubcomponentSeparator="&";
				def.RepetitionSeparator="~";
				def.EscapeCharacter=@"\";
				def.IsInternal=true;
				def.InternalType=HL7InternalType.Centricity;
				def.InternalTypeVersion=Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString();
				def.IsEnabled=false;
				def.Note="";
				def.ShowDemographics=HL7ShowDemographics.ChangeAndAdd;
				def.ShowAccount=true;
				def.ShowAppts=true;
				def.IsQuadAsToothNum=false;
			}
			def.hl7DefMessages=new List<HL7DefMessage>();
			HL7DefMessage msg=new HL7DefMessage();
			HL7DefSegment seg=new HL7DefSegment();
			#region Outbound Messages
				#region DFT - Detailed Financial Transaction
				//=======================================================================================================================
				//Detail financial transaction (DFT)
				def.AddMessage(msg,MessageTypeHL7.DFT,MessageStructureHL7.DFT_P03,InOutHL7.Outgoing,2);
				//MSH (Message Header) segment-------------------------------------------------
				msg.AddSegment(seg,0,SegmentNameHL7.MSH);
				//HL7 documentation says field 1 is Field Separator.  "This field contains the separator between the segment ID and the first real field.  As such it serves as the separator and defines the character to be used as a separator for the rest of the message." (HL7 v2.6 documentation) The separator is usually | (pipes) and is part of field 0, which is the segment ID followed by a |.  Encoding Characters is the first real field, so it will be numbered starting with 1 in our def.
				//MSH.1, Encoding Characters (DataType.ST)
				seg.AddField(1,"separators^~\\&");
				//MSH.2, Sending Application
				seg.AddFieldFixed(2,DataTypeHL7.HD,"OD");
				//MSH.4, Receiving Application
				seg.AddFieldFixed(4,DataTypeHL7.HD,def.Description);
				//MSH.6, Message Date and Time (YYYYMMDDHHMMSS)
				seg.AddField(6,"dateTime.Now");
				//MSH.8, Message Type^Event Type, example DFT^P03
				seg.AddField(8,"messageType");
				//MSH.9, Message Control ID
				seg.AddField(9,"messageControlId");
				//MSH.10, Processing ID (P-production, T-test)
				seg.AddFieldFixed(10,DataTypeHL7.PT,"P");
				//MSH.11, Version ID
				seg.AddFieldFixed(11,DataTypeHL7.VID,"2.3");
				//MSH.16, Application Ack Type (AL=Always, NE=Never, ER=Error/reject conditions only, SU=Successful completion only)
				seg.AddFieldFixed(15,DataTypeHL7.ID,"NE");
				//EVN (Event Type) segment-----------------------------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,1,SegmentNameHL7.EVN);
				//EVN.1, Event Type, example P03
				seg.AddField(1,"eventType");
				//EVN.2, Recorded Date/Time
				seg.AddField(2,"dateTime.Now");
				//EVN.3, Event Reason Code
				seg.AddFieldFixed(3,DataTypeHL7.IS,"01");
				//PID (Patient Identification) segment-----------------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,2,SegmentNameHL7.PID);
				//PID.1, Sequence Number (1 for DFT's)  "This field contains the number that identifies this transaction.  For the first occurrence of the segment, the sequence number shall be one, for the second occurrence, the sequence number shall be two, etc." (HL7 v2.6 documentation)  We only send 1 PID segment in DFT's so this number will always be 1.
				seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
				//PID.2, Patient ID (External)
				seg.AddField(2,"pat.ChartNumber");
				//PID.3, Patient ID (Internal)
				seg.AddField(3,"pat.PatNum");
				//PV1 (Patient Visit) segment--------------------------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,3,SegmentNameHL7.PV1);
				//PV1.1, Set ID - PV1 (1 for DFT's)  See the comment above for the Sequence Number of the PID segment.  Always 1 since we only send one PV1 segment per DFT message.
				seg.AddFieldFixed(1,DataTypeHL7.SI,"1");
				//PV1.2, Patient Class  (E=Emergency, I=Inpatient, O=Outpatient, P=Preadmit, R=Recurring patient, B=Obstetrics, C=Commercial Account, N=Not Applicable, U=Unkown)  We will just send O for outpatient for every DFT message.
				seg.AddFieldFixed(2,DataTypeHL7.IS,"O");
	//todo: ClinicNum?
				//PV1.3, Assigned Patient Location
				//PV1.7, Attending/Primary Care Doctor
				seg.AddField(7,"prov.provIdNameLFM");
	//todo: Referring Dr?
				//PV1.8, Referring Doctor
				//PV1.19, Visit Number
				seg.AddField(19,"apt.AptNum");
				//PV1.44, Admit Date/Time
				seg.AddField(44,"proc.procDateTime");
				//PV1.50, Alternate Visit ID
				//FT1 (Financial Transaction Information) segment------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,4,true,true,SegmentNameHL7.FT1);
				//FT1.1, Sequence Number (starts with 1)
				seg.AddField(1,"sequenceNum");
				//FT1.2, Transaction ID
				seg.AddField(2,"proc.ProcNum");
				//FT1.4, Transaction Date (YYYYMMDDHHMMSS)
				seg.AddField(4,"proc.procDateTime");
				//FT1.5, Transaction Posting Date (YYYYMMDDHHMMSS)
				seg.AddField(5,"proc.procDateTime");
				//FT1.6, Transaction Type
				seg.AddFieldFixed(6,DataTypeHL7.IS,"CG");
				//FT1.10, Transaction Quantity
				seg.AddFieldFixed(10,DataTypeHL7.NM,"1.0");
				//FT1.11, Transaction Amount Extended (Total fee to charge for this procedure, independent of transaction quantity)
				seg.AddField(11,"proc.ProcFee");
				//FT1.12, Transaction Amount Unit (Fee for this procedure for each transaction quantity)
				seg.AddField(12,"proc.ProcFee");
	//todo: ClinicNum?
				//FT1.16, Assigned Patient Location
				//FT1.19, Diagnosis Code
				seg.AddField(19,"proc.DiagnosticCode");
				//FT1.21, Ordering Provider
				seg.AddField(21,"prov.provIdNameLFM");
				//FT1.22, Unit Cost (procedure fee)
				seg.AddField(22,"proc.ProcFee");
				//FT1.25, Procedure Code
				seg.AddField(25,"proccode.ProcCode");
				//FT1.26, Modifiers (treatment area)
				seg.AddField(26,"proc.toothSurfRange");
				//DG1 (Diagnosis) segment is optional, skip for now
				//PR1 (Procedures) segment is optional, skip for now
				#endregion DFT - Detailed Financial Transaction
			#endregion Outbound Messages
			return def;
		}

	}
}

