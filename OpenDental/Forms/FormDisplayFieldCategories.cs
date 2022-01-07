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
		//private bool changed;
		private bool _isCemtMode;
		//private List<DisplayField> ListShowing;
		//private List<DisplayField> ListAvailable;

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
			foreach(DisplayFieldCategory cat in Enum.GetValues(typeof(DisplayFieldCategory))
				.OfType<DisplayFieldCategory>().OrderBy(x => x.GetDescription())) 
			{
				if(cat == DisplayFieldCategory.None) {//skip None because user not allowed to select that
					continue;
				}
				bool displayIsCemtOnly=EnumTools.GetAttributeOrDefault<PermissionAttribute>(cat).IsCEMT;
				if(_isCemtMode != displayIsCemtOnly) {
					continue;
				}
				if(cat == DisplayFieldCategory.OrthoChart) { //orthochart tabs can have their own name.
					listCategory.Items.Add(OrthoChartTabs.GetFirst(true).TabName,cat);
					continue;
				}
				listCategory.Items.Add(Lan.g("enumDisplayFieldCategory",cat.GetDescription()),cat);
			}
			listCategory.SelectedIndex=0;
		}

		private void listCategory_DoubleClick(object sender,EventArgs e) {
			ShowCategoryEdit();
			Close();
		}

		private void ShowCategoryEdit() {
			DisplayFieldCategory selectedCategory = listCategory.GetSelected<DisplayFieldCategory>();
			if(selectedCategory==DisplayFieldCategory.None) {//should never happen.
				return;
			}
			//The ortho chart is a more complicated display field so it has its own window.
			if(selectedCategory==DisplayFieldCategory.OrthoChart) {
				using FormDisplayFieldsOrthoChart FormDFOC=new FormDisplayFieldsOrthoChart();
				FormDFOC.ShowDialog();
			}
			else {//All other display fields use the base display fields window.
				using FormDisplayFields FormF=new FormDisplayFields();
				FormF.Category=selectedCategory;
				FormF.ShowDialog();
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			ShowCategoryEdit();
			Close();
		}

		

		

		

		

		

		

		


	}
}





















