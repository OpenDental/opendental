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
		///<summay>Currently selected clinic for which we are editing the eclipboard image capture defs.</summay>
		private long _clinicNum;
		///<summary>List of all definitions in 'EClipboardImageCapture' defcat. </summary>
		private List<Def> _listDefsAllEClipboardImages;
		///<summary>The corresponding EClipboardImageCaptureDef objects for the list of EClipboard Images patients are allowed to take. Must be
		///set before opening this form.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefs;
		///<summary>After clicking OK, this is the list of selected EClipboardImageCaptureDef.</summary>
		public List<EClipboardImageCaptureDef> ListEClipboardImageCaptureDefsSelected;
		

		public FormEClipboardImagePicker(long clinicNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_clinicNum=clinicNum;
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
			List<Def> listDefsEClipboardImagesInUse=_listDefsAllEClipboardImages.FindAll(x => ListEClipboardImageCaptureDefs.Any(y => y.ClinicNum==_clinicNum && y.DefNum==x.DefNum));
			//If the eClipboard image def does not have a corresponding EClipboardImageCaptureDef, users are not allowed to submit that image, so we add these defs to
			//the 'available' list as long the def is not marked as 'hidden'.
			List<Def> listDefsAvailableEClipboardImages=_listDefsAllEClipboardImages.FindAll(x => !listDefsEClipboardImagesInUse.Any(y => y.DefNum==x.DefNum) && !x.IsHidden);
			bool isSelfPortraitAllowed=ListEClipboardImageCaptureDefs.Any(x => x.IsSelfPortrait && x.ClinicNum==_clinicNum);
			//If the self portrait has a correspoding EClipboardImageCaptureDef, then we add it the 'In Use' list, since users are able to submit self portraits.
			if(isSelfPortraitAllowed) { 
				listDefsEClipboardImagesInUse.Add(defSelfPortrait);
			}
			else { //Otherwise we add it to the available list since patients are not currently allowed to submit self-portraits. 
				listDefsAvailableEClipboardImages.Add(defSelfPortrait);
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
					List<Def> listDefsImageCat=OpenDentBusiness.Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true);//Get the defs from the ImageCats DefCat
					listDefsSelected[i].DefNum=listDefsImageCat.FirstOrDefault(x => x.ItemValue.Contains("P"))?.DefNum??0;//If there's an image category for patient pictues, use that, otherwise use 0.
				}
				EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
				eClipboardImageCaptureDef.DefNum=listDefsSelected[i].DefNum;
				eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
				eClipboardImageCaptureDef.FrequencyDays=0;
				eClipboardImageCaptureDef.ClinicNum=_clinicNum;
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
				List<Def> listDefsImageCat=OpenDentBusiness.Defs.GetDefsForCategory(OpenDentBusiness.DefCat.ImageCats,true);//Get the defs for the ImageCats DefCat.
				listDefsSelected[0].DefNum=listDefsImageCat.Find(x => x.ItemValue.Contains("P"))?.DefNum??0;//If there's an image category for patient pictures, use that, otherwise use 0.
			}
			EClipboardImageCaptureDef eClipboardImageCaptureDef=new EClipboardImageCaptureDef();
			eClipboardImageCaptureDef.DefNum=listDefsSelected[0].DefNum;
			eClipboardImageCaptureDef.IsSelfPortrait=isSelfPortrait;
			eClipboardImageCaptureDef.FrequencyDays=0;
			eClipboardImageCaptureDef.ClinicNum=_clinicNum;
			ListEClipboardImageCaptureDefsSelected.Add(eClipboardImageCaptureDef);
			DialogResult=DialogResult.OK;
		}

	}
}