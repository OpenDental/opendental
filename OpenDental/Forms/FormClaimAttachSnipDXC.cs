using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OpenDental {
	///<summary></summary>
	public partial class FormClaimAttachSnipDXC:FormODBase {
		public Claim Claim;
		public Patient Patient;
		private ClaimConnect.ImageAttachment _claimConnectImageAttachment;
		private static readonly string _snipSketchURI="ms-screensketch";
		private Stopwatch _stopwatchKillSnipToolProcesses=new Stopwatch();
		private string _textClaimStatus;
		///<summary>Stores the original title of the form to change the title back after "Waiting For Snip" title is done</summary>
		private string _titleOriginal="Image Info";

		public FormClaimAttachSnipDXC() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			listBoxImageType.Items.Clear();
			listBoxImageType.Items.AddEnums<ClaimConnect.ImageTypeCode>();
			listBoxImageType.SelectedIndex=-1;//No default, force user to select a type
			textDateCreated.Text=DateTime.Today.ToShortDateString();
		}

		private void FormClaimAttachmentItemEdit_Load(object sender,EventArgs e) {
			//Jordan wants not visible out for now
			checkIsXrayMirrored.Visible=false;
			List<Def> listDefsClaimAttachments=GetImageCatDefs();
			if(listDefsClaimAttachments.Count>0) {//At least one Claim Attachment image definition exists.
				labelClaimAttachWarning.Visible=false;
			}
			//Non-DXC numbers (NEA) can be wiped out as they are no longer supported by DentalXChange starting 12/01/19.
			if(!string.IsNullOrWhiteSpace(Claim.AttachmentID) 
				&& !Claim.AttachmentID.ToLower().StartsWith("dxc") 
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"The claim has a non DentalXChange Attachment ID. Would you like to clear it out?")) 
			{
				ClearAttachmentID();
			}
			Plugins.HookAddCode(this,"FormClaimAttachment.Load_end",Patient,Claim,(Action<Bitmap>)ShowImageAttachmentItemEdit);
			if(ODEnvironment.IsCloudServer) {
				ODProgress.ShowAction(()=>StartSnipping(),"Opening snipping tool...");
			}
			else {
				StartSnipping();//this also minimizes
			}
		}

		private void FormClaimAttachmentItemEdit_Shown(object sender,EventArgs e) {
			//Check for if the attachmentID is already in use. If so inform the user they need to redo their attachments.
			ValidateClaimDXC();
			if(_textClaimStatus.ToUpper().Contains("ATTACHMENT ID HAS BEEN ASSOCIATED TO A DIFFERENT CLAIM")
				|| _textClaimStatus.ToUpper().Contains("HAS ALREADY BEEN DELIVERED TO THE PAYER"))
			{
				MessageBox.Show("The attachment ID is associated to another claim. Please redo your attachments.");
				ClearAttachmentID();
				if(!ValidateClaimDXC()){
					return;
				}
			}
			//BringToFront();//but it's already minimized, so why?
		}

		///<summary>Checks to see if the user has made a Claim Attachment image category definition. Returns the list of Defs found.</summary>
		private List<Def> GetImageCatDefs() {
			//Filter down to image categories that have been marked as Claim Attachment.
			return Defs.GetCatList((int)DefCat.ImageCats).ToList().FindAll(x => x.ItemValue.Contains("C") && !x.IsHidden);
		}

		///<summary>Returns true if claim is valid for DXC. Changes textClaimStatus.</summary>
		private bool ValidateClaimDXC() {
			ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
			//Usually super fast, but with a web call, they need a way to cancel if locked up.
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => validateClaimResponse=ClaimConnect.ValidateClaim(Claim,true);
			progressOD.StartingMessage="Communicating with DentalXChange...";
			try{
				progressOD.ShowDialog();
			}
			catch(ODException ex) {
				_textClaimStatus=ex.Message;
				MsgBox.Show(_textClaimStatus);
				return false;
			}
			catch(Exception ex) {
				_textClaimStatus=ex.Message;
				MsgBox.Show(_textClaimStatus);
				return false;
			}
			if(progressOD.IsCancelled){
				return false;
			}
			if(validateClaimResponse._isValidClaim) {
				_textClaimStatus="The claim is valid.";
				return true;
			}
			//Otherwise the claim must have errors, display them to the user.
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<validateClaimResponse.ValidationErrors.Length;i++) {
				stringBuilder.AppendLine(validateClaimResponse.ValidationErrors[i]);
			}
			_textClaimStatus=stringBuilder.ToString();
			MsgBox.Show(_textClaimStatus);
			return false;
		}

		private void EndSnipping() {
			timerMonitorClipboard.Stop();
			//Show the window in case it was minimized
			WindowState=FormWindowState.Normal;
			//Remove the "waiting for snip" text from the title
			Text=_titleOriginal;
			}

		private void timerMonitorClipboard_Tick(object sender,EventArgs e) {
			//every 250ms
			timerMonitorClipboard.Stop();
			bool hasRunningProcess;
			if(ODEnvironment.IsCloudServer) {
				hasRunningProcess=ODCloudClient.GetProcessesSnipTool();
			}
			else {
				List<Process> listProcesses=GetProcessesSnipTool();
				hasRunningProcess=listProcesses.Count>0;
			}
			if(!hasRunningProcess) {
				WindowState=FormWindowState.Normal;
				BringToFront();
				MsgBox.Show(this,"The snipping tool was closed while waiting for a snip. Stopping snip.");
				EndSnipping();
				return;
			}
			using Bitmap bitmapClipboard=ODClipboard.GetImage();
			if(bitmapClipboard==null) {
				timerMonitorClipboard.Start();
				return;
			}
			EndSnipping();
			BringToFront();
			//Start trying to kill Snip & Sketch and Snipping Tool
			_stopwatchKillSnipToolProcesses.Restart();
			timerKillSnipToolProcesses.Start();
			//Create the attachment but with default values
			try {
				PrepareImage(bitmapClipboard);
			}
			catch { 
			}
		}

		///<summary>100ms. Monitor the list of running processes for Snip & Sketch and Snipping Tool, for a short duration,
		///and kill any matching processes.  Doesn't stop trying until the duration is over. </summary>
		private void timerKillSnipToolProcesses_Tick(object sender,EventArgs e) {
			if(ODEnvironment.IsCloudServer) {
				if(ODCloudClient.GetProcessesSnipTool()) { 
					ODCloudClient.KillProcesses(); 
				}
			}
			else {
				List<Process> listProcesses=GetProcessesSnipTool();
				KillProcesses(listProcesses);
			}
			if(_stopwatchKillSnipToolProcesses.Elapsed>TimeSpan.FromSeconds(3)) {
				timerKillSnipToolProcesses.Stop();
				_stopwatchKillSnipToolProcesses.Reset();
			}
		}

		///<summary> Kill all passed-in processes, ignoring any failures, and return True if any were killed. </summary>
		private bool KillProcesses(List<Process> listProcessesToKills) {
			int countProcesseskilled=0;
			for(int i=listProcessesToKills.Count-1;i>=0;i--) {//backward
				try {
					listProcessesToKills[i].Kill();
				}
				catch { }
				countProcesseskilled+=1;
			}
			return countProcesseskilled>0;
		}

		/// <summary> Return any running Snipping Tool or Snip & Sketch processes </summary>
		private List<Process> GetProcessesSnipTool() {
			string snippingToolProcess="SnippingTool";
			string snipAndSketchProcess="ScreenSketch";
			Process[] processesArrayRunnings=Process.GetProcesses();
			List<Process> listProcesses=new List<Process>();
			for(int i=0;i<processesArrayRunnings.Length;i++) {
				//Skip any processes that aren't Snipping Tool or Snip & Sketch
				if(processesArrayRunnings[i].ProcessName!=snippingToolProcess && processesArrayRunnings[i].ProcessName!=snipAndSketchProcess) {
					continue;
				}
				listProcesses.Add(processesArrayRunnings[i]);
			}
			return listProcesses;
		}

		///<summary>Returns false if a protocol handler for ms-screensketch does not exist, meaning Snip & Sketch is not installed.</summary>
		public static bool DoesSnipAndSketchExist() {
			try {
				using Microsoft.Win32.RegistryKey registryKeyScreenSketchProtocol=Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(_snipSketchURI);
				return registryKeyScreenSketchProtocol!=null;
			}
			catch {
				return false;
			}
		}

		///<summary>Returns true if the Snipping Tool exe can be located.  False otherwise.</summary>
		public static bool DoesSnippingToolExist() {
			string windowsFolder=Environment.GetFolderPath(Environment.SpecialFolder.Windows);
			string snippingToolPath32=$"{windowsFolder}\\sysnative\\SnippingTool.exe";
			string snippingToolPath64=$"{windowsFolder}\\system32\\SnippingTool.exe";
			if(System.IO.File.Exists(snippingToolPath64) || System.IO.File.Exists(snippingToolPath32)) {
				return true;
			}
			return false;
		}

		///<summary>Attempts to start Snip & Sketch, then Snipping Tool if that fails. Returns true if either started, false if neither did.</summary>
		public static bool StartSnipAndSketchOrSnippingTool() {
			if(ODEnvironment.IsCloudServer) {
				return ODCloudClient.StartSnipAndSketchOrSnippingTool(_snipSketchURI);
			}
			//Determine if the screensketch protocol is in the registry; if not, we assume Snip & Sketch is not installed.
			if(DoesSnipAndSketchExist()) {
				Process processSnipAndSketch=new Process();
				processSnipAndSketch.StartInfo.UseShellExecute=true;
				processSnipAndSketch.StartInfo.FileName=$"{_snipSketchURI}:";
				try {
					processSnipAndSketch.Start();
					return true;
				}
				catch {

				}
			}
			//Couldn't start Snip & Sketch, so try Snipping Tool next.
			//A 64-bit process on a 64-bit OS uses `system32`.
			//A 32-bit process on a 64-bit OS uses `sysnative` to access the 64-bit system tools.
			//A 32-bit process on a 32-bit OS uses `system32` as there's no need for `sysnative`.
			if(DoesSnippingToolExist()) {
				Process processSnippingTool=new Process();
				string pathToSnippingTool;
				if(Environment.Is64BitProcess || !Environment.Is64BitOperatingSystem) {
					pathToSnippingTool="system32";
				} else {
					pathToSnippingTool="sysnative";
				}
				processSnippingTool.StartInfo.FileName=$"{Environment.GetFolderPath(Environment.SpecialFolder.Windows)}\\{pathToSnippingTool}\\SnippingTool.exe";
				try {
					processSnippingTool.Start();
				}
				catch {
					//Couldn't start either tool
					return false;
				}
				return true;
			}
			return false;
		}

		///<summary>Mimics FormImageSelectClaimAttachment.StartSnipping()</summary>
		private void StartSnipping() {
			if(!ODClipboard.Clear()) {
				MsgBox.Show(this,"Couldn't access clipboard, try again.");
				return;
			}
			//If we're in the middle of trying to kill Snip Tool processes, stop for now.
			timerKillSnipToolProcesses.Stop();
			_stopwatchKillSnipToolProcesses.Reset();
			if(ODEnvironment.IsCloudServer) {
				if(ODCloudClient.GetProcessesSnipTool()) {
					ODCloudClient.KillProcesses();
					Thread.Sleep(100);
				}
			}
			else {
				List<Process> listProcesses=GetProcessesSnipTool();
				if(KillProcesses(listProcesses)) {
					//Wait a short time before launching, since otherwise the Win32Exception "The remote procedure call failed and did not execute" can happen
					Thread.Sleep(100);
				}
			}
			if(!StartSnipAndSketchOrSnippingTool()) {
				MsgBox.Show(this,"Neither the Snip & Sketch tool nor the Snipping Tool could be launched.  Copy an image to the clipboard, then use the Paste Image button to add it as an attachment.  If you are on a Remote Desktop connection, launch your local system's Snip & Sketch or Snipping Tool, and make snips using either of those, which will be automatically copied to the clipboard.  If neither tool is available on your system, use the Print Screen keyboard key, or other screenshot software, to copy screenshots to the clipboard.");
				return;
			}
			Text=_titleOriginal+$" ({Lan.g(this,"Waiting For Snip")}...)";
			//Wait half a second before minimizing, otherwise Snip & Sketch can end up behind Open Dental
			Thread.Sleep(500);
			//begin monitoring the clipboard for results
			timerMonitorClipboard.Start();
		}

		///<summary>Caller should dispose of this bitmap.</summary>
		private void ShowImageAttachmentItemEdit(Bitmap bitmap) {
			PrepareImage(bitmap);
		}

		///<summary>Caller should dispose of this bitmap. Assigns image to pictureBoxImagePreview. Creates attachment to send to DXC from image.</summary>
		private void PrepareImage(Bitmap bitmap) {
			if(bitmap==null) {
				return;
			}
			ODImaging.ImageApplyOrientation(bitmap);
			pictureBoxImagePreview.Image=ODImaging.ImageScaleMaxHeightAndWidth(bitmap,pictureBoxImagePreview.Height,pictureBoxImagePreview.Width);
			pictureBoxImagePreview.Invalidate();
			//Create an ImageAttachment object to send to ClaimConnect.
			_claimConnectImageAttachment=ClaimConnect.ImageAttachment.Create(
				fileName:textFileName.Text,
				createdDate:PIn.Date(textDateCreated.Text),
				typeCodeImage:listBoxImageType.GetSelected<ClaimConnect.ImageTypeCode>(),
				imageClaim:bitmap,
				rightOrientation:!checkIsXrayMirrored.Checked);
			//In case this window was minimized
			WindowState=FormWindowState.Normal;
			BringToFront();
		}

		///<summary>Sends the passed-in attachments to ClaimConnect.  Will set the attachment id to the claim if needed.</summary>
		private void AddAttachments() {
			List<ClaimConnect.ImageAttachment> listClaimConnectImageAttachments=new List<ClaimConnect.ImageAttachment>();
			listClaimConnectImageAttachments.Add(_claimConnectImageAttachment);
			if(string.IsNullOrWhiteSpace(Claim.AttachmentID)) {
				//If an attachment has not already been created, create one.
				string attachmentId=ClaimConnect.CreateAttachment(listClaimConnectImageAttachments,narrative:"",Claim);
				//Update claim if attachmentID was set. Must happen here so that the validation will consider the new attachmentID.
				Claim.AttachmentID=attachmentId;
				//Set the claims attached flag to 'Misc' so that the attachmentID will write to the PWK segment 
				//when the claim is generated as an 837.
				Claim.AttachedFlags="Misc";
			}
			else {//An attachment already exists for this claim.
				ClaimConnect.AddAttachment(Claim, listClaimConnectImageAttachments);
			}
			Claims.Update(Claim);
		}

		///<summary>Wipes out the existing attachmentID, makes a securitylog for the old ID, and clears the 'Misc' attached flag on the claim.
		///This must be done when a non-DXC attachmentID has been detected so that claim validation will work as expected.</summary>
		private void ClearAttachmentID() {
			//Blindly set the claim's attached flags back to 'Mail' so that, deep down in the 837 text generation logic, the PWK segment will not be written
			//which will allow DentalXChange to validate the claim as if it is brand new with no attachments. See X837_5010.GenerateMessageText().
			Claim.AttachedFlags="Mail";
			string oldAttachmentID=Claim.AttachmentID;
			Claim.AttachmentID="";
			DateTime secDateTEdit=Claim.SecDateTEdit;//Preserve the date prior to any claim updates affecting it.
			Claims.Update(Claim);
			SecurityLogs.MakeLogEntry(EnumPermType.ClaimEdit,Claim.PatNum
				,$"Removed attachmentID {oldAttachmentID} for ClaimNum:{Claim.ClaimNum}",Claim.ClaimNum,secDateTEdit);
		}

		private bool SendAttachmentToDXCAndSaveLocally() {
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
			if(listBoxImageType.SelectedIndex==-1) {
				MsgBox.Show(this,"Select an image type.");
				return false;
			}
			//The user must create an image attachment before sending.
			if(_claimConnectImageAttachment==null) {
				MsgBox.Show(this,"An image must be specified before continuing.");
				return false;
			}
			_claimConnectImageAttachment.ImageFileNameDisplay=textFileName.Text;
			_claimConnectImageAttachment.ImageDate=PIn.Date(textDateCreated.Text);
			_claimConnectImageAttachment.ImageType=listBoxImageType.GetSelected<ClaimConnect.ImageTypeCode>();
			_claimConnectImageAttachment.ImageOrientationType="right";
			if(checkIsXrayMirrored.Checked) {
				_claimConnectImageAttachment.ImageOrientationType="left";
			}
			try {
				AddAttachments();
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
			if(PrefC.GetBool(PrefName.SaveDXCAttachments)) {
				Document documentCur=ImageStore.Import(_claimConnectImageAttachment.ImageFileAsBase64,defNumImageType,ImageType.Attachment,Patient);
				_claimConnectImageAttachment.ImageFileNameActual=documentCur.FileName;
			}
			//Create attachment objects
			List<ClaimAttach> listClaimAttaches=new List<ClaimAttach>();
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach.DisplayedFileName=_claimConnectImageAttachment.ImageFileNameDisplay;
			claimAttach.ActualFileName=_claimConnectImageAttachment.ImageFileNameActual;
			claimAttach.ClaimNum=Claim.ClaimNum;
			listClaimAttaches.Add(claimAttach);
			//Keep a running list of attachments sent to DXC for the claim. This will show in the attachments listbox.
			Claim.Attachments.AddRange(listClaimAttaches);
			Claims.Update(Claim);
			MsgBox.Show(this,"Attachments sent successfully!");
			return true;
		}

		private void butSendAndAgain_Click(object sender,EventArgs e) {
			bool attachmentSentAndSaved=SendAttachmentToDXCAndSaveLocally();
			if(!attachmentSentAndSaved) {
				return;
			}
			if(ODEnvironment.IsCloudServer) {
				ODProgress.ShowAction(()=>StartSnipping(),"Opening snipping tool...");
			}
			else {
				StartSnipping();
			}
			textFileName.Text="Attachment";
		}

		///<summary>Saves image to the patient on the claim's directory in the images module. Creates a ClaimAttach object to associate to the given claim.</summary>
		private void butSend_Click(object sender,EventArgs e) {
			bool attachmentSentAndSaved=SendAttachmentToDXCAndSaveLocally();
			if(attachmentSentAndSaved) {
				Close();
			}
		}

	}
}