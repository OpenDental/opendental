using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;
using System.Globalization;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormRecallTypes:FormODBase {
		private bool changed;
		private List<RecallType> _listRecallTypes;
		//public bool IsSelectionMode;
		//<summary>Only used if IsSelectionMode.  On OK, contains selected pharmacyNum.  Can be 0.  Can also be set ahead of time externally.</summary>
		//public int SelectedPharmacyNum;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormRecallTypesCanada";
			}
			return "FormRecallTypes";
		}

		///<summary></summary>
		public FormRecallTypes()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormRecallTypes_Load(object sender, System.EventArgs e) {
			/*if(IsSelectionMode){
				butClose.Text=Lan.g(this,"Cancel");
			}
			else{
				butOK.Visible=false;
				butNone.Visible=false;
			}*/
			FillGrid();
			/*if(SelectedPharmacyNum!=0){
				for(int i=0;i<PharmacyC.Listt.Count;i++){
					if(PharmacyC.Listt[i].PharmacyNum==SelectedPharmacyNum){
						gridMain.SetSelected(i,true);
						break;
					}
				}
			}*/
		}

		private void FillGrid(){
			RecallTypes.RefreshCache();
			_listRecallTypes=RecallTypes.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableRecallTypes","Description"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Special Type"),110);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Triggers"),190);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Interval"),60);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Time Pattern"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableRecallTypes","Procedures"),190);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			//string txt;
			for(int i=0;i<_listRecallTypes.Count;i++){
				row=new GridRow();
				row.Cells.Add(_listRecallTypes[i].Description);
				row.Cells.Add(RecallTypes.GetSpecialTypeStr(_listRecallTypes[i].RecallTypeNum));
				row.Cells.Add(GetStringForType(_listRecallTypes[i].RecallTypeNum));
				row.Cells.Add(_listRecallTypes[i].DefaultInterval.ToString());
				row.Cells.Add(_listRecallTypes[i].TimePattern);
				row.Cells.Add(_listRecallTypes[i].Procedures);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private string GetStringForType(long recallTypeNum) {
			if(recallTypeNum==0){
				return "";
			}
			List<RecallTrigger> triggerList=RecallTriggers.GetForType(recallTypeNum);
			string retVal="";
			for(int i=0;i<triggerList.Count;i++){
				if(i>0){
					retVal+=",";
				}
				retVal+=ProcedureCodes.GetStringProcCode(triggerList[i].CodeNum);
			}
			return retVal;
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormRecallTypeEdit FormRE=new FormRecallTypeEdit();
			FormRE.RecallTypeCur=new RecallType();
			FormRE.RecallTypeCur.IsNew=true;
			FormRE.ShowDialog();
			FillGrid();
			changed=true;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			/*if(IsSelectionMode){
				SelectedPharmacyNum=PharmacyC.Listt[e.Row].PharmacyNum;
				DialogResult=DialogResult.OK;
				return;
			}
			else{*/
			using FormRecallTypeEdit FormR=new FormRecallTypeEdit();
			FormR.RecallTypeCur=_listRecallTypes[e.Row].Copy();
			FormR.ShowDialog();
			FillGrid();
			changed=true;
			//}*/
		}

		private void butSynch_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			GC.Collect();//free up resources since this could take a lot of memory
			DataValid.SetInvalid(InvalidType.RecallTypes);
			Action actionCloseRecallSyncProgress=ODProgress.Show(ODEventType.RecallSync,typeof(RecallSyncEvent),Lan.g(this,"Running Prep Queries")+"...",false,true);
			bool isSyncCompleted=Recalls.SynchAllPatients();
			actionCloseRecallSyncProgress?.Invoke();
			GC.Collect();//clean up resources, force the garbage collector to collect since resources may remain tied-up
			Cursor=Cursors.Default;
			if(isSyncCompleted) {
				changed=false;
				MsgBox.Show(this,"Done.");
			}
			else {
				MsgBox.Show(this,"Synch is currently running from a different workstation.");
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void FormRecallTypes_FormClosing(object sender,FormClosingEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.RecallTypes);
				if(MessageBox.Show(Lan.g(this,"Recalls for all patients should be synchronized.  Synchronize now?"),"",MessageBoxButtons.YesNo)
					==DialogResult.Yes)
				{
					Cursor=Cursors.WaitCursor;
					GC.Collect();//free up resources since this could take a lot of memory
					Action actionCloseRecallSyncProgress=ODProgress.Show(ODEventType.RecallSync,typeof(RecallSyncEvent),Lan.g(this,"Running Prep Queries")+"...");
					bool isSyncSuccessful=Recalls.SynchAllPatients();
					actionCloseRecallSyncProgress?.Invoke();
					GC.Collect();//clean up resources, force the garbage collector to collect since resources may remain tied-up
					Cursor=Cursors.Default;
					if(!isSyncSuccessful) {
						MsgBox.Show(this,"Synch is currently running from a different workstation.  Recalls should be synchronized again later.");
					}
				}
			}
		}

	

	

		

		

		



		
	}
}





















