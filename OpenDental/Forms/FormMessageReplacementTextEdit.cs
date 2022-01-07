using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormMessageReplacementTextEdit:FormODBase {
		///<summary>Set this to override the title of the form.</summary>
		public string FormTitle;
		public string TextEditorText;

		public FormMessageReplacementTextEdit(string formTitle="") {
			FormTitle=formTitle;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormMessageReplacementTextEdit_Load(object sender,EventArgs e) {
			if(!string.IsNullOrWhiteSpace(FormTitle)) {
				this.Text=FormTitle;
			}
			textBoxEditor.Text=TextEditorText;
		}

		private void butReplacementText_Click(object sender,EventArgs e) {
			using FormMessageReplacements formMessageReplacements=new FormMessageReplacements(MessageReplaceType.PaymentPlan,false);
			if(formMessageReplacements.ShowDialog()!=DialogResult.OK){
				return;
			}
			textBoxEditor.SelectedText=formMessageReplacements.Replacement;
		}

		private void butOK_Click(object sender,EventArgs e) {
			TextEditorText=textBoxEditor.Text;
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}