using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using OpenDentBusiness;

namespace OpenDental {
	///<summary></summary>
	public partial class FormRpRecall : FormODBase {
		private string SQLselect;
		private string SQLfrom;
		private string SQLwhere;
		private string SQLstatement;
		private List<long> RecallNumList;

		///<summary></summary>
		public FormRpRecall(){
			InitializeComponent();
			InitializeLayoutManager();
			SQLselect="";
			SQLfrom="FROM patient "; 
			SQLwhere="WHERE ";
			FillPatientSelectList(); 
			Lan.F(this); 
		}

		///<summary></summary>
		public FormRpRecall(List<long> recallNumList) {
			InitializeComponent();
			InitializeLayoutManager();
			RecallNumList=new List<long>(recallNumList);
			SQLselect="";
			SQLfrom="FROM patient ";
			SQLwhere="WHERE ";
			FillPatientSelectList();
		} 
 
		private void FillPatientSelectList(){ 
			listPatientSelect.Items.Add(Lan.g(this,"LName"));
			listPatientSelect.Items.Add(Lan.g(this,"FName"));
			listPatientSelect.Items.Add(Lan.g(this,"MiddleI"));
			listPatientSelect.Items.Add(Lan.g(this,"Preferred"));
			listPatientSelect.Items.Add(Lan.g(this,"Salutation"));
			listPatientSelect.Items.Add(Lan.g(this,"Address")); 
			listPatientSelect.Items.Add(Lan.g(this,"Address2"));
			listPatientSelect.Items.Add(Lan.g(this,"City")); 
			listPatientSelect.Items.Add(Lan.g(this,"State"));
			listPatientSelect.Items.Add(Lan.g(this,"Zip"));
			listPatientSelect.Items.Add(Lan.g(this,"HmPhone"));
			listPatientSelect.Items.Add(Lan.g(this,"WkPhone")); 
			listPatientSelect.Items.Add(Lan.g(this,"WirelessPhone"));
			listPatientSelect.Items.Add(Lan.g(this,"Birthdate"));
			listPatientSelect.Items.Add(Lan.g(this,"RecallStatus"));
			listPatientSelect.Items.Add(Lan.g(this,"DateDue"));
			listPatientSelect.SetAll(true);
			listPatientSelect2.Items.Add(Lan.g(this,"BillingType"));
			listPatientSelect2.Items.Add(Lan.g(this,"CreditType"));
			listPatientSelect2.Items.Add(Lan.g(this,"SSN"));
			listPatientSelect2.Items.Add(Lan.g(this,"ChartNumber"));
			listPatientSelect2.Items.Add(Lan.g(this,"FeeSched"));
			listPatientSelect2.Items.Add(Lan.g(this,"Position")); 
			listPatientSelect2.Items.Add(Lan.g(this,"Gender"));
			listPatientSelect2.Items.Add(Lan.g(this,"PatStatus"));
			listPatientSelect2.Items.Add(Lan.g(this,"PatNum")); 
			listPatientSelect2.Items.Add(Lan.g(this,"Email"));
			listPatientSelect2.Items.Add(Lan.g(this,"EstBalance")); 
			listPatientSelect2.Items.Add(Lan.g(this,"AddrNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"FamFinUrgNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"MedUrgNote"));
			listPatientSelect2.Items.Add(Lan.g(this,"ApptModNote"));
			//listPatientSelect2.Items.Add(Lan.g(this,"PriPlanNum"));//Primary Carrier?
			//listPatientSelect2.Items.Add(Lan.g(this,"PriRelationship"));// ?
			//listPatientSelect2.Items.Add(Lan.g(this,"SecPlanNum"));//Secondary Carrier? 
			//listPatientSelect2.Items.Add(Lan.g(this,"SecRelationship"));// ?
			listPatientSelect2.Items.Add(Lan.g(this,"RecallInterval"));
			listPatientSelect2.Items.Add(Lan.g(this,"StudentStatus"));
			listPatientSelect2.Items.Add(Lan.g(this,"SchoolName"));
			listPatientSelect2.Items.Add(Lan.g(this,"PriProv"));
			listPatientSelect2.Items.Add(Lan.g(this,"SecProv"));
			//listPatientSelect2.Items.Add(Lan.g(this,"NextAptNum"));
			listPatientSelect2.Items.Add(Lan.g(this,"Guarantor"));
			listPatientSelect2.Items.Add(Lan.g(this,"ImageFolder"));
		}

		private void FillSQLstatement(){
			SQLstatement=SQLselect+SQLfrom+SQLwhere;
		}

	private void CreateSQL(){
		CreateSQLselect();
		CreateSQLfrom();
		CreateSQLwhere();
		FillSQLstatement();
	}

	private void CreateSQLselect(){
		SQLselect="";
		if(listPatientSelect.SelectedIndices.Count==0 && listPatientSelect2.SelectedIndices.Count==0){
				butOK.Enabled=false;
				return;
			}
			SQLselect="SELECT ";
			List<string> PatFieldsSelected=new List<string>();
			for(int i=0;i<listPatientSelect.SelectedIndices.Count;i++) {
				PatFieldsSelected.Add(listPatientSelect.Items.GetTextShowingAt(listPatientSelect.SelectedIndices[i]));
			}
			for(int i=0;i<listPatientSelect2.SelectedIndices.Count;i++) {
				PatFieldsSelected.Add(listPatientSelect2.Items.GetTextShowingAt(listPatientSelect2.SelectedIndices[i]));
			}
			for(int i=0;i<PatFieldsSelected.Count;i++){
				if(i>0){
					SQLselect+=",";
				}
				if(PatFieldsSelected[i]=="RecallInterval"){
					SQLselect+="ROUND(recall.RecallInterval/65536) AS MonthInterval";
					//returns the months.  It will malfunction if a year is present
				}
				else if(PatFieldsSelected[i]=="DateDue"
					|| PatFieldsSelected[i]=="RecallStatus"){
					SQLselect+="recall."+PatFieldsSelected[i];
				}
				else{
					SQLselect+="patient."+PatFieldsSelected[i];
				}
			}
			butOK.Enabled=true;
		}

	private void CreateSQLfrom(){ 
	  SQLfrom=" FROM patient,recall ";
	}

	private void CreateSQLwhere() {
	  SQLwhere="WHERE patient.PatNum=recall.PatNum ";
			if(RecallNumList.Count>0) {
				SQLwhere+=$"AND recall.RecallNum IN({string.Join(",",RecallNumList.Select(x => POut.Long(x)))})";
			}
	}

		private void ListPatientSelect_SelectedIndexChanged(object sender, System.EventArgs e) {
			CreateSQL();
		}

		private void listPatientSelect2_SelectedIndexChanged(object sender, System.EventArgs e) {
			CreateSQL();
		}

		private void butOK_Click(object sender, System.EventArgs e) {
			ReportSimpleGrid report=new ReportSimpleGrid();
			report.Query=SQLstatement;
			using FormQuery FormQuery2=new FormQuery(report);
			FormQuery2.IsReport=false;
			FormQuery2.SubmitQuery();	
			FormQuery2.textQuery.Text=report.Query;
			FormQuery2.ShowDialog();
			DialogResult=DialogResult.OK; 
		}

		

	}
}
