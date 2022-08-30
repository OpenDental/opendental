using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormOrthoHardwareEdit:FormODBase {
		public OrthoHardware OrthoHardwareCur;

		public FormOrthoHardwareEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoHardwareEdit_Load(object sender,EventArgs e) {
			textDateExam.Text=OrthoHardwareCur.DateExam.ToShortDateString();
			textType.Text=OrthoHardwareCur.OrthoHardwareType.ToString();
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Bracket){
				labelTeeth.Text=Lan.g(this,"Tooth");
				labelComments.Text="Example: "+Tooth.Display("7",toothNumberingNomenclature);
				textToothRange.Text=Tooth.Display(OrthoHardwareCur.ToothRange,toothNumberingNomenclature);
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Elastic){
				labelTeeth.Text=Lan.g(this,"Teeth");
				labelComments.Text="Example: "+Tooth.Display("6",toothNumberingNomenclature)+","+Tooth.Display("27",toothNumberingNomenclature)+","+Tooth.Display("28",toothNumberingNomenclature);
				textToothRange.Text=Tooth.DisplayOrthoCommas(OrthoHardwareCur.ToothRange,toothNumberingNomenclature);
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				labelTeeth.Text=Lan.g(this,"Tooth Range");
				labelComments.Text="Example: "+Tooth.Display("3",toothNumberingNomenclature)+"-"+Tooth.Display("14",toothNumberingNomenclature);
				textToothRange.Text=Tooth.DisplayOrthoDash(OrthoHardwareCur.ToothRange,toothNumberingNomenclature);
			}
			textNote.Text=OrthoHardwareCur.Note;
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(OrthoHardwareCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			OrthoHardwares.Delete(OrthoHardwareCur.OrthoHardwareNum);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender,EventArgs e) {
			if(textToothRange.Text==""){
				if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Bracket){
					MsgBox.Show(this,"Please enter a tooth.");
				}
				else{
					MsgBox.Show(this,"Please enter teeth.");
				}
				return;
			}
			if(!textDateExam.IsValid()){
				return;
			}
			OrthoHardwareCur.DateExam=textDateExam.Value;
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Bracket){
				if(!Tooth.IsValidEntry(textToothRange.Text,toothNumberingNomenclature)){
					MsgBox.Show(this,"Invalid tooth number.");
					return;
				}
				OrthoHardwareCur.ToothRange=Tooth.Parse(textToothRange.Text,toothNumberingNomenclature);
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Elastic){
				try{
					OrthoHardwareCur.ToothRange=Tooth.ParseOrthoCommas(textToothRange.Text,toothNumberingNomenclature);
				}
				catch(Exception ex){
					MsgBox.Show(ex.Message);
					return;
				}
			}
			if(OrthoHardwareCur.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				try{
					OrthoHardwareCur.ToothRange=Tooth.ParseOrthoDash(textToothRange.Text,toothNumberingNomenclature);
				}
				catch(Exception ex){
					MsgBox.Show(ex.Message);
					return;
				}
			}
			OrthoHardwareCur.Note=textNote.Text;
			if(OrthoHardwareCur.IsNew){
				OrthoHardwares.Insert(OrthoHardwareCur);
			}
			else{
				OrthoHardwares.Update(OrthoHardwareCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
	}
}