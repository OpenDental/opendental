using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {

	///<summary>This defines the items that will be available for clicking when composing a manual message.
	///Also, these are referred to in the button definitions as a sequence of elements.</summary>
	[Serializable]
	public class SigElementDef:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SigElementDefNum;
		///<summary>If this element should cause a button to light up, this would be the row.  0 means none.</summary>
		public byte LightRow;
		///<summary>If a light row is set, this is the color it will turn when triggered.  Ack sets it back to white.
		///Note that color and row can be in two separate elements of the same signal.</summary>
		[XmlIgnore]
		public Color LightColor;
		///<summary>Enum:SignalElementType  0=User,1=Extra,2=Message.</summary>
		public SignalElementType SigElementType;
		///<summary>The text that shows for the element, like the user name or the two word message.  No long text is stored here.</summary>
		public string SigText;
		///<summary>The sound to play for this element.  Wav file stored in the database in string format until "played".  If empty string, then no sound.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string Sound;
		///<summary>The order of this element within the list of the same type.</summary>
		public int ItemOrder;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("LightColor",typeof(int))]
		public int LightColorXml {
			get {
				return LightColor.ToArgb();
			}
			set {
				LightColor=Color.FromArgb(value);
			}
		}

		///<summary></summary>
		public SigElementDef Copy() {
			return (SigElementDef)this.MemberwiseClone();
		}

		
		
	}

		



		
	

	

	


}










