using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans835ProcEdit:FormODBase {

		private Hx835_Proc _proc;
		private decimal _patRespSum;
		private decimal _contractualObligationSum;
		private decimal _payorInitiatedReductionSum;
		private decimal _otherAdjustmentSum;
		#region Printing variables
		private int _gridPageCur;
		private int _yPosCur;
		private EraProcPrintingProgress _gridPrintProgress;
		#endregion

		public FormEtrans835ProcEdit(Hx835_Proc proc) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_proc=proc;
		}

		private void FormEtrans835ClaimEdit_Load(object sender,EventArgs e) {
			FillAll();
		}

		private void FormEtrans835ClaimEdit_Resize(object sender,EventArgs e) {
			FillProcedureAdjustments();//Because the grid columns change size depending on the form size.
			FillRemarks();//Because the grid columns change size depending on the form size.
			FillSupplementalInfo();//Because the grid columns change size depending on the form size.
		}

		private void FillAll() {
			FillProcedureAdjustments();
			FillHeader();//Must be after FillProcedureAdjustments().
			FillRemarks();
			FillSupplementalInfo();
		}

		private void FillHeader() {
			Text="Procedure Paid - Patient: "+_proc.ClaimPaid.PatientName;
			textProcAdjudicated.Text=_proc.ProcCodeAdjudicated;
			if(ProcedureCodes.IsValidCode(_proc.ProcCodeAdjudicated)) {
				textProcAdjudicated.Text=_proc.ProcCodeAdjudicated+" - "+ProcedureCodes.GetProcCode(_proc.ProcCodeAdjudicated).AbbrDesc;
			}
			textProcSubmitted.Text=_proc.ProcCodeBilled;
			if(ProcedureCodes.IsValidCode(_proc.ProcCodeBilled)) {
				textProcSubmitted.Text=_proc.ProcCodeBilled+" - "+ProcedureCodes.GetProcCode(_proc.ProcCodeBilled).AbbrDesc;
			}
			textDateService.Text=_proc.DateServiceStart.ToShortDateString();
			if(_proc.DateServiceEnd>_proc.DateServiceStart) {
				textDateService.Text+=" to "+_proc.DateServiceEnd.ToShortDateString();
				LayoutManager.MoveWidth(textDateService,160);//Increase width to accout for extra text.
			}
			textInsPaid.Text=_proc.InsPaid.ToString("f2");
			if(_proc.ProcNum==0) {
				textProcNum.Text="";
			}
			else {
				textProcNum.Text=_proc.ProcNum.ToString();
			}
			textProcFee.Text=_proc.ProcFee.ToString("f2");			
			textInsPaidCalc.Text=(_proc.ProcFee-_patRespSum-_contractualObligationSum-_payorInitiatedReductionSum-_otherAdjustmentSum).ToString("f2");
		}

		private void FillProcedureAdjustments() {
			if(_proc.ListProcAdjustments.Count==0) {
				gridProcedureAdjustments.Title="Procedure Adjustments (None Reported)";
			}
			else {
				gridProcedureAdjustments.Title="Procedure Adjustments";
			}
			gridProcedureAdjustments.BeginUpdate();
			gridProcedureAdjustments.ListGridColumns.Clear();
			const int colWidthDescription=200;
			const int colWidthAdjAmt=80;
			int colWidthVariable=gridProcedureAdjustments.Width-10-colWidthDescription-colWidthAdjAmt;
			gridProcedureAdjustments.ListGridColumns.Add(new UI.GridColumn("Description",colWidthDescription,HorizontalAlignment.Left));
			gridProcedureAdjustments.ListGridColumns.Add(new UI.GridColumn("Reason",colWidthVariable,HorizontalAlignment.Left));
			gridProcedureAdjustments.ListGridColumns.Add(new UI.GridColumn("AdjAmt",colWidthAdjAmt,HorizontalAlignment.Right));
			gridProcedureAdjustments.ListGridRows.Clear();
			_patRespSum=0;
			_contractualObligationSum=0;
			_payorInitiatedReductionSum=0;
			_otherAdjustmentSum=0;
			for(int i=0;i<_proc.ListProcAdjustments.Count;i++) {
				Hx835_Adj adj=_proc.ListProcAdjustments[i];
				GridRow row=new GridRow();
				row.Tag=adj;
				row.Cells.Add(new GridCell(adj.AdjustRemarks));//Remarks
				row.Cells.Add(new GridCell(adj.ReasonDescript));//Reason
				row.Cells.Add(new GridCell(adj.AdjAmt.ToString("f2")));//AdjAmt
				if(adj.AdjCode=="PR") {//Patient Responsibility
					_patRespSum+=adj.AdjAmt;
				}
				else if(adj.AdjCode=="CO") {//Contractual Obligations
					_contractualObligationSum+=adj.AdjAmt;
				}
				else if(adj.AdjCode=="PI") {//Payor Initiated Reductions
					_payorInitiatedReductionSum+=adj.AdjAmt;
				}
				else {//Other Adjustments
					_otherAdjustmentSum+=adj.AdjAmt;
				}
				gridProcedureAdjustments.ListGridRows.Add(row);
			}
			gridProcedureAdjustments.EndUpdate();
			textPatRespSum.Text=_patRespSum.ToString("f2");
			textContractualObligSum.Text=_contractualObligationSum.ToString("f2");
			textPayorReductionSum.Text=_payorInitiatedReductionSum.ToString("f2");
			textOtherAdjustmentSum.Text=_otherAdjustmentSum.ToString("f2");
		}

		private void FillRemarks() {
			if(_proc.ListRemarks.Count==0) {
				gridRemarks.Title="Remarks (None Reported)";
			}
			else {
				gridRemarks.Title="Remarks";
			}
			gridRemarks.BeginUpdate();
			gridRemarks.ListGridColumns.Clear();
			gridRemarks.ListGridColumns.Add(new UI.GridColumn("",gridRemarks.Width,HorizontalAlignment.Left));
			gridRemarks.ListGridRows.Clear();
			for(int i=0;i<_proc.ListRemarks.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=_proc.ListRemarks[i].Value;
				row.Cells.Add(new UI.GridCell(_proc.ListRemarks[i].Value));
				gridRemarks.ListGridRows.Add(row);
			}
			gridRemarks.EndUpdate();
		}

		private void FillSupplementalInfo() {
			if(_proc.ListSupplementalInfo.Count==0) {
				gridSupplementalInfo.Title="Supplemental Info (None Reported)";
			}
			else {
				gridSupplementalInfo.Title="Supplemental Info";
			}
			gridSupplementalInfo.BeginUpdate();
			gridSupplementalInfo.ListGridColumns.Clear();
			const int colWidthAmt=80;
			int colWidthVariable=gridSupplementalInfo.Width-10-colWidthAmt;
			gridSupplementalInfo.ListGridColumns.Add(new GridColumn("Description",colWidthVariable,HorizontalAlignment.Left));
			gridSupplementalInfo.ListGridColumns.Add(new GridColumn("Amt",colWidthAmt,HorizontalAlignment.Right));
			gridSupplementalInfo.ListGridRows.Clear();
			for(int i=0;i<_proc.ListSupplementalInfo.Count;i++) {
				Hx835_Info info=_proc.ListSupplementalInfo[i];
				GridRow row=new GridRow();
				row.Tag=info;
				row.Cells.Add(info.FieldName);//Description
				row.Cells.Add(info.FieldValue);//Amount
				gridSupplementalInfo.ListGridRows.Add(row);
			}
			gridSupplementalInfo.EndUpdate();
		}

		private void gridProcedureAdjustments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Adj adj=(Hx835_Adj)gridProcedureAdjustments.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(adj.AdjCode+" "+adj.AdjustRemarks+"\r\r"+adj.ReasonDescript+"\r\n"+adj.AdjAmt.ToString("f2"));
			msgbox.Show(this);//This window is just used to display information.
		}

		private void gridRemarks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			string remark=(string)gridRemarks.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(remark);
			msgbox.Show(this);//This window is just used to display information.
		}

		private void gridSupplementalInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Info info=(Hx835_Info)gridSupplementalInfo.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgbox=new MsgBoxCopyPaste(info.FieldName+"\r\n"+info.FieldValue);
			msgbox.Show(this);//This window is just used to display information.
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_gridPageCur=0;
			_gridPrintProgress=EraProcPrintingProgress.DocumentHeader;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Printed 835 Procedure")+((_proc.ProcNum==0)?"":(" "+_proc.ProcNum)),
				printoutOrientation:PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			Graphics g=e.Graphics;
			int y=bounds.Top;
			int midX=bounds.X+bounds.Width/2;
			#region printHeading
			if(_gridPrintProgress==EraProcPrintingProgress.DocumentHeader) {//Only print the document heading on the first page.
				string text;
				using(Font font=new Font("Arial",13,FontStyle.Bold)) {
					_yPosCur=0;
					text=Lan.g(this,"835 Procedure:")+((_proc.ProcNum==0) ? "" : " "+_proc.ProcNum);
					g.DrawString(text,font,Brushes.Black,midX-g.MeasureString(text,font).Width/2,y);
					y+=25;
					text=Lan.g(this,"Patient:")+" "+_proc.ClaimPaid.PatientName;
					g.DrawString(text,font,Brushes.Black,midX-g.MeasureString(text,font).Width/2,y);
					y+=25;
				}
				using(Font font=new Font("Arial",9,FontStyle.Bold)) {
					text=Lan.g(this,"Proc Adjudicated:")+" "+textProcAdjudicated.Text+"\t\t"+Lan.g(this,"Date Service:")+" "+textDateService.Text;
					g.DrawString(text,font,Brushes.Black,midX-g.MeasureString(text,font).Width/2,y);
					y+=20;
					text=Lan.g(this,"Proc Submitted:")+" "+textProcSubmitted.Text+"\t\t"+Lan.g(this,"Ins Paid:")+" "+textInsPaid.Text;
					g.DrawString(text,font,Brushes.Black,midX-g.MeasureString(text,font).Width/2,y);
					y+=25;//Extra 5 pixels to visually group grid header with grid.
					text=groupBalancing.Text;//Translated
					g.DrawString(text,font,Brushes.Black,midX-g.MeasureString(text,font).Width/2,y);
					y+=20;
				}
				#region In memory balancing grid
				//int colWidth=gridProcedureAdjustments.WidthAllColumns/6;
				GridOD grid=new GridOD();
				grid.Width=gridProcedureAdjustments.Width;
				grid.BeginUpdate();
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Proc Fee")+" -",40){ IsWidthDynamic=true });
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Patient Resp Sum")+" -",40){ IsWidthDynamic=true });
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Contractual Oblig. Sum")+" -",40){ IsWidthDynamic=true });
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Payor Reduction Sum")+" -",40){ IsWidthDynamic=true });
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Other Adjustment Sum")+" =",40){ IsWidthDynamic=true });
				grid.ListGridColumns.Add(new GridColumn(Lan.g(this,"Ins Paid Calc"),40){ IsWidthDynamic=true });
				//grid.ComputeColumns();
				grid.ListGridRows.Add(new GridRow(textProcFee.Text,
					textPatRespSum.Text,
					textContractualObligSum.Text,
					textPayorReductionSum.Text,
					textOtherAdjustmentSum.Text,
					textInsPaidCalc.Text)
				);
				grid.EndUpdate();
				#endregion
				y=grid.PrintPage(g,_gridPageCur,bounds,y,true);
				grid.Dispose();
				TransitionPrintState();
				_yPosCur=y;
			}
			#endregion
			#region GridProcedureAdjustments
			if(_gridPrintProgress==EraProcPrintingProgress.GridProcAdjHeader) {//Only draw header once.
				PrintGridHeaderHelper(midX,g,gridProcedureAdjustments.Title);
			}
			if(_gridPrintProgress==EraProcPrintingProgress.GridProcAdj) {
				PrintGridHelper(gridProcedureAdjustments,bounds,g,e);
			}
			#endregion
			#region GridRemarks
			if(_gridPrintProgress==EraProcPrintingProgress.GridRemarksHeader) {//Only draw header once
				PrintGridHeaderHelper(midX,g,gridRemarks.Title);
			}
			if(_gridPrintProgress==EraProcPrintingProgress.GridRemarks) {
				PrintGridHelper(gridRemarks,bounds,g,e);
			}
			#endregion
			#region GridSupplementalInfo
			if(_gridPrintProgress==EraProcPrintingProgress.GridSupplementalHeader) {//Only draw header once
				PrintGridHeaderHelper(midX,g,gridSupplementalInfo.Title);
			}
			if(_gridPrintProgress==EraProcPrintingProgress.GridSupplemental) {
				PrintGridHelper(gridSupplementalInfo,bounds,g,e);
			}
			#endregion
			//g.Dispose();//dg System provided object which will be disposed by the system.
		}

		///<summary>Transitions the printing state after printing the title.
		///There is no validation that the vertical position of this title will not cause the title to be truncated by the bottom of the page.</summary>
		private void PrintGridHeaderHelper(int midX,Graphics g,string title) {
			//Curently does not check to see if text will fit on current page.  We can enhance this latter if we need to.
			_yPosCur+=15;
			using(Font font=new Font("Arial",10,FontStyle.Bold)) {
				g.DrawString(title,font,Brushes.Black,midX-g.MeasureString(title,font).Width/2,_yPosCur);
			}
			_yPosCur+=20;
			_gridPageCur=0;//Reset so that ODGrid.PrintPage(...) fields are reset correctly when printing next grid.
			TransitionPrintState();
		}

		///<summary>Transitions the printing state, and sets _yPosCur after finishing printing to save next header/grid starting point.</summary>
		private void PrintGridHelper(GridOD grid,Rectangle bounds,Graphics g,PrintPageEventArgs e) {
			int y=grid.PrintPage(g,_gridPageCur,bounds,_yPosCur);
			if(y==-1) {//Additional page needed to print the rest of the grid.
				e.HasMorePages=true;//Triggers pd_PrintPage(...) to fire again.
				_gridPageCur++;
			}
			else {//Done printing grid
				_yPosCur=y;
				TransitionPrintState();	
			}
		}

		///<summary>Increments the print state to the next state in the EraProcPrintingProgress enum.</summary>
		private void TransitionPrintState() {
			_gridPrintProgress=(EraProcPrintingProgress)(((int)_gridPrintProgress)+1);//Transition
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
			Close();
		}

		///<summary>This enum represents the printing state.
		///The printing state starts at option 0 and increments by 1 each time a state is completed.</summary>
		private enum EraProcPrintingProgress {
			///<summary>0</summary>
			DocumentHeader,
			///<summary>1</summary>
			GridProcAdjHeader,
			///<summary>2</summary>
			GridProcAdj,
			///<summary>3</summary>
			GridRemarksHeader,
			///<summary>4</summary>
			GridRemarks,
			///<summary>5</summary>
			GridSupplementalHeader,
			///<summary>6</summary>
			GridSupplemental,
			///<summary>7</summary>
			Done
		}
	}
}