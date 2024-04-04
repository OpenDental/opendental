using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDental.Thinfinity;
using OpenDental.UI;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
///<summary></summary>
	public partial class FrmDocInfo : FrmODBase {
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

		///<summary></summary>
		public FrmDocInfo(Patient patient,Document document,bool isOkDisabled=false,bool isDocCreate=false){
			InitializeComponent();
			_patient=patient;
			_document=document;
			_documentOld=_document.Copy();
			_isOkDisabled=isOkDisabled;
			_isDocCreate=isDocCreate;
			Load+=FrmDocInfo_Load;
			KeyDown+=Frm_KeyDown;
			PreviewKeyDown+=FrmDocInfo_PreviewKeyDown;
		}

		///<summary></summary>
		public void FrmDocInfo_Load(object sender, System.EventArgs e){
			Lang.F(this);
			if(_isDocCreate){
				//Blocking for creating is done in other places outside this form
			}
			else if(!Security.IsAuthorized(EnumPermType.ImageEdit,_documentOld.DateCreated,suppressMessage:true)) {
				butSave.IsEnabled=false;
			}
			if(IsMountItem){
				labelCategory.Visible=false;
				listCategory.Visible=false;
			}
			else{
				labelMountItem.Visible=false;
			}
			if(_isOkDisabled) {
				butSave.IsEnabled=false;
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
				string patFolderName=TryGetPatientFolder();
				if(patFolderName.IsNullOrEmpty()) {
					IsDialogCancel=true;
					this.Close();
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolderName,_document.FileName);
				if(File.Exists(textFileName.Text)) {
					FileInfo fileInfo=new FileInfo(textFileName.Text);
					textSize.Text=fileInfo.Length.ToString("n0");
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				string patFolderName=TryGetPatientFolder();
				if(patFolderName.IsNullOrEmpty()) {
					IsDialogCancel=true;
					this.Close();
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolderName,_document.FileName,'/');
				butOpen.Text="Open File";//Open Folder seems like a nice idea. Maybe someone could build that. But this indicates what it currently does.
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
			checkPrintHeading.Checked=_document.PrintHeading;
		}

		///<summary>Returns patient folder name when the ImageStore was able to find or create a patient folder for the selected patient.  Sets patFolder to the corresponding folder name.
		///Otherwise, displays an error message to the user (with additional details regarding what went wrong) and returns empty string.</summary>
		private string TryGetPatientFolder() {
			string patFolderName;
			try {
				patFolderName=ImageStore.GetPatientFolder(_patient,ImageStore.GetPreferredAtoZpath());
			}
			catch(Exception ex) {
				FriendlyException.Show(ex.Message,ex);
				patFolderName="";
			}
			return patFolderName;
		}

		private void Frm_KeyDown(object sender,KeyEventArgs e) {
			if(e.Key==Key.Enter) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butOpen_Click(object sender,EventArgs e) {
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				if(ODCloudClient.IsAppStream) {
					CloudClientL.ExportForCloud(textFileName.Text,doPromptForName:false);
				}
				else if(ODBuild.IsThinfinity()) {
					ThinfinityUtils.HandleFile(textFileName.Text);
				}
				else {
					System.Diagnostics.Process.Start("Explorer",Path.GetDirectoryName(textFileName.Text));
				}
				return;
			}
			if(!CloudStorage.IsCloudStorage) {
				return;
			}
			//CloudStorage from here down------------------------
			string patFolderName=TryGetPatientFolder();
			if(patFolderName.IsNullOrEmpty()) {
				return;
			}
			string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(_document.FileName));
			ProgressWin progressWin=new UI.ProgressWin();
			progressWin.StartingMessage="Downloading...";
			byte[] byteArray=null;
			progressWin.ActionMain=() => {
				byteArray=CloudStorage.Download(patFolderName,_document.FileName);
			};
			progressWin.ShowDialog();
			if(byteArray==null || byteArray.Length==0){
				return;
			}
			File.WriteAllBytes(tempFile,byteArray);
			if(ODBuild.IsThinfinity()) {
				ThinfinityUtils.HandleFile(tempFile);
			}
			else {
				System.Diagnostics.Process.Start(tempFile);
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
			FrmAuditOneType frmAuditOneType=new FrmAuditOneType(0,listPermissions,Lang.g(this,"Audit Trail for Document"),_document.DocNum);
			frmAuditOneType.ShowDialog();
		}

		private void FrmDocInfo_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender, System.EventArgs e){
			if(!textDate.IsValid()) {
				MessageBox.Show(Lang.g(this,"Please fix data entry errors first."));
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
					MessageBox.Show(Lang.g(this,"Not allowed to future date this image from")+": "
						+"\r\n"+_document.DateCreated.ToString()+" to "+dateTimeEntered.ToString()
						+"\r\n\r\n"+Lang.g(this,"A user with the SecurityAdmin permission must grant you access for")
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
			if(checkPrintHeading.Checked==true) {
				_document.PrintHeading=true;
			}
			else {
				_document.PrintHeading=false;
			}
			if(Documents.Update(_document,_documentOld)) {
				ImageStore.LogDocument(Lang.g(this,"Document Edited")+": ",EnumPermType.ImageEdit,_document,_documentOld.DateTStamp);
			}
			IsDialogOK=true;
		}

	}
}