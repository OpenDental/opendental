using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental;
using OpenDentBusiness;
using CodeBase;
using ImagingDeviceManager;

namespace OpenDentalImaging {
	public partial class FormImagingDeviceEdit:FormODBase {
		public ImagingDevice ImagingDeviceCur;

		public FormImagingDeviceEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImagingDeviceEdit_Load(object sender, EventArgs e){
			textDescription.Text=ImagingDeviceCur.Description;
			textComputerName.Text=ImagingDeviceCur.ComputerName;
			listDeviceType.Items.AddEnums<EnumImgDeviceType>();
			listDeviceType.SetSelectedEnum(ImagingDeviceCur.DeviceType);
			comboTwainName.Text=ImagingDeviceCur.TwainName;
			checkShowTwainUI.Checked=ImagingDeviceCur.ShowTwainUI;
		}

		private void listDeviceType_SelectionChangeCommitted(object sender,EventArgs e) {
			if(listDeviceType.SelectedIndex==1){
				listDeviceType.SelectedIndex=0;
				MsgBox.Show("This is a placeholder for ongoing SDK enhancement efforts. For XDR, just use the TwainRadiograph device type.  It works well.");
			}
		}

		private void butThis_Click(object sender,EventArgs e) {
			textComputerName.Text=ODEnvironment.MachineName;
		}

		private void comboTwainName_DropDown(object sender,EventArgs e) {
			try {
				xImageDeviceManager.Obfuscator.ActivateEZTwain();
			}
			catch {
				Cursor=Cursors.Default;
				MsgBox.Show(this,"EzTwain4.dll not found.  Please run the setup file in your images folder.");
				return;
			}
			EZTwain.GetSourceList();
			comboTwainName.Items.Clear();
			while(true){
				StringBuilder stringBuilder=new StringBuilder();
				bool gotName=EZTwain.GetNextSourceName(stringBuilder);
				if(!gotName){
					break;
				}
				comboTwainName.Items.Add(stringBuilder.ToString());
			}
		}

		private void butTwain_Click(object sender,EventArgs e) {
			
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(ImagingDeviceCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			ImagingDevices.Delete(ImagingDeviceCur.ImagingDeviceNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			ImagingDeviceCur.Description=textDescription.Text;
			ImagingDeviceCur.ComputerName=textComputerName.Text;
			ImagingDeviceCur.DeviceType=listDeviceType.GetSelected<EnumImgDeviceType>();
			ImagingDeviceCur.TwainName=comboTwainName.Text;
			ImagingDeviceCur.ShowTwainUI=checkShowTwainUI.Checked;
			if(ImagingDeviceCur.IsNew){
				ImagingDevices.Insert(ImagingDeviceCur);
			}
			else{
				ImagingDevices.Update(ImagingDeviceCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}