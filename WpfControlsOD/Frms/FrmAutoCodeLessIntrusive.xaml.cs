using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CodeBase;
using OpenDentBusiness;
using WpfControls.UI;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FrmAutoCodeLessIntrusive:FrmODBase {
		///<summary>The text to display in this dialog</summary>
		public string StrMain;
		private Patient _patient;
		public Procedure ProcedureCur;
		private ProcedureCode _procedureCode;
		private long _verifyCodeNum;
		private List<PatPlan> _listPatPlans;
		private List<InsSub> _listInsSubs;
		private List<InsPlan> _listInsPlans;
		private List<Benefit> _listBenefits;
		private List<ClaimProc> _listClaimProcs;
		private string _strTeeth;

		///<summary></summary>
		public FrmAutoCodeLessIntrusive(Patient patient,Procedure procedure,ProcedureCode procedureCode,long verifyCodeNum,List<PatPlan> listPatPlans,
			List<InsSub> listInsSubs,List<InsPlan> listInsPlans,List<Benefit> listBenefits,List<ClaimProc> listClaimProcs,string strTeeth=null)
		{
			_patient=patient;
			ProcedureCur=procedure;
			_procedureCode=procedureCode;
			_verifyCodeNum=verifyCodeNum;
			_listPatPlans=listPatPlans;
			_listInsSubs=listInsSubs;
			_listInsPlans=listInsPlans;
			_listBenefits=listBenefits;
			_listClaimProcs=listClaimProcs;
			_strTeeth=strTeeth;
			InitializeComponent();
			//Lan.F(this,new Control[] {labelMain});
			//labelMain is translated from calling Form (FormProcEdit)
		}

		private void FrmAutoCodeLessIntrusive_Loaded(object sender, RoutedEventArgs e) {
			//Moved from FormProcEdit.SaveAndClose() in version 16.3+
			labelMain.Text=ProcedureCodes.GetProcCode(_verifyCodeNum).ProcCode
				+" ("+ProcedureCodes.GetProcCode(_verifyCodeNum).Descript+") "
				+Lans.g("FormProcEdit","is the recommended procedure code for this procedure.  Change procedure code and fee?");
			if(PrefC.GetBool(PrefName.ProcEditRequireAutoCodes)) {
				butNo.Text=Lans.g(this,"Edit Proc");//Button will otherwise say 'No'.
			}
		}

		private void butYes_Click(object sender, System.EventArgs e) {
			//Customers have been complaining about procedurelog entries changing their CodeNum column to 0.
			//Based on a security log provided by a customer, we were able to determine that this is one of two potential violators.
			//The following code is here simply to try and get the user to call us so that we can have proof and hopefully find the core of the issue.
			try {
				if(_verifyCodeNum < 1) {
					throw new ApplicationException("Invalid Verify Code");
				}
			}
			catch(ApplicationException ae) {
				string error="Please notify support with the following information.\r\n"
					+"Error: "+ae.Message+"\r\n"
					+"_verifyCode: "+_verifyCodeNum.ToString()+"\r\n"
					+"_procCur.CodeNum: "+(ProcedureCur==null ? "NULL" : ProcedureCur.CodeNum.ToString())+"\r\n"
					+"_procCodeCur.CodeNum: "+(_procedureCode==null ? "NULL" : _procedureCode.CodeNum.ToString())+"\r\n"
					+"\r\n"
					+"StackTrace:\r\n"+ae.StackTrace;
				MsgBoxCopyPaste msgBoxCopyPaste=new MsgBoxCopyPaste(error);
				msgBoxCopyPaste.Text="Fatal Error!!!";
				msgBoxCopyPaste.Show();//Use .Show() to make it easy for the user to keep this window open while they call in.
				return;
			}
			//Don't change code if proc is linked to OrthoCase
			if(OrthoProcLinks.IsProcLinked(ProcedureCur.ProcNum)) {
				MsgBox.Show("This procedure is attached to an ortho case and its code cannot be changed.");
				return;
			}
			//Moved from FormProcEdit.SaveAndClose() in version 16.3+
			Procedure procedureOld=ProcedureCur.Copy();
			ProcedureCur.CodeNum=_verifyCodeNum;
			List<ProcStat> listProcStats=new List<ProcStat>();
			listProcStats.Add(ProcStat.TP);
			listProcStats.Add(ProcStat.C);
			listProcStats.Add(ProcStat.TPi);
			listProcStats.Add(ProcStat.Cn);
			if(listProcStats.Contains(ProcedureCur.ProcStatus)) {//Only change the fee if Complete, TP, TPi, or Cn.
				InsSub insSub=null;
				InsPlan insPlan=null;
				if(_listPatPlans.Count>0) {
					insSub=InsSubs.GetSub(_listPatPlans[0].InsSubNum,_listInsSubs);
					insPlan=InsPlans.GetPlan(insSub.PlanNum,_listInsPlans);
				}
				ProcedureCur.ProcFee=Fees.GetAmount0(ProcedureCur.CodeNum,FeeScheds.GetFeeSched(_patient,_listInsPlans,_listPatPlans,_listInsSubs,ProcedureCur.ProvNum),
					ProcedureCur.ClinicNum,ProcedureCur.ProvNum);
				if(insPlan!=null && insPlan.PlanType=="p") {//PPO
					double standardFee=Fees.GetAmount0(ProcedureCur.CodeNum,Providers.GetProv(Patients.GetProvNum(_patient)).FeeSched,ProcedureCur.ClinicNum,
						ProcedureCur.ProvNum);
					ProcedureCur.ProcFee=Math.Max(ProcedureCur.ProcFee,standardFee);
				}
			}
			Procedures.Update(ProcedureCur,procedureOld);
			//Compute estimates required, otherwise if adding through quick add, it could have incorrect WO or InsEst if code changed.
			Procedures.ComputeEstimates(ProcedureCur,_patient.PatNum,_listClaimProcs,true,_listInsPlans,_listPatPlans,_listBenefits,_patient.Age,_listInsSubs);
			Recalls.Synch(ProcedureCur.PatNum);
			if(!ProcedureCur.ProcStatus.In(ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				IsDialogOK=true;
				return;
			}
			string strLogText=_procedureCode.ProcCode+" ("+ProcedureCur.ProcStatus+"), ";
			if(_strTeeth!=null && _strTeeth.Trim()!="") {
				strLogText+=Lans.g(this,"Teeth")+": "+_strTeeth+", ";
			}
			strLogText+=Lans.g(this,"Fee")+": "+ProcedureCur.ProcFee.ToString("F")+", "+_procedureCode.Descript;
			if(ProcedureCur.ProcStatus.In(ProcStat.EO,ProcStat.EC)) {
				SecurityLogs.MakeLogEntry(Permissions.ProcExistingEdit,_patient.PatNum,strLogText);
			}
			IsDialogOK=true;
		}

		private void butNo_Click(object sender, System.EventArgs e) {
			IsDialogOK=false;
		}

	}
}





















