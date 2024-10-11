using System;
using System.Collections;
using System.Drawing;
using System.Xml.Serialization;

namespace OpenDentBusiness{
	///<summary>Holds settings for eClipboard. Each row is attached to one SheetDef or EFormDef. This table might typically only have 3 to 4 rows in it. There could be more for different clinics. This information helps the software decide whether or not to display a sheet or eForm for a specific patient based on certain criteria. Examples include MinAge, MaxAge, and ResubmitInterval. Forms will also end up on the eClipboard if their sheet.ShowInTerminal is non-zero or eForm.Status=ReadyForPatientFill.</summary>
	[Serializable()]
	[CrudTable(IsSynchable=true)]
	public class EClipboardSheetDef:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long EClipboardSheetDefNum;
		///<summary>FK to SheetDef.SheetDefNum. Can be zero if this row is for an eForm.</summary>
		public long SheetDefNum;
		///<summary>FK to clinic.ClinicNum.  0 if no clinic or if default clinic.</summary>
		public long ClinicNum;
		///<summary>Indicates the acceptable amount of time that can pass since the last time the patient has filled this form out. Once this has elapsed, if the EClipboardCreateMissingFormsOnCheckIn pref is turned on, this form will automatically be added to the patient forms to fill out when the patient is checked in.</summary>
		[CrudColumn(SpecialType = CrudSpecialColType.TimeSpanLong),XmlIgnore]
		public TimeSpan ResubmitInterval;
		///<summary>The order in which the patient will be asked to fill out this form.</summary>
		public int ItemOrder;
		///<summary>Determines how forms will be show in eClipboard, fresh copy or last available filled out version.</summary>
		public PrefillStatuses PrefillStatus;
		///<summary>Indicates the minimum age of the patient to be given the form to fill out. If their age is below the minimum limit, they will not be given the form to fill out. A value of -1 means ignore any age requirements.</summary>
		public int MinAge;
		///<summary>Indicates the maximum age of the patient to be given the form to fill out. If their age is over or equal to this maximum limit, they will not be given the form to fill out. A value of -1 means ignore any age requirements.</summary>
		public int MaxAge;
		///<summary>Comma delimited list of sheetdef nums to ignore. This can only be set if the preFillStatus is set to Once. These sheetDefs are ignored until this sheet is filled out. For example, an office may have a sheet for new patients that is only filled out once. Once the new patient has filled out this sheet, these sheetDefs will no longer be ignored when the patient checks in again. Not used in eForms.</summary>//jordan I don't understand why this field is here.
		[CrudColumn(SpecialType=CrudSpecialColType.IsText)]
		public string IgnoreSheetDefNums;
		///<summary>For both Sheets and eForms. Holds the previous revision number that marks what def revision number we expect to have a submission for from the patient. This value can be incremented when changes are made to a linked sheet or eForm def, but it is not always updated. This should hold the last revision number that was updated for this eClipboardSheetDef. If a patient has a form filled out that has a revision ID that matches this or is greater (this field will be 0 by default, or they filled out a more up to date form manually in the office and not through eClipboard), then this form will be filtered out if set to PrefillStatuses.Once when we determine which forms load for the patient in eClipboard. If this value is higher than the RevID of the last form the patient filled out, we consider this eClipboardSheetDef to be updated and the patient will need to fill out the form again when set to PrefillStatuses.Once.</summary>
		public long PrefillStatusOverride;
		///<summary>FK to EFormDef.EFormDefNum. Can be zero if this row is for a sheet. Only for custom eForms.</summary>
		public long EFormDefNum;

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