using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;
using System.Collections.Generic;

namespace OpenDental{
	/// <summary>description for FormBasicTemplate.</summary>
	public partial class FormProcTPEdit : FormODBase {
		private DateTime _dateTP;
		private bool _isSigned;
		private List<Def> _listDefsTxPriority;
		public ProcTP ProcTPCur;

		///<summary></summary>
		public FormProcTPEdit(ProcTP procTP,DateTime dateTP,bool isSigned)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			ProcTPCur=procTP.Copy();
			_dateTP=dateTP;
			_isSigned=isSigned;
		}

		private void FormProcTPEdit_Load(object sender, System.EventArgs e){
			//this window never comes up for new TP.  Always saved ahead of time.
			if(!Security.IsAuthorized(EnumPermType.TreatPlanEdit,_dateTP) || _isSigned) {
				if(_isSigned) {
					labelWarning.Visible=true;
				}
				butSave.Enabled=false;
				butDelete.Enabled=false;
			}
			comboPriority.Items.Add(Lan.g(this,"none"));
			comboPriority.SelectedIndex=0;
			_listDefsTxPriority=Defs.GetDefsForCategory(DefCat.TxPriorities,true);
			for(int i=0;i<_listDefsTxPriority.Count;i++){
				comboPriority.Items.Add(_listDefsTxPriority[i].ItemName);
				if(ProcTPCur.Priority==_listDefsTxPriority[i].DefNum){
					comboPriority.SelectedIndex=i+1;
				}
			}
			textToothNumTP.Text=ProcTPCur.ToothNumTP;
			textSurf.Text=ProcTPCur.Surf;//already converted to international format before saved to db.
			textCode.Text=ProcTPCur.ProcCode;
			textDescript.Text=ProcTPCur.Descript;
			textFeeAmt.Text=ProcTPCur.FeeAmt.ToString("F");
			textPriInsAmt.Text=ProcTPCur.PriInsAmt.ToString("F");
			textSecInsAmt.Text=ProcTPCur.SecInsAmt.ToString("F");
			textDiscount.Text=ProcTPCur.Discount.ToString("F");
			textPatAmt.Text=ProcTPCur.PatAmt.ToString("F");
			textPrognosis.Text=ProcTPCur.Prognosis;
			textDx.Text=ProcTPCur.Dx;
			textProcAbbr.Text=ProcTPCur.ProcAbbr;
			if(ProcTPCur.FeeAllowed>-1) {
				textFeeAllowed.Text=ProcTPCur.FeeAllowed.ToString("F");
			}
			else {
				textFeeAllowed.Text="";
			}
			if(!Clinics.IsMedicalPracticeOrClinic(Clinics.ClinicNum)) { 
				return;	
			}
			labelToothNum.Visible=false;
			textToothNumTP.Visible=false;
			labelSurface.Visible=false;
			textSurf.Visible=false;
		}

		private void butDelete_Click(object sender, System.EventArgs e) {
			ProcTPs.Delete(ProcTPCur);
			SecurityLogs.MakeLogEntry(EnumPermType.TreatPlanEdit,ProcTPCur.PatNum,"Delete tp proc: "+ProcTPCur.Descript);
			ProcTPCur=null;
			DialogResult=DialogResult.OK;
		}

		private void butSave_Click(object sender, System.EventArgs e) {
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
				ProcTPCur.Priority=0;
			}
			else{
				ProcTPCur.Priority=_listDefsTxPriority[comboPriority.SelectedIndex-1].DefNum;
			}
			ProcTPCur.ToothNumTP=textToothNumTP.Text;
			ProcTPCur.Surf=textSurf.Text;
			ProcTPCur.ProcCode=textCode.Text;
			ProcTPCur.Descript=textDescript.Text;
			ProcTPCur.FeeAmt=PIn.Double(textFeeAmt.Text);
			ProcTPCur.PriInsAmt=PIn.Double(textPriInsAmt.Text);
			ProcTPCur.SecInsAmt=PIn.Double(textSecInsAmt.Text);
			ProcTPCur.Discount=PIn.Double(textDiscount.Text);
			ProcTPCur.PatAmt=PIn.Double(textPatAmt.Text);
			ProcTPCur.Prognosis=textPrognosis.Text;
			ProcTPCur.Dx=textDx.Text;
			ProcTPCur.ProcAbbr=textProcAbbr.Text;
			if(String.IsNullOrWhiteSpace(textFeeAllowed.Text)) {
				ProcTPCur.FeeAllowed=-1;
			}
			else {
				ProcTPCur.FeeAllowed=PIn.Double(textFeeAllowed.Text);
			}
			ProcTPs.InsertOrUpdate(ProcTPCur,false);//IsNew not applicable here
			SecurityLogs.MakeLogEntry(EnumPermType.TreatPlanEdit,ProcTPCur.PatNum,"Edit proc: "+ProcTPCur.Descript);
			DialogResult=DialogResult.OK;
		}

	}
}