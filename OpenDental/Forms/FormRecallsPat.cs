using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormRecallsPat:FormODBase {
		public long PatNum;
		///<summary>This is just the list for the current patient.</summary>
		private List<Recall> _listRecalls;
		private bool _isPerio;

		public FormRecallsPat() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecallsPat_Load(object sender,EventArgs e) {
			/*
			//patient may or may not have existing recalls.
			Recall recallCur=null;
			for(int i=0;i<RecallList.Count;i++){
				if(RecallList[i].PatNum==PatCur.PatNum){
					recallCur=RecallList[i];
				}
			}*/
			//for testing purposes and because synchronization might have bugs, always synch here:
			//This might add a recall.
			//Recalls.Synch(PatNum);			
			FillGrid();
		}

		private void FillGrid() {
			Recalls.Synch(PatNum);
			Recalls.SynchScheduledApptFull(PatNum);
			_listRecalls=Recalls.GetList(PatNum);
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRecallsPat","Type"),90);
			gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("TableRecallsPat","Disabled"),60,HorizontalAlignment.Center);
			//gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","PreviousDate"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Due Date"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Sched Date"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Interval"),70);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Status"),80);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Time Override"),90);
			gridMain.Columns.Add(col);
			col=new GridColumn(Lan.g("TableRecallsPat","Note"),100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			GridCell cell;
			_isPerio=false;
			butPerio.Text=Lan.g(this,"Set Perio");
			string cellStr;
			for(int i=0;i<_listRecalls.Count;i++){
				if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==_listRecalls[i].RecallTypeNum){
					_isPerio=true;
					butPerio.Text=Lan.g(this,"Set Prophy");
				}
				row=new GridRow();
				row.Cells.Add(RecallTypes.GetDescription(_listRecalls[i].RecallTypeNum));
				//if(RecallList[i].IsDisabled){
				//	row.Cells.Add("X");
				//}
				//else{
				//	row.Cells.Add("");
				//}
				if(_listRecalls[i].DatePrevious.Year<1880){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listRecalls[i].DatePrevious.ToShortDateString());
				}
				if(_listRecalls[i].DateDue.Year<1880){
					row.Cells.Add("");
				}
				else{
					cell=new GridCell(_listRecalls[i].DateDue.ToShortDateString());
					if(_listRecalls[i].DateDue<DateTime.Today){
						cell.Bold=YN.Yes;
						cell.ColorText=Color.Firebrick;
					}
					row.Cells.Add(cell);
				}
				if(_listRecalls[i].DateScheduled.Year<1880) {
					row.Cells.Add("");
				}
				else {
					row.Cells.Add(_listRecalls[i].DateScheduled.ToShortDateString());
				}
				row.Cells.Add(_listRecalls[i].RecallInterval.ToString());
				row.Cells.Add(Defs.GetValue(DefCat.RecallUnschedStatus,_listRecalls[i].RecallStatus));
				row.Cells.Add(_listRecalls[i].TimePatternOverride);
				cellStr="";
				if(_listRecalls[i].IsDisabled) {
					cellStr+=Lan.g(this,"Disabled");
				}
				if(_listRecalls[i].DisableUntilDate.Year>1880) {
					if(cellStr!="") {
						cellStr+=", ";
					}
					cellStr+=Lan.g(this,"Disabled until ")+_listRecalls[i].DisableUntilDate.ToShortDateString();
				}
				if(_listRecalls[i].DisableUntilBalance>0) {
					if(cellStr!="") {
						cellStr+=", ";
					}
					cellStr+=Lan.g(this,"Disabled until balance ")+_listRecalls[i].DisableUntilBalance.ToString("c");
				}
				if(_listRecalls[i].Note!="") {
					if(cellStr!="") {
						cellStr+=", ";
					}
					cellStr+=_listRecalls[i].Note;
				}
				row.Cells.Add(cellStr);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			using FormRecallEdit formRecallEdit=new FormRecallEdit();
			formRecallEdit.RecallCur=_listRecalls[e.Row].Copy();
			formRecallEdit.ShowDialog();
			FillGrid();
		}

		private void butPerio_Click(object sender,EventArgs e) {
			//make sure we have both special types properly setup.
			if(!RecallTypes.PerioAndProphyBothHaveTriggers()){
				MsgBox.Show(this,"Prophy and Perio special recall types are not setup properly.  They must both exist, and they must both have a trigger.");
				return;
			}
			if(_isPerio){
				//change the perio types to prophy
				for(int i=0;i<_listRecalls.Count;i++){
					if(PrefC.GetLong(PrefName.RecallTypeSpecialPerio)==_listRecalls[i].RecallTypeNum){
						_listRecalls[i].RecallTypeNum=PrefC.GetLong(PrefName.RecallTypeSpecialProphy);
						_listRecalls[i].RecallInterval=RecallTypes.GetInterval(PrefC.GetLong(PrefName.RecallTypeSpecialProphy));
						//previous date will be reset below in synch, but probably won't change since similar triggers.
						Recalls.Update(_listRecalls[i]);
						SecurityLogs.MakeLogEntry(Permissions.RecallEdit,_listRecalls[i].PatNum,"Recall changed to Prophy from the Recalls for Patient window.");
						break;
					}
				}
				FillGrid();
				return;
			}
			bool isFound=false;
			//change any prophy types to perio
			for(int i=0;i<_listRecalls.Count;i++){
				if(PrefC.GetLong(PrefName.RecallTypeSpecialProphy)==_listRecalls[i].RecallTypeNum){
					_listRecalls[i].RecallTypeNum=PrefC.GetLong(PrefName.RecallTypeSpecialPerio);
					_listRecalls[i].RecallInterval=RecallTypes.GetInterval(PrefC.GetLong(PrefName.RecallTypeSpecialPerio));
					//previous date will be reset below in synch, but probably won't change since similar triggers.
					Recalls.Update(_listRecalls[i]);
					SecurityLogs.MakeLogEntry(Permissions.RecallEdit,_listRecalls[i].PatNum,"Recall changed to Perio from the Recalls for Patient window.");
					isFound=true;
					break;
				}
			}
			//if none found, then add a perio
			if(!isFound){
				Recall recall=new Recall();
				recall.PatNum=PatNum;
				recall.RecallInterval=RecallTypes.GetInterval(PrefC.GetLong(PrefName.RecallTypeSpecialPerio));
				recall.RecallTypeNum=PrefC.GetLong(PrefName.RecallTypeSpecialPerio);
				Recalls.Insert(recall);
				SecurityLogs.MakeLogEntry(Permissions.RecallEdit,recall.PatNum,"Perio recall added from the Recalls for Patient window.");
			}
			FillGrid();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			Recall recall=new Recall();
			recall.RecallTypeNum=0;//user will have to pick
			recall.PatNum=PatNum;
			recall.RecallInterval=new Interval(0,0,6,0);
			using FormRecallEdit formRecallEdit=new FormRecallEdit();
			formRecallEdit.IsNew=true;
			formRecallEdit.RecallCur=recall;
			formRecallEdit.ShowDialog();
			FillGrid();
		}

		private void butClose_Click(object sender,EventArgs e) {	
			Close();
		}

		private void FormRecallsPat_FormClosing(object sender,FormClosingEventArgs e) {
			//check for duplicates that might cause a malfunction.
			int prophyCount=0;
			for(int i=0;i<_listRecalls.Count;i++) {
				if(RecallTypes.ProphyType==_listRecalls[i].RecallTypeNum) {
					prophyCount++;
				}
				if(RecallTypes.PerioType==_listRecalls[i].RecallTypeNum) {
					prophyCount++;
				}
			}
			if(prophyCount>1) {
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Multiple prophy and/or perio recalls detected.  A patient should have only one prophy or perio recall, and the calculations will not work correctly otherwise.  Continue anyway?")){
					e.Cancel=true;
				}
			}
			Plugins.HookAddCode(this,"FormRecallsPat.FormClosing_end",PatNum);
		}

		

		

		

		
		

		
	}
}