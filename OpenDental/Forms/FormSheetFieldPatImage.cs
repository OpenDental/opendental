using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormSheetFieldPatImage:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		///<summary>Ignored. Available for mobile but all fields are relevant</summary>
		public bool IsEditMobile;
		public bool IsReadOnly;
		private List<Def> _listImageCatDefs;

		public FormSheetFieldPatImage() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldPatImage_Load(object sender,EventArgs e) {
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
			if(IsReadOnly){
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			FillCombo();
			textXPos.Text=SheetFieldDefCur.XPos.ToString();
			textYPos.Text=SheetFieldDefCur.YPos.ToString();
			textWidth.Text=SheetFieldDefCur.Width.ToString();
			textHeight.Text=SheetFieldDefCur.Height.ToString();
		}

		private void FillCombo(){
			comboImageCategory.Items.Clear();
			_listImageCatDefs=Defs.GetDefsForCategory(DefCat.ImageCats,true);
			for(int i=0;i<_listImageCatDefs.Count;i++) {
				comboImageCategory.Items.Add(_listImageCatDefs[i].ItemName);
				if(SheetFieldDefCur.FieldName==_listImageCatDefs[i].DefNum.ToString()) {
					comboImageCategory.SelectedIndex=i;
				}
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			SheetFieldDefCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textXPos.IsValid()
				|| !textYPos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(comboImageCategory.SelectedIndex<0) {
				MsgBox.Show(this,"Please select an image category first.");
				return;
			}
			SheetFieldDefCur.FieldName=_listImageCatDefs[comboImageCategory.SelectedIndex].DefNum.ToString();
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			//don't save to database here.
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
		

		

	

		

		

		

		

		
	}
}