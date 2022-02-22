using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEquipmentEdit:FormODBase {
		public Equipment EquipmentCur;
		public bool IsNew;

		public FormEquipmentEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEquipmentEdit_Load(object sender,EventArgs e) {
			textDateEntry.Text=EquipmentCur.DateEntry.ToShortDateString();
			textDescription.Text=EquipmentCur.Description;
			textSerialNumber.Text=EquipmentCur.SerialNumber;
			textModelYear.Text=EquipmentCur.ModelYear;
			if(EquipmentCur.DatePurchased.Year>1880) {
				textDatePurchased.Text=EquipmentCur.DatePurchased.ToShortDateString();
			}
			if(EquipmentCur.DateSold.Year>1880) {
				textDateSold.Text=EquipmentCur.DateSold.ToShortDateString();
			}
			if(EquipmentCur.PurchaseCost>0) {
				textPurchaseCost.Text=EquipmentCur.PurchaseCost.ToString("f");
			}
			if(EquipmentCur.MarketValue>0) {
				textMarketValue.Text=EquipmentCur.MarketValue.ToString("f");
			}
			textLocation.Text=EquipmentCur.Location;
			textStatus.Text=EquipmentCur.Status;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew){
				DialogResult=DialogResult.Cancel;
			}
			if(!Security.IsAuthorized(Permissions.EquipmentDelete,EquipmentCur.DateEntry)) {
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			try{
				Equipments.Delete(EquipmentCur);
			}
			catch(ApplicationException ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butGenerate_Click(object sender,EventArgs e) {
			EquipmentCur.SerialNumber=Equipments.GenerateSerialNum();
			textSerialNumber.Text=EquipmentCur.SerialNumber;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textDatePurchased.IsValid()
				|| !textDateSold.IsValid()
				|| !textPurchaseCost.IsValid()
				|| !textMarketValue.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description.");
				return;
			}
			if(textDatePurchased.Text=="") {
				MsgBox.Show(this,"Please enter date purchased.");
				return;
			}
			if(PIn.Date(textDatePurchased.Text) > DateTime.Today) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Date is in the future.  Continue anyway?")) {
					return;
				}
			}
			EquipmentCur.Description=textDescription.Text;
			EquipmentCur.SerialNumber=textSerialNumber.Text;
			EquipmentCur.ModelYear=textModelYear.Text;
			EquipmentCur.DatePurchased=PIn.Date(textDatePurchased.Text);
			EquipmentCur.DateSold=PIn.Date(textDateSold.Text);
			EquipmentCur.PurchaseCost=PIn.Double(textPurchaseCost.Text);
			EquipmentCur.MarketValue=PIn.Double(textMarketValue.Text);
			EquipmentCur.Location=textLocation.Text;
			EquipmentCur.Status=textStatus.Text;
			if(!string.IsNullOrEmpty(textSerialNumber.Text) && Equipments.HasExisting(EquipmentCur)) {
				MsgBox.Show(this,"Serial number already in use.  Please enter another.");
				return;
			}
			if(IsNew) {
				Equipments.Insert(EquipmentCur);
			}
			else {
				Equipments.Update(EquipmentCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

      

		
	}
}