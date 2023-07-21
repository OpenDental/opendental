using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class UserControlImagingGeneral:UserControl {

		#region Fields - Private
		///<summary>Helps store and set the default Image Category to import to in the Image Module.</summary>
		private long _imageCategoryDefault;
		private long _videoImageCategoryDefault;
		#endregion Fields - Private

		#region Fields - Public
		public bool Changed;
		public bool DoSwapImagingModule;
		#endregion Fields - Public

		#region Constructors
		public UserControlImagingGeneral() {
			InitializeComponent();
			Font=LayoutManagerForms.FontInitial;
		}
		#endregion Constructors

		#region Methods - Event Handlers
		private void butBrowseImportFolder_Click(object sender,EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textDefaultImageImportFolder.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textDefaultImageImportFolder.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseAutoImportFolder_Click(object sender,EventArgs e) {
			using FolderBrowserDialog folderBrowserDialog=new FolderBrowserDialog();
			folderBrowserDialog.SelectedPath=textAutoImportFolder.Text;
			if(folderBrowserDialog.ShowDialog()==DialogResult.Cancel){
				return;
			}
			textAutoImportFolder.Text=ODFileUtils.CombinePaths(folderBrowserDialog.SelectedPath,"");//Add trail slash.
		}

		private void butBrowseImageCategoryDefault_Click(object sender,EventArgs e) {
			List<Def> listDefs=Defs.GetDefs(DefCat.ImageCats,new List<long>(){ _imageCategoryDefault });
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ImageCats,listDefs);
			formDefinitionPicker.IsMultiSelectionMode=false;
			if(formDefinitionPicker.ShowDialog()==DialogResult.Cancel){
				return;
			}
			if(formDefinitionPicker.ListDefsSelected.Count==0) {
				_imageCategoryDefault=0;
				textImageCategoryDefault.Text="";
				return;
			}
			Def defSelected=formDefinitionPicker.ListDefsSelected.First();//Guaranteed user selected a Def.
			_imageCategoryDefault=defSelected.DefNum;
			textImageCategoryDefault.Text=defSelected.ItemName;
		}

		private void butVideoImageCategoryDefault_Click(object sender,EventArgs e) {
			List<Def> listDefs=Defs.GetDefs(DefCat.ImageCats,new List<long>(){ _videoImageCategoryDefault });
			using FormDefinitionPicker formDefinitionPicker=new FormDefinitionPicker(DefCat.ImageCats,listDefs);
			formDefinitionPicker.IsMultiSelectionMode=false;
			if(formDefinitionPicker.ShowDialog()==DialogResult.Cancel){
				return;
			}
			if(formDefinitionPicker.ListDefsSelected.Count==0) {
				_videoImageCategoryDefault=0;
				textVideoImageCategoryDefault.Text="";
				return;
			}
			Def defSelected=formDefinitionPicker.ListDefsSelected.First();//Guaranteed user selected a Def.
			_videoImageCategoryDefault=defSelected.DefNum;
			textVideoImageCategoryDefault.Text=defSelected.ItemName;
		}

		private void checkImagesModuleUsesOld2020_Click(object sender,EventArgs e) {
			bool isImageModuleSettingChanged=(PrefC.GetBool(PrefName.ImagesModuleUsesOld2020)!=checkImagesModuleUsesOld2020.Checked);
			if(isImageModuleSettingChanged) {
				MsgBox.Show(this,"The entire program will need to close to reset imaging. All other instances connected to this database should be manually closed before changing this setting.");
			}
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		#endregion Methods - Private

		#region Methods - Public
		public void FillImagingGeneral() {
			textDefaultImageImportFolder.Text=PrefC.GetString(PrefName.DefaultImageImportFolder);
			textAutoImportFolder.Text=PrefC.GetString(PrefName.AutoImportFolder);
			checkPDFLaunchWindow.Checked=PrefC.GetBool(PrefName.PdfLaunchWindow);
			checkImagesModuleUsesOld2020.Checked=PrefC.GetBool(PrefName.ImagesModuleUsesOld2020);
			_imageCategoryDefault=PrefC.GetLong(PrefName.ImageCategoryDefault);
			textImageCategoryDefault.Text=Defs.GetName(DefCat.ImageCats,_imageCategoryDefault);
			_videoImageCategoryDefault=PrefC.GetLong(PrefName.VideoImageCategoryDefault);
			textVideoImageCategoryDefault.Text=Defs.GetName(DefCat.ImageCats,_videoImageCategoryDefault);
			textUnits.Text=MountDefs.GetScaleUnits(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
			textScale.Value=MountDefs.GetScale(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
			textDecimals.Value=MountDefs.GetDecimals(PrefC.GetString(PrefName.ImagingDefaultScaleValue));
		}

		public bool SaveImagingGeneral() {
			if(!textScale.IsValid() || !textDecimals.IsValid()) {
				MsgBox.Show(this,"Please fix data entry errors first.");
				return false;
			}
			Changed|=Prefs.UpdateString(PrefName.DefaultImageImportFolder,textDefaultImageImportFolder.Text);
			Changed|=Prefs.UpdateString(PrefName.AutoImportFolder,textAutoImportFolder.Text);
			Changed|=Prefs.UpdateBool(PrefName.PdfLaunchWindow,checkPDFLaunchWindow.Checked);
			Changed|=Prefs.UpdateLong(PrefName.ImageCategoryDefault,_imageCategoryDefault);
			Changed|=Prefs.UpdateLong(PrefName.VideoImageCategoryDefault,_videoImageCategoryDefault);
			string scaleValue=MountDefs.SetScale((float)textScale.Value,textDecimals.Value,textUnits.Text);
			Changed|=Prefs.UpdateString(PrefName.ImagingDefaultScaleValue,scaleValue);
			if(Prefs.UpdateBool(PrefName.ImagesModuleUsesOld2020,checkImagesModuleUsesOld2020.Checked)) {
				Changed=true;
				DoSwapImagingModule=true;
			}
			return true;
		}
		#endregion Methods - Public
	}
}
