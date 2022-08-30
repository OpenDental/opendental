using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormAutoCommExclusionDays : FormODBase {

		private long _clinicNum;
		private List<AutoCommExcludeDate> _listExcludeDates = new List<AutoCommExcludeDate>();
		private AutoCommExcludeDate.AutoCommExcludeDays _excludeDays;
		private ClinicPrefHelper _clinicPrefHelper = new ClinicPrefHelper(PrefName.EConfirmExcludeDays,PrefName.EConfirmExcludeDaysUseHQ);
		/// <summary>TRUE: Using HQ (Clinic 0) exclusion days, don't make exclusion changes for this clinic. FALSE: clinic has their own exclusion days</summary>
		private bool _useHQSettings;

		public FormAutoCommExclusionDays(long clinicNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
		}

		private void FormAutoCommExclusionDays_Load(object sender,EventArgs e) {
			comboBoxClinicPicker.SelectedClinicNum = _clinicNum;
			Reload();
		}

		private void Reload() {
			if(checkShowPastDates.Checked) {
				_listExcludeDates=AutoCommExcludeDates.Refresh(_clinicNum);
			}
			else {
				_listExcludeDates=AutoCommExcludeDates.GetFutureForClinic(_clinicNum);
			}
			string val = _clinicPrefHelper.GetStringVal(PrefName.EConfirmExcludeDays, _clinicNum);
			if(val == AutoCommExcludeDate.AutoCommExcludeDays.None.ToString()) {
				_excludeDays = 0;
			}
			else {
				_excludeDays = (AutoCommExcludeDate.AutoCommExcludeDays)byte.Parse(_clinicPrefHelper.GetStringVal(PrefName.EConfirmExcludeDays, _clinicNum));
			}
			if(_clinicNum>0) {
				_useHQSettings=_clinicPrefHelper.GetBoolVal(PrefName.EConfirmExcludeDaysUseHQ,_clinicNum);
				checkUseHQ.Checked=_useHQSettings;
				checkUseHQ.Visible=true;
			}
			else {
				checkUseHQ.Checked=false;
				checkUseHQ.Visible=false;
			}
			listBoxExclusionDates.Items.Clear();
			listBoxExclusionDates.Items.AddList(_listExcludeDates,x => x.DateExclude.ToShortDateString());
			listBoxExclusionDays.Items.Clear();
			listBoxExclusionDays.Items.AddEnums<AutoCommExcludeDate.AutoCommExcludeDays>();
			listBoxExclusionDays.Items.RemoveAt(0); //remove "none" from list, implied by having nothing selected.
			foreach(AutoCommExcludeDate.AutoCommExcludeDays day in Enum.GetValues(typeof(AutoCommExcludeDate.AutoCommExcludeDays))){
				if(day != AutoCommExcludeDate.AutoCommExcludeDays.None && _excludeDays.HasFlag(day)) {
					listBoxExclusionDays.SetSelectedEnum(day);
				}
			}
		}

		private void ComboBoxClinicPicker_SelectionChangeCommitted(object sender,EventArgs e) {
			if(!ExcludeDaysSelectionOK()) {
				comboBoxClinicPicker.SelectedClinicNum = _clinicNum;
				return;
			}
			Save();
			_clinicNum=comboBoxClinicPicker.SelectedClinicNum;
			Reload();
		}

		private void checkShowPastDates_CheckedChanged(object sender,EventArgs e) {
			Reload();
		}

		private void checkUseHQ_CheckedChanged(object sender,EventArgs e) {
            if(checkUseHQ.Checked && _clinicNum > 0) {
				listBoxExclusionDates.Enabled=false;
				listBoxExclusionDays.Enabled=false;
				butAdd.Enabled=false;
				butDelete.Enabled=false;
				labelExclusionDays.Enabled=false;
				labelExclusionDates.Enabled=false;
				labelUseHQMessage.Visible=true;
			}
            else {
				listBoxExclusionDates.Enabled=true;
				listBoxExclusionDays.Enabled=true;
				butAdd.Enabled=true;
				butDelete.Enabled=true;
				labelExclusionDays.Enabled=true;
				labelExclusionDates.Enabled=true;
				labelUseHQMessage.Visible=false;
			}
		}

		private void Save() {
			AutoCommExcludeDate.AutoCommExcludeDays excludeDays = AutoCommExcludeDate.AutoCommExcludeDays.None;
			listBoxExclusionDays.GetListSelected<AutoCommExcludeDate.AutoCommExcludeDays>().ForEach(day => excludeDays |= day);
			if(checkUseHQ.Checked && _clinicNum>0) {//clinic set to use HQ defaults
				_clinicPrefHelper.ValChangedByUser(PrefName.EConfirmExcludeDaysUseHQ,_clinicNum,POut.Bool(true));
			}
			else if(_clinicNum>0) {//clinic using clinic specific settings
				_clinicPrefHelper.ValChangedByUser(PrefName.EConfirmExcludeDaysUseHQ,_clinicNum,POut.Bool(false));
			}
			//always save exclusion days. When using HQ defaults, list box is disabled, unable to make changed that will save when set to use HQ defaults
			_clinicPrefHelper.ValChangedByUser(PrefName.EConfirmExcludeDays,_clinicNum, ((int)(byte)excludeDays).ToString());
		}

		private bool ExcludeDaysSelectionOK() {
			if(listBoxExclusionDays.SelectedIndices.Count() == 7) {
				MsgBox.Show(this,"Cannot block all days of the week. To block eConfirmations entirely, disable the eConfirmation rule.");
				return false;
            }
			return true;
        }

        private void butAdd_Click(object sender,EventArgs e) {
			Save();
			using FormCalendar formDatePicker = new FormCalendar();
			formDatePicker.DateSelected=DateTime.Now;
			formDatePicker.MinDate=DateTime.Now;
			formDatePicker.ShowDialog();
			if(formDatePicker.DialogResult != DialogResult.OK) {
				return;
			}
			AutoCommExcludeDate autoCommExcludeDate = new AutoCommExcludeDate();
			autoCommExcludeDate.DateExclude=formDatePicker.DateSelected;
			autoCommExcludeDate.ClinicNum=_clinicNum;
            if(_listExcludeDates.Any(x => x.DateExclude == autoCommExcludeDate.DateExclude && x.ClinicNum == autoCommExcludeDate.ClinicNum)) {
				return;
            }
			AutoCommExcludeDates.Insert(autoCommExcludeDate);
			Reload();
        }

        private void butDelete_Click(object sender,EventArgs e) {
			Save();
			AutoCommExcludeDate dateSelected = listBoxExclusionDates.GetSelected<AutoCommExcludeDate>();
			if(dateSelected==null) {
				MsgBox.Show(this,"Please selete a date to delete.");
				return;
			}
			AutoCommExcludeDates.Delete(dateSelected.AutoCommExcludeDateNum);
			Reload();
        }

		private void butOK_Click(object sender,EventArgs e) {
			if(!ExcludeDaysSelectionOK()) {
				return;
			}
			Save();
			_clinicPrefHelper.SyncAllPrefs();
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}