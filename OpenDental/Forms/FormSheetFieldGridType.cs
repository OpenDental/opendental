using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetFieldGridType:FormODBase {
		private List<string> listGridTypes;
		public SheetDef SheetDefCur;
		public string SelectedSheetGridType;
		///<summary>Typically only used when SheetDefCur is associated to a dynamic layout sheetDef.
		///This dictates if any additional grids can show for current value.</summary>
		public SheetFieldLayoutMode LayoutMode=SheetFieldLayoutMode.Default;

		public FormSheetFieldGridType() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldGrid_Load(object sender,EventArgs e) {
			listGridTypes=SheetUtil.GetGridsAvailable(SheetDefCur.SheetType,LayoutMode);
			if(listGridTypes.Count==0) {
				DialogResult=DialogResult.Cancel;//should never happen, but in case we launched the grid window without any valid grid types for this sheet type.
			}
			fillComboGridTypes();
		}

		///<summary>Calling function should almost always call fillColumnNames() after this because the selected gridType may have just been changed.</summary>
		private void fillComboGridTypes() {
			comboGridType.Items.Clear();
			for(int i=0;i<listGridTypes.Count;i++) {
				comboGridType.Items.Add(listGridTypes[i]);
			}
			comboGridType.SelectedIndex=0;
		}

		private void butOK_Click(object sender,EventArgs e) {
			SelectedSheetGridType=listGridTypes[comboGridType.SelectedIndex];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}