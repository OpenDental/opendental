using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	///<summary></summary>
	public partial class FormEtrans835ClaimPay : FormODBase {
		private List<Procedure> _listProcedures;
		private Patient _patient;
		private Family _family;
		private List<InsPlan> _listInsPlans;
		private List<PatPlan> _listPatPlans;
		private List<InsSub> _listInsSubs;
		private List<Def> _listDefsClaimPaymentTracking;
		private X835 _x835;
		private Hx835_Claim _hx835_Claim;
		///<summary>All Hx835_Claims from _listOtherSplitClaims and claimPaid all in one list.</summary>
		private List<Hx835_Claim> _listHx835_ClaimsAll;
		private Claim _claim;
		///<summary>The claim procs shown in the grid.  These procs are saved to/from the grid, but changes are not saved to the database unless the OK button is pressed or an individual claim proc is double-clicked for editing.</summary>
		public List<ClaimProc> ListClaimProcs;
		private List<ClaimProc> _listClaimProcsOld;
		private InsPlan _insPlan;
		///<summary>Index of the Split column in the gridPayments.</summary>
		private int _idxSplit=-1;
		///<summary>Index of the Deduct column in the gridPayments.</summary>
		private int _idxDeduct=-1;
		///<summary>Index of the Allowed column in the gridPayments.</summary>
		private int _idxAllowed=-1;
		///<summary>Index of the InsPay/InsEst column in the gridPayments.</summary>
		private int _idxInsPayEst=-1;
		///<summary>Index of the Writeoff column in the gridPayments.</summary>
		private int _idxWriteoff=-1;
		///<summary>Index of the claim payment tracking column in the gridPayments.</summary>
		private int _idxClaimPaymentTracking=-1;
		///<summary>Index of the Remarks column in the gridPayments.</summary>
		private int _idxRemarks=-1;

		///<summary>Flag used to for claim fee and fee billed validation logic.
		///Supplemental payments have a feeBilled of 0.
		///When validating a selection we compare the sum of all feeBilled to claimPaid.ClaimFee, both are set to 0 when true.</summary>
		private bool _isSupplementalPay;

		///<summary>The claimPaid is the individual EOB to load this window for.
		///The listOtherSplitClaims will contain any split claims which are associated to claimPaid.</summary>
		public FormEtrans835ClaimPay(X835 x835,Hx835_Claim hx835_Claim,Claim claim,Patient patient,Family family,List<InsPlan> listInsPlans,List<PatPlan> listPatPlans,List<InsSub> listInsSubs,bool isSupplementalPay) {
			InitializeComponent();
			InitializeLayoutManager();
			_x835=x835;
			_hx835_Claim=hx835_Claim;
			_listHx835_ClaimsAll=new List<Hx835_Claim>();
			_listHx835_ClaimsAll.Add(_hx835_Claim);
			_listHx835_ClaimsAll.AddRange(_hx835_Claim.GetOtherNotDetachedSplitClaims());
			_claim=claim;
			_family=family;
			_patient=patient;
			_listInsPlans=listInsPlans;
			_listInsSubs=listInsSubs;
			_listPatPlans=listPatPlans;
			//If the claim is already received, then the only way to enter payment on top of the existing is to use supplemental.
			_isSupplementalPay=isSupplementalPay;
			Lan.F(this);
		}

		private void FormEtrans835ClaimPay_Load(object sender, System.EventArgs e) {
			_listDefsClaimPaymentTracking=Defs.GetDefsForCategory(DefCat.ClaimPaymentTracking,true);
			_listClaimProcsOld=new List<ClaimProc>();
			for(int i=0;i<ListClaimProcs.Count;i++) {
				_listClaimProcsOld.Add(ListClaimProcs[i].Copy());
			}
			_listProcedures=Procedures.Refresh(_patient.PatNum);
			FillGridClaimAdjustments();
			FillGridProcedureBreakdown();
			decimal claimFee=_listHx835_ClaimsAll.Sum(x => x.ClaimFee);
			if(_isSupplementalPay) {
				//Supplemental payments do not need to validate the claimFee matches the sum of feeBilleds.
				//So we set both the claimFee and all feeBilled values to 0.
				//This mimics how supplemental payments show in FormClaimEdit.
				claimFee=0;
			}
			textEobClaimFee.Text=claimFee.ToString("F");
			textEobDedApplied.Text=_listHx835_ClaimsAll.Sum(x => x.PatientDeductAmt).ToString("F");
			textEobInsPayAllowed.Text=_listHx835_ClaimsAll.Sum(x => x.AllowedAmt).ToString("F");
			textEobInsPayAmt.Text=_listHx835_ClaimsAll.Sum(x => x.InsPaid).ToString("F");
			_insPlan=InsPlans.RefreshOne(_claim.PlanNum);
			if(_insPlan!=null && _insPlan.PlanType.In("","f")) {
				checkIncludeWOPercCoPay.Checked=PrefC.GetBool(PrefName.EraIncludeWOPercCoPay);
			}
			else {
				checkIncludeWOPercCoPay.Checked=true;//Current behaviour is to include all WOs regardless of plan type.
				checkIncludeWOPercCoPay.Visible=false;
			}
			FillGridProcedures();
		}

		private void FormEtrans835ClaimPay_Shown(object sender,EventArgs e) {
			InsPlan insPlan=InsPlans.GetPlan(ListClaimProcs[0].PlanNum,_listInsPlans);
			int selectedIndex=0;
			if(_hx835_Claim.IsSplitClaim) {
				//For split claims we show all the procs on a claim 
				//but make rows bold if they are associated to this split claims procs.
				//Auto select the first bold row.
				selectedIndex=0;
				for(int i=0;i<gridPayments.ListGridRows.Count;i++){
					if(gridPayments.ListGridRows[i].Bold){
						selectedIndex=i;
						break;
					}
				}
			}
			if(insPlan.AllowedFeeSched!=0){//allowed fee sched
				gridPayments.SetSelected(new Point(7,selectedIndex));//Allowed column of the selected row.
			}
			else{
				gridPayments.SetSelected(new Point(8,selectedIndex));//InsPay column of the selected row.
			}
			HighlightEraProcRow(selectedIndex);
		}

		private void FillGridClaimAdjustments() {
			if(_listHx835_ClaimsAll.All(x => x.ListClaimAdjustments.Count==0)) {
				gridClaimAdjustments.Title="EOB Claim Adjustments (None Reported)";
			}
			else {
				gridClaimAdjustments.Title="EOB Claim Adjustments";
			}
			gridClaimAdjustments.BeginUpdate();
			gridClaimAdjustments.Columns.Clear();
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("Reason",445,HorizontalAlignment.Left));
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("Allowed",62,HorizontalAlignment.Right));
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("Ins Pay",62,HorizontalAlignment.Right));
			GridColumn col=new UI.GridColumn("Remarks",62,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridClaimAdjustments.Columns.Add(col);
			gridClaimAdjustments.ListGridRows.Clear();
			List<Hx835_Adj> listHx835_Adjs=_listHx835_ClaimsAll.SelectMany(x => x.ListClaimAdjustments).ToList();
			for(int i=0;i<listHx835_Adjs.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=listHx835_Adjs[i];
				row.Cells.Add(new GridCell(listHx835_Adjs[i].ReasonDescript));//Reason
				row.Cells.Add(new GridCell((-listHx835_Adjs[i].AdjAmt).ToString("f2")));//Allowed
				row.Cells.Add(new GridCell((-listHx835_Adjs[i].AdjAmt).ToString("f2")));//Ins Pay
				row.Cells.Add(new GridCell(listHx835_Adjs[i].AdjustRemarks));//Remarks
				gridClaimAdjustments.ListGridRows.Add(row);
			}
			gridClaimAdjustments.EndUpdate();
		}

		private void FillGridProcedureBreakdown() {
			if(_listHx835_ClaimsAll.All(x => x.ListProcs.Count==0)) {
				gridProcedureBreakdown.Title="EOB Procedure Breakdown (None Reported)";
			}
			else {
				gridProcedureBreakdown.Title="EOB Procedure Breakdown";
			}
			gridProcedureBreakdown.BeginUpdate();
			gridProcedureBreakdown.Columns.Clear();
			gridProcedureBreakdown.Columns.Add(new GridColumn("ProcNum",116,HorizontalAlignment.Left));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Code",50,HorizontalAlignment.Center));
			gridProcedureBreakdown.Columns.Add(new GridColumn("",25,HorizontalAlignment.Center));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Description",130,HorizontalAlignment.Left));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Fee Billed",62,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Deduct",62,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Allowed",62,HorizontalAlignment.Right));
			string insTitle="InsPay";
			if(_hx835_Claim.IsPreauth) {
				insTitle="InsEst";
			}
			gridProcedureBreakdown.Columns.Add(new GridColumn(insTitle,62,HorizontalAlignment.Right));
			GridColumn col=new GridColumn("Remarks",62,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridProcedureBreakdown.Columns.Add(col);
			gridProcedureBreakdown.ListGridRows.Clear();
			List<Hx835_Proc> listHx835_Procs=_listHx835_ClaimsAll.SelectMany(x => x.ListProcs).ToList();
			for(int i=0;i<listHx835_Procs.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=listHx835_Procs[i];
				if(listHx835_Procs[i].ProcNum==0) {
					row.Cells.Add(new GridCell(""));//ProcNum
				}
				else {
					row.Cells.Add(new GridCell(listHx835_Procs[i].ProcNum.ToString()));//ProcNum
				}
				row.Cells.Add(new GridCell(listHx835_Procs[i].ProcCodeAdjudicated));//Code
				row.Cells.Add(new GridCell(""));//Blank
				string procDescript="";
				if(ProcedureCodes.IsValidCode(listHx835_Procs[i].ProcCodeAdjudicated)) {
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(listHx835_Procs[i].ProcCodeAdjudicated);
					procDescript=procedureCode.AbbrDesc;
				}
				row.Cells.Add(new GridCell(procDescript));//Description
				decimal procFee=listHx835_Procs[i].ProcFee;
				if(_isSupplementalPay) {
					//Supplemental payments do not have a proc fee or fee billed.
					//Supplemental payments do not need to validate the claimFee matches the sum of feeBilleds
					procFee=0;
				}
				row.Cells.Add(new GridCell(procFee.ToString("f2")));//Fee Billed
				row.Cells.Add(new GridCell(listHx835_Procs[i].DeductibleAmt.ToString("f2")));//Deduct
				row.Cells.Add(new GridCell(listHx835_Procs[i].AllowedAmt.ToString("f2")));//Allowed
				row.Cells.Add(new GridCell(listHx835_Procs[i].InsPaid.ToString("f2")));//InsPay or InsEst
				row.Cells.Add(new GridCell(listHx835_Procs[i].GetRemarks()));//Remarks
				gridProcedureBreakdown.ListGridRows.Add(row);
			}
			gridProcedureBreakdown.EndUpdate();
		}

		private void FillGridProcedures(){
			bool isWOIncluded=checkIncludeWOPercCoPay.Checked;
			//Changes made in this window do not get saved until after this window closes.
			//But if you double click on a row, then you will end up saving.  That shouldn't hurt anything, but could be improved.
			//also calculates totals for this "payment"
			//the payment itself is imaginary and is simply the sum of the claimprocs on this form
			gridPayments.BeginUpdate();
			gridPayments.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableClaimProc","Split"),30,HorizontalAlignment.Center);//Split proc selection column
			gridPayments.Columns.Add(col);
			_idxSplit=gridPayments.Columns.Count-1;
			col=new GridColumn(Lan.g("TableClaimProc","Date"),66);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Prov"),50);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Code"),50);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Tth"),25);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Description"),130);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Fee Billed"),62,HorizontalAlignment.Right);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Deduct"),62,HorizontalAlignment.Right,isEditable:true);
			gridPayments.Columns.Add(col);
			_idxDeduct=gridPayments.Columns.Count-1;
			col=new GridColumn(Lan.g("TableClaimProc","Allowed"),62,HorizontalAlignment.Right,isEditable:true);
			gridPayments.Columns.Add(col);
			_idxAllowed=gridPayments.Columns.Count-1;
			string insTitle="InsPay";
			if(_hx835_Claim.IsPreauth) {
				insTitle="InsEst";
			}
			col=new GridColumn(Lan.g("TableClaimProc",insTitle),62,HorizontalAlignment.Right,isEditable:true);
			gridPayments.Columns.Add(col);
			_idxInsPayEst=gridPayments.Columns.Count-1;
			col=new GridColumn(Lan.g("TableClaimProc","Writeoff"),62,HorizontalAlignment.Right,isEditable:isWOIncluded);
			gridPayments.Columns.Add(col);
			_idxWriteoff=gridPayments.Columns.Count-1;
			col=new GridColumn(Lan.g("TableClaimProc","Status"),50,HorizontalAlignment.Center);
			gridPayments.Columns.Add(col);
			col=new GridColumn(Lan.g("TableClaimProc","Pmt"),30,HorizontalAlignment.Center);
			gridPayments.Columns.Add(col);
			if(PrefC.GetBool(PrefName.ClaimEditShowPayTracking)) {
				List<string> listDefDescripts=new List<string>();
				listDefDescripts.Add("None");
				listDefDescripts.AddRange(_listDefsClaimPaymentTracking.Select(x => x.ItemName));
				col=new GridColumn(Lan.g("TableClaimProc","Pay Tracking"),90);
				col.ListDisplayStrings=listDefDescripts;
				col.DropDownWidth=160;
				gridPayments.Columns.Add(col);
				_idxClaimPaymentTracking=gridPayments.Columns.Count-1;
			}
			col=new GridColumn(Lan.g("TableClaimProc","Remarks"),62,isEditable:true);
			col.IsWidthDynamic=true;
			gridPayments.Columns.Add(col);
			_idxRemarks=gridPayments.Columns.Count-1;
			gridPayments.ListGridRows.Clear();
			GridRow row;
			Procedure procedure;
			for(int i=0;i<ListClaimProcs.Count;i++){
				ClaimProc claimProc=ListClaimProcs[i];
				if((_isSupplementalPay && claimProc.Status==ClaimProcStatus.Received && claimProc.ProcNum!=0)//Skip original claims recieved claimProcs.
					|| (_isSupplementalPay && claimProc.Status==ClaimProcStatus.Received && claimProc.ProcNum==0 && !claimProc.IsNew)//Skip pre-exiting "By Total" payment claimProcs.
					|| (_isSupplementalPay && claimProc.Status==ClaimProcStatus.Supplemental && !claimProc.IsNew))//Skip pre-existing supplemental payments.
				{
					//When entering supplemental payments we only want to show newly created supplemental claimProcs or "By Total" claimProcs.
					continue;
				}
				row=new GridRow();
				row.Tag=claimProc;
				if(_hx835_Claim.IsSplitClaim
					&& claimProc.Status.In(ClaimProcStatus.Received,ClaimProcStatus.Supplemental)
					&& claimProc.IsNew)
				{
					//Highlight the subset of procdures for a split claim that we are entering payment for.
					//Split claims process a sub set of a claims procs. We show all procs here even if this split claim does not contain them.
					row.Bold=true;
				}
				row.Cells.Add("");//Split claim selection column
				if(claimProc.ProcNum==0) {//Total payment
					//We want to always show the "Payment Date" instead of the procedure date for total payments because they are not associated to procedures.
					row.Cells.Add(claimProc.DateCP.ToShortDateString());
				}
				else {
					row.Cells.Add(claimProc.ProcDate.ToShortDateString());
				}
				row.Cells.Add(Providers.GetAbbr(claimProc.ProvNum));
				if(claimProc.ProcNum==0) {
					row.Cells.Add("");
					row.Cells.Add("");
					row.Cells.Add(Lan.g(this,"Total Payment"));
				}
				else {
					procedure=Procedures.GetProcFromList(_listProcedures,claimProc.ProcNum);
					row.Cells.Add(ProcedureCodes.GetProcCode(procedure.CodeNum).ProcCode);
					row.Cells.Add(Tooth.Display(procedure.ToothNum));
					row.Cells.Add(ProcedureCodes.GetProcCode(procedure.CodeNum).Descript);
				}
				row.Cells.Add(claimProc.FeeBilled.ToString("F"));
				row.Cells.Add(claimProc.DedApplied.ToString("F"));
				if(claimProc.AllowedOverride==-1){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(claimProc.AllowedOverride.ToString("F"));
				}
				if(claimProc.Status==ClaimProcStatus.Preauth) {
					row.Cells.Add(claimProc.InsPayEst.ToString("F"));
				}
				else {
					row.Cells.Add(claimProc.InsPayAmt.ToString("F"));
				}
				if(isWOIncluded) {
					row.Cells.Add(claimProc.WriteOff.ToString("F"));
				}
				else{
					row.Cells.Add(0d.ToString("F"));
				}
				switch(claimProc.Status){
					case ClaimProcStatus.Received:
						row.Cells.Add("Recd");
						break;
					case ClaimProcStatus.NotReceived:
						row.Cells.Add("");
						break;
					//adjustment would never show here
					case ClaimProcStatus.Preauth:
						row.Cells.Add("PreA");
						break;
					case ClaimProcStatus.Supplemental:
						row.Cells.Add("Supp");
						break;
					case ClaimProcStatus.CapClaim:
						row.Cells.Add("Cap");
						break;
					//Estimate would never show here
					//Cap would never show here
				}
				row.Cells.Add(claimProc.ClaimPaymentNum>0?"X":"");
				if(PrefC.GetBool(PrefName.ClaimEditShowPayTracking)) {
					Def defClaimPaymentTracking=_listDefsClaimPaymentTracking.FirstOrDefault(x => x.DefNum==claimProc.ClaimPaymentTracking);
					if(defClaimPaymentTracking==null) {
						row.Cells.Add("");
						row.Cells[row.Cells.Count-1].ComboSelectedIndex=0;
					}
					else {
						row.Cells.Add(defClaimPaymentTracking.ItemName);
						row.Cells[row.Cells.Count-1].ComboSelectedIndex=_listDefsClaimPaymentTracking.IndexOf(defClaimPaymentTracking)+1;
					}
					//bool isDefPresent=false;
					//for(int j=0;j<_listClaimPaymentTrackingDefs.Count;j++) {
					//	if(claimProc.ClaimPaymentTracking==_listClaimPaymentTrackingDefs[j].DefNum) {
					//		row.Cells.Add(_listClaimPaymentTrackingDefs[j].ItemName);
					//		row.Cells[row.Cells.Count-1].ComboSelectedIndex=j+1;
					//		isDefPresent=true;
					//		break;
					//	}
					//}
					//if(!isDefPresent) { //The ClaimPaymentTracking definition has been hidden or ClaimPaymentTracking==0 (not possible to hide last pay tracking def)
					//	row.Cells.Add("");
					//	row.Cells[row.Cells.Count-1].ComboSelectedIndex=0;
					//}
				}
				row.Cells.Add(claimProc.Remarks);
				gridPayments.ListGridRows.Add(row);
			}
			gridPayments.EndUpdate();
			FillTotals();
		}

		private void gridClaimAdjustments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Adj hx835_Adj=(Hx835_Adj)gridClaimAdjustments.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(hx835_Adj.AdjCode+" "+hx835_Adj.AdjustRemarks+"\r\r"+hx835_Adj.ReasonDescript+"\r\n"+hx835_Adj.AdjAmt.ToString("f2"));
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridProcedureBreakdown_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Proc hx835_Proc=(Hx835_Proc)gridProcedureBreakdown.ListGridRows[e.Row].Tag;
			FormEtrans835ProcEdit formEtrans835ProcEdit=new FormEtrans835ProcEdit(hx835_Proc);
			formEtrans835ProcEdit.Show(this);//This window is just used to display information.
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return;
			}
			try {
				SaveGridChanges();
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			List<ClaimProcHist> listClaimProcHists=null;
			List<ClaimProcHist> listClaimProcHistsLoop=null;
			ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[e.Row].Tag;
			using FormClaimProc formClaimProc=new FormClaimProc(claimProc,null,_family,_patient,_listInsPlans,listClaimProcHists,ref listClaimProcHistsLoop,_listPatPlans,false,_listInsSubs);
			formClaimProc.IsInClaim=true;
			//no need to worry about permissions here
			formClaimProc.ShowDialog();//Modal because this window can change information.
			if(formClaimProc.DialogResult!=DialogResult.OK){
				return;
			}
			if(claimProc.DoDelete) {
				ListClaimProcs.Remove(claimProc);
			}
			FillGridProcedures();
			FillTotals();
		}

		private void gridMain_CellTextChanged(object sender,EventArgs e) {
			FillTotals();
		}
		
		private void gridPayments_CellClick(object sender,ODGridClickEventArgs e) {
			HighlightEraProcRow(e.Row);
			switch(e.Col) {
				case 0://Split claim column
					bool isSpliting=false;
					if(gridPayments.ListGridRows[e.Row].Cells[e.Col].Text=="") {//Row is not selected to split currently, will select for split.
						isSpliting=true;
					}
					gridPayments.BeginUpdate();
					gridPayments.ListGridRows[e.Row].Cells[e.Col].Text=(isSpliting?"X":"");
					gridPayments.EndUpdate();
					break;
				case 10://WO column and not including WOs, so do not let them edit.
					if(!checkIncludeWOPercCoPay.Checked) {
						MsgBox.Show(this,"WriteOffs cannot be edited when Include Writeoffs checkbox is not checked.");
					}
					break;
			}
		}

		private void HighlightEraProcRow(int idxPaymentRow) {
			ClaimProc claimProcSelected=(ClaimProc)gridPayments.ListGridRows[idxPaymentRow].Tag;
			if(claimProcSelected.ProcNum==0) {
				return;
			}
			List<Hx835_ShortClaimProc> listHx835_ShortClaimProcs=new List<Hx835_ShortClaimProc>() { new Hx835_ShortClaimProc(claimProcSelected) };
			for(int i=0;i<gridProcedureBreakdown.ListGridRows.Count;i++){
				Hx835_Proc hx835_Proc=(Hx835_Proc)gridProcedureBreakdown.ListGridRows[i].Tag;
				Hx835_ShortClaimProc hx835_ShortClaimProcMatched;
				hx835_Proc.TryGetMatchedClaimProc(out hx835_ShortClaimProcMatched,listHx835_ShortClaimProcs,_isSupplementalPay);
				gridProcedureBreakdown.SetSelected(i,(hx835_ShortClaimProcMatched!=null));
			}
		}

		///<Summary>Fails silently if text is in invalid format.</Summary>
		private void FillTotals(){
			double claimFee=0;
			double dedApplied=0;
			double insPayAmtAllowed=0;
			double insPayAmt=0;
			double writeOff=0;
			//double amt;
			for(int i=0;i<gridPayments.ListGridRows.Count;i++){
				ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
				claimFee+=claimProc.FeeBilled;//5
				dedApplied+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxDeduct].Text);//6.deduct
				insPayAmtAllowed+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text);//7.allowed
				insPayAmt+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);//8.inspayest
				writeOff+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text);//9.writeoff
			}
			textClaimFee.Text=claimFee.ToString("F");
			textDedApplied.Text=dedApplied.ToString("F");
			textInsPayAllowed.Text=insPayAmtAllowed.ToString("F");
			textInsPayAmt.Text=insPayAmt.ToString("F");
			textWriteOff.Text=writeOff.ToString("F");
		}

		private List<ClaimProc> GetListClaimProcHypothetical() {
			List<ClaimProc> listClaimProcsHypothetical=new List<ClaimProc>();
			for(int i=0;i<gridPayments.ListGridRows.Count;i++) {
				ClaimProc claimProc=((ClaimProc)gridPayments.ListGridRows[i].Tag).Copy();
				int idxInsPay=gridPayments.Columns.GetIndex(Lan.g("TableClaimProc","InsPay"));
				int idxWriteOff=gridPayments.Columns.GetIndex(Lan.g("TableClaimProc","Writeoff"));
				if(idxInsPay!=-1) {
					claimProc.InsPayAmt=PIn.Double(gridPayments.ListGridRows[i].Cells[idxInsPay].Text);
				}
				claimProc.WriteOff=PIn.Double(gridPayments.ListGridRows[i].Cells[idxWriteOff].Text);
				listClaimProcsHypothetical.Add(claimProc);
			}
			return listClaimProcsHypothetical;
		}

		///<Summary>Surround with try-catch.</Summary>
		private void SaveGridChanges(){
		//validate all grid cells
			double dbl;
			for(int i=0;i<gridPayments.ListGridRows.Count;i++){
				if(gridPayments.ListGridRows[i].Cells[_idxDeduct].Text!=""){//deduct
					try{
						dbl=Convert.ToDouble(gridPayments.ListGridRows[i].Cells[_idxDeduct].Text);
					}
					catch{
						throw new ApplicationException(Lan.g(this,"Deductible not valid: ")+gridPayments.ListGridRows[i].Cells[_idxDeduct].Text);
					}
				}
				if(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text!=""){//allowed
					try{
						dbl=Convert.ToDouble(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text);
					}
					catch{
						throw new ApplicationException(Lan.g(this,"Allowed amt not valid: ")+gridPayments.ListGridRows[i].Cells[_idxAllowed].Text);
					}
				}
				if(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text!=""){//inspay
					try{
						dbl=Convert.ToDouble(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);
					}
					catch{
						throw new ApplicationException(Lan.g(this,"Ins Pay not valid: ")+gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);
					}
				}
				if(gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text!=""){//writeoff
					try{
						dbl=Convert.ToDouble(gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text);
					}
					catch{
						throw new ApplicationException(Lan.g(this,"Writeoff not valid: ")+gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text);
					}
					if(dbl<0 && !_hx835_Claim.IsReversal){//Claim reversals have negative writeoffs.
						throw new ApplicationException(Lan.g(this,"Writeoff cannot be negative: ")+gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text);
					}
				}
			}
			for(int i=0;i<gridPayments.ListGridRows.Count;i++) {
				ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
				claimProc.DedApplied=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxDeduct].Text);
				if(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text==""){
					claimProc.AllowedOverride=-1;
				}
				else{
					claimProc.AllowedOverride=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text);
				}
				if(claimProc.Status==ClaimProcStatus.Preauth) {
					claimProc.InsPayEst=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);
				}
				else {
					claimProc.InsPayAmt=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);
				}
				claimProc.WriteOff=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxWriteoff].Text);
				if(PrefC.GetBool(PrefName.ClaimEditShowPayTracking)) {
					int index=gridPayments.ListGridRows[i].Cells[_idxClaimPaymentTracking].ComboSelectedIndex;
					claimProc.ClaimPaymentTracking=0;//Set to 0 if combo index is 0.
					if(index!=0) {//Otherwise set to chosen DefNum.
						claimProc.ClaimPaymentTracking=_listDefsClaimPaymentTracking[index-1].DefNum;
					}
				}
				claimProc.Remarks=gridPayments.ListGridRows[i].Cells[_idxRemarks].Text;
			}
		}

		private void butDeductible_Click(object sender, System.EventArgs e) {
			if(gridPayments.SelectedCell.X==-1) {
				MessageBox.Show(Lan.g(this,"Please select one payment line.  Then click this button to assign the deductible to that line."));
				return;
			}
			if(ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return;
			}
			try {
				SaveGridChanges();
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			Double dedAmt=0;
			ClaimProc claimProc;
			//remove the existing deductible from each payment line and move it to dedAmt.
			for(int i=0;i<gridPayments.ListGridRows.Count;i++) {
				claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
				if(claimProc.DedApplied > 0){
					dedAmt+=claimProc.DedApplied;
					claimProc.InsPayEst+=claimProc.DedApplied;//dedAmt might be more
					claimProc.InsPayAmt+=claimProc.DedApplied;
					claimProc.DedApplied=0;
				}
			}
			if(dedAmt==0){
				MessageBox.Show(Lan.g(this,"There does not seem to be a deductible to apply.  You can still apply a deductible manually by double clicking on a payment line."));
				return;
			}
			//then move dedAmt to the selected proc
			claimProc=(ClaimProc)gridPayments.ListGridRows[gridPayments.SelectedCell.Y].Tag;
			claimProc.DedApplied=dedAmt;
			claimProc.InsPayEst-=dedAmt;
			claimProc.InsPayAmt-=dedAmt;
			FillGridProcedures();
		}

		private void butWriteOff_Click(object sender, System.EventArgs e) {
			if(MessageBox.Show(Lan.g(this,"Write off unpaid amount on each procedure?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			//We don't check these preferences when taking this same action in FormClaimPayTotal.
			//if(ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
			//	return;
			//}
			try {
				SaveGridChanges();
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			//fix later: does not take into account other payments.
			double amtUnpaid=0;
			List<Procedure> listProcedures=Procedures.Refresh(_patient.PatNum);
			for(int i=0;i<gridPayments.ListGridRows.Count;i++) {
				ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
				if(claimProc.ProcNum==0) {
					continue;//Ignore "Total Payment" lines.
				}
				amtUnpaid=Procedures.GetProcFromList(listProcedures,claimProc.ProcNum).ProcFee
					//((Procedure)Procedures.HList[ClaimProcsToEdit[i].ProcNum]).ProcFee
					-claimProc.DedApplied
					-claimProc.InsPayAmt;
				if(amtUnpaid > 0){
					claimProc.WriteOff=amtUnpaid;
				}
			}
			FillGridProcedures();
		}

		private void butViewEobDetails_Click(object sender,EventArgs e) {
			FormEtrans835ClaimEdit formEtrans835ClaimEdit=new FormEtrans835ClaimEdit(_x835,_hx835_Claim);
			formEtrans835ClaimEdit.Show(this);//This window is just used to display information.
		}
		
		///<summary>Called when entering split claim payment information.
		///Returns true if the entered payment rows sum up to the sub set proc information present on this split claim.</summary>
		private bool HasValidSplitClaimTotals() {
			double claimFee=0;
			double dedApplied=0;
			double insPayAmtAllowed=0;
			double insPayAmt=0;
			for(int i=0;i<gridPayments.ListGridRows.Count;i++){
				ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
				if((_isSupplementalPay && claimProc.Status!=ClaimProcStatus.Supplemental)
					|| (!_isSupplementalPay && claimProc.Status!=ClaimProcStatus.Received )
					|| !claimProc.IsNew)
				{
					//Split claims show all the of the original claims procs.
					//But we only enter payment for the procs that are on this split claim.
					continue;
				}
				claimFee+=claimProc.FeeBilled;
				dedApplied+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxDeduct].Text);
				insPayAmtAllowed+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxAllowed].Text);
				insPayAmt+=PIn.Double(gridPayments.ListGridRows[i].Cells[_idxInsPayEst].Text);
			}
			if(textEobClaimFee.Text!=claimFee.ToString("F")
				|| textEobDedApplied.Text!=dedApplied.ToString("F")
				|| textEobInsPayAllowed.Text!=insPayAmtAllowed.ToString("F")
				|| textEobInsPayAmt.Text!=insPayAmt.ToString("F"))
			{ 
				return false;
			}
			return true;
		}

		private void checkIncludeWOPercCoPay_CheckedChanged(object sender,EventArgs e) {
			if(checkIncludeWOPercCoPay.Checked) {
				//WO's might have been cleared from grid and saved as 0 in claimProc.WriteOff. See SaveGridChanges().
				//Need to set claimProc.WriteOff back to original value when including writeoffs.
				for(int i=0;i<gridPayments.ListGridRows.Count;i++) {
					ClaimProc claimProc=(ClaimProc)gridPayments.ListGridRows[i].Tag;
					ClaimProc claimProcOld=_listClaimProcsOld.First(x => x.ClaimProcNum==claimProc.ClaimProcNum);
					if(claimProc.WriteOff==claimProcOld.WriteOff) {
							continue;
					}
					claimProc.WriteOff=claimProcOld.WriteOff;
				}
			}
			FillGridProcedures();
		}
		
		private void butSplitProcs_Click(object sender,EventArgs e) {
			List<GridRow> listRowsSelected=gridPayments.ListGridRows.Where(x => x.Cells[_idxSplit].Text=="X").ToList();//Split column is "checked".
			if(listRowsSelected.Count()==0) {
				MsgBox.Show(this,"Please use the Split column to select at least one procedure.");
				return;
			}
			if(ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return;
			}
			try {
				SaveGridChanges();//User must zero out the inspay column to allow them to split.  Save changes to claimProcs.
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			#region Claim Validation
			List<ClaimProc> listClaimProcsSelected=listRowsSelected.Select(x => x.Tag as ClaimProc).ToList();
			switch(ClaimL.ClaimIsValid(_claim,listClaimProcsSelected)) {
				case ClaimIsValidState.False:
				case ClaimIsValidState.FalseClaimProcsChanged://Status of claimProcs were not preauth and claim was preauth.
					return;
				case ClaimIsValidState.True:
					//Good to go.
				break;
			}
			#endregion
			#region Selection validation
			//The following logic mimics FormClaimEdit.butSplit_Click(...)
			if(listClaimProcsSelected.Any(x => x.ProcNum==0)){
				MsgBox.Show(this,"Only procedures can be selected.  No total payments allowed.  Deselect all total payments before continuing.");
				return;
			}
			if(listClaimProcsSelected.Any(x => x.InsPayAmt!=0)) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"All selected procedures must have zero insurance payment amounts.\r\nSet all selected insurance payment amounts to zero?")) {
					return;
				}
				listClaimProcsSelected.ForEach(x => x.InsPayAmt=0);
			}
			//Make sure that there is at least one procedure left on the claim before splitting.
			//The claim would become orphaned if we allow users to split off all procedures on the claim and DBM would be required to run to clean up.
			if(gridPayments.ListGridRows.Count==listClaimProcsSelected.Count) {//All procedures are selected for the split...
				MsgBox.Show(this,"All procedures were selected.  At least one procedure must remain on this claim after splitting.  Deselect at least one procedure before continuing.");
				return;
			}
			#endregion
			//Point of no return-------------------------------------------------------------------------
			//Selection validation logic ensures these are only procs that are eligible to split from this claim.
			//InsertSplitClaim() changes the database immediately.  If the user cancels the current window, changes will be saved anyway.
			Claim claimSplit=Claims.InsertSplitClaim(_claim,listClaimProcsSelected);
			#region Split Etrans835Attach
			//Insert a Etrans835Attach row to allow us to associate the split claim back to the original ERA/Claim it is split from.
			//The split attach is inserted immediately.  If the user cancels the current window, changes will be saved anyway.
			Etrans835Attach etrans835AttachSplit=new Etrans835Attach();
			etrans835AttachSplit.EtransNum=_x835.EtransSource.EtransNum;
			etrans835AttachSplit.ClpSegmentIndex=_hx835_Claim.ClpSegmentIndex;
			etrans835AttachSplit.ClaimNum=claimSplit.ClaimNum;
			Etrans835Attaches.Insert(etrans835AttachSplit);
			#endregion
			//Update the multi-visit groups
			for(int i=0;i<ListClaimProcs.Count;i++) {
				Procedure procedure=Procedures.GetOneProc(ListClaimProcs[i].ProcNum,false);
				ProcMultiVisits.UpdateGroupForProc(procedure.ProcNum,procedure.ProcStatus);
			}
			//Remove all split claimProcs from the internal list, will not show in grid after FillGridProcedures();
			ListClaimProcs.RemoveAll(x => listClaimProcsSelected.Contains(x));
			FillGridProcedures();
			MsgBox.Show(this,"Done.");
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(ClaimL.AreCreditsGreaterThanProcFee(GetListClaimProcHypothetical())) {
				return;
			}
			try {
				SaveGridChanges();
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			List<ClaimProc> listClaimProcsInGrid=gridPayments.ListGridRows.Select(x=>x.Tag as ClaimProc).ToList();
			if(!PrefC.GetBool(PrefName.EraAllowTotalPayments)) {
				if(listClaimProcsInGrid.Any(x=>x.ProcNum==0 && (!CompareDouble.IsZero(x.InsPayAmt) || !CompareDouble.IsZero(x.DedApplied) || !CompareDouble.IsZero(x.WriteOff)))) {
					MsgBox.Show("Please allocate all InsPay, Deductible, and Writeoff amounts from Total Payment rows to a procedure before continuing.");
					return;
				} 
			}
			bool isPromptNeeded=false;
			if(_hx835_Claim.IsSplitClaim) {
				isPromptNeeded=!HasValidSplitClaimTotals();//Show prompt if validation fails.
			}
			else if(textEobClaimFee.Text!=textClaimFee.Text
				|| textEobDedApplied.Text!=textDedApplied.Text
				|| textEobInsPayAllowed.Text!=textInsPayAllowed.Text
				|| textEobInsPayAmt.Text!=textInsPayAmt.Text)
			{
				isPromptNeeded=true;
			}
			if(isPromptNeeded && !MsgBox.Show(this,MsgBoxButtons.YesNo,"Some of the EOB totals do not match the totals entered.  Continue?")) {
				return;
			}
			List<ClaimProc> listClaimProcs=gridPayments.GetTags<ClaimProc>();
			FeeL.SaveAllowedFeesFromClaimPayment(listClaimProcs,_listInsPlans);
			Etranss.ReceiveEraPayment(_claim,_hx835_Claim,ListClaimProcs,checkIncludeWOPercCoPay.Checked,_isSupplementalPay,_insPlan);
			if(PrefC.GetBool(PrefName.PromptForSecondaryClaim) && Security.IsAuthorized(Permissions.ClaimSend,suppressMessage:true)) {
				ClaimL.PromptForSecondaryClaim(ListClaimProcs);
			}
			if(PrefC.GetBool(PrefName.ClaimSnapshotEnabled)) {
				Claim claim=Claims.GetClaim(_listClaimProcsOld[0].ClaimNum);
				if(claim.ClaimType!="PreAuth") {
					ClaimSnapshots.CreateClaimSnapshot(_listClaimProcsOld,ClaimSnapshotTrigger.InsPayment,claim.ClaimType);
				}
			}
			for(int i=0;i<listClaimProcs.Count;i++) {
				ClaimProc claimProcOld=_listClaimProcsOld.FirstOrDefault(x => x.ClaimProcNum==listClaimProcs[i].ClaimProcNum);
				ClaimProcs.CreateAuditTrailEntryForClaimProcPayment(listClaimProcs[i],claimProcOld,isInsPayCreate:true,isPaymentFromERA:true);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
