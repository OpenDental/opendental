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
///<summary></summary>
	public partial class FormUnsched:FormODBase {
		///<summary></summary>
		public static string procsForCur;
		private List<Appointment> _listUnschedApt;
		private bool headingPrinted;
		private int headingPrintH;
		private int pagesPrinted;
		private List<long> _listAptSelected;
		private List<Provider> _listProviders;
		private List<Site> _listSites;

		///<summary>PatientGoTo must be set before calling Show() or ShowDialog().</summary>
		public FormUnsched(){
			InitializeComponent();// Required for Windows Form Designer support
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormUnsched_Load(object sender, System.EventArgs e) {
			Cursor=Cursors.WaitCursor;
			comboOrder.Items.Add(Lan.g(this,"UnschedStatus"));
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
			_listAptSelected=new List<long>();
			LayoutMenu();
			InitDateRange();
			Cursor=Cursors.Default;
		}

		private void LayoutMenu() {
			menuMain.BeginUpdate();
			menuMain.Add(new MenuItemOD("Setup",setupToolStripMenuItem_Click));
			menuMain.EndUpdate();
		}

		///<summary>There is a bug in ODProgress.cs that forces windows that use a progress bar on load to go behind other applications. 
		///This is a temporary workaround until we decide how to address the issue.</summary>
		private void FormUnsched_Shown(object sender,EventArgs e) {
			FillGrid();
		}

		private void setupToolStripMenuItem_Click(object sender,EventArgs e) {
			using FormUnschedListSetup formSetup=new FormUnschedListSetup();
			if(formSetup.ShowDialog()==DialogResult.OK) {
				InitDateRange();
			}
		}

		private void InitDateRange() {
			int dayCount=PrefC.GetInt(PrefName.UnschedDaysPast);
			if(dayCount==-1) {
				//Set the text to blank
				dateRangePicker.SetDateTimeFrom(DateTime.MinValue);
			}
			else {
				dateRangePicker.SetDateTimeFrom(DateTime.Today.AddDays(-dayCount));
			}
			dayCount=PrefC.GetInt(PrefName.UnschedDaysFuture);
			if(dayCount==-1) {
				//Set the text to blank. We check for DateTime.MinValue in Appointments.RefreshUnsched() and modify the query to not have an end date.
				dateRangePicker.SetDateTimeTo(DateTime.MinValue);
			}
			else {
				dateRangePicker.SetDateTimeTo(DateTime.Today.AddDays(dayCount));
			}
		}

		private void menuRight_click(object sender,System.EventArgs e) {
			if(grid.SelectedIndices.Length==0) {
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
				case 3:
					Delete_Click();
					break;
			}
		}

		private void grid_MouseUp(object sender,MouseEventArgs e) {
			if(e.Button==MouseButtons.Right && grid.SelectedIndices.Length>0) {
				//To maintain legacy behavior we will use the last selected index if multiple are selected.
				Patient pat=Patients.GetLim(_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);
				toolStripMenuItemSelectPatient.Text=Lan.g(grid.TranslationName,"Select Patient")+" ("+pat.GetNameFL()+")";
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum;
			Patient pat=Patients.GetPat(patNum);
			FormOpenDental.S_Contr_PatientSelected(pat,true);
		}

		///<summary>If multiple patients are selected in UnchedList, will select the last patient to remain consistent with sending to pinboard behavior.</summary>
		private void SeeChart_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Patient pat=Patients.GetPat(_listUnschedApt[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoChart(pat.PatNum);
		}

		private void SendPinboard_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			_listAptSelected.Clear();
			int patsRestricted=0;
			for(int i=0;i<grid.SelectedIndices.Length;i++) {
				if(PatRestrictionL.IsRestricted(_listUnschedApt[grid.SelectedIndices[i]].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				_listAptSelected.Add(_listUnschedApt[grid.SelectedIndices[i]].AptNum);
			}
			if(patsRestricted>0) {
				if(_listAptSelected.Count==0) {
					MsgBox.Show("All selected appointments have been skipped due to patient restriction "
						+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+".");
					return;
				}
				MessageBox.Show("Appointments skipped due to patient restriction "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			GotoModule.PinToAppt(_listAptSelected,0);//This will send all appointments in _listAptSelected to the pinboard, and will select the patient attached to the last appointment in _listAptSelected.
		}

		private void Delete_Click() {
			if(!Security.IsAuthorized(Permissions.AppointmentEdit)) {
				return;
			}
			if(grid.SelectedIndices.Length>1) {
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete all selected appointments permanently?")) {
					return;
				}
			}
			List<Appointment> listApptsWithNote=new List<Appointment>();
			List<long> listSelectedAptNums=new List<long>();
			foreach(int i in grid.SelectedIndices) {
				listSelectedAptNums.Add(_listUnschedApt[i].AptNum);
				if(!string.IsNullOrEmpty(_listUnschedApt[i].Note)) {
					listApptsWithNote.Add(_listUnschedApt[i]);
				}
			}
			if(listApptsWithNote.Count>0) {//There were notes in the appointment(s) we are about to delete and we must ask if they want to save them in a commlog.
				string commlogMsg="";
				if(grid.SelectedIndices.Length==1) {
					commlogMsg=Commlogs.GetDeleteApptCommlogMessage(listApptsWithNote[0].Note,listApptsWithNote[0].AptStatus);
				}
				else {
					commlogMsg="One or more appointments have notes.  Save appointment notes in CommLogs?";
				}
				DialogResult result=MessageBox.Show(commlogMsg,"Question...",MessageBoxButtons.YesNoCancel);
				if(result==DialogResult.Cancel) {
					return;
				}
				else if(result==DialogResult.Yes) {
					foreach(Appointment apptCur in listApptsWithNote) {
						Commlog commlogCur=new Commlog();
						commlogCur.PatNum=apptCur.PatNum;
						commlogCur.CommDateTime=DateTime.Now;
						commlogCur.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						commlogCur.Note=Lan.g(this,"Deleted Appt. & saved note")+": ";
						if(apptCur.ProcDescript!="") {
							commlogCur.Note+=apptCur.ProcDescript+": ";
						}
						commlogCur.Note+=apptCur.Note;
						commlogCur.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(commlogCur);
					}
				}
			}
			Appointments.Delete(listSelectedAptNums);
			foreach(int i in grid.SelectedIndices) {
				SecurityLogs.MakeLogEntry(Permissions.AppointmentEdit,_listUnschedApt[i].PatNum,
					Lan.g(this,"Appointment deleted from the Unscheduled list."),_listUnschedApt[i].AptNum,_listUnschedApt[i].DateTStamp);
			}
			FillGrid();
		}

		private void FillGrid() {
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
			bool showBrokenAppts;
			showBrokenAppts=checkBrokenAppts.Checked;
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.SelectedClinicNum : -1;
			Dictionary<long,string> dictPatNames=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => { 
				_listUnschedApt=Appointments.RefreshUnsched(order,provNum,siteNum,showBrokenAppts,clinicNum,
					codeRangeFilter.StartRange,codeRangeFilter.EndRange,dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo());
				dictPatNames=Patients.GetPatientNames(_listUnschedApt.Select(x => x.PatNum).ToList());
				};
			progressOD.ShowDialogProgress();
			int scrollVal=grid.ScrollValue;
			grid.BeginUpdate();
			grid.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableUnsched","Patient"),140);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Date"),65);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","AptStatus"),90);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","UnschedStatus"),110);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Prov"),50);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Procedures"),150);
			grid.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Notes"),200);
			grid.ListGridColumns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			foreach(Appointment apt in _listUnschedApt) {
				row=new GridRow();
				string patName=Lan.g(this,"UNKNOWN");
				dictPatNames.TryGetValue(apt.PatNum,out patName);
				row.Cells.Add(patName);
				if(apt.AptDateTime.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(apt.AptDateTime.ToShortDateString());
				}
				if(apt.AptStatus == ApptStatus.Broken) {
					row.Cells.Add(Lan.g(this,"Broken"));
				}
				else {
					row.Cells.Add(Lan.g(this,"Unscheduled"));
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,apt.UnschedStatus));
				row.Cells.Add(Providers.GetAbbr(apt.ProvNum));
				row.Cells.Add(apt.ProcDescript);
				row.Cells.Add(apt.Note);
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollValue=scrollVal;
		}

		private void grid_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;//tbApts.SelectedRow;
			int currentScroll=grid.ScrollValue;//tbApts.ScrollValue;
			Patient pat=Patients.GetPat(_listUnschedApt[e.Row].PatNum);//If multiple selected, just take the one that was clicked on.
			FormOpenDental.S_Contr_PatientSelected(pat,true);
			using FormApptEdit FormAE=new FormApptEdit(_listUnschedApt[e.Row].AptNum);
			FormAE.PinIsVisible=true;
			FormAE.ShowDialog();
			if(FormAE.DialogResult!=DialogResult.OK) {
				return;
			}
			if(FormAE.PinClicked) {
				SendPinboard_Click();//Whatever they double clicked on will still be selected, just fire the event.
				DialogResult=DialogResult.OK;//this is an obsolete line. Window stays open.
			}
			else {
				FillGrid();
				grid.SetSelected(currentSelection,true);
				grid.ScrollValue=currentScroll;
			}
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			pagesPrinted=0;
			headingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Unscheduled appointment list printed"));
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
				text=Lan.g(this,"Unscheduled List");
				g.DrawString(text,headingFont,Brushes.Black,center-g.MeasureString(text,headingFont).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				headingPrinted=true;
				headingPrintH=yPos;
			}
			#endregion
			yPos=grid.PrintPage(g,pagesPrinted,bounds,headingPrintH);
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
