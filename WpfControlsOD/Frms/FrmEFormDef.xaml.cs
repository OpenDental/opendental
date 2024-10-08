using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary></summary>
	public partial class FrmEFormDef : FrmODBase {
		public EFormDef EFormDefCur;

		///<summary></summary>
		public FrmEFormDef() {
			InitializeComponent();
			Load+=FrmEFormDef_Load;
		}

		private void FrmEFormDef_Load(object sender, EventArgs e) {
			Lang.F(this);
			textDescription.Text=EFormDefCur.Description;
			listBoxType.Items.AddEnums<EnumEFormType>();
			listBoxType.SetSelectedEnum(EFormDefCur.FormType);
			checkShowLabelsBold.Checked=EFormDefCur.ShowLabelsBold;
			textVIntMaxWidth.Value=EFormDefCur.MaxWidth;
			int spaceBelowDefault=PrefC.GetInt(PrefName.EformsSpaceBelowEachField);
			labelSpaceBelowDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceBelowDefault.ToString();
			if(EFormDefCur.SpaceBelowEachField==-1){
				textSpaceBelow.Text="";
			}
			else{
				textSpaceBelow.Text=EFormDefCur.SpaceBelowEachField.ToString();
			}
			int spaceToRightDefault=PrefC.GetInt(PrefName.EformsSpaceToRightEachField);
			labelSpaceToRightDefault.Text=Lang.g(this,"leave blank to use the default value of ")+spaceToRightDefault.ToString();
			if(EFormDefCur.SpaceToRightEachField==-1){
				textSpaceToRight.Text="";
			}
			else{
				textSpaceToRight.Text=EFormDefCur.SpaceToRightEachField.ToString();
			}
		}

		private void butSave_Click(object sender, EventArgs e) {
			int spaceBelow=-1;
			if(textSpaceBelow.Text!=""){
				try{
					spaceBelow=Convert.ToInt32(textSpaceBelow.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space Below first.");
					return;
				}
				if(spaceBelow<0 || spaceBelow>100){
					MsgBox.Show(this,"Space Below value is invalid.");
					return;
				}
			}
			int spaceToRight=-1;
			if(textSpaceToRight.Text!=""){
				try{
					spaceToRight=Convert.ToInt32(textSpaceToRight.Text);
				}
				catch{
					MsgBox.Show(this,"Please fix error in Space to Right first.");
					return;
				}
				if(spaceToRight<0 || spaceToRight>100){
					MsgBox.Show(this,"Space to Right value is invalid.");
					return;
				}
			}
			//end of validation
			EFormDefCur.Description=textDescription.Text;
			EFormDefCur.FormType=listBoxType.GetSelected<EnumEFormType>();
			EFormDefCur.ShowLabelsBold=checkShowLabelsBold.Checked==true;
			EFormDefCur.MaxWidth=450;
			if(textVIntMaxWidth.IsValid()){//instead of telling user about this, we will just ignore errors
				EFormDefCur.MaxWidth=textVIntMaxWidth.Value;
			}
			EFormDefCur.SpaceBelowEachField=spaceBelow;
			EFormDefCur.SpaceToRightEachField=spaceToRight;
			IsDialogOK=true;
		}
	}
}