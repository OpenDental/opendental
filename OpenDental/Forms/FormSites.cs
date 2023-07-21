using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormSites:FormODBase {
		private bool _isChanged;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected siteNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long SiteNumSelected;
		private List<Site> _listSites;

		///<summary></summary>
		public FormSites()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSites_Load(object sender, System.EventArgs e) {
			if(IsSelectionMode){
				butClose.Text=Lan.g(this,"Cancel");
				if(!Security.IsAuthorized(Permissions.Setup,true)){
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
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSites","Description"),220));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSites","PlaceOfService"),142));
			gridMain.Columns.Add(new GridColumn(Lan.g("TableSites","Note"),90){ IsWidthDynamic=true });
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

		private void butAdd_Click(object sender, System.EventArgs e) {
			//This button is not visible unless user has appropriate permission for setup.
			using FormSiteEdit formSiteEdit=new FormSiteEdit();
			formSiteEdit.SiteCur=new Site();
			formSiteEdit.SiteCur.IsNew=true;
			formSiteEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				SiteNumSelected=_listSites[e.Row].SiteNum;
				DialogResult=DialogResult.OK;
				return;
			}
			using FormSiteEdit formSiteEdit=new FormSiteEdit();
			formSiteEdit.SiteCur=_listSites[e.Row];
			formSiteEdit.ShowDialog();
			FillGrid();
			_isChanged=true;
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			SiteNumSelected=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
			//	MsgBox.Show(this,"Please select an item first.");
			//	return;
				SiteNumSelected=0;
			}
			else{
				SiteNumSelected=_listSites[gridMain.GetSelectedIndex()].SiteNum;
			}
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			if(IsSelectionMode){
				DialogResult=DialogResult.Cancel;
			}
			else{
				Close();
			}
		}

		private void FormSites_FormClosing(object sender,FormClosingEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

		

		

		

		



		
	}
}





















