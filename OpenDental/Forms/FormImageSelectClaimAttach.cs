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
		private Document[] Docs;
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),100);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Category"),120);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Description"),300);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			Docs=Documents.GetAllWithPat(PatNum);
			for(int i=0;i<Docs.Length;i++){
				row=new GridRow();
				row.Cells.Add(Docs[i].DateCreated.ToString());
				row.Cells.Add(Defs.GetName(DefCat.ImageCats,Docs[i].DocCategory));
			  row.Cells.Add(Docs[i].Description);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			SaveAttachment();
		}

		private void SaveAttachment(){
			Patient PatCur=Patients.GetPat(PatNum);
			//if(PatCur.ImageFolder=="") {
			//	MsgBox.Show(this,"Invalid patient image folder.");
			//	return;
			//}
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"Error. Not using AtoZ images folder.");
				return;
			}
			string patfolder=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
				//ODFileUtils.CombinePaths(
				//FormPath.GetPreferredImagePath(),PatCur.ImageFolder.Substring(0,1).ToUpper(),PatCur.ImageFolder);
			//if(!Directory.Exists(patfolder)) {
			//	MsgBox.Show(this,"Patient folder not found in AtoZ folder.");
			//	return;
			//}
			Document doc=Docs[gridMain.GetSelectedIndex()];
			if(!ImageHelper.HasImageExtension(doc.FileName)){
				MsgBox.Show(this,"Invalid file.  Only images may be attached, no other file format.");
				return;
			}
			string oldPath=ODFileUtils.CombinePaths(patfolder,doc.FileName);
			Random rnd=new Random();
			string newName=DateTime.Now.ToString("yyyyMMdd")+"_"+DateTime.Now.TimeOfDay.Ticks.ToString()+rnd.Next(1000).ToString()
				+Path.GetExtension(oldPath);
			string attachPath=EmailAttaches.GetAttachPath();
			string newPath=ODFileUtils.CombinePaths(attachPath,newName);
			if(CloudStorage.IsCloudStorage) {
				oldPath=oldPath.Replace("\\","/");
				newPath=newPath.Replace("\\","/");
			}
			try {
				if(ImageHelper.HasImageExtension(oldPath)) {
					if(doc.CropH !=0
						|| doc.CropW !=0
						|| doc.CropX !=0
						|| doc.CropY !=0
						|| doc.DegreesRotated !=0
						|| doc.IsFlipped
						|| doc.WindowingMax !=0
						|| doc.WindowingMin !=0
						|| CloudStorage.IsCloudStorage) 
					{
						//this does result in a significantly larger images size if jpg.  A later optimization would recompress it.
						Bitmap bitmapold=null;
						if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
							bitmapold=(Bitmap)Bitmap.FromFile(oldPath);
							Bitmap bitmapnew=ImageHelper.ApplyDocumentSettingsToImage(doc,bitmapold,ImageSettingFlags.ALL);
							bitmapnew.Save(newPath);
						}
						else if(CloudStorage.IsCloudStorage) {
							//First, download the file. 
							using FormProgress FormP=new FormProgress();
							FormP.DisplayText="Downloading Image...";
							FormP.NumberFormat="F";
							FormP.NumberMultiplication=1;
							FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
							FormP.TickMS=1000;
							OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(patfolder
								,doc.FileName
								,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
							FormP.ShowDialog();
							if(FormP.DialogResult==DialogResult.Cancel) {
								state.DoCancel=true;
								return;
							}
							//Successfully downloaded, now do stuff with state.FileContent
							using FormProgress FormP2=new FormProgress();
							FormP2.DisplayText="Uploading Image for Claim Attach...";
							FormP2.NumberFormat="F";
							FormP2.NumberMultiplication=1;
							FormP2.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
							FormP2.TickMS=1000;
							OpenDentalCloud.Core.TaskStateUpload state2=CloudStorage.UploadAsync(attachPath
								,newName
								,state.FileContent
								,new OpenDentalCloud.ProgressHandler(FormP2.UpdateProgress));
							FormP2.ShowDialog();
							if(FormP2.DialogResult==DialogResult.Cancel) {
								state2.DoCancel=true;
								return;
							}
							//Upload was successful
						}
					}
					else {
						File.Copy(oldPath,newPath);
					}
				}
				else {
					File.Copy(oldPath,newPath);
				}
				ClaimAttachNew=new ClaimAttach();
				ClaimAttachNew.DisplayedFileName=Docs[gridMain.GetSelectedIndex()].FileName;
				ClaimAttachNew.ActualFileName=newName;
				DialogResult=DialogResult.OK;
			}
			catch(FileNotFoundException ex) {
				MessageBox.Show(Lan.g(this,"File not found: ")+ex.Message);
				return;
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
			}
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





















