using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;

namespace OpenDental{
	/// <summary></summary>
	public partial class FormToothGridDef:FormODBase {
		//private bool changed;
		//public DisplayFieldCategory category;
		///<summary>This is the list of all "hardcoded" display fields. Users may add additional fields that will be used to populate ListShowing, but will not be added to AvailList.</summary>
		private List<string> AvailList;
		
		///<summary></summary>
		public FormToothGridDef()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDisplayFields_Load(object sender,EventArgs e) {
			FillAvailList();//fills local variable
			FillListAvailable();//fills list in UI
			//category = DisplayFieldCategory.ToothGrid;
			FillGrids();
			//ListShowing = ToothGridDefs
			//labelCategory.Text=category.ToString();
			//textCustomField.Visible=false;
			//labelCustomField.Visible=false;
			//listAvailable.Height=412;
			//DisplayFields.RefreshCache();
			//ListShowing=DisplayFields.GetForCategory(category);
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  textCustomField.Visible=true;
			//  labelCustomField.Visible=true;
			//  listAvailable.Height=227;//227px for short, 412px for tall
			//  labelAvailable.Text=Lan.g(this,"Previously Used Fields");
			//}
			//FillGrids();
		}

		private void FillAvailList() {
			AvailList = new List<string>();
			AvailList.Add("Tooth");
			AvailList.Add("IsPrimary");
			AvailList.Add("IsMissing");
		}

		private void FillListAvailable() {
			listAvailable.Items.AddList(AvailList,x => x.ToString());
		}

		private void FillGrids(){
			//AvailList=DisplayFields.GetAllAvailableList(category);//This one needs to be called repeatedly.
			//gridMain.BeginUpdate();
			//gridMain.Columns.Clear();
			//ODGridColumn col=new ODGridColumn(Lan.g("FormDisplayFields","FieldName"),110);
			//gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("FormDisplayFields","New Descript"),110);
			//gridMain.Columns.Add(col);
			//col=new ODGridColumn(Lan.g("FormDisplayFields","Width"),60);
			//gridMain.Columns.Add(col);
			//gridMain.Rows.Clear();
			//ODGridRow row;
			//for(int i=0;i<ListShowing.Count;i++){
			//  row=new ODGridRow();
			//  row.Cells.Add(ListShowing[i].InternalName);
			//  row.Cells.Add(ListShowing[i].Description);
			//  row.Cells.Add(ListShowing[i].ColumnWidth.ToString());
			//  gridMain.Rows.Add(row);
			//}
			//gridMain.EndUpdate();
			////Remove things from AvailList that are in the ListShowing.
			//for(int i=0;i<ListShowing.Count;i++){
			//  for(int j=0;j<AvailList.Count;j++) {
			//    //Only removing one item from AvailList per iteration of i, so RemoveAt() is safe without going backwards.
			//    if(category==DisplayFieldCategory.OrthoChart) {
			//      //OrthoChart category does not use InternalNames.
			//      if(ListShowing[i].Description==AvailList[j].Description) {
			//        AvailList.RemoveAt(j);
			//        break;
			//      }
			//    }
			//    else {
			//      if(ListShowing[i].InternalName==AvailList[j].InternalName) {
			//        AvailList.RemoveAt(j);
			//        break;
			//      }
			//    }
			//  }
			//}
			//listAvailable.Items.Clear();
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  listAvailable.Items.AddList(AvailList,x => x.Description);
			//}
			//else {
			//  listAvailable.Items.AddList(AvailList,x => x.ToString());
			//}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			//using FormDisplayFieldEdit formD=new FormDisplayFieldEdit();
			//formD.FieldCur=ListShowing[e.Row];
			//DisplayField tempField=ListShowing[e.Row].Copy();
			//formD.ShowDialog();
			//if(formD.DialogResult!=DialogResult.OK) {
			//  ListShowing[e.Row]=tempField.Copy();
			//  return;
			//}
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  if(ListShowing[e.Row].Description=="") {
			//    ListShowing[e.Row]=tempField.Copy();
			//    MsgBox.Show(this,"Description cannot be blank.");
			//    return;
			//  }
			//  for(int i=0;i<ListShowing.Count;i++) {//Check against ListShowing only
			//    if(i==e.Row) {
			//      continue;
			//    }
			//    if(ListShowing[e.Row].Description==ListShowing[i].Description) {
			//      ListShowing[e.Row]=tempField;
			//      MsgBox.Show(this,"That field name already exists.");
			//      return;
			//    }
			//  }
			//  for(int i=0;i<AvailList.Count;i++) {//check against AvailList only
			//    if(ListShowing[e.Row].Description==AvailList[i].Description) {
			//      ListShowing[e.Row]=tempField;
			//      MsgBox.Show(this,"That field name already exists.");
			//      return;
			//    }
			//  }
			//}
			//FillGrids();
			//changed=true;
		}

		private void butDefault_Click(object sender,EventArgs e) {
			//ListShowing=DisplayFields.GetDefaultList(category);//empty for ortho
			//FillGrids();
			//changed=true;
		}

		private void butLeft_Click(object sender,EventArgs e) {
			
				//if(listAvailable.SelectedIndices.Count==0 && textCustomField.Text=="") {
				//  MsgBox.Show(this,"Please select an item in the list on the right or create a new field first.");
				//  return;
				//}
				//if(textCustomField.Text!="") {//Add new ortho chart field
				//  foreach(ToothGridDef defShowing in ListShowing) {
				//    if(textCustomField.Text==defShowing.NameInternal || textCustomField.Text==defShowing.NameShowing) {
				//      MsgBox.Show(this,"That field is already displaying.");
				//      return;
				//    }
				//  }
				//  for(int i=0;i<AvailList.Count;i++) {
				//    if(textCustomField.Text==AvailList[i].Description) {
				//      ListShowing.Add(AvailList[i]);
				//      textCustomField.Text="";
				//      changed=true;
				//      FillGrids();
				//      return;
				//    }
				//  }
				//  DisplayField df=new DisplayField("",100,DisplayFieldCategory.OrthoChart);
				//  df.Description=textCustomField.Text;
				//  ListShowing.Add(df);
				//  textCustomField.Text="";
				//}
				//else {//add existing ortho chart field(s)
				//  DisplayField field;
				//  for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
				//    field=AvailList[listAvailable.SelectedIndices[i]];
				//    field.ColumnWidth=100;
				//    ListShowing.Add(field);
				//  }
				}


			//if(category==DisplayFieldCategory.OrthoChart) {//Ortho Chart
			//  if(listAvailable.SelectedIndices.Count==0 && textCustomField.Text=="") {
			//    MsgBox.Show(this,"Please select an item in the list on the right or create a new field first.");
			//    return;
			//  }
			//  if(textCustomField.Text!="") {//Add new ortho chart field
			//    for(int i=0;i<ListShowing.Count;i++) {
			//      if(textCustomField.Text==ListShowing[i].Description) {
			//        MsgBox.Show(this,"That field is already displaying.");
			//        return;
			//      }
			//    }
			//    for(int i=0;i<AvailList.Count;i++) {
			//      if(textCustomField.Text==AvailList[i].Description) {
			//        ListShowing.Add(AvailList[i]);
			//        textCustomField.Text="";
			//        changed=true;
			//        FillGrids();
			//        return;
			//      }
			//    }
			//    DisplayField df=new DisplayField("",100,DisplayFieldCategory.OrthoChart);
			//    df.Description=textCustomField.Text;
			//    ListShowing.Add(df);
			//    textCustomField.Text="";
			//  }
			//  else {//add existing ortho chart field(s)
			//    DisplayField field;
			//    for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
			//      field=AvailList[listAvailable.SelectedIndices[i]];
			//      field.ColumnWidth=100;
			//      ListShowing.Add(field);
			//    }
			//  }
			//}
			//else {//All other display field types
			//  if(listAvailable.SelectedIndices.Count==0) {
			//    MsgBox.Show(this,"Please select an item in the list on the right first.");
			//    return;
			//  }
			//  DisplayField field;
			//  for(int i=0;i<listAvailable.SelectedIndices.Count;i++) {
			//    field=(DisplayField)listAvailable.Items.GetObjectAt(listAvailable.SelectedIndices[i]);
			//    ListShowing.Add(field);
			//  }
			//}
			//changed=true;
			//FillGrids();
		//}

		private void butRight_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid on the left first.");
			//  return;
			//}
			//for(int i=gridMain.SelectedIndices.Length-1;i>=0;i--){//go backwards
			//  ListShowing.RemoveAt(gridMain.SelectedIndices[i]);
			//}
			//FillGrids();
			//changed=true;
		}

