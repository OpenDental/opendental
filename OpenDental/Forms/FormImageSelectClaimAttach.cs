using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>Only used from Claim Edit to "attach" an image. These are not the kind that get sent with claims.</summary>
	public partial class FormImageSelectClaimAttach:FormODBase {
		public long PatNum;
		private Document[] _documentArray;
		///<summary>If DialogResult==OK, then this will contain the new ClaimAttach with the filename that the file was saved under.  File will be in the EmailAttachments folder.  But ClaimNum will not be set.</summary>
		public ClaimAttach ClaimAttachNew;

		///<summary></summary>
		public FormImageSelectClaimAttach()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormImageSelect_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),100);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Category"),120);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),300);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_documentArray=Documents.GetAllWithPat(PatNum);
			for(int i=0;i<_documentArray.Length;i++){
				row=new GridRow();
				row.Cells.Add(_documentArray[i].DateCreated.ToString());
				row.Cells.Add(Defs.GetName(DefCat.ImageCats,_documentArray[i].DocCategory));
			  row.Cells.Add(_documentArray[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SaveAttachment();
		}

		private void SaveAttachment(){
			Patient patient=Patients.GetPat(PatNum);
			//if(PatCur.ImageFolder=="") {
			//	MsgBox.Show(this,"Invalid patient image folder.");
			//	return;
			//}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Error. Not using AtoZ images folder.");
				return;
			}
			string patientfolder=ImageStore.GetPatientFolder(patient,ImageStore.GetPreferredAtoZpath());
				//ODFileUtils.CombinePaths(
				//FormPath.GetPreferredImagePath(),PatCur.ImageFolder.Substring(0,1).ToUpper(),PatCur.ImageFolder);
			//if(!Directory.Exists(patfolder)) {
			//	MsgBox.Show(this,"Patient folder not found in AtoZ folder.");
			//	return;
			//}
			Document document=_documentArray[gridMain.GetSelectedIndex()];
			if(!ImageHelper.HasImageExtension(document.FileName)){
				MsgBox.Show(this,"Invalid file.  Only images may be attached, no other file format.");
				return;
			}
			string oldPath=ODFileUtils.CombinePaths(patientfolder,document.FileName);
			if(!File.Exists(oldPath)) {
				MsgBox.Show(this,"File not found: "+oldPath);
				return;
			}
			Random random=new Random();
			string newName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+random.Next(1000).ToString()
				+Path.GetExtension(oldPath);
			string attachPath=EmailAttaches.GetAttachPath();
			string newPath=ODFileUtils.CombinePaths(attachPath,newName);
			if(CloudStorage.IsCloudStorage) {
				oldPath=oldPath.Replace("\\","/");
				newPath=newPath.Replace("\\","/");
			}
			if(!ImageHelper.HasImageExtension(oldPath)) {
				try {
					File.Copy(oldPath,newPath); 
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				ClaimAttachNew=new ClaimAttach();
				ClaimAttachNew.DisplayedFileName=_documentArray[gridMain.GetSelectedIndex()].FileName;
				ClaimAttachNew.ActualFileName=newName;
				DialogResult=DialogResult.OK;
				return;
			}
			if(document.CropH ==0
				&& document.CropW ==0
				&& document.CropX ==0
				&& document.CropY ==0
				&& document.DegreesRotated ==0
				&& !document.IsFlipped
				&& document.WindowingMax ==0
				&& document.WindowingMin ==0
				&& !CloudStorage.IsCloudStorage) 
			{
				try {
					File.Copy(oldPath,newPath); 
				}
				catch(Exception ex) {
					MessageBox.Show(ex.Message);
					return;
				}
				ClaimAttachNew=new ClaimAttach();
				ClaimAttachNew.DisplayedFileName=_documentArray[gridMain.GetSelectedIndex()].FileName;
				ClaimAttachNew.ActualFileName=newName;
				DialogResult=DialogResult.OK;
				return;
			}
			//this does result in a significantly larger images size if jpg.  A later optimization would recompress it.
			Bitmap bitmapold=null;
			if(PrefC.AtoZfolderUsed!=DataStorageType.LocalAtoZ) {
				bitmapold=(Bitmap)Bitmap.FromFile(oldPath);  
				Bitmap bitmapnew=ImageHelper.ApplyDocumentSettingsToImage(document,bitmapold,ImageSettingFlags.ALL);
				bitmapnew.Save(newPath); 
				ClaimAttachNew=new ClaimAttach();
				ClaimAttachNew.DisplayedFileName=_documentArray[gridMain.GetSelectedIndex()].FileName;
				ClaimAttachNew.ActualFileName=newName;
				DialogResult=DialogResult.OK;
				return;
			}
			if(!CloudStorage.IsCloudStorage) {
				ClaimAttachNew=new ClaimAttach();
				ClaimAttachNew.DisplayedFileName=_documentArray[gridMain.GetSelectedIndex()].FileName;
				ClaimAttachNew.ActualFileName=newName;
				DialogResult=DialogResult.OK;
				return;
			}
			//IsCloudStorage from here down--------------------------------------------------------------------
			//First, download the file. 
			using FormProgress formProgress=new FormProgress();
			formProgress.DisplayText="Downloading Image...";
			formProgress.NumberFormat="F";
			formProgress.NumberMultiplication=1;
			formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
			formProgress.TickMS=1000;
			OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(patientfolder
				,document.FileName
				,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
			formProgress.ShowDialog();
			if(formProgress.DialogResult==DialogResult.Cancel) {
				state.DoCancel=true;
				return;
			}
			//Successfully downloaded, now do stuff with state.FileContent
			using FormProgress formProgress2=new FormProgress();
			formProgress2.DisplayText="Uploading Image for Claim Attach...";
			formProgress2.NumberFormat="F";
			formProgress2.NumberMultiplication=1;
			formProgress2.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
			formProgress2.TickMS=1000;
			OpenDentalCloud.Core.TaskStateUpload state2=CloudStorage.UploadAsync(attachPath
				,newName
				,state.FileContent
				,new OpenDentalCloud.ProgressHandler(formProgress2.UpdateProgress));
			formProgress2.ShowDialog(); 
			if(formProgress2.DialogResult==DialogResult.Cancel) {
				state2.DoCancel=true;
				return;
			}
			//Upload was successful
			ClaimAttachNew=new ClaimAttach();
			ClaimAttachNew.DisplayedFileName=_documentArray[gridMain.GetSelectedIndex()].FileName;
			ClaimAttachNew.ActualFileName=newName;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1){
				MsgBox.Show(this,"Please select an image first.");
				return;
			}
			SaveAttachment();
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















