using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetFieldComboBox:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		public bool IsEditMobile;
		public bool IsReadOnly;
		private string _selectedOption;

		public FormSheetFieldComboBox() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldComboBox_Load(object sender,EventArgs e) {
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			if(IsEditMobile) {
				textTabOrder.Enabled=false;
			}
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			textUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			labelUiLabelMobile.Visible=SheetDefs.IsMobileAllowed(SheetDefCur.SheetType);
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
			textTabOrder.Text=SheetFieldDefCur.TabOrder.ToString();
			textReportable.Text=SheetFieldDefCur.ReportableName;
			textUiLabelMobile.Text=SheetFieldDefCur.UiLabelMobile;
			if(SheetFieldDefCur.FieldValue=="") {
				return;
			}
			_selectedOption=SheetFieldDefCur.FieldValue.Split(';')[0];
			string[] arrayOptions=SheetFieldDefCur.FieldValue.Split(';')[1].Split('|');
			for(int i=0;i<arrayOptions.Length;i++){
				if(String.IsNullOrWhiteSpace(arrayOptions[i])) {
					continue;
				}
				listboxComboOptions.Items.Add(arrayOptions[i]);
			}
		}

		private void textOption_KeyDown(object sender,KeyEventArgs e) {
			if(e.KeyData!=Keys.Enter) {
				return;
			}
			e.SuppressKeyPress=true;
			butAdd_Click(null,null);//If they press enter on the text, add the text to the listbox.
		}

		private void listComboType_SelectionChangeCommitted(object sender,EventArgs e) {
			listboxComboOptions.Items.Clear();
			if(listComboType.SelectedItem.ToString().In("Gender Identity","Sexual Orientation")) {
				if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
					MsgBox.Show(Lan.g(this,$"Public Health is required to use {listComboType.SelectedItem.ToString()}. Turn on Public Health in the Show Features window"));
					listComboType.ClearSelected();
					return;
				}
				else if(Snomeds.GetCodeCount()==0) {
					MsgBox.Show(Lan.g(this,$"SNOMEDCT codes must be downloaded to use {listComboType.SelectedItem.ToString()}. SNOMEDCT codes can be imported from the Code Systems Importer window"));
					listComboType.ClearSelected();
					return;
				}
			}
			if(listComboType.SelectedItem.ToString()=="Clear") {
				//already cleared
			}
			if(listComboType.SelectedItem.ToString()=="Gender Identity") {//Gender Identity
				string[] enumVals=Enum.GetNames(typeof(GenderId));
				listboxComboOptions.Items.AddList(enumVals,x => x.ToString());
			}
			else if(listComboType.SelectedItem.ToString()=="Sexual Orientation") {//Sexual Orientation
				string[] enumVals=Enum.GetNames(typeof(SexOrientation));
				listboxComboOptions.Items.AddList(enumVals,x => x.ToString());
			}
			else if(listComboType.SelectedItem.ToString()=="Patient Race") {//Patient Race
				string[] enumVals=Enum.GetNames(typeof(PatientRaceOld));
				listboxComboOptions.Items.AddList(enumVals,x => x.ToString());
			}
			else if(listComboType.SelectedItem.ToString()=="Patient Grade") {//Patient Grade
				string[] enumVals=Enum.GetNames(typeof(PatientGrade));
				listboxComboOptions.Items.AddList(enumVals,x => x.ToString());
			}
			else if(listComboType.SelectedItem.ToString()=="Urgency") {//Urgency
				string[] enumVals=Enum.GetNames(typeof(TreatmentUrgency));
				listboxComboOptions.Items.AddList(enumVals,x => x.ToString());
			}
			listComboType.ClearSelected();
		}

		private void butAdd_Click(object sender,EventArgs e) {
			if(String.IsNullOrWhiteSpace(textOption.Text)) {
				return;
			}
			listboxComboOptions.Items.Add(textOption.Text);
			textOption.Clear();
		}

		private void butRemove_Click(object sender,EventArgs e) {
			if(listboxComboOptions.SelectedIndex==-1) {
				return;
			}
			listboxComboOptions.Items.RemoveAt(listboxComboOptions.SelectedIndex);
		}

		private void butUp_Click(object sender,EventArgs e) {
			if(listboxComboOptions.SelectedIndex==-1 || listboxComboOptions.SelectedIndex==0) {
				return;
			}
			int idx=listboxComboOptions.SelectedIndex;
			string item=listboxComboOptions.SelectedItem.ToString();
			listboxComboOptions.Items.SetValue(idx,listboxComboOptions.Items.GetTextShowingAt(idx-1));
			listboxComboOptions.Items.SetValue(idx-1,item);
			listboxComboOptions.SelectedIndex=idx-1;
		}

		private void butDown_Click(object sender,EventArgs e) {
			if(listboxComboOptions.SelectedIndex==-1 || listboxComboOptions.SelectedIndex==listboxComboOptions.Items.Count-1) {
				return;
			}
			int idx=listboxComboOptions.SelectedIndex;
			string item=listboxComboOptions.SelectedItem.ToString();
			listboxComboOptions.Items.SetValue(idx,listboxComboOptions.Items.GetTextShowingAt(idx+1));
			listboxComboOptions.Items.SetValue(idx+1,item);
			listboxComboOptions.SelectedIndex=idx+1;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			SaveAndClose();
		}

		private void SaveAndClose(){
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			SheetFieldDefCur.FieldName="";
			if(listComboType.SelectedIndex!=-1) {
				SheetFieldDefCur.FieldName=listComboType.SelectedItem.ToString();
			}
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			SheetFieldDefCur.TabOrder=PIn.Int(textTabOrder.Text);
			SheetFieldDefCur.ReportableName=PIn.String(textReportable.Text);
			SheetFieldDefCur.UiLabelMobile=textUiLabelMobile.Text;
			//ComboBox FieldValue will be:  selectedItem;all|possible|options|here|with|selectedItem|also
			//This is so we don't have to change the database schema for combo boxes.
			SheetFieldDefCur.FieldValue=_selectedOption+";";//NOTE: ; can change to whatever.  Maybe {?  Maybe something else not used often like @?
			for(int i=0;i<listboxComboOptions.Items.Count;i++) {
				if(i>0) {
					SheetFieldDefCur.FieldValue+="|";
				}
				SheetFieldDefCur.FieldValue+=listboxComboOptions.Items.GetTextShowingAt(i);
			}
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}