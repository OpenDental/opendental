using System.Collections.Generic;
using System.Reflection;

namespace OpenDentBusiness.HL7 {
	///<summary></summary>
	public class InternalEcwStandalone {

		public static HL7Def GetDeepInternal(HL7Def def) {
			//ok to pass in null
			//HL7Def def=HL7Defs.GetInternalFromDb("eCWStandalone");
			if(def==null) {//wasn't in the database
				def=new HL7Def();
				def.IsNew=true;
				def.Description="eCW Standalone";
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
				def.InternalType=HL7InternalType.eCWStandalone;
				def.InternalTypeVersion=Assembly.GetAssembly(typeof(Db)).GetName().Version.ToString();
				def.IsEnabled=false;
				def.Note="";
				def.ShowDemographics=HL7ShowDemographics.ChangeAndAdd;
				def.ShowAccount=true;
				def.ShowAppts=true;
				def.IsQuadAsToothNum=false;
			}
			def.hl7DefMessages=new List<HL7DefMessage>();//so that if this is called repeatedly, it won't pile on duplicate messages.
			//in either case, now get all child objects, which can't be in the database.
			#region Inbound Messages
				#region ADT - Patient Demographics (Admits, Discharges, and Transfers)
				//----------------------------------------------------------------------------------------------------------------------------------
				//eCW incoming patient information (ADT).
				HL7DefMessage msg=new HL7DefMessage();
				def.AddMessage(msg,MessageTypeHL7.ADT,MessageStructureHL7.ADT_A01,InOutHL7.Incoming,0);
				//MSH segment------------------------------------------------------------------
				HL7DefSegment seg=new HL7DefSegment();
				msg.AddSegment(seg,0,SegmentNameHL7.MSH);
				//MSH.8, Message Type
				seg.AddField(8,"messageType");
				//MSH.9, Message Control ID
				seg.AddField(9,"messageControlId");
				//PID segment------------------------------------------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,2,SegmentNameHL7.PID);
				//PID.2, Patient ID
				seg.AddField(2,"pat.ChartNumber");
				//PID.4, Alternate Patient ID, PID.4 is not saved with using standalone integration
				//PID.5, Patient Name
				seg.AddField(5,"pat.nameLFM");
				//PID.7, Date/Time of Birth
				seg.AddField(7,"pat.birthdateTime");
				//PID.8, Administrative Sex
				seg.AddField(8,"pat.Gender");
				//PID.10, Race
				seg.AddField(10,"pat.Race");
				//PID.11, Patient Address
				seg.AddField(11,"pat.addressCityStateZip");
				//PID.13, Phone Number - Home
				seg.AddField(13,"pat.HmPhone");
				//PID.14, Phone Number - Business
				seg.AddField(14,"pat.WkPhone");
				//PID.16, Marital Status
				seg.AddField(16,"pat.Position");
				//PID.19, SSN - Patient
				seg.AddField(19,"pat.SSN");
				//PID.22, Fee Schedule
				seg.AddField(22,"pat.FeeSched");
				//GT1 segment------------------------------------------------------------------
				seg=new HL7DefSegment();
				msg.AddSegment(seg,5,SegmentNameHL7.GT1);
				//GT1.2, Guarantor Number
				seg.AddField(2,"guar.ChartNumber");
				//GT1.3, Guarantor Name
				seg.AddField(3,"guar.nameLFM");
				//GT1.5, Guarantor Address
				seg.AddField(5,"guar.addressCityStateZip");
				//GT1.6, Guarantor Phone Number - Home
				seg.AddField(6,"guar.HmPhone");
				//GT1.7, Guarantor Phone Number - Business
				seg.AddField(7,"guar.WkPhone");
				//GT1.8, Guarantor Date/Time of Birth
				seg.AddField(8,"guar.birthdateTime");
				//GT1.9, Guarantor Administrative Sex
				seg.AddField(9,"guar.Gender");
				//GT1.12, Guarantor SSN
				seg.AddField(12,"guar.SSN");
				#endregion ADT - Patient Demographics (Admits, Discharges, and Transfers)
			#endregion Inbound Messages
			return def;
		}

	}
}
