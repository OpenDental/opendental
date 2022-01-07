using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using OpenDentBusiness;
using CodeBase;
using DataConnectionBase;
using System.Text.RegularExpressions;

namespace OpenDental {
	public partial class FormRpArizonaPrimaryCareEncounter:FormODBase {
		public FormRpArizonaPrimaryCareEncounter() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butBrowse_Click(object sender,EventArgs e) {
			if(folderEncounter.ShowDialog()==DialogResult.OK){
				this.textEncounterFolder.Text=this.folderEncounter.SelectedPath;
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			Clipboard.SetText(this.textLog.Text);
		}

		private void butRun_Click(object sender,EventArgs e) {
			//The encounter file is a list of all of the appointments provided for Arizona Primary Care patients within the specified
			//date range. Since each encounter/appointment is reimbursed by the local government at a flat rate, we only need to report
			//a single procedure for each appointment in the encounter file and if there is a question by the government as to the other
			//procedures that were performed during a particular appointment, then the dental office can simply look that information up
			//in Open Dental (but no such calls will likely happen). Thus we always use the same Diagnosis code corresponding to the single
			//ADA code that we emit in this flat file, just to keep things simple and workable.
			this.textLog.Text="";
			string outFile=ODFileUtils.CombinePaths(this.textEncounterFolder.Text,this.textEncounterFile.Text);
			if(File.Exists(outFile)) {
				if(MessageBox.Show("The file at "+outFile+" already exists. Overwrite?","Overwrite File?",
					MessageBoxButtons.YesNo)!=DialogResult.Yes) {
					return;
				}
			}
			string command="";
			//Locate the payment definition number for payments of patients using the Arizona Primary Care program.
			command="SELECT DefNum FROM definition WHERE Category="+POut.Long((int)DefCat.PaymentTypes)+" AND IsHidden=0 AND LOWER(TRIM(ItemName))='noah'";
			DataTable payDefNumTab=Reports.GetTable(command);
			if(payDefNumTab.Rows.Count!=1) {
				MessageBox.Show("You must define exactly one payment type with the name 'NOAH' before running this report. "+
					"This payment type must be used on payments made by Arizona Primary Care patients.");
				return;
			}
			long payDefNum=PIn.Long(payDefNumTab.Rows[0][0].ToString());
			string outputText="";
			string patientsIdNumberStr="SPID#";
			//Only certain procedures can be billed to the Arizona Primary Care program.
			//Since the code list doesn't change often, it is simply hard coded here.
			string billableProcedures="'D0120','D0140','D0150','D0160','D1110','D1120','D1201','D1203','D1204','D1205','D1208',"+
				"'D1351','D1510','D1515','D1520','D1525','D1550','D4341','D4355','D4910','D2140','D2150','D2160','D2161',"+
				"'D2330','D2331','D2332','D2335','D2390','D2391','D2392','D2393','D2394','D2910','D2920','D2930','D2931',"+
				"'D2932','D2940','D2950','D2970','D3110','D3120','D3220','D3221','D3230','D3240','D7140','D7210','D7220',"+
				"'D7270','D7285','D7286','D7510','D9110','D9310','D9610'";
			//Get the list of all Arizona Primary Care patients, based on the patients which have an insurance carrier named 'noah'
			command="SELECT DISTINCT p.PatNum FROM patplan pp,inssub,insplan i,patient p,carrier c "+
				"WHERE p.PatNum=pp.PatNum AND inssub.InsSubNum=pp.InsSubNum AND inssub.PlanNum=i.PlanNum AND i.CarrierNum=c.CarrierNum "+
				"AND LOWER(TRIM(c.CarrierName))='noah'";
			DataTable primaryCarePatients=Reports.GetTable(command);
			for(int i=0;i<primaryCarePatients.Rows.Count;i++) {
				string patNum=POut.Long(PIn.Long(primaryCarePatients.Rows[i][0].ToString()));
				//Now that we have an Arizona Primary Care patient's patNum, we need to see if there are any appointments
				//that the patient has attented (completed) in the date range specified where there is at least one ADA coded procedure
				//associated with that appointment. If there are, then those appointments will be placed into the flat file.
				command="SELECT a.AptNum FROM appointment a WHERE a.PatNum="+patNum+" AND a.AptStatus="+((int)ApptStatus.Complete)+" AND "+
					"a.AptDateTime BETWEEN "+POut.Date(dateTimeFrom.Value)+" AND "+POut.Date(dateTimeTo.Value)+" AND "+
					"(SELECT COUNT(*) FROM procedurelog pl,procedurecode pc WHERE pl.AptNum=a.AptNum AND pc.CodeNum=pl.CodeNum AND "+
					"pc.ProcCode IN ("+billableProcedures+") "+DbHelper.LimitAnd(1)+")>0";
				DataTable appointmentList=Reports.GetTable(command);
				for(int j=0;j<appointmentList.Rows.Count;j++){
					string aptNum=POut.Long(PIn.Long(appointmentList.Rows[j][0].ToString()));
					string datesql="CURDATE()";
					if(DataConnection.DBtype==DatabaseType.Oracle){
						datesql="(SELECT CURRENT_DATE FROM dual)";
					}
					command="SELECT "+
						"TRIM((SELECT f.FieldValue FROM patfield f WHERE f.PatNum=p.PatNum AND "+
							"LOWER(f.FieldName)=LOWER('"+patientsIdNumberStr+"') "+DbHelper.LimitAnd(1)+")) PCIN, "+//Patient's Care ID Number
						"p.BirthDate,"+//birthdate
						"(CASE p.Gender WHEN 0 THEN 'M' WHEN 1 THEN 'F' ELSE '' END) Gender,"+//Gender
						"CONCAT(CONCAT(p.Address,' '),p.Address2) Address,"+//address
						"p.City,"+//city
						"p.State,"+//state
						"p.Zip,"+//zipcode
						"(SELECT CASE pp.Relationship WHEN 0 THEN 1 ELSE 0 END FROM patplan pp,inssub,insplan i,carrier c WHERE "+//Relationship to subscriber
							"pp.PatNum="+patNum+" AND inssub.InsSubNum=pp.InsSubNum AND inssub.PlanNum=i.PlanNum AND i.CarrierNum=c.CarrierNum AND LOWER(TRIM(c.CarrierName))='noah' "+DbHelper.LimitAnd(1)+") InsRelat,"+
						"(CASE p.Position WHEN 0 THEN 1 WHEN 1 THEN 2 ELSE 3 END) MaritalStatus,"+//Marital status
						"(CASE WHEN p.EmployerNum=0 THEN (CASE WHEN ("+DbHelper.DateAddYear("p.BirthDate","18")+">"+datesql+") THEN 3 ELSE 2 END) ELSE 1 END) EmploymentStatus,"+
						"(CASE p.StudentStatus WHEN 'f' THEN 1 WHEN 'p' THEN 2 ELSE 3 END) StudentStatus,"+//student status
						"'ADHS PCP' InsurancePlanName,"+//insurance plan name
						"'' ReferringPhysicianName,"+//Name of referring physician
						"'' ReferringPhysicianID,"+//ID # of referring physician
						"'V722' DiagnosisCode1,"+//Diagnosis Code 1. Always set to V72.2 for simplicity and workability
						"'' DiagnosisCode2,"+//Diagnosis code 2
						"'' DiagnosisCode3,"+//Diagnosis code 3
						"'' DiagnosisCode4,"+//Diagnosis code 4
						"(SELECT a.AptDateTime FROM appointment a WHERE a.AptNum="+aptNum+" "+DbHelper.LimitAnd(1)+") DateOfEncounter,"+//Date of encounter
						"("+DbHelper.LimitOrderBy("SELECT pc.ProcCode FROM procedurecode pc,procedurelog pl "+
							"WHERE pl.AptNum="+aptNum+" AND pl.CodeNum=pc.CodeNum AND pc.ProcCode IN ("+billableProcedures+") ORDER BY pl.ProcNum",1)+") Procedure1,"+
						"'' Procedure1Modifier1,"+//Procedure modifier 1
						"'' Procedure1Modifier2,"+//Procedure modifier 2
						"'' Procedure1DiagnosisCode,"+//Diagnosis code
						"("+DbHelper.LimitOrderBy("SELECT pl.ProcFee FROM procedurecode pc,procedurelog pl "+
							"WHERE pl.AptNum="+aptNum+" AND pl.CodeNum=pc.CodeNum AND pc.ProcCode IN ("+billableProcedures+") ORDER BY pl.ProcNum",1)+") Procedure1Charges,"+
						"'' Procedure2,"+//2nd procedure cpt/hcpcs
						"'' Procedure2Modifier1,"+//2nd procedure modifier 1
						"'' Procedure2Modifier2,"+//2nd procedure modifier 2
						"'' Procedure2DiagnosisCode,"+//Diagnosis code
						"0 Procedure2Charges,"+//charges
						"'' Procedure3,"+//3rd procedure cpt/hcpcs
						"'' Procedure3Modifier1,"+//3rd procedure modifier 1
						"'' Procedure3Modifier2,"+//3rd procedure modifier 2
						"'' Procedure3DiagnosisCode,"+//Diagnosis code
						"0 Procedure3Charges,"+//Charges
						"'' Procedure4,"+//4th procedure cpt/hcpcs
						"'' Procedure4Modifier1,"+//4th procedure modifier 1
						"'' Procedure4Modifier2,"+//4th procedure modifier 2
						"'' Procedure4DiagnosisCode,"+//Diagnosis code
						"0 Procedure4Charges,"+//Charges
						"'' Procedure5,"+//5th procedure cpt/hcpcs
						"'' Procedure5Modifier1,"+//5th procedure modifier 1
						"'' Procedure5Modifier2,"+//5th procedure modifier 2
						"'' Procedure5DiagnosisCode,"+//diagnosis code
						"0 Procedure5Charges,"+//Charges
						"'' Procedure6,"+//6th procedure cpt/hcpcs
						"'' Procedure6Modifier1,"+//6th procedure modifier 1
						"'' Procedure6Modifier2,"+//6th procedure modifier 2
						"'' Procedure6DiagnosisCode,"+//Diagnosis code
						"0 Procedure6Charges,"+//Charges
						"(SELECT SUM(pl.ProcFee) FROM procedurelog pl WHERE pl.AptNum="+aptNum+") TotalCharges,"+//Total charges
						"(SELECT SUM(a.AdjAmt) FROM adjustment a WHERE a.PatNum="+patNum+" AND a.AdjType="+
							payDefNum+") AmountPaid,"+//Amount paid
						"0 BalanceDue,"+//Balance due
						"TRIM((SELECT cl.Description FROM appointment ap,clinic cl WHERE ap.AptNum="+aptNum+" AND "+
							"ap.ClinicNum=cl.ClinicNum "+DbHelper.LimitAnd(1)+")) ClinicDescription,"+
						"(SELECT pr.StateLicense FROM provider pr,appointment ap WHERE ap.AptNum="+aptNum+" AND pr.ProvNum=ap.ProvNum "+DbHelper.LimitAnd(1)+") PhysicianID,"+
						"(SELECT CONCAT(CONCAT(pr.FName,' '),pr.MI) FROM provider pr,appointment ap "+
							"WHERE ap.AptNum="+aptNum+" AND pr.ProvNum=ap.ProvNum "+DbHelper.LimitAnd(1)+") PhysicianFAndMNames,"+//Physician's first name and middle initial
						"(SELECT pr.LName FROM provider pr,appointment ap "+
							"WHERE ap.AptNum="+aptNum+" AND pr.ProvNum=ap.ProvNum "+DbHelper.LimitAnd(1)+") PhysicianLName "+//Physician's last name
						"FROM patient p WHERE "+
						"p.PatNum="+patNum;
					DataTable primaryCareReportRow=Reports.GetTable(command);
					string outputRow="";
					string rowErrors="";
					string rowWarnings="";
					//Patient's ID Number
					string pcin=primaryCareReportRow.Rows[0]["PCIN"].ToString();
					if(pcin.Length<9) {
						rowErrors+="ERROR: Incorrectly formatted patient data for patient with patnum "+patNum+
							". Patient ID Number '"+pcin+"' is not at least 9 characters long."+Environment.NewLine;
					}
					outputRow+=pcin.PadLeft(15,'0');
					//Patient's date of birth
					outputRow+=PIn.Date(primaryCareReportRow.Rows[0]["Birthdate"].ToString()).ToString("MMddyyyy");
					//Patient's gender
					outputRow+=PIn.String(primaryCareReportRow.Rows[0]["Gender"].ToString());
					//Patient's address
					string householdAddress=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Address"].ToString()));
					if(householdAddress.Length>29) {
						string newHouseholdAddress=householdAddress.Substring(0,29);
						rowWarnings+="WARNING: Address for patient with patnum of "+patNum+" was longer than 29 characters and "+
							"was truncated in the report ouput. Address was changed from '"+
							householdAddress+"' to '"+newHouseholdAddress+"'"+Environment.NewLine;
						householdAddress=newHouseholdAddress;
					}
					outputRow+=householdAddress.PadRight(29,' ');
					//Patient's city
					string householdCity=POut.String(PIn.String(primaryCareReportRow.Rows[0]["City"].ToString()));
					if(householdCity.Length>15) {
						string newHouseholdCity=householdCity.Substring(0,15);
						rowWarnings+="WARNING: City name for patient with patnum of "+patNum+" was longer than 15 characters and "+
							"was truncated in the report ouput. City name was changed from '"+
							householdCity+"' to '"+newHouseholdCity+"'"+Environment.NewLine;
						householdCity=newHouseholdCity;
					}
					outputRow+=householdCity.PadRight(15,' ');
					//Patient's State
					string householdState=POut.String(PIn.String(primaryCareReportRow.Rows[0]["State"].ToString()));
					if(householdState.ToUpper()!="AZ") {
						rowErrors+="ERROR: State abbreviation for patient with patnum of "+patNum+" must be set to AZ."+Environment.NewLine;
						householdState="AZ";
					}
					outputRow+=householdState;
					//Patient's zip code
					string householdZip=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Zip"].ToString()));
					if(householdZip.Length>5) {
						string newHouseholdZip=householdZip.Substring(0,5);
						rowWarnings+="WARNING: The zipcode for patient with patnum of "+patNum+" was longer than 5 characters in length and "+
							"was truncated in the report ouput. The zipcode was changed from '"+
							householdZip+"' to '"+newHouseholdZip+"'"+Environment.NewLine;
						householdZip=newHouseholdZip;
					}
					if(householdZip.Length<5){
						rowWarnings+="WARNING: The zipcode for patient with patnum of "+patNum+" was shorter than 5 characters in length "+
							"(current zipcode is '"+householdZip+"')"+Environment.NewLine;
						householdZip=householdZip.PadLeft(5,'0');
					}
					outputRow+=householdZip.PadRight(5,' ');
					//Patient's relationship to insured.
					string insuranceRelationship=POut.String(PIn.String(primaryCareReportRow.Rows[0]["InsRelat"].ToString()));
					if(insuranceRelationship!="1"){//Not self?
						rowWarnings+="WARNING: The patient insurance relationship is not 'self' for the patient with a patnum of "+patNum+Environment.NewLine;
					}
					outputRow+=insuranceRelationship;
					//Patient's marital status
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["MaritalStatus"].ToString()));
					//Patient's employment status
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["EmploymentStatus"].ToString()));
					//Patient's student status
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["StudentStatus"].ToString()));
					//Insurance plan name
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["InsurancePlanName"].ToString())).PadRight(25,' ');
					//Name of referring physician.
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["ReferringPhysicianName"].ToString())).PadRight(26,' ');
					//ID# of referring physician
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["ReferringPhysicianID"].ToString())).PadLeft(6,' ');
					//Diagnosis code 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["DiagnosisCode1"].ToString())).PadRight(6,'0');
					//Diagnosis code 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["DiagnosisCode2"].ToString())).PadRight(6,'0');
					//Diagnosis code 3
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["DiagnosisCode3"].ToString())).PadRight(6,'0');
					//Diagnosis code 4
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["DiagnosisCode4"].ToString())).PadRight(6,'0');
					//Date of encounter
					outputRow+=PIn.Date(primaryCareReportRow.Rows[0]["DateOfEncounter"].ToString()).ToString("MMddyyyy");
					//Procedure 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure1"].ToString())).PadRight(5,'0');
					//Procedure 1 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure1Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 1 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure1Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 1 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure1DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 1 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure1Charges"].ToString())).ToString().PadLeft(6,'0');
					//Procedure 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure2"].ToString())).PadRight(5,'0');
					//Procedure 2 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure2Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 2 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure2Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 2 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure2DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 2 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure2Charges"].ToString())).ToString().PadLeft(6,'0');
					//Procedure 3
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure3"].ToString())).PadRight(5,'0');
					//Procedure 3 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure3Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 3 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure3Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 3 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure3DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 3 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure3Charges"].ToString())).ToString().PadLeft(6,'0');
					//Procedure 4
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure4"].ToString())).PadRight(5,'0');
					//Procedure 4 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure4Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 4 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure4Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 4 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure4DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 4 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure4Charges"].ToString())).ToString().PadLeft(6,'0');
					//Procedure 5
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure5"].ToString())).PadRight(5,'0');
					//Procedure 5 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure5Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 5 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure5Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 5 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure5DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 5 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure5Charges"].ToString())).ToString().PadLeft(6,'0');
					//Procedure 6
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure6"].ToString())).PadRight(5,'0');
					//Procedure 6 modifier 1
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure6Modifier1"].ToString())).PadRight(2,'0');
					//Procedure 6 modifier 2
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure6Modifier2"].ToString())).PadRight(2,'0');
					//Procedure 6 diagnosis code
					outputRow+=POut.String(PIn.String(primaryCareReportRow.Rows[0]["Procedure6DiagnosisCode"].ToString())).PadRight(4,'0');
					//Procedure 6 charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["Procedure6Charges"].ToString())).ToString().PadLeft(6,'0');
					//Total charges
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["TotalCharges"].ToString())).ToString().PadLeft(7,'0');
					//Amount paid
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["AmountPaid"].ToString())).ToString().PadLeft(7,'0');
					//Balance due
					outputRow+=Math.Round(PIn.Double(primaryCareReportRow.Rows[0]["BalanceDue"].ToString())).ToString().PadLeft(7,'0');
					//Facility site number
					string siteId=PIn.String(primaryCareReportRow.Rows[0]["ClinicDescription"].ToString());
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
					//Physician ID
					string physicianId=PIn.String(primaryCareReportRow.Rows[0]["PhysicianID"].ToString());
					if(physicianId.Length>12){
						string newPhysicianId=physicianId.Substring(0,12);
						rowWarnings+="WARNING: The physician ID '"+physicianId+"' of the provider associated to the patient with a patnum of '"+patNum+
							"' is longer than 12 digits. The physician id has been truncated from '"+physicianId+"' to '"+newPhysicianId+"'."+Environment.NewLine;
						physicianId=newPhysicianId;
					}
					outputRow+=physicianId.PadLeft(12,'0');
					//Pysician's First Name and Middle Initial
					string physicianFirstAndMiddle=PIn.String(primaryCareReportRow.Rows[0]["PhysicianFAndMNames"].ToString());
					if(physicianFirstAndMiddle.Length>12){
						string newPhysicianFirstAndMiddle=physicianFirstAndMiddle.Substring(0,12);
						rowWarnings+="WARNING: The physician first name and middle initial of the provider associated to the patient with "+
							"a patnum of '"+patNum+"' was truncated from '"+physicianFirstAndMiddle+"' to '"+newPhysicianFirstAndMiddle+"'."+Environment.NewLine;
						physicianFirstAndMiddle=newPhysicianFirstAndMiddle;
					}
					outputRow+=physicianFirstAndMiddle.PadRight(12,' ');
					//Physician's last name.
					string physicianLastName=PIn.String(primaryCareReportRow.Rows[0]["PhysicianLName"].ToString());
					if(physicianLastName.Length>20){
						string newPhysicianLastName=physicianLastName.Substring(0,20);
						rowWarnings+="WARNING: The physician last name of the provider associated to the patient with a patnum of '"+patNum+"' "+
							"was truncated from '"+physicianLastName+"' to '"+newPhysicianLastName+"'."+Environment.NewLine;
						physicianLastName=newPhysicianLastName;
					}
					outputRow+=physicianLastName.PadRight(20,' ');
					//Finish adding the row to the output file and log warnings and errors.
					textLog.Text+=rowErrors+rowWarnings;
					if(rowErrors.Length>0) {
						continue;
					}
					outputText+=outputRow+Environment.NewLine;//Only add the row to the output file if it is properly formatted.
				}
			}
			File.WriteAllText(outFile,outputText);
			MessageBox.Show("Done.");
		}

		private void butFinished_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click_1(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}