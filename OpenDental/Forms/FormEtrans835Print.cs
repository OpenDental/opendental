using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEtrans835Print:FormODBase {

		private X835 _x835;
		private Hx835_Claim _claimPaid;
		private int _pagesPrintedCount;
		private bool _isHeadingPrinted;
		private int _headingPrintY;


		///<summary>Set x835 to view all x835.ListClaims info. Otherwise set claimPaid to a specific Hx835_Claim to view.</summary>
		public FormEtrans835Print(X835 x835,Hx835_Claim claimPaid=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_x835=x835;
			_claimPaid=claimPaid;
		}

		private void FormEtrans835Print_Load(object sender,EventArgs e) {
			FillGridMain();
		}

		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new UI.GridColumn("EnterBy",50,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new UI.GridColumn("Claim",340,HorizontalAlignment.Left) { IsWidthDynamic=true });
			gridMain.ListGridColumns.Add(new UI.GridColumn("Date",66,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new UI.GridColumn("Code",40,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new UI.GridColumn("CodeBill",56,HorizontalAlignment.Center));
			gridMain.ListGridColumns.Add(new UI.GridColumn("Billed",56,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new UI.GridColumn("PatResp",48,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new UI.GridColumn("Allowed",56,HorizontalAlignment.Right));
			gridMain.ListGridColumns.Add(new UI.GridColumn("InsPay",56,HorizontalAlignment.Right));
			gridMain.ListGridRows.Clear();
			List<Hx835_Claim> listClaims;
			if(_x835!=null) {
				listClaims=_x835.ListClaimsPaid;
			}
			else {
				listClaims=new List<Hx835_Claim>() { _claimPaid };
			}
			for(int i=0;i<listClaims.Count;i++) {
				Hx835_Claim claimPaid=listClaims[i];
				UI.GridRow rowClaim=new UI.GridRow();
				//If there is no procedure detail, then the user will need to enter by total, because they will not know the procedure amounts paid.
				if(claimPaid.ListProcs.Count==0) {
					rowClaim.Cells.Add(new UI.GridCell("Total"));//EnterBy
				}
				//If there is procedure detail, and there are also claim level adjustments, then the user will need to enter the procedure amounts by procedure and the claim adjustment by total.
				else if(claimPaid.ClaimAdjustmentTotal!=0) {
					rowClaim.Cells.Add(new UI.GridCell("Proc &\r\nTotal"));//EnterBy
				}
				//If there is procedure detail, and there are no claim level adjustments, the user will need to enter the payments by procedure only.
				else {
					rowClaim.Cells.Add(new UI.GridCell("Proc"));//EnterBy
				}
				string strClaim="Patient: "+claimPaid.PatientName;
				if(claimPaid.SubscriberName!=claimPaid.PatientName) {
					strClaim+="\r\nSubscriber: "+claimPaid.SubscriberName;
				}
				if(claimPaid.ClaimTrackingNumber!="") {
					strClaim+="\r\nClaim Identifier: "+claimPaid.ClaimTrackingNumber;
				}
				if(claimPaid.PayerControlNumber!="") {
					strClaim+="\r\nPayer Control Number: "+claimPaid.PayerControlNumber;
				}
				if(claimPaid.ListProcs.Count>0 && claimPaid.ClaimAdjustmentTotal!=0) {
					//If there is no procedure detail, then the user will need to enter the claim payment by total.  In this case, the user only cares about the InsPaid for the entire claim.  Showing the adjustments would cause user confusion.
					//If there is procedure detail, then we need to show the claim adjustment total, because the user will need to enter this amount by total in addition to any procedure amounts entered.
					strClaim+="\r\nClaim Adjustments: "+claimPaid.ClaimAdjustmentTotal.ToString("f2");
				}
				rowClaim.Cells.Add(new UI.GridCell(strClaim));//Claim
				string strDateClaim=claimPaid.DateServiceStart.ToShortDateString();
				if(claimPaid.DateServiceEnd.Year>1880) {
					strDateClaim+=" to \r\n"+claimPaid.DateServiceEnd.ToShortDateString();
				}
				rowClaim.Cells.Add(new UI.GridCell(strDateClaim));//Date
				rowClaim.Cells.Add(new UI.GridCell(""));//Code
				rowClaim.Cells.Add(new UI.GridCell(""));//CodeBilled
				rowClaim.Cells.Add(new UI.GridCell(claimPaid.ClaimFee.ToString("f2")));//Billed
				rowClaim.Cells.Add(new UI.GridCell(claimPaid.PatientRespAmt.ToString("f2")));//PatResp
				rowClaim.Cells.Add(new UI.GridCell(""));//Allowed
				rowClaim.Cells.Add(new UI.GridCell(claimPaid.InsPaid.ToString("f2")));//InsPay
				gridMain.ListGridRows.Add(rowClaim);
				for(int j=0;j<claimPaid.ListProcs.Count;j++) {
					Hx835_Proc proc=claimPaid.ListProcs[j];
					UI.GridRow rowProc=new UI.GridRow();
					rowProc.Cells.Add(new UI.GridCell(""));//EnterBy
					rowProc.Cells.Add(new UI.GridCell(""));//Claim
					string strDateProc=proc.DateServiceStart.ToShortDateString();
					if(proc.DateServiceEnd.Year>1880) {
						strDateProc+=" to \r\n"+proc.DateServiceEnd.ToShortDateString();
					}
					rowProc.Cells.Add(new UI.GridCell(strDateProc));//Date
					rowProc.Cells.Add(new UI.GridCell(proc.ProcCodeAdjudicated));//Code
					rowProc.Cells.Add(new UI.GridCell(proc.ProcCodeBilled));//CodeBilled
					rowProc.Cells.Add(new UI.GridCell(proc.ProcFee.ToString("f2")));//Billed
					rowProc.Cells.Add(new UI.GridCell(proc.PatRespTotal.ToString("f2")));//PatResp
					rowProc.Cells.Add(new UI.GridCell(proc.AllowedAmt.ToString("f2")));//Allowed
					rowProc.Cells.Add(new UI.GridCell(proc.InsPaid.ToString("f2")));//InsPay
					gridMain.ListGridRows.Add(rowProc);
				}
			}
			gridMain.EndUpdate();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrintedCount=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,
				Lan.g(this,"Electronic remittance advice (ERA) printed"),
				PrintoutOrientation.Portrait,
				margins:new Margins(25,25,50,50)
			);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Electronic Remittance Advice (ERA)");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				yPos+=20;
				_isHeadingPrinted=true;
				_headingPrintY=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrintedCount,bounds,_headingPrintY);
			_pagesPrintedCount++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}

	}
}