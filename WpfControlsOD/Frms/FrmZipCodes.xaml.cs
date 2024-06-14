using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	///<summary></summary>
	public partial class FrmZipCodes:FrmODBase {

		private bool _isChanged;
		private List<ZipCode> _listZipCodes;

		///<summary></summary>
		public FrmZipCodes() {
			InitializeComponent();
			Load+=FrmZipCodes_Load;
			gridZipCode.CellDoubleClick+=gridZipCode_CellDoubleClick;
			PreviewKeyDown+=FrmZipCodes_PreviewKeyDown;
			FormClosing+=FrmZipCodes_Closing;
		}

		private void FrmZipCodes_Load(object sender, EventArgs e) {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				this.Text=Lang.g(this,"Postal Codes");
				gridZipCode.Title=Lang.g(this,"Postal Codes");
			}
			Lang.F(this);
		  FillGrid();
		}

		private void FillGrid(){
			ZipCodes.RefreshCache();
			_listZipCodes=ZipCodes.GetDeepCopy();
			gridZipCode.BeginUpdate();
			gridZipCode.Columns.Clear();
			GridColumn gridColumn;
			gridColumn=new GridColumn(Lans.g(this,"ZipCode"),75);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				gridColumn=new GridColumn(Lans.g(this,"Postal"),75);
			}
			gridZipCode.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(this,"City"),270);
			gridZipCode.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(this,"State"),80);
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {//Canadian. en-CA or fr-CA
				gridColumn=new GridColumn(Lans.g(this,"Prov"),80);
			}
			gridZipCode.Columns.Add(gridColumn);
			gridColumn=new GridColumn(Lans.g(this,"Frequent"),0);
			gridColumn.IsWidthDynamic=true;
			gridZipCode.Columns.Add(gridColumn);
			gridZipCode.ListGridRows.Clear();
			for(int i=0;i<_listZipCodes.Count;i++) {
				GridRow gridRow=new GridRow();
				gridRow.Cells.Add(_listZipCodes[i].ZipCodeDigits);
				gridRow.Cells.Add(_listZipCodes[i].City);
				gridRow.Cells.Add(_listZipCodes[i].State);
				gridRow.Cells.Add((_listZipCodes[i].IsFrequent ? "X" : ""));
				gridRow.Tag=_listZipCodes[i];
				gridZipCode.ListGridRows.Add(gridRow);
			}
			gridZipCode.EndUpdate();
		}

		private void gridZipCode_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(gridZipCode.SelectedIndices.Count()==0) {
				return;
			}
			FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
			frmZipCodeEdit.ZipCodeCur=(ZipCode)gridZipCode.ListGridRows[e.Row].Tag;
			frmZipCodeEdit.ShowDialog();
			if(!frmZipCodeEdit.IsDialogOK) {
				return;
			}
			_isChanged=true;
			FillGrid();
		}

		private void FrmZipCodes_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butAdd.IsAltKey(Key.A,e)) {
				butAdd_Click(this,new EventArgs());
			}
			if(butDelete.IsAltKey(Key.D,e)) {
				butDelete_Click(this,new EventArgs());
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(gridZipCode.SelectedIndices.Count()==0) {
				MsgBox.Show(this,"Please select an item first.");
				return;
			}
			ZipCode zipCode = gridZipCode.SelectedTag<ZipCode>();
			if(!MsgBox.Show(MsgBoxButtons.OKCancel,Lans.g(this,"Delete Zipcode?"),"")) {
				return;
			}
			_isChanged=true;
			ZipCodes.Delete(zipCode);
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			FrmZipCodeEdit frmZipCodeEdit=new FrmZipCodeEdit();
			frmZipCodeEdit.ZipCodeCur=new ZipCode();
			frmZipCodeEdit.IsNew=true;
			frmZipCodeEdit.ShowDialog();
			if(!frmZipCodeEdit.IsDialogOK){
				return;
			}
			_isChanged=true;
			FillGrid();
		}

		private void FrmZipCodes_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.ZipCodes);
			}
		}
	
	}
}