using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
//using Microsoft.Win32;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>This form allows customers to customize the look of eClipboard for a Clinic. They may pick a name to displayed to patients, and an image to be displayed on the checkin screen.</summary>
	public partial class FormMobileBrandingProfileEdit:FormODBase {
		private MobileBrandingProfile _mobileBrandingProfile;
		public long ClinicNum;

		public FormMobileBrandingProfileEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		/// <summary>Fetches the mobile branding profile if it exists. Otherwise creates a new one, and fills fields.</summary>
		private void FormMobileBrandingProfileEdit_Load(object sender,EventArgs e) {
			_mobileBrandingProfile=MobileBrandingProfiles.GetByClinicNum(ClinicNum);
			if(_mobileBrandingProfile==null ) {
				_mobileBrandingProfile=new MobileBrandingProfile();
				_mobileBrandingProfile.IsNew=true;
			}
			FillFields();
		}

		/// <summary>Fills the fields of the form with values from existing mobileBrandingProfile, or defaults.</summary>
		private void FillFields() {
			if(_mobileBrandingProfile.IsNew) {
				Clinic clinic=Clinics.GetClinic(ClinicNum);
				if(clinic!=null) {
					textDescription.Text=clinic.Description;
				}
				return;
			}
			textDescription.Text=_mobileBrandingProfile.OfficeDescription;
			if(_mobileBrandingProfile.LogoFilePath.IsNullOrEmpty()) {
				return;
			}
			try {
				odPictureBoxPreview.Image=Image.FromFile(_mobileBrandingProfile.LogoFilePath); 
			}
			catch { 
				MsgBox.Show("Branding Image could not be loaded.");	
			}
			textFilePathImage.Text=_mobileBrandingProfile.LogoFilePath;
		}

		/// <summary>Attempts to display the image preview. Does nothing if fails. </summary>
		private void TrySetPreviewImage() {
			//Dispose existing image, before we lose our reference to it.
			if(odPictureBoxPreview.Image!=null) {
				odPictureBoxPreview.Image.Dispose();
				odPictureBoxPreview.Image=null;
			}
			try {
				odPictureBoxPreview.Image=Image.FromFile(textFilePathImage.Text);
			}
			//Do nothing if exception, wait until the user presses ok to inform them that the image is invalid.
			catch {}
		}

		///<summary>Returns false if description or logo are invalid, otherwise true.</summary>
		private bool ValidateMobileBrandingProfile() {
			if(textDescription.Text.IsNullOrEmpty() && textFilePathImage.Text.IsNullOrEmpty()) {
				MsgBox.Show("To clear Branding Profile, use clear button.");
				return false;
			}
			if(!textFilePathImage.Text.IsNullOrEmpty()){
				try {
					//fetch and dispose the file. Will throw if image is invalid.
					Image.FromFile(textFilePathImage.Text).Dispose();
				}
				catch (Exception e) {
					MsgBox.Show("Branding Image could not be loaded.\r\n"+e.Message);
					return false;
				}
			}
			if(textDescription.Text.IsNullOrEmpty() && !textFilePathImage.Text.IsNullOrEmpty()) {
				MsgBox.Show("Clinic Name is required.");
				return false;
			}
			return true;
		}

		/// <summary>Try to update the image preview.</summary>
		private void textFilePathImage_Leave(object sender,EventArgs e) {
			TrySetPreviewImage();
		}

		/// <summary>Opens file picker, and sets path.</summary>
		private void butSelectImage_Click(object sender,EventArgs e) {
			using OpenFileDialog openFileDialog =new OpenFileDialog();
			openFileDialog.Multiselect=false;
			DialogResult dialogResult= openFileDialog.ShowDialog();
			if(dialogResult!=DialogResult.OK) {
				return;
			}
			textFilePathImage.Text=openFileDialog.FileName;
			TrySetPreviewImage();
		}

		///<summary>Deletes branding profile if it exists in DB, and then clears the form.</summary>
		private void butClear_Click(object sender,EventArgs e) {
			if(_mobileBrandingProfile.MobileBrandingProfileNum!=0) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete Mobile Branding Profile?")) {
					return;
				}
				MobileBrandingProfiles.Delete(_mobileBrandingProfile.MobileBrandingProfileNum);
			}
			//Dispose image
			if(odPictureBoxPreview.Image!=null) {
				odPictureBoxPreview.Image.Dispose();
				odPictureBoxPreview.Image=null;
			}
			textDescription.Clear();
			textFilePathImage.Clear();
			_mobileBrandingProfile=new MobileBrandingProfile();
			_mobileBrandingProfile.IsNew=true;
		}

		/// <summary>Insert or Update</summary>
		private void butOK_Click(object sender,EventArgs e) {
			//If this is a new mobileBrandingProfile, and both fields are empty, close the form on ok click without saving.
			if(_mobileBrandingProfile.IsNew && textDescription.Text.IsNullOrEmpty() && textFilePathImage.Text.IsNullOrEmpty()) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!ValidateMobileBrandingProfile()) {
				return;
			}
			//Save values from form to MobileBrandingProfile
			_mobileBrandingProfile.ClinicNum=ClinicNum;
			_mobileBrandingProfile.LogoFilePath=textFilePathImage.Text;
			_mobileBrandingProfile.OfficeDescription=textDescription.Text;
			//If mobile branding profile is new, insert, otherwise update.
			if(_mobileBrandingProfile.IsNew) {
				MobileBrandingProfiles.Insert(_mobileBrandingProfile);
			}
			else {
				MobileBrandingProfiles.Update(_mobileBrandingProfile);
			}
			DialogResult=DialogResult.OK;
		}

		/// <summary>Closes the form.</summary>
		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
