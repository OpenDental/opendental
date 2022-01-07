using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;

namespace OpenDental{
	///<summary></summary>
	public partial class FormClaimPayEditOld:FormODBase {
		//private bool ControlDown;
		///<summary></summary>
		public bool IsNew;
		private decimal splitTot;
		///<summary>The list of splits to display in the grid.</summary>
		private List<ClaimPaySplit> splits;
		private ClaimPayment ClaimPaymentCur;
		///<summary>Set this externally.</summary>
		public long OriginatingClaimNum;
		List<Clinic> _listClinics;

		///<summary></summary>
		public FormClaimPayEditOld(ClaimPayment claimPaymentCur) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			ClaimPaymentCur=claimPaymentCur;
			splits=new List<ClaimPaySplit>();
			Lan.F(this);
		}

		private void FormClaimPayEdit_Load(object sender, System.EventArgs e) {
			//ClaimPayment created before opening this form
			if(IsNew){
				if(!Security.IsAuthorized(Permissions.InsPayCreate)){//date not checked here
					DialogResult=DialogResult.Cancel;//causes claimPayment to be deleted.
					return;
				}
			}
			else{
				if(!Security.IsAuthorized(Permissions.InsPayEdit,ClaimPaymentCur.CheckDate)){
					butOK.Enabled=false;
					butDelete.Enabled=false;
				}
			}
			if(IsNew){
				checkShowUn.Checked=true;
			}
			if(!PrefC.HasClinicsEnabled){
				comboClinic.Visible=false;
				labelClinic.Visible=false;
			}
			comboClinic.Items.Clear();
			comboClinic.Items.Add(Lan.g(this,"None"));
			comboClinic.SelectedIndex=0;
			_listClinics=Clinics.GetDeepCopy(true);
			for(int i=0;i<_listClinics.Count;i++){
				comboClinic.Items.Add(_listClinics[i].Abbr);
				if(_listClinics[i].ClinicNum==ClaimPaymentCur.ClinicNum){
					comboClinic.SelectedIndex=i+1;
				}
			}
			textDate.Text=ClaimPaymentCur.CheckDate.ToShortDateString();
			textCheckNum.Text=ClaimPaymentCur.CheckNum;
			textBankBranch.Text=ClaimPaymentCur.BankBranch;
			textCarrierName.Text=ClaimPaymentCur.CarrierName;
			textNote.Text=ClaimPaymentCur.Note;
			FillGrid();
			if(IsNew){
				gridMain.SetAll(true);
				splitTot=0;
				for(int i=0;i<gridMain.SelectedIndices.Length;i++){
					splitTot+=(decimal)splits[gridMain.SelectedIndices[i]].InsPayAmt;
				}
				textAmount.Text=splitTot.ToString("F");
			}
		}

