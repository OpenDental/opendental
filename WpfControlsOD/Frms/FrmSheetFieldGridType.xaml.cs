using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	public partial class FrmSheetFieldGridType:FrmODBase {
		private List<string> _listGridTypes;
		public SheetDef SheetDefCur;
		public string SheetGridTypeSelected;
		///<summary>Typically only used when SheetDefCur is associated to a dynamic layout sheetDef.
		///This dictates if any additional grids can show for current value.</summary>
		public SheetFieldLayoutMode LayoutMode=SheetFieldLayoutMode.Default;

		public FrmSheetFieldGridType() {
			InitializeComponent();
			Load+=FrmSheetFieldGridType_Load;
			PreviewKeyDown+=FrmSheetFieldGridType_PreviewKeyDown;
		}

		private void FrmSheetFieldGridType_Load(object sender,EventArgs e) {
			Lang.F(this);
			_listGridTypes=SheetUtil.GetGridsAvailable(SheetDefCur.SheetType,LayoutMode);
			if(_listGridTypes.Count==0) {
				IsDialogOK=false;//should never happen, but in case we launched the grid window without any valid grid types for this sheet type.
			}
			FillComboGridTypes();
		}

		///<summary>Calling function should almost always call FillColumnNames() after this because the selected gridType may have just been changed.</summary>
		private void FillComboGridTypes() {
			comboGridType.Items.Clear();
			for(int i=0;i<_listGridTypes.Count;i++) {
				comboGridType.Items.Add(_listGridTypes[i]);
			}
			comboGridType.SelectedIndex=0;
		}

		private void FrmSheetFieldGridType_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			SheetGridTypeSelected=_listGridTypes[comboGridType.SelectedIndex];
			IsDialogOK=true;
		}

	}
}