using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEServicesMobileAppDeviceManage: FormODBase {
		private ClinicPrefHelper _clinicPrefHelper;
		///<summary>When true, shows the eClipboard column to enable and disable devices.</summary>
		private bool _canEditEClipboard;
		///<summary>When true, shows the ODTouch column to enable and disable devices.</summary>
		private bool _canEditODTouch;
		///<summary>List of the state of the old mobile app devices before entering this form</summary>
		private List<MobileAppDevice> _listMobileAppDevicesOld;
		///<summary>List of mobile app devices.</summary>
		private List<MobileAppDevice> _listMobileAppDevicesAll;
		///<summary>List of MobileAppDeviceNums that will be deleted on save.</summary>
		private List<MobileAppDevice> _listMobileAppDevicesDelete;

		public FormEServicesMobileAppDeviceManage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicPrefHelper=new ClinicPrefHelper(PrefName.ODTouchDeviceLimit);
			_listMobileAppDevicesDelete=new List<MobileAppDevice>();
		}

		private void FormEServicesMobileAppDeviceManage_Load(object sender,EventArgs e) {
			//Needs to be called before FillGrid to determine which controls can be enabled and disabled.
			_listMobileAppDevicesOld=MobileAppDevices.GetForUser(Security.CurUser);
			_listMobileAppDevicesAll=MobileAppDevices.GetForUser(Security.CurUser);
			SetUIEClipboardEnabled();
		}

		private void clinicPickerEClipboard_SelectionChangeCommitted(object sender,EventArgs e) {
			SetUIEClipboardEnabled();
		}

		///<summary>Fills the big main grid.</summary>
		private void FillGridMobileAppDevices() {
			gridMobileAppDevices.BeginUpdate();
			//Columns
			gridMobileAppDevices.Columns.Clear();
			GridColumn col=new GridColumn("Device Name",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last eClipboard Attempt",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last eClipboard Login",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last ODTouch Attempt",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last ODTouch Login",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn("Clinic",100){IsWidthDynamic=true };
				gridMobileAppDevices.Columns.Add(col);
			}
			col=new GridColumn("Device State",100) {IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("eClipboard",70,HorizontalAlignment.Center);
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("ODTouch",70,HorizontalAlignment.Center);
			gridMobileAppDevices.Columns.Add(col);
			if(_canEditEClipboard || _canEditODTouch) {
				col=new GridColumn("Delete",45,HorizontalAlignment.Center);
				gridMobileAppDevices.Columns.Add(col);
			}
			//Rows
			gridMobileAppDevices.ListGridRows.Clear();
			List<MobileAppDevice> listMobileAppDevices=_listMobileAppDevicesAll;
			if(GetClinicNumEClipboardTab()>0) {
				listMobileAppDevices=_listMobileAppDevicesAll.FindAll(x=>x.ClinicNum==GetClinicNumEClipboardTab());
			}
			for(int i=0;i<listMobileAppDevices.Count;i++) {
				GridRow row=new GridRow();
				MobileAppDevice mobileAppDevice=listMobileAppDevices[i];
				row.Cells.Add(mobileAppDevice.DeviceName+"\r\n("+mobileAppDevice.UniqueID+")");
				string rowValue="";
				if(mobileAppDevice.EclipboardLastAttempt.Year>1880) {
					rowValue=mobileAppDevice.EclipboardLastAttempt.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(mobileAppDevice.EclipboardLastLogin.Year>1880) {
					rowValue=mobileAppDevice.EclipboardLastLogin.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(mobileAppDevice.ODTouchLastAttempt.Year>1880) {
					rowValue=mobileAppDevice.ODTouchLastAttempt.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(mobileAppDevice.ODTouchLastLogin.Year>1880) {
					rowValue=mobileAppDevice.ODTouchLastLogin.ToString();
				}
				row.Cells.Add(rowValue);
				if(PrefC.HasClinicsEnabled) {
					if(mobileAppDevice.ClinicNum==0) {
						rowValue=Clinics.GetPracticeAsClinicZero().Abbr;
					}
					else {
						rowValue=Clinics.GetClinic(mobileAppDevice.ClinicNum).Abbr;
					}
					row.Cells.Add(rowValue);
				}
				row.Cells.Add(mobileAppDevice.DevicePage.GetDescription());
				row.Cells.Add((mobileAppDevice.IsEclipboardEnabled ? "X" : ""));
				row.Cells.Add((mobileAppDevice.IsODTouchEnabled ? "X" : ""));
				if(_canEditEClipboard || _canEditODTouch) {
					#region Delete click handler
					void DeleteClick(object sender,EventArgs e) {
						if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will remove the device from the database and all other workstations on Save." +
							" Continue?")) {
							return;
						}
						_listMobileAppDevicesDelete.Add(gridMobileAppDevices.SelectedTag<MobileAppDevice>());
						_listMobileAppDevicesAll.RemoveAll(x => x.MobileAppDeviceNum==gridMobileAppDevices.SelectedTag<MobileAppDevice>().MobileAppDeviceNum);
						FillGridMobileAppDevices();
					}
					#endregion Delete click handler
					GridCell gridCell=new GridCell("Delete");
					gridCell.ColorBackG=Color.LightGray;
					gridCell.ClickEvent=DeleteClick;
					row.Cells.Add(gridCell);
				}
				row.Tag=mobileAppDevice;
				gridMobileAppDevices.ListGridRows.Add(row);
			}
			gridMobileAppDevices.EndUpdate();
		}

		///<summary>Called when user clicks on use defaults for clinic, AuthorizeTab, clinicPicker.SelectedIndexChanged, and CheckEClipboardCreateMissingForms_Click.  It sets various areas enabled or disabled.  Doesn't change the checked values.</summary>
		private void SetUIEClipboardEnabled() {
			//Determine if the user has permissions to alter eService features.
			bool hasSetupPermission=Security.IsAuthorized(EnumPermType.EServicesSetup,true);
			long selectedClinicNum=GetClinicNumEClipboardTab();
			//Determine if the selected clinic is signed up for eClipboard.
			bool isClinicSignedUpEClipboard=MobileAppDevices.IsClinicSignedUpForEClipboard(selectedClinicNum);
			//Determine if the selected clinic is signed up for ODTouch.
			bool isClinicSignedUpODTouch=ClinicPrefs.IsODTouchAllowed(selectedClinicNum);
			if(PrefC.HasClinicsEnabled && selectedClinicNum==0) {
				isClinicSignedUpEClipboard=Clinics.GetForUserod(Security.CurUser).Any(x => MobileAppDevices.IsClinicSignedUpForEClipboard(x.ClinicNum));
				isClinicSignedUpODTouch=Clinics.GetForUserod(Security.CurUser).Any(x => ClinicPrefs.IsODTouchAllowed(x.ClinicNum));
			}
			_canEditEClipboard=hasSetupPermission && isClinicSignedUpEClipboard;
			_canEditODTouch=hasSetupPermission && isClinicSignedUpODTouch;
			gridMobileAppDevices.Enabled=hasSetupPermission;
			labelEClipboardNotSignedUp.Visible=!isClinicSignedUpEClipboard;
			FillGridMobileAppDevices();
		}

		///<summary>The current clinic num for this tab, handles whether or not the practice has clinics.</summary>
		private long GetClinicNumEClipboardTab() {
			if(!PrefC.HasClinicsEnabled) {
				return 0; //No clinics, HQ clinic
			}
			if(clinicPickerEClipboard==null) {
				return 0; //combobox hasn't loaded yet
			}
			return clinicPickerEClipboard.ClinicNumSelected;
		}

		private void gridMobileAppDevices_CellClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.EServicesSetup)) {
				return;
			}
			int idxeClipboardColumn=gridMobileAppDevices.Columns.GetIndex("eClipboard");
			int idxODTouchColumn=gridMobileAppDevices.Columns.GetIndex("ODTouch");
			if(!e.Col.In(idxeClipboardColumn,idxODTouchColumn)) {//They did not select the right column.
				return;
			}
			MobileAppDevice mobileAppDevice=gridMobileAppDevices.SelectedTag<MobileAppDevice>();
			//There is not a tag somehow.
			if(mobileAppDevice==null) {
				return;
			}
			long selectedClinicNum=0;
			if(PrefC.HasClinicsEnabled) {
				selectedClinicNum=mobileAppDevice.ClinicNum;
			}
			if(e.Col.In(idxeClipboardColumn) && !MobileAppDevices.IsClinicSignedUpForEClipboard(selectedClinicNum)) {
				MsgBox.Show("To manage devices go to the Signup Portal and enable eClipboard.");
				return;
			}
			if(e.Col.In(idxODTouchColumn) && !ClinicPrefs.IsODTouchAllowed(selectedClinicNum)){
				MsgBox.Show("To manage devices go to the Signup Portal and enable ODTouch.");
				return;
			}
			if(e.Col==idxeClipboardColumn) {
				if(mobileAppDevice.IsEclipboardEnabled){
					if(mobileAppDevice.PatNum>0) {
						MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
							" or wait until the patient is no longer using the device.");
						return;
					}
				}
				mobileAppDevice.IsEclipboardEnabled=!mobileAppDevice.IsEclipboardEnabled;//Flip the bit.
			}
			if(e.Col==idxODTouchColumn) {
				mobileAppDevice.IsODTouchEnabled=!mobileAppDevice.IsODTouchEnabled;//Flip the bit.
				long clinicNum=mobileAppDevice.ClinicNum;
				int deviceLimitForClinic=_clinicPrefHelper.GetIntVal(PrefName.ODTouchDeviceLimit,clinicNum);
				int deviceCountForClinic=_listMobileAppDevicesAll.Where(x=>x.ClinicNum==clinicNum && x.IsODTouchEnabled).Count();
				int previousDeviceCountForClinic=_listMobileAppDevicesOld.Where(x=>x.ClinicNum==clinicNum && x.IsODTouchEnabled).Count();
				//If the total enabled device count has changed for the clinic, and the user doesn't want to pay extra, return.
				if(deviceLimitForClinic<deviceCountForClinic && previousDeviceCountForClinic<deviceCountForClinic && mobileAppDevice.IsODTouchEnabled
					&& !MsgBox.Show(MsgBoxButtons.YesNo,$"The ODTouch device count of {deviceCountForClinic} exceeds the device limit of {deviceLimitForClinic}. This will incur a surplus charge per device over that limit. Continue?"))
				{
					mobileAppDevice.IsODTouchEnabled=false;
					return;//Don't activate device.
				}
				SecurityLogs.MakeLogEntry(EnumPermType.EServicesSetup,0,$"ODTouch {(mobileAppDevice.IsODTouchEnabled ? "enabled" : "disabled")} for device {mobileAppDevice.UniqueID}",mobileAppDevice.MobileAppDeviceNum,LogSources.None,DateTime.Now,Security.CurUser.UserNum);
			}
			//Update the device because the signal processing of this form isn't friendly to keeping an in-memory list that syncs when the form closes
			OpenDentBusiness.WebTypes.PushNotificationUtils.CI_IsAllowedChanged(mobileAppDevice.MobileAppDeviceNum,mobileAppDevice.IsEclipboardEnabled);
			FillGridMobileAppDevices();	//Fill the grid to show the changes.
		}

		private void butSave_Click(object sender,EventArgs e) {
			MobileAppDevices.DeleteMany(_listMobileAppDevicesDelete);
			_listMobileAppDevicesAll.ForEach(x => MobileAppDevices.Update(x));
			DialogResult=DialogResult.OK;
		}

	}
}