using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.UI;

namespace OpenDental {
	public partial class FormApptPrintSetup:FormODBase {
		public DateTime ApptPrintStartTime;
		public DateTime ApptPrintStopTime;
		public int ApptPrintFontSize;
		public int ApptPrintColsPerPage;
		public bool IsPrintPreview;
		///<summary>Only used if user clicks to print all routing slips for current view.</summary>
		private List<long> _listAptNums;
		private DateTime _dateSelected;
		private bool _isWeeklyView;

		public FormApptPrintSetup(List<long> listAptNums,DateTime dateSelected,bool isWeeklyView) {
			InitializeComponent();
			InitializeLayoutManager();
			_listAptNums=listAptNums;
			_dateSelected=dateSelected;
			_isWeeklyView=isWeeklyView;
			Lan.F(this);
		}

		private void FormApptPrintSetup_Load(object sender,EventArgs e) {
			TimeSpan time;
			string timeStart=PrefC.GetDateT(PrefName.ApptPrintTimeStart).ToShortTimeString();
			string timeStop=PrefC.GetDateT(PrefName.ApptPrintTimeStop).ToShortTimeString();
			for(int i=0;i<=24;i++) {
				time=new TimeSpan(i,0,0);
				comboStart.Items.Add(time.ToShortTimeString());
				comboStop.Items.Add(time.ToShortTimeString());
				if(time.ToShortTimeString()==timeStart) {
					comboStart.SelectedIndex=i;
				}
				if(time.ToShortTimeString()==timeStop) {
					comboStop.SelectedIndex=i;
				}
			}
			textFontSize.Text=PrefC.GetString(PrefName.ApptPrintFontSize);
			textColumnsPerPage.Text=PrefC.GetInt(PrefName.ApptPrintColumnsPerPage).ToString();
			if(PrefC.HasClinicsEnabled && Clinics.ClinicNum==0) {//Hide if clinics is enabled and headquarters is selected
				groupBoxPrintRouting.Enabled=false;
			}
			if(_isWeeklyView) { //Disable if the user has the weekly view selected in the appt module
				groupBoxPrintRouting.Enabled=false;
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!ValidEntries()) {
				return;
			}
			SaveChanges(false);
		}

		private bool ValidEntries() {
			DateTime start=PIn.DateT(comboStart.SelectedItem.ToString());
			DateTime stop=PIn.DateT(comboStop.SelectedItem.ToString());
			if(start.Minute>0 || stop.Minute>0) {
				MsgBox.Show(this,"Please use hours only, no minutes.");
				return false;
			}
			//If stop time is the same as start time and not midnight to midnight.
			if(stop.Hour==start.Hour && (stop.Hour!=0 && start.Hour!=0)) {
				MsgBox.Show(this,"Start time must be different than stop time.");
				return false;
			}
			if(stop.Hour!=0 && stop.Hour<start.Hour) {//If stop time is earlier than start time.
				MsgBox.Show(this,"Start time cannot exceed stop time.");
				return false;
			}
			if(start==DateTime.MinValue) {
				MsgBox.Show(this,"Please enter a valid start time.");
				return false;
			}
			if(stop==DateTime.MinValue) {
				MsgBox.Show(this,"Please enter a valid stop time.");
				return false;
			}
			if(!textColumnsPerPage.IsValid() || !textFontSize.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			if(PIn.Int(textColumnsPerPage.Text)<1) {
				MsgBox.Show(this,"Columns per page cannot be 0 or less.");
				return false;
			}
			return true;
		}

		private void SaveChanges(bool suppressMessage) {
			if(ValidEntries()) {
				Prefs.UpdateDateT(PrefName.ApptPrintTimeStart,PIn.DateT(comboStart.SelectedItem.ToString()));
				Prefs.UpdateDateT(PrefName.ApptPrintTimeStop,PIn.DateT(comboStop.SelectedItem.ToString()));
				Prefs.UpdateString(PrefName.ApptPrintFontSize,textFontSize.Text);
				Prefs.UpdateInt(PrefName.ApptPrintColumnsPerPage,PIn.Int(textColumnsPerPage.Text));
				if(!suppressMessage) {
					MsgBox.Show(this,"Settings saved.");
				}
			}
		}

		private bool PrintViewSetup() {
			bool changed=false;
			if(!ValidEntries()) {
				return false;
			}
			if(PIn.DateT(comboStart.SelectedItem.ToString()).Hour!=PrefC.GetDateT(PrefName.ApptPrintTimeStart).Hour
				|| PIn.DateT(comboStop.SelectedItem.ToString()).Hour!=PrefC.GetDateT(PrefName.ApptPrintTimeStop).Hour
				|| textFontSize.Text!=PrefC.GetString(PrefName.ApptPrintFontSize)
				|| textColumnsPerPage.Text!=PrefC.GetInt(PrefName.ApptPrintColumnsPerPage).ToString())
			{
				changed=true;
			}
			if(changed) {
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Save the changes that were made?")) {
					SaveChanges(true);
				}
			}
			ApptPrintStartTime=PIn.DateT(comboStart.SelectedItem.ToString());
			ApptPrintStopTime=PIn.DateT(comboStop.SelectedItem.ToString());
			ApptPrintFontSize=PIn.Int(textFontSize.Text);
			ApptPrintColsPerPage=PIn.Int(textColumnsPerPage.Text);
			return true;
		}

		private void butPreview_Click(object sender,EventArgs e) {
			if(!PrintViewSetup()) {
				return;
			}
			IsPrintPreview=true;
			DialogResult=DialogResult.OK;
		}

		///<summary>Run the RpRouting report for the selected day, current clinic, and all providers</summary>
		private void butAllForDay_Click(object sender,EventArgs e) {
			using FormRpRouting formRpRouting=new FormRpRouting();
			formRpRouting.DateSelected=_dateSelected;
			formRpRouting.IsAutoRunForDateSelected=true;
			formRpRouting.ShowDialog();
		}

		///<summary>Run the RpRouting report for the currently selected appointment view.</summary>
		private void butCurrentView_Click(object sender,EventArgs e) {
			using FormRpRouting formRpRouting=new FormRpRouting();
			formRpRouting.ListAptNums=_listAptNums;
			formRpRouting.IsAutoRunForListAptNums=true;
			formRpRouting.ShowDialog();
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!PrintViewSetup()) {
				return;
			}
			IsPrintPreview=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		
		
	}
}