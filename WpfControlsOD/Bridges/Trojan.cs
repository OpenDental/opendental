using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Win32;
using OpenDentBusiness;
using System.Drawing;
using CodeBase;
using System.Globalization;
using System.Linq;

namespace OpenDental.Bridges {
	public class Trojan {
		private static Collection<string[]> collectionStringArraysPatient;
		private static Collection<string[]> collectionStringArraysTrojan;
		private static DataTable dataTablePatient;
		private static DataTable dataTableTrojan;

		public static void StartupCheck(){
			//Skip all if not using Trojan.
			Program ProgramCur=Programs.GetCur(ProgramName.Trojan);
			if(!Programs.IsEnabledByHq(ProgramCur,out _) || !ProgramCur.Enabled || ODEnvironment.IsCloudServer) {
				return;
			}
			//Ensure that Trojan has a sane install.
			RegistryKey regKey=Registry.LocalMachine.OpenSubKey("Software\\TROJAN BENEFIT SERVICE");
			string file="";
			if(ODBuild.IsDebug()) {
				file=@"C:\Trojan\ETW\";
				ProcessDeletedPlans(file+@"DELETEDPLANS.TXT");
				ProcessTrojanPlanUpdates(file+@"ALLPLANS.TXT");
			}
			else {
				if(regKey==null || regKey.GetValue("INSTALLDIR")==null) {
					//jsparks: The below is wrong.  The user should create a registry key manually.
					return;
					//The old trojan registry key is missing. Try to locate the new Trojan registry key.
					//regKey=Registry.LocalMachine.OpenSubKey("Software\\Trojan Eligibility");
					//if(regKey==null||regKey.GetValue("INSTALLDIR")==null) {//Unix OS will exit here.
					//	return;
					//}
				}
				//Process DELETEDPLANS.TXT for recently deleted insurance plans.
				file=regKey.GetValue("INSTALLDIR").ToString()+@"\DELETEDPLANS.TXT";//C:\ETW\DELETEDPLANS.TXT

				ProcessDeletedPlans(file);
				//Process ALLPLANS.TXT for new insurance plan information.
				file=regKey.GetValue("INSTALLDIR").ToString()+@"\ALLPLANS.TXT";//C:\ETW\ALLPLANS.TXT
				ProcessTrojanPlanUpdates(file);
			}
		}

		///<summary>Process the deletion of existing insurance plans.</summary>
		private static void ProcessDeletedPlans(string file){
			if(!File.Exists(file)) {
				//Nothing to process.
				return;
			}
			string deleteplantext=File.ReadAllText(file);
			if(deleteplantext=="") {
				//Nothing to process. Don't delete the file in-case Trojan is filling the file right now.
				return;
			}
			collectionStringArraysPatient=new Collection<string[]>();
			collectionStringArraysTrojan=new Collection<string[]>();
			string[] trojanplans=deleteplantext.Split(new string[] { "\n" },StringSplitOptions.RemoveEmptyEntries);
			Collection <string[]> records=new Collection<string[]>();
			for(int i=0;i<trojanplans.Length;i++) {
				string[] record=trojanplans[i].Split(new string[] {"\t"},StringSplitOptions.None);
				for(int j=0;j<record.Length;j++){
					//Remove any white space around the field and remove the surrounding quotes.
					record[j]=record[j].Trim().Substring(1);
					record[j]=record[j].Substring(0,record[j].Length-1);
				}
				records.Add(record);
				string whoToContact=record[3].ToUpper();
				if(whoToContact=="T"){
					collectionStringArraysTrojan.Add(record);
				}
				else{//whoToContact="P"
					collectionStringArraysPatient.Add(record);
				}
			}
			if(collectionStringArraysPatient.Count>0){
				dataTablePatient=TrojanQueries.GetPendingDeletionTable(collectionStringArraysPatient);
				if(dataTablePatient.Rows.Count>0){
					FormLauncher formLauncher=new FormLauncher(EnumFormName.FormPrintTrojan);
					formLauncher.Show();
					formLauncher.MethodGetVoid("PrintDelForPatients",dataTablePatient,collectionStringArraysPatient);
				}
			}
			if(collectionStringArraysTrojan.Count>0) {
				dataTableTrojan=TrojanQueries.GetPendingDeletionTableTrojan(collectionStringArraysTrojan);
				if(dataTableTrojan.Rows.Count>0) {
					FormLauncher formLauncher=new FormLauncher(EnumFormName.FormPrintTrojan);
					formLauncher.Show();
					formLauncher.MethodGetVoid("PrintDelForTrojan",dataTableTrojan,collectionStringArraysTrojan);
				}
			}
			//Now that the plans have been reported, drop the plans that are marked finally deleted.
			for(int i=0;i<records.Count;i++){
				if(records[i][1]=="F") {
					try {
						InsPlan[] insplans=InsPlans.GetByTrojanID(records[i][0]);
						for(int j=0;j<insplans.Length;j++) {
							InsPlan planOld = insplans[j].Copy();
							insplans[j].PlanNote="PLAN DROPPED BY TROJAN"+Environment.NewLine+insplans[j].PlanNote;
							insplans[j].TrojanID="";
							InsPlans.Update(insplans[j],planOld);
							PatPlan[] patplans=PatPlans.GetByPlanNum(insplans[j].PlanNum);
							for(int k=0;k<patplans.Length;k++) {
								PatPlans.Delete(patplans[k].PatPlanNum);
							}
						}
					} 
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						return;
					}
				}
			}
			try {
				File.Delete(file);
			}
			catch(Exception ex) {
				MessageBox.Show(file+" could not be deleted.  Please delete manually.");
			}
		}

