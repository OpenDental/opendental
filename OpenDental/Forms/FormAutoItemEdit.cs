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
			foreach(string s in Enum.GetNames(typeof(AutoCondition))) {
				listConditions.Items.Add(Lan.g("enumAutoConditions",s));
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
				AutoCodeCond AutoCodeCondCur=new AutoCodeCond();
				AutoCodeCondCur.AutoCodeItemNum=AutoCodeItemCur.AutoCodeItemNum;
				AutoCodeCondCur.Cond=(AutoCondition)listConditions.SelectedIndices[i];
				AutoCodeConds.Insert(AutoCodeCondCur);
			}
			DialogResult=DialogResult.OK;
		}

		private void butChange_Click(object sender,System.EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult==DialogResult.Cancel) {
				textADA.Text=ProcedureCodes.GetStringProcCode(AutoCodeItemCur.CodeNum);
				return;
			}
			if(AutoCodeItems.GetContainsKey(FormP.SelectedCodeNum)
				&& AutoCodeItems.GetOne(FormP.SelectedCodeNum).AutoCodeNum != AutoCodeItemCur.AutoCodeNum) 
			{
				//This section is a fix for an old bug that did not cause items to get deleted properly
				if(!AutoCodes.GetContainsKey(AutoCodeItems.GetOne(FormP.SelectedCodeNum).AutoCodeNum)) {
					AutoCodeItems.Delete(AutoCodeItems.GetOne(FormP.SelectedCodeNum));
					textADA.Text=ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum);
				}
				else {
					MessageBox.Show(Lan.g(this,"That procedure code is already in use in a different Auto Code.  Not allowed to use it here."));
					textADA.Text=ProcedureCodes.GetStringProcCode(AutoCodeItemCur.CodeNum);
				}
			}
			else {
				textADA.Text=ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum);
			}
		}

	}
}










