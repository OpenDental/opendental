using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;
using PdfSharp.Pdf;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpTreatmentFinder:FormODBase {
		private bool headingPrinted;
		private int headingPrintH;
		private int pagesPrinted;
		private int patientsPrinted;
		private List<Provider> _listProviders;

		///<summary>We do not hold onto the data table in memory so we use this list to keep track of the headers we'll need when exporting.</summary>
		private List<string> _listHeaders=new List<string>();
		///<summary></summary>
		public FormRpTreatmentFinder() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			gridMain.ContextMenu=contextRightClick;
		}

		private void FormRpTreatmentFinder_Load(object sender, System.EventArgs e) {
			comboBoxMultiProv.IncludeAll=true;
			comboBoxMultiBilling.IncludeAll=true;
			datePickerEnd.SetDateTime(DateTime.Today);
			datePickerStart.SetDateTime(DateTime.Today.AddYears(-1));
			_listProviders=Providers.GetListReports();
			//DateTime today=DateTime.Today;
			//will start out 1st through 30th of previous month
			//date1.SelectionStart=new DateTime(today.Year,today.Month,1).AddMonths(-1);
			//date2.SelectionStart=new DateTime(today.Year,today.Month,1).AddDays(-1);
			for(int i=0;i<_listProviders.Count;i++){
			  comboBoxMultiProv.Items.Add(_listProviders[i].GetLongDesc(),_listProviders[i]);
			}
			comboBoxMultiProv.IsAllSelected=true;
			List<Def> listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,true);
			for(int i=0;i<listBillingTypeDefs.Count;i++){
				comboBoxMultiBilling.Items.Add(listBillingTypeDefs[i].ItemName,listBillingTypeDefs[i]);
			}
			comboBoxMultiBilling.IsAllSelected=true;
			comboMonthStart.SelectedIndex=0;
			checkBenefitAssumeGeneral.Checked=PrefC.GetBool(PrefName.TreatFinderProcsAllGeneral);
			if(RemotingClient.RemotingRole!=RemotingRole.ClientWeb) {//for middle tier, don't allow mutiple clinics
				comboClinics.IncludeAll=true;
				comboClinics.SelectionModeMulti=true;
			}
			FillGrid();
		}

		private void FillGrid() {
			if(!textOverAmount.IsValid()) {
				return;
			}
			DateTime dateFrom=datePickerStart.GetDateTime();
			DateTime dateTo=datePickerEnd.GetDateTime();
			int monthStart=comboMonthStart.SelectedIndex;
			double aboveAmount=PIn.Double(textOverAmount.Text);
			Stopwatch sw=new Stopwatch();
			if(ODBuild.IsDebug()) {
				sw=Stopwatch.StartNew();
			}
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			//0=PatNum
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","LName"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","FName"),100));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Contact"),120));
			//4=address
			//5=cityStateZip
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Annual Max"),80,HorizontalAlignment.Right,GridSortingStrategy.StringCompare));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Amt Used"),70,HorizontalAlignment.Right,GridSortingStrategy.StringCompare));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Amt Pend"),70,HorizontalAlignment.Right,GridSortingStrategy.StringCompare));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Amt Rem"),70,HorizontalAlignment.Right,GridSortingStrategy.StringCompare));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Treat Plan"),70,HorizontalAlignment.Right,GridSortingStrategy.AmountParse));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Insurance Carrier"),225));
			if(PrefC.HasClinicsEnabled) {
				gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableTreatmentFinder","Clinic"),120));
			}
			gridMain.ListGridRows.Clear();
			Cursor=Cursors.WaitCursor;
			using(DataTable table=RpTreatmentFinder.GetTreatmentFinderList(checkIncludeNoIns.Checked,checkIncludePatsWithApts.Checked,monthStart,dateFrom,dateTo,
				aboveAmount,comboBoxMultiProv.GetSelectedProvNums(),
				comboBoxMultiBilling.GetListSelected<Def>().Select(x => x.DefNum).ToList(),codeRangeFilter.StartRange,codeRangeFilter.EndRange,
				comboClinics.ListSelectedClinicNums,checkBenefitAssumeGeneral.Checked,checkUseTreatingProvider.Checked))
			{
				if(PrefC.HasClinicsEnabled) {
					_listHeaders=table.Columns.AsEnumerable<DataColumn>().Select(x=>x.ColumnName).ToList();
				}
				else {
					_listHeaders=table.Columns.AsEnumerable<DataColumn>().Where(x=>!x.ColumnName.Contains("clinic")).Select(x=>x.ColumnName).ToList();
				}
				GridRow row;
				foreach(DataRow rowCur in table.Rows) {
					row=new GridRow() { Tag=rowCur };
					double indMax=PIn.Double(rowCur[8].ToString());
					double famMax=PIn.Double(rowCur[9].ToString());
					//Temporary filter just showing columns wanted. Probably it will become user defined.
					for(int j=0;j<table.Columns.Count;j++) {
						//0- PatNum,4-address,5-city,6-State,7-Zip are just for the export, 9-AnnualMaxFam,11-AmtUsedFam,13-AmtPendingFam,15-AmtRemainingFam on new line
						if(ListTools.In(j,0,4,5,6,7,9,11,13,15) || (j==18 && !PrefC.HasClinicsEnabled)) {
							continue;
						}
						string cellData=rowCur[j].ToString();
						if(ListTools.In(j,8,10,12,14)) {//AnnualMax,AmtUsed,AmtPending,AmtRemaining
							cellData=(indMax<=0?"":("I: "+rowCur[j].ToString()+(famMax>0?"\r\n":"")))+(famMax<=0?"":("F: "+rowCur[j+1].ToString()));
						}
						row.Cells.Add(cellData);
					}
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
			if(ODBuild.IsDebug()) {
				sw.Stop();
				Console.WriteLine("Finished fetching data and filling grid: {0}, Rows: {1}",(sw.Elapsed.Seconds==0?"":(sw.Elapsed.Seconds+" sec "))+(sw.Elapsed.TotalMilliseconds-(sw.Elapsed.Seconds*1000))+" ms",gridMain.ListGridRows.Count);
			}
			Cursor=Cursors.Default;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.SelectedGridRows.Count==0) {//When deselecting with CTR+Click.
				return;
			}
			GotoModule.GotoChart(PIn.Long(gridMain.SelectedTag<DataRow>()["PatNum"].ToString()));
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Might not need cellDoubleClick
		}

		private void comboBoxMultiProv_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxMultiProv.SelectedIndices.Count==0) {
				comboBoxMultiProv.IsAllSelected=true;
			}
		}

		private void comboBoxMultiBilling_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxMultiBilling.SelectedIndices.Count==0) {
				comboBoxMultiBilling.IsAllSelected=true;
			}
		}

		private void butLettersPreview_Click(object sender,EventArgs e) {
			//Create letters. loop through each row and insert information into sheets,
			//take all the sheets and add to one giant pdf for preview.
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select patient(s) first.");
				return;
			}
			using FormSheetPicker FormS=new FormSheetPicker();
			FormS.SheetType=SheetTypeEnum.PatientLetter;
			FormS.ShowDialog();
			if(FormS.DialogResult!=DialogResult.OK) {
				return;
			}
			SheetDef sheetDef;
			Sheet sheet=null;
			List<Sheet> listSheets=new List<Sheet>(); //for saving
			for(int i=0;i<FormS.SelectedSheetDefs.Count;i++) {
				PdfDocument document=new PdfDocument();
				PdfPage page=new PdfPage();
				string filePathAndName="";
				for(int j=0;j<gridMain.SelectedIndices.Length;j++) {
					sheetDef=FormS.SelectedSheetDefs[i];
					sheet=SheetUtil.CreateSheet(sheetDef,PIn.Long(((DataRow)gridMain.SelectedGridRows[j].Tag)["PatNum"].ToString()));
					SheetParameter.SetParameter(sheet,"PatNum",PIn.Long(((DataRow)gridMain.SelectedGridRows[j].Tag)["PatNum"].ToString()));
					//Purposefully not setting the optional "AptNum" SheetParameter here (AptNum is required for StaticTextFields like apptDateMonthSpelled).
					//Allen - 02/25/2021 14:01 via Job #26156:
					//We do not need to consider any place that does automation or mass sheet creation at this time.
					//We can just document that this is currently unsupported in the manual for these fields.
					SheetFiller.FillFields(sheet);
					sheet.SheetFields.Sort(SheetFields.SortDrawingOrderLayers);
					SheetUtil.CalculateHeights(sheet);
					//SheetPrinting.PagesPrinted=0;//Clear out the pages printed variable before printing all pages for this pdf.
					int pageCount=Sheets.CalculatePageCount(sheet,SheetPrinting.PrintMargin);
					int yPos=0;
					for(int k=0;k<pageCount;k++) {
						page=document.AddPage();
						yPos=SheetPrinting.CreatePdfPage(sheet,page,null,null,null,yPos,k);
					}
					listSheets.Add(sheet);
				}
				filePathAndName=PrefC.GetRandomTempFile(".pdf");
				document.Save(filePathAndName);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(filePathAndName);
				}
				else {
					Process.Start(filePathAndName);
				}
				DialogResult=DialogResult.OK;
			}
			if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to save the sheets for the selected patients?")) {
					Sheets.SaveNewSheetList(listSheets);
			}
		}

		private void butLabelSingle_Click(object sender,EventArgs e) {
		  if(gridMain.SelectedIndices.Length==0) {
		    MsgBox.Show(this,"Please select patient(s) first.");
		    return;
		  }
		  int patientsPrinted=0;
		  string text;
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				text="";
				//print single label
				DataRow curRow=(DataRow)gridMain.SelectedGridRows[i].Tag;
		    text=curRow["FName"].ToString()+" "+curRow["LName"].ToString()+"\r\n";
		    text+=curRow["address"].ToString()+"\r\n";
		    text+=curRow["City"].ToString()+", "+curRow["State"].ToString()+" "+curRow["Zip"].ToString()+"\r\n";
		    LabelSingle.PrintText(0,text);
		    patientsPrinted++;
			}
		}

		private void butLabelPreview_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0){
				MsgBox.Show(this,"Please select patient(s) first.");
		    return;
			}
			pagesPrinted=0;
			patientsPrinted=0;
			PrinterL.TryPreview(pdLabels_PrintPage,
				Lan.g(this,"Treatment finder labels printed"),
				PrintSituation.LabelSheet,
				new Margins(0,0,0,0),
				PrintoutOrigin.AtMargin,
				totalPages:(int)Math.Ceiling((double)gridMain.SelectedIndices.Length/30)
			);
		}

		private void pdLabels_PrintPage(object sender, PrintPageEventArgs ev){
			int totalPages=(int)Math.Ceiling((double)gridMain.SelectedIndices.Length/30);
			Graphics g=ev.Graphics;
			float yPos=63;
			float xPos=50;
			string text="";
			while(yPos<1000 && patientsPrinted<gridMain.SelectedIndices.Length){
				text="";
				DataRow curRow=(DataRow)gridMain.SelectedGridRows[patientsPrinted].Tag;
				text=curRow["FName"].ToString()+" "+curRow["LName"].ToString()+"\r\n";
				text+=curRow["address"].ToString()+"\r\n";
				text+=curRow["City"].ToString()+", "+curRow["State"].ToString()+" "+curRow["Zip"].ToString()+"\r\n";
				Rectangle rect=new Rectangle((int)xPos,(int)yPos,275,100);
				MapAreaRoomControl.FitText(text,new Font(FontFamily.GenericSansSerif,11),Brushes.Black,rect,new StringFormat(),g);
				//reposition for next label
				xPos+=275;
				if(xPos>850){//drop a line
					xPos=50;
					yPos+=100;
				}
				patientsPrinted++;
			}
			pagesPrinted++;
			if(pagesPrinted==totalPages){
				ev.HasMorePages=false;
				pagesPrinted=0;//because it has to print again from the print preview
				patientsPrinted=0;
			}
			else{
				ev.HasMorePages=true;
			}
			g.Dispose();
		}

		private void menuItemFamily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FamilyModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			long patNum=PIn.Long(gridMain.SelectedTag<DataRow>()["PatNum"].ToString());
			GotoModule.GotoFamily(patNum);
		}

		private void menuItemAccount_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			long patNum=PIn.Long(gridMain.SelectedTag<DataRow>()["PatNum"].ToString());
			GotoModule.GotoAccount(patNum);
		}

		private void butGotoFamily_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.FamilyModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			long patNum=PIn.Long(gridMain.SelectedTag<DataRow>()["PatNum"].ToString());
			GotoModule.GotoFamily(patNum);
		}

		private void butGotoAccount_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.AccountModule)) {
				return;
			}
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a patient first.");
				return;
			}
			ShrinkWindowBeforeMinMax();
			WindowState=FormWindowState.Minimized;
			long patNum=PIn.Long(gridMain.SelectedTag<DataRow>()["PatNum"].ToString());
			GotoModule.GotoAccount(patNum);
		}

		private void buttonExport_Click(object sender,EventArgs e) {
			string fileName=Lan.g(this,"Treatment Finder");
			string filePath=ODFileUtils.CombinePaths(Path.GetTempPath(),fileName);
			if(ODBuild.IsWeb()) {
				//file download dialog will come up later, after file is created.
				filePath+=".txt";//Provide the filepath an extension so that Thinfinity can offer as a download.
			}
			else {
				using SaveFileDialog saveFileDialog2=new SaveFileDialog();
				saveFileDialog2.AddExtension=true;
				saveFileDialog2.Title=Lan.g(this,"Treatment Finder");
				saveFileDialog2.FileName=Lan.g(this,"Treatment Finder");
				if(!Directory.Exists(PrefC.GetString(PrefName.ExportPath))) {
					try {
						Directory.CreateDirectory(PrefC.GetString(PrefName.ExportPath));
						saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
					}
					catch {
						//initialDirectory will be blank
					}
				}
				else {
					saveFileDialog2.InitialDirectory=PrefC.GetString(PrefName.ExportPath);
				}
				saveFileDialog2.Filter="Text files(*.txt)|*.txt|Excel Files(*.xls)|*.xls|All files(*.*)|*.*";
				saveFileDialog2.FilterIndex=0;
				if(saveFileDialog2.ShowDialog()!=DialogResult.OK) {
					return;
				}
				filePath=saveFileDialog2.FileName;
			}
			try{
			  using(StreamWriter sw=new StreamWriter(filePath,false))
				{
					sw.WriteLine(string.Join("\t",_listHeaders));
					List<DataRow> listRows=gridMain.ListGridRows.Select(x=>x.Tag as DataRow).ToList();
					for(int i=0;i<listRows.Count;i++) {
						string line="";
						for(int j=0;j<_listHeaders.Count;j++) {
							string cell=string.Concat(listRows[i][j].ToString().Where(x => !ListTools.In(x,'\r','\n','\t','\"')).ToArray());
							line+=cell+"\t";
						}
						sw.WriteLine(line);
					}
				}
      }
      catch{
        MessageBox.Show(Lan.g(this,"File in use by another program.  Close and try again."));
				return;
			}
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.ExportForDownload(filePath);
			}
			else {
				MessageBox.Show(Lan.g(this,"File created successfully"));
			}
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Treatment finder list printed"),PrintoutOrientation.Landscape);
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
			if(!headingPrinted) {
				text=Lan.g(this,"Treatment Finder");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				yPos+=(int)g.MeasureString(text,headingFont).Height;
				if(checkIncludeNoIns.Checked) {
					text="Include patients without insurance";
				}
				else {
					text="Only patients with insurance";
				}
				g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=20;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=gridMain.PrintPage(g,pagesPrinted,bounds,headingPrintH);
			pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

		private void checkIncludeHiddenBillingTypes_CheckedChanged(object sender,EventArgs e) {
			comboBoxMultiBilling.Items.Clear();
			List<Def> listBillingTypeDefs=Defs.GetDefsForCategory(DefCat.BillingTypes,!checkIncludeHiddenBillingTypes.Checked);
			listBillingTypeDefs.ForEach(x => comboBoxMultiBilling.Items.Add(x.ItemName + (x.IsHidden? " (hidden)":""),x));
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			Close();
		}

	}
}
