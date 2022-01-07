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
		private ClaimProc ClaimProcCur;
		private ClaimProc _claimProcOld;

		///<summary></summary>
		public FormInsAdj(ClaimProc claimProcCur){
			ClaimProcCur=claimProcCur;
			_claimProcOld=ClaimProcCur.Copy();
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormInsAdj_Load(object sender, System.EventArgs e) {
			textDate.Text=ClaimProcCur.ProcDate.ToShortDateString();
			textInsUsed.Text=ClaimProcCur.InsPayAmt.ToString("F");
			textDedUsed.Text=ClaimProcCur.DedApplied.ToString("F");
		}

		private void butDelete_Click(object sender,EventArgs e) {
			if(IsNew) {
				DialogResult=DialogResult.Cancel;
				return;
			}
			if(!MsgBox.Show("All",MsgBoxButtons.OKCancel,"Delete?")) {
				return;
			}
			ClaimProcs.Delete(ClaimProcCur);
			InsEditPatLogs.MakeLogEntry(null,ClaimProcCur,InsEditPatLogType.Adjustment);
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
			ClaimProcCur.ProcDate=PIn.Date(textDate.Text);
			ClaimProcCur.InsPayAmt=PIn.Double(textInsUsed.Text);
			ClaimProcCur.DedApplied=PIn.Double(textDedUsed.Text);
			if(IsNew){
				ClaimProcs.Insert(ClaimProcCur);
				InsEditPatLogs.MakeLogEntry(ClaimProcCur,null,InsEditPatLogType.Adjustment);
			}
			else{
				ClaimProcs.Update(ClaimProcCur);
				InsEditPatLogs.MakeLogEntry(ClaimProcCur,_claimProcOld,InsEditPatLogType.Adjustment);
			}
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		
		
	}
}
