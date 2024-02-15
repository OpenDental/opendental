using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;

namespace OpenDental {
	public partial class FormApptSearchAdvanced:FormODBase {
		#region Private Variables
		///<summary>The appointment passed in that we are trying to find a slot for.</summary>
		private Appointment _appointment;
		/// <summary>The list of providers possibly set on previous search window before entering advanced search.</summary>
		private List<long> _listProvNums=new List<long>();
		/// <summary>The before time that was possibly set before entering this window. </summary>
		private string _beforeTime;
		/// <summary>The after time that was possibly set before entering this window. </summary>
		private string _afterTime;
		/// <summary>The after date that was possibly set before entering this window. </summary>
		private DateTime _dateAfter;
		private List<ScheduleOpening> _listScheduleOpenings=new List<ScheduleOpening>();
		#endregion

		///<summary>Pass in the currently selected apptNum along with all ApptNums that are associated to the current pinboard.</summary>
		public FormApptSearchAdvanced(long apptNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_appointment=Appointments.GetOneApt(apptNum);
			if(_appointment==null) {
				MsgBox.Show(this,"Invalid appointment on the Pinboard.");
				DialogResult=DialogResult.Abort;
				return;
			}
		}

		private void FormApptSearchAdvanced_Load(object sender,EventArgs e) {
			if(_dateAfter!=DateTime.MinValue.Date && _dateAfter!=null) {
				dateSearchFrom.Text=_dateAfter.ToShortDateString();
			}
			else {
				dateSearchFrom.Text=DateTime.Today.AddDays(1).ToShortDateString();
			}
			dateSearchTo.Text=PIn.Date(dateSearchFrom.Text).AddYears(2).AddDays(1).ToShortDateString();//default to 2 years from the afterDate. 
			textBefore.Text=_beforeTime;//just blindly set these. They will be validated when the search is ran. 
			textAfter.Text=_afterTime;
			//Fill all combo and listboxes.
			FillClinics();
			FillApptViews();
			FillBlockouts();
			FillProviders(GetProvidersForSelectedClinic(),_listProvNums);
		}

		internal void SetSearchArgs(long apptNum,List<long> listProvNums,string beforeTime,string afterTime,DateTime dateAfter) {
			_listProvNums=listProvNums;
			_beforeTime=beforeTime;
			_afterTime=afterTime;
			_dateAfter=dateAfter.Date;
		}

		///<summary>Fills comboBox with providers.  Optionally pass in a list of providers to set prov box with specific provs.</summary>
		private void FillProviders(List<Provider> listProvidersForClinic,List<long> listProvNumsToSelect=null) {
			if(listProvNumsToSelect==null || listProvNumsToSelect.Count==0) {
				listProvNumsToSelect=new List<long>();
			}
			List<int> listSelectedIndices=new List<int>();
			comboBoxMultiProv.Items.Clear();
			comboBoxMultiProv.Items.AddProvNone();
			comboBoxMultiProv.Items.AddProvsFull(listProvidersForClinic);
			for(int i=0;i<listProvidersForClinic.Count;i++) {
				if(listProvNumsToSelect.Contains(listProvidersForClinic[i].ProvNum)) {
					comboBoxMultiProv.SetSelected(i+1,true);//becuase listProvsForClinic doesn't contain none
				}
			}
			if(comboBoxMultiProv.GetListSelected<Provider>().Count==0) {//no providers coming in, set default to 'none'
				comboBoxMultiProv.SetSelected(0,true);
			}
		}

		private List<Provider> GetProvidersForSelectedClinic(ProvMode provMode=ProvMode.All) {
			List<Provider> listProviders=Providers.GetProvsForClinic(comboBoxClinic.ClinicNumSelected);//returns all for no clinics
			switch(provMode) {
				case ProvMode.Dent:
					listProviders.RemoveAll(x => x.IsSecondary);
					break;
				case ProvMode.Hyg:
					listProviders.RemoveAll(x => !x.IsSecondary);
					break;
				case ProvMode.All:
				default:
					break;
			}
			return listProviders;
		}

		///<summary>Puts the form into clinic mode when using the clinics feature.  Otherwise; does nothing.</summary>
		private void FillClinics() {
			if(!PrefC.HasClinicsEnabled) {
				return;
			}
			if(comboBoxClinic.IsNothingSelected) {
				comboBoxClinic.ClinicNumSelected=0;//unsassigned
			}
			if(comboBoxClinic.IsUnassignedSelected) {
				comboApptView.Visible=true;
				labelAptViews.Visible=true;
			}
		}

		///<summary>Only fill appt views when clinic is HQ. </summary>
		private void FillApptViews() {
			if(!PrefC.HasClinicsEnabled || comboBoxClinic.ClinicNumSelected > 0) {
				return;//Either clinics are enabled and we're on a sepecific clinic OR clinics are not enabled. In either case don't fill appt views. 
			}
			List<ApptView> listApptViews=ApptViews.GetForClinic(comboBoxClinic.ClinicNumSelected);
			comboApptView.Items.Clear();
			for(int i=0;i<listApptViews.Count;i++) {
				comboApptView.Items.Add(listApptViews[i].Description,listApptViews[i]);
			}
			if(listApptViews.Count!=0) {//default to the first
				comboApptView.SelectedIndex=0;
			}
		}

