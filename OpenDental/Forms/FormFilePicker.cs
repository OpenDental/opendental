using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using OpenDental.UI;
using System.Drawing;
using OpenDentBusiness;
using System.IO;
using CodeBase;
using OpenDental.Thinfinity;

namespace OpenDental {
	public partial class FormFilePicker:FormODBase {
		///<summary>List of selected files, including their path.</summary>
		public List<string> ListSelectedFiles=new List<string>();
		///<summary>If this is true, the "Select Local File" button will be invisible.</summary>
		public bool DoHideLocalButton;
		///<summary>Re-use the memory for each new thumbmail.  This prevents memory leaks.</summary>
		private Bitmap _bitmapThumbnail;
		///<summary>The SelectedFiles are local files, not files from the cloud.</summary>
		public bool WasLocalFileSelected;

		public FormFilePicker(string defaultPath) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			textPath.Text=defaultPath;
		}

		private void FormFilePicker_Load(object sender,EventArgs e) {
			if(ODCloudClient.IsAppStream){
				DoHideLocalButton=true;
			}
			butFileChoose.Visible=!DoHideLocalButton;
			FillGrid();
		}

		private void FillGrid() {
			//Get Cloud directory based on textPath.Text
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("FilePickerTable","File Name"),20);
			col.IsWidthDynamic=true;
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//Get list of contents in directory of textPath.Text
			OpenDentalCloud.Core.TaskStateListFolders taskStateListFoldersState=CloudStorage.ListFolderContents(textPath.Text);
			List<string> listFiles=taskStateListFoldersState.ListFolderPathsDisplay;
			for(int i=0;i<listFiles.Count;i++){
				row=new GridRow();
				row.Cells.Add(Path.GetFileName(listFiles[i]));	
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butGo_Click(object sender,EventArgs e) {
			//Refresh the grid contents to show whatever is in the Path textbox.
			if(!textPath.Text.Contains(ImageStore.GetPreferredAtoZpath())) {
				textPath.Text=ImageStore.GetPreferredAtoZpath();//They deleted the path for some reason.  It must have at least the base path.
			}
			FillGrid();
		}

		private void butPreview_Click(object sender,EventArgs e) {
			//A couple options here
			//Download the file and run the explorer windows process to show the temporary file
			if(!gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text.Contains(".")) {//File path doesn't contain an extension and thus is a subfolder.
				return;
			}
			using FormProgress formProgress=new FormProgress();
			formProgress.DisplayText="Downloading...";
			formProgress.NumberFormat="F";
			formProgress.NumberMultiplication=1;
			formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
			formProgress.TickMS=1000;
			OpenDentalCloud.ProgressHandler progressHandler=new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress);
			string fileName=Path.GetFileName(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text);
			OpenDentalCloud.Core.TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(textPath.Text,fileName,progressHandler);
			if(formProgress.ShowDialog()==DialogResult.Cancel) {
				taskStateDownload.DoCancel=true;
				return;
			}
			string extension = Path.GetExtension(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text);
			string tempFile=ODFileUtils.CreateRandomFile(Path.GetTempPath(),extension);
			File.WriteAllBytes(tempFile,taskStateDownload.FileContent);
			if(ODBuild.IsWeb()) {
				ThinfinityUtils.HandleFile(tempFile);
			}
			else {
				System.Diagnostics.Process.Start(tempFile);
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			string[] stringArrayFileNames;
			if(ODCloudClient.IsAppStream) {
				List<string> listImportFilePaths=new List<string>(){ODCloudClient.ImportFileForCloud()};
				if(listImportFilePaths[0].IsNullOrEmpty()) {
					return;
				}
				stringArrayFileNames=listImportFilePaths.ToArray();
			}
			else {
				using OpenFileDialog openFileDialog=new OpenFileDialog();
				openFileDialog.Multiselect=true;
				openFileDialog.InitialDirectory="";
				if(openFileDialog.ShowDialog()!=DialogResult.OK) {
					return;
				}
				stringArrayFileNames=openFileDialog.FileNames;
			}
			for(int i=0;i<stringArrayFileNames.Length;i++) {
				string combinedPath=FileAtoZ.CombinePaths(textPath.Text,Path.GetFileName(stringArrayFileNames[i]));
				FileAtoZ.Copy(stringArrayFileNames[i],combinedPath,FileAtoZSourceDestination.LocalToAtoZ);
			}
			FillGrid();
		}

		private void butFileChoose_Click(object sender,EventArgs e) {
			//Choose file using standard windows choose file dialogue.
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			openFileDialog.Multiselect=true;
			openFileDialog.InitialDirectory="";
			if(openFileDialog.ShowDialog()!=DialogResult.OK) {
				return;
			}
			ListSelectedFiles=openFileDialog.FileNames.ToList();
			WasLocalFileSelected=true;
			DialogResult=DialogResult.OK;//Close the window when they choose files this way. 
			//It overrides their selected files  from the cloud, but we have no way of clearing SelectedFiles of the ones they chose in the normal 
			//OpenFileDialog window, so I would rather attach less and be safe than attaching things they may forget they have selected if they had chosen
			//things with OpenFileDialog then went and selected more from the cloud.
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			//Determine if it's a folder or a file that was clicked
			//If a folder, do nothing
			//If a file, download a thumbnail and display it
			if(!ImageStore.HasImageExtension(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text)) {
				labelThumbnail.Visible=false;
				odPictureBox.Visible=false;
				return;
			}
			//Place thumbnail within odPictureox to display
			OpenDentalCloud.Core.TaskStateThumbnail taskStateThumbnail=null;
			try {
				taskStateThumbnail=CloudStorage.GetThumbnail(textPath.Text,gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text);
			}
			catch(Exception ex) {
				labelThumbnail.Visible=false;
				odPictureBox.Visible=false;
				ex.DoNothing();
				return;
			}
			if(taskStateThumbnail==null || taskStateThumbnail.FileContent==null || taskStateThumbnail.FileContent.Length<2) {
				labelThumbnail.Visible=true;
				odPictureBox.Visible=false;
			}
			else {
				labelThumbnail.Visible=false;
				odPictureBox.Visible=true;
				using MemoryStream memoryStream=new MemoryStream(taskStateThumbnail.FileContent);
				_bitmapThumbnail=new Bitmap(Image.FromStream(memoryStream));
				odPictureBox.Image?.Dispose();
				odPictureBox.Image=_bitmapThumbnail;
				odPictureBox.Invalidate();
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//Determine if it's a folder or a file. 
			//If a folder, append the folder's name to the path and display folder contents
			//If it's a file, return it as the only item selected.
			if(gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text.Contains(".")) {//They selected a file because there is an extension.
				ListSelectedFiles.Clear();
				ListSelectedFiles.Add(ODFileUtils.CombinePaths(textPath.Text,gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text,'/'));
				DialogResult=DialogResult.OK;
			}
			else {
				textPath.Text=ODFileUtils.CombinePaths(textPath.Text,gridMain.ListGridRows[gridMain.GetSelectedIndex()].Cells[0].Text,'/');
				FillGrid();
			}
		}

		private void textPath_KeyPress(object sender,KeyEventArgs e) {
			if(e.KeyCode==Keys.Enter) {
				e.Handled=true;
				e.SuppressKeyPress=true;
				butGo_Click(null,null);
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one file.");
				return;
			}
			//Add all selected files to the list to be returned
			ListSelectedFiles.Clear();//Just in case
			for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
				ListSelectedFiles.Add(ODFileUtils.CombinePaths(textPath.Text,gridMain.ListGridRows[gridMain.SelectedIndices[i]].Cells[0].Text,'/'));
			}
			DialogResult=DialogResult.OK;
		}

	}
}