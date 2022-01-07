using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Globalization;

namespace OpenDental {
	///<summary></summary>
	public partial class FormInsCatsSetup:FormODBase {
		private bool changed;
		private List<CovCat> _listCovCats;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormInsCatsSetupCanada";
			}
			return "FormInsCatsSetup";
		}

		///<summary></summary>
		public FormInsCatsSetup() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsCatsSetup_Load(object sender,System.EventArgs e) {
			FillSpans();
		}

		private void FillSpans() {
			CovCats.RefreshCache();
			CovSpans.RefreshCache();
			_listCovCats=CovCats.GetDeepCopy();
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Category"),90);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"From Code"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"To Code"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Hidden"),45);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"E-Benefit Category"),100);
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			CovSpan[] spansForCat;
			for(int i=0;i<_listCovCats.Count;i++) {
				row=new GridRow();
				row.Tag=_listCovCats[i].Copy();
				row.ColorBackG=Color.FromArgb(225,225,225);
				if(i!=0) {
					gridMain.ListGridRows[gridMain.ListGridRows.Count-1].ColorLborder=Color.Black;
				}
				row.Cells.Add(_listCovCats[i].Description);
				row.Cells.Add("");
				row.Cells.Add("");
				if(_listCovCats[i].IsHidden){
					row.Cells.Add("X");
				}
				else {
					row.Cells.Add("");
				}
				if(_listCovCats[i].EbenefitCat==EbenefitCategory.None){
					row.Cells.Add("");
				}
				else{
					row.Cells.Add(_listCovCats[i].EbenefitCat.ToString());
				}
				gridMain.ListGridRows.Add(row);
				spansForCat=CovSpans.GetForCat(_listCovCats[i].CovCatNum);
				for(int j=0;j<spansForCat.Length;j++){
					row=new GridRow();
					row.Tag=spansForCat[j].Copy();
					row.Cells.Add("");
					row.Cells.Add(spansForCat[j].FromCode);
					row.Cells.Add(spansForCat[j].ToCode);
					row.Cells.Add("");
					row.Cells.Add("");
					gridMain.ListGridRows.Add(row);
				}
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,OpenDental.UI.ODGridClickEventArgs e) {
			bool isCat=false;
			long selectedKey=0;
			if(gridMain.ListGridRows[e.Row].Tag.GetType()==typeof(CovCat)){
				isCat=true;
				selectedKey=((CovCat)gridMain.ListGridRows[e.Row].Tag).CovCatNum;
				using FormInsCatEdit FormE=new FormInsCatEdit((CovCat)gridMain.ListGridRows[e.Row].Tag);
				FormE.ShowDialog();
				if(FormE.DialogResult!=DialogResult.OK) {
					return;
				}	
			}
			else{//covSpan
				selectedKey=((CovSpan)gridMain.ListGridRows[e.Row].Tag).CovSpanNum;
				using FormInsSpanEdit FormE=new FormInsSpanEdit((CovSpan)gridMain.ListGridRows[e.Row].Tag);
				FormE.ShowDialog();
				if(FormE.DialogResult!=DialogResult.OK){
					return;
				}
			}
			changed=true;
			FillSpans();
			for(int i=0;i<gridMain.ListGridRows.Count;i++){
				if(isCat && gridMain.ListGridRows[i].Tag.GetType()==typeof(CovCat) 
					&& selectedKey==((CovCat)gridMain.ListGridRows[i].Tag).CovCatNum)
				{
					gridMain.SetSelected(i,true);
				}
				if(!isCat && gridMain.ListGridRows[i].Tag.GetType()==typeof(CovSpan) 
					&& selectedKey==((CovSpan)gridMain.ListGridRows[i].Tag).CovSpanNum)
				{
					gridMain.SetSelected(i,true);
				}
			}
		}		

		private void butAddSpan_Click(object sender, System.EventArgs e){
			if(gridMain.SelectedIndices.Length<1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			if(gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag.GetType()!=typeof(CovCat)){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			CovSpan covspan=new CovSpan();
			covspan.CovCatNum=((CovCat)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).CovCatNum;
			using FormInsSpanEdit FormE=new FormInsSpanEdit(covspan);
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult!=DialogResult.OK){
				return;
			}
			changed=true;
			FillSpans();
		}

		private void butUp_Click(object sender, System.EventArgs e) {
			if(gridMain.SelectedIndices.Length<1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			if(gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag.GetType()!=typeof(CovCat)){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			long catNum=((CovCat)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).CovCatNum;
			CovCats.MoveUp((CovCat)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			changed=true;
			FillSpans();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag.GetType()==typeof(CovCat) && catNum==((CovCat)gridMain.ListGridRows[i].Tag).CovCatNum){
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butDown_Click(object sender, System.EventArgs e){
			if(gridMain.SelectedIndices.Length<1){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			if(gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag.GetType()!=typeof(CovCat)){
				MsgBox.Show(this,"Please select a category first.");
				return;
			}
			long catNum=((CovCat)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag).CovCatNum;
			CovCats.MoveDown((CovCat)gridMain.ListGridRows[gridMain.SelectedIndices[0]].Tag);
			changed=true;
			FillSpans();
			for(int i=0;i<gridMain.ListGridRows.Count;i++) {
				if(gridMain.ListGridRows[i].Tag.GetType()==typeof(CovCat) && catNum==((CovCat)gridMain.ListGridRows[i].Tag).CovCatNum) {
					gridMain.SetSelected(i,true);
				}
			}
		}

		private void butAddCat_Click(object sender, System.EventArgs e) {
			CovCat covcat=new CovCat();
			covcat.CovOrder=(byte)_listCovCats.Count;
			covcat.DefaultPercent=-1;
			using FormInsCatEdit FormE=new FormInsCatEdit(covcat);
			FormE.IsNew=true;
			FormE.ShowDialog();
			if(FormE.DialogResult==DialogResult.OK){
				changed=true;
				FillSpans();
			}	
		}

		/*
		private void butDefaultsCheck_Click(object sender,EventArgs e) {
			string retVal=CheckDefaults();
			if(retVal=="") {
				MsgBox.Show(this,"Categories are set up correctly.  Spans have not been checked.  Push the Reset button to automatically reset the spans to default.");
			}
			else {
				MessageBox.Show(retVal);
			}
		}*/

		private string CheckDefaults(){
			//There needs to be at least 14 categories, each with an etype and no duplicates.
			string retVal="";
			int count;
			for(int i=1;i<15;i++) {//starts with 1 because we don't care about None category
				count=CovCats.CountForEbenCat((EbenefitCategory)i);
				if(count>1){
					retVal+="Duplicate category: "+((EbenefitCategory)i).ToString()+"\r\n";
				}
				if(count==0){
					retVal+="Missing category: "+((EbenefitCategory)i).ToString()+"\r\n";
				}
			}
			if(retVal != "") {
				retVal="The following errors must be fixed manually:\r\n\r\n"+retVal+"\r\n"
					+"Remember that any changes you make affect all current patients who are using those categories.";
			}
			return retVal;
		}

		private void butDefaultsReset_Click(object sender,EventArgs e) {
			string retVal=CheckDefaults();
			if(retVal!="") {
				MessageBox.Show(retVal);
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Reset orders and spans to default?")) {
				return;
			}
			CovCats.SetOrdersToDefault();
			CovCats.SetSpansToDefault();
			FillSpans();
			MsgBox.Show(this,"Done.");
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void FormInsCatsSetup_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.InsCats);
			}
		}

		

		

	}
}
