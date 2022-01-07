using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>The Next appoinment tracking tool.</summary>
	public partial class FormTrackNext : FormODBase {
		private List<Appointment> _listPlannedAppts;
		private int pagesPrinted;
		private bool headingPrinted;
		private int headingPrintH;
		private List<Provider> _listProviders;
		private List<Site> _listSites;

		///<summary>PatientGoTo must be set before calling Show() or ShowDialog().</summary>
		public FormTrackNext(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormTrackNext_Load(object sender, System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			comboOrder.Items.Add(Lan.g(this,"Status"));
			comboOrder.Items.Add(Lan.g(this,"Alphabetical"));
			comboOrder.Items.Add(Lan.g(this,"Date"));
			comboOrder.SelectedIndex=0;
			comboProv.Items.Add(Lan.g(this,"All"));
			comboProv.SelectedIndex=0;
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				comboProv.Items.Add(_listProviders[i].GetLongDesc());
			}
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)){
				comboSite.Visible=false;
				labelSite.Visible=false;
			}
			else{
				comboSite.Items.Add(Lan.g(this,"All"));
				comboSite.SelectedIndex=0;
				_listSites=Sites.GetDeepCopy();
				for(int i=0;i<_listSites.Count;i++) {
					comboSite.Items.Add(_listSites[i].Description);
				}
			}
			if(PrefC.GetBool(PrefName.EnterpriseApptList)){
				comboClinic.IncludeAll=false;
			}
			LayoutMenu();
			InitDateRange();
			RefreshAptList();
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormTrackNext_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void InitDateRange() {
			int dayCount=PrefC.GetInt(PrefName.PlannedApptDaysPast);
			dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-dayCount));
			dayCount=PrefC.GetInt(PrefName.PlannedApptDaysFuture);
			dateRangePicker.SetDateTimeTo(DateTime.Today.AddDays(dayCount));
		}
		
		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormTrackNextSetup formTNS=new FormTrackNextSetup();
			if(formTNS.ShowDialog()==DialogResult.OK) {
				InitDateRange();
			}
		}

		private void menuRight_click(object sender,System.EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			switch(menuRightClick.Items.IndexOf((ToolStripMenuItem)sender)) {
				case 0: 
					SelectPatient_Click();
					break;
				case 1:
					SeeChart_Click();
					break;
				case 2:
					SendPinboard_Click();
					break;
			}
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Patient"),140);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Date"),65);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Status"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Prov"),50);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Procedures"),150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Notes"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Dictionary<long,string> dictPatNames=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				dictPatNames=Patients.GetLimForPats(_listPlannedAppts.Select(x => x.PatNum).ToList())
					.ToDictionary(x => x.PatNum,x => x.GetNameLF());
				};
			progressOD.ShowDialogProgress();
			foreach(Appointment apt in _listPlannedAppts){
				row=new GridRow();
				string patName=Lan.g(this,"UNKNOWN");
				dictPatNames.TryGetValue(apt.PatNum,out patName);
				row.Cells.Add(patName);
				if(apt.AptDateTime.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(apt.AptDateTime.ToShortDateString());
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,apt.UnschedStatus));
				if(apt.IsHygiene) {
					Provider provHyg=Providers.GetFirstOrDefault(x => x.ProvNum==apt.ProvHyg);
					row.Cells.Add(provHyg==null?Lan.g(this,"INVALID"):provHyg.Abbr);
				}
				else {
					Provider prov=Providers.GetFirstOrDefault(x => x.ProvNum==apt.ProvNum);
					row.Cells.Add(prov==null?Lan.g(this,"INVALID"):prov.Abbr);
				}
				row.Cells.Add(apt.ProcDescript);
				row.Cells.Add(apt.Note);
			  
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void RefreshAptList() {
			string order="";
			switch(comboOrder.SelectedIndex) {
				case 0:
					order="status";
					break;
				case 1:
					order="alph";
					break;
				case 2:
					order="date";
					break;
			}
			long provNum=0;
			if(comboProv.SelectedIndex!=0) {
				provNum=_listProviders[comboProv.SelectedIndex-1].ProvNum;
			}
			long siteNum=0;
			if(!PrefC.GetBool(PrefName.EasyHidePublicHealth) && comboSite.SelectedIndex!=0) {
				siteNum=_listSites[comboSite.SelectedIndex-1].SiteNum;
			}
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.SelectedClinicNum : -1;
			_listPlannedAppts=Appointments.RefreshPlannedTracker(order,provNum,siteNum,clinicNum,codeRangeFilter.StartRange,
				codeRangeFilter.EndRange,dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo());
		}

		private void grid_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && gridMain.SelectedIndices.Length>0) {
				Patient pat=Patients.GetLim(_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum);
				toolStripMenuItemSelectPatient.Text=Lan.g(gridMain.TranslationName,"Select Patient")+" ("+pat.GetNameFL()+")";
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum;
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
		}

		private void SeeChart_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			//Only one can be selected at a time in this grid, but just in case we change it in the future it will select the last one in the list to be consistent with other patient selections.
			Patient pat=Patients.GetPat(_listPlannedAppts[gridMain.SelectedIndices[gridMain.SelectedIndices.Length-1]].PatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoChart(pat.PatNum);
		}

		private void SendPinboard_Click() {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			List<long> listAppts=new List<long>();
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				listAppts.Add(_listPlannedAppts[gridMain.SelectedIndices[i]].AptNum);//Will only be one unless multiselect is enabled in the future
				_listPlannedAppts.RemoveAt(gridMain.SelectedIndices[i]);
			}
			GotoModule.PinToAppt(listAppts,0);//This will send all appointments in _listAptSelected to the pinboard, and will select the patient attached to the last appointment in _listAptSelected.
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=gridMain.GetSelectedIndex();
			int currentScroll=gridMain.ScrollValue;
			Patient pat=Patients.GetPat(_listPlannedAppts[e.Row].PatNum);//Only one can be selected at a time in this grid.
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormAE=new FormApptEdit(_listPlannedAppts[e.Row].AptNum);
			FormAE.PinIsVisible=true;
			FormAE.ShowDialog();
			if(FormAE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormAE.PinClicked) {
				SendPinboard_Click();
				DialogResult=DialogResult.OK;
			}
			else {
				RefreshAptList();
				FillGrid();
				gridMain.SetSelected(currentSelection,true);
				gridMain.ScrollValue=currentScroll;
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			RefreshAptList();
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Planned appointment tracker list printed"));
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
				text=Lan.g(this,"Planned Appointment Tracker");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
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

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

	}
}