		private void FillGrid(){
			splits=Claims.RefreshByCheckOld(ClaimPaymentCur.ClaimPaymentNum,checkShowUn.Checked);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableClaimPaySplits","Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableClaimPaySplits","Prov"),40);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableClaimPaySplits","Patient"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableClaimPaySplits","Carrier"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableClaimPaySplits","Fee"),65,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableClaimPaySplits","Payment"),65,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);			 
			gridMain.ListGridRows.Clear();
			GridRow row;
			splitTot=0;
			for(int i=0;i<splits.Count;i++){
				row=new GridRow();
				row.Cells.Add(splits[i].DateClaim.ToShortDateString());
				row.Cells.Add(splits[i].ProvAbbr);
				row.Cells.Add(splits[i].PatName);
				row.Cells.Add(splits[i].Carrier);
				row.Cells.Add(splits[i].FeeBilled.ToString("F"));
				row.Cells.Add(splits[i].InsPayAmt.ToString("F"));
				if(splits[i].ClaimNum==OriginatingClaimNum){
					row.Bold=true;
				}  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			for(int i=0;i<splits.Count;i++) {
				if(splits[i].ClaimPaymentNum==ClaimPaymentCur.ClaimPaymentNum) {
					gridMain.SetSelected(i,true);
					splitTot+=(decimal)splits[i].InsPayAmt;
				}
			}
			textAmount.Text=splitTot.ToString("F");
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			splitTot=0;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				splitTot+=(decimal)splits[gridMain.SelectedIndices[i]].InsPayAmt;
			}
			textAmount.Text=splitTot.ToString("F");
		}

		private void checkShowUn_Click(object sender, System.EventArgs e) {
			FillGrid();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//this button is disabled if user does not have permision to edit.
			if(IsNew){
				DialogResult=DialogResult.Cancel;//causes claimPayment to be deleted.
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this insurance check?")){
				return;
			}
			try{
				ClaimPayments.Delete(ClaimPaymentCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			for(int i=0;i<splits.Count;i++){
				if(splits[i].ClaimPaymentNum==ClaimPaymentCur.ClaimPaymentNum){
					SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,splits[i].PatNum,
						"Delete for patient: "
						+Patients.GetLim(splits[i].PatNum).GetNameLF()+", "
						+Lan.g(this,"Total Amt: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
						+Lan.g(this,"Claim Split: ")+splits[i].InsPayAmt.ToString("c"));
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			if(PIn.Date(textDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed) 
				&& !PrefC.GetBool(PrefName.AllowFutureInsPayments)) 
			{
				MsgBox.Show(this,"Payments cannot be for a date in the future.");
				return; //probably not necesasary since this is an old form, but just in case we use it again
			}
			if(!textDate.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(gridMain.SelectedIndices.Length==0){
				MessageBox.Show(Lan.g(this,"At least one item must be selected, or use the delete button."));	
				return;
			}
			if(IsNew){
				//prevents backdating of initial check
				if(!Security.IsAuthorized(Permissions.InsPayCreate,PIn.Date(textDate.Text))){
					return;
				}
				//prevents attaching claimprocs with a date that is older than allowed by security.



			}
			else{
				//Editing an old entry will already be blocked if the date was too old, and user will not be able to click OK button.
				//This catches it if user changed the date to be older.
				if(!Security.IsAuthorized(Permissions.InsPayEdit,PIn.Date(textDate.Text))){
					return;
				}
			}
			if(comboClinic.SelectedIndex==0){
				ClaimPaymentCur.ClinicNum=0;
			}
			else{
				ClaimPaymentCur.ClinicNum=_listClinics[comboClinic.SelectedIndex-1].ClinicNum;
			}
			ClaimPaymentCur.CheckAmt=PIn.Double(textAmount.Text);
			ClaimPaymentCur.CheckDate=PIn.Date(textDate.Text);
			ClaimPaymentCur.CheckNum=textCheckNum.Text;
			ClaimPaymentCur.BankBranch=textBankBranch.Text;
			ClaimPaymentCur.CarrierName=textCarrierName.Text;
			ClaimPaymentCur.Note=textNote.Text;
			try{
				ClaimPayments.Update(ClaimPaymentCur);//error thrown if trying to change amount and already attached to a deposit.
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			//this could be optimized to only save changes.
			//Would require a starting list to compare to.
			//But this isn't bad, since changes all saved at the very end
			List<int> selectedRows=new List<int>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++){
				selectedRows.Add(gridMain.SelectedIndices[i]);
			}
			for(int i=0;i<splits.Count;i++){
				if(selectedRows.Contains(i)){//row is selected
					ClaimProcs.SetForClaimOld(splits[i].ClaimNum,ClaimPaymentCur.ClaimPaymentNum,ClaimPaymentCur.CheckDate,true);
					//Audit trail isn't perfect, since it doesn't make an entry if you remove a claim from a payment.
					//And it always makes more audit trail entries when you click OK, even if you didn't actually attach new claims.
					//But since this will cover the vast majority if situations.
					if(IsNew){
						SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,splits[i].PatNum,
							Patients.GetLim(splits[i].PatNum).GetNameLF()+", "
							+Lan.g(this,"Total Amt: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
							+Lan.g(this,"Claim Split: ")+splits[i].InsPayAmt.ToString("c"));
					}
					else{
						SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,splits[i].PatNum,
							Patients.GetLim(splits[i].PatNum).GetNameLF()+", "
							+Lan.g(this,"Total Amt: ")+ClaimPaymentCur.CheckAmt.ToString("c")+", "
							+Lan.g(this,"Claim Split: ")+splits[i].InsPayAmt.ToString("c"));
					}
				}
				else{//row not selected
					//If user had not been attaching their inspayments to checks, then this will cause such payments to annoyingly have their
					//date changed to the current date.  This prompts them to call us.  Then, we tell them to attach to checks.
					ClaimProcs.SetForClaimOld(splits[i].ClaimNum,ClaimPaymentCur.ClaimPaymentNum,ClaimPaymentCur.CheckDate,false);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void FormClaimPayEdit_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.OK){
				return;
			}
			if(IsNew){//cancel
				//ClaimProcs never saved in the first place
				ClaimPayments.Delete(ClaimPaymentCur);
			}
		}

		



	}
}













