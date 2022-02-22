using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental;

namespace TestCanada {
	public partial class FormTestCanada:Form {
		private static string dbname="canadatest";

		public FormTestCanada() {
			InitializeComponent();
		}

		private void butNewDb_Click(object sender,EventArgs e) {
			textResults.Text="";
			Application.DoEvents();
			Cursor=Cursors.WaitCursor;
			if(!DatabaseTools.SetDbConnection("")){
				MessageBox.Show("Could not connect");
				return;
			}
			DatabaseTools.FreshFromDump();
			textResults.Text+="Fresh database loaded from sql dump.";
			Cursor=Cursors.Default;
		}

		private void butClear_Click(object sender,EventArgs e) {
			textResults.Text="";
			Application.DoEvents();
			Cursor=Cursors.WaitCursor;
			if(!DatabaseTools.SetDbConnection(dbname)) {//if database doesn't exist
				//MessageBox.Show("Database canadatest does not exist.");
				DatabaseTools.SetDbConnection("");
				textResults.Text+=DatabaseTools.FreshFromDump();//this also sets database to be unittest.
			}
			textResults.Text+=DatabaseTools.ClearDb();
			textResults.Text+="Done.";
			Cursor=Cursors.Default;
		}

		private void butObjects_Click(object sender,EventArgs e) {
			FillObjects();
			textResults.Text+="Done.";
			Cursor=Cursors.Default;
		}

		private void FillObjects(){
			textResults.Text="";
			Application.DoEvents();
			Cursor=Cursors.WaitCursor;
			if(!DatabaseTools.SetDbConnection("canadatest")) {//if database doesn't exist
				//MessageBox.Show("Database canadatest does not exist.");
				DatabaseTools.SetDbConnection("");
				textResults.Text+=DatabaseTools.FreshFromDump();//this also sets database to be unittest.
			}
			else {
				textResults.Text+=DatabaseTools.ClearDb();
			}
			Prefs.RefreshCache();
			List <Userod> listUsers=Userods.GetAll();
			Security.CurUser=listUsers[0];
			textResults.Text+=ProviderTC.SetInitialProviders();
			Application.DoEvents();
			textResults.Text+=CarrierTC.SetInitialCarriers();
			Application.DoEvents();
			textResults.Text+=PatientTC.SetInitialPatients(); 
			Application.DoEvents();
			textResults.Text+=ClaimTC.CreateAllClaims();
			Application.DoEvents();
			textResults.Text+=PredeterminationTC.CreateAllPredeterminations();
			Application.DoEvents();
		}

