using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CodeBase;

namespace OpenDentBusiness.HL7 {
	///<summary>A 'row' in the message.  Composed of fields</summary>
	public class SegmentHL7 {
		public List<FieldHL7> Fields;
		///<summary>The name</summary>
		public SegmentNameHL7 Name;
		///<summary>The original full text of the segment.</summary>
		private string _fullText;
		///<summary>Delimiter order: component separator, repetition separator, escape character, subcomponent separator.
		///<para>Defaults: ^ component, ~ repetition, \ escape character, &amp; subcomponent.</para></summary>
		private char[] _delimiters;
		public int SequenceNumIndex=1;//the index for the field of the sequence num for repeating segs, usually the 2nd field in the seg, but set in the def

		///<summary>Only use this constructor when generating a message instead of parsing a message.  Uses the default message delimiters.</summary>
		internal SegmentHL7(SegmentNameHL7 name) {
			InitializeVariables(name,new char[] { '^','~','\\','&' });//default delimiters in this order:  component separator, repetition separator, escape character, subcomponent separator
		}

		///<summary>Only use this constructor when generating a message instead of parsing a message.  Uses the delimiters provided, retrieved from the enabled HL7 def if exists.</summary>
		internal SegmentHL7(SegmentNameHL7 name,char[] delimiters) {
			InitializeVariables(name,delimiters);
		}

		private void InitializeVariables(SegmentNameHL7 name,char[] delimiters) {
			_fullText="";
			Name=name;
			_delimiters=delimiters;
			Fields=new List<FieldHL7>();
			//remember that the "field quantity" is one more than the last index, because 0-based.
			//All fields are initially added with just one component
			//This can all probably be removed now since we add fields dynamically as needed:
			//if(name==SegmentNameHL7.MSA) {
			//	AddFields(3);
			//}
			//if(name==SegmentNameHL7.MSH) {
			//	AddFields(12);
			//}
			//if(name==SegmentNameHL7.EVN) {
			//	AddFields(4);
			//}
			//if(name==SegmentNameHL7.PID) {
			//	AddFields(23);
			//}
			//if(name==SegmentNameHL7.PV1) {
			//	AddFields(51);
			//}
			//if(name==SegmentNameHL7.FT1) {
			//	AddFields(27);
			//}
			//if(name==SegmentNameHL7.DG1) {
			//	AddFields(5);
			//}
			//if(name==SegmentNameHL7.ZX1) {
			//	AddFields(6);
			//}
		}

		private void AddFields(int quantity) {
			FieldHL7 field;
			for(int i=0;i<quantity;i++) {
				field=new FieldHL7(_delimiters);
				Fields.Add(field);
			}
		}

		///<summary>Use this constructor when we have a message to parse.  Uses the default message delimiters.</summary>
		public SegmentHL7(string rowtext) {
			_delimiters=new char[] { '^','~','\\','&' };
			FullText=rowtext;
		}

		///<summary>Use this constructor when we have a message to parse.  Uses the delimiters provided, retrieved from the enabled HL7 def if exists.</summary>
		public SegmentHL7(string rowtext,char[] delimiters) {
			_delimiters=delimiters;
			FullText=rowtext;
		}

		public override string ToString() {
			return FullText;
		}

		///<summary>Setting the FullText resets all the child fields.</summary>
		public string FullText {
			get {
				return _fullText;
			}
			set {
				_fullText=value??"";
				Fields=new List<FieldHL7>();
				if(string.IsNullOrEmpty(value)) {
					return;
				}
				string fieldSeparatorEscaped="\\";//default escape character is \
				if(_delimiters!=null && _delimiters.Length>2) {
					fieldSeparatorEscaped=_delimiters[2].ToString();//user may have a different escape char defined (totally not recommended, but possible)
				}
				fieldSeparatorEscaped+="|";//'\|' is a literal pipe, not a field separator
				string[] fields=_fullText.Replace(fieldSeparatorEscaped,"&124;")//replace \| with &124; (ASCII code for | is 124)
					.Split(new string[] { "|" },StringSplitOptions.None)//split by any remaining |'s
					.Select(x => x.Replace("&124;",fieldSeparatorEscaped)).ToArray();//put back \|'s
				FieldHL7 field;
				for(int i=0;i<fields.Length;i++) {
					field=new FieldHL7(fields[i],_delimiters);
					Fields.Add(field);
				}
				try {
					Name=(SegmentNameHL7)Enum.Parse(typeof(SegmentNameHL7),Fields[0].FullText);
				}
				catch {
					Name=SegmentNameHL7.Unknown;
				}
			}
		}

