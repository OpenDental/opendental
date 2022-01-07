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
	public partial class FormAutoCode : FormODBase {
		private bool changed;
		private List<AutoCode> _listAutoCodes=new List<AutoCode>();

		protected override string GetHelpOverride() {
			if(CultureInfo.CurrentCulture.Name.EndsWith("CA")) {
				return "FormAutoCodeCanada";
			}
			return "FormAutoCode";
		}

		///<summary></summary>
		public FormAutoCode(){
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormAutoCode_Load(object sender, System.EventArgs e) {
			FillList();
		}

		private void FillList() {
			AutoCodes.RefreshCache();
			listAutoCodes.Items.Clear();
			_listAutoCodes=AutoCodes.GetListDeep();
			foreach(AutoCode autoCode in _listAutoCodes) {
				if(autoCode.IsHidden) {
					listAutoCodes.Items.Add(autoCode.Description+"(hidden)");
				}
				else {
					listAutoCodes.Items.Add(autoCode.Description);
				}
			}
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			using FormAutoCodeEdit FormACE=new FormAutoCodeEdit();
			FormACE.IsNew=true;
			FormACE.AutoCodeCur=new AutoCode();
			AutoCodes.Insert(FormACE.AutoCodeCur);
			FormACE.ShowDialog();
			if(FormACE.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillList();
		}

		private void listAutoCodes_DoubleClick(object sender,System.EventArgs e) {
			if(listAutoCodes.SelectedIndex==-1) {
				return;
			}
			AutoCode AutoCodeCur=_listAutoCodes[listAutoCodes.SelectedIndex];
			using FormAutoCodeEdit FormACE=new FormAutoCodeEdit();
			FormACE.AutoCodeCur=AutoCodeCur;
			FormACE.ShowDialog();
			if(FormACE.DialogResult!=DialogResult.OK) {
				return;
			}
			changed=true;
			FillList();
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.OK;
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(listAutoCodes.SelectedIndex < 0) {
				MessageBox.Show(Lan.g(this,"You must first select a row"));
				return;
			}
			AutoCode autoCodeCur=_listAutoCodes[listAutoCodes.SelectedIndex];
			try {
				AutoCodes.Delete(autoCodeCur);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			changed=true;
			FillList();
		}

		private void FormAutoCode_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(changed){
				DataValid.SetInvalid(InvalidType.AutoCodes);
			}
			DialogResult=DialogResult.OK;
		}
	}
}








