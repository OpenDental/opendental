using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmSiteEdit:FrmODBase {
		///<summary></summary>
		public Site SiteCur;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FrmSiteEdit()
		{
			InitializeComponent();
			Load+=FrmSiteEdit_Load;
			comboZip.SelectionChangeCommitted+=comboZip_SelectionChangeCommitted;
			textZip.TextChanged+=textZip_TextChanged;
			PreviewKeyDown+=FrmSiteEdit_PreviewKeyDown;
		}

		private void FrmSiteEdit_Load(object sender,EventArgs e) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				labelST.Text=Lang.g(this,"Province");
				labelZip.Text=Lang.g(this,"Postal Code");
				butEditZip.Text=Lang.g(this,"Edit Postal");
			}
			Lang.F(this);
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
			butSiteLink.IsEnabled=(!SiteCur.IsNew);
		}

		private void butPickProvider_Click(object sender,EventArgs e) {
			//for dental schools
			FrmProviderPick frmProviderPick=new FrmProviderPick(comboProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
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
			//The combo box is tucked under textZip.
			if(comboZip.SelectedIndex==-1){//Prevent UE from the TextChanged event
				return;
			}
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

		private void FrmSiteEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butEditZip.IsAltKey(Key.E,e)) {
				butEditZip_Click(this,new EventArgs());
			}
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butEditZip_Click(object sender,EventArgs e) {
			if(textZip.Text.Length==0) {
				if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
					MessageBox.Show(Lang.g(this,"Please enter a Postal Code first."));
				}
				else{
					MessageBox.Show(Lang.g(this,"Please enter a zipcode first."));
				}
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
				siteLink.ForeColor=System.Drawing.Color.Black;
				siteLink.InnerColor=System.Drawing.Color.LightCyan;
				siteLink.OuterColor=System.Drawing.Color.Blue;
			}
			FrmSiteLinkEdit frmSiteLinkEdit=new FrmSiteLinkEdit(siteLink);
			frmSiteLinkEdit.ShowDialog();
			if(frmSiteLinkEdit.IsDialogOK) {
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(SiteCur.IsNew){
				IsDialogOK=false;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Site?")) {
				return;
			}
			try{
				Sites.DeleteObject(SiteCur.SiteNum);
				IsDialogOK=true;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MessageBox.Show(Lang.g(this,"Description cannot be blank."));
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
			IsDialogOK=true;
		}

	}
}