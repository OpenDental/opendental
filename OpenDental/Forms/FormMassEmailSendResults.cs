using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMassEmailSendResults:FormODBase {
		private List<PatientInfo> _listPatientInfo;
		private MassEmailSendResult _results;

		public FormMassEmailSendResults(List<PatientInfo> listPatientInfo,MassEmailSendResult results) {
			InitializeComponent();
			_listPatientInfo=listPatientInfo;
			_results=results;
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMassEmailSendResults_Load(object sender,EventArgs e) {
			FillGrid();
			FillUI();
		}

		private void FillGrid() {
			gridResults.BeginUpdate();
			gridResults.Columns.Clear();
			GridColumn gridColumn=new GridColumn(Lans.g(gridResults.TranslationName,"Status"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridResults.TranslationName,"Details"),140,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridResults.TranslationName,"Name"),140,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(gridResults.TranslationName,"Email"),140,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(gridColumn);
			gridResults.ListGridRows.Clear();
			GridRow gridRow;
			_listPatientInfo=_listPatientInfo.OrderByDescending(x=>x.Email.In(_results.ListTemplateDestinationsUnableToSend.Select(x=>x.ToAddress).ToArray())).ToList();
			foreach(PatientInfo patientInfo in _listPatientInfo) {
				MassEmailDestinationFailed failed=null;
				if(_results!=null && _results.ListTemplateDestinationsUnableToSend!=null) {
					failed=_results.ListTemplateDestinationsUnableToSend.FirstOrDefault(x=>x.PatNum==patientInfo.PatNum);
				}
				gridRow=new GridRow();
				string status=failed==null ? "Sent" : "Failed";
				gridRow.Cells.Add(status);//Check for selected patients
				gridRow.Cells.Add(failed?.Description??"");
				gridRow.Cells.Add(patientInfo.Name);//Patient Name
				gridRow.Cells.Add(patientInfo.Email);//Patient Email
				gridRow.Tag=patientInfo;
				gridResults.ListGridRows.Add(gridRow);
			}
			gridResults.EndUpdate();
		}

		private void FillUI() {
			long numberSent=_listPatientInfo.Count;
			long numRemoved=0;
			if(!_results.ListTemplateDestinationsUnableToSend.IsNullOrEmpty()) {
				numRemoved+=_results.ListTemplateDestinationsUnableToSend.Count;
			}
			numberSent-=numRemoved;
			labelResultCount.Text=Lans.g(this,"Total sent:")+" "+numberSent+"/"+_listPatientInfo.Count;
		}

		private void butPrint_Click(object sender,EventArgs e) {
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Mass Email send results printed"),PrintoutOrientation.Landscape);
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			int pagesPrinted=0;	
			Rectangle bounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font headingFont=new Font("Arial",13,FontStyle.Bold);
			Font subHeadingFont=new Font("Arial",10,FontStyle.Bold);
			int yPos=bounds.Top;
			int center=bounds.X+bounds.Width/2;
			#region printHeading
			text=Lan.g(this,gridResults.Title);
			g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
			yPos+=(int)g.MeasureString(text,headingFont).Height;
			//print today's date
			text = Lan.g(this,"Run On:")+" "+DateTime.Today.ToShortDateString();
			g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
			yPos+=20;
			text=labelResultCount.Text;
			g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
			yPos+=20;
			int headingPrintH=yPos;
			#endregion
			yPos=gridResults.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			g.Dispose();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}