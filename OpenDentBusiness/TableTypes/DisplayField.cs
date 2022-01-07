using System;
using System.Collections;
using System.ComponentModel;

namespace OpenDentBusiness{
	
	///<summary>Allows customization of which fields display in various lists and grids.  For now, the only grid is ProgressNotes.  Will also eventually let users set column widths and translate titles.  For now, the selections are the same for all computers.</summary>
	[Serializable]
	public class DisplayField:TableBase{
		///<summary>Primary key.</summary>
		[CrudColumn(IsPriKey=true)]
		public long DisplayFieldNum;
		///<summary>This is the internal name that OD uses to identify the field within this category.  This will be the default description if the user doesn't specify an alternate.  For Ortho chart, this column will be blank, "Signature", or "Provider".</summary>
		public string InternalName;
		///<summary>Order to display in the grid or list. Every entry must have a unique itemorder.</summary>
		public int ItemOrder;
		///<summary>Optional alternate description to display for field.  Can be in another language.  For Ortho, this is the 'key', since InternalName is blank.</summary>
		public string Description;
		///<summary>For grid columns, this lets user override the column width.  Especially useful for foreign languages.</summary>
		public int ColumnWidth;
		///<summary>Enum:DisplayFieldCategory If category is 0, then this is attached to a ChartView.</summary>
		public DisplayFieldCategory Category;
		///<summary>FK to chartview.ChartViewNum. 0 if attached to a category.</summary>
		public long ChartViewNum;
		///<summary>Newline delimited string which contains the selectable options in combo box dropdowns.  Specifically for the Ortho chart.</summary>
		[CrudColumn(SpecialType=CrudSpecialColType.TextIsClob)]
		public string PickList;
		///<summary>Only used in Ortho. Because ortho chart display fields utilize the InternalName field for Signature and Provider indicators, this field is here to override description. Some users want to use different fields but use the same description for multiple tabs. Example: The display field of WeightWeekly shows as "Weight" and in another tab the field for WeightMonthly can also show as "Weight".</summary>
		public string DescriptionOverride;


		public DisplayField(){
			
		}

		public DisplayField(string internalName,int columnWidth,DisplayFieldCategory category){
			this.InternalName=internalName;
			//this.Description=description;
			this.ColumnWidth=columnWidth;
			this.Description="";
			this.Category=category;
		}

		///<summary>Returns a copy.</summary>
		public DisplayField Copy() {
			return (DisplayField)this.MemberwiseClone();
		}

		public override string ToString(){
			return InternalName+", "+ColumnWidth.ToString();
		}

	}

	///<summary></summary>
	public enum DisplayFieldCategory {
		///<summary>0- This indicates progress notes.</summary>
		None,
		//<summary>0</summary>
		//ProgressNotes,
		///<summary>1</summary>
		[Description("Patient Select")]
		PatientSelect,
		///<summary>2- Family module.</summary>
		[Description("Patient Information")]
		PatientInformation,
		///<summary>3</summary>
		[Description("Account Module")]
		AccountModule,
		///<summary>4</summary>
		[Description("Recall List")]
		RecallList,
		///<summary>5</summary>
		[Description("Chart Patient Information")]
		ChartPatientInformation,
		///<summary>6</summary>
		[Description("Procedure Group Note")]
		ProcedureGroupNote,
		///<summary>7</summary>
		[Description("Treatment Plan Module")]
		TreatmentPlanModule,
		///<summary>8</summary>
		[Description("Ortho Chart")]
		OrthoChart,
		///<summary>9</summary>
		[Description("Appointment Bubble")]
		AppointmentBubble,
		///<summary>10- Account module patient information</summary>
		[Description("Account Patient Information")]
		AccountPatientInformation,
		///<summary>11</summary>
		[Description("Statement Main Grid")]
		StatementMainGrid,
		///<summary>12</summary>
		[Description("Family Recall Grid")]
		FamilyRecallGrid,
		///<summary>13</summary>
		[Description("Appointment Edit")]
		AppointmentEdit,
		///<summary>14</summary>
		[Description("Planned Appointment Edit")]
		PlannedAppointmentEdit,
		///<summary>15</summary>
		[Description("Outstanding Ins Report")]
		OutstandingInsReport,
		///<summary>16</summary>
		[Description("Patient Search")]
		[PermissionAttribute(IsCEMT=true)]
		CEMTSearchPatients,
		///<summary>17 - A/R Manager Sent Grid</summary>
		[Description("A/R Manager Sent Grid")]
		ArManagerSentGrid,
		///<summary>18 - A/R Manager Unsent Grid</summary>
		[Description("A/R Manager Unsent Grid")]
		ArManagerUnsentGrid,
		///<summary>19 - A/R Manager Excluded Grid</summary>
		[Description("A/R Manager Excluded Grid")]
		ArManagerExcludedGrid,
	}
	

	///<summary>Permissions-related attributes.</summary>
	public class PermissionAttribute:Attribute {
		private bool _isCEMT=false;
		///<summary></summary>
		public bool IsCEMT {
			get {
				return _isCEMT;
			}
			set {
				_isCEMT=value;
			}
		}
	}

	


}









