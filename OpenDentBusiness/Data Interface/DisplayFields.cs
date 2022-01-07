using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDentBusiness {
	public class DisplayFields {
		#region CachePattern

		private class DisplayFieldCache : CacheListAbs<DisplayField> {
			protected override List<DisplayField> GetCacheFromDb() {
				string command="SELECT * FROM displayfield ORDER BY ItemOrder";
				return Crud.DisplayFieldCrud.SelectMany(command);
			}
			protected override List<DisplayField> TableToList(DataTable table) {
				return Crud.DisplayFieldCrud.TableToList(table);
			}
			protected override DisplayField Copy(DisplayField displayField) {
				return displayField.Copy();
			}
			protected override DataTable ListToTable(List<DisplayField> listDisplayFields) {
				return Crud.DisplayFieldCrud.ListToTable(listDisplayFields,"DisplayField");
			}
			protected override void FillCacheIfNeeded() {
				DisplayFields.GetTableFromCache(false);
			}
		}

		///<summary>The object that accesses the cache in a thread-safe manner.</summary>
		private static DisplayFieldCache _displayFieldCache=new DisplayFieldCache();

		private static List<DisplayField> GetWhere(Predicate<DisplayField> match,bool isShort=false) {
			return _displayFieldCache.GetWhere(match,isShort);
		}

		///<summary>Refreshes the cache and returns it as a DataTable. This will refresh the ClientWeb's cache and the ServerWeb's cache.</summary>
		public static DataTable RefreshCache() {
			return GetTableFromCache(true);
		}

		///<summary>Fills the local cache with the passed in DataTable.</summary>
		public static void FillCacheFromTable(DataTable table) {
			_displayFieldCache.FillCacheFromTable(table);
		}

		///<summary>Always refreshes the ClientWeb's cache.</summary>
		public static DataTable GetTableFromCache(bool doRefreshCache) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_displayFieldCache.FillCacheFromTable(table);
				return table;
			}
			return _displayFieldCache.GetTableFromCache(doRefreshCache);
		}

		#endregion
		
		///<summary></summary>
		public static long Insert(DisplayField field) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				field.DisplayFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),field);
				return field.DisplayFieldNum;
			}
			return Crud.DisplayFieldCrud.Insert(field);
		}

		///<summary></summary>
		public static void Update(DisplayField field) {			
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),field);
				return;
			}
			Crud.DisplayFieldCrud.Update(field);
		}

		///<summary></summary>
		public static void Delete(long displayFieldNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),displayFieldNum);
				return;
			}
			string command="DELETE FROM displayfield WHERE DisplayFieldNum = "+POut.Long(displayFieldNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForChartView(long chartViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chartViewNum);
				return;
			}
			string command="DELETE FROM displayfield WHERE ChartViewNum = "+POut.Long(chartViewNum);
			Db.NonQ(command);
		}

		///<Summary>Returns an ordered list for just one category.  Do not use with None, or it will malfunction.  These are display fields that the user has entered, which are stored in the db, and then are pulled into the cache.  Categories with no display fields will return the default list.</Summary>
		public static List<DisplayField> GetForCategory(DisplayFieldCategory category){
			//No need to check RemotingRole; no call to db.
			List<DisplayField> retVal=GetWhere(x => x.Category==category);
			if(retVal.Count==0) {//default
				return DisplayFields.GetDefaultList(category);
			}
			return retVal;
		}

		///<Summary>Returns an ordered list for just one chart view</Summary>
		public static List<DisplayField> GetForChartView(long ChartViewNum) {
			//No need to check RemotingRole; no call to db.
			List<DisplayField> retVal=GetWhere(x => x.ChartViewNum==ChartViewNum && x.Category==DisplayFieldCategory.None);
			if(retVal.Count==0) {//default
				return DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			}
			return retVal;
		}

		public static List<DisplayField> GetDefaultList(DisplayFieldCategory category){
			//No need to check RemotingRole; no call to db.
			List<DisplayField> list=new List<DisplayField>();
			switch (category) {
				#region 'None'
				case DisplayFieldCategory.None:
					list.Add(new DisplayField("Date",67,category));
					//list.Add(new DisplayField("Time",40));
					list.Add(new DisplayField("Th",27,category));
					list.Add(new DisplayField("Surf",40,category));
					list.Add(new DisplayField("Dx",28,category));
					list.Add(new DisplayField("Description",218,category));
					list.Add(new DisplayField("Stat",25,category));
					list.Add(new DisplayField("Prov",42,category));
					list.Add(new DisplayField("Amount",48,category));
					list.Add(new DisplayField("Proc Code",62,category));
					list.Add(new DisplayField("User",62,category));
					list.Add(new DisplayField("Signed",55,category));
					//list.Add(new DisplayField("Priority",65,category));
					//list.Add(new DisplayField("Date TP",67,category));
					//list.Add(new DisplayField("Date Entry",67,category));
					//list.Add(new DisplayField("Prognosis",60,category));
					//list.Add(new DisplayField("Length",40,category));
					//list.Add(new DisplayField("Abbr",50,category));
					//list.Add(new DisplayField("Locked",50,category));
					//list.Add(new DisplayField("HL7 Sent",60,category));
					//if(PrefC.HasClinicsEnabled) {
					//	list.Add(new DisplayField("Clinic",90,category));
					//}
					//if(Programs.UsingOrion){
						//list.Add(new DisplayField("DPC",33,category));
						//list.Add(new DisplayField("Schedule By",72,category));
						//list.Add(new DisplayField("Stop Clock",67,category));
						//list.Add(new DisplayField("Stat 2",36,category));
						//list.Add(new DisplayField("On Call",45,category));
						//list.Add(new DisplayField("Effective Comm",90,category));
						//list.Add(new DisplayField("End Time",56,category));
						//list.Add(new DisplayField("Quadrant",55,category));
						//list.Add(new DisplayField("DPCpost",52,category));
					//}
					break;
				#endregion 'None'
				#region PatientSelect
				case DisplayFieldCategory.PatientSelect:
					list.Add(new DisplayField("LastName",75,category));
					list.Add(new DisplayField("First Name",75,category));
					//list.Add(new DisplayField("MI",25,category));
					list.Add(new DisplayField("Pref Name",60,category));
					list.Add(new DisplayField("Age",30,category));
					list.Add(new DisplayField("SSN",70,category));
					list.Add(new DisplayField("Hm Phone",90,category));
					list.Add(new DisplayField("Wk Phone",90,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {//if for OD HQ
						//list.Add(new DisplayField("OtherPhone",90,category));
						//list.Add(new DisplayField("Country",90,category));
						//list.Add(new DisplayField("RegKey",150,category));
					}
					list.Add(new DisplayField("PatNum",80,category));
					//list.Add(new DisplayField("ChartNum",60,category));
					list.Add(new DisplayField("Address",100,category));
					list.Add(new DisplayField("Status",65,category));
					//list.Add(new DisplayField("Bill Type",90,category));
					//list.Add(new DisplayField("City",80,category));
					//list.Add(new DisplayField("State",55,category));
					//list.Add(new DisplayField("Pri Prov",85,category));
					//list.Add(new DisplayField("Birthdate",70,category));
					//list.Add(new DisplayField("Site",90,category));
					//list.Add(new DisplayField("Email",90,category));
					//list.Add(new DisplayField("Clinic",90,category));
					//list.Add(new DisplayField("Wireless Ph",90,category));
					//list.Add(new DisplayField("Sec Prov",85,category));
					//list.Add(new DisplayField("LastVisit",70,category));
					//list.Add(new DisplayField("NextVisit",70,category));
					//list.Add(new DisplayField("Invoice Number",90,category));
					//list.Add(new DisplayField("Specialty",90,category));
					break;
				#endregion PatientSelect
				#region PatientInformation
				case DisplayFieldCategory.PatientInformation:
					list.Add(new DisplayField("Last",0,category));
					list.Add(new DisplayField("First",0,category));
					list.Add(new DisplayField("Middle",0,category));
					list.Add(new DisplayField("Preferred",0,category));
					list.Add(new DisplayField("Title",0,category));
					list.Add(new DisplayField("Salutation",0,category));
					list.Add(new DisplayField("Status",0,category));
					list.Add(new DisplayField("Gender",0,category));
					list.Add(new DisplayField("Position",0,category));
					list.Add(new DisplayField("Birthdate",0,category));
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("SS#",0,category));
					list.Add(new DisplayField("Address",0,category));
					list.Add(new DisplayField("Address2",0,category));
					list.Add(new DisplayField("City",0,category));
					list.Add(new DisplayField("State",0,category));
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
						list.Add(new DisplayField("Country",0,category));
					}
					if(PrefC.IsODHQ) {
						list.Add(new DisplayField("Tax Address",0,category));
					}
					list.Add(new DisplayField("Zip",0,category));
					list.Add(new DisplayField("Hm Phone",0,category));
					list.Add(new DisplayField("Wk Phone",0,category));
					list.Add(new DisplayField("Wireless Ph",0,category));
					list.Add(new DisplayField("E-mail",0,category));
					list.Add(new DisplayField("Contact Method",0,category));
					list.Add(new DisplayField("ABC0",0,category));
					//list.Add(new DisplayField("Chart Num",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					//list.Add(new DisplayField("Ward",0,category));
					//list.Add(new DisplayField("AdmitDate",0,category));
					list.Add(new DisplayField("Primary Provider",0,category));
					list.Add(new DisplayField("Sec. Provider",0,category));
					list.Add(new DisplayField("Payor Types",0,category));
					list.Add(new DisplayField("Language",0,category));
					//list.Add(new DisplayField("Clinic",0,category));
					//list.Add(new DisplayField("ResponsParty",0,category));
					list.Add(new DisplayField("Referrals",0,category));
					list.Add(new DisplayField("Addr/Ph Note",0,category));
					list.Add(new DisplayField("PatFields",0,category));
					//list.Add(new DisplayField("Guardians",0,category));
					//list.Add(new DisplayField("Arrive Early",0,category));
					//list.Add(new DisplayField("Super Head",0,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						list.Add(new DisplayField("References",0,category));
					}
					list.Add(new DisplayField("Pat Restrictions",0,category));
					list.Add(new DisplayField("ICE Name",0,category));
					list.Add(new DisplayField("ICE Phone",0,category));
					break;
				#endregion PatientInformation
				#region AccountModule
				case DisplayFieldCategory.AccountModule:
					list.Add(new DisplayField("Date",65,category));
					list.Add(new DisplayField("Patient",100,category));
					list.Add(new DisplayField("Prov",40,category));
					//list.Add(new DisplayField("Clinic",50,category));
					list.Add(new DisplayField("Code",46,category));
					list.Add(new DisplayField("Tth",26,category));
					list.Add(new DisplayField("Description",270,category));
					list.Add(new DisplayField("Charges",60,category));
					list.Add(new DisplayField("Credits",60,category));
					list.Add(new DisplayField("Balance",60,category));
					//list.Add(new DisplayField("Signed",60,category));
					//list.Add(new DisplayField("Abbr",110,category));
					break;
				#endregion AccountModule
				#region RecallList
				case DisplayFieldCategory.RecallList:
					list.Add(new DisplayField("Due Date",75,category));
					list.Add(new DisplayField("Patient",120,category));
					list.Add(new DisplayField("Age",30,category));
					list.Add(new DisplayField("Type",60,category));
					list.Add(new DisplayField("Interval",50,category));
					list.Add(new DisplayField("#Remind",55,category));
					list.Add(new DisplayField("LastRemind",75,category));
					list.Add(new DisplayField("Contact",120,category));
					list.Add(new DisplayField("Status",130,category));
					list.Add(new DisplayField("Note",215,category));
					//list.Add(new DisplayField("BillingType",100,category));
					//list.Add(new DisplayField("WebSched",100,category));
					break;
				#endregion RecallList
				#region ChartPatientInformation
				case DisplayFieldCategory.ChartPatientInformation:
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("ABC0",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("Referred From",0,category));
					list.Add(new DisplayField("Date First Visit",0,category));
					list.Add(new DisplayField("Prov. (Pri, Sec)",0,category));
					list.Add(new DisplayField("Pri Ins",0,category));
					list.Add(new DisplayField("Sec Ins",0,category));
					list.Add(new DisplayField("Payor Types",0,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						list.Add(new DisplayField("Registration Keys",0,category));
						list.Add(new DisplayField("Ehr Provider Keys",0,category));
						list.Add(new DisplayField("References",0,category));
					}
					if(!Programs.UsingEcwTightOrFullMode()) {//different default list for eCW:
						list.Add(new DisplayField("Premedicate",0,category));
						list.Add(new DisplayField("Problems",0,category));
						list.Add(new DisplayField("Med Urgent",0,category));
						list.Add(new DisplayField("Medical Summary",0,category));
						list.Add(new DisplayField("Service Notes",0,category));
						list.Add(new DisplayField("Medications",0,category));
						list.Add(new DisplayField("Allergies",0,category));
						list.Add(new DisplayField("Pat Restrictions",0,category));
					}
					//list.Add(new DisplayField("PatFields",0,category));
					//list.Add(new DisplayField("Birthdate",0,category));
					//list.Add(new DisplayField("City",0,category));
					//list.Add(new DisplayField("AskToArriveEarly",0,category));
					//list.Add(new DisplayField("Super Head",0,category));
					//list.Add(new DisplayField("Patient Portal",0,category));
					//list.Add(new DisplayField("Broken Appts",0,category));
					//if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
					//	list.Add(new DisplayField("Tobacco Use",0,category));
					//list.Add(new DisplayField("Specialty",0,category));
					//}
					break;
				#endregion ChartPatientInformation
				#region ProcedureGroupNote
				case DisplayFieldCategory.ProcedureGroupNote:
					list.Add(new DisplayField("Date",67,category));
					list.Add(new DisplayField("Th",27,category));
					list.Add(new DisplayField("Surf",40,category));
					list.Add(new DisplayField("Description",203,category));
					list.Add(new DisplayField("Stat",25,category));
					list.Add(new DisplayField("Prov",42,category));
					list.Add(new DisplayField("Amount",48,category));
					list.Add(new DisplayField("Proc Code",62,category));
					//if(Programs.UsingOrion){
					//  list.Add(new DisplayField("Stat 2",36,category));
					//  list.Add(new DisplayField("On Call",45,category));
					//  list.Add(new DisplayField("Effective Comm",90,category));
					//  list.Add(new DisplayField("Repair",45,category));
					//	list.Add(new DisplayField("DPCpost",52,category));
					//}
					break;
				#endregion ProcedureGroupNote
				#region TreatmentPlanModule
				case DisplayFieldCategory.TreatmentPlanModule:
					list.Add(new DisplayField("Done",50,category));
					list.Add(new DisplayField("Priority",50,category));
					list.Add(new DisplayField("Tth",40,category));
					list.Add(new DisplayField("Surf",45,category));
					list.Add(new DisplayField("Code",50,category));
					list.Add(new DisplayField("Sub",28,category));
					list.Add(new DisplayField("Description",202,category));
					list.Add(new DisplayField("Fee",50,category));
					list.Add(new DisplayField("Allowed",50,category));
					list.Add(new DisplayField("Pri Ins",50,category));
					list.Add(new DisplayField("Sec Ins",50,category));
					list.Add(new DisplayField("DPlan",50,category));
					list.Add(new DisplayField("Discount",55,category));
					list.Add(new DisplayField("Pat",50,category));
					//list.Add(new DisplayField("Prognosis",60,category));
					//list.Add(new DisplayField("Dx",28,category));
					//list.Add(new DisplayField("Abbr",110,category));//proc abbr
					//list.Add(new DisplayField("Tax Est",50,category));
					//list.Add(new DisplayField("Prov",50,category));//provnum
					//list.Add(new DisplayField("DateTP",65,category));
					//list.Add(new DisplayField("Clinic",50,category));//clinicnum
					//list.Add(new DisplayField(InternalNames.TreatmentPlanModule.Appt,35,category));
					//list.Add(new DisplayField(InternalNames.TreatmentPlanModule.CatPercUCR,65,category));
					break;
				#endregion TreatmentPlanModule
				#region OrthoChart
				case DisplayFieldCategory.OrthoChart:
					//Ortho chart has no default columns. User must explicitly set up columns.
					break;
				#endregion OrthoChart
				#region AppointmentBubble
				case DisplayFieldCategory.AppointmentBubble:
					list.Add(new DisplayField("Patient Name",0,category));
					list.Add(new DisplayField("Patient Picture",0,category));
					list.Add(new DisplayField("Appt Day",0,category));
					list.Add(new DisplayField("Appt Date",0,category));
					list.Add(new DisplayField("Appt Time",0,category));
					list.Add(new DisplayField("Appt Length",0,category));
					list.Add(new DisplayField("Provider",0,category));
					list.Add(new DisplayField("Production",0,category));
					list.Add(new DisplayField("Confirmed",0,category));
					list.Add(new DisplayField("ASAP",0,category));
					list.Add(new DisplayField("Med Flag",0,category));
					list.Add(new DisplayField("Med Note",0,category));
					list.Add(new DisplayField("Lab",0,category));
					list.Add(new DisplayField("Procedures",0,category));
					list.Add(new DisplayField("Note",0,category));
					list.Add(new DisplayField("Horizontal Line",0,category));
					list.Add(new DisplayField("PatNum",0,category));
					list.Add(new DisplayField("ChartNum",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("Home Phone",0,category));
					list.Add(new DisplayField("Work Phone",0,category));
					list.Add(new DisplayField("Wireless Phone",0,category));
					list.Add(new DisplayField("Contact Methods",0,category));
					list.Add(new DisplayField("Insurance",0,category));
					list.Add(new DisplayField("Address Note",0,category));
					list.Add(new DisplayField("Fam Note",0,category));
					list.Add(new DisplayField("Appt Mod Note",0,category));
					//list.Add(new DisplayField("ReferralFrom",0,category));
					//list.Add(new DisplayField("ReferralTo",0,category));
					//list.Add(new DisplayField("Referral From With Phone",0,category));
					//list.Add(new DisplayField("Referral To With Phone",0,category));
					//list.Add(new DisplayField("Language",0,category));
					//list.Add(new DisplayField("Email",0,category));	
					//list.Add(new DisplayField("Insurance Color",0,category));
					//list.Add(new DisplayField("Discount Plan",0,category));
					//list.Add(new DisplayField("Estimated Patient Portion",0,category));
					//list.Add(new DisplayField("CareCredit Status",0,category));
					//list.Add(new DisplayField("Verify Insurance",0,category));
					break;
				#endregion AppointmentBubble
				#region AccountPatientInformation
				case DisplayFieldCategory.AccountPatientInformation:
					//AccountPatientInformation has no default columns.  User must explicitly set up columns.
					//list.Add(new DisplayField("Billing Type",0,category));
					//list.Add(new DisplayField("PatFields",0,category));
					break;
				#endregion AccountPatientInformation
				#region StatementMainGrid
				case DisplayFieldCategory.StatementMainGrid:
					int i=0;
					list.Add(new DisplayField {Category=category,InternalName="date",Description="Date",ColumnWidth=75,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="patient",Description="Patient",ColumnWidth=100,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="ProcCode",Description="Code",ColumnWidth=45,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="tth",Description="Tooth",ColumnWidth=45,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="description",Description="Description",ColumnWidth=275,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="charges",Description="Charges",ColumnWidth=60,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="credits",Description="Credits",ColumnWidth=60,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="balance",Description="Balance",ColumnWidth=60,ItemOrder=++i});
					break;
				#endregion StatementMainGrid
				#region FamilyRecallGrid
				case DisplayFieldCategory.FamilyRecallGrid:
					list.Add(new DisplayField("Type",90,category));
					list.Add(new DisplayField("Due Date",80,category));
					list.Add(new DisplayField("Sched Date",80,category));
					list.Add(new DisplayField("Notes",255,category));
					//list.Add(new DisplayField("Previous Date",90,category));
					//list.Add(new DisplayField("Interval",80,category));
					break;
				#endregion FamilyRecallGrid
				#region AppointmentEdit
				case DisplayFieldCategory.AppointmentEdit:
					list.Add(new DisplayField("Stat",35,category));
					list.Add(new DisplayField("Priority",45,category));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						list.Add(new DisplayField("Code",125,category));
					}
					else {
						list.Add(new DisplayField("Tth",25,category));
						list.Add(new DisplayField("Surf",50,category));
						list.Add(new DisplayField("Code",50,category));
					}
					list.Add(new DisplayField("Description",275,category));
					list.Add(new DisplayField("Fee",60,category));
					//list.Add(new DisplayField("Abbreviation",80,category));
					//list.Add(new DisplayField("Layman's Term",100,category));
					break;
				#endregion AppointmentEdit
				#region PlannedAppointmentEdit
				case DisplayFieldCategory.PlannedAppointmentEdit:
					list.Add(new DisplayField("Stat",35,category));
					list.Add(new DisplayField("Priority",45,category));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						list.Add(new DisplayField("Code",125,category));
					}
					else {
						list.Add(new DisplayField("Tth",25,category));
						list.Add(new DisplayField("Surf",50,category));
						list.Add(new DisplayField("Code",50,category));
					}
					list.Add(new DisplayField("Description",275,category));
					list.Add(new DisplayField("Fee",60,category));
					//list.Add(new DisplayField(InternalNames.PlannedAppointmentEdit.Abbreviation,80,category));
					break;
				#endregion PlannedAppointmentEdit
				#region OutstandingInsClaimsReport
				case DisplayFieldCategory.OutstandingInsReport:
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Carrier",145,category));
						list.Add(new DisplayField("Phone",95,category));
					}
					else {
						list.Add(new DisplayField("Carrier",200,category));
						list.Add(new DisplayField("Phone",100,category));
					}
					list.Add(new DisplayField("Type",55,category));
					list.Add(new DisplayField("User",60,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("PatName",115,category));
						list.Add(new DisplayField("Clinic",65,category));
					}
					else {
						list.Add(new DisplayField("PatName",120,category));
					}
					list.Add(new DisplayField("DateService",75,category));
					list.Add(new DisplayField("DateSent",70,category));
					list.Add(new DisplayField("DateSentOrig",80,category));
					list.Add(new DisplayField("TrackStat",100,category));
					list.Add(new DisplayField("DateStat",70,category));
					list.Add(new DisplayField("Error",50,category));
					list.Add(new DisplayField("Amount",60,category));
					//list.Add(new DisplayField("GroupNum",60,category));
					//list.Add(new DisplayField("GroupName",100,category));
					//list.Add(new DisplayField("SubName",120,category));
					//list.Add(new DisplayField("SubDOB",65,category));
					//list.Add(new DisplayField("SubID",65,category));
					//list.Add(new DisplayField("PatDOB",65,category));
					break;
				#endregion OutstandingInsClaimsReport
				#region CEMTSearchPatients
				case DisplayFieldCategory.CEMTSearchPatients:
					list.Add(new DisplayField("Conn",167,category));
					list.Add(new DisplayField("PatNum",64,category));
					list.Add(new DisplayField("LName",94,category));
					list.Add(new DisplayField("FName",94,category));
					list.Add(new DisplayField("SSN",94,category));
					list.Add(new DisplayField("PatStatus",60,category));
					list.Add(new DisplayField("Age",40,category));
					list.Add(new DisplayField("City",80,category));
					list.Add(new DisplayField("State",40,category));
					list.Add(new DisplayField("Address",167,category));
					list.Add(new DisplayField("Hm Phone",94,category));
					list.Add(new DisplayField("Email",190,category));
					list.Add(new DisplayField("ChartNum",70,category));
					list.Add(new DisplayField("Country",60,category));
					//list.Add(new DisplayField("MI",25,category));
					//list.Add(new DisplayField("Pref Name",60,category));
					//list.Add(new DisplayField("Wk Phone",94,category));
					//list.Add(new DisplayField("Bill Type",90,category));
					//list.Add(new DisplayField("Pri Prov",85,category));
					//list.Add(new DisplayField("Birthdate",70,category));
					//list.Add(new DisplayField("Site",90,category));
					//list.Add(new DisplayField("Clinic",90,category));
					//list.Add(new DisplayField("Wireless Ph",94,category));
					//list.Add(new DisplayField("Sec Prov",85,category));
					//list.Add(new DisplayField("LastVisit",70,category));
					//list.Add(new DisplayField("NextVisit",70,category));
					break;
				#endregion CEMTSearchPatients
				#region A/R Manager Unsent/Excluded Grids
				case DisplayFieldCategory.ArManagerUnsentGrid:
				case DisplayFieldCategory.ArManagerExcludedGrid:
					list.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?140:240,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Clinic",100,category));
					}
					list.Add(new DisplayField("Prov",70,category));
					list.Add(new DisplayField("Billing Type",83,category));
					list.Add(new DisplayField("0-30 Days",73,category));
					list.Add(new DisplayField("31-60 Days",75,category));
					list.Add(new DisplayField("61-90 Days",75,category));
					list.Add(new DisplayField("> 90 Days",73,category));
					list.Add(new DisplayField("Total",60,category));
					list.Add(new DisplayField("-Ins Est",65,category));
					list.Add(new DisplayField("=Patient",65,category));
					list.Add(new DisplayField("PayPlan Due",80,category));
					list.Add(new DisplayField("Last Paid",65,category));
					list.Add(new DisplayField("DateTime Suspended",135,category));
					//list.Add(new DisplayField("Last Proc",65,category));
					break;
				#endregion A/R Manager Unsent/Excluded Grids
				#region A/R Manager Sent Grid
				case DisplayFieldCategory.ArManagerSentGrid:
					list.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?150:250,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Clinic",100,category));
					}
					list.Add(new DisplayField("Prov",75,category));
					list.Add(new DisplayField("0-30 Days",65,category));
					list.Add(new DisplayField("31-60 Days",70,category));
					list.Add(new DisplayField("61-90 Days",70,category));
					list.Add(new DisplayField("> 90 Days",65,category));
					list.Add(new DisplayField("Total",55,category));
					list.Add(new DisplayField("-Ins Est",55,category));
					list.Add(new DisplayField("=Patient",55,category));
					list.Add(new DisplayField("PayPlan Due",80,category));
					list.Add(new DisplayField("Last Paid",65,category));
					list.Add(new DisplayField("Demand Type",90,category));
					list.Add(new DisplayField("Last Transaction",184,category));
					//list.Add(new DisplayField("Last Proc",65,category));
					break;
				#endregion A/R Manager Sent Grid
				default:
					break;
			}
			return list;
		}

		public static List<DisplayField> GetAllAvailableList(DisplayFieldCategory category){
			//No need to check RemotingRole; no call to db. 
			List<DisplayField> list=new List<DisplayField>();
			switch (category) {
				#region 'None'
				case DisplayFieldCategory.None://Currently only used for ChartViews
					list.Add(new DisplayField("Date",67,category));
					list.Add(new DisplayField("Time",40,category));
					list.Add(new DisplayField("Th",27,category));
					list.Add(new DisplayField("Surf",40,category));
					list.Add(new DisplayField("Dx",28,category));
					list.Add(new DisplayField("Description",218,category));
					list.Add(new DisplayField("Stat",25,category));
					list.Add(new DisplayField("Prov",42,category));
					list.Add(new DisplayField("Amount",48,category));
					list.Add(new DisplayField("Proc Code",62,category));
					list.Add(new DisplayField("User",62,category));
					list.Add(new DisplayField("Signed",55,category));
					list.Add(new DisplayField("Priority",44,category));
					list.Add(new DisplayField("Date TP",67,category));
					list.Add(new DisplayField("Date Entry",67,category));
					list.Add(new DisplayField("Prognosis",60,category));
					list.Add(new DisplayField("Length",40,category));
					list.Add(new DisplayField("Abbr",50,category));
					list.Add(new DisplayField("Locked",50,category));
					list.Add(new DisplayField("HL7 Sent",60,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("ClinicDesc",100,category));
						list.Add(new DisplayField("Clinic",90,category));
					}
					if(Programs.UsingOrion){
						list.Add(new DisplayField("DPC",33,category));
						list.Add(new DisplayField("Schedule By",72,category));
						list.Add(new DisplayField("Stop Clock",67,category));
						list.Add(new DisplayField("Stat 2",36,category));
						list.Add(new DisplayField("On Call",45,category));
						list.Add(new DisplayField("Effective Comm",90,category));
						list.Add(new DisplayField("End Time",56,category));//not visible unless orion
						list.Add(new DisplayField("Quadrant",55,category));//behavior is specific to orion
						list.Add(new DisplayField("DPCpost",52,category));
					}
					break;
				#endregion 'None'
				#region PatientSelect
				case DisplayFieldCategory.PatientSelect:
					list.Add(new DisplayField("LastName",75,category));
					list.Add(new DisplayField("First Name",75,category));
					list.Add(new DisplayField("MI",25,category));
					list.Add(new DisplayField("Pref Name",60,category));
					list.Add(new DisplayField("Age",30,category));
					list.Add(new DisplayField("SSN",70,category));
					list.Add(new DisplayField("Hm Phone",90,category));
					list.Add(new DisplayField("Wk Phone",90,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {//if for OD HQ
						list.Add(new DisplayField("OtherPhone",90,category));
						list.Add(new DisplayField("Country",90,category));
						list.Add(new DisplayField("RegKey",150,category));
					}
					list.Add(new DisplayField("PatNum",80,category));
					list.Add(new DisplayField("ChartNum",60,category));
					list.Add(new DisplayField("Address",100,category));
					list.Add(new DisplayField("Status",65,category));
					list.Add(new DisplayField("Bill Type",90,category));
					list.Add(new DisplayField("City",80,category));
					list.Add(new DisplayField("State",55,category));
					list.Add(new DisplayField("Pri Prov",85,category));
					list.Add(new DisplayField("Birthdate",70,category));
					list.Add(new DisplayField("Site",90,category));
					list.Add(new DisplayField("Email",90,category));
					list.Add(new DisplayField("Clinic",90,category));
					list.Add(new DisplayField("Wireless Ph",90,category));
					list.Add(new DisplayField("Sec Prov",85,category));
					list.Add(new DisplayField("LastVisit",70,category));
					list.Add(new DisplayField("NextVisit",70,category));
					list.Add(new DisplayField("Invoice Number",90,category));
					list.Add(new DisplayField("Specialty",90,category));
					break;
				#endregion PatientSelect
				#region PatientInformation
				case DisplayFieldCategory.PatientInformation:
					list.Add(new DisplayField("Last",0,category));
					list.Add(new DisplayField("First",0,category));
					list.Add(new DisplayField("Middle",0,category));
					list.Add(new DisplayField("Preferred",0,category));
					list.Add(new DisplayField("Title",0,category));
					list.Add(new DisplayField("Salutation",0,category));
					list.Add(new DisplayField("Status",0,category));
					list.Add(new DisplayField("Gender",0,category));
					list.Add(new DisplayField("Position",0,category));
					list.Add(new DisplayField("Birthdate",0,category));
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("SS#",0,category));
					list.Add(new DisplayField("Address",0,category));
					list.Add(new DisplayField("Address2",0,category));
					list.Add(new DisplayField("City",0,category));
					list.Add(new DisplayField("State",0,category));
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
						list.Add(new DisplayField("Country",0,category));
					}
					if(PrefC.IsODHQ) {
						list.Add(new DisplayField("Tax Address",0,category));
					}
					list.Add(new DisplayField("Zip",0,category));
					list.Add(new DisplayField("Hm Phone",0,category));
					list.Add(new DisplayField("Wk Phone",0,category));
					list.Add(new DisplayField("Wireless Ph",0,category));
					list.Add(new DisplayField("E-mail",0,category));
					list.Add(new DisplayField("Contact Method",0,category));
					list.Add(new DisplayField("ABC0",0,category));
					list.Add(new DisplayField("Chart Num",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("Ward",0,category));
					list.Add(new DisplayField("AdmitDate",0,category));
					list.Add(new DisplayField("Primary Provider",0,category));
					list.Add(new DisplayField("Sec. Provider",0,category));
					list.Add(new DisplayField("Payor Types",0,category));
					list.Add(new DisplayField("Language",0,category));
					list.Add(new DisplayField("Clinic",0,category));
					list.Add(new DisplayField("ResponsParty",0,category));
					list.Add(new DisplayField("Referrals",0,category));
					list.Add(new DisplayField("Addr/Ph Note",0,category));
					list.Add(new DisplayField("PatFields",0,category));
					list.Add(new DisplayField("Guardians",0,category));
					list.Add(new DisplayField("Arrive Early",0,category));
					list.Add(new DisplayField("Super Head",0,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						list.Add(new DisplayField("References",0,category));
					}
					list.Add(new DisplayField("Pat Restrictions",0,category));
					list.Add(new DisplayField("ICE Name",0,category));
					list.Add(new DisplayField("ICE Phone",0,category));
					break;
				#endregion PatientInformation
				#region AccountModule
				case DisplayFieldCategory.AccountModule:
					list.Add(new DisplayField("Date",65,category));
					list.Add(new DisplayField("Patient",100,category));
					list.Add(new DisplayField("Prov",40,category));
					list.Add(new DisplayField("Code",46,category));
					list.Add(new DisplayField("Tth",26,category));
					list.Add(new DisplayField("Description",270,category));
					list.Add(new DisplayField("Charges",60,category));
					list.Add(new DisplayField("Credits",60,category));
					list.Add(new DisplayField("Balance",60,category));
					list.Add(new DisplayField("Signed",60,category));
					list.Add(new DisplayField("Abbr",110,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Clinic",50,category));
						list.Add(new DisplayField("ClinicDesc",110,category));
					}
					break;
				#endregion AccountModule
				#region RecallList
				case DisplayFieldCategory.RecallList:
					list.Add(new DisplayField("Due Date",75,category));
					list.Add(new DisplayField("Patient",120,category));
					list.Add(new DisplayField("Age",30,category));
					list.Add(new DisplayField("Type",60,category));
					list.Add(new DisplayField("Interval",50,category));
					list.Add(new DisplayField("#Remind",55,category));
					list.Add(new DisplayField("LastRemind",75,category));
					list.Add(new DisplayField("Contact",120,category));
					list.Add(new DisplayField("Status",130,category));
					list.Add(new DisplayField("Note",215,category));
					list.Add(new DisplayField("BillingType",100,category));
					list.Add(new DisplayField("WebSched",100,category));
					break;
				#endregion RecallList
				#region ChartPatientInformation
				case DisplayFieldCategory.ChartPatientInformation:
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("ABC0",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("Referred From",0,category));
					list.Add(new DisplayField("Date First Visit",0,category));
					list.Add(new DisplayField("Prov. (Pri, Sec)",0,category));
					list.Add(new DisplayField("Pri Ins",0,category));
					list.Add(new DisplayField("Sec Ins",0,category));
					list.Add(new DisplayField("Payor Types",0,category));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						list.Add(new DisplayField("Registration Keys",0,category));
						list.Add(new DisplayField("Ehr Provider Keys",0,category));
						list.Add(new DisplayField("References",0,category));
					}
					list.Add(new DisplayField("Premedicate",0,category));
					list.Add(new DisplayField("Problems",0,category));
					list.Add(new DisplayField("Med Urgent",0,category));
					list.Add(new DisplayField("Medical Summary",0,category));
					list.Add(new DisplayField("Service Notes",0,category));
					list.Add(new DisplayField("Medications",0,category));
					list.Add(new DisplayField("Allergies",0,category));
					list.Add(new DisplayField("PatFields",0,category));
					list.Add(new DisplayField("Birthdate",0,category));
					list.Add(new DisplayField("City",0,category));
					list.Add(new DisplayField("AskToArriveEarly",0,category));
					list.Add(new DisplayField("Super Head",0,category));
					list.Add(new DisplayField("Patient Portal",0,category));
					list.Add(new DisplayField("Broken Appts",0,category));
					if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
						list.Add(new DisplayField("Tobacco Use",0,category));
					}
					list.Add(new DisplayField("Pat Restrictions",0,category));
					list.Add(new DisplayField("Specialty",0,category));
					break;
				#endregion ChartPatientInformation
				#region ProcedureGroupNote
				case DisplayFieldCategory.ProcedureGroupNote:
					list.Add(new DisplayField("Date",67,category));
					list.Add(new DisplayField("Th",27,category));
					list.Add(new DisplayField("Surf",40,category));
					list.Add(new DisplayField("Description",218,category));
					list.Add(new DisplayField("Stat",25,category));
					list.Add(new DisplayField("Prov",42,category));
					list.Add(new DisplayField("Amount",48,category));
					list.Add(new DisplayField("Proc Code",62,category));
					if(Programs.UsingOrion){
						list.Add(new DisplayField("Stat 2",36,category));
						list.Add(new DisplayField("On Call",45,category));
						list.Add(new DisplayField("Effective Comm",90,category));
						list.Add(new DisplayField("Repair",45,category));
						list.Add(new DisplayField("DPCpost",52,category));
					}
					break;
				#endregion ProcedureGroupNote
				#region TreatmentPlanModule
				case DisplayFieldCategory.TreatmentPlanModule:
					list.Add(new DisplayField("Done",50,category));
					list.Add(new DisplayField("Priority",50,category));
					list.Add(new DisplayField("Tth",40,category));
					list.Add(new DisplayField("Surf",45,category));
					list.Add(new DisplayField("Code",50,category));
					list.Add(new DisplayField("Sub",28,category));
					list.Add(new DisplayField("Description",202,category));
					list.Add(new DisplayField("Fee",50,category));
					list.Add(new DisplayField("Allowed",50,category));
					list.Add(new DisplayField("Pri Ins",50,category));
					list.Add(new DisplayField("Sec Ins",50,category));
					list.Add(new DisplayField("DPlan",50,category));
					list.Add(new DisplayField("Discount",55,category));
					list.Add(new DisplayField("Pat",50,category));
					list.Add(new DisplayField("Prognosis",60,category));
					list.Add(new DisplayField("Dx",28,category));
					list.Add(new DisplayField("Abbr",110,category));//proc abbr
					list.Add(new DisplayField("Tax Est",50,category)); //Sales tax
					list.Add(new DisplayField("Prov",50,category));//provnum
					list.Add(new DisplayField("DateTP",65,category));
					list.Add(new DisplayField("Clinic",50,category));//clinicnum
					list.Add(new DisplayField(InternalNames.TreatmentPlanModule.Appt,35,category));
					list.Add(new DisplayField(InternalNames.TreatmentPlanModule.CatPercUCR,65,category));
					break;
				#endregion TreatmentPlanModule
				#region OrthoChart
				case DisplayFieldCategory.OrthoChart:
					list=GetForCategory(DisplayFieldCategory.OrthoChart); //The display fields that the user has already saved
					List<string> listDistinctFieldNames=OrthoCharts.GetDistinctFieldNames();
					foreach(string fieldName in listDistinctFieldNames) {
						//If there aren't any display fields with the current field name, create one and add it to the list of available fields.
						if(!list.Any(x => x.Description==fieldName)) {
							DisplayField displayField=new DisplayField("",20,DisplayFieldCategory.OrthoChart);
							displayField.IsNew=true;
							displayField.Description=fieldName;
							list.Add(displayField);
						}
					}
					break;
				#endregion OrthoChart
				#region AppointmentBubble
				case DisplayFieldCategory.AppointmentBubble:
					list.Add(new DisplayField("Patient Name",0,category));
					list.Add(new DisplayField("Patient Picture",0,category));
					list.Add(new DisplayField("Appt Day",0,category));
					list.Add(new DisplayField("Appt Date",0,category));
					list.Add(new DisplayField("Appt Time",0,category));
					list.Add(new DisplayField("Appt Length",0,category));
					list.Add(new DisplayField("Provider",0,category));
					list.Add(new DisplayField("Production",0,category));
					list.Add(new DisplayField("Confirmed",0,category));
					list.Add(new DisplayField("ASAP",0,category));
					list.Add(new DisplayField("Med Flag",0,category));
					list.Add(new DisplayField("Med Note",0,category));
					list.Add(new DisplayField("Lab",0,category));
					list.Add(new DisplayField("Procedures",0,category));
					list.Add(new DisplayField("Note",0,category));
					list.Add(new DisplayField("Horizontal Line",0,category));
					list.Add(new DisplayField("PatNum",0,category));
					list.Add(new DisplayField("ChartNum",0,category));
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("Age",0,category));
					list.Add(new DisplayField("Home Phone",0,category));
					list.Add(new DisplayField("Work Phone",0,category));
					list.Add(new DisplayField("Wireless Phone",0,category));
					list.Add(new DisplayField("Contact Methods",0,category));
					list.Add(new DisplayField("Insurance",0,category));
					list.Add(new DisplayField("Insurance Color",0,category));
					list.Add(new DisplayField("Address Note",0,category));
					list.Add(new DisplayField("Fam Note",0,category));
					list.Add(new DisplayField("Appt Mod Note",0,category));
					list.Add(new DisplayField("ReferralFrom",0,category));
					list.Add(new DisplayField("ReferralTo",0,category));
					list.Add(new DisplayField("Referral From With Phone",0,category));
					list.Add(new DisplayField("Referral To With Phone",0,category));
					list.Add(new DisplayField("Language",0,category));
					list.Add(new DisplayField("Email",0,category));
					list.Add(new DisplayField("Discount Plan",0,category));
					list.Add(new DisplayField("Estimated Patient Portion",0,category));
					list.Add(new DisplayField("CareCredit Status",0,category));
					list.Add(new DisplayField("Verify Insurance",0,category));
					break;
				#endregion AppointmentBubble
				#region AccountPatientInformation
				case DisplayFieldCategory.AccountPatientInformation:
					list.Add(new DisplayField("Billing Type",0,category));
					list.Add(new DisplayField("PatFields",0,category));
					break;
				#endregion AccountPatientInformation
				#region StatementMainGrid
				case DisplayFieldCategory.StatementMainGrid:
					int i=0;
					list.Add(new DisplayField {Category=category,InternalName="date",Description="Date",ColumnWidth=75,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="prov",Description="Prov",ColumnWidth=60,ItemOrder=++i });
					list.Add(new DisplayField {Category=category,InternalName="patient",Description="Patient",ColumnWidth=100,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="ProcCode",Description="Code",ColumnWidth=45,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="tth",Description="Tooth",ColumnWidth=45,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="description",Description="Description",ColumnWidth=275,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="charges",Description="Charges",ColumnWidth=60,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="credits",Description="Credits",ColumnWidth=60,ItemOrder=++i});
					list.Add(new DisplayField {Category=category,InternalName="invoiceNum",Description="Invoice#",ColumnWidth=60,ItemOrder=++i });
					list.Add(new DisplayField {Category=category,InternalName="balance",Description="Balance",ColumnWidth=60,ItemOrder=++i});
					break;
				#endregion StatementMainGrid
				#region FamilyRecallGrid
				case DisplayFieldCategory.FamilyRecallGrid:
					list.Add(new DisplayField("Type",90,category));
					list.Add(new DisplayField("Due Date",80,category));
					list.Add(new DisplayField("Sched Date",80,category));
					list.Add(new DisplayField("Notes",255,category));
					list.Add(new DisplayField("Previous Date",90,category));
					list.Add(new DisplayField("Interval",80,category));
					break;
				#endregion FamilyRecallGrid
				#region AppointmentEdit
				case DisplayFieldCategory.AppointmentEdit:
					list.Add(new DisplayField("Stat",35,category));
					list.Add(new DisplayField("Priority",45,category));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						list.Add(new DisplayField("Code",125,category));
					}
					else {
						list.Add(new DisplayField("Tth",25,category));
						list.Add(new DisplayField("Surf",50,category));
						list.Add(new DisplayField("Code",50,category));
					}
					list.Add(new DisplayField("Description",255,category));
					list.Add(new DisplayField("Fee",70,category));
					list.Add(new DisplayField("Abbreviation",80,category));
					list.Add(new DisplayField("Layman's Term",100,category));
					break;
				#endregion AppointmentEdit
				#region PlannedAppointmentEdit
				case DisplayFieldCategory.PlannedAppointmentEdit:
					list.Add(new DisplayField("Stat",35,category));
					list.Add(new DisplayField("Priority",45,category));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						list.Add(new DisplayField("Code",125,category));
					}
					else {
						list.Add(new DisplayField("Tth",25,category));
						list.Add(new DisplayField("Surf",50,category));
						list.Add(new DisplayField("Code",50,category));
					}
					list.Add(new DisplayField("Description",275,category));
					list.Add(new DisplayField("Fee",60,category));
					list.Add(new DisplayField(InternalNames.PlannedAppointmentEdit.Abbreviation,80,category));
					break;
				#endregion PlannedAppointmentEdit
				#region OutstandingInsClaimsReport
				case DisplayFieldCategory.OutstandingInsReport:
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Carrier",145,category));
						list.Add(new DisplayField("Phone",95,category));
					}
					else {
						list.Add(new DisplayField("Carrier",200,category));
						list.Add(new DisplayField("Phone",100,category));
					}
					list.Add(new DisplayField("Type",55,category));
					list.Add(new DisplayField("User",60,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("PatName",115,category));
						list.Add(new DisplayField("Clinic",65,category));
					}
					else {
						list.Add(new DisplayField("PatName",120,category));
					}
					list.Add(new DisplayField("DateService",75,category));
					list.Add(new DisplayField("DateSent",70,category));
					list.Add(new DisplayField("DateSentOrig",80,category));
					list.Add(new DisplayField("TrackStat",100,category));
					list.Add(new DisplayField("DateStat",70,category));
					list.Add(new DisplayField("Error",50,category));
					list.Add(new DisplayField("Amount",60,category));
					list.Add(new DisplayField("GroupNum",60,category));
					list.Add(new DisplayField("GroupName",100,category));
					list.Add(new DisplayField("SubName",120,category));
					list.Add(new DisplayField("SubDOB",65,category));
					list.Add(new DisplayField("SubID",65,category));
					list.Add(new DisplayField("PatDOB",65,category));
					break;
				#endregion OutstandingInsClaimsReport
				#region CEMTSearchPatients
				case DisplayFieldCategory.CEMTSearchPatients:
					list.Add(new DisplayField("Conn",167,category));
					list.Add(new DisplayField("PatNum",64,category));
					list.Add(new DisplayField("LName",94,category));
					list.Add(new DisplayField("FName",94,category));
					list.Add(new DisplayField("SSN",94,category));
					list.Add(new DisplayField("PatStatus",60,category));
					list.Add(new DisplayField("Age",40,category));
					list.Add(new DisplayField("City",80,category));
					list.Add(new DisplayField("State",40,category));
					list.Add(new DisplayField("Address",167,category));
					list.Add(new DisplayField("Hm Phone",94,category));
					list.Add(new DisplayField("Email",190,category));
					list.Add(new DisplayField("ChartNum",70,category));
					list.Add(new DisplayField("Country",60,category));
					list.Add(new DisplayField("MI",25,category));
					list.Add(new DisplayField("Pref Name",60,category));
					list.Add(new DisplayField("Wk Phone",94,category));
					list.Add(new DisplayField("Bill Type",90,category));
					list.Add(new DisplayField("Pri Prov",85,category));
					list.Add(new DisplayField("Birthdate",70,category));
					list.Add(new DisplayField("Site",90,category));
					list.Add(new DisplayField("Clinic",90,category));
					list.Add(new DisplayField("Wireless Ph",94,category));
					list.Add(new DisplayField("Sec Prov",85,category));
					list.Add(new DisplayField("LastVisit",70,category));
					list.Add(new DisplayField("NextVisit",70,category));
					break;
				#endregion CEMTSearchPatients
				#region A/R Manager Unsent/Excluded Grids
				case DisplayFieldCategory.ArManagerUnsentGrid:
				case DisplayFieldCategory.ArManagerExcludedGrid:
					list.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?140:240,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Clinic",100,category));
					}
					list.Add(new DisplayField("Prov",70,category));
					list.Add(new DisplayField("Billing Type",83,category));
					list.Add(new DisplayField("0-30 Days",73,category));
					list.Add(new DisplayField("31-60 Days",75,category));
					list.Add(new DisplayField("61-90 Days",75,category));
					list.Add(new DisplayField("> 90 Days",73,category));
					list.Add(new DisplayField("Total",60,category));
					list.Add(new DisplayField("-Ins Est",65,category));
					list.Add(new DisplayField("=Patient",65,category));
					list.Add(new DisplayField("PayPlan Due",80,category));
					list.Add(new DisplayField("Last Paid",65,category));
					list.Add(new DisplayField("DateTime Suspended",135,category));
					list.Add(new DisplayField("Last Proc",65,category));
					list.Add(new DisplayField(InternalNames.ArManagerUnsentGrid.DateBalBegan,95,category));
					list.Add(new DisplayField(InternalNames.ArManagerUnsentGrid.DaysBalBegan,95,category));
					break;
				#endregion A/R Manager Unsent/Excluded Grids
				#region A/R Manager Sent Grid
				case DisplayFieldCategory.ArManagerSentGrid:
					list.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?150:250,category));
					if(PrefC.HasClinicsEnabled) {
						list.Add(new DisplayField("Clinic",100,category));
					}
					list.Add(new DisplayField("Prov",75,category));
					list.Add(new DisplayField("0-30 Days",65,category));
					list.Add(new DisplayField("31-60 Days",70,category));
					list.Add(new DisplayField("61-90 Days",70,category));
					list.Add(new DisplayField("> 90 Days",65,category));
					list.Add(new DisplayField("Total",55,category));
					list.Add(new DisplayField("-Ins Est",55,category));
					list.Add(new DisplayField("=Patient",55,category));
					list.Add(new DisplayField("PayPlan Due",80,category));
					list.Add(new DisplayField("Last Paid",65,category));
					list.Add(new DisplayField("Demand Type",90,category));
					list.Add(new DisplayField("Last Transaction",184,category));
					list.Add(new DisplayField("Last Proc",65,category));
					list.Add(new DisplayField(InternalNames.ArManagerSentGrid.DateBalBegan,95,category));
					list.Add(new DisplayField(InternalNames.ArManagerSentGrid.DaysBalBegan,95,category));
					break;
				#endregion A/R Manager Sent Grid
				default:
					break;
			}
			return list;
		}

		public static void SaveListForCategory(List<DisplayField> ListShowing,DisplayFieldCategory category){
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ListShowing,category);
				return;
			}
			bool isDefault=true;
			List<DisplayField> defaultList=GetDefaultList(category);
			if(ListShowing.Count!=defaultList.Count){
				isDefault=false;
			}
			else{
				for(int i=0;i<ListShowing.Count;i++){
					if(ListShowing[i].Description!=""){
						isDefault=false;
						break;
					}
					if(ListShowing[i].InternalName!=defaultList[i].InternalName){
						isDefault=false;
						break;
					}
					if(ListShowing[i].ColumnWidth!=defaultList[i].ColumnWidth) {
						isDefault=false;
						break;
					}
				}
			}
			string command="DELETE FROM displayfield WHERE Category="+POut.Long((int)category);
			Db.NonQ(command);
			if(isDefault){
				return;
			}
			for(int i=0;i<ListShowing.Count;i++){
				ListShowing[i].ItemOrder=i;
				Insert(ListShowing[i]);
			}
		}

		public static void SaveListForChartView(List<DisplayField> ListShowing,long ChartViewNum) {
			if(RemotingClient.RemotingRole==RemotingRole.ClientWeb) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),ListShowing,ChartViewNum);
				return;
			}
			//Odds are, that if they are creating a custom view, that the fields are not default. If they are default, this code still works.
			string command="DELETE FROM displayfield WHERE ChartViewNum="+POut.Long((long)ChartViewNum);
			Db.NonQ(command);
			for(int i=0;i<ListShowing.Count;i++) {
				ListShowing[i].ItemOrder=i;
				ListShowing[i].ChartViewNum=ChartViewNum;
				Insert(ListShowing[i]);
			}
		}
		
		///<summary>This class can be used to have a strongly-typed reference to display field internal names.</summary>
		public class InternalNames {
			public class ChartView {
				public const string Date="Date";
				public const string Time="Time";
				public const string Th="Th";
				public const string Surf="Surf";
				public const string Dx="Dx";
				public const string Description="Description";
				public const string Stat="Stat";
				public const string Prov="Prov";
				public const string Amount="Amount";
				public const string ProcCode="Proc Code";
				public const string User="User";
				public const string Signed="Signed";
				public const string Priority="Priority";
				public const string DateEntry="Date Entry";
				public const string Prognosis="Prognosis";
				public const string DateTP="Date TP";
				public const string EndTime="End Time";
				public const string Quadrant="Quadrant";
				public const string ScheduleBy="Schedule By";
				public const string StopClock="Stop Clock";
				public const string DPC="DPC";
				public const string EffectiveComm="Effective Comm";
				public const string OnCall="On Call";
				public const string Stat2="Stat 2";
				public const string DPCpost="DPCpost";
				public const string Length="Length";
				public const string Abbr="Abbr";
				public const string Locked="Locked";
				public const string HL7Sent="HL7 Sent";
				public const string Clinic="Clinic";
				public const string ClinicDesc="ClinicDesc";
			}			
			public class ArManagerUnsentGrid {
				public const string DateBalBegan="Date Bal Began";
				public const string DaysBalBegan="Days Bal Began";
			}
			public class ArManagerSentGrid {
				public const string DateBalBegan="Date Bal Began";
				public const string DaysBalBegan="Days Bal Began";
			}
			public class ArManagerExcludedGrid {
				public const string DateBalBegan="Date Bal Began";
				public const string DaysBalBegan="Days Bal Began";
			}
			public class TreatmentPlanModule {
				public const string Fee="Fee";
				public const string Appt="Appt";
				public const string CatPercUCR="Cat% UCR";
			}
			public class PlannedAppointmentEdit {
				public const string Abbreviation="Abbreviation";
			}
		}
	}
}
