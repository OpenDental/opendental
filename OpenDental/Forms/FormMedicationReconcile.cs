using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.IO;

namespace OpenDental {
	public partial class FormMedicationReconcile:FormODBase {
		public Patient PatCur;
		private Bitmap BitmapOriginal;
		private List<EhrMeasureEvent> ehrMeasureEventsList;
		private List<MedicationPat> medList;

		public FormMedicationReconcile() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMedicationReconcile_Load(object sender,EventArgs e) {
			FillMeds();
			FillReconcilesGrid();
		}

		private void FillMeds() {
			Medications.RefreshCache();
			medList=MedicationPats.Refresh(PatCur.PatNum,checkDiscontinued.Checked);
			gridMeds.BeginUpdate();
			gridMeds.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableMedications","Medication"),140);
			gridMeds.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes for Patient"),225);
			gridMeds.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Disc"),10,HorizontalAlignment.Center);//discontinued
			gridMeds.ListGridColumns.Add(col);
			gridMeds.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<medList.Count;i++) {
				row=new GridRow();
				if(medList[i].MedicationNum==0) {
					row.Cells.Add(medList[i].MedDescript);
				}
				else {
					Medication generic=Medications.GetGeneric(medList[i].MedicationNum);
					string medName=Medications.GetMedication(medList[i].MedicationNum).MedName;
					if(generic.MedicationNum!=medList[i].MedicationNum) {//not generic
						medName+=" ("+generic.MedName+")";
					}
					row.Cells.Add(medName);
				}
				row.Cells.Add(medList[i].PatNote);
				if(MedicationPats.IsMedActive(medList[i])) {
					row.Cells.Add("");
				}
				else {//discontinued
					row.Cells.Add("X");
				}
				gridMeds.ListGridRows.Add(row);
			}
			gridMeds.EndUpdate();
		}

		private void resizePictBox() {
			if(pictBox.BackgroundImage!=null) {
				pictBox.BackgroundImage.Dispose();
			}
			int width;
			int height;
			float ratio;
			//Resize the image at the width of the pictBox, then only resize to the height if it doesn't fit.
			width=pictBox.Width-4;
			ratio=(float)width/BitmapOriginal.Width;
			height=(int)(BitmapOriginal.Height*ratio);
			if(height>pictBox.Height) {
				height=pictBox.Height-4;
				ratio=(float)height/BitmapOriginal.Height;
				width=(int)(BitmapOriginal.Width*ratio);
			}
			Bitmap newBitmap=new Bitmap(width,height);
			Graphics g=Graphics.FromImage(newBitmap);
			g.DrawImage(BitmapOriginal,0,0,width,height);
			g.Dispose();
			if(pictBox.BackgroundImage!=null) {
				pictBox.BackgroundImage.Dispose();
			}
			pictBox.BackgroundImage=newBitmap;
		}

		private void FormMedicationReconcile_ResizeEnd(object sender,EventArgs e) {
			resizePictBox();
		}

		private void checkDiscontinued_MouseUp(object sender,MouseEventArgs e) {
			FillMeds();
		}

		private void checkDiscontinued_KeyUp(object sender,KeyEventArgs e) {
			FillMeds();
		}

