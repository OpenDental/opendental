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
		private Hx835_Claim _hx835_Claim;
		private int _pagesPrintedCount;
		private bool _isHeadingPrinted;
		private int _headingPrintY;


		///<summary>Set x835 to view all x835.ListClaims info. Otherwise set claimPaid to a specific Hx835_Claim to view.</summary>
		public FormEtrans835Print(X835 x835,Hx835_Claim hx835_Claim=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_x835=x835;
			_hx835_Claim=hx835_Claim;
		}

		private void FormEtrans835Print_Load(object sender,EventArgs e) {
			FillGridMain();
		}

		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new UI.GridColumn("EnterBy",50,HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn("Claim",340,HorizontalAlignment.Left) { IsWidthDynamic=true });
			gridMain.Columns.Add(new UI.GridColumn("Date",66,HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn("Code",40,HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn("CodeBill",56,HorizontalAlignment.Center));
			gridMain.Columns.Add(new UI.GridColumn("Billed",56,HorizontalAlignment.Right));
			gridMain.Columns.Add(new UI.GridColumn("PatResp",48,HorizontalAlignment.Right));
			gridMain.Columns.Add(new UI.GridColumn("Allowed",56,HorizontalAlignment.Right));
			gridMain.Columns.Add(new UI.GridColumn("InsPay",56,HorizontalAlignment.Right));
			gridMain.ListGridRows.Clear();
			List<Hx835_Claim> listHx835_Claims;
			if(_x835!=null) {
				listHx835_Claims=_x835.ListClaimsPaid;
			}
			else {
				listHx835_Claims=new List<Hx835_Claim>() { _hx835_Claim };
			}
			for(int i=0;i<listHx835_Claims.Count;i++) {
				Hx835_Claim hx835_Claim=listHx835_Claims[i];
				UI.GridRow row=new UI.GridRow();
				//If there is no procedure detail, then the user will need to enter by total, because they will not know the procedure amounts paid.
				if(hx835_Claim.ListProcs.Count==0) {
					row.Cells.Add(new UI.GridCell("Total"));//EnterBy
				}
				//If there is procedure detail, and there are also claim level adjustments, then the user will need to enter the procedure amounts by procedure and the claim adjustment by total.
				else if(hx835_Claim.ClaimAdjustmentTotal!=0) {
					row.Cells.Add(new UI.GridCell("Proc &\r\nTotal"));//EnterBy
				}
				//If there is procedure detail, and there are no claim level adjustments, the user will need to enter the payments by procedure only.
				else {
					row.Cells.Add(new UI.GridCell("Proc"));//EnterBy
				}
				string strClaim="Patient: "+hx835_Claim.PatientName;
				if(hx835_Claim.SubscriberName!=hx835_Claim.PatientName) {
					strClaim+="\r\nSubscriber: "+hx835_Claim.SubscriberName;
				}
				if(hx835_Claim.ClaimTrackingNumber!="") {
					strClaim+="\r\nClaim Identifier: "+hx835_Claim.ClaimTrackingNumber;
				}
				if(hx835_Claim.PayerControlNumber!="") {
					strClaim+="\r\nPayer Control Number: "+hx835_Claim.PayerControlNumber;
				}
				if(hx835_Claim.ListProcs.Count>0 && hx835_Claim.ClaimAdjustmentTotal!=0) {
					//If there is no procedure detail, then the user will need to enter the claim payment by total.  In this case, the user only cares about the InsPaid for the entire claim.  Showing the adjustments would cause user confusion.
					//If there is procedure detail, then we need to show the claim adjustment total, because the user will need to enter this amount by total in addition to any procedure amounts entered.
					strClaim+="\r\nClaim Adjustments: "+hx835_Claim.ClaimAdjustmentTotal.ToString("f2");
				}
				row.Cells.Add(new UI.GridCell(strClaim));//Claim
				string strDateClaim=hx835_Claim.DateServiceStart.ToShortDateString();
				if(hx835_Claim.DateServiceEnd.Year>1880) {
					strDateClaim+=" to \r\n"+hx835_Claim.DateServiceEnd.ToShortDateString();
				}
				row.Cells.Add(new UI.GridCell(strDateClaim));//Date
				row.Cells.Add(new UI.GridCell(""));//Code
				row.Cells.Add(new UI.GridCell(""));//CodeBilled
				row.Cells.Add(new UI.GridCell(hx835_Claim.ClaimFee.ToString("f2")));//Billed
				row.Cells.Add(new UI.GridCell(hx835_Claim.PatientRespAmt.ToString("f2")));//PatResp
				row.Cells.Add(new UI.GridCell(""));//Allowed
				row.Cells.Add(new UI.GridCell(hx835_Claim.InsPaid.ToString("f2")));//InsPay
				gridMain.ListGridRows.Add(row);
				for(int j=0;j<hx835_Claim.ListProcs.Count;j++) {
					Hx835_Proc hx835_Proc=hx835_Claim.ListProcs[j];
					UI.GridRow rowProc=new UI.GridRow();
					rowProc.Cells.Add(new UI.GridCell(""));//EnterBy
					rowProc.Cells.Add(new UI.GridCell(""));//Claim
					string strDateProc=hx835_Proc.DateServiceStart.ToShortDateString();
					if(hx835_Proc.DateServiceEnd.Year>1880) {
						strDateProc+=" to \r\n"+hx835_Proc.DateServiceEnd.ToShortDateString();
					}
					rowProc.Cells.Add(new UI.GridCell(strDateProc));//Date
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.ProcCodeAdjudicated));//Code
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.ProcCodeBilled));//CodeBilled
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.ProcFee.ToString("f2")));//Billed
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.PatRespTotal.ToString("f2")));//PatResp
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.AllowedAmt.ToString("f2")));//Allowed
					rowProc.Cells.Add(new UI.GridCell(hx835_Proc.InsPaid.ToString("f2")));//InsPay
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
			Rectangle rectangle=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangle.Top;
			int center=rectangle.X+rectangle.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Electronic Remittance Advice (ERA)");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,fontHeading).Height;
				yPos+=20;
				_isHeadingPrinted=true;
				_headingPrintY=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,_pagesPrintedCount,rectangle,_headingPrintY);
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