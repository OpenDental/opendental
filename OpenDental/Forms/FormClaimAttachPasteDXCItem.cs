using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	public partial class FormClaimAttachPasteDXCItem:FormODBase {
		public FormClaimAttachPasteDXC.AttachmentItem AttachmentItemCur;

		public FormClaimAttachPasteDXCItem() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimAttachPasteDXCItem_Load(object sender, EventArgs e){
			textFileName.Text=AttachmentItemCur.ImageAttachment.ImageFileNameDisplay;
			textDateCreated.Text=AttachmentItemCur.ImageAttachment.ImageDate.ToShortDateString();
			listBoxImageType.Items.Clear();
			listBoxImageType.Items.AddEnums<ClaimConnect.ImageTypeCode>();
			listBoxImageType.SetSelected(-1);
			if(AttachmentItemCur.HasTypeBeenSet) {//See if user has set an ImageType yet
				listBoxImageType.SetSelectedEnum(AttachmentItemCur.ImageAttachment.ImageType);
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			AttachmentItemCur.ImageAttachment=null;
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(string.IsNullOrWhiteSpace(textFileName.Text)) {
				MsgBox.Show(this,"Enter the filename for this attachment.");
				return;
			}
			if(textFileName.Text.IndexOfAny(Path.GetInvalidFileNameChars())>=0) {//returns -1 if nothing found
				MsgBox.Show(this,"Invalid characters detected in the filename. Please remove them and try again.");
				return;
			}
			if(!textDateCreated.IsValid()) {
				MsgBox.Show(this,"Enter a valid date.");
				return;
			}
			if(listBoxImageType.SelectedIndex==-1) {
				MsgBox.Show(this,"Select an image type.");
				return;
			}
			AttachmentItemCur.ImageAttachment.ImageFileNameDisplay=textFileName.Text;
			AttachmentItemCur.ImageAttachment.ImageDate=PIn.Date(textDateCreated.Text);
			AttachmentItemCur.ImageAttachment.ImageType=listBoxImageType.GetSelected<ClaimConnect.ImageTypeCode>();
			AttachmentItemCur.HasTypeBeenSet=true;//The user will have picked a type at least once by now
			DialogResult=DialogResult.OK;
		}

	}
}