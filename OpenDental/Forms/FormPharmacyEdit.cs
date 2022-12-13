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
		public Pharmacy PharmacyCur;

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
			textStoreName.Text=PharmacyCur.StoreName;
			textPhone.Text=PharmacyCur.Phone;
			textFax.Text=PharmacyCur.Fax;
			textAddress.Text=PharmacyCur.Address;
			textAddress2.Text=PharmacyCur.Address2;
			textCity.Text=PharmacyCur.City;
			textState.Text=PharmacyCur.State;
			textZip.Text=PharmacyCur.Zip;
			textNote.Text=PharmacyCur.Note;
			//Selects all clinics that have a link to this pharmacy. This will only display and select the clinics the user has access to.
			List<PharmClinic> listPharmClinics=PharmClinics.GetPharmClinicsForPharmacy(PharmacyCur.PharmacyNum);
			comboClinic.ListSelectedClinicNums=listPharmClinics.Select(x => x.ClinicNum).ToList();
			//Save the currently selected clinics for synching down below.
			comboClinic.Tag=listPharmClinics.Where(x => comboClinic.ListSelectedClinicNums.Contains(x.ClinicNum)).ToList();
			//remember, this is NOT all PharmClinics for this Pharmacy, but only the ones that this user has permission to see
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(PharmacyCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete this Pharmacy?")) {
				return;
			}
			try{
				Pharmacies.DeleteObject(PharmacyCur.PharmacyNum);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
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
			PharmacyCur.StoreName=textStoreName.Text;
			PharmacyCur.PharmID="";
			PharmacyCur.Phone=textPhone.Text;
			PharmacyCur.Fax=textFax.Text;
			PharmacyCur.Address=textAddress.Text;
			PharmacyCur.Address2=textAddress2.Text;
			PharmacyCur.City=textCity.Text;
			PharmacyCur.State=textState.Text;
			PharmacyCur.Zip=textZip.Text;
			PharmacyCur.Note=textNote.Text;
			if(PharmacyCur.IsNew){
				Pharmacies.Insert(PharmacyCur);
			}
			else{
				Pharmacies.Update(PharmacyCur);
			}
			//Update PharmClinic links
			List<PharmClinic> listPharmClinicOld=(List<PharmClinic>)comboClinic.Tag;
			List<PharmClinic> listPharmClinicNew=new List<PharmClinic>();
			//comboClinic.All might be selected, and would result in ListSelectedClinicNums containing only the clinics showing in the combobox, which very will not include clinics that user does not have permissions for.  "All" is not separately tested for.  Because the new list is synched against the old list, clinics that aren't showing are not affected one way or the other.
			for(int i=0;i<comboClinic.ListSelectedClinicNums.Count;i++) {
				if(listPharmClinicOld.Any(x => x.ClinicNum==comboClinic.ListSelectedClinicNums[i])) {//if it existed before, add it to the list
					listPharmClinicNew.Add(listPharmClinicOld.First(x => x.ClinicNum==comboClinic.ListSelectedClinicNums[i]));
				}
				else {//otherwise, create a new link.
					listPharmClinicNew.Add(new PharmClinic(PharmacyCur.PharmacyNum,comboClinic.ListSelectedClinicNums[i]));
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





















