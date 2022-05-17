using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormInsAdj : FormODBase {
		///<summary></summary>
		public bool IsNew;
		private ClaimProc _claimProcCur;
		private ClaimProc _claimProcOld;

		///<summary></summary>
		public FormInsAdj(ClaimProc claimProc){
			_claimProcCur=claimProc;
			_claimProcOld=_claimProcCur.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsAdj_Load(object sender, System.EventArgs e) {
			textDate.Text=_claimProcCur.ProcDate.ToShortDateString();
			textInsUsed.Text=_claimProcCur.InsPayAmt.ToString("F");
			textDedUsed.Text=_claimProcCur.DedApplied.ToString("F");
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show("All",MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			ClaimProcs.Delete(_claimProcCur);
			InsEditPatLogs.MakeLogEntry(null,_claimProcCur,InsEditPatLogType.Adjustment);
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textDate.IsValid()
				|| !textInsUsed.IsValid()
				|| !textDedUsed.IsValid())
			{
				MessageBox.Show(Lan.g("All","Please fix data entry errors first."));
				return;
			}
			_claimProcCur.ProcDate=PIn.Date(textDate.Text);
			_claimProcCur.InsPayAmt=PIn.Double(textInsUsed.Text);
			_claimProcCur.DedApplied=PIn.Double(textDedUsed.Text);
			if(IsNew){
				ClaimProcs.Insert(_claimProcCur);
				InsEditPatLogs.MakeLogEntry(_claimProcCur,null,InsEditPatLogType.Adjustment);
				DialogResult=DialogResult.OK;
				return;
			}
			ClaimProcs.Update(_claimProcCur);
			InsEditPatLogs.MakeLogEntry(_claimProcCur,_claimProcOld,InsEditPatLogType.Adjustment);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}
	}
}
