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
		private List<EnumApptViewElement> displayedAvailable;
		///<summary>The actual ApptFieldDefNums of all available elements because no language translation is needed.</summary>
		private List<long> displayedAvailableApptFieldDefs;
		///<summary>The actual PatFieldDefNums of all available elements because no language translation is needed.</summary>
		private List<long> _listPatFieldDefNums;
		///<summary>A local list of ApptViewItems which are displayed in all three lists on the right.  Not updated to db until the form is closed.</summary>
		private List<ApptViewItem> _displayedElementsAll;
		private List<ApptViewItem> displayedElementsMain;
		private List<ApptViewItem> displayedElementsUR;
		private List<ApptViewItem> displayedElementsLR;
		///<summary>Set this value before opening the form.</summary>
		public ApptView ApptViewCur;
		///<summary>Set this value with the clinic selected in FormApptViews. Only to be used to set comboClinic.SelectedClinicNum.</summary>
		public long ClinicNumInitial;
		///<summary>List of all ApptViewItems for this view.  All 5 types.</summary>
		private List<ApptViewItem> _listApptViewItems;
		///<summary>List of ApptViewItems that does not include OpNum or ProvNum. This includes ElementDesc, ApptFieldDefNum, and PatFieldDefNum.</summary>
		private List<ApptViewItem> _listApptViewItemDefs;
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
			if(ApptViewCur.OnlySchedBeforeTime > new TimeSpan(0,0,0)) {
				textBeforeTime.Text=(DateTime.Today+ApptViewCur.OnlySchedBeforeTime).ToShortTimeString();
			}
			if(ApptViewCur.OnlySchedAfterTime > new TimeSpan(0,0,0)) {
				textAfterTime.Text=(DateTime.Today+ApptViewCur.OnlySchedAfterTime).ToShortTimeString();
			}
			comboClinic.SelectedClinicNum=ClinicNumInitial;
			UpdateDisplayFilterGroup();
			_listApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==ApptViewCur.ApptViewNum);
			_listApptViewItemDefs=_listApptViewItems.FindAll(x => x.OpNum==0 && x.ProvNum==0);
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
			_displayedElementsAll=new List<ApptViewItem>(_listApptViewItemDefs);
			FillElements();
		}

		///<summary>Fills the five lists based on the displayedElements lists. No database transactions are performed here.</summary>
		private void FillElements(){
			displayedElementsMain=new List<ApptViewItem>();
			displayedElementsUR=new List<ApptViewItem>();
			displayedElementsLR=new List<ApptViewItem>();
			for(int i=0;i<_displayedElementsAll.Count;i++) {
				if(_displayedElementsAll[i].ElementAlignment==ApptViewAlignment.Main) {
					displayedElementsMain.Add(_displayedElementsAll[i]);
				}
				else if(_displayedElementsAll[i].ElementAlignment==ApptViewAlignment.UR) {
					displayedElementsUR.Add(_displayedElementsAll[i]);
				}
				else if(_displayedElementsAll[i].ElementAlignment==ApptViewAlignment.LR) {
					displayedElementsLR.Add(_displayedElementsAll[i]);
				}
			}
			//Now fill the lists on the screen--------------------------------------------------
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn("",100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<displayedElementsMain.Count;i++){
				row=new GridRow();
				if(displayedElementsMain[i].ApptFieldDefNum>0){
					row.Cells.Add(ApptFieldDefs.GetFieldName(displayedElementsMain[i].ApptFieldDefNum));
				}
				else if(displayedElementsMain[i].PatFieldDefNum>0){
					row.Cells.Add(PatFieldDefs.GetFieldName(displayedElementsMain[i].PatFieldDefNum));
				}
				else{
					row.Cells.Add(displayedElementsMain[i].ElementDesc);
				}
				if(displayedElementsMain[i].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
					|| displayedElementsMain[i].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
					|| displayedElementsMain[i].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
					|| displayedElementsMain[i].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
					|| displayedElementsMain[i].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()
					|| displayedElementsMain[i].ElementDesc==EnumApptViewElement.LateColor.GetDescription())
				{
					row.ColorBackG=displayedElementsMain[i].ElementColor;
				}
				else{
					row.ColorText=displayedElementsMain[i].ElementColor;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//gridUR---------------------------------------------------------
			gridUR.BeginUpdate();
			gridUR.ListGridColumns.Clear();
			col=new GridColumn("",100);
			gridUR.ListGridColumns.Add(col);
			gridUR.ListGridRows.Clear();
			for(int i=0;i<displayedElementsUR.Count;i++) {
				row=new GridRow();
				if(displayedElementsUR[i].ApptFieldDefNum>0) {
					row.Cells.Add(ApptFieldDefs.GetFieldName(displayedElementsUR[i].ApptFieldDefNum));
				}
				else if(displayedElementsUR[i].PatFieldDefNum>0) {
					row.Cells.Add(PatFieldDefs.GetFieldName(displayedElementsUR[i].PatFieldDefNum));
				}
				else {
					row.Cells.Add(displayedElementsUR[i].ElementDesc);
				}
				if(displayedElementsUR[i].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
					|| displayedElementsUR[i].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
					|| displayedElementsUR[i].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
					|| displayedElementsUR[i].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
					|| displayedElementsUR[i].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()
					|| displayedElementsUR[i].ElementDesc==EnumApptViewElement.LateColor.GetDescription())
				{
					row.ColorBackG=displayedElementsUR[i].ElementColor;
				}
				else{
					row.ColorText=displayedElementsUR[i].ElementColor;
				}
				gridUR.ListGridRows.Add(row);
			}
			gridUR.EndUpdate();
			//gridLR-----------------------------------------------------------
			gridLR.BeginUpdate();
			gridLR.ListGridColumns.Clear();
			col=new GridColumn("",100);
			gridLR.ListGridColumns.Add(col);
			gridLR.ListGridRows.Clear();
			for(int i=0;i<displayedElementsLR.Count;i++) {
				row=new GridRow();
				if(displayedElementsLR[i].ApptFieldDefNum>0) {
					row.Cells.Add(ApptFieldDefs.GetFieldName(displayedElementsLR[i].ApptFieldDefNum));
				}
				else if(displayedElementsLR[i].PatFieldDefNum>0) {
					row.Cells.Add(PatFieldDefs.GetFieldName(displayedElementsLR[i].PatFieldDefNum));
				}
				else {
					row.Cells.Add(displayedElementsLR[i].ElementDesc);
				}
				if(displayedElementsLR[i].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
					|| displayedElementsLR[i].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
					|| displayedElementsLR[i].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
					|| displayedElementsLR[i].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
					|| displayedElementsLR[i].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()
					|| displayedElementsLR[i].ElementDesc==EnumApptViewElement.LateColor.GetDescription())
				{
					row.ColorBackG=displayedElementsLR[i].ElementColor;
				}
				else{
					row.ColorText=displayedElementsLR[i].ElementColor;
				}
				gridLR.ListGridRows.Add(row);
			}
			gridLR.EndUpdate();
			//gridAvailable-----------------------------------------------------------
			gridAvailable.BeginUpdate();
			gridAvailable.ListGridColumns.Clear();
			col=new GridColumn("",100);
			gridAvailable.ListGridColumns.Add(col);
			gridAvailable.ListGridRows.Clear();
			displayedAvailable=new List<EnumApptViewElement>();
			for(int i=0;i<Enum.GetValues(typeof(EnumApptViewElement)).Length;i++){
				if(((EnumApptViewElement)i)==EnumApptViewElement.None){
					continue;//none is not available for display and won't show anywhere in the UI
				}
				if(!ElementIsDisplayed((EnumApptViewElement)i)) {
					displayedAvailable.Add((EnumApptViewElement)i);
					row=new GridRow();
					row.Cells.Add(Lan.g(this,((EnumApptViewElement)i).GetDescription()));
					gridAvailable.ListGridRows.Add(row);
				}
			}
			gridAvailable.EndUpdate();
			//gridApptFieldDefs-----------------------------------------------------------
			gridApptFieldDefs.BeginUpdate();
			gridApptFieldDefs.ListGridColumns.Clear();
			col=new GridColumn("",100);
			gridApptFieldDefs.ListGridColumns.Add(col);
			gridApptFieldDefs.ListGridRows.Clear();
			displayedAvailableApptFieldDefs=new List<long>();
			List<ApptFieldDef> listApptFieldDefs=ApptFieldDefs.GetDeepCopy();
			for(int i=0;i<listApptFieldDefs.Count;i++) {
				if(!ApptFieldIsDisplayed(listApptFieldDefs[i].ApptFieldDefNum)) {
					displayedAvailableApptFieldDefs.Add(listApptFieldDefs[i].ApptFieldDefNum);
					row=new GridRow();
					row.Cells.Add(listApptFieldDefs[i].FieldName);
					gridApptFieldDefs.ListGridRows.Add(row);
				}
			}
			gridApptFieldDefs.EndUpdate();
			//gridPatFieldDefs-----------------------------------------------------------
			gridPatFieldDefs.BeginUpdate();
			gridPatFieldDefs.ListGridColumns.Clear();
			col=new GridColumn("",100);
			gridPatFieldDefs.ListGridColumns.Add(col);
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
					|| comboClinic.SelectedClinicNum==0 //or this view is not assigned to a clinic
					|| listOperatories[i].ClinicNum==comboClinic.SelectedClinicNum) //or the operatory is assigned to same clinic as this view
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
			for(int i=0;i<_displayedElementsAll.Count;i++){
				if(_displayedElementsAll[i].ApptFieldDefNum!=0 || _displayedElementsAll[i].PatFieldDefNum!=0){
					continue;
				}
				if(_displayedElementsAll[i].ElementDesc==apptViewElement.GetDescription()){
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FillElements. Used to determine whether a apptfield is already displayed. If not, then it is displayed in the apptFieldDef rows on the left.</summary>
		private bool ApptFieldIsDisplayed(long apptFieldDefNum){
			for(int i=0;i<_displayedElementsAll.Count;i++){
				if(_displayedElementsAll[i].ApptFieldDefNum==apptFieldDefNum){
					return true;
				}
			}
			return false;
		}

		///<summary>Called from FillElements. Used to determine whether a PatFieldDef is already displayed. If not, then it is displayed in the patFieldDef rows on the left.</summary>
		private bool PatFieldIsDisplayed(long patFieldDefNum){
			for(int i=0;i<_displayedElementsAll.Count;i++){
				if(_displayedElementsAll[i].PatFieldDefNum==patFieldDefNum){
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

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length>0) {
				_displayedElementsAll.Remove(displayedElementsMain[gridMain.SelectedIndices[0]]);
			}
			else if(gridUR.SelectedIndices.Length>0) {
				_displayedElementsAll.Remove(displayedElementsUR[gridUR.SelectedIndices[0]]);
			}
			else if(gridLR.SelectedIndices.Length>0) {
				_displayedElementsAll.Remove(displayedElementsLR[gridLR.SelectedIndices[0]]);
			}
			FillElements();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(gridAvailable.GetSelectedIndex()!=-1) {
				//the item order is not used until saving to db.
				string strDescript=displayedAvailable[gridAvailable.GetSelectedIndex()].GetDescription();
				Color color=Color.Black;
				ApptViewItem item=new ApptViewItem(strDescript,0,color);
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_displayedElementsAll.IndexOf(displayedElementsMain[gridMain.GetSelectedIndex()]);
					_displayedElementsAll.Insert(newIdx,item);
				}
				else {//add to end
					_displayedElementsAll.Add(item);
				}
				FillElements();
				for(int i=0;i<displayedElementsMain.Count;i++) {//the new item will always show first in the main list.
					if(displayedElementsMain[i]==item) {
						gridMain.SetSelected(i,true);//reselect the item
						break;
					}
				}
			}
			else if(gridApptFieldDefs.GetSelectedIndex()!=-1) {
				ApptViewItem item=new ApptViewItem();
				item.ElementColor=Color.Black;
				item.ApptFieldDefNum=displayedAvailableApptFieldDefs[gridApptFieldDefs.GetSelectedIndex()];
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_displayedElementsAll.IndexOf(displayedElementsMain[gridMain.GetSelectedIndex()]);
					_displayedElementsAll.Insert(newIdx,item);
				}
				else {//add to end
					_displayedElementsAll.Add(item);
				}
				FillElements();
				for(int i=0;i<displayedElementsMain.Count;i++) {//the new item will always show first in the main list.
					if(displayedElementsMain[i]==item) {
						gridMain.SetSelected(i,true);//reselect the item
						break;
					}
				}
			}
			else if(gridPatFieldDefs.GetSelectedIndex()!=-1) {
				ApptViewItem item=new ApptViewItem();
				item.ElementColor=Color.Black;
				item.PatFieldDefNum=_listPatFieldDefNums[gridPatFieldDefs.GetSelectedIndex()]; 
				if(gridMain.SelectedIndices.Length==1) {//insert
					int newIdx=_displayedElementsAll.IndexOf(displayedElementsMain[gridMain.GetSelectedIndex()]);
					_displayedElementsAll.Insert(newIdx,item);
				}
				else {//add to end
					_displayedElementsAll.Add(item);
				}
				FillElements();
				for(int i=0;i<displayedElementsMain.Count;i++) {//the new item will always show first in the main list.
					if(displayedElementsMain[i]==item) {
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
			ApptViewItem item;
			if(gridMain.GetSelectedIndex()!=-1) {
				oldIdx=gridMain.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				item=displayedElementsMain[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsMain[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
				FillElements();
				gridMain.SetSelected(newIdx,true);
			}
			else if(gridUR.GetSelectedIndex()!=-1) {
				oldIdx=gridUR.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				item=displayedElementsUR[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsUR[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
				FillElements();
				gridUR.SetSelected(newIdx,true);
			}
			else if(gridLR.GetSelectedIndex()!=-1) {
				oldIdx=gridLR.GetSelectedIndex();
				if(oldIdx==0) {
					return;//can't move up any more
				}
				item=displayedElementsLR[oldIdx];
				newIdx=oldIdx-1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsLR[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
				FillElements();
				gridLR.SetSelected(newIdx,true);
			}
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			int oldIdx;
			int newIdx;
			int newIdxAll;
			ApptViewItem item;
			if(gridMain.GetSelectedIndex()!=-1) {
				oldIdx=gridMain.GetSelectedIndex();
				if(oldIdx==displayedElementsMain.Count-1) {
					return;//can't move down any more
				}
				item=displayedElementsMain[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsMain[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
				FillElements();
				gridMain.SetSelected(newIdx,true);
			}
			if(gridUR.GetSelectedIndex()!=-1) {
				oldIdx=gridUR.GetSelectedIndex();
				if(oldIdx==displayedElementsUR.Count-1) {
					return;//can't move down any more
				}
				item=displayedElementsUR[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsUR[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
				FillElements();
				gridUR.SetSelected(newIdx,true);
			}
			if(gridLR.GetSelectedIndex()!=-1) {
				oldIdx=gridLR.GetSelectedIndex();
				if(oldIdx==displayedElementsLR.Count-1) {
					return;//can't move down any more
				}
				item=displayedElementsLR[oldIdx];
				newIdx=oldIdx+1;
				newIdxAll=_displayedElementsAll.IndexOf(displayedElementsLR[newIdx]);
				_displayedElementsAll.Remove(item);
				_displayedElementsAll.Insert(newIdxAll,item);
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
			using FormApptViewItemEdit formA=new FormApptViewItemEdit();
			formA.ApptVItem=displayedElementsMain[e.Row];
			formA.ShowDialog();
			FillElements();
			ReselectItem(formA.ApptVItem);
		}

		private void gridUR_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormApptViewItemEdit formA=new FormApptViewItemEdit();
			formA.ApptVItem=displayedElementsUR[e.Row];
			formA.ShowDialog();
			FillElements();
			ReselectItem(formA.ApptVItem);
		}

		private void gridLR_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormApptViewItemEdit formA=new FormApptViewItemEdit();
			formA.ApptVItem=displayedElementsLR[e.Row];
			formA.ShowDialog();
			FillElements();
			ReselectItem(formA.ApptVItem);
		}

		///<summary>When we know what item we want to select, but we don't know which of the three areas it might now be in.</summary>
		private void ReselectItem(ApptViewItem item){
			//another way of doing this would be to test which area it was in first, but that wouldn't make the code more compact.
			for(int i=0;i<displayedElementsMain.Count;i++) {
				if(displayedElementsMain[i]==item) {
					gridMain.SetSelected(i,true);
					break;
				}
			}
			for(int i=0;i<displayedElementsUR.Count;i++) {
				if(displayedElementsUR[i]==item) {
					gridUR.SetSelected(i,true);
					break;
				}
			}
			for(int i=0;i<displayedElementsLR.Count;i++) {
				if(displayedElementsLR[i]==item) {
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
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//this does mess up the item orders a little, but missing numbers don't actually hurt anything.
			if(MessageBox.Show(Lan.g(this,"Delete this category?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			ApptViewItems.DeleteAllForView(ApptViewCur);
			ApptViews.Delete(ApptViewCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
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
			if(displayedElementsMain.Count==0){
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
			ApptViewItems.DeleteAllForView(ApptViewCur);//start with a clean slate
			ApptViewItem item;
			for(int i=0;i<_listOpNums.Count;i++){
				if(listOps.SelectedIndices.Contains(i)){
					item=new ApptViewItem();
					item.ApptViewNum=ApptViewCur.ApptViewNum;
					item.OpNum=_listOpNums[i];
					ApptViewItems.Insert(item);
				}
			}
			for(int i=0;i<_listProviders.Count;i++){
				if(listProv.SelectedIndices.Contains(i)){
					item=new ApptViewItem();
					item.ApptViewNum=ApptViewCur.ApptViewNum;
					item.ProvNum=_listProviders[i].ProvNum;
					ApptViewItems.Insert(item);
				}
			}
			ApptViewCur.StackBehavUR=(ApptViewStackBehavior)listStackUR.SelectedIndex;
			ApptViewCur.StackBehavLR=(ApptViewStackBehavior)listStackLR.SelectedIndex;
			for(int i=0;i<displayedElementsMain.Count;i++){
				item=displayedElementsMain[i];
				item.ApptViewNum=ApptViewCur.ApptViewNum;
				//elementDesc, elementColor, and Alignment already handled.
				item.ElementOrder=(byte)i;
				ApptViewItems.Insert(item);
			}
			for(int i=0;i<displayedElementsUR.Count;i++) {
				item=displayedElementsUR[i];
				item.ApptViewNum=ApptViewCur.ApptViewNum;
				item.ElementOrder=(byte)i;
				ApptViewItems.Insert(item);
			}
			for(int i=0;i<displayedElementsLR.Count;i++) {
				item=displayedElementsLR[i];
				item.ApptViewNum=ApptViewCur.ApptViewNum;
				item.ElementOrder=(byte)i;
				ApptViewItems.Insert(item);
			}
			ApptViewCur.WaitingRmName=listWaitingRmNameFormat.GetSelected<EnumWaitingRmName>();
			ApptViewCur.Description=textDescription.Text;
			ApptViewCur.RowsPerIncr=PIn.Byte(textRowsPerIncr.Text);//already validated
			ApptViewCur.WidthOpMinimum=widthOpMinimum;
			ApptViewCur.ApptTimeScrollStart=timeScroll.TimeOfDay;
			ApptViewCur.IsScrollStartDynamic=checkDynamicScroll.Checked;
			ApptViewCur.IsApptBubblesDisabled=checkApptBubblesDisabled.Checked;
			ApptViewCur.OnlyScheduledProvs=checkOnlyScheduledProvs.Checked;
			ApptViewCur.OnlySchedBeforeTime=timeBefore.TimeOfDay;
			ApptViewCur.OnlySchedAfterTime=timeAfter.TimeOfDay;
			ApptViewCur.ClinicNum=0;//Default is all clinics
			if(PrefC.HasClinicsEnabled) {
				//_listUserClinicNums will contain only a 0 if the clinics show feature is disabled.
				//If the user is not restricted to a clinic, the list will contain 0 in the first position since comboClinic will contain 'All' as the first option.
				//Restricted users (Security.CurUser.ClinicsIsRestricted=true && Security.CurUser.ClinicNum>0) won't have access to the unassigned views (AssignedClinic=0)
				ApptViewCur.ClinicNum=comboClinic.SelectedClinicNum;
			}
			ApptViews.Update(ApptViewCur);//same whether isnew or not
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormApptViewEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(DialogResult==DialogResult.OK) {
				return;
			}
			if(IsNew){
				ApptViewItems.DeleteAllForView(ApptViewCur);
				ApptViews.Delete(ApptViewCur);
			}
		}
	}
}





















