using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	public partial class FormProcBroken:FormODBase {
		public bool IsNew;
		private Procedure _procCur;
		private Procedure _procOld;
		private bool _isNonRefundable;

		public bool IsProcDeleted { get; private set; }

		public double AmountTotal {
			get {
				return _procCur.ProcFee;
			}
		}

		public FormProcBroken(Procedure proc,bool isNonRefundable) {
			_procCur=proc;
			_procOld=proc.Copy();
			_isNonRefundable=isNonRefundable;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
		}

		private void FormProcBroken_Load(object sender,EventArgs e) {
			textDateEntry.Text=_procCur.DateEntryC.ToShortDateString();
			textProcDate.Text=_procCur.ProcDate.ToShortDateString();
			textAmount.Text=_procCur.ProcFee.ToString("f");
			comboClinic.SelectedClinicNum=_procCur.ClinicNum;
			fillComboProv();
			comboProv.SetSelectedProvNum(_procCur.ProvNum);
			textUser.Text=Userods.GetName(_procCur.UserNum);
			textChartNotes.Text=_procCur.Note;
			textAccountNotes.Text=_procCur.BillingNote;
			labelAmountDescription.Text="";
			if(_isNonRefundable) {
				labelAmountDescription.Text=AmountTotal.ToString("c")+": Non-Refundable portion";
			}
		}

		private void ComboClinic_SelectionChangeCommitted(object sender, EventArgs e){
			fillComboProv();
		}

		private void butPickProv_Click(object sender,EventArgs e) {
			using FormProviderPick FormP = new FormProviderPick(comboProv.Items.GetAll<Provider>());
			FormP.SelectedProvNum=comboProv.GetSelectedProvNum();
			FormP.ShowDialog();
			if(FormP.DialogResult!=DialogResult.OK) {
				return;
			}
			comboProv.SetSelectedProvNum(FormP.SelectedProvNum);
		}

		///<summary>Fills combo provider based on which clinic is selected and attempts to preserve provider selection if any.</summary>
		private void fillComboProv() {
			long provNum=comboProv.GetSelectedProvNum();
			comboProv.Items.Clear();
			comboProv.Items.AddProvsAbbr(Providers.GetProvsForClinic(comboClinic.SelectedClinicNum));
			comboProv.SetSelectedProvNum(provNum);
		}

		private void butAutoNoteChart_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textChartNotes.AppendText(FormA.CompletedNote);
			}
		}

		private void butAutoNoteAccount_Click(object sender,EventArgs e) {
			using FormAutoNoteCompose FormA=new FormAutoNoteCompose();
			FormA.ShowDialog();
			if(FormA.DialogResult==DialogResult.OK) {
				textAccountNotes.AppendText(FormA.CompletedNote);
			}
		}

		private void butOK_Click(object sender,System.EventArgs e) {
			if(!textProcDate.IsValid() || !textAmount.IsValid())
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(textProcDate.Text=="") {
				MsgBox.Show(this,"Please enter a date first.");
				return;
			}
			if(_procCur.ProcStatus==ProcStat.C && PIn.Date(textProcDate.Text).Date > DateTime.Today.Date && !PrefC.GetBool(PrefName.FutureTransDatesAllowed)) {
				MsgBox.Show(this,"Completed procedures cannot be set for future dates.");
				return;
			}
			if(textAmount.Text=="") {
				MsgBox.Show(this,"Please enter an amount.");
				return;
			}
			if(comboProv.GetSelectedProvNum()!=_procOld.ProvNum && PrefC.GetBool(PrefName.ProcProvChangesClaimProcWithClaim)) {
				List<ClaimProc> listClaimProc=ClaimProcs.GetForProc(ClaimProcs.Refresh(_procOld.PatNum),_procOld.ProcNum);
				if(listClaimProc.Any(x => x.Status==ClaimProcStatus.Received
					|| x.Status==ClaimProcStatus.Supplemental
					|| x.Status==ClaimProcStatus.CapClaim)) 
				{
					MsgBox.Show(this,"The provider cannot be changed when this procedure is attached to a claim.");
					return;
				}
			}
			_procCur.ProcDate=PIn.Date(textProcDate.Text);
			_procCur.ProcFee=PIn.Double(textAmount.Text);
			_procCur.Note=textChartNotes.Text;
			_procCur.BillingNote=textAccountNotes.Text;
			_procCur.ProvNum=comboProv.GetSelectedProvNum();
			_procCur.ClinicNum=comboClinic.SelectedClinicNum;
			Procedures.Update(_procCur,_procOld);
			ProcedureCode procedureCode=ProcedureCodes.GetProcCode(_procCur.CodeNum);
			string logText=procedureCode.ProcCode+" ("+_procCur.ProcStatus+"), "+Lan.g(this,"Fee")+": "+_procCur.ProcFee.ToString("c")+", "+procedureCode.Descript;
			SecurityLogs.MakeLogEntry(IsNew ? Permissions.ProcComplCreate : Permissions.ProcCompleteEdit,_procCur.PatNum,logText);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender,System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		private void FormProcBroken_FormClosing(object sender,FormClosingEventArgs e) {
			if(DialogResult==DialogResult.Cancel && IsNew) {
				if(DiscountPlanSubs.HasDiscountPlan(_procCur.PatNum)) {
					Adjustments.DeleteForProcedure(_procCur.ProcNum);//Delete discount plan adjustment
				}
				Procedures.Delete(_procCur.ProcNum);
				IsProcDeleted=true;
			}
		}

		
	}
}