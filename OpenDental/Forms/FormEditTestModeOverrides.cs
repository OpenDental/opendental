using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormEditTestModeOverrides:FormODBase {

		private Introspection.IntrospectionEntity _introspectionEntity;
		private string _textOverrideOrig;

		public FormEditTestModeOverrides() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormEditTestModeOverrides_Load(object sender,EventArgs e) {
			comboSelectOverride.Items.AddEnums<Introspection.IntrospectionEntity>();
			comboSelectOverride.SetSelected(0);
			RefreshSelection();
		}

		private void RefreshSelection() {
			_introspectionEntity=comboSelectOverride.GetSelected<Introspection.IntrospectionEntity>();
			Introspection.TryGetOverride(_introspectionEntity,out _textOverrideOrig);
			textEnterOverrideValue.Text=_textOverrideOrig;
			textEnterOverrideValue.Enabled=true;
		}

		private void comboSelectOverride_SelectionChangeCommitted(object sender,EventArgs e) {
			PromptSave();
			RefreshSelection();
		}

		private void textEnterOverrideValue_TextChanged(object sender,EventArgs e) {
			if(textEnterOverrideValue.Text!=_textOverrideOrig) {
				butSave.Enabled=true;
			}
			else {
				butSave.Enabled=false;
			}
		}

		private void PromptSave() {
			if(butSave.Enabled && MsgBox.Show(this,MsgBoxButtons.YesNo,"Save changes?")) {
				SaveChanges();
			}
		}

		private void SaveChanges() {
			bool changed=Introspection.SetOverride(_introspectionEntity,textEnterOverrideValue.Text);
			if(changed) {
				butSave.Enabled=false;
				MessageBox.Show(Lan.g(this,"Override saved succcessfully."));
			}
			_textOverrideOrig=Introspection.GetOverride(_introspectionEntity);
			textEnterOverrideValue.Text=_textOverrideOrig;
		}

		private void butSave_Click(object sender,EventArgs e) {
			SaveChanges();
		}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormEditTestModeOverrides_FormClosing(object sender,FormClosingEventArgs e) {
			PromptSave();
		}

	}
}