		///<summary>Process existing insurance plan updates from the ALLPLANS.TXT file.</summary>
		private static void ProcessTrojanPlanUpdates(string file){
			if(!File.Exists(file)) {
				//Nothing to process.
				return;
			}
			MessageBox.Show("Trojan update found.  Please print or save the text file when it opens, then close it.  You will be given a chance to cancel the update after that.");
			ODFileUtils.ProcessStart(file);
			if(!MsgBox.Show("Trojan",MsgBoxButtons.OKCancel,"Trojan plans will now be updated.")) {
				return;
			}
			Cursor.Current=Cursors.WaitCursor;
			string allplantext="";
			using(StreamReader sr=new StreamReader(file)) {
				allplantext=sr.ReadToEnd();
			}
			if(allplantext=="") {
				Cursor.Current=Cursors.Default;
				MessageBox.Show("Could not read file contents: "+file);
				return;
			}
			bool updateBenefits=MsgBox.Show("Trojan",MsgBoxButtons.YesNo,"Also update benefits?  Any customized benefits will be overwritten.");
			bool updateNote=MsgBox.Show("Trojan",MsgBoxButtons.YesNo,"Automatically update plan notes?  Any existing notes will be overwritten.  If you click No, you will be presented with each change for each plan so that you can edit the notes as needed.");
			string[] trojanplans=allplantext.Split(new string[] { "TROJANID" },StringSplitOptions.RemoveEmptyEntries);
			int plansAffected=0;
			try {
				for(int i=0;i<trojanplans.Length;i++) {
					trojanplans[i]="TROJANID"+trojanplans[i];
					plansAffected+=ProcessTrojanPlan(trojanplans[i],updateBenefits,updateNote);
				}
			}
			catch(Exception e) {//this will happen if user clicks cancel in a note box.
				MessageBox.Show("Error: "+e.Message+"\r\n\r\nWe will need a copy of the ALLPLANS.txt.");
				return;
			}
			Cursor.Current=Cursors.Default;
			MessageBox.Show(plansAffected.ToString()+" plans updated.");
			try{
				File.Delete(file);
			}
			catch{
				MessageBox.Show(file+" could not be deleted.  Please delete manually.");
			}
		}

		///<summary>Returns number of subscribers affected.  Can throw an exception if user clicks cancel in a note box.</summary>
		private static int ProcessTrojanPlan(string trojanPlan,bool updateBenefits,bool updateNoteAutomatic){
			TrojanObject troj=ProcessTextToObject(trojanPlan);
			Carrier carrier=new Carrier();
			carrier.Phone=troj.ELIGPHONE;
			carrier.ElectID=troj.PAYERID;
			carrier.CarrierName=troj.MAILTO;
			carrier.Address=troj.MAILTOST;
			carrier.City=troj.MAILCITYONLY;
			carrier.State=troj.MAILSTATEONLY;
			carrier.Zip=troj.MAILZIPONLY;
			carrier.NoSendElect=NoSendElectType.SendElect;//regardless of what Trojan says.  Nobody sends paper anymore.
			if(carrier.CarrierName==null || carrier.CarrierName=="") {
				//if, for some reason, carrier is absent from the file, we can't do a thing with it.
				return 0;
			}
			carrier=Carriers.GetIdentical(carrier);
			//now, save this all to the database.
			troj.CarrierNum=carrier.CarrierNum;
			InsPlan plan=TrojanQueries.GetPlanWithTrojanID(troj.TROJANID);
			if(plan==null) {
				return 0;
			}
			TrojanQueries.UpdatePlan(troj,plan.PlanNum,updateBenefits);
			plan=InsPlans.RefreshOne(plan.PlanNum);
			InsPlan planOld = plan.Copy();
			if(updateNoteAutomatic) {
				if(plan.PlanNote!=troj.PlanNote) {
					plan.PlanNote=troj.PlanNote;
					InsPlans.Update(plan,planOld);
				}
			}
			else {
				//let user pick note
				if(plan.PlanNote!=troj.PlanNote) {
					string[] notes=new string[2];
					notes[0]=plan.PlanNote;
					notes[1]=troj.PlanNote;
					FormLauncher formLauncher=new FormLauncher(EnumFormName.FormNotePick);
					formLauncher.SetField("StringArrayNotes",notes);
					formLauncher.ShowDialog();
					if(formLauncher.IsDialogOK){
						string noteSelected=formLauncher.GetField<string>("NoteSelected");
						if(plan.PlanNote!=noteSelected) {
							plan.PlanNote=noteSelected;
							InsPlans.Update(plan,planOld);
						}
					}
				}
			}
			return 1;
		}

