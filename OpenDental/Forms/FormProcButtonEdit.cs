using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
///<summary></summary>
	public partial class FormProcButtonEdit:FormODBase {
		///<summary></summary>
    public bool IsNew;
		private ProcButton ProcButtonCur;
		///<summary>Deep copy of all AutoCode items in the cache that are not hidden (short).</summary>
		private List<AutoCode> _listShortDeep;
		private List<Def> _listProcButtonCatDefs;

		///<summary></summary>
		public FormProcButtonEdit(ProcButton procButtonCur){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ProcButtonCur=procButtonCur;
		}

		private void FormChartProcedureEntryEdit_Load(object sender, System.EventArgs e) {
			AutoCodes.RefreshCache();
      ProcButtonItems.RefreshCache();     
      if(IsNew){
        this.Text=Lan.g(this,"Add Procedure Button");
      }
      else{
        this.Text=Lan.g(this,"Edit Procedure Button");
      }
			textDescript.Text=ProcButtonCur.Description;
			_listProcButtonCatDefs=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true);
			for(int i=0;i<_listProcButtonCatDefs.Count;i++){
				comboCategory.Items.Add(_listProcButtonCatDefs[i].ItemName);
				if(ProcButtonCur.Category==_listProcButtonCatDefs[i].DefNum){
					comboCategory.SelectedIndex=i;
				}
			}
			if(comboCategory.SelectedIndex==-1){
				comboCategory.SelectedIndex=0;//we know that there will always be at least one cat. Validated in FormProcButtons
			}
			pictureBox.Image=PIn.Bitmap(ProcButtonCur.ButtonImage);
			checkMultiVisit.Checked=ProcButtonCur.IsMultiVisit;
			long[] codeNumList=ProcButtonItems.GetCodeNumListForButton(ProcButtonCur.ProcButtonNum);
			long[] auto=ProcButtonItems.GetAutoListForButton(ProcButtonCur.ProcButtonNum);
			listADA.Items.Clear();
			for(int i=0;i<codeNumList.Length;i++) {
				listADA.Items.Add(ProcedureCodes.GetStringProcCode(codeNumList[i]));
			}
			listAutoCodes.Items.Clear();
			_listShortDeep=AutoCodes.GetListDeep(true);
			for(int i=0;i<_listShortDeep.Count;i++) {
				listAutoCodes.Items.Add(_listShortDeep[i].Description);
				for(int j=0;j<auto.Length;j++){
					if(auto[j]==_listShortDeep[i].AutoCodeNum) {
						listAutoCodes.SetSelected(i);
						break;
					}
				}
      }
			//fill images to pick from
			for(int i=0;i<imageList.Images.Count;i++){
				listView.Items.Add("",i);
			}
		}

		private void butImport_Click(object sender,EventArgs e) {
			using OpenFileDialog dlg=new OpenFileDialog();
			if(dlg.ShowDialog()!=DialogResult.OK){
				return;
			}
			try{
				Image importedImg=Image.FromFile(dlg.FileName);
				if(importedImg.Size!=new Size(20,20)) {
					MessageBox.Show(Lan.g(this,"Image should be 20x20. Image selected was: ")+importedImg.Size.Width+"x"+importedImg.Size.Height);
					return;
				}
				pictureBox.Image=Image.FromFile(dlg.FileName);
			}
			catch{
				MsgBox.Show(this,"Error loading file.");
			}
		}

		private void listView_ItemActivate(object sender,EventArgs e) {
			if(listView.SelectedIndices.Count==0){
				return;
			}
			pictureBox.Image=imageList.Images[listView.SelectedIndices[0]];
			listView.SelectedIndices.Clear();
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(listADA.SelectedIndex<1) { //Handles none selected, and top selected
				return;
			}
			string selectedProcCode=listADA.GetStringSelectedItems();
			string movingProcCode=listADA.Items.GetTextShowingAt(listADA.SelectedIndex-1);
			listADA.Items.SetValue(listADA.SelectedIndex,movingProcCode);
			listADA.Items.SetValue(listADA.SelectedIndex-1,selectedProcCode);
			listADA.SelectedIndex=listADA.SelectedIndex-1;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(listADA.SelectedIndex==-1 || listADA.SelectedIndex==listADA.Items.Count-1) { //Handles none selected, and top selected
				return;
			}
			string selectedProcCode=listADA.GetStringSelectedItems();
			string movingProcCode=listADA.Items.GetTextShowingAt(listADA.SelectedIndex+1);
			listADA.Items.SetValue(listADA.SelectedIndex,movingProcCode);
			listADA.Items.SetValue(listADA.SelectedIndex+1,selectedProcCode);
			listADA.SelectedIndex=listADA.SelectedIndex+1;
		}

		private void butClear_Click(object sender,EventArgs e) {
			pictureBox.Image=null;
		} 

		private void butAdd_Click(object sender, System.EventArgs e) {
		  using FormProcCodes FormP=new FormProcCodes();
      FormP.IsSelectionMode=true;
      FormP.ShowDialog();
      if(FormP.DialogResult!=DialogResult.Cancel){
        listADA.Items.Add(ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum));  
      } 
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
      if(listADA.SelectedIndex < 0){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
      }
      listADA.Items.RemoveAt(listADA.SelectedIndex);        
		}

	 	private void butOK_Click(object sender, System.EventArgs e) {
      if(textDescript.Text==""){
				MessageBox.Show(Lan.g(this,"You must type in a description."));
				return; 
      }
			if(listADA.Items.Count==0  && listAutoCodes.SelectedIndices.Count==0){
        MessageBox.Show(Lan.g(this,"You must pick at least one Auto Code or Procedure Code."));
        return;
      }
			for(int i=0;i<listAutoCodes.SelectedIndices.Count;i++){
				AutoCode autoCode=_listShortDeep[listAutoCodes.SelectedIndices[i]];
				if(AutoCodeItems.GetListForCode(autoCode.AutoCodeNum).Count==0) {
					//This AutoCode was saved with no AutoCodeItems attached, which is invalid.
					MessageBox.Show(this,Lan.g(this,"The following AutoCode has no associated Procedure Codes: ")+"\r\n"+autoCode.Description+"\r\n"
						+Lan.g(this,"AutoCode must be setup correctly before it can be used with a Quick Proc Button."));
					return;
				}
			}
			//Point of no return.
      ProcButtonCur.Description=textDescript.Text;
			if(ProcButtonCur.Category != _listProcButtonCatDefs[comboCategory.SelectedIndex].DefNum){
				//This will put it at the end of the order in the new category
				ProcButtonCur.ItemOrder
					=ProcButtons.GetForCat(_listProcButtonCatDefs[comboCategory.SelectedIndex].DefNum).Length;
			}
			ProcButtonCur.Category=_listProcButtonCatDefs[comboCategory.SelectedIndex].DefNum;
			ProcButtonCur.ButtonImage=POut.Bitmap((Bitmap)pictureBox.Image,System.Drawing.Imaging.ImageFormat.Png);
			ProcButtonCur.IsMultiVisit=checkMultiVisit.Checked;
      if(IsNew){
        ProcButtonCur.ItemOrder=ProcButtons.GetCount();
        ProcButtons.Insert(ProcButtonCur);
      }
      else{
        ProcButtons.Update(ProcButtonCur);
      }
      ProcButtonItems.DeleteAllForButton(ProcButtonCur.ProcButtonNum);
			ProcButtonItem item;
      for(int i=0;i<listADA.Items.Count;i++){
        item=new ProcButtonItem();
        item.ProcButtonNum=ProcButtonCur.ProcButtonNum;
        item.CodeNum=ProcedureCodes.GetCodeNum(listADA.Items.GetTextShowingAt(i));
				item.ItemOrder=i+1;//not i++, that would mess up the itteration.
        ProcButtonItems.Insert(item);
      }
      for(int i=0;i<listAutoCodes.SelectedIndices.Count;i++){
        item=new ProcButtonItem();
        item.ProcButtonNum=ProcButtonCur.ProcButtonNum;
        item.AutoCodeNum=_listShortDeep[listAutoCodes.SelectedIndices[i]].AutoCodeNum;
				item.ItemOrder=i+1;//not i++, that would mess up the itteration.
				ProcButtonItems.Insert(item);
      }
      DialogResult=DialogResult.OK;    
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}
