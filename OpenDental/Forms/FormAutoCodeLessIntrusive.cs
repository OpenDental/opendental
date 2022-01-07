using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CodeBase;
using OpenDentBusiness;

namespace OpenDental {
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormAutoCodeLessIntrusive:FormODBase {
		///<summary>The text to display in this dialog</summary>
		public string mainText;
		private Patient _patCur;
		private Procedure _procCur;
		private ProcedureCode _procCodeCur;
		private long _verifyCode;
		private List<PatPlan> _listPatPlans;
		private List<InsSub> _listInsSubs;
		private List<InsPlan> _listInsPlans;
		private List<Benefit> _listBenefits;
		private List<ClaimProc> _listClaimProcs;
		private string _teethText;

		public Procedure Proc {
			get {
				return _procCur;
			}
		}

		///<summary></summary>
		public FormAutoCodeLessIntrusive(Patient pat,Procedure proc,ProcedureCode procCode,long verifyCode,List<PatPlan> listPatPlans,
			List<InsSub> listInsSubs,List<InsPlan> listInsPlans,List<Benefit> listBenefits,List<ClaimProc> listClaimProcs,string teethText=null)
		{
			_patCur=pat;
			_procCur=proc;
			_procCodeCur=procCode;
			_verifyCode=verifyCode;
			_listPatPlans=listPatPlans;
			_listInsSubs=listInsSubs;
			_listInsPlans=listInsPlans;
			_listBenefits=listBenefits;
			_listClaimProcs=listClaimProcs;
			_teethText=teethText;
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this,new Control[] {labelMain});
			//labelMain is translated from calling Form (FormProcEdit)
		}

		private void FormAutoCodeLessIntrusive_Load(object sender, System.EventArgs e) {
			//Moved from FormProcEdit.SaveAndClose() in version 16.3+
			labelMain.Text=ProcedureCodes.GetProcCode(_verifyCode).ProcCode
				+" ("+ProcedureCodes.GetProcCode(_verifyCode).Descript+") "
				+Lan.g("FormProcEdit","is the recommended procedure code for this procedure.  Change procedure code and fee?");
			if(PrefC.GetBool(PrefName.ProcEditRequireAutoCodes)) {
				butNo.Text=Lan.g(this,"Edit Proc");//Button will otherwise say 'No'.
			}
		}

		private void butYes_Click(object sender, System.EventArgs e) {
			//Customers have been complaining about procedurelog entries changing their CodeNum column to 0.
			//Based on a security log provided by a customer, we were able to determine that this is one of two potential violators.
			//The following code is here simply to try and get the user to call us so that we can have proof and hopefully find the core of the issue.
			try {
				if(_verifyCode < 1) {
					throw new ApplicationException("Invalid Verify Code");
				}
			}
			catch(ApplicationException ae) {
				string error="Please notify support with the following information.\r\n"
					+"Error: "+ae.Message+"\r\n"
					+"_verifyCode: "+_verifyCode.ToString()+"\r\n"
					+"_procCur.CodeNum: "+(_procCur==null ? "NULL" : _procCur.CodeNum.ToString())+"\r\n"
					+"_procCodeCur.CodeNum: "+(_procCodeCur==null ? "NULL" : _procCodeCur.CodeNum.ToString())+"\r\n"
					+"\r\n"
					+"StackTrace:\r\n"+ae.StackTrace;
				MsgBoxCopyPaste MsgBCP=new MsgBoxCopyPaste(error);
				MsgBCP.Text="Fatal Error!!!";
				MsgBCP.Show();//Use .Show() to make it easy for the user to keep this window open while they call in.
				return;
			}
			//Don't change code if proc is linked to OrthoCase
			if(OrthoProcLinks.IsProcLinked(_procCur.ProcNum)) {
				MsgBox.Show("This procedure is attached to an ortho case and its code cannot be changed.");
				return;
			}
			//Moved from FormProcEdit.SaveAndClose() in version 16.3+
			Procedure procOld=_procCur.Copy();
			_procCur.CodeNum=_verifyCode;
			if(new[] { ProcStat.TP,ProcStat.C,ProcStat.TPi,ProcStat.Cn }.Contains(_procCur.ProcStatus)) {//Only change the fee if Complete, TP, TPi, or Cn.
				InsSub prisub=null;
				InsPlan priplan=null;
				if(_listPatPlans.Count>0) {
					prisub=InsSubs.GetSub(_listPatPlans[0].InsSubNum,_listInsSubs);
					priplan=InsPlans.GetPlan(prisub.PlanNum,_listInsPlans);
				}
				_procCur.ProcFee=Fees.GetAmount0(_procCur.CodeNum,FeeScheds.GetFeeSched(_patCur,_listInsPlans,_listPatPlans,_listInsSubs,_procCur.ProvNum),
					_procCur.ClinicNum,_procCur.ProvNum);
				if(priplan!=null && priplan.PlanType=="p") {//PPO
					double standardfee=Fees.GetAmount0(_procCur.CodeNum,Providers.GetProv(Patients.GetProvNum(_patCur)).FeeSched,_procCur.ClinicNum,
						_procCur.ProvNum);
					_procCur.ProcFee=Math.Max(_procCur.ProcFee,standardfee);
				}
			}
			Procedures.Update(_procCur,procOld);
			//Compute estimates required, otherwise if adding through quick add, it could have incorrect WO or InsEst if code changed.
			Procedures.ComputeEstimates(_procCur,_patCur.PatNum,_listClaimProcs,true,_listInsPlans,_listPatPlans,_listBenefits,_patCur.Age,_listInsSubs);
			Recalls.Synch(_procCur.PatNum);
			if(ListTools.In(_procCur.ProcStatus,ProcStat.C,ProcStat.EO,ProcStat.EC)) {
				string logText=_procCodeCur.ProcCode+" ("+_procCur.ProcStatus+"), ";
				if(_teethText!=null && _teethText.Trim()!="") {
					logText+=Lan.g(this,"Teeth")+": "+_teethText+", ";
				}
				logText+=Lan.g(this,"Fee")+": "+_procCur.ProcFee.ToString("F")+", "+_procCodeCur.Descript;
				if(ListTools.In(_procCur.ProcStatus,ProcStat.EO,ProcStat.EC)) {
					SecurityLogs.MakeLogEntry(Permissions.ProcExistingEdit,_patCur.PatNum,logText);
				}
			}
			DialogResult=DialogResult.OK;
		}

		private void butNo_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		


	}
}





















