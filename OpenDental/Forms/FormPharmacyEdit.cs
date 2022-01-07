using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary></summary>
	public partial class FormPharmacyEdit:FormODBase {
		///<summary>Current pharmacy that is being edited.</summary>
		public Pharmacy PharmCur;

		///<summary></summary>
		public FormPharmacyEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormPharmacyEdit_Load(object sender, System.EventArgs e) {
			textStoreName.Text=PharmCur.StoreName;
			textPhone.Text=PharmCur.Phone;
			textFax.Text=PharmCur.Fax;
			textAddress.Text=PharmCur.Address;
			textAddress2.Text=PharmCur.Address2;
			textCity.Text=PharmCur.City;
			textState.Text=PharmCur.State;
			textZip.Text=PharmCur.Zip;
			textNote.Text=PharmCur.Note;
			//Selects all clinics that have a link to this pharmacy. This will only display and select the clinics the user has access to.
			List<PharmClinic> listPharmClinics=PharmClinics.GetPharmClinicsForPharmacy(PharmCur.PharmacyNum);
			comboClinic.ListSelectedClinicNums=listPharmClinics.Select(x => x.ClinicNum).ToList();
			//Save the currently selected clinics for synching down below.
			comboClinic.Tag=listPharmClinics.Where(x => comboClinic.ListSelectedClinicNums.Contains(x.ClinicNum)).ToList();
			//remember, this is NOT all PharmClinics for this Pharmacy, but only the ones that this user has permission to see
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(PharmCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Pharmacy?")) {
				return;
			}
			try{
				Pharmacies.DeleteObject(PharmCur.PharmacyNum);
				DialogResult=DialogResult.OK;
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
			}
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(textStoreName.Text==""){
				MessageBox.Show(Lan.g(this,"Store name cannot be blank."));
				return;
			}
			if(CultureInfo.CurrentCulture.Name=="en-US"){
				if(textPhone.Text!="" && TelephoneNumbers.FormatNumbersExactTen(textPhone.Text)==""){
					MessageBox.Show(Lan.g(this,"Phone number must be in a 10-digit format."));
					return;
				}
				if(textFax.Text!="" && TelephoneNumbers.FormatNumbersExactTen(textFax.Text)==""){
					MessageBox.Show(Lan.g(this,"Fax number must be in a 10-digit format."));
					return;
				}
			}
			PharmCur.StoreName=textStoreName.Text;
			PharmCur.PharmID="";
			PharmCur.Phone=textPhone.Text;
			PharmCur.Fax=textFax.Text;
			PharmCur.Address=textAddress.Text;
			PharmCur.Address2=textAddress2.Text;
			PharmCur.City=textCity.Text;
			PharmCur.State=textState.Text;
			PharmCur.Zip=textZip.Text;
			PharmCur.Note=textNote.Text;
			try{
				if(PharmCur.IsNew){
					Pharmacies.Insert(PharmCur);
				}
				else{
					Pharmacies.Update(PharmCur);
				}
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			//Update PharmClinic links
			List<PharmClinic> listPharmClinicOld=(List<PharmClinic>)comboClinic.Tag;
			List<PharmClinic> listPharmClinicNew=new List<PharmClinic>();
			//comboClinic.All might be selected, and would result in ListSelectedClinicNums containing only the clinics showing in the combobox, which very will not include clinics that user does not have permissions for.  "All" is not separately tested for.  Because the new list is synched against the old list, clinics that aren't showing are not affected one way or the other.
			foreach(long clinicNumNew in comboClinic.ListSelectedClinicNums) {
				if(listPharmClinicOld.Any(x => x.ClinicNum==clinicNumNew)) {//if it existed before, add it to the list
					listPharmClinicNew.Add(listPharmClinicOld.First(x => x.ClinicNum==clinicNumNew));
				}
				else {//otherwise, create a new link.
					listPharmClinicNew.Add(new PharmClinic(PharmCur.PharmacyNum,clinicNumNew));
				}
			}
			PharmClinics.Sync(listPharmClinicNew,listPharmClinicOld);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

		

		


	}
}





















