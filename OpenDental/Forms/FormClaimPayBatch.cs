using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using System.Globalization;

namespace OpenDental{
///<summary></summary>
	public partial class FormClaimPayBatch:FormODBase {
		//private bool ControlDown;
		///<summary></summary>
		public bool IsNew;
		private ClaimPayment ClaimPaymentCur;
		private bool IsDeleting;
		///<summary>If this is not zero upon closing, then we will jump to the account module of that patient and highlight the claim.</summary>
		public long GotoClaimNum;
		///<summary>If this is not zero upon closing, then we will jump to the account module of that patient and highlight the claim.</summary>
		public long GotoPatNum;
		///<summary>The list of claims that have been detached in this window.</summary>
		private List<ClaimPaySplit> _listDetachedClaims=new List<ClaimPaySplit>();
		private List<ClaimPaySplit> _listClaimsOutstanding=null;
		private List<ClaimPaySplit> _listClaimsAttached;
		///<summary>Set to true if the batch list was accessed originally by going through a claim.  This disables the GotoAccount feature.  It also causes OK/Cancel buttons to show so that user can cancel out of a brand new check creation.</summary>
		public bool IsFromClaim;

		///<summary></summary>
		public FormClaimPayBatch(ClaimPayment claimPaymentCur,bool isRefreshNeeded=false) {
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			if(isRefreshNeeded) {
				claimPaymentCur=ClaimPayments.GetOne(claimPaymentCur?.ClaimPaymentNum??0);
			}
			ClaimPaymentCur=claimPaymentCur;
			gridAttached.ContextMenu=menuRightAttached;
			gridOut.ContextMenu=menuRightOut;
			Lan.F(this);
		}

		private void FormClaimPayEdit_Load(object sender, System.EventArgs e) {
			SetFilterControlsAndAction(() => FillGrids(false),textName,textClaimID);
			if(ClaimPaymentCur==null) {
				MsgBox.Show(this,"Claim payment does not exist.");
				DialogResult=DialogResult.Abort;
				if(!this.Modal) {
					Close();
				}
				return;
			}
			if(IsFromClaim && IsNew) {
				//ok and cancel
				labelInstruct1.Visible=false;
				labelInstruct2.Visible=false;
				gridOut.Visible=false;
				groupFilters.Visible=false;
				butAttach.Visible=false;
				butViewEra.Visible=false;
			}
			else {
				butOK.Visible=false;
				butClose.Text=Lan.g(this,"Close");
			}
			if(IsFromClaim) {
				//Remove context menus from the grids.  This preserves old functionality.
				gridAttached.ContextMenu=null;
				gridOut.ContextMenu=null;
			}
			FillClaimPayment();
			textCarrier.Text=textCarrierName.Text;
			FillGrids();
			if(ClaimPaymentCur.IsPartial){
				//an incomplete payment that's not yet locked
			}
			else{//locked
				if(!Security.IsAuthorized(Permissions.InsPayEdit,ClaimPaymentCur.CheckDate)) {
					butDelete.Enabled=false;
					gridAttached.AllowSelection=false;
					gridAttached.CellDoubleClick-=gridAttached_CellDoubleClick;
					butClaimPayEdit.Enabled=false;
					butUp.Visible=false;
					butDown.Visible=false;
				}
				//someone with permission can double click on the top grid to edit amounts and can edit the object fields as well.
				butDetach.Visible=false;
				gridOut.Visible=false;
				groupFilters.Visible=false;
				labelInstruct1.Visible=false;
				labelInstruct2.Visible=false;
				butAttach.Visible=false;
			}
			if(EobAttaches.Exists(ClaimPaymentCur.ClaimPaymentNum)) {
				textEobIsScanned.Text=Lan.g(this,"Yes");
				butView.Text="View EOB";
			}
			else {
				textEobIsScanned.Text=Lan.g(this,"No");
				butView.Text="Scan EOB";
			}
		}

