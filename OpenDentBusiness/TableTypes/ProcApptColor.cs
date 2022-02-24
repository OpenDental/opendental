using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>An individual procedure code color range.</summary>
	[Serializable]
	public class ProcApptColor : TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long ProcApptColorNum;
		///<summary>Procedure code range defined by user.  Includes commas and dashes, but no spaces.  The codes need not be valid since they are ranges.</summary>
		public string CodeRange;
		///<summary>Adds most recent completed date to ProcsColored</summary>
		public bool ShowPreviousDate;
		///<summary>Color that shows in appointments</summary>
		[XmlIgnore]
		public Color ColorText;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ColorText",typeof(int))]
		public int ColorTextXml {
			get {
				return ColorText.ToArgb();
			}
			set {
				ColorText=Color.FromArgb(value);
			}
		}

		public ProcApptColor Copy() {
			return (ProcApptColor)this.MemberwiseClone();
		}	
	}
}

