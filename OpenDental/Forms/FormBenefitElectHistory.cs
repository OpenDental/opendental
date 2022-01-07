using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using OpenDentBusiness;
using OpenDental.UI;
using System.Linq;

namespace OpenDental {
	public partial class FormBenefitElectHistory:FormODBase {
		private List<Etrans> listEtrans;
		private long _planNum;
		private long _patPlanNum;
		public List<Benefit> ListBenefits;
		private long _subNum;
		private Patient[] _patientArray;
		private long _subPatNum;
		private long _carrierNum;

		public FormBenefitElectHistory(long planNum,long patPlanNum,long subNum,long subPatNum,long carrierNum) {
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_planNum=planNum;
			_patPlanNum=patPlanNum;
			_subNum=subNum;
			_subPatNum=subPatNum;
			_carrierNum=carrierNum;
		}

		private void FormBenefitElectHistory_Load(object sender,EventArgs e) {
			FillGrid();
		}

		private void FillGrid(){
			listEtrans=Etranss.GetList270ForPlan(_planNum,_subNum);
			List<long> listPatNums=listEtrans.Select(x => x.PatNum).ToList();
			listPatNums.Add(_subPatNum);
			_patientArray=Patients.GetMultPats(listPatNums);//Can contain 0.
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn column=new GridColumn(Lan.g(this,"Date"),100);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g(this,"Patient"),100);
			gridMain.ListGridColumns.Add(column);
			column=new GridColumn(Lan.g(this,"Response"),100);
			gridMain.ListGridColumns.Add(column);
			gridMain.ListGridRows.Clear();
			GridRow row;
			for(int i=0;i<listEtrans.Count;i++){
				row=new GridRow();
				row.Cells.Add(listEtrans[i].DateTimeTrans.ToShortDateString());
				//All old 270s do not have a patNum set, so they were subscriber request.
				long patNum;
				if(listEtrans[i].PatNum==0) {
					patNum=_subPatNum;
				}
				else {
					patNum=listEtrans[i].PatNum;
				}
				string patName=Patients.GetOnePat(_patientArray,patNum).GetNameLFnoPref();
				row.Cells.Add(patName);
				row.Cells.Add(listEtrans[i].Note);
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			Etrans etrans=listEtrans[e.Row];
			if(etrans.Etype==EtransType.Eligibility_CA) {
				using FormEtransEdit formEtransEdit=new FormEtransEdit();
				formEtransEdit.EtransCur=etrans;
				formEtransEdit.ShowDialog();
			}
			else {
				string settingErrors271=X271.ValidateSettings();
				if(settingErrors271!="") {
					MessageBox.Show(settingErrors271);
					return;
				}
				bool isDependent=(etrans.PatNum!=0 && _subPatNum!=etrans.PatNum);//Old rows will be 0, but when 0 then request was for subscriber.
				Carrier carrier=Carriers.GetCarrier(_carrierNum);
				using FormEtrans270Edit formEtrans270Edit=new FormEtrans270Edit(_patPlanNum,_planNum,_subNum,isDependent,_subPatNum,carrier.IsCoinsuranceInverted);
				formEtrans270Edit.EtransCur=etrans;
				formEtrans270Edit.benList=ListBenefits;
				formEtrans270Edit.ShowDialog();
			}
			FillGrid();
		}

		//private void butOK_Click(object sender,EventArgs e) {
		//	DialogResult=DialogResult.OK;
		//}

		private void butClose_Click(object sender,EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		
	}
}