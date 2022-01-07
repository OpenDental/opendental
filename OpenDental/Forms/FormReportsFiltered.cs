using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormReportsFiltered:FormODBase {
		#region Fields Public
		public DisplayReport DisplayReportCur;
		#endregion Fields Public	

		#region Fields Private
		private List<DisplayReport> _listDisplayReports;
		#endregion Fields Private

		#region Constructor
		public FormReportsFiltered() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}
		#endregion Constructor

		#region Methods - Event Handlers	
		private void FormReportsFiltered_Load(object sender,EventArgs e) {
			FillList();
		}

		private void listBoxFavorites_MouseDown(object sender,MouseEventArgs e) {
			int selected=listBoxFavorites.IndexFromPoint(e.Location);
			if(selected==-1) {
				return;
			}
			DisplayReportCur=_listDisplayReports[selected];
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
		#endregion Methods - Event Handlers

		#region Methods - Private
		private void FillList() {
			_listDisplayReports=DisplayReports.GetSubMenuReports();
			if(_listDisplayReports.Count>0) {
				List<long> listReportPermissionFkeys=GroupPermissions.GetPermsForReports()
					.Where(x => Security.CurUser.IsInUserGroup(x.UserGroupNum))
					.Select(x => x.FKey)
					.ToList();
				_listDisplayReports.RemoveAll(x => !listReportPermissionFkeys.Contains(x.DisplayReportNum));//Remove reports user does not have permission for
			}
			for(int i=0;i<_listDisplayReports.Count;i++) {
				listBoxFavorites.Items.Add(_listDisplayReports[i].Description);
			}
		}
		#endregion Methods - Private
	}
}