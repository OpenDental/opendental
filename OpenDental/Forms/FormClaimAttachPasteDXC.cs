using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;
using CodeBase;
using OpenDentBusiness.Eclaims;
using System.Text;

namespace OpenDental {
	///<summary></summary>
	public partial class FormClaimAttachPasteDXC:FormODBase {
		public Claim ClaimCur;
		public Patient PatientCur;
		///<summary>Stores list of attachment information for DXC</summary>
		private List<AttachmentItem> _listAttachmentItems=new List<AttachmentItem>();
		///<summary>Stores a list of the image Ids from DXC to be saved locally.</summary>
		private List<int> _listImageReferenceIds;

		public FormClaimAttachPasteDXC() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimAttachPasteDXC_Load(object sender,EventArgs e) {
			List<Def> listDefsClaimAttachments=GetImageCatDefs();
			if(listDefsClaimAttachments.Count>0) {//At least one Claim Attachment image definition exists.
				labelClaimAttachWarning.Visible=false;
			}
			if(!GetImagesFromClipboard()) {//If getting images from clipboard fails, then kick out
				Close();
				return;
			}
			FillGrid();
			ValidateClaimDXC();
			if(_listAttachmentItems.Count<1) {
				return;
			}
			//Load first image into pictureBox upon opening
			Bitmap bitmap=_listAttachmentItems[0].Bitmap;
			if(bitmap==null) {
				return;
			}
			pictureBox.Image=bitmap;
			try {
				gridMain.SetSelected(0);
			}
			catch {
				return;
			}
			textNarrative.Text=ClaimCur.Narrative;
		}

