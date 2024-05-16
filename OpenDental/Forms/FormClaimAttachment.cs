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
using Dicom.Imaging.Mathematics;

namespace OpenDental {
	///<summary>As of 5/14/2024, this form will no longer send attachments to DentalXChange. We want users to use the right click attachment workflows instead of this form, so all the controls have been removed from the FormClaimEdit DXC tab. The DXC logic will remain in this form in case we ever need to revert</summary>
	public partial class FormClaimAttachment:FormODBase {
		private Claim _claim;
		private Patient _patient;
		private static bool _isAlreadyOpen=false;
		///<summary>Stores a list of the image Ids from DXC to be saved locally.</summary>
		private List<int> _listImageReferenceIds;
		private static readonly string _snipSketchURI="ms-screensketch";
		///<summary> Keeps track of how long we've been trying to kill all running Snip Tool processes </summary>
		private Stopwatch _stopwatchKillSnipToolProcesses=new Stopwatch();
		private string _titleOriginal;
		private List<ClaimConnect.ImageAttachment> _listImageAttachmentsDXC;
		private List<EDS.ImageAttachment> _listImageAttachmentsEDS;
		private bool _areAttachmentsRequired=false;
		private EclaimsCommBridge _eclaimsCommBridge;

		///<summary>Initialize the form and refresh the claim we are adding attachments to.</summary>
		public FormClaimAttachment(Claim claim,EclaimsCommBridge eClaimsCommBridge) {
			InitializeComponent();
			InitializeLayoutManager();
			_claim=claim;
			_listImageAttachmentsDXC=new List<ClaimConnect.ImageAttachment>();
			_listImageAttachmentsEDS=new List<EDS.ImageAttachment>();
			_eclaimsCommBridge=eClaimsCommBridge;
		}

		public EclaimsCommBridge GetEclaimsCommBridge() {
			return _eclaimsCommBridge;
		}

		private void FormClaimAttachment_Load(object sender,EventArgs e) {
			if(_isAlreadyOpen) {
				MsgBox.Show(this,"A claim attachment window is already open.");
				Close();
				return;
			}
			if(!_eclaimsCommBridge.In(EclaimsCommBridge.ClaimConnect,EclaimsCommBridge.EDS)) {
				MsgBox.Show(this,$"Sending attachments to {_eclaimsCommBridge} clearing house not currently supported");
				Close();
				return;
			}
			_isAlreadyOpen=true;
			_patient=Patients.GetPat(_claim.PatNum);
			List<Def> listDefsClaimAttachments=CheckImageCatDefs();
			if(listDefsClaimAttachments.Count<1) {//At least one Claim Attachment image definition exists.
				labelClaimAttachWarning.Visible=true;
			}
			//Non-DXC numbers (NEA) can be wiped out as they are no longer supported by DentalXChange starting 12/01/19.
			//Will never use ClaimConnect
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect 
				&& !string.IsNullOrWhiteSpace(_claim.AttachmentID) 
				&& !_claim.AttachmentID.ToLower().StartsWith("dxc") 
				&& MsgBox.Show(this,MsgBoxButtons.YesNo,"The claim has a non DentalXChange Attachment ID. Would you like to clear it out?")) 
			{
					ClearAttachmentID();
			}
			textNarrative.Text=_claim.Narrative;
			Plugins.HookAddCode(this,"FormClaimAttachment.Load_end",_patient,_claim,(Action<Bitmap>)ShowImageAttachmentItemEdit);
		}

