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
using OpenDental.Thinfinity;
using System.Diagnostics;

namespace OpenDental {
	///<summary></summary>
	public partial class FormClaimAttachHistory:FormODBase {
		public Claim ClaimCur;
		public Patient PatientCur;

		public FormClaimAttachHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimAttachHistory_Load(object sender,EventArgs e) {
			ValidateClaimDXC();
			textNarrative.Text=ClaimCur.Narrative;
			if(ClaimCur.AttachmentID=="") {
				textNarrative.ReadOnly=true;
				textNarrative.BackColor=System.Drawing.SystemColors.Control;
			}
			textAttachmentID.Text=ClaimCur.AttachmentID;
			FillGrid();
		}

		///<summary>Wipes out the existing attachmentID,narrative, and list of ClaimAttaches. Makes a securitylog for the old ID, and clears the 'Misc' attached flag on the claim. This must be done when a non-DXC attachmentID has been detected so that claim validation will work as expected.</summary>
		private void ClearAttachmentID() {
			//Blindly set the claim's attached flags back to 'Mail' so that, deep down in the 837 text generation logic, the PWK segment will not be written
			//which will allow DentalXChange to validate the claim as if it is brand new with no attachments. See X837_5010.GenerateMessageText().
			ClaimCur.AttachedFlags="Mail";
			string oldAttachmentID=ClaimCur.AttachmentID;
			ClaimCur.AttachmentID="";
			ClaimCur.Narrative="";//Clear narrative for new attachment
			ClaimCur.Attachments=new List<ClaimAttach>();//Empty ClaimAttach list
			DateTime secDateTEdit=ClaimCur.SecDateTEdit;//Preserve the date prior to any claim updates affecting it.
			Claims.Update(ClaimCur);
			SecurityLogs.MakeLogEntry(EnumPermType.ClaimEdit,ClaimCur.PatNum
				,$"Removed attachmentID {oldAttachmentID} for ClaimNum:{ClaimCur.ClaimNum}",ClaimCur.ClaimNum,secDateTEdit);
		}

		///<summary>Returns true if claim is valid for DXC. Changes _textClaimStatus.</summary>
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

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn("File",350);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<ClaimCur.Attachments.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(ClaimCur.Attachments[i].DisplayedFileName);
				gridRow.Tag=ClaimCur.Attachments[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		private void textNarrative_TextChanged(object sender,EventArgs e) {
			labelCharCount.Text=textNarrative.Text.Length+"/2000";//2000 char limit set by DXC
		}

		///<summary>The selected image opens in default photo viewer. This code was copied from FormClaimEdit gridSent_CellDoubleClick().</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!PrefC.GetBool(PrefName.SaveDXCAttachments)) {
				MsgBox.Show(this,$"Not allowed to view attachment. Attachments can only be viewed when the 'Save Attachments to Imaging Module' preference is set.");
				return;
			}
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach=(ClaimAttach)gridMain.ListGridRows[e.Row].Tag;
			string patFolder=ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath());
			if(CloudStorage.IsCloudStorage) {
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName,'/');
				if(!CloudStorage.FileExists(pathAndFileName)) {
					//Couldn't find file, display message and return
					MsgBox.Show(this,"File no longer exists.");
					return;
				}
				//found it, download and display
				//This chunk of code was pulled from FormFilePicker.cs
				UI.ProgressWin progressWin=new UI.ProgressWin();
				progressWin.StartingMessage="Downloading...";
				byte[] byteArray=null;
				progressWin.ActionMain=() => {
					byteArray=CloudStorage.Download(patFolder,claimAttach.ActualFileName);
				};
				progressWin.ShowDialog();
				if(progressWin.IsCancelled){//user clicked cancel
					return;
				}
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(pathAndFileName));
				File.WriteAllBytes(tempFile,byteArray);
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					Process.Start(tempFile);
				}
			}
			else {//Local storage
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName);
				if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(pathAndFileName);
				}
				else {
					try {
						Process.Start(pathAndFileName);
					}
					catch(Exception ex) {
						ex.DoNothing();
						MsgBox.Show(this,"Could not open the attachment.");
					}
				}
			}
		}

		private void butClear_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"This will clear out this claim's attachment information. You will have to recreate the entire attachment for this claim. Would you like to continue?"))
			{
				return;
			}
			ClearAttachmentID();
			FillGrid();
			textAttachmentID.Text=ClaimCur.AttachmentID;
			textNarrative.Text=ClaimCur.Narrative;
		}

		private void buttonDeleteImageAttachments_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				MsgBox.Show("Select attachment image(s) to delete.");
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete the selected image(s)?")) {
				return;
			}
			List<ClaimAttach> listClaimAttaches=new List<ClaimAttach>();
			int counterOldClaimAttach=0;//Track the number of Claim Attaches with a bad ImageReferenceId
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				if(((ClaimAttach)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag).ImageReferenceId==0) {
					counterOldClaimAttach++;
					continue;//Don't allow ClaimAttach with a bad ImageReferenceId as this will cause the DXC call to fail
				}
				listClaimAttaches.Add((ClaimAttach)gridMain.ListGridRows[gridMain.SelectedIndices[i]].Tag);
			}
			if(listClaimAttaches.Count==0) {//All selected attachment images have a bad ImageReferenceId
				MsgBox.Show("None of the selected attachment image(s) are able to be deleted. Any images that were sent before the delete image feature was added cannot be deleted.");
				return;
			}
			if(counterOldClaimAttach!=0) {//One or more selected attachment images have a bad ImageReferenceId
				if(!MsgBox.Show(MsgBoxButtons.OKCancel,counterOldClaimAttach+" attachment image(s) cannot be deleted because they were sent before the delete image feature was added. Would you like to delete the others?")) {
					return;
				}
			}
			try {
				ClaimConnect.DeleteImages(ClaimCur,listClaimAttaches);
			}
			catch {
				MsgBox.Show("Unable to delete the selected attachment image(s).");
				return;
			}
			//Delete local claimattaches to match what was removed from DXC
			for(int i=0;i<listClaimAttaches.Count;i++) {
				ClaimCur.Attachments.Remove(listClaimAttaches[i]);
			}
			Claims.Update(ClaimCur);
			FillGrid();
		}


		private void FormClaimAttachHistory_FormClosing(object sender,FormClosingEventArgs e) {
		if(ClaimCur.Narrative==textNarrative.Text) {//Limited to 2000 char in UI
			return;
		}
		try {
			ClaimConnect.AddNarrative(ClaimCur,textNarrative.Text);//revises existing narrative
		}
		catch(Exception ex) {
			return;//form will close. Fails silently.
		}
		ClaimCur.Narrative=textNarrative.Text;
		Claims.Update(ClaimCur);
		}

	}
}