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
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				DataTable table=Meth.GetTable(MethodBase.GetCurrentMethod(),doRefreshCache);
				_displayFieldCache.FillCacheFromTable(table);
				return table;
			}
			return _displayFieldCache.GetTableFromCache(doRefreshCache);
		}

		public static void ClearCache() {
			_displayFieldCache.ClearCache();
		}
		#endregion
		
		///<summary></summary>
		public static long Insert(DisplayField displayField) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				displayField.DisplayFieldNum=Meth.GetLong(MethodBase.GetCurrentMethod(),displayField);
				return displayField.DisplayFieldNum;
			}
			return Crud.DisplayFieldCrud.Insert(displayField);
		}

		///<summary></summary>
		public static void Update(DisplayField displayField) {			
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),displayField);
				return;
			}
			Crud.DisplayFieldCrud.Update(displayField);
		}

		///<summary></summary>
		public static void Delete(long displayFieldNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),displayFieldNum);
				return;
			}
			string command="DELETE FROM displayfield WHERE DisplayFieldNum = "+POut.Long(displayFieldNum);
			Db.NonQ(command);
		}

		///<summary></summary>
		public static void DeleteForChartView(long chartViewNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),chartViewNum);
				return;
			}
			string command="DELETE FROM displayfield WHERE ChartViewNum = "+POut.Long(chartViewNum);
			Db.NonQ(command);
		}

		/// <summary> Returns true if a display field with the provided InternalName is set to show in the provided DisplayFieldCategory. </summary>
		public static bool IsInUse(DisplayFieldCategory displayFieldCategory, string internalName) {
			Meth.NoCheckMiddleTierRole();
			if(string.IsNullOrEmpty(internalName)) {
				return false;
			}
			List<DisplayField> listDisplayFields=GetForCategory(displayFieldCategory);
			bool isVisibleCategory=listDisplayFields.Find(x=>x.InternalName==internalName)!=null;
			return isVisibleCategory;
		}

		///<Summary>Returns an ordered list for just one category.  Do not use with None, or it will malfunction.  These are display fields that the user has entered, which are stored in the db, and then are pulled into the cache.  Categories with no display fields will return the default list.</Summary>
		public static List<DisplayField> GetForCategory(DisplayFieldCategory displayFieldCategory){
			Meth.NoCheckMiddleTierRole();
			List<DisplayField> listDisplayFields=GetWhere(x => x.Category==displayFieldCategory);
			if(listDisplayFields.Count==0) {//default
				return DisplayFields.GetDefaultList(displayFieldCategory);
			}
			return listDisplayFields;
		}

		///<Summary>Returns an ordered list for just one chart view</Summary>
		public static List<DisplayField> GetForChartView(long chartViewNum) {
			Meth.NoCheckMiddleTierRole();
			List<DisplayField> listDisplayFields=GetWhere(x => x.ChartViewNum==chartViewNum && x.Category==DisplayFieldCategory.None);
			if(listDisplayFields.Count==0) {//default
				return DisplayFields.GetDefaultList(DisplayFieldCategory.None);
			}
			return listDisplayFields;
		}

		public static List<DisplayField> GetDefaultList(DisplayFieldCategory displayFieldCategory){
			Meth.NoCheckMiddleTierRole();
			List<DisplayField> listDisplayFields=new List<DisplayField>();
			switch (displayFieldCategory) {
				#region 'None'
				case DisplayFieldCategory.None:
					listDisplayFields.Add(new DisplayField("Date",67,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Time",40));
					listDisplayFields.Add(new DisplayField("Th",27,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Dx",28,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",218,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stat",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",42,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",48,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Proc Code",62,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("User",62,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Signed",55,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Priority",65,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Date TP",67,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Date Entry",67,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Prognosis",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Length",40,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Abbr",50,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Locked",50,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("HL7 Sent",60,displayFieldCategory));
					//if(PrefC.HasClinicsEnabled) {
					//	listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					//}
					//if(Programs.UsingOrion){
						//listDisplayFields.Add(new DisplayField("DPC",33,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Schedule By",72,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Stop Clock",67,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Stat 2",36,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("On Call",45,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Effective Comm",90,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("End Time",56,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Quadrant",55,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("DPCpost",52,displayFieldCategory));
					//}
					break;
				#endregion 'None'
				#region PatientSelect
				case DisplayFieldCategory.PatientSelect:
					listDisplayFields.Add(new DisplayField("LastName",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("First Name",75,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("MI",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pref Name",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",30,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SSN",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wk Phone",90,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {//if for OD HQ
						//listDisplayFields.Add(new DisplayField("OtherPhone",90,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("Country",90,displayFieldCategory));
						//listDisplayFields.Add(new DisplayField("RegKey",150,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("PatNum",80,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("ChartNum",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",65,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Bill Type",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("City",80,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("State",55,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Pri Prov",85,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Birthdate",70,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Site",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Email",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Wireless Ph",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Sec Prov",85,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("LastVisit",70,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("NextVisit",70,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Invoice Number",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Specialty",90,displayFieldCategory));
					break;
				#endregion PatientSelect
				#region PatientInformation
				case DisplayFieldCategory.PatientInformation:
					listDisplayFields.Add(new DisplayField("Last",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("First",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Middle",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred Pronoun",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Title",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Salutation",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Gender",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Position",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Birthdate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SS#",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address2",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("State",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
						listDisplayFields.Add(new DisplayField("Country",0,displayFieldCategory));
					}
					if(PrefC.IsODHQ) {
						listDisplayFields.Add(new DisplayField("Tax Address",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Zip",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wk Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Ph",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("E-mail",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact Method",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ABC0",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Chart Num",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Ward",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("AdmitDate",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("DischargeDate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Primary Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec. Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Payor Types",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Language",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("ResponsParty",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referrals",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Addr/Ph Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Guardians",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Arrive Early",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Super Head",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						listDisplayFields.Add(new DisplayField("References",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Pat Restrictions",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ICE Name",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ICE Phone",0,displayFieldCategory));
					break;
				#endregion PatientInformation
				#region AccountModule
				case DisplayFieldCategory.AccountModule:
					listDisplayFields.Add(new DisplayField("Date",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",40,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",46,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",26,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",270,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Charges",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Credits",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Balance",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Signed",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));
					break;
				#endregion AccountModule
				#region RecallList
				case DisplayFieldCategory.RecallList:
					listDisplayFields.Add(new DisplayField("Due Date",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",120,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",30,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Type",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Interval",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("#Remind",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LastRemind",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact",120,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",130,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Note",215,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("BillingType",100,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("WebSched",100,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Carrier Name",100,displayFieldCategory));
					break;
				#endregion RecallList
				#region ChartPatientInformation
				case DisplayFieldCategory.ChartPatientInformation:
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred Pronoun",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ABC0",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referred From",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Date First Visit",0,displayFieldCategory));
					if(!PrefC.GetBool(PrefName.EasyHideHospitals)){ //true will hide
						listDisplayFields.Add(new DisplayField("Admit Date",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Discharge Date",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov. (Pri, Sec)",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Ins",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Ins",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Payor Types",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						listDisplayFields.Add(new DisplayField("Registration Keys",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Ehr Provider Keys",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("References",0,displayFieldCategory));
					}
					if(!Programs.UsingEcwTightOrFullMode()) {//different default listDisplayFields for eCW:
						listDisplayFields.Add(new DisplayField("Premedicate",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Problems",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Med Urgent",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Medical Summary",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Service Notes",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Medications",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Allergies",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Pat Restrictions",0,displayFieldCategory));
					}
					//listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Birthdate",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("City",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("AskToArriveEarly",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Super Head",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Patient Portal",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Broken Appts",0,displayFieldCategory));
					//if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
					//	listDisplayFields.Add(new DisplayField("Tobacco Use",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Specialty",0,displayFieldCategory));
					//}
					break;
				#endregion ChartPatientInformation
				#region ProcedureGroupNote
				case DisplayFieldCategory.ProcedureGroupNote:
					listDisplayFields.Add(new DisplayField("Date",67,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Th",27,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",203,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stat",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",42,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",48,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Proc Code",62,displayFieldCategory));
					//if(Programs.UsingOrion){
					//  listDisplayFields.Add(new DisplayField("Stat 2",36,displayFieldCategory));
					//  listDisplayFields.Add(new DisplayField("On Call",45,displayFieldCategory));
					//  listDisplayFields.Add(new DisplayField("Effective Comm",90,displayFieldCategory));
					//  listDisplayFields.Add(new DisplayField("Repair",45,displayFieldCategory));
					//	listDisplayFields.Add(new DisplayField("DPCpost",52,displayFieldCategory));
					//}
					break;
				#endregion ProcedureGroupNote
				#region TreatmentPlanModule
				case DisplayFieldCategory.TreatmentPlanModule:
					listDisplayFields.Add(new DisplayField("Done",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",45,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sub",28,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",202,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Allowed",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Ins",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Ins",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DPlan",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Discount",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pat",50,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Prognosis",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Dx",28,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));//proc abbr
					//listDisplayFields.Add(new DisplayField("Tax Est",50,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Prov",50,displayFieldCategory));//provnum
					//listDisplayFields.Add(new DisplayField("DateTP",65,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));//clinicnum
					//listDisplayFields.Add(new DisplayField(InternalNames.TreatmentPlanModule.Appt,35,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField(InternalNames.TreatmentPlanModule.CatPercUCR,65,displayFieldCategory));
					break;
				#endregion TreatmentPlanModule
				#region OrthoChart
				case DisplayFieldCategory.OrthoChart:
					//Ortho chart has no default columns. User must explicitly set up columns.
					break;
				#endregion OrthoChart
				#region AppointmentBubble
				case DisplayFieldCategory.AppointmentBubble:
					listDisplayFields.Add(new DisplayField("Patient Name",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient Picture",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Day",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Date",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Time",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Length",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Production",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Confirmed",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ASAP",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Med Flag",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Med Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Lab",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Procedures",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Horizontal Line",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatNum",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ChartNum",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Home Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Work Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact Methods",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Insurance",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fam Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Mod Note",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Net Production",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("ReferralFrom",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("ReferralTo",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Referral From With Phone",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Referral To With Phone",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Language",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Email",0,displayFieldCategory));	
					//listDisplayFields.Add(new DisplayField("Insurance Color",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Discount Plan",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Estimated Patient Portion",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("CareCredit Status",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Verify Insurance",0,displayFieldCategory));
					break;
				#endregion AppointmentBubble
				#region AccountPatientInformation
				case DisplayFieldCategory.AccountPatientInformation:
					//AccountPatientInformation has no default columns.  User must explicitly set up columns.
					//listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					break;
				#endregion AccountPatientInformation
				#region StatementMainGrid
				case DisplayFieldCategory.StatementMainGrid:
					int i=0;
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="date",Description="Date",ColumnWidth=75,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="patient",Description="Patient",ColumnWidth=100,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="ProcCode",Description="Code",ColumnWidth=45,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="tth",Description="Tooth",ColumnWidth=45,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="description",Description="Description",ColumnWidth=275,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="charges",Description="Charges",ColumnWidth=60,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="credits",Description="Credits",ColumnWidth=60,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="balance",Description="Balance",ColumnWidth=60,ItemOrder=++i});
					break;
				#endregion StatementMainGrid
				#region FamilyRecallGrid
				case DisplayFieldCategory.FamilyRecallGrid:
					listDisplayFields.Add(new DisplayField("Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Due Date",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sched Date",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Notes",255,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Previous Date",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Interval",80,displayFieldCategory));
					break;
				#endregion FamilyRecallGrid
				#region AppointmentEdit
				case DisplayFieldCategory.AppointmentEdit:
					listDisplayFields.Add(new DisplayField("Stat",35,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",45,displayFieldCategory));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						listDisplayFields.Add(new DisplayField("Code",125,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Tth",25,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Surf",50,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Description",275,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Abbreviation",80,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Layman's Term",100,displayFieldCategory));
					break;
				#endregion AppointmentEdit
				#region PlannedAppointmentEdit
				case DisplayFieldCategory.PlannedAppointmentEdit:
					listDisplayFields.Add(new DisplayField("Stat",35,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",45,displayFieldCategory));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						listDisplayFields.Add(new DisplayField("Code",125,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Tth",25,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Surf",50,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Description",275,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField(InternalNames.PlannedAppointmentEdit.Abbreviation,80,displayFieldCategory));
					break;
				#endregion PlannedAppointmentEdit
				#region OutstandingInsClaimsReport
				case DisplayFieldCategory.OutstandingInsReport:
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Carrier",145,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Phone",95,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Carrier",200,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Phone",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Type",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("User",60,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("PatName",115,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Clinic",65,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("PatName",120,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("DateService",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateSent",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateSentOrig",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("TrackStat",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateStat",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Error",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("GroupNum",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("GroupName",100,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("SubName",120,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("SubDOB",65,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("SubID",65,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("PatDOB",65,displayFieldCategory));
					break;
				#endregion OutstandingInsClaimsReport
				#region CEMTSearchPatients
				case DisplayFieldCategory.CEMTSearchPatients:
					listDisplayFields.Add(new DisplayField("Conn",167,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatNum",64,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LName",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("FName",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SSN",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatStatus",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("State",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",167,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Email",190,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ChartNum",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Country",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("MI",25,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Pref Name",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Wk Phone",94,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Bill Type",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Pri Prov",85,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Birthdate",70,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Site",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Wireless Ph",94,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Sec Prov",85,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("LastVisit",70,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("NextVisit",70,displayFieldCategory));
					break;
				#endregion CEMTSearchPatients
				#region A/R Manager Unsent/Excluded Grids
				case DisplayFieldCategory.ArManagerUnsentGrid:
				case DisplayFieldCategory.ArManagerExcludedGrid:
					listDisplayFields.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?140:240,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Clinic",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",83,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("0-30 Days",73,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("31-60 Days",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("61-90 Days",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("> 90 Days",73,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Total",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("-Ins Est",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("=Patient",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PayPlan Due",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Paid",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateTime Suspended",135,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Last Proc",65,displayFieldCategory));
					break;
				#endregion A/R Manager Unsent/Excluded Grids
				#region A/R Manager Sent Grid
				case DisplayFieldCategory.ArManagerSentGrid:
					listDisplayFields.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?150:250,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Clinic",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("0-30 Days",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("31-60 Days",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("61-90 Days",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("> 90 Days",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Total",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("-Ins Est",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("=Patient",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PayPlan Due",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Paid",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Demand Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Transaction",184,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Last Proc",65,displayFieldCategory));
					break;
				#endregion A/R Manager Sent Grid
				#region Statement Limited Custom SuperFamily
				case DisplayFieldCategory.LimitedCustomStatement:
					listDisplayFields.Add(new DisplayField("Date",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Guarantor",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",40,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",46,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",26,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",270,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Charges",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Credits",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Signed",60,displayFieldCategory));
					//listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));
					break;
				#endregion Statement Limited Custom SuperFamily
				#region SuperFamily Grid Columns
				case DisplayFieldCategory.SuperFamilyGridCols:
					listDisplayFields.Add(new DisplayField("Name",280,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stmt",50,displayFieldCategory));
					break;
				#endregion SuperFamily Grid Columns
				default:
					break;
			}
			return listDisplayFields;
		}

		public static List<DisplayField> GetAllAvailableList(DisplayFieldCategory displayFieldCategory){
			Meth.NoCheckMiddleTierRole(); 
			List<DisplayField> listDisplayFields=new List<DisplayField>();
			switch (displayFieldCategory) {
				#region 'None'
				case DisplayFieldCategory.None://Currently only used for ChartViews
					listDisplayFields.Add(new DisplayField("Date",67,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Time",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Th",27,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Dx",28,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",218,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stat",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",42,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",48,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Proc Code",62,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("User",62,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Signed",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",44,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Date TP",67,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Date Entry",67,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prognosis",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Length",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Abbr",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Locked",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("HL7 Sent",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Attachment",55,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("ClinicDesc",100,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					}
					break;
				#endregion 'None'
				#region PatientSelect
				case DisplayFieldCategory.PatientSelect:
					listDisplayFields.Add(new DisplayField("LastName",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("First Name",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("MI",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pref Name",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",30,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SSN",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wk Phone",90,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {//if for OD HQ
						listDisplayFields.Add(new DisplayField("OtherPhone",90,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Country",90,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("RegKey",150,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("PatNum",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ChartNum",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Bill Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("State",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Prov",85,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Birthdate",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Site",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Email",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Ph",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Prov",85,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LastVisit",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("NextVisit",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Invoice Number",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Specialty",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Ward",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("AdmitDate",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DischargeDate",100,displayFieldCategory));
					break;
				#endregion PatientSelect
				#region PatientInformation
				case DisplayFieldCategory.PatientInformation:
					listDisplayFields.Add(new DisplayField("Last",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("First",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Middle",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred Pronoun",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Title",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Salutation",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Gender",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Position",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Birthdate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SS#",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address2",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("State",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
						listDisplayFields.Add(new DisplayField("Country",0,displayFieldCategory));
					}
					if(PrefC.IsODHQ) {
						listDisplayFields.Add(new DisplayField("Tax Address",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Zip",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wk Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Ph",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("E-mail",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact Method",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ABC0",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Chart Num",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Ward",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("AdmitDate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DischargeDate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Primary Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec. Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Payor Types",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Language",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Clinic",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ResponsParty",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referrals",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Addr/Ph Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Guardians",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Arrive Early",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Super Head",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						listDisplayFields.Add(new DisplayField("References",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Pat Restrictions",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ICE Name",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ICE Phone",0,displayFieldCategory));
					break;
				#endregion PatientInformation
				#region AccountModule
				case DisplayFieldCategory.AccountModule:
					listDisplayFields.Add(new DisplayField("Date",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",46,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",26,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",270,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Charges",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Credits",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Balance",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Signed",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("ClinicDesc",110,displayFieldCategory));
					}
					break;
				#endregion AccountModule
				#region RecallList
				case DisplayFieldCategory.RecallList:
					listDisplayFields.Add(new DisplayField("Due Date",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",120,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",30,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Type",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Interval",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("#Remind",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LastRemind",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact",120,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Status",130,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Note",215,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("BillingType",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("WebSched",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Carrier Name",100,displayFieldCategory));
					break;
				#endregion RecallList
				#region ChartPatientInformation
				case DisplayFieldCategory.ChartPatientInformation:
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Preferred Pronoun",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ABC0",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referred From",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Date First Visit",0,displayFieldCategory));
					if(!PrefC.GetBool(PrefName.EasyHideHospitals)){ //true will hide
						listDisplayFields.Add(new DisplayField("Admit Date",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Discharge Date",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov. (Pri, Sec)",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Ins",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Ins",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Payor Types",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.DistributorKey)) {
						listDisplayFields.Add(new DisplayField("Registration Keys",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Ehr Provider Keys",0,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("References",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Premedicate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Problems",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Med Urgent",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Medical Summary",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Service Notes",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Medications",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Allergies",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Birthdate",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("AskToArriveEarly",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Super Head",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient Portal",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Broken Appts",0,displayFieldCategory));
					if(PrefC.GetBool(PrefName.ShowFeatureEhr)) {
						listDisplayFields.Add(new DisplayField("Tobacco Use",0,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Pat Restrictions",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Specialty",0,displayFieldCategory));
					break;
				#endregion ChartPatientInformation
				#region ProcedureGroupNote
				case DisplayFieldCategory.ProcedureGroupNote:
					listDisplayFields.Add(new DisplayField("Date",67,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Th",27,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",218,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stat",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",42,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",48,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Proc Code",62,displayFieldCategory));
					break;
				#endregion ProcedureGroupNote
				#region TreatmentPlanModule
				case DisplayFieldCategory.TreatmentPlanModule:
					listDisplayFields.Add(new DisplayField("Done",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Surf",45,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sub",28,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",202,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Allowed",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Ins",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Ins",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DPlan",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Discount",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pat",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prognosis",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Dx",28,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));//proc abbr
					listDisplayFields.Add(new DisplayField("Tax Est",50,displayFieldCategory)); //Sales tax
					listDisplayFields.Add(new DisplayField("Prov",50,displayFieldCategory));//provnum
					listDisplayFields.Add(new DisplayField("DateTP",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));//clinicnum
					listDisplayFields.Add(new DisplayField(InternalNames.TreatmentPlanModule.Appt,35,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.TreatmentPlanModule.CatPercUCR,65,displayFieldCategory));
					break;
				#endregion TreatmentPlanModule
				#region OrthoChart
				case DisplayFieldCategory.OrthoChart:
					listDisplayFields=GetForCategory(DisplayFieldCategory.OrthoChart); //The display fields that the user has already saved
					List<string> listDistinctFieldNames=OrthoCharts.GetDistinctFieldNames();
					for(int j=0;j<listDistinctFieldNames.Count;j++){
						if(listDisplayFields.Exists(x => x.Description==listDistinctFieldNames[j])) {
							continue;
						}
						//If there aren't any display fields with the current field name, create one and add it to the list of available fields.
						DisplayField displayField=new DisplayField("",20,DisplayFieldCategory.OrthoChart);
						displayField.IsNew=true;
						displayField.Description=listDistinctFieldNames[j];
						listDisplayFields.Add(displayField);
					}
					break;
				#endregion OrthoChart
				#region AppointmentBubble
				case DisplayFieldCategory.AppointmentBubble:
					listDisplayFields.Add(new DisplayField("Patient Name",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient Picture",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Day",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Date",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Time",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Length",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Provider",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Production",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Net Production",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Confirmed",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ASAP",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Med Flag",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Med Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Lab",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Procedures",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Horizontal Line",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatNum",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ChartNum",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Home Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Work Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Contact Methods",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Insurance",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Insurance Color",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fam Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Appt Mod Note",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ReferralFrom",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ReferralTo",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referral From With Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Referral To With Phone",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Language",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Email",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Discount Plan",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Estimated Patient Portion",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("CareCredit Status",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Verify Insurance",0,displayFieldCategory));
					break;
				#endregion AppointmentBubble
				#region AccountPatientInformation
				case DisplayFieldCategory.AccountPatientInformation:
					listDisplayFields.Add(new DisplayField("Billing Type",0,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatFields",0,displayFieldCategory));
					break;
				#endregion AccountPatientInformation
				#region StatementMainGrid
				case DisplayFieldCategory.StatementMainGrid:
					int i=0;
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="date",Description="Date",ColumnWidth=75,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="prov",Description="Prov",ColumnWidth=60,ItemOrder=++i });
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="patient",Description="Patient",ColumnWidth=100,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="ProcCode",Description="Code",ColumnWidth=45,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="tth",Description="Tooth",ColumnWidth=45,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="description",Description="Description",ColumnWidth=275,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="charges",Description="Charges",ColumnWidth=60,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="credits",Description="Credits",ColumnWidth=60,ItemOrder=++i});
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="invoiceNum",Description="Invoice#",ColumnWidth=60,ItemOrder=++i });
					listDisplayFields.Add(new DisplayField {Category=displayFieldCategory,InternalName="balance",Description="Balance",ColumnWidth=60,ItemOrder=++i});
					break;
				#endregion StatementMainGrid
				#region FamilyRecallGrid
				case DisplayFieldCategory.FamilyRecallGrid:
					listDisplayFields.Add(new DisplayField("Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Due Date",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sched Date",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Notes",255,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Previous Date",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Interval",80,displayFieldCategory));
					break;
				#endregion FamilyRecallGrid
				#region AppointmentEdit
				case DisplayFieldCategory.AppointmentEdit:
					listDisplayFields.Add(new DisplayField("Stat",35,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",45,displayFieldCategory));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						listDisplayFields.Add(new DisplayField("Code",125,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Tth",25,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Surf",50,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Description",255,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Abbreviation",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Layman's Term",100,displayFieldCategory));
					break;
				#endregion AppointmentEdit
				#region PlannedAppointmentEdit
				case DisplayFieldCategory.PlannedAppointmentEdit:
					listDisplayFields.Add(new DisplayField("Stat",35,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Priority",45,displayFieldCategory));
					if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
						listDisplayFields.Add(new DisplayField("Code",125,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Tth",25,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Surf",50,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Code",50,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Description",275,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Fee",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.PlannedAppointmentEdit.Abbreviation,80,displayFieldCategory));
					break;
				#endregion PlannedAppointmentEdit
				#region OutstandingInsClaimsReport
				case DisplayFieldCategory.OutstandingInsReport:
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Carrier",145,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Phone",95,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("Carrier",200,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Phone",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Type",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("User",60,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("PatName",115,displayFieldCategory));
						listDisplayFields.Add(new DisplayField("Clinic",65,displayFieldCategory));
					}
					else {
						listDisplayFields.Add(new DisplayField("PatName",120,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("DateService",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateSent",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateSentOrig",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("TrackStat",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateStat",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Error",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Amount",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("GroupNum",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("GroupName",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SubName",120,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SubDOB",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SubID",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatDOB",65,displayFieldCategory));
					break;
				#endregion OutstandingInsClaimsReport
				#region CEMTSearchPatients
				case DisplayFieldCategory.CEMTSearchPatients:
					listDisplayFields.Add(new DisplayField("Conn",167,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatNum",64,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LName",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("FName",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("SSN",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PatStatus",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Age",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("City",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("State",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Address",167,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Hm Phone",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Email",190,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("ChartNum",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Country",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("MI",25,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pref Name",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wk Phone",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Bill Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Pri Prov",85,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Birthdate",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Site",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Clinic",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Wireless Ph",94,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Sec Prov",85,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("LastVisit",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("NextVisit",70,displayFieldCategory));
					break;
				#endregion CEMTSearchPatients
				#region A/R Manager Unsent/Excluded Grids
				case DisplayFieldCategory.ArManagerUnsentGrid:
				case DisplayFieldCategory.ArManagerExcludedGrid:
					listDisplayFields.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?140:240,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Clinic",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Billing Type",83,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("0-30 Days",73,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("31-60 Days",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("61-90 Days",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("> 90 Days",73,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Total",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("-Ins Est",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("=Patient",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PayPlan Due",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Paid",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("DateTime Suspended",135,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Proc",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.ArManagerUnsentGrid.DateBalBegan,95,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.ArManagerUnsentGrid.DaysBalBegan,95,displayFieldCategory));
					break;
				#endregion A/R Manager Unsent/Excluded Grids
				#region A/R Manager Sent Grid
				case DisplayFieldCategory.ArManagerSentGrid:
					listDisplayFields.Add(new DisplayField("Guarantor",PrefC.HasClinicsEnabled?150:250,displayFieldCategory));
					if(PrefC.HasClinicsEnabled) {
						listDisplayFields.Add(new DisplayField("Clinic",100,displayFieldCategory));
					}
					listDisplayFields.Add(new DisplayField("Prov",75,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("0-30 Days",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("31-60 Days",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("61-90 Days",70,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("> 90 Days",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Total",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("-Ins Est",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("=Patient",55,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("PayPlan Due",80,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Paid",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Demand Type",90,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Transaction",184,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Last Proc",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.ArManagerSentGrid.DateBalBegan,95,displayFieldCategory));
					listDisplayFields.Add(new DisplayField(InternalNames.ArManagerSentGrid.DaysBalBegan,95,displayFieldCategory));
					break;
				#endregion A/R Manager Sent Grid
				#region Statement Limited Custom SuperFamily
				case DisplayFieldCategory.LimitedCustomStatement:
					listDisplayFields.Add(new DisplayField("Date",65,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Patient",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Guarantor",100,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Prov",40,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Clinic",50,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Code",46,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Tth",26,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Description",270,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Charges",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Credits",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Signed",60,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Abbr",110,displayFieldCategory));
					break;
				#endregion Statement Limited Custom SuperFamily
				#region SuperFamily Grid Columns
				case DisplayFieldCategory.SuperFamilyGridCols:
					listDisplayFields.Add(new DisplayField("Name",280,displayFieldCategory));
					listDisplayFields.Add(new DisplayField("Stmt",50,displayFieldCategory));
					List<PatFieldDef> listPatFieldDefs=PatFieldDefs.GetDeepCopy().FindAll(x=>!x.IsHidden);
					for(int j=0;j<listPatFieldDefs.Count;j++) {
						DisplayField displayField=new DisplayField("",100,DisplayFieldCategory.SuperFamilyGridCols);
						displayField.Description=listPatFieldDefs[j].FieldName;
						listDisplayFields.Add(displayField);
					}
					break;
				#endregion SuperFamily Grid Columns
				default:
					break;
			}
			return listDisplayFields;
		}

		public static void SaveListForCategory(List<DisplayField> listDisplayFieldsShowing,DisplayFieldCategory displayFieldCategory){
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDisplayFieldsShowing,displayFieldCategory);
				return;
			}
			bool isDefault=true;
			List<DisplayField> listDisplayFieldsDefault=GetDefaultList(displayFieldCategory);
			if(listDisplayFieldsShowing.Count!=listDisplayFieldsDefault.Count){
				isDefault=false;
			}
			else{
				for(int i=0;i<listDisplayFieldsShowing.Count;i++){
					if(listDisplayFieldsShowing[i].Description!=""){
						isDefault=false;
						break;
					}
					if(listDisplayFieldsShowing[i].InternalName!=listDisplayFieldsDefault[i].InternalName){
						isDefault=false;
						break;
					}
					if(listDisplayFieldsShowing[i].ColumnWidth!=listDisplayFieldsDefault[i].ColumnWidth) {
						isDefault=false;
						break;
					}
				}
			}
			string command="DELETE FROM displayfield WHERE Category="+POut.Long((int)displayFieldCategory);
			Db.NonQ(command);
			if(isDefault){
				return;
			}
			for(int i=0;i<listDisplayFieldsShowing.Count;i++){
				listDisplayFieldsShowing[i].ItemOrder=i;
				Insert(listDisplayFieldsShowing[i]);
			}
		}

		public static void SaveListForChartView(List<DisplayField> listDisplayFieldsShowing,long chartViewNum) {
			if(RemotingClient.MiddleTierRole==MiddleTierRole.ClientMT) {
				Meth.GetVoid(MethodBase.GetCurrentMethod(),listDisplayFieldsShowing,chartViewNum);
				return;
			}
			//Odds are, that if they are creating a custom view, that the fields are not default. If they are default, this code still works.
			string command="DELETE FROM displayfield WHERE ChartViewNum="+POut.Long((long)chartViewNum);
			Db.NonQ(command);
			for(int i=0;i<listDisplayFieldsShowing.Count;i++) {
				listDisplayFieldsShowing[i].ItemOrder=i;
				listDisplayFieldsShowing[i].ChartViewNum=chartViewNum;
				Insert(listDisplayFieldsShowing[i]);
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