		private void FormClaimAttachment_Shown(object sender,EventArgs e) {
			FillGrid();
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
				//Check for if the attachmentID is already in use. If so inform the user they need to redo their attachments.
				ValidateClaimDXC();
				if(textClaimStatus.Text.ToUpper().Contains("ATTACHMENT ID HAS BEEN ASSOCIATED TO A DIFFERENT CLAIM")
					|| textClaimStatus.Text.ToUpper().Contains("HAS ALREADY BEEN DELIVERED TO THE PAYER"))
				{
					MessageBox.Show("The attachment ID is associated to another claim. Please redo your attachments.");
					ClearAttachmentID();
					if(!ValidateClaimDXC()){
						return;
					}
				}
			}
			else if (_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				_areAttachmentsRequired=ValidateAttachmentsNeededEDS();
			}
			//Save this for later when we need to revert back to it
			_titleOriginal=Text;
			BringToFront();
		}

		private void FormClaimAttachment_FormClosed(object sender,FormClosedEventArgs e) {
			_isAlreadyOpen=false;
		}

		///<summary>Purposely does not load in historical data. This form is only for creating new attachments.
		///The grid is populated by AddImageToGrid().</summary>
		private void FillGrid() {
			gridAttachedImages.BeginUpdate();
			gridAttachedImages.Columns.Clear();
			gridAttachedImages.ListGridRows.Clear();
			GridColumn col;
			col=new GridColumn("Date", 80);
			gridAttachedImages.Columns.Add(col);
			col=new GridColumn("Image Type",150);
			gridAttachedImages.Columns.Add(col);
			col=new GridColumn("File",150);
			gridAttachedImages.Columns.Add(col);
			int countAttachedImages=0;
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
				countAttachedImages=_listImageAttachmentsDXC.Count;
			}
			else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				countAttachedImages=_listImageAttachmentsEDS.Count;
			}
			GridRow row=new GridRow();
			for(int i=0;i<countAttachedImages;i++) {
				if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
					row=CreateRow(_listImageAttachmentsDXC[i].ImageDate,
						_listImageAttachmentsDXC[i].ImageType.GetDescription(),
						_listImageAttachmentsDXC[i].ImageFileNameDisplay);
					row.Tag=_listImageAttachmentsDXC[i];
				}
				else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
					row=CreateRow(_listImageAttachmentsEDS[i].FileDate,
						_listImageAttachmentsEDS[i].DocumentTypeCode.GetDescription(),
						_listImageAttachmentsEDS[i].FileDisplayName);
					row.Tag=_listImageAttachmentsEDS[i];
				}
				gridAttachedImages.ListGridRows.Add(row);
			}
			gridAttachedImages.EndUpdate();
		}

		/// <summary>Returns a GridRow with 3 cells; 1 for each of the arguments passed in.</summary>
		private GridRow CreateRow(DateTime imageDate,string imageType,string displayName) {
			GridRow gridRow=new GridRow();
			gridRow.Cells.Add(imageDate.ToShortDateString());
			gridRow.Cells.Add(imageType);
			gridRow.Cells.Add(displayName);
			return gridRow;
		}

		///<summary>Checks to see if the user has made a Claim Attachment image category definition. Returns the list of Defs found.</summary>
		private List<Def> CheckImageCatDefs() {
			//Filter down to image categories that have been marked as Claim Attachment.
			return Defs.GetCatList((int)DefCat.ImageCats).ToList().FindAll(x => x.ItemValue.Contains("C")&&!x.IsHidden);
		}

		///<summary>Returns true if claim is valid for DXC.  Includes a progress bar.  Changes textClaimStatus.</summary>
		private bool ValidateClaimDXC() {
			ClaimConnect.ValidateClaimResponse validateClaimResponse=null;
			ProgressWin progressOD=new ProgressWin();
			progressOD.ActionMain=() => validateClaimResponse=ClaimConnect.ValidateClaim(_claim,true);
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
		
		///<summary>Returns true if claim requires attachments, false otherwise. Includes a progress bar. Required check for EDS since attachments can only be sent for claims that require them.</summary>
		private bool ValidateAttachmentsNeededEDS() {
			EDS.ListPayerResponses listPayerResponses=null;
			ProgressWin progressWin=new ProgressWin();
			progressWin.ActionMain=() => listPayerResponses=EDS.ValidateClaim(_claim);
			progressWin.StartingMessage="Communicating with EDS...";
			try{
				progressWin.ShowDialog();
			}
			catch(Exception ex) {
				textClaimStatus.Text=ex.Message;
				return false;
			}
			if(progressWin.IsCancelled){
				return false;
			}
			//If any of the payers require attachments for this claim, we can create an attachment id so this claim is valid.
			if(listPayerResponses.Payers.Any(x => x.ClaimLevelResponse)) {
				textClaimStatus.Text="The claim is valid.";
				return true;
			}
			//All the claimlevelresponses are false, check the procedureresponses.
			//It is possible that an EDS claim does not require attachments but individual procedures do.
			StringBuilder stringBuilder=new StringBuilder();
			for(int i=0;i<listPayerResponses.Payers.Count;i++) {
				List<EDS.ProcedureResponse> listProcedureResponses=listPayerResponses.Payers[i].ProcedureResponses.FindAll(x => x.Response);
				for(int j=0;j<listProcedureResponses.Count;j++) {
					stringBuilder.AppendLine(listProcedureResponses[j].ProcID+":");
					if(!listProcedureResponses[j].Documents.IsNullOrEmpty()) {
						stringBuilder.AppendLine(string.Join("\r\n",listProcedureResponses[j].Documents.Select(x => x.DocumentTypeCode.GetDescription())));
					}
					if(!listProcedureResponses[j].Comments.IsNullOrEmpty()) {
						stringBuilder.AppendLine("Comments: "+listProcedureResponses[j].Comments);
					}
				}
			}
			if(stringBuilder.Length > 0) {//The string builder was added to, meaning some procedurecodes require attachments. Show this to the user.
				textClaimStatus.Text="The claim is valid.\r\nProcedure Codes that require attachments:\r\n"+stringBuilder.ToString();
				return true;
			}
			//No attachments allowed for this claim.
			textClaimStatus.Text="This claim does not require attachments. Attachments can only be added to an EDS claim if they are required.";			
			return false;
		}

		///<summary>Creates the ClaimAttach objects for each item in the grid and associates it to the given claim.
		///The parameter path should be the full path to the image and fileName should simply be the file name by itself.</summary>
		private ClaimAttach CreateClaimAttachment(string fileNameDisplay,string fileNameActual) {
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach.DisplayedFileName=fileNameDisplay;
			claimAttach.ActualFileName=fileNameActual;
			claimAttach.ClaimNum=_claim.ClaimNum;
			return claimAttach;
		}

		private void EndSnipping() {
			timerMonitorClipboard.Stop();
			//Show the window in case it was minimized
			WindowState=FormWindowState.Normal;
			//Remove the "waiting for snip" text from the title
			Text=_titleOriginal;
			butSnipTool.Enabled=true;
			butAddImage.Enabled=true;
			butPasteImage.Enabled=true;
    }

		private void timerMonitorClipboard_Tick(object sender,EventArgs e) {
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
			using Bitmap bitmapClipboard=GetImageFromClipboard(isSilent:true,doShowProgressBar:false);
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
				CreateImageAttachment(bitmapClipboard,isSnip:true);
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
			_titleOriginal=Text;
			Text=_titleOriginal+$" ({Lan.g(this,"Waiting For Snip")}...)";
			butSnipTool.Enabled=false;
			butAddImage.Enabled=false;
			butPasteImage.Enabled=false;
			//Wait half a second before minimizing, otherwise Snip & Sketch can end up behind Open Dental
			Thread.Sleep(500);
			if(!ODEnvironment.IsCloudServer) {
				WindowState=FormWindowState.Minimized;
			}
			//begin monitoring the clipboard for results
			timerMonitorClipboard.Start();
		}

		private void buttonSnipTool_Click(object sender,EventArgs e) {
			if(ODEnvironment.IsCloudServer) {
				ODProgress.ShowAction(()=>StartSnipping(),"Opening snipping tool...");
			}
			else {
				StartSnipping();
			}
		}

		///<summary>Caller should dispose of this bitmap.</summary>
		private void ShowImageAttachmentItemEdit(Bitmap bitmap) {
			CreateImageAttachment(bitmap);
		}

		///<summary>Caller should dispose of this bitmap.</summary>
		private void CreateImageAttachment(Bitmap bitmap,bool isSnip=false) {
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
			BringToFront();
			using FormClaimAttachmentItemEdit formClaimAttachmentItemEdit=new FormClaimAttachmentItemEdit(bitmap,_eclaimsCommBridge);
			formClaimAttachmentItemEdit.IsSnip=isSnip;
			formClaimAttachmentItemEdit.ShowDialog(this);
			if(formClaimAttachmentItemEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
				ClaimConnect.ImageAttachment imageAttachmentDXC=formClaimAttachmentItemEdit.ImageAttachmentDXC;
				_listImageAttachmentsDXC.Add(imageAttachmentDXC);
			}
			else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				EDS.ImageAttachment imageAttachmentEDS=formClaimAttachmentItemEdit.ImageAttachmentEDS;
				_listImageAttachmentsEDS.Add(imageAttachmentEDS);
			}
			FillGrid();
			if(formClaimAttachmentItemEdit.DoNewSnip) {
				if(ODEnvironment.IsCloudServer) {
					ODProgress.ShowAction(()=>StartSnipping(),"Opening snipping tool...");
				}
				else {
					StartSnipping();
				}
			}
		}

		private void buttonAddImage_Click(object sender,EventArgs e) {
			string patFolder=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=false;
			openFileDialog.InitialDirectory=patFolder;
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			//The filename property is the entire path of the file.
			string selectedFile=openFileDialog.FileName;
			if(selectedFile.EndsWith(".pdf")) {
				MessageBox.Show(this,"PDF attachments are not supported.");
				return;
			}
			//There is purposely no validation that the user selected an image as that will be handled on the clearinghouse's end.
			try {
				using Bitmap bitmap=(Bitmap)Image.FromFile(selectedFile);
				ShowImageAttachmentItemEdit(bitmap);
			}
			catch(System.IO.FileNotFoundException ex) {
				FriendlyException.Show(Lan.g(this,"The selected file at")+": "+selectedFile+" "+Lan.g(this,"could not be found"),ex);
			}
			catch(System.OutOfMemoryException ex) {
				//Image.FromFile() will throw an OOM exception when the image format is invalid or not supported.
				//See MSDN if you have trust issues:  https://msdn.microsoft.com/en-us/library/stf701f5(v=vs.110).aspx
				MsgBox.Show(Lan.g(this,"The file does not have a valid image format. Please try again or call support."));
			}
			catch(Exception ex) {
				FriendlyException.Show(Lan.g(this,"An error occurred. Please try again or call support.")+"\r\n"+ex.Message,ex);
			}
		}

		///<summary>Allows users to paste an image from their clipboard. Throws exception if the content on the clipboard is not a supported image format.</summary>
		private void ButPasteImage_Click(object sender,EventArgs e) {
			using Bitmap bitmapPasted=GetImageFromClipboard();
			if(bitmapPasted!=null) {
				ShowImageAttachmentItemEdit(bitmapPasted);
			}
		}

		private Bitmap GetImageFromClipboard(bool isSilent=false, bool doShowProgressBar=true) {
			Bitmap bitmapClipboard=ODClipboard.GetImage();
			if(bitmapClipboard!=null || isSilent) {
				return bitmapClipboard;
			}
			string[] stringArrayFiles;
			try {
				stringArrayFiles=ODClipboard.GetFileDropList();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return null;
			}
			if(stringArrayFiles==null) {
				MsgBox.Show(this,"Could not find a valid image or image file on the clipboard.");
				return null;
			}
			if(stringArrayFiles.Length>1) {//Show error if multiple images are in the list
				MsgBox.Show(this,"Can only paste one image from a file at a time");
				return null;
			}
			string fileName=stringArrayFiles.First();
			if(!File.Exists(fileName)) {
				MsgBox.Show(this,"The file on the clipboard could not be located. It may have been moved, deleted or renamed.");
				return null;
			}
			try {
				bitmapClipboard?.Dispose();
				bitmapClipboard=new Bitmap(fileName);//Create a bitmap of the original file to avoid the transparency bug with the image class
			}
			catch(ArgumentException) {//Catch the "Parameter is not valid" error
				MsgBox.Show(this,"Invalid file type when copying image from location. Valid file types include BMP, GIF, JPEG, PNG, and TIFF.");
				return null;
			}
			return bitmapClipboard;
		}

		private void gridAttachedImages_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
			ClaimConnect.ImageAttachment imageAttachmentSelected=gridAttachedImages.SelectedTag<ClaimConnect.ImageAttachment>();
			using Image image=GetSelectedImage();
			using FormClaimAttachmentItemEdit formClaimAttachmentItemEdit=new FormClaimAttachmentItemEdit(
				image,
				imageAttachmentSelected.ImageFileNameDisplay,
				imageAttachmentSelected.ImageDate,
				imageAttachmentSelected.ImageType,
				imageAttachmentSelected.ImageOrientationType=="right",
				_eclaimsCommBridge);
				formClaimAttachmentItemEdit.ShowDialog();
				if(formClaimAttachmentItemEdit.DialogResult==DialogResult.OK) {//Update row
					_listImageAttachmentsDXC[gridAttachedImages.GetSelectedIndex()]=formClaimAttachmentItemEdit.ImageAttachmentDXC;
					FillGrid();
				}
			}
			else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				EDS.ImageAttachment imageAttachmentSelected=gridAttachedImages.SelectedTag<EDS.ImageAttachment>();
				using Image image=GetSelectedImage();
				using FormClaimAttachmentItemEdit formClaimAttachmentItemEdit=new FormClaimAttachmentItemEdit(
				image,
				imageAttachmentSelected.FileDisplayName,
				imageAttachmentSelected.FileDate,
				imageAttachmentSelected.DocumentTypeCode,
				imageAttachmentSelected.OrientationCode.ToLower()=="right",
				_eclaimsCommBridge,
				imageAttachmentSelected.Narrative);
				formClaimAttachmentItemEdit.ShowDialog();
				if(formClaimAttachmentItemEdit.DialogResult==DialogResult.OK) {//Update row
					_listImageAttachmentsEDS[gridAttachedImages.GetSelectedIndex()]=formClaimAttachmentItemEdit.ImageAttachmentEDS;
					FillGrid();
				}
			}
		}

		private Image GetSelectedImage() {
			byte[] byteArr=new byte[0];
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
				byteArr=gridAttachedImages.SelectedTag<ClaimConnect.ImageAttachment>().ImageFileAsBase64;
			}
			else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				byteArr=gridAttachedImages.SelectedTag<EDS.ImageAttachment>().FileData;
			}
			using MemoryStream memoryStream=new MemoryStream(byteArr);
			return Image.FromStream(memoryStream);
		}

		private void ContextMenu_ItemClicked(object sender,ToolStripItemClickedEventArgs e) {
			ToolStripItem toolStripItem=e.ClickedItem;
			if(toolStripItem.Text=="Delete") {
				//Delete every selected row
				for(int i=gridAttachedImages.SelectedIndices.Length-1;i>=0;i--) {//because we are removing items
					if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
						_listImageAttachmentsDXC.RemoveAt(gridAttachedImages.SelectedIndices[i]);
					}
					else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
						_listImageAttachmentsEDS.RemoveAt(gridAttachedImages.SelectedIndices[i]);
					}
				}
				FillGrid();
			}
		}

		private void textNarrative_TextChanged(object sender,EventArgs e) {
			//EDS requires narratives to be associated to exactly 1 image of type narrative (ideally a picture of the narrative itself but no real way to check that). We want to make sure that if the user is typing a narrative that they have associated it to an appropriate attachment. This is done by addding the attachment first, then selecting the attachment before typing up their narrative.
			if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				EDS.ImageAttachment imageAttachment=gridAttachedImages.SelectedTag<EDS.ImageAttachment>();
				//If there is an attachment selected that is not of type narrative, and the user it trying to type a narrative, block them.
				if((imageAttachment==null || imageAttachment.DocumentTypeCode!=EDS.EnumDocumentTypeCode.Narrative) 
					&& !textNarrative.Text.IsNullOrEmpty()) 
				{
					MsgBox.Show("Please select an attachment of type Narrative to associate this narrative text to.");
					textNarrative.Text="";//Clear out any narrative they are typing. They must select an attachment first.
					return;
				}
				//This image attachment already has a narrative saved so pre-fill with what already exists if what already exsits is is longer than 1 char. This handles the edge case where we're deleting a narrative and we just hit backspace on the last character in the text box. In that case, if we set textNarrative.Text to imageAttachment.Narrative, that character will never be deleted.
				if(textNarrative.Text.IsNullOrEmpty() && textNarrative.Text.Length > 1 && !imageAttachment.Narrative.IsNullOrEmpty()) {
					textNarrative.Text=imageAttachment.Narrative;
				}
				else {//Save the new narrative text.
					imageAttachment.Narrative=textNarrative.Text;
				}
			}
			labelCharCount.Text=textNarrative.Text.Length+"/2000";
		}

		private void gridAttachedImages_SelectionCommitted(object sender,EventArgs e) {
			if(_eclaimsCommBridge!=EclaimsCommBridge.EDS) {
				return;
			}
			EDS.ImageAttachment imageAttachment=gridAttachedImages.SelectedTag<EDS.ImageAttachment>();
			textNarrative.Text=imageAttachment.Narrative;
		}

		///<summary>Sends every attachment in the grid to the clearinghouse. Sets the claims attachmentID to the response from clearinghouse. Will also prompt the user to re-validate the claim.</summary>
		private void CreateAndSendAttachments() {
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {
				//Grab all ImageAttachments from the grid.
				List<ClaimConnect.ImageAttachment> listImageAttachmentsToSends=new List<ClaimConnect.ImageAttachment>();
				for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
					listImageAttachmentsToSends.Add((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
				}
				AddAttachments(listImageAttachmentsToSends);
			}
			else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				//Create/Retrieve the attachment id for this claim. 
				EDS.AttachmentIDResponse attachmentIdResponse;
				attachmentIdResponse=EDS.GetAttachmentID(_claim);
				Claim claimOld=_claim.Copy();
				_claim.AttachmentID=attachmentIdResponse.AttachmentID;
				//Set the claims attached flag to 'Misc' so that the attachmentID will write to the PWK segment 
				//when the claim is generated as an 837.
				_claim.AttachedFlags="Misc";
				Claims.Update(_claim,claimOld);
				//Save the images in the grid to said attachment id. This works like a directory in EDS and will be used to keep track of attachments on their side.
				List<EDS.ImageAttachment> listImageAttachments=gridAttachedImages.GetTags<EDS.ImageAttachment>();
				EDS.SaveAttachments(attachmentIdResponse.AttachmentID,listImageAttachments,_claim.ClinicNum);
			}
		}

		///<summary>Mimics CreateAndSendAttachments() but is split out for simplicity as this method will be rarely called. Sends every attachment in the grid one-by-one to DentalXChange. Sets the claims attachmentID to the response from Dentalxchange for the first attachment sent. Will also prompt the user to re-validate the claim.</summary>
		private void BatchSendAttachments() {
			ClaimConnect.ImageAttachment imageAttachment;
			for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
				List<ClaimConnect.ImageAttachment> listImageAttachments=new List<ClaimConnect.ImageAttachment>();
				imageAttachment=((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
				listImageAttachments.Add(imageAttachment);
				AddAttachments(listImageAttachments);
			}
		}

		///<summary>Sends the passed-in attachments to ClaimConnect.  Will set the attachment id to the claim if needed.</summary>
		private void AddAttachments(List<ClaimConnect.ImageAttachment> listImageAttachments) {
			if(string.IsNullOrWhiteSpace(_claim.AttachmentID)) {
				//If an attachment has not already been created, create one.
				string attachmentId=ClaimConnect.OpenAttachment(_claim,textNarrative.Text);
				//Update claim if attachmentID was set. Must happen here so that the validation will consider the new attachmentID.
				_claim.AttachmentID=attachmentId;
				_listImageReferenceIds=ClaimConnect.AddAttachmentImage(_claim,listImageAttachments);
				ClaimConnect.SubmitAttachment(_claim);
				//Set the claims attached flag to 'Misc' so that the attachmentID will write to the PWK segment 
				//when the claim is generated as an 837.
				_claim.AttachedFlags="Misc";
			}
			else {//An attachment already exists for this claim.
				_listImageReferenceIds=ClaimConnect.AddAttachmentImage(_claim, listImageAttachments);
				if(_claim.Narrative!=textNarrative.Text) {
					ClaimConnect.AddNarrative(_claim,textNarrative.Text);
				}
			}
			_claim.Narrative=textNarrative.Text;
			Claims.Update(_claim);
		}

		///<summary>Wipes out the existing attachmentID, makes a securitylog for the old ID, and clears the 'Misc' attached flag on the claim.
		///This must be done when a non-DXC attachmentID has been detected so that claim validation will work as expected.</summary>
		private void ClearAttachmentID() {
			//Blindly set the claim's attached flags back to 'Mail' so that, deep down in the 837 text generation logic, the PWK segment will not be written
			//which will allow DentalXChange to validate the claim as if it is brand new with no attachments. See X837_5010.GenerateMessageText().
			_claim.AttachedFlags="Mail";
			string oldAttachmentID=_claim.AttachmentID;
			_claim.AttachmentID="";
			_claim.Narrative="";//Clear out narrative when changing attachments
			textNarrative.Text="";
			DateTime claimSecDateTEdit=_claim.SecDateTEdit;//Preserve the date prior to any claim updates effecting it.
			Claims.Update(_claim);
			SecurityLogs.MakeLogEntry(EnumPermType.ClaimEdit,_claim.PatNum
				,$"Removed attachmentID {oldAttachmentID} for ClaimNum:{_claim.ClaimNum}",_claim.ClaimNum,claimSecDateTEdit);
		}

		///<summary>Saves all images in the grid to the patient on the claim's directory in the images module. Also creates
		///a list of ClaimAttach objects to associate to the given claim.</summary>
		private void buttonOK_Click(object sender,EventArgs e) {
			//The user must create an image or narrative attachment before sending.
			if(gridAttachedImages.ListGridRows.Count==0 && textNarrative.Text.Trim().Length==0) {
				MsgBox.Show(this,"An image or narrative must be specified before continuing.");
				return;
			}
			if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
				if(!_areAttachmentsRequired) {
					MsgBox.Show(this,"Unable to add attachments to this claim since EDS does not require them");
					return;
				}
				//EDS requires Narratives be their own image attachments along with their actual narrative text. Grab a list of all Narrative attachments that do not have narratives attached.
				List<EDS.ImageAttachment> listImageAttachments=gridAttachedImages.GetTags<EDS.ImageAttachment>().FindAll(x => x.DocumentTypeCode==EDS.EnumDocumentTypeCode.Narrative && x.Narrative.IsNullOrEmpty());
				if(!listImageAttachments.IsNullOrEmpty()) {
					string message=Lans.g("FormClaimAttachment","The following attachments require Narratives")+"\r\n"+
						string.Join("\r\n",listImageAttachments.Select(x => x.FileDisplayName).ToList());
					MsgBox.Show(message);
					return;
				}
			}
			try {
				CreateAndSendAttachments();
			}
			//Creating and sending DXC Attachments will sometimes time out when an arbitrarily large group of attachments are being sent, 
			//at which point each attachment should be sent individually.
			catch(TimeoutException ex) {
				if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
					ProgressWin progressWin=new ProgressWin();
					progressWin.ActionMain=BatchSendAttachments;
					progressWin.StartingMessage="Sending attachments timed out. Attempting to send individually. Please wait.";
					progressWin.ShowDialog();
					if(progressWin.IsCancelled){
						return;
					}
				}
				else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
					FriendlyException.Show("An error has occurred while trying to add attachments. If the problem persists please contact your clearinghouse's support.",ex);
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
			if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect && !ValidateClaimDXC()) {//Will never use ClaimConnect
				if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"There were errors validating the claim, would you like to continue?")) {
					return;
				}
			}
			//Used for determining which category to save the image attachments to. 0 will save the image to the first category in the Images module.
			long imageTypeDefNum=0;
			Def defClaimAttachCat=CheckImageCatDefs().FirstOrDefault();
			if(defClaimAttachCat==null) {//User does not have a Claim Attachment image category, just use the first image category available.
				imageTypeDefNum=Defs.GetCatList((int)DefCat.ImageCats).FirstOrDefault(x => !x.IsHidden).DefNum;
			}
			else {
				imageTypeDefNum=defClaimAttachCat.DefNum;
			}
			List<ClaimAttach> listClaimAttachments=new List<ClaimAttach>();
			for(int i=0;i<gridAttachedImages.ListGridRows.Count;i++) {
				if(_eclaimsCommBridge==EclaimsCommBridge.ClaimConnect) {//Will never use ClaimConnect
					ClaimConnect.ImageAttachment imageAttachmentRow=((ClaimConnect.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
					if(PrefC.GetBool(PrefName.SaveDXCAttachments)) {
						Document documentCur=ImageStore.Import(imageAttachmentRow.ImageFileAsBase64,imageTypeDefNum,ImageType.Attachment,_patient);
						imageAttachmentRow.ImageFileNameActual=documentCur.FileName;
					}
					//Create attachment objects
					ClaimAttach claimAttach=CreateClaimAttachment(imageAttachmentRow.ImageFileNameDisplay,imageAttachmentRow.ImageFileNameActual);
					claimAttach.ImageReferenceId=_listImageReferenceIds[i];
					listClaimAttachments.Add(claimAttach);
				}
				else if(_eclaimsCommBridge==EclaimsCommBridge.EDS) {
					EDS.ImageAttachment imageAttachmentRow=((EDS.ImageAttachment)gridAttachedImages.ListGridRows[i].Tag);
					if(PrefC.GetBool(PrefName.SaveEDSAttachments)) {
						Document documentCur=ImageStore.Import(imageAttachmentRow.FileData,imageTypeDefNum,ImageType.Attachment,_patient);
						imageAttachmentRow.FileNameActual=documentCur.FileName;
					}
					//Create attachment objects
					ClaimAttach claimAttach=CreateClaimAttachment(imageAttachmentRow.FileDisplayName,imageAttachmentRow.FileNameActual);
					listClaimAttachments.Add(claimAttach);
				}
			}
			//Keep a running list of attachments sent to DXC for the claim. This will show in the attachments listbox.
			_claim.Attachments.AddRange(listClaimAttachments);
			Claims.Update(_claim);
			MsgBox.Show(this,"Attachments sent successfully!");
			DialogResult=DialogResult.OK;
			Close();
		}
	}
}
