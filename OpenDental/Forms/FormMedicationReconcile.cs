using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMedicationReconcile:FormODBase {
		public Patient PatientCur;
		private Bitmap _bitmapOriginal;
		private List<EhrMeasureEvent> _listEhrMeasureEvents;
		private List<MedicationPat> _listMedicationPats;

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
			_listMedicationPats=MedicationPats.Refresh(PatientCur.PatNum,checkDiscontinued.Checked);
			gridMeds.BeginUpdate();
			gridMeds.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableMedications","Medication"),140);
			gridMeds.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Notes for Patient"),225);
			gridMeds.Columns.Add(col);
			col=new GridColumn(Lan.g("TableMedications","Disc"),10,HorizontalAlignment.Center);//discontinued
			gridMeds.Columns.Add(col);
			gridMeds.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listMedicationPats.Count;i++) {
				row=new GridRow();
				if(_listMedicationPats[i].MedicationNum==0) {
					row.Cells.Add(_listMedicationPats[i].MedDescript);
				}
				else {
					Medication medicationGeneric=Medications.GetGeneric(_listMedicationPats[i].MedicationNum);
					string medName=Medications.GetMedication(_listMedicationPats[i].MedicationNum).MedName;
					if(medicationGeneric.MedicationNum!=_listMedicationPats[i].MedicationNum) {//not generic
						medName+=" ("+medicationGeneric.MedName+")";
					}
					row.Cells.Add(medName);
				}
				row.Cells.Add(_listMedicationPats[i].PatNote);
				if(MedicationPats.IsMedActive(_listMedicationPats[i])) {
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
			if(_bitmapOriginal is null){
				return;
			}
			int width;
			int height;
			float ratio;
			//Resize the image at the width of the pictBox, then only resize to the height if it doesn't fit.
			width=pictBox.Width-4;
			ratio=(float)width/_bitmapOriginal.Width;
			height=(int)(_bitmapOriginal.Height*ratio);
			if(height>pictBox.Height) {
				height=pictBox.Height-4;
				ratio=(float)height/_bitmapOriginal.Height;
				width=(int)(_bitmapOriginal.Width*ratio);
			}
			Bitmap bitmapNew=new Bitmap(width,height);
			using Graphics g=Graphics.FromImage(bitmapNew);
			g.DrawImage(_bitmapOriginal,0,0,width,height);
			pictBox.BackgroundImage?.Dispose();
			pictBox.BackgroundImage=bitmapNew;
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
			gridReconcileEvents.Columns.Clear();
			GridColumn col=new GridColumn("DateTime",130);
			gridReconcileEvents.Columns.Add(col);
			col=new GridColumn("Details",600);
			gridReconcileEvents.Columns.Add(col);
			_listEhrMeasureEvents=EhrMeasureEvents.RefreshByType(PatientCur.PatNum,EhrMeasureEventType.MedicationReconcile);
			gridReconcileEvents.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listEhrMeasureEvents.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listEhrMeasureEvents[i].DateTEvent.ToString());
				row.Cells.Add(_listEhrMeasureEvents[i].EventType.ToString());
				gridReconcileEvents.ListGridRows.Add(row);
			}
			gridReconcileEvents.EndUpdate();
		}


		private void gridMeds_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormMedPat formMedPat=new FormMedPat();
			formMedPat.MedicationPatCur=_listMedicationPats[e.Row];
			formMedPat.ShowDialog();
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
			using FormImageSelect formImageSelect=new FormImageSelect();
			formImageSelect.PatNum=PatientCur.PatNum;
			formImageSelect.ShowDialog();
			if(formImageSelect.DialogResult!=DialogResult.OK) {
				return;
			}		
			string patFolder=ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath());
			Document document=Documents.GetByNum(formImageSelect.SelectedDocNum);
			if(!ImageStore.HasImageExtension(document.FileName)) {
				MsgBox.Show(this,"The selected file is not a supported image type.");
				return;
			}
			textDocDateDesc.Text=document.DateTStamp.ToShortDateString()+" - "+document.Description.ToString();
			_bitmapOriginal?.Dispose();
			if(PrefC.AtoZfolderUsed==DataStorageType.LocalAtoZ) {
				_bitmapOriginal=ImageStore.OpenImage(document,patFolder);
			}
			else {
				using FormProgress formProgress=new FormProgress();
				formProgress.DisplayText="Downloading Image...";
				formProgress.NumberFormat="F";
				formProgress.NumberMultiplication=1;
				formProgress.MaxVal=100;//Doesn't matter what this value is as long as it is greater than 0
				formProgress.TickMS=1000;
				OpenDentalCloud.Core.TaskStateDownload state=CloudStorage.DownloadAsync(patFolder
					,document.FileName
					,new OpenDentalCloud.ProgressHandler(formProgress.UpdateProgress));
				formProgress.ShowDialog();
				if(formProgress.DialogResult==DialogResult.Cancel) {
					state.DoCancel=true;
					return;
				}
				else { 
					using MemoryStream memoryStream=new MemoryStream(state.FileContent);
					_bitmapOriginal=new Bitmap(memoryStream);
				}
			}
			Bitmap bitmap=ImageHelper.ApplyDocumentSettingsToImage(document,_bitmapOriginal,ImageSettingFlags.ALL);
			pictBox.BackgroundImage?.Dispose();
			pictBox.BackgroundImage=bitmap;
			resizePictBox();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			//select medication from list.  Additional meds can be added to the list from within that dlg
			using FormMedications formMedications=new FormMedications();
			formMedications.IsSelectionMode=true;
			formMedications.ShowDialog();
			if(formMedications.DialogResult!=DialogResult.OK) {
				return;
			}
			MedicationPat medicationPat=new MedicationPat();
			medicationPat.PatNum=PatientCur.PatNum;
			medicationPat.MedicationNum=formMedications.SelectedMedicationNum;
			medicationPat.ProvNum=PatientCur.PriProv;
			using FormMedPat formMedPat=new FormMedPat();
			formMedPat.MedicationPatCur=medicationPat;
			formMedPat.IsNew=true;
			formMedPat.ShowDialog();
			if(formMedPat.DialogResult!=DialogResult.OK) {
				return;
			}
			FillMeds();
		}

		private void butAddEvent_Click(object sender,EventArgs e) {
			EhrMeasureEvent ehrMeasureEventNew = new EhrMeasureEvent();
			ehrMeasureEventNew.DateTEvent=DateTime.Now;
			ehrMeasureEventNew.EventType=EhrMeasureEventType.MedicationReconcile;
			ehrMeasureEventNew.PatNum=PatientCur.PatNum;
			ehrMeasureEventNew.MoreInfo="";
			EhrMeasureEvents.Insert(ehrMeasureEventNew);
			FillReconcilesGrid();
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(gridReconcileEvents.SelectedIndices.Length<1) {
				MessageBox.Show("Please select at least one record to delete.");
				return;
			}
			for(int i=0;i<gridReconcileEvents.SelectedIndices.Length;i++) {
				EhrMeasureEvents.Delete(_listEhrMeasureEvents[gridReconcileEvents.SelectedIndices[i]].EhrMeasureEventNum);
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