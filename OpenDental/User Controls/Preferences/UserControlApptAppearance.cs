using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlApptAppearance:UserControl {
		
		#region Fields - Private
		private ColorDialog _colorDialog=new ColorDialog();
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		#endregion Fields - Private

		#region Constructors
		public UserControlApptAppearance() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butApptLineColor_Click(object sender,EventArgs e) {
			_colorDialog.Color=butColor.BackColor;//Pre-select current pref color
			if(_colorDialog.ShowDialog()==DialogResult.OK) {
				butApptLineColor.BackColor=_colorDialog.Color;
			}
			_colorDialog.Dispose();
		}

		private void butColor_Click(object sender,EventArgs e) {
			_colorDialog.Color=butColor.BackColor;//Pre-select current pref color
			if(_colorDialog.ShowDialog()==DialogResult.OK) {
				butColor.BackColor=_colorDialog.Color;
			}
			_colorDialog.Dispose();
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private List<string> UpdateDefNumsList(PrefName pref, List<string> listAppend) {
			listAppend.Remove("0");
			return PrefC.GetString(pref).Split(',')
				.Where(x => !string.IsNullOrWhiteSpace(x) && x!="0")
				.Union(listAppend).ToList();
		}
		#endregion Methods - Private

		#region Methods - Public
		public void FillApptAppearance() {
			checkSolidBlockouts.Checked=PrefC.GetBool(PrefName.SolidBlockouts);
			checkApptBubbleDelay.Checked = PrefC.GetBool(PrefName.ApptBubbleDelay);
			checkAppointmentBubblesDisabled.Checked=PrefC.GetBool(PrefName.AppointmentBubblesDisabled);			
			checkApptExclamation.Checked=PrefC.GetBool(PrefName.ApptExclamationShowForUnsentIns);
			checkApptRefreshEveryMinute.Checked=PrefC.GetBool(PrefName.ApptModuleRefreshesEveryMinute);
			for(int i=0;i<11;i++) {
				double seconds=(double)i/10;
				if(i==0) {
					comboDelay.Items.Add(Lan.g(this,"No delay"),seconds);
				} 
				else {
					comboDelay.Items.Add(seconds.ToString("f1") + " "+Lan.g(this,"seconds"),seconds);
				}
				if(PrefC.GetDouble(PrefName.FormClickDelay,doUseEnUSFormat: true)==seconds) {
					comboDelay.SelectedIndex=i;
				}
			}
			butColor.BackColor=PrefC.GetColor(PrefName.WaitingRoomAlertColor);
			butApptLineColor.BackColor=PrefC.GetColor(PrefName.AppointmentTimeLineColor);
			checkApptModuleDefaultToWeek.Checked=PrefC.GetBool(PrefName.ApptModuleDefaultToWeek);
			comboWeekViewStartDay.Items.Add(Lan.g(this,"Sunday"));
			comboWeekViewStartDay.Items.Add(Lan.g(this,"Monday"));
			comboWeekViewStartDay.SetSelected(PrefC.GetInt(PrefName.ApptWeekViewStartDay));
			textApptFontSize.Text=PrefC.GetString(PrefName.ApptFontSize);
			textApptProvbarWidth.Text=PrefC.GetString(PrefName.ApptProvbarWidth);
			comboTimeArrived.Items.AddDefNone();
			comboTimeArrived.SelectedIndex=0;
			comboTimeArrived.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeArrived.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeArrivedTrigger));
			comboTimeSeated.Items.AddDefNone();
			comboTimeSeated.SelectedIndex=0;
			comboTimeSeated.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeSeated.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeSeatedTrigger));
			comboTimeDismissed.Items.AddDefNone();
			comboTimeDismissed.SelectedIndex=0;
			comboTimeDismissed.Items.AddDefs(Defs.GetDefsForCategory(DefCat.ApptConfirmed,true));
			comboTimeDismissed.SetSelectedDefNum(PrefC.GetLong(PrefName.AppointmentTimeDismissedTrigger));
			checkWaitingRoomFilterByView.Checked=PrefC.GetBool(PrefName.WaitingRoomFilterByView);
			textWaitRoomWarn.Text=PrefC.GetInt(PrefName.WaitingRoomAlertTime).ToString();
			textApptBubNoteLength.Text=PrefC.GetInt(PrefName.AppointmentBubblesNoteLength).ToString();
			checkUseOpHygProv.Checked=PrefC.GetBool(PrefName.ApptSecondaryProviderConsiderOpOnly);
			comboApptSchedEnforceSpecialty.Items.AddList(Enum.GetValues(typeof(ApptSchedEnforceSpecialty)).OfType<ApptSchedEnforceSpecialty>()
				.Select(x => x.GetDescription()).ToArray());
			comboApptSchedEnforceSpecialty.SelectedIndex=PrefC.GetInt(PrefName.ApptSchedEnforceSpecialty);
			if(!PrefC.HasClinicsEnabled) {
				comboApptSchedEnforceSpecialty.Visible=false;
				labelApptSchedEnforceSpecialty.Visible=false;
			}
			checkReplaceBlockouts.Checked=PrefC.GetBool(PrefName.ReplaceExistingBlockout);
		}

		public bool SaveApptAppearance() {
			int noteLength=0;
			if(!int.TryParse(textApptBubNoteLength.Text,out noteLength)) {
				MsgBox.Show(this,"Max appointment note length is invalid. Please enter a valid number to continue.");
				return false;
			}
			if(noteLength<0) {
				MsgBox.Show(this,"Max appointment note length cannot be a negative number.");
				return false;
			}
			float apptFontSize=0;
			if(!float.TryParse(textApptFontSize.Text,out apptFontSize)){
				MsgBox.Show(this,"Appt Font Size invalid.");
				return false;
			}
			if(apptFontSize<1 || apptFontSize>40){
				MsgBox.Show(this,"Appt Font Size must be between 1 and 40.");
				return false;
			}
			if(!textApptProvbarWidth.IsValid()) {
				MsgBox.Show(this,"Please fix data errors first.");
				return false;
			}
			int waitingRoomAlertTime=0;
			try {
				waitingRoomAlertTime=PIn.Int(textWaitRoomWarn.Text);
				if(waitingRoomAlertTime<0) {
					throw new ApplicationException("Waiting room time cannot be negative");//User never sees this message.
				}
			}
			catch {
				MsgBox.Show(this,"Waiting room alert time is invalid.");
				return false;
			}
			Changed|=Prefs.UpdateInt(PrefName.AppointmentBubblesNoteLength,noteLength);
			Changed|=Prefs.UpdateBool(PrefName.AppointmentBubblesDisabled,checkAppointmentBubblesDisabled.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptBubbleDelay,checkApptBubbleDelay.Checked);
			Changed|=Prefs.UpdateBool(PrefName.SolidBlockouts,checkSolidBlockouts.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptExclamationShowForUnsentIns,checkApptExclamation.Checked);
			Changed|=Prefs.UpdateBool(PrefName.ApptModuleRefreshesEveryMinute,checkApptRefreshEveryMinute.Checked);
			Changed|=Prefs.UpdateInt(PrefName.WaitingRoomAlertColor,butColor.BackColor.ToArgb());
			Changed|=Prefs.UpdateInt(PrefName.AppointmentTimeLineColor,butApptLineColor.BackColor.ToArgb());
			Changed|=Prefs.UpdateBool(PrefName.ApptModuleDefaultToWeek,checkApptModuleDefaultToWeek.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ApptWeekViewStartDay,comboWeekViewStartDay.SelectedIndex);
			Changed|=Prefs.UpdateDouble(PrefName.FormClickDelay,comboDelay.GetSelected<double>(),doUseEnUSFormat:true);
			Changed|=Prefs.UpdateString(PrefName.ApptFontSize,apptFontSize.ToString());
			Changed|=Prefs.UpdateInt(PrefName.ApptProvbarWidth,PIn.Int(textApptProvbarWidth.Text));
			Changed|=Prefs.UpdateBool(PrefName.WaitingRoomFilterByView,checkWaitingRoomFilterByView.Checked);
			Changed|=Prefs.UpdateInt(PrefName.WaitingRoomAlertTime,waitingRoomAlertTime);
			Changed|=Prefs.UpdateBool(PrefName.ApptSecondaryProviderConsiderOpOnly,checkUseOpHygProv.Checked);
			Changed|=Prefs.UpdateInt(PrefName.ApptSchedEnforceSpecialty,comboApptSchedEnforceSpecialty.SelectedIndex);
			Changed|=Prefs.UpdateBool(PrefName.ReplaceExistingBlockout,checkReplaceBlockouts.Checked);
			long timeArrivedTrigger=0;
			if(comboTimeArrived.SelectedIndex>0){
				timeArrivedTrigger=comboTimeArrived.GetSelectedDefNum();
			}
			List<string> listTriggerNewNums=new List<string>();
			if(Prefs.UpdateLong(PrefName.AppointmentTimeArrivedTrigger,timeArrivedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeArrivedTrigger));
				Changed=true;
			}
			long timeSeatedTrigger=0;
			if(comboTimeSeated.SelectedIndex>0){
				timeSeatedTrigger=comboTimeSeated.GetSelectedDefNum();
			}
			if(Prefs.UpdateLong(PrefName.AppointmentTimeSeatedTrigger,timeSeatedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeSeatedTrigger));
				Changed=true;
			}
			long timeDismissedTrigger=0;
			if(comboTimeDismissed.SelectedIndex>0){
				timeDismissedTrigger=comboTimeDismissed.GetSelectedDefNum();
			}
			if(Prefs.UpdateLong(PrefName.AppointmentTimeDismissedTrigger,timeDismissedTrigger)){
				listTriggerNewNums.Add(POut.Long(timeDismissedTrigger));
				Changed=true;
			}
			if(listTriggerNewNums.Count>0) {
				//Adds the appointment triggers to the list of confirmation statuses excluded from sending eConfirms,eReminders, and eThankYous.
				List<string> listEConfirm=UpdateDefNumsList(PrefName.ApptConfirmExcludeEConfirm,listTriggerNewNums);
				List<string> listESend=UpdateDefNumsList(PrefName.ApptConfirmExcludeESend,listTriggerNewNums);
				List<string> listERemind=UpdateDefNumsList(PrefName.ApptConfirmExcludeERemind,listTriggerNewNums);
				List<string> listEThanks=UpdateDefNumsList(PrefName.ApptConfirmExcludeEThankYou,listTriggerNewNums);
				List<string> listExcludeEclipboard=UpdateDefNumsList(PrefName.ApptConfirmExcludeEclipboard,listTriggerNewNums);
				List<string> listArrivalSMS=UpdateDefNumsList(PrefName.ApptConfirmExcludeArrivalSend,listTriggerNewNums);
				List<string> listArrivalResponse=UpdateDefNumsList(PrefName.ApptConfirmExcludeArrivalResponse,listTriggerNewNums);
				List<string> listGeneralMessage=UpdateDefNumsList(PrefName.ApptConfirmExcludeGeneralMessage,listTriggerNewNums);
				//Update new Value strings in database.  We don't remove the old ones.
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEConfirm,string.Join(",",listEConfirm));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeESend,string.Join(",",listESend));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeERemind,string.Join(",",listERemind));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEThankYou,string.Join(",",listEThanks));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeEclipboard,string.Join(",",listExcludeEclipboard));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalSend,string.Join(",",listArrivalSMS));
				Prefs.UpdateString(PrefName.ApptConfirmExcludeArrivalResponse,string.Join(",",listArrivalResponse));
				//Don't include dismissed trigger.
				listGeneralMessage=listGeneralMessage.Where(x => x!=timeDismissedTrigger.ToString()).ToList();
				Prefs.UpdateString(PrefName.ApptConfirmExcludeGeneralMessage,string.Join(",",listGeneralMessage));
			}
			List<string> listByod=UpdateDefNumsList(PrefName.ApptConfirmByodEnabled,new List<string> { timeArrivedTrigger.ToString() });
			//Only needs to be updated with the arrival trigger
			Prefs.UpdateString(PrefName.ApptConfirmByodEnabled,string.Join(",",listByod));
			return true;
		}
		#endregion Methods - Public
	}
}
