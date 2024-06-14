using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEtrans835Edit:FormODBase {
		public string TranSetId835;
		///<summary>Must be set before the form is shown.</summary>
		public Etrans EtransCur;
		///<summary>Must be set before the form is shown.  The message text for EtransCur.</summary>
		public string MessageText835;
		private X835 _x835;
		private decimal _sumClaimInsPaid;
		private decimal _sumProvAdjAmt;
		private static FormEtrans835Edit _formEtrans835Edit=null;
		private ContextMenu _contextMenuClaimDetails=new ContextMenu();
		///<summary>List of all claimProcs associated to the attached claims.</summary>
		private List<Hx835_ShortClaimProc> _listHx835_ShortClaimProcs;
		///<summary>List of all attaches associated to the Hx835_Claim in Claims Paid grid, even if associated to another etrans ordered by DateTimeEntry.
		///Also contains SplitClaim attaches so that we can query for the associated claimProcs via the attach.ClaimNum.
		///Used to identify processed ERAs.</summary>
		private List<Etrans835Attach> _listEtrans835Attaches; 
		private long _preLoadedPrintPreviewClaimNum;
		private bool _hasPrintPreviewOnLoad=false;

		///<summary>Set printPreviewClaimNum to a print preview a specific claim on this ERA after loading.
		///If the claimNum is not found on this ERA then all claims will preview.</summary>
		public FormEtrans835Edit(long printPreviewClaimNum=0) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_preLoadedPrintPreviewClaimNum=printPreviewClaimNum;
		}

		private void FormEtrans835Edit_Load(object sender,EventArgs e) {
			if(_formEtrans835Edit!=null && !_formEtrans835Edit.IsDisposed) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Opening another ERA will close the current ERA you already have open.  Continue?")) {
					//Form already exists and user wants to keep current instance.
					TranSetId835=_formEtrans835Edit.TranSetId835;
					EtransCur=_formEtrans835Edit.EtransCur;
					MessageText835=_formEtrans835Edit.MessageText835;
				}
				_formEtrans835Edit.Close();//Always close old form and open new form, so the new copy will come to front, since BringToFront() does not always work.
			}
			_formEtrans835Edit=this;//Set the static variable to this form because we're always going to show this form even if they're viewing old information.
			List<Etrans835Attach> listEtrans835Attaches=Etrans835Attaches.GetForEtrans(EtransCur.EtransNum);
			_x835=new X835(EtransCur,MessageText835,TranSetId835,listEtrans835Attaches);
			RefreshFromDb();
			FillAll();
			Menu.MenuItemCollection menuItemCollection=new Menu.MenuItemCollection(_contextMenuClaimDetails);
			List<MenuItem> listMenuItems=new List<MenuItem>();
			listMenuItems.Add(new MenuItem(Lan.g(this,"Go to Account"),new EventHandler(gridClaimDetails_RightClickHelper)));
			listMenuItems.Add(new MenuItem(Lan.g(this,"Add Tracking Status"),new EventHandler(gridClaimDetails_RightClickHelper)));//Enabled by default
			menuItemCollection.AddRange(listMenuItems.ToArray());
			gridClaimDetails.ContextMenu=_contextMenuClaimDetails;
			gridClaimDetails.AllowSortingByColumn=true;
		}
		
		private void FormEtrans835Edit_Shown(object sender,EventArgs e) {
			if(_hasPrintPreviewOnLoad) {
				//This way the form will fully load visually before showing the print preview.
				EtransL.PrintPreview835(_x835,_preLoadedPrintPreviewClaimNum);
				SecurityLogs.MakeLogEntry(EnumPermType.InsPayCreate,0,"ERA EOB window opened for eTrans number: "+EtransCur.EtransNum+", Carrier: "+_x835.PayerName+", Amount: "+_x835.InsPaid+", Date: "+EtransCur.DateTimeTrans+".");
			}
		}

		private void RefreshFromDb() {
			_x835.RefreshAttachesAndClaimProcsFromDb(out _listEtrans835Attaches,out _listHx835_ShortClaimProcs);
		}

		private void FormEtrans835Edit_Resize(object sender,EventArgs e) {
			//This funciton is called before FormEtrans835Edit_Load() when using ShowDialog(). Therefore, x835 is null the first time FormEtrans835Edit_Resize() is called.
			if(_x835==null) {
				return;
			}
			gridProviderAdjustments.Width=butSave.Right-gridProviderAdjustments.Left;
			FillProviderAdjustmentDetails();//Because the grid columns change size depending on the form size.
			gridClaimDetails.Width=gridProviderAdjustments.Width;
			gridClaimDetails.Height=labelPaymentAmount.Top-5-gridClaimDetails.Top;
			FillClaimDetails();//Because the grid columns change size depending on the form size.
		}

		///<summary>Reads the X12 835 text in the MessageText variable and displays the information in this form.</summary>
		private void FillAll() {
			//*835 has 3 parts: Table 1 (header), Table 2 (claim level details, one CLP segment for each claim), and Table 3 (PLB: provider/check level details).
			FillHeader();//Table 1
			FillClaimDetails();//Table 2
			FillProviderAdjustmentDetails();//Table 3
			FillFooter();
			//The following concepts should each be addressed as development progresses.
			//*837 CLM01 -> 835 CLP01 (even for split claims)
			//*Advance payments (pg. 23): in PLB segment with adjustment reason code PI.  Can be yearly or monthly.  We need to find a way to pull provider level adjustments into a deposit.
			//*Bundled procs (pg. 27): have the original proc listed in SV06. Use Line Item Control Number to identify the original proc line.
			//*Predetermination (pg. 28): Identified by claim status code 25 in CLP02. Claim adjustment reason code is 101.
			//*Claim reversals (pg. 30): Identified by code 22 in CLP02. The original claim adjustment codes can be found in CAS01 to negate the original claim.
			//Use CLP07 to identify the original claim, or if different, get the original ref num from REF02 of 2040REF*F8.
			//*Interest and Prompt Payment Discounts (pg. 31): Located in AMT segments with qualifiers I (interest) and D8 (discount). Found at claim and provider/check level.
			//Not part of AR, but part of deposit. Handle this situation by using claimprocs with 2 new status, one for interest and one for discount? Would allow reports, deposits, and claim checks to work as is.
			//*Capitation and related payments or adjustments (pg. 34 & 52): Not many of our customers use capitation, so this will probably be our last concern.
			//*Claim splits (pg. 36): MIA or MOA segments will exist to indicate the claim was split.
			//*Service Line Splits (pg. 42): LQ segment with LQ01=HE and LQ02=N123 indicate the procedure was split.
		}

		///<summary>Reads the X12 835 text in the MessageText variable and displays the information from Table 1 (Header).</summary>
		private void FillHeader() {
			//Payer information
			textPayerName.Text=_x835.PayerName;
			textPayerID.Text=_x835.PayerId;
			textPayerAddress1.Text=_x835.PayerAddress;
			textPayerCity.Text=_x835.PayerCity;
			textPayerState.Text=_x835.PayerState;
			textPayerZip.Text=_x835.PayerZip;
			textPayerContactInfo.Text=_x835.GetPayerContactInfo();
			//Payee information
			textPayeeName.Text=_x835.PayeeName;
			labelPayeeIdType.Text=Lan.g(this,"Payee")+" "+_x835.PayeeIdType;
			textPayeeID.Text=_x835.PayeeId;
			//Payment information
			textTransHandlingDesc.Text=_x835.TransactionHandlingDescript;
			textPaymentMethod.Text=_x835.PayMethodDescript;
			if(_x835.IsCredit) {
				textPaymentAmount.Text=_x835.InsPaid.ToString("f2");
			}
			else {
				textPaymentAmount.Text="-"+_x835.InsPaid.ToString("f2");
			}
			textAcctNumEndingIn.Text=_x835.AccountNumReceiving;
			if(_x835.DateEffective.Year>1880) {
				textDateEffective.Text=_x835.DateEffective.ToShortDateString();
			}
			textCheckNumOrRefNum.Text=_x835.TransRefNum;
			textNote.Text=EtransCur.Note;
		}

		///<summary>Reads the X12 835 text in the MessageText variable and displays the information from Table 2 (Detail).</summary>
		private void FillClaimDetails() {
			gridClaimDetails.BeginUpdate();
			gridClaimDetails.Columns.Clear();
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"Recd"),32,HorizontalAlignment.Center));
			GridColumn col=new GridColumn(Lan.g(this,"Patient"),70,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridClaimDetails.Columns.Add(col);
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"DateService"),80,HorizontalAlignment.Center));
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"Claim\r\nIdentifier"),50,HorizontalAlignment.Left));
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"Payor\r\nControl#"),56,HorizontalAlignment.Center));//Payer Claim Control Number (CLP07)
			col=new GridColumn(Lan.g(this,"Status"),70,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridClaimDetails.Columns.Add(col);//Claim Status Code Description (CLP02)
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"ClaimFee"),70,HorizontalAlignment.Right));//Total Claim Charge Amount (CLP03)
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"InsPaid"),70,HorizontalAlignment.Right));//Claim Payment Amount (CLP04)
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"PatPort"),70,HorizontalAlignment.Right));//Patient Portion
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"Deduct"),70,HorizontalAlignment.Right));//Deductible
			gridClaimDetails.Columns.Add(new GridColumn(Lan.g(this,"Writeoff"),70,HorizontalAlignment.Right));//Writeoff
			gridClaimDetails.ListGridRows.Clear();
			_sumClaimInsPaid=0;
			List<int> listMatchedRowIndices=new List<int>();
			List<Hx835_Claim> listHx835_ClaimsPaid=_x835.ListClaimsPaid;
			for(int i=0;i<listHx835_ClaimsPaid.Count;i++) {
				Hx835_Claim hx835_Claim=listHx835_ClaimsPaid[i];
				GridRow row=new GridRow();
				List<GridRow> listRows=new List<GridRow>();
				listRows.Add(row);
				List<Hx835_Claim> listHx835_Claims=new List<Hx835_Claim>();
				listHx835_Claims.Add(hx835_Claim);
				SetClaimDetailRows(listRows, listHx835_Claims);
				_sumClaimInsPaid+=hx835_Claim.InsPaid;
				gridClaimDetails.ListGridRows.Add(row);
				if(!_hasPrintPreviewOnLoad//Only show print prevew once.
					&& hx835_Claim.ClaimNum!=0 && hx835_Claim.ClaimNum==_preLoadedPrintPreviewClaimNum)//Specific claim num provided for auto selecting and print preview.
				{
					listMatchedRowIndices.Add(i);
				}
			}
			gridClaimDetails.EndUpdate();
			if(listMatchedRowIndices.Count>0) {
				_hasPrintPreviewOnLoad=true;
				listMatchedRowIndices.ForEach(x => gridClaimDetails.SetSelected(x,setValue:true));
			}
		}
		
		///<summary>ListRows and ListClaimsPaid must be 1:1 and in the same order.</summary>
		private void SetClaimDetailRows(List<GridRow> listRows,List<Hx835_Claim> listHx835_ClaimsPaid,bool isRefreshNeeded=false) {
			if(isRefreshNeeded) {
				RefreshFromDb();
			}
			for(int i=0;i<listRows.Count;i++) {
				UI.GridRow row=listRows[i];
				Hx835_Claim hx835_Claim=listHx835_ClaimsPaid[i];
				row.Tag=hx835_Claim;
				row.Cells.Clear();
				string claimStatus="";
				if(hx835_Claim.IsProcessed(_listHx835_ShortClaimProcs,_listEtrans835Attaches)) {
					claimStatus="X";
				}
				else if(hx835_Claim.IsAttachedToClaim && hx835_Claim.ClaimNum==0) {
					claimStatus="N/A";//User detached claim manually.
				}
				row.Cells.Add(claimStatus);
				row.Cells.Add(new UI.GridCell(hx835_Claim.PatientName.ToString()));//Patient
				string strDateService=hx835_Claim.DateServiceStart.ToShortDateString();
				if(hx835_Claim.DateServiceEnd>hx835_Claim.DateServiceStart) {
					strDateService+=" to "+hx835_Claim.DateServiceEnd.ToShortDateString();
				}
				row.Cells.Add(new UI.GridCell(strDateService));//DateService
				row.Cells.Add(new UI.GridCell(hx835_Claim.ClaimTrackingNumber));//Claim Identfier
				row.Cells.Add(new UI.GridCell(hx835_Claim.PayerControlNumber));//PayorControlNum
				row.Cells.Add(new UI.GridCell(hx835_Claim.StatusCodeDescript));//Status
				row.Cells.Add(new UI.GridCell(hx835_Claim.ClaimFee.ToString("f2")));//ClaimFee
				row.Cells.Add(new UI.GridCell(hx835_Claim.InsPaid.ToString("f2")));//InsPaid
				row.Cells.Add(new UI.GridCell(hx835_Claim.PatientPortionAmt.ToString("f2")));//PatPort
				row.Cells.Add(new UI.GridCell(hx835_Claim.PatientDeductAmt.ToString("f2")));//Deduct
				row.Cells.Add(new UI.GridCell(hx835_Claim.WriteoffAmt.ToString("f2")));//Writeoff
			}
		}

		/// <summary>Runs aging for all families for patient's on claims attached to payment</summary>
		private void RunAgingForClaims() {
			List<long> listPatNumsOnClaims = Claims.GetClaimsFromClaimNums(_listEtrans835Attaches.Select(x => x.ClaimNum).ToList()).Select(x => x.PatNum).ToList();
			List<long> listGuarantors = Patients.GetGuarantorsForPatNums(listPatNumsOnClaims);
			//looping through guarNums 1 at a time to avoid using the famaging table, otherwise we would need to check/set the AgingBeginDateTime pref and signal other instances
			for(int i=0;i<listGuarantors.Count;i++) {
				Ledgers.ComputeAging(listGuarantors[i],DateTime.Today);
			}
		}

		///<summary>Reads the X12 835 text in the MessageText variable and displays the information from Table 3 (Summary).</summary>
		private void FillProviderAdjustmentDetails() {
			if(_x835.ListProvAdjustments.Count==0) {
				gridProviderAdjustments.Title="Provider Adjustments (None Reported)";
			}
			else {
				gridProviderAdjustments.Title="Provider Adjustments";
			}
			const int colWidthNPI=88;
			const int colWidthFiscalPeriod=80;
			const int colWidthReasonCode=90;
			const int colWidthRefIdent=80;
			const int colWidthAmount=80;
			int widthCol=gridProviderAdjustments.Width-colWidthNPI-colWidthFiscalPeriod-colWidthReasonCode-colWidthRefIdent-colWidthAmount;
			gridProviderAdjustments.BeginUpdate();
			gridProviderAdjustments.Columns.Clear();
			gridProviderAdjustments.Columns.Add(new GridColumn("NPI",colWidthNPI,HorizontalAlignment.Center));
			gridProviderAdjustments.Columns.Add(new GridColumn("FiscalPeriod",colWidthFiscalPeriod,HorizontalAlignment.Center));
			gridProviderAdjustments.Columns.Add(new GridColumn("Reason",widthCol,HorizontalAlignment.Left));
			gridProviderAdjustments.Columns.Add(new GridColumn("ReasonCode",colWidthReasonCode,HorizontalAlignment.Center));
			gridProviderAdjustments.Columns.Add(new GridColumn("RefIdent",colWidthRefIdent,HorizontalAlignment.Center));			
			gridProviderAdjustments.Columns.Add(new GridColumn("AdjAmt",colWidthAmount,HorizontalAlignment.Right));
			gridProviderAdjustments.EndUpdate();
			gridProviderAdjustments.BeginUpdate();
			gridProviderAdjustments.ListGridRows.Clear();
			_sumProvAdjAmt=0;
			for(int i=0;i<_x835.ListProvAdjustments.Count;i++) {
				Hx835_ProvAdj hx835_ProvAdj=_x835.ListProvAdjustments[i];
				GridRow row=new GridRow();
				row.Tag=hx835_ProvAdj;
				row.Cells.Add(new GridCell(hx835_ProvAdj.Npi));//NPI
				row.Cells.Add(new GridCell(hx835_ProvAdj.DateFiscalPeriod.ToShortDateString()));//FiscalPeriod
				row.Cells.Add(new GridCell(hx835_ProvAdj.ReasonCodeDescript));//Reason
				row.Cells.Add(new GridCell(hx835_ProvAdj.ReasonCode));//ReasonCode
				row.Cells.Add(new GridCell(hx835_ProvAdj.RefIdentification));//RefIdent
				row.Cells.Add(new GridCell(hx835_ProvAdj.AdjAmt.ToString("f2")));//AdjAmt
				_sumProvAdjAmt+=hx835_ProvAdj.AdjAmt;
				gridProviderAdjustments.ListGridRows.Add(row);
			}
			gridProviderAdjustments.EndUpdate();
		}

		private void FillFooter() {
			textClaimInsPaidSum.Text=_sumClaimInsPaid.ToString("f2");
			textProjAdjAmtSum.Text=_sumProvAdjAmt.ToString("f2");
			textPayAmountCalc.Text=(_sumClaimInsPaid-_sumProvAdjAmt).ToString("f2");
		}

		private void butRawMessage_Click(object sender,EventArgs e) {
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(MessageText835);
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridProviderAdjustments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_ProvAdj hx835_ProvAdj=(Hx835_ProvAdj)gridProviderAdjustments.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(
				hx835_ProvAdj.Npi+"\r\n"
				+hx835_ProvAdj.DateFiscalPeriod.ToShortDateString()+"\r\n"
				+hx835_ProvAdj.ReasonCode+" "+hx835_ProvAdj.ReasonCodeDescript+"\r\n"
				+hx835_ProvAdj.RefIdentification+"\r\n"
				+hx835_ProvAdj.AdjAmt.ToString("f2"));
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridClaimDetails_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Claim hx835_Claim=(Hx835_Claim)gridClaimDetails.ListGridRows[e.Row].Tag;
			Claim claim=hx835_Claim.GetClaimFromDb();
			bool isAttachNeeded=(!hx835_Claim.IsAttachedToClaim);
			if(claim==null) {//Not found in db.
				claim=ClaimSelectionHelper(hx835_Claim);
				if(claim!=null) {//A claim was selected.
					isAttachNeeded=true;//Hard attach selected claim to db claim, since user manually picked matching claim.
				}
			}
			if(claim==null) {//Claim not found and user did not select one.
				return;
			}
			//From this point on claim is not null.
			Patient patient=Patients.GetPat(claim.PatNum);
			bool doesPatientNameMatch=hx835_Claim.DoesPatientNameMatch(patient);
			if(!doesPatientNameMatch) {
				MsgBox.Show(this,"A matching claim was found, but the patient name the carrier sent does not match the patient on the claim.");
			}
			bool isReadOnly=true;
			Etrans835Attaches.CreateForClaim(_x835,hx835_Claim,claim.ClaimNum,isAttachNeeded,_listEtrans835Attaches);
			if(hx835_Claim.IsSplitClaim) {
				//Sync ClaimNum for all split claims in the same group, based on user selection.
				List<Hx835_Claim> listHx835_ClaimsOtherNotDetachedSplit=hx835_Claim.GetOtherNotDetachedSplitClaims();
				for(int i=0;i<listHx835_ClaimsOtherNotDetachedSplit.Count;i++) {
					Etrans835Attaches.CreateForClaim(_x835,listHx835_ClaimsOtherNotDetachedSplit[i],claim.ClaimNum,isAttachNeeded,_listEtrans835Attaches);
				}
			}
			if(hx835_Claim.IsProcessed(_listHx835_ShortClaimProcs,_listEtrans835Attaches)) {
				//If the claim is already received, then we do not allow the user to enter payments.
				//The user can edit the claim to change the status from received if they wish to enter the payments again.
				if(Security.IsAuthorized(EnumPermType.ClaimView)) {
					Family family=Patients.GetFamily(claim.PatNum);
					using FormClaimEdit formClaimEdit=new FormClaimEdit(claim,patient,family);
					formClaimEdit.ShowDialog();//Modal, because the user could edit information in this window.
				}
				isReadOnly=false;
			}
			else if(Security.IsAuthorized(EnumPermType.InsPayCreate)) {//Claim found and is not received.  Date not checked here, but it will be checked when actually creating the check.
				List<ClaimProc> listClaimProcsForClaim=ClaimProcs.RefreshForClaim(claim.ClaimNum);
				EtransL.ImportEraClaimData(_x835,hx835_Claim,claim,patient,listClaimProcsForClaim);
				RefreshFromDb();//ClaimProcs could have been split, need to refresh both claimProc list and attaches.
				isReadOnly=false;
			}
			if(isReadOnly) {
				FormEtrans835ClaimEdit formEtrans835ClaimEdit=new FormEtrans835ClaimEdit(_x835,hx835_Claim);
				formEtrans835ClaimEdit.Show(this);//This window is just used to display information.
			}
			if(!gridClaimDetails.IsDisposed) {//Not sure how the grid is sometimes disposed, perhaps because of it being non-modal.
				//Refresh the claim detail row in case something changed above.
				gridClaimDetails.BeginUpdate();
				List<GridRow> listRows=new List<GridRow>();
				listRows.Add(gridClaimDetails.ListGridRows[e.Row]);//Row associated to claimPaid
				if(hx835_Claim.IsSplitClaim) {//Need to update multiple rows.
					List<Hx835_Claim> listHx835_ClaimsSplit=hx835_Claim.GetOtherNotDetachedSplitClaims();
					List<GridRow> listRowsAdditional=gridClaimDetails.ListGridRows
						.Where(x =>
							x!=gridClaimDetails.ListGridRows[e.Row]
							&& listHx835_ClaimsSplit.Contains((Hx835_Claim)x.Tag)
						).ToList();
					listRows.AddRange(listRowsAdditional);
				}
				SetClaimDetailRows(listRows,listRows.Select(x => (Hx835_Claim)x.Tag ).ToList(),isRefreshNeeded:true);
				gridClaimDetails.EndUpdate();
			}
		}

		///<summary>If given claim!=null, attempts to open the patient select and claim select windows.
		///Sets isAttachNeeded to true if user went through full patient and claim selection logic, claim was not null when provided.
		///Returns false if user does not select a claim.</summary>
		private Claim ClaimSelectionHelper(Hx835_Claim hx835_ClaimPaid) {
			PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(doLimit:false,hx835_ClaimPaid.PatientName.Lname,hx835_ClaimPaid.PatientName.Fname,"","",hideInactive:false,
				"","","","","",0,guarOnly:false,showArchived:false,DateTime.MinValue,0,"","","","","","","");
			DataTable tablePts=Patients.GetPtDataTable(ptTableSearchParams);//Mimics FormPatientSelect.cs
			long selectedPatNum=0;
			if(tablePts.Rows.Count==1) {
				selectedPatNum=PIn.Long(tablePts.Rows[0]["PatNum"].ToString());
			}
			using FormEtrans835ClaimSelect formEtrans835ClaimSelect=new FormEtrans835ClaimSelect(selectedPatNum,hx835_ClaimPaid);
			formEtrans835ClaimSelect.ShowDialog();
			if(formEtrans835ClaimSelect.DialogResult!=DialogResult.OK) {
				return null;
			}
			Claim claim=formEtrans835ClaimSelect.ClaimSelected; //Set claim so below we can act if a claim was already linked.
			if(!String.IsNullOrEmpty(hx835_ClaimPaid.ClaimTrackingNumber) && hx835_ClaimPaid.ClaimTrackingNumber!="0") {//Claim was not printed, it is an eclaim.
				claim.ClaimIdentifier=hx835_ClaimPaid.ClaimTrackingNumber;//Already checked DOS and ClaimFee, update claim identifier to link claims.
				Claims.UpdateClaimIdentifier(claim.ClaimNum,claim.ClaimIdentifier);//Update DB
			}
			return claim;
		}

		///<summary>Click method used by all gridClaimDetails right click options.</summary>
		private void gridClaimDetails_RightClickHelper(object sender,EventArgs e) {
			int index=gridClaimDetails.GetSelectedIndex();
			if(index==-1) {//Should not happen, menu item is only enabled when exactly 1 row is selected.
				return;
			}
			Hx835_Claim hx835_Claim=(Hx835_Claim)gridClaimDetails.ListGridRows[index].Tag;
			int idxMenuItem=((MenuItem)sender).Index;//sender is the selected menuItem
#region Go To Account
			if(idxMenuItem==0) {
				EtransL.GoToAccountForHx835_Claim(hx835_Claim);
			}
#endregion
#region Add Tracking Status
			else if(idxMenuItem==1) {				
				Claim claim=hx835_Claim.GetClaimFromDb();
				bool isAttachNeeded=(!hx835_Claim.IsAttachedToClaim);
				if(claim==null) {//Not found in db.
					claim=ClaimSelectionHelper(hx835_Claim);
					if(claim!=null) {//A claim was selected.
						isAttachNeeded=true;//Hard attach selected claim to db claim, since user manually picked matching claim.
					}
				}
				if(claim==null) {//Claim not found and user did not select one.
					return;
				}
				Etrans835Attaches.CreateForClaim(_x835,hx835_Claim,claim.ClaimNum,isAttachNeeded,_listEtrans835Attaches);
				if(FormClaimEdit.AddClaimCustomTracking(claim,hx835_Claim.GetRemarks())) {//A tracking status was chosen and the tracking status contains an error code.
					List<int> listIndices=new List<int>();
					listIndices.Add(index);
					DetachClaimHelper(listIndices);//Detach claim from ERA for convenience.
				}
				FillClaimDetails();
			}
#endregion
		}

		private void butDetachClaim_Click(object sender,EventArgs e) {
			if(gridClaimDetails.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a claim from the claims paid grid and try again.");
				return;
			}
			List<Hx835_Claim> listHx835_ClaimsSelected=gridClaimDetails.SelectedTags<Hx835_Claim>();//See SetClaimDetailRows(...)
			listHx835_ClaimsSelected.RemoveAll(x => !x.IsAttachedToClaim || x.ClaimNum==0);//Limit to era claims that were attached to internal claim.
			List<Claim> listClaimsAttached=_x835.RefreshClaims();
			List<Hx835_Claim> listHx835_ClaimsReceived=listHx835_ClaimsSelected.FindAll(x => listClaimsAttached.Any(y => y.ClaimNum==x.ClaimNum && y.ClaimStatus=="R"));
			StringBuilder stringBuilderWarnings=new StringBuilder();
			if(listHx835_ClaimsReceived.Count>0) {
				stringBuilderWarnings.AppendLine(Lan.g(this,"You have selected some claims which are already received."));
				stringBuilderWarnings.AppendLine(Lan.g(this,"Detaching these will not reverse any payment information."));
			}
			if(gridClaimDetails.SelectedIndices.Length>1) {
				stringBuilderWarnings.AppendLine(Lan.g(this,"All selected claims will be immediately detached from this ERA even if you click Cancel when you leave the ERA window."));
			}
			if(stringBuilderWarnings.Length>0) {
				stringBuilderWarnings.AppendLine(Lan.g(this,"Click OK to continue, or click Cancel to leave claims attached."));
				if(MessageBox.Show(this,stringBuilderWarnings.ToString(),"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			DetachClaimHelper(gridClaimDetails.SelectedIndices.ToList());
		}

		private void DetachClaimHelper(List<int> listGridIndices) {
			gridClaimDetails.BeginUpdate();
			for(int i=0;i<listGridIndices.Count;i++) {
				GridRow row=gridClaimDetails.ListGridRows[listGridIndices[i]];
				Hx835_Claim hx835_Claim=(Hx835_Claim)row.Tag;
				Etrans835Attaches.DetachEraClaim(hx835_Claim);
				List<GridRow> listRows=new List<GridRow>();
				listRows.Add(row);
				List<Hx835_Claim> listHx835_Claims=new List<Hx835_Claim>();
				listHx835_Claims.Add(hx835_Claim);
				SetClaimDetailRows(listRows,listHx835_Claims,isRefreshNeeded:true);
			}
			gridClaimDetails.EndUpdate();
		}

		private void buttonFindClaimMatch_Click(object sender,EventArgs e) {
			List<Hx835_Claim> listHx835_ClaimsDetached=gridClaimDetails
				.SelectedIndices
				.Select(x => (Hx835_Claim)gridClaimDetails.ListGridRows[x].Tag)
				.ToList();
			if(listHx835_ClaimsDetached.Any(x => x.IsAttachedToClaim && x.ClaimNum!=0)) {
				MsgBox.Show(this,"Only manually detached rows can be selected.");
				return;
			}
			if(listHx835_ClaimsDetached.Count==0) {
				MsgBox.Show(this,"Select at least one manually detached row and try again.");
				return;
			}
			List<X12ClaimMatch> listX12ClaimMatches=_x835.GetClaimMatches(listHx835_ClaimsDetached);
			List<long> listClaimNums=Claims.GetClaimFromX12(listX12ClaimMatches);//Can return null.
			if(listClaimNums!=null) {
				_x835.SetClaimNumsForUnattached(listClaimNums,listHx835_ClaimsDetached);//List are 1:1 and in same order
				for(int i=0;i<listHx835_ClaimsDetached.Count;i++) {
					Etrans835Attaches.CreateForClaim(_x835,listHx835_ClaimsDetached[i],listClaimNums[i],isNewAttachNeeded:true,_listEtrans835Attaches);
				}
				List<GridRow> listRows=gridClaimDetails.ListGridRows
					.Where(x =>
						listHx835_ClaimsDetached.Contains((Hx835_Claim)x.Tag)
					).ToList();
				gridClaimDetails.BeginUpdate();
				SetClaimDetailRows(listRows,listRows.Select(x => ((Hx835_Claim)x.Tag)).ToList(),isRefreshNeeded:true);
				gridClaimDetails.EndUpdate();
			}
			MsgBox.Show(this,"Done");
		}

		private void butClaimDetails_Click(object sender,EventArgs e) {
			if(gridClaimDetails.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Choose a claim paid before viewing details.");
				return;
			}
			Hx835_Claim hx835_Claim=(Hx835_Claim)gridClaimDetails.ListGridRows[gridClaimDetails.SelectedIndices[0]].Tag;
			FormEtrans835ClaimEdit formEtrans835ClaimEdit=new FormEtrans835ClaimEdit(_x835,hx835_Claim);
			formEtrans835ClaimEdit.Show(this);//This window is just used to display information.
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrintClickHelper(isPreviewMode:false);
		}
		
		private void PrintClickHelper(bool isPreviewMode) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.ERA));
			SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue=_x835;//Required param
			SheetFiller.FillFields(sheet);
			SheetPrinting.Print(sheet,isPreviewMode:isPreviewMode);
		}

		private void butPreview_Click(object sender,EventArgs e) {
			PrintClickHelper(isPreviewMode:true);
		}

		///<summary>Attempt to automatically process all claims on the ERA and finalize the payment.</summary>
		private void butAutoProcess_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.InsPayCreate,MiscData.GetNowDateTime().Date)) {
				return;//The user does not have permission to create an insurance payment for today's date.
			}
			bool doAutoDeposit=PrefC.GetBool(PrefName.ShowAutoDeposit);
			if(doAutoDeposit && !Security.IsAuthorized(EnumPermType.DepositSlips,MiscData.GetNowDateTime().Date)) {
				return;//An auto deposit would be created and the user does not have permission to create deposits.
			}
			List<Hx835_ShortClaim> listHx835_ShortClaims=_x835.RefreshClaims().Select(x => new Hx835_ShortClaim(x)).ToList();
			X835Status x835Status=_x835.GetStatus(_x835.GetClaimDataList(listHx835_ShortClaims),_listHx835_ShortClaimProcs,_listEtrans835Attaches);
			if(!x835Status.In(X835Status.Unprocessed,X835Status.Partial,X835Status.NotFinalized)) {
				MsgBox.Show(this,"Only ERAs with a status of Unprocessed, Partial, or NotFinalized can be processed automatically.");
				return;
			}
			SecurityLogs.MakeLogEntry(EnumPermType.InsPayCreate,0,"ERA auto-process button clicked.");
			Cursor=Cursors.WaitCursor;
			EraAutomationResult eraAutomationResult=eraAutomationResult=Etranss.TryAutoProcessEraEob(_x835,_listEtrans835Attaches,isFullyAutomatic:false);
			RefreshFromDb();
			FillClaimDetails();
			List<EraAutomationResult> listEraAutomationResults=new List<EraAutomationResult>();
			listEraAutomationResults.Add(eraAutomationResult);
			string automationResultMessage=EraAutomationResult.CreateMessage(listEraAutomationResults,isForSingleEra:true);
			textNote.Text=EraAutomationResult.CreateEtransNote(eraAutomationResult.Status,textNote.Text,eraAutomationResult.CountProcessedClaimsWithNameMismatch);
			EtransCur.Note=textNote.Text;
			Etrans835 etrans835=Etrans835s.GetByEtransNums(EtransCur.EtransNum).FirstOrDefault();
			if(etrans835==null) {
				etrans835=new Etrans835();
				etrans835.EtransNum=EtransCur.EtransNum;
			}
			if(eraAutomationResult.Status==X835Status.Finalized) {
				Etrans835s.Upsert(etrans835,_x835,newAutoProcessedStatus:X835AutoProcessed.SemiAutoComplete);
				MessageBox.Show(automationResultMessage);
				DialogResult=DialogResult.OK;
				Close();//Update of EtransCur happens in FormClosing
			}
			else {
				Etrans835s.Upsert(etrans835,_x835,newAutoProcessedStatus:X835AutoProcessed.SemiAutoIncomplete);
				Etrans etransOld=EtransCur.Copy();
				Etranss.Update(EtransCur,etransOld);//Update so that note gets saved even if cancel is clicked.
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(automationResultMessage);
				msgBoxCopyPaste.Show();
			}
			Cursor=Cursors.Default;
		}

		///<summary>Since ERAs are only used in the United States, we do not need to translate any text shown to the user.</summary>
		private void butBatch_Click(object sender,EventArgs e) {
			if(gridClaimDetails.ListGridRows.Count==0) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!EtransL.FinalizeBatchPayment(_x835)) {
				Cursor=Cursors.Default;
				return;
			}
			EtransCur.Note=textNote.Text;//update happens in FormClosing
			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!EtransL.TryDeleteEra(_x835,_x835.RefreshClaims().Select(x => new Hx835_ShortClaim(x)).ToList(),_listHx835_ShortClaimProcs,_listEtrans835Attaches)) {
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butSave_Click(object sender,EventArgs e) {
			EtransCur.Note=textNote.Text;//update happens in FormClosing
			DialogResult=DialogResult.OK;
			if(PrefC.GetBool(PrefName.AgingCalculateOnBatchClaimReceipt)) {
				RunAgingForClaims();
			}
			Close();
		}

		private void FormEtrans835Edit_FormClosing(object sender,FormClosingEventArgs e) {
			//This originally only happened on OK click, but other buttons also close the form after claims are processed.
			bool isReceived=true;
			for(int i=0;i<gridClaimDetails.ListGridRows.Count;i++) {
				if(gridClaimDetails.ListGridRows[i].Cells[0].Text=="") {//Will count as Recieved even if some claims are manually detached from the ERA ("Recd" column shows "N/A")
					isReceived=false;
					break;
				}
			}
			if(isReceived) {
				EtransCur.AckCode="Recd";
			}
			else {
				EtransCur.AckCode="";
			}
			Etranss.Update(EtransCur);
			Etrans835 etrans835=Etrans835s.GetByEtransNums(EtransCur.EtransNum).FirstOrDefault();
			if(etrans835==null) {
				etrans835=new Etrans835();
				etrans835.EtransNum=EtransCur.EtransNum;
			}
			Etrans835s.Upsert(etrans835,_x835);
			_formEtrans835Edit=null;
			SecurityLogs.MakeLogEntry(EnumPermType.InsPayCreate,0,"ERA edit window closed for eTrans number: "+EtransCur.EtransNum+", Carrier: "+_x835.PayerName+", Amount: "+_x835.InsPaid+", Date: "+EtransCur.DateTimeTrans+".");
		}
		
	}
}