using System;
using System.Linq;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormDisplayFieldCategories:FormODBase {
		private bool _isCemtMode;

		///<summary></summary>
		public FormDisplayFieldCategories(bool isCemtMode=false)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isCemtMode=isCemtMode;
		}

		private void FormDisplayFields_Load(object sender,EventArgs e) {
			//Alphabetical order.  When new display fields are added this will need to be changed.
			List<DisplayFieldCategory> listDisplayFieldCategories=
				Enum.GetValues(typeof(DisplayFieldCategory)).OfType<DisplayFieldCategory>().OrderBy(x=>x.GetDescription()).ToList();
			for(int i=0;i<listDisplayFieldCategories.Count;i++) {
				if(listDisplayFieldCategories[i]==DisplayFieldCategory.None) {//skip None because user not allowed to select that
					continue;
				}
				bool isDisplayCemtOnly=EnumTools.GetAttributeOrDefault<PermissionAttribute>(listDisplayFieldCategories[i]).IsCEMT;
				if(_isCemtMode!=isDisplayCemtOnly) {
					continue;
				}
				if(listDisplayFieldCategories[i]==DisplayFieldCategory.OrthoChart) { //orthochart tabs can have their own name.
					listCategory.Items.Add(OrthoChartTabs.GetFirst(isShort:true).TabName,listDisplayFieldCategories[i]);
					continue;
				}
				listCategory.Items.Add(Lan.g("enumDisplayFieldCategory",listDisplayFieldCategories[i].GetDescription()),listDisplayFieldCategories[i]);
			}
			listCategory.SelectedIndex=0;
		}

		private void listCategory_DoubleClick(object sender,EventArgs e) {
			ShowCategoryEdit();
			Close();
		}

		private void ShowCategoryEdit() {
			DisplayFieldCategory displayFieldCategorySelected=listCategory.GetSelected<DisplayFieldCategory>();
			if(displayFieldCategorySelected==DisplayFieldCategory.None) {//should never happen.
				return;
			}
			//The ortho chart is a more complicated display field so it has its own window.
			if(displayFieldCategorySelected==DisplayFieldCategory.OrthoChart) {
				using FormDisplayFieldsOrthoChart formDisplayFieldsOrthoChart=new FormDisplayFieldsOrthoChart();
				formDisplayFieldsOrthoChart.ShowDialog();
			}
			else {//All other display fields use the base display fields window.
				using FormDisplayFields formDisplayFields=new FormDisplayFields();
				formDisplayFields.DisplayFieldCategoryCur=displayFieldCategorySelected;
				formDisplayFields.ShowDialog();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ShowCategoryEdit();
			Close();
		}

		

		

		

		

		

		

		


	}
}





















