using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	/// <summary></summary>
	public partial class FrmSites:FrmODBase {
		private bool _isChanged;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected siteNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long SiteNumSelected;
		private List<Site> _listSites;

		///<summary></summary>
		public FrmSites()
		{
			InitializeComponent();
			Load+=FrmSites_Load;
			gridMain.CellDoubleClick+=gridMain_CellDoubleClick;
			FormClosing+=FrmSites_FormClosing;
			PreviewKeyDown+=FrmSites_PreviewKeyDown;
		}

		private void FrmSites_Load(object sender, System.EventArgs e) {
			Lang.F(this);
			if(IsSelectionMode){
				if(!Security.IsAuthorized(EnumPermType.Setup,true)){
					butAdd.Visible=false;
				}
			}
			else{
				butOK.Visible=false;
				butNone.Visible=false;
			}
			FillGrid();
			if(SiteNumSelected!=0){
				for(int i=0;i<_listSites.Count;i++){
					if(_listSites[i].SiteNum==SiteNumSelected){
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}
		}

		private void FillGrid(){
			Sites.RefreshCache();
			_listSites=Sites.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			gridMain.Columns.Add(new GridColumn(Lang.g("TableSites","Description"),220));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableSites","PlaceOfService"),142));
			gridMain.Columns.Add(new GridColumn(Lang.g("TableSites","Note"),90){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listSites.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listSites[i].Description);
				row.Cells.Add(_listSites[i].PlaceService.ToString());
				row.Cells.Add(_listSites[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void FrmSites_PreviewKeyDown(object sender,KeyEventArgs e) {
			if(butAdd.IsAltKey(Key.A,e)) {
				butAdd_Click(this,new EventArgs());
			}
			if(butOK.IsAltKey(Key.O,e)) {
				butOK_Click(this,new EventArgs());
			}
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//This button is not visible unless user has appropriate permission for setup.
			FrmSiteEdit frmSiteEdit=new FrmSiteEdit();
			frmSiteEdit.SiteCur=new Site();
			frmSiteEdit.SiteCur.IsNew=true;
			frmSiteEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void gridMain_CellDoubleClick(object sender,GridClickEventArgs e) {
			if(IsSelectionMode){
				SiteNumSelected=_listSites[e.Row].SiteNum;
				IsDialogOK=true;
				return;
			}
			FrmSiteEdit frmSiteEdit=new FrmSiteEdit();
			frmSiteEdit.SiteCur=_listSites[e.Row];
			frmSiteEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			SiteNumSelected=0;
			IsDialogOK=true;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
				SiteNumSelected=0;
			}
			else{
				SiteNumSelected=_listSites[gridMain.GetSelectedIndex()].SiteNum;
			}
			IsDialogOK=true;
		}

		private void FrmSites_FormClosing(object sender,CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

	}
}