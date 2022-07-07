using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CodeBase;
using DataConnectionBase;
using static OpenDentBusiness.LargeTableHelper;//so you don't have to type the class name each time.

namespace OpenDentBusiness {
	public partial class ConvertDatabases {

		#region Helper Methods
		private static bool IsUsingReplication() {
			string command="SELECT COUNT(*) FROM replicationserver";
			if(Db.GetCount(command)!="0") {
				return true;
			}
			command="SHOW MASTER STATUS ";
			DataTable tableReplicationMasterStatus=Db.GetTable(command);
			command="SHOW SLAVE STATUS";
			DataTable tableSlaveStatus=Db.GetTable(command);
			if(tableReplicationMasterStatus.Rows.Count > 0 || tableSlaveStatus.Rows.Count > 0) {
				return true;
			}
			//Last check Galera cluster (NADG)
			command="SHOW GLOBAL VARIABLES LIKE '%wsrep_on%' ";
			tableSlaveStatus=Db.GetTable(command);
			for(int i = 0;i<tableSlaveStatus.Rows.Count;i++) {
				DataRow row=tableSlaveStatus.Rows[i];
				if(row["wsrep_on"]!=null && PIn.String(row["wsrep_on"].ToString())=="ON") {
					command=$"SELECT COUNT(DISTINCT wcm.node_uuid) ";
					command+="FROM mysql.wsrep_cluster wc ";
					command+="INNER JOIN mysql.wsrep_cluster_members wcm ON wc.cluster_uuid=wcm.cluster_uuid ";
					int count=Db.GetInt(command);
					if(count>0) {
						return true;
					}
				}
			}
			return false;
		}

		///<summary>Removing duplicate proc codes from frequency limitation preferences.
		///B36962 - We are removing duplicate procCodes from frequency limitation preferences.
		///Codes that were in more than one category was causing bugs in ControlFamily.cs, FormInsBenefits.cs, and FormBenefitFrequencies.cs where the 
		///forms would be filled incorrectly because of overlapping benefits in a category or if a benefit was already found in a previous category, 
		///it would be entirely ignored in the other categories. Benefits also get saved to the db based on what is populated in the two forms 
		///since they are textboxes/comboboxes that are not associated with a benefit and only populated by the benefit last in. 
		///Removing duplicate procCodes helps eliminate this issue and code has been written as part of this job in UserControlTreatPlanFreqLimit.cs 
		///that blocks users from entering a procCode more than once.</summary>
		private static void RemoveDuplicateProcCodesFromFrequencyLimitationPreferences() {
			string command;
			//Grab all frequency limitation preferences and split them into a list of procCodes. 
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenBWCodes'";
			List<string> listInsBenBWCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenPanoCodes'";
			List<string> listInsBenPanoCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenExamCodes'";
			List<string> listInsBenExamCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenCancerScreeningCodes'";
			List<string> listInsBenCancerScreeningCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenProphyCodes'";
			List<string> listInsBenProphyCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenFlourideCodes'";
			List<string> listInsBenFlourideCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenSealantCodes'";
			List<string> listInsBenSealantCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenCrownCodes'";
			List<string> listInsBenCrownCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenSRPCodes'";
			List<string> listInsBenSRPCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenFullDebridementCodes'";
			List<string> listInsBenFullDebridementCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenPerioMaintCodes'";
			List<string> listInsBenPerioMaintCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenDenturesCodes'";
			List<string> listInsBenDenturesCodes=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='InsBenImplantCodes'";
			List<string> listInsBenImplantCodes=Db.GetScalar(command).Split(',').ToList();
			List<string> listProcCodes=listInsBenBWCodes
				.Concat(listInsBenPanoCodes)
				.Concat(listInsBenExamCodes)
				.Concat(listInsBenCancerScreeningCodes)
				.Concat(listInsBenProphyCodes)
				.Concat(listInsBenFlourideCodes)
				.Concat(listInsBenSealantCodes)
				.Concat(listInsBenCrownCodes)
				.Concat(listInsBenSRPCodes)
				.Concat(listInsBenFullDebridementCodes)
				.Concat(listInsBenPerioMaintCodes)
				.Concat(listInsBenDenturesCodes)
				.Concat(listInsBenImplantCodes)
				.ToList();
			//GroupBy procCode, if any group has more than 1 code then it is currently in multiple preferences.
			List<string> listProcCodesDuplicates=listProcCodes.GroupBy(x => x).Where(x => x.Count()>1).Select(x => x.Key).ToList();
			if(listProcCodesDuplicates.IsNullOrEmpty()) {//No duplicates found, kick out
				return;
			}
			List<string> listPreferenceCategories=new List<string>();//These are all prefnames in the db.
			//Loop through every duplicate procCode, for any preference it is currently in add that PrefName to listPreferencesCategories.
			//We only need to consider preferences with the codes from listProcCodeDuplicates.
			for(int i=0;i<listProcCodesDuplicates.Count;i++) {
				string procCode=listProcCodesDuplicates[i];
				if(listInsBenBWCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenBWCodes");
				}
				if(listInsBenPanoCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenPanoCodes");
				}
				if(listInsBenExamCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenExamCodes");
				}
				if(listInsBenCancerScreeningCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenCancerScreeningCodes");
				}
				if(listInsBenProphyCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenProphyCodes");
				}
				if(listInsBenFlourideCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenFlourideCodes");
				}
				if(listInsBenSealantCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenSealantCodes");
				}
				if(listInsBenCrownCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenCrownCodes");
				}
				if(listInsBenSRPCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenSRPCodes");
				}
				if(listInsBenFullDebridementCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenFullDebridementCodes");
				}
				if(listInsBenPerioMaintCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenPerioMaintCodes");
				}
				if(listInsBenDenturesCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenDenturesCodes");
				}
				if(listInsBenImplantCodes.Contains(procCode)) {
					listPreferenceCategories.Add("InsBenImplantCodes");
				}
			}
			//Distinct the category list, only need PrefNames once to loop through.
			listPreferenceCategories=listPreferenceCategories.Distinct().ToList();
			List<string> listDefaultCodes=new List<string>();
			//Loop through all the preferences we found. This loop grabs the default codes for the current preference. 
			//If a code is found to be part of a default list of procCodes for a given preference, we will prefer to leave that procCode in the default and remove it from
			//the other preferences.
			for(int i=0;i<listPreferenceCategories.Count;i++) {
				//Default codes were found via previous convert scripts. 
				switch(listPreferenceCategories[i]) {
					case "InsBenBWCodes":
						listDefaultCodes=new List<string> { "D0272","D0274" };//Found in To16_4_19()
						break;
					case "InsBenPanoCodes":
						listDefaultCodes=new List<string> { "D0210","D0330" };//Found in To16_4_19()
						break;
					case "InsBenExamCodes":
						listDefaultCodes=new List<string> { "D0120","D0150" };//Found in To16_4_19()
						break;
					case "InsBenCancerScreeningCodes":
						listDefaultCodes=new List<string> { "D0431" };//Found in To18_2_0()
						break;
					case "InsBenProphyCodes":
						listDefaultCodes=new List<string> { "D1110","D1120" };//Found in To18_2_0()
						break;
					case "InsBenFlourideCodes":
						listDefaultCodes=new List<string> { "D1206","D1208" };//Found in To18_2_0()
						break;
					case "InsBenSealantCodes":
						listDefaultCodes=new List<string> { "D1351" };//Found in To18_2_0()
						break;
					case "InsBenCrownCodes":
						listDefaultCodes=new List<string> { "D2740","D2750","D2751","D2752","D2780","D2781","D2782","D2783","D2790","D2791","D2792","D2794" };//Found in To18_2_0()
						break;
					case "InsBenSRPCodes":
						listDefaultCodes=new List<string> { "D4341","D4342" };//Found in To18_2_0()
						break;
					case "InsBenFullDebridementCodes":
						listDefaultCodes=new List<string> { "D4355" };//Found in To18_2_0()
						break;
					case "InsBenPerioMaintCodes":
						listDefaultCodes=new List<string> { "D4910" };//Found in To18_2_0()
						break;
					case "InsBenDenturesCodes":
						listDefaultCodes=new List<string> { "D5110","D5120","D5130","D5140","D5211","D5212","D5213","D5214","D5221","D5222","D5223","D5224","D5225","D5226" };//Found in To18_2_0()
						break;
					case "InsBenImplantCodes":
						listDefaultCodes=new List<string> { "D6010" };//Found in To18_2_0()
						break;
					default:
						break;
				}
				//Loop through the list of duplicate procCodes. Looping backwards so we can remove at the current index.
				//Removing procCodes here means the procCode was contained in the default code and has been removed from all other preferences.
				for(int j=listProcCodesDuplicates.Count()-1;j>=0;j--) {
					//Current procCode is contained in the current preference's default list.
					if(listDefaultCodes.Contains(listProcCodesDuplicates[j])) {
						//Loop through the list of preference names again. Inside this loop we are removing the current procCode from all lists.
						for(int k=0;k<listPreferenceCategories.Count();k++) {
							//If these are equal, the inner loop of preference names is currently on the preference where the code was found as a default.
							//We don't want to remove the procCode from the current preference, continue to next preference.
							if(listPreferenceCategories[i]==listPreferenceCategories[k]) {
								continue;
							}
							//Remove code from list for the current preference.
							switch(listPreferenceCategories[k]) {
								case "InsBenBWCodes":
									listInsBenBWCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenPanoCodes":
									listInsBenPanoCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenExamCodes":
									listInsBenExamCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenCancerScreeningCodes":
									listInsBenCancerScreeningCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenProphyCodes":
									listInsBenProphyCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenFlourideCodes":
									listInsBenFlourideCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenSealantCodes":
									listInsBenSealantCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenCrownCodes":
									listInsBenCrownCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenSRPCodes":
									listInsBenSRPCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenFullDebridementCodes":
									listInsBenFullDebridementCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenPerioMaintCodes":
									listInsBenPerioMaintCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenDenturesCodes":
									listInsBenDenturesCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenImplantCodes":
									listInsBenImplantCodes.Remove(listProcCodesDuplicates[j]);
									break;
								default:
									break;
							}
						}
						//ProcCode has been processed, remove from list so we don't process it again later.
						listProcCodesDuplicates.RemoveAt(j);
					}
				}
			}
			//Loop through all remaining procCodes. These codes are not contained in any duplicate code so we will leave the procCode in the first preference we see.
			//Loop through the preference categories again. Grab the list of procCodes for the current preference.
			//These lists may have been modified from the previous loop.
			for(int i=0;i<listPreferenceCategories.Count;i++) {
				List<string> listProcCodesForPrefCur=new List<string>();
				switch(listPreferenceCategories[i]) {
					case "InsBenBWCodes":
						listProcCodesForPrefCur=listInsBenBWCodes;
						break;
					case "InsBenPanoCodes":
						listProcCodesForPrefCur=listInsBenPanoCodes;
						break;
					case "InsBenExamCodes":
						listProcCodesForPrefCur=listInsBenExamCodes;
						break;
					case "InsBenCancerScreeningCodes":
						listProcCodesForPrefCur=listInsBenCancerScreeningCodes;
						break;
					case "InsBenProphyCodes":
						listProcCodesForPrefCur=listInsBenProphyCodes;
						break;
					case "InsBenFlourideCodes":
						listProcCodesForPrefCur=listInsBenFlourideCodes;
						break;
					case "InsBenSealantCodes":
						listProcCodesForPrefCur=listInsBenSealantCodes;
						break;
					case "InsBenCrownCodes":
						listProcCodesForPrefCur=listInsBenCrownCodes;
						break;
					case "InsBenSRPCodes":
						listProcCodesForPrefCur=listInsBenSRPCodes;
						break;
					case "InsBenFullDebridementCodes":
						listProcCodesForPrefCur=listInsBenFullDebridementCodes;
						break;
					case "InsBenPerioMaintCodes":
						listProcCodesForPrefCur=listInsBenPerioMaintCodes;
						break;
					case "InsBenDenturesCodes":
						listProcCodesForPrefCur=listInsBenDenturesCodes;
						break;
					case "InsBenImplantCodes":
						listProcCodesForPrefCur=listInsBenImplantCodes;
						break;
					default:
						break;
				}
				//Loop through remaining duplicate procedure codes.
				for(int j=listProcCodesDuplicates.Count()-1;j>=0;j--) {
					//If the current preference contains the current procCode, remove the procCode from all other preferences.
					if(listProcCodesForPrefCur.Contains(listProcCodesDuplicates[j])) {
						//Loop through the list of preference names again. Inside this loop we are removing the current procCode from all lists.
						for(int k=0;k<listPreferenceCategories.Count();k++) {
							//If these are equal, the inner loop of preference names is currently on the preference where the code was found as a default.
							//We don't want to remove the procCode from the current preference, continue to next preference.
							if(listPreferenceCategories[i]==listPreferenceCategories[k]) {
								continue;
							}
							switch(listPreferenceCategories[k]) {
								case "InsBenBWCodes":
									listInsBenBWCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenPanoCodes":
									listInsBenPanoCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenExamCodes":
									listInsBenExamCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenCancerScreeningCodes":
									listInsBenCancerScreeningCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenProphyCodes":
									listInsBenProphyCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenFlourideCodes":
									listInsBenFlourideCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenSealantCodes":
									listInsBenSealantCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenCrownCodes":
									listInsBenCrownCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenSRPCodes":
									listInsBenSRPCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenFullDebridementCodes":
									listInsBenFullDebridementCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenPerioMaintCodes":
									listInsBenPerioMaintCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenDenturesCodes":
									listInsBenDenturesCodes.Remove(listProcCodesDuplicates[j]);
									break;
								case "InsBenImplantCodes":
									listInsBenImplantCodes.Remove(listProcCodesDuplicates[j]);
									break;
								default:
									break;
							}
						}
					}
				}
			}
			//Looping through each preference one more time, this time to update the database with the modified preferences.
			for(int i=0;i<listPreferenceCategories.Count();i++) {
				//Get the list of procCodes we are updating for the current preference.
				List<string> listToUpdate=new List<string>();
				switch(listPreferenceCategories[i]) {
					case "InsBenBWCodes":
						listToUpdate=listInsBenBWCodes;
						break;
					case "InsBenPanoCodes":
						listToUpdate=listInsBenPanoCodes;
						break;
					case "InsBenExamCodes":
						listToUpdate=listInsBenExamCodes;
						break;
					case "InsBenCancerScreeningCodes":
						listToUpdate=listInsBenCancerScreeningCodes;
						break;
					case "InsBenProphyCodes":
						listToUpdate=listInsBenProphyCodes;
						break;
					case "InsBenFlourideCodes":
						listToUpdate=listInsBenFlourideCodes;
						break;
					case "InsBenSealantCodes":
						listToUpdate=listInsBenSealantCodes;
						break;
					case "InsBenCrownCodes":
						listToUpdate=listInsBenCrownCodes;
						break;
					case "InsBenSRPCodes":
						listToUpdate=listInsBenSRPCodes;
						break;
					case "InsBenFullDebridementCodes":
						listToUpdate=listInsBenFullDebridementCodes;
						break;
					case "InsBenPerioMaintCodes":
						listToUpdate=listInsBenPerioMaintCodes;
						break;
					case "InsBenDenturesCodes":
						listToUpdate=listInsBenDenturesCodes;
						break;
					case "InsBenImplantCodes":
						listToUpdate=listInsBenImplantCodes;
						break;
					default:
						break;
				}
				//Update preference.
				command="UPDATE preference SET ValueString='"+POut.String(String.Join(",",listToUpdate))+"' WHERE PrefName='"+POut.String(listPreferenceCategories[i])+"'";
				Db.NonQ(command);
			}
		}
		#endregion

