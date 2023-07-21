using System;
using System.Collections.Generic;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMapAreaEdit:FormODBase {

		///<summary>The item being edited</summary>
		public MapArea MapAreaCur;

		public FormMapAreaEdit() {
			InitializeComponent();
			InitializeLayoutManager();
		}

		private void FormMapAreaEdit_Load(object sender,EventArgs e) {
			textXPos.Text=MapAreaCur.XPos.ToString();
			textYPos.Text=MapAreaCur.YPos.ToString();
			textDescription.Text=MapAreaCur.Description;
			//show/hide fields according to MaptItemType
			if(MapAreaCur.ItemType==MapItemType.Cubicle) {
				panelLabel.Visible=false;
				//labelDescriptionExample.Text//already set
				textWidth.Text=MapAreaCur.Width.ToString();
				textHeight.Text=MapAreaCur.Height.ToString();
				textExtension.Text=MapAreaCur.Extension.ToString();
			}
			else {//label
				panelCubicle.Visible=false;
				labelDescription.Text="Text";			
				labelDescriptionExample.Text="example: 12";
				textFontSize.Text=MapAreaCur.FontSize.ToString();
			}
		}

		private void textBoxExtension_TextChanged(object sender,EventArgs e) {
			//this will get triggered on load when the text changes
			if(!textExtension.IsValid()){
				textName.Text="";
				return;
			}
			if(textExtension.Text.Length!=4){
				textName.Text="";
				return;
			}
			List<PhoneEmpDefault> listPhoneEmpDefaults=PhoneEmpDefaults.GetDeepCopy();
			PhoneEmpDefault phoneEmpDefault=listPhoneEmpDefaults.Find(x=>x.PhoneExt==textExtension.Value);
			if(phoneEmpDefault is null){
				textName.Text="";
				return;
			}
			textName.Text=phoneEmpDefault.EmpName;
		}

		private void butPick_Click(object sender,EventArgs e) {
			using FormPhoneEmpDefaults formPhoneEmpDefaults=new FormPhoneEmpDefaults();
			formPhoneEmpDefaults.IsSelectionMode=true;
			formPhoneEmpDefaults.ShowDialog();
			if(formPhoneEmpDefaults.DialogResult!=DialogResult.OK){
				return;
			}
			textExtension.Value=formPhoneEmpDefaults.PhoneEmpDefaultSelected.PhoneExt;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(MapAreaCur.IsNew) {
				DialogResult=System.Windows.Forms.DialogResult.Cancel;
				return;
			}
			if(MapAreaCur.ItemType==MapItemType.Cubicle){
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove this cubicle?")) {
					return;
				}
			}
			else{//label
				if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Remove this label?")) {
					return;
				}
			}
			MapAreas.Delete(MapAreaCur.MapAreaNum);
			DialogResult=System.Windows.Forms.DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(!textXPos.IsValid()
				|| !textYPos.IsValid())
			{
				MsgBox.Show("Please fix entry errors first.");
				return;
			}
			if(MapAreaCur.ItemType==MapItemType.Cubicle) {
				if(!textWidth.IsValid()
					|| !textHeight.IsValid()
					|| !textExtension.IsValid())
				{
					MsgBox.Show("Please fix entry errors first.");
					return;
				}
			}
			else{//label
				if(textDescription.Text=="") {
					textDescription.Focus();
					MessageBox.Show(Lan.g(this,"Invalid Text"));
					return;
				}
				if(!textFontSize.IsValid())
				{
					MsgBox.Show("Please fix entry errors first.");
					return;
				}

			}
			MapAreaCur.XPos=textXPos.Value;
			MapAreaCur.YPos=textYPos.Value;
			MapAreaCur.Description=textDescription.Text;
			if(MapAreaCur.ItemType==MapItemType.Cubicle) {
				MapAreaCur.Width=textWidth.Value;
				MapAreaCur.Height=textHeight.Value;
				MapAreaCur.Extension=textExtension.Value;
			}
			else{//Label
				MapAreaCur.FontSize=(float)textFontSize.Value;
			}
			if(MapAreaCur.IsNew) {
				MapAreas.Insert(MapAreaCur);
			}
			else {
				MapAreas.Update(MapAreaCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}
