using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.WebTypes;

namespace OpenDental {
	///<summary>Form used to manage kiosk terminals and load/clear patient forms displayed on the kiosks.</summary>
	public partial class FormTerminalManager:FormODBase {
		private bool _isSetupMode;
		private Appointment _appt;

		///<summary></summary>
		public FormTerminalManager(bool setupMode=false,Appointment appt=null) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isSetupMode=setupMode;
			groupBoxPassword.Visible=_isSetupMode;
			_appt=appt;
		}

		private void FormTerminalManager_Load(object sender,EventArgs e) {
			PatientChangedEvent.Fired+=PatientChangedEvent_Fired;
			EClipboardEvent.Fired+=eClipboardChangedEvent_Fired;
			textPassword.Text=PrefC.GetString(PrefName.TerminalClosePassword);
			FillGrid();
			contrClinicPicker.SelectionChangeCommitted+=contrClinicPick_SelectionChangeCommitted;
			butByod.Enabled=_appt!=null && OpenDentBusiness.AutoComm.Byod.IsEnabledForConfirmed(_appt.Confirmed,_appt.ClinicNum,out string error);
		}

		public override void ProcessSignalODs(List<Signalod> listSignals) {
			if(listSignals.Any(x => x.IType==InvalidType.Prefs)) {
				textPassword.Text=PrefC.GetString(PrefName.TerminalClosePassword);
			}
			int processIdCur=Process.GetCurrentProcess().Id;
			if(listSignals.All(x => x.IType!=InvalidType.Kiosk || (x.FKeyType==KeyType.ProcessId && x.FKey==processIdCur))) {
				return;
			}
			FillGrid();
		}

