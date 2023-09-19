using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeBase;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace OpenDentBusiness {
	public partial class ConvertDatabases {
		#region Helper Methods
		///<summary>Converts frequency limitation preferences into CodeGroups. Current frequency limitation benefits will be converted into these new code groups.</summary>
		private static void ConvertFrequencyLimitationPreferences() {
			//It is important to have ItemOrder mimic the hard coded values of the old 'benefit.GetFrequencyGroupNum()' method so that the sorting of benefits (comparer) doesn't change by default.
			List<FrequencyLimitationHelper> listFrequencyLimitationHelpers=new List<FrequencyLimitationHelper>();
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenBWCodes","BW",0,codeGroupFixed:1,isQuantityRequired:false,isCovCatAllowed:true));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenExamCodes","Exam",1,codeGroupFixed:3,isQuantityRequired:false,isCovCatAllowed:true));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenPanoCodes","Pano/FMX",2,codeGroupFixed:2,isQuantityRequired:false,isCovCatAllowed:true));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenCancerScreeningCodes","Cancer Screening",3));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenProphyCodes","Prophy",4,codeGroupFixed:5));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenFlourideCodes","Fluoride",5,codeGroupFixed:8));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenSealantCodes","Sealant",6,codeGroupFixed:9));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenCrownCodes","Crown",7));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenSRPCodes","SRP",8,codeGroupFixed:6));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenFullDebridementCodes","Full Debridement",9,codeGroupFixed:7));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenPerioMaintCodes","Perio Maint",10,codeGroupFixed:4));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenDenturesCodes","Dentures",11));
			listFrequencyLimitationHelpers.Add(new FrequencyLimitationHelper("InsBenImplantCodes","Implants",12));
			for(int i=0;i<listFrequencyLimitationHelpers.Count;i++) {
				listFrequencyLimitationHelpers[i].CreateCodeGroup();
				listFrequencyLimitationHelpers[i].ConvertBenefits();
			}
		}

		private class FrequencyLimitationHelper {
			///<summary>This value corresponds to CodeGroup.EnumCodeGroupFixed</summary>
			private int _codeGroupFixed;
			private long _codeGroupNum;
			private string _groupName;
			private bool _isCovCatAllowed;
			private bool _isQuantityRequired;
			private int _itemOrder;
			///<summary>The obsolete PrefName.</summary>
			private string _prefNameOld;
			private string _procCodes;

			#region Enum Values
			private const int _benefitTypeLimitations=5;
			private const int _coverageLevelNone=0;
			private const int _ebenefitCatRoutinePreventive=12;
			private const int _quantityQualifierNumberOfServices=1;
			private const int _quantityQualifierYears=4;
			private const int _quantityQualifierMonths=5;
			private const int _timePeriodNone=0;
			private const int _timePeriodServiceYear=1;
			private const int _timePeriodCalendarYear=2;
			private const int _timePeriodNumberInLast12Months=5;
			#endregion

			public FrequencyLimitationHelper(string prefNameOld,string groupName,int itemOrder,int codeGroupFixed=0,bool isQuantityRequired=true,bool isCovCatAllowed=false) {
				_codeGroupFixed=codeGroupFixed;
				_groupName=groupName;
				_isCovCatAllowed=isCovCatAllowed;
				_isQuantityRequired=isQuantityRequired;
				_itemOrder=itemOrder;
				_prefNameOld=prefNameOld;
			}

			internal void CreateCodeGroup() {
				//Get the ValueString from the preference table which will be used as the ProcCodes value on the codegroup table (both are comma separated).
				_procCodes=Db.GetScalar($"SELECT ValueString FROM preference WHERE PrefName='{_prefNameOld}'");
				//If any prefs are empty, fill them with codes. This covers the "default fallback" hard coded codes that are seen in many places.
				if(string.IsNullOrWhiteSpace(_procCodes) && !CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
					_procCodes=GetDefaultProcCodesForPrefName(_prefNameOld);
				}
				//Insert a new code group into the database and keep track of the InsertID (PK) that the database returns.
				string command=$"INSERT INTO codegroup (GroupName,ProcCodes,ItemOrder,CodeGroupFixed,IsHidden) VALUES ('{_groupName}','{POut.String(_procCodes)}',{_itemOrder},{_codeGroupFixed},0)";
				_codeGroupNum=Db.NonQ(command,true);
			}

			internal void ConvertBenefits() {
				//The following code mimics the exact behavior of ProcedureCodes.GetCodeNumsForPref() when this convert script section was written.
				//This is done so that there is absolutely no change in behavior in the program after converting the preferences into the new codegroup table.
				//E.g. ProcedureCodes.GetCodeNumsForPref() was case-sensitive so the following query will utilize BINARY in order to preserve that behavior.
				//Extract the ProcCode values from the comma separated list in the old preference ValueString column. Trim any white space to mimic ProcedureCodes.GetCodeNumsForPref().
				List<string> listProcCodes=_procCodes.Split(",",StringSplitOptions.RemoveEmptyEntries).Select(x => POut.String(x).Trim()).ToList();
				if(listProcCodes.IsNullOrEmpty()) {
					return;//Nothing to convert.
				}
				//Find all of the CodeNums that exactly match the ProcCodes that were just extracted (case-sensitive).
				List<long> listCodeNums=Db.GetListLong($"SELECT CodeNum FROM procedurecode WHERE BINARY ProcCode IN ('{string.Join("','",listProcCodes)}')");
				//Grab all benefits that do not have a CodeGroupNum set and match all of the criteria for the current FrequencyLimitation being converted.
				//The following code mimics all of the old "Benefits.Is...Frequency()" methods that take in a benefit and check to see if the benefit passed in is an X frequency or not (e.g. IsBitewingFrequency).
				string command=@$"SELECT BenefitNum
					FROM benefit
					WHERE BenefitType = {_benefitTypeLimitations}
					AND MonetaryAmt = -1
					AND Percent = -1
					AND ((QuantityQualifier IN ({_quantityQualifierMonths},{_quantityQualifierYears}) AND TimePeriod={_timePeriodNone})
						OR (QuantityQualifier = {_quantityQualifierNumberOfServices} AND TimePeriod IN ({_timePeriodServiceYear},{_timePeriodCalendarYear},{_timePeriodNumberInLast12Months})))
					AND CoverageLevel = {_coverageLevelNone}";
				//Exam frequency limitation benefits not required to utilize code nums and can also use the RoutinePreventive CovCat.
				//The 'Simplified View' within the Edit Benefits window used to automatically convert the exam frequency limitation benefit over to using RoutinePreventive CovCat.
				//This was simply how the program has always stored this benefit (e.g. as far back as v15.3 when frequency limitations didn't impact insurance estimates).
				//That behavior continued even when frequency limitations started impacting estimates (around v16.4).
				//However, the RoutinePreventive CovCat was purely used as an identifier and was not utilized for the code span associated with it.
				//This convert script uses the RoutinePreventive CovCat as an identifier for exam frequency limitation benefits but ignores the procedure codes associated with the CovCat.
				if(_prefNameOld=="InsBenExamCodes") {
					//Check to see if the RoutinePreventive covcat exists; this logic mimics CovCats.GetForEbenCat()
					DataTable table=Db.GetTable($"SELECT CovCatNum FROM covcat WHERE EbenefitCat={_ebenefitCatRoutinePreventive} AND IsHidden=0 ORDER BY CovOrder LIMIT 1");
					//If there is no RoutinePreventive covcat AND there are no valid CodeNums found from the preference then there is nothing to convert.
					if(table.Rows.Count==0 && listCodeNums.IsNullOrEmpty()) {
						return;//Nothing to convert.
					}
					//Grab benefits that are associated with the RoutinePreventive CovCat OR any of the CodeNums explicitly specified within the preference.
					command+=" AND (";
					if(table.Rows.Count > 0) {
						command+=$"(CodeNum=0 AND CovCatNum={table.Rows[0]["CovCatNum"].ToString()})";
					}
					if(!listCodeNums.IsNullOrEmpty()) {
						if(table.Rows.Count > 0) {
							command+=" OR ";//Allow matches based off of the RoutinePreventive covcat OR the CodeNums specified within the preference.
						}
						command+=$"(CodeNum IN ({string.Join(",",listCodeNums)}))";
					}
					command+=")";
				}
				else {//InsBenExamCodes is the only preference allowed to ignore ProcCodes and utilize CovCats instead. All others are required to match explicit CodeNums.
					if(listCodeNums.IsNullOrEmpty()) {
						return;//Nothing to convert.
					}
					//However, there could be old benefits with both a CovCat and a CodeNum set and only InsBenBWCodes and InsBenPanoCodes had logic that would allow such a scenario.
					if(!_isCovCatAllowed) {
						command+=" AND CovCatNum=0";
					}
					command+=$" AND CodeNum IN ({string.Join(",",listCodeNums)})";
				}
				if(_isQuantityRequired) {
					command+=" AND Quantity!=0";
				}
				List<long> listBenefitNums=Db.GetListLong(command);
				if(listBenefitNums.IsNullOrEmpty()) {
					return;//Nothing to convert.
				}
				//Convert all of the benefits that were just found over to using code groups.
				//Do this by clearing out CodeNum and CovCatNum; then set CodeGroupNum accordingly.
				Db.NonQ($"UPDATE benefit SET CodeNum=0,CovCatNum=0,CodeGroupNum={_codeGroupNum} WHERE BenefitNum IN ({string.Join(",",listBenefitNums)})");
			}

			private string GetDefaultProcCodesForPrefName(string prefName) {
				switch(prefName) {
					//Default codes below this point were found in To16_4_19()
					case "InsBenBWCodes":
						return "D0272,D0274";
					case "InsBenExamCodes":
						return "D0120,D0150";
					case "InsBenPanoCodes":
						return "D0210,D0330";
					//Default codes below this point were found in To18_2_1()
					case "InsBenCancerScreeningCodes":
						return "D0431";
					case "InsBenProphyCodes":
						return "D1110,D1120";
					case "InsBenFlourideCodes":
						return "D1206,D1208";
					case "InsBenSealantCodes":
						return "D1351";
					case "InsBenCrownCodes":
						return "D2740,D2750,D2751,D2752,D2780,D2781,D2782,D2783,D2790,D2791,D2792,D2794";
					case "InsBenSRPCodes":
						return "D4341,D4342";
					case "InsBenFullDebridementCodes":
						return "D4355";
					case "InsBenPerioMaintCodes":
						return "D4910";
					case "InsBenDenturesCodes":
						return "D5110,D5120,D5130,D5140,D5211,D5212,D5213,D5214,D5221,D5222,D5223,D5224,D5225,D5226";
					case "InsBenImplantCodes":
						return "D6010";
					default:
						return "";
				}
			}
		}
		#endregion

		private static void To23_2_1() {
			string strUpgrading="Upgrading database to version: 23.2.1";
			string command;
			DataTable table;
			//-----------------------------------------------------------------------------------------------------
			//41732 - Insert PracticeBooster bridge----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'PracticeBooster', " 
				 +"'PracticeBooster from practicebooster.com', " 
				 +"'0', " 
				 +"'"+POut.String(@"https://practicebooster.com/login.asp")+"', "
				 +"'', "//leave blank if none 
				 +"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'7', "//ToolBarsAvail.MainToolbar
				 +"'PracticeBooster')"; 
			Db.NonQ(command);
			//end PracticeBooster bridge
			//B44132 Adding missing alert links for the 'All' option
			List<int> listAlertTypes=new List<int> {17,18,19,23,25,31,33,34,35,36,37,38};
			for (int i = 0; i < listAlertTypes.Count; i++) {
				command=$"SELECT AlertCategoryLinkNum FROM alertcategorylink WHERE AlertCategoryNum=1 AND AlertType={listAlertTypes[i]}";
				long alertCategoryLinkNum=PIn.Long(Db.GetScalar(command));
				if (alertCategoryLinkNum < 1 ) { 
					command=$"INSERT INTO alertcategorylink(AlertCategoryNum,AlertType) VALUES(1, {listAlertTypes[i]});"; 
					Db.NonQ(command);
				}
			}//end B44132
			//F44562 adding permissions for audit trail viewing
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",247)";//Permission is ViewAppointmentAuditTrail
				Db.NonQ(command);
			}//End F44562
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RedirectShortURLsFromHQ','')";
			Db.NonQ(command);
			//Insert JazzClassicCapture bridge-----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'JazzClassicCapture', " 
				 +"'Jazz Classic Capture', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Program Files\Jazz Imaging LLC\Jazz Classic\Classic.exe")+"', " 
				 +"'"+POut.String(@"/C")+"', " 
				 +"'')"; 
			programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'XML output file path', " 
				 +"'C:\\\\Program Files\\\\Jazz Imaging LLC\\\\Jazz Classic\\\\OpenDentalPatientInfo.xml')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'2', "//ToolBarsAvail.ChartModule 
				 +"'Capture')"; 
			Db.NonQ(command); 
			//end JazzClassicCapture bridge 
			//Insert JazzClassicExamView bridge----------------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'JazzClassicExamView', " 
				 +"'Jazz Classic Exam View', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Program Files\Jazz Imaging LLC\Jazz Classic\Classic.exe")+"', " 
				 +"'"+POut.String(@"/E")+"', " 
				 +"'')"; 
			programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'XML output file path', " 
				 +"'C:\\\\Program Files\\\\Jazz Imaging LLC\\\\Jazz Classic\\\\OpenDentalPatientInfo.xml')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'2', "//ToolBarsAvail.ChartModule 
				 +"'View Exam')"; 
			Db.NonQ(command); 
			//end JazzClassicExamView bridge 
			//Insert JazzClassicPatientUpdate bridge-----------------------------------------------------------------
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'JazzClassicPatientUpdate', " 
				 +"'Jazz Classic Patient Update', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Program Files\Jazz Imaging LLC\Jazz Classic\Classic.exe")+"', " 
				 +"'"+POut.String(@"/P")+"', " 
				 +"'')"; 
			programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'XML output file path', " 
				 +"'C:\\\\Program Files\\\\Jazz Imaging LLC\\\\Jazz Classic\\\\OpenDentalPatientInfo.xml')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'7', "//ToolBarsAvail.MainToolbar 
				 +"'Classic Patient Update')"; 
			Db.NonQ(command); 
			//end JazzClassicPatientUpdate bridge 
			//Set the default image for all three Jazz Classic bridges.
			command="UPDATE program "
				+"SET ButtonImage='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" +
				"71X00NgAAAABJRU5ErkJggg==' "
				+"WHERE ProgName IN ('JazzClassicCapture','JazzClassicExamView','JazzClassicPatientUpdate')";
			Db.NonQ(command);
			//Insert Shining3D bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'Shining3D', " 
				 +"'Shining3D from https://www.shining3d.com', " 
				 +"'0', " 
				 +"'"+POut.String(@"D:\DentalLauncher\IntraoralScan\Bin\DentalLauncher.exe")+"', " 
				 +"'"+POut.String(@"--source ""OpenDental"" --pName ""[NameFL]"" --pAge ""[BirthDate_yyyyMMdd]"" --pGender ""[Gender]""")+"', "
				 +"'')"; 
			programNum=Db.NonQ(command,true); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'7', "//ToolBarsAvail.MainToolbar 
				 +"'IntraoralScan')"; 
			Db.NonQ(command);
			//end Shining3D bridge 
			//Frequency Limitations--------------------------------------------------------------------------------
			command="DROP TABLE IF EXISTS codegroup";
			Db.NonQ(command);
			command=@"CREATE TABLE codegroup (
				CodeGroupNum bigint NOT NULL auto_increment PRIMARY KEY,
				GroupName varchar(50) NOT NULL,
				ProcCodes text NOT NULL,
				ItemOrder int NOT NULL,
				CodeGroupFixed tinyint NOT NULL,
				IsHidden tinyint NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,strUpgrading+" - Adding benefit.CodeGroupNum");
			//Add a new column to the benefit table named "CodeGroupNum" as an FK to codegroup.CodeGroupNum.
			command="ALTER TABLE benefit ADD CodeGroupNum bigint NOT NULL, ADD INDEX (CodeGroupNum)";
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,strUpgrading+" - Frequency Limitations");
			ConvertFrequencyLimitationPreferences();
			ODEvent.Fire(ODEventType.ConvertDatabases,strUpgrading);
			//I45000 SkySQL - Read Only Server
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ReadOnlyServerSslCa','')";
			Db.NonQ(command);
			//end I45000
			//I44628 - Permission to select archived patients
			//Add permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",249)"; //249 - ArchivedPatientSelect
				Db.NonQ(command);
			}
			//End I44628
		}//End of 23_2_1() method

		private static void To23_2_5(){
			string command="DROP TABLE IF EXISTS orthochartlog";
			Db.NonQ(command);
			command=@"CREATE TABLE orthochartlog (
				OrthoChartLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				ComputerName varchar(255) NOT NULL,
				DateTimeLog datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeService datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				UserNum bigint NOT NULL,
				ProvNum bigint NOT NULL,
				OrthoChartRowNum bigint NOT NULL,
				LogData MEDIUMTEXT NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoChartLoggingOn','0')";
			Db.NonQ(command);
		}//End of 23_2_5() method

		private static void To23_2_9(){
			//E46260
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('EraChkPaymentType','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAchPaymentType','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraFwtPaymentType','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraDefaultPaymentType','0')";
			Db.NonQ(command);
			//End of E46260
		}//End of To23_2_9() method

		private static void To23_2_19(){
			//I44056 adding a new table called payplantemplate
			string command="DROP TABLE IF EXISTS payplantemplate";
			Db.NonQ(command);
			command=@"CREATE TABLE payplantemplate (
				PayPlanTemplateNum bigint NOT NULL auto_increment PRIMARY KEY,
				PayPlanTemplateName varchar(255) NOT NULL,
				ClinicNum bigint NOT NULL,
				APR double NOT NULL,
				InterestDelay int NOT NULL,
				PayAmt double NOT NULL,
				NumberOfPayments int NOT NULL,
				ChargeFrequency tinyint NOT NULL,
				DownPayment double NOT NULL,
				DynamicPayPlanTPOption tinyint NOT NULL,
				Note varchar(255) NOT NULL,
				IsHidden tinyint NOT NULL,
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//End I44056
		}//End of To23_2_19() method

	}
}