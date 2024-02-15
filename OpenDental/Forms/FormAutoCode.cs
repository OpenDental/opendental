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
		private bool _changed;
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
			for(int i=0;i<_listAutoCodes.Count;i++) {
				if(_listAutoCodes[i].IsHidden) {
					listAutoCodes.Items.Add(_listAutoCodes[i].Description+"(hidden)");
				}
				else {
					listAutoCodes.Items.Add(_listAutoCodes[i].Description);
				}
			}
		}

		private void butAdd_Click(object sender,System.EventArgs e) {
			using FormAutoCodeEdit formAutoCodeEdit=new FormAutoCodeEdit();
			formAutoCodeEdit.IsNew=true;
			formAutoCodeEdit.AutoCodeCur=new AutoCode();
			AutoCodes.Insert(formAutoCodeEdit.AutoCodeCur);
			formAutoCodeEdit.ShowDialog();
			if(formAutoCodeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_changed=true;
			FillList();
		}

		private void listAutoCodes_DoubleClick(object sender,System.EventArgs e) {
			if(listAutoCodes.SelectedIndex==-1) {
				return;
			}
			AutoCode autoCode=_listAutoCodes[listAutoCodes.SelectedIndex];
			using FormAutoCodeEdit formAutoCodeEdit=new FormAutoCodeEdit();
			formAutoCodeEdit.AutoCodeCur=autoCode;
			formAutoCodeEdit.ShowDialog();
			if(formAutoCodeEdit.DialogResult!=DialogResult.OK) {
				return;
			}
			_changed=true;
			FillList();
		}

		private void butDelete_Click(object sender,System.EventArgs e) {
			if(listAutoCodes.SelectedIndex < 0) {
				MessageBox.Show(Lan.g(this,"You must first select a row"));
				return;
			}
			AutoCode autoCode=_listAutoCodes[listAutoCodes.SelectedIndex];
			try {
				AutoCodes.Delete(autoCode);
			}
			catch(ApplicationException ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			_changed=true;
			FillList();
		}

		private void FormAutoCode_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if(_changed){
				DataValid.SetInvalid(InvalidType.AutoCodes);
			}
			DialogResult=DialogResult.OK;
		}
	}
}








