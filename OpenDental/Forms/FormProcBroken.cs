using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcBroken:FormODBase {
		public bool IsNew;
		public bool IsProcDeleted;
		private Procedure _procedure;
		private Procedure _procedureOld;
		private bool _isNonRefundable;

		public double GetAmountTotal() {
			return _procedure.ProcFee;
		}

		public FormProcBroken(Procedure procedure,bool isNonRefundable) {
			_procedure=procedure;
			_procedureOld=procedure.Copy();
			_isNonRefundable=isNonRefundable;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcBroken_Load(object sender,EventArgs e) {
			textDateEntry.Text=_procedure.DateEntryC.ToShortDateString();
			textProcDate.Text=_procedure.ProcDate.ToShortDateString();
			textAmount.Text=_procedure.ProcFee.ToString("f");
			comboClinic.ClinicNumSelected=_procedure.ClinicNum;
			fillComboProv();
			comboProv.SetSelectedProvNum(_procedure.ProvNum);
			textUser.Text=Userods.GetName(_procedure.UserNum);
			textChartNotes.Text=_procedure.Note;
			textAccountNotes.Text=_procedure.BillingNote;
			labelAmountDescription.Text="";
			if(_isNonRefundable) {
				labelAmountDescription.Text=GetAmountTotal().ToString("c")+": Non-Refundable portion";
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			fillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			FrmProviderPick frmProviderPick = new FrmProviderPick(comboProv.Items.GetAll<Provider>());
			frmProviderPick.ProvNumSelected=comboProv.GetSelectedProvNum();
			frmProviderPick.ShowDialog();
			if(!frmProviderPick.IsDialogOK) {
				return;
			}
			comboProv.SetSelectedProvNum(frmProviderPick.ProvNumSelected);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void fillComboProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.ClinicNumSelected));
			comboProv.SetSelectedProvNum(provNum);
		}

		private void butAutoNoteChart_Click(object sender,EventArgs e) {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textChartNotes.AppendText(frmAutoNoteCompose.StrCompletedNote);
			}
		}

		private void butAutoNoteAccount_Click(object sender,EventArgs e) {
			FrmAutoNoteCompose frmAutoNoteCompose=new FrmAutoNoteCompose();
			frmAutoNoteCompose.ShowDialog();
			if(frmAutoNoteCompose.IsDialogOK) {
				textAccountNotes.AppendText(frmAutoNoteCompose.StrCompletedNote);
			}
		}

		private void butSave_Click(object sender,System.EventArgs e) {
			if(!textProcDate.IsValid() || !textAmount.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textProcDate.Text=="") {
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			if(_procedure.ProcStatus==ProcStat.C && PIn.Date(textProcDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
				return;
			}
			if(textAmount.Text=="") {
				MsgBox.Show(this,"Please enter an amount.");
				return;
			}
			if(comboProv.GetSelectedProvNum()!=_procedureOld.ProvNum && PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim)) {
				List<ClaimProc> listClaimProcs=ClaimProcs.GetForProc(ClaimProcs.Refresh(_procedureOld.PatNum),_procedureOld.ProcNum);
				if(listClaimProcs.Any(x => x.Status==ClaimProcStatus.Received
					|| x.Status==ClaimProcStatus.Supplemental
					|| x.Status==ClaimProcStatus.CapClaim)) 
				{
					MsgBox.Show(this,"The provider cannot be changed when this procedure is attached to a claim.");
					return;
				}
			}
			_procedure.ProcDate=PIn.Date(textProcDate.Text);
			_procedure.ProcFee=PIn.Double(textAmount.Text);
			_procedure.Note=textChartNotes.Text;
			_procedure.BillingNote=textAccountNotes.Text;
			_procedure.ProvNum=comboProv.GetSelectedProvNum();
			_procedure.ClinicNum=comboClinic.ClinicNumSelected;
			Procedures.Update(_procedure,_procedureOld);
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_procedure.CodeNum);
			string logText=procedureCode.ProcCode+" ("+_procedure.ProcStatus+"), "+Lan.g(this,"Fee")+": "+_procedure.ProcFee.ToString("c")+", "+procedureCode.Descript;
			SecurityLogs.MakeLogEntry(IsNew ? EnumPermType.ProcComplCreate : EnumPermType.ProcCompleteEdit,_procedure.PatNum,logText);
			DialogResult=DialogResult.OK;
		}

		private void FormProcBroken_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel && IsNew) {
				if(DiscountPlanSubs.HasDiscountPlan(_procedure.PatNum)) {
					Adjustments.DeleteForProcedure(_procedure.ProcNum);//Delete discount plan adjustment
				}
				Procedures.Delete(_procedure.ProcNum);
				IsProcDeleted=true;
			}
		}

	}
}