		private void FillBlockouts() {
			comboBlockout.Items.Clear();
			comboBlockout.Items.AddDefNone();//if they select 'None' then only search for the provider(s).
			List<Def> listDefsBlockout=Defs.GetDefsForCategory(DefCat.BlockoutTypes,isShort: true);
			for(int i=0;i<listDefsBlockout.Count;i++){
				if(listDefsBlockout[i].ItemValue.Contains(BlockoutType.NoSchedule.GetDescription())) {
					continue;//The search will already not show results for Do Not Schedule Blockout types, so users shouldn't be able to select them.
				}
				comboBlockout.Items.Add(listDefsBlockout[i].ItemName,listDefsBlockout[i]);
			}
			comboBlockout.SelectedIndex=0;//default to none
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(gridMain.TranslationName,"Day"),85);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Date"),85,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(gridMain.TranslationName,"Time"),85,HorizontalAlignment.Center);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listScheduleOpenings.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listScheduleOpenings[i].DateTimeAvail.DayOfWeek.ToString());
				row.Cells.Add(_listScheduleOpenings[i].DateTimeAvail.Date.ToShortDateString());
				row.Cells.Add(_listScheduleOpenings[i].DateTimeAvail.ToShortTimeString());
				row.Tag=_listScheduleOpenings[i];
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void DoSearch() {
			Cursor=Cursors.WaitCursor;
			DateTime startDate=dateSearchFrom.Value.Date.AddDays(-1);//Text on boxes is To/From. This will effecitvely make it the 'afterDate'. 
			DateTime endDate=dateSearchTo.Value.Date.AddDays(1);
			_listScheduleOpenings.Clear();
			#region validation
			if(startDate.Year < 1880 || endDate.Year < 1880) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Invalid date selection.");
				return;
			}
			TimeSpan beforeTime=new TimeSpan(0);
			if(textBefore.Text!="") {
				try {
					beforeTime=GetBeforeAfterTime(textBefore.Text,radioBeforePM.Checked);
				}
				catch {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Invalid 'Starting before' time.");
					return;
				}
			}
			TimeSpan afterTime=new TimeSpan(0);
			if(textAfter.Text!="") {
				try {
					afterTime=GetBeforeAfterTime(textAfter.Text,radioAfterPM.Checked);
				}
				catch {
					Cursor=Cursors.Default;
					MsgBox.Show(this,"Invalid 'Starting after' time.");
					return;
				}
			}
			//If appointment search behavior is 'Provider Time' then not allowed to have provider be none
			if(PrefC.GetInt(PrefName.AppointmentSearchBehavior)==0 && comboBoxMultiProv.GetSelectedProvNums().Contains(0)) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please pick a provider.");
				return;
			}
			//This message will only show for 'Provider Time Operatory' searching
			if(comboBoxMultiProv.GetSelectedProvNums().Contains(0) && comboBlockout.GetSelectedDefNum()==0) {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"Please pick a provider and/or a blockout type.");
				return;
			}
			#endregion
			//get lists of info to do the search
			List<long> listOpNums=new List<long>();
			List<long> listClinicNums=new List<long>();
			List<long> listProvNums=new List<long>();
			long blockoutType=0;
			if(comboBlockout.GetSelectedDefNum()!=0) {
				blockoutType=comboBlockout.GetSelectedDefNum();
				listProvNums.Add(0);//providers don't matter for blockouts
			}
			if(!comboBoxMultiProv.GetSelectedProvNums().Contains(0)) {
				List<long> listProvNumsSelected=comboBoxMultiProv.GetSelectedProvNums();
				for(int i=0;i<listProvNumsSelected.Count;i++) {
					listProvNums.Add(listProvNumsSelected[i]);
				}
			}
			if(PrefC.HasClinicsEnabled) {
				if(comboBoxClinic.ClinicNumSelected==0) {//HQ //and unassigned (which is clinic num 0)
					long apptViewNum=comboApptView.GetSelected<ApptView>().ApptViewNum;
					//get the disctinct clinic nums for the operatories in the current appointment view
					List<long> listOpsForView=ApptViewItems.GetOpsForView(apptViewNum);
					List<Operatory> listOperatories=Operatories.GetOperatories(listOpsForView,isShort: true);
					listClinicNums=listOperatories.Select(x => x.ClinicNum).Distinct().ToList();
					listOpNums=listOperatories.Select(x => x.OperatoryNum).ToList();
				}
				else {
					listClinicNums.Add(comboBoxClinic.ClinicNumSelected);
					listOpNums=Operatories.GetOpsForClinic(comboBoxClinic.ClinicNumSelected).Select(x => x.OperatoryNum).ToList();
				}
			}
			else {//no clinics
				listOpNums=Operatories.GetDeepCopy(isShort: true).Select(x => x.OperatoryNum).ToList();
			}
			if(blockoutType!=0 && listProvNums.Max()>0) {
				_listScheduleOpenings.AddRange(ApptSearch.GetSearchResultsForBlockoutAndProvider(listProvNums,_appointment.AptNum,startDate,endDate,listOpNums,listClinicNums
					,beforeTime,afterTime,new List<long>(){blockoutType},15));
			}
			else {
				_listScheduleOpenings=ApptSearch.GetSearchResults(_appointment.AptNum,startDate,endDate,listProvNums,listOpNums,listClinicNums
					,beforeTime,afterTime,new List<long>(){blockoutType},resultCount:15);
			}
			Cursor=Cursors.Default;
			FillGrid();
		}

		///<summary>Helper method that returns a TimeSpan based on the time fields specified by the user.</summary>
		private TimeSpan GetBeforeAfterTime(string timeText,bool isAfterPM) {
			TimeSpan timeSpan;
			string[] hrMin=timeText.Split(new char[] { ':' },StringSplitOptions.RemoveEmptyEntries);//doesn't work with foreign times.
			string hr="0";
			if(hrMin.Length > 0) {
				hr=hrMin[0];
			}
			string min="0";
			if(hrMin.Length > 1) {
				min=hrMin[1];
			}
			timeSpan=TimeSpan.FromHours(PIn.Double(hr))+TimeSpan.FromMinutes(PIn.Double(min));
			if(isAfterPM && timeSpan.Hours < 12) {
				timeSpan=timeSpan+TimeSpan.FromHours(12);
			}
			return timeSpan;
		}

		private void butProviders_Click(object sender,EventArgs e) {
			List<Provider> listProviders=null;
			listProviders=comboBoxMultiProv.Items.GetAll<Provider>();
			using FormProvidersMultiPick formProvidersMultiPick=new FormProvidersMultiPick(listProviders);
			formProvidersMultiPick.ListProvidersSelected=comboBoxMultiProv.GetListSelected<Provider>();
			formProvidersMultiPick.ShowDialog();
			if(formProvidersMultiPick.DialogResult!=DialogResult.OK) {
				return;
			}
			List<long> listProvNums=new List<long>();
			for(int i=0;i<formProvidersMultiPick.ListProvidersSelected.Count;i++) {
				listProvNums.Add(formProvidersMultiPick.ListProvidersSelected[i].ProvNum);
			}
			FillProviders(GetProvidersForSelectedClinic(),listProvNums);
		}

		private void comboBoxMultiProv_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxMultiProv.GetSelectedProvNums().Contains(0)) {
				//If the user every has 'none' selected, force it to be the only item selected.
				comboBoxMultiProv.SetSelected(0,true);
			}
		}

		private void comboBoxClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			if(comboBoxClinic.ClinicNumSelected==0) {
				comboApptView.Visible=true;
				labelAptViews.Visible=true;
				FillApptViews();
			}
			else {
				comboApptView.Visible=false;
				labelAptViews.Visible=false;
			}
			List<long> listCurProvNums=comboBoxMultiProv.GetSelectedProvNums().FindAll(x => x!=0).ToList();
			//attempt to re-select the providers if they still exist in the newly selected clinic.
			FillProviders(GetProvidersForSelectedClinic(),listCurProvNums);
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			//get the day for the row that was clicked on.
			DateTime rowDate=((ScheduleOpening)gridMain.ListGridRows[e.Row].Tag).DateTimeAvail;
			//move the calendar day on the appt module to the day that was clicked on. 
			GlobalFormOpenDental.GotoAppointment(rowDate.Date,_appointment.AptNum);
			//if clinics, move to the clinic as well? 
		}

		private void butMore_Click(object sender,EventArgs e) {
			if(_listScheduleOpenings.Count<1) {
				return;
			}
			//use the same appointment we were using from before, even if there is a new appointment on the pinboard
			dateSearchFrom.Text=_listScheduleOpenings[_listScheduleOpenings.Count-1].DateTimeAvail.ToShortDateString();
			DoSearch();//should we prevent them (disable the button if they have changed any of their settings? 
		}

		private void butProvDentist_Click(object sender,EventArgs e) {
			List<Provider> listProvidersDent=GetProvidersForSelectedClinic(ProvMode.Dent);
			FillProviders(listProvidersDent,listProvidersDent.Select(x => x.ProvNum).ToList());//fill box with dentists and select them all
			DoSearch();
		}

		private void butProvHygenist_Click(object sender,EventArgs e) {
			List<Provider> listProvidersHyg=GetProvidersForSelectedClinic(ProvMode.Hyg);
			FillProviders(listProvidersHyg,listProvidersHyg.Select(x => x.ProvNum).ToList());
			DoSearch();
		}

		private void butSearch_Click(object sender,EventArgs e) { 
			//validate that there is an appointment on the pinboard. 
			if(_appointment.AptNum<=0) {
				MsgBox.Show(this,"Invalid appointments on pinboard.");
				return;
			}
			DoSearch();
			butMore.Enabled=true;
		}

		private enum ProvMode {
			All,
			Dent,
			Hyg
		}
	}
}