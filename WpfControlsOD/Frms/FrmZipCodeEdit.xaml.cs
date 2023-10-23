using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmZipCodeEdit:FrmODBase {

		///<summary></summary>
		public bool IsNew;
		public ZipCode ZipCodeCur;

		///<summary></summary>
		public FrmZipCodeEdit() {
			InitializeComponent();
			//Lan.F(this);
			KeyDown+=Frm_KeyDown;
			Load+=FrmZipCodeEdit_Load;
			textCity.TextChanged+=textCity_TextChanged;
			textState.TextChanged+=textState_TextChanged;
		}

		private void FrmZipCodeEdit_Load(object sender,EventArgs e) {
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
					this.Text=Lans.g(this,"Add Zip Code");
				}
				else {
					this.Text=Lans.g(this,"Edit Zip Code");
				}
			}
			textZip.Text=ZipCodeCur.ZipCodeDigits;
			textZip.SelectAll();
			textState.Text=ZipCodeCur.State;
			textState.SelectAll();
			textCity.Text=ZipCodeCur.City;
			textCity.SelectAll();
			checkIsFrequent.Checked=ZipCodeCur.IsFrequent;
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void textCity_TextChanged(object sender, System.EventArgs e) {
			if(textCity.Text.Length==1){
				textCity.Text=textCity.Text.ToUpper();
				textCity.SelectionStart=1;
			}
		}

		private void textState_TextChanged(object sender, System.EventArgs e){
			if(CultureInfo.CurrentCulture.Name=="en-US" //if USA or Canada, capitalize first 2 letters
				|| CultureInfo.CurrentCulture.Name.EndsWith("CA")) //Canadian. en-CA or fr-CA
			{ 
				if(textState.Text.Length==1 || textState.Text.Length==2){
					textState.Text=textState.Text.ToUpper();
					textState.SelectionStart=2;
				}
				return;
			}
			if(textState.Text.Length==1){
				textState.Text=textState.Text.ToUpper();
				textState.SelectionStart=1;
			}
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textZip.Text=="" || textCity.Text=="" || textState.Text==""){
				MsgBox.Show(this,"City,State, or Zip Cannot be left blank");
				return;
			}
			ZipCodeCur.City=textCity.Text;
			ZipCodeCur.State=textState.Text;
			ZipCodeCur.ZipCodeDigits=textZip.Text;
			ZipCodeCur.IsFrequent=(bool)checkIsFrequent.Checked;
			if(IsNew){
				ZipCodes.Insert(ZipCodeCur);
			}
			else{
				ZipCodes.Update(ZipCodeCur);
			}
			IsDialogOK=true;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			IsDialogOK=false;
		}

		

	}
}
