using DataConnectionBase;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>Helper table for HQ only that links additional information to a particular site.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class SiteLink:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long SiteLinkNum;
		///<summary>FK to site.SiteNum</summary>
		public long SiteNum;
		///<summary>Computers that share these first three octets in their default gateway will be associated to this site.</summary>
		public string OctetStart;
		///<summary>FK to employee.EmployeeNum.  This is the employee that is currently the triage coordinator for this site.</summary>
		public long EmployeeNum;
		///<summary>The color that is used to color random HQ controls throughout the program on behalf of the site.</summary>
		[XmlIgnore]
		public Color SiteColor;
		///<summary>The fore color that is used to color random HQ controls throughout the program.</summary>
		[XmlIgnore]
		public Color ForeColor;
		///<summary>The inner color that is used to color random HQ controls throughout the program.</summary>
		[XmlIgnore]
		public Color InnerColor;
		///<summary>The outer color that is used to color random HQ controls throughout the program.</summary>
		[XmlIgnore]
		public Color OuterColor;
		///<summary>JSON serialized dictionary of the connection setting overrides for several databases.
		///Key = ConnectionNames enum, Value = CentralConnection object.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string ConnectionSettingsHQOverrides;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("SiteColor",typeof(int))]
		public int SiteColorXml {
			get {
				return SiteColor.ToArgb();
			}
			set {
				SiteColor=Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ForeColor",typeof(int))]
		public int ForeColorXml {
			get {
				return ForeColor.ToArgb();
			}
			set {
				ForeColor=Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("InnerColor",typeof(int))]
		public int InnerColorXml {
			get {
				return InnerColor.ToArgb();
			}
			set {
				InnerColor=Color.FromArgb(value);
			}
		}

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("OuterColor",typeof(int))]
		public int OuterColorXml {
			get {
				return OuterColor.ToArgb();
			}
			set {
				OuterColor=Color.FromArgb(value);
			}
		}

		///<summary>Returns a strongly typed deserialized version of ConnectionSettingsHQOverrides.</summary>
		[XmlIgnore,JsonIgnore]
		public Dictionary<ConnectionNames,CentralConnection> DictConnectionSettingsHQOverrides {
			get {
				if(ConnectionSettingsHQOverrides==null) {
					return null;
				}
				return JsonConvert.DeserializeObject<Dictionary<ConnectionNames,CentralConnection>>(ConnectionSettingsHQOverrides);
			}
		}

		///<summary>Returns a copy of the sitelink.</summary>
		public SiteLink Copy() {
			return (SiteLink)this.MemberwiseClone();
		}
	}
}

/*
	CREATE TABLE sitelink (
	SiteLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
	SiteNum bigint NOT NULL,
	OctetStart varchar(255) NOT NULL,
	EmployeeNum bigint NOT NULL,
	SiteColor int NOT NULL,
	ForeColor int NOT NULL,
	InnerColor int NOT NULL,
	OuterColor int NOT NULL,
	ConnectionSettingsHQOverrides text NOT NULL,
	INDEX(SiteNum),
	INDEX(EmployeeNum)
	) DEFAULT CHARSET=utf8
*/