		private void FillReconcilesGrid() {
			gridReconcileEvents.BeginUpdate();
			gridReconcileEvents.ListGridColumns.Clear();
			GridColumn col=new GridColumn("DateTime",130);
			gridReconcileEvents.ListGridColumns.Add(col);
			col=new GridColumn("Details",600);
			gridReconcileEvents.ListGridColumns.Add(col);
			ehrMeasureEventsList=EhrMeasureEvents.RefreshByType(PatCur.PatNum,EhrMeasureEventType.MedicationReconcile);
			gridReconcileEvents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<ehrMeasureEventsList.Count;i++) {
				row=new GridRow();
				row.Cells.Add(ehrMeasureEventsList[i].DateTEvent.ToString());
				row.Cells.Add(ehrMeasureEventsList[i].EventType.ToString());
				gridReconcileEvents.ListGridRows.Add(row);
			}
			gridReconcileEvents.EndUpdate();
		}


		private void gridMeds_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedPat FormMP=new FormMedPat();
			FormMP.MedicationPatCur=medList[e.Row];
			FormMP.ShowDialog();
			FillMeds();
		}

		private void FormMedicationReconcile_Resize(object sender,EventArgs e) {
			resizePictBox();
		}

		private void butPickRxListImage_Click(object sender,EventArgs e) {	
			if(PrefC.AtoZfolderUsed==DataStorageType.InDatabase) {
				MsgBox.Show(this,"This option is not supported with images stored in the database.");
				return;
			}
			using FormImageSelect formIS=new FormImageSelect();
			formIS.PatNum=PatCur.PatNum;
			formIS.ShowDialog();
			if(formIS.DialogResult!=DialogResult.OK) {
				return;
			}		
			string patFolder=ImageStore.GetPatientFolder(PatCur,ImageStore.GetPreferredAtoZpath());
			Document doc=Documents.GetByNum(formIS.SelectedDocNum);
			if(!ImageStore.HasImageExtension(doc.FileName)) {
				MsgBox.Show(this,"The selected file is not a supported image type.");
				return;
			}
			textDocDateDesc.Text=doc.DateTStamp.ToShortDateString()+" - "+doc.Description.ToString();
			if(BitmapOriginal!=null) {
				BitmapOriginal.Dispose();
			}
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				BitmapOriginal=ImageStore.OpenImage(doc,patFolder);
			}
			else {
				using FormProgress FormP=new FormProgress();
				FormP.DisplayText="Downloading Image...";
				FormP.NumberFormat="F";
				FormP.NumberMultiplication=1;
				FormP.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				FormP.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(patFolder
					,doc.FileName
					,new OpenDentalCloud.ProgressHandler(FormP.UpdateProgress));
				FormP.ShowDialog();
				if(FormP.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				else { 
					using (MemoryStream ms=new MemoryStream(state.FileContent)) {
						BitmapOriginal=new Bitmap(ms);
					}
				}
			}
			Bitmap bitmap=ImageHelper.ApplyDocumentSettingsToImage(doc,BitmapOriginal,ImageSettingFlags.ALL);
			pictBox.BackgroundImage=bitmap;
			resizePictBox();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//select medication from list.  Additional meds can be added to the list from within that dlg
			using FormMedications FormM=new FormMedications();
			FormM.IsSelectionMode=true;
			FormM.ShowDialog();
			if(FormM.DialogResult!=DialogResult.OK) {
				return;
			}
			MedicationPat MedicationPatCur=new MedicationPat();
			MedicationPatCur.PatNum=PatCur.PatNum;
			MedicationPatCur.MedicationNum=FormM.SelectedMedicationNum;
			MedicationPatCur.ProvNum=PatCur.PriProv;
			using FormMedPat FormMP=new FormMedPat();
			FormMP.MedicationPatCur=MedicationPatCur;
			FormMP.IsNew=true;
			FormMP.ShowDialog();
			if(FormMP.DialogResult!=DialogResult.OK) {
				return;
			}
			FillMeds();
		}

		private void butAddEvent_Click(object sender,EventArgs e) {
			EhrMeasureEvent newMeasureEvent = new EhrMeasureEvent();
			newMeasureEvent.DateTEvent=DateTime.Now;
			newMeasureEvent.EventType=EhrMeasureEventType.MedicationReconcile;
			newMeasureEvent.PatNum=PatCur.PatNum;
			newMeasureEvent.MoreInfo="";
			EhrMeasureEvents.Insert(newMeasureEvent);
			FillReconcilesGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridReconcileEvents.SelectedIndices.Length<1) {
				MessageBox.Show("Please select at least one record to delete.");
				return;
			}
			for(int i=0;i<gridReconcileEvents.SelectedIndices.Length;i++) {
				EhrMeasureEvents.Delete(ehrMeasureEventsList[gridReconcileEvents.SelectedIndices[i]].EhrMeasureEventNum);
			}
			FillReconcilesGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		

	}
}