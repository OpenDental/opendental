using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental {
	public partial class FormAlertCategoryEdit:FormODBase {

		private List<AlertType> listShownAlertTypes;
		private List<AlertCategoryLink> listOldAlertCategoryLinks;//New list generated on OK click.
		private AlertCategory _categoryCur;

		public FormAlertCategoryEdit(AlertCategory category) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_categoryCur=category;
		}
		
		private void FormAlertCategoryEdit_Load(object sender,EventArgs e) {
			textDesc.Text=_categoryCur.Description;
			listShownAlertTypes=Enum.GetValues(typeof(AlertType)).OfType<AlertType>().ToList();
			listShownAlertTypes.RemoveAll(x => !PrefC.IsODHQ && GenericTools.IsODHQ(x));
			if(_categoryCur.IsHQCategory) {
				textDesc.Enabled=false;
				butDelete.Enabled=false;
				butOK.Enabled=false;
			}
			listOldAlertCategoryLinks=AlertCategoryLinks.GetForCategory(_categoryCur.AlertCategoryNum);
			InitAlertTypeSelections();
		}

		private void InitAlertTypeSelections() {
			listBoxAlertTypes.Items.Clear();
			List<AlertType> listCategoryAlertTypes=listOldAlertCategoryLinks.Select(x => x.AlertType).ToList();
			foreach(AlertType type in listShownAlertTypes) {
				listBoxAlertTypes.Items.Add(Lans.g(this,type.GetDescription()));
				int index=listBoxAlertTypes.Items.Count-1;
				listBoxAlertTypes.SetSelected(index,listCategoryAlertTypes.Contains(type));
			}
		}

		private void listBoxAlertTypes_MouseClick(object sender,MouseEventArgs e) {
			if(_categoryCur.IsHQCategory) {
				InitAlertTypeSelections();
				MsgBox.Show(this,"You can only edit custom alert categories.");
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			_categoryCur.Description=textDesc.Text;
			List<AlertType> listSelectedTypes=listBoxAlertTypes.SelectedIndices
				.OfType<int>()
				.Select(x => (AlertType)listShownAlertTypes[x])
				.ToList();
			List<AlertCategoryLink> listNewAlertCategoryType=listOldAlertCategoryLinks.Select(x => x.Copy()).ToList();
			listNewAlertCategoryType.RemoveAll(x => !listSelectedTypes.Contains(x.AlertType));//Remove unselected AlertTypes
			foreach(AlertType type in listSelectedTypes) {
				if(!listOldAlertCategoryLinks.Exists(x => x.AlertType==type)) {//Add newly selected AlertTypes.
					listNewAlertCategoryType.Add(new AlertCategoryLink(_categoryCur.AlertCategoryNum,type));
				}
			}
			AlertCategoryLinks.Sync(listNewAlertCategoryType,listOldAlertCategoryLinks);
			AlertCategories.Update(_categoryCur);
			DataValid.SetInvalid(InvalidType.AlertCategoryLinks,InvalidType.AlertCategories);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.YesNo,"Are you sure you would like to delete this?")) {
				return;
			}
			AlertCategoryLinks.DeleteForCategory(_categoryCur.AlertCategoryNum);
			AlertCategories.Delete(_categoryCur.AlertCategoryNum);
			DataValid.SetInvalid(InvalidType.AlertCategories,InvalidType.AlertCategories);
			DialogResult=DialogResult.OK;
		}
	}
}