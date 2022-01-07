using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using OpenDentBusiness.Crud;
using System.Linq;

namespace OpenDental{
///<summary></summary>
	public partial class FormReqNeededs:FormODBase {
		private List<ReqNeeded> _listReqsAll;
		///<summary>Stale deep copy of _listReqsAll to use with sync.</summary>
		private List<ReqNeeded> _listReqsAllOld;
		private List<ReqNeeded> _listReqsInGrid;
		private List<SchoolClass> _listSchoolClasses;
		private List<SchoolCourse> _listSchoolCourses;
		
		///<summary></summary>
		public FormReqNeededs(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRequirementsNeeded_Load(object sender, System.EventArgs e) {
			//comboClass.Items.Add(Lan.g(this,"All"));
			//comboClass.SelectedIndex=0;
			_listSchoolClasses=SchoolClasses.GetDeepCopy();
			_listSchoolCourses=SchoolCourses.GetDeepCopy();
			for(int i=0;i<_listSchoolClasses.Count;i++) {
				comboClassFrom.Items.Add(_listSchoolClasses[i].Descript);
				comboClassTo.Items.Add(_listSchoolClasses[i].Descript);
			}
			for(int i=0;i<_listSchoolCourses.Count;i++) {
				comboCourseFrom.Items.Add(_listSchoolCourses[i].Descript);
				comboCourseTo.Items.Add(_listSchoolCourses[i].Descript);
			}
			if(comboClassFrom.Items.Count>0) {
				comboClassFrom.SelectedIndex=0;
				comboClassTo.SelectedIndex=0;
			}
			if(comboCourseFrom.Items.Count>0) {
				comboCourseFrom.SelectedIndex=0;
				comboCourseTo.SelectedIndex=0;
			}
			ReloadReqList();
			FillGrid();
		}

		private void ReloadReqList() {
			_listReqsAll=ReqNeededs.GetListFromDb();
			_listReqsAllOld=_listReqsAll.Select(x => x.Copy()).ToList();
		}

		private bool RemoveReqFromAllList(ReqNeeded req) {
			for(int i=0;i<_listReqsAll.Count;i++) {
				if(_listReqsAll[i].ReqNeededNum==req.ReqNeededNum) {
					_listReqsAll.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private void FillGrid() {
			if(comboClassFrom.SelectedIndex==-1 || comboCourseFrom.SelectedIndex==-1){
				return;
			}
			long selectedReqNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				selectedReqNum=_listReqsInGrid[gridMain.GetSelectedIndex()].ReqNeededNum;
			}
			long schoolClass=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
			long schoolCourse=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
			int scroll=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableRequirementsNeeded","Description"),200);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listReqsInGrid=new List<ReqNeeded>();
			for(int i=0;i<_listReqsAll.Count;i++) {
				if(_listReqsAll[i].SchoolClassNum==schoolClass && _listReqsAll[i].SchoolCourseNum==schoolCourse) {
					_listReqsInGrid.Add(_listReqsAll[i].Copy());
				}
			}
			_listReqsInGrid=_listReqsInGrid.OrderBy(x => x.Descript).ToList();
			for(int i=0;i<_listReqsInGrid.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listReqsInGrid[i].Descript);
				gridMain.ListGridRows.Add(row);
				if(_listReqsInGrid[i].ReqNeededNum==selectedReqNum) {
					gridMain.SetSelected(i,true);
					continue;
				}
			}
			gridMain.EndUpdate();
			gridMain.ScrollValue=scroll;
		}

		private void comboClass_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void comboCourse_SelectionChangeCommitted(object sender,EventArgs e) {
			FillGrid();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(comboClassFrom.SelectedIndex==-1 || comboCourseFrom.SelectedIndex==-1){
				MsgBox.Show(this,"Please select a Class and Course first.");
				return;
			}
			using FormReqNeededEdit FormR=new FormReqNeededEdit();
			FormR.ReqCur=new ReqNeeded();
			FormR.ReqCur.SchoolClassNum=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
			FormR.ReqCur.SchoolCourseNum=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
			FormR.IsNew=true;
			FormR.ShowDialog();
			if(FormR.DialogResult!=DialogResult.OK){
				return;
			}
			if(_listReqsInGrid.Any(x => x.Descript==FormR.ReqCur.Descript//Alternative to LINQ would be to create a method and loop through the whole list
					&& x.SchoolClassNum==FormR.ReqCur.SchoolClassNum 
					&& x.SchoolCourseNum==FormR.ReqCur.SchoolCourseNum)) {
				MsgBox.Show(this,"Requirement already exist.");
				return;
			}
			_listReqsAll.Add(FormR.ReqCur);
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReqNeededEdit FormR=new FormReqNeededEdit();
			FormR.ReqCur=_listReqsInGrid[e.Row];//Previously got from the database but we want the copy from the list
			FormR.ShowDialog();
			if(FormR.DialogResult==DialogResult.OK) {
				if(FormR.ReqCur==null) {
					RemoveReqFromAllList(_listReqsInGrid[gridMain.GetSelectedIndex()]);
				}
				else {
					ReqNeeded reqNeeded=_listReqsAll.FirstOrDefault(x => x.ReqNeededNum==FormR.ReqCur.ReqNeededNum);
					if(reqNeeded != null) {//This should never be null.
						reqNeeded=FormR.ReqCur;
					}
				}
				FillGrid();
			}
		}

		private void butCopy_Click(object sender,EventArgs e) {
			if(comboClassTo.SelectedIndex==-1 ||comboCourseTo.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a Class and Course first.");
				return;
			}
			if(MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you would like to copy over the requirements? Doing so will not replace any previous requirements.")) {
				long schoolClassFrom=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
				long schoolClassTo=_listSchoolClasses[comboClassTo.SelectedIndex].SchoolClassNum;
				long schoolCourseFrom=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
				long schoolCourseTo=_listSchoolCourses[comboCourseTo.SelectedIndex].SchoolCourseNum;
				if(schoolClassFrom==schoolClassTo && schoolCourseFrom==schoolCourseTo) {
						 return;
				}
				ReqNeeded reqCur;
				for(int i=0;i<_listReqsInGrid.Count;i++) {
					reqCur=new ReqNeeded();
					reqCur.Descript=_listReqsInGrid[i].Descript;
					reqCur.SchoolClassNum=schoolClassTo;
					reqCur.SchoolCourseNum=schoolCourseTo;
					if(_listReqsAll.Any(x => x.Descript==reqCur.Descript//Alternative to LINQ would be to create a method and loop through the whole list
							&& x.SchoolClassNum==reqCur.SchoolClassNum 
							&& x.SchoolCourseNum==reqCur.SchoolCourseNum)) 
          {
						continue;
					}
					_listReqsAll.Add(reqCur);
				}
				comboClassFrom.SelectedIndex=comboClassTo.SelectedIndex;
				comboCourseFrom.SelectedIndex=comboCourseTo.SelectedIndex;
				FillGrid();
			}
		}

		/*private void butSynch_Click(object sender,EventArgs e) {
			if(comboClass.SelectedIndex==-1 || comboCourse.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a Class and Course first.");
				return;
			}
			ReqNeededs.Synch(SchoolClasses.List[comboClass.SelectedIndex].SchoolClassNum,
				SchoolCourses.List[comboCourse.SelectedIndex].SchoolCourseNum);
			MsgBox.Show(this,"Done.");
		}*/

		private void butDeleteReq_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Are you sure you would like to delete all requirements needed?")) {
				return;
			}
			for(int i=0;i<_listReqsInGrid.Count;i++) {
				RemoveReqFromAllList(_listReqsInGrid[i]);
			}
			FillGrid();
		}

		private void butOk_Click(object sender,EventArgs e) {
			ReqNeededs.Sync(_listReqsAll,_listReqsAllOld);
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		



		

		

		

	

	}
}
