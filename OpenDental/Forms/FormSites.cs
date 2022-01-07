using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormSites:FormODBase {
		private bool changed;
		public bool IsSelectionMode;
		///<summary>Only used if IsSelectionMode.  On OK, contains selected siteNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		public long SelectedSiteNum;
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
			if(SelectedSiteNum!=0){
				for(int i=0;i<_listSites.Count;i++){
					if(_listSites[i].SiteNum==SelectedSiteNum){
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
			gridMain.ListGridColumns.Clear();
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSites","Description"),220));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSites","PlaceOfService"),142));
			gridMain.ListGridColumns.Add(new GridColumn(Lan.g("TableSites","Note"),90){ IsWidthDynamic=true });
			gridMain.ListGridRows.Clear();
			GridRow row;
			foreach(Site site in _listSites) {
				row=new GridRow();
				row.Cells.Add(site.Description);
				row.Cells.Add(site.PlaceService.ToString());
				row.Cells.Add(site.Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			//This button is not visible unless user has appropriate permission for setup.
			using FormSiteEdit FormS=new FormSiteEdit();
			FormS.SiteCur=new Site();
			FormS.SiteCur.IsNew=true;
			FormS.ShowDialog();
			FillGrid();
			changed=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(IsSelectionMode){
				SelectedSiteNum=_listSites[e.Row].SiteNum;
				DialogResult=DialogResult.OK;
				return;
			}
			else{
				using FormSiteEdit FormS=new FormSiteEdit();
				FormS.SiteCur=_listSites[e.Row];
				FormS.ShowDialog();
				FillGrid();
				changed=true;
			}
		}

		private void butNone_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			SelectedSiteNum=0;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//not even visible unless is selection mode
			if(gridMain.GetSelectedIndex()==-1){
			//	MsgBox.Show(this,"Please select an item first.");
			//	return;
				SelectedSiteNum=0;
			}
			else{
				SelectedSiteNum=_listSites[gridMain.GetSelectedIndex()].SiteNum;
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
			if(changed){
				DataValid.SetInvalid(InvalidType.Sites);
			}
		}

		

		

		

		



		
	}
}





