		///<summary>Converts the text for one plan into an object which will then be processed as needed.</summary>
		public static TrojanObject ProcessTextToObject(string text){
			string[] lines=text.Split(new string[] { "\r\n" },StringSplitOptions.RemoveEmptyEntries);
			string line;
			string[] fields;
			int percent;
			double amt;
			string rowVal;
			TrojanObject troj=new TrojanObject();
			troj.BenefitList=new List<Benefit>();
			troj.BenefitNotes="";
			bool usesAnnivers=false;
			Benefit ben;
			Benefit benCrownMajor=null;
			Benefit benCrownOnly=null;
      for(int i=0;i<lines.Length;i++){
				line=lines[i];
				fields=line.Split(new char[] {'\t'});
				if(fields.Length!=3){
					continue;
				}
				//remove any trailing or leading spaces:
				fields[0]=fields[0].Trim();
				fields[1]=fields[1].Trim();
				fields[2]=fields[2].Trim();
				rowVal=fields[2].Trim();
				if(fields[2]==""){
					continue;
				}
				else{//as long as there is data, add it to the notes
					if(troj.BenefitNotes!="") {
						troj.BenefitNotes+="\r\n";
					}
					troj.BenefitNotes+=fields[1]+": "+fields[2];
					if(fields.Length==4) {
						troj.BenefitNotes+=" "+fields[3];
					}
				}
				switch(fields[0]){
					//default://for all rows that are not handled below
					case "TROJANID":
						troj.TROJANID=fields[2];
						break;
					case "ENAME":
						troj.ENAME=fields[2];
						break;
					case "PLANDESC":
						troj.PLANDESC=fields[2];
						break;
					case "ELIGPHONE":
						troj.ELIGPHONE=fields[2];
						break;
					case "POLICYNO":
						troj.POLICYNO=fields[2];
						break;
					case "ECLAIMS":
						if(fields[2]=="YES") {//accepts eclaims
							troj.ECLAIMS=true;
						}
						else {
							troj.ECLAIMS=false;
						}
						break;
					case "PAYERID":
						troj.PAYERID=fields[2];
						break;
					case "MAILTO":
						troj.MAILTO=fields[2];
						break;
					case "MAILTOST":
						troj.MAILTOST=fields[2];
						break;
					case "MAILCITYONLY":
						troj.MAILCITYONLY=fields[2];
						break;
					case "MAILSTATEONLY":
						troj.MAILSTATEONLY=fields[2];
						break;
					case "MAILZIPONLY":
						troj.MAILZIPONLY=fields[2];
						break;
					case "PLANMAX"://eg $3000 per person per year
						if(!fields[2].StartsWith("$"))
							break;
						fields[2]=fields[2].Remove(0,1);
						fields[2]=fields[2].Split(new char[] { ' ' })[0];
						if(CovCats.GetCount(true) > 0) {
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.Limitations;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
							ben.MonetaryAmt=PIn.Double(fields[2]);
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							ben.CoverageLevel=BenefitCoverageLevel.Individual;
							troj.BenefitList.Add(ben.Copy());
						}
						break;
					case "PLANYR"://eg Calendar year or Anniversary year or month renewal
						string monthName=fields[2].Split(new char[] {' '})[0];
						usesAnnivers=true;
						if(fields[2]=="Calendar year" || monthName.In(DateTimeFormatInfo.CurrentInfo.MonthNames)) {
							usesAnnivers=false;
							troj.MonthRenewal=DateTimeFormatInfo.CurrentInfo.MonthNames.ToList().IndexOf(monthName)+1;
						}
						//MessageBox.Show("Warning.  Plan uses Anniversary year rather than Calendar year.  Please verify the Plan Start Date.");
						break;
					case "DEDUCT"://eg There is no deductible
						if(!fields[2].StartsWith("$")) {
							amt=0;
						}
						else {
							fields[2]=fields[2].Remove(0,1);
							fields[2]=fields[2].Split(new char[] { ' ' })[0];
							amt=PIn.Double(fields[2]);
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.General).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=0;//amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.Deductible;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						ben.MonetaryAmt=0;//amt;
						ben.CoverageLevel=BenefitCoverageLevel.Individual;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "PREV"://eg 100% or 'Incentive begins at 70%' or '80% Endo Major see notes'
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Diagnostic).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.RoutinePreventive).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "BASIC":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Restorative).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Endodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Periodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.OralSurgery).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					case "MAJOR":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Prosthodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						benCrownMajor=new Benefit();
						benCrownMajor.BenefitType=InsBenefitType.CoInsurance;
						benCrownMajor.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
						benCrownMajor.Percent=percent;
						benCrownMajor.TimePeriod=BenefitTimePeriod.CalendarYear;
						//troj.BenefitList.Add(ben.Copy());//later
						break;
					case "CROWNS"://Examples: Paid Major, or 80%.  We will only process percentages.
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						benCrownOnly=new Benefit();
						benCrownOnly.BenefitType=InsBenefitType.CoInsurance;
						benCrownOnly.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Crowns).CovCatNum;
						benCrownOnly.Percent=percent;
						benCrownOnly.TimePeriod=BenefitTimePeriod.CalendarYear;
						//troj.BenefitList.Add(ben.Copy());
						break;
					case "ORMAX"://eg $3500 lifetime
						if(!fields[2].StartsWith("$")) {
							break;
						}
						fields[2]=fields[2].Remove(0,1);
						fields[2]=fields[2].Split(new char[] { ' ' })[0];
						if(CovCats.GetCount(true) > 0) {
							ben=new Benefit();
							ben.BenefitType=InsBenefitType.Limitations;
							ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
							ben.MonetaryAmt=PIn.Double(fields[2]);
							ben.TimePeriod=BenefitTimePeriod.CalendarYear;
							troj.BenefitList.Add(ben.Copy());
						}
						break;
					case "ORPCT":
						if(rowVal.ToLower()=="not covered") {
							percent=0;
						}
						else {
							percent=ConvertPercentToInt(rowVal);//remove %
						}
						if(percent<0 || percent>100) {
							break;
						}
						ben=new Benefit();
						ben.BenefitType=InsBenefitType.CoInsurance;
						ben.CovCatNum=CovCats.GetForEbenCat(EbenefitCategory.Orthodontics).CovCatNum;
						ben.Percent=percent;
						ben.TimePeriod=BenefitTimePeriod.CalendarYear;
						troj.BenefitList.Add(ben.Copy());
						break;
					/*case "FEE":
						if(!ProcedureCodes.IsValidCode(fields[1])) {
							break;//skip
						}
						if(textTrojanID.Text==""){
							break;
						}
						feeSchedNum=Fees.ImportTrojan(fields[1],PIn.PDouble(fields[3]),textTrojanID.Text);
						//the step above probably created a new feeschedule, requiring a reset of the three listboxes.
						resetFeeSched=true;
						break;*/
					case "NOTES"://typically multiple instances
						if(troj.PlanNote!=null && troj.PlanNote!="") {
							troj.PlanNote+="\r\n";
						}
						troj.PlanNote+=fields[2];
						break;
				}//switch
			}//for
			//Set crowns
			if(benCrownOnly!=null){
				troj.BenefitList.Add(benCrownOnly.Copy());
			}
			else if(benCrownMajor!=null){
				troj.BenefitList.Add(benCrownMajor.Copy());
			}
			//set calendar vs serviceyear
			if(usesAnnivers) {
				for(int i=0;i<troj.BenefitList.Count;i++) {
					troj.BenefitList[i].TimePeriod=BenefitTimePeriod.ServiceYear;
				}
			}
			return troj;
		}

		///<summary>Takes a string percentage and returns the integer value.  Returns -1 if no match.</summary>
		private static int ConvertPercentToInt(string percent) {
			Match regMatch=Regex.Match(percent,@"([0-9]+)\%");
			if(regMatch.Success) {
				return PIn.Int(regMatch.Groups[1].Value);
			}
			return -1;
		}



	}

	
}
