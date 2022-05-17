using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormZipCodeEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		public ZipCode ZipCodeCur;

		///<summary></summary>
		public FormZipCodeEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormZipCodeEdit_Load(object sender, System.EventArgs e) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(IsNew) {
					this.Text="Add Postal Code";
				}
				else {
					this.Text="Edit Postal Code";
				}
				labelZipCode.Text="Postal Code";
				labelState.Text="Province";
			}
			else {
				if(IsNew) {
					this.Text=Lan.g(this,"Add Zip Code");
				}
				else {
					this.Text=Lan.g(this,"Edit Zip Code");
				}
			}
			textZip.Text=ZipCodeCur.ZipCodeDigits;
			textCity.Text=ZipCodeCur.City;
			textState.Text=ZipCodeCur.State;
			checkIsFrequent.Checked=ZipCodeCur.IsFrequent;
		}

		private void textCity_TextChanged(object sender, System.EventArgs e) {
			if(textCity.Text.Length==1){
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender, System.EventArgs e){
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				if(textState.Text.Length==1 || textState.Text.Length==2){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=2;
				}
			}
			else{
				if(textState.Text.Length==1){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=1;
				}
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textZip.Text=="" || textCity.Text=="" || textState.Text==""){
				MessageBox.Show(Lan.g(this,"City,State, or Zip Cannot be left blank"));
				return;
			}
      ZipCodeCur.City=textCity.Text;
			ZipCodeCur.State=textState.Text;
			ZipCodeCur.ZipCodeDigits=textZip.Text;
			ZipCodeCur.IsFrequent=checkIsFrequent.Checked;
			if(IsNew){
				ZipCodes.Insert(ZipCodeCur);
			}
			else{
				ZipCodes.Update(ZipCodeCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

	}
}