		///<summary>Read-only property.  This is the sequence number of this segment within the HL7 message.  Repeatable segments have a sequence number
		///identifying which repetition the segment is and is used to order the repeating segments.  If the value in the field with index SequenceNumPos
		///cannot be parsed into an int, or if the segment does not have an index SequenceNumPos field, the SequenceNum returned will be -1.</summary>
		public int SequenceNum {
			get {
				int retval=-1;
				try {
					retval=Int32.Parse(Fields[SequenceNumIndex].FullText);
				}
				catch(Exception ex) {
					ex.DoNothing();
					//do nothing, the sequence num will remain -1
				}
				return retval;
			}
		}

		///<summary></summary>
		public string GetFieldFullText(int indexPos) {
			if(indexPos > Fields.Count-1) {
				return "";
			}
			return Fields[indexPos].FullText;
		}

		///<summary>Really just a handy shortcut.  Identical to getting component 0 or to GetFieldFullText if there is only one component.
		///This will return an empty string if fieldIndex is greater than the number of fields in the segment.</summary>
		public string GetFieldComponent(int fieldIndex) {
			return GetFieldComponent(fieldIndex,0);
		}

		public string GetFieldComponent(int fieldIndex,int componentIndex) {
			if(fieldIndex > Fields.Count-1) {
				return "";
			}
			return Fields[fieldIndex].GetComponentVal(componentIndex);
		}

		///<summary></summary>
		public FieldHL7 GetField(int indexPos) {
			if(indexPos > Fields.Count-1) {
				return null;
			}
			return Fields[indexPos];
		}

		///<summary>Pass in one val to set the whole field.  Pass in multiple vals to set multiple components.  It also sets the fullText of the segment.</summary>
		public void SetField(int fieldIndex,params string[] vals) {
			if(fieldIndex>Fields.Count-1) {
				AddFields(fieldIndex-(Fields.Count-1));
			}
			Fields[fieldIndex].SetVals(vals);
			//kind of repetitive to recalc the whole segment again here, but it goes very fast.
			RefreshFullText();
		}

		private void RefreshFullText() {
			_fullText="";
			for(int i=0;i<Fields.Count;i++) {
				if(i>0) {
					_fullText+="|";
				}
				_fullText+=Fields[i].FullText;
			}
		}

		///<summary>Not often used.  If RepeatField() is called before SetField(), then the first field instance will be blank.
		///Some HL7 fields are allowed to "repeat" multiple times.
		///For example, in immunization messaging export (VXU messages), PID-3 repeats twice, once for patient ID and once for SSN.</summary>
		public void RepeatField(int fieldIndex,params string[] vals) {
			if(fieldIndex>Fields.Count-1) {//The field does not exist yet.
				AddFields(fieldIndex-(Fields.Count-1));
			}
			//The field exists and has already been set.  Repeat the field with the new values.
			Fields[fieldIndex].RepeatVals(vals);
			//kind of repetitive to recalc the whole segment again here, but it goes very fast.
			RefreshFullText();
		}

		///<summary>Sets the field if not yet set, otherwise repeats the field with the new values.
		///Not often used.  Some HL7 fields are allowed to "repeat" multiple times.
		///For example, in immunization messaging export (VXU messages), PID-3 repeats twice, once for patient ID and once for SSN.</summary>
		public void SetOrRepeatField(int fieldIndex,params string[] vals) {
			if(fieldIndex>Fields.Count-1) {//The field does not exist yet.
				AddFields(fieldIndex-(Fields.Count-1));
			}
			if(Fields[fieldIndex].FullText=="") {//The field exists and has not been set yet.
				SetField(fieldIndex,vals);
				return;
			}
			//The field exists and has already been set.  Repeat the field with the new values.
			Fields[fieldIndex].RepeatVals(vals);
			//kind of repetitive to recalc the whole segment again here, but it goes very fast.
			RefreshFullText();
		}

		///<summary>yyyyMMdd[HHmm[ss]].  If not in that format, it returns minVal.</summary>
		public DateTime GetDateTime(int fieldIndex) {
			if(fieldIndex > Fields.Count-1) {
				return DateTime.MinValue;
			}
			string str=Fields[fieldIndex].FullText.Trim();//trailing space was causing problems.
			try {
				if(str.Length==8) {
					return DateTime.ParseExact(str,"yyyyMMdd",DateTimeFormatInfo.InvariantInfo);
				}
				if(str.Length==12) {
					return DateTime.ParseExact(str,"yyyyMMddHHmm",DateTimeFormatInfo.InvariantInfo);
				}
				if(str.Length==14) {
					return DateTime.ParseExact(str,"yyyyMMddHHmmss",DateTimeFormatInfo.InvariantInfo);
				}
			}
			catch {
				return DateTime.MinValue;
			}
			return DateTime.MinValue;
		}

	}

	
}
