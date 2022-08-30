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
		private List<PatientInfo> _listPatientInfos;
		private MassEmailSendResult _massEmailSendResult;

		public FormMassEmailSendResults(List<PatientInfo> listPatientsInfo,MassEmailSendResult massEmailSendResult) {
			InitializeComponent();
			_listPatientInfos=listPatientsInfo;
			_massEmailSendResult=massEmailSendResult;
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
			GridColumn col=new GridColumn(Lans.g(gridResults.TranslationName,"Status"),40,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(col);
			col=new GridColumn(Lans.g(gridResults.TranslationName,"Details"),140,HorizontalAlignment.Center,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(col);
			col=new GridColumn(Lans.g(gridResults.TranslationName,"Name"),140,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(col);
			col=new GridColumn(Lans.g(gridResults.TranslationName,"Email"),140,GridSortingStrategy.StringCompare);
			gridResults.Columns.Add(col);
			gridResults.ListGridRows.Clear();
			GridRow row;
			_listPatientInfos=_listPatientInfos.OrderByDescending(x=>x.Email.In(_massEmailSendResult.ListTemplateDestinationsUnableToSend.Select(x=>x.ToAddress).ToArray())).ToList();
			foreach(PatientInfo patientInfo in _listPatientInfos) {
				MassEmailDestinationFailed massEmailDestinationFailed=null;
				if(_massEmailSendResult!=null && _massEmailSendResult.ListTemplateDestinationsUnableToSend!=null) {
					massEmailDestinationFailed=_massEmailSendResult.ListTemplateDestinationsUnableToSend.FirstOrDefault(x=>x.PatNum==patientInfo.PatNum);
				}
				row=new GridRow();
				string status=massEmailDestinationFailed==null ? "Sent" : "Failed";
				row.Cells.Add(status);//Check for selected patients
				row.Cells.Add(massEmailDestinationFailed?.Description??"");
				row.Cells.Add(patientInfo.Name);//Patient Name
				row.Cells.Add(patientInfo.Email);//Patient Email
				row.Tag=patientInfo;
				gridResults.ListGridRows.Add(row);
			}
			gridResults.EndUpdate();
		}

		private void FillUI() {
			long numberSent=_listPatientInfos.Count;
			long numRemoved=0;
			if(!_massEmailSendResult.ListTemplateDestinationsUnableToSend.IsNullOrEmpty()) {
				numRemoved+=_massEmailSendResult.ListTemplateDestinationsUnableToSend.Count;
			}
			numberSent-=numRemoved;
			labelResultCount.Text=Lans.g(this,"Total sent:")+" "+numberSent+"/"+_listPatientInfos.Count;
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