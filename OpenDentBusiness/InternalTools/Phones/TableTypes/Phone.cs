using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness {
	///<summary>This table is not part of the general release.  User would have to add it manually.  All schema changes are done directly on our live database as needed.  This is the live table that is updated very frequently to show status of each phone.</summary>
	[Serializable]
	[CrudTable(IsMissingInGeneral=true)]
	public class Phone:TableBase {
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long PhoneNum;
		///<summary></summary>
		public int Extension;
		///<summary></summary>
		public string EmployeeName;
		///<summary>This enum is stored in the db as a string, so it needs special handling.  In phoneTrackingServer initialize, this value is pulled from employee.ClockStatus as Home, Lunch, Break, or Working(which gets converted to Available).  After that, the phone server uses those 4 in addition to WrapUp, Off, Training, TeamAssist, OfflineAssist, Backup, and None(which is displayed as an empty string).  The main program sets Unavailable sometimes, and pulls from employee.ClockStatus sometimes.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.EnumAsString)]
		public ClockStatusEnum ClockStatus;
		///<summary>Either blank or 'In use'</summary>
		public string Description;
		///<summary></summary>
		[XmlIgnore]
		public Color ColorBar;
		///<summary></summary>
		[XmlIgnore]
		public Color ColorText;
		///<summary>FK to employee.EmployeeNum.</summary>
		public long EmployeeNum;
		///<summary>The phone number or name of customer.</summary>
		public string CustomerNumber;
		///<summary>Blank or 'in' or 'out'.</summary>
		public string InOrOut;
		///<summary>FK to patient.PatNum.  The customer.</summary>
		public long PatNum;
		///<summary>The date/time that the phonecall started.  Used to calculate how long user has been on phone.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime DateTimeStart;
		///<summary>Always set to the phone number of the caller.</summary>
		public string CustomerNumberRaw;
		///<summary>A copy of DateTimeStart made when a call has ended.  Gets set to 0001-01-01 after the 30 second wrap up thread has run.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.DateT)]
		public DateTime LastCallTimeStart;
		///<summary>Enum:AsteriskRingGroups 0=all, 1=none, 2=backup.  This represents the active ring group for this extension.  This is different (and will often times differ) from the phoneempdefault.RingGroups which is just a default value for the extension.</summary>
		public AsteriskQueues RingGroups;
		///<summary>Used with proximity sensors. Set from ProximityOD/WebcamOD.</summary>
		public bool IsProximal;
		///<summary>.</summary>
		public DateTime DateTimeNeedsHelpStart;
		///<summary>.</summary>
		public DateTime DateTProximal;
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! WARNING - SCHEMA CHANGES !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		//  Changes to this table must also be reflected in:
		//    - PhoneTrackingServer.AsteriskPhones.GetExtensionsFromPhoneTableAndReconstructIfNeeded()

		///<summary>Used with proximity sensors. Set from ProximityOD/WebcamOD.</summary>
		public bool IsProxVisible {
			get {
				return IsProximal
					&& DateTProximal.AddSeconds(60)>DateTime.Now
					&& ClockStatus!=ClockStatusEnum.Home
					&& ClockStatus!=ClockStatusEnum.None
					&& ClockStatus!=ClockStatusEnum.Break
					&& ClockStatus!=ClockStatusEnum.Off;
			}
		}

		///<summary>Only used for serialization purposes.</summary>
		[XmlElement("ColorBar",typeof(int))]
		public int ColorBarXml {
			get {
				return ColorBar.ToArgb();
			}
			set {
				ColorBar=Color.FromArgb(value);
			}
		}

		///<summary>Only used for serialization purposes.</summary>
		[XmlElement("ColorText",typeof(int))]
		public int ColorTextXml {
			get {
				return ColorText.ToArgb();
			}
			set {
				ColorText=Color.FromArgb(value);
			}
		}

		public Phone Copy() {
			return (Phone)this.MemberwiseClone();
		}

		public override string ToString() {
			return this.EmployeeName + " - " + this.Extension;
		}
	}

	///<summary></summary>
	public enum ClockStatusEnum {
		///<summary>This shows in the UI as blank.</summary>
		None,
		///<summary></summary>
		Home,
		///<summary></summary>
		Lunch,
		///<summary></summary>
		Break,
		///<summary></summary>
		Available,
		///<summary></summary>
		WrapUp,
		///<summary></summary>
		Off,
		///<summary></summary>
		Training,
		///<summary></summary>
		TeamAssist,
		///<summary></summary>
		OfflineAssist,
		///<summary></summary>
		Backup,
		///<summary></summary>
		Unavailable,
		///<summary></summary>
		NeedsHelp,
		///<summary></summary>
		HelpOnTheWay,
		///<summary>Triage Coordinatior or Responder.</summary>
		[Description("TC/Responder")]
		TCResponder,
	}

	
}




