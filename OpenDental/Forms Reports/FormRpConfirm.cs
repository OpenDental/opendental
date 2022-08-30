using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental{
///<summary></summary>
	public partial class FormRpConfirm : FormODBase{
		private long[] AptNums;
  
		///<summary></summary>
		public FormRpConfirm(long[] aptNums) {
			InitializeComponent();
			InitializeLayoutManager();
			AptNums=(long[])aptNums.Clone();
		} 

		private void FormRpConfirm_Load(object sender, System.EventArgs e){
			listSelect.Items.Add("LName");
			listSelect.Items.Add("FName"); 
			listSelect.Items.Add("MiddleI");
      listSelect.Items.Add("Preferred");
			listSelect.Items.Add("Salutation"); 
      listSelect.Items.Add("Address"); 
      listSelect.Items.Add("Address2");
			listSelect.Items.Add("City"); 
			listSelect.Items.Add("State");
			listSelect.Items.Add("Zip");
			listSelect.Items.Add("HmPhone");
			listSelect.Items.Add("WkPhone"); 
			listSelect.Items.Add("WirelessPhone"); 
			listSelect.Items.Add("Birthdate");
			listSelect.Items.Add("AptDateTime");
			listSelect.SetAll(true);
      listSelect2.Items.Add("BillingType");      
      listSelect2.Items.Add("CreditType");
			listSelect2.Items.Add("SSN");  
			listSelect2.Items.Add("ChartNumber");   
			listSelect2.Items.Add("FeeSched");
			listSelect2.Items.Add("Position"); 
			listSelect2.Items.Add("Gender");
			listSelect2.Items.Add("PatStatus");
			listSelect2.Items.Add("PatNum"); 
      listSelect2.Items.Add("Email");
      listSelect2.Items.Add("EstBalance"); 
      listSelect2.Items.Add("AddrNote"); 
      listSelect2.Items.Add("FamFinUrgNote"); 
      listSelect2.Items.Add("MedUrgNote"); 
			listSelect2.Items.Add("ApptModNote");
      listSelect2.Items.Add("PriCarrier");
      listSelect2.Items.Add("PriRelationship");
			listSelect2.Items.Add("SecCarrier");
			listSelect2.Items.Add("SecRelationship");
			//listSelect2.Items.Add("RecallInterval"); 
			listSelect2.Items.Add("StudentStatus");
      listSelect2.Items.Add("SchoolName"); 
			listSelect2.Items.Add("PriProv"); 
      listSelect2.Items.Add("SecProv");
			//listSelect2.Items.Add("NextAptNum"); 
			listSelect2.Items.Add("Guarantor"); 
			listSelect2.Items.Add("ImageFolder");
			
 		}

		private void butOK_Click(object sender, System.EventArgs e) {
			List<string> fieldsSelected=new List<string>(); 
			if(listSelect.SelectedIndices.Count==0 && listSelect2.SelectedIndices.Count==0){
				MsgBox.Show(this,"At least one field must be selected.");
				return;
			}
			for(int i=0;i<listSelect.SelectedIndices.Count;i++) {
				fieldsSelected.Add(listSelect.Items.GetTextShowingAt(listSelect.SelectedIndices[i]));
			}
			for(int i = 0;i<listSelect2.SelectedIndices.Count;i++) {
				fieldsSelected.Add(listSelect2.Items.GetTextShowingAt(listSelect2.SelectedIndices[i]));
			}
			string command="SELECT ";
			for(int i=0;i<fieldsSelected.Count;i++){
				if(i>0){
					command+=",";
				}
				if(fieldsSelected[i]=="AptDateTime"){
					command+="appointment.AptDateTime";
				}
				else if(fieldsSelected[i]=="PriCarrier") {
					command+="(SELECT carrier.CarrierName FROM patplan,inssub,insplan,carrier WHERE patplan.PatNum=patient.PatNum "
						+"AND patplan.InsSubNum=inssub.InsSubNum AND inssub.PlanNum=insplan.PlanNum AND insplan.CarrierNum=carrier.CarrierNum AND patplan.Ordinal=1 "+DbHelper.LimitAnd(1)+") PriCarrier";
				}
				else if(fieldsSelected[i]=="PriRelationship") {
					command+="(SELECT Relationship FROM patplan WHERE patplan.PatNum=patient.PatNum AND patplan.Ordinal=1 "+DbHelper.LimitAnd(1)+") PriRelationship";
				}
				else if(fieldsSelected[i]=="SecCarrier") {
					command+="(SELECT carrier.CarrierName FROM patplan,inssub,insplan,carrier WHERE patplan.PatNum=patient.PatNum "
						+"AND patplan.InsSubNum=inssub.InsSubNum AND inssub.PlanNum=insplan.PlanNum AND insplan.CarrierNum=carrier.CarrierNum AND patplan.Ordinal=2 "+DbHelper.LimitAnd(1)+") SecCarrier";
				}
				else if(fieldsSelected[i]=="SecRelationship") {
					command+="(SELECT Relationship FROM patplan WHERE patplan.PatNum=patient.PatNum AND patplan.Ordinal=2 "+DbHelper.LimitAnd(1)+") SecRelationship";
				}
				else{
					command+="patient."+fieldsSelected[i];
				}
			}
			command+=" FROM patient,appointment "
				+"WHERE patient.PatNum=appointment.PatNum AND(";       
			for(int i=0;i<AptNums.Length;i++){
				if(i>0){ 
					command+=" OR";
				}
				command+=" appointment.AptNum='"+AptNums[i]+"'";
			}
			command+=")";
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=command;
			using FormQuery FormQ=new FormQuery(report);
			FormQ.IsReport=false;
			FormQ.SubmitQuery();	
      FormQ.textQuery.Text=report.Query;					
			FormQ.ShowDialog();		
      DialogResult=DialogResult.OK; 
		}

		private void butCancel_Click(object sender, System.EventArgs e) {
			DialogResult=DialogResult.Cancel;
		}

		

		

	}
}
