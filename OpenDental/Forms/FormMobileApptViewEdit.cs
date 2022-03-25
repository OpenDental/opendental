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
	public partial class FormMobileApptViewEdit : FormODBase {
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
		private List<ApptViewItem> _listApptViewItemsDisplayedUR;
		private List<ApptViewItem> _listApptViewItemsDisplayedLR;
		///<summary>Set this value before opening the form.</summary>
		private ApptView _apptViewCur;
		///<summary>Set this value with the clinic selected in FormApptViews. Only to be used to set comboClinic.SelectedClinicNum.</summary>
		private long _clinicNumInitial;
		///<summary>List of all ApptViewItems for this view.  All 5 types.</summary>
		private List<ApptViewItem> _listApptViewItems;
		///<summary>List of ApptViewItems that does not include OpNum or ProvNum. This includes ElementDesc, ApptFieldDefNum, and PatFieldDefNum.</summary>
		private List<ApptViewItem> _listApptViewItemsDef;
		///<summary>This is a list of all operatories available to add to this view based on AssignedClinicNum and the clinic the ops are assigned to.  If the clinics show feature is turned off (EasyNoClinics=true) or if the view is not assigned to a clinic, all unhidden ops will be available.  If an op is not assigned to a clinic, it will only be available to add to views also not assigned to a clinic.  If the view is assigned to a clinic, ops assigned to the same clinic will be available to add to the view.</summary>
		private List<long> _listOpNums;
		private List<Provider> _listProviders;
		///<summary>This list is used to check the ApptFieldDefs if they are hidden. That way they won't be added to the grids.</summary>
		private List<FieldDefLink> _listFieldDefLinks;

		///<summary></summary>
		public FormMobileApptViewEdit(ApptView apptView, long clinicNum)
		{
			_apptViewCur=apptView;
			_clinicNumInitial=clinicNum;
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormApptViewEdit_Load(object sender, System.EventArgs e) {
			_listApptViewItems=ApptViewItems.GetWhere(x => x.ApptViewNum==_apptViewCur.ApptViewNum && x.IsMobile);
			_listApptViewItemsDef=_listApptViewItems.FindAll(x => x.OpNum==0 && x.ProvNum==0);
			_listApptViewItemsDisplayedAll=new List<ApptViewItem>(_listApptViewItemsDef);
			_listFieldDefLinks=FieldDefLinks.GetAll();
			FillElements();
		}

		///<summary>Fills the five lists based on the displayedElements lists. No database transactions are performed here.</summary>
		private void FillElements(){
			_listApptViewItemsDisplayedMain=new List<ApptViewItem>();
			for(int i=0;i<_listApptViewItemsDisplayedAll.Count;i++) {
				if(_listApptViewItemsDisplayedAll[i].ElementAlignment==ApptViewAlignment.Main) {
					_listApptViewItemsDisplayedMain.Add(_listApptViewItemsDisplayedAll[i]);
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
				if(_listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.MedOrPremed_plus.GetDescription()
					|| _listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.HasIns_I.GetDescription()
					|| _listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.InsToSend_excl.GetDescription()
					|| _listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.RecallPastDue_R.GetDescription()
					|| _listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.ProphyPerioPastDue_P.GetDescription()
					|| _listApptViewItemsDisplayedMain[i].ElementDesc==EnumApptViewElement.LateColor.GetDescription())
				{
					row.ColorBackG=_listApptViewItemsDisplayedMain[i].ElementColor;
				}
				else{
					row.ColorText=_listApptViewItemsDisplayedMain[i].ElementColor;
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			//gridAvailable-----------------------------------------------------------
			gridAvailable.BeginUpdate();
			gridAvailable.Columns.Clear();
			col=new GridColumn("",100);
			gridAvailable.Columns.Add(col);
			gridAvailable.ListGridRows.Clear();
			_listEnumApptViewElementsAvailable=new List<EnumApptViewElement>();
			for(int i=0;i<Enum.GetValues(typeof(EnumApptViewElement)).Length;i++){
				if(((EnumApptViewElement)i)==EnumApptViewElement.None
						|| ((EnumApptViewElement)i)==EnumApptViewElement.ConfirmedColor // ConfirmedColor and ProcsColored are not currently supported for mobile appt views
						|| ((EnumApptViewElement)i)==EnumApptViewElement.ProcsColored)
				{
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

		private void butLeft_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length>0) {
				_listApptViewItemsDisplayedAll.Remove(_listApptViewItemsDisplayedMain[gridMain.SelectedIndices[0]]);
			}
			FillElements();
		}

		private void butRight_Click(object sender, System.EventArgs e) {
			if(gridAvailable.GetSelectedIndex()!=-1) {
				//the item order is not used until saving to db.
				string strDescript=_listEnumApptViewElementsAvailable[gridAvailable.GetSelectedIndex()].GetDescription();
				Color color=Color.Black;
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

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			using FormApptViewItemEdit formApptViewItemEdit=new FormApptViewItemEdit();
			formApptViewItemEdit.ApptViewItemCur=_listApptViewItemsDisplayedMain[e.Row];
			formApptViewItemEdit.ShowDialog();
			FillElements();
			ReselectItem(formApptViewItemEdit.ApptViewItemCur);
		}

		///<summary>Returns the ApptFieldDef.FieldName with the tag (hidden) if the ApptFieldDef is hidden. Otherwise return the FieldName.</summary>
		private string MarkFieldNameIfHidden(long apptFieldDefNum) {
			if(_listFieldDefLinks.Exists(x => x.FieldDefNum==apptFieldDefNum && x.FieldDefType==FieldDefTypes.Appointment)) {
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
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			//this does mess up the item orders a little, but missing numbers don't actually hurt anything.
			if(MessageBox.Show(Lan.g(this,"Delete mobile view?"),"",MessageBoxButtons.OKCancel)
				!=DialogResult.OK){
				return;
			}
			ApptViewItems.DeleteAllForView(_apptViewCur);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(_listApptViewItemsDisplayedMain.Count==0){
				MessageBox.Show(Lan.g(this,"At least one row type must be displayed."));
				return;
			}
			ApptViewItems.DeleteAllForView(_apptViewCur,isMobile:true);//start with a clean slate
			ApptViewItem apptViewItem;
			for(int i=0;i<_listApptViewItemsDisplayedMain.Count;i++){
				apptViewItem=_listApptViewItemsDisplayedMain[i];
				apptViewItem.ApptViewNum=_apptViewCur.ApptViewNum;
				apptViewItem.ElementOrder=(byte)i;
				apptViewItem.IsMobile=true;
				ApptViewItems.Insert(apptViewItem);
			}
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
				ApptViewItems.DeleteAllForView(_apptViewCur);
			}
		}


	}
}





















