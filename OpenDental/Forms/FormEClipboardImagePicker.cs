using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEClipboardImagePicker:FormODBase {
		///<summary>List of all definitions in 'EClipboardImageCapture' defcat.</summary>
		private List<Def> _listDefsAllEClipboardImages;
		///<summary>The list of EClipboardImageCaptureDefs that are already selected in the parent form, so we don't want to show the here. Must be set before opening this form.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefs;
		///<summary>After clicking OK, this is the list of selected EClipboardImageCaptureDef.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefsSelected;
		///<summary>Currently selected clinic for which we are editing the eclipboard image capture defs. Must be set before opening this form.</summary>
		public long ClinicNum;
		

		public FormEClipboardImagePicker() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEClipboardImagePicker_Load(object sender,EventArgs e) { 
			//Get all 'EClipboardImageCapture' defcat definitions. These are the images that patients may be prompted to submit when checking in via eClipboard.
			_listDefsAllEClipboardImages=Defs.GetDefsForCategory(DefCat.EClipboardImageCapture);
			ListEClipboardImageCaptureDefsSelected=new List<EClipboardImageCaptureDef>();
			gridImages.SelectionMode=GridSelectionMode.MultiExtended;
			FillGrid();
			gridImages.SetAll(true);
		}

		///<summary>Fills grid on this form with Defs. After clicking OK, all selected Defs will be converted to EClipboardImageCaptureDefs.</summary>
		private void FillGrid() { 
			//Since 'EClipboardAllowSelfPortraitOnCheckIn' pref is now edited in this form, we have to treat the self-portrait as a def so that it behaves like the other
			//'eClipboard Images' definitions in the form. So we create this mock def for the self-portrait. This def should NOT be inserted into the DB.
			//By default, this Def's DefNum will be 0. This will indicate this is the self-portrait def since it is not in the DB and does not have a primary key.
			Def defSelfPortrait=new Def();
			//Set the name and the description of the self-portrait.
			defSelfPortrait.ItemName="Self Portrait";
			defSelfPortrait.ItemValue="Allows patient to submit a self-portrait upon checkin";
			//If the eClipboard image def has a corresponding EClipboardImageCaptureDef, users are allowed to submit that image, so we add these defs to the 'In Use' list.
			List<Def> listDefsEClipboardImagesInUse=_listDefsAllEClipboardImages.FindAll(x => ListEClipboardImageCaptureDefs.Any(y => y.ClinicNum==ClinicNum && y.DefNum==x.DefNum));
			//If the eClipboard image def does not have a corresponding EClipboardImageCaptureDef, users are not allowed to submit that image, so we add these defs to
			//the 'available' list as long the def is not marked as 'hidden'.
			List<Def> listDefsAvailableEClipboardImages=_listDefsAllEClipboardImages.FindAll(x => !listDefsEClipboardImagesInUse.Any(y => y.DefNum==x.DefNum) && !x.IsHidden);
			bool isSelfPortraitInUse=ListEClipboardImageCaptureDefs.Any(x => x.IsSelfPortrait && x.ClinicNum==ClinicNum);
			List<Def> listDefsImageCat=Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true);//Get the defs for the ImageCats DefCat.
			long defNumPatientPictures=listDefsImageCat.Find(x => x.ItemValue.Contains("P"))?.DefNum??0;//Zero here indicates no Image Category for Patient Picture usage.
			if(defNumPatientPictures>0){//No Image Category marked for PatientPictures, self portrait won't be displayed as an option to the user.
				labelImageCategoryWarning.Visible=false;
				if(!isSelfPortraitInUse){//If self portrait is not currently in use.
					listDefsAvailableEClipboardImages.Add(defSelfPortrait);
				}
			}
			gridImages.BeginUpdate();
			gridImages.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g(this,"Definition"),120);
			gridImages.Columns.Add(col);
			col=new GridColumn(Lan.g(this,"ItemValue"),240);
			gridImages.Columns.Add(col);
			gridImages.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listDefsAvailableEClipboardImages.Count;i++) {
				Def def=listDefsAvailableEClipboardImages[i];
				row=new GridRow();
				row.Cells.Add(def.ItemName);
				row.Cells.Add(def.ItemValue);
				row.Tag=def;
				gridImages.ListGridRows.Add(row);
			}
			gridImages.EndUpdate();
		}

		private void butOK_Click(object sender,EventArgs e) {
			bool isSelfPortrait=false;
			List<Def> listDefsSelected=gridImages.SelectedTags<Def>();
			if(listDefsSelected.Count==0){
				DialogResult=DialogResult.Cancel;
				return;
			}
			for(int i=0;i<listDefsSelected.Count;i++){
				if(listDefsSelected[i].DefNum==0){
					//If DefNum is 0, then this def is the dummy def we created for the self-portrait. So we need to mark this EClipboardImageCaptureDef as being the self-portrait.
					isSelfPortrait=true;
					//We will also set the defnum to the image category that has the patient pictures usage, if any.
					List<Def> listDefsImageCat=Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true);//Get the defs for the ImageCats DefCat
					listDefsSelected[i].DefNum=listDefsImageCat.Find(x => x.ItemValue.Contains("P")).DefNum;//Use the DefNum of the image category for patient pictures. Won't be null here.
				}
				EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
				eClipboardImageCaptureDef.DefNum=listDefsSelected[i].DefNum;
				eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
				eClipboardImageCaptureDef.FrequencyDays=0;
				eClipboardImageCaptureDef.ClinicNum=ClinicNum;
				ListEClipboardImageCaptureDefsSelected.Add(eClipboardImageCaptureDef);
			}
			DialogResult=DialogResult.OK;
		}

		private void gridImages_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			bool isSelfPortrait=false;
			List<Def> listDefsSelected=gridImages.SelectedTags<Def>();
			if(listDefsSelected.Count>1 || listDefsSelected[0]==null){
				return;
			}
			if(listDefsSelected[0].DefNum==0){
				//If DefNum is 0, then this def is the dummy def we created for the self-portrait. So we need to mark this EClipboardImageCaptureDef as being the self-portrait.
				isSelfPortrait=true;
				//We will also set the defnum to the image category that has the patient pictures usage, if any.
				List<Def> listDefsImageCat=Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true);//Get the defs for the ImageCats DefCat.
				listDefsSelected[0].DefNum=listDefsImageCat.Find(x => x.ItemValue.Contains("P")).DefNum;//Use the DefNum of the image category for patient pictures. Won't be null here.
			}
			EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
			eClipboardImageCaptureDef.DefNum=listDefsSelected[0].DefNum;
			eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
			eClipboardImageCaptureDef.FrequencyDays=0;
			eClipboardImageCaptureDef.ClinicNum=ClinicNum;
			ListEClipboardImageCaptureDefsSelected.Add(eClipboardImageCaptureDef);
			DialogResult=DialogResult.OK;
		}

	}
}