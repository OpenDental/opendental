using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmAlertCategoryEdit:FrmODBase {
		private List<AlertType> _listAlertTypesShown;
		private List<AlertCategoryLink> _listAlertCategoryLinksOld;//New list generated on OK click.
		private AlertCategory _alertCategory;

		public FrmAlertCategoryEdit(AlertCategory alertCategory) {
			InitializeComponent();
			_alertCategory=alertCategory;
			Load+=FrmAlertCategoryEdit_Load;
			listBoxAlertTypes.MouseDown+=listBoxAlertTypes_MouseDown;
			PreviewKeyDown+=FrmAlertCategoryEdit_PreviewKeyDown;
		}
		
		private void FrmAlertCategoryEdit_Load(object sender,EventArgs e) {
			Lang.F(this);
			textDesc.Text=_alertCategory.Description;
			_listAlertTypesShown=Enum.GetValues(typeof(AlertType)).OfType<AlertType>().ToList();
			_listAlertTypesShown.RemoveAll(x => !PrefC.IsODHQ && GenericTools.IsODHQ(x));
			if(_alertCategory.IsHQCategory) {
				textDesc.IsEnabled=false;
				butDelete.IsEnabled=false;
				butSave.IsEnabled=false;
			}
			_listAlertCategoryLinksOld=AlertCategoryLinks.GetForCategory(_alertCategory.AlertCategoryNum);
			InitAlertTypeSelections();
		}

		private void InitAlertTypeSelections() {
			listBoxAlertTypes.Items.Clear();
			List<AlertType> listAlertTypes=_listAlertCategoryLinksOld.Select(x => x.AlertType).ToList();
			for(int i=0;i<_listAlertTypesShown.Count;i++) {
				listBoxAlertTypes.Items.Add(Lans.g(this,_listAlertTypesShown[i].GetDescription()));
				int index=listBoxAlertTypes.Items.Count-1;
				listBoxAlertTypes.SetSelected(index,listAlertTypes.Contains(_listAlertTypesShown[i]));
			}
		}

		private void listBoxAlertTypes_MouseDown(object sender,MouseButtonEventArgs e) {
			if(_alertCategory.IsHQCategory) {
				InitAlertTypeSelections();
				MsgBox.Show(this,"You can only edit custom alert categories.");
			}
		}

		private void FrmAlertCategoryEdit_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butSave.IsAltKey(Key.S,e)) {
				butSave_Click(this,new EventArgs());
			}
		}

		private void butSave_Click(object sender,EventArgs e) {
			_alertCategory.Description=textDesc.Text;
			List<AlertType> listAlertTypesSelected=listBoxAlertTypes.SelectedIndices
				.OfType<int>()
				.Select(x => (AlertType)_listAlertTypesShown[x])
				.ToList();
			List<AlertCategoryLink> listAlertCategoryLinksNew=_listAlertCategoryLinksOld.Select(x => x.Copy()).ToList();
			listAlertCategoryLinksNew.RemoveAll(x => !listAlertTypesSelected.Contains(x.AlertType));//Remove unselected AlertTypes
			for(int i=0;i<listAlertTypesSelected.Count;i++) {
				if(!_listAlertCategoryLinksOld.Exists(x => x.AlertType==listAlertTypesSelected[i])) {//Add newly selected AlertTypes.
					listAlertCategoryLinksNew.Add(new AlertCategoryLink(_alertCategory.AlertCategoryNum,listAlertTypesSelected[i]));
				}
			}
			AlertCategoryLinks.Sync(listAlertCategoryLinksNew,_listAlertCategoryLinksOld);
			AlertCategories.Update(_alertCategory);
			DataValid.SetInvalid(InvalidType.AlertCategoryLinks,InvalidType.AlertCategories);
			IsDialogOK=true;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you would like to delete this?")) {
				return;
			}
			AlertCategoryLinks.DeleteForCategory(_alertCategory.AlertCategoryNum);
			AlertCategories.Delete(_alertCategory.AlertCategoryNum);
			DataValid.SetInvalid(InvalidType.AlertCategories,InvalidType.AlertCategories);
			IsDialogOK=true;
		}

	}
}