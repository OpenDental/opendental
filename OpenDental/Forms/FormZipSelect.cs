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
		private bool changed;
		public ZipCode ZipSelected;
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
				itemText=(_listZipCodes[i]).City+" "
					+(_listZipCodes[i]).State;
				if((_listZipCodes[i]).IsFrequent){
					itemText+=Lan.g(this," (freq)");
				}
				listMatches.Items.Add(itemText);
			}
			listMatches.SelectedIndex=-1;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=new ZipCode();
			FormZCE.ZipCodeCur.ZipCodeDigits=(_listZipCodes[0]).ZipCodeDigits;
			FormZCE.IsNew=true;
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(FormZCE.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			using FormZipCodeEdit FormZCE=new FormZipCodeEdit();
			FormZCE.ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			FormZCE.ShowDialog();
			if(FormZCE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(FormZCE.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipCode ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			ZipCodes.Delete(ZipCodeCur);
			changed=true;
			ZipCodes.RefreshCache();
			FillList();
		}

		private void listMatches_DoubleClick(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				return;
			}
			ZipSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;		
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ZipSelected=_listZipCodes[listMatches.SelectedIndex];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormZipSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}

		


	}
}