		private void FillGrid() {
			if(_listAttachmentItems.Count<1) {//See if list is now empty after deleting item
				MsgBox.Show("All images have been deleted.");
				Close();
				return;
			}
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn("Image Name",150);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Date",75);
			gridMain.Columns.Add(gridColumn);
			gridColumn=new GridColumn("Image Type",150);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<_listAttachmentItems.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(_listAttachmentItems[i].ImageAttachment.ImageFileNameDisplay);
				gridRow.Cells.Add(_listAttachmentItems[i].ImageAttachment.ImageDate.ToShortDateString());
				if(_listAttachmentItems[i].HasTypeBeenSet) {//Leave blank if user has not set type yet
					gridRow.Cells.Add(_listAttachmentItems[i].ImageAttachment.ImageType.ToString());
				}
				else{
					gridRow.Cells.Add("");
				}
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private bool GetImagesFromClipboard() {
			//Get either a single image or a list of filepaths
			Bitmap bitmap=ODClipboard.GetImage();//Null if no bitmap on clipboard
			List<string> listFilePaths=ODClipboard.GetFileDropList()?.ToList();//Null if no files on clipboard
			if(listFilePaths==null && bitmap==null) {
				MsgBox.Show("There are no Images saved to your clipboard");
				return false;
			}
			if(bitmap!=null) {
				AttachmentItem attachmentItem=new AttachmentItem();
				//ImageTypeCode is set to ReferralForm initially for the Create method. AttachmentItem.HasBeenSet keeps track if user has set an ImageTypeCode yet.
				ClaimConnect.ImageAttachment imageAttachment=ClaimConnect.ImageAttachment.Create("Attachment",DateTime.Today,ClaimConnect.ImageTypeCode.ReferralForm,bitmap);
				attachmentItem.ImageAttachment=imageAttachment;
				attachmentItem.Bitmap=bitmap;
				_listAttachmentItems.Add(attachmentItem);
				return true;
			}
			for(int i=0;i<listFilePaths.Count;i++) {
				//Make sure files exist
				if(!File.Exists(listFilePaths[i])) {
					MsgBox.Show(this,"A file on the clipboard could not be located. It may have been moved, deleted or renamed.");
					return false;
				}
				Bitmap bitmapFromFile;
				try {
					bitmapFromFile=new Bitmap(listFilePaths[i]);
				}
				catch(ArgumentException) {
					MsgBox.Show(this,"One or more invalid file types were pasted. Valid file types include BMP, GIF, JPEG, PNG, and TIFF.");
					return false;
				}
				//We need to release the file lock.
				Bitmap bitmapCopy=new Bitmap(bitmapFromFile);
				bitmapFromFile?.Dispose();
				string imageName;
				try {
					imageName=Path.GetFileNameWithoutExtension(listFilePaths[i]);
				}
				catch {
					MsgBox.Show(this,"There was an issue getting the name of a file on the clipboard.");
					return false;
				}
				if(imageName.IsNullOrEmpty()) {
					imageName="Image";
				}
				AttachmentItem attachmentItem=new AttachmentItem();
				//ImageTypeCode is set to Referral form initially for this method. AttachmentItem.HasBeenSet keeps track if user has set an ImageTypeCode yet.
				ClaimConnect.ImageAttachment imageAttachment=ClaimConnect.ImageAttachment.Create(imageName,DateTime.Today,ClaimConnect.ImageTypeCode.ReferralForm,bitmapCopy);
				attachmentItem.ImageAttachment=imageAttachment;
				attachmentItem.Bitmap=bitmapCopy;
				_listAttachmentItems.Add(attachmentItem);
			}
			return true;
		}

		private List<Def> GetImageCatDefs() {
			//Filter down to image categories that have been marked as Claim Attachment.
			return Defs.GetCatList((int)DefCat.ImageCats).ToList().FindAll(x => x.ItemValue.Contains("C") && !x.IsHidden);
		}

		private bool ValidateClaimDXC() {
			ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
			//Usually super fast, but with a web call, they need a way to cancel if locked up.
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => validateClaimResponse=ClaimConnect.ValidateClaim(ClaimCur,true);
			progressOD.StartingMessage="Communicating with DentalXChange...";
			try{
				progressOD.ShowDialog();
			}
			catch(ODException ex) {
				textClaimStatus.Text=ex.Message;
				return false;
			}
			catch(Exception ex) {
				textClaimStatus.Text=ex.Message;
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			if(validateClaimResponse._isValidClaim) {
				textClaimStatus.Text="The claim is valid.";
				return true;
			}
			//Otherwise the claim must have errors, display them to the user.
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<validateClaimResponse.ValidationErrors.Length;i++) {
				stringBuilder.AppendLine(validateClaimResponse.ValidationErrors[i]);
			}
			textClaimStatus.Text=stringBuilder.ToString();
			return false;
		}

		///<summary>Sends the passed-in attachments to ClaimConnect.  Will set the attachment id to the claim if needed.</summary>
		private void AddAttachments(List<ClaimConnect.ImageAttachment> listImageAttachments) {
			if(string.IsNullOrWhiteSpace(ClaimCur.AttachmentID)) {
				//If an attachment has not already been created, create one.
				string attachmentId=ClaimConnect.OpenAttachment(ClaimCur,textNarrative.Text);
				//Update claim if attachmentID was set. Must happen here so that the validation will consider the new attachmentID.
				ClaimCur.AttachmentID=attachmentId;
				_listImageReferenceIds=ClaimConnect.AddAttachmentImage(ClaimCur,listImageAttachments);
				ClaimConnect.SubmitAttachment(ClaimCur);
				//Set the claims attached flag to 'Misc' so that the attachmentID will write to the PWK segment 
				//when the claim is generated as an 837.
				ClaimCur.AttachedFlags="Misc";
			}
			else {//An attachment already exists for this claim.
				_listImageReferenceIds=ClaimConnect.AddAttachmentImage(ClaimCur,listImageAttachments);
				if(ClaimCur.Narrative!=textNarrative.Text) {
					ClaimConnect.AddNarrative(ClaimCur,textNarrative.Text);
				}
			}
			ClaimCur.Narrative=textNarrative.Text;
			Claims.Update(ClaimCur);
		}

		///<summary>Mimics AddAttachments() but is split out for simplicity as this method will be rarely called. Sends every attachment one-by-one to DentalXChange. Sets the claims attachmentID to the response from Dentalxchange for the first attachment sent.</summary>
		private void BatchAddAttachments(List<ClaimConnect.ImageAttachment> listImageAttachments) {
			for(int i=0;i<listImageAttachments.Count;i++) {
				List<ClaimConnect.ImageAttachment> listImageAttachmentsOnlyOne=new List<ClaimConnect.ImageAttachment>();
				listImageAttachmentsOnlyOne.Add(listImageAttachments[i]);
				AddAttachments(listImageAttachmentsOnlyOne);//Send list of 1 attachment to DXC
			}
		}

		///<summary>Sends attachments to DXC and saves locally. Mostly copied from FormClaimAttachment.</summary>
		private bool SendAttachmentsToDXCAndSaveLocally() {
			//Additional validation occurs in FormClaimAttachPasteDXCItem as well
			if(_listAttachmentItems.Any(x => x.HasTypeBeenSet==false)) {
				MsgBox.Show(this,"The image type for one or more attachments has not been set");
				return false;
			}
			//The user must create an image attachment before sending.
			if(_listAttachmentItems.Any(x => x.ImageAttachment.ImageFileAsBase64==null)
				|| _listAttachmentItems.Any(x => x.Bitmap==null)) {
				MsgBox.Show(this,"An image for one or more attachments has not been set");
				return false;
			}
			//Create a list of ClaimConnectImageAttachments from _listAttachmentItems to send to DXC
			List<ClaimConnect.ImageAttachment> listImageAttachments=new List<ClaimConnect.ImageAttachment>();
			for(int i=0;i<_listAttachmentItems.Count;i++) {
				listImageAttachments.Add(_listAttachmentItems[i].ImageAttachment);
			}
			try {
				AddAttachments(listImageAttachments);//Send to DXC
			}
			//Creating and sending DXC Attachments will sometimes time out when an arbitrarily large group of attachments are being sent, 
			//at which point each attachment should be sent individually.
			catch(TimeoutException ex) {
				ProgressWin progressWin=new ProgressWin();
				progressWin.ActionMain=() =>BatchAddAttachments(listImageAttachments);
				progressWin.StartingMessage="Sending attachments timed out. Attempting to send individually. Please wait.";
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){
					return false;
				}
			}
			catch(ODException ex) {
				//ODExceptions should already be Lans.g when throwing meaningful messages.
				//If they weren't translated, the message was from a third party and shouldn't be translated anyway.
				MessageBox.Show(ex.Message);
				return false;
			}
			catch(Exception ex) {
				//a catch-all for any exceptions that could be thrown from this method that aren't from a timeout and aren't handled as ODExceptions
				FriendlyException.Show("An error has occurred while trying to add attachments. If the problem persists please contact your clearinghouse's support.",ex);
				return false;
			}
			//Validate the claim, if it isn't valid let the user decide if they want to continue
			if(!ValidateClaimDXC()) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"There were errors validating the claim, would you like to continue?")) {
					return false;
				}
			}
			//Used for determining which category to save the image attachments to. 0 will save the image to the first category in the Images module.
			long defNumImageType=0;
			Def defClaimAttachCat=GetImageCatDefs().FirstOrDefault();
			if(defClaimAttachCat==null) {//User does not have a Claim Attachment image category, just use the first image category available.
				defNumImageType=Defs.GetCatList((int)DefCat.ImageCats).FirstOrDefault(x => !x.IsHidden).DefNum;
			}
			else {
				defNumImageType=defClaimAttachCat.DefNum;
			}
			List<ClaimAttach> listClaimAttaches=new List<ClaimAttach>();
			for(int i=0;i<listImageAttachments.Count;i++) {
				if(PrefC.GetBool(PrefName.SaveDXCAttachments)) {
					//Save pasted images to AtoZ folder
					Document documentCur=ImageStore.Import(listImageAttachments[i].ImageFileAsBase64,defNumImageType,ImageType.Attachment,PatientCur);
					listImageAttachments[i].ImageFileNameActual=documentCur.FileName;
					//Set description of newly created document
					Document documentOld=documentCur.Copy();
					documentCur.Description=listImageAttachments[i].ImageFileNameDisplay;
					Documents.Update(documentCur,documentOld);
				}
				//Create attachment objects
				ClaimAttach claimAttach=new ClaimAttach();
				claimAttach.DisplayedFileName=listImageAttachments[i].ImageFileNameDisplay;
				claimAttach.ActualFileName=listImageAttachments[i].ImageFileNameActual;
				claimAttach.ClaimNum=ClaimCur.ClaimNum;
				claimAttach.ImageReferenceId=_listImageReferenceIds[i];
				listClaimAttaches.Add(claimAttach);
			}
			//Keep a running list of attachments sent to DXC for the claim. This will show in the attachments listbox.
			ClaimCur.Attachments.AddRange(listClaimAttaches);
			Claims.Update(ClaimCur);
			MsgBox.Show(this,"Attachment sent successfully!");
			return true;
		}

		private void textNarrative_TextChanged(object sender,EventArgs e) {
			labelCharCount.Text=textNarrative.Text.Length+"/2000";//2000 char limit set by DXC
		}

		///<summary>Open FormClaimAttachPasteDXCItem for the specific row selected to edit the information for said row.</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormClaimAttachPasteDXCItem formClaimAttachPasteDXCItem=new FormClaimAttachPasteDXCItem();
			formClaimAttachPasteDXCItem.AttachmentItemCur=_listAttachmentItems[e.Row];
			formClaimAttachPasteDXCItem.ShowDialog();
			if(formClaimAttachPasteDXCItem.AttachmentItemCur.ImageAttachment==null) {//Attachment has been deleted
				_listAttachmentItems[e.Row].Bitmap?.Dispose();
				_listAttachmentItems.RemoveAt(e.Row);
				pictureBox.Image?.Dispose();
				pictureBox.Image=null;
			}
			else if(formClaimAttachPasteDXCItem.DialogResult==DialogResult.OK) {//Attachment has been saved
				_listAttachmentItems[e.Row].ImageAttachment=formClaimAttachPasteDXCItem.AttachmentItemCur.ImageAttachment;
				_listAttachmentItems[e.Row].HasTypeBeenSet=formClaimAttachPasteDXCItem.AttachmentItemCur.HasTypeBeenSet;
			}
			FillGrid();
		}

		private void FormClaimAttachPasteDXC_FormClosing(object sender,FormClosingEventArgs e) {
			for(int i=0;i<_listAttachmentItems.Count;i++) {
				_listAttachmentItems[i].Bitmap?.Dispose();
			}
		}

		/// <summary>Displays image for the selected row in the pictureBox.</summary>
		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			Bitmap bitmap=_listAttachmentItems[e.Row].Bitmap;
			if(bitmap==null) {
				MsgBox.Show("Cannot load the image for this file");
				return;
			}
			pictureBox.Image=bitmap;
		}
		
		///<summary>Set the pictureBox.Image as soon as a right click occurs. Very similar to gridMain_CellClick().</summary>
		private void contextMenuImageGrid_Popup(object sender,EventArgs e) {
			int idxAttachSelected=gridMain.GetSelectedIndex();
			if(idxAttachSelected==-1) {
				return;
			}
			Bitmap bitmap=_listAttachmentItems[idxAttachSelected].Bitmap;
			if(bitmap==null) {
				MsgBox.Show("Cannot load the image for this file");
				return;
			}
			pictureBox.Image=bitmap;
		}

		///<summary>Centralizes setting an imageTypeCode from the right click menu.</summary>
		private void SetImageTypeCode(ClaimConnect.ImageTypeCode imageTypeCode) {
			int idxAttachSelected=gridMain.GetSelectedIndex();
			if(idxAttachSelected==-1) {
				return;
			}
			_listAttachmentItems[idxAttachSelected].HasTypeBeenSet=true;
			_listAttachmentItems[idxAttachSelected].ImageAttachment.ImageType=imageTypeCode;
			FillGrid();//Refill grid to show updated imageTypeCode
		}

		private void menuItemReferralForm_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.ReferralForm);
		}

		private void menuItemDiagnosticReport_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.DiagnosticReport);
		}

		private void menuItemExplanationOfBenefits_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.ExplanationOfBenefits);
		}

		private void menuItemOtherAttachments_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.OtherAttachments);
		}

		private void menuItemPeriodontalCharts_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.PeriodontalCharts);
		}

		private void menuItemXRays_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.XRays);
		}

		private void menuItemDentalModels_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.DentalModels);
		}

		private void menuItemRadiologyReports_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.RadiologyReports);
		}

		private void menuItemIntraOralPhotograph_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.IntraOralPhotograph);
		}

		private void menuItemNarrative_Click(object sender,EventArgs e) {
			SetImageTypeCode(ClaimConnect.ImageTypeCode.Narrative);
		}

		private void buttonPasteAgain_Click(object sender,EventArgs e) {
			if(!GetImagesFromClipboard()) {
				return;
			}
			FillGrid();
		}
		
		private void butSend_Click(object sender,EventArgs e) {
			bool attachmentSentAndSaved=SendAttachmentsToDXCAndSaveLocally();
			if(attachmentSentAndSaved) {
				Close();
			}
		}

		/// <summary>Class to help store additional data for the attachment. Gets sent to FormClaimAttachPasteDXCItem.</summary>
		public class AttachmentItem {
			///<summary>The object that is sent to DXC.</summary>
			public ClaimConnect.ImageAttachment ImageAttachment;
			///<summary>Checks to see if user has set an ImageTypeCode. Used by FormClaimAttachPasteDXCItem.</summary>
			public bool HasTypeBeenSet=false;
			///<summary>Keeps track of bitmap for this row to give to pictureBox. This AttachmentItem is responsible for disposing of the bitmap when form closes.</summary>
			public Bitmap Bitmap;
		}


	}
}