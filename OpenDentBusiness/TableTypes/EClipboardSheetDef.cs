using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>Used in the accounting section in chart of accounts.  Not related to patient accounts in any way.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class EClipboardSheetDef:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EClipboardSheetDefNum;
		///<summary>FK to SheetDef.SheetDefNum.</summary>
		public long SheetDefNum;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic or if default clinic.</summary>
		public long ClinicNum;
		///<summary>Indicates the acceptable amount of time that can pass since the last time the patient has filled this sheet out. Once this has
		///elapsed, if the EClipboardCreateMissingFormsOnCheckIn pref is turned on, this sheet will automatically be added to the patient
		///sheets to fill out when the patient is checked-in.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan ResubmitInterval;
		///<summary>The order in which the patient will be asked to fill out this sheet.</summary>
		public int ItemOrder;
		///<summary>Determines how sheets will be show in eClipboard, fresh copy or last available filled out version.</summary>
		public PrefillStatuses PrefillStatus;
		///<summary>Indicates the minimum age of the patient to be given the sheet to fill out. If their age is below the minimum limit, 
		///they will not be given the form to fill out. A value of -1 means ignore any age requirements.</summary>
		public int MinAge;
		///<summary>Indicates the maximum age of the patient to be given the sheet to fill out. If their age is over or equal to this maximum 
		///limit, they will not be given the form to fill out. A value of -1 means ignore any age requirements.</summary>
		public int MaxAge;

		///<summary>Used only for serialization purposes</summary>
		[XmlElement("ResubmitInterval",typeof(long))]
		public long ResubmitIntervalXml {
			get {
				return ResubmitInterval.Ticks;
			}
			set {
				ResubmitInterval=TimeSpan.FromTicks(value);
			}
		}

		///<summary></summary>
		public EClipboardSheetDef Clone() {
			return (EClipboardSheetDef)this.MemberwiseClone();
		}
	}
}

///<summary></summary>
public enum PrefillStatuses { 
	///<summary></summary>
	New, 
	///<summary>Update means we pull last filled out sheet where RevID for the sheet matches the current SheetDef RevID</summary>
	Update,
	///<summary>Once means we only show this sheet once, resubmit is 0</summary>
	Once
};