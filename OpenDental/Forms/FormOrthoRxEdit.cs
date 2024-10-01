using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental {
	public partial class FormOrthoRxEdit:FormODBase {
		public OrthoRx OrthoRxCur;

		public FormOrthoRxEdit() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormOrthoRxEdit_Load(object sender,EventArgs e) {
			textDescription.Text=OrthoRxCur.Description;
			List<OrthoHardwareSpec> listOrthoHardwareSpecs=OrthoHardwareSpecs.GetDeepCopy();
			comboHardwareSpec.Items.AddNone<OrthoHardwareSpec>();
			comboHardwareSpec.Items.AddList(listOrthoHardwareSpecs,x=>x.Description);
			comboHardwareSpec.SetSelectedKey<OrthoHardwareSpec>(OrthoRxCur.OrthoHardwareSpecNum,x=>x.OrthoHardwareSpecNum);
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum!=0){
				if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
					textToothRange.Text=Tooth.DisplayOrthoCommas(OrthoRxCur.ToothRange,toothNumberingNomenclature);
				}
				if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
					textToothRange.Text=Tooth.DisplayOrthoDash(OrthoRxCur.ToothRange,toothNumberingNomenclature);
				}
			}
			SetTeethInstructions();
		}

		private void SetTeethInstructions(){
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum==0){
				labelTeeth.Text=Lan.g(this,"Teeth");
				labelComments.Text="";
			}
			else if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
				labelTeeth.Text=Lan.g(this,"Teeth");
				labelComments.Text="Example: "+Tooth.Display("6",toothNumberingNomenclature)+","+Tooth.Display("27",toothNumberingNomenclature)+","+Tooth.Display("28",toothNumberingNomenclature);
			}
			else if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				labelTeeth.Text=Lan.g(this,"Tooth Range");
				labelComments.Text="Example: "+Tooth.Display("3",toothNumberingNomenclature)+"-"+Tooth.Display("14",toothNumberingNomenclature);
			}
		}

		private void comboHardwareSpec_SelectionChangeCommitted(object sender, EventArgs e){
			SetTeethInstructions();
		}

		private void butUpper_Click(object sender,EventArgs e) {
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum==0){
				MsgBox.Show(this,"Please select a hardware spec first.");
				return;
			}
			if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
				string str="2,3,4,5,6,7,8,9,10,11,12,13,14,15";
				textToothRange.Text=Tooth.DisplayOrthoCommas(str,toothNumberingNomenclature);
			}
			else if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				string str="2-15";
				textToothRange.Text=Tooth.DisplayOrthoDash(str,toothNumberingNomenclature);
			}
		}

		private void butLower_Click(object sender,EventArgs e) {
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum==0){
				MsgBox.Show(this,"Please select a hardware spec first.");
				return;
			}
			if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
				string str="18,19,20,21,22,23,24,25,26,27,28,29,30,31";
				textToothRange.Text=Tooth.DisplayOrthoCommas(str,toothNumberingNomenclature);
			}
			else if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				string str="18-31";
				textToothRange.Text=Tooth.DisplayOrthoDash(str,toothNumberingNomenclature);
			}
		}

		private void butAll_Click(object sender,EventArgs e) {
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum==0){
				MsgBox.Show(this,"Please select a hardware spec first.");
				return;
			}
			if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
				string str="2,3,4,5,6,7,8,9,10,11,12,13,14,15,18,19,20,21,22,23,24,25,26,27,28,29,30,31";
				textToothRange.Text=Tooth.DisplayOrthoCommas(str,toothNumberingNomenclature);
			}
			else if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				MsgBox.Show(this,"All doesn't work with wires.");
			}
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(OrthoRxCur.IsNew){
				DialogResult=DialogResult.Cancel;
				return;
			}
			try{
				OrthoRxs.Delete(OrthoRxCur.OrthoRxNum);
			}
			catch(Exception ex){
				MsgBox.Show(ex.Message);
				return;
			}
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender,EventArgs e) {
			if(textDescription.Text==""){
				MsgBox.Show(this,"Please enter a description first.");
				return;
			}
			OrthoRxCur.Description=textDescription.Text;
			OrthoHardwareSpec orthoHardwareSpec=comboHardwareSpec.GetSelected<OrthoHardwareSpec>();
			if(orthoHardwareSpec.OrthoHardwareSpecNum==0){
				MsgBox.Show(this,"Please select a hardware spec first.");
				return;
			}
			OrthoRxCur.OrthoHardwareSpecNum=orthoHardwareSpec.OrthoHardwareSpecNum;
			ToothNumberingNomenclature toothNumberingNomenclature=(ToothNumberingNomenclature)PrefC.GetInt(PrefName.UseInternationalToothNumbers);
			if(toothNumberingNomenclature==ToothNumberingNomenclature.Universal){
				toothNumberingNomenclature=ToothNumberingNomenclature.Palmer;
			}
			if(orthoHardwareSpec.OrthoHardwareType.In(EnumOrthoHardwareType.Bracket,EnumOrthoHardwareType.Elastic)){
				try{
					OrthoRxCur.ToothRange=Tooth.ParseOrthoCommas(textToothRange.Text,toothNumberingNomenclature);
				}
				catch(Exception ex){
					MsgBox.Show(ex.Message);
					return;
				}
			}
			if(orthoHardwareSpec.OrthoHardwareType==EnumOrthoHardwareType.Wire){
				try{
					OrthoRxCur.ToothRange=Tooth.ParseOrthoDash(textToothRange.Text,toothNumberingNomenclature);
				}
				catch(Exception ex){
					MsgBox.Show(ex.Message);
					return;
				}
			}
			if(OrthoRxCur.IsNew){
				OrthoRxs.Insert(OrthoRxCur);
			}
			else{
				OrthoRxs.Update(OrthoRxCur);
			}
			DialogResult=DialogResult.OK;
		}

	}
}