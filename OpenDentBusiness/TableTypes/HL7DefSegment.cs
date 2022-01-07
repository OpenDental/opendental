using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>multiple segments per message</summary>
	[Serializable]
	public class HL7DefSegment:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long HL7DefSegmentNum;
		///<summary>FK to hl7defmessage.HL7DefMessageNum</summary>
		public long HL7DefMessageNum;
		///<summary>Since we don't enforce or automate, it can be 1-based or 0-based.  For outgoing, this affects the message structure.  For incoming, this is just for convenience and organization in the HL7 Def windows.</summary>
		public int ItemOrder;
		///<summary>For example, a DFT can have multiple FT1 segments.  This turns out to be a completely useless field, since we already know which ones can repeat.</summary>
		public bool CanRepeat;
		///<summary>If this is false, and an incoming message is missing this segment, then it gets logged as an error/failure.  If this is true, then it will gracefully skip a missing incoming segment.  Not used for outgoing.</summary>
		public bool IsOptional;
		///<summary>Stored in db as string, but used in OD as enum SegmentNameHL7. Example: PID.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public SegmentNameHL7 SegmentName;
		///<summary>.</summary>
//TODO: This column may need to be changed to the TextIsClobNote attribute to remove more than 50 consecutive new line characters.
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Note;

		///<Summary>List of segments associated with this hierarchical definition.  Use items in this list to get to items lower in the hierarchy.</Summary>
		[CrudColumn(IsNotDbColumn=true)]
		[XmlIgnore]
		public List<HL7DefField> hl7DefFields;

		///<summary></summary>
		public HL7DefSegment Clone() {
			return (HL7DefSegment)this.MemberwiseClone();
		}

		public void AddField(int ordinalPos,string tableId,DataTypeHL7 dataType,string fieldName,string fixedText) {
			if(hl7DefFields==null){
				hl7DefFields=new List<HL7DefField>();
			}
			HL7DefField field=new HL7DefField();
			field.OrdinalPos=ordinalPos;
			field.DataType=dataType;
			field.TableId=tableId;
			field.FieldName=fieldName;
			field.FixedText=fixedText;
			this.hl7DefFields.Add(field);
		}

		public void AddField(int ordinalPos,string fieldName) {
			DataTypeHL7 dataType=FieldNameAndType.GetTypeFromName(fieldName);
			string tableNum=FieldNameAndType.GetTableNumFromName(fieldName);
			AddField(ordinalPos,tableNum,dataType,fieldName,"");
		}

		/// <summary></summary>
		public void AddFieldFixed(int ordinalPos,DataTypeHL7 dataType,string fixedText) {
			AddField(ordinalPos,"",dataType,"",fixedText);
		}

	}

	///<summary>The items in this enumeration can be freely rearranged without damaging the database.  But can't change spelling or remove existing item.</summary>
	public enum SegmentNameHL7 {
		///<summary>Db Resource Appointment Information</summary>
		AIG,
		///<summary>Location Resource Appointment Information</summary>
		AIL,
		///<summary>Personnel Resource Appointment Information</summary>
		AIP,
		///<summary>Allergy Information</summary>
		AL1,
		///<summary>Appointment Request Information</summary>
		ARQ,
		///<summary>Diagnosis Information</summary>
		DG1,
		///<summary>Event Type</summary>
		EVN,
		///<summary>Financial Transaction Information</summary>
		FT1,
		///<summary>Guarantor Information</summary>
		GT1,
		///<summary>Insurance Information</summary>
		IN1,
		///<summary>Message Acknowledgment</summary>
		MSA,
		///<summary>Message Header</summary>
		MSH,
		///<summary>Next of Kin</summary>
		NK1,
		///<summary>Notes and Comments</summary>
		NTE,
		///<summary>Observations Request</summary>
		OBR,
		///<summary>Observation Related to OBR</summary>
		OBX,
		///<summary>Common Order.  Used in outgoing vaccinations VXUs as well as incoming lab result ORUs.</summary>
		ORC,
		///<summary>Patient Identification</summary>
		PID,
		///<summary>Patient additional demographics</summary>
		PD1,
		///<summary>Procedures</summary>
		PR1,
		///<summary>Problem Detail</summary>
		PRB,
		///<summary>Patient Visit</summary>
		PV1,
		///<summary>Resource Group Segment</summary>
		RGS,
		///<summary>Pharmacy Administration Segment</summary>
		RXA,
		///<summary>Pharmacy/Treatment Route</summary>
		RXR,
		///<summary>Scheduling Activity Information</summary>
		SCH,
		///<summary>Specimen</summary>
		SPM,
		///<summary>Timing/Quantity</summary>
		TQ1,
		///<summary>This can happen for unsupported segments.</summary>
		Unknown,
		///<summary>Encapsulated Data Format</summary>
		ZEF,
		///<summary>Place of Service</summary>
		ZPS,
		///<summary>We use for PDF Data</summary>
		ZX1,
	}
}
