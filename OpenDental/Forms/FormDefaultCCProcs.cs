using CodeBase;
using OpenDentBusiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormDefaultCCProcs:FormODBase {
		/// <summary>A comma-separated list of procedure codes.</summary>
		private string _defaultCCProcs;
		/// <summary>A list of procedure codes from _defaultCCProcs.</summary>
		private List<string> _listStrCCProcs;

		public FormDefaultCCProcs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDefaultCCProcs_Load(object sender,EventArgs e) {
			_defaultCCProcs=PrefC.GetString(PrefName.DefaultCCProcs);
			_listStrCCProcs=_defaultCCProcs.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			_listStrCCProcs.Sort();
			FillProcs();
		}

		private void FillProcs() {
			listProcs.Items.Clear();
			for(int i=0;i<_listStrCCProcs.Count;i++) {
				listProcs.Items.Add(_listStrCCProcs[i]+"- "+ProcedureCodes.GetLaymanTerm(ProcedureCodes.GetProcCode(_listStrCCProcs[i]).CodeNum));
			}
		}

		///<summary>Syncs all the procedures currently in the proc list to all credit card not excluded.</summary>
		private void butSync_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			CreditCards.SyncDefaultProcs(_listStrCCProcs);
			Cursor=Cursors.Default;
		}

		private void butAddProc_Click(object sender,EventArgs e) {
			using FormProcCodes formProcCodes=new FormProcCodes();
			formProcCodes.IsSelectionMode=true;
			formProcCodes.ShowDialog();
			if(formProcCodes.DialogResult!=DialogResult.OK) {
				return;
			}
			string procCode=ProcedureCodes.GetStringProcCode(formProcCodes.CodeNumSelected);
			if(_listStrCCProcs.Exists(x => x==procCode)) {
				return;
			}
			_listStrCCProcs.Add(procCode);
			_listStrCCProcs.Sort();
			FillProcs();
		}

		private void butRemoveProc_Click(object sender,EventArgs e) {
			if(listProcs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a procedure first.");
				return;
			}
			_listStrCCProcs.RemoveAt(listProcs.SelectedIndex);
			FillProcs();
		}

		private void butSave_Click(object sender,EventArgs e) {
			_defaultCCProcs=string.Join(",",_listStrCCProcs);
			if(Prefs.UpdateString(PrefName.DefaultCCProcs,_defaultCCProcs)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

	}
}