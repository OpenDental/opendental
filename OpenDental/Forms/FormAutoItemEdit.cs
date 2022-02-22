using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;
using System.Globalization;

namespace OpenDental{
	///<summary></summary>
	public partial class FormAutoItemEdit : FormODBase {
		///<summary></summary>
    public bool IsNew;
		///<summary>Set this value externally before opening this form, even if IsNew.</summary>
		public AutoCodeItem AutoCodeItemCur;

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormAutoItemEditCanada";
			}
			return "FormAutoItemEdit";
		}

		///<summary></summary>
		public FormAutoItemEdit(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

 		private void FormAutoItemEdit_Load(object sender, System.EventArgs e) {
			AutoCodeConds.RefreshCache();    
			if(IsNew){
				this.Text=Lan.g(this,"Add Auto Code Item");  
			}
			else{ 
				this.Text=Lan.g(this,"Edit Auto Code Item");
				textADA.Text=ProcedureCodes.GetStringProcCode(AutoCodeItemCur.CodeNum);    
			}
			FillList();
		}

		private void FillList() {
			listConditions.Items.Clear();
			for(int i=0;i<Enum.GetNames(typeof(AutoCondition)).Length;i++) {
				listConditions.Items.Add(Lan.g("enumAutoConditions",Enum.GetNames(typeof(AutoCondition))[i]));
			}
			List<AutoCodeCond> listAutoCodeConds=AutoCodeConds.GetWhere(x => x.AutoCodeItemNum==AutoCodeItemCur.AutoCodeItemNum);
			for(int i=0;i<listAutoCodeConds.Count;i++) {
				listConditions.SetSelected((int)listAutoCodeConds[i].Cond,true);
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(textADA.Text=="") {
				MessageBox.Show(Lan.g(this,"Code cannot be left blank."));
				listConditions.SelectedIndex=-1;
				FillList();
				return;
			}
			AutoCodeItemCur.CodeNum=ProcedureCodes.GetCodeNum(textADA.Text);
			if(IsNew) {
				AutoCodeItems.Insert(AutoCodeItemCur);
			}
			else {
				AutoCodeItems.Update(AutoCodeItemCur);
			}
			AutoCodeConds.DeleteForItemNum(AutoCodeItemCur.AutoCodeItemNum);
			for(int i=0;i<listConditions.SelectedIndices.Count;i++) {
				AutoCodeCond autoCodeCond=new AutoCodeCond();
				autoCodeCond.AutoCodeItemNum=AutoCodeItemCur.AutoCodeItemNum;
				autoCodeCond.Cond=(AutoCondition)listConditions.SelectedIndices[i];
				AutoCodeConds.Insert(autoCodeCond);
			}
			DialogResult=DialogResult.OK;
		}

		private void butChange_Click(object sender,System.EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult==DialogResult.Cancel) {
				textADA.Text=ProcedureCodes.GetStringProcCode(AutoCodeItemCur.CodeNum);
				return;
			}
			if(AutoCodeItems.GetContainsKey(formProcCodes.SelectedCodeNum)
				&& AutoCodeItems.GetOne(formProcCodes.SelectedCodeNum).AutoCodeNum != AutoCodeItemCur.AutoCodeNum) 
			{
				//This section is a fix for an old bug that did not cause items to get deleted properly
				if(AutoCodes.GetContainsKey(AutoCodeItems.GetOne(formProcCodes.SelectedCodeNum).AutoCodeNum)) {
					MessageBox.Show(Lan.g(this,"That procedure code is already in use in a different Auto Code.  Not allowed to use it here."));
					textADA.Text=ProcedureCodes.GetStringProcCode(AutoCodeItemCur.CodeNum);
				}
				else {
					AutoCodeItems.Delete(AutoCodeItems.GetOne(formProcCodes.SelectedCodeNum));
					textADA.Text=ProcedureCodes.GetStringProcCode(formProcCodes.SelectedCodeNum);
				}
			}
			else {
				textADA.Text=ProcedureCodes.GetStringProcCode(formProcCodes.SelectedCodeNum);
			}
		}

	}
}