		private void butUp_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid first.");
			//  return;
			//}
			//int[] selected=new int[gridMain.SelectedIndices.Length];
			//for(int i=0;i<gridMain.SelectedIndices.Length;i++){
			//  selected[i]=gridMain.SelectedIndices[i];
			//}
			//if(selected[0]==0){
			//  return;
			//}
			//for(int i=0;i<selected.Length;i++){
			//  ListShowing.Reverse(selected[i]-1,2);
			//}
			//FillGrids();
			//for(int i=0;i<selected.Length;i++){
			//  gridMain.SetSelected(selected[i]-1,true);
			//}
			//changed=true;
		}

		private void butDown_Click(object sender,EventArgs e) {
			//if(gridMain.SelectedIndices.Length==0) {
			//  MsgBox.Show(this,"Please select an item in the grid first.");
			//  return;
			//}
			//int[] selected=new int[gridMain.SelectedIndices.Length];
			//for(int i=0;i<gridMain.SelectedIndices.Length;i++) {
			//  selected[i]=gridMain.SelectedIndices[i];
			//}
			//if(selected[selected.Length-1]==ListShowing.Count-1) {
			//  return;
			//}
			//for(int i=selected.Length-1;i>=0;i--) {//go backwards
			//  ListShowing.Reverse(selected[i],2);
			//}
			//FillGrids();
			//for(int i=0;i<selected.Length;i++) {
			//  gridMain.SetSelected(selected[i]+1,true);
			//}
			//changed=true;
		}

		private void listAvailable_Click(object sender,EventArgs e) {
			//textCustomField.Text="";
		}

		private void textCustomField_Click(object sender,EventArgs e) {
			//listAvailable.SelectedIndex=-1;
		}

		private void butOK_Click(object sender,EventArgs e) {
			//if(!changed) {
			//  DialogResult=DialogResult.OK;
			//  return;
			//}
			//if(category==DisplayFieldCategory.OrthoChart) {
			//  DisplayFields.SaveListForOrthoChart(ListShowing);
			//}
			//else {
			//  DisplayFields.SaveListForCategory(ListShowing,category);
			//}
			//DataValid.SetInvalid(InvalidType.DisplayFields);
			//DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormDisplayFields_FormClosing(object sender,FormClosingEventArgs e) {

		}

		
		

		

		

		

		

		


	}
}





















