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
		public static string ProceduresForCur;
		private List<Appointment> _listAppointmentsUnsched;
		private bool _isHeadingPrinted;
		private int _heightHeadingPrint;
		private int _pagesPrinted;
		private List<long> _listAptNumsSelected;
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
			_listAptNumsSelected=new List<long>();
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
			using FormUnschedListSetup formUnschedListSetup=new FormUnschedListSetup();
			if(formUnschedListSetup.ShowDialog()==DialogResult.OK) {
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
				Patient patient=Patients.GetLim(_listAppointmentsUnsched[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);
				toolStripMenuItemSelectPatient.Text=Lan.g(grid.TranslationName,"Select Patient")+" ("+patient.GetNameFL()+")";
			}
		}

		private void SelectPatient_Click() {
			//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			long patNum=_listAppointmentsUnsched[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum;
			Patient patient=Patients.GetPat(patNum);
			GlobalFormOpenDental.PatientSelected(patient,true);
		}

		///<summary>If multiple patients are selected in UnchedList, will select the last patient to remain consistent with sending to pinboard behavior.</summary>
		private void SeeChart_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select an appointment first.");
				return;
			}
			Patient patient=Patients.GetPat(_listAppointmentsUnsched[grid.SelectedIndices[grid.SelectedIndices.Length-1]].PatNum);//If multiple selected, just take the last one to remain consistent with SendPinboard_Click.
			GlobalFormOpenDental.PatientSelected(patient,false);
			GlobalFormOpenDental.GoToModule(EnumModuleType.Chart,patNum:patient.PatNum);
		}

		private void SendPinboard_Click() {
			if(grid.SelectedIndices.Length==0) {
				MsgBox.Show("Please select an appointment first.");
				return;
			}
			_listAptNumsSelected.Clear();
			int patsRestricted=0;
			for(int i=0;i<grid.SelectedIndices.Length;i++) {
				if(PatRestrictionL.IsRestricted(_listAppointmentsUnsched[grid.SelectedIndices[i]].PatNum,PatRestrict.ApptSchedule,true)) {
					patsRestricted++;
					continue;
				}
				_listAptNumsSelected.Add(_listAppointmentsUnsched[grid.SelectedIndices[i]].AptNum);
			}
			if(patsRestricted>0) {
				if(_listAptNumsSelected.Count==0) {
					MsgBox.Show("All selected appointments have been skipped due to patient restriction "
						+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)+".");
					return;
				}
				MessageBox.Show("Appointments skipped due to patient restriction "+PatRestrictions.GetPatRestrictDesc(PatRestrict.ApptSchedule)
					+": "+patsRestricted+".");
			}
			GlobalFormOpenDental.GoToModule(EnumModuleType.Appointments, listPinApptNums:_listAptNumsSelected, dateSelected:DateTime.Today);//This will send all appointments in _listAptSelected to the pinboard, and will select the patient attached to the last appointment in _listAptSelected.
		}

		private void Delete_Click() {
			if(!Security.IsAuthorized(EnumPermType.AppointmentEdit)) {
				return;
			}
			if(grid.SelectedIndices.Length>1) {
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,"Delete all selected appointments permanently?")) {
					return;
				}
			}
			List<Appointment> listAppointmentsWithNote=new List<Appointment>();
			List<long> listAptNumsSelected=new List<long>();
			for(int i = 0;i<grid.SelectedIndices.Length;i++) {
				listAptNumsSelected.Add(_listAppointmentsUnsched[grid.SelectedIndices[i]].AptNum);
				if(!string.IsNullOrEmpty(_listAppointmentsUnsched[grid.SelectedIndices[i]].Note)) {
					listAppointmentsWithNote.Add(_listAppointmentsUnsched[grid.SelectedIndices[i]]);
				}
			}
			if(listAppointmentsWithNote.Count>0) {//There were notes in the appointment(s) we are about to delete and we must ask if they want to save them in a commlog.
				string commlogMsg="";
				if(grid.SelectedIndices.Length==1) {
					commlogMsg=Commlogs.GetDeleteApptCommlogMessage(listAppointmentsWithNote[0].Note,listAppointmentsWithNote[0].AptStatus);
				}
				else {
					commlogMsg="One or more appointments have notes.  Save appointment notes in CommLogs?";
				}
				DialogResult dialogResult=MessageBox.Show(commlogMsg,"Question...",MessageBoxButtons.YesNoCancel);
				if(dialogResult==DialogResult.Cancel) {
					return;
				}
				else if(dialogResult==DialogResult.Yes) {
					for(int i = 0;i<listAppointmentsWithNote.Count;i++) {
						Commlog commlog =new Commlog();
						commlog.PatNum=listAppointmentsWithNote[i].PatNum;
						commlog.CommDateTime=DateTime.Now;
						commlog.CommType=Commlogs.GetTypeAuto(CommItemTypeAuto.APPT);
						commlog.Note=Lan.g(this,"Deleted Appt. & saved note")+": ";
						if(listAppointmentsWithNote[i].ProcDescript!="") {
							commlog.Note+=listAppointmentsWithNote[i].ProcDescript+": ";
						}
						commlog.Note+=listAppointmentsWithNote[i].Note;
						commlog.UserNum=Security.CurUser.UserNum;
						//there is no dialog here because it is just a simple entry
						Commlogs.Insert(commlog);
					}
				}
			}
			Appointments.Delete(listAptNumsSelected);
			for(int i = 0;i<grid.SelectedIndices.Length;i++) {
				SecurityLogs.MakeLogEntry(EnumPermType.AppointmentEdit,_listAppointmentsUnsched[grid.SelectedIndices[i]].PatNum,
					Lan.g(this,"Appointment deleted from the Unscheduled list."),_listAppointmentsUnsched[grid.SelectedIndices[i]].AptNum,_listAppointmentsUnsched[grid.SelectedIndices[i]].DateTStamp);
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
			long clinicNum=PrefC.HasClinicsEnabled ? comboClinic.ClinicNumSelected : -1;
			Dictionary<long,string> dictPatNames=null;
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => {
				_listAppointmentsUnsched=Appointments.RefreshUnsched(order,provNum,siteNum,showBrokenAppts,clinicNum,
					codeRangeFilter.StartRange,codeRangeFilter.EndRange,dateRangePicker.GetDateTimeFrom(),dateRangePicker.GetDateTimeTo());
				dictPatNames=Patients.GetPatientNames(_listAppointmentsUnsched.Select(x => x.PatNum).ToList());
			};
			progressOD.ShowDialog();
			int scrollVal=grid.ScrollValue;
			grid.BeginUpdate();
			grid.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableUnsched","Patient"),140);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Date"),65);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","AptStatus"),90);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","UnschedStatus"),110);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Prov"),50);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Procedures"),150);
			grid.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUnsched","Notes"),200);
			grid.Columns.Add(col);
			grid.ListGridRows.Clear();
			GridRow row;
			for(int i = 0;i<_listAppointmentsUnsched.Count;i++) {
				row=new GridRow();
				string patName=Lan.g(this,"UNKNOWN");
				dictPatNames.TryGetValue(_listAppointmentsUnsched[i].PatNum,out patName);
				row.Cells.Add(patName);
				if(_listAppointmentsUnsched[i].AptDateTime.Year < 1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listAppointmentsUnsched[i].AptDateTime.ToShortDateString());
				}
				if(_listAppointmentsUnsched[i].AptStatus == ApptStatus.Broken) {
					row.Cells.Add(Lan.g(this,"Broken"));
				}
				else {
					row.Cells.Add(Lan.g(this,"Unscheduled"));
				}
				row.Cells.Add(Defs.GetName(DefCat.RecallUnschedStatus,_listAppointmentsUnsched[i].UnschedStatus));
				row.Cells.Add(Providers.GetAbbr(_listAppointmentsUnsched[i].ProvNum));
				row.Cells.Add(_listAppointmentsUnsched[i].ProcDescript);
				row.Cells.Add(_listAppointmentsUnsched[i].Note);
				grid.ListGridRows.Add(row);
			}
			grid.EndUpdate();
			grid.ScrollValue=scrollVal;
		}

		private void grid_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			int currentSelection=e.Row;//tbApts.SelectedRow;
			int currentScroll=grid.ScrollValue;//tbApts.ScrollValue;
			Patient patient=Patients.GetPat(_listAppointmentsUnsched[e.Row].PatNum);//If multiple selected, just take the one that was clicked on.
			GlobalFormOpenDental.PatientSelected(patient,true);
			using FormApptEdit formApptEdit=new FormApptEdit(_listAppointmentsUnsched[e.Row].AptNum);
			formApptEdit.PinIsVisible=true;
			formApptEdit.ShowDialog();
			if(formApptEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(formApptEdit.PinClicked) {
				SendPinboard_Click();//Whatever they double clicked on will still be selected, just fire the event.
				DialogResult=DialogResult.OK;//this is an obsolete line. Window stays open.
				return;
			}
			FillGrid();
			grid.SetSelected(currentSelection,true);
			grid.ScrollValue=currentScroll;
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butPrint_Click(object sender,EventArgs e) {
			_pagesPrinted=0;
			_isHeadingPrinted=false;
			PrinterL.TryPrintOrDebugRpPreview(pd_PrintPage,Lan.g(this,"Unscheduled appointment list printed"));
		}

		private void pd_PrintPage(object sender,System.Drawing.Printing.PrintPageEventArgs e) {
			Rectangle rectangleBounds=e.MarginBounds;
			//new Rectangle(50,40,800,1035);//Some printers can handle up to 1042
			Graphics g=e.Graphics;
			string text;
			Font fontHeading=new Font("Arial",13,FontStyle.Bold);
			Font fontSubHeading=new Font("Arial",10,FontStyle.Bold);
			int yPos=rectangleBounds.Top;
			int center=rectangleBounds.X+rectangleBounds.Width/2;
			#region printHeading
			if(!_isHeadingPrinted) {
				text=Lan.g(this,"Unscheduled List");
				g.DrawString(text,fontHeading,Brushes.Black,center-g.MeasureString(text,fontHeading).Width/2,yPos);
				//yPos+=(int)g.MeasureString(text,headingFont).Height;
				//text=textDateFrom.Text+" "+Lan.g(this,"to")+" "+textDateTo.Text;
				//g.DrawString(text,subHeadingFont,Brushes.Black,center-g.MeasureString(text,subHeadingFont).Width/2,yPos);
				yPos+=25;
				_isHeadingPrinted=true;
				_heightHeadingPrint=yPos;
			}
			#endregion
			yPos=grid.PrintPage(g,_pagesPrinted,rectangleBounds,_heightHeadingPrint);
			_pagesPrinted++;
			if(yPos==-1) {
				e.HasMorePages=true;
			}
			else {
				e.HasMorePages=false;
			}
			g.Dispose();
		}

	}
}