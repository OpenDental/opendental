using System;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness.Mobile {

	///<summary>One username/password for one customer.</summary>
	[Serializable]
	[CrudTable(IsMobile=true)]
	public class Providerm:TableBase {
		///<summary>Primary key 1.</summary>
		[CrudColumn(IsPriKeyMobile1=true)]
		public long CustomerNum;
		///<summary>Primary key 2.</summary>
		[CrudColumn(IsPriKeyMobile2=true)]
		public long ProvNum;
		///<summary>Abbreviation.</summary>
		public string Abbr;
		///<summary>True if hygienist.</summary>
		public bool IsSecondary;
		///<summary>Color that shows in appointments</summary>
		[XmlIgnore]
		public Color ProvColor;

		///<summary></summary>
		public Providerm Copy() {
			return (Providerm)this.MemberwiseClone();
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ProvColor",typeof(int))]
		public int ProvColorXml {
			get {
				return ProvColor.ToArgb();
			}
			set {
				ProvColor = Color.FromArgb(value);
			}
		}


	}




}