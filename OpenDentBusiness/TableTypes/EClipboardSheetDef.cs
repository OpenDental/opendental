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
		///<summary>Comma delimited list of sheetdef nums to ignore.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string IgnoreSheetDefNums;
		///<summary>Holds the previous revision number that marks what sheet def revision number we expect to have a submission for from the patient.
		///This value can be incremented when changes are made to a linked sheet definition, but it is not always updated. This should hold the last revision number 
		///that was updated for this eClipboardSheetDef. If a patient has a Sheet filled out that has a revision ID that matches this or is greater 
		///(this field will be 0 by default, or they filled out a more up to date form manually in the office and not through eClipboard), 
		///then this form will be filtered out if set to PrefillStatuses.Once when we determine which forms load for the patient in eClipboard. 
		///If this value is higher than the RevID of the last sheet the patient filled out, we consider this eClipboardSheetDef to be updated and the patient will need 
		///to fill out the form again when set to PrefillStatuses.Once.
		/// </summary>
		public long PrefillStatusOverride;

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
	///<summary>Prefill means we pull last filled out sheet where RevID for the sheet matches the current SheetDef RevID</summary>
	PreFill,
	///<summary>Once means we only show this sheet once, resubmit is 0</summary>
	Once
};