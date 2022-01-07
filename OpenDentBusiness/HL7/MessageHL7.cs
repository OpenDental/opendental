using System;
using System.Collections.Generic;
using System.Text;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	public class MessageHL7 {
		public List<SegmentHL7> Segments;
		private string originalMsgText;//We'll store this for now, but I don't think we'll use it.
		public MessageTypeHL7 MsgType;
		public EventTypeHL7 EventType;
		public MessageStructureHL7 MsgStructure;
		public string ControlId;
		public string AckCode;
		///<summary>We will grab the event type sent to us to echo back to eCW in acknowledgment. All ADT's and SIU's will be treated the same, so while they may send an event type we do not have in our enumeration, we still want to process it and send back the ACK with the correct event type.</summary>
		public string AckEvent;
		///<summary>The default delimiters are: ^ component separator, ~ repetition separator, \ escape character, and &amp; subcomponent separator.  In that order.</summary>
		public char[] Delimiters;

		///<summary>Only use this constructor when generating a message instead of parsing a message.</summary>
		internal MessageHL7(MessageTypeHL7 msgType) {
			Segments=new List<SegmentHL7>();
			MsgType=msgType;
			ControlId="";
			AckCode="";
			AckEvent="";
			Delimiters=new char[] { '^','~','\\','&' };//this is the default delimiters
			//if def is enabled, set delimiters to user defined values
			HL7Def enabledDef=HL7Defs.GetOneDeepEnabled();
			if(enabledDef!=null) {
				Delimiters=new char[4];
				Delimiters[0]=enabledDef.ComponentSeparator.ToCharArray()[0];//the enabled def is forced to have a component separator that is a single character
				Delimiters[1]=enabledDef.RepetitionSeparator.ToCharArray()[0];//the enabled def is forced to have a repetition separator that is a single character
				Delimiters[2]=enabledDef.EscapeCharacter.ToCharArray()[0];//the enabled def is forced to have an escape character that is a single character
				Delimiters[3]=enabledDef.SubcomponentSeparator.ToCharArray()[0];//the enabled def is forced to have a subcomponent separator that is a single character
			}
		}

		public MessageHL7(string msgtext) {
			AckCode="";
			ControlId="";
			AckEvent="";
			originalMsgText=msgtext;
			Segments=new List<SegmentHL7>();
			string[] rows=msgtext.Split(new string[] { "\r","\n" },StringSplitOptions.RemoveEmptyEntries);
			//We need to get the separator characters in order to create the field objects.
			//The separators are part of the MSH segment and we force users to leave them in position 1 for incoming messages.
			Delimiters=new char[] { '^','~','\\','&' };//this is the default, but we will get them from the MSH segment of the incoming message in case they are using something unique.
			//if def is enabled, set delimiters to user defined values
			HL7Def enabledDef=HL7Defs.GetOneDeepEnabled();
			if(enabledDef!=null) {
				for(int i=0;i<rows.Length;i++) {
					//we're going to assume that the user has not inserted an escaped '|' before the second field of the message and just split by '|'s without
					//checking for escaped '|'s.  Technically '\|' would be a literal pipe and should not indicate a new field, but we only want to retrieve the
					//delimiters from MSH.1 and we require field 0 to be MSH and field 1 should be ^~\&.
					string[] fields=rows[i].Split(new string[] { "|" },StringSplitOptions.None);
					if(fields.Length>1 && fields[0]=="MSH" && fields[1].Length==4) {
						//Encoding characters are in the following order:  component separator, repetition separator, escape character, subcomponent separator
						Delimiters=fields[1].ToCharArray();//we force users to leave the delimiters in position 1 of the MSH segment
						break;
					}
				}
			}
			SegmentHL7 segment;
			for(int i=0;i<rows.Length;i++) {
				segment=new SegmentHL7(rows[i],Delimiters);//this creates the field objects.
				Segments.Add(segment);
				if(i==0 && segment.Name==SegmentNameHL7.MSH) {
//js 7/3/12 Make this more intelligent because we also now need the suffix
					string msgtype=segment.GetFieldComponent(8,0);//We force the user to leave the 'messageType' field in this position, position 8 of the MSH segment
					string evnttype=segment.GetFieldComponent(8,1);
					string msgStructure=segment.GetFieldComponent(8,2);
					AckEvent=evnttype;//We will use this when constructing the acknowledgment to echo back to sender the same event type sent to us
					//If message type or event type are not in this list, they will default to the not supported type and will not be processed
					try {
						MsgType=(MessageTypeHL7)Enum.Parse(typeof(MessageTypeHL7),msgtype,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
						MsgType=MessageTypeHL7.NotDefined;
					}
					try {
						EventType=(EventTypeHL7)Enum.Parse(typeof(EventTypeHL7),evnttype,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
						EventType=EventTypeHL7.NotDefined;
					}
					try {
						MsgStructure=(MessageStructureHL7)Enum.Parse(typeof(MessageStructureHL7),msgStructure,true);
					}
					catch(Exception ex) {
						ex.DoNothing();
						MsgStructure=MessageStructureHL7.NotDefined;
					}
				}
			}
		}

		///<summary>This will always be generated on the fly, based on the FullText of all the segments combined.  FullText for any other object is cached rather than being generated on the fly.</summary>
		public override string ToString() {
			string retVal="";
			for(int i=0;i<Segments.Count;i++) {
				if(i>0) {
					retVal+="\r\n";//in our generic HL7 interface, we should change this to just an \r aka 0D aka \u000d
				}
				retVal+=Segments[i].FullText;
			}
			return retVal;
		}

		///<summary>If an optional segment is not present, this will return null.  If a required segment is missing, this will throw an exception.
		///This should only be used for non-repeatable segments, and only those that would be in a message 0 or 1 time.  If a segment can be in the
		///message more than one time, like a NTE note segment, then this will return the first one found every time and there could be many others.
		///Use GetSegments if the segment can repeat.</summary>
		public SegmentHL7 GetSegment(SegmentNameHL7 segmentName,bool isRequired) {
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].Name==segmentName) {
					return Segments[i];
				}
			}
			if(isRequired) {
				throw new ApplicationException(segmentName+" segment is missing.");
			}
			return null;
		}

		///<summary>If an optional segment is not present, this will return an empty list.  If a required segment is missing, this will throw an exception.</summary>
		public List<SegmentHL7> GetSegments(SegmentNameHL7 segmentName,bool isRequired) {
			List<SegmentHL7> retVal=new List<SegmentHL7>();
			for(int i=0;i<Segments.Count;i++) {
				if(Segments[i].Name!=segmentName) {
					continue;
				}
				retVal.Add(Segments[i]);
			}
			if(isRequired && retVal.Count==0) {
				throw new ApplicationException(segmentName+" segment is missing.");
			}
			return retVal;
		}

		///<summary>Returns false if the segment passed in was found in the message def but not in the message itself. Otherwise; returns true.</summary>
		public bool TryGetSegmentOrder(SegmentNameHL7 segmentName,HL7DefMessage hl7defmsg,out int segmentOrder,out int segmentDefOrder) {
			//Identify the location of the segment within the current message along with the location within the message definition.
			segmentOrder=Segments.FindIndex(x => x.Name==segmentName);
			segmentDefOrder=hl7defmsg.hl7DefSegments.FindIndex(x => x.SegmentName==segmentName);
			if(segmentDefOrder==-1) {
				return true;
			}
			//The segment passed in is present within the def, make sure it exists in the msg.
			return (segmentOrder > -1);
		}

	}

	
}
