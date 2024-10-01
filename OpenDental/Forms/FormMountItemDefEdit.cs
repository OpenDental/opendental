using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMountItemDefEdit:FormODBase {
		public MountItemDef MountItemDefCur;

		public FormMountItemDefEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMountItemDefEdit_Load(object sender, EventArgs e){
			textXpos.Text=MountItemDefCur.Xpos.ToString();
			textYpos.Text=MountItemDefCur.Ypos.ToString();
			textWidth.Text=MountItemDefCur.Width.ToString();
			textHeight.Text=MountItemDefCur.Height.ToString();
			textRotate.Value=MountItemDefCur.RotateOnAcquire;
			textToothNumbers.Text=Tooth.DisplayRange(MountItemDefCur.ToothNumbers);
			textTextShowing.Text=MountItemDefCur.TextShowing;
			textFontSize.Value=MountItemDefCur.FontSize;
		}

		private void butAddField_Click(object sender,EventArgs e) {
			List<MessageReplaceType> listMessageReplaceTypes=new List<MessageReplaceType>();
			listMessageReplaceTypes.Add(MessageReplaceType.Office);
			listMessageReplaceTypes.Add(MessageReplaceType.Patient);
			listMessageReplaceTypes.Add(MessageReplaceType.Mount);
			FrmMessageReplacements frmMessageReplacements=new FrmMessageReplacements(listMessageReplaceTypes);
			frmMessageReplacements.MessageReplacementSystemType=MessageReplacementSystemType.Mount;
			frmMessageReplacements.IsSelectionMode=true;
			frmMessageReplacements.ShowDialog();
			if(frmMessageReplacements.IsDialogOK) {
				textTextShowing.SelectedText=frmMessageReplacements.ReplacementTextSelected;
			}
		}

		private void butDelete_Click(object sender, EventArgs e){
			if(MountItemDefCur.IsNew){//although not currenly used
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show(this,MsgBoxButtons.OKCancel,"Delete?")){
				return;
			}
			MountItemDefs.Delete(MountItemDefCur.MountItemDefNum);
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(!textXpos.IsValid()
				|| !textYpos.IsValid()
				|| !textWidth.IsValid()
				|| !textHeight.IsValid()
				|| !textRotate.IsValid()
				|| !textFontSize.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(!textRotate.Value.In(0,90,180,270)){
				MsgBox.Show(this,"Rotate value invalid.");
				return;
			}
			if(!textTextShowing.Text.IsNullOrEmpty() && textFontSize.Value==0){
				MsgBox.Show(this,"Please enter a font size.");
				return;
			}
			try{
				MountItemDefCur.ToothNumbers=Tooth.ParseRange(textToothNumbers.Text);
			}
			catch(Exception ex){
				MessageBox.Show(ex.Message);
				return;
			}
			MountItemDefCur.Xpos=PIn.Int(textXpos.Text);
			MountItemDefCur.Ypos=PIn.Int(textYpos.Text);
			MountItemDefCur.Width=PIn.Int(textWidth.Text);
			MountItemDefCur.Height=PIn.Int(textHeight.Text);
			MountItemDefCur.RotateOnAcquire=textRotate.Value;
			MountItemDefCur.TextShowing=textTextShowing.Text;
			MountItemDefCur.FontSize=(float)textFontSize.Value;
			if(MountItemDefCur.IsNew){//but not currenly used
				MountItemDefs.Insert(MountItemDefCur);
			}
			else{
				MountItemDefs.Update(MountItemDefCur);
			}
			DialogResult=DialogResult.OK;
		}

	}
}