using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.IO;
using System.Text.RegularExpressions;
using CodeBase;

namespace OpenDental {
	public partial class FormRpArizonaPrimaryCareEligibility:FormODBase {

		public FormRpArizonaPrimaryCareEligibility() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRpArizonaPrimaryCareEligibility_Load(object sender,EventArgs e) {
			dateTimeTo.Value=DateTime.Today;
			dateTimeFrom.Value=DateTime.Today.AddMonths(-1);
		}

		private void butFinished_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			if(folderEligibilityPath.ShowDialog()==DialogResult.OK){
				textEligibilityFolder.Text=folderEligibilityPath.SelectedPath;
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			Clipboard.SetText(this.textLog.Text);
		}

		private void butRun_Click(object sender,EventArgs e) {
			this.textLog.Text="";
			string outFile=ODFileUtils.CombinePaths(textEligibilityFolder.Text,textEligibilityFile.Text);
			if(File.Exists(outFile)) {
				if(MessageBox.Show("The file at "+outFile+" already exists. Overwrite?","Overwrite File?",
					MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
			}
			string outputText="";
			string patientsIdNumberStr="SPID#";
			string householdGrossIncomeStr="Household Gross Income";
			string householdPercentOfPovertyStr="Household % of Poverty";
			string statusStr="Eligibility Status";
			string command="";
			//Locate the payment definition number for copayments of patients using the Arizona Primary Care program.
			command="SELECT DefNum FROM definition WHERE Category="+POut.Long((int)DefCat.PaymentTypes)+" AND IsHidden=0 AND LOWER(TRIM(ItemName))='noah'";
			DataTable copayDefNumTab=Reports.GetTable(command);
			if(copayDefNumTab.Rows.Count!=1){
				MessageBox.Show("You must define exactly one payment type with the name 'NOAH' before running this report. "+
					"This payment type must be used on payments made by Arizona Primary Care patients.");
				return;
			}
			long copayDefNum=PIn.Long(copayDefNumTab.Rows[0][0].ToString());
			//Get the list of all Arizona Primary Care patients, based on the patients which have an insurance carrier named 'noah'
			command="SELECT DISTINCT p.PatNum FROM patplan pp,inssub,insplan i,patient p,carrier c "+
				"WHERE p.PatNum=pp.PatNum AND inssub.InsSubNum=pp.InsSubNum AND inssub.PlanNum=i.PlanNum AND i.CarrierNum=c.CarrierNum "+
				"AND LOWER(TRIM(c.CarrierName))='noah' AND "+
				"(SELECT MAX(a.AptDateTime) FROM appointment a WHERE a.PatNum=p.PatNum AND a.AptStatus="+((int)ApptStatus.Complete)+") BETWEEN "+
					POut.Date(dateTimeFrom.Value)+" AND "+POut.Date(dateTimeTo.Value);
			DataTable primaryCarePatients=Reports.GetTable(command);
			for(int i=0;i<primaryCarePatients.Rows.Count;i++) {
				string patNum=POut.Long(PIn.Long(primaryCarePatients.Rows[i][0].ToString()));
				command="SELECT "+
					"TRIM((SELECT f.FieldValue FROM patfield f WHERE f.PatNum=p.PatNum AND "+
						"LOWER(f.FieldName)=LOWER('"+patientsIdNumberStr+"') "+DbHelper.LimitAnd(1)+")) PCIN, "+//Patient's Care ID Number
					"TRIM(("+DbHelper.LimitOrderBy("SELECT cl.Description FROM appointment ap,clinic cl WHERE ap.PatNum="+patNum+" AND "+
						"ap.AptStatus="+((int)ApptStatus.Complete)+" AND ap.ClinicNum=cl.ClinicNum ORDER BY ap.AptDateTime DESC",1)+")) SiteIDNumber,"+
					"p.BirthDate,"+
					"CASE p.Position WHEN "+((int)PatientPosition.Single)+" THEN 1 "+
						"WHEN "+((int)PatientPosition.Married)+" THEN 2 ELSE 3 END MaritalStatus,"+//Marital status
					//"CASE p.Race WHEN "+((int)PatientRaceOld.Asian)+" THEN 'A' WHEN "+((int)PatientRaceOld.HispanicLatino)+" THEN 'H' "+
					//  "WHEN "+((int)PatientRaceOld.HawaiiOrPacIsland)+" THEN 'P' WHEN "+((int)PatientRaceOld.AfricanAmerican)+" THEN 'B' "+
					//  "WHEN "+((int)PatientRaceOld.AmericanIndian)+" THEN 'I' WHEN "+((int)PatientRaceOld.White)+" THEN 'W' ELSE 'O' END PatRace,"+
					"CONCAT(CONCAT(TRIM(p.Address),' '),TRIM(p.Address2)) HouseholdAddress,"+//Patient address
					"p.City HouseholdCity,"+//Household residence city
					"p.State HouseholdState,"+//Household residence state
					"p.Zip HouseholdZip,"+//Household residence zip code
					"TRIM((SELECT f.FieldValue FROM patfield f WHERE f.PatNum=p.PatNum AND "+
						"LOWER(f.FieldName)=LOWER('"+householdGrossIncomeStr+"') "+DbHelper.LimitAnd(1)+")) HGI, "+//Household gross income
					"TRIM((SELECT f.FieldValue FROM patfield f WHERE f.PatNum=p.PatNum AND "+
						"LOWER(f.FieldName)=LOWER('"+householdPercentOfPovertyStr+"') "+DbHelper.LimitAnd(1)+")) HPP, "+//Household % of poverty
					"("+DbHelper.LimitOrderBy("SELECT a.AdjAmt FROM adjustment a WHERE a.PatNum="+patNum+" AND a.AdjType="+
						copayDefNum+" ORDER BY AdjDate DESC",1)+") HSFS,"+//Household sliding fee scale
					"(SELECT i.DateEffective FROM insplan i,inssub,patplan pp WHERE pp.PatNum="+patNum+" AND inssub.InsSubNum=pp.InsSubNum AND inssub.PlanNum=i.PlanNum "+DbHelper.LimitAnd(1)+") DES,"+//Date of eligibility status
					"TRIM((SELECT f.FieldValue FROM patfield f WHERE f.PatNum=p.PatNum AND "+
						"LOWER(f.FieldName)=LOWER('"+statusStr+"') "+DbHelper.LimitAnd(1)+")) CareStatus "+//Status
					"FROM patient p WHERE "+
					"p.PatNum="+patNum;
				DataTable primaryCareReportRow=Reports.GetTable(command);
				if(primaryCareReportRow.Rows.Count!=1) {
					//Either the results are ambiguous or for some reason, the patient number listed in the patfield table
					//does not actually exist. In either of these cases, it makes the most sense to just skip this patient
					//and continue with the rest of the reporting.
					continue;
				}
				string outputRow="";
				string rowErrors="";
				string rowWarnings="";
				string pcin=PIn.String(primaryCareReportRow.Rows[0]["PCIN"].ToString());
				if(pcin.Length<9) {
					rowErrors+="ERROR: Incorrectly formatted patient data for patient with patnum "+patNum+
						". Patient ID Number '"+pcin+"' is not at least 9 characters long."+Environment.NewLine;
				}
				outputRow+=pcin.PadLeft(15,'0');//Patient's ID Number
				string siteId=primaryCareReportRow.Rows[0]["SiteIDNumber"].ToString();
				if(siteId=="null"){
					siteId="";
				}
				if(!Regex.IsMatch(siteId,"^.*_[0-9]{5}$")){
					rowErrors+="ERROR: The clinic description for the clinic associated with the last completed appointment "+
						"for the patient with a patnum of "+patNum+" must be the clinic name, follwed by a '_', followed by the 5-digit Site ID Number "+
						"for the clinic. i.e. ClinicName_12345. The current clinic description is '"+siteId+"'."+Environment.NewLine;
				}else{
					siteId=siteId.Substring(siteId.Length-5);
				}
				outputRow+=siteId;
				outputRow+=PIn.Date(primaryCareReportRow.Rows[0]["Birthdate"].ToString()).ToString("MMddyyyy");//Patient's Date of Birth
				outputRow+=POut.Long(PIn.Long(primaryCareReportRow.Rows[0]["MaritalStatus"].ToString()));
				//outputRow+=primaryCareReportRow.Rows[0]["PatRace"].ToString();
				//Gets the old patient race enum based on the PatientRace entries in the db and displays the corresponding letters.
				switch (PatientRaces.GetPatientRaceOldFromPatientRaces(PIn.Long(patNum))) {
					case PatientRaceOld.Asian:
						outputRow+='A';
						break;
					case PatientRaceOld.HispanicLatino:
						outputRow+='H';
						break;
					case PatientRaceOld.HawaiiOrPacIsland:
						outputRow+='P';
						break;
					case PatientRaceOld.AfricanAmerican:
						outputRow+='B';
						break;
					case PatientRaceOld.AmericanIndian:
						outputRow+='I';
						break;
					case PatientRaceOld.White:
						outputRow+='W';
						break;
					default:
						outputRow+='O';
						break;
				}
				//Household residence address
				string householdAddress=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HouseholdAddress"].ToString()));
				if(householdAddress.Length>29) {
					string newHouseholdAddress=householdAddress.Substring(0,29);
					rowWarnings+="WARNING: Address for patient with patnum of "+patNum+" was longer than 29 characters and "+
						"was truncated in the report ouput. Address was changed from '"+
						householdAddress+"' to '"+newHouseholdAddress+"'"+Environment.NewLine;
					householdAddress=newHouseholdAddress;
				}
				outputRow+=householdAddress.PadRight(29,' ');
				//Household residence city
				string householdCity=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HouseholdCity"].ToString()));
				if(householdCity.Length>15) {
					string newHouseholdCity=householdCity.Substring(0,15);
					rowWarnings+="WARNING: City name for patient with patnum of "+patNum+" was longer than 15 characters and "+
						"was truncated in the report ouput. City name was changed from '"+
						householdCity+"' to '"+newHouseholdCity+"'"+Environment.NewLine;
					householdCity=newHouseholdCity;
				}
				outputRow+=householdCity.PadRight(15,' ');
				//Household residence state
				string householdState=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HouseholdState"].ToString()));
				if(householdState.Length>2) {
					string newHouseholdState=householdState.Substring(0,2);
					rowWarnings+="WARNING: State abbreviation for patient with patnum of "+patNum+" was longer than 2 characters and "+
						"was truncated in the report ouput. State abbreviation was changed from '"+
						householdState+"' to '"+newHouseholdState+"'"+Environment.NewLine;
					householdState=newHouseholdState;
				}
				outputRow+=householdState.PadRight(2,' ');
				//Household residence zip code
				string householdZip=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HouseholdZip"].ToString()));
				if(householdZip.Length>5) {
					string newHouseholdZip=householdZip.Substring(0,5);
					rowWarnings+="WARNING: The zipcode for patient with patnum of "+patNum+" was longer than 5 characters and "+
						"was truncated in the report ouput. The zipcode was changed from '"+
						householdZip+"' to '"+newHouseholdZip+"'"+Environment.NewLine;
					householdZip=newHouseholdZip;
				}
				outputRow+=householdZip.PadRight(5,' ');
				//Household gross income
				string householdGrossIncome=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HGI"].ToString()));
				if(householdGrossIncome==""||householdGrossIncome=="null") {
					householdGrossIncome="0";
				}
				//Remove any character that is not a digit or a decimal.
				string newHouseholdGrossIncome=Math.Round(Convert.ToDouble(Regex.Replace(householdGrossIncome,"[^0-9\\.]","")),0).ToString();
				if(householdGrossIncome!=newHouseholdGrossIncome) {
					rowWarnings+="WARNING: The household gross income for patient with patnum "+patNum+" contained invalid characters "+
						"and was changed in the output report from '"+householdGrossIncome+"' to '"+newHouseholdGrossIncome+"'."+Environment.NewLine;
				}
				householdGrossIncome=newHouseholdGrossIncome.PadLeft(7,'0');
				if(householdGrossIncome.Length>7) {
					rowErrors+="ERROR: Abnormally large household gross income of '"+householdGrossIncome+
						"' for patient with patnum of "+patNum+"."+Environment.NewLine;
				}
				outputRow+=householdGrossIncome;
				//Household percent of poverty
				string householdPercentPoverty=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HPP"].ToString()));
				if(householdPercentPoverty==""||householdPercentPoverty=="null") {
					householdPercentPoverty="0";
				}
				string newHouseholdPercentPoverty=Regex.Replace(householdPercentPoverty,"[^0-9]","");//Remove anything that is not a digit.
				if(newHouseholdPercentPoverty!=householdPercentPoverty) {
					rowWarnings+="WARNING: Household percent poverty for the patient with a patnum of "+patNum+
						" had to be modified in the output report from '"+householdPercentPoverty+"' to '"+newHouseholdPercentPoverty+
						"' based on output requirements."+Environment.NewLine;
				}
				householdPercentPoverty=newHouseholdPercentPoverty.PadLeft(3,'0');
				if(householdPercentPoverty.Length>3||Convert.ToInt16(householdPercentPoverty)>200) {
					rowErrors+="ERROR: Household percent poverty must be between 0 and 200 percent, but is set to '"+
						householdPercentPoverty+"' for the patient with the patnum of "+patNum+Environment.NewLine;
				}
				outputRow+=householdPercentPoverty;
				string householdSlidingFeeScale=POut.String(PIn.String(primaryCareReportRow.Rows[0]["HSFS"].ToString()));
				if(householdSlidingFeeScale.Length==0){
					householdSlidingFeeScale="0";
				}
				string newHouseholdSlidingFeeScale=Regex.Replace(householdSlidingFeeScale,"[^0-9]","");
				if(newHouseholdSlidingFeeScale!=householdSlidingFeeScale){
					rowWarnings+="WARNING: The household sliding fee scale (latest NOAH copay amount) for the patient with a patnum of "+patNum+
						" contains invalid characters and was changed from '"+householdSlidingFeeScale+"' to '"+newHouseholdSlidingFeeScale+"'."+Environment.NewLine;
					householdSlidingFeeScale=newHouseholdSlidingFeeScale;
				}
				if(householdSlidingFeeScale.Length>3 || Convert.ToInt16(householdSlidingFeeScale)>100){
					rowWarnings+="WARNING: The household sliding fee scale (latest NOAH copay amount) for the patient with a patnum of "+patNum+
						" is '"+householdSlidingFeeScale+"', but will be reported as 100."+Environment.NewLine;
					householdSlidingFeeScale="100";
				}
				outputRow+=householdSlidingFeeScale.PadLeft(3,'0');
				string dateOfEligibilityStatusStr=primaryCareReportRow.Rows[0]["DES"].ToString();
				DateTime dateOfEligibilityStatus=DateTime.MinValue;
				if(dateOfEligibilityStatusStr!="" && dateOfEligibilityStatusStr!="null"){
					dateOfEligibilityStatus=PIn.Date(dateOfEligibilityStatusStr);
				}
				outputRow+=dateOfEligibilityStatus.ToString("MMddyyyy");
				//Primary care status
				string primaryCareStatus=POut.String(PIn.String(primaryCareReportRow.Rows[0]["CareStatus"].ToString())).ToUpper();
				if(primaryCareStatus!="A"&&primaryCareStatus!="B"&&primaryCareStatus!="C"&&primaryCareStatus!="D") {
					rowErrors+="ERROR: The primary care status of the patient with a patnum of "+patNum+" is set to '"+primaryCareStatus+
						"', but must be set to A, B, C or D. "+Environment.NewLine;
				}
				outputRow+=primaryCareStatus;
				textLog.Text+=rowErrors+rowWarnings;
				if(rowErrors.Length>0) {
					continue;
				}
				outputText+=outputRow+Environment.NewLine;//Only add the row to the output file if it is properly formatted.
			}
			File.WriteAllText(outFile,outputText);
			MessageBox.Show("Done.");
		}

	}
}