		private static void To20_5_1() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS imagingdevice";
			Db.NonQ(command);
			command=@"CREATE TABLE imagingdevice (
				ImagingDeviceNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				ComputerName varchar(255) NOT NULL,
				DeviceType tinyint NOT NULL,
				TwainName varchar(255) NOT NULL,
				ItemOrder int NOT NULL,
				ShowTwainUI tinyint NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS eservicelog";
			Db.NonQ(command);
			command=@"CREATE TABLE eservicelog (
				EServiceLogNum bigint NOT NULL auto_increment PRIMARY KEY,
				EServiceCode tinyint NOT NULL,
				EServiceOperation tinyint NOT NULL,
				LogDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				AptNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				PayNum bigint NOT NULL,
				SheetNum bigint NOT NULL,
				INDEX(EServiceCode),
				INDEX(EServiceOperation),
				INDEX(AptNum),
				INDEX(PatNum),
				INDEX(PayNum),
				INDEX(SheetNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD RotateOnAcquire int NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD ToothNumbers varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE wikilistheaderwidth ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procedurecode ADD PaintText varchar(255) NOT NULL";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintText='W' WHERE PaintType=15";//ToothPaintingType.Text
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseExactMatchPhone'";
			//This preference might have already been added in 20.2.49.
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhone','0')";
				Db.NonQ(command);
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseExactMatchPhoneNumDigits','10')";
				Db.NonQ(command);
			}
			command="INSERT INTO alertcategory (IsHQCategory,InternalName,Description) VALUES(1,'SupplementalBackups','Supplemental Backups')";
			long alertCatNum=Db.NonQ(command, true);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="SELECT UserNum,ClinicNum FROM alertsub WHERE AlertCategoryNum=1";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				command=$@"INSERT INTO alertsub (UserNum,ClinicNum,AlertCategoryNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["UserNum"].ToString()))},
						{POut.Long(PIn.Long(table.Rows[i]["ClinicNum"].ToString()))},
						{POut.Long(alertCatNum)}
					)";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SameForFamilyCheckboxesUnchecked','0')";
			Db.NonQ(command);
			command="UPDATE procedurecode SET PaintType=17 WHERE ProcCode IN('D1510','D1516','D1517')";//17=SpaceMaintainer
			Db.NonQ(command);
			command="UPDATE procedurecode SET TreatArea=7 WHERE ProcCode IN('D1516','D1517')";//7=ToothRange for bilateral
			Db.NonQ(command);
			AlterTable("toothinitial","ToothInitialNum",new ColNameAndDef("DrawText","varchar(255) NOT NULL"));
			command="INSERT INTO preference(PrefName,ValueString) VALUES('IncomeTransfersTreatNegativeProductionAsIncome','1')";
			Db.NonQ(command);
			command="ALTER TABLE mount ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE document ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='EnterpriseAllowRefreshWhileTyping'";
			//This preference might have already been added in 20.3.41
			if(Db.GetScalar(command)=="0") {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseAllowRefreshWhileTyping','1')";
				Db.NonQ(command);
			}
			command="ALTER TABLE activeinstance ADD ConnectionType tinyint(4) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SaveDXCSOAPAsXML','0')";
			Db.NonQ(command);		
			command="ALTER TABLE clinic ADD TimeZone varchar(75) NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('WebFormsDownloadAlertFrequency','3600000')";//1 hour in ms
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
			alertCatNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},30)";//30=WebformsReady
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AutoImportFolder','')";
			Db.NonQ(command);
			try {
				command="SELECT * FROM userod";
				table=Db.GetTable(command);
				command="SELECT ValueString FROM preference WHERE PrefName='DomainObjectGuid'";
				string domainObjectGuid=Db.GetScalar(command);
				for(int i=0;i<table.Rows.Count;i++) {
					if(!table.Rows[i]["DomainUser"].ToString().IsNullOrEmpty()) {
						string newDomainUser=domainObjectGuid+"\\"+table.Rows[i]["DomainUser"].ToString();
						command="UPDATE userod ";
						command+="SET DomainUser='"+POut.String(newDomainUser)+"' ";
						command+="WHERE UserNum="+POut.String(table.Rows[i]["UserNum"].ToString());
						Db.NonQ(command);
					}
				}
			}
			catch(Exception ex) {
				ex.DoNothing();
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowIncomeTransferManager','1')";
			Db.NonQ(command);	
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimPayByTotalSplitsAuto','1')";
			Db.NonQ(command);
			command="SELECT valuestring FROM preference WHERE PrefName='SheetsDefaultStatement'";
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultReceipt','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultInvoice','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);	
			command=$@"INSERT INTO preference(PrefName,ValueString) VALUES('SheetsDefaultLimited','{POut.Long(sheetDefNumDefault)}')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",200)";//200 - FormAdded, Used for logging form creation in EClipboard.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDefaultProvider','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('SalesTaxDoAutomate','0')";
			Db.NonQ(command);
			command="ALTER TABLE program ADD CustErr varchar(255) NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS cert";
			Db.NonQ(command);
			command=@"CREATE TABLE cert (
				CertNum bigint NOT NULL auto_increment PRIMARY KEY,
				Description varchar(255) NOT NULL,
				WikiPageLink varchar(255) NOT NULL,
				ItemOrder int NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
			command=@"CREATE TABLE certlinkcategory (
				CertLinkCategoryNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				CertCategoryNum bigint NOT NULL,
				INDEX(CertNum),
				INDEX(CertCategoryNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",201)";//201 - Used to restrict access to Image Exporting when necessary.
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DefaultImageImportFolder','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",202)";//202 - Used to restrict access to Image Create.
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS statementprod";
			Db.NonQ(command);
			command=@"CREATE TABLE statementprod (
					StatementProdNum bigint NOT NULL auto_increment PRIMARY KEY,
					StatementNum bigint NOT NULL,
					FKey bigint NOT NULL,
					ProdType tinyint NOT NULL,
					LateChargeAdjNum bigint NOT NULL,
					INDEX(StatementNum),
					INDEX(FKey),
					INDEX(ProdType),
					INDEX(LateChargeAdjNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=1";//1 is AdjTypes
			int order=PIn.Int(Db.GetCount(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+"VALUES (1,'Late Charge',"+POut.Int(order)+",'+')";//1 is AdjTypes
			long defNum=Db.NonQ(command,true);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeAdjustmentType','"+POut.Long(defNum)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeLastRunDate','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeAccountNoTil','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeBalancesLessThan','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargePercent','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMin','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeMax','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeStart','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDateRangeEnd','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeDefaultBillingTypes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ShowFeatureLateCharges','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesInactivateDeclinedCards','0')";
			Db.NonQ(command);//Unset by default (same effect as Off for now, for backwards compatibility).
			command="ALTER TABLE creditcard ADD IsRecurringActive tinyint NOT NULL";
			Db.NonQ(command);
			command="UPDATE creditcard SET IsRecurringActive=1";//Default to true for existing credit cards.
			Db.NonQ(command);
		}//End of 20_5_1() method

		private static void To20_5_2() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS discountplansub";
			Db.NonQ(command);
			command=@"CREATE TABLE discountplansub (
				DiscountSubNum bigint NOT NULL auto_increment PRIMARY KEY,
				DiscountPlanNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				DateEffective date NOT NULL DEFAULT '0001-01-01',
				DateTerm date NOT NULL DEFAULT '0001-01-01',
				INDEX(DiscountPlanNum),
				INDEX(PatNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT PatNum,DiscountPlanNum FROM patient WHERE DiscountPlanNum>0";
			table=Db.GetTable(command);
			long patNum;
			for(int i=0;i<table.Rows.Count;i++) {
				patNum=PIn.Long(table.Rows[i]["PatNum"].ToString());
				command=$@"INSERT INTO discountplansub (DiscountPlanNum,PatNum) 
					VALUES(
						{POut.Long(PIn.Long(table.Rows[i]["DiscountPlanNum"].ToString()))},
						{POut.Long(patNum)}
					)";
				Db.NonQ(command);
				//Optionally set the DiscountPlanNum to 0
				command="UPDATE patient ";
				command+="SET DiscountPlanNum=0 ";
				command+="WHERE PatNum="+POut.Long(patNum);
				Db.NonQ(command);
			}
			command="DELETE FROM grouppermission WHERE PermType=89";//Permission already existed, but not enforced. Refreshing this Permission from scratch.
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",89)";//89 - Used to restrict access to Image Edit.
				Db.NonQ(command);
			}
			command="ALTER TABLE programproperty ADD IsMasked tinyint NOT NULL";
			Db.NonQ(command);
			command="SELECT ProgramPropertyNum FROM programproperty WHERE PropertyDesc LIKE '%password%'";
			List<long> listPasswordPropNums=Db.GetListLong(command);
			for(int i=0;i<listPasswordPropNums.Count;i++) {
				command=$"UPDATE programproperty SET IsMasked={POut.Bool(true)} WHERE ProgramPropertyNum={POut.Long(listPasswordPropNums[i])}";
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmPostcardFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmEmailFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmTextFamMessage','We would like to confirm your appointments. [FamilyApptList].')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ConfirmGroupByFamily','0')";
			Db.NonQ(command);
			command="ALTER TABLE orthochart ADD ProvNum bigint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_2() method

		private static void To20_5_3() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LateChargeExcludeExistingLateCharges','0')";
			Db.NonQ(command);
			command="ALTER TABLE statementprod ADD DocNum bigint NOT NULL,ADD INDEX (DocNum)";
			Db.NonQ(command);
			command="ALTER TABLE emailaddress ADD DownloadInbox tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 20_5_3() method

		private static void To20_5_4() {
			string command;
			DataTable table;
			command=$"SELECT * FROM ebill WHERE ElectPassword!=''";
			table=Db.GetTable(command);
			foreach(DataRow row in table.Rows) {
				CDT.Class1.Encrypt(row["ElectPassword"].ToString(),out string encPW);
				long ebillNum=PIn.Long(row["EbillNum"].ToString());
				command=$"UPDATE ebill SET ElectPassword='{POut.String(encPW)}' WHERE EbillNum={POut.Long(ebillNum)}";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS certemployee";
			Db.NonQ(command);
			command=@"CREATE TABLE certemployee (
				CertEmployeeNum bigint NOT NULL auto_increment PRIMARY KEY,
				CertNum bigint NOT NULL,
				EmployeeNum bigint NOT NULL,
				DateCompleted date NOT NULL DEFAULT '0001-01-01',
				Note varchar(255) NOT NULL,
				UserNum bigint NOT NULL,
				INDEX(UserNum),
				INDEX (CertNum),
				INDEX (EmployeeNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			long groupNum;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",203)";//203 - Permission to update Employee Certifications.
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				+"VALUES("+POut.Long(groupNum)+",204)";//204 - Permission to set up Certifications.
				Db.NonQ(command);
			}
		}//End of 20_5_4() method

		private static void To20_5_5() {
			string command;
			DataTable table;
			command="ALTER TABLE cert ADD IsHidden tinyint NOT NULL";
			Db.NonQ(command);
			if(!ColumnExists(GetCurrentDatabase(),"deposit","IsSentToQuickBooksOnline")) {
				command="ALTER TABLE deposit ADD IsSentToQuickBooksOnline tinyint NOT NULL";
				Db.NonQ(command);
			}
		}//End of 20_5_5() method

		private static void To20_5_11() {
			string command="ALTER TABLE carecreditwebresponse ADD HasLogged tinyint NOT NULL";
			Db.NonQ(command);
			//Allen says this is what we want to do.
			command=@"SELECT COALESCE(MIN(SheetDefNum),0) FROM sheetdef WHERE SheetType=15";//SheetTypeEnum.Statement
			long sheetDefNumDefault=PIn.Long(Db.GetScalar(command));
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultStatement'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultReceipt'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultInvoice'";
			Db.NonQ(command);	
			command=$@"UPDATE preference SET ValueString={POut.Long(sheetDefNumDefault)} WHERE PrefName='SheetsDefaultLimited'";
			Db.NonQ(command);
		}

		private static void To20_5_13() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			string newPath=programPath.Replace("DentalEye.exe","CmdLink.exe");//shouldn't launch from this exe anymore
			command="UPDATE program SET Path='"+POut.String(newPath)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			string note="Please set the file path to open CmdLink.exe in order to send patient data to DentalEye. Ex: C:\\Program Files (x86)\\DentalEye\\CmdLink.exe.";
			command="UPDATE program SET Note='"+POut.String(note)+"' WHERE ProgName='DentalEye'";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES('BillingElectIncludeAdjustDescript',1)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE preference.PrefName='PdfLaunchWindow';";
			if(PIn.Int(Db.GetCount(command))==0) {
				command="INSERT INTO preference (PrefName,ValueString) VALUES('PdfLaunchWindow','0');";//false by default
				Db.NonQ(command);
			}
		}//End of 20_5_13() method

		private static void To20_5_17() {
			string command;
			command="ALTER TABLE cert ADD CertCategoryNum bigint NOT NULL";
			Db.NonQ(command);
			command="UPDATE cert SET CertCategoryNum=(SELECT CertCategoryNum FROM certlinkcategory WHERE cert.CertNum=certlinkcategory.CertNum)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS certlinkcategory";
			Db.NonQ(command);
		}//End of 20_5_17() method

		private static void To20_5_32() {
			string command;
			//alertcategorylink.AlertCategoryNum for SBs may have been set incorrectly in 20.5.1
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='SupplementalBackups' AND IsHQCategory=1";
			long alertCatNum=Db.GetLong(command);
			command=$@"UPDATE alertcategorylink SET AlertCategoryNum={POut.Long(alertCatNum)} WHERE AlertType=23";//23=SupplementalBackups
			Db.NonQ(command);
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}//End of 20_5_32() method

		private static void To20_5_33() {
			string command;
			DoseSpotSelfReportedInvalidNote();
		}

		private static void To20_5_34() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
			table=Db.GetTable(command);
			long groupNum;
			foreach(DataRow row in table.Rows) {
				groupNum=PIn.Long(row["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
				   +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
				Db.NonQ(command);
			}
		}

		private static void To20_5_38() {
			string command;
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 20_5_38() method

		private static void To20_5_41() {
			string command;
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 20_5_41() method

		private static void To20_5_48() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}

		private static void To20_5_57() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
			Db.NonQ(command);
		}//End of 20_5_57() method

		private static void To20_5_61() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
			Db.NonQ(command);
		}//End of 20_5_61() method

		private static void To20_5_65() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
			Db.NonQ(command);
		}//End of 20_5_65() method

		private static void To21_1_1() {
			string command;
			DataTable table;
			//Set the ExistingPat 2FA prefs pref to defautl vals for all clinics
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			if(listClinicNums.Count>0) {
				foreach(long clinicNum in listClinicNums) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthEmail','1')"; //Default to true
					Db.NonQ(command);
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'WebSchedExistingPatDoAuthText','0')"; //Default to false
					Db.NonQ(command);
				}
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthEmail','1')"; //Default to true
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatDoAuthText','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD DynamicPayPlanTPOption tinyint NOT NULL";//will default to 0 'None' which should be ok since 1 will get set once TP added and OK clicked.
			Db.NonQ(command);
			if(!IndexExists("smsfrommobile","ClinicNum,SmsStatus,IsHidden")) {
				command="ALTER TABLE smsfrommobile ADD INDEX ClinicStatusHidden (ClinicNum,SmsStatus,IsHidden)";
				List<string> listIndexesToDrop=GetIndexNames("smsfrommobile","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE tsitranslog CHANGE DemandType ServiceType TINYINT NOT NULL";
			Db.NonQ(command);
			command="UPDATE program SET ProgDesc='Central Data Storage from centraldatastorage.com' "
				+"WHERE ProgName='CentralDataStorage' "
				+"AND ProgDesc='Cental Data Storage from centraldatastorage.com'";
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD PlanNote text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE discountplansub ADD SubNote text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingUseNoReply','1')";//default to true.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailSecureStatus','0')";//default to NotActivated or Enabled.
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('Ins834IsEmployerCreate','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE document MODIFY COLUMN Note MEDIUMTEXT NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TemplateFailureAutoReply text NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TemplateFailureAutoReply='There was an error confirming your appointment with [OfficeName]."
				+" Please call [OfficePhone] to confirm.' WHERE TypeCur=1"; //1 - ConfirmationFutureDay
			Db.NonQ(command);
			command="ALTER TABLE discountplan ADD ExamFreqLimit int NOT NULL,ADD XrayFreqLimit int NOT NULL,ADD ProphyFreqLimit int NOT NULL,"
				+"ADD FluorideFreqLimit int NOT NULL,ADD PerioFreqLimit int NOT NULL,ADD LimitedExamFreqLimit int NOT NULL,ADD PAFreqLimit int NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanExamCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanXrayCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanProphyCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanFluorideCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPerioCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanLimitedCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DiscountPlanPACodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('CloudAllowedIpAddresses','')";
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES("+POut.Long(groupNum)+",206)";
				 Db.NonQ(command);
			}
			command="TRUNCATE TABLE eservicelog";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN SheetNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN PayNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN AptNum";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceOperation";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog DROP COLUMN EServiceCode";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceType tinyint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD EServiceAction smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD KeyType smallint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD LogGuid VARCHAR(16)";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD ClinicNum bigint";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD FKey bigint";
			Db.NonQ(command);
			if(!IndexExists("eservicelog","LogDateTime,ClinicNum")) {
				command="ALTER TABLE eservicelog ADD INDEX ClinicDateTime (LogDateTime,ClinicNum)";
				List<string> listIndexesToDrop=GetIndexNames("eservicelog","ClinicNum");
				if(!listIndexesToDrop.IsNullOrEmpty()) {
					command+=","+string.Join(",",listIndexesToDrop.Select(x => "DROP INDEX "+x));
				}
				Db.NonQ(command);
			}
			command="ALTER TABLE discountplan ADD AnnualMax double NOT NULL DEFAULT -1";
			Db.NonQ(command);
		}//End of 21_1_1() method

		private static void To21_1_3() {
			string command;
			DataTable table;
			//This was done way back in 16.4 but the SheetsDefaultTreatmentPlan preference did not get fully implemented.
			//This commit is to officially implement SheetsDefaultTreatmentPlan (along with clinic specific overrides for the pref).
			//Therefore, the current value in the preference table needs to be updated with the most recent SheetDefNum that would be currently used.
			//The following code mimics the behavior of SheetDefs.GetInternalOrCustom() which is being used for TP sheets at the time of this commit.
			//E.g. ListSheetDefs.OrderBy(x => x.Description).ThenBy(x => x.SheetDefNum).FirstOrDefault()
			command=@"SELECT SheetDefNum
				FROM sheetdef 
				WHERE SheetType=17
				ORDER BY Description,SheetDefNum
				LIMIT 1";
			table=Db.GetTable(command);//GetScalar won't work with this particular query because it may not return a row (no custom sheet def).
			long treatmentPlanSheetDefNum=0;
			if(table.Rows.Count > 0) {
				treatmentPlanSheetDefNum=PIn.Long(table.Rows[0]["SheetDefNum"].ToString());
			}
			command=$@"UPDATE preference SET ValueString='{POut.Long(treatmentPlanSheetDefNum)}' WHERE PrefName='SheetsDefaultTreatmentPlan'";
			Db.NonQ(command);
		}

		private static void To21_1_5() {
			string command;
			DataTable table;
			command="UPDATE alertitem SET ClinicNum=-1 WHERE Type=23 AND ClinicNum=0";//23=SupplementalBackups
			Db.NonQ(command);
		}

		private static void To21_1_6() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EraAutomationBehavior','1')";//1 - EraAutomationMode.ReviewAll by default.
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD EraAutomationOverride tinyint NOT NULL";//0 - EraAutomationMode.UseGlobal by default.
			Db.NonQ(command);
			DoseSpotSelfReportedInvalidNote();
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger')";
			List<long> listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			string defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmByodEnabled','"+defNums+"')";
			Db.NonQ(command);
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger',"
				+"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
			listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().ToList();
			defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmExcludeEclipboard','"+defNums+"')";
			Db.NonQ(command);
			//Set default to 'Legacy' to set the column value without updating the timestamp.
			command="ALTER TABLE emailmessage ADD MsgType varchar(255) NOT NULL DEFAULT 'Legacy',ADD FailReason varchar(255) NOT NULL";
			Db.NonQ(command);
			//Set default back to ''
			command="ALTER TABLE emailmessage MODIFY MsgType varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 21_1_6() method

		private static void To21_1_9() {
			string command;
			DataTable table;
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=208";
			table=Db.GetTable(command);
			if(table.Rows.Count==0) {
				command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=149";//Currently has Payment Plan Edit permissions
				table=Db.GetTable(command);
				long groupNum;
				foreach(DataRow row in table.Rows) {
					groupNum=PIn.Long(row["UserGroupNum"].ToString());
					command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",208)";//208 - Will be given Payment Plan Charge Date Edit permissions
					Db.NonQ(command);
				}
			}
		}//End of 21_1_9() method

		private static void To21_1_13() {
			string command;
			command="SELECT DISTINCT ClinicNum FROM clinicpref";
			List<long> listClinicNums=Db.GetListLong(command);
			for(int i=0;i<listClinicNums.Count;i++) {
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthEmail' AND ClinicNum={POut.Long(listClinicNums[i])}";
				List<long> listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
				command=$@"SELECT ClinicPrefNum FROM clinicpref WHERE PrefName='WebSchedExistingPatDoAuthText' AND ClinicNum={POut.Long(listClinicNums[i])}";
				listClinicPrefNums=Db.GetListLong(command);
				if(listClinicPrefNums.Count>1) {
					//We will not delete the last one.
					for(int j=0;j<listClinicPrefNums.Count-1;j++) {
						command=@$"DELETE FROM clinicpref WHERE ClinicPrefNum={POut.Long(listClinicPrefNums[j])}";
						Db.NonQ(command);
					}
				}
			}
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
		}//End of 21_1_13() method

		private static void To21_1_16() {
			string command;
			DataTable table;
			if(!IndexExists("payment","PayType")) {
				command="ALTER TABLE payment ADD INDEX (PayType)";//FK to definition.DefNum with category 10
				Db.NonQ(command);
			}
			command="SELECT program.Path FROM program WHERE ProgName='DentalEye'";
			string programPath=Db.GetScalar(command);//only one path
			if(programPath.Contains("DentalEye.exe\\CmdLink.exe")) {//this isn't a valid path, so fix it
				programPath=programPath.Replace("\\DentalEye.exe","");
				command="UPDATE program SET Path='"+POut.String(programPath)+"' WHERE ProgName='DentalEye'";
				Db.NonQ(command);
			}
		}//End of 21_1_16() method

		private static void To21_1_22() {
			string command;
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType")) {
				command="ALTER TABLE claim ADD INDEX PatStatusType (PatNum,ClaimStatus,ClaimType)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
		}//End of 21_1_22() method

		private static void To21_1_28() {
			//Update existing discountplans with all frequency limitations set to 0, to have unlimited frequency limitation.
			string command=$@"UPDATE discountplan
					SET ExamFreqLimit=-1,XrayFreqLimit=-1,ProphyFreqLimit=-1,FluorideFreqLimit=-1,PerioFreqLimit=-1,LimitedExamFreqLimit=-1,PAFreqLimit=-1
					WHERE ExamFreqLimit=0 AND XrayFreqLimit=0 AND ProphyFreqLimit=0 AND FluorideFreqLimit=0 AND PerioFreqLimit=0 AND LimitedExamFreqLimit=0 AND PAFreqLimit=0";
			Db.NonQ(command);
		}//End of 21_1_28() method

		private static void To21_1_31() {
			string command="SELECT * FROM preference WHERE PrefName='EraRefreshOnLoad'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')";//Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_31() method

		private static void To21_1_35() {
			string command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
		}//End of 21_1_35() method

		private static void To21_1_37() {
			string command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_1_37() method

		private static void To21_2_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailDefaultSendPlatform','Secure')";//Defaults to SecureEmail(EmailHosting)
			Db.NonQ(command);
			command="ALTER TABLE payplancharge ADD IsOffset tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD MinAge INT NOT NULL DEFAULT -1";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD MaxAge INT NOT NULL DEFAULT -1";
			Db.NonQ(command);
			command="ALTER TABLE apptview ADD WaitingRmName tinyint NOT NULL";
			Db.NonQ(command);
			//This set of code will get and encrypt third party passwords that are stored in the database as plaintext.
			//XVWeb, Appriss Client, and XCharge passwords have already been encrypted.
			command=$"SELECT ProgramNum from program WHERE ProgName In ('XVWeb','Xcharge','SFTP')";
			List<long> listProgNums=Db.GetListLong(command);
			string listStrPrognums=string.Join(",",listProgNums);
			command=$"SELECT ProgramPropertyNum,PropertyValue from programproperty WHERE IsMasked=1 " +//Find all passwords
				$"AND PropertyValue!='' "	+																															 //that have a value
				$"AND PropertyDesc!='Appriss Client Password' " +																				 //aren't the client key password for Appriss
				$"AND ProgramNum NOT IN ({listStrPrognums})";																					   //and aren't in our list of programnums
			table=Db.GetTable(command);
			long progPropertyNum;
			string password;
			string obfuscatedPassword="";
			for(int i=0;i<table.Rows.Count;i++) {
				progPropertyNum=PIn.Long(table.Rows[i]["ProgramPropertyNum"].ToString());
				password=PIn.String(table.Rows[i]["PropertyValue"].ToString());
				try {
					if(CDT.Class1.Decrypt(password,out _) || !CDT.Class1.Encrypt(password,out obfuscatedPassword)) {
						continue;
					}
					command=$@"UPDATE programproperty SET PropertyValue='{obfuscatedPassword}' WHERE ProgramPropertyNum={POut.Long(progPropertyNum)}";
					Db.NonQ(command);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			command="DROP TABLE IF EXISTS transactioninvoice";
			Db.NonQ(command);
			command=@"CREATE TABLE transactioninvoice (
					TransactionInvoiceNum bigint NOT NULL auto_increment PRIMARY KEY,
					FileName varchar(255) NOT NULL,
					InvoiceData text NOT NULL
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="ALTER TABLE transaction ADD TransactionInvoiceNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE transaction ADD INDEX (TransactionInvoiceNum)";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog MODIFY LogGuid VARCHAR(36) NOT NULL";
			Db.NonQ(command);
			command="UPDATE program SET Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PracticeManagementIntegration\DentalDesktopCmd.exe")+"' "
				+"WHERE Path='"+POut.String(@"C:\Program Files\3Shape\Dental Desktop\Plugins\ThreeShape.PMSIntegration\DentalDesktopCmd.exe")+"' "
				+"AND ProgName='"+POut.String("ThreeShape")+"'";
			Db.NonQ(command);
			command="ALTER TABLE referral ADD BusinessName varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE referral ADD DisplayNote varchar(4000) NOT NULL";
			Db.NonQ(command);
			command="ALTER table transactioninvoice MODIFY COLUMN InvoiceData mediumtext";
			Db.NonQ(command);
			//This set of code will encrypt HL7 passwords (each location has it's own HL7 table row, so there may be multiple).
			command=@"SELECT HL7DefNum,SftpPassword
				FROM hl7def
				WHERE SftpPassword!=''";//Don't have to set if password has no value.
			table=Db.GetTable(command);
			long hl7DefNum;
			for(int i=0;i<table.Rows.Count;i++) {
				hl7DefNum=PIn.Long(table.Rows[i]["HL7DefNum"].ToString());
				password=PIn.String(table.Rows[i]["SftpPassword"].ToString());
				try {
					if(CDT.Class1.Decrypt(password,out _) || !CDT.Class1.Encrypt(password,out obfuscatedPassword)) {
						continue;
					}
					command=$@"UPDATE hl7def SET SftpPassword='{obfuscatedPassword}' WHERE HL7DefNum={POut.Long(hl7DefNum)}";
					Db.NonQ(command);
				}
				catch(Exception ex) {
					ex.DoNothing();
				}
			}
			//This set of code will update abbreviations for 2020 and 2021 ADA codes if the user has already used D code tools.
			//A list of the codes we need to add abbreviations for.
			List<string> listStrCodes=new List<string>() {"D0419","D1551","D1552","D1553","D1556","D1557","D1558","D2753","D5284","D5286","D6082","D6083",
				"D6084","D6086","D6087","D6088","D6097","D6098","D6099","D6120","D6121","D6122","D6123","D6195","D6243","D6753","D6784","D7922","D8696","D8697",
				"D8698","D8699","D8701","D8702","D8703","D8704","D9997","D0604","D0605","D0701","D0702","D0703","D0704","D0705","D0706","D0707","D0708","D0709",
				"D1321","D1355","D2928","D3471","D3472","D3473","D3501","D3502","D3503","D5995","D5996","D6191","D6192","D7961","D7962","D7993","D7994"};
			string strCodes=string.Join(",",listStrCodes.Select(x => $"'{POut.String(x)}'"));
			List<string> listAbbrs=new List<string>() {"SLFL","RBSMMAX","RBSMMAN","REMFSMQ","REMFUSMQ","REMFSPMAX","REMFBSMAN","PFMT","RPDUFQ","RPDURQ",
				"IMBPFMB","IMPPFMN","IMPPFMT","IMPFMCB","IMPFMCN","IMPFMCT","ABUFPMT","IMPRETPFMB","IMPRETPFMN","IMPRETPFMT","IMPRETFMCB","IMPRETFMCN",
				"IMPRETFMCT","ABURETPFMT","PONPFMT","RETPFMT","3/4RETFMCT","SOCKMED","ORREPAIRMAX","ORREPAIRMAN","ORRECMAX","ORRECMAN","REFRETMAX","REFRETMAN",
				"REPLRETMAX","REPLRETMAN","CASEMANAGE","Antig","Antib","PanC","CephC","2DC","3DC","EOC","OCCC","PAC","BWXC","FMXC","ConSA","CPM","PCR","RRA",
				"RRB","RRM","SERA","SERB","SERM","PMCU","PMCL","SPABPL","SMATPL","FRENB","FRENL","CRANIMP","ZYGIMP"};
			command=$@"SELECT CodeNum,AbbrDesc,ProcCode FROM procedurecode
				WHERE ProcCode IN ({strCodes})";  
			table=Db.GetTable(command);
			long codeNum;
			string abbrDesc;
			string procCode;
			int index;
			for(int i=0;i<table.Rows.Count;i++) {
				codeNum=PIn.Long(table.Rows[i]["CodeNum"].ToString());
				abbrDesc=PIn.String(table.Rows[i]["AbbrDesc"].ToString());
				procCode=PIn.String(table.Rows[i]["ProcCode"].ToString());
				if(!abbrDesc.IsNullOrEmpty()) {//Customers can add their own abbreviations, so skip abbreviation if not blank.
					continue;
				}
				index=listStrCodes.IndexOf(procCode);//Get index of code
				if(index==-1) {
					continue;
				}
				command=$@"UPDATE procedurecode SET AbbrDesc='{POut.String(listAbbrs[index])}' WHERE CodeNum={POut.Long(codeNum)}";
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS referralcliniclink";
			Db.NonQ(command);
			command=@"CREATE TABLE referralcliniclink (
				ReferralClinicLinkNum bigint NOT NULL auto_increment PRIMARY KEY,
				ReferralNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(ReferralNum),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardDoTwoFactorAuth','0')"; //Default to false
			Db.NonQ(command);
			if(!IndexExists("fee","FeeSched,CodeNum,ClinicNum,ProvNum")) {//may have been added manually
				command="ALTER TABLE fee ADD INDEX FeeSchedCodeClinicProv (FeeSched,CodeNum,ClinicNum,ProvNum)";
				List<string> listIndexNames=GetIndexNames("fee","FeeSched");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS hieclinic";
			Db.NonQ(command);
			command=@"CREATE TABLE hieclinic (
					HieClinicNum bigint NOT NULL auto_increment PRIMARY KEY,
					ClinicNum bigint NOT NULL,
					SupportedCarrierFlags tinyint NOT NULL,
					PathExportCCD varchar(255) NOT NULL,
					TimeOfDayExportCCD bigint NOT NULL,
					IsEnabled tinyint NOT NULL,
					INDEX(ClinicNum),
					INDEX(TimeOfDayExportCCD)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS hiequeue";
			Db.NonQ(command);
			command=@"CREATE TABLE hiequeue (
					HieQueueNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					INDEX(PatNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Set of code to disable Dental Intel.
			command=$"SELECT ProgramNum from program WHERE ProgName='DentalIntel'";
			long progNum=Db.GetLong(command);
			command=$@"UPDATE program SET Enabled=0,IsDisabledByHQ=1 WHERE ProgramNum={POut.Long(progNum)}";
			Db.NonQ(command);//Disable the program, since this will not appear anywhere anymore.
			command=$@"UPDATE programproperty SET PropertyValue='1'
				WHERE ProgramNum={POut.Long(progNum)}
				AND PropertyDesc='Disable Advertising HQ'";//Set 'Disable Advertising HQ' affiliated to Dental Intel to true.
			Db.NonQ(command);
			//Insert RayBridge bridge (new version of SMARTDent)----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'RayBridge', " 
				+"'SMARTDent New from www.raymedical.com', " 
				+"'0', " 
				+"'"+POut.String(@"C:\Ray\RayBridge\RayBridge.exe")+"', " 
				+"'"+POut.String(@"")+"', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				+"'0')"; 
			Db.NonQ(command); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+POut.Long(programNum)+"',"
				+"'Xml output file path',"
				+"'"+POut.String(@"C:\Ray\PatientInfo.xml")+"'" 
				+")";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'2', "//ToolBarsAvail.ChartModule 
				+"'SmartDent')"; 
			Db.NonQ(command); 
			//end RayBridge bridge
			command="SELECT ValueString FROM preference WHERE PrefName='PracticePhone'";
			string practicePhone=Db.GetScalar(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticeBillingPhone','"+POut.String(practicePhone)+"')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PracticePayToPhone','"+POut.String(practicePhone)+"')";
			Db.NonQ(command);
			//Insert VisionX bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				+") VALUES("
				+"'VisionX', "
				+"'VisionX from www.airtechniques.com', "
				+"'0', "
				+"'"+POut.String(@"C:\Program Files\Air Techniques\VisionX\Clients\VisionX.exe")+"', "
				+"'', "
				+"'"+POut.String(@"No command line or path is needed.")+"')";
			programNum=Db.NonQ(command,true);//we now have a ProgramNum to work with
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+programNum.ToString()+"', "
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				+"'0')";
			Db.NonQ(command);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				+") VALUES("
				+"'"+programNum.ToString()+"', "
				+"'Text file path', "
				+"'"+POut.String(@"C:\ProgramData\Air Techniques\VisionX\WorkstationService\Examination\DBSWINLegacySupport\patimport.txt")+"')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				+"VALUES ("
				+"'"+programNum.ToString()+"', "
				+"'"+((int)ToolBarsAvail.ChartModule).ToString()+"', "
				+"'VisionX')";
			Db.NonQ(command);
			//end VisionX bridge
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ADPRunIID','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RecurringChargesShowInactive','0')";
			Db.NonQ(command);string upgrading="Upgrading database to version: 21.2.1";
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			//New alert categorylink, Update Complete - Action Required
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
			long alertCatNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},32)";//32=Update
			Db.NonQ(command);
			command="ALTER TABLE carrier ADD OrthoInsPayConsolidate tinyint NOT NULL";//0 - OrthoInsPayConsolidate.Global by default.
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS cloudaddress";
			Db.NonQ(command);
			command=@"CREATE TABLE cloudaddress (
				CloudAddressNum bigint NOT NULL auto_increment PRIMARY KEY,
				IpAddress varchar(50) NOT NULL,
				UserNumLastConnect bigint NOT NULL,
				DateTimeLastConnect datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				INDEX(UserNumLastConnect)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='CloudAllowedIpAddresses'";
			string str=Db.GetScalar(command);
			string[] addresses=str.Split(",",StringSplitOptions.RemoveEmptyEntries);
			if(addresses.Length>0) {//If the database has no allowed addresses then we don't need to insert any into the new table.
				command=$"INSERT INTO cloudaddress (IpAddress) Values {string.Join(",",addresses.Select(x => "('"+POut.String(x)+"')"))}";
				Db.NonQ(command);
			}
			//Sync Patient Portal Invites into Automated Messaging preference
			command="SELECT ClinicNum,IsConfirmEnabled,IsConfirmDefault FROM clinic";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long clinicNum=PIn.Long(table.Rows[i]["ClinicNum"].ToString());
				bool isAutoCommEnabled=PIn.Bool(table.Rows[i]["IsConfirmEnabled"].ToString());
				bool isConfirmDefault=PIn.Bool(table.Rows[i]["IsConfirmDefault"].ToString());
				command="SELECT ValueString FROM clinicpref WHERE PrefName='PatientPortalInviteEnabled' AND ClinicNum="+POut.Long(clinicNum);
				string clinicPrefValueString=Db.GetScalar(command);
				bool isPatientPortalInviteEnabled=PIn.Bool(clinicPrefValueString);
				command="SELECT COUNT(*) FROM clinicpref WHERE PrefName='PatientPortalInviteUseDefaults' AND ClinicNum="+POut.Long(clinicNum);
				long count=Db.GetLong(command);
				//If the PatientPortalInviteUseDefaults clinicpref doesn't exist, create it for this clinic.
				//On previous versions, the absence of this preference set use defaults to true. Starting with this version, use defaults will be false if the clinicpref doesn't exist.
				//We need to create this clinicpref to preserve old logic.
				if(count==0) {
					command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'PatientPortalInviteUseDefaults','1')";
					Db.NonQ(command);
				}
				//If automated messaging is enabled but patient portal invites are not, disable all patient portal invite rules for that clinic
				if(isAutoCommEnabled && !isPatientPortalInviteEnabled) {
					//Disable all of the rules for patient portal invite for the clinic
					command="UPDATE apptreminderrule SET IsEnabled=0 WHERE TypeCur='3'" //ApptReminderType.PatientPortalInvite
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Set patient portal invite use defaults to false for the clinic
					command="UPDATE clinicpref SET ValueString='0' WHERE PrefName='PatientPortalInviteUseDefaults'"
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
				}
				//If automated messaging is not enabled but patient portal invites are, disable all other automated messaging rules for that clinic except for birthdays.
				else if(!isAutoCommEnabled && isPatientPortalInviteEnabled) {
					command="UPDATE apptreminderrule SET IsEnabled=0 WHERE TypeCur NOT IN('3','6')" //ApptReminderType.PatientPortalInvite, ApptReminderType.Birthday
						+" AND ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Turn on autocomm for this clinic, which will allow invites to remain on. We will turn off all other varieties of autocomm below.
					command="UPDATE clinic SET IsConfirmEnabled='1' WHERE ClinicNum="+POut.Long(clinicNum);
					Db.NonQ(command);
					//Set 'UseDefaults' to be false when created.
					isConfirmDefault=false;
				}
				//Create the 'UseDefaults' preferences.
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptArrivalUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptConfirmUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptReminderUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
				command="INSERT INTO clinicpref(ClinicNum,PrefName,ValueString) VALUES("+clinicNum+",'ApptThankYouUseDefaults','"+POut.Bool(isConfirmDefault)+"')";
				Db.NonQ(command);
			}
			command="ALTER TABLE mobileappdevice ADD IsBYODDevice TINYINT NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TextPaymentLinkAppointmentBalance','[nameF] please visit this [StatementShortURL] to pay your balance of [StatementBalance] for your recent appointment.')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TextPaymentLinkAccountBalance','[nameF] please visit this [StatementShortURL] to pay your balance of [StatementBalance]')";
			Db.NonQ(command);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('EClipboardImageCaptureDefs','')";
			Db.NonQ(command);
			command="SELECT MAX(ItemOrder) FROM definition WHERE Category=18";
			int order=PIn.Int(Db.GetScalar(command))+1;
			command=$"INSERT INTO definition (Category,ItemOrder,ItemName,ItemValue,ItemColor,IsHidden) VALUES({POut.Long(18)},{POut.Long(order)},'eClipboard','',0,0)";
			Db.NonQ(command);
			//53 - DefCat.eClipboardImageCapture
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Photo ID Front','Please take a picture of the front side of your photo ID',0)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Photo ID Back','Please take a picture of the back side of your photo ID',1)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Insurance Card Front','Please take a picture of the front side of your insurance card',2)";
			Db.NonQ(command);
			command="INSERT INTO definition (Category,ItemName,ItemValue,ItemOrder) VALUES (53,'Insurance Card Back','Please take a picture of the back side of your insurance card',3)";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("claimproc","ClaimProcNum",new ColNameAndDef("ClaimAdjReasonCodes","varchar(255) NOT NULL"));
			command="ALTER TABLE paysplit ADD PayPlanDebitType tinyint NOT NULL";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("treatplan","TreatPlanNum",new ColNameAndDef("MobileAppDeviceNum","bigint NOT NULL"),new IndexColsAndName("MobileAppDeviceNum",""));
			try {
				if(IsUsingReplication()) {
					string replicationMonitorMsg="Monitoring the slave status is now monitored by the OpenDentalReplicationService. "
					+"Each replication server will need the new OpenDentalReplicationService installed. "
					+"Please visit https://opendental.com and search for 'Slave Monitor' for more information.";
					command=$"INSERT INTO alertitem (ClinicNum,Description,Type,Severity,Actions,FormToOpen,FKey,ItemValue,UserNum) VALUES (-1,'{POut.String(replicationMonitorMsg)}',33,3,5,0,0,'',0)";
					Db.NonQ(command);
					command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='OdAllTypes'";
					alertCatNum=Db.GetLong(command);
					command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCatNum)},33)";//33=ReplicationService warning 
					Db.NonQ(command);
				}
			}
			catch {
				//Do nothing. Treat the office as not using replication.
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('RefundAdjustmentType','0')";
			Db.NonQ(command);
			command="DELETE FROM userodpref WHERE ValueString='' AND FkeyType=0";//FkeyType 0=Definition, expanded imaging categories. ValueString '' meant collapsed, which was meaningless.
			Db.NonQ(command);
			command="UPDATE userodpref SET ValueString='' WHERE FkeyType=0";//Expanded imaging categories were full of meaningless junk strings. No ValueString needed.
			Db.NonQ(command);
			command=$"INSERT INTO preference (PrefName,ValueString) VALUES('ApptGeneralMessageAutoEnabled','0')"; //Defaults to disabled
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS apptgeneralmessagesent";
			Db.NonQ(command);
			command=@"CREATE TABLE apptgeneralmessagesent (
				ApptGeneralMessageSentNum bigint NOT NULL auto_increment PRIMARY KEY,
				ApptNum bigint NOT NULL,
				PatNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				TSPrior bigint NOT NULL,
				ApptReminderRuleNum bigint NOT NULL,
				SmsSendStatus tinyint NOT NULL,
				EmailSendStatus tinyint NOT NULL,
				INDEX(ApptNum),
				INDEX(PatNum),
				INDEX(ClinicNum),
				INDEX(ApptReminderRuleNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT * FROM preference WHERE PrefName='EraRefreshOnLoad'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraRefreshOnLoad','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_2_1() method

		private static void To21_2_2() {
			string command;
			if(CultureInfo.CurrentCulture.Name=="en-US"){
				command="UPDATE procedurecode SET TreatArea=0 "//None
					+"WHERE TreatArea=3";//Mouth. ~337 rows
				Db.NonQ(command);
				command="UPDATE procedurecode SET TreatArea=3 "//Mouth
					+"WHERE TreatArea=0 "//some codes my have been set by user to something like quad, so this won't touch those
					+"AND ProcCode IN('D0330','D0701','D1110','D1120','D1206','D1208','D6190','D7285','D7286','D7287','D7288','D8050','D8060','D8070','D8080','D8090',"
					+"'D8210','D8220','D8660','D8670','D8680','D8690','D8695')";//only 23 codes are ever supposed to be Mouth
				Db.NonQ(command);
			}
			command="ALTER TABLE procedurecode ADD AreaAlsoToothRange tinyint NOT NULL";
			Db.NonQ(command);
			command=$"SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=29";//PaySplitUnearnedType
			int itemOrderNext=PIn.Int(Db.GetScalar(command));
			command="INSERT INTO definition (Category,ItemName,ItemOrder,ItemValue) "
				+$"VALUES (29,'Payment Plan Prepay',{POut.Int(itemOrderNext)},'X')";//29 is PaySplitUnearnedType and X is hidden / Do Not Show.
			long defNum=Db.NonQ(command,true);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('DynamicPayPlanPrepaymentUnearnedType','{defNum}')";
			Db.NonQ(command);
		}//End of 21_2_2() method
	
		private static void To21_2_7() {
			string command;
			command="DROP TABLE IF EXISTS treatplanparam";
			Db.NonQ(command);
			command=@"CREATE TABLE treatplanparam (
				TreatPlanParamNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				TreatPlanNum bigint NOT NULL,
				ShowDiscount tinyint NOT NULL,
				ShowMaxDed tinyint NOT NULL,
				ShowSubTotals tinyint NOT NULL,
				ShowTotals tinyint NOT NULL,
				ShowCompleted tinyint NOT NULL,
				ShowFees tinyint NOT NULL,
				ShowIns tinyint NOT NULL,
				INDEX(PatNum),
				INDEX(TreatPlanNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
		}//End of 21_2_7() method

		private static void To21_2_8() {
			string command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
			command="ALTER TABLE orthochart ADD OrthoChartRowNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE orthochart ADD INDEX (OrthoChartRowNum)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthochartrow";
			Db.NonQ(command);
			command=@"CREATE TABLE orthochartrow (
					OrthoChartRowNum bigint NOT NULL auto_increment PRIMARY KEY,
					PatNum bigint NOT NULL,
					DateTimeService datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
					UserNum bigint NOT NULL,
					ProvNum bigint NOT NULL,
					Signature text NOT NULL,
					INDEX(PatNum),
					INDEX(UserNum),
					INDEX(ProvNum)
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT DISTINCT(PatNum) FROM orthochart";
			List<long> listPatNums=Db.GetListLong(command);
			command="SELECT Description,(CASE WHEN InternalName='Signature' THEN 1 ELSE 0 END) isSignature "
				+"FROM displayfield WHERE Category=8 AND InternalName!='Provider'";
			DataTable tableDisplayFields=Db.GetTable(command);
			string fieldNameSig=tableDisplayFields.Select().Where(x => PIn.Bool(x["isSignature"].ToString())).Select(x => PIn.String(x["Description"].ToString())).FirstOrDefault();
			for(int i=0;i<listPatNums.Count;i++) {
				command=$@"SELECT * 
				FROM orthochart
				WHERE PatNum={listPatNums[i]}
				ORDER BY OrthoChartNum";
				DataTable table=Db.GetTable(command);
				//Group orthocharts into groups of PatNum,Prov, and DateService
				List<OrthoObj> listOrthoObjs=table.Select().GroupBy(x => new {
					patnum=PIn.Long(x["PatNum"].ToString()),
					dateservice=PIn.Date(x["DateService"].ToString()),
					provNum=PIn.Long(x["ProvNum"].ToString()),
					FieldName=PIn.String(x["FieldName"].ToString())
				}).GroupBy(x => new {
					patnum=x.Key.patnum,
					dateservice=x.Key.dateservice,
					provnum=x.Key.provNum
				}).Select(x => new OrthoObj() {
					PatNum=x.Key.patnum,
					ProvNum=x.Key.provnum,
					DateTService=x.Key.dateservice,
					ListDataRows=x.Select(y => new OrthoDataRows() {
						FieldName=y.Key.FieldName,
						listRows=y.ToList()
					}).ToList()
				}).ToList();
				for(int j=0;j<listOrthoObjs.Count;j++) {
					OrthoObj orthoObj=listOrthoObjs[j];
					List<DataRow> listDataRowsForOrthoChartRow;
					for(int k=0;k<orthoObj.ListDataRows.Max(x => x.listRows.Count);k++) {
						listDataRowsForOrthoChartRow=new List<DataRow>();
						listDataRowsForOrthoChartRow.AddRange(orthoObj.ListDataRows.Where(x => x.listRows.Count>k).Select(x => x.listRows[k]));
						string sigVal="";
						if(!string.IsNullOrEmpty(fieldNameSig)) {
							DataRow dataRowSig=listDataRowsForOrthoChartRow.FirstOrDefault(x => PIn.String(x["FieldName"].ToString())==fieldNameSig);
							if(dataRowSig!=null) {
								sigVal=dataRowSig["FieldValue"].ToString();
							}
						}
						command=$@"INSERT INTO orthochartrow(PatNum,DateTimeService,UserNum,ProvNum,Signature)
						VALUES({POut.Long(orthoObj.PatNum)},{POut.DateT(orthoObj.DateTService)},{listDataRowsForOrthoChartRow.First()["UserNum"].ToString()},{POut.Long(orthoObj.ProvNum)},'{sigVal}')";
						long orthoChartRowNum=Db.NonQ(command,true);
						command=$@"UPDATE orthochart SET OrthoChartRowNum={POut.Long(orthoChartRowNum)} WHERE OrthoChartNum IN ({string.Join(",",listDataRowsForOrthoChartRow.Select(x => x["OrthoChartNum"].ToString()))})";
						Db.NonQ(command);
					}
				}
			}
		}//End of 21_2_8() method

		private static void To21_2_9() {
			string command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
		}//End of 21_2_9() method

		private static void To21_2_14() {
			string command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=92";//92 - FeeSchedEdit
			DataTable table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",211)";//211 - AllowFeeEditWhileReceivingClaim
				Db.NonQ(command);
			}
		}//End of 21_2_14() method

		private static void To21_2_20() {
			string command="DROP TABLE IF EXISTS etrans835";
			Db.NonQ(command);
			command=@"CREATE TABLE etrans835 (
				Etrans835Num bigint NOT NULL auto_increment PRIMARY KEY,
				EtransNum bigint NOT NULL,
				PayerName varchar(60) NOT NULL,
				TransRefNum varchar(50) NOT NULL,
				InsPaid double NOT NULL,
				ControlId varchar(9) NOT NULL,
				PaymentMethodCode varchar(3) NOT NULL,
				PatientName varchar(100) NOT NULL,
				Status tinyint NOT NULL,
				INDEX(EtransNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AsapPromptEnabled','1')";//Default to true to maintain behavior
			Db.NonQ(command);
		}//End of 21_2_20() method

		private static void To21_2_22() {
			string command="SELECT ProgramNum FROM program WHERE ProgName='PreXionAquire'";
			long progNum=Db.GetLong(command);
			command=$"UPDATE program SET ProgName='PreXionAcquire', ProgDesc='PreXion Acquire' WHERE ProgramNum={POut.Long(progNum)}";
			Db.NonQ(command);
			command=$"UPDATE toolbutitem SET ButtonText='PreXion Acquire' WHERE ProgramNum={POut.Long(progNum)} AND ButtonText='PreXion Aquire'";
			Db.NonQ(command);
		}//End of 21_2_22() method

		private static void To21_2_25() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('AgingCalculateOnBatchClaimReceipt','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EmailHostingEndpoint','https://emailhosting.opendental.cloud:8200/')";
			Db.NonQ(command);
		}//End of the 21_2_25() method

		private static void To21_2_27() {
			string command="ALTER TABLE insplan ADD IsBlueBookEnabled tinyint NOT NULL DEFAULT 1";
			Db.NonQ(command);
			command="UPDATE insplan SET IsBlueBookEnabled=0 WHERE FeeSched>0"; // set all insplans with a fee scheduled to blue book disabled.
			Db.NonQ(command);
		}

		private static void To21_2_28() {
			string command="INSERT INTO preference(PrefName,ValueString) VALUES('ReportsIncompleteProcsExcludeCodes','D9986,D9987')"; //Default to broken appointment codes
			Db.NonQ(command);
		}

		private static void To21_2_30() {
			string command="ALTER TABLE etrans835 ADD AutoProcessed tinyint NOT NULL,ADD IsApproved tinyint NOT NULL";
			Db.NonQ(command);
		}

		private static void To21_2_36() {
			string command;
			if(!IndexExists("mount","PatNum")) {
				command="ALTER TABLE mount ADD INDEX (PatNum)";
				Db.NonQ(command);
			}
			if(!IndexExists("mountitem","MountNum")) {
				command="ALTER TABLE mountitem ADD INDEX (MountNum)";
				Db.NonQ(command);
			}
		}//End of the 21_2_36() method

		private static void To21_2_45() {
			string command;
			if(!IndexExists("document","MountItemNum")) {
				command="ALTER TABLE document ADD INDEX (MountItemNum)";
				Db.NonQ(command);
			}
		}

		private static void To21_2_47() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2022.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D4320",
					"D4321",
					"D8050",
					"D8060",
					"D8690"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
		}

		private static void To21_2_51() {
			//The url we inserted for these programs is no longer valid. This is us fixing that going forward.
			string command="SELECT * FROM program where ProgName='Trophy' OR ProgName='TrophyEnhanced'";
			DataTable listTrophyProgs=Db.GetTable(command);
			for(int i=0;i<listTrophyProgs.Rows.Count;i++) {
				DataRow progCur=listTrophyProgs.Rows[i];
				if(progCur["ProgDesc"].ToString().Contains("from www.trophy-imaging.com")) {
					progCur["ProgDesc"]=progCur["ProgDesc"].ToString().Replace("from www.trophy-imaging.com","");
					long progNum=PIn.Long(progCur["ProgramNum"].ToString());
					command=$"UPDATE program SET ProgDesc='{POut.String(progCur["ProgDesc"].ToString())}' WHERE ProgramNum={POut.Long(progNum)}";
					Db.NonQ(command);
				}
			}
		} 

		private static void To21_2_53() {
			string command;
			//Updating D9613 to default to TreatmentArea.Quad in CDT 2022 (only if user did not change from TreatmentArea.Mouth).
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				command="UPDATE procedurecode SET procedurecode.TreatArea=4 WHERE procedurecode.ProcCode='D9613' AND procedurecode.TreatArea=3";//4 - Quad
				Db.NonQ(command);
			}//end United States CDT codes update
		}//End of 21_2_53()

		private class OrthoObj {
			public long ProvNum;
			public long PatNum;
			public DateTime DateTService;
			public List<OrthoDataRows> ListDataRows;
		}

		private class OrthoDataRows {
			public string FieldName;
			public List<DataRow> listRows;
		}

		private static void To21_3_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardHasMultiPageCheckIn','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD SendMultipleInvites tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE apptreminderrule ADD TimeSpanMultipleInvites bigint NOT NULL";
			Db.NonQ(command);
			command="UPDATE apptreminderrule SET TimeSpanMultipleInvites=25920000000000";//30 days in ticks
			Db.NonQ(command);
			//check databasemaintenance for DiscountPlanSubWithInvalidDiscountPlanNum, insert if not there and set IsOld to True or update to set IsOld to true
			command="SELECT MethodName "
				+"FROM databasemaintenance "
				+"WHERE MethodName='DiscountPlanSubWithInvalidDiscountPlanNum'";
			string methodName=Db.GetScalar(command);
			if(methodName=="") {//didn't find row in table, insert
				command="INSERT INTO databasemaintenance "
					+"(MethodName, IsOld) "
					+"VALUES ('DiscountPlanSubWithInvalidDiscountPlanNum',1)";//true by default
			}
			else {//found row, update IsOld
				command="UPDATE databasemaintenance "
					+"SET IsOld = 1 "
					+"WHERE MethodName='DiscountPlanSubWithInvalidDiscountPlanNum'";//true by default
			}
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD EmailResponse varchar(255) NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE xwebresponse ADD EmailResponse varchar(255) NOT NULL";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='ReportingServerCompName'";
			string reportingServerCompName=Db.GetScalar(command);
			command="SELECT ValueString FROM preference WHERE PrefName='ReportingServerURI'";
			string reportingServerURI=Db.GetScalar(command);
			bool isReportingServerInUse=reportingServerCompName!="" || reportingServerURI!="";
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('AuditTrailUseReportingServer','{POut.Bool(isReportingServerInUse)}')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EserviceLogUploadTimeNext','0001-01-01 00:00:00')";
			Db.NonQ(command);
			command=$"SELECT ProgramNum FROM program WHERE ProgName='XVWeb'";
			long programNumXVWeb=Db.GetLong(command);
			if(programNumXVWeb>0) {
				command=$"UPDATE programproperty SET PropertyDesc='Image Category' WHERE PropertyDesc='ImageCategory' AND ProgramNum={POut.Long(programNumXVWeb)}";
				Db.NonQ(command);
			}
			command="SELECT * FROM preference WHERE PrefName='EraStrictClaimMatching'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraStrictClaimMatching','0')"; //Default to false.
				Db.NonQ(command);
			}
			command="SELECT * FROM preference WHERE PrefName='EraShowStatusAndClinic'";
			if(Db.GetTable(command).Rows.Count==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EraShowStatusAndClinic','1')"; //Default to true.
				Db.NonQ(command);
			}
			command="ALTER TABLE patientnote ADD UserNumOrthoLocked bigint NOT NULL";
			Db.NonQ(command);
			AlterTable("patient","PatNum",listIndexColsAndNames: new List<IndexColsAndName>() {
				new IndexColsAndName("ClinicNum,PatStatus","ClinicPatStatus"),
				new IndexColsAndName("Birthdate,PatStatus","BirthdateStatus")
			});
			AlterTable("recall","RecallNum",indexColsAndName:new IndexColsAndName("DateDue,IsDisabled,RecallTypeNum,DateScheduled","DateDisabledType"));
			AlterTable("procedurelog","ProcNum",indexColsAndName:new IndexColsAndName("ProcDate,ClinicNum,ProcStatus","DateClinicStatus"));
			command="ALTER TABLE smsfrommobile ADD INDEX StatusHiddenClinic (SmsStatus,IsHidden,ClinicNum),DROP INDEX ClinicStatusHidden";
			List<string> listRedundantIndexNames=GetRedundantIndexNames("smsfrommobile","SmsStatus,IsHidden,ClinicNum");
			if(listRedundantIndexNames.Any()) {
				command+=","+string.Join(",",listRedundantIndexNames.Select(x => "DROP INDEX "+x));
			}
			Db.NonQ(command);
			command="ALTER TABLE insverify ADD INDEX (DateLastAssigned)";
			Db.NonQ(command);
			command="ALTER TABLE disease ADD INDEX (DiseaseDefNum)";
			Db.NonQ(command);
			command="SELECT COUNT(*) FROM preference WHERE PrefName='UpdateAlterLargeTablesDirectly'";
			if(Db.GetInt(command)==0) {
				command="INSERT INTO preference(PrefName,ValueString) VALUES('UpdateAlterLargeTablesDirectly','1')";
				Db.NonQ(command);
			}
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8"; // 8 - Setup permission
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					+"VALUES("+POut.Long(groupNum)+",212)"; // 212 - ManageHighSecurityProgProperties permission
				Db.NonQ(command);
			}
			command="ALTER TABLE programproperty ADD IsHighSecurity tinyint NOT NULL";
			Db.NonQ(command);
			command=$"UPDATE programproperty SET IsHighSecurity={POut.Bool(true)} WHERE IsMasked=1";
			Db.NonQ(command);
			command="ALTER TABLE xchargetransaction ADD BatchTotal double NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('LicenseAgreementAccepted','0')"; //Default to false
			Db.NonQ(command);
			command="ALTER TABLE updatehistory ADD Signature text NOT NULL";
			Db.NonQ(command);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('ApptReminderPremedTemplate','Remember to take your Pre-Med. ')";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD DateTimeUploaded datetime NOT NULL DEFAULT '0001-01-01 12:00:00'";
			Db.NonQ(command);
			command="ALTER TABLE eservicelog ADD INDEX (DateTimeUploaded)";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS imagedraw";
			Db.NonQ(command);
			command=@"CREATE TABLE imagedraw (
				ImageDrawNum bigint NOT NULL auto_increment PRIMARY KEY,
				DocNum bigint NOT NULL,
				MountNum bigint NOT NULL,
				ColorDraw int NOT NULL,
				ColorBack int NOT NULL,
				DrawingSegment text NOT NULL,
				DrawText varchar(255) NOT NULL,
				FontSize float NOT NULL,
				DrawType tinyint NOT NULL,
				INDEX(DocNum),
				INDEX(MountNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardAllowPaymentOnCheckin','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'SteriSimple', " 
				+"'SteriSimple from www.sterisimple.ca', " 
				+"'0', " 
				+"'"+POut.String(@"C:\SteriSimple\SteriSoft\SteriRecall\RecallRepeater.exe")+"', " 
				+"'', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true); 
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(programNum)+"', " 
				+"'Enter 0 to use PatientNum, or 1 to use ChartNum', " 
				+"'0')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'7', "//ToolBarsAvail.MainToolbar 
				+"'SteriSimple')";
			Db.NonQ(command);
			command="ALTER TABLE sheetdef ADD AutoCheckSaveImage tinyint NOT NULL DEFAULT 1";//defaults to true to maintain current behavior
			Db.NonQ(command);
			command=$"SELECT ProgramNum FROM program WHERE ProgName='DexisIntegrator'";
			long programNumDexisIntegrator=Db.GetLong(command);
			if(programNumDexisIntegrator>0) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +POut.Long(programNumDexisIntegrator)+", " 
					 +"'Enter 0 to use DDE, or 1 to use communication file', " 
					 +"'0')"; 
				Db.NonQ(command);
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
					 +") VALUES(" 
					 +POut.Long(programNumDexisIntegrator)+", " 
					 +"'Communication files folder path', " 
					 +"'')"; 
				Db.NonQ(command); 
			}
			command="ALTER TABLE eservicelog ";
			List<string> listIndexNames=GetIndexNames("eservicelog","LogDateTime,ClinicNum");
			if(listIndexNames.Any()) {
				command+=string.Join(",",listIndexNames.Select(x => "DROP INDEX "+x))+",";
			}
			command+="ADD INDEX ClinicDateTime (ClinicNum,LogDateTime)";
			Db.NonQ(command);
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			table=Db.GetTable(command);
			if(table.Rows.Count > 0 && PIn.Bool(table.Rows[0][0].ToString())) {
				command="DROP TABLE IF EXISTS jobteam";
				Db.NonQ(command);
				command=@"CREATE TABLE jobteam (
					JobTeamNum bigint NOT NULL auto_increment PRIMARY KEY,
					TeamName varchar(100) NOT NULL,
					TeamDescription varchar(100) NOT NULL,
					TeamFocus tinyint NOT NULL,
					IsHidden tinyint NOT NULL
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
				command="DROP TABLE IF EXISTS jobteamuser";
				Db.NonQ(command);
				command=@"CREATE TABLE jobteamuser (
					JobTeamUserNum bigint NOT NULL auto_increment PRIMARY KEY,
					JobTeamNum bigint NOT NULL,
					UserNumEngineer bigint NOT NULL,
					IsTeamLead tinyint NOT NULL,
					IsHidden tinyint NOT NULL,
					INDEX(JobTeamNum),
					INDEX(UserNumEngineer)
					) DEFAULT CHARSET=utf8";
				Db.NonQ(command);
			}
			#endregion
		}//End of 21_3_1() method
	
		private static void To21_3_2() {
			string command;
			DataTable table;
			//Insert Pixel Bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				+") VALUES(" 
				+"'Pixel', " 
				+"'Pixel by Digital Doc', " 
				+"'0', " 
				+"'"+POut.String(@"""C:\Program Files (x86)\DigitalDoc\Pixel\PixelBridge.exe""")+"', " 
				+"'"+POut.String(@"[LName] [FName] [PatNum]")+"', "//leave blank if none 
				+"'')"; 
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				+"VALUES (" 
				+"'"+POut.Long(programNum)+"', " 
				+"'2', "//ToolBarsAvail.ChartModule 
				+"'Pixel')"; 
			Db.NonQ(command); 
			//end Pixel bridge
			//Add permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",214)"; // 214 - MedicationDefEdit permission
				 Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",215)"; // 215 - AllergyDefEdit permission
				 Db.NonQ(command);
			}
		}//End of 21_3_2() method

		private static void To21_3_3() {
			string command;
			DataTable table;
			//Add PatientEdit permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",108)"; // 108 - PatientEdit permission
				 Db.NonQ(command);
			}
			command="ALTER TABLE payplan ADD MobileAppDeviceNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE payplan ADD INDEX (MobileAppDeviceNum)";
			Db.NonQ(command);
			if(!ColumnExists(GetCurrentDatabase(),"etrans835","AutoProcessed")) {
				command="ALTER TABLE etrans835 ADD AutoProcessed tinyint NOT NULL,ADD IsApproved tinyint NOT NULL";
				Db.NonQ(command);
			}
		}//End of 21_3_3() method

		private static void To21_3_4() {
			string command;
			DataTable table;
			//Add Rate3 to TimeCards and ClockEvents-------------------------------------------
			command="ALTER TABLE timecardrule ADD HasWeekendRate3 TINYINT NOT NULL";
			Db.NonQ(command);
			command=$"UPDATE timecardrule SET HasWeekendRate3={POut.Bool(false)}";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("clockevent","ClockEventNum",new ColNameAndDef("Rate3Hours","TIME NOT NULL"));
			command="UPDATE clockevent SET Rate3Hours='-01:00:00'";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("clockevent","ClockEventNum",new ColNameAndDef("Rate3Auto","TIME NOT NULL"));
		}//End of 21_3_4() method

		private static void To21_3_6() {
			string command;
			DataTable table;
			command="SELECT ProgramNum FROM program WHERE ProgName='SteriSimple'";
			long steriSimpleProgramNum=Db.GetLong(command);
			if(steriSimpleProgramNum!=0) {
				//Users that updated to v21.3 prior to v21.3.6 were missing this button. Only add the button if it is not already present.
				command=$"SELECT COUNT(*) FROM toolbutitem WHERE ProgramNum={POut.Long(steriSimpleProgramNum)}";
				if(Db.GetCount(command)=="0") {
					command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
						+"VALUES (" 
						+POut.Long(steriSimpleProgramNum)+", " 
						+"7, "//ToolBarsAvail.MainToolbar 
						+"'SteriSimple')";
					Db.NonQ(command);
				}
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanSaveSignedToPdf','0')"; //Default to false
			Db.NonQ(command);
		}//End of 21_3_6() method
	
		private static void To21_3_8() {
			string command;
			command="SELECT ProgramNum FROM program where ProgName='DrCeph'";
			long drCephNum=Db.GetLong(command);
			if(drCephNum!=0) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				+") VALUES(" 
				+"'"+POut.Long(drCephNum)+"', " 
				+"'Custom Patient Race Options', "
				+"'')";
				Db.NonQ(command);
			}
		}//End of 21_3_8()

		private static void To21_3_10() {
			string command;
			if(!IndexExists("mount","PatNum")) {
				command="ALTER TABLE mount ADD INDEX (PatNum)";
				Db.NonQ(command);
			}
			if(!IndexExists("mountitem","MountNum")) {
				command="ALTER TABLE mountitem ADD INDEX (MountNum)";
				Db.NonQ(command);
			}
			DataTable table;
			command=@"INSERT INTO preference (PrefName, ValueString,Comments) VALUES ('AdvertisingPostCardGuid','','')";
			Db.NonQ(command);
			//Add Advertising permission to groups with existing permission eServiceSetup.
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=91";  //91 is the existing Permission number for eServiceSetup.
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",216)";  //216 is the enum value of the new permission Advertising.
				 Db.NonQ(command);
			}
		}//End of 21_3_10()

		private static void To21_3_11() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatRecallName','Teeth Cleaning')";
			Db.NonQ(command);
		}//End of 21_3_11()

		private static void To21_3_13() {
			string upgrading="Upgrading database to version: 21.3.13";
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			string command;
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('EmailSecureDefaultClinic','0')";
			Db.NonQ(command);
		}//End of 21_3_13()

		private static void To21_3_14() {
			string command;
			#region HQ Only
			//We are running this section of code for HQ only
			//This is very uncommon and normally manual queries should be run instead of doing a convert script.  But this change needs to happen as soon as we update to this version.
			command="SELECT ValueString FROM preference WHERE PrefName='DockPhonePanelShow'";
			if(PIn.Bool(Db.GetScalar(command))) {
				command=$@"UPDATE job SET PhaseCur=(CASE PhaseCur
					WHEN 'Concept' THEN 0
					WHEN 'Definition' THEN 1
					WHEN 'Development' THEN 2
					WHEN 'Documentation' THEN 3
					WHEN 'Complete' THEN 4
					WHEN 'Cancelled' THEN 5
					WHEN 'Quote' THEN 6
					ELSE PhaseCur END
				),
				Category=(CASE Category
					WHEN 'Feature' THEN 0
					WHEN 'Bug' THEN 1
					WHEN 'Enhancement' THEN 2
					WHEN 'Query' THEN 3
					WHEN 'ProgramBridge' THEN 4
					WHEN 'InternalRequest' THEN 5
					WHEN 'HqRequest' THEN 6
					WHEN 'Conversion' THEN 7
					WHEN 'Research' THEN 8
					WHEN 'SpecialProject' THEN 9
					WHEN 'NeedNoApproval' THEN 10
					WHEN 'MarketingDesign' THEN 11
					WHEN 'UnresolvedIssue' THEN 12
					ELSE Category END
				)";
				Db.NonQ(command);
				command="ALTER TABLE job MODIFY PhaseCur TINYINT NOT NULL,MODIFY Category TINYINT NOT NULL,ADD INDEX (PhaseCur),ADD INDEX (Category)";
				Db.NonQ(command);
			}
			#endregion
		}//End of 21_3_14()

		private static void To21_3_17() {
			string command;
			//Add permissions to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			List<long> listNums=Db.GetListLong(command);
			for(int i=0;i<listNums.Count;i++) {
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) VALUES"
						+"("+POut.Long(listNums[i])+",71)," // 71 - PatProblemListEdit permission
						+"("+POut.Long(listNums[i])+",72)," // 72 - PatMedicationListEdit permission
						+"("+POut.Long(listNums[i])+",73)"; // 73 - PatAllergyListEdit permission
				 Db.NonQ(command);
			}
		}//End of 21_3_17()

		private static void To21_3_20() {
			string command;
			if(!IndexExists("document","MountItemNum")) {
				command="ALTER TABLE document ADD INDEX (MountItemNum)";
				Db.NonQ(command);
			}
		}

		private static void To21_3_21() {
			string command;
			List<SheetTypeEnum> listSheetTypeEnums = Enum.GetValues(typeof(SheetTypeEnum)).Cast<SheetTypeEnum>()
					.Where(x => !EnumTools.GetAttributeOrDefault<SheetTypeAttribute>(x).CanAutoSave).ToList();
			command=$@"UPDATE sheetdef SET sheetdef.AutoCheckSaveImage = 0 WHERE sheetdef.SheetType IN ({String.Join(",",listSheetTypeEnums.Select(x => ((long)x)).ToList())})";
			DataCore.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PayPlanTermsAndConditions','I agree to the terms of this payment plan at an interest rate of %[APR] and agree to pay an amount of [PaymentAmt] [ChargeFrequency]. I understand that the cost and duration of the loan may change if I change the ''Total Amount'' of treatment financed.')";
			Db.NonQ(command);
			command="SELECT MAX(displayreport.ItemOrder) FROM displayreport WHERE displayreport.Category=3";//3=Lists report category
			long itemOrder=Db.GetLong(command)+1;//get the next available ItemOrder for the Category specified.
			command="INSERT INTO displayreport(InternalName,ItemOrder,Description,Category,IsHidden,IsVisibleInSubMenu) "
				+"VALUES('ODEraAutoProcessed',"+POut.Long(itemOrder)+",'ERAs Automatically Processed',3,0,0)";
			long reportNumNew=Db.NonQ(command,getInsertID:true);
			//Only usergroups with the InsPayCreate permission (PermType 36) can access the ERAs window.
			//By default, only these usergroups will have access to the ERAs Automatically Processed report.
			List<long> listGroupNums=Db.GetListLong("SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=36");
			for(int i=0;i<listGroupNums.Count;i++) {
				command="INSERT INTO grouppermission (NewerDate,NewerDays,UserGroupNum,PermType,FKey) "
					 +"VALUES('0001-01-01',0,"+POut.Long(listGroupNums[i])+",22,"+POut.Long(reportNumNew)+")";//22=Reports PermType
				Db.NonQ(command);
			}
		}//End of 21_3_21()

		private static void To21_3_22() {
			string command;
			//Moving codes to the Obsolete category that were deleted in CDT 2022.
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				//Move deprecated codes to the Obsolete procedure code category.
				//Make sure the procedure code category exists before moving the procedure codes.
				string procCatDescript="Obsolete";
				long defNum=0;
				command="SELECT DefNum FROM definition WHERE Category=11 AND ItemName='"+POut.String(procCatDescript)+"'";//11 is DefCat.ProcCodeCats
				DataTable dtDef=Db.GetTable(command);
				if(dtDef.Rows.Count==0) { //The procedure code category does not exist, add it
					command="SELECT COUNT(*) FROM definition WHERE Category=11";//11 is DefCat.ProcCodeCats
					int countCats=PIn.Int(Db.GetCount(command));
						command="INSERT INTO definition (Category,ItemName,ItemOrder) "
								+"VALUES (11"+",'"+POut.String(procCatDescript)+"',"+POut.Int(countCats)+")";//11 is DefCat.ProcCodeCats
					defNum=Db.NonQ(command,true);
				}
				else { //The procedure code category already exists, get the existing defnum
					defNum=PIn.Long(dtDef.Rows[0]["DefNum"].ToString());
				}
				string[] cdtCodesDeleted=new string[] {
					"D4320",
					"D4321",
					"D8050",
					"D8060",
					"D8690"
				};
				//Change the procedure codes' category to Obsolete.
				command="UPDATE procedurecode SET ProcCat="+POut.Long(defNum)
					+" WHERE ProcCode IN('"+string.Join("','",cdtCodesDeleted.Select(x => POut.String(x)))+"') ";
				Db.NonQ(command);
			}//end United States CDT codes update
			//Default to PrefName.InsVerifyFutureDateBenefitYear for InsVerifyFutureDatePatEnrollmentYear.  Maintain old value for backward compatibility.
			command="SELECT ValueString FROM preference WHERE PrefName='InsVerifyFutureDateBenefitYear'";
			bool insVerifyFutureDateBenefitYear=PIn.Bool(Db.GetScalar(command));
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('InsVerifyFutureDatePatEnrollmentYear','{POut.Bool(insVerifyFutureDateBenefitYear)}')";
			Db.NonQ(command);
		}//End of 21_3_22()

		private static void To21_3_28() {
			//The url we inserted for these programs is no longer valid. This is us fixing that going forward.
			//Adding extra check here because most of our customers will have already gotten this fixed in 21.2
			string command="SELECT * FROM program where (ProgName='Trophy' OR ProgName='TrophyEnhanced') AND ProgDesc LIKE '%from www.trophy-imaging.com%'";
			DataTable table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow progCur=table.Rows[i];
				if(progCur["ProgDesc"].ToString().Contains("from www.trophy-imaging.com")) {
					progCur["ProgDesc"]=progCur["ProgDesc"].ToString().Replace("from www.trophy-imaging.com","");
					long progNum=PIn.Long(progCur["ProgramNum"].ToString());
					command=$"UPDATE program SET ProgDesc='{POut.String(progCur["ProgDesc"].ToString())}' WHERE ProgramNum={POut.Long(progNum)}";
					Db.NonQ(command);
				}
			}
		}//End of 21_3_28()

		private static void To21_3_29() {
			string command;
			//Updating D9613 to default to TreatmentArea.Quad in CDT 2022 (only if user did not change from TreatmentArea.Mouth).
			if(CultureInfo.CurrentCulture.Name.EndsWith("US")) {//United States
				command="UPDATE procedurecode SET procedurecode.TreatArea=4 WHERE procedurecode.ProcCode='D9613' AND procedurecode.TreatArea=3";//4 - Quad
				Db.NonQ(command);
			}//end United States CDT codes update
		}//End of 21_3_29()

		private static void To21_3_50() {
			string command;
			//E30604 - Enterprise Pref to use PriProv's PPO fee for Hyg procs
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseHygProcUsePriProvFee','0')"; //Default to false
			Db.NonQ(command);
		}//End of 21_3_50()

		private static void To21_3_51() {
			string command;
			//B34518 - Preventing invalidated procedures from overwriting graphics on the chart
			command="UPDATE procedurelog SET HideGraphics=1 WHERE ProcStatus=6 AND IsLocked=1";
			Db.NonQ(command);
		}

		private static void To21_4_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingSelectInsFilingCodes','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoDebondProcCompletedSetsMonthsTreat','0')"; //Default to false
			Db.NonQ(command);
			command=$"INSERT INTO preference(PrefName,ValueString) VALUES('DefaultImageCategoryImportFolder','0')";
			Db.NonQ(command);
			//Add TextMessageView permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",217)"; // 217 - Text Message View
				Db.NonQ(command);
			}
			//Add TextMessageSend permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",218)"; // 218 - Text Message Send
				Db.NonQ(command);
			}
			//Updating a database to or beyond version 21.1.1 will default DynamicPayPlanTPOption to 0-None for existing Dynamic Payment Plans,
			//but None should not be an option for them. Update that column to 1-AwaitComplete for those plans which is the default for new Dynamic Payment Plans.
			command="UPDATE payplan SET payplan.DynamicPayPlanTPOption=1 WHERE payplan.DynamicPayPlanTPOption=0 AND payplan.IsDynamic=1";
			Db.NonQ(command);
			command=@"ALTER TABLE resellerservice ADD HostedUrl VARCHAR(255)";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ColorFore int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ColorTextBack int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mountdef ADD ScaleValue varchar(255) NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mount ADD ColorFore int NOT NULL";
			Db.NonQ(command);
			command=@"ALTER TABLE mount ADD ColorTextBack int NOT NULL";
			Db.NonQ(command);
			string black="-16777216";
			string white="-1";
			command=@"UPDATE mountdef SET ColorFore="+white+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorTextBack="+black+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorFore="+black+" WHERE ColorFore=0";
			Db.NonQ(command);
			command=@"UPDATE mountdef SET ColorTextBack="+white+" WHERE ColorTextBack=0";
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorFore="+white+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorTextBack="+black+" WHERE ColorBack="+black;
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorFore="+black+" WHERE ColorFore=0";
			Db.NonQ(command);
			command=@"UPDATE mount SET ColorTextBack="+white+" WHERE ColorTextBack=0";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EClipboardShowPerioGraphical','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE mobileappdevice ADD DevicePage tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitemdef ADD TextShowing text NOT NULL";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS taskattachment";
			Db.NonQ(command);
			command=@"CREATE TABLE taskattachment (
								TaskAttachmentNum bigint NOT NULL auto_increment PRIMARY KEY,
								TaskNum bigint NOT NULL,
								DocNum bigint NOT NULL,
								TextValue text NOT NULL,
								Description varchar(255) NOT NULL,
								INDEX(TaskNum),
								INDEX(DocNum)
								) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('TaskAttachmentCategory','0')";
			Db.NonQ(command);
			//Add multi-column indexes to several financial tables which will be used within the Family Balancer window.
			IndexColsAndName indexSecDateTEditPatNum=new IndexColsAndName("SecDateTEdit,PatNum","SecDateTEditPN");
			//The procedurelog table is the only table that does not have a SecDateTEdit column but instead has a 'timestamp' data type column called DateTStamp.
			IndexColsAndName indexDateTStampPatNum=new IndexColsAndName("DateTStamp,PatNum","DateTStampPN");
			AlterTable("adjustment","AdjNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("claimproc","ClaimProcNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("payplancharge","PayPlanChargeNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("paysplit","SplitNum",indexColsAndName:indexSecDateTEditPatNum);
			AlterTable("procedurelog","ProcNum",indexColsAndName:indexDateTStampPatNum);
			command="ALTER TABLE mountitemdef ADD FontSize float NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD TextShowing text NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountitem ADD FontSize float NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD IgnoreSheetDefNums text NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptPrintColorBehavior','0')";//Default to ApptPrintColorBehavior.FullColor
			Db.NonQ(command);
			//Add RxMerge permission to everyone------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",219)";// 219 - Permission for RxMerge
				 Db.NonQ(command);
			}
			command="ALTER TABLE paysplit ADD SecurityHash varchar(255) NOT NULL";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
			AlterTable("document","DocNum",new ColNameAndDef("DegreesRotated","float NOT NULL"));
			AlterTable("document","DocNum",new ColNameAndDef("IsCropOld","tinyint unsigned NOT NULL"));
			command="UPDATE document SET IsCropOld=1 WHERE CropW>0 AND CropH>0";
			Db.NonQ(command);
			command="SELECT AlertCategoryNum FROM alertcategory WHERE InternalName='eServices'";
			long alertCategoryNum=Db.GetLong(command);
			command=$@"INSERT INTO alertcategorylink (AlertCategoryNum,AlertType) VALUES({POut.Long(alertCategoryNum)},34)";//34=WebSchedRecallsNotSending alert 
			Db.NonQ(command);
			AlterTable("appointment","AptNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			AlterTable("histappointment","HistApptNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			Misc.SecurityHash.UpdateHashing();
			//Add Definition Edit permission to everyone who has Setup permissions--------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					   +"VALUES("+POut.Long(groupNum)+",220)"; // 220 - Definition Edit
				Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('EnterpriseCommlogOmitDefaults','0')"; //Default false
			Db.NonQ(command);
			LargeTableHelper.AlterTable("eservicelog","EServiceLogNum",new ColNameAndDef("Note","varchar(255) NOT NULL"));
			//Add Update Install permission to everyone who has Setup permissions---------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";//Setup
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						 +"VALUES("+POut.Long(groupNum)+",221)"; // 221 - Update Install
				Db.NonQ(command);
			}
			command = "INSERT INTO preference(PrefName,ValueString) VALUES('RxHideProvsWithoutDEA','0')"; //Default false
			Db.NonQ(command);
			command="ALTER TABLE alertitem ADD SecDateTEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS limitedbetafeature";
			Db.NonQ(command);
			command=@"CREATE TABLE limitedbetafeature (
				LimitedBetaFeatureNum bigint NOT NULL auto_increment PRIMARY KEY,
				LimitedBetaFeatureTypeNum bigint NOT NULL,
				ClinicNum bigint NOT NULL,
				IsSignedUp tinyint NOT NULL,
				INDEX(LimitedBetaFeatureTypeNum),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS apikey";
			Db.NonQ(command);
			command=@"CREATE TABLE apikey (
					APIKeyNum bigint NOT NULL auto_increment PRIMARY KEY,
					CustApiKey varchar(255) NOT NULL,
					DevName varchar(255) NOT NULL
					) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS insplanpreference";
			Db.NonQ(command);
			command=@"CREATE TABLE insplanpreference (
				InsPlanPrefNum bigint NOT NULL auto_increment PRIMARY KEY,
				PlanNum bigint NOT NULL,
				FKey bigint NOT NULL,
				FKeyType tinyint NOT NULL,
				ValueString text NOT NULL,
				INDEX(PlanNum),
				INDEX(FKey)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			LargeTableHelper.AlterTable("patient","PatNum",new ColNameAndDef("SecurityHash","varchar(255) NOT NULL"));
			Misc.SecurityHash.UpdateHashing();
			command="DROP TABLE IF EXISTS webschedcarrierrule";
			Db.NonQ(command);
			command=@"CREATE TABLE webschedcarrierrule (
				WebSchedCarrierRuleNum bigint NOT NULL auto_increment PRIMARY KEY,
				ClinicNum bigint NOT NULL,
				CarrierName varchar(255) NOT NULL,
				DisplayName varchar(255) NOT NULL,
				Message text NOT NULL,
				Rule tinyint NOT NULL,
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatRequestInsurance','0')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedNewPatRequestInsurance','0')";
			Db.NonQ(command);
			command="ALTER TABLE apptfielddef ADD ItemOrder int NOT NULL";
			Db.NonQ(command);
			command="SELECT * FROM apptfielddef ORDER BY FieldName";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long appFieldDefNum=PIn.Long(table.Rows[i]["ApptFieldDefNum"].ToString());
				command="UPDATE apptfielddef SET ItemOrder="+POut.Int(i)+" WHERE ApptFieldDefNum="+POut.Long(appFieldDefNum);
				Db.NonQ(command);
			}
		}//End of 21_4_1() method

		private static void To21_4_4() {
			string command;
			DataTable table;
			command="ALTER TABLE xwebresponse ADD LogGuid varchar(36)";
			Db.NonQ(command);
			command="ALTER TABLE payconnectresponseweb ADD LogGuid varchar(36)";
			Db.NonQ(command);
		}//End of 21_4_4() method

		private static void To21_4_7() {
			string command;
			DataTable table;
			command="ALTER TABLE payplan ADD SecurityHash varchar(255) NOT NULL";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_7() method

		private static void To21_4_8() {
			string command;
			DataTable table;
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_8() method

		private static void To21_4_9() {
			string command;
			DataTable table;
			//Insert Ai-Dental bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'AiDental', " 
				 +"'Ai-Dental', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Ai-Dental\Ai-Dental-Client\Ai-Dental.exe")+"', " 
				 +"'"+POut.String(@"link")+"', "//leave blank if none 
				 +"'')"; 
			long programNum=Db.NonQ(command,true);  
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue" 
				 +") VALUES(" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'Text file path for Ai-Dental', " 
				 +"'"+POut.String(@"C:\Ai-Dental\Ai-Dental-Client\patdata.txt")+"')"; 
			Db.NonQ(command); 
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'4', "//ToolBarsAvail.FamilyModule, Based on attached video for the job P31954
				 +"'Ai-Dental')"; 
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('IncomeTransfersMadeUponClaimReceived','0')"; //Default to unknown
			Db.NonQ(command);
			command="UPDATE displayreport SET Description='Broken Appointments' WHERE InternalName='ODBrokenAppointments' AND Description='BrokenAppointments'";
			Db.NonQ(command);
		}//End of 21_4_9() method
	
		private static void To21_4_16() {
			Misc.SecurityHash.UpdateHashing();
			Db.NonQ("ALTER TABLE emailaddress ADD QueryString varchar(1000) NOT NULL");
		}//End of 21_4_16() method

		private static void To21_4_17() {
			string command;
			DataTable table;
			command="SELECT ProgramNum FROM program WHERE ProgName='AiDental'";
			long programNum=Db.GetLong(command);
			if(programNum!=0) {
				command="UPDATE program SET CommandLine='"+POut.String(@"[PatNum].[LName].[FName]")+"' WHERE ProgramNum='"+POut.Long(programNum)+"'";
				Db.NonQ(command);
				command="DELETE FROM programproperty WHERE ProgramNum='"+POut.Long(programNum)+"' AND PropertyDesc='Text file path for Ai-Dental'";
				Db.NonQ(command);
			}  
		}//End of 21_4_17() method

		private static void To21_4_20() {
			string command;
			DataTable table;
			command="UPDATE patient SET SecurityHash=''";
			Db.NonQ(command);
			Misc.SecurityHash.ResetPatientHashing();
			Misc.SecurityHash.UpdateHashing();
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedRecallApptSearchMaximumMonths','12')";//Default to 12 months
			Db.NonQ(command);
		}//End of 21_4_20() method

		private static void To21_4_21() {
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_21() method

		private static void To21_4_23() {
			string command="";
			//E34356 - Default Prexion bridge to use PatNum instead of ChartNum
			//we don't want to alter a bridge that is currently in use
			command="SELECT ProgramNum FROM program WHERE ProgName='PreXionViewer' AND Enabled=0";
			long prexionViewerProgNum=Db.GetLong(command);
			if(prexionViewerProgNum>0){
				command="SELECT ProgramNum FROM program WHERE ProgName='PreXionAcquire'";
				long prexionAcquireProgNum=Db.GetLong(command);
				long usePatNum=0;
				//If acquire is set up a specific way then we want viewer to be set up the same way
				if(prexionAcquireProgNum>0){
					command=$"SELECT PropertyValue from programproperty WHERE PropertyDesc='Enter 0 to use PatientNum, or 1 to use ChartNum' AND ProgramNum={POut.Long(prexionAcquireProgNum)}";
					usePatNum=Db.GetLong(command);
				}
				command=$"UPDATE programproperty SET PropertyValue='{POut.Long(usePatNum)}' WHERE PropertyDesc='Enter 0 to use PatientNum, or 1 to use ChartNum' AND ProgramNum={POut.Long(prexionViewerProgNum)}";
				Db.NonQ(command);
			}
			//Add the new PaySimple PaySimplePrintReceipt property------------------------------------------------------
			command="SELECT ClinicNum FROM clinic";
			List<long> listClinicNums=Db.GetListLong(command);
			listClinicNums.Add(0);//Add HQ
			command="SELECT ProgramNum FROM program WHERE ProgName='PaySimple'";
			long progNum=PIn.Long(Db.GetScalar(command));
			foreach(long clinicNum in listClinicNums) {
				command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue,ComputerName,ClinicNum) "
					+"VALUES ("+POut.Long(progNum)+",'PaySimplePrintReceipt','1','',"+POut.Long(clinicNum)+")";
				Db.NonQ(command);
			}
		}//End of 21_4_23() method

		private static void To21_4_24() {
			string command;
			//E30604 - Enterprise Pref to use PriProv's PPO fee for Hyg procs
			command="SELECT * FROM preference WHERE PrefName='EnterpriseHygProcUsePriProvFee'";
			if(Db.GetTable(command).Rows.Count==0) { //Check to see if it's already been added
				command="INSERT INTO preference(PrefName,ValueString) VALUES ('EnterpriseHygProcUsePriProvFee','0')";//Default to false.
				Db.NonQ(command);
			}
		}//End of 21_4_24() method

		private static void To21_4_25() {
			string command;
			//B34518 - Preventing invalidated procedures from overwriting graphics on the chart
			command="UPDATE procedurelog SET HideGraphics=1 WHERE ProcStatus=6 AND IsLocked=1";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
		}

		private static void To21_4_27() {
			Misc.SecurityHash.UpdateHashing();
		}

		private static void To21_4_30() {
			Misc.SecurityHash.UpdateHashing();
		}

		private static void To21_4_38() {
			string command;
			//B35468 - Default to using new Image module for everyone.
			command="UPDATE preference SET ValueString='0' WHERE PrefName='ImagesModuleUsesOld2020'";
			Db.NonQ(command);
		}//End of 21_4_38() method

		private static void To21_4_41()	{
			Misc.SecurityHash.UpdateHashing();
		}//End of 21_4_41() method

		private static void To21_4_49() {
			string command;
			command="ALTER TABLE covcat MODIFY CovOrder INT NOT NULL";
			Db.NonQ(command);
		}//End of 21_4_49() method

		private static void To22_1_1() {
			string command;
			DataTable table;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('WebSchedExistingPatAllowDisabledRecalls','0')";//Default false
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD PrefillStatusOverride bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE eclipboardsheetdef ADD INDEX (PrefillStatusOverride)";
			Db.NonQ(command);
			command="ALTER TABLE mountdef ADD DefaultCat bigint NOT NULL";
			Db.NonQ(command);
			//Lantek clearinghouse.
			command=@"INSERT INTO clearinghouse (Description,ExportPath,Payors,Eformat,ISA05,SenderTin,ISA07,ISA08,ISA15,Password,ResponsePath,CommBridge
					,ClientProgram,LastBatchNumber,ModemPort,LoginID,SenderName,SenderTelephone,GS03,ISA02,ISA04,ISA16,SeparatorData,SeparatorSegment)
				 VALUES ('Lantek Networks Transmission Service','"+POut.String(@"C:\Lantek\Claims")+"','','5','ZZ','','ZZ','660873269','P','','"+POut.String(@"C:\Lantek\Reports")+"','20','"+POut.String(@"C:\Lantek\Program\Trakker.exe")+"',0,0,'','','7877877878','660873269','','','','','')";
			Db.NonQ(command);
			command="UPDATE clearinghouse SET HqClearinghouseNum=ClearinghouseNum WHERE HqClearinghouseNum=0";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ImagingDefaultScaleValue','')";
			Db.NonQ(command);
			command="ALTER TABLE apptviewitem ADD IsMobile tinyint NOT NULL";
			Db.NonQ(command);
			//From this point on, group permissions with only FKey of 0 will denote access to everything within that permission (including hidden).
			//Go through each user group's permissions and check if they have all Reports permissions.
			//If so, remove all non-zero FKey entries and keep/insert one FKey of 0.
			//If they have some permissions, remove FKey of 0.
			command="SELECT COUNT(*) FROM patfielddef "
				+"WHERE FieldName IN ('SPID#','Eligibility Status','Household Gross Income','Household % of Poverty')";
			bool hasArizonaPrimaryCare=PIn.Long(Db.GetCount(command))==4;
			command="SELECT DisplayReportNum FROM displayreport";
			if(!hasArizonaPrimaryCare) {
				command+=" WHERE category!=5";//5 - Arizona Primary Care reports
			}
			List<long> listDisplayReportNums=Db.GetListLong(command);
			command="SELECT UserGroupNum FROM usergroup";
			List<long> listUserGroupNums=Db.GetListLong(command);
			for(int i=0;i < listUserGroupNums.Count;i++) {
				command="SELECT FKey "
					+"FROM grouppermission "
					+"WHERE PermType=22 "//22 - Reports
					+"AND FKey!=0 "
					+$"AND UserGroupNum={POut.Long(listUserGroupNums[i])}";
				List<long> listDisplayReportPermissionsForGroup=Db.GetListLong(command).Distinct().ToList();
				//Check to see if every single available display report has an explicit groupermission set.
				List<long> listIntersectingDisplayReportNums=listDisplayReportNums.Intersect(listDisplayReportPermissionsForGroup).ToList();
				if(listIntersectingDisplayReportNums.Count==listDisplayReportNums.Count) {//Has all permissions.
					command=$"DELETE FROM grouppermission WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} AND PermType=22";
					Db.NonQ(command);
					command=$"INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType,FKey) VALUES (0,{POut.Long(listUserGroupNums[i])},22,0)";
					Db.NonQ(command);
				}
				else {//Has some or no permissions, remove FKey of 0 regardless.
					command="DELETE FROM grouppermission "
						+$"WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} "
						+"AND PermType=22 "
						+"AND FKey=0";
					Db.NonQ(command);
				}
			}
			//Do the same with DashboardWidgets permissions.
			command="SELECT SheetDefNum FROM sheetdef WHERE SheetType=25";//25 - PatientDashboardWidgets
			List<long> listDashboardWidgetNums=Db.GetListLong(command);
			for(int i=0;i < listUserGroupNums.Count;i++) {
				command="SELECT FKey "
					+"FROM grouppermission "
					+"WHERE PermType=173 "//173 - DashboardWidgets
					+"AND FKey!=0 "
					+$"AND UserGroupNum={listUserGroupNums[i]}";
				List<long> listDashboardWidgetPermissionsForGroup=Db.GetListLong(command).Distinct().ToList();
				//Check to see if every single available dashboard widget has an explicit groupermission set.
				List<long> listIntersectingDashboardWidgetNums=listDashboardWidgetNums.Intersect(listDashboardWidgetPermissionsForGroup).ToList();
				if(listIntersectingDashboardWidgetNums.Count==listDashboardWidgetNums.Count) {//Has all permissions.
					command=$"DELETE FROM grouppermission WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} AND PermType=173";
					Db.NonQ(command);
					command=$"INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType,FKey) VALUES (0,{POut.Long(listUserGroupNums[i])},173,0)";
					Db.NonQ(command);
				}
				else {//Has some or no permissions, remove FKey of 0 regardless.
					command="DELETE FROM grouppermission "
						+$"WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} "
						+"AND PermType=173 "
						+"AND FKey=0";
					Db.NonQ(command);
				}
			}
			//Do the same with AdjustmentTypeDeny permissions.
			command="SELECT DefNum FROM definition WHERE Category=1";//1 - DefCat.AdjTypes
			List<long> listAdjustmentTypeNums=Db.GetListLong(command);
			for(int i=0;i < listUserGroupNums.Count;i++) {
				command="SELECT FKey "
					+"FROM grouppermission "
					+"WHERE PermType=222 "//222 - AdjustmentTypeDeny
					+"AND FKey!=0 "
					+$"AND UserGroupNum={listUserGroupNums[i]}";
				List<long> listAdjustmentTypeDenyPermissionsForGroup=Db.GetListLong(command).Distinct().ToList();
				//Check to see if every single available adjustment type has an explicit grouppermission set.
				List<long> listIntersectingAdjustmentTypeNums=listAdjustmentTypeNums.Intersect(listAdjustmentTypeDenyPermissionsForGroup).ToList();
				if(listIntersectingAdjustmentTypeNums.Count==listAdjustmentTypeNums.Count) {//Has all permissions denied.
					command=$"DELETE FROM grouppermission WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} AND PermType=222";
					Db.NonQ(command);
					command=$"INSERT INTO grouppermission (NewerDays,UserGroupNum,PermType,FKey) VALUES (0,{POut.Long(listUserGroupNums[i])},222,0)";
					Db.NonQ(command);
				}
				else {//Has some or no permissions, remove FKey of 0 regardless.
					command="DELETE FROM grouppermission "
						+$"WHERE UserGroupNum={POut.Long(listUserGroupNums[i])} "
						+"AND PermType=222 "
						+"AND FKey=0";
					Db.NonQ(command);
				}
			}
			//F31254 Default perio preference.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PerioDefaultProbeDepths','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingElectIncludeClinicNums','0')";
			Db.NonQ(command);
			//Add CarrierEdit to groups with InsPlanEdit------------------------------------------------------
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=110"; //InsPlanEdit
			table=Db.GetTable(command);
			long groupNum;
			for(int i=0;i<table.Rows.Count;i++) {
				 groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				 command="INSERT INTO grouppermission (UserGroupNum,PermType) "
						+"VALUES("+POut.Long(groupNum)+",224)"; //CarrierEdit
				 Db.NonQ(command);
			}
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ClaimPrimaryRecievedForceSecondaryStatus','0')";//Default false
			Db.NonQ(command);
			//Insert MedLink bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note" 
				 +") VALUES(" 
				 +"'MeditLink', " 
				 +"'Medit Link from www.meditlink.com', " 
				 +"'0', " 
				 +"'"+POut.String(@"C:\Program Files\Medit\Medit Link\Medit Link\Medit_Link.exe")+"', " 
				 +"'"+POut.String(@"{"+"\"NameFL\": \"[NameFL]\", \"PatNum\": \"[PatNum]\", \"PatientGenderMF\": \"[PatientGenderMF]\", \"Birthdate_yyyyMMdd\": \"[Birthdate_yyyyMMdd]\"}")+"', "//leave blank if none 
				 +"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) " 
				 +"VALUES (" 
				 +"'"+POut.Long(programNum)+"', " 
				 +"'2', "//ToolBarsAvail.ChartModule 
				 +"'MeditLink')"; 
			Db.NonQ(command); 
			//end MedLink bridge 
			//E32326 - Adjustments offsetting each other
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AdjustmentsOffsetEachOther','1')"; //Default to true
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS apisubscription";
			Db.NonQ(command);
			command=@"CREATE TABLE apisubscription (
				ApiSubscriptionNum bigint NOT NULL auto_increment PRIMARY KEY,
				EndPointUrl varchar(255) NOT NULL,
				Workstation varchar(255) NOT NULL,
				CustomerKey varchar(255) NOT NULL,
				WatchTable varchar(255) NOT NULL,
				PollingSeconds int NOT NULL,
				UiEventType varchar(255) NOT NULL,
				DateTimeStart datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DateTimeStop datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				Note varchar(255) NOT NULL,
				INDEX(PollingSeconds)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="SELECT ValueString FROM preference WHERE PrefName='RecallMaxNumberReminders'";
			long maxAutoReminders=Db.GetLong(command);
			command="INSERT INTO preference (PrefName,ValueString,Comments) VALUES('RecallMaxNumberAutoReminders','"+POut.Long(maxAutoReminders)+"','')";
			Db.NonQ(command);
			command="ALTER TABLE insfilingcode ADD ExcludeOtherCoverageOnPriClaims tinyint NOT NULL";
			Db.NonQ(command);
			//F31366 - Billing Options #days option.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('BillingDaysExcludeInsPending','0')";
			Db.NonQ(command);
			if(!IndexExists("claim","PatNum,ClaimStatus,ClaimType,DateSent")) {
				command="ALTER TABLE claim ADD INDEX PatStatusTypeDate (PatNum,ClaimStatus,ClaimType,DateSent)";
				List<string> listIndexNames=GetIndexNames("claim","PatNum,ClaimStatus,ClaimType");
				if(listIndexNames.Count>0) {
					command+=","+string.Join(",",listIndexNames.Select(x => $"DROP INDEX {x}"));
				}
				Db.NonQ(command);
			}
			//Insert preference ApptConfirmExcludeGeneralMessage using AppointmentTimeSeatedTrigger, AppointmentTimeSeatedTrigger, AppointmentTimeDismissedTrigger.
			command="SELECT preference.ValueString FROM preference WHERE preference.PrefName IN ('AppointmentTimeArrivedTrigger',"
				+"'AppointmentTimeSeatedTrigger','AppointmentTimeDismissedTrigger')";
			List<long> listDefNums=Db.GetListLong(command,hasExceptions:false).Where(x => x!=0).Distinct().OrderBy(x => x).ToList();//Using OrderBy to save database calls when editing this pref.
			string defNums=string.Join(",",listDefNums);
			command="INSERT INTO preference (PrefName,ValueString) VALUES ('ApptConfirmExcludeGeneralMessage','"+defNums+"')";
			Db.NonQ(command);
			//Insert new ApptConfirmed status type for 'Out the Door'.
			command="SELECT MAX(ItemOrder)+1 FROM definition WHERE Category=2";//Category 2=ApptConfirmed DefCat.
			string maxOrder=Db.GetScalar(command);
			if(maxOrder=="") {
				maxOrder="0";
			}
			command="INSERT INTO definition (Category, ItemOrder, ItemName, ItemValue) VALUES (2,"+maxOrder+",'Out the Door','WebSched')";
			Db.NonQ(command,true);
		}

		private static void To22_1_4() {
			string command;
			DataTable table;
			command="INSERT INTO preference (PrefName,ValueString) VALUES('EnterpriseManualRefreshMainTaskLists','0')";
			Db.NonQ(command);
		}//End of 22_1_4() method

		private static void To22_1_6() {
			string command;
			DataTable table;
			command="DROP TABLE IF EXISTS eclipboardimagecapturedef";
			Db.NonQ(command);
			command=@"CREATE TABLE eclipboardimagecapturedef (
				EClipboardImageCaptureDefNum bigint NOT NULL auto_increment PRIMARY KEY,
				DefNum bigint NOT NULL,
				IsSelfPortrait tinyint NOT NULL,
				FrequencyDays int NOT NULL,
				ClinicNum bigint NOT NULL,
				INDEX(DefNum),
				INDEX(ClinicNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS eclipboardimagecapture";
			Db.NonQ(command);
			command=@"CREATE TABLE eclipboardimagecapture (
				EClipboardImageCaptureNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				DefNum bigint NOT NULL,
				IsSelfPortrait tinyint NOT NULL,
				DateTimeUpserted datetime NOT NULL DEFAULT '0001-01-01 00:00:00',
				DocNum bigint NOT NULL,
				INDEX(PatNum),
				INDEX(DefNum),
				INDEX(DocNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//Converting the default clinic preference to records inside the new eclipboardimagecapturedef table.
			//This pref is now deprecated. 
			command="SELECT ValueString FROM preference where PrefName='EClipboardImageCaptureDefs'";
			string eClipboardImageCaptureDefsPref=Db.GetScalar(command);
			List<string> listDefNums=new List<string>();
			if(!string.IsNullOrWhiteSpace(eClipboardImageCaptureDefsPref)){
				listDefNums=eClipboardImageCaptureDefsPref.Split(',').ToList();
			}
			for(int i=0;i<listDefNums.Count;i++) { 
				command=$@"INSERT INTO eclipboardimagecapturedef (DefNum,IsSelfPortrait,FrequencyDays,ClinicNum) 
					VALUES ({POut.String(listDefNums[i])},{POut.Bool(false)},{POut.Int(0)},{POut.Long(0)})";
				Db.NonQ(command);
			}
			//Converting the default self portrait pref into a record in the new eclipboardimagecapturedef table.
			command="SELECT ValueString FROM preference WHERE PrefName='EClipboardAllowSelfPortraitOnCheckIn'";
			bool allowSelfPortrait=PIn.Bool(Db.GetScalar(command));
			//Category 18 is the DefCat 'Image Categories. 'P' is the Patient Pictures usage, where we store selfies.
			command="SELECT DefNum FROM definition WHERE Category=18 AND ItemValue LIKE '%P%'";
			long patientPicturesImageCatDefNum=Db.GetLong(command);
			if(allowSelfPortrait) {
				command=$@"INSERT INTO eclipboardimagecapturedef (DefNum,IsSelfPortrait,FrequencyDays,ClinicNum) 
					VALUES ({POut.Long(patientPicturesImageCatDefNum)},{POut.Bool(true)},{POut.Int(0)},{POut.Long(0)})";
				Db.NonQ(command);
			}
			//Now convert the EClipboardImageCaptureDefs pref for clinics, if any. 
			command="SELECT * FROM clinicpref WHERE PrefName='EClipboardImageCaptureDefs'";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				DataRow dataRow=table.Rows[i];
				eClipboardImageCaptureDefsPref=dataRow["ValueString"].ToString();
				listDefNums=new List<string>();
				if(!string.IsNullOrWhiteSpace(eClipboardImageCaptureDefsPref)){
					listDefNums=eClipboardImageCaptureDefsPref.Split(',').ToList();
				}
				long clinicNum=PIn.Long(dataRow["ClinicNum"].ToString());
				for(int j=0;j<listDefNums.Count;j++) { 
					command=$@"INSERT INTO eclipboardimagecapturedef (DefNum,IsSelfPortrait,FrequencyDays,ClinicNum) 
						VALUES ({POut.String(listDefNums[j])},{POut.Bool(false)},{POut.Int(0)},{POut.Long(clinicNum)})";
					Db.NonQ(command);
				}
				command=$"SELECT ValueString FROM clinicpref WHERE ClinicNum={POut.Long(clinicNum)} AND PrefName='EClipboardAllowSelfPortraitOnCheckIn'";
				allowSelfPortrait=PIn.Bool(Db.GetScalar(command));
				if(allowSelfPortrait) {
					command=$@"INSERT INTO eclipboardimagecapturedef (DefNum,IsSelfPortrait,FrequencyDays,ClinicNum) 
						VALUES ({POut.Long(patientPicturesImageCatDefNum)},{POut.Bool(true)},{POut.Int(0)},{POut.Long(clinicNum)})";
					Db.NonQ(command);
				}
			}
		}//End of 22_1_6() method

		private static void To22_1_7() {
			string command;
			//I6045 - New pref to prevent processing transactions from an existing payment
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PaymentsCompletedDisableMerchantButtons','1')"; //Default to true
			Db.NonQ(command);
			command="ALTER TABLE payment ADD IsCcCompleted tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 22_1_7() method

		private static void To22_1_12() {
			Misc.SecurityHash.UpdateHashing();
		}//End of 22_1_12() method

		private static void To22_1_16() {
			string command;
			//B35292 - Update Lantek's phone number field to be empty if it's still the originally inserted value from 22.1.0.0 conversion.
			command=@"UPDATE clearinghouse SET 
				SenderTelephone='' 
				WHERE Description='Lantek Networks Transmission Service' 
				AND SenderTelephone='7877877878'";
			Db.NonQ(command);
		}//End of 22_1_16() method

		private static void To22_1_17() {
			string command;
			//H35078 - WFH ClockEvents
			command="ALTER TABLE clockevent ADD IsWorkingHome tinyint NOT NULL";
			Db.NonQ(command);
		}//End of 22_1_17() method

		private static void To22_1_18() {
			//Updating ItemValue for Out the Door, previously set to WebSched.
			string command=@"UPDATE definition SET
			ItemValue='OutTheDoor'
			WHERE ItemName='Out the Door' 
			AND ItemValue='WebSched'";
			Db.NonQ(command);
			//Enabling dismiss trigger by default for General Messages. If the DefNum for the dismiss trigger is found in the exclude preference, remove it and update.
			command="SELECT ValueString FROM preference WHERE PrefName='ApptConfirmExcludeGeneralMessage'";
			List<string> defNums=Db.GetScalar(command).Split(',').ToList();
			command="SELECT ValueString FROM preference WHERE PrefName='AppointmentTimeDismissedTrigger'";
			string defNumDismissed=Db.GetScalar(command);
			if(defNums.Contains(defNumDismissed)) {
				defNums.Remove(defNumDismissed);
				command="UPDATE preference SET ValueString='" +string.Join(",",defNums)+"' WHERE PrefName='ApptConfirmExcludeGeneralMessage'";
				Db.NonQ(command);
			}
		}//End of 22_1_18() method

		private static void To22_1_19() {
			string command;
			//B35468 - Default to using new Image module for everyone.
			command="UPDATE preference SET ValueString='0' WHERE PrefName='ImagesModuleUsesOld2020'";
			Db.NonQ(command);
		}//End of 22_1_19() method

		private static void To22_1_24(){
			Misc.SecurityHash.UpdateHashing();
		}//End of 22_1_24() method

		private static void To22_1_26() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('AccountingInvoiceAttachmentsSaveInDatabase','1')";//true to preserve existing behavior.
			Db.NonQ(command);
			command="ALTER TABLE transactioninvoice ADD FilePath varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 22_1_26() method

		private static void To22_1_28() {
			string command;
			//I35616 - Enhance Plugin Whitelisting Verification.
			command="INSERT INTO preference(PrefName,ValueString) VALUES('DatabaseIntegritiesWhiteList','')"; 
			Db.NonQ(command);
		}//End of 22_1_28() method

		private static void To22_1_31() {
			string command;
			command = "ALTER TABLE clinic ADD EmailAliasOverride varchar(255) NOT NULL";
			Db.NonQ(command);
		}//End of 22_1_31() method

		private static void To22_1_34() {
			string command;
			command="ALTER TABLE covcat MODIFY CovOrder INT NOT NULL";
			Db.NonQ(command);
		}//End of 22_1_34() method

		private static void To22_1_35() {
			#region InsVerifyChecks
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsBatchVerifyCreateAdjustments','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsBatchVerifyCheckDeductible','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsBatchVerifyCheckAnnualMax','0')"; //Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsBatchVerifyChangeInsHist','0')";//Default to false
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsBatchVerifyChangeEffectiveDates','0')";//Default to false 
			Db.NonQ(command);
			//We are changing default behavior of automated Insurance verification to be controlled by preferences.
			//We must determine if this is pertinent to the customer, and if it is, display a notification to each of their administrators upon updating.
			DataTable tableScheduledProcessesInsVerify=Db.GetTable("SELECT * FROM scheduledprocess WHERE ScheduledAction='InsVerifyBatch'");//ScheduledActionEnum.InsVerifyBatch=1
			if(tableScheduledProcessesInsVerify.Rows.Count>0) {
				//If they do, find their admin users.
				DataTable tableUserOdAdmins=Db.GetTable("SELECT UserNum FROM usergroupattach " +
					"WHERE usergroupattach.UserGroupNum IN (SELECT UserGroupNum FROM grouppermission WHERE PermType=24)");//Permissions.SecurityAdmin=24
				if(tableUserOdAdmins!=null && tableUserOdAdmins.Rows.Count>0) {
					//If we found their admin users, create an alert for each one, notifying them of the changed behavior.
					string description= "Your office has batch insurance verification scheduled. By default, batch insurance verification no longer checks deductibles or annual maximums, and it does not make changes to plan effective dates, insurance adjustments, or insurance history. These settings can be changed on the Scheduled Processes window (See documentation for Scheduled Processes for details).";
					for(int i =0;i<tableUserOdAdmins.Rows.Count;i++) {
						long userNum=PIn.Long(tableUserOdAdmins.Rows[i]["usernum"].ToString());
						command=$"INSERT INTO alertitem (ClinicNum,Description,Type,Severity,Actions,FormToOpen,FKey,ItemValue,UserNum) VALUES (" +
							//AlertType.Generic=0, SeverityType.Medium, ActionType.Delete|ActionType.MarkAsRead=5, FormToOpen.None=0, Fkey=0
							$"-1, '{POut.String(description)}', 0, 2, 5, 0, 0, '', {POut.Long(userNum)})";
						Db.NonQ(command);
					}
				}
			}	
			#endregion
		}//End of 22_1_35() method

		private static void To22_1_37() {
			string command;
			command="UPDATE preference SET ValueString='https://www.patientviewer.com' WHERE PrefName='PatientPortalURL' AND ValueString LIKE 'System.Linq%'";
			Db.NonQ(command);
		}//End of 22_1_37() method

		private static void To22_1_39() {
			string command;
			DataTable table;
			command="SELECT DocNum FROM document WHERE MountItemNum NOT IN(SELECT MountItemNum FROM mountitem) AND MountItemNum!=0";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long docNum=PIn.Long(table.Rows[i]["DocNum"].ToString());
				command="UPDATE document SET MountItemNum=0 WHERE DocNum="+POut.Long(docNum);
				Db.NonQ(command);
			}
		}//End of 22_1_39() method

		private static void To22_1_44() {
			RemoveDuplicateProcCodesFromFrequencyLimitationPreferences();
		}//End of 22_1_44() method


		private static void To22_2_1() {
			DataTable table;
			string upgrading = "Upgrading database to version: 22.2.0. ";
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			string command;
			#region ApptGeneralMessageSent
			string tableName = POut.String("apptgeneralmessagesent");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"CHANGE SmsSendStatus SendStatus tinyint NOT NULL,"+
				"ADD ApptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD MessageType tinyint NOT NULL,"+
				"ADD MessageFk bigint NOT NULL,"+
				"ADD DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD ResponseDescript text NOT NULL,"+
				"ADD INDEX (MessageFk)";
			Db.NonQ(command);
			//Original rows become TEXT rows.
			command=$"UPDATE {tableName} SET "+
				"MessageType=1"; //TEXT
			Db.NonQ(command);
			//Duplicate original rows as EMAIL rows.
			command=$"INSERT INTO {tableName} "+
				"("+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"EmailSendStatus,"+
					"SendStatus,"+
					"MessageType,"+
					"MessageFk,"+
					"DateTimeSent,"+
					"ResponseDescript"+
				") "+
				"SELECT "+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"0,"+
					"EmailSendStatus,"+
					"2,"+//Email
					"0,"+
					"DateTimeSent,"+
					"ResponseDescript "+
				$"FROM {tableName}";
			Db.NonQ(command);
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN EmailSendStatus";
			Db.NonQ(command);
			#endregion
			#region ApptReminderSent
			tableName=POut.String("apptremindersent");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"ADD PatNum bigint NOT NULL,"+
				"ADD ClinicNum bigint NOT NULL,"+
				"ADD SendStatus tinyint NOT NULL,"+
				"ADD MessageType tinyint NOT NULL,"+
				"ADD MessageFk bigint NOT NULL,"+
				"ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD ResponseDescript text NOT NULL,"+
				"ADD INDEX (PatNum),"+
				"ADD INDEX (ClinicNum),"+
				"ADD INDEX (MessageFk)";
			Db.NonQ(command);
			//Original rows become TEXT rows.
			command=$"UPDATE {tableName} SET "+
				"SendStatus=IF(IsSmsSent,3,4),"+//SendSuccessful or SendFailed
				"MessageType=1"; //TEXT
			Db.NonQ(command);
			//Duplicate original rows as EMAIL rows.
			command=$"INSERT INTO {tableName} "+
				"("+
					"ApptNum,"+
					"ApptDateTime,"+
					"TSPrior,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"ApptReminderRuleNum,"+
					"IsSmsSent,"+
					"IsEmailSent,"+
					"SendStatus,"+
					"MessageType,"+
					"MessageFk,"+
					"DateTimeSent,"+
					"ResponseDescript"+
				") "+
				"SELECT "+
					"ApptNum,"+
					"ApptDateTime,"+
					"TSPrior,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"ApptReminderRuleNum,"+
					"0,"+
					"0,"+
					"IF(IsEmailSent,3,4),"+//SendSuccessful or SendFailed
					"2,"+//Email
					"0,"+
					"DateTimeSent,"+
					"ResponseDescript "+
				$"FROM {tableName}";
			Db.NonQ(command);
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN IsSmsSent,"+
				"DROP COLUMN IsEmailSent";
			Db.NonQ(command);
			#endregion
			#region ApptThankYouSent
			tableName=POut.String("apptthankyousent");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"CHANGE SmsSentStatus SendStatus tinyint NOT NULL,"+
				"ADD MessageType tinyint NOT NULL,"+
				"ADD MessageFk bigint NOT NULL,"+
				"ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD INDEX (MessageFk)";
			Db.NonQ(command);
			//Original rows become TEXT rows.
			command=$"UPDATE {tableName} SET "+
				"MessageType=1"; //TEXT
			Db.NonQ(command);
			//Duplicate original rows as EMAIL rows.
			command=$"INSERT INTO {tableName} "+
				"("+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"EmailSentStatus,"+
					"SendStatus,"+
					"MessageType,"+
					"MessageFk,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"GuidMessageToMobile,"+
					"ApptDateTime,"+
					"ApptSecDateTEntry,"+
					"DateTimeThankYouTransmit,"+
					"DoNotResend,"+
					"ShortGUID"+
				") "+
				"SELECT "+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"0,"+
					"EmailSentStatus,"+
					"2,"+//Email
					"0,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"'',"+
					"ApptDateTime,"+
					"ApptSecDateTEntry,"+
					"DateTimeThankYouTransmit,"+
					"DoNotResend,"+
					"ShortGUID "+
				$"FROM {tableName}";
			Db.NonQ(command);
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN EmailSentStatus,"+
				"DROP COLUMN IsForSms,"+
				"DROP COLUMN IsForEmail,"+
				"DROP COLUMN PhonePat,"+
				"DROP COLUMN GuidMessageToMobile,"+
				"DROP COLUMN MsgTextToMobileTemplate,"+
				"DROP COLUMN MsgTextToMobile,"+
				"DROP COLUMN EmailSubjTemplate,"+
				"DROP COLUMN EmailSubj,"+
				"DROP COLUMN EmailTextTemplate,"+
				"DROP COLUMN EmailText,"+
				"DROP COLUMN ShortGuidEmail";
			Db.NonQ(command);
			#endregion
			#region ConfirmationRequest
			tableName=POut.String("confirmationrequest");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"CHANGE SmsSentOk SendStatus tinyint NOT NULL,"+
				"CHANGE AptDateTimeOrig ApptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD MessageType tinyint NOT NULL,"+
				"ADD MessageFk bigint NOT NULL,"+
				"ADD DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD INDEX (MessageFk)";
			Db.NonQ(command);
			//Original rows become TEXT rows.
			command=$"UPDATE {tableName} SET "+
				"SendStatus=IF(SendStatus,3,4),"+//SendSuccessful or SendFailed
				"MessageFk=(SELECT SmsToMobileNum FROM smstomobile WHERE smstomobile.GuidMessage=GuidMessageToMobile LIMIT 1),"+//Must retain FK relationship to smstomobile
				"MessageType=1"; //TEXT
			Db.NonQ(command);
			//Duplicate original rows as EMAIL rows.
			command=$"INSERT INTO {tableName} "+
				"("+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"EmailSentOk,"+
					"SendStatus,"+
					"MessageType,"+
					"MessageFk,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"DateTimeConfirmExpire,"+
					"ConfirmCode,"+
					"DateTimeConfirmTransmit,"+
					"DateTimeRSVP,"+
					"RSVPStatus,"+
					"GuidMessageFromMobile,"+
					"ApptDateTime,"+
					"DoNotResend,"+
					"ShortGUID"+
				") "+
				"SELECT "+
					"ApptNum,"+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"TSPrior,"+
					"ApptReminderRuleNum,"+
					"0,"+
					"IF(EmailSentOk,3,4),"+//SendSuccessful or SendFailed
					"2,"+//Email
					"0,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"DateTimeConfirmExpire,"+
					"ConfirmCode,"+
					"DateTimeConfirmTransmit,"+
					"DateTimeRSVP,"+
					"RSVPStatus,"+
					"GuidMessageFromMobile,"+
					"ApptDateTime,"+
					"DoNotResend,"+
					"ShortGuidEmail "+
				$"FROM {tableName}";
			Db.NonQ(command);
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN IsForSms,"+
				"DROP COLUMN IsForEmail,"+
				"DROP COLUMN PhonePat,"+
				"DROP COLUMN EmailSentOk,"+
				"DROP COLUMN GuidMessageToMobile,"+
				"DROP COLUMN MsgTextToMobileTemplate,"+
				"DROP COLUMN MsgTextToMobile,"+
				"DROP COLUMN EmailSubjTemplate,"+
				"DROP COLUMN EmailSubj,"+
				"DROP COLUMN EmailTextTemplate,"+
				"DROP COLUMN EmailText,"+
				"DROP COLUMN SecondsFromEntryToExpire,"+
				"DROP COLUMN ShortGuidEmail";
			Db.NonQ(command);
			#endregion
			#region PatientPortalInvite
			tableName=POut.String("patientportalinvite");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"CHANGE AptNum ApptNum bigint NOT NULL,"+
				"CHANGE EmailSendStatus SendStatus tinyint NOT NULL,"+
				"ADD MessageType tinyint NOT NULL,"+
				"CHANGE EmailMessageNum MessageFk bigint NOT NULL,"+
				"ADD DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"CHANGE Note ResponseDescript text NOT NULL,"+
				"ADD ApptReminderRuleNum bigint NOT NULL,"+
				"ADD ApptDateTime datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD INDEX (MessageFk),"+
				"ADD INDEX (ApptReminderRuleNum)";
			Db.NonQ(command);
			//Original rows become EMAIL rows.
			command=$"UPDATE {tableName} SET "+
				"MessageType=2"; //EMAIL
			Db.NonQ(command);
			//No row duplication because PatientPortalInvites have only been Email up to this point.
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN TemplateEmail,"+
				"DROP COLUMN TemplateEmailSubj";
			Db.NonQ(command);
			#endregion
			#region PromotionLog
			tableName=POut.String("promotionlog");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.
			command=$"ALTER TABLE {tableName} "+
				"ADD ClinicNum bigint NOT NULL,"+
				"ADD SendStatus tinyint NOT NULL,"+
				"ADD MessageType tinyint NOT NULL,"+
				"CHANGE EmailMessageNum MessageFk bigint NOT NULL,"+
				"ADD DateTimeEntry datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD ResponseDescript text NOT NULL,"+
				"ADD ApptReminderRuleNum bigint NOT NULL,"+
				"ADD INDEX (ClinicNum),"+
				"ADD INDEX (MessageFk),"+
				"ADD INDEX (ApptReminderRuleNum)";
			Db.NonQ(command);
			command=$"UPDATE {tableName} SET "+
				//PromotionLogStatus => 0=Unknown,1=Pending,2=Bounced,3=Unsubscribed,4,Complaint,5=Delivered,6=Failed,7=Opened
				//AutoCommStatus => 0=Undefined,1=DoNotSend,2=SendNotAttempted,3=SendSuccessful,4=SendFailed,5=SentAwaitingReceipt
				"SendStatus=CASE PromotionStatus "+
					"WHEN 0 THEN 0 "+ //Unknown => Undefined
					"WHEN 1 THEN 5 "+ //Pending => SentAwaitingReceipt
					"WHEN 5 THEN 3 "+ //Delivered => SendSuccessful
					"WHEN 6 THEN 4 "+ //Failed => SendFailed
					"WHEN 7 THEN 3 "+ //Delivered => SendSuccessful
					"ELSE 4 "+ //Bounced/Unsubscribed/Complaint => SendFailed
					"END, "+
				"MessageType=2"; //EMAIL
			Db.NonQ(command);
			#endregion
			#region WebSchedRecall
			tableName=POut.String("webschedrecall");
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading} Altering {tableName}.");//No translation in convert script.	
			command=$"ALTER TABLE {tableName} "+
				"ADD CommlogNum bigint NOT NULL,"+
				"CHANGE ShortGUIDSms ShortGUID varchar(255) NOT NULL,"+
				"CHANGE SmsSendStatus SendStatus tinyint NOT NULL,"+
				"ADD MessageType tinyint NOT NULL,"+
				"ADD MessageFk bigint NOT NULL,"+
				"CHANGE DateTimeReminderSent DateTimeSent datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"CHANGE DateDue DateDue datetime NOT NULL DEFAULT '0001-01-01 00:00:00',"+
				"ADD ApptReminderRuleNum bigint NOT NULL,"+
				"ADD INDEX (CommlogNum),"+
				"ADD INDEX (MessageFk),"+
				"ADD INDEX (ApptReminderRuleNum),"+
				"ADD INDEX (DateTimeEntry)";
			Db.NonQ(command);
			//Special case:  We are adding a Cleanup query to AutoCommWebSchedRecall.  Better to run a potentially long delete here as opposed to tying up
			//the Recall thread's first run and/or causing slowness during workstation runtime.
			command="SELECT ValueString FROM preference WHERE PrefName='RecallDaysPast' LIMIT 1";
			string recallDaysPastString = Db.GetScalar(command);
			if(recallDaysPastString!="-1") {
				//We only perform the cleanup if the office has a lower bound configured for Recall date range.
				DateTime dateWebSchedRecallCleanup = DateTime.Today.AddDays(-PIn.Int(recallDaysPastString)).AddYears(-1);
				command=$"DELETE FROM {tableName} WHERE DateTimeEntry<{POut.Date(dateWebSchedRecallCleanup)}";
				Db.NonQ(command);
			}
			//Original rows become TEXT rows.
			command=$"UPDATE {tableName} SET "+
				"MessageFk=(SELECT SmsToMobileNum FROM smstomobile WHERE smstomobile.GuidMessage=GuidMessageToMobile LIMIT 1),"+
				"MessageType=1"; //TEXT
			Db.NonQ(command);
			//Duplicate original rows as EMAIL rows.
			command=$"INSERT INTO {tableName} "+
				"("+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"ApptReminderRuleNum,"+
					"EmailSendStatus,"+
					"SendStatus,"+
					"MessageType,"+
					"MessageFk,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"RecallNum,"+
					"DateDue,"+
					"ReminderCount,"+
					"PreferRecallMethod,"+
					"DateTimeSendFailed,"+
					"Source"+
				") "+
				"SELECT "+
					"PatNum,"+
					"ClinicNum,"+
					"DateTimeEntry,"+
					"ApptReminderRuleNum,"+
					"0,"+
					"EmailSendStatus,"+
					"2,"+//Email
					"0,"+
					"DateTimeSent,"+
					"ResponseDescript,"+
					"RecallNum,"+
					"DateDue,"+
					"ReminderCount,"+
					"PreferRecallMethod,"+
					"DateTimeSendFailed,"+
					"Source "+
				$"FROM {tableName}";
			Db.NonQ(command);
			command=$"ALTER TABLE {tableName} "+
				"DROP COLUMN EmailSendStatus,"+
				"DROP COLUMN PhonePat,"+
				"DROP COLUMN EmailPat,"+
				"DROP COLUMN MsgTextToMobileTemplate,"+
				"DROP COLUMN MsgTextToMobile,"+
				"DROP COLUMN EmailSubjTemplate,"+
				"DROP COLUMN EmailSubj,"+
				"DROP COLUMN EmailTextTemplate,"+
				"DROP COLUMN EmailText,"+
				"DROP COLUMN GuidMessageToMobile,"+
				"DROP COLUMN ShortGUIDEmail,"+
				"DROP COLUMN PreferRecallMethod";
			Db.NonQ(command);
			#endregion
			#region Read Only Tasks
			command="ALTER TABLE task ADD IsReadOnly BOOL NOT NULL DEFAULT FALSE";
			Db.NonQ(command);
			command="ALTER TABLE taskhist ADD IsReadOnly BOOL NOT NULL DEFAULT FALSE";
			Db.NonQ(command);
			#endregion
			ODEvent.Fire(ODEventType.ConvertDatabases,upgrading);//No translation in convert script.
			command="ALTER TABLE mount ADD FlipOnAcquire tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountdef ADD FlipOnAcquire tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mount ADD AdjModeAfterSeries tinyint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE mountdef ADD AdjModeAfterSeries tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('VideoImageCategoryDefault','0')";
			Db.NonQ(command);
			//jordan We rarely do this, but it's worth it here. Pref not hit when starting up.
			command="UPDATE preference SET PrefName='ImageCategoryDefault' WHERE PrefName='DefaultImageCategoryImportFolder'";
			Db.NonQ(command);
			command="ALTER TABLE computerpref ADD VideoRectangle varchar(255) NOT NULL";
			Db.NonQ(command);
			command="UPDATE definition SET ItemValue= REPLACE(ItemValue,'X','XM') WHERE Category=18;";//imageCats
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ApptWeekViewStartDay','0')";
			Db.NonQ(command);
			command="UPDATE displayreport SET Description='Write-offs' WHERE InternalName='ODWriteoffs' AND Description='Writeoffs'";
			Db.NonQ(command);
			command="UPDATE displayreport SET Description='PPO Write-offs' WHERE InternalName='ODPPOWriteoffs' AND Description='PPO Writeoffs'";
			Db.NonQ(command);
			command="UPDATE definition SET ItemName='Write-off' WHERE ItemName='Write Off'";
			Db.NonQ(command);
			command="UPDATE definition SET ItemName='Insurance Write-off' WHERE ItemName='Insurance Writeoff'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('InsPlanMergeInProgress','')";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('OrthoShowInChart','1')";//on for everyone to preserve existing behavior
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('ProcNoteSigsBlockedAutoNoteIncomplete','0')"; //Default to false
			Db.NonQ(command);
			command="UPDATE orthochart,orthochartrow SET orthochart.PatNum=orthochartrow.PatNum WHERE orthochart.OrthoChartRowNum=orthochartrow.OrthoChartRowNum";
			//this makes it easier to run a query to get all orthocharts(cells) for a patient.
			Db.NonQ(command);
			//34954 include Other gender.
			command="Update sheetfielddef SET FieldValue=';Male|Female|Other|Unknown' WHERE FieldName='Gender' AND FieldType=13";//13 is ComboBox
			Db.NonQ(command);
			command="ALTER TABLE patientnote ADD Pronoun tinyint NOT NULL";
			Db.NonQ(command);
			command="INSERT INTO preference(Prefname,ValueString) VALUES('ShowPreferredPronounsForPats','1')";//Default to true
			Db.NonQ(command);
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading}Adding SheetFieldDefNum Column");//No translation in convert script.
			LargeTableHelper.AlterTable("sheetfield","SheetFieldNum",new ColNameAndDef("SheetFieldDefNum","bigint NOT NULL"),new IndexColsAndName("SheetFieldDefNum",""));
			ODEvent.Fire(ODEventType.ConvertDatabases,$"{upgrading}");//No translation in convert script.
			command="ALTER TABLE procmultivisit ADD PatNum bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE procmultivisit ADD INDEX (PatNum)";
			Db.NonQ(command);
			command="UPDATE procmultivisit,procedurelog SET procmultivisit.PatNum=procedurelog.PatNum WHERE procmultivisit.ProcNum=procedurelog.ProcNum";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthohardware";
			Db.NonQ(command);
			command=@"CREATE TABLE orthohardware (
				OrthoHardwareNum bigint NOT NULL auto_increment PRIMARY KEY,
				PatNum bigint NOT NULL,
				DateExam date NOT NULL DEFAULT '0001-01-01',
				OrthoHardwareType tinyint NOT NULL,
				OrthoHardwareSpecNum bigint NOT NULL,
				ToothRange varchar(255) NOT NULL,
				Note varchar(255) NOT NULL,
				INDEX(PatNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="DROP TABLE IF EXISTS orthohardwarespec";
			Db.NonQ(command);
			command=@"CREATE TABLE orthohardwarespec (
				OrthoHardwareSpecNum bigint NOT NULL auto_increment PRIMARY KEY,
				OrthoHardwareType tinyint NOT NULL,
				Description varchar(255) NOT NULL,
				ItemColor int NOT NULL,
				IsHidden tinyint NOT NULL,
				ItemOrder int NOT NULL
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			command="INSERT INTO orthohardwarespec(OrthoHardwareType,Description,ItemColor) VALUES(0,'Bracket',-6250336)";
			Db.NonQ(command);
			command="INSERT INTO orthohardwarespec(OrthoHardwareType,Description,ItemColor) VALUES(1,'Wire',-7829368)";
			Db.NonQ(command);
			command="INSERT INTO orthohardwarespec(OrthoHardwareType,Description,ItemColor) VALUES(2,'Elastic',-1517178)";
			Db.NonQ(command);
			Misc.SecurityHash.UpdateHashing();
			command=$@"UPDATE preference SET ValueString={POut.Bool(true)} WHERE PrefName='AgingIsEnterprise'";
			Db.NonQ(command);
			command=$@"UPDATE preference SET ValueString={POut.Bool(false)} WHERE PrefName='AgingCalculatedMonthlyInsteadOfDaily'";
			Db.NonQ(command);
			command="SELECT ValueString from preference WHERE PrefName='AgingServiceTimeDue'";
			DateTime dateAgingServiceTimeDue = PIn.DateT(Db.GetScalar(command));
			if(dateAgingServiceTimeDue.Year<1880) {
				//Set the preference value to 2am.
				command=$@"UPDATE preference SET ValueString={POut.DateT(DateTime.Today.AddHours(2))} WHERE PrefName='AgingServiceTimeDue'";
				Db.NonQ(command);
			}
			AlterTable("commlog","CommlogNum",new ColNameAndDef("DateTEntry","datetime NOT NULL DEFAULT '0001-01-01 00:00:00'"));
			command="INSERT INTO preference(Prefname,ValueString) VALUES('ChartOrthoTabAutomaticCheckboxes','1')";//Default to true
			Db.NonQ(command);
			command="SELECT DISTINCT UserGroupNum FROM grouppermission WHERE PermType=8";
			table=Db.GetTable(command);
			long groupNum;
			for(int i = 0;i<table.Rows.Count;i++) {
				groupNum=PIn.Long(table.Rows[i]["UserGroupNum"].ToString());
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",51)";  //51 - Provider Edit, Allows user to edit all providers.
				Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",229)";  //229 - Setup Wizard, Allows user to use setup wizard.
				Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",230)";  //230 - Show Features, Allows user to use show features.
				Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",231)";  //231 - Setup Printer, Allows user to setup printer.
				Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",232)";  //232 - Provider Add, Allows user to add provider.
				Db.NonQ(command);
				command="INSERT INTO grouppermission (UserGroupNum,PermType) "
					 +"VALUES("+POut.Long(groupNum)+",233)";  //233 - Clinic Edit, Allows user to edit clinic.
				Db.NonQ(command);
			}
			command="DROP TABLE IF EXISTS orthorx";
			Db.NonQ(command);
			command=@"CREATE TABLE orthorx (
				OrthoRxNum bigint NOT NULL auto_increment PRIMARY KEY,
				OrthoHardwareSpecNum bigint NOT NULL,
				Description varchar(255) NOT NULL,
				ToothRange varchar(255) NOT NULL,
				ItemOrder int NOT NULL,
				INDEX(OrthoHardwareSpecNum)
				) DEFAULT CHARSET=utf8";
			Db.NonQ(command);
			//I34918 - Auto Disable Popups
			command="ALTER TABLE popup ADD DateTimeDisabled datetime NOT NULL DEFAULT '0001-01-01 00:00:00'";
			Db.NonQ(command);
			command="INSERT INTO preference(PrefName,ValueString) VALUES('PopupsDisableTimeSpan','')";
			Db.NonQ(command);
			command="UPDATE popup SET DateTimeDisabled=NOW() WHERE IsDisabled=1";
			Db.NonQ(command);
			command="ALTER TABLE task ADD TriageCategory bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE task ADD INDEX (TriageCategory)";
			Db.NonQ(command);
			command="ALTER TABLE taskhist ADD TriageCategory bigint NOT NULL";
			Db.NonQ(command);
			command="ALTER TABLE taskhist ADD INDEX (TriageCategory)";
			Db.NonQ(command);
		}//End of 22_2_1() method

		private static void To22_2_4() {
			string command;
			command="INSERT INTO preference(PrefName,ValueString) VALUES ('InsPayNoInitialPrimaryMoreThanProc','0')";//Default to false.
			Db.NonQ(command);
		}//End of 22_2_4() method

		private static void To22_2_8() {
			string command;
			//Insert PORTRAY bridge----------------------------------------------------------------- 
			command="INSERT INTO program (ProgName,ProgDesc,Enabled,Path,CommandLine,Note"
				 +") VALUES("
				 +"'PORTRAY', "
				 +"'PORTRAY from Surround Medical Systems', "
				 +"'0', "
				 +"'"+POut.String(@"C:\Program Files\Surround Medical\PORTRAY\Application\PORTRAY.exe")+"', "
				 +"'"+POut.String(@"<[PatNum]> <[FName]> <[PatientMiddleInitial]> <[LName]> <[NamePreferredOrFirst]> <[Birthdate]> <[PatientGenderMF]>")+"', "//leave blank if none 
				 +"'')";
			long programNum=Db.NonQ(command,true);
			command="INSERT INTO programproperty (ProgramNum,PropertyDesc,PropertyValue"
				 +") VALUES("
				 +"'"+POut.Long(programNum)+"', "
				 +"'Enter 0 to use PatientNum, or 1 to use ChartNum', "
				 +"'0')";
			Db.NonQ(command);
			command="INSERT INTO toolbutitem (ProgramNum,ToolBar,ButtonText) "
				 +"VALUES ("
				 +"'"+POut.Long(programNum)+"', "
				 +"'7', "//ToolBarsAvail.MainToolbar
				 +"'PORTRAY')";
			Db.NonQ(command);
			//end PORTRAY bridge
		}//End of 22_2_8() method

		private static void To22_2_9() {
			string command;
			command="UPDATE preference SET ValueString='https://www.patientviewer.com' WHERE PrefName='PatientPortalURL' AND ValueString LIKE 'System.Linq%'";
			Db.NonQ(command);
		}//End of 22_2_9() method

		private static void To22_2_12() {
			string command;
			DataTable table;
			command="SELECT DocNum FROM document WHERE MountItemNum NOT IN(SELECT MountItemNum FROM mountitem) AND MountItemNum!=0";
			table=Db.GetTable(command);
			for(int i=0;i<table.Rows.Count;i++) {
				long docNum=PIn.Long(table.Rows[i]["DocNum"].ToString());
				command="UPDATE document SET MountItemNum=0 WHERE DocNum="+POut.Long(docNum);
				Db.NonQ(command);
			}
		}//End of 22_2_12() method

		private static void To22_2_15() {
			string command = "ALTER TABLE insplan ADD INDEX(AllowedFeeSched)";
			Db.NonQ(command);
		}//End of 22_2_15() method

		private static void To22_2_18() {
			RemoveDuplicateProcCodesFromFrequencyLimitationPreferences();
		}//End of 22_2_18() method
	}
}

		