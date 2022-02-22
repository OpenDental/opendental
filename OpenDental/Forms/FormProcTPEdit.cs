using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public partial class FormProcTPEdit : FormODBase {
		private DateTime DateTP;
		private bool _isSigned;
		private List<Def> _listTxPriorityDefs;
		public ProcTP ProcCur;

		///<summary></summary>
		public FormProcTPEdit(ProcTP procCur,DateTime dateTP,bool isSigned)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ProcCur=procCur.Copy();
			DateTP=dateTP;
			_isSigned=isSigned;
		}

		private void FormProcTPEdit_Load(object sender, System.EventArgs e){
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(Permissions.TreatPlanEdit,DateTP) || _isSigned) {
				if(_isSigned) {
					labelWarning.Visible=true;
				}
				butOK.Enabled=false;
				butDelete.Enabled=false;
			}
			comboPriority.Items.Add(Lan.g(this,"none"));
			comboPriority.SelectedIndex=0;
			_listTxPriorityDefs=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			for(int i=0;i<_listTxPriorityDefs.Count;i++){
				comboPriority.Items.Add(_listTxPriorityDefs[i].ItemName);
				if(ProcCur.Priority==_listTxPriorityDefs[i].DefNum){
					comboPriority.SelectedIndex=i+1;
				}
			}
			textToothNumTP.Text=ProcCur.ToothNumTP;
			textSurf.Text=ProcCur.Surf;//already converted to international format before saved to db.
			textCode.Text=ProcCur.ProcCode;
			textDescript.Text=ProcCur.Descript;
			textFeeAmt.Text=ProcCur.FeeAmt.ToString("F");
			textPriInsAmt.Text=ProcCur.PriInsAmt.ToString("F");
			textSecInsAmt.Text=ProcCur.SecInsAmt.ToString("F");
			textDiscount.Text=ProcCur.Discount.ToString("F");
			textPatAmt.Text=ProcCur.PatAmt.ToString("F");
			textPrognosis.Text=ProcCur.Prognosis;
			textDx.Text=ProcCur.Dx;
			textProcAbbr.Text=ProcCur.ProcAbbr;
			textFeeAllowed.Text=ProcCur.FeeAllowed.ToString("F");
			if(Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) {
				labelToothNum.Visible=false;
				textToothNumTP.Visible=false;
				labelSurface.Visible=false;
				textSurf.Visible=false;
			}
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			ProcTPs.Delete(ProcCur);
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,ProcCur.PatNum,"Delete tp proc: "+ProcCur.Descript);
			ProcCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			if(!textFeeAmt.IsValid()
				|| !textPriInsAmt.IsValid()
				|| !textSecInsAmt.IsValid()
				|| !textDiscount.IsValid()
				|| !textPatAmt.IsValid()
				|| !textFeeAllowed.IsValid()) 
			{
				MsgBox.Show(this,"Please fix data entry errors first.");
				return;
			}
			if(comboPriority.SelectedIndex==0){
				ProcCur.Priority=0;
			}
			else{
				ProcCur.Priority=_listTxPriorityDefs[comboPriority.SelectedIndex-1].DefNum;
			}
			ProcCur.ToothNumTP=textToothNumTP.Text;
			ProcCur.Surf=textSurf.Text;
			ProcCur.ProcCode=textCode.Text;
			ProcCur.Descript=textDescript.Text;
			ProcCur.FeeAmt=PIn.Double(textFeeAmt.Text);
			ProcCur.PriInsAmt=PIn.Double(textPriInsAmt.Text);
			ProcCur.SecInsAmt=PIn.Double(textSecInsAmt.Text);
			ProcCur.Discount=PIn.Double(textDiscount.Text);
			ProcCur.PatAmt=PIn.Double(textPatAmt.Text);
			ProcCur.Prognosis=textPrognosis.Text;
			ProcCur.Dx=textDx.Text;
			ProcCur.ProcAbbr=textProcAbbr.Text;
			ProcCur.FeeAllowed=PIn.Double(textFeeAllowed.Text);
			ProcTPs.InsertOrUpdate(ProcCur,false);//IsNew not applicable here
			SecurityLogs.MakeLogEntry(Permissions.TreatPlanEdit,ProcCur.PatNum,"Edit proc: "+ProcCur.Descript);
			DialogResult=DialogResult.OK;
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		


	}
}





















