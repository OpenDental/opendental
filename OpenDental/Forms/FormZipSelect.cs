using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormZipSelect : FormODBase {
		private bool _isChanged;
		public ZipCode ZipCodeSelected;
		private List<ZipCode> _listZipCodes;
		private string _zipCodeDigits;

		///<summary></summary>
		public FormZipSelect(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		///<summary>Pass in a 5 digit zipcode to filter down the loaded list of ZipCodes.</summary>
		public FormZipSelect(string zipCodeDigits): this(){
			_zipCodeDigits=zipCodeDigits;
		}

		//This form is only accessed directly from the patient edit window, either by pushing the
		//button, or when user enters a zipcode that has more than one city available.
		
		private void FormZipSelect_Load(object sender, System.EventArgs e) {
		  FillList();
		}
		
		private void FillList(){
			//refreshing is done within each routine
			listMatches.Items.Clear();
			string itemText="";
			_listZipCodes=ZipCodes.GetDeepCopy();
			if(!string.IsNullOrWhiteSpace(_zipCodeDigits)) {
				_listZipCodes.RemoveAll(x => x.ZipCodeDigits!=_zipCodeDigits);
			}
			for(int i=0;i<_listZipCodes.Count;i++){ 
				itemText=(_listZipCodes[i]).City+" "+(_listZipCodes[i]).State;
				if((_listZipCodes[i]).IsFrequent){
					itemText+=Lan.g(this," (freq)");
				}
				listMatches.Items.Add(itemText);
			}
			listMatches.SelectedIndex=-1;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
			formZipCodeEdit.ZipCodeCur=new ZipCode();
			if(_listZipCodes.Count>0) {
				formZipCodeEdit.ZipCodeCur.ZipCodeDigits=(_listZipCodes[0]).ZipCodeDigits;
			}
			formZipCodeEdit.IsNew=true;
			formZipCodeEdit.ShowDialog();
			if(formZipCodeEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_isChanged=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(formZipCodeEdit.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			using FormZipCodeEdit formZipCodeEdit=new FormZipCodeEdit();
			formZipCodeEdit.ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			formZipCodeEdit.ShowDialog();
			if(formZipCodeEdit.DialogResult!=DialogResult.OK){
				return;
			}
			_isChanged=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(formZipCodeEdit.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipCode zipCode=_listZipCodes[listMatches.SelectedIndex];
			ZipCodes.Delete(zipCode);
			_isChanged=true;
			ZipCodes.RefreshCache();
			FillList();
		}

		private void listMatches_DoubleClick(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				return;
			}
			ZipCodeSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;		
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipCodeSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormZipSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}

		


	}
}





