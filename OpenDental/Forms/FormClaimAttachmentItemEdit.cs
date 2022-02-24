using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	///<summary>This form takes the image the user wants to send through DentalXChange and gathers the additional information (specified by the user)
	///that is needed to create the attachment through ClaimConnect's API.</summary>
	public partial class FormClaimAttachmentItemEdit:FormODBase {
		public bool DoNewSnip=false;
		public ClaimConnect.ImageAttachment ImageAttachment;
		public bool IsSnip=false;
		private Image _claimImage;

		///<summary>Used for opening and editing an existing image attachment row in FormClaimAttachment.</summary>
		public FormClaimAttachmentItemEdit(Image image,string fileName,DateTime date,ClaimConnect.ImageTypeCode imageType,bool rightOrientation) : this(image){
			textFileName.Text=fileName;
			textDateCreated.Text=date.ToShortDateString();//Override today's date with the passed in date.
			listBoxImageType.SelectedIndex=(int)imageType;
			checkIsXrayMirrored.Checked=!rightOrientation;
		}

		///<summary>Takes an image the user has chosen to send with their claim.</summary>
		public FormClaimAttachmentItemEdit(Image image) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			listBoxImageType.Items.Clear();
			foreach(ClaimConnect.ImageTypeCode imageTypeCode in Enum.GetValues(typeof(ClaimConnect.ImageTypeCode))) {
				listBoxImageType.Items.Add(imageTypeCode.GetDescription(),imageTypeCode);
			}
			listBoxImageType.SelectedIndex=(int)ClaimConnect.ImageTypeCode.XRays;
			_claimImage=image;
			textDateCreated.Text=DateTime.Today.ToShortDateString();
			pictureBoxImagePreview.Image=(Image)image.Clone();
		}

		private void FormClaimAttachmentItemEdit_Load(object sender,EventArgs e) {
			if(!IsSnip) {
				butNewSnip.Visible=false;
				labelNewSnip.Visible=false;
			}
		}

		private void butNewSnip_Click(object sender,EventArgs e) {
			if(!ValidateAndCreateAttachment()) {
				return;
			}
			DoNewSnip=true;
			DialogResult=DialogResult.OK;
		}

		///<summary>Called on ValidateAndCreateAttachment(). This method takes the user entered data and creates the ClaimConnect.ImageAttachment object
		///used by FormClaimAttach. This object is eventually used in the ClaimConnect.CreateAttachment() API call to DentalXChange.</summary>
		private void CreateImageAttachment() {
			ImageAttachment=ClaimConnect.ImageAttachment.Create(
				fileName:textFileName.Text,
				createdDate:PIn.Date(textDateCreated.Text),
				typeCodeImage:listBoxImageType.GetSelected<ClaimConnect.ImageTypeCode>(),
				imageClaim:_claimImage,
				rightOrientation:!checkIsXrayMirrored.Checked);
		}

		private bool ValidateAndCreateAttachment() {
			if(string.IsNullOrWhiteSpace(textFileName.Text)) {
				MsgBox.Show(this,"Enter the filename for this attachment.");
				return false;
			}
			if(textFileName.Text.IndexOfAny(Path.GetInvalidFileNameChars())>=0) {//returns -1 if nothing found
				MsgBox.Show(this,"Invalid characters detected in the filename. Please remove them and try again.");
				return false;
			}
			if(!textDateCreated.IsValid()) {
				MsgBox.Show(this,"Enter a valid date.");
				return false;
			}
			CreateImageAttachment();
			return true;
		}

		///<summary></summary>
		private void buttonOK_Click(object sender,EventArgs e) {
			if(!ValidateAndCreateAttachment()) {
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void buttonCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}