		private void butScripts_Click(object sender,EventArgs e) {
			if(textSingleScript.Text==""){
				MessageBox.Show("Please enter a script number first.");
				return;
			}
			int singleScript=0;
			try{
				singleScript=PIn.Int(textSingleScript.Text);
				if(singleScript==0){
					MessageBox.Show("Invalid number.");
					return;
				}
			}
			catch{
				MessageBox.Show("Invalid number.");
				return;
			}
			int checkedCount=0;
			if(checkEligibility.Checked) {
				checkedCount++;
			}
			if(checkClaims.Checked){
				checkedCount++;
			}
			if(checkClaimReversals.Checked){
				checkedCount++;
			}
			if(checkOutstanding.Checked){
				checkedCount++;
			}
			if(checkPredeterm.Checked){
				checkedCount++;
			}
			if(checkPayReconcil.Checked){
				checkedCount++;
			}
			if(checkSumReconcil.Checked){
				checkedCount++;
			}
			if(checkedCount==0){
				MessageBox.Show("Please select a category.");
				return;
			}
			FillObjects();
			textResults.Text+="---------------------------------------\r\n";
			Application.DoEvents();
			if(checkEligibility.Checked) {
				if(singleScript==1){
					textResults.Text+=Eligibility.RunOne(checkShowForms.Checked);
				}
				else if(singleScript==2){
					textResults.Text+=Eligibility.RunTwo(checkShowForms.Checked);
				}
				else if(singleScript==3){
					textResults.Text+=Eligibility.RunThree(checkShowForms.Checked);
				}
				else if(singleScript==4){
					textResults.Text+=Eligibility.RunFour(checkShowForms.Checked);
				}
				else if(singleScript==5){
					textResults.Text+=Eligibility.RunFive(checkShowForms.Checked);
				}
				else if(singleScript==6){
					textResults.Text+=Eligibility.RunSix(checkShowForms.Checked);
				}
				else{
					MessageBox.Show("Script number not found.");
					return;
				}
			}
			if(checkClaims.Checked){
				if(singleScript==1){
					textResults.Text+=ClaimTC.RunOne(checkShowForms.Checked);
				}
				else if(singleScript==2) {
					textResults.Text+=ClaimTC.RunTwo(checkShowForms.Checked);
				}
				else if(singleScript==3) {
					textResults.Text+=ClaimTC.RunThree(checkShowForms.Checked);
				}
				else if(singleScript==4) {
					textResults.Text+=ClaimTC.RunFour(checkShowForms.Checked);
				}
				else if(singleScript==5) {
					textResults.Text+=ClaimTC.RunFive(checkShowForms.Checked);
				}
				else if(singleScript==6) {
					textResults.Text+=ClaimTC.RunSix(checkShowForms.Checked);
				}
				else if(singleScript==7) {
					textResults.Text+=ClaimTC.RunSeven(checkShowForms.Checked);
				}
				else if(singleScript==8) {
					textResults.Text+=ClaimTC.RunEight(checkShowForms.Checked);
				}
				else if(singleScript==9) {
					textResults.Text+=ClaimTC.RunNine(checkShowForms.Checked);
				}
				else if(singleScript==10) {
					textResults.Text+=ClaimTC.RunTen(checkShowForms.Checked);
				}
				else if(singleScript==11) {
					textResults.Text+=ClaimTC.RunEleven(checkShowForms.Checked);
				}
				else if(singleScript==12) {
					textResults.Text+=ClaimTC.RunTwelve(checkShowForms.Checked);
				}
				else{
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			if(checkClaimReversals.Checked){
				if(singleScript==1) {
					textResults.Text+=Reversal.RunOne();
				}
				else if(singleScript==2) {
					textResults.Text+=Reversal.RunTwo();
				}
				else if(singleScript==3) {
					textResults.Text+=Reversal.RunThree();
				}
				else if(singleScript==4) {
					textResults.Text+=Reversal.RunFour();
				}
				else {
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			if(checkOutstanding.Checked){
				if(singleScript==1) {
					textResults.Text+=OutstandingTrans.RunOne();
				}
				else if(singleScript==2) {
					textResults.Text+=OutstandingTrans.RunTwo();
				}
				else if(singleScript==3) {
					textResults.Text+=OutstandingTrans.RunThree();
				}
				else {
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			if(checkPredeterm.Checked){
				if(singleScript==1) {
					textResults.Text+=PredeterminationTC.RunOne(checkShowForms.Checked);
				}
				else if(singleScript==2) {
					textResults.Text+=PredeterminationTC.RunTwo(checkShowForms.Checked);
				}
				else if(singleScript==3) {
					textResults.Text+=PredeterminationTC.RunThree(checkShowForms.Checked);
				}
				else if(singleScript==4) {
					textResults.Text+=PredeterminationTC.RunFour(checkShowForms.Checked);
				}
				else if(singleScript==5) {
					textResults.Text+=PredeterminationTC.RunFive(checkShowForms.Checked);
				}
				else if(singleScript==6) {
					textResults.Text+=PredeterminationTC.RunSix(checkShowForms.Checked);
				}
				else if(singleScript==7) {
					textResults.Text+=PredeterminationTC.RunSeven(checkShowForms.Checked);
				}
				else if(singleScript==8) {
					textResults.Text+=PredeterminationTC.RunEight(checkShowForms.Checked);
				}
				else {
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			if(checkPayReconcil.Checked){
				if(singleScript==1) {
					textResults.Text+=PaymentReconciliation.RunOne();
				}
				else if(singleScript==2) {
					textResults.Text+=PaymentReconciliation.RunTwo();
				}
				else if(singleScript==3) {
					textResults.Text+=PaymentReconciliation.RunThree();
				}
				else {
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			if(checkSumReconcil.Checked){
				if(singleScript==1) {
					textResults.Text+=SummaryReconciliation.RunOne();
				}
				else if(singleScript==2) {
					textResults.Text+=SummaryReconciliation.RunTwo();
				}
				else if(singleScript==3) {
					textResults.Text+=SummaryReconciliation.RunThree();
				}
				else {
					MessageBox.Show("Script number not found (not implemented yet).");
					return;
				}
			}
			textResults.Text+="Done.";
			Cursor=Cursors.Default;
		}

		private void checkEligibility_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkEligibility);
		}

		private void checkClaims_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkClaims);
		}

		private void checkClaimReversals_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkClaimReversals);
		}

		private void checkOutstanding_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkOutstanding);
		}

		private void checkPredeterm_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkPredeterm);
		}

