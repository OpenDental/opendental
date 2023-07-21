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
		private List<ReqNeeded> _listReqNeededs;
		///<summary>Stale deep copy of _listReqsAll to use with sync.</summary>
		private List<ReqNeeded> _listReqNeededsOld;
		private List<ReqNeeded> _listReqNeededsInGrid;
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
				comboClassFrom.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
				comboClassTo.Items.Add(SchoolClasses.GetDescript(_listSchoolClasses[i]));
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
			_listReqNeededs=ReqNeededs.GetListFromDb();
			_listReqNeededsOld=_listReqNeededs.Select(x => x.Copy()).ToList();
		}

		private bool RemoveReqFromAllList(ReqNeeded reqNeeded) {
			for(int i=0;i<_listReqNeededs.Count;i++) {
				if(_listReqNeededs[i].ReqNeededNum==reqNeeded.ReqNeededNum) {
					_listReqNeededs.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		private void FillGrid() {
			if(comboClassFrom.SelectedIndex==-1 || comboCourseFrom.SelectedIndex==-1){
				return;
			}
			long reqNeededNum=0;
			if(gridMain.GetSelectedIndex()!=-1) {
				reqNeededNum=_listReqNeededsInGrid[gridMain.GetSelectedIndex()].ReqNeededNum;
			}
			long schoolClassNum=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
			long schoolCourseNum=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
			int scroll=gridMain.ScrollValue;
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col;
			col=new GridColumn(Lan.g("TableRequirementsNeeded","Description"),200);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			_listReqNeededsInGrid=new List<ReqNeeded>();
			for(int i=0;i<_listReqNeededs.Count;i++) {
				if(_listReqNeededs[i].SchoolClassNum==schoolClassNum && _listReqNeededs[i].SchoolCourseNum==schoolCourseNum) {
					_listReqNeededsInGrid.Add(_listReqNeededs[i].Copy());
				}
			}
			_listReqNeededsInGrid=_listReqNeededsInGrid.OrderBy(x => x.Descript).ToList();
			for(int i=0;i<_listReqNeededsInGrid.Count;i++) {
				row=new GridRow();
				row.Cells.Add(_listReqNeededsInGrid[i].Descript);
				gridMain.ListGridRows.Add(row);
				if(_listReqNeededsInGrid[i].ReqNeededNum==reqNeededNum) {
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
			using FormReqNeededEdit formReqNeededEdit=new FormReqNeededEdit();
			formReqNeededEdit.ReqNeededCur=new ReqNeeded();
			formReqNeededEdit.ReqNeededCur.SchoolClassNum=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
			formReqNeededEdit.ReqNeededCur.SchoolCourseNum=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
			formReqNeededEdit.IsNew=true;
			formReqNeededEdit.ShowDialog();
			if(formReqNeededEdit.DialogResult!=DialogResult.OK){
				return;
			}
			if(_listReqNeededsInGrid.Any(x => x.Descript==formReqNeededEdit.ReqNeededCur.Descript//Alternative to LINQ would be to create a method and loop through the whole list
					&& x.SchoolClassNum==formReqNeededEdit.ReqNeededCur.SchoolClassNum 
					&& x.SchoolCourseNum==formReqNeededEdit.ReqNeededCur.SchoolCourseNum)) {
				MsgBox.Show(this,"Requirement already exist.");
				return;
			}
			_listReqNeededs.Add(formReqNeededEdit.ReqNeededCur);
			FillGrid();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormReqNeededEdit formReqNeededEdit=new FormReqNeededEdit();
			formReqNeededEdit.ReqNeededCur=_listReqNeededsInGrid[e.Row];//Previously got from the database but we want the copy from the list
			formReqNeededEdit.ShowDialog();
			if(formReqNeededEdit.DialogResult==DialogResult.OK) {
				if(formReqNeededEdit.ReqNeededCur==null) {
					RemoveReqFromAllList(_listReqNeededsInGrid[gridMain.GetSelectedIndex()]);
				}
				else {
					int index=_listReqNeededs.FindIndex(x => x.ReqNeededNum==formReqNeededEdit.ReqNeededCur.ReqNeededNum);
					if(index!=-1) {
						_listReqNeededs[index]=formReqNeededEdit.ReqNeededCur;
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
				long schoolClassNumFrom=_listSchoolClasses[comboClassFrom.SelectedIndex].SchoolClassNum;
				long schoolClassNumTo=_listSchoolClasses[comboClassTo.SelectedIndex].SchoolClassNum;
				long schoolCourseNumFrom=_listSchoolCourses[comboCourseFrom.SelectedIndex].SchoolCourseNum;
				long schoolCourseNumTo=_listSchoolCourses[comboCourseTo.SelectedIndex].SchoolCourseNum;
				if(schoolClassNumFrom==schoolClassNumTo && schoolCourseNumFrom==schoolCourseNumTo) {
						 return;
				}
				ReqNeeded reqNeeded;
				for(int i=0;i<_listReqNeededsInGrid.Count;i++) {
					reqNeeded=new ReqNeeded();
					reqNeeded.Descript=_listReqNeededsInGrid[i].Descript;
					reqNeeded.SchoolClassNum=schoolClassNumTo;
					reqNeeded.SchoolCourseNum=schoolCourseNumTo;
					if(_listReqNeededs.Any(x => x.Descript==reqNeeded.Descript//Alternative to LINQ would be to create a method and loop through the whole list
							&& x.SchoolClassNum==reqNeeded.SchoolClassNum 
							&& x.SchoolCourseNum==reqNeeded.SchoolCourseNum)) 
					{
						continue;
					}
					_listReqNeededs.Add(reqNeeded);
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
			for(int i=0;i<_listReqNeededsInGrid.Count;i++) {
				RemoveReqFromAllList(_listReqNeededsInGrid[i]);
			}
			FillGrid();
		}

		private void butOk_Click(object sender,EventArgs e) {
			ReqNeededs.Sync(_listReqNeededs,_listReqNeededsOld);
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		



		

		

		

	

	}
}
