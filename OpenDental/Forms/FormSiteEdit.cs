using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormSiteEdit:FormODBase {
		//<summary></summary>
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
			comboPlaceService.Items.Clear();
			foreach(string name in Enum.GetNames(typeof(PlaceOfService))) {
				comboPlaceService.Items.Add(Lan.g("enumPlaceOfService",name));
			}
			comboPlaceService.SelectedIndex=(int)SiteCur.PlaceService;
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
			using FormProviderPick FormPP=new FormProviderPick(comboProv.Items.GetAll<Provider>());
			FormPP.SelectedProvNum=comboProv.GetSelectedProvNum();
			FormPP.ShowDialog();
			if(FormPP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormPP.SelectedProvNum);
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
				ZipCode ZipCodeCur=new ZipCode();
				ZipCodeCur.ZipCodeDigits=textZip.Text;
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=ZipCodeCur;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				if(FormZE.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);//FormZipCodeEdit does not contain internal refresh
				FillComboZip();
				textCity.Text=ZipCodeCur.City;
				textState.Text=ZipCodeCur.State;
				textZip.Text=ZipCodeCur.ZipCodeDigits;
			}
			else if(listZipCodes.Count==1) {
				//only one match found.  Use it.
				textCity.Text=((ZipCode)listZipCodes[0]).City;
				textState.Text=((ZipCode)listZipCodes[0]).State;
			}
			else {
				//multiple matches found.  Pick one
				using FormZipSelect FormZS=new FormZipSelect();
				FormZS.ShowDialog();
				FillComboZip();
				if(FormZS.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
		}

		private void butEditZip_Click(object sender,EventArgs e) {
			if(textZip.Text.Length==0) {
				MessageBox.Show(Lan.g(this,"Please enter a zipcode first."));
				return;
			}
			List<ZipCode> listZipCodes=ZipCodes.GetALMatches(textZip.Text);
			if(listZipCodes.Count==0) {
				using FormZipCodeEdit FormZE=new FormZipCodeEdit();
				FormZE.ZipCodeCur=new ZipCode();
				FormZE.ZipCodeCur.ZipCodeDigits=textZip.Text;
				FormZE.IsNew=true;
				FormZE.ShowDialog();
				FillComboZip();
				if(FormZE.DialogResult!=DialogResult.OK) {
					return;
				}
				DataValid.SetInvalid(InvalidType.ZipCodes);
				textCity.Text=FormZE.ZipCodeCur.City;
				textState.Text=FormZE.ZipCodeCur.State;
				textZip.Text=FormZE.ZipCodeCur.ZipCodeDigits;
			}
			else {
				using FormZipSelect FormZS=new FormZipSelect();
				FormZS.ShowDialog();
				FillComboZip();
				if(FormZS.DialogResult!=DialogResult.OK) {
					return;
				}
				textCity.Text=FormZS.ZipSelected.City;
				textState.Text=FormZS.ZipSelected.State;
				textZip.Text=FormZS.ZipSelected.ZipCodeDigits;
			}
		}

		private void butSiteLink_Click(object sender,EventArgs e) {
			SiteLink siteLink=SiteLinks.GetFirstOrDefault(x => x.SiteNum==SiteCur.SiteNum);
			if(siteLink==null) {
				siteLink=new SiteLink() {
					SiteNum=SiteCur.SiteNum,
					ForeColor=Color.Black,
					InnerColor=Color.LightCyan,
					OuterColor=Color.Blue
				};
			}
			using FormSiteLinkEdit FormSE=new FormSiteLinkEdit(siteLink);
			FormSE.ShowDialog();
			if(FormSE.DialogResult==DialogResult.OK) {
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





















