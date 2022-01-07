using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormTaxAddress:FormODBase {
		#region Fields
		public Address AddressCur;
		public Patient PatCur;
		#endregion Fields

		#region Constructor
		public FormTaxAddress() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Events
		private void FormTaxAddress_Load(object sender,EventArgs e) {
			textAddress.Text=AddressCur.Address1;
			textAddress2.Text=AddressCur.Address2;
			textCity.Text=AddressCur.City;
			textState.Text=AddressCur.State;
			textZipCode.Text=AddressCur.Zip;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(AddressCur.IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this tax address?")) {
				return;
			}
			Addresses.Delete(AddressCur.AddressNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			string error="";
			if(textAddress.Text=="") {
				error+="Please provide an address\n";
			}
			if(textCity.Text=="") {
				error+="Please provide a city\n";
			}
			if(textState.Text=="") {
				error+="Please provide a state\n";
			}
			//Zipcode 
			//Regular Expression found at:
			//https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s14.html
			string regexp="^[0-9]{5}(?:-[0-9]{4})?$";
			if(!Regex.IsMatch(textZipCode.Text,regexp)) {
				error+="Please provide a valid zip code\n";
			}
			if(error!="") {
				MsgBox.Show(error);
				//DialogResult=DialogResult.None;
				return;
			}
			AddressCur.Address1=textAddress.Text;
			AddressCur.Address2=textAddress2.Text;
			AddressCur.City=textCity.Text;
			AddressCur.State=textState.Text;
			AddressCur.Zip=textZipCode.Text;
			if(AddressCur.IsNew) {
				Addresses.Insert(AddressCur);
			}
			else {
				Addresses.Update(AddressCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion Events



	}
}