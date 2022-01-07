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
		private bool changed;
		///<summary>defnum</summary>
		private long selectedCat;
		///<summary>This list of displayed buttons for the selected cat.</summary>
		private ProcButton[] ButtonList;
		private List<ProcButtonQuick> listProcButtonQuicks;
		private List<Def> _listProcButtonCatDefs;

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
			foreach(Control c in PanelClient.Controls) {//make all controls visible. Then hide below.
				c.Visible=true;
			}
			if(listCategories.SelectedIndex==0) {
				listViewButtons.Visible=false;
				butAdd.Visible=false;
				butDelete.Visible=false;
				butUp.Visible=false;
				butDown.Visible=false;
			}
			else {
				panelQuickButtons.Visible=false;
				labelEdit.Visible=false;
			}
		}

		///<summary>Make the QuickButtonGrid exactly the same size as it will display in the chart module.</summary>
		private void ResizeControls() {
			
		}

		private void fillPanelQuickButtons() {
			panelQuickButtons.BeginUpdate();
			panelQuickButtons.ListODPanelItems.Clear();
			listProcButtonQuicks=ProcButtonQuicks.GetAll();
			listProcButtonQuicks.Sort(ProcButtonQuicks.sortYX);
			ODPanelItem pItem;
			for(int i=0;i<listProcButtonQuicks.Count;i++) {
				pItem=new ODPanelItem();
				pItem.Text=listProcButtonQuicks[i].Description;
				pItem.YPos=listProcButtonQuicks[i].YPos;
				pItem.ItemOrder=listProcButtonQuicks[i].ItemOrder;
				pItem.ItemType=(listProcButtonQuicks[i].IsLabel?ODPanelItemType.Label:ODPanelItemType.Button);
				pItem.Tags.Add(listProcButtonQuicks[i]);
				panelQuickButtons.ListODPanelItems.Add(pItem);
			}
			panelQuickButtons.EndUpdate();
		}

		private void FillCategories(){
			ProcButtonQuicks.ValidateAll();
			listCategories.Items.Clear();
			listCategories.Items.Add("Quick Buttons");//hardcoded category.
			_listProcButtonCatDefs=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true);
			if(_listProcButtonCatDefs.Count==0){
				selectedCat=0;
				listCategories.SelectedIndex=0;
				return;
			}
			for(int i=0;i<_listProcButtonCatDefs.Count;i++){
				listCategories.Items.Add(_listProcButtonCatDefs[i].ItemName);
				if(selectedCat==_listProcButtonCatDefs[i].DefNum){
					listCategories.SelectedIndex=i+1;
				}
			}
			if(listCategories.SelectedIndex==-1){//category was hidden, or just opening the form
				listCategories.SelectedIndex=0;
				selectedCat=0;
			}
			if(listCategories.SelectedIndex>0) {//hardcoded category doesn't have a DefNum.
				selectedCat=_listProcButtonCatDefs[listCategories.SelectedIndex-1].DefNum;
			}
		}

		private void FillButtons(){
			listViewButtons.Items.Clear();
			imageListProcButtons.Images.Clear();
			if(selectedCat==0) {
				//empty button list and return because we will be using and OD grid to display these buttons.
				ButtonList=new ProcButton[0];
				return;
			}
			ProcButtons.RefreshCache();
			ButtonList=ProcButtons.GetForCat(selectedCat);
			//first check and fix any order problems
			for(int i=0;i<ButtonList.Length;i++) {
				if(ButtonList[i].ItemOrder!=i) {
					ButtonList[i].ItemOrder=i;
					ProcButtons.Update(ButtonList[i]);
				}
			}
			ListViewItem item;
			for(int i=0;i<ButtonList.Length;i++) {
				if(ButtonList[i].ButtonImage!=""){
					//image keys are simply the ProcButtonNum
					try {
						imageListProcButtons.Images.Add(ButtonList[i].ProcButtonNum.ToString(),PIn.Bitmap(ButtonList[i].ButtonImage));
					}
					catch {
						imageListProcButtons.Images.Add(new Bitmap(20,20));//Add a blank image so the list stays in synch
					}
				}
				item=new ListViewItem(new string[] {ButtonList[i].Description},ButtonList[i].ProcButtonNum.ToString());
				listViewButtons.Items.Add(item);
			}
    }

		private void listViewButtons_DoubleClick(object sender,EventArgs e) {
			if(listViewButtons.SelectedIndices.Count==0) {//Nothing selected
				return;
			}
			ProcButton but=ButtonList[listViewButtons.SelectedIndices[0]].Copy();
			using FormProcButtonEdit FormPBE=new FormProcButtonEdit(but);
			FormPBE.ShowDialog();
			changed=true;
			FillButtons();
		}

		private void listCategories_Click(object sender,EventArgs e) {
			if(listCategories.SelectedIndex==-1){
				return;
			}
			SetVisibility();
			if(listCategories.SelectedIndex==0) {
				selectedCat=0;
			}
			else {
				selectedCat=_listProcButtonCatDefs[listCategories.SelectedIndex-1].DefNum;
			}
			FillButtons();
		}

		private void butEdit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Setup)) {
				return;
			}
			using FormDefinitions FormD=new FormDefinitions(DefCat.ProcButtonCats);
			FormD.ShowDialog();
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
        ProcButton but=ButtonList[listViewButtons.SelectedIndices[0]].Copy();
        but.ItemOrder++;
        ProcButtons.Update(but);
        selected=but.ItemOrder;
        but=ButtonList[listViewButtons.SelectedIndices[0]+1].Copy();
        but.ItemOrder--;
        ProcButtons.Update(but);
      }		
      FillButtons();
			changed=true;
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
        ProcButton but=ButtonList[listViewButtons.SelectedIndices[0]].Copy();
        but.ItemOrder--;
        ProcButtons.Update(but);
        selected=but.ItemOrder;
        but=ButtonList[listViewButtons.SelectedIndices[0]-1].Copy();
        but.ItemOrder++;
        ProcButtons.Update(but);
      }	
      FillButtons();	
			changed=true;
			listViewButtons.SelectedIndices.Clear();
			listViewButtons.SelectedIndices.Add(selected);
		}

		private void butAdd_Click(object sender, System.EventArgs e) {
			if(listCategories.SelectedIndex==-1){
				return;
			}
			ProcButton but=new ProcButton();
			but.Category=selectedCat;
			but.ItemOrder=listViewButtons.Items.Count;
      using FormProcButtonEdit FormPBE=new FormProcButtonEdit(but);
      FormPBE.IsNew=true;
      FormPBE.ShowDialog();
			changed=true;
      FillButtons();	
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listViewButtons.SelectedIndices.Count==0){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			ProcButtons.Delete(ButtonList[listViewButtons.SelectedIndices[0]]);
			changed=true;
			FillButtons();
		}

		private void butClose_Click(object sender,System.EventArgs e) {
			Close();
		}

		private void FormProcButtons_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.ProcButtons);
			}
		}

		private void panelQuickButtons_RowDoubleClick(object sender,ODButtonPanelEventArgs e) {
			using FormProcButtonQuickEdit FormPBQ=new FormProcButtonQuickEdit();
			//Search through tags of the ODPanelItem for the PBQ.
			for(int i=0;e.Item!=null && i<e.Item.Tags.Count;i++) {
				if(e.Item.Tags[i].GetType()==typeof(ProcButtonQuick)){
					FormPBQ.pbqCur=(ProcButtonQuick)e.Item.Tags[i];
					break;
				}
			}
			if(FormPBQ.pbqCur==null) {//clicked on either a blank row or to the right of existing buttons on a row.
				FormPBQ.IsNew=true;
				FormPBQ.pbqCur=new ProcButtonQuick();
				FormPBQ.pbqCur.YPos=e.Row;//Set Row
				for(int i=0;i<listProcButtonQuicks.Count;i++){ //Set ItemOrder
					if(listProcButtonQuicks[i].YPos!=FormPBQ.pbqCur.YPos //Wrong row
						|| FormPBQ.pbqCur.ItemOrder>listProcButtonQuicks[i].ItemOrder) { //Already have a larger item order
							continue;
					}
					FormPBQ.pbqCur.ItemOrder=listProcButtonQuicks[i].ItemOrder+1;//new PBQ should have the highest item order in the row.
				}
			}
			FormPBQ.ShowDialog();
			if(FormPBQ.DialogResult!=DialogResult.OK) {
				return;
			}
			fillPanelQuickButtons();
		}

		

		

		
		

	}
}
