using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;
using CodeBase;

namespace OpenDental {
	///<summary>This image picker shows all files within a specified Windows folder and lets you pick one.  Used for Wiki and email templates.</summary>
	public partial class FormImagePicker:FormODBase {
		///<summary>This contains the entire qualified names including path and extension.</summary>
		private List<string> _listImageNames;
		public string ImageNameSelected;
		private string _imageFolder;

		///<summary>Check that the imageFolder exists and is accessible before calling this form.</summary>
		public FormImagePicker(string imageFolder) {
			InitializeComponent();
			InitializeLayoutManager();
			_imageFolder=imageFolder;
			Lan.F(this);
		}

		private void FormImagePicker_Load(object sender,EventArgs e) {
			FillGrid();
		}

		/// <summary></summary>
		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Image Name"),70);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			List<string> listFileNames=null;
			try {
				listFileNames=FileAtoZ.GetFilesInDirectory(_imageFolder);//All files from the wiki file path, including images and other files.
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
				DialogResult=DialogResult.Cancel;
				return;
			}
			_listImageNames=new List<string>();
			for(int i=0;i<listFileNames.Count;i++) {
				//If the user has entered a search keyword, then only show file names which contain the keyword.
				if(textSearch.Text!="" && !Path.GetFileName(listFileNames[i]).ToLower().Contains(textSearch.Text.ToLower())) {
					continue;
				}
				//Only add image files to the ImageNamesList, not other files such at text files.
				if(ImageHelper.HasImageExtension(listFileNames[i])) {
					_listImageNames.Add(listFileNames[i]);
				}
			}
			for(int i=0;i<_listImageNames.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(Path.GetFileName(_listImageNames[i]));
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			labelImageSize.Text=Lan.g(this,"Image Size")+":";
			picturePreview.Image=null;
			picturePreview.Invalidate();
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			paintPreviewPicture();
		}

		private void paintPreviewPicture() {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			string imagePath=_listImageNames[gridMain.GetSelectedIndex()];
			Image imageTmp=FileAtoZ.GetImage(imagePath);//Could throw an exception if someone deletes the image right after this window loads.
			ODImaging.ImageApplyOrientation(imageTmp);
			picturePreview.Image?.Dispose();
			picturePreview.Image=ODImaging.ImageScaleMaxHeightAndWidth(imageTmp,picturePreview.Height,picturePreview.Width);
			labelImageSize.Text=Lan.g(this,"Image Size")+": "+(int)imageTmp.PhysicalDimension.Width+" x "+(int)imageTmp.PhysicalDimension.Height;
			picturePreview.Invalidate();
			imageTmp?.Dispose();
		}

		private void FormWikiImages_ResizeEnd(object sender,EventArgs e) {
			paintPreviewPicture();
		}

		private void butImport_Click(object sender,EventArgs e) {
			string[] stringArrayFileNames;
			if(!ODBuild.IsThinfinity() && ODCloudClient.IsAppStream) {
				List<string> listImportFilePaths=new List<string>(){ODCloudClient.ImportFileForCloud()};
				if(listImportFilePaths[0].IsNullOrEmpty()) {
					return;
				}
				stringArrayFileNames=listImportFilePaths.ToArray();
			}
			else {
				using OpenFileDialog openFileDialog=new OpenFileDialog();
				openFileDialog.Multiselect=true;
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				stringArrayFileNames=openFileDialog.FileNames;
			}
			Invalidate();
			for(int i=0;i<stringArrayFileNames.Length;i++) {
				//check file types?
				string destinationPath=FileAtoZ.CombinePaths(_imageFolder,Path.GetFileName(stringArrayFileNames[i]));
				if(!FileAtoZ.Exists(destinationPath)){
					FileAtoZ.Copy(stringArrayFileNames[i],destinationPath,FileAtoZSourceDestination.LocalToAtoZ);
					return;
				}
				//from here down, file already exists
				InputBoxParam inputBoxParam=new InputBoxParam();
				inputBoxParam.InputBoxType_=InputBoxType.TextBox;
				inputBoxParam.LabelText=Lan.g(this,"New file name.");
				inputBoxParam.Text=Path.GetFileName(stringArrayFileNames[i]);
				InputBox inputBox=new InputBox(inputBoxParam);
				inputBox.ShowDialog();
				if(inputBox.IsDialogCancel) {
					continue;//cancel, next file.
				}
				bool isCancel=false;
				string stringResult=inputBox.StringResult;
				while(true){
					if(!string.IsNullOrWhiteSpace(stringResult) && !FileAtoZ.Exists(FileAtoZ.CombinePaths(_imageFolder,stringResult))){
						break;
					}
					MsgBox.Show(this,"File name cannot be blank or in use.");
					InputBox inputBoxRefresh=new InputBox(inputBoxParam);
					inputBoxRefresh.ShowDialog();
					stringResult=inputBoxRefresh.StringResult;
					if(inputBoxRefresh.IsDialogCancel) {
						isCancel=true;
						break;
					}
				}
				if(isCancel) {
					continue;//cancel rename, and go to next file.
				}
				destinationPath=FileAtoZ.CombinePaths(_imageFolder,stringResult);
				FileAtoZ.Copy(stringArrayFileNames[i],destinationPath,FileAtoZSourceDestination.LocalToAtoZ);
				stringArrayFileNames[i]=stringResult;
			}
			FillGrid();
			if(stringArrayFileNames.Length==1) {//if importing exactly one image, select it upon returning.
				textSearch.Text=Path.GetFileName(stringArrayFileNames[0]);
			}
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			ImageNameSelected=Path.GetFileName(_listImageNames[gridMain.GetSelectedIndex()]);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			ImageNameSelected=Path.GetFileName(_listImageNames[gridMain.GetSelectedIndex()]);
			DialogResult=DialogResult.OK;
		}

	}
}