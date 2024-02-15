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
		private ProcButton _procButton;
		///<summary>Deep copy of all AutoCode items in the cache that are not hidden (short).</summary>
		private List<AutoCode> _listAutoCodesShortDeep;
		private List<Def> _listDefsProcButtonCat;

		///<summary></summary>
		public FormProcButtonEdit(ProcButton procButton){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_procButton=procButton;
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
			textDescript.Text=_procButton.Description;
			_listDefsProcButtonCat=Defs.GetDefsForCategory(DefCat.ProcButtonCats,true);
			for(int i=0;i<_listDefsProcButtonCat.Count;i++){
				comboCategory.Items.Add(_listDefsProcButtonCat[i].ItemName);
				if(_procButton.Category==_listDefsProcButtonCat[i].DefNum){
					comboCategory.SelectedIndex=i;
				}
			}
			if(comboCategory.SelectedIndex==-1){
				comboCategory.SelectedIndex=0;//we know that there will always be at least one cat. Validated in FormProcButtons
			}
			pictureBox.Image=PIn.Bitmap(_procButton.ButtonImage);
			checkMultiVisit.Checked=_procButton.IsMultiVisit;
			long[] longArrayCodeNum=ProcButtonItems.GetCodeNumListForButton(_procButton.ProcButtonNum).ToArray();
			long[] longArrayAuto=ProcButtonItems.GetAutoListForButton(_procButton.ProcButtonNum).ToArray();
			listADA.Items.Clear();
			for(int i=0;i<longArrayCodeNum.Length;i++) {
				listADA.Items.Add(ProcedureCodes.GetStringProcCode(longArrayCodeNum[i]));
			}
			listAutoCodes.Items.Clear();
			_listAutoCodesShortDeep=AutoCodes.GetListDeep(true);
			for(int i=0;i<_listAutoCodesShortDeep.Count;i++) {
				listAutoCodes.Items.Add(_listAutoCodesShortDeep[i].Description);
				for(int j=0;j<longArrayAuto.Length;j++){
					if(longArrayAuto[j]==_listAutoCodesShortDeep[i].AutoCodeNum) {
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
			using OpenFileDialog openFileDialog=new OpenFileDialog();
			if(openFileDialog.ShowDialog()!=DialogResult.OK){
				return;
			}
			Image imageImported;
			try{
				imageImported=Image.FromFile(openFileDialog.FileName);
			}
			catch{
				MsgBox.Show(this,"Error loading file.");
				return;
			}
			if(imageImported.Size!=new Size(20,20)) {
				MessageBox.Show(Lan.g(this,"Image should be 20x20. Image selected was: ")+imageImported.Size.Width+"x"+imageImported.Size.Height);
				return;
			}
			pictureBox.Image?.Dispose();
			pictureBox.Image=imageImported;
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
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.Cancel){
				listADA.Items.Add(ProcedureCodes.GetStringProcCode(formProcCodes.CodeNumSelected));  
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			if(listADA.SelectedIndex < 0){
				MessageBox.Show(Lan.g(this,"Please select an item first."));
				return;
			}
			listADA.Items.RemoveAt(listADA.SelectedIndex);
		}

		private void butSave_Click(object sender, System.EventArgs e) {
			if(textDescript.Text==""){
				MessageBox.Show(Lan.g(this,"You must type in a description."));
				return; 
			}
			if(listADA.Items.Count==0  && listAutoCodes.SelectedIndices.Count==0){
				MessageBox.Show(Lan.g(this,"You must pick at least one Auto Code or Procedure Code."));
				return;
			}
			for(int i=0;i<listAutoCodes.SelectedIndices.Count;i++){
				AutoCode autoCode=_listAutoCodesShortDeep[listAutoCodes.SelectedIndices[i]];
				if(AutoCodeItems.GetListForCode(autoCode.AutoCodeNum).Count==0) {
					//This AutoCode was saved with no AutoCodeItems attached, which is invalid.
					MessageBox.Show(this,Lan.g(this,"The following AutoCode has no associated Procedure Codes: ")+"\r\n"+autoCode.Description+"\r\n"
						+Lan.g(this,"AutoCode must be setup correctly before it can be used with a Quick Proc Button."));
					return;
				}
			}
			//Point of no return.
			_procButton.Description=textDescript.Text;
			if(_procButton.Category != _listDefsProcButtonCat[comboCategory.SelectedIndex].DefNum){
				//This will put it at the end of the order in the new category
				_procButton.ItemOrder
					=ProcButtons.GetForCat(_listDefsProcButtonCat[comboCategory.SelectedIndex].DefNum).Length;
			}
			_procButton.Category=_listDefsProcButtonCat[comboCategory.SelectedIndex].DefNum;
			_procButton.ButtonImage=POut.Bitmap((Bitmap)pictureBox.Image,System.Drawing.Imaging.ImageFormat.Png);
			_procButton.IsMultiVisit=checkMultiVisit.Checked;
			if(IsNew){
				_procButton.ItemOrder=ProcButtons.GetCount();
				ProcButtons.Insert(_procButton);
			}
			else{
				ProcButtons.Update(_procButton);
			}
			ProcButtonItems.DeleteAllForButton(_procButton.ProcButtonNum);
			ProcButtonItem procButtonItem;
			for(int i=0;i<listADA.Items.Count;i++){
				procButtonItem=new ProcButtonItem();
				procButtonItem.ProcButtonNum=_procButton.ProcButtonNum;
				procButtonItem.CodeNum=ProcedureCodes.GetCodeNum(listADA.Items.GetTextShowingAt(i));
				procButtonItem.ItemOrder=i+1;//not i++, that would mess up the itteration.
				ProcButtonItems.Insert(procButtonItem);
			}
			for(int i=0;i<listAutoCodes.SelectedIndices.Count;i++){
				procButtonItem=new ProcButtonItem();
				procButtonItem.ProcButtonNum=_procButton.ProcButtonNum;
				procButtonItem.AutoCodeNum=_listAutoCodesShortDeep[listAutoCodes.SelectedIndices[i]].AutoCodeNum;
				procButtonItem.ItemOrder=i+1;//not i++, that would mess up the itteration.
				ProcButtonItems.Insert(procButtonItem);
			}
			DialogResult=DialogResult.OK;
		}

	}
}