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
		/// <summary>When true, shows the eClipboard column to enable and disable devices.</summary>
		private bool _canEditEClipboard;
		/// <summary>When true, shows the ODTouch column to enable and disable devices.</summary>
		private bool _canEditODTouch;

		public FormEServicesMobileAppDeviceManage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEServicesMobileAppDeviceManage_Load(object sender,EventArgs e) {
			//Needs to be called before FillGrid to determine which controls can be enabled and disabled.
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
			col=new GridColumn("Last Attempt",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Last Login",100){IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn("Clinic",100){IsWidthDynamic=true };
				gridMobileAppDevices.Columns.Add(col);
			}
			col=new GridColumn("Device State",100) {IsWidthDynamic=true };
			gridMobileAppDevices.Columns.Add(col);
			col=new GridColumn("Enabled",60,HorizontalAlignment.Center);
			gridMobileAppDevices.Columns.Add(col);
			if(_canEditEClipboard || _canEditODTouch) {
				col=new GridColumn("Delete",45,HorizontalAlignment.Center);
				gridMobileAppDevices.Columns.Add(col);
			}
			//Rows
			gridMobileAppDevices.ListGridRows.Clear();
			List<MobileAppDevice> listMobileAppDevices=MobileAppDevices.GetForUser(Security.CurUser);
			if(GetClinicNumEClipboardTab()>0) {
				listMobileAppDevices.RemoveAll(x => x.ClinicNum!=GetClinicNumEClipboardTab());
			}
			for(int i=0;i<listMobileAppDevices.Count;i++) {
				GridRow row=new GridRow();
				MobileAppDevice mobileAppDevice=listMobileAppDevices[i];
				row.Cells.Add(mobileAppDevice.DeviceName+"\r\n("+mobileAppDevice.UniqueID+")");
				string rowValue="";
				if(mobileAppDevice.LastAttempt.Year>1880) {
					rowValue=mobileAppDevice.LastAttempt.ToString();
				}
				row.Cells.Add(rowValue);
				rowValue="";
				if(mobileAppDevice.LastLogin.Year>1880) {
					rowValue=mobileAppDevice.LastLogin.ToString();
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
				row.Cells.Add((mobileAppDevice.IsAllowed ? "X" : ""));
				if(_canEditEClipboard || _canEditODTouch) {
					#region Delete click handler
					void DeleteClick(object sender,EventArgs e) {
						if(gridMobileAppDevices.SelectedTag<MobileAppDevice>().PatNum>0) {
							MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
								" or wait until the patient is no longer using the device.");
							return;
						}
						if(!MsgBox.Show(MsgBoxButtons.YesNo,"This will immediately remove the device from the database and all other workstations." +
							" Continue?")) {
							return;
						}
						MobileAppDevices.Delete(gridMobileAppDevices.SelectedTag<MobileAppDevice>().MobileAppDeviceNum);
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
			bool hasSetupPermission=Security.IsAuthorized(Permissions.EServicesSetup,true);
			//Determine if the selected clinic is signed up for eClipboard.
			bool isClinicSignedUpEClipboard=MobileAppDevices.IsClinicSignedUpForEClipboard(GetClinicNumEClipboardTab());
			if(PrefC.HasClinicsEnabled && GetClinicNumEClipboardTab()==0) {
				isClinicSignedUpEClipboard=Clinics.GetForUserod(Security.CurUser).Any(x => MobileAppDevices.IsClinicSignedUpForEClipboard(x.ClinicNum));
			}
			//Determine if the selected clinic is signed up for ODTouch.
			bool isClinicSignedUpODTouch=LimitedBetaFeatures.IsAllowed(EServiceFeatureInfoEnum.ODTouch,clinicPickerEClipboard.SelectedClinicNum);
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
			return clinicPickerEClipboard.SelectedClinicNum;
		}

		private void gridMobileAppDevices_CellClick(object sender,ODGridClickEventArgs e) {
			if(!Security.IsAuthorized(Permissions.EServicesSetup)) {
				return;
			}
			int idxEnabledColumn=gridMobileAppDevices.Columns.GetIndex("Enabled");
			if(!e.Col.In(idxEnabledColumn)) {//They did not select the right column.
				return;
			}
			if(!_canEditEClipboard && !_canEditODTouch) {
				MsgBox.Show("To manage devices go to the Signup Portal and enable eClipboard.");
				return;
			}
			MobileAppDevice mobileAppDevice=gridMobileAppDevices.SelectedTag<MobileAppDevice>();
			//There is not a tag somehow.
			if(mobileAppDevice==null) {
				return;
			}
			if(e.Col==idxEnabledColumn) {
				if(mobileAppDevice.IsAllowed){
					if(mobileAppDevice.PatNum>0) {
						MsgBox.Show("A patient is currently using this device. Please clear the patient from the device using the Kiosk Manager" +
							" or wait until the patient is no longer using the device.");
						return;
					}
				}
				mobileAppDevice.IsAllowed=!mobileAppDevice.IsAllowed;//Flip the bit.
			}
			//Update the device because the signal processing of this form isn't friendly to keeping an in-memory list that syncs when the form closes
			MobileAppDevices.Update(mobileAppDevice);
			OpenDentBusiness.WebTypes.PushNotificationUtils.CI_IsAllowedChanged(mobileAppDevice.MobileAppDeviceNum,mobileAppDevice.IsAllowed);
			FillGridMobileAppDevices();	//Fill the grid to show the changes.
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}
	}
}