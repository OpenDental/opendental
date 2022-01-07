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
		private Patient _patCur;
		private Document _documentCur;
		///<summary>If the image is stored in the db, it would be a waste of time to update the image if just changing a few fields in this window, so a synch is used.</summary>
		private Document _documentOld;
		//private string _initialCategoryName;
		private bool _isOkDisabled;
		private List<Def> _listImageCatDefs;
		///<summary>Used for permissions in the case where the user has permission to create but not edit a document.</summary>
		private bool _isDocCreate;

		///<summary>This hides a few useless fields.</summary>
		public bool IsMountItem;

		///<summary>Deprecated. Poorly designed.  Use the other constructor. ALWAYS save docCur before loading this form. This constructor only remains in use in the increasingly outdated ControlImages(without j).</summary>
		[Obsolete]
		public FormDocInfo(Patient patCur,Document docCur,string initialCategoryName,bool isOkDisabled=false,bool isDocCreate=false){
			InitializeComponent();
			InitializeLayoutManager();
			_patCur=patCur;
			_documentCur=docCur;
			_documentOld=_documentCur.Copy();
			_isOkDisabled=isOkDisabled;
			_isDocCreate=isDocCreate;
			if(_documentCur.DocNum==0){
				List<Def> listDefNumsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
				_documentCur.DocCategory=listDefNumsImageCats[0].DefNum;
				for(int i=0;i<listDefNumsImageCats.Count;i++){
					if(listDefNumsImageCats[0].ItemName==initialCategoryName){
						_documentCur.DocCategory=listDefNumsImageCats[i].DefNum;
					}
				}
			}
			Lan.F(this);
		}

		///<summary></summary>
		public FormDocInfo(Patient patCur,Document docCur,bool isOkDisabled=false,bool isDocCreate=false){
			InitializeComponent();
			InitializeLayoutManager();
			_patCur=patCur;
			_documentCur=docCur;
			_documentOld=_documentCur.Copy();
			_isOkDisabled=isOkDisabled;
			_isDocCreate=isDocCreate;
			Lan.F(this);
		}

		///<summary></summary>
		public void FormDocInfo_Load(object sender, System.EventArgs e){
			if(_isDocCreate){
				//Blocking for creating is done in other places outside this form
			}
			else if(!Security.IsAuthorized(Permissions.ImageEdit,_documentOld.DateCreated,true)) {
				butOK.Enabled=false;
			}
			if(IsMountItem){
				labelCategory.Visible=false;
				listCategory.Visible=false;
				//labelType.Visible=false;
				//listType.Visible=false;
			}
			else{
				labelMountItem.Visible=false;
			}
			if(_isOkDisabled) {
				butOK.Enabled=false;
			}
			listCategory.Items.Clear();
			_listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listImageCatDefs.Count;i++){
				string folderName=_listImageCatDefs[i].ItemName;
				listCategory.Items.Add(folderName);
				if(i==0 || _listImageCatDefs[i].DefNum==_documentCur.DocCategory){
					listCategory.SelectedIndex=i;
				}
			}
			textDate.Text=_documentCur.DateCreated.ToShortDateString();
			textTime.Text=_documentCur.DateCreated.ToLongTimeString();
			comboProv.Items.AddProvNone();
			comboProv.Items.AddProvsAbbr(Providers.GetDeepCopy());
			comboProv.SetSelectedProvNum(_documentCur.ProvNum);
			listType.Items.Clear();
			listType.Items.AddEnums<ImageType>();
			listType.SelectedIndex=(int)_documentCur.ImgType;
			textToothNumbers.Text=Tooth.FormatRangeForDisplay(_documentCur.ToothNumbers);
			textDescript.Text=_documentCur.Description;
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				string patFolder;
				if(!TryGetPatientFolder(out patFolder)) {
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolder,_documentCur.FileName);
				if(File.Exists(textFileName.Text)) {
					FileInfo fileInfo=new FileInfo(textFileName.Text);
					textSize.Text=fileInfo.Length.ToString("n0");
				}
			}
			else if(CloudStorage.IsCloudStorage) {
				string patFolder;
				if(!TryGetPatientFolder(out patFolder)) {
					return;
				}
				textFileName.Text=ODFileUtils.CombinePaths(patFolder,_documentCur.FileName,'/');
			}
			else {
				labelFileName.Visible=false;
				textFileName.Visible=false;
				butOpen.Visible=false;
				textSize.Text=_documentCur.RawBase64.Length.ToString("n0");
			}
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelToothNums.Visible=false;
				textToothNumbers.Visible=false;
			}
		}

		///<summary>Returns true when the ImageStore was able to find or create a patient folder for the selected patient.  Sets patFolder to the corresponding folder name.
		///Otherwise, displays an error message to the user (with additional details regarding what went wrong) and returns false.
		///Optionally set isFormClosedOnError false if the DialogResult should not be set to Abort and the current window closed.</summary>
		private bool TryGetPatientFolder(out string patFolder,bool isFormClosedOnError=true) {
			patFolder="";
			try {
				patFolder=ImageStore.GetPatientFolder(_patCur,ImageStore.GetPreferredAtoZpath());
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
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(textFileName.Text);
				}
				else {
					System.Diagnostics.Process.Start("Explorer",Path.GetDirectoryName(textFileName.Text));
				}
			}
			else if(CloudStorage.IsCloudStorage) {//First download, then open
				using FormProgress FormP=new FormProgress();
				FormP.DisplayText="Downloading...";
				FormP.NumberFormat="F";
				FormP.NumberMultiplication=1;
				FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				FormP.TickMS=1000;
				string patFolder;
				if(!TryGetPatientFolder(out patFolder,false)) {
					return;
				}
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(patFolder
					,_documentCur.FileName
					,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				//Create temp file here or create the file with the actual name?  Changes made when opening the file won't be saved, so I think temp file is best.
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(_documentCur.FileName));
				File.WriteAllBytes(tempFile,state.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					System.Diagnostics.Process.Start(tempFile);
				}
			}
		}

		private void butAudit_Click(object sender,EventArgs e) {
			List<Permissions> listPermissions=new List<Permissions>();
			listPermissions.Add(Permissions.ImageEdit);
			listPermissions.Add(Permissions.ImageDelete);
			listPermissions.Add(Permissions.ImageCreate);
			listPermissions.Add(Permissions.ImageExport);
			using FormAuditOneType formA=new FormAuditOneType(0,listPermissions,Lan.g(this,"Audit Trail for Document"),_documentCur.DocNum);
			formA.ShowDialog();
		}

		private void butOK_Click(object sender, System.EventArgs e){
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
			DateTime time;
			if(!DateTime.TryParse(textTime.Text,out time)) {
				MsgBox.Show(this,"Please enter a valid time.");
				return;
			}
			//We had a security bug where users could change the date to a more recent date, and then subsequently delete.
			//The code below is for that specific scenario.
			DateTime dateTimeEntered=PIn.DateT(textDate.Text+" "+textTime.Text);
			if(dateTimeEntered>_documentCur.DateCreated) {
				//user is trying to change the date to some date after the previously linked date
				//is the new doc date allowed?
				if(!Security.IsAuthorized(Permissions.ImageDelete,_documentCur.DateCreated,true)) {
					//suppress the default security message above (it's too confusing for this case) and generate our own here
					MessageBox.Show(this,Lan.g(this,"Not allowed to future date this image from")+": "
						+"\r\n"+_documentCur.DateCreated.ToString()+" to "+dateTimeEntered.ToString()
						+"\r\n\r\n"+Lan.g(this,"A user with the SecurityAdmin permission must grant you access for")
						+":\r\n"+GroupPermissions.GetDesc(Permissions.ImageDelete));
					return;
				}
			}
			try{
				_documentCur.ToothNumbers=Tooth.FormatRangeForDb(textToothNumbers.Text);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			_documentCur.DocCategory=_listImageCatDefs[listCategory.SelectedIndex].DefNum;
			_documentCur.DateCreated=dateTimeEntered;	
			_documentCur.ProvNum=comboProv.GetSelectedProvNum();
			_documentCur.Description=textDescript.Text;			
			_documentCur.ImgType=listType.GetSelected<ImageType>();
			if(Documents.Update(_documentCur,_documentOld)) {
				ImageStore.LogDocument(Lan.g(this,"Document Edited")+": ",Permissions.ImageEdit,_documentCur,_documentOld.DateTStamp);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}