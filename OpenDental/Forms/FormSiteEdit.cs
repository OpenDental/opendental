using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary></summary>

	public partial class FormSiteEdit:FormODBase {
		///<summary></summary>
		//public bool IsNew;
		public Site SiteCur;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FormSiteEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSiteEdit_Load(object sender,EventArgs e) {
			textDescription.Text=SiteCur.Description;
			comboPlaceService.Items.AddEnums<PlaceOfService>();
			comboPlaceService.SetSelectedEnum(SiteCur.PlaceService);
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy(true));
			comboProv.SetSelectedProvNum(SiteCur.ProvNum);
			textAddress.Text=SiteCur.Address;
			textAddress2.Text=SiteCur.Address2;
			textCity.Text=SiteCur.City;
			textState.Text=SiteCur.State;
			FillComboZip();
			textZip.Text=SiteCur.Zip;
			textNote.Text=SiteCur.Note;
			butSiteLink.Visible=PrefC.IsODHQ;
			butSiteLink.Enabled=(!SiteCur.IsNew);
		}

		private void butPickProvider_Click(object sender,EventArgs e) {
			//for dental schools
			using FormProviderPick formProviderPick=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			formProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			formProviderPick.ShowDialog();
			if(formProviderPick.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(formProviderPick.ProvNumSelected);
		}

		private void FillComboZip() {
			comboZip.Items.Clear();
			_listZipCodes=ZipCodes.GetDeepCopy(true);
			for(int i=0;i<_listZipCodes.Count;i++) {
				comboZip.Items.Add((_listZipCodes[i]).ZipCodeDigits
					+" ("+(_listZipCodes[i]).City+")");
			}
		}

		private void textZip_TextChanged(object sender,EventArgs e) {
			comboZip.SelectedIndex=-1;
		}

		private void comboZip_SelectionChangeCommitted(object sender,EventArgs e) {
			//this happens when a zipcode is selected from the combobox of frequent zips.
			//The combo box is tucked under textZip because Microsoft makes stupid controls.
			textCity.Text=(_listZipCodes[comboZip.SelectedIndex]).City;
			textState.Text=(_listZipCodes[comboZip.SelectedIndex]).State;
			textZip.Text=(_listZipCodes[comboZip.SelectedIndex]).ZipCodeDigits;
		}

		private void textZip_Validating(object sender,CancelEventArgs e) {
			//fired as soon as control loses focus.
			//it's here to validate if zip is typed in to text box instead of picked from list.
			if(textZip.Text.Length<5) {
				return;
			}
			if(comboZip.SelectedIndex!=-1) {
				return;
			}
			//the autofill only works if both city and state are left blank
			if(textCity.Text!="" || textState.Text!="") {
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0) {
				//No match found. Must enter info for new zipcode
				ZipCode zipCode=new ZipCode();
				zipCode.ZipCodeDigits=textZip.Text;
				FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
				frmZipCodeEdit.ZipCodeCur=zipCode;
				frmZipCodeEdit.IsNew=true;
				frmZipCodeEdit.ShowDialog();
				if(!frmZipCodeEdit.IsDialogOK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				FillComboZip();
				textCity.Text=zipCode.City;
				textState.Text=zipCode.State;
				textZip.Text=zipCode.ZipCodeDigits;
				return;
			}
			if(listZipCodes.Count==1) {
				//only one match found.  Use it.
				textCity.Text=((ZipCode)listZipCodes[0]).City;
				textState.Text=((ZipCode)listZipCodes[0]).State;
				return;
			}
			//multiple matches found.  Pick one
			FrmZipSelect frmZipSelect=new FrmZipSelect();
			frmZipSelect.ShowDialog();
			FillComboZip();
			if(!frmZipSelect.IsDialogOK) {
				return;
			}
			DataValid.SetInvalid(InvalidType.ZipCodes);
			textCity.Text=frmZipSelect.ZipCodeSelected.City;
			textState.Text=frmZipSelect.ZipCodeSelected.State;
			textZip.Text=frmZipSelect.ZipCodeSelected.ZipCodeDigits;
		}

		private void butEditZip_Click(object sender,EventArgs e) {
			if(textZip.Text.Length==0) {
				MessageBox.Show(Lan.g(this,"Please enter a zipcode first."));
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0) {
				FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
				frmZipCodeEdit.ZipCodeCur=new ZipCode();
				frmZipCodeEdit.ZipCodeCur.ZipCodeDigits=textZip.Text;
				frmZipCodeEdit.IsNew=true;
				frmZipCodeEdit.ShowDialog();
				FillComboZip();
				if(!frmZipCodeEdit.IsDialogOK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=frmZipCodeEdit.ZipCodeCur.City;
				textState.Text=frmZipCodeEdit.ZipCodeCur.State;
				textZip.Text=frmZipCodeEdit.ZipCodeCur.ZipCodeDigits;
				return;
			}
			FrmZipSelect frmZipSelect=new FrmZipSelect();
			frmZipSelect.ShowDialog();
			FillComboZip();
			if(!frmZipSelect.IsDialogOK) {
				return;
			}
			textCity.Text=frmZipSelect.ZipCodeSelected.City;
			textState.Text=frmZipSelect.ZipCodeSelected.State;
			textZip.Text=frmZipSelect.ZipCodeSelected.ZipCodeDigits;
		}

		private void butSiteLink_Click(object sender,EventArgs e) {
			SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==SiteCur.SiteNum);
			if(siteLink==null) {
				siteLink=new SiteLink();
				siteLink.SiteNum=SiteCur.SiteNum;
				siteLink.ForeColor=Color.Black;
				siteLink.InnerColor=Color.LightCyan;
				siteLink.OuterColor=Color.Blue;
			}
			using FormSiteLinkEdit formSiteLinkEdit=new FormSiteLinkEdit(siteLink);
			formSiteLinkEdit.ShowDialog();
			if(formSiteLinkEdit.DialogResult==DialogResult.OK) {
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SiteCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Site?")) {
				return;
			}
			try{
				Sites.DeleteObject(SiteCur.SiteNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MessageBox.Show(Lan.g(this,"Description cannot be blank."));
				return;
			}
			SiteCur.Description=textDescription.Text;
			SiteCur.PlaceService=(PlaceOfService)comboPlaceService.SelectedIndex;
			SiteCur.ProvNum=comboProv.GetSelectedProvNum();
			SiteCur.Address=textAddress.Text;
			SiteCur.Address2=textAddress2.Text;
			SiteCur.City=textCity.Text;
			SiteCur.State=textState.Text;
			SiteCur.Zip=textZip.Text;
			SiteCur.Note=textNote.Text;
			try{
				if(SiteCur.IsNew) {
					Sites.Insert(SiteCur);
				}
				else {
					Sites.Update(SiteCur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}





















