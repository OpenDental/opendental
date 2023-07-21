using System;
using System.Collections.Generic;
using System.Data;
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
	public partial class FrmZipSelect:FrmODBase {
		private bool _isChanged;
		public ZipCode ZipCodeSelected;
		public string ZipCodeDigits;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FrmZipSelect() {
			InitializeComponent();
			KeyDown+=Frm_KeyDown;
			//Lan.F(this);
		}

		//This form is only accessed directly from the patient edit window, either by pushing the
		//button, or when user enters a zipcode that has more than one city available.
		
		private void FrmZipSelect_Loaded(object sender,RoutedEventArgs e) {
		  FillList();

		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butSave_Click(this,new EventArgs());
			}
		}
		
		private void FillList(){
			//refreshing is done within each routine
			listMatches.Items.Clear();
			string itemText="";
			_listZipCodes=ZipCodes.GetDeepCopy();
			if(!string.IsNullOrWhiteSpace(ZipCodeDigits)) {
				_listZipCodes.RemoveAll(x => x.ZipCodeDigits!=ZipCodeDigits);
			}
			for(int i=0;i<_listZipCodes.Count;i++){ 
				itemText=(_listZipCodes[i]).City+" "+(_listZipCodes[i]).State;
				if((_listZipCodes[i]).IsFrequent){
					itemText+=Lans.g(this," (freq)");
				}
				listMatches.Items.Add(itemText);
			}
			listMatches.SelectedIndex=-1;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
			frmZipCodeEdit.ZipCodeCur=new ZipCode();
			if(_listZipCodes.Count>0) {
				frmZipCodeEdit.ZipCodeCur.ZipCodeDigits=(_listZipCodes[0]).ZipCodeDigits;
			}
			frmZipCodeEdit.IsNew=true;
			frmZipCodeEdit.ShowDialog();
			if(!frmZipCodeEdit.IsDialogOK){
				return;
			}
			_isChanged=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(frmZipCodeEdit.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butEdit_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
			frmZipCodeEdit.ZipCodeCur=_listZipCodes[listMatches.SelectedIndex];
			frmZipCodeEdit.ShowDialog();
			if(!frmZipCodeEdit.IsDialogOK){
				return;
			}
			_isChanged=true;
			ZipCodes.RefreshCache();
			ZipCodes.GetALMatches(frmZipCodeEdit.ZipCodeCur.ZipCodeDigits);
			FillList();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item first.");
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
			IsDialogOK=true;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(listMatches.SelectedIndex==-1){
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ZipCodeSelected=_listZipCodes[listMatches.SelectedIndex];
			IsDialogOK=true;
		}

		private void FrmZipSelect_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}

		


	}
}





