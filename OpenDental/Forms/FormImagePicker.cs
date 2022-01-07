using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;

namespace OpenDental {
	///<summary>This image picker shows all files within a specified Windows folder and lets you pick one.  Used for Wiki and email templates.</summary>
	public partial class FormImagePicker:FormODBase {
		///<summary>This contains the entire qualified names including path and extension.</summary>
		private List<string> ImageNamesList;
		public string SelectedImageName;
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
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Image Name"),70);
			gridMain.ListGridColumns.Add(col);
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
			ImageNamesList=new List<string>();
			for(int i=0;i<listFileNames.Count;i++) {
				//If the user has entered a search keyword, then only show file names which contain the keyword.
				if(textSearch.Text!="" && !Path.GetFileName(listFileNames[i]).ToLower().Contains(textSearch.Text.ToLower())) {
					continue;
				}
				//Only add image files to the ImageNamesList, not other files such at text files.
				if(ImageHelper.HasImageExtension(listFileNames[i])) {
					ImageNamesList.Add(listFileNames[i]);
				}
			}
			for(int i=0;i<ImageNamesList.Count;i++) {
				GridRow row=new GridRow();
				row.Cells.Add(Path.GetFileName(ImageNamesList[i]));
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
			string imagePath=ImageNamesList[gridMain.GetSelectedIndex()];
			Image tmpImg=FileAtoZ.GetImage(imagePath);//Could throw an exception if someone deletes the image right after this window loads.
			float imgScale=1;//will be between 0 and 1
			if(tmpImg.PhysicalDimension.Height>picturePreview.Height || tmpImg.PhysicalDimension.Width>picturePreview.Width) {//image is too large
				//Image is larger than PictureBox, resize to fit.
				if(tmpImg.PhysicalDimension.Width/picturePreview.Width>tmpImg.PhysicalDimension.Height/picturePreview.Height) {//resize image based on width
					imgScale=picturePreview.Width/tmpImg.PhysicalDimension.Width;
				}
				else {//resize image based on height
					imgScale=picturePreview.Height/tmpImg.PhysicalDimension.Height;
				}						
			}
			if(picturePreview.Image!=null) {
				picturePreview.Image.Dispose();
				picturePreview.Image=null;
			}
			picturePreview.Image=new Bitmap(tmpImg,(int)(tmpImg.PhysicalDimension.Width*imgScale),(int)(tmpImg.PhysicalDimension.Height*imgScale));
			labelImageSize.Text=Lan.g(this,"Image Size")+": "+(int)tmpImg.PhysicalDimension.Width+" x "+(int)tmpImg.PhysicalDimension.Height;
			picturePreview.Invalidate();
			if(tmpImg!=null) {
				tmpImg.Dispose();
			}
			tmpImg=null;
		}

		private void FormWikiImages_ResizeEnd(object sender,EventArgs e) {
			paintPreviewPicture();
		}

		private void butImport_Click(object sender,EventArgs e) {
			using OpenFileDialog openFD=new OpenFileDialog();
			openFD.Multiselect=true;
			if(openFD.ShowDialog()!=DialogResult.OK) {
				return;
			}
			Invalidate();
			foreach(string fileName in openFD.FileNames) {
				//check file types?
				string destinationPath=FileAtoZ.CombinePaths(_imageFolder,Path.GetFileName(fileName));
				if(FileAtoZ.Exists(destinationPath)){
					using InputBox ip=new InputBox(Lan.g(this,"New file name."));
					switch(MessageBox.Show(Lan.g(this,"Overwrite Existing File")+": "+destinationPath,"",MessageBoxButtons.YesNoCancel)){
						case DialogResult.No://rename, do not overwrite
							ip.textResult.Text=Path.GetFileName(fileName);
							ip.ShowDialog();
							if(ip.DialogResult!=DialogResult.OK) {
								continue;//cancel, next file.
							}
							bool cancel=false;
							while(!cancel && FileAtoZ.Exists(FileAtoZ.CombinePaths(_imageFolder,ip.textResult.Text))){
								MsgBox.Show(this,"File name already exists.");
								if(ip.ShowDialog()!=DialogResult.OK) {
									cancel=true;
								}
							}
							if(cancel) {
								continue;//cancel rename, and go to next file.
							}
							destinationPath=FileAtoZ.CombinePaths(_imageFolder,ip.textResult.Text);
							break;//proceed to save file.
						case DialogResult.Yes://overwrite
							try {
								FileAtoZ.Delete(destinationPath);
							}
							catch(Exception ex){
								MessageBox.Show(Lan.g(this,"Cannot copy file")+":" +fileName+"\r\n"+ex.Message);
								continue;
							}
							break;//file deleted, proceed to save.
						default://cancel
							continue;//skip this file.
					}
				}
				FileAtoZ.Copy(fileName,destinationPath,FileAtoZSourceDestination.LocalToAtoZ);
			}
			FillGrid();
			if(openFD.FileNames.Length==1) {//if importing exactly one image, select it upon returning.
				textSearch.Text=Path.GetFileName(openFD.FileNames[0]);
			}
		}

		private void textSearch_TextChanged(object sender,EventArgs e) {
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			SelectedImageName=Path.GetFileName(ImageNamesList[gridMain.GetSelectedIndex()]);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.GetSelectedIndex()==-1) {
				return;
			}
			SelectedImageName=Path.GetFileName(ImageNamesList[gridMain.GetSelectedIndex()]);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}