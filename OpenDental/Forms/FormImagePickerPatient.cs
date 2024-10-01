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

namespace OpenDental {
	///<summary>This image picker shows all images and mounts for a patient and lets you pick one. The code in this form was basically just copied in chunks from ControlImagesJ.</summary>
	public partial class FormImagePickerPatient:FormODBase {
		public Patient PatientCur;
		///<summary>Handles both incoming and outgoing selections</summary>
		public long DocNumSelected;
		private WpfControls.UI.ImageSelector imageSelector;
		///<summary>Handles both incoming and outgoing selections</summary>
		public long MountNumSelected;
		private string _patFolder;

		///<summary>Check that the imageFolder exists and is accessible before calling this form.</summary>
		public FormImagePickerPatient() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			imageSelector=new WpfControls.UI.ImageSelector();
			elementHostImageSelector.Child=imageSelector;
			imageSelector.ItemDoubleClick+=imageSelector_ItemDoubleClick;
			imageSelector.SelectionChangeCommitted+=imageSelector_SelectionChangeCommitted;
			float scaleZoom=LayoutManager.ScaleMyFont();
			imageSelector.LayoutTransform=new System.Windows.Media.ScaleTransform(scaleZoom,scaleZoom);
		}

		private void FormImagePickerPatient_Load(object sender,EventArgs e) {
			FillTree();
			if(DocNumSelected!=0){
				imageSelector.SetSelected(EnumImageNodeType.Document,DocNumSelected);
			}
			if(MountNumSelected!=0){
				imageSelector.SetSelected(EnumImageNodeType.Mount,MountNumSelected);
			}
			FillImage();
		}

		///<summary>Refreshes list from db, then fills the treeview.  Set keepSelection to true in order to keep the current selection active.</summary>
		public void FillTree() {
			List<Def> listDefsImageCats=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			imageSelector.SetCategories(listDefsImageCats);
			DataSet dataSet=Documents.RefreshForPatient(PatientCur.PatNum);
			_patFolder=ImageStore.GetPatientFolder(PatientCur,ImageStore.GetPreferredAtoZpath());
			imageSelector.SetData(PatientCur,dataSet.Tables["DocumentList"],keepSelection:false,_patFolder);
			imageSelector.LoadExpandedPrefs();
		}

		private void imageSelector_SelectionChangeCommitted(object sender,EventArgs e) {
			FillImage();
		}

		private void FillImage(){
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			if(nodeType==EnumImageNodeType.Document){
				long docNum=priKey;
				Bitmap bitmap=ImageHelper.GetBitmapOfDocumentFromDb(docNum);
				pictureBox.Image?.Dispose();
				pictureBox.Image=bitmap;
			}
			if(nodeType==EnumImageNodeType.Mount){
				long mountNum=priKey;
				Bitmap bitmap=MountHelper.GetBitmapOfMountFromDb(mountNum);
				pictureBox.Image?.Dispose();
				pictureBox.Image=bitmap;
			}
		}

		private void imageSelector_ItemDoubleClick(object sender,EventArgs e) {
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			if(nodeType==EnumImageNodeType.Document){
				DocNumSelected=priKey;
				MountNumSelected=0;
			}
			else if(nodeType==EnumImageNodeType.Mount){
				MountNumSelected=priKey;
				DocNumSelected=0;
			}
			else{
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			EnumImageNodeType nodeType=imageSelector.GetSelectedType();
			long priKey=imageSelector.GetSelectedKey();
			if(nodeType==EnumImageNodeType.Document){
				DocNumSelected=priKey;
				MountNumSelected=0;
			}
			else if(nodeType==EnumImageNodeType.Mount){
				MountNumSelected=priKey;
				DocNumSelected=0;
			}
			else{
				return;
			}
			DialogResult=DialogResult.OK;
		}

	}
}