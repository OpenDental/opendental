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
		private List<string> _listCCProcs;

		public FormDefaultCCProcs() {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormDefaultCCProcs_Load(object sender,EventArgs e) {
			_defaultCCProcs=PrefC.GetString(PrefName.DefaultCCProcs);
			_listCCProcs=_defaultCCProcs.Split(new char[] { ',' },StringSplitOptions.RemoveEmptyEntries).ToList();
			_listCCProcs.Sort();
			FillProcs();
		}

		private void FillProcs() {
			listProcs.Items.Clear();
			foreach(string procStr in _listCCProcs) {
				listProcs.Items.Add(procStr+"- "+ProcedureCodes.GetLaymanTerm(ProcedureCodes.GetProcCode(procStr).CodeNum));
			}
		}

		///<summary>Syncs all the procedures currently in the proc list to all credit card not excluded.</summary>
		private void butSync_Click(object sender,EventArgs e) {
			Cursor=Cursors.WaitCursor;
			CreditCards.SyncDefaultProcs(_listCCProcs);
			Cursor=Cursors.Default;
		}

		private void butAddProc_Click(object sender,EventArgs e) {
			using FormProcCodes FormP=new FormProcCodes();
			FormP.IsSelectionMode=true;
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			string procCode=ProcedureCodes.GetStringProcCode(FormP.SelectedCodeNum);
			if(_listCCProcs.Exists(x => x==procCode)) {
				return;
			}
			_listCCProcs.Add(procCode);
			_listCCProcs.Sort();
			FillProcs();
		}

		private void butRemoveProc_Click(object sender,EventArgs e) {
			if(listProcs.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a procedure first.");
				return;
			}
			_listCCProcs.RemoveAt(listProcs.SelectedIndex);
			FillProcs();
		}

		private void butOK_Click(object sender,EventArgs e) {
			_defaultCCProcs=string.Join(",",_listCCProcs);
			if(Prefs.UpdateString(PrefName.DefaultCCProcs,_defaultCCProcs)) {
				DataValid.SetInvalid(InvalidType.Prefs);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

	}
}