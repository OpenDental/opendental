using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormApptViewEdit : FormODBase {
		///<summary></summary>
		public bool IsNew;
		/// <summary>Those elements which are showing in the list of available elements.</summary>
		private List<EnumApptViewElement> _listEnumApptViewElementsAvailable;
		///<summary>The actual ApptFieldDefNums of all available elements because no language translation is needed.</summary>
		private List<long> _listApptFieldDefNumsAvailable;
		///<summary>The actual PatFieldDefNums of all available elements because no language translation is needed.</summary>
		private List<long> _listPatFieldDefNums;
		///<summary>A local list of ApptViewItems which are displayed in all three lists on the right.  Not updated to db until the form is closed.</summary>
		private List<ApptViewItem> _listApptViewItemsDisplayedAll;
		private List<ApptViewItem> _listApptViewItemsDisplayedMain;
		private List<ApptViewItem> _listMobileApptViewItems;
		private List<ApptViewItem> _listApptViewItemsDisplayedUR;
		private List<ApptViewItem> _listApptViewItemsDisplayedLR;
		///<summary>Set this value before opening the form.</summary>
		public ApptView ApptViewCur;
		///<summary>Set this value with the clinic selected in FormApptViews. Only to be used to set comboClinic.SelectedClinicNum.</summary>
		public long ClinicNumInitial;
		///<summary>List of all ApptViewItems for this view.  All 5 types.</summary>
		private List<ApptViewItem> _listApptViewItems;
		///<summary>List of ApptViewItems that does not include OpNum or ProvNum. This includes ElementDesc, ApptFieldDefNum, and PatFieldDefNum.</summary>
		private List<ApptViewItem> _listApptViewItemsDef;
		///<summary>This is a list of all operatories available to add to this view based on AssignedClinicNum and the clinic the ops are assigned to.  If the clinics show feature is turned off (EasyNoClinics=true) or if the view is not assigned to a clinic, all unhidden ops will be available.  If an op is not assigned to a clinic, it will only be available to add to views also not assigned to a clinic.  If the view is assigned to a clinic, ops assigned to the same clinic will be available to add to the view.</summary>
		private List<long> _listOpNums;
		private List<Provider> _listProviders;

		///<summary></summary>
		public FormApptViewEdit()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptViewEdit_Load(object sender, System.EventArgs e) {
			textDescription.Text=ApptViewCur.Description;
			butMobileView.Visible=MobileAppDevices.IsClinicSignedUpForMobileWeb(ClinicNumInitial);
			if(ApptViewCur.RowsPerIncr==0){
				textRowsPerIncr.Text="1";
			}
			else{
				textRowsPerIncr.Text=ApptViewCur.RowsPerIncr.ToString();
			}
			textWidthOpMinimum.Text=POut.Int(ApptViewCur.WidthOpMinimum);
			textScrollTime.Text=ApptViewCur.ApptTimeScrollStart.ToStringHmm();
			checkDynamicScroll.Checked=ApptViewCur.IsScrollStartDynamic;
			checkApptBubblesDisabled.Checked=ApptViewCur.IsApptBubblesDisabled;
			if(IsNew) {
				checkApptBubblesDisabled.Checked=PrefC.GetBool(PrefName.AppointmentBubblesDisabled);
			}
			checkOnlyScheduledProvs.Checked=ApptViewCur.OnlyScheduledProvs;
			checkOnlyScheduledProvDays.Checked=ApptViewCur.OnlyScheduledProvDays;
			if(ApptViewCur.OnlySchedBeforeTime > new TimeSpan(0,0,0)) {
				textBeforeTime.Text=(DateTime.Today+ApptViewCur.OnlySchedBeforeTime).ToShortTimeString();
			}
			if(ApptViewCur.OnlySchedAfterTime > new TimeSpan(0,0,0)) {
				textAfterTime.Text=(DateTime.Today+ApptViewCur.OnlySchedAfterTime).ToShortTimeString();
			}
			comboClinic.ClinicNumSelected=ClinicNumInitial;
			UpdateDisplayFilterGroup();
			_listApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==ApptViewCur.ApptViewNum && !x.IsMobile);
			_listMobileApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==ApptViewCur.ApptViewNum && x.IsMobile);
			_listApptViewItemsDef=_listApptViewItems.FindAll(x => x.OpNum==0 && x.ProvNum==0);
			FillOperatories();
			_listProviders=Providers.GetDeepCopy(true);
			for(int i=0;i<_listProviders.Count;i++) {
				listProv.Items.Add(_listProviders[i].GetLongDesc());
				if(_listApptViewItems.Select(x => x.ProvNum).Contains(_listProviders[i].ProvNum)) {
					listProv.SetSelected(i,true);
				}
			}
			listWaitingRmNameFormat.Items.AddEnums<EnumWaitingRmName>();
			listWaitingRmNameFormat.SetSelected((int)ApptViewCur.WaitingRmName);
			for(int i=0;i<Enum.GetNames(typeof(ApptViewStackBehavior)).Length;i++){
				listStackUR.Items.Add(Lan.g("enumApptViewStackBehavior",Enum.GetNames(typeof(ApptViewStackBehavior))[i]));
				listStackLR.Items.Add(Lan.g("enumApptViewStackBehavior",Enum.GetNames(typeof(ApptViewStackBehavior))[i]));
			}
			listStackUR.SelectedIndex=(int)ApptViewCur.StackBehavUR;
			listStackLR.SelectedIndex=(int)ApptViewCur.StackBehavLR;
			_listApptViewItemsDisplayedAll=new List<ApptViewItem>(_listApptViewItemsDef);
			FillElements();
		}

		///<summary>Fills the five lists based on the displayedElements lists. No database transactions are performed here.</summary>
		private void FillElements(){
			_listApptViewItemsDisplayedMain=new List<ApptViewItem>();
			_listApptViewItemsDisplayedUR=new List<ApptViewItem>();
			_listApptViewItemsDisplayedLR=new List<ApptViewItem>();
			for(int i=0;i<_listApptViewItemsDisplayedAll.Count;i++) {
				if(_listApptViewItemsDisplayedAll[i].ElementAlignment==ApptViewAlignment.Main) {
					_listApptViewItemsDisplayedMain.Add(_listApptViewItemsDisplayedAll[i]);
				}
				else if(_listApptViewItemsDisplayedAll[i].ElementAlignment==ApptViewAlignment.UR) {
					_listApptViewItemsDisplayedUR.Add(_listApptViewItemsDisplayedAll[i]);
				}
				else if(_listApptViewItemsDisplayedAll[i].ElementAlignment==ApptViewAlignment.LR) {
					_listApptViewItemsDisplayedLR.Add(_listApptViewItemsDisplayedAll[i]);
				}
			}
			//Now fill the lists on the screen--------------------------------------------------
			gridMain.BeginUpdate();
			gridMain.Columns.Clear();
			GridColumn col=new GridColumn("",100);
			gridMain.Columns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++){
				row=new GridRow();
				if(_listApptViewItemsDisplayedMain[i].ApptFieldDefNum>0){
					row.Cells.Add(MarkFieldNameIfHidden(_listApptViewItemsDisplayedMain[i].ApptFieldDefNum));
				}
				else if(_listApptViewItemsDisplayedMain[i].PatFieldDefNum>0){
					row.Cells.Add(PatFieldDefs.GetFieldName(_listApptViewItemsDisplayedMain[i].PatFieldDefNum));
				}
				else{
					row.Cells.Add(_listApptViewItemsDisplayedMain[i].ElementDesc);
				}
				if(DoSetBackgroundColor(_listApptViewItemsDisplayedMain[i].ElementDesc))
				{
					row.ColorBackG=_listApptViewItemsDisplayedMain[i].ElementColor;
				}
				else{
					row.ColorText=_listApptViewItemsDisplayedMain[i].ElementColor;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//gridUR---------------------------------------------------------
			gridUR.BeginUpdate();
			gridUR.Columns.Clear();
			col=new GridColumn("",100);
			gridUR.Columns.Add(col);
			gridUR.ListGridRows.Clear();
			for(int i=0;i<_listApptViewItemsDisplayedUR.Count;i++) {
				row=new GridRow();
				if(_listApptViewItemsDisplayedUR[i].ApptFieldDefNum>0) {
					row.Cells.Add(MarkFieldNameIfHidden(_listApptViewItemsDisplayedUR[i].ApptFieldDefNum));
				}
				else if(_listApptViewItemsDisplayedUR[i].PatFieldDefNum>0) {
					row.Cells.Add(PatFieldDefs.GetFieldName(_listApptViewItemsDisplayedUR[i].PatFieldDefNum));
				}
				else {
					row.Cells.Add(_listApptViewItemsDisplayedUR[i].ElementDesc);
				}
				if(DoSetBackgroundColor(_listApptViewItemsDisplayedUR[i].ElementDesc))
				{
					row.ColorBackG=_listApptViewItemsDisplayedUR[i].ElementColor;
				}
				else{
					row.ColorText=_listApptViewItemsDisplayedUR[i].ElementColor;
				}
				gridUR.ListGridRows.Add(row);
			}
			gridUR.EndUpdate();
			//gridLR-----------------------------------------------------------
			gridLR.BeginUpdate();
			gridLR.Columns.Clear();
			col=new GridColumn("",100);
			gridLR.Columns.Add(col);
			gridLR.ListGridRows.Clear();
			for(int i=0;i<_listApptViewItemsDisplayedLR.Count;i++) {
				row=new GridRow();
				if(_listApptViewItemsDisplayedLR[i].ApptFieldDefNum>0) {
					row.Cells.Add(ApptFieldDefs.GetFieldName(_listApptViewItemsDisplayedLR[i].ApptFieldDefNum));
				}
				else if(_listApptViewItemsDisplayedLR[i].PatFieldDefNum>0) {
					row.Cells.Add(PatFieldDefs.GetFieldName(_listApptViewItemsDisplayedLR[i].PatFieldDefNum));
				}
				else {
					row.Cells.Add(_listApptViewItemsDisplayedLR[i].ElementDesc);
				}
				if(DoSetBackgroundColor(_listApptViewItemsDisplayedLR[i].ElementDesc))
				{
					row.ColorBackG=_listApptViewItemsDisplayedLR[i].ElementColor;
				}
				else{
					row.ColorText=_listApptViewItemsDisplayedLR[i].ElementColor;
				}
				gridLR.ListGridRows.Add(row);
			}
			gridLR.EndUpdate();
			//gridAvailable-----------------------------------------------------------
			gridAvailable.BeginUpdate();
			gridAvailable.Columns.Clear();
			col=new GridColumn("",100);
			gridAvailable.Columns.Add(col);
			gridAvailable.ListGridRows.Clear();
			_listEnumApptViewElementsAvailable=new List<EnumApptViewElement>();
			for(int i=0;i<Enum.GetValues(typeof(EnumApptViewElement)).Length;i++){
				if(((EnumApptViewElement)i)==EnumApptViewElement.None){
					continue;//none is not available for display and won't show anywhere in the UI
				}
				if(!ElementIsDisplayed((EnumApptViewElement)i)) {
					_listEnumApptViewElementsAvailable.Add((EnumApptViewElement)i);
					row=new GridRow();
					row.Cells.Add(Lan.g(this,((EnumApptViewElement)i).GetDescription()));
					gridAvailable.ListGridRows.Add(row);
				}
			}
			gridAvailable.EndUpdate();
			//gridApptFieldDefs-----------------------------------------------------------
			gridApptFieldDefs.BeginUpdate();
			gridApptFieldDefs.Columns.Clear();
			col=new GridColumn("",100);
			gridApptFieldDefs.Columns.Add(col);
			gridApptFieldDefs.ListGridRows.Clear();
			_listApptFieldDefNumsAvailable=new List<long>();
			List<ApptFieldDef> listApptFieldDefs=ApptFieldDefs.GetDeepCopy();
			for(int i=0;i<listApptFieldDefs.Count;i++) {
				if(!ApptFieldIsDisplayed(listApptFieldDefs[i].ApptFieldDefNum)) {
					_listApptFieldDefNumsAvailable.Add(listApptFieldDefs[i].ApptFieldDefNum);
					row=new GridRow();
					row.Cells.Add(MarkFieldNameIfHidden(listApptFieldDefs[i].ApptFieldDefNum));
					gridApptFieldDefs.ListGridRows.Add(row);
				}
			}
			gridApptFieldDefs.EndUpdate();
			//gridPatFieldDefs-----------------------------------------------------------
			gridPatFieldDefs.BeginUpdate();
			gridPatFieldDefs.Columns.Clear();
			col=new GridColumn("",100);
			gridPatFieldDefs.Columns.Add(col);
			gridPatFieldDefs.ListGridRows.Clear();
			_listPatFieldDefNums=new List<long>();
			List<PatFieldDef> listPatFieldDefs=PatFieldDefs.GetDeepCopy(true);
			for(int i=0;i<listPatFieldDefs.Count;i++) {
				if(!PatFieldIsDisplayed(listPatFieldDefs[i].PatFieldDefNum)) {
					_listPatFieldDefNums.Add(listPatFieldDefs[i].PatFieldDefNum);
					row=new GridRow();
					row.Cells.Add(listPatFieldDefs[i].FieldName);
					gridPatFieldDefs.ListGridRows.Add(row);
				}
			}
			gridPatFieldDefs.EndUpdate();
		}

		///<summary>Fills the list box of operatories available for the view.  Considers clinics.</summary>
		private void FillOperatories() {
			listOps.ClearSelected();
			listOps.Items.Clear();
			_listOpNums=new List<long>();
			List<Operatory> listOperatories=Operatories.GetDeepCopy(true);
			for(int i=0;i<listOperatories.Count;i++) {
				if(!PrefC.HasClinicsEnabled //add op to list of ops available for the view if the clinics show feature is turned off
					|| comboClinic.ClinicNumSelected==0 //or this view is not assigned to a clinic
					|| listOperatories[i].ClinicNum==comboClinic.ClinicNumSelected) //or the operatory is assigned to same clinic as this view
				{
					listOps.Items.Add(listOperatories[i].OpName);
					_listOpNums.Add(listOperatories[i].OperatoryNum);
					if(_listApptViewItems.Select(x => x.OpNum).Contains(listOperatories[i].OperatoryNum)) {
						listOps.SetSelected(listOps.Items.Count-1,true);
					}
				}
			}
		}

		///<summary>Called from FillElements. Used to determine whether a given element is already displayed. If not, then it is displayed in the available rows on the left.</summary>
		private bool ElementIsDisplayed(EnumApptViewElement apptViewElement){
			for(int i=0;i<_listApptViewItemsDisplayedAll.Count;i++){
				if(_listApptViewItemsDisplayedAll[i].ApptFieldDefNum!=0 || _listApptViewItemsDisplayedAll[i].PatFieldDefNum!=0){
					continue;
				}
				if(_listApptViewItemsDisplayedAll[i].ElementDesc==apptViewElement.GetDescription()){
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FillElements. Used to determine whether a apptfield is already displayed. If not, then it is displayed in the apptFieldDef rows on the left.</summary>
		private bool ApptFieldIsDisplayed(long apptFieldDefNum){
			for(int i=0;i<_listApptViewItemsDisplayedAll.Count;i++){
				if(_listApptViewItemsDisplayedAll[i].ApptFieldDefNum==apptFieldDefNum){
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FillElements. Used to determine whether a PatFieldDef is already displayed. If not, then it is displayed in the patFieldDef rows on the left.</summary>
		private bool PatFieldIsDisplayed(long patFieldDefNum){
			for(int i=0;i<_listApptViewItemsDisplayedAll.Count;i++){
				if(_listApptViewItemsDisplayedAll[i].PatFieldDefNum==patFieldDefNum){
					return true;
				}
			}
			return false;
		}

		private void checkOnlyScheduledProvs_Click(object sender,EventArgs e) {
			UpdateDisplayFilterGroup();
		}

		///<summary>Updates the display filter visibility based on the state of checkOnlyScheduledProvs.</summary>
		private void UpdateDisplayFilterGroup(){
			if(checkOnlyScheduledProvs.Checked) {
				labelBeforeTime.Visible=true;
				labelAfterTime.Visible=true;
				textBeforeTime.Visible=true;
				textAfterTime.Visible=true;
			}
			else {
				labelBeforeTime.Visible=false;
				labelAfterTime.Visible=false;
				textBeforeTime.Visible=false;
				textAfterTime.Visible=false;
			}
		}

		public void UpdateMobileViewList(List<ApptViewItem> listUpdatedItems) {
			_listMobileApptViewItems=listUpdatedItems;
		}

		private void butMobileView_Click(object sender,EventArgs e) {
			using FormApptViewEditMobile formMobileApptViewEdit=new FormApptViewEditMobile(ApptViewCur,_listMobileApptViewItems,this);
			formMobileApptViewEdit.ShowDialog();
		}

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length>0) {
				_listApptViewItemsDisplayedAll.Remove(_listApptViewItemsDisplayedMain[gridMain.SelectedIndices[0]]);
			}
			else if(gridUR.SelectedIndices.Length>0) {
				_listApptViewItemsDisplayedAll.Remove(_listApptViewItemsDisplayedUR[gridUR.SelectedIndices[0]]);
			}
			else if(gridLR.SelectedIndices.Length>0) {
				_listApptViewItemsDisplayedAll.Remove(_listApptViewItemsDisplayedLR[gridLR.SelectedIndices[0]]);
			}
			FillElements();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(gridAvailable.GetSelectedIndex()!=-1) {
				//the item order is not used until saving to db.
				string strDescript=_listEnumApptViewElementsAvailable[gridAvailable.GetSelectedIndex()].GetDescription();
				Color color=Color.Black;
				if(DoSetBackgroundColor(strDescript)) {
					//Default background color to White.
					color=Color.White;
				}
				ApptViewItem item=new ApptViewItem(strDescript,0,color);
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedMain[gridMain.GetSelectedIndex()]);
					_listApptViewItemsDisplayedAll.Insert(newIdx,item);
				}
				else {//add to end
					_listApptViewItemsDisplayedAll.Add(item);
				}
				FillElements();
				for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++) {//the new item will always show first in the main list.
					if(_listApptViewItemsDisplayedMain[i]==item) {
						gridMain.SetSelected(i,true);//reselect the item
						break;
					}
				}
			}
			else if(gridApptFieldDefs.GetSelectedIndex()!=-1) {
				ApptViewItem apptViewItem=new ApptViewItem();
				apptViewItem.ElementColor=Color.Black;
				apptViewItem.ApptFieldDefNum=_listApptFieldDefNumsAvailable[gridApptFieldDefs.GetSelectedIndex()];
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedMain[gridMain.GetSelectedIndex()]);
					_listApptViewItemsDisplayedAll.Insert(newIdx,apptViewItem);
				}
				else {//add to end
					_listApptViewItemsDisplayedAll.Add(apptViewItem);
				}
				FillElements();
				for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++) {//the new item will always show first in the main list.
					if(_listApptViewItemsDisplayedMain[i]==apptViewItem) {
						gridMain.SetSelected(i,true);//reselect the item
						break;
					}
				}
			}
			else if(gridPatFieldDefs.GetSelectedIndex()!=-1) {
				ApptViewItem apptViewItem=new ApptViewItem();
				apptViewItem.ElementColor=Color.Black;
				apptViewItem.PatFieldDefNum=_listPatFieldDefNums[gridPatFieldDefs.GetSelectedIndex()]; 
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedMain[gridMain.GetSelectedIndex()]);
					_listApptViewItemsDisplayedAll.Insert(newIdx,apptViewItem);
				}
				else {//add to end
					_listApptViewItemsDisplayedAll.Add(apptViewItem);
				}
				FillElements();
				for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++) {//the new item will always show first in the main list.
					if(_listApptViewItemsDisplayedMain[i]==apptViewItem) {
						gridMain.SetSelected(i,true);//reselect the item
						break;
					}
				}
			}
		}

		private void butUp_Click(object sender,System.EventArgs e) {
			int oldIdx;
			int newIdx;
			int newIdxAll;//within the list of all.
			ApptViewItem apptViewItem;
			if(gridMain.GetSelectedIndex()!=-1) {
				oldIdx=gridMain.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				apptViewItem=_listApptViewItemsDisplayedMain[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedMain[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridMain.SetSelected(newIdx,true);
			}
			else if(gridUR.GetSelectedIndex()!=-1) {
				oldIdx=gridUR.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				apptViewItem=_listApptViewItemsDisplayedUR[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedUR[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridUR.SetSelected(newIdx,true);
			}
			else if(gridLR.GetSelectedIndex()!=-1) {
				oldIdx=gridLR.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				apptViewItem=_listApptViewItemsDisplayedLR[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedLR[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridLR.SetSelected(newIdx,true);
			}
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			int oldIdx;
			int newIdx;
			int newIdxAll;
			ApptViewItem apptViewItem;
			if(gridMain.GetSelectedIndex()!=-1) {
				oldIdx=gridMain.GetSelectedIndex();
				if(oldIdx==_listApptViewItemsDisplayedMain.Count-1) {
					return;//can't move down any more
				}
				apptViewItem=_listApptViewItemsDisplayedMain[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedMain[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridMain.SetSelected(newIdx,true);
			}
			if(gridUR.GetSelectedIndex()!=-1) {
				oldIdx=gridUR.GetSelectedIndex();
				if(oldIdx==_listApptViewItemsDisplayedUR.Count-1) {
					return;//can't move down any more
				}
				apptViewItem=_listApptViewItemsDisplayedUR[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedUR[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridUR.SetSelected(newIdx,true);
			}
			if(gridLR.GetSelectedIndex()!=-1) {
				oldIdx=gridLR.GetSelectedIndex();
				if(oldIdx==_listApptViewItemsDisplayedLR.Count-1) {
					return;//can't move down any more
				}
				apptViewItem=_listApptViewItemsDisplayedLR[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_listApptViewItemsDisplayedAll.IndexOf(_listApptViewItemsDisplayedLR[newIdx]);
				_listApptViewItemsDisplayedAll.Remove(apptViewItem);
				_listApptViewItemsDisplayedAll.Insert(newIdxAll,apptViewItem);
				FillElements();
				gridLR.SetSelected(newIdx,true);
			}
		}

		private void gridAvailable_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridAvailable.SelectedIndices.Length>0) {
				gridApptFieldDefs.SetAll(false);
				gridPatFieldDefs.SetAll(false);
			}
		}

		private void gridApptFieldDefs_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridApptFieldDefs.SelectedIndices.Length>0) {
				gridAvailable.SetAll(false);
				gridPatFieldDefs.SetAll(false);
			}
		}

		private void gridPatFieldDefs_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridPatFieldDefs.SelectedIndices.Length>0) {
				gridAvailable.SetAll(false);
				gridApptFieldDefs.SetAll(false);
			}
		}

		private void gridMain_CellClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(gridMain.SelectedIndices.Length>0) {
				gridUR.SetAll(false);
				gridLR.SetAll(false);
			}
		}

		private void gridUR_CellClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(gridUR.SelectedIndices.Length>0) {
				gridMain.SetAll(false);
				gridLR.SetAll(false);
			}
		}

		private void gridLR_CellClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			if(gridLR.SelectedIndices.Length>0) {
				gridUR.SetAll(false);
				gridMain.SetAll(false);
			}
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			FrmApptViewItemEdit frmApptViewItemEdit=new FrmApptViewItemEdit();
			frmApptViewItemEdit.ApptViewItemCur=_listApptViewItemsDisplayedMain[e.Row];
			frmApptViewItemEdit.ShowDialog();
			FillElements();
			ReselectItem(frmApptViewItemEdit.ApptViewItemCur);
		}

		private void gridUR_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			FrmApptViewItemEdit frmApptViewItemEdit=new FrmApptViewItemEdit();
			frmApptViewItemEdit.ApptViewItemCur=_listApptViewItemsDisplayedUR[e.Row];
			frmApptViewItemEdit.ShowDialog();
			FillElements();
			ReselectItem(frmApptViewItemEdit.ApptViewItemCur);
		}

		private void gridLR_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			FrmApptViewItemEdit frmApptViewItemEdit=new FrmApptViewItemEdit();
			frmApptViewItemEdit.ApptViewItemCur=_listApptViewItemsDisplayedLR[e.Row];
			frmApptViewItemEdit.ShowDialog();
			FillElements();
			ReselectItem(frmApptViewItemEdit.ApptViewItemCur);
		}

		///<summary>Returns true if FillGrid sets the background color.</summary>
		private bool DoSetBackgroundColor(string apptItemDescription) {
			return apptItemDescription.In(
				EnumApptViewElement.MedOrPremed_plus.GetDescription(),
				EnumApptViewElement.HasIns_I.GetDescription(),
				EnumApptViewElement.InsToSend_excl.GetDescription(),
				EnumApptViewElement.RecallPastDue_R.GetDescription(),
				EnumApptViewElement.ProphyPerioPastDue_P.GetDescription(),
				EnumApptViewElement.LateColor.GetDescription());
		}

		///<summary>Returns the ApptFieldDef.FieldName with the tag (hidden) if the ApptFieldDef is hidden. Otherwise return the FieldName.</summary>
		private string MarkFieldNameIfHidden(long apptFieldDefNum) {
			if(FieldDefLinks.GetExists(x => x.FieldDefNum==apptFieldDefNum && x.FieldDefType==FieldDefTypes.Appointment)) {
				return ApptFieldDefs.GetFieldName(apptFieldDefNum)+" (Hidden)";
			}
			return ApptFieldDefs.GetFieldName(apptFieldDefNum);
		}

		///<summary>When we know what item we want to select, but we don't know which of the three areas it might now be in.</summary>
		private void ReselectItem(ApptViewItem apptViewItem){
			//another way of doing this would be to test which area it was in first, but that wouldn't make the code more compact.
			for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++) {
				if(_listApptViewItemsDisplayedMain[i]==apptViewItem) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
			for(int i=0;i<_listApptViewItemsDisplayedUR.Count;i++) {
				if(_listApptViewItemsDisplayedUR[i]==apptViewItem) {
					gridUR.SetSelected(i,true);
					break;
				}
			}
			for(int i=0;i<_listApptViewItemsDisplayedLR.Count;i++) {
				if(_listApptViewItemsDisplayedLR[i]==apptViewItem) {
					gridLR.SetSelected(i,true);
					break;
				}
			}
		}

		private void textRowsPerIncr_Validating(object sender, System.ComponentModel.CancelEventArgs e) {
			try{
				Convert.ToInt32(textRowsPerIncr.Text);
			}
			catch{
				MessageBox.Show(Lan.g(this,"Must be a number between 1 and 3."));
				e.Cancel=true;
				return;
			}
			if(PIn.Long(textRowsPerIncr.Text)<1 || PIn.Long(textRowsPerIncr.Text)>3){
				MessageBox.Show(Lan.g(this,"Must be a number between 1 and 3."));
				e.Cancel=true;
			}
		}

		///<summary>This will remove operatories from the list of ops available to assign to this view and fill the list with ops assigned to the same clinic or unassigned.  If the current view has operatories selected that are assigned to a different view.</summary>
		private void comboClinic_SelectionChangeCommitted(object sender,EventArgs e) {
			FillOperatories();
			butMobileView.Visible=MobileAppDevices.IsClinicSignedUpForMobileWeb(comboClinic.ClinicNumSelected);
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//this does mess up the item orders a little, but missing numbers don't actually hurt anything.
			if(MessageBox.Show(Lan.g(this,"Delete this category?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			ApptViewItems.DeleteAllForView(ApptViewCur);
			ApptViewItems.DeleteAllForView(ApptViewCur,isMobile:true);//deleting the OD Proper view, need to also delete the mobile version of it.
			ApptViews.Delete(ApptViewCur);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(listProv.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one provider must be selected.");
				return;
			}
			if(listOps.SelectedIndices.Count==0){// && !checkOnlyScheduledProvs.Checked) {
				MsgBox.Show(this,"At least one operatory must be selected.");
				return;
			}
			if(textDescription.Text==""){
				MessageBox.Show(Lan.g(this,"A description must be entered."));
				return;
			}
			int widthOpMinimum=0;
			try{
				widthOpMinimum=System.Convert.ToInt32(textWidthOpMinimum.Text);
				if(widthOpMinimum<0 || widthOpMinimum>2000){
					throw new Exception();
				}
			}
			catch{
				MsgBox.Show(this,"Invalid Minimum Op width.");//seems silly to tell them it could be as high as 2000
				return;
			}
			if(_listApptViewItemsDisplayedMain.Count==0){
				MessageBox.Show(Lan.g(this,"At least one row type must be displayed."));
				return;
			}
			DateTime timeBefore=new DateTime();//only the time portion will be used.
			if(checkOnlyScheduledProvs.Checked && textBeforeTime.Text!="") {
				try {
					timeBefore=DateTime.Parse(textBeforeTime.Text);
				}
				catch {
					MsgBox.Show(this,"Time before invalid.");
					return;
				}
			}
			DateTime timeAfter=new DateTime();
			if(checkOnlyScheduledProvs.Checked && textAfterTime.Text!="") {
				try {
					timeAfter=DateTime.Parse(textAfterTime.Text);
				}
				catch {
					MsgBox.Show(this,"Time after invalid.");
					return;
				}
			}
			DateTime timeScroll=new DateTime();
			if(textScrollTime.Text=="") {
				timeScroll=DateTime.Parse("08:00:00");
			}
			else {
				try {
					timeScroll=DateTime.Parse(textScrollTime.Text);
				}
				catch {
					MsgBox.Show(this,"Scroll start time invalid.");
					return;
				}
			}
			//start with a clean slate
			ApptViewItems.DeleteAllForView(ApptViewCur);
			ApptViewItems.DeleteAllForView(ApptViewCur,isMobile:true);
			ApptViewItem apptViewItem;
			bool isClinicMobile=MobileAppDevices.IsClinicSignedUpForMobileWeb(comboClinic.ClinicNumSelected);
			for(int i=0;i<_listOpNums.Count;i++){
				if(listOps.SelectedIndices.Contains(i)){
					apptViewItem=new ApptViewItem();
					apptViewItem.ApptViewNum=ApptViewCur.ApptViewNum;
					apptViewItem.OpNum=_listOpNums[i];
					apptViewItem.IsMobile=false;
					ApptViewItems.Insert(apptViewItem);
					//if they are signed up for mobile, save a mobile version of operatory selection, since they cannot be changed in mobile appt view edit window.
					//If there are no mobile view items, then the mobile view doesn't exist so don't add the operatory.
					if(isClinicMobile && _listMobileApptViewItems.Count!=0) {
						apptViewItem.IsMobile=true;
						ApptViewItems.Insert(apptViewItem);
					}
				}
			}
			for(int i=0;i<_listProviders.Count;i++){
				if(listProv.SelectedIndices.Contains(i)){
					apptViewItem=new ApptViewItem();
					apptViewItem.ApptViewNum=ApptViewCur.ApptViewNum;
					apptViewItem.ProvNum=_listProviders[i].ProvNum;
					apptViewItem.IsMobile=false;
					ApptViewItems.Insert(apptViewItem);
					//if they are signed up for mobile, save a mobile version of provider selection, since they cannot be changed in mobile appt view edit window.
					//If there are no mobile view items, then the mobile view doesn't exist so don't add the provider.
					if(isClinicMobile && _listMobileApptViewItems.Count!=0) {
						apptViewItem.IsMobile=true;
						ApptViewItems.Insert(apptViewItem);
					}
				}
			}
			ApptViewCur.StackBehavUR=(ApptViewStackBehavior)listStackUR.SelectedIndex;
			ApptViewCur.StackBehavLR=(ApptViewStackBehavior)listStackLR.SelectedIndex;
			for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++){
				apptViewItem=_listApptViewItemsDisplayedMain[i];
				apptViewItem.ApptViewNum=ApptViewCur.ApptViewNum;
				//elementDesc, elementColor, and Alignment already handled.
				apptViewItem.ElementOrder=(byte)i;
				apptViewItem.IsMobile=false;
				ApptViewItems.Insert(apptViewItem);
			}
			for(int i=0;i<_listApptViewItemsDisplayedUR.Count;i++) {
				apptViewItem=_listApptViewItemsDisplayedUR[i];
				apptViewItem.ApptViewNum=ApptViewCur.ApptViewNum;
				apptViewItem.ElementOrder=(byte)i;
				apptViewItem.IsMobile=false;
				ApptViewItems.Insert(apptViewItem);
			}
			for(int i=0;i<_listApptViewItemsDisplayedLR.Count;i++) {
				apptViewItem=_listApptViewItemsDisplayedLR[i];
				apptViewItem.ApptViewNum=ApptViewCur.ApptViewNum;
				apptViewItem.ElementOrder=(byte)i;
				apptViewItem.IsMobile=false;
				ApptViewItems.Insert(apptViewItem);
			}
			if(isClinicMobile) {
				ApptViewItem apptViewItemMobile;
				for(int i=0;i<_listMobileApptViewItems.Count;i++) {
					//Only add the ApptViewItems that do not include OpNum or ProvNum. This includes ElementDesc, ApptFieldDefNum, and PatFieldDefNum .
					if(_listMobileApptViewItems[i].ProvNum==0 && _listMobileApptViewItems[i].OpNum==0) {
						apptViewItemMobile=_listMobileApptViewItems[i];
						apptViewItemMobile.ApptViewNum=ApptViewCur.ApptViewNum;
						apptViewItemMobile.ElementOrder=(byte)i;
						apptViewItemMobile.IsMobile=true;
						ApptViewItems.Insert(apptViewItemMobile);
					}
				}
			}
			ApptViewCur.WaitingRmName=listWaitingRmNameFormat.GetSelected<EnumWaitingRmName>();
			ApptViewCur.Description=textDescription.Text;
			ApptViewCur.RowsPerIncr=PIn.Byte(textRowsPerIncr.Text);//already validated
			ApptViewCur.WidthOpMinimum=widthOpMinimum;
			ApptViewCur.ApptTimeScrollStart=timeScroll.TimeOfDay;
			ApptViewCur.IsScrollStartDynamic=checkDynamicScroll.Checked;
			ApptViewCur.IsApptBubblesDisabled=checkApptBubblesDisabled.Checked;
			ApptViewCur.OnlyScheduledProvs=checkOnlyScheduledProvs.Checked;
			ApptViewCur.OnlyScheduledProvDays=checkOnlyScheduledProvDays.Checked;
			ApptViewCur.OnlySchedBeforeTime=timeBefore.TimeOfDay;
			ApptViewCur.OnlySchedAfterTime=timeAfter.TimeOfDay;
			long clinicOld=ApptViewCur.ClinicNum;
			ApptViewCur.ClinicNum=0;//Default is all clinics
			if(PrefC.HasClinicsEnabled) {
				//_listUserClinicNums will contain only a 0 if the clinics show feature is disabled.
				//If the user is not restricted to a clinic, the list will contain 0 in the first position since comboClinic will contain 'All' as the first option.
				//Restricted users (Security.CurUser.ClinicsIsRestricted=true && Security.CurUser.ClinicNum>0) won't have access to the unassigned views (AssignedClinic=0)
				ApptViewCur.ClinicNum=comboClinic.ClinicNumSelected;
			}
			//User just moved this appointment view to a different clinic and this view was associated to the current Computer. Clear it out.
			if(ApptViewCur.ClinicNum!=clinicOld && ComputerPrefs.LocalComputer.ApptViewNum==ApptViewCur.ApptViewNum) {
				ComputerPrefs.LocalComputer.ApptViewNum=0;
				ComputerPrefs.Update(ComputerPrefs.LocalComputer);
				UserodApptViews.InsertOrUpdate(Security.CurUser.UserNum,clinicOld,0);
			}
			ApptViews.Update(ApptViewCur);//same whether isnew or not
			DialogResult=DialogResult.OK;
		}

		private void FormApptViewEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew){
				ApptViewItems.DeleteAllForView(ApptViewCur);
				ApptViewItems.DeleteAllForView(ApptViewCur,isMobile:true);
				ApptViews.Delete(ApptViewCur);
			}
		}
	}
}