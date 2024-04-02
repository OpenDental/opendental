using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;
using OpenDentBusiness;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental{
///<summary></summary>
	public partial class FormDocInfo : FormODBase {
		private Patient _patient;
		private Document _document;
		///<summary>If the image is stored in the db, it would be a waste of time to update the image if just changing a few fields in this window, so a synch is used.</summary>
		private Document _documentOld;
		//private string _initialCategoryName;
		private bool _isOkDisabled;
		private List<Def> _listDefsImageCats;
		///<summary>Used for permissions in the case where the user has permission to create but not edit a document.</summary>
		private bool _isDocCreate;

		///<summary>This hides the category selection</summary>
		public bool IsMountItem;

		///<summary>Deprecated. Poorly designed.  Use the other constructor. ALWAYS save docCur before loading this form. This constructor only remains in use in the increasingly outdated ControlImages(without j).</summary>
		[Obsolete]
		public FormDocInfo(Patient patient,Document document,string initialCategoryName,bool isOkDisabled=false,bool isDocCreate=false){
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patient;
			_document=document;
			_documentOld=_document.Copy();
			_isOkDisabled=isOkDisabled;
			_isDocCreate=isDocCreate;
			if(_document.DocNum==0){
				List<Def> listDefNumsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				_document.DocCategory=listDefNumsImageCats[0].DefNum;
				for(int i=0;i<listDefNumsImageCats.Count;i++){
					if(listDefNumsImageCats[0].ItemName==initialCategoryName){
						_document.DocCategory=listDefNumsImageCats[i].DefNum;
					}
				}
			}
			Lan.F(this);
		}

		///<summary></summary>
		public FormDocInfo(Patient patient,Document document,bool isOkDisabled=false,bool isDocCreate=false){
			InitializeComponent();
			InitializeLayoutManager();
			_patient=patient;
			_document=document;
			_documentOld=_document.Copy();
			_isOkDisabled=isOkDisabled;
			_isDocCreate=isDocCreate;
			Lan.F(this);
		}

		///<summary></summary>
		public void FormDocInfo_Load(object sender, System.EventArgs e){
			if(_isDocCreate){
				//Blocking for creating is done in other places outside this form
			}
			else if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentOld.DateCreated,suppressMessage:true)) {
				butSave.Enabled=false;
			}
			if(IsMountItem){
				labelCategory.Visible=false;
				listCategory.Visible=false;
			}
			else{
				labelMountItem.Visible=false;
			}
			if(_isOkDisabled) {
				butSave.Enabled=false;
			}
			if(ODEnvironment.IsCloudServer) {
				butOpen.Text="Open File";
				if(ODCloudClient.IsAppStream || Path.GetExtension(_document.FileName).ToLower()!=".pdf") {
					butOpen.Text="Export";
				}
			}
			listCategory.Items.Clear();
			_listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listDefsImageCats.Count;i++){
				string folderName=_listDefsImageCats[i].ItemName;
				listCategory.Items.Add(folderName);
				if(i==0 || _listDefsImageCats[i].DefNum==_document.DocCategory){
					listCategory.SelectedIndex=i;
				}
			}
			textDate.Text=_document.DateCreated.ToShortDateString();
			textTime.Text=_document.DateCreated.ToLongTimeString();
			FillComboProv();
			comboProv.SetSelectedProvNum(_document.ProvNum);
			listType.Items.Clear();
			listType.Items.AddEnums<ImageType>();
			listType.SelectedIndex=(int)_document.ImgType;
			textToothNumbers.Text=Tooth.DisplayRange(_document.ToothNumbers);
			textDescript.Text=_document.Description;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string patFolderName;
				if(!TryGetPatientFolder(out patFolderName)) {
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolderName,_document.FileName);
				if(File.Exists(textFileName.Text)) {
					FileInfo fileInfo=new FileInfo(textFileName.Text);
					textSize.Text=fileInfo.Length.ToString("n0");
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				string patFolderName;
				if(!TryGetPatientFolder(out patFolderName)) {
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolderName,_document.FileName,'/');
			}
			else {
				labelFileName.Visible=false;
				textFileName.Visible=false;
				butOpen.Visible=false;
				textSize.Text=_document.RawBase64.Length.ToString("n0");
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelToothNums.Visible=false;
				textToothNumbers.Visible=false;
			}
		}

		///<summary>Returns true when the ImageStore was able to find or create a patient folder for the selected patient.  Sets patFolder to the corresponding folder name.
		///Otherwise, displays an error message to the user (with additional details regarding what went wrong) and returns false.
		///Optionally set isFormClosedOnError false if the DialogResult should not be set to Abort and the current window closed.</summary>
		private bool TryGetPatientFolder(out string patFolderName,bool isFormClosedOnError=true) {
			patFolderName="";
			try {
				patFolderName=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				if(isFormClosedOnError) {
					this.DialogResult=DialogResult.Abort;
					this.Close();
				}
				return false;
			}
			return true;
		}

		private void butOpen_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				if(ODCloudClient.IsAppStream) {
					CloudClientL.ExportForCloud(textFileName.Text,doPromptForName:false);
				}
				else if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(textFileName.Text);
				}
				else {
					System.Diagnostics.Process.Start("Explorer",Path.GetDirectoryName(textFileName.Text));
				}
			}
			else if(CloudStorage.IsCloudStorage) {//First download, then open
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				string patFolderName;
				if(!TryGetPatientFolder(out patFolderName,isFormClosedOnError:false)) {
					return;
				}
				OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(patFolderName
					,_document.FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					taskStateDownload.DoCancel=true;
					return;
				}
				//Create temp file here or create the file with the actual name?  Changes made when opening the file won't be saved, so I think temp file is best.
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(_document.FileName));
				File.WriteAllBytes(tempFile,taskStateDownload.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					System.Diagnostics.Process.Start(tempFile);
				}
			}
		}

		private void FillComboProv(){
			long provNum=comboProv.GetSelectedProvNum();
			List<Provider> listProviders=Providers.GetProvsForClinic(Clinics.ClinicNum);
			comboProv.Items.Clear();
			comboProv.Items.AddProvNone();
			if(PrefC.GetBool(PrefName.EasyHideDentalSchools)) {//not dental school
				comboProv.Items.AddProvsAbbr(listProviders);
			}
			else{
				comboProv.Items.AddProvsFull(listProviders);
			}
			comboProv.SetSelectedProvNum(provNum);
		}

		private void butAudit_Click(object sender,EventArgs e) {
			List<EnumPermType> listPermissions=new List<EnumPermType>();
			listPermissions.Add(EnumPermType.ImageEdit);
			listPermissions.Add(EnumPermType.ImageDelete);
			listPermissions.Add(EnumPermType.ImageCreate);
			listPermissions.Add(EnumPermType.ImageExport);
			using FormAuditOneType formAuditOneType=new FormAuditOneType(0,listPermissions,Lan.g(this,"Audit Trail for Document"),_document.DocNum);
			formAuditOneType.ShowDialog();
		}

		private void butSave_Click(object sender, System.EventArgs e){
			if(!textDate.IsValid()) {
				MessageBox.Show(Lan.g(this,"Please fix data entry errors first."));
				return;
			}
			if(textDate.Text=="") {
				MsgBox.Show(this,"Please enter a date.");
				return;
			}
			if(textTime.Text=="") {
				MsgBox.Show(this,"Please enter a time.");
				return;
			}
			DateTime date;
			if(!DateTime.TryParse(textTime.Text,out date)) {
				MsgBox.Show(this,"Please enter a valid time.");
				return;
			}
			//We had a security bug where users could change the date to a more recent date, and then subsequently delete.
			//The code below is for that specific scenario.
			DateTime dateTimeEntered=PIn.DateT(textDate.Text+" "+textTime.Text);
			if(dateTimeEntered>_document.DateCreated) {
				//user is trying to change the date to some date after the previously linked date
				//is the new doc date allowed?
				if(!Security.IsAuthorized(EnumPermType.ImageDelete,_document.DateCreated,suppressMessage:true)) {
					//suppress the default security message above (it's too confusing for this case) and generate our own here
					MessageBox.Show(this,Lan.g(this,"Not allowed to future date this image from")+": "
						+"\r\n"+_document.DateCreated.ToString()+" to "+dateTimeEntered.ToString()
						+"\r\n\r\n"+Lan.g(this,"A user with the SecurityAdmin permission must grant you access for")
						+":\r\n"+GroupPermissions.GetDesc(EnumPermType.ImageDelete));
					return;
				}
			}
			try{
				_document.ToothNumbers=Tooth.ParseRange(textToothNumbers.Text);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			_document.DocCategory=_listDefsImageCats[listCategory.SelectedIndex].DefNum;
			_document.DateCreated=dateTimeEntered;	
			_document.ProvNum=comboProv.GetSelectedProvNum();
			_document.Description=textDescript.Text;			
			_document.ImgType=listType.GetSelected<ImageType>();
			if(Documents.Update(_document,_documentOld)) {
				ImageStore.LogDocument(Lan.g(this,"Document Edited")+": ",EnumPermType.ImageEdit,_document,_documentOld.DateTStamp);
			}
			DialogResult=DialogResult.OK;
		}

	}
}