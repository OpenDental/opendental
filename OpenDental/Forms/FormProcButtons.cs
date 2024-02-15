using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Collections.Generic;
using CodeBase;

namespace OpenDental{
///<summary></summary>
	public partial class FormProcButtons : FormODBase {
		private bool _isChanged;
		///<summary>defnum</summary>
		private long _catSelected;
		///<summary>This list of displayed buttons for the selected cat.</summary>
		private ProcButton[] _procButtonArray;
		private List<ProcButtonQuick> _listProcButtonQuicks;
		private List<Def> _listDefsProcButtonCat;

		///<summary></summary>
		public FormProcButtons(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormChartProcedureEntry_Load(object sender,System.EventArgs e) {
			fillPanelQuickButtons();
			ResizeControls();
			FillCategories();
			FillButtons();
			SetVisibility(); 
		}

		private void SetVisibility() {
			for(int i=0;i<PanelClient.Controls.Count;i++) {//make all controls visible. Then hide below.
				PanelClient.Controls[i].Visible=true;
			}
			if(listCategories.SelectedIndex==0) {
				listViewButtons.Visible=false;
				butAdd.Visible=false;
				butDelete.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
				return;
			}
			panelQuickButtons.Visible=false;
			labelEdit.Visible=false;
		}

		///<summary>Make the QuickButtonGrid exactly the same size as it will display in the chart module.</summary>
		private void ResizeControls() {
			//this code might have done something in the past, but it seems meaningless.  Size of panelQuickButtons does not change in Chart Module.
			/*
			try {
				Control[] controlArray=this.Owner.Controls.Find("ControlChart",true);
				//force redraw and resize of control, Also deselects current patient.
				((ControlChart)controlArray[0]).ModuleSelected(0);
				controlArray=this.Owner.Controls.Find("panelQuickButtons",true);
				//set display size to actual size in from the control module. This is a dynamically sized control.
				LayoutManager.MoveSize(panelQuickButtons,controlArray[0].Size);
				LayoutManager.MoveLocation(labelEdit,new Point(panelQuickButtons.Location.X,panelQuickButtons.Bounds.Bottom+LayoutManager.Scale(20)));
			}
			catch(Exception ex) {
				ex.DoNothing();
				//could not locate the gridquickbuttons control.
			}*/
		}

		private void fillPanelQuickButtons() {
			panelQuickButtons.BeginUpdate();
			panelQuickButtons.ListODPanelItems.Clear();
			_listProcButtonQuicks=ProcButtonQuicks.GetAll();
			_listProcButtonQuicks.Sort(ProcButtonQuicks.sortYX);
			ODPanelItem panelItem;
			for(int i=0;i<_listProcButtonQuicks.Count;i++) {
				panelItem=new ODPanelItem();
				panelItem.Text=_listProcButtonQuicks[i].Description;
				panelItem.YPos=_listProcButtonQuicks[i].YPos;
				panelItem.ItemOrder=_listProcButtonQuicks[i].ItemOrder;
				panelItem.ItemType=(_listProcButtonQuicks[i].IsLabel?ODPanelItemType.Label:ODPanelItemType.Button);
				panelItem.Tags.Add(_listProcButtonQuicks[i]);
				panelQuickButtons.ListODPanelItems.Add(panelItem);
			}
			panelQuickButtons.EndUpdate();
		}

		private void FillCategories(){
			ProcButtonQuicks.ValidateAll();
			listCategories.Items.Clear();
			listCategories.Items.Add("Quick Buttons");//hardcoded category.
			_listDefsProcButtonCat=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true);
			if(_listDefsProcButtonCat.Count==0){
				_catSelected=0;
				listCategories.SelectedIndex=0;
				return;
			}
			for(int i=0;i<_listDefsProcButtonCat.Count;i++){
				listCategories.Items.Add(_listDefsProcButtonCat[i].ItemName);
				if(_catSelected==_listDefsProcButtonCat[i].DefNum){
					listCategories.SelectedIndex=i+1;
				}
			}
			if(listCategories.SelectedIndex==-1){//category was hidden, or just opening the form
				listCategories.SelectedIndex=0;
				_catSelected=0;
			}
			if(listCategories.SelectedIndex>0) {//hardcoded category doesn't have a DefNum.
				_catSelected=_listDefsProcButtonCat[listCategories.SelectedIndex-1].DefNum;
			}
		}

		private void FillButtons(){
			listViewButtons.Items.Clear();
			imageListProcButtons.Images.Clear();
			if(_catSelected==0) {
				//empty button list and return because we will be using and OD grid to display these buttons.
				_procButtonArray=new ProcButton[0];
				return;
			}
			ProcButtons.RefreshCache();
			_procButtonArray=ProcButtons.GetForCat(_catSelected);
			//first check and fix any order problems
			for(int i=0;i<_procButtonArray.Length;i++) {
				if(_procButtonArray[i].ItemOrder!=i) {
					_procButtonArray[i].ItemOrder=i;
					ProcButtons.Update(_procButtonArray[i]);
				}
			}
			ListViewItem listViewItem;
			for(int i=0;i<_procButtonArray.Length;i++) {
				if(_procButtonArray[i].ButtonImage!=""){
					//image keys are simply the ProcButtonNum
					try {
						imageListProcButtons.Images.Add(_procButtonArray[i].ProcButtonNum.ToString(),PIn.Bitmap(_procButtonArray[i].ButtonImage));
					}
					catch {
						imageListProcButtons.Images.Add(new Bitmap(20,20));//Add a blank image so the list stays in synch
					}
				}
				listViewItem=new ListViewItem(new string[] {_procButtonArray[i].Description},_procButtonArray[i].ProcButtonNum.ToString());
				listViewButtons.Items.Add(listViewItem);
			}
		}