		private void checkPayReconcil_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkPayReconcil);
		}

		private void checkSumReconcil_Click(object sender,EventArgs e) {
			UncheckAllExcept(checkSumReconcil);
		}

		private void UncheckAllExcept(CheckBox checkbox) {
			if(checkbox!=checkEligibility){
				checkEligibility.Checked=false;
			}
			if(checkbox!=checkClaims){
				checkClaims.Checked=false;
			}
			if(checkbox!=checkClaimReversals){
				checkClaimReversals.Checked=false;
			}
			if(checkbox!=checkOutstanding){
				checkOutstanding.Checked=false;
			}
			if(checkbox!=checkPredeterm){
				checkPredeterm.Checked=false;
			}
			if(checkbox!=checkPayReconcil){
				checkPayReconcil.Checked=false;
			}
			if(checkbox!=checkSumReconcil){
				checkSumReconcil.Checked=false;
			}
		}

		private void butShowEtrans_Click(object sender,EventArgs e) {
			if(!checkClaims.Checked){
				MessageBox.Show("Only works for claims right now.");
				return;
			}
			//In case the form was just opened
			DatabaseTools.SetDbConnection(dbname);
			int scriptNum=PIn.Int(textSingleScript.Text);
			long patNum=0;
			double claimFee=0;
			string predeterm="";
			switch(scriptNum){
				case 1:
					patNum=Patients.GetPatNumByNameAndBirthday("Fête","Lisa",new DateTime(1960,4,12));
					claimFee=222.35;
					break;
				case 2:
					patNum=Patients.GetPatNumByNameAndBirthday("Fête","Lisa",new DateTime(1960,4,12));
					claimFee=1254.85;
					break;
				case 3:
					patNum=Patients.GetPatNumByNameAndBirthday("Smith","John",new DateTime(1948,3,2));
					claimFee=439.55;
					break;
				case 4:
					patNum=Patients.GetPatNumByNameAndBirthday("Smith","John",new DateTime(1988,11,2));
					claimFee=222.35;
					break;
				case 5:
					patNum=Patients.GetPatNumByNameAndBirthday("Howard","Bob",new DateTime(1964,5,16));
					claimFee=222.35;
					break;
				case 6:
					patNum=Patients.GetPatNumByNameAndBirthday("Howard","Bob",new DateTime(1964,5,16));
					claimFee=232.35;
					break;
				case 7:
					patNum=Patients.GetPatNumByNameAndBirthday("Howard","Bob",new DateTime(1964,5,16));
					claimFee=232.35;
					predeterm="PD78901234";
					break;
				case 8:
					patNum=Patients.GetPatNumByNameAndBirthday("West","Martha",new DateTime(1954,12,25));
					claimFee=565.35;
					break;
				case 9:
					patNum=Patients.GetPatNumByNameAndBirthday("Arpège","Madeleine",new DateTime(1940,5,1));
					claimFee=527.35;
					break;
			}
			List<Claim> claimList=Claims.Refresh(patNum);
			Claim claim=null;
			for(int i=0;i<claimList.Count;i++){
				if(claimList[i].ClaimFee==claimFee && claimList[i].PreAuthString==predeterm){
					claim=claimList[i];
				}
			}
			if(claim==null){
				MessageBox.Show("Claim not found.");
				return;
			}
			List<Etrans> etransList=Etranss.GetHistoryOneClaim(claim.ClaimNum);
			if(etransList.Count==0) {
				MessageBox.Show("No history found of sent e-claim.");
				return;
			}
			FormEtransEdit FormE=new FormEtransEdit();
			FormE.EtransCur=etransList[0];
			FormE.ShowDialog();
		}

		private void radioCompareInput_Click(object sender,EventArgs e) {
			radioCompareInput.Checked=true;
			radioCompareOutput.Checked=false;
		}

		private void radioCompareOutput_Click(object sender,EventArgs e) {
			radioCompareInput.Checked=false;
			radioCompareOutput.Checked=true;
		}

		private void buttonCompFileBrowse_Click(object sender,EventArgs e) {
			openFileDialog1.FileName=textCompareFilePath.Text;
			if(openFileDialog1.ShowDialog()==DialogResult.OK) {
				textCompareFilePath.Text=openFileDialog1.FileName;
			}
		}

		private void butCompare_Click(object sender,EventArgs e) {
			if(!File.Exists(textCompareFilePath.Text)) {
				MessageBox.Show("The specified file path of file to compare either doesn't exist or is inaccessible.");
				return;
			}
			Cursor=Cursors.WaitCursor;
			string outputToCompare="";
			if(radioCompareInput.Checked) {
				outputToCompare=File.ReadAllText(@"C:\iCA\_nput.000",Encoding.GetEncoding(850));
			}
			else { //radioCompareOutput.Checked
				outputToCompare=File.ReadAllText(@"C:\iCA\output.000",Encoding.GetEncoding(850));
			}
			string[] fileLines=File.ReadAllLines(textCompareFilePath.Text,Encoding.GetEncoding(850));
			richTextCompare.Text="";
			for(int i=0;i<fileLines.Length;i++) {
				string line=fileLines[i];
				if(line.Trim()=="") {
					continue;
				}
				//Display the top line
				for(int j=0;j<outputToCompare.Length;j++) {
					richTextCompare.SelectionColor=Color.Red;
					if(j<line.Length) {
						if(outputToCompare[j]==line[j]) {
							richTextCompare.SelectionColor=Color.Gray;
						}
					}
					richTextCompare.SelectedText+=outputToCompare[j];
				}
				richTextCompare.SelectedText+=Environment.NewLine;
				//Display the bottom line
				for(int j=0;j<line.Length;j++) {
					richTextCompare.SelectionColor=Color.Green;
					if(j<outputToCompare.Length) {
						if(line[j]==outputToCompare[j]) {
							richTextCompare.SelectionColor=Color.Black;
						}
					}
					richTextCompare.SelectedText+=line[j];
				}
				richTextCompare.SelectedText+=Environment.NewLine;				
			}
			Cursor=Cursors.Default;
		}

		

		/*
		private void SetCheckAll() {
			bool someChecked=false;
			if(checkEligibility.Checked
				|| checkClaims.Checked
				|| checkClaimReversals.Checked
				|| checkOutstanding.Checked
				|| checkPredeterm.Checked
				|| checkPayReconcil.Checked
				|| checkSumReconcil.Checked) 
			{
				someChecked=true;
			}
			bool someUnchecked=false;
			if(!checkEligibility.Checked
				|| !checkClaims.Checked
				|| !checkClaimReversals.Checked
				|| !checkOutstanding.Checked
				|| !checkPredeterm.Checked
				|| !checkPayReconcil.Checked
				|| !checkSumReconcil.Checked) 
			{
				someUnchecked=true;
			}
			if(someChecked && someUnchecked) {
				checkAll.CheckState=CheckState.Indeterminate;
			}
			else if(someChecked) {
				checkAll.CheckState=CheckState.Checked;
			}
			else {
				checkAll.CheckState=CheckState.Unchecked;
			}
		}

		private void checkAll_Click(object sender,EventArgs e) {
			if(checkAll.CheckState==CheckState.Indeterminate) {
				checkAll.CheckState=CheckState.Unchecked;//make it behave like a two state box
			}
			if(checkAll.CheckState==CheckState.Checked) {
				checkEligibility.Checked=true;
				checkClaims.Checked=true;
				checkClaimReversals.Checked=true;
				checkOutstanding.Checked=true;
				checkPredeterm.Checked=true;
				checkPayReconcil.Checked=true;
				checkSumReconcil.Checked=true;
			}
			if(checkAll.CheckState==CheckState.Unchecked) {
				checkEligibility.Checked=false;
				checkClaims.Checked=false;
				checkClaimReversals.Checked=false;
				checkOutstanding.Checked=false;
				checkPredeterm.Checked=false;
				checkPayReconcil.Checked=false;
				checkSumReconcil.Checked=false;
			}
		}*/
	}
}
