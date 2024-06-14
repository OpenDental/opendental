using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormUserPrefAdditional:FormODBase {
		/// <summary>This is a list of providerclinic rows that were given to this form, containing any modifications that were made while in FormProvAdditional.</summary>
		public List<UserOdPref> ListUserOdPrefsOut=new List<UserOdPref>();
		private Userod _userod;
		private List<UserOdPref> _listUserOdPrefs;

		public FormUserPrefAdditional(List<UserOdPref> listUserOdPrefs,Userod userOd) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listUserOdPrefs=listUserOdPrefs.Select(x => x.Clone()).ToList();
			_userod=userOd.Copy();
		}

		private void FormProvAdditional_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid() {
			Cursor=Cursors.WaitCursor;
			gridUserProperties.BeginUpdate();
			gridUserProperties.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableUserPrefProperties","Clinic"),120);
			gridUserProperties.Columns.Add(col);
			col=new GridColumn(Lan.g("TableUserPrefProperties","DoseSpot User ID"),120,true);
			gridUserProperties.Columns.Add(col);
			gridUserProperties.ListGridRows.Clear();
			GridRow row;
			UserOdPref userOdPrefDefault=_listUserOdPrefs.Find(x => x.ClinicNum==0);
			//Doesn't exist in Db, create one
			if(userOdPrefDefault==null) {
				userOdPrefDefault=UserOdPrefs.GetByCompositeKey(_userod.UserNum,Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program,0);
				//Doesn't exist in db, add to list to be synced later
				_listUserOdPrefs.Add(userOdPrefDefault);
			}
			row=new GridRow();
			row.Cells.Add("Default");
			row.Cells.Add(userOdPrefDefault.ValueString);
			row.Tag=userOdPrefDefault;
			gridUserProperties.ListGridRows.Add(row);
			List<Clinic> listClinics = Clinics.GetForUserod(Security.CurUser);
			for(int i = 0;i<listClinics.Count;i++) {
				row=new GridRow();
				UserOdPref userOdPref=_listUserOdPrefs.Find(x => x.ClinicNum==listClinics[i].ClinicNum);
				//wasn't in list, check Db and create a new one if needed
				if(userOdPref==null) {
					userOdPref=UserOdPrefs.GetByCompositeKey(_userod.UserNum,Programs.GetCur(ProgramName.eRx).ProgramNum,UserOdFkeyType.Program,listClinics[i].ClinicNum);
					//Doesn't exist in db, add to list to be synced later
					_listUserOdPrefs.Add(userOdPref);
				}
				row.Cells.Add(listClinics[i].Abbr);
				row.Cells.Add(userOdPref.ValueString);
				row.Tag=userOdPref;
				gridUserProperties.ListGridRows.Add(row);
			}
			gridUserProperties.EndUpdate();
			Cursor=Cursors.Default;
		}

		private void gridProvProperties_CellLeave(object sender,ODGridClickEventArgs e) {
			string newDoseSpotID=PIn.String(gridUserProperties.ListGridRows[e.Row].Cells[e.Col].Text);
			UserOdPref userOdPref=(UserOdPref)gridUserProperties.ListGridRows[e.Row].Tag;
			userOdPref.ValueString=newDoseSpotID;
		}

		private void butOK_Click(object sender,EventArgs e) {
			ListUserOdPrefsOut=new List<UserOdPref>();
			for(int i = 0;i<gridUserProperties.ListGridRows.Count;i++) {
				ListUserOdPrefsOut.Add((UserOdPref)gridUserProperties.ListGridRows[i].Tag);
			}
			DialogResult=DialogResult.OK;
		}

	}
}