		private void FillGrid() {
			SheetDevice selected=new SheetDevice(new MobileAppDevice());//just instantiate to something random that won't match any of our actual devices
			if(gridMain.GetSelectedIndex()>-1) {
				selected=(SheetDevice)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			}
			List<SheetDevice> listDevices=new List<SheetDevice>();
			foreach(TerminalActive kiosk in TerminalActives.Refresh()) {
				listDevices.Add(new SheetDevice(kiosk));
			}
			List<MobileAppDevice> listMobileDevices=new List<MobileAppDevice>();
			if(PrefC.HasClinicsEnabled) {
				//Option "All" is selected and at least one clinic is signed up for the eClipboard feature
				if(contrClinicPicker.IsAllSelected && PrefC.GetString(PrefName.EClipboardClinicsSignedUp)!="") {
					listMobileDevices=MobileAppDevices.GetForUser(Security.CurUser).FindAll(x => x.IsAllowed);
				}
				//A specific clinic is selected and that is signed up for the eClipboard feature
				else if(MobileAppDevices.IsClinicSignedUpForEClipboard(contrClinicPicker.SelectedClinicNum)) {
					listMobileDevices=MobileAppDevices.GetForUser(Security.CurUser).FindAll(x => x.IsAllowed && x.ClinicNum==contrClinicPicker.SelectedClinicNum);
				}
			}
			//We aren't using clinics and the zero clinic is signed up
			else if(MobileAppDevices.IsClinicSignedUpForEClipboard(0)) {
				listMobileDevices=MobileAppDevices.GetForUser(Security.CurUser).FindAll(x => x.IsAllowed);
			}
			//Add the clinics we decided on the the d
			foreach(MobileAppDevice device in listMobileDevices) {
				listDevices.Add(new SheetDevice(device));
			}
			listDevices.Sort((x,y) => {
				//First, Kisok devices. Next, eClipboard devices. Last, BYOD devices.
				if(x.IsKiosk && y.IsKiosk) {
					return x.Name.CompareTo(y.Name);
				}
				if(x.IsKiosk) {
					return -1;
				}
				if(y.IsKiosk) {
					return 1;
				}
				bool boolX=x.MobileDevice?.IsBYODDevice??false;
				bool boolY=y.MobileDevice?.IsBYODDevice??false;
				int ret=boolX.CompareTo(boolY);
				if(ret==0) {
					ret=x.Name.CompareTo(y.Name);
				}
				return ret;
			});			
			int selectedIndex=-1;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("Device Name",135);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Session Name",110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("Patient",150);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn("BYOD",50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn("Clinic",150);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn("Action",50,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			if(_isSetupMode) {
				col=new GridColumn("Delete",50,HorizontalAlignment.Center);
				gridMain.ListGridColumns.Add(col);
			} 
			gridMain.ListGridRows.Clear();
			foreach(SheetDevice device in listDevices) {
				GridRow row=new GridRow();
				row.Tag=device;
				if(device.IsMobileAppDevice){
					row.Cells.Add(new GridCell(device.Name+"\r\n("+device.MobileDevice.UniqueID+")"));
				}
				else{
					row.Cells.Add(new GridCell(device.Name));
				}
				row.Cells.Add(new GridCell(device.SessionName));
				row.Cells.Add(new GridCell(device.PatName));
				if(device.IsMobileAppDevice) {
					row.Cells.Add(new GridCell(device.MobileDevice.IsBYODDevice ? "X" : " "));
				}
				else {
					row.Cells.Add(new GridCell(" "));
				}
				if(PrefC.HasClinicsEnabled) {
					row.Cells.Add(new GridCell(device.ClinicDesc));
				}
				#region Load/Clear click handler
				void CellClick(object sender,EventArgs e) {
					FillGrid();
					if(device.IsMobileAppDevice && device.MobileDevice.IsBYODDevice) {
						MsgBox.Show(this,"Use delete click for BYOD devices.");
						return;
					}
					if(device.PatNum==0) { //we are trying to load the patient
						if(FormOpenDental.CurPatNum==0) {
							MsgBox.Show(this,"There is currently no patient selected to send to the device. Select a patient in Open Dental " +
								"in order to continue.");
							return;
						}
						if(device.IsKiosk) { //kiosk only
							if(listSheets.Items.Count==0) { //eClipboard will allow to continue to load here in case we just want to take a photo
								MsgBox.Show(this,"There are no sheets to send to the computer or device for the current patient.");
								return;
							}
						}
						else { //eclipboard only
							if(MobileAppDevices.PatientIsAlreadyUsingDevice(FormOpenDental.CurPatNum)) {
								MsgBox.Show(this,"The patient you have selected is already using another device. Select a patient who is not currently "+
									"using a device in order to continue.");
								return;
							}
							Appointment apptForToday=Appointments.GetAppointmentsForPat(FormOpenDental.CurPatNum).FirstOrDefault(x => x.AptDateTime.Date==DateTime.Today.Date);
							if(apptForToday==null) {
								MsgBox.Show(this,"The patient you have selected does not have an appointment today. Only patients with an "+
									"appointment on the same day can be sent to eClipboard.");
								return;
							}
							List<Sheet> listNonMobileSheets=listSheets.Items.GetAll<Sheet>().FindAll(x => !x.HasMobileLayout);
							if(listNonMobileSheets.Count>0) {
								if(!MsgBox.Show(MsgBoxButtons.YesNo,"The following sheets that have been queued for this patient cannot be " +
									$"loaded onto an eClipboard device because they do not have a mobile layout: \r\n" +
									$"{string.Join(", ",listNonMobileSheets.Select(x => x.Description))}. \r\nDo you still wish to continue?")) {
									return;
								}
							}
							//They are in setup mode (not normal workflow) and there are no sheets for this patient. They have not run the rules to generate
							//sheets as the patient has not been marked as arrived. When they push the patient to the device, they will not generate the sheets
							//from there either. Ask them if they want to generate the sheets in this case.
							if(_isSetupMode && listSheets.Items.Count==0 && ClinicPrefs.GetBool(PrefName.EClipboardCreateMissingFormsOnCheckIn,apptForToday.ClinicNum)) {
								bool generateSheets=MsgBox.Show(MsgBoxButtons.YesNo,"This patient has no forms to load. Would you like to generate the "+
									"forms based on the eClipboard rules?");
								if(generateSheets) {
									//We do not need to update the UI here. It will be updated at the end of this click method.
									Sheets.CreateSheetsForCheckIn(apptForToday);
								}
							}
						}
						device.SetPatNum(FormOpenDental.CurPatNum);
					}
					else { //we are trying to clear the patient
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A patient is currently using the terminal.  If you continue, they will lose the information that is on their "
							+"screen.  Continue anyway?")) {
							return;
						}
						device.SetPatNum(0);
						if(device.MobileDevice != null) { // device might have disconnected without the grid being updated yet
							TreatPlans.RemoveMobileAppDeviceNum(device.MobileDevice.MobileAppDeviceNum);
						}
					}
					FillGrid();
				}
				#endregion Load/Clear click handler
				GridCell cell=new GridCell(device.PatNum==0?"Load":"Clear");
				cell.ColorBackG=Color.LightGray;
				cell.ClickEvent=CellClick;
				row.Cells.Add(cell);
				if(_isSetupMode) {
					#region Delete click handler
					void DeleteClick(object sender,EventArgs e) {
						FillGrid();
						if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"A row should not be deleted unless it is showing erroneously and there really is "+
							"nothing running on the computer or device shown.  Continue anyway?")) {
							return;
						}
						device.Delete();
						FillGrid();
					}
					#endregion Delete click handler
					cell=new GridCell("Delete");
					cell.ColorBackG=Color.LightGray;
					cell.ClickEvent=DeleteClick;
					row.Cells.Add(cell);
				}
				gridMain.ListGridRows.Add(row);
				if(selected!=null && device.Matches(selected)) {
					selectedIndex=gridMain.ListGridRows.Count-1;
				}
			}
			gridMain.EndUpdate();
			gridMain.SetSelected(selectedIndex,true);//selectedIndex could be -1 if the selected term is not in the list, default to row 0
			FillPat();
		}

		private void FillPat() {
			bool isRowSelected=gridMain.GetSelectedIndex()>-1;
			groupBoxPatient.Enabled=isRowSelected;
			listSheets.Visible=isRowSelected;
			butPatForms.Visible=isRowSelected;
			listTreatPlans.Visible=isRowSelected;
			butRemoveTreatPlan.Visible=isRowSelected;
			if(!isRowSelected) {
				groupBoxPatient.Text="Select a Device First";
				labelPatient.Text="";
				labelSheets.Text="";
				return;
			}
			SheetDevice device=(SheetDevice)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			listSheets.Items.Clear();
			listTreatPlans.Items.Clear();
			butRemoveTreatPlan.Enabled=false;
			if(device.PatNum==0) {
				groupBoxPatient.Text="Patient to Load to Device";
				labelSheets.Text="Forms to Load to Device";
				if(FormOpenDental.CurPatNum==0) {
					labelPatient.Text="None Selected";
					butPatForms.Enabled=false;
				}
				else {
					labelPatient.Text=Patients.GetLim(FormOpenDental.CurPatNum).GetNameLF();
					butPatForms.Enabled=true;
					Sheets.GetForTerminal(FormOpenDental.CurPatNum).ForEach(x => listSheets.Items.Add(x.Description,x));
				}
			}
			else {
				groupBoxPatient.Text="Patient on Device";
				labelSheets.Text="Forms on Device";
				labelPatient.Text=device.PatName;
				butPatForms.Enabled=!device.IsKiosk;
				Sheets.GetForTerminal(device.PatNum).ForEach(x => listSheets.Items.Add(x.Description,x));
				TreatPlans.GetAllForPat(device.PatNum).ForEach(x => { if(x.MobileAppDeviceNum>0){ listTreatPlans.Items.Add(x.Heading,x); } } );
			}
		}

		#region EventHandlers

		public void PatientChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.Patient || e.Tag.GetType()!=typeof(long) || this.IsDisposed) {
				return;
			}
			FillPat();
		}

		public void eClipboardChangedEvent_Fired(ODEventArgs e) {
			if(e.EventType!=ODEventType.eClipboard || this.IsDisposed) {
				return;
			}
			FillGrid();
		}

		private void contrClinicPick_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_SelectionCommitted(object sender,EventArgs e) {
			FillPat();
		}

		private void butPatForms_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()<0) {
				return;
			}
			using FormPatientForms formPF=new FormPatientForms();
			SheetDevice device=(SheetDevice)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			formPF.PatNum=device.PatNum==0?FormOpenDental.CurPatNum:device.PatNum;
			formPF.ShowDialog();
			FillPat();
		}		
		
		private void butByod_Click(object sender,EventArgs e) {
			AppointmentL.SendByodLink(_appt);
		}

		private void butRemoveTreatPlan_Click(object sender,EventArgs e) {
			TreatPlan treatPlan=(TreatPlan)listTreatPlans.SelectedItem;
			SheetDevice device=(SheetDevice)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag;
			PushNotificationUtils.CI_RemoveTreatmentPlan(device.MobileDevice.MobileAppDeviceNum,treatPlan);
			FillPat();
		}

		private void listTreatPlans_SelectedIndexChanged(object sender,EventArgs e) {
			if(listTreatPlans.Items.Count==0) {
				butRemoveTreatPlan.Enabled=false;
				return;
			}
			if(((TreatPlan)listTreatPlans?.SelectedItem).MobileAppDeviceNum>0) {
				butRemoveTreatPlan.Enabled=true;
				return;
			}
			butRemoveTreatPlan.Enabled=false;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(Prefs.UpdateString(PrefName.TerminalClosePassword,textPassword.Text)){
				Signalods.SetInvalid(InvalidType.Prefs);
			}
			MsgBox.Show(this,"Done.");
		}

		private void butClose_Click(object sender,EventArgs e) {
			Close();
		}

		private void FormTerminalManager_FormClosing(object sender,FormClosingEventArgs e) {
			PatientChangedEvent.Fired-=PatientChangedEvent_Fired;
			EClipboardEvent.Fired-=eClipboardChangedEvent_Fired;
			if(Prefs.UpdateString(PrefName.TerminalClosePassword,textPassword.Text)){
				Signalods.SetInvalid(InvalidType.Prefs);
			}
		}

		#endregion EventHandlers

		///<summary>A wrapper class so that we can treat TerminalActives and MobileAppDevices interchangeably within this form. Contains a Kiosk
		///and a MobileDevice object, however only one of them can have a value for each instance of this class.</summary>
		private class SheetDevice {
			///<summary>The ComputerKiosk we are looking at. Must be null if MobileDevice is not null.</summary>
			private TerminalActive _computerKiosk;
			///<summary>The Mobile Device we are looking at. Must be null if ComputerKiosk is not null.</summary>
			private MobileAppDevice _mobileDevice;

			///<summary>The name the office uses to identify this device. The ComputerName for Kiosks and the DeviceName for MobileDevices.</summary>
			public string Name {
				get {
					return IsKiosk?_computerKiosk.ComputerName:_mobileDevice.DeviceName;
				}
			}

			///<summary>The SessionName for the Kiosk that we use to uniquely identify each kiosk. Not applicable for MobileDevices and returns 
			///an empty string when this instance represents a MobileDevice.</summary>
			public string SessionName {
				get {
					return IsKiosk?_computerKiosk.SessionName:"eClipboard";
				}
			}

			///<summary>The PatNum for the patient who is currently using this device or computer. 0 If none. -1 for MobileDevices only, which
			///indicates a patient is checking in but we don't know which patient it is yet.</summary>
			public long PatNum {
				get {
					return IsKiosk?_computerKiosk.PatNum:_mobileDevice.PatNum;
				}
			}


			///<summary>The name of the patient who is currently using this device or computer. Returns empty string if no patient, returns the
			///message "Check-In In-Progress" if someone is checking in but we don't know who yet.</summary>
			public string PatName {
				get {
					long patNum=IsKiosk?_computerKiosk.PatNum:_mobileDevice.PatNum;
					if(!IsKiosk && patNum==-1) {
						return "Check-In In-Progress";
					}
					return patNum>0 ? Patients.GetLim(patNum).GetNameLF() : "";
				}
			}

			public string ClinicDesc {
				get {
					return IsKiosk?"Unassigned":Clinics.GetDesc(_mobileDevice.ClinicNum);
				}
			}

			///<summary>Returns true if ComputerKiosk is not null and false if it is. This makes the assumption that only one or the other
			///of ComputerKiosk or MobileDevice can be initialized at a time (which shouldbe enforced by the structure of this class.</summary>
			public bool IsKiosk {
				get { return _computerKiosk!=null; }
			}

			///<summary>Returns true if _mobileDevice is not null and false if it is. This makes the assumption that only one or the other
			///of ComputerKiosk or MobileDevice can be initialized at a time (which shouldbe enforced by the structure of this class.</summary>
			public bool IsMobileAppDevice {
				get { return _mobileDevice!=null; }
			}

			///<summary>Returns _mobileDevice which can be null. Should only be called after checking IsMobileAppDevice when true.</summary>
			public MobileAppDevice MobileDevice{
				get{
					return _mobileDevice;
				}
			}

			public SheetDevice(TerminalActive kiosk) {
				_computerKiosk=kiosk;
			}

			public SheetDevice(MobileAppDevice mobileDevice) {
				_mobileDevice=mobileDevice;
			}

			///<summary>Just checks if the primary keys for this device and the passed in device matches</summary>
			public bool Matches(SheetDevice device) {
				if(IsKiosk && device.IsKiosk) {
					return this._computerKiosk.TerminalActiveNum==device._computerKiosk.TerminalActiveNum;
				}
				else if(!IsKiosk && !device.IsKiosk) {
					return this._mobileDevice.MobileAppDeviceNum==device._mobileDevice.MobileAppDeviceNum;
				}
				return false; //one is kiosk and the other isn't
			}

			///<summary>Delete the Kiosk or MobileDevice.</summary>
			public void Delete() {
				if(IsKiosk) { 
					TerminalActives.DeleteForCmptrSessionAndId(_computerKiosk.ComputerName,_computerKiosk.SessionId,processId:_computerKiosk.ProcessId);
				}
				else {
					MobileAppDevices.Delete(_mobileDevice.MobileAppDeviceNum);
				}
			}

			///<summary>Sets the PatNum for the selected Kiosk or MobileDevice.</summary>
			public void SetPatNum(long patNum) {
				if(IsKiosk) {
					TerminalActives.SetPatNum(_computerKiosk.TerminalActiveNum,patNum);
					Signalods.SetInvalid(InvalidType.Kiosk,KeyType.ProcessId,Process.GetCurrentProcess().Id);//signal the terminal manager to refresh its grid
				}
				else {
					MobileAppDevices.SetPatNum(_mobileDevice.MobileAppDeviceNum,patNum);
					if(patNum>0) {
						OpenDentBusiness.WebTypes.PushNotificationUtils.CI_CheckinPatient(patNum,_mobileDevice.MobileAppDeviceNum);
					}
					else {
						OpenDentBusiness.WebTypes.PushNotificationUtils.CI_GoToCheckin(_mobileDevice.MobileAppDeviceNum);
					}
				}
			}
		}
	}
}