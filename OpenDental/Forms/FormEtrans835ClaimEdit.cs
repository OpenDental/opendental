using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans835ClaimEdit:FormODBase {

		private X835 _x835;
		private Hx835_Claim _hx835_Claim;
		private decimal _sumClaimAdjAmt;
		private decimal _sumProcAdjAmt;

		public FormEtrans835ClaimEdit(X835 x835,Hx835_Claim hx835_Claim) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_x835=x835;
			_hx835_Claim=hx835_Claim;
		}

		private void FormEtrans835ClaimEdit_Load(object sender,EventArgs e) {
			FillAll();
		}

		private void FormEtrans835ClaimEdit_Resize(object sender,EventArgs e) {
			FillClaimAdjustments();//Because the grid columns change size depending on the form size.
			FillProcedureBreakdown();//Because the grid columns change size depending on the form size.
			FillAdjudicationInfo();//Because the grid columns change size depending on the form size.
			FillSupplementalInfo();//Because the grid columns change size depending on the form size.
		}

		private void FillAll() {
			FillClaimAdjustments();
			FillProcedureBreakdown();
			FillAdjudicationInfo();
			FillSupplementalInfo();
			FillHeader();//Must be last, so internal summations are complete before filling totals in textboxes.
		}

		private void FillHeader() {
			Text="Claim Explanation of Benefits (EOB)";
			if(_hx835_Claim.Npi!="") {
				Text+=" - NPI: "+_hx835_Claim.Npi;
			}
			Text+=" - Patient: "+_hx835_Claim.PatientName;
			textSubscriberName.Text=_hx835_Claim.SubscriberName.ToString();
			textPatientName.Text=_hx835_Claim.PatientName.ToString();
			textDateService.Text=_hx835_Claim.DateServiceStart.ToShortDateString();
			if(_hx835_Claim.DateServiceEnd>_hx835_Claim.DateServiceStart) {
				textDateService.Text+=" to "+_hx835_Claim.DateServiceEnd.ToShortDateString();
				textDateService.Width=160;//Increase width to accout for extra text.
			}
			textClaimIdentifier.Text=_hx835_Claim.ClaimTrackingNumber;
			textPayorControlNum.Text=_hx835_Claim.PayerControlNumber;
			textStatus.Text=_hx835_Claim.StatusCodeDescript;
			textClaimFee.Text=_hx835_Claim.ClaimFee.ToString("f2");
			textClaimFee2.Text=_hx835_Claim.ClaimFee.ToString("f2");
			textInsPaid.Text=_hx835_Claim.InsPaid.ToString("f2");
			textInsPaidCalc.Text=(_hx835_Claim.ClaimFee-_sumClaimAdjAmt-_sumProcAdjAmt).ToString("f2");
			textPatResp.Text=_hx835_Claim.PatientRespAmt.ToString("f2");
			if(_hx835_Claim.DatePayerReceived.Year>1880) {
				textDatePayerReceived.Text=_hx835_Claim.DatePayerReceived.ToShortDateString();
			}
		}

		private void FillClaimAdjustments() {
			if(_hx835_Claim.ListClaimAdjustments.Count==0) {
				gridClaimAdjustments.Title="EOB Claim Adjustments (None Reported)";
			}
			else {
				gridClaimAdjustments.Title="EOB Claim Adjustments";
			}
			gridClaimAdjustments.BeginUpdate();
			gridClaimAdjustments.Columns.Clear();
			const int colWidthReason=507;
			const int colWidthAdjAmt=62;
			int widthCol=gridClaimAdjustments.Width-10-colWidthReason-colWidthAdjAmt;
			//The size and order of the columns here mimics the EOB Claim Adjustments grid in FormEtrans835ClaimPay as close as possible.
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("Reason",colWidthReason,HorizontalAlignment.Left));
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("AdjAmt",colWidthAdjAmt,HorizontalAlignment.Right));
			gridClaimAdjustments.Columns.Add(new UI.GridColumn("Remarks",widthCol,HorizontalAlignment.Left));
			gridClaimAdjustments.ListGridRows.Clear();
			_sumClaimAdjAmt=0;
			for(int i=0;i<_hx835_Claim.ListClaimAdjustments.Count;i++) {
				Hx835_Adj hx835_Adj=_hx835_Claim.ListClaimAdjustments[i];
				GridRow row=new GridRow();
				row.Tag=hx835_Adj;
				row.Cells.Add(new GridCell(hx835_Adj.ReasonDescript));//Reason
				row.Cells.Add(new GridCell(hx835_Adj.AdjAmt.ToString("f2")));//AdjAmt
				row.Cells.Add(new GridCell(hx835_Adj.AdjustRemarks));//Remarks
				_sumClaimAdjAmt+=_hx835_Claim.ListClaimAdjustments[i].AdjAmt;
				gridClaimAdjustments.ListGridRows.Add(row);
			}
			gridClaimAdjustments.EndUpdate();
			textClaimAdjAmtSum.Text=_sumClaimAdjAmt.ToString("f2");
		}

		private void FillProcedureBreakdown() {
			if(_hx835_Claim.ListProcs.Count==0) {
				gridProcedureBreakdown.Title="EOB Procedure Breakdown (None Reported)";
			}
			else {
				gridProcedureBreakdown.Title="EOB Procedure Breakdown";
			}
			gridProcedureBreakdown.BeginUpdate();
			gridProcedureBreakdown.Columns.Clear();
			gridProcedureBreakdown.Columns.Add(new GridColumn("ProcNum",80,HorizontalAlignment.Left));
			gridProcedureBreakdown.Columns.Add(new GridColumn("ProcCode",80,HorizontalAlignment.Center));
			GridColumn col=new GridColumn("ProcDescript",80,HorizontalAlignment.Left);
			col.IsWidthDynamic=true;
			gridProcedureBreakdown.Columns.Add(col);
			gridProcedureBreakdown.Columns.Add(new GridColumn("FeeBilled",70,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("InsPaid",70,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("PatPort",70,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Deduct",70,HorizontalAlignment.Right));
			gridProcedureBreakdown.Columns.Add(new GridColumn("Writeoff",70,HorizontalAlignment.Right));
			gridProcedureBreakdown.ListGridRows.Clear();
			_sumProcAdjAmt=0;
			for(int i=0;i<_hx835_Claim.ListProcs.Count;i++) {
				//Logic mimics SheetUtil.getTable_EraClaimsPaid(...)
				Hx835_Proc hx835_Proc=_hx835_Claim.ListProcs[i];
				GridRow row=new GridRow();
				row.Tag=hx835_Proc;
				if(hx835_Proc.ProcNum==0) {
					row.Cells.Add(new GridCell(""));//ProcNum
				}
				else {
					row.Cells.Add(new GridCell(hx835_Proc.ProcNum.ToString()));//ProcNum
				}
				row.Cells.Add(new GridCell(hx835_Proc.ProcCodeAdjudicated));//ProcCode
				string procDescript="";
				if(ProcedureCodes.IsValidCode(hx835_Proc.ProcCodeAdjudicated)) {
					ProcedureCode procedureCode=ProcedureCodes.GetProcCode(hx835_Proc.ProcCodeAdjudicated);
					procDescript=procedureCode.AbbrDesc;
				}
				row.Cells.Add(new GridCell(procDescript));//ProcDescript
				row.Cells.Add(new GridCell(hx835_Proc.ProcFee.ToString("f2")));//FeeBilled
				row.Cells.Add(new GridCell(hx835_Proc.InsPaid.ToString("f2")));//InsPaid
				row.Cells.Add(new GridCell(hx835_Proc.PatientPortionAmt.ToString("f2")));//PatPort
				row.Cells.Add(new GridCell(hx835_Proc.DeductibleAmt.ToString("f2")));//Deduct
				row.Cells.Add(new GridCell(hx835_Proc.WriteoffAmt.ToString("f2")));//Writeoff
				gridProcedureBreakdown.ListGridRows.Add(row);
			}
			gridProcedureBreakdown.EndUpdate();
			textProcAdjAmtSum.Text=_sumProcAdjAmt.ToString("f2");
		}

		private void FillAdjudicationInfo() {
			if(_hx835_Claim.ListAdjudicationInfo.Count==0) {
				gridAdjudicationInfo.Title="EOB Claim Adjudication Info (None Reported)";
			}
			else {
				gridAdjudicationInfo.Title="EOB Claim Adjudication Info";
			}
			gridAdjudicationInfo.BeginUpdate();
			gridAdjudicationInfo.Columns.Clear();
			gridAdjudicationInfo.Columns.Add(new UI.GridColumn("Description",gridAdjudicationInfo.Width/2,HorizontalAlignment.Left));
			gridAdjudicationInfo.Columns.Add(new UI.GridColumn("Value",gridAdjudicationInfo.Width/2,HorizontalAlignment.Left));
			gridAdjudicationInfo.ListGridRows.Clear();
			for(int i=0;i<_hx835_Claim.ListAdjudicationInfo.Count;i++) {
				Hx835_Info hx835_Info=_hx835_Claim.ListAdjudicationInfo[i];
				GridRow row=new GridRow();
				row.Tag=hx835_Info;
				row.Cells.Add(new UI.GridCell(hx835_Info.FieldName));//Description
				row.Cells.Add(new UI.GridCell(hx835_Info.FieldValue));//Value
				gridAdjudicationInfo.ListGridRows.Add(row);
			}
			gridAdjudicationInfo.EndUpdate();
		}

		private void FillSupplementalInfo() {
			if(_hx835_Claim.ListSupplementalInfo.Count==0) {
				gridSupplementalInfo.Title="EOB Supplemental Info (None Reported)";
			}
			else {
				gridSupplementalInfo.Title="EOB Supplemental Info";
			}
			gridSupplementalInfo.BeginUpdate();
			gridSupplementalInfo.Columns.Clear();
			const int colWidthAmt=80;
			int widthCol=gridSupplementalInfo.Width-10-colWidthAmt;
			gridSupplementalInfo.Columns.Add(new GridColumn("Description",widthCol,HorizontalAlignment.Left));
			gridSupplementalInfo.Columns.Add(new GridColumn("Amt",colWidthAmt,HorizontalAlignment.Right));
			gridSupplementalInfo.ListGridRows.Clear();
			for(int i=0;i<_hx835_Claim.ListSupplementalInfo.Count;i++) {
				Hx835_Info hx835_info=_hx835_Claim.ListSupplementalInfo[i];
				GridRow row=new GridRow();
				row.Tag=hx835_info;
				row.Cells.Add(hx835_info.FieldName);//Description
				row.Cells.Add(hx835_info.FieldValue);//Amount
				gridSupplementalInfo.ListGridRows.Add(row);
			}
			gridSupplementalInfo.EndUpdate();
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

		private void gridAdjudicationInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Info hx835_Info=(Hx835_Info)gridAdjudicationInfo.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(hx835_Info.FieldName+"\r\n"+hx835_Info.FieldValue);
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridSupplementalInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Info hx835_Info=(Hx835_Info)gridSupplementalInfo.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(hx835_Info.FieldName+"\r\n"+hx835_Info.FieldValue);
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}
		
		private void butPrint_Click(object sender,EventArgs e) {
			Sheet sheet=SheetUtil.CreateSheet(SheetDefs.GetInternalOrCustom(SheetInternalType.ERA));
			X835 x835=_x835.Copy();
			x835.ListClaimsPaid=new List<Hx835_Claim>();
			x835.ListClaimsPaid.Add(_hx835_Claim); //Only print the current claim.
			SheetParameter.GetParamByName(sheet.Parameters,"ERA").ParamValue=x835;//Required param
			SheetParameter.GetParamByName(sheet.Parameters,"IsSingleClaimPaid").ParamValue=true;//Value is null if not set
			SheetFiller.FillFields(sheet);
			SheetPrinting.Print(sheet,isPreviewMode:true);
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}
		
	}
}