		private void FillClaimPayment() {
			textClinic.Text=Clinics.GetAbbr(ClaimPaymentCur.ClinicNum);
			if(ClaimPaymentCur.CheckDate.Year>1800) {
				textDate.Text=ClaimPaymentCur.CheckDate.ToShortDateString();
			}
			if(ClaimPaymentCur.DateIssued.Year>1800) {
				textDateIssued.Text=ClaimPaymentCur.DateIssued.ToShortDateString();
			}
			textAmount.Text=ClaimPaymentCur.CheckAmt.ToString("F");
			textCheckNum.Text=ClaimPaymentCur.CheckNum;
			textBankBranch.Text=ClaimPaymentCur.BankBranch;
			textCarrierName.Text=ClaimPaymentCur.CarrierName;
			textNote.Text=ClaimPaymentCur.Note;
			textPayType.Text=Defs.GetName(DefCat.InsurancePaymentType,ClaimPaymentCur.PayType);
			textPayGroup.Text=Defs.GetName(DefCat.ClaimPaymentGroups,ClaimPaymentCur.PayGroup);
		}

		private void FillGrids(bool doRefreshOutstandingClaims=true){
			Cursor.Current=Cursors.WaitCursor;
			#region gridAttached
			_listClaimsAttached=Claims.GetAttachedToPayment(ClaimPaymentCur.ClaimPaymentNum);
			List<long> listClaimNumsToUpdate=new List<long>();
			List<int> listPaymentRows=new List<int>();
			for(int i=0;i<_listClaimsAttached.Count;i++) {
				if(_listClaimsAttached[i].PaymentRow!=i+1) {
					listClaimNumsToUpdate.Add(_listClaimsAttached[i].ClaimNum);
					listPaymentRows.Add(i+1);
					_listClaimsAttached[i].PaymentRow=i+1;
				}
			}
			ClaimProcs.SetPaymentRow(listClaimNumsToUpdate,ClaimPaymentCur.ClaimPaymentNum,listPaymentRows);
			gridAttached.BeginUpdate();
			gridAttached.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"#"),25);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Service Date"),80);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic"),70);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Claim Status"),80);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Carrier"),186);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),130);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee"),70,HorizontalAlignment.Right);
			gridAttached.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Payment"),70,HorizontalAlignment.Right);
			gridAttached.ListGridColumns.Add(col); 
			gridAttached.ListGridRows.Clear();
			GridRow row;
			double total=0;
			foreach(ClaimPaySplit claimPS in _listClaimsAttached) {
				total+=claimPS.InsPayAmt;
				row=new GridRow();
				row.Cells.Add(claimPS.PaymentRow.ToString());
				row.Cells.Add(claimPS.DateClaim.ToShortDateString());
				row.Cells.Add(claimPS.ClinicDesc);
				if(claimPS.ClaimStatus=="S") {
					row.Cells.Add("Sent");
				}
				else if(claimPS.ClaimStatus=="R") {
					row.Cells.Add("Received");
				}
				else {
					row.Cells.Add("Unknown");
				}
				row.Cells.Add(claimPS.Carrier);
				row.Cells.Add(claimPS.PatName);
				row.Cells.Add(claimPS.FeeBilled.ToString("F"));
				row.Cells.Add(claimPS.InsPayAmt.ToString("F"));
				row.Tag=claimPS;
				gridAttached.ListGridRows.Add(row);
			}
			gridAttached.Tag=_listClaimsAttached;
			gridAttached.EndUpdate();
			textTotal.Text=total.ToString("F");
			#endregion gridAttached
			if((IsFromClaim && IsNew) || !ClaimPaymentCur.IsPartial) {//gridOut isn't visible
				//if new batch claim payment opened from a claim, or if it's locked (not partial), gridOut isn't visible so no need to fill it
				Cursor.Current=Cursors.Default;
				return;
			}
			#region gridOutstanding
			int scrollValue=gridOut.ScrollValue;
			int selectedIdx=gridOut.GetSelectedIndex();
			if(!doRefreshOutstandingClaims && _listClaimsOutstanding!=null) {
				//Remove ClaimPaySplits that are attached to the insurance payment.
				_listClaimsOutstanding.RemoveAll(x => ListTools.In(x.ClaimNum,_listClaimsAttached.Select(y => y.ClaimNum)));
				//Add claims that have been detached if necessary.
				foreach(ClaimPaySplit detachedClaim in _listDetachedClaims) {
					if(!_listClaimsAttached.Any(x => x.ClaimNum==detachedClaim.ClaimNum)
						&& !_listClaimsOutstanding.Any(x => x.ClaimNum==detachedClaim.ClaimNum)) 
					{
						_listClaimsOutstanding.Add(detachedClaim);
					}
				}
			}
			else {
				_listClaimsOutstanding=Claims.GetOutstandingClaims(textCarrier.Text,PrefC.DateClaimReceivedAfter);
				_listDetachedClaims.Clear();
			}
			gridOut.BeginUpdate();
			gridOut.ListGridColumns.Clear();
			col=new GridColumn("",25);//so that it lines up with the grid above
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Service Date"),80);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Clinic"),70);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Claim Status"),80);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Carrier"),186);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Patient"),130);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Fee"),70,HorizontalAlignment.Right);
			gridOut.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Payment"),70,HorizontalAlignment.Right);
			gridOut.ListGridColumns.Add(col);
			gridOut.ListGridRows.Clear();
			foreach(ClaimPaySplit claimPS in _listClaimsOutstanding) {
				if(textClaimID.Text!="" && !claimPS.ClaimIdentifier.Contains(textClaimID.Text.Trim())) {
					continue;
				}
				if(textName.Text!="" && !claimPS.PatName.ToLower().Contains(textName.Text.ToLower().Trim())) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add("");
				row.Cells.Add(claimPS.DateClaim.ToShortDateString());
				row.Cells.Add(claimPS.ClinicDesc);
				if(claimPS.ClaimStatus=="S") {
					row.Cells.Add("Sent");
				}
				else if(claimPS.ClaimStatus=="R") {
					row.Cells.Add("Received");
				}
				else {
					row.Cells.Add("Unknown");
				}
				row.Cells.Add(claimPS.Carrier);
				row.Cells.Add(claimPS.PatName);
				row.Cells.Add(claimPS.FeeBilled.ToString("F"));
				row.Cells.Add(claimPS.InsPayAmt.ToString("F"));
				row.Tag=claimPS;
				gridOut.ListGridRows.Add(row);
			}
			gridOut.Tag=_listClaimsOutstanding;
			gridOut.EndUpdate();
			gridOut.ScrollValue=scrollValue;
			gridOut.SetSelected(selectedIdx,true);
			#endregion gridOutstanding
			Cursor.Current=Cursors.Default;
		}

		/// <summary>runs aging for all families for patient's on claims attached to payment</summary>
		private void runAgingforClaims() {
			List<long> listGuarNums = Patients.GetGuarantorsForPatNums(_listClaimsAttached.Select(x => x.PatNum).ToList());
			Ledgers.ComputeAging(listGuarNums,DateTime.Today);
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrids();
		}

		private void butClaimPayEdit_Click(object sender,EventArgs e) {
			using FormClaimPayEdit FormCPE=new FormClaimPayEdit(ClaimPaymentCur);
			FormCPE.IsFinalizePayment=IsNew;
			FormCPE.ShowDialog();
			ClaimPaymentCur=FormCPE.ClaimPaymentCur;
			FillClaimPayment();
			FillGrids();//For customer 5769, who was getting ocassional Chinese chars in the Amount boxes.
		}

		private void butAttach_Click(object sender,EventArgs e) {
			if(gridOut.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one paid claim from the Outstanding Claims grid below.");
				return;
			}
			bool hasClaimNoValidReceivedPayments=false;
			int paymentRow=gridAttached.ListGridRows.Count;//1-indexed
			foreach(ClaimPaySplit selectedClaimPS in gridOut.SelectedTags<ClaimPaySplit>()) {
				if(ClaimProcs.AttachToPayment(selectedClaimPS.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,ClaimPaymentCur.CheckDate,paymentRow) > 0) {
					paymentRow++;//Only increment the paymentRow if there were claimprocs attached to the payment.
				}
				else {
					hasClaimNoValidReceivedPayments=true;
				}
			}
			if(hasClaimNoValidReceivedPayments) {
				MsgBox.Show(this,"There was at least one outstanding claim selected with no valid received payments.");
			}
			FillGrids();//Always refresh the outstanding claims grid just in case there were non-received claimprocs attached to the claim (e.g. pre-auth).
		}

		private void butDetach_Click(object sender,EventArgs e) {
			if(gridAttached.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a claim from the attached claims grid above.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove selected claims from this check?")) {
				return;
			}
			List<ClaimPaySplit> listDetachedClaims=gridAttached.SelectedTags<ClaimPaySplit>();
			ClaimProcs.DetachFromPayment(listDetachedClaims.Select(x => x.ClaimNum).ToList(),ClaimPaymentCur.ClaimPaymentNum);
			_listDetachedClaims.AddRange(listDetachedClaims);
			FillGrids(false);
			bool didReorder=false;
			for(int i=0;i<gridAttached.ListGridRows.Count;i++) {
				ClaimPaySplit claimPSAttached=(ClaimPaySplit)gridAttached.ListGridRows[i].Tag;
				if(claimPSAttached.PaymentRow!=i+1) {
					ClaimProcs.SetPaymentRow(claimPSAttached.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,i+1);
					didReorder=true;
				}
			}
			if(didReorder) {
				FillGrids(false);
			}
		}

		private void gridAttached_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			//top grid
			//bring up claimedit window.  User should be able to edit if not locked.
			if(e.Row<0 || e.Row>=gridAttached.ListGridRows.Count) {//If an invalid row was clicked on somehow, return.
				return;
			}
			ClaimPaySplit claimPS=(ClaimPaySplit)gridAttached.ListGridRows[e.Row].Tag;
			Claim claimCur=Claims.GetClaim(claimPS.ClaimNum);
			if(claimCur==null) {
				MsgBox.Show(this,"The claim has been deleted.");
				FillGrids();
				return;
			}
			using FormClaimEdit FormCE=new FormClaimEdit(claimCur,Patients.GetPat(claimCur.PatNum),Patients.GetFamily(claimCur.PatNum));
			FormCE.IsFromBatchWindow=true;
			FormCE.ShowDialog();
			FillGrids();	
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(gridAttached.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridAttached.SelectedIndices.Length];//remember the selected rows so that we can reselect them
			for(int i=0;i<gridAttached.SelectedIndices.Length;i++) {
				selected[i]=gridAttached.SelectedIndices[i];
			}
			if(selected[0]==0) {//can't go up
				return;
			}
			for(int i=0;i<selected.Length;i++) {
				ClaimPaySplit claimPaySplitAbove=(ClaimPaySplit)gridAttached.ListGridRows[selected[i]-1].Tag;
				ClaimPaySplit selectedClaimPaySplit=(ClaimPaySplit)gridAttached.ListGridRows[selected[i]].Tag;
				//In the db, move the one above down to the current pos
				ClaimProcs.SetPaymentRow(claimPaySplitAbove.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,selected[i]+1);
				//and move this row up one
				ClaimProcs.SetPaymentRow(selectedClaimPaySplit.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,selected[i]);
			}
			FillGrids(false);
			for(int i=0;i<selected.Length;i++) {
				gridAttached.SetSelected(selected[i]-1,true);
			}
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(gridAttached.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an item in the grid first.");
				return;
			}
			int[] selected=new int[gridAttached.SelectedIndices.Length];
			for(int i=0;i<gridAttached.SelectedIndices.Length;i++) {
				selected[i]=gridAttached.SelectedIndices[i];
			}
			if(selected[selected.Length-1]==gridAttached.ListGridRows.Count-1) {//already at the bottom
				return;
			}
			for(int i=selected.Length-1;i>=0;i--) {//go backwards
				ClaimPaySplit claimPaySplitBelow=(ClaimPaySplit)gridAttached.ListGridRows[selected[i]+1].Tag;
				ClaimPaySplit selectedClaimPaySplit=(ClaimPaySplit)gridAttached.ListGridRows[selected[i]].Tag;
				//In the db, move the one below up to the current pos
				ClaimProcs.SetPaymentRow(claimPaySplitBelow.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,selected[i]+1);
				//and move this row down one
				ClaimProcs.SetPaymentRow(selectedClaimPaySplit.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,selected[i]+2);
			}
			FillGrids(false);
			for(int i=0;i<selected.Length;i++) {
				gridAttached.SetSelected(selected[i]+1,true);
			}
		}

		private void gridOut_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimView)) {
				return;
			}
			//bottom grid
			//bring up claimedit window
			//after returning from the claim edit window, use a query to get a list of all the claimprocs that have amounts entered for that claim, but have ClaimPaymentNumber of 0.
			//Set all those claimprocs to be attached.
			if(e.Row<0 || e.Row>=gridOut.ListGridRows.Count) {//If an invalid row was clicked on, return.
				return;
			}
			ClaimPaySplit claimPS=(ClaimPaySplit)gridOut.ListGridRows[e.Row].Tag;
			Claim claimCur=Claims.GetClaim(claimPS.ClaimNum);
			if(claimCur==null) {
				MsgBox.Show(this,"The claim has been deleted.");
				FillGrids();
				return;
			}
			using FormClaimEdit FormCE=new FormClaimEdit(claimCur,Patients.GetPat(claimCur.PatNum),Patients.GetFamily(claimCur.PatNum));
			FormCE.IsFromBatchWindow=true;
			FormCE.ShowDialog();
			if(FormCE.DialogResult!=DialogResult.OK){
				return;
			}
			if(ClaimProcs.AttachToPayment(claimCur.ClaimNum,ClaimPaymentCur.ClaimPaymentNum,ClaimPaymentCur.CheckDate,gridAttached.ListGridRows.Count+1)==0) {
				MsgBox.Show(this,"There are no valid received payments for this claim.");
			}
			FillGrids(false);
		}

		private void butCarrierPick_Click(object sender,EventArgs e) {
			using FormCarriers formC=new FormCarriers();
			formC.IsSelectMode=true;
			if(formC.ShowDialog()!=DialogResult.OK) {
				return;
			}
			textCarrier.Text=formC.CarrierSelected.CarrierName;
			FillGrids();
		}

		private void menuItemGotoAccount_Click(object sender,EventArgs e) {
			//for the upper grid
			if(gridAttached.SelectedIndices.Length!=1 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			ClaimPaySplit selectedClaimPS=gridAttached.SelectedTag<ClaimPaySplit>();
			GotoPatNum=selectedClaimPS.PatNum;
			GotoClaimNum=selectedClaimPS.ClaimNum;
			Patient pat=Patients.GetPat(GotoPatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoClaim(GotoClaimNum);
		}

		private void menuItemGotoOut_Click(object sender,EventArgs e) {
			//for the lower grid
			if(gridOut.SelectedIndices.Length!=1 || !Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			ClaimPaySplit selectedClaimPS=gridOut.SelectedTag<ClaimPaySplit>();
			GotoPatNum=selectedClaimPS.PatNum;
			GotoClaimNum=selectedClaimPS.ClaimNum;
			Patient pat=Patients.GetPat(GotoPatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoClaim(GotoClaimNum);
		}

		//private void menuItemGoToAccount_Click(object sender,EventArgs e) {
			
			//Patient pat=Patients.GetPat(FormCS.GotoPatNum);
			//OnPatientSelected(FormCS.GotoPatNum,pat.GetNameLF(),pat.Email!="",pat.ChartNumber);
			//GotoModule.GotoClaim(FormCS.GotoClaimNum);
		//}

		private void ShowSecondaryClaims() {
			List<ClaimPaySplit> listClaimsAttached=(List<ClaimPaySplit>)gridAttached.Tag;
			DataTable secCPs=Claims.GetSecondaryClaims(listClaimsAttached);
			if(secCPs.Rows.Count==0) {
				return;
			}
			string message="Some of the payments have secondary claims: \r\n"
				+"Date of Service | PatNum | Patient Name";
			for(int i=0;i<secCPs.Rows.Count;i++) {
				//claimProc=secondaryClaims[i];
				message+="\r\n"+PIn.Date(secCPs.Rows[i]["ProcDate"].ToString()).ToShortDateString()
					+" | "+secCPs.Rows[i]["PatNum"].ToString()
					+" | "+Patients.GetPat(PIn.Long(secCPs.Rows[i]["PatNum"].ToString())).GetNameLF();
			}
			message+="\r\n\r\nPrint this list, then use it to review and send secondary claims.";
			using MsgBoxCopyPaste msgBox=new MsgBoxCopyPaste(message);
			msgBox.ShowDialog();
		}

		///<summary>Validates that the numbers behind the Amount and Total text boxes equate.
		///Shows a friendly exception message which will allow us engineers to click the Details label in order to get more information.
		///Customers have called in with very strange things happening with these two text box values not equating in the past (UI glitches?).</summary>
		private bool IsAmountAndTotalEqual(bool isSilent=false) {
			List<ClaimPaySplit> listClaimsAttached=(List<ClaimPaySplit>)gridAttached.Tag;
			// There are instances that the check amount when this form is launched gets modified by another instance of Open Dental and the amount when the form launched
			// does not match the current amount of the check. This can lead to a claimpayment being marked as partial (it was partial when this form was launched), but in reality
			// the payment is not partial, it was modified after this window loaded to be a full payment. Checking the current value of the payment at this stage before marking the 
			// payment as partial or not should reduce this error significantly.
			ClaimPayment curClaimPayment=ClaimPayments.GetOne(ClaimPaymentCur.ClaimPaymentNum);
			double amountRaw;
			if(curClaimPayment==null) {
				MsgBox.Show(Lan.g(this, "Payment has been deleted in another instance of the program, please close and re-open."));
				return false;
            }
			else {
				amountRaw=curClaimPayment.CheckAmt;
			}
			double totalRaw=listClaimsAttached.Sum(x => x.InsPayAmt);//textTotal is filled like this every time FillGrid is invoked.
			//We used to use textbox.Text which was displaying the above doubles utilizing .ToString("F")
			//which uses the "Default precision specifier: Defined by NumberFormatInfo.NumberDecimalDigits."
			//When the precision specifier controls the number of fractional digits in the result string, the result strings reflect numbers that are 
			//rounded away from zero (that is, using MidpointRounding.AwayFromZero).
			//Therefore, in order to preserve old behavior, we are going to apply rounding utilizing
			//NumberFormatInfo.NumberDecimalDigits along with MidpointRounding.AwayFromZero on both doubles in question (amount and total).
			//E.g. we need to preserve the old logic which would take the double 18934.1879 and display it to the user as 18934.19 (note the rounding).
			//see https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#FFormatString for more details.
			int digits=NumberFormatInfo.CurrentInfo.NumberDecimalDigits;
			double amountRounded=Math.Round(amountRaw,digits,MidpointRounding.AwayFromZero);
			double totalRounded=Math.Round(totalRaw,digits,MidpointRounding.AwayFromZero);
			if(!CompareDouble.IsEqual(amountRounded,totalRounded)) {
				if(!isSilent) {
					FriendlyException.Show("Amounts do not match.",new ApplicationException("Variables:\r\n"
						+"NumberFormatInfo.CurrentInfo.NumberDecimalDigits: "+digits+"\r\n"
						+"amountRaw: "+amountRaw+"\r\n"
						+"amountRounded: "+amountRounded+"\r\n"
						+"totalRaw: "+totalRaw+"\r\n"
						+"totalRounded: "+totalRounded+"\r\n"
						+"The above values need to equate within a small epsilon to be acceptable.  See ODExtensions.IsZero().\r\n"
						+"Math.Abs("+amountRounded+"-"+totalRounded+") = "+Math.Abs(amountRounded-totalRounded)));
				}
				return false;
			}
			return true;
		}

		private void butView_Click(object sender,EventArgs e) {
			using FormImages formI=new FormImages();
			formI.ClaimPaymentNum=ClaimPaymentCur.ClaimPaymentNum;
			formI.ShowDialog();
			if(EobAttaches.Exists(ClaimPaymentCur.ClaimPaymentNum)) {
				textEobIsScanned.Text=Lan.g(this,"Yes");
				butView.Text="View EOB";
			}
			else {
				textEobIsScanned.Text=Lan.g(this,"No");
				butView.Text="Scan EOB";
			}
			FillClaimPayment();//For customer 5769, who was getting ocassional Chinese chars in the Amount boxes.
			FillGrids();//ditto
		}
		
		private void butViewEra_Click(object sender,EventArgs e) {//Only clickable when IsFromClaim is true
			List<long> listClaimNums=_listClaimsAttached.Select(x => x.ClaimNum).Distinct().ToList();
			EtransL.ViewEra(listClaimNums);
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this insurance check?")){
				return;
			}
			if(ClaimPaymentCur.IsPartial) {//probably new
				//everyone should have permission to delete a partial payment
			}
			else {//locked
				//this delete button already disabled if no permission
			}
			try{
				ClaimPayments.Delete(ClaimPaymentCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			IsDeleting=true;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!IsAmountAndTotalEqual()) {
				return;
			}
			if(gridAttached.ListGridRows.Count==0) {
				MsgBox.Show(this,"At least one claim must be attached to this insurance payment.");
				return;
			}
			if(!PrefC.GetBool(PrefName.AllowFutureInsPayments)
				&& !PrefC.GetBool(PrefName.FutureTransDatesAllowed)
				&& ClaimPaymentCur.CheckDate.Date>MiscData.GetNowDateTime().Date)
			{
				MsgBox.Show(this,"Insurance Payment Date must not be a future date.");
				return;
			}
			//No need to prompt user about secondary claims because they already went into each Account individually.
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;			
			Close();
		}

		private void FormClaimPayBatch_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel && IsFromClaim && IsNew) {//This acts as a Cancel button. Happens when butClose or the red x is clicked.
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Delete this payment?")) {
					e.Cancel=true;
					return;
				}
				IsDeleting=true;//the actual deletion will be handled in FormClaimEdit.
			}
			if(DialogResult==DialogResult.Abort || ClaimPaymentCur==null) {//This means that the ClaimPayment was null or deleted, so there's nothing we can do.
				return;
			}
			if(IsDeleting){//This is here because the delete button could also set this.
				SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,0,"Claim Payment Deleted: "+ClaimPaymentCur.ClaimPaymentNum);
				return;
			}
			if(IsDisposed) {//This should only happen if interupted by an Auto-Logoff.
				return; //Leave the payment as partial so the user can come back and edit.
			}
			if(ClaimPaymentCur.IsPartial) {
				if(IsAmountAndTotalEqual(isSilent:true)) {
					if(gridAttached.ListGridRows.Count > 0
						&& !PrefC.GetBool(PrefName.PromptForSecondaryClaim))
					{
						//If PromptForSecondaryClaim is enabled the user was prompted to make a decision when there are secondary claims for every claim they attached.
						//No point in showing them a list of any secondary claims since they already decided how to handle them individually.
						ShowSecondaryClaims();//always continues after this dlg
					}
					ClaimPaymentCur.IsPartial=false;
					try {
						ClaimPayments.Update(ClaimPaymentCur);
					}
					catch(ApplicationException ex) {
						MessageBox.Show(ex.Message);
						e.Cancel=true;
						return;
					}
				}
			}
			else {//locked
				if(!IsAmountAndTotalEqual()) {
					//Someone edited a locked payment
					if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Amounts do not match.  Continue anyway?")) {
						e.Cancel=true;
						return;
					}
				}
			}
			if(PrefC.GetBool(PrefName.AgingCalculateOnBatchClaimReceipt)) {
				runAgingforClaims();
			}
			if(IsNew) {
				SecurityLogs.MakeLogEntry(Permissions.InsPayCreate,0,"Claim Payment: "+ClaimPaymentCur.ClaimPaymentNum);
			}
			else {
				SecurityLogs.MakeLogEntry(Permissions.InsPayEdit,0,"Claim Payment: "+ClaimPaymentCur.ClaimPaymentNum);
			}
		}
	}
}