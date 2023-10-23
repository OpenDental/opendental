using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental {
	public partial class FormEtrans835ProcEdit:FormODBase {

		private Hx835_Proc _hx835_Proc;
		private decimal _sumPatResponsibility;
		private decimal _sumContractualObligation;
		private decimal _sumPayorInitiatedReduction;
		private decimal _sumOtherAdjustments;
		#region Printing variables
		private int _pageNumber;
		private int _yPos;
		private EraProcPrintingProgress _eraProcPrintingProgress;
		#endregion

		public FormEtrans835ProcEdit(Hx835_Proc hx835_Proc) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_hx835_Proc=hx835_Proc;
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
			Text="Procedure Paid - Patient: "+_hx835_Proc.ClaimPaid.PatientName;
			textProcAdjudicated.Text=_hx835_Proc.ProcCodeAdjudicated;
			if(ProcedureCodes.IsValidCode(_hx835_Proc.ProcCodeAdjudicated)) {
				textProcAdjudicated.Text=_hx835_Proc.ProcCodeAdjudicated+" - "+ProcedureCodes.GetProcCode(_hx835_Proc.ProcCodeAdjudicated).AbbrDesc;
			}
			textProcSubmitted.Text=_hx835_Proc.ProcCodeBilled;
			if(ProcedureCodes.IsValidCode(_hx835_Proc.ProcCodeBilled)) {
				textProcSubmitted.Text=_hx835_Proc.ProcCodeBilled+" - "+ProcedureCodes.GetProcCode(_hx835_Proc.ProcCodeBilled).AbbrDesc;
			}
			textDateService.Text=_hx835_Proc.DateServiceStart.ToShortDateString();
			if(_hx835_Proc.DateServiceEnd>_hx835_Proc.DateServiceStart) {
				textDateService.Text+=" to "+_hx835_Proc.DateServiceEnd.ToShortDateString();
				LayoutManager.MoveWidth(textDateService,160);//Increase width to accout for extra text.
			}
			textInsPaid.Text=_hx835_Proc.InsPaid.ToString("f2");
			if(_hx835_Proc.ProcNum==0) {
				textProcNum.Text="";
			}
			else {
				textProcNum.Text=_hx835_Proc.ProcNum.ToString();
			}
			textProcFee.Text=_hx835_Proc.ProcFee.ToString("f2");			
			textInsPaidCalc.Text=(_hx835_Proc.ProcFee-_sumPatResponsibility-_sumContractualObligation-_sumPayorInitiatedReduction-_sumOtherAdjustments).ToString("f2");
		}

		private void FillProcedureAdjustments() {
			if(_hx835_Proc.ListProcAdjustments.Count==0) {
				gridProcedureAdjustments.Title="Procedure Adjustments (None Reported)";
			}
			else {
				gridProcedureAdjustments.Title="Procedure Adjustments";
			}
			gridProcedureAdjustments.BeginUpdate();
			gridProcedureAdjustments.Columns.Clear();
			const int colWidthDescription=200;
			const int colWidthAdjAmt=80;
			int widthCol=gridProcedureAdjustments.Width-10-colWidthDescription-colWidthAdjAmt;
			gridProcedureAdjustments.Columns.Add(new UI.GridColumn("Description",colWidthDescription,HorizontalAlignment.Left));
			gridProcedureAdjustments.Columns.Add(new UI.GridColumn("Reason",widthCol,HorizontalAlignment.Left));
			gridProcedureAdjustments.Columns.Add(new UI.GridColumn("AdjAmt",colWidthAdjAmt,HorizontalAlignment.Right));
			gridProcedureAdjustments.ListGridRows.Clear();
			_sumPatResponsibility=0;
			_sumContractualObligation=0;
			_sumPayorInitiatedReduction=0;
			_sumOtherAdjustments=0;
			for(int i=0;i<_hx835_Proc.ListProcAdjustments.Count;i++) {
				Hx835_Adj hx835_Adj=_hx835_Proc.ListProcAdjustments[i];
				GridRow row=new GridRow();
				row.Tag=hx835_Adj;
				row.Cells.Add(new GridCell(hx835_Adj.AdjustRemarks));//Remarks
				row.Cells.Add(new GridCell(hx835_Adj.ReasonDescript));//Reason
				row.Cells.Add(new GridCell(hx835_Adj.AdjAmt.ToString("f2")));//AdjAmt
				if(hx835_Adj.AdjCode=="PR") {//Patient Responsibility
					_sumPatResponsibility+=hx835_Adj.AdjAmt;
				}
				else if(hx835_Adj.AdjCode=="CO") {//Contractual Obligations
					_sumContractualObligation+=hx835_Adj.AdjAmt;
				}
				else if(hx835_Adj.AdjCode=="PI") {//Payor Initiated Reductions
					_sumPayorInitiatedReduction+=hx835_Adj.AdjAmt;
				}
				else {//Other Adjustments
					_sumOtherAdjustments+=hx835_Adj.AdjAmt;
				}
				gridProcedureAdjustments.ListGridRows.Add(row);
			}
			gridProcedureAdjustments.EndUpdate();
			textPatRespSum.Text=_sumPatResponsibility.ToString("f2");
			textContractualObligSum.Text=_sumContractualObligation.ToString("f2");
			textPayorReductionSum.Text=_sumPayorInitiatedReduction.ToString("f2");
			textOtherAdjustmentSum.Text=_sumOtherAdjustments.ToString("f2");
		}

		private void FillRemarks() {
			if(_hx835_Proc.ListRemarks.Count==0) {
				gridRemarks.Title="Remarks (None Reported)";
			}
			else {
				gridRemarks.Title="Remarks";
			}
			gridRemarks.BeginUpdate();
			gridRemarks.Columns.Clear();
			gridRemarks.Columns.Add(new UI.GridColumn("",gridRemarks.Width,HorizontalAlignment.Left));
			gridRemarks.ListGridRows.Clear();
			for(int i=0;i<_hx835_Proc.ListRemarks.Count;i++) {
				GridRow row=new GridRow();
				row.Tag=_hx835_Proc.ListRemarks[i].Value;
				row.Cells.Add(new UI.GridCell(_hx835_Proc.ListRemarks[i].Value));
				gridRemarks.ListGridRows.Add(row);
			}
			gridRemarks.EndUpdate();
		}

		private void FillSupplementalInfo() {
			if(_hx835_Proc.ListSupplementalInfo.Count==0) {
				gridSupplementalInfo.Title="Supplemental Info (None Reported)";
			}
			else {
				gridSupplementalInfo.Title="Supplemental Info";
			}
			gridSupplementalInfo.BeginUpdate();
			gridSupplementalInfo.Columns.Clear();
			const int colWidthAmt=80;
			int widthCol=gridSupplementalInfo.Width-10-colWidthAmt;
			gridSupplementalInfo.Columns.Add(new GridColumn("Description",widthCol,HorizontalAlignment.Left));
			gridSupplementalInfo.Columns.Add(new GridColumn("Amt",colWidthAmt,HorizontalAlignment.Right));
			gridSupplementalInfo.ListGridRows.Clear();
			for(int i=0;i<_hx835_Proc.ListSupplementalInfo.Count;i++) {
				Hx835_Info hx835_Info=_hx835_Proc.ListSupplementalInfo[i];
				GridRow row=new GridRow();
				row.Tag=hx835_Info;
				row.Cells.Add(hx835_Info.FieldName);//Description
				row.Cells.Add(hx835_Info.FieldValue);//Amount
				gridSupplementalInfo.ListGridRows.Add(row);
			}
			gridSupplementalInfo.EndUpdate();
		}

		private void gridProcedureAdjustments_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Adj hx835_Adj=(Hx835_Adj)gridProcedureAdjustments.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(hx835_Adj.AdjCode+" "+hx835_Adj.AdjustRemarks+"\r\r"+hx835_Adj.ReasonDescript+"\r\n"+hx835_Adj.AdjAmt.ToString("f2"));
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridRemarks_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			string remark=(string)gridRemarks.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(remark);
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void gridSupplementalInfo_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Hx835_Info hx835_Info=(Hx835_Info)gridSupplementalInfo.ListGridRows[e.Row].Tag;
			MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(hx835_Info.FieldName+"\r\n"+hx835_Info.FieldValue);
			msgBoxCopyPaste.Show(this);//This window is just used to display information.
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pageNumber=0;
			_eraProcPrintingProgress=EraProcPrintingProgress.DocumentHeader;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Printed 835 Procedure")+((_hx835_Proc.ProcNum==0)?"":(" "+_hx835_Proc.ProcNum)),
				printoutOrientation:PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Rectangle rectangle=e.MarginBounds;
			Graphics g=e.Graphics;
			int yPos=rectangle.Top;
			int midX=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(_eraProcPrintingProgress==EraProcPrintingProgress.DocumentHeader) {//Only print the document heading on the first page.
				string text;
				using Font fontLarge=new Font("Arial",13,FontStyle.Bold);
				_yPos=0;
				text=Lan.g(this,"835 Procedure:")+((_hx835_Proc.ProcNum==0) ? "" : " "+_hx835_Proc.ProcNum);
				g.DrawString(text,fontLarge,Brushes.Black,midX-g.MeasureString(text,fontLarge).Width/2,yPos);
				yPos+=25;
				text=Lan.g(this,"Patient:")+" "+_hx835_Proc.ClaimPaid.PatientName;
				g.DrawString(text,fontLarge,Brushes.Black,midX-g.MeasureString(text,fontLarge).Width/2,yPos);
				yPos+=25;
				using Font fontSmall=new Font("Arial",9,FontStyle.Bold);
				text=Lan.g(this,"Proc Adjudicated:")+" "+textProcAdjudicated.Text+"\t\t"+Lan.g(this,"Date Service:")+" "+textDateService.Text;
				g.DrawString(text,fontSmall,Brushes.Black,midX-g.MeasureString(text,fontSmall).Width/2,yPos);
				yPos+=20;
				text=Lan.g(this,"Proc Submitted:")+" "+textProcSubmitted.Text+"\t\t"+Lan.g(this,"Ins Paid:")+" "+textInsPaid.Text;
				g.DrawString(text,fontSmall,Brushes.Black,midX-g.MeasureString(text,fontSmall).Width/2,yPos);
				yPos+=25;//Extra 5 pixels to visually group grid header with grid.
				text=groupBalancing.Text;//Translated
				g.DrawString(text,fontSmall,Brushes.Black,midX-g.MeasureString(text,fontSmall).Width/2,yPos);
				yPos+=20;
				#region In memory balancing grid
				//int colWidth=gridProcedureAdjustments.WidthAllColumns/6;
				GridOD grid=new GridOD();
				grid.Width=gridProcedureAdjustments.Width;
				grid.BeginUpdate();
				GridColumn col;
				col=new GridColumn(Lan.g(this,"Proc Fee")+" -",40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Patient Resp Sum")+" -",40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Contractual Oblig. Sum")+" -",40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Payor Reduction Sum")+" -",40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Other Adjustment Sum")+" =",40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
				col=new GridColumn(Lan.g(this,"Ins Paid Calc"),40);
				col.IsWidthDynamic=true;
				grid.Columns.Add(col);
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
				yPos=grid.PrintPage(g,_pageNumber,rectangle,yPos,HasHeaderSpaceOnEveryPage:true);
				grid.Dispose();
				TransitionPrintState();
				_yPos=yPos;
			}
			#endregion
			#region GridProcedureAdjustments
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridProcAdjHeader) {//Only draw header once.
				PrintGridHeaderHelper(midX,g,gridProcedureAdjustments.Title);
			}
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridProcAdj) {
				PrintGridHelper(gridProcedureAdjustments,rectangle,g,e);
			}
			#endregion
			#region GridRemarks
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridRemarksHeader) {//Only draw header once
				PrintGridHeaderHelper(midX,g,gridRemarks.Title);
			}
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridRemarks) {
				PrintGridHelper(gridRemarks,rectangle,g,e);
			}
			#endregion
			#region GridSupplementalInfo
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridSupplementalHeader) {//Only draw header once
				PrintGridHeaderHelper(midX,g,gridSupplementalInfo.Title);
			}
			if(_eraProcPrintingProgress==EraProcPrintingProgress.GridSupplemental) {
				PrintGridHelper(gridSupplementalInfo,rectangle,g,e);
			}
			#endregion
			//g.Dispose();//dg System provided object which will be disposed by the system.
		}

		///<summary>Transitions the printing state after printing the title.
		///There is no validation that the vertical position of this title will not cause the title to be truncated by the bottom of the page.</summary>
		private void PrintGridHeaderHelper(int midX,Graphics g,string title) {
			//Curently does not check to see if text will fit on current page.  We can enhance this latter if we need to.
			_yPos+=15;
			using Font font=new Font("Arial",10,FontStyle.Bold);
			g.DrawString(title,font,Brushes.Black,midX-g.MeasureString(title,font).Width/2,_yPos);
			_yPos+=20;
			_pageNumber=0;//Reset so that ODGrid.PrintPage(...) fields are reset correctly when printing next grid.
			TransitionPrintState();
		}

		///<summary>Transitions the printing state, and sets _yPosCur after finishing printing to save next header/grid starting point.</summary>
		private void PrintGridHelper(GridOD grid,Rectangle rectangle,Graphics g,PrintPageEventArgs e) {
			int y=grid.PrintPage(g,_pageNumber,rectangle,_yPos);
			if(y==-1) {//Additional page needed to print the rest of the grid.
				e.HasMorePages=true;//Triggers pd_PrintPage(...) to fire again.
				_pageNumber++;
			}
			else {//Done printing grid
				_yPos=y;
				TransitionPrintState();	
			}
		}

		///<summary>Increments the print state to the next state in the EraProcPrintingProgress enum.</summary>
		private void TransitionPrintState() {
			_eraProcPrintingProgress=(EraProcPrintingProgress)(((int)_eraProcPrintingProgress)+1);//Transition
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