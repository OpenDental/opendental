using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormBugSearch:FormODBase {
		private List<Bug> _listBugsAll=null;
		private List<MobileBug> _listMobileBugsAll=null;
		private Job _jobCur;
		public Bug BugCur;
		public MobileBug MobileBugCur;
		private bool _isLoading;

		public FormBugSearch(Job jobCur) {
			InitializeComponent();
			InitializeLayoutManager();
			_jobCur=jobCur;
			Lan.F(this);
		}

		private void FormBugSearch_Load(object sender,EventArgs e) {
			_isLoading=true;
			LoadDataAsync();
		}

		private void LoadDataAsync() {
			ODThread thread=new ODThread((o) => {
				_listBugsAll=Bugs.GetAll();
				_listMobileBugsAll=MobileBugs.GetAll();
				this.BeginInvoke((Action)(FillGridMain));
			});
			thread.AddExceptionHandler((ex) => {
				try {
					this.BeginInvoke((Action)(() => {
						MessageBox.Show(ex.Message);
					}));
				}
				catch { }
			});
			thread.Start(true);
		}

		private void FillGridMain() {
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			if(checkMobile.Checked) {
				gridMain.ListGridColumns.Add(new GridColumn("BugId",50));
				gridMain.ListGridColumns.Add(new GridColumn("Date",65) { SortingStrategy=GridSortingStrategy.DateParse });
				gridMain.ListGridColumns.Add(new GridColumn("Status",60) { TextAlign=HorizontalAlignment.Center });
				gridMain.ListGridColumns.Add(new GridColumn("OD Found",75));
				gridMain.ListGridColumns.Add(new GridColumn("OD Fixed",75));
				gridMain.ListGridColumns.Add(new GridColumn("Platform",75));
				gridMain.ListGridColumns.Add(new GridColumn("Description",50));
			}
			else {
				gridMain.ListGridColumns.Add(new GridColumn("BugId",50));
				gridMain.ListGridColumns.Add(new GridColumn("Date",75) { SortingStrategy=GridSortingStrategy.DateParse });
				gridMain.ListGridColumns.Add(new GridColumn("Status",75) { TextAlign=HorizontalAlignment.Center });
				gridMain.ListGridColumns.Add(new GridColumn("Pri",50) { TextAlign=HorizontalAlignment.Center });
				gridMain.ListGridColumns.Add(new GridColumn("Vers. Found",75));
				gridMain.ListGridColumns.Add(new GridColumn("Vers. Fixed",75));
				gridMain.ListGridColumns.Add(new GridColumn("Description",50));
			}
			gridMain.ListGridRows.Clear();
			if(_listBugsAll==null && _listMobileBugsAll==null) {
				gridMain.EndUpdate();
				return;//have not returned from DB yet OR no filter set.
			}
			List<string> searchTokens=new List<string>();
			if(checkToken.Checked) {
				searchTokens=textFilter.Text.ToLower().Split(' ').ToList();
			}
			else {
				searchTokens.Add(textFilter.Text.ToLower());
			}
			if(checkMobile.Checked) {
				//listMobileBugsFiltered contains any row that contains all tokens (tokens can appear in any column)
				List<MobileBug> listMobileBugsFiltered=_listMobileBugsAll.FindAll(x => (checkShow.Checked || x.BugStatus==MobileBugStatus.Found)
					&& searchTokens.All(y => x.Description.ToLower().Contains(y) || x.ODVersionsFound.ToLower().Contains(y) || x.ODVersionsFixed.ToLower().Contains(y)
							|| x.Platforms.ToString().ToLower().Contains(y)));
				foreach(MobileBug mobileBug in listMobileBugsFiltered) {
					GridRow row=new GridRow();
					row.Cells.Add(mobileBug.MobileBugNum.ToString());
					row.Cells.Add(mobileBug.DateTimeCreated.ToShortDateString());
					row.Cells.Add(mobileBug.BugStatus.ToString());
					row.Cells.Add(mobileBug.ODVersionsFound.Replace(";","\r\n"));
					row.Cells.Add(mobileBug.ODVersionsFixed.Replace(";","\r\n"));
					row.Cells.Add(mobileBug.Platforms.ToString());
					row.Cells.Add(mobileBug.Description);
					row.Tag=mobileBug;
					gridMain.ListGridRows.Add(row);
					if(MobileBugCur!=null && mobileBug.MobileBugNum==MobileBugCur.MobileBugNum) {
						gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
					}
				}
			}
			else {
				//listBugsFiltered contains any row that contains all tokens (tokens can appear in any column)
				List<Bug> listBugsFiltered=_listBugsAll.FindAll(x => (checkShow.Checked || new[]{BugStatus.Verified,BugStatus.Accepted}.Contains(x.Status_))
					&& searchTokens.All(y => x.Description.ToLower().Contains(y) || x.VersionsFound.ToLower().Contains(y) || x.VersionsFixed.ToLower().Contains(y)));
				foreach(Bug bug in listBugsFiltered) {
					GridRow row=new GridRow();
					row.Cells.Add(bug.BugId.ToString());
					row.Cells.Add(bug.CreationDate.ToShortDateString());
					row.Cells.Add(bug.Status_.ToString());
					row.Cells.Add(bug.PriorityLevel.ToString());
					row.Cells.Add(bug.VersionsFound.Replace(";","\r\n"));
					row.Cells.Add(bug.VersionsFixed.Replace(";","\r\n"));
					row.Cells.Add(bug.Description);
					row.Tag=bug;
					gridMain.ListGridRows.Add(row);
					if(BugCur!=null && bug.BugId==BugCur.BugId) {
						gridMain.SetSelected(gridMain.ListGridRows.Count-1,true);
					}
				}
			}
			gridMain.EndUpdate();
			_isLoading=false;
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(e.Row>=gridMain.ListGridRows.Count) {
				return;
			}
			if(gridMain.ListGridRows[e.Row].Tag.GetType()==typeof(Bug)) {
				MobileBugCur=null;
				BugCur=(Bug)gridMain.ListGridRows[e.Row].Tag;
			}
			else {
				BugCur=null;
				MobileBugCur=(MobileBug)gridMain.ListGridRows[e.Row].Tag;
			}
		}

		private void textFilter_TextChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			FillGridMain();
		}

		private void checkToken_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			FillGridMain();
		}

		private void checkShow_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			FillGridMain();
		}

		private void checkMobile_CheckedChanged(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			FillGridMain();
		}

		private void butRefresh_Click(object sender,EventArgs e) {
			_isLoading=true;
			LoadDataAsync();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isLoading) {
				return;
			}
			if(gridMain.ListGridRows[e.Row].Tag.GetType()==typeof(Bug)) {
				MobileBugCur=null;
				BugCur=(Bug)gridMain.ListGridRows[e.Row].Tag;
			}
			else {
				BugCur=null;
				MobileBugCur=(MobileBug)gridMain.ListGridRows[e.Row].Tag;
			}
			DialogResult=DialogResult.OK;
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			using FormBugEdit FormBE=new FormBugEdit();
			FormBE.IsNew=true;
			FormBE.BugCur=Bugs.GetNewBugForUser();
			if(_jobCur.Category==JobCategory.Enhancement) {
				FormBE.BugCur.Description="(Enhancement)";
			}
			FormBE.BugCur.Description+=_jobCur.Title;
			FormBE.ShowDialog();
			if(FormBE.DialogResult==DialogResult.OK) {
				MobileBugCur=null;
				BugCur=FormBE.BugCur;
				DialogResult=DialogResult.OK;
			}
		}

		private void butAddMobile_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			using FormMobileBugEdit formMobileBugEdit=new FormMobileBugEdit();
			formMobileBugEdit.IsNew=true;
			formMobileBugEdit.MobileBugCur=MobileBugs.GetNewBugForUser();
			if(_jobCur.Category==JobCategory.Enhancement) {
				formMobileBugEdit.MobileBugCur.Description="(Enhancement)";
			}
			formMobileBugEdit.MobileBugCur.Description+=_jobCur.Title;
			formMobileBugEdit.ShowDialog();
			if(formMobileBugEdit.DialogResult==DialogResult.OK) {
				BugCur=null;
				MobileBugCur=formMobileBugEdit.MobileBugCur;
				DialogResult=DialogResult.OK;
			}
		}

		private void butViewSubs_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			using FormBugSubmissions FormBugSubs=new FormBugSubmissions(_jobCur);
			if(FormBugSubs.ShowDialog()==DialogResult.OK && FormBugSubs.BugCur!=null) {//Bug was created.
				MobileBugCur=null;
				BugCur=FormBugSubs.BugCur;
				DialogResult=DialogResult.OK;
			}
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(_isLoading) {
				return;
			}
			if(BugCur==null && MobileBugCur==null) {
				MsgBox.Show(this,"Select a bug first.");
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			BugCur=null;
			MobileBugCur=null;
			DialogResult=DialogResult.Cancel;
		}
	}



}