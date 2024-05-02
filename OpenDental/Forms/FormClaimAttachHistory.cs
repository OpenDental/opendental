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
			if(textClaimStatus.Text.ToUpper().Contains("ATTACHMENT ID HAS BEEN ASSOCIATED TO A DIFFERENT CLAIM")
				|| textClaimStatus.Text.ToUpper().Contains("HAS ALREADY BEEN DELIVERED TO THE PAYER")) {
				//If users do not clear out their attachment a message will be in the validation textbox telling users they should do so
				if(MsgBox.Show(this,MsgBoxButtons.YesNo,"The attachment ID is associated to another claim. Any future attachment information will be sent to the wrong claim until you clear out the attachment. Would you like to do this now?"))
				{
					//Potential way to save users time by making a new attachment with the same information for the new claim
					//if(MsgBox.Show(this,MsgBoxButtons.YesNo,"Would you like to reuse the current attachment information to create a new attachment for the correct claim before this information is cleared out?")) {
						//not implemented
					//}
					ClearAttachmentID();
					ValidateClaimDXC();
				}
			}
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
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"This will clear out this claim's attachment information. You will have to recreate the entire attachment for this claim. Would you like to continue?"))
			{
				return;
			}
			ClearAttachmentID();
			FillGrid();
			textAttachmentID.Text=ClaimCur.AttachmentID;
			textNarrative.Text=ClaimCur.Narrative;
		}

			private void FormClaimAttachHistory_FormClosing(object sender,FormClosingEventArgs e) {
			if(ClaimCur.Narrative==textNarrative.Text) {
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