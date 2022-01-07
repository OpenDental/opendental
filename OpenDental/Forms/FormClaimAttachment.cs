using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Eclaims;

namespace OpenDental {
	///<summary></summary>
	public partial class FormClaimAttachment:FormODBase {
		private Claim _claimCur;
		private Patient _claimPat;
		private static bool _isAlreadyOpen=false;
		private static readonly string _snipSketchURI="ms-screensketch";
		///<summary> Keeps track of how long we've been trying to kill all running Snip Tool processes </summary>
		private Stopwatch _stopwatchKillSnipToolProcesses=new Stopwatch();
		private string _titleOriginal;
		private List<ClaimConnect.ImageAttachment> _listImageAttachments;

		///<summary>Initialize the form and refresh the claim we are adding attachments to.</summary>
		private FormClaimAttachment(Claim claim) {
			InitializeComponent();
			InitializeLayoutManager();
			_claimCur=claim;
			_listImageAttachments=new List<ClaimConnect.ImageAttachment>();
		}

		private void FormClaimAttachment_Load(object sender,EventArgs e) {
			if(_isAlreadyOpen) {
				MsgBox.Show(this,"A claim attachment window is already open.");
				Close();
			}
			_isAlreadyOpen=true;
			_claimPat=Patients.GetPat(_claimCur.PatNum);
			List<Def> imageTypeCategories=new List<Def>();
			List<Def> listClaimAttachmentDefs=CheckImageCatDefs();
			if(listClaimAttachmentDefs.Count<1) {//At least one Claim Attachment image definition exists.
				labelClaimAttachWarning.Visible=true;
			}
			//Non-DXC numbers (NEA) can be wiped out as they are no longer supported by DentalXChange starting 12/01/19.
			if(!string.IsNullOrWhiteSpace(_claimCur.AttachmentID) && !_claimCur.AttachmentID.ToLower().StartsWith("dxc") && 
				MsgBox.Show(this,MsgBoxButtons.YesNo,"The claim has a non DentalXChange Attachment ID. Would you like to clear it out?")) 
			{
				ClearAttachmentID();
			}
			Plugins.HookAddCode(this,"FormClaimAttachment.Load_end",_claimPat,_claimCur,(Action<Bitmap>)ShowImageAttachmentItemEdit);
		}

		private void FormClaimAttachment_Shown(object sender,EventArgs e) {
			FillGrid();
			//Check for if the attachmentID is already in use. If so inform the user they need to redo their attachments.
			ValidateClaimHelper();
			if(textClaimStatus.Text.ToUpper().Contains("ATTACHMENT ID HAS BEEN ASSOCIATED TO A DIFFERENT CLAIM")
				|| textClaimStatus.Text.ToUpper().Contains("HAS ALREADY BEEN DELIVERED TO THE PAYER"))
			{
				MessageBox.Show("The attachment ID is associated to another claim. Please redo your attachments.");
				ClearAttachmentID();
				if(!ValidateClaimHelper()){
					return;
				}
			}
		}

		public static void Open(Claim claim) {
			FormClaimAttachment formClaimAttachment=new FormClaimAttachment(claim);
			//Set the main FormOpenDental as the parent so the form will appear on top of it
			formClaimAttachment.Show(Application.OpenForms.OfType<FormOpenDental>().ToList()[0]);
		}

		private void FormClaimAttachment_FormClosed(object sender,FormClosedEventArgs e) {
			_isAlreadyOpen=false;
		}

		///<summary>Purposely does not load in historical data. This form is only for creating new attachments.
		///The grid is populated by AddImageToGrid().</summary>
		private void FillGrid() {
			gridAttachedImages.BeginUpdate();
			gridAttachedImages.ListGridColumns.Clear();
			gridAttachedImages.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("Date", 80);
			gridAttachedImages.ListGridColumns.Add(col);
			col=new GridColumn("Image Type",150);
			gridAttachedImages.ListGridColumns.Add(col);
			col=new GridColumn("File",150);
			gridAttachedImages.ListGridColumns.Add(col);
			GridRow row;
			for(int i=0;i<_listImageAttachments.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listImageAttachments[i].ImageDate.ToShortDateString());
				row.Cells.Add(_listImageAttachments[i].ImageType.GetDescription());
				row.Cells.Add(_listImageAttachments[i].ImageFileNameDisplay);
				row.Tag=_listImageAttachments[i];
				gridAttachedImages.ListGridRows.Add(row);
			}
			gridAttachedImages.EndUpdate();
		}

