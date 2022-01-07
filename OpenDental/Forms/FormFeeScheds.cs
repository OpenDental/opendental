using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Linq;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormFeeScheds:FormODBase {
		private FeeSched _feeSchedToMove;
		private List<FeeSched> _listFeeSchedsForType;
		private bool _hasChanged=false;
		private bool _isSelectionMode;
		public long SelectedFeeSchedNum;
		///<summary>Is in middle of moving a row to a new Order, and the second click in the grid will finish the job.</summary>
		private bool _IsMovingToOrder=false;
		///<summary>If IsSelectionMode then is a list of all non-hidden fee schedules.  Otherwise, uses the cache deep copy.</summary>
		private List<FeeSched> _listFeeScheds;

		///<summary></summary>
		public FormFeeScheds(bool isSelectionMode=true)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_isSelectionMode=isSelectionMode;
		}

		private void FormFeeSchedules_Load(object sender, System.EventArgs e) {
			_listFeeScheds=FeeScheds.GetDeepCopy(_isSelectionMode);
			CheckItemOrders();
			Array arrayFeeSchedTypes=Enum.GetValues(typeof(FeeScheduleType));
			AllowedFeeSchedsAutomate pref=(AllowedFeeSchedsAutomate)PrefC.GetInt(PrefName.AllowedFeeSchedsAutomate);
			for(int i=0;i<arrayFeeSchedTypes.Length;i++) {
				FeeScheduleType feeScheduleType=((FeeScheduleType)arrayFeeSchedTypes.GetValue(i));
				if(feeScheduleType==FeeScheduleType.OutNetwork) {
					listType.Items.Add("Out of Network");
				}
				else {
					listType.Items.Add(arrayFeeSchedTypes.GetValue(i).ToString());
				}
				listType.SetSelected(i);
				//Deselect the Item if the FeeScheduleType is Out of Network and using the blue book feature.
				if(feeScheduleType==FeeScheduleType.OutNetwork && pref==AllowedFeeSchedsAutomate.BlueBook){
					listType.SetSelected(i,false);
				}
				//Deselect the Item if the FeeScheduleType is ManualBlueBook and not using the blue book feature (using Legacy Blue book or None).
				if(feeScheduleType==FeeScheduleType.ManualBlueBook && pref!=AllowedFeeSchedsAutomate.BlueBook) {
					listType.SetSelected(i,false);
				}
			}
			butSort.Enabled=false;
			if(!Security.IsAuthorized(Permissions.SecurityAdmin,true)){
				butCleanUp.Visible=false;
				labelCleanUp.Visible=false;
				butHideUnused.Visible=false;
				labelHideUnused.Visible=false;
			}
			if(_isSelectionMode) {
				butOK.Visible=true;
				butClose.Text="Cancel";
				butUp.Visible=false;
				butDown.Visible=false;
				butSort.Visible=false;
				labelSort.Visible=false;
				butAdd.Visible=false;
				butCleanUp.Visible=false;
				labelCleanUp.Visible=false;
				butHideUnused.Visible=false;
				labelHideUnused.Visible=false;
				groupBox7.Visible=false;
				butSetOrder.Visible=false;
				labelSetOrder.Visible=false;
				checkBoxShowHidden.Visible=false;
			}
			FillGrid();
		}

		///<summary>Also fixes if any errors found.</summary>
		private void CheckItemOrders() {
			Cursor=Cursors.WaitCursor;
			bool ordersChanges=false;
			_listFeeScheds.Sort(CompareItemOrder);
			for(int i=0;i<_listFeeScheds.Count;i++) {
				if(_listFeeScheds[i].ItemOrder==i) {
					continue;
				}
				ordersChanges=true;
				_hasChanged=true;
				break;
			}
			if(ordersChanges) {
				FeeScheds.CorrectFeeSchedOrder();
				FeeScheds.RefreshCache();
				_listFeeScheds=FeeScheds.GetDeepCopy(_isSelectionMode);
			}
			Cursor=Cursors.Default;
		}

		private void FillGrid(){
			_listFeeSchedsForType=new List<FeeSched>();
			for(int i=0;i<listType.SelectedIndices.Count;i++) {
				List<FeeSched> listFeeSchedsToAdd=FeeScheds.GetListForType((FeeScheduleType)listType.SelectedIndices[i],true,_listFeeScheds);
				_listFeeSchedsForType.AddRange(listFeeSchedsToAdd);
			}
			_listFeeSchedsForType.Sort(CompareItemOrder);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableFeeScheds","Description"),145);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableFeeScheds","Type"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g("TableFeeScheds","Hidden"),60,HorizontalAlignment.Center);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listFeeSchedsForType.Count;i++){
				if(_isSelectionMode && _listFeeSchedsForType[i].IsHidden) {
					continue;
				}
				if(_listFeeSchedsForType[i].IsHidden && !checkBoxShowHidden.Checked) {
					continue;
				}
				row=new GridRow();
				row.Tag=_listFeeSchedsForType[i];
				row.Cells.Add(_listFeeSchedsForType[i].Description);
				row.Cells.Add(_listFeeSchedsForType[i].FeeSchedType.ToString());
				if(_listFeeSchedsForType[i].IsHidden){
					row.Cells.Add("X");
				}
				else{
					row.Cells.Add("");
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			using FormFeeSchedEdit FormF=new FormFeeSchedEdit();
			FormF.FeeSchedCur=new FeeSched();
			FormF.FeeSchedCur.IsNew=true;
			FormF.FeeSchedCur.ItemOrder=_listFeeScheds.Count;
			if(listType.SelectedIndices.Count==1){
				FormF.FeeSchedCur.FeeSchedType=(FeeScheduleType)(listType.SelectedIndices[0]);
			}
			FormF.ShowDialog();
			if(FormF.DialogResult!=DialogResult.OK) {
				return;
			}
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy();
			_hasChanged=true;
			FillGrid();
			for(int i=0;i<_listFeeSchedsForType.Count;i++){
				if(FormF.FeeSchedCur.FeeSchedNum==_listFeeSchedsForType[i].FeeSchedNum){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void gridMain_CellClick(object sender,ODGridClickEventArgs e) {
			if(!_IsMovingToOrder) {
				return;
			}
			FeeScheds.RepositionFeeSched(_feeSchedToMove,((FeeSched)gridMain.ListGridRows[e.Row].Tag).ItemOrder);
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy();
			FillGrid();
			gridMain.SetSelected(e.Row,true);
			_IsMovingToOrder=false;
			_hasChanged=true;
			butSetOrder.BackColor=SystemColors.Control;
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_isSelectionMode) {
				SelectedFeeSchedNum=((FeeSched)gridMain.ListGridRows[e.Row].Tag).FeeSchedNum;
				DialogResult=DialogResult.OK;
				Close();
				return;
			}
			using FormFeeSchedEdit FormF=new FormFeeSchedEdit();
			FormF.FeeSchedCur=(FeeSched)gridMain.ListGridRows[e.Row].Tag;
			FormF.ShowDialog();
			if(FormF.DialogResult!=DialogResult.OK) {
				return;
			}
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy();
			_hasChanged=true;
			FillGrid();
			for(int i=0;i<_listFeeSchedsForType.Count;i++){
				if(FormF.FeeSchedCur.FeeSchedNum==_listFeeSchedsForType[i].FeeSchedNum){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void listType_SelectionChangeCommitted(object sender,EventArgs e) {
			//Can only sort all fee scheds when all types are being viewed.
			if(listType.SelectedIndices.Count==listType.Items.Count) {
				butSort.Enabled=true;
			}
			else {
				butSort.Enabled=false;
			}
			FillGrid();
		}

		private void checkBoxShowHidden_Click(object sender,EventArgs e) {
			FillGrid();
		}

		private void butUp_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a fee schedule first.");
				return;
			}
			if(idx==0){
				return;
			}
			//swap the orders.  This makes it work no matter which types are being viewed.
			_hasChanged=true;
			int oldItemOrder=((FeeSched)gridMain.ListGridRows[idx].Tag).ItemOrder;
			((FeeSched)gridMain.ListGridRows[idx].Tag).ItemOrder=((FeeSched)gridMain.ListGridRows[idx-1].Tag).ItemOrder;
			FeeScheds.Update((FeeSched)gridMain.ListGridRows[idx].Tag);
			((FeeSched)gridMain.ListGridRows[idx-1].Tag).ItemOrder=oldItemOrder;
			FeeScheds.Update((FeeSched)gridMain.ListGridRows[idx-1].Tag);
			FillGrid();
			gridMain.SetSelected(idx-1,true);
		}

		private void butDown_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1){
				MsgBox.Show(this,"Please select a fee schedule first.");
				return;
			}
			if(idx==gridMain.ListGridRows.Count-1){
				return;
			}
			_hasChanged=true;
			int oldItemOrder=((FeeSched)gridMain.ListGridRows[idx].Tag).ItemOrder;
			((FeeSched)gridMain.ListGridRows[idx].Tag).ItemOrder=((FeeSched)gridMain.ListGridRows[idx+1].Tag).ItemOrder;
			FeeScheds.Update((FeeSched)gridMain.ListGridRows[idx].Tag);
			((FeeSched)gridMain.ListGridRows[idx+1].Tag).ItemOrder=oldItemOrder;
			FeeScheds.Update((FeeSched)gridMain.ListGridRows[idx+1].Tag);
			FillGrid();
			gridMain.SetSelected(idx+1,true);
		}

		private void butSetOrder_Click(object sender,EventArgs e) {
			int idx=gridMain.GetSelectedIndex();
			if(idx==-1) {
				MsgBox.Show(this,"Please select a fee schedule first.");
				return;
			}
			butSetOrder.BackColor=Color.Red;
			_feeSchedToMove=(FeeSched)gridMain.ListGridRows[idx].Tag;
			_IsMovingToOrder=true;
		}

		private void butSort_Click(object sender,EventArgs e) {
			//only enabled if viewing all types
			FeeScheds.SortFeeSched();
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy();
			FillGrid();
		}

		///<summary>This sorts feescheds by their item order.</summary>
		private static int CompareItemOrder(FeeSched feeSched1,FeeSched feeSched2) {
			return feeSched1.ItemOrder.CompareTo(feeSched2.ItemOrder);
		}

		///<summary>This sorts feescheds by type and alphabetically.</summary>
		private static int CompareFeeScheds(FeeSched feeSched1,FeeSched feeSched2) {
			if(feeSched1==null){
				if(feeSched2==null){
					return 0;//both null, so equal
				}
				else{
					return -1;
				}
			}
			if(feeSched2==null){
				return 1;
			}
			if(feeSched1.FeeSchedType!=feeSched2.FeeSchedType){
				return feeSched1.FeeSchedType.CompareTo(feeSched2.FeeSchedType);
			}
			return feeSched1.Description.CompareTo(feeSched2.Description);
		}

		private void butIns_Click(object sender,EventArgs e) {
			using FormFeesForIns FormF=new FormFeesForIns();
			FormF.ShowDialog();
			//DialogResult=DialogResult.OK;
		}

		private void butCleanUp_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete allowed fee schedules that are not in use or that are attached to hidden insurance plans?")) {
				return;
			}
			long changed=FeeScheds.CleanupAllowedScheds();
			MessageBox.Show(changed.ToString()+" "+Lan.g(this,"unused fee schedules deleted."));
			if(changed==0) {
				return;
			}
			_hasChanged=true;
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy(_isSelectionMode);  //After deletion, refresh in-memory copy to continue editing.
			CheckItemOrders();
			FillGrid();
		}

		private void butHideUnused_Click(object sender,EventArgs e) {
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Hide fee schedules that are not in use by insurance plans, patients, or providers?\r\n"
				+"A backup of the database will be made first.")) 
			{
				return;
			}
			Action actionProgress=ODProgress.Show(ODEventType.ProgressBar,startingMessage:Lans.g(this,"Backing up database..."));
			try {
				MiscData.MakeABackup();
			} 
			catch(Exception ex) {
				actionProgress?.Invoke();
				FriendlyException.Show(Lans.g(this,"Unable to make a backup. No fee schedules have been altered."),ex);
				return;
			}
			ODEvent.Fire(ODEventType.ProgressBar,Lans.g(this,"Hiding unused fee schedules..."));
			long countChanged=FeeScheds.HideUnusedScheds();
			actionProgress?.Invoke();
			MessageBox.Show(countChanged.ToString()+" "+Lans.g(this,"unused fee schedules hidden."));
			if(countChanged==0) {
				return;
			}
			_hasChanged=true;
			FeeScheds.RefreshCache();
			_listFeeScheds=FeeScheds.GetDeepCopy(_isSelectionMode);
			FillGrid();
		}

		private void butOK_Click(object sender,EventArgs e) {
			//only visible in selection mode
			if(gridMain.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select a row first.");
				return;
			}
			SelectedFeeSchedNum=((FeeSched)gridMain.ListGridRows[gridMain.GetSelectedIndex()].Tag).FeeSchedNum;
			DialogResult=DialogResult.OK;
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			//also cancel button
			SelectedFeeSchedNum=0;
			DialogResult=DialogResult.Cancel;
		}

		private void FormFeeSchedules_FormClosing(object sender,FormClosingEventArgs e) {
			if(_hasChanged){
				DataValid.SetInvalid(InvalidType.FeeScheds);
			}
		}
	}

	
		

	
}





















