using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
		private decimal _claimInsPaidSum;
		private decimal _provAdjAmtSum;
		private static FormEtrans835Edit _form835=null;
		private ContextMenu gridClaimDetailsMenu=new ContextMenu();
		///<summary>List of all claimProcs associated to the attached claims.</summary>
		private List<Hx835_ShortClaimProc> _listClaimProcs;
		///<summary>List of all attaches associated to the Hx835_Claim in Claims Paid grid, even if associated to another etrans ordered by DateTimeEntry.
		///Also contains SplitClaim attaches so that we can query for the associated claimProcs via the attach.ClaimNum.
		///Used to identify processed ERAs.</summary>
		private List<Etrans835Attach> _listAttaches; 
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
			if(_form835!=null && !_form835.IsDisposed) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Opening another ERA will close the current ERA you already have open.  Continue?")) {
					//Form already exists and user wants to keep current instance.
					TranSetId835=_form835.TranSetId835;
					EtransCur=_form835.EtransCur;
					MessageText835=_form835.MessageText835;
				}
				_form835.Close();//Always close old form and open new form, so the new copy will come to front, since BringToFront() does not always work.
			}
			_form835=this;//Set the static variable to this form because we're always going to show this form even if they're viewing old information.
			List <Etrans835Attach> listAttached=Etrans835Attaches.GetForEtrans(EtransCur.EtransNum);
			_x835=new X835(EtransCur,MessageText835,TranSetId835,listAttached);
			RefreshFromDb();
			FillAll();
			Menu.MenuItemCollection menuItemCollection=new Menu.MenuItemCollection(gridClaimDetailsMenu);
			List<MenuItem> listMenuItems=new List<MenuItem>();
			listMenuItems.Add(new MenuItem(Lan.g(this,"Go to Account"),new EventHandler(gridClaimDetails_RightClickHelper)));
			listMenuItems.Add(new MenuItem(Lan.g(this,"Add Tracking Status"),new EventHandler(gridClaimDetails_RightClickHelper)));//Enabled by default
			menuItemCollection.AddRange(listMenuItems.ToArray());
			gridClaimDetails.ContextMenu=gridClaimDetailsMenu;
			gridClaimDetails.AllowSortingByColumn=true;
		}
		
		private void FormEtrans835Edit_Shown(object sender,EventArgs e) {
			if(_hasPrintPreviewOnLoad) {
				//This way the form will fully load visually before showing the print preview.
				EtransL.PrintPreview835(_x835,_preLoadedPrintPreviewClaimNum);
			}
		}

		private void RefreshFromDb() {
			List<long> listClaimNums=_x835.ListClaimsPaid.Select(x => x.ClaimNum).Where(x => x!=0).ToList();
			_listAttaches=Etrans835Attaches.GetForEtransNumOrClaimNums(false,EtransCur.EtransNum,listClaimNums.ToArray());//Includes manually detached and split attaches.
			//We have to add additional claimNums from _listAttaches to account for claims split from their original ERA.
			listClaimNums.AddRange(_listAttaches.Where(x => x.ClaimNum!=0).Select(x => x.ClaimNum).Distinct());
			_listClaimProcs=Hx835_ShortClaimProc.RefreshForClaims(listClaimNums);
		}

		private void FormEtrans835Edit_Resize(object sender,EventArgs e) {
			//This funciton is called before FormEtrans835Edit_Load() when using ShowDialog(). Therefore, x835 is null the first time FormEtrans835Edit_Resize() is called.
			if(_x835==null) {
				return;
			}
			gridProviderAdjustments.Width=butOK.Right-gridProviderAdjustments.Left;
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
			textPayerContactInfo.Text=_x835.PayerContactInfo;
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
			gridClaimDetails.ListGridColumns.Clear();
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Recd"),32,HorizontalAlignment.Center));
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient"),70,HorizontalAlignment.Left){ IsWidthDynamic=true });
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"DateService"),80,HorizontalAlignment.Center));
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Claim\r\nIdentifier"),50,HorizontalAlignment.Left));
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Payor\r\nControl#"),56,HorizontalAlignment.Center));//Payer Claim Control Number (CLP07)
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Status"),70,HorizontalAlignment.Left){ IsWidthDynamic=true });//Claim Status Code Description (CLP02)
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"ClaimFee"),70,HorizontalAlignment.Right));//Total Claim Charge Amount (CLP03)
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"InsPaid"),70,HorizontalAlignment.Right));//Claim Payment Amount (CLP04)
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"PatPort"),70,HorizontalAlignment.Right));//Patient Portion
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Deduct"),70,HorizontalAlignment.Right));//Deductible
			gridClaimDetails.ListGridColumns.Add(new GridColumn(Lan.g(this,"Writeoff"),70,HorizontalAlignment.Right));//Writeoff
			gridClaimDetails.ListGridRows.Clear();
			_claimInsPaidSum=0;
			List<int> listMatchedRows=new List<int>();
			List<Hx835_Claim> listClaimsPaid=_x835.ListClaimsPaid;
			for(int i=0;i<listClaimsPaid.Count;i++) {
				Hx835_Claim claimPaid=listClaimsPaid[i];
				GridRow row=new GridRow();
				SetClaimDetailRows(new List<GridRow>() { row },new List<Hx835_Claim>() { claimPaid });
				_claimInsPaidSum+=claimPaid.InsPaid;
				gridClaimDetails.ListGridRows.Add(row);
				if(!_hasPrintPreviewOnLoad//Only show print prevew once.
					&& claimPaid.ClaimNum!=0 && claimPaid.ClaimNum==_preLoadedPrintPreviewClaimNum)//Specific claim num provided for auto selecting and print preview.
				{
					listMatchedRows.Add(i);
				}
			}
			gridClaimDetails.EndUpdate();
			if(listMatchedRows.Count>0) {
				_hasPrintPreviewOnLoad=true;
				listMatchedRows.ForEach(x => gridClaimDetails.SetSelected(x,true));
			}
		}
		
		///<summary>ListRows and ListClaimsPaid must be 1:1 and in the same order.</summary>
		private void SetClaimDetailRows(List<GridRow> listRows,List<Hx835_Claim> listClaimsPaid,bool isRefreshNeeded=false) {
			if(isRefreshNeeded) {
				RefreshFromDb();
			}
			for(int i=0;i<listRows.Count;i++) {
				UI.GridRow row=listRows[i];
				Hx835_Claim claimPaid=listClaimsPaid[i];
				row.Tag=claimPaid;
				row.Cells.Clear();
				string claimStatus="";
				if(claimPaid.IsProcessed(_listClaimProcs,_listAttaches)) {
					claimStatus="X";
				}
				else if(claimPaid.IsAttachedToClaim && claimPaid.ClaimNum==0) {
					claimStatus="N/A";//User detached claim manually.
				}
				row.Cells.Add(claimStatus);
				row.Cells.Add(new UI.GridCell(claimPaid.PatientName.ToString()));//Patient
				string strDateService=claimPaid.DateServiceStart.ToShortDateString();
				if(claimPaid.DateServiceEnd>claimPaid.DateServiceStart) {
					strDateService+=" to "+claimPaid.DateServiceEnd.ToShortDateString();
				}
				row.Cells.Add(new UI.GridCell(strDateService));//DateService
				row.Cells.Add(new UI.GridCell(claimPaid.ClaimTrackingNumber));//Claim Identfier
				row.Cells.Add(new UI.GridCell(claimPaid.PayerControlNumber));//PayorControlNum
				row.Cells.Add(new UI.GridCell(claimPaid.StatusCodeDescript));//Status
				row.Cells.Add(new UI.GridCell(claimPaid.ClaimFee.ToString("f2")));//ClaimFee
				row.Cells.Add(new UI.GridCell(claimPaid.InsPaid.ToString("f2")));//InsPaid
				row.Cells.Add(new UI.GridCell(claimPaid.PatientPortionAmt.ToString("f2")));//PatPort
				row.Cells.Add(new UI.GridCell(claimPaid.PatientDeductAmt.ToString("f2")));//Deduct
				row.Cells.Add(new UI.GridCell(claimPaid.WriteoffAmt.ToString("f2")));//Writeoff
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
			int colWidthVariable=gridProviderAdjustments.Width-colWidthNPI-colWidthFiscalPeriod-colWidthReasonCode-colWidthRefIdent-colWidthAmount;
			gridProviderAdjustments.BeginUpdate();
			gridProviderAdjustments.ListGridColumns.Clear();
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("NPI",colWidthNPI,HorizontalAlignment.Center));
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("FiscalPeriod",colWidthFiscalPeriod,HorizontalAlignment.Center));
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("Reason",colWidthVariable,HorizontalAlignment.Left));
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("ReasonCode",colWidthReasonCode,HorizontalAlignment.Center));
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("RefIdent",colWidthRefIdent,HorizontalAlignment.Center));			
			gridProviderAdjustments.ListGridColumns.Add(new GridColumn("AdjAmt",colWidthAmount,HorizontalAlignment.Right));
			gridProviderAdjustments.EndUpdate();
			gridProviderAdjustments.BeginUpdate();
			gridProviderAdjustments.ListGridRows.Clear();
			_provAdjAmtSum=0;
			for(int i=0;i<_x835.ListProvAdjustments.Count;i++) {
				Hx835_ProvAdj provAdj=_x835.ListProvAdjustments[i];
				GridRow row=new GridRow();
				row.Tag=provAdj;
				row.Cells.Add(new GridCell(provAdj.Npi));//NPI
				row.Cells.Add(new GridCell(provAdj.DateFiscalPeriod.ToShortDateString()));//FiscalPeriod
				row.Cells.Add(new GridCell(provAdj.ReasonCodeDescript));//Reason
				row.Cells.Add(new GridCell(provAdj.ReasonCode));//ReasonCode
				row.Cells.Add(new GridCell(provAdj.RefIdentification));//RefIdent
				row.Cells.Add(new GridCell(provAdj.AdjAmt.ToString("f2")));//AdjAmt
				_provAdjAmtSum+=provAdj.AdjAmt;
				gridProviderAdjustments.ListGridRows.Add(row);
			}
			gridProviderAdjustments.EndUpdate();
		}

		private void FillFooter() {
			textClaimInsPaidSum.Text=_claimInsPaidSum.ToString("f2");
			textProjAdjAmtSum.Text=_provAdjAmtSum.ToString("f2");
			textPayAmountCalc.Text=(_claimInsPaidSum-_provAdjAmtSum).ToString("f2");
		}

		private void butRawMessage_Click(object sender,EventArgs e) {
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(MessageText835);
			msgbox.Show(this);//This window is just used to display information.
		}

		private void gridProviderAdjustments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_ProvAdj provAdj=(Hx835_ProvAdj)gridProviderAdjustments.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(
				provAdj.Npi+"\r\n"
				+provAdj.DateFiscalPeriod.ToShortDateString()+"\r\n"
				+provAdj.ReasonCode+" "+provAdj.ReasonCodeDescript+"\r\n"
				+provAdj.RefIdentification+"\r\n"
				+provAdj.AdjAmt.ToString("f2"));
			msgbox.Show(this);//This window is just used to display information.
		}

		private void gridClaimDetails_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Claim claimPaid=(Hx835_Claim)gridClaimDetails.ListGridRows[e.Row].Tag;
			Claim claim=claimPaid.GetClaimFromDb();
			bool isAttachNeeded=(!claimPaid.IsAttachedToClaim);
			if(claim==null) {//Not found in db.
				claim=ClaimSelectionHelper(claimPaid);
				if(claim!=null) {//A claim was selected.
					isAttachNeeded=true;//Hard attach selected claim to db claim, since user manually picked matching claim.
				}
			}
			if(claim==null) {//Claim not found and user did not select one.
				return;
			}
			//From this point on claim is not null.
			Patient pat=Patients.GetPat(claim.PatNum);
			if((claimPaid.PatientName.Fname.ToLower().Replace(" ","")!=pat.FName.ToLower().Replace(" ","")
				&& claimPaid.PatientName.Fname.ToLower().Replace(" ","")!=pat.Preferred.ToLower().Replace(" ",""))
				|| claimPaid.PatientName.Lname.ToLower().Replace(" ","")!=pat.LName.ToLower().Replace(" ",""))
			{
				MsgBox.Show(this,"A matching claim was found, but the patient name the carrier sent does not match the patient on the claim.");
			}
			bool isReadOnly=true;
			CreateAttachForClaim(claimPaid,claim.ClaimNum,isAttachNeeded);
			if(claimPaid.IsSplitClaim) {
				//Sync ClaimNum for all split claims in the same group, based on user selection.
				claimPaid.GetOtherNotDetachedSplitClaims()
					.ForEach(x => CreateAttachForClaim(x,claim.ClaimNum,isAttachNeeded));
			}
			if(claimPaid.IsProcessed(_listClaimProcs,_listAttaches)) {
				//If the claim is already received, then we do not allow the user to enter payments.
				//The user can edit the claim to change the status from received if they wish to enter the payments again.
				if(Security.IsAuthorized(Permissions.ClaimView)) {
					Family fam=Patients.GetFamily(claim.PatNum);
					using FormClaimEdit formCE=new FormClaimEdit(claim,pat,fam);
					formCE.ShowDialog();//Modal, because the user could edit information in this window.
				}
				isReadOnly=false;
			}
			else if(Security.IsAuthorized(Permissions.InsPayCreate)) {//Claim found and is not received.  Date not checked here, but it will be checked when actually creating the check.
				EtransL.ImportEraClaimData(_x835,claimPaid,claim,false);
				RefreshFromDb();//ClaimProcs could have been split, need to refresh both claimProc list and attaches.
				isReadOnly=false;
			}
			if(isReadOnly) {
				FormEtrans835ClaimEdit formC=new FormEtrans835ClaimEdit(_x835,claimPaid);
				formC.Show(this);//This window is just used to display information.
			}
			if(!gridClaimDetails.IsDisposed) {//Not sure how the grid is sometimes disposed, perhaps because of it being non-modal.
				//Refresh the claim detail row in case something changed above.
				gridClaimDetails.BeginUpdate();
				List<GridRow> listRows=new List<GridRow>() { gridClaimDetails.ListGridRows[e.Row] };//Row associated to claimPaid
				if(claimPaid.IsSplitClaim) {//Need to update multiple rows.
					List<Hx835_Claim> listOtherSplitClaims=claimPaid.GetOtherNotDetachedSplitClaims();
					List<GridRow> listAdditionalRows=gridClaimDetails.ListGridRows
						.Where(x =>
							x!=gridClaimDetails.ListGridRows[e.Row]
							&& listOtherSplitClaims.Contains((Hx835_Claim)x.Tag)
						).ToList();
					listRows.AddRange(listAdditionalRows);
				}
				SetClaimDetailRows(listRows,listRows.Select(x => (Hx835_Claim)x.Tag ).ToList(),true);
				gridClaimDetails.EndUpdate();
			}
		}

		///<summary>If given claim!=null, attempts to open the patient select and claim select windows.
		///Sets isAttachNeeded to true if user went through full patient and claim selection logic, claim was not null when provided.
		///Returns false if user does not select a claim.</summary>
		private Claim ClaimSelectionHelper(Hx835_Claim claimPaid) {
			PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(false,claimPaid.PatientName.Lname,claimPaid.PatientName.Fname,"","",false,
				"","","","","",0,false,false,DateTime.MinValue,0,"","","","","","","");
			DataTable ptTable=Patients.GetPtDataTable(ptTableSearchParams);//Mimics FormPatientSelect.cs
			long selectedPatNum=0;
			if(ptTable.Rows.Count==1) {
				selectedPatNum=PIn.Long(ptTable.Rows[0]["PatNum"].ToString());
			}
			using FormEtrans835ClaimSelect eTransClaimSelect=new FormEtrans835ClaimSelect(selectedPatNum,claimPaid);
			eTransClaimSelect.ShowDialog();
			if(eTransClaimSelect.DialogResult!=DialogResult.OK) {
				return null;
			}
			Claim claim=eTransClaimSelect.ClaimSelected; //Set claim so below we can act if a claim was already linked.
			if(!String.IsNullOrEmpty(claimPaid.ClaimTrackingNumber) && claimPaid.ClaimTrackingNumber!="0") {//Claim was not printed, it is an eclaim.
				claim.ClaimIdentifier=claimPaid.ClaimTrackingNumber;//Already checked DOS and ClaimFee, update claim identifier to link claims.
				Claims.UpdateClaimIdentifier(claim.ClaimNum,claim.ClaimIdentifier);//Update DB
			}
			return claim;
		}
		
		///<summary>Inserts new Etrans835Attach for given claimPaid and claim.
		///Deletes any existing Etrans835Attach prior to inserting new one.
		///Sets claimPaid.ClaimNum and claimPaid.IsAttachedToClaim.</summary>
		private void CreateAttachForClaim(Hx835_Claim claimPaid,long claimNum,bool isNewAttachNeeded) {
			if(!isNewAttachNeeded 
				&& _listAttaches.Exists(
					x => x.ClaimNum==claimNum 
					&& x.EtransNum==_x835.EtransSource.EtransNum 
					&& x.ClpSegmentIndex==claimPaid.ClpSegmentIndex)
				)
			{
				//Not forcing a new attach and one already exists.
				return;
			}
			//Create a hard link between the selected claim and the claim info on the 835.
			Etrans835Attaches.DeleteMany(claimPaid.ClpSegmentIndex,EtransCur.EtransNum);//Detach existing if any.
			Etrans835Attach attach=new Etrans835Attach();
			attach.EtransNum=EtransCur.EtransNum;
			attach.ClaimNum=claimNum;
			attach.ClpSegmentIndex=claimPaid.ClpSegmentIndex;
			Etrans835Attaches.Insert(attach);
			claimPaid.ClaimNum=claimNum;
			claimPaid.IsAttachedToClaim=true;
		}

		///<summary>Click method used by all gridClaimDetails right click options.</summary>
		private void gridClaimDetails_RightClickHelper(object sender,EventArgs e) {
			int index=gridClaimDetails.GetSelectedIndex();
			if(index==-1) {//Should not happen, menu item is only enabled when exactly 1 row is selected.
				return;
			}
			Hx835_Claim claimPaid=(Hx835_Claim)gridClaimDetails.ListGridRows[index].Tag;
			int menuItemIndex=((MenuItem)sender).Index;//sender is the selected menuItem
#region Go To Account
			if(menuItemIndex==0) {
				if(claimPaid.ClaimNum!=0) {
					Claim claim=Claims.GetClaim(claimPaid.ClaimNum);
					if(claim!=null) {
						GotoModule.GotoAccount(claim.PatNum);
						return;
					}
				}
				PtTableSearchParams ptTableSearchParams=new PtTableSearchParams(false,claimPaid.PatientName.Lname,claimPaid.PatientName.Fname,"","",false,
					"","","","","",0,false,false,DateTime.MinValue,0,"","","","","","","");
				DataTable ptTable=Patients.GetPtDataTable(ptTableSearchParams);//Mimics FormPatientSelect.cs
				if(ptTable.Rows.Count==0) {
					MsgBox.Show(this,"Patient could not be found.  Please manually attach to a claim and try again.");
				}
				else if(ptTable.Rows.Count==1) {
					GotoModule.GotoAccount(PIn.Long(ptTable.Rows[0]["PatNum"].ToString()));
				}
				else {
					MsgBox.Show(this,"Multiple patients with same name found.  Please manually attach to a claim and try again.");
				}
			}
#endregion
#region Add Tracking Status
			else if(menuItemIndex==1) {				
				Claim claim=claimPaid.GetClaimFromDb();
				bool isAttachNeeded=(!claimPaid.IsAttachedToClaim);
				if(claim==null) {//Not found in db.
					claim=ClaimSelectionHelper(claimPaid);
					if(claim!=null) {//A claim was selected.
						isAttachNeeded=true;//Hard attach selected claim to db claim, since user manually picked matching claim.
					}
				}
				if(claim==null) {//Claim not found and user did not select one.
					return;
				}
				CreateAttachForClaim(claimPaid,claim.ClaimNum,isAttachNeeded);
				if(FormClaimEdit.AddClaimCustomTracking(claim,claimPaid.GetRemarks())) {//A tracking status was chosen and the tracking status contains an error code.
					DetachClaimHelper(new List<int> { index });//Detach claim from ERA for convenience.
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
			List<Hx835_Claim> listSelectedEraClaims=gridClaimDetails.SelectedTags<Hx835_Claim>();//See SetClaimDetailRows(...)
			listSelectedEraClaims.RemoveAll(x => !x.IsAttachedToClaim || x.ClaimNum==0);//Limit to era claims that were attached to internal claim.
			List<Claim> listAttachedClaims=_x835.RefreshClaims();
			List<Hx835_Claim> listRecievedEraClaims=listSelectedEraClaims.FindAll(x => listAttachedClaims.Any(y => y.ClaimNum==x.ClaimNum && y.ClaimStatus=="R"));
			StringBuilder sbWarnings=new StringBuilder();
			if(listRecievedEraClaims.Count>0) {
				sbWarnings.AppendLine(Lan.g(this,"You have selected some claims which are already recieved."));
				sbWarnings.AppendLine(Lan.g(this,"Detaching these will not reverse any payment information."));
			}
			if(gridClaimDetails.SelectedIndices.Length>1) {
				sbWarnings.AppendLine(Lan.g(this,"All selected claims will be immediately detached from this ERA even if you click Cancel when you leave the ERA window."));
			}
			if(sbWarnings.Length>0) {
				sbWarnings.AppendLine(Lan.g(this,"Click OK to continue, or click Cancel to leave claims attached."));
				if(MessageBox.Show(this,sbWarnings.ToString(),"",MessageBoxButtons.OKCancel)!=DialogResult.OK) {
					return;
				}
			}
			DetachClaimHelper(gridClaimDetails.SelectedIndices.ToList());
		}

		private void DetachClaimHelper(List<int> listGridIndices) {
			gridClaimDetails.BeginUpdate();
			for(int i=0;i<listGridIndices.Count;i++) {
				GridRow row=gridClaimDetails.ListGridRows[listGridIndices[i]];
				Hx835_Claim claimPaid=(Hx835_Claim)row.Tag;
				Etrans835Attaches.DetachEraClaim(claimPaid);
				SetClaimDetailRows(new List<GridRow>() { row },new List<Hx835_Claim>() { claimPaid },true);
			}
			gridClaimDetails.EndUpdate();
		}

		private void buttonFindClaimMatch_Click(object sender,EventArgs e) {
			List<Hx835_Claim> listDetachedPaidClaims=gridClaimDetails
				.SelectedIndices
				.Select(x => (Hx835_Claim)gridClaimDetails.ListGridRows[x].Tag)
				.ToList();
			if(listDetachedPaidClaims.Any(x => x.IsAttachedToClaim && x.ClaimNum!=0)) {
				MsgBox.Show(this,"Only manually detached rows can be selected.");
				return;
			}
			if(listDetachedPaidClaims.Count==0) {
				MsgBox.Show(this,"Select at least one manually detached row and try again.");
				return;
			}
			List<X12ClaimMatch> listMatches=_x835.GetClaimMatches(listDetachedPaidClaims);
			List<long> listClaimNums=Claims.GetClaimFromX12(listMatches);//Can return null.
			if(listClaimNums!=null) {
				_x835.SetClaimNumsForUnattached(listClaimNums,listDetachedPaidClaims);//List are 1:1 and in same order
				for(int i=0;i<listDetachedPaidClaims.Count;i++) {
					CreateAttachForClaim(listDetachedPaidClaims[i],listClaimNums[i],true);
				}
				List<GridRow> listGridRows=gridClaimDetails.ListGridRows
					.Where(x =>
						listDetachedPaidClaims.Contains((Hx835_Claim)x.Tag)
					).ToList();
				gridClaimDetails.BeginUpdate();
				SetClaimDetailRows(listGridRows,listGridRows.Select(x => ((Hx835_Claim)x.Tag)).ToList(),true);
				gridClaimDetails.EndUpdate();
			}
			MsgBox.Show(this,"Done");
		}

		private void butClaimDetails_Click(object sender,EventArgs e) {
			if(gridClaimDetails.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Choose a claim paid before viewing details.");
				return;
			}
			Hx835_Claim claimPaid=(Hx835_Claim)gridClaimDetails.ListGridRows[gridClaimDetails.SelectedIndices[0]].Tag;
			FormEtrans835ClaimEdit formE=new FormEtrans835ClaimEdit(_x835,claimPaid);
			formE.Show(this);//This window is just used to display information.
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrintClickHelper(false);
		}
		
		private void PrintClickHelper(bool isPreviewMode) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.ERA));
			SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue=_x835;//Required param
			SheetFiller.FillFields(sheet);
			SheetPrinting.Print(sheet,isPreviewMode:isPreviewMode);
		}

		private void butPreview_Click(object sender,EventArgs e) {
			PrintClickHelper(true);
		}
		
		///<summary>Since ERAs are only used in the United States, we do not need to translate any text shown to the user.</summary>
		private void butBatch_Click(object sender,EventArgs e) {
			if(gridClaimDetails.ListGridRows.Count==0) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			if(!EtransL.TryFinalizeBatchPayment(_x835)) {
				Cursor=Cursors.Default;
				return;
			}

			Cursor=Cursors.Default;
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!EtransL.TryDeleteEra(_x835,_x835.RefreshClaims().Select(x => new Hx835_ShortClaim(x)).ToList(),_listClaimProcs,_listAttaches)) {
				return;
			}
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butOK_Click(object sender,EventArgs e) {
			EtransCur.Note=textNote.Text;
			bool isReceived=true;
			for(int i=0;i<gridClaimDetails.ListGridRows.Count;i++) {
				if(gridClaimDetails.ListGridRows[i].Cells[0].Text=="") {
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
			DialogResult=DialogResult.OK;
			Close();
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

		private void FormEtrans835Edit_FormClosing(object sender,FormClosingEventArgs e) {
			_form835=null;
		}
		
	}
}