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
	public class FormSites:FormODBase {
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butClose;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.GridOD gridMain;
		private bool changed;
		private OpenDental.UI.Button butOK;
		public bool IsSelectionMode;
		private OpenDental.UI.Button butNone;
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

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSites));
			this.label1 = new System.Windows.Forms.Label();
			this.butNone = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butAdd = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 2);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(619, 18);
			this.label1.TabIndex = 11;
			this.label1.Text = "Used to keep track of multiple service locations for mobile clinics";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butNone
			// 
			this.butNone.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.butNone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butNone.Location = new System.Drawing.Point(652, 341);
			this.butNone.Name = "butNone";
			this.butNone.Size = new System.Drawing.Size(75, 24);
			this.butNone.TabIndex = 1;
			this.butNone.Text = "None";
			this.butNone.Click += new System.EventHandler(this.butNone_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(652, 608);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(17, 23);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(619, 639);
			this.gridMain.TabIndex = 4;
			this.gridMain.Title = "Sites";
			this.gridMain.TranslationName = "TableSites";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(652, 23);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 0;
			this.butAdd.Text = "&Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(652, 638);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormSites
			// 
			this.ClientSize = new System.Drawing.Size(739, 674);
			this.Controls.Add(this.butNone);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSites";
			this.ShowInTaskbar = false;
			this.Text = "Sites";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSites_FormClosing);
			this.Load += new System.EventHandler(this.FormSites_Load);
			this.ResumeLayout(false);

		}
		#endregion

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





















