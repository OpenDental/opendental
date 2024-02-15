using OpenDentBusiness;
using System;
using System.Windows;
using System.Windows.Input;

namespace OpenDental {
	/// <summary>
	/// Interaction logic for FrmFamilyBalancerAutoSettingsEdit.xaml
	/// </summary>
	public partial class FrmFamilyBalancerSetup : FrmODBase {
		public FrmFamilyBalancerSetup() {
			InitializeComponent();
			Load+=FrmFamilyBalancerAutoEdit_Load;
			PreviewKeyDown+=FrmFamilyBalancerSetup_PreviewKeyDown;
		}

		private void FrmFamilyBalancerAutoEdit_Load(object sender,System.EventArgs e) {
			checkEnabled.Checked=PrefC.GetBool(PrefName.FamilyBalancerEnabled);
			bool useFIFO=PrefC.GetBool(PrefName.FamilyBalancerUseFIFO);
			textTimeRun.Text=PrefC.GetDateT(PrefName.FamilyBalancerTimeRun).ToShortTimeString();
			radioFIFO.Checked=useFIFO;
			radioRigorous.Checked=!useFIFO;
			bool useLastRunDate=PrefC.GetBool(PrefName.FamilyBalancerChangedSinceUseLastDayRun);
			radioLastRunDate.Checked=useLastRunDate;
			radioDaysBefore.Checked=!useLastRunDate;
			textVIntDays.IsEnabled=!useLastRunDate;
			textVIntDays.Text=PrefC.GetString(PrefName.FamilyBalancerChangedSinceNumDays);
			checkDeleteAll.Checked=PrefC.GetBool(PrefName.FamilyBalancerDeleteAllTransfers);
		}

		private void TextVIntDayToggleEnabled() {
			textVIntDays.IsEnabled=radioDaysBefore.Checked;
			Visibility visibility=Visibility.Hidden;
			if(radioDaysBefore.Checked && !textVIntDays.IsValid()) { //Only show the error if radioDaysBefore is selected and we don't have a valid number entry.
				visibility=Visibility.Visible;
			}
			textVIntDays.canvasError.Visibility=visibility;
		}

		private void radioLastRunDate_Click(object sender,System.EventArgs e) {
			TextVIntDayToggleEnabled();
		}

		private void radioDaysBefore_Click(object sender,System.EventArgs e) {
			TextVIntDayToggleEnabled();
		}

		private void FrmFamilyBalancerSetup_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,System.EventArgs e) {
			if(!radioFIFO.Checked && !radioRigorous.Checked) {
				MsgBox.Show("Please select either FIFO or Rigorous.");
				return;
			}
			if(!radioDaysBefore.Checked && !radioLastRunDate.Checked) {
				MsgBox.Show("Please select either Last Run Date or Days Before Today");
				return;
			}
			if(textVIntDays.IsEnabled){
				if(!textVIntDays.IsValid()) {
					MsgBox.Show("Please enter a number for number of Days Before Today.");
					return;
				}
				if(textVIntDays.Value<0) {
					MsgBox.Show("Number of Days Before Today must be 0 or greater.");
					return;
				}
			}
			DateTime dateTimeRun;
			try {
				dateTimeRun=DateTime.Parse(textTimeRun.Text);
			}
			catch {
				MsgBox.Show("Please enter a valid Time To Run.");
				return;
			}
			bool changed=false;
			changed|=Prefs.UpdateBool(PrefName.FamilyBalancerEnabled,checkEnabled.Checked==true);
			changed|=Prefs.UpdateBool(PrefName.FamilyBalancerUseFIFO,radioFIFO.Checked);
			changed|=Prefs.UpdateBool(PrefName.FamilyBalancerChangedSinceUseLastDayRun,radioLastRunDate.Checked);
			changed|=Prefs.UpdateDateT(PrefName.FamilyBalancerTimeRun,dateTimeRun);
			if(radioDaysBefore.Checked==true) {
				changed|=Prefs.UpdateLong(PrefName.FamilyBalancerChangedSinceNumDays,textVIntDays.Value);
			}
			changed|=Prefs.UpdateBool(PrefName.FamilyBalancerDeleteAllTransfers,checkDeleteAll.Checked==true);
			if(changed) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			IsDialogOK=true;
		}
	}
}
