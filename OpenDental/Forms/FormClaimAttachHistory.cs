using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;
using CodeBase;
using OpenDentBusiness.Eclaims;
using System.Text;
using OpenDental.Thinfinity;
using System.Diagnostics;
using OpenDentalCloud.Core;

namespace OpenDental {
	///<summary></summary>
	public partial class FormClaimAttachHistory:FormODBase {
		public Claim ClaimCur;
		public Patient PatientCur;

		public FormClaimAttachHistory() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormClaimAttachHistory_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn("File",350);
			gridMain.Columns.Add(gridColumn);
			gridMain.ListGridRows.Clear();
			GridRow gridRow;
			for(int i=0;i<ClaimCur.Attachments.Count;i++) {
				gridRow=new GridRow();
				gridRow.Cells.Add(ClaimCur.Attachments[i].DisplayedFileName);
				gridRow.Tag=ClaimCur.Attachments[i];
				gridMain.ListGridRows.Add(gridRow);
			}
			gridMain.EndUpdate();
		}

		///<summary>The selected image opens in Microsoft Photos. This code was copied from FormClaimEdit gridSent_CellDoubleClick().</summary>
		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(!PrefC.GetBool(PrefName.SaveDXCAttachments)) {
				MsgBox.Show(this,$"Not allowed to view attachment. Attachments can only be viewed when the 'Save Attachments to Imaging Module' preference is set.");
				return;
			}
			ClaimAttach claimAttach=new ClaimAttach();
			claimAttach=(ClaimAttach)gridMain.ListGridRows[e.Row].Tag;
			string patFolder=ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath());
			if(CloudStorage.IsCloudStorage) {
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName,'/');
				if(!CloudStorage.FileExists(pathAndFileName)) {
					//Couldn't find file, display message and return
					MsgBox.Show(this,"File no longer exists.");
					return;
				}
				//found it, download and display
				//This chunk of code was pulled from FormFilePicker.cs
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				TaskStateDownload taskStateDownload=CloudStorage.DownloadAsync(patFolder,claimAttach.ActualFileName,
					new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				if(formProgress.ShowDialog()==DialogResult.Cancel) {
					taskStateDownload.DoCancel=true;
					return;
				}
				string tempFile=PrefC.GetRandomTempFile(Path.GetExtension(pathAndFileName));
				File.WriteAllBytes(tempFile,taskStateDownload.FileContent);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(tempFile);
				}
				else {
					Process.Start(tempFile);
				}
			}
			else {//Local storage
				string pathAndFileName=ODFileUtils.CombinePaths(patFolder,claimAttach.ActualFileName);
				if(ODBuild.IsWeb()) {
					ThinfinityUtils.HandleFile(pathAndFileName);
				}
				else {
					try {
						Process.Start(pathAndFileName);
					}
					catch(Exception ex) {
						ex.DoNothing();
						MsgBox.Show(this,"Could not open the attachment.");
					}
				}
			}
		}


	}
}