		private void listViewButtons_DoubleClick(object sender,EventArgs e) {
			if(listViewButtons.SelectedIndices.Count==0) {//Nothing selected
				return;
			}
			ProcButton procButton=_procButtonArray[listViewButtons.SelectedIndices[0]].Copy();
			using FormProcButtonEdit formProcButtonEdit=new FormProcButtonEdit(procButton);
			formProcButtonEdit.ShowDialog();
			_isChanged=true;
			FillButtons();
		}

		private void listCategories_Click(object sender,EventArgs e) {
			if(listCategories.SelectedIndex==-1){
				return;
			}
			SetVisibility();
			if(listCategories.SelectedIndex==0) {
				_catSelected=0;
			}
			else {
				_catSelected=_listDefsProcButtonCat[listCategories.SelectedIndex-1].DefNum;
			}
			FillButtons();
		}

		private void butEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(EnumPermType.DefEdit)) {
				return;
			}
			using FormDefinitions formDefinitions=new FormDefinitions(DefCat.ProcButtonCats);
			formDefinitions.ShowDialog();
			FillCategories();
			FillButtons();
		}

		private void butDown_Click(object sender, System.EventArgs e) {
			int selected=0;
			if(listViewButtons.SelectedIndices.Count==0){
				return;
			}
			else if(listViewButtons.SelectedIndices[0]==listViewButtons.Items.Count-1){
				return;
			}
			else{
				ProcButton procButton=_procButtonArray[listViewButtons.SelectedIndices[0]].Copy();
				procButton.ItemOrder++;
				ProcButtons.Update(procButton);
				selected=procButton.ItemOrder;
				procButton=_procButtonArray[listViewButtons.SelectedIndices[0]+1].Copy();
				procButton.ItemOrder--;
				ProcButtons.Update(procButton);
			}
			FillButtons();
			_isChanged=true;
			listViewButtons.SelectedIndices.Clear();
			listViewButtons.SelectedIndices.Add(selected);
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			int selected=0;
			if(listViewButtons.SelectedIndices.Count==0){
				return;
			}
			else if(listViewButtons.SelectedIndices[0]==0){
				return; 
			}
			else{
				ProcButton procButton=_procButtonArray[listViewButtons.SelectedIndices[0]].Copy();
				procButton.ItemOrder--;
				ProcButtons.Update(procButton);
				selected=procButton.ItemOrder;
				procButton=_procButtonArray[listViewButtons.SelectedIndices[0]-1].Copy();
				procButton.ItemOrder++;
				ProcButtons.Update(procButton);
			}	
			FillButtons();
			_isChanged=true;
			listViewButtons.SelectedIndices.Clear();
			listViewButtons.SelectedIndices.Add(selected);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(listCategories.SelectedIndex==-1){
				return;
			}
			ProcButton procButton=new ProcButton();
			procButton.Category=_catSelected;
			procButton.ItemOrder=listViewButtons.Items.Count;
			using FormProcButtonEdit formProcButtonEdit=new FormProcButtonEdit(procButton);
			formProcButtonEdit.IsNew=true;
			formProcButtonEdit.ShowDialog();
			_isChanged=true;
			FillButtons();
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listViewButtons.SelectedIndices.Count==0){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ProcButtons.Delete(_procButtonArray[listViewButtons.SelectedIndices[0]]);
			_isChanged=true;
			FillButtons();
		}

		private void FormProcButtons_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_isChanged){
				DataValid.SetInvalid(InvalidType.ProcButtons);
			}
		}

		private void panelQuickButtons_RowDoubleClick(object sender,ODButtonPanelEventArgs e) {
			using FormProcButtonQuickEdit formProcButtonQuickEdit=new FormProcButtonQuickEdit();
			//Search through tags of the ODPanelItem for the PBQ.
			for(int i=0;e.Item!=null && i<e.Item.Tags.Count;i++) {
				if(e.Item.Tags[i].GetType()==typeof(ProcButtonQuick)){
					formProcButtonQuickEdit.ProcButtonQuickCur=(ProcButtonQuick)e.Item.Tags[i];
					break;
				}
			}
			if(formProcButtonQuickEdit.ProcButtonQuickCur==null) {//clicked on either a blank row or to the right of existing buttons on a row.
				formProcButtonQuickEdit.IsNew=true;
				formProcButtonQuickEdit.ProcButtonQuickCur=new ProcButtonQuick();
				formProcButtonQuickEdit.ProcButtonQuickCur.YPos=e.Row;//Set Row
				for(int i=0;i<_listProcButtonQuicks.Count;i++){ //Set ItemOrder
					if(_listProcButtonQuicks[i].YPos!=formProcButtonQuickEdit.ProcButtonQuickCur.YPos //Wrong row
						|| formProcButtonQuickEdit.ProcButtonQuickCur.ItemOrder>_listProcButtonQuicks[i].ItemOrder) { //Already have a larger item order
							continue;
					}
					formProcButtonQuickEdit.ProcButtonQuickCur.ItemOrder=_listProcButtonQuicks[i].ItemOrder+1;//new PBQ should have the highest item order in the row.
				}
			}
			formProcButtonQuickEdit.ShowDialog();
			if(formProcButtonQuickEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			fillPanelQuickButtons();
		}

	}
}