		///<summary>Checks to see if the user has made a Claim Attachment image category definition. Returns the list of Defs found.</summary>
		private List<Def> CheckImageCatDefs() {
			//Filter down to image categories that have been marked as Claim Attachment.
			return Defs.GetCatList((int)DefCat.ImageCats).ToList().FindAll(x => x.ItemValue.Contains("C")&&!x.IsHidden);
		}

		///<summary>Returns true if claim is valid.  Includes a progress bar.  Changes textClaimStatus.</summary>
		private bool ValidateClaimHelper() {
			ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
			ProgressOD progressOD=new ProgressOD();
			progressOD.ActionMain=() => validateClaimResponse=ClaimConnect.ValidateClaim(_claimCur,true);
			progressOD.StartingMessage="Communicating with DentalXChange...";
			try{
				progressOD.ShowDialogProgress();
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
			StringBuilder strBuild=new StringBuilder();
			for(int i=0;i<validateClaimResponse.ValidationErrors.Length;i++) {
				strBuild.AppendLine(validateClaimResponse.ValidationErrors[i]);
			}
			textClaimStatus.Text=strBuild.ToString();
			return false;
		}
		
		///<summary>Creates the ClaimAttach objects for each item in the grid and associates it to the given claim.
		///The parameter path should be the full path to the image and fileName should simply be the file name by itself.</summary>
		private ClaimAttach CreateClaimAttachment(string fileNameDisplay,string fileNameActual) {
			ClaimAttach claimAttachment=new ClaimAttach();
			claimAttachment.DisplayedFileName=fileNameDisplay;
			claimAttachment.ActualFileName=fileNameActual;
			claimAttachment.ClaimNum=_claimCur.ClaimNum;
			return claimAttachment;
		}

		private void timerMonitorClipboard_Tick(object sender,EventArgs e) {
			Bitmap bitmapClipboard=GetImageFromClipboard(isSilent:true);
			if(bitmapClipboard==null) {
				return;
			}
			//User made a snip; stop looking at the clipboard
			timerMonitorClipboard.Stop();
			//Start trying to kill Snip & Sketch and Snipping Tool
			_stopwatchKillSnipToolProcesses.Restart();
			timerKillSnipToolProcesses.Start();
			//Show the window in case it was minimized
			WindowState=FormWindowState.Normal;
			//Remove the "waiting for snip" text from the title
			Text=_titleOriginal;
			//Create the attachment but with default values
			CreateImageAttachment(bitmapClipboard,isSnip:true);
			butSnipTool.Enabled=true;
		}

		///<summary>100ms. Monitor the list of running processes for Snip & Sketch and Snipping Tool, for a short duration,
		///and kill any matching processes.  Doesn't stop trying until the duration is over. </summary>
		private void timerKillSnipToolProcesses_Tick(object sender,EventArgs e) {
			List<Process> listProcesses=GetProcessesSnipTool();
			KillProcesses(listProcesses);
			if(_stopwatchKillSnipToolProcesses.Elapsed>TimeSpan.FromSeconds(3)) {
				timerKillSnipToolProcesses.Stop();
				_stopwatchKillSnipToolProcesses.Reset();
			}
		}

		///<summary> Kill all passed-in processes, ignoring any failures, and return True if any were killed. </summary>
		private bool KillProcesses(List<Process> processes) {
			int killed=0;
			for(int i=processes.Count-1;i>=0;i--) {//backward
				try {
					processes[i].Kill();
					killed+=1;
				}
				catch { }
			}
			return killed>0;
		}

		/// <summary> Return any running Snipping Tool or Snip & Sketch processes </summary>
		private List<Process> GetProcessesSnipTool() {
			string snippingToolProcess="SnippingTool";
			string snipAndSketchProcess="ScreenSketch";
			Process[] processesRunning=Process.GetProcesses();
			List<Process> listProcesses=new List<Process>();
			for(int i=0;i<processesRunning.Length;i++) {
				//Skip any processes that aren't Snipping Tool or Snip & Sketch
				if(processesRunning[i].ProcessName!=snippingToolProcess && processesRunning[i].ProcessName!=snipAndSketchProcess) {
					continue;
				}
				listProcesses.Add(processesRunning[i]);
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
			//Couldn't start Snip & Sketch, so try Snipping Tool next
			if(DoesSnippingToolExist()) {
				Process processSnippingTool=new Process();
				string pathToSnippingTool;
				if(!Environment.Is64BitProcess) {
					pathToSnippingTool="sysnative";
				}
				else {
					pathToSnippingTool="system32";
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

		private void StartSnipping() {
			Clipboard.Clear();
			//If we're in the middle of trying to kill Snip Tool processes, stop for now.
			timerKillSnipToolProcesses.Stop();
			_stopwatchKillSnipToolProcesses.Reset();
			List<Process> listProcesses=GetProcessesSnipTool();
			if(KillProcesses(listProcesses)) {
				//Wait a short time before launching, since otherwise the Win32Exception "The remote procedure call failed and did not execute" can happen
				Thread.Sleep(100);
			}
			if(!StartSnipAndSketchOrSnippingTool()) {
				MsgBox.Show(this,"Neither the Snip & Sketch tool nor the Snipping Tool could be launched.  Copy an image to the clipboard, then use the Paste Image button to add it as an attachment.  If you are on a Remote Desktop connection, launch your local system's Snip & Sketch or Snipping Tool, and make snips using either of those, which will be automatically copied to the clipboard.  If neither tool is available on your system, use the Print Screen keyboard key, or other screenshot software, to copy screenshots to the clipboard.");
				return;
			}
			_titleOriginal=Text;
			Text+=$" ({Lan.g(this,"Waiting For Snip")}...)";
			butSnipTool.Enabled=false;
			//Wait half a second before minimizing, otherwise Snip & Sketch can end up behind Open Dental
			Thread.Sleep(500);
			WindowState=FormWindowState.Minimized;
			//begin monitoring the clipboard for results
			timerMonitorClipboard.Start();
		}

		private void buttonSnipTool_Click(object sender,EventArgs e) {
			StartSnipping();
		}

		private void ShowImageAttachmentItemEdit(Bitmap bitmap) {
			CreateImageAttachment(bitmap);
		}

		private void CreateImageAttachment(Bitmap bitmap,bool isSnip=false) {
			ClaimConnect.ImageAttachment imageAttachment;
			if(bitmap==null) {
				return;
			}
			if(isSnip) {
				//Close all other dialogs but this one first
				FormOpenDental formOpenDental=Application.OpenForms.OfType<FormOpenDental>().ToList()[0];
				formOpenDental.CloseOpenForms(isForceClose:true,isSnip:true);
			}
			//In case this window was minimized
			WindowState=FormWindowState.Normal;
			using FormClaimAttachmentItemEdit form=new FormClaimAttachmentItemEdit(bitmap);
			form.IsSnip=isSnip;
			form.ShowDialog(this);
			imageAttachment=form.ImageAttachment;
			if(form.DialogResult==DialogResult.OK) {
				_listImageAttachments.Add(imageAttachment);
				FillGrid();
				if(form.DoNewSnip) {
					StartSnipping();
				}
			}
		}

		private void buttonAddImage_Click(object sender,EventArgs e) {
			string patFolder=ImageStore.GetPatientFolder(_claimPat,ImageStore.GetPreferredAtoZpath());
			using OpenFileDialog fileDialog=new OpenFileDialog();
			fileDialog.Multiselect=false;
			fileDialog.InitialDirectory=patFolder;
			if(fileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//The filename property is the entire path of the file.
			string selectedFile=fileDialog.FileName;
			if(selectedFile.EndsWith(".pdf")) {
				MessageBox.Show(this,"DentalXChange does not support PDF attachments.");
				return;
			}
			//There is purposely no validation that the user selected an image as that will be handled on Dentalxchange's end.
			try {
				Bitmap bitmap=(Bitmap)Image.FromFile(selectedFile);
				ShowImageAttachmentItemEdit(bitmap);
			}
			catch(System.IO.FileNotFoundException ex) {
				FriendlyException.Show(Lan.g(this,"The selected file at")+": "+selectedFile+" "+Lan.g(this,"could not be found"),ex);
			}
			catch(System.OutOfMemoryException ex) {
				//Image.FromFile() will throw an OOM exception when the image format is invalid or not supported.
				//See MSDN if you have trust issues:  https://msdn.microsoft.com/en-us/library/stf701f5(v=vs.110).aspx
				FriendlyException.Show(Lan.g(this,"The file does not have a valid image format. Please try again or call support."+"\r\n"+ex.Message),ex);
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"An error occurred. Please try again or call support.")+"\r\n"+ex.Message,ex);
			}
		}

		///<summary>Allows users to paste an image from their clipboard. Throws if the content on the clipboard is not a supported image format.</summary>
		private void ButPasteImage_Click(object sender,EventArgs e) {
			Bitmap bitmapPasted;
			bitmapPasted=GetImageFromClipboard();
			if(bitmapPasted!=null) {
				ShowImageAttachmentItemEdit(bitmapPasted);
			}
		}

		private Bitmap GetImageFromClipboard(bool isSilent=false) {
			Bitmap bitmapClipboard=ODClipboard.GetImage();
			if(bitmapClipboard!=null || isSilent) {
				return bitmapClipboard;
			}
			string[] files;
			try {
				files=ODClipboard.GetFileDropList();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return null;
			}
			if(files==null) {
				MsgBox.Show(this,"Could not find a valid image or image file on the clipboard.");
				return null;
			}
			if(files.Length>1) {//Show error if multiple images are in the list
				MsgBox.Show(this,"Can only paste one image from a file at a time");
				return null;
			}
			string fileName=files.First();
			if(!File.Exists(fileName)) {
				MsgBox.Show(this,"The file on the clipboard could not be located. It may have been moved, deleted or renamed.");
				return null;
			}
			try {
				bitmapClipboard=new Bitmap(fileName);//Create a bitmap of the original file to avoid the transparency bug with the image class
			}
			catch(ArgumentException) {//Catch the "Parameter is not valid" error
				MsgBox.Show(this,"Invalid file type when copying image from location. Valid file types include BMP, GIF, JPEG, PNG, and TIFF.");
				return null;
			}
			return bitmapClipboard;
		}

		///<summary>Allows the user to edit an existing ImageAttachment object.</summary>
		private void gridAttachedImages_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			ClaimConnect.ImageAttachment selectedAttachment=gridAttachedImages.SelectedTag<ClaimConnect.ImageAttachment>();
			using MemoryStream memoryStream=new MemoryStream(selectedAttachment.ImageFileAsBase64);
			using Image image=Image.FromStream(memoryStream);
			using FormClaimAttachmentItemEdit FormCAIE=new FormClaimAttachmentItemEdit(
				image,
				selectedAttachment.ImageFileNameDisplay,
				selectedAttachment.ImageDate,
				selectedAttachment.ImageType,
				selectedAttachment.ImageOrientationType=="right");
			FormCAIE.ShowDialog();
			if(FormCAIE.DialogResult==DialogResult.OK) {//Update row
				_listImageAttachments[gridAttachedImages.GetSelectedIndex()]=FormCAIE.ImageAttachment;
				FillGrid();
			}
		}

		private void ContextMenu_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
			ToolStripItem item=e.ClickedItem;
			if(item.Text=="Delete") {
				//Delete every selected row
				foreach(int selectedIndex in gridAttachedImages.SelectedIndices) {
					_listImageAttachments.RemoveAt(selectedIndex);
				}
				FillGrid();
			}
		}

		private void textNarrative_TextChanged(object sender,EventArgs e) {
			labelCharCount.Text = textNarrative.Text.Length+"/2000";
		}

		///<summary>Sends every attachment in the grid to DentalXChange. Sets the claims attachmentID to the response from Dentalxchange. Will also prompt the user to re-validate the claim.</summary>
		private void CreateAndSendAttachments() {
			//Grab all ImageAttachments from the grid.
			List<ClaimConnect.ImageAttachment> listImagesToSend=new List<ClaimConnect.ImageAttachment>();
			for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
				listImagesToSend.Add((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
			}
			AddAttachments(listImagesToSend);
		}

		///<summary>Mimics CreateAndSendAttachments() but is split out for simplicity as this method will be rarely called. Sends every attachment in the grid one-by-one to DentalXChange. Sets the claims attachmentID to the response from Dentalxchange for the first attachment sent. Will also prompt the user to re-validate the claim.</summary>
		private void BatchSendAttachments() {
			ClaimConnect.ImageAttachment attachment;
			for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
				attachment=((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
				AddAttachments(new List<ClaimConnect.ImageAttachment>() { attachment });
			}
		}

		///<summary>Sends the passed-in attachments to ClaimConnect.  Will set the attachment id to the claim if needed.</summary>
		private void AddAttachments(List<ClaimConnect.ImageAttachment> listAttachments) {
			if(string.IsNullOrWhiteSpace(_claimCur.AttachmentID)) {
				//If an attachment has not already been created, create one.
				string attachmentId=ClaimConnect.CreateAttachment(listAttachments, textNarrative.Text,_claimCur);
				//Update claim if attachmentID was set. Must happen here so that the validation will consider the new attachmentID.
				_claimCur.AttachmentID=attachmentId;
				//Set the claims attached flag to 'Misc' so that the attachmentID will write to the PWK segment 
				//when the claim is generated as an 837.
				_claimCur.AttachedFlags="Misc";
			}
			else {//An attachment already exists for this claim.
				ClaimConnect.AddAttachment(_claimCur, listAttachments);
			}
			Claims.Update(_claimCur);
		}

		///<summary>Wipes out the existing attachmentID, makes a securitylog for the old ID, and clears the 'Misc' attached flag on the claim.
		///This must be done when a non-DXC attachmentID has been detected so that claim validation will work as expected.</summary>
		private void ClearAttachmentID() {
			//Blindly set the claim's attached flags back to 'Mail' so that, deep down in the 837 text generation logic, the PWK segment will not be written
			//which will allow DentalXChange to validate the claim as if it is brand new with no attachments. See X837_5010.GenerateMessageText().
			_claimCur.AttachedFlags="Mail";
			string oldAttachmentID=_claimCur.AttachmentID;
			_claimCur.AttachmentID="";
			DateTime claimSecDateTEdit=_claimCur.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			Claims.Update(_claimCur);
			SecurityLogs.MakeLogEntry(Permissions.ClaimEdit,_claimCur.PatNum
				,$"Removed attachmentID {oldAttachmentID} for ClaimNum:{_claimCur.ClaimNum}",_claimCur.ClaimNum,claimSecDateTEdit);
		}

		///<summary>Saves all images in the grid to the patient on the claim's directory in the images module. Also creates
		///a list of ClaimAttach objects to associate to the given claim.</summary>
		private void buttonOK_Click(object sender,EventArgs e) {
			//The user must create an image or narrative attachment before sending.
			if(gridAttachedImages.ListGridRows.Count==0 && textNarrative.Text.Trim().Length==0) {
				MsgBox.Show(this,"An image or narrative must be specified before continuing.");
				return;
			}
			try {
				CreateAndSendAttachments();
			}
			//Creating and sending Attachments will sometimes time out when an arbitrarily large group of attachments are being sent, 
			//at which point each attachment should be sent individually.
			catch(TimeoutException ex) {
				ex.DoNothing();
				ProgressOD progressOD=new ProgressOD();
				progressOD.ActionMain=BatchSendAttachments;
				progressOD.StartingMessage="Sending attachments timed out. Attempting to send individually. Please wait.";
				progressOD.ShowDialogProgress();
				if(progressOD.IsCancelled){
					return;
				}
			}
			catch(ODException ex) {
				//ODExceptions should already be Lans.g when throwing meaningful messages.
				//If they weren't translated, the message was from a third party and shouldn't be translated anyway.
				MessageBox.Show(ex.Message);
				return;
			}
			catch(Exception ex) {
				//a catch-all for any exceptions that could be thrown from this method that aren't from a timeout and aren't handled as ODExceptions
				FriendlyException.Show("An error has occurred while trying to add attachments. If the problem persists please contact your clearinghouse's support.",ex);
				return;
			}
			//Validate the claim, if it isn't valid let the user decide if they want to continue
			if(!ValidateClaimHelper()) {
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"There were errors validating the claim, would you like to continue?")) {
					return;
				}
			}
			//Used for determining which category to save the image attachments to. 0 will save the image to the first category in the Images module.
			long imageTypeDefNum=0;
			Def defClaimAttachCat=CheckImageCatDefs().FirstOrDefault();
			if(defClaimAttachCat!=null) {
				imageTypeDefNum=defClaimAttachCat.DefNum;
			}
			else {//User does not have a Claim Attachment image category, just use the first image category available.
				imageTypeDefNum=Defs.GetCatList((int)DefCat.ImageCats).FirstOrDefault(x => !x.IsHidden).DefNum;
			}
			List<ClaimAttach> listClaimAttachments=new List<ClaimAttach>();
			for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
				ClaimConnect.ImageAttachment imageRow=((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
				if(PrefC.GetBool(PrefName.SaveDXCAttachments)) {
					Document docCur=ImageStore.Import(imageRow.ImageFileAsBase64,imageTypeDefNum,ImageType.Attachment,_claimPat);
					imageRow.ImageFileNameActual=docCur.FileName;
				}
				//Create attachment objects
				listClaimAttachments.Add(CreateClaimAttachment(imageRow.ImageFileNameDisplay,imageRow.ImageFileNameActual));
			}
			//Keep a running list of attachments sent to DXC for the claim. This will show in the attachments listbox.
			_claimCur.Attachments.AddRange(listClaimAttachments);
			Claims.Update(_claimCur);
			MsgBox.Show("Attachments sent successfully!");
			DialogResult=DialogResult.OK;
			Close();
		}

		private void buttonCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
			Close();
		}
	}
}
