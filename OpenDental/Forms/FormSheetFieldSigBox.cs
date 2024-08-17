using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormSheetFieldSigBox:FormODBase {
		///<summary>This is the object we are editing.</summary>
		public SheetFieldDef SheetFieldDefCur;
		///<summary>We need access to a few other fields of the sheetDef.</summary>
		public SheetDef SheetDefCur;
		///<summary>Ignored.  Not sure if it's available for mobile or not.</summary>
		public bool IsEditMobile;
		public bool IsReadOnly;

		public FormSheetFieldSigBox() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormSheetFieldSigBox_Load(object sender,EventArgs e) {
			textYPos.MaxVal=SheetDefCur.HeightTotal-1;//The maximum y-value of the sheet field must be within the page vertically.
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
			checkRequired.Checked=SheetFieldDefCur.IsRequired;
			checkAllowElectronicSig.Checked=SheetFieldDefCur.CanElectronicallySign;
			checkRestrictSigProvider.Checked=SheetFieldDefCur.IsSigProvRestricted;
			textUiLabelMobile.Text=SheetFieldDefCur.UiLabelMobile;
			textName.Text=SheetFieldDefCur.FieldName;
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
			if(checkRequired.Checked && checkRestrictSigProvider.Checked) {
				MsgBox.Show(this,"You may not require signature boxes that are restricted to providers.");
				return;
			}
			SheetFieldDefCur.XPos=PIn.Int(textXPos.Text);
			SheetFieldDefCur.YPos=PIn.Int(textYPos.Text);
			SheetFieldDefCur.Width=PIn.Int(textWidth.Text);
			SheetFieldDefCur.Height=PIn.Int(textHeight.Text);
			SheetFieldDefCur.IsRequired=checkRequired.Checked;
			SheetFieldDefCur.CanElectronicallySign=checkAllowElectronicSig.Checked;
			SheetFieldDefCur.IsSigProvRestricted=checkRestrictSigProvider.Checked;
			SheetFieldDefCur.UiLabelMobile=textUiLabelMobile.Text;
			SheetFieldDefCur.FieldName=textName.Text;
			//don't save to database here.
			SheetFieldDefCur.IsNew=false;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	

		

		
	}
}